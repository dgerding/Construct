
/****** Object:  StoredProcedure [dbo].[AddBooleanPropertyValue]    Script Date: 07/18/2014 10:49:28 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AddBooleanPropertyValue]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[AddBooleanPropertyValue]
GO


/****** Object:  StoredProcedure [dbo].[AddBooleanPropertyValue]    Script Date: 07/18/2014 10:49:28 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[AddBooleanPropertyValue]
    @TableName NVARCHAR(MAX) ,
    @ItemID UNIQUEIDENTIFIER ,
    @SourceID UNIQUEIDENTIFIER ,
    @PropertyID UNIQUEIDENTIFIER ,
    @StartTime DATETIME2 ,
    @Interval INT ,
    @Latitude NVARCHAR(50) ,
    @Longitude NVARCHAR(50) ,
    @Value bit
AS 
    BEGIN
        SET NOCOUNT ON;
        
        DECLARE @IntervalStr nVARCHAR(MAX);
        DECLARE @PropertyIDStr nVARCHAR(MAX);
        
        if @Interval is null
        begin
			set @IntervalStr = 'NULL'
		end
		else
		begin
			Set @IntervalStr = '''' + Cast(@Interval as nvarchar(max)) + ''''
		end
		
		if @PropertyID is null
        begin
			set @PropertyIDStr = 'NULL'
		end
		else
		begin
			Set @PropertyIDStr = '''' + CAST(@PropertyID AS NVARCHAR(36)) + ''''
		end
       
        DECLARE @str VARCHAR(MAX)
        SET @str = N'
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
		''' + CAST(@ItemID AS NVARCHAR(36))  + ''',''' +
		CAST(@SourceID AS NVARCHAR(36)) + ''',' +
		@PropertyIDStr + ',''' +
		CAST(@StartTime AS NVARCHAR(max)) + ''',' +
		@IntervalStr + ',''' +
		CAST(@Latitude AS NVARCHAR(max)) + ''',''' +
		CAST(@Longitude AS NVARCHAR(max)) + ''',''' +
		CAST(@Value AS NVARCHAR(max)) + '''
	)'
		
        SELECT  @str = REPLACE(@str, 'ReplaceMe', @tableName)
        PRINT @str
        EXEC(@str)

    END
    

GO
