'Copyright (c) %CompanyName%.  All rights reserved.
'

Imports %ProductNamespace%
Imports %ProductNamespace%.Util

public class %ClassName%
	implements IObjectScopeProvider
	implements IObjectContainerProvider
	implements IDBConnection

	private _myDatabase as Database
	private _myScope as IObjectScope
	private myContainer as ObjectContainer
	private shared the%ClassName% as %ClassName%

	Public Sub New ()
    End Sub

    Public Sub New(ByVal aContainer As ObjectContainer)
        myContainer = aContainer
    End Sub

	' Allows a dynamically loaded assembly to open the database.
	' Must be called before the ObjectScope is aquired.
    Public Shared Sub AdjustForDynamicLoad()
        If the%ClassName% Is Nothing Then
            the%ClassName% = New %ClassName%()
        End If

        If the%ClassName%._myDatabase Is Nothing Then
            Dim assumedInitialConfiguration As String
            assumedInitialConfiguration = "<%RootNodeName%>" _
                           & "<references>" _
                           & "<reference assemblyname='PLACEHOLDER' configrequired='True'/>" _
                           & "</references>" _
                           & "</%RootNodeName%>"
            Dim repl As String = "PLACEHOLDER"
            Dim dll As System.Reflection.Assembly = the%ClassName%.GetType().Assembly
            assumedInitialConfiguration = assumedInitialConfiguration.Replace(repl, dll.GetName().Name)
            Dim xmlDoc As New System.Xml.XmlDocument
            xmlDoc.LoadXml(assumedInitialConfiguration)
			Dim allDlls As System.Reflection.Assembly() = New System.Reflection.Assembly() { dll }

            the%ClassName%._myDatabase = %ProductNamespace%.Database.Get("%ConnectionName%", _
											xmlDoc.DocumentElement, allDlls)
        End If
    End Sub

	public shared function Database() as Database 
		if the%ClassName% is nothing then
			the%ClassName% = new %ClassName%()
        end if

		if the%ClassName%._myDatabase is nothing then
			the%ClassName%._myDatabase = %ProductNamespace%.Database.Get( "%ConnectionName%" )
'		try
'			the%ClassName%.myDatabase.Properties.ConnectionTimeout = 10000
'		catch (Exception)
'
        end if

		return the%ClassName%._myDatabase
	end function

	public shared function ObjectScope() as IObjectScope 
		Database()

		if the%ClassName%._myScope is nothing then
			the%ClassName%._myScope = GetNewObjectScope()
        end if

		return the%ClassName%._myScope
	end function

	public shared function GetNewObjectScope() as IObjectScope 
		dim db as Database = Database()

		dim newScope as IObjectScope = db.GetObjectScope(%GetObjectScopeParameters%)
		return newScope
	end function

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
		Dim os As IObjectScope	= %ClassName%.GetNewObjectScope()
        Dim ccData As %ProductNamespace%.ObjectContainer.ChangeSet

		os.Transaction.Begin()

'		try
			Dim fillInstr As FillInstruction = FillInstruction.Deserialize( data )

			ccData = fillInstr.Execute( os )
'		finally
			os.Transaction.Rollback()
			os.Dispose()

        Return ccData
    End Function

    Public Sub Save(ByVal data As %ProductNamespace%.ObjectContainer.ChangeSet) Implements IDBConnection.Save
		Dim os As IObjectScope	= %ClassName%.GetNewObjectScope()
		%ProductNamespace%.ObjectContainer.CommitChanges( data, %ProductNamespace%.ObjectContainer.Verify.Changed, os, true, false )
		os.Dispose()
    End Sub

    Public Function Sync(ByVal data As %ProductNamespace%.ObjectContainer.ChangeSet) As %ProductNamespace%.ObjectContainer.ChangeSet Implements IDBConnection.Sync
		Dim os As IObjectScope	= %ClassName%.GetNewObjectScope()
        Dim ccData As ObjectContainer.ChangeSet

		ccData = %ProductNamespace%.ObjectContainer.CommitChanges( data, %ProductNamespace%.ObjectContainer.Verify.Changed, os, true, true )

		os.Dispose()
        Return ccData
    End Function
#End Region
end class
