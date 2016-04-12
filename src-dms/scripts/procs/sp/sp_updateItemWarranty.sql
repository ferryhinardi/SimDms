SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		David
-- Create date: 18 March 2016
-- Description:	Update utility4 for AOS | W / null
-- =============================================
CREATE PROCEDURE sp_updateItemWarranty
	-- Add the parameters for the stored procedure here
	@PartNo varchar(15),
	@isWarrantyParts bit
AS
BEGIN
		update spMstItems
		set Utility4 = case when @isWarrantyParts = 1 then 'W' else null end
		where PartNo = @PartNo
END
GO

