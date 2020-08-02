Imports System.IO
Imports System.Net.Http

Module WhoisHelper


    Private _log As NLog.Logger = NLog.LogManager.GetLogger("WhoisHelper")

    ''' <summary>
    ''' Funktion zum umsetzten der IP-Addresse in einen Ländercode
    ''' </summary>
    Public Async Function Resolve(ByVal _ip As String) As Task(Of WhoIsResult)

        Dim XMLReader As New Xml.XmlDocument
        Dim xmlKD As Xml.XmlElement
        Dim _country_code As String
        Dim _local_xml As String
        Dim _rt As New WhoIsResult

        _log.Info(String.Format("Queryring WohIs für IP {0}", _ip))

        _country_code = "N/A"

        _local_xml = Await HttpHelper.DownloadFile2Temp("http://xml.utrace.de/?query=" & _ip)

        XMLReader.Load(_local_xml)

        xmlKD = CType(XMLReader.DocumentElement.ChildNodes(0), Xml.XmlElement)

        For Each _node As Xml.XmlNode In xmlKD.ChildNodes

            If _node.Name = "countrycode" Then

                _country_code = _node.ChildNodes(0).Value

            End If

        Next

        _log.Info(String.Format("CounterCode determined : {0}", _country_code))


        _rt.CountryCode = _country_code

        If IO.File.Exists(_local_xml) Then 'CleanUp
            IO.File.Delete(_local_xml)
        End If

        Return _rt

    End Function


End Module
