Imports System.Text.RegularExpressions
Imports NLog

Module SFDLFileHelper

    Private _log As NLog.Logger = NLog.LogManager.GetLogger("SFDLFileHelper")
    Private _caes_lock As New Object


    Sub CheckAndEnqueueSession(ByVal _session As ContainerSession)

        If Not IsNothing(_session) AndAlso GeneralHelper.IsDownloadStopped = False Then

            SyncLock _session.SynLock

                If _session.SessionState = ContainerSessionState.None Then
                    _session.SessionState = ContainerSessionState.Queued
                End If

            End SyncLock

            SyncLock _caes_lock
                MainViewModel.ThisInstance.QueryDownloadItems()
            End SyncLock

        End If

    End Sub

    Public Async Function GetContainerTotalSize(ByVal _container As ContainerSession) As Task(Of Double)

        Dim _full_session_size As Double

        _full_session_size = Await System.Threading.Tasks.Task.Run(Function()
                                                                       Return _container.DownloadItems.Aggregate(_full_session_size, Function(current, _file) current + _file.FileSize)
                                                                   End Function)

        Return _full_session_size

    End Function
    Sub CheckAndFixPackageName(ByRef _container As ContainerSession)

        Dim _count As Integer = 1

        For Each _package In _container.ContainerFile.Packages

            If String.IsNullOrWhiteSpace(_package.Name) Then
                _package.Name = String.Format("Package{0}", _count)
            End If

        Next

    End Sub

    Function GetContainerVersion(ByVal _sfdl_file_path As String) As Integer

        Dim _version As Integer = 0
        Dim _xml As New Xml.XmlDocument

        _xml.LoadXml(My.Computer.FileSystem.ReadAllText(_sfdl_file_path, Text.Encoding.Default))

        Try

            For Each _element As Xml.XmlNode In _xml.GetElementsByTagName("ContainerVersion")
                _version = Integer.Parse(_element.InnerText.ToString)
            Next

        Catch ex As Exception
            _version = 0
        End Try

        'SFDL v2 COntainer
        If _version = 0 Then

            Try

                For Each _element As Xml.XmlNode In _xml.GetElementsByTagName("SFDLFileVersion")
                    _version = Integer.Parse(_element.InnerText.ToString)
                Next

            Catch ex As Exception
                _version = 0
            End Try

        End If

        _log.Info("SFDL File Version: {0}", _version)

        Return _version

    End Function

    Sub GenerateContainerFingerprint(ByRef _container_session As ContainerSession)

        Dim _fingerprint As String = String.Empty

        _fingerprint = _container_session.ContainerFile.MaxDownloadThreads.ToString
        _fingerprint = _fingerprint & _container_session.ContainerFile.Connection.Host
        _fingerprint = _fingerprint & _container_session.ContainerFile.Connection.Port
        _fingerprint = _fingerprint & _container_session.ContainerFile.Connection.Username
        _fingerprint = _fingerprint & _container_session.ContainerFile.Packages.Count
        _fingerprint = _fingerprint & _container_session.DisplayName
        _fingerprint = _fingerprint & _container_session.ContainerFile.Uploader
        _fingerprint = _fingerprint & _container_session.ContainerFileName

        If Not _container_session.ContainerFile.Packages.Count = 0 Then
            _fingerprint = _fingerprint & _container_session.ContainerFile.Packages(0).Name
        End If

        If Not _container_session.ContainerFile.Packages.Where(Function(mypackage) mypackage.BulkFolderMode = False).Count = 0 Then

            If Not _container_session.ContainerFile.Packages(0).FileList.Count = 0 Then
                _fingerprint = _fingerprint & _container_session.ContainerFile.Packages(0).FileList(0).FileName

                If Not _container_session.ContainerFile.Packages(0).FileList(0).FullPath.Length > 248 Then
                    _fingerprint = _fingerprint & IO.Path.GetDirectoryName(_container_session.ContainerFile.Packages(0).FileList(0).FullPath)
                Else
                    _fingerprint = _fingerprint & System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(_container_session.ContainerFile.Packages(0).FileList(0).FullPath))
                End If


            End If

        Else

            If Not _container_session.ContainerFile.Packages.Where(Function(mypackage) mypackage.BulkFolderMode = True).Count = 0 Then

                If Not _container_session.ContainerFile.Packages(0).BulkFolderList.Count = 0 Then
                    _fingerprint = _fingerprint & _container_session.ContainerFile.Packages(0).BulkFolderList(0).BulkFolderPath
                    _fingerprint = _fingerprint & _container_session.ContainerFile.Packages(0).BulkFolderList(0).PackageName
                End If

            End If

        End If

        _container_session.Fingerprint = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(_fingerprint))

    End Sub

    Sub DecryptSFDLContainer(ByRef _container As Container.Container, ByVal _password As String)

        Dim _decrypt_helper As New SFDL.Container.Decrypt

        With _container

            .Description = _decrypt_helper.DecryptString(.Description, _password)
            .Uploader = _decrypt_helper.DecryptString(.Uploader, _password)
            '.Encrypted = False

        End With

        With _container.Connection

            .Host = _decrypt_helper.DecryptString(.Host, _password)
            .Password = _decrypt_helper.DecryptString(.Password, _password)
            .Username = _decrypt_helper.DecryptString(.Username, _password)

        End With

        For Each _package In _container.Packages

            _package.Name = _decrypt_helper.DecryptString(_package.Name, _password)

            For Each _item In _package.FileList

                With _item

                    .DirectoryPath = _decrypt_helper.DecryptString(.DirectoryPath, _password)
                    .DirectoryRoot = _decrypt_helper.DecryptString(.DirectoryRoot, _password)
                    .FileName = _decrypt_helper.DecryptString(.FileName, _password)
                    .FullPath = _decrypt_helper.DecryptString(.FullPath, _password)
                    .PackageName = _decrypt_helper.DecryptString(.PackageName, _password)

                End With

            Next

            For Each _bulkfolder In _package.BulkFolderList

                _bulkfolder.BulkFolderPath = _decrypt_helper.DecryptString(_bulkfolder.BulkFolderPath, _password)
                _bulkfolder.PackageName = _decrypt_helper.DecryptString(_bulkfolder.PackageName, _password)

            Next

        Next

    End Sub

    Sub GenerateContainerSessionDownloadItems(ByVal _containersession As ContainerSession, ByVal _mark_files As Boolean, ByVal _blacklist As List(Of BlacklistItem), ByVal _create_sub_folder As Boolean)

        Dim _tmp_list As New List(Of DownloadItem)

        For Each _package In _containersession.ContainerFile.Packages

            For Each _file In _package.FileList

                Dim _dl_item As New DownloadItem()

                _dl_item.Init(_file)

                With _dl_item

                    Dim _blacklist_match As Boolean = False

                    .PackageName = _package.Name
                    .ParentContainerID = _containersession.ID
                    .LocalFile = GetDownloadFilePath(_create_sub_folder, _containersession, _dl_item)

#Region "Blacklist Check"

                    For Each _blacklist_item As BlacklistItem In _blacklist

                        Dim _blacklistpattern As Regex = New Regex(_blacklist_item.FileNameRegex)

                        If _blacklistpattern.IsMatch(.FileName) Then
                            _blacklist_match = True

                            Select Case _blacklist_item.ItemType

                                Case BlacklistItem.Type.Malicious
                                    .ExcludedType = DownloadItem.ExcludeType.Malicious

                                Case BlacklistItem.Type.User
                                    .ExcludedType = DownloadItem.ExcludeType.User

                                Case Else
                                    .ExcludedType = DownloadItem.ExcludeType.None

                            End Select

                        End If

                    Next

#End Region

                    If _mark_files = False And _blacklist_match = False Then
                        .isSelected = True
                    Else
                        .isSelected = False
                    End If

                End With

                If Not _dl_item.FileSize = 0 And IO.File.Exists(_dl_item.LocalFile) Then

                    'Check if File is already completly downloaded
                    Dim _fileinfo As New IO.FileInfo(_dl_item.LocalFile)

                    If _fileinfo.Length.Equals(_dl_item.FileSize) Then
                        _dl_item.isSelected = False
                        _dl_item.DownloadStatus = DownloadItem.Status.AlreadyDownloaded
                        _dl_item.LocalFileSize = _dl_item.FileSize
                    End If

                End If

                _tmp_list.Add(_dl_item)


            Next

        Next

        _containersession.DownloadItems.AddRange(_tmp_list)

    End Sub

    Function GetBulkFileList(ByRef _container_session As ContainerSession) As Boolean

        Dim _ftp As ArxOne.Ftp.FtpClient = Nothing
        Dim _rt As Boolean = True
        Dim _stopwatch As New Stopwatch


        Try

            SetupFTPClient(_ftp, _container_session.ContainerFile.Connection)

            _stopwatch.Start()

            For Each _package In _container_session.ContainerFile.Packages.Where(Function(mypackage) mypackage.BulkFolderMode = True)

                For Each _bulk_folder In _package.BulkFolderList
                    _package.FileList = GetRecursiveListing(_bulk_folder.BulkFolderPath, _ftp, _package.Name)
                Next

            Next

            _stopwatch.Stop()

            _log.Debug("Recursive listing completed - This took {0} seconds", _stopwatch.Elapsed.TotalSeconds)

        Catch ex As Exception
            _rt = False
            _log.Error(ex, ex.Message)
        End Try

        _ftp.Dispose()

        Return _rt

    End Function

    Function GetRecursiveListing(ByVal _bulk_folder As String, ByVal _ftp As ArxOne.Ftp.FtpClient, ByVal _packagename As String, Optional ByVal _subdirmode As Boolean = False, Optional _ftp_session As ArxOne.Ftp.FtpSession = Nothing) As List(Of SFDL.Container.FileItem)

        Dim _ftp_path As New ArxOne.Ftp.FtpPath(_bulk_folder)
        Dim _rt_list As New List(Of SFDL.Container.FileItem)
        Dim _mylog As NLog.Logger = NLog.LogManager.GetLogger("BulkRecursiveListing")
        Dim _ftp_unix_platform As New ArxOne.Ftp.Platform.UnixFtpPlatform
        Dim _ftp_entries As New List(Of ArxOne.Ftp.FtpEntry)
        Dim _ftp_list_session As ArxOne.Ftp.FtpSession = Nothing

        Try

            'Create Session for Bulk Listing
            If IsNothing(_ftp_session) Then
                _ftp_list_session = _ftp.Session
            Else
                _mylog.Info("Reusing existing ftp session")
                _ftp_list_session = _ftp_session
            End If


            _mylog.Debug("Getting Server Vendor...")

            _ftp.GetVendor(_ftp_list_session)

            _mylog.Debug("Vendor: {0}", _ftp.Vendor.ToString)

            Select Case _ftp.Vendor

#Region "Known FTP Servers"

                Case ArxOne.Ftp.FtpServerVendor.MLCBoardcom

                    _mylog.Info("Switching to MLCBoard FTP Server compatibility mode")

                    _ftp_list_session.Expect(_ftp_list_session.SendCommand("CWD", _ftp_path.ToString), 250)

                    For Each _item In ArxOne.Ftp.FtpClientUtility.List(_ftp, Nothing, _ftp_list_session)

                        Dim _entry As ArxOne.Ftp.FtpEntry

                        _entry = FTPHelper.TryParseLine(_item, _bulk_folder)

                        If Not IsNothing(_entry) Then
                            _ftp_entries.Add(_entry)
                        Else
                            _log.Error("Failed to parse line", _item)
                        End If

                    Next
#End Region

#Region "Generic FTP Servers"

                Case Else

#Region "MLSD Listing"

                    If _ftp_entries.Count = 0 AndAlso _ftp.ServerFeatures.HasFeature("MLSD") = True Then

                        Try

                            _mylog.Info("Server support MLSD Listing - Trying to get Item list via MLSD Command...")
                            _ftp_entries = ArxOne.Ftp.FtpClientUtility.MlsdEntries(_ftp, _ftp_path, _ftp_list_session).ToList()

                            If _ftp_entries.FirstOrDefault.Name.ToLower.Contains("no such file or directory") Or _ftp_entries.FirstOrDefault.Path.ToString.ToLower.Contains("no such file or directory") Then
                                Throw New Exception("MLSD Command failed")
                            End If

                        Catch ex As Exception
                            _mylog.Error("MLSD Command failed!")
                            _ftp_entries.Clear()
                        End Try

                    End If

#End Region


#Region "CWD first then LIST"

                    Try

                        If _ftp_entries.Count = 0 Then

                            _ftp_list_session.Expect(_ftp_list_session.SendCommand("CWD", _ftp_path.ToString), 250)

                            For Each _item In ArxOne.Ftp.FtpClientUtility.List(_ftp, Nothing, _ftp_list_session)

                                Dim _entry As ArxOne.Ftp.FtpEntry

                                _entry = FTPHelper.TryParseLine(_item, _bulk_folder)

                                If Not IsNothing(_entry) Then
                                    _ftp_entries.Add(_entry)
                                Else
                                    _log.Error("Failed to parse line", _item)
                                End If

                            Next

                        End If

                    Catch ex As Exception
                        _mylog.Error("LIST (CWD first) Command failed!")
                        _ftp_entries.Clear()
                    End Try

#End Region

#Region "LIST with Parameter"

                    Try

                        If _ftp_entries.Count = 0 Then

                            For Each _item In ArxOne.Ftp.FtpClientUtility.List(_ftp, _ftp_path, _ftp_list_session)

                                Dim _entry As ArxOne.Ftp.FtpEntry

                                _entry = FTPHelper.TryParseLine(_item, _bulk_folder)

                                If Not IsNothing(_entry) Then
                                    _ftp_entries.Add(_entry)
                                Else
                                    _log.Error("Failed to parse line", _item)
                                End If

                            Next

                        End If

                    Catch ex As Exception
                        _mylog.Error("LIST with Parameter Command failed!")
                        _ftp_entries.Clear()
                    End Try

#End Region



#End Region

            End Select

            _ftp_entries = _ftp_entries.OrderBy(Function(_myitem) _myitem.Name).ToList()

            For Each _entry In _ftp_entries

                Try

                    Dim _path_seperator As String = "/" 'Assume Unix Path Seperator

                    If Not IsNothing(_entry) Then

                        If _entry.Path.ToString.Contains("\") Then
                            _path_seperator = "\"
                        End If

                        If _entry.Type = ArxOne.Ftp.FtpEntryType.Directory And Not (_entry.Name.ToString.Equals(".") Or _entry.Name.ToString.Equals("..")) Then
                            _rt_list.AddRange(GetRecursiveListing(_entry.Path.ToString, _ftp, _packagename, True, _ftp_list_session))
                        Else

                            If _entry.Type = ArxOne.Ftp.FtpEntryType.File Then

                                Dim _file_item As New SFDL.Container.FileItem

                                _file_item.FullPath = _entry.Path.ToString
                                _file_item.FileName = _entry.Path.GetFileName
                                _file_item.FileSize = CLng(_entry.Size)
                                _file_item.PackageName = _packagename
                                _file_item.DirectoryPath = _bulk_folder

                                If _subdirmode = True Then
                                    _file_item.DirectoryRoot = _bulk_folder.Substring(0, _bulk_folder.LastIndexOf(_path_seperator))
                                Else
                                    _file_item.DirectoryRoot = _bulk_folder
                                End If

                                _file_item.HashType = Container.HashType.None

                                _rt_list.Add(_file_item)

                            End If

                        End If

                    End If

                Catch ex As Exception
                    _mylog.Error(ex, ex.Message)
                End Try

            Next

        Catch ex As Exception
            _mylog.Error(ex, ex.Message)
        End Try

        Return _rt_list

    End Function

    Private Function CleanDownloadPathInput(strIn As String) As String
        ' Replace invalid characters with empty strings.

        Dim _rt As String = String.Empty
        Dim _org_filename As String = String.Empty

        Try

            If Not String.IsNullOrWhiteSpace(strIn) Then

                _org_filename = IO.Path.GetFileName(strIn)

                _rt = strIn.Replace(_org_filename, "")

                _rt = IO.Path.Combine(_rt, Text.RegularExpressions.Regex.Replace(_org_filename, "[^\w\.@-]", ""))

            End If

        Catch e As TimeoutException
            Return String.Empty
        End Try

        Return _rt

    End Function

    Private Function SanitiseFileOrFolderName(ByVal filename As String) As String
        Dim invalidChars = Regex.Escape(New String(IO.Path.GetInvalidFileNameChars()))
        Dim invalidReStr = String.Format("[{0}]+", invalidChars)
        Dim reservedWords = {"CON", "PRN", "AUX", "CLOCK$", "NUL", "COM0", "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9", "LPT0", "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"}
        Dim sanitisedNamePart = Regex.Replace(filename, invalidReStr, "_")

        For Each reservedWord In reservedWords
            Dim reservedWordPattern = String.Format("^{0}\.", reservedWord)
            sanitisedNamePart = Regex.Replace(sanitisedNamePart, reservedWordPattern, "_reservedWord_.", RegexOptions.IgnoreCase)
        Next

        Return sanitisedNamePart
    End Function



    Function GetSessionLocalDownloadRoot(ByVal _container_session As ContainerSession, ByVal _user_download_dir As String) As String


        Return IO.Path.Combine(_user_download_dir, _container_session.DisplayName)


    End Function

    Function GetDownloadFilePath(ByVal _create_sub_folder As Boolean, ByVal _container_session As ContainerSession, ByVal _item As DownloadItem) As String

        Dim _download_dir As String = String.Empty
        Dim _tmp_last_sub_dir As String = String.Empty
        Dim _dowload_local_filename As String = String.Empty

        Try

            _download_dir = SanitiseFileOrFolderName(_container_session.LocalDownloadRoot)

            If _create_sub_folder Then
                _download_dir = IO.Path.Combine(_download_dir, SanitiseFileOrFolderName(_item.PackageName))
            End If

            _tmp_last_sub_dir = _item.DirectoryPath.Replace(_item.DirectoryRoot, "")

            If _tmp_last_sub_dir.StartsWith("\") Or _tmp_last_sub_dir.StartsWith("/") Then
                _tmp_last_sub_dir = _tmp_last_sub_dir.Remove(0, 1)
            End If

            If _tmp_last_sub_dir.EndsWith("\") Or _tmp_last_sub_dir.EndsWith("/") Then
                _tmp_last_sub_dir = _tmp_last_sub_dir.Remove(_tmp_last_sub_dir.Length - 1, 1)
            End If

            _dowload_local_filename = IO.Path.Combine(_download_dir, SanitiseFileOrFolderName(_tmp_last_sub_dir), SanitiseFileOrFolderName(_item.FileName))

            _dowload_local_filename = CleanDownloadPathInput(_dowload_local_filename)

        Catch ex As Exception
            _log.Error(ex, ex.Message)
        End Try


        Return _dowload_local_filename

    End Function

    Sub GenerateContainerSessionChains(ByVal _mycontainer_session As ContainerSession)

#Region "Parse Unrar/InstatVideo Chain"


        For Each _item In _mycontainer_session.DownloadItems.Where(Function(_my_item As DownloadItem) IO.Path.GetExtension(_my_item.FileName).Equals(".rar"))

            Dim _unrarchain As New UnRARChain
            Dim _searchpattern As Regex
            Dim _count As Integer
            Dim _log As Logger = LogManager.GetLogger("RarChainParser")

            If Not _item.FileName.Contains(".part") Then

                _count = 0

                _item.FirstUnRarFile = True
                _item.RequiredForInstantVideo = True
                _unrarchain.MasterUnRarChainFile = _item

                _log.Debug("First UnRar File: {0}", _item.FileName)

                _searchpattern = New Regex(Regex.Escape(IO.Path.GetFileNameWithoutExtension(_item.FileName)) & "\.r[0-9]{1,2}")

                For Each _chainitem As DownloadItem In _mycontainer_session.DownloadItems.Where(Function(_my_item As DownloadItem) _searchpattern.IsMatch(_my_item.FileName) And _my_item.PackageName.Equals(_item.PackageName))

                    _log.Debug("ChainItem FileName: {0}", _chainitem.FileName)

                    If _count < 1 Then
                        _chainitem.RequiredForInstantVideo = True
                    End If

                    _unrarchain.ChainMemberFiles.Add(_chainitem)

                    _count += 1

                Next

                _mycontainer_session.UnRarChains.Add(_unrarchain)

            Else

                _searchpattern = New Regex("^((?!\.part(?!0*1\.rar$)\d+\.rar$).)*\.(?:rar|r?0*1)$") 'THX @ http://stackoverflow.com/a/2537935

                _count = 0

                If _searchpattern.IsMatch(_item.FileName) Then 'MasterFile

                    Dim _tmp_filename_replace As String

                    _log.Debug("First UnRar File: {0}", _item.FileName)
                    _item.FirstUnRarFile = True
                    _item.RequiredForInstantVideo = True
                    _unrarchain.MasterUnRarChainFile = _item

                    _tmp_filename_replace = _item.FileName.Remove(_item.FileName.IndexOf(".part", StringComparison.Ordinal))

                    _searchpattern = New Regex(Regex.Escape(_tmp_filename_replace) & "\.part[0-9]{1,3}.rar")

                    For Each _chainitem As DownloadItem In _mycontainer_session.DownloadItems.Where(Function(_my_item As DownloadItem) (_searchpattern.IsMatch(_my_item.FileName) And Not _my_item.FileName.Equals(_unrarchain.MasterUnRarChainFile.FileName)) And _my_item.PackageName.Equals(_item.PackageName))

                        If _chainitem.FileName.StartsWith(_tmp_filename_replace) Then

                            _log.Debug("ChainItem FileName: {0}", _chainitem.FileName)

                            If _count < 1 Then
                                _chainitem.RequiredForInstantVideo = True
                            End If

                            _unrarchain.ChainMemberFiles.Add(_chainitem)

                            _count += 1

                        End If

                    Next

                    _mycontainer_session.UnRarChains.Add(_unrarchain)

                End If

            End If

        Next

#End Region

    End Sub

End Module
