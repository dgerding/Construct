
/****** Object:  StoredProcedure [dbo].[CreateTestItem_zTableData]    Script Date: 07/18/2014 10:58:06 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CreateTestItem_zTableData]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[CreateTestItem_zTableData]
GO


/****** Object:  StoredProcedure [dbo].[CreateTestItem_zTableData]    Script Date: 07/18/2014 10:58:06 ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER OFF
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[CreateTestItem_zTableData]

AS
BEGIN
	SET NOCOUNT ON;

    -- Insert statements for procedure here

END

GO

