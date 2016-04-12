if object_id('usprpt_SvRpReport021V3Web') is not null
       drop procedure usprpt_SvRpReport021V3Web

CREATE procedure [dbo].[usprpt_SvRpReport021V3Web]
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

---- A. Sales Revenue (SR)
--declare @A_01 numeric(18, 4) -- Total Labor Sales Revenue ( 2 + 3 + 4 )
--declare @A_02 numeric(18, 4) --   Labor Sales - Chargeable to Customer CPUS (External)
--declare @A_03 numeric(18, 4) --   Labor Sales - Non Chargeable (Warranty, FS1, FS2, FS3, FS4, FS5, FS6, FS7)
--declare @A_04 numeric(18, 4) --   Labor Sales - Non Chargeable (Internal, PDI)
--declare @A_05 numeric(18, 4) -- Total Parts Sales Revenue ( 6 + 7 + 8 )
--declare @A_06 numeric(18, 4) --   Parts Sales - Chargeable to Customer CPUS (External)
--declare @A_07 numeric(18, 4) --   Parts Sales - Non Chargeable (Warranty, FS1)
--declare @A_08 numeric(18, 4) --   Parts Sales - Non Chargeable (Internal, PDI)
--declare @A_09 numeric(18, 4) -- Total Counter Parts Sales Revenue
--declare @A_10 numeric(18, 4) -- Total Lubricant Sales Revenue ( 11 + 12 + 13 )  
--declare @A_11 numeric(18, 4) --   Lubricant Sales - Chargeable to Customer CPUS (External)
--declare @A_12 numeric(18, 4) --   Lubricant Sales - Non Chargeable (Warranty, FS1)
--declare @A_13 numeric(18, 4) --   Lubricant Sales - Non Chargeable (Internal, PDI)
--declare @A_14 numeric(18, 4) -- Total Sublet Sales Revenue
--declare @A_15 numeric(18, 4) -- Total Service Sales Revenue ( 16 + 17 + 18 )    
--declare @A_16 numeric(18, 4) --   Service Sales - Chargeable to Customer CPUS (External)
--declare @A_17 numeric(18, 4) --   Service Sales - Non Chargeable (Warranty, FS1)
--declare @A_18 numeric(18, 4) --   Service Sales - Non Chargeable (Internal, PDI)
--declare @A_19 numeric(18, 4) -- Hours Sold  
--declare @A_20 numeric(18, 4) -- Total Available Hours  
--declare @A_21 numeric(18, 4) -- Hours Sold / Available Hours  
--declare @A_22 numeric(18, 4) -- Service Revenue Targer Per Unit (Based on RKA) 
--declare @A_23 numeric(18, 4) -- Service Revenue per Unit exclude PDI (CPUS, Warranty, FS1) ((16 + 17) / 29)
--declare @A_24 numeric(18, 4) -- Labour  Sales Turnover / Productive staff (Technician & Foreman)  (1 / 61)
--declare @A_25 numeric(18, 4) -- Labour Sales Turnover / Service Advisors (1 / 60)
--declare @A_26 numeric(18, 4) -- Labour Sales Turnover / Stall (1 / 56)
--declare @A_27 numeric(18, 4) -- Absorption Rate
---- B. No of Unit Intake
--declare @B_28 numeric(18, 4) -- Unit Entry Target
--declare @B_29 numeric(18, 4) -- No of Work Order ( 30 + 31 )
--declare @B_30 numeric(18, 4) -- Passenger Car
--declare @B_31 numeric(18, 4) -- Commercial Vehicle 
--declare @B_32 numeric(18, 4) -- PDI
---- C. No of JobType
--declare @C_33 numeric(18, 4) -- Chargeable /  Customer Paid Unit Service (CPUS) (34 + 38 + 42 + 43)
--declare @C_34 numeric(18, 4) --   Periodical Maintenance under Warranty Period (= 100,000 km / = 3 Years) (sum of 35 to 37)
--declare @C_35 numeric(18, 4) --     10,000*; 30,000; 50,000; 70,000; & 90,000 km
--declare @C_36 numeric(18, 4) --     20,000*; 60,000 & 100,000 km
--declare @C_37 numeric(18, 4) --     40,000 & 80,000 km 
--declare @C_38 numeric(18, 4) --   Periodical Maintenance out of Warranty Period (> 100,000 km / >3 years) (sum of 39 to 41)
--declare @C_39 numeric(18, 4) --     +10,000; +30,000; +50,000; +70,000; & +90,000 km
--declare @C_40 numeric(18, 4) --     +20,000; +60,000 & +100,000 km
--declare @C_41 numeric(18, 4) --     +40,000 & +80,000 km
--declare @C_42 numeric(18, 4) --   5,000 km multiplier & above
--declare @C_43 numeric(18, 4) --   G/R Non Periodical Maintenance and Others
--declare @C_44 numeric(18, 4) -- No. of Free Service (FS 1, FS 2, and FS 3) (sum of 45 to 47)
--declare @C_45 numeric(18, 4) --   KSG  1,000 km
--declare @C_46 numeric(18, 4) --   KSG  5,000 km
--declare @C_47 numeric(18, 4) --   KSG 10,000 km
--declare @C_48 numeric(18, 4) --   KSG 20,000 km
--declare @C_53 numeric(18, 4) --   KSG 30,000 km
--declare @C_54 numeric(18, 4) --   KSG 40,000 km
--declare @C_55 numeric(18, 4) --   KSG 50,000 km
--declare @C_49 numeric(18, 4) -- Non-chargeable Repair (Warranty, Repeated Job, PDI) (Sum of 49 to 51)
--declare @C_50 numeric(18, 4) --   No. of Warranty Repair
--declare @C_51 numeric(18, 4) --   Repeat Job (Rework)
--declare @C_52 numeric(18, 4) --   No. of PDI intake
---- D. Workshop Service Strength
--declare @D_53 numeric(18, 4) -- No. of Working Stalls ( Available stalls Exclude Inspection Stall and Washing Stall)
--declare @D_54 numeric(18, 4) -- Total No. of Staff (55 + 56 + 57 + 58 + 61 + 62)
--declare @D_55 numeric(18, 4) --   No. of Admin & Support Staff
--declare @D_56 numeric(18, 4) --   No. of Service Relation Officer (SRO)
--declare @D_57 numeric(18, 4) --   No. of Service Advisors
--declare @D_58 numeric(18, 4) --   No. of Productive Staff (59 + 60)
--declare @D_59 numeric(18, 4) --     No. of Foreman
--declare @D_60 numeric(18, 4) --     No. of Technician
--declare @D_61 numeric(18, 4) --   No. of PDI Staff
--declare @D_62 numeric(18, 4) --   No. of Part Staff
---- E. Productivity Indicator
--declare @E_01 numeric(18, 4)
--declare @E_02 numeric(18, 4)
--declare @E_03 numeric(18, 4)
--declare @E_04 numeric(18, 4)
--declare @E_05 numeric(18, 4)
--declare @E_06 numeric(18, 4)
--declare @E_07 numeric(18, 4)
--declare @E_08 numeric(18, 4)
--declare @E_09 numeric(18, 4)
---- F. Activity
--declare @F_72 numeric(18, 4)
--declare @F_73 numeric(18, 4)
--declare @F_74 numeric(18, 4)
--declare @F_75 numeric(18, 4)
--declare @F_76 numeric(18, 4)
--declare @F_77 numeric(18, 4)
--declare @F_78 numeric(18, 4)
--declare @F_79 numeric(18, 4)
---- G. 
--declare @G_01 numeric(18, 4)
--declare @G_02 numeric(18, 4)
--declare @G_03 numeric(18, 4)
--declare @G_04 numeric(18, 4)
--declare @G_05 numeric(18, 4)
--declare @G_06 numeric(18, 4)
--declare @G_07 numeric(18, 4)
--declare @G_08 numeric(18, 4)
--declare @G_09 numeric(18, 4)
--declare @G_10 numeric(18, 4)
--declare @G_11 numeric(18, 4)
--declare @G_12 numeric(18, 4)

-- A. Sales Revenue (SR)
declare @A_01 numeric(18, 4) --Total Labour Sales Revenue (2 + 3 + 4)
declare @A_02 numeric(18, 4) --     Labour Sales - Chargeable to customer CPUS (External)
declare @A_03 numeric(18, 4) --     Labour Sales - Non-chargeable (Warranty, FS 1K, FS 5K, FS 10K, FS 20K, FS 30K, FS 40K, & FS 50K)
declare @A_04 numeric(18, 4) --     Labour Sales - Non-chargeable (Internal, PDI)
declare @A_05 numeric(18, 4) --Total Workshop Parts Sales Revenue (6 + 7 + 8)
declare @A_06 numeric(18, 4) --     Parts Sales - Chargeable to customer CPUS (External)
declare @A_07 numeric(18, 4) --     Parts Sales - Non-chargeable (Warranty)
declare @A_08 numeric(18, 4) --     Parts Sales - Non-chargeable (Internal, PDI)
declare @A_09 numeric(18, 4) --Total Counter Part Sales Revenue
declare @A_10 numeric(18, 4) --Total Lubricant Sales Revenue (11 + 12 + 13)
declare @A_11 numeric(18, 4) --     Lubricant Sales - Chargeable to customer CPUS (External)
declare @A_12 numeric(18, 4) --     Lubricant Sales - Non-chargeable (Warranty, FS 1K - commercial car)
declare @A_13 numeric(18, 4) --     Lubricant Sales - Non-chargeable (Internal, PDI)
declare @A_14 numeric(18, 4) -- Total Suzuki Genuine Accessories Sales Revenue - Chargeable to Customer CPUS (External) (15+16)
declare @A_15 numeric(18, 4) -- SGA Accessories Parts Sales
declare @A_16 numeric(18, 4) -- SGA Materials Sales
declare @A_17 numeric(18, 4) --Total Sublet Sales Revenue - Chargebale to Customer CPUS (External)
declare @A_18 numeric(18, 4) --Total Service Sales Revenue (17 + 18 + 19)
declare @A_19 numeric(18, 4) --     Total Service Sales - Chargeable to customer CPUS (External) (2 + 6 + 11 + 14 + 17)
declare @A_20 numeric(18, 4) --     Total Service Sales - Non-chargeable (Warranty) (3 +  7 + 12)
declare @A_21 numeric(18, 4) --     Total Service Sales - Non-chargeable (Internal, PDI) (4 + 8 + 13)
declare @A_22 numeric(18, 4) --Hours Sold
declare @A_23 numeric(18, 4) --Total Available Hours
declare @A_24 numeric(18, 4) --Hours Sold / Available hours (20 / 21)
declare @A_25 numeric(18, 4) --Service Revenue Target per Unit (Based on RKA)
declare @A_26 numeric(18, 4) --Service Revenue per Unit exclude PDI (CPUS, Warranty, FS) ((19 + 20) / 32)
declare @A_27 numeric(18, 4) --Labour  Sales Turnover / Productive staff (Technician & Foreman)  (1 / 67) 
declare @A_28 numeric(18, 4) --Labour Sales Turnover / Service Advisors (1 / 66) 
declare @A_29 numeric(18, 4) --Labour Sales Turnover / Stall (1 / 58)
declare @A_30 numeric(18, 4) --Absorption Rate

declare @B_01 numeric(18, 4) --Unit Entry Target (Based on RKA)
declare @B_02 numeric(18, 4) --No. of Unit/vehicle Intakes exclude PDI (33 + 34)
declare @B_03 numeric(18, 4) --     Passenger Car (exclude Angkot, Pickup, chassis)
declare @B_04 numeric(18, 4) --     Commercial Vehicle (Angkot, Pickup, Chassis)
declare @B_05 numeric(18, 4) --     PDI

declare @C_01 numeric(18, 4) --Total Customer Paid Unit Service exclude Free Sevice (37 + 41 + 45 + 46 + 47)
declare @C_02 numeric(18, 4) --     Periodical Maintenance under Warranty Period (= 100,000 km/= 3 Years) (sum of 38 to 40)
declare @C_03 numeric(18, 4) --         10,000*; 30,000*; 50,000*; 70,000; & 90,000 km
declare @C_04 numeric(18, 4) --         20,000*; 60,000; & 100,000 km
declare @C_05 numeric(18, 4) --         40,000; & 80,000 km 
declare @C_06 numeric(18, 4) --     Periodical Maintenance out of Warranty Period (>100,000 km/>3 years) (sum of 42 to 44)
declare @C_07 numeric(18, 4) --         +10,000; +30,000; +50,000; +70,000; & +90,000 km
declare @C_08 numeric(18, 4) --         +20,000; +60,000; & +100,000 km
declare @C_09 numeric(18, 4) --         +40,000; & +80,000 km
declare @C_10 numeric(18, 4) --     5,000 km multiplier & above
declare @C_11 numeric(18, 4) --     G/R Non Periodical Maintenance and Others exclude Accessories Installment
declare @C_12 numeric(18, 4) --Accessories Installment (SGA)
declare @C_13 numeric(18, 4) --No. of Free Service (FS 1K, FS 5K, FS 10K, FS 20K, FS 30K, FS 40K, & FS 50K) (sum of 49 to 55)
declare @C_14 numeric(18, 4) --     FS 1K (1,000 km)
declare @C_15 numeric(18, 4) --     FS 5K (5,000 km)
declare @C_16 numeric(18, 4) --     FS 10K (10,000 km)
declare @C_17 numeric(18, 4) --     FS 20K (20,000 km)
declare @C_18 numeric(18, 4) --     FS 30K (30,000 km)
declare @C_19 numeric(18, 4) --     FS 40K (40,000 km)
declare @C_20 numeric(18, 4) --     FS 50K (50,000 km)
declare @C_21 numeric(18, 4) --Non-chargeable Repair (Warranty, Repeated Job, PDI) (Sum of 57 to 59)
declare @C_22 numeric(18, 4) --     No. of Warranty Repair
declare @C_23 numeric(18, 4) --     Repeat Job (Rework)
declare @C_24 numeric(18, 4) --     No. of PDI intake

declare @D_01 numeric(18, 4) --No. of Available Stalls (59 + 60)
declare @D_02 numeric(18, 4) -- No. of Regular Stalls ( Available stalls Exclude Washing Stall and EM Stalls)
declare @D_03 numeric(18, 4) -- No. of Express Maintenance Stalls (Installed)
declare @D_04 numeric(18, 4) --Total No. of Staff (64 + 65 + 66 + 67 + 70 + 71)
declare @D_05 numeric(18, 4) --     No. of Admin & Support Staff
declare @D_06 numeric(18, 4) --     No. of Service Relation Officer (SRO)
declare @D_07 numeric(18, 4) --     No. of Service Advisors (SA)
declare @D_08 numeric(18, 4) --     No. of Productive Staff (68 + 69)
declare @D_09 numeric(18, 4) --         No. of Foreman
declare @D_10 numeric(18, 4) --         No. of Technician
declare @D_11 numeric(18, 4) --     No. of PDI Staff
declare @D_12 numeric(18, 4) --     No. of Part Staff

-- E. Productivity Indicator
declare @E_01 numeric(18, 4) --Repair Unit per Productive staff (32 / 67)
declare @E_02 numeric(18, 4)--Repair Unit per Technician (32 / 69)
declare @E_03 numeric(18, 4)--Repair Unit per Available Stall (32 / 60)
declare @E_04 numeric(18, 4)--No. of Customer / Service Advisor (29 / 60)
declare @E_05 numeric(18, 4)--No. of Workdays
declare @E_06 numeric(18, 4)--Repair Unit per Productive staff / day (72 / 76)
declare @E_07 numeric(18, 4)--Repair Unit per Technician / day (73 / 76)
declare @E_08 numeric(18, 4)--Repair Unit per Available Stall/day (74 / 76)
declare @E_09 numeric(18, 4)--No. of Customer / Service Advisor / day (75 / 76)

declare @F_01 numeric(18, 4)--No. of Service Reminder Target
declare @F_02 numeric(18, 4)--Service Reminder
declare @F_03 numeric(18, 4)--No. of work orders/vehicle intake from Reminder
declare @F_04 numeric(18, 4)--No. of post service follow-ups
declare @F_05 numeric(18, 4)--Service Bookings
declare @F_06 numeric(18, 4)--% Service Reminder unit intake to Reminder Target (83 / 81) 
declare @F_07 numeric(18, 4)--% Post Service Follow-ups to repair unit (work order) (84 / 32)
declare @F_08 numeric(18, 4)--% Service Bookings to repair unit (work order) (85 / 32)

-- G. 
declare @G_01 numeric(18, 4)--CSI Score
declare @G_02 numeric(18, 4)--Total instant feedback form received
declare @G_03 numeric(18, 4)--Total instant feedback form received ratio (to work order) (90 / 32)
declare @G_04 numeric(18, 4)--Total Complaints Received  (93 + 94)
declare @G_05 numeric(18, 4)--     Direct/verbal complaints received
declare @G_06 numeric(18, 4)--     Indirect/non-verbal complaints received
declare @G_07 numeric(18, 4)--Total No. of post service follow up 'Satisfied'
declare @G_08 numeric(18, 4)--Total No. of post service follow up 'Not-Satisfied'
declare @G_09 numeric(18, 4)--Total No.'Not-Satisfied' Closed 
declare @G_10 numeric(18, 4)--Ratio of post service follow ups 'Satisfied' per vehicle intake (95 / 32)
declare @G_11 numeric(18, 4)--Ratio of post service follow ups 'Not Satisfied' per vehicle intake (96 / 32)
declare @G_12 numeric(18, 4)--Ratio of total complaints & post service follow ups 'Not Satisfied' per vehicle intake ((92 + 96) / 32)

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

	--Total Labour Sales Revenue (2 + 3 + 4)
	set @A_01 = ISNULL((select sum(LbrDppAmt) from @t_inv_tsk where IsSubCon = 0),0)
	--  Labour Sales - Non-chargeable (Warranty, FS 1K, FS 5K, FS 10K, FS 20K, FS 30K, FS 40K, & FS 50K)
	set @A_03 = ISNULL((select sum(LbrDppAmt) from @t_inv_tsk where IsSubCon = 0 and (substring(InvoiceNo, 1 ,3) = 'INW' or substring(InvoiceNo, 1 ,3) = 'INF' and JobType != 'PDI')),0)
	--  Labour Sales - Non-chargeable (Internal, PDI)
	set @A_04 = ISNULL((select sum(LbrDppAmt) from @t_inv_tsk where IsSubCon = 0 and (substring(InvoiceNo, 1 ,3) = 'INI' or substring(InvoiceNo, 1 ,3) = 'INF' and JobType = 'PDI')), 0)	
	--Labour Sales - Chargeable to customer CPUS (External)
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

	--Total Workshop Parts Sales Revenue (6 + 7 + 8)
	set @A_05 = isnull((select sum(SprAmt) from @t_inv_itm where IsSublet = 0 and GroupTpgo = 'SPAREPART'), 0)
	--Parts Sales - Chargeable to customer CPUS (External)
	set @A_06 = isnull((select sum(SprAmt) from @t_inv_itm where IsSublet = 0 and GroupTpgo = 'SPAREPART' and GroupJob = 'CPUS'), 0)
	--Parts Sales - Non-chargeable (Warranty)
	set @A_07 = isnull((select sum(SprAmt) from @t_inv_itm where IsSublet = 0 and GroupTpgo = 'SPAREPART' and GroupJob = 'CLM,FSC'), 0)
	--Parts Sales - Non-chargeable (Internal, PDI)
	set @A_08 = isnull((select sum(SprAmt) from @t_inv_itm where IsSublet = 0 and GroupTpgo = 'SPAREPART' and GroupJob = 'INT,PDI'), 0)
	--Total Counter Parts Sales Revenue
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
	--Total Lubricant Sales Revenue (11 + 12 + 13)
	set @A_10 = isnull((select sum(SprAmt) from @t_inv_itm where IsSublet = 0 and GroupTpgo != 'SPAREPART'), 0)
	--Lubricant Sales - Chargeable to customer CPUS (External)
	set @A_11 = isnull((select sum(SprAmt) from @t_inv_itm where IsSublet = 0 and GroupTpgo != 'SPAREPART' and GroupJob = 'CPUS'), 0)
	-- Lubricant Sales - Non-chargeable (Warranty, FS 1K - commercial car)
	set @A_12 = isnull((select sum(SprAmt) from @t_inv_itm where IsSublet = 0 and GroupTpgo != 'SPAREPART' and GroupJob = 'CLM,FSC'), 0)
	-- Lubricant Sales - Non Chargeable (Internal, PDI)
	set @A_13 = isnull((select sum(SprAmt) from @t_inv_itm where IsSublet = 0 and GroupTpgo != 'SPAREPART' and GroupJob = 'INT,PDI'), 0)
	
	-- SGA Accessories Parts Sales
	set @A_15 = isnull((select x.DPPAmt from (select 
					inv.CompanyCode,
					inv.BranchCode,
					year(inv.InvoiceDate) PeriodYear,
					MONTH(inv.InvoiceDate) PeriodMonth,
					sum((invi.SupplyQty * invi.RetailPrice) - (invi.RetailPrice*(invi.discPct/100))) as DPPAmt
				from svTrnInvItem invi	
				inner join [svTrnInvoice] inv on inv.CompanyCode=invi.CompanyCode and inv.BranchCode=invi.BranchCode and inv.InvoiceNo=invi.InvoiceNo
				where invi.TypeOfGoods = '2'
				and invi.companycode=@CompanyCode
				and invi.branchCode=@BranchCode
				and year(inv.InvoiceDate)=@PeriodYear
				and MONTH(inv.InvoiceDate)=@Month0
				group by inv.CompanyCode,inv.BranchCode,year(inv.InvoiceDate),MONTH(inv.InvoiceDate)
				)x),0)

	-- SGA Materials Sales
	set @A_16= isnull((select x.DPPAmt from (select 
					inv.CompanyCode,
					inv.BranchCode,
					year(inv.InvoiceDate) PeriodYear,
					MONTH(inv.InvoiceDate) PeriodMonth,
					sum((invi.SupplyQty * invi.RetailPrice) - (invi.RetailPrice*(invi.discPct/100))) as DPPAmt
				from svTrnInvItem invi	
				inner join [svTrnInvoice] inv on inv.CompanyCode=invi.CompanyCode and inv.BranchCode=invi.BranchCode and inv.InvoiceNo=invi.InvoiceNo
				where invi.TypeOfGoods in (5,6)
				and invi.companycode=@CompanyCode
				and invi.branchCode=@BranchCode
				and year(inv.InvoiceDate)=@PeriodYear
				and MONTH(inv.InvoiceDate)=@Month0
				group by inv.CompanyCode,inv.BranchCode,year(inv.InvoiceDate),MONTH(inv.InvoiceDate)
				)x),0)
	
	-- Total Suzuki Genuine Accessories Sales Revenue - Chargeable to Customer CPUS (External) (15+16)
	set @A_14= @A_15 + @A_16
	--Total Sublet Sales Revenue - Chargebale to Customer CPUS (External)
	set @A_17 = isnull((select sum(isnull(LbrDppAmt, 0)) from @t_inv_tsk where IsSubCon = 1), 0) + isnull((select sum(isnull(SprAmt, 0)) from @t_inv_itm where IsSublet = 1), 0)
	--Total Service Sales - Chargeable to customer CPUS (External) (2 + 6 + 11 + 14 + 17)
	set @A_19 = @A_02 + @A_06 + @A_11 + @A_17
	--Total Service Sales - Non-chargeable (Warranty) (3 +  7 + 12)
	set @A_20 = @A_03 + @A_07 + @A_12
	-- Total Service Sales - Non-chargeable (Internal, PDI) (4 + 8 + 13)
	set @A_21 = @A_04 + @A_08 + @A_13
	--Total Service Sales Revenue (17 + 18 + 19)
	set @A_18 = @A_19 + @A_20 + @A_21	
	 --Hours Sold
	set @A_22 = (select sum(HourSold) from @t_inv_tsk where IsSubCon = 0)
	--Total Available Hours
	set @A_23 = isnull((select sum(a.WorkingHours)
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
	--Hours Sold / Available Hours  
	set @A_24 = (case isnull(@A_23, 0) when 0 then 0 else 1.0 * isnull(@A_22, 0)/ @A_23 end) * 100.0
	--Service Revenue Targer Per Unit (Based on RKA) 
	set @A_25 = isnull((
						select ServiceAmount
						  from svMstTarget
						 where CompanyCode=@CompanyCode
						   and BranchCode=@BranchCode
						   and ProductType=@ProductType
						   and PeriodYear=@PeriodYear
						   and PeriodMonth=@Month0
						 ),0)
						 
	-- B. No of Unit Intake	
	set @B_01 = isnull((
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
	 
	set @B_02 = (select count(*) from @t_inv_unit_exc)
	set @B_03 = (select count(*) from @t_inv_unit_exc where GroupType = 'P')
	set @B_04 = (select count(*) from @t_inv_unit_exc where GroupType = 'C')
	set @B_05 = (select count(*) from @t_inv_unit_pdi)
					
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
		 
   
    --10,000*; 30,000*; 50,000*; 70,000; & 90,000 km
	set @C_03 = (select count(*) from @t_unit_rtn where Odometer in (10000,30000,50000,70000,90000))
	--20,000*; 60,000; & 100,000 km
	set @C_04 = (select count(*) from @t_unit_rtn where Odometer in (20000,60000,100000))
	--40,000; & 80,000 km 
	set @C_05 = (select count(*) from @t_unit_rtn where Odometer in (40000,80000))
	--Periodical Maintenance under Warranty Period (= 100,000 km/= 3 Years) (sum of 38 to 40)
	set @C_02 = @C_03 + @C_04 + @C_05
	--+10,000; +30,000; +50,000; +70,000; & +90,000 km
	set @C_07 = (select count(*) from @t_unit_rtn where Odometer in (110000,130000,150000,170000,190000,210000,230000,250000,270000,290000))
	--+20,000; +60,000; & +100,000 km
	set @C_08 = (select count(*) from @t_unit_rtn where Odometer in (120000,160000,200000,220000,260000,300000))
	--+40,000; & +80,000 km
	set @C_09 = (select count(*) from @t_unit_rtn where Odometer in (140000,180000,240000,280000))
	--Periodical Maintenance out of Warranty Period (>100,000 km/>3 years) (sum of 42 to 44)
	set @C_06 = @C_07 + @C_08 + @C_09	
	--5,000 km multiplier & above
	set @C_10 = (select count(*) from @t_unit_rtn where (Odometer % 10000) = 5000)	
	-- FSC Job
	declare @t_unit_fsc_clm as table
	(
		InvoiceDate  varchar(20),
		ChassisCode  varchar(50), 
		ChassisNo    varchar(50),
		GroupJobType varchar(50),
		JobType      varchar(50),
		Odometer     numeric(10),
		InvoiceNo	 varchar(50)
	)	
	insert into @t_unit_fsc_clm
	select distinct convert(varchar, a.InvoiceDate, 112) InvoiceDate
		 , a.ChassisCode, convert(varchar, a.ChassisNo) ChassisNo
		 , b.GroupJobType, a.JobType
		 , b.WarrantyOdometer as Odometer
		 , a.InvoiceNo
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

	-- FS 1K (1,000 km)
	set @C_14 = (select count(*) from @t_unit_fsc_clm where GroupJobType = 'FSC' and Odometer = 1000 and InvoiceNo like 'INF%')
	-- FS 5K (5,000 km)
	set @C_15 = (select count(*) from @t_unit_fsc_clm where GroupJobType = 'FSC' and Odometer = 5000 and InvoiceNo like 'INF%')
	-- FS 10K (10,000 km)
	set @C_16 = (select count(*) from @t_unit_fsc_clm where GroupJobType = 'FSC' and Odometer = 10000 and InvoiceNo like 'INF%')
	--FS 20K (20,000 km)
	set @C_17 = (select count(*) from @t_unit_fsc_clm where GroupJobType = 'FSC' and Odometer = 20000 and InvoiceNo like 'INF%')
	--FS 30K (30,000 km)
	set @C_18 = (select count(*) from @t_unit_fsc_clm where GroupJobType = 'FSC' and Odometer = 30000 and InvoiceNo like 'INF%')	
	-- FS 40K (40,000 km)
	set @C_19 = (select count(*) from @t_unit_fsc_clm where GroupJobType = 'FSC' and Odometer = 40000 and InvoiceNo like 'INF%')
	--FS 50K (50,000 km)
	set @C_20 = (select count(*) from @t_unit_fsc_clm where GroupJobType = 'FSC' and Odometer = 50000 and InvoiceNo like 'INF%')
	--No. of Free Service (FS 1K, FS 5K, FS 10K, FS 20K, FS 30K, FS 40K, & FS 50K) (sum of 49 to 55)
	set @C_13 = @C_14 + @C_15 + @C_16 + @C_17 + @C_18 + @C_19 + @C_20
	--No. of Warranty Repair
	set @C_22 = (select count(*) from @t_unit_fsc_clm where GroupJobType = 'CLM' and InvoiceNo like 'INW%')	
	--No. of PDI intake
	set @C_24 = (select count(*) from @t_inv_tsk where JobType = 'PDI')

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
	
	--Repeat Job (Rework)
	set @C_23 = (select count(*) from @t_unit_oth where JobType = 'REWORK')

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

	--Non-chargeable Repair (Warranty, Repeated Job, PDI) (Sum of 57 to 59)
	set @C_21 = @C_22 + @C_23 + @C_24
	--G/R Non Periodical Maintenance and Others exclude Accessories Installment
	set @C_11 = (select count(*) from @t_job_all where GroupJobType = 'OTHERS')
	--Accessories Installment (SGA)
	set @C_12=isnull((select x.sgaInst from (select 
				inv.CompanyCode,
				inv.BranchCode,
				year(inv.InvoiceDate) PeriodYear,
				MONTH(inv.InvoiceDate) PeriodMonth,
				count(invi.InvoiceNo) sgaInst
			from svTrnInvItem invi	
			inner join [svTrnInvoice] inv on inv.CompanyCode=invi.CompanyCode 
				and inv.BranchCode=invi.BranchCode 
				and inv.InvoiceNo=invi.InvoiceNo
			where invi.TypeOfGoods = '2'
			and invi.companycode=@CompanyCode
			and invi.branchCode=@BranchCode
			and year(inv.InvoiceDate)=@PeriodYear
			and MONTH(inv.InvoiceDate)=@Month0
			group by inv.CompanyCode,inv.BranchCode,year(inv.InvoiceDate),MONTH(inv.InvoiceDate)
			)x),0)

	--Total Customer Paid Unit Service exclude Free Sevice (37 + 41 + 45 + 46 + 47)
	set @C_01 = @C_02 + @C_06 + @C_10 + @C_11

	-- D. Workshop Service Strength
	--No. of Available Stalls (59 + 60)
	set @D_01 = isnull((
						select TotalStall
						  from svMstTarget
						 where CompanyCode=@CompanyCode
						   and BranchCode=@BranchCode
						   and ProductType=@ProductType
						   and PeriodYear=@PeriodYear
						   and PeriodMonth=@Month0
						 ),0)	
	-- No. of Regular Stalls ( Available stalls Exclude Washing Stall and EM Stalls)
	--set @D_02= isnull((select x.Stall from (select 
	--			companycode
	--			,branchCode
	--			 ,@PeriodYear periodyear
	--			 ,@Month0 periodMonh
	--			 , count(stallCode) as Stall
	--		from svMstStall 
	--		where IsActive=1 
	--		and CompanyCode=@CompanyCode
	--		and branchCode=@BranchCode
	--		and StallCode not in ('REGULER')
	--		group by companycode,BranchCode) x),0)
	set @D_02=0
	-- No. of Express Maintenance Stalls (Installed)
	--set @D_03= isnull((select x.Stall from (select 
	--			companycode
	--			,branchCode
	--			 ,@PeriodYear periodyear
	--			 ,@Month0 periodMonh
	--			 , count(stallCode) as Stall
	--		from svMstStall 
	--		where IsActive=1 
	--		and CompanyCode=@CompanyCode
	--		and branchCode=@BranchCode
	--		and StallCode ='REGULER'
	--		group by companycode,BranchCode) x),0)	
	set @D_03=0
	--No. of Admin & Support Staff
	set @D_05 = isnull((
					select count(*) from gnMstEmployee
					 where CompanyCode = @CompanyCode
					   and BranchCode  = @BranchCode
					   and TitleCode  in ('6', '11', '22')
					   and PersonnelStatus = '1'
					), 0)
	--No. of Service Relation Officer (SRO)
	set @D_06 = isnull((
					select count(*) from gnMstEmployee
					 where CompanyCode = @CompanyCode
					   and BranchCode  = @BranchCode
					   and TitleCode  in ('4')
					   and PersonnelStatus = '1'
					), 0)	
	--No. of Service Advisors (SA)
	set @D_07 = isnull((
					select count(*) from gnMstEmployee
					 where CompanyCode = @CompanyCode
					   and BranchCode  = @BranchCode
					   and TitleCode  in ('3')
					   and PersonnelStatus = '1'
					), 0)
	--No. of Foreman
	set @D_09 = isnull((
					select count(*) from gnMstEmployee
					 where CompanyCode = @CompanyCode
					   and BranchCode  = @BranchCode
					   and TitleCode  in ('8')
					   and PersonnelStatus = '1'
					), 0)
	--No. of Technician
	set @D_10 = isnull((
					select count(*) from gnMstEmployee
					 where CompanyCode = @CompanyCode
					   and BranchCode  = @BranchCode
					   and TitleCode  in ('9')
					   and PersonnelStatus = '1'
					), 0)
	-- No. of Productive Staff (68 + 69)
	set @D_08 = @D_09 + @D_10
	--No. of PDI Staff
	set @D_11 = 0
	--No. of Part Staff
	set @D_12 = isnull((
					select count(*) from gnMstEmployee
					 where CompanyCode = @CompanyCode
					   and BranchCode  = @BranchCode
					   and TitleCode  in ('7', '10')
					   and PersonnelStatus = '1'
					), 0)
	--Total No. of Staff (64 + 65 + 66 + 67 + 70 + 71)
	set @D_04 = @D_05 + @D_06 + @D_07 + @D_08 + @D_11 + @D_12

	-- E. Productivity Indicators
	--Repair Unit per Productive staff (32 / 67)
	set @E_01 = (case isnull(@D_08, 0) when 0 then 0 else 1.0 * isnull(@B_02, 0) / @D_08 end)
	--Repair Unit per Technician (32 / 69)
	set @E_02 = (case isnull(@D_10, 0) when 0 then 0 else 1.0 * isnull(@B_02, 0) / @D_10 end)
	--Repair Unit per Available Stall (32 / 60)
	set @E_03 = (case isnull(@D_01, 0) when 0 then 0 else 1.0 * isnull(@B_02, 0) / @D_01 end)
	--No. of Customer / Service Advisor (29 / 60)
	set @E_04 = (case isnull(@D_07, 0) when 0 then 0 else 1.0 * isnull(@B_02, 0) / @D_07 end)
	--No. of Workdays
	set @E_05 = isnull((
						select TotalWorkingDays
						  from svMstTarget
						 where CompanyCode=@CompanyCode
						   and BranchCode=@BranchCode
						   and ProductType=@ProductType
						   and PeriodYear=@PeriodYear
						   and PeriodMonth=@Month0
						 ),0)						
	--Repair Unit per Productive staff / day (72 / 76)				
	set @E_06 = (case isnull(@E_05, 0) when 0 then 0 else 1.0 * isnull(@E_01, 0) / @E_05 end)
	--Repair Unit per Technician / day (73 / 76)
	set @E_07 = (case isnull(@E_05, 0) when 0 then 0 else 1.0 * isnull(@E_02, 0) / @E_05 end)
	--Repair Unit per Available Stall/day (74 / 76)
	set @E_08 = (case isnull(@E_05, 0) when 0 then 0 else 1.0 * isnull(@E_03, 0) / @E_05 end)
	--No. of Customer / Service Advisor / day (75 / 76)
	set @E_09 = (case isnull(@E_05, 0) when 0 then 0 else 1.0 * isnull(@E_04, 0) / @E_05 end)
	-- F. Service Retention & Marketing Activity
	--No. of Service Reminder Target
	set @F_01 = isnull((
						select SMRTarget
						  from svMstTarget
						 where CompanyCode=@CompanyCode
						   and BranchCode=@BranchCode
						   and ProductType=@ProductType
						   and PeriodYear=@PeriodYear
						   and PeriodMonth=@Month0
						 ),0)
	
	--Service Reminder
	set @F_02 =	isnull((
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
	
	--No. of work orders/vehicle intake from Reminder
	set @F_03 =	isnull((
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
	
	--No. of post service follow-ups
	set @F_04 =	isnull((
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

	--Service Bookings
	set @F_05 =	isnull((
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
	
	--% Service Reminder unit intake to Reminder Target (83 / 81) 
	set @F_06 = (case isnull(@F_01, 0) when 0 then 0 else 100.0 * isnull(@F_03, 0) / @F_01 end)

	--% Post Service Follow-ups to repair unit (work order) (84 / 32)
	set @F_07 = (case isnull(@B_02, 0) when 0 then 0 else 100.0 * isnull(@F_04, 0) / @B_02 end)
	
	--% Service Bookings to repair unit (work order) (85 / 32)
	set @F_08 = (case isnull(@B_02, 0) when 0 then 0 else 100.0 * isnull(@F_05, 0) / @B_02 end)

	-- G. CSI Performance
	--CSI Score
	set @G_01 = null
	--Total instant feedback form received
	set @G_02 = null
	--Total instant feedback form received ratio (to work order) (90 / 32)
	set @G_03 = null
	--Total Complaints Received  (93 + 94)
	set @G_04 = null
	-- Direct/verbal complaints received
	set @G_05 = null
	--Indirect/non-verbal complaints received
	set @G_06 = null
	--Total No. of post service follow up 'Satisfied'
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
	--Total No. of post service follow up 'Not-Satisfied'
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
	--Total No.'Not-Satisfied' Closed 
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
	--Ratio of post service follow ups 'Satisfied' per vehicle intake (95 / 32)
	set @G_10 = (case isnull(@B_02, 0) when 0 then 0 else 1.0 * isnull(@G_07, 0)/ @B_02 end)
	--Ratio of post service follow ups 'Not Satisfied' per vehicle intake (96 / 32)
	set @G_11 = (case isnull(@B_02, 0) when 0 then 0 else 1.0 * isnull(@G_08, 0)/ @B_02 end)
	--Ratio of total complaints & post service follow ups 'Not Satisfied' per vehicle intake ((92 + 96) / 32)
	set @G_12 = (case isnull(@B_02, 0) when 0 then 0 else (isnull(@G_04, 0) + isnull(@G_08, 0))/ @B_02 end)

	-- Additional Calculation
	--Service Revenue per Unit exclude PDI (CPUS, Warranty, FS) ((19 + 20) / 32)
	set @A_26 = (case isnull(@B_02, 0) when 0 then 0 else 1.0 * (isnull(@A_19, 0) + isnull(@A_22, 0))/ @B_02 end)
	--Labour  Sales Turnover / Productive staff (Technician & Foreman)  (1 / 67) 
	set @A_27 = (case isnull(@D_08, 0) when 0 then 0 else @A_01 / @D_08 end)
	 --Labour Sales Turnover / Service Advisors (1 / 66) 
	set @A_28 = (case isnull(@D_07, 0) when 0 then 0 else @A_01 / @D_07 end)
	 --Labour Sales Turnover / Stall (1 / 58)
	set @A_29 = (case isnull(@D_01, 0) when 0 then 0 else @A_01 / @D_01 end)
	
	--A	
	insert into @t_msidat select 'A', 'MSI_001', @Month0, @A_01
	insert into @t_msidat select 'A', 'MSI_002', @Month0, @A_02
	insert into @t_msidat select 'A', 'MSI_003', @Month0, @A_03
	insert into @t_msidat select 'A', 'MSI_004', @Month0, @A_04
	insert into @t_msidat select 'A', 'MSI_005', @Month0, @A_05
	insert into @t_msidat select 'A', 'MSI_006', @Month0, @A_06
	insert into @t_msidat select 'A', 'MSI_007', @Month0, @A_07
	insert into @t_msidat select 'A', 'MSI_008', @Month0, @A_08 
	insert into @t_msidat select 'A', 'MSI_009', @Month0, @A_09
	insert into @t_msidat select 'A', 'MSI_010', @Month0, @A_10
	insert into @t_msidat select 'A', 'MSI_011', @Month0, @A_11
	insert into @t_msidat select 'A', 'MSI_012', @Month0, @A_12
	insert into @t_msidat select 'A', 'MSI_013', @Month0, @A_13

	insert into @t_msidat select 'A', 'MSI_014', @Month0, @A_14
	insert into @t_msidat select 'A', 'MSI_015', @Month0, @A_15
	insert into @t_msidat select 'A', 'MSI_016', @Month0, @A_16

	insert into @t_msidat select 'A', 'MSI_017', @Month0, @A_17
	insert into @t_msidat select 'A', 'MSI_018', @Month0, @A_18
	insert into @t_msidat select 'A', 'MSI_019', @Month0, @A_19
	insert into @t_msidat select 'A', 'MSI_020', @Month0, @A_20
	insert into @t_msidat select 'A', 'MSI_021', @Month0, @A_21
	insert into @t_msidat select 'A', 'MSI_022', @Month0, @A_22
	insert into @t_msidat select 'A', 'MSI_023', @Month0, @A_23
	insert into @t_msidat select 'A', 'MSI_024', @Month0, @A_24
	insert into @t_msidat select 'A', 'MSI_025', @Month0, @A_25
	insert into @t_msidat select 'A', 'MSI_026', @Month0, @A_26
	insert into @t_msidat select 'A', 'MSI_027', @Month0, @A_27
	insert into @t_msidat select 'A', 'MSI_028', @Month0, @A_28
	insert into @t_msidat select 'A', 'MSI_029', @Month0, @A_29
	insert into @t_msidat select 'A', 'MSI_030', @Month0, @A_30
	
	--B
	insert into @t_msidat select 'B', 'MSI_031', @Month0, @B_01
	insert into @t_msidat select 'B', 'MSI_032', @Month0, @B_02
	insert into @t_msidat select 'B', 'MSI_033', @Month0, @B_03
	insert into @t_msidat select 'B', 'MSI_034', @Month0, @B_04
	insert into @t_msidat select 'B', 'MSI_035', @Month0, @B_05

	--C
	insert into @t_msidat select 'C', 'MSI_036', @Month0, @C_01
	insert into @t_msidat select 'C', 'MSI_037', @Month0, @C_02
	insert into @t_msidat select 'C', 'MSI_038', @Month0, @C_03
	insert into @t_msidat select 'C', 'MSI_039', @Month0, @C_04
	insert into @t_msidat select 'C', 'MSI_040', @Month0, @C_05
	insert into @t_msidat select 'C', 'MSI_041', @Month0, @C_06
	insert into @t_msidat select 'C', 'MSI_042', @Month0, @C_07
	insert into @t_msidat select 'C', 'MSI_043', @Month0, @C_08
	insert into @t_msidat select 'C', 'MSI_044', @Month0, @C_09
	insert into @t_msidat select 'C', 'MSI_045', @Month0, @C_10
	insert into @t_msidat select 'C', 'MSI_046', @Month0, @C_11
	insert into @t_msidat select 'C', 'MSI_047', @Month0, @C_12
	insert into @t_msidat select 'C', 'MSI_048', @Month0, @C_13
	insert into @t_msidat select 'C', 'MSI_049', @Month0, @C_14
	insert into @t_msidat select 'C', 'MSI_050', @Month0, @C_15
	insert into @t_msidat select 'C', 'MSI_051', @Month0, @C_16
	insert into @t_msidat select 'C', 'MSI_052', @Month0, @C_17
	insert into @t_msidat select 'C', 'MSI_053', @Month0, @C_18
	insert into @t_msidat select 'C', 'MSI_054', @Month0, @C_19
	insert into @t_msidat select 'C', 'MSI_055', @Month0, @C_20
	insert into @t_msidat select 'C', 'MSI_056', @Month0, @C_21
	insert into @t_msidat select 'C', 'MSI_057', @Month0, @C_22
	insert into @t_msidat select 'C', 'MSI_058', @Month0, @C_23
	insert into @t_msidat select 'C', 'MSI_059', @Month0, @C_24
	
	--D
	insert into @t_msidat select 'D', 'MSI_060', @Month0, @D_01
	insert into @t_msidat select 'D', 'MSI_061', @Month0, @D_02
	insert into @t_msidat select 'D', 'MSI_062', @Month0, @D_03
	insert into @t_msidat select 'D', 'MSI_063', @Month0, @D_04
	insert into @t_msidat select 'D', 'MSI_064', @Month0, @D_05
	insert into @t_msidat select 'D', 'MSI_065', @Month0, @D_06
	insert into @t_msidat select 'D', 'MSI_066', @Month0, @D_07
	insert into @t_msidat select 'D', 'MSI_067', @Month0, @D_08
	insert into @t_msidat select 'D', 'MSI_068', @Month0, @D_09
	insert into @t_msidat select 'D', 'MSI_069', @Month0, @D_10
	insert into @t_msidat select 'D', 'MSI_070', @Month0, @D_11
	insert into @t_msidat select 'D', 'MSI_071', @Month0, @D_12
	
	--E
	insert into @t_msidat select 'E', 'MSI_072', @Month0, @E_01
	insert into @t_msidat select 'E', 'MSI_073', @Month0, @E_02
	insert into @t_msidat select 'E', 'MSI_074', @Month0, @E_03
	insert into @t_msidat select 'E', 'MSI_075', @Month0, @E_04
	insert into @t_msidat select 'E', 'MSI_076', @Month0, @E_05
	insert into @t_msidat select 'E', 'MSI_077', @Month0, @E_06
	insert into @t_msidat select 'E', 'MSI_078', @Month0, @E_07
	insert into @t_msidat select 'E', 'MSI_079', @Month0, @E_08
	insert into @t_msidat select 'E', 'MSI_080', @Month0, @E_09

	--F
	insert into @t_msidat select 'F', 'MSI_081', @Month0, @F_01
	insert into @t_msidat select 'F', 'MSI_082', @Month0, @F_02
	insert into @t_msidat select 'F', 'MSI_083', @Month0, @F_03
	insert into @t_msidat select 'F', 'MSI_084', @Month0, @F_04
	insert into @t_msidat select 'F', 'MSI_085', @Month0, @F_05
	insert into @t_msidat select 'F', 'MSI_086', @Month0, @F_06
	insert into @t_msidat select 'F', 'MSI_087', @Month0, @F_07
	insert into @t_msidat select 'F', 'MSI_088', @Month0, @F_08
	--G
	insert into @t_msidat select 'G', 'MSI_089', @Month0, ISNULL(@G_01,0)
	insert into @t_msidat select 'G', 'MSI_090', @Month0, ISNULL(@G_02,0)
	insert into @t_msidat select 'G', 'MSI_091', @Month0, ISNULL(@G_03,0)
	insert into @t_msidat select 'G', 'MSI_092', @Month0, ISNULL(@G_04,0)
	insert into @t_msidat select 'G', 'MSI_093', @Month0, ISNULL(@G_05,0)
	insert into @t_msidat select 'G', 'MSI_094', @Month0, ISNULL(@G_06,0)
	insert into @t_msidat select 'G', 'MSI_095', @Month0, @G_07
	insert into @t_msidat select 'G', 'MSI_096', @Month0, @G_08
	insert into @t_msidat select 'G', 'MSI_097', @Month0, @G_09
	insert into @t_msidat select 'G', 'MSI_098', @Month0, @G_10
	insert into @t_msidat select 'G', 'MSI_099', @Month0, @G_11
	insert into @t_msidat select 'G', 'MSI_100', @Month0, @G_12
	
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

--delete SvHstSzkMSI
delete svHstSzkMSI_Invoice
 where CompanyCode = @CompanyCode
   and BranchCode = @BranchCode
   and PeriodYear = @PeriodYear
   and PeriodMonth in (select distinct MsiMonth from @t_msidat)

--insert into SvHstSzkMSI (CompanyCode, BranchCode, PeriodYear, PeriodMonth, SeqNo, MsiGroup, MsiDesc, Unit, MsiData, CreatedBy, CreatedDate)
insert into svHstSzkMSI_Invoice (CompanyCode, BranchCode, PeriodYear, PeriodMonth, SeqNo, MsiGroup, MsiDesc, Unit, MsiData, CreatedBy, CreatedDate)
select @CompanyCode CompanyCode, @BranchCode BranchCode
	 , @PeriodYear PeriodYear, a.MsiMonth PeriodMonth
	 , convert(int, right(MsiCode, 3)) as SeqNo
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
	 , isnull( [1], 0)  [Month01]
	 , isnull( [2], 0)  [Month02]
	 , isnull( [3], 0)  [Month03]
	 , isnull( [4], 0)  [Month04]
	 , isnull( [5], 0)  [Month05]
	 , isnull( [6], 0)  [Month06]
	 , isnull( [7], 0)  [Month07]
	 , isnull( [8], 0)  [Month08]
	 , isnull( [9], 0)  [Month09]
	 , isnull([10], 0)  [Month10]
	 , isnull([11], 0)  [Month11]
	 , isnull([12], 0)  [Month12]
  from (
	select CompanyCode, BranchCode, PeriodYear, PeriodMonth, SeqNo, MsiGroup, MsiDesc, Unit, MsiData from svHstSzkMSI_Invoice
	 where CompanyCode = @CompanyCode
	   and BranchCode = @BranchCode
	   and PeriodYear = @PeriodYear
	   and PeriodMonth >= @Month1
	   and PeriodMonth <= @Month2
  )#
 pivot (sum(MsiData) for PeriodMonth in ([1], [2], [3], [4], [5], [6], [7], [8], [9], [10], [11], [12])) as pvt
 order by pvt.MsiGroup, pvt.SeqNo

