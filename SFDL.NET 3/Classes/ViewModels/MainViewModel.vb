﻿Imports SFDL.NET3

Public Class MainViewModel
    Inherits ViewModelBase


#Region "Private Subs"

    Private Sub OpenSFDLFile(ByVal _sfdl_container_path As String)

        Dim _mytask As New Task(String.Format("SFDL Datei {0} wird geöffnet...", _sfdl_container_path))
        Dim _mycontainer As New Container.Container
        Dim _mycontainer_session As ContainerSession
        Dim _decrypt_password As String
        Dim _decrypt As New SFDL.Container.Decrypt


        AddHandler _mytask.TaskDone, AddressOf TaskDoneEvent

        ActiveTasks.Add(_mytask)

        System.Threading.Tasks.Task.Run(Sub()

                                            Try

                                                If GetContainerVersion(_sfdl_container_path) = 0 Or GetContainerVersion(_sfdl_container_path) > 10 Then
                                                    Throw New Exception("Diese SFDL Datei ist mit dieser Programmversion nicht kompatibel!")
                                                End If

                                                _mycontainer = XMLHelper.XMLDeSerialize(_mycontainer, _sfdl_container_path)

                                                If _mycontainer.Encrypted = True Then

                                                    Try
Decrypt:
                                                        _decrypt_password = MahApps.Metro.Controls.Dialogs.DialogCoordinator.Instance.ShowInputAsync(Me, "SFDL entschlüsseln", "Bitte gib ein Passwort ein um den SFDL Container zu entschlüsseln").Result

                                                        If String.IsNullOrWhiteSpace(_decrypt_password) Then
                                                            Throw New Exception("SFDL entschlüsseln abgebrochen")
                                                        End If

                                                        _decrypt.DecryptString(_mycontainer.Connection.Host, _decrypt_password)

                                                    Catch ex As SFDL.Container.FalsePasswordException
                                                        GoTo Decrypt
                                                    End Try

                                                    DecryptSFDLContainer(_mycontainer, _decrypt_password)

                                                End If

                                                _mycontainer_session = New ContainerSession(_mycontainer)
                                                _mycontainer_session.ContainerFileName = IO.Path.GetFileNameWithoutExtension(_sfdl_container_path)
                                                _mycontainer_session.ContainerFilePath = _sfdl_container_path

                                                If GetBulkFileList(_mycontainer_session) = True Then

                                                    GenerateContainerSessionDownloadItems(_mycontainer_session)

                                                    DispatchService.DispatchService.Invoke(Sub()

                                                                                               For Each _item In _mycontainer_session.DownloadItems
                                                                                                   DownloadItems.Add(_item)
                                                                                               Next

                                                                                               ContainerSessions.Add(_mycontainer_session)

                                                                                           End Sub)

                                                Else
                                                    Throw New Exception("Bulk Filelist konnte nicht ermittelt werden")
                                                End If

                                            Catch ex As Exception
                                                _mytask.SetTaskStatus(TaskStatus.Faulted, ex.Message)
                                            End Try

                                            _mytask.SetTaskStatus(TaskStatus.RanToCompletion, "SFDL geöffnet")

                                        End Sub)



    End Sub


#End Region

#Region "Button States"

    Private _button_downloadstart_enabled As Boolean

    Public Property ButtonDownloadStartEnabled As Boolean
        Set(value As Boolean)
            _button_downloadstart_enabled = value
            RaisePropertyChanged("ButtonDownloadStartEnabled")
        End Set
        Get
            Return _button_downloadstart_enabled
        End Get
    End Property

    Private _button_downloadstop_enabled As Boolean

    Public Property ButtonDownloadStopEnabled As Boolean
        Set(value As Boolean)
            _button_downloadstop_enabled = value
            RaisePropertyChanged("ButtonDownloadStopEnabled")
        End Set
        Get
            Return _button_downloadstop_enabled
        End Get
    End Property

    Private _button_instantvideo_enabled As Boolean
    Public Property ButtonInstantVideoEnabled As Boolean
        Set(value As Boolean)
            _button_instantvideo_enabled = value
            RaisePropertyChanged("ButtonInstantVideoEnabled")
        End Set
        Get
            Return _button_instantvideo_enabled
        End Get
    End Property



#End Region

#Region "Menu Commands"

    Public ReadOnly Property ShowSettingsDialogCommand() As ICommand
        Get
            Return New DelegateCommand(AddressOf ShowSettingsDialog)
        End Get
    End Property

    Private Sub ShowSettingsDialog()
        Dim _settings_dialog As New SettingsWindow
        _settings_dialog.ShowDialog()
    End Sub

    Public ReadOnly Property ExitApplicationCommand() As ICommand
        Get
            Return New DelegateCommand(AddressOf ExitApplication)
        End Get
    End Property

    Private Sub ExitApplication()
        Application.Current.Shutdown()
    End Sub

    Public ReadOnly Property OpenSFDLCommand() As ICommand
        Get
            Return New DelegateCommand(AddressOf OpenSFDL)
        End Get
    End Property

    Private Sub OpenSFDL()

        Dim _ofd As New Microsoft.Win32.OpenFileDialog()

        With _ofd

            .Multiselect = True
            .Title = "SFDL Datei(en) öffnen"
            .Filter = "SFDL Files (*.sfdl)|*.sfdl"

        End With

        If Not _ofd.ShowDialog = vbCancel Then

            For Each _file In _ofd.FileNames
                OpenSFDLFile(_file)
            Next

        End If

    End Sub


#End Region

#Region "Tasks"

    Private _active_tasks As New ObjectModel.ObservableCollection(Of Task)
    Public Property ActiveTasks As ObjectModel.ObservableCollection(Of Task)
        Set(value As ObjectModel.ObservableCollection(Of Task))
            _active_tasks = value
            RaisePropertyChanged("ActiveTasks")
        End Set
        Get
            Return _active_tasks
        End Get
    End Property

    Private Sub TaskDoneEvent(e As Task)

        'Wait 5 Seconds
        System.Threading.Thread.Sleep(5000)
        DispatchService.DispatchService.Invoke(Sub()
                                                   ActiveTasks.Remove(e)
                                               End Sub)

    End Sub


#End Region

#Region "Container Sessions"

    Private _container_sessions As New ObjectModel.ObservableCollection(Of ContainerSession)

    Public Property ContainerSessions As ObjectModel.ObservableCollection(Of ContainerSession)
        Set(value As ObjectModel.ObservableCollection(Of ContainerSession))
            _container_sessions = value
            RaisePropertyChanged("ContainerSessions")
        End Set
        Get
            Return _container_sessions
        End Get
    End Property

    Private _download_items As New ObjectModel.ObservableCollection(Of DownloadItem)

    Public Property DownloadItems As ObjectModel.ObservableCollection(Of DownloadItem)
        Set(value As ObjectModel.ObservableCollection(Of DownloadItem))
            _download_items = value
            RaisePropertyChanged("DownloadItems")
        End Set
        Get

            '_download_items.Clear()

            'For Each _session In ContainerSessions

            '    For Each _item In _session.DownloadItems
            '        _download_items.Add(_item)
            '    Next
            'Next

            Return _download_items
        End Get
    End Property

#End Region

End Class
