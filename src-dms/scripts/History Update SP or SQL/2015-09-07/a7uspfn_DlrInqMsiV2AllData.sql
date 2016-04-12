if object_id('uspfn_DlrInqMsiV2AllData') is not null
       drop procedure uspfn_DlrInqMsiV2AllData




-- =============================================
-- Author:		fhi.
-- Create date: 07-04-2015 modified 09-04-2015 // modified 04-09-2015
-- Description:	inq suzuki msi v2 all data
-- =============================================
CREATE PROCEDURE [dbo].[uspfn_DlrInqMsiV2AllData]
	@Area varchar(250),
	@CompanyCode varchar(20),
	@BranchCode varchar(20),
	@PeriodYear varchar(20),
	@Month1      int = 1,
	@Month2      int
AS
BEGIN
	DECLARE 
	@CompanyCode1 varchar(20),
	@BranchCode1 varchar(20),
	@intFlag INT,
	@count INT

---- start declare for temporary value
--declare
--@sgaa_1 numeric(18,2),
--@sgaa_2 numeric(18,2),
--@sgaa_3 numeric(18,2),
--@sgaa_4 numeric(18,2),
--@sgaa_5 numeric(18,2),
--@sgaa_6 numeric(18,2),
--@sgaa_7 numeric(18,2),
--@sgaa_8 numeric(18,2),
--@sgaa_9 numeric(18,2),
--@sgaa_10 numeric(18,2),
--@sgaa_11 numeric(18,2),
--@sgaa_12 numeric(18,2),

--@sgam_1 numeric(18,2),
--@sgam_2 numeric(18,2),
--@sgam_3 numeric(18,2),
--@sgam_4 numeric(18,2),
--@sgam_5 numeric(18,2),
--@sgam_6 numeric(18,2),
--@sgam_7 numeric(18,2),
--@sgam_8 numeric(18,2),
--@sgam_9 numeric(18,2),
--@sgam_10 numeric(18,2),
--@sgam_11 numeric(18,2),
--@sgam_12 numeric(18,2),

--@sgai_1 numeric(18,2),
--@sgai_2 numeric(18,2),
--@sgai_3 numeric(18,2),
--@sgai_4 numeric(18,2),
--@sgai_5 numeric(18,2),
--@sgai_6 numeric(18,2),
--@sgai_7 numeric(18,2),
--@sgai_8 numeric(18,2),
--@sgai_9 numeric(18,2),
--@sgai_10 numeric(18,2),
--@sgai_11 numeric(18,2),
--@sgai_12 numeric(18,2),

--@stl1_1 numeric(18,2),
--@stl1_2 numeric(18,2),
--@stl1_3 numeric(18,2),
--@stl1_4 numeric(18,2),
--@stl1_5 numeric(18,2),
--@stl1_6 numeric(18,2),
--@stl1_7 numeric(18,2),
--@stl1_8 numeric(18,2),
--@stl1_9 numeric(18,2),
--@stl1_10 numeric(18,2),
--@stl1_11 numeric(18,2),
--@stl1_12 numeric(18,2),

--@stl2_1 numeric(18,2),
--@stl2_2 numeric(18,2),
--@stl2_3 numeric(18,2),
--@stl2_4 numeric(18,2),
--@stl2_5 numeric(18,2),
--@stl2_6 numeric(18,2),
--@stl2_7 numeric(18,2),
--@stl2_8 numeric(18,2),
--@stl2_9 numeric(18,2),
--@stl2_10 numeric(18,2),
--@stl2_11 numeric(18,2),
--@stl2_12 numeric(18,2)
---- end

-- start create temp table 
	create table #tempTable1(
	CompanyCode varchar(20),
	BranchCode varchar(20),
	PeriodYear numeric(4,0),
	PeriodMonth numeric(2,0),
	SeqNo INT,
	MsiGroup varchar(50),
	MsiDesc varchar(500),
	Unit varchar(50),
	MsiData numeric(18,2),
	lastUpdateDate datetime
)

create table #tempTable2(
	CompanyCode varchar(20),
	BranchCode varchar(20),
	PeriodYear numeric(4,0),
	SeqNo INT,
	MsiGroup varchar(50),
	MsiDesc varchar(500),
	Unit varchar(50),
	Average numeric(18,2),
	Total numeric(18,2),
	Month01 numeric(18,2),
	Month02 numeric(18,2),
	Month03 numeric(18,2),
	Month04 numeric(18,2),
	Month05 numeric(18,2),
	Month06 numeric(18,2),
	Month07 numeric(18,2),
	Month08 numeric(18,2),
	Month09 numeric(18,2),
	Month10 numeric(18,2),
	Month11 numeric(18,2),
	Month12 numeric(18,2)
)

--end create temp table 
select * into #tempTable3
From (select distinct gdm.DealerCode,gdm.DealerName,gdom.OutletCode,gdom.OutletName from GnMstDealerMapping gdm 
																		inner join [gnMstDealerOutletMapping] gdom on gdom.DealerCode=gdm.DealerCode --and gdom.GroupNo=gdm.GroupNo
																		where 
																		gdm.Area like case when @Area = '' then '%%' else @Area end 
																		and gdm.DealerCode like case when @CompanyCode = '' then '%%' else @CompanyCode end)#tempTable3


--set @count= (select count(*) as rowsCount
--				from (select *,
--						row_number() over (order by companyCode) number
--						from (SELECT 
--								distinct CompanyCode,BranchCode
--								FROM svHstSzkMSI where companycode in (select CompanyCode from gnMstOrganizationHdr where CompanyCode in (select DealerCode from GnMstDealerMapping where Area like case when @Area = '' then '%%' else @Area end and DealerCode like case when @CompanyCode = '' then '%%' else @CompanyCode end))) a) b)

set @count= (select count(*) as rowsCount from (select *,row_number() over (order by companyCode) number
				from (SELECT distinct CompanyCode,BranchCode
						FROM svHstSzkMSI 
						where companycode in (select DealerCode from (select * from #tempTable3)x)
						and BranchCode in (select OutletCode from (select * from #tempTable3)x)
						) a 
			) b)


set @intFlag=1;

-- start write all data / branch and insert to #tempTable1
WHILE (@intFlag <=@count)
BEGIN
	--set  @CompanyCode1= (select CompanyCode from (select *,
	--				row_number() over (order by companyCode) number
	--				from 
	--				(SELECT 
	--					distinct CompanyCode,BranchCode
	--				FROM svHstSzkMSI
	--				where companycode in (select CompanyCode from gnMstOrganizationHdr where CompanyCode in (select DealerCode from GnMstDealerMapping where Area like case when @Area = '' then '%%' else @Area end and DealerCode like case when @CompanyCode = '' then '%%' else @CompanyCode end))) a)b
	--				where b.number=@intFlag)
	set  @CompanyCode1= (select CompanyCode from (select *,
													row_number() over (order by companyCode) number
													from (SELECT 
															distinct CompanyCode,BranchCode
															FROM svHstSzkMSI
															where companycode in (select DealerCode from (select * from #tempTable3)x)
															and BranchCode in (select OutletCode from (select * from #tempTable3)x)

															) a
													)b
					where b.number=@intFlag)

	--set  @BranchCode1= (select BranchCode from (select *,
	--				row_number() over (order by companyCode) number
	--				from 
	--				(SELECT 
	--					distinct CompanyCode,BranchCode
	--				FROM svHstSzkMSI
	--				where companycode in (select CompanyCode from gnMstOrganizationHdr where CompanyCode in (select DealerCode from GnMstDealerMapping where Area like case when @Area = '' then '%%' else @Area end and DealerCode like case when @CompanyCode = '' then '%%' else @CompanyCode end))) a)b
	--				where b.number=@intFlag)

	set  @BranchCode1= (select BranchCode from (select *,
													row_number() over (order by companyCode) number
													from (SELECT 
															distinct CompanyCode,BranchCode
															FROM svHstSzkMSI
															where companycode in (select DealerCode from (select * from #tempTable3)x)
															and BranchCode in (select OutletCode from (select * from #tempTable3)x)
															) a
													)b
					where b.number=@intFlag)

	SET @intFlag = @intFlag + 1


--set @sgaa_1 = (select x.DPPAmt from (select 
--					inv.CompanyCode,
--					inv.BranchCode,
--					year(inv.InvoiceDate) PeriodYear,
--					MONTH(inv.InvoiceDate) PeriodMonth,
--					sum((invi.SupplyQty * invi.RetailPrice) - (invi.RetailPrice*(invi.discPct/100))) as DPPAmt
--				from svTrnInvItem invi	
--				inner join [svTrnInvoice] inv on inv.CompanyCode=invi.CompanyCode and inv.BranchCode=invi.BranchCode and inv.InvoiceNo=invi.InvoiceNo
--				where invi.TypeOfGoods = '2'
--				and invi.companycode=@CompanyCode1
--				and invi.branchCode=@BranchCode1
--				and year(inv.InvoiceDate)=@PeriodYear
--				and MONTH(inv.InvoiceDate)='1'
--				group by inv.CompanyCode,inv.BranchCode,year(inv.InvoiceDate),MONTH(inv.InvoiceDate)
--				)x)
--set @sgaa_2 = (select x.DPPAmt from (select 
--					inv.CompanyCode,
--					inv.BranchCode,
--					year(inv.InvoiceDate) PeriodYear,
--					MONTH(inv.InvoiceDate) PeriodMonth,
--					sum((invi.SupplyQty * invi.RetailPrice) - (invi.RetailPrice*(invi.discPct/100))) as DPPAmt
--				from svTrnInvItem invi	
--				inner join [svTrnInvoice] inv on inv.CompanyCode=invi.CompanyCode and inv.BranchCode=invi.BranchCode and inv.InvoiceNo=invi.InvoiceNo
--				where invi.TypeOfGoods = '2'
--				and invi.companycode=@CompanyCode1
--				and invi.branchCode=@BranchCode1
--				and year(inv.InvoiceDate)=@PeriodYear
--				and MONTH(inv.InvoiceDate)='2'
--				group by inv.CompanyCode,inv.BranchCode,year(inv.InvoiceDate),MONTH(inv.InvoiceDate)
--				)x)
--set @sgaa_3 = (select x.DPPAmt from (select 
--					inv.CompanyCode,
--					inv.BranchCode,
--					year(inv.InvoiceDate) PeriodYear,
--					MONTH(inv.InvoiceDate) PeriodMonth,
--					sum((invi.SupplyQty * invi.RetailPrice) - (invi.RetailPrice*(invi.discPct/100))) as DPPAmt
--				from svTrnInvItem invi	
--				inner join [svTrnInvoice] inv on inv.CompanyCode=invi.CompanyCode and inv.BranchCode=invi.BranchCode and inv.InvoiceNo=invi.InvoiceNo
--				where invi.TypeOfGoods = '2'
--				and invi.companycode=@CompanyCode1
--				and invi.branchCode=@BranchCode1
--				and year(inv.InvoiceDate)=@PeriodYear
--				and MONTH(inv.InvoiceDate)='3'
--				group by inv.CompanyCode,inv.BranchCode,year(inv.InvoiceDate),MONTH(inv.InvoiceDate)
--				)x)
--set @sgaa_4 = (select x.DPPAmt from (select 
--					inv.CompanyCode,
--					inv.BranchCode,
--					year(inv.InvoiceDate) PeriodYear,
--					MONTH(inv.InvoiceDate) PeriodMonth,
--					sum((invi.SupplyQty * invi.RetailPrice) - (invi.RetailPrice*(invi.discPct/100))) as DPPAmt
--				from svTrnInvItem invi	
--				inner join [svTrnInvoice] inv on inv.CompanyCode=invi.CompanyCode and inv.BranchCode=invi.BranchCode and inv.InvoiceNo=invi.InvoiceNo
--				where invi.TypeOfGoods = '2'
--				and invi.companycode=@CompanyCode1
--				and invi.branchCode=@BranchCode1
--				and year(inv.InvoiceDate)=@PeriodYear
--				and MONTH(inv.InvoiceDate)='4'
--				group by inv.CompanyCode,inv.BranchCode,year(inv.InvoiceDate),MONTH(inv.InvoiceDate)
--				)x)
--set @sgaa_5 = (select x.DPPAmt from (select 
--					inv.CompanyCode,
--					inv.BranchCode,
--					year(inv.InvoiceDate) PeriodYear,
--					MONTH(inv.InvoiceDate) PeriodMonth,
--					sum((invi.SupplyQty * invi.RetailPrice) - (invi.RetailPrice*(invi.discPct/100))) as DPPAmt
--				from svTrnInvItem invi	
--				inner join [svTrnInvoice] inv on inv.CompanyCode=invi.CompanyCode and inv.BranchCode=invi.BranchCode and inv.InvoiceNo=invi.InvoiceNo
--				where invi.TypeOfGoods = '2'
--				and invi.companycode=@CompanyCode1
--				and invi.branchCode=@BranchCode1
--				and year(inv.InvoiceDate)=@PeriodYear
--				and MONTH(inv.InvoiceDate)='5'
--				group by inv.CompanyCode,inv.BranchCode,year(inv.InvoiceDate),MONTH(inv.InvoiceDate)
--				)x)
--set @sgaa_6 = (select x.DPPAmt from (select 
--					inv.CompanyCode,
--					inv.BranchCode,
--					year(inv.InvoiceDate) PeriodYear,
--					MONTH(inv.InvoiceDate) PeriodMonth,
--					sum((invi.SupplyQty * invi.RetailPrice) - (invi.RetailPrice*(invi.discPct/100))) as DPPAmt
--				from svTrnInvItem invi	
--				inner join [svTrnInvoice] inv on inv.CompanyCode=invi.CompanyCode and inv.BranchCode=invi.BranchCode and inv.InvoiceNo=invi.InvoiceNo
--				where invi.TypeOfGoods = '2'
--				and invi.companycode=@CompanyCode1
--				and invi.branchCode=@BranchCode1
--				and year(inv.InvoiceDate)=@PeriodYear
--				and MONTH(inv.InvoiceDate)='6'
--				group by inv.CompanyCode,inv.BranchCode,year(inv.InvoiceDate),MONTH(inv.InvoiceDate)
--				)x)
--set @sgaa_7 = (select x.DPPAmt from (select 
--					inv.CompanyCode,
--					inv.BranchCode,
--					year(inv.InvoiceDate) PeriodYear,
--					MONTH(inv.InvoiceDate) PeriodMonth,
--					sum((invi.SupplyQty * invi.RetailPrice) - (invi.RetailPrice*(invi.discPct/100))) as DPPAmt
--				from svTrnInvItem invi	
--				inner join [svTrnInvoice] inv on inv.CompanyCode=invi.CompanyCode and inv.BranchCode=invi.BranchCode and inv.InvoiceNo=invi.InvoiceNo
--				where invi.TypeOfGoods = '2'
--				and invi.companycode=@CompanyCode1
--				and invi.branchCode=@BranchCode1
--				and year(inv.InvoiceDate)=@PeriodYear
--				and MONTH(inv.InvoiceDate)='7'
--				group by inv.CompanyCode,inv.BranchCode,year(inv.InvoiceDate),MONTH(inv.InvoiceDate)
--				)x)
--set @sgaa_8 = (select x.DPPAmt from (select 
--					inv.CompanyCode,
--					inv.BranchCode,
--					year(inv.InvoiceDate) PeriodYear,
--					MONTH(inv.InvoiceDate) PeriodMonth,
--					sum((invi.SupplyQty * invi.RetailPrice) - (invi.RetailPrice*(invi.discPct/100))) as DPPAmt
--				from svTrnInvItem invi	
--				inner join [svTrnInvoice] inv on inv.CompanyCode=invi.CompanyCode and inv.BranchCode=invi.BranchCode and inv.InvoiceNo=invi.InvoiceNo
--				where invi.TypeOfGoods = '2'
--				and invi.companycode=@CompanyCode1
--				and invi.branchCode=@BranchCode1
--				and year(inv.InvoiceDate)=@PeriodYear
--				and MONTH(inv.InvoiceDate)='8'
--				group by inv.CompanyCode,inv.BranchCode,year(inv.InvoiceDate),MONTH(inv.InvoiceDate)
--				)x)
--set @sgaa_9 = (select x.DPPAmt from (select 
--					inv.CompanyCode,
--					inv.BranchCode,
--					year(inv.InvoiceDate) PeriodYear,
--					MONTH(inv.InvoiceDate) PeriodMonth,
--					sum((invi.SupplyQty * invi.RetailPrice) - (invi.RetailPrice*(invi.discPct/100))) as DPPAmt
--				from svTrnInvItem invi	
--				inner join [svTrnInvoice] inv on inv.CompanyCode=invi.CompanyCode and inv.BranchCode=invi.BranchCode and inv.InvoiceNo=invi.InvoiceNo
--				where invi.TypeOfGoods = '2'
--				and invi.companycode=@CompanyCode1
--				and invi.branchCode=@BranchCode1
--				and year(inv.InvoiceDate)=@PeriodYear
--				and MONTH(inv.InvoiceDate)='9'
--				group by inv.CompanyCode,inv.BranchCode,year(inv.InvoiceDate),MONTH(inv.InvoiceDate)
--				)x)
--set @sgaa_10 = (select x.DPPAmt from (select 
--					inv.CompanyCode,
--					inv.BranchCode,
--					year(inv.InvoiceDate) PeriodYear,
--					MONTH(inv.InvoiceDate) PeriodMonth,
--					sum((invi.SupplyQty * invi.RetailPrice) - (invi.RetailPrice*(invi.discPct/100))) as DPPAmt
--				from svTrnInvItem invi	
--				inner join [svTrnInvoice] inv on inv.CompanyCode=invi.CompanyCode and inv.BranchCode=invi.BranchCode and inv.InvoiceNo=invi.InvoiceNo
--				where invi.TypeOfGoods = '2'
--				and invi.companycode=@CompanyCode1
--				and invi.branchCode=@BranchCode1
--				and year(inv.InvoiceDate)=@PeriodYear
--				and MONTH(inv.InvoiceDate)='10'
--				group by inv.CompanyCode,inv.BranchCode,year(inv.InvoiceDate),MONTH(inv.InvoiceDate)
--				)x)
--set @sgaa_11 = (select x.DPPAmt from (select 
--					inv.CompanyCode,
--					inv.BranchCode,
--					year(inv.InvoiceDate) PeriodYear,
--					MONTH(inv.InvoiceDate) PeriodMonth,
--					sum((invi.SupplyQty * invi.RetailPrice) - (invi.RetailPrice*(invi.discPct/100))) as DPPAmt
--				from svTrnInvItem invi	
--				inner join [svTrnInvoice] inv on inv.CompanyCode=invi.CompanyCode and inv.BranchCode=invi.BranchCode and inv.InvoiceNo=invi.InvoiceNo
--				where invi.TypeOfGoods = '2'
--				and invi.companycode=@CompanyCode1
--				and invi.branchCode=@BranchCode1
--				and year(inv.InvoiceDate)=@PeriodYear
--				and MONTH(inv.InvoiceDate)='11'
--				group by inv.CompanyCode,inv.BranchCode,year(inv.InvoiceDate),MONTH(inv.InvoiceDate)
--				)x)
--set @sgaa_12 = (select x.DPPAmt from (select 
--					inv.CompanyCode,
--					inv.BranchCode,
--					year(inv.InvoiceDate) PeriodYear,
--					MONTH(inv.InvoiceDate) PeriodMonth,
--					sum((invi.SupplyQty * invi.RetailPrice) - (invi.RetailPrice*(invi.discPct/100))) as DPPAmt
--				from svTrnInvItem invi	
--				inner join [svTrnInvoice] inv on inv.CompanyCode=invi.CompanyCode and inv.BranchCode=invi.BranchCode and inv.InvoiceNo=invi.InvoiceNo
--				where invi.TypeOfGoods = '2'
--				and invi.companycode=@CompanyCode1
--				and invi.branchCode=@BranchCode1
--				and year(inv.InvoiceDate)=@PeriodYear
--				and MONTH(inv.InvoiceDate)='12'
--				group by inv.CompanyCode,inv.BranchCode,year(inv.InvoiceDate),MONTH(inv.InvoiceDate)
--				)x)

--set @sgam_1=(select x.DPPAmt from (select 
--					inv.CompanyCode,
--					inv.BranchCode,
--					year(inv.InvoiceDate) PeriodYear,
--					MONTH(inv.InvoiceDate) PeriodMonth,
--					sum((invi.SupplyQty * invi.RetailPrice) - (invi.RetailPrice*(invi.discPct/100))) as DPPAmt
--				from svTrnInvItem invi	
--				inner join [svTrnInvoice] inv on inv.CompanyCode=invi.CompanyCode and inv.BranchCode=invi.BranchCode and inv.InvoiceNo=invi.InvoiceNo
--				where invi.TypeOfGoods in (5,6)
--				and invi.companycode=@CompanyCode1
--				and invi.branchCode=@BranchCode1
--				and year(inv.InvoiceDate)=@PeriodYear
--				and MONTH(inv.InvoiceDate)='1'
--				group by inv.CompanyCode,inv.BranchCode,year(inv.InvoiceDate),MONTH(inv.InvoiceDate)
--				)x)
--set @sgam_2=(select x.DPPAmt from (select 
--					inv.CompanyCode,
--					inv.BranchCode,
--					year(inv.InvoiceDate) PeriodYear,
--					MONTH(inv.InvoiceDate) PeriodMonth,
--					sum((invi.SupplyQty * invi.RetailPrice) - (invi.RetailPrice*(invi.discPct/100))) as DPPAmt
--				from svTrnInvItem invi	
--				inner join [svTrnInvoice] inv on inv.CompanyCode=invi.CompanyCode and inv.BranchCode=invi.BranchCode and inv.InvoiceNo=invi.InvoiceNo
--				where invi.TypeOfGoods in (5,6)
--				and invi.companycode=@CompanyCode1
--				and invi.branchCode=@BranchCode1
--				and year(inv.InvoiceDate)=@PeriodYear
--				and MONTH(inv.InvoiceDate)='2'
--				group by inv.CompanyCode,inv.BranchCode,year(inv.InvoiceDate),MONTH(inv.InvoiceDate)
--				)x)
--set @sgam_3=(select x.DPPAmt from (select 
--					inv.CompanyCode,
--					inv.BranchCode,
--					year(inv.InvoiceDate) PeriodYear,
--					MONTH(inv.InvoiceDate) PeriodMonth,
--					sum((invi.SupplyQty * invi.RetailPrice) - (invi.RetailPrice*(invi.discPct/100))) as DPPAmt
--				from svTrnInvItem invi	
--				inner join [svTrnInvoice] inv on inv.CompanyCode=invi.CompanyCode and inv.BranchCode=invi.BranchCode and inv.InvoiceNo=invi.InvoiceNo
--				where invi.TypeOfGoods in (5,6)
--				and invi.companycode=@CompanyCode1
--				and invi.branchCode=@BranchCode1
--				and year(inv.InvoiceDate)=@PeriodYear
--				and MONTH(inv.InvoiceDate)='3'
--				group by inv.CompanyCode,inv.BranchCode,year(inv.InvoiceDate),MONTH(inv.InvoiceDate)
--				)x)
--set @sgam_4=(select x.DPPAmt from (select 
--					inv.CompanyCode,
--					inv.BranchCode,
--					year(inv.InvoiceDate) PeriodYear,
--					MONTH(inv.InvoiceDate) PeriodMonth,
--					sum((invi.SupplyQty * invi.RetailPrice) - (invi.RetailPrice*(invi.discPct/100))) as DPPAmt
--				from svTrnInvItem invi	
--				inner join [svTrnInvoice] inv on inv.CompanyCode=invi.CompanyCode and inv.BranchCode=invi.BranchCode and inv.InvoiceNo=invi.InvoiceNo
--				where invi.TypeOfGoods in (5,6)
--				and invi.companycode=@CompanyCode1
--				and invi.branchCode=@BranchCode1
--				and year(inv.InvoiceDate)=@PeriodYear
--				and MONTH(inv.InvoiceDate)='4'
--				group by inv.CompanyCode,inv.BranchCode,year(inv.InvoiceDate),MONTH(inv.InvoiceDate)
--				)x)
--set @sgam_5=(select x.DPPAmt from (select 
--					inv.CompanyCode,
--					inv.BranchCode,
--					year(inv.InvoiceDate) PeriodYear,
--					MONTH(inv.InvoiceDate) PeriodMonth,
--					sum((invi.SupplyQty * invi.RetailPrice) - (invi.RetailPrice*(invi.discPct/100))) as DPPAmt
--				from svTrnInvItem invi	
--				inner join [svTrnInvoice] inv on inv.CompanyCode=invi.CompanyCode and inv.BranchCode=invi.BranchCode and inv.InvoiceNo=invi.InvoiceNo
--				where invi.TypeOfGoods in (5,6)
--				and invi.companycode=@CompanyCode1
--				and invi.branchCode=@BranchCode1
--				and year(inv.InvoiceDate)=@PeriodYear
--				and MONTH(inv.InvoiceDate)='5'
--				group by inv.CompanyCode,inv.BranchCode,year(inv.InvoiceDate),MONTH(inv.InvoiceDate)
--				)x)
--set @sgam_6=(select x.DPPAmt from (select 
--					inv.CompanyCode,
--					inv.BranchCode,
--					year(inv.InvoiceDate) PeriodYear,
--					MONTH(inv.InvoiceDate) PeriodMonth,
--					sum((invi.SupplyQty * invi.RetailPrice) - (invi.RetailPrice*(invi.discPct/100))) as DPPAmt
--				from svTrnInvItem invi	
--				inner join [svTrnInvoice] inv on inv.CompanyCode=invi.CompanyCode and inv.BranchCode=invi.BranchCode and inv.InvoiceNo=invi.InvoiceNo
--				where invi.TypeOfGoods in (5,6)
--				and invi.companycode=@CompanyCode1
--				and invi.branchCode=@BranchCode1
--				and year(inv.InvoiceDate)=@PeriodYear
--				and MONTH(inv.InvoiceDate)='6'
--				group by inv.CompanyCode,inv.BranchCode,year(inv.InvoiceDate),MONTH(inv.InvoiceDate)
--				)x)
--set @sgam_7=(select x.DPPAmt from (select 
--					inv.CompanyCode,
--					inv.BranchCode,
--					year(inv.InvoiceDate) PeriodYear,
--					MONTH(inv.InvoiceDate) PeriodMonth,
--					sum((invi.SupplyQty * invi.RetailPrice) - (invi.RetailPrice*(invi.discPct/100))) as DPPAmt
--				from svTrnInvItem invi	
--				inner join [svTrnInvoice] inv on inv.CompanyCode=invi.CompanyCode and inv.BranchCode=invi.BranchCode and inv.InvoiceNo=invi.InvoiceNo
--				where invi.TypeOfGoods in (5,6)
--				and invi.companycode=@CompanyCode1
--				and invi.branchCode=@BranchCode1
--				and year(inv.InvoiceDate)=@PeriodYear
--				and MONTH(inv.InvoiceDate)='7'
--				group by inv.CompanyCode,inv.BranchCode,year(inv.InvoiceDate),MONTH(inv.InvoiceDate)
--				)x)
--set @sgam_8=(select x.DPPAmt from (select 
--					inv.CompanyCode,
--					inv.BranchCode,
--					year(inv.InvoiceDate) PeriodYear,
--					MONTH(inv.InvoiceDate) PeriodMonth,
--					sum((invi.SupplyQty * invi.RetailPrice) - (invi.RetailPrice*(invi.discPct/100))) as DPPAmt
--				from svTrnInvItem invi	
--				inner join [svTrnInvoice] inv on inv.CompanyCode=invi.CompanyCode and inv.BranchCode=invi.BranchCode and inv.InvoiceNo=invi.InvoiceNo
--				where invi.TypeOfGoods in (5,6)
--				and invi.companycode=@CompanyCode1
--				and invi.branchCode=@BranchCode1
--				and year(inv.InvoiceDate)=@PeriodYear
--				and MONTH(inv.InvoiceDate)='8'
--				group by inv.CompanyCode,inv.BranchCode,year(inv.InvoiceDate),MONTH(inv.InvoiceDate)
--				)x)
--set @sgam_9=(select x.DPPAmt from (select 
--					inv.CompanyCode,
--					inv.BranchCode,
--					year(inv.InvoiceDate) PeriodYear,
--					MONTH(inv.InvoiceDate) PeriodMonth,
--					sum((invi.SupplyQty * invi.RetailPrice) - (invi.RetailPrice*(invi.discPct/100))) as DPPAmt
--				from svTrnInvItem invi	
--				inner join [svTrnInvoice] inv on inv.CompanyCode=invi.CompanyCode and inv.BranchCode=invi.BranchCode and inv.InvoiceNo=invi.InvoiceNo
--				where invi.TypeOfGoods in (5,6)
--				and invi.companycode=@CompanyCode1
--				and invi.branchCode=@BranchCode1
--				and year(inv.InvoiceDate)=@PeriodYear
--				and MONTH(inv.InvoiceDate)='9'
--				group by inv.CompanyCode,inv.BranchCode,year(inv.InvoiceDate),MONTH(inv.InvoiceDate)
--				)x)
--set @sgam_10=(select x.DPPAmt from (select 
--					inv.CompanyCode,
--					inv.BranchCode,
--					year(inv.InvoiceDate) PeriodYear,
--					MONTH(inv.InvoiceDate) PeriodMonth,
--					sum((invi.SupplyQty * invi.RetailPrice) - (invi.RetailPrice*(invi.discPct/100))) as DPPAmt
--				from svTrnInvItem invi	
--				inner join [svTrnInvoice] inv on inv.CompanyCode=invi.CompanyCode and inv.BranchCode=invi.BranchCode and inv.InvoiceNo=invi.InvoiceNo
--				where invi.TypeOfGoods in (5,6)
--				and invi.companycode=@CompanyCode1
--				and invi.branchCode=@BranchCode1
--				and year(inv.InvoiceDate)=@PeriodYear
--				and MONTH(inv.InvoiceDate)='10'
--				group by inv.CompanyCode,inv.BranchCode,year(inv.InvoiceDate),MONTH(inv.InvoiceDate)
--				)x)
--set @sgam_11=(select x.DPPAmt from (select 
--					inv.CompanyCode,
--					inv.BranchCode,
--					year(inv.InvoiceDate) PeriodYear,
--					MONTH(inv.InvoiceDate) PeriodMonth,
--					sum((invi.SupplyQty * invi.RetailPrice) - (invi.RetailPrice*(invi.discPct/100))) as DPPAmt
--				from svTrnInvItem invi	
--				inner join [svTrnInvoice] inv on inv.CompanyCode=invi.CompanyCode and inv.BranchCode=invi.BranchCode and inv.InvoiceNo=invi.InvoiceNo
--				where invi.TypeOfGoods in (5,6)
--				and invi.companycode=@CompanyCode1
--				and invi.branchCode=@BranchCode1
--				and year(inv.InvoiceDate)=@PeriodYear
--				and MONTH(inv.InvoiceDate)='11'
--				group by inv.CompanyCode,inv.BranchCode,year(inv.InvoiceDate),MONTH(inv.InvoiceDate)
--				)x)
--set @sgam_12=(select x.DPPAmt from (select 
--					inv.CompanyCode,
--					inv.BranchCode,
--					year(inv.InvoiceDate) PeriodYear,
--					MONTH(inv.InvoiceDate) PeriodMonth,
--					sum((invi.SupplyQty * invi.RetailPrice) - (invi.RetailPrice*(invi.discPct/100))) as DPPAmt
--				from svTrnInvItem invi	
--				inner join [svTrnInvoice] inv on inv.CompanyCode=invi.CompanyCode and inv.BranchCode=invi.BranchCode and inv.InvoiceNo=invi.InvoiceNo
--				where invi.TypeOfGoods in (5,6)
--				and invi.companycode=@CompanyCode1
--				and invi.branchCode=@BranchCode1
--				and year(inv.InvoiceDate)=@PeriodYear
--				and MONTH(inv.InvoiceDate)='12'
--				group by inv.CompanyCode,inv.BranchCode,year(inv.InvoiceDate),MONTH(inv.InvoiceDate)
--				)x)

--set @sgai_1=(select x.sgaInst from (select 
--				inv.CompanyCode,
--				inv.BranchCode,
--				year(inv.InvoiceDate) PeriodYear,
--				MONTH(inv.InvoiceDate) PeriodMonth,
--				count(invi.InvoiceNo) sgaInst
--			from svTrnInvItem invi	
--			inner join [svTrnInvoice] inv on inv.CompanyCode=invi.CompanyCode 
--				and inv.BranchCode=invi.BranchCode 
--				and inv.InvoiceNo=invi.InvoiceNo
--			where invi.TypeOfGoods = '2'
--			and invi.companycode=@CompanyCode1
--			and invi.branchCode=@BranchCode1
--			and year(inv.InvoiceDate)=@PeriodYear
--			and MONTH(inv.InvoiceDate)='1'
--			group by inv.CompanyCode,inv.BranchCode,year(inv.InvoiceDate),MONTH(inv.InvoiceDate)
--			)x)
--set @sgai_2=(select x.sgaInst from (select 
--				inv.CompanyCode,
--				inv.BranchCode,
--				year(inv.InvoiceDate) PeriodYear,
--				MONTH(inv.InvoiceDate) PeriodMonth,
--				count(invi.InvoiceNo) sgaInst
--			from svTrnInvItem invi	
--			inner join [svTrnInvoice] inv on inv.CompanyCode=invi.CompanyCode 
--				and inv.BranchCode=invi.BranchCode 
--				and inv.InvoiceNo=invi.InvoiceNo
--			where invi.TypeOfGoods = '2'
--			and invi.companycode=@CompanyCode1
--			and invi.branchCode=@BranchCode1
--			and year(inv.InvoiceDate)=@PeriodYear
--			and MONTH(inv.InvoiceDate)='2'
--			group by inv.CompanyCode,inv.BranchCode,year(inv.InvoiceDate),MONTH(inv.InvoiceDate)
--			)x)
--set @sgai_3=(select x.sgaInst from (select 
--				inv.CompanyCode,
--				inv.BranchCode,
--				year(inv.InvoiceDate) PeriodYear,
--				MONTH(inv.InvoiceDate) PeriodMonth,
--				count(invi.InvoiceNo) sgaInst
--			from svTrnInvItem invi	
--			inner join [svTrnInvoice] inv on inv.CompanyCode=invi.CompanyCode 
--				and inv.BranchCode=invi.BranchCode 
--				and inv.InvoiceNo=invi.InvoiceNo
--			where invi.TypeOfGoods = '2'
--			and invi.companycode=@CompanyCode1
--			and invi.branchCode=@BranchCode1
--			and year(inv.InvoiceDate)=@PeriodYear
--			and MONTH(inv.InvoiceDate)='3'
--			group by inv.CompanyCode,inv.BranchCode,year(inv.InvoiceDate),MONTH(inv.InvoiceDate)
--			)x)
--set @sgai_4=(select x.sgaInst from (select 
--				inv.CompanyCode,
--				inv.BranchCode,
--				year(inv.InvoiceDate) PeriodYear,
--				MONTH(inv.InvoiceDate) PeriodMonth,
--				count(invi.InvoiceNo) sgaInst
--			from svTrnInvItem invi	
--			inner join [svTrnInvoice] inv on inv.CompanyCode=invi.CompanyCode 
--				and inv.BranchCode=invi.BranchCode 
--				and inv.InvoiceNo=invi.InvoiceNo
--			where invi.TypeOfGoods = '2'
--			and invi.companycode=@CompanyCode1
--			and invi.branchCode=@BranchCode1
--			and year(inv.InvoiceDate)=@PeriodYear
--			and MONTH(inv.InvoiceDate)='4'
--			group by inv.CompanyCode,inv.BranchCode,year(inv.InvoiceDate),MONTH(inv.InvoiceDate)
--			)x)
--set @sgai_5=(select x.sgaInst from (select 
--				inv.CompanyCode,
--				inv.BranchCode,
--				year(inv.InvoiceDate) PeriodYear,
--				MONTH(inv.InvoiceDate) PeriodMonth,
--				count(invi.InvoiceNo) sgaInst
--			from svTrnInvItem invi	
--			inner join [svTrnInvoice] inv on inv.CompanyCode=invi.CompanyCode 
--				and inv.BranchCode=invi.BranchCode 
--				and inv.InvoiceNo=invi.InvoiceNo
--			where invi.TypeOfGoods = '2'
--			and invi.companycode=@CompanyCode1
--			and invi.branchCode=@BranchCode1
--			and year(inv.InvoiceDate)=@PeriodYear
--			and MONTH(inv.InvoiceDate)='5'
--			group by inv.CompanyCode,inv.BranchCode,year(inv.InvoiceDate),MONTH(inv.InvoiceDate)
--			)x)
--set @sgai_6=(select x.sgaInst from (select 
--				inv.CompanyCode,
--				inv.BranchCode,
--				year(inv.InvoiceDate) PeriodYear,
--				MONTH(inv.InvoiceDate) PeriodMonth,
--				count(invi.InvoiceNo) sgaInst
--			from svTrnInvItem invi	
--			inner join [svTrnInvoice] inv on inv.CompanyCode=invi.CompanyCode 
--				and inv.BranchCode=invi.BranchCode 
--				and inv.InvoiceNo=invi.InvoiceNo
--			where invi.TypeOfGoods = '2'
--			and invi.companycode=@CompanyCode1
--			and invi.branchCode=@BranchCode1
--			and year(inv.InvoiceDate)=@PeriodYear
--			and MONTH(inv.InvoiceDate)='6'
--			group by inv.CompanyCode,inv.BranchCode,year(inv.InvoiceDate),MONTH(inv.InvoiceDate)
--			)x)
--set @sgai_7=(select x.sgaInst from (select 
--				inv.CompanyCode,
--				inv.BranchCode,
--				year(inv.InvoiceDate) PeriodYear,
--				MONTH(inv.InvoiceDate) PeriodMonth,
--				count(invi.InvoiceNo) sgaInst
--			from svTrnInvItem invi	
--			inner join [svTrnInvoice] inv on inv.CompanyCode=invi.CompanyCode 
--				and inv.BranchCode=invi.BranchCode 
--				and inv.InvoiceNo=invi.InvoiceNo
--			where invi.TypeOfGoods = '2'
--			and invi.companycode=@CompanyCode1
--			and invi.branchCode=@BranchCode1
--			and year(inv.InvoiceDate)=@PeriodYear
--			and MONTH(inv.InvoiceDate)='7'
--			group by inv.CompanyCode,inv.BranchCode,year(inv.InvoiceDate),MONTH(inv.InvoiceDate)
--			)x)
--set @sgai_8=(select x.sgaInst from (select 
--				inv.CompanyCode,
--				inv.BranchCode,
--				year(inv.InvoiceDate) PeriodYear,
--				MONTH(inv.InvoiceDate) PeriodMonth,
--				count(invi.InvoiceNo) sgaInst
--			from svTrnInvItem invi	
--			inner join [svTrnInvoice] inv on inv.CompanyCode=invi.CompanyCode 
--				and inv.BranchCode=invi.BranchCode 
--				and inv.InvoiceNo=invi.InvoiceNo
--			where invi.TypeOfGoods = '2'
--			and invi.companycode=@CompanyCode1
--			and invi.branchCode=@BranchCode1
--			and year(inv.InvoiceDate)=@PeriodYear
--			and MONTH(inv.InvoiceDate)='8'
--			group by inv.CompanyCode,inv.BranchCode,year(inv.InvoiceDate),MONTH(inv.InvoiceDate)
--			)x)
--set @sgai_9=(select x.sgaInst from (select 
--				inv.CompanyCode,
--				inv.BranchCode,
--				year(inv.InvoiceDate) PeriodYear,
--				MONTH(inv.InvoiceDate) PeriodMonth,
--				count(invi.InvoiceNo) sgaInst
--			from svTrnInvItem invi	
--			inner join [svTrnInvoice] inv on inv.CompanyCode=invi.CompanyCode 
--				and inv.BranchCode=invi.BranchCode 
--				and inv.InvoiceNo=invi.InvoiceNo
--			where invi.TypeOfGoods = '2'
--			and invi.companycode=@CompanyCode1
--			and invi.branchCode=@BranchCode1
--			and year(inv.InvoiceDate)=@PeriodYear
--			and MONTH(inv.InvoiceDate)='9'
--			group by inv.CompanyCode,inv.BranchCode,year(inv.InvoiceDate),MONTH(inv.InvoiceDate)
--			)x)
--set @sgai_10=(select x.sgaInst from (select 
--				inv.CompanyCode,
--				inv.BranchCode,
--				year(inv.InvoiceDate) PeriodYear,
--				MONTH(inv.InvoiceDate) PeriodMonth,
--				count(invi.InvoiceNo) sgaInst
--			from svTrnInvItem invi	
--			inner join [svTrnInvoice] inv on inv.CompanyCode=invi.CompanyCode 
--				and inv.BranchCode=invi.BranchCode 
--				and inv.InvoiceNo=invi.InvoiceNo
--			where invi.TypeOfGoods = '2'
--			and invi.companycode=@CompanyCode1
--			and invi.branchCode=@BranchCode1
--			and year(inv.InvoiceDate)=@PeriodYear
--			and MONTH(inv.InvoiceDate)='10'
--			group by inv.CompanyCode,inv.BranchCode,year(inv.InvoiceDate),MONTH(inv.InvoiceDate)
--			)x)
--set @sgai_11=(select x.sgaInst from (select 
--				inv.CompanyCode,
--				inv.BranchCode,
--				year(inv.InvoiceDate) PeriodYear,
--				MONTH(inv.InvoiceDate) PeriodMonth,
--				count(invi.InvoiceNo) sgaInst
--			from svTrnInvItem invi	
--			inner join [svTrnInvoice] inv on inv.CompanyCode=invi.CompanyCode 
--				and inv.BranchCode=invi.BranchCode 
--				and inv.InvoiceNo=invi.InvoiceNo
--			where invi.TypeOfGoods = '2'
--			and invi.companycode=@CompanyCode1
--			and invi.branchCode=@BranchCode1
--			and year(inv.InvoiceDate)=@PeriodYear
--			and MONTH(inv.InvoiceDate)='11'
--			group by inv.CompanyCode,inv.BranchCode,year(inv.InvoiceDate),MONTH(inv.InvoiceDate)
--			)x)
--set @sgai_12=(select x.sgaInst from (select 
--				inv.CompanyCode,
--				inv.BranchCode,
--				year(inv.InvoiceDate) PeriodYear,
--				MONTH(inv.InvoiceDate) PeriodMonth,
--				count(invi.InvoiceNo) sgaInst
--			from svTrnInvItem invi	
--			inner join [svTrnInvoice] inv on inv.CompanyCode=invi.CompanyCode 
--				and inv.BranchCode=invi.BranchCode 
--				and inv.InvoiceNo=invi.InvoiceNo
--			where invi.TypeOfGoods = '2'
--			and invi.companycode=@CompanyCode1
--			and invi.branchCode=@BranchCode1
--			and year(inv.InvoiceDate)=@PeriodYear
--			and MONTH(inv.InvoiceDate)='12'
--			group by inv.CompanyCode,inv.BranchCode,year(inv.InvoiceDate),MONTH(inv.InvoiceDate)
--			)x)

--set @stl1_1 = (select x.Stall from (select 
--				companycode
--				,branchCode
--				 ,@PeriodYear periodyear
--				 ,'1' periodMonh
--				 , count(stallCode) as Stall
--			from svMstStall 
--			where IsActive=1 
--			and CompanyCode=@CompanyCode1
--			and branchCode=@BranchCode1
--			and StallCode not in ('REGULER')
--			group by companycode,BranchCode) x)
--set @stl1_2 = (select x.Stall from (select 
--				companycode
--				,branchCode
--				 ,@PeriodYear periodyear
--				 ,'2' periodMonh
--				 , count(stallCode) as Stall
--			from svMstStall 
--			where IsActive=1 
--			and CompanyCode=@CompanyCode1
--			and branchCode=@BranchCode1
--			and StallCode not in ('REGULER')
--			group by companycode,BranchCode) x)
--set @stl1_3 = (select x.Stall from (select 
--				companycode
--				,branchCode
--				 ,@PeriodYear periodyear
--				 ,'3' periodMonh
--				 , count(stallCode) as Stall
--			from svMstStall 
--			where IsActive=1 
--			and CompanyCode=@CompanyCode1
--			and branchCode=@BranchCode1
--			and StallCode not in ('REGULER')
--			group by companycode,BranchCode) x)
--set @stl1_4 = (select x.Stall from (select 
--				companycode
--				,branchCode
--				 ,@PeriodYear periodyear
--				 ,'4' periodMonh
--				 , count(stallCode) as Stall
--			from svMstStall 
--			where IsActive=1 
--			and CompanyCode=@CompanyCode1
--			and branchCode=@BranchCode1
--			and StallCode not in ('REGULER')
--			group by companycode,BranchCode) x)
--set @stl1_5 = (select x.Stall from (select 
--				companycode
--				,branchCode
--				 ,@PeriodYear periodyear
--				 ,'5' periodMonh
--				 , count(stallCode) as Stall
--			from svMstStall 
--			where IsActive=1 
--			and CompanyCode=@CompanyCode1
--			and branchCode=@BranchCode1
--			and StallCode not in ('REGULER')
--			group by companycode,BranchCode) x)
--set @stl1_6 = (select x.Stall from (select 
--				companycode
--				,branchCode
--				 ,@PeriodYear periodyear
--				 ,'6' periodMonh
--				 , count(stallCode) as Stall
--			from svMstStall 
--			where IsActive=1 
--			and CompanyCode=@CompanyCode1
--			and branchCode=@BranchCode1
--			and StallCode not in ('REGULER')
--			group by companycode,BranchCode) x)
--set @stl1_7 = (select x.Stall from (select 
--				companycode
--				,branchCode
--				 ,@PeriodYear periodyear
--				 ,'7' periodMonh
--				 , count(stallCode) as Stall
--			from svMstStall 
--			where IsActive=1 
--			and CompanyCode=@CompanyCode1
--			and branchCode=@BranchCode1
--			and StallCode not in ('REGULER')
--			group by companycode,BranchCode) x)
--set @stl1_8 = (select x.Stall from (select 
--				companycode
--				,branchCode
--				 ,@PeriodYear periodyear
--				 ,'8' periodMonh
--				 , count(stallCode) as Stall
--			from svMstStall 
--			where IsActive=1 
--			and CompanyCode=@CompanyCode1
--			and branchCode=@BranchCode1
--			and StallCode not in ('REGULER')
--			group by companycode,BranchCode) x)
--set @stl1_9 = (select x.Stall from (select 
--				companycode
--				,branchCode
--				 ,@PeriodYear periodyear
--				 ,'9' periodMonh
--				 , count(stallCode) as Stall
--			from svMstStall 
--			where IsActive=1 
--			and CompanyCode=@CompanyCode1
--			and branchCode=@BranchCode1
--			and StallCode not in ('REGULER')
--			group by companycode,BranchCode) x)
--set @stl1_10 = (select x.Stall from (select 
--				companycode
--				,branchCode
--				 ,@PeriodYear periodyear
--				 ,'10' periodMonh
--				 , count(stallCode) as Stall
--			from svMstStall 
--			where IsActive=1 
--			and CompanyCode=@CompanyCode1
--			and branchCode=@BranchCode1
--			and StallCode not in ('REGULER')
--			group by companycode,BranchCode) x)
--set @stl1_11 = (select x.Stall from (select 
--				companycode
--				,branchCode
--				 ,@PeriodYear periodyear
--				 ,'11' periodMonh
--				 , count(stallCode) as Stall
--			from svMstStall 
--			where IsActive=1 
--			and CompanyCode=@CompanyCode1
--			and branchCode=@BranchCode1
--			and StallCode not in ('REGULER')
--			group by companycode,BranchCode) x)
--set @stl1_12 = (select x.Stall from (select 
--				companycode
--				,branchCode
--				 ,@PeriodYear periodyear
--				 ,'12' periodMonh
--				 , count(stallCode) as Stall
--			from svMstStall 
--			where IsActive=1 
--			and CompanyCode=@CompanyCode1
--			and branchCode=@BranchCode1
--			and StallCode not in ('REGULER')
--			group by companycode,BranchCode) x)

--set @stl2_1 = (select x.Stall from (select 
--				companycode
--				,branchCode
--				 ,@PeriodYear periodyear
--				 ,'1' periodMonh
--				 , count(stallCode) as Stall
--			from svMstStall 
--			where IsActive=1 
--			and CompanyCode=@CompanyCode1
--			and branchCode=@BranchCode1
--			and StallCode ='REGULER'
--			group by companycode,BranchCode) x)
--set @stl2_2 = (select x.Stall from (select 
--				companycode
--				,branchCode
--				 ,@PeriodYear periodyear
--				 ,'2' periodMonh
--				 , count(stallCode) as Stall
--			from svMstStall 
--			where IsActive=1 
--			and CompanyCode=@CompanyCode1
--			and branchCode=@BranchCode1
--			and StallCode ='REGULER'
--			group by companycode,BranchCode) x)
--set @stl2_3 = (select x.Stall from (select 
--				companycode
--				,branchCode
--				 ,@PeriodYear periodyear
--				 ,'3' periodMonh
--				 , count(stallCode) as Stall
--			from svMstStall 
--			where IsActive=1 
--			and CompanyCode=@CompanyCode1
--			and branchCode=@BranchCode1
--			and StallCode ='REGULER'
--			group by companycode,BranchCode) x)
--set @stl2_4 = (select x.Stall from (select 
--				companycode
--				,branchCode
--				 ,@PeriodYear periodyear
--				 ,'4' periodMonh
--				 , count(stallCode) as Stall
--			from svMstStall 
--			where IsActive=1 
--			and CompanyCode=@CompanyCode1
--			and branchCode=@BranchCode1
--			and StallCode ='REGULER'
--			group by companycode,BranchCode) x)
--set @stl2_5 = (select x.Stall from (select 
--				companycode
--				,branchCode
--				 ,@PeriodYear periodyear
--				 ,'5' periodMonh
--				 , count(stallCode) as Stall
--			from svMstStall 
--			where IsActive=1 
--			and CompanyCode=@CompanyCode1
--			and branchCode=@BranchCode1
--			and StallCode ='REGULER'
--			group by companycode,BranchCode) x)
--set @stl2_6 = (select x.Stall from (select 
--				companycode
--				,branchCode
--				 ,@PeriodYear periodyear
--				 ,'6' periodMonh
--				 , count(stallCode) as Stall
--			from svMstStall 
--			where IsActive=1 
--			and CompanyCode=@CompanyCode1
--			and branchCode=@BranchCode1
--			and StallCode ='REGULER'
--			group by companycode,BranchCode) x)
--set @stl2_7 = (select x.Stall from (select 
--				companycode
--				,branchCode
--				 ,@PeriodYear periodyear
--				 ,'7' periodMonh
--				 , count(stallCode) as Stall
--			from svMstStall 
--			where IsActive=1 
--			and CompanyCode=@CompanyCode1
--			and branchCode=@BranchCode1
--			and StallCode ='REGULER'
--			group by companycode,BranchCode) x)
--set @stl2_8 = (select x.Stall from (select 
--				companycode
--				,branchCode
--				 ,@PeriodYear periodyear
--				 ,'8' periodMonh
--				 , count(stallCode) as Stall
--			from svMstStall 
--			where IsActive=1 
--			and CompanyCode=@CompanyCode1
--			and branchCode=@BranchCode1
--			and StallCode ='REGULER'
--			group by companycode,BranchCode) x)
--set @stl2_9 = (select x.Stall from (select 
--				companycode
--				,branchCode
--				 ,@PeriodYear periodyear
--				 ,'9' periodMonh
--				 , count(stallCode) as Stall
--			from svMstStall 
--			where IsActive=1 
--			and CompanyCode=@CompanyCode1
--			and branchCode=@BranchCode1
--			and StallCode ='REGULER'
--			group by companycode,BranchCode) x)
--set @stl2_10 = (select x.Stall from (select 
--				companycode
--				,branchCode
--				 ,@PeriodYear periodyear
--				 ,'10' periodMonh
--				 , count(stallCode) as Stall
--			from svMstStall 
--			where IsActive=1 
--			and CompanyCode=@CompanyCode1
--			and branchCode=@BranchCode1
--			and StallCode ='REGULER'
--			group by companycode,BranchCode) x)
--set @stl2_11 = (select x.Stall from (select 
--				companycode
--				,branchCode
--				 ,@PeriodYear periodyear
--				 ,'11' periodMonh
--				 , count(stallCode) as Stall
--			from svMstStall 
--			where IsActive=1 
--			and CompanyCode=@CompanyCode1
--			and branchCode=@BranchCode1
--			and StallCode ='REGULER'
--			group by companycode,BranchCode) x)
--set @stl2_12 = (select x.Stall from (select 
--				companycode
--				,branchCode
--				 ,@PeriodYear periodyear
--				 ,'12' periodMonh
--				 , count(stallCode) as Stall
--			from svMstStall 
--			where IsActive=1 
--			and CompanyCode=@CompanyCode1
--			and branchCode=@BranchCode1
--			and StallCode ='REGULER'
--			group by companycode,BranchCode) x)

;with svHstSzkMSIUnion(CompanyCode,BranchCode,PeriodYear,PeriodMonth,SeqNo,MsiGroup,MsiDesc,Unit,MsiData,lastUpdateDate) as
(
select
	CompanyCode,
		BranchCode,
		PeriodYear,
		PeriodMonth,
		SeqNo,
		MsiGroup,
		MsiDesc,
		Unit,
		MsiData,
		CreatedDate 
--from svHstSzkMSI
from svHstSzkMSI_Invoice
where 
CompanyCode =@CompanyCode1
and BranchCode=@BranchCode1
and PeriodYear=@PeriodYear
and PeriodMonth >=@Month1
and periodMonth <=@Month2

--union 
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	1 PeriodMonth,14 SeqNo,'A. Sales Revenue' MsiGroup,' aTotal Suzuki Genuine Accessories Sales Revenue - Chargeable to Customer CPUS (External) (15+16)' MsiDesc,
--	'Rp' Unit,(isnull(@sgaa_1,0)+isnull(@sgam_1,0)) MsiData,null LastUpdate

--union 
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	1 PeriodMonth,14 SeqNo,'A. Sales Revenue' MsiGroup,' SGA Accessories Parts Sales' MsiDesc,
--	'Rp' Unit,@sgaa_1 MsiData,null LastUpdate

--union 
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	1 PeriodMonth,14 SeqNo,'A. Sales Revenue' MsiGroup,' SGA Materials Sales' MsiDesc,
--	'Rp' Unit,@sgam_1 MsiData,null LastUpdate

--union
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	1 PeriodMonth,43 SeqNo,'C. No of Job Type' MsiGroup,'Accessories Installment (SGA)' MsiDesc,
--	'job' Unit,@sgai_1 MsiData,null LastUpdate

--union
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	1 PeriodMonth,57 SeqNo,'D. Workshop Service Strength' MsiGroup,' No. of Regular Stalls ( Available stalls Exclude Washing Stall and EM Stalls)' MsiDesc,
--	'stall' Unit,@stl1_1 MsiData,null LastUpdate

--union
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	1 PeriodMonth,57 SeqNo,'D. Workshop Service Strength' MsiGroup,'No. of Express Maintenance Stalls (Installed)' MsiDesc,
--	'stall' Unit,@stl2_1 MsiData,null LastUpdate

----
--union 
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	2 PeriodMonth,14 SeqNo,'A. Sales Revenue' MsiGroup,' aTotal Suzuki Genuine Accessories Sales Revenue - Chargeable to Customer CPUS (External) (15+16)' MsiDesc,
--	'Rp' Unit,(isnull(@sgaa_2,0)+isnull(@sgam_2,0)) MsiData,null LastUpdate

--union 
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	2 PeriodMonth,14 SeqNo,'A. Sales Revenue' MsiGroup,' SGA Accessories Parts Sales' MsiDesc,
--	'Rp' Unit,@sgaa_2 MsiData,null LastUpdate

--union 
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	2 PeriodMonth,14 SeqNo,'A. Sales Revenue' MsiGroup,' SGA Materials Sales' MsiDesc,
--	'Rp' Unit,@sgam_2 MsiData,null LastUpdate

--union
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	2 PeriodMonth,43 SeqNo,'C. No of Job Type' MsiGroup,'Accessories Installment (SGA)' MsiDesc,
--	'job' Unit,@sgai_2 MsiData,null LastUpdate

--union
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	2 PeriodMonth,57 SeqNo,'D. Workshop Service Strength' MsiGroup,' No. of Regular Stalls ( Available stalls Exclude Washing Stall and EM Stalls)' MsiDesc,
--	'stall' Unit,@stl1_2 MsiData,null LastUpdate

--union
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	2 PeriodMonth,57 SeqNo,'D. Workshop Service Strength' MsiGroup,'No. of Express Maintenance Stalls (Installed)' MsiDesc,
--	'stall' Unit,@stl2_2 MsiData,null LastUpdate

----
--union 
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	3 PeriodMonth,14 SeqNo,'A. Sales Revenue' MsiGroup,' aTotal Suzuki Genuine Accessories Sales Revenue - Chargeable to Customer CPUS (External) (15+16)' MsiDesc,
--	'Rp' Unit,(isnull(@sgaa_3,0)+isnull(@sgam_3,0)) MsiData,null LastUpdate

--union 
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	3 PeriodMonth,14 SeqNo,'A. Sales Revenue' MsiGroup,' SGA Accessories Parts Sales' MsiDesc,
--	'Rp' Unit,@sgaa_3 MsiData,null LastUpdate

--union 
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	3 PeriodMonth,14 SeqNo,'A. Sales Revenue' MsiGroup,' SGA Materials Sales' MsiDesc,
--	'Rp' Unit,@sgam_3 MsiData,null LastUpdate

--union
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	3 PeriodMonth,43 SeqNo,'C. No of Job Type' MsiGroup,'Accessories Installment (SGA)' MsiDesc,
--	'job' Unit,@sgai_3 MsiData,null LastUpdate

--union
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	3 PeriodMonth,57 SeqNo,'D. Workshop Service Strength' MsiGroup,' No. of Regular Stalls ( Available stalls Exclude Washing Stall and EM Stalls)' MsiDesc,
--	'stall' Unit,@stl1_3 MsiData,null LastUpdate

--union
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	3 PeriodMonth,57 SeqNo,'D. Workshop Service Strength' MsiGroup,'No. of Express Maintenance Stalls (Installed)' MsiDesc,
--	'stall' Unit,@stl2_3 MsiData,null LastUpdate

----
--union 
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	4 PeriodMonth,14 SeqNo,'A. Sales Revenue' MsiGroup,' aTotal Suzuki Genuine Accessories Sales Revenue - Chargeable to Customer CPUS (External) (15+16)' MsiDesc,
--	'Rp' Unit,(isnull(@sgaa_4,0)+isnull(@sgam_4,0)) MsiData,null LastUpdate

--union 
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	4 PeriodMonth,14 SeqNo,'A. Sales Revenue' MsiGroup,' SGA Accessories Parts Sales' MsiDesc,
--	'Rp' Unit,@sgaa_4 MsiData,null LastUpdate

--union 
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	4 PeriodMonth,14 SeqNo,'A. Sales Revenue' MsiGroup,' SGA Materials Sales' MsiDesc,
--	'Rp' Unit,@sgam_4 MsiData,null LastUpdate

--union
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	4 PeriodMonth,43 SeqNo,'C. No of Job Type' MsiGroup,'Accessories Installment (SGA)' MsiDesc,
--	'job' Unit,@sgai_4 MsiData,null LastUpdate

--union
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	4 PeriodMonth,57 SeqNo,'D. Workshop Service Strength' MsiGroup,' No. of Regular Stalls ( Available stalls Exclude Washing Stall and EM Stalls)' MsiDesc,
--	'stall' Unit,@stl1_4 MsiData,null LastUpdate

--union
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	4 PeriodMonth,57 SeqNo,'D. Workshop Service Strength' MsiGroup,'No. of Express Maintenance Stalls (Installed)' MsiDesc,
--	'stall' Unit,@stl2_4 MsiData,null LastUpdate

----
--union 
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	5 PeriodMonth,14 SeqNo,'A. Sales Revenue' MsiGroup,' aTotal Suzuki Genuine Accessories Sales Revenue - Chargeable to Customer CPUS (External) (15+16)' MsiDesc,
--	'Rp' Unit,(isnull(@sgaa_5,0)+isnull(@sgam_5,0)) MsiData,null LastUpdate

--union 
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	5 PeriodMonth,14 SeqNo,'A. Sales Revenue' MsiGroup,' SGA Accessories Parts Sales' MsiDesc,
--	'Rp' Unit,@sgaa_5 MsiData,null LastUpdate

--union 
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	5 PeriodMonth,14 SeqNo,'A. Sales Revenue' MsiGroup,' SGA Materials Sales' MsiDesc,
--	'Rp' Unit,@sgam_5 MsiData,null LastUpdate

--union
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	5 PeriodMonth,43 SeqNo,'C. No of Job Type' MsiGroup,'Accessories Installment (SGA)' MsiDesc,
--	'job' Unit,@sgai_5 MsiData,null LastUpdate

--union
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	5 PeriodMonth,57 SeqNo,'D. Workshop Service Strength' MsiGroup,' No. of Regular Stalls ( Available stalls Exclude Washing Stall and EM Stalls)' MsiDesc,
--	'stall' Unit,@stl1_5 MsiData,null LastUpdate

--union
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	5 PeriodMonth,57 SeqNo,'D. Workshop Service Strength' MsiGroup,'No. of Express Maintenance Stalls (Installed)' MsiDesc,
--	'stall' Unit,@stl2_5 MsiData,null LastUpdate

----
--union 
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	6 PeriodMonth,14 SeqNo,'A. Sales Revenue' MsiGroup,' aTotal Suzuki Genuine Accessories Sales Revenue - Chargeable to Customer CPUS (External) (15+16)' MsiDesc,
--	'Rp' Unit,(isnull(@sgaa_6,0)+isnull(@sgam_6,0)) MsiData,null LastUpdate

--union 
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	6 PeriodMonth,14 SeqNo,'A. Sales Revenue' MsiGroup,' SGA Accessories Parts Sales' MsiDesc,
--	'Rp' Unit,@sgaa_6 MsiData,null LastUpdate

--union 
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	6 PeriodMonth,14 SeqNo,'A. Sales Revenue' MsiGroup,' SGA Materials Sales' MsiDesc,
--	'Rp' Unit,@sgam_6 MsiData,null LastUpdate

--union
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	6 PeriodMonth,43 SeqNo,'C. No of Job Type' MsiGroup,'Accessories Installment (SGA)' MsiDesc,
--	'job' Unit,@sgai_6 MsiData,null LastUpdate

--union
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	6 PeriodMonth,57 SeqNo,'D. Workshop Service Strength' MsiGroup,' No. of Regular Stalls ( Available stalls Exclude Washing Stall and EM Stalls)' MsiDesc,
--	'stall' Unit,@stl1_6 MsiData,null LastUpdate

--union
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	6 PeriodMonth,57 SeqNo,'D. Workshop Service Strength' MsiGroup,'No. of Express Maintenance Stalls (Installed)' MsiDesc,
--	'stall' Unit,@stl2_6 MsiData,null LastUpdate

----
--union 
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	7 PeriodMonth,14 SeqNo,'A. Sales Revenue' MsiGroup,' aTotal Suzuki Genuine Accessories Sales Revenue - Chargeable to Customer CPUS (External) (15+16)' MsiDesc,
--	'Rp' Unit,(isnull(@sgaa_7,0)+isnull(@sgam_7,0)) MsiData,null LastUpdate

--union 
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	7 PeriodMonth,14 SeqNo,'A. Sales Revenue' MsiGroup,' SGA Accessories Parts Sales' MsiDesc,
--	'Rp' Unit,@sgaa_7 MsiData,null LastUpdate

--union 
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	7 PeriodMonth,14 SeqNo,'A. Sales Revenue' MsiGroup,' SGA Materials Sales' MsiDesc,
--	'Rp' Unit,@sgam_7 MsiData,null LastUpdate

--union
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	7 PeriodMonth,43 SeqNo,'C. No of Job Type' MsiGroup,'Accessories Installment (SGA)' MsiDesc,
--	'job' Unit,@sgai_7 MsiData,null LastUpdate

--union
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	7 PeriodMonth,57 SeqNo,'D. Workshop Service Strength' MsiGroup,' No. of Regular Stalls ( Available stalls Exclude Washing Stall and EM Stalls)' MsiDesc,
--	'stall' Unit,@stl1_7 MsiData,null LastUpdate

--union
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	7 PeriodMonth,57 SeqNo,'D. Workshop Service Strength' MsiGroup,'No. of Express Maintenance Stalls (Installed)' MsiDesc,
--	'stall' Unit,@stl2_7 MsiData,null LastUpdate

----
--union 
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	8 PeriodMonth,14 SeqNo,'A. Sales Revenue' MsiGroup,' aTotal Suzuki Genuine Accessories Sales Revenue - Chargeable to Customer CPUS (External) (15+16)' MsiDesc,
--	'Rp' Unit,(isnull(@sgaa_8,0)+isnull(@sgam_8,0)) MsiData,null LastUpdate

--union 
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	8 PeriodMonth,14 SeqNo,'A. Sales Revenue' MsiGroup,' SGA Accessories Parts Sales' MsiDesc,
--	'Rp' Unit,@sgaa_8 MsiData,null LastUpdate

--union 
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	8 PeriodMonth,14 SeqNo,'A. Sales Revenue' MsiGroup,' SGA Materials Sales' MsiDesc,
--	'Rp' Unit,@sgam_8 MsiData,null LastUpdate

--union
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	8 PeriodMonth,43 SeqNo,'C. No of Job Type' MsiGroup,'Accessories Installment (SGA)' MsiDesc,
--	'job' Unit,@sgai_8 MsiData,null LastUpdate

--union
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	8 PeriodMonth,57 SeqNo,'D. Workshop Service Strength' MsiGroup,' No. of Regular Stalls ( Available stalls Exclude Washing Stall and EM Stalls)' MsiDesc,
--	'stall' Unit,@stl1_8 MsiData,null LastUpdate

--union
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	8 PeriodMonth,57 SeqNo,'D. Workshop Service Strength' MsiGroup,'No. of Express Maintenance Stalls (Installed)' MsiDesc,
--	'stall' Unit,@stl2_8 MsiData,null LastUpdate

----
--union 
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	9 PeriodMonth,14 SeqNo,'A. Sales Revenue' MsiGroup,' aTotal Suzuki Genuine Accessories Sales Revenue - Chargeable to Customer CPUS (External) (15+16)' MsiDesc,
--	'Rp' Unit,(isnull(@sgaa_9,0)+isnull(@sgam_9,0)) MsiData,null LastUpdate

--union 
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	9 PeriodMonth,14 SeqNo,'A. Sales Revenue' MsiGroup,' SGA Accessories Parts Sales' MsiDesc,
--	'Rp' Unit,@sgaa_9 MsiData,null LastUpdate

--union 
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	9 PeriodMonth,14 SeqNo,'A. Sales Revenue' MsiGroup,' SGA Materials Sales' MsiDesc,
--	'Rp' Unit,@sgam_9 MsiData,null LastUpdate

--union
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	9 PeriodMonth,43 SeqNo,'C. No of Job Type' MsiGroup,'Accessories Installment (SGA)' MsiDesc,
--	'job' Unit,@sgai_9 MsiData,null LastUpdate

--union
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	9 PeriodMonth,57 SeqNo,'D. Workshop Service Strength' MsiGroup,' No. of Regular Stalls ( Available stalls Exclude Washing Stall and EM Stalls)' MsiDesc,
--	'stall' Unit,@stl1_9 MsiData,null LastUpdate

--union
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	9 PeriodMonth,57 SeqNo,'D. Workshop Service Strength' MsiGroup,'No. of Express Maintenance Stalls (Installed)' MsiDesc,
--	'stall' Unit,@stl2_9 MsiData,null LastUpdate

----
--union 
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	10 PeriodMonth,14 SeqNo,'A. Sales Revenue' MsiGroup,' aTotal Suzuki Genuine Accessories Sales Revenue - Chargeable to Customer CPUS (External) (15+16)' MsiDesc,
--	'Rp' Unit,(isnull(@sgaa_10,0)+isnull(@sgam_10,0)) MsiData,null LastUpdate

--union 
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	10 PeriodMonth,14 SeqNo,'A. Sales Revenue' MsiGroup,' SGA Accessories Parts Sales' MsiDesc,
--	'Rp' Unit,@sgaa_10 MsiData,null LastUpdate

--union 
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	10 PeriodMonth,14 SeqNo,'A. Sales Revenue' MsiGroup,' SGA Materials Sales' MsiDesc,
--	'Rp' Unit,@sgam_10 MsiData,null LastUpdate

--union
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	10 PeriodMonth,43 SeqNo,'C. No of Job Type' MsiGroup,'Accessories Installment (SGA)' MsiDesc,
--	'job' Unit,@sgai_10 MsiData,null LastUpdate

--union
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	10 PeriodMonth,57 SeqNo,'D. Workshop Service Strength' MsiGroup,' No. of Regular Stalls ( Available stalls Exclude Washing Stall and EM Stalls)' MsiDesc,
--	'stall' Unit,@stl1_10 MsiData,null LastUpdate

--union
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	10 PeriodMonth,57 SeqNo,'D. Workshop Service Strength' MsiGroup,'No. of Express Maintenance Stalls (Installed)' MsiDesc,
--	'stall' Unit,@stl2_10 MsiData,null LastUpdate

----
--union 
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	11 PeriodMonth,14 SeqNo,'A. Sales Revenue' MsiGroup,' aTotal Suzuki Genuine Accessories Sales Revenue - Chargeable to Customer CPUS (External) (15+16)' MsiDesc,
--	'Rp' Unit,(isnull(@sgaa_11,0)+isnull(@sgam_11,0)) MsiData,null LastUpdate

--union 
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	11 PeriodMonth,14 SeqNo,'A. Sales Revenue' MsiGroup,' SGA Accessories Parts Sales' MsiDesc,
--	'Rp' Unit,@sgaa_11 MsiData,null LastUpdate

--union 
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	11 PeriodMonth,14 SeqNo,'A. Sales Revenue' MsiGroup,' SGA Materials Sales' MsiDesc,
--	'Rp' Unit,@sgam_11 MsiData,null LastUpdate

--union
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	11 PeriodMonth,43 SeqNo,'C. No of Job Type' MsiGroup,'Accessories Installment (SGA)' MsiDesc,
--	'job' Unit,@sgai_11 MsiData,null LastUpdate

--union
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	11 PeriodMonth,57 SeqNo,'D. Workshop Service Strength' MsiGroup,' No. of Regular Stalls ( Available stalls Exclude Washing Stall and EM Stalls)' MsiDesc,
--	'stall' Unit,@stl1_11 MsiData,null LastUpdate

--union
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	11 PeriodMonth,57 SeqNo,'D. Workshop Service Strength' MsiGroup,'No. of Express Maintenance Stalls (Installed)' MsiDesc,
--	'stall' Unit,@stl2_11 MsiData,null LastUpdate

----
--union 
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	12 PeriodMonth,14 SeqNo,'A. Sales Revenue' MsiGroup,' aTotal Suzuki Genuine Accessories Sales Revenue - Chargeable to Customer CPUS (External) (15+16)' MsiDesc,
--	'Rp' Unit,(isnull(@sgaa_12,0)+isnull(@sgam_12,0)) MsiData,null LastUpdate

--union 
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	12 PeriodMonth,14 SeqNo,'A. Sales Revenue' MsiGroup,' SGA Accessories Parts Sales' MsiDesc,
--	'Rp' Unit,@sgaa_12 MsiData,null LastUpdate

--union 
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	12 PeriodMonth,14 SeqNo,'A. Sales Revenue' MsiGroup,' SGA Materials Sales' MsiDesc,
--	'Rp' Unit,@sgam_12 MsiData,null LastUpdate

--union
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	12 PeriodMonth,43 SeqNo,'C. No of Job Type' MsiGroup,'Accessories Installment (SGA)' MsiDesc,
--	'job' Unit,@sgai_12 MsiData,null LastUpdate

--union
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	12 PeriodMonth,57 SeqNo,'D. Workshop Service Strength' MsiGroup,' No. of Regular Stalls ( Available stalls Exclude Washing Stall and EM Stalls)' MsiDesc,
--	'stall' Unit,@stl1_12 MsiData,null LastUpdate

--union
--select 
--	@CompanyCode1 CompanyCode,@BranchCode1 BranchCode,@PeriodYear PeriodYear,
--	12 PeriodMonth,57 SeqNo,'D. Workshop Service Strength' MsiGroup,'No. of Express Maintenance Stalls (Installed)' MsiDesc,
--	'stall' Unit,@stl2_12 MsiData,null LastUpdate

)
--,
--svHstSzkMSISetSeqNoNew as (
--	select 	* 
--		, case 
--			when MsiDesc=' aTotal Suzuki Genuine Accessories Sales Revenue - Chargeable to Customer CPUS (External) (15+16)' then ' Total Suzuki Genuine Accessories Sales Revenue - Chargeable to Customer CPUS (External) (15+16)' 
--			when MsiDesc='  Labour Sales - Non-chargeable (Warranty, FS1 + FS2 + FS3 + FS4 + FS5 + FS6 + FS7)' then '     Labour Sales - Non-chargeable (Warranty, FS 1K, FS 5K, FS 10K, FS 20K, FS 30K, FS 40K, & FS 50K)' 
--			when MsiDesc='  Parts Sales - Non-chargeable (Warranty, FS1)' then '     Parts Sales - Non-chargeable (Warranty)' 
--			when MsiDesc='  Lubricant Sales - Chargeable to customer CPUS' then '     Lubricant Sales - Chargeable to customer CPUS (External)' 
--			when MsiDesc='  Lubricant Sales - Non-chargeable (Warranty, FS1)' then '     Lubricant Sales - Non-chargeable (Warranty, FS 1K - commercial car)' 
--			when MsiDesc='Total Sublet Sales Revenue' then 'Total Sublet Sales Revenue - Chargebale to Customer CPUS (External)' 
--			when MsiDesc='Total Service Sales Revenue (16 + 17 + 18)' then 'Total Service Sales Revenue (17 + 18 + 19)' 
--			when MsiDesc='  Total Service Sales - CPUS (External) (2 + 6 + 11 + 14)' then '     Total Service Sales - Chargeable to customer CPUS (External) (2 + 6 + 11 + 14 + 17)' 
--			when MsiDesc='  Total Service Sales - (Warranty, FS1) (3 +  7 + 12)' then '     Total Service Sales - Non-chargeable (Warranty) (3 +  7 + 12)' 
--			when MsiDesc='  Total Service Sales - (Internal, PDI) (4 + 8 + 13) ' then '     Total Service Sales - Non-chargeable (Internal, PDI) (4 + 8 + 13)' 
--			when MsiDesc='Hours Sold / Available hours (19 / 20) ' then 'Hours Sold / Available hours (20 / 21)' 
--			when MsiDesc='Service Revenue per Unit exclude PDI (CPUS, Warranty, FS1) ((16 + 17) / 29)' then 'Service Revenue per Unit exclude PDI (CPUS, Warranty, FS) ((19 + 20) / 32)' 
--			when MsiDesc='Labour Sales Turnover / Productive staff (Technician & Foreman)  (1 / 61)' then 'Labour  Sales Turnover / Productive staff (Technician & Foreman)  (1 / 67) ' 
--			when MsiDesc='Labour Sales Turnover / Service Advisors (1 / 60)' then 'Labour Sales Turnover / Service Advisors (1 / 66) ' 
--			when MsiDesc='Labour Sales Turnover / Stall (1 / 56)' then 'Labour Sales Turnover / Stall (1 / 58)' 
--			when MsiDesc='No. of Work Order / vehicle Intake exclude PDI (30 + 31)' then 'No. of Unit/vehicle Intakes exclude PDI (33 + 34)' 
--			when MsiDesc='  Passenger Car (exclude Angkot, Pickup, Chassis, Box)' then '     Passenger Car (exclude Angkot, Pickup, chassis)' 
--			when MsiDesc='  Commercial Vehicle (Angkot, Pickup, Chassis, Box)' then '     Commercial Vehicle (Angkot, Pickup, Chassis)' 
--			when MsiDesc='Customer Paid Unit Service exclude Free Service (34 + 38 + 42 + 43)' then 'Total Customer Paid Unit Service exclude Free Sevice (37 + 41 + 45 + 46 + 47)' 
--			when MsiDesc='  Periodical Maintenance under Warranty Period (= 100,000 km / = 3 Years) (sum of 35 to 37)' then '     Periodical Maintenance under Warranty Period (≤ 100,000 km/≤ 3 Years) (sum of 38 to 40)' --ddd
--			when MsiDesc='    10,000*; 30,000; 50,000; 70,000; & 90,000 km' then '         10,000*; 30,000*; 50,000*; 70,000; & 90,000 km' 
--			when MsiDesc='  Periodical Maintenance out of Warranty Period (> 100,000 km / >3 years) (sum of 39 to 41)' then '     Periodical Maintenance out of Warranty Period (>100,000 km/>3 years) (sum of 42 to 44)' 
--			when MsiDesc='  G/R Non Periodical Maintenance and Others' then '     G/R Non Periodical Maintenance and Others exclude Accessories Installment' 
--			when MsiDesc='No. of Free Service (sum of 45 to 51)' then 'No. of Free Service (FS 1K, FS 5K, FS 10K, FS 20K, FS 30K, FS 40K, & FS 50K) (sum of 49 to 55)' 
--			when MsiDesc='  FS  1,000 km' then '     FS 1K (1,000 km)' 
--			when MsiDesc='  FS  5,000 km' then '     FS 5K (5,000 km)' 
--			when MsiDesc='  FS 10,000 km' then '     FS 10K (10,000 km)' 
--			when MsiDesc='  FS 20,000 km' then '     FS 20K (20,000 km)' 
--			when MsiDesc='  FS 30,000 km' then '     FS 30K (30,000 km)' 
--			when MsiDesc='  FS 40,000 km' then '     FS 40K (40,000 km)' 
--			when MsiDesc='  FS 50,000 km' then '     FS 50K (50,000 km)' 
--			when MsiDesc='Non-chargeable Repair (Warranty, Repeated Job, PDI) (Sum of 53 to 54)' then 'Non-chargeable Repair (Warranty, Repeated Job, PDI) (Sum of 57 to 59)' 
--			when MsiDesc='No. of Working Stalls (Available stalls Exclude Washing Stall)' then 'No. of Available Stalls (59 + 60)' 
--			when MsiDesc='Total No. of Staff (58 + 59 + 60 + 61 + 64 + 65)' then 'Total No. of Staff (64 + 65 + 66 + 67 + 70 + 71)' 
--			when MsiDesc='  No. of Service Advisors' then '     No. of Service Advisors (SA)' 
--			when MsiDesc='  No. of Productive Staff (62 + 63)' then '     No. of Productive Staff (68 + 69)' 
--			when MsiDesc='Repair Unit per Productive staff (29 / 61)' then 'Repair Unit per Productive staff (32 / 67)' 
--			when MsiDesc='Repair Unit per Technician (29 / 63)' then 'Repair Unit per Technician (32 / 69)' 
--			when MsiDesc='Repair Unit per Working Stall (29 / 56) ' then 'Repair Unit per Available Stall (32 / 60)' 
--			when MsiDesc='Repair Unit per Working Stall (29 / 56) ' then 'No. of Customer / Service Advisor (32 / 66)' 
--			when MsiDesc='Repair Unit per Productive staff / day (66 / 70)' then 'Repair Unit per Productive staff / day (72 / 76)' 
--			when MsiDesc='Repair Unit per Technician / day (67 / 70) ' then 'Repair Unit per Technician / day (73 / 76)' 
--			when MsiDesc='Repair Unit per Working Stall / day (68 / 70)' then 'Repair Unit per Available Stall/day (74 / 76)' 
--			when MsiDesc='No. of Customer / Service Advisor / day (69 / 70)' then 'No. of Customer / Service Advisor / day (75 / 76)' 
--			when MsiDesc='No. of work order per vehicle intake from Reminder' then 'No. of work orders/vehicle intake from Reminder' 
--			when MsiDesc='Service Follow-ups' then 'No. of post service follow-ups' 
--			when MsiDesc='% Service Reminder unit intake to Reminder Target (76 / 75) ' then '% Service Reminder unit intake to Reminder Target (83 / 81) ' 
--			when MsiDesc='% Service Follow-ups to repair unit (work order) (78 / 29) ' then '% Post Service Follow-ups to repair unit (work order) (84 / 32)' 
--			when MsiDesc='% Service Bookings to repair unit (work order) (79 / 29) ' then '% Service Bookings to repair unit (work order) (85 / 32)' 
--			when MsiDesc='Total instant feedback form received ratio (to work order) (84 / 29)' then 'Total instant feedback form received ratio (to work order) (90 / 32)' 
--			when MsiDesc='Total Complaints Received (87 + 88)' then 'Total Complaints Received  (93 + 94)' 
--			when MsiDesc='Total No. Follow up ''Satisfied''' then 'Total No. of post service follow up ''Satisfied''' 
--			when MsiDesc='Total No. Follow up ''Not-Satisfied''' then 'Total No. of post service follow up ''Not-Satisfied''' 
--			when MsiDesc='Follow up ''Satisfied''/Intake Ratio (89 / 29)' then 'Ratio of post service follow ups ''Satisfied'' per vehicle intake (95 / 32)' 
--			when MsiDesc='Follow up '' Not Satisfied''/Intake Ratio (90 / 29)' then 'Ratio of post service follow ups ''Not Satisfied'' per vehicle intake (96 / 32)' 
--			when MsiDesc='Total Complaints & Follow up ''Not Satisfied''/Intake Ratio ((86 + 90) / 29)' then 'Ratio of total complaints & post service follow ups ''Not Satisfied'' per vehicle intake ((92 + 96) / 32)' 			

--			when MsiDesc='  Labour Sales - Chargeable to customer CPUS (External)' then '     Labour Sales - Chargeable to customer CPUS (External)'
--			when MsiDesc='  Labour Sales - Non-chargeable (Internal, PDI)' then '     Labour Sales - Non-chargeable (Internal, PDI)'
--			when MsiDesc='  Parts Sales - Chargeable to customer CPUS (External)' then '     Parts Sales - Chargeable to customer CPUS (External)'
--			when MsiDesc='  Parts Sales - Non-chargeable (Internal, PDI)' then '     Parts Sales - Non-chargeable (Internal, PDI)'
--			when MsiDesc='  Lubricant Sales - Non-chargeable (Internal, PDI)' then '     Lubricant Sales - Non-chargeable (Internal, PDI)'
--			when MsiDesc='  PDI' then '     PDI'
--			when MsiDesc='    20,000*; 60,000; & 100,000 km' then '         20,000*; 60,000; & 100,000 km'
--			when MsiDesc='    40,000; & 80,000 km ' then '         40,000; & 80,000 km '
--			when MsiDesc='    +10,000; +30,000; +50,000; +70,000; & +90,000 km' then '         +10,000; +30,000; +50,000; +70,000; & +90,000 km'
--			when MsiDesc='    +20,000; +60,000; & +100,000 km' then '         +20,000; +60,000; & +100,000 km'
--			when MsiDesc='    +40,000; & +80,000 km' then '         +40,000; & +80,000 km'
--			when MsiDesc='  5,000 km multiplier & above' then '     5,000 km multiplier & above'
--			when MsiDesc='  No. of Warranty Repair' then '     No. of Warranty Repair'
--			when MsiDesc='  Repeat Job (Rework)' then '     Repeat Job (Rework)'
--			when MsiDesc='  No. of PDI intake' then '     No. of PDI intake'
--			when MsiDesc='  No. of Admin & Support Staff' then '     No. of Admin & Support Staff'
--			when MsiDesc='  No. of Service Relation Officer (SRO)' then '     No. of Service Relation Officer (SRO)'
--			when MsiDesc='    No. of Foreman' then '         No. of Foreman'
--			when MsiDesc='    No. of Technician' then '         No. of Technician'
--			when MsiDesc='  No. of PDI Staff' then '     No. of PDI Staff'
--			when MsiDesc='  No. of Part Staff' then '     No. of Part Staff'
--			when MsiDesc='  Direct/verbal complaints received' then '     Direct/verbal complaints received'
--			when MsiDesc='  Indirect/non-verbal complaints received' then '     Indirect/non-verbal complaints received'	


--			else MsiDesc end as MsiDescNew
--		,ROW_NUMBER() over (partition by periodMonth order by periodMonth) as SeqNoNew
--	from svHstSzkMSIUnion
--),
--svHstSzkMSIV2a as(
--	select 
--		CompanyCode,
--		BranchCode,
--		PeriodYear,
--		PeriodMonth,
--		SeqNoNew SeqNo,
--		MsiGroup,
--		MsiDescNew MsiDesc,
--		Unit,
--		MsiData,
--		lastUpdateDate
--	from svHstSzkMSISetSeqNoNew
	
--),
--svHstSzkMSIV2b as (
--	select 
--		CompanyCode,
--		BranchCode,
--		PeriodYear,
--		PeriodMonth,
--		SeqNo,
--		MsiGroup,
--		MsiDesc,
--		Unit,
--		MsiData,
--		lastUpdateDate,
--		case 
--			when (SeqNo=1 and MsiDesc=' Total Suzuki Genuine Accessories Sales Revenue - Chargeable to Customer CPUS (External) (15+16)') then 'not-available'
--			when (SeqNo=2 and MsiDesc=' SGA Accessories Parts Sales') then 'not-available'
--			when (SeqNo=3 and MsiDesc=' SGA Materials Sales') then 'not-available'
--			when (SeqNo=4 and MsiDesc='Accessories Installment (SGA)') then 'not-available'
--			when (SeqNo=5 and MsiDesc=' No. of Regular Stalls ( Available stalls Exclude Washing Stall and EM Stalls)') then 'not-available'
--			when (SeqNo=6 and MsiDesc='No. of Express Maintenance Stalls (Installed)') then 'not-available'
--			else 'available' end as checking

--	from svHstSzkMSIV2a
--),

--svHstSzkMSIV2 as (
--	select 
--		CompanyCode,
--		BranchCode,
--		PeriodYear,
--		PeriodMonth,
--		SeqNo,
--		MsiGroup,
--		MsiDesc,
--		Unit,
--		MsiData,
--		lastUpdateDate 
--	from svHstSzkMSIV2b
--	where checking='available'
--)  

--select * from svHstSzkMSIV2
insert into #tempTable1(CompanyCode,
		BranchCode,
		PeriodYear,
		PeriodMonth,
		SeqNo,
		MsiGroup,
		MsiDesc,
		Unit,
		MsiData,
		lastUpdateDate )
	--values
	--select * from svHstSzkMSIV2
	select* from svHstSzkMSIUnion

end
-- end write all data / branch and insert to #tempTable1

if(@Area ='')
	begin
		-- strat write pivote all data
		select top 1 CompanyCode,BranchCode from #tempTable1

		select  PeriodYear, SeqNo, MsiGroup, MsiDesc, Unit
				 , (isnull( [1], 0) + isnull( [2], 0) + isnull( [3], 0) 
				 +  isnull( [4], 0) + isnull( [5], 0) + isnull( [6], 0) 
				 +  isnull( [7], 0) + isnull( [8], 0) + isnull( [9], 0)
				 +  isnull([10], 0) + isnull([11], 0) + isnull([12], 0)) / month(getdate()) as Average
				 , (isnull( [1], 0) + isnull( [2], 0) + isnull( [3], 0) 
				 +  isnull( [4], 0) + isnull( [5], 0) + isnull( [6], 0) 
				 +  isnull( [7], 0) + isnull( [8], 0) + isnull( [9], 0)
				 +  isnull([10], 0) + isnull([11], 0) + isnull([12], 0)) as Total
				 , isnull( [1], 0) [Month01]
				 , isnull( [2], 0) [Month02]
				 , isnull( [3], 0) [Month03]
				 , isnull( [4], 0) [Month04]
				 , isnull( [5], 0) [Month05]
				 , isnull( [6], 0) [Month06]
				 , isnull( [7], 0) [Month07]
				 , isnull( [8], 0) [Month08]
				 , isnull( [9], 0) [Month09]
				 , isnull([10], 0) [Month10]
				 , isnull([11], 0) [Month11]
				 , isnull([12], 0) [Month12]
			  from (
				select  PeriodYear, PeriodMonth, SeqNo, MsiGroup, MsiDesc, Unit, MsiData
				  from #tempTable1
				 --where CompanyCode in (select CompanyCode from gnMstOrganizationHdr where CompanyCode in (select DealerCode from GnMstDealerMapping where Area=@Area))
				 where CompanyCode in (select distinct CompanyCode from #tempTable1)
				   and BranchCode  in (select distinct BranchCode from #tempTable1)
				   and PeriodYear like case when @PeriodYear = '' then '%%' else @PeriodYear end
			  )#
			 pivot (sum(MsiData) for PeriodMonth in ([1], [2], [3], [4], [5], [6], [7], [8], [9], [10], [11], [12])) as pvt
			 order by pvt.SeqNo

		-- end write pivote all data
	end
else
	begin


-- start write pivot and insert to #tempTable2 and select table #tempTable2
set @count = (select count(*) from (select 
							*
							,ROW_NUMBER() over (order by companycode) as rowNumber
						from (select 
								distinct CompanyCode, BranchCode
								from #tempTable1)a)b)
set @intFlag=1;

WHILE (@intFlag <=@count)
begin
	set  @CompanyCode= (select CompanyCode 
						from (select 
									*
									,ROW_NUMBER() over (order by companycode) as rowNumber
								from (select 
										distinct CompanyCode, BranchCode
										from #tempTable1)a)b
						where @intFlag=rowNumber)

	set  @BranchCode= (select BranchCode 
						from (select 
									*
									,ROW_NUMBER() over (order by companycode) as rowNumber
								from (select 
										distinct CompanyCode, BranchCode
										from #tempTable1)a)b
						where @intFlag=rowNumber)

	SET @intFlag = @intFlag + 1

	insert into #tempTable2(CompanyCode,
		BranchCode,
		PeriodYear,
		SeqNo,
		MsiGroup,
		MsiDesc,
		Unit,
		Average,
		Total,
		Month01,
		Month02,
		Month03,
		Month04,
		Month05,
		Month06,
		Month07,
		Month08,
		Month09,
		Month10,
		Month11,
		Month12 )
	--values
	select CompanyCode, BranchCode, PeriodYear, SeqNo, MsiGroup, MsiDesc, Unit
		 , (isnull( [1], 0) + isnull( [2], 0) + isnull( [3], 0) 
		 +  isnull( [4], 0) + isnull( [5], 0) + isnull( [6], 0) 
		 +  isnull( [7], 0) + isnull( [8], 0) + isnull( [9], 0)
		 --+  isnull([10], 0) + isnull([11], 0) + isnull([12], 0)) / month(getdate()) as Average
		 +  isnull([10], 0) + isnull([11], 0) + isnull([12], 0)) / (@Month2 - @Month1 + 1) as Average
		 , (isnull( [1], 0) + isnull( [2], 0) + isnull( [3], 0) 
		 +  isnull( [4], 0) + isnull( [5], 0) + isnull( [6], 0) 
		 +  isnull( [7], 0) + isnull( [8], 0) + isnull( [9], 0)
		 +  isnull([10], 0) + isnull([11], 0) + isnull([12], 0)) as Total
		 , isnull( [1], 0) [Month01]
		 , isnull( [2], 0) [Month02]
		 , isnull( [3], 0) [Month03]
		 , isnull( [4], 0) [Month04]
		 , isnull( [5], 0) [Month05]
		 , isnull( [6], 0) [Month06]
		 , isnull( [7], 0) [Month07]
		 , isnull( [8], 0) [Month08]
		 , isnull( [9], 0) [Month09]
		 , isnull([10], 0) [Month10]
		 , isnull([11], 0) [Month11]
		 , isnull([12], 0) [Month12]
	  from (
		select CompanyCode, BranchCode, PeriodYear, PeriodMonth, SeqNo, MsiGroup, MsiDesc, Unit, MsiData
		  from #tempTable1
		 where CompanyCode in (select CompanyCode from gnMstOrganizationHdr where CompanyCode in (select DealerCode from GnMstDealerMapping where Area=@Area))
		   and BranchCode like case when @BranchCode = '' then '%%' else @BranchCode end
		   and PeriodYear like case when @PeriodYear = '' then '%%' else @PeriodYear end
	  )#
	 pivot (sum(MsiData) for PeriodMonth in ([1], [2], [3], [4], [5], [6], [7], [8], [9], [10], [11], [12])) as pvt
	 order by pvt.CompanyCode,pvt.BranchCode,pvt.SeqNo

end

--select * from #tempTable2 order by CompanyCode,BranchCode,SeqNo
select 
		t2.CompanyCode,		
		t2.BranchCode,		
		t2.PeriodYear,
		t2.SeqNo,
		t2.MsiGroup,
		t2.MsiDesc,
		t2.Unit,
		t2.Average,
		t2.Total,
		t2.Month01,
		t2.Month02,
		t2.Month03,
		t2.Month04,
		t2.Month05,
		t2.Month06,
		t2.Month07,
		t2.Month08,
		t2.Month09,
		t2.Month10,
		t2.Month11,
		t2.Month12,
		goh.CompanyName,
		gmc.CompanyName as BranchName
from #tempTable2 t2
inner join gnMstOrganizationHdr goh on goh.CompanyCode = t2.CompanyCode
inner join gnMstCoProfile gmc on gmc.BranchCode = t2.BranchCode
order by t2.CompanyCode,t2.BranchCode,t2.SeqNo

-- end write pivot and insert to #tempTable2 and select table #tempTable2

-- strat write pivote all data
select  PeriodYear, SeqNo, MsiGroup, MsiDesc, Unit
		 , (isnull( [1], 0) + isnull( [2], 0) + isnull( [3], 0) 
		 +  isnull( [4], 0) + isnull( [5], 0) + isnull( [6], 0) 
		 +  isnull( [7], 0) + isnull( [8], 0) + isnull( [9], 0)
		 --+  isnull([10], 0) + isnull([11], 0) + isnull([12], 0)) / month(getdate()) as Average
		 +  isnull([10], 0) + isnull([11], 0) + isnull([12], 0)) / (@Month2 - @Month1 + 1) as Average
		 , (isnull( [1], 0) + isnull( [2], 0) + isnull( [3], 0) 
		 +  isnull( [4], 0) + isnull( [5], 0) + isnull( [6], 0) 
		 +  isnull( [7], 0) + isnull( [8], 0) + isnull( [9], 0)
		 +  isnull([10], 0) + isnull([11], 0) + isnull([12], 0)) as Total
		 , isnull( [1], 0) [Month01]
		 , isnull( [2], 0) [Month02]
		 , isnull( [3], 0) [Month03]
		 , isnull( [4], 0) [Month04]
		 , isnull( [5], 0) [Month05]
		 , isnull( [6], 0) [Month06]
		 , isnull( [7], 0) [Month07]
		 , isnull( [8], 0) [Month08]
		 , isnull( [9], 0) [Month09]
		 , isnull([10], 0) [Month10]
		 , isnull([11], 0) [Month11]
		 , isnull([12], 0) [Month12]
	  from (
		select  PeriodYear, PeriodMonth, SeqNo, MsiGroup, MsiDesc, Unit, MsiData
		  from #tempTable1
		 --where CompanyCode in (select CompanyCode from gnMstOrganizationHdr where CompanyCode in (select DealerCode from GnMstDealerMapping where Area=@Area))
		 where CompanyCode in (select distinct CompanyCode from #tempTable1)
		   and BranchCode  in (select distinct BranchCode from #tempTable1)
		   and PeriodYear like case when @PeriodYear = '' then '%%' else @PeriodYear end
	  )#
	 pivot (sum(MsiData) for PeriodMonth in ([1], [2], [3], [4], [5], [6], [7], [8], [9], [10], [11], [12])) as pvt
	 order by pvt.SeqNo

-- end write pivote all data
end
-- start drop temp table
drop table #tempTable1
drop table #tempTable2
drop table #tempTable3

-- end drop temp table

END




