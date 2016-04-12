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
	
var TaskName = "UPDATE SP usprtp_SvRpTrn008";
var TaskNo = "20141205002";
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
declare @iFlag int
select @iFlag=case when right(paramvalue,3)='.31' then 1 else 0 end 
from sysparameter where paramid='SDMS_VERSION'

IF @iFlag=1
    SET NOEXEC OFF --Enables execution of code (Default)
ELSE 
    SET NOEXEC ON --Disables execution of code
GO

ALTER procedure [dbo].[usprpt_SvRpTrn008]
 @CompanyCode varchar(20),       
 @BranchCode varchar(20),      
 @FpjGovNoFrom varchar(20),
 @FpjGovNoTo varchar(20),
 @FpjType int,
 @Potongan bit,
 @StartDate varchar(20),
 @EndDate varchar(20)

as

--usprpt_SvRpTrn008 '6079401','607940100','14.84587994','14.84589289',8,False,'20140101','20140228'

--declare @CompanyCode varchar(20)       
--declare	@BranchCode varchar(20)      
--declare @FpjGovNoFrom varchar(20)
--declare @FpjGovNoTo varchar(20)
--declare @FpjType int
--declare @Potongan bit
--declare @StartDate varchar(20)
--declare @EndDate varchar(20)

--set @CompanyCode = '6079401'
--set	@BranchCode = '607940100'
--set @FpjGovNoFrom = '14.84587994'
--set @FpjGovNoTo= '14.84589289'
--set @FpjType = 8
--set @Potongan = 0
--set @StartDate = '20140101'
--set @EndDate = '20140228'  

-- Setting Header Faktur Pajak --
---------------------------------
declare @fCompName	varchar(max)
declare @fAdd		varchar(max)
declare @fAdd1		varchar(max)
declare @fAdd2		varchar(max)
declare @fNPWP		varchar(max)
declare @fSKP		varchar(max)
declare @fSKPDate	varchar(max)
declare @fCity		varchar(max)

declare @fStatus varchar(1)
set @fStatus = 0

declare @fInfoPKP varchar(1)
set @fInfoPKP = 1

if exists (select 1 from gnMstLookUpDtl where codeid='FPJFLAG')
begin
	set @fStatus = (select paravalue from gnmstlookupdtl where codeid='FPJFLAG' and lookupvalue='STATUS')
end

if exists (select * from gnMstLookUpHdr where codeid='FPJ_INFO_PKP')
	begin
		set @fInfoPKP = (select LookupValue from gnmstlookupdtl where codeid='FPJ_INFO_PKP')
	end
	
if (@fStatus = '1')
begin
	set @fCompName	= (select isnull(lookupvaluename,'') from gnmstlookupdtl where codeid='FPJINFO' and lookupvalue='NAME')
	set @fAdd1		= (select isnull(lookupvaluename,'') from gnmstlookupdtl where codeid='FPJINFO' and lookupvalue='ADD1')
	set @fAdd2		= (select isnull(lookupvaluename,'') from gnmstlookupdtl where codeid='FPJINFO' and lookupvalue='ADD2')
	set @fAdd		= @fAdd1+' '+@fAdd2
	set @fNPWP		= (select isnull(lookupvaluename,'') from gnmstlookupdtl where codeid='FPJINFO' and lookupvalue='NPWP')
	set @fSKP		= (select isnull(lookupvaluename,'') from gnmstlookupdtl where codeid='FPJINFO' and lookupvalue='SKPNO')
	set @fSKPDate	= (select isnull(lookupvaluename,'') from gnmstlookupdtl where codeid='FPJINFO' and lookupvalue='SKPDATE')
	set @fCity		= (select isnull(lookupvaluename,'') from gnmstlookupdtl where codeid='FPJINFO' and lookupvalue='CITY')		
end

declare @IsHoldingAddr as bit
if (select count(*) from gnMstLookUpDtl where CompanyCode= @CompanyCode and CodeID='FPJADDR') > 0
	set @IsHoldingAddr = (select convert(numeric,LookUpValue) from gnMstLookUpDtl where CompanyCode= @CompanyCode and CodeID='FPJADDR')
else
	set @IsHoldingAddr = 0

declare @ReportParam as varchar(2)
declare @IsCentral as bit
declare @IsRecalPPN as bit

declare @StatusEnd  as int
declare @CountDtl as int

declare @FlagAsterix as bit
set @FlagAsterix = isnull((select ParaValue from GnMstLookUpDtl where CompanyCode = @CompanyCode and CodeID = 'XFAK' and LookUpValue = 'SHOW_SV'), 0)

declare @ShowFpjDisc as bit
set @ShowFpjDisc = isnull((select ParaValue from GnMstLookUpDtl where CompanyCode = @CompanyCode and CodeID = 'SRV_FLAG' and LookUpValue = 'SHOW_FPJDISC'), 0)

-- check seberapa banyak FPJGovNo yang akan di display
create table #t_fpj(
	rownum int,
	FPjGovNo varchar(20)
)

-- 0= Without  newline, 1= With newline at detail
set @ReportParam='0'
if (select count(LookupValue) from gnMstLookUpDtl where CodeID='RPSV') > 0
	set @ReportParam= (select LookUpValue from gnMstLookUpDtl where CodeID='RPSV')


-- 0= Without BranchCode, 1= With BranchCode
set @IsCentral=0
if ((select count(LookupValue) from gnMstLookUpDtl where CodeID='FPGB') > 0 and (@FpjType = '3' or @FpjType = '8')
	and (select IsBranch from gnMstOrganizationDtl where CompanyCode=@CompanyCode and BranchCode= @BranchCode)=0)
begin
	if (select LookUpValue from gnMstLookUpDtl where CodeID='FPGB') = '1'
		set @IsCentral= 1
	else
		set @IsCentral= 0
end

-- 0= Not Recalculate PPN, 1= Recalculate PPN (For FPJ Gabungan)
set @IsRecalPPN=0
if (select count(ParaValue) from gnMstLookUpDtl where CodeID='CPPN' and LookUpValue='STATUS') > 0
	set @IsRecalPPN= (select convert(bit,ParaValue) from gnMstLookUpDtl where CodeID='CPPN' and LookUpValue='STATUS')

-- get FPJGovNo from transaction
if(len(@FpjGovNoFrom) > 8 and len(@FpjGovNoTo) > 8)
	if (@FpjType = '3' or @FpjType = '8')
		insert into #t_fpj
		select (row_number() over (order by FpjGovNo)) as rownum, FpjGovNo from svTrnFakturPajak
		 where right(FpjGovNo,len(FPJGovNo) - 8) between @FpjGovNoFrom and @FpjGovNoTo
		   and convert(varchar,FpjDate,112) between @StartDate and @EndDate
		   and CompanyCode = @CompanyCode
		   and BranchCode = @BranchCode
		   and IsPKP = 1 and FPJGovNo <> ''
		   and NoOfInvoice > 1		
	else
		insert into #t_fpj
		select (row_number() over (order by FpjGovNo)) as rownum, FpjGovNo from svTrnFakturPajak
		 where right(FpjGovNo,len(FPJGovNo) - 8) between @FpjGovNoFrom and @FpjGovNoTo
		   and convert(varchar,FpjDate,112) between @StartDate and @EndDate
		   and CompanyCode = @CompanyCode
		   and BranchCode = @BranchCode
		   and IsPKP = 1 and FPJGovNo <> ''
		   and NoOfInvoice = 1		
else
	if (@FpjType = '3' or @FpjType = '8')
		insert into #t_fpj
		select (row_number() over (order by FpjGovNo)) as rownum, FpjGovNo from svTrnFakturPajak
		 where convert(varchar, FpjDate, 112) between @StartDate and @StartDate
		   and CompanyCode = @CompanyCode
		   and BranchCode = @BranchCode
		   and IsPKP = 1 and FPJGovNo <> ''
		   and NoOfInvoice > 1		
	else
		insert into #t_fpj
		select (row_number() over (order by FpjGovNo)) as rownum, FpjGovNo from svTrnFakturPajak
		 where convert(varchar, FpjDate, 112) between @StartDate and @StartDate
		   and CompanyCode = @CompanyCode
		   and BranchCode = @BranchCode
		   and IsPKP = 1 and FPJGovNo <> ''
		   and NoOfInvoice = 1
declare @n int 
set @n = 0

-- start looping
declare @FpjGovNo varchar(20)
while (@n < (select max(rownum) from #t_fpj))
begin
	set @n = @n + 1;
	set @FpjGovNo = (select FpjGovNo from #t_fpj where rownum = @n)

	-- get FPJGovNo
	select * into #fpj from (
	select CompanyCode, BranchCode, FPjGovNo, FpjNo, FpjDate, SignedDate, DueDate, NoOfInvoice
	  from svTrnFakturPajak
	 where 1 = 1
	   and CompanyCode = @CompanyCode
	   and BranchCode = @BranchCode
	   and FpjGovNo = @FpjGovNo
	) #fpj

	-- get invoice data from transaction	
	select * into #inv from (
	select CompanyCode
		  ,BranchCode
		  ,InvoiceNo
		  ,JobOrderNo
		  ,CustomerCodeBill
		  ,case when (@FpjType = '3' OR @FPJType = '8') then '' else PoliceRegNo end PoliceRegNo 
		  ,isnull(LaborGrossAmt,0) LaborGrossAmt
		  ,isnull(PartsGrossAmt,0) PartsGrossAmt
		  ,isnull(MaterialGrossAmt,0) MaterialGrossAmt
		  ,isnull(LaborDiscAmt,0) LaborDiscAmt
		  ,isnull(PartsDiscAmt,0) PartsDiscAmt
		  ,isnull(MaterialDiscAmt,0) MaterialDiscAmt
		  ,isnull(TotalDppAmt,0) TotalDppAmt
		  ,isnull(TotalPphAmt,0) TotalPphAmt
		  ,isnull(TotalPpnAmt,0) TotalPpnAmt
		  ,isnull(TotalSrvAmt,0) TotalSrvAmt
      from svTrnInvoice a
	 where 1 = 1
	   and CompanyCode = @CompanyCode
	   and ((case when @IsCentral = 1 then BranchCode end) <> ''
			or (case when @IsCentral = 0 then BranchCode end) = @BranchCode)
	   --and BranchCode = @BranchCode
	   and isLocked=(case when (select top 1 NoOfInvoice from #fpj) > 1 then @IsCentral else isLocked end)
	   and FpjNo in (select FpjNo from #fpj) --b.FpjNo
	) #inv	
	
	-- get Data SPK
	select * into #srv from (
	select CompanyCode, BranchCode, ProductType, JobOrderNo, BasicModel, JobType from svTrnService
	 where 1 = 1
	   and CompanyCode = @CompanyCode
	   and ((case when @IsCentral = 1 then BranchCode end) <> ''
			or (case when @IsCentral = 0 then BranchCode end) = @BranchCode)
	   and BranchCode+''+JobOrderNo in (select BranchCode+''+JobOrderNo from #inv)
	) #srv

	declare @dtl as table(DetailDesc varchar(300))
	declare @temp as table(FPJGovNo varchar(35), CodeID varchar(3),intCount int)

	select * into #tsk from (
	select
		 row_number() over (order by a.OperationNo) RecNo
		,a.*, (a.OperationHour * a.OperationCost) as OperationAmt
		,b.JobOrderNo, @FpjGovNo FpjGovNo,b.LaborDiscAmt,b.PartsDiscAmt,b.MaterialDiscAmt
	  from svTrnInvTask a, #inv b
	 where 1 = 1
	   and a.CompanyCode = @CompanyCode
	   and a.BranchCode = b.BranchCode
	   and a.InvoiceNo = b.InvoiceNo
	   and a.InvoiceNo = b.InvoiceNo
	   and a.IsSubCon= (case when a.InvoiceNo like 'INW%' then isnull(a.IsSubCon, 0) else 0 end)
	)#tsk

	select * into #itm from (
	select row_number() over (order by a.PartNo) RecNo
	     , a.PartNo, (a.SupplyQty - a.ReturnQty) as SupplyQty
	     , a.RetailPrice
	     --, (a.RetailPrice * (a.SupplyQty - a.ReturnQty)) RetailAmt
	     , ROUND((a.RetailPrice * (a.SupplyQty - a.ReturnQty)),0) RetailAmt --Perubahan
	     , a.DiscPct, a.TypeOfGoods
	  from svTrnInvItem a, #inv b
	 where 1 = 1
	   and a.CompanyCode = @CompanyCode
	   and a.BranchCode = b.BranchCode
	   and a.InvoiceNo = b.InvoiceNo
	)#itm

	select * into #oth from (
	select
		 row_number() over (order by a.OperationNo) RecNo
		,a.*, (a.OperationHour * a.OperationCost) as OperationAmt
		,b.JobOrderNo, @FpjGovNo FpjGovNo
	  from svTrnInvTask a, #inv b
	 where 1 = 1
	   and a.CompanyCode = @CompanyCode
	   and a.BranchCode = b.BranchCode
	   and a.InvoiceNo = b.InvoiceNo
	   and a.InvoiceNo = b.InvoiceNo
	   and isnull(a.IsSubCon, 0) = 1
	)#oth

----------------------------------------------------------------------------	
	--Type of Goods SPAREPART
	select * into #tpSp from (
	select LookUpValue from gnMstLookUpDtl where companyCode = @CompanyCode
		and CodeId = 'GTGO' and ParaValue = 'SPAREPART' )#tpSp

	--Type of Goods OLI
	select * into #tpOli from (
	select LookUpValue from gnMstLookUpDtl where companyCode = @CompanyCode
		and CodeId = 'GTGO' and ParaValue = 'OLI' )#tpOli

	--Type of Goods MATERIAL
	select * into #tpMt from (
	select LookUpValue from gnMstLookUpDtl where companyCode = @CompanyCode
		and CodeId = 'GTGO' and ParaValue = 'MATERIAL' )#tpMt

	--Type of Goods RINCI MATERIAL
	select * into #tpRMt from (
	select LookUpValue from gnMstLookUpDtl where companyCode = @CompanyCode
		and CodeId = 'GTGO' and ParaValue in ('MATERIAL','OLI') )#tpRMt
-----------------------------------------------------------------------------

	select * into #itmSP from (
	select
		 row_number() over (order by a.PartNo) RecNo
		,a.PartNo
		,(a.SupplyQty - a.ReturnQty) SupplyQty
		,a.RetailPrice
		,(a.RetailPrice * (a.SupplyQty - a.ReturnQty)) RetailAmt
		,a.TypeOfGoods
	  from svTrnInvItem a, #inv b
	 where 1 = 1
	   and a.CompanyCode = @CompanyCode
	   and a.BranchCode = b.BranchCode
	   and a.InvoiceNo = b.InvoiceNo
	   and a.TypeOfGoods in (select * from #tpSp)
	)#itmSP

	select * into #itmOli from (
	select
		 row_number() over (order by a.PartNo) RecNo
		,a.PartNo
		,(a.SupplyQty - a.ReturnQty) SupplyQty
		,a.RetailPrice
		,(a.RetailPrice * (a.SupplyQty - a.ReturnQty)) RetailAmt
		,a.TypeOfGoods
	  from svTrnInvItem a, #inv b
	 where 1 = 1
	   and a.CompanyCode = @CompanyCode
	   and a.BranchCode = b.BranchCode
	   and a.InvoiceNo = b.InvoiceNo
	   and a.TypeOfGoods in(select * from #tpOli)
	)#itmOli

	select * into #itmMtrl from (
	select
		 row_number() over (order by a.PartNo) RecNo
		,a.PartNo
		,(a.SupplyQty - a.ReturnQty) SupplyQty
		,a.RetailPrice
		,(a.RetailPrice * (a.SupplyQty - a.ReturnQty)) RetailAmt
		,a.TypeOfGoods
	  from svTrnInvItem a, #inv b
	 where 1 = 1
	   and a.CompanyCode = @CompanyCode
	   and a.BranchCode = b.BranchCode
	   and a.InvoiceNo = b.InvoiceNo
	   and a.TypeOfGoods in (select * from #tpMt)
	)#itmMtrl

	select * into #itmRMtrl from (
	select
		 row_number() over (order by a.PartNo) RecNo
		,a.PartNo
		,(a.SupplyQty - a.ReturnQty) SupplyQty
		,a.RetailPrice
		,(a.RetailPrice * (a.SupplyQty - a.ReturnQty)) RetailAmt
		,a.TypeOfGoods
	  from svTrnInvItem a, #inv b
	 where 1 = 1
	   and a.CompanyCode = @CompanyCode
	   and a.BranchCode = b.BranchCode
	   and a.InvoiceNo = b.InvoiceNo
	   and a.TypeOfGoods in (select * from #tpRMt)
	)#itmRMtrl

	declare @TaskAmt money
	declare @ItemAmt money
	declare @MaterialAmt money
	declare @SubletAmt money
	declare @SalesAmt int
	declare @DiscAmt money

	-- select condition for 'Standard'
	if (@FpjType = '1')
	begin
		-- Insert Jasa Perbaikan
		if ((select count(*) from #tsk) > 0)
		begin
			set @TaskAmt = isnull((select sum(OperationHour * OperationCost) from #tsk),0)
				if (@ReportParam = '1')
					insert into @dtl values (' ')
			insert into @dtl
			select left(('    JASA PERBAIKAN'
				+ replicate(' ', 100)
				), 85)
				+ right((
				+ replicate(' ',20)
				+ left(convert(varchar, @TaskAmt, 1),len(convert(varchar, @TaskAmt, 1)) - 3)
				),18)

			insert into @dtl
			select left((
				  right((replicate(' ',2)
				+ convert(varchar,a.RecNo)
				),2)
				+ '  '
				+ a.OperationNo
				+ ' - '
				+ isnull(c.Description, '')
				),80)
			  from #tsk a
		  left join #srv b
			on b.JobOrderNo = a.jobOrderNo
		  left join svMstTask c
			on c.CompanyCode = b.CompanyCode
		   and c.ProductType = b.ProductType
		   and c.BasicModel = b.BasicModel
		   and (c.JobType = b.JobType or c.JobType = 'CLAIM' or c.JobType = 'OTHER')
		   and c.OperationNo = a.OperationNo

			if(@Potongan = 1)
			begin
			insert into @dtl
			select left(('                                        '
				+ replicate(' ', 100)
				), 85)
				+ right((
				+ replicate('-',20)
				+ '-'
				),18)

			set @TaskAmt = isnull((select sum(OperationHour * OperationCost) from #tsk),0)
			set @DiscAmt = (select sum (OperationHour * OperationCost* DiscPct * 0.01) from #tsk)
			insert into @dtl
			select left(('                                        Potongan Jasa'
				+ replicate(' ', 100)
				), 85)
				+ right((
				+ replicate(' ',20)
				+ left(convert(varchar, @DiscAmt, 1),len(convert(varchar, @DiscAmt, 1)) - 3)
				),18)

			insert into @dtl
			select left(('                                        '
				+ replicate(' ', 100)
				), 85)
				+ right((
				+ replicate('-',20)
				+ '-'
				),18)

			insert into @dtl
			select left(('                                        '
				+ replicate(' ', 100)
				), 85)
				+ right((
				+ replicate(' ',20)
				+ left(convert(varchar, isnull(@TaskAmt,0) - isnull(@DiscAmt,0), 1),len(convert(varchar, isnull(@TaskAmt,0) - isnull(@DiscAmt,0), 1)) - 3)
				),18)
			end

			if (select count(*) from #itm) > 0
			begin
				insert into @dtl values ('')
			end

			-- add by Seandy 20-9-2011
			-- Untuk menentukan jenis coretan pada keterangan(ada atau tidaknya)
			insert into @temp 
				select @FpjGovNo, 'SRV', COUNT(*) intCount from #tsk a
			  left join #srv b
				on b.JobOrderNo = a.jobOrderNo
			  left join svMstTask c
				on c.CompanyCode = b.CompanyCode
			   and c.ProductType = b.ProductType
			   and c.BasicModel = b.BasicModel
			   and (c.JobType = b.JobType or c.JobType = 'CLAIM' or c.JobType = 'OTHER')
			   and c.OperationNo = a.OperationNo
 		end

		-- Insert Pemakaian Sparepart
		if ((select count(*) from #itm) > 0)
		begin
			insert into @dtl values ('    PEMAKAIAN SPAREPART/MATERIAL')
			insert into @dtl
			select left((
				  right((replicate(' ',2)
				+ convert(varchar,a.RecNo)
				),2)
				+ '  '
				+ a.PartNo
				+ ' - '
				+ b.PartName
				+ replicate(' ',100)
				),65)
				+ right((
				  replicate(' ',50)
				+ convert(varchar,a.SupplyQty)
				),10)
				+ right((
				  replicate(' ',50)
				+ left(cast(convert(varchar,cast(a.RetailAmt as money),1) as varchar),len(cast(convert(varchar,cast(a.RetailAmt as money),1) as varchar))-3)
				),28)
			  from #itm a
			  left join spMstItemInfo b
				on b.CompanyCode = @CompanyCode
			   and b.PartNo = a.PartNo

			if(@Potongan = 1)
			begin
			insert into @dtl
			select left(('                                        '
				+ replicate(' ', 100)
				), 85)
				+ right((
				+ replicate('-',20)
				+ '-'
				),18)

			set @ItemAmt = isnull((select sum(RetailAmt) from #itm),0)
			set @DiscAmt = isnull((select top 1 (PartsDiscAmt + MaterialDiscAmt) from #tsk),0)
			insert into @dtl
			select left(('                                        Potongan Part'
				+ replicate(' ', 100)
				), 85)
				+ right((
				+ replicate(' ',20)
				+ left(convert(varchar, @DiscAmt, 1),len(convert(varchar, @DiscAmt, 1)) - 3)
				),18)

			insert into @dtl
			select left(('                                        '
				+ replicate(' ', 100)
				), 85)
				+ right((
				+ replicate('-',20)
				+ '-'
				),18)

			insert into @dtl
			select left(('                                        '
				+ replicate(' ', 100)
				), 85)
				+ right((
				+ replicate(' ',20)
				+ left(convert(varchar, isnull(@ItemAmt,0) - isnull(@DiscAmt,0), 1),len(convert(varchar, isnull(@ItemAmt,0) - isnull(@DiscAmt,0), 1)) - 3)
				),18)
			end

			if (select count(*) from #oth) > 0
			begin
				insert into @dtl values ('')
			end

			-- add by Seandy 20-9-2011
			-- Untuk menentukan jenis coretan pada keterangan(ada atau tidaknya)
			insert into @temp
				select @FpjGovNo,'OIL',COUNT(*) intCount from #itm a
			  left join spMstItemInfo b
				on b.CompanyCode = @CompanyCode
			   and b.PartNo = a.PartNo
		end

		-- Insert Jasa SubCon (Lain - lain)
		if ((select count(*) from #oth) > 0)
		begin
			set @TaskAmt = isnull((select sum(OperationHour * OperationCost) from #oth),0)
			if (@ReportParam = '1')
				insert into @dtl values (' ')
			insert into @dtl
			select left(('    LAIN - LAIN'
				+ replicate(' ', 100)
				), 85)
				+ right((
				+ replicate(' ',20)
				+ left(convert(varchar, @TaskAmt, 1),len(convert(varchar, @TaskAmt, 1)) - 3)
				),18)

			insert into @dtl
			select left((
				  right((replicate(' ',2)
				+ convert(varchar,a.RecNo)
				),2)
				+ '  '
				+ a.OperationNo
				+ ' - '
				+ isnull(c.Description, '')
				),80)
			  from #oth a
		  left join #srv b
			on b.JobOrderNo = a.jobOrderNo
		  left join svMstTask c
			on c.CompanyCode = b.CompanyCode
		   and c.ProductType = b.ProductType
		   and c.BasicModel = b.BasicModel
		   and (c.JobType = b.JobType or c.JobType = 'CLAIM' or c.JobType = 'OTHER')
		   and c.OperationNo = a.OperationNo

			if(@Potongan = 1)
			begin
				insert into @dtl
				select left(('                                        '
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate('-',20)
					+ '-'
					),18)

				set @TaskAmt = isnull((select sum(OperationHour * OperationCost) from #oth),0)
				set @DiscAmt = (select sum (OperationHour * OperationCost* DiscPct * 0.01) from #oth)
				insert into @dtl
				select left(('                                  Potongan Lain-Lain'
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate(' ',20)
					+ left(convert(varchar, @DiscAmt, 1),len(convert(varchar, @DiscAmt, 1)) - 3)
					),18)

				insert into @dtl
				select left(('                                        '
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate('-',20)
					+ '-'
					),18)

				insert into @dtl
				select left(('                                        '
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate(' ',20)
					+ left(convert(varchar, isnull(@TaskAmt,0) - isnull(@DiscAmt,0), 1),len(convert(varchar, isnull(@TaskAmt,0) - isnull(@DiscAmt,0), 1)) - 3)
					),18)
			end

			-- add by Seandy 20-9-2011
			-- Untuk menentukan jenis coretan pada keterangan(ada atau tidaknya)
			insert into @temp
				select @FpjGovNo,'DLL',COUNT(*) intCount from #oth a
				  left join #srv b
					on b.JobOrderNo = a.jobOrderNo
				  left join svMstTask c
					on c.CompanyCode = b.CompanyCode
				   and c.ProductType = b.ProductType
				   and c.BasicModel = b.BasicModel
				   and (c.JobType = b.JobType or c.JobType = 'CLAIM' or c.JobType = 'OTHER')
				   and c.OperationNo = a.OperationNo
		end
	end

	-- select condition for 'Rinci'
	if (@FpjType = '2')
	begin
		-- Insert Jasa Perbaikan
		if ((select count(*) from #tsk) > 0)
		begin
			if (@ReportParam = '1')
				insert into @dtl values (' ')
			insert into @dtl values ('    JASA PERBAIKAN')
			insert into @dtl
			select left((
				  right((replicate(' ',2)
				+ convert(varchar,a.RecNo)
				),2)
				+ '  '
				+ a.OperationNo
				+ ' - '
				+ isnull(c.Description, '')
				+ replicate(' ',100)
				),75)
				
				+ case when (isnull(a.DiscPct, 0) = 0 or @ShowFpjDisc = 0) then replicate(' ', 10)
                      else right(replicate(' ', 10) + '(' + convert(varchar, cast(a.DiscPct as money), 1) + '%)', 10)
                      end 				
				
				+ right((replicate(' ',18)
				+ left(convert(varchar, cast(a.OperationAmt as money), 1), len(convert(varchar, cast(a.OperationAmt as money), 1)) - 3)
				),18)
			  from #tsk a
		  left join #srv b
			on b.JobOrderNo = a.jobOrderNo
		  left join svMstTask c
			on c.CompanyCode = b.CompanyCode
		   and c.ProductType = b.ProductType
		   and c.BasicModel = b.BasicModel
		   and (c.JobType = b.JobType or c.JobType = 'CLAIM' or c.JobType = 'OTHER')
		   and c.OperationNo = a.OperationNo

			if(@Potongan = 1)
			begin
				insert into @dtl
				select left(('                                        '
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate('-',20)
					+ '-'
					),18)

				set @TaskAmt = isnull((select sum(OperationHour * OperationCost) from #tsk),0)
				if ((select count(*) from #tsk) > 1)
				begin
				insert into @dtl
				select left(('                                            Sub Total'
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate(' ',20)
					+ left(convert(varchar, @TaskAmt, 1),len(convert(varchar, @TaskAmt, 1)) - 3)
					),18)
				end

				set @DiscAmt = (select sum (OperationHour * OperationCost* DiscPct * 0.01) from #tsk)
				insert into @dtl
				select left(('                                        Potongan Jasa'
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate(' ',20)
					+ left(convert(varchar, @DiscAmt, 1),len(convert(varchar, @DiscAmt, 1)) - 3)
					),18)

				insert into @dtl
				select left(('                                        '
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate('-',20)
					+ '-'
					),18)

				insert into @dtl
				select left(('                          '
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate(' ',20)
					+ left(convert(varchar, isnull(@TaskAmt,0) - isnull(@DiscAmt,0), 1),len(convert(varchar, isnull(@TaskAmt,0) - isnull(@DiscAmt,0), 1)) - 3)
					),18)
			end
			else
			begin
				if ((select count(*) from #tsk) > 1)
				begin
				set @TaskAmt = isnull((select sum(OperationHour * OperationCost) from #tsk),0)
				insert into @dtl
				select left(('                                            Sub Total'
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate(' ',20)
					+ left(convert(varchar, @TaskAmt, 1),len(convert(varchar, @TaskAmt, 1)) - 3)
					),18)
				end
			end
			
			if (select count(*) from #itm) > 0
			begin
				insert into @dtl values ('')
			end

			-- add by Seandy 20-9-2011
			-- Untuk menentukan jenis coretan pada keterangan(ada atau tidaknya)
			insert into @temp 
				select @FpjGovNo, 'SRV', COUNT(*) intCount from #tsk a
			  left join #srv b
				on b.JobOrderNo = a.jobOrderNo
			  left join svMstTask c
				on c.CompanyCode = b.CompanyCode
			   and c.ProductType = b.ProductType
			   and c.BasicModel = b.BasicModel
			   and (c.JobType = b.JobType or c.JobType = 'CLAIM' or c.JobType = 'OTHER')
			   and c.OperationNo = a.OperationNo
		end

		-- Insert Jasa SubCon (Lain - Lain)
		if ((select count(*) from #oth) > 0)
		begin
			if (@ReportParam = '1') insert into @dtl values (' ')
			insert into @dtl values ('    LAIN - LAIN')
			insert into @dtl
			select left((
				  right((replicate(' ',2)
				+ convert(varchar,a.RecNo)
				),2)
				+ '  '
				+ a.OperationNo
				+ ' - '
				+ isnull(c.Description, '')
				+ replicate(' ',100)
				),75)
				+ case when (isnull(a.DiscPct, 0) = 0 or @ShowFpjDisc = 0) then replicate(' ', 10)
                       else right(replicate(' ', 10) + '(' + convert(varchar, cast(a.DiscPct as money), 1) + '%)', 10)
                       end 				
				+ right((replicate(' ',18)
				+ left(convert(varchar, cast(a.OperationAmt as money), 1), len(convert(varchar, cast(a.OperationAmt as money), 1)) - 3)
				),18)
			  from #oth a
		  left join #srv b
			on b.JobOrderNo = a.jobOrderNo
		  left join svMstTask c
			on c.CompanyCode = b.CompanyCode
		   and c.ProductType = b.ProductType
		   and c.BasicModel = b.BasicModel
		   and (c.JobType = b.JobType or c.JobType = 'CLAIM' or c.JobType = 'OTHER')
		   and c.OperationNo = a.OperationNo

			if(@Potongan = 1)
			begin
				insert into @dtl
				select left(('                                        '
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate('-',20)
					+ '-'
					),18)

				set @TaskAmt = isnull((select sum(OperationHour * OperationCost) from #oth),0)
				if ((select count(*) from #oth) > 1)
				begin
				insert into @dtl
				select left(('                                            Sub Total'
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate(' ',20)
					+ left(convert(varchar, @TaskAmt, 1),len(convert(varchar, @TaskAmt, 1)) - 3)
					),18)
				end

				set @DiscAmt = isnull((select sum(OperationHour * OperationCost * DiscPct * 0.01) from #oth),0)
				insert into @dtl
				select left(('                                  Potongan Lain-Lain'
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate(' ',20)
					+ left(convert(varchar, @DiscAmt, 1),len(convert(varchar, @DiscAmt, 1)) - 3)
					),18)

				insert into @dtl
				select left(('                                        '
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate('-',20)
					+ '-'
					),18)

				insert into @dtl
				select left(('                                        '
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate(' ',20)
					+ left(convert(varchar, isnull(@TaskAmt,0) - isnull(@DiscAmt,0), 1),len(convert(varchar, isnull(@TaskAmt,0) - isnull(@DiscAmt,0), 1)) - 3)
					),18)
			end
			else
			begin
				if ((select count(*) from #oth) > 1)
				begin
				set @TaskAmt = isnull((select sum(OperationHour * OperationCost) from #oth),0)
				insert into @dtl
				select left(('                                            Sub Total'
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate(' ',20)
					+ left(convert(varchar, @TaskAmt, 1),len(convert(varchar, @TaskAmt, 1)) - 3)
					),18)
				end
			end

			-- add by Seandy 20-9-2011
			-- Untuk menentukan jenis coretan pada keterangan(ada atau tidaknya)
			insert into @temp
				select @FpjGovNo,'DLL',COUNT(*) intCount from #oth a
				  left join #srv b
					on b.JobOrderNo = a.jobOrderNo
				  left join svMstTask c
					on c.CompanyCode = b.CompanyCode
				   and c.ProductType = b.ProductType
				   and c.BasicModel = b.BasicModel
				   and (c.JobType = b.JobType or c.JobType = 'CLAIM' or c.JobType = 'OTHER')
				   and c.OperationNo = a.OperationNo
		end

		-- Insert Sparepart / Material
		if ((select count(*) from #itm) > 0)
		begin
			insert into @dtl values ('    PEMAKAIAN SPAREPART/MATERIAL')
			insert into @dtl
			select left((
				  right((replicate(' ',2)
				+ convert(varchar,a.RecNo)
				),2)
				+ '  '
				+ a.PartNo
				+ ' - '
				+ b.PartName
				+ replicate(' ',100)
				),65)
				+ right((
				  replicate(' ',50)
				+ convert(varchar,a.SupplyQty)
				),10)
				+ case when (isnull(a.DiscPct, 0) = 0 or @ShowFpjDisc = 0) then replicate(' ', 10)
                       else right(replicate(' ', 10) + '(' + convert(varchar, cast(a.DiscPct as money), 1) + '%)', 10)
                       end 				
				+ right((
				  replicate(' ',18)
				+ left(cast(convert(varchar,cast(a.RetailAmt as money),1) as varchar),len(cast(convert(varchar,cast(a.RetailAmt as money),1) as varchar))-3)
				),18)
			  from #itm a
			  left join spMstItemInfo b
				on b.CompanyCode = @CompanyCode
			   and b.PartNo = a.PartNo

			if(@Potongan = 1)
			begin
				insert into @dtl
				select left(('                                        '
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate('-',20)
					+ '-'
					),18)

				set @ItemAmt = isnull((select sum(RetailAmt) from #itm),0)
				if ((select count(*) from #itm) > 1)
				begin
				insert into @dtl
				select left(('                                            Sub Total'
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate(' ',20)
					+ left(convert(varchar, @ItemAmt, 1),len(convert(varchar, @ItemAmt, 1)) - 3)
					),18)
				end

				set @DiscAmt = isnull((select top 1 (PartsDiscAmt + MaterialDiscAmt) from #tsk),0)
				insert into @dtl
				select left(('                                        Potongan Part'
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate(' ',20)
					+ left(convert(varchar, @DiscAmt, 1),len(convert(varchar, @DiscAmt, 1)) - 3)
					),18)

				insert into @dtl
				select left(('                                        '
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate('-',20)
					+ '-'
					),18)

				insert into @dtl
				select left(('                                        '
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate(' ',20)
					+ left(convert(varchar, isnull(@ItemAmt,0) - isnull(@DiscAmt,0), 1),len(convert(varchar, isnull(@ItemAmt,0) - isnull(@DiscAmt,0), 1)) - 3)
					),18)
			end
			else
			begin
				if ((select count(*) from #itm) > 1)
				begin
					set @ItemAmt = isnull((select sum(RetailAmt) from #itm),0)
					insert into @dtl
					select left(('                                            Sub Total'
						+ replicate(' ', 100)
						), 85)
						+ right((
						+ replicate(' ',20)
						+ left(convert(varchar, @ItemAmt, 1),len(convert(varchar, @ItemAmt, 1)) - 3)
						),18)
				end
			end

			if (select count(*) from #oth) > 0
			begin
				insert into @dtl values ('')
			end

			-- add by Seandy 20-9-2011
			-- Untuk menentukan jenis coretan pada keterangan(ada atau tidaknya)
			insert into @temp
				select @FpjGovNo,'OIL',COUNT(*) intCount from #itm a
			  left join spMstItemInfo b
				on b.CompanyCode = @CompanyCode
			   and b.PartNo = a.PartNo
		end

	end

	-- select condition for 'Gabungan'
	if (@FpjType = '3')
	begin
		set @SalesAmt = isnull((select SUM(c.LaborGrossAmt + c.PartsGrossAmt + c.MaterialGrossAmt) SalesAmt from #inv c),0)

		if (@ReportParam = '1')
				insert into @dtl values (' ')
		insert into @dtl
		select left(('1   Untuk No Invoice ' + (select top 1 InvoiceNo from #inv order by InvoiceNo) + ' s/d ' + (select top 1 InvoiceNo from #inv order by InvoiceNo desc) 
			+ replicate(' ', 100)
			),83)
			+ right((
			+ replicate(' ',20)
			+ left(convert(varchar, cast(@SalesAmt as money), 1),len(convert(varchar, cast(@SalesAmt as money), 1)) - 3)
			),20)
		insert into @dtl
		select '    (No. ' + (select top 1 JobOrderNo from #inv order by JobOrderNo) + ' s/d ' + (select top 1 JobOrderNo from #inv order by JobOrderNo desc) + ')'
		select * into #inv_temp from (select * from #inv) #inv_temp
		delete #inv where InvoiceNo <> (select top 1 InvoiceNo from #inv order by JobOrderNo)
		update #inv set LaborGrossAmt = (select sum(LaborGrossAmt) from #inv_temp)
		update #inv set PartsGrossAmt = (select sum(PartsGrossAmt) from #inv_temp)
		update #inv set MaterialGrossAmt = (select sum(MaterialGrossAmt) from #inv_temp)
		update #inv set LaborDiscAmt = (select sum(LaborDiscAmt) from #inv_temp)
		update #inv set PartsDiscAmt = (select sum(PartsDiscAmt) from #inv_temp)
		update #inv set MaterialDiscAmt = (select sum(MaterialDiscAmt) from #inv_temp)
		update #inv set TotalDppAmt = (select sum(TotalDppAmt) from #inv_temp)
		update #inv set TotalPphAmt = (select sum(TotalPphAmt) from #inv_temp)
		update #inv set TotalPpnAmt = (select sum(TotalPpnAmt) from #inv_temp)
		update #inv set TotalSrvAmt = TotalDppAmt + TotalPpnAmt
		alter table #inv alter column JobOrderNo varchar(50)
		update #inv set JobOrderNo = (select top 1 JobOrderNo from #inv_temp order by JobOrderNo) + '-' + right((select top 1 JobOrderNo from #inv_temp order by JobOrderNo desc),6)
		alter table #inv alter column InvoiceNo varchar(50)
		update #inv set InvoiceNo = (select top 1 InvoiceNo from #inv_temp order by InvoiceNo) + '-' + right((select top 1 InvoiceNo from #inv_temp order by InvoiceNo desc),6)
		drop table #inv_temp				
	end

	-- select condition for 'Perincian on Lampiran'
	if (@FpjType = '4')
	begin
		set @TaskAmt = isnull((select sum(OperationHour * OperationCost) from #tsk),0)
		set @ItemAmt = isnull((select sum(RetailAmt) from #itm),0)
		insert into @dtl
		select left(('1   Perincian dapat dilihat pada Faktur Penjualan'
			+ replicate(' ', 100)
			), 83)
			+ right((
			+ replicate(' ',20)
			+ left(convert(varchar, @TaskAmt + @ItemAmt, 1),len(convert(varchar, @TaskAmt + @ItemAmt, 1)) - 3)
			),20)
		insert into @dtl
		select '    dengan Nomor ' + a.InvoiceNo from #inv a
	end

	-- Laporan 'Standard Body Repair'
	if (@FpjType = '5')
	begin
		if ((select count(*) from #tsk) > 0)
		begin
			set @TaskAmt = isnull((select sum(OperationHour * OperationCost) from #tsk),0)
			if (@ReportParam = '1')
				insert into @dtl values (' ')
			insert into @dtl
			select left(('    JASA PERBAIKAN'
				+ replicate(' ', 100)
				), 85)
				+ right((
				+ replicate(' ',20)
				+ left(convert(varchar, @TaskAmt, 1),len(convert(varchar, @TaskAmt, 1)) - 3)
				),18)

			insert into @dtl
			select left((
				  right((replicate(' ',2)
				+ convert(varchar,a.RecNo)
				),2)
				+ '  '
				+ a.OperationNo
				+ ' - '
				+ c.Description
				),80)
			  from #tsk a
		  left join #srv b
			on b.JobOrderNo = a.jobOrderNo
		  left join svMstTask c
			on c.CompanyCode = b.CompanyCode
		   and c.ProductType = b.ProductType
		   and c.BasicModel = b.BasicModel
		   and (c.JobType = b.JobType or c.JobType = 'CLAIM' or c.JobType = 'OTHER')
		   and c.OperationNo = a.OperationNo

			if(@Potongan = 1)
			begin
				insert into @dtl
				select left(('                                        '
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate('-',20)
					+ '-'
					),18)

				set @TaskAmt = isnull((select sum(OperationHour * OperationCost) from #tsk),0)
				set @DiscAmt = (select sum (OperationHour * OperationCost* DiscPct * 0.01) from #tsk)
				insert into @dtl
				select left(('                                        Potongan Jasa'
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate(' ',20)
					+ left(convert(varchar, @DiscAmt, 1),len(convert(varchar, @DiscAmt, 1)) - 3)
					),18)

				insert into @dtl
				select left(('                                        '
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate('-',20)
					+ '-'
					),18)

				insert into @dtl
				select left(('                                        '
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate(' ',20)
					+ left(convert(varchar, isnull(@TaskAmt,0) - isnull(@DiscAmt,0), 1),len(convert(varchar, isnull(@TaskAmt,0) - isnull(@DiscAmt,0), 1)) - 3)
					),18)
			end

			if (select count(*) from #itmSP) > 0
			begin
				insert into @dtl values ('')
			end
		end

	if ((select count(*) from #itmSP) > 0)
	begin
		insert into @dtl values ('    PEMAKAIAN SPAREPART')
		insert into @dtl
		select left((
			  right((replicate(' ',2)
			+ convert(varchar,a.RecNo)
			),2)
			+ '  '
			+ a.PartNo
			+ ' - '
			+ b.PartName
			+ replicate(' ',100)
			),65)
			+ right((
			  replicate(' ',50)
			+ convert(varchar,a.SupplyQty)
			),10)
			+ right((
			  replicate(' ',50)
			+ left(cast(convert(varchar,cast(a.RetailAmt as money),1) as varchar),len(cast(convert(varchar,cast(a.RetailAmt as money),1) as varchar))-3)
			),28)
		  from  #itmSP a
		  left join spMstItemInfo b
			on b.CompanyCode = @CompanyCode
		   and b.PartNo = a.PartNo
	end

			if(@Potongan = 1)
			begin
				insert into @dtl
				select left(('                                        '
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate('-',20)
					+ '-'
					),18)

				set @ItemAmt = isnull((select sum(RetailAmt) from #itm),0)
				set @DiscAmt = isnull((select top 1 (PartsDiscAmt + MaterialDiscAmt) from #tsk),0)
				insert into @dtl
				select left(('                                        Potongan Part'
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate(' ',20)
					+ left(convert(varchar, @DiscAmt, 1),len(convert(varchar, @DiscAmt, 1)) - 3)
					),18)

				insert into @dtl
				select left(('                                        '
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate('-',20)
					+ '-'
					),18)

				insert into @dtl
				select left(('                                        '
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate(' ',20)
					+ left(convert(varchar, isnull(@ItemAmt,0) - isnull(@DiscAmt,0), 1),len(convert(varchar, isnull(@ItemAmt,0) - isnull(@DiscAmt,0), 1)) - 3)
					),18)
			end

		if (select count(*) from #itmMtrl) > 0
		begin
			insert into @dtl values ('')
		end

	if ((select count(*) from #itmMtrl) > 0)
	begin
			insert into @dtl values ('    PEMAKAIAN MATERIAL / OLI')
			set @MaterialAmt = (select sum(RetailAmt) from #itmMtrl)
			insert into @dtl
			select left((' 1  MATERIAL'
				+ replicate(' ', 100)
				), 85)
				+ right((
				+ replicate(' ',20)
				+ left(convert(varchar, @MaterialAmt, 1),len(convert(varchar, @MaterialAmt, 1)) - 3)
				),18)
	end

		if ((select count(*) from #itmOli) > 0)
		begin
			if ((select count(*) from #itmMtrl) = 0)
			begin
				insert into @dtl values ('')
				insert into @dtl values ('    PEMAKAIAN MATERIAL / OLI')
			end
			insert into @dtl
			select case when (select count(*) from #itmMtrl) > 0 then
				left((
				  right((replicate(' ',2)
				+ convert(varchar,a.RecNo + 1)
				),2)
				+ '  '
				+ a.PartNo
				+ ' - '
				+ b.PartName
				+ replicate(' ',100)
				),65)
				+ right((
				  replicate(' ',50)
				+ convert(varchar,a.SupplyQty)
				),10)
				+ right((
				  replicate(' ',50)
				+ left(cast(convert(varchar,cast(a.RetailAmt as money),1) as varchar),len(cast(convert(varchar,cast(a.RetailAmt as money),1) as varchar))-3)
				),28)
				else
				left((
				  right((replicate(' ',2)
				+ convert(varchar,a.RecNo)
				),2)
				+ '  '
				+ a.PartNo
				+ ' - '
				+ b.PartName
				+ replicate(' ',100)
				),65)
				+ right((
				  replicate(' ',50)
				+ convert(varchar,a.SupplyQty)
				),10)
				+ right((
				  replicate(' ',50)
				+ left(cast(convert(varchar,cast(a.RetailAmt as money),1) as varchar),len(cast(convert(varchar,cast(a.RetailAmt as money),1) as varchar))-3)
				),28)
				end
			from #itmOli a
				left join spMstItemInfo b
				on b.CompanyCode = @CompanyCode
				and b.PartNo = a.PartNo

			if(@Potongan = 1)
			begin
				insert into @dtl
				select left(('                                        '
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate('-',20)
					+ '-'
					),18)

				if ((select count(*) from #ItmMtrl) = 0)
				begin
					set @ItemAmt = isnull((select sum(RetailAmt) from #itmOli),0)
				end
				else
				begin	
					set @ItemAmt = isnull((select sum(RetailAmt) from #itmMtrl),0) + isnull((select sum(RetailAmt) from #itmOli),0) 
				end

				set @DiscAmt = (select sum (OperationHour * OperationCost* DiscPct * 0.01) from #oth)
				insert into @dtl
				select left(('                                  Potongan Lain-Lain'
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate(' ',20)
					+ left(convert(varchar, @DiscAmt, 1),len(convert(varchar, @DiscAmt, 1)) - 3)
					),18)

				insert into @dtl
				select left(('                                        '
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate('-',20)
					+ '-'
					),18)

				insert into @dtl
				select left(('                                        '
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate(' ',20)
					+ left(convert(varchar, isnull(@ItemAmt,0) - isnull(@DiscAmt,0), 1),len(convert(varchar, isnull(@ItemAmt,0) - isnull(@DiscAmt,0), 1)) - 3)
					),18)
			end
		end
	end

	-- select condition for 'RinciBodyRepair'
	if (@FpjType = '6')
	begin
		if ((select count(*) from #tsk) > 0)
		begin
			if (@ReportParam = '1')
				insert into @dtl values (' ')
			insert into @dtl values ('    JASA PERBAIKAN')
			insert into @dtl
			select left((
				  right((replicate(' ',2)
				+ convert(varchar,a.RecNo)
				),2)
				+ '  '
				+ a.OperationNo
				+ ' - '
				+ c.Description
				+ replicate(' ',100)
				),65)
				+ right((
				  replicate(' ',10)
				),10)
				+ right((
				  replicate(' ',50)
				+ left(convert(varchar, cast(a.OperationAmt as money), 1), len(convert(varchar, cast(a.OperationAmt as money), 1)) - 3)
				),28)
			  from #tsk a
		  left join #srv b
			on b.JobOrderNo = a.jobOrderNo
		  left join svMstTask c
			on c.CompanyCode = b.CompanyCode
		   and c.ProductType = b.ProductType
		   and c.BasicModel = b.BasicModel
		   and (c.JobType = b.JobType or c.JobType = 'CLAIM' or c.JobType = 'OTHER')
		   and c.OperationNo = a.OperationNo

			if(@Potongan = 1)
			begin
				insert into @dtl
				select left(('                                        '
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate('-',20)
					+ '-'
					),18)

				set @TaskAmt = isnull((select sum(OperationHour * OperationCost) from #tsk),0)
				if ((select count(*) from #tsk) > 1)
				begin
				insert into @dtl
				select left(('                 Sub Total'
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate(' ',20)
					+ left(convert(varchar, @TaskAmt, 1),len(convert(varchar, @TaskAmt, 1)) - 3)
					),18)
				end

				set @DiscAmt = (select sum (OperationHour * OperationCost* DiscPct * 0.01) from #tsk)
				insert into @dtl
				select left(('                                        Potongan Jasa'
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate(' ',20)
					+ left(convert(varchar, @DiscAmt, 1),len(convert(varchar, @DiscAmt, 1)) - 3)
					),18)

				insert into @dtl
				select left(('                                        '
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate('-',20)
					+ '-'
					),18)

				insert into @dtl
				select left(('                                        '
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate(' ',20)
					+ left(convert(varchar, isnull(@TaskAmt,0) - isnull(@DiscAmt,0), 1),len(convert(varchar, isnull(@TaskAmt,0) - isnull(@DiscAmt,0), 1)) - 3)
					),18)
			end
			else
			begin
				if ((select count(*) from #tsk) > 1)
				begin
				set @TaskAmt = isnull((select sum(OperationHour * OperationCost) from #tsk),0)
				insert into @dtl
				select left(('                                            Sub Total'
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate(' ',20)
					+ left(convert(varchar, @TaskAmt, 1),len(convert(varchar, @TaskAmt, 1)) - 3)
					),18)
				end
			end

		if (select count(*) from #itm) > 0
		begin
			insert into @dtl values ('')
		end

		end

		if ((select count(*) from #itm) > 0)
		begin
			insert into @dtl values ('    PEMAKAIAN SPAREPART')
			insert into @dtl
			select left((
				  right((replicate(' ',2)
				+ convert(varchar,a.RecNo)
				),2)
				+ '  '
				+ a.PartNo
				+ ' - '
				+ b.PartName
				+ replicate(' ',100)
				),65)
				+ right((
				  replicate(' ',50)
				+ convert(varchar,a.SupplyQty)
				),10)
				+ right((
				  replicate(' ',50)
				+ left(cast(convert(varchar,cast(a.RetailAmt as money),1) as varchar),len(cast(convert(varchar,cast(a.RetailAmt as money),1) as varchar))-3)
				),28)
			  from #itmSP a
			  left join spMstItemInfo b
				on b.CompanyCode = @CompanyCode
			   and b.PartNo = a.PartNo
			if(@Potongan = 1)
			begin
				insert into @dtl
				select left(('                                        '
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate('-',20)
					+ '-'
					),18)

				set @ItemAmt = isnull((select sum(RetailAmt) from #itmSP),0)
				if ((select count(*) from #itm) > 1)
				begin
				insert into @dtl
				select left(('                                            Sub Total'
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate(' ',20)
					+ left(convert(varchar, @ItemAmt, 1),len(convert(varchar, @ItemAmt, 1)) - 3)
					),18)
				end

				set @DiscAmt = isnull((select top 1 (PartsDiscAmt + MaterialDiscAmt) from #tsk),0)
				insert into @dtl
				select left(('                                        Potongan Part'
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate(' ',20)
					+ left(convert(varchar, @DiscAmt, 1),len(convert(varchar, @DiscAmt, 1)) - 3)
					),18)


				insert into @dtl
				select left(('                                        '
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate('-',20)
					+ '-'
					),18)

				insert into @dtl
				select left(('                                        '
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate(' ',20)
					+ left(convert(varchar, isnull(@ItemAmt,0) - isnull(@DiscAmt,0), 1),len(convert(varchar, isnull(@ItemAmt,0) - isnull(@DiscAmt,0), 1)) - 3)
					),18)
			end
			else
			begin
				if ((select count(*) from #itm) > 1)
				begin
					set @ItemAmt = isnull((select sum(RetailAmt) from #itmSP),0)
					insert into @dtl
					select left(('                                            Sub Total'
						+ replicate(' ', 100)
						), 85)
						+ right((
						+ replicate(' ',20)
						+ left(convert(varchar, @ItemAmt, 1),len(convert(varchar, @ItemAmt, 1)) - 3)
						),18)
				end
			end
		end

		if (select count(*) from #itmRMtrl) > 0
		begin
			insert into @dtl values ('')
		end

	if ((select count(*) from #itmRMtrl) > 0)
	begin
		insert into @dtl values ('    PEMAKAIAN MATERIAL / OLI')
		insert into @dtl
		select left((
			  right((replicate(' ',2)
			+ convert(varchar,a.RecNo)
			),2)
			+ '  '
			+ a.PartNo
			+ ' - '
			+ b.PartName
			+ replicate(' ',100)
			),65)
			+ right((
			  replicate(' ',50)
			+ convert(varchar,a.SupplyQty)
			),10)
			+ right((
			  replicate(' ',50)
			+ left(cast(convert(varchar,cast(a.RetailAmt as money),1) as varchar),len(cast(convert(varchar,cast(a.RetailAmt as money),1) as varchar))-3)
			),28)
		  from #itmRMtrl a
		  left join spMstItemInfo b
			on b.CompanyCode = @CompanyCode
		   and b.PartNo = a.PartNo
	
			if(@Potongan = 1)
			begin
				insert into @dtl
				select left(('                                        '
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate('-',20)
					+ '-'
					),18)

				if ((select count(*) from #ItmMtrl) = 0)
				begin
					set @ItemAmt = isnull((select sum(RetailAmt) from #itmOli),0)
				end
				else
				begin	
					set @ItemAmt = isnull((select sum(RetailAmt) from #itmMtrl),0) + isnull((select sum(RetailAmt) from #itmOli),0) 
				end

					if ((select count(*) from #itmRMtrl) > 1)
					begin
						insert into @dtl
						select left(('                                            Sub Total'
							+ replicate(' ', 100)
							), 85)
							+ right((
							+ replicate(' ',20)
							+ left(convert(varchar, @ItemAmt, 1),len(convert(varchar, @ItemAmt, 1)) - 3)
							),18)

					end
				end

				set @DiscAmt = (select sum (OperationHour * OperationCost* DiscPct * 0.01) from #oth)
				insert into @dtl
				select left(('                                  Potongan Lain-Lain'
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate(' ',20)
					+ left(convert(varchar, @DiscAmt, 1),len(convert(varchar, @DiscAmt, 1)) - 3)
					),18)

				insert into @dtl
				select left(('                                        '
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate('-',20)
					+ '-'
					),18)

				insert into @dtl
				select left(('                                        '
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate(' ',20)
					+ left(convert(varchar, isnull(@ItemAmt,0) - isnull(@DiscAmt,0), 1),len(convert(varchar, isnull(@ItemAmt,0) - isnull(@DiscAmt,0), 1)) - 3)
					),18)
			end
			else
			begin
				if ((select count(*) from #ItmMtrl) = 0)
				begin
					set @ItemAmt = isnull((select sum(RetailAmt) from #itmOli),0)
				end
				else
				begin	
					set @ItemAmt = isnull((select sum(RetailAmt) from #itmMtrl),0) + isnull((select sum(RetailAmt) from #itmOli),0) 
				end

					if ((select count(*) from #itmRMtrl) > 1)
					begin
						insert into @dtl
						select left(('                                            Sub Total'
							+ replicate(' ', 100)
							), 85)
							+ right((
							+ replicate(' ',20)
							+ left(convert(varchar, @ItemAmt, 1),len(convert(varchar, @ItemAmt, 1)) - 3)
							),18)

					end
			end

	End

	-- select condition for 'Khusus Body Repair'
	if (@FpjType = '7')
	begin
		if ((select count(*) from #tsk) > 0)
		begin
			if (@ReportParam = '1')
				insert into @dtl values (' ')
			insert into @dtl values ('    JASA PERBAIKAN')
			insert into @dtl
			select left((
				  right((replicate(' ',2)
				+ convert(varchar,a.RecNo)
				),2)
				+ '  '
				+ a.OperationNo
				+ ' - '
				+ c.Description
				+ replicate(' ',100)
				),65)
				+ right((
				  replicate(' ',10)
				),10)
				+ right((
				  replicate(' ',50)
				+ left(convert(varchar, cast(a.OperationAmt as money), 1), len(convert(varchar, cast(a.OperationAmt as money), 1)) - 3)
				),28)
			  from #tsk a
		  left join #srv b
			on b.JobOrderNo = a.jobOrderNo
		  left join svMstTask c
			on c.CompanyCode = b.CompanyCode
		   and c.ProductType = b.ProductType
		   and c.BasicModel = b.BasicModel
		   and (c.JobType = b.JobType or c.JobType = 'CLAIM' or c.JobType = 'OTHER')
		   and c.OperationNo = a.OperationNo

			if(@Potongan = 1)
			begin
				insert into @dtl
				select left(('                                        '
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate('-',20)
					+ '-'
					),18)

				set @TaskAmt = isnull((select sum(OperationHour * OperationCost) from #tsk),0)
				if ((select count(*) from #tsk) > 1)
				begin
				insert into @dtl
				select left(('                                            Sub Total'
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate(' ',20)
					+ left(convert(varchar, @TaskAmt, 1),len(convert(varchar, @TaskAmt, 1)) - 3)
					),18)
				end

				set @DiscAmt = (select sum (OperationHour * OperationCost* DiscPct * 0.01) from #tsk)
				insert into @dtl
				select left(('                                        Potongan Jasa'
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate(' ',20)
					+ left(convert(varchar, @DiscAmt, 1),len(convert(varchar, @DiscAmt, 1)) - 3)
					),18)

				insert into @dtl
				select left(('                                        '
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate('-',20)
					+ '-'
					),18)

				insert into @dtl
				select left(('                                        '
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate(' ',20)
					+ left(convert(varchar, isnull(@TaskAmt,0) - isnull(@DiscAmt,0), 1),len(convert(varchar, isnull(@TaskAmt,0) - isnull(@DiscAmt,0), 1)) - 3)
					),18)
			end
			else
			begin
				if ((select count(*) from #tsk) > 1)
				begin
				set @TaskAmt = isnull((select sum(OperationHour * OperationCost) from #tsk),0)
				insert into @dtl
				select left(('                                            Sub Total'
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate(' ',20)
					+ left(convert(varchar, @TaskAmt, 1),len(convert(varchar, @TaskAmt, 1)) - 3)
					),18)
				end
			end

			if (select count(*) from #itm) > 0
			begin
				insert into @dtl values ('')
			end

		end

		if ((select count(*) from #itm) > 0)
		begin
			insert into @dtl values ('    PEMAKAIAN SPAREPART')
			insert into @dtl
			select left((
				  right((replicate(' ',2)
				+ convert(varchar,a.RecNo)
				),2)
				+ '  '
				+ a.PartNo
				+ ' - '
				+ b.PartName
				+ replicate(' ',100)
				),65)
				+ right((
				  replicate(' ',50)
				+ convert(varchar,a.SupplyQty)
				),10)
				+ right((
				  replicate(' ',50)
				+ left(cast(convert(varchar,cast(a.RetailAmt as money),1) as varchar),len(cast(convert(varchar,cast(a.RetailAmt as money),1) as varchar))-3)
				),28)
			  from #itmSP a
			  left join spMstItemInfo b
				on b.CompanyCode = @CompanyCode
			   and b.PartNo = a.PartNo

			if(@Potongan = 1)
			begin
				insert into @dtl
				select left(('                                        '
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate('-',20)
					+ '-'
					),18)

				set @ItemAmt = isnull((select sum(RetailAmt) from #itmSP),0)
				if ((select count(*) from #itm) > 1)
				begin
				insert into @dtl
				select left(('                                            Sub Total'
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate(' ',20)
					+ left(convert(varchar, @ItemAmt, 1),len(convert(varchar, @ItemAmt, 1)) - 3)
					),18)
				end

				set @DiscAmt = isnull((select top 1 (PartsDiscAmt + MaterialDiscAmt) from #tsk),0)
				insert into @dtl
				select left(('                                        Potongan Part'
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate(' ',20)
					+ left(convert(varchar, @DiscAmt, 1),len(convert(varchar, @DiscAmt, 1)) - 3)
					),18)


				insert into @dtl
				select left(('                                        '
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate('-',20)
					+ '-'
					),18)

				insert into @dtl
				select left(('                                        '
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate(' ',20)
					+ left(convert(varchar, isnull(@ItemAmt,0) - isnull(@DiscAmt,0), 1),len(convert(varchar, isnull(@ItemAmt,0) - isnull(@DiscAmt,0), 1)) - 3)
					),18)
			end
			else
			begin
				if ((select count(*) from #itm) > 1)
				begin
					set @ItemAmt = isnull((select sum(RetailAmt) from #itmSP),0)
					insert into @dtl
					select left(('                                            Sub Total'
						+ replicate(' ', 100)
						), 85)
						+ right((
						+ replicate(' ',20)
						+ left(convert(varchar, @ItemAmt, 1),len(convert(varchar, @ItemAmt, 1)) - 3)
						),18)
				end
			end

		end
		if (select count(*) from #ItmMtrl) > 0
		begin
			insert into @dtl values ('')
		end

	if ((select count(*) from #ItmMtrl) > 0)
		begin
			insert into @dtl values ('    PEMAKAIAN MATERIAL / OLI')
			set @MaterialAmt = (select sum(RetailAmt) from #ItmMtrl)
			Print(@MaterialAmt)
			if (@ReportParam = '1')
			begin
				insert into @dtl values (' ')
			end
			insert into @dtl
			select left((' 1  MATERIAL'
				+ replicate(' ', 100)
				), 85)
				+ right((
				+ replicate(' ',20)
				+ left(convert(varchar, @MaterialAmt, 1),len(convert(varchar, @MaterialAmt, 1)) - 3)
				),18)
		end

		if ((select count(*) from #itmOli) > 0)
		begin
			if ((select count(*) from #ItmMtrl) = 0)
			begin
				insert into @dtl values (' ')
				insert into @dtl values ('    PEMAKAIAN MATERIAL / OLI')
			end
			insert into @dtl
			select case when (select count(*) from #ItmMtrl) > 0 then
				left((
				  right((replicate(' ',2)
				+ convert(varchar,a.RecNo + 1)
				),2)
				+ '  '
				+ a.PartNo
				+ ' - '
				+ b.PartName
				+ replicate(' ',100)
				),65)
				+ right((
				  replicate(' ',50)
				+ convert(varchar,a.SupplyQty)
				),10)
				+ right((
				  replicate(' ',50)
				+ left(cast(convert(varchar,cast(a.RetailAmt as money),1) as varchar),len(cast(convert(varchar,cast(a.RetailAmt as money),1) as varchar))-3)
				),28)
				else
				left((
				  right((replicate(' ',2)
				+ convert(varchar,a.RecNo)
				),2)
				+ '  '
				+ a.PartNo
				+ ' - '
				+ b.PartName
				+ replicate(' ',100)
				),65)
				+ right((
				  replicate(' ',50)
				+ convert(varchar,a.SupplyQty)
				),10)
				+ right((
				  replicate(' ',50)
				+ left(cast(convert(varchar,cast(a.RetailAmt as money),1) as varchar),len(cast(convert(varchar,cast(a.RetailAmt as money),1) as varchar))-3)
				),28)
				end
			from #itmOli a
				left join spMstItemInfo b
				on b.CompanyCode = @CompanyCode
				and b.PartNo = a.PartNo

			if(@Potongan = 1)
			begin
				insert into @dtl
				select left(('                                        '
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate('-',20)
					+ '-'
					),18)

				if ((select count(*) from #ItmMtrl) = 0)
				begin
					set @ItemAmt = isnull((select sum(RetailAmt) from #itmOli),0)
				end
				else
				begin	
					set @ItemAmt = isnull((select sum(RetailAmt) from #itmMtrl),0) + isnull((select sum(RetailAmt) from #itmOli),0) 
				end
				if ((select count(*) from #itmRMtrl) > 1)
				begin
					insert into @dtl
					select left(('                                            Sub Total'
						+ replicate(' ', 100)
						), 85)
						+ right((
						+ replicate(' ',20)
						+ left(convert(varchar, @ItemAmt, 1),len(convert(varchar, @ItemAmt, 1)) - 3)
						),18)
				end

				set @DiscAmt = (select sum (OperationHour * OperationCost* DiscPct * 0.01) from #oth)
				insert into @dtl
				select left(('                                  Potongan Lain-Lain'
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate(' ',20)
					+ left(convert(varchar, @DiscAmt, 1),len(convert(varchar, @DiscAmt, 1)) - 3)
					),18)

				insert into @dtl
				select left(('                                        '
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate('-',20)
					+ '-'
					),18)

				insert into @dtl
				select left(('                                        '
					+ replicate(' ', 100)
					), 85)
					+ right((
					+ replicate(' ',20)
					+ left(convert(varchar, isnull(@ItemAmt,0) - isnull(@DiscAmt,0), 1),len(convert(varchar, isnull(@ItemAmt,0) - isnull(@DiscAmt,0), 1)) - 3)
					),18)
			end
			else
			begin
				if ((select count(*) from #ItmMtrl) = 0)
				begin
					set @ItemAmt = isnull((select sum(RetailAmt) from #itmOli),0)
				end
				else
				begin	
					set @ItemAmt = isnull((select sum(RetailAmt) from #itmMtrl),0) + isnull((select sum(RetailAmt) from #itmOli),0) 
				end
				if ((select count(*) from #itmRMtrl) > 1)
				begin
					insert into @dtl
					select left(('                                            Sub Total'
						+ replicate(' ', 100)
						), 85)
						+ right((
						+ replicate(' ',20)
						+ left(convert(varchar, @ItemAmt, 1),len(convert(varchar, @ItemAmt, 1)) - 3)
						),18)
				end
			end
		end
	End

	-- select condition for 'Gabungan'
	if (@FpjType = '8')
	begin
		set @TaskAmt = isnull((select sum(OperationHour * OperationCost) from #tsk where IsSubCon != 1),0)
		set @ItemAmt = isnull((select sum(RetailAmt) from #itm where TypeOfGoods in ('0', '1', '2')),0)
		set @SalesAmt = isnull((select sum(c.LaborGrossAmt + c.PartsGrossAmt + c.MaterialGrossAmt) SalesAmt from #inv c),0)
		set @SubletAmt = isnull((select sum(OperationHour * OperationCost) from #tsk where IsSubCon = 1),0) + 
		                 isnull((select sum(RetailAmt) from #itm where TypeOfGoods not in ('0', '1', '2')),0)
		

		if (@ReportParam = '1')
		--insert into @dtl values ('1 ')
		insert into @dtl
		select left(('1    Untuk No Invoice ' + (select top 1 InvoiceNo from #inv order by InvoiceNo) + ' s/d ' + (select top 1 InvoiceNo from #inv order by InvoiceNo desc) 
			+ replicate(' ', 100)
			),83)
		
		insert into @dtl
		select '2    (No. ' + (select top 1 JobOrderNo from #inv order by JobOrderNo) + ' s/d ' + (select top 1 JobOrderNo from #inv order by JobOrderNo desc) + ')'
		
		insert into @dtl values ('3 ')
		
		if(@ItemAmt <> 0)
		begin
			insert into @dtl
			select left(('4          SPARE PART DAN MATERIAL (SGP & SGO)' 
				+ replicate(' ', 100)
				),83)
				+ right((
				+ replicate(' ',20)
				+ left(convert(varchar, cast(@ItemAmt as money), 1),len(convert(varchar, cast(@ItemAmt as money), 1)) - 3)
				),20)
		end
		
		if(@TaskAmt <> 0)
		begin
				insert into @dtl
				select left(('5          JASA' 
					+ replicate(' ', 100)
					),83)
					+ right((
					+ replicate(' ',20)
					+ left(convert(varchar, cast(@TaskAmt as money), 1),len(convert(varchar, cast(@TaskAmt as money), 1)) - 3)
					),20)
		end
		
		if(@SubletAmt <> 0)
		begin
			insert into @dtl
			select left(('6          SUBLET' 
				+ replicate(' ', 100)
				),83)
				+ right((
				+ replicate(' ',20)
				+ left(convert(varchar, cast(@SubletAmt as money), 1),len(convert(varchar, cast(@SubletAmt as money), 1)) - 3)
				),20)
		end
		
		select * into #inv_tempD from (select * from #inv) #inv_tempD
		delete #inv where InvoiceNo <> (select top 1 InvoiceNo from #inv order by JobOrderNo)
		update #inv set LaborGrossAmt = (select sum(LaborGrossAmt) from #inv_tempD)
		update #inv set PartsGrossAmt = (select sum(PartsGrossAmt) from #inv_tempD)
		update #inv set MaterialGrossAmt = (select sum(MaterialGrossAmt) from #inv_tempD)
		update #inv set LaborDiscAmt = (select sum(LaborDiscAmt) from #inv_tempD)
		update #inv set PartsDiscAmt = (select sum(PartsDiscAmt) from #inv_tempD)
		update #inv set MaterialDiscAmt = (select sum(MaterialDiscAmt) from #inv_tempD)
		update #inv set TotalDppAmt = (select sum(TotalDppAmt) from #inv_tempD)
		update #inv set TotalPphAmt = (select sum(TotalPphAmt) from #inv_tempD)
		update #inv set TotalPpnAmt = (select sum(TotalPpnAmt) from #inv_tempD)
		update #inv set TotalSrvAmt = TotalDppAmt + TotalPpnAmt
		alter table #inv alter column JobOrderNo varchar(50)
		update #inv set JobOrderNo = (select top 1 JobOrderNo from #inv_tempD order by JobOrderNo) + '-' + right((select top 1 JobOrderNo from #inv_tempD order by JobOrderNo desc),6)
		alter table #inv alter column InvoiceNo varchar(50)
		update #inv set InvoiceNo = (select top 1 InvoiceNo from #inv_tempD order by InvoiceNo) + '-' + right((select top 1 InvoiceNo from #inv_tempD order by InvoiceNo desc),6)
		drop table #inv_tempD		
	end

	-- insert to declare to decide last line every FPJGovNo
	set @CountDtl = (select COUNT(*) from @dtl)

	if(@FPJType <> '3' AND @FPJType <> '4' AND @FPJType <> '8')
	begin	
		if @CountDtl <> (case when @CountDtl = 19 then 17 else 18 end)
		begin
			insert into @dtl values (' ')
			set @StatusEnd = 1
		end
		else
		begin
			set @StatusEnd = 0
		end
	end

	-- select data to display result on Report
	select * into #result from (
	select 
		 b.FpjGovNo
		,b.CompanyCode
		,case when @fInfoPKP = 1 then
			(case @fStatus when '1' then @fCompName else f.CompanyGovName end)
		 else '' end as CompanyName
		,case when @fInfoPKP = 1 then
			(case @fStatus when '1' then @fAdd else 
				(case when @IsHoldingAddr=0 then isnull(f.Address1, '') + ' '  + isnull(f.Address2, '') + ' ' + isnull(f.Address3, '') + ' ' + isnull(f.Address4, '')
					else (select isnull(Address1, '') + ' '  + isnull(Address2, '') + ' ' + isnull(Address3, '') + ' ' + isnull(Address4, '') from gnMstCoProfile where CompanyCode=@CompanyCode
					and BranchCode=(select BranchCode from gnMstOrganizationDtl where CompanyCode=@CompanyCode and IsBranch=0))
				end)end)
		  else '' end as CompanyAddress
		,case when @fInfoPKP = 1 then
			(case @fStatus when '1' then @fNPWP else f.NPWPNo end)
		 else '' end as NPWPNo
		,case @fStatus when '1' then @fSKP else f.SkpNo end as SkpNo
		,case @fStatus when '1' then @fSKPDate else f.SkpDate end as SkpDate
		,b.FpjDate
		,case when (@FpjType = '3' OR @FPJType = '8') then '' else isnull(c.PoliceRegNo,'') end PoliceRegNo  
		,c.CustomerCodeBill CustCodeBill
		,g.CustomerGovName CustNameBill
		,g.Address1 CustAddr1Bill
		,g.Address2 CustAddr2Bill
		,g.Address3 CustAddr3Bill
		,g.NPWPNo CustNpwpBill
		,g.SkpNo CustSkpBill
		,a.DetailDesc
		,c.LaborGrossAmt + c.PartsGrossAmt + c.MaterialGrossAmt SalesAmt  
		,c.LaborDiscAmt + c.PartsDiscAmt + c.MaterialDiscAmt DiscAmt  
		,0 as DPAmt
		,c.TotalDppAmt DPPAmt  
		,case when (@IsRecalPPN=1) and substring(c.InvoiceNo,1,3) in ('INF','INW') then floor(c.TotalDppAmt*10/100)
			else isnull(c.TotalPphAmt, 0) + isnull(c.TotalPpnAmt, 0) 
		end PPNAmt  
		,'NIHIL' as DppBm  
		,'NIHIL' as PpnBm  
		, c.InvoiceNo
		,case @fStatus when '1' then @fCity else h.LookupValueName end as CityName
		,b.SignedDate
		,c.JobOrderNo
		,case when (@IsRecalPPN=1) and substring(c.InvoiceNo,1,3) in ('INF','INW') then floor(c.TotalDppAmt*10/100)+ c.TotalDppAmt
			else c.TotalSrvAmt
		end TotalAmt
		,b.DueDate    
		,'              ##########' as Asteric1
		,'            ######################' as Asteric3
		,isnull((select distinct intCount from @temp z where z.FPJGovNo = b.FpjGovNo and z.CodeID = 'SRV'),0)TotService
		,isnull((select distinct intCount from @temp z where z.FPJGovNo = b.FpjGovNo and z.CodeID = 'OIL'),0) TotOil
		,isnull((select distinct intCount from @temp z where z.FPJGovNo = b.FpjGovNo and z.CodeID = 'DLL'),0) TotDLL
		,case when @FlagAsterix = 0 or @FlagAsterix is null
			then '                        ################################'  
			else 
				case when isnull((select distinct intCount from @temp z where z.FPJGovNo = b.FpjGovNo and z.CodeID = 'SRV'),0) = 0 AND isnull((select distinct intCount from @temp z where z.FPJGovNo = b.FpjGovNo and z.CodeID = 'OIL'),0) <> 0 
				then '###########                            ####################'
				else
					case when isnull((select distinct intCount from @temp z where z.FPJGovNo = b.FpjGovNo and z.CodeID = 'SRV'),0) <> 0 AND isnull((select distinct intCount from @temp z where z.FPJGovNo = b.FpjGovNo and z.CodeID = 'OIL'),0) <> 0
					then
						'                                                 ####################'
					else
						case when isnull((select distinct intCount from @temp z where z.FPJGovNo = b.FpjGovNo and z.CodeID = 'SRV'),0) <> 0 AND isnull((select distinct intCount from @temp z where z.FPJGovNo = b.FpjGovNo and z.CodeID = 'OIL'),0) = 0
						then	'                        ################################'
						else
							'###########                            ####################'
						end
					end
				end
			end
			as Asteric2
		,'STANDARD' ReportType
		, 7 as xLocation
		, 247 as yLocation
		, isnull(@CountDtl + @StatusEnd,0) TotService2
	  from @dtl a, #fpj b
	  left join #inv c
		on c.CompanyCode = b.CompanyCode
	   and ((case when @IsCentral = 1 then c.BranchCode end) <> ''
			or (case when @IsCentral = 0 then c.BranchCode end) = @BranchCode)
	  left join gnMstCoProfile f
		on f.CompanyCode = b.CompanyCode
	   and f.BranchCode = b.BranchCode
	  left join gnMstCustomer g
		on g.CompanyCode = c.CompanyCode
	   and g.CustomerCode = c.CustomerCodeBill
	  left join gnMstLookupDtl h
		on h.CompanyCode = b.CompanyCode
	   and h.LookupValue  = f.CityCode
	   and h.CodeId  = 'CITY'
	)#result					
	   
	if (@n = 1) 	
	begin		
		select * into #results from (select * from #result)x
		delete #results
	end
	if (@n > 0) 
		insert into #results select * from #result

	drop table #result
	drop table #fpj
	drop table #inv
	drop table #srv
	drop table #tsk
	drop table #itm
	drop table #oth
	drop table #itmSP
	drop table #itmMtrl
	drop table #itmRMtrl
	drop table #itmOli
	drop table #tpSp
	drop table #tpOli
	drop table #tpMt
	drop table #tpRMt
	delete @dtl
	delete @temp
end

if (@n > 0)
begin
	if (@FpjType = '3')	
	begin
		select distinct DetailDesc DD, *
		  , left(DetailDesc, 4) Detail1
		  , substring(DetailDesc, 4, 50) + right(left((DetailDesc + replicate(' ',75)), 75), 10) Detail2
		  , right(left((DetailDesc + replicate(' ',103)), 103), 15) Detail3
		  , case when (select count(FpjGovNo) from #results where FpjGovNo=a.FPJGovNo) = 19 then 17 else 18 end PageBreak   
		  , isnull((
				select top 1 isnull(ParaValue, 1) 
				  from gnMstLookupDtl
				 where CompanyCode = @CompanyCode
				   and CodeID = 'FPIF'
				   and LookUpValue = 'STATUS'
				), 1) IsShowSummary
		  , (PPNAmt + DPPAmt) TotPPNBm
		from #results a
		order by FPJGovNo asc, DetailDesc desc
	end
	else if (@FPJType = '8')	
	begin
		select distinct DetailDesc DD
		  , FpjGovNo
		  , CompanyCode
		  , CompanyName
		  , CompanyAddress
		  , NPWPNo
		  , SkpNo
		  , SkpDate
		  , FpjDate
		  , PoliceRegNo
		  , CustCodeBill
		  , CustNameBill
		  , CustAddr1Bill
		  , CustAddr2Bill
		  , CustAddr3Bill
		  , CustNpwpBill
		  , CustSkpBill
		  , right(rtrim(DetailDesc), len(rtrim(DetailDesc)) - 1) DetailDesc
		  , SalesAmt
		  , DiscAmt
		  , DPAmt
		  , DPPAmt
		  , PPNAmt
		  , DppBm
		  , PpnBm
		  , InvoiceNo
		  , CityName
		  , SignedDate
		  , JobOrderNo
		  , TotalAmt
		  , DueDate
		  , Asteric1
		  , Asteric3
		  , TotService
		  , TotOil
		  , TotDLL
		  , Asteric2
		  , ReportType
		  , xLocation
		  , yLocation
		  , TotService2
		  , left(DetailDesc, 4) Detail1
		  , substring(DetailDesc, 4, 50) + right(left((DetailDesc + replicate(' ',75)), 75), 10) Detail2
		  , right(left((DetailDesc + replicate(' ',103)), 103), 15) Detail3
		  , case when (select count(FpjGovNo) from #results where FpjGovNo=a.FPJGovNo) = 19 then 17 else 18 end PageBreak   
		  , isnull((
				select top 1 isnull(ParaValue, 1) 
				  from gnMstLookupDtl
				 where CompanyCode = @CompanyCode
				   and CodeID = 'FPIF'
				   and LookUpValue = 'STATUS'
				), 1) IsShowSummary
		  , (PPNAmt + DPPAmt) TotPPNBm
		from #results a
		order by FPJGovNo asc, a.DetailDesc		
	end
	else
	begin
		select DetailDesc DD, *
		  , left(DetailDesc, 4) Detail1
		  , substring(DetailDesc, 4, 50) + right(left((DetailDesc + replicate(' ',75)), 75), 10) Detail2
		  , right(left((DetailDesc + replicate(' ',85)), 85), 8)
		  + right(left((DetailDesc + replicate(' ',103)), 103), 15) Detail3
		  , case when (select count(FpjGovNo) from #results where FpjGovNo=a.FPJGovNo) = 19 then 17 else 18 end PageBreak   
		  , isnull((
				select top 1 isnull(ParaValue, 1) 
				  from gnMstLookupDtl
				 where CompanyCode = @CompanyCode
				   and CodeID = 'FPIF'
				   and LookUpValue = 'STATUS'
				), 1) IsShowSummary
		  , (PPNAmt + DPPAmt) TotPPNBm
		from #results a
		order by FPJGovNo
	end
		
	drop table #results
end

drop table #t_fpj

GO

PRINT 'ALTER SP DONE'

SET NOEXEC OFF --RESTORES NOEXEC SETTING TO ITS DEFAULT
PRINT 'DONE'
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


var start1 = function (cfg, callback) 
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