ALTER procedure [dbo].[usprpt_GnGenerateCsvSkemaTaxOut]
--DECLARE
	@CompanyCode varchar(15)
	,@BranchCodeFrom varchar(15)
	,@BranchCodeTo varchar(15)
	,@ProfitCenterCode varchar(3)
	,@PeriodFrom smalldatetime		
	,@PeriodTo smalldatetime
	,@ProductType varchar(2)
	,@table varchar(1)=1
AS

--execute usprpt_GnGenerateCsvSkemaTaxOut '6083401100','','','20150701','20150731','4W',1
--declare @nomorfaktur as varchar(100)
--select @CompanyCode=N'6115204001',@BranchCodeFrom=N'',@BranchCodeTo=N'',@ProfitCenterCode='200',@PeriodFrom='20150701',
--@PeriodTo='20150731',@ProductType=N'2W', @table='1', 
--@nomorfaktur = '0011592770301' 

IF @table = 1 OR @table = 3
BEGIN
DECLARE @TblTempDetail AS TABLE
(
	[OF] varchar(2),
	CompanyCode varchar(20),
	BranchCode varchar(20),
	cBranch int,
	KODE_OBJEK varchar(100),
	ProfitCenter varchar(3),
	NAMA varchar(max),
	HARGA_SATUAN decimal,
	JUMLAH_BARANG decimal(12,2),
	HARGA_TOTAL decimal,
	DISKON decimal,
	DPP decimal,
	sisaDPP decimal,
	sisaPPN decimal,
	PPN decimal,
	TARIF_PPNBM decimal,
	PPNBM decimal,
	CUSTOMERCODE varchar(100),
	NOMOR_FAKTUR varchar(100),
	FPJNO varchar(20),
	InvoiceNo varchar(20)
)

if @ProfitCenterCode = '000'
insert into @TblTempDetail
select 'OF' [OF]
	, a.CompanyCode
	, a.BranchCode
	, (select count(distinct BranchCode) from gnTaxOut where CompanyCode = a.CompanyCode and TaxNo = b.TaxNo) cBranch
	, AccountNo KODE_OBJEK
	, '000' ProfitCenter
	, Description NAMA
	, isnull ((convert(decimal(12,0),a.UnitPriceAmt)), 0) HARGA_SATUAN
	, isnull ((convert(decimal(12,0),a.Quantity)), 0) JUMLAH_BARANG
	, isnull ((convert(decimal(12,0),a.UnitPriceAmt * a.Quantity)),0) HARGA_TOTAL
	, isnull ((convert(decimal(12,0),a.DiscAmt)), 0) DISKON
	, FLOOR(a.UnitPriceAmt * a.Quantity) DPP
	, (a.UnitPriceAmt * a.Quantity) - CONVERT(DECIMAL,FLOOR(a.UnitPriceAmt * a.Quantity)) sisaDPP
	, ((a.UnitPriceAmt * a.Quantity)*0.1) -  CONVERT(DECIMAL,FLOOR((((a.UnitPriceAmt * a.Quantity))*0.1))) sisaPPN
	, FLOOR(((a.UnitPriceAmt * a.Quantity)*0.1)) PPN
	, 0 TARIF_PPNBM
	, 0 PPNBM
	, b.CUSTOMERCODE 
	, substring(REPLACE(REPLACE(b.TaxNo, '.', ''), '-', ''),4,13) NOMOR_FAKTUR
	, b.FPJNo
	, a.InvoiceNo
	from arTrnInvoiceDtl a
	inner join (select companycode, branchcode, fpjno, customercode, referenceNo, TaxNo
				from gntaxout 
				 WHERE CompanyCode = @CompanyCode
					AND (CASE WHEN @BranchCodeFrom = '' THEN '' ELSE BranchCode END) BETWEEN @BranchCodeFrom AND @BranchCodeTo
					AND CONVERT(VARCHAR, TaxDate, 112) BETWEEN @PeriodFrom AND @PeriodTo 
					AND IsPKP = 1
					AND DocumentType = 'F'
					AND (CASE WHEN @ProfitCenterCode = '' THEN '' ELSE ProfitCenter END) = @ProfitCenterCode
				) b
		on a.companycode = b.companycode and a.branchcode = b.branchcode
			and a.InvoiceNo = b.referenceNo
if @ProfitCenterCode = '100'
insert into @TblTempDetail
select 'OF' [OF]
	, a.CompanyCode
	, a.BranchCode
	, (select count(distinct BranchCode) from gnTaxOut where CompanyCode = a.CompanyCode and TaxNo = b.TaxNo) cBranch
	, a.SalesModelCode KODE_OBJEK
	, '100' ProfitCenter
	, 'Sales Model Desc : '+ (select SalesModelDesc from omMstModel where CompanyCode = a.CompanyCode and SalesModelCode = a.SalesModelCode)
	  +' Nomor Rangka : '+ CONVERT(varchar, c.ChassisNo,100)
	  +' Nomor Mesin : '+  CONVERT(varchar, c.EngineNo,100) NAMA
	, isnull ((convert(decimal(12,0),a.BeforeDiscDPP)), 0) HARGA_SATUAN
	, isnull ((convert(decimal(12,0),a.Quantity)), 0) JUMLAH_BARANG
	, isnull ((convert(decimal(12,0),a.BeforeDiscDPP)),0) HARGA_TOTAL
	, isnull ((convert(decimal(12,0),a.DiscExcludePPn)), 0) DISKON
	, FLOOR(a.BeforeDiscDPP - a.DiscExcludePPn) DPP
	, (a.BeforeDiscDPP - a.DiscExcludePPn) - CONVERT(DECIMAL,FLOOR(a.BeforeDiscDPP - a.DiscExcludePPn)) sisaDPP
	, ((a.BeforeDiscDPP - a.DiscExcludePPn)*0.1) -  CONVERT(DECIMAL,FLOOR((((a.BeforeDiscDPP - a.DiscExcludePPn))*0.1))) sisaPPN
	, ROUND(((a.BeforeDiscDPP - a.DiscExcludePPn)*0.1),0) PPN
	, 0 TARIF_PPNBM
	, 0 PPNBM
	, b.CUSTOMERCODE 
	, substring(REPLACE(REPLACE(b.TaxNo, '.', ''), '-', ''),4,13) TaxNo
	, b.FPJNo
	, a.InvoiceNo
from omTrSalesInvoicemodel a
inner join (select companycode, branchcode, fpjno, customercode, referenceNo, TaxNo, DPPAmt
			from gntaxout 
			 WHERE  CompanyCode = @CompanyCode
				AND (CASE WHEN @BranchCodeFrom = '' THEN '' ELSE BranchCode END) BETWEEN @BranchCodeFrom AND @BranchCodeTo
				AND CONVERT(VARCHAR, TaxDate, 112) BETWEEN @PeriodFrom AND @PeriodTo 
				AND IsPKP = 1
				AND DocumentType = 'F'
				AND (CASE WHEN @ProfitCenterCode = '' THEN '' ELSE ProfitCenter END) = @ProfitCenterCode
			) b
	on a.companycode = b.companycode and a.branchcode = b.branchcode
		and a.InvoiceNo = b.referenceNo	
inner join omTrSalesInvoiceVin c on a.CompanyCode = c.CompanyCode
	and a.BranchCode = c.BranchCode
	and a.InvoiceNo = c.InvoiceNo
	and a.BPKNo = c.BPKNo
	and a.SalesModelCode = c.SalesModelCode
	and a.SalesModelYear = c.SalesModelYear
if @ProfitCenterCode = '200'
insert into @TblTempDetail
select 'OF' [OF]
		, a.CompanyCode
		, a.BranchCode
		, (select count(distinct BranchCode) from gnTaxOut where CompanyCode = a.CompanyCode and TaxNo = b.TaxNo) cBranch
		, rtrim(PartNo) KODE_OBJEK
		, '200' ProfitCenter
		, (select replace(rtrim(Partname),',',' ') from spmstiteminfo where companycode = a.companycode and partno = a.partno) NAMA
		, isnull (convert(decimal(12,2),a.RetailPrice,0),0.00) HARGA_SATUAN
		, isnull (convert(decimal(12,2),a.SupplyQty,0),0.00) JUMLAH_BARANG
		, isnull (convert(decimal(12,2),a.RetailPrice * a.SupplyQty,0),0.00) HARGA_TOTAL
		, isnull (convert(decimal(12,2),a.RetailPrice * a.SupplyQty * DiscPct /100.00,0),0.00) DISKON
		, FLOOR((a.RetailPrice * a.SupplyQty)-(a.RetailPrice * a.SupplyQty * DiscPct /100)) DPP
		, ((a.RetailPrice * a.SupplyQty)-(a.RetailPrice * a.SupplyQty * DiscPct /100)) - CONVERT(DECIMAL,FLOOR((a.RetailPrice * a.SupplyQty)-(a.RetailPrice * a.SupplyQty * DiscPct /100))) sisaDPP
		, (((a.RetailPrice * a.SupplyQty)-(a.RetailPrice * a.SupplyQty * DiscPct /100))*0.1) -  CONVERT(DECIMAL,FLOOR(((((a.RetailPrice * a.SupplyQty)-(a.RetailPrice * a.SupplyQty * DiscPct /100)))*0.1))) sisaPPN
		, FLOOR((((a.RetailPrice * a.SupplyQty)-(a.RetailPrice * a.SupplyQty * DiscPct /100))*0.1)) PPN
		, 0 TARIF_PPNBM
		, 0 PPNBM
		, b.CUSTOMERCODE 
		, substring(REPLACE(REPLACE(b.TaxNo, '.', ''), '-', ''),4,13) NOMOR_FAKTUR
		, b.FPJNo
		, a.InvoiceNo
		from svTrnInvItem a
		inner join (select CompanyCode, BranchCode, FPJNo, CustomerCode, ReferenceNo, TaxNo
					from gntaxout 
					 WHERE  CompanyCode = @CompanyCode
									AND (CASE WHEN @BranchCodeFrom = '' THEN '' ELSE BranchCode END) BETWEEN @BranchCodeFrom AND @BranchCodeTo
									AND CONVERT(VARCHAR, TaxDate, 112) BETWEEN @PeriodFrom AND @PeriodTo 
									AND IsPKP = 1
									AND DocumentType = 'F'
									AND (CASE WHEN @ProfitCenterCode = '' THEN '' ELSE ProfitCenter END) = @ProfitCenterCode
					) b
			on a.companycode = b.companycode and a.branchcode = b.branchcode
				and a.InvoiceNo = b.referenceNo	
		inner join (select CompanyCode, BranchCode, TaxNo, COUNT(ReferenceNo) countReff
					from gntaxout 
					 WHERE  CompanyCode = @CompanyCode
									AND (CASE WHEN @BranchCodeFrom = '' THEN '' ELSE BranchCode END) BETWEEN @BranchCodeFrom AND @BranchCodeTo
									AND CONVERT(VARCHAR, TaxDate, 112) BETWEEN @PeriodFrom AND @PeriodTo 
									AND IsPKP = 1
									AND DocumentType = 'F'
									AND (CASE WHEN @ProfitCenterCode = '' THEN '' ELSE ProfitCenter END) = @ProfitCenterCode
					group by CompanyCode, BranchCode, TaxNo
					) c
			on c.CompanyCode = b.CompanyCode and c.branchcode = b.branchcode
				and c.TaxNo = b.TaxNo	
		union all 
		select 'OF' [OF]
		, a.CompanyCode
		, a.BranchCode
		, (select count(distinct BranchCode) from gnTaxOut where CompanyCode = a.CompanyCode and TaxNo = b.TaxNo) cBranch
		, rtrim(OperationNo) KODE_OBJEK
		, '200' ProfitCenter
		, (select top 1 replace(rtrim(Description),',',' ') from svMstTask 
			where CompanyCode=a.CompanyCode and BasicModel=x.BasicModel and OperationNo=a.OperationNo
		  ) NAMA
		, isnull (convert(decimal(12,2),a.OperationCost,0),0.00) HARGA_SATUAN
		, isnull (convert(decimal(12,2),a.OperationHour,0),0.00) JUMLAH_BARANG
		, isnull (convert(decimal(12,2),a.OperationCost * a.OperationHour,0),0.00) HARGA_TOTAL
		, isnull (convert(decimal(12,2),a.OperationCost * a.OperationHour * DiscPct /100.00,0),0.00) DISKON
		, FLOOR((a.OperationCost * a.OperationHour)-(a.OperationCost * a.OperationHour * DiscPct /100)) DPP
		, ((a.OperationCost * a.OperationHour)-(a.OperationCost * a.OperationHour * DiscPct /100)) - CONVERT(DECIMAL,FLOOR(((a.OperationCost * a.OperationHour)-(a.OperationCost * a.OperationHour * DiscPct /100)))) sisaDPP
		, (((a.OperationCost * a.OperationHour)-(a.OperationCost * a.OperationHour * DiscPct /100))*0.1) -  CONVERT(DECIMAL,FLOOR((((a.OperationCost * a.OperationHour)-(a.OperationCost * a.OperationHour * DiscPct /100))*0.1))) sisaPPN
		, FLOOR((((a.OperationCost * a.OperationHour)-(a.OperationCost * a.OperationHour * DiscPct /100))*0.1)) PPN
		, 0 TARIF_PPNBM
		, 0 PPNBM
		, b.CUSTOMERCODE 
		, substring(REPLACE(REPLACE(b.TaxNo, '.', ''), '-', ''),4,13) NOMOR_FAKTUR
		, b.FPJNo
		, a.InvoiceNo
		from svTrnInvTask a
		inner join svTrnInvoice x
			on a.CompanyCode=x.CompanyCode and a.BranchCode=x.BranchCode and a.InvoiceNo=x.InvoiceNo
		inner join (select companycode, branchcode, fpjno, customercode, referenceNo, TaxNo
					from gntaxout 
					 WHERE  CompanyCode = @CompanyCode
									AND (CASE WHEN @BranchCodeFrom = '' THEN '' ELSE BranchCode END) BETWEEN @BranchCodeFrom AND @BranchCodeTo
									AND CONVERT(VARCHAR, TaxDate, 112) BETWEEN @PeriodFrom AND @PeriodTo 
									AND IsPKP = 1
									AND DocumentType = 'F'
									AND (CASE WHEN @ProfitCenterCode = '' THEN '' ELSE ProfitCenter END) = @ProfitCenterCode
					) b
			on a.companycode = b.companycode and a.branchcode = b.branchcode
				and a.InvoiceNo = b.referenceNo	
		inner join (select CompanyCode, BranchCode, TaxNo, COUNT(ReferenceNo) countReff
					from gntaxout 
					 WHERE  CompanyCode = @CompanyCode
									AND (CASE WHEN @BranchCodeFrom = '' THEN '' ELSE BranchCode END) BETWEEN @BranchCodeFrom AND @BranchCodeTo
									AND CONVERT(VARCHAR, TaxDate, 112) BETWEEN @PeriodFrom AND @PeriodTo 
									AND IsPKP = 1
									AND DocumentType = 'F'
									AND (CASE WHEN @ProfitCenterCode = '' THEN '' ELSE ProfitCenter END) = @ProfitCenterCode
					group by CompanyCode, BranchCode, TaxNo
					) c
			on c.CompanyCode = b.CompanyCode and c.branchcode = b.branchcode
				and c.TaxNo = b.TaxNo
if @ProfitCenterCode = '300'
insert into @TblTempDetail 
select 'OF' [OF]
	, a.CompanyCode
	, a.BranchCode
	, (select count(distinct BranchCode) from gnTaxOut where CompanyCode = a.CompanyCode and TaxNo = b.TaxNo) cBranch
	, PartNo KODE_OBJEK
	, '300' ProfitCenter
	, (select replace(rtrim(PartName),',',' ') from spMstItemInfo where CompanyCode = a.CompanyCode and PartNo = a.PartNo) NAMA
	, isnull ((convert(decimal(12,2),a.retailprice)), 0.00) HARGA_SATUAN
	, isnull ((convert(decimal(12,2),a.QtyBill)), 0.00) JUMLAH_BARANG
	, isnull ((convert(decimal(12,2),a.SalesAmt)), 0.00)HARGA_TOTAL
	, isnull ((convert(decimal(12,2),a.DiscAmt)), 0.00) DISKON
	, FLOOR(a.NetSalesAmt) DPP
	, a.NetSalesAmt - CONVERT(DECIMAL,FLOOR(a.NetSalesAmt)) sisaDPP
	, (a.NetSalesAmt*0.1) -  CONVERT(DECIMAL,FLOOR(a.NetSalesAmt*0.1)) sisaPPN
	, FLOOR(a.NetSalesAmt*0.1) PPN
	, 0 TARIF_PPNBM
	, 0 PPNBM
	, b.CUSTOMERCODE 
	, substring(REPLACE(REPLACE(b.TaxNo, '.', ''), '-', ''),4,13) NOMOR_FAKTUR
	, b.FPJNo
	, a.InvoiceNo
	from spTrnSInvoiceDtl a
	inner join (select CompanyCode, BranchCode, FPJNo, CustomerCode, ReferenceNo, TaxNo
				from gnTaxOut 
				 WHERE  CompanyCode = @CompanyCode
								AND (CASE WHEN @BranchCodeFrom = '' THEN '' ELSE BranchCode END) BETWEEN @BranchCodeFrom AND @BranchCodeTo
								AND CONVERT(VARCHAR, TaxDate, 112) BETWEEN @PeriodFrom AND @PeriodTo 
								AND IsPKP = 1
								AND DocumentType = 'F'
								AND (CASE WHEN @ProfitCenterCode = '' THEN '' ELSE ProfitCenter END) = @ProfitCenterCode
				) b
		on a.CompanyCode = b.CompanyCode and a.BranchCode = b.BranchCode
			and a.InvoiceNo = b.ReferenceNo
if @ProfitCenterCode = ''
insert into @TblTempDetail
select 'OF' [OF]
	, a.CompanyCode
	, a.BranchCode
	, (select count(distinct BranchCode) from gnTaxOut where CompanyCode = a.CompanyCode and TaxNo = b.TaxNo) cBranch
	, AccountNo KODE_OBJEK
	, '000' ProfitCenter
	, Description NAMA
	, isnull ((convert(decimal(12,0),a.UnitPriceAmt)), 0) HARGA_SATUAN
	, isnull ((convert(decimal(12,0),a.Quantity)), 0) JUMLAH_BARANG
	, isnull ((convert(decimal(12,0),a.UnitPriceAmt * a.Quantity)),0) HARGA_TOTAL
	, isnull ((convert(decimal(12,0),a.DiscAmt)), 0) DISKON
	, FLOOR(a.UnitPriceAmt * a.Quantity) DPP
	, (a.UnitPriceAmt * a.Quantity) - CONVERT(DECIMAL,FLOOR(a.UnitPriceAmt * a.Quantity)) sisaDPP
	, ((a.UnitPriceAmt * a.Quantity)*0.1) -  CONVERT(DECIMAL,FLOOR((((a.UnitPriceAmt * a.Quantity))*0.1))) sisaPPN
	, FLOOR(((a.UnitPriceAmt * a.Quantity)*0.1)) PPN
	, 0 TARIF_PPNBM
	, 0 PPNBM
	, b.CUSTOMERCODE 
	, substring(REPLACE(REPLACE(b.TaxNo, '.', ''), '-', ''),4,13) NOMOR_FAKTUR
	, b.FPJNo
	, a.InvoiceNo
	from arTrnInvoiceDtl a
	inner join (select companycode, branchcode, fpjno, customercode, referenceNo, TaxNo
				from gntaxout 
				 WHERE CompanyCode = @CompanyCode
					AND (CASE WHEN @BranchCodeFrom = '' THEN '' ELSE BranchCode END) BETWEEN @BranchCodeFrom AND @BranchCodeTo
					AND CONVERT(VARCHAR, TaxDate, 112) BETWEEN @PeriodFrom AND @PeriodTo 
					AND IsPKP = 1
					AND DocumentType = 'F'
					AND (CASE WHEN @ProfitCenterCode = '' THEN '' ELSE ProfitCenter END) = @ProfitCenterCode
				) b
		on a.companycode = b.companycode and a.branchcode = b.branchcode
			and a.InvoiceNo = b.referenceNo
union all
select 'OF' [OF]
	, a.CompanyCode
	, a.BranchCode
	, (select count(distinct BranchCode) from gnTaxOut where CompanyCode = a.CompanyCode and TaxNo = b.TaxNo) cBranch
	, a.SalesModelCode KODE_OBJEK
	, '100' ProfitCenter
	, 'Sales Model Desc : '+ (select SalesModelDesc from omMstModel where CompanyCode = a.CompanyCode and SalesModelCode = a.SalesModelCode)
	  +' Nomor Rangka : '+ CONVERT(varchar, c.ChassisNo,100)
	  +' Nomor Mesin : '+  CONVERT(varchar, c.EngineNo,100) NAMA
	, isnull ((convert(decimal(12,0),a.BeforeDiscDPP)), 0) HARGA_SATUAN
	, isnull ((convert(decimal(12,0),a.Quantity)), 0) JUMLAH_BARANG
	, isnull ((convert(decimal(12,0),a.BeforeDiscDPP)),0) HARGA_TOTAL
	, isnull ((convert(decimal(12,0),a.DiscExcludePPn)), 0) DISKON
	, FLOOR(a.BeforeDiscDPP - a.DiscExcludePPn) DPP
	, (a.BeforeDiscDPP - a.DiscExcludePPn) - CONVERT(DECIMAL,FLOOR(a.BeforeDiscDPP - a.DiscExcludePPn)) sisaDPP
	, ((a.BeforeDiscDPP - a.DiscExcludePPn)*0.1) -  CONVERT(DECIMAL,FLOOR((((a.BeforeDiscDPP - a.DiscExcludePPn))*0.1))) sisaPPN
	, FLOOR(((a.BeforeDiscDPP - a.DiscExcludePPn)*0.1)) PPN
	, 0 TARIF_PPNBM
	, 0 PPNBM
	, b.CUSTOMERCODE 
	, substring(REPLACE(REPLACE(b.TaxNo, '.', ''), '-', ''),4,13) TaxNo
	, b.FPJNo
	, a.InvoiceNo
from omTrSalesInvoicemodel a
inner join (select companycode, branchcode, fpjno, customercode, referenceNo, TaxNo, DPPAmt
			from gntaxout 
			 WHERE  CompanyCode = @CompanyCode
				AND (CASE WHEN @BranchCodeFrom = '' THEN '' ELSE BranchCode END) BETWEEN @BranchCodeFrom AND @BranchCodeTo
				AND CONVERT(VARCHAR, TaxDate, 112) BETWEEN @PeriodFrom AND @PeriodTo 
				AND IsPKP = 1
				AND DocumentType = 'F'
				AND (CASE WHEN @ProfitCenterCode = '' THEN '' ELSE ProfitCenter END) = @ProfitCenterCode
			) b
	on a.companycode = b.companycode and a.branchcode = b.branchcode
		and a.InvoiceNo = b.referenceNo	
inner join omTrSalesInvoiceVin c on a.CompanyCode = c.CompanyCode
	and a.BranchCode = c.BranchCode
	and a.InvoiceNo = c.InvoiceNo
	and a.BPKNo = c.BPKNo
	and a.SalesModelCode = c.SalesModelCode
	and a.SalesModelYear = c.SalesModelYear
union all
select 'OF' [OF]
		, a.CompanyCode
		, a.BranchCode
		, (select count(distinct BranchCode) from gnTaxOut where CompanyCode = a.CompanyCode and TaxNo = b.TaxNo) cBranch
		, rtrim(PartNo) KODE_OBJEK
		, '200' ProfitCenter
		, (select replace(rtrim(Partname),',',' ') from spmstiteminfo where companycode = a.companycode and partno = a.partno) NAMA
		, isnull (convert(decimal(12,2),a.RetailPrice,0),0.00) HARGA_SATUAN
		, isnull (convert(decimal(12,2),a.SupplyQty,0),0.00) JUMLAH_BARANG
		, isnull (convert(decimal(12,2),a.RetailPrice * a.SupplyQty,0),0.00) HARGA_TOTAL
		, isnull (convert(decimal(12,2),a.RetailPrice * a.SupplyQty * DiscPct /100.00,0),0.00) DISKON
		, FLOOR((a.RetailPrice * a.SupplyQty)-(a.RetailPrice * a.SupplyQty * DiscPct /100)) DPP
		, ((a.RetailPrice * a.SupplyQty)-(a.RetailPrice * a.SupplyQty * DiscPct /100)) - CONVERT(DECIMAL,FLOOR((a.RetailPrice * a.SupplyQty)-(a.RetailPrice * a.SupplyQty * DiscPct /100))) sisaDPP
		, (((a.RetailPrice * a.SupplyQty)-(a.RetailPrice * a.SupplyQty * DiscPct /100))*0.1) -  CONVERT(DECIMAL,FLOOR(((((a.RetailPrice * a.SupplyQty)-(a.RetailPrice * a.SupplyQty * DiscPct /100)))*0.1))) sisaPPN
		, FLOOR((((a.RetailPrice * a.SupplyQty)-(a.RetailPrice * a.SupplyQty * DiscPct /100))*0.1)) PPN
		, 0 TARIF_PPNBM
		, 0 PPNBM
		, b.CUSTOMERCODE 
		, substring(REPLACE(REPLACE(b.TaxNo, '.', ''), '-', ''),4,13) NOMOR_FAKTUR
		, b.FPJNo
		, a.InvoiceNo
		from svTrnInvItem a
		inner join (select CompanyCode, BranchCode, FPJNo, CustomerCode, ReferenceNo, TaxNo
					from gntaxout 
					 WHERE  CompanyCode = @CompanyCode
									AND (CASE WHEN @BranchCodeFrom = '' THEN '' ELSE BranchCode END) BETWEEN @BranchCodeFrom AND @BranchCodeTo
									AND CONVERT(VARCHAR, TaxDate, 112) BETWEEN @PeriodFrom AND @PeriodTo 
									AND IsPKP = 1
									AND DocumentType = 'F'
									AND (CASE WHEN @ProfitCenterCode = '' THEN '' ELSE ProfitCenter END) = @ProfitCenterCode
					) b
			on a.companycode = b.companycode and a.branchcode = b.branchcode
				and a.InvoiceNo = b.referenceNo	
		inner join (select CompanyCode, BranchCode, TaxNo, COUNT(ReferenceNo) countReff
					from gntaxout 
					 WHERE  CompanyCode = @CompanyCode
									AND (CASE WHEN @BranchCodeFrom = '' THEN '' ELSE BranchCode END) BETWEEN @BranchCodeFrom AND @BranchCodeTo
									AND CONVERT(VARCHAR, TaxDate, 112) BETWEEN @PeriodFrom AND @PeriodTo 
									AND IsPKP = 1
									AND DocumentType = 'F'
									AND (CASE WHEN @ProfitCenterCode = '' THEN '' ELSE ProfitCenter END) = @ProfitCenterCode
					group by CompanyCode, BranchCode, TaxNo
					) c
			on c.CompanyCode = b.CompanyCode and c.branchcode = b.branchcode
				and c.TaxNo = b.TaxNo	
		union all 
		select 'OF' [OF]
		, a.CompanyCode
		, a.BranchCode
		, (select count(distinct BranchCode) from gnTaxOut where CompanyCode = a.CompanyCode and TaxNo = b.TaxNo) cBranch
		, rtrim(OperationNo) KODE_OBJEK
		, '200' ProfitCenter
		, (select top 1 replace(rtrim(Description),',',' ') from svMstTask 
			where CompanyCode=a.CompanyCode and BasicModel=x.BasicModel and OperationNo=a.OperationNo
		  ) NAMA
		, isnull (convert(decimal(12,2),a.OperationCost,0),0.00) HARGA_SATUAN
		, isnull (convert(decimal(12,2),a.OperationHour,0),0.00) JUMLAH_BARANG
		, isnull (convert(decimal(12,2),a.OperationCost * a.OperationHour,0),0.00) HARGA_TOTAL
		, isnull (convert(decimal(12,2),a.OperationCost * a.OperationHour * DiscPct /100.00,0),0.00) DISKON
		, FLOOR((a.OperationCost * a.OperationHour)-(a.OperationCost * a.OperationHour * DiscPct /100)) DPP
		, ((a.OperationCost * a.OperationHour)-(a.OperationCost * a.OperationHour * DiscPct /100)) - CONVERT(DECIMAL,FLOOR(((a.OperationCost * a.OperationHour)-(a.OperationCost * a.OperationHour * DiscPct /100)))) sisaDPP
		, (((a.OperationCost * a.OperationHour)-(a.OperationCost * a.OperationHour * DiscPct /100))*0.1) -  CONVERT(DECIMAL,FLOOR((((a.OperationCost * a.OperationHour)-(a.OperationCost * a.OperationHour * DiscPct /100))*0.1))) sisaPPN
		, FLOOR((((a.OperationCost * a.OperationHour)-(a.OperationCost * a.OperationHour * DiscPct /100))*0.1)) PPN
		, 0 TARIF_PPNBM
		, 0 PPNBM
		, b.CUSTOMERCODE 
		, substring(REPLACE(REPLACE(b.TaxNo, '.', ''), '-', ''),4,13) NOMOR_FAKTUR
		, b.FPJNo
		, a.InvoiceNo
		from svTrnInvTask a
		inner join svTrnInvoice x
			on a.CompanyCode=x.CompanyCode and a.BranchCode=x.BranchCode and a.InvoiceNo=x.InvoiceNo
		inner join (select companycode, branchcode, fpjno, customercode, referenceNo, TaxNo
					from gntaxout 
					 WHERE  CompanyCode = @CompanyCode
									AND (CASE WHEN @BranchCodeFrom = '' THEN '' ELSE BranchCode END) BETWEEN @BranchCodeFrom AND @BranchCodeTo
									AND CONVERT(VARCHAR, TaxDate, 112) BETWEEN @PeriodFrom AND @PeriodTo 
									AND IsPKP = 1
									AND DocumentType = 'F'
									AND (CASE WHEN @ProfitCenterCode = '' THEN '' ELSE ProfitCenter END) = @ProfitCenterCode
					) b
			on a.companycode = b.companycode and a.branchcode = b.branchcode
				and a.InvoiceNo = b.referenceNo	
		inner join (select CompanyCode, BranchCode, TaxNo, COUNT(ReferenceNo) countReff
					from gntaxout 
					 WHERE  CompanyCode = @CompanyCode
									AND (CASE WHEN @BranchCodeFrom = '' THEN '' ELSE BranchCode END) BETWEEN @BranchCodeFrom AND @BranchCodeTo
									AND CONVERT(VARCHAR, TaxDate, 112) BETWEEN @PeriodFrom AND @PeriodTo 
									AND IsPKP = 1
									AND DocumentType = 'F'
									AND (CASE WHEN @ProfitCenterCode = '' THEN '' ELSE ProfitCenter END) = @ProfitCenterCode
					group by CompanyCode, BranchCode, TaxNo
					) c
			on c.CompanyCode = b.CompanyCode and c.branchcode = b.branchcode
				and c.TaxNo = b.TaxNo
union all
select 'OF' [OF]
	, a.CompanyCode
	, a.BranchCode
	, (select count(distinct BranchCode) from gnTaxOut where CompanyCode = a.CompanyCode and TaxNo = b.TaxNo) cBranch
	, PartNo KODE_OBJEK
	, '300' ProfitCenter
	, (select replace(rtrim(PartName),',',' ') from spMstItemInfo where CompanyCode = a.CompanyCode and PartNo = a.PartNo) NAMA
	, isnull ((convert(decimal(12,2),a.retailprice)), 0.00) HARGA_SATUAN
	, isnull ((convert(decimal(12,2),a.QtyBill)), 0.00) JUMLAH_BARANG
	, isnull ((convert(decimal(12,2),a.SalesAmt)), 0.00)HARGA_TOTAL
	, isnull ((convert(decimal(12,2),a.DiscAmt)), 0.00) DISKON
	, FLOOR(a.NetSalesAmt) DPP
	, a.NetSalesAmt - CONVERT(DECIMAL,FLOOR(a.NetSalesAmt)) sisaDPP
	, (a.NetSalesAmt*0.1) -  CONVERT(DECIMAL,FLOOR(a.NetSalesAmt*0.1)) sisaPPN
	, FLOOR(a.NetSalesAmt*0.1) PPN
	, 0 TARIF_PPNBM
	, 0 PPNBM
	, b.CUSTOMERCODE 
	, substring(REPLACE(REPLACE(b.TaxNo, '.', ''), '-', ''),4,13) NOMOR_FAKTUR
	, b.FPJNo
	, a.InvoiceNo
	from spTrnSInvoiceDtl a
	inner join (select CompanyCode, BranchCode, FPJNo, CustomerCode, ReferenceNo, TaxNo
				from gnTaxOut 
				 WHERE  CompanyCode = @CompanyCode
								AND (CASE WHEN @BranchCodeFrom = '' THEN '' ELSE BranchCode END) BETWEEN @BranchCodeFrom AND @BranchCodeTo
								AND CONVERT(VARCHAR, TaxDate, 112) BETWEEN @PeriodFrom AND @PeriodTo 
								AND IsPKP = 1
								AND DocumentType = 'F'
								AND (CASE WHEN @ProfitCenterCode = '' THEN '' ELSE ProfitCenter END) = @ProfitCenterCode
				) b
		on a.CompanyCode = b.CompanyCode and a.BranchCode = b.BranchCode
			and a.InvoiceNo = b.ReferenceNo

UPDATE a 
SET a.DPP = a.DPP + a.sisaDPP
--select *
FROM
(SELECT a.CompanyCode, a.BranchCode, a.NOMOR_FAKTUR, b.InvoiceNo, b.KodeTempDPP, a.DPP, ROUND(b.sisaDPP,0) sisaDPP
FROM @TblTempDetail a
INNER JOIN
(SELECT a.CompanyCode, a.BranchCode, a.NOMOR_FAKTUR,
(SELECT TOP 1 KODE_OBJEK FROM @TblTempDetail WHERE CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and NOMOR_FAKTUR = a.NOMOR_FAKTUR and InvoiceNo = a.InvoiceNo ORDER BY DPP) KodeTempDPP, 
a.InvoiceNo, ROUND(SUM(sisaDPP),0) sisaDPP
FROM @TblTempDetail a
GROUP BY a.CompanyCode, a.BranchCode, a.NOMOR_FAKTUR, a.InvoiceNo, a.ProfitCenter
having ROUND(SUM(sisaDPP),0) > 0 and ProfitCenter != '100') b
ON a.CompanyCode = b.CompanyCode
AND a.BranchCode = b.BranchCode
AND a.NOMOR_FAKTUR = b.NOMOR_FAKTUR
AND a.KODE_OBJEK = b.KodeTempDPP
AND a.InvoiceNo = b.InvoiceNo) a

UPDATE a 
SET a.PPN = a.PPN + a.sisaPPN
FROM
(SELECT a.CompanyCode, a.BranchCode, a.NOMOR_FAKTUR, b.InvoiceNo, b.KodeTempPPN, a.PPN,  ROUND(b.sisaPPN,0) sisaPPN
FROM @TblTempDetail a
INNER JOIN
(SELECT a.CompanyCode, a.BranchCode, a.NOMOR_FAKTUR,
(SELECT TOP 1 KODE_OBJEK FROM @TblTempDetail WHERE CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and NOMOR_FAKTUR = a.NOMOR_FAKTUR and InvoiceNo = a.InvoiceNo ORDER BY PPN) KodeTempPPN, 
a.InvoiceNo, ROUND(SUM(sisaPPN),0) sisaPPN
FROM @TblTempDetail a
GROUP BY a.CompanyCode, a.BranchCode, a.NOMOR_FAKTUR, a.InvoiceNo, a.ProfitCenter
having ROUND(SUM(sisaPPN),0) > 0 and ProfitCenter != '100') b
ON a.CompanyCode = b.CompanyCode
AND a.BranchCode = b.BranchCode
AND a.NOMOR_FAKTUR = b.NOMOR_FAKTUR
AND a.KODE_OBJEK = b.KodeTempPPN
AND a.InvoiceNo = b.InvoiceNo) a

SELECT * INTO #t3 FROM
(
SELECT a1.CompanyCode, a1.BranchCode, a1.cBranch, a1.NOMOR_FAKTUR, a1.ProfitCenter, 
(SELECT TOP 1 InvoiceNo FROM @TblTempDetail WHERE CompanyCode = a1.CompanyCode and BranchCode = a1.BranchCode and NOMOR_FAKTUR = a1.NOMOR_FAKTUR ORDER BY PPN) InvoiceNo,
case when a1.cBranch > 1 then (SELECT TOP 1 KODE_OBJEK FROM @TblTempDetail WHERE CompanyCode = b.CompanyCode and BranchCode = b.BranchCode and NOMOR_FAKTUR = a1.NOMOR_FAKTUR 
and InvoiceNo = (SELECT TOP 1 InvoiceNo FROM @TblTempDetail WHERE CompanyCode = a1.CompanyCode and BranchCode = a1.BranchCode and NOMOR_FAKTUR = a1.NOMOR_FAKTUR ORDER BY PPN)) 
ELSE (SELECT TOP 1 KODE_OBJEK FROM @TblTempDetail WHERE CompanyCode = b.CompanyCode and BranchCode = b.BranchCode and NOMOR_FAKTUR = a1.NOMOR_FAKTUR 
and InvoiceNo = (SELECT TOP 1 InvoiceNo FROM @TblTempDetail WHERE CompanyCode = a1.CompanyCode and BranchCode = a1.BranchCode and NOMOR_FAKTUR = a1.NOMOR_FAKTUR ORDER BY PPN)) END KodeTempPPN, 
case when a1.cBranch > 1 then ((case when b.TotalPpnAmt > a1.JUMLAH_PPN then b.TotalPpnAmt-a1.JUMLAH_PPN else a1.JUMLAH_PPN - b.TotalPpnAmt end)) else (case when b.TotalPpnAmt > a1.JUMLAH_PPN then b.TotalPpnAmt-a1.JUMLAH_PPN else a1.JUMLAH_PPN - b.TotalPpnAmt end) end selisihPPN,
case when a1.cBranch > 1 then ((case when b.TotalPpnAmt > a1.JUMLAH_PPN then '+' else '-' end)) else (case when b.TotalPpnAmt > a1.JUMLAH_PPN then '+' else '-' end) end plusMinPPN
FROM(
SELECT a.CompanyCode, a.BranchCode, (SELECT TOP 1 cBranch FROM @TblTempDetail WHERE CompanyCode = a.CompanyCode AND NOMOR_FAKTUR = a.NOMOR_FAKTUR) cBranch,
NOMOR_FAKTUR, ProfitCenter, SUM(PPN) JUMLAH_PPN
FROM @TblTempDetail a
--WHERE ProfitCenter in ('300','100','200')
GROUP BY a.CompanyCode, a.BranchCode, NOMOR_FAKTUR, ProfitCenter
) a1
INNER join 
	(
	select a.CompanyCode, a.BranchCode, substring(REPLACE(REPLACE(TaxNo, '.', ''), '-', ''),4,13) TaxNo, SUM(PPNAmt) TotalPPNAmt
	from gnTaxOut a
	where 
	a.CompanyCode = @CompanyCode
	and (CASE WHEN @BranchCodeFrom = '' THEN '' ELSE a.BranchCode END) BETWEEN @BranchCodeFrom AND @BranchCodeTo
	group by a.CompanyCode, a.BranchCode, substring(REPLACE(REPLACE(TaxNo, '.', ''), '-', ''),4,13)
	) b
		on b.CompanyCode = a1.CompanyCode
		and b.BranchCode = a1.BranchCode
		and b.TaxNo = a1.NOMOR_FAKTUR
where a1.JUMLAH_PPN!= b.TotalPPNAmt 
and (case when a1.cBranch > 1 then ((case when b.TotalPpnAmt > a1.JUMLAH_PPN then b.TotalPpnAmt-a1.JUMLAH_PPN else a1.JUMLAH_PPN - b.TotalPpnAmt end)) else (case when b.TotalPpnAmt > a1.JUMLAH_PPN then b.TotalPpnAmt-a1.JUMLAH_PPN else a1.JUMLAH_PPN - b.TotalPpnAmt end) end) > 0
) #t3

IF (SELECT COUNT(*) FROM #t3) > 1
UPDATE a 
SET a.PPN = (CASE WHEN a.plusMinPPN = '+' THEN a.PPN + a.sisaPPN ELSE a.PPN - a.sisaPPN END)
FROM(
select a.CompanyCode, a.BranchCode, a.NOMOR_FAKTUR, b.KodeTempPPN, b.InvoiceNo, a.PPN, ROUND(b.selisihPPN,0) sisaPPN, b.plusMinPPN
FROM @TblTempDetail a
INNER JOIN #t3 b ON
a.CompanyCode = b.CompanyCode
AND a.BranchCode = b.BranchCode
AND a.NOMOR_FAKTUR = b.NOMOR_FAKTUR 
AND a.KODE_OBJEK = b.KodeTempPPN
AND a.InvoiceNo = b.InvoiceNo
) a

DROP TABLE #t3

SELECT * INTO #t4 FROM
(
SELECT a1.CompanyCode, a1.BranchCode, a1.cBranch, a1.NOMOR_FAKTUR, a1.ProfitCenter, 
(SELECT TOP 1 InvoiceNo FROM @TblTempDetail WHERE CompanyCode = a1.CompanyCode and BranchCode = a1.BranchCode and NOMOR_FAKTUR = a1.NOMOR_FAKTUR ORDER BY PPN) InvoiceNo,
case when a1.cBranch > 1 then (SELECT TOP 1 KODE_OBJEK FROM @TblTempDetail WHERE CompanyCode = b.CompanyCode and BranchCode = b.BranchCode and NOMOR_FAKTUR = a1.NOMOR_FAKTUR 
and InvoiceNo = (SELECT TOP 1 InvoiceNo FROM @TblTempDetail WHERE CompanyCode = a1.CompanyCode and BranchCode = a1.BranchCode and NOMOR_FAKTUR = a1.NOMOR_FAKTUR ORDER BY DPP)) 
ELSE (SELECT TOP 1 KODE_OBJEK FROM @TblTempDetail WHERE CompanyCode = b.CompanyCode and BranchCode = b.BranchCode and NOMOR_FAKTUR = a1.NOMOR_FAKTUR 
and InvoiceNo = (SELECT TOP 1 InvoiceNo FROM @TblTempDetail WHERE CompanyCode = a1.CompanyCode and BranchCode = a1.BranchCode and NOMOR_FAKTUR = a1.NOMOR_FAKTUR ORDER BY PPN)) END KodeTempDPP, 
case when a1.cBranch > 1 then ((case when b.TotalDPPAmt > a1.JUMLAH_DPP then b.TotalDPPAmt-a1.JUMLAH_DPP else a1.JUMLAH_DPP - b.TotalDPPAmt end)) else (case when b.TotalDPPAmt > a1.JUMLAH_DPP then b.TotalDPPAmt-a1.JUMLAH_DPP else a1.JUMLAH_DPP - b.TotalDPPAmt end) end selisihDPP,
case when a1.cBranch > 1 then ((case when b.TotalDPPAmt > a1.JUMLAH_DPP then '+' else '-' end)) else (case when b.TotalDPPAmt > a1.JUMLAH_DPP then '+' else '-' end) end plusMinDPP
FROM(
SELECT a.CompanyCode, a.BranchCode, (SELECT TOP 1 cBranch FROM @TblTempDetail WHERE CompanyCode = a.CompanyCode AND NOMOR_FAKTUR = a.NOMOR_FAKTUR) cBranch,
NOMOR_FAKTUR, ProfitCenter, SUM(DPP) JUMLAH_DPP
FROM @TblTempDetail a
WHERE ProfitCenter in ('300','100','200')
GROUP BY a.CompanyCode, a.BranchCode, NOMOR_FAKTUR, ProfitCenter
) a1
INNER join 
	(
	select a.CompanyCode, a.BranchCode, substring(REPLACE(REPLACE(TaxNo, '.', ''), '-', ''),4,13) TaxNo, SUM(DPPAmt) TotalDPPAmt
	from gnTaxOut a
	where 
	a.CompanyCode = @CompanyCode
	and (CASE WHEN @BranchCodeFrom = '' THEN '' ELSE a.BranchCode END) BETWEEN @BranchCodeFrom AND @BranchCodeTo
	group by a.CompanyCode, a.BranchCode, substring(REPLACE(REPLACE(TaxNo, '.', ''), '-', ''),4,13)
	) b
		on b.CompanyCode = a1.CompanyCode
		and b.BranchCode = a1.BranchCode
		and b.TaxNo = a1.NOMOR_FAKTUR
where a1.JUMLAH_DPP != b.TotalDPPAmt 
and (case when a1.cBranch > 1 then ((case when b.TotalDPPAmt > a1.JUMLAH_DPP then b.TotalDPPAmt-a1.JUMLAH_DPP else a1.JUMLAH_DPP - b.TotalDPPAmt end)) else (case when b.TotalDPPAmt > a1.JUMLAH_DPP then b.TotalDPPAmt-a1.JUMLAH_DPP else a1.JUMLAH_DPP - b.TotalDPPAmt end) end) > 0
) #t3

IF (SELECT COUNT(*) FROM #t4) > 1
UPDATE a 
SET a.DPP = (CASE WHEN a.plusMinDPP = '+' THEN a.DPP + a.sisaDPP ELSE a.DPP - a.sisaDPP END)
FROM(
select a.CompanyCode, a.BranchCode, a.NOMOR_FAKTUR, b.KodeTempDPP, b.InvoiceNo, a.DPP, ROUND(b.selisihDPP,0) sisaDPP, b.plusMinDPP
FROM @TblTempDetail a
INNER JOIN #t4 b ON
a.CompanyCode = b.CompanyCode
AND a.BranchCode = b.BranchCode
AND a.NOMOR_FAKTUR = b.NOMOR_FAKTUR 
AND a.KODE_OBJEK = b.KodeTempDPP
AND a.InvoiceNo = b.InvoiceNo
) a

DROP TABLE #t4

IF @table = 1
BEGIN
SELECT DISTINCT
	'FK' FK
	, LEFT(gnTaxOut.TaxNo,2) KD_JENIS_TRANSAKSI
	, 0 FG_PENGGANTI
	, substring(REPLACE(REPLACE(gnTaxOut.TaxNo, '.', ''), '-', ''),4,13) NOMOR_FAKTUR
	, (case when len(PeriodMonth) = 1 then convert(varchar, PeriodMonth, 1) 
		else convert(varchar, PeriodMonth, 2) end) MASA_PAJAK
	, PeriodYear TAHUN_PAJAK
	, CONVERT(VARCHAR, TaxDate, 103) TANGGAL_FAKTUR
	, REPLACE(REPLACE(NPWP, '.', ''), '-', '') NPWP
	, CustomerName NAMA
	, REPLACE(REPLACE(
		(CASE WHEN LEFT(FPJNO,3) = 'FPJ' THEN 
		(SELECT Address1 + ' ' + Address2 + ' ' + Address3 + ' ' + Address4 FROM spTrnSFPJInfo WHERE CompanyCode = gnTaxOut.CompanyCode AND BranchCode = gnTaxOut.BranchCode AND FPJNO =	gnTaxOut.FPJNO) ELSE  
		  CASE WHEN LEFT(FPJNO,3) = 'FPS' THEN 
		(SELECT Address1 + ' ' + Address2 + ' ' + Address3 + ' ' + Address4 FROM svTrnFakturPajak WHERE CompanyCode = gnTaxOut.CompanyCode AND BranchCode = gnTaxOut.BranchCode AND FPJNO = gnTaxOut.FPJNO) ELSE				
		(SELECT Address1 + ' ' + Address2 + ' ' + Address3 + ' ' + Address4 FROM gnMstCustomer a WHERE a.CompanyCode = gnTaxOut.CompanyCode AND a.CustomerCode = gnTaxOut.CustomerCode)	
		  END END), 
	  CHAR(13),''),CHAR(10),'') ALAMAT_LENGKAP
	, JUMLAH_DPP = (SELECT SUM(DPP) FROM @TblTempDetail WHERE CompanyCode = gnTaxOut.CompanyCode AND NOMOR_FAKTUR = substring(REPLACE(REPLACE(gnTaxOut.TaxNo, '.', ''), '-', ''),4,13))
	, JUMLAH_PPN = (SELECT SUM(PPN) FROM @TblTempDetail WHERE CompanyCode = gnTaxOut.CompanyCode AND NOMOR_FAKTUR = substring(REPLACE(REPLACE(gnTaxOut.TaxNo, '.', ''), '-', ''),4,13))
	, PPNBmAmt JUMLAH_PPNBM 
	, '' ID_KETERANGAN_TAMBAHAN
	, 0 FG_UANG_MUKA
	, 0 UANG_MUKA_DPP
	, 0 UANG_MUKA_PPN
	, 0 UANG_MUKA_PPNBM
	, (Case when left(FPJNo,3) = 'FPS'
				then (case when a.cBranch > 1  then 'Tanggal Jatuh Tempo : ' + convert(varchar(20),SubmissionDate,106) else 'No Invoice : ' + ReferenceNo + ' Tanggal Jatuh Tempo : ' + convert(varchar(20),SubmissionDate,106) end)
	       when left(FPJNo,3) = 'FPJ' 
				then (case when a.cBranch > 1  then 'Tanggal Jatuh Tempo : ' + convert(varchar(20),SubmissionDate,106) else 'No Invoice : ' + ReferenceNo + ' (' + FPJNo + ')' + ' Tanggal Jatuh Tempo : ' + convert(varchar(20),SubmissionDate,106) end) -- Penambahan
	       when left(FPJNo,3) = 'FPH' then 'No FPJ : ' + FPJNo
	       else (case when a.cBranch > 1 then 'Tanggal Jatuh Tempo : ' + convert(varchar(20),SubmissionDate,106) else 'No Invoice : ' + ReferenceNo + ' Tanggal Jatuh Tempo : ' + convert(varchar(20),SubmissionDate,106) end)
	  end) + (case when a.cBranch = 1 THEN + ' - Cabang ' + SUBSTRING(gnTaxOut.BranchCode,len(gnTaxOut.BranchCode)-1,2) else '' END) REFERENSI   
	, CUSTOMERCODE
FROM 
	gnTaxOut 
INNER JOIN 
(SELECT CompanyCode, TaxNo, COUNT(a.TaxNo) cBranch
FROM gnTaxOut a 
GROUP BY CompanyCode, TaxNo) a ON 
	a.CompanyCode = gnTaxOut.CompanyCode AND a.TaxNo = gnTaxOut.TaxNo
WHERE
	gnTaxOut.CompanyCode = @CompanyCode
	AND (CASE WHEN @BranchCodeFrom = '' THEN '' ELSE gnTaxOut.BranchCode END) BETWEEN @BranchCodeFrom AND @BranchCodeTo
	AND CONVERT(VARCHAR, TaxDate, 112) BETWEEN @PeriodFrom AND @PeriodTo 
	AND IsPKP = 1
	AND DocumentType = 'F'
	AND (CASE WHEN @ProfitCenterCode = '' THEN '' ELSE ProfitCenter END) = @ProfitCenterCode
	--AND substring(REPLACE(REPLACE(gnTaxOut.TaxNo, '.', ''), '-', ''),4,13) = @nomorfaktur	
ORDER BY substring(REPLACE(REPLACE(gnTaxOut.TaxNo, '.', ''), '-', ''),4,13)
END

IF @table = 3
BEGIN
SELECT 
[OF]
	, KODE_OBJEK
	, NAMA
	, HARGA_SATUAN
	, convert(decimal(12,2),JUMLAH_BARANG) JUMLAH_BARANG
	, HARGA_TOTAL
	, DISKON
	, DPP
	, PPN
	, TARIF_PPNBM
	, PPNBM
	, CUSTOMERCODE
	, NOMOR_FAKTUR
	, FPJNO
from @TblTempDetail
ORDER BY NOMOR_FAKTUR
--where NOMOR_FAKTUR = @nomorfaktur
END
END

IF @table = 2
BEGIN
	select  'LT' LT, REPLACE(REPLACE(NPWPNo, '.', ''), '-', '') NPWP, CustomerName NAMA, REPLACE(REPLACE(Address1, CHAR(13),''),CHAR(10),'')  + REPLACE(REPLACE(Address2, CHAR(13),''),CHAR(10),'') JALAN, '-' BLOK, '-' NOMOR, '0' RT, '0' RW, 
		KecamatanDistrik KECAMATAN, KelurahanDesa KELURAHAN, KotaKabupaten KABUPATEN, 
		case when isnull(ProvinceCode,'') = '' then '-' else (isnull((select top 1 lookupvaluename from gnmstlookupdtl where codeid='PROV' and LookUpValue = ProvinceCode),'-' ))end PROPINSI, 
		ZipNo KODE_POS, PhoneNo NOMOR_TELEPON, a.CustomerCode
	from gnMstCustomer a
	inner join
		(select distinct CustomerCode from gnTaxOut a WHERE
		CompanyCode = @CompanyCode
		AND (CASE WHEN @BranchCodeFrom = '' THEN '' ELSE BranchCode END) BETWEEN @BranchCodeFrom AND @BranchCodeTo
		AND ProductType = @ProductType
		AND CONVERT(VARCHAR, TaxDate, 112) BETWEEN @PeriodFrom AND @PeriodTo 
		AND IsPKP = 1) b on a.CustomerCode = b.CustomerCode
END

