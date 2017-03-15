

Class Application

    Private Shared ReadOnly SingleInstance As New SingleInstance(New Guid("6da4f80f-3146-4967-b97c-1f4158ab0fb7"))

    Private Sub Application_Startup(sender As Object, e As StartupEventArgs) Handles Me.Startup

        Dim _settings As New Settings
        Dim _settings_xml_path As String = String.Empty
        Dim _log As NLog.Logger = Nothing

        Try

            If SingleInstance.IsFirstInstance Then

                AddHandler SingleInstance.ArgumentsReceived, AddressOf SingleInstanceParameter

                If e.Args.Where(Function(myarg) myarg.ToLowerInvariant.ToLower.Equals("/log")).Count = 1 Then
                    LogHelper.GenerateLogConfig(IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "log.log"))
                Else
                    LogHelper.GenerateLogConfig()
                End If



                My.Settings.Upgrade()

                _log = NLog.LogManager.GetLogger("Startup")

                _settings_xml_path = IO.Path.Combine(Environment.GetEnvironmentVariable("appdata"), "SFDL.NET 3\settings.xml")

                If IO.Directory.Exists(IO.Path.GetDirectoryName(_settings_xml_path)) = False Then
                    IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(_settings_xml_path))
                End If

                If IO.File.Exists(_settings_xml_path) Then
                    _settings = CType(XMLHelper.XMLDeSerialize(_settings, _settings_xml_path), Settings)
                Else
                    _settings = Settings.InitNewSettings
                    XMLHelper.XMLSerialize(_settings, _settings_xml_path)
                End If

                Application.Current.Resources.Add("Settings", _settings)
                Application.Current.Resources.Add("DownloadStopped", False)


                System.Threading.Thread.CurrentThread.CurrentUICulture = Globalization.CultureInfo.GetCultureInfoByIetfLanguageTag(_settings.Language)

                Globalization.CultureInfo.DefaultThreadCurrentCulture = Globalization.CultureInfo.GetCultureInfoByIetfLanguageTag(_settings.Language)

                FrameworkElement.LanguageProperty.OverrideMetadata(GetType(FrameworkElement), New FrameworkPropertyMetadata(Markup.XmlLanguage.GetLanguage(_settings.Language)))

                If _settings.PreventStandby = True Then
                    StandyHandler.PreventStandby()
                    _log.Info("System Standby is now blocked")
                End If

                SingleInstance.ListenForArgumentsFromSuccessiveInstances()

                Else
                    ' if there is an argument available, fire it
                    If e.Args.Length > 0 Then
                    SingleInstance.PassArgumentsToFirstInstance(e.Args)
                End If

                Environment.[Exit](0)

            End If

        Catch ex As Exception

            If Not IsNothing(_log) Then
                _log.Fatal(ex, ex.Message)
            Else
                Console.WriteLine(ex.ToString)
            End If

        End Try

    End Sub

    Private Shared Sub SingleInstanceParameter(sender As Object, e As ArgumentsReceivedEventArgs)

        Dim _settings As New Settings

        _settings = CType(Application.Current.Resources("Settings"), Settings)

        For Each _item In e.Args

            If Not String.IsNullOrWhiteSpace(_item) Then


                If IO.Path.GetExtension(_item).ToLower = ".sfdl" Then

                    DispatchService.DispatchService.Invoke(Sub()
                                                               MainViewModel.ThisInstance.OpenSFDLFile(_item)
                                                           End Sub)
                End If

            End If
        Next

    End Sub

    Private Sub Application_Exit(sender As Object, e As ExitEventArgs) Handles Me.[Exit]


    End Sub

    ' Application-level events, such as Startup, Exit, and DispatcherUnhandledException
    ' can be handled in this file.

End Class
