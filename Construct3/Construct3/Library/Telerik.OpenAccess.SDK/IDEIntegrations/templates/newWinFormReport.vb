Public Partial Class %ClassName%
    Inherits System.Windows.Forms.Form

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub


    Private Sub objectView1_PositionChanged()
        Me.labelNavLocation.Text = (((Me.BindingContext(ObjectView1).Position + 1).ToString + " of  ") _
                    + Me.BindingContext(ObjectView1).Count.ToString)
    End Sub
    Private Sub buttonNavFirst_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles buttonNavFirst.Click
        Me.BindingContext(ObjectView1).Position = 0
        Me.objectView1_PositionChanged()
    End Sub

    Private Sub buttonNavPrev_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles buttonNavPrev.Click
        Me.BindingContext(ObjectView1).Position = (Me.BindingContext(ObjectView1).Position - 1)
        Me.objectView1_PositionChanged()
    End Sub

    Private Sub buttonNavNext_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles buttonNavNext.Click
        Me.BindingContext(ObjectView1).Position = (Me.BindingContext(ObjectView1).Position + 1)
        Me.objectView1_PositionChanged()
    End Sub

    Private Sub buttonLast_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles buttonLast.Click
        Me.BindingContext(ObjectView1).Position = (Me.ObjectView1.Count - 1)
        Me.objectView1_PositionChanged()
    End Sub

    Private Sub buttonAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles buttonAdd.Click
        Try
            'Clear out the current edits
            Me.BindingContext(ObjectView1).EndCurrentEdit()
            Me.BindingContext(ObjectView1).AddNew()
        Catch eEndEdit As System.Exception
            System.Windows.Forms.MessageBox.Show(eEndEdit.Message)
        End Try
        Me.objectView1_PositionChanged()
    End Sub

    Private Sub buttonDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles buttonDelete.Click
        If (Me.BindingContext(ObjectView1).Count > 0) Then
            Me.BindingContext(ObjectView1).RemoveAt(Me.BindingContext(ObjectView1).Position)
            Me.objectView1_PositionChanged()
        End If
    End Sub

    Private Sub buttonCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles buttonCancel.Click
        Me.BindingContext(ObjectView1).CancelCurrentEdit()
        Me.ObjectProvider1.CancelAll()
    End Sub

    Private Sub buttonSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles buttonSave.Click
        Me.BindingContext(ObjectView1).EndCurrentEdit()
        Me.ObjectProvider1.SaveAll()
    End Sub

    Private Sub Form3_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.objectView1_PositionChanged()
    End Sub

End Class
