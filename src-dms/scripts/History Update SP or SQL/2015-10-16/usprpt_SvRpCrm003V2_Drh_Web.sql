
Alter procedure [dbo].[usprpt_SvRpCrm003V2_Drh_Web] 
	@CompanyCode	Varchar(15),
	@BranchCode		Varchar(15),
	@DateParam		Datetime,
	@OptionType		Varchar(100), 
	@Range			int, 
	@Interval		int,
	@IncPDI			bit
as

-- select @CompanyCode='6006406',@BranchCode='6006406',@DateParam='20120129',@OptionType='4FOLLOWUP',@Range=-1,@Interval=0,@IncPDI=0

if @OptionType = 'REMINDER'
begin
	select distinct 
		 e.LookUpValueName as Inisial
		 , b.BasicModel as [Type]
		 , b.PoliceRegNo as [No Polisi]
		 , case when b.TransmissionType is null then 'MT' else case when b.TransmissionType = ' ' then 'MT' else b.TransmissionType end end TM 
		 , year(getdate()) as [Tahun]
		 , b.EngineCode as [Kode Mesin]
		 , b.EngineNo as [No Mesin]
		 , b.ChassisCode as [Kode Rangka]
		 , b.ChassisNo as [No Rangka]
		 , c.CustomerName as [Nama Pelanggan]
		 , isnull(c.Address1, '')+' '+isnull(c.Address2, '')+' '+isnull(c.Address3, '')+' '+isnull(c.Address4, '') as [Alamat Pelanggan]
		 , c.PhoneNo as [No.Telpon Rumah]
		 , c.Spare03 as [Addtional Phone 1] 
		 , c.Spare04 as [Addtional Phone 2]
		 , c.OfficePhoneNo as [No.Telpon Kantor]
		 , c.HPNo as [No. HP]
		 , case when (convert(varchar, b.LastServiceDate, 112) = '19000101') then null else b.LastServiceDate end as [Tanggal Kunjungan]
		 , a.Odometer as [RM]
		 , d.PMNow as [P/M Saat Ini]
		 , d.PMNext as [P/M Berikut]		 
		 , isnull(case when a.JobType like 'PB%' then 
 			case when REPLACE(a.JobType,'KM','') = 'PB100000' 
			then
				case when b.LastServiceDate is not null then DateAdd(MONTH,3, b.LastServiceDate) else b.LastServiceDate end
			else
				DateAdd(month
				,(select convert(int,(select b.TimePeriod from svMstRoutineService b
				where JobType = 'PB' + convert(varchar,convert(int,Substring(REPLACE(a.JobType,'KM',''),3,LEN(a.JobType)-2))+5000)
				)) - convert(int,(select TimePeriod from svMstRoutineService 
				where JobType = (select JobType from svTrnService where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and ServiceNo = a.ServiceNo))) TimePeriod
				)
				,b.LastServiceDate)
			end
 		   else DateAdd(MONTH,3, b.LastServiceDate)
		   end,'') as [Estimasi Berikut]  
		 , case(d.ReminderDate) WHEN '19000101' THEN NULL ELSE d.ReminderDate END as [Tgl.Reminder]
		 , case(d.IsConfirmed)when '1' then 'Ya' else 'Tidak' END as [Berhasil DiHubungi]
		 , case(d.IsBooked) when '1' then 'Ya' else 'Tidak' END as [Booking]
		 , case(d.BookingDate) WHEN '19000101' THEN NULL ELSE d.BookingDate END as [Tgl.Booking]
		 , case(d.IsVisited) when '1' then 'Ya' else 'Tidak' END as [Konsumen Datang]
		 , case(d.FollowUpDate) WHEN '19000101' THEN NULL ELSE d.FollowUpDate END as [Tgl.FollowUp]
		 , case(d.IsSatisfied) when '1' then 'Ya' else 'Tidak' END as [Puas / Tidak]
		 , d.Reason as Alasan
		 , case when b.ContactName is null then convert(varchar,c.CustomerName) else b.ContactName end as [Nama Kontak]
		 , case when b.ContactAddress is null then isnull(c.Address1, '')+' '+isnull(c.Address2, '')+' '+isnull(c.Address3, '')+' '+isnull(c.Address4, '') else b.ContactAddress end as [Alamat Kontak]
		 , case when b.ContactPhone is null then c.PhoneNo else b.ContactPhone end as [No.Telepon Kontak]
		 , h.EmployeeName as [Nama Service Advisor]
		 , i.EmployeeName as [Nama Mekanik] 
		 , a.ServiceRequestDesc as [Permintaan Perawatan]		 
		 , convert(varchar,g.Remarks) as [Rekomendasi]
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
	  left join gnMstLookUpDtl f on b.CompanyCode = a.CompanyCode
	   and f.CodeId = 'CCRS'
	   and f.LookUpValue = d.CustomerCategory
	  left join svTrnInvoice g on g.CompanyCode = a.CompanyCode
	   and g.BranchCode = a.BranchCode
	   and g.ProductType = a.ProductType
	   and g.InvoiceNo = a.InvoiceNo
	  left join gnMstEmployee h on h.CompanyCode = a.CompanyCode
	   and h.BranchCode = a.BranchCode
	   and h.EmployeeID = a.ForemanID
	  left join gnMstEmployee i on i.CompanyCode = a.CompanyCode
	   and i.BranchCode = a.BranchCode
	   and i.EmployeeID = a.MechanicID
	 where 1 = 1  
	   and a.CompanyCode = @CompanyCode  
	   and a.BranchCode  = @BranchCode  
	   and convert(varchar, @DateParam, 112)  
		 between convert(varchar, dateadd(day, -@Interval, dateadd(month, @Range, a.JobOrderDate)), 112)  
		     and convert(varchar, dateadd(day, @Interval, dateadd(month, @Range, a.JobOrderDate)), 112)  
	   and a.JobType <> (case @IncPDI when 1 then '' else 'PDI' end)
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
	select distinct 
		 e.LookUpValueName as Inisial
		 , b.BasicModel as [Type]
		 , b.PoliceRegNo as [No Polisi]
		 , case when b.TransmissionType is null then 'MT' else case when b.TransmissionType = ' ' then 'MT' else b.TransmissionType end end TM 
		 , year(getdate()) as [Tahun]
		 , b.EngineCode as [Kode Mesin]
		 , b.EngineNo as [No Mesin]
		 , b.ChassisCode as [Kode Rangka]
		 , b.ChassisNo as [No Rangka]
		 , c.CustomerName as [Nama Pelanggan]
		 , isnull(c.Address1, '')+' '+isnull(c.Address2, '')+' '+isnull(c.Address3, '')+' '+isnull(c.Address4, '') as [Alamat Pelanggan]
		 , c.PhoneNo as [No.Telpon Rumah]
		 , c.Spare03 as [Addtional Phone 1] 
		 , c.Spare04 as [Addtional Phone 2]
		 , c.OfficePhoneNo as [No.Telpon Kantor]
		 , c.HPNo as [No. HP]
		 , case when (convert(varchar, b.LastServiceDate, 112) = '19000101') then null else b.LastServiceDate end as [Tanggal Kunjungan]
		 , a.Odometer as [RM]
		 , d.PMNow as [P/M Saat Ini]
		 , d.PMNext as [P/M Berikut]		 
		 , isnull(case when a.JobType like 'PB%' then 
 			case when REPLACE(a.JobType,'KM','') = 'PB100000' 
			then
				case when b.LastServiceDate is not null then DateAdd(MONTH,3, b.LastServiceDate) else b.LastServiceDate end
			else
				DateAdd(month
				,(select convert(int,(select b.TimePeriod from svMstRoutineService b
				where JobType = 'PB' + convert(varchar,convert(int,Substring(REPLACE(a.JobType,'KM',''),3,LEN(a.JobType)-2))+5000)
				)) - convert(int,(select TimePeriod from svMstRoutineService 
				where JobType = (select JobType from svTrnService where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and ServiceNo = a.ServiceNo))) TimePeriod
				)
				,b.LastServiceDate)
			end
 		   else DateAdd(MONTH,3, b.LastServiceDate)
		   end,'') as [Estimasi Berikut]  
		 , case(d.ReminderDate) WHEN '19000101' THEN NULL ELSE d.ReminderDate END as [Tgl.Reminder]
		 , case(d.IsConfirmed)when '1' then 'Ya' else 'Tidak' END as [Berhasil DiHubungi]
		 , case(d.IsBooked) when '1' then 'Ya' else 'Tidak' END as [Booking]
		 , case(d.BookingDate) WHEN '19000101' THEN NULL ELSE d.BookingDate END as [Tgl.Booking]
		 , case(d.IsVisited) when '1' then 'Ya' else 'Tidak' END as [Konsumen Datang]
		 , case(d.FollowUpDate) WHEN '19000101' THEN NULL ELSE d.FollowUpDate END as [Tgl.FollowUp]
		 , case(d.IsSatisfied) when '1' then 'Ya' else 'Tidak' END as [Puas / Tidak]
		 , d.Reason as Alasan
		 , case when b.ContactName is null then convert(varchar,c.CustomerName) else b.ContactName end as [Nama Kontak]
		 , case when b.ContactAddress is null then isnull(c.Address1, '')+' '+isnull(c.Address2, '')+' '+isnull(c.Address3, '')+' '+isnull(c.Address4, '') else b.ContactAddress end as [Alamat Kontak]
		 , case when b.ContactPhone is null then c.PhoneNo else b.ContactPhone end as [No.Telepon Kontak]
		 , h.EmployeeName as [Nama Service Advisor]
		 , i.EmployeeName as [Nama Mekanik] 
		 , a.ServiceRequestDesc as [Permintaan Perawatan]		 
		 , convert(varchar,g.Remarks) as [Rekomendasi]
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
	  left join gnMstLookupDtl e  
		on e.CompanyCode = a.CompanyCode  
	   and e.CodeID = 'CIRS'  
	   and e.LookupValue = d.VisitInitial  
	  left join gnMstLookUpDtl f on b.CompanyCode = a.CompanyCode
	   and f.CodeId = 'CCRS'
	   and f.LookUpValue = d.CustomerCategory
	  left join svTrnInvoice g on g.CompanyCode = a.CompanyCode
	   and g.BranchCode = a.BranchCode
	   and g.ProductType = a.ProductType
	   and g.InvoiceNo = a.InvoiceNo
	  left join gnMstEmployee h on h.CompanyCode = a.CompanyCode
	   and h.BranchCode = a.BranchCode
	   and h.EmployeeID = a.ForemanID
	  left join gnMstEmployee i on i.CompanyCode = a.CompanyCode
	   and i.BranchCode = a.BranchCode
	   and i.EmployeeID = a.MechanicID
	 where 1 = 1  
	   and a.CompanyCode = @CompanyCode  
	   and a.BranchCode = @BranchCode  
	   and a.ServiceStatus not in ('0','1','2','3','4','5','6')  
	   and convert(varchar, @DateParam, 112) = convert(varchar, dateadd(day, @Interval, a.JobOrderDate), 112)    
	   and a.JobType <> (case @IncPDI when 1 then '' else 'PDI' end)
	   and (b.LastServiceDate is null or convert(varchar, a.JobOrderDate, 112) >= convert(varchar, b.LastServiceDate, 112))  
	   and d.RetentionNo = ISNULL((
								select max(RetentionNo)
								  from svTrnDailyRetention
								 where CompanyCode = a.CompanyCode
								   and BranchCode = a.BranchCode
								   and PeriodYear = year(a.JobOrderDate)  
								   and PeriodMonth = month(a.JobOrderDate)
								   and CustomerCode = a.CustomerCode
								),0)  
end
