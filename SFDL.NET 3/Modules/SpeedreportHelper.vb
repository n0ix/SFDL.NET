Module SpeedreportHelper

    Dim _log As NLog.Logger = NLog.LogManager.GetLogger("SpeedreportHelper")

    Private Function SecToHMS(ByVal Sec As Double) As String
        '
        Dim ts As TimeSpan
        Dim totHrs As Integer
        Dim H, M, S, HMS As String
        '
        'Place milliseconds into timespand variable
        'to expose conversion properties
        ts = TimeSpan.FromSeconds(Sec)
        '
        'Get H M S values and format for leading zero
        'Add a trailing semi colon on Hours and minutes
        'Total hours will allow display of more than 24 hrs
        'while minutes and seconds will be limited to 0-59
        '
        totHrs = CInt(Math.Truncate(ts.TotalHours)) 'strip away decimal points
        H = Format(totHrs, "0#") & ":"
        M = Format(ts.Minutes, "0#") & ":"
        S = Format(ts.Seconds, "0#")
        '
        'Combine Hours Minutes and seconds into HH:MM:SS string
        HMS = H & M & S
        '
        Return HMS

    End Function

    Private Function CalculateSizeAsMB(ByVal _item_list As List(Of DownloadItem)) As Double

        Dim _full_session_size As Double

        _full_session_size = _item_list.Aggregate(_full_session_size, Function(current, _file) current + _file.SizeDownloaded)

        _full_session_size = (_full_session_size / 1024) / 1024

        Return _full_session_size

    End Function

    Private Function CalculateSpeed(ByVal _starttime As Date, ByVal _stoptime As Date, ByVal _item_list As List(Of DownloadItem)) As Double

        Dim _full_session_size As Double
        Dim _time_elapsed As Double
        Dim _rt As Double

        _full_session_size = _item_list.Aggregate(_full_session_size, Function(current, _file) current + _file.SizeDownloaded)

        _full_session_size = _full_session_size / 1024

        _time_elapsed = DateDiff(DateInterval.Second, _starttime, _stoptime)

        _rt = _full_session_size / _time_elapsed

        Return _rt

    End Function

    Function GenerateSimpleSpeedreport(ByVal session As ContainerSession) As String

        Dim _rt_speedreport As String = String.Empty
        Dim _speed As Double = 0
        Dim _size As Double = 0

        Try


            Dim _tmp_list As New List(Of DownloadItem)

            For Each _item In session.DownloadItems

                If Not _item.SizeDownloaded = 0 Then
                    _tmp_list.Add(_item)
                End If

            Next

            _size = CalculateSizeAsMB(_tmp_list)
            _speed = CalculateSpeed(session.DownloadStartedTime, session.DownloadStoppedTime, _tmp_list)

            _rt_speedreport = String.Format(My.Resources.Strings.GenerateSimpleSpeedreport_AppTask_Completed_Message, Math.Round(_size, 2) & " MB", SecToHMS(DateDiff(DateInterval.Second, session.DownloadStartedTime, session.DownloadStoppedTime)), Math.Round(_speed, 2) & " KB/s")

        Catch ex As NoSpeedreportDataException
            _log.Info("Nothing was downloaded - Skipping Speedreport generation")
            _rt_speedreport = "nodata"

        Catch ex As Exception
            _log.Error(ex, ex.Message)
        End Try

        Return _rt_speedreport

    End Function

    Function GenerateSpeedreport(ByVal session As ContainerSession, ByVal _speedreportSettings As SpeedreportSettings) As String

        Dim _rt_speedreport As String = String.Empty
        Dim _speed As Double = 0
        Dim _size As Double = 0

        Try


            Dim _tmp_list As New List(Of DownloadItem)

            For Each _item In session.DownloadItems

                If Not _item.SizeDownloaded = 0 Then
                    _tmp_list.Add(_item)
                End If

            Next

            _size = CalculateSizeAsMB(_tmp_list)
            _speed = CalculateSpeed(session.DownloadStartedTime, session.DownloadStoppedTime, _tmp_list)

            If Math.Round(_size, 2) = 0 Then
                Throw New NoSpeedreportDataException
            End If

            _rt_speedreport = _speedreportSettings.SpeedreportTemplate

            _rt_speedreport = _rt_speedreport.Replace("%%USERNAME%%", _speedreportSettings.SpeedreportUsername)
            _rt_speedreport = _rt_speedreport.Replace("%%CONNECTION%%", _speedreportSettings.SpeedreportConnection)
            _rt_speedreport = _rt_speedreport.Replace("%%COMMENT%%", _speedreportSettings.SpeedreportComment)
            _rt_speedreport = _rt_speedreport.Replace("%%SPEED%%", Math.Round(_speed, 2) & " KB/s")
            _rt_speedreport = _rt_speedreport.Replace("%%SFDL_FILENAME%%", session.ContainerFileName)
            _rt_speedreport = _rt_speedreport.Replace("%%SFDL_UPPER%%", session.ContainerFile.Uploader)
            _rt_speedreport = _rt_speedreport.Replace("%%DLTIME%%", SecToHMS(DateDiff(DateInterval.Second, session.DownloadStartedTime, session.DownloadStoppedTime)))
            _rt_speedreport = _rt_speedreport.Replace("%%SFDL_SIZE%%", Math.Round(_size, 2) & " MB")

        Catch ex As NoSpeedreportDataException
            _log.Info("Nothing was downloaded - Skipping Speedreport generation")
            _rt_speedreport = "nodata"

        Catch ex As Exception
            _log.Error(ex, ex.Message)
        End Try

        Return _rt_speedreport

    End Function

End Module
