'Copyright (c) %CompanyName%.  All rights reserved.
'
Imports %ProductNamespace%
Imports %ProductNamespace%.Util

public class %ClassName%
	implements IObjectScopeProvider
	implements IObjectContainerProvider
	implements IDBConnection

	private myContainer as ObjectContainer
	private shared the%ClassName% as %ClassName%

	Public Sub New ()
    End Sub

    Public Sub New(ByVal aContainer As ObjectContainer)
        myContainer = aContainer
    End Sub

    Public Shared Function Database() As Database
        Return Nothing
    End Function

    Public Shared Function ObjectScope() As IObjectScope
        Return Nothing
    End Function
    Public Shared Function GetNewObjectScope() As IObjectScope
        Return Nothing
    End Function

    Public Shared Function ObjectContainer() As ObjectContainer
        If the%ClassName% Is Nothing Then
            the%ClassName% = New %ClassName%
        End If
        If the%ClassName%.myContainer Is Nothing Then
            the%ClassName%.myContainer = GetNewObjectContainer()
        End If

        Return the%ClassName%.myContainer
    End Function

    Public Shared Function GetNewObjectContainer() As ObjectContainer
        Dim oc As ObjectContainer = New ObjectContainer
        oc.DBConnection = New %ClassName%(oc)
        oc.AutoSync = True
        Return oc
    End Function

#Region " IDBConnection Members "

    Public Function Load(ByVal data As FillInstructionData) As %ProductNamespace%.ObjectContainer.ChangeSet Implements IDBConnection.Load
        Dim local_inst As %WebserviceNamespace%.FillInstructionData = New %WebserviceNamespace%.FillInstructionData
        data.CopyTo(local_inst)
        Dim erg As Object
        Dim ccData As %ProductNamespace%.ObjectContainer.ChangeSet
        Dim srv As %WebserviceNamespace%.%ProxyClassName% = New %WebserviceNamespace%.%ProxyClassName%
        erg = srv.Load(local_inst)
        If (Not erg Is Nothing) Then
            ccData = New %ProductNamespace%.ObjectContainer.ChangeSet(erg)
        End If
        Return ccData
    End Function

    Public Sub Save(ByVal data As %ProductNamespace%.ObjectContainer.ChangeSet) Implements IDBConnection.Save
        Dim local_chg As %WebserviceNamespace%.ChangeSet = New %WebserviceNamespace%.ChangeSet
        data.CopyTo(local_chg)
        Dim srv As %WebserviceNamespace%.%ProxyClassName% = New %WebserviceNamespace%.%ProxyClassName%
        srv.Save(local_chg)
    End Sub

    Public Function Sync(ByVal data As %ProductNamespace%.ObjectContainer.ChangeSet) As %ProductNamespace%.ObjectContainer.ChangeSet Implements IDBConnection.Sync
        Dim local_chg As %WebserviceNamespace%.ChangeSet = New %WebserviceNamespace%.ChangeSet
        data.CopyTo(local_chg)
        Dim srv As %WebserviceNamespace%.%ProxyClassName% = New %WebserviceNamespace%.%ProxyClassName%
        Dim erg As Object = srv.Sync(local_chg)

        Dim ccData As %ProductNamespace%.ObjectContainer.ChangeSet
        If (Not erg Is Nothing) Then
            ccData = New %ProductNamespace%.ObjectContainer.ChangeSet(erg)
        End If
        Return ccData
    End Function
#End Region
end class
