Imports System.IO
Imports System.Text




Friend Class AppMgr



    <STAThread()> _
    Shared Sub Main(ByVal ags() As String)

        AddHandler Application.ThreadException, AddressOf ExceptionManager.HandleApplicationException

        Dim args As List(Of String) = CommandLineArgs()

        '*** start the program as console application or windows gui based on command line arguments
        If args.Count = 0 Then

            Application.EnableVisualStyles()
            Application.SetCompatibleTextRenderingDefault(False)

            Dim frm As frmMain = New frmMain()
            Application.Run(frm)

        ElseIf args.Count >= 1 Then

            Dim strError As StringBuilder = New StringBuilder()
            Dim strTimescale As String = ""
            Dim strOutputDirectory As String = ""
            Dim strInputPath As String = ""
            Dim inputType As InputType = Nothing
            Dim aggType As AggregateType? = Nothing
            Dim locationType As FileLocation = Nothing
            Dim CalculateFrequencies As Boolean = False
            Dim CalculateDroughtPeriods As Boolean = False
            Dim delimiter As Delimiter? = Nothing
            Dim InternationalDecimalFormat As Boolean = False
            Dim InternationalDateFormat As Boolean = False
            Dim outputType As OutputFileType = OutputFileType.Comma

            '*** retreiving file arguments
            For intCount As Integer = 0 To args.Count - 1

                Select Case args(intCount).ToLower

                    Case "-i"
                        ' input file
                        If (intCount + 1) > args.Count - 1 Then
                            strError.AppendLine(" - arguments are formatted incorrectly.")
                            Exit For
                        ElseIf locationType <> Nothing Then
                            strError.AppendLine(" - a directory or file may be selected for input, not both.")
                            Exit For
                        End If
                        strInputPath = args(intCount + 1)
                        locationType = FileLocation.File

                    Case "-d"
                        ' input directory
                        If (intCount + 1) > args.Count - 1 Then
                            strError.AppendLine(" - arguments are formatted incorrectly.")
                            Exit For
                        ElseIf locationType <> Nothing Then
                            strError.AppendLine(" - a directory or file may be selected for input, not both.")
                            Exit For
                        End If
                        strInputPath = args(intCount + 1)
                        locationType = FileLocation.Directory

                    Case "-o"
                        ' output directory
                        If (intCount + 1) > args.Count - 1 Then
                            strError.AppendLine(" - arguments are formatted incorrectly.")
                            Exit For
                        End If
                        strOutputDirectory = args(intCount + 1)

                    Case "-s"
                        ' scale
                        If (intCount + 1) > args.Count - 1 Then
                            strError.AppendLine(" - arguments are formatted incorrectly.")
                            Exit For
                        End If
                        strTimescale = args(intCount + 1)

                    Case "-t"
                        ' type
                        If (intCount + 1) > args.Count - 1 Then
                            strError.AppendLine(" - arguments are formatted incorrectly.")
                            Exit For
                        End If

                        Select Case args(intCount + 1).Substring(0, 1).ToLower()

                            Case "m"
                                inputType = inputType.Monthly

                            Case "w"
                                inputType = inputType.Weekly

                            Case "d"
                                inputType = inputType.Daily

                        End Select

                    Case "-a"
                        ' aggregate
                        If (intCount + 1) > args.Count - 1 Then
                            strError.AppendLine(" - arguments are formatted incorrectly.")
                            Exit For
                        End If
                        Select Case args(intCount + 1).Substring(0, 1).ToLower()

                            Case "m"
                                aggType = AggregateType.Month

                            Case "w"
                                aggType = AggregateType.Week

                        End Select

                    Case "-f"
                        'frequencies
                        CalculateFrequencies = True

                    Case "-p"
                        ' drought periods
                        CalculateDroughtPeriods = True

                    Case "-l"
                        If (intCount + 1) > args.Count - 1 Then
                            strError.AppendLine(" - arguments are formatted incorrectly.")
                            Exit For
                        End If
                        ' data delimiter
                        Select Case args(intCount + 1).Substring(0, 1).ToLower()

                            Case "c"
                                delimiter = NationalDroughtMitigationCenter.Delimiter.Comma

                            Case "s"
                                delimiter = NationalDroughtMitigationCenter.Delimiter.Space

                            Case "t"
                                delimiter = NationalDroughtMitigationCenter.Delimiter.Tab

                        End Select

                    Case "-e"
                        InternationalDateFormat = True

                    Case "-b"
                        InternationalDecimalFormat = True

                    Case "-r"
                        ' data delimiter
                        Select Case args(intCount + 1).Substring(0, 1).ToLower()

                            Case "c"
                                outputType = OutputFileType.Comma

                            Case "s"
                                outputType = OutputFileType.Space

                            Case "e"
                                outputType = OutputFileType.Excel

                        End Select

                End Select

            Next


            Dim lstTimeScales As List(Of Integer) = strTimescale.Split(",").Select(Function(x As String) Integer.Parse(x)).ToList()

            '*** determine correct aggregate type based on input selection
            If IsNothing(aggType) Then

                Select Case inputType

                    Case InputType.Weekly
                        aggType = AggregateType.Week

                    Case InputType.Monthly
                        aggType = AggregateType.Month

                End Select

            End If


            '*** check generic input data
            strError.Append(SPICollection.CheckInputData(strInputPath, strOutputDirectory, locationType, lstTimeScales, inputType, aggType))

            '*** check delimiter
            If IsNothing(delimiter) Then
                strError.AppendLine(" - the type of delimiter used to separate data within the input file is required.")
            End If


            '*** Process SPI
            If strError.Length <= 0 Then

                '*** creating a list of files to parse
                Dim files As List(Of String) = New List(Of String)()
                If locationType = NationalDroughtMitigationCenter.FileLocation.File Then
                    files.Add(strInputPath)
                Else
                    For Each fi As FileInfo In (New DirectoryInfo(strInputPath)).GetFiles()
                        If fi.Extension = ".txt" Or fi.Extension = ".cor" Or fi.Extension = ".csv" Or fi.Extension = ".dat" Then
                            files.Add(fi.FullName)
                        End If
                    Next
                End If


                For Each strFile As String In files


                    Dim spicol As SPICollection = New SPICollection(InternationalDecimalFormat, InternationalDateFormat, inputType, aggType)

                    Try

                        '*** get data from the file and aggregate if necessary
                        spicol.ParseInputFileData(strFile, delimiter)

                    Catch ex As Exception

                        strError.AppendLine(ExceptionManager.ExtractExceptionInfo(ex, strFile))
                        Continue For

                    End Try


                    For Each intTimeScale In lstTimeScales

                        Dim spi As SPI = New SPI(intTimeScale)

                        Try

                            '*** retreiving data and calculating spi
                            Select Case aggType

                                Case AggregateType.Month
                                    spi.CalculateByMonth(spicol.Precipitation)

                                Case AggregateType.Week
                                    spi.CalculateByWeek(spicol.Precipitation)

                            End Select

                            '*** calculating frequencies
                            If CalculateFrequencies Then

                                spi.CalculateFrequencies()

                            End If

                            '*** calculating drought periods
                            If CalculateDroughtPeriods Then

                                spi.CalculateDroughtPeriods()

                            End If

                            spicol.Add(spi)

                        Catch ex As Exception

                            strError.AppendLine(ExceptionManager.ExtractExceptionInfo(ex, strFile))
                            Continue For

                        End Try

                    Next

                    '*** writing data to file system
                    spicol.WriteSPIData(strOutputDirectory, aggType, outputType)

                    '*** calculating frequencies
                    If CalculateFrequencies Then
                        spicol.WriteFrequencyData(strOutputDirectory, aggType, outputType)
                    End If

                    '*** calculating drought periods
                    If CalculateDroughtPeriods Then
                        spicol.WriteDroughtPeriodData(strOutputDirectory, aggType, outputType)
                    End If

                Next

            End If


            '*** logging errors with input parameters
            If strError.Length > 0 Then

                ExceptionManager.WriteErrorData(strOutputDirectory, strError.ToString())

            End If


        End If


    End Sub



#Region "Public Shared Function CommandLineArgs() As List(Of String)"
    Public Shared Function CommandLineArgs() As List(Of String)

        Dim bInsideQuote As Boolean
        Dim CmdArgs As New List(Of String)
        Dim iCnt As Integer
        Dim FirstArg As Boolean = True


        Dim value As String = Environment.CommandLine

        If value = "" Then
            Return New List(Of String)()
        End If


        For Each c As Char In value.ToCharArray

            iCnt += 1
            If c = """"c Then
                bInsideQuote = Not bInsideQuote
                Continue For
            End If

            If c = " " Then
                If bInsideQuote Then
                    Continue For
                Else
                    Mid$(value, iCnt, 1) = "§"
                End If
            End If

        Next

        'Replace multiple separators
        While value.IndexOf("§§") > 0
            value = value.Replace("§§", "§")
        End While

        For Each sArg As String In value.Split("§".ToCharArray, StringSplitOptions.RemoveEmptyEntries)

            If FirstArg Then
                FirstArg = False
                Continue For
            End If

            CmdArgs.Add(sArg.Replace("""", ""))

        Next

        Return CmdArgs

    End Function
#End Region

End Class

