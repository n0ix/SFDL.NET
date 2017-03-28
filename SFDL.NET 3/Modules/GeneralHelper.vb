Module GeneralHelper

    Private _lock_download_stopped As New Object

    Public Function IsDownloadStopped() As Boolean

        SyncLock _lock_download_stopped
            Return CBool(Application.Current.Resources("DownloadStopped"))
        End SyncLock

    End Function

    Public Sub SetDownloadStoppedFlag(ByVal _flag As Boolean)


        SyncLock _lock_download_stopped
            Application.Current.Resources("DownloadStopped") = _flag
        End SyncLock

    End Sub


End Module
