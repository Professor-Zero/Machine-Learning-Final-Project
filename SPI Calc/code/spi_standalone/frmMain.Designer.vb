<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMain
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.grpInput = New System.Windows.Forms.GroupBox()
        Me.cmbDataDelimiter = New System.Windows.Forms.ComboBox()
        Me.lblDataDelimiter = New System.Windows.Forms.Label()
        Me.btnInputFile = New System.Windows.Forms.Button()
        Me.txtInputFile = New System.Windows.Forms.TextBox()
        Me.cmbInputDataType = New System.Windows.Forms.ComboBox()
        Me.lblInputType = New System.Windows.Forms.Label()
        Me.rdoInputFile = New System.Windows.Forms.RadioButton()
        Me.rdoInputDirectory = New System.Windows.Forms.RadioButton()
        Me.btnInputDirectory = New System.Windows.Forms.Button()
        Me.txtInputDirectory = New System.Windows.Forms.TextBox()
        Me.lblTitle = New System.Windows.Forms.Label()
        Me.grpOutput = New System.Windows.Forms.GroupBox()
        Me.grpOutputOptions = New System.Windows.Forms.GroupBox()
        Me.rdoExcel = New System.Windows.Forms.RadioButton()
        Me.rdoComma = New System.Windows.Forms.RadioButton()
        Me.rdoSpace = New System.Windows.Forms.RadioButton()
        Me.chkInternationalDate = New System.Windows.Forms.CheckBox()
        Me.chkInternationalNumber = New System.Windows.Forms.CheckBox()
        Me.chkList = New System.Windows.Forms.CheckedListBox()
        Me.lblOutputDir = New System.Windows.Forms.Label()
        Me.btnOutput = New System.Windows.Forms.Button()
        Me.chkFrequencies = New System.Windows.Forms.CheckBox()
        Me.cmbAggregateType = New System.Windows.Forms.ComboBox()
        Me.lblAggregateType = New System.Windows.Forms.Label()
        Me.txtOutput = New System.Windows.Forms.TextBox()
        Me.lblTimeScale = New System.Windows.Forms.Label()
        Me.chkDroughtPeriods = New System.Windows.Forms.CheckBox()
        Me.dlgFile = New System.Windows.Forms.OpenFileDialog()
        Me.dlgFolder = New System.Windows.Forms.FolderBrowserDialog()
        Me.btnGenerate = New System.Windows.Forms.Button()
        Me.BackgroundWorker1 = New System.ComponentModel.BackgroundWorker()
        Me.grpInput.SuspendLayout()
        Me.grpOutput.SuspendLayout()
        Me.grpOutputOptions.SuspendLayout()
        Me.SuspendLayout()
        '
        'grpInput
        '
        Me.grpInput.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.grpInput.Controls.Add(Me.cmbDataDelimiter)
        Me.grpInput.Controls.Add(Me.lblDataDelimiter)
        Me.grpInput.Controls.Add(Me.btnInputFile)
        Me.grpInput.Controls.Add(Me.txtInputFile)
        Me.grpInput.Controls.Add(Me.cmbInputDataType)
        Me.grpInput.Controls.Add(Me.lblInputType)
        Me.grpInput.Controls.Add(Me.rdoInputFile)
        Me.grpInput.Controls.Add(Me.rdoInputDirectory)
        Me.grpInput.Controls.Add(Me.btnInputDirectory)
        Me.grpInput.Controls.Add(Me.txtInputDirectory)
        Me.grpInput.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grpInput.Location = New System.Drawing.Point(3, 69)
        Me.grpInput.Name = "grpInput"
        Me.grpInput.Size = New System.Drawing.Size(601, 112)
        Me.grpInput.TabIndex = 0
        Me.grpInput.TabStop = False
        Me.grpInput.Text = "Input Options:"
        '
        'cmbDataDelimiter
        '
        Me.cmbDataDelimiter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbDataDelimiter.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmbDataDelimiter.FormattingEnabled = True
        Me.cmbDataDelimiter.Items.AddRange(New Object() {"comma", "space", "tab"})
        Me.cmbDataDelimiter.Location = New System.Drawing.Point(136, 37)
        Me.cmbDataDelimiter.Name = "cmbDataDelimiter"
        Me.cmbDataDelimiter.Size = New System.Drawing.Size(104, 21)
        Me.cmbDataDelimiter.TabIndex = 8
        '
        'lblDataDelimiter
        '
        Me.lblDataDelimiter.AutoSize = True
        Me.lblDataDelimiter.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblDataDelimiter.Location = New System.Drawing.Point(46, 41)
        Me.lblDataDelimiter.Name = "lblDataDelimiter"
        Me.lblDataDelimiter.Size = New System.Drawing.Size(76, 13)
        Me.lblDataDelimiter.TabIndex = 9
        Me.lblDataDelimiter.Text = "Data Delimiter:"
        '
        'btnInputFile
        '
        Me.btnInputFile.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnInputFile.Location = New System.Drawing.Point(574, 63)
        Me.btnInputFile.Name = "btnInputFile"
        Me.btnInputFile.Size = New System.Drawing.Size(25, 20)
        Me.btnInputFile.TabIndex = 7
        Me.btnInputFile.Text = "..."
        Me.btnInputFile.UseVisualStyleBackColor = True
        '
        'txtInputFile
        '
        Me.txtInputFile.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtInputFile.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtInputFile.Location = New System.Drawing.Point(136, 63)
        Me.txtInputFile.Name = "txtInputFile"
        Me.txtInputFile.Size = New System.Drawing.Size(437, 20)
        Me.txtInputFile.TabIndex = 6
        '
        'cmbInputDataType
        '
        Me.cmbInputDataType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbInputDataType.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmbInputDataType.FormattingEnabled = True
        Me.cmbInputDataType.Location = New System.Drawing.Point(136, 13)
        Me.cmbInputDataType.Name = "cmbInputDataType"
        Me.cmbInputDataType.Size = New System.Drawing.Size(104, 21)
        Me.cmbInputDataType.TabIndex = 0
        '
        'lblInputType
        '
        Me.lblInputType.AutoSize = True
        Me.lblInputType.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblInputType.Location = New System.Drawing.Point(46, 17)
        Me.lblInputType.Name = "lblInputType"
        Me.lblInputType.Size = New System.Drawing.Size(60, 13)
        Me.lblInputType.TabIndex = 4
        Me.lblInputType.Text = "Data Type:"
        '
        'rdoInputFile
        '
        Me.rdoInputFile.AutoSize = True
        Me.rdoInputFile.Checked = True
        Me.rdoInputFile.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.rdoInputFile.Location = New System.Drawing.Point(46, 65)
        Me.rdoInputFile.Name = "rdoInputFile"
        Me.rdoInputFile.Size = New System.Drawing.Size(45, 17)
        Me.rdoInputFile.TabIndex = 0
        Me.rdoInputFile.TabStop = True
        Me.rdoInputFile.Text = "File"
        Me.rdoInputFile.UseVisualStyleBackColor = True
        '
        'rdoInputDirectory
        '
        Me.rdoInputDirectory.AutoSize = True
        Me.rdoInputDirectory.Location = New System.Drawing.Point(46, 86)
        Me.rdoInputDirectory.Name = "rdoInputDirectory"
        Me.rdoInputDirectory.Size = New System.Drawing.Size(67, 17)
        Me.rdoInputDirectory.TabIndex = 2
        Me.rdoInputDirectory.Text = "Directory"
        Me.rdoInputDirectory.UseVisualStyleBackColor = True
        '
        'btnInputDirectory
        '
        Me.btnInputDirectory.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnInputDirectory.Enabled = False
        Me.btnInputDirectory.Location = New System.Drawing.Point(574, 84)
        Me.btnInputDirectory.Name = "btnInputDirectory"
        Me.btnInputDirectory.Size = New System.Drawing.Size(25, 20)
        Me.btnInputDirectory.TabIndex = 2
        Me.btnInputDirectory.Text = "..."
        Me.btnInputDirectory.UseVisualStyleBackColor = True
        '
        'txtInputDirectory
        '
        Me.txtInputDirectory.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtInputDirectory.Enabled = False
        Me.txtInputDirectory.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtInputDirectory.Location = New System.Drawing.Point(136, 84)
        Me.txtInputDirectory.Name = "txtInputDirectory"
        Me.txtInputDirectory.Size = New System.Drawing.Size(437, 20)
        Me.txtInputDirectory.TabIndex = 1
        '
        'lblTitle
        '
        Me.lblTitle.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.lblTitle.AutoSize = True
        Me.lblTitle.Font = New System.Drawing.Font("Microsoft Sans Serif", 25.0!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTitle.ForeColor = System.Drawing.SystemColors.AppWorkspace
        Me.lblTitle.Location = New System.Drawing.Point(1, 10)
        Me.lblTitle.MinimumSize = New System.Drawing.Size(558, 39)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(606, 39)
        Me.lblTitle.TabIndex = 1
        Me.lblTitle.Text = "Standard Precipitation Index Generator"
        '
        'grpOutput
        '
        Me.grpOutput.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.grpOutput.Controls.Add(Me.grpOutputOptions)
        Me.grpOutput.Controls.Add(Me.chkInternationalDate)
        Me.grpOutput.Controls.Add(Me.chkInternationalNumber)
        Me.grpOutput.Controls.Add(Me.chkList)
        Me.grpOutput.Controls.Add(Me.lblOutputDir)
        Me.grpOutput.Controls.Add(Me.btnOutput)
        Me.grpOutput.Controls.Add(Me.chkFrequencies)
        Me.grpOutput.Controls.Add(Me.cmbAggregateType)
        Me.grpOutput.Controls.Add(Me.lblAggregateType)
        Me.grpOutput.Controls.Add(Me.txtOutput)
        Me.grpOutput.Controls.Add(Me.lblTimeScale)
        Me.grpOutput.Controls.Add(Me.chkDroughtPeriods)
        Me.grpOutput.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grpOutput.Location = New System.Drawing.Point(2, 185)
        Me.grpOutput.Name = "grpOutput"
        Me.grpOutput.Size = New System.Drawing.Size(602, 213)
        Me.grpOutput.TabIndex = 2
        Me.grpOutput.TabStop = False
        Me.grpOutput.Text = "Output Options:"
        '
        'grpOutputOptions
        '
        Me.grpOutputOptions.Controls.Add(Me.rdoExcel)
        Me.grpOutputOptions.Controls.Add(Me.rdoComma)
        Me.grpOutputOptions.Controls.Add(Me.rdoSpace)
        Me.grpOutputOptions.Location = New System.Drawing.Point(255, 98)
        Me.grpOutputOptions.Name = "grpOutputOptions"
        Me.grpOutputOptions.Size = New System.Drawing.Size(149, 85)
        Me.grpOutputOptions.TabIndex = 21
        Me.grpOutputOptions.TabStop = False
        Me.grpOutputOptions.Text = "Output As"
        '
        'rdoExcel
        '
        Me.rdoExcel.AutoSize = True
        Me.rdoExcel.Location = New System.Drawing.Point(17, 63)
        Me.rdoExcel.Name = "rdoExcel"
        Me.rdoExcel.Size = New System.Drawing.Size(87, 17)
        Me.rdoExcel.TabIndex = 22
        Me.rdoExcel.Text = "Excel (XSLX)"
        Me.rdoExcel.UseVisualStyleBackColor = True
        '
        'rdoComma
        '
        Me.rdoComma.AutoSize = True
        Me.rdoComma.Location = New System.Drawing.Point(17, 41)
        Me.rdoComma.Name = "rdoComma"
        Me.rdoComma.Size = New System.Drawing.Size(106, 17)
        Me.rdoComma.TabIndex = 21
        Me.rdoComma.Text = "Comma Delimited"
        Me.rdoComma.UseVisualStyleBackColor = True
        '
        'rdoSpace
        '
        Me.rdoSpace.AutoSize = True
        Me.rdoSpace.Checked = True
        Me.rdoSpace.Location = New System.Drawing.Point(17, 18)
        Me.rdoSpace.Name = "rdoSpace"
        Me.rdoSpace.Size = New System.Drawing.Size(102, 17)
        Me.rdoSpace.TabIndex = 20
        Me.rdoSpace.TabStop = True
        Me.rdoSpace.Text = "Space Delimited"
        Me.rdoSpace.UseVisualStyleBackColor = True
        '
        'chkInternationalDate
        '
        Me.chkInternationalDate.AutoSize = True
        Me.chkInternationalDate.Location = New System.Drawing.Point(255, 38)
        Me.chkInternationalDate.Name = "chkInternationalDate"
        Me.chkInternationalDate.Size = New System.Drawing.Size(227, 17)
        Me.chkInternationalDate.TabIndex = 17
        Me.chkInternationalDate.Text = "Use International Date format (yyyy-mm-dd)"
        Me.chkInternationalDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.chkInternationalDate.UseVisualStyleBackColor = True
        '
        'chkInternationalNumber
        '
        Me.chkInternationalNumber.AutoSize = True
        Me.chkInternationalNumber.Location = New System.Drawing.Point(255, 20)
        Me.chkInternationalNumber.Name = "chkInternationalNumber"
        Me.chkInternationalNumber.Size = New System.Drawing.Size(173, 17)
        Me.chkInternationalNumber.TabIndex = 16
        Me.chkInternationalNumber.Text = "Use Comma Decimal Separator"
        Me.chkInternationalNumber.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.chkInternationalNumber.UseVisualStyleBackColor = True
        '
        'chkList
        '
        Me.chkList.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.chkList.CheckOnClick = True
        Me.chkList.FormattingEnabled = True
        Me.chkList.Location = New System.Drawing.Point(136, 40)
        Me.chkList.Name = "chkList"
        Me.chkList.Size = New System.Drawing.Size(105, 139)
        Me.chkList.TabIndex = 10
        '
        'lblOutputDir
        '
        Me.lblOutputDir.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lblOutputDir.AutoSize = True
        Me.lblOutputDir.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblOutputDir.Location = New System.Drawing.Point(46, 191)
        Me.lblOutputDir.Name = "lblOutputDir"
        Me.lblOutputDir.Size = New System.Drawing.Size(52, 13)
        Me.lblOutputDir.TabIndex = 9
        Me.lblOutputDir.Text = "Directory:"
        '
        'btnOutput
        '
        Me.btnOutput.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnOutput.Location = New System.Drawing.Point(574, 187)
        Me.btnOutput.Name = "btnOutput"
        Me.btnOutput.Size = New System.Drawing.Size(25, 20)
        Me.btnOutput.TabIndex = 1
        Me.btnOutput.Text = "..."
        Me.btnOutput.UseVisualStyleBackColor = True
        '
        'chkFrequencies
        '
        Me.chkFrequencies.AutoSize = True
        Me.chkFrequencies.Location = New System.Drawing.Point(255, 74)
        Me.chkFrequencies.Name = "chkFrequencies"
        Me.chkFrequencies.Size = New System.Drawing.Size(119, 17)
        Me.chkFrequencies.TabIndex = 6
        Me.chkFrequencies.Text = "Output Frequencies"
        Me.chkFrequencies.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.chkFrequencies.UseVisualStyleBackColor = True
        '
        'cmbAggregateType
        '
        Me.cmbAggregateType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbAggregateType.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmbAggregateType.FormattingEnabled = True
        Me.cmbAggregateType.Location = New System.Drawing.Point(136, 16)
        Me.cmbAggregateType.Name = "cmbAggregateType"
        Me.cmbAggregateType.Size = New System.Drawing.Size(104, 21)
        Me.cmbAggregateType.TabIndex = 1
        '
        'lblAggregateType
        '
        Me.lblAggregateType.AutoSize = True
        Me.lblAggregateType.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblAggregateType.Location = New System.Drawing.Point(46, 20)
        Me.lblAggregateType.Name = "lblAggregateType"
        Me.lblAggregateType.Size = New System.Drawing.Size(86, 13)
        Me.lblAggregateType.TabIndex = 2
        Me.lblAggregateType.Text = "Aggregate Type:"
        '
        'txtOutput
        '
        Me.txtOutput.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtOutput.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtOutput.Location = New System.Drawing.Point(136, 187)
        Me.txtOutput.Name = "txtOutput"
        Me.txtOutput.Size = New System.Drawing.Size(437, 20)
        Me.txtOutput.TabIndex = 0
        '
        'lblTimeScale
        '
        Me.lblTimeScale.AutoSize = True
        Me.lblTimeScale.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTimeScale.Location = New System.Drawing.Point(46, 43)
        Me.lblTimeScale.Name = "lblTimeScale"
        Me.lblTimeScale.Size = New System.Drawing.Size(63, 13)
        Me.lblTimeScale.TabIndex = 1
        Me.lblTimeScale.Text = "Time Scale:"
        '
        'chkDroughtPeriods
        '
        Me.chkDroughtPeriods.AutoSize = True
        Me.chkDroughtPeriods.Location = New System.Drawing.Point(255, 56)
        Me.chkDroughtPeriods.Name = "chkDroughtPeriods"
        Me.chkDroughtPeriods.Size = New System.Drawing.Size(137, 17)
        Me.chkDroughtPeriods.TabIndex = 5
        Me.chkDroughtPeriods.Text = "Output Drought Periods"
        Me.chkDroughtPeriods.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.chkDroughtPeriods.UseVisualStyleBackColor = True
        '
        'dlgFolder
        '
        Me.dlgFolder.RootFolder = System.Environment.SpecialFolder.MyComputer
        '
        'btnGenerate
        '
        Me.btnGenerate.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.btnGenerate.Location = New System.Drawing.Point(267, 406)
        Me.btnGenerate.Name = "btnGenerate"
        Me.btnGenerate.Size = New System.Drawing.Size(75, 23)
        Me.btnGenerate.TabIndex = 0
        Me.btnGenerate.Text = "&Generate"
        Me.btnGenerate.UseVisualStyleBackColor = True
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(608, 439)
        Me.Controls.Add(Me.btnGenerate)
        Me.Controls.Add(Me.grpOutput)
        Me.Controls.Add(Me.lblTitle)
        Me.Controls.Add(Me.grpInput)
        Me.DoubleBuffered = True
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimumSize = New System.Drawing.Size(624, 477)
        Me.Name = "frmMain"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "SPI Generator"
        Me.grpInput.ResumeLayout(False)
        Me.grpInput.PerformLayout()
        Me.grpOutput.ResumeLayout(False)
        Me.grpOutput.PerformLayout()
        Me.grpOutputOptions.ResumeLayout(False)
        Me.grpOutputOptions.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents grpInput As System.Windows.Forms.GroupBox
    Friend WithEvents btnInputDirectory As System.Windows.Forms.Button
    Friend WithEvents txtInputDirectory As System.Windows.Forms.TextBox
    Friend WithEvents lblTitle As System.Windows.Forms.Label
    Friend WithEvents grpOutput As System.Windows.Forms.GroupBox
    Friend WithEvents btnOutput As System.Windows.Forms.Button
    Friend WithEvents txtOutput As System.Windows.Forms.TextBox
    Friend WithEvents dlgFile As System.Windows.Forms.OpenFileDialog
    Friend WithEvents dlgFolder As System.Windows.Forms.FolderBrowserDialog
    Friend WithEvents btnGenerate As System.Windows.Forms.Button
    Friend WithEvents BackgroundWorker1 As System.ComponentModel.BackgroundWorker
    Friend WithEvents cmbAggregateType As System.Windows.Forms.ComboBox
    Friend WithEvents lblAggregateType As System.Windows.Forms.Label
    Friend WithEvents lblTimeScale As System.Windows.Forms.Label
    Friend WithEvents cmbInputDataType As System.Windows.Forms.ComboBox
    Friend WithEvents lblInputType As System.Windows.Forms.Label
    Friend WithEvents rdoInputFile As System.Windows.Forms.RadioButton
    Friend WithEvents rdoInputDirectory As System.Windows.Forms.RadioButton
    Friend WithEvents chkFrequencies As System.Windows.Forms.CheckBox
    Friend WithEvents chkDroughtPeriods As System.Windows.Forms.CheckBox
    Friend WithEvents lblOutputDir As System.Windows.Forms.Label
    Friend WithEvents btnInputFile As System.Windows.Forms.Button
    Friend WithEvents txtInputFile As System.Windows.Forms.TextBox
    Friend WithEvents chkList As System.Windows.Forms.CheckedListBox
    Friend WithEvents cmbDataDelimiter As System.Windows.Forms.ComboBox
    Friend WithEvents lblDataDelimiter As System.Windows.Forms.Label
    Friend WithEvents chkInternationalDate As System.Windows.Forms.CheckBox
    Friend WithEvents chkInternationalNumber As System.Windows.Forms.CheckBox
    Friend WithEvents grpOutputOptions As GroupBox
    Friend WithEvents rdoSpace As RadioButton
    Friend WithEvents rdoExcel As RadioButton
    Friend WithEvents rdoComma As RadioButton
End Class
