IF object_id('usprpt_OmRpSalRgs039PivotWeb') IS NOT NULL
	DROP PROCEDURE usprpt_OmRpSalRgs039PivotWeb
GO
-- usprpt_OmRpSalRgs039Pivot '2012-10-1','2012-10-31','','','','','','','','SALES'
CREATE procedure [dbo].[usprpt_OmRpSalRgs039PivotWeb]
	@StartDate Datetime,
	@EndDate Datetime,
	@Area varchar(100),
	@Dealer varchar(100),
	@Outlet varchar(100),
	@BranchHead varchar(100),
	@SalesHead varchar(100),
	@SalesCoordinator varchar(100),
	@Salesman varchar(100),
	@SalesType varchar(100)
as 
Declare @MainTable table
(
	GroupNo					varchar(150)
	, Area					varchar(150)
	, CompanyCode			varchar(150)
	, CompanyName			varchar(150)
	, BranchCode			varchar(150)
	, BranchName			varchar(150)
	, BranchHeadID			varchar(150)
	, BranchHeadName		varchar(150)
	, SalesHeadID			varchar(150)
	, SalesHeadName			varchar(150)
	, SalesCoordinatorID	varchar(150)
	, SalesCoordinatorName	varchar(150)
	, SalesmanID			varchar(150)
	, SalesmanName			varchar(150)
	, ModelCatagory			varchar(150)
	, SalesType				varchar(150)
	, InvoiceNo				varchar(150)
	, InvoiceDate			datetime
	, SONo					varchar(150)
	, SalesModelCode		varchar(150)
	, SalesModelYear		numeric(4,0)
	, SalesModelDesc		varchar(150)
	, FakturPolisiNo		varchar(150)
	, FakturPolisiDate		datetime
	, FakturPolisiDesc		varchar(150)
	, MarketModel			varchar(150)
	, ColourCode			varchar(150)
	, ColourName			varchar(150)
	, GroupMarketModel		varchar(150)
	, ColumnMarketModel		varchar(150)
	, JoinDate				datetime
	, ResignDate			datetime
	, GradeDate				datetime
	, Grade					varchar(150)
	, ChassisCode			varchar(150)
	, ChassisNo				varchar(150)
	, EngineCode			varchar(150) 
	, EngineNo				varchar(150)
	, COGS					numeric(18,0)
	, BeforeDiscDPP			numeric(18,0)
	, DiscExcludePPn		numeric(18,0)
	, DiscIncludePPn		numeric(18,0)
	, AfterDiscDPP			numeric(18,0)
	, AfterDiscPPn			numeric(18,0)
	, AfterDiscPPnBM		numeric(18,0)
	, AfterDiscTotal		numeric(18,0)
	, PPnBMPaid				numeric(18,0)
	, OthersDPP				numeric(18,0)
	, OthersPPn				numeric(18,0)
	, ShipAmt				numeric(18,0)
	, DepositAmt			numeric(18,0)
	, OthersAmt				numeric(18,0)
)

insert into @MainTable
exec uspfn_OmInquirySalesWeb @StartDate, @EndDate, @Area, @Dealer, @Outlet, @BranchHead, @SalesHead, @SalesCoordinator, @Salesman, @SalesType

select * into #t1 from(
select Area
		, CompanyCode
		, CompanyName
		, BranchCode
		, BranchName
		, BranchHeadName
		, SalesHeadName
		, SalesCoordinatorName
		, SalesmanName
		, SalesType
		, SalesModelCode
		, SalesModelYear
		, SalesModelDesc
		, FakturPolisiDesc
		, case when GroupMarketModel = '' then 'XYZ' else case when substring(GroupMarketModel,4,1) = '.' then Right(GroupMarketModel,LEN(GroupMarketModel) - 4) else GroupMarketModel end end GroupMarketModel
		, case when ColumnMarketModel = '' then 'XYZ' else case when substring(ColumnMarketModel,4,1) = '.' then Right(ColumnMarketModel,LEN(ColumnMarketModel) - 4) else ColumnMarketModel end end ColumnMarketModel
		, Grade
		, ModelCatagory
		, MarketModel
		, ColourCode
		, ColourName
		, YEAR(InvoiceDate) Year 
		, MONTH(InvoiceDate) Month
		, InvoiceDate
		, count(ChassisCode) SoldTotal
  from @MainTable
  group by Area 
		, CompanyCode
		, CompanyName
		, BranchCode
		, BranchName
		, BranchHeadName
		, SalesHeadName
		, SalesCoordinatorName
		, SalesmanName
		, SalesType
		, SalesModelCode
		, SalesModelYear
		, SalesModelDesc
		, FakturPolisiDesc
		, GroupMarketModel
		, ColumnMarketModel
		, Grade
		, ModelCatagory
		, MarketModel
		, ColourCode
		, ColourName
		, YEAR(InvoiceDate) 
		, MONTH(InvoiceDate)
		, InvoiceDate
)#t1

Declare @NSDS table
(
	Area					varchar(100)
	, CompanyCode			varchar(15)
	, CompanyName			varchar(100)
	, BranchCode			varchar(15)
	, BranchName			varchar(100)
	, BranchHeadName		varchar(100)
	, SalesHeadName			varchar(100)
	, SalesCoordinatorName	varchar(100)
	, SalesmanName			varchar(100)
	, SalesType				varchar(25)
	, SalesModelCode		varchar(25)
	, SalesModelYear		numeric(4,0)
	, SalesModelDesc		varchar(150)
	, FakturPolisiDesc		varchar(100)
	, GroupMarketModel		varchar(100)
	, ColumnMarketModel		varchar(100)
	, Grade					varchar(50)
	, ModelCatagory			varchar(15)
	, MarketModel			varchar(25)
	, ColourCode			varchar(25)
	, ColourName			varchar(100)
	, Year					numeric(4,0)
	, Month					numeric(4,0)
	, InvoiceDate			datetime
	, SoldTotal				decimal(18,0)
)

if @SalesType <> 'WHOLESALE'
begin
	insert into @NSDS
		select c.Area
			, case when a.CustomerCode = '6015402' then '6015401' else case when a.CustomerCode = '6051402' then '6051401' else a.CustomerCode end end CompanyCode
			, isnull(c.DealerAbbreviation,a.CustomerCode) CompanyName
			, isnull(d.OutletCode,'') BranchCode
			, isnull(d.OutletAbbreviation,'HQ') BranchName
			, '' BranchHeadName
			, '' SalesHeadName
			, '' SalesCoordinatorName
			, '' SalesmanName
			, '' SalesType
			, a.SalesModelCode
			, 1900 SalesModelYear
			, b.SalesModelDesc
			, '' FakturPolisiDesc
			, b.GroupMarketModel
			, b.ColumnMarketModel
			, '' Grade
			, (select Top 1 ModelCatagory from @MainTable where a.SalesModelCode = SalesModelCode) ModelCategory
			, (select Top 1 MarketModel from @MainTable where a.SalesModelCode = SalesModelCode) MarketModel
			, '' ColourCode
			, '' ColourName
			, case when @SalesType in ('RETAIL','SALES') then YEAR(a.DODate) else YEAR(a.ProcessDate) end Year
			, case when @SalesType in ('RETAIL','SALES') then MONTH(a.DODate) else MONTH(a.ProcessDate) end Month
			, case when @SalesType in ('RETAIL','SALES') then YEAR(a.DODate) else YEAR(a.ProcessDate) end InvoiceDate
			, COUNT(a.ChassisCode) SoldTotal
		from OmHstInquirySalesNSDS a
		left join OmMstModel b on a.SalesModelCode = b.SalesModelCode
		left join GnMstDealerMapping c on  a.CustomerCode = c.DealerCode
		left join GnMstDealerOutletMapping d on d.DealerCode = a.CustomerCode
			and d.LastUpdateBy = 'HQ'
		where case when @SalesType in ('RETAIL','SALES') 
				then convert(varchar,a.DoDate,112) 
				else convert(varchar,a.ProcessDate,112) 
				end between convert(varchar,@StartDate,112) and convert(varchar,@EndDate,112) 
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
			and isnull(c.DealerCode,'') like case when isnull(@Dealer,'') <> '' then @Dealer else '%%' end
			and isnull(d.OutletCode,'') like case when isnull(@Outlet,'') <> '' then @Outlet else '%%' end 
			and a.GroupAreaCode = '3'
		group by c.Area
			, a.CustomerCode
			, c.DealerAbbreviation
			, d.OutletCode
			, d.OutletAbbreviation
			, a.SalesModelcode
			, b.SalesModelDesc
			, b.GroupMarketModel
			, b.ColumnMarketModel
			, a.DoDate
			, a.ProcessDate
			
	select * from #t1
	union all
	select * from @NSDS 
End

if @SalesType = 'WHOLESALE'
	select * from #t1

drop table #t1