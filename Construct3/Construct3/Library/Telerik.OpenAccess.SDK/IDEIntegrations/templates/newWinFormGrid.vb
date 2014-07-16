Public Partial Class %ClassName%
    Inherits System.Windows.Forms.Form


    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub


    Private Sub buttonSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles buttonSave.Click
        Me.ObjectProvider1.SaveAll()
    End Sub
    Private Sub buttonCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles buttonCancel.Click
        Me.ObjectProvider1.CancelAll()
    End Sub
End Class
