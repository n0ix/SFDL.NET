Imports System.IO
Imports System.Windows.Forms
Imports Microsoft.Win32

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

    Public Function EnableLongPathSupport() As Boolean

        Dim reg As RegistryKey = Nothing

        Try

            Registry.LocalMachine.CreateSubKey("SYSTEM\CurrentControlSet\Control\FileSystem")
            reg = Registry.LocalMachine.OpenSubKey("SYSTEM\CurrentControlSet\Control\FileSystem", True)
            reg.SetValue("LongPathsEnabled", 1, RegistryValueKind.DWord)

        Catch ex As Exception
            Return False
        Finally
            If reg IsNot Nothing Then
                reg.Dispose()
            End If
        End Try

        Return True

    End Function

    Public Function IsLongPathEnabled() As Boolean

        If Registry.LocalMachine.OpenSubKey("SYSTEM\CurrentControlSet\Control\FileSystem").GetValue("LongPathsEnabled") = 1 Then
            Return True
        Else
            Return False
        End If

    End Function

    Public Function GetFreeDiskSpaceForPath(ByVal _path As String) As Double

        Dim diskLetter As String = IO.Path.GetPathRoot(_path)

        If diskLetter.StartsWith("\\?\") Then
            diskLetter = diskLetter.Replace("\\?\", "")
        End If

        If diskLetter.EndsWith(":\") Then
            diskLetter = diskLetter.Replace(":\", "")
        End If

        Dim driveInfo As DriveInfo = New DriveInfo(diskLetter)

        Return driveInfo.AvailableFreeSpace

    End Function

End Module
