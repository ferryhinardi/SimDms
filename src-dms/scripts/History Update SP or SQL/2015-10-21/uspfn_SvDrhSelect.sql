ALTER procedure [dbo].[uspfn_SvDrhSelect]  
 --DECLARE
    @CompanyCode varchar(20),  
    @BranchCode  varchar(20),  
    @DateParam   datetime,  
    @OptionType  varchar(20),  
    @Interval    int,  
    @Range       int,  
    @InclPdi     bit = 0

as  

--SELECT @CompanyCode= '6145203',@BranchCode= '614520301', @DateParam= '20140110',@OptionType= '4FOLLOWUP',@Interval= 3,@Range= -1,@InclPdi= 0--,@UserID= 'ga'
    
if (@DateParam is null or convert(varchar, @DateParam, 112) <= 19000101)  
    set @DateParam = getdate()  
  
if @OptionType = 'REMINDER'  
begin  
    ----===============================================================================  
    ---- SELECT DAILY RETENTION SERVICE  
    ----===============================================================================  
 select distinct d.RetentionNo, a.CompanyCode, a.BranchCode, isnull(d.PeriodYear, -1) PeriodYear, isnull(d.PeriodMonth, -1) PeriodMonth  
   , a.CustomerCode, c.CustomerName, a.JobOrderNo, b.LastServiceDate  
   , case when (convert(varchar, a.JobOrderDate, 112) = '19000101') then null else a.JobOrderDate end JobOrderDate  
   , a.BasicModel, b.ChassisCode, b.ChassisNo, b.PoliceRegNo, a.JobType, a.ServiceRequestDesc Remark  
   , case when (convert(varchar, d.ReminderDate, 112) = '19000101') then null else d.ReminderDate end ReminderDate  
   , case when (convert(varchar, d.BookingDate, 112) = '19000101') then null else d.BookingDate end BookingDate  
   , case when (convert(varchar, d.FollowUpDate, 112) = '19000101') then null else d.FollowUpDate end FollowUpDate  
   , convert(varchar(20), case d.IsConfirmed when null then '' when 0 then 'Tidak' when 1 then 'YA' end) IsConfirmed  
   , convert(varchar(20), case d.IsBooked when null then '' when 0 then 'Tidak' when 1 then 'YA' end) IsBooked  
   , convert(varchar(20), case d.IsVisited when null then '' when 0 then 'Tidak' when 1 then 'YA' end) IsVisited  
   , convert(varchar(20), case d.IsSatisfied when null then '' when 0 then 'Tidak' when 1 then 'YA' end) IsSatisfied  
   , d.Reason
   , case isnull(d.IsVisited, 0) when 0 then '' else d.VisitInitial end VisitInitial
   , case isnull(d.IsVisited, 0) when 0 then '' else e.LookupValueName end VisitInitialDesc
   , isnull(c.Address1, '')+isnull(c.Address2, '')+isnull(c.Address3, '')+isnull(c.Address4, '') Address  
   , case isnull(b.ContactName, '') when '' then c.CustomerName else rtrim(b.ContactName) end ContactName  
   , case isnull(b.ContactAddress, '') when '' then (isnull(c.Address1, '')+isnull(c.Address2, '')+isnull(c.Address3, '')+isnull(c.Address4, '')) else b.ContactAddress end ContactAddress  
   , case isnull(b.ContactPhone, '') when '' then c.PhoneNo else b.ContactPhone end ContactPhone  
   , c.PhoneNo, d.StatisfyReasonGroup, d.StatisfyReasonCode    
   , convert(varchar(20), case d.IsReminder when null then '' when 0 then 'Tidak' when 1 then 'YA' end) IsReminder    
   , convert(varchar(20), case d.IsFollowUp when null then '' when 0 then 'Tidak' when 1 then 'YA' end) IsFollowUp
   , b.TransmissionType  
   , a.Odometer, c.HPNo as MobilePhone  
  from svTrnService a  
  left join SvMstCustomerVehicle b  
    on b.Companycode = a.CompanyCode   
   and b.ChassisCode = a.ChassisCode   
   and b.ChassisNo = a.ChassisNo  
  left join GnMstCustomer c  
    on c.CompanyCode = a.CompanyCode  
   and c.CustomerCode = a.CustomerCode  
  left join svTrnDailyRetention d  
    on d.CompanyCode = a.CompanyCode  
   and d.BranchCode = a.BranchCode and d.PeriodYear = year(a.JobOrderDate)  
   and d.PeriodMonth = month(a.JobOrderDate) and d.CustomerCode = a.CustomerCode  
   and d.ChassisCode = a.ChassisCode and d.ChassisNo = a.ChassisNo  
  left join gnMstLookupDtl e on e.CompanyCode = a.CompanyCode  
   and e.CodeID = 'CIRS'  
   and e.LookupValue = d.VisitInitial  
 where 1 = 1  
   and a.CompanyCode = @CompanyCode  
   and a.BranchCode  = @BranchCode  
   and convert(varchar, @DateParam, 112)  
	 between convert(varchar, dateadd(day, -@Interval, dateadd(month, @Range, a.JobOrderDate)), 112)  
	     and convert(varchar, dateadd(day, @Interval, dateadd(month, @Range, a.JobOrderDate)), 112)  
   and a.JobType <> (case @InclPDI when 1 then '' else 'PDI' end)
   and (b.LastServiceDate is null or convert(varchar, a.JobOrderDate, 112) >= convert(varchar, b.LastServiceDate, 112))  
   and d.RetentionNo = isnull((
						select max(RetentionNo)
						  from svTrnDailyRetention
						 where CompanyCode = a.CompanyCode
						   and BranchCode = a.BranchCode
						   and PeriodYear = year(a.JobOrderDate)  
						   and PeriodMonth = month(a.JobOrderDate)
						   and CustomerCode = a.CustomerCode
					   ),0)  

   
end  
if @OptionType = '4FOLLOWUP'  
begin  
    ----===============================================================================  
    ---- SELECT DAILY RETENTION SERVICE  
    ----===============================================================================  
    select distinct a.CompanyCode, a.BranchCode, isnull(d.PeriodYear, -1) PeriodYear, isnull(d.PeriodMonth, -1) PeriodMonth 
      , d.RetentionNo
      , a.CustomerCode, c.CustomerName, a.JobOrderNo, b.LastServiceDate  
      , case when (convert(varchar, a.JobOrderDate, 112) = '19000101') then null else a.JobOrderDate end JobOrderDate     
      , a.BasicModel, b.ChassisCode, b.ChassisNo, b.PoliceRegNo, a.JobType, a.ServiceRequestDesc Remark       
      , case when (convert(varchar, d.ReminderDate, 112) = '19000101') then null else d.ReminderDate end ReminderDate  
      , case when (convert(varchar, d.BookingDate, 112) = '19000101') then null else d.BookingDate end BookingDate  
      , case when (convert(varchar, d.FollowUpDate, 112) = '19000101') then null else d.FollowUpDate end FollowUpDate  
      , convert(varchar(20), case d.IsConfirmed when null then '' when 0 then 'Tidak' when 1 then 'YA' end) IsConfirmed  
      , convert(varchar(20), case d.IsBooked when null then '' when 0 then 'Tidak' when 1 then 'YA' end) IsBooked  
      , convert(varchar(20), case d.IsVisited when null then '' when 0 then 'Tidak' when 1 then 'YA' end) IsVisited  
      , convert(varchar(20), case d.IsSatisfied when null then '' when 0 then 'Tidak' when 1 then 'YA' end) IsSatisfied  
      , d.Reason
      , case isnull(d.IsVisited, 0) when 0 then '' else d.VisitInitial end VisitInitial
      , case isnull(d.IsVisited, 0) when 0 then '' else e.LookupValueName end VisitInitialDesc
      , isnull(c.Address1, '')+isnull(c.Address2, '')+isnull(c.Address3, '')+isnull(c.Address4, '') Address  
      , case isnull(b.ContactName, '') when '' then c.CustomerName else rtrim(b.ContactName) end ContactName  
      , case isnull(b.ContactAddress, '') when '' then (isnull(c.Address1, '')+isnull(c.Address2, '')+isnull(c.Address3, '')+isnull(c.Address4, '')) else b.ContactAddress end ContactAddress  
      , case isnull(b.ContactPhone, '') when '' then c.PhoneNo else b.ContactPhone end ContactPhone  
      , c.PhoneNo, d.StatisfyReasonGroup, d.StatisfyReasonCode
      , convert(varchar(20), case d.IsReminder when null then '' when 0 then 'Tidak' when 1 then 'YA' end) IsReminder
      , convert(varchar(20), case d.IsFollowUp when null then '' when 0 then 'Tidak' when 1 then 'YA' end) IsFollowUp
      , b.TransmissionType  
      , a.Odometer, c.HPNo as MobilePhone  
      from svTrnInvoice a  
      left join SvMstCustomerVehicle b  
        on b.Companycode = a.CompanyCode   
       and b.ChassisCode = a.ChassisCode   
       and b.ChassisNo = a.ChassisNo  
      left join GnMstCustomer c  
        on c.CompanyCode = a.CompanyCode  
       and c.CustomerCode = a.CustomerCode  
        inner join svTrnInvoice f
		on f.CompanyCode = a.CompanyCode
		and f.BranchCode = a.BranchCode
		and f.InvoiceNo = a.InvoiceNo
      left join svTrnDailyRetention d  
        on d.CompanyCode = a.CompanyCode  
		and d.BranchCode = a.BranchCode 
		and d.PeriodYear = year(f.InvoiceDate)  
		and d.PeriodMonth = month(f.InvoiceDate) 
		and d.CustomerCode = a.CustomerCode  
		and d.ChassisCode = a.ChassisCode 
		and d.ChassisNo = a.ChassisNo  
      left join gnMstLookupDtl e  
        on e.CompanyCode = a.CompanyCode  
       and e.CodeID = 'CIRS'  
       and e.LookupValue = d.VisitInitial  
 where a.CompanyCode = @CompanyCode
   and a.BranchCode = @BranchCode
		and convert(varchar, @DateParam, 112) = convert(varchar, dateadd(day, @Interval, a.JobOrderDate), 112) 
		and convert(varchar, a.InvoiceDate, 112) < convert(varchar, @DateParam, 112)
	   and a.JobType <> (case @InclPDI when 1 then '' else 'PDI' end)
	   and (b.LastServiceDate is null or convert(varchar, a.JobOrderDate, 112) >= convert(varchar, b.LastServiceDate, 112))
end
