create view [dbo].[PmKdpClnUpView]
with schemabinding
as

select a.CompanyCode, a.BranchCode, a.InquiryNumber, convert(varchar, a.InquiryNumber, 100) InquiryNumberStr, InquiryDate, convert(varchar, a.InquiryDate, 103) InquiryDateStr, a.NamaProspek, 
a.TipeKendaraan, a.Variant, a.Transmisi, a.ColourCode, a.PerolehanData, c.EmployeeID, c.EmployeeID + ' - ' + c.EmployeeName Wiraniaga, 
d.EmployeeID SpvEmployeeID, d.EmployeeID + ' - ' + d.EmployeeName Coordinator, a.LastProgress, 
(select top 1 NextFollowUpDate from dbo.pmActivities where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InquiryNumber = a.InquiryNumber order by NextFollowUpDate desc) NextFollowUpDate,
(select top 1 convert(varchar, NextFollowUpDate, 103) from dbo.pmActivities where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InquiryNumber = a.InquiryNumber order by NextFollowUpDate desc) NextFollowUpDateStr
from dbo.pmKDP a
inner join dbo.gnMstEmployee c on
	a.CompanyCode = c.CompanyCode 
	and a.BranchCode = c.BranchCode
	and a.EmployeeID = c.EmployeeID
inner join dbo.gnMstEmployee d on
	a.CompanyCode = d.CompanyCode 
	and a.BranchCode = d.BranchCode
	and a.SpvEmployeeID = d.EmployeeID	
where a.LastProgress in ('P', 'HP', 'SPK')	
	
GO

delete from sysmenudms
where menuid = 'its'

insert into sysmenudms
values ('itstrans', 'Transaksi', 'its', 1 ,2, '')

insert into sysmenudms
values ('itsclnupkdp', 'Clean Up ITS', 'itstrans', 2 ,2, 'trans/clnupkdp')

update sysmenudms
set menuheader = 'itstrans'
where menuid = 'itsclnupkdp'

insert into sysrolemenu
values ('ITS', 'itstrans')

update gnmstlookupdtl
set paravalue = '20140228'
where codeid = 'clnup'
and lookupvalue = 'itsdate'

insert into sysRolemenu
values('ITS',	'itsclnupkdp')

insert into sysRolemenu
values('its', 'itstrans')

select * into #t1 from(
SELECT ROW_NUMBER() OVER (ORDER BY p.UserId) AS Row, p.UserId, 'ITS' RoleID from pmPosition p where p.PositionId >= 20) #t1

declare @int as int
set @int = 0
declare @UserID as varchar(50)

while (@int < (select count(*) from #t1))
begin
set @int = @int + 1
set @UserID = (select userID from #t1 where Row = @int)
insert into sysroleuser
values (@UserID, 'ITS')
end

drop table #t1



declare @CompanyCode as varchar(15)
set @CompanyCode = (select top 1 CompanyCode from gnMstCoProfile)

declare @CodeID as varchar(15)
set @CodeID = 'CLNUP'

declare @LookupValue as varchar(15)
set @LookupValue = 'ITSDATE'

INSERT INTO gnMstLookUpHdr VALUES (@CompanyCode, @CodeID, 'Clean Up ITS', 1, 0, 'sa', getdate(), 'sa', getdate(), 0 , '', null);

INSERT INTO gnMstLookUpDtl VALUES (@CompanyCode, @CodeID, @LookupValue, 1, '20140228', 'Setting Date for clean up', 'sa', getdate(), 'sa', getdate());

set @LookupValue = 'PER'
INSERT INTO gnMstLookUpDtl VALUES (@CompanyCode, @CodeID, @LookupValue, 2, 1, '0:Disable Per Date for Clean up, 1:Enable Per Date for Clean Up', 'sa', getdate(), 'sa', getdate());

update sysmenudms
set menucaption = 'Clean Up ITS'
where menuid = 'itsclnupkdp'


