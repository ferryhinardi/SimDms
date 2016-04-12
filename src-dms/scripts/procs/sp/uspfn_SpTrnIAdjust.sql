
/****** Object:  StoredProcedure [dbo].[uspfn_SpTrnIAdjust]    Script Date: 6/19/2014 10:47:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[uspfn_SpTrnIAdjust] (  @CompanyCode varchar(10) ,@BranchCode varchar(10), @TypeOfGoods varchar(15))
as
SELECT c.AdjustmentNo, c.AdjustmentDate, c.ReferenceNo, 
c.ReferenceDate, c.TypeOfGoods
FROM SpTrnIAdjustHdr c with(nolock,nowait)
WHERE c.CompanyCode = @CompanyCode
AND c.BranchCode = @BranchCode
AND c.TypeOfGoods = @TypeOfGoods
AND c.Status in ('0','1')
ORDER BY c.AdjustmentNo DESC