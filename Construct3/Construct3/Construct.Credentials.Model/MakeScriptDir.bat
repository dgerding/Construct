if not exist "c:\Construct\Scripts" (
	md "C:\Construct\Scripts"
	)
pause

if not exist "c:\Construct\Scripts\ValidateCoreEntities.sql" (
	copy ValidateCoreEntities.sql "c:\Construct\Scripts\ValidateCoreEntities.sql"
	)
	pause

if not exist "c:\Construct\Scripts\DeleteAllRows" (
	copy DeleteAllRows.sql "c:\Construct\Scripts\DeleteAllRows.sql"
	)
	pause

if not exist "c:\Construct\Scripts\LoadTestItems.sql" (
	copy LoadTestItems.sql "c:\Construct\Scripts\LoadTestItems.sql"
	)
	pause

if not exist "c:\Construct\Scripts\DeleteItemZTables.sql" (
	copy DeleteItemZTables.sql "c:\Construct\Scripts\DeleteItemZTables.sql"
	)
	pause