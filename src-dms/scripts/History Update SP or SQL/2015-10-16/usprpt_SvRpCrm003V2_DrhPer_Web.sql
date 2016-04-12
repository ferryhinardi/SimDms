

ALTER procedure [dbo].[usprpt_SvRpCrm003V2_DrhPer_Web]
	@CompanyCode	  varchar(15),
	@BranchCode		  varchar(15),
	@OptionType		  varchar(100), 
	@ServiceDateFrom  varchar(15),
	@ServiceDateTo    varchar(15),
	@ReminderDateFrom varchar(15),
	@ReminderDateTo   varchar(15),
	@FollowUpDateFrom varchar(15),
	@FollowUpDateTo   varchar(15)
as

BEGIN
    select * into #t1
      from ( select CompanyCode, BranchCode, ChassisCode, ChassisNo, VisitInitial, CustomerCategory, 
                    RetentionNo, PMNow, PMNext, ReminderDate, IsConfirmed, IsBooked, BookingDate, 
                    IsVisited, FollowUpDate, IsSatisfied, Reason, LastRemark
               from svTrnDailyRetention a
              where RetentionNo = (select top 1 RetentionNo from svTrnDailyRetention
                                    where CompanyCode=a.CompanyCode
                                      and BranchCode =a.BranchCode
                                      and ChassisCode=a.ChassisCode
                                      and ChassisNo  =a.ChassisNo
                                    order by RetentionNo desc) ) #t1
      
	select e.LookUpValueName as Inisial
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
		 , case when (convert(varchar,b.LastServiceDate,112)='19000101') then a.JobOrderDate 
		        when b.LastServiceDate < a.JobOrderDate                  then a.JobOrderDate
		        else b.LastServiceDate 
		    end as [Tanggal Kunjungan]
		 , a.Odometer as [RM]
		 , d.PMNow as [P/M Saat Ini]
		 , d.PMNext as [P/M Berikut] 
		 , DateAdd(MONTH,3, b.LastServiceDate) as [Estimasi Berikut]
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
		          on b.Companycode =a.CompanyCode   
	             and b.ChassisCode =a.ChassisCode   
	             and b.ChassisNo   =a.ChassisNo  
	       left join GnMstCustomer c  
		          on c.CompanyCode =a.CompanyCode  
	             and c.CustomerCode=a.CustomerCode  
	       left join #t1 d --svTrnDailyRetention d  
		          on d.CompanyCode =a.CompanyCode  
	             and d.BranchCode  =a.BranchCode 
	             and d.ChassisCode =a.ChassisCode 
	             and d.ChassisNo   =a.ChassisNo  
	       left join gnMstLookupDtl e  
		          on e.CompanyCode =a.CompanyCode  
	             and e.CodeID      ='CIRS'  
	             and e.LookupValue =d.VisitInitial  
	       left join gnMstLookUpDtl f 
	              on b.CompanyCode =a.CompanyCode
	             and f.CodeId      ='CCRS'
	             and f.LookUpValue =d.CustomerCategory
	       left join svTrnInvoice g 
	              on g.CompanyCode =a.CompanyCode
	             and g.BranchCode  =a.BranchCode
	             and g.ProductType =a.ProductType
	             and g.InvoiceNo   =a.InvoiceNo
	       left join gnMstEmployee h 
	              on h.CompanyCode =a.CompanyCode
	             and h.BranchCode  =a.BranchCode
	             and h.EmployeeID  =a.ForemanID
	       left join gnMstEmployee i 
	              on i.CompanyCode =a.CompanyCode
	             and i.BranchCode  =a.BranchCode
	             and i.EmployeeID  =a.MechanicID
	       left join gnMstCoProfile p
	              on p.CompanyCode =a.CompanyCode
	             and p.BranchCode  =a.BranchCode
	 where a.CompanyCode = @CompanyCode  
	   and a.BranchCode  = @BranchCode  
	   and a.ServiceStatus in ('7','9')  -- 7:Invoice, 9:Faktur Pajak 
	   and (b.LastServiceDate is null or convert(varchar,a.JobOrderDate,112) >= convert(varchar,b.LastServiceDate,112))  
	   and (case when @ServiceDateFrom != '' 
	             then convert(varchar,a.JobOrderDate,112) 
	             else @ServiceDateFrom 
	        end) between @ServiceDateFrom and @ServiceDateTo
	   and (case when @ReminderDateFrom != '' 
	             then convert(varchar,d.ReminderDate,112) 
	             else @ReminderDateFrom 
	        end) between @ReminderDateFrom and @ReminderDateTo
	   and (case when @FollowUpDateFrom != '' 
	             then convert(varchar,d.FollowUpDate,112) 
	             else @FollowUpDateFrom 
	        end) between @FollowUpDateFrom and @FollowUpDateTo
     order by a.JobOrderNo
     
	drop table #t1
END
