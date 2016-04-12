var moment = require('moment');
var async = require('async');
var request = require('request');
var path = require('path');
var fs = require('fs');
var util = require('util');
var archiver = require('archiver');
var sqlODBC = require('node-sqlserver');
var Client = require('q-svn-spawn');
var config = require('../config');
var http = require('http');
var argv = require('yargs')
    .string('d')
    .argv;
	
var TaskName = "UPDATE SP usprpt_SvRpReport021V3";
var TaskNo = "20141209001";
var CurrentDealerCode = argv.d;

function hereDoc(f) {
  return f.toString().
	  replace(/^[^\/]+\/\*!?/, '').
	  replace(/\*\/[^\/]+$/, '');
}

var SQLCheck = hereDoc(function(){/*!
declare @column_list varchar(MAX)
SELECT @column_list = COALESCE(@column_list + ', ', '') + COLUMN_NAME 
FROM INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='#TABLENAME#'
SELECT @column_list list, COUNT(*) total
FROM INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='#TABLENAME#'
*/});

var SQL = hereDoc(function(){/*!
alter procedure usprpt_SvRpReport021V3
--declare
	@CompanyCode varchar(15),
	@BranchCode  varchar(15),
	@ProductType varchar(15), 
	@PeriodYear  int,
	@Month1      int = 1,
	@Month2      int,
	@UserID      varchar(20)
as

--select @CompanyCode=N'6026401',@BranchCode=N'602640100',@ProductType=N'4W',@PeriodYear=2012,@Month2=1,@UserID=N'ga'

set nocount on

if @Month1 > @Month2 set @Month1 = @Month2

-- A. Sales Revenue (SR)
declare @A_01 numeric(18, 4) -- Total Labor Sales Revenue ( 2 + 3 + 4 )
declare @A_02 numeric(18, 4) --   Labor Sales - Chargeable to Customer CPUS (External)
declare @A_03 numeric(18, 4) --   Labor Sales - Non Chargeable (Warranty, FS1, FS2, FS3, FS4, FS5, FS6, FS7)
declare @A_04 numeric(18, 4) --   Labor Sales - Non Chargeable (Internal, PDI)
declare @A_05 numeric(18, 4) -- Total Parts Sales Revenue ( 6 + 7 + 8 )
declare @A_06 numeric(18, 4) --   Parts Sales - Chargeable to Customer CPUS (External)
declare @A_07 numeric(18, 4) --   Parts Sales - Non Chargeable (Warranty, FS1)
declare @A_08 numeric(18, 4) --   Parts Sales - Non Chargeable (Internal, PDI)
declare @A_09 numeric(18, 4) -- Total Counter Parts Sales Revenue
declare @A_10 numeric(18, 4) -- Total Lubricant Sales Revenue ( 11 + 12 + 13 )  
declare @A_11 numeric(18, 4) --   Lubricant Sales - Chargeable to Customer CPUS (External)
declare @A_12 numeric(18, 4) --   Lubricant Sales - Non Chargeable (Warranty, FS1)
declare @A_13 numeric(18, 4) --   Lubricant Sales - Non Chargeable (Internal, PDI)
declare @A_14 numeric(18, 4) -- Total Sublet Sales Revenue
declare @A_15 numeric(18, 4) -- Total Service Sales Revenue ( 16 + 17 + 18 )    
declare @A_16 numeric(18, 4) --   Service Sales - Chargeable to Customer CPUS (External)
declare @A_17 numeric(18, 4) --   Service Sales - Non Chargeable (Warranty, FS1)
declare @A_18 numeric(18, 4) --   Service Sales - Non Chargeable (Internal, PDI)
declare @A_19 numeric(18, 4) -- Hours Sold  
declare @A_20 numeric(18, 4) -- Total Available Hours  
declare @A_21 numeric(18, 4) -- Hours Sold / Available Hours  
declare @A_22 numeric(18, 4) -- Service Revenue Targer Per Unit (Based on RKA) 
declare @A_23 numeric(18, 4) -- Service Revenue per Unit exclude PDI (CPUS, Warranty, FS1) ((16 + 17) / 29)
declare @A_24 numeric(18, 4) -- Labour  Sales Turnover / Productive staff (Technician & Foreman)  (1 / 61)
declare @A_25 numeric(18, 4) -- Labour Sales Turnover / Service Advisors (1 / 60)
declare @A_26 numeric(18, 4) -- Labour Sales Turnover / Stall (1 / 56)
declare @A_27 numeric(18, 4) -- Absorption Rate
-- B. No of Unit Intake
declare @B_28 numeric(18, 4) -- Unit Entry Target
declare @B_29 numeric(18, 4) -- No of Work Order ( 30 + 31 )
declare @B_30 numeric(18, 4) -- Passenger Car
declare @B_31 numeric(18, 4) -- Commercial Vehicle 
declare @B_32 numeric(18, 4) -- PDI
-- C. No of JobType
declare @C_33 numeric(18, 4) -- Chargeable /  Customer Paid Unit Service (CPUS) (34 + 38 + 42 + 43)
declare @C_34 numeric(18, 4) --   Periodical Maintenance under Warranty Period (= 100,000 km / = 3 Years) (sum of 35 to 37)
declare @C_35 numeric(18, 4) --     10,000*; 30,000; 50,000; 70,000; & 90,000 km
declare @C_36 numeric(18, 4) --     20,000*; 60,000 & 100,000 km
declare @C_37 numeric(18, 4) --     40,000 & 80,000 km 
declare @C_38 numeric(18, 4) --   Periodical Maintenance out of Warranty Period (> 100,000 km / >3 years) (sum of 39 to 41)
declare @C_39 numeric(18, 4) --     +10,000; +30,000; +50,000; +70,000; & +90,000 km
declare @C_40 numeric(18, 4) --     +20,000; +60,000 & +100,000 km
declare @C_41 numeric(18, 4) --     +40,000 & +80,000 km
declare @C_42 numeric(18, 4) --   5,000 km multiplier & above
declare @C_43 numeric(18, 4) --   G/R Non Periodical Maintenance and Others
declare @C_44 numeric(18, 4) -- No. of Free Service (FS 1, FS 2, and FS 3) (sum of 45 to 47)
declare @C_45 numeric(18, 4) --   KSG  1,000 km
declare @C_46 numeric(18, 4) --   KSG  5,000 km
declare @C_47 numeric(18, 4) --   KSG 10,000 km
declare @C_48 numeric(18, 4) --   KSG 20,000 km
declare @C_53 numeric(18, 4) --   KSG 30,000 km
declare @C_54 numeric(18, 4) --   KSG 40,000 km
declare @C_55 numeric(18, 4) --   KSG 50,000 km
declare @C_49 numeric(18, 4) -- Non-chargeable Repair (Warranty, Repeated Job, PDI) (Sum of 49 to 51)
declare @C_50 numeric(18, 4) --   No. of Warranty Repair
declare @C_51 numeric(18, 4) --   Repeat Job (Rework)
declare @C_52 numeric(18, 4) --   No. of PDI intake
-- D. Workshop Service Strength
declare @D_53 numeric(18, 4) -- No. of Working Stalls ( Available stalls Exclude Inspection Stall and Washing Stall)
declare @D_54 numeric(18, 4) -- Total No. of Staff (55 + 56 + 57 + 58 + 61 + 62)
declare @D_55 numeric(18, 4) --   No. of Admin & Support Staff
declare @D_56 numeric(18, 4) --   No. of Service Relation Officer (SRO)
declare @D_57 numeric(18, 4) --   No. of Service Advisors
declare @D_58 numeric(18, 4) --   No. of Productive Staff (59 + 60)
declare @D_59 numeric(18, 4) --     No. of Foreman
declare @D_60 numeric(18, 4) --     No. of Technician
declare @D_61 numeric(18, 4) --   No. of PDI Staff
declare @D_62 numeric(18, 4) --   No. of Part Staff
-- E. Productivity Indicator
declare @E_01 numeric(18, 4)
declare @E_02 numeric(18, 4)
declare @E_03 numeric(18, 4)
declare @E_04 numeric(18, 4)
declare @E_05 numeric(18, 4)
declare @E_06 numeric(18, 4)
declare @E_07 numeric(18, 4)
declare @E_08 numeric(18, 4)
declare @E_09 numeric(18, 4)
-- F. Activity
declare @F_72 numeric(18, 4)
declare @F_73 numeric(18, 4)
declare @F_74 numeric(18, 4)
declare @F_75 numeric(18, 4)
declare @F_76 numeric(18, 4)
declare @F_77 numeric(18, 4)
declare @F_78 numeric(18, 4)
declare @F_79 numeric(18, 4)
-- G. 
declare @G_01 numeric(18, 4)
declare @G_02 numeric(18, 4)
declare @G_03 numeric(18, 4)
declare @G_04 numeric(18, 4)
declare @G_05 numeric(18, 4)
declare @G_06 numeric(18, 4)
declare @G_07 numeric(18, 4)
declare @G_08 numeric(18, 4)
declare @G_09 numeric(18, 4)
declare @G_10 numeric(18, 4)
declare @G_11 numeric(18, 4)
declare @G_12 numeric(18, 4)

-- declare table msi data
declare @t_msidat as table
(
	MsiGrCode varchar(10),
	MsiCode   varchar(10),
	MsiMonth  int,
	MsiData   numeric(18, 4) 
)

-- declare table msi group
declare @t_msigrp as table
(
	MsiGroup  varchar(max),
	MsiGrDesc varchar(max)
)

declare @Month0 int set @Month0 = @Month1

while @Month0 <= @Month2
begin
	if exists (
		select * from SvHstSzkMSI
		 where CompanyCode = @CompanyCode
		   and BranchCode = @BranchCode
		   and PeriodYear = @PeriodYear
		   and PeriodMonth = @Month0
		   and PeriodMonth < @Month2) 
	begin
		set @Month0 = @Month0 + 1
		continue
	end			   

	-- A. Calculate Sales Revenue
	-- Labor Sales Revenue @A_01 -> @P_04
	declare @t_inv_tsk as table
	(
		InvoiceNo varchar(20),
		HourSold  numeric(18,2),  
		LbrDppAmt numeric(18,0),
		IsSubCon  bit,
		JobType   varchar(20),
		KsgType   varchar(20)  
	)
   	
	insert into @t_inv_tsk
	select a.InvoiceNo, b.OperationHour
		 , ceiling(b.OperationHour * b.OperationCost * (100.0 - b.DiscPct) * 0.01) LbrDppAmt
		 , IsSubCon = isnull((
						  select top 1 IsSubCon from svMstTask
						   where CompanyCode = a.CompanyCode
							 and BasicModel = a.BasicModel
							 and OperationNo = b.OperationNo
						  ), 0)
		 , a.JobType		
		 , case 
			  when (c.JobType like 'FS%' and c.GroupJobType = 'FSC' and c.WarrantyOdometer = 1000)  then 'FSC01'
			  when (c.JobType like 'FS%' and c.GroupJobType = 'FSC' and c.WarrantyOdometer = 5000)  then 'FSC02'
			  when (c.JobType like 'FS%' and c.GroupJobType = 'FSC' and c.WarrantyOdometer = 10000) then 'FSC03'
			  when (c.JobType like 'FS%' and c.GroupJobType = 'FSC' and c.WarrantyOdometer = 20000) then 'FSC04'
			  when (c.JobType like 'FS%' and c.GroupJobType = 'FSC' and c.WarrantyOdometer = 30000) then 'FSC05'
			  when (c.JobType like 'FS%' and c.GroupJobType = 'FSC' and c.WarrantyOdometer = 40000) then 'FSC06'
			  when (c.JobType like 'FS%' and c.GroupJobType = 'FSC' and c.WarrantyOdometer = 50000) then 'FSC07'
			  else ''
		   end as KsgType
	  from svTrnInvoice a
	 inner join svTrnInvTask b
		on b.CompanyCode = a.CompanyCode
	   and b.BranchCode = a.BranchCode
	   and b.InvoiceNo = a.InvoiceNo
	  left join svMstJob c
		on c.CompanyCode = a.CompanyCode
	   and c.BasicModel = a.BasicModel
	   and c.JobType = a.JobType
	 where a.CompanyCode = @CompanyCode
	   and a.BranchCode = @BranchCode
	   and Year(a.Invoicedate) = @PeriodYear
	   and Month(a.Invoicedate) = @Month0
	   and a.JobType != 'REWORK'

	set @A_01 = ISNULL((select sum(LbrDppAmt) from @t_inv_tsk where IsSubCon = 0),0)
	-- CLAIM & FSC
	set @A_03 = ISNULL((select sum(LbrDppAmt) from @t_inv_tsk where IsSubCon = 0 and (substring(InvoiceNo, 1 ,3) = 'INW' or substring(InvoiceNo, 1 ,3) = 'INF' and JobType != 'PDI')),0)
	-- INTERNAL & PDI
	set @A_04 = ISNULL((select sum(LbrDppAmt) from @t_inv_tsk where IsSubCon = 0 and (substring(InvoiceNo, 1 ,3) = 'INI' or substring(InvoiceNo, 1 ,3) = 'INF' and JobType = 'PDI')), 0)
	-- CPUS
	set @A_02 = @A_01 - (@A_03 + @A_04)
	
	-- Part Sales Revenue @P_06 -> @P_13 ex @P_09
	declare @t_inv_itm as table
	(
		InvoiceNo  varchar(20),
		GroupTpgo  varchar(50), 
		IsSublet   varchar(10),
		GroupJob   varchar(50),
		SprAmt     numeric(18,0)
	)	
	insert into @t_inv_itm
	select a.InvoiceNo
		 , c.ParaValue as GroupTpgo
		 , case when TypeofGoods in ('0', '1') then '0' else '1' end IsSublet
		 , case 
			 when left(a.InvoiceNo, 3) = 'INI' or a.JobType = 'PDI' then 'INT,PDI'
			 when left(a.InvoiceNo, 3) = 'INW' or left(a.InvoiceNo, 3) = 'INF' then 'CLM,FSC'
			 else 'CPUS'
		   end GroupJob
		 , (ceiling((b.SupplyQty - b.ReturnQty) * b.RetailPrice * (100.0 - b.DiscPct) * 0.01)) SprAmt
	  from svTrnInvoice a
	 inner join svTrnInvItem b
		on b.CompanyCode = a.CompanyCode
	   and b.BranchCode = a.BranchCode
	   and b.InvoiceNo = a.InvoiceNo
	  left join gnMstLookupDtl c
		on c.CompanyCode = a.CompanyCode
	   and c.CodeID = 'GTGO'
	   and c.LookupValue = b.TypeOfGoods
	 where a.CompanyCode = @CompanyCode
	   and a.BranchCode = @BranchCode
	   and Year(a.Invoicedate) = @PeriodYear
	   and Month(a.Invoicedate) = @Month0
	   and a.JobType != 'REWORK'

	set @A_05 = isnull((select sum(SprAmt) from @t_inv_itm where IsSublet = 0 and GroupTpgo = 'SPAREPART'), 0)
	set @A_06 = isnull((select sum(SprAmt) from @t_inv_itm where IsSublet = 0 and GroupTpgo = 'SPAREPART' and GroupJob = 'CPUS'), 0)
	set @A_07 = isnull((select sum(SprAmt) from @t_inv_itm where IsSublet = 0 and GroupTpgo = 'SPAREPART' and GroupJob = 'CLM,FSC'), 0)
	set @A_08 = isnull((select sum(SprAmt) from @t_inv_itm where IsSublet = 0 and GroupTpgo = 'SPAREPART' and GroupJob = 'INT,PDI'), 0)
	set @A_09 = isnull((
	            select sum(TotDppAmt)
	              from spTrnSFPJHdr
	             where 1 = 1
	               and CompanyCode = @CompanyCode
	               and BranchCode = @BranchCode
	               and year(FPJDate) = @PeriodYear
	               and month(FPJDate) = @Month0
	               and TypeOfGoods = 0
	             ), 0)
	set @A_10 = isnull((select sum(SprAmt) from @t_inv_itm where IsSublet = 0 and GroupTpgo != 'SPAREPART'), 0)
	set @A_11 = isnull((select sum(SprAmt) from @t_inv_itm where IsSublet = 0 and GroupTpgo != 'SPAREPART' and GroupJob = 'CPUS'), 0)
	set @A_12 = isnull((select sum(SprAmt) from @t_inv_itm where IsSublet = 0 and GroupTpgo != 'SPAREPART' and GroupJob = 'CLM,FSC'), 0)
	set @A_13 = isnull((select sum(SprAmt) from @t_inv_itm where IsSublet = 0 and GroupTpgo != 'SPAREPART' and GroupJob = 'INT,PDI'), 0)

	-- Sublet --> Subcon Task + Part Non SGP & Part Non SGO
	set @A_14 = isnull((select sum(isnull(LbrDppAmt, 0)) from @t_inv_tsk where IsSubCon = 1), 0) + isnull((select sum(isnull(SprAmt, 0)) from @t_inv_itm where IsSublet = 1), 0)
	
	set @A_16 = @A_02 + @A_06 + @A_11 + @A_14
	set @A_17 = @A_03 + @A_07 + @A_12
	set @A_18 = @A_04 + @A_08 + @A_13
	set @A_15 = @A_16 + @A_17 + @A_18
	
	set @A_19 = (select sum(HourSold) from @t_inv_tsk where IsSubCon = 0)
	set @A_20 = isnull((select sum(a.WorkingHours)
						  from gnMstAbsence a
						 where 1 = 1
						   and a.CompanyCode=@CompanyCode
						   and a.BranchCode=@BranchCode
						   and Year(a.WorkingDate)=@PeriodYear
						   and Month(a.WorkingDate)=@Month0  
						   and exists (
						      select * from gnMstEmployee 
						       where CompanyCode = a.CompanyCode
						         and BranchCode = a.BranchCode
						         and EmployeeID = a.EmployeeID
						         and TitleCode = '9'
						   ) 
						 ),0)
	set @A_21 = (case isnull(@A_20, 0) when 0 then 0 else 1.0 * isnull(@A_19, 0)/ @A_20 end) * 100.0
	set @A_22 = isnull((
						select ServiceAmount
						  from svMstTarget
						 where CompanyCode=@CompanyCode
						   and BranchCode=@BranchCode
						   and ProductType=@ProductType
						   and PeriodYear=@PeriodYear
						   and PeriodMonth=@Month0
						 ),0)
						 
	-- B. No of Unit Intake					 
	set @B_28 = isnull((
						select TotalUnitService
						  from svMstTarget
						 where CompanyCode=@CompanyCode
						   and BranchCode=@BranchCode
						   and ProductType=@ProductType
						   and PeriodYear=@PeriodYear
						   and PeriodMonth=@Month0
						 ),0)
						 
	declare @t_inv_unit as table
	(
		CompanyCode  varchar(20),
		BasicModel   varchar(50), 
		Unit         varchar(50),
		InvoiceDate  varchar(50),
		JmlInvoice   int,
		GroupType    varchar(20)
	)	
	insert into @t_inv_unit
	select x.*, case y.IsLocked when 0 then 'P' else 'C' end as GroupType 
	  from (
			select a.CompanyCode, a.BasicModel
				 , a.ChassisCode + convert(varchar, a.ChassisNo) Unit
				 , convert(varchar, a.JobOrderDate, 112) JobOrderDate
				 , count(a.InvoiceNo) JmlInvoice
			  from svTrnInvoice a
			 where a.CompanyCode = @CompanyCode
			   and a.BranchCode = @BranchCode
			   and year(a.InvoiceDate) = @PeriodYear
			   and month(a.InvoiceDate) = @Month0
			   and a.JobType != 'REWORK'
			 group by a.CompanyCode, a.BasicModel, a.ChassisCode, a.ChassisNo, convert(varchar, a.JobOrderDate, 112)
			)x
	  left join svMstRefferenceService y
		on y.CompanyCode = x.CompanyCode
	   and y.RefferenceType = 'BASMODEL'
	   and y.RefferenceCode = x.BasicModel

	declare @t_inv_unit_pdi as table
	(
		CompanyCode  varchar(20),
		BasicModel   varchar(50), 
		Unit         varchar(50),
		InvoiceDate  varchar(50),
		JmlInvoice   int,
		GroupType    varchar(20)
	)	
	insert into @t_inv_unit_pdi
	select x.*, case y.IsLocked when 0 then 'P' else 'C' end as GroupType 
	  from (
			select a.CompanyCode, a.BasicModel
				 , a.ChassisCode + convert(varchar, a.ChassisNo) Unit
				 , convert(varchar, a.JobOrderDate, 112) JobOrderDate
				 , count(a.InvoiceNo) JmlInvoice
			  from svTrnInvoice a
			 where a.CompanyCode = @CompanyCode
			   and a.BranchCode = @BranchCode
			   and a.JobType = 'PDI'
			   and year(a.InvoiceDate) = @PeriodYear
			   and month(a.InvoiceDate) = @Month0
			 group by a.CompanyCode, a.BasicModel, a.ChassisCode, a.ChassisNo, convert(varchar, a.JobOrderDate, 112)
			)x
	  left join svMstRefferenceService y
		on y.CompanyCode = x.CompanyCode
	   and y.RefferenceType = 'BASMODEL'
	   and y.RefferenceCode = x.BasicModel

	declare @t_inv_unit_exc as table
	(
		CompanyCode  varchar(20),
		BasicModel   varchar(50), 
		Unit         varchar(50),
		InvoiceDate  varchar(50),
		JmlInvoice   int,
		GroupType    varchar(20)
	)	

	insert into @t_inv_unit_exc
	select a.* from @t_inv_unit a
	 where not exists (
		select * from @t_inv_unit_pdi
		 where CompanyCode = a.CompanyCode
		   and Unit = a.Unit
		   and InvoiceDate = a.InvoiceDate
		   
	 )


	set @B_29 = (select count(*) from @t_inv_unit_exc)
	set @B_30 = (select count(*) from @t_inv_unit_exc where GroupType = 'P')
	set @B_31 = (select count(*) from @t_inv_unit_exc where GroupType = 'C')
	set @B_32 = (select count(*) from @t_inv_unit_pdi)
					
	-- C. No of JobType
	declare @t_inv_job as table
	(
		JobType      varchar(20),
		ChassisCode  varchar(20),
		ChassisNo    varchar(20), 
		JobOrderDate varchar(20)
	)
	
	insert into @t_inv_job
	select JobType, ChassisCode, ChassisNo, convert(varchar, JobOrderDate, 112) JobOrderDate
	  from svTrnInvoice
	 where 1 = 1
	   and BranchCode = @BranchCode
	   and year(InvoiceDate) = @PeriodYear
	   and month(InvoiceDate) = @Month0
	   and JobType != 'REWORK'
	 group by ChassisCode, ChassisNo, JobType, convert(varchar, JobOrderDate, 112)
	 
	-- RTN Unit
	declare @t_unit_rtn as table
	(
		InvoiceDate  varchar(20),
		ChassisCode  varchar(50), 
		ChassisNo    varchar(50),
		GroupJobType varchar(50),
		JobType      varchar(50),
		Odometer     numeric(10)
	)	
	insert into @t_unit_rtn
	select distinct convert(varchar, a.InvoiceDate, 112) InvoiceDate
		 , a.ChassisCode, convert(varchar, a.ChassisNo) ChassisNo
		 , b.GroupJobType, a.JobType
		 , b.WarrantyOdometer as Odometer
	  from svTrnInvoice a
	 inner join svMstJob b
		on b.CompanyCode = a.CompanyCode
	   and b.ProductType = a.ProductType
	   and b.BasicModel = a.BasicModel
	   and b.JobType = a.JobType
	 where a.CompanyCode = @CompanyCode
	   and a.BranchCode = @BranchCode
	   and year(a.InvoiceDate) = @PeriodYear
	   and month(a.InvoiceDate) = @Month0
	   and b.GroupJobType = 'RTN'
	   and not exists (
		select 1 from svTrnInvoice
		 where 1 = 1
		   and CompanyCode = a.CompanyCode
		   and BranchCode = a.BranchCode
		   and ChassisCode = a.ChassisCode
		   and ChassisNo = a.ChassisNo
		   and convert(varchar, InvoiceDate, 112) = convert(varchar, a.InvoiceDate, 112)
		   and (JobType = 'PDI' or JobType like 'FS%')
	   )
		 
   
	set @C_35 = (select count(*) from @t_unit_rtn where Odometer in (10000,30000,50000,70000,90000))
	set @C_36 = (select count(*) from @t_unit_rtn where Odometer in (20000,60000,100000))
	set @C_37 = (select count(*) from @t_unit_rtn where Odometer in (40000,80000))
	set @C_34 = @C_35 + @C_36 + @C_37

	set @C_39 = (select count(*) from @t_unit_rtn where Odometer in (110000,130000,150000,170000,190000,210000,230000,250000,270000,290000))
	set @C_40 = (select count(*) from @t_unit_rtn where Odometer in (120000,160000,200000,220000,260000,300000))
	set @C_41 = (select count(*) from @t_unit_rtn where Odometer in (140000,180000,240000,280000))
	set @C_38 = @C_39 + @C_40 + @C_41
	
	set @C_42 = (select count(*) from @t_unit_rtn where (Odometer % 10000) = 5000)
	
	-- FSC Job
	declare @t_unit_fsc_clm as table
	(
		InvoiceDate  varchar(20),
		ChassisCode  varchar(50), 
		ChassisNo    varchar(50),
		GroupJobType varchar(50),
		JobType      varchar(50),
		Odometer     numeric(10)
	)	
	insert into @t_unit_fsc_clm
	select distinct convert(varchar, a.InvoiceDate, 112) InvoiceDate
		 , a.ChassisCode, convert(varchar, a.ChassisNo) ChassisNo
		 , b.GroupJobType, a.JobType
		 , b.WarrantyOdometer as Odometer
	  from svTrnInvoice a
	 inner join svMstJob b
		on b.CompanyCode = a.CompanyCode
	   and b.ProductType = a.ProductType
	   and b.BasicModel = a.BasicModel
	   and b.JobType = a.JobType
	 where a.CompanyCode = @CompanyCode
	   and a.BranchCode = @BranchCode
	   and year(a.InvoiceDate) = @PeriodYear
	   and month(a.InvoiceDate) = @Month0
	   and b.GroupJobType in ('FSC','CLM')
	   and a.InvoiceNo like 'INF%'				-- Penambahan
	   and not exists (
		select 1 from svTrnInvoice
		 where 1 = 1
		   and CompanyCode = a.CompanyCode
		   and BranchCode = a.BranchCode
		   and ChassisCode = a.ChassisCode
		   and ChassisNo = a.ChassisNo
		   and convert(varchar, InvoiceDate, 112) = convert(varchar, a.InvoiceDate, 112)
		   and a.JobType = 'PDI'
	   )
	
	set @C_45 = (select count(*) from @t_unit_fsc_clm where GroupJobType = 'FSC' and Odometer = 1000)
	set @C_46 = (select count(*) from @t_unit_fsc_clm where GroupJobType = 'FSC' and Odometer = 5000)
	set @C_47 = (select count(*) from @t_unit_fsc_clm where GroupJobType = 'FSC' and Odometer = 10000)
	set @C_48 = (select count(*) from @t_unit_fsc_clm where GroupJobType = 'FSC' and Odometer = 20000)
	set @C_53 = (select count(*) from @t_unit_fsc_clm where GroupJobType = 'FSC' and Odometer = 30000)
	set @C_54 = (select count(*) from @t_unit_fsc_clm where GroupJobType = 'FSC' and Odometer = 40000)
	set @C_55 = (select count(*) from @t_unit_fsc_clm where GroupJobType = 'FSC' and Odometer = 50000)
	set @C_44 = @C_45 + @C_46 + @C_47 + @C_48 + @C_53 + @C_54 + @C_55
	set @C_50 = (select count(*) from @t_unit_fsc_clm where GroupJobType = 'CLM')
	
	set @C_52 = (select count(*) from @t_inv_tsk where JobType = 'PDI')

	-- Unit Others
	declare @t_unit_oth as table
	(
		InvoiceDate  varchar(20),
		ChassisCode  varchar(50), 
		ChassisNo    varchar(50),
		GroupJobType varchar(50),
		JobType      varchar(50),
		Odometer     numeric(10)
	)	
	insert into @t_unit_oth
	select distinct convert(varchar, a.InvoiceDate, 112) InvoiceDate
		 , a.ChassisCode, convert(varchar, a.ChassisNo) ChassisNo
		 , b.GroupJobType, a.JobType
		 , b.WarrantyOdometer as Odometer
	  from svTrnInvoice a
	 inner join svMstJob b
		on b.CompanyCode = a.CompanyCode
	   and b.ProductType = a.ProductType
	   and b.BasicModel = a.BasicModel
	   and b.JobType = a.JobType
	 where a.CompanyCode = @CompanyCode
	   and a.BranchCode = @BranchCode
	   and year(a.InvoiceDate) = @PeriodYear
	   and month(a.InvoiceDate) = @Month0
	   and b.GroupJobType not in ('FSC','CLM','RTN')

	set @C_51 = (select count(*) from @t_unit_oth where JobType = 'REWORK')

	-- Unit ALL
	declare @t_unit_all as table
	(
		InvoiceDate  varchar(20),
		ChassisCode  varchar(50), 
		ChassisNo    varchar(50)
	)	
	insert into @t_unit_all
	select distinct convert(varchar, a.JobOrderDate, 112) InvoiceDate
		 , a.ChassisCode, convert(varchar, a.ChassisNo) ChassisNo
	  from svTrnInvoice a
	 inner join svMstJob b
		on b.CompanyCode = a.CompanyCode
	   and b.ProductType = a.ProductType
	   and b.BasicModel = a.BasicModel
	   and b.JobType = a.JobType
	 where a.CompanyCode = @CompanyCode
	   and a.BranchCode = @BranchCode
	   and year(a.InvoiceDate) = @PeriodYear
	   and month(a.InvoiceDate) = @Month0

	-- Unit ALL
	declare @t_job_all as table
	(
		ChassisCode  varchar(50), 
		ChassisNo    varchar(50),
		GroupJobType varchar(20),
		JobType      varchar(20),
		Odometer     numeric(18,0),
		JobOrderDate datetime
	)	
	insert into @t_job_all
	select a.ChassisCode, a.ChassisNo
		 , case b.GroupJobType 
			 when 'RTN' then 
				(case 
					when b.WarrantyOdometer in (10000,30000,50000,70000,90000) then 'PB.A.1 3 5 7 9'
					when b.WarrantyOdometer in (20000,60000,100000) then 'PB.A.2 6 10'
					when b.WarrantyOdometer in (40000,80000) then 'PB.A.4 8'
					when b.WarrantyOdometer in (110000,130000,150000,170000,190000) then 'PB.B.1 3 5 7 9'
					when b.WarrantyOdometer in (120000,160000,110000) then 'PB.B.2 6 10'
					when b.WarrantyOdometer in (140000,180000) then 'PB.B.4 8'
					when b.WarrantyOdometer in (210000,230000,250000,270000,290000) then 'PB.B.1 3 5 7 9'
					when b.WarrantyOdometer in (220000,260000,210000) then 'PB.B.2 6 10'
					when b.WarrantyOdometer in (240000,280000) then 'PB.B.4 8'
					when b.WarrantyOdometer in (  5000, 15000, 25000, 35000, 45000, 55000, 65000, 75000, 85000, 95000) then 'PB.C.5000'
					when b.WarrantyOdometer in (105000,115000,125000,135000,145000,155000,165000,175000,185000,195000) then 'PB.C.5000'
					when b.WarrantyOdometer in (205000,215000,225000,235000,245000,155000,265000,275000,285000,295000) then 'PB.C.5000'
					else 'OTHERS'
					end
				)
			 when 'FSC' then 
				(case b.PdiFscSeq when 0 then 'PDI' else 'FS' end)
			 when 'CLM' then 'CLAIM'
			 else 'OTHERS'
		   end as GroupJobType
		 , case b.GroupJobType 
			 when 'RTN' then 'PB' + right(replicate(' ', 5) + convert(varchar, b.WarrantyOdometer), 6) 
			 when 'FSC' then 
				(case b.PdiFscSeq when 0 then 'PDI' else ('FS' + right(replicate(' ', 5) + convert(varchar, b.WarrantyOdometer), 6)) end)
			 when 'CLM' then 'CLAIM'
			 else 'OTHERS'
		   end as JobType
		 , b.WarrantyOdometer
		 , convert(datetime, convert(varchar, a.JobOrderDate, 112)) JobOrderDate  
	  from svTrnInvoice a  
	  left join svMstJob b
		on b.CompanyCode = a.CompanyCode
	   and b.BasicModel = a.BasicModel
	   and b.JobType = a.JobType  
	 where a.CompanyCode = @CompanyCode
	   and a.BranchCode = @BranchCode
	   and year(a.InvoiceDate) = @PeriodYear
	   and month(a.InvoiceDate) = @Month0
	 group by a.ChassisCode, a.ChassisNo, b.GroupJobType, b.PdiFscSeq, b.WarrantyOdometer, convert(varchar, a.JobOrderDate, 112)  

	set @C_49 = @C_50 + @C_51 + @C_52
	set @C_43 = (select count(*) from @t_job_all where GroupJobType = 'OTHERS')
	set @C_33 = @C_34 + @C_38 + @C_42 + @C_43

	-- D. Workshop Service Strength
	set @D_53 = isnull((
						select TotalStall
						  from svMstTarget
						 where CompanyCode=@CompanyCode
						   and BranchCode=@BranchCode
						   and ProductType=@ProductType
						   and PeriodYear=@PeriodYear
						   and PeriodMonth=@Month0
						 ),0)					
	set @D_55 = isnull((
					select count(*) from gnMstEmployee
					 where CompanyCode = @CompanyCode
					   and BranchCode  = @BranchCode
					   and TitleCode  in ('6', '11', '22')
					   and PersonnelStatus = '1'
					), 0)
	set @D_56 = isnull((
					select count(*) from gnMstEmployee
					 where CompanyCode = @CompanyCode
					   and BranchCode  = @BranchCode
					   and TitleCode  in ('4')
					   and PersonnelStatus = '1'
					), 0)
	set @D_57 = isnull((
					select count(*) from gnMstEmployee
					 where CompanyCode = @CompanyCode
					   and BranchCode  = @BranchCode
					   and TitleCode  in ('3')
					   and PersonnelStatus = '1'
					), 0)
	set @D_59 = isnull((
					select count(*) from gnMstEmployee
					 where CompanyCode = @CompanyCode
					   and BranchCode  = @BranchCode
					   and TitleCode  in ('8')
					   and PersonnelStatus = '1'
					), 0)
	set @D_60 = isnull((
					select count(*) from gnMstEmployee
					 where CompanyCode = @CompanyCode
					   and BranchCode  = @BranchCode
					   and TitleCode  in ('9')
					   and PersonnelStatus = '1'
					), 0)
	set @D_58 = @D_59 + @D_60
	set @D_61 = 0
	set @D_62 = isnull((
					select count(*) from gnMstEmployee
					 where CompanyCode = @CompanyCode
					   and BranchCode  = @BranchCode
					   and TitleCode  in ('7', '10')
					   and PersonnelStatus = '1'
					), 0)
	set @D_54 = @D_55 + @D_56 + @D_57 + @D_58 + @D_61 + @D_62

	-- E. Productivity Indicators
	set @E_01 = (case isnull(@D_58, 0) when 0 then 0 else 1.0 * isnull(@B_29, 0) / @D_58 end)
	set @E_02 = (case isnull(@D_60, 0) when 0 then 0 else 1.0 * isnull(@B_29, 0) / @D_60 end)
	set @E_03 = (case isnull(@D_53, 0) when 0 then 0 else 1.0 * isnull(@B_29, 0) / @D_53 end)
	set @E_04 = (case isnull(@D_57, 0) when 0 then 0 else 1.0 * isnull(@B_29, 0) / @D_57 end)
	set @E_05 = isnull((
						select TotalWorkingDays
						  from svMstTarget
						 where CompanyCode=@CompanyCode
						   and BranchCode=@BranchCode
						   and ProductType=@ProductType
						   and PeriodYear=@PeriodYear
						   and PeriodMonth=@Month0
						 ),0)					
					
	set @E_06 = (case isnull(@E_05, 0) when 0 then 0 else 1.0 * isnull(@E_01, 0) / @E_05 end)
	set @E_07 = (case isnull(@E_05, 0) when 0 then 0 else 1.0 * isnull(@E_02, 0) / @E_05 end)
	set @E_08 = (case isnull(@E_05, 0) when 0 then 0 else 1.0 * isnull(@E_03, 0) / @E_05 end)
	set @E_09 = (case isnull(@E_05, 0) when 0 then 0 else 1.0 * isnull(@E_04, 0) / @E_05 end)

	-- F. Service Retention & Marketing Activity
	set @F_72 = isnull((
						select SMRTarget
						  from svMstTarget
						 where CompanyCode=@CompanyCode
						   and BranchCode=@BranchCode
						   and ProductType=@ProductType
						   and PeriodYear=@PeriodYear
						   and PeriodMonth=@Month0
						 ),0)
	set @F_73 =	isnull((
					select count(*)
					  from svTrnDailyRetention
					 where 1 = 1
					   and CompanyCode = @CompanyCode
					   and BranchCode  = @BranchCode
					   --and PeriodYear  = @PeriodYear
					   --and PeriodMonth = @Month0
					   and year(ReminderDate) = @PeriodYear
					   and month(ReminderDate) = @Month0
					   and isnull(IsReminder,0) = 1
					), 0)
	set @F_74 =	isnull((
					select count(*)
					  from dbo.svTrnDailyRetention
					 where 1 = 1
					   and CompanyCode = @CompanyCode
					   and BranchCode  = @BranchCode
					   --and PeriodYear  = @PeriodYear
					   --and PeriodMonth = @Month0
					   and year(VisitDate) = @PeriodYear
					   and month(VisitDate) = @Month0
					   and isnull(IsVisited,0) = 1
					), 0)
	set @F_75 =	isnull((
					select count(*)
					  from dbo.svTrnDailyRetention
					 where 1 = 1
					   and CompanyCode = @CompanyCode
					   and BranchCode  = @BranchCode
					   --and PeriodYear  = @PeriodYear
					   --and PeriodMonth = @Month0
					   and year(FollowUpDate) = @PeriodYear
					   and month(FollowUpDate) = @Month0
					   and isnull(IsFollowUp,0) = 1
					), 0)
	set @F_76 =	isnull((
					select count(*)
					  from dbo.svTrnDailyRetention
					 where 1 = 1
					   and CompanyCode = @CompanyCode
					   and BranchCode  = @BranchCode
					   --and PeriodYear  = @PeriodYear --sebelum perubahan
					   --and PeriodMonth = @Month0     --sebelum perubahan
					   and year(BookingDate) = @PeriodYear --Sesudah Perubahan
					   and month(BookingDate) = @Month0    --Sesudah Perubahan
					   and isnull(IsBooked,0) = 1
					), 0)
	set @F_77 = (case isnull(@F_72, 0) when 0 then 0 else 100.0 * isnull(@F_74, 0) / @F_72 end)
	set @F_78 = (case isnull(@B_29, 0) when 0 then 0 else 100.0 * isnull(@F_75, 0) / @B_29 end)
	set @F_79 = (case isnull(@B_29, 0) when 0 then 0 else 100.0 * isnull(@F_76, 0) / @B_29 end)

	-- G. CSI Performance
	set @G_01 = null
	set @G_02 = null
	set @G_03 = null
	set @G_04 = null
	set @G_05 = null
	set @G_06 = null
	set @G_07 = isnull((
					select count(*)
					  from dbo.svTrnDailyRetention
					 where 1 = 1
					   and CompanyCode = @CompanyCode
					   and BranchCode  = @BranchCode
					   and PeriodYear  = @PeriodYear
					   and PeriodMonth = @Month0
					   and isnull(IsFollowUp,0) = 1
					   and isnull(IsSatisfied,0) = 1
					), 0)
	set @G_08 = isnull((
					select count(*)
					  from dbo.svTrnDailyRetention
					 where 1 = 1
					   and CompanyCode = @CompanyCode
					   and BranchCode  = @BranchCode
					   and PeriodYear  = @PeriodYear
					   and PeriodMonth = @Month0
					   and isnull(IsFollowUp,0) = 1
					   and isnull(IsSatisfied,0) = 0
					), 0)
	set @G_09 = isnull((
					select count(*)
					  from dbo.svTrnDailyRetention
					 where 1 = 1
					   and CompanyCode = @CompanyCode
					   and BranchCode  = @BranchCode
					   and PeriodYear  = @PeriodYear
					   and PeriodMonth = @Month0
					   and isnull(IsClosed,0) = 1
					), 0) 
	set @G_10 = (case isnull(@B_29, 0) when 0 then 0 else 1.0 * isnull(@G_07, 0)/ @B_29 end)
	set @G_11 = (case isnull(@B_29, 0) when 0 then 0 else 1.0 * isnull(@G_08, 0)/ @B_29 end)
	set @G_12 = (case isnull(@B_29, 0) when 0 then 0 else (isnull(@G_04, 0) + isnull(@G_08, 0))/ @B_29 end)

	-- Additional Calculation
	set @A_23 = (case isnull(@B_29, 0) when 0 then 0 else 1.0 * (isnull(@A_16, 0) + isnull(@A_17, 0))/ @B_29 end)
	set @A_24 = (case isnull(@D_58, 0) when 0 then 0 else @A_01 / @D_58 end)
	set @A_25 = (case isnull(@D_57, 0) when 0 then 0 else @A_01 / @D_57 end)
	set @A_26 = (case isnull(@D_53, 0) when 0 then 0 else @A_01 / @D_53 end)

	insert into @t_msidat select 'A', 'MSI_01', @Month0, @A_01
	insert into @t_msidat select 'A', 'MSI_02', @Month0, @A_02
	insert into @t_msidat select 'A', 'MSI_03', @Month0, @A_03
	insert into @t_msidat select 'A', 'MSI_04', @Month0, @A_04
	insert into @t_msidat select 'A', 'MSI_05', @Month0, @A_05
	insert into @t_msidat select 'A', 'MSI_06', @Month0, @A_06
	insert into @t_msidat select 'A', 'MSI_07', @Month0, @A_07
	insert into @t_msidat select 'A', 'MSI_08', @Month0, @A_08 
	insert into @t_msidat select 'A', 'MSI_09', @Month0, @A_09
	insert into @t_msidat select 'A', 'MSI_10', @Month0, @A_10
	insert into @t_msidat select 'A', 'MSI_11', @Month0, @A_11
	insert into @t_msidat select 'A', 'MSI_12', @Month0, @A_12
	insert into @t_msidat select 'A', 'MSI_13', @Month0, @A_13
	insert into @t_msidat select 'A', 'MSI_14', @Month0, @A_14
	insert into @t_msidat select 'A', 'MSI_15', @Month0, @A_15
	insert into @t_msidat select 'A', 'MSI_16', @Month0, @A_16
	insert into @t_msidat select 'A', 'MSI_17', @Month0, @A_17
	insert into @t_msidat select 'A', 'MSI_18', @Month0, @A_18
	insert into @t_msidat select 'A', 'MSI_19', @Month0, @A_19
	insert into @t_msidat select 'A', 'MSI_20', @Month0, @A_20
	insert into @t_msidat select 'A', 'MSI_21', @Month0, @A_21
	insert into @t_msidat select 'A', 'MSI_22', @Month0, @A_22
	insert into @t_msidat select 'A', 'MSI_23', @Month0, @A_23
	insert into @t_msidat select 'A', 'MSI_24', @Month0, @A_24
	insert into @t_msidat select 'A', 'MSI_25', @Month0, @A_25
	insert into @t_msidat select 'A', 'MSI_26', @Month0, @A_26
	insert into @t_msidat select 'A', 'MSI_27', @Month0, @A_27

	insert into @t_msidat select 'B', 'MSI_28', @Month0, @B_28
	insert into @t_msidat select 'B', 'MSI_29', @Month0, @B_29
	insert into @t_msidat select 'B', 'MSI_30', @Month0, @B_30
	insert into @t_msidat select 'B', 'MSI_31', @Month0, @B_31
	insert into @t_msidat select 'B', 'MSI_32', @Month0, @B_32

	insert into @t_msidat select 'C', 'MSI_33', @Month0, @C_33
	insert into @t_msidat select 'C', 'MSI_34', @Month0, @C_34
	insert into @t_msidat select 'C', 'MSI_35', @Month0, @C_35
	insert into @t_msidat select 'C', 'MSI_36', @Month0, @C_36
	insert into @t_msidat select 'C', 'MSI_37', @Month0, @C_37
	insert into @t_msidat select 'C', 'MSI_38', @Month0, @C_38
	insert into @t_msidat select 'C', 'MSI_39', @Month0, @C_39
	insert into @t_msidat select 'C', 'MSI_40', @Month0, @C_40
	insert into @t_msidat select 'C', 'MSI_41', @Month0, @C_41
	insert into @t_msidat select 'C', 'MSI_42', @Month0, @C_42
	insert into @t_msidat select 'C', 'MSI_43', @Month0, @C_43
	insert into @t_msidat select 'C', 'MSI_44', @Month0, @C_44
	insert into @t_msidat select 'C', 'MSI_45', @Month0, @C_45
	insert into @t_msidat select 'C', 'MSI_46', @Month0, @C_46
	insert into @t_msidat select 'C', 'MSI_47', @Month0, @C_47
	insert into @t_msidat select 'C', 'MSI_48', @Month0, @C_48
	insert into @t_msidat select 'C', 'MSI_49', @Month0, @C_53
	insert into @t_msidat select 'C', 'MSI_50', @Month0, @C_54
	insert into @t_msidat select 'C', 'MSI_51', @Month0, @C_55
	insert into @t_msidat select 'C', 'MSI_52', @Month0, @C_49
	insert into @t_msidat select 'C', 'MSI_53', @Month0, @C_50
	insert into @t_msidat select 'C', 'MSI_54', @Month0, @C_51
	insert into @t_msidat select 'C', 'MSI_55', @Month0, @C_52

	insert into @t_msidat select 'D', 'MSI_56', @Month0, @D_53
	insert into @t_msidat select 'D', 'MSI_57', @Month0, @D_54
	insert into @t_msidat select 'D', 'MSI_58', @Month0, @D_55
	insert into @t_msidat select 'D', 'MSI_59', @Month0, @D_56
	insert into @t_msidat select 'D', 'MSI_60', @Month0, @D_57
	insert into @t_msidat select 'D', 'MSI_61', @Month0, @D_58
	insert into @t_msidat select 'D', 'MSI_62', @Month0, @D_59
	insert into @t_msidat select 'D', 'MSI_63', @Month0, @D_60
	insert into @t_msidat select 'D', 'MSI_64', @Month0, @D_61
	insert into @t_msidat select 'D', 'MSI_65', @Month0, @D_62

	insert into @t_msidat select 'E', 'MSI_66', @Month0, @E_01
	insert into @t_msidat select 'E', 'MSI_67', @Month0, @E_02
	insert into @t_msidat select 'E', 'MSI_68', @Month0, @E_03
	insert into @t_msidat select 'E', 'MSI_69', @Month0, @E_04
	insert into @t_msidat select 'E', 'MSI_70', @Month0, @E_05
	insert into @t_msidat select 'E', 'MSI_71', @Month0, @E_06
	insert into @t_msidat select 'E', 'MSI_72', @Month0, @E_07
	insert into @t_msidat select 'E', 'MSI_73', @Month0, @E_08
	insert into @t_msidat select 'E', 'MSI_74', @Month0, @E_09

	insert into @t_msidat select 'F', 'MSI_75', @Month0, @F_72
	insert into @t_msidat select 'F', 'MSI_76', @Month0, @F_73
	insert into @t_msidat select 'F', 'MSI_77', @Month0, @F_74
	insert into @t_msidat select 'F', 'MSI_78', @Month0, @F_75
	insert into @t_msidat select 'F', 'MSI_79', @Month0, @F_76
	insert into @t_msidat select 'F', 'MSI_80', @Month0, @F_77
	insert into @t_msidat select 'F', 'MSI_81', @Month0, @F_78
	insert into @t_msidat select 'F', 'MSI_82', @Month0, @F_79

	insert into @t_msidat select 'G', 'MSI_83', @Month0, @G_01
	insert into @t_msidat select 'G', 'MSI_84', @Month0, @G_02
	insert into @t_msidat select 'G', 'MSI_85', @Month0, @G_03
	insert into @t_msidat select 'G', 'MSI_86', @Month0, @G_04
	insert into @t_msidat select 'G', 'MSI_87', @Month0, @G_05
	insert into @t_msidat select 'G', 'MSI_88', @Month0, @G_06
	insert into @t_msidat select 'G', 'MSI_89', @Month0, @G_07
	insert into @t_msidat select 'G', 'MSI_90', @Month0, @G_08
	insert into @t_msidat select 'G', 'MSI_91', @Month0, @G_09
	insert into @t_msidat select 'G', 'MSI_92', @Month0, @G_10
	insert into @t_msidat select 'G', 'MSI_93', @Month0, @G_11
	insert into @t_msidat select 'G', 'MSI_94', @Month0, @G_12
	
	set @Month0 = @Month0 + 1
	delete @t_inv_tsk
	delete @t_inv_itm
	delete @t_inv_unit
	delete @t_inv_unit_pdi
	delete @t_inv_unit_exc
	delete @t_inv_job
end	

insert into @t_msigrp values ('A', 'A. Sales Revenue')
insert into @t_msigrp values ('B', 'B. No of Unit Intake')
insert into @t_msigrp values ('C', 'C. No of Job Type')
insert into @t_msigrp values ('D', 'D. Workshop Service Strength')
insert into @t_msigrp values ('E', 'E. Productivity Indicators')
insert into @t_msigrp values ('F', 'F. Service Retention & Marketing Activity')
insert into @t_msigrp values ('G', 'G. CSI Performance')

delete SvHstSzkMSI
 where CompanyCode = @CompanyCode
   and BranchCode = @BranchCode
   and PeriodYear = @PeriodYear
   and PeriodMonth in (select distinct MsiMonth from @t_msidat)

insert into SvHstSzkMSI (CompanyCode, BranchCode, PeriodYear, PeriodMonth, SeqNo, MsiGroup, MsiDesc, Unit, MsiData, CreatedBy, CreatedDate)
select @CompanyCode CompanyCode, @BranchCode BranchCode
	 , @PeriodYear PeriodYear, a.MsiMonth PeriodMonth
	 , convert(int, right(MsiCode, 2)) as SeqNo
	 , MsiGrDesc as MsiGroup, Description as MsiDesc, DescriptionEng Unit, a.MsiData
	 , @UserID, GETDATE ()
  from (select MsiGrCode, MsiCode, MsiMonth, MsiData from @t_msidat) a
  left join svMstRefferenceService b
    on b.CompanyCode = @CompanyCode
   and b.RefferenceType = 'MSIDATA'
   and b.RefferenceCode = a.MsiCode
  left join @t_msigrp c
    on c.MsiGroup = a.MsiGrCode


select CompanyCode, BranchCode, PeriodYear, SeqNo, MsiGroup, MsiDesc, Unit
	 , (isnull( [1], 0) + isnull( [2], 0) + isnull( [3], 0) 
	 +  isnull( [4], 0) + isnull( [5], 0) + isnull( [6], 0) 
	 +  isnull( [7], 0) + isnull( [8], 0) + isnull( [9], 0)
	 +  isnull([10], 0) + isnull([11], 0) + isnull([12], 0)) / (@Month2 - @Month1 + 1) as Average
	 , (isnull( [1], 0) + isnull( [2], 0) + isnull( [3], 0) 
	 +  isnull( [4], 0) + isnull( [5], 0) + isnull( [6], 0) 
	 +  isnull( [7], 0) + isnull( [8], 0) + isnull( [9], 0)
	 +  isnull([10], 0) + isnull([11], 0) + isnull([12], 0)) as Total
	 , isnull( [1], 0)  [1]
	 , isnull( [2], 0)  [2]
	 , isnull( [3], 0)  [3]
	 , isnull( [4], 0)  [4]
	 , isnull( [5], 0)  [5]
	 , isnull( [6], 0)  [6]
	 , isnull( [7], 0)  [7]
	 , isnull( [8], 0)  [8]
	 , isnull( [9], 0)  [9]
	 , isnull([10], 0) [10]
	 , isnull([11], 0) [11]
	 , isnull([12], 0) [12]
  from (
	select CompanyCode, BranchCode, PeriodYear, PeriodMonth, SeqNo, MsiGroup, MsiDesc, Unit, MsiData from SvHstSzkMSI
	 where CompanyCode = @CompanyCode
	   and BranchCode = @BranchCode
	   and PeriodYear = @PeriodYear
	   and PeriodMonth >= @Month1
	   and PeriodMonth <= @Month2
  )#
 pivot (sum(MsiData) for PeriodMonth in ([1], [2], [3], [4], [5], [6], [7], [8], [9], [10], [11], [12])) as pvt
 order by pvt.MsiGroup, pvt.SeqNo
*/});


var startTasks= function(callback)
{
    var taskJobs = [];		
    config.conn().forEach(listWorker);		
    async.series(taskJobs, function (err, docs) {
        if (err) config.log("Tasks", err);			
        if (callback) callback();
    });
    function listWorker(cfg){
        if(cfg.DealerCode == CurrentDealerCode)
        {
            taskJobs.push(function(callback){start(cfg, callback);});
        }			 
    }				
}

var download = function(url, dest, cb) {
  var file = fs.createWriteStream(dest);
  var request = http.get(url, function(response) {
    response.pipe(file);
    file.on('finish', function() {
      file.close(cb);  // close() is async, call cb after close completes.
    });
  }).on('error', function(err) { // Handle errors
    fs.unlink(dest); // Delete the file async. (But we don't check the result)
    if (cb) cb(err.message);
  });
};


var start2 = function (cfg, callback) 
{
    log("Starting " + TaskName + " for " + CurrentDealerCode); 

	var xSQL= [], sql = SQL.split('\nGO');	
    sql.forEach(ExecuteSQL);

    async.series(xSQL, function (err, docs) {
        if (err) log("ERROR" + err);
		log('done');
    });	
	
	function ExecuteSQL(s){
        xSQL.push(function(callback){		
			
            sqlODBC.query(cfg.ConnString,s , function (err, data) { 
				if (err) { 
					log("ERROR >> " + err);
				} else {
					if (data && data.length == 1) {
						if (data[0].info !== undefined)
							log(data[0].info);
					}
				}
                callback(); 
            });		
        });
    }	
}

var start = function (cfg, callback) 
{
    log("Starting " + TaskName + " for " + CurrentDealerCode); 

	var file = path.join(__dirname,  TaskNo + ".sql");
	log(file);
	
    fs.writeFileSync(file, SQL );
	
	var i = cfg.ConnString.indexOf('}');
	var s = (cfg.ConnString.substr(i+2));
	
	s = s.replace('Server=','');
	i = s.indexOf(';');
	var server = s.substr(0,i);
	s = s.substr(i+1);
	s = s.replace('Database=','');
	i = s.indexOf(';');
	var dbname = s.substr(0,i);
	s = s.substr(i+1);
	s = s.replace('Uid=','');
	i = s.indexOf(';');
	var userid = s.substr(0,i);
	s = s.substr(i+1);
	s = s.replace('Pwd=','');
	var password = s;
		
	var StartTime = moment().format("YYYY-MM-DD HH:mm:ss");
	
    run_shell ('osql', ['-S',server,'-d',dbname,'-U',userid,'-P',password,'-i',file] ,function(err, code, result) {
	
        var output = result;						
        var FinishTime = moment().format("YYYY-MM-DD HH:mm:ss");                        
        var errDesc = err || '';
		
		log(result);
        
    });	
	
	log('done');
	
}

function log(info) {
    var file = path.join(__dirname,  TaskNo + "-" + TaskName + ".log");
    fs.appendFileSync(file, moment().format("YYYY-MM-DD HH:mm:ss") + " : " + info + "\n");
    console.log(moment().format("YYYY-MM-DD HH:mm:ss"), ":", info );
}

function run_shell(cmd, args, cb)
{
	var proc = require('child_process').spawn(cmd, args)
	var result = '', strErr = '';
	
	proc.stdout.on('data', function (data) {
		result += data;
	});
	proc.stderr.on('data', function (data) {
		strErr += data;
	});
	proc.on('close', function (code) {
		cb(strErr,code,result);
	});
}

startTasks();