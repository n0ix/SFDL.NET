Imports System.Globalization

Public Class TestConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert

        Dim _my_string As String = CType(value, String)
        Dim _container As ContainerSession = Nothing

        If IsNothing(_my_string) Then
            Return Binding.DoNothing
        End If

        If String.IsNullOrWhiteSpace(_my_string) Then
            Return Binding.DoNothing
        End If

        _container = MainViewModel.ThisInstance.ContainerSessions.Where(Function(mysession) mysession.ID.ToString.Equals(_my_string.ToString)).FirstOrDefault

        If Not IsNothing(_container) Then

            Select Case parameter

                Case "SessionState"

                    Return _container.SessionState.ToString

                Case Else
                    Return String.Empty


            End Select

        Else
            Return Binding.DoNothing
        End If

    End Function
    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack

        Return Binding.DoNothing

    End Function

End Class
