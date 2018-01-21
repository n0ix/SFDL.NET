Imports System.ComponentModel
Imports MahApps.Metro.Controls.Dialogs
Public Class SelectContainerDownloadPathPopUpVieModel
    Inherits ViewModelBase
    Implements IDataErrorInfo

#Region "Properties"

    Public Property WindowInstance As SelectContainerDownloadPathPopUp

    Private _download_dir As String = String.Empty

    Public Property DownloadDirectory As String
        Set(value As String)
            _download_dir = value
            RaisePropertyChanged("DownloadDirectory")
        End Set
        Get
            Return _download_dir
        End Get
    End Property

#End Region

#Region "Commands"

    Private Sub SelectDownloadFolder()

        Dim _sdf_dialog As New Forms.FolderBrowserDialog

        With _sdf_dialog
            .ShowNewFolderButton = True
        End With

        If _sdf_dialog.ShowDialog() = Forms.DialogResult.OK Then
            Me.DownloadDirectory = _sdf_dialog.SelectedPath
        End If

    End Sub

    Public ReadOnly Property BrowseFolderCommand() As ICommand
        Get
            Return New DelegateCommand(AddressOf SelectDownloadFolder)
        End Get
    End Property


    Private Sub OK()
        WindowInstance.Close(DownloadDirectory)
    End Sub

    Public ReadOnly Property OKCommand() As ICommand
        Get
            Return New DelegateCommand(AddressOf OK)
        End Get
    End Property

    Private Sub Cancel()
        WindowInstance.Close("Cancel")
    End Sub

    Public ReadOnly Property CancelCommand() As ICommand
        Get
            Return New DelegateCommand(AddressOf Cancel)
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
                Else
                    'Check if Directory is writeable
                    If GeneralHelper.IsDirectoryWritable(Me.DownloadDirectory) = False Then
                        Return My.Resources.Strings.Settings_DownloadDirectory_DirectoryNotWriteable
                    End If
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

#End Region

End Class
