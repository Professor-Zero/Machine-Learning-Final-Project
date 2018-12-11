Imports System.Threading


Public Enum OutputFileType

    Space
    Comma
    Excel

End Enum



Public Enum FileLocation

    File
    Directory

End Enum


Public Enum InputType

    Daily
    Weekly
    Monthly

End Enum


Public Enum CalculationType

    SPI
    Frequency
    DroughtPeriod

End Enum


Public Enum AggregateType

    Month = 12
    Week = 52

End Enum




Friend Class SPI

    Friend Shared DroughtPeriodLevels As List(Of Double) = {-1, -1.5, -2, -2.5, -3, -3.5, -4}.ToList()



#Region "Friend TimeScale As Integer"
    Friend TimeScale As Integer
#End Region

#Region "Friend SPI As List(Of StandardPrecipitationIndexValue)"
    Friend SPI As List(Of StandardPrecipitationIndexValue)
#End Region

#Region "Friend Frequencies As List(Of StandardPrecipitationIndexFrequency)"
    Friend Frequencies As List(Of StandardPrecipitationIndexFrequency)
#End Region

#Region "Friend DroughtPeriods As Dictionary(Of Double, List(Of DroughtPeriod))"
    Friend DroughtPeriods As Dictionary(Of Double, List(Of DroughtPeriod))
#End Region




#Region "Friend Sub New()"
    Friend Sub New()

        Thread.CurrentThread.CurrentCulture = SPICollection.Culture
        Thread.CurrentThread.CurrentUICulture = SPICollection.Culture

        SPI = New List(Of StandardPrecipitationIndexValue)()
        Frequencies = New List(Of StandardPrecipitationIndexFrequency)()
        DroughtPeriods = New Dictionary(Of Double, List(Of DroughtPeriod))()

    End Sub
#End Region

#Region "Friend Sub New(ByVal timeScale As Integer)"
    Friend Sub New(ByVal timeScale As Integer)

        Me.New()
        Me.TimeScale = timeScale

    End Sub
#End Region




#Region "Friend Sub CalculateByMonth(ByVal Precipitation As Dictionary(Of DateTime, Double))"
    Friend Sub CalculateByMonth(ByVal Precipitation As Dictionary(Of DateTime, Double))

        SPI = New List(Of StandardPrecipitationIndexValue)()

        '*** summing precipitation by timescale
        Dim dicPrecipitation As Dictionary(Of DateTime, Double) = AggregateByTimeScale(TimeScale, Precipitation)

        '*** parsing data by month over all years
        For i As Integer = 0 To AggregateType.Month - 1

            Dim i2 As Integer = i
            Dim months As IEnumerable(Of KeyValuePair(Of DateTime, Double)) = (From n In dicPrecipitation Where Convert.ToDateTime(n.Key, SPICollection.Culture).Month = (i2 + 1) Select n)

            Dim req(months.Count - 1) As Double
            For counter As Integer = 0 To months.Count() - 1
                req(counter) = months.ElementAt(counter).Value
            Next

            '*** calculating spi
            Dim spiResults() As Double = StandardPrecipitationIndex.Parse(req)

            '*** saving spi
            For j As Integer = 0 To spiResults.Length - 1

                Dim spiVal As StandardPrecipitationIndexValue = New StandardPrecipitationIndexValue()
                spiVal.Date = Convert.ToDateTime((i + 1).ToString() + "/1/" + months.ElementAt(j).Key.Year.ToString(), SPICollection.Culture)
                spiVal.SPI = spiResults(j)
                SPI.Add(spiVal)

            Next

        Next

        '*** -99 timescale-1
        NullTimeScale(TimeScale)

    End Sub
#End Region

#Region "Friend Sub CalculateByWeek(ByVal Precipitation As Dictionary(Of DateTime, Double))"
    Friend Sub CalculateByWeek(ByVal Precipitation As Dictionary(Of DateTime, Double))

        SPI = New List(Of StandardPrecipitationIndexValue)()

        '*** summing precipitation by timescale
        Dim dicPrecipitation As Dictionary(Of DateTime, Double) = AggregateByTimeScale(TimeScale * 4 - 1, Precipitation)

        '*** parsing data by week over all years
        For i As Integer = 0 To AggregateType.Week - 1

            Dim int4Linq As Integer = i
            Dim weeks As IEnumerable(Of KeyValuePair(Of DateTime, Double)) = (From n In dicPrecipitation Where Convert.ToDateTime(n.Key, SPICollection.Culture).DayOfYear = (int4Linq * 7 + 1) Select n)

            Dim req(weeks.Count - 1) As Double
            For counter As Integer = 0 To weeks.Count() - 1
                req(counter) = weeks.ElementAt(counter).Value
            Next

            '*** calculating spi
            Dim spiResults() As Double = StandardPrecipitationIndex.Parse(req)

            '*** saving spi
            For j As Integer = 0 To spiResults.Length - 1

                Dim spiVal As StandardPrecipitationIndexValue = New StandardPrecipitationIndexValue()
                spiVal.Date = Convert.ToDateTime(weeks.ElementAt(j).Key.ToString(), SPICollection.Culture)
                spiVal.SPI = spiResults(j)
                SPI.Add(spiVal)

            Next

        Next

        '*** -99 timescale-1
        NullTimeScale(TimeScale * 4)

    End Sub
#End Region

#Region "Friend Sub CalculateDroughtPeriods()"
    Friend Sub CalculateDroughtPeriods()

        Dim prd As List(Of DroughtPeriod) = New List(Of DroughtPeriod)()

        If SPI IsNot Nothing AndAlso SPI.Count > 0 Then

            For Each lvl In DroughtPeriodLevels

                '*** calculating drought periods
                prd = StandardPrecipitationIndex.DroughtPeriods(SPI.ToArray(), lvl).ToList()
                DroughtPeriods.Add(lvl, prd)

            Next

        End If

    End Sub
#End Region

#Region "Friend Sub CalculateFrequencies()"
    Friend Sub CalculateFrequencies()

        Frequencies = New List(Of StandardPrecipitationIndexFrequency)()

        If SPI IsNot Nothing AndAlso SPI.Count > 0 Then

            '*** calculating frequencies
            Frequencies = StandardPrecipitationIndex.Frequency(SPI.ToArray()).ToList()

        End If

    End Sub
#End Region

#Region "Private Shared Function AggregateByTimeScale(ByVal timescale As Integer, ByVal dicOriginalPrecipitation As Dictionary(Of DateTime, Double)) As Dictionary(Of DateTime, Double)"
    Private Shared Function AggregateByTimeScale(ByVal timescale As Integer, ByVal dicOriginalPrecipitation As Dictionary(Of DateTime, Double)) As Dictionary(Of DateTime, Double)

        Dim dicPrecipitation As Dictionary(Of DateTime, Double) = New Dictionary(Of DateTime, Double)()

        For i As Integer = 0 To dicOriginalPrecipitation.Count - 1

            Dim val As Double = 0
            Dim k As Integer = 0

            If i < timescale - 1 Then

                val = StandardPrecipitationIndex.Missing(0)

            Else

                '*** start aggregating right away
                While k < timescale ' And k <= i

                    If StandardPrecipitationIndex.Missing.Contains(dicOriginalPrecipitation.ElementAt(i - k).Value) Then
                        val = StandardPrecipitationIndex.Missing(0)
                        Exit While
                    Else
                        val += dicOriginalPrecipitation.ElementAt(i - k).Value
                    End If

                    k = k + 1

                End While

            End If

            dicPrecipitation.Add(dicOriginalPrecipitation.ElementAt(i).Key, val)

        Next

        Return dicPrecipitation

    End Function
#End Region



#Region "Private Sub NullTimeScale(ByVal timescale As Integer)"
    Private Sub NullTimeScale(ByVal timescale As Integer)

        ' making sure values are ordered correctly.
        SPI = (From n In SPI Order By n.Date Select n).ToList()

        ' -99 timescale-1, because these values don't mean anything.
        For i As Integer = 0 To timescale - 2

            Dim tempspi As StandardPrecipitationIndexValue = New StandardPrecipitationIndexValue()
            tempspi.Date = SPI(i).Date
            tempspi.SPI = StandardPrecipitationIndex.Missing(0)

            SPI.RemoveAt(i)
            SPI.Insert(i, tempspi)

        Next

    End Sub
#End Region


End Class
