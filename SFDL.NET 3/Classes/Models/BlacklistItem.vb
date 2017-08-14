Imports System.Text.RegularExpressions

Public Class BlacklistItem

    Private m_filenameregex As String = String.Empty
    Private m_type As Type
    Public Sub New(ByVal _type As Type, Optional _filename_regex As String = "")
        m_filenameregex = _filename_regex
        m_type = _type
    End Sub

    Public Property FileNameRegex As String
        Set(value As String)
            m_filenameregex = value
        End Set
        Get
            Return m_filenameregex
        End Get
    End Property

    Public Property ItemType As Type
        Set(value As Type)
            m_type = value
        End Set
        Get
            Return m_type
        End Get
    End Property

    Public Property Hashes As New List(Of String)

    Public Enum Type
        User
        Malicious
    End Enum

End Class
