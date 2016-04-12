ALTER procedure [dbo].[uspfn_SyncPmExecSummary2]
as

declare @t_group as table (
    GroupSeq  int, 
    GroupCode varchar(20), 
    GroupName varchar(50)
)

insert into @t_group values (1, 'ERTIGA', 'Ertiga')
insert into @t_group values (2, 'WAGON R', 'Karimun Wagon R')
insert into @t_group values (3, 'SL415-PU', 'PU Futura')
insert into @t_group values (4, 'APV-PU', 'PU Mega Carry')
insert into @t_group values (5, 'OTHERS', 'Others')
insert into @t_group values (6, 'ALL', 'All Model')

declare @t_groupsum as table (
    GroupType varchar(10), 
    GroupCode varchar(50),
    DataCount int
)

declare @t_execsum as table (
    GroupType varchar(10), 
    GroupSeq  int, 
    GroupCode varchar(20),
    GroupName varchar(50),
    DataCount int
)


-- inquiry all data
insert into @t_groupsum
select 'H', c.GroupModel, DataCount=count(*)
	from suzukir4..pmHstITS a
      left join suzukir4..MsMstGroupModel c
        on c.ModelType = a.TipeKendaraan
     where a.LastProgress in ('P','HP','SPK')
       and exists (select top 1 1 from suzukir4..pmStatusHistory
                    where CompanyCode=a.CompanyCode
                      and BranchCode=a.BranchCode
                      and InquiryNumber=a.InquiryNumber
                      and LastProgress = 'SPK'
                    order by UpdateDate desc)
       and not exists (select top 1 1 from suzukir4..pmStatusHistory 
                    where CompanyCode=a.CompanyCode
                      and BranchCode=a.BranchCode
                      and InquiryNumber=a.InquiryNumber
                      and LastProgress in ('LOST','DO','DELIVERY'))
 group by c.GroupModel


declare @date1LM     datetime -- first date last month
declare @date1CM     datetime -- first date this month
declare @dateMin01   datetime -- D-1 (yesterday)
declare @dateMin02   datetime -- D-2
declare @dateMin30   datetime -- D-30
declare @dateMin31   datetime -- D-31
declare @dateCountLM int

set @date1LM     = left(convert(varchar, dateadd(month, -1, getdate()), 112), 6) + '01'
set @date1CM     = left(convert(varchar, getdate(), 112), 6) + '01'
set @dateMin01   = convert(varchar, dateadd(day, -1, getdate()), 112)
set @dateMin02   = convert(varchar, dateadd(day, -2, getdate()), 112)
set @dateMin30   = convert(varchar, dateadd(day, -30, getdate()), 112)
set @dateMin31   = convert(varchar, dateadd(day, -31, getdate()), 112)
set @dateCountLM = day(dateadd(day, -1, @date1CM))


-- Inquiry AVG Last Month
insert into @t_groupsum
select 'DI1', b.GroupModel, DataCount=count(*) 
  from suzukir4..pmHstITS a
      left join suzukir4..MsMstGroupModel b
        on b.ModelType = a.TipeKendaraan
     where convert(varchar, a.InquiryDate, 112) >= convert(varchar, @date1LM, 112) and convert(varchar, InquiryDate, 112) < convert(varchar, @date1CM, 112)
 group by b.GroupModel

-- Inquiry D-2
insert into @t_groupsum
select 'DI2', b.GroupModel, DataCount=count(*)
      from suzukir4..pmHstITS a
      left join suzukir4..MsMstGroupModel b
        on b.ModelType = a.TipeKendaraan
     where convert(varchar, a.InquiryDate, 112) >= convert(varchar, @dateMin31, 112) and convert(varchar, a.InquiryDate, 112) <= convert(varchar, @dateMin02, 112)
 group by b.GroupModel

-- Inquiry D-1
insert into @t_groupsum
select 'DI3', b.GroupModel, DataCount=count(*)
      from suzukir4..pmHstITS a
      left join suzukir4..MsMstGroupModel b
        on b.ModelType = a.TipeKendaraan
     where convert(varchar, a.InquiryDate, 112) = convert(varchar, @dateMin01, 112)
 group by b.GroupModel


-- SPK AVG Last Month
insert into @t_groupsum
select 'DS1', c.GroupModel, DataCount=count(*)
	from suzukir4..pmHstITS a
      left join suzukir4..MsMstGroupModel c
        on c.ModelType = a.TipeKendaraan
     where convert(varchar, @date1LM, 112) <= (select top 1 convert(varchar,isnull(UpdateDate,'1900/01/01'),112) 
                                                 from suzukir4..pmStatusHistory
                                                where CompanyCode=a.CompanyCode
                                                  and BranchCode=a.BranchCode
                                                  and InquiryNumber=a.InquiryNumber
                                                  and LastProgress = 'SPK'
                                                order by UpdateDate desc)
       and convert(varchar, @date1CM, 112) >  (select top 1 convert(varchar,isnull(UpdateDate,'1900/01/01'),112) 
                                                 from suzukir4..pmStatusHistory
                                                where CompanyCode=a.CompanyCode
                                                  and BranchCode=a.BranchCode
                                                  and InquiryNumber=a.InquiryNumber
                                                  and LastProgress = 'SPK'
                                                order by UpdateDate desc)
	   and convert(varchar, a.InquiryDate, 112) < convert(varchar, @date1CM, 112)
 group by c.GroupModel
  
-- SPK S2
insert into @t_groupsum
select 'DS2', c.GroupModel, DataCount=count(*)
	from suzukir4..pmHstITS a
      left join suzukir4..MsMstGroupModel c
        on c.ModelType = a.TipeKendaraan
     where convert(varchar, @dateMin30, 112) <= (select top 1 convert(varchar,isnull(UpdateDate,'1900/01/01'),112) 
                                                 from suzukir4..pmStatusHistory
                                                where CompanyCode=a.CompanyCode
                                                  and BranchCode=a.BranchCode
                                                  and InquiryNumber=a.InquiryNumber
                                                  and LastProgress = 'SPK'
                                                order by UpdateDate desc)
       and convert(varchar, @dateMin01, 112) >= (select top 1 convert(varchar,isnull(UpdateDate,'1900/01/01'),112) 
                                                 from suzukir4..pmStatusHistory
                                                where CompanyCode=a.CompanyCode
                                                  and BranchCode=a.BranchCode
                                                  and InquiryNumber=a.InquiryNumber
                                                  and LastProgress = 'SPK'
                                                order by UpdateDate desc)
	   and convert(varchar, a.InquiryDate, 112) <= convert(varchar, @dateMin01, 112)
 group by c.GroupModel
  
-- SPK S3
insert into @t_groupsum
select 'DS3', c.GroupModel, DataCount=count(*)
	from suzukir4..pmHstITS a
      left join suzukir4..MsMstGroupModel c
        on c.ModelType = a.TipeKendaraan
     where convert(varchar, getdate(), 112) = (select top 1 convert(varchar,isnull(UpdateDate,'1900/01/01'),112) 
                                                 from suzukir4..pmStatusHistory
                                                where CompanyCode=a.CompanyCode
                                                  and BranchCode=a.BranchCode
                                                  and InquiryNumber=a.InquiryNumber
                                                  and LastProgress = 'SPK'
                                                order by UpdateDate desc)
 group by c.GroupModel


-- Faktur AVG Last Month
insert into @t_groupsum
select 'DF1', GroupModel, count(1) as DataCount from (
	select distinct h.ChassisCode, h.ChassisNo, h.EngineCode, h.EngineNo, g.GroupModel
	  from SuzukiR4..omHstInquirySalesNSDS h
	  left join SuzukiR4..omMstModel m
		on m.SalesModelCode=left(h.SalesModelCode,15)
	  left join SuzukiR4..msMstGroupModel g
		on m.GroupCode=g.ModelType
	  where h.ProcessDate >= @date1LM and h.ProcessDate < @date1CM
    ) x
 group by GroupModel

-- DF2
insert into @t_groupsum
select 'DF2', GroupModel, count(1) as DataCount from (
	select distinct h.ChassisCode, h.ChassisNo, h.EngineCode, h.EngineNo, g.GroupModel
	  from SuzukiR4..omHstInquirySalesNSDS h
	  left join SuzukiR4..omMstModel m
		on m.SalesModelCode=left(h.SalesModelCode,15)
	  left join SuzukiR4..msMstGroupModel g
		on m.GroupCode=g.ModelType
	  where h.ProcessDate >= @dateMin30 and h.ProcessDate <= @dateMin01
    ) x
 group by GroupModel

-- DF3
insert into @t_groupsum
select 'DF3', GroupModel, count(1) as DataCount from (
	select distinct h.ChassisCode, h.ChassisNo, h.EngineCode, h.EngineNo, g.GroupModel
	  from SuzukiR4..omHstInquirySalesNSDS h
	  left join SuzukiR4..omMstModel m
		on m.SalesModelCode=left(h.SalesModelCode,15)
	  left join SuzukiR4..msMstGroupModel g
		on m.GroupCode=g.ModelType
	  where convert(varchar, h.ProcessDate, 112) >= convert(varchar, getdate(), 112)
    ) x
 group by GroupModel

insert into @t_execsum
select 'H' as GoupType
     , a.GroupSeq
     , a.GroupCode
     , a.GroupName
     , DataCount = (
		case GroupCode
			when 'ALL' then
				isnull((select sum(DataCount) from @t_groupsum where GroupType = 'H'), 0)
			when 'OTHERS' then
				isnull((select sum(DataCount) from @t_groupsum where GroupType = 'H' and GroupCode not in (select GroupCode from @t_group where GroupCode<>'OTHERS')), 0)
			else 
				isnull((select sum(DataCount) from @t_groupsum where GroupType = 'H' and GroupCode = a.GroupCode), 0)
		end)
  from @t_group a

insert into @t_execsum
select 'DI1' as GoupType
     , a.GroupSeq
     , a.GroupCode
     , a.GroupName
     , DataCount = (
		case GroupCode
			when 'ALL' then
				isnull((select (sum(DataCount)/@dateCountLM) from @t_groupsum where GroupType = 'DI1'), 0)
			when 'OTHERS' then
				isnull((select (sum(DataCount)/@dateCountLM) from @t_groupsum where GroupType = 'DI1' and GroupCode not in (select GroupCode from @t_group)), 0)
			else 
				isnull((select (sum(DataCount)/@dateCountLM) from @t_groupsum where GroupType = 'DI1' and GroupCode = a.GroupCode), 0)
		end)
  from @t_group a

insert into @t_execsum
select 'DI2' as GoupType
     , a.GroupSeq
     , a.GroupCode
     , a.GroupName
     , DataCount = (
		case GroupCode
			when 'ALL' then
				isnull((select (sum(DataCount)/30.0) from @t_groupsum where GroupType = 'DI2'), 0)
			when 'OTHERS' then
				isnull((select (sum(DataCount)/30.0) from @t_groupsum where GroupType = 'DI2' and GroupCode not in (select GroupCode from @t_group)), 0)
			else 
				isnull((select (sum(DataCount)/30.0) from @t_groupsum where GroupType = 'DI2' and GroupCode = a.GroupCode), 0)
		end)
  from @t_group a

insert into @t_execsum
select 'DI3' as GoupType
     , a.GroupSeq
     , a.GroupCode
     , a.GroupName
     , DataCount = (
		case GroupCode
			when 'ALL' then
				isnull((select sum(DataCount) from @t_groupsum where GroupType = 'DI3'), 0)
			when 'OTHERS' then
				isnull((select sum(DataCount) from @t_groupsum where GroupType = 'DI3' and GroupCode not in (select GroupCode from @t_group)), 0)
			else 
				isnull((select sum(DataCount) from @t_groupsum where GroupType = 'DI3' and GroupCode = a.GroupCode), 0)
		end)
  from @t_group a
  
insert into @t_execsum
select 'DS1' as GoupType
     , a.GroupSeq
     , a.GroupCode
     , a.GroupName
     , DataCount = (
		case GroupCode
			when 'ALL' then
				isnull((select (sum(DataCount)/@dateCountLM) from @t_groupsum where GroupType = 'DS1'), 0)
			when 'OTHERS' then
				isnull((select (sum(DataCount)/@dateCountLM) from @t_groupsum where GroupType = 'DS1' and GroupCode not in (select GroupCode from @t_group)), 0)
			else 
				isnull((select (sum(DataCount)/@dateCountLM) from @t_groupsum where GroupType = 'DS1' and GroupCode = a.GroupCode), 0)
		end)
  from @t_group a
  
insert into @t_execsum
select 'DS2' as GoupType
     , a.GroupSeq
     , a.GroupCode
     , a.GroupName
     , DataCount = (
		case GroupCode
			when 'ALL' then
				isnull((select (sum(DataCount)/30.0) from @t_groupsum where GroupType = 'DS2'), 0)
			when 'OTHERS' then
				isnull((select (sum(DataCount)/30.0) from @t_groupsum where GroupType = 'DS2' and GroupCode not in (select GroupCode from @t_group)), 0)
			else 
				isnull((select (sum(DataCount)/30.0) from @t_groupsum where GroupType = 'DS2' and GroupCode = a.GroupCode), 0)
		end)
  from @t_group a
  
insert into @t_execsum
select 'DS3' as GoupType
     , a.GroupSeq
     , a.GroupCode
     , a.GroupName
     , DataCount = (
		case GroupCode
			when 'ALL' then
				isnull((select sum(DataCount) from @t_groupsum where GroupType = 'DS3'), 0)
			when 'OTHERS' then
				isnull((select sum(DataCount) from @t_groupsum where GroupType = 'DS3' and GroupCode not in (select GroupCode from @t_group)), 0)
			else 
				isnull((select sum(DataCount) from @t_groupsum where GroupType = 'DS3' and GroupCode = a.GroupCode), 0)
		end)
  from @t_group a

insert into @t_execsum
select 'DF1' as GoupType
     , a.GroupSeq
     , a.GroupCode
     , a.GroupName
     , DataCount = (
		case GroupCode
			when 'ALL' then
				isnull((select (sum(DataCount)/@dateCountLM) from @t_groupsum where GroupType = 'DF1'), 0)
			when 'OTHERS' then
				isnull((select (sum(DataCount)/@dateCountLM) from @t_groupsum where GroupType = 'DF1' and GroupCode not in (select GroupCode from @t_group)), 0)
			else 
				isnull((select (sum(DataCount)/@dateCountLM) from @t_groupsum where GroupType = 'DF1' and GroupCode = a.GroupCode), 0)
		end)
  from @t_group a
  
insert into @t_execsum
select 'DF2' as GoupType
     , a.GroupSeq
     , a.GroupCode
     , a.GroupName
     , DataCount = (
		case GroupCode
			when 'ALL' then
				isnull((select (sum(DataCount)/30.0) from @t_groupsum where GroupType = 'DF2'), 0)
			when 'OTHERS' then
				isnull((select (sum(DataCount)/30.0) from @t_groupsum where GroupType = 'DF2' and GroupCode not in (select GroupCode from @t_group)), 0)
			else 
				isnull((select (sum(DataCount)/30.0) from @t_groupsum where GroupType = 'DF2' and GroupCode = a.GroupCode), 0)
		end)
  from @t_group a
  
insert into @t_execsum
select 'DF3' as GoupType
     , a.GroupSeq
     , a.GroupCode
     , a.GroupName
     , DataCount = (
		case GroupCode
			when 'ALL' then
				isnull((select sum(DataCount) from @t_groupsum where GroupType = 'DF3'), 0)
			when 'OTHERS' then
				isnull((select sum(DataCount) from @t_groupsum where GroupType = 'DF3' and GroupCode not in (select GroupCode from @t_group)), 0)
			else 
				isnull((select sum(DataCount) from @t_groupsum where GroupType = 'DF3' and GroupCode = a.GroupCode), 0)
		end)
  from @t_group a


--delete PmDashboardData where DashboardName = 'PmExecutiveSummary2'
--insert into PmDashboardData 
--			  (DashboardName, GroupType, GroupSeq, GroupCode, GroupName, DataCount)
--	 select 'PmExecutiveSummary2', GroupType, GroupSeq, GroupCode, GroupName, DataCount 
--	   from @t_execsum

--update PmDashboardData 
--   set DataCount=@t_execsum.DataCount 
--  from @t_execsum
-- where PmDashboardData.GroupType=@t_execsum.GroupType
--   and PmDashboardData.GroupSeq =@t_execsum.GroupSeq
--   and PmDashboardData.GroupCode=@t_execsum.GroupCode
--   and PmDashboardData.GroupName=@t_execsum.GroupName

update PmDashboardData 
   set DataCount=(select DataCount from @t_execsum
                   where PmDashboardData.GroupType=GroupType
					 and PmDashboardData.GroupSeq =GroupSeq
					 and PmDashboardData.GroupCode=GroupCode
					 and PmDashboardData.GroupName=GroupName)
 where exists (select top 1 1 from @t_execsum
                where PmDashboardData.GroupType=GroupType
				  and PmDashboardData.GroupSeq =GroupSeq
				  and PmDashboardData.GroupCode=GroupCode
				  and PmDashboardData.GroupName=GroupName)

select * from PmDashboardData where DashboardName = 'PmExecutiveSummary2'

