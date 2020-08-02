Imports System.Windows.Threading
Public Class DispatchService

    Public NotInheritable Class DispatchService
        Private Sub New()
        End Sub
        Public Shared Sub Invoke(action As Action)

            If Application.Current IsNot Nothing Then
                Dim dispatchObject As Dispatcher = Application.Current.Dispatcher
                If dispatchObject Is Nothing OrElse dispatchObject.CheckAccess() Then
                    action()
                Else
                    dispatchObject.Invoke(action)
                End If
            End If

        End Sub
    End Class

End Class
