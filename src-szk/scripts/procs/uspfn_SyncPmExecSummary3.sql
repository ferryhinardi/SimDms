alter procedure uspfn_SyncPmExecSummary3
as

declare @t_group as table (
    GroupSeq  int, 
    GroupCode varchar(20), 
    GroupName varchar(50)
)

insert into @t_group values (1, 'ERTIGA', 'Ertiga')
insert into @t_group values (2, 'WAGON R', 'Karimun Wagon R')
insert into @t_group values (3, 'SL415-PU', 'PU Futura')
--insert into @t_group values (3, 'ST100-PU', 'PU Futura')
insert into @t_group values (4, 'APV-PU', 'PU Mega Carry')
insert into @t_group values (5, 'OTHERS', 'Others')
insert into @t_group values (6, 'ALL', 'All Model')



declare
   @M0 varchar(6),
   @M1 varchar(6),
   @M2 varchar(6)

set @M0 = left(convert(varchar,getdate(),112),6)
set @M1 = left(convert(varchar,DATEADD(mm,-1,getdate()),112),6)
set @M2 = left(convert(varchar,DATEADD(mm,-2,getdate()),112),6)

declare @t_dash as table (
	GroupType varchar(20),
	Description varchar(220),
	Model varchar(50),
	Total int
)

insert into @t_dash
select * 
    from ( 
        select 'C3' as GroupType, '1. INDENT (M)' Description, 
               Model = case when isnull(m.GroupModel,' ') in ('ERTIGA','WAGON R','SL415-PU','APV-PU')
                            then isnull(m.GroupModel,' ') else 'OTHERS' end,
               count(*) Total
            from SuzukiR4..pmHstITS i
            inner join SuzukiR4..pmStatusHistory h
                    on h.CompanyCode  =i.CompanyCode
                and h.BranchCode   =i.BranchCode
                and h.InquiryNumber=i.InquiryNumber
                and h.LastProgress ='SPK'
                and left(convert(varchar,h.UpdateDate,112),6)=@M0
            left join SuzukiR4..MsMstGroupModel m
                    on m.ModelType    =i.TipeKendaraan
            where not exists (select 1 from SuzukiR4..pmStatusHistory
                            where CompanyCode  =h.CompanyCode
                                and BranchCode   =h.BranchCode
                                and InquiryNumber=h.InquiryNumber
                                and LastProgress in ('DO','DELIVERY','LOST'))
            group by case when isnull(m.GroupModel,' ') in ('ERTIGA','WAGON R','SL415-PU','APV-PU')
                          then isnull(m.GroupModel,' ') else 'OTHERS' end 
    UNION ALL            
        select 'C2' as GroupType, '2. INDENT (M-1)' Description, 
               case when isnull(m.GroupModel,' ') in ('ERTIGA','WAGON R','SL415-PU','APV-PU')
                          then isnull(m.GroupModel,' ') else 'OTHERS' end,
               count(*) Total
            from SuzukiR4..pmHstITS i
            inner join SuzukiR4..pmStatusHistory h
                    on h.CompanyCode  =i.CompanyCode
                and h.BranchCode   =i.BranchCode
                and h.InquiryNumber=i.InquiryNumber
                and h.LastProgress ='SPK'
                and left(convert(varchar,h.UpdateDate,112),6)=@M1
            left join SuzukiR4..MsMstGroupModel m
                    on m.ModelType    =i.TipeKendaraan
            where not exists (select 1 from SuzukiR4..pmStatusHistory
                            where CompanyCode  =h.CompanyCode
                                and BranchCode   =h.BranchCode
                                and InquiryNumber=h.InquiryNumber
                                and LastProgress in ('DO','DELIVERY','LOST'))
                                --and left(convert(varchar,UpdateDate,112),6)=@M1)
            group by case when isnull(m.GroupModel,' ') in ('ERTIGA','WAGON R','SL415-PU','APV-PU')
                          then isnull(m.GroupModel,' ') else 'OTHERS' end 
    UNION ALL            
        select 'C1' as GroupType, '3. INDENT (M-2)' Description, 
               case when isnull(m.GroupModel,' ') in ('ERTIGA','WAGON R','SL415-PU','APV-PU')
                          then isnull(m.GroupModel,' ') else 'OTHERS' end, 
               count(*) Total
            from SuzukiR4..pmHstITS i
            inner join SuzukiR4..pmStatusHistory h
                    on h.CompanyCode  =i.CompanyCode
                and h.BranchCode   =i.BranchCode
                and h.InquiryNumber=i.InquiryNumber
                and h.LastProgress ='SPK'
                and left(convert(varchar,h.UpdateDate,112),6)<=@M2
            left join SuzukiR4..MsMstGroupModel m
                    on m.ModelType    =i.TipeKendaraan
            where not exists (select 1 from SuzukiR4..pmStatusHistory
                            where CompanyCode  =h.CompanyCode
                                and BranchCode   =h.BranchCode
                                and InquiryNumber=h.InquiryNumber
                                and LastProgress in ('DO','DELIVERY','LOST'))
                                --and left(convert(varchar,UpdateDate,112),6)<=@M2)
            group by case when isnull(m.GroupModel,' ') in ('ERTIGA','WAGON R','SL415-PU','APV-PU')
                          then isnull(m.GroupModel,' ') else 'OTHERS' end 
) a
order by Description, Model

delete PmDashboardData where DashboardName = 'PmExecutiveSummary3'
insert into PmDashboardData (DashboardName, GroupType, GroupSeq, GroupCode, GroupName, DataCount)
select 'PmExecutiveSummary3' as DashboardName, a.GroupType, b.GroupSeq
     , a.Model as GroupCode, a.Model as GroupName, a.Total as DataCount
  from @t_dash a
  join @t_group b
    on b.GroupCode = a.Model

if (select sum(DataCount) from PmDashboardData where DashboardName='PmExecutiveSummary2' and GroupType='H') > 0
     begin
        ;with x as (select * from PmDashBoardData where DashboardName = 'PmExecutiveSummary3' and GroupType='C1')
        update x 
           set DataCount =(select isnull(DataCount,0) from PmDashboardData 
                            where DashboardName='PmExecutiveSummary2' and GroupType='H' and GroupCode=x.GroupCode)
			             -(select sum(isnull(DataCount,0)) from PmDashBoardData 
			                where DashboardName='PmExecutiveSummary3' and GroupType<>'C1' and GroupCode=x.GroupCode)
         where exists (select 1 from PmDashBoardData
                        where DashboardName='PmExecutiveSummary2' and GroupType='H' and GroupCode=x.GroupCode)
     end


select * from PmDashboardData where DashboardName = 'PmExecutiveSummary3'
