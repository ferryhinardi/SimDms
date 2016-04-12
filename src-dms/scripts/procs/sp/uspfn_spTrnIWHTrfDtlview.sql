
 SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
  alter PROCEDURE [dbo].[uspfn_spTrnIWHTrfDtlview]
@CompanyCode varchar(15),
@BranchCode varchar(15),
@WHTrfNo  varchar(25) 
 
AS
select  a.PartNo ,b.PartName,a.FromWarehouseCode  ,d.LookUpValueName [FromWarehouseName], a.ToWarehouseCode ,c.LookUpValueName [ToWarehouseName],a.ReasonCode ,a.Qty
  from spTrnIWHTrfDtl a
inner join spMstItemInfo b

on a.PartNo=b.PartNo
inner join  gnMstLookUpDtl AS d
ON a.CompanyCode = d.CompanyCode 
						 AND a.FromWarehouseCode  = d.LookUpValue
inner join gnMstLookUpDtl c  ON a.CompanyCode = c.CompanyCode 
						 AND a.ToWarehouseCode  = c.LookUpValue
where  c.CodeID='WRCD' and  d.CodeID='WRCD' and a.CompanyCode=@CompanyCode and a.WHTrfNo =@WHTrfNo   and a.BranchCode=@BranchCode