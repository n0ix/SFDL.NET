﻿Imports System.IO
Imports System.Security.Principal

Module FileRegisterHelper

    Public Function isAdministrator() As Boolean

        Dim pricipal As New WindowsPrincipal(WindowsIdentity.GetCurrent())
        Dim hasAdministrativeRight As Boolean = pricipal.IsInRole(WindowsBuiltInRole.Administrator)

        Return hasAdministrativeRight

    End Function

    Public Sub UpdateInstallState()

        Dim _sw As StreamWriter

        Try

            _sw = New StreamWriter(IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, ".installstate"), False)
            _sw.WriteLine(IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory).ToString)
            _sw.Flush()
            _sw.Close()

        Catch ex As Exception

        End Try

    End Sub

    Public Function CheckInstallState() As Boolean

        Dim _rt As Boolean = False

        If IO.File.Exists(IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, ".installstate")) Then

            Dim _installstate_path As String

            _installstate_path = My.Computer.FileSystem.ReadAllText(IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, ".installstate")).Trim

            If _installstate_path.Equals(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory).ToString) = False Then
                _rt = False
            Else
                _rt = True
            End If

        Else
            _rt = False
        End If

        Return _rt

    End Function

    Public Sub RunAsAdmin()

        Dim _runas As String
        Dim _process As New Process

        _runas = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "runasadmin.exe")

        _process.StartInfo.FileName = _runas
        _process.StartInfo.WorkingDirectory = System.AppDomain.CurrentDomain.BaseDirectory

        _process.Start()

        Application.Current.Shutdown()

    End Sub

End Module
