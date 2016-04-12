CREATE VIEW OmSelectReffNoView
AS
	select a.*,b.DONo,b.DODate,b.SKPNo,c.PONo,c.PODate,c.RefferenceNo,c.RefferenceDate
		,c.SupplierCode,c.BillTo,c.ShipTo,c.Remark
		,(select z.SupplierName from GnMstSupplier z where a.DealerCode = z.SupplierCode) as DealerName
	from OmUtlSDORDHdr a
		inner join omUtlSDORDDtl1 b on a.CompanyCode=b.CompanyCode and a.BranchCode=b.BranchCode
			and a.BatchNo=b.BatchNo
		inner join omTrPurchasePO c on a.CompanyCode=c.CompanyCode and a.BranchCode=c.BranchCode
			and c.RefferenceNo = b.SKPNo
	where 1=1 
		--and a.CompanyCode = convert(varchar,@CompanyCode)
		--and a.BranchCode = convert(varchar,@BranchCode)
	--	and a.RcvDealerCode = convert(varchar,@CompanyCode)
		and c.Status = '2'
		and b.Status = '0'
		and not exists (
			select 1 from omTrPurchaseBPU d 
			where d.CompanyCode = a.CompanyCode and d.BranchCode = a.BranchCode 
				and d.RefferenceDONo=b.DONo
				and d.Status <> '3'
		)