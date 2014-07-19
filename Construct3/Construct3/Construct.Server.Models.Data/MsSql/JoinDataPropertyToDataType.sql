
/****** Object:  StoredProcedure [dbo].[JoinDataPropertyToDataType]    Script Date: 07/18/2014 11:00:02 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[JoinDataPropertyToDataType]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[JoinDataPropertyToDataType]
GO


/****** Object:  StoredProcedure [dbo].[JoinDataPropertyToDataType]    Script Date: 07/18/2014 11:00:02 ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER OFF
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[JoinDataPropertyToDataType]
	@typeID UNIQUEIDENTIFIER,
	@propertyID UNIQUEIDENTIFIER
AS
BEGIN
	INSERT INTO dbo.Types_TypeProperty
	        ( DataTypeID, PropertyID )
	VALUES  ( @typeID, -- TypeID - uniqueidentifier
	          @propertyID -- PropertyID - uniqueidentifier
	          )
END

GO

