
/****** Object:  StoredProcedure [dbo].[GetAllPropertyValuesAfter]    Script Date: 07/18/2014 10:59:13 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllPropertyValuesBetween]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAllPropertyValuesBetween]
GO


/****** Object:  StoredProcedure [dbo].[GetAllPropertyValuesAfter]    Script Date: 07/18/2014 10:59:13 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
	CREATE PROCEDURE [dbo].[GetAllPropertyValuesBetween]
	@datatypeName nvarchar(max), 
	@propertyName nvarchar(max),
	@startTime datetime2,
	@endTime datetime2
AS

	SET NOCOUNT ON;
	
	DECLARE @tableName nvarchar(MAX) = 'z_' + @datatypeName + '_' + @propertyName
	print @tableName

begin
IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @tableName)) 
	begin
		declare @str varchar(max) = 'select * from [dbo].[' + @tableName + '] where StartTime BETWEEN ''' +
										CAST(@startTime as nvarchar(max)) + ''' AND ''' + CAST(@endTime as nvarchar(max)) + ''' order by StartTime asc'
		exec(@str)
	end

end


GO

