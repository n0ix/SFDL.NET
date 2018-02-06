Imports System.Collections.ObjectModel
Imports System.Collections.Specialized

Public Class ObservableCollectionEx(Of T)
    Inherits ObservableCollection(Of T)

    Private suppressOnCollectionChanged As Boolean

    Public Sub AddRange(items As IEnumerable(Of T))
        If items Is Nothing Then
            Throw New ArgumentNullException("items")
        End If
        If items.Any() Then
            Try
                Me.suppressOnCollectionChanged = True
                For Each item As Object In items
                    Me.Add(item)
                Next
            Finally
                Me.suppressOnCollectionChanged = False
                Me.OnCollectionChanged(New NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset))
            End Try
        End If
    End Sub
    Public Sub RemoveRange(items As IEnumerable(Of T))
        If items Is Nothing Then
            Throw New ArgumentNullException("items")
        End If
        If items.Any() Then
            Try
                Me.suppressOnCollectionChanged = True

                For Each item As Object In items
                    Me.Remove(item)
                Next

            Finally
                Me.suppressOnCollectionChanged = False
                Me.OnCollectionChanged(New NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset))
            End Try
        End If
    End Sub


    Protected Overloads Overrides Sub OnCollectionChanged(e As NotifyCollectionChangedEventArgs)
        If Not Me.suppressOnCollectionChanged Then
            MyBase.OnCollectionChanged(e)
        End If
    End Sub

End Class