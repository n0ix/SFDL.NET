Imports System.Windows
Imports MahApps.Metro.Controls
Imports MahApps.Metro.SimpleChildWindow

Partial Public Class SelectContainerDownloadPathPopUp
    Inherits ChildWindow

    Public Sub New()

        Dim _mvvm As New SelectContainerDownloadPathPopUpVieModel

        Me.InitializeComponent()

        _mvvm.WindowInstance = Me

        Me.DataContext = _mvvm

    End Sub

    Private Sub CMD_OK_Click(sender As Object, e As RoutedEventArgs)

        Me.Close()

    End Sub
End Class
