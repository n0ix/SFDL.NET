Imports System.Net.Http

Module HttpHelper

    Private _log As NLog.Logger = NLog.LogManager.GetLogger("HttpHelper")
    Public Async Function DownloadFile2Temp(ByVal _url As String) As Task(Of String)

        Dim _http_client As HttpClient = New HttpClient
        Dim _http_response As HttpResponseMessage
        Dim _content As HttpContent
        Dim _local_tmp_filepath As String = String.Empty

        Try

            _http_client.BaseAddress = New Uri(_url)
            ' _http_client.Timeout = TimeSpan.FromSeconds(_timeout)

            _http_response = Await _http_client.GetAsync(_url)

            _http_response.EnsureSuccessStatusCode()

            Await _http_response.Content.LoadIntoBufferAsync()

            _content = _http_response.Content

            _local_tmp_filepath = IO.Path.Combine(My.Computer.FileSystem.SpecialDirectories.Temp, IO.Path.GetRandomFileName)

            Using _filestream As New IO.FileStream(_local_tmp_filepath, IO.FileMode.Create, IO.FileAccess.Write)

                Await _content.CopyToAsync(_filestream)

            End Using

        Catch ex As Exception
            _log.Error(ex, ex.Message)
        End Try

        Return _local_tmp_filepath

    End Function


End Module
