Imports System.Text
Imports System.ComponentModel
Imports System.IO

Public Class frmProgressbar




    Public ErrorMessage As StringBuilder
    Public OutputMessage As String

    Private WithEvents _bw As BackgroundWorker


#Region "Public Sub New(ByVal inputs As SPIArguments, ByVal spiCount As Integer)"
    Public Sub New(ByVal inputs As SPIArguments, ByVal spiCount As Integer)

        InitializeComponent()

        Me.ProgressBar1.Maximum = spiCount

        ErrorMessage = New StringBuilder()

        _bw = New BackgroundWorker()
        _bw.WorkerReportsProgress = True
        _bw.WorkerSupportsCancellation = True
        _bw.RunWorkerAsync(inputs)

    End Sub
#End Region





#Region "Private Sub backgroundWorker_ProgressChanged(ByVal sender As Object, ByVal e As ProgressChangedEventArgs) Handles _bw.ProgressChanged"
    Private Sub backgroundWorker_ProgressChanged(ByVal sender As Object, ByVal e As ProgressChangedEventArgs) Handles _bw.ProgressChanged

        Me.ProgressBar1.Increment(1)

    End Sub
#End Region

#Region "Private Sub backgroundWorker1_RunWorkerCompleted(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs) Handles _bw.RunWorkerCompleted"
    Private Sub backgroundWorker1_RunWorkerCompleted(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs) Handles _bw.RunWorkerCompleted

        OutputMessage = e.Result

        Me.Close()

    End Sub
#End Region

#Region "Private Sub ComputeSPI(ByVal sender As Object, ByVal e As DoWorkEventArgs) Handles _bw.DoWork"
    Private Sub ComputeSPI(ByVal sender As Object, ByVal e As DoWorkEventArgs) Handles _bw.DoWork

        Dim inputs As SPIArguments = e.Argument

        If (inputs.locationType = FileLocation.File) Then

            e.Result = ComputeFile(inputs)

        ElseIf inputs.locationType = FileLocation.Directory Then

            e.Result = ComputeDirectory(inputs)

        End If

    End Sub
#End Region

#Region "Private Function ComputeFile(ByVal inputs As SPIArguments) As String"
    Private Function ComputeFile(ByVal inputs As SPIArguments) As String


        If _bw.CancellationPending Then
            Return inputs.strOutputLocation
        End If


        Dim strOutputFiles As String = ""

        Dim spicol As SPICollection = New SPICollection(inputs.InternationalNumberFormat, inputs.InternationalDateFormat, inputs.inputType, inputs.aggType)

        '*** get data from the file and aggregate if necessary
        Try

            spicol.ParseInputFileData(inputs.strInputLocation, inputs.delimiter)

        Catch ex As Exception

            ErrorMessage.Append(ExceptionManager.ExtractExceptionInfo(ex, inputs.strInputLocation))
            _bw.ReportProgress(1)
            Return inputs.strOutputLocation

        End Try


        For Each intTimeScale In inputs.lstTimeScales

            If _bw.CancellationPending Then
                Return inputs.strOutputLocation
            End If

            Dim spi As SPI = New SPI(intTimeScale)

            Try

                '*** retreiving data and calculating spi
                Select Case inputs.aggType

                    Case NationalDroughtMitigationCenter.AggregateType.Month
                        spi.CalculateByMonth(spicol.Precipitation)

                    Case NationalDroughtMitigationCenter.AggregateType.Week
                        spi.CalculateByWeek(spicol.Precipitation)

                End Select

                '*** calculating frequencies
                If inputs.CalculateFrequencies Then
                    spi.CalculateFrequencies()
                End If

                '*** calculating drought periods
                If inputs.CalculateDroughtPeriods Then
                    spi.CalculateDroughtPeriods()
                End If

                spicol.Add(spi)

            Catch ex As Exception

                ErrorMessage.Append(ExceptionManager.ExtractExceptionInfo(ex, inputs.strInputLocation))

            Finally

                _bw.ReportProgress(1)

            End Try

        Next

        If _bw.CancellationPending Then
            Return inputs.strOutputLocation
        End If

        Try

            '*** writing data to file system
            strOutputFiles += spicol.WriteSPIData(inputs.strOutputLocation, inputs.aggType, inputs.outputType)

            '*** calculating frequencies
            If inputs.CalculateFrequencies Then
                strOutputFiles += spicol.WriteFrequencyData(inputs.strOutputLocation, inputs.aggType, inputs.outputType)
            End If

            '*** calculating drought periods
            If inputs.CalculateDroughtPeriods Then
                strOutputFiles += spicol.WriteDroughtPeriodData(inputs.strOutputLocation, inputs.aggType, inputs.outputType)
            End If

        Catch ex As Exception

            ErrorMessage.Append(ExceptionManager.ExtractExceptionInfo(ex, inputs.strInputLocation))
            Return inputs.strOutputLocation

        End Try

        Return strOutputFiles

    End Function
#End Region

#Region "Private Function ComputeDirectory(ByVal inputs As SPIArguments) As String"
    Private Function ComputeDirectory(ByVal inputs As SPIArguments) As String

        Dim strInputLocation As String = inputs.strInputLocation


        For Each fi As IO.FileInfo In (New DirectoryInfo(strInputLocation)).GetFiles()
            If _bw.CancellationPending Then

                Exit For

            ElseIf fi.Extension = ".txt" Or fi.Extension = ".cor" Or fi.Extension = ".csv" Or fi.Extension = ".dat" Then

                inputs.strInputLocation = fi.FullName
                ComputeFile(inputs)

            End If
        Next

        Return inputs.strOutputLocation

    End Function
#End Region

#Region "Private Sub btnCancel_Click(sender As System.Object, e As System.EventArgs) Handles btnCancel.Click"
    Private Sub btnCancel_Click(sender As System.Object, e As System.EventArgs) Handles btnCancel.Click

        _bw.CancelAsync()

    End Sub
#End Region

End Class




