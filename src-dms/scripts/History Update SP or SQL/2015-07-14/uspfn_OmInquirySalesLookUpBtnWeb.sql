IF object_id('uspfn_OmInquirySalesLookUpBtnWeb') IS NOT NULL
	DROP PROCEDURE uspfn_OmInquirySalesLookUpBtnWeb
GO
CREATE procedure [dbo].[uspfn_OmInquirySalesLookUpBtnWeb]
	@StartDate Datetime,
	@EndDate Datetime,
	@Area varchar(100),
	@Dealer varchar(100),
	@Outlet varchar(100),
	@BranchHead varchar(100),
	@SalesHead varchar(100),
	@SalesCoordinator varchar(100),
	@Salesman varchar(100),
	@Detail int,
	@SalesType varchar(50)
as
Declare @MainTable table
(
	GroupNo					varchar(100)
	, Area					varchar(100)
	, CompanyCode			varchar(15)
	, CompanyName			varchar(100)
	, BranchCode			varchar(15)
	, BranchName			varchar(100)
	, BranchHeadID			varchar(15)
	, BranchHeadName		varchar(100)
	, SalesHeadID			varchar(15)
	, SalesHeadName			varchar(100)
	, SalesCoordinatorID	varchar(15)
	, SalesCoordinatorName	varchar(100)
	, SalesmanID			varchar(15)
	, SalesmanName			varchar(100)
	, ModelCatagory			varchar(15)
	, SalesType				varchar(25)
	, InvoiceNo				varchar(15)
	, InvoiceDate			datetime
	, SONo					varchar(15)
	, SalesModelCode		varchar(25)
	, Year					numeric(4,0)
	, SalesModelDesc		varchar(150)
	, FakturPolisiNo		varchar(15)
	, FakturPolisiDate		datetime
	, FakturPolisiDesc		varchar(150)
	, MarketModel			varchar(25)
	, ColourCode			varchar(25)
	, ColourName			varchar(100)
	, GroupMarketModel		varchar(100)
	, ColumnMarketModel		varchar(100)
	, JoinDate				datetime
	, ResignDate			datetime
	, GradeDate				datetime
	, Grade					varchar(50)
	, ChassisCode			varchar(15)
	, ChassisNo				varchar(15)
	, EngineCode			varchar(15) 
	, EngineNo				varchar(15)
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

select * into #t6 from (
select GroupNo
		, Area
		, CompanyCode
		, CompanyName
		, BranchCode
		, BranchName
		, BranchHeadID
		, BranchHeadName
		, SalesHeadID
		, SalesHeadName
		, SalesCoordinatorID
		, SalesCoordinatorName
		, SalesmanID
		, SalesmanName
		, ModelCatagory
		, SalesModelDesc
		, MarketModel
		, ColourName
		, YEAR(InvoiceDate) Year 
		, MONTH(InvoiceDate) Month
		, COUNT(SalesModelCode) SoldTotal
  from @MainTable
  group by GroupNo
		, Area 
		, CompanyCode
		, CompanyName
		, BranchCode
		, BranchName
		, BranchHeadID
		, BranchHeadName
		, SalesHeadID
		, SalesHeadName
		, SalesCoordinatorID
		, SalesCoordinatorName
		, SalesmanID
		, SalesmanName
		, SalesModelDesc
		, ModelCatagory
		, MarketModel
		, ColourName
		, YEAR(InvoiceDate) 
		, MONTH(InvoiceDate)
)#t6

declare @TempTable table(
	Area					varchar(100),
	CompanyCode				varchar(100),
	CompanyName				varchar(100),
	BranchCode				varchar(100),
	BranchName				varchar(100),
	BranchHeadID			varchar(100),
	BranchHeadName			varchar(100),
	SalesHeadID				varchar(100),
	SalesHeadName			varchar(100),
	SalesCoordinatorID		varchar(100),
	SalesCoordinatorName	varchar(100),
	SalesmanID				varchar(100),
	SalesmanName			varchar(100)
)
insert into @TempTable
select '<----Select All---->','<----Select All---->','<----Select All---->','<----Select All---->','<----Select All---->','<----Select All---->','<----Select All---->','<----Select All---->',
'<----Select All---->','<----Select All---->','<----Select All---->','<----Select All---->','<----Select All---->'

if(@Detail = 4)
begin
	insert into @TempTable
	select distinct Area
		, CompanyCode
		, CompanyName
		, BranchCode
		, BranchName
		, BranchHeadID
		, BranchHeadName
		, '' [1]
		, '' [2]
		, '' [3]
		, '' [4]
		, '' [5]
		, '' [6] 
	from #t6 
	where (#t6.Area like Case when @Area <> '' 
								then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
										then 'JABODETABEK'
										else @Area end
								else '%%' end
			or #t6.Area like Case when @Area <> '' 
								then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
										then 'CABANG'
										else @Area end
								else '%%' end)
		and #t6.CompanyCode like Case when @Dealer <> '' then @Dealer else '%%' end
		and #t6.BranchCode like Case when @Outlet <> '' then @Outlet else '%%' end
		and isnull(#t6.BranchHeadName,'') <> '' 
	order by BranchHeadName
end
else if(@Detail = 5)
begin
	insert into @TempTable
	select distinct Area
		, CompanyCode
		, CompanyName
		, BranchCode
		, BranchName
		, BranchHeadID
		, BranchHeadName 
		, SalesHeadID
		, SalesHeadName 
		, '' [1]
		, '' [2]
		, '' [3]
		, '' [4]
	from #t6
	where  (#t6.Area like Case when @Area <> '' 
								then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
										then 'JABODETABEK'
										else @Area end
								else '%%' end
			or #t6.Area like Case when @Area <> '' 
								then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
										then 'CABANG'
										else @Area end
								else '%%' end)
		and #t6.CompanyCode like Case when @Dealer <> '' then @Dealer else '%%' end
		and #t6.BranchCode like Case when @Outlet <> '' then @Outlet else '%%' end 
		and #t6.BranchHeadID like Case when @BranchHead <> '' then @BranchHead else '%%' end 
		and isnull(#t6.SalesHeadID,'') <> '' 
	order by SalesHeadName
end
else if(@Detail = 6)
begin
	insert into @TempTable
	select distinct Area
		, CompanyCode
		, CompanyName
		, BranchCode
		, BranchName
		, BranchHeadID
		, BranchHeadName 
		, SalesHeadID
		, SalesHeadName 
		, SalesCoordinatorID
		, SalesCoordinatorName 
		, '' [1]
		, '' [2]
	from #t6 
	where  (#t6.Area like Case when @Area <> '' 
								then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
										then 'JABODETABEK'
										else @Area end
								else '%%' end
			or #t6.Area like Case when @Area <> '' 
								then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
										then 'CABANG'
										else @Area end
								else '%%' end)
		and #t6.CompanyCode like Case when @Dealer <> '' then @Dealer else '%%' end
		and #t6.BranchCode like Case when @Outlet <> '' then @Outlet else '%%' end
		and #t6.BranchHeadID like Case when @BranchHead <> '' then @BranchHead else '%%' end 
		and #t6.SalesHeadID like Case when @SalesHead <> '' then @SalesHead else '%%' end 
		--and isnull(#t6.SalesCoordinatorName,'') <> '' 
	order by SalesCoordinatorName
end
else if(@Detail = 7)
begin
	insert into @TempTable
	select distinct Area
		, CompanyCode
		, CompanyName
		, BranchCode
		, BranchName
		, BranchHeadID
		, BranchHeadName 
		, SalesHeadID
		, SalesHeadName 
		, SalesCoordinatorID
		, SalesCoordinatorName 
		, SalesmanID
		, SalesmanName
	from #t6 
	where  (#t6.Area like Case when @Area <> '' 
								then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
										then 'JABODETABEK'
										else @Area end
								else '%%' end
			or #t6.Area like Case when @Area <> '' 
								then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
										then 'CABANG'
										else @Area end
								else '%%' end)
		and #t6.CompanyCode like Case when @Dealer <> '' then @Dealer else '%%' end
		and #t6.BranchCode like Case when @Outlet <> '' then @Outlet else '%%' end
		and #t6.BranchHeadID like Case when @BranchHead <> '' then @BranchHead else '%%' end 
		and #t6.SalesHeadID like Case when @SalesHead <> '' then @SalesHead else '%%' end 
		and #t6.SalesCoordinatorID like Case when @SalesCoordinator <> '' then @SalesCoordinator else '%%' end 
		and isnull(#t6.SalesmanName,'') <> '' 
	order by SalesmanName
end
else if(@Detail = 8)
begin
	insert into @TempTable
	select distinct MarketModel
		, '' [1]
		, '' [2]
		, '' [3]
		, '' [4]
		, '' [5]
		, '' [6]
		, '' [7]
		, '' [8]
		, '' [9]
		, '' [10]
		, '' [11]
		, '' [12]
	from #t6 
	where  (#t6.Area like Case when @Area <> '' 
								then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
										then 'JABODETABEK'
										else @Area end
								else '%%' end
			or #t6.Area like Case when @Area <> '' 
								then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
										then 'CABANG'
										else @Area end
								else '%%' end)
		and #t6.CompanyCode like Case when @Dealer <> '' then @Dealer else '%%' end
		and #t6.BranchCode like Case when @Outlet <> '' then @Outlet else '%%' end
		and #t6.BranchHeadID like Case when @BranchHead <> '' then @BranchHead else '%%' end 
		and #t6.SalesHeadID like Case when @SalesHead <> '' then @SalesHead else '%%' end 
		and #t6.SalesCoordinatorID like Case when @SalesCoordinator <> '' then @SalesCoordinator else '%%' end 
	order by MarketModel
end

select * from @TempTable