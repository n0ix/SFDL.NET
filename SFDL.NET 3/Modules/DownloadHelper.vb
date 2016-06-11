﻿
Imports System.Text

Class DownloadHelper

    Private _log As NLog.Logger = NLog.LogManager.GetLogger("DownloadHelper")
    Private _ftp_client_list As New Dictionary(Of Guid, ArxOne.Ftp.FtpClient)
    Private _obj_ftp_client_list_lock As New Object

    Public Event ItemDownloadComplete(ByVal _item As DownloadItem)

#Region "Private Subs"

    Private Sub GetItemFileSize(ByRef _item As DownloadItem, ByVal _ftp_client As ArxOne.Ftp.FtpClient)

        Try

            For Each _ftpitem In ArxOne.Ftp.FtpClientUtility.List(_ftp_client, _item.DirectoryPath)

                Try

                    Dim _entry As ArxOne.Ftp.FtpEntry

                    _entry = FTPHelper.TryParseLine(_ftpitem, _item.DirectoryPath)

                    If _entry.Name.Equals(_item.FileName) Then
                        _item.FileSize = _entry.Size
                    End If

                Catch ex As Exception
                    _log.Debug(ex.Message)
                End Try

            Next

        Catch ex As Exception
            _log.Error(ex.Message)
        End Try

    End Sub

    Private Sub ParseFTPException(ByVal ex As ArxOne.Ftp.Exceptions.FtpException, ByVal _item As DownloadItem)

        'Select Case ex.InnerException

        '    Case "421" ' Service not available, closing control connection. This may be a reply to any command if the service knows it must shut down.

        '        If Exception.Message.ToString.ToLower.Contains("maximum login limit has been reached.") Then
        '            UpdateItem(_ftp_thread_info.FileItem, UpdateItemEventType.ServerFull)
        '        Else
        '            UpdateItem(_ftp_thread_info.FileItem, UpdateItemEventType.ServerDown)
        '        End If

        '    Case "425" ' Can't open data connection.

        '        UpdateItem(_ftp_thread_info.FileItem, UpdateItemEventType.ConnectionError)

        '    Case "426" 'Connection closed; transfer aborted.

        '                    'Download Stopped

        '    Case "430" ' Invalid username or password

        '        UpdateItem(_ftp_thread_info.FileItem, UpdateItemEventType.AuthError)

        '    Case "434" 'Requested host unavailable.

        '        UpdateItem(_ftp_thread_info.FileItem, UpdateItemEventType.ServerDown)

        '    Case "450" 'Requested file action not taken.

        '        UpdateItem(_ftp_thread_info.FileItem, UpdateItemEventType.FileNotFound)

        '    Case "451" 'Requested action aborted. Local error in processing

        '        UpdateItem(_ftp_thread_info.FileItem, UpdateItemEventType.InternalServerError)

        '    Case "452"

        '        UpdateItem(_ftp_thread_info.FileItem, UpdateItemEventType.InternalServerError)

        '    Case "501" 'Syntax error in parameters or arguments.

        '        UpdateItem(_ftp_thread_info.FileItem, UpdateItemEventType.DirectoryNotFound)

        '    Case "502" 'Command not implemented.

        '        UpdateItem(_ftp_thread_info.FileItem, UpdateItemEventType.InternalServerError)

        '    Case "503" 'Bad sequence of commands.

        '        UpdateItem(_ftp_thread_info.FileItem, UpdateItemEventType.InternalServerError)

        '    Case "504" 'Command not implemented for that parameter

        '        UpdateItem(_ftp_thread_info.FileItem, UpdateItemEventType.InternalServerError)

        '    Case "530" 'Not logged in.

        '        UpdateItem(_ftp_thread_info.FileItem, UpdateItemEventType.AuthError)

        '    Case "550" 'Requested action not taken. File unavailable (e.g., file not found, no access).

        '        UpdateItem(_ftp_thread_info.FileItem, UpdateItemEventType.FileNotFound)

        '    Case "553" ' Requested action not taken. File name not allowed.

        '        UpdateItem(_ftp_thread_info.FileItem, UpdateItemEventType.FileNotFound)

        '    Case Else

        '        UpdateItem(_ftp_thread_info.FileItem, UpdateItemEventType.UnknownError)

        'End Select

    End Sub


#End Region

    Sub DisposeFTPClients()

        For Each _client In _ftp_client_list
            _client.Value.Dispose()
        Next

    End Sub

    Sub DownloadContainerItems(items As List(Of DownloadItem), ByVal _download_dir As String, ByVal _connection_info As SFDL.Container.Connection, ByVal _ct As System.Threading.CancellationToken)

        Dim _tasks = New List(Of System.Threading.Tasks.Task)

        Try

            For Each _item As DownloadItem In items

                Dim _dl_task As System.Threading.Tasks.Task
                Dim _ftp_client As ArxOne.Ftp.FtpClient

                SyncLock _obj_ftp_client_list_lock

                    'Check if any FTP Client Exits for this Parent Container Session
                    If Not _ftp_client_list.ContainsKey(_item.ParentContainerID) Then
                        SetupFTPClient(_ftp_client, _connection_info)
                        _ftp_client_list.Add(_item.ParentContainerID, _ftp_client)
                    Else
                        _ftp_client = _ftp_client_list(_item.ParentContainerID)
                    End If

                End SyncLock

                'ToDo: Prüfen ob Verbindung zum Server hergestellt werden kann ->> Fehlerbehandlung

                _dl_task = System.Threading.Tasks.Task.Run(Sub()
                                                               DownloadItem(_item, _ftp_client, _ct)
                                                           End Sub)
                _tasks.Add(_dl_task)

            Next

            System.Threading.Tasks.Task.WhenAll(_tasks).Wait()

        Catch ex As Exception
            _log.Error(ex, ex.Message)
        End Try


    End Sub

    Private Sub DownloadItem(ByVal _item As DownloadItem, ByVal _ftp_client As ArxOne.Ftp.FtpClient, ByVal _ct As System.Threading.CancellationToken)

        Dim _settings As New Settings
        Dim _filemode As IO.FileMode
        Dim _restart As Long = 0
        Dim _disk_free_space As Long

        'IO Stream Variablen
        Const Length As Integer = 256
        Dim buffer As [Byte]()
        Dim bytesRead As Integer = 0
        Dim bytestotalread As Integer = 0
        Dim _starttime As DateTime = DateTime.Now

        Dim _percent_downloaded As Integer = 0
        Dim _current As Long = 0
        Dim _ctime As TimeSpan
        Dim elapsed As TimeSpan
        Dim bytesPerSec As Integer = 0

        _settings = Application.Current.Resources("Settings")

        Try

            If String.IsNullOrWhiteSpace(_item.LocalFile) Then
                Throw New Exception("Dateipfad ist leer!")
            End If

            If _item.LocalFile.Length >= 255 Then
                Throw New FileNameTooLongException("Dateipfad ist zu lang! - Kann Datei nicht schreiben!")
            End If

            GetItemFileSize(_item, _ftp_client)

            _disk_free_space = My.Computer.FileSystem.GetDriveInfo(IO.Path.GetPathRoot(_settings.DownloadDirectory)).AvailableFreeSpace

            _log.Info("Freier Speicherplatz: {0}", _disk_free_space)

            If _item.FileSize > _disk_free_space Then
                Throw New Exception("Zu wenig Speicherplatz!")
            End If

            If _settings.ExistingFileHandling = ExistingFileHandling.ResumeFile And IO.File.Exists(_item.LocalFile) Then
                _filemode = IO.FileMode.Append
                _restart = New IO.FileInfo(_item.LocalFile).Length
            Else
                _filemode = IO.FileMode.Create
            End If

            If _filemode = (IO.FileMode.Append And _item.FileSize.Equals(New IO.FileInfo(_item.LocalFile).Length)) And Not _item.FileSize = 0 Then
                _log.Info("Datei ist bereits vollständig - Überspringe FTP Connect!")
                _item.SizeDownloaded = _item.FileSize
            Else

                Using _ftp_read_stream = ArxOne.Ftp.FtpClientUtility.Retr(_ftp_client, New ArxOne.Ftp.FtpPath(_item.FullPath), ArxOne.Ftp.FtpTransferMode.Binary, _restart)

                    buffer = New Byte(8192) {}
                    bytesRead = _ftp_read_stream.Read(buffer, 0, buffer.Length)

                    Using _local_write_stream As New IO.FileStream(_item.LocalFile, _filemode, IO.FileAccess.Write, IO.FileShare.None, 8192, False)

                        While bytesRead > 0 And _ct.IsCancellationRequested = False

                            Dim _tmp_percent_downloaded As Double = 0
                            Dim _new_perc As Integer = 0
                            Dim _download_speed As String = String.Empty

                            _local_write_stream.Write(buffer, 0, bytesRead)

                            bytesRead = _ftp_read_stream.Read(buffer, 0, Length)
                            bytestotalread += bytesRead

                            elapsed = DateTime.Now.Subtract(_starttime)
                            bytesPerSec = CInt(If(elapsed.TotalSeconds < 1, bytestotalread, bytestotalread / elapsed.TotalSeconds))

#Region "Berechnung Download Speed / Fortschritt"

                            _tmp_percent_downloaded = CDbl(_local_write_stream.Position) / CDbl(_item.FileSize)
                            _new_perc = CInt(_tmp_percent_downloaded * 100)

                            If _new_perc <> _percent_downloaded Then 'Nicht jedesmal Updaten

                                Dim _tmp_speed As Double

                                _percent_downloaded = _new_perc

                                _current = bytestotalread
                                _ctime = DateTime.Now.Subtract(_starttime)

                                _tmp_speed = Math.Round(bytesPerSec / 1024, 2)

                                If _tmp_speed >= 1024 Then
                                    _download_speed = Math.Round(_tmp_speed / 1024, 2) & " MB/s"
                                Else
                                    _download_speed = _tmp_speed & " KB/s"
                                End If

                                _item.DownloadSpeed = _download_speed
                                _item.DownloadProgress = _percent_downloaded
                                _item.SizeDownloaded = bytestotalread

                            End If

#End Region

                            'ThrottleByteTransfer(_max_bytes_per_second, bytestotalread, _ctime, bytesPerSec)

                        End While

                    End Using

                End Using

            End If

            _item.DownloadStatus = NET3.DownloadItem.Status.Completed

        Catch ex As NotEnoughFreeDiskSpaceException
            _log.Error(ex, ex.Message)
            _item.DownloadStatus = NET3.DownloadItem.Status.Failed_NotEnoughDiskSpace

        Catch ex As FileNameTooLongException
            _log.Warn(ex, ex.Message)
            _item.DownloadStatus = NET3.DownloadItem.Status.Failed_FileNameTooLong

        Catch ex As ArxOne.Ftp.Exceptions.FtpException

            If Application.Current.Resources("DownloadStopped") = True Then
                _log.Info("Download wurde gestoppt!")
                _item.DownloadStatus = NET3.DownloadItem.Status.Stopped
            Else
                _log.Error(ex, ex.Message)
                ParseFTPException(ex, _item)
            End If

            'If ex.InnerException.Message.Contains("Maximum login limit has been reached") Then
            '    _item.DownloadStatus = NET3.DownloadItem.Status.Failed_ServerFull
            'Else
            '    _item.DownloadStatus = NET3.DownloadItem.Status.Failed
            'End If


        Catch ex As Exception
            If Application.Current.Resources("DownloadStopped") = True Then
                _log.Info("Download wurde gestoppt!")
                _item.DownloadStatus = NET3.DownloadItem.Status.Stopped
            Else
                _log.Error(ex, ex.Message)
                ParseFTPException(ex, _item)
            End If

        Finally
            PostDownload(_item, _ftp_client)
        End Try

    End Sub

    Private Sub PostDownload(ByRef _item As DownloadItem, ByVal _ftp_client As ArxOne.Ftp.FtpClient)

        Dim _hashcommand As String = String.Empty
        Dim _reply As ArxOne.Ftp.FtpReply
        Dim _tmp_hash As String = String.Empty
        Dim _hashtype As Container.HashType

        Try

            _item.DownloadSpeed = String.Empty

            If Application.Current.Resources("DownloadStopped") = False And _item.DownloadStatus = NET3.DownloadItem.Status.Completed Then

                If _item.HashType = Container.HashType.None Then

                    If _ftp_client.ServerFeatures.HasFeature("MD5") Then
                        _hashcommand = "MD5"
                        _hashtype = Container.HashType.MD5
                    End If

                    If _ftp_client.ServerFeatures.HasFeature("XMD5") Then
                        _hashcommand = "XMD5"
                        _hashtype = Container.HashType.MD5
                    End If

                    If _ftp_client.ServerFeatures.HasFeature("XSHA1") Then
                        _hashcommand = "XSHA1"
                        _hashtype = Container.HashType.SHA1
                    End If

                    If _ftp_client.ServerFeatures.HasFeature("XCRC") Then
                        _hashcommand = "XCRC"
                        _hashtype = Container.HashType.CRC
                    End If

                    If Not String.IsNullOrWhiteSpace(_hashcommand) Then
                        _log.Info("Server Support Hash Alogrightm {0}", _hashcommand)

                        _reply = _ftp_client.Session.SendCommand(_hashcommand, _item.FullPath)

                        If _reply.Code.IsSuccess = True Then

                            _log.Info("Hash Serverseitig erfolgreich ermittelt!")

                            _item.HashType = _hashtype
                            _tmp_hash = _reply.Lines(0).ToString.Replace(_item.FullPath, "")
                            _tmp_hash = _tmp_hash.Replace(Chr(34), "")
                            _item.FileHash = _tmp_hash.Trim
                        Else
                            _log.Error("Hash konnte nicht ermittelt werden!")
                        End If

                    Else
                        _log.Info("Server does not Support any Hash Algorithm")
                    End If


                Else
                    _log.Info("Download Item has already an Hash (provided via SFDL Container)")
                End If

                Select Case _item.HashType

                    Case Container.HashType.None

                        _item.DownloadStatus = NET3.DownloadItem.Status.Completed

                    Case Container.HashType.MD5

                        _log.Info("Prüfe ob MD5 Hashes übereinstimmen")

                        If HashHelper.MD5FileHash(_item.LocalFile).ToLower.Equals(_item.FileHash.ToLower) Then
                            _log.Info("MD5 Hash is Valid!")
                            _item.DownloadStatus = NET3.DownloadItem.Status.Completed_HashValid
                        Else
                            _log.Info("MD5 Hash Invalid")
                            _item.DownloadStatus = NET3.DownloadItem.Status.Completed_HashInvalid
                        End If

                    Case Container.HashType.SHA1

                        _log.Info("Prüfe ob SHA1 Hashes übereinstimmen")

                        If HashHelper.SHA1FileHash(_item.LocalFile).ToLower.Equals(_item.FileHash.ToLower) Then
                            _log.Info("SHA1 Hash is Valid!")
                            _item.DownloadStatus = NET3.DownloadItem.Status.Completed_HashValid
                        Else
                            _log.Info("SHA1 Hash Invalid")
                            _item.DownloadStatus = NET3.DownloadItem.Status.Completed_HashInvalid
                        End If

                    Case Container.HashType.CRC

                        _log.Info("Prüfe ob CRC Hashes übereinstimmen")

                        If HashHelper.CRC32FileHash(_item.LocalFile).ToLower.Equals(_item.FileHash.ToLower) Then
                            _log.Info("CRC Hash is Valid!")
                            _item.DownloadStatus = NET3.DownloadItem.Status.Completed_HashValid
                        Else
                            _log.Info("CRC Hash Invalid")
                            _item.DownloadStatus = NET3.DownloadItem.Status.Completed_HashInvalid
                        End If

                End Select

            Else
                _log.Info("Download wurde gestoppt - Überspringe Hash Check")
            End If

        Catch ex As Exception
            _log.Error(ex.Message)
            _item.DownloadStatus = NET3.DownloadItem.Status.Failed
        Finally
            RaiseEvent ItemDownloadComplete(_item)
        End Try


    End Sub

End Class
