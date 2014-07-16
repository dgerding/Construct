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
    Friend WithEvents buttonSave As System.Windows.Forms.Button
    Friend WithEvents buttonCancel As System.Windows.Forms.Button
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Me.buttonSave = New System.Windows.Forms.Button
        Me.buttonCancel = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'buttonSave
        '
        Me.buttonSave.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.buttonSave.Location = New System.Drawing.Point(608, 16)
        Me.buttonSave.Name = "buttonSave"
        Me.buttonSave.TabIndex = 1
        Me.buttonSave.Text = "&SaveAll"
        '
        'buttonCancel
        '
        Me.buttonCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.buttonCancel.Location = New System.Drawing.Point(8, 16)
        Me.buttonCancel.Name = "buttonCancel"
        Me.buttonCancel.TabIndex = 2
        Me.buttonCancel.Text = "&CancelAll"
        '
        '%ClassName%
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(696, 485)
        Me.Controls.Add(Me.buttonSave)
        Me.Controls.Add(Me.buttonCancel)
        Me.Name = "%ClassName%"
        Me.Text = "%ProductName% Data Form"
        Me.ResumeLayout(False)

    End Sub

#End Region

End Class
