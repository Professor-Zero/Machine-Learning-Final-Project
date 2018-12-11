Imports System.IO
Imports System.Text


Public Class frmMain


    Public _strError As StringBuilder

#Region "Public Sub New()"
    Public Sub New()

        InitializeComponent()

        Me.Text = Application.ProductName

        cmbInputDataType.DataSource = [Enum].GetValues(GetType(InputType))
        cmbDataDelimiter.DataSource = [Enum].GetValues(GetType(Delimiter))
        chkList.DataSource = {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 18, 24, 36, 48, 60, 72, 84, 96}

#If Not DEBUG Then
        Me.Text &= "  v " + Application.ProductVersion.Remove(Application.ProductVersion.Length - 2)
#Else

        'txtInput.Text = "C:\temp\input_dir"
        'txtInputFile.Text = "C:\ndmc-unl\droughtriskatlas\code\spi_dll_c#\docs\sampleinput\commadelimited_monthly.csv"
        'txtInputDirectory.Text = "C:\Users\scott.NDMC\Desktop\comma"
        'txtOutput.Text = "C:\temp"
        'rdoInputDirectory.Checked = True

#End If

    End Sub
#End Region




#Region "Private Sub btnGenerate_Click(sender As Object, e As EventArgs) Handles btnGenerate.Click"
    Private Sub btnGenerate_Click(sender As Object, e As EventArgs) Handles btnGenerate.Click


        _strError = New StringBuilder()

        Dim inputs As SPIArguments = New SPIArguments()

        inputs.locationType = IIf(rdoInputDirectory.Checked, FileLocation.Directory, FileLocation.File)
        inputs.strInputLocation = IIf(rdoInputDirectory.Checked, txtInputDirectory.Text.Trim(), txtInputFile.Text.Trim())
        inputs.strOutputLocation = txtOutput.Text.Trim()
        inputs.lstTimeScales = chkList.CheckedItems.Cast(Of Integer)().ToList()
        inputs.aggType = CType(cmbAggregateType.SelectedItem, AggregateType)
        inputs.inputType = CType(cmbInputDataType.SelectedValue, InputType)
        inputs.delimiter = CType(cmbDataDelimiter.SelectedItem, Delimiter)
        inputs.InternationalNumberFormat = chkInternationalNumber.Checked
        inputs.InternationalDateFormat = chkInternationalDate.Checked
        inputs.CalculateFrequencies = chkFrequencies.Checked
        inputs.CalculateDroughtPeriods = chkDroughtPeriods.Checked

        If rdoSpace.Checked Then
            inputs.outputType = OutputFileType.Space
        ElseIf rdoComma.Checked Then
            inputs.outputType = OutputFileType.Comma
        ElseIf rdoExcel.Checked Then
            inputs.outputType = OutputFileType.Excel
        End If

        '*** check inputs
        _strError.Append(SPICollection.CheckInputData(inputs.strInputLocation, inputs.strOutputLocation, inputs.locationType, inputs.lstTimeScales, inputs.inputType, inputs.aggType))

        If _strError.Length > 0 Then

            MessageBox.Show("Please correct the following before continuing:" & vbCrLf & vbCrLf & _strError.ToString(), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error)

        Else

            Dim intFileCount As Integer = 0

            Select Case inputs.locationType

                Case FileLocation.File
                    intFileCount = 1

                Case FileLocation.Directory

                    For Each fi As FileInfo In (New DirectoryInfo(inputs.strInputLocation)).GetFiles()
                        If fi.Extension = ".txt" Or fi.Extension = ".cor" Or fi.Extension = ".csv" Or fi.Extension = ".dat" Then

                            intFileCount += 1

                        End If
                    Next

            End Select

            Dim frmProgress As frmProgressbar
            frmProgress = New frmProgressbar(inputs, intFileCount * inputs.lstTimeScales.Count)
            frmProgress.ShowDialog(Me)

            Dim strErrorPath As String = ""
            Dim strMsg As String = ""
            Dim strInputLocation As String = IIf(rdoInputDirectory.Checked, txtInputDirectory.Text.Trim(), txtInputFile.Text.Trim())
            Dim strOutputLocation As String = txtOutput.Text.Trim()


            '*** writing any errors encountered
            If (frmProgress.ErrorMessage.Length > 0) Then
                strErrorPath = ExceptionManager.WriteErrorData(txtOutput.Text.Trim(), frmProgress.ErrorMessage.ToString())
            End If

            strMsg = "Parsed:" + vbCrLf + strInputLocation + vbCrLf + vbCrLf + "Saved To:" + vbCrLf + frmProgress.OutputMessage

            If frmProgress.ErrorMessage.Length > 0 Then
                strMsg += vbCrLf + vbCrLf + "Errors were encountered and logged to:" + vbCrLf + strErrorPath
            End If

            'MessageBox.Show(strMsg, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information)

            Dim frmResults As frmResults = New frmResults()
            frmResults.Message = strMsg
            frmResults.ShowDialog(Me)

            frmProgress.Dispose()

        End If


    End Sub
#End Region

#Region "Private Sub btnInputFile_Click(sender As Object, e As EventArgs) Handles btnInputDirectory.Click, btnOutput.Click, btnInputFile.Click"
    Private Sub btnInputFile_Click(sender As Object, e As EventArgs) Handles btnInputDirectory.Click, btnOutput.Click, btnInputFile.Click

        If sender.Equals(btnInputFile) AndAlso rdoInputFile.Checked Then

            If (dlgFile.ShowDialog() = DialogResult.OK) Then
                txtInputFile.Text = dlgFile.FileName
            End If

        ElseIf sender.Equals(btnInputDirectory) AndAlso rdoInputDirectory.Checked Then

            If (dlgFolder.ShowDialog() = DialogResult.OK) Then
                txtInputDirectory.Text = dlgFolder.SelectedPath
            End If

        ElseIf sender.Equals(btnOutput) Then

            If (dlgFolder.ShowDialog() = DialogResult.OK) Then
                txtOutput.Text = dlgFolder.SelectedPath
            End If

        End If

    End Sub
#End Region

#Region "Private Sub rdoInput_CheckedChanged(sender As Object, e As EventArgs) Handles rdoInputFile.CheckedChanged"
    Private Sub rdoInput_CheckedChanged(sender As Object, e As EventArgs) Handles rdoInputFile.CheckedChanged


        Dim bold As Font = New Font(rdoInputFile.Font, rdoInputFile.Font.Style Or FontStyle.Bold)
        Dim notbold As Font = New Font(rdoInputFile.Font, rdoInputFile.Font.Style And Not FontStyle.Bold)

        'setting radio button font to bold if selected
        If rdoInputFile.Checked Then

            rdoInputFile.Font = bold
            rdoInputDirectory.Font = notbold

            txtInputFile.Enabled = True
            btnInputFile.Enabled = True

            txtInputDirectory.Enabled = False
            btnInputDirectory.Enabled = False

        Else

            rdoInputFile.Font = notbold
            rdoInputDirectory.Font = bold

            txtInputFile.Enabled = False
            btnInputFile.Enabled = False

            txtInputDirectory.Enabled = True
            btnInputDirectory.Enabled = True

        End If


    End Sub
#End Region

#Region "Private Sub cmbInputDataChanged(sender As Object, e As EventArgs) Handles cmbInputDataType.SelectedIndexChanged"
    Private Sub cmbInputDataChanged(sender As Object, e As EventArgs) Handles cmbInputDataType.SelectedIndexChanged

        cmbAggregateType.Items.Clear()

        '*** setting aggregate options for the selected input data type
        Select Case CType(cmbInputDataType.SelectedValue, InputType)

            Case InputType.Daily
                cmbAggregateType.Items.Add(AggregateType.Week)
                cmbAggregateType.Items.Add(AggregateType.Month)
                cmbAggregateType.SelectedItem = AggregateType.Month

            Case InputType.Weekly
                cmbAggregateType.Items.Add(AggregateType.Week)
                cmbAggregateType.Items.Add(AggregateType.Month)
                cmbAggregateType.SelectedItem = AggregateType.Week

            Case InputType.Monthly
                cmbAggregateType.Items.Add(AggregateType.Month)
                cmbAggregateType.SelectedItem = AggregateType.Month

        End Select

    End Sub
#End Region


End Class
