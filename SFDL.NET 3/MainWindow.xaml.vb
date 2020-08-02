
Imports System.ComponentModel
Imports MahApps.Metro
Imports MahApps.Metro.Controls.Dialogs
Imports Microsoft.Win32
Imports NLog

Public Class MainWindow

    Dim _force_exit As Boolean = False

    Public Sub New()

        Dim _mvvm As New MainViewModel(DialogCoordinator.Instance)
        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        _mvvm.WindowInstance = Me

        Me.DataContext = _mvvm

    End Sub

    Private Sub LoadTheme()

        Dim baseColorScheme As String = CType(Application.Current.Resources("Settings"), Settings).BaseColorScheme
        Dim colorScheme As String = CType(Application.Current.Resources("Settings"), Settings).ColorScheme

        If Not String.IsNullOrWhiteSpace(baseColorScheme) And Not String.IsNullOrWhiteSpace(colorScheme) Then
            ControlzEx.Theming.ThemeManager.Current.ChangeThemeBaseColor(Application.Current, baseColorScheme)
            ControlzEx.Theming.ThemeManager.Current.ChangeThemeColorScheme(Application.Current, colorScheme)
        End If


        If (CType(Application.Current.Resources("Settings"), Settings).SyncThemeWithWindows) Then
            ControlzEx.Theming.ThemeManager.Current.SyncTheme(ControlzEx.Theming.ThemeSyncMode.SyncAll)
        End If

    End Sub

    Private Async Sub MainWindow_ContentRendered(sender As Object, e As EventArgs) Handles Me.ContentRendered

        Dim _settings As Settings = CType(Application.Current.Resources("Settings"), Settings)
        Dim _log As Logger = LogManager.GetLogger("ContentRendered")
        Dim _new_update As Boolean = False

        For Each _arg In Environment.GetCommandLineArgs

            If Not String.IsNullOrWhiteSpace(_arg) Then

                If IO.Path.GetExtension(_arg).ToLower = ".sfdl" Then
                    MainViewModel.ThisInstance.OpenSFDLFile(_arg)
                End If

            End If

        Next

        ComB_Container_Info.DataContext = MainViewModel.ThisInstance
        InstantVideoStreamList.DataContext = MainViewModel.ThisInstance

        If My.Settings.UserWindowState = WindowState.Normal Then

            If Not My.Settings.UserWindowHeight = 0 Then
                Me.Height = My.Settings.UserWindowHeight
            End If

            If Not My.Settings.UserWindowWitdh = 0 Then
                Me.Width = My.Settings.UserWindowWitdh
            End If

        Else
            Me.WindowState = WindowState.Maximized
        End If

        LoadTheme()

        If Environment.Is64BitOperatingSystem Then

            If IO.File.Exists(IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "bin", "unrar.exe")) = False Or IO.File.Exists(IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "bin", "cRARk_x64.exe")) = False Then
                Await ShowMessageAsync(My.Resources.Strings.VariousStrings_Warning, My.Resources.Strings.VariousStrings_UnRARExecutableMissingException)
            End If

            If IO.File.Exists(IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "lib", "lib64", "librhash.dll")) = False Then
                Await ShowMessageAsync(My.Resources.Strings.VariousStrings_Warning, My.Resources.Strings.VariousStrings_LibrHashMissingException)
            End If

        Else

            If IO.File.Exists(IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "bin", "unrar.exe")) = False Or IO.File.Exists(IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "bin", "cRARk_x86.exe")) = False Then
                Await ShowMessageAsync(My.Resources.Strings.VariousStrings_Warning, My.Resources.Strings.VariousStrings_UnRARExecutableMissingException)
            End If

            If IO.File.Exists(IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "lib", "lib32", "librhash.dll")) = False Then
                Await ShowMessageAsync(My.Resources.Strings.VariousStrings_Warning, My.Resources.Strings.VariousStrings_LibrHashMissingException)
            End If

        End If

        If Not IsNothing(Application.Current.Resources("FaultedSettings")) AndAlso CType(Application.Current.Resources("FaultedSettings"), Boolean) = True Then

            Dim _dialog_settings As New MetroDialogSettings

            _dialog_settings.AffirmativeButtonText = My.Resources.Strings.VariousStrings_AffirmativeButton_OK

            Await ShowMessageAsync(My.Resources.Strings.VariousStrings_Warning, My.Resources.Strings.FailedToLoadSettings, MessageDialogStyle.Affirmative, _dialog_settings)

        End If

#Region "Check and Update InstallState and File Registration"


        If isAdministrator() = True And CheckInstallState() = False Then

            Try 'Versuchen die Dateiendung .sfdl und den URI Handler zu registrieren

                Dim _file_assoiciation As New FileAssociation

                _file_assoiciation.Extension = "sfdl"
                _file_assoiciation.ContentType = "application/sfdl.net"
                _file_assoiciation.FullName = "SFDL.NET Files"
                _file_assoiciation.IconIndex = 0
                _file_assoiciation.IconPath = IO.Path.Combine(IO.Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory), "Icon.ico")
                _file_assoiciation.ProperName = "SFDL.NET File"
                _file_assoiciation.AddCommand("open", System.Reflection.Assembly.GetExecutingAssembly().Location & " " & Chr(34) & "%1" & Chr(34))

                _file_assoiciation.Create()

                _log.Info("SFDL Extension registerd!")

                'Enable LongPaths Support
                Registry.LocalMachine.CreateSubKey("SYSTEM\CurrentControlSet\Control\FileSystem")
                Dim reg As RegistryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\CurrentControlSet\Control\FileSystem", True)
                reg.SetValue("LongPathsEnabled", 1, RegistryValueKind.DWord)
                reg.Close()

                UpdateInstallState()

            Catch ex As Exception
                _log.Error(ex, ex.Message)
            End Try

        End If


        If CheckInstallState() = False Then

            Dim _result As MessageDialogResult
            Dim _dialog_settings As New MetroDialogSettings

            _dialog_settings.AffirmativeButtonText = My.Resources.Strings.VariousStrings_AffirmativeButton_Yes
            _dialog_settings.NegativeButtonText = My.Resources.Strings.VariousStrings_NegativeButton

            _result = Await ShowMessageAsync(My.Resources.Strings.VariousStrings_Warning, My.Resources.Strings.InstallPathChangedPrompt, MessageDialogStyle.AffirmativeAndNegative, _dialog_settings)

            If _result = MessageDialogResult.Affirmative Then
                RunAsAdmin()
            End If

        End If

#End Region

        _log.Info("Adding PowerModeChanged Handler")

        AddHandler Microsoft.Win32.SystemEvents.PowerModeChanged, AddressOf PowerModeChanged

    End Sub

    Private Sub PowerModeChanged(sender As Object, e As PowerModeChangedEventArgs)

        Dim _log As Logger = LogManager.GetLogger("PowerModeChanged")

        If e.Mode = Microsoft.Win32.PowerModes.Resume Then

            Dim _settings As Settings = CType(Application.Current.Resources("Settings"), Settings)

            If Not IsNothing(_settings) AndAlso _settings.PreventStandby = True Then
                _log.Info("System has resumed from Standy/Hibernation - recalling 'BlockStandby'")
                StandyHandler.PreventStandby()
            Else
                _log.Debug("System has resumed from Standy/Hibernation")
            End If
        Else
            _log.Debug("System PoweMode Changed")
        End If

    End Sub

    Private Sub OnGridKeyUp(sender As Object, e As KeyEventArgs)

        If Not IsNothing(ListView_DownloadItems.SelectedItems) AndAlso (e.Key = Key.Enter) Then

            For Each _item As DownloadItem In ListView_DownloadItems.SelectedItems

                If _item.isSelected = True Then
                    _item.isSelected = False
                Else
                    _item.isSelected = True
                End If

            Next

            e.Handled = True

        End If

    End Sub

    Private Async Sub SFDL_MainWindow_Closing(sender As Object, e As CancelEventArgs) Handles SFDL_MainWindow.Closing

        Dim _somthing_running As Boolean = False

        Dim _log As Logger = LogManager.GetLogger("SFDL_MainWindow_Closing")

        Try


            If _force_exit = False Then

                For Each _container_session In MainViewModel.ThisInstance.ContainerSessions

                    If _container_session.SessionState = ContainerSessionState.DownloadRunning Or _container_session.UnRarChains.Where(Function(mychain) mychain.UnRARRunning = True).Count >= 1 Then
                        _somthing_running = True
                    End If

                Next

                If _somthing_running = True Then

                    Dim _result As MessageDialogResult
                    Dim _dialog_settings As New MetroDialogSettings

                    _dialog_settings.AffirmativeButtonText = My.Resources.Strings.VariousStrings_AffirmativeButton_Yes
                    _dialog_settings.NegativeButtonText = My.Resources.Strings.VariousStrings_NegativeButton

                    e.Cancel = True

                    _result = Await ShowMessageAsync(My.Resources.Strings.ExitApplication_Prompt_Title, My.Resources.Strings.ExitApplication_Prompt_Message, MessageDialogStyle.AffirmativeAndNegative, _dialog_settings)

                    If _result = MessageDialogResult.Affirmative Then
                        _force_exit = True
                        Application.Current.Shutdown()
                    End If

                Else
                    MainViewModel.ThisInstance.SaveSessions()
                End If

            Else
                MainViewModel.ThisInstance.SaveSessions()
            End If

            'Only if exit has not been canceld
            If e.Cancel = False Then
                NotifyIcon.Dispose()
                _log.Info("Info removing PowerMode Handler")
                RemoveHandler Microsoft.Win32.SystemEvents.PowerModeChanged, AddressOf PowerModeChanged
            End If

        Catch ex As Exception
            _log.Error(ex, ex.Message)
        End Try

    End Sub

    Private Sub ListView_DownloadItems_ContextMenuOpening(sender As Object, e As ContextMenuEventArgs) Handles ListView_DownloadItems.ContextMenuOpening


        If ListView_DownloadItems.SelectedItems.Count = 0 Then
            'Disable ContextMenu Items
            LV_DownloadItems_CM_CloseContainer.IsEnabled = False
            LV_DownloadItems_CM_MarkAllItems.IsEnabled = False
            LV_DownloadItems_CM_OpenParentFolder.IsEnabled = False
            LV_DownloadItems_CM_UnMarkAllItems.IsEnabled = False
        Else
            LV_DownloadItems_CM_CloseContainer.IsEnabled = True
            LV_DownloadItems_CM_MarkAllItems.IsEnabled = True
            LV_DownloadItems_CM_OpenParentFolder.IsEnabled = True
            LV_DownloadItems_CM_UnMarkAllItems.IsEnabled = True

        End If

    End Sub

    Private Sub SFDL_MainWindow_StateChanged(sender As Object, e As EventArgs) Handles SFDL_MainWindow.StateChanged

        'We save the last State before we got minimized so we can restore it correctly
        If MainViewModel.ThisInstance.WindowState = WindowState.Normal Or MainViewModel.ThisInstance.WindowState = WindowState.Maximized Then
            My.Settings.UserWindowState = MainViewModel.ThisInstance.WindowState
            My.Settings.Save()
        ElseIf MainViewModel.ThisInstance.WindowState = WindowState.Minimized Then

            Dim _settings As Settings = CType(Application.Current.Resources("Settings"), Settings)

            If Not IsNothing(_settings) AndAlso _settings.MinimizeToTray = True Then
                Me.Hide()
            End If

        End If

    End Sub

    Private Sub SFDL_MainWindow_SizeChanged(sender As Object, e As SizeChangedEventArgs) Handles SFDL_MainWindow.SizeChanged

        If MainViewModel.ThisInstance.WindowState = WindowState.Normal Or MainViewModel.ThisInstance.WindowState = WindowState.Maximized Then
            My.Settings.UserWindowState = MainViewModel.ThisInstance.WindowState
            My.Settings.UserWindowHeight = Me.Height
            My.Settings.UserWindowWitdh = Me.Width
            My.Settings.Save()
        End If

    End Sub
End Class
