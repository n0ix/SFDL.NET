Imports System.Text.RegularExpressions

Module UnRARHelper
    Function isUnRarChainComplete(ByVal _chain As UnRARChain) As Boolean

        Dim _log As NLog.Logger = NLog.LogManager.GetLogger("isUnRarChainComplete")

        Dim _rt As Boolean = True

        Try

            If Not IO.File.Exists(_chain.MasterUnRarChainFile.LocalFile) Then
                _rt = False
            Else
                If Not New IO.FileInfo(_chain.MasterUnRarChainFile.LocalFile).Length.Equals(_chain.MasterUnRarChainFile.FileSize) Then
                    _rt = False
                End If
            End If

            For Each _chainmember As DownloadItem In _chain.ChainMemberFiles

                If Not IO.File.Exists(_chainmember.LocalFile) Then
                    _rt = False
                Else
                    If Not New IO.FileInfo(_chainmember.LocalFile).Length.Equals(_chainmember.FileSize) Then
                        _rt = False
                    End If
                End If

            Next


        Catch ex As Exception
            _log.Error(ex, ex.Message)
            _rt = False
        End Try

        Return _rt


    End Function

    Friend Function ParseUnRarProgress(ByVal _line As String) As String

        Dim _percent As String = String.Empty

        Dim re As Regex = New Regex("[0-9]{1,3}%")
        Dim mc As Match = re.Match(_line.Trim)

        If mc.Success Then
            _line = mc.Value
            _percent = _line.TrimEnd("%")
        Else
            _percent = String.Empty

        End If

        Return _percent

    End Function

    Friend Function ParseUnRARVolumeFiles(ByVal _line As String) As String

        Dim _volume As String = String.Empty
        Dim sourcestring As String = _line

        If _line.Contains(".rar") Or _line.Contains(".part") Then

            Dim re As Regex = New Regex("^Extracting[ ]{1,2}.{1,255}\..{1,3}")
            Dim mc As MatchCollection = re.Matches(sourcestring)
            Dim mIdx As Integer = 0

            For Each m As Match In mc
                For groupIdx As Integer = 0 To m.Groups.Count - 1
                    If Not String.IsNullOrEmpty(m.Value.ToString) Then
                        _volume = m.Value.ToString.Replace("Extracting from", "").Trim
                    End If
                Next
                mIdx = mIdx + 1
            Next

        End If

        Return _volume

    End Function

    Friend Function ParseUnRARArchiveFiles(ByVal _line As String) As String

        Dim _archive_file As String = String.Empty
        Dim sourcestring As String = _line

        If Not _line.StartsWith("Extracting from") Then

            Dim re As Regex = New Regex("^Extracting[ ]{1,2}.{1,255}\..{1,3}")
            Dim mc As MatchCollection = re.Matches(sourcestring)
            Dim mIdx As Integer = 0

            For Each m As Match In mc
                For groupIdx As Integer = 0 To m.Groups.Count - 1
                    If Not String.IsNullOrEmpty(m.Value.ToString) Then
                        _archive_file = m.Value.ToString.Replace("Extracting", "").Trim
                    End If
                Next
                mIdx = mIdx + 1
            Next

        End If

        Return _archive_file


    End Function

    Friend Function ParsecRARkPassword(ByVal _raw_output As String) As String

        Dim _password As String = String.Empty

        Dim sourcestring As String = _raw_output
        Dim re As Regex = New Regex(".+- CRC OK")
        Dim mc As MatchCollection = re.Matches(sourcestring)
        Dim mIdx As Integer = 0

        For Each m As Match In mc
            For groupIdx As Integer = 0 To m.Groups.Count - 1
                If Not String.IsNullOrEmpty(m.Value.ToString) Then
                    _password = m.Value.ToString
                End If
            Next
            mIdx = mIdx + 1
        Next

        _password = _password.Replace(" - CRC OK", "")

        _password = _password.Trim(ChrW(7))

        Return _password.Trim

    End Function

    Private Function CrackUnRARPassword(ByVal _filename As String) As CrackUnRARPasswordResult

        Dim _crark_process As Process
        Dim _crark_bin As String = String.Empty
        Dim _result As New CrackUnRARPasswordResult
        Dim _log As NLog.Logger = NLog.LogManager.GetLogger("CrackUnRARPassword")
        Dim _crark_raw_output As String = String.Empty

        Try

            If Environment.Is64BitOperatingSystem Then
                _crark_bin = IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "bin", "cRARk_x64.exe")
            Else
                _crark_bin = IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "bin", "cRARk_x86.exe")
            End If

            If IO.File.Exists(_crark_bin) = False Then
                Throw New Exception("cRARk Executable Is missing!")
            End If

            _crark_process = New Process

            With _crark_process.StartInfo

                .CreateNoWindow = True
                .FileName = _crark_bin
                .WorkingDirectory = IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "bin")
                .RedirectStandardOutput = True
                .RedirectStandardInput = True
                .UseShellExecute = False

                .Arguments = String.Format("-p{0}{1}{2} {3}{4}{5}", Chr(34), IO.Path.Combine(Environment.GetEnvironmentVariable("appdata"), "SFDL.NET 3", "sfdl_passwords.def"), Chr(34), Chr(34), _filename, Chr(34))

            End With

            _log.Info(String.Format("Starting cRARk.exe with the following parameters: {0}", _crark_process.StartInfo.Arguments))

            _crark_process.Start()

            _crark_process.WaitForExit(TimeSpan.FromSeconds(60).TotalMilliseconds)

            _crark_raw_output = _crark_process.StandardOutput.ReadToEnd

            If _crark_raw_output.ToLower.Contains("is not rar") Then
                Throw New Exception("Not a vaid RAR File!")
            End If

            If _crark_raw_output.ToLower.Contains("not encrypted") Then

                _result.PasswordNeeded = False
                _result.PasswordFound = True
                _result.Password = String.Empty

            Else

                If _crark_raw_output.ToLower.Contains("crc ok") Then
                    _result.PasswordFound = True
                    _result.PasswordNeeded = True
                    _result.Password = ParsecRARkPassword(_crark_raw_output)
                End If

                If _crark_raw_output.ToLower.Contains("poassword not found") Then
                    _result.PasswordFound = False
                    _result.PasswordNeeded = True
                    _result.Password = String.Empty
                End If

            End If

        Catch ex As Exception
            _result.ErrorOccured = True
            _result.ErrorMessage = ex.Message
            _log.Error(ex, ex.Message)
        End Try

        Return _result

    End Function

    Private Function DoUnRAR(ByVal _filename As String, ByVal _extract_dir As String, ByVal _password As String, ByVal _app_task As AppTask) As Boolean

        Dim _result As Boolean = False
        Dim _unrar_process As Process
        Dim _unrar_exe As String
        Dim _tmp_output As String
        Dim _out_lines As New Text.StringBuilder
        Dim _log As NLog.Logger = NLog.LogManager.GetLogger("DoUnRAR")
        Dim _exitcode As Int32

        Dim _line As String
        Dim _percent1 As String

        Try

            _unrar_exe = IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "bin", "7z.exe")

            If IO.File.Exists(_unrar_exe) = False Then
                Throw New Exception("UnRAR Executable Is missing!")
            End If

            _unrar_process = New Process

            With _unrar_process.StartInfo

                .CreateNoWindow = True
                .FileName = _unrar_exe
                .WorkingDirectory = System.AppDomain.CurrentDomain.BaseDirectory
                .RedirectStandardOutput = True
                .UseShellExecute = False
                .StandardOutputEncoding = Text.Encoding.UTF8

                If String.IsNullOrWhiteSpace(_password) Then
                    .Arguments = String.Format("x -aos -bso1 -bse1 -bsp1 -bt -p -o{0} {1}", Chr(34) & _extract_dir & Chr(34), Chr(34) & _filename & Chr(34))
                Else
                    .Arguments = String.Format("x -aos -bso0 -bse1 -bsp1 -p{0} -o{1} {2}", Chr(34) & _password & Chr(34), Chr(34) & _extract_dir & Chr(34), Chr(34) & _filename & Chr(34))
                End If

            End With

            _log.Info("UnRAR Parameters:   " & _unrar_process.StartInfo.Arguments.ToString)

            _unrar_process.Start()

            '_unrar_process.BeginOutputReadLine()

            While _unrar_process.HasExited = False

                Try

                    _line = _unrar_process.StandardOutput.ReadLine
                    _out_lines.AppendLine(_line)
                    ' _log.Debug(_line)
                    _percent1 = ParseUnRarProgress(_line.Trim)

                    If Not _percent1 = String.Empty Then
                        '_log.Debug("{0}__% entpackt", _percent1)
                        _app_task.SetTaskStatus(TaskStatus.Running, String.Format(My.Resources.Strings.UnRAR_AppTask_Status_1_Message, _filename, _percent1))
                    End If

                Catch ex As Exception
                    _log.Warn(ex, ex.Message)
                End Try

            End While

            _unrar_process.WaitForExit()
            _exitcode = _unrar_process.ExitCode
            _tmp_output = _out_lines.ToString
            _tmp_output = _tmp_output & _unrar_process.StandardOutput.ReadToEnd
            _tmp_output = _tmp_output.Trim

            If _exitcode = 0 Then
                _log.Info("UnRAR Process has exited with Code=" & _exitcode)
                _log.Info(_tmp_output)
                _result = True

            ElseIf _exitcode = 1 Then
                _log.Info("UnRAR Process has exited with Code=" & _exitcode)

                _result = False

            ElseIf _exitcode = 2 Then
                _log.Info("UnRAR Process has exited with Code=" & _exitcode)

                _result = False

            ElseIf _exitcode = 8 Then
                _log.Info("UnRAR Process has exited with Code=" & _exitcode)

                _result = False

            Else
                _log.Info("UnRAR Process has exited with Code=" & _exitcode)
                Throw New Exception("UnRAR Process has exited with Code=" & _exitcode)

            End If


        Catch ex As Exception
            _log.Error(ex, ex.Message)
            _result = False
        End Try

        Return _result



    End Function

    Public Function UnRAR(ByVal _unrarchain As UnRARChain, ByVal _app_task As AppTask, ByVal _unrar_settings As UnRARSettings) As Boolean

        Dim _crark_result As New CrackUnRARPasswordResult
        Dim _unrar_password As String = String.Empty
        Dim _log As NLog.Logger = NLog.LogManager.GetLogger("UnRAR")
        Dim _rt As Boolean = True

        _log.Info("Cracking UnRar Password...")

        Try

            _app_task.SetTaskStatus(TaskStatus.Running, String.Format(My.Resources.Strings.UnRAR_AppTask_Status_3_Message, IO.Path.GetFileName(_unrarchain.MasterUnRarChainFile.LocalFile)))

            _crark_result = CrackUnRARPassword(_unrarchain.MasterUnRarChainFile.LocalFile)

            If (_unrar_settings.UseUnRARPasswordList = False Or _unrar_settings.UnRARPasswordList.Count = 0) And _crark_result.PasswordNeeded = True Then
                Throw New Exception(My.Resources.Strings.UnRAR_Exception_NoPasswordsInList)
            End If

            If _crark_result.ErrorOccured = True Then
                Throw New Exception(My.Resources.Strings.UnRAR_Exception_CrackPasswordFailed)
            End If

            If _crark_result.PasswordNeeded = True And _crark_result.PasswordFound = False Then
                Throw New Exception(My.Resources.Strings.UnRAR_Exception_NoValidPasswordFound)
            End If

            _log.Info("Now passing all needed Arguments to UnRar Binary and wait the extraction to finish")

            If DoUnRAR(_unrarchain.MasterUnRarChainFile.LocalFile, IO.Path.GetDirectoryName(_unrarchain.MasterUnRarChainFile.LocalFile), _crark_result.Password, _app_task) = True Then

                If _unrar_settings.DeleteAfterUnRAR = True Then

                    Try

                        _app_task.SetTaskStatus(TaskStatus.Running, My.Resources.Strings.UnRAR_AppTask_Status_2_Message)

                        For Each _file In _unrarchain.ChainMemberFiles
                            IO.File.Delete(_file.LocalFile)
                        Next

                        IO.File.Delete(_unrarchain.MasterUnRarChainFile.LocalFile)

                        _app_task.SetTaskStatus(TaskStatus.RanToCompletion, String.Format(My.Resources.Strings.UnRAR_AppTask_Completed_1_Message, IO.Path.GetFileName(_unrarchain.MasterUnRarChainFile.LocalFile)))

                    Catch ex As Exception
                        _log.Error(ex, ex.Message)
                        _app_task.SetTaskStatus(TaskStatus.Faulted, String.Format(My.Resources.Strings.UnRAR_AppTask_Faulted_1_Message, IO.Path.GetFileName(_unrarchain.MasterUnRarChainFile.LocalFile)))
                    End Try

                Else
                    _app_task.SetTaskStatus(TaskStatus.RanToCompletion, String.Format(My.Resources.Strings.UnRAR_AppTask_Completed_2_Message, IO.Path.GetFileName(_unrarchain.MasterUnRarChainFile.LocalFile)))
                End If

            Else
                Throw New Exception(My.Resources.Strings.UnRAR_AppTask_Faulted_2_Message)
            End If

        Catch ex As Exception
            _log.Error(ex, ex.Message)
            _app_task.SetTaskStatus(TaskStatus.Faulted, ex.Message)
            _rt = False
        End Try

        Return _rt

    End Function

End Module
