'Copyright (c) %CompanyName%.  All rights reserved.
'
' usage example:
'
' ' Get ObjectScope from Helper
' Dim scope As IObjectScope = %ClassName%.ObjectScope
' ' start transaction
' scope.Transaction.Begin()
' ' create new persistent object person and add to scope
' Dim p As New Person
' scope.Add(p)
' ' commit transction
' scope.Transaction.Commit()
'
Imports %ProductNamespace%
Imports %ProductNamespace%.Util
'OABPE Imports System.Web
'OABPE Imports System.Threading

Public Class %ClassName%
    Implements IObjectScopeProvider
    Private _myDatabase As Database
    Private _myScope As IObjectScope

	Private Shared the%ClassName% As %ClassName%

    Public Sub New()
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
            Dim allDlls As System.Reflection.Assembly() = New System.Reflection.Assembly() {dll}

            the%ClassName%._myDatabase = %ProductNamespace%.Database.Get("%ConnectionName%", _
											xmlDoc.DocumentElement, allDlls)
        End If
    End Sub

    Public Shared Function Database() As Database
		If the%ClassName% Is Nothing Then
			the%ClassName% = New %ClassName%()
        End If

		If the%ClassName%._myDatabase Is Nothing Then
			the%ClassName%._myDatabase = %ProductNamespace%.Database.Get( "%ConnectionName%" )
        End If

		Return the%ClassName%._myDatabase
    End Function

    Public Shared Function ObjectScope() As IObjectScope
        Database()

		If the%ClassName%._myScope Is Nothing Then
            the%ClassName%._myScope = GetNewObjectScope()
        End If

		Return the%ClassName%._myScope
    End Function

    Public Shared Function GetNewObjectScope() As IObjectScope
        Dim db As Database = Database()

		Dim newScope As IObjectScope = db.GetObjectScope(%GetObjectScopeParameters%)
        Return newScope
    End Function
    'OABPE Public Shared Function GetPerRequestScope(ByVal context As HttpContext) As IObjectScope
    'OABPE     Dim key As String = HttpContext.Current.GetHashCode().ToString("x") + Thread.CurrentContext.ContextID.ToString()
    'OABPE     Dim scope As IObjectScope
    'OABPE     If context Is Nothing Then
    'OABPE         scope = ObjectScopeProvider1.GetNewObjectScope()
    'OABPE     Else
    'OABPE         scope = CType(context.Items(key), IObjectScope)
    'OABPE         If scope Is Nothing Then
    'OABPE             scope = ObjectScopeProvider1.GetNewObjectScope()
    'OABPE             context.Items(key) = scope
    'OABPE         End If
    'OABPE     End If
    'OABPE     Return scope
    'OABPE End Function
End Class
