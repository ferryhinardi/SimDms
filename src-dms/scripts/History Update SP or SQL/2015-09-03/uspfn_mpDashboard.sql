IF (OBJECT_ID('uspfn_mpDashboard') is not null)
	drop proc dbo.uspfn_mpDashboard
GO

/****** Object:  StoredProcedure [dbo].[uspfn_mpDashboard]    Script Date: 9/8/2015 2:53:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--uspfn_mpDashboard '6006400130', '3-3-2015', '3-3-2015', false
CREATE procedure [dbo].[uspfn_mpDashboard]
@BranchCode varchar(25),
@Start date,
@End date,
@Total int

as

declare @tblResult table(
	Outlet varchar(50),
	BranchManager int,
	SalesHead int,
	Platinum int,
	Gold int,
	Silver int,
	Trainee int,
	PlatinumPct int,
	GoldPct int,
	SilverPct int,
	TraineePct int,
	TotalSalesPerson int,
	TotalSalesForce int
)

declare @Positions table(
	Position varchar(5)
)

insert into @Positions VALUES ('BM')
insert into @Positions VALUES ('SH')
insert into @Positions VALUES ('S1')
insert into @Positions VALUES ('S2')
insert into @Positions VALUES ('S3')
insert into @Positions VALUES ('S4')

;with _1 as 
(
	select CompanyCode, EmployeeID, max(AssignDate) AssignDate from HrEmployeeAchievement 
	where Convert(date, AssignDate) between @Start and @End and Department = 'SALES'
	group by CompanyCode, EmployeeID
), _2 AS
(
	select b.CompanyCode,b.EmployeeID,b.AssignDate,b.Position,b.Grade from _1 a 
	join HrEmployeeAchievement b on a.CompanyCode = b.CompanyCode and a.EmployeeId = b.EmployeeID and a.AssignDate = b.AssignDate 
	where b.Position not in ('SC','GM')
), _3 AS
(
    select b.CompanyCode,b.EmployeeID,a.AssignDate,a.Position,a.Grade,max(b.MutationDate) MutationDate from _2 a 
	join HrEmployeeMutation b on a.CompanyCode = b.CompanyCode and a.EmployeeID = b.EmployeeID
	where CONVERT(date,b.MutationDate) between @Start and @End
	group by b.CompanyCode,b.EmployeeID,a.AssignDate,a.Position,a.Grade
), _4 as
(
	select b.CompanyCode,b.BranchCode,b.EmployeeID,a.AssignDate,a.Position,a.Grade,b.MutationDate from _3 a 
	join HrEmployeeMutation b on a.CompanyCode = b.CompanyCode and a.EmployeeID = b.EmployeeID and a.MutationDate = b.MutationDate
), _5 as
(
	select b.CompanyCode,a.BranchCode,b.EmployeeID,a.AssignDate,(a.Position + isnull(a.Grade,1)) Position ,a.MutationDate from _4 a 
	join HrEmployee b on a.CompanyCode = b.CompanyCode and a.EmployeeID = b.EmployeeID
	where (b.ResignDate is null or Convert(date,b.ResignDate) <= Convert(date,b.JoinDate) or Convert(date,b.ResignDate) > @End) 
), _6 as
(
	select a.DealerCode as CompanyCode,a.OutletCode as BranchCode,a.OutletAbbreviation,b.Position from gnMstDealerOutletMapping a
	CROSS JOIN @Positions b
), _7 as
(
	select a.BranchCode,a.OutletAbbreviation, a.Position, (select count(*) from _5 b where a.CompanyCode = b.CompanyCode and a.BranchCode = b.BranchCode and a.Position = b.Position) as nPosition
	from _6 a
), _8 as
(
	select * from _7
	pivot
	(
	  max(nPosition)
	  for Position in ([BM],[SH],[S1],[S2],[S3],[S4])
	) piv
), _9 as
(
	select BranchCode,OutletAbbreviation,BM,SH,S1,S2,S3,S4,(S1+S2+S3+S4) TotalSalesPerson, (BM+SH+S1+S2+S3+S4) TotalSalesForce from _8 
	where BranchCode = (case when @BranchCode = '' then BranchCode else @BranchCode end)
), _10 as
(
	select (BranchCode +' - '+ OutletAbbreviation) Outlet, BM as BranchManager, SH as SalesHead,  
	S1 as Platinum,isnull(convert(numeric(18,2),(convert(numeric(18,2), S1)/nullif(TotalSalesPerson,0) * 100)),0) PlatinumPct,
	S2 as Gold,isnull(convert(numeric(18,2),(convert(numeric(18,2), S2)/nullif(TotalSalesPerson,0) * 100)),0) GoldPct,
	S3 as Silver,isnull(convert(numeric(18,2),(convert(numeric(18,2), S3)/nullif(TotalSalesPerson,0) * 100)),0) SilverPct,
	S4 as Trainee,isnull(convert(numeric(18,2),(convert(numeric(18,2), S4)/nullif(TotalSalesPerson,0) * 100)),0) TraineePct,
	TotalSalesPerson, TotalSalesForce from _9
)

select * into #temp1 from _10

if(@Total = 0)
BEGIN
select * from #temp1
END
ELSE IF (@Total = 1)
BEGIN
select 'TOTAL DEALER' as Outlet, SUM(BranchManager) as BranchManager, SUM(SalesHead) as SalesHead
, SUM(Platinum) as Platinum , isnull(convert(numeric(18,2),(convert(numeric(18,2), SUM(Platinum))/nullif(SUM(TotalSalesPerson),0) * 100)),0) PlatinumPct
, SUM(Gold) as Gold, isnull(convert(numeric(18,2),(convert(numeric(18,2), SUM(Gold))/nullif(SUM(TotalSalesPerson),0) * 100)),0) GoldPct
, SUM(Silver) as Silver, isnull(convert(numeric(18,2),(convert(numeric(18,2), SUM(Silver))/nullif(SUM(TotalSalesPerson),0) * 100)),0) SilverPct
, SUM(Trainee) as Trainee, isnull(convert(numeric(18,2),(convert(numeric(18,2), SUM(Trainee))/nullif(SUM(TotalSalesPerson),0) * 100)),0) TraineePct
, SUM(TotalSalesPerson) as TotalSalesPerson, SUM(TotalSalesForce) as TotalSalesForce from #temp1
END
ELSE IF (@Total = 2)
BEGIN
select * from #temp1
select 'TOTAL DEALER' as Outlet, SUM(BranchManager) as BranchManager, SUM(SalesHead) as SalesHead
, SUM(Platinum) as Platinum , isnull(convert(numeric(18,2),(convert(numeric(18,2), SUM(Platinum))/nullif(SUM(TotalSalesPerson),0) * 100)),0) PlatinumPct
, SUM(Gold) as Gold, isnull(convert(numeric(18,2),(convert(numeric(18,2), SUM(Gold))/nullif(SUM(TotalSalesPerson),0) * 100)),0) GoldPct
, SUM(Silver) as Silver, isnull(convert(numeric(18,2),(convert(numeric(18,2), SUM(Silver))/nullif(SUM(TotalSalesPerson),0) * 100)),0) SilverPct
, SUM(Trainee) as Trainee, isnull(convert(numeric(18,2),(convert(numeric(18,2), SUM(Trainee))/nullif(SUM(TotalSalesPerson),0) * 100)),0) TraineePct
, SUM(TotalSalesPerson) as TotalSalesPerson, SUM(TotalSalesForce) as TotalSalesForce from #temp1
END

drop table #temp1
