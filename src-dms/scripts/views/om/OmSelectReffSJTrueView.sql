/****** Object:  View [dbo].[OmSelectReffSJTrueView]    Script Date: 02/16/2015 10:58:59 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER VIEW [dbo].[OmSelectReffSJTrueView]
AS
SELECT a.CompanyCode, a.BranchCode, a.BatchNo, b.SJNo, b.SJDate, b.SKPNo, b.DONo, a.DealerCode, c.ShipTo, b.DODate, c.PONo,
    (select SupplierName from gnMstSupplier where supplierCode = c.supplierCode) as DealerName, c.SupplierCode, b.FlagRevisi
FROM OmUtlSSJALHdr a
    inner join OmUtlSSJALDtl1 b on a.CompanyCode=b.CompanyCode and a.BranchCode=b.BranchCode
        and a.BatchNo=b.BatchNo
    inner join omTrPurchaseBPU c on a.CompanyCode=c.CompanyCode and a.BranchCode=c.BranchCode
        and c.RefferenceDONo = b.DONo
WHERE 1=1
	-- and a.CompanyCode = convert(varchar,@CompanyCode)
	-- and a.BranchCode = convert(varchar,@BranchCode)
	-- and a.RcvDealerCode = convert(varchar,@CompanyCode)
    and c.status = '2'
    and b.Status = '0'
    and not exists (
        select 1 from omTrPurchaseBPU d 
        where 1=1
            and d.CompanyCode = a.CompanyCode and d.BranchCode = a.BranchCode 
            and d.RefferenceSJNo=b.SJNo
            and d.Status <> '3'
    )
-- order by FlagRevisi Desc,a.BatchNo,b.SJNo,b.SKPNo
GO


