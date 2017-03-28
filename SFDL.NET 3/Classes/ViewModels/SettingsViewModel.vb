Imports System.ComponentModel
Imports MahApps.Metro.Controls.Dialogs
Public Class SettingsViewModel
    Inherits ViewModelBase
    Implements IDataErrorInfo

    Private _settings As New Settings
    Private _selected_unrar_password As String = String.Empty
    Private _selected_blacklist_item As String = String.Empty

    Public Sub New()
        _settings = CType(Application.Current.Resources("Settings"), Settings)
    End Sub

#Region "General/Basic Settings Properties"

    Public ReadOnly Property AppAccentList As List(Of MahApps.Metro.Accent)
        Get
            Return CType(MahApps.Metro.ThemeManager.Accents, List(Of MahApps.Metro.Accent))
        End Get
    End Property

    Public ReadOnly Property AppThemeList As List(Of MahApps.Metro.AppTheme)
        Get
            Return CType(MahApps.Metro.ThemeManager.AppThemes, List(Of MahApps.Metro.AppTheme))
        End Get
    End Property

    Public Property PreventStandby As Boolean
        Set(value As Boolean)
            _settings.PreventStandby = value
            RaisePropertyChanged("PreventStandby")
        End Set
        Get
            Return _settings.PreventStandby
        End Get
    End Property

    Public Property SearchUpdates As Boolean
        Set(value As Boolean)
            _settings.SearchUpdates = value
            RaisePropertyChanged("SearchUpdates")
        End Set
        Get
            Return _settings.SearchUpdates
        End Get
    End Property
    Public Property Language As String
        Set(value As String)
            _settings.Language = value
            RaisePropertyChanged("Language")
        End Set
        Get
            Return _settings.Language
        End Get
    End Property
    Public Property InstantVideo As Boolean
        Set(value As Boolean)
            _settings.InstantVideo = value
            RaisePropertyChanged("InstantVideo")
        End Set
        Get
            Return _settings.InstantVideo
        End Get
    End Property
    Public Property DownloadDirectory As String
        Set(value As String)
            _settings.DownloadDirectory = value
            RaisePropertyChanged("DownloadDirectory")
        End Set
        Get
            Return _settings.DownloadDirectory
        End Get
    End Property

    Public Property CreatePackageSubfolder As Boolean
        Set(value As Boolean)
            _settings.CreatePackageSubfolder = value
        End Set
        Get
            Return _settings.CreatePackageSubfolder
        End Get
    End Property
    Public Property ExistingFileHandling As ExistingFileHandling
        Set(value As ExistingFileHandling)
            _settings.ExistingFileHandling = value
            RaisePropertyChanged("ExistingFileHandling")
        End Set
        Get
            Return _settings.ExistingFileHandling
        End Get
    End Property
    Public Property MaxDownloadThreads As Integer
        Set(value As Integer)
            _settings.MaxDownloadThreads = value
            RaisePropertyChanged("MaxDownloadThreads")
        End Set
        Get
            Return _settings.MaxDownloadThreads
        End Get
    End Property
    Public Property MaxRetry As Integer
        Set(value As Integer)
            _settings.MaxRetry = value
            RaisePropertyChanged("MaxRetry")
        End Set
        Get
            Return _settings.MaxRetry
        End Get
    End Property
    Public Property RetryWaitTime As Integer
        Set(value As Integer)
            _settings.RetryWaitTime = value
            RaisePropertyChanged("RetryWaitTime")
        End Set
        Get
            Return _settings.RetryWaitTime
        End Get
    End Property
    Public Property NotMarkAllContainerFiles As Boolean
        Set(value As Boolean)
            _settings.NotMarkAllContainerFiles = value
            RaisePropertyChanged("MarkAllContainerFiles")
        End Set
        Get
            Return _settings.NotMarkAllContainerFiles
        End Get
    End Property
    Public Property DeleteSFDLAfterOpen As Boolean
        Set(value As Boolean)
            _settings.DeleteSFDLAfterOpen = value
            RaisePropertyChanged("DeleteSFDLAfterOpen")
        End Set
        Get
            Return _settings.DeleteSFDLAfterOpen
        End Get
    End Property

    Public Property AppAccent As MahApps.Metro.Accent
        Set(value As MahApps.Metro.Accent)
            _settings.AppAccent = value.Name
            RaisePropertyChanged("AppAccent")
            MahApps.Metro.ThemeManager.ChangeAppStyle(Application.Current, MahApps.Metro.ThemeManager.GetAccent(value.Name), Me.AppTheme)
        End Set
        Get
            Return MahApps.Metro.ThemeManager.GetAccent(_settings.AppAccent)
        End Get
    End Property

    Public Property AppTheme As MahApps.Metro.AppTheme
        Set(value As MahApps.Metro.AppTheme)
            _settings.AppTheme = value.Name
            RaisePropertyChanged("AppTheme")
            MahApps.Metro.ThemeManager.ChangeAppStyle(Application.Current, Me.AppAccent, MahApps.Metro.ThemeManager.GetAppTheme(value.Name))
        End Set
        Get
            Return MahApps.Metro.ThemeManager.GetAppTheme(_settings.AppTheme)
        End Get
    End Property

    Public Property DownloadItemBlacklist As ObjectModel.ObservableCollection(Of String)
        Set(value As ObjectModel.ObservableCollection(Of String))
            _settings.DownloadItemBlacklist = value
            RaisePropertyChanged("DownloadItemBlacklist")
        End Set
        Get
            Return _settings.DownloadItemBlacklist
        End Get
    End Property

    Public Property SelectedBlacklistItem As String
        Set(value As String)
            _selected_blacklist_item = value
        End Set
        Get
            Return _selected_blacklist_item
        End Get
    End Property


#End Region

#Region "UnRAR Settings Properties"

    Public Property UnRARAfterDownload As Boolean
        Set(value As Boolean)
            _settings.UnRARSettings.UnRARAfterDownload = value
            RaisePropertyChanged("UnRARAfterDownload")
        End Set
        Get
            Return _settings.UnRARSettings.UnRARAfterDownload
        End Get
    End Property

    Public Property DeleteAfterUnRAR As Boolean
        Set(value As Boolean)
            _settings.UnRARSettings.DeleteAfterUnRAR = value
            RaisePropertyChanged("UnRARAfterDownload")
        End Set
        Get
            Return _settings.UnRARSettings.DeleteAfterUnRAR
        End Get
    End Property

    Public Property UseUnRARPasswordList As Boolean
        Set(value As Boolean)
            _settings.UnRARSettings.UseUnRARPasswordList = value
            RaisePropertyChanged("UseUnRARPasswordList")
        End Set
        Get
            Return _settings.UnRARSettings.UseUnRARPasswordList
        End Get
    End Property

    Public Property UnRARPasswordList As ObjectModel.ObservableCollection(Of String)
        Set(value As ObjectModel.ObservableCollection(Of String))
            _settings.UnRARSettings.UnRARPasswordList = value
            RaisePropertyChanged("UnRARPasswordList")
        End Set
        Get
            Return _settings.UnRARSettings.UnRARPasswordList
        End Get
    End Property

    Public Property SelectedUnRARPassword As String
        Set(value As String)
            _selected_unrar_password = value
        End Set
        Get
            Return _selected_unrar_password
        End Get
    End Property


#End Region

#Region "Speedreport Settings"

    Public Property SpeedreportEnabled As Boolean
        Set(value As Boolean)
            _settings.SpeedReportSettings.SpeedreportEnabled = value
            RaisePropertyChanged("SpeedreportEnabled")
        End Set
        Get
            Return _settings.SpeedReportSettings.SpeedreportEnabled
        End Get
    End Property

    Public Property SpeedreportUsername As String
        Set(value As String)
            _settings.SpeedReportSettings.SpeedreportUsername = value
            RaisePropertyChanged("SpeedreportUsername")
        End Set
        Get
            Return _settings.SpeedReportSettings.SpeedreportUsername
        End Get
    End Property

    Public Property SpeedreportConnection As String
        Set(value As String)
            _settings.SpeedReportSettings.SpeedreportConnection = value
            RaisePropertyChanged("SpeedreportConnection")
        End Set
        Get
            Return _settings.SpeedReportSettings.SpeedreportConnection
        End Get
    End Property

    Public Property SpeedreportComment As String
        Set(value As String)
            _settings.SpeedReportSettings.SpeedreportComment = value
            RaisePropertyChanged("SpeedreportComment")
        End Set
        Get
            Return _settings.SpeedReportSettings.SpeedreportComment
        End Get
    End Property
    Public Property SpeedreportTemplate As String
        Set(value As String)
            _settings.SpeedReportSettings.SpeedreportTemplate = value
            RaisePropertyChanged("SpeedreportTemplate")
        End Set
        Get
            Return _settings.SpeedReportSettings.SpeedreportTemplate
        End Get
    End Property


#End Region

#Region "Commands"

    Public ReadOnly Property BrowseFolderCommand() As ICommand
        Get
            Return New DelegateCommand(AddressOf SelectDownloadFolder)
        End Get
    End Property

    Private Async Sub SaveSettings()

        Dim _error As Boolean = False
        Dim _password_def As New Text.StringBuilder
        Dim _password_def_file As String = IO.Path.Combine(Environment.GetEnvironmentVariable("appdata"), "SFDL.NET 3", "sfdl_passwords.def")

        Try

            Application.Current.Resources("Settings") = _settings

            MainViewModel.ThisInstance.UpdateSettings()

            XMLHelper.XMLSerialize(_settings, IO.Path.Combine(Environment.GetEnvironmentVariable("appdata"), "SFDL.NET 3\settings.xml"))

            If IO.File.Exists(_password_def_file) Then
                IO.File.Delete(_password_def_file)
            End If


            _password_def.AppendLine("")
            _password_def.AppendLine("##")

            For Each _item In _settings.UnRARSettings.UnRARPasswordList
                _password_def.AppendLine(_item)
            Next

            My.Computer.FileSystem.WriteAllText(_password_def_file, _password_def.ToString, False, System.Text.Encoding.Default)

            If CBool(Application.Current.Resources("DownloadStopped")) = False Then
                Await MahApps.Metro.Controls.Dialogs.DialogCoordinator.Instance.ShowMessageAsync(Me, My.Resources.Strings.VariousStrings_Warning, My.Resources.Strings.Settings_SaveSettings_DownloadActive_Message)
            End If

        Catch ex As Exception
            _error = True
        End Try

        If _error = True Then
            Await MahApps.Metro.Controls.Dialogs.DialogCoordinator.Instance.ShowMessageAsync(Me, My.Resources.Strings.Settings_SaveTitle, My.Resources.Strings.Settings_SaveError)
        Else
            Await MahApps.Metro.Controls.Dialogs.DialogCoordinator.Instance.ShowMessageAsync(Me, My.Resources.Strings.Settings_SaveTitle, My.Resources.Strings.Settings_SaveSuccessful)
            Application.Current.Windows.OfType(Of Window).SingleOrDefault(Function(mywin) mywin.Name.Equals("SDFL_SettingsWindow")).Close()
        End If

    End Sub

    Public ReadOnly Property SaveSettingsCommand() As ICommand
        Get
            Return New DelegateCommand(AddressOf SaveSettings)
        End Get
    End Property
    Private Sub SelectDownloadFolder()

        Dim _sdf_dialog As New Forms.FolderBrowserDialog

        With _sdf_dialog
            .ShowNewFolderButton = True
        End With

        If _sdf_dialog.ShowDialog() = Forms.DialogResult.OK Then
            Me.DownloadDirectory = _sdf_dialog.SelectedPath
        End If

    End Sub
    Public ReadOnly Property SelectDownloadFolderCommand() As ICommand
        Get
            Return New DelegateCommand(AddressOf SelectDownloadFolder)
        End Get
    End Property

    Private Async Sub AddBlacklistItem()

        Dim _new_blacklist_item As String = String.Empty
        Dim _regex_fail As Boolean = False

        _new_blacklist_item = Await DialogCoordinator.Instance.ShowInputAsync(Me, My.Resources.Strings.Settings_Input_AddBlacklistItem_Title, My.Resources.Strings.Settings_Input_AddBlacklistItem_Message)

        Try

            If Not String.IsNullOrWhiteSpace(_new_blacklist_item) Then
                Dim _regex_test As Text.RegularExpressions.Regex
                _regex_test = New Text.RegularExpressions.Regex(_new_blacklist_item)
            End If

        Catch ex As Exception
            _regex_fail = True
            'Regex not valid!
        End Try

        If _regex_fail = True Then
            Await DialogCoordinator.Instance.ShowMessageAsync(Me, My.Resources.Strings.VariousStrings_Error, My.Resources.Strings.Settings_Input_AddBlacklistItem_RegexNotValid_Message, MessageDialogStyle.Affirmative)
        Else
            If Not _settings.DownloadItemBlacklist.Contains(_new_blacklist_item) And Not String.IsNullOrWhiteSpace(_new_blacklist_item) Then
                Me.DownloadItemBlacklist.Add(_new_blacklist_item)
            End If
        End If

    End Sub

    Public ReadOnly Property AddBlacklistItemCommand As ICommand
        Get
            Return New DelegateCommand(AddressOf AddBlacklistItem)
        End Get
    End Property

    Private Async Sub RemoveBlacklistItem()

        If Not String.IsNullOrWhiteSpace(_selected_blacklist_item) Then

            Dim _result As MessageDialogResult

            _result = Await DialogCoordinator.Instance.ShowMessageAsync(Me, My.Resources.Strings.Settings_Question_RemoveBlacklistItem_Title, My.Resources.Strings.Settings_Question_RemoveBlacklistItem_Message, MessageDialogStyle.AffirmativeAndNegative)

            If _result = MessageDialogResult.Affirmative Then
                DownloadItemBlacklist.Remove(_selected_blacklist_item)
            End If

        End If

    End Sub

    Public ReadOnly Property RemoveBlacklistItemCommand As ICommand
        Get
            Return New DelegateCommand(AddressOf RemoveBlacklistItem)
        End Get
    End Property


    Private Async Sub AddUnRARPassword()

        Dim _new_password As String = String.Empty

        _new_password = Await DialogCoordinator.Instance.ShowInputAsync(Me, My.Resources.Strings.Settings_Input_AddUnRARPassword_Title, My.Resources.Strings.Settings_Input_AddUnRARPassword_Message)

        If Not _settings.UnRARSettings.UnRARPasswordList.Contains(_new_password) And Not String.IsNullOrWhiteSpace(_new_password) Then
            Me.UnRARPasswordList.Add(_new_password)
        End If

    End Sub

    Public ReadOnly Property AddUnRARPasswordCommand As ICommand
        Get
            Return New DelegateCommand(AddressOf AddUnRARPassword)
        End Get
    End Property

    Private Async Sub RemoveUnRARPassword()

        If Not String.IsNullOrWhiteSpace(_selected_unrar_password) Then

            Dim _result As MessageDialogResult

            _result = Await DialogCoordinator.Instance.ShowMessageAsync(Me, My.Resources.Strings.Settings_Question_RemoveUnRARPassword_Title, My.Resources.Strings.Settings_Question_RemoveUnRARPassword_Message, MessageDialogStyle.AffirmativeAndNegative)

            If _result = MessageDialogResult.Affirmative Then
                UnRARPasswordList.Remove(_selected_unrar_password)
            End If

        End If

    End Sub

    Public ReadOnly Property RemoveUnRARPasswordCommand As ICommand
        Get
            Return New DelegateCommand(AddressOf RemoveUnRARPassword)
        End Get
    End Property

    Private Sub InsertSpeedReportVariable(ByVal parameter As Object)

        Dim _param As String = CType(parameter, String)

        Select Case _param

            Case "username"

                Me.SpeedreportTemplate = Me.SpeedreportTemplate & Space(1) & "%%USERNAME%%" & Space(1)

            Case "connection"

                Me.SpeedreportTemplate = Me.SpeedreportTemplate & Space(1) & "%%CONNECTION%%" & Space(1)

            Case "comment"

                Me.SpeedreportTemplate = Me.SpeedreportTemplate & Space(1) & "%%COMMENT%%" & Space(1)

            Case "downloadspeed"

                Me.SpeedreportTemplate = Me.SpeedreportTemplate & Space(1) & "%%SPEED%%" & Space(1)

            Case "downloadtime"

                Me.SpeedreportTemplate = Me.SpeedreportTemplate & Space(1) & "%%DLTIME%%" & Space(1)

            Case "sfdl_filename"

                Me.SpeedreportTemplate = Me.SpeedreportTemplate & Space(1) & "%%SFDL_FILENAME%%" & Space(1)

            Case "sfdl_uploader"

                Me.SpeedreportTemplate = Me.SpeedreportTemplate & Space(1) & "%%SFDL_UPPER%%" & Space(1)

            Case "sfdl_downloadsize"

                Me.SpeedreportTemplate = Me.SpeedreportTemplate & Space(1) & "%%SFDL_SIZE%%" & Space(1)

        End Select

    End Sub

    Public ReadOnly Property InsertSpeedReportVariableCommand As ICommand
        Get
            Return New DelegateCommand(AddressOf InsertSpeedReportVariable)
        End Get
    End Property

    Default Public ReadOnly Property Item(columnName As String) As String Implements IDataErrorInfo.Item
        Get
            If columnName = "DownloadDirectory" Then
                If String.IsNullOrEmpty(Me.DownloadDirectory) Then
                    Return My.Resources.Strings.Settings_DownloadDirectory_ChooseFolder
                End If
                If Not IO.Directory.Exists(Me.DownloadDirectory) Then
                    Return My.Resources.Strings.Settings_DownloadDirectory_DirectoryNotFalid
                End If
            End If
            Return String.Empty
        End Get
    End Property

    Public ReadOnly Property [Error] As String Implements IDataErrorInfo.Error
        Get
            Return String.Empty
        End Get
    End Property

    Public ReadOnly Property ResetSettingsCommand As ICommand
        Get
            Return New DelegateCommand(AddressOf ResetSettings)
        End Get
    End Property

    Private Async Sub ResetSettings()

        Dim _result As MessageDialogResult

        _result = Await DialogCoordinator.Instance.ShowMessageAsync(Me, My.Resources.Strings.Settings_Question_ResetSettings_Title, My.Resources.Strings.Settings_Question_ResetSettings_Message, MessageDialogStyle.AffirmativeAndNegative)

        _settings = Settings.InitNewSettings

        SaveSettings()

    End Sub

    Public ReadOnly Property ResetSpeedreportCommand As ICommand
        Get
            Return New DelegateCommand(AddressOf ResetSpeedreport)
        End Get
    End Property

    Private Async Sub ResetSpeedreport()


        Dim _template As New Text.StringBuilder
        Dim _result As MessageDialogResult


        _result = Await DialogCoordinator.Instance.ShowMessageAsync(Me, My.Resources.Strings.Settings_Question_ResetSpeedreport_Title, My.Resources.Strings.Settings_Question_ResetSpeedreport_Message, MessageDialogStyle.AffirmativeAndNegative)


        _template.AppendLine("SFDL: %%SFDL_FILENAME%%")
        _template.AppendLine("Upper: %%SFDL_UPPER%%")
        _template.AppendLine("")
        _template.AppendLine("%%SFDL_SIZE%% in %%DLTIME%% heruntergeladen @ %%SPEED%% (Im Durchschnitt)")
        _template.AppendLine("")
        _template.AppendLine("Kommentar: %%COMMENT%%")

        SpeedreportTemplate = _template.ToString()



    End Sub

#End Region



End Class
