﻿Imports Amib.Threading

Class DownloadHelper
    Implements IDisposable

    Private _log As NLog.Logger = NLog.LogManager.GetLogger("DownloadHelper")
    Private _ftp_client_collection As New Dictionary(Of String, ArxOne.Ftp.FtpClient)
    Private _ftp_session_collection As New Dictionary(Of String, List(Of ArxOne.Ftp.FtpSession))
    Private _settings As New Settings
    Private _obj_ftp_client_lock As New Object
    Private _obj_dl_count_lok As New Object
    Private _obj_session_dic_lock As New Object
    Private _dl_count As Integer = 0
    Private _ftp_session_count As Integer = 0
    Private _post_stp As SmartThreadPool


    Public Sub New()
        _settings = CType(Application.Current.Resources("Settings"), Settings)
        _post_stp = New SmartThreadPool
        _post_stp.MaxThreads = _settings.MaxChecksumThreads
    End Sub

    Public Event ServerFull(ByVal _item As DownloadItem)

#Region "Private Subs"

    Private Function IsDiskFull(ex As Exception) As Boolean
        Const HR_ERROR_HANDLE_DISK_FULL As Integer = CInt(112) '0x80070027
        Const HR_ERROR_DISK_FULL As Integer = CInt(-2147024784) '0x80070070

        Return ex.HResult = HR_ERROR_HANDLE_DISK_FULL OrElse ex.HResult = HR_ERROR_DISK_FULL
    End Function

    Private Sub ThrottleByteTransfer(maxBytesPerSecond As Long, bytesTotal As Long, elapsed As TimeSpan, bytesPerSec As Integer)
        ' we only throttle if the maxBytesPerSecond is not zero (zero turns off the throttle)
        If maxBytesPerSecond > 0 Then
            ' we only throttle if our through-put is higher than what we want
            If bytesPerSec > maxBytesPerSecond Then
                Dim elapsedMilliSec As Double = If(elapsed.TotalSeconds = 0, elapsed.TotalMilliseconds, elapsed.TotalSeconds * 1000)

                ' need to calc a delay in milliseconds for the throttle wait based on how fast the 
                ' transfer is relative to the speed it needs to be
                Dim millisecDelay As Double = (bytesTotal / (maxBytesPerSecond / 1000) - elapsedMilliSec)

                ' can only sleep to a max of an Int32 so we need to check this since bytesTotal is a long value
                ' this should never be an issue but never say never
                If millisecDelay > 10000 Then
                    millisecDelay = 10000
                End If

                ' go to sleep
                System.Threading.Thread.Sleep(CInt(millisecDelay))
            End If
        End If
    End Sub

    Private Sub GetItemFileSize(ByRef _item As DownloadItem, ByVal _ftp_session As ArxOne.Ftp.FtpSession)

        Try

            For Each _ftpitem In ArxOne.Ftp.FtpClientUtility.List(_ftp_session.Connection.Client, _item.DirectoryPath, _ftp_session)

                Try

                    Dim _entry As ArxOne.Ftp.FtpEntry

                    _entry = CType(FTPHelper.TryParseLine(_ftpitem, _item.DirectoryPath), ArxOne.Ftp.FtpEntry)

                    If _entry.Name.Equals(_item.FileName) Then
                        _item.FileSize = CLng(_entry.Size)
                    End If

                Catch ex As Exception
                    _log.Debug(ex.Message)
                End Try

            Next

        Catch ex As Exception
            _log.Error(ex.Message)
        End Try

    End Sub

    Private Sub ParseFTPException(ByVal ex As Object, ByVal _item As DownloadItem)

        Dim _err_message As String = String.Empty
        Dim _err_code As Integer = 0

#Region "FtpAuthenticationException"

        If ex.GetType Is GetType(ArxOne.Ftp.Exceptions.FtpAuthenticationException) Then

            Dim _myex As ArxOne.Ftp.Exceptions.FtpAuthenticationException

            _myex = TryCast(ex, ArxOne.Ftp.Exceptions.FtpAuthenticationException)

            If Not IsNothing(_myex.Code) Then
                _err_code = _myex.Code
            End If

            If Not IsNothing(_myex.Message) Then
                _err_message = _myex.Message
            End If

        End If

#End Region

#Region "FtpException"

        If ex.GetType Is GetType(ArxOne.Ftp.Exceptions.FtpException) Then

            Dim _myex As ArxOne.Ftp.Exceptions.FtpException

            _myex = TryCast(ex, ArxOne.Ftp.Exceptions.FtpException)

            If Not IsNothing(_myex.Message) Then
                _err_message = _err_message & _myex.Message
            End If

            If Not IsNothing(_myex.InnerException) AndAlso Not IsNothing(_myex.InnerException.Message) Then
                _err_message = _err_message & _myex.InnerException.Message
            End If

        End If

#End Region

#Region "FtpFileException"

        If ex.GetType Is GetType(ArxOne.Ftp.Exceptions.FtpFileException) Then

            Dim _myex As ArxOne.Ftp.Exceptions.FtpFileException

            _myex = TryCast(ex, ArxOne.Ftp.Exceptions.FtpFileException)

            If Not IsNothing(_myex.Code) Then
                _err_code = _myex.Code
            End If

            If Not IsNothing(_myex.Message) Then
                _err_message = _myex.Message
            End If

        End If

#End Region

#Region "FtpProtocolException"

        If ex.GetType Is GetType(ArxOne.Ftp.Exceptions.FtpProtocolException) Then

            Dim _myex As ArxOne.Ftp.Exceptions.FtpProtocolException

            _myex = TryCast(ex, ArxOne.Ftp.Exceptions.FtpProtocolException)

            If Not IsNothing(_myex.Code) Then
                _err_code = _myex.Code
            End If

            If Not IsNothing(_myex.Message) Then
                _err_message = _myex.Message
            End If

        End If

#End Region

#Region "FtpTransportException"

        If ex.GetType Is GetType(ArxOne.Ftp.Exceptions.FtpTransportException) Then

            Dim _myex As ArxOne.Ftp.Exceptions.FtpTransportException

            _myex = TryCast(ex, ArxOne.Ftp.Exceptions.FtpTransportException)

            If Not IsNothing(_myex.Message) Then
                _err_message = _myex.Message
            End If

            If Not IsNothing(_myex.InnerException) AndAlso Not IsNothing(_myex.InnerException.Message) Then
                _err_message = _err_message & _myex.InnerException.Message
            End If

        End If

#End Region

#Region "General Exception"

        If ex.GetType Is GetType(Exception) Then

            Dim _myex As Exception

            _myex = TryCast(ex, Exception)

            If Not IsNothing(_myex.Message) Then
                _err_message = _myex.Message
            End If

            If Not IsNothing(_myex.InnerException) AndAlso Not IsNothing(_myex.InnerException.Message) Then
                _err_message = _err_message & _myex.InnerException.Message
            End If

        End If

#End Region

        _item.DownloadStatus = NET3.DownloadItem.Status.Failed

        If Not _err_code = 0 Then

            If _err_code = 421 Then '  Can't open data connection.
                _item.DownloadStatus = NET3.DownloadItem.Status.Failed_ServerFull
                _item.RetryPossible = True
            End If

            If _err_code = 530 Then 'Not Logged in
                _item.DownloadStatus = NET3.DownloadItem.Status.Failed_AuthError
                _item.RetryPossible = True
            End If

            If _err_code = 430 Then ' Invalid username or password
                _item.DownloadStatus = NET3.DownloadItem.Status.Failed_AuthError
                _item.RetryPossible = True
            End If

            If _err_code = 425 Then '  Can't open data connection.
                _item.DownloadStatus = NET3.DownloadItem.Status.Failed_ConnectionError
                _item.RetryPossible = True
            End If

            If _err_code = 426 Then 'Connection closed; transfer aborted.
                _item.DownloadStatus = NET3.DownloadItem.Status.Failed_IOError
                _item.RetryPossible = True
            End If

            If _err_code = 434 Then 'Requested host unavailable.
                _item.DownloadStatus = NET3.DownloadItem.Status.Failed_ServerDown
                _item.RetryPossible = False
            End If

            If _err_code = 450 Then 'Requested file action not taken.
                _item.DownloadStatus = NET3.DownloadItem.Status.Failed_FileNotFound
                _item.RetryPossible = False
            End If

            If _err_code = 451 Then 'Requested action aborted. Local error in processing
                _item.DownloadStatus = NET3.DownloadItem.Status.Failed_FileNotFound
                _item.RetryPossible = False
            End If

            If _err_code = 452 Then 'Requested action aborted. Local error in processing
                _item.DownloadStatus = NET3.DownloadItem.Status.Failed_FileNotFound
                _item.RetryPossible = False
            End If

            If _err_code = 501 Then 'Syntax error in parameters or arguments.
                _item.DownloadStatus = NET3.DownloadItem.Status.Failed_DirectoryNotFound
                _item.RetryPossible = False
            End If

            If _err_code = 550 Then 'Requested action not taken. File unavailable (e.g., file not found, no access).
                _item.DownloadStatus = NET3.DownloadItem.Status.Failed_FileNotFound
                _item.RetryPossible = False
            End If

        Else

            If String.IsNullOrWhiteSpace(_err_message) = False Then

                If _err_message.ToLower.Contains("maximum login limit has been reached.") Then
                    _item.DownloadStatus = NET3.DownloadItem.Status.Failed_ServerFull
                    _item.RetryPossible = True
                End If

                If _err_message.ToLower.Contains("Not logged in, only sessions from same IP allowed concurrently") Then
                    _item.DownloadStatus = NET3.DownloadItem.Status.Failed_ServerFull
                    _item.RetryPossible = True
                End If

                If _err_message.ToLower.Contains("authentication failed") Then 'Invalid username or password or Server full
                    _item.DownloadStatus = NET3.DownloadItem.Status.Failed_ServerFull
                    _item.RetryPossible = True
                End If

                If _err_message.ToLower.Contains("io exception") Then 'General IO Exception - Mostly means that the server is full
                    _item.DownloadStatus = NET3.DownloadItem.Status.Failed_ServerFull
                    _item.RetryPossible = True
                End If

            End If

        End If



    End Sub

    Private Function AddFTPSessionToDic(ByVal _ftp_server_uid As String, ByVal _ftp_session As ArxOne.Ftp.FtpSession)

        Dim _current_list As New List(Of ArxOne.Ftp.FtpSession)
        Dim _rt As New List(Of ArxOne.Ftp.FtpSession)

        SyncLock _obj_session_dic_lock

            If _ftp_session_collection.ContainsKey(_ftp_server_uid) Then

                _current_list = _ftp_session_collection(_ftp_server_uid)

                If Not IsNothing(_current_list) Then
                    _current_list.Add(_ftp_session)
                    _rt = _current_list
                End If

            Else
                _current_list = New List(Of ArxOne.Ftp.FtpSession)
                _current_list.Add(_ftp_session)
                _rt = _current_list

            End If

        End SyncLock

        Return _rt

    End Function

    Private Function RemoveFTPSessionFromDic(ByVal _ftp_server_uid As String, ByVal _ftp_session As ArxOne.Ftp.FtpSession)

        Dim _current_list As New List(Of ArxOne.Ftp.FtpSession)
        Dim _rt As New List(Of ArxOne.Ftp.FtpSession)

        SyncLock _obj_session_dic_lock

            If _ftp_session_collection.ContainsKey(_ftp_server_uid) Then

                _current_list = _ftp_session_collection(_ftp_server_uid)

                If Not IsNothing(_current_list) Then
                    _current_list.Remove(_ftp_session)
                    _rt = _current_list
                End If

            End If

        End SyncLock

        Return _rt

    End Function

#End Region

    Function DownloadContainerItem(ByVal _item As DownloadItem, ByVal _args As DownloadContainerItemsArgs) As DownloadItem

        Dim _ftp_session As ArxOne.Ftp.FtpSession = Nothing
        Dim _ftp_client As ArxOne.Ftp.FtpClient = Nothing
        Dim _ftp_server_uid As String = String.Empty
        Dim _batr As New BasicAvailabilityTestResult

        Try

            Globalization.CultureInfo.GetCultureInfoByIetfLanguageTag(_settings.Language)

            If _item.IWorkItemResult.IsCanceled = True Then
                Throw New DownloadStoppedException("DownloadStopped!")
            End If

            SyncLock _obj_ftp_client_lock

                _ftp_server_uid = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(String.Format("{0}{1}{2}{3}", _args.ConnectionInfo.Host, _args.ConnectionInfo.Port, _args.ConnectionInfo.Username, _args.ConnectionInfo.Password).ToLower))

                _log.Info("FTP Server UID {0}", _ftp_server_uid)

                If _ftp_client_collection.ContainsKey(_ftp_server_uid) = False Then

                    _log.Info("There is no FTP Client for this Connection - Creating a new one")

                    _log.Info("Starting Basic availability Test...")

                    _batr = BasicAvailabilityTest(_args.ConnectionInfo)

                    If _batr.PingTest = False And _batr.PortTest = False Then 'Both Test has to fail
                        _log.Info("Basic availability Test  failed!")
                        Throw New BasicAvailabilityTestFailedException("Test Failed")
                    Else
                        _log.Info("Basic availability Test passed!")

                        SetupFTPClient(_ftp_client, _args.ConnectionInfo)
                        _ftp_client_collection.Add(_ftp_server_uid, _ftp_client)

                    End If

                Else
                    _log.Info("There is already an FTP Client for this Connection")
                    _ftp_client = _ftp_client_collection(_ftp_server_uid)
                End If

                If _args.SingleSessionMode = True Then

                    If _ftp_session_collection.ContainsKey(_ftp_server_uid) = False Then
                        _log.Info("SSM Mode - No FTP Session for this Connection found - Creating a new one")
                        _ftp_session = _ftp_client.Session
                        _ftp_session_collection.Add(_ftp_server_uid, AddFTPSessionToDic(_ftp_server_uid, _ftp_session))
                    Else
                        _log.Info("SSM Mode - Using existing FTP Session for this Connection")
                        _ftp_session = _ftp_session_collection(_ftp_server_uid).First
                    End If
                Else

                    _ftp_session = _ftp_client.Session

                    If _ftp_session_collection.ContainsKey(_ftp_server_uid) = False Then
                        _ftp_session_collection.Add(_ftp_server_uid, AddFTPSessionToDic(_ftp_server_uid, _ftp_session))
                    Else
                        _ftp_session_collection(_ftp_server_uid) = AddFTPSessionToDic(_ftp_server_uid, _ftp_session)
                    End If

                End If

            End SyncLock

            DownloadItem(_item, _ftp_session, _args.SingleSessionMode, _ftp_server_uid, _args.RetryMode)

        Catch ex As DownloadStoppedException
            _log.Info("Download Stopped")
            _item.DownloadStatus = NET3.DownloadItem.Status.Stopped

        Catch ex As BasicAvailabilityTestFailedException
            _log.Error(ex, ex.Message)
            _item.DownloadStatus = NET3.DownloadItem.Status.Failed_ConnectionError

        Catch ex As Exception
            _log.Error(ex, ex.Message)
            _item.DownloadStatus = NET3.DownloadItem.Status.Failed_AuthError
            ParseFTPException(ex, _item)
        Finally

            'PostDownload(_item, _ftp_session, _args.SingleSessionMode, _ftp_server_uid)
            _post_stp.QueueWorkItem(Function()
                                        Me.PostDownload(_item, _ftp_session, _args.SingleSessionMode, _ftp_server_uid)
                                        Return True
                                    End Function)

        End Try

        Return _item

    End Function


    Private Sub DownloadItem(ByVal _item As DownloadItem, ByVal _ftp_session As ArxOne.Ftp.FtpSession, ByVal _ssm As Boolean, ByVal _ftp_server_uid As String, Optional _isRetry As Boolean = False)

        Dim _filemode As IO.FileMode
        Dim _restart As Long = 0

        'IO Stream Variablen
        Dim buffer As [Byte]()
        Dim bytesRead As Integer = 0
        Dim _starttime As DateTime

        Dim _percent_downloaded As Double = 0
        Dim _ctime As TimeSpan
        Dim elapsed As TimeSpan
        Dim bytesPerSec As Integer = 0
        Dim _skip_download As Boolean = False


        Try

            SyncLock _obj_dl_count_lok
                _dl_count += 1
            End SyncLock


            If _item.IWorkItemResult.IsCanceled = True Then
                Throw New DownloadStoppedException("DownloadStopped")
            End If

            If String.IsNullOrWhiteSpace(_item.LocalFile) Then
                Throw New Exception("Filepath is empty!")
            End If

            If IO.Directory.Exists(IO.Path.GetDirectoryName(_item.LocalFile)) = False Then

                _log.Warn("Target directory not exists --> creating")
                IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(_item.LocalFile))
            End If

            If _item.FileSize = 0 Then
                _log.Warn("No fie size found - trying to determining")
                GetItemFileSize(_item, _ftp_session)
            End If

            If GetFreeDiskSpaceForPath(_item.LocalFile) < _item.FileSize Then
                Throw New IO.IOException("NotEnoughDiskSpace")
            End If

            If (_settings.ExistingFileHandling = ExistingFileHandling.ResumeFile Or _isRetry = True) AndAlso (IO.File.Exists(_item.LocalFile)) Then

                _filemode = IO.FileMode.Append
                _restart = New IO.FileInfo(_item.LocalFile).Length

                If _item.FileSize.Equals(New IO.FileInfo(_item.LocalFile).Length) And Not _item.FileSize = 0 Then
                    _item.LocalFileSize = _item.FileSize
                    _skip_download = True
                Else
                    _item.LocalFileSize = _restart
                    _log.Info("File already exists but is not fully downloaded")
                End If

            Else
                _filemode = IO.FileMode.Create
            End If

            If _skip_download = True Then
                _log.Info("File already completed - Skipping FTP Connect!")
            Else

                _starttime = Now

                _item.DownloadStatus = NET3.DownloadItem.Status.Running

                Using _ftp_read_stream = ArxOne.Ftp.FtpClientUtility.Retr(_ftp_session.Connection.Client, New ArxOne.Ftp.FtpPath(_item.FullPath), ArxOne.Ftp.FtpTransferMode.Binary, _restart, _ftp_session)

                    buffer = New Byte(8192) {}
                    bytesRead = _ftp_read_stream.Read(buffer, 0, buffer.Length)

                    Using _local_write_stream As New IO.FileStream(_item.LocalFile, _filemode, IO.FileAccess.Write, IO.FileShare.None, 8192, False)

                        While bytesRead > 0 And (_item.IWorkItemResult.IsCanceled = False)

                            Dim _tmp_percent_downloaded As Double = 0
                            Dim _new_perc As Double = 0
                            Dim _download_speed As String = String.Empty

                            _local_write_stream.Write(buffer, 0, bytesRead)

                            bytesRead = _ftp_read_stream.Read(buffer, 0, buffer.Length)
                            _item.SizeDownloaded += bytesRead

                            elapsed = DateTime.Now.Subtract(_starttime)
                            bytesPerSec = CInt(If(elapsed.TotalSeconds < 1, _item.SizeDownloaded, _item.SizeDownloaded / elapsed.TotalSeconds))

#Region "Berechnung Download Speed / Fortschritt"

                            _tmp_percent_downloaded = _local_write_stream.Position / _item.FileSize
                            _new_perc = _tmp_percent_downloaded * 100

                            If _new_perc <= 0.1 Then
                                _new_perc = Math.Round(_new_perc, 4)
                            ElseIf _new_perc <= 1 Then
                                _new_perc = Math.Round(_new_perc, 2)
                            Else
                                _new_perc = Math.Round(_new_perc, 1)
                            End If


                            If _new_perc <> _percent_downloaded Then

                                Dim _tmp_speed As Double

                                _percent_downloaded = _new_perc

                                _ctime = DateTime.Now.Subtract(_starttime)

                                _tmp_speed = Math.Round(bytesPerSec / 1024, 2)

                                If _tmp_speed >= 1024 Then
                                    _download_speed = Math.Round(_tmp_speed / 1024, 2) & " MB/s"
                                Else
                                    _download_speed = _tmp_speed & " KB/s"
                                End If

                                _item.DownloadSpeed = _download_speed
                                _item.DownloadProgress = _percent_downloaded
                                _item.LocalFileSize = _local_write_stream.Length


                            End If

#End Region

#Region "Limit Speed"

                            Dim _max_bytes_per_second As Long

                            If Not String.IsNullOrWhiteSpace(MainViewModel.ThisInstance.MaxDownloadSpeed) Then

                                _max_bytes_per_second = Integer.Parse(MainViewModel.ThisInstance.MaxDownloadSpeed)

                                If Not _max_bytes_per_second <= 0 Then

                                    If Not _dl_count <= 1 Then
                                        _max_bytes_per_second = CLng((_max_bytes_per_second * 1024) / _dl_count)
                                    Else
                                        _max_bytes_per_second = CLng((_max_bytes_per_second * 1024))
                                    End If

                                    ThrottleByteTransfer(_max_bytes_per_second, _item.SizeDownloaded, _ctime, bytesPerSec)

                                End If

                            End If
#End Region
                        End While

                    End Using

                End Using

            End If

            _item.DownloadStatus = NET3.DownloadItem.Status.Completed

        Catch ex As DownloadStoppedException
            _log.Info("Download Stopped")
            _item.DownloadStatus = NET3.DownloadItem.Status.Stopped

        Catch ex As NotEnoughFreeDiskSpaceException
            _log.Error(ex, ex.Message)
            _item.DownloadStatus = NET3.DownloadItem.Status.Failed_NotEnoughDiskSpace

        Catch ex As FileNameTooLongException
            _log.Warn(ex, ex.Message)
            _item.DownloadStatus = NET3.DownloadItem.Status.Failed_FileNameTooLong


        Catch ex As ArxOne.Ftp.Exceptions.FtpAuthenticationException

            If _item.IWorkItemResult.IsCanceled = True Or GeneralHelper.IsDownloadStopped = True Then
                _log.Info("Download has been stopped")
                _item.DownloadStatus = NET3.DownloadItem.Status.Stopped
            Else
                _log.Error(ex, ex.Message)
                ParseFTPException(ex, _item)
            End If

        Catch ex As ArxOne.Ftp.Exceptions.FtpFileException

            If GeneralHelper.IsDownloadStopped = True Then
                _log.Info("Download has been stopped")
                _item.DownloadStatus = NET3.DownloadItem.Status.Stopped
            Else
                _log.Error(ex, ex.Message)
                ParseFTPException(ex, _item)
            End If

        Catch ex As ArxOne.Ftp.Exceptions.FtpProtocolException

            If _item.IWorkItemResult.IsCanceled = True Or GeneralHelper.IsDownloadStopped = True Then
                _log.Info("Download has been stopped")
                _item.DownloadStatus = NET3.DownloadItem.Status.Stopped
            Else
                _log.Error(ex, ex.Message)
                ParseFTPException(ex, _item)
            End If

        Catch ex As ArxOne.Ftp.Exceptions.FtpTransportException

            If _item.IWorkItemResult.IsCanceled = True Or GeneralHelper.IsDownloadStopped = True Then
                _log.Info("Download has been stopped!")
                _item.DownloadStatus = NET3.DownloadItem.Status.Stopped
            Else
                _log.Error(ex, ex.Message)

                If String.IsNullOrWhiteSpace(_item.LocalFile) = False AndAlso (GetFreeDiskSpaceForPath(_item.LocalFile) <= 4096 Or GetFreeDiskSpaceForPath(_item.LocalFile) < _item.FileSize) Then
                    _log.Error("Disk is full or not enough space to fully save this file")
                    _item.DownloadStatus = NET3.DownloadItem.Status.Failed_NotEnoughDiskSpace
                Else
                    ParseFTPException(ex, _item)
                End If

            End If

        Catch ex As ArxOne.Ftp.Exceptions.FtpException

            If _item.IWorkItemResult.IsCanceled = True Or GeneralHelper.IsDownloadStopped = True Then
                _log.Info("Download has been stopped!")
                _item.DownloadStatus = NET3.DownloadItem.Status.Stopped
            Else
                _log.Error(ex, ex.Message)
                ParseFTPException(ex, _item)
            End If

        Catch ex As IO.IOException

            _log.Error(ex, ex.Message)

            If IsDiskFull(ex) = True Or ex.Message = "NotEnoughDiskSpace" Then
                _item.DownloadStatus = NET3.DownloadItem.Status.Failed_NotEnoughDiskSpace
            Else
                _item.DownloadStatus = NET3.DownloadItem.Status.Failed_IOError
            End If


        Catch ex As Exception
            If _item.IWorkItemResult.IsCanceled = True Or GeneralHelper.IsDownloadStopped = True Then
                _log.Info("Download has been stopped!")
                _item.DownloadStatus = NET3.DownloadItem.Status.Stopped
            Else
                _log.Error(ex, ex.Message)
                ParseFTPException(ex, _item)
            End If

        Finally

            SyncLock _obj_dl_count_lok
                _dl_count -= 1
            End SyncLock

            _item.DownloadSpeed = String.Empty

        End Try

    End Sub

    Public Sub PostDownload(ByRef _item As DownloadItem, ByVal _ftp_session As ArxOne.Ftp.FtpSession, ByVal _ssm As Boolean, ByVal _ftp_server_uid As String)

        Dim _hashcommand As String = String.Empty
        Dim _reply As ArxOne.Ftp.FtpReply
        Dim _tmp_hash As String = String.Empty
        Dim _hashtype As Container.HashType = Container.HashType.None

        Try

            If _item.IWorkItemResult.IsCanceled = True Then
                Throw New DownloadStoppedException("DownloadStopped!")
            End If

            _item.DownloadSpeed = String.Empty

            If GeneralHelper.IsDownloadStopped = False And _item.DownloadStatus = NET3.DownloadItem.Status.Completed Then

                _item.PreserveStatusAndUnCheck()

                _item.DownloadStatus = NET3.DownloadItem.Status.HashCheck

                If _item.HashType = Container.HashType.None Then

                    If _ftp_session.Connection.Client.ServerFeatures.HasFeature("MD5") Then
                        _hashcommand = "MD5"
                        _hashtype = Container.HashType.MD5
                    End If

                    If _ftp_session.Connection.Client.ServerFeatures.HasFeature("XMD5") Then
                        _hashcommand = "XMD5"
                        _hashtype = Container.HashType.MD5
                    End If

                    If _ftp_session.Connection.Client.ServerFeatures.HasFeature("XSHA1") Then
                        _hashcommand = "XSHA1"
                        _hashtype = Container.HashType.SHA1
                    End If

                    If _ftp_session.Connection.Client.ServerFeatures.HasFeature("XCRC") Then
                        _hashcommand = "XCRC"
                        _hashtype = Container.HashType.CRC
                    End If

#Region "Try to get FTP Server Hash"


                    If Not String.IsNullOrWhiteSpace(_hashcommand) Then

                        Try

                            _log.Info("Server supports Hash Alogrightm {0}", _hashcommand)

                            _reply = _ftp_session.Expect(_ftp_session.SendCommand(_hashcommand, _item.FullPath), 250, 251)

                            If _reply.Code.IsSuccess = True Then

                                _log.Info("Hash server side successfully determined!")

                                _item.HashType = _hashtype
                                _tmp_hash = _reply.Lines(0).ToString.Replace(_item.FullPath, "")
                                _tmp_hash = _tmp_hash.Replace(Chr(34), "")
                                _item.FileHash = _tmp_hash.Trim

                            Else
                                Throw New ArxOne.Ftp.Exceptions.FtpFileException("Hash could not be determined", New ArxOne.Ftp.FtpReplyCode(550))
                            End If

                        Catch ex As ArxOne.Ftp.Exceptions.FtpFileException

                            If ex.Code.Code = 550 Then 'FileNotFound

                                Try

                                    _log.Warn("Hash could Not be detemined - Trying alternate method")

                                    'Try to ChangeDir
                                    _reply = _ftp_session.Expect(_ftp_session.SendCommand("CWD", _item.DirectoryPath), 250)

                                    If _reply.Code.IsSuccess = True Then

                                        _log.Debug("Directory sucessfully changed")

                                        _reply = _ftp_session.Expect(_ftp_session.SendCommand(_hashcommand, _item.FileName), 250, 251)

                                        If _reply.Code.IsSuccess = True Then

                                            _log.Info("Hash server side successfully determined via alternate Method!")

                                            _item.HashType = _hashtype
                                            _tmp_hash = _reply.Lines(0).ToString.Replace(_item.FullPath, "")
                                            _tmp_hash = _tmp_hash.Replace(Chr(34), "")
                                            _item.FileHash = _tmp_hash.Trim

                                        Else
                                            _log.Error("Alternate Method failed - Hash could not be determined")
                                        End If

                                    End If

                                Catch exalternate As Exception
                                    _log.Error(exalternate, "Hash could not be determined")
                                End Try

                            End If


                        Catch ex As Exception
                            _log.Error(ex, "Hash could not be determined")
                        End Try

                    Else
                        _log.Info("Server does not Support any Hash Algorithm")
                    End If

#End Region


                Else
                    _log.Info("Download Item has already an Hash (provided via SFDL Container)")
                End If

                Select Case _item.HashType

                    Case Container.HashType.None

                        _item.DownloadStatus = NET3.DownloadItem.Status.Completed

                    Case Container.HashType.MD5

                        _log.Info("Check if MD5 hashes match")

                        If RHash.Hasher.GetHashForFile(_item.LocalFile, RHash.HashType.MD5).ToLower.Equals(_item.FileHash.ToLower) Then
                            _log.Info("MD5 Hash is Valid!")
                            _item.DownloadStatus = NET3.DownloadItem.Status.Completed_HashValid
                        Else
                            _log.Info("MD5 Hash Invalid")
                            _item.DownloadStatus = NET3.DownloadItem.Status.Completed_HashInvalid
                        End If

                    Case Container.HashType.SHA1

                        _log.Info("Check if SHA1 hashes match ")

                        If RHash.Hasher.GetHashForFile(_item.LocalFile, RHash.HashType.SHA1).ToLower.Equals(_item.FileHash.ToLower) Then
                            _log.Info("SHA1 Hash is Valid!")
                            _item.DownloadStatus = NET3.DownloadItem.Status.Completed_HashValid
                        Else
                            _log.Info("SHA1 Hash Invalid")
                            _item.DownloadStatus = NET3.DownloadItem.Status.Completed_HashInvalid
                        End If

                    Case Container.HashType.CRC

                        _log.Info("Check if CRC hashes match ")

                        _item.FileHash = _item.FileHash.PadLeft(8, "0"c)

                        If RHash.Hasher.GetHashForFile(_item.LocalFile, RHash.HashType.CRC32).ToLower.Equals(_item.FileHash.ToLower) Then
                            _log.Info("CRC Hash is Valid!")
                            _item.DownloadStatus = NET3.DownloadItem.Status.Completed_HashValid
                        Else
                            _log.Info("CRC Hash Invalid")
                            _item.DownloadStatus = NET3.DownloadItem.Status.Completed_HashInvalid
                        End If

                End Select

            Else
                _log.Info("Download was stopped or item was not fully downloaded - skipping Hash Check")
            End If

        Catch ex As DownloadStoppedException
            _log.Info("Download Stopped")
            _item.DownloadStatus = NET3.DownloadItem.Status.Stopped

        Catch ex As Exception
            _log.Error(ex.Message)
            _item.DownloadStatus = NET3.DownloadItem.Status.Completed_HashInvalid

        Finally

            If _item.DownloadStatus = NET3.DownloadItem.Status.Failed_ServerFull And _ssm = False Then
                RaiseEvent ServerFull(_item)
            End If

            If (_item.RetryPossible And _item.RetryCount < _settings.MaxRetry) And Not _item.DownloadStatus = NET3.DownloadItem.Status.Stopped Then

                _item.DownloadStatus = NET3.DownloadItem.Status.RetryWait
                System.Threading.Thread.Sleep(_settings.RetryWaitTime * 1000)
                _item.RetryCount += 1
                _log.Info("Spooling item to the retry waitinglist")
                _item.DownloadStatus = NET3.DownloadItem.Status.Retry

            End If


            SyncLock _obj_ftp_client_lock

                Try

                    If _ssm = False And _ftp_session_collection(_ftp_server_uid).Count >= 1 Then

                        _log.Info("Invalidating Session...")
                        RemoveFTPSessionFromDic(_ftp_server_uid, _ftp_session)
                        _ftp_session.Invalidate()

                    Else
                        _log.Info("Running in Single Session Mode or we have only one remaining ftp session --> let this Session intact")
                    End If

                Catch ex As Exception
                    _log.Error("Failed to invalidate FTP Session")
                End Try

            End SyncLock

        End Try

    End Sub


#Region "IDisposable Support"
    Private disposedValue As Boolean ' Dient zur Erkennung redundanter Aufrufe.

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                Try

                    For Each _client In _ftp_client_collection
                        Try
                            _client.Value.Dispose()
                        Catch ex As Exception
                        End Try
                    Next

                Catch ex As Exception
                End Try
            End If

            ' TODO: nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalize() weiter unten überschreiben.
            ' TODO: große Felder auf Null setzen.
        End If
        disposedValue = True
    End Sub

    ' TODO: Finalize() nur überschreiben, wenn Dispose(disposing As Boolean) weiter oben Code zur Bereinigung nicht verwalteter Ressourcen enthält.
    'Protected Overrides Sub Finalize()
    '    ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(disposing As Boolean) weiter oben ein.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' Dieser Code wird von Visual Basic hinzugefügt, um das Dispose-Muster richtig zu implementieren.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(disposing As Boolean) weiter oben ein.
        Dispose(True)
        ' TODO: Auskommentierung der folgenden Zeile aufheben, wenn Finalize() oben überschrieben wird.
        ' GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
