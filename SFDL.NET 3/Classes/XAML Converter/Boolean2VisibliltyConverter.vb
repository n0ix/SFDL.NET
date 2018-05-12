
Imports System.Globalization


Public Class Boolean2VisibliltyConverter
    Implements IValueConverter
    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        Dim original As Boolean = CBool(value)

        If original = True Then
            Return Visibility.Visible
        Else
            Return Visibility.Hidden
        End If

    End Function
    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack

        Dim original As Visibility = value

        If original = Visibility.Visible Then
            Return True
        Else
            Return False
        End If

    End Function
End Class
