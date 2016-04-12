ALTER procedure [dbo].[uspfn_SvDrhInitial]
    @CompanyCode varchar(20),
    @BranchCode  varchar(20),
    @DateParam   datetime,
    @OptionType  varchar(20),
    @Interval    int,
    @Range       int,
    @InclPdi     bit = 0,
    @UserID      varchar(20) = ''
as

if (@DateParam is null or convert(varchar, @DateParam, 112) <= 19000101) set @DateParam = getdate()

if @OptionType = 'REMINDER'
begin
    ----===============================================================================
    ---- INSERT DAILY RETENTION WITH ASSUMPTIONS NOT YET REGISTERED IN RETENTION SYSTEM
    ----===============================================================================
    insert into SvTrnDailyRetention (CompanyCode,BranchCode,PeriodYear,PeriodMonth
               ,CustomerCode,ChassisCode,ChassisNo,CustomerCategory,VisitInitial
               ,VisitDate,PMNow,PMNext,EstimationNextVisit,ReminderDate,IsConfirmed
               ,IsBooked,IsVisited,IsSatisfied,BookingDate,FollowUpDate,Reason
               ,RefferenceDate,LastServiceDate,LastRemark,CreatedBy
               ,CreatedDate,LastUpdateBy,LastUpdateDate)
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
       and not exists (
            select * from svTrnDailyRetention d
             where d.CompanyCode = a.CompanyCode
               and d.BranchCode = a.BranchCode
               and d.CustomerCode = a.CustomerCode
               and d.PeriodYear = year(a.JobOrderDate)
               and d.PeriodMonth = month(a.JobOrderDate)
            )
end
if @OptionType = '4FOLLOWUP'
begin
    ----===============================================================================
    ---- INSERT DAILY RETENTION SERVICE
    ----===============================================================================
    insert into SvTrnDailyRetention (CompanyCode,BranchCode,PeriodYear,PeriodMonth
               ,CustomerCode,ChassisCode,ChassisNo,CustomerCategory,VisitInitial
               ,VisitDate,PMNow,PMNext,EstimationNextVisit,ReminderDate,IsConfirmed
               ,IsBooked,IsVisited,IsSatisfied,BookingDate,FollowUpDate,Reason
               ,RefferenceDate,LastServiceDate,LastRemark,CreatedBy
               ,CreatedDate,LastUpdateBy,LastUpdateDate)
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
       --and a.ServiceStatus not in ('0','1','2','3','4','5','6')
       and not exists (
            select * from svTrnDailyRetention d
             where d.CompanyCode = a.CompanyCode
               and d.BranchCode = a.BranchCode
               and d.CustomerCode = a.CustomerCode
               and d.PeriodYear = year(a.JobOrderDate)
               and d.PeriodMonth = month(a.JobOrderDate)
            )
end