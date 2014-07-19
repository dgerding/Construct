

/****** Object:  StoredProcedure [dbo].[CreateInt32PropertyValueTable]    Script Date: 07/18/2014 10:56:39 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CreateInt32PropertyValueTable]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[CreateInt32PropertyValueTable]
GO


/****** Object:  StoredProcedure [dbo].[CreateInt32PropertyValueTable]    Script Date: 07/18/2014 10:56:39 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
	CREATE PROCEDURE [dbo].[CreateInt32PropertyValueTable]
    @datatypeName nvarchar(max), 
    @propertyName nvarchar(max)
AS

	SET NOCOUNT ON;
	
	DECLARE @tableName nvarchar(MAX) = 'z_' + @datatypeName + '_' + @propertyName

begin
IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @tableName)) 
BEGIN
  print 'Table already exists.'
END
else
begin
	declare @str varchar(max)
	set @str='CREATE TABLE [dbo].[ReplaceMe](
	[ItemID] [uniqueidentifier] NOT NULL,
	[SourceID] [uniqueidentifier] NOT NULL,
	[PropertyID] [uniqueidentifier] NOT NULL,
	[Interval] [int] NULL,
	[StartTime] [datetime2] NOT NULL,
	[Value] [int] NULL,
	[Latitude] [nvarchar](50) NOT NULL,
	[Longitude] [nvarchar](50) NOT NULL
) ON [PRIMARY]'
	select @str=replace(@str,'ReplaceMe',@tableName)
	exec(@str)
	print 'Table not found'
END
end

GO
