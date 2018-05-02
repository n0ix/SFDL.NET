Public Class NativeMethods

    Protected Friend Declare Function SetThreadExecutionState Lib "kernel32" (ByVal esflags As EXECUTION_STATE) As EXECUTION_STATE
    Protected Friend Declare Function AttachConsole Lib "kernel32.dll" (ByVal dwProcessId As Int32) As Boolean
    Protected Friend Declare Function FreeConsole Lib "kernel32.dll" () As Boolean
    Protected Friend Declare Function SetSuspendState Lib "Powrprof.dll" (ByVal hiberate As Boolean, ByVal forceCritical As Boolean, ByVal disableWakeEvent As Boolean) As Boolean


    Public Enum EXECUTION_STATE
        ' Stay in working state by resetting display idle timer
        ES_SYSTEM_REQUIRED = &H1
        ' Force display on by resetting system idle timer
        ES_DISPLAY_REQUIRED = &H2
        ' Force this state until next ES_CONTINUOUS call
        ' and one of the other flags are cleared
        ES_CONTINUOUS = &H80000000
    End Enum

End Class