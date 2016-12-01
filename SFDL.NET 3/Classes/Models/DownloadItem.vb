﻿Imports System.ComponentModel
Imports SFDL.Container
Imports SFDL.NET3.My.Resources

Public Class DownloadItem
    Inherits FileItem
    Implements IDisposable
    Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Public Sub RaisePropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Private _download_speed As String = String.Empty
    Private _selected As Boolean = False
    Private _download_progress As Integer = 0
    Private _parent_container_id As Guid
    Private _status_image As String = "Resources/Icons/appbar.sign.parking.png"
    Private _status As Status = Status.None
    Private _id As Guid


    Public Sub Init(ByVal _fileitem As FileItem)

        DirectoryPath = _fileitem.DirectoryPath
        DirectoryRoot = _fileitem.DirectoryRoot
        DownloadProgress = 0
        DownloadSpeed = String.Empty
        DownloadStatus = Status.Queued
        FileName = _fileitem.FileName
        FileHash = _fileitem.FileHash
        FileSize = _fileitem.FileSize
        FullPath = _fileitem.FullPath
        HashType = _fileitem.HashType
        PackageName = _fileitem.PackageName
        isSelected = True

        _id = New Guid

    End Sub


    Public Property isSelected As Boolean
        Set(value As Boolean)

            If (DownloadStatus = Status.Running Or DownloadStatus = Status.Retry) Or DownloadStatus = Status.RetryWait Then
                'do nothing
            Else

                _selected = value

                If value = True Then
                    DownloadStatus = Status.Queued
                Else
                    DownloadStatus = Status.None
                End If

                RaisePropertyChanged("isSelected")

            End If

        End Set
        Get
            Return _selected
        End Get
    End Property
    Public Property DownloadProgress As Integer
        Set(value As Integer)
            _download_progress = value
            RaisePropertyChanged("DownloadProgress")
        End Set
        Get
            Return _download_progress
        End Get
    End Property

    Public ReadOnly Property DownloadStatusString
        Get

            Select Case DownloadStatus

                Case Status.None

                    Return String.Empty

                Case Status.Queued

                    Return Strings.DownloadStatus_Queued

                Case Status.Running

                    Return Strings.DownloadStatus_Running

                Case Status.Stopped

                    Return Strings.DownloadStatus_Stopped

                Case Status.Completed

                    Return Strings.DownloadStatus_Completed

                Case Status.Completed_HashInvalid

                    Return Strings.DownloadStatus_Completed_HashInvalid

                Case Status.Completed_HashValid

                    Return Strings.DownloadStatus_Completed_HashValid

                Case Status.Failed

                    Return Strings.DownloadStatus_Failed

                Case Status.Failed_FileNameTooLong

                    Return Strings.DownloadStatus_Failed_FileNameTooLong

                Case Status.Failed_NotEnoughDiskSpace

                    Return Strings.DownloadStatus_Failed_NotEnoughDiskSpace

                Case Status.Failed_ServerFull

                    Return Strings.DownloadStatus_Failed_ServerFull

                Case Status.Failed_ServerDown

                    Return Strings.DownloadStatus_Failed_ServerDown

                Case Status.Failed_AuthError

                    Return Strings.DownloadStatus_Failed_AuthError

                Case Status.Failed_ConnectionError

                    Return Strings.DownloadStatus_Failed_ConnectionError

                Case Status.Failed_FileNotFound

                    Return Strings.DownloadStatus_Failed_FileNotFound

                Case Status.Failed_DirectoryNotFound

                    Return Strings.DownloadStatus_Failed_DirectoryNotFound

                Case Status.Failed_InternalServerError

                    Return Strings.DownloadStatus_Failed_InternalServerError

                Case Status.RetryWait

                    Return Strings.DownloadStatus_RetryWait

                Case Status.Retry

                    Return Strings.DownloadStatus_Retry

                Case Status.AlreadyDownloaded

                    Return Strings.DownloadStatus_AlreadyDownloaded

                Case Else

                    Return Strings.DownloadStatus_Failed

            End Select

        End Get
    End Property

    Public Property DownloadStatus As Status
        Set(value As Status)

            _status = value

            RaisePropertyChanged("DownloadStatus")
            RaisePropertyChanged("DownloadStatusString")

            Select Case value

                Case Status.None

                    DownloadStatusImage = "None"

                Case Status.Completed

                    DownloadStatusImage = "Completed"

                Case Status.Queued

                    DownloadStatusImage = "Queued"

                Case Status.Running

                    DownloadStatusImage = "Running"

                Case Status.Stopped

                    DownloadStatusImage = "Stopped"

                Case Status.Completed_HashValid

                    DownloadStatusImage = "Completed_HashValid"

                Case Status.Completed_HashInvalid

                    DownloadStatusImage = "Completed_HashInvalid"

                Case Status.Failed

                    DownloadStatusImage = "Failed"

                Case Status.Failed_FileNameTooLong

                    DownloadStatusImage = "Failed_FileNameTooLong"

                Case Status.Failed_NotEnoughDiskSpace

                    DownloadStatusImage = "Failed_NotEnoughDiskSpace"

                Case Status.Failed_ServerFull

                    DownloadStatusImage = "Failed_ServerFull"

                Case Status.Failed_ServerDown

                    DownloadStatusImage = "Failed_ServerDown"

                Case Status.Failed_ConnectionError

                    DownloadStatusImage = "Failed_ConnectionError"

                Case Status.Failed_AuthError

                    DownloadStatusImage = "Failed_AuthError"

                Case Status.Failed_FileNotFound

                    DownloadStatusImage = "Failed_FileNotFound"

                Case Status.Failed_DirectoryNotFound

                    DownloadStatusImage = "Failed_DirectoryNotFound"

                Case Status.Failed_InternalServerError

                    DownloadStatusImage = "Failed_InternalServerError"

                Case Status.Retry

                    DownloadStatusImage = "Retry"

                Case Status.RetryWait

                    DownloadStatusImage = "RetryWait"

                Case Status.AlreadyDownloaded

                    DownloadStatusImage = "AlreadyDownloaded"

            End Select

        End Set
        Get
            Return _status
        End Get
    End Property
    Public Property DownloadSpeed As String
        Set(value As String)
            _download_speed = value
            RaisePropertyChanged("DownloadSpeed")
        End Set
        Get
            Return _download_speed
        End Get
    End Property
    Public Property ParentContainerID As Guid
        Set(value As Guid)
            _parent_container_id = value
            RaisePropertyChanged("ParentContainerID")
        End Set
        Get
            Return _parent_container_id
        End Get
    End Property
    Public Property DownloadStatusImage As String
        Set(value As String)
            _status_image = value
            RaisePropertyChanged("DownloadStatusImage")
        End Set
        Get
            Return _status_image
        End Get
    End Property

    Public ReadOnly Property GroupDescriptionIdentifier As String
        Get
            Return PackageName & ";" & _parent_container_id.ToString
        End Get
    End Property


    Public ReadOnly Property ID As Guid
        Get
            Return _id
        End Get
    End Property

    Public Property SizeDownloaded As Long = 0
    Public Property RetryPossible As Boolean = False
    Public Property RetryCount As Integer = 0
    Public Property LocalFile As String = String.Empty
    Public Property FirstUnRarFile As Boolean = False
    Public Property RequiredForInstantVideo As Boolean = False
    Public Property SingleSessionMode As Boolean = False

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


    Public Enum Status
        None
        Queued
        Running
        Stopped
        RetryWait
        Retry
        Failed
        Failed_FileNameTooLong
        Failed_NotEnoughDiskSpace
        Failed_ServerFull
        Failed_ServerDown
        Failed_ConnectionError
        Failed_AuthError
        Failed_FileNotFound
        Failed_DirectoryNotFound
        Failed_InternalServerError
        Completed
        Completed_HashValid
        Completed_HashInvalid
        AlreadyDownloaded
    End Enum

    Public Enum SimplifiedStatus
        OK
        Running
        Failed
        None
    End Enum

#Region "IDisposable Support"
    Private disposedValue As Boolean ' Dient zur Erkennung redundanter Aufrufe.

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: verwalteten Zustand (verwaltete Objekte) entsorgen.
            End If

            ' TODO: nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalize() weiter unten überschreiben.
            ' TODO: große Felder auf Null setzen.
        End If
        disposedValue = True
    End Sub

    ' TODO: Finalize() nur überschreiben, wenn Dispose(disposing As Boolean) weiter oben Code zur Bereinigung nicht verwalteter Ressourcen enthält.
    'Protected Overrides Sub Finalize()
    '    ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(disposing As Boolean) weiter oben ein.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' Dieser Code wird von Visual Basic hinzugefügt, um das Dispose-Muster richtig zu implementieren.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(disposing As Boolean) weiter oben ein.
        Dispose(True)
        ' TODO: Auskommentierung der folgenden Zeile aufheben, wenn Finalize() oben überschrieben wird.
        ' GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
