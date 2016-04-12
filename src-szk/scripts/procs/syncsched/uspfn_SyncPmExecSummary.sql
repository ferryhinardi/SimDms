ALTER procedure [dbo].[uspfn_SyncPmExecSummary]
as

declare @t_groupsum as table (
    GroupCode varchar(10), 
    GroupName varchar(50),
    SpkCount  int
)

declare @t_execsum as table (
    FieldName varchar(20),
    FieldDesc varchar(220),
    FieldType varchar(10),
    InqValue1 int,
    InqValue2 int,
    InqValue3 numeric(10, 4),
    InqValAll int,
    SpkValue1 int,
    SpkValue2 int,
    SpkValue3 numeric(10, 4),
    Sequence  int
)

insert into @t_groupsum
select 'H', GroupModel, count(1) as SpkCount from (
    --di RUBAH dengan menghitung 1x SPK sesuai Update terakhir, 14 October 2014
    --select a.CompanyCode, a.BranchCode, a.InquiryNumber, c.GroupModel
    --  from suzukir4..pmStatusHistory a
    --  join suzukir4..pmHstITS b
    --    on b.CompanyCode = a.CompanyCode
    --   and b.BranchCode = a.BranchCode
    --   and b.InquiryNumber = a.InquiryNumber
    -- inner join suzukir4..MsMstGroupModel c
    --    on c.ModelType = b.TipeKendaraan
    -- where a.LastProgress = 'SPK'
    --   and not exists (select top 1 1 from suzukir4..pmStatusHistory 
    --                where CompanyCode=a.CompanyCode
    --                  and BranchCode=a.BranchCode
    --                  and InquiryNumber=a.InquiryNumber
    --                  and LastProgress in ('LOST','DO','DELIVERY'))
    select a.CompanyCode, a.BranchCode, a.InquiryNumber, c.GroupModel
      from suzukir4..pmHstITS a
     inner join suzukir4..MsMstGroupModel c
        on c.ModelType = a.TipeKendaraan
     where exists (select top 1 1 from suzukir4..pmStatusHistory
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
    ) x
 group by GroupModel

insert into @t_execsum (FieldName, FieldDesc, FieldType, Sequence, InqValAll)
select 'AllModel' as Name, 'All Model' as Caption, 'H', 1, (select sum(SpkCount) from @t_groupsum where GroupCode = 'H')
 union all
select 'Ertiga' as Name, 'Ertiga' as Caption, 'H', 2, isnull((select top 1 SpkCount from @t_groupsum where GroupCode = 'H' and GroupName = 'ERTIGA'), 0)
 union all
select 'WagonR' as Name, 'Karimun Wagon R' as Caption, 'H', 3, isnull((select top 1 SpkCount from @t_groupsum where GroupCode = 'H' and GroupName = 'WAGON R'), 0)
 union all
select 'Futura' as Name, 'Pick Up Futura' as Caption, 'H', 4, isnull((select top 1 SpkCount from @t_groupsum where GroupCode = 'H' and GroupName = 'SL415-PU'), 0)
 union all
select 'MCarry' as Name, 'Pick Up Mega Carry' as Caption, 'H', 5, isnull((select top 1 SpkCount from @t_groupsum where GroupCode = 'H' and GroupName = 'APV-PU'), 0)

declare @date1     datetime -- first date
declare @date2     datetime -- last date 
declare @date3     datetime -- last month date 
declare @dateCount int

set @date1 = left(convert(varchar, dateadd(month, -1, getdate()), 112), 6) + '01'
set @date2 = left(convert(varchar, getdate(), 112), 6) + '01'
set @date3 = convert(varchar, dateadd(day, -1, getdate()), 112)
set @dateCount = day(dateadd(day, -1, @date2))


insert into @t_groupsum
select 'D1', GroupModel, count(1) as SpkCount from (
    select b.CompanyCode, b.BranchCode, b.InquiryNumber, c.GroupModel
      from suzukir4..pmHstITS b
      left join suzukir4..MsMstGroupModel c
        on c.ModelType = b.TipeKendaraan
     where b.InquiryDate >= @date1 and InquiryDate < @date2
    ) x
 group by GroupModel

insert into @t_groupsum
select 'D2', GroupModel, count(1) as SpkCount from (
    select b.CompanyCode, b.BranchCode, b.InquiryNumber, c.GroupModel
      from suzukir4..pmHstITS b
      left join suzukir4..MsMstGroupModel c
        on c.ModelType = b.TipeKendaraan
     where convert(varchar, b.InquiryDate, 112) = convert(varchar, @date3, 112)
    ) x
 group by GroupModel

insert into @t_groupsum
select 'D3', GroupModel, count(1) as SpkCount from (
    --select a.CompanyCode, a.BranchCode, a.InquiryNumber, c.GroupModel
    --  from suzukir4..pmStatusHistory a
    --  join suzukir4..pmHstITS b
    --    on b.CompanyCode = a.CompanyCode
    --   and b.BranchCode = a.BranchCode
    --   and b.InquiryNumber = a.InquiryNumber
    --  left join suzukir4..MsMstGroupModel c
    --    on c.ModelType = b.TipeKendaraan
    -- where a.LastProgress = 'SPK'
    --   and a.UpdateDate >= @date1 and a.UpdateDate < @date2
	   --and a.UpdateDate < @date2
    --   and b.InquiryDate < @date2
  select a.CompanyCode, a.BranchCode, a.InquiryNumber, c.GroupModel
      from suzukir4..pmHstITS a
     inner join suzukir4..MsMstGroupModel c
        on c.ModelType = a.TipeKendaraan
     where convert(varchar,@date1,112) <= (select top 1 convert(varchar,isnull(UpdateDate,'1900/01/01'),112) 
                                                 from suzukir4..pmStatusHistory
                                                where CompanyCode=a.CompanyCode
                                                  and BranchCode=a.BranchCode
                                                  and InquiryNumber=a.InquiryNumber
                                                  and LastProgress = 'SPK'
                                                order by UpdateDate desc)
       and convert(varchar,@date2,112) >  (select top 1 convert(varchar,isnull(UpdateDate,'1900/01/01'),112) 
                                                 from suzukir4..pmStatusHistory
                                                where CompanyCode=a.CompanyCode
                                                  and BranchCode=a.BranchCode
                                                  and InquiryNumber=a.InquiryNumber
                                                  and LastProgress = 'SPK'
                                                order by UpdateDate desc)
	   and convert(varchar, a.InquiryDate, 112) < convert(varchar, @date2, 112)
    ) x
 group by GroupModel

insert into @t_groupsum
select 'D4', GroupModel, count(1) as SpkCount from (
    --select a.CompanyCode, a.BranchCode, a.InquiryNumber, c.GroupModel
    --  from suzukir4..pmStatusHistory a
    --  join suzukir4..pmHstITS b
    --    on b.CompanyCode = a.CompanyCode
    --   and b.BranchCode = a.BranchCode
    --   and b.InquiryNumber = a.InquiryNumber
    --  left join suzukir4..MsMstGroupModel c
    --    on c.ModelType = b.TipeKendaraan
    -- where a.LastProgress = 'SPK'
    --   and convert(varchar, a.UpdateDate, 112) = convert(varchar, getdate(), 112)
  select a.CompanyCode, a.BranchCode, a.InquiryNumber, c.GroupModel
      from suzukir4..pmHstITS a
     inner join suzukir4..MsMstGroupModel c
        on c.ModelType = a.TipeKendaraan
     where convert(varchar, getdate(), 112) = (select top 1 convert(varchar,isnull(UpdateDate,'1900/01/01'),112) 
                                                 from suzukir4..pmStatusHistory
                                                where CompanyCode=a.CompanyCode
                                                  and BranchCode=a.BranchCode
                                                  and InquiryNumber=a.InquiryNumber
                                                  and LastProgress = 'SPK'
                                                order by UpdateDate desc)
    ) x
 group by GroupModel

insert into @t_execsum (FieldName, FieldDesc, FieldType, InqValue1, InqValue2, InqValue3, SpkValue1, SpkValue2, SpkValue3, Sequence)
select 'AllModel' as Name, 'All Model' as Caption, 'D'
     , Val1 = (select (sum(SpkCount) / @dateCount) from @t_groupsum where GroupCode = 'D1')
     , Val2 = (select sum(SpkCount) from @t_groupsum where GroupCode = 'D2')
     , Val3 = 0
     , Val4 = (select (sum(SpkCount) / @dateCount) from @t_groupsum where GroupCode = 'D3')
     , Val5 = (select sum(SpkCount) from @t_groupsum where GroupCode = 'D4')
     , Val6 = 0
     , Val7 = 0

union all
select 'Ertiga' as Name, 'Ertiga' as Caption, 'D'
     , Val1 = (select (sum(SpkCount) / @dateCount) from @t_groupsum where GroupCode = 'D1' and GroupName = 'ERTIGA')
     , Val2 = (select sum(SpkCount) from @t_groupsum where GroupCode = 'D2' and GroupName = 'ERTIGA')
     , Val3 = 0
     , Val4 = (select (sum(SpkCount) / @dateCount) from @t_groupsum where GroupCode = 'D3' and GroupName = 'ERTIGA')
     , Val5 = (select sum(SpkCount) from @t_groupsum where GroupCode = 'D4' and GroupName = 'ERTIGA')
     , Val6 = 0
     , Val7 = 1

 union all
select 'WagonR' as Name, 'Karimun Wagon R' as Caption, 'D'
     , Val1 = (select (sum(SpkCount) / @dateCount)  from @t_groupsum where GroupCode = 'D1' and GroupName = 'WAGON R')
     , Val2 = (select sum(SpkCount) from @t_groupsum where GroupCode = 'D2' and GroupName = 'WAGON R')
     , Val3 = 0
     , Val4 = (select (sum(SpkCount) / @dateCount) from @t_groupsum where GroupCode = 'D3' and GroupName = 'WAGON R')
     , Val5 = (select sum(SpkCount) from @t_groupsum where GroupCode = 'D4' and GroupName = 'WAGON R')
     , Val6 = 0
     , Val7 = 2

 union all
select 'Futura' as Name, 'Pick Up Futura' as Caption, 'D'
     , Val1 = (select (sum(SpkCount) / @dateCount) from @t_groupsum where GroupCode = 'D1' and GroupName = 'SL415-PU')
     , Val2 = (select sum(SpkCount) from @t_groupsum where GroupCode = 'D2' and GroupName = 'SL415-PU')
     , Val3 = 0
     , Val4 = (select (sum(SpkCount) / @dateCount) from @t_groupsum where GroupCode = 'D3' and GroupName = 'SL415-PU')
     , Val5 = (select sum(SpkCount) from @t_groupsum where GroupCode = 'D4' and GroupName = 'SL415-PU')
     , Val6 = 0
     , Val7 = 3

 union all
select 'MCarry' as Name, 'Pick Up Mega Carry' as Caption, 'D'
     , Val1 = (select (sum(SpkCount) / @dateCount) from @t_groupsum where GroupCode = 'D1' and GroupName = 'APV-PU')
     , Val2 = (select sum(SpkCount) from @t_groupsum where GroupCode = 'D2' and GroupName = 'APV-PU')
     , Val3 = 0
     , Val4 = (select (sum(SpkCount) / @dateCount) from @t_groupsum where GroupCode = 'D3' and GroupName = 'APV-PU')
     , Val5 = (select sum(SpkCount) from @t_groupsum where GroupCode = 'D4' and GroupName = 'APV-PU')
     , Val6 = 0
     , Val7 = 4


update @t_execsum 
   set InqValue3 = (InqValue2 / (1.0 * InqValue1)) * 100
     , SpkValue3 = (SpkValue2 / (1.0 * SpkValue1)) * 100
 where FieldType = 'D'

--delete PmExecSummaryViewDash
--insert into PmExecSummaryViewDash (FieldName, FieldDesc, FieldType, InqValue1, InqValue2, InqValue3, InqValAll, SpkValue1, SpkValue2, SpkValue3, Sequence)
--select * from @t_execsum
update PmExecSummaryViewDash 
   set InqValue1  =(select InqValue1 from @t_execsum
                     where PmExecSummaryViewDash.FieldName=FieldName
				 	   and PmExecSummaryViewDash.FieldDesc =FieldDesc
					   and PmExecSummaryViewDash.FieldType=FieldType
					   and PmExecSummaryViewDash.Sequence=Sequence),
       InqValue2  =(select InqValue2 from @t_execsum
                     where PmExecSummaryViewDash.FieldName=FieldName
				 	   and PmExecSummaryViewDash.FieldDesc =FieldDesc
					   and PmExecSummaryViewDash.FieldType=FieldType
					   and PmExecSummaryViewDash.Sequence=Sequence),
       InqValue3  =(select InqValue3 from @t_execsum
                     where PmExecSummaryViewDash.FieldName=FieldName
				 	   and PmExecSummaryViewDash.FieldDesc =FieldDesc
					   and PmExecSummaryViewDash.FieldType=FieldType
					   and PmExecSummaryViewDash.Sequence=Sequence),
       InqValAll  =(select InqValAll from @t_execsum
                     where PmExecSummaryViewDash.FieldName=FieldName
				  	   and PmExecSummaryViewDash.FieldDesc =FieldDesc
					   and PmExecSummaryViewDash.FieldType=FieldType
					   and PmExecSummaryViewDash.Sequence=Sequence),
       SpkValue1  =(select SpkValue1 from @t_execsum
                     where PmExecSummaryViewDash.FieldName=FieldName
				  	   and PmExecSummaryViewDash.FieldDesc =FieldDesc
					   and PmExecSummaryViewDash.FieldType=FieldType
					   and PmExecSummaryViewDash.Sequence=Sequence),
       SpkValue2  =(select SpkValue2 from @t_execsum
                     where PmExecSummaryViewDash.FieldName=FieldName
				  	   and PmExecSummaryViewDash.FieldDesc =FieldDesc
					   and PmExecSummaryViewDash.FieldType=FieldType
					   and PmExecSummaryViewDash.Sequence=Sequence),
       SpkValue3  =(select SpkValue3 from @t_execsum
                     where PmExecSummaryViewDash.FieldName=FieldName
				  	   and PmExecSummaryViewDash.FieldDesc =FieldDesc
					   and PmExecSummaryViewDash.FieldType=FieldType
					   and PmExecSummaryViewDash.Sequence=Sequence)
 where exists (select top 1 1 from @t_execsum
                     where PmExecSummaryViewDash.FieldName=FieldName
				  	   and PmExecSummaryViewDash.FieldDesc =FieldDesc
					   and PmExecSummaryViewDash.FieldType=FieldType
					   and PmExecSummaryViewDash.Sequence=Sequence)

select * from PmExecSummaryViewDash

