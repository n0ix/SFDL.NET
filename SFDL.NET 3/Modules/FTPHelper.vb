Imports System.Net.Sockets
Imports System.Text
Imports ArxOne.Ftp

Module FTPHelper

    Sub SetupFTPClient(ByRef _ftp_client As ArxOne.Ftp.FtpClient, ByVal _connection_info As SFDL.Container.Connection)

        Dim _creds As Net.NetworkCredential
        Dim _ftp_client_param As New ArxOne.Ftp.FtpClientParameters
        Dim _log As NLog.Logger = NLog.LogManager.GetLogger("SetupFTPClient")

        With _connection_info

            If .AuthRequired = True Then
                _creds = New Net.NetworkCredential(.Username, .Password)
            Else
                _creds = New Net.NetworkCredential("anonymous", "Password")
            End If

            With _ftp_client_param


                If Net.IPAddress.TryParse(_connection_info.Host, Net.IPAddress.Parse("127.0.0.10")) = True Then
                    .ActiveTransferHost = Net.IPAddress.Parse(_connection_info.Host)
                Else
                    _log.Warn("SFDL does not contain a valid IP Adress - assuming it as a DNS Name and will try to resolv it")
                    .ActiveTransferHost = Net.Dns.GetHostEntry(_connection_info.Host).AddressList.First
                    _log.Warn(String.Format("Resoved IP Adress:{0}", .ActiveTransferHost.ToString))
                End If

                .AnonymousPassword = "sfdl@anon.net"

                If _connection_info.DataConnectionType = Container.FTPDataConnectionType.Passive Then
                    .Passive = True
                Else
                    .Passive = False
                End If

                .SslProtocols = _connection_info.SSLProtocol

                Select Case _connection_info.CharacterEncoding

                    Case Container.CharacterEncoding.ASCII

                        .DefaultEncoding = System.Text.Encoding.ASCII

                    Case Container.CharacterEncoding.Standard

                        .DefaultEncoding = System.Text.Encoding.Default

                    Case Container.CharacterEncoding.UTF7

                        .DefaultEncoding = System.Text.Encoding.UTF7

                    Case Container.CharacterEncoding.UTF8

                        .DefaultEncoding = System.Text.Encoding.UTF8

                End Select

                .ConnectTimeout = TimeSpan.FromSeconds(_connection_info.ConnectTimeout)
                .SessionTimeout = TimeSpan.FromSeconds(_connection_info.CommandTimeout)
                .ReadWriteTimeout = TimeSpan.FromSeconds(_connection_info.CommandTimeout)

            End With

            _ftp_client = New ArxOne.Ftp.FtpClient(New Uri(String.Format("ftp://{0}:{1}", .Host, .Port)), _creds, _ftp_client_param)

            AddHandler _ftp_client.ConnectionInitialized, AddressOf FTPConnectionInitialized
            AddHandler _ftp_client.Reply, AddressOf _log_ftp
            AddHandler _ftp_client.Request, AddressOf _log_ftp
            AddHandler _ftp_client.IOError, AddressOf _log_ftp

            _ftp_client.SendSingleCommand("NOOP")

            If _ftp_client.ServerFeatures.HasFeature("UTF8") And _connection_info.CharacterEncoding = Container.CharacterEncoding.UTF8 Then
                _ftp_client.SendSingleCommand("OPTS", "UTF8 ON")
            End If

        End With


    End Sub

    Private Sub FTPConnectionInitialized(sender As Object, e As EventArgs)
        Debug.WriteLine("FTP Connection initialized")
    End Sub

    Public Function BasicAvailabilityTest(ByVal _connection_info As SFDL.Container.Connection) As BasicAvailabilityTestResult

        Dim _rt As New BasicAvailabilityTestResult
        Dim _log As NLog.Logger = NLog.LogManager.GetLogger("BasicAvailabilityTest")

        _rt.PingTest = True

        '#Region "Ping Test"

        '        Try

        '            If My.Computer.Network.Ping(_connection_info.Host, 500) = True Then
        '                _rt.PingTest = True
        '            Else
        '                _log.Error("Ping Test Failed")
        '            End If

        '        Catch ex As Exception
        '            _log.Error("Ping Test Failed")
        '        End Try

        '#End Region


#Region "Port Test"
        If TestPort(_connection_info.Host, _connection_info.Port) = True Then
            _rt.PortTest = True
        Else
            _log.Error("Port Test Failed")
        End If
#End Region

        Return _rt

    End Function
    Private Function TestPort(ByVal _server As String, ByVal _port As Integer) As Boolean

        Dim _rt As Boolean = False
        Dim _tcpclient As New TcpClient

        Try

            _tcpclient = New TcpClient(_server, _port)

            System.Threading.Thread.Sleep(500)

            _rt = True

        Catch ex As Exception
            _rt = False
        Finally

            If _tcpclient.Connected = True Then
                _tcpclient.Close()
            End If

        End Try

        Return _rt

    End Function

    Private Sub _log_ftp(sender As Object, e As ArxOne.Ftp.ProtocolMessageEventArgs)

        Dim _log As NLog.Logger = NLog.LogManager.GetLogger("FTPClient")
        Dim _log_line As New StringBuilder


        If Not IsNothing(e.RequestCommand) Then
            _log_line.AppendLine(e.RequestCommand)
        End If

        If Not IsNothing(e.RequestParameters) Then
            For Each _line In e.RequestParameters
                _log_line.AppendLine(_line)
            Next
        End If

        If Not IsNothing(e.Reply) Then

            _log_line.AppendLine(e.Reply.Code.ToString)

            For Each _line In e.Reply.Lines
                _log_line.AppendLine(_line)
            Next

        End If

        _log.Info(_log_line)

    End Sub

    Function TryParseLine(ByVal _item As String, _parent_folder As String) As ArxOne.Ftp.FtpEntry

        Dim _ftp_unix_platform As New ArxOne.Ftp.Platform.UnixFtpPlatform
        Dim _ftp_windows_platform As New ArxOne.Ftp.Platform.WindowsFtpPlatform
        Dim _ftp_filezilla_platform As New ArxOne.Ftp.Platform.WindowsFileZillaFtpPlatform
        Dim _rt As ArxOne.Ftp.FtpEntry = Nothing

        _rt = _ftp_unix_platform.Parse(_item, _parent_folder)

        If IsNothing(_rt) Then
            _rt = _ftp_windows_platform.Parse(_item, _parent_folder)
        End If

        If IsNothing(_rt) Then
            _rt = _ftp_filezilla_platform.Parse(_item, _parent_folder)
        End If

        Return _rt

    End Function

End Module
