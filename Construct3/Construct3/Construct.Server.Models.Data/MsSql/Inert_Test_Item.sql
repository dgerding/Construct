
/****** Object:  StoredProcedure [dbo].[Insert_Test_Item]    Script Date: 07/18/2014 10:59:41 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Insert_Test_Item]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Insert_Test_Item]
GO


/****** Object:  StoredProcedure [dbo].[Insert_Test_Item]    Script Date: 07/18/2014 10:59:41 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Insert_Test_Item] 
	-- Add the parameters for the stored procedure here
	@ItemID uniqueidentifier = '00000000-0000-0000-0000-000000000000',
	@SourceID uniqueidentifier = '00000000-0000-0000-0000-000000000000',
	@StartTime datetime2 = '00:00:00',
	@Latitude nvarchar(max) = '0.0',
	@Longitude nvarchar(max) = '0.0',
	
	@doubleOne float = 0.0,
	@intOne int = 0,
	@name nvarchar(max) = '',
	@TheBool bit = false,
	@TheBytes varbinary(max) = 0,
	@TheGuid uniqueidentifier = '00000000-0000-0000-0000-000000000000',
	@TheInt int = 0,
	@TheString nvarchar(max) = ''

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	INSERT INTO [dbo].[z_Test_doubleOne]([ItemID] ,
										[SourceID] ,
										[PropertyID] ,
										[Interval] ,
										[StartTime],
										[Value],
										[Latitude],
										[Longitude]
										)
										VALUES
										(@ItemID, @SourceID, 'D7535E8F-A3B3-43CA-B6D5-EEAD354B0B8A', null, @StartTime, 
										  @doubleOne, @Latitude, @Longitude)


	INSERT INTO [dbo].[z_Test_intOne]([ItemID] ,
										[SourceID] ,
										[PropertyID] ,
										[Interval] ,
										[StartTime],
										[Value],
										[Latitude],
										[Longitude]
										)
										VALUES
										(@ItemID, @SourceID, 'B919F2B4-1BFC-4A2E-A9E6-41B692F31307', null, @StartTime, 
									   @intOne, @Latitude, @Longitude)
	
	INSERT INTO [dbo].[z_Test_name]([ItemID] ,
										[SourceID] ,
										[PropertyID] ,
										[Interval] ,
										[StartTime],
										[Value],
										[Latitude],
										[Longitude]
										)
										VALUES(
										@ItemID, @SourceID, '3994D895-998D-49F6-A0AB-29D1A3026838', null, @StartTime, 
										@name, @Latitude, @Longitude)
	
	INSERT INTO [dbo].[z_Test_TheBool]([ItemID] ,
										[SourceID] ,
										[PropertyID] ,
										[Interval] ,
										[StartTime],
										[Value],
										[Latitude],
										[Longitude])
										VALUES
										(@ItemID, @SourceID, '388A0BC9-6010-40BD-BAD0-ACCDAFE35912', null, @StartTime, 
										@TheBool, @Latitude, @Longitude)
	
	INSERT INTO [dbo].[z_Test_TheBytes]([ItemID] ,
										[SourceID] ,
										[PropertyID] ,
										[Interval] ,
										[StartTime],
										[Value],
										[Latitude],
										[Longitude])
										VALUES
										(@ItemID, @SourceID, '715E510E-D7E9-439E-BF6A-7B4F213CA71C', null, @StartTime, 
										 @TheBytes, @Latitude, @Longitude)

	INSERT INTO [dbo].[z_Test_TheGuid]([ItemID] ,
										[SourceID] ,
										[PropertyID] ,
										[Interval] ,
										[StartTime],
										[Value],
										[Latitude],
										[Longitude])
										VALUES
										(@ItemID, @SourceID, '12E166B9-8DB7-4EF6-BAC7-F86ECABD4843', null, @StartTime, 
										@TheGuid, @Latitude, @Longitude)
	
	INSERT INTO [dbo].[z_Test_TheInt]([ItemID] ,
										[SourceID] ,
										[PropertyID] ,
										[Interval] ,
										[StartTime],
										[Value],
										[Latitude],
										[Longitude])
										VALUES
										(@ItemID, @SourceID, '14F0E115-3EF3-4D59-882C-5E622B8849DB', null, @StartTime, 
									   @TheInt, @Latitude, @Longitude)
	
	INSERT INTO [dbo].[z_Test_TheString]([ItemID] ,
										[SourceID] ,
										[PropertyID] ,
										[Interval] ,
										[StartTime],
										[Value],
										[Latitude],
										[Longitude])
										VALUES
										(@ItemID, @SourceID, '92EBA461-1872-4504-A26E-A46C292AB623', null, @StartTime, 
										  @TheString, @Latitude, @Longitude)

END

GO

