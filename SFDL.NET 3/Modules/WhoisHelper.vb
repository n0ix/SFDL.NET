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
        _rt.CountryImage = Await DownloadFlagImage(_country_code)

        If IO.File.Exists(_local_xml) Then 'CleanUp
            IO.File.Delete(_local_xml)
        End If

        Return _rt

    End Function

    Private Async Function DownloadFlagImage(ByVal _countrycode As String) As Task(Of BitmapImage)

        Dim _flag As System.Drawing.Image
        Dim _bitmap_flag As BitmapImage
        Dim _local_flag As String
        Dim _memory_stream As MemoryStream

        _local_flag = Await DownloadFile2Temp("http://n1.dlcache.com/flags/" & _countrycode.ToLower & ".gif")

        _flag = System.Drawing.Image.FromFile(_local_flag)

        _bitmap_flag = New BitmapImage

        _bitmap_flag.BeginInit()

        _memory_stream = New MemoryStream

        _flag.Save(_memory_stream, System.Drawing.Imaging.ImageFormat.Bmp)

        _memory_stream.Seek(0, SeekOrigin.Begin)

        _bitmap_flag.StreamSource = _memory_stream

        _bitmap_flag.EndInit()

        _flag.Dispose()

        If IO.File.Exists(_local_flag) Then 'CleanUp
            IO.File.Delete(_local_flag)
        End If

        Return _bitmap_flag

    End Function

End Module
