Public Class Settings

    Public Property DeleteSFDLAfterOpen As Boolean = False
    Public Property Language As String = "de-DE"
    Public Property DownloadDirectory As String = String.Empty
    Public Property ExistingFileHandling As ExistingFileHandling = ExistingFileHandling.ResumeFile
    Public Property PreventStandby As Boolean = True
    Public Property CreatePackageSubfolder As Boolean = False
    Public Property AskForDownloadDirectory As Boolean = False
    Public Property MaxDownloadThreads As Integer = 3
    Public Property MaxRetry As Integer = 3
    Public Property RetryWaitTime As Integer = 3
    Public Property NotMarkAllContainerFiles As Boolean = False
    Public Property SearchUpdates As Boolean = True
    Public Property MinimizeToTray As Boolean = False
    Public Property InstantVideo As Boolean = False
    Public Property UnRARSettings As New UnRARSettings
    Public Property SpeedReportSettings As New SpeedreportSettings
    Public Property RemoteControlSettings As New RemoteControlSettings
    Public Property AppAccent As String = "Blue"
    Public Property AppTheme As String = "BaseLight"
    Public Property ExcludeMaliciousFiles As Boolean = True
    Public Property AutoPasswordContainer As Boolean = False
    Public Property AutoPasswordContainerList As New List(Of String)
    Public Property DownloadItemBlacklist As New ObjectModel.ObservableCollection(Of String)

    Public Shared Function SaveSettings(ByVal _settings As Settings) As Boolean

        Dim _rt As Boolean = True

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


        Catch ex As Exception
            _rt = False
        End Try

        Return _rt

    End Function

    Public Shared Function InitNewSettings() As Settings

        Dim _rt As New Settings

        With _rt

            .AutoPasswordContainer = True
            .CreatePackageSubfolder = False
            .DeleteSFDLAfterOpen = False
            .DownloadDirectory = IO.Path.Combine(Environment.GetEnvironmentVariable("USERPROFILE"), "Downloads")
            .ExistingFileHandling = ExistingFileHandling.ResumeFile
            .InstantVideo = False
            .AppAccent = "Blue"
            .AppTheme = "BaseLight"
            .Language = "de"
            .MaxDownloadThreads = 3
            .MaxRetry = 3
            .PreventStandby = True
            .UnRARSettings = New UnRARSettings
            .ExcludeMaliciousFiles = True
            .SpeedReportSettings = New SpeedreportSettings

        End With


        With _rt.SpeedReportSettings

            Dim _template As New Text.StringBuilder

            _template.AppendLine("SFDL: %%SFDL_FILENAME%%")
            _template.AppendLine("Upper: %%SFDL_UPPER%%")
            _template.AppendLine("")
            _template.AppendLine("%%SFDL_SIZE%% in %%DLTIME%% heruntergeladen @ %%SPEED%% (Im Durchschnitt)")
            _template.AppendLine("")
            _template.AppendLine("Kommentar: %%COMMENT%%")


            .SpeedreportTemplate = _template.ToString

        End With

        Return _rt

    End Function

End Class


Public Enum ExistingFileHandling
    ResumeFile
    OverwriteFile
End Enum
