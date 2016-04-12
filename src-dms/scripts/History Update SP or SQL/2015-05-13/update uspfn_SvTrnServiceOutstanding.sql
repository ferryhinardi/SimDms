ALTER procedure [dbo].[uspfn_SvTrnServiceOutstanding]
	@OutType     varchar(15),
	@CompanyCode varchar(15),
	@BranchCode  varchar(15),
	@ProductType varchar(15),
	@PoliceRegNo varchar(15),
	@JobType     varchar(15),
	@ChassisCode varchar(15),
	@ChassisNo	 varchar(10)

as      

create table #t1(ServiceNo bigint, ServiceType varchar(10), JobOrderNo varchar(15), JobOrderDate datetime, JobType varchar(20), MessageInfo varchar(max))

if @OutType = 'FSC'
begin
	insert into #t1
	select top 1 a.ServiceNo
		 , a.ServiceType
		 , case a.ServiceType
			 when 0 then a.EstimationNo
			 when 1 then a.BookingNo
		     else a.JobOrderNo
		   end JobOrderNo
		 , case a.ServiceType
			 when 0 then a.EstimationDate
			 when 1 then a.BookingDate
		     else a.JobOrderDate
		   end JobOrderDate
		 , a.JobType
		 , 'Kendaraan ini sudah pernah di Free Service, transaksi tidak bisa dilanjutkan' 
	  from svTrnService a, svTrnSrvTask b
	 where a.JobType like 'FSC%'
	   and b.CompanyCode = a.CompanyCode
	   and b.BranchCode  = a.BranchCode
	   and b.ProductType = a.ProductType
	   and b.ServiceNo   = a.ServiceNo
	   and b.BillType    = 'F'
	   and a.CompanyCode = @CompanyCode
	   and a.BranchCode  = @BranchCode
	   and a.ProductType = @ProductType
	   and a.PoliceRegNo = @PoliceRegNo
	   and a.JobType     = @JobType
	   and a.ChassisCode = @ChassisCode
	   and a.ChassisNo	 = CONVERT(varchar, @ChassisNo, 10)
	   and a.ServiceType = 2
	   and a.ServiceStatus <> '6'
end

if @OutType = 'OUT'
begin
	insert into #t1
	select top 1 ServiceNo
		 , ServiceType
		 , case ServiceType
			 when 0 then EstimationNo
			 when 1 then BookingNo
		   else JobOrderNo end JobOrderNo
		 , case ServiceType
			 when 0 then EstimationDate
			 when 1 then BookingDate
		   else JobOrderDate end JobOrderDate
		 , JobType
		 , 'Kendaraan ini masih ada outstanding, masih akan dilanjutkan?' 
	  from svTrnService
	 where ServiceStatus in ('0','1','2','3','4','5')
	   and CompanyCode = @CompanyCode
	   and BranchCode  = @BranchCode
	   and ProductType = @ProductType
	   and PoliceRegNo = @PoliceRegNo
	   and ChassisCode = @ChassisCode
	   and ChassisNo   = CONVERT(varchar, @ChassisNo, 10)
	   and ServiceType = '2'
end

if @OutType = 'BOK'
begin
	insert into #t1
	select top 1 ServiceNo
		 , ServiceType
		 , BookingNo as JobOrderNo
		 , BookingDate as JobOrderDate
		 , JobType
		 , 'Kendaraan ini masih ada outstanding booking, masih akan dilanjutkan?' 
	  from svTrnService
	 where ServiceStatus in ('0','1','2','3','4','5')
	   and CompanyCode = @CompanyCode
	   and BranchCode  = @BranchCode
	   and ProductType = @ProductType
	   and PoliceRegNo = @PoliceRegNo
	   and ChassisCode = @ChassisCode
	   and ChassisNo   = CONVERT(varchar, @ChassisNo, 10)
	   and ServiceType = '1'
	   and datediff(month, BookingDate, getdate()) <= 1
end

if @OutType = 'EST'
begin
	insert into #t1
	select top 1 ServiceNo
		 , ServiceType
		 , EstimationNo as JobOrderNo
		 , EstimationDate as JobOrderDate
		 , JobType
		 , 'Kendaraan ini masih ada outstanding estimasi, masih akan dilanjutkan?' 
	  from svTrnService
	 where ServiceStatus in ('0','1','2','3','4','5')
	   and CompanyCode = @CompanyCode
	   and BranchCode  = @BranchCode
	   and ProductType = @ProductType
	   and PoliceRegNo = @PoliceRegNo
	   and ChassisCode = @ChassisCode
	   and ChassisNo   = CONVERT(varchar, @ChassisNo, 10)
	   and ServiceType = '0'
	   and datediff(month, EstimationDate, getdate()) <= 1
end

select * from #t1
drop table #t1
