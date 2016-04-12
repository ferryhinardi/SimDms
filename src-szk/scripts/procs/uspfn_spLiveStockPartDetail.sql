create proc [dbo].[uspfn_spLiveStockPartDetail] --'95141-57B70L000','%'  95141-57B70L000
(    
 @partno varchar(20),
 @area varchar(100)   
)    
as    
begin    
	select Area, d.OutletName as Dealer , d.OutletAbbreviation as Outlet, 
	case when exists(select * from gnMstLookUpDtl where CompanyCode = '0000000' and CodeID = 'LSTP' and LookUpValue = 'QTY') then 'TERSEDIA' 
	else cast((SUM(OnHand) - (SUM(AllocationSP + AllocationSL + AllocationSR))) as varchar(50)) end as QtyAvail    
	from gnMstDealerMapping a     
	join spMstItems b on a.DealerCode = b.CompanyCode    
	join spMstItemInfo c on b.CompanyCode = c.CompanyCode and b.PartNo = c.PartNo    
	join gnMstDealerOutletMapping d on a.DealerCode = d.DealerCode and b.BranchCode=d.OutletCode
	where b.PartNo = @partno and Area like @area
	group by Area, d.OutletName, d.OutletAbbreviation 
	having (SUM(OnHand) - (SUM(AllocationSP + AllocationSL + AllocationSR))) > 0
end