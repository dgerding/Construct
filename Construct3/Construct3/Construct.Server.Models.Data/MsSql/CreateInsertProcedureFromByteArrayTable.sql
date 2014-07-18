
/****** Object:  StoredProcedure [dbo].[CreateInsertProcedureForByteArrayTable]    Script Date: 07/18/2014 10:56:18 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CreateInsertProcedureForByteArrayTable]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[CreateInsertProcedureForByteArrayTable]
GO



/****** Object:  StoredProcedure [dbo].[CreateInsertProcedureForByteArrayTable]    Script Date: 07/18/2014 10:56:18 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- This is used to create a stored procedure 
-- on a 'per ByteArray property basis'
-- that can handle varbinary(max) parameters
-- =============================================
CREATE PROCEDURE [dbo].[CreateInsertProcedureForByteArrayTable]
		@tableName NVARCHAR(MAX)

AS
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @str NVARCHAR(MAX)
	
	select @str='
	IF OBJECTPROPERTY(object_id(''dbo.z_InsertByteArrayInto_' + @tableName + '''), N''IsProcedure'') = 1
	   
	   DROP PROCEDURE [dbo].[z_InsertByteArrayInto_' + @tableName   + ']
	
	'
    EXEC(@str)
	
	SELECT @str='
	
	
	Create Procedure z_InsertByteArrayInto_' + @tableName +
	'    
    @ItemID UNIQUEIDENTIFIER ,
    @SourceID UNIQUEIDENTIFIER ,
    @PropertyID UNIQUEIDENTIFIER, 
    @StartTime DATETIME2 ,
    @Interval INT ,
    @Latitude NVARCHAR(50) ,
    @Longitude NVARCHAR(50) ,
    @Value VARBINARY(max)
AS 
    BEGIN
        SET NOCOUNT ON;

        insert into [dbo].[ReplaceMe](
	[ItemID], 
	[SourceID],
	[PropertyID], 
	[StartTime],
	[Interval],
	[Latitude],
	[Longitude],
	[Value]
	)
	values
	(
	@ItemID,
	@SourceID,
	@PropertyID,
	@StartTime,
	@Interval,
	@Latitude,
	@Longitude,
	@Value
	)
	
	END
	'
	SELECT  @str = REPLACE(@str, 'ReplaceMe', @tableName)
    
    EXEC (@str)
    
END

GO

