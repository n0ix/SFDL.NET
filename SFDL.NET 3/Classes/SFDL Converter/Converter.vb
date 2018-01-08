Public Class Converter

    Public Shared Sub ConvertSFDLv2ToSFDLv3(ByVal _mylegacycontainer As SFDL.Container.Legacy.SFDLFile, ByRef _mycontainer As SFDL.Container.Container)

        With _mycontainer.Connection

            .AuthRequired = _mylegacycontainer.ConnectionInfo.AuthRequired
            .CharacterEncoding = CType(_mylegacycontainer.ConnectionInfo.CharacterEncoding, Container.CharacterEncoding)

            Select Case _mylegacycontainer.ConnectionInfo.DataConnectionType

                Case Container.Legacy.FTPDataConnectionType.AutoActive
                    .DataConnectionType = Container.FTPDataConnectionType.Active

                Case Container.Legacy.FTPDataConnectionType.AutoPassive
                    .DataConnectionType = Container.FTPDataConnectionType.Passive

                Case Container.Legacy.FTPDataConnectionType.EPRT
                    .DataConnectionType = Container.FTPDataConnectionType.Passive

                Case Container.Legacy.FTPDataConnectionType.EPSV
                    .DataConnectionType = Container.FTPDataConnectionType.Passive

                Case Container.Legacy.FTPDataConnectionType.PASV
                    .DataConnectionType = Container.FTPDataConnectionType.Passive

                Case Container.Legacy.FTPDataConnectionType.PASVEX
                    .DataConnectionType = Container.FTPDataConnectionType.Passive

                Case Container.Legacy.FTPDataConnectionType.PORT
                    .DataConnectionType = Container.FTPDataConnectionType.Passive

            End Select

            .DataType = CType(_mylegacycontainer.ConnectionInfo.DataType, Container.FTPDataType)
            .Host = _mylegacycontainer.ConnectionInfo.Host
            .Password = _mylegacycontainer.ConnectionInfo.Password
            .Port = _mylegacycontainer.ConnectionInfo.Port

            Select Case _mylegacycontainer.ConnectionInfo.EncryptionMode

                Case Container.Legacy.FTPEncryptionMode.Implicit

                    .SSLProtocol = Security.Authentication.SslProtocols.Tls

                Case Container.Legacy.FTPEncryptionMode.Explicit

                    .SSLProtocol = Security.Authentication.SslProtocols.Ssl3

                Case Else

                    .SSLProtocol = Security.Authentication.SslProtocols.None

            End Select

            .Username = _mylegacycontainer.ConnectionInfo.Username

        End With

        _mycontainer.ContainerVersion = 10
        _mycontainer.Description = _mylegacycontainer.Description
        _mycontainer.Uploader = _mylegacycontainer.Uploader
        _mycontainer.Encrypted = _mylegacycontainer.Encrypted
        _mycontainer.MaxDownloadThreads = _mylegacycontainer.MaxDownloadThreads

        For Each _package In _mylegacycontainer.Packages

            Dim _new_package As New SFDL.Container.Package

            With _package

                _new_package.Name = .Packagename
                _new_package.BulkFolderMode = .BulkFolderMode

                For Each _bulkfolder In .BulkFolderList

                    Dim _new_bulkfolder As New SFDL.Container.BulkFolder

                    _new_bulkfolder.BulkFolderPath = _bulkfolder.BulkFolderPath
                    _new_bulkfolder.PackageName = _bulkfolder.PackageName

                    _new_package.BulkFolderList.Add(_new_bulkfolder)

                Next

                For Each _file In .FileList

                    Dim _new_file As New SFDL.Container.FileItem

                    _new_file.DirectoryPath = _file.DirectoryPath
                    _new_file.DirectoryRoot = _file.DirectoryRoot
                    _new_file.FileHash = _file.FileHash
                    _new_file.FileName = _file.FileName
                    _new_file.FileSize = _file.FileSize
                    _new_file.FullPath = _file.FileFullPath
                    _new_file.HashType = CType(_file.FileHashType, Container.HashType)
                    _new_file.PackageName = _file.PackageName

                    _new_package.FileList.Add(_new_file)

                Next

            End With

            _mycontainer.Packages.Add(_new_package)

        Next

    End Sub

End Class
