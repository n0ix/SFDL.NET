Imports System.ComponentModel
Imports System.Runtime.CompilerServices

Public Class SelectContainerDownloadPathPopUpVieModel
    Inherits ViewModelBase
    Implements IDataErrorInfo

    Private _closing As Boolean = False
    Private _init As Boolean = True


    Public Sub New(ByVal closeHandler As Action(Of Object))
        _closeCommand = New DelegateCommand(closeHandler)
    End Sub


    Public Async Function GetResult() As Task(Of String)

        Await System.Threading.Tasks.Task.Run(Function()

                                                  While _closing = False
                                                      'Wait
                                                  End While
                                                  Return True
                                              End Function)

        Return _download_dir
    End Function



#Region "Properties"

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

    Private ReadOnly _closeCommand As ICommand

    Private Sub SelectDownloadFolder()

        Dim _sdf_dialog As New Forms.FolderBrowserDialog

        _sdf_dialog.ShowNewFolderButton = True

        If (My.Settings.UserRecentFolderPath IsNot Nothing AndAlso String.IsNullOrWhiteSpace(My.Settings.UserRecentFolderPath) = False) Then
            _sdf_dialog.SelectedPath = My.Settings.UserRecentFolderPath
        End If

        If _sdf_dialog.ShowDialog() = Forms.DialogResult.OK Then
            Me.DownloadDirectory = _sdf_dialog.SelectedPath
            My.Settings.UserRecentFolderPath = _sdf_dialog.SelectedPath
            My.Settings.Save()

        End If

    End Sub

    Public ReadOnly Property BrowseFolderCommand() As ICommand
        Get
            Return New DelegateCommand(AddressOf SelectDownloadFolder)
        End Get
    End Property

    Private Sub Ok()

        _closing = True

        CloseCommand.Execute(Me)

    End Sub

    Public ReadOnly Property OKCommand As ICommand
        Get
            Return New DelegateCommand(AddressOf OK)
        End Get
    End Property

    Public ReadOnly Property CloseCommand As ICommand
        Get
            Return _closeCommand
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
