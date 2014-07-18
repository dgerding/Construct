
/****** Object:  StoredProcedure [dbo].[Create_Login]    Script Date: 07/18/2014 10:52:30 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Create_Login]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Create_Login]
GO



/****** Object:  StoredProcedure [dbo].[Create_Login]    Script Date: 07/18/2014 10:52:30 ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[Create_Login]	
@username sysname AS
BEGIN	
SET NOCOUNT ON	DECLARE @SQL NVARCHAR(4000);		
SET @SQL = 'CREATE LOGIN ' + @username + ' WITH PASSWORD = ''ConstructifyConstructifyConstructify'', DEFAULT_DATABASE=[Construct], DEFAULT_LANGUAGE=[English], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF';	
EXECUTE(@SQL);		
EXEC sys.sp_addsrvrolemember @loginame = @username, @rolename = N'sysadmin';		
SET @SQL = 'ALTER LOGIN ' + @username + ' DISABLE';	
EXECUTE(@SQL);
END;

GO

