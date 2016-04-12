alter procedure [dbo].[usprpt_spAosLogWarning]
	   @CompanyCode varchar(15)
     , @BranchCode varchar(15)
	 , @ProdType varchar(2)
	 , @Year int
	 , @Month int
	 , @PosNo varchar(15)
as

--exec usprpt_spAosLog '6006406', '6006401', '4W', 2016, 1

--declare @CompanyCode varchar(15)
--      , @BranchCode varchar(15)
--	  , @ProdType varchar(2)
--	  , @Year int
--	  , @Month int
--, @PosNo varchar(15)

--select @CompanyCode = '', @BranchCode = '', @ProdType = '4W', @Year = 2016, @Month = 1, , @PosNo = ''
if @ProdType = '4W' begin
	select 
		   row_number() over(order by a.CompanyCode, a.BranchCode, a.POSNO) No
		 , a.POSDate
		 , d.DealerAbbreviation as Dealer
		 , e.OutletAbbreviation as Outlet
		 , a.POSNo
		 , b.PartNo
		 , c.PartName
		 , b.OrderQty
		 , case a.isGenPORDD when 0 then 'NOT SEND' when 1 then 'SEND' end as [Status]
	from spTrnPPOSHdr a
	inner join spTrnPPOSDtl b on a.CompanyCode = b.CompanyCode and a.BranchCode = b.BranchCode and a.POSNo = b.POSNo
	inner join spMstItemInfo c on a.CompanyCode = c.CompanyCode and b.PartNo = c.PartNo
	inner join gnMstDealerMapping d on a.CompanyCode = d.DealerCode
	inner join gnMstDealerOutletMapping e on a.CompanyCode = e.DealerCode and a.BranchCode = e.OutletCode
	where a.CompanyCode = case @CompanyCode when '' then a.CompanyCode else @CompanyCode end
	and a.BranchCode = case @BranchCode when '' then a.BranchCode else @BranchCode end
	and year(a.POSDate) = @Year
	and month(a.POSDate) = @Month
	and a.OrderType = '8'
	and a.ProductType = @ProdType
	and a.IsDeleted = 0
	and a.POSNo = case @PosNo when '' then a.POSNo else @PosNo end
end
else if @ProdType = '2W' begin
	select 
		   row_number() over(order by a.CompanyCode, a.BranchCode, a.POSNO) No
		 , a.POSDate
		 , d.DealerAbbreviation as Dealer
		 , e.OutletAbbreviation as Outlet
		 , a.POSNo
		 , b.PartNo
		 , c.PartName
		 , b.OrderQty
		 , case a.isGenPORDD when 0 then 'NOT SEND' when 1 then 'SEND' end as [Status]
	from SimDmsR2..spTrnPPOSHdr a
	inner join SimDmsR2..spTrnPPOSDtl b on a.CompanyCode = b.CompanyCode and a.BranchCode = b.BranchCode and a.POSNo = b.POSNo
	inner join SimDmsR2..spMstItemInfo c on a.CompanyCode = c.CompanyCode and b.PartNo = c.PartNo
	inner join SimDmsR2..gnMstDealerMapping d on a.CompanyCode = d.DealerCode
	inner join SimDmsR2..gnMstDealerOutletMapping e on a.CompanyCode = e.DealerCode and a.BranchCode = e.OutletCode
	where a.CompanyCode = case @CompanyCode when '' then a.CompanyCode else @CompanyCode end
	and a.BranchCode = case @BranchCode when '' then a.BranchCode else @BranchCode end
	and year(a.POSDate) = @Year
	and month(a.POSDate) = @Month
	and a.OrderType = '8'
	and a.ProductType = @ProdType
	and a.IsDeleted = 0
	and a.POSNo = case @PosNo when '' then a.POSNo else @PosNo end
end


