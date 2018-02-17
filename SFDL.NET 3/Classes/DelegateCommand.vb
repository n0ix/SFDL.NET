Public Class DelegateCommand
    Implements ICommand
    Private ReadOnly _action As Action(Of Object)

    Public Sub New(action As Action(Of Object))
        _action = action
    End Sub

    Public Sub Execute(parameter As Object) Implements ICommand.Execute
        _action.Invoke(parameter)
    End Sub

    Private Function CanExecute(parameter As Object) As Boolean Implements ICommand.CanExecute
        Return True
    End Function

    Public Event CanExecuteChanged As EventHandler
    Private Event ICommand_CanExecuteChanged As EventHandler Implements ICommand.CanExecuteChanged
End Class
