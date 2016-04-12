CREATE procedure [dbo].[uspfn_SvDrhInitialWeb]
    @CompanyCode varchar(20),
    @BranchCode  varchar(20),
    @DateParam   datetime,
    @OptionType  varchar(20),
    @Interval    int,
    @Range       int,
    @InclPdi     bit = 0,
    @UserID      varchar(20) = '',
	@IsOdom		 bit = 0

as

--declare
--	@CompanyCode varchar(15),
--	@BranchCode varchar(15),
--    @DateParam   datetime,
--    @OptionType  varchar(20),
--    @Interval    int,
--    @Range       int,
--    @InclPdi     bit = 0,
--	@IsOdom		 bit

--	set @CompanyCode = '6006400001'
--    set @BranchCode    = '6006400105'
--	set @DateParam = '2015-9-01'
--	set @Interval = 0
--	set @range = 3
--	set @IsOdom = 1
--	set @OptionType = '4FOLLOWUP'

if (@DateParam is null or convert(varchar, @DateParam, 112) <= 19000101) set @DateParam = getdate()

if @OptionType = 'REMINDER'
begin
    ----===============================================================================
    ---- INSERT DAILY RETENTION WITH ASSUMPTIONS NOT YET REGISTERED IN RETENTION SYSTEM
    ----===============================================================================

select * into #t1 from(
select a.CompanyCode, a.BranchCode
         , year(a.JobOrderDate) PeriodYear
         , month(a.JobOrderDate) PeriodMonth
         , a.CustomerCode, a.ChassisCode, a.ChassisNo
         , case when isnull((
            select BPKDate from OmMstVehicle
             where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode
               and ChassisCode = a.ChassisCode and ChassisNo = a.ChassisNo)
            , null) is null then 20 else 10 end CustomerCategory
         , case when a.BookingNo <> '' then 10 else 30 end VisitInitial
         , a.JobOrderDate VisitDate
         , case when a.Odometer < 2000 then 1000 else round(a.odometer/5000,0) * 5000 end as PMNow
         , case when a.Odometer < 2000 then 5000 else (round(a.odometer/5000,0) * 5000) + 5000 end as PMNext
         , case when isnull((select top 1 TimeDim from SvMstRoutineService where CompanyCode = a.CompanyCode),'') = 'D' then
                dateadd(day, isnull(
                   (
                        select top 1 TimePeriod 
                        from SvMstRoutineService 
                        where CompanyCode = a.CompanyCode 
                            and Odometer = case when a.Odometer < 2000 then 5000 else (round(a.Odometer/5000,0)                                                                    * 5000) + 5000 END
                   ) - 
                   (
                        select TimePeriod 
                        from SvMstRoutineService 
                        where CompanyCode = a.CompanyCode 
                            and JobType = a.JobType 
                    )
                    ,0),a.JobOrderDate)
            else case when isnull((select top 1 TimeDim from SvMstRoutineService where CompanyCode = a.CompanyCode),'') = 'M' then
                dateadd(month, isnull(
                   (
                        select top 1 TimePeriod 
                        from SvMstRoutineService 
                        where CompanyCode = a.CompanyCode 
                            and Odometer = case when a.Odometer < 2000 THEN 5000 else (ROUND(a.Odometer/5000,0) * 5000) + 5000 END
                   ) - 
                   (
                        select TimePeriod 
                        from SvMstRoutineService 
                        where CompanyCode = a.CompanyCode 
                            and JobType = a.JobType 
                   )
            ,0),a.JobOrderDate)
                else 
                dateadd(year, isnull(
                   (
                        select top 1 TimePeriod 
                        from SvMstRoutineService 
                        where CompanyCode = a.CompanyCode 
                            and Odometer = case when a.Odometer < 2000 THEN 5000 else (ROUND(a.Odometer/5000,0) * 5000) + 5000 END
                   ) - 
                   (
                        select TimePeriod 
                        from SvMstRoutineService 
                        where CompanyCode = a.CompanyCode 
                            and JobType = a.JobType 
                   )
            ,0),a.JobOrderDate) end end 
           as EstimationNextVisit
         , null ReminderDate
         , convert(bit, 0) IsConfirmed
         , convert(bit, 0) IsBooked
         , convert(bit, 0) IsVisited
         , convert(bit, 0) IsSatisfied
         , a.BookingDate
         , null FollowUpDate
         , '' Reason
         , case when isnull((select BPKDate from OmMstVehicle where CompanyCode = a.CompanyCode 
                and BranchCode = a.BranchCode and ChassisCode = a.ChassisCode 
                and ChassisNo = a.ChassisNo), NULL) IS NULL  
            then null else (select BPKDate from OmMstVehicle where CompanyCode = a.CompanyCode 
                and BranchCode = a.BranchCode and ChassisCode = a.ChassisCode and ChassisNo = a.ChassisNo)
             end RefferenceDate
         , (select LastServiceDate from SvMstCustomerVehicle where CompanyCode = a.CompanyCode 
                and BranchCode = a.BranchCode and ChassisCode = a.ChassisCode 
                and ChassisNo = a.ChassisNo) LastServiceDate
         , a.ServiceRequestDesc LastRemark
         , @UserID CreatedBy
         , GetDate() CreatedDate
         , @UserID LastUpdateBy
         , GetDate() LastUpdateDate
		 , a.ServiceNo
      from svTrnService a
      left join SvMstCustomerVehicle b
        on b.Companycode = a.CompanyCode 
       and b.ChassisCode = a.ChassisCode 
       and b.ChassisNo = a.ChassisNo
      left join GnMstCustomer c
        on c.CompanyCode = a.CompanyCode
       and c.CustomerCode = a.CustomerCode
     where 1 = 1
       and a.CompanyCode = @CompanyCode
       and a.BranchCode    = @BranchCode
       and ((
		convert(varchar, @DateParam, 112) between
        convert(varchar, dateadd(day, -@Interval, dateadd(month, @Range, JobOrderDate)), 112) and
         convert(varchar, dateadd(day, @Interval, dateadd(month, @Range, JobOrderDate)), 112)
		) or (convert(varchar, @DateParam, 112) = convert(varchar, dateadd(month, 1, JobOrderDate), 112)))
       and a.ServiceStatus in ('7','9')
       and not exists (
            select * from svTrnDailyRetention d
             where d.CompanyCode = a.CompanyCode
               and d.BranchCode = a.BranchCode
               and d.CustomerCode = a.CustomerCode
               and d.PeriodYear = year(a.JobOrderDate)
               and d.PeriodMonth = month(a.JobOrderDate)
            )
			and a.ServiceType = 2
			)#t1

;WITH _Service AS (
    SELECT *, ROW_NUMBER()
    over (
        PARTITION BY ChassisCode, ChassisNo 
        order by ServiceNo desc
    ) AS RowNo 
    FROM svtrnservice srv
	where srv.BranchCode = @BranchCode and srv.ChassisNo in (select ChassisNo from #t1) and ServiceType = 2 
), _row1 as(
SELECT RowNo, ServiceNo, Odometer,JobOrderDate, ChassisCode, ChassisNo, CustomerCode 
FROM _Service WHERE RowNo = 1
), _row2 as(
SELECT RowNo, ServiceNo, Odometer,JobOrderDate, ChassisCode, ChassisNo, CustomerCode 
FROM _Service WHERE RowNo = 2
), _row3 as(
SELECT RowNo, ServiceNo, Odometer,JobOrderDate, ChassisCode, ChassisNo, CustomerCode 
FROM _Service WHERE RowNo = 3
), _odom as
(
select  
--((_row1.Odometer + _row2.Odometer) / (DATEDIFF(day,_row2.JobOrderDate , _row1.JobOrderDate) +  DATEDIFF(day,_row3.JobOrderDate , _row2.JobOrderDate))) OdomDays,
DATEADD(Day,((_row1.Odometer + _row2.Odometer) / (DATEDIFF(day,_row2.JobOrderDate , _row1.JobOrderDate) +  DATEDIFF(day,_row3.JobOrderDate , _row2.JobOrderDate))), _row1.JobOrderDate) OdomDate,
 _row1.ChassisNo, _row1.ChassisCode, _row1.ServiceNo
from _row1, _row2, _row3
where _row1.ChassisNo = _row2.ChassisNo and _row2.ChassisNo = _row3.ChassisNo
)

insert into SvTrnDailyRetention (CompanyCode,BranchCode,PeriodYear,PeriodMonth
               ,CustomerCode,ChassisCode,ChassisNo,CustomerCategory,VisitInitial
               ,VisitDate,PMNow,PMNext,EstimationNextVisit,ReminderDate,IsConfirmed
               ,IsBooked,IsVisited,IsSatisfied,BookingDate,FollowUpDate,Reason
               ,RefferenceDate,LastServiceDate,LastRemark,CreatedBy
               ,CreatedDate,LastUpdateBy,LastUpdateDate)
select CompanyCode,BranchCode,PeriodYear,PeriodMonth
               ,CustomerCode,#t1.ChassisCode,#t1.ChassisNo,CustomerCategory,VisitInitial
               ,VisitDate,PMNow,PMNext
			   , case when @IsOdom = 0 then case when EstimationNextVisit < _odom.OdomDate then EstimationNextVisit else _odom.OdomDate end ELSE EstimationNextVisit end EstimationNextVisit
			   ,ReminderDate,IsConfirmed
               ,IsBooked,IsVisited,IsSatisfied,BookingDate,FollowUpDate,Reason
               ,RefferenceDate,LastServiceDate,LastRemark,CreatedBy
               ,CreatedDate,LastUpdateBy,LastUpdateDate
from #t1 
left join _odom
on  #t1.ChassisNo = _odom.ChassisNo and #t1.ChassisCode = _odom.ChassisCode and #t1.ServiceNo = _odom.ServiceNo

drop table #t1
end
if @OptionType = '4FOLLOWUP'
begin
    ----===============================================================================
    ---- INSERT DAILY RETENTION SERVICE
    ----===============================================================================
   
select * into #t2 from(
select a.CompanyCode, a.BranchCode
         , year(a.JobOrderDate) PeriodYear
         , month(a.JobOrderDate) PeriodMonth
         , a.CustomerCode, a.ChassisCode, a.ChassisNo
         , case when isnull((
            select BPKDate from OmMstVehicle
             where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode
               and ChassisCode = a.ChassisCode and ChassisNo = a.ChassisNo)
            , null) is null then 20 else 10 end CustomerCategory
         , case when a.BookingNo <> '' then 10 else 30 end VisitInitial
         , a.JobOrderDate VisitDate
         , case when a.Odometer < 2000 then 1000 else round(a.odometer/5000,0) * 5000 end as PMNow
         , case when a.Odometer < 2000 then 5000 else (round(a.odometer/5000,0) * 5000) + 5000 end as PMNext
         , case when isnull((select top 1 TimeDim from SvMstRoutineService where CompanyCode = a.CompanyCode),'') = 'D' then
                dateadd(day, isnull(
                   (
                        select top 1 TimePeriod 
                        from SvMstRoutineService 
                        where CompanyCode = a.CompanyCode 
                            and Odometer = case when a.Odometer < 2000 then 5000 else (round(a.Odometer/5000,0)                                                                    * 5000) + 5000 END

                   ) - 
                   (
                        select TimePeriod 
                        from SvMstRoutineService 
                        where CompanyCode = a.CompanyCode 
                            and JobType = a.JobType 
                    )
                    ,0),a.JobOrderDate)
            else case when isnull((select top 1 TimeDim from SvMstRoutineService where CompanyCode = a.CompanyCode),'') = 'M' then
                dateadd(month, isnull(
                   (
                        select top 1 TimePeriod 
                        from SvMstRoutineService 
                        where CompanyCode = a.CompanyCode 
                            and Odometer = case when a.Odometer < 2000                                                                    THEN 5000 else (ROUND(a.Odometer/5000,0)                                                                    * 5000) + 5000 END

                   ) - 
                   (
                        select TimePeriod 
                        from SvMstRoutineService 
                        where CompanyCode = a.CompanyCode 
                            and JobType = a.JobType 
                    )
            ,0),a.JobOrderDate)
                else 
                dateadd(year, isnull(
                   (
                        select top 1 TimePeriod 
                        from SvMstRoutineService 
                        where CompanyCode = a.CompanyCode 
                            and Odometer = case when a.Odometer < 2000                                                                    THEN 5000 else (ROUND(a.Odometer/5000,0)                                                                    * 5000) + 5000 END

                   ) - 
                   (
                        select TimePeriod 
                        from SvMstRoutineService 
                        where CompanyCode = a.CompanyCode 
                            and JobType = a.JobType 
                    )
            ,0),a.JobOrderDate) end end 
           as  EstimationNextVisit
         , null ReminderDate
         , convert(bit, 0) IsConfirmed
         , convert(bit, 0) IsBooked
         , convert(bit, 0) IsVisited
         , convert(bit, 0) IsSatisfied
         , a.BookingDate
         , null FollowUpDate
         , '' Reason
         , case when isnull((select BPKDate from OmMstVehicle where CompanyCode = a.CompanyCode 
                and BranchCode = a.BranchCode and ChassisCode = a.ChassisCode 
                and ChassisNo = a.ChassisNo), NULL) IS NULL  
            then null else (select BPKDate from OmMstVehicle where CompanyCode = a.CompanyCode 
                and BranchCode = a.BranchCode and ChassisCode = a.ChassisCode and ChassisNo = a.ChassisNo)
             end RefferenceDate
         , (select LastServiceDate from SvMstCustomerVehicle where CompanyCode = a.CompanyCode 
                and BranchCode = a.BranchCode and ChassisCode = a.ChassisCode 
                and ChassisNo = a.ChassisNo) LastServiceDate
         , a.ServiceRequestDesc LastRemark
         , @UserID CreatedBy
         , GetDate() CreatedDate
         , @UserID LastUpdateBy
         , GetDate() LastUpdateDate
		 , a.ServiceNo
      from svTrnService a
      left join SvMstCustomerVehicle b
        on b.Companycode = a.CompanyCode 
       and b.ChassisCode = a.ChassisCode 
       and b.ChassisNo = a.ChassisNo
      left join GnMstCustomer c
        on c.CompanyCode = a.CompanyCode
       and c.CustomerCode = a.CustomerCode
     where 1 = 1
       and a.CompanyCode = @CompanyCode
       and a.BranchCode = @BranchCode
       and ((convert(varchar, @DateParam, 112) = convert(varchar, dateadd(day, @Interval, a.JobOrderDate), 112)
		) or (convert(varchar, @DateParam, 112) = convert(varchar, dateadd(month, 1, JobOrderDate), 112)))
       and a.ServiceStatus in ('7','9')
       --and a.ServiceStatus not in ('0','1','2','3','4','5','6')
       and not exists (
            select * from svTrnDailyRetention d
             where d.CompanyCode = a.CompanyCode
               and d.BranchCode = a.BranchCode
               and d.CustomerCode = a.CustomerCode
               and d.PeriodYear = year(a.JobOrderDate)
               and d.PeriodMonth = month(a.JobOrderDate)
            )
			)#t2

;WITH _Service AS (
    SELECT *, ROW_NUMBER()
    over (
        PARTITION BY ChassisCode, ChassisNo 
        order by ServiceNo desc
    ) AS RowNo 
    FROM svtrnservice srv
	where srv.BranchCode = @BranchCode and srv.ChassisNo in (select ChassisNo from #t2) and ServiceType = 2 
), _row1 as(
SELECT RowNo, ServiceNo, Odometer,JobOrderDate, ChassisCode, ChassisNo, CustomerCode 
FROM _Service WHERE RowNo = 1
), _row2 as(
SELECT RowNo, ServiceNo, Odometer,JobOrderDate, ChassisCode, ChassisNo, CustomerCode 
FROM _Service WHERE RowNo = 2
), _row3 as(
SELECT RowNo, ServiceNo, Odometer,JobOrderDate, ChassisCode, ChassisNo, CustomerCode 
FROM _Service WHERE RowNo = 3
), _odom as
(
select  
--((_row1.Odometer + _row2.Odometer) / (DATEDIFF(day,_row2.JobOrderDate , _row1.JobOrderDate) +  DATEDIFF(day,_row3.JobOrderDate , _row2.JobOrderDate))) OdomDays,
DATEADD(Day,((_row1.Odometer + _row2.Odometer) / (DATEDIFF(day,_row2.JobOrderDate , _row1.JobOrderDate) +  DATEDIFF(day,_row3.JobOrderDate , _row2.JobOrderDate))), _row1.JobOrderDate) OdomDate,
 _row1.ChassisNo, _row1.ChassisCode, _row1.ServiceNo
from _row1, _row2, _row3
where _row1.ChassisNo = _row2.ChassisNo and _row2.ChassisNo = _row3.ChassisNo
)

insert into SvTrnDailyRetention (CompanyCode,BranchCode,PeriodYear,PeriodMonth
               ,CustomerCode,ChassisCode,ChassisNo,CustomerCategory,VisitInitial
               ,VisitDate,PMNow,PMNext,EstimationNextVisit,ReminderDate,IsConfirmed
               ,IsBooked,IsVisited,IsSatisfied,BookingDate,FollowUpDate,Reason
               ,RefferenceDate,LastServiceDate,LastRemark,CreatedBy
               ,CreatedDate,LastUpdateBy,LastUpdateDate)
select CompanyCode,BranchCode,PeriodYear,PeriodMonth
               ,CustomerCode,#t2.ChassisCode,#t2.ChassisNo,CustomerCategory,VisitInitial
               ,VisitDate,PMNow,PMNext
			   , case when @IsOdom = 0 then case when EstimationNextVisit < _odom.OdomDate then EstimationNextVisit else _odom.OdomDate end ELSE EstimationNextVisit end EstimationNextVisit
			   ,ReminderDate,IsConfirmed
               ,IsBooked,IsVisited,IsSatisfied,BookingDate,FollowUpDate,Reason
               ,RefferenceDate,LastServiceDate,LastRemark,CreatedBy
               ,CreatedDate,LastUpdateBy,LastUpdateDate
from #t2 
left join _odom
on  #t2.ChassisNo = _odom.ChassisNo and #t2.ChassisCode = _odom.ChassisCode and #t2.ServiceNo = _odom.ServiceNo

drop table #t2
end


