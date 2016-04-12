--created by Benedict 27/10/2015 last updated 27/10/2015

if object_id('[dbo].[uspfn_pmRandomizeKDP]') is not null drop procedure [dbo].[uspfn_pmRandomizeKDP]
go

create procedure [dbo].[uspfn_pmRandomizeKDP]
	@CompanyCode	varchar(20)
,	@EmployeeID		varchar(20)
as

--declare 
--	@CompanyCode	varchar(20)
--,	@EmployeeID		varchar(20)

--select 
--	@CompanyCode = '6006406'
--,	@EmployeeID	= '455'

declare
	@TeamLeader		varchar(20)
,	@BranchCode		varchar(20)
,	@InquiryNumber	int
,	@m				int
,	@membersCount	int

declare @Employee table(
	CompanyCode	varchar(20)
,	BranchCode	varchar(20)
,	EmployeeID	varchar(20)
,	Position	varchar(10)
,	TeamLeader	varchar(20)
)

declare @TeamMembers table(
	rownum		int
,	CompanyCode	varchar(20)
,	BranchCode	varchar(20)
,	EmployeeID	varchar(20)
,	Position	varchar(10)
,	TeamLeader	varchar(20)
)

declare @kdps table(
	InquiryNumber	int
,	BranchCode		varchar(20)
,	CompanyCode		varchar(20)
,	EmployeeID		varchar(20)
,	SpvEmployeeID	varchar(20)
)

;with _1 as
(
    select a.CompanyCode, a.EmployeeID, max(a.MutationDate) as MutationDate from HrEmployeeMutation a
    where a.CompanyCode = @CompanyCode and a.EmployeeID = @EmployeeID
    group by a.CompanyCode, a.EmployeeID
), _2 as
(
    select a.CompanyCode, a.EmployeeID, b.BranchCode, a.MutationDate from _1 a
    inner join HrEmployeeMutation b on a.CompanyCode = b.CompanyCode and a.EmployeeID = b.EmployeeID and a.MutationDate = b.MutationDate
), _3 as
(
    select a.CompanyCode, b.BranchCode, a.EmployeeID, a.Position, a.TeamLeader from HrEmployee a
    left join _2 b on a.CompanyCode = b.CompanyCode and a.EmployeeID = b.EmployeeID
    where a.CompanyCode = @CompanyCode and a.Department = 'SALES'
    and a.EmployeeID = @EmployeeID
)
select 
	@TeamLeader = (select Teamleader from _3)
,	@BranchCode = (select BranchCode from _3)

;with _1 as(
	select a.CompanyCode, a.EmployeeID, max(a.MutationDate) as MutationDate from HrEmployeeMutation a
	where a.CompanyCode = @CompanyCode and a.BranchCode = @BranchCode and a.IsDeleted = 0 
	group by a.CompanyCode, a.EmployeeID
), _2 as(
	select a.CompanyCode, a.EmployeeID, b.BranchCode, a.MutationDate from _1 a
	inner join HrEmployeeMutation b on a.CompanyCode = b.CompanyCode and a.EmployeeID = b.EmployeeID and a.MutationDate = b.MutationDate
), _3 as(
	select a.CompanyCode,  b.BranchCode, a.EmployeeID, a.Position, a.TeamLeader from HrEmployee a
	left join _2 b on a.CompanyCode = b.CompanyCode and a.EmployeeID = b.EmployeeID
	where a.CompanyCode = @CompanyCode and a.Department = 'SALES'
	and a.TeamLeader = @TeamLeader and a.PersonnelStatus = '1' and a.IsDeleted = 0 and a.IsAssigned = 1
)
insert into @TeamMembers
select row_number() over(order by newid()), * from _3
where EmployeeID <> @EmployeeID

insert into @kdps
select InquiryNumber, BranchCode, CompanyCode, EmployeeID, SpvEmployeeID from pmKDP
where CompanyCode = @CompanyCode and BranchCode = @BranchCode and EmployeeID = @EmployeeID
order by newid()

begin tran
select @m = 1, @membersCount = (select count(*) from @TeamMembers)
while 1=1 begin
	if (select count(*) from @kdps) = 0 break
	select @InquiryNumber = (select top 1 InquiryNumber from @kdps)

	update pmKDP
	set EmployeeID = (select EmployeeID from @TeamMembers where rownum = @m)
	where InquiryNumber = @InquiryNumber and BranchCode = @BranchCode and CompanyCode = @CompanyCode
	
	if @@error <> 0 begin
		rollback tran
		return 10
	end

	delete from @kdps where InquiryNumber = @InquiryNumber and BranchCode = @BranchCode and CompanyCode = @CompanyCode
	if(@m = @membersCount) set @m = 1
	else set @m = @m + 1
end
commit tran
go