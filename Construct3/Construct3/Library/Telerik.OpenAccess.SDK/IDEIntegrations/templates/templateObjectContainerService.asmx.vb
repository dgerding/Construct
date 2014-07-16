'Copyright (c) %CompanyName%.  All rights reserved.
'
Imports System.Web.Services
Imports %ProductNamespace%
Imports %ProductNamespace%.Util

namespace %DefaultNamespace%

<System.Web.Services.WebService(Namespace := "http://tempuri.org/%DefaultNamespace%/%ClassName%")> _
Public Class %ClassName%
    Inherits System.Web.Services.WebService

#Region " Web Services Designer Generated Code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Web Services Designer.
        InitializeComponent()

        'Add your own initialization code after the InitializeComponent() call

    End Sub

    'Required by the Web Services Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Web Services Designer
    'It can be modified using the Web Services Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        components = New System.ComponentModel.Container()
    End Sub

    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        'CODEGEN: This procedure is required by the Web Services Designer
        'Do not modify it using the code editor.
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

#End Region

#Region " %ProductName% generated code "

    <WebMethod()> _
    Public Function Load(ByVal data As FillInstructionData) As %ProductNamespace%.ObjectContainer.ChangeSet
        Dim ocp As ObjectContainerProvider1 = New ObjectContainerProvider1
		Return ocp.Load( data )
    End Function

    <WebMethod()> _
    Public Sub Save(ByVal data As %ProductNamespace%.ObjectContainer.ChangeSet)
        Dim ocp As ObjectContainerProvider1 = New ObjectContainerProvider1
		ocp.Save( data )
    End Sub

    <WebMethod()> _
    Public Function Sync(ByVal data As %ProductNamespace%.ObjectContainer.ChangeSet) As %ProductNamespace%.ObjectContainer.ChangeSet
        Dim ocp As ObjectContainerProvider1 = New ObjectContainerProvider1
		Return ocp.Sync( data )
    End Function
#End Region
End Class

End Namespace