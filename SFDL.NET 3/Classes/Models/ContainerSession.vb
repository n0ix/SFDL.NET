Imports System.ComponentModel

<Serializable>
Public Class ContainerSession
    Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Public Sub RaisePropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Private _lock_instant_video_streams As New Object

    Public Sub InitCollectionSync()

        _lock_instant_video_streams = New Object

        Me.InstantVideoStreams = New ObjectModel.ObservableCollection(Of InstantVideoStream)

        BindingOperations.EnableCollectionSynchronization(InstantVideoStreams, _lock_instant_video_streams)

    End Sub

    Public Sub Init(ByVal _container As Container.Container)

        Me.ID = Guid.NewGuid
        Me.ContainerFile = _container
        Me.SessionState = ContainerSessionState.Queued
        Me.InstantVideoStreams = New ObjectModel.ObservableCollection(Of InstantVideoStream)

        BindingOperations.EnableCollectionSynchronization(InstantVideoStreams, _lock_instant_video_streams)

    End Sub

    Public Property ID As Guid
    Public Property ContainerFile As SFDL.Container.Container
    Public Property DisplayName As String = String.Empty
    Public Property ContainerFileName As String = String.Empty
    Public Property ContainerFilePath As String = String.Empty
    Public Property DownloadStartedTime As Date = Date.MinValue
    Public Property DownloadStoppedTime As Date = Date.MinValue
    Public Property UnRarChains As New List(Of UnRARChain)
    Public Property DownloadItems As New List(Of DownloadItem)
    Public Property Fingerprint As String = String.Empty
    Public Property SynLock As New Object
    <Xml.Serialization.XmlIgnore>
    Public Property WIG As Amib.Threading.IWorkItemsGroup = Nothing
    Public Property SingleSessionMode As Boolean = False
    Public Property InstantVideoStreams As ObjectModel.ObservableCollection(Of InstantVideoStream)
    Public Property LocalDownloadRoot As String = String.Empty
    Public Property Priority As Integer = 0

    ''' <summary>
    ''' Used for Grouping!
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Name As String
        Get
            Return String.Format("{0} | Priority: {1}", DisplayName, Priority)
        End Get
    End Property

    Private _is_expanded As Boolean = True

    Public Property IsExpanded As Boolean
        Set(value As Boolean)
            _is_expanded = value
            RaisePropertyChanged("IsExpanded")
        End Set
        Get
            Return _is_expanded
        End Get
    End Property


    Private _session_state_image As String = "None"

    Public Property SessionStateImage As String
        Set(value As String)
            _session_state_image = value
            RaisePropertyChanged("SessionStateImage")
        End Set
        Get
            Return _session_state_image
        End Get
    End Property

    Private _session_state As ContainerSessionState = ContainerSessionState.Queued

    Public Property SessionState As ContainerSessionState
        Set(value As ContainerSessionState)

            _session_state = value

            RaisePropertyChanged("SessionState")

            Select Case _session_state

                Case ContainerSessionState.None

                    SessionStateImage = "None"

                Case ContainerSessionState.DonwloadStopped

                    SessionStateImage = "Stopped"

                Case ContainerSessionState.DownloadComplete

                    SessionStateImage = "Completed"

                Case ContainerSessionState.DownloadRunning

                    SessionStateImage = "Running"

                Case ContainerSessionState.Queued

                    SessionStateImage = "Queued"

            End Select


        End Set
        Get
            Return _session_state
        End Get
    End Property

#Region "Overrides for Grouping"

    Public Overrides Function Equals(ByVal obj As Object) As Boolean
        Return Object.Equals(obj, ID)
    End Function

    Public Overrides Function GetHashCode() As Integer
        Return If(Not IsNothing(ID), ID.GetHashCode(), 0)
    End Function

    Public Overrides Function ToString() As String
        Return If(Not IsNothing(Name), Name.ToString(), String.Empty)
    End Function

#End Region

End Class
