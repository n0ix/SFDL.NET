Imports System.Windows.Forms
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

    Public Function IsDirectoryWritable(dirPath As String) As Boolean
        Try
            Using fs As IO.FileStream = IO.File.Create(IO.Path.Combine(dirPath, IO.Path.GetRandomFileName()), 1, IO.FileOptions.DeleteOnClose)
            End Using
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

End Module
