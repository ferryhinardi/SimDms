USE [SimDms]
GO

/****** Object:  StoredProcedure [dbo].[uspfn_SvDtlRevenueRegSpkXls]    Script Date: 10/6/2015 2:27:51 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		fhi
-- Create date: 25092015 modified 06102015 by fhi
-- Description:	Detail Revenue Register SPK for Excell
-- =============================================
ALTER PROCEDURE [dbo].[uspfn_SvDtlRevenueRegSpkXls] 
	(
		 @Area     varchar(50),
		 @Dealer   varchar(20),
		 @Outlet   varchar(20),
		 @DateFrom datetime,
		 @DateTo   datetime,
		 @Revenue  varchar(100),
		 @Pdi	   varchar(15)
	)

	 --@Area     varchar(50)='cabang',
	 --@Dealer   varchar(20)='6006400001',
	 --@Outlet   varchar(20)='6006400101',
	 --@DateFrom datetime='20150105 00:00:00',
  --   @DateTo   datetime='20150108 23:59:59',
	 --@Revenue  varchar(100)='TotalServiceSlsRevenue',
	 --@Pdi	   varchar(15)='include'
AS
BEGIN
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
declare @A_18 numeric(18, 4) --Total Service Sales Revenue (19 + 20 + 21)
declare @A_19 numeric(18, 4) --     Total Service Sales - Chargeable to customer CPUS (External) (2 + 6 + 11 + 14 + 17)
declare @A_20 numeric(18, 4) --     Total Service Sales - Non-chargeable (Warranty) (3 +  7 + 12)
declare @A_21 numeric(18, 4) --     Total Service Sales - Non-chargeable (Internal, PDI) (4 + 8 + 13)

declare
 @jType varchar(25)

-- declare @t_tempSGAAccPartSls as table
--(
--	CompanyCode  varchar(25),
--	BranchCode varchar(25),
--	SupplySlipNo varchar(25),
--	DPPAmt numeric(18,2)
--)

--insert into @t_tempSGAAccPartSls
--	select
--		srv.CompanyCode
--		,srv.BranchCode
--		,srvi.SupplySlipNo
--		,sum((SupplyQty * RetailPrice) - (RetailPrice*(discPct/100))) as DPPAmt 
--		from svTrnsrvItem srvi
--		inner join svtrnservice srv on srv.CompanyCode=srvi.CompanyCode and srv.BranchCode=srvi.BranchCode and srv.serviceNo=srvi.ServiceNo
--		inner join gnMstDealerMapping d on d.DealerCode = srvi.CompanyCode
--		where 1=1
--		and d.Area = (case @Area when '' then d.Area else @Area end)
--		and srvi.companycode=(case @Dealer when '' then srvi.CompanyCode else @Dealer end)
--		and srvi.branchCode=(case @Outlet when '' then srvi.BranchCode else @Outlet end) 
--		and srvi.TypeOfGoods = '2'
--		and srv.JobOrderClosed between @DateFrom and @DateTo
--		group by srv.CompanyCode,srv.BranchCode,srv.JobOrderClosed,srvi.PartNo,srvi.SupplySlipNo

----select * from @t_tempSGAAccPartSls

--declare @t_tempMaterialSls as table
--(
--	CompanyCode  varchar(25),
--	BranchCode varchar(25),
--	SupplySlipNo varchar(25),
--	DPPAmt numeric(18,2)
--)

--insert into @t_tempMaterialSls
--	select 
--		srv.CompanyCode
--		,srv.BranchCode
--		,srvi.SupplySlipNo
--		,sum((SupplyQty * RetailPrice) - (RetailPrice*(discPct/100))) as DPPAmt 
--	from svTrnsrvItem srvi	
--	inner join svtrnservice srv on srv.CompanyCode=srvi.CompanyCode and srv.BranchCode=srvi.BranchCode and srv.serviceNo=srvi.ServiceNo
--	left join gnMstDealerMapping d on d.DealerCode = srvi.CompanyCode
--	where srvi.TypeOfGoods in ('5','6')
--	and d.Area = (case @Area when '' then d.Area else @Area end)
--	and srvi.companycode=(case @Dealer when '' then srvi.CompanyCode else @Dealer end)
--	and srvi.branchCode=(case @Outlet when '' then srvi.BranchCode else @Outlet end) 
--	and srv.JobOrderClosed between @DateFrom and @DateTo
--	group by srv.CompanyCode,srv.BranchCode,srvi.SupplySlipNo,srv.JobOrderClosed

----select * from @t_tempMaterialSls

declare @t_inv_tsk as table
	(
		CompanyCode varchar(25),
		BranchCode varchar(25),
		InvoiceNo varchar(20),
		JobOrderNo varchar(20),
		HourSold  numeric(18,2),  
		LbrDppAmt numeric(18,0),
		IsSubCon  bit,
		JobType   varchar(20),
		KsgType   varchar(20),
		OperationNo   varchar(50)  
	)	
insert into @t_inv_tsk
	select 
		a.companyCode
		, a.branchcode
		, a.InvoiceNo
		,joborderno
		, b.OperationHour
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
		   , b.OperationNo
	  from svTrnService a
	  --inner join svTrnInvTask b on b.CompanyCode = a.CompanyCode and b.BranchCode = a.BranchCode and b.InvoiceNo = a.InvoiceNo
	  inner join svTrnSrvTask b on b.CompanyCode = a.CompanyCode and b.BranchCode = a.BranchCode and b.serviceNo = a.ServiceNo
	  left join svMstJob c on c.CompanyCode = a.CompanyCode and c.BasicModel = a.BasicModel and c.JobType = a.JobType
	  left join gnMstDealerMapping d on d.DealerCode = a.CompanyCode

	 where 1=1		
		and a.CompanyCode = (case @Dealer when '' then a.CompanyCode else @Dealer end)
		and a.BranchCode = (case @Outlet when '' then a.BranchCode else @Outlet end)  
		and d.Area = (case @Area when '' then d.Area else @Area end)
		and JobOrderDate between @DateFrom and @DateTo
		and a.JobType != 'REWORK'

--select * from @t_inv_tsk

--@t_inv_itm
	declare @t_inv_itm as table
	(
		CompanyCode varchar(25),
		BranchCode varchar(25),
		InvoiceNo  varchar(20),
		JobOrderNo  varchar(20),
		PartNo  varchar(20),
		SupplySlipNo  varchar(20),
		GroupTpgo  varchar(50), 
		IsSublet   varchar(10),
		GroupJob   varchar(50),
		SprAmt     numeric(18,0),
		typeOfGood int
	)	
	insert into @t_inv_itm	
	select 
		a.companyCode
		, a.branchcode
		, a.InvoiceNo
		,joborderno
		,b.PartNo
		,b.SupplySlipNo
		, c.ParaValue as GroupTpgo
		, case when TypeofGoods in ('0', '1') then '0' else '1' end IsSublet
		, case 
			 when left(a.InvoiceNo, 3) = 'INI' or a.JobType = 'PDI' then 'INT,PDI'
			 when left(a.InvoiceNo, 3) = 'INW' or left(a.InvoiceNo, 3) = 'INF' then 'CLM,FSC'
			 else 'CPUS'
		   end GroupJob
		 , (ceiling((b.SupplyQty - b.ReturnQty) * b.RetailPrice * (100.0 - b.DiscPct) * 0.01)) SprAmt
		 , b.TypeOfGoods
	  from svTrnService a
	  --inner join svTrnInvItem b on b.CompanyCode = a.CompanyCode and b.BranchCode = a.BranchCode and b.InvoiceNo = a.InvoiceNo
	  inner join svTrnsrvItem b on b.CompanyCode = a.CompanyCode and b.BranchCode = a.BranchCode and b.serviceno = a.serviceno
	  left join gnMstLookupDtl c on c.CompanyCode = '0000000' and c.CodeID = 'GTGO' and c.LookupValue = b.TypeOfGoods
	  left join gnMstDealerMapping d on d.DealerCode = a.CompanyCode
	 where 1=1	  
	  and a.CompanyCode = (case @Dealer when '' then a.CompanyCode else @Dealer end)
	  and a.BranchCode = (case @Outlet when '' then a.BranchCode else @Outlet end) 
	  and d.Area = (case @Area when '' then d.Area else @Area end)
	  and a.JobOrderDate between @DateFrom and @DateTo
	  and a.JobType != 'REWORK'

--@sum Counter Parts Sales Revenue
	declare @t_count_part_sls_revenue as table
	(
		CompanyCode varchar(25),
		BranchCode varchar(25),
		InvoiceNo varchar(25),
		TotDppAmt  numeric(18,2)
	)

	insert into @t_count_part_sls_revenue
	 select a.CompanyCode,a.BranchCode,a.InvoiceNo,sum(TotDppAmt)
	 from spTrnSFPJHdr a
	 left join gnMstDealerMapping d on d.DealerCode = a.CompanyCode
	 where 1 = 1	 
	 and a.CompanyCode = (case @Dealer when '' then a.CompanyCode else @Dealer end)
	 and a.BranchCode = (case @Outlet when '' then a.BranchCode else @Outlet end)
	 and d.Area = (case @Area when '' then d.Area else @Area end) 
	 and a.FPJDate between @DateFrom and @DateTo
	 and a.TypeOfGoods = 0
	 group by a.CompanyCode,a.BranchCode,a.InvoiceNo

declare @t_tempRptDataTotalRevenue as table
(
	CompanyCode  varchar(25),
	DealerName varchar(255),
	BranchCode varchar(25),
	BranchName varchar(255),
	InvoiceNo  varchar(25),
	JobOrderNo  varchar(25),
	JobOrderDate datetime,
	BasicModel varchar(25),
	PoliceRegNo varchar(25),
	Odometer  numeric(10,0),
	JobType varchar(15),
	JobTypeDesc varchar(100),
	TaskPartNo varchar(50),
	NamaJasaPart  varchar(100),
	DemandQty numeric(12,2),
	SupplyQty numeric(12,2),
	ReturnQty numeric(12,2),
	SupplySlipNo  varchar(15),
	SSReturnNo varchar(15),
	SaName varchar(100),
	FmName varchar(100),
	ServiceStatusDesc  varchar(25),
	OperationNo varchar(25),
	a_01_TotalLabourSlsRevenue numeric(18,2),
	a_09_TotalCounterPartSlsRevenue numeric(18,2),
	a_15_SGAAccPartsSales numeric(18,2),
	a_16_SGAMaterialsSales numeric(18,2),
	a_14_TotalSGASalesRevenue numeric(18,2),
	a_05_TotalWorkshopPartsSlsRev numeric(18,2),
	a_10_TotalLubricantSlsRev numeric(18,2),
	a_17_TotalSubletSlsRevCPUS numeric(18,2),
	a_18_TotalServiceSlsRev numeric(18,2)
)

if(@Pdi='include')
	begin
		set @jType =''
	end
else
	begin
		set @jType ='PDI'
	end

;with x as (
select a.CompanyCode
     , d.DealerAbbreviation
     , a.BranchCode
     , d.OutletAbbreviation
	 , a.InvoiceNo
     , a.JobOrderNo
     , a.JobOrderDate
     , a.BasicModel
     , a.PoliceRegNo
     , a.Odometer
     , a.JobType
     , a.JobTypeDesc
     , a.TaskPartNo
     , replace((case a.TaskPartType when 'T' then a.OperationName else a.PartName end), '', '') as NamaJasaPart
     , a.DemandQty
     , a.SupplyQty
     , a.ReturnQty
     , a.SupplySlipNo
     , a.SSReturnNo
     , a.SaName
     , a.FmName
     , a.ServiceStatusDesc
	 , a.OperationNo
  from SvRegisterSpk a
  left join DealerInfo b on b.DealerCode = a.CompanyCode
  left join OutletInfo c on c.CompanyCode = a.CompanyCode and c.BranchCode = a.BranchCode
  left join (select gdm.Area,gdm.DealerCode,gdm.DealerAbbreviation,gdom.OutletCode,gdom.OutletAbbreviation  from gnmstDealermapping  gdm
			inner join gnmstDealerOutletMapping gdom on gdom.dealercode=gdm.dealercode 
			and gdom.groupno= gdm.groupNo) d on d.DealerCode = a.CompanyCode and d.outletCode=a.branchcode
 where 1 = 1   
   and a.CompanyCode = (case @Dealer when '' then a.CompanyCode else @Dealer end)
   and a.BranchCode = (case @Outlet when '' then a.BranchCode else @Outlet end)
   and d.Area = (case @Area when '' then d.Area else @Area end)
   and a.JobOrderDate >= @DateFrom
   and convert(date, a.JobOrderDate) <= @DateTo
   and a.JobType <> case when @Pdi='include' then '' else 'PDI' end
)

insert into @t_tempRptDataTotalRevenue
select x.*
,(((ISNULL(tsk4.LbrDppAmt,0)) - ((ISNULL(tsk2.LbrDppAmt ,0))+ISNULL(tsk3.LbrDppAmt,0)))) + (ISNULL(tsk2.LbrDppAmt ,0)) + (isnull(tsk5.LbrDppAmt,0)) as a_01_TotalLabourSlsRevenue
,isnull(TotDppAmt,0) a_09_TotalCounterPartSlsRevenue
--,ISNULL(sgaAcc.DPPAmt,0) a_15_SGAAccPartsSales
--, isnull(mtrSls.DPPAmt,0) a_16_SGAMaterialsSales
--, (ISNULL(sgaAcc.DPPAmt,0)) + (isnull(mtrSls.DPPAmt,0)) a_14_TotalSGASalesRevenue
,ISNULL(itm10.SprAmt,0) a_15_SGAAccPartsSales
, isnull(itm11.SprAmt,0) a_16_SGAMaterialsSales
, (ISNULL(itm10.SprAmt,0)) + (isnull(itm11.SprAmt,0)) a_14_TotalSGASalesRevenue
, isnull(itm1.SprAmt,0) a_05_TotalWorkshopPartsSlsRev
, isnull(itm2.SprAmt,0) a_10_TotalLubricantSlsRev
, (isnull(tsk1.LbrDppAmt,0)) + (ISNULL(itm3.SprAmt,0)) a_17_TotalSubletSlsRevCPUS
, (((ISNULL(tsk4.LbrDppAmt,0)) - ((ISNULL(tsk2.LbrDppAmt ,0))+ISNULL(tsk3.LbrDppAmt,0))+(isnull(itm4.SprAmt,0))+(isnull(itm5.SprAmt,0)) + ((isnull(tsk1.LbrDppAmt,0)) + (ISNULL(itm3.SprAmt,0)))))+
	((ISNULL(tsk2.LbrDppAmt ,0))+(isnull(itm6.SprAmt,0))+(isnull(itm7.SprAmt,0))) + ((isnull(tsk5.LbrDppAmt,0))+(isnull(itm8.SprAmt,0))+(isnull(itm9.SprAmt,0))+(ISNULL(itm10.SprAmt,0)) + (isnull(itm11.SprAmt,0))) as a_18_TotalServiceSlsRev
from x
left join @t_inv_tsk tsk on tsk.CompanyCode=x.CompanyCode 
	and tsk.BranchCode=x.branchCode and tsk.JobOrderNo=x.jobOrderNo
	and tsk.OperationNo=x.OperationNo
left join @t_inv_itm itm on itm.CompanyCode=x.CompanyCode and itm.BranchCode=x.BranchCode
	and itm.jobOrderNo=x.JobOrderNo
	and itm.PartNo=x.TaskPartNo
	and itm.SupplySlipNo=x.SupplySlipNo
left join @t_count_part_sls_revenue rev on rev.CompanyCode=x.CompanyCode and rev.BranchCode=x.BranchCode
	and rev.InvoiceNo=x.InvoiceNo
--left join @t_tempSGAAccPartSls sgaAcc on sgaAcc.CompanyCode=x.CompanyCode and sgaAcc.BranchCode=x.BranchCode
--	and sgaAcc.SupplySlipNo=x.SupplySlipNo
--left join @t_tempMaterialSls mtrSls on mtrSls.CompanyCode=x.CompanyCode and mtrSls.BranchCode=x.BranchCode
--	and mtrSls.SupplySlipNo=x.SupplySlipNo
left join @t_inv_itm itm1 on itm1.CompanyCode=x.CompanyCode and itm1.BranchCode=x.BranchCode
	and itm1.jobOrderNo=x.JobOrderNo
	and itm1.PartNo=x.TaskPartNo
	and itm1.SupplySlipNo=x.SupplySlipNo
	and itm1.IsSublet = 0 
	and itm1.GroupTpgo = 'SPAREPART'
left join @t_inv_itm itm2 on itm2.CompanyCode=x.CompanyCode and itm2.BranchCode=x.BranchCode
	and itm2.jobOrderNo=x.JobOrderNo
	and itm2.PartNo=x.TaskPartNo
	and itm2.SupplySlipNo=x.SupplySlipNo
	and itm2.IsSublet = 0 
	and itm2.GroupTpgo != 'SPAREPART'
left join @t_inv_tsk tsk1 on tsk1.CompanyCode=x.CompanyCode 
	and tsk1.BranchCode=x.branchCode and tsk1.JobOrderNo=x.jobOrderNo
	and tsk1.OperationNo=x.OperationNo
	and tsk1.IsSubCon = 1
left join @t_inv_itm itm3 on itm3.CompanyCode=x.CompanyCode and itm3.BranchCode=x.BranchCode
	and itm3.jobOrderNo=x.JobOrderNo and itm3.PartNo=x.TaskPartNo and itm3.SupplySlipNo=x.SupplySlipNo
	and itm3.IsSublet = 1
	and itm3.typeOfGood not in (2,5,6)
left join @t_inv_tsk tsk2 on tsk2.CompanyCode=x.CompanyCode and tsk2.BranchCode=x.BranchCode 
	and tsk2.OperationNo=x.OperationNo and tsk2.JobOrderNo=x.jobOrderNo and tsk2.IsSubCon = 0 
	and (substring(tsk2.InvoiceNo, 1 ,3) = 'INW' or substring(tsk2.InvoiceNo, 1 ,3) = 'INF' and tsk2.JobType != 'PDI')
left join @t_inv_tsk tsk3 on tsk3.CompanyCode=x.CompanyCode and tsk3.BranchCode=x.BranchCode 
	and tsk3.OperationNo=x.OperationNo and tsk3.JobOrderNo=x.jobOrderNo and tsk3.IsSubCon = 0 
	and (substring(tsk3.InvoiceNo, 1 ,3) = 'INI' or substring(tsk3.InvoiceNo, 1 ,3) = 'INF' and tsk3.JobType = 'PDI')
left join @t_inv_tsk tsk4 on tsk4.CompanyCode=x.CompanyCode 
	and tsk4.BranchCode=x.branchCode and tsk4.JobOrderNo=x.jobOrderNo
	and tsk4.OperationNo=x.OperationNo and tsk4.IsSubCon=0
left join @t_inv_itm itm4 on itm4.CompanyCode=x.CompanyCode and itm4.BranchCode=x.BranchCode 
	and itm4.jobOrderNo=x.JobOrderNo and itm4.PartNo=x.TaskPartNo and itm4.SupplySlipNo=x.SupplySlipNo
	and itm4.IsSublet = 0 and itm4.GroupTpgo = 'SPAREPART' and itm4.GroupJob = 'CPUS'
left join @t_inv_itm itm5 on itm5.CompanyCode=x.CompanyCode and itm5.BranchCode=x.BranchCode 
	and itm5.jobOrderNo=x.JobOrderNo and itm5.PartNo=x.TaskPartNo and itm5.SupplySlipNo=x.SupplySlipNo
	and itm5.IsSublet = 0 and itm5.GroupTpgo != 'SPAREPART' and itm5.GroupJob = 'CPUS'
left join @t_inv_itm itm6 on itm6.CompanyCode=x.CompanyCode and itm6.BranchCode=x.BranchCode
	and itm6.jobOrderNo=x.JobOrderNo and itm6.PartNo=x.TaskPartNo and itm6.SupplySlipNo=x.SupplySlipNo
	and itm6.IsSublet = 0 and itm6.GroupTpgo = 'SPAREPART' and itm6.GroupJob = 'CLM,FSC'
left join @t_inv_itm itm7 on itm7.CompanyCode=x.CompanyCode and itm7.BranchCode=x.BranchCode 
	and itm7.jobOrderNo=x.JobOrderNo and itm7.PartNo=x.TaskPartNo and itm7.SupplySlipNo=x.SupplySlipNo
	and itm7.IsSublet = 0 and itm7.GroupTpgo != 'SPAREPART' and itm7.GroupJob = 'CLM,FSC'
left join @t_inv_tsk tsk5 on tsk5.CompanyCode=x.CompanyCode and tsk5.BranchCode=x.BranchCode  
	and tsk5.BranchCode=x.branchCode and tsk5.JobOrderNo=x.jobOrderNo and tsk5.IsSubCon = 0 
	and (substring(tsk5.InvoiceNo, 1 ,3) = 'INI' or substring(tsk5.InvoiceNo, 1 ,3) = 'INF' and tsk5.JobType = 'PDI')
left join @t_inv_itm itm8 on itm8.CompanyCode=x.CompanyCode and itm8.BranchCode=x.BranchCode
	and itm8.jobOrderNo=x.JobOrderNo and itm8.PartNo=x.TaskPartNo and itm8.SupplySlipNo=x.SupplySlipNo 
	and itm8.IsSublet = 0 and itm8.GroupTpgo = 'SPAREPART' and itm8.GroupJob = 'INT,PDI'
left join @t_inv_itm itm9 on itm9.CompanyCode=x.CompanyCode and itm9.BranchCode=x.BranchCode 
	and itm9.jobOrderNo=x.JobOrderNo and itm9.PartNo=x.TaskPartNo and itm9.SupplySlipNo=x.SupplySlipNo
	and itm9.IsSublet = 0 and itm9.GroupTpgo != 'SPAREPART' and itm9.GroupJob = 'INT,PDI'
left join @t_inv_itm itm10 on itm10.CompanyCode=x.CompanyCode and itm10.BranchCode=x.BranchCode 
	and itm10.jobOrderNo=x.JobOrderNo and itm10.PartNo=x.TaskPartNo and itm10.SupplySlipNo=x.SupplySlipNo
	and itm10.typeOfGood=2
left join @t_inv_itm itm11 on itm11.CompanyCode=x.CompanyCode and itm11.BranchCode=x.BranchCode 
	and itm11.jobOrderNo=x.JobOrderNo and itm10.PartNo=x.TaskPartNo and itm11.SupplySlipNo=x.SupplySlipNo
	and itm11.typeOfGood in (5,6)

if (@Revenue='TotalWorkshopPartRevenue')
	begin
		select 
			CompanyCode [Kode Dealer],
			DealerName [Name Dealer ],
			BranchCode [Kode Outlet],
			BranchName [Nama Outlet  ],
			JobOrderNo [No SPK     ],
			JobOrderDate [Tanggal SPK],
			BasicModel [Model      ],
			PoliceRegNo [No Polisi  ],
			Odometer  [Odometer   ],
			JobType [Kode Pekerjaan],
			JobTypeDesc [Keterangan Pekerjaan],
			TaskPartNo [Kode Jasa / Part],
			NamaJasaPart  [Nama Jasa / Part              ],
			DemandQty [Demand Qty ],
			SupplyQty [Supply Qty ],
			ReturnQty [return Qty ],
			SupplySlipNo  [No Supply Slip],
			SSReturnNo [No SS Return ],
			SaName [Nama SA      ],
			FmName [Nama Foreman ],
			ServiceStatusDesc  [Service Status],
			a_05_TotalWorkshopPartsSlsRev [Total Workshop Parts Sls Revenue]
		from @t_tempRptDataTotalRevenue
		order by [No SPK     ],[Kode Dealer]
	end
else if (@Revenue='TotalCounterPartSlsRevenue')
	begin
		select 
			CompanyCode [Kode Dealer],
			DealerName [Name Dealer ],
			BranchCode [Kode Outlet],
			BranchName [Nama Outlet  ],
			JobOrderNo [No SPK     ],
			JobOrderDate [Tanggal SPK],
			BasicModel [Model      ],
			PoliceRegNo [No Polisi  ],
			Odometer  [Odometer   ],
			JobType [Kode Pekerjaan],
			JobTypeDesc [Keterangan Pekerjaan],
			TaskPartNo [Kode Jasa / Part],
			NamaJasaPart  [Nama Jasa / Part              ],
			DemandQty [Demand Qty ],
			SupplyQty [Supply Qty ],
			ReturnQty [return Qty ],
			SupplySlipNo  [No Supply Slip],
			SSReturnNo [No SS Return ],
			SaName [Nama SA      ],
			FmName [Nama Foreman ],
			ServiceStatusDesc  [Service Status],
			a_09_TotalCounterPartSlsRevenue [Total Counter Part Sls Revenue]
		from @t_tempRptDataTotalRevenue
		order by [No SPK     ],[Kode Dealer]
	end
else if (@Revenue='TotalRubricantSlsRevenue')
	begin
			
		select 
			CompanyCode [Kode Dealer],
			DealerName [Name Dealer ],
			BranchCode [Kode Outlet],
			BranchName [Nama Outlet  ],
			JobOrderNo [No SPK     ],
			JobOrderDate [Tanggal SPK],
			BasicModel [Model      ],
			PoliceRegNo [No Polisi  ],
			Odometer  [Odometer   ],
			JobType [Kode Pekerjaan],
			JobTypeDesc [Keterangan Pekerjaan],
			TaskPartNo [Kode Jasa / Part],
			NamaJasaPart  [Nama Jasa / Part              ],
			DemandQty [Demand Qty ],
			SupplyQty [Supply Qty ],
			ReturnQty [return Qty ],
			SupplySlipNo  [No Supply Slip],
			SSReturnNo [No SS Return ],
			SaName [Nama SA      ],
			FmName [Nama Foreman ],
			ServiceStatusDesc  [Service Status],
			a_10_TotalLubricantSlsRev [Total Lubricant Sls Revenue]
		from @t_tempRptDataTotalRevenue
		order by [No SPK     ],[Kode Dealer]
	end
else if (@Revenue='TotalSGASalesRevenue')
	begin	

		select 
			CompanyCode [Kode Dealer],
			DealerName [Name Dealer ],
			BranchCode [Kode Outlet],
			BranchName [Nama Outlet  ],
			JobOrderNo [No SPK     ],
			JobOrderDate [Tanggal SPK],
			BasicModel [Model      ],
			PoliceRegNo [No Polisi  ],
			Odometer  [Odometer   ],
			JobType [Kode Pekerjaan],
			JobTypeDesc [Keterangan Pekerjaan],
			TaskPartNo [Kode Jasa / Part],
			NamaJasaPart  [Nama Jasa / Part              ],
			DemandQty [Demand Qty ],
			SupplyQty [Supply Qty ],
			ReturnQty [return Qty ],
			SupplySlipNo  [No Supply Slip],
			SSReturnNo [No SS Return ],
			SaName [Nama SA      ],
			FmName [Nama Foreman ],
			ServiceStatusDesc  [Service Status],
			a_14_TotalSGASalesRevenue [Total SGA Sls Revenue Chargeable to Custr CPUS]
		from @t_tempRptDataTotalRevenue
		order by [No SPK     ],[Kode Dealer]
	end
else if (@Revenue='SGAAccessoriesPartSales')
	begin
	
		select 
			CompanyCode [Kode Dealer],
			DealerName [Name Dealer ],
			BranchCode [Kode Outlet],
			BranchName [Nama Outlet  ],
			JobOrderNo [No SPK     ],
			JobOrderDate [Tanggal SPK],
			BasicModel [Model      ],
			PoliceRegNo [No Polisi  ],
			Odometer  [Odometer   ],
			JobType [Kode Pekerjaan],
			JobTypeDesc [Keterangan Pekerjaan],
			TaskPartNo [Kode Jasa / Part],
			NamaJasaPart  [Nama Jasa / Part              ],
			DemandQty [Demand Qty ],
			SupplyQty [Supply Qty ],
			ReturnQty [return Qty ],
			SupplySlipNo  [No Supply Slip],
			SSReturnNo [No SS Return ],
			SaName [Nama SA      ],
			FmName [Nama Foreman ],
			ServiceStatusDesc  [Service Status],
			a_15_SGAAccPartsSales [SGA Accessories Parts Sales]
		from @t_tempRptDataTotalRevenue
		order by [No SPK     ],[Kode Dealer]
	end
else if (@Revenue='SGAMeterialSales')
	begin
	
		select 
			CompanyCode [Kode Dealer],
			DealerName [Name Dealer ],
			BranchCode [Kode Outlet],
			BranchName [Nama Outlet  ],
			JobOrderNo [No SPK     ],
			JobOrderDate [Tanggal SPK],
			BasicModel [Model      ],
			PoliceRegNo [No Polisi  ],
			Odometer  [Odometer   ],
			JobType [Kode Pekerjaan],
			JobTypeDesc [Keterangan Pekerjaan],
			TaskPartNo [Kode Jasa / Part],
			NamaJasaPart  [Nama Jasa / Part              ],
			DemandQty [Demand Qty ],
			SupplyQty [Supply Qty ],
			ReturnQty [return Qty ],
			SupplySlipNo  [No Supply Slip],
			SSReturnNo [No SS Return ],
			SaName [Nama SA      ],
			FmName [Nama Foreman ],
			ServiceStatusDesc  [Service Status],
			a_16_SGAMaterialsSales [SGA Materials Sales]
		from @t_tempRptDataTotalRevenue
		order by [No SPK     ],[Kode Dealer]
	end
else if (@Revenue='TotalSubletSlsRevenue')
	begin
		
		select 
			CompanyCode [Kode Dealer],
			DealerName [Name Dealer ],
			BranchCode [Kode Outlet],
			BranchName [Nama Outlet  ],
			JobOrderNo [No SPK     ],
			JobOrderDate [Tanggal SPK],
			BasicModel [Model      ],
			PoliceRegNo [No Polisi  ],
			Odometer  [Odometer   ],
			JobType [Kode Pekerjaan],
			JobTypeDesc [Keterangan Pekerjaan],
			TaskPartNo [Kode Jasa / Part],
			NamaJasaPart  [Nama Jasa / Part              ],
			DemandQty [Demand Qty ],
			SupplyQty [Supply Qty ],
			ReturnQty [return Qty ],
			SupplySlipNo  [No Supply Slip],
			SSReturnNo [No SS Return ],
			SaName [Nama SA      ],
			FmName [Nama Foreman ],
			ServiceStatusDesc  [Service Status],
			a_17_TotalSubletSlsRevCPUS [Total Sublet Sls Revenue - Chargebale to Custr CPUS ]
		from @t_tempRptDataTotalRevenue
		order by [No SPK     ],[Kode Dealer]
	end
else if (@Revenue='TotalServiceSlsRevenue')
	begin
	
		select 
			CompanyCode [Kode Dealer],
			DealerName [Name Dealer ],
			BranchCode [Kode Outlet],
			BranchName [Nama Outlet  ],
			JobOrderNo [No SPK     ],
			JobOrderDate [Tanggal SPK],
			BasicModel [Model      ],
			PoliceRegNo [No Polisi  ],
			Odometer  [Odometer   ],
			JobType [Kode Pekerjaan],
			JobTypeDesc [Keterangan Pekerjaan],
			TaskPartNo [Kode Jasa / Part],
			NamaJasaPart  [Nama Jasa / Part              ],
			DemandQty [Demand Qty ],
			SupplyQty [Supply Qty ],
			ReturnQty [return Qty ],
			SupplySlipNo  [No Supply Slip],
			SSReturnNo [No SS Return ],
			SaName [Nama SA      ],
			FmName [Nama Foreman ],
			ServiceStatusDesc  [Service Status],
			A_18_TotalServiceSlsRev [Total Service Sls Revenue]
		from @t_tempRptDataTotalRevenue
		order by [No SPK     ],[Kode Dealer]
	end
else
	begin
		select 
			CompanyCode [Kode Dealer],
			DealerName [Name Dealer ],
			BranchCode [Kode Outlet],
			BranchName [Nama Outlet  ],
			JobOrderNo [No SPK     ],
			JobOrderDate [Tanggal SPK],
			BasicModel [Model      ],
			PoliceRegNo [No Polisi  ],
			Odometer  [Odometer   ],
			JobType [Kode Pekerjaan],
			JobTypeDesc [Keterangan Pekerjaan],
			TaskPartNo [Kode Jasa / Part],
			NamaJasaPart  [Nama Jasa / Part              ],
			DemandQty [Demand Qty ],
			SupplyQty [Supply Qty ],
			ReturnQty [return Qty ],
			SupplySlipNo  [No Supply Slip],
			SSReturnNo [No SS Return ],
			SaName [Nama SA      ],
			FmName [Nama Foreman ],
			ServiceStatusDesc  [Service Status],
			a_01_TotalLabourSlsRevenue [Total Labour Sales Revenue],
			a_05_TotalWorkshopPartsSlsRev [Total Workshop Parts Sls Revenue],
			a_09_TotalCounterPartSlsRevenue [Total Counter Part Sls Revenue],
			a_10_TotalLubricantSlsRev [Total Lubricant Sls Revenue],
			a_14_TotalSGASalesRevenue [Total SGA Sls Revenue Chargeable to Custr CPUS],
			a_15_SGAAccPartsSales [SGA Accessories Parts Sales],
			a_16_SGAMaterialsSales [SGA Materials Sales],
			a_17_TotalSubletSlsRevCPUS [Total Sublet Sls Revenue - Chargebale to Custr CPUS ],
			A_18_TotalServiceSlsRev [Total Service Sls Revenue]
		from @t_tempRptDataTotalRevenue
		order by [No SPK     ],[Kode Dealer]
	end


--delete @t_tempSGAAccPartSls
--delete @t_tempMaterialSls
delete @t_inv_tsk
delete @t_inv_itm
delete @t_count_part_sls_revenue
END


GO


