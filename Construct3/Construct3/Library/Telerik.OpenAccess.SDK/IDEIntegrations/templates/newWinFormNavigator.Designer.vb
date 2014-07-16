Partial Class %ClassName%
    ''' <summary>
    ''' Required designer variable.
    ''' </summary>
    Private components As System.ComponentModel.IContainer = Nothing

    ''' <summary>
    ''' Clean up any resources being used.
    ''' </summary>
    ''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso (components IsNot Nothing) Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

#Region "Windows Form Designer generated code"

    ''' <summary>
    ''' Required method for Designer support - do not modify
    ''' the contents of this method with the code editor.
    ''' </summary>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As New System.ComponentModel.ComponentResourceManager(GetType(%ClassName%))
        Me.bindingNavigator1 = New System.Windows.Forms.BindingNavigator(Me.components)
        Me.bindingNavigatorAddNewItem = New System.Windows.Forms.ToolStripButton()
        Me.bindingNavigatorCountItem = New System.Windows.Forms.ToolStripLabel()
        Me.bindingNavigatorDeleteItem = New System.Windows.Forms.ToolStripButton()
        Me.bindingNavigatorSeparator = New System.Windows.Forms.ToolStripSeparator()
        Me.bindingNavigatorMoveFirstItem = New System.Windows.Forms.ToolStripButton()
        Me.bindingNavigatorMovePreviousItem = New System.Windows.Forms.ToolStripButton()
        Me.bindingNavigatorSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.bindingNavigatorPositionItem = New System.Windows.Forms.ToolStripTextBox()
        Me.bindingNavigatorSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.bindingNavigatorMoveNextItem = New System.Windows.Forms.ToolStripButton()
        Me.bindingNavigatorMoveLastItem = New System.Windows.Forms.ToolStripButton()
        Me.toolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.bindingNavigatorCancelItem = New System.Windows.Forms.ToolStripButton()
        Me.bindingNavigatorSaveItem = New System.Windows.Forms.ToolStripButton()
        DirectCast((Me.bindingNavigator1), System.ComponentModel.ISupportInitialize).BeginInit()
        Me.bindingNavigator1.SuspendLayout()
        Me.SuspendLayout()
        '
        ' bindingNavigator1
        '
        Me.bindingNavigator1.AddNewItem = Me.bindingNavigatorAddNewItem
        Me.bindingNavigator1.CountItem = Me.bindingNavigatorCountItem
        Me.bindingNavigator1.DeleteItem = Me.bindingNavigatorDeleteItem
        Me.bindingNavigator1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.bindingNavigatorAddNewItem, Me.bindingNavigatorDeleteItem, Me.bindingNavigatorSeparator, Me.bindingNavigatorMoveFirstItem, Me.bindingNavigatorMovePreviousItem, Me.bindingNavigatorSeparator1, _
        Me.bindingNavigatorPositionItem, Me.bindingNavigatorCountItem, Me.bindingNavigatorSeparator2, Me.bindingNavigatorMoveNextItem, Me.bindingNavigatorMoveLastItem, Me.toolStripSeparator1, _
        Me.bindingNavigatorCancelItem, Me.bindingNavigatorSaveItem})
        Me.bindingNavigator1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow
        Me.bindingNavigator1.Location = New System.Drawing.Point(0, 0)
        Me.bindingNavigator1.MoveFirstItem = Me.bindingNavigatorMoveFirstItem
        Me.bindingNavigator1.MoveLastItem = Me.bindingNavigatorMoveLastItem
        Me.bindingNavigator1.MoveNextItem = Me.bindingNavigatorMoveNextItem
        Me.bindingNavigator1.MovePreviousItem = Me.bindingNavigatorMovePreviousItem
        Me.bindingNavigator1.Name = "bindingNavigator1"
        Me.bindingNavigator1.Padding = New System.Windows.Forms.Padding(180, 0, 1, 0)
        Me.bindingNavigator1.PositionItem = Me.bindingNavigatorPositionItem
        Me.bindingNavigator1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
        Me.bindingNavigator1.Size = New System.Drawing.Size(657, 25)
        Me.bindingNavigator1.Stretch = True
        Me.bindingNavigator1.TabIndex = 0
        Me.bindingNavigator1.Text = "bindingNavigator1"
        '
        ' bindingNavigatorAddNewItem
        '
        Me.bindingNavigatorAddNewItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.bindingNavigatorAddNewItem.Image = DirectCast((resources.GetObject("bindingNavigatorAddNewItem.Image")), System.Drawing.Image)
        Me.bindingNavigatorAddNewItem.Margin = New System.Windows.Forms.Padding(10, 1, 10, 2)
        Me.bindingNavigatorAddNewItem.Name = "bindingNavigatorAddNewItem"
        Me.bindingNavigatorAddNewItem.RightToLeftAutoMirrorImage = True
        Me.bindingNavigatorAddNewItem.Size = New System.Drawing.Size(23, 22)
        Me.bindingNavigatorAddNewItem.Text = "Add new"
        AddHandler Me.bindingNavigatorAddNewItem.Click, AddressOf Me.bindingNavigatorAddNewItem_Click
        '
        ' bindingNavigatorCountItem
        '
        Me.bindingNavigatorCountItem.Name = "bindingNavigatorCountItem"
        Me.bindingNavigatorCountItem.Size = New System.Drawing.Size(36, 22)
        Me.bindingNavigatorCountItem.Text = "of {0}"
        Me.bindingNavigatorCountItem.ToolTipText = "Total number of items"
        '
        ' bindingNavigatorDeleteItem
        '
        Me.bindingNavigatorDeleteItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.bindingNavigatorDeleteItem.Image = DirectCast((resources.GetObject("bindingNavigatorDeleteItem.Image")), System.Drawing.Image)
        Me.bindingNavigatorDeleteItem.Margin = New System.Windows.Forms.Padding(0, 1, 10, 2)
        Me.bindingNavigatorDeleteItem.Name = "bindingNavigatorDeleteItem"
        Me.bindingNavigatorDeleteItem.RightToLeftAutoMirrorImage = True
        Me.bindingNavigatorDeleteItem.Size = New System.Drawing.Size(23, 22)
        Me.bindingNavigatorDeleteItem.Text = "Delete"
        AddHandler Me.bindingNavigatorDeleteItem.Click, AddressOf Me.bindingNavigatorDeleteItem_Click
        '
        ' bindingNavigatorSeparator
        '
        Me.bindingNavigatorSeparator.ForeColor = System.Drawing.SystemColors.ControlDarkDark
        Me.bindingNavigatorSeparator.Name = "bindingNavigatorSeparator"
        Me.bindingNavigatorSeparator.Size = New System.Drawing.Size(6, 25)
        '
        ' bindingNavigatorMoveFirstItem
        '
        Me.bindingNavigatorMoveFirstItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.bindingNavigatorMoveFirstItem.Image = DirectCast((resources.GetObject("bindingNavigatorMoveFirstItem.Image")), System.Drawing.Image)
        Me.bindingNavigatorMoveFirstItem.Name = "bindingNavigatorMoveFirstItem"
        Me.bindingNavigatorMoveFirstItem.RightToLeftAutoMirrorImage = True
        Me.bindingNavigatorMoveFirstItem.Size = New System.Drawing.Size(23, 22)
        Me.bindingNavigatorMoveFirstItem.Text = "Move first"
        '
        ' bindingNavigatorMovePreviousItem
        '
        Me.bindingNavigatorMovePreviousItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.bindingNavigatorMovePreviousItem.Image = DirectCast((resources.GetObject("bindingNavigatorMovePreviousItem.Image")), System.Drawing.Image)
        Me.bindingNavigatorMovePreviousItem.Name = "bindingNavigatorMovePreviousItem"
        Me.bindingNavigatorMovePreviousItem.RightToLeftAutoMirrorImage = True
        Me.bindingNavigatorMovePreviousItem.Size = New System.Drawing.Size(23, 22)
        Me.bindingNavigatorMovePreviousItem.Text = "Move previous"
        '
        ' bindingNavigatorSeparator1
        '
        Me.bindingNavigatorSeparator1.Name = "bindingNavigatorSeparator1"
        Me.bindingNavigatorSeparator1.Size = New System.Drawing.Size(6, 25)
        '
        ' bindingNavigatorPositionItem
        '
        Me.bindingNavigatorPositionItem.AccessibleName = "Position"
        Me.bindingNavigatorPositionItem.AutoSize = False
        Me.bindingNavigatorPositionItem.Name = "bindingNavigatorPositionItem"
        Me.bindingNavigatorPositionItem.Size = New System.Drawing.Size(50, 21)
        Me.bindingNavigatorPositionItem.Text = "0"
        Me.bindingNavigatorPositionItem.ToolTipText = "Current position"
        '
        ' bindingNavigatorSeparator2
        '
        Me.bindingNavigatorSeparator2.Name = "bindingNavigatorSeparator2"
        Me.bindingNavigatorSeparator2.Size = New System.Drawing.Size(6, 25)
        '
        ' bindingNavigatorMoveNextItem
        '
        Me.bindingNavigatorMoveNextItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.bindingNavigatorMoveNextItem.Image = DirectCast((resources.GetObject("bindingNavigatorMoveNextItem.Image")), System.Drawing.Image)
        Me.bindingNavigatorMoveNextItem.Name = "bindingNavigatorMoveNextItem"
        Me.bindingNavigatorMoveNextItem.RightToLeftAutoMirrorImage = True
        Me.bindingNavigatorMoveNextItem.Size = New System.Drawing.Size(23, 22)
        Me.bindingNavigatorMoveNextItem.Text = "Move next"
        '
        ' bindingNavigatorMoveLastItem
        '
        Me.bindingNavigatorMoveLastItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.bindingNavigatorMoveLastItem.Image = DirectCast((resources.GetObject("bindingNavigatorMoveLastItem.Image")), System.Drawing.Image)
        Me.bindingNavigatorMoveLastItem.Name = "bindingNavigatorMoveLastItem"
        Me.bindingNavigatorMoveLastItem.RightToLeftAutoMirrorImage = True
        Me.bindingNavigatorMoveLastItem.Size = New System.Drawing.Size(23, 22)
        Me.bindingNavigatorMoveLastItem.Text = "Move last"
        '
        ' toolStripSeparator1
        '
        Me.toolStripSeparator1.BackColor = System.Drawing.SystemColors.Control
        Me.toolStripSeparator1.ForeColor = System.Drawing.SystemColors.ControlDarkDark
        Me.toolStripSeparator1.Name = "toolStripSeparator1"
        Me.toolStripSeparator1.Size = New System.Drawing.Size(6, 25)
        '
        ' bindingNavigatorCancelItem
        '
        Me.bindingNavigatorCancelItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.bindingNavigatorCancelItem.Image = DirectCast((resources.GetObject("bindingNavigatorCancelItem.Image")), System.Drawing.Image)
        Me.bindingNavigatorCancelItem.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.bindingNavigatorCancelItem.Margin = New System.Windows.Forms.Padding(10, 1, 0, 2)
        Me.bindingNavigatorCancelItem.Name = "bindingNavigatorCancelItem"
        Me.bindingNavigatorCancelItem.Size = New System.Drawing.Size(23, 22)
        Me.bindingNavigatorCancelItem.Text = "Cancel"
        Me.bindingNavigatorCancelItem.ToolTipText = "Cancel all operation"
        AddHandler Me.bindingNavigatorCancelItem.Click, AddressOf Me.bindingNavigatorCancelItem_Click
        '
        ' bindingNavigatorSaveItem
        '
        Me.bindingNavigatorSaveItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.bindingNavigatorSaveItem.Image = DirectCast((resources.GetObject("bindingNavigatorSaveItem.Image")), System.Drawing.Image)
        Me.bindingNavigatorSaveItem.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.bindingNavigatorSaveItem.Margin = New System.Windows.Forms.Padding(10, 1, 10, 2)
        Me.bindingNavigatorSaveItem.Name = "bindingNavigatorSaveItem"
        Me.bindingNavigatorSaveItem.Size = New System.Drawing.Size(23, 22)
        Me.bindingNavigatorSaveItem.Text = "Save"
        Me.bindingNavigatorSaveItem.ToolTipText = "Save all operations"
        AddHandler Me.bindingNavigatorSaveItem.Click, AddressOf Me.bindingNavigatorSaveItem_Click
        '
        ' newWinFormNavigator
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0F, 13.0F)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(737, 273)
        Me.Controls.Add(Me.bindingNavigator1)
        Me.Name = "newWinFormNavigator"
        Me.Text = "%ProductName% Data Form"
        DirectCast((Me.bindingNavigator1), System.ComponentModel.ISupportInitialize).EndInit()
        Me.bindingNavigator1.ResumeLayout(False)
        Me.bindingNavigator1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

#End Region

    Friend WithEvents bindingNavigator1 As System.Windows.Forms.BindingNavigator
    Friend WithEvents bindingNavigatorAddNewItem As System.Windows.Forms.ToolStripButton
    Friend WithEvents bindingNavigatorCountItem As System.Windows.Forms.ToolStripLabel
    Friend WithEvents bindingNavigatorDeleteItem As System.Windows.Forms.ToolStripButton
    Friend WithEvents bindingNavigatorMoveFirstItem As System.Windows.Forms.ToolStripButton
    Friend WithEvents bindingNavigatorMovePreviousItem As System.Windows.Forms.ToolStripButton
    Friend WithEvents bindingNavigatorSeparator As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents bindingNavigatorPositionItem As System.Windows.Forms.ToolStripTextBox
    Friend WithEvents bindingNavigatorSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents bindingNavigatorMoveNextItem As System.Windows.Forms.ToolStripButton
    Friend WithEvents bindingNavigatorMoveLastItem As System.Windows.Forms.ToolStripButton
    Friend WithEvents bindingNavigatorSeparator2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents toolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents bindingNavigatorCancelItem As System.Windows.Forms.ToolStripButton
    Friend WithEvents bindingNavigatorSaveItem As System.Windows.Forms.ToolStripButton
End Class
 