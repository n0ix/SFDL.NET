
Imports System.IO
Imports System.IO.Pipes
Imports System.Threading
Imports System.Collections.Generic


'''Code from http://web.archive.org/web/20080506103924/http://www.flawlesscode.com/post/2008/02/Enforcing-single-instance-with-argument-passing.aspx
'''
''' <summary>
''' Enforces single instance for an application.
''' </summary>
Public Class SingleInstance
    Implements IDisposable

    Private mutex As Mutex = Nothing
    Private ownsMutex As [Boolean] = False
    Private identifier As Guid = Guid.Empty

    ''' <summary>
    ''' Enforces single instance for an application.
    ''' </summary>
    ''' <param name="identifier">An identifier unique to this application.</param>
    Public Sub New(identifier As Guid)
        Me.identifier = identifier
        mutex = New Mutex(True, identifier.ToString(), ownsMutex)
    End Sub

    ''' <summary>
    ''' Indicates whether this is the first instance of this application.
    ''' </summary>
    Public ReadOnly Property IsFirstInstance() As [Boolean]
        Get
            Return ownsMutex
        End Get
    End Property

    ''' <summary>
    ''' Passes the given arguments to the first running instance of the application.
    ''' </summary>
    ''' <param name="arguments">The arguments to pass.</param>
    ''' <returns>Return true if the operation succeded, false otherwise.</returns>
    Public Function PassArgumentsToFirstInstance(arguments As [String]()) As [Boolean]
        If IsFirstInstance Then
            Throw New InvalidOperationException("This is the first instance.")
        End If

        Try
            Using client As New NamedPipeClientStream(identifier.ToString())
                Using writer As New StreamWriter(client)
                    client.Connect(200)

                    For Each argument As [String] In arguments
                        writer.WriteLine(argument)
                    Next
                End Using
            End Using
            Return True
        Catch generatedExceptionName As TimeoutException
            'Couldn't connect to server
        Catch generatedExceptionName As IOException
        End Try
        'Pipe was broken
        Return False
    End Function

    ''' <summary>
    ''' Listens for arguments being passed from successive instances of the applicaiton.
    ''' </summary>
    Public Sub ListenForArgumentsFromSuccessiveInstances()
        If Not IsFirstInstance Then
            Throw New InvalidOperationException("This is not the first instance.")
        End If
        ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf ListenForArguments))
    End Sub

    ''' <summary>
    ''' Listens for arguments on a named pipe.
    ''' </summary>
    ''' <param name="state">State object required by WaitCallback delegate.</param>
    Private Sub ListenForArguments(state As [Object])
        Try
            Using server As New NamedPipeServerStream(identifier.ToString())
                Using reader As New StreamReader(server)
                    server.WaitForConnection()

                    Dim arguments As New List(Of [String])()
                    While server.IsConnected
                        arguments.Add(reader.ReadLine())
                    End While

                    ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf CallOnArgumentsReceived), arguments.ToArray())
                End Using
            End Using
        Catch generatedExceptionName As IOException
        Finally
            'Pipe was broken
            ListenForArguments(Nothing)
        End Try
    End Sub

    ''' <summary>
    ''' Calls the OnArgumentsReceived method casting the state Object to String[].
    ''' </summary>
    ''' <param name="state">The arguments to pass.</param>
    Private Sub CallOnArgumentsReceived(state As [Object])
        OnArgumentsReceived(DirectCast(state, [String]()))
    End Sub
    ''' <summary>
    ''' Event raised when arguments are received from successive instances.
    ''' </summary>
    Public Event ArgumentsReceived As EventHandler(Of ArgumentsReceivedEventArgs)
    ''' <summary>
    ''' Fires the ArgumentsReceived event.
    ''' </summary>
    ''' <param name="arguments">The arguments to pass with the ArgumentsReceivedEventArgs.</param>
    Private Sub OnArgumentsReceived(arguments As [String]())
        RaiseEvent ArgumentsReceived(Me, New ArgumentsReceivedEventArgs() With {
                .Args = arguments
            })
    End Sub

#Region "IDisposable"
    Private disposed As [Boolean] = False

    Protected Overridable Sub Dispose(disposing As Boolean)

        Try

            DispatchService.DispatchService.Invoke(Sub()

                                                       If Not disposed Then
                                                           If mutex IsNot Nothing AndAlso ownsMutex Then
                                                               mutex.ReleaseMutex()
                                                               mutex = Nothing
                                                           End If
                                                           disposed = True
                                                       End If

                                                   End Sub)

        Catch ex As Exception
        End Try

    End Sub

    Protected Overrides Sub Finalize()
        Try
            Dispose(False)
        Finally
            MyBase.Finalize()
        End Try
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region
End Class
