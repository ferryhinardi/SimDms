CREATE procedure [dbo].[sp_EdpBpsNoBrowse] (  

@CompanyCode varchar(10) ,
@BranchCode varchar(10),
@TypeOfGoods varchar(10),
@ProductType varchar(10),
@CustomerCode varchar(30))


as

SELECT 
 a.BPSFNo
,a.BPSFDate
,a.PickingSlipNo
,a.PickingSlipDate
FROM spTrnSBPSFHdr a
WHERE a.CompanyCode=@CompanyCode
  AND a.BranchCode=@BranchCode
  AND a.CustomerCode=@CustomerCode
  AND a.TypeOfGoods=@TypeOfGoods
GROUP BY  a.BPSFNo, a.BPSFDate, a.PickingSlipNo, a.PickingSlipDate
ORDER BY  a.BPSFNo DESC
GO


