IF object_id('uspfn_OmInquirySalesWeb') IS NOT NULL
	DROP PROCEDURE uspfn_OmInquirySalesWeb
GO
-- Inquiry Sales for flexible report
-- uspfn_OmInquirySales '2012-1-1','2013-12-31','CABANG','6006406','6006406','CECEP CAHYADI','IVAN RIKI SAEFUL','','',''
-- uspfn_OmInquirySales '2013-2-1','2013-2-12','','','','','','','','WHOLESALE'
-- uspfn_OmInquirySales '2013-2-1','2013-2-12','','','','','','','','SALES'
-- uspfn_OmInquirySales '2013-6-1','2013-6-27','','','','','','','','FPOL'
CREATE procedure [dbo].[uspfn_OmInquirySalesWeb]
	@StartDate Datetime,
	@EndDate Datetime,
	@Area varchar(100),
	@Dealer varchar(100),
	@Outlet varchar(100),
	@BranchHead varchar(100),
	@SalesHead varchar(100),
	@SalesCoordinator varchar(100),
	@Salesman varchar(100),
	@SalesType varchar(25)
as 
declare @National varchar(10);
set @National = (select top 1 ISNULL(ParaValue,0) from gnMstLookUpDtl
                  where CodeID='QSLS' and LookUpValue='NATIONAL')

if @National <> '1'
begin

	if (@SalesType = 'SALES' or @SalesType = 'WHOLESALE' or @SalesType = '')
	begin
		select map.GroupNo 
			, inq.Area
			, inq.CompanyCode
			, inq.CompanyName
			, inq.BranchCode
			, inq.BranchName
			, inq.BranchHeadID
			, inq.BranchHeadName
			, inq.SalesHeadID
			, inq.SalesHeadName
			, inq.SalesCoordinatorID
			, inq.SalesCoordinatorName
			, inq.SalesmanID
			, inq.SalesmanName
			, inq.ModelCatagory
			, inq.SalesType
			, inq.InvoiceNo
			, inq.InvoiceDate
			, substring(inq.SONo,1,13) SoNo
			, inq.SalesModelCode
			, inq.SalesModelYear
			, inq.SalesModelDesc
			, inq.FakturPolisiNo
			, inq.FakturPolisiDate
			, inq.FakturPolisiDesc
			, inq.MarketModel
			, inq.ColourCode
			, inq.ColourName
			, inq.GroupMarketModel
			, inq.ColumnMarketModel
			, inq.JoinDate
			, inq.ResignDate
			, inq.GradeDate
			, case when isnull(grdDtl.LookUpValueName,'') <> '' then grdDtl.LookUpValueName else inq.Grade end Grade
			, inq.ChassisCode  
			, inq.ChassisNo  
			, inq.EngineCode  
			, inq.EngineNo  
			, inq.COGS
			, inq.BeforeDiscDPP
			, inq.DiscExcludePPn
			, inq.DiscIncludePPn
			, inq.AfterDiscDPP
			, inq.AfterDiscPPn
			, inq.AfterDiscPPnBM
			, inq.AfterDiscTotal
			, inq.PPnBMPaid
			, inq.OthersDPP
			, inq.OthersPPn
			, inq.ShipAmt
			, inq.DepositAmt
			, inq.OthersAmt
			from omHstInquirySales inq with (nolock, nowait)
			Left Join GnMstLookUpDtl grdDtl on inq.CompanyCode = grdDtl.CompanyCode
				and grdDtl.CodeID = 'ITSG'
				and grdDtl.LookUpValue = inq.Grade
			left join gnMstDealerMapping map
				on map.DealerCode = inq.CompanyCode
			where convert(varchar, inq.InvoiceDate, 112) between convert(varchar, @StartDate, 112) and convert(varchar, @EndDate, 112)
			and isnull(inq.CompanyCode,'') like case when @Dealer <> '' then @Dealer else '%%' end
			and isnull(inq.BranchCode,'') like case when @Outlet <> '' then @Outlet else '%%' end 
			--and isnull(inq.Area,'') like case when @Area <> '' then @Area else '%%' end
			and (inq.Area like Case when @Area <> '' 
								then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
										then 'JABODETABEK'
										else @Area end
								else '%%' end
			or inq.Area like Case when @Area <> '' 
								then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
										then 'CABANG'
										else @Area end
								else '%%' end) 
			and isnull(inq.BranchHeadID,'') like case when @BranchHead <> '' then @BranchHead else '%%' end  
			and isnull(inq.SalesHeadID,'') like case when @SalesHead <> '' then @SalesHead else '%%' end   
			and isnull(inq.SalesCoordinatorID,'') like case when @SalesCoordinator <> '' then @SalesCoordinator else '%%' end    
			and isnull(inq.SalesmanID,'') like case when @Salesman <> '' then @Salesman else '%%' end   
			and isnull(inq.MarketModel,'') <> ''
			and inq.Status = '1'
			and map.isActive = 1
			order by map.GroupNo,CompanyCode, inq.BranchCode, inq.BranchHeadName, inq.SalesHeadName, inq.SalesCoordinatorName, inq.SalesmanName, inq.MarketModel
	end

	if @SalesType = 'RETAIL'
	BEGIN
			select map.GroupNo 
			, inq.Area
			, inq.CompanyCode
			, inq.CompanyName
			, inq.BranchCode
			, inq.BranchName
			, inq.BranchHeadID
			, inq.BranchHeadName
			, inq.SalesHeadID
			, inq.SalesHeadName
			, inq.SalesCoordinatorID
			, inq.SalesCoordinatorName
			, inq.SalesmanID
			, inq.SalesmanName
			, inq.ModelCatagory
			, inq.SalesType
			, inq.InvoiceNo
			, inq.InvoiceDate
			, substring(inq.SONo,1,13) SoNo
			, inq.SalesModelCode
			, inq.SalesModelYear
			, inq.SalesModelDesc
			, inq.FakturPolisiNo
			, inq.FakturPolisiDate
			, inq.FakturPolisiDesc
			, inq.MarketModel
			, inq.ColourCode
			, inq.ColourName
			, inq.GroupMarketModel
			, inq.ColumnMarketModel
			, inq.JoinDate
			, inq.ResignDate
			, inq.GradeDate
			, case when isnull(grdDtl.LookUpValueName,'') <> '' then grdDtl.LookUpValueName else inq.Grade end Grade
			, inq.ChassisCode  
			, inq.ChassisNo  
			, inq.EngineCode  
			, inq.EngineNo  
			, inq.COGS
			, inq.BeforeDiscDPP
			, inq.DiscExcludePPn
			, inq.DiscIncludePPn
			, inq.AfterDiscDPP
			, inq.AfterDiscPPn
			, inq.AfterDiscPPnBM
			, inq.AfterDiscTotal
			, inq.PPnBMPaid
			, inq.OthersDPP
			, inq.OthersPPn
			, inq.ShipAmt
			, inq.DepositAmt
			, inq.OthersAmt
			from omHstInquirySales inq with (nolock, nowait)
			Left Join GnMstLookUpDtl grdDtl on inq.CompanyCode = grdDtl.CompanyCode
				and grdDtl.CodeID = 'ITSG'
				and grdDtl.LookUpValue = inq.Grade
			left join gnMstDealerMapping map
				on map.DealerCode = inq.CompanyCode
			where convert(varchar, inq.InvoiceDate, 112) between convert(varchar, @StartDate, 112) and convert(varchar, @EndDate, 112)
			and isnull(inq.CompanyCode,'') like case when @Dealer <> '' then @Dealer else '%%' end
			and isnull(inq.BranchCode,'') like case when @Outlet <> '' then @Outlet else '%%' end 
			--and isnull(inq.Area,'') like case when @Area <> '' then @Area else '%%' end
			and (inq.Area like Case when @Area <> '' 
								then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
										then 'JABODETABEK'
										else @Area end
								else '%%' end
			or inq.Area like Case when @Area <> '' 
								then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
										then 'CABANG'
										else @Area end
								else '%%' end) 			
			and isnull(inq.BranchHeadID,'') like case when @BranchHead <> '' then @BranchHead else '%%' end  
			and isnull(inq.SalesHeadID,'') like case when @SalesHead <> '' then @SalesHead else '%%' end   
			and isnull(inq.SalesCoordinatorID,'') like case when @SalesCoordinator <> '' then @SalesCoordinator else '%%' end    
			and isnull(inq.SalesmanID,'') like case when @Salesman <> '' then @Salesman else '%%' end   
			and isnull(inq.MarketModel,'') <> ''
			and inq.Status = '1'
			and (inq.SoNo not like '%MD' and inq.SoNo not like '%SD') 
			and map.isActive = 1
			order by map.GroupNo,CompanyCode, inq.BranchCode, inq.BranchHeadName, inq.SalesHeadName, inq.SalesCoordinatorName, inq.SalesmanName, inq.MarketModel
	END
	
	IF @SalesType = 'FPOL'
	BEGIN
	print('Masuk')
	select map.GroupNo 
			, inq.Area
			, inq.CompanyCode
			, inq.CompanyName
			, inq.BranchCode
			, inq.BranchName
			, inq.BranchHeadID
			, inq.BranchHeadName
			, inq.SalesHeadID
			, inq.SalesHeadName
			, inq.SalesCoordinatorID
			, inq.SalesCoordinatorName
			, inq.SalesmanID
			, inq.SalesmanName
			, inq.ModelCatagory
			, inq.SalesType
			, inq.InvoiceNo
			, inq.InvoiceDate
			, substring(inq.SONo,1,13) SoNo
			, inq.SalesModelCode
			, inq.SalesModelYear
			, inq.SalesModelDesc
			, inq.FakturPolisiNo
			, inq.FakturPolisiDate
			, inq.FakturPolisiDesc
			, inq.MarketModel
			, inq.ColourCode
			, inq.ColourName
			, inq.GroupMarketModel
			, inq.ColumnMarketModel
			, inq.JoinDate
			, inq.ResignDate
			, inq.GradeDate
			, case when isnull(grdDtl.LookUpValueName,'') <> '' then grdDtl.LookUpValueName else inq.Grade end Grade
			, inq.ChassisCode  
			, inq.ChassisNo  
			, inq.EngineCode  
			, inq.EngineNo  
			, inq.COGS
			, inq.BeforeDiscDPP
			, inq.DiscExcludePPn
			, inq.DiscIncludePPn
			, inq.AfterDiscDPP
			, inq.AfterDiscPPn
			, inq.AfterDiscPPnBM
			, inq.AfterDiscTotal
			, inq.PPnBMPaid
			, inq.OthersDPP
			, inq.OthersPPn
			, inq.ShipAmt
			, inq.DepositAmt
			, inq.OthersAmt
			from omHstInquirySales inq with (nolock, nowait)
			Left Join GnMstLookUpDtl grdDtl on inq.CompanyCode = grdDtl.CompanyCode
				and grdDtl.CodeID = 'ITSG'
				and grdDtl.LookUpValue = inq.Grade
			left join gnMstDealerMapping map
				on map.DealerCode = inq.CompanyCode
			where convert(varchar, inq.SuzukiFPolDate, 112) between convert(varchar, @StartDate, 112) and convert(varchar, @EndDate, 112)
			and isnull(inq.CompanyCode,'') like case when @Dealer <> '' then @Dealer else '%%' end
			and isnull(inq.BranchCode,'') like case when @Outlet <> '' then @Outlet else '%%' end 
			--and isnull(inq.Area,'') like case when @Area <> '' then @Area else '%%' end
			and (inq.Area like Case when @Area <> '' 
								then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
										then 'JABODETABEK'
										else @Area end
								else '%%' end
			or inq.Area like Case when @Area <> '' 
								then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
										then 'CABANG'
										else @Area end
								else '%%' end) 			
			and isnull(inq.BranchHeadID,'') like case when @BranchHead <> '' then @BranchHead else '%%' end  
			and isnull(inq.SalesHeadID,'') like case when @SalesHead <> '' then @SalesHead else '%%' end   
			and isnull(inq.SalesCoordinatorID,'') like case when @SalesCoordinator <> '' then @SalesCoordinator else '%%' end    
			and isnull(inq.SalesmanID,'') like case when @Salesman <> '' then @Salesman else '%%' end   
			and isnull(inq.MarketModel,'') <> ''
			and inq.Status = '1'
			and map.isActive = 1
			order by map.GroupNo,CompanyCode, inq.BranchCode, inq.BranchHeadName, inq.SalesHeadName, inq.SalesCoordinatorName, inq.SalesmanName, inq.MarketModel
	END
end

else

begin

if @SalesType = 'SALES'
begin
select map.GroupNo 
	, inq.Area
	, case when inq.CompanyCode = '6015402' then '6015401' else case when inq.CompanyCode = '6051402' then '6051401' else inq.CompanyCode end end CompanyCode
	, inq.CompanyName
	, inq.BranchCode
	, inq.BranchName
	, inq.BranchHeadID
	, inq.BranchHeadName
	, inq.SalesHeadID
	, inq.SalesHeadName
	, inq.SalesCoordinatorID
	, inq.SalesCoordinatorName
	, inq.SalesmanID
	, inq.SalesmanName
	, inq.ModelCatagory
	, inq.SalesType
	, inq.InvoiceNo
	, inq.SuzukiDoDate InvoiceDate
	, substring(inq.SONo,1,13) SoNo
	, inq.SalesModelCode
	, inq.SalesModelYear
	, inq.SalesModelDesc
	, inq.FakturPolisiNo
	, inq.SuzukiFPolDate FakturPolisiDate
	, inq.FakturPolisiDesc
	, inq.MarketModel
	, inq.ColourCode
	, inq.ColourName
	, inq.GroupMarketModel
	, inq.ColumnMarketModel
	, inq.JoinDate
	, inq.ResignDate
	, inq.GradeDate
	, case when isnull(grdDtl.LookUpValueName,'') <> '' then grdDtl.LookUpValueName else inq.Grade end Grade
	, inq.ChassisCode  
	, inq.ChassisNo  
	, inq.EngineCode  
	, inq.EngineNo  
	, inq.COGS
	, inq.BeforeDiscDPP
	, inq.DiscExcludePPn
	, inq.DiscIncludePPn
	, inq.AfterDiscDPP
	, inq.AfterDiscPPn
	, inq.AfterDiscPPnBM
	, inq.AfterDiscTotal
	, inq.PPnBMPaid
	, inq.OthersDPP
	, inq.OthersPPn
	, inq.ShipAmt
	, inq.DepositAmt
	, inq.OthersAmt
	from omHstInquirySales inq with (nolock, nowait)
	Left Join GnMstLookUpDtl grdDtl on inq.CompanyCode = grdDtl.CompanyCode
		and grdDtl.CodeID = 'ITSG'
		and grdDtl.LookUpValue = inq.Grade
	left join gnMstDealerMapping map
		on map.DealerCode = inq.CompanyCode
	where convert(varchar,inq.SuzukiDODate,112) between convert(varchar,@StartDate,112) and convert(varchar,@EndDate,112)
	and isnull(inq.CompanyCode,'') like case when @Dealer <> '' then @Dealer else '%%' end
	and isnull(inq.BranchCode,'') like case when @Outlet <> '' then @Outlet else '%%' end 
	--and isnull(inq.Area,'') like case when @Area <> '' then @Area else '%%' end
	and (inq.Area like Case when @Area <> '' 
						then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
								then 'JABODETABEK'
								else @Area end
						else '%%' end
	or inq.Area like Case when @Area <> '' 
						then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
								then 'CABANG'
								else @Area end
						else '%%' end) 	
	and isnull(inq.BranchHeadID,'') like case when @BranchHead <> '' then @BranchHead else '%%' end  
	and isnull(inq.SalesHeadID,'') like case when @SalesHead <> '' then @SalesHead else '%%' end   
	and isnull(inq.SalesCoordinatorID,'') like case when @SalesCoordinator <> '' then @SalesCoordinator else '%%' end    
	and isnull(inq.SalesmanID,'') like case when @Salesman <> '' then @Salesman else '%%' end   
	and inq.Status = '1'
	and map.isActive = 1
union all
select c.GroupNo 
	, c.Area
	, case when a.CustomerCode = '6015402' then '6015401' else case when a.CustomerCode = '6051402' then '6051401' else a.CustomerCode end end CompanyCode
	, isnull(c.DealerAbbreviation,a.CustomerCode) CompanyName
	, isnull(d.OutletCode,'HQ') BranchCode
	, isnull(d.OutletAbbreviation,'HQ') BranchName
	, '' BranchHeadID
	, '' BranchHeadName
	, '' SalesHeadID
	, '' SalesHeadName
	, '' SalesCoordinatorID
	, '' SalesCoordinatorName
	, '' SalesmanID
	, '' SalesmanName
	, '' ModelCatagory
	, '' SalesTyper
	, '' InvoiceNo
	, a.DODate InvoiceDate
	, '' SoNo
	, a.SalesModelCode
	, '1900' SalesModelYear
	, b.SalesModelDesc
	, a.FPNo
	, a.PROCESSDATE
	, '' FakturPolisiDesc
	, '' MarketModel
	, '' ColourCode
	, '' ColourName
	, b.GroupMarketModel
	, b.ColumnMarketModel
	, '1900-01-01' JoinDate
	, '1900-01-01' ResignDate
	, '1900-01-01' GradeDate
	, '' Grade
	, a.ChassisCode  
	, a.ChassisNo  
	, a.EngineCode  
	, a.EngineNo  
	, 0 COGS
	, 0 BeforeDiscDPP
	, 0 DiscExcludePPn
	, 0 DiscIncludePPn
	, 0 AfterDiscDPP
	, 0 AfterDiscPPn
	, 0 AfterDiscPPnBM
	, 0 AfterDiscTotal
	, 0 PPnBMPaid
	, 0 OthersDPP
	, 0 OthersPPn
	, 0 ShipAmt
	, 0 DepositAmt
	, 0 OthersAmt
from OmHstInquirySalesNSDS a with (nolock, nowait)
left join OmMstModel b on a.SalesModelCode = b.SalesModelCode
left join GnMstDealerMapping c on  a.CustomerCode = c.DealerCode
left join GnMstDealerOutletMapping d on d.DealerCode = a.CustomerCode
	and d.LastUpdateBy = 'HQ'
where convert(varchar,a.DODATE,112) between convert(varchar,@StartDate,112) and convert(varchar,@EndDate,112)
	and isnull(c.DealerCode,'') like case when isnull(@Dealer,'') <> '' then @Dealer else '%%' end
	and isnull(d.OutletCode,'') like case when isnull(@Outlet,'') <> '' then @Outlet else '%%' end 
	--and isnull(c.Area,'') like case when isnull(@Area,'') <> '' then @Area else '%%' end 
	and (c.Area like Case when @Area <> '' 
						then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
								then 'JABODETABEK'
								else @Area end
						else '%%' end
	or c.Area like Case when @Area <> '' 
						then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
								then 'CABANG'
								else @Area end
						else '%%' end) 
	and a.GroupAreaCode <> '3' 
	and IsExists = 0
	and c.isActive = 1
order by map.GroupNo,CompanyCode, inq.BranchCode, inq.BranchHeadName, inq.SalesHeadName, inq.SalesCoordinatorName, inq.SalesmanName, inq.MarketModel

end

if @SalesType = 'WHOLESALE'
begin	
select map.GroupNo 
	, inq.Area
	, case when inq.CompanyCode = '6015402' then '6015401' else case when inq.CompanyCode = '6051402' then '6051401' else inq.CompanyCode end end CompanyCode
	, inq.CompanyName
	, inq.BranchCode
	, inq.BranchName
	, inq.BranchHeadID
	, inq.BranchHeadName
	, inq.SalesHeadID
	, inq.SalesHeadName
	, inq.SalesCoordinatorID
	, inq.SalesCoordinatorName
	, inq.SalesmanID
	, inq.SalesmanName
	, inq.ModelCatagory
	, inq.SalesType
	, inq.InvoiceNo
	, inq.SuzukiDoDate InvoiceDate
	, substring(inq.SONo,1,13) SoNo
	, inq.SalesModelCode
	, inq.SalesModelYear
	, inq.SalesModelDesc
	, inq.FakturPolisiNo
	, inq.SuzukiFPolDate FakturPolisiDate
	, inq.FakturPolisiDesc
	, inq.MarketModel
	, inq.ColourCode
	, inq.ColourName
	, inq.GroupMarketModel
	, inq.ColumnMarketModel
	, inq.JoinDate
	, inq.ResignDate
	, inq.GradeDate
	, case when isnull(grdDtl.LookUpValueName,'') <> '' then grdDtl.LookUpValueName else inq.Grade end Grade
	, inq.ChassisCode  
	, inq.ChassisNo  
	, inq.EngineCode  
	, inq.EngineNo  
	, inq.COGS
	, inq.BeforeDiscDPP
	, inq.DiscExcludePPn
	, inq.DiscIncludePPn
	, inq.AfterDiscDPP
	, inq.AfterDiscPPn
	, inq.AfterDiscPPnBM
	, inq.AfterDiscTotal
	, inq.PPnBMPaid
	, inq.OthersDPP
	, inq.OthersPPn
	, inq.ShipAmt
	, inq.DepositAmt
	, inq.OthersAmt
	from omHstInquirySales inq with (nolock, nowait)
	Left Join GnMstLookUpDtl grdDtl on inq.CompanyCode = grdDtl.CompanyCode
		and grdDtl.CodeID = 'ITSG'
		and grdDtl.LookUpValue = inq.Grade
	left join gnMstDealerMapping map
		on map.DealerCode = inq.CompanyCode
	where convert(varchar,inq.SuzukiDODate,112) between convert(varchar,@StartDate,112) and convert(varchar,@EndDate,112)
	and isnull(inq.CompanyCode,'') like case when @Dealer <> '' then @Dealer else '%%' end
	and isnull(inq.BranchCode,'') like case when @Outlet <> '' then @Outlet else '%%' end 
	--and isnull(inq.Area,'') like case when @Area <> '' then @Area else '%%' end
	and (inq.Area like Case when @Area <> '' 
						then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
								then 'JABODETABEK'
								else @Area end
						else '%%' end
	or inq.Area like Case when @Area <> '' 
						then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
								then 'CABANG'
								else @Area end
						else '%%' end) 	
	--and isnull(inq.BranchHeadName,'') like case when @BranchHead <> '' then @BranchHead else '%%' end  
	and isnull(inq.BranchHeadID,'') like case when @BranchHead <> '' then @BranchHead else '%%' end  
	and isnull(inq.SalesHeadID,'') like case when @SalesHead <> '' then @SalesHead else '%%' end   
	and isnull(inq.SalesCoordinatorID,'') like case when @SalesCoordinator <> '' then @SalesCoordinator else '%%' end    
	and isnull(inq.SalesmanID,'') like case when @Salesman <> '' then @Salesman else '%%' end   
	and inq.Status = '1'
	and map.isActive = 1
--union all
--select c.GroupNo 
--	, c.Area
--	, case when a.CustomerCode = '6015402' then '6015401' else case when a.CustomerCode = '6051402' then '6051401' else a.CustomerCode end end CompanyCode
--	, isnull(c.DealerAbbreviation,a.CustomerCode) CompanyName
--	, isnull(d.OutletCode,'HQ') BranchCode
--	, isnull(d.OutletAbbreviation,'HQ') BranchName
--	, '' BranchHeadID
--	, '' BranchHeadName
--	, '' SalesHeadID
--	, '' SalesHeadName
--	, '' SalesCoordinatorID
--	, '' SalesCoordinatorName
--	, '' SalesmanID
--	, '' SalesmanName
--	, '' ModelCatagory
--	, '' SalesType
--	, '' InvoiceNo
--	, a.DODate InvoiceDate
--	, '' SoNo
--	, a.SalesModelCode
--	, '1900' SalesModelYear
--	, b.SalesModelDesc
--	, a.FPNo
--	, a.PROCESSDATE
--	, '' FakturPolisiDesc
--	, '' MarketModel
--	, '' ColourCode
--	, '' ColourName
--	, b.GroupMarketModel
--	, b.ColumnMarketModel
--	, '1900-01-01' JoinDate
--	, '1900-01-01' ResignDate
--	, '1900-01-01' GradeDate
--	, '' Grade
--	, a.ChassisCode  
--	, a.ChassisNo  
--	, a.EngineCode  
--	, a.EngineNo  
--	, 0 COGS
--	, 0 BeforeDiscDPP
--	, 0 DiscExcludePPn
--	, 0 DiscIncludePPn
--	, 0 AfterDiscDPP
--	, 0 AfterDiscPPn
--	, 0 AfterDiscPPnBM
--	, 0 AfterDiscTotal
--	, 0 PPnBMPaid
--	, 0 OthersDPP
--	, 0 OthersPPn
--	, 0 ShipAmt
--	, 0 DepositAmt
--	, 0 OthersAmt
--from OmHstInquirySalesNSDS a with (nolock, nowait)
--left join OmMstModel b on a.SalesModelCode = b.SalesModelCode
--left join GnMstDealerMapping c on  a.CustomerCode = c.DealerCode
--left join GnMstDealerOutletMapping d on d.DealerCode = a.CustomerCode
--	and d.LastUpdateBy = 'HQ'
--where convert(varchar,a.DODATE,112) between convert(varchar,@StartDate,112) and convert(varchar,@EndDate,112)
--	and isnull(c.DealerCode,'') like case when @Dealer <> '' then @Dealer else '%%' end
--	and isnull(d.OutletCode,'') like case when @Outlet <> '' then @Outlet else '%%' end 
--	and isnull(c.Area,'') like case when @Area <> '' then @Area else '%%' end
--	and GroupAreaCode <> '3' 
--	and IsExists = 0
--order by map.GroupNo,CompanyCode, inq.BranchCode, inq.BranchHeadName, inq.SalesHeadName, inq.SalesCoordinatorName, inq.SalesmanName, inq.MarketModel
end
if @SalesType = 'RETAIL'
begin
select map.GroupNo 
	, inq.Area
	, case when inq.CompanyCode = '6015402' then '6015401' else case when inq.CompanyCode = '6051402' then '6051401' else inq.CompanyCode end end CompanyCode
	, inq.CompanyName
	, inq.BranchCode
	, inq.BranchName
	, inq.BranchHeadID
	, inq.BranchHeadName
	, inq.SalesHeadID
	, inq.SalesHeadName
	, inq.SalesCoordinatorID
	, inq.SalesCoordinatorName
	, inq.SalesmanID
	, inq.SalesmanName
	, inq.ModelCatagory
	, inq.SalesType
	, inq.InvoiceNo
	, inq.SuzukiDoDate InvoiceDate
	, substring(inq.SONo,1,13) SoNo
	, inq.SalesModelCode
	, inq.SalesModelYear
	, inq.SalesModelDesc
	, inq.FakturPolisiNo
	, inq.SuzukiFPolDate FakturPolisiDate
	, inq.FakturPolisiDesc
	, inq.MarketModel
	, inq.ColourCode
	, inq.ColourName
	, inq.GroupMarketModel
	, inq.ColumnMarketModel
	, inq.JoinDate
	, inq.ResignDate
	, inq.GradeDate
	, case when isnull(grdDtl.LookUpValueName,'') <> '' then grdDtl.LookUpValueName else inq.Grade end Grade
	, inq.ChassisCode  
	, inq.ChassisNo  
	, inq.EngineCode  
	, inq.EngineNo  
	, inq.COGS
	, inq.BeforeDiscDPP
	, inq.DiscExcludePPn
	, inq.DiscIncludePPn
	, inq.AfterDiscDPP
	, inq.AfterDiscPPn
	, inq.AfterDiscPPnBM
	, inq.AfterDiscTotal
	, inq.PPnBMPaid
	, inq.OthersDPP
	, inq.OthersPPn
	, inq.ShipAmt
	, inq.DepositAmt
	, inq.OthersAmt
	from omHstInquirySales inq with (nolock, nowait)
	Left Join GnMstLookUpDtl grdDtl on inq.CompanyCode = grdDtl.CompanyCode
		and grdDtl.CodeID = 'ITSG'
		and grdDtl.LookUpValue = inq.Grade
	left join gnMstDealerMapping map
		on map.DealerCode = inq.CompanyCode
	where convert(varchar,inq.SuzukiDODate,112) between convert(varchar,@StartDate,112) and convert(varchar,@EndDate,112)
	and isnull(inq.CompanyCode,'') like case when @Dealer <> '' then @Dealer else '%%' end
	and isnull(inq.BranchCode,'') like case when @Outlet <> '' then @Outlet else '%%' end 
	--and isnull(inq.Area,'') like case when @Area <> '' then @Area else '%%' end
	and (inq.Area like Case when @Area <> '' 
						then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
								then 'JABODETABEK'
								else @Area end
						else '%%' end
	or inq.Area like Case when @Area <> '' 
						then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
								then 'CABANG'
								else @Area end
						else '%%' end) 	
						--and isnull(inq.BranchHeadName,'') like case when @BranchHead <> '' then @BranchHead else '%%' end  
	and isnull(inq.BranchHeadID,'') like case when @BranchHead <> '' then @BranchHead else '%%' end  
	and isnull(inq.SalesHeadID,'') like case when @SalesHead <> '' then @SalesHead else '%%' end   
	and isnull(inq.SalesCoordinatorID,'') like case when @SalesCoordinator <> '' then @SalesCoordinator else '%%' end    
	and isnull(inq.SalesmanID,'') like case when @Salesman <> '' then @Salesman else '%%' end   
	and isnull(inq.MarketModel,'') <> ''
	and inq.Status = '1'
	and (inq.SoNo not like '%MD' and inq.SoNo not like '%SD') 
	and map.isActive = 1
order by map.GroupNo,inq.CompanyCode, inq.BranchCode, inq.BranchHeadName, inq.SalesHeadName, inq.SalesCoordinatorName, inq.SalesmanName, inq.MarketModel
end
if @SalesType = 'FPOL'
begin
select map.GroupNo 
	, inq.Area
	, case when inq.CompanyCode = '6015402' then '6015401' else case when inq.CompanyCode = '6051402' then '6051401' else inq.CompanyCode end end CompanyCode
	, inq.CompanyName
	, inq.BranchCode
	, inq.BranchName
	, inq.BranchHeadID
	, inq.BranchHeadName
	, inq.SalesHeadID
	, inq.SalesHeadName
	, inq.SalesCoordinatorID
	, inq.SalesCoordinatorName
	, inq.SalesmanID
	, inq.SalesmanName
	, inq.ModelCatagory
	, inq.SalesType
	, inq.InvoiceNo
	, inq.SuzukiFPOLDate InvoiceDate
	, substring(inq.SONo,1,13) SoNo
	, inq.SalesModelCode
	, inq.SalesModelYear
	, inq.SalesModelDesc
	, inq.FakturPolisiNo
	, inq.SuzukiFPolDate FakturPolisiDate
	, inq.FakturPolisiDesc
	, inq.MarketModel
	, inq.ColourCode
	, inq.ColourName
	, inq.GroupMarketModel
	, inq.ColumnMarketModel
	, inq.JoinDate
	, inq.ResignDate
	, inq.GradeDate
	, case when isnull(grdDtl.LookUpValueName,'') <> '' then grdDtl.LookUpValueName else inq.Grade end Grade
	, inq.ChassisCode  
	, inq.ChassisNo  
	, inq.EngineCode  
	, inq.EngineNo  
	, inq.COGS
	, inq.BeforeDiscDPP
	, inq.DiscExcludePPn
	, inq.DiscIncludePPn
	, inq.AfterDiscDPP
	, inq.AfterDiscPPn
	, inq.AfterDiscPPnBM
	, inq.AfterDiscTotal
	, inq.PPnBMPaid
	, inq.OthersDPP
	, inq.OthersPPn
	, inq.ShipAmt
	, inq.DepositAmt
	, inq.OthersAmt
	from omHstInquirySales inq with (nolock, nowait)
	Left Join GnMstLookUpDtl grdDtl on inq.CompanyCode = grdDtl.CompanyCode
		and grdDtl.CodeID = 'ITSG'
		and grdDtl.LookUpValue = inq.Grade
	left join gnMstDealerMapping map
		on map.DealerCode = inq.CompanyCode
	where convert(varchar,inq.SuzukiFPolDate,112) between convert(varchar,@StartDate,112) and convert(varchar,@EndDate,112)
	and isnull(inq.CompanyCode,'') like case when @Dealer <> '' then @Dealer else '%%' end
	and isnull(inq.BranchCode,'') like case when @Outlet <> '' then @Outlet else '%%' end 
	--and isnull(inq.Area,'') like case when @Area <> '' then @Area else '%%' end
	and (inq.Area like Case when @Area <> '' 
						then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
								then 'JABODETABEK'
								else @Area end
						else '%%' end
	or inq.Area like Case when @Area <> '' 
						then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
								then 'CABANG'
								else @Area end
						else '%%' end) 	--and isnull(inq.BranchHeadName,'') like case when @BranchHead <> '' then @BranchHead else '%%' end  
	and isnull(inq.BranchHeadID,'') like case when @BranchHead <> '' then @BranchHead else '%%' end  
	and isnull(inq.SalesHeadID,'') like case when @SalesHead <> '' then @SalesHead else '%%' end   
	and isnull(inq.SalesCoordinatorID,'') like case when @SalesCoordinator <> '' then @SalesCoordinator else '%%' end    
	and isnull(inq.SalesmanID,'') like case when @Salesman <> '' then @Salesman else '%%' end   
	and isnull(inq.MarketModel,'') <> ''
	and inq.Status = '1'
	and map.isActive = 1
--union
--select c.GroupNo 
--	, c.Area
--	, case when a.CustomerCode = '6015402' then '6015401' else case when a.CustomerCode = '6051402' then '6051401' else a.CustomerCode end end CompanyCode
--	, isnull(c.DealerAbbreviation,a.CustomerCode) CompanyName
--	, isnull(d.OutletCode,'HQ') BranchCode
--	, isnull(d.OutletAbbreviation,'HQ') BranchName
--	, '' BranchHeadID
--	, '' BranchHeadName
--	, '' SalesHeadID
--	, '' SalesHeadName
--	, '' SalesCoordinatorID
--	, '' SalesCoordinatorName
--	, '' SalesmanID
--	, '' SalesmanName
--	, '' ModelCatagory
--	, '' SalesType
--	, '' InvoiceNo
--	, a.ProcessDate InvoiceDate
--	, '' SoNo
--	, a.SalesModelCode
--	, '1900' SalesModelYear
--	, b.SalesModelDesc
--	, a.FPNo
--	, a.PROCESSDATE
--	, '' FakturPolisiDesc
--	, '' MarketModel
--	, '' ColourCode
--	, '' ColourName
--	, b.GroupMarketModel
--	, b.ColumnMarketModel
--	, '1900-01-01' JoinDate
--	, '1900-01-01' ResignDate
--	, '1900-01-01' GradeDate
--	, '' Grade
--	, a.ChassisCode  
--	, a.ChassisNo  
--	, a.EngineCode  
--	, a.EngineNo  
--	, 0 COGS
--	, 0 BeforeDiscDPP
--	, 0 DiscExcludePPn
--	, 0 DiscIncludePPn
--	, 0 AfterDiscDPP
--	, 0 AfterDiscPPn
--	, 0 AfterDiscPPnBM
--	, 0 AfterDiscTotal
--	, 0 PPnBMPaid
--	, 0 OthersDPP
--	, 0 OthersPPn
--	, 0 ShipAmt
--	, 0 DepositAmt
--	, 0 OthersAmt
--from OmHstInquirySalesNSDS a with (nolock, nowait)
--left join OmMstModel b on a.SalesModelCode = b.SalesModelCode
--left join GnMstDealerMapping c on  a.CustomerCode = c.DealerCode
--left join GnMstDealerOutletMapping d on d.DealerCode = a.CustomerCode
--	and d.LastUpdateBy = 'HQ'
--where a.ProcessDate between @StartDate and @EndDate
--	and isnull(c.DealerCode,'') like case when @Dealer <> '' then @Dealer else '%%' end
--	and isnull(d.OutletCode,'') like case when @Outlet <> '' then @Outlet else '%%' end 
--	and isnull(c.Area,'') like case when @Area <> '' then @Area else '%%' end 
--	and GroupAreaCode <> '3' 
--	and IsExists = 0
--	and convert(varchar,ProcessDate,112) between convert(varchar,@StartDate,112) and convert(varchar,@EndDate,112)
--	and not exists (select 1 
--					from OmHstInquirySales
--					where convert(varchar,SuzukiFPolDate,112) between convert(varchar,@StartDate,112) and convert(varchar,@EndDate,112)
--						and a.ChassisCode = ChassisCode
--						and a.ChassisNo = ChassisNo
--						and MarketModel is not null
--						and Status = '1')
--order by map.GroupNo,CompanyCode, inq.BranchCode, inq.BranchHeadName, inq.SalesHeadName, inq.SalesCoordinatorName, inq.SalesmanName, inq.MarketModel
end

if @SalesType = ''
begin
select map.GroupNo 
	, inq.Area
	, inq.CompanyCode
	, inq.CompanyName
	, inq.BranchCode
	, inq.BranchName
	, inq.BranchHeadID
	, inq.BranchHeadName
	, inq.SalesHeadID
	, inq.SalesHeadName
	, inq.SalesCoordinatorID
	, inq.SalesCoordinatorName
	, inq.SalesmanID
	, inq.SalesmanName
	, inq.ModelCatagory
	, inq.SalesType
	, inq.InvoiceNo
	, inq.SuzukiDoDate InvoiceDate
	, substring(inq.SONo,1,13) SoNo
	, inq.SalesModelCode
	, inq.SalesModelYear
	, inq.SalesModelDesc
	, inq.FakturPolisiNo
	, inq.SuzukiFPolDate FakturPolisiDate
	, inq.FakturPolisiDesc
	, inq.MarketModel
	, inq.ColourCode
	, inq.ColourName
	, inq.GroupMarketModel
	, inq.ColumnMarketModel
	, inq.JoinDate
	, inq.ResignDate
	, inq.GradeDate
	, case when isnull(grdDtl.LookUpValueName,'') <> '' then grdDtl.LookUpValueName else inq.Grade end Grade
	, inq.ChassisCode  
	, inq.ChassisNo  
	, inq.EngineCode  
	, inq.EngineNo  
	, inq.COGS
	, inq.BeforeDiscDPP
	, inq.DiscExcludePPn
	, inq.DiscIncludePPn
	, inq.AfterDiscDPP
	, inq.AfterDiscPPn
	, inq.AfterDiscPPnBM
	, inq.AfterDiscTotal
	, inq.PPnBMPaid
	, inq.OthersDPP
	, inq.OthersPPn
	, inq.ShipAmt
	, inq.DepositAmt
	, inq.OthersAmt
	from omHstInquirySales inq with (nolock, nowait)
	Left Join GnMstLookUpDtl grdDtl on inq.CompanyCode = grdDtl.CompanyCode
		and grdDtl.CodeID = 'ITSG'
		and grdDtl.LookUpValue = inq.Grade
	left join gnMstDealerMapping map
		on map.DealerCode = inq.CompanyCode
	where convert(varchar,inq.SuzukiDODate,112) between convert(varchar,@StartDate,112) and convert(varchar,@EndDate,112)
	and isnull(inq.CompanyCode,'') like case when @Dealer <> '' then @Dealer else '%%' end
	and isnull(inq.BranchCode,'') like case when @Outlet <> '' then @Outlet else '%%' end 
	--and isnull(inq.Area,'') like case when @Area <> '' then @Area else '%%' end
	and (inq.Area like Case when @Area <> '' 
						then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
								then 'JABODETABEK'
								else @Area end
						else '%%' end
	or inq.Area like Case when @Area <> '' 
						then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
								then 'CABANG'
								else @Area end
						else '%%' end) 	--and isnull(inq.BranchHeadName,'') like case when @BranchHead <> '' then @BranchHead else '%%' end  
	and isnull(inq.BranchHeadID,'') like case when @BranchHead <> '' then @BranchHead else '%%' end  
	and isnull(inq.SalesHeadID,'') like case when @SalesHead <> '' then @SalesHead else '%%' end   
	and isnull(inq.SalesCoordinatorID,'') like case when @SalesCoordinator <> '' then @SalesCoordinator else '%%' end    
	and isnull(inq.SalesmanID,'') like case when @Salesman <> '' then @Salesman else '%%' end   
	and isnull(inq.MarketModel,'') <> ''
	and inq.Status = '1'
	and map.isActive = 1
end

end

-- uspfn_OmInquirySales '6006406','6006406','2012-1-1','2012-4-30','','','','','','',''
--select * from omHstInquirySales where (SoNo like '%SD' or SoNo like '%MD')
--select * from #t1
--select * from #t5
--select * from gnMstEmployee where EmployeeID='00288'
--update gnMstEmployee set PersonnelStatus=1 where EmployeeID='00288'

--select * from pmMstTeamMembers where EmployeeID='S20013'
--select * from pmMstTeamMembers where BranchCode='6006402' and TeamID='108' and IsSupervisor=1USE [DMS_V2]
