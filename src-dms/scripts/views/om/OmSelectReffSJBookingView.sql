USE DMS_V2

/****** Object:  View [dbo].[OmSelectReffSJBookingView]    Script Date: 02/16/2015 11:02:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER VIEW [dbo].[OmSelectReffSJBookingView]
AS 
	SELECT a.CompanyCode, a.BranchCode, a.BatchNo, b.DONo, b.DODate, b.SJNo, b.SJDate, b.SKPNo, a.DealerCode, 
		(select SupplierName from gnMstSupplier where supplierCode = c.supplierCode) as DealerName,
		c.PONo, c.ShipTo, c.SupplierCode
	FROM OmUtlSSJALHdr a
		inner join OmUtlSSJALDtl1 b on a.CompanyCode=b.CompanyCode and a.BranchCode=b.BranchCode
			and a.BatchNo=b.BatchNo
		inner join omTrPurchasePO c on a.CompanyCode=c.CompanyCode and a.BranchCode=c.BranchCode
			and c.RefferenceNo = b.SKPNo
	WHERE 1=1
		--and a.CompanyCode= convert(varchar,@CompanyCode)
		--and a.BranchCode= convert(varchar,@BranchCode)
		--and a.RcvDealerCode = convert(varchar,@CompanyCode)
		and c.status = '2'
		and b.Status = '0'
		and b.IsBlokir = 'Y'
		and not exists (
			select 1 from omTrPurchaseBPU 
			where 1=1
				and CompanyCode = a.CompanyCode and BranchCode = a.BranchCode
				and RefferenceDONo= b.DONO
				and RefferenceSJNo= b.SJNO
				and Status <> '3'
		)
GO


