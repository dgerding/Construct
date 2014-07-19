
/****** Object:  StoredProcedure [dbo].[GetAllPropertyValues]    Script Date: 07/18/2014 10:58:31 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllPropertyValues]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAllPropertyValues]
GO


/****** Object:  StoredProcedure [dbo].[GetAllPropertyValues]    Script Date: 07/18/2014 10:58:31 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
	CREATE PROCEDURE [dbo].[GetAllPropertyValues]
	@datatypeName nvarchar(max), 
	@propertyName nvarchar(max)
AS

	SET NOCOUNT ON;
	
	DECLARE @tableName nvarchar(MAX) = 'z_' + @datatypeName + '_' + @propertyName
	print @tableName

begin
IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @tableName)) 
	begin
		declare @str varchar(max)
		set @str='select * from [dbo].[ReplaceMe]'
		select @str=replace(@str,'ReplaceMe',@tableName)
		exec(@str)
	end

end

GO

