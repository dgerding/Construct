Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Text
Imports System.Windows.Forms

Partial Public Class %ClassName%
    Inherits Form
    ''' <summary>
    ''' Summary description for %ClassName%.
    ''' </summary>
    Public Sub New()
        InitializeComponent()
        AddHandler Me.Closed, AddressOf FormClosedEvent
        ' Add the following code if you want to dispose on close
        Me.bindingNavigator1.BindingSource = objectView1
    End Sub

    Private Sub FormClosedEvent(ByVal sender As Object, ByVal e As System.EventArgs)
        Me.Dispose()
    End Sub

    Private Sub bindingNavigatorAddNewItem_Click(ByVal sender As Object, ByVal e As EventArgs)
        Try
            ' Clear out the current edits
            Me.BindingContext(objectView1).EndCurrentEdit()
            Me.BindingContext(objectView1).AddNew()
        Catch eEndEdit As System.Exception
            System.Windows.Forms.MessageBox.Show(eEndEdit.Message)
        End Try

    End Sub



    Private Sub bindingNavigatorSaveItem_Click(ByVal sender As Object, ByVal e As EventArgs)
        Me.BindingContext(objectView1).EndCurrentEdit()
        Me.objectProvider1.SaveAll()

    End Sub

    Private Sub bindingNavigatorDeleteItem_Click(ByVal sender As Object, ByVal e As EventArgs)

    End Sub

    Private Sub bindingNavigatorCancelItem_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim r As DialogResult = MessageBox.Show("Do you really want to cancel ?", "confirm", MessageBoxButtons.YesNo)
        If r = DialogResult.Yes Then
            Me.BindingContext(objectView1).CancelCurrentEdit()
            Me.objectProvider1.CancelAll()
        End If
    End Sub

End Class