
/****** Object:  StoredProcedure [dbo].[AddItemHeader]    Script Date: 07/18/2014 10:51:27 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AddItemHeader]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[AddItemHeader]
GO



/****** Object:  StoredProcedure [dbo].[AddItemHeader]    Script Date: 07/18/2014 10:51:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[AddItemHeader]
		@SourceTime DATETIME,
		@SourceID UNIQUEIDENTIFIER,
		@Latitude NVARCHAR(50),
		@Longitude NVARCHAR(50),
		@DataTypeID UNIQUEIDENTIFIER
		
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    INSERT INTO dbo.Items_Item
            ( SourceTime ,
              SourceId ,
              RecordCreationTime,
              Latitude,
              Longitude,
              DataTypeID
            )
    VALUES  ( @SourceTime, -- SourceTime - datetime2
		      @SourceID,
		      GETDATE(), -- RecordCreationTime - datetime2
              @Latitude , -- Longitude - nvarchar(50)
              @Longitude, -- Latitude - nvarchar(50)
              @DataTypeID -- DataTypeID - uniqueidentifier
            )
END

GO

