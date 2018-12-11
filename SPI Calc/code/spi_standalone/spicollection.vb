Imports System.Globalization
Imports System.IO
Imports System.Threading
Imports DocumentFormat.OpenXml
Imports DocumentFormat.OpenXml.Packaging
Imports DocumentFormat.OpenXml.Spreadsheet



Public Enum Delimiter

    Comma
    Space
    Tab

End Enum


Friend Class SPICollection
    Inherits List(Of SPI)

    Public Shared Culture As CultureInfo = New CultureInfo("en-US", False)

    Private _strFileHeader As String
    Private _strFileName As String
    Private _nfiDecimalFormat As NumberFormatInfo
    Private _strDateFormat As String
    Private _inputType As InputType
    Private _aggType As AggregateType


#Region "Friend Precipitation As Dictionary(Of DateTime, Double)"
    Friend Precipitation As Dictionary(Of DateTime, Double)
#End Region




#Region "Friend Sub New(ByVal InternationalDecimalFormat As Boolean, ByVal InternationalDateFormat As Boolean, ByVal inputType As InputType, ByVal aggregateType As AggregateType)"
    Friend Sub New(ByVal InternationalDecimalFormat As Boolean, ByVal InternationalDateFormat As Boolean, ByVal inputType As InputType, ByVal aggregateType As AggregateType)

        Thread.CurrentThread.CurrentCulture = SPICollection.Culture
        Thread.CurrentThread.CurrentUICulture = SPICollection.Culture

        Precipitation = New Dictionary(Of DateTime, Double)()

        _inputType = inputType
        _aggType = aggregateType

        _nfiDecimalFormat = New NumberFormatInfo()
        If InternationalDecimalFormat Then
            _nfiDecimalFormat.NumberDecimalSeparator = ","
        Else
            _nfiDecimalFormat.NumberDecimalSeparator = "."
        End If

        If InternationalDateFormat Then
            _strDateFormat = "yyyy-MM-dd"
        Else
            _strDateFormat = "MM/dd/yyyy"
        End If

    End Sub
#End Region




#Region "Friend Sub ParseInputFileData(ByVal FilePath As String, ByVal delimiter As Delimiter)"
    Friend Sub ParseInputFileData(ByVal FilePath As String, ByVal delimiter As Delimiter)

        Dim strDelimiter As String = ""

        Select Case delimiter

            Case NationalDroughtMitigationCenter.Delimiter.Comma
                strDelimiter = ","

            Case NationalDroughtMitigationCenter.Delimiter.Space
                strDelimiter = " "

            Case NationalDroughtMitigationCenter.Delimiter.Tab
                strDelimiter = vbTab

        End Select

        ParseInputFile(FilePath, strDelimiter)



        '*** retreiving input data
        If _inputType = NationalDroughtMitigationCenter.InputType.Daily Then

            '*** is the first value the first of january?
            If Precipitation.Count >= 0 AndAlso (Precipitation.First.Key.Month <> 1 Or Precipitation.First.Key.Day <> 1) Then
                Throw New Exception("The Input data doesn't start on January 1.  Application doesn't know how to aggregate data if it doesn't start on the first of they year.  -SMO")
            End If

            If Not (Precipitation.Last.Key.Month = 12 AndAlso Precipitation.Last.Key.Day = 31) Then
                Throw New Exception("Input data doesn't end on December 31.  Application doesn't know how to aggregate data if it doesn't end on the last day of the year.  -SMO")
            End If

            Select Case _aggType

                Case AggregateType.Week
                    CompoundToWeeklyData()

                Case AggregateType.Month
                    CompoundToMonthlyData()

            End Select

        End If


    End Sub
#End Region

#Region "Private Sub ParseInputFile(ByVal FilePath As String, ByVal delimiter As String)"
    Private Sub ParseInputFile(ByVal FilePath As String, ByVal delimiter As String)


        Dim _bolHasColumnDates As Boolean = False

        '*** retreiving input data
        Dim str As String() = New String() {}

        '*** parsing file data
        If File.Exists(FilePath) Then

            '*** saving file name
            Dim fil As FileInfo = New FileInfo(FilePath)
            _strFileName = fil.Name.Replace(fil.Extension, "")

            '*** opening and reading all file data
            str = File.ReadAllLines(FilePath)

            '*** parsing precipitation and temperature from each line
            For counter As Integer = 0 To 1

                Select Case counter

                    Case 0
                        '*** saving the first line for later use
                        _strFileHeader = str(counter)

                    Case 1

                        Dim aRow As String() = str(counter).Split(New String() {delimiter}, StringSplitOptions.RemoveEmptyEntries)

                        Dim test As DateTime

                        If DateTime.TryParse(aRow(0), test) Then
                            _bolHasColumnDates = True
                        Else
                            _bolHasColumnDates = False
                        End If

                End Select

            Next

            If _bolHasColumnDates Then
                ParseInputFileColumnDates(str, delimiter)
            Else
                ParseInputFileHeaderDate(str, delimiter)
            End If

        End If


    End Sub
#End Region

#Region "Friend Shared Function CheckInputData(ByVal input As String, ByVal output As String, ByVal fileLocation As FileLocation, ByVal timeScales As List(Of Integer), ByVal inputType As InputType, ByVal outputType As AggregateType?) As String"
    Friend Shared Function CheckInputData(ByVal input As String, ByVal output As String, ByVal fileLocation As FileLocation, ByVal timeScales As List(Of Integer), ByVal inputType As InputType, ByVal outputType As AggregateType?) As String

        Dim rtn As String = ""
        Dim intTimeScale As Integer = 1

        '*** input file or directory
        If String.IsNullOrEmpty(input) Then
            rtn &= " - Input location can't be opened or doesn't exist." & vbCrLf
        Else

            If fileLocation = NationalDroughtMitigationCenter.FileLocation.File AndAlso Not File.Exists(input) Then
                rtn &= " - Input file can't be opened or doesn't exist." & vbCrLf
            ElseIf fileLocation = NationalDroughtMitigationCenter.FileLocation.Directory AndAlso Not Directory.Exists(input) Then
                rtn &= " - Input directory can't be opened or doesn't exist." & vbCrLf
            End If

        End If

        '*** output directory
        If String.IsNullOrEmpty(output) Or Not Directory.Exists(output) Then
            rtn &= " - Output directory doesn't exist." & vbCrLf
        End If

        '*** time scale
        If timeScales.Count < 1 Then
            rtn &= " - No Time Scales selected." + vbCrLf
        End If

        '*** checking input type
        If IsNothing(inputType) Then
            rtn &= " - the type of input data is required.  i.e.  daily, weekly, or monthly." & vbCrLf
        End If

        '*** output type
        If outputType Is Nothing Then

            rtn &= " - Output type not specified." & vbCrLf

        ElseIf outputType = NationalDroughtMitigationCenter.InputType.Daily Then

            rtn &= " - Unable to process data as daily." & vbCrLf

        End If


        '*** check Aggregate type inputs
        If inputType = NationalDroughtMitigationCenter.InputType.Daily AndAlso outputType = NationalDroughtMitigationCenter.InputType.Daily Then

            rtn &= " - aggregate type argument is required if the input type is set to daily." & vbCrLf

        ElseIf outputType IsNot Nothing Then

            If inputType = NationalDroughtMitigationCenter.InputType.Monthly And outputType = NationalDroughtMitigationCenter.InputType.Weekly Then

                rtn &= " - can't aggregate monthly input data to weekly." & vbCrLf

            ElseIf inputType = NationalDroughtMitigationCenter.InputType.Weekly And outputType = NationalDroughtMitigationCenter.InputType.Monthly Then

                rtn &= " - can't aggregate weekly input data to monthly." & vbCrLf

            End If

        End If


        Return rtn

    End Function
#End Region

#Region "Friend Function WriteSPIData(ByVal filePath As String, ByVal aggType As AggregateType, ByVal outputType As OutputFileType) As String"
    Friend Function WriteSPIData(ByVal filePath As String, ByVal aggType As AggregateType, ByVal outputType As OutputFileType) As String

        '*** creating output file name
        If Not filePath.EndsWith("\") Then
            filePath += "\"
        End If

        filePath += _strFileName & "_SPI_" & aggType.ToString().Substring(0, 1)
        For intTimescale As Integer = 0 To Me.Count - 1
            filePath += "_" & Me.ElementAt(intTimescale).TimeScale.ToString().PadLeft(2, "0")
        Next

        If outputType = OutputFileType.Comma Then
            filePath += ".csv"
        ElseIf outputType = OutputFileType.Excel Then
            filePath += ".xlsx"
        Else
            filePath += ".txt"
        End If

        If outputType = OutputFileType.Excel Then

            Dim pa As ArrayList = New ArrayList()

            Using fs As FileStream = File.Create(filePath)
                Using document As SpreadsheetDocument = SpreadsheetDocument.Create(fs, SpreadsheetDocumentType.Workbook, True)

                    Dim wbp As WorkbookPart = document.AddWorkbookPart()
                    wbp.Workbook = New Workbook()

                    Dim wsp As WorksheetPart = wbp.AddNewPart(Of WorksheetPart)()
                    wsp.Worksheet = New Worksheet()

                    Dim sheets As Sheets = wbp.Workbook.AppendChild(New Sheets())

                    Dim s As Sheet = New Sheet()
                    s.Id = wbp.GetIdOfPart(wsp)
                    s.SheetId = 1
                    s.Name = "Sheet1"

                    sheets.Append(s)
                    wbp.Workbook.Save()

                    Dim sheetData As SheetData = wsp.Worksheet.AppendChild(New SheetData())

                    ' Constructing header
                    Dim row As Row = New Row()
                    row.Append(ConstructCell(_strFileHeader, CellValues.String))

                    ' Insert the header row to the Sheet Data
                    sheetData.AppendChild(row)

                    row = New Row()

                    pa.Add(ConstructCell("date", CellValues.String))

                    For intTimescale As Integer = 0 To Me.Count - 1

                        pa.Add(ConstructCell("spi" & Me.ElementAt(intTimescale).TimeScale, CellValues.String))

                    Next

                    row.Append(CType(pa.ToArray(GetType(Cell)), Cell()))

                    ' Insert the row to the Sheet Data
                    sheetData.AppendChild(row)

                    If Me.Count > 0 AndAlso Me.ElementAt(0).SPI IsNot Nothing AndAlso Me.ElementAt(0).SPI.Count > 0 Then

                        For intValue As Integer = 0 To Me.ElementAt(0).SPI.Count - 1

                            pa.Clear()

                            pa.Add(ConstructCell(Me.ElementAt(0).SPI(intValue).Date.ToString(_strDateFormat, SPICollection.Culture), CellValues.String))

                            row = New Row()

                            'iterating throught the spi columns
                            For intTimescale As Integer = 0 To Me.Count - 1

                                Dim spivalue As StandardPrecipitationIndexValue = Me.ElementAt(intTimescale).SPI.ElementAt(intValue)
                                pa.Add(ConstructCell(System.Math.Round(spivalue.SPI, 2).ToString(_nfiDecimalFormat), CellValues.String))

                            Next

                            row.Append(CType(pa.ToArray(GetType(Cell)), Cell()))

                            ' Insert the row to the Sheet Data
                            sheetData.AppendChild(row)

                        Next

                    Else

                        row = New Row()
                        row.Append(ConstructCell("No SPI data calculated.", CellValues.String))

                        ' Insert the header row to the Sheet Data
                        sheetData.AppendChild(row)

                    End If

                    wsp.Worksheet.Save()

                End Using
            End Using

        Else

            Using sw As StreamWriter = New StreamWriter(filePath)

                If outputType = OutputFileType.Comma Then
                    sw.WriteLine("""" & _strFileHeader & """")
                    sw.Write("""date""")
                Else
                    sw.WriteLine(_strFileHeader)
                    sw.Write("date")
                End If

                For intTimescale As Integer = 0 To Me.Count - 1

                    If outputType = OutputFileType.Comma Then
                        sw.Write(",")
                        sw.Write(" ""spi" & Me.ElementAt(intTimescale).TimeScale & """")
                    Else
                        sw.Write(" spi" & Me.ElementAt(intTimescale).TimeScale)
                    End If

                Next

                sw.WriteLine()

                If Me.Count > 0 AndAlso Me.ElementAt(0).SPI IsNot Nothing AndAlso Me.ElementAt(0).SPI.Count > 0 Then

                    ' iterating through all of the rows
                    For intValue As Integer = 0 To Me.ElementAt(0).SPI.Count - 1

                        If outputType = OutputFileType.Comma Then
                            sw.Write("""" & Me.ElementAt(0).SPI(intValue).Date.ToString(_strDateFormat, SPICollection.Culture) & """")
                        Else
                            sw.Write(Me.ElementAt(0).SPI(intValue).Date.ToString(_strDateFormat, SPICollection.Culture))
                        End If


                        'iterating throught the spi columns
                        For intTimescale As Integer = 0 To Me.Count - 1

                            Dim spivalue As StandardPrecipitationIndexValue = Me.ElementAt(intTimescale).SPI.ElementAt(intValue)

                            If outputType = OutputFileType.Comma Then
                                sw.Write(",")
                                sw.Write(" """ & System.Math.Round(spivalue.SPI, 2).ToString(_nfiDecimalFormat) & """")
                            Else
                                sw.Write(" " & System.Math.Round(spivalue.SPI, 2).ToString(_nfiDecimalFormat))
                            End If

                        Next

                        sw.WriteLine()

                    Next

                Else

                    sw.WriteLine("No SPI data calculated.")

                End If

            End Using

        End If

        Return filePath & vbCrLf

    End Function
#End Region

#Region "Friend Function WriteFrequencyData(ByVal filePath As String, ByVal aggType As AggregateType, ByVal outputType As OutputFileType) As String"
    Friend Function WriteFrequencyData(ByVal filePath As String, ByVal aggType As AggregateType, ByVal outputType As OutputFileType) As String

        Dim strPaths As String = ""

        '*** creating output file name
        If Not filePath.EndsWith("\") Then
            filePath += "\"
        End If

        For Each myspi In Me

            Dim strFullPath As String = filePath
            strFullPath += _strFileName & "_SPI_Frequency_" & myspi.TimeScale.ToString().PadLeft(2, "0")
            strFullPath += "_" & aggType.ToString().Substring(0, 1)

            If outputType = OutputFileType.Comma Then
                strFullPath += ".csv"
            ElseIf outputType = OutputFileType.Excel Then
                strFullPath += ".xlsx"
            Else
                strFullPath += ".txt"
            End If

            strPaths += strFullPath & vbCrLf

            If outputType = OutputFileType.Excel Then

                Using fs As FileStream = File.Create(strFullPath)
                    Using document As SpreadsheetDocument = SpreadsheetDocument.Create(fs, SpreadsheetDocumentType.Workbook, True)

                        Dim wbp As WorkbookPart = document.AddWorkbookPart()
                        wbp.Workbook = New Workbook()

                        Dim wsp As WorksheetPart = wbp.AddNewPart(Of WorksheetPart)()
                        wsp.Worksheet = New Worksheet()

                        Dim sheets As Sheets = wbp.Workbook.AppendChild(New Sheets())

                        Dim s As Sheet = New Sheet()
                        s.Id = wbp.GetIdOfPart(wsp)
                        s.SheetId = 1
                        s.Name = "Sheet1"

                        sheets.Append(s)
                        wbp.Workbook.Save()

                        Dim sheetData As SheetData = wsp.Worksheet.AppendChild(New SheetData())

                        ' Constructing header
                        Dim row As Row = New Row()
                        row.Append(ConstructCell(_strFileHeader, CellValues.String))

                        ' Insert the header row to the Sheet Data
                        sheetData.AppendChild(row)

                        row = New Row()

                        row.Append(
                            ConstructCell("spi", CellValues.String),
                            ConstructCell("frequency", CellValues.String),
                            ConstructCell("percentage", CellValues.String),
                            ConstructCell("rankingpercentile", CellValues.String))

                        ' Insert the row to the Sheet Data
                        sheetData.AppendChild(row)

                        If myspi.Frequencies IsNot Nothing AndAlso myspi.Frequencies.Count > 0 Then
                            For Each Val As StandardPrecipitationIndexFrequency In (From n In myspi.Frequencies Order By n.SPI Ascending Select n)
                                row = New Row()

                                row.Append(
                                    ConstructCell(Val.SPI.ToString(_nfiDecimalFormat), CellValues.String),
                                    ConstructCell(Val.Count.ToString(_nfiDecimalFormat), CellValues.String),
                                    ConstructCell(System.Math.Round(Val.Percentage, 2).ToString(_nfiDecimalFormat), CellValues.String),
                                    ConstructCell(System.Math.Round(Val.RankingPercentile, 2).ToString(_nfiDecimalFormat), CellValues.String))

                                ' Insert the row to the Sheet Data
                                sheetData.AppendChild(row)
                            Next
                        Else
                            row = New Row()
                            row.Append(
                                ConstructCell("No frequencies found.", CellValues.String))

                            ' Insert the header row to the Sheet Data
                            sheetData.AppendChild(row)
                        End If

                        wsp.Worksheet.Save()

                    End Using
                End Using

            Else

                Using sw As StreamWriter = New StreamWriter(strFullPath)

                    If outputType = OutputFileType.Comma Then
                        sw.WriteLine("""" & _strFileHeader & """")
                        sw.WriteLine("""spi"", ""frequency"", ""percentage"", ""rankingpercentile""")
                    Else
                        sw.WriteLine(_strFileHeader)
                        sw.WriteLine("spi frequency percentage rankingpercentile")
                    End If

                    If myspi.Frequencies IsNot Nothing AndAlso myspi.Frequencies.Count > 0 Then

                        For Each Val As StandardPrecipitationIndexFrequency In (From n In myspi.Frequencies Order By n.SPI Ascending Select n)

                            If outputType = OutputFileType.Comma Then
                                sw.WriteLine("""" & Val.SPI.ToString(_nfiDecimalFormat) & """, """ & Val.Count.ToString(_nfiDecimalFormat) & """, """ & System.Math.Round(Val.Percentage, 2).ToString(_nfiDecimalFormat) & """, """ & System.Math.Round(Val.RankingPercentile, 2).ToString(_nfiDecimalFormat) & """")
                            Else
                                sw.WriteLine(Val.SPI.ToString(_nfiDecimalFormat) & " " & Val.Count.ToString(_nfiDecimalFormat) & " " & System.Math.Round(Val.Percentage, 2).ToString(_nfiDecimalFormat) & " " & System.Math.Round(Val.RankingPercentile, 2).ToString(_nfiDecimalFormat))
                            End If

                        Next

                    Else

                        sw.WriteLine("No frequencies found.")

                    End If

                End Using

            End If

        Next

        Return strPaths

    End Function
#End Region

#Region "Friend Function WriteDroughtPeriodData(ByVal filePath As String, ByVal aggType As AggregateType, byval outputType As OutputFileType) As String"
    Friend Function WriteDroughtPeriodData(ByVal filePath As String, ByVal aggType As AggregateType, ByVal outputType As OutputFileType) As String

        Dim strPaths As String = ""

        '*** creating output file name
        If Not filePath.EndsWith("\") Then
            filePath += "\"
        End If

        'iterating through spi
        For Each myspi In Me

            'iterating through drought period levels
            For Each Val As KeyValuePair(Of Double, List(Of DroughtPeriod)) In (From n In myspi.DroughtPeriods Order By n.Key Ascending Select n)

                Dim strFullPath As String = filePath
                strFullPath += _strFileName & "_SPI_DroughtPeriod_" & myspi.TimeScale.ToString().PadLeft(2, "0")
                strFullPath += "_" & aggType.ToString().Substring(0, 1)
                strFullPath += "_" & Val.Key.ToString()

                If outputType = OutputFileType.Comma Then
                    strFullPath += ".csv"
                ElseIf outputType = OutputFileType.Excel Then
                    strFullPath += ".xlsx"
                Else
                    strFullPath += ".txt"
                End If

                strPaths += strFullPath & vbCrLf

                If outputType = OutputFileType.Excel Then

                    Using fs As FileStream = File.Create(strFullPath)
                        Using document As SpreadsheetDocument = SpreadsheetDocument.Create(fs, SpreadsheetDocumentType.Workbook, True)

                            Dim wbp As WorkbookPart = document.AddWorkbookPart()
                            wbp.Workbook = New Workbook()

                            Dim wsp As WorksheetPart = wbp.AddNewPart(Of WorksheetPart)()
                            wsp.Worksheet = New Worksheet()

                            Dim sheets As Sheets = wbp.Workbook.AppendChild(New Sheets())

                            Dim s As Sheet = New Sheet()
                            s.Id = wbp.GetIdOfPart(wsp)
                            s.SheetId = 1
                            s.Name = "Sheet1"

                            sheets.Append(s)
                            wbp.Workbook.Save()

                            Dim sheetData As SheetData = wsp.Worksheet.AppendChild(New SheetData())

                            ' Constructing header
                            Dim row As Row = New Row()
                            row.Append(ConstructCell(_strFileHeader, CellValues.String))

                            ' Insert the header row to the Sheet Data
                            sheetData.AppendChild(row)

                            row = New Row()

                            row.Append(
                            ConstructCell("start_date", CellValues.String),
                            ConstructCell("end_date", CellValues.String),
                            ConstructCell("duration", CellValues.String),
                            ConstructCell("peak", CellValues.String),
                            ConstructCell("sum", CellValues.String),
                            ConstructCell("average", CellValues.String),
                            ConstructCell("median", CellValues.String))

                            ' Insert the row to the Sheet Data
                            sheetData.AppendChild(row)

                            If Val.Value IsNot Nothing AndAlso Val.Value.Count > 0 Then
                                For Each dp In Val.Value
                                    row = New Row()

                                    row.Append(
                                    ConstructCell(dp.StartDate.ToString(_strDateFormat, SPICollection.Culture), CellValues.String),
                                    ConstructCell(dp.EndDate.ToString(_strDateFormat, SPICollection.Culture), CellValues.String),
                                    ConstructCell(System.Math.Round(dp.Duration, 2).ToString(_nfiDecimalFormat), CellValues.String),
                                    ConstructCell(System.Math.Round(dp.Peak, 2).ToString(_nfiDecimalFormat), CellValues.String),
                                    ConstructCell(System.Math.Round(dp.Sum, 2).ToString(_nfiDecimalFormat), CellValues.String),
                                    ConstructCell(System.Math.Round(dp.Average, 2).ToString(_nfiDecimalFormat), CellValues.String),
                                    ConstructCell(System.Math.Round(dp.Median, 2).ToString(_nfiDecimalFormat), CellValues.String))

                                    ' Insert the row to the Sheet Data
                                    sheetData.AppendChild(row)
                                Next
                            Else
                                row = New Row()
                                row.Append(
                                ConstructCell("No drought periods found.", CellValues.String))

                                ' Insert the header row to the Sheet Data
                                sheetData.AppendChild(row)
                            End If

                            wsp.Worksheet.Save()

                        End Using
                    End Using

                Else

                    Using sw As StreamWriter = New StreamWriter(strFullPath)

                        If outputType = OutputFileType.Comma Then
                            sw.WriteLine("""" & _strFileHeader & """")
                            sw.WriteLine("""start_date"", ""end_date"", ""duration"", ""peak"", ""sum"", ""average"", ""median""")
                        Else
                            sw.WriteLine(_strFileHeader)
                            sw.WriteLine("start_date end_date duration peak sum average median")
                        End If


                        If Val.Value IsNot Nothing AndAlso Val.Value.Count > 0 Then

                            For Each dp In Val.Value

                                If outputType = OutputFileType.Comma Then
                                    sw.WriteLine("""" & dp.StartDate.ToString(_strDateFormat, SPICollection.Culture) & """, """ & dp.EndDate.ToString(_strDateFormat, SPICollection.Culture) & """, """ & System.Math.Round(dp.Duration, 2).ToString(_nfiDecimalFormat) & """, """ &
                                         System.Math.Round(dp.Peak, 2).ToString(_nfiDecimalFormat) & """, """ & System.Math.Round(dp.Sum, 2).ToString(_nfiDecimalFormat) & """, """ &
                                         System.Math.Round(dp.Average, 2).ToString(_nfiDecimalFormat) & """, """ & System.Math.Round(dp.Median, 2).ToString(_nfiDecimalFormat) & """")
                                Else
                                    sw.WriteLine(dp.StartDate.ToString(_strDateFormat, SPICollection.Culture) & " " & dp.EndDate.ToString(_strDateFormat, SPICollection.Culture) & " " & System.Math.Round(dp.Duration, 2).ToString(_nfiDecimalFormat) & " " &
                                         System.Math.Round(dp.Peak, 2).ToString(_nfiDecimalFormat) & " " & System.Math.Round(dp.Sum, 2).ToString(_nfiDecimalFormat) & " " &
                                         System.Math.Round(dp.Average, 2).ToString(_nfiDecimalFormat) & " " & System.Math.Round(dp.Median, 2).ToString(_nfiDecimalFormat))
                                End If
                            Next

                        Else

                            sw.WriteLine("No drought periods found.")

                        End If

                    End Using

                End If

            Next
        Next

        Return strPaths

    End Function
#End Region




#Region "Private Sub ParseInputFileColumnDates(ByVal FileLines As String(), ByVal delimiter As String)"
    Private Sub ParseInputFileColumnDates(ByVal FileLines As String(), ByVal delimiter As String)

        '*** retreiving input data
        Dim period As DateTime

        '*** parsing precipitation and temperature from each line
        For counter As Integer = 1 To FileLines.Length - 1

            Dim aRow As String() = FileLines(counter).Split(New String() {delimiter}, StringSplitOptions.RemoveEmptyEntries)

            If Not DateTime.TryParse(aRow(0), period) Then
                Throw New Exception("unable to parse date string:  " & aRow(0))
            End If

            If aRow.Length > 1 Then
                Precipitation.Add(period, Convert.ToDouble(aRow(1)))
            End If

        Next

    End Sub
#End Region

#Region "Private Sub ParseInputFileHeaderDate(ByVal strFile As String(), ByVal delimiter As String)"
    Private Sub ParseInputFileHeaderDate(ByVal FileLines As String(), ByVal delimiter As String)


        Dim period As DateTime
        Dim intPeriodCounter As Integer = 0

        '*** parsing precipitation and temperature from each line
        For counter As Integer = 1 To FileLines.Length - 1

            If counter = 1 Then

                Dim aRow As String() = FileLines(counter).Split(New String() {delimiter}, StringSplitOptions.RemoveEmptyEntries)

                '*** start date
                period = Convert.ToDateTime(aRow(1) & "/1/" & aRow(0), SPICollection.Culture)

            Else

                Dim aRow As String() = FileLines(counter).Split(New String() {delimiter}, StringSplitOptions.RemoveEmptyEntries)
                If aRow.Length > 0 Then
                    Precipitation.Add(period, Convert.ToDouble(aRow(0)))
                End If

                ' need to add whatever input type of data is added here to get the count right. 
                Select Case _inputType

                    Case NationalDroughtMitigationCenter.InputType.Daily
                        period = period.AddDays(1)

                    Case NationalDroughtMitigationCenter.InputType.Weekly
                        intPeriodCounter += 1
                        If intPeriodCounter < 52 Then
                            period = period.AddDays(7)
                        Else
                            intPeriodCounter = 0
                            period = "1/1/" & period.AddYears(1).Year
                        End If

                    Case NationalDroughtMitigationCenter.InputType.Monthly
                        period = period.AddMonths(1)

                End Select

            End If

        Next


    End Sub
#End Region

#Region "Private Sub CompoundToWeeklyData()"
    Private Sub CompoundToWeeklyData()

        'The aggregate parameter would only take effect if the -t parameter was set to daily.
        Dim dicPrecip As Dictionary(Of DateTime, Double) = Precipitation.ToDictionary(Of DateTime, Double)(Function(g) g.Key, Function(x) x.Value)

        Precipitation.Clear()

        '*** aggregate each year's worth of data into weeks
        Dim intYears As List(Of Integer) = dicPrecip.Select(Function(x) x.Key.Year).Distinct().ToList()

        For i As Integer = 0 To intYears.Count - 1

            Dim iterationvar As Integer = i
            Dim lnqPrecipDataByYear = From n In dicPrecip Where n.Key.Year = intYears(iterationvar) Order By n.Key

            '*** data is good, aggregate by every 7 days
            Dim dblPrecip As Double = 0
            Dim intDays As Integer = 0

            Dim j As Integer = 0

            While j < lnqPrecipDataByYear.Count() - 1

                '*** if there are more than 7 but less than 14 days left in the year
                If lnqPrecipDataByYear.Count() - 1 - j >= 14 Then
                    intDays = 7
                Else
                    intDays = lnqPrecipDataByYear.Count() - j
                End If


                For k As Integer = 0 To intDays - 1

                    dblPrecip += lnqPrecipDataByYear(j + k).Value

                Next

                Precipitation.Add(lnqPrecipDataByYear(j).Key, dblPrecip)

                j += intDays

                intDays = 0
                dblPrecip = 0

            End While

        Next

    End Sub
#End Region

#Region "Private Sub CompoundToMonthlyData()"
    Private Sub CompoundToMonthlyData()

        'The aggregate parameter would only take effect if the -t parameter was set to daily.
        Dim dicPrecip As Dictionary(Of DateTime, Double) = Precipitation.ToDictionary(Of DateTime, Double)(Function(g) g.Key, Function(x) x.Value)
        Precipitation.Clear()

        '*** aggregating data into months
        Precipitation = (From p In dicPrecip Group By mydate = Convert.ToDateTime(p.Key.Month & "/1/" & p.Key.Year, SPICollection.Culture)
            Into TotalData = Sum(p.Value)
            Select mydate, TotalData).ToDictionary(Function(g) g.mydate, Function(h) h.TotalData)

    End Sub
#End Region

#Region "Private Function ConstructCell(ByVal cellValue As String, ByVal dataType As CellValues) As Cell"
    Private Function ConstructCell(ByVal cellValue As String, ByVal dataType As CellValues) As Cell
        Dim tmpCell As Cell = New Cell()
        tmpCell.CellValue = New CellValue(cellValue)
        tmpCell.DataType = New EnumValue(Of CellValues)(dataType)

        Return tmpCell
    End Function
#End Region

End Class
