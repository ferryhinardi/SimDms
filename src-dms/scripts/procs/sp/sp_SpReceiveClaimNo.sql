

/****** Object:  StoredProcedure [dbo].[sp_SpReceiveClaimNo]    Script Date: 07/03/2014 14:13:51 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE procedure [dbo].[sp_SpReceiveClaimNo] (  

@CompanyCode varchar(10) ,
@BranchCode varchar(10),
@TypeOfGoods varchar(10))

as
SELECT DISTINCT a.CompanyCode, a.BranchCode, a.ClaimNo
,a.ClaimDate , TypeOfGoods            
FROM spTrnPClaimHdr a
INNER JOIN spTrnPClaimDtl b ON b.CompanyCode = a.CompanyCode
AND b.BranchCode = a.BranchCode
AND b.ClaimNo = a.ClaimNo
WHERE a.CompanyCode = @CompanyCode
AND a.BranchCode = @BranchCode   
AND a.Status = '2'
AND a.TypeOfGoods = @TypeOfGoods
AND 
(SELECT ISNULL(SUM(OvertageQty) + SUM(ShortageQty) + SUM(DemageQty) + SUM(WrongQty),0) FROM spTrnPClaimDtl 
WHERE CompanyCode =  @CompanyCode
AND BranchCode = @BranchCode   
AND ClaimNo = a.ClaimNo) >
(SELECT  ISNULL(SUM(c.RcvOvertageQty) + SUM(c.RcvShortageQty) + SUM(c.RcvDamageQty) + SUM(c.RcvWrongQty),0) FROM spTrnPRcvClaimDtl c
INNER JOIN spTrnPRcvClaimHdr b ON c.CompanyCode = b.CompanyCode AND c.BranchCode = b.BranchCode AND c.ClaimNo = b.ClaimNo AND c.ClaimReceivedNo = b.ClaimReceivedNo
WHERE c.CompanyCode =  @CompanyCode
AND c.BranchCode = @BranchCode   
AND c.ClaimNo = a.ClaimNo AND b.Status NOT IN (3))
ORDER BY a.ClaimNo DESC


GO


