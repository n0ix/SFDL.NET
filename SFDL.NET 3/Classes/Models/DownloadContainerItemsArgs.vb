Public Class DownloadContainerItemsArgs

    Public Property DownloadDirectory As String = String.Empty
    Public Property ConnectionInfo As New SFDL.Container.Connection
    Public Property SingleSessionMode As Boolean = False
    Public Property RetryMode As Boolean = False
    Public Property ChunkMode As Boolean = False

End Class
