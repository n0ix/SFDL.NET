Public Class Settings

    Public Property DeleteSFDLAfterOpen As Boolean = False
    Public Property Language As String = "de-DE"
    Public Property DownloadDirectory As String = String.Empty
    Public Property ExistingFileHandling As ExistingFileHandling = ExistingFileHandling.ResumeFile
    Public Property PreventStandby As Boolean = True
    Public Property CreatePackageSubfolder As Boolean = False
    Public Property MaxDownloadThreads As Integer = 3
    Public Property MaxRetry As Integer = 3
    Public Property RetryWaitTime As Integer = 3
    Public Property NotMarkAllContainerFiles As Boolean = False
    Public Property SearchUpdates As Boolean = True
    Public Property InstantVideo As Boolean = False
    Public Property UnRARSettings As New UnRARSettings
    Public Property SpeedReportSettings As New SpeedreportSettings
    Public Property RemoteControlSettings As New RemoteControlSettings
    Public Property AppAccent As String = "Blue"
    Public Property AppTheme As String = "BaseLight"
    Public Property DownloadItemBlacklist As New ObjectModel.ObservableCollection(Of String)
    Public Property ExcludeMaliciousFileExtensions As Boolean = True

    Public Shared Function InitNewSettings() As Settings

        Dim _rt As New Settings

        With _rt

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
            .ExcludeMaliciousFileExtensions = True
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
