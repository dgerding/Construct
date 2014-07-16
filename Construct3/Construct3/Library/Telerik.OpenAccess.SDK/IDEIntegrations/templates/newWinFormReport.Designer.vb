Public Partial Class %ClassName%
    Inherits System.Windows.Forms.Form

#Region " Windows Form Designer generated code "

    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Friend WithEvents buttonNavFirst As System.Windows.Forms.Button
    Friend WithEvents buttonNavPrev As System.Windows.Forms.Button
    Friend WithEvents labelNavLocation As System.Windows.Forms.Label
    Friend WithEvents buttonNavNext As System.Windows.Forms.Button
    Friend WithEvents buttonLast As System.Windows.Forms.Button
    Friend WithEvents buttonAdd As System.Windows.Forms.Button
    Friend WithEvents buttonDelete As System.Windows.Forms.Button
    Friend WithEvents buttonCancel As System.Windows.Forms.Button
    Friend WithEvents buttonSave As System.Windows.Forms.Button
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Me.buttonNavFirst = New System.Windows.Forms.Button
        Me.buttonNavPrev = New System.Windows.Forms.Button
        Me.labelNavLocation = New System.Windows.Forms.Label
        Me.buttonNavNext = New System.Windows.Forms.Button
        Me.buttonLast = New System.Windows.Forms.Button
        Me.buttonAdd = New System.Windows.Forms.Button
        Me.buttonDelete = New System.Windows.Forms.Button
        Me.buttonCancel = New System.Windows.Forms.Button
        Me.buttonSave = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'buttonNavFirst
        '
        Me.buttonNavFirst.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.buttonNavFirst.Location = New System.Drawing.Point(224, 16)
        Me.buttonNavFirst.Name = "buttonNavFirst"
        Me.buttonNavFirst.Size = New System.Drawing.Size(40, 23)
        Me.buttonNavFirst.TabIndex = 10
        Me.buttonNavFirst.Text = "<<"
        '
        'buttonNavPrev
        '
        Me.buttonNavPrev.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.buttonNavPrev.Location = New System.Drawing.Point(264, 16)
        Me.buttonNavPrev.Name = "buttonNavPrev"
        Me.buttonNavPrev.Size = New System.Drawing.Size(35, 23)
        Me.buttonNavPrev.TabIndex = 11
        Me.buttonNavPrev.Text = "<"
        '
        'labelNavLocation
        '
        Me.labelNavLocation.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.labelNavLocation.BackColor = System.Drawing.Color.White
        Me.labelNavLocation.Location = New System.Drawing.Point(296, 16)
        Me.labelNavLocation.Name = "labelNavLocation"
        Me.labelNavLocation.Size = New System.Drawing.Size(95, 23)
        Me.labelNavLocation.TabIndex = 9
        Me.labelNavLocation.Text = "No Objects"
        Me.labelNavLocation.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'buttonNavNext
        '
        Me.buttonNavNext.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.buttonNavNext.Location = New System.Drawing.Point(392, 16)
        Me.buttonNavNext.Name = "buttonNavNext"
        Me.buttonNavNext.Size = New System.Drawing.Size(35, 23)
        Me.buttonNavNext.TabIndex = 0
        Me.buttonNavNext.Text = ">"
        '
        'buttonLast
        '
        Me.buttonLast.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.buttonLast.Location = New System.Drawing.Point(424, 16)
        Me.buttonLast.Name = "buttonLast"
        Me.buttonLast.Size = New System.Drawing.Size(40, 23)
        Me.buttonLast.TabIndex = 13
        Me.buttonLast.Text = ">>"
        '
        'buttonAdd
        '
        Me.buttonAdd.Location = New System.Drawing.Point(16, 16)
        Me.buttonAdd.Name = "buttonAdd"
        Me.buttonAdd.TabIndex = 14
        Me.buttonAdd.Text = "&Add"
        '
        'buttonDelete
        '
        Me.buttonDelete.Location = New System.Drawing.Point(104, 16)
        Me.buttonDelete.Name = "buttonDelete"
        Me.buttonDelete.TabIndex = 15
        Me.buttonDelete.Text = "&Delete"
        '
        'buttonCancel
        '
        Me.buttonCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.buttonCancel.Location = New System.Drawing.Point(512, 16)
        Me.buttonCancel.Name = "buttonCancel"
        Me.buttonCancel.TabIndex = 16
        Me.buttonCancel.Text = "&Cancel"
        '
        'buttonSave
        '
        Me.buttonSave.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.buttonSave.Location = New System.Drawing.Point(600, 16)
        Me.buttonSave.Name = "buttonSave"
        Me.buttonSave.TabIndex = 17
        Me.buttonSave.Text = "&Save"
        '
        '%ClassName%
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(696, 333)
        Me.Controls.Add(Me.buttonNavFirst)
        Me.Controls.Add(Me.buttonNavPrev)
        Me.Controls.Add(Me.labelNavLocation)
        Me.Controls.Add(Me.buttonNavNext)
        Me.Controls.Add(Me.buttonLast)
        Me.Controls.Add(Me.buttonAdd)
        Me.Controls.Add(Me.buttonDelete)
        Me.Controls.Add(Me.buttonCancel)
        Me.Controls.Add(Me.buttonSave)
        Me.Name = "%ClassName%"
        Me.Text = "%ProductName% Data Form"
        Me.ResumeLayout(False)

    End Sub

#End Region


End Class
