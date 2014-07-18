
/****** Object:  StoredProcedure [dbo].[GetTypes]    Script Date: 07/18/2014 10:59:23 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetTypes]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetTypes]
GO


/****** Object:  StoredProcedure [dbo].[GetTypes]    Script Date: 07/18/2014 10:59:23 ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER OFF
GO

CREATE procedure [dbo].[GetTypes]
as
begin
	select * from vwTypes
end

GO

