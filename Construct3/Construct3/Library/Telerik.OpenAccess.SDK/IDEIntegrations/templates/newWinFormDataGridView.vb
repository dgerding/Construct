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
    'Generated By %CompanyName%
    %Comments%Private Sub dataGridViewMaster_CellContentClick(ByVal sender As Object, _
    %Comments%ByVal e As DataGridViewCellEventArgs) _
    %Comments%Handles dataGridViewMaster.CellContentClick
        %Comments%Dim DataGridViewColumn As DataGridViewColumn = dataGridViewMaster.Columns(e.ColumnIndex)
        %Comments%If (TypeOf DataGridViewColumn Is DataGridViewLinkColumn) Then
        %Comments%If (Me.dataGridViewDetail.DataMember <> DataGridViewColumn.DataPropertyName) Then
        %Comments%Me.bindingsource1.DataMember = DataGridViewColumn.DataPropertyName
        %Comments%Me.bindingsource1.ResetBindings(True)
        %Comments%Else
        %Comments%Me.bindingsource1.ResetBindings(False)
        %Comments%End If
        %Comments%End If
    %Comments%End Sub

End Class
