
if object_id('uspfn_GetCostPrice') is not null
	drop PROCEDURE uspfn_GetCostPrice
GO
-- =============================================
-- Author:		SDMS.RUDIANA
-- Create date: 2015-06-05
-- Description:	Get CostPrice Value
-- =============================================
CREATE PROCEDURE [dbo].[uspfn_GetCostPrice] (@CompanyCode varchar(15), @BranchCode varchar(15), 
	@PartNo varchar(15), @CostPrice numeric(18,2) output)
AS
BEGIN
	DECLARE @Discount NUMERIC(18,2)
	DECLARE @PurcDiscPct numeric(18,2)
	DECLARE @DiscPct numeric(18,2)

	set @Discount = 0;
    set @PurcDiscPct = (SELECT PurcDiscPct FROM spMstItems WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND PartNo = @PartNo)

	if(@PurcDiscPct is not null) begin
		--discount from master items
		SET @Discount = @PurcDiscPct
	end
	else begin
        --discount from master supplier
        set @DiscPct = (select DiscPct from gnMstSupplierProfitCenter where CompanyCode = @CompanyCode and BranchCode = @BranchCode
			and SupplierCode = (select dbo.GetBranchMD(@CompanyCode, @BranchCode)) and ProfitCenterCode = '300')
		
		if(@DiscPct is not null)begin
			if(@DiscPct >= 0) begin
				SET @Discount = @Discount + @DiscPct;
			end
		end 
	end
	
	--declare @xCostPrice numeric(18,2)
	declare @sql nvarchar(512);
	set @sql = 'select @CostPrice = (b.RetailPrice - (b.RetailPrice * (' + convert(varchar, @Discount) + ' *  0.01)))
		from ' + dbo.GetDbMD(@CompanyCode, @BranchCode) + '..spMstItemPrice b 
		where b.CompanyCode =''' + dbo.GetCompanyMD(@CompanyCode, @BranchCode) + '''
		and b.BranchCode =''' + dbo.GetBranchMD(@CompanyCode, @BranchCode) + '''
		and b.PartNo =''' + @PartNo + ''''


		print @sql

	execute sp_executesql @sql, N'@CostPrice numeric(18,2) OUTPUT', @CostPrice = @CostPrice OUTPUT
END
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[uspfn_SvTrnServiceOutstandingWeb]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[uspfn_SvTrnServiceOutstandingWeb]
GO

CREATE procedure [dbo].[uspfn_SvTrnServiceOutstandingWeb]
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
GO

DROP VIEW [dbo].[SvSaView]
GO

CREATE view [dbo].[SvSaView]
as 
select a.CompanyCode, a.BranchCode, a.EmployeeID, a.EmployeeName  from GnMstEmployee a
where a.TitleCode IN ('7')
   AND PersonnelStatus = '1'
GO


if object_id('uspfn_GenerateBPSLampiranNew') is not null
	drop procedure uspfn_GenerateBPSLampiranNew
GO

CREATE procedure [dbo].[uspfn_GenerateBPSLampiranNew] 
(
	@CompanyCode	VARCHAR(MAX),
	@BranchCode		VARCHAR(MAX),
	@JobOrderNo		VARCHAR(MAX),
	@ProductType	VARCHAR(MAX),
	@CustomerCode	VARCHAR(MAX),
	@UserID			VARCHAR(MAX),
	@PickedBy		VARCHAR(MAX)
)
AS
BEGIN

/*
PSEUDOCODE PROCESS :
Line 38  : RE-CALCULATE AMOUNT DETAIL
Line 93  : RE-CALCULATE AMOUNT HEADER AND UPDATE STATUS
Line 140 : UPDATE SO SUPPLY AND STATUS HEADER 
Line 167 : UPDATE SUPPLY SLIP QTY SERVICE 
Line 237 : INSERT NEW SRV ITEM BASED PICKING LIST
Line 276 : INSERT BPS AND LAMPIRAN
Line 292 : INSERT BPS HEADER
Line 352 : INSERT BPS DETAIL
Line 395 : INSERT LAMPIRAN HEADER
Line 458 : INSERT LAMPIRAN DETAIL
Line 500 : UPDATE STOCK
Line 571 : UPDATE DEMAND CUST AND DEMAND ITEM
Line 611 : INSERT TO ITEM MOVEMENT
Line 650 : UPDATE TRANSDATE
*/

--DECLARE	@CompanyCode	VARCHAR(MAX),
--		@BranchCode		VARCHAR(MAX),
--		@JobOrderNo		VARCHAR(MAX),
--		@ProductType	VARCHAR(MAX),
--		@CustomerCode	VARCHAR(MAX),
--		@UserID			VARCHAR(MAX),
--		@PickedBy		VARCHAR(MAX)

--SET	@CompanyCode	= '6156401000'
--SET	@BranchCode		= '6156401001'
--SET	@JobOrderNo		= 'SPK/15/001666'
--SET	@ProductType	= '4W'
--SET	@CustomerCode	= '0032710'
--SET	@UserID			= 'ga'
--SET	@PickedBy		= '0004'
		
--exec uspfn_GenerateBPSLampiranNew '6006400001','6006400101','SPK/14/101625','4W','JKT-1852626','ga','00001'

--===============================================================================================================================
-- RE-CALCULATE AMOUNT DETAIL
--===============================================================================================================================
DECLARE @DefaultDate		DATETIME

SET @DefaultDate = '1900-01-01 00:00:00.000'

if object_id('#tmpSvSDMovement') is not null drop table #tmpSvSDMovement

DECLARE @DbMD AS VARCHAR(15)
SET @DbMD = (SELECT DbMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)

  declare @PurchaseDisc as decimal
  set @PurchaseDisc = (select isnull(DiscPct, 0) from gnMstSupplierProfitCenter   
       where CompanyCode = @CompanyCode   
       and BranchCode = @BranchCode  
       and SupplierCode = dbo.GetBranchMD(@CompanyCode,@BranchCode)
       and ProfitCenterCode = '300')  
         
   if (@PurchaseDisc = 0) raiserror ('Purchase Discount belum di-setting untuk Part tersebut!',16,1);   

SELECT * INTO #TempPickingSlipDtl FROM (
SELECT
	a.CompanyCode
	, a.BranchCode 
	, a.PickingSlipNo
	, a.PickingSlipDate
	, a.CustomerCode
	, a.TypeOfGoods
	, b.DocNo
	, b.PartNo
	, b.QtyPicked
	, b.QtyPicked * b.RetailPrice SalesAmt
	, Floor((b.QtyPicked * b.RetailPrice) * DiscPct/100) DiscAmt
	, (b.QtyPicked * b.RetailPrice) - Floor((b.QtyPicked * b.RetailPrice) * DiscPct/100) NetSalesAmt
FROM SpTrnSPickingHdr a WITH (NOLOCK, NOWAIT)
INNER JOIN SpTrnSPickingDtl b WITH (NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode 
	AND a.BranchCode = b.BranchCode AND a.PickingSlipNo = b.PickingSlipNo
WHERE 
	1 = 1
	AND a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND Status < 2
	AND b.DocNo IN (SELECT DocNo FROM SpTrnSordHdr WITH (NOLOCK, NOWAIT)
				WHERE 
					1 = 1
					AND CompanyCode =@CompanyCode
					AND BranchCode = @BranchCode
					AND UsageDocNo = @JobOrderNo
					AND Status = 4)
) #TempPickingSlipDtl

UPDATE SpTrnSPickingDtl
SET SalesAmt = b.SalesAmt 
	, DiscAmt = b.DiscAmt
	, NetSalesAmt = b.NetSalesAmt
	, TotSalesAmt = b.NetSalesAmt
	, QtyBill = b.QtyPicked
	, LastUpdateBy = @UserID
	, LastUpdateDate = GetDate()
FROM SpTrnSPickingDtl a, #TempPickingSlipDtl b
WHERE
	1 = 1
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.PickingSlipNo = b.PickingSlipNo
	AND a.PartNo = b.PartNo

--===============================================================================================================================
-- RE-CALCULATE AMOUNT HEADER AND UPDATE STATUS
--===============================================================================================================================
SELECT * INTO #TempPickingSlipHdr FROM (
SELECT
	a.CompanyCode
	, a.BranchCode
	, a.PickingSlipNo	
	, SUM(b.QtyPicked) TotSalesQty
	, SUM(b.SalesAmt) TotSalesAmt
	, SUM(b.DiscAmt) TotDiscAmt
	, SUM(b.NetSalesAmt) TotDPPAmt
	, floor(SUM(b.NetSalesAmt) * (ISNULL((SELECT TaxPct FROM GnMstTax x WITH (NOLOCK, NOWAIT) WHERE x.CompanyCode = @CompanyCode AND x.TaxCode IN 
		(SELECT TaxCode FROM GnMstCustomerProfitCenter y WITH (NOLOCK, NOWAIT) WHERE y.CompanyCode = @CompanyCode AND y.BranchCode = @BranchCode
			AND y.ProfitCenterCode = '300' AND y.CustomerCode = @CustomerCode)),0)/100))
	  TotPPNAmt
FROM spTrnSPickingHdr a WITH (NOLOCK, NOWAIT)
LEFT JOIN spTrnSPickingDtl b WITH (NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.PickingSlipNo = b.PickingSlipNo
WHERE
	1 = 1
	AND a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND a.PickingSlipNo IN (SELECT DISTINCT(PickingSlipNo) FROM #TempPickingSlipDtl WITH (NOLOCK, NOWAIT))
GROUP BY a.CompanyCode
	, a.BranchCode
	, a.PickingSlipNo	
) #TempPickingSlipHdr

UPDATE SpTrnSPickingHdr
SET TotSalesQty = b.TotSalesQty
	, TotSalesAmt = b.TotSalesAmt
	, TotDiscAmt = b.TotDiscAmt
	, TotDPPAmt = b.TotDPPAmt
	, TotPPNAmt = b.TotPPNAmt
	, TotFinalSalesAmt = b.TotDPPAmt + b.TotPPNAmt
	, Status = 2
	, PickedBy = @PickedBy
	, LastUpdateBy = @UserID
	, LastUpdateDate = GetDate()
FROM SpTrnSPickingHdr a, #TempPickingSlipHdr b
WHERE
	1 = 1
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.PickingSlipNo = b.PickingSlipNo

--===============================================================================================================================
-- UPDATE SO SUPPLY AND STATUS HEADER 
--===============================================================================================================================
UPDATE SpTrnSOSupply
SET	Status = 2
	, QtyPicked = b.QtyPicked
	, QtyBill = b.QtyPicked
	, LastUpdateBy = @UserID
	, LastUpdateDate = GetDate()
FROM SpTrnSOSupply a, #TempPickingSlipDtl b
WHERE
	1 = 1
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.DocNo = b.DocNo
	AND a.PartNo = b.PartNo

UPDATE SpTrnSORDHdr 
SET Status = 5
	, LastUpdateBy = @UserID
	, LastUpdateDate = GetDate()
WHERE
	1 = 1
	AND CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND DocNo IN (SELECT DISTINCT(DocNo) FROM #TempPickingSlipDtl)

--===============================================================================================================================
-- UPDATE SUPPLY SLIP QTY SERVICE 
--===============================================================================================================================
DECLARE @ServiceNo VARCHAR(MAX)

SET @ServiceNo = (SELECT ServiceNo FROM svTrnService WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
		AND ProductType = @ProductType AND JobOrderNo = @JobOrderNo)

SELECT * INTO #TempServiceItem FROM (
SELECT 
	a.CompanyCode
	, a.BranchCode
	, a.ProductType
	, a.ServiceNo
	, a.PartNo
	, a.PartSeq
	, a.DemandQty
	, a.SupplyQty
	, b.QtyBill
	, b.DocNo
	, c.RetailPriceInclTax - (c.RetailPriceInclTax * isnull(d.DiscPct, 0) * 0.01) CostPrice
	, a.RetailPrice
	, a.TypeOfGoods
	, a.BillType
	, a.DiscPct
FROM SvTrnSrvItem a WITH (NOLOCK, NOWAIT)
INNER JOIN SpTrnSPickingDtl b WITH (NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.PartNo = b.PartNo AND a.SupplySlipNo = b.DocNo
INNER JOIN SpMstItemPrice c ON a.CompanyCode = c.CompanyCode AND a.BranchCode = c.BranchCode AND a.PartNo = c.PartNo
INNER JOIN gnMstSupplierProfitCenter d ON c.CompanyCode = d.CompanyCode AND a.BranchCode = d.BranchCode
WHERE
	1 = 1
	AND a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND a.ProductType = @ProductType
	AND a.ServiceNo = @ServiceNo
	AND a.PartSeq IN (SELECT MAX(PartSeq) FROM SvTrnSrvItem WITH (NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode
		AND ProductType = @ProductType AND ServiceNo = a.ServiceNo AND PartNo = a.PartNo)
	AND a.SupplySlipNo = b .DocNo
	AND d.SupplierCode = dbo.GetBranchMD(@CompanyCode,@BranchCode)
	AND d.ProfitCenterCode = '300'
) #TempServiceItem 

UPDATE svTrnSrvItem
SET SupplyQty = (CASE WHEN b.QtyBill > b.DemandQty 
				THEN 
					CASE WHEN b.DemandQty = 0 THEN b.QtyBill ELSE b.DemandQty END
				ELSE b.QtyBill END)
	, LastUpdateBy = @UserID
	, LastUpdateDate = Getdate()
FROM svTrnSrvItem a, #TempServiceItem b
WHERE
	1 = 1
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.ProductType = b.ProductType
	AND a.ServiceNo = b.ServiceNo
	AND a.PartNo = b.PartNo
	AND a.PartSeq = b.PartSeq
	AND a.SupplySlipNo = b.DocNo

UPDATE svTrnSrvItem
SET CostPrice = b.CostPrice
	, LastUpdateBy = @UserID
	, LastUpdateDate = Getdate()
FROM svTrnSrvItem a, #TempServiceItem b
WHERE
	1 = 1
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.ProductType = b.ProductType
	AND a.ServiceNo = b.ServiceNo
	AND a.PartNo = b.PartNo
	AND a.SupplySlipNo = b.DocNo

--===============================================================================================================================
-- INSERT NEW SRV ITEM BASED PICKING LIST
--===============================================================================================================================
INSERT INTO SvTrnSrvItem (CompanyCode, BranchCode, ProductType, ServiceNo, PartNo, PartSeq, DemandQty, SupplyQty, ReturnQty, CostPrice, RetailPrice, TypeOfGoods, BillType, SupplySlipNo, SupplySlipDate, SSReturnNo, SSReturnDate, CreatedBy, CreatedDate, LastupdateBy, LastupdateDate, DiscPct)
SELECT 
	a.CompanyCode
	, a.BranchCode
	, a.ProductType
	, a.ServiceNo
	, a.PartNo
	, a.PartSeq + 1
	, 0 DemandQty
	, a.QtyBill - a.DemandQty SupplyQty
	, 0 ReturnQty
	, a.CostPrice
	, a.RetailPrice
	, a.TypeOfGoods
	, a.BillType
	, a.DocNo SupplySlipNo
	, (SELECT DocDate FROM SpTrnSORDHdr WITH (NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode
		AND DocNo = a.DocNo) SupplySlipDate
	, NULL SSReturnNo
	, NULL SSReturnDate
	, @UserID CreatedBy
	, GetDate() CreatedDate
	, @UserID LastUpdateBy
	, GetDate() LastUpdateDate
	, a.DiscPct
FROM #TempServiceItem a WITH (NOLOCK, NOWAIT)
WHERE
	1 = 1
	AND a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND a.ProductType = @ProductType
	AND a.DemandQty < a.QtyBill
	AND a.QtyBill > 0
	AND a.DemandQty > 0

DROP TABLE #TempServiceItem 

--===============================================================================================================================
-- INSERT BPS AND LAMPIRAN
--===============================================================================================================================
DECLARE @PickingSlipNo	VARCHAR(MAX)
DECLARE	@TempBPSFNo		VARCHAR(MAX)
DECLARE	@TempLMPNo		VARCHAR(MAX)
DECLARE @MaxBPSFNo		INT
DECLARE @MaxLmpNo		INT

DECLARE db_cursor CURSOR FOR
SELECT DISTINCT PickingSlipNo FROM #TempPickingSlipHdr
OPEN db_cursor
FETCH NEXT FROM db_cursor INTO @PickingSlipNo

WHILE @@FETCH_STATUS = 0
BEGIN	

--===============================================================================================================================
-- INSERT BPS HEADER
--===============================================================================================================================
SET @MaxBPSFNo = ISNULL((SELECT DocumentSequence + 1 FROM GnMstDocument WHERE 
	CompanyCode = @CompanyCode
		AND BranchCode = @BranchCode
		AND DocumentType = 'BPF' 
		AND ProfitCenterCode = '300' 
		AND DocumentYear = YEAR(GetDate())),0)

SET @TempBPSFNo = ISNULL((SELECT 'BPF/' + RIGHT(YEAR(GETDATE()),2) + '/' + RIGHT(1000000 + CONVERT(VARCHAR, @MaxBPSFNo, 1), 6)),'BPF/YY/XXXXXX')

INSERT INTO SpTrnSBPSFHdr
SELECT 
	CompanyCode
	, BranchCode
	, @TempBPSFNo BPSFNo
	, GetDate() BPSFDate
	, PickingSlipNo
	, PickingSlipDate
	, TransType
	, SalesType
	, CustomerCode
	, CustomerCodeBill
	, CustomerCodeShip
	, TotSalesQty
	, TotSalesAmt
	, TotDiscAmt
	, TotDPPAmt
	, TotPPNAmt
	, TotFinalSalesAmt
	, '2' Status
	, 0 PrintSeq
	, TypeOfGoods
	, @UserID CreatedBy
	, GetDate() CreatedDate
	, @UserID LastUpdateBy
	, GetDate() LastUpdateDate
	, NULL isLocked
	, NULL LockingBy
	, NULL LockingDate
FROM SpTrnSPickingHdr WITH (NOLOCK, NOWAIT)
WHERE
	1 = 1
	AND CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND PickingSlipNo = @PickingSlipNo

UPDATE GnMstDocument
SET DocumentSequence = DocumentSequence + 1
	, LastUpdateDate = GetDate()
	, LastUpdateBy = @UserID
WHERE
	1 = 1
	AND CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND DocumentPrefix = 'BPF'
	AND ProfitCenterCode = '300'
	AND DocumentYear = Year(GetDate())

--===============================================================================================================================
-- INSERT BPS DETAIL
--===============================================================================================================================
INSERT INTO SpTrnSBPSFDtl
SELECT
	CompanyCode
	, BranchCode
	, @TempBPSFNo
	, WarehouseCode
	, PartNo
	, PartNoOriginal
	, DocNo
	, DocDate
	, ReferenceNo
	, ReferenceDate
	, LocationCode
	, QtyBill
	, RetailPriceInclTax
	, RetailPrice
	, CostPrice
	, DiscPct
	, SalesAmt
	, DiscAmt
	, NetSalesAmt
	, 0 PPNAmt
	, TotSalesAmt
	, ProductType
	, PartCategory 
	, MovingCode
	, ABCClass
	, '' ExPickingListNo
	, @DefaultDate ExPickingListDate
	, @UserID CreatedBy
	, GetDate() CreatedDate
	, @UserID LastUpdateBy
	, GetDate() LastUpdateDate
FROM SpTrnSPickingDtl WITH (NOLOCK, NOWAIT)
WHERE 
	1 = 1
	AND CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND PickingSlipNo = @PickingSlipNo

--===============================================================================================================================
-- INSERT LAMPIRAN HEADER
--===============================================================================================================================
SET @MaxLmpNo = ISNULL((SELECT DocumentSequence + 1 FROM GnMstDocument WHERE 
	CompanyCode = @CompanyCode
		AND BranchCode = @BranchCode
		AND DocumentType = 'LMP' 
		AND ProfitCenterCode = '300' 
		AND DocumentYear = YEAR(GetDate())),0)

SET @TempLmpNo = ISNULL((SELECT 'LMP/' + RIGHT(YEAR(GETDATE()),2) +'/' + RIGHT(1000000 + CONVERT(VARCHAR, @MaxLmpNo, 1), 6)),'LMP/YY/XXXXXX')

INSERT INTO SpTrnSLmpHdr
SELECT
	CompanyCode
	, BranchCode
	, @TempLmpNo LmpNo	
	, GetDate() LmpDate
	, @TempBPSFNo BPSFNo
	, (SELECT BPSFDate FROM SpTrnSBPSFHdr WITH (NOLOCK, NOWAIT) WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND BPSFNo = @TempBPSFNo)
		BPSFDate
	, PickingSlipNo
	, PickingSlipDate
	, TransType
	, CustomerCode
	, CustomerCodeBill
	, CustomerCodeShip
	, TotSalesQty
	, TotSalesAmt
	, TotDiscAmt
	, TotDPPAmt
	, TotPPNAmt
	, TotFinalSalesAmt
	, CONVERT(BIT, 0) isPKP
	, '0' Status
	, 0 PrintSeq
	, TypeOfGoods
	, @UserID CreatedBy
	, GetDate() CreatedDate
	, @UserID LastUpdateBy
	, GetDate() LastUpdateDate
	, NULL IsLocked
	, NULL LockingBy
	, NULL LockingDate
FROM SpTrnSPickingHdr WITH (NOLOCK, NOWAIT)
WHERE 
	1 = 1
	AND CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND PickingSlipNo = @PickingSlipNo

UPDATE GnMstDocument
SET DocumentSequence = DocumentSequence + 1
	, LastUpdateDate = GetDate()
	, LastUpdateBy = @UserID
WHERE
	1 = 1
	AND CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND DocumentPrefix = 'LMP'
	AND ProfitCenterCode = '300'
	AND DocumentYear = Year(GetDate())

--===============================================================================================================================
-- INSERT LAMPIRAN DETAIL
--===============================================================================================================================
INSERT INTO SpTrnSLmpDtl
SELECT
	a.CompanyCode
	, a.BranchCode
	, @TempLmpNo LmpNo
	, a.WarehouseCode
	, a.PartNo
	, a.PartNoOriginal
	, a.DocNo
	, a.DocDate
	, a.ReferenceNo
	, a.ReferenceDate
	, a.LocationCode
	, a.QtyBill
	, a.RetailPriceInclTax
	, a.RetailPrice
	, a.CostPrice
	, a.DiscPct
	, a.SalesAmt
	, a.DiscAmt
	, a.NetSalesAmt
	, 0 PPNAmt
	, a.TotSalesAmt
	, a.ProductType
	, a.PartCategory 
	, a.MovingCode
	, a.ABCClass
	, @UserID CreatedBy
	, GetDate() CreatedDate
	, @UserID LastUpdateBy
	, GetDate() LastUpdateDate
FROM SpTrnSPickingDtl a WITH (NOLOCK, NOWAIT)
WHERE 
	1 = 1
	AND a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND a.PickingSlipNo = @PickingSlipNo
	AND a.QtyPicked > 0

--===============================================================================================================================
-- UPDATE STOCK
--===============================================================================================================================

--===============================================================================================================================
-- VALIDATION QTY
--===============================================================================================================================
	DECLARE @Onhand_SRValid NUMERIC(18,2)	
	DECLARE @Allocation_SRValid NUMERIC(18,2)
	DECLARE @errmsg VARCHAR(MAX)
	DECLARE @CompanyMD AS VARCHAR(15)
	DECLARE @BranchMD AS VARCHAR(15)
	DECLARE @WarehouseMD AS VARCHAR(15)

	SET @CompanyMD = (SELECT CompanyMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)
	SET @BranchMD = (SELECT BranchMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)
	SET @WarehouseMD = (SELECT WarehouseMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)

declare @validString varchar(max)

declare @valid_2 table(
PartNo varchar(20),
QtyValidSR NUMERIC(18,2),
QtyValidOnhand NUMERIC(18,2)
)
    set @validString ='SELECT a.PartNo
		, a.AllocationSR - b.QtyBill QtyValidSR
		, a.Onhand - b.QtyBill QtyValidOnhand
	FROM '+ @DbMD +'..SpMstItems a, SpTrnSPickingDtl b
	WHERE 1 = 1
		AND a.CompanyCode = '''+ @CompanyMD +'''
		AND a.BranchCode = '''+ @BranchMD+'''
		AND b.PickingSlipNo = '''+@PickingSlipNo+'''
        AND b.ReferenceNo = '''+@JobOrderNo+'''
		AND b.CompanyCode = '''+ @CompanyCode +'''
		AND b.BranchCode = '''+ @BranchCode+'''
		AND a.PartNo = b.PartNo'
	
	--print(@validString)
	insert into @valid_2 exec(@validString)

select * from @valid_2

	SET @Allocation_SRValid = ISNULL((SELECT TOP 1 QtyValidSR FROM @valid_2 WHERE QtyValidSR < 0),0)
	SET @Onhand_SRValid = ISNULL((SELECT TOP 1 QtyValidOnhand FROM @valid_2 WHERE QtyValidOnhand < 0),0)
	select @Allocation_SRValid
	select @Onhand_SRValid

	IF (@Onhand_SRValid < 0 OR @Allocation_SRValid < 0)
	BEGIN
		SET @errmsg = 'PEMBUATAN DOKUMEN LAMPIRAN GAGAL ! ' + char(13) + char(13) + 'Error Message: Terdapat part dengan quantity Onhand atau alokasi kurang dari nol !'
		
		CLOSE db_cursor
		DEALLOCATE db_cursor 
		
		DROP TABLE #TempPickingSlipDtl
		DROP TABLE #TempPickingSlipHdr
		
		RAISERROR (@errmsg,16,1);
		
		RETURN
	END
--===============================================================================================================================

--DECLARE @DbMD AS VARCHAR(15)
--SET @DbMD = (SELECT DbMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)

DECLARE @Query AS VARCHAR(MAX)
SET @Query = 
'UPDATE '+@DbMD+'..SpMstItems
SET
	AllocationSR = AllocationSR - b.QtySupply
	, Onhand = Onhand - b.QtyBill
	, LastUpdateBy = ' + '''' + @UserID + '''' +'
	, LastUpdateDate = GetDate()
	, LastSalesDate = GetDate()
FROM ' + @DbMD + '..SpMstItems a, SpTrnSPickingDtl b
WHERE
	1 = 1
	AND a.CompanyCode = ' + ''''+@CompanyMD+'''' + '
	AND a.BranchCode = ' + ''''+@BranchMD +''''+ '
	AND b.PickingSlipNo = ' + ''''+@PickingSlipNo+'''' + '
	AND b.ReferenceNo = ' + ''''+@JobOrderNo+'''' + '
	AND b.CompanyCode = ' + ''''+@CompanyCode+'''' + '
	AND b.BranchCode = ' + ''''+@BranchCode+'''' + '
	AND a.PartNo = b.PartNo
UPDATE '+ @DbMD +'..SpMstItemLoc
SET
	AllocationSR = AllocationSR - b.QtySupply
	, Onhand = Onhand - b.QtyBill
	, LastUpdateBy = ' + '''' + @UserID + '''' +'
	, LastUpdateDate = GetDate()
FROM ' + @DbMD + '..SpMstItemLoc a, SpTrnSPickingDtl b
WHERE
	1 = 1
	AND a.CompanyCode = ' + ''''+@CompanyMD+'''' + '
	AND a.BranchCode = ' + ''''+@BranchMD +''''+ '
	AND a.WarehouseCode = ' + ''''+@WarehouseMD +''''+ '
	AND b.PickingSlipNo = ' + ''''+@PickingSlipNo+'''' + '
	AND b.CompanyCode = ' + ''''+@CompanyCode+'''' + '
	AND b.BranchCode = ' + ''''+@BranchCode+'''' + '
	AND a.PartNo = b.PartNo'

EXEC(@query)
	--print(@query)
--===============================================================================================================================
-- UPDATE DEMAND CUST AND DEMAND ITEM
--===============================================================================================================================
UPDATE SpHstDemandCust
SET SalesFreq = SalesFreq + 1
	, SalesQty = SalesQty + b.QtyBill
	, LastUpdateBy = @UserID 
	, LastUpdateDate = GetDate()
FROM SpHstDemandCust a, SpTrnSPickingDtl b
WHERE
	1 = 1
	AND a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND b.PickingSlipNo = @PickingSlipNo
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.Year = Year(b.DocDate)
	AND a.Month = Month(b.DocDate)
	AND a.CustomerCode IN (SELECT CustomerCode FROM SpTrnSPickingHdr WHERE CompanyCode = @CompanyCode AND BranchCode = BranchCode
							AND PickingSlipNo = @PickingSlipNo)
	AND a.PartNo = b.PartNo
	

UPDATE SpHstDemandItem
SET SalesFreq = SalesFreq + 1
	, SalesQty = SalesQty + b.QtyBill
	, LastUpdateBy = @UserID 
	, LastUpdateDate = GetDate()
FROM SpHstDemandItem a, SpTrnSPickingDtl b
WHERE
	1 = 1
	AND a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND b.PickingSlipNo = @PickingSlipNo
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.Year = Year(b.DocDate)
	AND a.Month = Month(b.DocDate)
	AND a.PartNo = b.PartNo
--
----=============================================================================================================================
---- INSERT TO ITEM MOVEMENT
----=============================================================================================================================
INSERT INTO SpTrnIMovement
SELECT
	@CompanyCode CompanyCode
	, @BranchCode BranchCode
	, a.LmpNo DocNo
	, (SELECT LmPDate FROM SpTrnSLmpHdr WITH (NOLOCK, NOWAIT) WHERE CompanyCode = @CompanyCode 
		AND BranchCode = @BranchCode AND LmpNo = a.LmpNo) 
	  DocDate
	, dateadd(s,ROW_NUMBER() OVER(Order by a.PartNo),getdate()) CreatedDate 
	, '00' WarehouseCode
	, LocationCode 
	, a.PartNo
	, 'OUT' SignCode
	, 'LAMPIRAN' SubSignCode
	, a.QtyBill
	, a.RetailPrice
	, a.CostPrice
	, a.ABCClass
	, a.MovingCode
	, a.ProductType
	, a.PartCategory
	, @UserID CreatedBy
FROM SpTrnSLmpDtl a WITH (NOLOCK, NOWAIT)
WHERE
	1 = 1
	AND CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND LmpNo IN (SELECT LmpNo FROM SpTrnSLmpHdr WITH (NOLOCK, NOWAIT) WHERE CompanyCode = @CompanyCode 
				AND BranchCode = @BranchCode AND PickingSlipNo = @PickingSlipNo)

--===============================================================================================================================
-- INSERT INTO svSDMovement
--===============================================================================================================================
declare @md bit
set @md = (select case WHEN EXISTS(select * from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode and CompanyMD = @CompanyCode and BranchMD = @BranchCode) then cast(1 as bit) ELSE cast(0 as bit) END)

 if(@md = 0)
 begin
	set @Query = '
	insert into ' + @DbMD + '..svSDMovement (CompanyCode, BranchCode, DocNo, DocDate, PartNo, PartSeq
	, WarehouseCode, QtyOrder, Qty, DiscPct, CostPrice, RetailPrice
	, TypeOfGoods, CompanyMD, BranchMD, WarehouseMD, RetailPriceInclTaxMD, RetailPriceMD
	, CostPriceMD, QtyFlag, ProductType, ProfitCenterCode, Status, ProcessStatus
	, ProcessDate, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)  
	select h.CompanyCode, h.BranchCode, h.LmpNo, h.LmpDate, d.PartNo, ROW_NUMBER() OVER(Order by d.LmpNo)
	,d.WarehouseCode, d.QtyBill, d.QtyBill, d.DiscPct
	--,isnull(((select RetailPrice from spTrnSLmpDtl
	--	where CompanyCode = ''' + @CompanyCode + '''  and BranchCode = ''' + @BranchCode  + '''
	--	and ProductType = ''' + @ProductType  + ''' and LmpNo = ''' + @TempLmpNo + ''' and PartNo = d.PartNo) / 1.1 * 
	--	((100 - isnull((select isnull(DiscPct, 0) from gnMstSupplierProfitCenter   
	--		where CompanyCode = ''' + @CompanyCode + ''' and BranchCode = ''' + @BranchCode  + ''' and SupplierCode = dbo.GetBranchMD(''' + @CompanyCode + ''', ''' + @BranchCode  + ''') 
	--		and ProfitCenterCode = ''300''),0)) * 0.01)),0) CostPrice
	, d.CostPrice
	, d.RetailPrice
	,h.TypeOfGoods, ''' + @CompanyMD + ''', ''' + @BranchMD + ''', ''' + @WarehouseMD + ''', p.RetailPriceInclTax, p.RetailPrice, 
	p.CostPrice,''x'', d.ProductType,''300'', ''0'',''0'',''' + convert(varchar, GETDATE()) + ''',''' + @UserID + ''',''' +
	  convert(varchar, GETDATE()) + ''',''' +  @UserID + ''',''' +  convert(varchar, GETDATE()) + '''
	 from spTrnSLmpDtl d 
	 inner join spTrnSLmpHdr h on h.CompanyCode = d.CompanyCode and h.BranchCode = d.BranchCode and h.LmpNo = d.LmpNo  
	 join spmstitemprice p
	 on p.PartNo = d.PartNo and p.CompanyCode = d.CompanyCode and p.BranchCode = d.BranchCode
	  where 1 = 1   
		and d.CompanyCode = ''' + @CompanyCode + ''' 
		and d.BranchCode  = ''' + @BranchCode  + '''
		and d.ProductType = ''' + @ProductType  + '''
		and d.LmpNo = ''' + @TempLmpNo + '''';
	
	exec(@Query);
end

FETCH NEXT FROM db_cursor INTO @PickingSlipNo
END
CLOSE db_cursor
DEALLOCATE db_cursor 

--===============================================================================================================================
-- UPDATE TRANSDATE
--===============================================================================================================================

update gnMstCoProfileSpare
set TransDate=getdate()
	, LastUpdateBy=@UserID
	, LastUpdateDate=getdate()
where CompanyCode = @CompanyCode AND BranchCode = @BranchCode


--===============================================================================================================================
-- DROP SECTION HEADER
--===============================================================================================================================
DROP TABLE #TempPickingSlipDtl
DROP TABLE #TempPickingSlipHdr
end
GO

if object_id('uspfn_omSoLkp') is not null
	drop procedure uspfn_omSoLkp
GO

CREATE procedure [dbo].[uspfn_omSoLkp] 
(
	@CompanyCode varchar(25),
	@BranchCode varchar(25)
)
as
 
 -- exec uspfn_omSoLkp '6115204001','6115204105'
 
 declare @DbMD as varchar(15)  
 declare @Sql as varchar(max) 
 declare @ssql as varchar(max) 
 
 set @DbMD = (select DbMD from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)  
 


 set @ssql='select * from gnMstCompanyMapping '

set @Sql= 'SELECT a.CompanyCode, a.BranchCode,
                a.SONo, a.SODate, a.SalesType, a.CustomerCode, a.TOPCode, a.Installment, a.FinalPaymentDate,
                a.TOPDays, a.BillTo, a.ShipTo, a.ProspectNo, a.SKPKNo, a.Salesman, a.WareHouseCode, a.isLeasing, 
                a.LeasingCo, a.GroupPriceCode, a.Insurance, a.PaymentType, a.PrePaymentAmt, a.PrePaymentBy, 
                a.CommissionBy, a.CommissionAmt, a.PONo, a.ContractNo, a.Remark, a.Status,
                a.SalesCoordinator, a.SalesHead, a.BranchManager, a.RefferenceNo,
                CASE convert(varchar, a.RefferenceDate, 111) when convert(varchar, ''1900/01/01'') 
                then '''' else convert(varchar, a.RefferenceDate, 111) end as RefferenceDates, 
                CASE convert(varchar, a.RefferenceDate, 111) when convert(varchar, ''1900/01/01'') 
                then ''undefined'' else convert(varchar, a.RefferenceDate, 111) end as RefferenceDate, 
                CASE convert(varchar, a.RequestDate, 111) when convert(varchar, ''1900/01/01'') 
                then ''undefined'' else convert(varchar, a.RequestDate, 111) end as RequestDate,
                CASE convert(varchar, a.PrePaymentDate, 111) when convert(varchar, ''1900/01/01'') 
                then ''undefined'' else convert(varchar, a.PrePaymentDate, 111) end as PrePaymentDate,
                e.Address1 + '' '' + e.Address2 + '' '' + e.Address3 + '' '' + e.Address4 as Address,
                case when year(a.RefferenceDate) <> 1900 then convert(bit,1) else convert(bit,0) end isC1,
                case when a.SKPKNo <> '''' then convert(bit,1) else convert(bit,0) end isC2,
                case when year(a.PrePaymentDate) <> 1900 then convert(bit,1) else convert(bit,0) end isC3,
                case when year(a.RequestDate) <> 1900 then convert(bit,1) else convert(bit,0) end isC4,
				(SELECT b.CustomerName
                        FROM gnMstCustomer b
                        WHERE a.CompanyCode = b.CompanyCode
						AND a.CustomerCode = b.CustomerCode)  
						AS CustomerName,
				(SELECT c.EmployeeName
                        FROM gnMstEmployee c
                        WHERE a.CompanyCode = c.CompanyCode AND a.BranchCode = c.BranchCode
					    AND a.Salesman = c.EmployeeID) as SalesmanName,
				(SELECT b.CustomerName
                        FROM gnMstCustomer b
                        WHERE a.CompanyCode = b.CompanyCode
						AND a.Shipto = b.CustomerCode)  
						AS ShipName,
                (SELECT b.CustomerName
                        FROM gnMstCustomer b
                        WHERE a.CompanyCode = b.CompanyCode
						AND a.LeasingCo = b.CustomerCode)  
						AS LeasingCoName,
				(SELECT c.EmployeeName
                        FROM gnMstEmployee c
                        WHERE a.CompanyCode = c.CompanyCode AND a.BranchCode = c.BranchCode
					    AND a.PrePaymentby = c.EmployeeID) as PrePaymentName,
				(SELECT d.RefferenceDesc1
                        FROM omMstRefference d
                        WHERE a.CompanyCode = d.CompanyCode
						AND d.RefferenceType = ''GRPR'' 
                        AND d.RefferenceCode = a.GroupPriceCode) AS GroupPriceName,
				(SELECT b.CustomerName
                        FROM gnMstCustomer b
                        WHERE a.CompanyCode = b.CompanyCode
						AND a.CustomerCode = b.CustomerCode)  
						AS BillName,
				(SELECT TOP 1 b.lookupvaluename
                        FROM '+@DbMD+'..gnMstLookUpDtl b
                        WHERE a.WareHouseCode = b.LookUpValue
						AND a.WareHouseCode = b.LookUpValue and CodeID =''MPWH'')  
						AS WareHouseName,
                (a.CustomerCode
                    + '' || ''
                    + (SELECT b.CustomerName
                        FROM gnMstCustomer b
                        WHERE a.CompanyCode = b.CompanyCode
						AND a.CustomerCode = b.CustomerCode))  
						AS Customer, 
                (a.Salesman
                    + '' || ''
                    + (SELECT c.EmployeeName
                        FROM gnMstEmployee c
                        WHERE a.CompanyCode = c.CompanyCode AND a.BranchCode = c.BranchCode
					    AND a.Salesman = c.EmployeeID))  AS Sales, 
                (a.GroupPriceCode
                    + '' || ''
                    + (SELECT d.RefferenceDesc1
                        FROM omMstRefference d
                        WHERE a.CompanyCode = d.CompanyCode
						AND d.RefferenceType = ''GRPR'' 
                        AND d.RefferenceCode = a.GroupPriceCode))  AS GroupPrice, 
                CASE a.Status when 0 then ''OPEN''
                                when 1 then ''PRINTED''
                                when 2 then ''APPROVED''
                                when 3 then ''DELETED''
                                when 4 then ''REJECTED''
                                when 9 then ''FINISHED'' END as Stat
                , CASE ISNULL(a.SalesType, 0) WHEN 0 THEN ''Wholesales'' ELSE ''Direct'' END AS TypeSales
                ,(select distinct x.TipeKendaraan
                    from pmKDP x
	                    left join gnMstEmployee b on x.CompanyCode=b.CompanyCode and x.BranchCode=b.BranchCode
		                    and x.EmployeeID=b.EmployeeID
	                    left join omTrSalesSO c on c.CompanyCode = x.CompanyCode 
		                    and c.BranchCode = x.BranchCode
		                    and c.ProspectNo = x.InquiryNumber
	                    left join omTrSalesInvoice d on d.CompanyCode = x.CompanyCode
		                    and d.BranchCode = x.BranchCode
		                    and d.SONo = c.SONo
	                    left join omTrSalesReturn e on e.CompanyCode = x.CompanyCode
		                    and e.BranchCode = x.BranchCode
		                    and e.InvoiceNo = d.InvoiceNo
                    where x.InquiryNumber=a.ProspectNo) as VehicleType
                FROM omTrSalesSO a
                INNER JOIN gnMstCustomer e
                ON a.CompanyCode = e.CompanyCode AND a.CustomerCode = e.CustomerCode
				Where a.CompanyCode = '+ @CompanyCode+' and a.BranchCode = '+ @BranchCode +'
				order by a.SONo desc
				'
--print @Sql

exec (@Sql)

GO

if object_id('uspfn_spSuppliers') is not null
	drop procedure uspfn_spSuppliers
GO

CREATE procedure [dbo].[uspfn_spSuppliers]  (  @CompanyCode varchar(10) ,@BranchCode varchar(10), @ProfitCenterCode char(3)) 
	   as
SELECT distinct
a.SupplierCode, a.SupplierName, (a.address1+' '+a.address2+' '+a.address3+' '+a.address4) as Alamat,
b.DiscPct as Diskon, (Case a.Status when 0 then 'Tidak Aktif' else 'Aktif' end) as [Status],
(SELECT Lookupvaluename FROM gnmstlookupdtl WHERE codeid='PFCN' 
 AND lookupvalue = b.ProfitCentercode) as Profit
FROM 
gnMstSupplier a
JOIN gnmstSupplierProfitCenter b ON a.CompanyCode= b.CompanyCode
AND a.SupplierCode = b.SupplierCode
WHERE 
a.CompanyCode=@CompanyCode
AND b.BranchCode=@BranchCode
AND b. ProfitCenterCode=@ProfitCenterCode
AND b.isBlackList=0
AND a.status = 1 
GO

if object_id('uspfn_SpOrderSparepartView') is not null
	drop procedure uspfn_SpOrderSparepartView
GO
create procedure [dbo].[uspfn_SpOrderSparepartView] @CompanyCode varchar(10) ,@BranchCode varchar(10), @TypeOfGoods varchar(2), @ProductType varchar(2)
as  
SELECT    
ItemInfo.PartNo,  
Items.ABCClass,  
ItemLoc.OnHand - itemLoc.ReservedSP - itemLoc.ReservedSR - itemLoc.ReservedSL - itemLoc.AllocationSP - itemLoc.AllocationSL - itemLoc.AllocationSR AS AvailQty,  
Items.OnOrder,  
Items.ReservedSP,  
Items.ReservedSR,  
Items.ReservedSL,  
Items.MovingCode,  
ItemInfo.SupplierCode,  
ItemInfo.PartName,  
ItemInfo.DiscPct,
ItemPrice.RetailPrice,  
ItemPrice.RetailPriceInclTax,  
ItemPrice.PurchasePrice  
FROM SpMstItems Items  
INNER JOIN SpMstItemInfo ItemInfo ON Items.CompanyCode  = ItemInfo.CompanyCode                            
                         AND Items.PartNo = ItemInfo.PartNo  
INNER JOIN spMstItemPrice ItemPrice ON Items.CompanyCode = ItemPrice.CompanyCode  
                        AND Items.BranchCode = ItemPrice.BranchCode AND Items.PartNo = ItemPrice.PartNo  
INNER JOIN spMstItemLoc ItemLoc ON Items.CompanyCode = ItemLoc.CompanyCode AND Items.BranchCode = ItemLoc.BranchCode  
                        AND Items.PartNo = ItemLoc.PartNo  
WHERE Items.CompanyCode = @CompanyCode  
  AND Items.BranchCode  = @BranchCode 
  AND Items.TypeOfGoods = @TypeOfGoods
  AND Items.ProductType = @ProductType  
  AND Items.Status > 0  
  AND ItemLoc.WarehouseCode = '00'
GO

if object_id('uspfn_SvTrnServiceSaveItem') is not null
	drop procedure uspfn_SvTrnServiceSaveItem
GO
create procedure [dbo].[uspfn_SvTrnServiceSaveItem]  
--DECLARE
	@CompanyCode varchar(15),  
	@BranchCode varchar(15),  
	@ProductType varchar(15),  
	@ServiceNo bigint,  
	@BillType varchar(15),  
	@PartNo varchar(20),  
	@DemandQty numeric(18,2),  
	@PartSeq numeric(5,2),  
	@UserID varchar(15),  
	@DiscPct numeric(5,2)  
as        
  
--set @CompanyCode = '6115204001'  
--set @BranchCode = '6115204102'  
--set @ProductType = '2W'  
--set @ServiceNo = 16455  
--set @BillType = 'C'  
--set @PartNo = 'K1200-50002-000'  
--set @DemandQty = 1 
--set @PartSeq = -1  
--set @UserID = 'yo'  
--set @DiscPct = 0  

declare @errmsg varchar(max)  
declare @QueryTemp as varchar(max)  
declare @IsSPK as char(1)
  
begin try  
 -- select data svTrnService  
 select * into #srv from (  
   select a.* from svTrnService a  
  where 1 = 1  
    and a.CompanyCode = @CompanyCode  
    and a.BranchCode  = @BranchCode  
    and a.ProductType = @ProductType  
    and a.ServiceNo   = @ServiceNo  
 )#srv  
   
 declare @CostPrice as decimal  
 declare @RetailPrice as decimal  
 declare @TypeOfGoods as varchar(2)  
 declare @CostPriceMD as decimal  
 declare @RetailPriceMD as decimal  
 declare @RetailPriceInclTaxMD as decimal  
   
 declare @DealerCode as varchar(2)  
 declare @CompanyMD as varchar(15)  
 declare @BranchMD as varchar(15)  
 declare @WarehouseMD as varchar(15)  
  
 set @DealerCode = 'MD'  
 set @CompanyMD = (select CompanyMD from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)  
 set @BranchMD = (select BranchMD from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)  
 set @WarehouseMD = (select WarehouseMD from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)  
 
if object_id('#tmpSvSDMovement') is not null drop table #tmpSvSDMovement
 
 -- Check MD or SD
	-- If SD  
 if (@CompanyCode != @CompanyMD and @BranchCode != @BranchMD)   
 begin  
	  set @DealerCode = 'SD'  

	  set @IsSPK = isnull((select a.ServiceType from #srv a where a.ServiceType = '2'),0)
	  
	  declare @DbName as varchar(50)  
	  set @DbName = (select DbMD from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)  
	  
	  declare @PurchaseDisc as decimal  
	  set @PurchaseDisc = (select isnull(DiscPct, 0) from gnMstSupplierProfitCenter   
		   where CompanyCode = @CompanyCode   
		   and BranchCode = @BranchCode  
		   and SupplierCode = @BranchMD
		   and ProfitCenterCode = '300')  
	         
	  if (@PurchaseDisc = 0) raiserror ('Purchase Discount belum di-setting untuk Part tersebut!',16,1);            
	       
	  declare @tblTemp as table  
	  (  
	   CostPrice decimal(18,2),  
	   RetailPrice decimal(18,2),  
	   RetailPriceInclTax decimal(18,2),  
	   TypeOfGoods varchar (2)  
	  )
	  
	  declare @tblTemp1 as table  
	  (  
	   CostPrice decimal(18,2),  
	   RetailPrice decimal(18,2),  
	   RetailPriceInclTax decimal(18,2),  
	   TypeOfGoods varchar (2)  
	  )
	    
	  -- Untuk ItemPrice Mengambil dari masing-masing dealer	
		set @QueryTemp = 'select   
			  b.CostPrice   
			 ,b.RetailPrice  
			 ,b.RetailPriceInclTax  
			 ,a.TypeOfGoods 
			from (select
				i.PartNo   
				,i.TypeOfGoods  
				 from ' + @DbName +'..spMstItems i  
				 where i.CompanyCode = ''' + @CompanyMD + '''  
				 and i.BranchCode  = ''' + @BranchMD + '''  
				 and i.PartNo      = ''' + @PartNo + '''
			) a inner join ' + @DbName +'..spMstItemPrice b on b.PartNo = a.PartNo
		 where b.CompanyCode = ''' + @CompanyMD + '''
		 and b.BranchCode = ''' + @BranchMD + ''''
		          
	  insert into @tblTemp    
	  exec (@QueryTemp)  
	  
		set @QueryTemp = 'select   
			  b.CostPrice   
			 ,b.RetailPrice  
			 ,b.RetailPriceInclTax  
			 ,a.TypeOfGoods 
			from (select
				i.PartNo   
				,i.TypeOfGoods  
				 from ' + @DbName +'..spMstItems i  
				 where i.CompanyCode = ''' + @CompanyMD + '''  
				 and i.BranchCode  = ''' + @BranchMD + '''  
				 and i.PartNo      = ''' + @PartNo + '''
			) a inner join ' + @DbName +'..spMstItemPrice b on b.PartNo = a.PartNo
		 where b.CompanyCode = ''' + @CompanyMD + '''
		 and b.BranchCode = ''' + @BranchMD + ''''
		 
  	  insert into @tblTemp1
	  exec (@QueryTemp)  
	  print (@QueryTemp)  

	  set @CostPrice = 0
	  EXEC uspfn_GetCostPrice @CompanyCode, @BranchCode, @PartNo , @CostPrice OUTPUT

	  --set @CostPrice = ((select RetailPriceInclTax from @tblTemp1) - ((select RetailPriceInclTax from @tblTemp1) * @PurchaseDisc * 0.01))  
	  --select @CostPrice  
	  set @RetailPrice = (select RetailPrice from @tblTemp)
	  --select a.RetailPrice from spMstItemPrice a where a.CompanyCode = @CompanyCode and a.BranchCode = @BranchCode and a.PartNo = @PartNo)    
	  set @TypeOfGoods = (select TypeOfGoods from @tblTemp)  
	    
	  set @CostPriceMD = (select CostPrice from @tblTemp)  
	  set @RetailPriceMD = (select RetailPrice from @tblTemp)  
	  set @RetailPriceInclTaxMD = (select RetailPriceInclTax from @tblTemp)  
	    
	  -- select @PurchaseDisc  
 end   
 -- If MD
 else  
 begin
	 declare @tblTempPart as table  
	  (  
	   CostPrice decimal(18,2),  
	   RetailPrice decimal(18,2),  
	   RetailPriceInclTax decimal(18,2),  
	   TypeOfGoods varchar (2)  
	  )  

	  set @QueryTemp = 'select   
		a.CostPrice   
	   ,a.RetailPrice  
		 from ' + @DbName + '..spMstItemPrice a  
	   where 1 = 1  
		 and a.CompanyCode = ''' + @CompanyMD + '''  
		 and a.BranchCode  = ''' + @BranchMD + '''  
		 and a.PartNo      = ''' + @PartNo + ''''  
	          
	  insert into @tblTempPart    
	  exec (@QueryTemp)  
	   
	  --select * into #part from (  
	  --select   
	  --  a.CostPrice   
	  -- ,a.RetailPrice  
	  --  from spMstItemPrice a  
	  -- where 1 = 1  
	  --   and a.CompanyCode = @CompanyCode  
	  --   and a.BranchCode  = @BranchCode  
	  --   and a.PartNo      = @PartNo  
	  --)#part  
	    
	  --set @CostPrice = (select CostPrice from @tblTempPart)  
	  set @RetailPrice = (select RetailPrice from @tblTempPart)  
 end  
 -- EOF Check MD or SD
  
 
 if (@PartSeq > 0)  
 begin    
	-- select data mst job  
	select * into #job from (  
	select b.*  
	from svTrnService a, svMstJob b  
	where 1 = 1  
	 and b.CompanyCode = a.CompanyCode  
	 and b.ProductType = a.ProductType  
	 and b.BasicModel = a.BasicModel  
	 and b.JobType = a.JobType  
		and a.CompanyCode = @CompanyCode  
	 and a.BranchCode  = @BranchCode  
	 and a.ServiceNo   = @ServiceNo  
	 and b.GroupJobType = 'FSC'  
	)#  
	if exists (select * from #job)  
	begin  
	   -- update svTrnSrvItem  
	   set @Querytemp ='
	   update svTrnSrvItem set  
		 DemandQty      = '+ convert(varchar,@DemandQty) +'
		,CostPrice      = '+ convert(varchar,@CostPrice) +' 
		,RetailPrice    = isnull((  
			 select top 1 b.RetailPrice from #srv a, svMstTaskPart b  
			  where b.CompanyCode = a.CompanyCode  
				and b.ProductType = a.ProductType  
				and b.BasicModel = a.BasicModel  
				and b.JobType = a.JobType  
				and b.PartNo = '''+ @PartNo +''' 
				and b.BillType = ''F'' 
			 ), (  
			  select top 1 isnull(RetailPrice, 0) RetailPrice  
				from spMstItemPrice  
			   where 1 = 1  
				 and CompanyCode = '''+ @CompanyCode +'''
				 and BranchCode = '''+ @BranchCode +'''
				 and PartNo = '''+ @PartNo  +'''
			  )  
			 )  
		,LastupdateBy   = (select LastupdateBy from #srv)  
		,LastupdateDate = (select LastupdateDate from #srv)  
		,BillType       = '''+ @BillType +'''
		,DiscPct        = '+ convert(varchar,@DiscPct) +'  
		where 1 = 1  
		  and CompanyCode  = '''+ @CompanyCode +''' 
		  and BranchCode   = '''+ @BranchCode +''' 
		  and ProductType  = (select ProductType from #srv)  
		  and ServiceNo    = (select ServiceNo from #srv)  
		  and PartNo       = '''+ @PartNo +''' 
		  and PartSeq      = '+ convert(varchar,@PartSeq) +'' 
		  
		  exec(@QueryTemp) 
	  end  
	  else  
	  begin  
	   -- update svTrnSrvItem  
	   update svTrnSrvItem set  
		 DemandQty      = @DemandQty  
		,CostPrice      = @CostPrice  
		,RetailPrice    = @RetailPrice  
		,LastupdateBy   = (select LastupdateBy from #srv)  
		,LastupdateDate = (select LastupdateDate from #srv)  
		,BillType       = @BillType  
		,DiscPct        = @DiscPct  
		where 1 = 1  
		  and CompanyCode  = @CompanyCode  
		  and BranchCode   = @BranchCode  
		  and ProductType  = (select ProductType from #srv)  
		  and ServiceNo    = (select ServiceNo from #srv)  
		  and PartNo       = @PartNo  
		  and PartSeq      = @PartSeq           
	  end   
	    
	--update svSDMovement  
	if (@DealerCode = 'SD' and @IsSPK = '2')  
	begin    
		set @QueryTemp = 'update ' + @DbName + '..svSDMovement set    
		QtyOrder    = ' + case when @DemandQty is null then '0' else convert(varchar, @DemandQty) end + ' 
		,DiscPct    = ' + case when  @DiscPct is null then '0' else convert(varchar, @DiscPct) end + '
		,CostPrice    = ' + case when @CostPrice is null then '0' else convert(varchar, @CostPrice) end + '  
		,RetailPrice   = ' + case when @RetailPrice is null then '0' else convert(varchar, @RetailPrice) end + '  
		,CostPriceMD   = ' + case when @CostPriceMD is null then '0' else convert(varchar, @CostPriceMD) end + '  
		,RetailPriceMD   = ' + case when @RetailPriceMD is null then '0' else convert(varchar, @RetailPriceMD) end + '  
		,RetailPriceInclTaxMD = ' + case when @RetailPriceInclTaxMD is null then '0' else convert(varchar, @RetailPriceInclTaxMD) end + '  
		,[Status]  = ''' + case when (select ServiceStatus from #srv) is null then '''' else (select ServiceStatus from #srv) end + '''  
		,LastupdateBy   = ''' + case when (select LastupdateBy from #srv) is null then '''' else (select LastupdateBy from #srv) end + '''  
		,LastupdateDate = ''' + case when (select LastupdateDate from #srv) is null then '''' else convert(varchar,(select LastupdateDate from #srv)) end + '''  
		where CompanyCode = ''' + case when @CompanyCode is null then '''' else @CompanyCode end + '''
		  and BranchCode = ''' + case when @BranchCode is null then '''' else @BranchCode end + '''
		  and DocNo  = ''' + case when (select JobOrderNo from #srv) is null then '''' else (select JobOrderNo from #srv) end + '''  
		  and PartNo       =  ''' + case when @PartNo is null then '''' else @PartNo end  + '''
		  and PartSeq      = ' + case when @PartSeq is null then '0' else convert(varchar, @PartSeq) end + '';  
		  
		  --print @QueryTemp;  
		exec 	(@QueryTemp);
	end
 end  
 else  
 begin  
	if((select count(*) from svTrnSrvItem  
	where 1 = 1  
	  and CompanyCode  = @CompanyCode  
	  and BranchCode   = @BranchCode  
	  and ProductType  = (select ProductType from #srv)  
	  and ServiceNo    = (select ServiceNo from #srv)  
	  and PartNo       = @PartNo  
	  and (isnull(SupplySlipNo,'') = '')  
	) > 0)  
	begin  
		raiserror ('Part yang sama sudah diproses di Entry SPK namun belum dapat No SSS, mohon diselesaikan dahulu!',16,1);  
	end  

	declare @PartSeqNew as int  
	set @PartSeqNew = (isnull((select isnull(max(PartSeq), 0) + 1    
	  from svTrnSrvItem   
		where CompanyCode = @CompanyCode  
	   and BranchCode  = @BranchCode   
	   and ProductType = @ProductType  
	   and ServiceNo   = @ServiceNo  
	   and PartNo      = @PartNo), 1))  
	     
	-- insert svTrnSrvItem  
	set @QueryTemp=' insert into svTrnSrvItem (CompanyCode, BranchCode, ProductType, ServiceNo, PartNo, PartSeq, DemandQty, SupplyQty, ReturnQty, CostPrice, RetailPrice, TypeOfGoods, BillType, SupplySlipNo, SupplySlipDate, SSReturnNo, SSReturnDate, CreatedBy, CreatedDate, LastupdateBy, LastupdateDate, DiscPct, MechanicID)  
	select   
	'''+ @CompanyCode +''' CompanyCode  
	,''' + @BranchCode +''' BranchCode  
	,'''+ @ProductType +''' ProductType  
	,'+ convert(varchar,@ServiceNo) +' ServiceNo  
	,a.PartNo  
	,'''+ convert(varchar,@PartSeqNew)  +'''
	--,(row_number() over (order by a.PartNo)) PartSeq  
	,'+ convert(varchar,@DemandQty )+' DemandQty  
	,''0'' SupplyQty  
	,''0'' ReturnQty  
	,'+ convert(varchar,isnull(@CostPrice,0))  +'
	,a.RetailPrice   
	,b.TypeOfGoods  
	,'''+ @BillType +''' BillType  
	,null SupplySlipNo  
	,null SupplySlipDate  
	,null SSReturnNo  
	,null SSReturnDate  
	,c.LastupdateBy CreatedBy  
	,c.LastupdateDate CreatedDate  
	,c.LastupdateBy  
	,c.LastupdateDate  
	,'+ convert(varchar,isnull(@DiscPct,0))  +'
	,(select MechanicID from svTrnService where CompanyCode = '''+ @CompanyCode +''' and BranchCode = '''+ @BranchCode +''' and ServiceNo = '+ convert(varchar,@ServiceNo) +')  
    from spMstItemPrice a, '+ @DbName +'..spMstItems b, 
    #srv c, gnmstcompanymapping d 
   where 1 = 1  
	 and d.CompanyMd = b.CompanyCode
	 and d.BranchMD = b.BranchCode
        and d.CompanyCode = c.CompanyCode  
     and d.BranchCode  = c.BranchCode  
     and b.PartNo      = a.PartNo  
        and (b.CompanyCode = '''+ @CompanyMD +'''
     and b.BranchCode  = '''+ @BranchMD +'''
     and b.PartNo      = '''+ @PartNo +''')
     and (a.CompanyCode = '''+ @CompanyCode +'''
     and a.BranchCode  = '''+ @BranchCode +'''
     and a.PartNo      = '''+ @PartNo +''')' 
		   
	exec(@QueryTemp)

	--print(@QueryTemp)

	--select   @CostPrice   
	--xxx

	if (@DealerCode = 'SD' and @IsSPK = '2')  
	begin
		create table #tmpSvSDMovement(
			CompanyCode varchar(15)
			,BranchCode varchar(15)
			,JobOrderNo varchar(20)   
			,JobOrderDate datetime  
			,PartNo varchar(20)
			,PartSeqNew int
			,WarehouseMD varchar(20)   
			,DemandQty numeric(18,2)
			,Qty numeric(18,2)
			,DiscPct numeric(18,2)
			,CostPrice numeric(18,2)
			,RetailPrice numeric(18,2) 
			,TypeOfGoods varchar(15) 
			,CompanyMD varchar(15)
			,BranchMD varchar(15)   
			,WarehouseMD2 varchar(15)
			,RetailPriceInclTaxMD numeric(18,2) 
			,RetailPriceMD numeric(18,2) 
			,CostPriceMD numeric(18,2)  
			,QtyFlag char(1)
			,ProductType varchar(15) 
			,ProfitCenterCode varchar(15)
			,Status char(1)
			,ProcessStatus char(1)
			,ProcessDate datetime 
			,CreatedBy varchar(15) 
			,CreatedDate datetime 
			,LastUpdateBy varchar(15) 
			,LastUpdateDate datetime	
		);

		insert into #tmpSvSDMovement 
			select case when @CompanyCode is null then '' else @CompanyCode end 
			,case when @BranchCode is null then '' else @BranchCode end
			,case when (select JobOrderNo from #srv) is null then convert(varchar,@ServiceNo) else (select JobOrderNo from #srv) end
			,case when (select JobOrderDate from #srv) is null then '1900/01/01' else (select JobOrderDate from #srv) end 
			,case when @PartNo is null then '' else  @PartNo end 
			,case when @PartSeqNew is null then '0' else convert(varchar, @PartSeqNew) end
			,case when @WarehouseMD is null then '' else @WarehouseMD end  
			,case when @DemandQty  is null then '0' else convert(varchar, @DemandQty) end
 			,case when @DemandQty  is null then '0' else convert(varchar, @DemandQty) end
			,case when @DiscPct is null then '0' else convert(varchar, @DiscPct) end  
			,case when @CostPrice is null then '0' else convert(varchar, @CostPrice) end 
			,case when @RetailPrice is null then '0' else convert(varchar, @RetailPrice) end  
			,case when @TypeOfGoods is null then '' else @TypeOfGoods end 
			,case when @CompanyMD is null then '' else @CompanyMD end   
			,case when @BranchMD is null then '' else @BranchMD end  
			,case when @WarehouseMD is null then '' else @WarehouseMD end  
			,case when @RetailPriceInclTaxMD is null then '0' else convert(varchar, @RetailPriceInclTaxMD) end  
			,case when @RetailPriceMD is null then '0' else convert(varchar, @RetailPriceMD) end   
			,case when @CostPriceMD is null then '0' else convert(varchar, @CostPriceMD) end
			,'x'
			,case when @ProductType is null then '' else @ProductType end  
			,'300'  
			,'0' 
			,'0'
			, GETDATE() 
			,case when (select CreatedBy from #srv) is null then '' else (select CreatedBy from #srv) end     
			,case when (select CreatedDate from #srv) is null then '1900/01/01' else convert(varchar,(select CreatedDate from #srv)) end 
			,case when (select LastUpdateBy from #srv) is null then '' else (select LastUpdateBy from #srv) end
			,case when (select LastUpdateDate from #srv) is null then '1900/01/01' else convert(varchar,(select LastUpdateDate from #srv)) end
		 
		declare @intCountTemp int
		set @intCountTemp = (select count(isnull(JobOrderNo,'')) DocNo from #tmpSvSDMovement)
		if (@intCountTemp > 0 ) begin 
			declare @intStringEmpty int
			set @intStringEmpty = (select count(isnull(JobOrderNo,'')) DocNo from #tmpSvSDMovement where JobOrderNo = '' or JobOrderNo is null)
			select @intCountTemp
			select @intStringEmpty
			if (@intStringEmpty < 1) begin
				set @QueryTemp = '
					insert into ' + @DbName + '..svSDMovement (CompanyCode, BranchCode, DocNo, DocDate, PartNo, PartSeq, WarehouseCode, QtyOrder, Qty, DiscPct, CostPrice, RetailPrice,   
					TypeOfGoods, CompanyMD, BranchMD, WarehouseMD, RetailPriceInclTaxMD, RetailPriceMD, CostPriceMD, QtyFlag, ProductType, ProfitCenterCode, 
					Status, ProcessStatus, ProcessDate, CreatedBy,   
					CreatedDate, LastUpdateBy, LastUpdateDate)  
					select * from #tmpSvSDMovement';
				exec(@QueryTemp);
			end
		end
		 
		drop table #tmpSvSDMovement;     
	end   
 end  

 update svTrnSrvItem  
    set DiscPct = @DiscPct  
  where 1 = 1  
    and CompanyCode = @CompanyCode  
    and BranchCode = @BranchCode  
    and ProductType = @ProductType  
    and ServiceNo = @ServiceNo  
    and PartNo = @PartNo  
   
 if (@DealerCode = 'SD' and @IsSPK = '2')  
 begin    
	set @QueryTemp = 'update ' + @DbName + '..svSDMovement   
	  set DiscPct = ' + convert(varchar,@DiscPct) 
	  + ' where 1 = 1'  
	  +	' and CompanyCode = ''' + case when @CompanyMD is null then '''' else  @CompanyMD end + ''''
	  + ' and BranchCode = ''' + case when @BranchMD is null then '''' else  @BranchMD end + ''''
	  + ' and DocNo = ''' + case when (select JobOrderNo from #srv) is null then '''' else (select JobOrderNo from #srv) end  + ''''
	  + ' and PartNo = ''' + case when @PartNo is null then '''' else @PartNo end + ''''  
	  + ' and PartSeq = ' + convert(varchar,@PartSeq) + '';
  
	exec (@QueryTemp)  
 end  
   
	drop table #srv  
end try  
begin catch  
 set @errmsg = error_message()  
 raiserror (@errmsg,16,1);  
end catch  

--rollback tran

GO

if object_id('uspfn_GenerateSSPickingslipNew') is not null
	drop procedure uspfn_GenerateSSPickingslipNew
GO
create procedure [dbo].[uspfn_GenerateSSPickingslipNew]
	@CompanyCode	VARCHAR(MAX),
	@BranchCode		VARCHAR(MAX),
	@JobOrderNo		VARCHAR(MAX),
	@ProductType	VARCHAR(MAX),
	@CustomerCode	VARCHAR(MAX),
	@TransType		VARCHAR(MAX),
	@UserID			VARCHAR(MAX),
	@DocDate		DATETIME
AS
BEGIN

--declare	@CompanyCode	VARCHAR(MAX)
--declare	@BranchCode		VARCHAR(MAX)
--declare	@JobOrderNo		VARCHAR(MAX)
--declare	@ProductType	VARCHAR(MAX)
--declare	@CustomerCode	VARCHAR(MAX)
--declare	@TransType		VARCHAR(MAX)
--declare	@UserID			VARCHAR(MAX)
--declare	@DocDate		DATETIME

--set	@CompanyCode	= '6156401000'
--set	@BranchCode		= '6156401001'
--set	@JobOrderNo		= 'SPK/15/001833'
--set	@ProductType	= '4W'
--set	@CustomerCode	= '000003'
--set	@TransType		= '20'
--set	@UserID			= 'TRAININGZZZ'
--set	@DocDate		= '3/12/2015 9:47:01 AM'


--exec uspfn_GenerateSSPickingslipNew '6006400001','6006400101','SPK/14/101589','4W','2105885','20','ga','3/2/2015 4:03:01 PM'
--================================================================================================================================
-- TABLE MASTER
--================================================================================================================================
-- Temporary for Item --
------------------------
SELECT * INTO #Item FROM (
SELECT a.CompanyCode
	, a.BranchCode
	, a.ProductType
	, a.ServiceNo
	, a.RetailPrice
	, a.PartNo
	, a.Billtype
	, SUM(ISNULL(a.DemandQty, 0) - (ISNULL(a.SupplyQty, 0))) QtyOrder
FROM svTrnSrvItem a WITH (NOLOCK, NOWAIT)
INNER JOIN svTrnService b ON b.CompanyCode = a.CompanyCode
	AND b.BranchCode = a.BranchCode
	AND b.ProductType = a.ProductType
	AND b.ServiceNo = a.ServiceNo
	AND b.JobOrderNo = @JobOrderNo
WHERE a.CompanyCode = @CompanyCode 
	AND a.BranchCode = @BranchCode 
	AND a.ProductType = @ProductType 
GROUP BY a.CompanyCode, a.BranchCode, a.ProductType
	, a.ServiceNo, a.PartNo, a.RetailPrice, a.BillType ) #Item 

DECLARE @CompanyMD AS VARCHAR(15)
DECLARE @BranchMD AS VARCHAR(15)
DECLARE @WarehouseMD AS VARCHAR(15)

SET @CompanyMD = (SELECT CompanyMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)
SET @BranchMD = (SELECT BranchMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)
SET @WarehouseMD = (SELECT WarehouseMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)

SELECT * INTO #SrvOrder FROM (
SELECT DISTINCT(a.CompanyCode) 
    , a.BranchCode
	, a.ProductType
	, a.ServiceNo
	, a.PartNo
    , (SELECT Paravalue FROM gnMstLookUpDtl WHERE CompanyCode = a.CompanyCode AND CodeID = 'GTGO' AND LookUpValue = a.TypeOfGoods) TipePart
    , (SELECT PartName FROM spMstItemInfo WHERE CompanyCode = a.CompanyCode AND PartNo = a.PartNo) PartName
	, a.RetailPrice
	, a.CostPrice
    , a.TypeOfGoods
    , a.BillType
	, SUM(a.QtyOrder) QtyOrder
    , 0 QtySupply
    , 0 QtyBO
    , (SUM(a.QtyOrder) * a.RetailPrice) * ((100 - a.PartDiscPct)/100) NetSalesAmt
    , a.PartDiscPct DiscPct
FROM
(
	SELECT
		DISTINCT(a.CompanyCode) 
		, a.BranchCode
		, a.ProductType
		, a.ServiceNo
		, a.PartNo
		, a.RetailPrice
		, a.CostPrice
		, a.TypeOfGoods
		, a.BillType
		, ISNULL(Item.QtyOrder,0) AS QtyOrder
		, a.DiscPct PartDiscPct 
	FROM
		svTrnSrvItem a WITH (NOLOCK, NOWAIT)
		LEFT JOIN svTrnService b WITH (NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode
			AND a.BranchCode = b.BranchCode	
			AND a.ProductType = b.ProductType
			AND a.ServiceNo = b.ServiceNo
		LEFT JOIN #Item Item ON Item.CompanyCode = a.CompanyCode 
			AND Item.BranchCode = a.BranchCode 
			AND Item.ProductType = a.ProductType 
			AND Item.ServiceNo = a.ServiceNo 
			AND Item.PartNo = a.PartNo 
			AND Item.RetailPrice = a.RetailPrice 
			AND Item.BillType = a.Billtype
		--LEFT JOIN SpMstItemPrice c WITH (NOLOCK, NOWAIT) ON a.CompanyCode = c.CompanyCode 
		--	AND a.BranchCode = c.BranchCode 
		--	AND a.PartNo = c.PartNo
	WHERE
		a.CompanyCode = @CompanyCode
		AND a.BranchCode = @BranchCode
		AND a.ProductType = @ProductType
		AND Item.QtyOrder > 0
		AND JobOrderNo = @JobOrderNo
		AND (a.SupplySlipNo is null OR a.SupplySlipNo = '')
) a
GROUP BY
	a.CompanyCode
	, a.BranchCode
	, a.ProductType
	, a.ServiceNo
	, a.PartNo
	, a.RetailPrice
	, a.CostPrice
    , a.TypeOfGoods
    , a.BillType
    , a.PartDiscPct 
) #SrvOrder

select * from #srvorder

--================================================================================================================================
-- INSERT TABLE SpTrnSORDHdr AND SpTrnSORDDtl
--================================================================================================================================
DECLARE @MaxDocNo			INT
DECLARE	@MaxPickingList		INT
DECLARE @TempDocNo			VARCHAR(MAX)
DECLARE @TempPickingList	VARCHAR(MAX)
DECLARE @TypeOfGoods		VARCHAR(MAX)
DECLARE @DefaultDate		DATETIME

SET @DefaultDate = '1900-01-01 00:00:00.000'

--===============================================================================================================================
-- LOOPING BASED ON THE TYPE OF GOODS
-- ==============================================================================================================================
DECLARE db_cursor CURSOR FOR
SELECT DISTINCT TypeOfGoods FROM #SrvOrder
WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND ProductType = @ProductType 

OPEN db_cursor
FETCH NEXT FROM db_cursor INTO @TypeOfGoods

WHILE @@FETCH_STATUS = 0
BEGIN

--===============================================================================================================================
-- INSERT HEADER
-- ==============================================================================================================================
SET @MaxDocNo = ISNULL((SELECT DocumentSequence + 1 FROM GnMstDocument WHERE 
	CompanyCode = @CompanyCode
		AND BranchCode = @BranchCode
		AND DocumentType = 'SSS' 
		AND ProfitCenterCode = '300' 
		AND DocumentYear = YEAR(GetDate())),0)

SET @TempDocNo = ISNULL((SELECT 'SSS/' + RIGHT(YEAR(GETDATE()),2) +'/' + RIGHT(1000000 + CONVERT(VARCHAR, @MaxDocNo, 1), 6)),'SSS/YY/XXXXXX')

INSERT INTO SpTrnSORDHdr
([CompanyCode]
           ,[BranchCode]
           ,[DocNo]
           ,[DocDate]
           ,[UsageDocNo]
           ,[UsageDocDate]
           ,[CustomerCode]
           ,[CustomerCodeBill]
           ,[CustomerCodeShip]
           ,[isBO]
           ,[isSubstitution]
           ,[isIncludePPN]
           ,[TransType]
           ,[SalesType]
           ,[IsPORDD]
           ,[OrderNo]
           ,[OrderDate]
           ,[TOPCode]
           ,[TOPDays]
           ,[PaymentCode]
           ,[PaymentRefNo]
           ,[TotSalesQty]
           ,[TotSalesAmt]
           ,[TotDiscAmt]
           ,[TotDPPAmt]
           ,[TotPPNAmt]
           ,[TotFinalSalesAmt]
           ,[isPKP]
           ,[ExPickingSlipNo]
           ,[ExPickingSlipDate]
           ,[Status]
           ,[PrintSeq]
           ,[TypeOfGoods]
           ,[isDropsign]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastUpdateBy]
           ,[LastUpdateDate]
           ,[isLocked]
           ,[LockingBy]
           ,[LockingDate])

SELECT 
	@CompanyCode CompanyCode
	, @BranchCode BranchCode
	, @TempDocNo DocNo 
	, @DocDate DocDate
	, @JobOrderNo UsageDocNo
	, (SELECT JobOrderDate FROM SvTrnService WHERE 1 =1 AND CompanyCode = @CompanyCode AND BranchCode = @BranchCode 
		AND ProductType = @ProductType AND JobOrderNo = @JobOrderNo) UsageDocDate
	, (SELECT CustomerCode FROM SvTrnService WHERE 1 = 1AND CompanyCode = @CompanyCode AND BranchCode = @BranchCode 
		AND ProductType = @ProductType AND JobOrderNo = @JobOrderNo) CustomerCode
	, (SELECT CustomerCodeBill FROM SvTrnService WHERE 1 = 1 AND CompanyCode = @CompanyCode AND BranchCode = @BranchCode 
		AND ProductType = @ProductType AND JobOrderNo = @JobOrderNo) CustomerCodeBill
	, (SELECT CustomerCode FROM SvTrnService WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode 
		AND ProductType = @ProductType AND JobOrderNo = @JobOrderNo) CustomerCodeShip
	, CONVERT(BIT, 0) isBO
	, CONVERT(BIT, 0) isSubstitution
	, CONVERT(BIT, 1) isIncludePPN
	, @TransType TransType
	, '2' SalesType
	, CONVERT(BIT, 0) isPORDD
	, @JobOrderNo OrderNo
	, @DocDate OrderDate
	, ISNULL((SELECT TOPCode FROM GnMstCustomerProfitCenter WITH (NOLOCK, NOWAIT) WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
		AND ProfitCenterCode = '300' AND CustomerCode = @CustomerCode),'W') TOPCode
	, ISNULL((SELECT ParaValue FROM GnMstLookUpDtl WITH (NOLOCK, NOWAIT) WHERE CompanyCode = @CompanyCode AND CodeID = 'TOPC' AND 
		LookupValue IN 
		(SELECT TOPCode FROM GnMstCustomerProfitCenter WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
			AND ProfitCenterCode = '300' AND CustomerCode = @CustomerCode)
	  ),0) TOPDays
	, ISNULL((SELECT PaymentCode FROM GnMstCustomerProfitCenter WITH (NOLOCK, NOWAIT) WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
		AND ProfitCenterCode = '300' AND CustomerCode = @CustomerCode),'W') PaymentCode
	, '' PaymentReffNo
	, 0 TotSalesQty
	, 0 TotSalesAmt
	, 0 TotDiscAmt
	, 0 TotDPPAmt
	, 0 TotPPNAmt
	, 0 TotFinalSalesAmt
	, CONVERT(BIT, 0) isPKP
	, NULL ExPickingSlipNo
	, NULL ExPickingSlipDate
	, '4' Status
	, 0 PrintSeq
	, @TypeOfGoods TypeOfGoods
	, NULL IsDropSign
	, @UserID CreatedBY
	, GetDate() CreatedDate
	, @UserID LastUpdateBy
	, GetDate() LastUpdateDate
	, NULL isLocked
	, NULL LockingBy
	, NULL LockingDate


UPDATE GnMstDocument
SET DocumentSequence = DocumentSequence + 1
	, LastUpdateDate = GetDate()
	, LastUpdateBy = @UserID
WHERE
	CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND DocumentPrefix = 'SSS'
	AND ProfitCenterCode = '300'
	AND DocumentYear = Year(GetDate())

--===============================================================================================================================
-- INSERT DETAIL
-- ==============================================================================================================================
DECLARE @DbMD AS VARCHAR(15)
SET @DbMD = (SELECT DbMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)

declare @TempAvailStock as table
(
	PartNo varchar(50),
	AvailStock decimal
)

DECLARE @Query AS VARCHAR(MAX)
--SET @Query = 
--'SELECT PartNo, (Onhand - AllocationSP - AllocationSL - AllocationSR - ReservedSP - ReservedSL - ReservedSR) AvailStock
--FROM ' + @DbMD + '..SpMstItemLoc WITH (NOLOCK, NOWAIT) 
--WHERE CompanyCode = '+''''+@CompanyMD+''''+' AND BranchCode ='+''''+@BranchMD +''''+' AND WarehouseCode = '+''''+@WarehouseMD+''''+''

--INSERT INTO #TempAvailStock

SET @Query = 
'SElect * into #TempAvailStock from (SELECT PartNo, (Onhand - AllocationSP - AllocationSL - AllocationSR - ReservedSP - ReservedSL - ReservedSR) AvailStock
FROM ' + @DbMD + '..SpMstItemLoc WITH (NOLOCK, NOWAIT) 
WHERE CompanyCode = '+''''+@CompanyMD+''''+' AND BranchCode ='+''''+@BranchMD +''''+' AND WarehouseCode = '+''''+@WarehouseMD+''''+')#TempAvailStock

INSERT INTO SpTrnSORDDtl 
(
	[CompanyCode] ,
	[BranchCode] ,
	[DocNo] ,
	[PartNo] ,
	[WarehouseCode] ,
	[PartNoOriginal] ,
	[ReferenceNo] ,
	[ReferenceDate] ,
	[LocationCode] ,
	[QtyOrder] ,
	[QtySupply] ,
	[QtyBO] ,
	[QtyBOSupply] ,
	[QtyBOCancel] ,
	[QtyBill] ,
	[RetailPriceInclTax] ,
	[RetailPrice] ,
	[CostPrice] ,
	[DiscPct] ,
	[SalesAmt] ,
	[DiscAmt] ,
	[NetSalesAmt] ,
	[PPNAmt] ,
	[TotSalesAmt] ,
	[MovingCode] ,
	[ABCClass] ,
	[ProductType] ,
	[PartCategory] ,
	[Status] ,
	[CreatedBy] ,
	[CreatedDate] ,
	[LastUpdateBy] ,
	[LastUpdateDate] ,
	[StockAllocatedBy] ,
	[StockAllocatedDate] ,
	[FirstDemandQty] )
SELECT
	''' + @CompanyCode +''' CompanyCode
	, ''' + @BranchCode +''' BranchCode
	, ''' + @TempDocNo +''' DocNo 
	, a.PartNo
	, ''00'' WarehouseCode
	, a.PartNo PartNoOriginal
	, ''' + @JobOrderNo +''' ReferenceNo
	, (SELECT JobOrderDate FROM SvTrnService WHERE 1 =1 AND CompanyCode = ''' + @CompanyCode +''' AND BranchCode = ''' + @BranchCode +'''
		AND ProductType = ''' + @ProductType +''' AND JobOrderNo = ''' + @JobOrderNo +''' ) ReferenceDate
	, (SELECT distinct LocationCode FROM ' + @DbMD +'..SpMstItemLoc WHERE CompanyCode = ''' + @CompanyMD +''' AND BranchCode = ''' + @BranchMD +''' AND WarehouseCode = ''00''
		AND PartNo = a.PartNo ) LocationCode
	, a.QtyOrder
	, CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN a.QtyOrder 
		ELSE ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0)
		END AS QtySupply
	, CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN 0 
		ELSE a.QtyOrder - ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0)
		END AS QtyBO
	, 0 QtyBOSupply
	, CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN 0 
		ELSE a.QtyOrder - ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0)
		END AS QtyBOCancel
	, 0 QtyBill
	, a.RetailPrice + FLOOR(a.RetailPrice * 10 /100) RetailPriceIncltax
	, a.RetailPrice
	, a.CostPrice
	, a.DiscPct
	, CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN a.QtyOrder * a.RetailPrice
		ELSE ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) * a.RetailPrice
		END AS SalesAmt
	, CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN floor((a.QtyOrder * a.RetailPrice) * a.DiscPct/100)
		ELSE floor((ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) * a.RetailPrice) * a.DiscPct/100)
		END AS DiscAmt
	, CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN floor((a.QtyOrder * a.RetailPrice)- ((a.QtyOrder * a.RetailPrice) * a.DiscPct/100))
		ELSE floor((ISNULL((SELECT AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) * a.RetailPrice) - 
			(ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) * a.RetailPrice * a.DiscPct/100))
		END AS NetSalesAmt
	, 0 PPNAmt
	,  CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN floor((a.QtyOrder * a.RetailPrice)- ((a.QtyOrder * a.RetailPrice) * a.DiscPct/100))
		ELSE floor((ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) * a.RetailPrice) - 
			(ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) * a.RetailPrice * a.DiscPct/100))
		END AS TotSalesAmt
	, (SELECT distinct MovingCode FROM '+ @DbMD +'..SpMstItems WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyMD +''' AND BranchCode = '''+ @BranchMD +''' AND PartNo = a.PartNo) 
		MovingCode
	, (SELECT distinct ABCClass FROM '+ @DbMD +'..SpMstItems WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyMD +''' AND BranchCode = '''+ @BranchMD +''' AND PartNo = a.PartNo) 
		ABCClass
	, '''+ @ProductType +''' ProductType
	, (SELECT distinct PartCategory FROM '+ @DbMD +'..SpMstItems WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyMD +'''  AND BranchCode = '''+ @BranchMD +''' AND PartNo = a.PartNo) 
		PartCategory
	, ''2'' Status
	, '''+ @UserID +''' CreatedBy
	, '''+ Convert(varchar,GetDate()) +''' CreatedDate
	, '''+ @UserID +''' LastUpdateBy
	, '''+ Convert(varchar,GetDate()) +''' LastUpdateDate
	, '''+ @UserID +''' StockAllocatedBy
	, '''+ Convert(varchar,GetDate()) +''' StockAllocatedDate
	, a.QtyOrder FirstDemandQty
FROM #SrvOrder a
WHERE a.TypeOfGoods = '+@TypeOfGoods +'


select top 10 * from spTrnSORDDtl order by createddate desc
--===============================================================================================================================
-- INSERT SO SUPPLY
-- ==============================================================================================================================

SELECT * INTO #TempSOSupply FROM (
SELECT
	'''+ @CompanyCode +''' CompanyCode
	, '''+ @BranchCode +''' BranchCode
	, '''+ @TempDocNo +''' DocNo 
	, 0 SupSeq
	, a.PartNo 
	, a.PartNo PartNoOriginal
	, '''' PickingSlipNo	
	, '''+ @JobOrderNo +''' ReferenceNo
	, '''+ CONVERT(varchar, @DefaultDate )+''' ReferenceDate
	, ''00'' WarehouseCode
	, (SELECT distinct LocationCode FROM '+ @DbMD+'..SpMstItemLoc WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyMD +''' AND BranchCode = '''+ @BranchMD+''' AND WarehouseCode = ''00''
		AND PartNo = a.PartNo) LocationCode
	, CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN a.QtyOrder 
		ELSE ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0)
		END AS QtySupply
	, 0 QtyPicked
	, 0 QtyBill
	, a.RetailPrice + FLOOR(a.RetailPrice *10 /100) RetailPriceIncltax
	, a.RetailPrice
	, b.CostPrice
	, a.DiscPct
	, (SELECT distinct MovingCode FROM '+ @DbMD+'..SpMstItems WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyMD +''' AND BranchCode = '''+ @BranchMD +''' AND PartNo = a.PartNo) 
		MovingCode
	, (SELECT distinct ABCClass FROM '+ @DbMD +'..SpMstItems WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyMD +''' AND BranchCode = '''+ @BranchMD +''' AND PartNo = a.PartNo) 
		ABCClass
	, '''+ @ProductType +'''ProductType
	, (SELECT distinct PartCategory FROM '+ @DbMD +'..SpMstItems WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyMD +''' AND BranchCode = '''+ @BranchMD +''' AND PartNo = a.PartNo) 
		PartCategory
	, ''1'' Status
	, '''+ @UserID +''' CreatedBy
	, '''+ Convert(varchar,GetDate()) +''' CreatedDate
	, '''+ @UserID +''' LastUpdateBy
	, '''+ Convert(varchar,GetDate()) +''' LastUpdateDate
	, '''+ @UserID +''' StockAllocatedBy
	, '''+ Convert(varchar,GetDate()) +''' StockAllocatedDate
FROM #SrvOrder a
inner join '+ @DbMD +'..spMstItemPrice b WITH (NOLOCK, NOWAIT) ON a.CompanyCode = '''+ @CompanyCode +''' AND a.BranchCode = '''+ @BranchCode +''' AND a.PartNo = b.PartNo
WHERE a.TypeOfGoods = '+ @TypeOfGoods +'
)#TempSOSupply

INSERT INTO SpTrnSOSupply SELECT 
	CompanyCode,BranchCode,DocNo,SupSeq,PartNo,PartNoOriginal
	, ROW_NUMBER() OVER(ORDER BY PartNo) PTSeq,PickingSlipNo
	, ReferenceNo,ReferenceDate,WarehouseCode,LocationCode
	, QtySupply,QtyPicked,QtyBill,RetailPriceIncltax,RetailPrice,CostPrice
	, DiscPct,MovingCode,ABCClass,ProductType,PartCategory,Status
	, CreatedBy,CreatedDate,LastUpdateBy,LastUpdateDate
FROM #TempSOSupply WHERE QtySupply > 0

--===============================================================================================================================
-- UPDATE STATUS DETAIL BASED ON SUPPLY
--===============================================================================================================================

UPDATE SpTrnSORDDtl
SET Status = 4
FROM SpTrnSORDDtl a, #TempSOSupply b
WHERE 1 = 1
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.PartNo = b.PartNo
	
--===============================================================================================================================
-- UPDATE HISTORY DEMAND ITEM AND CUSTOMER
--===============================================================================================================================

UPDATE SpHstDemandItem 
SET DemandFreq = DemandFreq + 1
	, DemandQty = DemandQty + b.QtyOrder
	, LastUpdateBy = '''+ @UserID +'''
	, LastUpdateDate = '''+ Convert(varchar,GetDate()) +''' 
FROM SpHstDemandItem a, SpTrnSordDtl b
WHERE 
	1 = 1
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.Year = '''+ Convert(varchar,Year(GetDate())) +''' 
	AND a.Month  = '''+ Convert(varchar,Month(GetDate())) +''' 
	AND a.PartNo = b.PartNo
	AND b.DocNo = '''+ @TempDocNo +'''

UPDATE SpHstDemandCust
SET DemandFreq = DemandFreq + 1
	, DemandQty = DemandQty + b.QtyOrder
	, LastUpdateBy = '''+ @UserID +''' 
	, LastUpdateDate = '''+ Convert(varchar,GetDate()) +''' 
FROM SpHstDemandCust a, SpTrnSordDtl b
WHERE 
	1 = 1
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.Year = '''+ Convert(varchar,Year(GetDate())) +'''
	AND a.Month  = '''+ Convert(varchar,Month(GetDate())) +'''
	AND a.PartNo = b.PartNo
	AND a.CustomerCode = '''+ @CustomerCode +'''
	AND b.DocNo = '''+ @TempDocNo +'''

INSERT INTO SpHstDemandItem
SELECT 
	CompanyCode
	, BranchCode
	, '''+ Convert(varchar,Year(GetDate())) +''' Year
	, '''+ Convert(varchar,Month(GetDate())) +''' Month
	, PartNo
	, 1 DemandFreq
	, QtyOrder DemandQty
	, 0 SalesFreq
	, 0 SalesQty
	, MovingCode
	, ProductType
	, PartCategory
	, ABCClass
	, '''+ @UserID +''' LastUpdateBy
	, '''+ CONVERT(varchar, GetDate()) +''' LastUpdateDate
FROM SpTrnSordDtl a WITH (NOLOCK, NOWAIT)
WHERE a.CompanyCode= '''+ @CompanyCode +''' AND a.BranchCode = '''+ @BranchCode +''' AND a.DocNo = '''+ @TempDocNo +''' -- add CompanyCode and BranchCode 13 Des 2010
 AND NOT EXISTS
( SELECT 1 FROM SpHstDemandItem WITH (NOLOCK, NOWAIT) WHERE 
	1 = 1 
	AND CompanyCode = a.CompanyCode 
	AND BranchCode = a.BranchCode
	AND Month = '''+ Convert(varchar,Month(GetDate())) +''' 
	AND Year = '''+ Convert(varchar,Year(GetDate())) +''' 
	AND PartNo = a.PartNo
)

INSERT INTO SpHstDemandCust
SELECT 
	CompanyCode
	, BranchCode
	, '''+ Convert(varchar,Year(GetDate())) +'''  Year
	, '''+ Convert(varchar,Month(GetDate())) +'''  Month
	, '''+ @CustomerCode +''' CustomerCode
	, PartNo
	, 1 DemandFreq
	, (SELECT QtyOrder FROM SpTrnSORDDTl WITH (NOLOCK, NOWAIT) 
		WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode 
		AND DocNo = a.DocNo AND PartNo = a.PartNo) DemandQty
	, 0 SalesFreq
	, 0 SalesQty
	, MovingCode
	, ProductType
	, PartCategory
	, ABCClass
	, '''+ @UserID +''' LastUpdateBy
	, '''+ CONVERT(varchar, GetDate()) +''' LastUpdateDate
FROM SpTrnSordDtl a WITH (NOLOCK, NOWAIT)
WHERE a.CompanyCode= '''+ @CompanyCode +''' and a.BranchCode= '''+ @BranchCode +''' AND a.DocNo = '''+ @TempDocNo +''' -- add CompanyCode and BranchCode 13 Des 2010
AND NOT EXISTS
( SELECT PartNo FROM SpHstDemandCust WITH (NOLOCK, NOWAIT) WHERE 
	1 = 1 
	AND CompanyCode = a.CompanyCode 
	AND BranchCode = a.BranchCode
	AND Month = '''+ Convert(varchar,Month(GetDate())) +''' 
	AND Year = '''+ Convert(varchar,Year(GetDate())) +'''  
	AND PartNo = a.PartNo
)

--===============================================================================================================================
-- UPDATE LAST DEMAND DATE MASTER
--===============================================================================================================================

UPDATE '+@DbMD+'..SpMstItems 
SET LastDemandDate = '''+Convert(varchar,GetDate())+'''
FROM '+@DbMD+'..SpMstItems a, SpTrnSordDtl b
WHERE 
	1 = 1
	AND a.CompanyCode = '''+@CompanyMD+'''
	AND a.BranchCode = '''+@BranchMD+'''
	AND a.PartNo = b.PartNo
	AND b.DocNo = '''+@TempDocNo+'''

--===============================================================================================================================
-- UPDATE STOCK AND MOVEMENT
--===============================================================================================================================

UPDATE '+@DbMD+'..spMstItems
SET AllocationSR = AllocationSR + b.QtySupply
	, LastUpdateBy = '''+@UserID+'''
	, LastUpdateDate = '''+Convert(varchar,GetDate())+'''
FROM '+@DbMD+'..SpMstItems a, #TempSOSupply b
WHERE 
	1 = 1
	AND a.CompanyCode = '''+@CompanyMD+'''
	AND a.BranchCode = '''+@BranchMD+'''
	AND a.PartNo = b.PartNo

UPDATE '+@DbMD+'..spMstItemloc
SET AllocationSR = AllocationSR + b.QtySupply
	, LastUpdateBy = '''+@UserID+'''
	, LastUpdateDate = '''+Convert(varchar,GetDate())+'''
FROM '+@DbMD+'..SpMstItemLoc a, #TempSOSupply b
WHERE 
	1 = 1
	AND a.CompanyCode = '''+@CompanyMD +'''
	AND a.BranchCode = '''+@BranchMD+'''
	AND a.WarehouseCode = '''+@WarehouseMD+'''
	AND a.PartNo = b.PartNo

INSERT INTO SpTrnIMovement
SELECT
	'''+@CompanyCode +''' CompanyCode
	, '''+@BranchCode +''' BranchCode
	, a.DocNo
	, (SELECT DocDate FROM SpTrnSORDHdr WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyCode +'''
		AND BranchCode = '''+ @BranchCode +''' AND DocNo = a.DocNo) 
	  DocDate
	, dateadd(s,ROW_NUMBER() OVER(Order by a.PartNo),'''+convert(varchar,getdate())+''') CreatedDate 
	, ''00'' WarehouseCode
	, (SELECT LocationCode FROM SpTrnSORDDtl WITH (NOLOCK, NOWAIT) WHERE CompanyCode =  '''+@CompanyCode +'''
		AND BranchCode = '''+@BranchCode +''' AND DocNo = '''+@TempDocNo +''' AND PartNo = a.PartNo)
	  LocationCode
	, a.PartNo
	, ''OUT'' SignCode
	, ''SA-NPJUAL'' SubSignCode
	, a.QtySupply
	, a.RetailPrice
	, a.CostPrice
	, a.ABCClass
	, a.MovingCode
	, a.ProductType
	, a.PartCategory
	, '''+@UserID +''' CreatedBy
FROM #TempSOSupply a

--===============================================================================================================================
-- UPDATE SUPPLY SLIP TO SPK
--===============================================================================================================================
DECLARE @ServiceNo VARCHAR(MAX)

SET @ServiceNo = (SELECT ServiceNo FROM svTrnService WHERE CompanyCode = '''+@CompanyCode +''' AND BranchCode = '''+@BranchCode+'''
		AND ProductType = '''+@ProductType +''' AND JobOrderNo = '''+@JobOrderNo+''')

SELECT * INTO #TempServiceItem FROM (
SELECT 
	a.CompanyCode
	, a.BranchCode
	, a.ProductType
	, a.ServiceNo
	, a.PartNo
	, a.PartSeq
	, a.DemandQty
	, a.SupplyQty
	, b.QtySupply
	, b.DocNo
	, a.CostPrice
	, a.RetailPrice
	, a.TypeOfGoods
	, a.BillType
FROM SvTrnSrvItem a WITH (NOLOCK, NOWAIT)
INNER JOIN #TempSOSupply b WITH (NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.PartNo = b.PartNo
WHERE
	1 = 1
	AND a.CompanyCode = '''+@CompanyCode+'''
	AND a.BranchCode = '''+@BranchCode+'''
	AND a.ProductType = '''+@ProductType+'''
	AND a.ServiceNo = @ServiceNo
	AND a.PartSeq IN (SELECT MAX(PartSeq) FROM SvTrnSrvItem WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode
		AND ProductType = '''+@ProductType +''' AND ServiceNo = a.ServiceNo AND PartNo = a.PartNo)
	AND (a.SupplySlipNo = '''' OR a.SupplySlipNo IS NULL)
) #TempServiceItem 

SELECT * INTO #TempServiceItemIns FROM( 
SELECT 
	a.CompanyCode
	, a.BranchCode
	, a.ProductType
	, a.ServiceNo
	, a.PartNo
	, a.PartSeq
	, a.DemandQty
	, a.SupplyQty
	, b.QtySupply
	, b.DocNo
	, a.CostPrice
	, a.RetailPrice
	, a.TypeOfGoods
	, a.BillType
	, a.DiscPct
FROM SvTrnSrvItem a WITH (NOLOCK, NOWAIT)
INNER JOIN #TempSOSupply b WITH (NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.PartNo = b.PartNo
WHERE
	1 = 1 
	AND a.CompanyCode = '''+ @CompanyCode +''' 
	AND a.BranchCode = '''+ @BranchCode +''' 
	AND a.ProductType = '''+ @ProductType +'''  
	AND a.ServiceNo = @ServiceNo
	AND a.PartSeq IN (SELECT MAX(PartSeq) FROM SvTrnSrvItem WITH (NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode
		AND ProductType = '''+ @ProductType +''' AND ServiceNo = a.ServiceNo AND PartNo = a.PartNo) 
	AND (a.SupplySlipNo != '''' OR a.SupplySlipNo IS NOT NULL)
) #TempServiceItemIns


UPDATE svTrnSrvItem
SET SupplySlipNo = b.DocNo
	, SupplySlipDate = ISNULL((SELECT DocDate FROM SpTrnSORDHdr WITH (NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode
							AND DocNo = b.DocNo),'''+Convert(varchar,@DefaultDate)+''')
FROM svTrnSrvItem a, #TempServiceItem b
WHERE
	1 = 1
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.ProductType = b.ProductType
	AND a.ServiceNo = b.ServiceNo
	AND a.PartNo = b.PartNo
	AND a.PartSeq = b.PartSeq
	
--===============================================================================================================================
-- INSERT NEW SRV ITEM BASED SUPPLY SLIP
--===============================================================================================================================
INSERT INTO SvTrnSrvItem (CompanyCode, BranchCode, ProductType, ServiceNo, PartNo, PartSeq, DemandQty, SupplyQty, ReturnQty, CostPrice, RetailPrice, TypeOfGoods, BillType, SupplySlipNo, SupplySlipDate, SSReturnNo, SSReturnDate, CreatedBy, CreatedDate, LastupdateBy, LastupdateDate, DiscPct)
SELECT 
	a.CompanyCode
	, a.BranchCode
	, a.ProductType
	, a.ServiceNo
	, a.PartNo
	, a.PartSeq + 1
	, 0 DemandQty
	, 0 SupplyQty
	, 0 ReturnQty
	, a.CostPrice
	, a.RetailPrice
	, a.TypeOfGoods
	, a.BillType
	, a.DocNo SupplySlipNo
	, (SELECT DocDate FROM SpTrnSORDHdr WITH (NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode
		AND DocNo = a.DocNo) SupplySlipDate
	, NULL SSReturnNo
	, NULL SSReturnDate
	, '''+ @UserID +''' CreatedBy
	, '''+ Convert(varchar,GetDate()) +''' CreatedDate
	, '''+ @UserID +''' LastUpdateBy
	, '''+ Convert(varchar,GetDate()) +''' LastUpdateDate
	, a.DiscPct
FROM #TempServiceItemIns a WITH (NOLOCK, NOWAIT)
WHERE
	1 = 1
	AND a.CompanyCode = '''+ @CompanyCode +'''
	AND a.BranchCode = '''+ @BranchCode +'''
	AND a.ProductType = '''+ @ProductType+'''


--===============================================================================================================================
DROP TABLE #TempServiceItem 
DROP TABLE #TempServiceItemIns
DROP TABLE #TempSOSupply'

EXEC(@query)

--select convert(xml,@query)


--===============================================================================================================================
-- INSERT SVSDMOVEMENT
--===============================================================================================================================

declare @md bit
set @md = (select case WHEN EXISTS(select * from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode and CompanyMD = @CompanyCode and BranchMD = @BranchCode) then cast(1 as bit) ELSE cast(0 as bit) END)

if(@md = 0)
begin

 declare @QueryTemp as varchar(max)  
 
	set @Query ='insert into '+ @DbMD +'..svSDMovement
	select a.CompanyCode, a.BranchCode, a.DocNo, a.CreatedDate, a.PartNo
	, Seq = convert(integer, ROW_NUMBER() OVER (PARTITION BY a.ReferenceNo ORDER BY a.DocNo)) ,
	a.WarehouseCode, a.QtyOrder, a.QtySupply, a.DiscPct
	--,isnull(((select RetailPrice from spTrnSORDDtl
	--		where CompanyCode = ''' + @CompanyCode + '''  and BranchCode = ''' + @BranchCode  + '''
	--		and DocNo = ''' + @TempDocNo + ''' and PartNo = a.PartNo) / 1.1 * 
	--		((100 - isnull((select isnull(DiscPct, 0) from gnMstSupplierProfitCenter   
	--			where CompanyCode = ''' + @CompanyCode + ''' and BranchCode = ''' + @BranchCode  + ''' and SupplierCode = dbo.GetBranchMD(''' + @CompanyCode + ''', ''' + @BranchCode  + ''') 
	--			and ProfitCenterCode = ''300''),0)) * 0.01)),0) 
	, a.CostPrice
	, a.RetailPrice, b.TypeOfGoods
	, '''+ @CompanyMD +''','''+ @BranchMD +''','''+ @WarehouseMD +''', md.RetailPriceInclTax, md.RetailPrice, md.CostPrice
	,''x'','''+ @producttype +''',''300'',''0'',''0'','''+ convert(varchar,GETDATE()) +''','''+ @UserID +''','''+ convert(varchar,GETDATE()) +'''
	,'''+ @UserID +''','''+ convert(varchar,GETDATE()) +'''
	from spTrnSORDDtl a 
	join spTrnSORDHdr b on a.CompanyCode = b.CompanyCode
	and a.BranchCode = b.BranchCode 
	and a.DocNo = b.DocNo
	join '+ @DbMD +'..spmstitemprice md
	on md.CompanyCode = '''+ @CompanyMD +'''
	and md.branchcode = '''+ @BranchMD +'''
	and md.PartNo = a.PartNo
	where a.CompanyCode = '''+ @CompanyCode +''' 
	and a.BranchCode = '''+ @BranchCode +'''
	and a.ReferenceNo = '''+ @JobOrderNo +'''
	and a.DocNo = ''' + @TempDocNo + '''';

	exec (@Query)
	print (@QUERY)

end

--===============================================================================================================================
-- INSERT PICKING HEADER AND DETAIL
--===============================================================================================================================

SET @MaxPickingList = ISNULL((SELECT DocumentSequence + 1 FROM GnMstDocument WHERE 
	CompanyCode = @CompanyCode
		AND BranchCode = @BranchCode
		AND DocumentType = 'PLS' 
		AND ProfitCenterCode = '300' 
		AND DocumentYear = YEAR(GetDate())),0)

SET @TempPickingList = ISNULL((SELECT 'PLS/' + RIGHT(YEAR(GETDATE()),2) +'/' + RIGHT(1000000 + CONVERT(VARCHAR, @MaxPickingList, 1), 6)),'PLS/YY/XXXXXX')

INSERT INTO SpTrnSPickingHdr 
SELECT 
	CompanyCode
	, BranchCode
	, @TempPickingList PickingSlipNo
	, GetDate() PickingSlipDate
	, CustomerCode
	, CustomerCodeBill
	, CustomerCodeShip
	, '' PickedBy
	, CONVERT(BIT, 0) isBORelease
	, isSubstitution
	, isIncludePPN
	, TransType
	, SalesType
	, TotSalesQty
	, TotSalesAmt
	, TotDiscAmt
	, TotDPPAmt
	, TotPPNAmt
	, TotFinalSalesAmt
	, '' Remark
	, '0' Status
	, '0' PrintSeq
	, TypeOfGoods
	, CreatedBy
	, CreatedDate
	, LastUpdateBy
	, LastUpdateDate
	, NULL isLocked
	, NULL LockingBy
	, NULL LockingDate
FROM SpTrnSORDHdr WITH (NOLOCK, NOWAIT)
WHERE 
	1 = 1
	AND CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND DocNo = (SELECT distinct DocNo FROM spTrnSORDDtl WHERE CompanyCode = @CompanyCode AND Branchcode = @BranchCode 
					AND DocNo = @TempDocNo AND QtySupply > 0)		

UPDATE GnMstDocument
SET DocumentSequence = DocumentSequence + 1
	, LastUpdateDate = GetDate()
	, LastUpdateBy = @UserID
WHERE
	1 = 1
	AND CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND DocumentPrefix = 'PLS'
	AND ProfitCenterCode = '300'
	AND DocumentYear = Year(GetDate())

-- ==============================================================================================================================
-- UPDATE SALES ORDER HEADER 
-- ==============================================================================================================================
UPDATE SpTrnSORDHdr
	SET ExPickingSlipNo = @TempPickingList,
		ExPickingSlipDate = ISNULL((SELECT PickingSlipDate FROM SpTrnSPickingHdr WHERE CompanyCode = @CompanyCode
				AND BranchCode = @BranchCode AND PickingSlipNo = @TempPickingList),'')
	
WHERE
	1 = 1
	AND CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND DocNo = @TempDocNo

UPDATE SpTrnSOSupply
	SET PickingSlipNo = @TempPickingList
WHERE
	1 = 1
	AND CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND DocNo = @TempDocNo
-- ==============================================================================================================================
-- INSERT PICKING DETAIL
-- ==============================================================================================================================

INSERT INTO SpTrnSPickingDtl
SELECT 
	a.CompanyCode
	, a.BranchCode
	, @TempPickingList PickingSlipNo
	, '00' WarehouseCode
	, a.PartNo
	, a.PartNoOriginal
	, a.DocNo
	, b.DocDate 
	, a.ReferenceNo
	, a.ReferenceDate
	, a.LocationCode
	, a.QtySupply QtyOrder
	, a.QtySupply
	, a.QtySupply QtyPicked 
	, 0 QtyBill
	, a.RetailPriceInclTax
	, a.RetailPrice
	, a.CostPrice
	, a.DiscPct
	, a.SalesAmt
	, a.DiscAmt
	, a.NetSalesAmt
	, a.TotSalesAmt
	, a.MovingCode
	, a.ABCClass
	, a.ProductType
	, a.PartCategory
	, '' ExPickingSlipNo
	, @DefaultDate ExPickingSlipDate
	, CONVERT(BIT, 0) isClosed
	, @UserID CreatedBy
	, GetDate() CreatedDate
	, @UserID LastUpdateBy
	, GetDate() LastUpdateDate
FROM SpTrnSORDDtl a WITH (NOLOCK, NOWAIT)
INNER JOIN SpTrnSORDHdr b WITH (NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.DocNo = b.DocNo
WHERE
	1 = 1
	AND a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND a.DocNo = @TempDocNo
	AND a.QtySupply > 0


--================================================================================================================================
-- UPDATE AMOUNT HEADER
--================================================================================================================================
SELECT * INTO #TempHeader FROM (
SELECT 
	header.CompanyCode
	, header.BranchCode
	, header.DocNo
	, header.TotSalesQty
	, header.TotSalesAmt
	, header.TotDiscAmt
	, header.TotDPPAmt
	, floor(header.TotDPPAmt * (ISNULL((SELECT TaxPct FROM GnMstTax WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
			AND TaxCode IN (SELECT TaxCode FROM GnMstCustomerProfitCenter WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
			AND ProfitCenterCode = '300' AND CustomerCode = @CustomerCode)),0)/100)) 
		TotPPNAmt
	, header.TotDPPAmt + floor(header.TotDPPAmt * (ISNULL((SELECT TaxPct FROM GnMstTax WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
			AND TaxCode IN (SELECT TaxCode FROM GnMstCustomerProfitCenter WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
			AND ProfitCenterCode = '300' AND CustomerCode = @CustomerCode)),0)/100))
		TotFinalSalesAmt
FROM (
SELECT 
	CompanyCode
	, BranchCode
	, DocNo
	, SUM(QtyOrder) TotSalesQty
	, SUM(SalesAmt) TotSalesAmt
	, SUM(DiscAmt) TotDiscAmt
	, SUM(NetSalesAmt) TotDPPAmt
FROM SpTrnSORDDtl WITH (NOLOCK, NOWAIT)
WHERE CompanyCode = @CompanyCode 
	AND BranchCode = @BranchCode
	AND DocNo = @TempDocNo
GROUP BY CompanyCode
	, BranchCode
	, DocNo
) header ) #TempHeader

UPDATE SpTrnSORDHdr
SET 
	TotSalesQty = b.TotSalesQty
	, TotSalesAmt = b.TotSalesAmt
	, TotDiscAmt = b.TotDiscAmt
	, TotDPPAmt = b.TotDPPAmt
	, TotPPNAmt = b.TotPPNAmt
	, TotFinalSalesAmt = b.TotFinalSalesAmt
FROM SpTrnSORDHdr a, #TempHeader b
WHERE 
	1 = 1
	AND a.CompanyCode = b.CompanyCode 
	AND a.BranchCode = b.BranchCode
	AND a.DocNo = b.DocNo

DROP TABLE #TempHeader

FETCH NEXT FROM db_cursor INTO @TypeOfGoods
END
CLOSE db_cursor
DEALLOCATE db_cursor 

--===============================================================================================================================
-- Update Transdate
--===============================================================================================================================

update gnMstCoProfileSpare
set TransDate=getdate()
	, LastUpdateBy=@UserID
	, LastUpdateDate=getdate()
where CompanyCode = @CompanyCode AND BranchCode = @BranchCode

--===============================================================================================================================
-- DROP TABLE SECTION 
--===============================================================================================================================
DROP TABLE #SrvOrder
DROP TABLE #Item

--rollback tran
END
GO

if object_id('uspfn_GenerateBPSLampiranNew') is not null
	drop procedure uspfn_GenerateBPSLampiranNew
GO
create procedure [dbo].[uspfn_GenerateBPSLampiranNew] 
(
	@CompanyCode	VARCHAR(MAX),
	@BranchCode		VARCHAR(MAX),
	@JobOrderNo		VARCHAR(MAX),
	@ProductType	VARCHAR(MAX),
	@CustomerCode	VARCHAR(MAX),
	@UserID			VARCHAR(MAX),
	@PickedBy		VARCHAR(MAX)
)
AS
BEGIN

/*
PSEUDOCODE PROCESS :
Line 38  : RE-CALCULATE AMOUNT DETAIL
Line 93  : RE-CALCULATE AMOUNT HEADER AND UPDATE STATUS
Line 140 : UPDATE SO SUPPLY AND STATUS HEADER 
Line 167 : UPDATE SUPPLY SLIP QTY SERVICE 
Line 237 : INSERT NEW SRV ITEM BASED PICKING LIST
Line 276 : INSERT BPS AND LAMPIRAN
Line 292 : INSERT BPS HEADER
Line 352 : INSERT BPS DETAIL
Line 395 : INSERT LAMPIRAN HEADER
Line 458 : INSERT LAMPIRAN DETAIL
Line 500 : UPDATE STOCK
Line 571 : UPDATE DEMAND CUST AND DEMAND ITEM
Line 611 : INSERT TO ITEM MOVEMENT
Line 650 : UPDATE TRANSDATE
*/

--DECLARE	@CompanyCode	VARCHAR(MAX),
--		@BranchCode		VARCHAR(MAX),
--		@JobOrderNo		VARCHAR(MAX),
--		@ProductType	VARCHAR(MAX),
--		@CustomerCode	VARCHAR(MAX),
--		@UserID			VARCHAR(MAX),
--		@PickedBy		VARCHAR(MAX)

--SET	@CompanyCode	= '6156401000'
--SET	@BranchCode		= '6156401001'
--SET	@JobOrderNo		= 'SPK/15/001666'
--SET	@ProductType	= '4W'
--SET	@CustomerCode	= '0032710'
--SET	@UserID			= 'ga'
--SET	@PickedBy		= '0004'
		
--exec uspfn_GenerateBPSLampiranNew '6006400001','6006400101','SPK/14/101625','4W','JKT-1852626','ga','00001'

--===============================================================================================================================
-- RE-CALCULATE AMOUNT DETAIL
--===============================================================================================================================
DECLARE @DefaultDate		DATETIME

SET @DefaultDate = '1900-01-01 00:00:00.000'

if object_id('#tmpSvSDMovement') is not null drop table #tmpSvSDMovement

DECLARE @DbMD AS VARCHAR(15)
SET @DbMD = (SELECT DbMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)

  declare @PurchaseDisc as decimal
  set @PurchaseDisc = (select isnull(DiscPct, 0) from gnMstSupplierProfitCenter   
       where CompanyCode = @CompanyCode   
       and BranchCode = @BranchCode  
       and SupplierCode = dbo.GetBranchMD(@CompanyCode,@BranchCode)
       and ProfitCenterCode = '300')  
         
   if (@PurchaseDisc = 0) raiserror ('Purchase Discount belum di-setting untuk Part tersebut!',16,1);   

SELECT * INTO #TempPickingSlipDtl FROM (
SELECT
	a.CompanyCode
	, a.BranchCode 
	, a.PickingSlipNo
	, a.PickingSlipDate
	, a.CustomerCode
	, a.TypeOfGoods
	, b.DocNo
	, b.PartNo
	, b.QtyPicked
	, b.QtyPicked * b.RetailPrice SalesAmt
	, Floor((b.QtyPicked * b.RetailPrice) * DiscPct/100) DiscAmt
	, (b.QtyPicked * b.RetailPrice) - Floor((b.QtyPicked * b.RetailPrice) * DiscPct/100) NetSalesAmt
FROM SpTrnSPickingHdr a WITH (NOLOCK, NOWAIT)
INNER JOIN SpTrnSPickingDtl b WITH (NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode 
	AND a.BranchCode = b.BranchCode AND a.PickingSlipNo = b.PickingSlipNo
WHERE 
	1 = 1
	AND a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND Status < 2
	AND b.DocNo IN (SELECT DocNo FROM SpTrnSordHdr WITH (NOLOCK, NOWAIT)
				WHERE 
					1 = 1
					AND CompanyCode =@CompanyCode
					AND BranchCode = @BranchCode
					AND UsageDocNo = @JobOrderNo
					AND Status = 4)
) #TempPickingSlipDtl

UPDATE SpTrnSPickingDtl
SET SalesAmt = b.SalesAmt 
	, DiscAmt = b.DiscAmt
	, NetSalesAmt = b.NetSalesAmt
	, TotSalesAmt = b.NetSalesAmt
	, QtyBill = b.QtyPicked
	, LastUpdateBy = @UserID
	, LastUpdateDate = GetDate()
FROM SpTrnSPickingDtl a, #TempPickingSlipDtl b
WHERE
	1 = 1
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.PickingSlipNo = b.PickingSlipNo
	AND a.PartNo = b.PartNo

--===============================================================================================================================
-- RE-CALCULATE AMOUNT HEADER AND UPDATE STATUS
--===============================================================================================================================
SELECT * INTO #TempPickingSlipHdr FROM (
SELECT
	a.CompanyCode
	, a.BranchCode
	, a.PickingSlipNo	
	, SUM(b.QtyPicked) TotSalesQty
	, SUM(b.SalesAmt) TotSalesAmt
	, SUM(b.DiscAmt) TotDiscAmt
	, SUM(b.NetSalesAmt) TotDPPAmt
	, floor(SUM(b.NetSalesAmt) * (ISNULL((SELECT TaxPct FROM GnMstTax x WITH (NOLOCK, NOWAIT) WHERE x.CompanyCode = @CompanyCode AND x.TaxCode IN 
		(SELECT TaxCode FROM GnMstCustomerProfitCenter y WITH (NOLOCK, NOWAIT) WHERE y.CompanyCode = @CompanyCode AND y.BranchCode = @BranchCode
			AND y.ProfitCenterCode = '300' AND y.CustomerCode = @CustomerCode)),0)/100))
	  TotPPNAmt
FROM spTrnSPickingHdr a WITH (NOLOCK, NOWAIT)
LEFT JOIN spTrnSPickingDtl b WITH (NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.PickingSlipNo = b.PickingSlipNo
WHERE
	1 = 1
	AND a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND a.PickingSlipNo IN (SELECT DISTINCT(PickingSlipNo) FROM #TempPickingSlipDtl WITH (NOLOCK, NOWAIT))
GROUP BY a.CompanyCode
	, a.BranchCode
	, a.PickingSlipNo	
) #TempPickingSlipHdr

UPDATE SpTrnSPickingHdr
SET TotSalesQty = b.TotSalesQty
	, TotSalesAmt = b.TotSalesAmt
	, TotDiscAmt = b.TotDiscAmt
	, TotDPPAmt = b.TotDPPAmt
	, TotPPNAmt = b.TotPPNAmt
	, TotFinalSalesAmt = b.TotDPPAmt + b.TotPPNAmt
	, Status = 2
	, PickedBy = @PickedBy
	, LastUpdateBy = @UserID
	, LastUpdateDate = GetDate()
FROM SpTrnSPickingHdr a, #TempPickingSlipHdr b
WHERE
	1 = 1
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.PickingSlipNo = b.PickingSlipNo

--===============================================================================================================================
-- UPDATE SO SUPPLY AND STATUS HEADER 
--===============================================================================================================================
UPDATE SpTrnSOSupply
SET	Status = 2
	, QtyPicked = b.QtyPicked
	, QtyBill = b.QtyPicked
	, LastUpdateBy = @UserID
	, LastUpdateDate = GetDate()
FROM SpTrnSOSupply a, #TempPickingSlipDtl b
WHERE
	1 = 1
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.DocNo = b.DocNo
	AND a.PartNo = b.PartNo

UPDATE SpTrnSORDHdr 
SET Status = 5
	, LastUpdateBy = @UserID
	, LastUpdateDate = GetDate()
WHERE
	1 = 1
	AND CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND DocNo IN (SELECT DISTINCT(DocNo) FROM #TempPickingSlipDtl)

--===============================================================================================================================
-- UPDATE SUPPLY SLIP QTY SERVICE 
--===============================================================================================================================
DECLARE @ServiceNo VARCHAR(MAX)

SET @ServiceNo = (SELECT ServiceNo FROM svTrnService WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
		AND ProductType = @ProductType AND JobOrderNo = @JobOrderNo)

SELECT * INTO #TempServiceItem FROM (
SELECT 
	a.CompanyCode
	, a.BranchCode
	, a.ProductType
	, a.ServiceNo
	, a.PartNo
	, a.PartSeq
	, a.DemandQty
	, a.SupplyQty
	, b.QtyBill
	, b.DocNo
	, c.RetailPriceInclTax - (c.RetailPriceInclTax * isnull(d.DiscPct, 0) * 0.01) CostPrice
	, a.RetailPrice
	, a.TypeOfGoods
	, a.BillType
	, a.DiscPct
FROM SvTrnSrvItem a WITH (NOLOCK, NOWAIT)
INNER JOIN SpTrnSPickingDtl b WITH (NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.PartNo = b.PartNo AND a.SupplySlipNo = b.DocNo
INNER JOIN SpMstItemPrice c ON a.CompanyCode = c.CompanyCode AND a.BranchCode = c.BranchCode AND a.PartNo = c.PartNo
INNER JOIN gnMstSupplierProfitCenter d ON c.CompanyCode = d.CompanyCode AND a.BranchCode = d.BranchCode
WHERE
	1 = 1
	AND a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND a.ProductType = @ProductType
	AND a.ServiceNo = @ServiceNo
	AND a.PartSeq IN (SELECT MAX(PartSeq) FROM SvTrnSrvItem WITH (NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode
		AND ProductType = @ProductType AND ServiceNo = a.ServiceNo AND PartNo = a.PartNo)
	AND a.SupplySlipNo = b .DocNo
	AND d.SupplierCode = dbo.GetBranchMD(@CompanyCode,@BranchCode)
	AND d.ProfitCenterCode = '300'
) #TempServiceItem 

UPDATE svTrnSrvItem
SET SupplyQty = (CASE WHEN b.QtyBill > b.DemandQty 
				THEN 
					CASE WHEN b.DemandQty = 0 THEN b.QtyBill ELSE b.DemandQty END
				ELSE b.QtyBill END)
	, LastUpdateBy = @UserID
	, LastUpdateDate = Getdate()
FROM svTrnSrvItem a, #TempServiceItem b
WHERE
	1 = 1
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.ProductType = b.ProductType
	AND a.ServiceNo = b.ServiceNo
	AND a.PartNo = b.PartNo
	AND a.PartSeq = b.PartSeq
	AND a.SupplySlipNo = b.DocNo

UPDATE svTrnSrvItem
SET CostPrice = b.CostPrice
	, LastUpdateBy = @UserID
	, LastUpdateDate = Getdate()
FROM svTrnSrvItem a, #TempServiceItem b
WHERE
	1 = 1
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.ProductType = b.ProductType
	AND a.ServiceNo = b.ServiceNo
	AND a.PartNo = b.PartNo
	AND a.SupplySlipNo = b.DocNo

--===============================================================================================================================
-- INSERT NEW SRV ITEM BASED PICKING LIST
--===============================================================================================================================
INSERT INTO SvTrnSrvItem (CompanyCode, BranchCode, ProductType, ServiceNo, PartNo, PartSeq, DemandQty, SupplyQty, ReturnQty, CostPrice, RetailPrice, TypeOfGoods, BillType, SupplySlipNo, SupplySlipDate, SSReturnNo, SSReturnDate, CreatedBy, CreatedDate, LastupdateBy, LastupdateDate, DiscPct)
SELECT 
	a.CompanyCode
	, a.BranchCode
	, a.ProductType
	, a.ServiceNo
	, a.PartNo
	, a.PartSeq + 1
	, 0 DemandQty
	, a.QtyBill - a.DemandQty SupplyQty
	, 0 ReturnQty
	, a.CostPrice
	, a.RetailPrice
	, a.TypeOfGoods
	, a.BillType
	, a.DocNo SupplySlipNo
	, (SELECT DocDate FROM SpTrnSORDHdr WITH (NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode
		AND DocNo = a.DocNo) SupplySlipDate
	, NULL SSReturnNo
	, NULL SSReturnDate
	, @UserID CreatedBy
	, GetDate() CreatedDate
	, @UserID LastUpdateBy
	, GetDate() LastUpdateDate
	, a.DiscPct
FROM #TempServiceItem a WITH (NOLOCK, NOWAIT)
WHERE
	1 = 1
	AND a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND a.ProductType = @ProductType
	AND a.DemandQty < a.QtyBill
	AND a.QtyBill > 0
	AND a.DemandQty > 0

DROP TABLE #TempServiceItem 

--===============================================================================================================================
-- INSERT BPS AND LAMPIRAN
--===============================================================================================================================
DECLARE @PickingSlipNo	VARCHAR(MAX)
DECLARE	@TempBPSFNo		VARCHAR(MAX)
DECLARE	@TempLMPNo		VARCHAR(MAX)
DECLARE @MaxBPSFNo		INT
DECLARE @MaxLmpNo		INT

DECLARE db_cursor CURSOR FOR
SELECT DISTINCT PickingSlipNo FROM #TempPickingSlipHdr
OPEN db_cursor
FETCH NEXT FROM db_cursor INTO @PickingSlipNo

WHILE @@FETCH_STATUS = 0
BEGIN	

--===============================================================================================================================
-- INSERT BPS HEADER
--===============================================================================================================================
SET @MaxBPSFNo = ISNULL((SELECT DocumentSequence + 1 FROM GnMstDocument WHERE 
	CompanyCode = @CompanyCode
		AND BranchCode = @BranchCode
		AND DocumentType = 'BPF' 
		AND ProfitCenterCode = '300' 
		AND DocumentYear = YEAR(GetDate())),0)

SET @TempBPSFNo = ISNULL((SELECT 'BPF/' + RIGHT(YEAR(GETDATE()),2) + '/' + RIGHT(1000000 + CONVERT(VARCHAR, @MaxBPSFNo, 1), 6)),'BPF/YY/XXXXXX')

INSERT INTO SpTrnSBPSFHdr
SELECT 
	CompanyCode
	, BranchCode
	, @TempBPSFNo BPSFNo
	, GetDate() BPSFDate
	, PickingSlipNo
	, PickingSlipDate
	, TransType
	, SalesType
	, CustomerCode
	, CustomerCodeBill
	, CustomerCodeShip
	, TotSalesQty
	, TotSalesAmt
	, TotDiscAmt
	, TotDPPAmt
	, TotPPNAmt
	, TotFinalSalesAmt
	, '2' Status
	, 0 PrintSeq
	, TypeOfGoods
	, @UserID CreatedBy
	, GetDate() CreatedDate
	, @UserID LastUpdateBy
	, GetDate() LastUpdateDate
	, NULL isLocked
	, NULL LockingBy
	, NULL LockingDate
FROM SpTrnSPickingHdr WITH (NOLOCK, NOWAIT)
WHERE
	1 = 1
	AND CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND PickingSlipNo = @PickingSlipNo

UPDATE GnMstDocument
SET DocumentSequence = DocumentSequence + 1
	, LastUpdateDate = GetDate()
	, LastUpdateBy = @UserID
WHERE
	1 = 1
	AND CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND DocumentPrefix = 'BPF'
	AND ProfitCenterCode = '300'
	AND DocumentYear = Year(GetDate())

--===============================================================================================================================
-- INSERT BPS DETAIL
--===============================================================================================================================
INSERT INTO SpTrnSBPSFDtl
SELECT
	CompanyCode
	, BranchCode
	, @TempBPSFNo
	, WarehouseCode
	, PartNo
	, PartNoOriginal
	, DocNo
	, DocDate
	, ReferenceNo
	, ReferenceDate
	, LocationCode
	, QtyBill
	, RetailPriceInclTax
	, RetailPrice
	, CostPrice
	, DiscPct
	, SalesAmt
	, DiscAmt
	, NetSalesAmt
	, 0 PPNAmt
	, TotSalesAmt
	, ProductType
	, PartCategory 
	, MovingCode
	, ABCClass
	, '' ExPickingListNo
	, @DefaultDate ExPickingListDate
	, @UserID CreatedBy
	, GetDate() CreatedDate
	, @UserID LastUpdateBy
	, GetDate() LastUpdateDate
FROM SpTrnSPickingDtl WITH (NOLOCK, NOWAIT)
WHERE 
	1 = 1
	AND CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND PickingSlipNo = @PickingSlipNo

--===============================================================================================================================
-- INSERT LAMPIRAN HEADER
--===============================================================================================================================
SET @MaxLmpNo = ISNULL((SELECT DocumentSequence + 1 FROM GnMstDocument WHERE 
	CompanyCode = @CompanyCode
		AND BranchCode = @BranchCode
		AND DocumentType = 'LMP' 
		AND ProfitCenterCode = '300' 
		AND DocumentYear = YEAR(GetDate())),0)

SET @TempLmpNo = ISNULL((SELECT 'LMP/' + RIGHT(YEAR(GETDATE()),2) +'/' + RIGHT(1000000 + CONVERT(VARCHAR, @MaxLmpNo, 1), 6)),'LMP/YY/XXXXXX')

INSERT INTO SpTrnSLmpHdr
SELECT
	CompanyCode
	, BranchCode
	, @TempLmpNo LmpNo	
	, GetDate() LmpDate
	, @TempBPSFNo BPSFNo
	, (SELECT BPSFDate FROM SpTrnSBPSFHdr WITH (NOLOCK, NOWAIT) WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND BPSFNo = @TempBPSFNo)
		BPSFDate
	, PickingSlipNo
	, PickingSlipDate
	, TransType
	, CustomerCode
	, CustomerCodeBill
	, CustomerCodeShip
	, TotSalesQty
	, TotSalesAmt
	, TotDiscAmt
	, TotDPPAmt
	, TotPPNAmt
	, TotFinalSalesAmt
	, CONVERT(BIT, 0) isPKP
	, '0' Status
	, 0 PrintSeq
	, TypeOfGoods
	, @UserID CreatedBy
	, GetDate() CreatedDate
	, @UserID LastUpdateBy
	, GetDate() LastUpdateDate
	, NULL IsLocked
	, NULL LockingBy
	, NULL LockingDate
FROM SpTrnSPickingHdr WITH (NOLOCK, NOWAIT)
WHERE 
	1 = 1
	AND CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND PickingSlipNo = @PickingSlipNo

UPDATE GnMstDocument
SET DocumentSequence = DocumentSequence + 1
	, LastUpdateDate = GetDate()
	, LastUpdateBy = @UserID
WHERE
	1 = 1
	AND CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND DocumentPrefix = 'LMP'
	AND ProfitCenterCode = '300'
	AND DocumentYear = Year(GetDate())

--===============================================================================================================================
-- INSERT LAMPIRAN DETAIL
--===============================================================================================================================
INSERT INTO SpTrnSLmpDtl
SELECT
	a.CompanyCode
	, a.BranchCode
	, @TempLmpNo LmpNo
	, a.WarehouseCode
	, a.PartNo
	, a.PartNoOriginal
	, a.DocNo
	, a.DocDate
	, a.ReferenceNo
	, a.ReferenceDate
	, a.LocationCode
	, a.QtyBill
	, a.RetailPriceInclTax
	, a.RetailPrice
	, a.CostPrice
	, a.DiscPct
	, a.SalesAmt
	, a.DiscAmt
	, a.NetSalesAmt
	, 0 PPNAmt
	, a.TotSalesAmt
	, a.ProductType
	, a.PartCategory 
	, a.MovingCode
	, a.ABCClass
	, @UserID CreatedBy
	, GetDate() CreatedDate
	, @UserID LastUpdateBy
	, GetDate() LastUpdateDate
FROM SpTrnSPickingDtl a WITH (NOLOCK, NOWAIT)
WHERE 
	1 = 1
	AND a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND a.PickingSlipNo = @PickingSlipNo
	AND a.QtyPicked > 0

--===============================================================================================================================
-- UPDATE STOCK
--===============================================================================================================================

--===============================================================================================================================
-- VALIDATION QTY
--===============================================================================================================================
	DECLARE @Onhand_SRValid NUMERIC(18,2)	
	DECLARE @Allocation_SRValid NUMERIC(18,2)
	DECLARE @errmsg VARCHAR(MAX)
	DECLARE @CompanyMD AS VARCHAR(15)
	DECLARE @BranchMD AS VARCHAR(15)
	DECLARE @WarehouseMD AS VARCHAR(15)

	SET @CompanyMD = (SELECT CompanyMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)
	SET @BranchMD = (SELECT BranchMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)
	SET @WarehouseMD = (SELECT WarehouseMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)

declare @validString varchar(max)

declare @valid_2 table(
PartNo varchar(20),
QtyValidSR NUMERIC(18,2),
QtyValidOnhand NUMERIC(18,2)
)
    set @validString ='SELECT a.PartNo
		, a.AllocationSR - b.QtyBill QtyValidSR
		, a.Onhand - b.QtyBill QtyValidOnhand
	FROM '+ @DbMD +'..SpMstItems a, SpTrnSPickingDtl b
	WHERE 1 = 1
		AND a.CompanyCode = '''+ @CompanyMD +'''
		AND a.BranchCode = '''+ @BranchMD+'''
		AND b.PickingSlipNo = '''+@PickingSlipNo+'''
        AND b.ReferenceNo = '''+@JobOrderNo+'''
		AND b.CompanyCode = '''+ @CompanyCode +'''
		AND b.BranchCode = '''+ @BranchCode+'''
		AND a.PartNo = b.PartNo'
	
	--print(@validString)
	insert into @valid_2 exec(@validString)

select * from @valid_2

	SET @Allocation_SRValid = ISNULL((SELECT TOP 1 QtyValidSR FROM @valid_2 WHERE QtyValidSR < 0),0)
	SET @Onhand_SRValid = ISNULL((SELECT TOP 1 QtyValidOnhand FROM @valid_2 WHERE QtyValidOnhand < 0),0)
	select @Allocation_SRValid
	select @Onhand_SRValid

	IF (@Onhand_SRValid < 0 OR @Allocation_SRValid < 0)
	BEGIN
		SET @errmsg = 'PEMBUATAN DOKUMEN LAMPIRAN GAGAL ! ' + char(13) + char(13) + 'Error Message: Terdapat part dengan quantity Onhand atau alokasi kurang dari nol !'
		
		CLOSE db_cursor
		DEALLOCATE db_cursor 
		
		DROP TABLE #TempPickingSlipDtl
		DROP TABLE #TempPickingSlipHdr
		
		RAISERROR (@errmsg,16,1);
		
		RETURN
	END
--===============================================================================================================================

--DECLARE @DbMD AS VARCHAR(15)
--SET @DbMD = (SELECT DbMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)

DECLARE @Query AS VARCHAR(MAX)
SET @Query = 
'UPDATE '+@DbMD+'..SpMstItems
SET
	AllocationSR = AllocationSR - b.QtySupply
	, Onhand = Onhand - b.QtyBill
	, LastUpdateBy = ' + '''' + @UserID + '''' +'
	, LastUpdateDate = GetDate()
	, LastSalesDate = GetDate()
FROM ' + @DbMD + '..SpMstItems a, SpTrnSPickingDtl b
WHERE
	1 = 1
	AND a.CompanyCode = ' + ''''+@CompanyMD+'''' + '
	AND a.BranchCode = ' + ''''+@BranchMD +''''+ '
	AND b.PickingSlipNo = ' + ''''+@PickingSlipNo+'''' + '
	AND b.ReferenceNo = ' + ''''+@JobOrderNo+'''' + '
	AND b.CompanyCode = ' + ''''+@CompanyCode+'''' + '
	AND b.BranchCode = ' + ''''+@BranchCode+'''' + '
	AND a.PartNo = b.PartNo
UPDATE '+ @DbMD +'..SpMstItemLoc
SET
	AllocationSR = AllocationSR - b.QtySupply
	, Onhand = Onhand - b.QtyBill
	, LastUpdateBy = ' + '''' + @UserID + '''' +'
	, LastUpdateDate = GetDate()
FROM ' + @DbMD + '..SpMstItemLoc a, SpTrnSPickingDtl b
WHERE
	1 = 1
	AND a.CompanyCode = ' + ''''+@CompanyMD+'''' + '
	AND a.BranchCode = ' + ''''+@BranchMD +''''+ '
	AND a.WarehouseCode = ' + ''''+@WarehouseMD +''''+ '
	AND b.PickingSlipNo = ' + ''''+@PickingSlipNo+'''' + '
	AND b.CompanyCode = ' + ''''+@CompanyCode+'''' + '
	AND b.BranchCode = ' + ''''+@BranchCode+'''' + '
	AND a.PartNo = b.PartNo'

EXEC(@query)
	--print(@query)
--===============================================================================================================================
-- UPDATE DEMAND CUST AND DEMAND ITEM
--===============================================================================================================================
UPDATE SpHstDemandCust
SET SalesFreq = SalesFreq + 1
	, SalesQty = SalesQty + b.QtyBill
	, LastUpdateBy = @UserID 
	, LastUpdateDate = GetDate()
FROM SpHstDemandCust a, SpTrnSPickingDtl b
WHERE
	1 = 1
	AND a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND b.PickingSlipNo = @PickingSlipNo
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.Year = Year(b.DocDate)
	AND a.Month = Month(b.DocDate)
	AND a.CustomerCode IN (SELECT CustomerCode FROM SpTrnSPickingHdr WHERE CompanyCode = @CompanyCode AND BranchCode = BranchCode
							AND PickingSlipNo = @PickingSlipNo)
	AND a.PartNo = b.PartNo
	

UPDATE SpHstDemandItem
SET SalesFreq = SalesFreq + 1
	, SalesQty = SalesQty + b.QtyBill
	, LastUpdateBy = @UserID 
	, LastUpdateDate = GetDate()
FROM SpHstDemandItem a, SpTrnSPickingDtl b
WHERE
	1 = 1
	AND a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND b.PickingSlipNo = @PickingSlipNo
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.Year = Year(b.DocDate)
	AND a.Month = Month(b.DocDate)
	AND a.PartNo = b.PartNo
--
----=============================================================================================================================
---- INSERT TO ITEM MOVEMENT
----=============================================================================================================================
INSERT INTO SpTrnIMovement
SELECT
	@CompanyCode CompanyCode
	, @BranchCode BranchCode
	, a.LmpNo DocNo
	, (SELECT LmPDate FROM SpTrnSLmpHdr WITH (NOLOCK, NOWAIT) WHERE CompanyCode = @CompanyCode 
		AND BranchCode = @BranchCode AND LmpNo = a.LmpNo) 
	  DocDate
	, dateadd(s,ROW_NUMBER() OVER(Order by a.PartNo),getdate()) CreatedDate 
	, '00' WarehouseCode
	, LocationCode 
	, a.PartNo
	, 'OUT' SignCode
	, 'LAMPIRAN' SubSignCode
	, a.QtyBill
	, a.RetailPrice
	, a.CostPrice
	, a.ABCClass
	, a.MovingCode
	, a.ProductType
	, a.PartCategory
	, @UserID CreatedBy
FROM SpTrnSLmpDtl a WITH (NOLOCK, NOWAIT)
WHERE
	1 = 1
	AND CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND LmpNo IN (SELECT LmpNo FROM SpTrnSLmpHdr WITH (NOLOCK, NOWAIT) WHERE CompanyCode = @CompanyCode 
				AND BranchCode = @BranchCode AND PickingSlipNo = @PickingSlipNo)

--===============================================================================================================================
-- INSERT INTO svSDMovement
--===============================================================================================================================
declare @md bit
set @md = (select case WHEN EXISTS(select * from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode and CompanyMD = @CompanyCode and BranchMD = @BranchCode) then cast(1 as bit) ELSE cast(0 as bit) END)

 if(@md = 0)
 begin
	set @Query = '
	insert into ' + @DbMD + '..svSDMovement (CompanyCode, BranchCode, DocNo, DocDate, PartNo, PartSeq
	, WarehouseCode, QtyOrder, Qty, DiscPct, CostPrice, RetailPrice
	, TypeOfGoods, CompanyMD, BranchMD, WarehouseMD, RetailPriceInclTaxMD, RetailPriceMD
	, CostPriceMD, QtyFlag, ProductType, ProfitCenterCode, Status, ProcessStatus
	, ProcessDate, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)  
	select h.CompanyCode, h.BranchCode, h.LmpNo, h.LmpDate, d.PartNo, ROW_NUMBER() OVER(Order by d.LmpNo)
	,d.WarehouseCode, d.QtyBill, d.QtyBill, d.DiscPct
	--,isnull(((select RetailPrice from spTrnSLmpDtl
	--	where CompanyCode = ''' + @CompanyCode + '''  and BranchCode = ''' + @BranchCode  + '''
	--	and ProductType = ''' + @ProductType  + ''' and LmpNo = ''' + @TempLmpNo + ''' and PartNo = d.PartNo) / 1.1 * 
	--	((100 - isnull((select isnull(DiscPct, 0) from gnMstSupplierProfitCenter   
	--		where CompanyCode = ''' + @CompanyCode + ''' and BranchCode = ''' + @BranchCode  + ''' and SupplierCode = dbo.GetBranchMD(''' + @CompanyCode + ''', ''' + @BranchCode  + ''') 
	--		and ProfitCenterCode = ''300''),0)) * 0.01)),0) CostPrice
	, d.CostPrice
	, d.RetailPrice
	,h.TypeOfGoods, ''' + @CompanyMD + ''', ''' + @BranchMD + ''', ''' + @WarehouseMD + ''', md.RetailPriceInclTax, md.RetailPrice, 
	md.CostPrice,''x'', ''' + @ProductType  + ''',''300'', ''0'',''0'',''' + convert(varchar, GETDATE()) + ''',''' + @UserID + ''',''' +
	  convert(varchar, GETDATE()) + ''',''' +  @UserID + ''',''' +  convert(varchar, GETDATE()) + '''
	 from spTrnSLmpDtl d 
	 inner join spTrnSLmpHdr h on h.CompanyCode = d.CompanyCode and h.BranchCode = d.BranchCode and h.LmpNo = d.LmpNo  
	 join '+ @DbMD +'..spmstitemprice md
	on md.CompanyCode = '''+ @CompanyMD +'''
	and md.branchcode = '''+ @BranchMD +'''
	and md.PartNo = d.PartNo
	where d.CompanyCode = ''' + @CompanyCode + ''' 
	and d.BranchCode = ''' + @BranchCode +'''
	and d.LmpNo = ''' + @TempLmpNo + '''';
	
	exec(@Query);
end

FETCH NEXT FROM db_cursor INTO @PickingSlipNo
END
CLOSE db_cursor
DEALLOCATE db_cursor 

--===============================================================================================================================
-- UPDATE TRANSDATE
--===============================================================================================================================

update gnMstCoProfileSpare
set TransDate=getdate()
	, LastUpdateBy=@UserID
	, LastUpdateDate=getdate()
where CompanyCode = @CompanyCode AND BranchCode = @BranchCode


--===============================================================================================================================
-- DROP SECTION HEADER
--===============================================================================================================================
DROP TABLE #TempPickingSlipDtl
DROP TABLE #TempPickingSlipHdr
end
go

if object_id('usprpt_GnGenerateSeqTaxOnline') is not null
	drop PROCEDURE usprpt_GnGenerateSeqTaxOnline
GO
create procedure [dbo].[usprpt_GnGenerateSeqTaxOnline]
--DECLARE 
	@CompanyCode as varchar(15)
	,@BranchCode as varchar(15)
	,@StartDate as varchar(8)
	,@FPJDate as varchar(8)
	,@ProfitCenterCode as varchar(3)
	,@UserId as varchar(15)
	,@DocNo as varchar(5000)
	,@LastSeqNo as decimal
	,@TaxCabCode as varchar(3)
AS
BEGIN

declare @t_tax table(
	CompanyCode varchar(15)
	,BranchCode varchar(15)
	,ProfitCenterCode varchar(3)
	,DocNo varchar(15)
	,DocDate varchar(15)
	,DueDate datetime
	,RefNo varchar(15)
	,RefDate datetime
	,TaxTransCode varchar(2)
	,CustomerCodeBill varchar(15)
)

if @ProfitCenterCode='' or @ProfitCenterCode='300'
begin
	insert into @t_tax
	SELECT	CompanyCode, BranchCode, '300' AS ProfitCenterCode, InvoiceNo AS DocNo, convert(varchar,InvoiceDate,112) AS DocDate, convert(varchar,DueDate,112) DueDate
			, '' AS RefNo, NULL AS RefDate
			,(
				SELECT	ISNULL(TaxTransCode, '') 
				FROM	GnMstCustomerProfitCenter 
				WHERE	CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode 
						AND CustomerCode = a.CustomerCodeBill AND ProfitCenterCode = '300'
			) AS TaxTransCode
			, CustomerCodeBill
	FROM	SpTrnSFPJHdr a
	WHERE	CompanyCode = @CompanyCode  
			--AND BranchCode like @BranchCode
			AND isPKP = 1 AND ISNULL(FPJGovNo, '') = ''
			AND CONVERT(VARCHAR, InvoiceDate, 112) BETWEEN @StartDate AND @FPJDate
			AND ((case when @ProfitCenterCode='' then BranchCode+' '+InvoiceNo end) = BranchCode+' '+InvoiceNo
				or (case when @ProfitCenterCode<>'' then @DocNo end) like '%|'+BranchCode+' '+InvoiceNo+'|%')
	GROUP BY CompanyCode, BranchCode,InvoiceNo, CustomerCodeBill,convert(varchar,InvoiceDate,112),convert(varchar,DueDate,112)
end

if @ProfitCenterCode='' or @ProfitCenterCode='200'
begin
	insert into @t_tax
	SELECT	CompanyCode, BranchCode, '200' AS ProfitCenterCode, FPJNo AS DocNo, convert(varchar,FPJDate,112) AS DocDate, DueDate
			, '' AS RefNo, NULL AS RefDate
			,(
				SELECT	ISNULL(TaxTransCode, '') 
				FROM	GnMstCustomerProfitCenter 
				WHERE	CompanyCode = SVTrnFakturPajak.CompanyCode AND BranchCode = SVTrnFakturPajak.BranchCode 
						AND CustomerCode = SVTrnFakturPajak.CustomerCodeBill AND ProfitCenterCode = '200'
			) AS TaxTransCode
			, CustomerCodeBill
	FROM	SVTrnFakturPajak
	WHERE	CompanyCode = @CompanyCode 
			--AND BranchCode like @BranchCode
			AND isPKP = 1 AND ISNULL(FPJGovNo, '') = ''
			AND IsLocked= 0
			AND CONVERT(VARCHAR, FPJDate, 112) BETWEEN @StartDate AND @FPJDate
			AND ((case when @ProfitCenterCode='' then BranchCode+' '+FPJNo end)=BranchCode+' '+FPJNo
				or (case when @ProfitCenterCode<>'' then @DocNo end) like '%|' + BranchCode+' '+FPJNo + '|%'
			)
end

if @ProfitCenterCode='' or @ProfitCenterCode='100'
begin
	insert into @t_tax
	SELECT	CompanyCode, BranchCode, '100' AS ProfitCenterCode, InvoiceNo AS DocNo, convert(varchar,InvoiceDate,112) AS DocDate, DueDate
			, '' AS RefNo, NULL AS RefDate
			,(
				SELECT	ISNULL(TaxTransCode, '') 
				FROM	GnMstCustomerProfitCenter 
				WHERE	CompanyCode = OmFakturPajakHdr.CompanyCode AND BranchCode = OmFakturPajakHdr.BranchCode 
						AND CustomerCode = OmFakturPajakHdr.CustomerCode AND ProfitCenterCode = '100') AS TaxTransCode
			, CustomerCode
	FROM	OmFakturPajakHdr
	WHERE	CompanyCode = @CompanyCode 
			--AND BranchCode like @BranchCode
			AND TaxType = 'Standard' AND ISNULL(FakturPajakNo, '') = ''
			AND CONVERT(VARCHAR, InvoiceDate, 112) BETWEEN @StartDate AND @FPJDate
			AND ((case when @ProfitCenterCode='' then BranchCode+' '+InvoiceNo end)=BranchCode+' '+InvoiceNo
				or (case when @ProfitCenterCode<>'' then @DocNo end) like '%|' + BranchCode+' '+InvoiceNo + '|%'
			)
end

if @ProfitCenterCode='' or @ProfitCenterCode='000'
begin
	insert into @t_tax
	SELECT	CompanyCode, BranchCode, '000' AS ProfitCenterCode, InvoiceNo AS DocNo, convert(varchar,InvoiceDate,112) AS DocDate, DueDate
			, FPJNo AS RefNo, FPJDate AS RefDate
			,(
				SELECT	ISNULL(TaxTransCode, '') 
				FROM	GnMstCustomerProfitCenter 
				WHERE	CompanyCode = ARFakturPajakHdr.CompanyCode AND BranchCode = ARFakturPajakHdr.BranchCode 
						AND CustomerCode = ARFakturPajakHdr.CustomerCode AND ProfitCenterCode = '000'
			) AS TaxTransCode
			, CustomerCode
	FROM	ARFakturPajakHdr
	WHERE	CompanyCode = @CompanyCode 
			--AND BranchCode like @BranchCode
			AND TaxType = 'Standard' AND ISNULL(FakturPajakNo, '') = ''
			AND CONVERT(VARCHAR, InvoiceDate, 112) BETWEEN @StartDate AND @FPJDate
			AND ((case when @ProfitCenterCode='' then BranchCode+' '+InvoiceNo end)=BranchCode+' '+InvoiceNo
				or (case when @ProfitCenterCode<>'' then @DocNo end) like '%|' + BranchCode+' '+InvoiceNo + '|%'
			)
end

select * into #f1
from (
	select row_number() over(order by a.BranchCode,a.DocDate,b.LookupValue,a.CustomerCodeBill,a.ProfitCenterCode asc) OrderNo,a.*,isnull(b.LookupValue,'')LookupValue
	from @t_tax a
	left join gnMstLookupDtl b on b.CompanyCode = a.CompanyCode
		and b.CodeID = 'FPJG'
		and b.LookupValue = a.CustomerCodeBill	
) #f1  order by LookupValue desc

if(Convert(varchar,@FPJDate,112) < '20130401')
begin
	-- create FPJGovNo
	select * into #f3 
	from (
		select 
			a.OrderNo,a.CompanyCode,a.BranchCode,year(DocDate) PeriodTaxYear
			,month(DocDate) PeriodTaxMonth,ProfitCenterCode
			,left(TaxTransCode+'000',3)+'.'+
			right('000'+@TaxCabCode,3)+'-'+
			right( isnull(convert(varchar(4),year(DocDate)),year(getDate())),2 )+'.'+ 
			right( '00000000'+convert(varchar(8),@LastSeqNo+a.OrderNo),8 ) FPJGovNo
			,DocDate as FPJGovDate,DocNo,convert(datetime,DocDate) DocDate,RefNo,RefDate,@UserId CreatedBy,getDate() CreatedDate
		from 
			#f1 a
	) #f3


	-- insert to tabel GenerateTax
	insert into
		GnGenerateTax(
			CompanyCode, BranchCode, PeriodTaxYear, PeriodTaxMonth, ProfitCenterCode, FPJGovNo, 
			FPJGovDate, DocNo, DocDate , RefNo, RefDate, CreatedBy, CreatedDate
	) 
	select 
		CompanyCode, BranchCode, PeriodTaxYear, PeriodTaxMonth, ProfitCenterCode, FPJGovNo, 
		FPJGovDate, DocNo, DocDate , RefNo, RefDate, CreatedBy, CreatedDate
	from 
		#f3

	drop table #f1

	-- update Sparepart
	update	SPTrnSFPJHdr
	set		FPJGovNo= a.FPJGovNo
			,FPJSignature= a.FPJGovDate
	from	#f3 a, SPTrnSFPJHdr b
	where	a.CompanyCode= b.CompanyCode and a.BranchCode= b.BranchCode and a.DocNo= b.InvoiceNo

	-- update Service
	update	SVTrnFakturPajak
	set		FPJGovNo= a.FPJGovNo
			,SignedDate= a.FPJGovDate
	from	#f3 a, SVTrnFakturPajak b
	where	a.CompanyCode= b.CompanyCode and a.BranchCode= b.BranchCode and a.DocNo= b.FPJNo

	-- update Sales
	update	OmFakturPajakHdr
	set		FakturPajakNo= a.FPJGovNo
			,FakturPajakDate= a.FPJGovDate 
	from	#f3 a, OmFakturPajakHdr b
	where	a.CompanyCode= b.CompanyCode and a.BranchCode= b.BranchCode and a.DocNo= b.InvoiceNo

	-- update Finance
	update	ArFakturPajakHdr
	set		FakturPajakNo= a.FPJGovNo
			,FakturPajakDate= a.FPJGovDate
	from	#f3 a, ArFakturPajakHdr b
	where	a.CompanyCode= b.CompanyCode and a.BranchCode= b.BranchCode and a.DocNo= b.InvoiceNo

	select top 1 convert(decimal,right(FPJGovNo,8)) from #f3 order by right(FPJGovNo,8) desc
	drop table #f3
end
else
begin
---------------------------------------------------------------------------------------------------
 --									Region Setelah Tanggal 1 April 2013                           --
 ---------------------------------------------------------------------------------------------------
	Declare @TotalFPJ				varchar(25)
	Declare @TotalFPJX				varchar(25)
	Declare @EndFPJ					varchar(25)
	Declare @CurrentFPJ				varchar(25)
	Declare @CurrentDocNo			varchar(100)
	Declare @CurrentCompanyCode		varchar(15)
	Declare @CurrentBranchCode		varchar(15)
	Declare @CurrentTaxTransCode	varchar(2)
	Declare @OrderNo				varchar(10)
	Declare @CurrentCustCodeBill	varchar(15)
	
	declare @IntOrderNo as int
	set @IntOrderNo = 0
	
	DECLARE temp CURSOR FOR
	SELECT	CompanyCode, BranchCode, DocNo, TaxTransCode, OrderNo, CustomerCodeBill
	FROM	#f1

	OPEN temp
	FETCH NEXT FROM temp INTO @CurrentCompanyCode,@CurrentBranchCode,@CurrentDocNo,@CurrentTaxTransCode,@OrderNo,@CurrentCustCodeBill
	WHILE @@FETCH_STATUS = 0
		BEGIN
		--!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		--									Penambahan no Faktur Pajak
		--!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
									--set @TotalFPJ =   @LastSeqNo + @OrderNo
		--!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
			
			if @CurrentCustCodeBill = (select top 1 LookupValue from gnMstLookupDtl where CompanyCode = @CurrentCompanyCode and CodeID = 'FPJG' and LookupValue = @CurrentCustCodeBill)
			begin
				print @CurrentFPJ + ' for ' + @CurrentCustCodeBill
				set @TotalFPJ =   @LastSeqNo + convert(varchar, @IntOrderNo,10)
			end
			else
			begin
				set @IntOrderNo = @IntOrderNo + 1
				set @TotalFPJ =   @LastSeqNo + convert(varchar, @IntOrderNo,10)
			end									   
            --update by Hasim (6 Jan 2014), because could be problem if length of tax no less than 11...
            --original...
			--set @CurrentFPJ = (select LEFT (convert(varchar,@CurrentTaxTransCode) + '000',3)+'.'+
			--							LEFT (convert(varchar,@TotalFPJ),3) + '-' +
			--							RIGHT(convert(varchar,YEAR(getdate())),2) + '.' +
			--							RIGHT(convert(bigint,@TotalFPJ),8))
            --revised...
            set @TotalFPJX  = RIGHT('00000000000'+convert(varchar(11),@TotalFPJ),11)
			set @CurrentFPJ = (select LEFT (convert(varchar,@CurrentTaxTransCode) + '000',3)+'.'+
									  LEFT (convert(varchar,@TotalFPJX),3) + '-' +
									  RIGHT(convert(varchar,YEAR(getdate())),2) + '.' +
									  RIGHT(convert(varchar,@TotalFPJX),8))

			--insert to tabel GenerateTax
			insert into
				GnGenerateTax(
					CompanyCode, BranchCode, PeriodTaxYear, PeriodTaxMonth, ProfitCenterCode, FPJGovNo, 
					FPJGovDate, DocNo, DocDate , RefNo, RefDate, CreatedBy, CreatedDate
			) 
			select 
				CompanyCode, BranchCode,year(DocDate) PeriodTaxYear, month(DocDate) PeriodTaxMonth, ProfitCenterCode
				, @CurrentFPJ FPJGovNo
				, DocDate as FPJGovDate, DocNo, DocDate , RefNo, RefDate, @UserId CreatedBy,getDate() CreatedDate
			from 
				#f1
			where CompanyCode = @CurrentCompanyCode
			  and BranchCode = @CurrentBranchCode
			  and DocNo = @CurrentDocNo
			  and TaxTransCode = @CurrentTaxTransCode

			 --update Sparepart
			update	SPTrnSFPJHdr
			set		FPJGovNo= @CurrentFPJ
					,FPJSignature= a.DocDate
			from	#f1 a, SPTrnSFPJHdr b
			where	a.CompanyCode= b.CompanyCode and a.BranchCode= b.BranchCode and a.DocNo= b.InvoiceNo 
				and b.CompanyCode = @CurrentCompanyCode and b.BranchCode = @CurrentBranchCode and b.InvoiceNo = @CurrentDocNo

			-- update Service
			update	SVTrnFakturPajak
			set		FPJGovNo= @CurrentFPJ
					,SignedDate= a.DocDate
			from	#f1 a, SVTrnFakturPajak b
			where	a.CompanyCode = b.CompanyCode and a.BranchCode = b.BranchCode and a.DocNo= b.FPJNo
				and b.CompanyCode = @CurrentCompanyCode  and b.BranchCode = @CurrentBranchCode  and b.FPJNo = @CurrentDocNo

			-- update Sales
			update	OmFakturPajakHdr
			set		FakturPajakNo= @CurrentFPJ
					,FakturPajakDate= a.DocDate 
			from	#f1 a, OmFakturPajakHdr b
			where	a.CompanyCode = b.CompanyCode and a.BranchCode = b.BranchCode and a.DocNo= b.InvoiceNo
				and b.CompanyCode = @CurrentCompanyCode  and b.BranchCode = @CurrentBranchCode  and b.InvoiceNo = @CurrentDocNo

			-- update Finance
			update	ArFakturPajakHdr
			set		FakturPajakNo= @CurrentFPJ
					,FakturPajakDate= a.DocDate
			from	#f1 a, ArFakturPajakHdr b
			where	a.CompanyCode = b.CompanyCode and a.BranchCode = b.BranchCode and a.DocNo= b.InvoiceNo
				and b.CompanyCode = @CurrentCompanyCode  and b.BranchCode = @CurrentBranchCode  and b.InvoiceNo = @CurrentDocNo
				
			FETCH NEXT FROM temp INTO @CurrentCompanyCode,@CurrentBranchCode,@CurrentDocNo,@CurrentTaxTransCode,@OrderNo,@CurrentCustCodeBill
		END
	CLOSE temp
	DEALLOCATE temp
	
	drop table #f1

	select @TotalFPJ FPJGovNo
end

-- update TransDate Sparepart
update GnMstCoProfileSpare 
set TransDate = convert(datetime, @FPJDate)
WHERE CompanyCode = @CompanyCode  
	--AND BranchCode like @BranchCode
	and convert(datetime, TransDate, 112) < @FPJDate

-- update TransDate Service
update GnMstCoProfileService 
set TransDate = convert(datetime, @FPJDate) 
WHERE CompanyCode = @CompanyCode 
	--AND BranchCode like @BranchCode
	and convert(datetime, TransDate, 112) < @FPJDate 

-- update TransDate Sales
update GnMstCoProfileSales 
set TransDate = convert(datetime, @FPJDate) 
WHERE CompanyCode = @CompanyCode 
	--AND BranchCode like @BranchCode
	and convert(datetime, TransDate, 112) < @FPJDate 

-- update TransDate Finance
update GnMstCoProfileFinance 
set TransDateAR = convert(datetime, @FPJDate) 
WHERE CompanyCode = @CompanyCode 
	--AND BranchCode like @BranchCode
	and convert(datetime, TransDateAR, 112) < @FPJDate


END

GO
if object_id('usprpt_OmRpLabaRugi002') is not null
	drop PROCEDURE usprpt_OmRpLabaRugi002
GO
create procedure [dbo].[usprpt_OmRpLabaRugi002] 
--DECLARE    
	@CompanyCode varchar ( 20 ) , 
    @BranchCode varchar ( 20 ) , 
    @DateStart datetime , 
    @DateEnd datetime , 
    @SalesType char ( 1 ) , 
    @SalesFrom varchar ( 20 ) , 
    @SalesTo varchar ( 20 ) , 
    @param char ( 1 ) 

--SELECT @CompanyCode='6115204001',@BranchCode='%',@DateStart='20150501',@DateEnd='20150531',@SalesType='',@SalesFrom='',@SalesTo= '',@param ='1'


AS
BEGIN

select * into #t1
from (
	SELECT 
		a.CompanyCode , 
		a.BranchCode
		,( 
			SELECT CompanyName FROM gnMstCoProfile 
			WHERE CompanyCode = a.CompanyCode 
			AND BranchCode = a.BranchCode 
		) BranchName,
		d.SalesType , 
		g.SalesModelCode + ' - ' + g.SalesModelDesc AS SalesModelCode , 
		count(c.ChassisNo) AS Unit,
		isnull (sum(b.BeforeDiscDPP) , 0 ) AS PenjualanBrutto , 
		isnull (sum(b.AfterDiscDPP) , 0 ) AS penjualan , 
		--isnull (isnull (sum(b.BeforeDiscDPP) , 0 )- isnull (sum(b.DiscExcludePPN ), 0 ), 0 ) AS penjualan , 
		isnull (sum(b.DiscExcludePPN ), 0 ) AS Discount , 
		isnull (sum(c.COGS) , 0 ) AS Biaya ,
		isnull ( (sum(b.AfterDiscDPP) - sum(c.COGS) ) , 0 ) AS LabaRugi 
		,((isnull(sum(b.AfterDiscDPP),0)-isnull(sum(c.COGS),0)) / isnull(sum(b.AfterDiscDPP),0 ))* 100 Percentage
		,'Invoice' TypeTrans
	FROM OmTrSalesInvoice a 
	INNER JOIN OmTrSalesInvoiceModel b ON a.CompanyCode = b.CompanyCode 
		AND a.BranchCode = b.BranchCode 
		AND a.InvoiceNo = b.InvoiceNo 
	INNER JOIN OmTrSalesInvoiceVin c ON c.CompanyCode = a.CompanyCode 
		AND c.BranchCode = a.BranchCode 
		AND c.InvoiceNo = a.InvoiceNo 
		AND c.BPKNo=b.BPKNo
		AND c.SalesModelCode = b.SalesModelCode 
		AND c.SalesModelYear = b.SalesModelYear 
	INNER JOIN OmTrSalesSO d ON d.CompanyCode = a.CompanyCode 
		AND d.BranchCode = a.BranchCode 
		AND d.SONo = a.SONo 
	LEFT JOIN omMstModel g ON g.CompanyCode = a.CompanyCode 
		AND g.SalesModelCode = b.SalesModelCode 
	WHERE a.CompanyCode = @CompanyCode 
		AND a.BranchCode LIKE @BranchCode 
		AND ((case when @param = '0' then a.InvoiceDate end) <> ''
			 or (case when @param = '1' then convert ( varchar , a.InvoiceDate , 112 ) end) 
				BETWEEN convert ( varchar , @DateStart , 112 ) AND convert ( varchar , @DateEnd , 112 ) 
		)
		AND 
		( ( CASE WHEN @SalesType = '' THEN d.SalesType END ) <> '' 
			OR ( CASE WHEN @SalesType <> '' THEN d.SalesType END ) = @SalesType 
		) 
		AND 
		( ( CASE WHEN @SalesFrom = '' THEN d.SalesCode END ) <> '' 
			OR ( CASE WHEN @SalesFrom <> '' THEN d.SalesCode END ) BETWEEN @SalesFrom AND @SalesTo 
		) 
	group by
		a.CompanyCode
		,a.BranchCode
		,d.SalesType
		,g.SalesModelCode + ' - ' + g.SalesModelDesc
) #t1

insert into #t1
SELECT 
		a.CompanyCode , 
		a.BranchCode
		,( 
			SELECT CompanyName FROM gnMstCoProfile 
			WHERE CompanyCode = a.CompanyCode 
			AND BranchCode = a.BranchCode 
		) BranchName,
		d.SalesType , 
		g.SalesModelCode + ' - ' + g.SalesModelDesc AS SalesModelCode , 
		count(c.ChassisNo) * -1 AS Unit,
		isnull (sum(b.BeforeDiscDPP) , 0 ) * -1 AS PenjualanBrutto , 
		--isnull (sum(b.AfterDiscDPP) , 0 ) * -1 AS penjualan ,
		isnull (isnull (sum(b.BeforeDiscDPP) , 0 )- isnull (sum(b.DiscExcludePPN ), 0 ), 0 ) * -1 AS penjualan ,  
		isnull (sum(b.DiscExcludePPN ), 0 ) * -1 AS Discount , 
		isnull (sum(i.COGS) , 0 ) * -1 AS Biaya ,
		isnull ( (sum(b.AfterDiscDPP) - sum(i.COGS) ) , 0 ) * -1 AS LabaRugi 
		,((isnull(sum(b.AfterDiscDPP),0)-isnull(sum(i.COGS),0)) / isnull(sum(b.AfterDiscDPP),0 ))* 100 * -1 Percentage
		,'Retur' TypeTrans
	FROM OmTrSalesReturn a 
	INNER JOIN OmTrSalesReturnDetailModel b ON a.CompanyCode = b.CompanyCode 
		AND a.BranchCode = b.BranchCode 
		AND a.ReturnNo = b.ReturnNo 
	INNER JOIN OmTrSalesReturnVin c ON c.CompanyCode = a.CompanyCode 
		AND c.BranchCode = a.BranchCode 
		AND c.ReturnNo = a.ReturnNo 
		AND c.BPKNo=b.BPKNo
		AND c.SalesModelCode = b.SalesModelCode 
		AND c.SalesModelYear = b.SalesModelYear 
	left join omTrSalesInvoice h on a.CompanyCode=h.CompanyCode and a.BranchCode=h.BranchCode and a.InvoiceNo=h.InvoiceNo
	left join omTrSalesInvoiceVin i on h.CompanyCode=i.CompanyCode and h.BranchCode=i.BranchCode and h.InvoiceNo=i.InvoiceNo
		and c.ChassisCode=i.ChassisCode and c.ChassisNo=i.ChassisNo
	INNER JOIN OmTrSalesSO d ON d.CompanyCode = a.CompanyCode 
		AND d.BranchCode = a.BranchCode 
		AND d.SONo = h.SONo 
	LEFT JOIN omMstModel g ON g.CompanyCode = a.CompanyCode 
		AND g.SalesModelCode = b.SalesModelCode 
	WHERE a.CompanyCode = @CompanyCode 
		AND a.BranchCode LIKE @BranchCode 
		AND ((case when @param = '0' then a.ReturnDate end) <> ''
			 or (case when @param = '1' then convert ( varchar , a.ReturnDate , 112 ) end) 
				BETWEEN convert ( varchar , @DateStart , 112 ) AND convert ( varchar , @DateEnd , 112 ) 
		)
		AND 
		( ( CASE WHEN @SalesType = '' THEN d.SalesType END ) <> '' 
			OR ( CASE WHEN @SalesType <> '' THEN d.SalesType END ) = @SalesType 
		) 
		AND 
		( ( CASE WHEN @SalesFrom = '' THEN d.SalesCode END ) <> '' 
			OR ( CASE WHEN @SalesFrom <> '' THEN d.SalesCode END ) BETWEEN @SalesFrom AND @SalesTo 
		) 
	group by
		a.CompanyCode
		,a.BranchCode
		,d.SalesType
		,g.SalesModelCode + ' - ' + g.SalesModelDesc

select * from #t1
drop table #t1

END
GO

if object_id('uspfn_CheckChassisNoMatch') is not null
	drop PROCEDURE uspfn_CheckChassisNoMatch
GO
create procedure [dbo].[uspfn_CheckChassisNoMatch]
	@CompanyCode varchar(25),
	@BranchCode varchar(25),
	@SONumber varchar(50)
as

begin
	declare @CountSOVin int;
	declare @CountSOModel int;
	declare @QtySO int;

	set @CountSOVin = (
		select count(a.SONo)
		  from omTrSalesSOVin a
		 where a.CompanyCode = @CompanyCode
		   and a.BranchCode = @BranchCode
		   and a.SONo = @SONumber
		   and a.ChassisNo != 0
	);

	set @CountSOModel = (
		select count(a.SONo)
		  from omTrSalesSOModel a
		 where a.CompanyCode = @CompanyCode
		   and a.BranchCode = @BranchCode
		   and a.SONo = @SONumber
	);

	set @QtySO = (select SUM(QuantitySO) Qty from omTrSalesSOModel a
		where a.CompanyCode = @CompanyCode
		   and a.BranchCode = @BranchCode
		   and a.SONo = @SONumber 
	 );


	if @CountSOModel = 0
		select convert(bit, 0) as Status;
	if @QtySO = @CountSOVin
		select convert(bit, 1) as Status;
	else
		select convert(bit, 0) as Status
end

go

if object_id('uspfn_omSoLkp') is not null
	drop PROCEDURE uspfn_omSoLkp
GO
create procedure [dbo].[uspfn_omSoLkp] 
(
	@CompanyCode varchar(25),
	@BranchCode varchar(25)
)
as
 
 -- exec uspfn_omSoLkp '6115204001','6115204105'
 
 declare @DbMD as varchar(15)  
 declare @Sql as varchar(max) 
 declare @ssql as varchar(max) 
 
 set @DbMD = (select TOP 1 DbMD from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)  
 


 set @ssql='select * from gnMstCompanyMapping '

set @Sql= 'SELECT a.CompanyCode, a.BranchCode,
                a.SONo, a.SODate, a.SalesType, a.CustomerCode, a.TOPCode, a.Installment, a.FinalPaymentDate,
                a.TOPDays, a.BillTo, a.ShipTo, a.ProspectNo, a.SKPKNo, a.Salesman, a.WareHouseCode, a.isLeasing, 
                a.LeasingCo, a.GroupPriceCode, a.Insurance, a.PaymentType, a.PrePaymentAmt, a.PrePaymentBy, 
                a.CommissionBy, a.CommissionAmt, a.PONo, a.ContractNo, a.Remark, a.Status,
                a.SalesCoordinator, a.SalesHead, a.BranchManager, a.RefferenceNo,
                CASE convert(varchar, a.RefferenceDate, 111) when convert(varchar, ''1900/01/01'') 
                then '''' else convert(varchar, a.RefferenceDate, 111) end as RefferenceDates, 
                CASE convert(varchar, a.RefferenceDate, 111) when convert(varchar, ''1900/01/01'') 
                then ''undefined'' else convert(varchar, a.RefferenceDate, 111) end as RefferenceDate, 
                CASE convert(varchar, a.RequestDate, 111) when convert(varchar, ''1900/01/01'') 
                then ''undefined'' else convert(varchar, a.RequestDate, 111) end as RequestDate,
                CASE convert(varchar, a.PrePaymentDate, 111) when convert(varchar, ''1900/01/01'') 
                then ''undefined'' else convert(varchar, a.PrePaymentDate, 111) end as PrePaymentDate,
                e.Address1 + '' '' + e.Address2 + '' '' + e.Address3 + '' '' + e.Address4 as Address,
                case when year(a.RefferenceDate) <> 1900 then convert(bit,1) else convert(bit,0) end isC1,
                case when a.SKPKNo <> '''' then convert(bit,1) else convert(bit,0) end isC2,
                case when year(a.PrePaymentDate) <> 1900 then convert(bit,1) else convert(bit,0) end isC3,
                case when year(a.RequestDate) <> 1900 then convert(bit,1) else convert(bit,0) end isC4,
				(SELECT b.CustomerName
                        FROM gnMstCustomer b
                        WHERE a.CompanyCode = b.CompanyCode
						AND a.CustomerCode = b.CustomerCode)  
						AS CustomerName,
				(SELECT c.EmployeeName
                        FROM gnMstEmployee c
                        WHERE a.CompanyCode = c.CompanyCode AND a.BranchCode = c.BranchCode
					    AND a.Salesman = c.EmployeeID) as SalesmanName,
				(SELECT b.CustomerName
                        FROM gnMstCustomer b
                        WHERE a.CompanyCode = b.CompanyCode
						AND a.Shipto = b.CustomerCode)  
						AS ShipName,
                (SELECT b.CustomerName
                        FROM gnMstCustomer b
                        WHERE a.CompanyCode = b.CompanyCode
						AND a.LeasingCo = b.CustomerCode)  
						AS LeasingCoName,
				(SELECT c.EmployeeName
                        FROM gnMstEmployee c
                        WHERE a.CompanyCode = c.CompanyCode AND a.BranchCode = c.BranchCode
					    AND a.PrePaymentby = c.EmployeeID) as PrePaymentName,
				(SELECT d.RefferenceDesc1
                        FROM omMstRefference d
                        WHERE a.CompanyCode = d.CompanyCode
						AND d.RefferenceType = ''GRPR'' 
                        AND d.RefferenceCode = a.GroupPriceCode) AS GroupPriceName,
				(SELECT b.CustomerName
                        FROM gnMstCustomer b
                        WHERE a.CompanyCode = b.CompanyCode
						AND a.CustomerCode = b.CustomerCode)  
						AS BillName,
				(SELECT TOP 1 b.lookupvaluename
                        FROM '+@DbMD+'..gnMstLookUpDtl b
                        WHERE a.WareHouseCode = b.LookUpValue
						AND a.WareHouseCode = b.LookUpValue and CodeID =''MPWH'')  
						AS WareHouseName,
                (a.CustomerCode
                    + '' || ''
                    + (SELECT b.CustomerName
                        FROM gnMstCustomer b
                        WHERE a.CompanyCode = b.CompanyCode
						AND a.CustomerCode = b.CustomerCode))  
						AS Customer, 
                (a.Salesman
                    + '' || ''
                    + (SELECT c.EmployeeName
                        FROM gnMstEmployee c
                        WHERE a.CompanyCode = c.CompanyCode AND a.BranchCode = c.BranchCode
					    AND a.Salesman = c.EmployeeID))  AS Sales, 
                (a.GroupPriceCode
                    + '' || ''
                    + (SELECT d.RefferenceDesc1
                        FROM omMstRefference d
                        WHERE a.CompanyCode = d.CompanyCode
						AND d.RefferenceType = ''GRPR'' 
                        AND d.RefferenceCode = a.GroupPriceCode))  AS GroupPrice, 
                CASE a.Status when 0 then ''OPEN''
                                when 1 then ''PRINTED''
                                when 2 then ''APPROVED''
                                when 3 then ''DELETED''
                                when 4 then ''REJECTED''
                                when 9 then ''FINISHED'' END as Stat
                , CASE ISNULL(a.SalesType, 0) WHEN 0 THEN ''Wholesales'' ELSE ''Direct'' END AS TypeSales
                ,(select distinct x.TipeKendaraan
                    from pmKDP x
	                    left join gnMstEmployee b on x.CompanyCode=b.CompanyCode and x.BranchCode=b.BranchCode
		                    and x.EmployeeID=b.EmployeeID
	                    left join omTrSalesSO c on c.CompanyCode = x.CompanyCode 
		                    and c.BranchCode = x.BranchCode
		                    and c.ProspectNo = x.InquiryNumber
	                    left join omTrSalesInvoice d on d.CompanyCode = x.CompanyCode
		                    and d.BranchCode = x.BranchCode
		                    and d.SONo = c.SONo
	                    left join omTrSalesReturn e on e.CompanyCode = x.CompanyCode
		                    and e.BranchCode = x.BranchCode
		                    and e.InvoiceNo = d.InvoiceNo
                    where x.InquiryNumber=a.ProspectNo AND x.CompanyCode = a.CompanyCode AND x.BranchCode = a.BranchCode) as VehicleType
                FROM omTrSalesSO a
                INNER JOIN gnMstCustomer e
                ON a.CompanyCode = e.CompanyCode AND a.CustomerCode = e.CustomerCode
				Where a.CompanyCode = '+ @CompanyCode+' and a.BranchCode = '+ @BranchCode +'
				order by a.SONo desc
				'
--print @Sql

exec (@Sql)

go

if object_id('uspfn_spGetFPJLookUp') is not null
	drop PROCEDURE uspfn_spGetFPJLookUp
GO
create procedure [dbo].[uspfn_spGetFPJLookUp]        
@CompanyCode varchar(15),        
@BranchCode  varchar(15),        
@TypeOfGoods varchar(4),        
@IsPKPOnly  varchar(2)        
as        
SELECT         
     a. FPJNo        
    , a.FPJDate        
    , a.PickingSlipNo        
    , a.PickingSlipDate        
    , a.InvoiceNo        
    , a.InvoiceDate        
    , (SELECT CustomerName FROM gnMstCustomer WHERE CompanyCode = a.CompanyCode AND CustomerCode = a.CustomerCode) CustomerName        
    , a.CustomerCode       
    , a.TOPCode      
    , a.TOPDays      
    , a.TotSalesQty      
    , a.TotSalesAmt      
    , a.TotDiscAmt      
    , a.TotDPPAmt      
    , a.TotPPNAmt      
    , a.TotFinalSalesAmt      
    , a.TransType      
    , a.CustomerCodeBill      
    , a.CustomerCodeShip  
    , a.Status  
    , a.FPJGovNo  
    , a.FPJSignature      
    , c.CustomerCode CustomerCodeTagih      
    , b.CustomerName CustomerNameTagih    
    , b.Address1 Address1Tagih    
    , b.Address2 Address2Tagih    
    , b.Address3 Address3Tagih      
    , b.Address4 Address4Tagih     
    , c.CustomerName       
    , c.Address1       
    , c.Address2       
    , c.Address3     
    , c.Address4     
FROM             
    spTrnSFPJHdr a        
    join SpTrnSFPJInfo b      
    on a.CompanyCode = b.CompanyCode      
    and a.BranchCode = b.BranchCode      
    and a.FPJNo = b.FPJNo      
    join gnMstCustomer c      
    on a.CompanyCode = b.CompanyCode      
    and a.CustomerCode = c.CustomerCode      
WHERE             
    a.CompanyCode = @CompanyCode        
    AND a.BranchCode = @BranchCode        
 AND ((CASE WHEN @IsPKPOnly = 1 THEN a.IsPKP END) = 1 OR (CASE WHEN @IsPKPOnly = 0 THEN a.IsPKP END) = a.IsPKP)        
 --AND ((CASE WHEN @TypeOfGoods = '%' THEN a.TypeOfGoods END) = a.TypeOfGoods OR (CASE WHEN @TypeOfGoods <> '%' THEN a.TypeOfGoods END) = '0')        
 ORDER BY a.FPJNo DESC

 go

 if object_id('usprpt_SpRpTrn011') is not null
	drop PROCEDURE usprpt_SpRpTrn011
GO
create procedure [dbo].[usprpt_SpRpTrn011]
   @CompanyCode  VARCHAR (15),
   @BranchCode   VARCHAR (15),
   @FPJNoStart   VARCHAR (15),
   @FPJNoEnd     VARCHAR (15),
   @ProfitCenter VARCHAR (15),
   @CounterDiv   DECIMAL,
   @TypeOfGoods  VARCHAR (2) 
AS

declare @IsBranch as bit
set @IsBranch=(select isBranch from gnMstOrganizationDtl where CompanyCode=@CompanyCode and BranchCode=@BranchCode)

BEGIN
SELECT 
	c.FPJNo,
	c.FPJDate,
	c.InvoiceNo,
	c.PickingSlipNo, 
	c.FpjGovNo as fakturFPJGovNo,
	CASE WHEN
		ISNULL((SELECT COUNT(DISTINCT ReferenceNo) FROM spTrnSFpjDtl WHERE 
			CompanyCode = @CompanyCode 
			AND BranchCode = @BranchCode
			AND FPJNo = c.FpjNo),0) = 1 
		THEN ISNULL((SELECT DISTINCT ReferenceNo FROM spTrnSFpjDtl WHERE 
			CompanyCode = @CompanyCode 
			AND BranchCode = @BranchCode
			AND FPJNo = c.FpjNo),'')
		ELSE ''
	END OrderNo,
	c.CustomerCode, 
	c.DueDate,
	c.TotDiscAmt,
	c.TotDppAmt, 
	c.TotPPNAmt,
	c.TotFinalSalesAmt,
	x.CustomerName + ' / '+ d.CustomerName + ' [' + c.CustomerCode + ']'as CustomerName, 
	x.Address1, 
	x.Address2, 
	x.Address3, 
	x.Address4, 
	d.PhoneNo, 
	d.FaxNo,
	CASE WHEN c.ISPKP = '1' THEN d.NPWPNo ELSE '-' END NPWPNo, 
	a.DocNo,  
	a.PartNo, 
	a.PartNoOriginal,
	a.QtyBill,
	a.RetailPrice,
	a.DiscPct,
	a.DiscAmt, 
	a.RetailPrice - ((a.RetailPrice * a.DiscPct) / 100) NetRetailPrice,
	a.NetSalesAmt, 
	b.PartName,
	e.LookUpValueName  TOPC, 
	h.LookUpValueName  CITY,
	i.TaxPct, 
	RIGHT(replicate('0', 2) + convert(varchar(2), c.PrintSeq), 2) PrintSeq,
	c.PrintSeq,
	c.TypeOfGoods,
	case 
		when @IsBranch=0 then f.CompanyGovName
		else f.CompanyGovName+' ('+ substring(a.BranchCode,len(a.branchCode)-1,2)+')' 
	end CompanyName,
	f.Address1 AS AddressCo1,
	f.Address2 AS AddressCo2,
	f.Address3 AS AddressCo3,
	k.Remark,
	UPPER (j.SignName)  SignName, 
	UPPER (j.TitleSign)  TitleSign
	, (SELECT LookUpValueName FROM gnMstLookUpDtl WHERE CompanyCode = a.CompanyCode AND CodeID = 'NOTE' AND LookUpValue = 'SPFP01') Note1
	, (SELECT LookUpValueName FROM gnMstLookUpDtl WHERE CompanyCode = a.CompanyCode AND CodeID = 'NOTE' AND LookUpValue = 'SPFP02') Note2
	, CASE WHEN c.IsPKP = '0' THEN (SELECT LookUpValueName FROM gnMstLookUpDtl WHERE CompanyCode = a.CompanyCode AND CodeID = 'NOTE' AND LookUpValue = 'SPFP03') ELSE '' END Note3
FROM                         
(
   SELECT CompanyCode,
          BranchCode, 
		  FPJNo,
          PartNo,
		  DocNo,
		  PartNoOriginal,
          RetailPrice,
          DiscPct,
          SUM(DiscAmt) AS DiscAmt,
          SUM(NetSalesAmt) AS NetSalesAmt,
          SUM(QtyBill) AS QtyBill
   FROM SpTrnSFPJDtl WITH (NOLOCK, NOWAIT)
   GROUP BY CompanyCode, BranchCode,FPJNo, DocNo,PartNo,PartNoOriginal,
          RetailPrice, DiscPct
   HAVING CompanyCode = @CompanyCode
      AND BranchCode = @BranchCode
      AND FPJNo BETWEEN @FPJNoStart AND @FPJNoEnd
) a 
	INNER JOIN SpMstItemInfo b WITH (NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode
		AND a.PartNo = b.PartNo
    INNER JOIN SpTrnSFPJHdr c WITH (NOLOCK, NOWAIT) ON a.CompanyCode = c.CompanyCode
      AND a.BranchCode = c.BranchCode
      AND a.FPJNo = c.FPJNo
    INNER JOIN GnMstCustomer d WITH (NOLOCK, NOWAIT) ON c.CompanyCode = d.CompanyCode
       AND c.CustomerCode = d.CustomerCode
	INNER JOIN SpTrnSFPJInfo x WITH (NOLOCK, NOWAIT) ON c.CompanyCode = x.CompanyCode 
	   AND c.BranchCode = x.BranchCode
	   AND c.FPJNo = x.FPJNo
	INNER JOIN GnMstLookUpDtl e WITH (NOLOCK, NOWAIT) ON c.CompanyCode = e.CompanyCode
		AND c.TOPCode = e.LookUpValue
		AND e.CodeID = 'TOPC'
	INNER JOIN GnMstCoProfile f WITH (NOLOCK, NOWAIT) ON a.CompanyCode = f.CompanyCode
		AND a.BranchCode = f.BranchCode
	LEFT JOIN GnMstLookUpDtl h WITH (NOLOCK, NOWAIT) ON f.CompanyCode = h.CompanyCode
		AND f.CItyCode = h.LookUpValue
		AND h.CodeID = 'CITY'
	INNER JOIN GnMstTax i WITH (NOLOCK, NOWAIT) ON a.CompanyCode = i.CompanyCode 
		AND i.TaxCode = 'PPN'
	LEFT JOIN GnMstSignature j WITH (NOLOCK, NOWAIT) ON a.CompanyCode = j.CompanyCode
		AND a.BranchCode = j.BranchCode
		AND j.ProfitCenterCode = @ProfitCenter
		AND j.DocumentType = CONVERT (VARCHAR (3), 'SDH')
		AND j.SeqNo = @CounterDiv
	LEFT JOIN spTrnSPickingHdr k ON a.CompanyCode = k.CompanyCode
		AND a.BranchCode = k.BranchCode
		AND c.PickingSlipNo = k.PickingSlipNo
WHERE a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND a.FPJNo BETWEEN @FPJNoStart AND @FPJNoEnd
	--AND c.TypeOfGoods = @TypeOfGoods
ORDER BY a.FPJNo, a.PartNo ASC

END
go

if object_id('uspfn_GetLookupLMP') is not null
	drop PROCEDURE uspfn_GetLookupLMP
GO

create PROCEDURE [dbo].[uspfn_GetLookupLMP] @CompanyCode varchar(15), @BranchCode varchar(15), @SalesType varchar(15), @CodeID varchar(6),  
@TypeOfGoods varchar(15), @ProductType varchar(15)  
as  
SELECT * FROM   
(  
SELECT  
 PickingSlipNo, PickingSlipDate,  
 BPSFNo, BPSFDate,  
 (  
   SELECT TOP 1 PRODUCTTYPE FROM spTrnSBPSFDtl  
  WHERE spTrnSBPSFHdr.CompanyCode = spTrnSBPSFDtl.CompanyCode  
  AND spTrnSBPSFHdr.BranchCode = spTrnSBPSFDtl.BranchCode  
  AND spTrnSBPSFHdr.BPSFNo = spTrnSBPSFDtl.BPSFNo  
 ) AS ProductType,
 spTrnSBPSFHdr.CustomerCode,
 spTrnSBPSFHdr.CustomerCodeShip,
 spTrnSBPSFHdr.CustomerCodeBill as CustomerCodeTagih,
 b.CustomerName,
 b.Address1,
 b.Address2,
 b.Address3,
 b.Address4,
 b.CustomerName CustomerNameTagih,
 b.Address1 Address1Tagih,
 b.Address2 Address2Tagih,
 b.Address3 Address3Tagih,
 b.Address4 Address4Tagih,
 c.LookUpValueName TransType  
FROM spTrnSBPSFHdr  
left join gnMstCustomer b
ON spTrnSBPSFHdr.CompanyCode = b.CompanyCode
AND spTrnSBPSFHdr.CustomerCode = b.CustomerCode 
left join gnMstLookupDtl c on
spTrnSBPSFHdr.CompanyCode = c.CompanyCode
and spTrnSBPSFHdr.TransType= c.LookupValue 
AND c.CodeID = @CodeID
WHERE spTrnSBPSFHdr.CompanyCode = @CompanyCode  
AND spTrnSBPSFHdr.BranchCode    = @BranchCode  
AND spTrnSBPSFHdr.SalesType     = @SalesType  
AND spTrnSBPSFHdr.TypeOfGoods   = @TypeOfGoods  
AND (spTrnSBPSFHdr.Status = '1' OR spTrnSBPSFHdr.Status = '0')  
AND (spTrnSBPSFHdr.PickingSlipNo NOT IN (SELECT PickingSlipNo FROM spTrnSLmpHdr where CompanyCode = @CompanyCode AND BranchCode = @BranchCode))  
) A  
WHERE A.ProductType = @ProductType  
ORDER BY A.PickingSlipNo DESC
go

if object_id('uspfn_GetLmpDoc') is not null
	drop PROCEDURE uspfn_GetLmpDoc
GO
create procedure [dbo].[uspfn_GetLmpDoc] @CompanyCode varchar(15), @BranchCode varchar(15), @TypeOfGoods varchar(5), @TransType varchar(5), @CodeID varchar(6),@BeginDate datetime, @EndDate datetime  
as  
--declare @CompanyCode varchar(15)  
--declare @BranchCode varchar(15)  
--declare @TypeOfGoods varchar(15)  
--declare @TransType varchar(5)  
--declare @BeginDate Datetime  
--declare @EndDate datetime  
--set @CompanyCode = '6006406'  
--set @BranchCode = '6006401'  
--set @TypeOfGoods = '0'  
--set @TransType = '1%'  
--set @BeginDate = '2014/03/01'  
--set @EndDate = '2014/05/30'  
SELECT  
  
 spTrnSLmpHdr.CompanyCode,  
 spTrnSLmpHdr.BranchCode,  
 spTrnSLmpHdr.LmpNo  
,spTrnSLmpHdr.LmpDate  
,spTrnSLmpHdr.BPSFNo  
,spTrnSLmpHdr.BPSFDate  
,spTrnSLmpHdr.PickingSlipNo  
,spTrnSLmpHdr.PickingSlipDate  
,spTrnSLmpHdr.TypeOfGoods  
,spTrnSLmpHdr.CustomerCodeShip  
,spTrnSLmpHdr.CustomerCode  
,spTrnSLmpHdr.CustomerCodeBill  
,spTrnSLmpHdr.TotSalesQty  
,spTrnSLmpHdr.TotSalesAmt  
,spTrnSLmpHdr.TotDiscAmt  
,spTrnSLmpHdr.TotDPPAmt  
,spTrnSLmpHdr.TotPPNAmt  
,spTrnSLmpHdr.CreatedBy  
,spTrnSLmpHdr.CreatedDate  
,spTrnSLmpHdr.LastUpdateBy  
,spTrnSLmpHdr.LastUpdateDate  
,spTrnSLmpHdr.LastUpdateBy  
,spTrnSLmpHdr.LastUpdateDate  
,spTrnSLmpHdr.isPKP  
,spTrnSLmpHdr.isLocked  
,spTrnSLmpHdr.LockingBy  
,spTrnSLmpHdr.LockingDate,  
 spTrnSLmpHdr.CustomerCode,  
 spTrnSLmpHdr.CustomerCodeShip,
 spTrnSLmpHdr.CustomerCodeBill as CustomerCodeTagih,
 b.CustomerName,  
 b.Address1,  
 b.Address2,  
 b.Address3,  
 b.Address4,  
 b.CustomerName CustomerNameTagih,  
 b.Address1 Address1Tagih,  
 b.Address2 Address2Tagih,  
 b.Address3 Address3Tagih,  
 b.Address4 Address4Tagih,  
 c.LookUpValueName TransType    
  
FROM spTrnSLmpHdr  
left join gnMstCustomer b  
ON spTrnSLmpHdr.CompanyCode = b.CompanyCode  
AND spTrnSLmpHdr.CustomerCode = b.CustomerCode   
left join gnMstLookupDtl c on  
spTrnSLmpHdr.CompanyCode = c.CompanyCode  
and spTrnSLmpHdr.TransType= c.LookupValue   
AND c.CodeID = @CodeID  
WHERE spTrnSLmpHdr.CompanyCode=@CompanyCode  
  AND spTrnSLmpHdr.BranchCode=@BranchCode  
  AND spTrnSLmpHdr.TypeOfGoods = @TypeOfGoods   
  AND Convert(Varchar, spTrnSLmpHdr.LmpDate, 111) between @BeginDate and @EndDate  
  AND TransType LIKE @TransType  
ORDER BY spTrnSLmpHdr.LmpNo DESC
go

if object_id('uspfn_SvTrnServiceInsertDefaultTaskNew') is not null
	drop procedure uspfn_SvTrnServiceInsertDefaultTaskNew
GO
CREATE procedure [dbo].[uspfn_SvTrnServiceInsertDefaultTaskNew]
	@CompanyCode varchar(15),
	@BranchCode varchar(15),
	@ProductType varchar(15),
	@ServiceNo bigint,
	@ServiceBookNo varchar(15),
	@UserID varchar(15)
as      

--declare @CompanyCode varchar(15),
--		@BranchCode varchar(15),
--		@ProductType varchar(15),
--		@ServiceNo bigint,
--		@ServiceBookNo varchar(15)		

--set @CompanyCode = '6006406'
--set	@BranchCode = '6006401'
--set	@ProductType = '4W'
--set	@ServiceNo = '40655'
--set @ServiceBookNo = 'EJ06'

-- check jika count svTrnSrvTask ada maka tidak perlu insert task 
if(select count(*) from svTrnSrvTask
    where 1 = 1
      and CompanyCode = @CompanyCode
      and BranchCode  = @BranchCode
      and ProductType = @ProductType
      and ServiceNo   = @ServiceNo) > 0
	return

-- select data svTrnService
select * into #srv from (
  select a.* from svTrnService a
	where 1 = 1
	  and a.CompanyCode = @CompanyCode
	  and a.BranchCode  = @BranchCode
	  and a.ProductType = @ProductType
	  and a.ServiceNo   = @ServiceNo
)#srv

-----------------------------------------------
-- insert default svTrnSrvTask
-----------------------------------------------
select * into #task from(
select a.CompanyCode, a.ProductType, a.BasicModel, a.JobType, a.OperationNo, a.Description
	 , isnull(c.OperationHour, a.OperationHour) OperationHour
	 , isnull(c.ClaimHour, a.ClaimHour) ClaimHour
	 , isnull(c.LaborCost, a.LaborCost) LaborCost
	 , isnull(c.LaborPrice, a.LaborPrice) LaborPrice
	 , a.IsSubCon, a.IsCampaign, b.CreatedBy as LastupdateBy, getdate() as  LastupdateDate
	 , case when exists (
			select pkg.CompanyCode, pkg.PackageCode, pkg.JobType
			  from svMstPackage pkg
			 inner join svMstPackageTask tsk
				on tsk.CompanyCode = pkg.CompanyCode
			   and tsk.PackageCode = pkg.PackageCode
			 inner join svMstPackageContract con
				on con.CompanyCode = pkg.CompanyCode
			   and con.PackageCode = pkg.PackageCode
			 where pkg.CompanyCode = b.CompanyCode
			   and pkg.JobType = b.JobType
			   and tsk.OperationNo = a.OperationNo
			   and con.ChassisCode = b.ChassisCode
			   and con.ChassisNo = b.ChassisNo
		) then 'P' else (case when isnull(a.BillType, '') = '' then 'C' else a.BillType end)
		  end as BillType
  from svMstTask a
 inner join #srv b
    on b.CompanyCode = a.CompanyCode
   and b.ProductType = a.ProductType
   and b.BasicModel  = a.BasicModel
   and b.JobType     = a.JobType
  left join svMstTaskPrice c
	on c.CompanyCode = a.CompanyCode
   and c.BranchCode  = b.BranchCode
   and c.ProductType = a.ProductType
   and c.BasicModel  = a.BasicModel
   and c.JobType     = a.JobType
   and c.OperationNo = a.OperationNo
 where 1 = 1
)#task

-- jika svMstTask tidak tepat 1 record, return
if (select count(*) from #task) <> 1 return

select * into #job from(
select a.* from svMstJob a, #task b
 where 1 = 1
   and a.CompanyCode = b.CompanyCode
   and a.ProductType = b.ProductType
   and a.BasicModel  = b.BasicModel
   and a.JobType     = b.JobType
)#job

-- jika svMstJob tidak tepat 1 record, return
if (select count(*) from #job) <> 1 return

-- prepare data svTrnSrvTask yg akan di Insert
declare @JobType varchar(15) set @JobType = (select JobType from #job)

if (left(@JobType,3) = 'FSC' or left(@JobType,3) = 'PDI')
begin
	insert into svTrnSrvTask (CompanyCode, BranchCode, ProductType, ServiceNo, OperationNo, OperationHour, OperationCost, IsSubCon, SubConPrice, PONo, ClaimHour, TypeOfGoods, BillType, SharingTask, TaskStatus, StartService, FinishService, CreatedBy, CreatedDate, LastupdateBy, LastupdateDate, DiscPct)
	select
		 @CompanyCode CompanyCode
		,@BranchCode BranchCode
		,@ProductType ProductType
		,@ServiceNo ServiceNo
		,a.OperationNo
		,a.OperationHour
		,OperationCost = isnull((select top 1 a.RegularLaborAmount
						   from svMstPdiFscRate a, #srv b, #task c, #job d 
						  where 1 = 1
						    and a.CompanyCode = b.CompanyCode
						    and a.ProductType = b.ProductType
						    and a.BasicModel = b.BasicModel
						    and a.IsCampaign = c.IsCampaign
						    and a.TransmissionType = a.TransmissionType
						    and a.PdiFscSeq = d.PdiFscSeq
						    and a.EffectiveDate <= getdate()
						    and a.IsActive = 1
						 order by a.EffectiveDate desc),0)
		,IsSubCon
		,a.LaborCost SubConPrice
		,'' PONo
		,ClaimHour
		,'L' TypeOfGoods
	    , case when isnull(a.BillType, '') = '' then 'C' else a.BillType end as BillType
		,'0' SharingTask
		,'0' TaskStatus
		,null StartService
		,null FinishService
		,b.LastupdateBy CreatedBy
		,b.LastupdateDate CreatedDate
		,b.LastupdateBy
		,b.LastupdateDate
		,isnull((
			select cus.LaborDiscPct
			  from svMstBillingType bil
			 inner join gnMstCustomerProfitCenter cus
				on cus.CompanyCode = bil.CompanyCode
			   and cus.CustomerCode = bil.CustomerCode 
			 where 1 = 1
			   and bil.CompanyCode = @CompanyCode
			   and cus.BranchCode = @BranchCode
			   and cus.ProfitCenterCode = '200'
			   and bil.BillType = 'F'
			), b.LaborDiscPct) as LaborDiscPct
	from #task a, #srv b
end
else if @JobType = 'CLAIM'
begin
	insert into svTrnSrvTask (CompanyCode, BranchCode, ProductType, ServiceNo, OperationNo, OperationHour, OperationCost, IsSubCon, SubConPrice, PONo, ClaimHour, TypeOfGoods, BillType, SharingTask, TaskStatus, StartService, FinishService, CreatedBy, CreatedDate, LastupdateBy, LastupdateDate, DiscPct)
	select
		 @CompanyCode CompanyCode
		,@BranchCode BranchCode
		,@ProductType ProductType
		,@ServiceNo ServiceNo
		,a.OperationNo
		,a.ClaimHour OperationHour
		,a.LaborPrice OperationCost
		,a.IsSubCon
		,a.LaborCost SubConPrice
		,'' PONo
		,a.ClaimHour
		,'L' TypeOfGoods
		,'W' BillType
		,'0' SharingTask
		,'0' TaskStatus
		,null StartService
		,null FinishService
		,b.LastupdateBy CreatedBy
		,b.LastupdateDate CreatedDate
		,b.LastupdateBy
		,b.LastupdateDate
		,b.LaborDiscPct
	from #task a, #srv b
end
else if @JobType = 'REWORK'
begin
	insert into svTrnSrvTask (CompanyCode, BranchCode, ProductType, ServiceNo, OperationNo, OperationHour, OperationCost, IsSubCon, SubConPrice, PONo, ClaimHour, TypeOfGoods, BillType, SharingTask, TaskStatus, StartService, FinishService, CreatedBy, CreatedDate, LastupdateBy, LastupdateDate, DiscPct)
	select
		 @CompanyCode CompanyCode
		,@BranchCode BranchCode
		,@ProductType ProductType
		,@ServiceNo ServiceNo
		,a.OperationNo
		,a.OperationHour
		,a.LaborPrice OperationCost
		,a.IsSubCon
		,a.LaborCost SubConPrice
		,'' PONo
		,ClaimHour
		,'L' TypeOfGoods
		,'I' BillType
		,'0' SharingTask
		,'0' TaskStatus
		,null StartService
		,null FinishService
		,b.LastupdateBy CreatedBy
		,b.LastupdateDate CreatedDate
		,b.LastupdateBy
		,b.LastupdateDate
		,b.LaborDiscPct
	from #task a, #srv b
end
else
begin
	insert into svTrnSrvTask (CompanyCode, BranchCode, ProductType, ServiceNo, OperationNo, OperationHour, OperationCost, IsSubCon, SubConPrice, PONo, ClaimHour, TypeOfGoods, BillType, SharingTask, TaskStatus, StartService, FinishService, CreatedBy, CreatedDate, LastupdateBy, LastupdateDate, DiscPct)
	select
		 @CompanyCode CompanyCode
		,@BranchCode BranchCode
		,@ProductType ProductType
		,@ServiceNo ServiceNo
		,a.OperationNo
		,a.OperationHour
		,a.LaborPrice OperationCost
		,IsSubCon
		,a.LaborCost SubConPrice
		,'' PONo
		,ClaimHour
		,'L' TypeOfGoods
		, case when isnull(a.BillType, '') = '' then 'C' else a.BillType end as BillType
		,'0' SharingTask
		,'0' TaskStatus
		,null StartService
		,null FinishService
		,b.LastupdateBy CreatedBy
		,b.LastupdateDate CreatedDate
		,b.LastupdateBy
		,b.LastupdateDate
		,isnull((
			select top 1 tsk.DiscPct
			  from svMstPackage pkg
			 inner join svMstPackageTask tsk
				on tsk.CompanyCode = pkg.CompanyCode
			   and tsk.PackageCode = pkg.PackageCode
			 inner join svMstPackageContract con
				on con.CompanyCode = pkg.CompanyCode
			   and con.PackageCode = pkg.PackageCode
			 where pkg.CompanyCode = b.CompanyCode
			   and pkg.JobType = b.JobType
			   and tsk.OperationNo = a.OperationNo
			   and con.ChassisCode = b.ChassisCode
			   and con.ChassisNo = b.ChassisNo
			), b.LaborDiscPct) LaborDiscPct
		
	from #task a, #srv b
end
-----------------------------------------------
-- insert default svTrnSrvItem
-----------------------------------------------
select * into #part from(
select a.* from svMstTaskPart a, #task b
 where 1 = 1
   and a.CompanyCode = b.CompanyCode
   and a.ProductType = b.ProductType
   and a.BasicModel  = b.BasicModel
   and a.JobType     = b.JobType
   and a.OperationNo = b.OperationNo
)#part

DECLARE @QueryTemp VARCHAR(MAX)

  declare @tblTemp as table  
	  (  
	  CompanyCode varchar(20),
	  BranchCode varchar(20),
	  PartNo varchar(20),
	  TypeOfGoods varchar(2),
	   RetailPrice decimal(18,2),  
	   PurcDiscPct decimal(18,2) 
	  )
	    
	  	
		set @QueryTemp = 'select c.CompanyCode,c.BranchCode,a.PartNo,a.TypeOfGoods,b.RetailPrice,a.PurcDiscPct from '+ dbo.GetDbMD(@CompanyCode, @BranchCode) +'..spmstitems a
		join '+ dbo.GetDbMD(@CompanyCode, @BranchCode) +'..spMstItemPrice b on a.CompanyCode = b.CompanyCode and a.BranchCode = b.BranchCode and a.PartNo = b.PartNo
		join gnMstCompanyMapping c on a.CompanyCode = c.CompanyMD and a.BranchCode = c.BranchMD'
		          
	  insert into @tblTemp    
	  exec (@QueryTemp)

	  declare @PurchaseDisc as decimal  
	  set @PurchaseDisc = (select isnull(DiscPct, 0) from gnMstSupplierProfitCenter   
		   where CompanyCode = @CompanyCode   
		   and BranchCode = @BranchCode  
		   and SupplierCode = dbo.GetBranchMD(@CompanyCode, @BranchCode)
		   and ProfitCenterCode = '300')  
	         
	  if (@PurchaseDisc = 0) raiserror ('Purchase Discount belum di-setting untuk Part tersebut!',16,1);         

insert into svTrnSrvItem (CompanyCode, BranchCode, ProductType, ServiceNo, PartNo, PartSeq, DemandQty, SupplyQty, ReturnQty, CostPrice, RetailPrice, TypeOfGoods, BillType, SupplySlipNo, SupplySlipDate, SSReturnNo, SSReturnDate, CreatedBy, CreatedDate, LastupdateBy, LastupdateDate, DiscPct)
select 
	 @CompanyCode CompanyCode
	,@BranchCode BranchCode
	,@ProductType ProductType
	,@ServiceNo ServiceNo
	,a.PartNo
	,(row_number() over (order by a.PartNo)) PartSeq
	,a.Quantity DemandQty
	,'0' SupplyQty
	,'0' ReturnQty
	, case when ISNULL(d.PurcDiscPct,0) = 0 
	then (d.RetailPrice - (d.RetailPrice *(@PurchaseDisc * 0.01))) 
	else (d.RetailPrice - (d.RetailPrice * (d.PurcDiscPct * 0.01))) end CostPrice
	,case rtrim(a.BillType) when 'F' then a.RetailPrice else d.RetailPrice end RetailPrice
	,d.TypeOfGoods
	,case when exists (
			select pkg.CompanyCode, pkg.PackageCode, pkg.JobType
			  from svMstPackage pkg
			 inner join svMstPackagePart prt
				on prt.CompanyCode = pkg.CompanyCode
			   and prt.PackageCode = pkg.PackageCode
			 inner join svMstPackageContract con
				on con.CompanyCode = pkg.CompanyCode
			   and con.PackageCode = pkg.PackageCode
			 inner join #srv srv
				on srv.CompanyCode = con.CompanyCode
			   and srv.ChassisCode = con.ChassisCode
			   and srv.ChassisNo = con.ChassisNo
			 where pkg.CompanyCode = b.CompanyCode
			   and pkg.JobType = b.JobType
			   and prt.PartNo = a.PartNo
		) then 'P' else 
		(case when isnull(a.BillType, '') = '' then 'C' else 
			(case when substring(@ServiceBookNo, 1, 2) >= 'EJ' and f.PartName like '%OIL FILTER%' and (select PdiFscSeq from #job) = 1 then 'C' else 		
			
				(case when substring(@ServiceBookNo, 1, 2) >= 'EJ' and f.PartName like '%ENGINE%' and (select PdiFscSeq from #job) = 3 then 'C' else 		
				a.BillType 
				end)
			
			end)		
		end)
		  end as BillType
	,null SupplySlipNo
	,null SupplySlipDate
	,null SSReturnNo
	,null SSReturnDate
	,b.LastupdateBy CreatedBy
	,b.LastupdateDate CreatedDate
	,b.LastupdateBy
	,b.LastupdateDate
	,isnull((
		select top 1 prt.DiscPct
		  from svMstPackage pkg
		 inner join svMstPackagePart prt
			on prt.CompanyCode = pkg.CompanyCode
		   and prt.PackageCode = pkg.PackageCode
		 inner join svMstPackageContract con
			on con.CompanyCode = pkg.CompanyCode
		   and con.PackageCode = pkg.PackageCode
		 inner join #srv srv
			on srv.CompanyCode = con.CompanyCode
		   and srv.ChassisCode = con.ChassisCode
		   and srv.ChassisNo = con.ChassisNo
		 where pkg.CompanyCode = b.CompanyCode
		   and pkg.JobType = b.JobType
		   and prt.PartNo = a.PartNo
		), 
		(case when rtrim(a.BillType) = 'F' and rtrim(e.ParaValue) = 'SPAREPART' then 0
		      when rtrim(a.BillType) = 'F' then 0
		      when rtrim(e.ParaValue) = 'SPAREPART' then (select top 1 PartDiscPct from #srv) 
			  else (select top 1 MaterialDiscPct from #srv) end)
		) as DiscPct
  from #part a
  left join #task b
    on b.CompanyCode = a.CompanyCode
   and b.ProductType = a.ProductType
   and b.BasicModel  = a.BasicModel
   and b.JobType     = a.JobType
   and b.OperationNo = a.OperationNo
   LEFT join @tblTemp d
   on d.CompanyCode = a.CompanyCode
   and d.BranchCode = @BranchCode
   and d.PartNo = a.PartNo
  --left join spMstItemPrice c
  --  on c.CompanyCode = a.CompanyCode
  -- and c.BranchCode  = @BranchCode
  -- and c.PartNo      = a.PartNo
  --left join spMstItems d
  --  on d.CompanyCode = a.CompanyCode
  -- and d.BranchCode  = @BranchCode
  -- and d.PartNo      = a.PartNo
  left join gnMstLookupDtl e
    on e.CompanyCode = d.CompanyCode
   and e.CodeID = 'GTGO'
   and e.LookupValue = d.TypeOfGoods
   left join spMstItemInfo f
	on f.CompanyCode = a.CompanyCode
	and f.PartNo = a.PartNo
 where 1 = 1
   and b.CompanyCode = a.CompanyCode
   and b.ProductType = a.ProductType
   and b.BasicModel  = a.BasicModel
   and b.JobType     = a.JobType
   and b.OperationNo = a.OperationNo
exec uspfn_SvInsertDefaultTaskMovement @CompanyCode, @BranchCode, @ProductType, @ServiceNo, @UserID

drop table #srv
drop table #task
drop table #part
drop table #job
GO
if object_id('Uspfn_SrvItemUpdateSSReturnNo') is not null
	drop procedure Uspfn_SrvItemUpdateSSReturnNo
GO
create procedure [dbo].[Uspfn_SrvItemUpdateSSReturnNo] @CompanyCode varchar(15), @BranchCode varchar(15),
@ProductType varchar(2), @ReturnNo varchar(25), @PartNo varchar(25), @IsSaveProcess bit, @LastUpdateBy varchar(25)
as
SELECT
	a.CompanyCode
	, a.BranchCode
	, d.ProductType
	, d.ServiceNo
	, a.PartNo
	, (SELECT TOP 1 PartSeq FROM svTrnSrvItem WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode 
        AND ProductType = d.ProductType AND ServiceNo = d.ServiceNo AND PartNo = a.PartNo AND SupplySlipNo = 
        c.DocNo ORDER BY PartSeq DESC) PartSeq
	, a.ReturnNo SSReturnNo
	, b.ReturnDate SSReturnDate
	, a.QtyReturn
INTO
	#SrvItem
FROM 
	spTrnSRturSSDtl a WITH(NOLOCK, NOWAIT)
	LEFT JOIN spTrnSRturSSHdr b WITH(NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode
		AND a.BranchCode = b.BranchCode
		AND a.ReturnNo = b.ReturnNo
	LEFT JOIN spTrnSORDHdr c WITH(NOLOCK, NOWAIT) ON a.CompanyCode = c.CompanyCode
		AND a.BranchCode = c.BranchCode
		AND a.DocNo = c.DocNo
	LEFT JOIN svTrnService d WITH(NOLOCK, NOWAIT) ON a.CompanyCode = d.CompanyCode
		AND a.BranchCode = d.BranchCode
		AND c.UsageDocNo = d.JobOrderNo
WHERE
	a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND d.ProductType = @ProductType
	AND a.ReturnNo = @ReturnNo
	AND a.PartNo = @PartNo

UPDATE
	svTrnSrvItem
SET
	SSReturnNo = CASE @IsSaveProcess WHEN '1' THEN b.SSReturnNo ELSE '' END 
    , SSReturnDate = CASE @IsSaveProcess WHEN '1' THEN b.SSReturnDate ELSE '1900-01-01 00:00:00.000' END
	, ReturnQty = b.QtyReturn
	, LastupdateBy = @LastupdateBy
	, LastupdateDate = GETDATE()
FROM
	svTrnSrvItem a, #SrvItem b
WHERE
	a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.ProductType = b.ProductType
	AND a.ServiceNo = b.ServiceNo
	AND a.PartNo = b.PartNo
	AND a.PartSeq = b.PartSeq

DROP TABLE #SrvItem
GO

if object_id('uspfn_SvInqGetClaim') is not null
	drop procedure uspfn_SvInqGetClaim
GO
create procedure [dbo].[uspfn_SvInqGetClaim]
	 @CompanyCode	varchar(20),
	 @InvoiceFrom	varchar(20),
	 @InvoiceTo		varchar(20),
	 @BranchFrom	varchar(20),
	 @BranchTo		varchar(20),
	 @IsSprClaim	bit = 0

as

if @BranchFrom = '[All Branch]' and @BranchTo = '[All Branch]'
begin
	select * into #t1 from (
	select (row_number() over (order by a.InvoiceNo)) as SeqNo
		 , a.BranchCode, a.InvoiceNo, isnull(b.JobOrderDate, c.InvoiceDate) InvoiceDate
		 , a.IsCbu, a.CategoryCode, a.ComplainCode, a.DefectCode
		 , a.SubletHour, a.SubletAmt, a.CausalPartNo, a.TroubleDescription
		 , a.ProblemExplanation, a.OperationNo, a.OperationHour, a.OperationAmt
		 , isnull(b.BasicModel, c.BasicModel) BasicModel
		 , isnull(b.ServiceBookNo, '') ServiceBookNo
		 , isnull(b.ChassisCode, c.ChassisCode) ChassisCode
		 , isnull(b.ChassisNo, c.ChassisNo) ChassisNo
		 , isnull(b.EngineCode, c.EngineCode) EngineCode
		 , isnull(b.EngineNo, c.EngineNo) EngineNo
		 , isnull(b.Odometer, 0) Odometer
		 , isnull(b.TotalSrvAmount, c.TotalSrvAmt) ClaimAmt
		 , isnull(b.TotalSrvAmount, c.TotalSrvAmt) TotalSrvAmt
	  from svTrnInvClaim a
	  left join svTrnService b
		on b.CompanyCode = a.CompanyCode
	   and b.BranchCode = a.BranchCode
	   and b.JobOrderNo = a.InvoiceNo
	  left join svTrnInvoice c
		on c.CompanyCode = a.CompanyCode
	   and c.BranchCode = a.BranchCode
	   and c.InvoiceNo = a.InvoiceNo
	 where 1 = 1
	   and a.CompanyCode = @CompanyCode
	   and convert(varchar, b.JobOrderDate, 106) between @InvoiceFrom and @InvoiceTo
	   and isnull(b.IsSparepartClaim, 0) = @IsSprClaim
	   and not exists (
		select * from svTrnClaimApplication
		 where CompanyCode = a.CompanyCode
		   and BranchCode = a.BranchCode
		   and InvoiceNo = a.InvoiceNo
		)
	   and b.ServiceStatus != '6' --Penambahan
	)#
	
	select * from #t1

	select (row_number() over (order by a.BasicModel)) as SeqNo
		 , a.BasicModel, count(a.BasicModel) Qty, sum(a.TotalSrvAmt) as TotalSrvAmt
	  from #t1 a
	 group by a.BasicModel

	drop table #t1
end
else
begin
	select * into #t2 from (
	select (row_number() over (order by a.InvoiceNo)) as SeqNo
		 , a.BranchCode, a.InvoiceNo, isnull(b.JobOrderDate, c.InvoiceDate) InvoiceDate
		 , a.IsCbu, a.CategoryCode, a.ComplainCode, a.DefectCode
		 , a.SubletHour, a.SubletAmt, a.CausalPartNo, a.TroubleDescription
		 , a.ProblemExplanation, a.OperationNo, a.OperationHour, a.OperationAmt
		 , isnull(b.BasicModel, c.BasicModel) BasicModel
		 , isnull(b.ServiceBookNo, '') ServiceBookNo
		 , isnull(b.ChassisCode, c.ChassisCode) ChassisCode
		 , isnull(b.ChassisNo, c.ChassisNo) ChassisNo
		 , isnull(b.EngineCode, c.EngineCode) EngineCode
		 , isnull(b.EngineNo, c.EngineNo) EngineNo
		 , isnull(b.Odometer, 0) Odometer
		 , isnull(b.TotalSrvAmount, c.TotalSrvAmt) ClaimAmt
		 , isnull(b.TotalSrvAmount, c.TotalSrvAmt) TotalSrvAmt
	  from svTrnInvClaim a
	  left join svTrnService b
		on b.CompanyCode = a.CompanyCode
	   and b.BranchCode = a.BranchCode
	   and b.JobOrderNo = a.InvoiceNo
	  left join svTrnInvoice c
		on c.CompanyCode = a.CompanyCode
	   and c.BranchCode = a.BranchCode
	   and c.InvoiceNo = a.InvoiceNo
	 where 1 = 1
	   and a.CompanyCode = @CompanyCode
	   and a.BranchCode between @BranchFrom and @BranchTo
	   and convert(varchar, a.InvoiceNo, 106) between @InvoiceFrom and @InvoiceTo
	   --and a.InvoiceNo between @InvoiceFrom and @InvoiceTo
	   and isnull(b.IsSparepartClaim, 0) = @IsSprClaim
	   and not exists (
		select * from svTrnClaimApplication
		 where CompanyCode = a.CompanyCode
		   and BranchCode = a.BranchCode
		   and InvoiceNo = a.InvoiceNo
		)
       and b.ServiceStatus != '6' --Penambahan
	)#
		
	select * from #t2

	select (row_number() over (order by a.BasicModel)) as SeqNo
		 , a.BasicModel, count(a.BasicModel) Qty, sum(a.TotalSrvAmt) as TotalSrvAmt
	  from #t2 a
	 group by a.BasicModel

	drop table #t2
end
Go

if object_id('uspfn_SvTrnServiceInsertDefaultTaskNew') is not null
	drop procedure uspfn_SvTrnServiceInsertDefaultTaskNew
GO
CREATE procedure [dbo].[uspfn_SvTrnServiceInsertDefaultTaskNew]
	@CompanyCode varchar(15),
	@BranchCode varchar(15),
	@ProductType varchar(15),
	@ServiceNo bigint,
	@ServiceBookNo varchar(15),
	@UserID varchar(15)
as      

--declare @CompanyCode varchar(15),
--		@BranchCode varchar(15),
--		@ProductType varchar(15),
--		@ServiceNo bigint,
--		@ServiceBookNo varchar(15)		

--set @CompanyCode = '6006406'
--set	@BranchCode = '6006401'
--set	@ProductType = '4W'
--set	@ServiceNo = '40655'
--set @ServiceBookNo = 'EJ06'

-- check jika count svTrnSrvTask ada maka tidak perlu insert task 
if(select count(*) from svTrnSrvTask
    where 1 = 1
      and CompanyCode = @CompanyCode
      and BranchCode  = @BranchCode
      and ProductType = @ProductType
      and ServiceNo   = @ServiceNo) > 0
	return

-- select data svTrnService
select * into #srv from (
  select a.* from svTrnService a
	where 1 = 1
	  and a.CompanyCode = @CompanyCode
	  and a.BranchCode  = @BranchCode
	  and a.ProductType = @ProductType
	  and a.ServiceNo   = @ServiceNo
)#srv

-----------------------------------------------
-- insert default svTrnSrvTask
-----------------------------------------------
select * into #task from(
select a.CompanyCode, a.ProductType, a.BasicModel, a.JobType, a.OperationNo, a.Description
	 , isnull(c.OperationHour, a.OperationHour) OperationHour
	 , isnull(c.ClaimHour, a.ClaimHour) ClaimHour
	 , isnull(c.LaborCost, a.LaborCost) LaborCost
	 , isnull(c.LaborPrice, a.LaborPrice) LaborPrice
	 , a.IsSubCon, a.IsCampaign, b.CreatedBy as LastupdateBy, getdate() as  LastupdateDate
	 , case when exists (
			select pkg.CompanyCode, pkg.PackageCode, pkg.JobType
			  from svMstPackage pkg
			 inner join svMstPackageTask tsk
				on tsk.CompanyCode = pkg.CompanyCode
			   and tsk.PackageCode = pkg.PackageCode
			 inner join svMstPackageContract con
				on con.CompanyCode = pkg.CompanyCode
			   and con.PackageCode = pkg.PackageCode
			 where pkg.CompanyCode = b.CompanyCode
			   and pkg.JobType = b.JobType
			   and tsk.OperationNo = a.OperationNo
			   and con.ChassisCode = b.ChassisCode
			   and con.ChassisNo = b.ChassisNo
		) then 'P' else (case when isnull(a.BillType, '') = '' then 'C' else a.BillType end)
		  end as BillType
  from svMstTask a
 inner join #srv b
    on b.CompanyCode = a.CompanyCode
   and b.ProductType = a.ProductType
   and b.BasicModel  = a.BasicModel
   and b.JobType     = a.JobType
  left join svMstTaskPrice c
	on c.CompanyCode = a.CompanyCode
   and c.BranchCode  = b.BranchCode
   and c.ProductType = a.ProductType
   and c.BasicModel  = a.BasicModel
   and c.JobType     = a.JobType
   and c.OperationNo = a.OperationNo
 where 1 = 1
)#task

-- jika svMstTask tidak tepat 1 record, return
if (select count(*) from #task) <> 1 return

select * into #job from(
select a.* from svMstJob a, #task b
 where 1 = 1
   and a.CompanyCode = b.CompanyCode
   and a.ProductType = b.ProductType
   and a.BasicModel  = b.BasicModel
   and a.JobType     = b.JobType
)#job

-- jika svMstJob tidak tepat 1 record, return
if (select count(*) from #job) <> 1 return

-- prepare data svTrnSrvTask yg akan di Insert
declare @JobType varchar(15) set @JobType = (select JobType from #job)

if (left(@JobType,3) = 'FSC' or left(@JobType,3) = 'PDI')
begin
	insert into svTrnSrvTask (CompanyCode, BranchCode, ProductType, ServiceNo, OperationNo, OperationHour, OperationCost, IsSubCon, SubConPrice, PONo, ClaimHour, TypeOfGoods, BillType, SharingTask, TaskStatus, StartService, FinishService, CreatedBy, CreatedDate, LastupdateBy, LastupdateDate, DiscPct)
	select
		 @CompanyCode CompanyCode
		,@BranchCode BranchCode
		,@ProductType ProductType
		,@ServiceNo ServiceNo
		,a.OperationNo
		,a.OperationHour
		,OperationCost = isnull((select top 1 a.RegularLaborAmount
						   from svMstPdiFscRate a, #srv b, #task c, #job d 
						  where 1 = 1
						    and a.CompanyCode = b.CompanyCode
						    and a.ProductType = b.ProductType
						    and a.BasicModel = b.BasicModel
						    and a.IsCampaign = c.IsCampaign
						    and a.TransmissionType = a.TransmissionType
						    and a.PdiFscSeq = d.PdiFscSeq
						    and a.EffectiveDate <= getdate()
						    and a.IsActive = 1
						 order by a.EffectiveDate desc),0)
		,IsSubCon
		,a.LaborCost SubConPrice
		,'' PONo
		,ClaimHour
		,'L' TypeOfGoods
	    , case when isnull(a.BillType, '') = '' then 'C' else a.BillType end as BillType
		,'0' SharingTask
		,'0' TaskStatus
		,null StartService
		,null FinishService
		,b.LastupdateBy CreatedBy
		,b.LastupdateDate CreatedDate
		,b.LastupdateBy
		,b.LastupdateDate
		,isnull((
			select cus.LaborDiscPct
			  from svMstBillingType bil
			 inner join gnMstCustomerProfitCenter cus
				on cus.CompanyCode = bil.CompanyCode
			   and cus.CustomerCode = bil.CustomerCode 
			 where 1 = 1
			   and bil.CompanyCode = @CompanyCode
			   and cus.BranchCode = @BranchCode
			   and cus.ProfitCenterCode = '200'
			   and bil.BillType = 'F'
			), b.LaborDiscPct) as LaborDiscPct
	from #task a, #srv b
end
else if @JobType = 'CLAIM'
begin
	insert into svTrnSrvTask (CompanyCode, BranchCode, ProductType, ServiceNo, OperationNo, OperationHour, OperationCost, IsSubCon, SubConPrice, PONo, ClaimHour, TypeOfGoods, BillType, SharingTask, TaskStatus, StartService, FinishService, CreatedBy, CreatedDate, LastupdateBy, LastupdateDate, DiscPct)
	select
		 @CompanyCode CompanyCode
		,@BranchCode BranchCode
		,@ProductType ProductType
		,@ServiceNo ServiceNo
		,a.OperationNo
		,a.ClaimHour OperationHour
		,a.LaborPrice OperationCost
		,a.IsSubCon
		,a.LaborCost SubConPrice
		,'' PONo
		,a.ClaimHour
		,'L' TypeOfGoods
		,'W' BillType
		,'0' SharingTask
		,'0' TaskStatus
		,null StartService
		,null FinishService
		,b.LastupdateBy CreatedBy
		,b.LastupdateDate CreatedDate
		,b.LastupdateBy
		,b.LastupdateDate
		,b.LaborDiscPct
	from #task a, #srv b
end
else if @JobType = 'REWORK'
begin
	insert into svTrnSrvTask (CompanyCode, BranchCode, ProductType, ServiceNo, OperationNo, OperationHour, OperationCost, IsSubCon, SubConPrice, PONo, ClaimHour, TypeOfGoods, BillType, SharingTask, TaskStatus, StartService, FinishService, CreatedBy, CreatedDate, LastupdateBy, LastupdateDate, DiscPct)
	select
		 @CompanyCode CompanyCode
		,@BranchCode BranchCode
		,@ProductType ProductType
		,@ServiceNo ServiceNo
		,a.OperationNo
		,a.OperationHour
		,a.LaborPrice OperationCost
		,a.IsSubCon
		,a.LaborCost SubConPrice
		,'' PONo
		,ClaimHour
		,'L' TypeOfGoods
		,'I' BillType
		,'0' SharingTask
		,'0' TaskStatus
		,null StartService
		,null FinishService
		,b.LastupdateBy CreatedBy
		,b.LastupdateDate CreatedDate
		,b.LastupdateBy
		,b.LastupdateDate
		,b.LaborDiscPct
	from #task a, #srv b
end
else
begin
	insert into svTrnSrvTask (CompanyCode, BranchCode, ProductType, ServiceNo, OperationNo, OperationHour, OperationCost, IsSubCon, SubConPrice, PONo, ClaimHour, TypeOfGoods, BillType, SharingTask, TaskStatus, StartService, FinishService, CreatedBy, CreatedDate, LastupdateBy, LastupdateDate, DiscPct)
	select
		 @CompanyCode CompanyCode
		,@BranchCode BranchCode
		,@ProductType ProductType
		,@ServiceNo ServiceNo
		,a.OperationNo
		,a.OperationHour
		,a.LaborPrice OperationCost
		,IsSubCon
		,a.LaborCost SubConPrice
		,'' PONo
		,ClaimHour
		,'L' TypeOfGoods
		, case when isnull(a.BillType, '') = '' then 'C' else a.BillType end as BillType
		,'0' SharingTask
		,'0' TaskStatus
		,null StartService
		,null FinishService
		,b.LastupdateBy CreatedBy
		,b.LastupdateDate CreatedDate
		,b.LastupdateBy
		,b.LastupdateDate
		,isnull((
			select top 1 tsk.DiscPct
			  from svMstPackage pkg
			 inner join svMstPackageTask tsk
				on tsk.CompanyCode = pkg.CompanyCode
			   and tsk.PackageCode = pkg.PackageCode
			 inner join svMstPackageContract con
				on con.CompanyCode = pkg.CompanyCode
			   and con.PackageCode = pkg.PackageCode
			 where pkg.CompanyCode = b.CompanyCode
			   and pkg.JobType = b.JobType
			   and tsk.OperationNo = a.OperationNo
			   and con.ChassisCode = b.ChassisCode
			   and con.ChassisNo = b.ChassisNo
			), b.LaborDiscPct) LaborDiscPct
		
	from #task a, #srv b
end
-----------------------------------------------
-- insert default svTrnSrvItem
-----------------------------------------------
select * into #part from(
select a.* from svMstTaskPart a, #task b
 where 1 = 1
   and a.CompanyCode = b.CompanyCode
   and a.ProductType = b.ProductType
   and a.BasicModel  = b.BasicModel
   and a.JobType     = b.JobType
   and a.OperationNo = b.OperationNo
)#part

DECLARE @QueryTemp VARCHAR(MAX)

  declare @tblTemp as table  
	  (  
	  CompanyCode varchar(20),
	  BranchCode varchar(20),
	  PartNo varchar(20),
	  TypeOfGoods varchar(2),
	   RetailPrice decimal(18,2),  
	   PurcDiscPct decimal(18,2) 
	  )
	    
	  	
		set @QueryTemp = 'select c.CompanyCode,c.BranchCode,a.PartNo,a.TypeOfGoods,b.RetailPrice,a.PurcDiscPct from '+ dbo.GetDbMD(@CompanyCode, @BranchCode) +'..spmstitems a
		join '+ dbo.GetDbMD(@CompanyCode, @BranchCode) +'..spMstItemPrice b on a.CompanyCode = b.CompanyCode and a.BranchCode = b.BranchCode and a.PartNo = b.PartNo
		join gnMstCompanyMapping c on a.CompanyCode = c.CompanyMD and a.BranchCode = c.BranchMD'
		          
	  insert into @tblTemp    
	  exec (@QueryTemp)

	  declare @PurchaseDisc as decimal  
	  set @PurchaseDisc = (select DiscPct from gnMstSupplierProfitCenter   
		   where CompanyCode = @CompanyCode   
		   and BranchCode = @BranchCode  
		   and SupplierCode = dbo.GetBranchMD(@CompanyCode, @BranchCode)
		   and ProfitCenterCode = '300')  
	         
	  if (@PurchaseDisc is null) raiserror ('Profit Center 300 belum tersetting pada Supplier tersebut!!!',16,1);         

insert into svTrnSrvItem (CompanyCode, BranchCode, ProductType, ServiceNo, PartNo, PartSeq, DemandQty, SupplyQty, ReturnQty, CostPrice, RetailPrice, TypeOfGoods, BillType, SupplySlipNo, SupplySlipDate, SSReturnNo, SSReturnDate, CreatedBy, CreatedDate, LastupdateBy, LastupdateDate, DiscPct)
select 
	 @CompanyCode CompanyCode
	,@BranchCode BranchCode
	,@ProductType ProductType
	,@ServiceNo ServiceNo
	,a.PartNo
	,(row_number() over (order by a.PartNo)) PartSeq
	,a.Quantity DemandQty
	,'0' SupplyQty
	,'0' ReturnQty
	, case when ISNULL(d.PurcDiscPct,0) = 0 
	then (d.RetailPrice - (d.RetailPrice *(@PurchaseDisc * 0.01))) 
	else (d.RetailPrice - (d.RetailPrice * (d.PurcDiscPct * 0.01))) end CostPrice
	,case rtrim(a.BillType) when 'F' then a.RetailPrice else d.RetailPrice end RetailPrice
	,d.TypeOfGoods
	,case when exists (
			select pkg.CompanyCode, pkg.PackageCode, pkg.JobType
			  from svMstPackage pkg
			 inner join svMstPackagePart prt
				on prt.CompanyCode = pkg.CompanyCode
			   and prt.PackageCode = pkg.PackageCode
			 inner join svMstPackageContract con
				on con.CompanyCode = pkg.CompanyCode
			   and con.PackageCode = pkg.PackageCode
			 inner join #srv srv
				on srv.CompanyCode = con.CompanyCode
			   and srv.ChassisCode = con.ChassisCode
			   and srv.ChassisNo = con.ChassisNo
			 where pkg.CompanyCode = b.CompanyCode
			   and pkg.JobType = b.JobType
			   and prt.PartNo = a.PartNo
		) then 'P' else 
		(case when isnull(a.BillType, '') = '' then 'C' else 
			(case when substring(@ServiceBookNo, 1, 2) >= 'EJ' and f.PartName like '%OIL FILTER%' and (select PdiFscSeq from #job) = 1 then 'C' else 		
			
				(case when substring(@ServiceBookNo, 1, 2) >= 'EJ' and f.PartName like '%ENGINE%' and (select PdiFscSeq from #job) = 3 then 'C' else 		
				a.BillType 
				end)
			
			end)		
		end)
		  end as BillType
	,null SupplySlipNo
	,null SupplySlipDate
	,null SSReturnNo
	,null SSReturnDate
	,b.LastupdateBy CreatedBy
	,b.LastupdateDate CreatedDate
	,b.LastupdateBy
	,b.LastupdateDate
	,isnull((
		select top 1 prt.DiscPct
		  from svMstPackage pkg
		 inner join svMstPackagePart prt
			on prt.CompanyCode = pkg.CompanyCode
		   and prt.PackageCode = pkg.PackageCode
		 inner join svMstPackageContract con
			on con.CompanyCode = pkg.CompanyCode
		   and con.PackageCode = pkg.PackageCode
		 inner join #srv srv
			on srv.CompanyCode = con.CompanyCode
		   and srv.ChassisCode = con.ChassisCode
		   and srv.ChassisNo = con.ChassisNo
		 where pkg.CompanyCode = b.CompanyCode
		   and pkg.JobType = b.JobType
		   and prt.PartNo = a.PartNo
		), 
		(case when rtrim(a.BillType) = 'F' and rtrim(e.ParaValue) = 'SPAREPART' then 0
		      when rtrim(a.BillType) = 'F' then 0
		      when rtrim(e.ParaValue) = 'SPAREPART' then (select top 1 PartDiscPct from #srv) 
			  else (select top 1 MaterialDiscPct from #srv) end)
		) as DiscPct
  from #part a
  left join #task b
    on b.CompanyCode = a.CompanyCode
   and b.ProductType = a.ProductType
   and b.BasicModel  = a.BasicModel
   and b.JobType     = a.JobType
   and b.OperationNo = a.OperationNo
   LEFT join @tblTemp d
   on d.CompanyCode = a.CompanyCode
   and d.BranchCode = @BranchCode
   and d.PartNo = a.PartNo
  --left join spMstItemPrice c
  --  on c.CompanyCode = a.CompanyCode
  -- and c.BranchCode  = @BranchCode
  -- and c.PartNo      = a.PartNo
  --left join spMstItems d
  --  on d.CompanyCode = a.CompanyCode
  -- and d.BranchCode  = @BranchCode
  -- and d.PartNo      = a.PartNo
  left join gnMstLookupDtl e
    on e.CompanyCode = d.CompanyCode
   and e.CodeID = 'GTGO'
   and e.LookupValue = d.TypeOfGoods
   left join spMstItemInfo f
	on f.CompanyCode = a.CompanyCode
	and f.PartNo = a.PartNo
 where 1 = 1
   and b.CompanyCode = a.CompanyCode
   and b.ProductType = a.ProductType
   and b.BasicModel  = a.BasicModel
   and b.JobType     = a.JobType
   and b.OperationNo = a.OperationNo
exec uspfn_SvInsertDefaultTaskMovement @CompanyCode, @BranchCode, @ProductType, @ServiceNo, @UserID

drop table #srv
drop table #task
drop table #part
drop table #job
GO
if object_id('usprpt_OmFakturPajak') is not null
	drop procedure usprpt_OmFakturPajak
GO
create procedure [dbo].[usprpt_OmFakturPajak]
--DECLARE
	@CompanyCode	VARCHAR(15),
	@BranchCode	VARCHAR(15),
	@FPJDateStart DATETIME,
	@FPJDateEnd DATETIME,
	@FPJNoStart	VARCHAR(30),
	@FPJNoEnd	VARCHAR(30),
	@SignName VARCHAR(100),
	@TitleSign VARCHAR(100),
	@Param bit = 1
AS
BEGIN

--'6114201','611420100','20150501','20150530','010.001-15.70827239','010.001-15.70827239','Wiwik W','Pimpinan FAD',True

--SELECT @CompanyCode = '6114201',@BranchCode='611420100',@FPJDateStart='20150501',@FPJDateEnd='20150630',@FPJNoStart='010.001-15.70827992',
--@FPJNoEnd='010.001-15.70827992',@SignName='Wiwik W',@TitleSign='Pimpinan FAD',@Param=1

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
	declare @fInv		int
	
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
set @fInv		= (select isnull(ParaValue,'1') from gnmstlookupdtl where codeid='FPJFLAG' and lookupvalue='SALES')		
-- parameter use address holding or not
declare @IsHoldingAddr as bit
if (select count(*) from gnMstLookUpDtl where CompanyCode= @CompanyCode and CodeID='FPJADDR') > 0
	set @IsHoldingAddr = (select convert(numeric,LookUpValue) from gnMstLookUpDtl where CompanyCode= @CompanyCode and CodeID='FPJADDR')
else
	set @IsHoldingAddr = 0

-- parameter to show info or not
declare @IsShowInfo as bit
if (select count(*) from gnMstLookUpDtl where CompanyCode= @CompanyCode and CodeID='FPIF' and LookUpValue='STATUS') > 0
	set @IsShowInfo = (select convert(numeric,ParaValue) from gnMstLookUpDtl where CompanyCode= @CompanyCode and CodeID='FPIF' and LookUpValue='STATUS')
else
	set @IsShowInfo = 1
	
	
	declare @tabData as table
	(
		CompanyCode varchar(max),
		BranchCode varchar(max),
		InvoiceNo varchar(max),
		ItemOrder varchar(max),
		ItemCode varchar(max),
		ItemName varchar(max),
		PPnBM decimal(18,2),
		PPnBMSell decimal(18,2),
		ItemQuantity decimal(5,2),
		ItemDPP decimal(18,2),
		Potongan decimal(18,2),
		TaxPct decimal(18,2),
		AfterDiscPpn  decimal(18,2)
	)
	IF (@Param=1)
	BEGIN
		-- Sembunyikan Detail Part .....
		SELECT * INTO #t1 FROM (
		SELECT 
			acc.CompanyCode
			, acc.BranchCode
			, acc.InvoiceNo
			, acc.PartNo AS ItemCode
			, ISNULL((acc.Quantity)/(select count(chassisno) from omFakturPajakDetail where companycode=acc.companycode 
						and branchcode=acc.branchcode and invoiceno=acc.invoiceno) * acc.RetailPrice,0) AS ItemDPP
			, ISNULL((acc.Quantity)/(select count(chassisno) from omFakturPajakDetail where companycode=acc.companycode 
						and branchcode=acc.branchcode and invoiceno=acc.invoiceno) * acc.DiscExcludePPn, 0) AS Potongan
			, 0 AS TaxPct
		FROM 
			omFakturPajakDetailAccsSeq acc 
		INNER JOIN omFakturPajakhdr hdr ON hdr.CompanyCode = acc.CompanyCode
			AND hdr.BranchCode = acc.BranchCode
			AND hdr.InvoiceNo = acc.InvoiceNo
			AND ((CASE WHEN @FPJNoStart = '' THEN hdr.FakturPajakNo END) <> ''
				OR (CASE WHEN @FPJNoStart <> '' THEN hdr.FakturPajakNo END) BETWEEN @FPJNoStart AND @FPJNoEnd)
			AND CONVERT(VARCHAR, hdr.FakturPajakDate, 112) BETWEEN CONVERT(VARCHAR, @FPJDateStart, 112) AND CONVERT(VARCHAR, @FPJDateEnd, 112)
			AND hdr.TaxType = 'Standard'
		WHERE 
			acc.CompanyCode = @CompanyCode AND acc.BranchCode = @BranchCode	)#t1
				
		select * into #Others from(
				Select a.CompanyCode
					 , a.BranchCode
					 , a.InvoiceNo
					 , a.SalesModelCode
					 , ISNULL((a.Quantity)/(select count(chassisno) from omFakturPajakDetail where companycode=a.companycode 
						and branchcode=a.branchcode and invoiceno=a.invoiceno) * a.DPP, 0) ItemDPP
					 , 0 Potongan
				from omFakturPajakDetailOthers a
				left join omFakturPajakDetail b on a.CompanyCode = b.CompanyCode
					  and a.BranchCode = b.BranchCode
					  and a.InvoiceNo = b.InvoiceNo
					  and a.SalesModelCode = b.SalesModelCode
				INNER JOIN omFakturPajakhdr hdr ON hdr.CompanyCode = a.CompanyCode
					   AND hdr.BranchCode = a.BranchCode
					   AND hdr.InvoiceNo = a.InvoiceNo
					   AND ((CASE WHEN @FPJNoStart = '' THEN hdr.FakturPajakNo END) <> ''
					   	   OR (CASE WHEN @FPJNoStart <> '' THEN hdr.FakturPajakNo END) BETWEEN @FPJNoStart AND @FPJNoEnd)
					   AND CONVERT(VARCHAR, hdr.FakturPajakDate, 112) BETWEEN CONVERT(VARCHAR, @FPJDateStart, 112) AND CONVERT(VARCHAR, @FPJDateEnd, 112)
					   AND hdr.TaxType = 'Standard'
				where a.CompanyCode = @CompanyCode
				  and a.BranchCode = @BranchCode
		)#Others
		
		--SELECT * INTO #t2 FROM (
		--SELECT
		--	CompanyCode 
		--	, BranchCode
		--	, InvoiceNo 
		--	, SUM(ItemDPP) ItemDPP
		--	, SUM(Potongan) Potongan
		--FROM #t1 GROUP BY CompanyCode, BranchCode, InvoiceNo ) #t2
		
		SELECT CompanyCode 
				, BranchCode
				, InvoiceNo 
				, SUM(ItemDPP) ItemDPP
				, SUM(Potongan) Potongan
		INTO #t2 FROM (
			SELECT
				CompanyCode 
				, BranchCode
				, InvoiceNo 
				, SUM(ItemDPP) ItemDPP
				, SUM(Potongan) Potongan
			FROM #t1 GROUP BY CompanyCode, BranchCode, InvoiceNo
		UNION
			SELECT
				CompanyCode 
				, BranchCode
				, InvoiceNo 
				, SUM(ItemDPP) ItemDPP
				, SUM(Potongan) Potongan
			FROM #Others a
			GROUP BY a.CompanyCode, a.BranchCode, a.InvoiceNo 
		) #t2
		group by CompanyCode,BranchCode,InvoiceNo
		
		-- INSER DATA KE TABEL RESULT
		INSERT INTO @tabData 
		-- Unit
		SELECT 
			mdl.CompanyCode
			, mdl.BranchCode
			, mdl.InvoiceNo
			, '1' AS ItemOrder
			, mdl.SalesModelCode AS ItemCode
			, LEFT(CONVERT(VARCHAR, mdl.ChassisNo) + '             ', 13) +  LEFT(CONVERT(VARCHAR, mdl.EngineNo) + '           ', 11) AS ItemName
			, ISNULL(Vec.PPnBMBuyPaid,0) AS PPnBM
			, mdl.AfterDiscPPnBM AS PPnBMSell
			, 1 AS ItemQuantity
			, mdl.BeforeDiscDPP + (ISNULL(t.ItemDPP,0)) AS ItemDPP
			, mdl.DiscExcludePPN + (ISNULL(t.Potongan,0)) AS Potongan
			, ISNULL((SELECT ISNULL(TaxPct, 0) FROM  GnMstTax INNER JOIN OmMstModel ON OmMstModel.CompanyCode = GnMstTax.CompanyCode AND OmMstModel.PPnBMCodeSell = GnMstTax.TaxCode AND OmMstModel.SalesModelCode = mdl.SalesModelCode), 0) AS TaxPct
			, mdl.AfterDiscPpn		
		FROM 
			omFakturPajakDetail mdl 
		INNER JOIN omFakturPajakhdr hdr ON hdr.CompanyCode = mdl.CompanyCode
			AND hdr.BranchCode = mdl.BranchCode
			AND hdr.InvoiceNo = mdl.InvoiceNo
			AND ((CASE WHEN @FPJNoStart = '' THEN hdr.FakturPajakNo END) <> ''
				OR (CASE WHEN @FPJNoStart <> '' THEN hdr.FakturPajakNo END) BETWEEN @FPJNoStart AND @FPJNoEnd)
			AND CONVERT(VARCHAR, hdr.FakturPajakDate, 112) BETWEEN CONVERT(VARCHAR, @FPJDateStart, 112) AND CONVERT(VARCHAR, @FPJDateEnd, 112)
			AND hdr.TaxType = 'Standard'
		LEFT JOIN OmMstVehicle vec ON mdl.CompanyCode = vec.CompanyCode 
			AND mdl.ChassisCode = vec.ChassisCode 
			AND mdl.ChassisNo = vec.ChassisNo
		LEFT JOIN #t2 t ON t.CompanyCode = mdl.CompanyCode
			AND t.BranchCode = mdl.BranchCode
			AND t.InvoiceNo = mdl.InvoiceNo
		WHERE 
			mdl.CompanyCode = @CompanyCode AND mdl.BranchCode = @BranchCode 
		UNION ALL
		SELECT distinct
			hdr.CompanyCode
			, hdr.BranchCode
			, hdr.InvoiceNo
			, '2' AS ItemOrder
			, 'SPAREPART/MATERIAL' AS ItemCode
			, '' AS ItemName
			, 0 AS PPnBM
			, 0 AS PPnBMSell
			, 0 AS ItemQuantity
			, 0 AS ItemDPP
			, 0 AS Potongan
			, 0 AS TaxPct
			, 0 AS AfterDiscPpn
		FROM omFakturPajakDetailAccsSeq acc
		INNER JOIN omFakturPajakHdr hdr ON hdr.CompanyCode=acc.CompanyCode
			AND hdr.BranchCode=acc.BranchCode
			AND hdr.InvoiceNo=acc.InvoiceNo
		WHERE hdr.CompanyCode = @CompanyCode
			AND hdr.BranchCode = @BranchCode
			AND ((CASE WHEN @FPJNoStart = '' THEN hdr.FakturPajakNo END) <> ''
				OR (CASE WHEN @FPJNoStart <> '' THEN hdr.FakturPajakNo END) BETWEEN @FPJNoStart AND @FPJNoEnd)
			AND CONVERT(VARCHAR, hdr.FakturPajakDate, 112) BETWEEN CONVERT(VARCHAR, @FPJDateStart, 112) AND CONVERT(VARCHAR, @FPJDateEnd, 112)
			AND hdr.TaxType = 'Standard'
		UNION ALL
		-- SPAREPART/MATERIAL
		SELECT 
			acc.CompanyCode
			, acc.BranchCode
			, acc.InvoiceNo
			, '3' AS ItemOrder
			, acc.PartNo AS ItemCode
			, acc.PartName AS ItemName
			, 0 AS PPnBM
			, 0 AS PPnBMSell
			, 0 AS ItemQuantity
			, 0 AS ItemDPP
			, 0 AS Potongan
			, 0 AS TaxPct
			, 0 AS AfterDiscPpn
		FROM 
			omFakturPajakDetailAccsSeq acc 
		INNER JOIN omFakturPajakhdr hdr ON hdr.CompanyCode = acc.CompanyCode
			AND hdr.BranchCode = acc.BranchCode
			AND hdr.InvoiceNo = acc.InvoiceNo
			AND ((CASE WHEN @FPJNoStart = '' THEN hdr.FakturPajakNo END) <> ''
				OR (CASE WHEN @FPJNoStart <> '' THEN hdr.FakturPajakNo END) BETWEEN @FPJNoStart AND @FPJNoEnd)
			AND CONVERT(VARCHAR, hdr.FakturPajakDate, 112) BETWEEN CONVERT(VARCHAR, @FPJDateStart, 112) AND CONVERT(VARCHAR, @FPJDateEnd, 112)
			AND hdr.TaxType = 'Standard'
		WHERE 
			acc.CompanyCode = @CompanyCode AND acc.BranchCode = @BranchCode	
		UNION ALL
		SELECT 
			oth.CompanyCode
			, oth.BranchCode
			, oth.InvoiceNo
			, '4' AS ItemOrder
			, oth.OtherCode AS ItemCode
			, oth.OtherName AS ItemName
			, 0 AS PPnBM
			, 0 AS PPnBMSell
			, 0 AS ItemQuantity
			, 0 AS ItemDPP
			, 0 AS Potongan
			, 0 AS TaxPct
			, 0 AS AfterDiscPpn			
		FROM 
			omFakturPajakDetailOthers oth 
		INNER JOIN omFakturPajakhdr hdr ON hdr.CompanyCode = oth.CompanyCode
			AND hdr.BranchCode = oth.BranchCode
			AND hdr.InvoiceNo = oth.InvoiceNo
			AND ((CASE WHEN @FPJNoStart = '' THEN hdr.FakturPajakNo END) <> ''
				OR (CASE WHEN @FPJNoStart <> '' THEN hdr.FakturPajakNo END) BETWEEN @FPJNoStart AND @FPJNoEnd)
			AND CONVERT(VARCHAR, hdr.FakturPajakDate, 112) BETWEEN CONVERT(VARCHAR, @FPJDateStart, 112) AND CONVERT(VARCHAR, @FPJDateEnd, 112)
			AND hdr.TaxType = 'Standard'
		WHERE 
			oth.CompanyCode = @CompanyCode AND oth.BranchCode = @BranchCode	
							 
		DROP TABLE #t1
		DROP TABLE #t2			
	END
	ELSE
	BEGIN
		-- Tampilkan Part --
		--------------------
		
		-- INSER DATA KE TABEL RESULT
		INSERT INTO @tabData 
		-- Unit
		SELECT 
			mdl.CompanyCode
			, mdl.BranchCode
			, mdl.InvoiceNo
			, '1' AS ItemOrder
			, mdl.SalesModelCode AS ItemCode
			, LEFT(CONVERT(VARCHAR, mdl.ChassisNo) + '             ', 13) +  LEFT(CONVERT(VARCHAR, mdl.EngineNo) + '           ', 11) AS ItemName
			, ISNULL(Vec.PPnBMBuyPaid,0) AS PPnBM
			, mdl.AfterDiscPPnBM AS PPnBMSell
			, 1 AS ItemQuantity
			, mdl.BeforeDiscDPP AS ItemDPP
			, mdl.DiscExcludePPN Potongan
			, ISNULL((SELECT ISNULL(TaxPct, 0) FROM  GnMstTax INNER JOIN OmMstModel ON OmMstModel.CompanyCode = GnMstTax.CompanyCode AND OmMstModel.PPnBMCodeSell = GnMstTax.TaxCode AND OmMstModel.SalesModelCode = mdl.SalesModelCode), 0) AS TaxPct
			, mdl.AfterDiscPpn
		FROM 
			omFakturPajakDetail mdl 
		INNER JOIN omFakturPajakhdr hdr ON hdr.CompanyCode = mdl.CompanyCode
			AND hdr.BranchCode = mdl.BranchCode
			AND hdr.InvoiceNo = mdl.InvoiceNo
			AND ((CASE WHEN @FPJNoStart = '' THEN hdr.FakturPajakNo END) <> ''
				OR (CASE WHEN @FPJNoStart <> '' THEN hdr.FakturPajakNo END) BETWEEN @FPJNoStart AND @FPJNoEnd)
			AND CONVERT(VARCHAR, hdr.FakturPajakDate, 112) BETWEEN CONVERT(VARCHAR, @FPJDateStart, 112) AND CONVERT(VARCHAR, @FPJDateEnd, 112)
			AND hdr.TaxType = 'Standard'
		LEFT JOIN OmMstVehicle vec ON mdl.CompanyCode = vec.CompanyCode 
			AND mdl.ChassisCode = vec.ChassisCode 
			AND mdl.ChassisNo = vec.ChassisNo
		WHERE 
			mdl.CompanyCode = @CompanyCode AND mdl.BranchCode = @BranchCode 
		UNION ALL
		SELECT distinct
			hdr.CompanyCode
			, hdr.BranchCode
			, hdr.InvoiceNo
			, '2' AS ItemOrder
			, 'SPAREPART/MATERIAL' AS ItemCode
			, '' AS ItemName
			, 0 AS PPnBM
			, 0 AS PPnBMSell
			, 0 AS ItemQuantity
			, 0 AS ItemDPP
			, 0 AS Potongan
			, 0 AS TaxPct
			, 0 AS AfterDiscPpn
		FROM omFakturPajakDetailAccsSeq acc
		INNER JOIN omFakturPajakHdr hdr ON hdr.CompanyCode=acc.CompanyCode
			AND hdr.BranchCode=acc.BranchCode
			AND hdr.InvoiceNo=acc.InvoiceNo
		WHERE hdr.CompanyCode = @CompanyCode
			AND hdr.BranchCode = @BranchCode
			AND ((CASE WHEN @FPJNoStart = '' THEN hdr.FakturPajakNo END) <> ''
				OR (CASE WHEN @FPJNoStart <> '' THEN hdr.FakturPajakNo END) BETWEEN @FPJNoStart AND @FPJNoEnd)
			AND CONVERT(VARCHAR, hdr.FakturPajakDate, 112) BETWEEN CONVERT(VARCHAR, @FPJDateStart, 112) AND CONVERT(VARCHAR, @FPJDateEnd, 112)
			AND hdr.TaxType = 'Standard'
		UNION ALL
		-- SPAREPART/MATERIAL
		SELECT 
			acc.CompanyCode
			, acc.BranchCode
			, acc.InvoiceNo
			, '3' AS ItemOrder
			, acc.PartNo AS ItemCode
			, acc.PartName AS ItemName
			, 0 AS PPnBM
			, 0 AS PPnBMSell
			, acc.Quantity AS ItemQuantity
			, (acc.Quantity * acc.RetailPrice) AS ItemDPP
			, (acc.Quantity * acc.DiscExcludePPn) AS Potongan
			, 0 AS TaxPct
			, 0 AS AfterDiscPpn			
		FROM 
			omFakturPajakDetailAccsSeq acc 
		INNER JOIN omFakturPajakhdr hdr ON hdr.CompanyCode = acc.CompanyCode
			AND hdr.BranchCode = acc.BranchCode
			AND hdr.InvoiceNo = acc.InvoiceNo
			AND ((CASE WHEN @FPJNoStart = '' THEN hdr.FakturPajakNo END) <> ''
				OR (CASE WHEN @FPJNoStart <> '' THEN hdr.FakturPajakNo END) BETWEEN @FPJNoStart AND @FPJNoEnd)
			AND CONVERT(VARCHAR, hdr.FakturPajakDate, 112) BETWEEN CONVERT(VARCHAR, @FPJDateStart, 112) AND CONVERT(VARCHAR, @FPJDateEnd, 112)
			AND hdr.TaxType = 'Standard'
		WHERE 
			acc.CompanyCode = @CompanyCode AND acc.BranchCode = @BranchCode	
		UNION ALL
		SELECT 
			oth.CompanyCode
			, oth.BranchCode
			, oth.InvoiceNo
			, '4' AS ItemOrder
			, oth.OtherCode AS ItemCode
			, oth.OtherName AS ItemName
			, 0 AS PPnBM
			, 0 AS PPnBMSell
			, oth.Quantity AS ItemQuantity
			, (oth.Quantity * oth.DPP) AS ItemDPP
			, 0 AS Potongan
			, 0 AS TaxPct
			, 0 AS AfterDiscPpn			
		FROM 
			omFakturPajakDetailOthers oth 
		INNER JOIN omFakturPajakhdr hdr ON hdr.CompanyCode = oth.CompanyCode
			AND hdr.BranchCode = oth.BranchCode
			AND hdr.InvoiceNo = oth.InvoiceNo
			AND ((CASE WHEN @FPJNoStart = '' THEN hdr.FakturPajakNo END) <> ''
				OR (CASE WHEN @FPJNoStart <> '' THEN hdr.FakturPajakNo END) BETWEEN @FPJNoStart AND @FPJNoEnd)
			AND CONVERT(VARCHAR, hdr.FakturPajakDate, 112) BETWEEN CONVERT(VARCHAR, @FPJDateStart, 112) AND CONVERT(VARCHAR, @FPJDateEnd, 112)
			AND hdr.TaxType = 'Standard'
		WHERE 
			oth.CompanyCode = @CompanyCode AND oth.BranchCode = @BranchCode	
	END

SELECT * INTO #hasil FROM (
SELECT
	a.TaxType AS TaxType
	,a.InvoiceNo AS InvoiceNo
	,a.InvoiceDate AS InvoiceDate
	,a.FakturPajakNo AS FPJNo
	,(SELECT dbo.GetDateIndonesian (CONVERT(VARCHAR,a.FakturPajakDate, 101))) AS FPJDate
	,a.CustomerCode AS fakturCustCode
	,case when @fInfoPKP = 1 then
		(case @fStatus when '1' then @fCompName else e.CompanyGovName end)
	 else '' end AS CompanyName
	,case @fStatus when '1' then @fSKP else e.SKPNo end AS compSKPNo
	,case @fStatus when '1' then @fSKPDate else e.SKPDate end AS compSKPDate
	,case when @fInfoPKP = 1 then
		(case @fStatus when '1' then @fAdd else 
			(case when @IsHoldingAddr=0 then ISNULL(e.Address1,'') + ' ' + ISNULL(e.Address2,'') + ' ' + ISNULL(e.Address3,'') + ' ' + ISNULL(e.Address4,'')
				else (select ISNULL(Address1,'') + ' ' + ISNULL(Address2,'') + ' ' + ISNULL(Address3,'') + ' ' + ISNULL(Address4,'') from gnMstCoProfile where CompanyCode=@CompanyCode
				and BranchCode=(select BranchCode from gnMstOrganizationDtl where CompanyCode=@CompanyCode and IsBranch=0))end ) end)
		 else '' end AS compAddr	 
	,e.PhoneNo AS compPhoneNo
	,e.FaxNo AS compFaxNo
	,case when @fInfoPKP = 1 then
		(case @fStatus when '1' then @fNPWP else e.NPWPNo end)
	 else '' end AS compNPWPNo
	,d.CustomerGovName AS CustomerName
	,d.SKPNo AS custSKPNo
	,d.SKPDate AS custSKPDate
	,ISNULL(d.Address1,'') AS custAddr1
	,ISNULL(d.Address2,'') AS custAddr2
	,ISNULL(d.Address3,'')+ ' ' + ISNULL(d.Address4,'') AS custAddr3
	,d.PhoneNo AS custPhoneNo
	,d.FaxNo AS custFaxNo
	,d.NPWPNo AS custNPWPNo
	,a.DueDate AS fakturDueDate
	,a.DiscAmt AS DiscAmt
	,a.DppAmt AS DppAmt
	,a.PPNAmt AS PPNAmt
	,a.DppAmt - a.DiscAmt AS SubAmt
	,a.TotalAmt AS TotalAmt
	,a.TotQuantity
	,a.PPnBMPaid
	,case @fStatus when '1' then @fCity else 
		(SELECT LookUpValueName FROM gnMstLookupDtl WHERE CodeID = 'CITY' AND LookUpValue = e.CityCode) end as cityNm
	,ISNULL(@SignName, '') AS TaxPerson
	,ISNULL(@TitleSign,'') AS JobTitle
	,'Model              No.Rangka    No.Mesin            PPnBM' AS ItemHeader
	,dtl.ItemOrder
	,dtl.ItemCode
	,dtl.ItemName
	,dtl.PPnBM
	,dtl.ItemQuantity
	,dtl.ItemDPP
	,dtl.Potongan
	,dtl.AfterDiscPpn
	,CASE WHEN copro.ProductType = '2W' THEN 
		(CASE WHEN so.SalesType = '0' THEN 
			(SELECT LookupValuename FROM gnMstLookUpDtl WHERE CodeID = 'FPJN' AND  LookupValue = '2WWS') 
				ELSE (SELECT LookupValuename FROM gnMstLookUpDtl WHERE CodeID = 'FPJN' and  LookupValue = '2WDS') END) 
		ELSE 
		(CASE WHEN so.SalesType = '0' THEN 
			(SELECT LookupValuename FROM gnMstLookUpDtl WHERE CodeID = 'FPJN' AND  LookupValue = '4WWS') 
				ELSE (SELECT LookupValuename FROM gnMstLookUpDtl WHERE CodeID = 'FPJN' and  LookupValue = '4WDS') END)  
	END AS Keterangan
	,@IsShowInfo as FlagShowInfo
	,@param HidePart
	,dtl.TaxPct
	,dtl.PPnBMSell
FROM 
	omFakturPajakHdr a
LEFT JOIN GnMstCustomer d ON a.CompanyCode = d.CompanyCode 
	AND a.CustomerCode = d.CustomerCode
LEFT JOIN GnMstCoProfile e ON a.CompanyCode = e.CompanyCode 
	AND a.BranchCode = e.BranchCode
INNER JOIN @tabData dtl ON dtl.CompanyCode = a.CompanyCode
	AND dtl.BranchCode = a.BranchCode
	AND dtl.InvoiceNo = a.InvoiceNo
LEFT JOIN omTrSalesInvoice inv ON inv.CompanyCode = a.CompanyCode 
	AND inv.BranchCode = a.BranchCode 
	AND inv.InvoiceNo = a.InvoiceNo
LEFT JOIN omTrSalesSO so	ON a.CompanyCode = so.CompanyCode
	AND a.BranchCode = so.BranchCode 
	AND inv.SONo = so.SONo
LEFT JOIN GnMstCoProfile copro ON a.CompanyCode = copro.CompanyCode 
	AND a.BranchCode = copro.BranchCode
WHERE  
	a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode ) #hasil

if (@param=1)
	BEGIN
	SELECT TaxType, InvoiceNo, InvoiceDate, FPJNo, FPJDate, fakturCustCode, CompanyName, compSKPNo, compSKPDate, compAddr, compPhoneNo, compFaxNo, compNPWPNo
		, CustomerName, custSKPNo, custSKPDate, custAddr1, custAddr2, custAddr3, custPhoneNo, custFaxNo, custNPWPNo, fakturDueDate, DiscAmt, DPPAmt
		--, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select round(((sum(ItemDPP) - sum(Potongan)) * 0.1),0) from #hasil where FPJNo = c.FPJNo) ELSE PPNAmt END PPNAmt
		, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select sum(AfterDiscPpn) from #hasil where FPJNo = c.FPJNo) ELSE PPNAmt END PPNAmt
		, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select sum(ItemDPP) - sum(Potongan) from #hasil where FPJNo = c.FPJNo ) ELSE SubAmt END SubAmt
		--, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select  (sum(ItemDPP) - sum(Potongan)) + round(((sum(ItemDPP) - sum(Potongan)) * 0.1),0) from #hasil where FPJNo = c.FPJNo ) ELSE TotalAmt END TotalAmt
		--, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select  (sum(ItemDPP) - sum(Potongan)) + sum(AfterDiscPpn) from #hasil where FPJNo = c.FPJNo ) ELSE TotalAmt END TotalAmt
		, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select sum(TotalAmt) from #hasil where FPJNo = c.FPJNo ) ELSE TotalAmt END TotalAmt
		, TotQuantity, PPNBMPaid, cityNm, TaxPerson, JobTitle, ItemHeader, ItemOrder, ItemCode, ItemName, PPnBM, ItemQuantity, ItemDPP, Potongan, Keterangan
		, FlagShowInfo, HidePart, TaxPct, PPnBMSell
		, (SELECT COUNT(INVOICENO) FROM #hasil WHERE FPJNo = c.FPJNo) MaxRow
		, (select sum(ItemQuantity) from #hasil where FPJNo=c.FPJNo and ItemOrder='1') SumQty
		, (select sum(ItemDPP) from #hasil where FPJNo = c.FPJNo ) XAmt
		, (select sum(Potongan) from #hasil where FPJNo = c.FPJNo ) XPotongan
		, @fInv ViewInvoice
	FROM #hasil c order by c.InvoiceNo, c.ItemOrder
	END
else
	SELECT TaxType, InvoiceNo, InvoiceDate, FPJNo, FPJDate, fakturCustCode, CompanyName, compSKPNo, compSKPDate, compAddr, compPhoneNo, compFaxNo, compNPWPNo
		, CustomerName, custSKPNo, custSKPDate, custAddr1, custAddr2, custAddr3, custPhoneNo, custFaxNo, custNPWPNo, fakturDueDate, DiscAmt, DPPAmt
		--, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select round(((sum(ItemDPP) - sum(Potongan)) * 0.1),0) from #hasil where FPJNo = c.FPJNo) ELSE PPNAmt END PPNAmt
		, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select sum(AfterDiscPpn) from #hasil where FPJNo = c.FPJNo) ELSE PPNAmt END PPNAmt
		, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select sum(ItemDPP) - sum(Potongan) from #hasil where FPJNo = c.FPJNo ) ELSE SubAmt END SubAmt
		--, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select  (sum(ItemDPP) - sum(Potongan)) + round(((sum(ItemDPP) - sum(Potongan)) * 0.1),0) from #hasil where FPJNo = c.FPJNo ) ELSE TotalAmt END TotalAmt
		--, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select  (sum(ItemDPP) - sum(Potongan)) + sum(AfterDiscPpn) from #hasil where FPJNo = c.FPJNo ) ELSE TotalAmt END TotalAmt
		, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select sum(TotalAmt) from #hasil where FPJNo = c.FPJNo ) ELSE TotalAmt END TotalAmt
		, TotQuantity, PPNBMPaid, cityNm, TaxPerson, JobTitle, ItemHeader, ItemOrder, ItemCode, ItemName, PPnBM, ItemQuantity, ItemDPP, Potongan, Keterangan
		, FlagShowInfo, HidePart, TaxPct, PPnBMSell
		, (SELECT COUNT(INVOICENO) FROM #hasil WHERE FPJNo = c.FPJNo) MaxRow
		, (select sum(ItemQuantity) from #hasil where FPJNo=c.FPJNo) SumQty
		, (select sum(ItemDPP) from #hasil where FPJNo = c.FPJNo ) XAmt
		, (select sum(Potongan) from #hasil where FPJNo = c.FPJNo ) XPotongan
		, @fInv ViewInvoice
	FROM #hasil c order by c.InvoiceNo, c.ItemOrder

DROP TABLE #hasil, #Others--, #t1
END
GO
if object_id('uspfn_GenerateSSPickingslipNew') is not null
	drop procedure uspfn_GenerateSSPickingslipNew
GO
create procedure [dbo].[uspfn_GenerateSSPickingslipNew]
	@CompanyCode	VARCHAR(MAX),
	@BranchCode		VARCHAR(MAX),
	@JobOrderNo		VARCHAR(MAX),
	@ProductType	VARCHAR(MAX),
	@CustomerCode	VARCHAR(MAX),
	@TransType		VARCHAR(MAX),
	@UserID			VARCHAR(MAX),
	@DocDate		DATETIME
AS
BEGIN

--declare	@CompanyCode	VARCHAR(MAX)
--declare	@BranchCode		VARCHAR(MAX)
--declare	@JobOrderNo		VARCHAR(MAX)
--declare	@ProductType	VARCHAR(MAX)
--declare	@CustomerCode	VARCHAR(MAX)
--declare	@TransType		VARCHAR(MAX)
--declare	@UserID			VARCHAR(MAX)
--declare	@DocDate		DATETIME

--set	@CompanyCode	= '6156401000'
--set	@BranchCode		= '6156401001'
--set	@JobOrderNo		= 'SPK/15/001833'
--set	@ProductType	= '4W'
--set	@CustomerCode	= '000003'
--set	@TransType		= '20'
--set	@UserID			= 'TRAININGZZZ'
--set	@DocDate		= '3/12/2015 9:47:01 AM'


--exec uspfn_GenerateSSPickingslipNew '6006400001','6006400101','SPK/14/101589','4W','2105885','20','ga','3/2/2015 4:03:01 PM'
--================================================================================================================================
-- TABLE MASTER
--================================================================================================================================
-- Temporary for Item --
------------------------
SELECT * INTO #Item FROM (
SELECT a.CompanyCode
	, a.BranchCode
	, a.ProductType
	, a.ServiceNo
	, a.RetailPrice
	, a.PartNo
	, a.Billtype
	, SUM(ISNULL(a.DemandQty, 0) - (ISNULL(a.SupplyQty, 0))) QtyOrder
FROM svTrnSrvItem a WITH (NOLOCK, NOWAIT)
INNER JOIN svTrnService b ON b.CompanyCode = a.CompanyCode
	AND b.BranchCode = a.BranchCode
	AND b.ProductType = a.ProductType
	AND b.ServiceNo = a.ServiceNo
	AND b.JobOrderNo = @JobOrderNo
WHERE a.CompanyCode = @CompanyCode 
	AND a.BranchCode = @BranchCode 
	AND a.ProductType = @ProductType 
GROUP BY a.CompanyCode, a.BranchCode, a.ProductType
	, a.ServiceNo, a.PartNo, a.RetailPrice, a.BillType ) #Item 

DECLARE @CompanyMD AS VARCHAR(15)
DECLARE @BranchMD AS VARCHAR(15)
DECLARE @WarehouseMD AS VARCHAR(15)

SET @CompanyMD = (SELECT CompanyMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)
SET @BranchMD = (SELECT BranchMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)
SET @WarehouseMD = (SELECT WarehouseMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)

SELECT * INTO #SrvOrder FROM (
SELECT DISTINCT(a.CompanyCode) 
    , a.BranchCode
	, a.ProductType
	, a.ServiceNo
	, a.PartNo
    , (SELECT Paravalue FROM gnMstLookUpDtl WHERE CompanyCode = a.CompanyCode AND CodeID = 'GTGO' AND LookUpValue = a.TypeOfGoods) TipePart
    , (SELECT PartName FROM spMstItemInfo WHERE CompanyCode = a.CompanyCode AND PartNo = a.PartNo) PartName
	, a.RetailPrice
	, a.CostPrice
    , a.TypeOfGoods
    , a.BillType
	, SUM(a.QtyOrder) QtyOrder
    , 0 QtySupply
    , 0 QtyBO
    , (SUM(a.QtyOrder) * a.RetailPrice) * ((100 - a.PartDiscPct)/100) NetSalesAmt
    , a.PartDiscPct DiscPct
FROM
(
	SELECT
		DISTINCT(a.CompanyCode) 
		, a.BranchCode
		, a.ProductType
		, a.ServiceNo
		, a.PartNo
		, a.RetailPrice
		, a.CostPrice
		, a.TypeOfGoods
		, a.BillType
		, ISNULL(Item.QtyOrder,0) AS QtyOrder
		, a.DiscPct PartDiscPct 
	FROM
		svTrnSrvItem a WITH (NOLOCK, NOWAIT)
		LEFT JOIN svTrnService b WITH (NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode
			AND a.BranchCode = b.BranchCode	
			AND a.ProductType = b.ProductType
			AND a.ServiceNo = b.ServiceNo
		LEFT JOIN #Item Item ON Item.CompanyCode = a.CompanyCode 
			AND Item.BranchCode = a.BranchCode 
			AND Item.ProductType = a.ProductType 
			AND Item.ServiceNo = a.ServiceNo 
			AND Item.PartNo = a.PartNo 
			AND Item.RetailPrice = a.RetailPrice 
			AND Item.BillType = a.Billtype
		--LEFT JOIN SpMstItemPrice c WITH (NOLOCK, NOWAIT) ON a.CompanyCode = c.CompanyCode 
		--	AND a.BranchCode = c.BranchCode 
		--	AND a.PartNo = c.PartNo
	WHERE
		a.CompanyCode = @CompanyCode
		AND a.BranchCode = @BranchCode
		AND a.ProductType = @ProductType
		AND Item.QtyOrder > 0
		AND JobOrderNo = @JobOrderNo
		AND (a.SupplySlipNo is null OR a.SupplySlipNo = '')
) a
GROUP BY
	a.CompanyCode
	, a.BranchCode
	, a.ProductType
	, a.ServiceNo
	, a.PartNo
	, a.RetailPrice
	, a.CostPrice
    , a.TypeOfGoods
    , a.BillType
    , a.PartDiscPct 
) #SrvOrder

select * from #srvorder

--================================================================================================================================
-- INSERT TABLE SpTrnSORDHdr AND SpTrnSORDDtl
--================================================================================================================================
DECLARE @MaxDocNo			INT
DECLARE	@MaxPickingList		INT
DECLARE @TempDocNo			VARCHAR(MAX)
DECLARE @TempPickingList	VARCHAR(MAX)
DECLARE @TypeOfGoods		VARCHAR(MAX)
DECLARE @DefaultDate		DATETIME

SET @DefaultDate = '1900-01-01 00:00:00.000'

--===============================================================================================================================
-- LOOPING BASED ON THE TYPE OF GOODS
-- ==============================================================================================================================
DECLARE db_cursor CURSOR FOR
SELECT DISTINCT TypeOfGoods FROM #SrvOrder
WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND ProductType = @ProductType 

OPEN db_cursor
FETCH NEXT FROM db_cursor INTO @TypeOfGoods

WHILE @@FETCH_STATUS = 0
BEGIN

--===============================================================================================================================
-- INSERT HEADER
-- ==============================================================================================================================
SET @MaxDocNo = ISNULL((SELECT DocumentSequence + 1 FROM GnMstDocument WHERE 
	CompanyCode = @CompanyCode
		AND BranchCode = @BranchCode
		AND DocumentType = 'SSS' 
		AND ProfitCenterCode = '300' 
		AND DocumentYear = YEAR(GetDate())),0)

SET @TempDocNo = ISNULL((SELECT 'SSS/' + RIGHT(YEAR(GETDATE()),2) +'/' + RIGHT(1000000 + CONVERT(VARCHAR, @MaxDocNo, 1), 6)),'SSS/YY/XXXXXX')

INSERT INTO SpTrnSORDHdr
([CompanyCode]
           ,[BranchCode]
           ,[DocNo]
           ,[DocDate]
           ,[UsageDocNo]
           ,[UsageDocDate]
           ,[CustomerCode]
           ,[CustomerCodeBill]
           ,[CustomerCodeShip]
           ,[isBO]
           ,[isSubstitution]
           ,[isIncludePPN]
           ,[TransType]
           ,[SalesType]
           ,[IsPORDD]
           ,[OrderNo]
           ,[OrderDate]
           ,[TOPCode]
           ,[TOPDays]
           ,[PaymentCode]
           ,[PaymentRefNo]
           ,[TotSalesQty]
           ,[TotSalesAmt]
           ,[TotDiscAmt]
           ,[TotDPPAmt]
           ,[TotPPNAmt]
           ,[TotFinalSalesAmt]
           ,[isPKP]
           ,[ExPickingSlipNo]
           ,[ExPickingSlipDate]
           ,[Status]
           ,[PrintSeq]
           ,[TypeOfGoods]
           ,[isDropsign]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastUpdateBy]
           ,[LastUpdateDate]
           ,[isLocked]
           ,[LockingBy]
           ,[LockingDate])

SELECT 
	@CompanyCode CompanyCode
	, @BranchCode BranchCode
	, @TempDocNo DocNo 
	, @DocDate DocDate
	, @JobOrderNo UsageDocNo
	, (SELECT JobOrderDate FROM SvTrnService WHERE 1 =1 AND CompanyCode = @CompanyCode AND BranchCode = @BranchCode 
		AND ProductType = @ProductType AND JobOrderNo = @JobOrderNo) UsageDocDate
	, (SELECT CustomerCode FROM SvTrnService WHERE 1 = 1AND CompanyCode = @CompanyCode AND BranchCode = @BranchCode 
		AND ProductType = @ProductType AND JobOrderNo = @JobOrderNo) CustomerCode
	, (SELECT CustomerCodeBill FROM SvTrnService WHERE 1 = 1 AND CompanyCode = @CompanyCode AND BranchCode = @BranchCode 
		AND ProductType = @ProductType AND JobOrderNo = @JobOrderNo) CustomerCodeBill
	, (SELECT CustomerCode FROM SvTrnService WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode 
		AND ProductType = @ProductType AND JobOrderNo = @JobOrderNo) CustomerCodeShip
	, CONVERT(BIT, 0) isBO
	, CONVERT(BIT, 0) isSubstitution
	, CONVERT(BIT, 1) isIncludePPN
	, @TransType TransType
	, '2' SalesType
	, CONVERT(BIT, 0) isPORDD
	, @JobOrderNo OrderNo
	, @DocDate OrderDate
	, ISNULL((SELECT TOPCode FROM GnMstCustomerProfitCenter WITH (NOLOCK, NOWAIT) WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
		AND ProfitCenterCode = '300' AND CustomerCode = @CustomerCode),'W') TOPCode
	, ISNULL((SELECT ParaValue FROM GnMstLookUpDtl WITH (NOLOCK, NOWAIT) WHERE CompanyCode = @CompanyCode AND CodeID = 'TOPC' AND 
		LookupValue IN 
		(SELECT TOPCode FROM GnMstCustomerProfitCenter WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
			AND ProfitCenterCode = '300' AND CustomerCode = @CustomerCode)
	  ),0) TOPDays
	, ISNULL((SELECT PaymentCode FROM GnMstCustomerProfitCenter WITH (NOLOCK, NOWAIT) WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
		AND ProfitCenterCode = '300' AND CustomerCode = @CustomerCode),'W') PaymentCode
	, '' PaymentReffNo
	, 0 TotSalesQty
	, 0 TotSalesAmt
	, 0 TotDiscAmt
	, 0 TotDPPAmt
	, 0 TotPPNAmt
	, 0 TotFinalSalesAmt
	, CONVERT(BIT, 0) isPKP
	, NULL ExPickingSlipNo
	, NULL ExPickingSlipDate
	, '4' Status
	, 0 PrintSeq
	, @TypeOfGoods TypeOfGoods
	, NULL IsDropSign
	, @UserID CreatedBY
	, GetDate() CreatedDate
	, @UserID LastUpdateBy
	, GetDate() LastUpdateDate
	, NULL isLocked
	, NULL LockingBy
	, NULL LockingDate


UPDATE GnMstDocument
SET DocumentSequence = DocumentSequence + 1
	, LastUpdateDate = GetDate()
	, LastUpdateBy = @UserID
WHERE
	CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND DocumentPrefix = 'SSS'
	AND ProfitCenterCode = '300'
	AND DocumentYear = Year(GetDate())

--===============================================================================================================================
-- INSERT DETAIL
-- ==============================================================================================================================
DECLARE @DbMD AS VARCHAR(15)
SET @DbMD = (SELECT DbMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)

declare @TempAvailStock as table
(
	PartNo varchar(50),
	AvailStock decimal
)

DECLARE @Query AS VARCHAR(MAX)
--SET @Query = 
--'SELECT PartNo, (Onhand - AllocationSP - AllocationSL - AllocationSR - ReservedSP - ReservedSL - ReservedSR) AvailStock
--FROM ' + @DbMD + '..SpMstItemLoc WITH (NOLOCK, NOWAIT) 
--WHERE CompanyCode = '+''''+@CompanyMD+''''+' AND BranchCode ='+''''+@BranchMD +''''+' AND WarehouseCode = '+''''+@WarehouseMD+''''+''

--INSERT INTO #TempAvailStock

SET @Query = 
'SElect * into #TempAvailStock from (SELECT PartNo, (Onhand - AllocationSP - AllocationSL - AllocationSR - ReservedSP - ReservedSL - ReservedSR) AvailStock
FROM ' + @DbMD + '..SpMstItemLoc WITH (NOLOCK, NOWAIT) 
WHERE CompanyCode = '+''''+@CompanyMD+''''+' AND BranchCode ='+''''+@BranchMD +''''+' AND WarehouseCode = '+''''+@WarehouseMD+''''+')#TempAvailStock

INSERT INTO SpTrnSORDDtl 
(
	[CompanyCode] ,
	[BranchCode] ,
	[DocNo] ,
	[PartNo] ,
	[WarehouseCode] ,
	[PartNoOriginal] ,
	[ReferenceNo] ,
	[ReferenceDate] ,
	[LocationCode] ,
	[QtyOrder] ,
	[QtySupply] ,
	[QtyBO] ,
	[QtyBOSupply] ,
	[QtyBOCancel] ,
	[QtyBill] ,
	[RetailPriceInclTax] ,
	[RetailPrice] ,
	[CostPrice] ,
	[DiscPct] ,
	[SalesAmt] ,
	[DiscAmt] ,
	[NetSalesAmt] ,
	[PPNAmt] ,
	[TotSalesAmt] ,
	[MovingCode] ,
	[ABCClass] ,
	[ProductType] ,
	[PartCategory] ,
	[Status] ,
	[CreatedBy] ,
	[CreatedDate] ,
	[LastUpdateBy] ,
	[LastUpdateDate] ,
	[StockAllocatedBy] ,
	[StockAllocatedDate] ,
	[FirstDemandQty] )
SELECT
	''' + @CompanyCode +''' CompanyCode
	, ''' + @BranchCode +''' BranchCode
	, ''' + @TempDocNo +''' DocNo 
	, a.PartNo
	, ''00'' WarehouseCode
	, a.PartNo PartNoOriginal
	, ''' + @JobOrderNo +''' ReferenceNo
	, (SELECT JobOrderDate FROM SvTrnService WHERE 1 =1 AND CompanyCode = ''' + @CompanyCode +''' AND BranchCode = ''' + @BranchCode +'''
		AND ProductType = ''' + @ProductType +''' AND JobOrderNo = ''' + @JobOrderNo +''' ) ReferenceDate
	, (SELECT distinct LocationCode FROM ' + @DbMD +'..SpMstItemLoc WHERE CompanyCode = ''' + @CompanyMD +''' AND BranchCode = ''' + @BranchMD +''' AND WarehouseCode = ''00''
		AND PartNo = a.PartNo ) LocationCode
	, a.QtyOrder
	, CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN a.QtyOrder 
		ELSE ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0)
		END AS QtySupply
	, CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN 0 
		ELSE a.QtyOrder - ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0)
		END AS QtyBO
	, 0 QtyBOSupply
	, CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN 0 
		ELSE a.QtyOrder - ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0)
		END AS QtyBOCancel
	, 0 QtyBill
	, a.RetailPrice + FLOOR(a.RetailPrice * 10 /100) RetailPriceIncltax
	, a.RetailPrice
	, a.CostPrice
	, a.DiscPct
	, CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN a.QtyOrder * a.RetailPrice
		ELSE ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) * a.RetailPrice
		END AS SalesAmt
	, CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN floor((a.QtyOrder * a.RetailPrice) * a.DiscPct/100)
		ELSE floor((ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) * a.RetailPrice) * a.DiscPct/100)
		END AS DiscAmt
	, CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN floor((a.QtyOrder * a.RetailPrice)- ((a.QtyOrder * a.RetailPrice) * a.DiscPct/100))
		ELSE floor((ISNULL((SELECT AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) * a.RetailPrice) - 
			(ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) * a.RetailPrice * a.DiscPct/100))
		END AS NetSalesAmt
	, 0 PPNAmt
	,  CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN floor((a.QtyOrder * a.RetailPrice)- ((a.QtyOrder * a.RetailPrice) * a.DiscPct/100))
		ELSE floor((ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) * a.RetailPrice) - 
			(ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) * a.RetailPrice * a.DiscPct/100))
		END AS TotSalesAmt
	, (SELECT distinct MovingCode FROM '+ @DbMD +'..SpMstItems WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyMD +''' AND BranchCode = '''+ @BranchMD +''' AND PartNo = a.PartNo) 
		MovingCode
	, (SELECT distinct ABCClass FROM '+ @DbMD +'..SpMstItems WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyMD +''' AND BranchCode = '''+ @BranchMD +''' AND PartNo = a.PartNo) 
		ABCClass
	, '''+ @ProductType +''' ProductType
	, (SELECT distinct PartCategory FROM '+ @DbMD +'..SpMstItems WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyMD +'''  AND BranchCode = '''+ @BranchMD +''' AND PartNo = a.PartNo) 
		PartCategory
	, ''2'' Status
	, '''+ @UserID +''' CreatedBy
	, '''+ Convert(varchar,GetDate()) +''' CreatedDate
	, '''+ @UserID +''' LastUpdateBy
	, '''+ Convert(varchar,GetDate()) +''' LastUpdateDate
	, '''+ @UserID +''' StockAllocatedBy
	, '''+ Convert(varchar,GetDate()) +''' StockAllocatedDate
	, a.QtyOrder FirstDemandQty
FROM #SrvOrder a
WHERE a.TypeOfGoods = '+@TypeOfGoods +'


select top 10 * from spTrnSORDDtl order by createddate desc
--===============================================================================================================================
-- INSERT SO SUPPLY
-- ==============================================================================================================================

SELECT * INTO #TempSOSupply FROM (
SELECT
	'''+ @CompanyCode +''' CompanyCode
	, '''+ @BranchCode +''' BranchCode
	, '''+ @TempDocNo +''' DocNo 
	, 0 SupSeq
	, a.PartNo 
	, a.PartNo PartNoOriginal
	, '''' PickingSlipNo	
	, '''+ @JobOrderNo +''' ReferenceNo
	, '''+ CONVERT(varchar, @DefaultDate )+''' ReferenceDate
	, ''00'' WarehouseCode
	, (SELECT distinct LocationCode FROM '+ @DbMD+'..SpMstItemLoc WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyMD +''' AND BranchCode = '''+ @BranchMD+''' AND WarehouseCode = ''00''
		AND PartNo = a.PartNo) LocationCode
	, CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN a.QtyOrder 
		ELSE ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0)
		END AS QtySupply
	, 0 QtyPicked
	, 0 QtyBill
	, a.RetailPrice + FLOOR(a.RetailPrice *10 /100) RetailPriceIncltax
	, a.RetailPrice
	, b.CostPrice
	, a.DiscPct
	, (SELECT distinct MovingCode FROM '+ @DbMD+'..SpMstItems WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyMD +''' AND BranchCode = '''+ @BranchMD +''' AND PartNo = a.PartNo) 
		MovingCode
	, (SELECT distinct ABCClass FROM '+ @DbMD +'..SpMstItems WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyMD +''' AND BranchCode = '''+ @BranchMD +''' AND PartNo = a.PartNo) 
		ABCClass
	, '''+ @ProductType +'''ProductType
	, (SELECT distinct PartCategory FROM '+ @DbMD +'..SpMstItems WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyMD +''' AND BranchCode = '''+ @BranchMD +''' AND PartNo = a.PartNo) 
		PartCategory
	, ''1'' Status
	, '''+ @UserID +''' CreatedBy
	, '''+ Convert(varchar,GetDate()) +''' CreatedDate
	, '''+ @UserID +''' LastUpdateBy
	, '''+ Convert(varchar,GetDate()) +''' LastUpdateDate
	, '''+ @UserID +''' StockAllocatedBy
	, '''+ Convert(varchar,GetDate()) +''' StockAllocatedDate
FROM #SrvOrder a
WHERE a.CompanyCode = '''+ @CompanyCode +''' AND a.BranchCode = '''+ @BranchCode +''' 
and a.TypeOfGoods = '+ @TypeOfGoods +'
)#TempSOSupply

INSERT INTO SpTrnSOSupply SELECT 
	CompanyCode,BranchCode,DocNo,SupSeq,PartNo,PartNoOriginal
	, ROW_NUMBER() OVER(ORDER BY PartNo) PTSeq,PickingSlipNo
	, ReferenceNo,ReferenceDate,WarehouseCode,LocationCode
	, QtySupply,QtyPicked,QtyBill,RetailPriceIncltax,RetailPrice,CostPrice
	, DiscPct,MovingCode,ABCClass,ProductType,PartCategory,Status
	, CreatedBy,CreatedDate,LastUpdateBy,LastUpdateDate
FROM #TempSOSupply WHERE QtySupply > 0

--===============================================================================================================================
-- UPDATE STATUS DETAIL BASED ON SUPPLY
--===============================================================================================================================

UPDATE SpTrnSORDDtl
SET Status = 4
FROM SpTrnSORDDtl a, #TempSOSupply b
WHERE 1 = 1
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.PartNo = b.PartNo
	
--===============================================================================================================================
-- UPDATE HISTORY DEMAND ITEM AND CUSTOMER
--===============================================================================================================================

UPDATE SpHstDemandItem 
SET DemandFreq = DemandFreq + 1
	, DemandQty = DemandQty + b.QtyOrder
	, LastUpdateBy = '''+ @UserID +'''
	, LastUpdateDate = '''+ Convert(varchar,GetDate()) +''' 
FROM SpHstDemandItem a, SpTrnSordDtl b
WHERE 
	1 = 1
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.Year = '''+ Convert(varchar,Year(GetDate())) +''' 
	AND a.Month  = '''+ Convert(varchar,Month(GetDate())) +''' 
	AND a.PartNo = b.PartNo
	AND b.DocNo = '''+ @TempDocNo +'''

UPDATE SpHstDemandCust
SET DemandFreq = DemandFreq + 1
	, DemandQty = DemandQty + b.QtyOrder
	, LastUpdateBy = '''+ @UserID +''' 
	, LastUpdateDate = '''+ Convert(varchar,GetDate()) +''' 
FROM SpHstDemandCust a, SpTrnSordDtl b
WHERE 
	1 = 1
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.Year = '''+ Convert(varchar,Year(GetDate())) +'''
	AND a.Month  = '''+ Convert(varchar,Month(GetDate())) +'''
	AND a.PartNo = b.PartNo
	AND a.CustomerCode = '''+ @CustomerCode +'''
	AND b.DocNo = '''+ @TempDocNo +'''

INSERT INTO SpHstDemandItem
SELECT 
	CompanyCode
	, BranchCode
	, '''+ Convert(varchar,Year(GetDate())) +''' Year
	, '''+ Convert(varchar,Month(GetDate())) +''' Month
	, PartNo
	, 1 DemandFreq
	, QtyOrder DemandQty
	, 0 SalesFreq
	, 0 SalesQty
	, MovingCode
	, ProductType
	, PartCategory
	, ABCClass
	, '''+ @UserID +''' LastUpdateBy
	, '''+ CONVERT(varchar, GetDate()) +''' LastUpdateDate
FROM SpTrnSordDtl a WITH (NOLOCK, NOWAIT)
WHERE a.CompanyCode= '''+ @CompanyCode +''' AND a.BranchCode = '''+ @BranchCode +''' AND a.DocNo = '''+ @TempDocNo +''' -- add CompanyCode and BranchCode 13 Des 2010
 AND NOT EXISTS
( SELECT 1 FROM SpHstDemandItem WITH (NOLOCK, NOWAIT) WHERE 
	1 = 1 
	AND CompanyCode = a.CompanyCode 
	AND BranchCode = a.BranchCode
	AND Month = '''+ Convert(varchar,Month(GetDate())) +''' 
	AND Year = '''+ Convert(varchar,Year(GetDate())) +''' 
	AND PartNo = a.PartNo
)

INSERT INTO SpHstDemandCust
SELECT 
	CompanyCode
	, BranchCode
	, '''+ Convert(varchar,Year(GetDate())) +'''  Year
	, '''+ Convert(varchar,Month(GetDate())) +'''  Month
	, '''+ @CustomerCode +''' CustomerCode
	, PartNo
	, 1 DemandFreq
	, (SELECT QtyOrder FROM SpTrnSORDDTl WITH (NOLOCK, NOWAIT) 
		WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode 
		AND DocNo = a.DocNo AND PartNo = a.PartNo) DemandQty
	, 0 SalesFreq
	, 0 SalesQty
	, MovingCode
	, ProductType
	, PartCategory
	, ABCClass
	, '''+ @UserID +''' LastUpdateBy
	, '''+ CONVERT(varchar, GetDate()) +''' LastUpdateDate
FROM SpTrnSordDtl a WITH (NOLOCK, NOWAIT)
WHERE a.CompanyCode= '''+ @CompanyCode +''' and a.BranchCode= '''+ @BranchCode +''' AND a.DocNo = '''+ @TempDocNo +''' -- add CompanyCode and BranchCode 13 Des 2010
AND NOT EXISTS
( SELECT PartNo FROM SpHstDemandCust WITH (NOLOCK, NOWAIT) WHERE 
	1 = 1 
	AND CompanyCode = a.CompanyCode 
	AND BranchCode = a.BranchCode
	AND Month = '''+ Convert(varchar,Month(GetDate())) +''' 
	AND Year = '''+ Convert(varchar,Year(GetDate())) +'''  
	AND PartNo = a.PartNo
)

--===============================================================================================================================
-- UPDATE LAST DEMAND DATE MASTER
--===============================================================================================================================

UPDATE '+@DbMD+'..SpMstItems 
SET LastDemandDate = '''+Convert(varchar,GetDate())+'''
FROM '+@DbMD+'..SpMstItems a, SpTrnSordDtl b
WHERE 
	1 = 1
	AND a.CompanyCode = '''+@CompanyMD+'''
	AND a.BranchCode = '''+@BranchMD+'''
	AND a.PartNo = b.PartNo
	AND b.DocNo = '''+@TempDocNo+'''

--===============================================================================================================================
-- UPDATE STOCK AND MOVEMENT
--===============================================================================================================================

UPDATE '+@DbMD+'..spMstItems
SET AllocationSR = AllocationSR + b.QtySupply
	, LastUpdateBy = '''+@UserID+'''
	, LastUpdateDate = '''+Convert(varchar,GetDate())+'''
FROM '+@DbMD+'..SpMstItems a, #TempSOSupply b
WHERE 
	1 = 1
	AND a.CompanyCode = '''+@CompanyMD+'''
	AND a.BranchCode = '''+@BranchMD+'''
	AND a.PartNo = b.PartNo

UPDATE '+@DbMD+'..spMstItemloc
SET AllocationSR = AllocationSR + b.QtySupply
	, LastUpdateBy = '''+@UserID+'''
	, LastUpdateDate = '''+Convert(varchar,GetDate())+'''
FROM '+@DbMD+'..SpMstItemLoc a, #TempSOSupply b
WHERE 
	1 = 1
	AND a.CompanyCode = '''+@CompanyMD +'''
	AND a.BranchCode = '''+@BranchMD+'''
	AND a.WarehouseCode = '''+@WarehouseMD+'''
	AND a.PartNo = b.PartNo

INSERT INTO SpTrnIMovement
SELECT
	'''+@CompanyCode +''' CompanyCode
	, '''+@BranchCode +''' BranchCode
	, a.DocNo
	, (SELECT DocDate FROM SpTrnSORDHdr WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyCode +'''
		AND BranchCode = '''+ @BranchCode +''' AND DocNo = a.DocNo) 
	  DocDate
	, dateadd(s,ROW_NUMBER() OVER(Order by a.PartNo),'''+convert(varchar,getdate())+''') CreatedDate 
	, ''00'' WarehouseCode
	, (SELECT LocationCode FROM SpTrnSORDDtl WITH (NOLOCK, NOWAIT) WHERE CompanyCode =  '''+@CompanyCode +'''
		AND BranchCode = '''+@BranchCode +''' AND DocNo = '''+@TempDocNo +''' AND PartNo = a.PartNo)
	  LocationCode
	, a.PartNo
	, ''OUT'' SignCode
	, ''SA-NPJUAL'' SubSignCode
	, a.QtySupply
	, a.RetailPrice
	, a.CostPrice
	, a.ABCClass
	, a.MovingCode
	, a.ProductType
	, a.PartCategory
	, '''+@UserID +''' CreatedBy
FROM #TempSOSupply a

--===============================================================================================================================
-- UPDATE SUPPLY SLIP TO SPK
--===============================================================================================================================
DECLARE @ServiceNo VARCHAR(MAX)

SET @ServiceNo = (SELECT ServiceNo FROM svTrnService WHERE CompanyCode = '''+@CompanyCode +''' AND BranchCode = '''+@BranchCode+'''
		AND ProductType = '''+@ProductType +''' AND JobOrderNo = '''+@JobOrderNo+''')

SELECT * INTO #TempServiceItem FROM (
SELECT 
	a.CompanyCode
	, a.BranchCode
	, a.ProductType
	, a.ServiceNo
	, a.PartNo
	, a.PartSeq
	, a.DemandQty
	, a.SupplyQty
	, b.QtySupply
	, b.DocNo
	, a.CostPrice
	, a.RetailPrice
	, a.TypeOfGoods
	, a.BillType
FROM SvTrnSrvItem a WITH (NOLOCK, NOWAIT)
INNER JOIN #TempSOSupply b WITH (NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.PartNo = b.PartNo
WHERE
	1 = 1
	AND a.CompanyCode = '''+@CompanyCode+'''
	AND a.BranchCode = '''+@BranchCode+'''
	AND a.ProductType = '''+@ProductType+'''
	AND a.ServiceNo = @ServiceNo
	AND a.PartSeq IN (SELECT MAX(PartSeq) FROM SvTrnSrvItem WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode
		AND ProductType = '''+@ProductType +''' AND ServiceNo = a.ServiceNo AND PartNo = a.PartNo)
	AND (a.SupplySlipNo = '''' OR a.SupplySlipNo IS NULL)
) #TempServiceItem 

SELECT * INTO #TempServiceItemIns FROM( 
SELECT 
	a.CompanyCode
	, a.BranchCode
	, a.ProductType
	, a.ServiceNo
	, a.PartNo
	, a.PartSeq
	, a.DemandQty
	, a.SupplyQty
	, b.QtySupply
	, b.DocNo
	, a.CostPrice
	, a.RetailPrice
	, a.TypeOfGoods
	, a.BillType
	, a.DiscPct
FROM SvTrnSrvItem a WITH (NOLOCK, NOWAIT)
INNER JOIN #TempSOSupply b WITH (NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.PartNo = b.PartNo
WHERE
	1 = 1 
	AND a.CompanyCode = '''+ @CompanyCode +''' 
	AND a.BranchCode = '''+ @BranchCode +''' 
	AND a.ProductType = '''+ @ProductType +'''  
	AND a.ServiceNo = @ServiceNo
	AND a.PartSeq IN (SELECT MAX(PartSeq) FROM SvTrnSrvItem WITH (NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode
		AND ProductType = '''+ @ProductType +''' AND ServiceNo = a.ServiceNo AND PartNo = a.PartNo) 
	AND (a.SupplySlipNo != '''' OR a.SupplySlipNo IS NOT NULL)
) #TempServiceItemIns


UPDATE svTrnSrvItem
SET SupplySlipNo = b.DocNo
	, SupplySlipDate = ISNULL((SELECT DocDate FROM SpTrnSORDHdr WITH (NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode
							AND DocNo = b.DocNo),'''+Convert(varchar,@DefaultDate)+''')
FROM svTrnSrvItem a, #TempServiceItem b
WHERE
	1 = 1
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.ProductType = b.ProductType
	AND a.ServiceNo = b.ServiceNo
	AND a.PartNo = b.PartNo
	AND a.PartSeq = b.PartSeq
	
--===============================================================================================================================
-- INSERT NEW SRV ITEM BASED SUPPLY SLIP
--===============================================================================================================================
INSERT INTO SvTrnSrvItem (CompanyCode, BranchCode, ProductType, ServiceNo, PartNo, PartSeq, DemandQty, SupplyQty, ReturnQty, CostPrice, RetailPrice, TypeOfGoods, BillType, SupplySlipNo, SupplySlipDate, SSReturnNo, SSReturnDate, CreatedBy, CreatedDate, LastupdateBy, LastupdateDate, DiscPct)
SELECT 
	a.CompanyCode
	, a.BranchCode
	, a.ProductType
	, a.ServiceNo
	, a.PartNo
	, a.PartSeq + 1
	, 0 DemandQty
	, 0 SupplyQty
	, 0 ReturnQty
	, a.CostPrice
	, a.RetailPrice
	, a.TypeOfGoods
	, a.BillType
	, a.DocNo SupplySlipNo
	, (SELECT DocDate FROM SpTrnSORDHdr WITH (NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode
		AND DocNo = a.DocNo) SupplySlipDate
	, NULL SSReturnNo
	, NULL SSReturnDate
	, '''+ @UserID +''' CreatedBy
	, '''+ Convert(varchar,GetDate()) +''' CreatedDate
	, '''+ @UserID +''' LastUpdateBy
	, '''+ Convert(varchar,GetDate()) +''' LastUpdateDate
	, a.DiscPct
FROM #TempServiceItemIns a WITH (NOLOCK, NOWAIT)
WHERE
	1 = 1
	AND a.CompanyCode = '''+ @CompanyCode +'''
	AND a.BranchCode = '''+ @BranchCode +'''
	AND a.ProductType = '''+ @ProductType+'''


--===============================================================================================================================
DROP TABLE #TempServiceItem 
DROP TABLE #TempServiceItemIns
DROP TABLE #TempSOSupply'

EXEC(@query)

--select convert(xml,@query)


--===============================================================================================================================
-- INSERT SVSDMOVEMENT
--===============================================================================================================================

declare @md bit
set @md = (select case WHEN EXISTS(select * from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode and CompanyMD = @CompanyCode and BranchMD = @BranchCode) then cast(1 as bit) ELSE cast(0 as bit) END)

if(@md = 0)
begin

 declare @QueryTemp as varchar(max)  
 
	set @Query ='insert into '+ @DbMD +'..svSDMovement
	select a.CompanyCode, a.BranchCode, a.DocNo, a.CreatedDate, a.PartNo
	, Seq = convert(integer, ROW_NUMBER() OVER (PARTITION BY a.ReferenceNo ORDER BY a.DocNo)) ,
	a.WarehouseCode, a.QtyOrder, a.QtySupply, a.DiscPct
	--,isnull(((select RetailPrice from spTrnSORDDtl
	--		where CompanyCode = ''' + @CompanyCode + '''  and BranchCode = ''' + @BranchCode  + '''
	--		and DocNo = ''' + @TempDocNo + ''' and PartNo = a.PartNo) / 1.1 * 
	--		((100 - isnull((select isnull(DiscPct, 0) from gnMstSupplierProfitCenter   
	--			where CompanyCode = ''' + @CompanyCode + ''' and BranchCode = ''' + @BranchCode  + ''' and SupplierCode = dbo.GetBranchMD(''' + @CompanyCode + ''', ''' + @BranchCode  + ''') 
	--			and ProfitCenterCode = ''300''),0)) * 0.01)),0) 
	, a.CostPrice
	, a.RetailPrice, b.TypeOfGoods
	, '''+ @CompanyMD +''','''+ @BranchMD +''','''+ @WarehouseMD +''', md.RetailPriceInclTax, md.RetailPrice, md.CostPrice
	,''x'','''+ @producttype +''',''300'',''0'',''0'','''+ convert(varchar,GETDATE()) +''','''+ @UserID +''','''+ convert(varchar,GETDATE()) +'''
	,'''+ @UserID +''','''+ convert(varchar,GETDATE()) +'''
	from spTrnSORDDtl a 
	join spTrnSORDHdr b on a.CompanyCode = b.CompanyCode
	and a.BranchCode = b.BranchCode 
	and a.DocNo = b.DocNo
	join '+ @DbMD +'..spmstitemprice md
	on md.CompanyCode = '''+ @CompanyMD +'''
	and md.branchcode = '''+ @BranchMD +'''
	and md.PartNo = a.PartNo
	where a.ReferenceNo = '''+ @JobOrderNo +''''+
	' and a.DocNo = ''' + @TempDocNo + '''';

	exec (@Query)
	print (@QUERY)

end

--===============================================================================================================================
-- INSERT PICKING HEADER AND DETAIL
--===============================================================================================================================

SET @MaxPickingList = ISNULL((SELECT DocumentSequence + 1 FROM GnMstDocument WHERE 
	CompanyCode = @CompanyCode
		AND BranchCode = @BranchCode
		AND DocumentType = 'PLS' 
		AND ProfitCenterCode = '300' 
		AND DocumentYear = YEAR(GetDate())),0)

SET @TempPickingList = ISNULL((SELECT 'PLS/' + RIGHT(YEAR(GETDATE()),2) +'/' + RIGHT(1000000 + CONVERT(VARCHAR, @MaxPickingList, 1), 6)),'PLS/YY/XXXXXX')

INSERT INTO SpTrnSPickingHdr 
SELECT 
	CompanyCode
	, BranchCode
	, @TempPickingList PickingSlipNo
	, GetDate() PickingSlipDate
	, CustomerCode
	, CustomerCodeBill
	, CustomerCodeShip
	, '' PickedBy
	, CONVERT(BIT, 0) isBORelease
	, isSubstitution
	, isIncludePPN
	, TransType
	, SalesType
	, TotSalesQty
	, TotSalesAmt
	, TotDiscAmt
	, TotDPPAmt
	, TotPPNAmt
	, TotFinalSalesAmt
	, '' Remark
	, '0' Status
	, '0' PrintSeq
	, TypeOfGoods
	, CreatedBy
	, CreatedDate
	, LastUpdateBy
	, LastUpdateDate
	, NULL isLocked
	, NULL LockingBy
	, NULL LockingDate
FROM SpTrnSORDHdr WITH (NOLOCK, NOWAIT)
WHERE 
	1 = 1
	AND CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND DocNo = (SELECT distinct DocNo FROM spTrnSORDDtl WHERE CompanyCode = @CompanyCode AND Branchcode = @BranchCode 
					AND DocNo = @TempDocNo AND QtySupply > 0)		

UPDATE GnMstDocument
SET DocumentSequence = DocumentSequence + 1
	, LastUpdateDate = GetDate()
	, LastUpdateBy = @UserID
WHERE
	1 = 1
	AND CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND DocumentPrefix = 'PLS'
	AND ProfitCenterCode = '300'
	AND DocumentYear = Year(GetDate())

-- ==============================================================================================================================
-- UPDATE SALES ORDER HEADER 
-- ==============================================================================================================================
UPDATE SpTrnSORDHdr
	SET ExPickingSlipNo = @TempPickingList,
		ExPickingSlipDate = ISNULL((SELECT PickingSlipDate FROM SpTrnSPickingHdr WHERE CompanyCode = @CompanyCode
				AND BranchCode = @BranchCode AND PickingSlipNo = @TempPickingList),'')
	
WHERE
	1 = 1
	AND CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND DocNo = @TempDocNo

UPDATE SpTrnSOSupply
	SET PickingSlipNo = @TempPickingList
WHERE
	1 = 1
	AND CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND DocNo = @TempDocNo
-- ==============================================================================================================================
-- INSERT PICKING DETAIL
-- ==============================================================================================================================

INSERT INTO SpTrnSPickingDtl
SELECT 
	a.CompanyCode
	, a.BranchCode
	, @TempPickingList PickingSlipNo
	, '00' WarehouseCode
	, a.PartNo
	, a.PartNoOriginal
	, a.DocNo
	, b.DocDate 
	, a.ReferenceNo
	, a.ReferenceDate
	, a.LocationCode
	, a.QtySupply QtyOrder
	, a.QtySupply
	, a.QtySupply QtyPicked 
	, 0 QtyBill
	, a.RetailPriceInclTax
	, a.RetailPrice
	, a.CostPrice
	, a.DiscPct
	, a.SalesAmt
	, a.DiscAmt
	, a.NetSalesAmt
	, a.TotSalesAmt
	, a.MovingCode
	, a.ABCClass
	, a.ProductType
	, a.PartCategory
	, '' ExPickingSlipNo
	, @DefaultDate ExPickingSlipDate
	, CONVERT(BIT, 0) isClosed
	, @UserID CreatedBy
	, GetDate() CreatedDate
	, @UserID LastUpdateBy
	, GetDate() LastUpdateDate
FROM SpTrnSORDDtl a WITH (NOLOCK, NOWAIT)
INNER JOIN SpTrnSORDHdr b WITH (NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.DocNo = b.DocNo
WHERE
	1 = 1
	AND a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND a.DocNo = @TempDocNo
	AND a.QtySupply > 0


--================================================================================================================================
-- UPDATE AMOUNT HEADER
--================================================================================================================================
SELECT * INTO #TempHeader FROM (
SELECT 
	header.CompanyCode
	, header.BranchCode
	, header.DocNo
	, header.TotSalesQty
	, header.TotSalesAmt
	, header.TotDiscAmt
	, header.TotDPPAmt
	, floor(header.TotDPPAmt * (ISNULL((SELECT TaxPct FROM GnMstTax WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
			AND TaxCode IN (SELECT TaxCode FROM GnMstCustomerProfitCenter WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
			AND ProfitCenterCode = '300' AND CustomerCode = @CustomerCode)),0)/100)) 
		TotPPNAmt
	, header.TotDPPAmt + floor(header.TotDPPAmt * (ISNULL((SELECT TaxPct FROM GnMstTax WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
			AND TaxCode IN (SELECT TaxCode FROM GnMstCustomerProfitCenter WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
			AND ProfitCenterCode = '300' AND CustomerCode = @CustomerCode)),0)/100))
		TotFinalSalesAmt
FROM (
SELECT 
	CompanyCode
	, BranchCode
	, DocNo
	, SUM(QtyOrder) TotSalesQty
	, SUM(SalesAmt) TotSalesAmt
	, SUM(DiscAmt) TotDiscAmt
	, SUM(NetSalesAmt) TotDPPAmt
FROM SpTrnSORDDtl WITH (NOLOCK, NOWAIT)
WHERE CompanyCode = @CompanyCode 
	AND BranchCode = @BranchCode
	AND DocNo = @TempDocNo
GROUP BY CompanyCode
	, BranchCode
	, DocNo
) header ) #TempHeader

UPDATE SpTrnSORDHdr
SET 
	TotSalesQty = b.TotSalesQty
	, TotSalesAmt = b.TotSalesAmt
	, TotDiscAmt = b.TotDiscAmt
	, TotDPPAmt = b.TotDPPAmt
	, TotPPNAmt = b.TotPPNAmt
	, TotFinalSalesAmt = b.TotFinalSalesAmt
FROM SpTrnSORDHdr a, #TempHeader b
WHERE 
	1 = 1
	AND a.CompanyCode = b.CompanyCode 
	AND a.BranchCode = b.BranchCode
	AND a.DocNo = b.DocNo

DROP TABLE #TempHeader

FETCH NEXT FROM db_cursor INTO @TypeOfGoods
END
CLOSE db_cursor
DEALLOCATE db_cursor 

--===============================================================================================================================
-- Update Transdate
--===============================================================================================================================

update gnMstCoProfileSpare
set TransDate=getdate()
	, LastUpdateBy=@UserID
	, LastUpdateDate=getdate()
where CompanyCode = @CompanyCode AND BranchCode = @BranchCode

--===============================================================================================================================
-- DROP TABLE SECTION 
--===============================================================================================================================
DROP TABLE #SrvOrder
DROP TABLE #Item

--rollback tran
END
go
if object_id('uspfn_GenerateSSPickingslipNew') is not null
	drop procedure uspfn_GenerateSSPickingslipNew
GO

create procedure [dbo].[uspfn_GenerateSSPickingslipNew]
	@CompanyCode	VARCHAR(MAX),
	@BranchCode		VARCHAR(MAX),
	@JobOrderNo		VARCHAR(MAX),
	@ProductType	VARCHAR(MAX),
	@CustomerCode	VARCHAR(MAX),
	@TransType		VARCHAR(MAX),
	@UserID			VARCHAR(MAX),
	@DocDate		DATETIME
AS
BEGIN

--declare	@CompanyCode	VARCHAR(MAX)
--declare	@BranchCode		VARCHAR(MAX)
--declare	@JobOrderNo		VARCHAR(MAX)
--declare	@ProductType	VARCHAR(MAX)
--declare	@CustomerCode	VARCHAR(MAX)
--declare	@TransType		VARCHAR(MAX)
--declare	@UserID			VARCHAR(MAX)
--declare	@DocDate		DATETIME

--set	@CompanyCode	= '6156401000'
--set	@BranchCode		= '6156401001'
--set	@JobOrderNo		= 'SPK/15/001833'
--set	@ProductType	= '4W'
--set	@CustomerCode	= '000003'
--set	@TransType		= '20'
--set	@UserID			= 'TRAININGZZZ'
--set	@DocDate		= '3/12/2015 9:47:01 AM'


--exec uspfn_GenerateSSPickingslipNew '6006400001','6006400101','SPK/14/101589','4W','2105885','20','ga','3/2/2015 4:03:01 PM'
--================================================================================================================================
-- TABLE MASTER
--================================================================================================================================
-- Temporary for Item --
------------------------
SELECT * INTO #Item FROM (
SELECT a.CompanyCode
	, a.BranchCode
	, a.ProductType
	, a.ServiceNo
	, a.RetailPrice
	, a.PartNo
	, a.Billtype
	, SUM(ISNULL(a.DemandQty, 0) - (ISNULL(a.SupplyQty, 0))) QtyOrder
FROM svTrnSrvItem a WITH (NOLOCK, NOWAIT)
INNER JOIN svTrnService b ON b.CompanyCode = a.CompanyCode
	AND b.BranchCode = a.BranchCode
	AND b.ProductType = a.ProductType
	AND b.ServiceNo = a.ServiceNo
	AND b.JobOrderNo = @JobOrderNo
WHERE a.CompanyCode = @CompanyCode 
	AND a.BranchCode = @BranchCode 
	AND a.ProductType = @ProductType 
GROUP BY a.CompanyCode, a.BranchCode, a.ProductType
	, a.ServiceNo, a.PartNo, a.RetailPrice, a.BillType ) #Item 

DECLARE @CompanyMD AS VARCHAR(15)
DECLARE @BranchMD AS VARCHAR(15)
DECLARE @WarehouseMD AS VARCHAR(15)

SET @CompanyMD = (SELECT CompanyMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)
SET @BranchMD = (SELECT BranchMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)
SET @WarehouseMD = (SELECT WarehouseMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)

SELECT * INTO #SrvOrder FROM (
SELECT DISTINCT(a.CompanyCode) 
    , a.BranchCode
	, a.ProductType
	, a.ServiceNo
	, a.PartNo
    , (SELECT Paravalue FROM gnMstLookUpDtl WHERE CompanyCode = a.CompanyCode AND CodeID = 'GTGO' AND LookUpValue = a.TypeOfGoods) TipePart
    , (SELECT PartName FROM spMstItemInfo WHERE CompanyCode = a.CompanyCode AND PartNo = a.PartNo) PartName
	, a.RetailPrice
	, a.CostPrice
    , a.TypeOfGoods
    , a.BillType
	, SUM(a.QtyOrder) QtyOrder
    , 0 QtySupply
    , 0 QtyBO
    , (SUM(a.QtyOrder) * a.RetailPrice) * ((100 - a.PartDiscPct)/100) NetSalesAmt
    , a.PartDiscPct DiscPct
FROM
(
	SELECT
		DISTINCT(a.CompanyCode) 
		, a.BranchCode
		, a.ProductType
		, a.ServiceNo
		, a.PartNo
		, a.RetailPrice
		, a.CostPrice
		, a.TypeOfGoods
		, a.BillType
		, ISNULL(Item.QtyOrder,0) AS QtyOrder
		, a.DiscPct PartDiscPct 
	FROM
		svTrnSrvItem a WITH (NOLOCK, NOWAIT)
		LEFT JOIN svTrnService b WITH (NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode
			AND a.BranchCode = b.BranchCode	
			AND a.ProductType = b.ProductType
			AND a.ServiceNo = b.ServiceNo
		LEFT JOIN #Item Item ON Item.CompanyCode = a.CompanyCode 
			AND Item.BranchCode = a.BranchCode 
			AND Item.ProductType = a.ProductType 
			AND Item.ServiceNo = a.ServiceNo 
			AND Item.PartNo = a.PartNo 
			AND Item.RetailPrice = a.RetailPrice 
			AND Item.BillType = a.Billtype
		--LEFT JOIN SpMstItemPrice c WITH (NOLOCK, NOWAIT) ON a.CompanyCode = c.CompanyCode 
		--	AND a.BranchCode = c.BranchCode 
		--	AND a.PartNo = c.PartNo
	WHERE
		a.CompanyCode = @CompanyCode
		AND a.BranchCode = @BranchCode
		AND a.ProductType = @ProductType
		AND Item.QtyOrder > 0
		AND JobOrderNo = @JobOrderNo
		AND (a.SupplySlipNo is null OR a.SupplySlipNo = '')
) a
GROUP BY
	a.CompanyCode
	, a.BranchCode
	, a.ProductType
	, a.ServiceNo
	, a.PartNo
	, a.RetailPrice
	, a.CostPrice
    , a.TypeOfGoods
    , a.BillType
    , a.PartDiscPct 
) #SrvOrder

select * from #srvorder

--================================================================================================================================
-- INSERT TABLE SpTrnSORDHdr AND SpTrnSORDDtl
--================================================================================================================================
DECLARE @MaxDocNo			INT
DECLARE	@MaxPickingList		INT
DECLARE @TempDocNo			VARCHAR(MAX)
DECLARE @TempPickingList	VARCHAR(MAX)
DECLARE @TypeOfGoods		VARCHAR(MAX)
DECLARE @DefaultDate		DATETIME

SET @DefaultDate = '1900-01-01 00:00:00.000'

--===============================================================================================================================
-- LOOPING BASED ON THE TYPE OF GOODS
-- ==============================================================================================================================
DECLARE db_cursor CURSOR FOR
SELECT DISTINCT TypeOfGoods FROM #SrvOrder
WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND ProductType = @ProductType 

OPEN db_cursor
FETCH NEXT FROM db_cursor INTO @TypeOfGoods

WHILE @@FETCH_STATUS = 0
BEGIN

--===============================================================================================================================
-- INSERT HEADER
-- ==============================================================================================================================
SET @MaxDocNo = ISNULL((SELECT DocumentSequence + 1 FROM GnMstDocument WHERE 
	CompanyCode = @CompanyCode
		AND BranchCode = @BranchCode
		AND DocumentType = 'SSS' 
		AND ProfitCenterCode = '300' 
		AND DocumentYear = YEAR(GetDate())),0)

SET @TempDocNo = ISNULL((SELECT 'SSS/' + RIGHT(YEAR(GETDATE()),2) +'/' + RIGHT(1000000 + CONVERT(VARCHAR, @MaxDocNo, 1), 6)),'SSS/YY/XXXXXX')

INSERT INTO SpTrnSORDHdr
([CompanyCode]
           ,[BranchCode]
           ,[DocNo]
           ,[DocDate]
           ,[UsageDocNo]
           ,[UsageDocDate]
           ,[CustomerCode]
           ,[CustomerCodeBill]
           ,[CustomerCodeShip]
           ,[isBO]
           ,[isSubstitution]
           ,[isIncludePPN]
           ,[TransType]
           ,[SalesType]
           ,[IsPORDD]
           ,[OrderNo]
           ,[OrderDate]
           ,[TOPCode]
           ,[TOPDays]
           ,[PaymentCode]
           ,[PaymentRefNo]
           ,[TotSalesQty]
           ,[TotSalesAmt]
           ,[TotDiscAmt]
           ,[TotDPPAmt]
           ,[TotPPNAmt]
           ,[TotFinalSalesAmt]
           ,[isPKP]
           ,[ExPickingSlipNo]
           ,[ExPickingSlipDate]
           ,[Status]
           ,[PrintSeq]
           ,[TypeOfGoods]
           ,[isDropsign]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastUpdateBy]
           ,[LastUpdateDate]
           ,[isLocked]
           ,[LockingBy]
           ,[LockingDate])

SELECT 
	@CompanyCode CompanyCode
	, @BranchCode BranchCode
	, @TempDocNo DocNo 
	, @DocDate DocDate
	, @JobOrderNo UsageDocNo
	, (SELECT JobOrderDate FROM SvTrnService WHERE 1 =1 AND CompanyCode = @CompanyCode AND BranchCode = @BranchCode 
		AND ProductType = @ProductType AND JobOrderNo = @JobOrderNo) UsageDocDate
	, (SELECT CustomerCode FROM SvTrnService WHERE 1 = 1AND CompanyCode = @CompanyCode AND BranchCode = @BranchCode 
		AND ProductType = @ProductType AND JobOrderNo = @JobOrderNo) CustomerCode
	, (SELECT CustomerCodeBill FROM SvTrnService WHERE 1 = 1 AND CompanyCode = @CompanyCode AND BranchCode = @BranchCode 
		AND ProductType = @ProductType AND JobOrderNo = @JobOrderNo) CustomerCodeBill
	, (SELECT CustomerCode FROM SvTrnService WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode 
		AND ProductType = @ProductType AND JobOrderNo = @JobOrderNo) CustomerCodeShip
	, CONVERT(BIT, 0) isBO
	, CONVERT(BIT, 0) isSubstitution
	, CONVERT(BIT, 1) isIncludePPN
	, @TransType TransType
	, '2' SalesType
	, CONVERT(BIT, 0) isPORDD
	, @JobOrderNo OrderNo
	, @DocDate OrderDate
	, ISNULL((SELECT TOPCode FROM GnMstCustomerProfitCenter WITH (NOLOCK, NOWAIT) WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
		AND ProfitCenterCode = '300' AND CustomerCode = @CustomerCode),'W') TOPCode
	, ISNULL((SELECT ParaValue FROM GnMstLookUpDtl WITH (NOLOCK, NOWAIT) WHERE CompanyCode = @CompanyCode AND CodeID = 'TOPC' AND 
		LookupValue IN 
		(SELECT TOPCode FROM GnMstCustomerProfitCenter WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
			AND ProfitCenterCode = '300' AND CustomerCode = @CustomerCode)
	  ),0) TOPDays
	, ISNULL((SELECT PaymentCode FROM GnMstCustomerProfitCenter WITH (NOLOCK, NOWAIT) WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
		AND ProfitCenterCode = '300' AND CustomerCode = @CustomerCode),'W') PaymentCode
	, '' PaymentReffNo
	, 0 TotSalesQty
	, 0 TotSalesAmt
	, 0 TotDiscAmt
	, 0 TotDPPAmt
	, 0 TotPPNAmt
	, 0 TotFinalSalesAmt
	, CONVERT(BIT, 0) isPKP
	, NULL ExPickingSlipNo
	, NULL ExPickingSlipDate
	, '4' Status
	, 0 PrintSeq
	, @TypeOfGoods TypeOfGoods
	, NULL IsDropSign
	, @UserID CreatedBY
	, GetDate() CreatedDate
	, @UserID LastUpdateBy
	, GetDate() LastUpdateDate
	, NULL isLocked
	, NULL LockingBy
	, NULL LockingDate


UPDATE GnMstDocument
SET DocumentSequence = DocumentSequence + 1
	, LastUpdateDate = GetDate()
	, LastUpdateBy = @UserID
WHERE
	CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND DocumentPrefix = 'SSS'
	AND ProfitCenterCode = '300'
	AND DocumentYear = Year(GetDate())

--===============================================================================================================================
-- INSERT DETAIL
-- ==============================================================================================================================
DECLARE @DbMD AS VARCHAR(15)
SET @DbMD = (SELECT DbMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)

declare @TempAvailStock as table
(
	PartNo varchar(50),
	AvailStock decimal
)

DECLARE @Query AS VARCHAR(MAX)
--SET @Query = 
--'SELECT PartNo, (Onhand - AllocationSP - AllocationSL - AllocationSR - ReservedSP - ReservedSL - ReservedSR) AvailStock
--FROM ' + @DbMD + '..SpMstItemLoc WITH (NOLOCK, NOWAIT) 
--WHERE CompanyCode = '+''''+@CompanyMD+''''+' AND BranchCode ='+''''+@BranchMD +''''+' AND WarehouseCode = '+''''+@WarehouseMD+''''+''

--INSERT INTO #TempAvailStock

SET @Query = 
'SElect * into #TempAvailStock from (SELECT PartNo, (Onhand - AllocationSP - AllocationSL - AllocationSR - ReservedSP - ReservedSL - ReservedSR) AvailStock
FROM ' + @DbMD + '..SpMstItemLoc WITH (NOLOCK, NOWAIT) 
WHERE CompanyCode = '+''''+@CompanyMD+''''+' AND BranchCode ='+''''+@BranchMD +''''+' AND WarehouseCode = '+''''+@WarehouseMD+''''+')#TempAvailStock

INSERT INTO SpTrnSORDDtl 
(
	[CompanyCode] ,
	[BranchCode] ,
	[DocNo] ,
	[PartNo] ,
	[WarehouseCode] ,
	[PartNoOriginal] ,
	[ReferenceNo] ,
	[ReferenceDate] ,
	[LocationCode] ,
	[QtyOrder] ,
	[QtySupply] ,
	[QtyBO] ,
	[QtyBOSupply] ,
	[QtyBOCancel] ,
	[QtyBill] ,
	[RetailPriceInclTax] ,
	[RetailPrice] ,
	[CostPrice] ,
	[DiscPct] ,
	[SalesAmt] ,
	[DiscAmt] ,
	[NetSalesAmt] ,
	[PPNAmt] ,
	[TotSalesAmt] ,
	[MovingCode] ,
	[ABCClass] ,
	[ProductType] ,
	[PartCategory] ,
	[Status] ,
	[CreatedBy] ,
	[CreatedDate] ,
	[LastUpdateBy] ,
	[LastUpdateDate] ,
	[StockAllocatedBy] ,
	[StockAllocatedDate] ,
	[FirstDemandQty] )
SELECT
	''' + @CompanyCode +''' CompanyCode
	, ''' + @BranchCode +''' BranchCode
	, ''' + @TempDocNo +''' DocNo 
	, a.PartNo
	, ''00'' WarehouseCode
	, a.PartNo PartNoOriginal
	, ''' + @JobOrderNo +''' ReferenceNo
	, (SELECT JobOrderDate FROM SvTrnService WHERE 1 =1 AND CompanyCode = ''' + @CompanyCode +''' AND BranchCode = ''' + @BranchCode +'''
		AND ProductType = ''' + @ProductType +''' AND JobOrderNo = ''' + @JobOrderNo +''' ) ReferenceDate
	, (SELECT distinct LocationCode FROM ' + @DbMD +'..SpMstItemLoc WHERE CompanyCode = ''' + @CompanyMD +''' AND BranchCode = ''' + @BranchMD +''' AND WarehouseCode = ''00''
		AND PartNo = a.PartNo ) LocationCode
	, a.QtyOrder
	, CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN a.QtyOrder 
		ELSE ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0)
		END AS QtySupply
	, CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN 0 
		ELSE a.QtyOrder - ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0)
		END AS QtyBO
	, 0 QtyBOSupply
	, CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN 0 
		ELSE a.QtyOrder - ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0)
		END AS QtyBOCancel
	, 0 QtyBill
	, a.RetailPrice + FLOOR(a.RetailPrice * 10 /100) RetailPriceIncltax
	, a.RetailPrice
	, a.CostPrice
	, a.DiscPct
	, CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN a.QtyOrder * a.RetailPrice
		ELSE ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) * a.RetailPrice
		END AS SalesAmt
	, CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN floor((a.QtyOrder * a.RetailPrice) * a.DiscPct/100)
		ELSE floor((ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) * a.RetailPrice) * a.DiscPct/100)
		END AS DiscAmt
	, CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN floor((a.QtyOrder * a.RetailPrice)- ((a.QtyOrder * a.RetailPrice) * a.DiscPct/100))
		ELSE floor((ISNULL((SELECT AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) * a.RetailPrice) - 
			(ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) * a.RetailPrice * a.DiscPct/100))
		END AS NetSalesAmt
	, 0 PPNAmt
	,  CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN floor((a.QtyOrder * a.RetailPrice)- ((a.QtyOrder * a.RetailPrice) * a.DiscPct/100))
		ELSE floor((ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) * a.RetailPrice) - 
			(ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) * a.RetailPrice * a.DiscPct/100))
		END AS TotSalesAmt
	, (SELECT distinct MovingCode FROM '+ @DbMD +'..SpMstItems WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyMD +''' AND BranchCode = '''+ @BranchMD +''' AND PartNo = a.PartNo) 
		MovingCode
	, (SELECT distinct ABCClass FROM '+ @DbMD +'..SpMstItems WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyMD +''' AND BranchCode = '''+ @BranchMD +''' AND PartNo = a.PartNo) 
		ABCClass
	, '''+ @ProductType +''' ProductType
	, (SELECT distinct PartCategory FROM '+ @DbMD +'..SpMstItems WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyMD +'''  AND BranchCode = '''+ @BranchMD +''' AND PartNo = a.PartNo) 
		PartCategory
	, ''2'' Status
	, '''+ @UserID +''' CreatedBy
	, '''+ Convert(varchar,GetDate()) +''' CreatedDate
	, '''+ @UserID +''' LastUpdateBy
	, '''+ Convert(varchar,GetDate()) +''' LastUpdateDate
	, '''+ @UserID +''' StockAllocatedBy
	, '''+ Convert(varchar,GetDate()) +''' StockAllocatedDate
	, a.QtyOrder FirstDemandQty
FROM #SrvOrder a
WHERE a.TypeOfGoods = '+@TypeOfGoods +'


select top 10 * from spTrnSORDDtl order by createddate desc
--===============================================================================================================================
-- INSERT SO SUPPLY
-- ==============================================================================================================================

SELECT * INTO #TempSOSupply FROM (
SELECT
	'''+ @CompanyCode +''' CompanyCode
	, '''+ @BranchCode +''' BranchCode
	, '''+ @TempDocNo +''' DocNo 
	, 0 SupSeq
	, a.PartNo 
	, a.PartNo PartNoOriginal
	, '''' PickingSlipNo	
	, '''+ @JobOrderNo +''' ReferenceNo
	, '''+ CONVERT(varchar, @DefaultDate )+''' ReferenceDate
	, ''00'' WarehouseCode
	, (SELECT distinct LocationCode FROM '+ @DbMD+'..SpMstItemLoc WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyMD +''' AND BranchCode = '''+ @BranchMD+''' AND WarehouseCode = ''00''
		AND PartNo = a.PartNo) LocationCode
	, CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN a.QtyOrder 
		ELSE ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0)
		END AS QtySupply
	, 0 QtyPicked
	, 0 QtyBill
	, a.RetailPrice + FLOOR(a.RetailPrice *10 /100) RetailPriceIncltax
	, a.RetailPrice
	, a.CostPrice
	, a.DiscPct
	, (SELECT distinct MovingCode FROM '+ @DbMD+'..SpMstItems WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyMD +''' AND BranchCode = '''+ @BranchMD +''' AND PartNo = a.PartNo) 
		MovingCode
	, (SELECT distinct ABCClass FROM '+ @DbMD +'..SpMstItems WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyMD +''' AND BranchCode = '''+ @BranchMD +''' AND PartNo = a.PartNo) 
		ABCClass
	, '''+ @ProductType +'''ProductType
	, (SELECT distinct PartCategory FROM '+ @DbMD +'..SpMstItems WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyMD +''' AND BranchCode = '''+ @BranchMD +''' AND PartNo = a.PartNo) 
		PartCategory
	, ''1'' Status
	, '''+ @UserID +''' CreatedBy
	, '''+ Convert(varchar,GetDate()) +''' CreatedDate
	, '''+ @UserID +''' LastUpdateBy
	, '''+ Convert(varchar,GetDate()) +''' LastUpdateDate
	, '''+ @UserID +''' StockAllocatedBy
	, '''+ Convert(varchar,GetDate()) +''' StockAllocatedDate
FROM #SrvOrder a
WHERE a.CompanyCode = '''+ @CompanyCode +''' AND a.BranchCode = '''+ @BranchCode +''' 
and a.TypeOfGoods = '+ @TypeOfGoods +'
)#TempSOSupply

INSERT INTO SpTrnSOSupply SELECT 
	CompanyCode,BranchCode,DocNo,SupSeq,PartNo,PartNoOriginal
	, ROW_NUMBER() OVER(ORDER BY PartNo) PTSeq,PickingSlipNo
	, ReferenceNo,ReferenceDate,WarehouseCode,LocationCode
	, QtySupply,QtyPicked,QtyBill,RetailPriceIncltax,RetailPrice,CostPrice
	, DiscPct,MovingCode,ABCClass,ProductType,PartCategory,Status
	, CreatedBy,CreatedDate,LastUpdateBy,LastUpdateDate
FROM #TempSOSupply WHERE QtySupply > 0

--===============================================================================================================================
-- UPDATE STATUS DETAIL BASED ON SUPPLY
--===============================================================================================================================

UPDATE SpTrnSORDDtl
SET Status = 4
FROM SpTrnSORDDtl a, #TempSOSupply b
WHERE 1 = 1
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.PartNo = b.PartNo
	
--===============================================================================================================================
-- UPDATE HISTORY DEMAND ITEM AND CUSTOMER
--===============================================================================================================================

UPDATE SpHstDemandItem 
SET DemandFreq = DemandFreq + 1
	, DemandQty = DemandQty + b.QtyOrder
	, LastUpdateBy = '''+ @UserID +'''
	, LastUpdateDate = '''+ Convert(varchar,GetDate()) +''' 
FROM SpHstDemandItem a, SpTrnSordDtl b
WHERE 
	1 = 1
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.Year = '''+ Convert(varchar,Year(GetDate())) +''' 
	AND a.Month  = '''+ Convert(varchar,Month(GetDate())) +''' 
	AND a.PartNo = b.PartNo
	AND b.DocNo = '''+ @TempDocNo +'''

UPDATE SpHstDemandCust
SET DemandFreq = DemandFreq + 1
	, DemandQty = DemandQty + b.QtyOrder
	, LastUpdateBy = '''+ @UserID +''' 
	, LastUpdateDate = '''+ Convert(varchar,GetDate()) +''' 
FROM SpHstDemandCust a, SpTrnSordDtl b
WHERE 
	1 = 1
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.Year = '''+ Convert(varchar,Year(GetDate())) +'''
	AND a.Month  = '''+ Convert(varchar,Month(GetDate())) +'''
	AND a.PartNo = b.PartNo
	AND a.CustomerCode = '''+ @CustomerCode +'''
	AND b.DocNo = '''+ @TempDocNo +'''

INSERT INTO SpHstDemandItem
SELECT 
	CompanyCode
	, BranchCode
	, '''+ Convert(varchar,Year(GetDate())) +''' Year
	, '''+ Convert(varchar,Month(GetDate())) +''' Month
	, PartNo
	, 1 DemandFreq
	, QtyOrder DemandQty
	, 0 SalesFreq
	, 0 SalesQty
	, MovingCode
	, ProductType
	, PartCategory
	, ABCClass
	, '''+ @UserID +''' LastUpdateBy
	, '''+ CONVERT(varchar, GetDate()) +''' LastUpdateDate
FROM SpTrnSordDtl a WITH (NOLOCK, NOWAIT)
WHERE a.CompanyCode= '''+ @CompanyCode +''' AND a.BranchCode = '''+ @BranchCode +''' AND a.DocNo = '''+ @TempDocNo +''' -- add CompanyCode and BranchCode 13 Des 2010
 AND NOT EXISTS
( SELECT 1 FROM SpHstDemandItem WITH (NOLOCK, NOWAIT) WHERE 
	1 = 1 
	AND CompanyCode = a.CompanyCode 
	AND BranchCode = a.BranchCode
	AND Month = '''+ Convert(varchar,Month(GetDate())) +''' 
	AND Year = '''+ Convert(varchar,Year(GetDate())) +''' 
	AND PartNo = a.PartNo
)

INSERT INTO SpHstDemandCust
SELECT 
	CompanyCode
	, BranchCode
	, '''+ Convert(varchar,Year(GetDate())) +'''  Year
	, '''+ Convert(varchar,Month(GetDate())) +'''  Month
	, '''+ @CustomerCode +''' CustomerCode
	, PartNo
	, 1 DemandFreq
	, (SELECT QtyOrder FROM SpTrnSORDDTl WITH (NOLOCK, NOWAIT) 
		WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode 
		AND DocNo = a.DocNo AND PartNo = a.PartNo) DemandQty
	, 0 SalesFreq
	, 0 SalesQty
	, MovingCode
	, ProductType
	, PartCategory
	, ABCClass
	, '''+ @UserID +''' LastUpdateBy
	, '''+ CONVERT(varchar, GetDate()) +''' LastUpdateDate
FROM SpTrnSordDtl a WITH (NOLOCK, NOWAIT)
WHERE a.CompanyCode= '''+ @CompanyCode +''' and a.BranchCode= '''+ @BranchCode +''' AND a.DocNo = '''+ @TempDocNo +''' -- add CompanyCode and BranchCode 13 Des 2010
AND NOT EXISTS
( SELECT PartNo FROM SpHstDemandCust WITH (NOLOCK, NOWAIT) WHERE 
	1 = 1 
	AND CompanyCode = a.CompanyCode 
	AND BranchCode = a.BranchCode
	AND Month = '''+ Convert(varchar,Month(GetDate())) +''' 
	AND Year = '''+ Convert(varchar,Year(GetDate())) +'''  
	AND PartNo = a.PartNo
)

--===============================================================================================================================
-- UPDATE LAST DEMAND DATE MASTER
--===============================================================================================================================

UPDATE '+@DbMD+'..SpMstItems 
SET LastDemandDate = '''+Convert(varchar,GetDate())+'''
FROM '+@DbMD+'..SpMstItems a, SpTrnSordDtl b
WHERE 
	1 = 1
	AND a.CompanyCode = '''+@CompanyMD+'''
	AND a.BranchCode = '''+@BranchMD+'''
	AND a.PartNo = b.PartNo
	AND b.DocNo = '''+@TempDocNo+'''

--===============================================================================================================================
-- UPDATE STOCK AND MOVEMENT
--===============================================================================================================================

UPDATE '+@DbMD+'..spMstItems
SET AllocationSR = AllocationSR + b.QtySupply
	, LastUpdateBy = '''+@UserID+'''
	, LastUpdateDate = '''+Convert(varchar,GetDate())+'''
FROM '+@DbMD+'..SpMstItems a, #TempSOSupply b
WHERE 
	1 = 1
	AND a.CompanyCode = '''+@CompanyMD+'''
	AND a.BranchCode = '''+@BranchMD+'''
	AND a.PartNo = b.PartNo

UPDATE '+@DbMD+'..spMstItemloc
SET AllocationSR = AllocationSR + b.QtySupply
	, LastUpdateBy = '''+@UserID+'''
	, LastUpdateDate = '''+Convert(varchar,GetDate())+'''
FROM '+@DbMD+'..SpMstItemLoc a, #TempSOSupply b
WHERE 
	1 = 1
	AND a.CompanyCode = '''+@CompanyMD +'''
	AND a.BranchCode = '''+@BranchMD+'''
	AND a.WarehouseCode = '''+@WarehouseMD+'''
	AND a.PartNo = b.PartNo

INSERT INTO SpTrnIMovement
SELECT
	'''+@CompanyCode +''' CompanyCode
	, '''+@BranchCode +''' BranchCode
	, a.DocNo
	, (SELECT DocDate FROM SpTrnSORDHdr WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyCode +'''
		AND BranchCode = '''+ @BranchCode +''' AND DocNo = a.DocNo) 
	  DocDate
	, dateadd(s,ROW_NUMBER() OVER(Order by a.PartNo),'''+convert(varchar,getdate())+''') CreatedDate 
	, ''00'' WarehouseCode
	, (SELECT LocationCode FROM SpTrnSORDDtl WITH (NOLOCK, NOWAIT) WHERE CompanyCode =  '''+@CompanyCode +'''
		AND BranchCode = '''+@BranchCode +''' AND DocNo = '''+@TempDocNo +''' AND PartNo = a.PartNo)
	  LocationCode
	, a.PartNo
	, ''OUT'' SignCode
	, ''SA-NPJUAL'' SubSignCode
	, a.QtySupply
	, a.RetailPrice
	, a.CostPrice
	, a.ABCClass
	, a.MovingCode
	, a.ProductType
	, a.PartCategory
	, '''+@UserID +''' CreatedBy
FROM #TempSOSupply a

--===============================================================================================================================
-- UPDATE SUPPLY SLIP TO SPK
--===============================================================================================================================
DECLARE @ServiceNo VARCHAR(MAX)

SET @ServiceNo = (SELECT ServiceNo FROM svTrnService WHERE CompanyCode = '''+@CompanyCode +''' AND BranchCode = '''+@BranchCode+'''
		AND ProductType = '''+@ProductType +''' AND JobOrderNo = '''+@JobOrderNo+''')

SELECT * INTO #TempServiceItem FROM (
SELECT 
	a.CompanyCode
	, a.BranchCode
	, a.ProductType
	, a.ServiceNo
	, a.PartNo
	, a.PartSeq
	, a.DemandQty
	, a.SupplyQty
	, b.QtySupply
	, b.DocNo
	, a.CostPrice
	, a.RetailPrice
	, a.TypeOfGoods
	, a.BillType
FROM SvTrnSrvItem a WITH (NOLOCK, NOWAIT)
INNER JOIN #TempSOSupply b WITH (NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.PartNo = b.PartNo
WHERE
	1 = 1
	AND a.CompanyCode = '''+@CompanyCode+'''
	AND a.BranchCode = '''+@BranchCode+'''
	AND a.ProductType = '''+@ProductType+'''
	AND a.ServiceNo = @ServiceNo
	AND a.PartSeq IN (SELECT MAX(PartSeq) FROM SvTrnSrvItem WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode
		AND ProductType = '''+@ProductType +''' AND ServiceNo = a.ServiceNo AND PartNo = a.PartNo)
	AND (a.SupplySlipNo = '''' OR a.SupplySlipNo IS NULL)
) #TempServiceItem 

SELECT * INTO #TempServiceItemIns FROM( 
SELECT 
	a.CompanyCode
	, a.BranchCode
	, a.ProductType
	, a.ServiceNo
	, a.PartNo
	, a.PartSeq
	, a.DemandQty
	, a.SupplyQty
	, b.QtySupply
	, b.DocNo
	, a.CostPrice
	, a.RetailPrice
	, a.TypeOfGoods
	, a.BillType
	, a.DiscPct
FROM SvTrnSrvItem a WITH (NOLOCK, NOWAIT)
INNER JOIN #TempSOSupply b WITH (NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.PartNo = b.PartNo
WHERE
	1 = 1 
	AND a.CompanyCode = '''+ @CompanyCode +''' 
	AND a.BranchCode = '''+ @BranchCode +''' 
	AND a.ProductType = '''+ @ProductType +'''  
	AND a.ServiceNo = @ServiceNo
	AND a.PartSeq IN (SELECT MAX(PartSeq) FROM SvTrnSrvItem WITH (NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode
		AND ProductType = '''+ @ProductType +''' AND ServiceNo = a.ServiceNo AND PartNo = a.PartNo) 
	AND (a.SupplySlipNo != '''' OR a.SupplySlipNo IS NOT NULL)
) #TempServiceItemIns


UPDATE svTrnSrvItem
SET SupplySlipNo = b.DocNo
	, SupplySlipDate = ISNULL((SELECT DocDate FROM SpTrnSORDHdr WITH (NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode
							AND DocNo = b.DocNo),'''+Convert(varchar,@DefaultDate)+''')
FROM svTrnSrvItem a, #TempServiceItem b
WHERE
	1 = 1
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.ProductType = b.ProductType
	AND a.ServiceNo = b.ServiceNo
	AND a.PartNo = b.PartNo
	AND a.PartSeq = b.PartSeq
	
--===============================================================================================================================
-- INSERT NEW SRV ITEM BASED SUPPLY SLIP
--===============================================================================================================================
INSERT INTO SvTrnSrvItem (CompanyCode, BranchCode, ProductType, ServiceNo, PartNo, PartSeq, DemandQty, SupplyQty, ReturnQty, CostPrice, RetailPrice, TypeOfGoods, BillType, SupplySlipNo, SupplySlipDate, SSReturnNo, SSReturnDate, CreatedBy, CreatedDate, LastupdateBy, LastupdateDate, DiscPct)
SELECT 
	a.CompanyCode
	, a.BranchCode
	, a.ProductType
	, a.ServiceNo
	, a.PartNo
	, a.PartSeq + 1
	, 0 DemandQty
	, 0 SupplyQty
	, 0 ReturnQty
	, a.CostPrice
	, a.RetailPrice
	, a.TypeOfGoods
	, a.BillType
	, a.DocNo SupplySlipNo
	, (SELECT DocDate FROM SpTrnSORDHdr WITH (NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode
		AND DocNo = a.DocNo) SupplySlipDate
	, NULL SSReturnNo
	, NULL SSReturnDate
	, '''+ @UserID +''' CreatedBy
	, '''+ Convert(varchar,GetDate()) +''' CreatedDate
	, '''+ @UserID +''' LastUpdateBy
	, '''+ Convert(varchar,GetDate()) +''' LastUpdateDate
	, a.DiscPct
FROM #TempServiceItemIns a WITH (NOLOCK, NOWAIT)
WHERE
	1 = 1
	AND a.CompanyCode = '''+ @CompanyCode +'''
	AND a.BranchCode = '''+ @BranchCode +'''
	AND a.ProductType = '''+ @ProductType+'''


--===============================================================================================================================
DROP TABLE #TempServiceItem 
DROP TABLE #TempServiceItemIns
DROP TABLE #TempSOSupply'

EXEC(@query)

--select convert(xml,@query)


--===============================================================================================================================
-- INSERT SVSDMOVEMENT
--===============================================================================================================================

declare @md bit
set @md = (select case WHEN EXISTS(select * from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode and CompanyMD = @CompanyCode and BranchMD = @BranchCode) then cast(1 as bit) ELSE cast(0 as bit) END)

if(@md = 0)
begin

 declare @QueryTemp as varchar(max)  
 
	set @Query ='insert into '+ @DbMD +'..svSDMovement
	select a.CompanyCode, a.BranchCode, a.DocNo, a.CreatedDate, a.PartNo
	, Seq = convert(integer, ROW_NUMBER() OVER (PARTITION BY a.ReferenceNo ORDER BY a.DocNo)) ,
	a.WarehouseCode, a.QtyOrder, a.QtySupply, a.DiscPct
	--,isnull(((select RetailPrice from spTrnSORDDtl
	--		where CompanyCode = ''' + @CompanyCode + '''  and BranchCode = ''' + @BranchCode  + '''
	--		and DocNo = ''' + @TempDocNo + ''' and PartNo = a.PartNo) / 1.1 * 
	--		((100 - isnull((select isnull(DiscPct, 0) from gnMstSupplierProfitCenter   
	--			where CompanyCode = ''' + @CompanyCode + ''' and BranchCode = ''' + @BranchCode  + ''' and SupplierCode = dbo.GetBranchMD(''' + @CompanyCode + ''', ''' + @BranchCode  + ''') 
	--			and ProfitCenterCode = ''300''),0)) * 0.01)),0) 
	, a.CostPrice
	, a.RetailPrice, b.TypeOfGoods
	, '''+ @CompanyMD +''','''+ @BranchMD +''','''+ @WarehouseMD +''', md.RetailPriceInclTax, md.RetailPrice, md.CostPrice
	,''x'','''+ @producttype +''',''300'',''0'',''0'','''+ convert(varchar,GETDATE()) +''','''+ @UserID +''','''+ convert(varchar,GETDATE()) +'''
	,'''+ @UserID +''','''+ convert(varchar,GETDATE()) +'''
	from spTrnSORDDtl a 
	join spTrnSORDHdr b on a.CompanyCode = b.CompanyCode
	and a.BranchCode = b.BranchCode 
	and a.DocNo = b.DocNo
	join '+ @DbMD +'..spmstitemprice md
	on md.CompanyCode = '''+ @CompanyMD +'''
	and md.branchcode = '''+ @BranchMD +'''
	and md.PartNo = a.PartNo
	where a.ReferenceNo = '''+ @JobOrderNo +''''+
	' and a.DocNo = ''' + @TempDocNo + '''';

	exec (@Query)
	print (@QUERY)

end

--===============================================================================================================================
-- INSERT PICKING HEADER AND DETAIL
--===============================================================================================================================

SET @MaxPickingList = ISNULL((SELECT DocumentSequence + 1 FROM GnMstDocument WHERE 
	CompanyCode = @CompanyCode
		AND BranchCode = @BranchCode
		AND DocumentType = 'PLS' 
		AND ProfitCenterCode = '300' 
		AND DocumentYear = YEAR(GetDate())),0)

SET @TempPickingList = ISNULL((SELECT 'PLS/' + RIGHT(YEAR(GETDATE()),2) +'/' + RIGHT(1000000 + CONVERT(VARCHAR, @MaxPickingList, 1), 6)),'PLS/YY/XXXXXX')

INSERT INTO SpTrnSPickingHdr 
SELECT 
	CompanyCode
	, BranchCode
	, @TempPickingList PickingSlipNo
	, GetDate() PickingSlipDate
	, CustomerCode
	, CustomerCodeBill
	, CustomerCodeShip
	, '' PickedBy
	, CONVERT(BIT, 0) isBORelease
	, isSubstitution
	, isIncludePPN
	, TransType
	, SalesType
	, TotSalesQty
	, TotSalesAmt
	, TotDiscAmt
	, TotDPPAmt
	, TotPPNAmt
	, TotFinalSalesAmt
	, '' Remark
	, '0' Status
	, '0' PrintSeq
	, TypeOfGoods
	, CreatedBy
	, CreatedDate
	, LastUpdateBy
	, LastUpdateDate
	, NULL isLocked
	, NULL LockingBy
	, NULL LockingDate
FROM SpTrnSORDHdr WITH (NOLOCK, NOWAIT)
WHERE 
	1 = 1
	AND CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND DocNo = (SELECT distinct DocNo FROM spTrnSORDDtl WHERE CompanyCode = @CompanyCode AND Branchcode = @BranchCode 
					AND DocNo = @TempDocNo AND QtySupply > 0)		

UPDATE GnMstDocument
SET DocumentSequence = DocumentSequence + 1
	, LastUpdateDate = GetDate()
	, LastUpdateBy = @UserID
WHERE
	1 = 1
	AND CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND DocumentPrefix = 'PLS'
	AND ProfitCenterCode = '300'
	AND DocumentYear = Year(GetDate())

-- ==============================================================================================================================
-- UPDATE SALES ORDER HEADER 
-- ==============================================================================================================================
UPDATE SpTrnSORDHdr
	SET ExPickingSlipNo = @TempPickingList,
		ExPickingSlipDate = ISNULL((SELECT PickingSlipDate FROM SpTrnSPickingHdr WHERE CompanyCode = @CompanyCode
				AND BranchCode = @BranchCode AND PickingSlipNo = @TempPickingList),'')
	
WHERE
	1 = 1
	AND CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND DocNo = @TempDocNo

UPDATE SpTrnSOSupply
	SET PickingSlipNo = @TempPickingList
WHERE
	1 = 1
	AND CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND DocNo = @TempDocNo
-- ==============================================================================================================================
-- INSERT PICKING DETAIL
-- ==============================================================================================================================

INSERT INTO SpTrnSPickingDtl
SELECT 
	a.CompanyCode
	, a.BranchCode
	, @TempPickingList PickingSlipNo
	, '00' WarehouseCode
	, a.PartNo
	, a.PartNoOriginal
	, a.DocNo
	, b.DocDate 
	, a.ReferenceNo
	, a.ReferenceDate
	, a.LocationCode
	, a.QtySupply QtyOrder
	, a.QtySupply
	, a.QtySupply QtyPicked 
	, 0 QtyBill
	, a.RetailPriceInclTax
	, a.RetailPrice
	, a.CostPrice
	, a.DiscPct
	, a.SalesAmt
	, a.DiscAmt
	, a.NetSalesAmt
	, a.TotSalesAmt
	, a.MovingCode
	, a.ABCClass
	, a.ProductType
	, a.PartCategory
	, '' ExPickingSlipNo
	, @DefaultDate ExPickingSlipDate
	, CONVERT(BIT, 0) isClosed
	, @UserID CreatedBy
	, GetDate() CreatedDate
	, @UserID LastUpdateBy
	, GetDate() LastUpdateDate
FROM SpTrnSORDDtl a WITH (NOLOCK, NOWAIT)
INNER JOIN SpTrnSORDHdr b WITH (NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.DocNo = b.DocNo
WHERE
	1 = 1
	AND a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND a.DocNo = @TempDocNo
	AND a.QtySupply > 0


--================================================================================================================================
-- UPDATE AMOUNT HEADER
--================================================================================================================================
SELECT * INTO #TempHeader FROM (
SELECT 
	header.CompanyCode
	, header.BranchCode
	, header.DocNo
	, header.TotSalesQty
	, header.TotSalesAmt
	, header.TotDiscAmt
	, header.TotDPPAmt
	, floor(header.TotDPPAmt * (ISNULL((SELECT TaxPct FROM GnMstTax WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
			AND TaxCode IN (SELECT TaxCode FROM GnMstCustomerProfitCenter WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
			AND ProfitCenterCode = '300' AND CustomerCode = @CustomerCode)),0)/100)) 
		TotPPNAmt
	, header.TotDPPAmt + floor(header.TotDPPAmt * (ISNULL((SELECT TaxPct FROM GnMstTax WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
			AND TaxCode IN (SELECT TaxCode FROM GnMstCustomerProfitCenter WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
			AND ProfitCenterCode = '300' AND CustomerCode = @CustomerCode)),0)/100))
		TotFinalSalesAmt
FROM (
SELECT 
	CompanyCode
	, BranchCode
	, DocNo
	, SUM(QtyOrder) TotSalesQty
	, SUM(SalesAmt) TotSalesAmt
	, SUM(DiscAmt) TotDiscAmt
	, SUM(NetSalesAmt) TotDPPAmt
FROM SpTrnSORDDtl WITH (NOLOCK, NOWAIT)
WHERE CompanyCode = @CompanyCode 
	AND BranchCode = @BranchCode
	AND DocNo = @TempDocNo
GROUP BY CompanyCode
	, BranchCode
	, DocNo
) header ) #TempHeader

UPDATE SpTrnSORDHdr
SET 
	TotSalesQty = b.TotSalesQty
	, TotSalesAmt = b.TotSalesAmt
	, TotDiscAmt = b.TotDiscAmt
	, TotDPPAmt = b.TotDPPAmt
	, TotPPNAmt = b.TotPPNAmt
	, TotFinalSalesAmt = b.TotFinalSalesAmt
FROM SpTrnSORDHdr a, #TempHeader b
WHERE 
	1 = 1
	AND a.CompanyCode = b.CompanyCode 
	AND a.BranchCode = b.BranchCode
	AND a.DocNo = b.DocNo

DROP TABLE #TempHeader

FETCH NEXT FROM db_cursor INTO @TypeOfGoods
END
CLOSE db_cursor
DEALLOCATE db_cursor 

--===============================================================================================================================
-- Update Transdate
--===============================================================================================================================

update gnMstCoProfileSpare
set TransDate=getdate()
	, LastUpdateBy=@UserID
	, LastUpdateDate=getdate()
where CompanyCode = @CompanyCode AND BranchCode = @BranchCode

--===============================================================================================================================
-- DROP TABLE SECTION 
--===============================================================================================================================
DROP TABLE #SrvOrder
DROP TABLE #Item

--rollback tran
END
go
if object_id('uspfn_SvTrnServiceInsertDefaultTaskNew') is not null
	drop procedure uspfn_SvTrnServiceInsertDefaultTaskNew
GO
CREATE procedure [dbo].[uspfn_SvTrnServiceInsertDefaultTaskNew]
	@CompanyCode varchar(15),
	@BranchCode varchar(15),
	@ProductType varchar(15),
	@ServiceNo bigint,
	@ServiceBookNo varchar(15),
	@UserID varchar(15)
as      

--declare @CompanyCode varchar(15),
--		@BranchCode varchar(15),
--		@ProductType varchar(15),
--		@ServiceNo bigint,
--		@ServiceBookNo varchar(15)		

--set @CompanyCode = '6006406'
--set	@BranchCode = '6006401'
--set	@ProductType = '4W'
--set	@ServiceNo = '40655'
--set @ServiceBookNo = 'EJ06'

-- check jika count svTrnSrvTask ada maka tidak perlu insert task 
if(select count(*) from svTrnSrvTask
    where 1 = 1
      and CompanyCode = @CompanyCode
      and BranchCode  = @BranchCode
      and ProductType = @ProductType
      and ServiceNo   = @ServiceNo) > 0
	return

-- select data svTrnService
select * into #srv from (
  select a.* from svTrnService a
	where 1 = 1
	  and a.CompanyCode = @CompanyCode
	  and a.BranchCode  = @BranchCode
	  and a.ProductType = @ProductType
	  and a.ServiceNo   = @ServiceNo
)#srv

-----------------------------------------------
-- insert default svTrnSrvTask
-----------------------------------------------
select * into #task from(
select a.CompanyCode, a.ProductType, a.BasicModel, a.JobType, a.OperationNo, a.Description
	 , isnull(c.OperationHour, a.OperationHour) OperationHour
	 , isnull(c.ClaimHour, a.ClaimHour) ClaimHour
	 , isnull(c.LaborCost, a.LaborCost) LaborCost
	 , isnull(c.LaborPrice, a.LaborPrice) LaborPrice
	 , a.IsSubCon, a.IsCampaign, b.CreatedBy as LastupdateBy, getdate() as  LastupdateDate
	 , case when exists (
			select pkg.CompanyCode, pkg.PackageCode, pkg.JobType
			  from svMstPackage pkg
			 inner join svMstPackageTask tsk
				on tsk.CompanyCode = pkg.CompanyCode
			   and tsk.PackageCode = pkg.PackageCode
			 inner join svMstPackageContract con
				on con.CompanyCode = pkg.CompanyCode
			   and con.PackageCode = pkg.PackageCode
			 where pkg.CompanyCode = b.CompanyCode
			   and pkg.JobType = b.JobType
			   and tsk.OperationNo = a.OperationNo
			   and con.ChassisCode = b.ChassisCode
			   and con.ChassisNo = b.ChassisNo
		) then 'P' else (case when isnull(a.BillType, '') = '' then 'C' else a.BillType end)
		  end as BillType
  from svMstTask a
 inner join #srv b
    on b.CompanyCode = a.CompanyCode
   and b.ProductType = a.ProductType
   and b.BasicModel  = a.BasicModel
   and b.JobType     = a.JobType
  left join svMstTaskPrice c
	on c.CompanyCode = a.CompanyCode
   and c.BranchCode  = b.BranchCode
   and c.ProductType = a.ProductType
   and c.BasicModel  = a.BasicModel
   and c.JobType     = a.JobType
   and c.OperationNo = a.OperationNo
 where 1 = 1
)#task

-- jika svMstTask tidak tepat 1 record, return
if (select count(*) from #task) <> 1 return

select * into #job from(
select a.* from svMstJob a, #task b
 where 1 = 1
   and a.CompanyCode = b.CompanyCode
   and a.ProductType = b.ProductType
   and a.BasicModel  = b.BasicModel
   and a.JobType     = b.JobType
)#job

-- jika svMstJob tidak tepat 1 record, return
if (select count(*) from #job) <> 1 return

-- prepare data svTrnSrvTask yg akan di Insert
declare @JobType varchar(15) set @JobType = (select JobType from #job)

if (left(@JobType,3) = 'FSC' or left(@JobType,3) = 'PDI')
begin
	insert into svTrnSrvTask (CompanyCode, BranchCode, ProductType, ServiceNo, OperationNo, OperationHour, OperationCost, IsSubCon, SubConPrice, PONo, ClaimHour, TypeOfGoods, BillType, SharingTask, TaskStatus, StartService, FinishService, CreatedBy, CreatedDate, LastupdateBy, LastupdateDate, DiscPct)
	select
		 @CompanyCode CompanyCode
		,@BranchCode BranchCode
		,@ProductType ProductType
		,@ServiceNo ServiceNo
		,a.OperationNo
		,a.OperationHour
		,OperationCost = isnull((select top 1 a.RegularLaborAmount
						   from svMstPdiFscRate a, #srv b, #task c, #job d 
						  where 1 = 1
						    and a.CompanyCode = b.CompanyCode
						    and a.ProductType = b.ProductType
						    and a.BasicModel = b.BasicModel
						    and a.IsCampaign = c.IsCampaign
						    and a.TransmissionType = a.TransmissionType
						    and a.PdiFscSeq = d.PdiFscSeq
						    and a.EffectiveDate <= getdate()
						    and a.IsActive = 1
						 order by a.EffectiveDate desc),0)
		,IsSubCon
		,a.LaborCost SubConPrice
		,'' PONo
		,ClaimHour
		,'L' TypeOfGoods
	    , case when isnull(a.BillType, '') = '' then 'C' else a.BillType end as BillType
		,'0' SharingTask
		,'0' TaskStatus
		,null StartService
		,null FinishService
		,b.LastupdateBy CreatedBy
		,b.LastupdateDate CreatedDate
		,b.LastupdateBy
		,b.LastupdateDate
		,isnull((
			select cus.LaborDiscPct
			  from svMstBillingType bil
			 inner join gnMstCustomerProfitCenter cus
				on cus.CompanyCode = bil.CompanyCode
			   and cus.CustomerCode = bil.CustomerCode 
			 where 1 = 1
			   and bil.CompanyCode = @CompanyCode
			   and cus.BranchCode = @BranchCode
			   and cus.ProfitCenterCode = '200'
			   and bil.BillType = 'F'
			), b.LaborDiscPct) as LaborDiscPct
	from #task a, #srv b
end
else if @JobType = 'CLAIM'
begin
	insert into svTrnSrvTask (CompanyCode, BranchCode, ProductType, ServiceNo, OperationNo, OperationHour, OperationCost, IsSubCon, SubConPrice, PONo, ClaimHour, TypeOfGoods, BillType, SharingTask, TaskStatus, StartService, FinishService, CreatedBy, CreatedDate, LastupdateBy, LastupdateDate, DiscPct)
	select
		 @CompanyCode CompanyCode
		,@BranchCode BranchCode
		,@ProductType ProductType
		,@ServiceNo ServiceNo
		,a.OperationNo
		,a.ClaimHour OperationHour
		,a.LaborPrice OperationCost
		,a.IsSubCon
		,a.LaborCost SubConPrice
		,'' PONo
		,a.ClaimHour
		,'L' TypeOfGoods
		,'W' BillType
		,'0' SharingTask
		,'0' TaskStatus
		,null StartService
		,null FinishService
		,b.LastupdateBy CreatedBy
		,b.LastupdateDate CreatedDate
		,b.LastupdateBy
		,b.LastupdateDate
		,b.LaborDiscPct
	from #task a, #srv b
end
else if @JobType = 'REWORK'
begin
	insert into svTrnSrvTask (CompanyCode, BranchCode, ProductType, ServiceNo, OperationNo, OperationHour, OperationCost, IsSubCon, SubConPrice, PONo, ClaimHour, TypeOfGoods, BillType, SharingTask, TaskStatus, StartService, FinishService, CreatedBy, CreatedDate, LastupdateBy, LastupdateDate, DiscPct)
	select
		 @CompanyCode CompanyCode
		,@BranchCode BranchCode
		,@ProductType ProductType
		,@ServiceNo ServiceNo
		,a.OperationNo
		,a.OperationHour
		,a.LaborPrice OperationCost
		,a.IsSubCon
		,a.LaborCost SubConPrice
		,'' PONo
		,ClaimHour
		,'L' TypeOfGoods
		,'I' BillType
		,'0' SharingTask
		,'0' TaskStatus
		,null StartService
		,null FinishService
		,b.LastupdateBy CreatedBy
		,b.LastupdateDate CreatedDate
		,b.LastupdateBy
		,b.LastupdateDate
		,b.LaborDiscPct
	from #task a, #srv b
end
else
begin
	insert into svTrnSrvTask (CompanyCode, BranchCode, ProductType, ServiceNo, OperationNo, OperationHour, OperationCost, IsSubCon, SubConPrice, PONo, ClaimHour, TypeOfGoods, BillType, SharingTask, TaskStatus, StartService, FinishService, CreatedBy, CreatedDate, LastupdateBy, LastupdateDate, DiscPct)
	select
		 @CompanyCode CompanyCode
		,@BranchCode BranchCode
		,@ProductType ProductType
		,@ServiceNo ServiceNo
		,a.OperationNo
		,a.OperationHour
		,a.LaborPrice OperationCost
		,IsSubCon
		,a.LaborCost SubConPrice
		,'' PONo
		,ClaimHour
		,'L' TypeOfGoods
		, case when isnull(a.BillType, '') = '' then 'C' else a.BillType end as BillType
		,'0' SharingTask
		,'0' TaskStatus
		,null StartService
		,null FinishService
		,b.LastupdateBy CreatedBy
		,b.LastupdateDate CreatedDate
		,b.LastupdateBy
		,b.LastupdateDate
		,isnull((
			select top 1 tsk.DiscPct
			  from svMstPackage pkg
			 inner join svMstPackageTask tsk
				on tsk.CompanyCode = pkg.CompanyCode
			   and tsk.PackageCode = pkg.PackageCode
			 inner join svMstPackageContract con
				on con.CompanyCode = pkg.CompanyCode
			   and con.PackageCode = pkg.PackageCode
			 where pkg.CompanyCode = b.CompanyCode
			   and pkg.JobType = b.JobType
			   and tsk.OperationNo = a.OperationNo
			   and con.ChassisCode = b.ChassisCode
			   and con.ChassisNo = b.ChassisNo
			), b.LaborDiscPct) LaborDiscPct
		
	from #task a, #srv b
end
-----------------------------------------------
-- insert default svTrnSrvItem
-----------------------------------------------
select * into #part from(
select a.* from svMstTaskPart a, #task b
 where 1 = 1
   and a.CompanyCode = b.CompanyCode
   and a.ProductType = b.ProductType
   and a.BasicModel  = b.BasicModel
   and a.JobType     = b.JobType
   and a.OperationNo = b.OperationNo
)#part

DECLARE @QueryTemp VARCHAR(MAX)

  declare @tblTemp as table  
	  (  
	  CompanyCode varchar(20),
	  BranchCode varchar(20),
	  PartNo varchar(20),
	  TypeOfGoods varchar(2),
	   RetailPrice decimal(18,2),  
	   PurcDiscPct decimal(18,2) 
	  )
	    
	  	
		set @QueryTemp = 'select c.CompanyCode,c.BranchCode,a.PartNo,a.TypeOfGoods,b.RetailPrice,a.PurcDiscPct from '+ dbo.GetDbMD(@CompanyCode, @BranchCode) +'..spmstitems a
		join '+ dbo.GetDbMD(@CompanyCode, @BranchCode) +'..spMstItemPrice b on a.CompanyCode = b.CompanyCode and a.BranchCode = b.BranchCode and a.PartNo = b.PartNo
		join gnMstCompanyMapping c on a.CompanyCode = c.CompanyMD and a.BranchCode = c.BranchMD'
		          
	  insert into @tblTemp    
	  exec (@QueryTemp)

	  declare @PurchaseDisc as decimal  
	  set @PurchaseDisc = (select DiscPct from gnMstSupplierProfitCenter   
		   where CompanyCode = @CompanyCode   
		   and BranchCode = @BranchCode  
		   and SupplierCode = dbo.GetBranchMD(@CompanyCode, @BranchCode)
		   and ProfitCenterCode = '300')  
	         
	  if (@PurchaseDisc is null) raiserror ('Profit Center 300 belum tersetting pada Supplier tersebut!!!',16,1);         

insert into svTrnSrvItem (CompanyCode, BranchCode, ProductType, ServiceNo, PartNo, PartSeq, DemandQty, SupplyQty, ReturnQty, CostPrice, RetailPrice, TypeOfGoods, BillType, SupplySlipNo, SupplySlipDate, SSReturnNo, SSReturnDate, CreatedBy, CreatedDate, LastupdateBy, LastupdateDate, DiscPct)
select 
	 @CompanyCode CompanyCode
	,@BranchCode BranchCode
	,@ProductType ProductType
	,@ServiceNo ServiceNo
	,a.PartNo
	,(row_number() over (order by a.PartNo)) PartSeq
	,a.Quantity DemandQty
	,'0' SupplyQty
	,'0' ReturnQty
	, case when ISNULL(d.PurcDiscPct,0) = 0 
	then (d.RetailPrice - (d.RetailPrice *(@PurchaseDisc * 0.01))) 
	else (d.RetailPrice - (d.RetailPrice * (d.PurcDiscPct * 0.01))) end CostPrice
	,case rtrim(a.BillType) when 'F' then a.RetailPrice else d.RetailPrice end RetailPrice
	,d.TypeOfGoods
	,case when exists (
			select pkg.CompanyCode, pkg.PackageCode, pkg.JobType
			  from svMstPackage pkg
			 inner join svMstPackagePart prt
				on prt.CompanyCode = pkg.CompanyCode
			   and prt.PackageCode = pkg.PackageCode
			 inner join svMstPackageContract con
				on con.CompanyCode = pkg.CompanyCode
			   and con.PackageCode = pkg.PackageCode
			 inner join #srv srv
				on srv.CompanyCode = con.CompanyCode
			   and srv.ChassisCode = con.ChassisCode
			   and srv.ChassisNo = con.ChassisNo
			 where pkg.CompanyCode = b.CompanyCode
			   and pkg.JobType = b.JobType
			   and prt.PartNo = a.PartNo
		) then 'P' else 
		(case when isnull(a.BillType, '') = '' then 'C' else 
			(case when substring(@ServiceBookNo, 1, 2) >= 'EJ' and f.PartName like '%OIL FILTER%' and (select PdiFscSeq from #job) = 1 then 'C' else 		
			
				(case when substring(@ServiceBookNo, 1, 2) >= 'EJ' and f.PartName like '%ENGINE%' and (select PdiFscSeq from #job) = 3 then 'C' else 		
				a.BillType 
				end)
			
			end)		
		end)
		  end as BillType
	,null SupplySlipNo
	,null SupplySlipDate
	,null SSReturnNo
	,null SSReturnDate
	,b.LastupdateBy CreatedBy
	,b.LastupdateDate CreatedDate
	,b.LastupdateBy
	,b.LastupdateDate
	,isnull((
		select top 1 prt.DiscPct
		  from svMstPackage pkg
		 inner join svMstPackagePart prt
			on prt.CompanyCode = pkg.CompanyCode
		   and prt.PackageCode = pkg.PackageCode
		 inner join svMstPackageContract con
			on con.CompanyCode = pkg.CompanyCode
		   and con.PackageCode = pkg.PackageCode
		 inner join #srv srv
			on srv.CompanyCode = con.CompanyCode
		   and srv.ChassisCode = con.ChassisCode
		   and srv.ChassisNo = con.ChassisNo
		 where pkg.CompanyCode = b.CompanyCode
		   and pkg.JobType = b.JobType
		   and prt.PartNo = a.PartNo
		), 
		(case when rtrim(a.BillType) = 'F' and rtrim(e.ParaValue) = 'SPAREPART' then 0
		      when rtrim(a.BillType) = 'F' then 0
		      when rtrim(e.ParaValue) = 'SPAREPART' then (select top 1 PartDiscPct from #srv) 
			  else (select top 1 MaterialDiscPct from #srv) end)
		) as DiscPct
  from #part a
  left join #task b
    on b.CompanyCode = a.CompanyCode
   and b.ProductType = a.ProductType
   and b.BasicModel  = a.BasicModel
   and b.JobType     = a.JobType
   and b.OperationNo = a.OperationNo
   LEFT join @tblTemp d
   on d.CompanyCode = a.CompanyCode
   and d.BranchCode = @BranchCode
   and d.PartNo = a.PartNo
  --left join spMstItemPrice c
  --  on c.CompanyCode = a.CompanyCode
  -- and c.BranchCode  = @BranchCode
  -- and c.PartNo      = a.PartNo
  --left join spMstItems d
  --  on d.CompanyCode = a.CompanyCode
  -- and d.BranchCode  = @BranchCode
  -- and d.PartNo      = a.PartNo
  left join gnMstLookupDtl e
    on e.CompanyCode = d.CompanyCode
   and e.CodeID = 'GTGO'
   and e.LookupValue = d.TypeOfGoods
   left join spMstItemInfo f
	on f.CompanyCode = a.CompanyCode
	and f.PartNo = a.PartNo
 where 1 = 1
   and b.CompanyCode = a.CompanyCode
   and b.ProductType = a.ProductType
   and b.BasicModel  = a.BasicModel
   and b.JobType     = a.JobType
   and b.OperationNo = a.OperationNo
exec uspfn_SvInsertDefaultTaskMovement @CompanyCode, @BranchCode, @ProductType, @ServiceNo, @UserID

drop table #srv
drop table #task
drop table #part
drop table #job
GO
create procedure [dbo].[usprpt_GnGenerateCsvSkemaTaxOut]
	@CompanyCode varchar(15)
	,@BranchCode varchar(15)
	,@PeriodMonth int
	,@PeriodYear int
	,@ProductType varchar(2)
	,@table varchar(1)=1
as

--select @CompanyCode=N'6354401',@BranchCode=N'%',@PeriodMonth=10,@PeriodYear=2011,@ProductType=N'4W'

-- 0= Not Recalculate PPN, 1= Recalculate PPN (For FPJ Gabungan)
declare @IsRecalPPN as bit
set @IsRecalPPN=0

if @table = 1
begin
	if (select count(ParaValue) from gnMstLookUpDtl where CodeID='CPPN' and LookUpValue='STATUS') > 0
	set @IsRecalPPN= (select convert(bit,ParaValue) from gnMstLookUpDtl where CodeID='CPPN' and LookUpValue='STATUS')

		SELECT FK
			, KD_JENIS_TRANSAKSI
			, FG_PENGGANTI
			, NOMOR_FAKTUR
			, MASA_PAJAK
			, TAHUN_PAJAK
			, TANGGAL_FAKTUR
			, NPWP
			, NAMA_LAWAN_TRANSAKSI
			, ALAMAT_LENGKAP
			, JUMLAH_DPP
			, JUMLAH_PPN
			, JUMLAH_PPNBM
			, ID_KETERANGAN_TAMBAHAN
			, FG_UANG_MUKA
			, UANG_MUKA_DPP
			, UANG_MUKA_PPN
			, UANG_MUKA_PPNBM
			, REFERENSI
			, CUSTOMERCODE
			, FPJNO
		FROM (
			SELECT
				'FK' FK
				, LEFT(TaxNo,2) KD_JENIS_TRANSAKSI
				, 0 FG_PENGGANTI
				, REPLACE(REPLACE(TaxNo, '.', ''), '-', '') NOMOR_FAKTUR
				, (case when len(PeriodMonth) = 1 then '0' + convert(varchar, PeriodMonth, 1) + '0' +  convert(varchar, PeriodMonth, 1) 
					else convert(varchar, PeriodMonth, 2)+convert(varchar, PeriodMonth, 2) end) MASA_PAJAK
				, PeriodYear TAHUN_PAJAK
				, CONVERT(VARCHAR, TaxDate, 103) TANGGAL_FAKTUR
				, '''' + REPLACE(REPLACE(NPWP, '.', ''), '-', '') NPWP
				, CustomerName NAMA_LAWAN_TRANSAKSI
				, CASE WHEN LEFT(FPJNO,3) = 'FPJ' THEN 
				(SELECT Address1 + ' ' + Address2 FROM SPTRNSFPJINFO WHERE COMPANYCODE = gnTaxOut.COMPANYCODE AND BRANCHCODE = gnTaxOut.BRANCHCODE AND FPJNO =	gnTaxOut.FPJNO) ELSE  
				  CASE WHEN LEFT(FPJNO,3) = 'FPS' THEN 
				(SELECT Address1 + ' ' + Address2 FROM svTrnFakturPajak WHERE COMPANYCODE = gnTaxOut.COMPANYCODE AND BRANCHCODE = gnTaxOut.BRANCHCODE AND FPJNO = gnTaxOut.FPJNO) ELSE				(SELECT Address1 + ' ' + Address2 FROM GNMSTCUSTOMER WHERE COMPANYCODE = gnTaxOut.COMPANYCODE AND BRANCHCODE = gnTaxOut.BRANCHCODE 
				AND CUSTOMERCODE = gnTaxOut.CUSTOMERCODE)	
				  END END ALAMAT_LENGKAP
				, ISNULL(DPPAmt, 0) JUMLAH_DPP
				, ISNULL(PPNAmt, 0) JUMLAH_PPN
				, ISNULL(PPNBmAmt, 0) JUMLAH_PPNBM
				, '' ID_KETERANGAN_TAMBAHAN
				, 0 FG_UANG_MUKA
				, 0 UANG_MUKA_DPP
				, 0 UANG_MUKA_PPN
				, 0 UANG_MUKA_PPNBM
				, '' REFERENSI
				, CUSTOMERCODE
				, FPJNO
			FROM 
				gnTaxOut 
			WHERE
				CompanyCode = @CompanyCode
				AND BranchCode like @BranchCode
				AND ProductType = @ProductType
				AND PeriodYear = @PeriodYear
				AND PeriodMonth = @PeriodMonth
				AND IsPKP = 1
)A
end
if @table = 2
begin
	select  'LT' LT, NPWPNo NPWP, CustomerName NAMA, Address1 + Address2 JALAN, '-' BLOK, '-' NOMOR, '-' RT, '-' RW, 
	KecamatanDistrik KECAMATAN, KelurahanDesa KELURAHAN, KotaKabupaten KABUPATEN, 
	case when isnull(ProvinceCode,'') = '' then '-' else (select top 1 lookupvaluename from gnmstlookupdtl where LookUpValue = ProvinceCode )end PROPINSI, 
	ZipNo KODE_POS, PhoneNo NOMOR_TELEPON, CUSTOMERCODE
	from gnMstCustomer 
	where CustomerCode in 
	(select distinct CustomerCode from gnTaxOut WHERE
					CompanyCode = @CompanyCode
					AND BranchCode like @BranchCode
					AND ProductType = @ProductType
					AND PeriodYear = @PeriodYear
					AND PeriodMonth = @PeriodMonth
					AND IsPKP = 1)
end
if @table = 3
begin
		select * from (
						select 'OF' [OF]
						, PartNo KODE_OBJEK
						, (select replace(rtrim(Partname),',',' ') from spmstiteminfo where companycode = a.companycode and partno = a.partno) NAMA
						, isnull ((convert(decimal(12,2),a.retailprice)), 0) HARGA_SATUAN
						, isnull ((convert(decimal(12,2),a.QtyBill)), 0) JUMLAH_BARANG
						, isnull ((convert(decimal(12,2),a.SalesAmt)), 0)HARGA_TOTAL
						, isnull ((convert(decimal(12,2),a.DiscAmt)), 0) DISKON
						, isnull ((convert(decimal(12,2),a.NetSalesAmt)), 0) DPP
						, isnull ((convert(decimal(12,2),a.PPNAmt)), 0) PPN
						, 0 TARIF_PPNBM
						, 0 PPNBM
						, b.CUSTOMERCODE 
						, b.FPJNO
						from spTrnSInvoicedtl a
						inner join (select companycode, branchcode, fpjno, customercode, referenceNo
									from gntaxout 
									 WHERE CompanyCode = @CompanyCode
											AND BranchCode = @BranchCode
											AND ProductType = @ProductType
											AND PeriodYear = @PeriodYear
											AND PeriodMonth = @PeriodMonth
											AND IsPKP = 1
											AND left(fpjno,3)='FPJ'
									) b
							on a.companycode = b.companycode and a.branchcode = b.branchcode
								and a.InvoiceNo = b.referenceNo
						union all
						select 'OF' [OF]
						, SalesModelCode KODE_OBJEK
						, (select SalesModelDesc from ommstmodel where companycode = a.companycode and salesmodelcode = a.salesmodelcode) NAMA
						, isnull ((convert(decimal(12,2),a.BeforeDiscDPP)), 0) HARGA_SATUAN
						, isnull ((convert(decimal(12,2),a.Quantity)), 0) JUMLAH_BARANG
						, isnull ((convert(decimal(12,2),a.BeforeDiscDPP * a.Quantity)),0) HARGA_TOTAL
						, isnull ((convert(decimal(12,2),a.DiscExcludePPn * a.Quantity)), 0) DISKON
						, isnull ((convert(decimal(12,2),a.AfterDiscTotal * a.Quantity)), 0) DPP
						, isnull ((convert(decimal(12,2),a.AfterDiscPPn * a.Quantity)), 0) PPN
						, 0 TARIF_PPNBM
						, 0 PPNBM
						, b.CUSTOMERCODE 
						, b.FPJNO
						from omTrSalesInvoicemodel a
						inner join (select companycode, branchcode, fpjno, customercode, referenceNo
									from gntaxout 
									 WHERE CompanyCode = @CompanyCode
											AND BranchCode = @BranchCode
											AND ProductType = @ProductType
											AND PeriodYear = @PeriodYear
											AND PeriodMonth = @PeriodMonth
											AND IsPKP = 1
											AND left(fpjno,3)<>'FPJ' AND left(fpjno,3)<>'FPS' 
									) b
							on a.companycode = b.companycode and a.branchcode = b.branchcode
								and a.InvoiceNo = b.referenceNo	
						union all 
						select 'OF' [OF]
						, rtrim(PartNo) KODE_OBJEK
						, (select replace(rtrim(Partname),',',' ') from spmstiteminfo where companycode = a.companycode and partno = a.partno) NAMA
						, isnull ((convert(decimal(12,2),a.RetailPrice)), 0) HARGA_SATUAN
						, isnull ((convert(decimal(12,2),a.SupplyQty)), 0) JUMLAH_BARANG
						, isnull ((convert(decimal(12,2),a.RetailPrice * a.SupplyQty)), 0)HARGA_TOTAL
						, isnull ((convert(decimal(12,2),((a.RetailPrice * a.SupplyQty)*DiscPct)/100 )), 0) DISKON
						, isnull ((convert(decimal(12,2),(a.RetailPrice * a.SupplyQty) - (((a.RetailPrice * a.SupplyQty)*DiscPct)/100))), 0) DPP
						, isnull ((convert(decimal(12,2),(a.RetailPrice * a.SupplyQty) - (((a.RetailPrice * a.SupplyQty)*DiscPct)/100)* 0.1)), 0) PPN
						, 0 TARIF_PPNBM
						, 0 PPNBM
						, b.CUSTOMERCODE 
						, b.FPJNO
						from svTrnInvItem a
						inner join (select companycode, branchcode, fpjno, customercode, referenceNo
									from gntaxout 
									 WHERE CompanyCode = @CompanyCode
											AND BranchCode like @BranchCode
											AND ProductType = @ProductType
											AND PeriodYear = @PeriodYear
											AND PeriodMonth = @PeriodMonth
											AND IsPKP = 1
											AND left(fpjno,3)='FPS' 
									) b
							on a.companycode = b.companycode and a.branchcode = b.branchcode
								and a.InvoiceNo = b.referenceNo	

				) a
	 order by FPJNO
end

go
DROP VIEW [dbo].[SvSaView]
GO

CREATE view [dbo].[SvSaView]
as 
select a.CompanyCode, a.BranchCode, a.EmployeeID, a.EmployeeName  from GnMstEmployee a
where a.TitleCode IN ('3','7')
   AND PersonnelStatus = '1'

GO
/*
> Menu Id dan Menu Caption Terserah Pak Hasim, 
  Kalau kurang tepat diganti aja pak untuk menu id dan menu caption nya. 
> Untuk Sp ada di tbsdmsap01 database BITJKT 
  Nama Spnya : usprpt_GnGenerateCsvSkemaTaxOut
Thanks :) 
*/
if not exists (Select * from SysMenuDms where menuid = 'GnRpSkemaTaxOut')
begin 
	INSERT INTO [dbo].[SysMenuDms]
			   ([MenuId]
			   ,[MenuCaption]
			   ,[MenuHeader]
			   ,[MenuIndex]
			   ,[MenuLevel]
			   ,[MenuUrl]
			   ,[MenuIcon])
		 VALUES
			   ('GnRpSkemaTaxOut'
			   ,'Laporan eFaktur Pajak Keluaran'
			   ,'taxReport'
			   ,'7'
			   ,'2'
			   ,'report/skemataxout'
			   ,'')
end


go
/****** Object:  StoredProcedure [dbo].[uspfn_GenerateSSPickingslipNew]    Script Date: 6/16/2015 9:33:32 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER procedure [dbo].[uspfn_GenerateSSPickingslipNew]
	@CompanyCode	VARCHAR(MAX),
	@BranchCode		VARCHAR(MAX),
	@JobOrderNo		VARCHAR(MAX),
	@ProductType	VARCHAR(MAX),
	@CustomerCode	VARCHAR(MAX),
	@TransType		VARCHAR(MAX),
	@UserID			VARCHAR(MAX),
	@DocDate		DATETIME
AS
BEGIN

--declare	@CompanyCode	VARCHAR(MAX)
--declare	@BranchCode		VARCHAR(MAX)
--declare	@JobOrderNo		VARCHAR(MAX)
--declare	@ProductType	VARCHAR(MAX)
--declare	@CustomerCode	VARCHAR(MAX)
--declare	@TransType		VARCHAR(MAX)
--declare	@UserID			VARCHAR(MAX)
--declare	@DocDate		DATETIME

--set	@CompanyCode	= '6156401000'
--set	@BranchCode		= '6156401001'
--set	@JobOrderNo		= 'SPK/15/001833'
--set	@ProductType	= '4W'
--set	@CustomerCode	= '000003'
--set	@TransType		= '20'
--set	@UserID			= 'TRAININGZZZ'
--set	@DocDate		= '3/12/2015 9:47:01 AM'


--exec uspfn_GenerateSSPickingslipNew '6006400001','6006400101','SPK/14/101589','4W','2105885','20','ga','3/2/2015 4:03:01 PM'
--================================================================================================================================
-- TABLE MASTER
--================================================================================================================================
-- Temporary for Item --
------------------------
SELECT * INTO #Item FROM (
SELECT a.CompanyCode
	, a.BranchCode
	, a.ProductType
	, a.ServiceNo
	, a.RetailPrice
	, a.PartNo
	, a.Billtype
	, SUM(ISNULL(a.DemandQty, 0) - (ISNULL(a.SupplyQty, 0))) QtyOrder
FROM svTrnSrvItem a WITH (NOLOCK, NOWAIT)
INNER JOIN svTrnService b ON b.CompanyCode = a.CompanyCode
	AND b.BranchCode = a.BranchCode
	AND b.ProductType = a.ProductType
	AND b.ServiceNo = a.ServiceNo
	AND b.JobOrderNo = @JobOrderNo
WHERE a.CompanyCode = @CompanyCode 
	AND a.BranchCode = @BranchCode 
	AND a.ProductType = @ProductType 
GROUP BY a.CompanyCode, a.BranchCode, a.ProductType
	, a.ServiceNo, a.PartNo, a.RetailPrice, a.BillType ) #Item 

DECLARE @CompanyMD AS VARCHAR(15)
DECLARE @BranchMD AS VARCHAR(15)
DECLARE @WarehouseMD AS VARCHAR(15)

SET @CompanyMD = (SELECT CompanyMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)
SET @BranchMD = (SELECT BranchMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)
SET @WarehouseMD = (SELECT WarehouseMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)

SELECT * INTO #SrvOrder FROM (
SELECT DISTINCT(a.CompanyCode) 
    , a.BranchCode
	, a.ProductType
	, a.ServiceNo
	, a.PartNo
    , (SELECT Paravalue FROM gnMstLookUpDtl WHERE CompanyCode = a.CompanyCode AND CodeID = 'GTGO' AND LookUpValue = a.TypeOfGoods) TipePart
    , (SELECT PartName FROM spMstItemInfo WHERE CompanyCode = a.CompanyCode AND PartNo = a.PartNo) PartName
	, a.RetailPrice
	, a.CostPrice
    , a.TypeOfGoods
    , a.BillType
	, SUM(a.QtyOrder) QtyOrder
    , 0 QtySupply
    , 0 QtyBO
    , (SUM(a.QtyOrder) * a.RetailPrice) * ((100 - a.PartDiscPct)/100) NetSalesAmt
    , a.PartDiscPct DiscPct
FROM
(
	SELECT
		DISTINCT(a.CompanyCode) 
		, a.BranchCode
		, a.ProductType
		, a.ServiceNo
		, a.PartNo
		, a.RetailPrice
		, a.CostPrice
		, a.TypeOfGoods
		, a.BillType
		, ISNULL(Item.QtyOrder,0) AS QtyOrder
		, a.DiscPct PartDiscPct 
	FROM
		svTrnSrvItem a WITH (NOLOCK, NOWAIT)
		LEFT JOIN svTrnService b WITH (NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode
			AND a.BranchCode = b.BranchCode	
			AND a.ProductType = b.ProductType
			AND a.ServiceNo = b.ServiceNo
		LEFT JOIN #Item Item ON Item.CompanyCode = a.CompanyCode 
			AND Item.BranchCode = a.BranchCode 
			AND Item.ProductType = a.ProductType 
			AND Item.ServiceNo = a.ServiceNo 
			AND Item.PartNo = a.PartNo 
			AND Item.RetailPrice = a.RetailPrice 
			AND Item.BillType = a.Billtype
		--LEFT JOIN SpMstItemPrice c WITH (NOLOCK, NOWAIT) ON a.CompanyCode = c.CompanyCode 
		--	AND a.BranchCode = c.BranchCode 
		--	AND a.PartNo = c.PartNo
	WHERE
		a.CompanyCode = @CompanyCode
		AND a.BranchCode = @BranchCode
		AND a.ProductType = @ProductType
		AND Item.QtyOrder > 0
		AND JobOrderNo = @JobOrderNo
		AND (a.SupplySlipNo is null OR a.SupplySlipNo = '')
) a
GROUP BY
	a.CompanyCode
	, a.BranchCode
	, a.ProductType
	, a.ServiceNo
	, a.PartNo
	, a.RetailPrice
	, a.CostPrice
    , a.TypeOfGoods
    , a.BillType
    , a.PartDiscPct 
) #SrvOrder

select * from #srvorder

--================================================================================================================================
-- INSERT TABLE SpTrnSORDHdr AND SpTrnSORDDtl
--================================================================================================================================
DECLARE @MaxDocNo			INT
DECLARE	@MaxPickingList		INT
DECLARE @TempDocNo			VARCHAR(MAX)
DECLARE @TempPickingList	VARCHAR(MAX)
DECLARE @TypeOfGoods		VARCHAR(MAX)
DECLARE @DefaultDate		DATETIME

SET @DefaultDate = '1900-01-01 00:00:00.000'

--===============================================================================================================================
-- LOOPING BASED ON THE TYPE OF GOODS
-- ==============================================================================================================================
DECLARE db_cursor CURSOR FOR
SELECT DISTINCT TypeOfGoods FROM #SrvOrder
WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND ProductType = @ProductType 

OPEN db_cursor
FETCH NEXT FROM db_cursor INTO @TypeOfGoods

WHILE @@FETCH_STATUS = 0
BEGIN

--===============================================================================================================================
-- INSERT HEADER
-- ==============================================================================================================================
SET @MaxDocNo = ISNULL((SELECT DocumentSequence + 1 FROM GnMstDocument WHERE 
	CompanyCode = @CompanyCode
		AND BranchCode = @BranchCode
		AND DocumentType = 'SSS' 
		AND ProfitCenterCode = '300' 
		AND DocumentYear = YEAR(GetDate())),0)

SET @TempDocNo = ISNULL((SELECT 'SSS/' + RIGHT(YEAR(GETDATE()),2) +'/' + RIGHT(1000000 + CONVERT(VARCHAR, @MaxDocNo, 1), 6)),'SSS/YY/XXXXXX')

INSERT INTO SpTrnSORDHdr
([CompanyCode]
           ,[BranchCode]
           ,[DocNo]
           ,[DocDate]
           ,[UsageDocNo]
           ,[UsageDocDate]
           ,[CustomerCode]
           ,[CustomerCodeBill]
           ,[CustomerCodeShip]
           ,[isBO]
           ,[isSubstitution]
           ,[isIncludePPN]
           ,[TransType]
           ,[SalesType]
           ,[IsPORDD]
           ,[OrderNo]
           ,[OrderDate]
           ,[TOPCode]
           ,[TOPDays]
           ,[PaymentCode]
           ,[PaymentRefNo]
           ,[TotSalesQty]
           ,[TotSalesAmt]
           ,[TotDiscAmt]
           ,[TotDPPAmt]
           ,[TotPPNAmt]
           ,[TotFinalSalesAmt]
           ,[isPKP]
           ,[ExPickingSlipNo]
           ,[ExPickingSlipDate]
           ,[Status]
           ,[PrintSeq]
           ,[TypeOfGoods]
           ,[isDropsign]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastUpdateBy]
           ,[LastUpdateDate]
           ,[isLocked]
           ,[LockingBy]
           ,[LockingDate])

SELECT 
	@CompanyCode CompanyCode
	, @BranchCode BranchCode
	, @TempDocNo DocNo 
	, @DocDate DocDate
	, @JobOrderNo UsageDocNo
	, (SELECT JobOrderDate FROM SvTrnService WHERE 1 =1 AND CompanyCode = @CompanyCode AND BranchCode = @BranchCode 
		AND ProductType = @ProductType AND JobOrderNo = @JobOrderNo) UsageDocDate
	, (SELECT CustomerCode FROM SvTrnService WHERE 1 = 1AND CompanyCode = @CompanyCode AND BranchCode = @BranchCode 
		AND ProductType = @ProductType AND JobOrderNo = @JobOrderNo) CustomerCode
	, (SELECT CustomerCodeBill FROM SvTrnService WHERE 1 = 1 AND CompanyCode = @CompanyCode AND BranchCode = @BranchCode 
		AND ProductType = @ProductType AND JobOrderNo = @JobOrderNo) CustomerCodeBill
	, (SELECT CustomerCode FROM SvTrnService WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode 
		AND ProductType = @ProductType AND JobOrderNo = @JobOrderNo) CustomerCodeShip
	, CONVERT(BIT, 0) isBO
	, CONVERT(BIT, 0) isSubstitution
	, CONVERT(BIT, 1) isIncludePPN
	, @TransType TransType
	, '2' SalesType
	, CONVERT(BIT, 0) isPORDD
	, @JobOrderNo OrderNo
	, @DocDate OrderDate
	, ISNULL((SELECT TOPCode FROM GnMstCustomerProfitCenter WITH (NOLOCK, NOWAIT) WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
		AND ProfitCenterCode = '300' AND CustomerCode = @CustomerCode),'W') TOPCode
	, ISNULL((SELECT ParaValue FROM GnMstLookUpDtl WITH (NOLOCK, NOWAIT) WHERE CompanyCode = @CompanyCode AND CodeID = 'TOPC' AND 
		LookupValue IN 
		(SELECT TOPCode FROM GnMstCustomerProfitCenter WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
			AND ProfitCenterCode = '300' AND CustomerCode = @CustomerCode)
	  ),0) TOPDays
	, ISNULL((SELECT PaymentCode FROM GnMstCustomerProfitCenter WITH (NOLOCK, NOWAIT) WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
		AND ProfitCenterCode = '300' AND CustomerCode = @CustomerCode),'W') PaymentCode
	, '' PaymentReffNo
	, 0 TotSalesQty
	, 0 TotSalesAmt
	, 0 TotDiscAmt
	, 0 TotDPPAmt
	, 0 TotPPNAmt
	, 0 TotFinalSalesAmt
	, CONVERT(BIT, 0) isPKP
	, NULL ExPickingSlipNo
	, NULL ExPickingSlipDate
	, '4' Status
	, 0 PrintSeq
	, @TypeOfGoods TypeOfGoods
	, NULL IsDropSign
	, @UserID CreatedBY
	, GetDate() CreatedDate
	, @UserID LastUpdateBy
	, GetDate() LastUpdateDate
	, NULL isLocked
	, NULL LockingBy
	, NULL LockingDate


UPDATE GnMstDocument
SET DocumentSequence = DocumentSequence + 1
	, LastUpdateDate = GetDate()
	, LastUpdateBy = @UserID
WHERE
	CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND DocumentPrefix = 'SSS'
	AND ProfitCenterCode = '300'
	AND DocumentYear = Year(GetDate())

--===============================================================================================================================
-- INSERT DETAIL
-- ==============================================================================================================================
DECLARE @DbMD AS VARCHAR(15)
SET @DbMD = (SELECT DbMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)

declare @TempAvailStock as table
(
	PartNo varchar(50),
	AvailStock decimal
)

DECLARE @Query AS VARCHAR(MAX)
--SET @Query = 
--'SELECT PartNo, (Onhand - AllocationSP - AllocationSL - AllocationSR - ReservedSP - ReservedSL - ReservedSR) AvailStock
--FROM ' + @DbMD + '..SpMstItemLoc WITH (NOLOCK, NOWAIT) 
--WHERE CompanyCode = '+''''+@CompanyMD+''''+' AND BranchCode ='+''''+@BranchMD +''''+' AND WarehouseCode = '+''''+@WarehouseMD+''''+''

--INSERT INTO #TempAvailStock

SET @Query = 
'SElect * into #TempAvailStock from (SELECT PartNo, (Onhand - AllocationSP - AllocationSL - AllocationSR - ReservedSP - ReservedSL - ReservedSR) AvailStock
FROM ' + @DbMD + '..SpMstItemLoc WITH (NOLOCK, NOWAIT) 
WHERE CompanyCode = '+''''+@CompanyMD+''''+' AND BranchCode ='+''''+@BranchMD +''''+' AND WarehouseCode = '+''''+@WarehouseMD+''''+')#TempAvailStock

INSERT INTO SpTrnSORDDtl 
(
	[CompanyCode] ,
	[BranchCode] ,
	[DocNo] ,
	[PartNo] ,
	[WarehouseCode] ,
	[PartNoOriginal] ,
	[ReferenceNo] ,
	[ReferenceDate] ,
	[LocationCode] ,
	[QtyOrder] ,
	[QtySupply] ,
	[QtyBO] ,
	[QtyBOSupply] ,
	[QtyBOCancel] ,
	[QtyBill] ,
	[RetailPriceInclTax] ,
	[RetailPrice] ,
	[CostPrice] ,
	[DiscPct] ,
	[SalesAmt] ,
	[DiscAmt] ,
	[NetSalesAmt] ,
	[PPNAmt] ,
	[TotSalesAmt] ,
	[MovingCode] ,
	[ABCClass] ,
	[ProductType] ,
	[PartCategory] ,
	[Status] ,
	[CreatedBy] ,
	[CreatedDate] ,
	[LastUpdateBy] ,
	[LastUpdateDate] ,
	[StockAllocatedBy] ,
	[StockAllocatedDate] ,
	[FirstDemandQty] )
SELECT
	''' + @CompanyCode +''' CompanyCode
	, ''' + @BranchCode +''' BranchCode
	, ''' + @TempDocNo +''' DocNo 
	, a.PartNo
	, ''00'' WarehouseCode
	, a.PartNo PartNoOriginal
	, ''' + @JobOrderNo +''' ReferenceNo
	, (SELECT JobOrderDate FROM SvTrnService WHERE 1 =1 AND CompanyCode = ''' + @CompanyCode +''' AND BranchCode = ''' + @BranchCode +'''
		AND ProductType = ''' + @ProductType +''' AND JobOrderNo = ''' + @JobOrderNo +''' ) ReferenceDate
	, (SELECT distinct LocationCode FROM ' + @DbMD +'..SpMstItemLoc WHERE CompanyCode = ''' + @CompanyMD +''' AND BranchCode = ''' + @BranchMD +''' AND WarehouseCode = ''00''
		AND PartNo = a.PartNo ) LocationCode
	, a.QtyOrder
	, CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN a.QtyOrder 
		ELSE ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0)
		END AS QtySupply
	, CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN 0 
		ELSE a.QtyOrder - ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0)
		END AS QtyBO
	, 0 QtyBOSupply
	, CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN 0 
		ELSE a.QtyOrder - ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0)
		END AS QtyBOCancel
	, 0 QtyBill
	, a.RetailPrice + FLOOR(a.RetailPrice * 10 /100) RetailPriceIncltax
	, a.RetailPrice
	, a.CostPrice
	, a.DiscPct
	, CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN a.QtyOrder * a.RetailPrice
		ELSE ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) * a.RetailPrice
		END AS SalesAmt
	, CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN floor((a.QtyOrder * a.RetailPrice) * a.DiscPct/100)
		ELSE floor((ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) * a.RetailPrice) * a.DiscPct/100)
		END AS DiscAmt
	, CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN floor((a.QtyOrder * a.RetailPrice)- ((a.QtyOrder * a.RetailPrice) * a.DiscPct/100))
		ELSE floor((ISNULL((SELECT AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) * a.RetailPrice) - 
			(ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) * a.RetailPrice * a.DiscPct/100))
		END AS NetSalesAmt
	, 0 PPNAmt
	,  CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN floor((a.QtyOrder * a.RetailPrice)- ((a.QtyOrder * a.RetailPrice) * a.DiscPct/100))
		ELSE floor((ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) * a.RetailPrice) - 
			(ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) * a.RetailPrice * a.DiscPct/100))
		END AS TotSalesAmt
	, (SELECT distinct MovingCode FROM '+ @DbMD +'..SpMstItems WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyMD +''' AND BranchCode = '''+ @BranchMD +''' AND PartNo = a.PartNo) 
		MovingCode
	, (SELECT distinct ABCClass FROM '+ @DbMD +'..SpMstItems WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyMD +''' AND BranchCode = '''+ @BranchMD +''' AND PartNo = a.PartNo) 
		ABCClass
	, '''+ @ProductType +''' ProductType
	, (SELECT distinct PartCategory FROM '+ @DbMD +'..SpMstItems WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyMD +'''  AND BranchCode = '''+ @BranchMD +''' AND PartNo = a.PartNo) 
		PartCategory
	, ''2'' Status
	, '''+ @UserID +''' CreatedBy
	, '''+ Convert(varchar,GetDate()) +''' CreatedDate
	, '''+ @UserID +''' LastUpdateBy
	, '''+ Convert(varchar,GetDate()) +''' LastUpdateDate
	, '''+ @UserID +''' StockAllocatedBy
	, '''+ Convert(varchar,GetDate()) +''' StockAllocatedDate
	, a.QtyOrder FirstDemandQty
FROM #SrvOrder a
WHERE a.TypeOfGoods = '+@TypeOfGoods +'


select top 10 * from spTrnSORDDtl order by createddate desc
--===============================================================================================================================
-- INSERT SO SUPPLY
-- ==============================================================================================================================

SELECT * INTO #TempSOSupply FROM (
SELECT
	'''+ @CompanyCode +''' CompanyCode
	, '''+ @BranchCode +''' BranchCode
	, '''+ @TempDocNo +''' DocNo 
	, 0 SupSeq
	, a.PartNo 
	, a.PartNo PartNoOriginal
	, '''' PickingSlipNo	
	, '''+ @JobOrderNo +''' ReferenceNo
	, '''+ CONVERT(varchar, @DefaultDate )+''' ReferenceDate
	, ''00'' WarehouseCode
	, (SELECT distinct LocationCode FROM '+ @DbMD+'..SpMstItemLoc WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyMD +''' AND BranchCode = '''+ @BranchMD+''' AND WarehouseCode = ''00''
		AND PartNo = a.PartNo) LocationCode
	, CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN a.QtyOrder 
		ELSE ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0)
		END AS QtySupply
	, 0 QtyPicked
	, 0 QtyBill
	, a.RetailPrice + FLOOR(a.RetailPrice *10 /100) RetailPriceIncltax
	, a.RetailPrice
	, a.CostPrice
	, a.DiscPct
	, (SELECT distinct MovingCode FROM '+ @DbMD+'..SpMstItems WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyMD +''' AND BranchCode = '''+ @BranchMD +''' AND PartNo = a.PartNo) 
		MovingCode
	, (SELECT distinct ABCClass FROM '+ @DbMD +'..SpMstItems WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyMD +''' AND BranchCode = '''+ @BranchMD +''' AND PartNo = a.PartNo) 
		ABCClass
	, '''+ @ProductType +'''ProductType
	, (SELECT distinct PartCategory FROM '+ @DbMD +'..SpMstItems WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyMD +''' AND BranchCode = '''+ @BranchMD +''' AND PartNo = a.PartNo) 
		PartCategory
	, ''1'' Status
	, '''+ @UserID +''' CreatedBy
	, '''+ Convert(varchar,GetDate()) +''' CreatedDate
	, '''+ @UserID +''' LastUpdateBy
	, '''+ Convert(varchar,GetDate()) +''' LastUpdateDate
	, '''+ @UserID +''' StockAllocatedBy
	, '''+ Convert(varchar,GetDate()) +''' StockAllocatedDate
FROM #SrvOrder a
WHERE a.CompanyCode = '''+ @CompanyCode +''' AND a.BranchCode = '''+ @BranchCode +''' 
and a.TypeOfGoods = '+ @TypeOfGoods +'
)#TempSOSupply

INSERT INTO SpTrnSOSupply SELECT 
	CompanyCode,BranchCode,DocNo,SupSeq,PartNo,PartNoOriginal
	, ROW_NUMBER() OVER(ORDER BY PartNo) PTSeq,PickingSlipNo
	, ReferenceNo,ReferenceDate,WarehouseCode,LocationCode
	, QtySupply,QtyPicked,QtyBill,RetailPriceIncltax,RetailPrice,CostPrice
	, DiscPct,MovingCode,ABCClass,ProductType,PartCategory,Status
	, CreatedBy,CreatedDate,LastUpdateBy,LastUpdateDate
FROM #TempSOSupply WHERE QtySupply > 0

--===============================================================================================================================
-- UPDATE STATUS DETAIL BASED ON SUPPLY
--===============================================================================================================================

UPDATE SpTrnSORDDtl
SET Status = 4
FROM SpTrnSORDDtl a, #TempSOSupply b
WHERE 1 = 1
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.PartNo = b.PartNo
	
--===============================================================================================================================
-- UPDATE HISTORY DEMAND ITEM AND CUSTOMER
--===============================================================================================================================

UPDATE SpHstDemandItem 
SET DemandFreq = DemandFreq + 1
	, DemandQty = DemandQty + b.QtyOrder
	, LastUpdateBy = '''+ @UserID +'''
	, LastUpdateDate = '''+ Convert(varchar,GetDate()) +''' 
FROM SpHstDemandItem a, SpTrnSordDtl b
WHERE 
	1 = 1
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.Year = '''+ Convert(varchar,Year(GetDate())) +''' 
	AND a.Month  = '''+ Convert(varchar,Month(GetDate())) +''' 
	AND a.PartNo = b.PartNo
	AND b.DocNo = '''+ @TempDocNo +'''

UPDATE SpHstDemandCust
SET DemandFreq = DemandFreq + 1
	, DemandQty = DemandQty + b.QtyOrder
	, LastUpdateBy = '''+ @UserID +''' 
	, LastUpdateDate = '''+ Convert(varchar,GetDate()) +''' 
FROM SpHstDemandCust a, SpTrnSordDtl b
WHERE 
	1 = 1
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.Year = '''+ Convert(varchar,Year(GetDate())) +'''
	AND a.Month  = '''+ Convert(varchar,Month(GetDate())) +'''
	AND a.PartNo = b.PartNo
	AND a.CustomerCode = '''+ @CustomerCode +'''
	AND b.DocNo = '''+ @TempDocNo +'''

INSERT INTO SpHstDemandItem
SELECT 
	CompanyCode
	, BranchCode
	, '''+ Convert(varchar,Year(GetDate())) +''' Year
	, '''+ Convert(varchar,Month(GetDate())) +''' Month
	, PartNo
	, 1 DemandFreq
	, QtyOrder DemandQty
	, 0 SalesFreq
	, 0 SalesQty
	, MovingCode
	, ProductType
	, PartCategory
	, ABCClass
	, '''+ @UserID +''' LastUpdateBy
	, '''+ CONVERT(varchar, GetDate()) +''' LastUpdateDate
FROM SpTrnSordDtl a WITH (NOLOCK, NOWAIT)
WHERE a.CompanyCode= '''+ @CompanyCode +''' AND a.BranchCode = '''+ @BranchCode +''' AND a.DocNo = '''+ @TempDocNo +''' -- add CompanyCode and BranchCode 13 Des 2010
 AND NOT EXISTS
( SELECT 1 FROM SpHstDemandItem WITH (NOLOCK, NOWAIT) WHERE 
	1 = 1 
	AND CompanyCode = a.CompanyCode 
	AND BranchCode = a.BranchCode
	AND Month = '''+ Convert(varchar,Month(GetDate())) +''' 
	AND Year = '''+ Convert(varchar,Year(GetDate())) +''' 
	AND PartNo = a.PartNo
)

INSERT INTO SpHstDemandCust
SELECT 
	CompanyCode
	, BranchCode
	, '''+ Convert(varchar,Year(GetDate())) +'''  Year
	, '''+ Convert(varchar,Month(GetDate())) +'''  Month
	, '''+ @CustomerCode +''' CustomerCode
	, PartNo
	, 1 DemandFreq
	, (SELECT QtyOrder FROM SpTrnSORDDTl WITH (NOLOCK, NOWAIT) 
		WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode 
		AND DocNo = a.DocNo AND PartNo = a.PartNo) DemandQty
	, 0 SalesFreq
	, 0 SalesQty
	, MovingCode
	, ProductType
	, PartCategory
	, ABCClass
	, '''+ @UserID +''' LastUpdateBy
	, '''+ CONVERT(varchar, GetDate()) +''' LastUpdateDate
FROM SpTrnSordDtl a WITH (NOLOCK, NOWAIT)
WHERE a.CompanyCode= '''+ @CompanyCode +''' and a.BranchCode= '''+ @BranchCode +''' AND a.DocNo = '''+ @TempDocNo +''' -- add CompanyCode and BranchCode 13 Des 2010
AND NOT EXISTS
( SELECT PartNo FROM SpHstDemandCust WITH (NOLOCK, NOWAIT) WHERE 
	1 = 1 
	AND CompanyCode = a.CompanyCode 
	AND BranchCode = a.BranchCode
	AND Month = '''+ Convert(varchar,Month(GetDate())) +''' 
	AND Year = '''+ Convert(varchar,Year(GetDate())) +'''  
	AND PartNo = a.PartNo
)

--===============================================================================================================================
-- UPDATE LAST DEMAND DATE MASTER
--===============================================================================================================================

UPDATE '+@DbMD+'..SpMstItems 
SET LastDemandDate = '''+Convert(varchar,GetDate())+'''
FROM '+@DbMD+'..SpMstItems a, SpTrnSordDtl b
WHERE 
	1 = 1
	AND a.CompanyCode = '''+@CompanyMD+'''
	AND a.BranchCode = '''+@BranchMD+'''
	AND a.PartNo = b.PartNo
	AND b.DocNo = '''+@TempDocNo+'''

--===============================================================================================================================
-- UPDATE STOCK AND MOVEMENT
--===============================================================================================================================

UPDATE '+@DbMD+'..spMstItems
SET AllocationSR = AllocationSR + b.QtySupply
	, LastUpdateBy = '''+@UserID+'''
	, LastUpdateDate = '''+Convert(varchar,GetDate())+'''
FROM '+@DbMD+'..SpMstItems a, #TempSOSupply b
WHERE 
	1 = 1
	AND a.CompanyCode = '''+@CompanyMD+'''
	AND a.BranchCode = '''+@BranchMD+'''
	AND a.PartNo = b.PartNo

UPDATE '+@DbMD+'..spMstItemloc
SET AllocationSR = AllocationSR + b.QtySupply
	, LastUpdateBy = '''+@UserID+'''
	, LastUpdateDate = '''+Convert(varchar,GetDate())+'''
FROM '+@DbMD+'..SpMstItemLoc a, #TempSOSupply b
WHERE 
	1 = 1
	AND a.CompanyCode = '''+@CompanyMD +'''
	AND a.BranchCode = '''+@BranchMD+'''
	AND a.WarehouseCode = '''+@WarehouseMD+'''
	AND a.PartNo = b.PartNo

INSERT INTO SpTrnIMovement
SELECT
	'''+@CompanyCode +''' CompanyCode
	, '''+@BranchCode +''' BranchCode
	, a.DocNo
	, (SELECT DocDate FROM SpTrnSORDHdr WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyCode +'''
		AND BranchCode = '''+ @BranchCode +''' AND DocNo = a.DocNo) 
	  DocDate
	, dateadd(s,ROW_NUMBER() OVER(Order by a.PartNo),'''+convert(varchar,getdate())+''') CreatedDate 
	, ''00'' WarehouseCode
	, (SELECT LocationCode FROM SpTrnSORDDtl WITH (NOLOCK, NOWAIT) WHERE CompanyCode =  '''+@CompanyCode +'''
		AND BranchCode = '''+@BranchCode +''' AND DocNo = '''+@TempDocNo +''' AND PartNo = a.PartNo)
	  LocationCode
	, a.PartNo
	, ''OUT'' SignCode
	, ''SA-NPJUAL'' SubSignCode
	, a.QtySupply
	, a.RetailPrice
	, a.CostPrice
	, a.ABCClass
	, a.MovingCode
	, a.ProductType
	, a.PartCategory
	, '''+@UserID +''' CreatedBy
FROM #TempSOSupply a

--===============================================================================================================================
-- UPDATE SUPPLY SLIP TO SPK
--===============================================================================================================================
DECLARE @ServiceNo VARCHAR(MAX)

SET @ServiceNo = (SELECT ServiceNo FROM svTrnService WHERE CompanyCode = '''+@CompanyCode +''' AND BranchCode = '''+@BranchCode+'''
		AND ProductType = '''+@ProductType +''' AND JobOrderNo = '''+@JobOrderNo+''')

SELECT * INTO #TempServiceItem FROM (
SELECT 
	a.CompanyCode
	, a.BranchCode
	, a.ProductType
	, a.ServiceNo
	, a.PartNo
	, a.PartSeq
	, a.DemandQty
	, a.SupplyQty
	, b.QtySupply
	, b.DocNo
	, a.CostPrice
	, a.RetailPrice
	, a.TypeOfGoods
	, a.BillType
FROM SvTrnSrvItem a WITH (NOLOCK, NOWAIT)
INNER JOIN #TempSOSupply b WITH (NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.PartNo = b.PartNo
WHERE
	1 = 1
	AND a.CompanyCode = '''+@CompanyCode+'''
	AND a.BranchCode = '''+@BranchCode+'''
	AND a.ProductType = '''+@ProductType+'''
	AND a.ServiceNo = @ServiceNo
	AND a.PartSeq IN (SELECT MAX(PartSeq) FROM SvTrnSrvItem WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode
		AND ProductType = '''+@ProductType +''' AND ServiceNo = a.ServiceNo AND PartNo = a.PartNo)
	AND (a.SupplySlipNo = '''' OR a.SupplySlipNo IS NULL)
) #TempServiceItem 

SELECT * INTO #TempServiceItemIns FROM( 
SELECT 
	a.CompanyCode
	, a.BranchCode
	, a.ProductType
	, a.ServiceNo
	, a.PartNo
	, a.PartSeq
	, a.DemandQty
	, a.SupplyQty
	, b.QtySupply
	, b.DocNo
	, a.CostPrice
	, a.RetailPrice
	, a.TypeOfGoods
	, a.BillType
	, a.DiscPct
FROM SvTrnSrvItem a WITH (NOLOCK, NOWAIT)
INNER JOIN #TempSOSupply b WITH (NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.PartNo = b.PartNo
WHERE
	1 = 1 
	AND a.CompanyCode = '''+ @CompanyCode +''' 
	AND a.BranchCode = '''+ @BranchCode +''' 
	AND a.ProductType = '''+ @ProductType +'''  
	AND a.ServiceNo = @ServiceNo
	AND a.PartSeq IN (SELECT MAX(PartSeq) FROM SvTrnSrvItem WITH (NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode
		AND ProductType = '''+ @ProductType +''' AND ServiceNo = a.ServiceNo AND PartNo = a.PartNo) 
	AND (a.SupplySlipNo != '''' OR a.SupplySlipNo IS NOT NULL)
) #TempServiceItemIns


UPDATE svTrnSrvItem
SET SupplySlipNo = b.DocNo
	, SupplySlipDate = ISNULL((SELECT DocDate FROM SpTrnSORDHdr WITH (NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode
							AND DocNo = b.DocNo),'''+Convert(varchar,@DefaultDate)+''')
FROM svTrnSrvItem a, #TempServiceItem b
WHERE
	1 = 1
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.ProductType = b.ProductType
	AND a.ServiceNo = b.ServiceNo
	AND a.PartNo = b.PartNo
	AND a.PartSeq = b.PartSeq
	
--===============================================================================================================================
-- INSERT NEW SRV ITEM BASED SUPPLY SLIP
--===============================================================================================================================
INSERT INTO SvTrnSrvItem (CompanyCode, BranchCode, ProductType, ServiceNo, PartNo, PartSeq, DemandQty, SupplyQty, ReturnQty, CostPrice, RetailPrice, TypeOfGoods, BillType, SupplySlipNo, SupplySlipDate, SSReturnNo, SSReturnDate, CreatedBy, CreatedDate, LastupdateBy, LastupdateDate, DiscPct)
SELECT 
	a.CompanyCode
	, a.BranchCode
	, a.ProductType
	, a.ServiceNo
	, a.PartNo
	, a.PartSeq + 1
	, 0 DemandQty
	, 0 SupplyQty
	, 0 ReturnQty
	, a.CostPrice
	, a.RetailPrice
	, a.TypeOfGoods
	, a.BillType
	, a.DocNo SupplySlipNo
	, (SELECT DocDate FROM SpTrnSORDHdr WITH (NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode
		AND DocNo = a.DocNo) SupplySlipDate
	, NULL SSReturnNo
	, NULL SSReturnDate
	, '''+ @UserID +''' CreatedBy
	, '''+ Convert(varchar,GetDate()) +''' CreatedDate
	, '''+ @UserID +''' LastUpdateBy
	, '''+ Convert(varchar,GetDate()) +''' LastUpdateDate
	, a.DiscPct
FROM #TempServiceItemIns a WITH (NOLOCK, NOWAIT)
WHERE
	1 = 1
	AND a.CompanyCode = '''+ @CompanyCode +'''
	AND a.BranchCode = '''+ @BranchCode +'''
	AND a.ProductType = '''+ @ProductType+'''


--===============================================================================================================================
DROP TABLE #TempServiceItem 
DROP TABLE #TempServiceItemIns
DROP TABLE #TempSOSupply'

EXEC(@query)

--select convert(xml,@query)


--===============================================================================================================================
-- INSERT SVSDMOVEMENT
--===============================================================================================================================

declare @md bit
set @md = (select case WHEN EXISTS(select * from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode and CompanyMD = @CompanyCode and BranchMD = @BranchCode) then cast(1 as bit) ELSE cast(0 as bit) END)

if(@md = 0)
begin

 declare @QueryTemp as varchar(max)  
 
	set @Query ='insert into '+ @DbMD +'..svSDMovement
	select a.CompanyCode, a.BranchCode, a.DocNo, a.CreatedDate, a.PartNo
	, Seq = convert(integer, ROW_NUMBER() OVER (PARTITION BY a.ReferenceNo ORDER BY a.DocNo)) ,
	a.WarehouseCode, a.QtyOrder, a.QtySupply, a.DiscPct
	--,isnull(((select RetailPrice from spTrnSORDDtl
	--		where CompanyCode = ''' + @CompanyCode + '''  and BranchCode = ''' + @BranchCode  + '''
	--		and DocNo = ''' + @TempDocNo + ''' and PartNo = a.PartNo) / 1.1 * 
	--		((100 - isnull((select isnull(DiscPct, 0) from gnMstSupplierProfitCenter   
	--			where CompanyCode = ''' + @CompanyCode + ''' and BranchCode = ''' + @BranchCode  + ''' and SupplierCode = dbo.GetBranchMD(''' + @CompanyCode + ''', ''' + @BranchCode  + ''') 
	--			and ProfitCenterCode = ''300''),0)) * 0.01)),0) 
	, a.CostPrice
	, a.RetailPrice, b.TypeOfGoods
	, '''+ @CompanyMD +''','''+ @BranchMD +''','''+ @WarehouseMD +''', md.RetailPriceInclTax, md.RetailPrice, md.CostPrice
	,''x'','''+ @producttype +''',''300'',''0'',''0'','''+ convert(varchar,GETDATE()) +''','''+ @UserID +''','''+ convert(varchar,GETDATE()) +'''
	,'''+ @UserID +''','''+ convert(varchar,GETDATE()) +'''
	from spTrnSORDDtl a 
	join spTrnSORDHdr b on a.CompanyCode = b.CompanyCode
	and a.BranchCode = b.BranchCode 
	and a.DocNo = b.DocNo
	join '+ @DbMD +'..spmstitemprice md
	on md.CompanyCode = '''+ @CompanyMD +'''
	and md.branchcode = '''+ @BranchMD +'''
	and md.PartNo = a.PartNo
	where a.ReferenceNo = '''+ @JobOrderNo +''''+
	' and a.DocNo = ''' + @TempDocNo + '''';

	exec (@Query)
	print (@QUERY)

end

--===============================================================================================================================
-- INSERT PICKING HEADER AND DETAIL
--===============================================================================================================================

SET @MaxPickingList = ISNULL((SELECT DocumentSequence + 1 FROM GnMstDocument WHERE 
	CompanyCode = @CompanyCode
		AND BranchCode = @BranchCode
		AND DocumentType = 'PLS' 
		AND ProfitCenterCode = '300' 
		AND DocumentYear = YEAR(GetDate())),0)

SET @TempPickingList = ISNULL((SELECT 'PLS/' + RIGHT(YEAR(GETDATE()),2) +'/' + RIGHT(1000000 + CONVERT(VARCHAR, @MaxPickingList, 1), 6)),'PLS/YY/XXXXXX')

INSERT INTO SpTrnSPickingHdr 
SELECT 
	CompanyCode
	, BranchCode
	, @TempPickingList PickingSlipNo
	, GetDate() PickingSlipDate
	, CustomerCode
	, CustomerCodeBill
	, CustomerCodeShip
	, '' PickedBy
	, CONVERT(BIT, 0) isBORelease
	, isSubstitution
	, isIncludePPN
	, TransType
	, SalesType
	, TotSalesQty
	, TotSalesAmt
	, TotDiscAmt
	, TotDPPAmt
	, TotPPNAmt
	, TotFinalSalesAmt
	, '' Remark
	, '0' Status
	, '0' PrintSeq
	, TypeOfGoods
	, CreatedBy
	, CreatedDate
	, LastUpdateBy
	, LastUpdateDate
	, NULL isLocked
	, NULL LockingBy
	, NULL LockingDate
FROM SpTrnSORDHdr WITH (NOLOCK, NOWAIT)
WHERE 
	1 = 1
	AND CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND DocNo = (SELECT distinct DocNo FROM spTrnSORDDtl WHERE CompanyCode = @CompanyCode AND Branchcode = @BranchCode 
					AND DocNo = @TempDocNo AND QtySupply > 0)		

UPDATE GnMstDocument
SET DocumentSequence = DocumentSequence + 1
	, LastUpdateDate = GetDate()
	, LastUpdateBy = @UserID
WHERE
	1 = 1
	AND CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND DocumentPrefix = 'PLS'
	AND ProfitCenterCode = '300'
	AND DocumentYear = Year(GetDate())

-- ==============================================================================================================================
-- UPDATE SALES ORDER HEADER 
-- ==============================================================================================================================
UPDATE SpTrnSORDHdr
	SET ExPickingSlipNo = @TempPickingList,
		ExPickingSlipDate = ISNULL((SELECT PickingSlipDate FROM SpTrnSPickingHdr WHERE CompanyCode = @CompanyCode
				AND BranchCode = @BranchCode AND PickingSlipNo = @TempPickingList),'')
	
WHERE
	1 = 1
	AND CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND DocNo = @TempDocNo

UPDATE SpTrnSOSupply
	SET PickingSlipNo = @TempPickingList
WHERE
	1 = 1
	AND CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND DocNo = @TempDocNo
-- ==============================================================================================================================
-- INSERT PICKING DETAIL
-- ==============================================================================================================================

INSERT INTO SpTrnSPickingDtl
SELECT 
	a.CompanyCode
	, a.BranchCode
	, @TempPickingList PickingSlipNo
	, '00' WarehouseCode
	, a.PartNo
	, a.PartNoOriginal
	, a.DocNo
	, b.DocDate 
	, a.ReferenceNo
	, a.ReferenceDate
	, a.LocationCode
	, a.QtySupply QtyOrder
	, a.QtySupply
	, a.QtySupply QtyPicked 
	, 0 QtyBill
	, a.RetailPriceInclTax
	, a.RetailPrice
	, a.CostPrice
	, a.DiscPct
	, a.SalesAmt
	, a.DiscAmt
	, a.NetSalesAmt
	, a.TotSalesAmt
	, a.MovingCode
	, a.ABCClass
	, a.ProductType
	, a.PartCategory
	, '' ExPickingSlipNo
	, @DefaultDate ExPickingSlipDate
	, CONVERT(BIT, 0) isClosed
	, @UserID CreatedBy
	, GetDate() CreatedDate
	, @UserID LastUpdateBy
	, GetDate() LastUpdateDate
FROM SpTrnSORDDtl a WITH (NOLOCK, NOWAIT)
INNER JOIN SpTrnSORDHdr b WITH (NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.DocNo = b.DocNo
WHERE
	1 = 1
	AND a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND a.DocNo = @TempDocNo
	AND a.QtySupply > 0


--================================================================================================================================
-- UPDATE AMOUNT HEADER
--================================================================================================================================
SELECT * INTO #TempHeader FROM (
SELECT 
	header.CompanyCode
	, header.BranchCode
	, header.DocNo
	, header.TotSalesQty
	, header.TotSalesAmt
	, header.TotDiscAmt
	, header.TotDPPAmt
	, floor(header.TotDPPAmt * (ISNULL((SELECT TaxPct FROM GnMstTax WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
			AND TaxCode IN (SELECT TaxCode FROM GnMstCustomerProfitCenter WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
			AND ProfitCenterCode = '300' AND CustomerCode = @CustomerCode)),0)/100)) 
		TotPPNAmt
	, header.TotDPPAmt + floor(header.TotDPPAmt * (ISNULL((SELECT TaxPct FROM GnMstTax WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
			AND TaxCode IN (SELECT TaxCode FROM GnMstCustomerProfitCenter WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
			AND ProfitCenterCode = '300' AND CustomerCode = @CustomerCode)),0)/100))
		TotFinalSalesAmt
FROM (
SELECT 
	CompanyCode
	, BranchCode
	, DocNo
	, SUM(QtyOrder) TotSalesQty
	, SUM(SalesAmt) TotSalesAmt
	, SUM(DiscAmt) TotDiscAmt
	, SUM(NetSalesAmt) TotDPPAmt
FROM SpTrnSORDDtl WITH (NOLOCK, NOWAIT)
WHERE CompanyCode = @CompanyCode 
	AND BranchCode = @BranchCode
	AND DocNo = @TempDocNo
GROUP BY CompanyCode
	, BranchCode
	, DocNo
) header ) #TempHeader

UPDATE SpTrnSORDHdr
SET 
	TotSalesQty = b.TotSalesQty
	, TotSalesAmt = b.TotSalesAmt
	, TotDiscAmt = b.TotDiscAmt
	, TotDPPAmt = b.TotDPPAmt
	, TotPPNAmt = b.TotPPNAmt
	, TotFinalSalesAmt = b.TotFinalSalesAmt
FROM SpTrnSORDHdr a, #TempHeader b
WHERE 
	1 = 1
	AND a.CompanyCode = b.CompanyCode 
	AND a.BranchCode = b.BranchCode
	AND a.DocNo = b.DocNo

DROP TABLE #TempHeader

FETCH NEXT FROM db_cursor INTO @TypeOfGoods
END
CLOSE db_cursor
DEALLOCATE db_cursor 

--===============================================================================================================================
-- Update Transdate
--===============================================================================================================================

update gnMstCoProfileSpare
set TransDate=getdate()
	, LastUpdateBy=@UserID
	, LastUpdateDate=getdate()
where CompanyCode = @CompanyCode AND BranchCode = @BranchCode

--===============================================================================================================================
-- DROP TABLE SECTION 
--===============================================================================================================================
DROP TABLE #SrvOrder
DROP TABLE #Item

--rollback tran
END
go
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
    select a.CompanyCode, a.BranchCode, isnull(d.PeriodYear, -1) PeriodYear, isnull(d.PeriodMonth, -1) PeriodMonth  
      , d.RetentionNo, a.CustomerCode, c.CustomerName, a.JobOrderNo, b.LastServiceDate  
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
		and d.PeriodMonth = month(a.JobOrderDate) 
		and d.CustomerCode = a.CustomerCode  
		and d.ChassisCode = a.ChassisCode 
		and d.ChassisNo = a.ChassisNo  
      left join gnMstLookupDtl e  
        on e.CompanyCode = a.CompanyCode  
       and e.CodeID = 'CIRS'  
       and e.LookupValue = d.VisitInitial  
       inner join svTrnInvoice f
		on f.CompanyCode = a.CompanyCode
		and f.BranchCode = a.BranchCode
		and f.JobOrderNo = a.JobOrderNo
		and f.InvoiceNo like '%INC%'
	 left join svTrnInvoice g
		on g.CompanyCode = a.CompanyCode
		and g.BranchCode = a.BranchCode
		and g.JobOrderNo = a.JobOrderNo
		and g.InvoiceNo like '%INF%'
     where 1 = 1  
	   and a.CompanyCode = @CompanyCode  
	   and a.BranchCode = @BranchCode  
	   and a.JobType like '%FSC%' 
	   and g.InvoiceNo is null 
	   and convert(varchar, @DateParam, 112) = convert(varchar, dateadd(day, @Interval, a.JobOrderDate), 112)  
	   and a.JobType <> (case @InclPDI when 1 then '' else 'PDI' end)
	   and (b.LastServiceDate is null or convert(varchar, a.JobOrderDate, 112) >= convert(varchar, b.LastServiceDate, 112))  
	   and d.RetentionNo = ISNULL((
								select max(RetentionNo)
								  from svTrnDailyRetention
								 where CompanyCode = a.CompanyCode
								   and BranchCode = a.BranchCode
								   and PeriodYear = year(a.JobOrderDate)  
								   and PeriodMonth = month(a.JobOrderDate)
								   and CustomerCode = a.CustomerCode
								   and ChassisCode=a.ChassisCode
								   and ChassisNo=a.ChassisNo
								),0)  
	union							
    select a.CompanyCode, a.BranchCode, isnull(d.PeriodYear, -1) PeriodYear, isnull(d.PeriodMonth, -1) PeriodMonth  
      , d.RetentionNo, a.CustomerCode, c.CustomerName, a.JobOrderNo, b.LastServiceDate  
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
		and d.PeriodMonth = month(a.JobOrderDate) 
		and d.CustomerCode = a.CustomerCode  
		and d.ChassisCode = a.ChassisCode 
		and d.ChassisNo = a.ChassisNo  
      left join gnMstLookupDtl e  
        on e.CompanyCode = a.CompanyCode  
       and e.CodeID = 'CIRS'  
       and e.LookupValue = d.VisitInitial  
     where 1 = 1  
	   and a.CompanyCode = @CompanyCode  
	   and a.BranchCode = @BranchCode  
	   and a.ServiceStatus not in ('0','1','2','3','4','5','6')  
	   --and convert(varchar, @DateParam, 112) = convert(varchar, dateadd(month, 1, a.JobOrderDate), 112)  
	   and convert(varchar, @DateParam, 112) = convert(varchar, dateadd(day, @Interval, a.JobOrderDate), 112)  
	   and a.JobType <> (case @InclPDI when 1 then '' else 'PDI' end)
	   and (b.LastServiceDate is null or convert(varchar, a.JobOrderDate, 112) >= convert(varchar, b.LastServiceDate, 112))  
	   and d.RetentionNo = ISNULL((
								select max(RetentionNo)
								  from svTrnDailyRetention
								 where CompanyCode = a.CompanyCode
								   and BranchCode = a.BranchCode
								   and PeriodYear = year(a.JobOrderDate)  
								   and PeriodMonth = month(a.JobOrderDate)
								   and CustomerCode = a.CustomerCode
								   and ChassisCode=a.ChassisCode
								   and ChassisNo=a.ChassisNo
								),0)  

end
go
if object_id('usprpt_OmFakturPajak') is not null
	drop procedure usprpt_OmFakturPajak
GO
create procedure [dbo].[usprpt_OmFakturPajak]
--DECLARE
	@CompanyCode	VARCHAR(15),
	@BranchCode	VARCHAR(15),
	@FPJDateStart DATETIME,
	@FPJDateEnd DATETIME,
	@FPJNoStart	VARCHAR(30),
	@FPJNoEnd	VARCHAR(30),
	@SignName VARCHAR(100),
	@TitleSign VARCHAR(100),
	@Param bit = 1
AS
BEGIN

--'6114201','611420100','20150501','20150530','010.001-15.70827239','010.001-15.70827239','Wiwik W','Pimpinan FAD',True

--SELECT @CompanyCode = '6114201',@BranchCode='611420100',@FPJDateStart='20150501',@FPJDateEnd='20150630',@FPJNoStart='010.001-15.70827992',
--@FPJNoEnd='010.001-15.70827992',@SignName='Wiwik W',@TitleSign='Pimpinan FAD',@Param=1

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
	declare @fInv		int
	
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
set @fInv		= (select isnull(ParaValue,'1') from gnmstlookupdtl where codeid='FPJFLAG' and lookupvalue='SALES')		
-- parameter use address holding or not
declare @IsHoldingAddr as bit
if (select count(*) from gnMstLookUpDtl where CompanyCode= @CompanyCode and CodeID='FPJADDR') > 0
	set @IsHoldingAddr = (select convert(numeric,LookUpValue) from gnMstLookUpDtl where CompanyCode= @CompanyCode and CodeID='FPJADDR')
else
	set @IsHoldingAddr = 0

-- parameter to show info or not
declare @IsShowInfo as bit
if (select count(*) from gnMstLookUpDtl where CompanyCode= @CompanyCode and CodeID='FPIF' and LookUpValue='STATUS') > 0
	set @IsShowInfo = (select convert(numeric,ParaValue) from gnMstLookUpDtl where CompanyCode= @CompanyCode and CodeID='FPIF' and LookUpValue='STATUS')
else
	set @IsShowInfo = 1
	
	
	declare @tabData as table
	(
		CompanyCode varchar(max),
		BranchCode varchar(max),
		InvoiceNo varchar(max),
		ItemOrder varchar(max),
		ItemCode varchar(max),
		ItemName varchar(max),
		PPnBM decimal(18,2),
		PPnBMSell decimal(18,2),
		ItemQuantity decimal(5,2),
		ItemDPP decimal(18,2),
		Potongan decimal(18,2),
		TaxPct decimal(18,2),
		AfterDiscPpn  decimal(18,2)
	)
	IF (@Param=1)
	BEGIN
		-- Sembunyikan Detail Part .....
		SELECT * INTO #t1 FROM (
		SELECT 
			acc.CompanyCode
			, acc.BranchCode
			, acc.InvoiceNo
			, acc.PartNo AS ItemCode
			, ISNULL((acc.Quantity)/(select count(chassisno) from omFakturPajakDetail where companycode=acc.companycode 
						and branchcode=acc.branchcode and invoiceno=acc.invoiceno) * acc.RetailPrice,0) AS ItemDPP
			, ISNULL((acc.Quantity)/(select count(chassisno) from omFakturPajakDetail where companycode=acc.companycode 
						and branchcode=acc.branchcode and invoiceno=acc.invoiceno) * acc.DiscExcludePPn, 0) AS Potongan
			, 0 AS TaxPct
		FROM 
			omFakturPajakDetailAccsSeq acc 
		INNER JOIN omFakturPajakhdr hdr ON hdr.CompanyCode = acc.CompanyCode
			AND hdr.BranchCode = acc.BranchCode
			AND hdr.InvoiceNo = acc.InvoiceNo
			AND ((CASE WHEN @FPJNoStart = '' THEN hdr.FakturPajakNo END) <> ''
				OR (CASE WHEN @FPJNoStart <> '' THEN hdr.FakturPajakNo END) BETWEEN @FPJNoStart AND @FPJNoEnd)
			AND CONVERT(VARCHAR, hdr.FakturPajakDate, 112) BETWEEN CONVERT(VARCHAR, @FPJDateStart, 112) AND CONVERT(VARCHAR, @FPJDateEnd, 112)
			AND hdr.TaxType = 'Standard'
		WHERE 
			acc.CompanyCode = @CompanyCode AND acc.BranchCode = @BranchCode	)#t1
				
		select * into #Others from(
				Select a.CompanyCode
					 , a.BranchCode
					 , a.InvoiceNo
					 , a.SalesModelCode
					 , ISNULL((a.Quantity)/(select count(chassisno) from omFakturPajakDetail where companycode=a.companycode 
						and branchcode=a.branchcode and invoiceno=a.invoiceno) * a.DPP, 0) ItemDPP
					 , 0 Potongan
				from omFakturPajakDetailOthers a
				left join omFakturPajakDetail b on a.CompanyCode = b.CompanyCode
					  and a.BranchCode = b.BranchCode
					  and a.InvoiceNo = b.InvoiceNo
					  and a.SalesModelCode = b.SalesModelCode
				INNER JOIN omFakturPajakhdr hdr ON hdr.CompanyCode = a.CompanyCode
					   AND hdr.BranchCode = a.BranchCode
					   AND hdr.InvoiceNo = a.InvoiceNo
					   AND ((CASE WHEN @FPJNoStart = '' THEN hdr.FakturPajakNo END) <> ''
					   	   OR (CASE WHEN @FPJNoStart <> '' THEN hdr.FakturPajakNo END) BETWEEN @FPJNoStart AND @FPJNoEnd)
					   AND CONVERT(VARCHAR, hdr.FakturPajakDate, 112) BETWEEN CONVERT(VARCHAR, @FPJDateStart, 112) AND CONVERT(VARCHAR, @FPJDateEnd, 112)
					   AND hdr.TaxType = 'Standard'
				where a.CompanyCode = @CompanyCode
				  and a.BranchCode = @BranchCode
		)#Others
		
		--SELECT * INTO #t2 FROM (
		--SELECT
		--	CompanyCode 
		--	, BranchCode
		--	, InvoiceNo 
		--	, SUM(ItemDPP) ItemDPP
		--	, SUM(Potongan) Potongan
		--FROM #t1 GROUP BY CompanyCode, BranchCode, InvoiceNo ) #t2
		
		SELECT CompanyCode 
				, BranchCode
				, InvoiceNo 
				, SUM(ItemDPP) ItemDPP
				, SUM(Potongan) Potongan
		INTO #t2 FROM (
			SELECT
				CompanyCode 
				, BranchCode
				, InvoiceNo 
				, SUM(ItemDPP) ItemDPP
				, SUM(Potongan) Potongan
			FROM #t1 GROUP BY CompanyCode, BranchCode, InvoiceNo
		UNION
			SELECT
				CompanyCode 
				, BranchCode
				, InvoiceNo 
				, SUM(ItemDPP) ItemDPP
				, SUM(Potongan) Potongan
			FROM #Others a
			GROUP BY a.CompanyCode, a.BranchCode, a.InvoiceNo 
		) #t2
		group by CompanyCode,BranchCode,InvoiceNo
		
		-- INSER DATA KE TABEL RESULT
		INSERT INTO @tabData 
		-- Unit
		SELECT 
			mdl.CompanyCode
			, mdl.BranchCode
			, mdl.InvoiceNo
			, '1' AS ItemOrder
			, mdl.SalesModelCode AS ItemCode
			, LEFT(CONVERT(VARCHAR, mdl.ChassisNo) + '             ', 13) +  LEFT(CONVERT(VARCHAR, mdl.EngineNo) + '           ', 11) AS ItemName
			, ISNULL(Vec.PPnBMBuyPaid,0) AS PPnBM
			, mdl.AfterDiscPPnBM AS PPnBMSell
			, 1 AS ItemQuantity
			, mdl.BeforeDiscDPP + (ISNULL(t.ItemDPP,0)) AS ItemDPP
			, mdl.DiscExcludePPN + (ISNULL(t.Potongan,0)) AS Potongan
			, ISNULL((SELECT ISNULL(TaxPct, 0) FROM  GnMstTax INNER JOIN OmMstModel ON OmMstModel.CompanyCode = GnMstTax.CompanyCode AND OmMstModel.PPnBMCodeSell = GnMstTax.TaxCode AND OmMstModel.SalesModelCode = mdl.SalesModelCode), 0) AS TaxPct
			, mdl.AfterDiscPpn		
		FROM 
			omFakturPajakDetail mdl 
		INNER JOIN omFakturPajakhdr hdr ON hdr.CompanyCode = mdl.CompanyCode
			AND hdr.BranchCode = mdl.BranchCode
			AND hdr.InvoiceNo = mdl.InvoiceNo
			AND ((CASE WHEN @FPJNoStart = '' THEN hdr.FakturPajakNo END) <> ''
				OR (CASE WHEN @FPJNoStart <> '' THEN hdr.FakturPajakNo END) BETWEEN @FPJNoStart AND @FPJNoEnd)
			AND CONVERT(VARCHAR, hdr.FakturPajakDate, 112) BETWEEN CONVERT(VARCHAR, @FPJDateStart, 112) AND CONVERT(VARCHAR, @FPJDateEnd, 112)
			AND hdr.TaxType = 'Standard'
		LEFT JOIN OmMstVehicle vec ON mdl.CompanyCode = vec.CompanyCode 
			AND mdl.ChassisCode = vec.ChassisCode 
			AND mdl.ChassisNo = vec.ChassisNo
		LEFT JOIN #t2 t ON t.CompanyCode = mdl.CompanyCode
			AND t.BranchCode = mdl.BranchCode
			AND t.InvoiceNo = mdl.InvoiceNo
		WHERE 
			mdl.CompanyCode = @CompanyCode AND mdl.BranchCode = @BranchCode 
		UNION ALL
		SELECT distinct
			hdr.CompanyCode
			, hdr.BranchCode
			, hdr.InvoiceNo
			, '2' AS ItemOrder
			, 'SPAREPART/MATERIAL' AS ItemCode
			, '' AS ItemName
			, 0 AS PPnBM
			, 0 AS PPnBMSell
			, 0 AS ItemQuantity
			, 0 AS ItemDPP
			, 0 AS Potongan
			, 0 AS TaxPct
			, 0 AS AfterDiscPpn
		FROM omFakturPajakDetailAccsSeq acc
		INNER JOIN omFakturPajakHdr hdr ON hdr.CompanyCode=acc.CompanyCode
			AND hdr.BranchCode=acc.BranchCode
			AND hdr.InvoiceNo=acc.InvoiceNo
		WHERE hdr.CompanyCode = @CompanyCode
			AND hdr.BranchCode = @BranchCode
			AND ((CASE WHEN @FPJNoStart = '' THEN hdr.FakturPajakNo END) <> ''
				OR (CASE WHEN @FPJNoStart <> '' THEN hdr.FakturPajakNo END) BETWEEN @FPJNoStart AND @FPJNoEnd)
			AND CONVERT(VARCHAR, hdr.FakturPajakDate, 112) BETWEEN CONVERT(VARCHAR, @FPJDateStart, 112) AND CONVERT(VARCHAR, @FPJDateEnd, 112)
			AND hdr.TaxType = 'Standard'
		UNION ALL
		-- SPAREPART/MATERIAL
		SELECT 
			acc.CompanyCode
			, acc.BranchCode
			, acc.InvoiceNo
			, '3' AS ItemOrder
			, acc.PartNo AS ItemCode
			, acc.PartName AS ItemName
			, 0 AS PPnBM
			, 0 AS PPnBMSell
			, 0 AS ItemQuantity
			, 0 AS ItemDPP
			, 0 AS Potongan
			, 0 AS TaxPct
			, 0 AS AfterDiscPpn
		FROM 
			omFakturPajakDetailAccsSeq acc 
		INNER JOIN omFakturPajakhdr hdr ON hdr.CompanyCode = acc.CompanyCode
			AND hdr.BranchCode = acc.BranchCode
			AND hdr.InvoiceNo = acc.InvoiceNo
			AND ((CASE WHEN @FPJNoStart = '' THEN hdr.FakturPajakNo END) <> ''
				OR (CASE WHEN @FPJNoStart <> '' THEN hdr.FakturPajakNo END) BETWEEN @FPJNoStart AND @FPJNoEnd)
			AND CONVERT(VARCHAR, hdr.FakturPajakDate, 112) BETWEEN CONVERT(VARCHAR, @FPJDateStart, 112) AND CONVERT(VARCHAR, @FPJDateEnd, 112)
			AND hdr.TaxType = 'Standard'
		WHERE 
			acc.CompanyCode = @CompanyCode AND acc.BranchCode = @BranchCode	
		UNION ALL
		SELECT 
			oth.CompanyCode
			, oth.BranchCode
			, oth.InvoiceNo
			, '4' AS ItemOrder
			, oth.OtherCode AS ItemCode
			, oth.OtherName AS ItemName
			, 0 AS PPnBM
			, 0 AS PPnBMSell
			, 0 AS ItemQuantity
			, 0 AS ItemDPP
			, 0 AS Potongan
			, 0 AS TaxPct
			, 0 AS AfterDiscPpn			
		FROM 
			omFakturPajakDetailOthers oth 
		INNER JOIN omFakturPajakhdr hdr ON hdr.CompanyCode = oth.CompanyCode
			AND hdr.BranchCode = oth.BranchCode
			AND hdr.InvoiceNo = oth.InvoiceNo
			AND ((CASE WHEN @FPJNoStart = '' THEN hdr.FakturPajakNo END) <> ''
				OR (CASE WHEN @FPJNoStart <> '' THEN hdr.FakturPajakNo END) BETWEEN @FPJNoStart AND @FPJNoEnd)
			AND CONVERT(VARCHAR, hdr.FakturPajakDate, 112) BETWEEN CONVERT(VARCHAR, @FPJDateStart, 112) AND CONVERT(VARCHAR, @FPJDateEnd, 112)
			AND hdr.TaxType = 'Standard'
		WHERE 
			oth.CompanyCode = @CompanyCode AND oth.BranchCode = @BranchCode	
							 
		DROP TABLE #t1
		DROP TABLE #t2			
	END
	ELSE
	BEGIN
		-- Tampilkan Part --
		--------------------
		
		-- INSER DATA KE TABEL RESULT
		INSERT INTO @tabData 
		-- Unit
		SELECT 
			mdl.CompanyCode
			, mdl.BranchCode
			, mdl.InvoiceNo
			, '1' AS ItemOrder
			, mdl.SalesModelCode AS ItemCode
			, LEFT(CONVERT(VARCHAR, mdl.ChassisNo) + '             ', 13) +  LEFT(CONVERT(VARCHAR, mdl.EngineNo) + '           ', 11) AS ItemName
			, ISNULL(Vec.PPnBMBuyPaid,0) AS PPnBM
			, mdl.AfterDiscPPnBM AS PPnBMSell
			, 1 AS ItemQuantity
			, mdl.BeforeDiscDPP AS ItemDPP
			, mdl.DiscExcludePPN Potongan
			, ISNULL((SELECT ISNULL(TaxPct, 0) FROM  GnMstTax INNER JOIN OmMstModel ON OmMstModel.CompanyCode = GnMstTax.CompanyCode AND OmMstModel.PPnBMCodeSell = GnMstTax.TaxCode AND OmMstModel.SalesModelCode = mdl.SalesModelCode), 0) AS TaxPct
			, mdl.AfterDiscPpn
		FROM 
			omFakturPajakDetail mdl 
		INNER JOIN omFakturPajakhdr hdr ON hdr.CompanyCode = mdl.CompanyCode
			AND hdr.BranchCode = mdl.BranchCode
			AND hdr.InvoiceNo = mdl.InvoiceNo
			AND ((CASE WHEN @FPJNoStart = '' THEN hdr.FakturPajakNo END) <> ''
				OR (CASE WHEN @FPJNoStart <> '' THEN hdr.FakturPajakNo END) BETWEEN @FPJNoStart AND @FPJNoEnd)
			AND CONVERT(VARCHAR, hdr.FakturPajakDate, 112) BETWEEN CONVERT(VARCHAR, @FPJDateStart, 112) AND CONVERT(VARCHAR, @FPJDateEnd, 112)
			AND hdr.TaxType = 'Standard'
		LEFT JOIN OmMstVehicle vec ON mdl.CompanyCode = vec.CompanyCode 
			AND mdl.ChassisCode = vec.ChassisCode 
			AND mdl.ChassisNo = vec.ChassisNo
		WHERE 
			mdl.CompanyCode = @CompanyCode AND mdl.BranchCode = @BranchCode 
		UNION ALL
		SELECT distinct
			hdr.CompanyCode
			, hdr.BranchCode
			, hdr.InvoiceNo
			, '2' AS ItemOrder
			, 'SPAREPART/MATERIAL' AS ItemCode
			, '' AS ItemName
			, 0 AS PPnBM
			, 0 AS PPnBMSell
			, 0 AS ItemQuantity
			, 0 AS ItemDPP
			, 0 AS Potongan
			, 0 AS TaxPct
			, 0 AS AfterDiscPpn
		FROM omFakturPajakDetailAccsSeq acc
		INNER JOIN omFakturPajakHdr hdr ON hdr.CompanyCode=acc.CompanyCode
			AND hdr.BranchCode=acc.BranchCode
			AND hdr.InvoiceNo=acc.InvoiceNo
		WHERE hdr.CompanyCode = @CompanyCode
			AND hdr.BranchCode = @BranchCode
			AND ((CASE WHEN @FPJNoStart = '' THEN hdr.FakturPajakNo END) <> ''
				OR (CASE WHEN @FPJNoStart <> '' THEN hdr.FakturPajakNo END) BETWEEN @FPJNoStart AND @FPJNoEnd)
			AND CONVERT(VARCHAR, hdr.FakturPajakDate, 112) BETWEEN CONVERT(VARCHAR, @FPJDateStart, 112) AND CONVERT(VARCHAR, @FPJDateEnd, 112)
			AND hdr.TaxType = 'Standard'
		UNION ALL
		-- SPAREPART/MATERIAL
		SELECT 
			acc.CompanyCode
			, acc.BranchCode
			, acc.InvoiceNo
			, '3' AS ItemOrder
			, acc.PartNo AS ItemCode
			, acc.PartName AS ItemName
			, 0 AS PPnBM
			, 0 AS PPnBMSell
			, acc.Quantity AS ItemQuantity
			, (acc.Quantity * acc.RetailPrice) AS ItemDPP
			, (acc.Quantity * acc.DiscExcludePPn) AS Potongan
			, 0 AS TaxPct
			, 0 AS AfterDiscPpn			
		FROM 
			omFakturPajakDetailAccsSeq acc 
		INNER JOIN omFakturPajakhdr hdr ON hdr.CompanyCode = acc.CompanyCode
			AND hdr.BranchCode = acc.BranchCode
			AND hdr.InvoiceNo = acc.InvoiceNo
			AND ((CASE WHEN @FPJNoStart = '' THEN hdr.FakturPajakNo END) <> ''
				OR (CASE WHEN @FPJNoStart <> '' THEN hdr.FakturPajakNo END) BETWEEN @FPJNoStart AND @FPJNoEnd)
			AND CONVERT(VARCHAR, hdr.FakturPajakDate, 112) BETWEEN CONVERT(VARCHAR, @FPJDateStart, 112) AND CONVERT(VARCHAR, @FPJDateEnd, 112)
			AND hdr.TaxType = 'Standard'
		WHERE 
			acc.CompanyCode = @CompanyCode AND acc.BranchCode = @BranchCode	
		UNION ALL
		SELECT 
			oth.CompanyCode
			, oth.BranchCode
			, oth.InvoiceNo
			, '4' AS ItemOrder
			, oth.OtherCode AS ItemCode
			, oth.OtherName AS ItemName
			, 0 AS PPnBM
			, 0 AS PPnBMSell
			, oth.Quantity AS ItemQuantity
			, (oth.Quantity * oth.DPP) AS ItemDPP
			, 0 AS Potongan
			, 0 AS TaxPct
			, 0 AS AfterDiscPpn			
		FROM 
			omFakturPajakDetailOthers oth 
		INNER JOIN omFakturPajakhdr hdr ON hdr.CompanyCode = oth.CompanyCode
			AND hdr.BranchCode = oth.BranchCode
			AND hdr.InvoiceNo = oth.InvoiceNo
			AND ((CASE WHEN @FPJNoStart = '' THEN hdr.FakturPajakNo END) <> ''
				OR (CASE WHEN @FPJNoStart <> '' THEN hdr.FakturPajakNo END) BETWEEN @FPJNoStart AND @FPJNoEnd)
			AND CONVERT(VARCHAR, hdr.FakturPajakDate, 112) BETWEEN CONVERT(VARCHAR, @FPJDateStart, 112) AND CONVERT(VARCHAR, @FPJDateEnd, 112)
			AND hdr.TaxType = 'Standard'
		WHERE 
			oth.CompanyCode = @CompanyCode AND oth.BranchCode = @BranchCode	
	END

SELECT * INTO #hasil FROM (
SELECT
	a.TaxType AS TaxType
	,a.InvoiceNo AS InvoiceNo
	,a.InvoiceDate AS InvoiceDate
	,a.FakturPajakNo AS FPJNo
	,(SELECT dbo.GetDateIndonesian (CONVERT(VARCHAR,a.FakturPajakDate, 101))) AS FPJDate
	,a.CustomerCode AS fakturCustCode
	,case when @fInfoPKP = 1 then
		(case @fStatus when '1' then @fCompName else e.CompanyGovName end)
	 else '' end AS CompanyName
	,case @fStatus when '1' then @fSKP else e.SKPNo end AS compSKPNo
	,case @fStatus when '1' then @fSKPDate else e.SKPDate end AS compSKPDate
	,case when @fInfoPKP = 1 then
		(case @fStatus when '1' then @fAdd else 
			(case when @IsHoldingAddr=0 then ISNULL(e.Address1,'') + ' ' + ISNULL(e.Address2,'') + ' ' + ISNULL(e.Address3,'') + ' ' + ISNULL(e.Address4,'')
				else (select ISNULL(Address1,'') + ' ' + ISNULL(Address2,'') + ' ' + ISNULL(Address3,'') + ' ' + ISNULL(Address4,'') from gnMstCoProfile where CompanyCode=@CompanyCode
				and BranchCode=(select BranchCode from gnMstOrganizationDtl where CompanyCode=@CompanyCode and IsBranch=0))end ) end)
		 else '' end AS compAddr	 
	,e.PhoneNo AS compPhoneNo
	,e.FaxNo AS compFaxNo
	,case when @fInfoPKP = 1 then
		(case @fStatus when '1' then @fNPWP else e.NPWPNo end)
	 else '' end AS compNPWPNo
	,d.CustomerGovName AS CustomerName
	,d.SKPNo AS custSKPNo
	,d.SKPDate AS custSKPDate
	,ISNULL(d.Address1,'') AS custAddr1
	,ISNULL(d.Address2,'') AS custAddr2
	,ISNULL(d.Address3,'')+ ' ' + ISNULL(d.Address4,'') AS custAddr3
	,d.PhoneNo AS custPhoneNo
	,d.FaxNo AS custFaxNo
	,d.NPWPNo AS custNPWPNo
	,a.DueDate AS fakturDueDate
	,a.DiscAmt AS DiscAmt
	,a.DppAmt AS DppAmt
	,a.PPNAmt AS PPNAmt
	,a.DppAmt - a.DiscAmt AS SubAmt
	,a.TotalAmt AS TotalAmt
	,a.TotQuantity
	,a.PPnBMPaid
	,case @fStatus when '1' then @fCity else 
		(SELECT LookUpValueName FROM gnMstLookupDtl WHERE CodeID = 'CITY' AND LookUpValue = e.CityCode) end as cityNm
	,ISNULL(@SignName, '') AS TaxPerson
	,ISNULL(@TitleSign,'') AS JobTitle
	,'Model              No.Rangka    No.Mesin            PPnBM' AS ItemHeader
	,dtl.ItemOrder
	,dtl.ItemCode
	,dtl.ItemName
	,dtl.PPnBM
	,dtl.ItemQuantity
	,dtl.ItemDPP
	,dtl.Potongan
	,dtl.AfterDiscPpn
	,CASE WHEN copro.ProductType = '2W' THEN 
		(CASE WHEN so.SalesType = '0' THEN 
			(SELECT LookupValuename FROM gnMstLookUpDtl WHERE CodeID = 'FPJN' AND  LookupValue = '2WWS') 
				ELSE (SELECT LookupValuename FROM gnMstLookUpDtl WHERE CodeID = 'FPJN' and  LookupValue = '2WDS') END) 
		ELSE 
		(CASE WHEN so.SalesType = '0' THEN 
			(SELECT LookupValuename FROM gnMstLookUpDtl WHERE CodeID = 'FPJN' AND  LookupValue = '4WWS') 
				ELSE (SELECT LookupValuename FROM gnMstLookUpDtl WHERE CodeID = 'FPJN' and  LookupValue = '4WDS') END)  
	END AS Keterangan
	,@IsShowInfo as FlagShowInfo
	,@param HidePart
	,dtl.TaxPct
	,dtl.PPnBMSell
FROM 
	omFakturPajakHdr a
LEFT JOIN GnMstCustomer d ON a.CompanyCode = d.CompanyCode 
	AND a.CustomerCode = d.CustomerCode
LEFT JOIN GnMstCoProfile e ON a.CompanyCode = e.CompanyCode 
	AND a.BranchCode = e.BranchCode
INNER JOIN @tabData dtl ON dtl.CompanyCode = a.CompanyCode
	AND dtl.BranchCode = a.BranchCode
	AND dtl.InvoiceNo = a.InvoiceNo
LEFT JOIN omTrSalesInvoice inv ON inv.CompanyCode = a.CompanyCode 
	AND inv.BranchCode = a.BranchCode 
	AND inv.InvoiceNo = a.InvoiceNo
LEFT JOIN omTrSalesSO so	ON a.CompanyCode = so.CompanyCode
	AND a.BranchCode = so.BranchCode 
	AND inv.SONo = so.SONo
LEFT JOIN GnMstCoProfile copro ON a.CompanyCode = copro.CompanyCode 
	AND a.BranchCode = copro.BranchCode
WHERE  
	a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode ) #hasil

if (@param=1)
	BEGIN
	SELECT TaxType, InvoiceNo, InvoiceDate, FPJNo, FPJDate, fakturCustCode, CompanyName, compSKPNo, compSKPDate, compAddr, compPhoneNo, compFaxNo, compNPWPNo
		, CustomerName, custSKPNo, custSKPDate, custAddr1, custAddr2, custAddr3, custPhoneNo, custFaxNo, custNPWPNo, fakturDueDate, DiscAmt, DPPAmt
		--, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select round(((sum(ItemDPP) - sum(Potongan)) * 0.1),0) from #hasil where FPJNo = c.FPJNo) ELSE PPNAmt END PPNAmt
		, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select sum(AfterDiscPpn) from #hasil where FPJNo = c.FPJNo) ELSE PPNAmt END PPNAmt
		, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select sum(ItemDPP) - sum(Potongan) from #hasil where FPJNo = c.FPJNo ) ELSE SubAmt END SubAmt
		--, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select  (sum(ItemDPP) - sum(Potongan)) + round(((sum(ItemDPP) - sum(Potongan)) * 0.1),0) from #hasil where FPJNo = c.FPJNo ) ELSE TotalAmt END TotalAmt
		--, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select  (sum(ItemDPP) - sum(Potongan)) + sum(AfterDiscPpn) from #hasil where FPJNo = c.FPJNo ) ELSE TotalAmt END TotalAmt
		, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select sum(TotalAmt) from #hasil where FPJNo = c.FPJNo ) ELSE TotalAmt END TotalAmt
		, TotQuantity, PPNBMPaid, cityNm, TaxPerson, JobTitle, ItemHeader, ItemOrder, ItemCode, ItemName, PPnBM, ItemQuantity, ItemDPP, Potongan, Keterangan
		, FlagShowInfo, HidePart, TaxPct, PPnBMSell
		, (SELECT COUNT(INVOICENO) FROM #hasil WHERE FPJNo = c.FPJNo) MaxRow
		, (select sum(ItemQuantity) from #hasil where FPJNo=c.FPJNo and ItemOrder='1') SumQty
		, (select sum(ItemDPP) from #hasil where FPJNo = c.FPJNo ) XAmt
		, (select sum(Potongan) from #hasil where FPJNo = c.FPJNo ) XPotongan
		, @fInv ViewInvoice
	FROM #hasil c order by c.InvoiceNo, c.ItemOrder
	END
else
	SELECT TaxType, InvoiceNo, InvoiceDate, FPJNo, FPJDate, fakturCustCode, CompanyName, compSKPNo, compSKPDate, compAddr, compPhoneNo, compFaxNo, compNPWPNo
		, CustomerName, custSKPNo, custSKPDate, custAddr1, custAddr2, custAddr3, custPhoneNo, custFaxNo, custNPWPNo, fakturDueDate, DiscAmt, DPPAmt
		--, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select round(((sum(ItemDPP) - sum(Potongan)) * 0.1),0) from #hasil where FPJNo = c.FPJNo) ELSE PPNAmt END PPNAmt
		, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select sum(AfterDiscPpn) from #hasil where FPJNo = c.FPJNo) ELSE PPNAmt END PPNAmt
		, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select sum(ItemDPP) - sum(Potongan) from #hasil where FPJNo = c.FPJNo ) ELSE SubAmt END SubAmt
		--, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select  (sum(ItemDPP) - sum(Potongan)) + round(((sum(ItemDPP) - sum(Potongan)) * 0.1),0) from #hasil where FPJNo = c.FPJNo ) ELSE TotalAmt END TotalAmt
		--, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select  (sum(ItemDPP) - sum(Potongan)) + sum(AfterDiscPpn) from #hasil where FPJNo = c.FPJNo ) ELSE TotalAmt END TotalAmt
		, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select sum(TotalAmt) from #hasil where FPJNo = c.FPJNo ) ELSE TotalAmt END TotalAmt
		, TotQuantity, PPNBMPaid, cityNm, TaxPerson, JobTitle, ItemHeader, ItemOrder, ItemCode, ItemName, PPnBM, ItemQuantity, ItemDPP, Potongan, Keterangan
		, FlagShowInfo, HidePart, TaxPct, PPnBMSell
		, (SELECT COUNT(INVOICENO) FROM #hasil WHERE FPJNo = c.FPJNo) MaxRow
		, (select sum(ItemQuantity) from #hasil where FPJNo=c.FPJNo) SumQty
		, (select sum(ItemDPP) from #hasil where FPJNo = c.FPJNo ) XAmt
		, (select sum(Potongan) from #hasil where FPJNo = c.FPJNo ) XPotongan
		, @fInv ViewInvoice
	FROM #hasil c order by c.InvoiceNo, c.ItemOrder

DROP TABLE #hasil, #Others--, #t1
END
go

ALTER procedure [dbo].[uspfn_SvTrnInvoiceCancel]
	@CompanyCode   varchar(20),
	@BranchCode    varchar(20),
	@InvoiceNo     varchar(20),
	@UserInfo      varchar(max)
as  

declare @errmsg varchar(max)
declare @JobOrderNo varchar(20) 
declare @InvoiceDate DateTime

begin try
set nocount on
	set @JobOrderNo = (Select JobOrderNo from svTrnInvoice where CompanyCode = @CompanyCode and BranchCode = @BranchCode and InvoiceNo = @InvoiceNo)
	set @InvoiceDate = (Select InvoiceDate from svTrnInvoice where CompanyCode = @CompanyCode and BranchCode = @BranchCode and InvoiceNo = @InvoiceNo)

	declare @CompanyMD as varchar(15)
	declare @BranchMD as varchar(15)

	set @CompanyMD = (select CompanyMD from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)
	set @BranchMD = (select BranchMD from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)
	
	if (@CompanyCode != @CompanyMD and @BranchCode != @BranchMD)
	begin
		IF @InvoiceDate < getdate()
		begin
			raiserror('Tanggal invoice lebih kecil dari tanggal hari ini',16 ,1 );
		end
	end
	
	if exists (
	select * from ArInterface
	 where CompanyCode = @CompanyCode
	   and BranchCode  = @BranchCode
	   and (StatusFlag > '0' or ReceiveAmt > 0 or BlockAmt > 0 or DebetAmt > 0 or CreditAmt > 0)
	   and DocNo in (Select InvoiceNo from svTrnInvoice where CompanyCode = @CompanyCode and BranchCode = @BranchCode and JobOrderNo = @JobOrderNo)
	)
	begin
		raiserror('Invoice sudah ada proses Receiving, transaksi tidak bisa dilanjutkan',16 ,1 );
	end

	if exists (
	select * from svTrnFakturPajak
	 where CompanyCode = @CompanyCode
	   and BranchCode = @BranchCode
	   and FPJNo in (select FPJNo from svTrnInvoice where CompanyCode = @CompanyCode and BranchCode = @BranchCode and JobOrderNo = @JobOrderNo)
	   and isnull(FPJGovNo, '') <> ''
	   and left(FPJGovNo, 3) <> 'SHN'
	)
	begin
		raiserror('Invoice sudah tergenerate Nomor Pajak Pemerintah, transaksi tidak bisa dilanjutkan',16 ,1 );
	end

	;with x as (
		select a.BranchCode, a.CustomerCode, a.SalesAmt, b.TotalSrvAmt
		  from gnTrnBankBook a, svTrnInvoice b
		 where 1 = 1
		   and b.CompanyCode = a.CompanyCode
		   and b.BranchCode = a.BranchCode
		   and b.CustomerCodeBill = a.CustomerCode
		   and a.ProfitCenterCode = '200'
		   and a.CompanyCode = @BranchCode
		   and a.BranchCode = @BranchCode
		   and b.JobOrderNo = @JobOrderNo
	)
	update x set SalesAmt = SalesAmt - TotalSrvAmt where SalesAmt > 0

	delete from glInterface 
	 where CompanyCode = @CompanyCode
	   and BranchCode = @BranchCode
	   and DocNo in (Select InvoiceNo from svTrnInvoice where CompanyCode = @CompanyCode and BranchCode = @BranchCode and JobOrderNo = @JobOrderNo)
	delete from arInterface 
	 where CompanyCode = @CompanyCode
	   and BranchCode = @BranchCode
	   and DocNo in (Select InvoiceNo from svTrnInvoice where CompanyCode = @CompanyCode and BranchCode = @BranchCode and JobOrderNo = @JobOrderNo)

	update svTrnService set ServiceStatus = 5, InvoiceNo = ''
	 where CompanyCode = @CompanyCode
	   and BranchCode = @BranchCode
	   and JobOrderNo = @JobOrderNo

	delete svTrnFakturPajak
	 where CompanyCode = @CompanyCode
	   and BranchCode = @BranchCode
	   and FPJNo in (select FPJNo from svTrnInvoice where CompanyCode = @CompanyCode and BranchCode = @BranchCode and JobOrderNo = @JobOrderNo)

	-------------------------------------------------------------------------------------------------------------------
	-- Insert into table log
	-------------------------------------------------------------------------------------------------------------------
	declare @TransID   uniqueidentifier; 
	declare @TransCode varchar(20);

	set @TransID = newid()
	set @TransCode = 'CANCEL INVOICE' 

	insert into svTrnInvoiceLog (
		TransID,TransCode,CompanyCode,BranchCode,ProductType,InvoiceNo,InvoiceDate,InvoiceStatus,
		FPJNo,FPJDate,JobOrderNo,JobOrderDate,JobType,ServiceRequestDesc,ChassisCode,ChassisNo,
		EngineCode,EngineNo,PoliceRegNo,BasicModel,CustomerCode,CustomerCodeBill,Odometer,
		IsPKP,TOPCode,TOPDays,DueDate,SignedDate,LaborDiscPct,PartsDiscPct,MaterialDiscPct,
		PphPct,PpnPct,LaborGrossAmt,PartsGrossAmt,MaterialGrossAmt,LaborDiscAmt,PartsDiscAmt,MaterialDiscAmt,
		LaborDppAmt,PartsDppAmt,MaterialDppAmt,TotalDppAmt,TotalPphAmt,TotalPpnAmt,TotalSrvAmt,
		Remarks,PrintSeq,PostingFlag,PostingDate,CreatedBy,CreatedDate
	) 
	select 
		@TransID, @TransCode,CompanyCode,BranchCode,ProductType,InvoiceNo,InvoiceDate,InvoiceStatus,
		FPJNo,FPJDate,JobOrderNo,JobOrderDate,JobType,left(ServiceRequestDesc, 200),ChassisCode,ChassisNo,
		EngineCode,EngineNo,PoliceRegNo,BasicModel,CustomerCode,CustomerCodeBill,Odometer,
		IsPKP,TOPCode,TOPDays,DueDate,SignedDate,LaborDiscPct,PartsDiscPct,MaterialDiscPct,
		PphPct,PpnPct,LaborGrossAmt,PartsGrossAmt,MaterialGrossAmt,LaborDiscAmt,PartsDiscAmt,MaterialDiscAmt,
		LaborDppAmt,PartsDppAmt,MaterialDppAmt,TotalDppAmt,TotalPphAmt,TotalPpnAmt,TotalSrvAmt,
		Remarks,PrintSeq,PostingFlag,PostingDate,@UserInfo,CreatedDate
	from svTrnInvoice
	 where CompanyCode = @CompanyCode and BranchCode = @BranchCode and InvoiceNo in (Select InvoiceNo from svTrnInvoice where CompanyCode = @CompanyCode and BranchCode = @BranchCode and JobOrderNo = @JobOrderNo)

	insert into svTrnInvTaskLog(TransID,TransCode,CompanyCode,BranchCode,ProductType,InvoiceNo,OperationNo,OperationHour,ClaimHour,OperationCost,SubConPrice,IsSubCon,SharingTask)
	select @TransID,@TransCode,CompanyCode,BranchCode,ProductType,InvoiceNo,OperationNo,OperationHour,ClaimHour,OperationCost,SubConPrice,IsSubCon,SharingTask from svTrnInvTask 
	 where CompanyCode = @CompanyCode and BranchCode = @BranchCode and InvoiceNo in (Select InvoiceNo from svTrnInvoice where CompanyCode = @CompanyCode and BranchCode = @BranchCode and JobOrderNo = @JobOrderNo)

	insert into svTrnInvItemLog(TransID,TransCode,CompanyCode, BranchCode, ProductType, InvoiceNo, PartNo, MovingCode, ABCClass, SupplyQty, ReturnQty, CostPrice, RetailPrice, TypeOfGoods)
	select @TransID,@TransCode,CompanyCode, BranchCode, ProductType, InvoiceNo, PartNo, MovingCode, ABCClass, SupplyQty, ReturnQty, CostPrice, RetailPrice, TypeOfGoods from svTrnInvItem
	 where CompanyCode = @CompanyCode and BranchCode = @BranchCode and InvoiceNo in (Select InvoiceNo from svTrnInvoice where CompanyCode = @CompanyCode and BranchCode = @BranchCode and JobOrderNo = @JobOrderNo)

	delete svTrnInvItemDtl where CompanyCode = @CompanyCode and BranchCode = @BranchCode and InvoiceNo in (Select InvoiceNo from svTrnInvoice where CompanyCode = @CompanyCode and BranchCode = @BranchCode and JobOrderNo = @JobOrderNo)
	delete svTrnInvItem where CompanyCode = @CompanyCode and BranchCode = @BranchCode and InvoiceNo in (Select InvoiceNo from svTrnInvoice where CompanyCode = @CompanyCode and BranchCode = @BranchCode and JobOrderNo = @JobOrderNo)
	delete svTrnInvMechanic where CompanyCode = @CompanyCode and BranchCode = @BranchCode and InvoiceNo in (Select InvoiceNo from svTrnInvoice where CompanyCode = @CompanyCode and BranchCode = @BranchCode and JobOrderNo = @JobOrderNo)
	delete svTrnInvTask where CompanyCode = @CompanyCode and BranchCode = @BranchCode and InvoiceNo in (Select InvoiceNo from svTrnInvoice where CompanyCode = @CompanyCode and BranchCode = @BranchCode and JobOrderNo = @JobOrderNo)
	delete svTrnInvoice where CompanyCode = @CompanyCode and BranchCode = @BranchCode and InvoiceNo in (Select InvoiceNo from svTrnInvoice where CompanyCode = @CompanyCode and BranchCode = @BranchCode and JobOrderNo = @JobOrderNo)
	
	if (@CompanyCode != @CompanyMD and @BranchCode != @BranchMD)
	begin
	declare @Query VARCHAR(MAX)
		
	set @Query ='delete from '+dbo.GetDbMD(@CompanyCode, @BranchCode)+'..svSDMovement 
		where CompanyCode = '''+ @CompanyCode +''' and BranchCode = '''+ @BranchCode +'''
		and DocNo in (Select InvoiceNo from svTrnInvoice where CompanyCode = '''+ @CompanyCode +''' and BranchCode = '''+ @BranchCode +''' and JobOrderNo = '''+ @JobOrderNo +''')'	
	end
end try
begin catch
    set @errmsg = 'InvoiceNo : ' + @InvoiceNo + char(13) + 'Error Message:' + char(13) + error_message()
    raiserror (@errmsg,16,1);
end catch
go

GO

/****** Object:  StoredProcedure [dbo].[usprpt_SpRpTrn035]    Script Date: 6/16/2015 2:05:31 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




ALTER procedure [dbo].[usprpt_SpRpTrn035]
--declare
   @CompanyCode		VARCHAR(15),
   @BranchCode		VARCHAR(15),
   @FPJDateStart	DATETIME,
   @FPJDateEnd		DATETIME,
   @FPJNoStart		VARCHAR(30),
   @FPJNoEnd		VARCHAR(30),
   @ProfitCenter	VARCHAR(15),
   @SeqNo			INT
   
AS   
BEGIN

--SELECT @CompanyCode= '6145203',@BranchCode= '614520301',@FPJDateStart= '20150501',@FPJDateEnd= '20150531',@FPJNoStart= '010.001-15.29139679',
--@FPJNoEnd= '010.001-15.29139679',@ProfitCenter= '300',@SeqNo= 1

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
declare @fInv		int

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
set @fInv		= (select isnull(ParaValue,'1') from gnmstlookupdtl where codeid='FPJFLAG' and lookupvalue='SPARE')		

-- parameter use address holding or not
declare @IsHoldingAddr as bit
if (select count(*) from gnMstLookUpDtl where CompanyCode= @CompanyCode and CodeID='FPJADDR') > 0
	set @IsHoldingAddr = (select convert(numeric,LookUpValue) from gnMstLookUpDtl where CompanyCode= @CompanyCode and CodeID='FPJADDR')
else
	set @IsHoldingAddr = 0

declare @PaymentInfo as bit
if (select count(*) from gnMstLookUpDtl where CompanyCode= @CompanyCode and CodeID='PINF') > 0
	set @PaymentInfo = (select convert(numeric,ParaValue) from gnMstLookUpDtl where CompanyCode= @CompanyCode and CodeID='PINF' AND LookUpValue='STAT')
else
	set @PaymentInfo = 1

select * into #t1
from (
	SELECT	
		a.TPTrans, 
		case when @fInfoPKP = 1 then
			(case @fStatus when '1' then @fCompName else e.CompanyGovName end)
		else '' end as compNm, 
		case @fStatus when '1' then @fSKP else e.SKPNo end as compSKPNo, 		
		case when @fInfoPKP = 1 then
			(case @fStatus when '1' then @fSKPDate else e.SKPDate end)
		else '' end as compSKPDate,
		case when @fInfoPKP = 1 then
			(case @fStatus when '1' then @fAdd1 else 
				(case when @IsHoldingAddr=0 then ISNULL(e.Address1, '') + ' ' + ISNULL(e.Address2, '') + ' ' + ISNULL(e.Address3, '')
					else (select ISNULL(Address1, '') + ' ' + ISNULL(Address2, '') + ' ' + ISNULL(Address3, '') from gnMstCoProfile where CompanyCode=@CompanyCode
					and BranchCode=(select BranchCode from gnMstOrganizationDtl where CompanyCode=@CompanyCode and IsBranch=0))
				end)end)
		else '' end AS compAddr1,
		case when @fInfoPKP = 1 then
			(case @fStatus when '1' then @fAdd2 else 
				(case when @IsHoldingAddr=0 then ISNULL(e.Address2, '')
					else (select ISNULL(e.Address2, '') from gnMstCoProfile where CompanyCode=@CompanyCode
					and BranchCode=(select BranchCode from gnMstOrganizationDtl where CompanyCode=@CompanyCode and IsBranch=0))
				end)end)
		else '' end AS compAddr2
		,case @fStatus when '1' then '' else 
			(case when @IsHoldingAddr=0 then ISNULL(e.Address3, '')
				else (select ISNULL(e.Address3, '') from gnMstCoProfile where CompanyCode=@CompanyCode
				and BranchCode=(select BranchCode from gnMstOrganizationDtl where CompanyCode=@CompanyCode and IsBranch=0))
			end)
		 end AS compAddr3
		,case @fStatus when '1' then '' else 
			(case when @IsHoldingAddr=0 then ISNULL(e.Address4, '')
				else (select ISNULL(e.Address4, '') from gnMstCoProfile where CompanyCode=@CompanyCode
				and BranchCode=(select BranchCode from gnMstOrganizationDtl where CompanyCode=@CompanyCode and IsBranch=0))
			end)
		 end AS compAddr4,
		case @fStatus when '1' then '' else e.PhoneNo end as compPhoneNo, 
		case @fStatus when '1' then '' else e.FaxNo  end as compFaxNo, 
		case when @fInfoPKP = 1 then
			(case @fStatus when '1' then @fNPWP else e.NPWPNo  end)
		else '' end as compNPWPNo, 
		a.FPJNo  fakturFPJNo, 
		a.FPJDate  fakturFPJDate, 
		a.InvoiceNo  fakturInvNo,
		a.InvoiceDate  fakturInvDate, 
		a.FPJGovNo  fakturFPJGovNo, 
		a.PickingSlipNo  fakturPickSlipNo,
		/* RETURN MORE THAN 1 VALUE NEED MORE CHECK, TEMPORARY USING TOP 1 */ 
		--New--
		(SELECT TOP 1 g.OrderNo+', '+CONVERT(VARCHAR(20),g.OrderDate,105)
			FROM spTrnSORDHdr g
				LEFT JOIN spTrnSFPJDtl h ON g.CompanyCode = h.CompanyCode AND g.BranchCode = h.BranchCode AND g.DocNo = h.DocNo
			WHERE h.CompanyCode = a.CompanyCode AND h.CompanyCode = a.CompanyCode AND h.FPJNo = a.FPJNo
			GROUP BY g.OrderNo, g.OrderDate
		 )as OrderFeld,
		--End new 
		a.CustomerCode  fakturCustCode,
		x.CustomerName  custName, 
		d.SKPNo  custSKPNo, 
		d.SKPDate custSKPDate, 
		x.Address1 custAddr1, 
		x.Address2 custAddr2, 
		x.Address3 custAddr3, 
		x.Address4 custAddr4, 
		d.PhoneNo custPhoneNo, 
		d.FaxNo custFaxNo, 
		d.NPWPNo custNPWPNo, 
		a.DueDate fakturDueDate, 
		a.TotSalesQty fakturTotSaleQty, 
		a.TotSalesAmt fakturTotSalesAmt, 
		a.TotDiscAmt fakturTotDiscAmt, 
		a.TotDppAmt fakturTotDppAmt, 
		a.TotPPNAmt fakturPPNAmt, 
		a.TotFinalSalesAmt fakturTotFinalSalesAmt, 
		a.FPJSignature, 
		c.TaxPct  taxPercent, 
		case @fStatus when '1' then @fCity else 
		(SELECT LookUpValueName FROM gnMstLookupDtl WHERE CodeID = 'CITY' AND LookUpValue = e.CityCode) end as cityNm, 
		UPPER (f.SignName) SignName, 
		UPPER (f.TitleSign) TitleSign,
		g.PartNo,
		h.PartName,
		g.QtyBill,
		g.SalesAmt,
		g.DiscAmt,
		g.PPNAmt,
		(
			SELECT SUM(QtyBill) FROM spTrnSFPJDtl WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND FPJNo = a.FPJNo
		) AS TotQtyBill,
		(
		SELECT COUNT (PartNo) FROM spTrnSFPJDtl WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND FPJNo = a.FPJNo
		) AS JumlahPart
		,case when @fInfoPKP = 1 then
			(case @fStatus when '1' then @fAdd1 else 
			(case when @IsHoldingAddr=0 then e.Address1+' '+e.Address2
				else (select Address1+' '+Address2 from gnMstCoProfile where CompanyCode=@CompanyCode
				and BranchCode=(select BranchCode from gnMstOrganizationDtl where CompanyCode=@CompanyCode and IsBranch=0))
			end)end)
		 else '' end as Alamat1
		,case when @fInfoPKP = 1 then
			(case @fStatus when '1' then @fAdd2 else 
			(case when @IsHoldingAddr=0 then e.Address3+' '+e.Address4
				else (select Address3+' '+Address4 from gnMstCoProfile where CompanyCode=@CompanyCode
				and BranchCode=(select BranchCode from gnMstOrganizationDtl where CompanyCode=@CompanyCode and IsBranch=0))
			end)end)
		 else '' end as Alamat2,
		ISNULL(x.Address1,'')+' '+ISNULL(x.Address2,'') Alamat3,
		ISNULL(x.Address3,'')+' '+ISNULL(x.Address4,'') Alamat4,
		g.PartNo+' - '+h.PartName Item
		,CASE 
			WHEN @PaymentInfo=1 THEN 'DILUNASI DENGAN ' + (select LookUpValueName from gnMstLookUpDtl where CompanyCode=a.CompanyCode and CodeID='PYBY' and LookUpValue= i.PaymentCode)
			ELSE '' 
		end PaymentInfo,
		 ISNULL((SELECT TOP 1 ParaValue FROM GnMstLookUpDtl WHERE CompanyCode = @CompanyCode AND CodeID = 'FPIF'),'0') FlagDetails
		,(select count(PartNo) from spTrnSFPJDtl where CompanyCode=a.CompanyCode and BranchCode=a.BranchCode and FPJNo=a.FPJNo) MaxRow
	FROM 
		SpTrnSFPJHdr a WITH (NOLOCK, NOWAIT)
	LEFT JOIN GnMstTax c WITH (NOLOCK, NOWAIT)
		ON a.CompanyCode = c.CompanyCode AND c.TaxCode = 'PPN'
	LEFT JOIN GnMstCustomer d WITH (NOLOCK, NOWAIT) ON a.CompanyCode = d.CompanyCode
		AND a.CustomerCode = d.CustomerCode
	LEFT JOIN SpTrnSFPJInfo x WITH (NOLOCK, NOWAIT) ON a.CompanyCode = x.CompanyCode
		AND a.BranchCode = x.BranchCode
		AND a.FPJNo = x.FPJNo
	LEFt JOIN GnMstCoProfile e WITH (NOLOCK, NOWAIT) ON a.CompanyCode = e.CompanyCode
		AND a.BranchCode = e.BranchCode
	LEFT JOIN GnMstSignature f WITH (NOLOCK, NOWAIT) ON a.CompanyCode = f.CompanyCode
		AND a.BranchCode = f.BranchCode
		AND f.ProfitCenterCode = @ProfitCenter
		AND f.DocumentType = CONVERT (VARCHAR (3), a.FPJNo)
		AND f.SeqNo = @SeqNo
	LEFT JOIN spTrnSFPJDtl g WITH (NOLOCK, NOWAIT) ON g.CompanyCode = a.CompanyCode
		AND g.BranchCode = a.BranchCode
		AND g.FPJNo = a.FPJNo
	INNER JOIN spMstItemInfo h WITH (NOLOCK, NOWAIT) ON h.CompanyCode = a.CompanyCode
		AND h.PartNo = g.PartNo
	LEFT JOIN gnMstCustomerProfitCenter i on a.CompanyCode=i.CompanyCode and a.BranchCode=i.BranchCode
		AND a.CustomerCode=i.CustomerCode and i.ProfitCenterCode='300'
	WHERE 
		a.CompanyCode = @CompanyCode
		AND a.BranchCode = @BranchCode AND a.isPKP = 1
		AND CONVERT(VARCHAR, a.FPJSignature, 112) BETWEEN @FPJDateStart AND @FPJDateEnd
		AND ((CASE WHEN @FPJNoStart = '' THEN a.FPJGovNo END) <> ''
		OR (CASE WHEN @FPJNoStart <> '' THEN a.FPJGovNo END) BETWEEN @FPJNoStart AND @FPJNoEnd)
) #t1

select * into #t2 from(
select TPTrans, compNm, compSKPNo, compSKPDate, compAddr1, compAddr2, compAddr3, compAddr4, compPhoneNo, compFaxNo, compNPWPNo,
	case when (a.countFaktur) > 1 then (select top 1 fakturFPJNo from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturFPJNo) + '-' + (select substring((select top 1 fakturFPJNo from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturFPJNo desc), 8, 7)) else (select top 1 fakturFPJNo from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturFPJNo, 
	case when (a.countFaktur) > 1 then (select top 1 fakturFPJDate from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturFPJNo desc) else (select top 1 fakturFPJDate from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturFPJDate,
	case when (a.countFaktur) > 1 then (select top 1 fakturInvNo from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturInvNo) + '-' + (select substring((select top 1 fakturInvNo from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturInvNo desc), 8, 7)) else (select top 1 fakturInvNo from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturInvNo, 
	case when (a.countFaktur) > 1 then (select top 1 fakturInvDate from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturInvNo desc) else (select top 1 fakturInvDate from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturInvDate,
	fakturFPJGovNo,  
	case when (a.countFaktur) > 1 then (select top 1 fakturPickSlipNo from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturFPJNo) + '-' + (select substring((select top 1 fakturPickSlipNo from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturFPJNo desc), 8, 7)) else (select top 1 fakturPickSlipNo from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturPickSlipNo,
	case when (a.countFaktur) > 1 then ' ' else (select top 1 OrderFeld from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end OrderFeld,
	fakturCustCode, custName, custSKPNo, custSKPDate, custAddr1, custAddr2, custAddr3, custAddr4, custPhoneNo, custFaxNo, custNPWPNo, 
	case when (a.countFaktur) > 1 then (select top 1 fakturDueDate from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturFPJNo desc) else (select top 1 fakturDueDate from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturDueDate,
	
	case when (a.countFaktur) > 1 then (select sum(QtyBill) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) else sum(fakturTotSaleQty) end fakturTotSaleQty, 
	case when (a.countFaktur) > 1 then (select sum(SalesAmt) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) else sum(fakturTotSalesAmt) end fakturTotSalesAmt, 
	case when (a.countFaktur) > 1 then (select sum(DiscAmt) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) else sum(fakturTotDiscAmt) end fakturTotDiscAmt,
	case when (a.countFaktur) > 1 then (select (sum(SalesAmt) - sum(DiscAmt)) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) else sum(fakturTotDPPAmt) end fakturTotDppAmt, 
	--case when (a.countFaktur) > 1 then ((select (sum(SalesAmt) - sum(DiscAmt)) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) * 0.1) else sum(fakturPPNAmt) end fakturPPNAmt, 
	--sum(fakturPPNAmt) fakturPPNAmt, 
	case when (a.countFaktur) > 1 then ((select (sum(PPNAmt)) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo)) else sum(fakturPPNAmt) end fakturPPNAmt, 
	--case when (a.countFaktur) > 1 then ((select (sum(SalesAmt) - sum(DiscAmt)) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) + ((select (sum(SalesAmt) - sum(DiscAmt)) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) * 0.1)) else sum(fakturTotFinalSalesAmt) end fakturTotFinalSalesAmt, 
	--case when (a.countFaktur) > 1 then ((select (sum(SalesAmt) - sum(DiscAmt)) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) + (select sum(fakturPPNAmt) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo)) else sum(fakturTotFinalSalesAmt) end fakturTotFinalSalesAmt, 
	case when (a.countFaktur) > 1 then ((select (sum(SalesAmt) - sum(DiscAmt)) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) + ((select (sum(PPNAmt)) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo))) else sum(fakturTotFinalSalesAmt) end fakturTotFinalSalesAmt, 

	case when (a.countFaktur) > 1 then (select top 1 FPJSignature from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturFPJNo desc) else (select top 1 FPJSignature from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end FPJSignature,
	TaxPercent, cityNm, SignName, TitleSign, PartNo, PartName, sum(QtyBill) QtyBill, sum(SalesAmt) SalesAmt, 
	case when (a.countFaktur) > 1 then (select sum(QtyBill) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) else sum(TotQtyBill) end TotQtyBill, 
	sum(JumlahPart) JumlahPart, Alamat1, Alamat2, Alamat3, Alamat4, Item, PaymentInfo, FlagDetails, sum(MaxRow) MaxRow, a.countFaktur
from
(
select distinct TPTrans, compNm, compSKPNo, compSKPDate, compAddr1, compAddr2, compAddr3, compAddr4, compPhoneNo, compFaxNo, compNPWPNo, fakturFPJNo,
	fakturFPJDate, fakturInvNo, fakturInvDate, #t1.fakturFPJGovNo, fakturPickSlipNo, OrderFeld, fakturCustCode, custName, custSKPNo, custSKPDate, custAddr1, 
	custAddr2, custAddr3, custAddr4, custPhoneNo, custFaxNo, custNPWPNo, fakturDueDate, fakturTotSaleQty, fakturTotSalesAmt, fakturTotDiscAmt, fakturTotDPPAmt,
	fakturPPNAmt, fakturTotFinalSalesAmt, FPJSignature, TaxPercent, cityNm, SignName, TitleSign, PartNo, PartName, sum(QtyBill) QtyBill, sum(SalesAmt) SalesAmt,
	sum(DiscAmt) DiscAmt, TotQtyBill, JumlahPart, Alamat1, Alamat2, Alamat3, Alamat4, Item, PaymentInfo, FlagDetails, b.countFaktur, sum(MaxRow) MaxRow
from #t1 
left join (select fakturFPJGovNo, count(fakturFPJNo) countFaktur
		from(select distinct fakturFPJGovNo, fakturFPJNo from #t1) a
		group by fakturFPJGovNo) b on #t1.fakturFPJGovNo = b.fakturFPJGovNo
group by TPTrans, compNm, compSKPNo, compSKPDate, compAddr1, compAddr2, compAddr3, compAddr4, compPhoneNo, compFaxNo, compNPWPNo, fakturFPJNo,
	fakturFPJDate, fakturInvNo, fakturInvDate, #t1.fakturFPJGovNo, fakturPickSlipNo, OrderFeld, fakturCustCode, custName, custSKPNo, custSKPDate, custAddr1, 
	custAddr2, custAddr3, custAddr4, custPhoneNo, custFaxNo, custNPWPNo, fakturDueDate, fakturTotSaleQty, fakturTotSalesAmt, fakturTotDiscAmt, fakturTotDPPAmt,
	fakturPPNAmt, fakturTotFinalSalesAmt, FPJSignature, TaxPercent, cityNm, SignName, TitleSign, PartNo, PartName, Alamat1, Alamat2, Alamat3, Alamat4, Item, 
	PaymentInfo, FlagDetails, b.countFaktur, JumlahPart, TotQtyBill
) a
group by fakturFPJGovNo, TPTrans, compNm, compSKPNo, compSKPDate, compAddr1, compAddr2, compAddr3, compAddr4, compPhoneNo, compFaxNo, compNPWPNo, 
	fakturCustCode, custName, custSKPNo, custSKPDate, custAddr1, custAddr2, custAddr3, custAddr4, custPhoneNo, custFaxNo, custNPWPNo, TaxPercent, cityNm, 
	SignName, TitleSign, PartNo, PartName, Alamat1, Alamat2, Alamat3, Alamat4, PaymentInfo, FlagDetails, item, a.countFaktur
) #t2

select TPTrans, compNm, compSKPNo, compSKPDate, compAddr1, compAddr2, compAddr3, compAddr4, compPhoneNo, compFaxNo, compNPWPNo, fakturFPJNo, fakturFPJDate
	, fakturInvDate, fakturFPJGovNo, fakturPickSlipNo, OrderFeld, fakturCustCode, custName, custSKPNo, custSKPDate, custAddr1, custAddr2, custAddr3
	, custAddr4, custPhoneNo, custFaxNo, custNPWPNo, fakturDueDate, fakturTotSaleQty
	, fakturTotSalesAmt, fakturTotDiscAmt, fakturTotDppAmt, fakturPPNAmt
	, fakturTotFinalSalesAmt, FPJSignature, TaxPercent, cityNm, SignName, TitleSign
	, PartNo, PartName, QtyBill, SalesAmt, TotQtyBill
	, case when (a.countFaktur) > 1 then (select count(Item) from #t2 where fakturFPJGovNo = a.fakturFPJGovNo) else JumlahPart end JumlahPart
	, Alamat1, Alamat2, Alamat3, Alamat4, Item, PaymentInfo, FlagDetails
	, case when (a.countFaktur) > 1 then (select count(Item) from #t2 where fakturFPJGovNo = a.fakturFPJGovNo) else MaxRow end MaxRow
	, case when (case when (a.countFaktur) > 1 then (select count(Item) from #t2 where fakturFPJGovNo = a.fakturFPJGovNo) else MaxRow end) % 17 = 1 then 16 else 17 end as PageBreak
	, isnull(@fInv,1) ShowInvoice
from #t2 a

	drop table #t1, #t2
END




GO

IF OBJECT_ID('[dbo].[uspfn_OmFPDetailCustomer]') IS NOT NULL
DROP PROCEDURE [dbo].[uspfn_OmFPDetailCustomer]

GO
/****** Object:  StoredProcedure [dbo].[uspfn_OmFPDetailCustomer]    Script Date: 6/17/2015 9:33:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [dbo].[uspfn_OmFPDetailCustomer]
	@ChassisCode as varchar(15)
	,@ChassisNo as varchar(15)
as

declare @recSoVin as Int

set @recSoVin = (SELECT Count(*) FROM omTrSalesSOVin WHERE ChassisCode=@ChassisCode AND ChassisNo=@ChassisNo )

if (@recSoVin) > 0 --Sovin
begin
	SELECT a.CompanyCode, a.BranchCode, d.BPKNo, a.SONo, a.EndUserName, b.RefferenceNo SKPKNo, a.EndUserAddress1, a.EndUserAddress2, a.EndUserAddress3, c.CustomerName, c.Address1, c.Address2, c.Address3,
		c.CityCode,(SELECT LookUpValueName FROM gnMstLookUpDtl where CodeID = 'CITY' AND ParaValue = c.CityCode) as CityName, 
		c.PhoneNo, c.HPNo, c.birthDate ,b.Salesman, (SELECT EmployeeName FROM gnMstEmployee where EmployeeID = b.Salesman AND CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode) SalesmanName, b.SalesType
	FROM omTrSalesSOVin a
		left join omTrSalesSO b on a.companyCode = b.companyCode 
			and a.BranchCode= b.BranchCode
			and a.SONo = b.SONo
			and b.Status = '2'
		left join gnMstCustomer c on b.CompanyCode = c.CompanyCode
			and b.CustomerCode = c.CustomerCode
		left join omTrSalesBPK d on a.CompanyCode = d.CompanyCode
			and a.BranchCode = d.BranchCode
			and a.SONo = d.SONo
	WHERE a.ChassisCode=@ChassisCode AND a.ChassisNo=@ChassisNo
end
else --dodetail
	SELECT a.CompanyCode, a.BranchCode, d.BPKNo, e.SONo, 
		c.CustomerName EndUserName, b.RefferenceNo SKPKNo, c.Address1 EndUserAddress1, c.Address2 EndUserAddress2, c.Address3 EndUserAddress3, 
		c.CustomerName, c.Address1, c.Address2, c.Address3,
		c.CityCode,(SELECT LookUpValueName FROM gnMstLookUpDtl where CodeID = 'CITY' AND ParaValue = c.CityCode) as CityName, 
		c.PhoneNo, c.HPNo, c.birthDate ,b.Salesman, (SELECT EmployeeName FROM gnMstEmployee where EmployeeID = b.Salesman AND CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode) SalesmanName, b.SalesType
	FROM omTrSalesDODetail a
	left join omTrSalesDO e on a.CompanyCode = e.CompanyCode
		and a.BranchCode = e.BranchCode
		and a.Dono = e.Dono 
	left join omTrSalesSO b on a.companyCode = b.companyCode 
		and a.BranchCode= b.BranchCode
		and e.SONo = b.SONo
	left join gnMstCustomer c on b.CompanyCode = c.CompanyCode
		and b.CustomerCode = c.CustomerCode
	left join omTrSalesBPK d on a.CompanyCode = d.CompanyCode
		and a.BranchCode = d.BranchCode
		and e.SONo = d.SONo
	WHERE a.ChassisCode=@ChassisCode AND a.ChassisNo=@ChassisNo	
go

ALTER procedure [dbo].[uspfn_SvTrnServiceSelectDtl]
--declare
	@CompanyCode varchar(15),
	@BranchCode  varchar(15),
	@ProductType varchar(15),
	@ServiceNo   bigint
as      

begin
--select @CompanyCode='6006400001',	@BranchCode='6006400104', @ProductType='4W', @ServiceNo=58463

declare @t1 as table
(
 TaskPartSeq int
,BillType varchar(10)
,BillTypeDesc varchar(max)
,TypeOfGoods varchar(10)
,TypeOfGoodsDesc varchar(70)
,TaskPartNo varchar(50)
,OprHourDemandQty numeric(18,2)
,SupplyQty numeric(18,2)
,ReturnQty numeric(18,2)
,OprRetailPrice numeric(18,2)
,OprRetailPriceTotal numeric(18,2)
,SupplySlipNo varchar(20)
,TaskPartDesc varchar(max)
,BasicModel varchar(15)
,JobType varchar(15)
,IsSubCon bit
,Status varchar(10)
,GTGO varchar(10)
,DiscPct numeric(18,2)
,QtyAvail numeric(18,2)
,TaskStatus varchar(50)
)

declare @JobOrderNo varchar(15)
    set @JobOrderNo = isnull((select JobOrderNo from svTrnService where CompanyCode = @CompanyCode and BranchCode = @BranchCode and ProductType = @ProductType and ServiceNo = @ServiceNo), '')

insert into @t1
select 0 TaskSeq 
      ,a.BillType
      ,b.Description BillTypeDesc
      ,a.TypeOfGoods
      ,case a.TypeOfGoods when 'L' then 'Labor (Jasa)' end TypeOfGoodsDesc
      ,a.OperationNo
      ,a.OperationHour
      ,0 OperationHourSupply
      ,0 OperationHourReturn
      ,a.OperationCost
      ,a.OperationHour * a.OperationCost * (100 - a.DiscPct) * 0.01 OprRetailPriceTotal
      ,'' SupplySlipNo
      ,rtrim(d.Description) OperationDesc 
	  ,c.BasicModel
	  ,c.JobType
	  ,a.IsSubCon
	  ,(select min(MechanicStatus) from svTrnSrvMechanic 
		where CompanyCode = a.CompanyCode 
			and BranchCode = a.BranchCode
			and ProductType = a.ProductType
			and ServiceNo = a.ServiceNo
			and OperationNo = a.OperationNo) MechanicStatus
	  ,''
	  ,a.DiscPct
	  ,0
      ,case(a.TaskStatus)
          when 0 then 'Open Task'
          when 1 then 'Work In Progress'
          when 2 then 'Close Task'
          when 9 then 'Cancel'
       end TaskStatus
  from svTrnSrvTask a with (nolock,nowait)
  left join svMstBillingType b with (nolock,nowait)
    on b.CompanyCode = a.CompanyCode
   and b.BillType = a.BillType
  left join svTrnService c with (nolock,nowait)
    on c.CompanyCode = a.CompanyCode
   and c.BranchCode = a.BranchCode
   and c.ProductType = a.ProductType
   and c.ServiceNo = a.ServiceNo
  left join svMstTask d with (nolock,nowait)
    on d.CompanyCode = a.CompanyCode
   and d.ProductType = a.ProductType
   and d.BasicModel = c.BasicModel
   and (d.JobType = c.JobType or d.JobType = 'CLAIM' or d.JobType = 'OTHER')
   and d.OperationNo = a.OperationNo 
 where 1 = 1
   and a.CompanyCode = @CompanyCode
   and a.BranchCode  = @BranchCode
   and a.ProductType = @ProductType
   and a.ServiceNo   = @ServiceNo

declare @tblTemp as table
(
	PartNo  varchar(20),
	QtyAvail numeric(18,2)
)

declare @DealerCode as varchar(2)
declare @CompanyMD as varchar(15)
declare @BranchMD as varchar(15)

set @DealerCode = 'MD'
set @CompanyMD = (select CompanyMD from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)
set @BranchMD = (select BranchMD from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)

if (@CompanyCode != @CompanyMD and @BranchCode != @BranchMD)
begin	
	set @DealerCode = 'SD'
	declare @DbName as varchar(50)
	set @DbName = (select DbMD from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)
	
	declare @QueryTemp as varchar(max)
	
	set @QueryTemp = 'select 
			 distinct
			 a.PartNo
			 , (b.OnHand - (b.AllocationSP + b.AllocationSR + b.AllocationSL) - (b.ReservedSP + b.ReservedSR + b.ReservedSL)) 
		 from svTrnSrvItem a	 
		 left join ' + @DbName + '..spMstItems b on 
			a.PartNo = b.PartNo 
			and b.CompanyCode = ''' + @CompanyMD + '''
			and b.BranchCode = ''' + @BranchMD + '''
		 where a.CompanyCode = ''' + @CompanyCode + '''
		   and a.BranchCode  = ''' + @BranchCode + '''
		   and a.ProductType = ''' + @ProductType + '''
		   and a.ServiceNo   = ' + convert(varchar,@ServiceNo) + ''		   
				
		print(@QueryTemp)		
		insert into @tblTemp		
		exec (@QueryTemp)		
end

insert into @t1
select a.PartSeq
      ,a.BillType
      ,b.Description BillTypeDesc
      ,a.TypeOfGoods
      ,rtrim(c.LookupValueName) + case lower(g.ParaValue) when 'sparepart' then ' (SPR)' else ' (MTR)' end TypeOfGoodsDesc
      ,a.PartNo
      ,a.DemandQty
      ,a.SupplyQty
      ,a.ReturnQty
      ,a.RetailPrice
      ,(case isnull(a.SupplyQty, 0)
         when 0 then (isnull(a.DemandQty, 0) * isnull(a.RetailPrice, 0))
         else ((isnull(a.SupplyQty, 0) - isnull(a.ReturnQty, 0)) * isnull(a.RetailPrice, 0))
        end) * (100.0 - a.DiscPct) * 0.01
        as RetailPriceTotal
      ,a.SupplySlipNo
      ,rtrim(d.PartName) OperationDesc 
	  ,''
	  ,''
	  ,0
	  ,''
	  ,g.ParaValue
	  ,a.DiscPct
	  ,case when @DealerCode = 'MD' then (i.OnHand - (i.AllocationSP + i.AllocationSR + i.AllocationSL) - (i.ReservedSP + i.ReservedSR + i.ReservedSL)) else e.QtyAvail end QtyAvail
	  ,''
  from svTrnSrvItem a with (nolock,nowait)
  left join svMstBillingType b with (nolock,nowait)
    on b.CompanyCode = a.CompanyCode
   and b.BillType = a.BillType
  left join gnMstLookupDtl c with (nolock,nowait)
    on c.CompanyCode = a.CompanyCode
   and c.CodeID = 'TPGO'
   and c.LookupValue = TypeOfGoods
  left join spMstItemInfo d with (nolock,nowait)
    on d.CompanyCode = a.CompanyCode
   and d.PartNo = a.PartNo
  left join gnMstLookupDtl g with (nolock,nowait)
    on g.CompanyCode = a.CompanyCode
   and g.CodeID = 'GTGO'
   and g.LookupValue = TypeOfGoods
  left join svTrnService s with (nolock,nowait)
    on s.CompanyCode = a.CompanyCode
   and s.BranchCode = a.BranchCode
   and s.ServiceNo = a.ServiceNo
  left join spMstItems i
    on i.CompanyCode = a.CompanyCode 
   and i.BranchCode = a.BranchCode
   and i.ProductType = a.ProductType
   and i.PartNo = a.PartNo
   left join @tblTemp e
    on e.PartNo = a.PartNo
 where 1 = 1
   and a.CompanyCode = @CompanyCode
   and a.BranchCode  = @BranchCode
   and a.ProductType = @ProductType
   and a.ServiceNo   = @ServiceNo

select * into #t1 from (
select 
 a.* 
,P01 = isnull((
	select count(*) from spTrnSORDDtl with(nowait,nolock)
	 where 1 = 1
	   and CompanyCode = @CompanyCode
	   and BranchCode  = @BranchCode
	   and PartNo      = a.TaskPartNo
	   and DocNo in (select aa.DocNo
					   from spTrnSORDHdr aa with(nowait,nolock), svTrnService bb with(nowait,nolock)
					  where 1 = 1
						and bb.CompanyCode = aa.CompanyCode
						and bb.BranchCode  = aa.BranchCode
						and bb.JobOrderNo  = aa.UsageDocNo
						and isnull(bb.JobOrderNo, '') <> ''
						and aa.CompanyCode = @CompanyCode
						and aa.BranchCode  = @BranchCode
						and bb.ServiceNo   = @ServiceNo
					 )
	),0)
,P02 = isnull((
	select count(DocNo) from spTrnSOSupply with(nowait,nolock)
	 where 1 = 1
	   and CompanyCode = @CompanyCode
	   and BranchCode  = @BranchCode
	   and PartNo = a.TaskPartNo
	   and DocNo  = a.SupplySlipNo
	),0)
,P03 = isnull((
	select count(bb.PickingSlipNo) from spTrnSOSupply aa with(nowait,nolock)
	  left join spTrnSPickingHdr bb with(nowait,nolock) on 1 = 1
	   and bb.CompanyCode   = aa.CompanyCode
	   and bb.BranchCode    = aa.BranchCode
	   and bb.PickingSlipNo = aa.PickingSlipNo
	 where 1 = 1
	   and aa.CompanyCode = @CompanyCode
	   and aa.BranchCode  = @BranchCode
	   and aa.PartNo = a.TaskPartNo
	   and aa.DocNo  = a.SupplySlipNo
	   and isnull(aa.PickingSlipNo, '') <> ''
	),0)
,P04 = isnull((
	select count(bb.PickingSlipNo) from spTrnSOSupply aa with(nowait,nolock)
	  left join spTrnSPickingHdr bb with(nowait,nolock) on 1 = 1
	   and bb.CompanyCode   = aa.CompanyCode
	   and bb.BranchCode    = aa.BranchCode
	   and bb.PickingSlipNo = aa.PickingSlipNo
	   and bb.Status >= '2'
	 where 1 = 1
	   and aa.CompanyCode = @CompanyCode
	   and aa.BranchCode  = @BranchCode
	   and aa.PartNo = a.TaskPartNo
	   and aa.DocNo  = a.SupplySlipNo
	   and isnull(aa.PickingSlipNo, '') <> ''
	),0)
,P05 = isnull((
	select count(bb.PickingSlipNo) from spTrnSOSupply aa with(nowait,nolock)
	  left join spTrnSLmpHdr bb with(nowait,nolock) on 1 = 1
	   and bb.CompanyCode   = aa.CompanyCode
	   and bb.BranchCode    = aa.BranchCode
	   and bb.PickingSlipNo = aa.PickingSlipNo
	 where 1 = 1
	   and aa.CompanyCode = @CompanyCode
	   and aa.BranchCode  = @BranchCode
	   and aa.PartNo = a.TaskPartNo
	   and aa.DocNo  = a.SupplySlipNo
	   and isnull(aa.PickingSlipNo, '') <> ''
	),0)
,P06 = isnull((
	select count(bb.PickingSlipNo) from spTrnSOSupply aa with(nowait,nolock)
	  left join spTrnSLmpHdr bb with(nowait,nolock) on 1 = 1
	   and bb.CompanyCode   = aa.CompanyCode
	   and bb.BranchCode    = aa.BranchCode
	   and bb.PickingSlipNo = aa.PickingSlipNo
	   and bb.Status >= '1'
	 where 1 = 1
	   and aa.CompanyCode = @CompanyCode
	   and aa.BranchCode  = @BranchCode
	   and aa.PartNo = a.TaskPartNo
	   and aa.DocNo  = a.SupplySlipNo
	   and isnull(aa.PickingSlipNo, '') <> ''
	),0)
,S01 = isnull((
	select count(bb.PONo) from svTrnPoSubCon aa with(nowait,nolock)
	  left join svTrnPoSubConTask bb with(nowait,nolock) on 1 = 1
	   and bb.CompanyCode   = aa.CompanyCode
	   and bb.BranchCode    = aa.BranchCode
	   and bb.ProductType   = aa.ProductType
	   and bb.PONo          = aa.PONo
	 where 1 = 1
	   and aa.CompanyCode = @CompanyCode
	   and aa.BranchCode  = @BranchCode
	   and aa.ProductType = @ProductType
	   and aa.JobOrderNo  = @JobOrderNo
	   and aa.JobOrderNo <> ''
	   and aa.BasicModel  = a.BasicModel
	   and aa.JobType     = a.JobType
	   and bb.OperationNo = a.TaskPartNo
	),0)
,S02 = isnull((
	select count(bb.PONo) from svTrnPoSubCon aa with(nowait,nolock)
	  left join svTrnPoSubConTask bb with(nowait,nolock) on 1 = 1
	   and bb.CompanyCode   = aa.CompanyCode
	   and bb.BranchCode    = aa.BranchCode
	   and bb.ProductType   = aa.ProductType
	   and bb.PONo          = aa.PONo
	 where 1 = 1
	   and aa.CompanyCode = @CompanyCode
	   and aa.BranchCode  = @BranchCode
	   and aa.ProductType = @ProductType
	   and aa.JobOrderNo  = @JobOrderNo
	   and aa.JobOrderNo <> ''
	   and aa.BasicModel  = a.BasicModel
	   and aa.JobType     = a.JobType
	   and bb.OperationNo = a.TaskPartNo
	   and aa.POStatus >= '2'
	),0)
,S03 = isnull((
	select count(bb.PONo) from svTrnPoSubCon aa with(nowait,nolock)
	  left join svTrnPoSubConTask bb with(nowait,nolock) on 1 = 1
	   and bb.CompanyCode   = aa.CompanyCode
	   and bb.BranchCode    = aa.BranchCode
	   and bb.ProductType   = aa.ProductType
	   and bb.PONo          = aa.PONo
	 where 1 = 1
	   and aa.CompanyCode = @CompanyCode
	   and aa.BranchCode  = @BranchCode
	   and aa.ProductType = @ProductType
	   and aa.JobOrderNo  = @JobOrderNo
	   and aa.JobOrderNo <> ''
	   and aa.BasicModel  = a.BasicModel
	   and aa.JobType     = a.JobType
	   and bb.OperationNo = a.TaskPartNo
	   and aa.POStatus >= '3'
	),0)
,S04 = isnull((
	select count(bb.PONo) from svTrnPoSubCon aa with(nowait,nolock)
	  left join svTrnPoSubConTask bb with(nowait,nolock) on 1 = 1
	   and bb.CompanyCode   = aa.CompanyCode
	   and bb.BranchCode    = aa.BranchCode
	   and bb.ProductType   = aa.ProductType
	   and bb.PONo          = aa.PONo
	 where 1 = 1
	   and aa.CompanyCode = @CompanyCode
	   and aa.BranchCode  = @BranchCode
	   and aa.ProductType = @ProductType
	   and aa.JobOrderNo  = @JobOrderNo
	   and aa.JobOrderNo <> ''
	   and aa.BasicModel  = a.BasicModel
	   and aa.JobType     = a.JobType
	   and bb.OperationNo = a.TaskPartNo
	   and aa.POStatus >= '4'
	),0)
,S05 = isnull((
	select count(bb.PONo) from svTrnPoSubCon aa with(nowait,nolock)
	  left join svTrnPoSubConTask bb with(nowait,nolock) on 1 = 1
	   and bb.CompanyCode   = aa.CompanyCode
	   and bb.BranchCode    = aa.BranchCode
	   and bb.ProductType   = aa.ProductType
	   and bb.PONo          = aa.PONo
	 where 1 = 1
	   and aa.CompanyCode = @CompanyCode
	   and aa.BranchCode  = @BranchCode
	   and aa.ProductType = @ProductType
	   and aa.JobOrderNo  = @JobOrderNo
	   and aa.JobOrderNo <> ''
	   and aa.BasicModel  = a.BasicModel
	   and aa.JobType     = a.JobType
	   and bb.OperationNo = a.TaskPartNo
	   and aa.POStatus >= '5'
	),0)
from @t1 a
)#t1

update #t1
   set Status = (case P01 when 0 then 0 else 1 end)
			  + (case P02 when 0 then 0 else 1 end)
			  + (case P03 when 0 then 0 else 1 end)
			  + (case P04 when 0 then 0 else 1 end)
			  + (case P05 when 0 then 0 else 1 end)
			  + (case P06 when 0 then 0 else 1 end)
 where TypeOfGoods <> 'L'

update #t1
   set Status = (case S01 when 0 then 0 else 1 end)
			  + (case S02 when 0 then 0 else 1 end)
			  + (case S03 when 0 then 0 else 1 end)
			  + (case S04 when 0 then 0 else 1 end)
			  + (case S05 when 0 then 0 else 1 end)
 where TypeOfGoods = 'L' and IsSubCon = '1'

select
 row_number() over (order by TaskPartSeq) SeqNo
,TaskPartSeq
,BillType
,BillTypeDesc
,TypeOfGoods
,TypeOfGoodsDesc
,case isnull(TypeOfGoods, '') when 'L' then 'L' else '0' end ItemType
,TaskPartNo
,OprHourDemandQty
,SupplyQty
,ReturnQty
,OprRetailPrice
,OprRetailPriceTotal
,isnull(SupplySlipNo, '')SupplySlipNo
,TaskPartDesc
,Status
,StatusDesc = 
 case IsSubCon
	when 0 then
		 case TypeOfGoods 
			when 'L' then
				case Status
					when '0' then '0 - Open Task'
					when '1' then '1 - Work In Progress'
					when '2' then '2 - Finish Task'
				end
			else
				case Status
					when '1' then '1 - Entry Stock'
					when '2' then '2 - Alokasi Stock'
					when '3' then '3 - Generate PL'
					when '4' then '4 - Generate Bill'
					when '5' then '5 - Lampiran'
					when '6' then '6 - Print Lampiran'
				end
		 end	
	else
		case Status
			when '1' then '1 - Draft PO'
			when '2' then '2 - Generate PO'
			when '3' then '3 - Draft Receiving'
			when '4' then '4 - Cancel Receiving'
			when '5' then '5 - Receiving PO'
		end
 end
,QtyAvail
,(case when (SupplyQty > 0) then (SupplyQty - ReturnQty) else OprHourDemandQty end) * OprRetailPrice Price
,DiscPct
,OprRetailPriceTotal as PriceNet
,IsSubCon
,TaskStatus
,@ServiceNo ServiceNo
from #t1

drop table #t1

end

go
if object_id('uspfn_SvTrnServiceSaveItem') is not null
	drop procedure uspfn_SvTrnServiceSaveItem
GO
create procedure [dbo].[uspfn_SvTrnServiceSaveItem]  
--DECLARE
	@CompanyCode varchar(15),  
	@BranchCode varchar(15),  
	@ProductType varchar(15),  
	@ServiceNo bigint,  
	@BillType varchar(15),  
	@PartNo varchar(20),  
	@DemandQty numeric(18,2),  
	@PartSeq numeric(5,2),  
	@UserID varchar(15),  
	@DiscPct numeric(5,2)  
as        
  
--set @CompanyCode = '6115204001'  
--set @BranchCode = '6115204102'  
--set @ProductType = '2W'  
--set @ServiceNo = 16455  
--set @BillType = 'C'  
--set @PartNo = 'K1200-50002-000'  
--set @DemandQty = 1 
--set @PartSeq = -1  
--set @UserID = 'yo'  
--set @DiscPct = 0  

declare @errmsg varchar(max)  
declare @QueryTemp as varchar(max)  
declare @IsSPK as char(1)
  
begin try  
 -- select data svTrnService  
 select * into #srv from (  
   select a.* from svTrnService a  
  where 1 = 1  
    and a.CompanyCode = @CompanyCode  
    and a.BranchCode  = @BranchCode  
    and a.ProductType = @ProductType  
    and a.ServiceNo   = @ServiceNo  
 )#srv  
   
 declare @CostPrice as decimal  
 declare @RetailPrice as decimal  
 declare @TypeOfGoods as varchar(2)  
 declare @CostPriceMD as decimal  
 declare @RetailPriceMD as decimal  
 declare @RetailPriceInclTaxMD as decimal  
   
 declare @DealerCode as varchar(2)  
 declare @CompanyMD as varchar(15)  
 declare @BranchMD as varchar(15)  
 declare @WarehouseMD as varchar(15)  
  
 set @DealerCode = 'MD'  
 set @CompanyMD = (select CompanyMD from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)  
 set @BranchMD = (select BranchMD from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)  
 set @WarehouseMD = (select WarehouseMD from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)  
 
if object_id('#tmpSvSDMovement') is not null drop table #tmpSvSDMovement
 
 -- Check MD or SD
	-- If SD  
 if (@CompanyCode != @CompanyMD and @BranchCode != @BranchMD)   
 begin  
	  set @DealerCode = 'SD'  

	  set @IsSPK = isnull((select a.ServiceType from #srv a where a.ServiceType = '2'),0)
	  
	  declare @DbName as varchar(50)  
	  set @DbName = (select DbMD from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)  
	  
	  declare @PurchaseDisc as decimal  
	  set @PurchaseDisc = (select DiscPct from gnMstSupplierProfitCenter   
		   where CompanyCode = @CompanyCode   
		   and BranchCode = @BranchCode  
		   and SupplierCode = dbo.GetBranchMD(@CompanyCode, @BranchCode)
		   and ProfitCenterCode = '300')  
	         
	  if (@PurchaseDisc is null) raiserror ('Profit Center 300 belum tersetting pada Supplier tersebut!!!',16,1);         
	       
	  declare @tblTemp as table  
	  (  
	   CostPrice decimal(18,2),  
	   RetailPrice decimal(18,2),  
	   RetailPriceInclTax decimal(18,2),  
	   TypeOfGoods varchar (2)  
	  )
	  
	  declare @tblTemp1 as table  
	  (  
	   CostPrice decimal(18,2),  
	   RetailPrice decimal(18,2),  
	   RetailPriceInclTax decimal(18,2),  
	   TypeOfGoods varchar (2)  
	  )
	    
	  -- Untuk ItemPrice Mengambil dari masing-masing dealer	
		set @QueryTemp = 'select   
			  b.CostPrice   
			 ,b.RetailPrice  
			 ,b.RetailPriceInclTax  
			 ,a.TypeOfGoods 
			from (select
				i.PartNo   
				,i.TypeOfGoods  
				 from ' + @DbName +'..spMstItems i  
				 where i.CompanyCode = ''' + @CompanyMD + '''  
				 and i.BranchCode  = ''' + @BranchMD + '''  
				 and i.PartNo      = ''' + @PartNo + '''
			) a inner join ' + @DbName +'..spMstItemPrice b on b.PartNo = a.PartNo
		 where b.CompanyCode = ''' + @CompanyMD + '''
		 and b.BranchCode = ''' + @BranchMD + ''''
		          
	  insert into @tblTemp    
	  exec (@QueryTemp)  
	  
		set @QueryTemp = 'select   
			  b.CostPrice   
			 ,b.RetailPrice  
			 ,b.RetailPriceInclTax  
			 ,a.TypeOfGoods 
			from (select
				i.PartNo   
				,i.TypeOfGoods  
				 from ' + @DbName +'..spMstItems i  
				 where i.CompanyCode = ''' + @CompanyMD + '''  
				 and i.BranchCode  = ''' + @BranchMD + '''  
				 and i.PartNo      = ''' + @PartNo + '''
			) a inner join ' + @DbName +'..spMstItemPrice b on b.PartNo = a.PartNo
		 where b.CompanyCode = ''' + @CompanyMD + '''
		 and b.BranchCode = ''' + @BranchMD + ''''
		 
  	  insert into @tblTemp1
	  exec (@QueryTemp)  
	  print (@QueryTemp)  

	  set @CostPrice = 0
	  EXEC uspfn_GetCostPrice @CompanyCode, @BranchCode, @PartNo , @CostPrice OUTPUT

	  --set @CostPrice = ((select RetailPriceInclTax from @tblTemp1) - ((select RetailPriceInclTax from @tblTemp1) * @PurchaseDisc * 0.01))  
	  --select @CostPrice  
	  set @RetailPrice = (select RetailPrice from @tblTemp)
	  --select a.RetailPrice from spMstItemPrice a where a.CompanyCode = @CompanyCode and a.BranchCode = @BranchCode and a.PartNo = @PartNo)    
	  set @TypeOfGoods = (select TypeOfGoods from @tblTemp)  
	    
	  set @CostPriceMD = (select CostPrice from @tblTemp)  
	  set @RetailPriceMD = (select RetailPrice from @tblTemp)  
	  set @RetailPriceInclTaxMD = (select RetailPriceInclTax from @tblTemp)  
	    
	  -- select @PurchaseDisc  
 end   
 -- If MD
 else  
 begin
	 declare @tblTempPart as table  
	  (  
	   CostPrice decimal(18,2),  
	   RetailPrice decimal(18,2),  
	   RetailPriceInclTax decimal(18,2),  
	   TypeOfGoods varchar (2)  
	  )  

	  set @QueryTemp = 'select   
		a.CostPrice   
	   ,a.RetailPrice  
		 from ' + @DbName + '..spMstItemPrice a  
	   where 1 = 1  
		 and a.CompanyCode = ''' + @CompanyMD + '''  
		 and a.BranchCode  = ''' + @BranchMD + '''  
		 and a.PartNo      = ''' + @PartNo + ''''  
	          
	  insert into @tblTempPart    
	  exec (@QueryTemp)  
	   
	  --select * into #part from (  
	  --select   
	  --  a.CostPrice   
	  -- ,a.RetailPrice  
	  --  from spMstItemPrice a  
	  -- where 1 = 1  
	  --   and a.CompanyCode = @CompanyCode  
	  --   and a.BranchCode  = @BranchCode  
	  --   and a.PartNo      = @PartNo  
	  --)#part  
	    
	  --set @CostPrice = (select CostPrice from @tblTempPart)  
	  set @RetailPrice = (select RetailPrice from @tblTempPart)  
 end  
 -- EOF Check MD or SD
  
 
 if (@PartSeq > 0)  
 begin    
	-- select data mst job  
	select * into #job from (  
	select b.*  
	from svTrnService a, svMstJob b  
	where 1 = 1  
	 and b.CompanyCode = a.CompanyCode  
	 and b.ProductType = a.ProductType  
	 and b.BasicModel = a.BasicModel  
	 and b.JobType = a.JobType  
		and a.CompanyCode = @CompanyCode  
	 and a.BranchCode  = @BranchCode  
	 and a.ServiceNo   = @ServiceNo  
	 and b.GroupJobType = 'FSC'  
	)#  
	if exists (select * from #job)  
	begin  
	   -- update svTrnSrvItem  
	   set @Querytemp ='
	   update svTrnSrvItem set  
		 DemandQty      = '+ convert(varchar,@DemandQty) +'
		,CostPrice      = '+ convert(varchar,@CostPrice) +' 
		,RetailPrice    = isnull((  
			 select top 1 b.RetailPrice from #srv a, svMstTaskPart b  
			  where b.CompanyCode = a.CompanyCode  
				and b.ProductType = a.ProductType  
				and b.BasicModel = a.BasicModel  
				and b.JobType = a.JobType  
				and b.PartNo = '''+ @PartNo +''' 
				and b.BillType = ''F'' 
			 ), (  
			  select top 1 isnull(RetailPrice, 0) RetailPrice  
				from spMstItemPrice  
			   where 1 = 1  
				 and CompanyCode = '''+ @CompanyCode +'''
				 and BranchCode = '''+ @BranchCode +'''
				 and PartNo = '''+ @PartNo  +'''
			  )  
			 )  
		,LastupdateBy   = (select LastupdateBy from #srv)  
		,LastupdateDate = (select LastupdateDate from #srv)  
		,BillType       = '''+ @BillType +'''
		,DiscPct        = '+ convert(varchar,@DiscPct) +'  
		where 1 = 1  
		  and CompanyCode  = '''+ @CompanyCode +''' 
		  and BranchCode   = '''+ @BranchCode +''' 
		  and ProductType  = (select ProductType from #srv)  
		  and ServiceNo    = (select ServiceNo from #srv)  
		  and PartNo       = '''+ @PartNo +''' 
		  and PartSeq      = '+ convert(varchar,@PartSeq) +'' 
		  
		  exec(@QueryTemp) 
	  end  
	  else  
	  begin  
	   -- update svTrnSrvItem  
	   update svTrnSrvItem set  
		 DemandQty      = @DemandQty  
		,CostPrice      = @CostPrice  
		,RetailPrice    = @RetailPrice  
		,LastupdateBy   = (select LastupdateBy from #srv)  
		,LastupdateDate = (select LastupdateDate from #srv)  
		,BillType       = @BillType  
		,DiscPct        = @DiscPct  
		where 1 = 1  
		  and CompanyCode  = @CompanyCode  
		  and BranchCode   = @BranchCode  
		  and ProductType  = (select ProductType from #srv)  
		  and ServiceNo    = (select ServiceNo from #srv)  
		  and PartNo       = @PartNo  
		  and PartSeq      = @PartSeq           
	  end   
	    
	--update svSDMovement  
	if (@DealerCode = 'SD' and @IsSPK = '2')  
	begin    
		set @QueryTemp = 'update ' + @DbName + '..svSDMovement set    
		QtyOrder    = ' + case when @DemandQty is null then '0' else convert(varchar, @DemandQty) end + ' 
		,DiscPct    = ' + case when  @DiscPct is null then '0' else convert(varchar, @DiscPct) end + '
		,CostPrice    = ' + case when @CostPrice is null then '0' else convert(varchar, @CostPrice) end + '  
		,RetailPrice   = ' + case when @RetailPrice is null then '0' else convert(varchar, @RetailPrice) end + '  
		,CostPriceMD   = ' + case when @CostPriceMD is null then '0' else convert(varchar, @CostPriceMD) end + '  
		,RetailPriceMD   = ' + case when @RetailPriceMD is null then '0' else convert(varchar, @RetailPriceMD) end + '  
		,RetailPriceInclTaxMD = ' + case when @RetailPriceInclTaxMD is null then '0' else convert(varchar, @RetailPriceInclTaxMD) end + '  
		,[Status]  = ''' + case when (select ServiceStatus from #srv) is null then '''' else (select ServiceStatus from #srv) end + '''  
		,LastupdateBy   = ''' + case when (select LastupdateBy from #srv) is null then '''' else (select LastupdateBy from #srv) end + '''  
		,LastupdateDate = ''' + case when (select LastupdateDate from #srv) is null then '''' else convert(varchar,(select LastupdateDate from #srv)) end + '''  
		where CompanyCode = ''' + case when @CompanyCode is null then '''' else @CompanyCode end + '''
		  and BranchCode = ''' + case when @BranchCode is null then '''' else @BranchCode end + '''
		  and DocNo  = ''' + case when (select JobOrderNo from #srv) is null then '''' else (select JobOrderNo from #srv) end + '''  
		  and PartNo       =  ''' + case when @PartNo is null then '''' else @PartNo end  + '''
		  and PartSeq      = ' + case when @PartSeq is null then '0' else convert(varchar, @PartSeq) end + '';  
		  
		  --print @QueryTemp;  
		exec 	(@QueryTemp);
	end
 end  
 else  
 begin  
	if((select count(*) from svTrnSrvItem  
	where 1 = 1  
	  and CompanyCode  = @CompanyCode  
	  and BranchCode   = @BranchCode  
	  and ProductType  = (select ProductType from #srv)  
	  and ServiceNo    = (select ServiceNo from #srv)  
	  and PartNo       = @PartNo  
	  and (isnull(SupplySlipNo,'') = '')  
	) > 0)  
	begin  
		raiserror ('Part yang sama sudah diproses di Entry SPK namun belum dapat No SSS, mohon diselesaikan dahulu!',16,1);  
	end  

	declare @PartSeqNew as int  
	set @PartSeqNew = (isnull((select isnull(max(PartSeq), 0) + 1    
	  from svTrnSrvItem   
		where CompanyCode = @CompanyCode  
	   and BranchCode  = @BranchCode   
	   and ProductType = @ProductType  
	   and ServiceNo   = @ServiceNo  
	   and PartNo      = @PartNo), 1))  
	     
	-- insert svTrnSrvItem  
	set @QueryTemp=' insert into svTrnSrvItem (CompanyCode, BranchCode, ProductType, ServiceNo, PartNo, PartSeq, DemandQty, SupplyQty, ReturnQty, CostPrice, RetailPrice, TypeOfGoods, BillType, SupplySlipNo, SupplySlipDate, SSReturnNo, SSReturnDate, CreatedBy, CreatedDate, LastupdateBy, LastupdateDate, DiscPct, MechanicID)  
	select   
	'''+ @CompanyCode +''' CompanyCode  
	,''' + @BranchCode +''' BranchCode  
	,'''+ @ProductType +''' ProductType  
	,'+ convert(varchar,@ServiceNo) +' ServiceNo  
	,a.PartNo  
	,'''+ convert(varchar,@PartSeqNew)  +'''
	--,(row_number() over (order by a.PartNo)) PartSeq  
	,'+ convert(varchar,@DemandQty )+' DemandQty  
	,''0'' SupplyQty  
	,''0'' ReturnQty  
	,'+ convert(varchar,isnull(@CostPrice,0))  +'
	,a.RetailPrice   
	,b.TypeOfGoods  
	,'''+ @BillType +''' BillType  
	,null SupplySlipNo  
	,null SupplySlipDate  
	,null SSReturnNo  
	,null SSReturnDate  
	,c.LastupdateBy CreatedBy  
	,c.LastupdateDate CreatedDate  
	,c.LastupdateBy  
	,c.LastupdateDate  
	,'+ convert(varchar,isnull(@DiscPct,0))  +'
	,(select MechanicID from svTrnService where CompanyCode = '''+ @CompanyCode +''' and BranchCode = '''+ @BranchCode +''' and ServiceNo = '+ convert(varchar,@ServiceNo) +')  
    from spMstItemPrice a, '+ @DbName +'..spMstItems b, 
    #srv c, gnmstcompanymapping d 
   where 1 = 1  
	 and d.CompanyMd = b.CompanyCode
	 and d.BranchMD = b.BranchCode
        and d.CompanyCode = c.CompanyCode  
     and d.BranchCode  = c.BranchCode  
     and b.PartNo      = a.PartNo  
        and (b.CompanyCode = '''+ @CompanyMD +'''
     and b.BranchCode  = '''+ @BranchMD +'''
     and b.PartNo      = '''+ @PartNo +''')
     and (a.CompanyCode = '''+ @CompanyCode +'''
     and a.BranchCode  = '''+ @BranchCode +'''
     and a.PartNo      = '''+ @PartNo +''')' 
		   
	exec(@QueryTemp)

	--print(@QueryTemp)

	--select   @CostPrice   
	--xxx

	if (@DealerCode = 'SD' and @IsSPK = '2')  
	begin
		create table #tmpSvSDMovement(
			CompanyCode varchar(15)
			,BranchCode varchar(15)
			,JobOrderNo varchar(20)   
			,JobOrderDate datetime  
			,PartNo varchar(20)
			,PartSeqNew int
			,WarehouseMD varchar(20)   
			,DemandQty numeric(18,2)
			,Qty numeric(18,2)
			,DiscPct numeric(18,2)
			,CostPrice numeric(18,2)
			,RetailPrice numeric(18,2) 
			,TypeOfGoods varchar(15) 
			,CompanyMD varchar(15)
			,BranchMD varchar(15)   
			,WarehouseMD2 varchar(15)
			,RetailPriceInclTaxMD numeric(18,2) 
			,RetailPriceMD numeric(18,2) 
			,CostPriceMD numeric(18,2)  
			,QtyFlag char(1)
			,ProductType varchar(15) 
			,ProfitCenterCode varchar(15)
			,Status char(1)
			,ProcessStatus char(1)
			,ProcessDate datetime 
			,CreatedBy varchar(15) 
			,CreatedDate datetime 
			,LastUpdateBy varchar(15) 
			,LastUpdateDate datetime	
		);

		insert into #tmpSvSDMovement 
			select case when @CompanyCode is null then '' else @CompanyCode end 
			,case when @BranchCode is null then '' else @BranchCode end
			,case when (select JobOrderNo from #srv) is null then convert(varchar,@ServiceNo) else (select JobOrderNo from #srv) end
			,case when (select JobOrderDate from #srv) is null then '1900/01/01' else (select JobOrderDate from #srv) end 
			,case when @PartNo is null then '' else  @PartNo end 
			,case when @PartSeqNew is null then '0' else convert(varchar, @PartSeqNew) end
			,case when @WarehouseMD is null then '' else @WarehouseMD end  
			,case when @DemandQty  is null then '0' else convert(varchar, @DemandQty) end
 			,case when @DemandQty  is null then '0' else convert(varchar, @DemandQty) end
			,case when @DiscPct is null then '0' else convert(varchar, @DiscPct) end  
			,case when @CostPrice is null then '0' else convert(varchar, @CostPrice) end 
			,case when @RetailPrice is null then '0' else convert(varchar, @RetailPrice) end  
			,case when @TypeOfGoods is null then '' else @TypeOfGoods end 
			,case when @CompanyMD is null then '' else @CompanyMD end   
			,case when @BranchMD is null then '' else @BranchMD end  
			,case when @WarehouseMD is null then '' else @WarehouseMD end  
			,case when @RetailPriceInclTaxMD is null then '0' else convert(varchar, @RetailPriceInclTaxMD) end  
			,case when @RetailPriceMD is null then '0' else convert(varchar, @RetailPriceMD) end   
			,case when @CostPriceMD is null then '0' else convert(varchar, @CostPriceMD) end
			,'x'
			,case when @ProductType is null then '' else @ProductType end  
			,'300'  
			,'0' 
			,'0'
			, GETDATE() 
			,case when (select CreatedBy from #srv) is null then '' else (select CreatedBy from #srv) end     
			,case when (select CreatedDate from #srv) is null then '1900/01/01' else convert(varchar,(select CreatedDate from #srv)) end 
			,case when (select LastUpdateBy from #srv) is null then '' else (select LastUpdateBy from #srv) end
			,case when (select LastUpdateDate from #srv) is null then '1900/01/01' else convert(varchar,(select LastUpdateDate from #srv)) end
		 
		declare @intCountTemp int
		set @intCountTemp = (select count(isnull(JobOrderNo,'')) DocNo from #tmpSvSDMovement)
		if (@intCountTemp > 0 ) begin 
			declare @intStringEmpty int
			set @intStringEmpty = (select count(isnull(JobOrderNo,'')) DocNo from #tmpSvSDMovement where JobOrderNo = '' or JobOrderNo is null)
			select @intCountTemp
			select @intStringEmpty
			if (@intStringEmpty < 1) begin
				set @QueryTemp = '
					insert into ' + @DbName + '..svSDMovement (CompanyCode, BranchCode, DocNo, DocDate, PartNo, PartSeq, WarehouseCode, QtyOrder, Qty, DiscPct, CostPrice, RetailPrice,   
					TypeOfGoods, CompanyMD, BranchMD, WarehouseMD, RetailPriceInclTaxMD, RetailPriceMD, CostPriceMD, QtyFlag, ProductType, ProfitCenterCode, 
					Status, ProcessStatus, ProcessDate, CreatedBy,   
					CreatedDate, LastUpdateBy, LastUpdateDate)  
					select * from #tmpSvSDMovement';
				exec(@QueryTemp);
			end
		end
		 
		drop table #tmpSvSDMovement;     
	end   
 end  

 update svTrnSrvItem  
    set DiscPct = @DiscPct  
  where 1 = 1  
    and CompanyCode = @CompanyCode  
    and BranchCode = @BranchCode  
    and ProductType = @ProductType  
    and ServiceNo = @ServiceNo  
    and PartNo = @PartNo  
   
 if (@DealerCode = 'SD' and @IsSPK = '2')  
 begin    
	set @QueryTemp = 'update ' + @DbName + '..svSDMovement   
	  set DiscPct = ' + convert(varchar,@DiscPct) 
	  + ' where 1 = 1'  
	  +	' and CompanyCode = ''' + case when @CompanyMD is null then '''' else  @CompanyMD end + ''''
	  + ' and BranchCode = ''' + case when @BranchMD is null then '''' else  @BranchMD end + ''''
	  + ' and DocNo = ''' + case when (select JobOrderNo from #srv) is null then '''' else (select JobOrderNo from #srv) end  + ''''
	  + ' and PartNo = ''' + case when @PartNo is null then '''' else @PartNo end + ''''  
	  + ' and PartSeq = ' + convert(varchar,@PartSeq) + '';
  
	exec (@QueryTemp)  
 end  
   
	drop table #srv  
end try  
begin catch  
 set @errmsg = error_message()  
 raiserror (@errmsg,16,1);  
end catch  

--rollback tran

GO

if (SELECT count(*) from sysReport WHERE reportID='OmRpSalesTrn007BWeb') = 0
	begin
	INSERT INTO SysReport (ReportID, ReportPath, ReportSource, ReportProc, ReportDeviceID, ReportInfo)
	Values('OmRpSalesTrn007BWeb','Isi.Dms.Report.Sales,ReportSource.Sales.OmRpSalesTrn007BRpt','SP','usprpt_OmRpSalesTrn007Web','TEST_LETTER','PERMOHONAN FAKTUR POLISI')
	end
if (SELECT count(*) from sysReport WHERE reportID='OmRpSalesTrn007CWeb') = 0
	begin
	INSERT INTO SysReport (ReportID, ReportPath, ReportSource, ReportProc, ReportDeviceID, ReportInfo)
	Values('OmRpSalesTrn007CWeb','Isi.Dms.Report.Sales,ReportSource.Sales.OmRpSalesTrn007CRpt','SP','usprpt_OmRpSalesTrn007BWeb','TEST_LETTER','PERMOHONAN FAKTUR POLISI') 
	end
if (SELECT count(*) from sysReport WHERE reportID='OmRpSalesTrn007DNewWeb') = 0
	begin
	INSERT INTO SysReport (ReportID, ReportPath, ReportSource, ReportProc, ReportDeviceID, ReportInfo)
	Values('OmRpSalesTrn007DNewWeb','Isi.Dms.Report.Sales,ReportSource.Sales.OmRpSalesTrn007DNewRpt','SP','usprpt_OmRpSalesTrn007Web','LetterP2','PERMOHONAN FAKTUR POLISI') 
	end
go

ALTER procedure [dbo].[sp_updateOmMstVehicleSO]
	@companyCode varchar(25),
	@BranchCode varchar(25),
	@ChassisCode varchar(25),
	@ChassisNo varchar(25),
	@SONO varchar(25),
	@userId varchar(25)
as

begin
	declare @CountSOVin int;
	declare @CountSOModel int;
	declare @dbMD varchar(25), @sqlStr varchar(max);
	
	--set @Month = (select FiscalPeriod from gnMstCoProfileSales where companycode=@companyCode and BranchCode=@BranchCode)
	set @dbMD =(select dbMD from gnMstCompanyMapping where CompanyCode=@companyCode and BranchCode=@BranchCode)
	
	if @dbMD IS NULL
	begin
		if exists (select * from omMstVehicle where ChassisCode = @ChassisCode and ChassisNo = @ChassisNo)
		begin
			update omMstVehicle
			set status=3, SONo = @SONO, LastUpdateBy = @userId, LastUpdateDate =getdate()
			where ChassisCode = @ChassisCode and ChassisNo= @ChassisNo

			select convert(bit, 1) as Status
		end
		else select convert(bit, 0) as Status
		
	end
	else
		set @sqlStr = '
			if exists (select * from '+ @dbMD +'.dbo.omMstVehicle where ChassisCode = '''+@ChassisCode+''' and ChassisNo = '''+@ChassisNo+''')
			begin
				update '+ @dbMD +'.dbo.omMstVehicle
				set status=3, SONo = '''+@SONO+''', LastUpdateBy = '''+@userId+''', LastUpdateDate =getdate()
				where ChassisCode = '''+@ChassisCode+''' and ChassisNo= '''+@ChassisNo+'''

				select convert(bit, 1) as Status
			end
			else select convert(bit, 0) as Status
		'
	
	--if exists (select * from BAT_UAT.dbo.omMstVehicle where ChassisCode = @ChassisCode and ChassisNo =@ChassisNo)
	--begin
		--update BAT_UAT.dbo.omMstVehicle
		--set status=3, SONo = @SONO, LastUpdateBy = @userId, LastUpdateDate =getdate()
		--where ChassisCode = @ChassisCode and ChassisNo=@ChassisNo

		--select convert(bit, 1) as Status
	--end
	--else select convert(bit, 0) as Status
	--select	@sqlStr
	exec(@sqlStr)
end



go

-- uspfn_OmInquiryChassisDO '6115202','611520200','SOA/11/000287','FU150 SCD',2011,'MH8BG41CABJ','COLO','00'
ALTER procedure [dbo].[uspfn_OmInquiryChassisDO]
	@CompanyCode varchar(15)
	,@BranchCode varchar(15)
	,@SONo varchar(15)
	,@SalesModelCode varchar(30)
	--,@SalesModelYear int
	,@SalesModelYear varchar(15)
	,@ChassisCode varchar(15)
	,@RefType varchar(15)
	,@WarehouseCode varchar(15)

as

declare 
@val as int,
@CompanyMD as varchar(15)
,@DBMD as varchar(15)
,@QryTemp as varchar(max)


DECLARE @columnVal TABLE (columnVal int);

set @val=0

set @CompanyMD = (SELECT CompanyMD FROM gnMstCompanyMapping WHERE CompanyCode=@CompanyCode AND BranchCode=@BranchCode) 
set @DBMD = (SELECT DbMD FROM gnMstCompanyMapping WHERE CompanyCode=@CompanyCode AND BranchCode=@BranchCode) 

--set @val= (
if @DBMD IS NULL
begin
	set @QryTemp = 'SELECT isnull(count (a.ChassisNo),0) jml
	FROM omTrSalesSOVin a 
		INNER JOIN omMstVehicle b ON 
			a.ChassisCode = b.ChassisCode 
			AND a.ChassisNo = b.ChassisNo 
	WHERE a.CompanyCode = ''' + @CompanyCode + '''
		AND a.BranchCode = ''' + @BranchCode + ''' 
		AND a.SONo = ''' + @SONo + '''
		AND a.SalesModelCode = ''' + @SalesModelCode + ''' 
		AND a.SalesModelYear = ''' + @SalesModelYear + '''
		AND a.ChassisCode = ''' + @ChassisCode + '''
		and b.Status in (0,3)
		AND not exists 
		( 
			SELECT 1 
			FROM omTrSalesDODetail x
				inner join omTrSalesDO y on x.CompanyCode=y.CompanyCode and x.BranchCode=y.BranchCode
					and x.DONo=y.DONo
			WHERE x.CompanyCode = a.CompanyCode 
			AND x.BranchCode = a.BranchCode 
			AND x.SalesModelCode = a.SalesModelCode 
			AND x.ChassisCode = a.ChassisCode 
			and x.ChassisNo= a.ChassisNo
			and y.Status in (0,1)
		) 
		AND a.ChassisNo != 0 '
end
else
begin
	set @QryTemp = 'SELECT isnull(count (a.ChassisNo),0) jml
	FROM omTrSalesSOVin a 
		INNER JOIN ' + @DBMD + '.dbo.omMstVehicle b ON 
			a.ChassisCode = b.ChassisCode 
			AND a.ChassisNo = b.ChassisNo 
	WHERE a.CompanyCode = ''' + @CompanyCode + '''
		AND a.BranchCode = ''' + @BranchCode + ''' 
		AND a.SONo = ''' + @SONo + '''
		AND a.SalesModelCode = ''' + @SalesModelCode + ''' 
		AND a.SalesModelYear = ''' + @SalesModelYear + '''
		AND a.ChassisCode = ''' + @ChassisCode + '''
		and b.Status in (0,3)
		AND not exists 
		( 
			SELECT 1 
			FROM omTrSalesDODetail x
				inner join omTrSalesDO y on x.CompanyCode=y.CompanyCode and x.BranchCode=y.BranchCode
					and x.DONo=y.DONo
			WHERE x.CompanyCode = a.CompanyCode 
			AND x.BranchCode = a.BranchCode 
			AND x.SalesModelCode = a.SalesModelCode 
			AND x.ChassisCode = a.ChassisCode 
			and x.ChassisNo= a.ChassisNo
			and y.Status in (0,1)
		) 
		AND a.ChassisNo != 0 ' 
end

INSERT INTO @columnVal EXEC (@QryTemp);

set @val= (SELECT * FROM @columnval);

if @val = 0 
begin
	if @DBMD IS NULL
	begin
		set @QryTemp =
		'SELECT 
			a.ChassisCode , a.ChassisNo , a.EngineCode , a.EngineNo , 
			a.ColourCode , b.RefferenceDesc1 AS ColourName 
		FROM omMstVehicle a 
			LEFT JOIN omMstRefference b ON b.CompanyCode = a.CompanyCode 
				AND b.RefferenceType = ''' + @RefType + ''' 
				AND b.RefferenceCode = a.ColourCode 
		WHERE a.CompanyCode = ''' + @CompanyMD + '''  
			AND a.SalesModelCode = ''' + @SalesModelCode + ''' 
			AND a.ChassisCode = ''' + @ChassisCode + ''' 
			AND a.Status = 0 
			AND isnull (a.SONo,'''') = ''''  
			AND a.WarehouseCode = ''' + @WarehouseCode + '''
			AND exists 
			( 
				SELECT z.colourCode FROM OmTrSalesSOModelColour z 
				WHERE z.companyCode = a.CompanyCode 
				AND z.BranchCode = ''' + @BranchCode + '''
				AND z.SONo= ''' + @SONo + ''' 
				AND z.SalesModelCode = a.SalesModelCode 
				AND z.SalesModelYear = a.SalesModelYear 
				and z.ColourCode= a.ColourCode
			)'
	end
	else
	begin
		set @QryTemp =
		'SELECT 
			a.ChassisCode , a.ChassisNo , a.EngineCode , a.EngineNo , 
			a.ColourCode , b.RefferenceDesc1 AS ColourName 
		FROM ' + @DBMD + '.dbo.omMstVehicle a 
			LEFT JOIN ' + @DBMD + '.dbo.omMstRefference b ON b.CompanyCode = a.CompanyCode 
				AND b.RefferenceType = ''' + @RefType + ''' 
				AND b.RefferenceCode = a.ColourCode 
		WHERE a.CompanyCode = ''' + @CompanyMD + '''  
			AND a.SalesModelCode = ''' + @SalesModelCode + ''' 
			AND a.ChassisCode = ''' + @ChassisCode + ''' 
			AND a.Status = 0 
			AND isnull (a.SONo,'''') = ''''  
			AND a.WarehouseCode = ''' + @WarehouseCode + '''
			AND exists 
			( 
				SELECT z.colourCode FROM OmTrSalesSOModelColour z 
				WHERE z.companyCode = a.CompanyCode 
				AND z.BranchCode = ''' + @BranchCode + '''
				AND z.SONo= ''' + @SONo + ''' 
				AND z.SalesModelCode = a.SalesModelCode 
				AND z.SalesModelYear = a.SalesModelYear 
				and z.ColourCode= a.ColourCode
			)'
	end
	Exec (@QryTemp);
end
else
begin
	if @DBMD IS NULL
	begin
		set @QryTemp =
		'SELECT 
			a.ChassisNo , a.EngineCode , a.EngineNo , 
			a.ColourCode , b.RefferenceDesc1 AS ColourName 
		FROM omTrSalesSOVin a 
			LEFT JOIN omMstRefference b ON b.CompanyCode = a.CompanyCode 
				AND b.RefferenceType = ''' + @RefType + ''' 
				AND b.RefferenceCode = a.ColourCode 
			INNER JOIN omMstVehicle c ON 
				a.ChassisCode = c.ChassisCode 
				AND a.ChassisNo = c.ChassisNo 
		WHERE a.CompanyCode = ''' + @CompanyCode + ''' 
			AND a.BranchCode = ''' + @BranchCode + '''  
			AND a.SONo = ''' + @SONo + ''' 
			AND a.SalesModelCode = ''' + @SalesModelCode + '''
			AND a.SalesModelYear = ''' + @SalesModelYear + ''' 
			AND a.ChassisCode = ''' + @ChassisCode + ''' 
			AND c.WarehouseCode = ''' + @WarehouseCode + '''
			and c.Status in (0,3)
			AND not exists
			( 
				SELECT 1 
				FROM omTrSalesDODetail x
					inner join omTrSalesDO y on x.CompanyCode=y.CompanyCode and x.BranchCode=y.BranchCode
						and x.DONo=y.DONo
				WHERE x.CompanyCode = a.CompanyCode 
				AND x.BranchCode = a.BranchCode 
				AND x.SalesModelCode = a.SalesModelCode 
				AND x.ChassisCode = a.ChassisCode 
				and x.ChassisNo= a.ChassisNo
				and y.Status in (0,1)
			)'
	end
	else
	begin
		set @QryTemp =
		'SELECT 
			a.ChassisNo , a.EngineCode , a.EngineNo , 
			a.ColourCode , b.RefferenceDesc1 AS ColourName 
		FROM omTrSalesSOVin a 
			LEFT JOIN omMstRefference b ON b.CompanyCode = a.CompanyCode 
				AND b.RefferenceType = ''' + @RefType + ''' 
				AND b.RefferenceCode = a.ColourCode 
			INNER JOIN ' + @DBMD + '.dbo.omMstVehicle c ON 
				a.ChassisCode = c.ChassisCode 
				AND a.ChassisNo = c.ChassisNo 
		WHERE a.CompanyCode = ''' + @CompanyCode + ''' 
			AND a.BranchCode = ''' + @BranchCode + '''  
			AND a.SONo = ''' + @SONo + ''' 
			AND a.SalesModelCode = ''' + @SalesModelCode + '''
			AND a.SalesModelYear = ''' + @SalesModelYear + ''' 
			AND a.ChassisCode = ''' + @ChassisCode + ''' 
			AND c.WarehouseCode = ''' + @WarehouseCode + '''
			and c.Status in (0,3)
			AND not exists
			( 
				SELECT 1 
				FROM omTrSalesDODetail x
					inner join omTrSalesDO y on x.CompanyCode=y.CompanyCode and x.BranchCode=y.BranchCode
						and x.DONo=y.DONo
				WHERE x.CompanyCode = a.CompanyCode 
				AND x.BranchCode = a.BranchCode 
				AND x.SalesModelCode = a.SalesModelCode 
				AND x.ChassisCode = a.ChassisCode 
				and x.ChassisNo= a.ChassisNo
				and y.Status in (0,1)
			)'
	end
	Exec (@QryTemp);
end


go
-- uspfn_OmInquiryChassisReq '6007402','600740200'
ALTER procedure [dbo].[uspfn_OmInquiryChassisReq]
	@CompanyCode as varchar(15)
	,@BranchCode as varchar(15)
	,@Penjual as varchar(15)
	,@CBU as bit
as

declare @isDirect as bit
set @isDirect=0
if exists (
	select 1
	from gnMstCoProfile
	where CompanyCode=@CompanyCode and BranchCode=@Penjual
)
set @isDirect=1

--DECLARE @CompanyCode as varchar(15)
--	,@Penjual as varchar(15)
--
--set @CompanyCode='6117201'
--set @Penjual='6117201'

select * into #t1
from (
	select distinct isnull(b.CompanyCode, e.CompanyCode) CompanyCode, isnull(b.BranchCode, e.BranchCode) BranchCode, isnull(c.CustomerCode, e.CustomerCode) CustomerCode
			,z.ChassisCode,z.BPKNo,z.SONo,e.DONo,convert(varchar,z.chassisNo) chassisNo, z.salesModelCode
			, z.salesModelYear, isnull(z.SuzukiDONo,'') RefferenceDONo,
			isnull(z.SuzukiDODate,'19000101') RefferenceDODate, isnull(z.SuzukiSJNo,'') RefferenceSJNo, 
			isnull(z.SuzukiSJDate,'19000101') RefferenceSJDate, 
			a.EndUserName, a.EndUserAddress1, a.EndUserAddress2, a.EndUserAddress3,
			c.CustomerName, c.Address1, c.Address2, c.Address3,
			c.CityCode,(SELECT LookUpValueName FROM gnMstLookUpDtl where CodeID = 'CITY' AND ParaValue = c.CityCode) as CityName, 
			c.PhoneNo, c.HPNo, c.birthDate ,b.Salesman, (SELECT EmployeeName FROM HrEmployee where EmployeeID = b.Salesman) SalesmanName, b.SalesType
	from omMstVehicle z 
		left join omTrSalesSOVin a 
			on a.CompanyCode = z.CompanyCode and z.SONo=a.SONo
				AND a.ChassisCode = z.ChassisCode
				AND a.ChassisNo = z.ChassisNo
		left join omTrSalesSO b
			on a.companyCode = b.companyCode 
				and a.BranchCode= b.BranchCode
				and a.SONo = b.SONo
				and b.Status = '2' 
		left join gnMstCustomer c 
			on b.CompanyCode = c.CompanyCode
				and b.CustomerCode = c.CustomerCode 
		left join OmTrSalesDODetail d 
			on d.CompanyCode = z.CompanyCode and z.DONo=d.DONo
				and d.ChassisCode = z.ChassisCode		
				and d.ChassisNo = z.ChassisNo	
		left join OmTrSalesDO e 
			on e.CompanyCode = d.CompanyCode
				and e.DONo = d.DONo
				and e.BranchCode=d.BranchCode
				and e.Status = '2'
		inner join omMstModel f
			on f.CompanyCode = z.CompanyCode
				and f.SalesModelCode = z.SalesModelCode
	where 
		z.CompanyCode = @CompanyCode
		and z.ReqOutNo = ''
		and z.status in ('3','4','5','6')
		and not exists (select 1 from omTrSalesReqdetail where ChassisCode=z.ChassisCode and ChassisNo=z.ChassisNo)
		and ((case when z.ChassisNo is not null then z.SONo end) is not null 
			or (case when z.ChassisNo is not null then z.DONo end) is not null)
		and f.IsCBU = @CBU
) #t1

select * from #t1 
where ((case when @isDirect = 1 then BranchCode end)= @Penjual or (case when @isDirect <> 1 then CompanyCode end) = @Penjual
		or (case when @isDirect <> 1 then CustomerCode end)= @Penjual)
drop table #t1
go

ALTER procedure [dbo].[uspfn_omSlsBPKBrowse]   
(  
 @CompanyCode varchar(15),  
 @BranchCode varchar(15) 
)  
AS

DECLARE 
	@QRYTemp as varchar(max),
	@CompanyMD as varchar(15),
	@DBMD as varchar(15)
 
set @CompanyMD = (SELECT CompanyMD FROM gnMstCompanyMapping WHERE CompanyCode=@CompanyCode AND BranchCode=@BranchCode) 
set @DBMD = (SELECT DbMD FROM gnMstCompanyMapping WHERE CompanyCode=@CompanyCode AND BranchCode=@BranchCode) 
 
BEGIN
if @DBMD IS NULL
begin
	SELECT Distinct a.BPKNo, a.BPKDate, a.DONo, a.SONo, d.SKPKNo, d.RefferenceNo, a.CustomerCode  , c.CustomerName , a.ShipTo , e.CustomerName as ShipToDsc,
    c.Address1 + ' ' + c.Address2 + ' ' + c.Address3 + ' ' + c.Address4 as Address, a.WareHouseCode, f.LookUpValueName as WrhDsc, a.Expedition,g.SupplierName as ExpeditionDsc,a.Status,
    CASE a.Status WHEN '0' THEN 'Open' WHEN '1' THEN 'Printed' WHEN '2' THEN 'Approved' WHEN '3' THEN 'Canceled' WHEN '9' THEN 'Finished' END as StatusDsc       
    ,b.SalesType, CASE ISNULL(b.SalesType, 0) WHEN 0 THEN 'Wholesales' ELSE 'Direct' END AS TypeSales, a.Remark
    FROM omTrSalesBPK a
    LEFT JOIN gnMstCustomerProfitCenter b ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.CustomerCode = b.CustomerCode AND b.ProfitCenterCode = '100'
    LEFT JOIN gnMstCustomer c ON a.CompanyCode = c.CompanyCode AND a.CustomerCode = c.CustomerCode
    LEFT JOIN omTrSalesSO d ON a.CompanyCode = d.CompanyCode AND a.BranchCode = d.BranchCode AND a.SONo = d.SONo
    LEFT JOIN gnMstCustomer e ON a.CompanyCode = e.CompanyCode AND a.shipto = e.CustomerCode
    Left join gnMstLookUpDtl f on f.CompanyCode = @CompanyCode and a.WarehouseCode=f.LookUpValue and f.CodeID='MPWH'
    LEFT JOIN gnMstSupplier g ON a.CompanyCode = g.CompanyCode AND a.Expedition = g.SupplierCode
    WHERE a.CompanyCode = @CompanyCode
        AND a.BranchCode = @BranchCode                             
    ORDER BY a.BPKNo DESC
end
else
--exec uspfn_omSlsBPKBrowse 6006410,600641001
	SET @QRYTemp = 
			'SELECT Distinct a.BPKNo, a.BPKDate, a.DONo, a.SONo, d.SKPKNo, d.RefferenceNo, a.CustomerCode  , c.CustomerName , a.ShipTo , e.CustomerName as ShipToDsc,
            c.Address1 + '' '' + c.Address2 + '' '' + c.Address3 + '' '' + c.Address4 as Address, a.WareHouseCode, f.LookUpValueName as WrhDsc, a.Expedition,g.SupplierName as ExpeditionDsc,a.Status,
            CASE a.Status WHEN ''0'' THEN ''Open'' WHEN ''1'' THEN ''Printed'' WHEN ''2'' THEN ''Approved'' WHEN ''3'' THEN ''Canceled'' WHEN ''9'' THEN ''Finished'' END as StatusDsc       
            ,b.SalesType, CASE ISNULL(b.SalesType, 0) WHEN 0 THEN ''Wholesales'' ELSE ''Direct'' END AS TypeSales, a.Remark
            FROM omTrSalesBPK a
            LEFT JOIN gnMstCustomerProfitCenter b ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.CustomerCode = b.CustomerCode AND b.ProfitCenterCode = ''100''
            LEFT JOIN gnMstCustomer c ON a.CompanyCode = c.CompanyCode AND a.CustomerCode = c.CustomerCode
            LEFT JOIN omTrSalesSO d ON a.CompanyCode = d.CompanyCode AND a.BranchCode = d.BranchCode AND a.SONo = d.SONo
            LEFT JOIN gnMstCustomer e ON a.CompanyCode = e.CompanyCode AND a.shipto = e.CustomerCode
            Left join ' + @DBMD + '.dbo.gnMstLookUpDtl f on f.CompanyCode = ''' + @CompanyMD + ''' and a.WarehouseCode=f.LookUpValue and f.CodeID=''MPWH''
            LEFT JOIN gnMstSupplier g ON a.CompanyCode = g.CompanyCode AND a.Expedition = g.SupplierCode
            WHERE a.CompanyCode = ''' + @CompanyCode + '''
               AND a.BranchCode = ''' + @BranchCode + '''                             
            ORDER BY a.BPKNo DESC'

	Exec (@QRYTemp);
End



go
ALTER procedure [dbo].[uspfn_omSlsBPKLkpChassisNo]   
(  
 @CompanyCode varchar(15),  
 @BranchCode varchar(15),  
 @DONo varchar(15),
 @BPKNo varchar(15),
 @ChassisCode   varchar(15) 
)  
AS  
begin
select a.ChassisNo, a.EngineCode, a.EngineNo, 
a.ColourCode, b.RefferenceDesc1 from 
omTrSalesDODetail a
left join omMstRefference b on 
b.CompanyCode = a.CompanyCode and
b.RefferenceCode = a.ColourCode and
b.RefferenceType = 'COLO'
where
a.CompanyCode = @CompanyCode
AND a.BranchCode = @BranchCode
AND a.ChassisCode = @ChassisCode
AND a.DONo = @DONo
AND a.ChassisNo not in (select isnull(ChassisNo,0) from omTrSalesBPKDetail z
where z.CompanyCode = a.CompanyCode
and z.BranchCode = a.BranchCode
and z.BPKNo = @BPKNo
and z.ChassisCode = a.ChassisCode
AND not exists (select 1 from omTrSalesReturnVIN where CompanyCode=a.CompanyCode and BranchCode=a.BranchCode
		and ChassisCode=a.ChassisCode and ChassisNo=a.ChassisNo))
END		

go
ALTER procedure [dbo].[uspfn_omSlsBPKLkpDO]   
(  
 @CompanyCode varchar(15),  
 @BranchCode varchar(15),
 @ProfitCenterCode varchar(15)
 )  
AS 

DECLARE 
	@QRYTemp as varchar(max),
	@CompanyMD as varchar(15),
	@DBMD as varchar(15)
 
set @CompanyMD = (SELECT CompanyMD FROM gnMstCompanyMapping WHERE CompanyCode=@CompanyCode AND BranchCode=@BranchCode) 
set @DBMD = (SELECT DbMD FROM gnMstCompanyMapping WHERE CompanyCode=@CompanyCode AND BranchCode=@BranchCode) 
 
BEGIN  
-- exec uspfn_omSlsBPKLkpDO  6006410,600641001,'100'
if @DBMD IS NULL
begin
	SELECT Distinct a.DONo, a.DODate, a.SONo, g.SKPKNo, g.RefferenceNo, a.CustomerCode ,c.CustomerName, 
    c.Address1 + ' ' + c.Address2 + ' ' + c.Address3 + ' ' + c.Address4 as Address,
    a.ShipTo, c1.CustomerName as ShipName, 
    a.WareHouseCode, f.LookUpValueName as WareHouseName, a.Expedition, e.SupplierName as ExpeditionName,
    b.SalesType,(CASE ISNULL(b.SalesType, 0) WHEN 0 THEN 'WholeSales' ELSE 'Direct' END) AS SalesTypeDsc, a.Remark
    FROM omTrSalesDO a
    LEFT JOIN gnMstCustomerProfitCenter b ON b.CompanyCode = a.CompanyCode AND b.BranchCode = a.BranchCode AND b.CustomerCode = a.CustomerCode
	LEFT JOIN gnMstCustomer c ON c.CompanyCode = a.CompanyCode AND c.CustomerCode = a.CustomerCode
    LEFT JOIN gnMstCustomer c1 ON c1.CompanyCode = a.CompanyCode AND c1.CustomerCode = a.ShipTo
	LEFT JOIN omTrSalesDODetail d ON d.CompanyCode = a.CompanyCode AND d.BranchCode = a.BranchCode AND d.DoNo = a.DoNo
	LEFT JOIN gnMstSupplier e ON e.CompanyCode = a.CompanyCode AND e.SupplierCode = a.Expedition 
	LEFT JOIN gnMstLookUpDtl f ON f.CompanyCode = @CompanyCode AND f.LookUpValue = a.WareHouseCode and f.CodeID ='MPWH'
    LEFT JOIN omTrSalesSO g ON a.CompanyCode = g.CompanyCode AND a.BranchCode = g.BranchCode AND a.SONo = g.SONo            
    WHERE a.Status = '2'
    and d.StatusBPK <> '1'  
    AND a.CompanyCode = @CompanyCode
    AND b.BranchCode = @BranchCode
	AND b.ProfitCenterCode = @ProfitCenterCode                   
    ORDER BY a.DODate DESC
end
else
	SET @QRYTemp =
	'SELECT Distinct a.DONo, a.DODate, a.SONo, g.SKPKNo, g.RefferenceNo, a.CustomerCode ,c.CustomerName, 
            c.Address1 + '' '' + c.Address2 + '' '' + c.Address3 + '' '' + c.Address4 as Address,
            a.ShipTo, c1.CustomerName as ShipName, 
            a.WareHouseCode, f.LookUpValueName as WareHouseName, a.Expedition, e.SupplierName as ExpeditionName,
            b.SalesType,(CASE ISNULL(b.SalesType, 0) WHEN 0 THEN ''WholeSales'' ELSE ''Direct'' END) AS SalesTypeDsc, a.Remark
            FROM omTrSalesDO a
            LEFT JOIN gnMstCustomerProfitCenter b ON b.CompanyCode = a.CompanyCode AND b.BranchCode = a.BranchCode AND b.CustomerCode = a.CustomerCode
			LEFT JOIN gnMstCustomer c ON c.CompanyCode = a.CompanyCode AND c.CustomerCode = a.CustomerCode
            LEFT JOIN gnMstCustomer c1 ON c1.CompanyCode = a.CompanyCode AND c1.CustomerCode = a.ShipTo
			LEFT JOIN  omTrSalesDODetail d ON d.CompanyCode = a.CompanyCode AND d.BranchCode = a.BranchCode AND d.DoNo = a.DoNo
			LEFT JOIN gnMstSupplier e ON e.CompanyCode = a.CompanyCode AND e.SupplierCode = a.Expedition 
			LEFT JOIN ' + @DBMD + '.dbo.gnMstLookUpDtl f ON f.CompanyCode = ''' + @CompanyMD + ''' AND f.LookUpValue = a.WareHouseCode and f.CodeID =''MPWH''
            LEFT JOIN omTrSalesSO g ON a.CompanyCode = g.CompanyCode AND a.BranchCode = g.BranchCode AND a.SONo = g.SONo            
            WHERE a.Status = ''2''
            and d.StatusBPK <> ''1''  
            AND a.CompanyCode = ''' + @CompanyCode + '''
            AND b.BranchCode = ''' + @BranchCode + '''
			AND b.ProfitCenterCode = ''' + @ProfitCenterCode + '''                   
            ORDER BY a.DODate DESC'

	EXEC (@QRYTemp);
END      





go
ALTER procedure [dbo].[uspfn_omSlsDoBrowse]   
(  
 @CompanyCode varchar(15),  
 @BranchCode varchar(15) 
)  
AS 

DECLARE 
	@QRYTemp as varchar(max),
	@CompanyMD as varchar(15),
	@DBMD as varchar(15)
 
set @CompanyMD = (SELECT CompanyMD FROM gnMstCompanyMapping WHERE CompanyCode=@CompanyCode AND BranchCode=@BranchCode) 
set @DBMD = (SELECT DbMD FROM gnMstCompanyMapping WHERE CompanyCode=@CompanyCode AND BranchCode=@BranchCode) 


BEGIN  
--exec uspfn_omSlsDoBrowse 6006410,600641001
if @DBMD IS NULL
begin
	SELECT Distinct a.DONo, a.DODate, d.SKPKNo, d.RefferenceNo, a.CustomerCode, c.CustomerName , a.ShipTo ,c.CustomerName as ShipToDsc,
    c.Address1 + ' ' + c.Address2 + ' ' + c.Address3 + ' ' + c.Address4 as Address,
    a.WareHouseCode, a.Expedition, a.SONo, f.CustomerName as ExpeditionDsc,a.Remark,
    CASE a.Status WHEN 0 THEN 'OPEN' WHEN 1 THEN 'PRINT' WHEN 2 THEN 'APPROVED' WHEN 3 THEN 'CANCEL' WHEN 9 THEN 'FINISH' END as StatusDsc,a.Status
    , CASE ISNULL(b.SalesType, 0) WHEN 0 THEN 'Wholesales' ELSE 'Direct' END AS TypeSales, e.LookUpValueName as WrhDsc
    FROM omTrSalesDO a
    LEFT JOIN gnMstCustomerProfitCenter b ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.CustomerCode = b.CustomerCode AND b.ProfitCenterCode = '100'
    LEFT JOIN gnMstCustomer c ON a.CompanyCode = c.CompanyCode AND a.CustomerCode = c.CustomerCode
    LEFT JOIN omTrSalesSO d ON a.CompanyCode = d.CompanyCode AND a.BranchCode = d.BranchCode AND a.SONo = d.SONo      
    Left join gnMstLookUpDtl e on a.WarehouseCode=e.LookUpValue and e.CodeID='MPWH'
    LEFT JOIN gnMstCustomer f ON a.CompanyCode = c.CompanyCode AND a.Expedition = c.CustomerCode
    WHERE a.CompanyCode = @CompanyCode
        AND a.BranchCode = @BranchCode                             
    ORDER BY a.DONo DESC
end
else
 set @QRYTemp =  'SELECT Distinct a.DONo, a.DODate, d.SKPKNo, d.RefferenceNo, a.CustomerCode, c.CustomerName , a.ShipTo ,c.CustomerName as ShipToDsc,
            c.Address1 + '' '' + c.Address2 + '' '' + c.Address3 + '' '' + c.Address4 as Address,
            a.WareHouseCode, a.Expedition, a.SONo, f.CustomerName as ExpeditionDsc,a.Remark,
            CASE a.Status WHEN 0 THEN ''OPEN'' WHEN 1 THEN ''PRINT'' WHEN 2 THEN ''APPROVED'' WHEN 3 THEN ''CANCEL'' WHEN 9 THEN ''FINISH'' END as StatusDsc,a.Status
            , CASE ISNULL(b.SalesType, 0) WHEN 0 THEN ''Wholesales'' ELSE ''Direct'' END AS TypeSales, e.LookUpValueName as WrhDsc
            FROM omTrSalesDO a
            LEFT JOIN gnMstCustomerProfitCenter b ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.CustomerCode = b.CustomerCode AND b.ProfitCenterCode = ''100''
            LEFT JOIN gnMstCustomer c ON a.CompanyCode = c.CompanyCode AND a.CustomerCode = c.CustomerCode
            LEFT JOIN omTrSalesSO d ON a.CompanyCode = d.CompanyCode AND a.BranchCode = d.BranchCode AND a.SONo = d.SONo      
            Left join ' + @DBMD + '.dbo.gnMstLookUpDtl e on a.WarehouseCode=e.LookUpValue and e.CodeID=''MPWH''
            LEFT JOIN gnMstCustomer f ON a.CompanyCode = c.CompanyCode AND a.Expedition = c.CustomerCode
            WHERE a.CompanyCode = ''' + @CompanyCode + '''
               AND a.BranchCode = ''' + @BranchCode + '''                             
            ORDER BY a.DONo DESC'

	exec (@QRYTemp);
end         


go
ALTER procedure [dbo].[uspfn_omSlsDoUpdateSOVin]   
(  
 @CompanyCode varchar(15),  
 @BranchCode varchar(15),  
 @DONo varchar(15) 
)  
AS  

DECLARE 
	@QRYTemp as varchar(max),
	@CompanyMD as varchar(15),
	@DBMD as varchar(15)
 
set @CompanyMD = (SELECT CompanyMD FROM gnMstCompanyMapping WHERE CompanyCode=@CompanyCode AND BranchCode=@BranchCode) 
set @DBMD = (SELECT DbMD FROM gnMstCompanyMapping WHERE CompanyCode=@CompanyCode AND BranchCode=@BranchCode)

begin
--exec uspfn_omSlsDoUpdateSOVin 6006410,600641001,'DOS/14/000025'
if @DBMD IS NULL
begin
	select a.*,b.SONo,c.ServiceBookNo,c.KeyNo from OmTrSalesDODetail a inner join OmTrSalesDO b 
		on a.companyCode = b.companyCode and a.branchCode = b.branchCode and a.DONo = b.DONo
		inner join OmMstVehicle c on a.chassisCode = c.chassisCode and a.chassisNo = c.chassisNo
	where a.companyCode = @CompanyCode
	and a.branchCode = @BranchCode and a.DONo = @DONo
end
else
	set @QRYTemp =
	'select a.*,b.SONo,c.ServiceBookNo,c.KeyNo from OmTrSalesDODetail a inner join OmTrSalesDO b 
		on a.companyCode = b.companyCode and a.branchCode = b.branchCode and a.DONo = b.DONo
		inner join ' + @DBMD + '.dbo.OmMstVehicle c on a.chassisCode = c.chassisCode and a.chassisNo = c.chassisNo
	where a.companyCode = ''' + @CompanyCode + '''
	and a.branchCode = ''' + @BranchCode + ''' and a.DONo = ''' + @DONo + ''''
				
	exec (@QRYTemp);	 
end	


go
ALTER procedure [dbo].[uspfn_omSoLkp] 
(
	@CompanyCode varchar(25),
	@BranchCode varchar(25)
)
as
 
 -- exec uspfn_omSoLkp '6115204001','6115204105'
 
 declare @DbMD as varchar(15)  
 declare @Sql as varchar(max) 
 declare @ssql as varchar(max) 
 
 set @DbMD = (select TOP 1 DbMD from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)  
 
 set @ssql='select * from gnMstCompanyMapping '

 if @DbMD IS NULL 
	begin
		set @Sql= 'SELECT a.CompanyCode, a.BranchCode,
                a.SONo, a.SODate, a.SalesType, a.CustomerCode, a.TOPCode, a.Installment, a.FinalPaymentDate,
                a.TOPDays, a.BillTo, a.ShipTo, a.ProspectNo, a.SKPKNo, a.Salesman, a.WareHouseCode, a.isLeasing, 
                a.LeasingCo, a.GroupPriceCode, a.Insurance, a.PaymentType, a.PrePaymentAmt, a.PrePaymentBy, 
                a.CommissionBy, a.CommissionAmt, a.PONo, a.ContractNo, a.Remark, a.Status,
                a.SalesCoordinator, a.SalesHead, a.BranchManager, a.RefferenceNo,
                CASE convert(varchar, a.RefferenceDate, 111) when convert(varchar, ''1900/01/01'') 
                then '''' else convert(varchar, a.RefferenceDate, 111) end as RefferenceDates, 
                CASE convert(varchar, a.RefferenceDate, 111) when convert(varchar, ''1900/01/01'') 
                then ''undefined'' else convert(varchar, a.RefferenceDate, 111) end as RefferenceDate, 
                CASE convert(varchar, a.RequestDate, 111) when convert(varchar, ''1900/01/01'') 
                then ''undefined'' else convert(varchar, a.RequestDate, 111) end as RequestDate,
                CASE convert(varchar, a.PrePaymentDate, 111) when convert(varchar, ''1900/01/01'') 
                then ''undefined'' else convert(varchar, a.PrePaymentDate, 111) end as PrePaymentDate,
                e.Address1 + '' '' + e.Address2 + '' '' + e.Address3 + '' '' + e.Address4 as Address,
                case when year(a.RefferenceDate) <> 1900 then convert(bit,1) else convert(bit,0) end isC1,
                case when a.SKPKNo <> '''' then convert(bit,1) else convert(bit,0) end isC2,
                case when year(a.PrePaymentDate) <> 1900 then convert(bit,1) else convert(bit,0) end isC3,
                case when year(a.RequestDate) <> 1900 then convert(bit,1) else convert(bit,0) end isC4,
				(SELECT b.CustomerName
                        FROM gnMstCustomer b
                        WHERE a.CompanyCode = b.CompanyCode
						AND a.CustomerCode = b.CustomerCode)  
						AS CustomerName,
				(SELECT c.EmployeeName
                        FROM gnMstEmployee c
                        WHERE a.CompanyCode = c.CompanyCode AND a.BranchCode = c.BranchCode
					    AND a.Salesman = c.EmployeeID) as SalesmanName,
				(SELECT b.CustomerName
                        FROM gnMstCustomer b
                        WHERE a.CompanyCode = b.CompanyCode
						AND a.Shipto = b.CustomerCode)  
						AS ShipName,
                (SELECT b.CustomerName
                        FROM gnMstCustomer b
                        WHERE a.CompanyCode = b.CompanyCode
						AND a.LeasingCo = b.CustomerCode)  
						AS LeasingCoName,
				(SELECT c.EmployeeName
                        FROM gnMstEmployee c
                        WHERE a.CompanyCode = c.CompanyCode AND a.BranchCode = c.BranchCode
					    AND a.PrePaymentby = c.EmployeeID) as PrePaymentName,
				(SELECT d.RefferenceDesc1
                        FROM omMstRefference d
                        WHERE a.CompanyCode = d.CompanyCode
						AND d.RefferenceType = ''GRPR'' 
                        AND d.RefferenceCode = a.GroupPriceCode) AS GroupPriceName,
				(SELECT b.CustomerName
                        FROM gnMstCustomer b
                        WHERE a.CompanyCode = b.CompanyCode
						AND a.CustomerCode = b.CustomerCode)  
						AS BillName,
				(SELECT TOP 1 b.lookupvaluename
                        FROM gnMstLookUpDtl b
                        WHERE a.WareHouseCode = b.LookUpValue
						AND a.WareHouseCode = b.LookUpValue and CodeID =''MPWH'')  
						AS WareHouseName,
                (a.CustomerCode
                    + '' || ''
                    + (SELECT b.CustomerName
                        FROM gnMstCustomer b
                        WHERE a.CompanyCode = b.CompanyCode
						AND a.CustomerCode = b.CustomerCode))  
						AS Customer, 
                (a.Salesman
                    + '' || ''
                    + (SELECT c.EmployeeName
                        FROM gnMstEmployee c
                        WHERE a.CompanyCode = c.CompanyCode AND a.BranchCode = c.BranchCode
					    AND a.Salesman = c.EmployeeID))  AS Sales, 
                (a.GroupPriceCode
                    + '' || ''
                    + (SELECT d.RefferenceDesc1
                        FROM omMstRefference d
                        WHERE a.CompanyCode = d.CompanyCode
						AND d.RefferenceType = ''GRPR'' 
                        AND d.RefferenceCode = a.GroupPriceCode))  AS GroupPrice, 
                CASE a.Status when 0 then ''OPEN''
                                when 1 then ''PRINTED''
                                when 2 then ''APPROVED''
                                when 3 then ''DELETED''
                                when 4 then ''REJECTED''
                                when 9 then ''FINISHED'' END as Stat
                , CASE ISNULL(a.SalesType, 0) WHEN 0 THEN ''Wholesales'' ELSE ''Direct'' END AS TypeSales
                ,(select distinct x.TipeKendaraan
                    from pmKDP x
	                    left join gnMstEmployee b on x.CompanyCode=b.CompanyCode and x.BranchCode=b.BranchCode
		                    and x.EmployeeID=b.EmployeeID
	                    left join omTrSalesSO c on c.CompanyCode = x.CompanyCode 
		                    and c.BranchCode = x.BranchCode
		                    and c.ProspectNo = x.InquiryNumber
	                    left join omTrSalesInvoice d on d.CompanyCode = x.CompanyCode
		                    and d.BranchCode = x.BranchCode
		                    and d.SONo = c.SONo
	                    left join omTrSalesReturn e on e.CompanyCode = x.CompanyCode
		                    and e.BranchCode = x.BranchCode
		                    and e.InvoiceNo = d.InvoiceNo
                    where x.InquiryNumber=a.ProspectNo AND a.CompanyCode = x.CompanyCode AND a.BranchCode = x.BranchCode) as VehicleType
                FROM omTrSalesSO a
                INNER JOIN gnMstCustomer e
                ON a.CompanyCode = e.CompanyCode AND a.CustomerCode = e.CustomerCode
				Where a.CompanyCode = '+ @CompanyCode+' and a.BranchCode = '+ @BranchCode +'
				order by a.SONo desc'
	end
 else
	set @Sql= 'SELECT a.CompanyCode, a.BranchCode,
                a.SONo, a.SODate, a.SalesType, a.CustomerCode, a.TOPCode, a.Installment, a.FinalPaymentDate,
                a.TOPDays, a.BillTo, a.ShipTo, a.ProspectNo, a.SKPKNo, a.Salesman, a.WareHouseCode, a.isLeasing, 
                a.LeasingCo, a.GroupPriceCode, a.Insurance, a.PaymentType, a.PrePaymentAmt, a.PrePaymentBy, 
                a.CommissionBy, a.CommissionAmt, a.PONo, a.ContractNo, a.Remark, a.Status,
                a.SalesCoordinator, a.SalesHead, a.BranchManager, a.RefferenceNo,
                CASE convert(varchar, a.RefferenceDate, 111) when convert(varchar, ''1900/01/01'') 
                then '''' else convert(varchar, a.RefferenceDate, 111) end as RefferenceDates, 
                CASE convert(varchar, a.RefferenceDate, 111) when convert(varchar, ''1900/01/01'') 
                then ''undefined'' else convert(varchar, a.RefferenceDate, 111) end as RefferenceDate, 
                CASE convert(varchar, a.RequestDate, 111) when convert(varchar, ''1900/01/01'') 
                then ''undefined'' else convert(varchar, a.RequestDate, 111) end as RequestDate,
                CASE convert(varchar, a.PrePaymentDate, 111) when convert(varchar, ''1900/01/01'') 
                then ''undefined'' else convert(varchar, a.PrePaymentDate, 111) end as PrePaymentDate,
                e.Address1 + '' '' + e.Address2 + '' '' + e.Address3 + '' '' + e.Address4 as Address,
                case when year(a.RefferenceDate) <> 1900 then convert(bit,1) else convert(bit,0) end isC1,
                case when a.SKPKNo <> '''' then convert(bit,1) else convert(bit,0) end isC2,
                case when year(a.PrePaymentDate) <> 1900 then convert(bit,1) else convert(bit,0) end isC3,
                case when year(a.RequestDate) <> 1900 then convert(bit,1) else convert(bit,0) end isC4,
				(SELECT b.CustomerName
                        FROM gnMstCustomer b
                        WHERE a.CompanyCode = b.CompanyCode
						AND a.CustomerCode = b.CustomerCode)  
						AS CustomerName,
				(SELECT c.EmployeeName
                        FROM gnMstEmployee c
                        WHERE a.CompanyCode = c.CompanyCode AND a.BranchCode = c.BranchCode
					    AND a.Salesman = c.EmployeeID) as SalesmanName,
				(SELECT b.CustomerName
                        FROM gnMstCustomer b
                        WHERE a.CompanyCode = b.CompanyCode
						AND a.Shipto = b.CustomerCode)  
						AS ShipName,
                (SELECT b.CustomerName
                        FROM gnMstCustomer b
                        WHERE a.CompanyCode = b.CompanyCode
						AND a.LeasingCo = b.CustomerCode)  
						AS LeasingCoName,
				(SELECT c.EmployeeName
                        FROM gnMstEmployee c
                        WHERE a.CompanyCode = c.CompanyCode AND a.BranchCode = c.BranchCode
					    AND a.PrePaymentby = c.EmployeeID) as PrePaymentName,
				(SELECT d.RefferenceDesc1
                        FROM omMstRefference d
                        WHERE a.CompanyCode = d.CompanyCode
						AND d.RefferenceType = ''GRPR'' 
                        AND d.RefferenceCode = a.GroupPriceCode) AS GroupPriceName,
				(SELECT b.CustomerName
                        FROM gnMstCustomer b
                        WHERE a.CompanyCode = b.CompanyCode
						AND a.CustomerCode = b.CustomerCode)  
						AS BillName,
				(SELECT TOP 1 b.lookupvaluename
                        FROM '+@DbMD+'..gnMstLookUpDtl b
                        WHERE a.WareHouseCode = b.LookUpValue
						AND a.WareHouseCode = b.LookUpValue and CodeID =''MPWH'')  
						AS WareHouseName,
                (a.CustomerCode
                    + '' || ''
                    + (SELECT b.CustomerName
                        FROM gnMstCustomer b
                        WHERE a.CompanyCode = b.CompanyCode
						AND a.CustomerCode = b.CustomerCode))  
						AS Customer, 
                (a.Salesman
                    + '' || ''
                    + (SELECT c.EmployeeName
                        FROM gnMstEmployee c
                        WHERE a.CompanyCode = c.CompanyCode AND a.BranchCode = c.BranchCode
					    AND a.Salesman = c.EmployeeID))  AS Sales, 
                (a.GroupPriceCode
                    + '' || ''
                    + (SELECT d.RefferenceDesc1
                        FROM omMstRefference d
                        WHERE a.CompanyCode = d.CompanyCode
						AND d.RefferenceType = ''GRPR'' 
                        AND d.RefferenceCode = a.GroupPriceCode))  AS GroupPrice, 
                CASE a.Status when 0 then ''OPEN''
                                when 1 then ''PRINTED''
                                when 2 then ''APPROVED''
                                when 3 then ''DELETED''
                                when 4 then ''REJECTED''
                                when 9 then ''FINISHED'' END as Stat
                , CASE ISNULL(a.SalesType, 0) WHEN 0 THEN ''Wholesales'' ELSE ''Direct'' END AS TypeSales
                ,(select distinct x.TipeKendaraan
                    from pmKDP x
	                    left join gnMstEmployee b on x.CompanyCode=b.CompanyCode and x.BranchCode=b.BranchCode
		                    and x.EmployeeID=b.EmployeeID
	                    left join omTrSalesSO c on c.CompanyCode = x.CompanyCode 
		                    and c.BranchCode = x.BranchCode
		                    and c.ProspectNo = x.InquiryNumber
	                    left join omTrSalesInvoice d on d.CompanyCode = x.CompanyCode
		                    and d.BranchCode = x.BranchCode
		                    and d.SONo = c.SONo
	                    left join omTrSalesReturn e on e.CompanyCode = x.CompanyCode
		                    and e.BranchCode = x.BranchCode
		                    and e.InvoiceNo = d.InvoiceNo
                    where x.InquiryNumber=a.ProspectNo AND a.CompanyCode = x.CompanyCode AND a.BranchCode = x.BranchCode) as VehicleType
                FROM omTrSalesSO a
                INNER JOIN gnMstCustomer e
                ON a.CompanyCode = e.CompanyCode AND a.CustomerCode = e.CustomerCode
				Where a.CompanyCode = '+ @CompanyCode+' and a.BranchCode = '+ @BranchCode +'
				order by a.SONo desc
				'
--print @Sql

exec (@Sql)

go
ALTER procedure [dbo].[uspfn_SvTrnInvoiceCancel]
	@CompanyCode   varchar(20),
	@BranchCode    varchar(20),
	@InvoiceNo     varchar(20),
	@UserInfo      varchar(max)
as  

declare @errmsg varchar(max)
declare @JobOrderNo varchar(20) 
declare @InvoiceDate DateTime

begin try
set nocount on
	set @JobOrderNo = (Select JobOrderNo from svTrnInvoice where CompanyCode = @CompanyCode and BranchCode = @BranchCode and InvoiceNo = @InvoiceNo)
	set @InvoiceDate = (Select InvoiceDate from svTrnInvoice where CompanyCode = @CompanyCode and BranchCode = @BranchCode and InvoiceNo = @InvoiceNo)

	declare @CompanyMD as varchar(15)
	declare @BranchMD as varchar(15)

	set @CompanyMD = (select CompanyMD from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)
	set @BranchMD = (select BranchMD from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)
	
	if (@CompanyCode != @CompanyMD and @BranchCode != @BranchMD)
	begin
		IF convert(varchar,@InvoiceDate,112) < convert(varchar,getdate(),112)
		begin
			raiserror('Tanggal invoice lebih kecil dari tanggal hari ini',16 ,1 );
		end
	end
	
	if exists (
	select * from ArInterface
	 where CompanyCode = @CompanyCode
	   and BranchCode  = @BranchCode
	   and (StatusFlag > '0' or ReceiveAmt > 0 or BlockAmt > 0 or DebetAmt > 0 or CreditAmt > 0)
	   and DocNo in (Select InvoiceNo from svTrnInvoice where CompanyCode = @CompanyCode and BranchCode = @BranchCode and JobOrderNo = @JobOrderNo)
	)
	begin
		raiserror('Invoice sudah ada proses Receiving, transaksi tidak bisa dilanjutkan',16 ,1 );
	end

	if exists (
	select * from svTrnFakturPajak
	 where CompanyCode = @CompanyCode
	   and BranchCode = @BranchCode
	   and FPJNo in (select FPJNo from svTrnInvoice where CompanyCode = @CompanyCode and BranchCode = @BranchCode and JobOrderNo = @JobOrderNo)
	   and isnull(FPJGovNo, '') <> ''
	   and left(FPJGovNo, 3) <> 'SHN'
	)
	begin
		raiserror('Invoice sudah tergenerate Nomor Pajak Pemerintah, transaksi tidak bisa dilanjutkan',16 ,1 );
	end

	;with x as (
		select a.BranchCode, a.CustomerCode, a.SalesAmt, b.TotalSrvAmt
		  from gnTrnBankBook a, svTrnInvoice b
		 where 1 = 1
		   and b.CompanyCode = a.CompanyCode
		   and b.BranchCode = a.BranchCode
		   and b.CustomerCodeBill = a.CustomerCode
		   and a.ProfitCenterCode = '200'
		   and a.CompanyCode = @BranchCode
		   and a.BranchCode = @BranchCode
		   and b.JobOrderNo = @JobOrderNo
	)
	update x set SalesAmt = SalesAmt - TotalSrvAmt where SalesAmt > 0

	delete from glInterface 
	 where CompanyCode = @CompanyCode
	   and BranchCode = @BranchCode
	   and DocNo in (Select InvoiceNo from svTrnInvoice where CompanyCode = @CompanyCode and BranchCode = @BranchCode and JobOrderNo = @JobOrderNo)
	delete from arInterface 
	 where CompanyCode = @CompanyCode
	   and BranchCode = @BranchCode
	   and DocNo in (Select InvoiceNo from svTrnInvoice where CompanyCode = @CompanyCode and BranchCode = @BranchCode and JobOrderNo = @JobOrderNo)

	update svTrnService set ServiceStatus = 8, InvoiceNo = ''
	 where CompanyCode = @CompanyCode
	   and BranchCode = @BranchCode
	   and JobOrderNo = @JobOrderNo

	delete svTrnFakturPajak
	 where CompanyCode = @CompanyCode
	   and BranchCode = @BranchCode
	   and FPJNo in (select FPJNo from svTrnInvoice where CompanyCode = @CompanyCode and BranchCode = @BranchCode and JobOrderNo = @JobOrderNo)

	-------------------------------------------------------------------------------------------------------------------
	-- Insert into table log
	-------------------------------------------------------------------------------------------------------------------
	declare @TransID   uniqueidentifier; 
	declare @TransCode varchar(20);

	set @TransID = newid()
	set @TransCode = 'CANCEL INVOICE' 

	insert into svTrnInvoiceLog (
		TransID,TransCode,CompanyCode,BranchCode,ProductType,InvoiceNo,InvoiceDate,InvoiceStatus,
		FPJNo,FPJDate,JobOrderNo,JobOrderDate,JobType,ServiceRequestDesc,ChassisCode,ChassisNo,
		EngineCode,EngineNo,PoliceRegNo,BasicModel,CustomerCode,CustomerCodeBill,Odometer,
		IsPKP,TOPCode,TOPDays,DueDate,SignedDate,LaborDiscPct,PartsDiscPct,MaterialDiscPct,
		PphPct,PpnPct,LaborGrossAmt,PartsGrossAmt,MaterialGrossAmt,LaborDiscAmt,PartsDiscAmt,MaterialDiscAmt,
		LaborDppAmt,PartsDppAmt,MaterialDppAmt,TotalDppAmt,TotalPphAmt,TotalPpnAmt,TotalSrvAmt,
		Remarks,PrintSeq,PostingFlag,PostingDate,CreatedBy,CreatedDate
	) 
	select 
		@TransID, @TransCode,CompanyCode,BranchCode,ProductType,InvoiceNo,InvoiceDate,InvoiceStatus,
		FPJNo,FPJDate,JobOrderNo,JobOrderDate,JobType,left(ServiceRequestDesc, 200),ChassisCode,ChassisNo,
		EngineCode,EngineNo,PoliceRegNo,BasicModel,CustomerCode,CustomerCodeBill,Odometer,
		IsPKP,TOPCode,TOPDays,DueDate,SignedDate,LaborDiscPct,PartsDiscPct,MaterialDiscPct,
		PphPct,PpnPct,LaborGrossAmt,PartsGrossAmt,MaterialGrossAmt,LaborDiscAmt,PartsDiscAmt,MaterialDiscAmt,
		LaborDppAmt,PartsDppAmt,MaterialDppAmt,TotalDppAmt,TotalPphAmt,TotalPpnAmt,TotalSrvAmt,
		Remarks,PrintSeq,PostingFlag,PostingDate,@UserInfo,CreatedDate
	from svTrnInvoice
	 where CompanyCode = @CompanyCode and BranchCode = @BranchCode and InvoiceNo in (Select InvoiceNo from svTrnInvoice where CompanyCode = @CompanyCode and BranchCode = @BranchCode and JobOrderNo = @JobOrderNo)

	insert into svTrnInvTaskLog(TransID,TransCode,CompanyCode,BranchCode,ProductType,InvoiceNo,OperationNo,OperationHour,ClaimHour,OperationCost,SubConPrice,IsSubCon,SharingTask)
	select @TransID,@TransCode,CompanyCode,BranchCode,ProductType,InvoiceNo,OperationNo,OperationHour,ClaimHour,OperationCost,SubConPrice,IsSubCon,SharingTask from svTrnInvTask 
	 where CompanyCode = @CompanyCode and BranchCode = @BranchCode and InvoiceNo in (Select InvoiceNo from svTrnInvoice where CompanyCode = @CompanyCode and BranchCode = @BranchCode and JobOrderNo = @JobOrderNo)

	insert into svTrnInvItemLog(TransID,TransCode,CompanyCode, BranchCode, ProductType, InvoiceNo, PartNo, MovingCode, ABCClass, SupplyQty, ReturnQty, CostPrice, RetailPrice, TypeOfGoods)
	select @TransID,@TransCode,CompanyCode, BranchCode, ProductType, InvoiceNo, PartNo, MovingCode, ABCClass, SupplyQty, ReturnQty, CostPrice, RetailPrice, TypeOfGoods from svTrnInvItem
	 where CompanyCode = @CompanyCode and BranchCode = @BranchCode and InvoiceNo in (Select InvoiceNo from svTrnInvoice where CompanyCode = @CompanyCode and BranchCode = @BranchCode and JobOrderNo = @JobOrderNo)

	delete svTrnInvItemDtl where CompanyCode = @CompanyCode and BranchCode = @BranchCode and InvoiceNo in (Select InvoiceNo from svTrnInvoice where CompanyCode = @CompanyCode and BranchCode = @BranchCode and JobOrderNo = @JobOrderNo)
	delete svTrnInvItem where CompanyCode = @CompanyCode and BranchCode = @BranchCode and InvoiceNo in (Select InvoiceNo from svTrnInvoice where CompanyCode = @CompanyCode and BranchCode = @BranchCode and JobOrderNo = @JobOrderNo)
	delete svTrnInvMechanic where CompanyCode = @CompanyCode and BranchCode = @BranchCode and InvoiceNo in (Select InvoiceNo from svTrnInvoice where CompanyCode = @CompanyCode and BranchCode = @BranchCode and JobOrderNo = @JobOrderNo)
	delete svTrnInvTask where CompanyCode = @CompanyCode and BranchCode = @BranchCode and InvoiceNo in (Select InvoiceNo from svTrnInvoice where CompanyCode = @CompanyCode and BranchCode = @BranchCode and JobOrderNo = @JobOrderNo)
	delete svTrnInvoice where CompanyCode = @CompanyCode and BranchCode = @BranchCode and InvoiceNo in (Select InvoiceNo from svTrnInvoice where CompanyCode = @CompanyCode and BranchCode = @BranchCode and JobOrderNo = @JobOrderNo)
	
	if (@CompanyCode != @CompanyMD and @BranchCode != @BranchMD)
	begin
	declare @Query VARCHAR(MAX)
		
	set @Query ='delete from '+dbo.GetDbMD(@CompanyCode, @BranchCode)+'..svSDMovement 
		where CompanyCode = '''+ @CompanyCode +''' and BranchCode = '''+ @BranchCode +'''
		and DocNo in (Select InvoiceNo from svTrnInvoice where CompanyCode = '''+ @CompanyCode +''' and BranchCode = '''+ @BranchCode +''' and JobOrderNo = '''+ @JobOrderNo +''')'	
	end
	
end try
begin catch
    set @errmsg = 'InvoiceNo : ' + @InvoiceNo + char(13) + 'Error Message:' + char(13) + error_message()
    raiserror (@errmsg,16,1);
end catch
go
if object_id('usprpt_OmRpSalesTrn007BWeb') is not null
	drop PROCEDURE usprpt_OmRpSalesTrn007BWeb
GO
-- usprpt_OmRpSalesTrn007 '6006406','6006406','RFP/11/000003','RFP/11/000003'
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<PERMOHONAN FAKTUR POLISI>
-- =============================================
CREATE procedure [dbo].[usprpt_OmRpSalesTrn007BWeb]
	-- Add the parameters for the stored procedure here
	@CompanyCode VARCHAR(15),
	@BranchCode	 VARCHAR(15),
	@ReqNoA		 VARCHAR(15),
	@ReqNoB		 VARCHAR(15)

AS

DECLARE
	@QRYTmp		AS varchar(max),
	@DBMD		AS varchar(25),
	@CompanyMD  AS varchar(25)

BEGIN

set @CompanyMD = (SELECT TOP 1 CompanyMD FROM gnMstCompanyMapping WHERE CompanyCode=@CompanyCode AND BranchCode=@BranchCode) 
set @DBMD = (SELECT TOP 1 DbMD FROM gnMstCompanyMapping WHERE CompanyCode=@CompanyCode AND BranchCode=@BranchCode)

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

set @QRYTmp = 
'SELECT
	row_number () OVER (ORDER BY a.ReqNo) AS No
	, a.ReqNo
	, a.SKPKNo
	, a.FakturPolisiNo
	, ISNULL(e.RefferenceDONo, '''') DONo
	, case when convert(varchar,isnull((SELECT dbo.GetDateIndonesian (a.FakturPolisiDate)),''19000101''),112) <> ''19000101''  
		then (SELECT dbo.GetDateIndonesian (a.FakturPolisiDate)) 
		else (SELECT dbo.GetDateIndonesian (b.ReqDate)) 
		end AS ''Tanggal''
	, ISNULL(e.RefferenceDoDate, '''') DODate
	, ISNULL(d.CompanyName, '''') CompanyName
	, ISNULL(d.Address1, '''') CoAdd1
	, ISNULL(d.Address2, '''') CoAdd2
	, ISNULL(d.Address3, '''') CoAdd3
	, case d.ProductType 
		when ''2W'' then ''Harap dibuatkan Faktur untuk motor SUZUKI :''
		when ''4W'' then ''Harap dibuatkan Faktur untuk mobil SUZUKI :''
		when ''A'' then ''Harap dibuatkan Faktur untuk motor SUZUKI :''
		when ''B'' then ''Harap dibuatkan Faktur untuk mobil SUZUKI :''
		else ''Harap dibuatkan Faktur untuk SUZUKI :''
		end as Note
	, ISNULL(e.RefferenceSJNo, '''') SJNo
	, ISNULL(e.RefferenceSJDate, '''') SJDate
	, ISNULL(c.SalesModelCode, '''') Model
	, ISNULL(f.SalesModelDesc, '''') ModelDesc
	, ISNULL(g.RefferenceDesc1, '''') Warna
	, ISNULL(c.SalesModelYear, 0) Tahun
	, a.ChassisNo
	, ISNULL(c.EngineNo, 0) EngineNo
	, ((CASE ISNULL(a.DealerCategory, '''') WHEN ''M'' THEN ''Main Dealer'' WHEN ''S'' THEN ''Sub Dealer'' WHEN ''R'' THEN ''Show Room'' END) + '' / '' + h.CustomerName) AS  Penjual
	, a.SalesmanName
	, a.SKPKName
	, a.SKPKAddress1 Alamat1
	, a.SKPKAddress2 Alamat2
	, a.SKPKAddress3 Alamat3
	, ISNULL(i.LookUpValueName, '''') City
	, a.SKPKTelp1
	, a.SKPKTelp2
	, a.SKPKHP
	, ISNULL(a.SKPKBirthday, '''') SKPKDay
	, a.FakturPolisiName
	, a.FakturPolisiAddress1
	, a.FakturPolisiAddress2
	, a.FakturPolisiAddress3
	, a.FakturPolisiTelp1
	, a.FakturPolisiTelp2
	, a.FakturPolisiHP
	, a.FakturPolisiBirthday
	, (select ISNULL(LookUpValueName, '''') from gnMstLookUpDtl where CompanyCode=a.CompanyCode and CodeID=''FPCT'' and LookUpValue=a.DealerCategory
		) AS DealerCategory
	, ISNULL(b.Remark, '''') Remark
	, ISNULL(UPPER(z.SignName), '''') AS SignName1
	, ISNULL(UPPER(z.TitleSign), '''') AS TitleSign1 
	, a.IDNo
FROM
 omTrSalesReqDetail a
JOIN
 omTrSalesReq b ON b.CompanyCode=a.CompanyCode AND b.BranchCode=a.BranchCode
 AND b.ReqNo=a.ReqNo 
LEFT JOIN
 ' + @DBMD + '..omMstVehicle c ON c.CompanyCode=''' + @CompanyMD + ''' AND c.ChassisCode=a.ChassisCode
 AND c.ChassisNo=a.ChassisNo
LEFT JOIN
 gnMstCoProfile d ON d.CompanyCode=a.CompanyCode AND d.BranchCode=a.BranchCode
LEFT JOIN omTrPurchaseBPUDetail j on a.CompanyCode=j.CompanyCode and c.ChassisCode=j.ChassisCode
	and a.ChassisNo=j.ChassisNo
LEFT JOIN
 omTrPurchaseBPU e ON e.CompanyCode=j.CompanyCode AND e.BranchCode=j.BranchCode
	and e.BPUNo=j.BPUNo
LEFT JOIN
 ' + @DBMD + '..omMstModel f ON f.CompanyCode=''' + @CompanyMD + ''' AND f.SalesModelCode=c.SalesModelCode
LEFT JOIN
 ' + @DBMD + '..omMstRefference g ON g.CompanyCode=''' + @CompanyMD + ''' AND g.RefferenceType=''COLO''
 AND g.RefferenceCode=c.ColourCode
LEFT JOIN
 gnMstCustomer h ON h.CompanyCode=b.CompanyCode AND h.CustomerCode=b.SubDealerCode
LEFT JOIN
 gnMstLookUpDtl i ON i.CompanyCode=a.CompanyCode AND i.CodeID=''CITY'' 
 AND i.LookUpValue=a.SKPKCity
LEFT JOIN gnMstSIgnature z ON z.CompanyCode = a.CompanyCode
	AND z.BranchCode = a.BranchCode
	AND z.ProfitCenterCode = ''100''
	AND z.DocumentType = ''RFP''
	AND z.SeqNo = 1
LEFT JOIN omMstVehicle k ON k.CompanyCode = a.CompanyCode
	AND k.ChassisCode = a.ChassisCode
	AND k.ChassisNo = a.ChassisNo
WHERE
 a.CompanyCode	  =''' + @CompanyCode + '''
 AND a.BranchCode =''' + @BranchCode + '''
 AND a.ReqNo BETWEEN ''' + @ReqNoA + ''' AND ''' + @ReqNoB + '''
 AND k.SalesModelCode NOT IN (select LookUpValueName from gnMstLookUpDtl where CompanyCode=''' + @CompanyCode + ''' and CodeId =''BLOCK'')
ORDER BY ReqNo'

Exec (@QRYTmp);

END
--------------------------------------------------- BATAS ----------------------------------------------------------


go
if object_id('usprpt_OmRpSalesTrn007Web') is not null
	drop PROCEDURE usprpt_OmRpSalesTrn007Web
GO
create procedure [dbo].[usprpt_OmRpSalesTrn007Web]
	-- Add the parameters for the stored procedure here
	@CompanyCode VARCHAR(15),
	@BranchCode	 VARCHAR(15),
	@ReqNoA		 VARCHAR(15),
	@ReqNoB		 VARCHAR(15)

AS

DECLARE
	@QRYTmp		AS varchar(max),
	@DBMD		AS varchar(25),
	@CompanyMD  AS varchar(25)


BEGIN

set @CompanyMD = (SELECT TOP 1 CompanyMD FROM gnMstCompanyMapping WHERE CompanyCode=@CompanyCode AND BranchCode=@BranchCode) 
set @DBMD = (SELECT TOP 1 DbMD FROM gnMstCompanyMapping WHERE CompanyCode=@CompanyCode AND BranchCode=@BranchCode)

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;


set @QRYTmp =
'SELECT
	row_number () OVER (ORDER BY a.ReqNo) AS No
	, a.ReqNo
	, a.SKPKNo
	, a.FakturPolisiNo
	, ISNULL(c.SuzukiDONo, '''') DONo
	, (SELECT dbo.GetDateIndonesian (a.FakturPolisiDate)) AS ''Tanggal''
	, ISNULL(c.SuzukiDODate, '''') DODate
	, ISNULL(d.CompanyName, '''') CompanyName
	, ISNULL(d.Address1, '''') CoAdd1
	, ISNULL(d.Address2, '''') CoAdd2
	, ISNULL(d.Address3, '''') CoAdd3
	, case d.ProductType 
		when ''2W'' then ''Harap dibuatkan Faktur untuk motor SUZUKI :''
		when ''4W'' then ''Harap dibuatkan Faktur untuk mobil SUZUKI :''
		when ''A'' then ''Harap dibuatkan Faktur untuk motor SUZUKI :''
		when ''B'' then ''Harap dibuatkan Faktur untuk mobil SUZUKI :''
		else ''Harap dibuatkan Faktur untuk SUZUKI :''
		end as Note
	, ISNULL(c.SuzukiSJNo, '''') SJNo
	, ISNULL(c.SuzukiSJDate, '''') SJDate
	, ISNULL(c.SalesModelCode, '''') Model
	, ISNULL(f.SalesModelDesc, '''') ModelDesc
	, ISNULL(g.RefferenceDesc1, '''') Warna
	, ISNULL(c.SalesModelYear, 0) Tahun
	, a.ChassisNo
	, ISNULL(c.EngineNo, 0) EngineNo
	, ((CASE ISNULL(a.DealerCategory, '''') WHEN ''M'' THEN ''Main Dealer'' WHEN ''S'' THEN ''Sub Dealer'' WHEN ''R'' THEN ''Show Room'' END) + '' / '' + h.CustomerName) AS  Penjual
	, a.SalesmanName
	, a.SKPKName
	, a.SKPKAddress1 Alamat1
	, a.SKPKAddress2 Alamat2
	, a.SKPKAddress3 Alamat3
	, ISNULL(i.LookUpValueName, '''') City
	, a.SKPKTelp1
	, a.SKPKTelp2
	, a.SKPKHP
	, ISNULL(a.SKPKBirthday, '''') SKPKDay
	, a.FakturPolisiName
	, a.FakturPolisiAddress1
	, a.FakturPolisiAddress2
	, a.FakturPolisiAddress3
	, a.FakturPolisiTelp1
	, a.FakturPolisiTelp2
	, a.FakturPolisiHP
	, a.FakturPolisiBirthday
	, (select ISNULL(LookUpValueName, '''') from gnMstLookUpDtl where CompanyCode=a.CompanyCode and CodeID=''FPCT'' and LookUpValue=a.DealerCategory
		) AS DealerCategory
	, ISNULL(b.Remark, '''') Remark
	, ISNULL(UPPER(z.SignName), '''') AS SignName1
	, ISNULL(UPPER(z.TitleSign), '''') AS TitleSign1 
	, a.IDNo
FROM
 omTrSalesReqDetail a
JOIN
 omTrSalesReq b ON b.CompanyCode=a.CompanyCode AND b.BranchCode=a.BranchCode
 AND b.ReqNo=a.ReqNo 
LEFT JOIN
 ' + @DBMD + '..omMstVehicle c ON c.CompanyCode=''' + @CompanyMD + ''' 
 AND c.ChassisCode=a.ChassisCode
 AND c.ChassisNo=a.ChassisNo
LEFT JOIN
 gnMstCoProfile d ON d.CompanyCode=a.CompanyCode AND d.BranchCode=a.BranchCode
LEFT JOIN
 ' + @DBMD + '..omMstModel f ON f.CompanyCode=''' + @CompanyMD + ''' 
 AND f.SalesModelCode=c.SalesModelCode
LEFT JOIN
 ' + @DBMD + '..omMstRefference g ON g.CompanyCode=''' + @CompanyMD + '''
  AND g.RefferenceType=''COLO''
 AND g.RefferenceCode=c.ColourCode
LEFT JOIN
 gnMstCustomer h ON h.CompanyCode=b.CompanyCode AND h.CustomerCode=b.SubDealerCode
LEFT JOIN
 gnMstLookUpDtl i ON i.CompanyCode=a.CompanyCode AND i.CodeID=''CITY'' 
 AND i.LookUpValue=a.SKPKCity
LEFT JOIN gnMstSIgnature z
	ON z.CompanyCode = a.CompanyCode
	AND z.BranchCode = a.BranchCode
	AND z.ProfitCenterCode = ''100''
	AND z.DocumentType = ''RFP''
	AND z.SeqNo = 1
WHERE
 a.CompanyCode	  = ''' + @CompanyCode + '''
 AND a.BranchCode = ''' + @BranchCode + '''
 AND a.ReqNo BETWEEN ''' + @ReqNoA + ''' AND ''' + @ReqNoB + '''
ORDER BY ReqNo'

Exec (@QRYTmp);

END
--------------------------------------------------- BATAS ----------------------------------------------------------


go
ALTER procedure [dbo].[uspfn_SvTrnServiceSelectDtl]
--declare
	@CompanyCode varchar(15),
	@BranchCode  varchar(15),
	@ProductType varchar(15),
	@ServiceNo   bigint
as      

begin
--select @CompanyCode='6006400001',	@BranchCode='6006400104', @ProductType='4W', @ServiceNo=58463

declare @t1 as table
(
 TaskPartSeq int
,BillType varchar(10)
,BillTypeDesc varchar(max)
,TypeOfGoods varchar(10)
,TypeOfGoodsDesc varchar(70)
,TaskPartNo varchar(50)
,OprHourDemandQty numeric(18,2)
,SupplyQty numeric(18,2)
,ReturnQty numeric(18,2)
,OprRetailPrice numeric(18,2)
,OprRetailPriceTotal numeric(18,2)
,SupplySlipNo varchar(20)
,TaskPartDesc varchar(max)
,BasicModel varchar(15)
,JobType varchar(15)
,IsSubCon bit
,Status varchar(10)
,GTGO varchar(10)
,DiscPct numeric(18,2)
,QtyAvail numeric(18,2)
,TaskStatus varchar(50)
)

declare @JobOrderNo varchar(15)
    set @JobOrderNo = isnull((select JobOrderNo from svTrnService where CompanyCode = @CompanyCode and BranchCode = @BranchCode and ProductType = @ProductType and ServiceNo = @ServiceNo), '')

insert into @t1
select 0 TaskSeq 
      ,a.BillType
      ,b.Description BillTypeDesc
      ,a.TypeOfGoods
      ,case a.TypeOfGoods when 'L' then 'Labor (Jasa)' end TypeOfGoodsDesc
      ,a.OperationNo
      ,a.OperationHour
      ,0 OperationHourSupply
      ,0 OperationHourReturn
      ,a.OperationCost
      ,a.OperationHour * a.OperationCost * (100 - a.DiscPct) * 0.01 OprRetailPriceTotal
      ,'' SupplySlipNo
      ,rtrim(d.Description) OperationDesc 
	  ,c.BasicModel
	  ,c.JobType
	  ,a.IsSubCon
	  ,(select min(MechanicStatus) from svTrnSrvMechanic 
		where CompanyCode = a.CompanyCode 
			and BranchCode = a.BranchCode
			and ProductType = a.ProductType
			and ServiceNo = a.ServiceNo
			and OperationNo = a.OperationNo) MechanicStatus
	  ,''
	  ,a.DiscPct
	  ,0
      ,case(a.TaskStatus)
          when 0 then 'Open Task'
          when 1 then 'Work In Progress'
          when 2 then 'Close Task'
          when 9 then 'Cancel'
       end TaskStatus
  from svTrnSrvTask a with (nolock,nowait)
  left join svMstBillingType b with (nolock,nowait)
    on b.CompanyCode = a.CompanyCode
   and b.BillType = a.BillType
  left join svTrnService c with (nolock,nowait)
    on c.CompanyCode = a.CompanyCode
   and c.BranchCode = a.BranchCode
   and c.ProductType = a.ProductType
   and c.ServiceNo = a.ServiceNo
  left join svMstTask d with (nolock,nowait)
    on d.CompanyCode = a.CompanyCode
   and d.ProductType = a.ProductType
   and d.BasicModel = c.BasicModel
   and (d.JobType = c.JobType or d.JobType = 'CLAIM' or d.JobType = 'OTHER')
   and d.OperationNo = a.OperationNo 
 where 1 = 1
   and a.CompanyCode = @CompanyCode
   and a.BranchCode  = @BranchCode
   and a.ProductType = @ProductType
   and a.ServiceNo   = @ServiceNo

declare @tblTemp as table
(
	PartNo  varchar(20),
	QtyAvail numeric(18,2)
)

declare @DealerCode as varchar(2)
declare @CompanyMD as varchar(15)
declare @BranchMD as varchar(15)

set @DealerCode = 'MD'
set @CompanyMD = (select CompanyMD from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)
set @BranchMD = (select BranchMD from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)

if (@CompanyCode != @CompanyMD and @BranchCode != @BranchMD)
begin	
	set @DealerCode = 'SD'
	declare @DbName as varchar(50)
	set @DbName = (select DbMD from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)
	
	declare @QueryTemp as varchar(max)
	
	set @QueryTemp = 'select 
			 distinct
			 a.PartNo
			 , (b.OnHand - (b.AllocationSP + b.AllocationSR + b.AllocationSL) - (b.ReservedSP + b.ReservedSR + b.ReservedSL)) 
		 from svTrnSrvItem a	 
		 left join ' + @DbName + '..spMstItems b on 
			a.PartNo = b.PartNo 
			and b.CompanyCode = ''' + @CompanyMD + '''
			and b.BranchCode = ''' + @BranchMD + '''
		 where a.CompanyCode = ''' + @CompanyCode + '''
		   and a.BranchCode  = ''' + @BranchCode + '''
		   and a.ProductType = ''' + @ProductType + '''
		   and a.ServiceNo   = ' + convert(varchar,@ServiceNo) + ''		   
				
		print(@QueryTemp)		
		insert into @tblTemp		
		exec (@QueryTemp)		
end

insert into @t1
select a.PartSeq
      ,a.BillType
      ,b.Description BillTypeDesc
      ,a.TypeOfGoods
      ,rtrim(c.LookupValueName) + case lower(g.ParaValue) when 'sparepart' then ' (SPR)' else ' (MTR)' end TypeOfGoodsDesc
      ,a.PartNo
      ,a.DemandQty
      ,a.SupplyQty
      ,a.ReturnQty
      ,a.RetailPrice
      ,(case isnull(a.SupplyQty, 0)
         when 0 then (isnull(a.DemandQty, 0) * isnull(a.RetailPrice, 0))
         else ((isnull(a.SupplyQty, 0) - isnull(a.ReturnQty, 0)) * isnull(a.RetailPrice, 0))
        end) * (100.0 - a.DiscPct) * 0.01
        as RetailPriceTotal
      ,a.SupplySlipNo
      ,rtrim(d.PartName) OperationDesc 
	  ,''
	  ,''
	  ,0
	  ,''
	  ,g.ParaValue
	  ,a.DiscPct
	  ,case when @DealerCode = 'MD' then (i.OnHand - (i.AllocationSP + i.AllocationSR + i.AllocationSL) - (i.ReservedSP + i.ReservedSR + i.ReservedSL)) else e.QtyAvail end QtyAvail
	  ,''
  from svTrnSrvItem a with (nolock,nowait)
  left join svMstBillingType b with (nolock,nowait)
    on b.CompanyCode = a.CompanyCode
   and b.BillType = a.BillType
  left join gnMstLookupDtl c with (nolock,nowait)
    on c.CompanyCode = a.CompanyCode
   and c.CodeID = 'TPGO'
   and c.LookupValue = TypeOfGoods
  left join spMstItemInfo d with (nolock,nowait)
    on d.CompanyCode = a.CompanyCode
   and d.PartNo = a.PartNo
  left join gnMstLookupDtl g with (nolock,nowait)
    on g.CompanyCode = a.CompanyCode
   and g.CodeID = 'GTGO'
   and g.LookupValue = TypeOfGoods
  left join svTrnService s with (nolock,nowait)
    on s.CompanyCode = a.CompanyCode
   and s.BranchCode = a.BranchCode
   and s.ServiceNo = a.ServiceNo
  left join spMstItems i
    on i.CompanyCode = a.CompanyCode 
   and i.BranchCode = a.BranchCode
   and i.ProductType = a.ProductType
   and i.PartNo = a.PartNo
   left join @tblTemp e
    on e.PartNo = a.PartNo
 where 1 = 1
   and a.CompanyCode = @CompanyCode
   and a.BranchCode  = @BranchCode
   and a.ProductType = @ProductType
   and a.ServiceNo   = @ServiceNo

select * into #t1 from (
select 
 a.* 
,P01 = isnull((
	select count(*) from spTrnSORDDtl with(nowait,nolock)
	 where 1 = 1
	   and CompanyCode = @CompanyCode
	   and BranchCode  = @BranchCode
	   and PartNo      = a.TaskPartNo
	   and DocNo in (select aa.DocNo
					   from spTrnSORDHdr aa with(nowait,nolock), svTrnService bb with(nowait,nolock)
					  where 1 = 1
						and bb.CompanyCode = aa.CompanyCode
						and bb.BranchCode  = aa.BranchCode
						and bb.JobOrderNo  = aa.UsageDocNo
						and isnull(bb.JobOrderNo, '') <> ''
						and aa.CompanyCode = @CompanyCode
						and aa.BranchCode  = @BranchCode
						and bb.ServiceNo   = @ServiceNo
					 )
	),0)
,P02 = isnull((
	select count(DocNo) from spTrnSOSupply with(nowait,nolock)
	 where 1 = 1
	   and CompanyCode = @CompanyCode
	   and BranchCode  = @BranchCode
	   and PartNo = a.TaskPartNo
	   and DocNo  = a.SupplySlipNo
	),0)
,P03 = isnull((
	select count(bb.PickingSlipNo) from spTrnSOSupply aa with(nowait,nolock)
	  left join spTrnSPickingHdr bb with(nowait,nolock) on 1 = 1
	   and bb.CompanyCode   = aa.CompanyCode
	   and bb.BranchCode    = aa.BranchCode
	   and bb.PickingSlipNo = aa.PickingSlipNo
	 where 1 = 1
	   and aa.CompanyCode = @CompanyCode
	   and aa.BranchCode  = @BranchCode
	   and aa.PartNo = a.TaskPartNo
	   and aa.DocNo  = a.SupplySlipNo
	   and isnull(aa.PickingSlipNo, '') <> ''
	),0)
,P04 = isnull((
	select count(bb.PickingSlipNo) from spTrnSOSupply aa with(nowait,nolock)
	  left join spTrnSPickingHdr bb with(nowait,nolock) on 1 = 1
	   and bb.CompanyCode   = aa.CompanyCode
	   and bb.BranchCode    = aa.BranchCode
	   and bb.PickingSlipNo = aa.PickingSlipNo
	   and bb.Status >= '2'
	 where 1 = 1
	   and aa.CompanyCode = @CompanyCode
	   and aa.BranchCode  = @BranchCode
	   and aa.PartNo = a.TaskPartNo
	   and aa.DocNo  = a.SupplySlipNo
	   and isnull(aa.PickingSlipNo, '') <> ''
	),0)
,P05 = isnull((
	select count(bb.PickingSlipNo) from spTrnSOSupply aa with(nowait,nolock)
	  left join spTrnSLmpHdr bb with(nowait,nolock) on 1 = 1
	   and bb.CompanyCode   = aa.CompanyCode
	   and bb.BranchCode    = aa.BranchCode
	   and bb.PickingSlipNo = aa.PickingSlipNo
	 where 1 = 1
	   and aa.CompanyCode = @CompanyCode
	   and aa.BranchCode  = @BranchCode
	   and aa.PartNo = a.TaskPartNo
	   and aa.DocNo  = a.SupplySlipNo
	   and isnull(aa.PickingSlipNo, '') <> ''
	),0)
,P06 = isnull((
	select count(bb.PickingSlipNo) from spTrnSOSupply aa with(nowait,nolock)
	  left join spTrnSLmpHdr bb with(nowait,nolock) on 1 = 1
	   and bb.CompanyCode   = aa.CompanyCode
	   and bb.BranchCode    = aa.BranchCode
	   and bb.PickingSlipNo = aa.PickingSlipNo
	   and bb.Status >= '1'
	 where 1 = 1
	   and aa.CompanyCode = @CompanyCode
	   and aa.BranchCode  = @BranchCode
	   and aa.PartNo = a.TaskPartNo
	   and aa.DocNo  = a.SupplySlipNo
	   and isnull(aa.PickingSlipNo, '') <> ''
	),0)
,S01 = isnull((
	select count(bb.PONo) from svTrnPoSubCon aa with(nowait,nolock)
	  left join svTrnPoSubConTask bb with(nowait,nolock) on 1 = 1
	   and bb.CompanyCode   = aa.CompanyCode
	   and bb.BranchCode    = aa.BranchCode
	   and bb.ProductType   = aa.ProductType
	   and bb.PONo          = aa.PONo
	 where 1 = 1
	   and aa.CompanyCode = @CompanyCode
	   and aa.BranchCode  = @BranchCode
	   and aa.ProductType = @ProductType
	   and aa.JobOrderNo  = @JobOrderNo
	   and aa.JobOrderNo <> ''
	   and aa.BasicModel  = a.BasicModel
	   and aa.JobType     = a.JobType
	   and bb.OperationNo = a.TaskPartNo
	),0)
,S02 = isnull((
	select count(bb.PONo) from svTrnPoSubCon aa with(nowait,nolock)
	  left join svTrnPoSubConTask bb with(nowait,nolock) on 1 = 1
	   and bb.CompanyCode   = aa.CompanyCode
	   and bb.BranchCode    = aa.BranchCode
	   and bb.ProductType   = aa.ProductType
	   and bb.PONo          = aa.PONo
	 where 1 = 1
	   and aa.CompanyCode = @CompanyCode
	   and aa.BranchCode  = @BranchCode
	   and aa.ProductType = @ProductType
	   and aa.JobOrderNo  = @JobOrderNo
	   and aa.JobOrderNo <> ''
	   and aa.BasicModel  = a.BasicModel
	   and aa.JobType     = a.JobType
	   and bb.OperationNo = a.TaskPartNo
	   and aa.POStatus >= '2'
	),0)
,S03 = isnull((
	select count(bb.PONo) from svTrnPoSubCon aa with(nowait,nolock)
	  left join svTrnPoSubConTask bb with(nowait,nolock) on 1 = 1
	   and bb.CompanyCode   = aa.CompanyCode
	   and bb.BranchCode    = aa.BranchCode
	   and bb.ProductType   = aa.ProductType
	   and bb.PONo          = aa.PONo
	 where 1 = 1
	   and aa.CompanyCode = @CompanyCode
	   and aa.BranchCode  = @BranchCode
	   and aa.ProductType = @ProductType
	   and aa.JobOrderNo  = @JobOrderNo
	   and aa.JobOrderNo <> ''
	   and aa.BasicModel  = a.BasicModel
	   and aa.JobType     = a.JobType
	   and bb.OperationNo = a.TaskPartNo
	   and aa.POStatus >= '3'
	),0)
,S04 = isnull((
	select count(bb.PONo) from svTrnPoSubCon aa with(nowait,nolock)
	  left join svTrnPoSubConTask bb with(nowait,nolock) on 1 = 1
	   and bb.CompanyCode   = aa.CompanyCode
	   and bb.BranchCode    = aa.BranchCode
	   and bb.ProductType   = aa.ProductType
	   and bb.PONo          = aa.PONo
	 where 1 = 1
	   and aa.CompanyCode = @CompanyCode
	   and aa.BranchCode  = @BranchCode
	   and aa.ProductType = @ProductType
	   and aa.JobOrderNo  = @JobOrderNo
	   and aa.JobOrderNo <> ''
	   and aa.BasicModel  = a.BasicModel
	   and aa.JobType     = a.JobType
	   and bb.OperationNo = a.TaskPartNo
	   and aa.POStatus >= '4'
	),0)
,S05 = isnull((
	select count(bb.PONo) from svTrnPoSubCon aa with(nowait,nolock)
	  left join svTrnPoSubConTask bb with(nowait,nolock) on 1 = 1
	   and bb.CompanyCode   = aa.CompanyCode
	   and bb.BranchCode    = aa.BranchCode
	   and bb.ProductType   = aa.ProductType
	   and bb.PONo          = aa.PONo
	 where 1 = 1
	   and aa.CompanyCode = @CompanyCode
	   and aa.BranchCode  = @BranchCode
	   and aa.ProductType = @ProductType
	   and aa.JobOrderNo  = @JobOrderNo
	   and aa.JobOrderNo <> ''
	   and aa.BasicModel  = a.BasicModel
	   and aa.JobType     = a.JobType
	   and bb.OperationNo = a.TaskPartNo
	   and aa.POStatus >= '5'
	),0)
from @t1 a
)#t1

update #t1
   set Status = (case P01 when 0 then 0 else 1 end)
			  + (case P02 when 0 then 0 else 1 end)
			  + (case P03 when 0 then 0 else 1 end)
			  + (case P04 when 0 then 0 else 1 end)
			  + (case P05 when 0 then 0 else 1 end)
			  + (case P06 when 0 then 0 else 1 end)
 where TypeOfGoods <> 'L'

update #t1
   set Status = (case S01 when 0 then 0 else 1 end)
			  + (case S02 when 0 then 0 else 1 end)
			  + (case S03 when 0 then 0 else 1 end)
			  + (case S04 when 0 then 0 else 1 end)
			  + (case S05 when 0 then 0 else 1 end)
 where TypeOfGoods = 'L' and IsSubCon = '1'

select
 row_number() over (order by TaskPartSeq) SeqNo
,TaskPartSeq
,BillType
,BillTypeDesc
,TypeOfGoods
,TypeOfGoodsDesc
,case isnull(TypeOfGoods, '') when 'L' then 'L' else '0' end ItemType
,TaskPartNo
,OprHourDemandQty
,SupplyQty
,ReturnQty
,OprRetailPrice
,OprRetailPriceTotal
,isnull(SupplySlipNo, '')SupplySlipNo
,TaskPartDesc
,ISNULL(Status, '') Status
,StatusDesc = 
 ISNULL(case IsSubCon
	when 0 then
		 case TypeOfGoods 
			when 'L' then
				case Status
					when '0' then '0 - Open Task'
					when '1' then '1 - Work In Progress'
					when '2' then '2 - Finish Task'
				end
			else
				case Status
					when '1' then '1 - Entry Stock'
					when '2' then '2 - Alokasi Stock'
					when '3' then '3 - Generate PL'
					when '4' then '4 - Generate Bill'
					when '5' then '5 - Lampiran'
					when '6' then '6 - Print Lampiran'
				end
		 end	
	else
		case Status
			when '1' then '1 - Draft PO'
			when '2' then '2 - Generate PO'
			when '3' then '3 - Draft Receiving'
			when '4' then '4 - Cancel Receiving'
			when '5' then '5 - Receiving PO'
		end
 end, '') 
,QtyAvail
,(case when (SupplyQty > 0) then (SupplyQty - ReturnQty) else OprHourDemandQty end) * OprRetailPrice Price
,DiscPct
,OprRetailPriceTotal as PriceNet
,IsSubCon
,TaskStatus
,@ServiceNo ServiceNo
from #t1

drop table #t1

end

go

ALTER PROCEDURE [dbo].[uspfn_GetCostPrice] (@CompanyCode varchar(15), @BranchCode varchar(15), 
	@PartNo varchar(20), @CostPrice numeric(18,2) output)
AS
BEGIN
	DECLARE @Discount NUMERIC(18,2)
	DECLARE @PurcDiscPct numeric(18,2)
	DECLARE @DiscPct numeric(18,2)

	set @Discount = 0;
    set @PurcDiscPct = (SELECT PurcDiscPct FROM spMstItems WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND PartNo = @PartNo)

	if(@PurcDiscPct is not null) begin
		--discount from master items
		SET @Discount = @PurcDiscPct
	end
	else begin
        --discount from master supplier
        set @DiscPct = (select DiscPct from gnMstSupplierProfitCenter where CompanyCode = @CompanyCode and BranchCode = @BranchCode
			and SupplierCode = (select dbo.GetBranchMD(@CompanyCode, @BranchCode)) and ProfitCenterCode = '300')
		
		if(@DiscPct is not null)begin
			if(@DiscPct >= 0) begin
				SET @Discount = @Discount + @DiscPct;
			end
		end 
	end
	
	--declare @xCostPrice numeric(18,2)
	declare @sql nvarchar(512);
	set @sql = 'select @CostPrice = (b.RetailPrice - (b.RetailPrice * (' + convert(varchar, @Discount) + ' *  0.01)))
		from ' + dbo.GetDbMD(@CompanyCode, @BranchCode) + '..spMstItemPrice b 
		where b.CompanyCode =''' + dbo.GetCompanyMD(@CompanyCode, @BranchCode) + '''
		and b.BranchCode =''' + dbo.GetBranchMD(@CompanyCode, @BranchCode) + '''
		and b.PartNo =''' + @PartNo + ''''


		print @sql

	execute sp_executesql @sql, N'@CostPrice numeric(18,2) OUTPUT', @CostPrice = @CostPrice OUTPUT
END
go
-- =============================================
-- Author:		SDMS.RUDIANA
-- Create date: 2015-06-05
-- Description:	Get CostPrice Value
-- =============================================
ALTER PROCEDURE [dbo].[uspfn_GetCostPrice] (@CompanyCode varchar(15), @BranchCode varchar(15), 
	@PartNo varchar(20), @CostPrice numeric(18,2) output)
AS
BEGIN
	DECLARE @Discount NUMERIC(18,2)
	DECLARE @PurcDiscPct numeric(18,2)
	DECLARE @DiscPct numeric(18,2)

	set @Discount = 0;
    set @PurcDiscPct = (SELECT PurcDiscPct FROM spMstItems WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND PartNo = @PartNo)

	if(@PurcDiscPct is not null) begin
		--discount from master items
		SET @Discount = @PurcDiscPct
	end
	else begin
        --discount from master supplier
        set @DiscPct = (select DiscPct from gnMstSupplierProfitCenter where CompanyCode = @CompanyCode and BranchCode = @BranchCode
			and SupplierCode = (select dbo.GetBranchMD(@CompanyCode, @BranchCode)) and ProfitCenterCode = '300')
		
		if(@DiscPct is not null)begin
			if(@DiscPct >= 0) begin
				SET @Discount = @Discount + @DiscPct;
			end
		end 
	end
	
	--declare @xCostPrice numeric(18,2)
	declare @sql nvarchar(512);
	set @sql = 'select @CostPrice = (b.RetailPrice - (b.RetailPrice * (' + convert(varchar, @Discount) + ' *  0.01)))
		from ' + dbo.GetDbMD(@CompanyCode, @BranchCode) + '..spMstItemPrice b 
		where b.CompanyCode =''' + dbo.GetCompanyMD(@CompanyCode, @BranchCode) + '''
		and b.BranchCode =''' + dbo.GetBranchMD(@CompanyCode, @BranchCode) + '''
		and b.PartNo =''' + @PartNo + ''''


		print @sql

	execute sp_executesql @sql, N'@CostPrice numeric(18,2) OUTPUT', @CostPrice = @CostPrice OUTPUT
END
go
Create procedure [dbo].[uspfn_SelectPartsAcc]
	@CompanyCode varchar(25),
	@BranchCode varchar(25)
as

begin
	declare @CodeID varchar(25), @sqlstr varchar(max),@DBName varchar(75), @CompanySD varchar(75), @BranchSD varchar(75) ;
	set @CodeID = 'TPGO';
    set @DBName = (select DbName from gnMstCompanyMapping where CompanyMD=@CompanyCode and BranchMD=@BranchCode)
	set @CompanySD = (select CompanyCode from gnMstCompanyMapping where CompanyMD=@CompanyCode and BranchMD=@BranchCode)
	set @BranchSD = (select BranchCode from gnMstCompanyMapping where CompanyMD=@CompanyCode and BranchMD=@BranchCode)
	set  @sqlstr = '
					SELECT itemInfo.PartNo
                    , (item.OnHand - (item.AllocationSP + item.AllocationSR + item.AllocationSL)- (item.ReservedSP + item.ReservedSR + item.ReservedSL))  AS Available
                    , itemPrice.RetailPriceInclTax
                    , itemInfo.PartName
                    , (CASE itemInfo.Status
                        WHEN 1 THEN ''Aktif'' ELSE ''Tidak Aktif''
                       END)  AS Status
                    , dtl.LookUpValueName as JenisPart
                    , itemPrice.RetailPrice  AS NilaiPart
                FROM  spMstItemInfo itemInfo                    
                LEFT JOIN spMstItems item ON item.CompanyCode = itemInfo.CompanyCode
                    AND item.BranchCode = '''+ @BranchCode +'''
                    AND item.PartNo = itemInfo.PartNo
                LEFT JOIN '+@DBName+'..spMstItemPrice itemPrice ON itemPrice.CompanyCode = '''+ @CompanySD + '''
                    AND itemPrice.BranchCode = '''+ @BranchSD +'''
                    AND itemPrice.PartNo = item.PartNo
                LEFT JOIN GnMstLookUpDtl dtl ON item.companyCode = dtl.companyCode 
                    AND dtl.CodeId = '''+@CodeID +'''
                    AND dtl.LookUpValue = item.TypeOfGoods                    
                WHERE itemInfo.CompanyCode = '''+ @CompanyCode +'''
                    AND itemInfo.Status = ''1''
                    AND (item.OnHand - (item.AllocationSP + item.AllocationSR + item.AllocationSL)
                        - (item.ReservedSP + item.ReservedSR + item.ReservedSL)) > 0 
				order by  item.TypeOfGoods desc
			'
print @sqlstr
exec(@sqlstr)
end
go

INSERT INTO sysMenuDms 
VALUES('omSlsPrmhhnFPSD', 'Permohonan Faktur Polisi SD', 'omSls', 8, 2, 'sales/permohonanfpsd', NULL)

INSERT INTO sysRoleMenu
VALUES('ADMIN', 'omSlsPrmhhnFPSD')
go

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[uspfn_SpUpdateMovingCode]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[uspfn_SpUpdateMovingCode]
GO

CREATE PROCEDURE uspfn_SpUpdateMovingCode
--declare 
@CompanyCode as varchar(50),
@BranchCode as varchar(50),
@TransDate as datetime

--set @TransDate = GETDATE()
--set @CompanyCode = '6115204001'
--set @BranchCode = '6115204101'

as
begin

select * into #t1 from (
select 
 a.PartNo
,a.DemandFreq
,a.DemandQty
,convert(varchar,a.Year) + right('0' + convert(varchar,a.Month),2) as date0
,convert(varchar(6), dateadd(m,-11,@TransDate), 112) date1
,convert(varchar(6), dateadd(m,-10,@TransDate), 112) date2
,convert(varchar(6), dateadd(m,-9,@TransDate), 112) date3
,convert(varchar(6), dateadd(m,-8,@TransDate), 112) date4
,convert(varchar(6), dateadd(m,-7,@TransDate), 112) date5
,convert(varchar(6), dateadd(m,-6,@TransDate), 112) date6

,convert(varchar(6), dateadd(m,-5,@TransDate), 112) date7
,convert(varchar(6), dateadd(m,-4,@TransDate), 112) date8
,convert(varchar(6), dateadd(m,-3,@TransDate), 112) date9
,convert(varchar(6), dateadd(m,-2,@TransDate), 112) date10
,convert(varchar(6), dateadd(m,-1,@TransDate), 112) date11
,convert(varchar(6), dateadd(m,-0,@TransDate), 112) date12

from spHstDemandItem a WITH(NOWAIT, NOLOCK) 
where a.CompanyCode=@CompanyCode and a.BranchCode=@BranchCode
 and convert(varchar,a.Year) + right('0' + convert(varchar,a.Month),2) >= convert(varchar(6), dateadd(m,-12,@TransDate), 112)
) #t1

select * into #t2 from (
select 
 a.PartNo
,a.DemandFreq
,case when (date0=date1) and a.DemandFreq>0 then 1 else 0 end as T1
,case when (date0=date2) and a.DemandFreq>0 then 1 else 0 end as T2
,case when (date0=date3) and a.DemandFreq>0 then 1 else 0 end as T3
,case when (date0=date4) and a.DemandFreq>0 then 1 else 0 end as T4
,case when (date0=date5) and a.DemandFreq>0 then 1 else 0 end as T5
,case when (date0=date6) and a.DemandFreq>0 then 1 else 0 end as T6

,case when (date0=date7) and a.DemandFreq>0 then 1 else 0 end as T7
,case when (date0=date8) and a.DemandFreq>0 then 1 else 0 end as T8
,case when (date0=date9) and a.DemandFreq>0 then 1 else 0 end as T9
,case when (date0=date10) and a.DemandFreq>0 then 1 else 0 end as T10
,case when (date0=date11) and a.DemandFreq>0 then 1 else 0 end as T11
,case when (date0=date12) and a.DemandFreq>0 then 1 else 0 end as T12
from #t1 a
) #t2

select * into #t3 from (
select
 a.PartNo
,case when (sum(T1)> 0) then 1 else 0 end as D1
,case when (sum(T2)> 0) then 1 else 0 end as D2
,case when (sum(T3)> 0) then 1 else 0 end as D3
,case when (sum(T4)> 0) then 1 else 0 end as D4
,case when (sum(T5)> 0) then 1 else 0 end as D5
,case when (sum(T6)> 0) then 1 else 0 end as D6

,case when (sum(T7)> 0) then 1 else 0 end as D7
,case when (sum(T8)> 0) then 1 else 0 end as D8
,case when (sum(T9)> 0) then 1 else 0 end as D9
,case when (sum(T10)> 0) then 1 else 0 end as D10
,case when (sum(T11)> 0) then 1 else 0 end as D11
,case when (sum(T12)> 0) then 1 else 0 end as D12
from #t2 a
group by a.PartNo
) #t3

select * into #t4 from (
select 
 a.PartNo
,b.NewPartNo
from #t3 a
left join spMstItemMod b WITH(NOWAIT, NOLOCK)
  on b.PartNo = a.PartNo and b.CompanyCode = @CompanyCode
where b.NewPartNo <> ''
) #t4

insert into #t3
select 
 NewPartNo as PartNo
,D1=0,D2=0,D3=0,D4=0,D5=0,D6=0
,D7=0,D8=0,D9=0,D10=0,D11=0,D12=0
from #t4
where NewPartNo not in (select PartNo from #t3)

select * into #t5 from(
select distinct PartNo, D1, D2, D3, D4, D5, D6, D7, D8, D9, D10, D11, D12 from #t3)#t5

select * into #t6 from (
select 
	PartNo
	, CASE WHEN ISNULL((SELECT D1 FROM #t5 WHERE PartnO = a.PartNo),0) + ISNULL((SELECT D1 FROM #t5 WHERE PartnO = a.NewPartNo),0) > 0 THEN 1 ELSE 0 END D1
	, CASE WHEN ISNULL((SELECT D2 FROM #t5 WHERE PartnO = a.PartNo),0) + ISNULL((SELECT D2 FROM #t5 WHERE PartnO = a.NewPartNo),0) > 0 THEN 1 ELSE 0 END D2
	, CASE WHEN ISNULL((SELECT D3 FROM #t5 WHERE PartnO = a.PartNo),0) + ISNULL((SELECT D3 FROM #t5 WHERE PartnO = a.NewPartNo),0) > 0 THEN 1 ELSE 0 END D3
	, CASE WHEN ISNULL((SELECT D4 FROM #t5 WHERE PartnO = a.PartNo),0) + ISNULL((SELECT D4 FROM #t5 WHERE PartnO = a.NewPartNo),0) > 0 THEN 1 ELSE 0 END D4
	, CASE WHEN ISNULL((SELECT D5 FROM #t5 WHERE PartnO = a.PartNo),0) + ISNULL((SELECT D5 FROM #t5 WHERE PartnO = a.NewPartNo),0) > 0 THEN 1 ELSE 0 END D5
	, CASE WHEN ISNULL((SELECT D6 FROM #t5 WHERE PartnO = a.PartNo),0) + ISNULL((SELECT D6 FROM #t5 WHERE PartnO = a.NewPartNo),0) > 0 THEN 1 ELSE 0 END D6

	, CASE WHEN ISNULL((SELECT D7 FROM #t5 WHERE PartnO = a.PartNo),0) + ISNULL((SELECT D7 FROM #t5 WHERE PartnO = a.NewPartNo),0) > 0 THEN 1 ELSE 0 END D7
	, CASE WHEN ISNULL((SELECT D8 FROM #t5 WHERE PartnO = a.PartNo),0) + ISNULL((SELECT D8 FROM #t5 WHERE PartnO = a.NewPartNo),0) > 0 THEN 1 ELSE 0 END D8
	, CASE WHEN ISNULL((SELECT D9 FROM #t5 WHERE PartnO = a.PartNo),0) + ISNULL((SELECT D9 FROM #t5 WHERE PartnO = a.NewPartNo),0) > 0 THEN 1 ELSE 0 END D9
	, CASE WHEN ISNULL((SELECT D10 FROM #t5 WHERE PartnO = a.PartNo),0) + ISNULL((SELECT D10 FROM #t5 WHERE PartnO = a.NewPartNo),0) > 0 THEN 1 ELSE 0 END D10
	, CASE WHEN ISNULL((SELECT D11 FROM #t5 WHERE PartnO = a.PartNo),0) + ISNULL((SELECT D11 FROM #t5 WHERE PartnO = a.NewPartNo),0) > 0 THEN 1 ELSE 0 END D11
	, CASE WHEN ISNULL((SELECT D12 FROM #t5 WHERE PartnO = a.PartNo),0) + ISNULL((SELECT D12 FROM #t5 WHERE PartnO = a.NewPartNo),0) > 0 THEN 1 ELSE 0 END D12
from #t4 a) #t6

update #t5
set D1 = b.D1
	, D2 = b.D2
	, D3 = b.D3
	, D4 = b.D4
	, D5 = b.D5
	, D6 = b.D6
	, D7 = b.D7
	, D8 = b.D8
	, D9 = b.D9
	, D10 = b.D10
	, D11 = b.D11
	, D12 = b.D12
from #t5 a, #t6 b
where a.partno = b.partno

select * into #t7 from (
select @CompanyCode CompanyCode, @BranchCode BranchCode, partno, D1 + D2 + D3  + D4 + D5 + D6 + D7 + D8 + D9 + D10 + D11 + D12 dTotal from #t5) #t7

update spMstItems 
set MovingCode = CASE WHEN b.dTotal = 0 THEN 5
					  WHEN b.dTotal >= 1 AND b.dTotal <= 3 THEN 4
					  WHEN b.dTotal >= 4 AND b.dTotal <= 8 THEN 3
					  WHEN b.dTotal >= 9 AND b.dTotal <= 11 THEN 2	
					  WHEN b.dTotal = 12 THEN 1
					  ELSE 0
				 END
from spMstItems a, #t7 b
where 
	a.CompanyCode = b.CompanyCode
	and a.branchcode = b.branchcode
	and a.partno = b.partno
	and (datediff(mm, a.BornDate, @transdate) + 1) >= 12 

-- SET MOVING CODE : 0 FOR ITEM THAT BORN DATE < 12 MONTHS
update spMstItems set MovingCode = 0
where CompanyCode = @CompanyCode
  and BranchCode = @BranchCode
  and (datediff(mm, BornDate, @TransDate) + 1) < 12

drop table #t7
drop table #t6
drop table #t5
drop table #t4
drop table #t3
drop table #t2
drop table #t1
end

GO
update spMstMovingCode
set Param1 = 12, Param2 = 12
where MovingCode = '1'

update spMstMovingCode
set Param1 = 9, Param2 = 11
where MovingCode = '2'

update spMstMovingCode
set Param1 = 4, Param2 = 8
where MovingCode = '3'

update spMstMovingCode
set Param1 = 1, Param2 = 3
where MovingCode = '4'
go

ALTER procedure [dbo].[uspfn_SvTrnInvoiceCancel]
	@CompanyCode   varchar(20),
	@BranchCode    varchar(20),
	@InvoiceNo     varchar(20),
	@UserInfo      varchar(max)
as  

declare @errmsg varchar(max)
declare @JobOrderNo varchar(20) 
declare @InvoiceDate DateTime

begin try
set nocount on
	set @JobOrderNo = (Select JobOrderNo from svTrnInvoice where CompanyCode = @CompanyCode and BranchCode = @BranchCode and InvoiceNo = @InvoiceNo)
	set @InvoiceDate = (Select InvoiceDate from svTrnInvoice where CompanyCode = @CompanyCode and BranchCode = @BranchCode and InvoiceNo = @InvoiceNo)

	declare @CompanyMD as varchar(15)
	declare @BranchMD as varchar(15)

	set @CompanyMD = (select CompanyMD from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)
	set @BranchMD = (select BranchMD from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)
	
	if (@CompanyCode != @CompanyMD and @BranchCode != @BranchMD)
	begin
		IF convert(varchar,@InvoiceDate,112) < convert(varchar,getdate(),112)
		begin
			raiserror('Tanggal invoice lebih kecil dari tanggal hari ini',16 ,1 );
		end
	end
	
	if exists (
	select * from ArInterface
	 where CompanyCode = @CompanyCode
	   and BranchCode  = @BranchCode
	   and (StatusFlag > '0' or ReceiveAmt > 0 or BlockAmt > 0 or DebetAmt > 0 or CreditAmt > 0)
	   and DocNo in (Select InvoiceNo from svTrnInvoice where CompanyCode = @CompanyCode and BranchCode = @BranchCode and JobOrderNo = @JobOrderNo)
	)
	begin
		raiserror('Invoice sudah ada proses Receiving, transaksi tidak bisa dilanjutkan',16 ,1 );
	end

	if exists (
	select * from svTrnFakturPajak
	 where CompanyCode = @CompanyCode
	   and BranchCode = @BranchCode
	   and FPJNo in (select FPJNo from svTrnInvoice where CompanyCode = @CompanyCode and BranchCode = @BranchCode and JobOrderNo = @JobOrderNo)
	   and isnull(FPJGovNo, '') <> ''
	   and left(FPJGovNo, 3) <> 'SHN'
	)
	begin
		raiserror('Invoice sudah tergenerate Nomor Pajak Pemerintah, transaksi tidak bisa dilanjutkan',16 ,1 );
	end

	;with x as (
		select a.BranchCode, a.CustomerCode, a.SalesAmt, b.TotalSrvAmt
		  from gnTrnBankBook a, svTrnInvoice b
		 where 1 = 1
		   and b.CompanyCode = a.CompanyCode
		   and b.BranchCode = a.BranchCode
		   and b.CustomerCodeBill = a.CustomerCode
		   and a.ProfitCenterCode = '200'
		   and a.CompanyCode = @BranchCode
		   and a.BranchCode = @BranchCode
		   and b.JobOrderNo = @JobOrderNo
	)
	update x set SalesAmt = SalesAmt - TotalSrvAmt where SalesAmt > 0

	delete from glInterface 
	 where CompanyCode = @CompanyCode
	   and BranchCode = @BranchCode
	   and DocNo in (Select InvoiceNo from svTrnInvoice where CompanyCode = @CompanyCode and BranchCode = @BranchCode and JobOrderNo = @JobOrderNo)
	delete from arInterface 
	 where CompanyCode = @CompanyCode
	   and BranchCode = @BranchCode
	   and DocNo in (Select InvoiceNo from svTrnInvoice where CompanyCode = @CompanyCode and BranchCode = @BranchCode and JobOrderNo = @JobOrderNo)

	update svTrnService set ServiceStatus = 5, InvoiceNo = ''
	 where CompanyCode = @CompanyCode
	   and BranchCode = @BranchCode
	   and JobOrderNo = @JobOrderNo
		
	delete svTrnFakturPajak
	 where CompanyCode = @CompanyCode
	   and BranchCode = @BranchCode
	   and FPJNo in (select FPJNo from svTrnInvoice where CompanyCode = @CompanyCode and BranchCode = @BranchCode and JobOrderNo = @JobOrderNo)

	-------------------------------------------------------------------------------------------------------------------
	-- Insert into table log
	-------------------------------------------------------------------------------------------------------------------
	declare @TransID   uniqueidentifier; 
	declare @TransCode varchar(20);

	set @TransID = newid()
	set @TransCode = 'CANCEL INVOICE' 

	insert into svTrnInvoiceLog (
		TransID,TransCode,CompanyCode,BranchCode,ProductType,InvoiceNo,InvoiceDate,InvoiceStatus,
		FPJNo,FPJDate,JobOrderNo,JobOrderDate,JobType,ServiceRequestDesc,ChassisCode,ChassisNo,
		EngineCode,EngineNo,PoliceRegNo,BasicModel,CustomerCode,CustomerCodeBill,Odometer,
		IsPKP,TOPCode,TOPDays,DueDate,SignedDate,LaborDiscPct,PartsDiscPct,MaterialDiscPct,
		PphPct,PpnPct,LaborGrossAmt,PartsGrossAmt,MaterialGrossAmt,LaborDiscAmt,PartsDiscAmt,MaterialDiscAmt,
		LaborDppAmt,PartsDppAmt,MaterialDppAmt,TotalDppAmt,TotalPphAmt,TotalPpnAmt,TotalSrvAmt,
		Remarks,PrintSeq,PostingFlag,PostingDate,CreatedBy,CreatedDate
	) 
	select 
		@TransID, @TransCode,CompanyCode,BranchCode,ProductType,InvoiceNo,InvoiceDate,InvoiceStatus,
		FPJNo,FPJDate,JobOrderNo,JobOrderDate,JobType,left(ServiceRequestDesc, 200),ChassisCode,ChassisNo,
		EngineCode,EngineNo,PoliceRegNo,BasicModel,CustomerCode,CustomerCodeBill,Odometer,
		IsPKP,TOPCode,TOPDays,DueDate,SignedDate,LaborDiscPct,PartsDiscPct,MaterialDiscPct,
		PphPct,PpnPct,LaborGrossAmt,PartsGrossAmt,MaterialGrossAmt,LaborDiscAmt,PartsDiscAmt,MaterialDiscAmt,
		LaborDppAmt,PartsDppAmt,MaterialDppAmt,TotalDppAmt,TotalPphAmt,TotalPpnAmt,TotalSrvAmt,
		Remarks,PrintSeq,PostingFlag,PostingDate,@UserInfo,CreatedDate
	from svTrnInvoice
	 where CompanyCode = @CompanyCode and BranchCode = @BranchCode and InvoiceNo in (Select InvoiceNo from svTrnInvoice where CompanyCode = @CompanyCode and BranchCode = @BranchCode and JobOrderNo = @JobOrderNo)

	insert into svTrnInvTaskLog(TransID,TransCode,CompanyCode,BranchCode,ProductType,InvoiceNo,OperationNo,OperationHour,ClaimHour,OperationCost,SubConPrice,IsSubCon,SharingTask)
	select @TransID,@TransCode,CompanyCode,BranchCode,ProductType,InvoiceNo,OperationNo,OperationHour,ClaimHour,OperationCost,SubConPrice,IsSubCon,SharingTask from svTrnInvTask 
	 where CompanyCode = @CompanyCode and BranchCode = @BranchCode and InvoiceNo in (Select InvoiceNo from svTrnInvoice where CompanyCode = @CompanyCode and BranchCode = @BranchCode and JobOrderNo = @JobOrderNo)

	insert into svTrnInvItemLog(TransID,TransCode,CompanyCode, BranchCode, ProductType, InvoiceNo, PartNo, MovingCode, ABCClass, SupplyQty, ReturnQty, CostPrice, RetailPrice, TypeOfGoods)
	select @TransID,@TransCode,CompanyCode, BranchCode, ProductType, InvoiceNo, PartNo, MovingCode, ABCClass, SupplyQty, ReturnQty, CostPrice, RetailPrice, TypeOfGoods from svTrnInvItem
	 where CompanyCode = @CompanyCode and BranchCode = @BranchCode and InvoiceNo in (Select InvoiceNo from svTrnInvoice where CompanyCode = @CompanyCode and BranchCode = @BranchCode and JobOrderNo = @JobOrderNo)

	delete svTrnInvItemDtl where CompanyCode = @CompanyCode and BranchCode = @BranchCode and InvoiceNo in (Select InvoiceNo from svTrnInvoice where CompanyCode = @CompanyCode and BranchCode = @BranchCode and JobOrderNo = @JobOrderNo)
	delete svTrnInvItem where CompanyCode = @CompanyCode and BranchCode = @BranchCode and InvoiceNo in (Select InvoiceNo from svTrnInvoice where CompanyCode = @CompanyCode and BranchCode = @BranchCode and JobOrderNo = @JobOrderNo)
	delete svTrnInvMechanic where CompanyCode = @CompanyCode and BranchCode = @BranchCode and InvoiceNo in (Select InvoiceNo from svTrnInvoice where CompanyCode = @CompanyCode and BranchCode = @BranchCode and JobOrderNo = @JobOrderNo)
	delete svTrnInvTask where CompanyCode = @CompanyCode and BranchCode = @BranchCode and InvoiceNo in (Select InvoiceNo from svTrnInvoice where CompanyCode = @CompanyCode and BranchCode = @BranchCode and JobOrderNo = @JobOrderNo)
	delete svTrnInvoice where CompanyCode = @CompanyCode and BranchCode = @BranchCode and InvoiceNo in (Select InvoiceNo from svTrnInvoice where CompanyCode = @CompanyCode and BranchCode = @BranchCode and JobOrderNo = @JobOrderNo)
	
	if (@CompanyCode != @CompanyMD and @BranchCode != @BranchMD)
	begin
	declare @Query VARCHAR(MAX)
		
	set @Query ='delete from '+dbo.GetDbMD(@CompanyCode, @BranchCode)+'..svSDMovement 
		where CompanyCode = '''+ @CompanyCode +''' and BranchCode = '''+ @BranchCode +'''
		and DocNo in (Select InvoiceNo from svTrnInvoice where CompanyCode = '''+ @CompanyCode +''' and BranchCode = '''+ @BranchCode +''' and JobOrderNo = '''+ @JobOrderNo +''')'	
	end
	
end try
begin catch
    set @errmsg = 'InvoiceNo : ' + @InvoiceNo + char(13) + 'Error Message:' + char(13) + error_message()
    raiserror (@errmsg,16,1);
end catch
go
/****** Object:  StoredProcedure [dbo].[sp_EdpTransNo]    Script Date: 6/25/2015 2:42:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER procedure [dbo].[sp_EdpTransNo] (  

@CompanyCode varchar(10),
@BranchCode varchar(10),
@TypeOfGoods varchar(10),
@isTrex bit
--@LampiranNo varchar(10)
)

as

SELECT * INTO #t1 FROM ( 
SELECT
    a.LampiranNo
    , a.DealerCode as SupplierCode
    , ISNULL(b.SupplierName, '') as SupplierName
    , ISNULL(c.TypeOfGoods, '') TypeofGoods
FROM spUtlStockTrfHdr a
LEFT JOIN GnMstSupplier b ON b.CompanyCode = a.CompanyCode 
    AND b.SupplierCode = a.DealerCode
LEFT JOIN SpTrnSLmpHdr c ON c.CompanyCode = a.CompanyCode 
    AND c.BranchCode = a.DealerCode
    AND c.LmpNo = a.LampiranNo
WHERE a.CompanyCode = @CompanyCode
  AND a.BranchCode = @BranchCode
  AND a.Status in ('0','1') 
  AND c.isLocked = CASE @isTrex WHEN 1 THEN 1 ELSE 0 END
  ) #t1

SELECT * FROM #t1 WHERE TypeofGoods = @TypeOfGoods 

DROP TABLE #t1

go

/****** Object:  StoredProcedure [dbo].[uspfn_GenerateLampiranNPNew]    Script Date: 6/26/2015 9:01:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER procedure [dbo].[uspfn_GenerateLampiranNPNew]
--DECLARE
	@CompanyCode	VARCHAR(MAX),
	@BranchCode		VARCHAR(MAX),
	@PickingSlipNo	VARCHAR(MAX),
	@LmpDate		DATETIME,
	@ProductType	VARCHAR(MAX),
	@UserID			VARCHAR(MAX),
	@TypeOfGoods	VARCHAR(MAX)
	
--SET	@CompanyCode	= '6159401000'
--SET	@BranchCode		= '6159401001'
--SET	@PickingSlipNo	= 'PLS/15/010478'
--SET	@LmpDate		= '20150520'
--SET	@ProductType	= '4W'
--SET	@UserID			= 'yo'
--SET	@TypeOfGoods	= '0'

AS
BEGIN
	BEGIN TRY
	BEGIN TRAN
		DECLARE @MaxLmpNo INT
		SET @MaxLmpNo = ISNULL((SELECT DocumentSequence + 1 FROM GnMstDocument WHERE 
			CompanyCode = @CompanyCode
				AND BranchCode = @BranchCode
				AND DocumentType = 'LMP' 
				AND ProfitCenterCode = '300' 
				AND DocumentYear = YEAR(GetDate())),0)

		DECLARE @errmsg VARCHAR(MAX)
		DECLARE @TempLmpNo	VARCHAR(MAX)
		DECLARE @TempBPSFNo	VARCHAR(MAX)
		DECLARE @CustomerCode VARCHAR(MAX)

		SET @TempLmpNo  = ISNULL((SELECT 'LMP/' + RIGHT(YEAR(GETDATE()),2) +'/' + RIGHT(1000000 + CONVERT(VARCHAR, @MaxLmpNo, 1), 6)),'LMP/YY/XXXXXX')
		SET @TempBPSFNo = ISNULL((SELECT BPSFNo FROM SpTrnSBPSFHdr WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND PickingSlipNo = @PickingSlipNo),'')
		SET @CustomerCode = ISNULL((SELECT CustomerCode FROM SpTrnSPickingHdr WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND PickingSlipNo = @PickingSlipNo),'')

		IF (ISNULL((SELECT Status FROM SpTrnSBPSFhdr WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND BPSFNo = @TempBPSFno), '0') = '2')
		BEGIN
			SET @errmsg = 'PEMBUATAN DOKUMEN LAMPIRAN GAGAL ! ' + char(13) + char(13) + 'Error Message: Nomor picking ini sudah pernah di-proses, hubungi IT support untuk pemerikasaan data lebih lanjut !'
			RAISERROR (@errmsg,16,1);
			RETURN
		END

		UPDATE SpTrnSBPSFHdr
		SET Status = '2'
			, LastUpdateDate = GetDate()
			, LastUpdateBy = @UserID
		WHERE CompanyCode = @CompanyCode
			AND BranchCode = @BranchCode
			AND BPSFNo = @TempBPSFNo
		--===============================================================================================================================
		IF (ISNULL((SELECT LmpNo FROM SpTrnSLmpHdr WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND LmpNo = @TempLmpNo), '') <> '')
		BEGIN
			SET @errmsg = 'PEMBUATAN DOKUMEN LAMPIRAN GAGAL ! ' + char(13) + char(13) + 'Error Message: Nomor lampiran sudah ada, periksa setting-an sequence dokumen (LMP) pada general module !'
			RAISERROR (@errmsg,16,1);
			RETURN
		END

		DECLARE @isLocked BIT
		SET @isLocked = (SELECT IsLocked FROM SpTrnSPickingHdr WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND PickingSlipNo = @PickingSlipNo)

		INSERT INTO SpTrnSLmpHdr
		SELECT
			CompanyCode
			, BranchCode
			, @TempLmpNo LmpNo	
			, GetDate() LmpDate
			, @TempBPSFNo BPSFNo
			, (SELECT BPSFDate FROM SpTrnSBPSFHdr WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND BPSFNo = @TempBPSFNo)
				BPSFDate
			, PickingSlipNo
			, PickingSlipDate
			, TransType
			, CustomerCode
			, CustomerCodeBill
			, CustomerCodeShip
			, TotSalesQty
			, TotSalesAmt
			, TotDiscAmt
			, TotDPPAmt
			, TotPPNAmt
			, TotFinalSalesAmt
			, CONVERT(BIT, 0) isPKP
			, '0' Status
			, 0 PrintSeq
			, TypeOfGoods
			, @UserID CreatedBy
			, GetDate() CreatedDate
			, @UserID LastUpdateBy
			, GetDate() LastUpdateDate
			, @isLocked IsLocked	
			, NULL LockingBy
			, NULL LockingDate
		FROM SpTrnSPickingHdr 
		WHERE 
			1 = 1
			AND CompanyCode = @CompanyCode
			AND BranchCode = @BranchCode
			AND PickingSlipNo = @PickingSlipNo
		
		UPDATE GnMstDocument
		SET DocumentSequence = DocumentSequence + 1
			, LastUpdateDate = GetDate()
			, LastUpdateBy = @UserID
		WHERE
			1 = 1
			AND CompanyCode = @CompanyCode
			AND BranchCode = @BranchCode
			AND DocumentType = 'LMP'
			AND ProfitCenterCode = '300'
			AND DocumentYear = Year(GetDate())
		
		--===============================================================================================================================
		-- INSERT LAMPIRAN DETAIL
		--===============================================================================================================================
		DECLARE @QueryTemp VARCHAR(MAX)

		DECLARE @tblTemp as table  
		(  
			CompanyCode varchar(20),
			BranchCode varchar(20),
			PartNo varchar(20),
			TypeOfGoods varchar(2),
			RetailPrice decimal(18,2),  
			PurcDiscPct decimal(18,2) 
		)
		
		SET @QueryTemp = 'select c.CompanyCode,c.BranchCode,a.PartNo,a.TypeOfGoods,b.RetailPrice,a.PurcDiscPct from '+ dbo.GetDbMD(@CompanyCode, @BranchCode) +'..spmstitems a
		join '+ dbo.GetDbMD(@CompanyCode, @BranchCode) +'..spMstItemPrice b on a.CompanyCode = b.CompanyCode and a.BranchCode = b.BranchCode and a.PartNo = b.PartNo
		join gnMstCompanyMapping c on a.CompanyCode = c.CompanyMD and a.BranchCode = c.BranchMD'
		          
	  insert into @tblTemp    
	  exec (@QueryTemp)

	  declare @PurchaseDisc as decimal  
	  set @PurchaseDisc = (select DiscPct from gnMstSupplierProfitCenter   
		   where CompanyCode = @CompanyCode   
		   and BranchCode = @BranchCode  
		   and SupplierCode = dbo.GetBranchMD(@CompanyCode, @BranchCode)
		   and ProfitCenterCode = '300')  
	         
	  if (@PurchaseDisc is null) raiserror ('Profit Center 300 belum tersetting pada Supplier tersebut!!!',16,1);      

		INSERT INTO SpTrnSLmpDtl
		SELECT
			a.CompanyCode
			, a.BranchCode
			, @TempLmpNo LmpNo
			, a.WarehouseCode
			, a.PartNo
			, a.PartNoOriginal
			, a.DocNo
			, a.DocDate
			, a.ReferenceNo
			, a.ReferenceDate
			, a.LocationCode
			, a.QtyBill
			, a.RetailPriceInclTax
			, a.RetailPrice
			--, ISNULL((SELECT CostPrice FROM SpMstItemPrice WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND PartNo = a.PartNo),0) CostPrice
			, CASE WHEN (b.TypeOfGoods = 2) THEN FLOOR (b.RetailPrice - (b.RetailPrice * (25 * 0.01)))
			  WHEN (b.TypeOfGoods = 5) THEN FLOOR(b.RetailPrice)
			  WHEN ISNULL(b.PurcDiscPct,0) = 0 
		      THEN FLOOR ( b.RetailPrice - (b.RetailPrice *(@PurchaseDisc * 0.01)))
			  ELSE FLOOR (b.RetailPrice - (b.RetailPrice * (b.PurcDiscPct * 0.01))) END CostPrice
			, a.DiscPct
			, a.SalesAmt
			, a.DiscAmt
			, a.NetSalesAmt
			, 0 PPNAmt
			, a.TotSalesAmt
			, a.ProductType
			, a.PartCategory 
			, a.MovingCode
			, a.ABCClass
			, @UserID CreatedBy
			, GetDate() CreatedDate
			, @UserID LastUpdateBy
			, GetDate() LastUpdateDate
		FROM SpTrnSPickingDtl a
		LEFT join @tblTemp b
		ON b.CompanyCode = a.CompanyCode
		AND b.BranchCode = a.BranchCode
		AND b.PartNo = a.PartNo
		WHERE 
			1 = 1
			AND a.CompanyCode = @CompanyCode
			AND a.BranchCode = @BranchCode
			AND a.PickingSlipNo = @PickingSlipNo
			AND a.QtyPicked > 0
		
		--===============================================================================================================================
		-- INSERT SvSdMovement
		--===============================================================================================================================
		declare @SQL as varchar(max)
		declare @md bit
		set @md = (select case WHEN EXISTS(select * from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode and CompanyMD = @CompanyCode and BranchMD = @BranchCode) then cast(1 as bit) ELSE cast(0 as bit) END)
		if @md = 0
		BEGIN
		set @SQL = '
			insert into ' +dbo.GetDbMD(@CompanyCode,@BranchCode)+'..svSDMovement (CompanyCode, BranchCode, DocNo, DocDate, PartNo, PartSeq
			, WarehouseCode, QtyOrder, Qty, DiscPct, CostPrice, RetailPrice
			, TypeOfGoods, CompanyMD, BranchMD, WarehouseMD, RetailPriceInclTaxMD, RetailPriceMD
			, CostPriceMD, QtyFlag, ProductType, ProfitCenterCode, Status, ProcessStatus
			, ProcessDate, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)  
			select h.CompanyCode, h.BranchCode, h.LmpNo, h.LmpDate, d.PartNo, ROW_NUMBER() OVER(Order by d.LmpNo)
			,d.WarehouseCode, d.QtyBill, d.QtyBill, d.DiscPct
			, d.CostPrice
			, d.RetailPrice
			,h.TypeOfGoods, ''' +dbo.GetCompanyMD(@CompanyCode,@BranchCode)+ ''', ''' + +dbo.GetBranchMD(@CompanyCode,@BranchCode)+ + ''', ''' +dbo.GetWarehouseMD(@CompanyCode,@BranchCode)+ ''', p.RetailPriceInclTax, p.RetailPrice, 
			p.CostPrice,''x'', d.ProductType,''300'', ''0'',''0'',''' + convert(varchar, GETDATE()) + ''',''' + @UserID + ''',''' +
			  convert(varchar, GETDATE()) + ''',''' +  @UserID + ''',''' +  convert(varchar, GETDATE()) + '''
			 from spTrnSLmpDtl d 
			 inner join spTrnSLmpHdr h on h.CompanyCode = d.CompanyCode and h.BranchCode = d.BranchCode and h.LmpNo = d.LmpNo  
			 join '+ dbo.GetDbMD(@CompanyCode, @BranchCode) +'..spmstitemprice p
			 on p.PartNo = d.PartNo and p.CompanyCode = '''+  dbo.GetCompanyMD(@CompanyCode, @BranchCode) +''' and p.BranchCode = '''+ dbo.GetBranchMD(@CompanyCode, @BranchCode) +'''
			  where 1 = 1   
				and d.CompanyCode = ''' + @CompanyCode + ''' 
				and d.BranchCode  = ''' + @BranchCode  + '''
				and d.ProductType = ''' + @ProductType  + '''
				and d.LmpNo = ''' + @TempLmpNo + ''''
		exec(@SQL)
		END	
		--===============================================================================================================================
		-- UPDATE STOCK
		-- NOTES : Transtype = 11 --> + BorrowedQty
		--         Transtype = 12 --> - BorrowQty
		--===============================================================================================================================
		DECLARE @TempTransType VARCHAR (MAX) 
		SET @TempTransType = ISNULL((SELECT SUBSTRING(TransType,1,1) FROM SpTrnSLmpHdr WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND LmpNo = @TempLmpNo),0)
		
		--===============================================================================================================================
		-- VALIDATION QTY
		--===============================================================================================================================
		DECLARE @Onhand_Valid NUMERIC(18,2)	
		DECLARE @Allocation_SRValid NUMERIC(18,2)
		DECLARE @Allocation_SLValid NUMERIC(18,2)
		DECLARE @Allocation_SPValid NUMERIC(18,2)
		DECLARE @Valid2 as table
		(
			PartNo  varchar(20),
			QtyValidSR  decimal,
			QtyValidSL  decimal,
			QtyValidSP  decimal,
			QtyValidOnhand  decimal
		)

		set @SQL = 
		'SELECT * INTO #Valid_2 FROM(
			SELECT a.PartNo
				, b.AllocationSR - a.QtyBill QtyValidSR
				, b.AllocationSL - a.QtyBill QtyValidSL
				, b.AllocationSP - a.QtyBill QtyValidSP
				, b.Onhand - a.QtyBill QtyValidOnhand
			FROM SpTrnSPickingDtl a
			INNER JOIN '+dbo.GetDbMD(@CompanyCode,@BranchCode)+'..SpMstItems b ON b.CompanyCode = '+dbo.GetCompanyMD(@CompanyCode,@BranchCode)+'
				AND b.BranchCode = '+dbo.GetBranchMD(@CompanyCode,@BranchCode)+'
				AND b.PartNo = a.PartNo		
			WHERE 1 = 1
				AND a.CompanyCode = '''+@CompanyCode+'''
				AND a.BranchCode = '''+@BranchCode+'''
				AND a.PickingSlipNo = '''+@PickingSlipNo+'''
		) #Valid_2
				
		select * from #Valid_2
		drop table #Valid_2'
		
		insert into @Valid2
		exec(@SQL)
		SET @Allocation_SRValid = ISNULL((SELECT TOP 1 QtyValidSR FROM @Valid2 WHERE QtyValidSR < 0),0)
		SET @Allocation_SPValid = ISNULL((SELECT TOP 1 QtyValidSP FROM @Valid2 WHERE QtyValidSP < 0),0)
		SET @Allocation_SLValid = ISNULL((SELECT TOP 1 QtyValidSL FROM @Valid2 WHERE QtyValidSL < 0),0)
		SET @Onhand_Valid = ISNULL((SELECT TOP 1 QtyValidOnhand FROM @Valid2 WHERE QtyValidOnhand < 0),0)
		
		IF (@TempTransType = '2')
		BEGIN
			IF (@Onhand_Valid < 0 OR @Allocation_SRValid < 0)
			BEGIN
				SET @errmsg = 'PEMBUATAN DOKUMEN LAMPIRAN GAGAL ! ' + char(13) + char(13) + 'Error Message: Terdapat part dengan quantity Onhand atau alokasi kurang dari nol !'
				RAISERROR (@errmsg,16,1);
				RETURN
			END 
		END
		
		IF (@TempTransType = '1')
		BEGIN
			IF (@Onhand_Valid < 0 OR @Allocation_SPValid < 0)
			BEGIN
				SET @errmsg = 'PEMBUATAN DOKUMEN LAMPIRAN GAGAL ! ' + char(13) + char(13) + 'Error Message: Terdapat part dengan quantity Onhand atau alokasi kurang dari nol !'
				RAISERROR (@errmsg,16,1);
				RETURN
			END 
		END	

		IF (@TempTransType = '3')
		BEGIN
			IF (@Onhand_Valid < 0 OR @Allocation_SLValid < 0)
			BEGIN
				SET @errmsg = 'PEMBUATAN DOKUMEN LAMPIRAN GAGAL ! ' + char(13) + char(13) + 'Error Message: Terdapat part dengan quantity Onhand atau alokasi kurang dari nol !'
				RAISERROR (@errmsg,16,1);
				RETURN
			END 
		END	
		
		--===============================================================================================================================
		IF (ISNULL((SELECT TransType FROM SpTrnSLmpHdr WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND LmpNo = @TempLmpNo),0) = '11')
		BEGIN
		SET @SQL = 
		'UPDATE '+dbo.GetDbMD(@CompanyCode,@BranchCode)+'..SpMstItems
		SET
			BorrowedQty = BorrowedQty + b.QtyBill
		FROM '+dbo.GetDbMD(@CompanyCode,@BranchCode)+'..SpMstItems a, SpTrnSPickingDtl b
		WHERE
			1 = 1
			AND a.CompanyCode = '+dbo.GetCompanyMD(@CompanyCode,@BranchCode)+'
			AND a.BranchCode = '+dbo.GetBranchMD(@CompanyCode,@BranchCode)+'
			AND b.PickingSlipNo = '''+@PickingSlipNo+'''
			AND a.PartNo = b.PartNo'
		EXEC(@SQL)
		END

		IF (ISNULL((SELECT TransType FROM SpTrnSLmpHdr WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND LmpNo = @TempLmpNo),0) = '12')
		BEGIN
			SET @SQL =
			'UPDATE '+dbo.GetDbMD(@CompanyCode,@BranchCode)+'..SpMstItems
			SET
				BorrowQty = BorrowQty - b.QtyBill
			FROM '+dbo.GetDbMD(@CompanyCode,@BranchCode)+'..SpMstItems a, SpTrnSPickingDtl b
			WHERE
				1 = 1
				AND a.CompanyCode = '+dbo.GetCompanyMD(@CompanyCode,@BranchCode)+'
				AND a.BranchCode = '+dbo.GetBranchMD(@CompanyCode,@BranchCode)+'
				AND b.PickingSlipNo = '''+@PickingSlipNo+'''
				AND a.PartNo = b.PartNo'
			EXEC(@SQL)	
		END

		IF (@TempTransType = '2')
		BEGIN
		
		SET @SQL =
		'UPDATE '+dbo.GetDbMD(@CompanyCode,@BranchCode)+'..SpMstItems
		SET
			AllocationSR = AllocationSR - b.QtyBill
			, Onhand = Onhand - b.QtyBill
			, LastUpdateBy = '''+@UserID+'''
			, LastUpdateDate = GetDate()
			, LastSalesDate =  GetDate()
		FROM '+dbo.GetDbMD(@CompanyCode,@BranchCode)+'..SpMstItems a, SpTrnSPickingDtl b
		WHERE
			1 = 1
			AND a.CompanyCode = '+dbo.GetCompanyMD(@CompanyCode,@BranchCode)+'
			AND a.BranchCode = '+dbo.GetBranchMD(@CompanyCode,@BranchCode)+'
			AND b.PickingSlipNo = '''+@PickingSlipNo+'''
			AND a.PartNo = b.PartNo

		UPDATE '+dbo.GetDbMD(@CompanyCode,@BranchCode)+'..SpMstItemLoc
		SET
			AllocationSR = AllocationSR - b.QtyBill
			, Onhand = Onhand - b.QtyBill
			, LastUpdateBy = '''+@UserID+'''
			, LastUpdateDate = GetDate()
		FROM '+dbo.GetDbMD(@CompanyCode,@BranchCode)+'..SpMstItemLoc a, SpTrnSPickingDtl b
		WHERE
			1 = 1
			AND a.CompanyCode = '+dbo.GetCompanyMD(@CompanyCode,@BranchCode)+'
			AND a.BranchCode = '+dbo.GetBranchMD(@CompanyCode,@BranchCode)+'
			AND a.WarehouseCode = ''00''
			AND b.PickingSlipNo = '''+@PickingSlipNo+'''
			AND a.PartNo = b.PartNo'
		EXEC(@SQL)
		END

		IF (@TempTransType = '1')
		BEGIN
			SET @SQL =
			'UPDATE '+dbo.GetDbMD(@CompanyCode,@BranchCode)+'..SpMstItems
			SET
				AllocationSP = AllocationSP - b.QtyBill
				, Onhand = Onhand - b.QtyBill
				, LastUpdateBy = '''+@UserID+'''
				, LastUpdateDate = GetDate()
				, LastSalesDate = GetDate()
			FROM '+dbo.GetDbMD(@CompanyCode,@BranchCode)+'..SpMstItems a, SpTrnSPickingDtl b
			WHERE
				1 = 1
				AND a.CompanyCode = '+dbo.GetCompanyMD(@CompanyCode,@BranchCode)+'
				AND a.BranchCode = '+dbo.GetBranchMD(@CompanyCode,@BranchCode)+'
				AND b.PickingSlipNo = '''+@PickingSlipNo+'''
				AND a.PartNo = b.PartNo

			UPDATE '+dbo.GetDbMD(@CompanyCode,@BranchCode)+'..SpMstItemLoc
			SET
				AllocationSP = AllocationSP - b.QtyBill
				, Onhand = Onhand - b.QtyBill
				, LastUpdateBy = '''+@UserID+'''
				, LastUpdateDate = GetDate()
			FROM '+dbo.GetDbMD(@CompanyCode,@BranchCode)+'..SpMstItemLoc a, SpTrnSPickingDtl b
			WHERE
				1 = 1
				AND a.CompanyCode = '+dbo.GetCompanyMD(@CompanyCode,@BranchCode)+'
				AND a.BranchCode = '+dbo.GetBranchMD(@CompanyCode,@BranchCode)+'
				AND a.WarehouseCode = ''00''
				AND b.PickingSlipNo = '''+@PickingSlipNo+'''
			AND a.PartNo = b.PartNo'
			EXEC(@SQL)
		END

		IF (@TempTransType = '3')
		BEGIN
			SET @SQL =
			'UPDATE '+dbo.GetDbMD(@CompanyCode,@BranchCode)+'..SpMstItems
			SET
				AllocationSL = AllocationSL - b.QtyBill
				, Onhand = Onhand - b.QtyBill
				, LastUpdateBy = '''+@UserID+'''
				, LastUpdateDate = GetDate()
				, LastSalesDate = GetDate()
			FROM '+dbo.GetDbMD(@CompanyCode,@BranchCode)+'..SpMstItems a, SpTrnSPickingDtl b
			WHERE
				1 = 1
				AND a.CompanyCode = '+dbo.GetCompanyMD(@CompanyCode,@BranchCode)+'
				AND a.BranchCode = '+dbo.GetBranchMD(@CompanyCode,@BranchCode)+'
				AND b.PickingSlipNo = '''+@PickingSlipNo+'''
				AND a.PartNo = b.PartNo

			UPDATE '+dbo.GetDbMD(@CompanyCode,@BranchCode)+'..SpMstItemLoc
			SET
				AllocationSL = AllocationSL - b.QtyBill
				, Onhand = Onhand - b.QtyBill
				, LastUpdateBy = '''+@UserID+'''
				, LastUpdateDate = GetDate()
			FROM '+dbo.GetDbMD(@CompanyCode,@BranchCode)+'..SpMstItemLoc a, SpTrnSPickingDtl b
			WHERE
				1 = 1
				AND a.CompanyCode = '+dbo.GetCompanyMD(@CompanyCode,@BranchCode)+'
				AND a.BranchCode = '+dbo.GetBranchMD(@CompanyCode,@BranchCode)+'
				AND a.WarehouseCode = ''00''
				AND b.PickingSlipNo = '''+@PickingSlipNo+'''
				AND a.PartNo = b.PartNo'
			EXEC(@SQL)
		END
		--===============================================================================================================================
		-- UPDATE DEMAND CUST AND DEMAND ITEM
		--===============================================================================================================================

		UPDATE SpHstDemandCust
		SET SalesFreq = SalesFreq + 1
			, SalesQty = SalesQty + b.QtyBill
			, LastUpdateBy = @UserID 
			, LastUpdateDate = GetDate()
		FROM SpHstDemandCust a, SpTrnSPickingDtl b
		WHERE
			1 = 1
			AND a.CompanyCode = @CompanyCode
			AND a.BranchCode = @BranchCode
			AND b.CompanyCode = @CompanyCode
			AND b.BranchCode = @BranchCode
			AND b.PickingSlipNo = @PickingSlipNo
			AND a.CompanyCode = b.CompanyCode
			AND a.BranchCode = b.BranchCode
			AND a.Year = Year(b.DocDate)
			AND a.Month = Month(b.DocDate)
			AND a.CustomerCode = @CustomerCode
			AND a.PartNo = b.PartNo
			
		UPDATE SpHstDemandItem
		SET SalesFreq = SalesFreq + 1
			, SalesQty = SalesQty + b.QtyBill
			, LastUpdateBy = @UserID 
			, LastUpdateDate = GetDate()
		FROM SpHstDemandItem a, SpTrnSPickingDtl b
		WHERE
			1 = 1
			AND a.CompanyCode = @CompanyCode
			AND a.BranchCode = @BranchCode
			AND b.CompanyCode = @CompanyCode
			AND b.BranchCode = @BranchCode
			AND b.PickingSlipNo = @PickingSlipNo
			AND a.CompanyCode = b.CompanyCode
			AND a.BranchCode = b.BranchCode
			AND a.Year = Year(b.DocDate)
			AND a.Month = Month(b.DocDate)
			AND a.PartNo = b.PartNo

		----=============================================================================================================================
		---- INSERT TO ITEM MOVEMENT
		----=============================================================================================================================
		INSERT INTO SpTrnIMovement
		SELECT
			@CompanyCode CompanyCode
			, @BranchCode BranchCode
			, a.LmpNo DocNo
			, (SELECT LmPDate FROM SpTrnSLmpHdr WHERE CompanyCode = @CompanyCode 
				AND BranchCode = @BranchCode AND LmpNo = a.LmpNo) 
			  DocDate
			, dateadd(s,ROW_NUMBER() OVER(Order by a.PartNo),getdate()) CreatedDate 
			, '00' WarehouseCode
			, LocationCode 
			, a.PartNo
			, 'OUT' SignCode
			, 'LAMPIRAN' SubSignCode
			, a.QtyBill
			, a.RetailPrice
			, a.CostPrice
			, a.ABCClass
			, a.MovingCode
			, a.ProductType
			, a.PartCategory
			, @UserID CreatedBy
		FROM SpTrnSLmpDtl a
		WHERE
			1 = 1
			AND CompanyCode = @CompanyCode
			AND BranchCode = @BranchCode
			AND LmpNo = @TempLmpNo


		--===============================================================================================================================
		-- UPDATE AVERAGE COST
		-- NOTES : Transtype = 2% (SERVICE) CHECK ISLINKTOSERVICE
		--===============================================================================================================================
		IF (ISNULL((SELECT SUBSTRING(TransType,1,1) FROM SpTrnSLmpHdr WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND LmpNo = @TempLmpNo),0) = '2')
		BEGIN
			IF (CONVERT(VARCHAR,ISNULL((SELECT IsLinkToService FROM gnMstCoProfile WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode),0),0) = '1')
			BEGIN
				SELECT
					a.CompanyCode
					, a.BranchCode
					, d.ProductType
					, ISNULL(d.ServiceNo, 0) ServiceNo
					, a.PartNo
					, a.DocNo SupplySlipNo
					, ISNULL(a.CostPrice, 0) CostPrice
  					, ISNULL(a.RetailPrice, 0) RetailPrice
				INTO #1
				FROM spTrnSLmpDtl a 
					INNER JOIN spTrnSORDHdr c ON a.CompanyCode = c.CompanyCode
						AND a.BranchCode = c.BranchCode
						AND a.DocNo = c.DocNo
					INNER JOIN svTrnService d ON a.CompanyCode = d.CompanyCode
						AND a.BranchCode = d.BranchCode
				WHERE a.CompanyCode = @CompanyCode
					AND a.BranchCode = @BranchCode
					AND d.ProductType = @ProductType
					AND c.UsageDocNo = d.JobOrderNo
					AND a.LmpNo = @TempLmpNo

				UPDATE svTrnSrvItem 
				SET	CostPrice = b.CostPrice
					, LastUpdateBy = @UserID
					, LastUpdateDate = GETDATE()
				FROM svTrnSrvItem a, #1 b
				WHERE a.CompanyCode = b.CompanyCode
					AND a.BranchCode = b.BranchCode
					AND a.ProductType = b.ProductType
					AND a.ServiceNo = b.ServiceNo  
					AND a.PartNo = b.PartNo
					AND a.SupplySlipNo = b.SupplySlipNo	

				--===============================================================================================================================
				-- SERVICE PART
				--===============================================================================================================================
				SELECT * INTO #TempServiceItem FROM (
				SELECT 
					a.CompanyCode
					, a.BranchCode
					, a.ProductType
					, a.ServiceNo
					, a.PartNo
					, a.PartSeq
					, a.DemandQty
					, a.SupplyQty
					, b.QtyBill
					, b.DocNo
					, a.CostPrice
					, a.RetailPrice
					, a.TypeOfGoods
					, a.BillType
					, a.DiscPct
				FROM SvTrnSrvItem a 
				INNER JOIN SpTrnSPickingDtl b ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.PartNo = b.PartNo AND a.SupplySlipNo = b.DocNo
				WHERE
					1 = 1
					AND a.CompanyCode = @CompanyCode
					AND a.BranchCode = @BranchCode
					AND a.ProductType = @ProductType
					AND a.ServiceNo IN (SELECT ServiceNo 
										FROM SvTrnService 
										WHERE 1 = 1 AND CompanyCode = @CompanyCode 
											AND BranchCode = @BranchCode 
											AND JobOrderNo IN (SELECT ReferenceNo 
																FROM SpTrnSPickingDtl 
																WHERE 1= 1 AND CompanyCode = @CompanyCode 
																	AND  BranchCode = @BranchCode 
																	AND PickingSlipNo = @PickingSlipNo))
					AND a.PartSeq IN (SELECT MAX(PartSeq) FROM SvTrnSrvItem WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode
						AND ProductType = @ProductType AND ServiceNo = a.ServiceNo AND PartNo = a.PartNo)
					AND a.SupplySlipNo = b .DocNo
				) #TempServiceItem 

				UPDATE svTrnSrvItem
				SET SupplyQty = (CASE WHEN b.QtyBill > b.DemandQty 
								THEN 
									CASE WHEN b.DemandQty = 0 THEN b.QtyBill ELSE b.DemandQty END
								ELSE b.QtyBill END)
					, LastUpdateBy = @UserID
					, LastUpdateDate = Getdate()
				FROM svTrnSrvItem a, #TempServiceItem b
				WHERE
					1 = 1
					AND a.CompanyCode = b.CompanyCode
					AND a.BranchCode = b.BranchCode
					AND a.ProductType = b.ProductType
					AND a.ServiceNo = b.ServiceNo
					AND a.PartNo = b.PartNo
					AND a.PartSeq = b.PartSeq
					AND a.SupplySlipNo = b.DocNo

				UPDATE svTrnSrvItem
				SET CostPrice = b.CostPrice
					, LastUpdateBy = @UserID
					, LastUpdateDate = Getdate()
				FROM svTrnSrvItem a, #TempServiceItem b
				WHERE
					1 = 1
					AND a.CompanyCode = b.CompanyCode
					AND a.BranchCode = b.BranchCode
					AND a.ProductType = b.ProductType
					AND a.ServiceNo = b.ServiceNo
					AND a.PartNo = b.PartNo
					AND a.SupplySlipNo = b.DocNo

				--===============================================================================================================================
				-- INSERT NEW SRV ITEM BASED PICKING LIST
				--===============================================================================================================================
				INSERT INTO SvTrnSrvItem (CompanyCode, BranchCode, ProductType, ServiceNo, PartNo, PartSeq, DemandQty, SupplyQty, ReturnQty, CostPrice, RetailPrice, TypeOfGoods, BillType, SupplySlipNo, SupplySlipDate, SSReturnNo, SSReturnDate, CreatedBy, CreatedDate, LastupdateBy, LastupdateDate, DiscPct)
				SELECT 
					a.CompanyCode
					, a.BranchCode
					, a.ProductType
					, a.ServiceNo
					, a.PartNo
					, (select max(PartSeq)+1 from svTrnSrvItem b where b.CompanyCode = a.CompanyCode and b.BranchCode = a.BranchCode and 
						b.ProductType = a.ProductType and b.ServiceNo = a.ServiceNo) PartSeq
					, 0 DemandQty
					, a.QtyBill - a.DemandQty SupplyQty
					, 0 ReturnQty
					, a.CostPrice
					, a.RetailPrice
					, a.TypeOfGoods
					, a.BillType
					, a.DocNo SupplySlipNo
					, (SELECT TOP 1 DocDate FROM SpTrnSORDHdr WHERE 1= 1 AND CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode
						AND DocNo = a.DocNo) SupplySlipDate
					, NULL SSReturnNo
					, NULL SSReturnDate
					, @UserID CreatedBy
					, GetDate() CreatedDate
					, @UserID LastUpdateBy
					, GetDate() LastUpdateDate
					, a.DiscPct
				FROM #TempServiceItem a 
				WHERE
					1 = 1
					AND a.CompanyCode = @CompanyCode
					AND a.BranchCode = @BranchCode
					AND a.ProductType = @ProductType
					AND a.DemandQty < a.QtyBill
					AND a.QtyBill > 0
					AND a.DemandQty > 0

				DROP TABLE #TempServiceItem 	
				DROP TABLE #1
			END
		END

		--===============================================================================================================================
		-- GENERATE JOURNAL AND AUTOMATE TRANSFER STOCK
		-- NOTES : Transtype = 10 (TRANSFER STOCK)
		--===============================================================================================================================
		DECLARE @TempJournalPrefix	VARCHAR(MAX)
		DECLARE @MaxTempJournal		INT

		DECLARE @TempJournal		VARCHAR(MAX)
		DECLARE @Amount				NUMERIC(18,2)
		DECLARE @TempFiscalMonth	INT
		DECLARE @TempFiscalYear		INT

		DECLARE @PeriodeNum			NUMERIC(18,0)
		DECLARE @Periode			VARCHAR(MAX)
		DECLARE @PeriodeName		VARCHAR(MAX)
		DECLARE @GLDate				DATETIME

		SET @TempFiscalYear = ISNULL((SELECT FiscalYear FROM GnMstCoProfileSpare WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode),0) 
		SET @TempFiscalMonth  = ISNULL((SELECT FiscalMonth FROM GnMstCoProfileSpare WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode),0) 

		SET @PeriodeNum = ISNULL((SELECT  TOP 1 PeriodeNum
				FROM gnMstPeriode 
				WHERE CompanyCode = @CompanyCode
					AND BranchCode = @BranchCode AND FiscalYear = @TempFiscalYear
					AND FiscalMonth = @TempFiscalMonth AND StatusSparepart = 1
					AND (MONTH(FromDate) = MONTH(@LmpDate) AND YEAR(FromDate) = YEAR(@LmpDate))
					AND FiscalStatus = 1 ), NULL) 

		SET @Periode = ISNULL((SELECT  TOP 1 CONVERT(varchar, FiscalYear) + RIGHT('00' + CONVERT(varchar, PeriodeNum), 2) AS Periode
				FROM gnMstPeriode 
				WHERE CompanyCode = @CompanyCode
					AND BranchCode = @BranchCode AND FiscalYear = @TempFiscalYear
					AND FiscalMonth = @TempFiscalMonth AND StatusSparepart = 1
					AND (MONTH(FromDate) = MONTH(@LmpDate) AND YEAR(FromDate) = YEAR(@LmpDate))
					AND FiscalStatus = 1 ), NULL) 

		SET @PeriodeName =  ISNULL((SELECT  TOP 1 PeriodeName
				FROM gnMstPeriode 
				WHERE CompanyCode = @CompanyCode
					AND BranchCode = @BranchCode AND FiscalYear = @TempFiscalYear
					AND FiscalMonth = @TempFiscalMonth AND StatusSparepart = 1
					AND (MONTH(FromDate) = MONTH(@LmpDate) AND YEAR(FromDate) = YEAR(@LmpDate))
					AND FiscalStatus = 1 ), NULL)

		DECLARE @AccountTypeInTran	VARCHAR(MAX)
		DECLARE @AccountTypeInvent	VARCHAR(MAX)

		SET @AccountTypeInTran = ISNULL((SELECT b.AccountType FROM GnMstAccount b WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode AND b.AccountNo = 
				ISNULL((SELECT InTransitAccNo FROM SpMstAccount WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND TypeOfGoods = @TypeOfGoods),'')
				),'')
		SET @AccountTypeInvent = ISNULL((SELECT b.AccountType FROM GnMstAccount b WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode AND b.AccountNo = 
				ISNULL((SELECT InventoryAccNo FROM SpMstAccount WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND TypeOfGoods = @TypeOfGoods),'')
				),'')

		--===============================================================================================================================
		-- SET ACCOUNT FOR GENERATE JOURNAL
		--===============================================================================================================================
		DECLARE @CustCode VARCHAR(MAX)
		SET @CustCode = (SELECT CustomerCode FROM spTrnSPickingHdr WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND PickingSlipNo = @PickingSlipNo)
		
		Declare @TPGO VARCHAR(MAX)
		SET @TPGO = (SELECT TypeOfGoods FROM spTrnSPickingHdr WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND PickingSlipNo = @PickingSlipNo)

		DECLARE @InventoryAccNo VARCHAR(MAX)
		DECLARE @InTransitAccNo VARCHAR(MAX)

		IF (@isLocked = '1')
		BEGIN
			DECLARE @CompTo VARCHAR(MAX)
			SET @CompTo = (SELECT ISNULL(CompanyCodeTo,'') FROM spMstCompanyAccount WHERE CompanyCode = @CompanyCode AND BranchCodeTo = @CustCode)
			
			SET @InTransitAccNo =  (SELECT ISNULL(IntercompanyAccNoTo,'') FROM spMstCompanyAccountDtl WHERE CompanyCode = @CompanyCode AND CompanyCodeTo = @CompTo AND TPGO = @TPGO)
		END
		ELSE
			SET @InTransitAccNo =  ISNULL((SELECT InTransitAccNo FROM SpMstAccount WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND TypeOfGoods = @TypeOfGoods),'')

		SET @InventoryAccNo = ISNULL((SELECT InventoryAccNo FROM SpMstAccount WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND TypeOfGoods = @TypeOfGoods),'')
		SET @Amount = ISNULL((SELECT SUM(QtyBill * CostPrice) FROM SpTrnSLmpDtl WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND LmpNo = @TempLmpNo),0)

		--===============================================================================================================================
		-- GENERATE JOURNAL
		-- NOTES : Transtype = 1O (TRANSFER STOCK)
		--===============================================================================================================================
		IF (ISNULL((SELECT TransType FROM SpTrnSLmpHdr WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND LmpNo = @TempLmpNo),0) = '10')
		BEGIN
			IF (@isLocked != '1')
			BEGIN
				--===============================================================================================================================
				-- AUTOMATE TRANSFER STOCK
				--===============================================================================================================================
				INSERT INTO SpUtlStockTrfHdr
				SELECT
					a.CompanyCode
					, a.CustomerCode BranchCode
					, a.BranchCode DealerCode
					, a.LmpNo LampiranNo
					, a.CustomerCode RcvDealerCode
					, '' InvoiceNo 
					, '' BinningNo 
					, '1900-01-01 00:00:00.000' BinningDate
					, 0 Status
					, @UserID CreatedBy
					, GetDate() CreatedDate
					, @UserID LastUpdateBy
					, GetDate() LastUpdateDate
					, null TypeOfGoods
				FROM SpTrnSLmpHdr a
				WHERE a.CompanyCode = @CompanyCode
					AND a.BranchCode = @BranchCode
					AND a.LmpNo = @TempLmpNo

				INSERT INTO SpUtlStockTrfDtl
				SELECT
					a.CompanyCode CompanyCode
					, b.CustomerCode BranchCode
					, a.BranchCode DealerCode
					, a.LmpNo LampiranNo
					, a.DocNo OrderNo
					, a.PartNo PartNo
					, b.BPSFno SalesNo
					, a.PartNo PartNoShip
					, a.QtyBill QtyShipped 
					, 1.00 SalesUnit
					, a.CostPrice PurchasePrice
					, a.CostPrice costPrice
					, '1900-01-01 00:00:00.000' ProcessDate
					, @ProductType Producttype
					, '' partCategory
					, @UserID CreatedBy
					, GetDate() CreatedDate
					, @UserID LastUpdateBy
					, GetDate() LastUpdateDate 
				FROM SpTrnSLmpDtl a
				INNER JOIN SpTrnSLmpHdr b ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.LmpNo = b.LmpNo
				WHERE a.CompanyCode = @CompanyCode
					AND a.BranchCode = @BranchCode
					AND a.LmpNo = @TempLmpNo
			END

			--===============================================================================================================================
			-- GENERATE GLINTERFACE
			--===============================================================================================================================
			IF (ISNULL((SELECT TOP 1 DocNo FROM glInterface WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND DocNo = @TempLmpNo), '') <> '')
			BEGIN
				SET @errmsg = 'PEMBUATAN DOKUMEN LAMPIRAN GAGAL ! ' + char(13) + char(13) + 'Error Message: Nomor lampiran sudah ada dalam glInterface, periksa setting-an sequence dokumen (LMP) !'
				RAISERROR (@errmsg,16,1);
				RETURN
			END
			 
			INSERT INTO GLInterface
			SELECT
				a.CompanyCode
				, a.BranchCode
				, a.LmpNo DocNo
				, 1 SeqNo
				, a.LmpDate DocDate
				, '300' ProfitCenterCode
				, GetDate() AccDate
				, @InTransitAccNo AccountNo
				, 'SPAREPART' JournalCode
				, 'BPS' TypeJournal
				, a.LmpNo ApplyTo
				, @Amount AmountDB
				, 0 AmountCR
				, 'INTRANSIT' TypeTrans
				, '' BatchNo
				, '1900-01-01 00:00:00.000' BatchDate
				, 3 StatusFlag
				, @UserID CreateBy 
				, GetDate() CreateDate
				, @UserID UpdateBy
				, GetDate() LastUpdateDate
			FROM SpTrnSLmpHdr a
			WHERE a.CompanyCode = @CompanyCode
				AND a.BranchCode = @BranchCode
				AND a.LmpNo = @TempLmpNo

			INSERT INTO GLInterface
			SELECT
				a.CompanyCode
				, a.BranchCode
				, a.LmpNo DocNo
				, 2 SeqNo
				, a.LmpDate DocDate
				, '300' ProfitCenterCode
				, GetDate() AccDate
				, @InventoryAccNo AccountNo
				, 'SPAREPART' JournalCode
				, 'BPS' TypeJournal
				, a.LmpNo ApplyTo
				, 0 AmountDB	
				, @Amount AmountCR
				, 'INVENTORY' TypeTrans
				, '' BatchNo
				, '1900-01-01 00:00:00.000' BatchDate
				, 3 StatusFlag
				, @UserID CreateBy 
				, GetDate() CreateDate
				, @UserID UpdateBy
				, GetDate() LastUpdateDate
			FROM SpTrnSLmpHdr a
			WHERE a.CompanyCode = @CompanyCode
				AND a.BranchCode = @BranchCode
				AND a.LmpNo = @TempLmpNo

			SET @GLDate = ISNULL((SELECT TOP 1 DocDate FROM GlInterface WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND DocNo = @TempLmpNo), NULL)

			--===============================================================================================================================
			-- GENERATE GLJOURNAL
			--===============================================================================================================================
			SET @MaxTempJournal = ISNULL((SELECT DocumentSequence + 1 FROM GnMstDocument WHERE 
				CompanyCode = @CompanyCode
					AND BranchCode = @BranchCode
					AND DocumentType = 'JTS' 
					AND ProfitCenterCode = '300' 
					AND DocumentYear = YEAR(GetDate())),0)

			SET @TempJournalPrefix = ISNULL((SELECT DocumentPrefix FROM GnMstDocument WHERE 
				CompanyCode = @CompanyCode
					AND BranchCode = @BranchCode
					AND DocumentType = 'JTS' 
					AND ProfitCenterCode = '300' 
					AND DocumentYear = YEAR(GetDate())),'XXX')

			SET @TempJournal = ISNULL((SELECT @TempJournalPrefix + '/' + RIGHT(YEAR(GETDATE()),2) +'/' + RIGHT(1000000 + CONVERT(VARCHAR, @MaxTempJournal, 1), 6)),'XXX/YY/XXXXXX')

			IF (ISNULL((SELECT TOP 1 JournalNo FROM GlJournal WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND JournalNo = @TempJournal), '') <> '')
			BEGIN
				SET @errmsg = 'PEMBUATAN DOKUMEN LAMPIRAN GAGAL ! ' + char(13) + char(13) + 'Error Message: Nomor journal sudah terpakai, periksa setting-an sequence dokumen (JTS) !'
				RAISERROR (@errmsg,16,1);
				RETURN
			END

			INSERT INTO GLJournal
			SELECT
				CompanyCode
				, BranchCode
				, @TempFiscalYear FiscalYear
				, '300' ProfitCenterCode
				, @TempJournal JournalNo
				, 'Harian' JournalType
				, GetDate() JournalDate
				, 'SP' DocSource
				, @TempLmpNo + ',' + @TempBPSFNo ReffNo
				, GetDate() ReffDate
				, @TempFiscalMonth FiscalMonth
				, @PeriodeNum
				, @Periode
				, @PeriodeName
				, @GLDate
				, 1 BalanceType
				, @Amount AmountDB 
				, @Amount AmountCR
				, 1 Status
				, '' StatusRecon
				, NULL BatchNo
				, '1900-01-01 00:00:00.000' PostingDate 
				, 0 StatusReverse
				, '1900-01-01 00:00:00.000' ReverseDate
				, 1 Printseq
				, 0 FSend
				, '' SendBy
				, '1900-01-01 00:00:00.000' SendDate
				, @UserID CreatedBy
				, GetDate() CreatedDate
				, @UserID LastUpdateBy
				, GetDate() LastUpdateDate
			FROM SpTrnSLmpHdr a
			WHERE a.CompanyCode = @CompanyCode
				AND a.BranchCode = @BranchCode
				AND a.LmpNo = @TempLmpNo

			--===============================================================================================================================
			-- GENERATE GLJOURNALDTL
			--===============================================================================================================================
			INSERT INTO GLJournalDtl
			SELECT
				a.CompanyCode CompanyCode
				, a.BranchCode BranchCode
				, @TempFiscalYear FiscalYear
				, @TempJournal JournalNo
				, 1 SeqNo
				, @InTransitAccNo AccountNo
				, @TempLmpNo + ',' + @TempBPSFNo Description
				, 'Harian' JournalType
				, @Amount AmountDB 
				, 0 AmountCR
				, 'INTRANSIT' TypeTrans
				, @AccountTypeIntran
				, @TempLmpNo  DocNo
				, 0 StatusReverse
				, '1900-01-01 00:00:00.000' ReverseDate
				, 0 FSend
				, @UserID CreatedBy 
				, GetDate() CreatedDate
			FROM SpTrnSLmpHdr a
			WHERE a.CompanyCode = @CompanyCode
				AND a.BranchCode = @BranchCode
				AND a.LmpNo = @TempLmpNo

			INSERT INTO GLJournalDtl
			SELECT
				a.CompanyCode CompanyCode
				, a.BranchCode BranchCode
				, @TempFiscalYear FiscalYear
				, @TempJournal JournalNo
				, 2 SeqNo
				, @InventoryAccNo AccountNo
				, @TempLmpNo + ',' + @TempBPSFNo Description
				, 'Harian' JournalType
				, 0 AmountDB
				, @Amount AmountCR
				, 'INVENTORY' TypeTrans
				, @AccountTypeInvent
				, @TempLmpNo  DocNo
				, 0 StatusReverse
				, '1900-01-01 00:00:00.000' ReverseDate
				, 0 FSend
				, @UserID CreatedBy 
				, GetDate() CreatedDate
			FROM SpTrnSLmpHdr a
			WHERE a.CompanyCode = @CompanyCode
				AND a.BranchCode = @BranchCode
				AND a.LmpNo = @TempLmpNo

			UPDATE GnMstDocument
			SET DocumentSequence = DocumentSequence + 1
				, LastUpdateDate = GetDate()
				, LastUpdateBy = @UserID
			WHERE
				1 = 1
				AND CompanyCode = @CompanyCode
				AND BranchCode = @BranchCode
				AND DocumentType = 'JTS'
				AND ProfitCenterCode = '300'
				AND DocumentYear = Year(GetDate())
		END
		--===============================================================================================================================
		-- END GENERATE JOURNAL Transtype = 1O (TRANSFER STOCK) --
		--===============================================================================================================================
		
		--===============================================================================================================================
		-- GENERATE JOURNAL
		-- NOTES : Transtype = 14 (OTHERS)
		--===============================================================================================================================
		IF (ISNULL((SELECT TransType FROM SpTrnSLmpHdr WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND LmpNo = @TempLmpNo),0) = '14')
		BEGIN
			--===============================================================================================================================
			-- GENERATE GLINTERFACE
			--===============================================================================================================================
			IF (ISNULL((SELECT TOP 1 DocNo FROM GlInterface WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND DocNo = @TempLmpNo), '') <> '')
			BEGIN
				SET @errmsg = 'PEMBUATAN DOKUMEN LAMPIRAN GAGAL ! ' + char(13) + char(13) + 'Error Message: Nomor lampiran sudah ada dalam glInterface, periksa setting-an sequence dokumen (LMP) !'
				RAISERROR (@errmsg,16,1);
				RETURN
			END

			INSERT INTO GLInterface
			SELECT
				a.CompanyCode
				, a.BranchCode
				, a.LmpNo DocNo
				, 1 SeqNo
				, a.LmpDate DocDate
				, '300' ProfitCenterCode
				, GetDate() AccDate
				, @InTransitAccNo AccountNo
				, 'SPAREPART' JournalCode
				, 'BPS' TypeJournal
				, a.LmpNo ApplyTo
				, @Amount AmountDB
				, 0 AmountCR
				, 'INTRANSIT' TypeTrans
				, '' BatchNo
				, '1900-01-01 00:00:00.000' BatchDate
				, 3 StatusFlag
				, @UserID CreateBy 
				, GetDate() CreateDate
				, @UserID UpdateBy
				, GetDate() LastUpdateDate
			FROM SpTrnSLmpHdr a
			WHERE a.CompanyCode = @CompanyCode
				AND a.BranchCode = @BranchCode
				AND a.LmpNo = @TempLmpNo

			INSERT INTO GLInterface
			SELECT
				a.CompanyCode
				, a.BranchCode
				, a.LmpNo DocNo
				, 2 SeqNo
				, a.LmpDate DocDate
				, '300' ProfitCenterCode
				, GetDate() AccDate
				, @InventoryAccNo AccountNo
				, 'SPAREPART' JournalCode
				, 'BPS' TypeJournal
				, a.LmpNo ApplyTo
				, 0 AmountDB	
				, @Amount AmountCR
				, 'INVENTORY' TypeTrans
				, '' BatchNo
				, '1900-01-01 00:00:00.000' BatchDate
				, 3 StatusFlag
				, @UserID CreateBy 
				, GetDate() CreateDate
				, @UserID UpdateBy
				, GetDate() LastUpdateDate
			FROM SpTrnSLmpHdr a
			WHERE a.CompanyCode = @CompanyCode
				AND a.BranchCode = @BranchCode
				AND a.LmpNo = @TempLmpNo

			--===============================================================================================================================
			-- GENERATE GLJOURNAL
			--===============================================================================================================================
			SET @MaxTempJournal = ISNULL((SELECT DocumentSequence + 1 FROM GnMstDocument WHERE 
				CompanyCode = @CompanyCode
					AND BranchCode = @BranchCode
					AND DocumentType = 'JTI' 
					AND ProfitCenterCode = '300' 
					AND DocumentYear = YEAR(GetDate())),0)

			SET @TempJournalPrefix = ISNULL((SELECT DocumentPrefix FROM GnMstDocument WHERE 
				CompanyCode = @CompanyCode
					AND BranchCode = @BranchCode
					AND DocumentType = 'JTI' 
					AND ProfitCenterCode = '300' 
					AND DocumentYear = YEAR(GetDate())),'XXX')

			SET @TempJournal = ISNULL((SELECT @TempJournalPrefix + '/' + RIGHT(YEAR(GETDATE()),2) +'/' + RIGHT(1000000 + CONVERT(VARCHAR, @MaxTempJournal, 1), 6)),'XXX/YY/XXXXXX')
			SET @TempFiscalYear = ISNULL((SELECT FiscalYear FROM GnMstCoProfileSpare WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode),0) 
			SET @TempFiscalMonth  = ISNULL((SELECT FiscalMonth FROM GnMstCoProfileSpare WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode),0) 
			IF (ISNULL((SELECT TOP 1 JournalNo FROM GlJournal WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND JournalNo = @TempJournal), '') <> '')
			BEGIN
				SET @errmsg = 'PEMBUATAN DOKUMEN LAMPIRAN GAGAL ! ' + char(13) + char(13) + 'Error Message: Nomor journal sudah terpakai, periksa setting-an sequence dokumen (JTS) !'
				RAISERROR (@errmsg,16,1);
				RETURN
			END

			INSERT INTO GLJournal
			SELECT
				CompanyCode
				, BranchCode
				, @TempFiscalYear FiscalYear
				, '300' ProfitCenterCode
				, @TempJournal JournalNo
				, 'Harian' JournalType
				, GetDate() JournalDate
				, 'SP' DocSource
				, @TempLmpNo ReffNo
				, GetDate() ReffDate
				, @TempFiscalMonth FiscalMonth
				, @PeriodeNum
				, @Periode
				, @PeriodeName
				, @GLDate
				, 1 BalanceType
				, @Amount AmountDB 
				, @Amount AmountCR
				, 1 Status
				, '' StatusRecon
				, NULL BatchNo
				, '1900-01-01 00:00:00.000' PostingDate 
				, '' StatusReverse
				, '1900-01-01 00:00:00.000' ReverseDate
				, 1 Printseq
				, 0 FSend
				, '' SendBy
				, '1900-01-01 00:00:00.000' SendDate
				, @UserID CreatedBy
				, GetDate() CreatedDate
				, @UserID LastUpdateBy
				, GetDate() LastUpdateDate
			FROM SpTrnSLmpHdr a
			WHERE a.CompanyCode = @CompanyCode
				AND a.BranchCode = @BranchCode
				AND a.LmpNo = @TempLmpNo

			--===============================================================================================================================
			-- GENERATE GLJOURNALDTL
			--===============================================================================================================================
			INSERT INTO GLJournalDtl
			SELECT
				a.CompanyCode CompanyCode
				, a.BranchCode BranchCode
				, @TempFiscalYear FiscalYear
				, @TempJournal JournalNo
				, 1 SeqNo
				, @InTransitAccNo AccountNo
				, @TempLmpNo Description
				, 'Harian' JournalType
				, @Amount AmountDB 
				, 0 AmountCR
				, 'INTRANSIT' TypeTrans
				, @AccountTypeIntran
				, @TempLmpNo  DocNo
				, 0 StatusReverse
				, '1900-01-01 00:00:00.000' ReverseDate
				, 0 FSend
				, @UserID CreatedBy 
				, GetDate() CreatedDate
			FROM SpTrnSLmpHdr a
			WHERE a.CompanyCode = @CompanyCode
				AND a.BranchCode = @BranchCode
				AND a.LmpNo = @TempLmpNo

			INSERT INTO GLJournalDtl
			SELECT
				a.CompanyCode CompanyCode
				, a.BranchCode BranchCode
				, @TempFiscalYear FiscalYear
				, @TempJournal JournalNo
				, 2 SeqNo
				, @InventoryAccNo AccountNo
				, @TempLmpNo  Description
				, 'Harian' JournalType
				, 0 AmountDB
				, @Amount AmountCR
				, 'INVENTORY' TypeTrans
				, @AccountTypeInvent
				, @TempLmpNo  DocNo
				, 0 StatusReverse
				, '1900-01-01 00:00:00.000' ReverseDate
				, 0 FSend
				, @UserID CreatedBy 
				, GetDate() CreatedDate
			FROM SpTrnSLmpHdr a
			WHERE a.CompanyCode = @CompanyCode
				AND a.BranchCode = @BranchCode
				AND a.LmpNo = @TempLmpNo

			UPDATE GnMstDocument
			SET DocumentSequence = DocumentSequence + 1
				, LastUpdateDate = GetDate()
				, LastUpdateBy = @UserID
			WHERE
				1 = 1
				AND CompanyCode = @CompanyCode
				AND BranchCode = @BranchCode
				AND DocumentType = 'JTI'
				AND ProfitCenterCode = '300'
				AND DocumentYear = Year(GetDate())
		END
		--===============================================================================================================================
		-- END GENERATE JOURNAL Transtype = 14 (OTHERS) --
		--===============================================================================================================================

		--===============================================================================================================================
		-- UPDATE TRANSDATE
		--===============================================================================================================================
		update gnMstCoProfileSpare
		set TransDate=getdate()
			, LastUpdateBy=@UserID
			, LastUpdateDate=getdate()
		where CompanyCode = @CompanyCode AND BranchCode = @BranchCode
		
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN
		IF @errmsg = ''
			SET @errmsg = 'PEMBUATAN DOKUMEN LAMPIRAN GAGAL ! ' + char(13) + char(13) + 'Error Message: Terdapat proses yang mem-block proses, harap tunggu beberapa saat kemudian coba lagi !'
		RAISERROR (@errmsg,16,1);
		RETURN
	END CATCH
COMMIT TRAN
END

go

/****** Object:  StoredProcedure [dbo].[uspfn_GetCostPrice]    Script Date: 6/26/2015 8:24:12 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		SDMS.RUDIANA
-- Create date: 2015-06-05
-- Description:	Get CostPrice Value
-- =============================================
ALTER PROCEDURE [dbo].[uspfn_GetCostPrice] (@CompanyCode varchar(15), @BranchCode varchar(15), 
	@PartNo varchar(20), @CostPrice numeric(18,2) output)
AS
BEGIN
	DECLARE @Discount NUMERIC(18,2)
	DECLARE @PurcDiscPct numeric(18,2)
	DECLARE @DiscPct numeric(18,2)

	set @Discount = 0;
    set @PurcDiscPct = (SELECT PurcDiscPct FROM spMstItems WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND PartNo = @PartNo)
	
	DECLARE @TPGO VARCHAR(5)
	set @TPGO = (SELECT TypeOfGoods FROM spMstItems WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND PartNo = @PartNo)

	declare @sql nvarchar(512);

	IF(@TPGO = 2 or @TPGO = 5)
	BEGIN
	IF(@TPGO = 2)
	BEGIN
	set @sql = 'select @CostPrice = (b.RetailPrice - (b.RetailPrice * (' + convert(varchar, 25) + ' *  0.01)))
		from ' + dbo.GetDbMD(@CompanyCode, @BranchCode) + '..spMstItemPrice b 
		where b.CompanyCode =''' + dbo.GetCompanyMD(@CompanyCode, @BranchCode) + '''
		and b.BranchCode =''' + dbo.GetBranchMD(@CompanyCode, @BranchCode) + '''
		and b.PartNo =''' + @PartNo + ''''
	END
	ELSE
	BEGIN
	set @sql = 'select @CostPrice = b.RetailPrice
		from ' + dbo.GetDbMD(@CompanyCode, @BranchCode) + '..spMstItemPrice b 
		where b.CompanyCode =''' + dbo.GetCompanyMD(@CompanyCode, @BranchCode) + '''
		and b.BranchCode =''' + dbo.GetBranchMD(@CompanyCode, @BranchCode) + '''
		and b.PartNo =''' + @PartNo + ''''
	END
	END
	ELSE
	BEGIN
	if(@PurcDiscPct is not null) begin
		--discount from master items
		SET @Discount = @PurcDiscPct
	end
	else begin
        --discount from master supplier
        set @DiscPct = (select DiscPct from gnMstSupplierProfitCenter where CompanyCode = @CompanyCode and BranchCode = @BranchCode
			and SupplierCode = (select dbo.GetBranchMD(@CompanyCode, @BranchCode)) and ProfitCenterCode = '300')
		
		if(@DiscPct is not null)begin
			if(@DiscPct >= 0) begin
				SET @Discount = @Discount + @DiscPct;
			end
		end 
	end
	
	--declare @xCostPrice numeric(18,2)
	set @sql = 'select @CostPrice = (b.RetailPrice - (b.RetailPrice * (' + convert(varchar, @Discount) + ' *  0.01)))
		from ' + dbo.GetDbMD(@CompanyCode, @BranchCode) + '..spMstItemPrice b 
		where b.CompanyCode =''' + dbo.GetCompanyMD(@CompanyCode, @BranchCode) + '''
		and b.BranchCode =''' + dbo.GetBranchMD(@CompanyCode, @BranchCode) + '''
		and b.PartNo =''' + @PartNo + ''''


		--print @sql
		END

	execute sp_executesql @sql, N'@CostPrice numeric(18,2) OUTPUT', @CostPrice = @CostPrice OUTPUT

END
go
ALTER procedure [dbo].[uspfn_SelectPartsAcc]
	@CompanyCode varchar(25),
	@BranchCode varchar(25)
as

begin
	declare @CodeID varchar(25), @sqlstr varchar(max),@DBName varchar(75), @CompanySD varchar(75), @BranchSD varchar(75) ;
	set @CodeID = 'TPGO';
    set @DBName = (select DbName from gnMstCompanyMapping where CompanyMD=@CompanyCode and BranchMD=@BranchCode)
	set @CompanySD = (select CompanyCode from gnMstCompanyMapping where CompanyMD=@CompanyCode and BranchMD=@BranchCode)
	set @BranchSD = (select BranchCode from gnMstCompanyMapping where CompanyMD=@CompanyCode and BranchMD=@BranchCode)
	set  @sqlstr = '
					SELECT itemInfo.PartNo
                    , (item.OnHand - (item.AllocationSP + item.AllocationSR + item.AllocationSL)- (item.ReservedSP + item.ReservedSR + item.ReservedSL))  AS Available
                    , itemPrice.RetailPriceInclTax
                    , itemInfo.PartName
                    , (CASE itemInfo.Status
                        WHEN 1 THEN ''Aktif'' ELSE ''Tidak Aktif''
                       END)  AS Status
                    , dtl.LookUpValueName as JenisPart
                    , itemPrice.RetailPrice  AS NilaiPart
                FROM  spMstItemInfo itemInfo                    
                LEFT JOIN spMstItems item ON item.CompanyCode = itemInfo.CompanyCode
                    AND item.BranchCode = '''+ @BranchCode +'''
                    AND item.PartNo = itemInfo.PartNo
                LEFT JOIN '+@DBName+'..spMstItemPrice itemPrice ON itemPrice.CompanyCode = '''+ @CompanySD + '''
                    AND itemPrice.BranchCode = '''+ @BranchSD +'''
                    AND itemPrice.PartNo = item.PartNo
                LEFT JOIN GnMstLookUpDtl dtl ON item.companyCode = dtl.companyCode 
                    AND dtl.CodeId = '''+@CodeID +'''
                    AND dtl.LookUpValue = item.TypeOfGoods                    
                WHERE itemInfo.CompanyCode = '''+ @CompanyCode +'''
                    AND itemInfo.Status = ''1''
					AND (item.TypeOfGoods = ''2'' OR item.TypeOfGoods = ''5'')
                    AND (item.OnHand - (item.AllocationSP + item.AllocationSR + item.AllocationSL)
                        - (item.ReservedSP + item.ReservedSR + item.ReservedSL)) > 0 
				order by  item.TypeOfGoods desc
			'
print @sqlstr
exec(@sqlstr)
end
go
-- uspfn_OmInquiryChassisReq '6007402','600740200'
ALTER procedure [dbo].[uspfn_OmInquiryChassisReqMD]
	@CompanyCode as varchar(15)
	,@BranchCode as varchar(15)
	,@Penjual as varchar(15)
	,@CBU as bit
as

declare @isDirect as bit,
		@QRYTmp		AS varchar(max),
		@DBMD		AS varchar(25),
		@CompanyMD  AS varchar(25)

set @isDirect=0
if exists (
	select 1
	from gnMstCoProfile
	where CompanyCode=@CompanyCode and BranchCode=@Penjual
)
set @isDirect=1
set @CompanyMD = (SELECT TOP 1 CompanyMD FROM gnMstCompanyMapping WHERE CompanyCode=@CompanyCode AND BranchCode=@BranchCode) 
set @DBMD = (SELECT TOP 1 DbMD FROM gnMstCompanyMapping WHERE CompanyCode=@CompanyCode AND BranchCode=@BranchCode) 

set @QRYTmp =
'select * into #t1
from (
	select distinct isnull(b.BranchCode, e.BranchCode) BranchCode, isnull(c.CustomerCode, e.CustomerCode) CustomerCode
			,z.ChassisCode,z.BPKNo,z.SONo,z.DONo,convert(varchar,z.chassisNo) chassisNo, z.salesModelCode
			, z.salesModelYear, isnull(z.SuzukiDONo,'''') RefferenceDONo,
			isnull(z.SuzukiDODate,''19000101'') RefferenceDODate, isnull(z.SuzukiSJNo,'''') RefferenceSJNo, 
			isnull(z.SuzukiSJDate,''19000101'') RefferenceSJDate, 
			a.EndUserName, a.EndUserAddress1, a.EndUserAddress2, a.EndUserAddress3,
			c.CustomerName, c.Address1, c.Address2, c.Address3,
			c.CityCode,(SELECT LookUpValueName FROM gnMstLookUpDtl where CodeID = ''CITY'' AND ParaValue = c.CityCode) as CityName, 
			c.PhoneNo, c.HPNo, c.birthDate ,b.Salesman, (SELECT EmployeeName FROM HrEmployee where EmployeeID = b.Salesman) SalesmanName, b.SalesType
	from ' + @DBMD + '.dbo.omMstVehicle z 
		left join omTrSalesSOVin a 
			on a.CompanyCode = ''' + @CompanyCode + ''' 
			and z.SONo=a.SONo
				AND a.ChassisCode = z.ChassisCode
				AND a.ChassisNo = z.ChassisNo
		left join omTrSalesSO b
			on a.companyCode = b.companyCode 
				and a.BranchCode= b.BranchCode
				and a.SONo = b.SONo
				and b.Status = ''2'' 
		left join gnMstCustomer c 
			on b.CompanyCode = c.CompanyCode
				and b.CustomerCode = c.CustomerCode 
		left join OmTrSalesDODetail d 
			on d.CompanyCode = z.CompanyCode and z.DONo=d.DONo
				and d.ChassisCode = z.ChassisCode		
				and d.ChassisNo = z.ChassisNo	
		left join OmTrSalesDO e 
			on e.CompanyCode = d.CompanyCode
				and e.DONo = d.DONo
				and e.BranchCode=d.BranchCode
				and e.Status = ''2''
		inner join omMstModel f
			on f.CompanyCode = ''' + @CompanyCode + '''
				and f.SalesModelCode = z.SalesModelCode
	where 
		z.CompanyCode = ''' + @CompanyMD + '''
		and z.ReqOutNo = ''''
		and z.status in (''3'',''4'',''5'',''6'')
		and not exists (select 1 from omTrSalesReqdetail where ChassisCode=z.ChassisCode and ChassisNo=z.ChassisNo)
		and ((case when z.ChassisNo is not null then z.SONo end) is not null 
			or (case when z.ChassisNo is not null then z.DONo end) is not null)
		and f.IsCBU = ' + CONVERT(VARCHAR, @CBU, 1) + '
) #t1

select * from #t1 
drop table #t1'

--where ((case when ' + CONVERT(VARCHAR, @isDirect, 1) + ' = ''1'' then BranchCode end)= ''' + @Penjual + '''
--		or (case when ' + CONVERT(VARCHAR, @isDirect, 1) + ' <> ''1'' then BranchCode end)= ''' + @BranchCode + ''' )

Exec (@QRYTmp);

GO

/****** Object:  StoredProcedure [dbo].[usprpt_OmFakturPajak]    Script Date: 6/30/2015 9:13:42 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER procedure [dbo].[usprpt_OmFakturPajak]
--DECLARE
	@CompanyCode	VARCHAR(15),
	@BranchCode	VARCHAR(15),
	@FPJDateStart DATETIME,
	@FPJDateEnd DATETIME,
	@FPJNoStart	VARCHAR(30),
	@FPJNoEnd	VARCHAR(30),
	@SignName VARCHAR(100),
	@TitleSign VARCHAR(100),
	@Param bit = 1
AS
BEGIN

--'6114201','611420100','20150501','20150530','010.001-15.70827239','010.001-15.70827239','Wiwik W','Pimpinan FAD',True

--SELECT @CompanyCode = '6114201',@BranchCode='611420100',@FPJDateStart='20150501',@FPJDateEnd='20150630',@FPJNoStart='010.001-15.70827992',
--@FPJNoEnd='010.001-15.70827992',@SignName='Wiwik W',@TitleSign='Pimpinan FAD',@Param=1

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
	declare @fInv		int
	
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
set @fInv		= (select isnull(ParaValue,'1') from gnmstlookupdtl where codeid='FPJFLAG' and lookupvalue='SALES')		
-- parameter use address holding or not
declare @IsHoldingAddr as bit
if (select count(*) from gnMstLookUpDtl where CompanyCode= @CompanyCode and CodeID='FPJADDR') > 0
	set @IsHoldingAddr = (select convert(numeric,LookUpValue) from gnMstLookUpDtl where CompanyCode= @CompanyCode and CodeID='FPJADDR')
else
	set @IsHoldingAddr = 0

-- parameter to show info or not
declare @IsShowInfo as bit
if (select count(*) from gnMstLookUpDtl where CompanyCode= @CompanyCode and CodeID='FPIF' and LookUpValue='STATUS') > 0
	set @IsShowInfo = (select convert(numeric,ParaValue) from gnMstLookUpDtl where CompanyCode= @CompanyCode and CodeID='FPIF' and LookUpValue='STATUS')
else
	set @IsShowInfo = 1
	
	
	declare @tabData as table
	(
		CompanyCode varchar(max),
		BranchCode varchar(max),
		InvoiceNo varchar(max),
		ItemOrder varchar(max),
		ItemCode varchar(max),
		ItemName varchar(max),
		PPnBM decimal(18,2),
		PPnBMSell decimal(18,2),
		ItemQuantity decimal(5,2),
		ItemDPP decimal(18,2),
		Potongan decimal(18,2),
		TaxPct decimal(18,2),
		AfterDiscPpn  decimal(18,2)
	)
	IF (@Param=1)
	BEGIN
		-- Sembunyikan Detail Part .....
		SELECT * INTO #t1 FROM (
		SELECT 
			acc.CompanyCode
			, acc.BranchCode
			, acc.InvoiceNo
			, acc.PartNo AS ItemCode
			, ISNULL((acc.Quantity)/(select count(chassisno) from omFakturPajakDetail where companycode=acc.companycode 
						and branchcode=acc.branchcode and invoiceno=acc.invoiceno) * acc.RetailPrice,0) AS ItemDPP
			, ISNULL((acc.Quantity)/(select count(chassisno) from omFakturPajakDetail where companycode=acc.companycode 
						and branchcode=acc.branchcode and invoiceno=acc.invoiceno) * acc.DiscExcludePPn, 0) AS Potongan
			, 0 AS TaxPct
		FROM 
			omFakturPajakDetailAccsSeq acc 
		INNER JOIN omFakturPajakhdr hdr ON hdr.CompanyCode = acc.CompanyCode
			AND hdr.BranchCode = acc.BranchCode
			AND hdr.InvoiceNo = acc.InvoiceNo
			AND ((CASE WHEN @FPJNoStart = '' THEN hdr.FakturPajakNo END) <> ''
				OR (CASE WHEN @FPJNoStart <> '' THEN hdr.FakturPajakNo END) BETWEEN @FPJNoStart AND @FPJNoEnd)
			AND CONVERT(VARCHAR, hdr.FakturPajakDate, 112) BETWEEN CONVERT(VARCHAR, @FPJDateStart, 112) AND CONVERT(VARCHAR, @FPJDateEnd, 112)
			AND hdr.TaxType = 'Standard'
		WHERE 
			acc.CompanyCode = @CompanyCode AND acc.BranchCode = @BranchCode	)#t1
				
		select * into #Others from(
				Select a.CompanyCode
					 , a.BranchCode
					 , a.InvoiceNo
					 , a.SalesModelCode
					 , ISNULL((a.Quantity)/(select count(chassisno) from omFakturPajakDetail where companycode=a.companycode 
						and branchcode=a.branchcode and invoiceno=a.invoiceno) * a.DPP, 0) ItemDPP
					 , 0 Potongan
				from omFakturPajakDetailOthers a
				left join omFakturPajakDetail b on a.CompanyCode = b.CompanyCode
					  and a.BranchCode = b.BranchCode
					  and a.InvoiceNo = b.InvoiceNo
					  and a.SalesModelCode = b.SalesModelCode
				INNER JOIN omFakturPajakhdr hdr ON hdr.CompanyCode = a.CompanyCode
					   AND hdr.BranchCode = a.BranchCode
					   AND hdr.InvoiceNo = a.InvoiceNo
					   AND ((CASE WHEN @FPJNoStart = '' THEN hdr.FakturPajakNo END) <> ''
					   	   OR (CASE WHEN @FPJNoStart <> '' THEN hdr.FakturPajakNo END) BETWEEN @FPJNoStart AND @FPJNoEnd)
					   AND CONVERT(VARCHAR, hdr.FakturPajakDate, 112) BETWEEN CONVERT(VARCHAR, @FPJDateStart, 112) AND CONVERT(VARCHAR, @FPJDateEnd, 112)
					   AND hdr.TaxType = 'Standard'
				where a.CompanyCode = @CompanyCode
				  and a.BranchCode = @BranchCode
		)#Others
		
		--SELECT * INTO #t2 FROM (
		--SELECT
		--	CompanyCode 
		--	, BranchCode
		--	, InvoiceNo 
		--	, SUM(ItemDPP) ItemDPP
		--	, SUM(Potongan) Potongan
		--FROM #t1 GROUP BY CompanyCode, BranchCode, InvoiceNo ) #t2
		
		SELECT CompanyCode 
				, BranchCode
				, InvoiceNo 
				, SUM(ItemDPP) ItemDPP
				, SUM(Potongan) Potongan
		INTO #t2 FROM (
			SELECT
				CompanyCode 
				, BranchCode
				, InvoiceNo 
				, SUM(ItemDPP) ItemDPP
				, SUM(Potongan) Potongan
			FROM #t1 GROUP BY CompanyCode, BranchCode, InvoiceNo
		UNION
			SELECT
				CompanyCode 
				, BranchCode
				, InvoiceNo 
				, SUM(ItemDPP) ItemDPP
				, SUM(Potongan) Potongan
			FROM #Others a
			GROUP BY a.CompanyCode, a.BranchCode, a.InvoiceNo 
		) #t2
		group by CompanyCode,BranchCode,InvoiceNo
		
		-- INSER DATA KE TABEL RESULT
		INSERT INTO @tabData 
		-- Unit
		SELECT 
			mdl.CompanyCode
			, mdl.BranchCode
			, mdl.InvoiceNo
			, '1' AS ItemOrder
			, mdl.SalesModelCode AS ItemCode
			, LEFT(CONVERT(VARCHAR, mdl.ChassisNo) + '             ', 13) +  LEFT(CONVERT(VARCHAR, mdl.EngineNo) + '           ', 11) AS ItemName
			, ISNULL(Vec.PPnBMBuyPaid,0) AS PPnBM
			, mdl.AfterDiscPPnBM AS PPnBMSell
			, 1 AS ItemQuantity
			, mdl.BeforeDiscDPP + (ISNULL(t.ItemDPP,0)) AS ItemDPP
			, mdl.DiscExcludePPN + (ISNULL(t.Potongan,0)) AS Potongan
			, ISNULL((SELECT ISNULL(TaxPct, 0) FROM  GnMstTax INNER JOIN OmMstModel ON OmMstModel.CompanyCode = GnMstTax.CompanyCode AND OmMstModel.PPnBMCodeSell = GnMstTax.TaxCode AND OmMstModel.SalesModelCode = mdl.SalesModelCode), 0) AS TaxPct
			, mdl.AfterDiscPpn		
		FROM 
			omFakturPajakDetail mdl 
		INNER JOIN omFakturPajakhdr hdr ON hdr.CompanyCode = mdl.CompanyCode
			AND hdr.BranchCode = mdl.BranchCode
			AND hdr.InvoiceNo = mdl.InvoiceNo
			AND ((CASE WHEN @FPJNoStart = '' THEN hdr.FakturPajakNo END) <> ''
				OR (CASE WHEN @FPJNoStart <> '' THEN hdr.FakturPajakNo END) BETWEEN @FPJNoStart AND @FPJNoEnd)
			AND CONVERT(VARCHAR, hdr.FakturPajakDate, 112) BETWEEN CONVERT(VARCHAR, @FPJDateStart, 112) AND CONVERT(VARCHAR, @FPJDateEnd, 112)
			AND hdr.TaxType = 'Standard'
		LEFT JOIN OmMstVehicle vec ON mdl.CompanyCode = vec.CompanyCode 
			AND mdl.ChassisCode = vec.ChassisCode 
			AND mdl.ChassisNo = vec.ChassisNo
		LEFT JOIN #t2 t ON t.CompanyCode = mdl.CompanyCode
			AND t.BranchCode = mdl.BranchCode
			AND t.InvoiceNo = mdl.InvoiceNo
		WHERE 
			mdl.CompanyCode = @CompanyCode AND mdl.BranchCode = @BranchCode 
		UNION ALL
		SELECT distinct
			hdr.CompanyCode
			, hdr.BranchCode
			, hdr.InvoiceNo
			, '2' AS ItemOrder
			, 'SPAREPART/MATERIAL' AS ItemCode
			, '' AS ItemName
			, 0 AS PPnBM
			, 0 AS PPnBMSell
			, 0 AS ItemQuantity
			, 0 AS ItemDPP
			, 0 AS Potongan
			, 0 AS TaxPct
			, 0 AS AfterDiscPpn
		FROM omFakturPajakDetailAccsSeq acc
		INNER JOIN omFakturPajakHdr hdr ON hdr.CompanyCode=acc.CompanyCode
			AND hdr.BranchCode=acc.BranchCode
			AND hdr.InvoiceNo=acc.InvoiceNo
		WHERE hdr.CompanyCode = @CompanyCode
			AND hdr.BranchCode = @BranchCode
			AND ((CASE WHEN @FPJNoStart = '' THEN hdr.FakturPajakNo END) <> ''
				OR (CASE WHEN @FPJNoStart <> '' THEN hdr.FakturPajakNo END) BETWEEN @FPJNoStart AND @FPJNoEnd)
			AND CONVERT(VARCHAR, hdr.FakturPajakDate, 112) BETWEEN CONVERT(VARCHAR, @FPJDateStart, 112) AND CONVERT(VARCHAR, @FPJDateEnd, 112)
			AND hdr.TaxType = 'Standard'
		UNION ALL
		-- SPAREPART/MATERIAL
		SELECT 
			acc.CompanyCode
			, acc.BranchCode
			, acc.InvoiceNo
			, '3' AS ItemOrder
			, acc.PartNo AS ItemCode
			, acc.PartName AS ItemName
			, 0 AS PPnBM
			, 0 AS PPnBMSell
			, 0 AS ItemQuantity
			, 0 AS ItemDPP
			, 0 AS Potongan
			, 0 AS TaxPct
			, 0 AS AfterDiscPpn
		FROM 
			omFakturPajakDetailAccsSeq acc 
		INNER JOIN omFakturPajakhdr hdr ON hdr.CompanyCode = acc.CompanyCode
			AND hdr.BranchCode = acc.BranchCode
			AND hdr.InvoiceNo = acc.InvoiceNo
			AND ((CASE WHEN @FPJNoStart = '' THEN hdr.FakturPajakNo END) <> ''
				OR (CASE WHEN @FPJNoStart <> '' THEN hdr.FakturPajakNo END) BETWEEN @FPJNoStart AND @FPJNoEnd)
			AND CONVERT(VARCHAR, hdr.FakturPajakDate, 112) BETWEEN CONVERT(VARCHAR, @FPJDateStart, 112) AND CONVERT(VARCHAR, @FPJDateEnd, 112)
			AND hdr.TaxType = 'Standard'
		WHERE 
			acc.CompanyCode = @CompanyCode AND acc.BranchCode = @BranchCode	
		UNION ALL
		SELECT 
			oth.CompanyCode
			, oth.BranchCode
			, oth.InvoiceNo
			, '4' AS ItemOrder
			, oth.OtherCode AS ItemCode
			, oth.OtherName AS ItemName
			, 0 AS PPnBM
			, 0 AS PPnBMSell
			, 0 AS ItemQuantity
			, 0 AS ItemDPP
			, 0 AS Potongan
			, 0 AS TaxPct
			, 0 AS AfterDiscPpn			
		FROM 
			omFakturPajakDetailOthers oth 
		INNER JOIN omFakturPajakhdr hdr ON hdr.CompanyCode = oth.CompanyCode
			AND hdr.BranchCode = oth.BranchCode
			AND hdr.InvoiceNo = oth.InvoiceNo
			AND ((CASE WHEN @FPJNoStart = '' THEN hdr.FakturPajakNo END) <> ''
				OR (CASE WHEN @FPJNoStart <> '' THEN hdr.FakturPajakNo END) BETWEEN @FPJNoStart AND @FPJNoEnd)
			AND CONVERT(VARCHAR, hdr.FakturPajakDate, 112) BETWEEN CONVERT(VARCHAR, @FPJDateStart, 112) AND CONVERT(VARCHAR, @FPJDateEnd, 112)
			AND hdr.TaxType = 'Standard'
		WHERE 
			oth.CompanyCode = @CompanyCode AND oth.BranchCode = @BranchCode	
							 
		DROP TABLE #t1
		DROP TABLE #t2			
	END
	ELSE
	BEGIN
		-- Tampilkan Part --
		--------------------
		
		-- INSER DATA KE TABEL RESULT
		INSERT INTO @tabData 
		-- Unit
		SELECT 
			mdl.CompanyCode
			, mdl.BranchCode
			, mdl.InvoiceNo
			, '1' AS ItemOrder
			, mdl.SalesModelCode AS ItemCode
			, LEFT(CONVERT(VARCHAR, mdl.ChassisNo) + '             ', 13) +  LEFT(CONVERT(VARCHAR, mdl.EngineNo) + '           ', 11) AS ItemName
			, ISNULL(Vec.PPnBMBuyPaid,0) AS PPnBM
			, mdl.AfterDiscPPnBM AS PPnBMSell
			, 1 AS ItemQuantity
			, mdl.BeforeDiscDPP AS ItemDPP
			, mdl.DiscExcludePPN Potongan
			, ISNULL((SELECT ISNULL(TaxPct, 0) FROM  GnMstTax INNER JOIN OmMstModel ON OmMstModel.CompanyCode = GnMstTax.CompanyCode AND OmMstModel.PPnBMCodeSell = GnMstTax.TaxCode AND OmMstModel.SalesModelCode = mdl.SalesModelCode), 0) AS TaxPct
			, mdl.AfterDiscPpn
		FROM 
			omFakturPajakDetail mdl 
		INNER JOIN omFakturPajakhdr hdr ON hdr.CompanyCode = mdl.CompanyCode
			AND hdr.BranchCode = mdl.BranchCode
			AND hdr.InvoiceNo = mdl.InvoiceNo
			AND ((CASE WHEN @FPJNoStart = '' THEN hdr.FakturPajakNo END) <> ''
				OR (CASE WHEN @FPJNoStart <> '' THEN hdr.FakturPajakNo END) BETWEEN @FPJNoStart AND @FPJNoEnd)
			AND CONVERT(VARCHAR, hdr.FakturPajakDate, 112) BETWEEN CONVERT(VARCHAR, @FPJDateStart, 112) AND CONVERT(VARCHAR, @FPJDateEnd, 112)
			AND hdr.TaxType = 'Standard'
		LEFT JOIN OmMstVehicle vec ON mdl.CompanyCode = vec.CompanyCode 
			AND mdl.ChassisCode = vec.ChassisCode 
			AND mdl.ChassisNo = vec.ChassisNo
		WHERE 
			mdl.CompanyCode = @CompanyCode AND mdl.BranchCode = @BranchCode 
		UNION ALL
		SELECT distinct
			hdr.CompanyCode
			, hdr.BranchCode
			, hdr.InvoiceNo
			, '2' AS ItemOrder
			, 'SPAREPART/MATERIAL' AS ItemCode
			, '' AS ItemName
			, 0 AS PPnBM
			, 0 AS PPnBMSell
			, 0 AS ItemQuantity
			, 0 AS ItemDPP
			, 0 AS Potongan
			, 0 AS TaxPct
			, 0 AS AfterDiscPpn
		FROM omFakturPajakDetailAccsSeq acc
		INNER JOIN omFakturPajakHdr hdr ON hdr.CompanyCode=acc.CompanyCode
			AND hdr.BranchCode=acc.BranchCode
			AND hdr.InvoiceNo=acc.InvoiceNo
		WHERE hdr.CompanyCode = @CompanyCode
			AND hdr.BranchCode = @BranchCode
			AND ((CASE WHEN @FPJNoStart = '' THEN hdr.FakturPajakNo END) <> ''
				OR (CASE WHEN @FPJNoStart <> '' THEN hdr.FakturPajakNo END) BETWEEN @FPJNoStart AND @FPJNoEnd)
			AND CONVERT(VARCHAR, hdr.FakturPajakDate, 112) BETWEEN CONVERT(VARCHAR, @FPJDateStart, 112) AND CONVERT(VARCHAR, @FPJDateEnd, 112)
			AND hdr.TaxType = 'Standard'
		UNION ALL
		-- SPAREPART/MATERIAL
		SELECT 
			acc.CompanyCode
			, acc.BranchCode
			, acc.InvoiceNo
			, '3' AS ItemOrder
			, acc.PartNo AS ItemCode
			, acc.PartName AS ItemName
			, 0 AS PPnBM
			, 0 AS PPnBMSell
			, acc.Quantity AS ItemQuantity
			, (acc.Quantity * acc.RetailPrice) AS ItemDPP
			, (acc.Quantity * acc.DiscExcludePPn) AS Potongan
			, 0 AS TaxPct
			, 0 AS AfterDiscPpn			
		FROM 
			omFakturPajakDetailAccsSeq acc 
		INNER JOIN omFakturPajakhdr hdr ON hdr.CompanyCode = acc.CompanyCode
			AND hdr.BranchCode = acc.BranchCode
			AND hdr.InvoiceNo = acc.InvoiceNo
			AND ((CASE WHEN @FPJNoStart = '' THEN hdr.FakturPajakNo END) <> ''
				OR (CASE WHEN @FPJNoStart <> '' THEN hdr.FakturPajakNo END) BETWEEN @FPJNoStart AND @FPJNoEnd)
			AND CONVERT(VARCHAR, hdr.FakturPajakDate, 112) BETWEEN CONVERT(VARCHAR, @FPJDateStart, 112) AND CONVERT(VARCHAR, @FPJDateEnd, 112)
			AND hdr.TaxType = 'Standard'
		WHERE 
			acc.CompanyCode = @CompanyCode AND acc.BranchCode = @BranchCode	
		UNION ALL
		SELECT 
			oth.CompanyCode
			, oth.BranchCode
			, oth.InvoiceNo
			, '4' AS ItemOrder
			, oth.OtherCode AS ItemCode
			, oth.OtherName AS ItemName
			, 0 AS PPnBM
			, 0 AS PPnBMSell
			, oth.Quantity AS ItemQuantity
			, (oth.Quantity * oth.DPP) AS ItemDPP
			, 0 AS Potongan
			, 0 AS TaxPct
			, 0 AS AfterDiscPpn			
		FROM 
			omFakturPajakDetailOthers oth 
		INNER JOIN omFakturPajakhdr hdr ON hdr.CompanyCode = oth.CompanyCode
			AND hdr.BranchCode = oth.BranchCode
			AND hdr.InvoiceNo = oth.InvoiceNo
			AND ((CASE WHEN @FPJNoStart = '' THEN hdr.FakturPajakNo END) <> ''
				OR (CASE WHEN @FPJNoStart <> '' THEN hdr.FakturPajakNo END) BETWEEN @FPJNoStart AND @FPJNoEnd)
			AND CONVERT(VARCHAR, hdr.FakturPajakDate, 112) BETWEEN CONVERT(VARCHAR, @FPJDateStart, 112) AND CONVERT(VARCHAR, @FPJDateEnd, 112)
			AND hdr.TaxType = 'Standard'
		WHERE 
			oth.CompanyCode = @CompanyCode AND oth.BranchCode = @BranchCode	
	END

SELECT * INTO #hasil FROM (
SELECT
	a.TaxType AS TaxType
	,a.InvoiceNo AS InvoiceNo
	,a.InvoiceDate AS InvoiceDate
	,a.FakturPajakNo AS FPJNo
	,(SELECT dbo.GetDateIndonesian (CONVERT(VARCHAR,a.FakturPajakDate, 101))) AS FPJDate
	,a.CustomerCode AS fakturCustCode
	,case when @fInfoPKP = 1 then
		(case @fStatus when '1' then @fCompName else e.CompanyGovName end)
	 else '' end AS CompanyName
	,case @fStatus when '1' then @fSKP else e.SKPNo end AS compSKPNo
	,case @fStatus when '1' then @fSKPDate else e.SKPDate end AS compSKPDate
	,case when @fInfoPKP = 1 then
		(case @fStatus when '1' then @fAdd else 
			(case when @IsHoldingAddr=0 then ISNULL(e.Address1,'') + ' ' + ISNULL(e.Address2,'') + ' ' + ISNULL(e.Address3,'') + ' ' + ISNULL(e.Address4,'')
				else (select ISNULL(Address1,'') + ' ' + ISNULL(Address2,'') + ' ' + ISNULL(Address3,'') + ' ' + ISNULL(Address4,'') from gnMstCoProfile where CompanyCode=@CompanyCode
				and BranchCode=(select BranchCode from gnMstOrganizationDtl where CompanyCode=@CompanyCode and IsBranch=0))end ) end)
		 else '' end AS compAddr	 
	,e.PhoneNo AS compPhoneNo
	,e.FaxNo AS compFaxNo
	,case when @fInfoPKP = 1 then
		(case @fStatus when '1' then @fNPWP else e.NPWPNo end)
	 else '' end AS compNPWPNo
	,d.CustomerGovName AS CustomerName
	,d.SKPNo AS custSKPNo
	,d.SKPDate AS custSKPDate
	,ISNULL(d.Address1,'') AS custAddr1
	,ISNULL(d.Address2,'') AS custAddr2
	,ISNULL(d.Address3,'')+ ' ' + ISNULL(d.Address4,'') AS custAddr3
	,d.PhoneNo AS custPhoneNo
	,d.FaxNo AS custFaxNo
	,d.NPWPNo AS custNPWPNo
	,a.DueDate AS fakturDueDate
	,a.DiscAmt AS DiscAmt
	,a.DppAmt AS DppAmt
	,a.PPNAmt AS PPNAmt
	,a.DppAmt - a.DiscAmt AS SubAmt
	,a.TotalAmt AS TotalAmt
	,a.TotQuantity
	,a.PPnBMPaid
	,case @fStatus when '1' then @fCity else 
		(SELECT LookUpValueName FROM gnMstLookupDtl WHERE CodeID = 'CITY' AND LookUpValue = e.CityCode) end as cityNm
	,ISNULL(@SignName, '') AS TaxPerson
	,ISNULL(@TitleSign,'') AS JobTitle
	,'Model              No.Rangka    No.Mesin            PPnBM' AS ItemHeader
	,dtl.ItemOrder
	,dtl.ItemCode
	,dtl.ItemName
	,dtl.PPnBM
	,dtl.ItemQuantity
	,dtl.ItemDPP
	,dtl.Potongan
	,dtl.AfterDiscPpn
	,CASE WHEN copro.ProductType = '2W' THEN 
		(CASE WHEN so.SalesType = '0' THEN 
			(SELECT LookupValuename FROM gnMstLookUpDtl WHERE CodeID = 'FPJN' AND  LookupValue = '2WWS') 
				ELSE (SELECT LookupValuename FROM gnMstLookUpDtl WHERE CodeID = 'FPJN' and  LookupValue = '2WDS') END) 
		ELSE 
		(CASE WHEN so.SalesType = '0' THEN 
			(SELECT LookupValuename FROM gnMstLookUpDtl WHERE CodeID = 'FPJN' AND  LookupValue = '4WWS') 
				ELSE (SELECT LookupValuename FROM gnMstLookUpDtl WHERE CodeID = 'FPJN' and  LookupValue = '4WDS') END)  
	END AS Keterangan
	,@IsShowInfo as FlagShowInfo
	,@param HidePart
	,dtl.TaxPct
	,dtl.PPnBMSell
FROM 
	omFakturPajakHdr a
LEFT JOIN GnMstCustomer d ON a.CompanyCode = d.CompanyCode 
	AND a.CustomerCode = d.CustomerCode
LEFT JOIN GnMstCoProfile e ON a.CompanyCode = e.CompanyCode 
	AND a.BranchCode = e.BranchCode
INNER JOIN @tabData dtl ON dtl.CompanyCode = a.CompanyCode
	AND dtl.BranchCode = a.BranchCode
	AND dtl.InvoiceNo = a.InvoiceNo
LEFT JOIN omTrSalesInvoice inv ON inv.CompanyCode = a.CompanyCode 
	AND inv.BranchCode = a.BranchCode 
	AND inv.InvoiceNo = a.InvoiceNo
LEFT JOIN omTrSalesSO so	ON a.CompanyCode = so.CompanyCode
	AND a.BranchCode = so.BranchCode 
	AND inv.SONo = so.SONo
LEFT JOIN GnMstCoProfile copro ON a.CompanyCode = copro.CompanyCode 
	AND a.BranchCode = copro.BranchCode
WHERE  
	a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode ) #hasil

if (@param=1)
	BEGIN
	SELECT TaxType, InvoiceNo, InvoiceDate, FPJNo, FPJDate, fakturCustCode, CompanyName, compSKPNo, compSKPDate, compAddr, compPhoneNo, compFaxNo, compNPWPNo
		, CustomerName, custSKPNo, custSKPDate, custAddr1, custAddr2, custAddr3, custPhoneNo, custFaxNo, custNPWPNo, fakturDueDate, DiscAmt, DPPAmt
		--, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select round(((sum(ItemDPP) - sum(Potongan)) * 0.1),0) from #hasil where FPJNo = c.FPJNo) ELSE PPNAmt END PPNAmt
		, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select sum(AfterDiscPpn) from #hasil where FPJNo = c.FPJNo) ELSE PPNAmt END PPNAmt
		, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select sum(ItemDPP) - sum(Potongan) from #hasil where FPJNo = c.FPJNo ) ELSE SubAmt END SubAmt
		--, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select  (sum(ItemDPP) - sum(Potongan)) + round(((sum(ItemDPP) - sum(Potongan)) * 0.1),0) from #hasil where FPJNo = c.FPJNo ) ELSE TotalAmt END TotalAmt
		--, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select  (sum(ItemDPP) - sum(Potongan)) + sum(AfterDiscPpn) from #hasil where FPJNo = c.FPJNo ) ELSE TotalAmt END TotalAmt
		--, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select sum(TotalAmt) from #hasil where FPJNo = c.FPJNo ) ELSE TotalAmt END TotalAmt
		, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select sum(AfterDiscPpn) + (sum(ItemDPP) - sum(Potongan)) from #hasil where FPJNo = c.FPJNo ) ELSE TotalAmt END TotalAmt
		, TotQuantity, PPNBMPaid, cityNm, TaxPerson, JobTitle, ItemHeader, ItemOrder, ItemCode, ItemName, PPnBM, ItemQuantity, ItemDPP, Potongan, Keterangan
		, FlagShowInfo, HidePart, TaxPct, PPnBMSell
		, (SELECT COUNT(INVOICENO) FROM #hasil WHERE FPJNo = c.FPJNo) MaxRow
		, (select sum(ItemQuantity) from #hasil where FPJNo=c.FPJNo and ItemOrder='1') SumQty
		, (select sum(ItemDPP) from #hasil where FPJNo = c.FPJNo ) XAmt
		, (select sum(Potongan) from #hasil where FPJNo = c.FPJNo ) XPotongan
		, @fInv ViewInvoice
	FROM #hasil c order by c.InvoiceNo, c.ItemOrder
	END
else
	SELECT TaxType, InvoiceNo, InvoiceDate, FPJNo, FPJDate, fakturCustCode, CompanyName, compSKPNo, compSKPDate, compAddr, compPhoneNo, compFaxNo, compNPWPNo
		, CustomerName, custSKPNo, custSKPDate, custAddr1, custAddr2, custAddr3, custPhoneNo, custFaxNo, custNPWPNo, fakturDueDate, DiscAmt, DPPAmt
		--, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select round(((sum(ItemDPP) - sum(Potongan)) * 0.1),0) from #hasil where FPJNo = c.FPJNo) ELSE PPNAmt END PPNAmt
		, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select sum(AfterDiscPpn) from #hasil where FPJNo = c.FPJNo) ELSE PPNAmt END PPNAmt
		, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select sum(ItemDPP) - sum(Potongan) from #hasil where FPJNo = c.FPJNo ) ELSE SubAmt END SubAmt
		--, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select  (sum(ItemDPP) - sum(Potongan)) + round(((sum(ItemDPP) - sum(Potongan)) * 0.1),0) from #hasil where FPJNo = c.FPJNo ) ELSE TotalAmt END TotalAmt
		--, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select  (sum(ItemDPP) - sum(Potongan)) + sum(AfterDiscPpn) from #hasil where FPJNo = c.FPJNo ) ELSE TotalAmt END TotalAmt
		--, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select sum(TotalAmt) from #hasil where FPJNo = c.FPJNo ) ELSE TotalAmt END TotalAmt
		, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select sum(AfterDiscPpn) + (sum(ItemDPP) - sum(Potongan)) from #hasil where FPJNo = c.FPJNo ) ELSE TotalAmt END TotalAmt
		, TotQuantity, PPNBMPaid, cityNm, TaxPerson, JobTitle, ItemHeader, ItemOrder, ItemCode, ItemName, PPnBM, ItemQuantity, ItemDPP, Potongan, Keterangan
		, FlagShowInfo, HidePart, TaxPct, PPnBMSell
		, (SELECT COUNT(INVOICENO) FROM #hasil WHERE FPJNo = c.FPJNo) MaxRow
		, (select sum(ItemQuantity) from #hasil where FPJNo=c.FPJNo) SumQty
		, (select sum(ItemDPP) from #hasil where FPJNo = c.FPJNo ) XAmt
		, (select sum(Potongan) from #hasil where FPJNo = c.FPJNo ) XPotongan
		, @fInv ViewInvoice
	FROM #hasil c order by c.InvoiceNo, c.ItemOrder

DROP TABLE #hasil, #Others--, #t1
END

GO



ALTER procedure [dbo].[uspfn_SvInqFpjData]    
		 @CompanyCode nvarchar(20),    
		 @BranchCode nvarchar(20),    
		 @DocPrefix nvarchar(20),    
		 @IsPdi bit,    
		 @IsFsc bit,    
		 @IsFSCCampaign bit,
		 @IsSprClaim bit = 0,    
		 @CustBill nvarchar(20)    
    
as    

--set @CompanyCode = '6006406'
--set @BranchCode = '6006402'
--set	@DocPrefix = 'INF'
--set	@IsPdi = 0
--set	@IsFsc = 0
--set	@IsFSCCampaign = 1
--set	@IsSprClaim = 0
--set	@CustBill = ''
    
select row_number() over(order by a.InvoiceNo) RowNum    
     , convert(bit, 1) IsSelected    
     , a.CompanyCode    
     , a.BranchCode    
     , a.InvoiceNo    
     , a.InvoiceDate    
     , a.JobOrderNo    
     , a.JobOrderDate    
     , a.TotalDPPAmt    
     , isnull(a.TotalPPHAmt, 0) + isnull(TotalPPNAmt, 0) as TotalPpnAmt         
     , a.TotalSrvAmt    
     , a.JobType    
     , a.PoliceRegNo    
     , a.BasicModel    
     , a.ChassisCode    
     , a.ChassisNo    
     , a.EngineCode    
     , a.EngineNo    
     , a.TOPCode    
     , a.TOPDays    
     , a.DueDate    
     , a.FPJNo    
     , a.FPJDate    
     , a.CustomerCodeBill    
     , a.Odometer    
     , a.IsPkp    
     , a.CustomerCode    
     , a.CustomerCode + ' - ' + b.CustomerName Pelanggan    
     , a.CustomerCodeBill + ' - ' + c.CustomerName Pembayar    
     , a.DueDate    
     , isnull(d.IsSparepartClaim, 0) IsSparepartClaim    
     , isnull(e.CampaignFlag, 0) CampaignFlag
  from svTrnInvoice a with(nolock, nowait)    
	left join gnMstCustomer b with(nolock, nowait)    
		on b.CompanyCode = a.CompanyCode    
		and b.CustomerCode = a.CustomerCode    
	left join gnMstCustomer c with(nolock, nowait)    
		on c.CompanyCode = a.CompanyCode    
		and c.CustomerCode = a.CustomerCodeBill    
	left join svTrnService d with(nolock, nowait)    
		on d.CompanyCode = a.CompanyCode    
		and d.BranchCode = a.BranchCode    
		and d.JobOrderNo = a.JobOrderNo    
	left join svMstFscCampaign e 
		on a.ChassisCode = e.ChassisCode
		and a.ChassisNo = e.ChassisNo
	--left join svTrnSrvTask f							--Penambahan
	--	on f.CompanyCode = d.CompanyCode				--Penambahan
	--	and f.BranchCode = d.BranchCode					--Penambahan
	--	and f.ServiceNo = d.ServiceNo					--Penambahan
	--left join svMstBillingType g						--Penambahan
	--	on g.CompanyCode = f.CompanyCode				--Penambahan
	--	and g.BillType = f.BillType						--Penambahan
	--	and g.CustomerCode = a.CustomerCodeBill			--Penambahan
 where 1 = 1    
		and a.CompanyCode = @CompanyCode     
		and a.BranchCode = @BranchCode     
		and isnull(a.InvoiceStatus, '0') = '0'    
		and isnull(a.FPJNo, '') = ''    
		and left(a.InvoiceNo, 3) in (@DocPrefix, case @DocPrefix when 'INC' then 'INP' else '' end)
		--and g.CustomerCode = a.CustomerCodeBill			--Penambahan
   -- if INF check PDI or FSC or all    
		and a.JobType like (    
       case     
         when (@DocPrefix = 'INF' and isnull(@IsPdi, 0) = 1) then 'PDI%'    
         when (@DocPrefix = 'INF' and isnull(@IsFsc, 0) = 1) then 'FSC%'    
         when (@DocPrefix = 'INF' and isnull(@IsFscCampaign, 0) = 1) then 'FSC%'    
         else '%'    
       end)       
   -- if INF check FSC Campaign       
   and isnull(e.CampaignFlag,0) =
	(case when (@DocPrefix = 'INF' and isnull(@IsFscCampaign, 0) = 1) then e.CampaignFlag else 0 end)
   -- if INW check SrvClaim or SprClaim    
   and isnull(d.IsSparepartClaim, 0) = (    
       case     
         when @DocPrefix = 'INW' then @IsSprClaim    
         else isnull(d.IsSparepartClaim, 0)    
       end)    
   -- if INC check customer bill    
   and a.CustomerCodeBill like (    
       case     
         when @DocPrefix in  ('INC','INP') then @CustBill    
         else '%'    
       end)    
go

ALTER procedure [dbo].[uspfn_SelectPartsAcc]
	@CompanyCode varchar(25),
	@BranchCode varchar(25),
	@SONo varchar(25)
as

begin
	declare @CodeID varchar(25), @sqlstr varchar(max),@DBMD varchar(75), @CompanyMD varchar(75), @BranchMD varchar(75) ;
	set @CodeID = 'TPGO';
    set @DBMD = (select TOP 1 DbMD from gnMstCompanyMapping where CompanyCode=@CompanyCode and BranchCode=@BranchCode)
    set @CompanyMD = (select TOP 1 CompanyMD from gnMstCompanyMapping where CompanyCode=@CompanyCode and BranchCode=@BranchCode)
	set @BranchMD = (select TOP 1 BranchMD from gnMstCompanyMapping where CompanyCode=@CompanyCode and BranchCode=@BranchCode)
	--set @CompanySD = (select CompanyCode from gnMstCompanyMapping where CompanyMD=@CompanyCode and BranchMD=@BranchCode)
	--set @BranchSD = (select BranchCode from gnMstCompanyMapping where CompanyMD=@CompanyCode and BranchMD=@BranchCode)
	
	if @DBMD IS NULL
	begin
		SELECT itemInfo.PartNo
		, (item.OnHand - (item.AllocationSP + item.AllocationSR + item.AllocationSL)- (item.ReservedSP + item.ReservedSR + item.ReservedSL))  AS Available
		, itemPrice.RetailPriceInclTax
		, itemInfo.PartName
		, (CASE itemInfo.Status
			WHEN 1 THEN 'Aktif' ELSE 'Tidak Aktif'
			END)  AS Status
		, dtl.LookUpValueName as JenisPart
		, itemPrice.RetailPrice  AS NilaiPart
		FROM spMstItemInfo itemInfo                    
		INNER JOIN spMstItems item ON item.CompanyCode = itemInfo.CompanyCode
			AND item.BranchCode = @BranchCode
			AND item.PartNo = itemInfo.PartNo
		INNER JOIN spMstItemPrice itemPrice ON itemPrice.CompanyCode = @CompanyCode
			AND itemPrice.BranchCode = @BranchCode
			AND itemPrice.PartNo = item.PartNo
		LEFT JOIN GnMstLookUpDtl dtl ON item.companyCode = dtl.companyCode 
			AND dtl.CodeId = @CodeID
			AND dtl.LookUpValue = item.TypeOfGoods                    
		WHERE itemInfo.CompanyCode = @CompanyCode
			AND itemInfo.Status = '1'
			AND (item.TypeOfGoods = '2' OR item.TypeOfGoods = '5')
			AND (item.OnHand - (item.AllocationSP + item.AllocationSR + item.AllocationSL)
				- (item.ReservedSP + item.ReservedSR + item.ReservedSL)) > 0 
			AND itemInfo.PartNo NOT IN (SELECT x.PartNo FROM OmTrSalesSOAccsSeq x WHERE x.PartNo=itemInfo.PartNo AND x.SONo=@SONo
					AND x.CompanyCode=@CompanyCode AND BranchCode=@BranchCode)
		order by  item.TypeOfGoods desc
	end
	else
		set  @sqlstr = '
			SELECT itemInfo.PartNo
			, (item.OnHand - (item.AllocationSP + item.AllocationSR + item.AllocationSL)- (item.ReservedSP + item.ReservedSR + item.ReservedSL))  AS Available
			, itemPrice.RetailPriceInclTax
			, itemInfo.PartName
			, (CASE itemInfo.Status
				WHEN 1 THEN ''Aktif'' ELSE ''Tidak Aktif''
				END)  AS Status
			, dtl.LookUpValueName as JenisPart
			, itemPrice.RetailPrice  AS NilaiPart
			FROM ' + @DBMD + '..spMstItemInfo itemInfo                    
			INNER JOIN ' + @DBMD + '..spMstItems item ON item.CompanyCode = itemInfo.CompanyCode
				AND item.BranchCode = '''+ @BranchMD +'''
				AND item.PartNo = itemInfo.PartNo
			INNER JOIN ' + @DBMD + '..spMstItemPrice itemPrice ON itemPrice.CompanyCode = '''+ @CompanyMD + '''
				AND itemPrice.BranchCode = '''+ @BranchMD +'''
				AND itemPrice.PartNo = item.PartNo
			LEFT JOIN ' + @DBMD + '..GnMstLookUpDtl dtl ON item.companyCode = dtl.companyCode 
				AND dtl.CodeId = '''+ @CodeID +'''
				AND dtl.LookUpValue = item.TypeOfGoods                    
			WHERE itemInfo.CompanyCode = '''+ @CompanyMD +'''
				AND itemInfo.Status = ''1''
				AND (item.TypeOfGoods = ''2'' OR item.TypeOfGoods = ''5'')
				AND (item.OnHand - (item.AllocationSP + item.AllocationSR + item.AllocationSL)
					- (item.ReservedSP + item.ReservedSR + item.ReservedSL)) > 0
				AND itemInfo.PartNo NOT IN (SELECT x.PartNo FROM OmTrSalesSOAccsSeq x WHERE x.PartNo=itemInfo.PartNo AND x.SONo=''' + @SONo + '''
					AND x.CompanyCode = ''' + @CompanyCode + ''' AND BranchCode = ''' + @BranchCode + ''')  
			order by  item.TypeOfGoods desc
		'
	--print @sqlstr
	exec(@sqlstr)
end
go

ALTER VIEW [dbo].[SvReturnServiceView]
AS
SELECT a.CompanyCode, a.BranchCode, a.ProductType, 
     a.InvoiceNo, CASE a.InvoiceDate WHEN ('19000101') 
     THEN NULL 
     ELSE a.InvoiceDate END AS InvoiceDate, f.ReturnNo, 
     e.DescriptionEng AS InvoiceStatus, a.FPJNo, 
     CASE a.FPJDate WHEN ('19000101') THEN NULL 
     ELSE a.FPJDate END AS FPJDate, a.JobOrderNo, 
     CASE a.JobOrderDate WHEN ('19000101') THEN NULL 
     ELSE a.JobOrderDate END AS JobOrderDate, 
     a.JobType, a.ChassisCode, a.ChassisNo, 
     a.EngineCode, a.EngineNo, a.PoliceRegNo, 
     a.BasicModel, a.CustomerCode, a.CustomerCodeBill, 
     a.Remarks, 
     a.CustomerCode + ' - ' + b.CustomerName AS Customer,
      a.CustomerCodeBill + ' - ' + c.CustomerName AS CustomerBill,
      d.ServiceBookNo, a.Odometer, d.TransmissionType, 
     d.ColourCode
FROM dbo.svTrnInvoice AS a LEFT OUTER JOIN
     dbo.gnMstCustomer AS b ON 
     b.CompanyCode = a.CompanyCode AND 
     b.CustomerCode = a.CustomerCode LEFT OUTER JOIN
     dbo.gnMstCustomer AS c ON 
     c.CompanyCode = a.CompanyCode AND 
     c.CustomerCode = a.CustomerCodeBill LEFT OUTER JOIN
     dbo.svMstCustomerVehicle AS d ON 
     a.CompanyCode = d.CompanyCode AND 
     a.ChassisCode = d.ChassisCode AND 
     a.ChassisNo = d.ChassisNo AND 
     a.EngineCode = d.EngineCode AND 
     a.EngineNo = d.EngineNo AND 
     a.BasicModel = d.BasicModel LEFT OUTER JOIN
     dbo.svMstRefferenceService AS e ON 
     a.CompanyCode = e.CompanyCode AND 
     a.ProductType = e.ProductType AND 
     e.RefferenceType = 'INVSTATS' AND 
     a.InvoiceStatus = e.RefferenceCode LEFT OUTER JOIN
     dbo.SvTrnReturn AS f ON 
     a.CompanyCode = f.CompanyCode AND 
     a.BranchCode = f.BranchCode AND 
     a.ProductType = f.ProductType AND 
     a.InvoiceNo = f.InvoiceNo
WHERE (a.InvoiceStatus IN ('0', '1', '2', '3', '4'))

GO

ALTER procedure [dbo].[uspfn_SvTrnInvoiceDraft]
--DECLARE
	@CompanyCode varchar(15),
	@BranchCode  varchar(15),
	@JobOrderNo  varchar(15)
as  

--SELECT @CompanyCode = '6006400001', @BranchCode  ='6006400103', @JobOrderNo  = 'SPK/15/001546'
	

declare @errmsg   varchar(max)
declare @BillType varchar(10)

begin try
--set nocount on

-- get data from SvTrnService
select * into #srv from (
  select * from svTrnService with(nowait,nolock) 
    where 1 = 1  
      and CompanyCode  = @CompanyCode  
      and BranchCode   = @BranchCode  
      and EstimationNo = @JobOrderNo  
      and EstimationNo!= ''
  union all
  select * from svTrnService with(nowait,nolock) 
    where 1 = 1  
      and CompanyCode = @CompanyCode  
      and BranchCode  = @BranchCode  
      and BookingNo   = @JobOrderNo  
      and BookingNo  != ''
  union all
  select * from svTrnService with(nowait,nolock) 
    where 1 = 1  
      and CompanyCode = @CompanyCode  
      and BranchCode  = @BranchCode  
      and JobOrderNo  = @JobOrderNo  
      and JobOrderNo != ''
)#srv

select BillType into #t1 from (
select b.BillType from #srv a, svTrnSrvItem b with(nowait,nolock) 
 where 1 = 1
   and b.CompanyCode = a.CompanyCode
   and b.BranchCode  = a.BranchCode
   and b.ProductType = a.ProductType
   and b.ServiceNo   = a.ServiceNo
union
select b.BillType from #srv a, svTrnSrvTask b with(nowait,nolock) 
 where 1 = 1
   and b.CompanyCode = a.CompanyCode
   and b.BranchCode  = a.BranchCode
   and b.ProductType = a.ProductType
   and b.ServiceNo   = a.ServiceNo
)#

set @BillType = (select top 1 a.BillType from svMstBillingType a with(nowait,nolock), #t1 b where b.BillType = a.BillType order by a.LockingBy)
drop table #t1

-- get dicount from Service
declare @ProductType     varchar(20)  set @ProductType     = isnull((select top 1 ProductType     from #srv),0)
declare @ServiceNo       bigint       set @ServiceNo       = isnull((select top 1 ServiceNo       from #srv),0)

-- get ppn & pph dicount from Service
declare @PPnPct decimal
    set @PPnPct = isnull((select a.TaxPct
 						    from gnMstTax a with(nowait,nolock), gnMstCustomerProfitCenter b with(nowait,nolock) , #srv c
						   where c.CompanyCode  = b.CompanyCode
						     and c.BranchCode   = b.BranchCode
						     and c.CustomerCodeBill = b.CustomerCode
						     and b.CompanyCode  = a.CompanyCode
						     and b.TaxCode      = a.TaxCode
						     and b.ProfitCenterCode = '200'
						     and b.TaxCode      = 'PPN'
							),0)

declare @PPhPct decimal
    set @PPhPct = isnull((select a.TaxPct
							from gnMstTax a with(nowait,nolock), gnMstCustomerProfitCenter b with(nowait,nolock) , #srv c
						   where c.CompanyCode  = b.CompanyCode
						     and c.BranchCode   = b.BranchCode
						     and c.CustomerCodeBill = b.CustomerCode
						     and b.CompanyCode  = a.CompanyCode
						     and b.TaxCode      = a.TaxCode
						     and b.ProfitCenterCode = '200'
						     and b.TaxCode      = 'PPH'
							),0)

-- get data gross amount
declare @LaborGrossAmt decimal
    set @LaborGrossAmt = isnull((
						select ceiling(sum(a.OperationHour * a.OperationCost))
						  from svTrnSrvTask a with(nowait,nolock), #srv b
						 where a.CompanyCode = b.CompanyCode
						   and a.BranchCode  = b.BranchCode
						   and a.ProductType = b.ProductType
						   and a.ServiceNo   = b.ServiceNo
						   and a.BillType    = @BillType
						),0)

declare @PartsGrossAmt decimal
    set @PartsGrossAmt = isnull((
						--select ceiling(sum((i.SupplyQty - i.ReturnQty) * i.RetailPrice))--Sebelum Perubahan
						select ceiling(sum(Round((i.SupplyQty - i.ReturnQty) * i.RetailPrice,0)))--Sesudah Perubahan
						  from svTrnSrvItem i with(nowait,nolock), gnMstLookUpDtl g with(nowait,nolock)
						 where g.CompanyCode = i.CompanyCode
					 	   and g.LookUpValue = i.TypeOfGoods
						   and g.CodeID      = 'GTGO'
						   and g.ParaValue   = 'SPAREPART'
						   and i.CompanyCode = @CompanyCode
						   and i.BranchCode  = @BranchCode
						   and i.ProductType = @ProductType
						   and i.ServiceNo   = @ServiceNo
						   and i.BillType    = @BillType
						),0)

declare @MaterialGrossAmt decimal
    set @MaterialGrossAmt = isnull((
						 --select ceiling(sum((i.SupplyQty - i.ReturnQty) * i.RetailPrice))--Sebelum Perubahan
						 select ceiling(sum(Round((i.SupplyQty - i.ReturnQty) * i.RetailPrice,0)))--Sesudah Perubahan
						   from svTrnSrvItem i with(nowait,nolock), gnMstLookUpDtl g with(nowait,nolock)
						  where g.CompanyCode = i.CompanyCode
							and g.LookUpValue = i.TypeOfGoods
							and g.CodeID      = 'GTGO'
							and g.ParaValue  in ('OLI','MATERIAL')
							and i.CompanyCode = @CompanyCode
							and i.BranchCode  = @BranchCode
							and i.ProductType = @ProductType
							and i.ServiceNo   = @ServiceNo
						    and i.BillType    = @BillType
						  ),0)

-- calculate discount
declare @LaborDiscAmt decimal
    set @LaborDiscAmt = isnull((
						 --select ceiling(sum(OperationHour * OperationCost * (DiscPct/100.0))) -- sebelum perbaikan
						 select (sum(OperationHour * OperationCost * (DiscPct/100.0))) --setelah perbaikan
						   from svTrnSrvTask with(nowait,nolock)
						  where CompanyCode = @CompanyCode
							and BranchCode = @BranchCode
							and ProductType = @ProductType
							and ServiceNo = @ServiceNo
						    and BillType    = @BillType
						  ),0)

declare @PartsDiscAmt decimal
    set @PartsDiscAmt = isnull((
						 --select ceiling(sum((i.SupplyQty - i.ReturnQty) * i.RetailPrice * (i.DiscPct/100.0)))--Sebelum Perubahan
						 select ceiling(sum(Round((i.SupplyQty - i.ReturnQty) * i.RetailPrice * (i.DiscPct/100.0),0)))--Sesudah Perubahan
						   from svTrnSrvItem i with(nowait,nolock), gnMstLookUpDtl g with(nowait,nolock)
						 where g.CompanyCode = i.CompanyCode
					 	   and g.LookUpValue = i.TypeOfGoods
						   and g.CodeID      = 'GTGO'
						   and g.ParaValue   = 'SPAREPART'
						   and i.CompanyCode = @CompanyCode
						   and i.BranchCode  = @BranchCode
						   and i.ProductType = @ProductType
						   and i.ServiceNo   = @ServiceNo
						   and i.BillType    = @BillType
						  ),0)

declare @MaterialDiscAmt decimal
    set @MaterialDiscAmt = isnull((
						 --select ceiling(sum((i.SupplyQty - i.ReturnQty) * i.RetailPrice * (i.DiscPct/100.0)))--Sebelum Perubahan
						 select ceiling(sum(Round((i.SupplyQty - i.ReturnQty) * i.RetailPrice * (i.DiscPct/100.0),0)))--Sesudah Perubahan
						   from svTrnSrvItem i with(nowait,nolock), gnMstLookUpDtl g with(nowait,nolock)
						  where g.CompanyCode = i.CompanyCode
							and g.LookUpValue = i.TypeOfGoods
							and g.CodeID      = 'GTGO'
							and g.ParaValue  in ('OLI','MATERIAL')
							and i.CompanyCode = @CompanyCode
							and i.BranchCode  = @BranchCode
							and i.ProductType = @ProductType
							and i.ServiceNo   = @ServiceNo
						    and i.BillType    = @BillType
						  ),0)

-- calculate DPP (dasar pengenaan pajak)
--declare @LaborDppAmt     decimal	set @LaborDppAmt     = floor(@LaborGrossAmt    - @LaborDiscAmt)--Sebelum Perubahan
--declare @PartsDppAmt     decimal	set @PartsDppAmt     = floor(@PartsGrossAmt    - @PartsDiscAmt)--Sebelum Perubahan
--declare @MaterialDppAmt  decimal	set @MaterialDppAmt  = floor(@MaterialGrossAmt - @MaterialDiscAmt)--Sebelum Perubahan
--declare @TotalDppAmt     decimal	set @TotalDppAmt     = floor(@LaborDppAmt + @PartsDppAmt + @MaterialDppAmt)--Sebelum Perubahan
declare @LaborDppAmt     decimal	set @LaborDppAmt     = Round(@LaborGrossAmt    - @LaborDiscAmt,0)--Sesudah Perubahan
declare @PartsDppAmt     decimal	set @PartsDppAmt     = Round(@PartsGrossAmt    - @PartsDiscAmt,0)--Sesudah Perubahan
declare @MaterialDppAmt  decimal	set @MaterialDppAmt  = Round(@MaterialGrossAmt - @MaterialDiscAmt,0)--Sesudah Perubahan
declare @TotalDppAmt     decimal	set @TotalDppAmt     = Round(@LaborDppAmt + @PartsDppAmt + @MaterialDppAmt,0)--Sesudah Perubahan

-- calculate ppn & service amount
declare @TotalPpnAmt     decimal	set @TotalPpnAmt = floor(@TotalDppAmt * (@PpnPct / 100.0))
declare @TotalPphAmt     decimal	set @TotalPphAmt = floor(@TotalDppAmt * (@PphPct / 100.0))
declare @TotalSrvAmt     decimal	set @TotalSrvAmt = floor(@TotalDppAmt + @TotalPphAmt + @TotalPpnAmt)

    
;with t1 as (
select a.CompanyCode, a.BranchCode, a.ProductType, a.ServiceNo
     , a.EstimationNo, a.EstimationDate, a.BookingNo, a.BookingDate, a.JobOrderNo, a.JobOrderDate, a.ServiceType, a.IsSparepartClaim
     , a.PoliceRegNo, a.ServiceBookNo, a.BasicModel, a.TransmissionType
     , a.ChassisCode, a.ChassisNo, a.EngineCode, a.EngineNo, a.ColorCode
     , rtrim(rtrim(a.ColorCode)
     + case isnull(b.RefferenceDesc2,'') when '' then '' else ' - ' end
     + isnull(b.RefferenceDesc2,'')) as ColorCodeDesc
     , a.Odometer
     , a.CustomerCode, c.CustomerName, c.Address1 CustAddr1
     , c.Address2 CustAddr2, c.Address3 CustAddr3, c.Address4 CustAddr4
     , c.CityCode CityCode, d.LookupValueName CityName
     , a.InsurancePayFlag, a.InsuranceOwnRisk, a.InsuranceNo, a.InsuranceJobOrderNo
     , a.CustomerCodeBill, e.CustomerName CustomerNameBill
     , e.Address1 CustAddr1Bill, e.Address2 CustAddr2Bill
     , e.Address3 CustAddr3Bill, e.Address4 CustAddr4Bill
     , e.CityCode CityCodeBill, f.LookupValueName CityNameBill
     , e.PhoneNo, e.FaxNo, e.HPNo, a.LaborDiscPct, a.PartDiscPct
     , a.ServiceRequestDesc, a.ConfirmChangingPart, a.EstimateFinishDate
     , a.MaterialDiscPct, a.JobType, a.ForemanID, a.MechanicID
     , a.ServiceStatus
	 , @LaborDppAmt LaborDppAmt, @PartsDppAmt PartsDppAmt, @MaterialDppAmt MaterialDppAmt
	 , @TotalDppAmt TotalDppAmt, @TotalPpnAmt TotalPpnAmt
	 , @TotalSrvAmt TotalSrvAmt
	 , a.LaborDppAmt SrvLaborDppAmt, a.PartsDppAmt SrvPartsDppAmt, a.MaterialDppAmt SrvMaterialDppAmt
	 , a.TotalDppAmount SrvTotalDppAmt, a.TotalPpnAmount SrvTotalPpnAmt
	 , a.TotalSrvAmount SrvTotalSrvAmt
  from svTrnService a with (nowait,nolock)
  left join omMstRefference b with (nowait,nolock)
    on b.CompanyCode = a.CompanyCode
   and b.RefferenceType = 'COLO'
   and b.RefferenceCode = a.ColorCode
  left join gnMstCustomer c with (nowait,nolock)
    on c.CompanyCode = a.CompanyCode
   and c.CustomerCode = a.CustomerCode
  left join gnMstLookupDtl d with (nowait,nolock)
    on d.CompanyCode = a.CompanyCode
   and d.CodeID = 'CITY'
   and d.LookUpValue = c.CityCode
  left join gnMstCustomer e with (nowait,nolock)
    on e.CompanyCode = a.CompanyCode
   and e.CustomerCode = a.CustomerCodeBill
  left join gnMstLookupDtl f with (nowait,nolock)
    on f.CompanyCode = a.CompanyCode
   and f.CodeID = 'CITY'
   and f.LookUpValue = e.CityCode
 where 1 = 1
   and a.CompanyCode = @CompanyCode
   and a.BranchCode  = @BranchCode
   and a.ServiceNo   = (select ServiceNo from #srv)
) 
select a.CompanyCode, a.BranchCode, a.ProductType --, JobOrderNo, 
	 , a.ServiceNo, a.ServiceType
     , a.EstimationNo, a.EstimationDate, a.BookingNo, a.BookingDate, a.JobOrderNo, a.JobOrderDate
     , '' InvoiceNo, z.Remarks 
     -- Data Kendaraan
     , a.PoliceRegNo, a.ServiceBookNo, a.BasicModel, a.TransmissionType
     , a.ChassisCode, a.ChassisNo, a.EngineCode, a.EngineNo
     , a.ColorCode, a.ColorCodeDesc, a.Odometer
     -- Data Contract
     , b.IsContractStatus IsContract
     , b.ContractNo
	 , c.EndPeriod ContractEndPeriod
	 , c.IsActive ContractStatus
	 , case b.IsContractStatus 
		when 1 then (case c.IsActive when 1 then 'Aktif' else 'Tidak Aktif' end)
		else ''
	   end ContractStatusDesc
     -- Data Contract
	 , b.IsClubStatus IsClub
	 , b.ClubCode
	 , b.ClubDateFinish ClubEndPeriod
	 , d.IsActive ClubStatus
	 , case b.IsClubStatus
		when 1 then (case d.IsActive when 1 then 'Aktif' else 'Tidak Aktif' end)
		else ''
	   end ClubStatusDesc
     -- Data Customer
     , a.CustomerCode, a.CustomerName
     , a.CustAddr1, a.CustAddr2, a.CustAddr3, a.CustAddr4
     , a.CityCode, a.CityName
     -- Data Customer Bill
     , a.InsurancePayFlag, a.InsuranceOwnRisk, a.InsuranceNo, a.InsuranceJobOrderNo
     , a.CustomerCodeBill, a.CustomerNameBill
     , a.CustAddr1Bill, a.CustAddr2Bill, a.CustAddr3Bill, a.CustAddr4Bill
     , a.CityCodeBill, a.CityNameBill
     , a.PhoneNo, a.FaxNo, a.HPNo
     , a.LaborDiscPct, a.PartDiscPct, a.PartDiscPct PartsDiscPct, a.MaterialDiscPct
     --, IsPPnBill
     -- Data Pekerjaan
     , a.ServiceRequestDesc
     , a.JobType, e.Description JobTypeDesc
     , a.ConfirmChangingPart, a.EstimateFinishDate
     , a.ForemanID, g.EmployeeName ForemanName
	 , a.MechanicID, h.EmployeeName MechanicName, a.IsSparepartClaim
	 -- Data Total Biaya Perawatan
     , a.LaborDppAmt, a.PartsDppAmt, a.MaterialDppAmt, a.TotalDppAmt
     , a.TotalPpnAmt, a.TotalSrvAmt
     , a.SrvLaborDppAmt, a.SrvPartsDppAmt, a.SrvMaterialDppAmt, a.SrvTotalDppAmt
     , a.SrvTotalPpnAmt, a.SrvTotalSrvAmt

     , a.ServiceStatus
	 , f.Description ServiceStatusDesc
	 , isnull(i.TaxCode,'') TaxCode
	 , isnull(j.TaxPct,0) TaxPct
  from t1 a
  left join svMstCustomerVehicle b with (nowait,nolock)
    on b.CompanyCode = a.CompanyCode
   and b.ChassisCode = a.ChassisCode
   and b.ChassisNo = a.ChassisNo
  left join svMstContract c with (nowait,nolock)
    on c.CompanyCode = a.CompanyCode
   and c.ContractNo = b.ContractNo
   and b.IsContractStatus = 1
  left join svMstClub d with (nowait,nolock)
    on d.CompanyCode = a.CompanyCode
   and d.ClubCode = b.ClubCode
  left join SvMstRefferenceService e with (nowait,nolock)
    on e.CompanyCode = a.CompanyCode
   and e.ProductType = a.ProductType
   and e.RefferenceCode = a.JobType
   and e.RefferenceType = 'JOBSTYPE'
  left join SvMstRefferenceService f with (nowait,nolock)
    on f.CompanyCode = a.CompanyCode
   and f.ProductType = a.ProductType
   and f.RefferenceCode = a.ServiceStatus
   and f.RefferenceType = 'SERVSTAS'
  left join gnMstEmployee g with (nowait,nolock)
    on g.CompanyCode = a.CompanyCode
   and g.BranchCode = a.BranchCode
   and g.EmployeeID = a.ForemanID
  left join gnMstEmployee h with (nowait,nolock)
    on h.CompanyCode = a.CompanyCode
   and h.BranchCode = a.BranchCode
   and h.EmployeeID = a.MechanicID
  left join gnMstCustomerProfitCenter i with (nowait,nolock)
    on i.CompanyCode = a.CompanyCode
   and i.BranchCode = a.BranchCode
   and i.CustomerCode = a.CustomerCode
   and i.ProfitCenterCode = '200'
  left join gnMstTax j with (nowait,nolock)
    on j.CompanyCode = a.CompanyCode
   and j.TaxCode = i.TaxCode
  left join svTrnInvoice z with (nowait, nolock)
   on a.JobOrderNo = z.JobOrderNo AND a.CompanyCode = z.CompanyCode AND a.BranchCode = z.BranchCode

end try
begin catch
    set @errmsg = 'Error Message:' + char(13) + error_message()
    raiserror (@errmsg,16,1);
	drop table #srv
end catch


GO

IF object_id('uspfn_OmInquirySalesLookUpBtnWeb') IS NOT NULL
	DROP PROCEDURE uspfn_OmInquirySalesLookUpBtnWeb
GO
CREATE procedure [dbo].[uspfn_OmInquirySalesLookUpBtnWeb]
	@StartDate Datetime,
	@EndDate Datetime,
	@Area varchar(100),
	@Dealer varchar(100),
	@Outlet varchar(100),
	@BranchHead varchar(100),
	@SalesHead varchar(100),
	@SalesCoordinator varchar(100),
	@Salesman varchar(100),
	@Detail int,
	@SalesType varchar(50)
as
Declare @MainTable table
(
	GroupNo					varchar(100)
	, Area					varchar(100)
	, CompanyCode			varchar(15)
	, CompanyName			varchar(100)
	, BranchCode			varchar(15)
	, BranchName			varchar(100)
	, BranchHeadID			varchar(15)
	, BranchHeadName		varchar(100)
	, SalesHeadID			varchar(15)
	, SalesHeadName			varchar(100)
	, SalesCoordinatorID	varchar(15)
	, SalesCoordinatorName	varchar(100)
	, SalesmanID			varchar(15)
	, SalesmanName			varchar(100)
	, ModelCatagory			varchar(15)
	, SalesType				varchar(25)
	, InvoiceNo				varchar(15)
	, InvoiceDate			datetime
	, SONo					varchar(15)
	, SalesModelCode		varchar(25)
	, Year					numeric(4,0)
	, SalesModelDesc		varchar(150)
	, FakturPolisiNo		varchar(15)
	, FakturPolisiDate		datetime
	, FakturPolisiDesc		varchar(150)
	, MarketModel			varchar(25)
	, ColourCode			varchar(25)
	, ColourName			varchar(100)
	, GroupMarketModel		varchar(100)
	, ColumnMarketModel		varchar(100)
	, JoinDate				datetime
	, ResignDate			datetime
	, GradeDate				datetime
	, Grade					varchar(50)
	, ChassisCode			varchar(15)
	, ChassisNo				varchar(15)
	, EngineCode			varchar(15) 
	, EngineNo				varchar(15)
	, COGS					numeric(18,0)
	, BeforeDiscDPP			numeric(18,0)
	, DiscExcludePPn		numeric(18,0)
	, DiscIncludePPn		numeric(18,0)
	, AfterDiscDPP			numeric(18,0)
	, AfterDiscPPn			numeric(18,0)
	, AfterDiscPPnBM		numeric(18,0)
	, AfterDiscTotal		numeric(18,0)
	, PPnBMPaid				numeric(18,0)
	, OthersDPP				numeric(18,0)
	, OthersPPn				numeric(18,0)
	, ShipAmt				numeric(18,0)
	, DepositAmt			numeric(18,0)
	, OthersAmt				numeric(18,0)
)
insert into @MainTable
exec uspfn_OmInquirySalesWeb @StartDate, @EndDate, @Area, @Dealer, @Outlet, @BranchHead, @SalesHead, @SalesCoordinator, @Salesman, @SalesType

select * into #t6 from (
select GroupNo
		, Area
		, CompanyCode
		, CompanyName
		, BranchCode
		, BranchName
		, BranchHeadID
		, BranchHeadName
		, SalesHeadID
		, SalesHeadName
		, SalesCoordinatorID
		, SalesCoordinatorName
		, SalesmanID
		, SalesmanName
		, ModelCatagory
		, SalesModelDesc
		, MarketModel
		, ColourName
		, YEAR(InvoiceDate) Year 
		, MONTH(InvoiceDate) Month
		, COUNT(SalesModelCode) SoldTotal
  from @MainTable
  group by GroupNo
		, Area 
		, CompanyCode
		, CompanyName
		, BranchCode
		, BranchName
		, BranchHeadID
		, BranchHeadName
		, SalesHeadID
		, SalesHeadName
		, SalesCoordinatorID
		, SalesCoordinatorName
		, SalesmanID
		, SalesmanName
		, SalesModelDesc
		, ModelCatagory
		, MarketModel
		, ColourName
		, YEAR(InvoiceDate) 
		, MONTH(InvoiceDate)
)#t6

declare @TempTable table(
	Area					varchar(100),
	CompanyCode				varchar(100),
	CompanyName				varchar(100),
	BranchCode				varchar(100),
	BranchName				varchar(100),
	BranchHeadID			varchar(100),
	BranchHeadName			varchar(100),
	SalesHeadID				varchar(100),
	SalesHeadName			varchar(100),
	SalesCoordinatorID		varchar(100),
	SalesCoordinatorName	varchar(100),
	SalesmanID				varchar(100),
	SalesmanName			varchar(100)
)
insert into @TempTable
select '<----Select All---->','<----Select All---->','<----Select All---->','<----Select All---->','<----Select All---->','<----Select All---->','<----Select All---->','<----Select All---->',
'<----Select All---->','<----Select All---->','<----Select All---->','<----Select All---->','<----Select All---->'

if(@Detail = 4)
begin
	insert into @TempTable
	select distinct Area
		, CompanyCode
		, CompanyName
		, BranchCode
		, BranchName
		, BranchHeadID
		, BranchHeadName
		, '' [1]
		, '' [2]
		, '' [3]
		, '' [4]
		, '' [5]
		, '' [6] 
	from #t6 
	where (#t6.Area like Case when @Area <> '' 
								then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
										then 'JABODETABEK'
										else @Area end
								else '%%' end
			or #t6.Area like Case when @Area <> '' 
								then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
										then 'CABANG'
										else @Area end
								else '%%' end)
		and #t6.CompanyCode like Case when @Dealer <> '' then @Dealer else '%%' end
		and #t6.BranchCode like Case when @Outlet <> '' then @Outlet else '%%' end
		and isnull(#t6.BranchHeadName,'') <> '' 
	order by BranchHeadName
end
else if(@Detail = 5)
begin
	insert into @TempTable
	select distinct Area
		, CompanyCode
		, CompanyName
		, BranchCode
		, BranchName
		, BranchHeadID
		, BranchHeadName 
		, SalesHeadID
		, SalesHeadName 
		, '' [1]
		, '' [2]
		, '' [3]
		, '' [4]
	from #t6
	where  (#t6.Area like Case when @Area <> '' 
								then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
										then 'JABODETABEK'
										else @Area end
								else '%%' end
			or #t6.Area like Case when @Area <> '' 
								then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
										then 'CABANG'
										else @Area end
								else '%%' end)
		and #t6.CompanyCode like Case when @Dealer <> '' then @Dealer else '%%' end
		and #t6.BranchCode like Case when @Outlet <> '' then @Outlet else '%%' end 
		and #t6.BranchHeadID like Case when @BranchHead <> '' then @BranchHead else '%%' end 
		and isnull(#t6.SalesHeadID,'') <> '' 
	order by SalesHeadName
end
else if(@Detail = 6)
begin
	insert into @TempTable
	select distinct Area
		, CompanyCode
		, CompanyName
		, BranchCode
		, BranchName
		, BranchHeadID
		, BranchHeadName 
		, SalesHeadID
		, SalesHeadName 
		, SalesCoordinatorID
		, SalesCoordinatorName 
		, '' [1]
		, '' [2]
	from #t6 
	where  (#t6.Area like Case when @Area <> '' 
								then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
										then 'JABODETABEK'
										else @Area end
								else '%%' end
			or #t6.Area like Case when @Area <> '' 
								then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
										then 'CABANG'
										else @Area end
								else '%%' end)
		and #t6.CompanyCode like Case when @Dealer <> '' then @Dealer else '%%' end
		and #t6.BranchCode like Case when @Outlet <> '' then @Outlet else '%%' end
		and #t6.BranchHeadID like Case when @BranchHead <> '' then @BranchHead else '%%' end 
		and #t6.SalesHeadID like Case when @SalesHead <> '' then @SalesHead else '%%' end 
		--and isnull(#t6.SalesCoordinatorName,'') <> '' 
	order by SalesCoordinatorName
end
else if(@Detail = 7)
begin
	insert into @TempTable
	select distinct Area
		, CompanyCode
		, CompanyName
		, BranchCode
		, BranchName
		, BranchHeadID
		, BranchHeadName 
		, SalesHeadID
		, SalesHeadName 
		, SalesCoordinatorID
		, SalesCoordinatorName 
		, SalesmanID
		, SalesmanName
	from #t6 
	where  (#t6.Area like Case when @Area <> '' 
								then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
										then 'JABODETABEK'
										else @Area end
								else '%%' end
			or #t6.Area like Case when @Area <> '' 
								then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
										then 'CABANG'
										else @Area end
								else '%%' end)
		and #t6.CompanyCode like Case when @Dealer <> '' then @Dealer else '%%' end
		and #t6.BranchCode like Case when @Outlet <> '' then @Outlet else '%%' end
		and #t6.BranchHeadID like Case when @BranchHead <> '' then @BranchHead else '%%' end 
		and #t6.SalesHeadID like Case when @SalesHead <> '' then @SalesHead else '%%' end 
		and #t6.SalesCoordinatorID like Case when @SalesCoordinator <> '' then @SalesCoordinator else '%%' end 
		and isnull(#t6.SalesmanName,'') <> '' 
	order by SalesmanName
end
else if(@Detail = 8)
begin
	insert into @TempTable
	select distinct MarketModel
		, '' [1]
		, '' [2]
		, '' [3]
		, '' [4]
		, '' [5]
		, '' [6]
		, '' [7]
		, '' [8]
		, '' [9]
		, '' [10]
		, '' [11]
		, '' [12]
	from #t6 
	where  (#t6.Area like Case when @Area <> '' 
								then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
										then 'JABODETABEK'
										else @Area end
								else '%%' end
			or #t6.Area like Case when @Area <> '' 
								then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
										then 'CABANG'
										else @Area end
								else '%%' end)
		and #t6.CompanyCode like Case when @Dealer <> '' then @Dealer else '%%' end
		and #t6.BranchCode like Case when @Outlet <> '' then @Outlet else '%%' end
		and #t6.BranchHeadID like Case when @BranchHead <> '' then @BranchHead else '%%' end 
		and #t6.SalesHeadID like Case when @SalesHead <> '' then @SalesHead else '%%' end 
		and #t6.SalesCoordinatorID like Case when @SalesCoordinator <> '' then @SalesCoordinator else '%%' end 
	order by MarketModel
end

select * from @TempTable

GO

IF object_id('uspfn_OmInquirySalesWeb') IS NOT NULL
	DROP PROCEDURE uspfn_OmInquirySalesWeb
GO
-- Inquiry Sales for flexible report
-- uspfn_OmInquirySales '2012-1-1','2013-12-31','CABANG','6006406','6006406','CECEP CAHYADI','IVAN RIKI SAEFUL','','',''
-- uspfn_OmInquirySales '2013-2-1','2013-2-12','','','','','','','','WHOLESALE'
-- uspfn_OmInquirySales '2013-2-1','2013-2-12','','','','','','','','SALES'
-- uspfn_OmInquirySales '2013-6-1','2013-6-27','','','','','','','','FPOL'
CREATE procedure [dbo].[uspfn_OmInquirySalesWeb]
	@StartDate Datetime,
	@EndDate Datetime,
	@Area varchar(100),
	@Dealer varchar(100),
	@Outlet varchar(100),
	@BranchHead varchar(100),
	@SalesHead varchar(100),
	@SalesCoordinator varchar(100),
	@Salesman varchar(100),
	@SalesType varchar(25)
as 
declare @National varchar(10);
set @National = (select top 1 ISNULL(ParaValue,0) from gnMstLookUpDtl
                  where CodeID='QSLS' and LookUpValue='NATIONAL')

if @National <> '1'
begin

	if (@SalesType = 'SALES' or @SalesType = 'WHOLESALE' or @SalesType = '')
	begin
		select map.GroupNo 
			, inq.Area
			, inq.CompanyCode
			, inq.CompanyName
			, inq.BranchCode
			, inq.BranchName
			, inq.BranchHeadID
			, inq.BranchHeadName
			, inq.SalesHeadID
			, inq.SalesHeadName
			, inq.SalesCoordinatorID
			, inq.SalesCoordinatorName
			, inq.SalesmanID
			, inq.SalesmanName
			, inq.ModelCatagory
			, inq.SalesType
			, inq.InvoiceNo
			, inq.InvoiceDate
			, substring(inq.SONo,1,13) SoNo
			, inq.SalesModelCode
			, inq.SalesModelYear
			, inq.SalesModelDesc
			, inq.FakturPolisiNo
			, inq.FakturPolisiDate
			, inq.FakturPolisiDesc
			, inq.MarketModel
			, inq.ColourCode
			, inq.ColourName
			, inq.GroupMarketModel
			, inq.ColumnMarketModel
			, inq.JoinDate
			, inq.ResignDate
			, inq.GradeDate
			, case when isnull(grdDtl.LookUpValueName,'') <> '' then grdDtl.LookUpValueName else inq.Grade end Grade
			, inq.ChassisCode  
			, inq.ChassisNo  
			, inq.EngineCode  
			, inq.EngineNo  
			, inq.COGS
			, inq.BeforeDiscDPP
			, inq.DiscExcludePPn
			, inq.DiscIncludePPn
			, inq.AfterDiscDPP
			, inq.AfterDiscPPn
			, inq.AfterDiscPPnBM
			, inq.AfterDiscTotal
			, inq.PPnBMPaid
			, inq.OthersDPP
			, inq.OthersPPn
			, inq.ShipAmt
			, inq.DepositAmt
			, inq.OthersAmt
			from omHstInquirySales inq with (nolock, nowait)
			Left Join GnMstLookUpDtl grdDtl on inq.CompanyCode = grdDtl.CompanyCode
				and grdDtl.CodeID = 'ITSG'
				and grdDtl.LookUpValue = inq.Grade
			left join gnMstDealerMapping map
				on map.DealerCode = inq.CompanyCode
			where convert(varchar, inq.InvoiceDate, 112) between convert(varchar, @StartDate, 112) and convert(varchar, @EndDate, 112)
			and isnull(inq.CompanyCode,'') like case when @Dealer <> '' then @Dealer else '%%' end
			and isnull(inq.BranchCode,'') like case when @Outlet <> '' then @Outlet else '%%' end 
			--and isnull(inq.Area,'') like case when @Area <> '' then @Area else '%%' end
			and (inq.Area like Case when @Area <> '' 
								then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
										then 'JABODETABEK'
										else @Area end
								else '%%' end
			or inq.Area like Case when @Area <> '' 
								then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
										then 'CABANG'
										else @Area end
								else '%%' end) 
			and isnull(inq.BranchHeadID,'') like case when @BranchHead <> '' then @BranchHead else '%%' end  
			and isnull(inq.SalesHeadID,'') like case when @SalesHead <> '' then @SalesHead else '%%' end   
			and isnull(inq.SalesCoordinatorID,'') like case when @SalesCoordinator <> '' then @SalesCoordinator else '%%' end    
			and isnull(inq.SalesmanID,'') like case when @Salesman <> '' then @Salesman else '%%' end   
			and isnull(inq.MarketModel,'') <> ''
			and inq.Status = '1'
			and map.isActive = 1
			order by map.GroupNo,CompanyCode, inq.BranchCode, inq.BranchHeadName, inq.SalesHeadName, inq.SalesCoordinatorName, inq.SalesmanName, inq.MarketModel
	end

	if @SalesType = 'RETAIL'
	BEGIN
			select map.GroupNo 
			, inq.Area
			, inq.CompanyCode
			, inq.CompanyName
			, inq.BranchCode
			, inq.BranchName
			, inq.BranchHeadID
			, inq.BranchHeadName
			, inq.SalesHeadID
			, inq.SalesHeadName
			, inq.SalesCoordinatorID
			, inq.SalesCoordinatorName
			, inq.SalesmanID
			, inq.SalesmanName
			, inq.ModelCatagory
			, inq.SalesType
			, inq.InvoiceNo
			, inq.InvoiceDate
			, substring(inq.SONo,1,13) SoNo
			, inq.SalesModelCode
			, inq.SalesModelYear
			, inq.SalesModelDesc
			, inq.FakturPolisiNo
			, inq.FakturPolisiDate
			, inq.FakturPolisiDesc
			, inq.MarketModel
			, inq.ColourCode
			, inq.ColourName
			, inq.GroupMarketModel
			, inq.ColumnMarketModel
			, inq.JoinDate
			, inq.ResignDate
			, inq.GradeDate
			, case when isnull(grdDtl.LookUpValueName,'') <> '' then grdDtl.LookUpValueName else inq.Grade end Grade
			, inq.ChassisCode  
			, inq.ChassisNo  
			, inq.EngineCode  
			, inq.EngineNo  
			, inq.COGS
			, inq.BeforeDiscDPP
			, inq.DiscExcludePPn
			, inq.DiscIncludePPn
			, inq.AfterDiscDPP
			, inq.AfterDiscPPn
			, inq.AfterDiscPPnBM
			, inq.AfterDiscTotal
			, inq.PPnBMPaid
			, inq.OthersDPP
			, inq.OthersPPn
			, inq.ShipAmt
			, inq.DepositAmt
			, inq.OthersAmt
			from omHstInquirySales inq with (nolock, nowait)
			Left Join GnMstLookUpDtl grdDtl on inq.CompanyCode = grdDtl.CompanyCode
				and grdDtl.CodeID = 'ITSG'
				and grdDtl.LookUpValue = inq.Grade
			left join gnMstDealerMapping map
				on map.DealerCode = inq.CompanyCode
			where convert(varchar, inq.InvoiceDate, 112) between convert(varchar, @StartDate, 112) and convert(varchar, @EndDate, 112)
			and isnull(inq.CompanyCode,'') like case when @Dealer <> '' then @Dealer else '%%' end
			and isnull(inq.BranchCode,'') like case when @Outlet <> '' then @Outlet else '%%' end 
			--and isnull(inq.Area,'') like case when @Area <> '' then @Area else '%%' end
			and (inq.Area like Case when @Area <> '' 
								then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
										then 'JABODETABEK'
										else @Area end
								else '%%' end
			or inq.Area like Case when @Area <> '' 
								then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
										then 'CABANG'
										else @Area end
								else '%%' end) 			
			and isnull(inq.BranchHeadID,'') like case when @BranchHead <> '' then @BranchHead else '%%' end  
			and isnull(inq.SalesHeadID,'') like case when @SalesHead <> '' then @SalesHead else '%%' end   
			and isnull(inq.SalesCoordinatorID,'') like case when @SalesCoordinator <> '' then @SalesCoordinator else '%%' end    
			and isnull(inq.SalesmanID,'') like case when @Salesman <> '' then @Salesman else '%%' end   
			and isnull(inq.MarketModel,'') <> ''
			and inq.Status = '1'
			and (inq.SoNo not like '%MD' and inq.SoNo not like '%SD') 
			and map.isActive = 1
			order by map.GroupNo,CompanyCode, inq.BranchCode, inq.BranchHeadName, inq.SalesHeadName, inq.SalesCoordinatorName, inq.SalesmanName, inq.MarketModel
	END
	
	IF @SalesType = 'FPOL'
	BEGIN
	print('Masuk')
	select map.GroupNo 
			, inq.Area
			, inq.CompanyCode
			, inq.CompanyName
			, inq.BranchCode
			, inq.BranchName
			, inq.BranchHeadID
			, inq.BranchHeadName
			, inq.SalesHeadID
			, inq.SalesHeadName
			, inq.SalesCoordinatorID
			, inq.SalesCoordinatorName
			, inq.SalesmanID
			, inq.SalesmanName
			, inq.ModelCatagory
			, inq.SalesType
			, inq.InvoiceNo
			, inq.InvoiceDate
			, substring(inq.SONo,1,13) SoNo
			, inq.SalesModelCode
			, inq.SalesModelYear
			, inq.SalesModelDesc
			, inq.FakturPolisiNo
			, inq.FakturPolisiDate
			, inq.FakturPolisiDesc
			, inq.MarketModel
			, inq.ColourCode
			, inq.ColourName
			, inq.GroupMarketModel
			, inq.ColumnMarketModel
			, inq.JoinDate
			, inq.ResignDate
			, inq.GradeDate
			, case when isnull(grdDtl.LookUpValueName,'') <> '' then grdDtl.LookUpValueName else inq.Grade end Grade
			, inq.ChassisCode  
			, inq.ChassisNo  
			, inq.EngineCode  
			, inq.EngineNo  
			, inq.COGS
			, inq.BeforeDiscDPP
			, inq.DiscExcludePPn
			, inq.DiscIncludePPn
			, inq.AfterDiscDPP
			, inq.AfterDiscPPn
			, inq.AfterDiscPPnBM
			, inq.AfterDiscTotal
			, inq.PPnBMPaid
			, inq.OthersDPP
			, inq.OthersPPn
			, inq.ShipAmt
			, inq.DepositAmt
			, inq.OthersAmt
			from omHstInquirySales inq with (nolock, nowait)
			Left Join GnMstLookUpDtl grdDtl on inq.CompanyCode = grdDtl.CompanyCode
				and grdDtl.CodeID = 'ITSG'
				and grdDtl.LookUpValue = inq.Grade
			left join gnMstDealerMapping map
				on map.DealerCode = inq.CompanyCode
			where convert(varchar, inq.SuzukiFPolDate, 112) between convert(varchar, @StartDate, 112) and convert(varchar, @EndDate, 112)
			and isnull(inq.CompanyCode,'') like case when @Dealer <> '' then @Dealer else '%%' end
			and isnull(inq.BranchCode,'') like case when @Outlet <> '' then @Outlet else '%%' end 
			--and isnull(inq.Area,'') like case when @Area <> '' then @Area else '%%' end
			and (inq.Area like Case when @Area <> '' 
								then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
										then 'JABODETABEK'
										else @Area end
								else '%%' end
			or inq.Area like Case when @Area <> '' 
								then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
										then 'CABANG'
										else @Area end
								else '%%' end) 			
			and isnull(inq.BranchHeadID,'') like case when @BranchHead <> '' then @BranchHead else '%%' end  
			and isnull(inq.SalesHeadID,'') like case when @SalesHead <> '' then @SalesHead else '%%' end   
			and isnull(inq.SalesCoordinatorID,'') like case when @SalesCoordinator <> '' then @SalesCoordinator else '%%' end    
			and isnull(inq.SalesmanID,'') like case when @Salesman <> '' then @Salesman else '%%' end   
			and isnull(inq.MarketModel,'') <> ''
			and inq.Status = '1'
			and map.isActive = 1
			order by map.GroupNo,CompanyCode, inq.BranchCode, inq.BranchHeadName, inq.SalesHeadName, inq.SalesCoordinatorName, inq.SalesmanName, inq.MarketModel
	END
end

else

begin

if @SalesType = 'SALES'
begin
select map.GroupNo 
	, inq.Area
	, case when inq.CompanyCode = '6015402' then '6015401' else case when inq.CompanyCode = '6051402' then '6051401' else inq.CompanyCode end end CompanyCode
	, inq.CompanyName
	, inq.BranchCode
	, inq.BranchName
	, inq.BranchHeadID
	, inq.BranchHeadName
	, inq.SalesHeadID
	, inq.SalesHeadName
	, inq.SalesCoordinatorID
	, inq.SalesCoordinatorName
	, inq.SalesmanID
	, inq.SalesmanName
	, inq.ModelCatagory
	, inq.SalesType
	, inq.InvoiceNo
	, inq.SuzukiDoDate InvoiceDate
	, substring(inq.SONo,1,13) SoNo
	, inq.SalesModelCode
	, inq.SalesModelYear
	, inq.SalesModelDesc
	, inq.FakturPolisiNo
	, inq.SuzukiFPolDate FakturPolisiDate
	, inq.FakturPolisiDesc
	, inq.MarketModel
	, inq.ColourCode
	, inq.ColourName
	, inq.GroupMarketModel
	, inq.ColumnMarketModel
	, inq.JoinDate
	, inq.ResignDate
	, inq.GradeDate
	, case when isnull(grdDtl.LookUpValueName,'') <> '' then grdDtl.LookUpValueName else inq.Grade end Grade
	, inq.ChassisCode  
	, inq.ChassisNo  
	, inq.EngineCode  
	, inq.EngineNo  
	, inq.COGS
	, inq.BeforeDiscDPP
	, inq.DiscExcludePPn
	, inq.DiscIncludePPn
	, inq.AfterDiscDPP
	, inq.AfterDiscPPn
	, inq.AfterDiscPPnBM
	, inq.AfterDiscTotal
	, inq.PPnBMPaid
	, inq.OthersDPP
	, inq.OthersPPn
	, inq.ShipAmt
	, inq.DepositAmt
	, inq.OthersAmt
	from omHstInquirySales inq with (nolock, nowait)
	Left Join GnMstLookUpDtl grdDtl on inq.CompanyCode = grdDtl.CompanyCode
		and grdDtl.CodeID = 'ITSG'
		and grdDtl.LookUpValue = inq.Grade
	left join gnMstDealerMapping map
		on map.DealerCode = inq.CompanyCode
	where convert(varchar,inq.SuzukiDODate,112) between convert(varchar,@StartDate,112) and convert(varchar,@EndDate,112)
	and isnull(inq.CompanyCode,'') like case when @Dealer <> '' then @Dealer else '%%' end
	and isnull(inq.BranchCode,'') like case when @Outlet <> '' then @Outlet else '%%' end 
	--and isnull(inq.Area,'') like case when @Area <> '' then @Area else '%%' end
	and (inq.Area like Case when @Area <> '' 
						then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
								then 'JABODETABEK'
								else @Area end
						else '%%' end
	or inq.Area like Case when @Area <> '' 
						then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
								then 'CABANG'
								else @Area end
						else '%%' end) 	
	and isnull(inq.BranchHeadID,'') like case when @BranchHead <> '' then @BranchHead else '%%' end  
	and isnull(inq.SalesHeadID,'') like case when @SalesHead <> '' then @SalesHead else '%%' end   
	and isnull(inq.SalesCoordinatorID,'') like case when @SalesCoordinator <> '' then @SalesCoordinator else '%%' end    
	and isnull(inq.SalesmanID,'') like case when @Salesman <> '' then @Salesman else '%%' end   
	and inq.Status = '1'
	and map.isActive = 1
union all
select c.GroupNo 
	, c.Area
	, case when a.CustomerCode = '6015402' then '6015401' else case when a.CustomerCode = '6051402' then '6051401' else a.CustomerCode end end CompanyCode
	, isnull(c.DealerAbbreviation,a.CustomerCode) CompanyName
	, isnull(d.OutletCode,'HQ') BranchCode
	, isnull(d.OutletAbbreviation,'HQ') BranchName
	, '' BranchHeadID
	, '' BranchHeadName
	, '' SalesHeadID
	, '' SalesHeadName
	, '' SalesCoordinatorID
	, '' SalesCoordinatorName
	, '' SalesmanID
	, '' SalesmanName
	, '' ModelCatagory
	, '' SalesTyper
	, '' InvoiceNo
	, a.DODate InvoiceDate
	, '' SoNo
	, a.SalesModelCode
	, '1900' SalesModelYear
	, b.SalesModelDesc
	, a.FPNo
	, a.PROCESSDATE
	, '' FakturPolisiDesc
	, '' MarketModel
	, '' ColourCode
	, '' ColourName
	, b.GroupMarketModel
	, b.ColumnMarketModel
	, '1900-01-01' JoinDate
	, '1900-01-01' ResignDate
	, '1900-01-01' GradeDate
	, '' Grade
	, a.ChassisCode  
	, a.ChassisNo  
	, a.EngineCode  
	, a.EngineNo  
	, 0 COGS
	, 0 BeforeDiscDPP
	, 0 DiscExcludePPn
	, 0 DiscIncludePPn
	, 0 AfterDiscDPP
	, 0 AfterDiscPPn
	, 0 AfterDiscPPnBM
	, 0 AfterDiscTotal
	, 0 PPnBMPaid
	, 0 OthersDPP
	, 0 OthersPPn
	, 0 ShipAmt
	, 0 DepositAmt
	, 0 OthersAmt
from OmHstInquirySalesNSDS a with (nolock, nowait)
left join OmMstModel b on a.SalesModelCode = b.SalesModelCode
left join GnMstDealerMapping c on  a.CustomerCode = c.DealerCode
left join GnMstDealerOutletMapping d on d.DealerCode = a.CustomerCode
	and d.LastUpdateBy = 'HQ'
where convert(varchar,a.DODATE,112) between convert(varchar,@StartDate,112) and convert(varchar,@EndDate,112)
	and isnull(c.DealerCode,'') like case when isnull(@Dealer,'') <> '' then @Dealer else '%%' end
	and isnull(d.OutletCode,'') like case when isnull(@Outlet,'') <> '' then @Outlet else '%%' end 
	--and isnull(c.Area,'') like case when isnull(@Area,'') <> '' then @Area else '%%' end 
	and (c.Area like Case when @Area <> '' 
						then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
								then 'JABODETABEK'
								else @Area end
						else '%%' end
	or c.Area like Case when @Area <> '' 
						then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
								then 'CABANG'
								else @Area end
						else '%%' end) 
	and a.GroupAreaCode <> '3' 
	and IsExists = 0
	and c.isActive = 1
order by map.GroupNo,CompanyCode, inq.BranchCode, inq.BranchHeadName, inq.SalesHeadName, inq.SalesCoordinatorName, inq.SalesmanName, inq.MarketModel

end

if @SalesType = 'WHOLESALE'
begin	
select map.GroupNo 
	, inq.Area
	, case when inq.CompanyCode = '6015402' then '6015401' else case when inq.CompanyCode = '6051402' then '6051401' else inq.CompanyCode end end CompanyCode
	, inq.CompanyName
	, inq.BranchCode
	, inq.BranchName
	, inq.BranchHeadID
	, inq.BranchHeadName
	, inq.SalesHeadID
	, inq.SalesHeadName
	, inq.SalesCoordinatorID
	, inq.SalesCoordinatorName
	, inq.SalesmanID
	, inq.SalesmanName
	, inq.ModelCatagory
	, inq.SalesType
	, inq.InvoiceNo
	, inq.SuzukiDoDate InvoiceDate
	, substring(inq.SONo,1,13) SoNo
	, inq.SalesModelCode
	, inq.SalesModelYear
	, inq.SalesModelDesc
	, inq.FakturPolisiNo
	, inq.SuzukiFPolDate FakturPolisiDate
	, inq.FakturPolisiDesc
	, inq.MarketModel
	, inq.ColourCode
	, inq.ColourName
	, inq.GroupMarketModel
	, inq.ColumnMarketModel
	, inq.JoinDate
	, inq.ResignDate
	, inq.GradeDate
	, case when isnull(grdDtl.LookUpValueName,'') <> '' then grdDtl.LookUpValueName else inq.Grade end Grade
	, inq.ChassisCode  
	, inq.ChassisNo  
	, inq.EngineCode  
	, inq.EngineNo  
	, inq.COGS
	, inq.BeforeDiscDPP
	, inq.DiscExcludePPn
	, inq.DiscIncludePPn
	, inq.AfterDiscDPP
	, inq.AfterDiscPPn
	, inq.AfterDiscPPnBM
	, inq.AfterDiscTotal
	, inq.PPnBMPaid
	, inq.OthersDPP
	, inq.OthersPPn
	, inq.ShipAmt
	, inq.DepositAmt
	, inq.OthersAmt
	from omHstInquirySales inq with (nolock, nowait)
	Left Join GnMstLookUpDtl grdDtl on inq.CompanyCode = grdDtl.CompanyCode
		and grdDtl.CodeID = 'ITSG'
		and grdDtl.LookUpValue = inq.Grade
	left join gnMstDealerMapping map
		on map.DealerCode = inq.CompanyCode
	where convert(varchar,inq.SuzukiDODate,112) between convert(varchar,@StartDate,112) and convert(varchar,@EndDate,112)
	and isnull(inq.CompanyCode,'') like case when @Dealer <> '' then @Dealer else '%%' end
	and isnull(inq.BranchCode,'') like case when @Outlet <> '' then @Outlet else '%%' end 
	--and isnull(inq.Area,'') like case when @Area <> '' then @Area else '%%' end
	and (inq.Area like Case when @Area <> '' 
						then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
								then 'JABODETABEK'
								else @Area end
						else '%%' end
	or inq.Area like Case when @Area <> '' 
						then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
								then 'CABANG'
								else @Area end
						else '%%' end) 	
	--and isnull(inq.BranchHeadName,'') like case when @BranchHead <> '' then @BranchHead else '%%' end  
	and isnull(inq.BranchHeadID,'') like case when @BranchHead <> '' then @BranchHead else '%%' end  
	and isnull(inq.SalesHeadID,'') like case when @SalesHead <> '' then @SalesHead else '%%' end   
	and isnull(inq.SalesCoordinatorID,'') like case when @SalesCoordinator <> '' then @SalesCoordinator else '%%' end    
	and isnull(inq.SalesmanID,'') like case when @Salesman <> '' then @Salesman else '%%' end   
	and inq.Status = '1'
	and map.isActive = 1
--union all
--select c.GroupNo 
--	, c.Area
--	, case when a.CustomerCode = '6015402' then '6015401' else case when a.CustomerCode = '6051402' then '6051401' else a.CustomerCode end end CompanyCode
--	, isnull(c.DealerAbbreviation,a.CustomerCode) CompanyName
--	, isnull(d.OutletCode,'HQ') BranchCode
--	, isnull(d.OutletAbbreviation,'HQ') BranchName
--	, '' BranchHeadID
--	, '' BranchHeadName
--	, '' SalesHeadID
--	, '' SalesHeadName
--	, '' SalesCoordinatorID
--	, '' SalesCoordinatorName
--	, '' SalesmanID
--	, '' SalesmanName
--	, '' ModelCatagory
--	, '' SalesType
--	, '' InvoiceNo
--	, a.DODate InvoiceDate
--	, '' SoNo
--	, a.SalesModelCode
--	, '1900' SalesModelYear
--	, b.SalesModelDesc
--	, a.FPNo
--	, a.PROCESSDATE
--	, '' FakturPolisiDesc
--	, '' MarketModel
--	, '' ColourCode
--	, '' ColourName
--	, b.GroupMarketModel
--	, b.ColumnMarketModel
--	, '1900-01-01' JoinDate
--	, '1900-01-01' ResignDate
--	, '1900-01-01' GradeDate
--	, '' Grade
--	, a.ChassisCode  
--	, a.ChassisNo  
--	, a.EngineCode  
--	, a.EngineNo  
--	, 0 COGS
--	, 0 BeforeDiscDPP
--	, 0 DiscExcludePPn
--	, 0 DiscIncludePPn
--	, 0 AfterDiscDPP
--	, 0 AfterDiscPPn
--	, 0 AfterDiscPPnBM
--	, 0 AfterDiscTotal
--	, 0 PPnBMPaid
--	, 0 OthersDPP
--	, 0 OthersPPn
--	, 0 ShipAmt
--	, 0 DepositAmt
--	, 0 OthersAmt
--from OmHstInquirySalesNSDS a with (nolock, nowait)
--left join OmMstModel b on a.SalesModelCode = b.SalesModelCode
--left join GnMstDealerMapping c on  a.CustomerCode = c.DealerCode
--left join GnMstDealerOutletMapping d on d.DealerCode = a.CustomerCode
--	and d.LastUpdateBy = 'HQ'
--where convert(varchar,a.DODATE,112) between convert(varchar,@StartDate,112) and convert(varchar,@EndDate,112)
--	and isnull(c.DealerCode,'') like case when @Dealer <> '' then @Dealer else '%%' end
--	and isnull(d.OutletCode,'') like case when @Outlet <> '' then @Outlet else '%%' end 
--	and isnull(c.Area,'') like case when @Area <> '' then @Area else '%%' end
--	and GroupAreaCode <> '3' 
--	and IsExists = 0
--order by map.GroupNo,CompanyCode, inq.BranchCode, inq.BranchHeadName, inq.SalesHeadName, inq.SalesCoordinatorName, inq.SalesmanName, inq.MarketModel
end
if @SalesType = 'RETAIL'
begin
select map.GroupNo 
	, inq.Area
	, case when inq.CompanyCode = '6015402' then '6015401' else case when inq.CompanyCode = '6051402' then '6051401' else inq.CompanyCode end end CompanyCode
	, inq.CompanyName
	, inq.BranchCode
	, inq.BranchName
	, inq.BranchHeadID
	, inq.BranchHeadName
	, inq.SalesHeadID
	, inq.SalesHeadName
	, inq.SalesCoordinatorID
	, inq.SalesCoordinatorName
	, inq.SalesmanID
	, inq.SalesmanName
	, inq.ModelCatagory
	, inq.SalesType
	, inq.InvoiceNo
	, inq.SuzukiDoDate InvoiceDate
	, substring(inq.SONo,1,13) SoNo
	, inq.SalesModelCode
	, inq.SalesModelYear
	, inq.SalesModelDesc
	, inq.FakturPolisiNo
	, inq.SuzukiFPolDate FakturPolisiDate
	, inq.FakturPolisiDesc
	, inq.MarketModel
	, inq.ColourCode
	, inq.ColourName
	, inq.GroupMarketModel
	, inq.ColumnMarketModel
	, inq.JoinDate
	, inq.ResignDate
	, inq.GradeDate
	, case when isnull(grdDtl.LookUpValueName,'') <> '' then grdDtl.LookUpValueName else inq.Grade end Grade
	, inq.ChassisCode  
	, inq.ChassisNo  
	, inq.EngineCode  
	, inq.EngineNo  
	, inq.COGS
	, inq.BeforeDiscDPP
	, inq.DiscExcludePPn
	, inq.DiscIncludePPn
	, inq.AfterDiscDPP
	, inq.AfterDiscPPn
	, inq.AfterDiscPPnBM
	, inq.AfterDiscTotal
	, inq.PPnBMPaid
	, inq.OthersDPP
	, inq.OthersPPn
	, inq.ShipAmt
	, inq.DepositAmt
	, inq.OthersAmt
	from omHstInquirySales inq with (nolock, nowait)
	Left Join GnMstLookUpDtl grdDtl on inq.CompanyCode = grdDtl.CompanyCode
		and grdDtl.CodeID = 'ITSG'
		and grdDtl.LookUpValue = inq.Grade
	left join gnMstDealerMapping map
		on map.DealerCode = inq.CompanyCode
	where convert(varchar,inq.SuzukiDODate,112) between convert(varchar,@StartDate,112) and convert(varchar,@EndDate,112)
	and isnull(inq.CompanyCode,'') like case when @Dealer <> '' then @Dealer else '%%' end
	and isnull(inq.BranchCode,'') like case when @Outlet <> '' then @Outlet else '%%' end 
	--and isnull(inq.Area,'') like case when @Area <> '' then @Area else '%%' end
	and (inq.Area like Case when @Area <> '' 
						then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
								then 'JABODETABEK'
								else @Area end
						else '%%' end
	or inq.Area like Case when @Area <> '' 
						then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
								then 'CABANG'
								else @Area end
						else '%%' end) 	
						--and isnull(inq.BranchHeadName,'') like case when @BranchHead <> '' then @BranchHead else '%%' end  
	and isnull(inq.BranchHeadID,'') like case when @BranchHead <> '' then @BranchHead else '%%' end  
	and isnull(inq.SalesHeadID,'') like case when @SalesHead <> '' then @SalesHead else '%%' end   
	and isnull(inq.SalesCoordinatorID,'') like case when @SalesCoordinator <> '' then @SalesCoordinator else '%%' end    
	and isnull(inq.SalesmanID,'') like case when @Salesman <> '' then @Salesman else '%%' end   
	and isnull(inq.MarketModel,'') <> ''
	and inq.Status = '1'
	and (inq.SoNo not like '%MD' and inq.SoNo not like '%SD') 
	and map.isActive = 1
order by map.GroupNo,inq.CompanyCode, inq.BranchCode, inq.BranchHeadName, inq.SalesHeadName, inq.SalesCoordinatorName, inq.SalesmanName, inq.MarketModel
end
if @SalesType = 'FPOL'
begin
select map.GroupNo 
	, inq.Area
	, case when inq.CompanyCode = '6015402' then '6015401' else case when inq.CompanyCode = '6051402' then '6051401' else inq.CompanyCode end end CompanyCode
	, inq.CompanyName
	, inq.BranchCode
	, inq.BranchName
	, inq.BranchHeadID
	, inq.BranchHeadName
	, inq.SalesHeadID
	, inq.SalesHeadName
	, inq.SalesCoordinatorID
	, inq.SalesCoordinatorName
	, inq.SalesmanID
	, inq.SalesmanName
	, inq.ModelCatagory
	, inq.SalesType
	, inq.InvoiceNo
	, inq.SuzukiFPOLDate InvoiceDate
	, substring(inq.SONo,1,13) SoNo
	, inq.SalesModelCode
	, inq.SalesModelYear
	, inq.SalesModelDesc
	, inq.FakturPolisiNo
	, inq.SuzukiFPolDate FakturPolisiDate
	, inq.FakturPolisiDesc
	, inq.MarketModel
	, inq.ColourCode
	, inq.ColourName
	, inq.GroupMarketModel
	, inq.ColumnMarketModel
	, inq.JoinDate
	, inq.ResignDate
	, inq.GradeDate
	, case when isnull(grdDtl.LookUpValueName,'') <> '' then grdDtl.LookUpValueName else inq.Grade end Grade
	, inq.ChassisCode  
	, inq.ChassisNo  
	, inq.EngineCode  
	, inq.EngineNo  
	, inq.COGS
	, inq.BeforeDiscDPP
	, inq.DiscExcludePPn
	, inq.DiscIncludePPn
	, inq.AfterDiscDPP
	, inq.AfterDiscPPn
	, inq.AfterDiscPPnBM
	, inq.AfterDiscTotal
	, inq.PPnBMPaid
	, inq.OthersDPP
	, inq.OthersPPn
	, inq.ShipAmt
	, inq.DepositAmt
	, inq.OthersAmt
	from omHstInquirySales inq with (nolock, nowait)
	Left Join GnMstLookUpDtl grdDtl on inq.CompanyCode = grdDtl.CompanyCode
		and grdDtl.CodeID = 'ITSG'
		and grdDtl.LookUpValue = inq.Grade
	left join gnMstDealerMapping map
		on map.DealerCode = inq.CompanyCode
	where convert(varchar,inq.SuzukiFPolDate,112) between convert(varchar,@StartDate,112) and convert(varchar,@EndDate,112)
	and isnull(inq.CompanyCode,'') like case when @Dealer <> '' then @Dealer else '%%' end
	and isnull(inq.BranchCode,'') like case when @Outlet <> '' then @Outlet else '%%' end 
	--and isnull(inq.Area,'') like case when @Area <> '' then @Area else '%%' end
	and (inq.Area like Case when @Area <> '' 
						then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
								then 'JABODETABEK'
								else @Area end
						else '%%' end
	or inq.Area like Case when @Area <> '' 
						then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
								then 'CABANG'
								else @Area end
						else '%%' end) 	--and isnull(inq.BranchHeadName,'') like case when @BranchHead <> '' then @BranchHead else '%%' end  
	and isnull(inq.BranchHeadID,'') like case when @BranchHead <> '' then @BranchHead else '%%' end  
	and isnull(inq.SalesHeadID,'') like case when @SalesHead <> '' then @SalesHead else '%%' end   
	and isnull(inq.SalesCoordinatorID,'') like case when @SalesCoordinator <> '' then @SalesCoordinator else '%%' end    
	and isnull(inq.SalesmanID,'') like case when @Salesman <> '' then @Salesman else '%%' end   
	and isnull(inq.MarketModel,'') <> ''
	and inq.Status = '1'
	and map.isActive = 1
--union
--select c.GroupNo 
--	, c.Area
--	, case when a.CustomerCode = '6015402' then '6015401' else case when a.CustomerCode = '6051402' then '6051401' else a.CustomerCode end end CompanyCode
--	, isnull(c.DealerAbbreviation,a.CustomerCode) CompanyName
--	, isnull(d.OutletCode,'HQ') BranchCode
--	, isnull(d.OutletAbbreviation,'HQ') BranchName
--	, '' BranchHeadID
--	, '' BranchHeadName
--	, '' SalesHeadID
--	, '' SalesHeadName
--	, '' SalesCoordinatorID
--	, '' SalesCoordinatorName
--	, '' SalesmanID
--	, '' SalesmanName
--	, '' ModelCatagory
--	, '' SalesType
--	, '' InvoiceNo
--	, a.ProcessDate InvoiceDate
--	, '' SoNo
--	, a.SalesModelCode
--	, '1900' SalesModelYear
--	, b.SalesModelDesc
--	, a.FPNo
--	, a.PROCESSDATE
--	, '' FakturPolisiDesc
--	, '' MarketModel
--	, '' ColourCode
--	, '' ColourName
--	, b.GroupMarketModel
--	, b.ColumnMarketModel
--	, '1900-01-01' JoinDate
--	, '1900-01-01' ResignDate
--	, '1900-01-01' GradeDate
--	, '' Grade
--	, a.ChassisCode  
--	, a.ChassisNo  
--	, a.EngineCode  
--	, a.EngineNo  
--	, 0 COGS
--	, 0 BeforeDiscDPP
--	, 0 DiscExcludePPn
--	, 0 DiscIncludePPn
--	, 0 AfterDiscDPP
--	, 0 AfterDiscPPn
--	, 0 AfterDiscPPnBM
--	, 0 AfterDiscTotal
--	, 0 PPnBMPaid
--	, 0 OthersDPP
--	, 0 OthersPPn
--	, 0 ShipAmt
--	, 0 DepositAmt
--	, 0 OthersAmt
--from OmHstInquirySalesNSDS a with (nolock, nowait)
--left join OmMstModel b on a.SalesModelCode = b.SalesModelCode
--left join GnMstDealerMapping c on  a.CustomerCode = c.DealerCode
--left join GnMstDealerOutletMapping d on d.DealerCode = a.CustomerCode
--	and d.LastUpdateBy = 'HQ'
--where a.ProcessDate between @StartDate and @EndDate
--	and isnull(c.DealerCode,'') like case when @Dealer <> '' then @Dealer else '%%' end
--	and isnull(d.OutletCode,'') like case when @Outlet <> '' then @Outlet else '%%' end 
--	and isnull(c.Area,'') like case when @Area <> '' then @Area else '%%' end 
--	and GroupAreaCode <> '3' 
--	and IsExists = 0
--	and convert(varchar,ProcessDate,112) between convert(varchar,@StartDate,112) and convert(varchar,@EndDate,112)
--	and not exists (select 1 
--					from OmHstInquirySales
--					where convert(varchar,SuzukiFPolDate,112) between convert(varchar,@StartDate,112) and convert(varchar,@EndDate,112)
--						and a.ChassisCode = ChassisCode
--						and a.ChassisNo = ChassisNo
--						and MarketModel is not null
--						and Status = '1')
--order by map.GroupNo,CompanyCode, inq.BranchCode, inq.BranchHeadName, inq.SalesHeadName, inq.SalesCoordinatorName, inq.SalesmanName, inq.MarketModel
end

if @SalesType = ''
begin
select map.GroupNo 
	, inq.Area
	, inq.CompanyCode
	, inq.CompanyName
	, inq.BranchCode
	, inq.BranchName
	, inq.BranchHeadID
	, inq.BranchHeadName
	, inq.SalesHeadID
	, inq.SalesHeadName
	, inq.SalesCoordinatorID
	, inq.SalesCoordinatorName
	, inq.SalesmanID
	, inq.SalesmanName
	, inq.ModelCatagory
	, inq.SalesType
	, inq.InvoiceNo
	, inq.SuzukiDoDate InvoiceDate
	, substring(inq.SONo,1,13) SoNo
	, inq.SalesModelCode
	, inq.SalesModelYear
	, inq.SalesModelDesc
	, inq.FakturPolisiNo
	, inq.SuzukiFPolDate FakturPolisiDate
	, inq.FakturPolisiDesc
	, inq.MarketModel
	, inq.ColourCode
	, inq.ColourName
	, inq.GroupMarketModel
	, inq.ColumnMarketModel
	, inq.JoinDate
	, inq.ResignDate
	, inq.GradeDate
	, case when isnull(grdDtl.LookUpValueName,'') <> '' then grdDtl.LookUpValueName else inq.Grade end Grade
	, inq.ChassisCode  
	, inq.ChassisNo  
	, inq.EngineCode  
	, inq.EngineNo  
	, inq.COGS
	, inq.BeforeDiscDPP
	, inq.DiscExcludePPn
	, inq.DiscIncludePPn
	, inq.AfterDiscDPP
	, inq.AfterDiscPPn
	, inq.AfterDiscPPnBM
	, inq.AfterDiscTotal
	, inq.PPnBMPaid
	, inq.OthersDPP
	, inq.OthersPPn
	, inq.ShipAmt
	, inq.DepositAmt
	, inq.OthersAmt
	from omHstInquirySales inq with (nolock, nowait)
	Left Join GnMstLookUpDtl grdDtl on inq.CompanyCode = grdDtl.CompanyCode
		and grdDtl.CodeID = 'ITSG'
		and grdDtl.LookUpValue = inq.Grade
	left join gnMstDealerMapping map
		on map.DealerCode = inq.CompanyCode
	where convert(varchar,inq.SuzukiDODate,112) between convert(varchar,@StartDate,112) and convert(varchar,@EndDate,112)
	and isnull(inq.CompanyCode,'') like case when @Dealer <> '' then @Dealer else '%%' end
	and isnull(inq.BranchCode,'') like case when @Outlet <> '' then @Outlet else '%%' end 
	--and isnull(inq.Area,'') like case when @Area <> '' then @Area else '%%' end
	and (inq.Area like Case when @Area <> '' 
						then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
								then 'JABODETABEK'
								else @Area end
						else '%%' end
	or inq.Area like Case when @Area <> '' 
						then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
								then 'CABANG'
								else @Area end
						else '%%' end) 	--and isnull(inq.BranchHeadName,'') like case when @BranchHead <> '' then @BranchHead else '%%' end  
	and isnull(inq.BranchHeadID,'') like case when @BranchHead <> '' then @BranchHead else '%%' end  
	and isnull(inq.SalesHeadID,'') like case when @SalesHead <> '' then @SalesHead else '%%' end   
	and isnull(inq.SalesCoordinatorID,'') like case when @SalesCoordinator <> '' then @SalesCoordinator else '%%' end    
	and isnull(inq.SalesmanID,'') like case when @Salesman <> '' then @Salesman else '%%' end   
	and isnull(inq.MarketModel,'') <> ''
	and inq.Status = '1'
	and map.isActive = 1
end

end

-- uspfn_OmInquirySales '6006406','6006406','2012-1-1','2012-4-30','','','','','','',''
--select * from omHstInquirySales where (SoNo like '%SD' or SoNo like '%MD')
--select * from #t1
--select * from #t5
--select * from gnMstEmployee where EmployeeID='00288'
--update gnMstEmployee set PersonnelStatus=1 where EmployeeID='00288'

--select * from pmMstTeamMembers where EmployeeID='S20013'
--select * from pmMstTeamMembers where BranchCode='6006402' and TeamID='108' and IsSupervisor=1USE [DMS_V2]
GO
IF object_id('usprpt_OmRpSalRgs039PivotWeb') IS NOT NULL
	DROP PROCEDURE usprpt_OmRpSalRgs039PivotWeb
GO
-- usprpt_OmRpSalRgs039Pivot '2012-10-1','2012-10-31','','','','','','','','SALES'
CREATE procedure [dbo].[usprpt_OmRpSalRgs039PivotWeb]
	@StartDate Datetime,
	@EndDate Datetime,
	@Area varchar(100),
	@Dealer varchar(100),
	@Outlet varchar(100),
	@BranchHead varchar(100),
	@SalesHead varchar(100),
	@SalesCoordinator varchar(100),
	@Salesman varchar(100),
	@SalesType varchar(100)
as 
Declare @MainTable table
(
	GroupNo					varchar(150)
	, Area					varchar(150)
	, CompanyCode			varchar(150)
	, CompanyName			varchar(150)
	, BranchCode			varchar(150)
	, BranchName			varchar(150)
	, BranchHeadID			varchar(150)
	, BranchHeadName		varchar(150)
	, SalesHeadID			varchar(150)
	, SalesHeadName			varchar(150)
	, SalesCoordinatorID	varchar(150)
	, SalesCoordinatorName	varchar(150)
	, SalesmanID			varchar(150)
	, SalesmanName			varchar(150)
	, ModelCatagory			varchar(150)
	, SalesType				varchar(150)
	, InvoiceNo				varchar(150)
	, InvoiceDate			datetime
	, SONo					varchar(150)
	, SalesModelCode		varchar(150)
	, SalesModelYear		numeric(4,0)
	, SalesModelDesc		varchar(150)
	, FakturPolisiNo		varchar(150)
	, FakturPolisiDate		datetime
	, FakturPolisiDesc		varchar(150)
	, MarketModel			varchar(150)
	, ColourCode			varchar(150)
	, ColourName			varchar(150)
	, GroupMarketModel		varchar(150)
	, ColumnMarketModel		varchar(150)
	, JoinDate				datetime
	, ResignDate			datetime
	, GradeDate				datetime
	, Grade					varchar(150)
	, ChassisCode			varchar(150)
	, ChassisNo				varchar(150)
	, EngineCode			varchar(150) 
	, EngineNo				varchar(150)
	, COGS					numeric(18,0)
	, BeforeDiscDPP			numeric(18,0)
	, DiscExcludePPn		numeric(18,0)
	, DiscIncludePPn		numeric(18,0)
	, AfterDiscDPP			numeric(18,0)
	, AfterDiscPPn			numeric(18,0)
	, AfterDiscPPnBM		numeric(18,0)
	, AfterDiscTotal		numeric(18,0)
	, PPnBMPaid				numeric(18,0)
	, OthersDPP				numeric(18,0)
	, OthersPPn				numeric(18,0)
	, ShipAmt				numeric(18,0)
	, DepositAmt			numeric(18,0)
	, OthersAmt				numeric(18,0)
)

insert into @MainTable
exec uspfn_OmInquirySalesWeb @StartDate, @EndDate, @Area, @Dealer, @Outlet, @BranchHead, @SalesHead, @SalesCoordinator, @Salesman, @SalesType

select * into #t1 from(
select Area
		, CompanyCode
		, CompanyName
		, BranchCode
		, BranchName
		, BranchHeadName
		, SalesHeadName
		, SalesCoordinatorName
		, SalesmanName
		, SalesType
		, SalesModelCode
		, SalesModelYear
		, SalesModelDesc
		, FakturPolisiDesc
		, case when GroupMarketModel = '' then 'XYZ' else case when substring(GroupMarketModel,4,1) = '.' then Right(GroupMarketModel,LEN(GroupMarketModel) - 4) else GroupMarketModel end end GroupMarketModel
		, case when ColumnMarketModel = '' then 'XYZ' else case when substring(ColumnMarketModel,4,1) = '.' then Right(ColumnMarketModel,LEN(ColumnMarketModel) - 4) else ColumnMarketModel end end ColumnMarketModel
		, Grade
		, ModelCatagory
		, MarketModel
		, ColourCode
		, ColourName
		, YEAR(InvoiceDate) Year 
		, MONTH(InvoiceDate) Month
		, InvoiceDate
		, count(ChassisCode) SoldTotal
  from @MainTable
  group by Area 
		, CompanyCode
		, CompanyName
		, BranchCode
		, BranchName
		, BranchHeadName
		, SalesHeadName
		, SalesCoordinatorName
		, SalesmanName
		, SalesType
		, SalesModelCode
		, SalesModelYear
		, SalesModelDesc
		, FakturPolisiDesc
		, GroupMarketModel
		, ColumnMarketModel
		, Grade
		, ModelCatagory
		, MarketModel
		, ColourCode
		, ColourName
		, YEAR(InvoiceDate) 
		, MONTH(InvoiceDate)
		, InvoiceDate
)#t1

Declare @NSDS table
(
	Area					varchar(100)
	, CompanyCode			varchar(15)
	, CompanyName			varchar(100)
	, BranchCode			varchar(15)
	, BranchName			varchar(100)
	, BranchHeadName		varchar(100)
	, SalesHeadName			varchar(100)
	, SalesCoordinatorName	varchar(100)
	, SalesmanName			varchar(100)
	, SalesType				varchar(25)
	, SalesModelCode		varchar(25)
	, SalesModelYear		numeric(4,0)
	, SalesModelDesc		varchar(150)
	, FakturPolisiDesc		varchar(100)
	, GroupMarketModel		varchar(100)
	, ColumnMarketModel		varchar(100)
	, Grade					varchar(50)
	, ModelCatagory			varchar(15)
	, MarketModel			varchar(25)
	, ColourCode			varchar(25)
	, ColourName			varchar(100)
	, Year					numeric(4,0)
	, Month					numeric(4,0)
	, InvoiceDate			datetime
	, SoldTotal				decimal(18,0)
)

if @SalesType <> 'WHOLESALE'
begin
	insert into @NSDS
		select c.Area
			, case when a.CustomerCode = '6015402' then '6015401' else case when a.CustomerCode = '6051402' then '6051401' else a.CustomerCode end end CompanyCode
			, isnull(c.DealerAbbreviation,a.CustomerCode) CompanyName
			, isnull(d.OutletCode,'') BranchCode
			, isnull(d.OutletAbbreviation,'HQ') BranchName
			, '' BranchHeadName
			, '' SalesHeadName
			, '' SalesCoordinatorName
			, '' SalesmanName
			, '' SalesType
			, a.SalesModelCode
			, 1900 SalesModelYear
			, b.SalesModelDesc
			, '' FakturPolisiDesc
			, b.GroupMarketModel
			, b.ColumnMarketModel
			, '' Grade
			, (select Top 1 ModelCatagory from @MainTable where a.SalesModelCode = SalesModelCode) ModelCategory
			, (select Top 1 MarketModel from @MainTable where a.SalesModelCode = SalesModelCode) MarketModel
			, '' ColourCode
			, '' ColourName
			, case when @SalesType in ('RETAIL','SALES') then YEAR(a.DODate) else YEAR(a.ProcessDate) end Year
			, case when @SalesType in ('RETAIL','SALES') then MONTH(a.DODate) else MONTH(a.ProcessDate) end Month
			, case when @SalesType in ('RETAIL','SALES') then YEAR(a.DODate) else YEAR(a.ProcessDate) end InvoiceDate
			, COUNT(a.ChassisCode) SoldTotal
		from OmHstInquirySalesNSDS a
		left join OmMstModel b on a.SalesModelCode = b.SalesModelCode
		left join GnMstDealerMapping c on  a.CustomerCode = c.DealerCode
		left join GnMstDealerOutletMapping d on d.DealerCode = a.CustomerCode
			and d.LastUpdateBy = 'HQ'
		where case when @SalesType in ('RETAIL','SALES') 
				then convert(varchar,a.DoDate,112) 
				else convert(varchar,a.ProcessDate,112) 
				end between convert(varchar,@StartDate,112) and convert(varchar,@EndDate,112) 
			--and isnull(c.Area,'') like case when isnull(@Area,'') <> '' then @Area else '%%' end
				and (c.Area like Case when @Area <> '' 
						then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
								then 'JABODETABEK'
								else @Area end
						else '%%' end
					or c.Area like Case when @Area <> '' 
										then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
												then 'CABANG'
												else @Area end
										else '%%' end)
			and isnull(c.DealerCode,'') like case when isnull(@Dealer,'') <> '' then @Dealer else '%%' end
			and isnull(d.OutletCode,'') like case when isnull(@Outlet,'') <> '' then @Outlet else '%%' end 
			and a.GroupAreaCode = '3'
		group by c.Area
			, a.CustomerCode
			, c.DealerAbbreviation
			, d.OutletCode
			, d.OutletAbbreviation
			, a.SalesModelcode
			, b.SalesModelDesc
			, b.GroupMarketModel
			, b.ColumnMarketModel
			, a.DoDate
			, a.ProcessDate
			
	select * from #t1
	union all
	select * from @NSDS 
End

if @SalesType = 'WHOLESALE'
	select * from #t1

drop table #t1
GO

IF object_id('usprpt_OmRpSalRgs039Web') IS NOT NULL
	DROP PROCEDURE usprpt_OmRpSalRgs039Web
GO
-- usprpt_OmRpSalRgs039 '2013-2-1','2013-2-19','CABANG','6006406','6006406','CECEP CAHYADI','IVAN RIKI SAEFUL','AWALUDIN','','RETAIL'
-- usprpt_OmRpSalRgs039 '2013-06-01','2013-06-30','JAWA TIMUR / BALI / LOMBOK','6006408','6006408','','','','',''
--@StartDate=N'2013-01-01',@EndDate=N'2013-06-30',@Area=N'',@Dealer=N'',@Outlet=N'',@BranchHead=N'',@SalesHead=N'',@SalesCoordinator=N'',@Salesman=N'',@SalesType=N'FPOL'
CREATE procedure [dbo].[usprpt_OmRpSalRgs039Web]
	@StartDate Datetime,
	@EndDate Datetime,
	@Area varchar(100),
	@Dealer varchar(100),
	@Outlet varchar(100),
	@BranchHead varchar(100),
	@SalesHead varchar(100),
	@SalesCoordinator varchar(100),
	@Salesman varchar(100),
	@SalesType varchar(100)
as 
Declare @MainTable table
(
	GroupNo					varchar(150)
	, Area					varchar(150)
	, CompanyCode			varchar(150)
	, CompanyName			varchar(150)
	, BranchCode			varchar(150)
	, BranchName			varchar(150)
	, BranchHeadID			varchar(150)
	, BranchHeadName		varchar(150)
	, SalesHeadID			varchar(150)
	, SalesHeadName			varchar(150)
	, SalesCoordinatorID	varchar(150)
	, SalesCoordinatorName	varchar(150)
	, SalesmanID			varchar(150)
	, SalesmanName			varchar(150)
	, ModelCatagory			varchar(150)
	, SalesType				varchar(150)
	, InvoiceNo				varchar(150)
	, InvoiceDate			datetime
	, SONo					varchar(150)
	, SalesModelCode		varchar(150)
	, Year					numeric(4,0)
	, SalesModelDesc		varchar(150)
	, FakturPolisiNo		varchar(150)
	, FakturPolisiDate		datetime
	, FakturPolisiDesc		varchar(150)
	, MarketModel			varchar(150)
	, ColourCode			varchar(150)
	, ColourName			varchar(100)
	, GroupMarketModel		varchar(100)
	, ColumnMarketModel		varchar(100)
	, JoinDate				datetime
	, ResignDate			datetime
	, GradeDate				datetime
	, Grade					varchar(150)
	, ChassisCode			varchar(150)
	, ChassisNo				varchar(150)
	, EngineCode			varchar(150) 
	, EngineNo				varchar(150)
	, COGS					numeric(18,0)
	, BeforeDiscDPP			numeric(18,0)
	, DiscExcludePPn		numeric(18,0)
	, DiscIncludePPn		numeric(18,0)
	, AfterDiscDPP			numeric(18,0)
	, AfterDiscPPn			numeric(18,0)
	, AfterDiscPPnBM		numeric(18,0)
	, AfterDiscTotal		numeric(18,0)
	, PPnBMPaid				numeric(18,0)
	, OthersDPP				numeric(18,0)
	, OthersPPn				numeric(18,0)
	, ShipAmt				numeric(18,0)
	, DepositAmt			numeric(18,0)
	, OthersAmt				numeric(18,0)
)

insert into @MainTable
exec uspfn_OmInquirySalesWeb @StartDate, @EndDate, @Area, @Dealer, @Outlet, @BranchHead, @SalesHead, @SalesCoordinator, @Salesman, @SalesType

select * into #t1 from(
select Area
		, CompanyCode
		, CompanyName
		, BranchCode
		, BranchName
		, BranchHeadID
		, BranchHeadName
		, SalesHeadID
		, SalesHeadName
		, SalesCoordinatorID
		, SalesCoordinatorName
		, SalesmanID
		, SalesmanName
		, SalesType
		, SalesModelCode
		, year SalesModelYear
		, SalesModelDesc
		, FakturPolisiDesc
		, case when GroupMarketModel = '' then 'XYZ' else case when substring(GroupMarketModel,4,1) = '.' then Right(GroupMarketModel,LEN(GroupMarketModel) - 4) else GroupMarketModel end end GroupMarketModel
		, case when ColumnMarketModel = '' then 'XYZ' else case when substring(ColumnMarketModel,4,1) = '.' then Right(ColumnMarketModel,LEN(ColumnMarketModel) - 4) else ColumnMarketModel end end ColumnMarketModel
		, Grade
		, ModelCatagory
		, MarketModel
		, ColourCode
		, ColourName
		, YEAR(InvoiceDate) Year 
		, MONTH(InvoiceDate) Month
		, case when YEAR(InvoiceDate) = 1900 then null else InvoiceDate end InvoiceDate
		, SoNo
		, InvoiceNo
		, FakturPolisiNo
		, case when YEAR(FakturPolisiDate) = 1900 then null else FakturPolisiDate end FakturPolisiDate
		, COGS
		, AfterDiscDPP DPP
		, OthersDPP DPPAccs
		, OthersAmt COGSAccs
		, (COGS - AfterDiscDPP) + (OthersDPP - OthersAmt) Margin
  from @MainTable
)#t1

Declare @NSDS table
(
	Area					varchar(100)
	, CompanyCode			varchar(15)
	, CompanyName			varchar(100)
	, BranchCode			varchar(15)
	, BranchName			varchar(100)
	, BranchHeadID			varchar(15)
	, BranchHeadName		varchar(100)
	, SalesHeadID			varchar(15)
	, SalesHeadName			varchar(100)
	, SalesCoordinatorID	varchar(15)
	, SalesCoordinatorName	varchar(100)
	, SalesmanID			varchar(15)
	, SalesmanName			varchar(100)
	, SalesType				varchar(25)
	, SalesModelCode		varchar(25)
	, SalesModelYear		numeric(4,0)
	, SalesModelDesc		varchar(150)
	, FakturPolisiDesc		varchar(150)
	, GroupMarketModel		varchar(100)
	, ColumnMarketModel		varchar(100)
	, Grade					varchar(50)
	, ModelCatagory			varchar(15)
	, MarketModel			varchar(25)
	, ColourCode			varchar(25)
	, ColourName			varchar(100)
	, Year					numeric(4,0)
	, Month					numeric(4,0)
	, InvoiceDate			datetime
	, SONo					varchar(15)
	, InvoiceNo				varchar(15)
	, FakturPolisiNo		varchar(15)
	, FakturPolisiDate		datetime
	, COGS					numeric(18,0)
	, DPP					numeric(18,0)
	, DPPAccs				numeric(18,0)
	, COGSAccs				numeric(18,0)
	, Margin				numeric(18,0)
)

if @SalesType <> 'WHOLESALE'
begin
	insert into @NSDS
		select c.Area
			, case when a.CustomerCode = '6015402' then '6015401' else case when a.CustomerCode = '6051402' then '6051401' else a.CustomerCode end end CompanyCode
			, isnull(c.DealerAbbreviation,a.CustomerCode) CompanyName
			, isnull(d.OutletCode,'') BranchCode
			, isnull(d.OutletAbbreviation,'HQ') BranchName
			, '' BranchHeadID
			, '' BranchHeadName
			, '' SalesHeadID
			, '' SalesHeadName
			, '' SalesCoordinatorID
			, '' SalesCoordinatorName
			, '' SalesmanID
			, '' SalesmanName
			, '' SalesType
			, a.SalesModelCode
			, 1900 SalesModelYear
			, b.SalesModelDesc
			, '' FakturPolisiDesc
			, b.GroupMarketModel
			, b.ColumnMarketModel
			, '' Grade
			, (select Top 1 ModelCatagory from @MainTable where a.SalesModelCode = SalesModelCode) ModelCategory
			, (select Top 1 MarketModel from @MainTable where a.SalesModelCode = SalesModelCode) MarketModel
			, '' ColourCode
			, '' ColourName
			, case when @SalesType in ('RETAIL','SALES') then YEAR(a.DODate) else YEAR(a.ProcessDate) end Year
			, case when @SalesType in ('RETAIL','SALES') then MONTH(a.DODate) else MONTH(a.ProcessDate) end Month
			, case when @SalesType in ('RETAIL','SALES') then YEAR(a.DODate) else YEAR(a.ProcessDate) end InvoiceDate
			, '' SoNo
			, '' InvoiceNo
			, a.FPNo
			, a.PROCESSDATE
			, 0 COGS
			, 0 DPP
			, 0 DPPAccs
			, 0 COGSAccs
			, 0 Margin
		from OmHstInquirySalesNSDS a
		left join OmMstModel b on a.SalesModelCode = b.SalesModelCode
		left join GnMstDealerMapping c on  a.CustomerCode = c.DealerCode
		left join GnMstDealerOutletMapping d on d.DealerCode = a.CustomerCode
			and d.LastUpdateBy = 'HQ'
		where case when @SalesType in ('RETAIL','SALES') 
				then convert(varchar,a.DoDate,112) 
				else convert(varchar,a.ProcessDate,112) 
				end between convert(varchar,@StartDate,112) and convert(varchar,@EndDate,112) 
			--and isnull(c.Area,'') like case when isnull(@Area,'') <> '' then @Area else '%%' end
	and (c.Area like Case when @Area <> '' 
						then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
								then 'JABODETABEK'
								else @Area end
						else '%%' end
	or c.Area like Case when @Area <> '' 
						then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
								then 'CABANG'
								else @Area end
						else '%%' end) 	
			and isnull(c.DealerCode,'') like case when isnull(@Dealer,'') <> '' then @Dealer else '%%' end
			and isnull(d.OutletCode,'') like case when isnull(@Outlet,'') <> '' then @Outlet else '%%' end 
			and a.GroupAreaCode = '3'
			and c.isActive = 1
	select * from #t1
	union all
	select * from @NSDS 
End

if @SalesType = 'WHOLESALE'
	select * from #t1

drop table #t1
GO

ALTER procedure [dbo].[usprpt_GnGenerateCsvSkemaTaxOut]
--DECLARE
	@CompanyCode varchar(15)
	,@BranchCode varchar(15)
	,@PeriodFrom smalldatetime
	,@PeriodTo smalldatetime
	,@ProductType varchar(2)
	,@table varchar(1)=1
as

--execute usprpt_GnGenerateCsvSkemaTaxOut '6006406','6006406','2014/03/01','2014/03/31','4W',3
--select @CompanyCode=N'6115204',@BranchCode=N'%',@PeriodFrom='2015-07-01',@PeriodTo='2015-07-24',@ProductType=N'4W', @table='3'

-- 0= Not Recalculate PPN, 1= Recalculate PPN (For FPJ Gabungan)
declare @IsRecalPPN as bit
set @IsRecalPPN=0

if @table = 1
begin
	if (select count(ParaValue) from gnMstLookUpDtl where CodeID='CPPN' and LookUpValue='STATUS') > 0
	set @IsRecalPPN= (select convert(bit,ParaValue) from gnMstLookUpDtl where CodeID='CPPN' and LookUpValue='STATUS')

		SELECT FK
			, KD_JENIS_TRANSAKSI
			, FG_PENGGANTI
			, substring(NOMOR_FAKTUR,4,13) NOMOR_FAKTUR
			, MASA_PAJAK
			, TAHUN_PAJAK
			, TANGGAL_FAKTUR
			,  NPWP
			, NAMA_LAWAN_TRANSAKSI NAMA
			, REPLACE(REPLACE(ALAMAT_LENGKAP, CHAR(13),''),CHAR(10),'') ALAMAT_LENGKAP
			, sum(JUMLAH_DPP) JUMLAH_DPP
			, CASE WHEN LEFT(FPJNO,2) = 'FP' THEN floor(sum(JUMLAH_DPP * 0.1)) else sum(a.JUMLAH_PPN) end JUMLAH_PPN
			, sum(JUMLAH_PPNBM) JUMLAH_PPNBM
			, ID_KETERANGAN_TAMBAHAN
			, FG_UANG_MUKA
			, UANG_MUKA_DPP
			, UANG_MUKA_PPN
			, UANG_MUKA_PPNBM
			, REFERENSI
			, CUSTOMERCODE
			, FPJNO
		FROM (
			SELECT
				'FK' FK
				, LEFT(TaxNo,2) KD_JENIS_TRANSAKSI
				, 0 FG_PENGGANTI
				, REPLACE(REPLACE(TaxNo, '.', ''), '-', '') NOMOR_FAKTUR
				, (case when len(PeriodMonth) = 1 then convert(varchar, PeriodMonth, 1) 
					else convert(varchar, PeriodMonth, 2) end) MASA_PAJAK
				, PeriodYear TAHUN_PAJAK
				, CONVERT(VARCHAR, TaxDate, 103) TANGGAL_FAKTUR
				, REPLACE(REPLACE(NPWP, '.', ''), '-', '') NPWP
				, CustomerName NAMA_LAWAN_TRANSAKSI
				, CASE WHEN LEFT(FPJNO,3) = 'FPJ' THEN 
				(SELECT Address1 + ' ' + Address2 FROM SPTRNSFPJINFO WHERE COMPANYCODE = gnTaxOut.COMPANYCODE AND BRANCHCODE = gnTaxOut.BRANCHCODE AND FPJNO =	gnTaxOut.FPJNO) ELSE  
				  CASE WHEN LEFT(FPJNO,3) = 'FPS' THEN 
				(SELECT Address1 + ' ' + Address2 FROM svTrnFakturPajak WHERE COMPANYCODE = gnTaxOut.COMPANYCODE AND BRANCHCODE = gnTaxOut.BRANCHCODE AND FPJNO = gnTaxOut.FPJNO) ELSE				(SELECT Address1 + ' ' + Address2 FROM GNMSTCUSTOMER WHERE COMPANYCODE = gnTaxOut.COMPANYCODE AND BRANCHCODE = gnTaxOut.BRANCHCODE 
				AND CUSTOMERCODE = gnTaxOut.CUSTOMERCODE)	
				  END END ALAMAT_LENGKAP
				--, ISNULL(DPPAmt, 0) JUMLAH_DPP
				, JUMLAH_DPP = (case when left(FPJNO,3) = 'FPJ'
										  then floor((select sum(isnull(NetSalesAmt,0)) from spTrnSInvoicedtl
										          where CompanyCode=gnTaxOut.CompanyCode
												    and BranchCode=gnTaxOut.BranchCode
											  	    and InvoiceNo=gnTaxOut.ReferenceNo))
									 when left(FPJNo,3) = 'FPS'
									      then floor((select isnull(sum((RetailPrice * SupplyQty)-(RetailPrice * SupplyQty * DiscPct /100.00)),0) from svTrnInvItem
										          where CompanyCode=gnTaxOut.CompanyCode
												    and BranchCode=gnTaxOut.BranchCode
												  and InvoiceNo=gnTaxOut.ReferenceNo) 
											   +(select isnull(sum((OperationCost * OperationHour)-(OperationCost * OperationHour * DiscPct /100.00)),0) from svTrnInvTask
										          where CompanyCode=gnTaxOut.CompanyCode
												    and BranchCode=gnTaxOut.BranchCode
												  and InvoiceNo=gnTaxOut.ReferenceNo))
									 else isnull(DPPAmt,0) end)
				--, ISNULL(PPNAmt, 0) JUMLAH_PPN
				, JUMLAH_PPN = (case when left(FPJNO,3) = 'FPJ'
										  then floor(floor((select sum(isnull(NetSalesAmt,0)) from spTrnSInvoicedtl
										          where CompanyCode=gnTaxOut.CompanyCode
												    and BranchCode=gnTaxOut.BranchCode
												  and InvoiceNo=gnTaxOut.ReferenceNo)))
									 when left(FPJNo,3) = 'FPS'
									      then floor((floor((select isnull(sum((RetailPrice * SupplyQty)-(RetailPrice * SupplyQty * DiscPct /100.00)),0) from svTrnInvItem
										          where CompanyCode=gnTaxOut.CompanyCode
												    and BranchCode=gnTaxOut.BranchCode
												    and InvoiceNo=gnTaxOut.ReferenceNo) 
											   +(select isnull(sum((OperationCost * OperationHour)-(OperationCost * OperationHour * DiscPct /100.00)),0) from svTrnInvTask
										          where CompanyCode=gnTaxOut.CompanyCode
												    and BranchCode=gnTaxOut.BranchCode
												  and InvoiceNo=gnTaxOut.ReferenceNo))))
									 else isnull(PPNAmt,0) end)
				, ISNULL(PPNBmAmt, 0) JUMLAH_PPNBM
				, '' ID_KETERANGAN_TAMBAHAN
				, 0 FG_UANG_MUKA
				, 0 UANG_MUKA_DPP
				, 0 UANG_MUKA_PPN
				, 0 UANG_MUKA_PPNBM
				, Case when (left(FPJNo,3) = 'FPS' or left(FPJNo,3) = 'FPJ' ) then 'Tanggal Jatuh Tempo : ' + convert(varchar(20),SubmissionDate,106) 
					else 'Tanggal Expire : '+ convert(varchar(20),SubmissionDate,106) 
					--+' Nomor Rangka : '+ (select ChassisCode from omTrSalesInvoiceVin where invoiceNo = ReferenceNo) + 
					--(select convert(varchar(50),ChassisNo) from omTrSalesInvoiceVin where invoiceNo = ReferenceNo) 
					--+' Nomor Mesin : '+ (select EngineCode from omTrSalesInvoiceVin where invoiceNo = ReferenceNo) + 
					--(select convert(varchar(50),EngineNo) from omTrSalesInvoiceVin where invoiceNo = ReferenceNo)
					end REFERENSI
				, CUSTOMERCODE
				, FPJNO
			FROM 
				gnTaxOut 
			WHERE
				CompanyCode = @CompanyCode
				AND BranchCode = @BranchCode
				AND ProductType = @ProductType
				--AND PeriodYear = @PeriodYear
				--AND PeriodMonth = @PeriodMonth
				AND CONVERT(VARCHAR, TaxDate, 112) BETWEEN @PeriodFrom AND @PeriodTo 
				AND IsPKP = 1
)A
group by FK, KD_JENIS_TRANSAKSI, FG_PENGGANTI
			, substring(NOMOR_FAKTUR,4,13)
			, MASA_PAJAK, TAHUN_PAJAK, tanggal_faktur, npwp, NAMA_LAWAN_TRANSAKSI, ALAMAT_LENGKAP--, JUMLAH_DPP, JUMLAH_PPNBM
			, ID_KETERANGAN_TAMBAHAN
			, FG_UANG_MUKA
			, UANG_MUKA_DPP
			, UANG_MUKA_PPN
			, UANG_MUKA_PPNBM
			, REFERENSI
			, CUSTOMERCODE
			, FPJNO
end
if @table = 2
begin
	select  'LT' LT, REPLACE(REPLACE(NPWPNo, '.', ''), '-', '') NPWP, CustomerName NAMA, REPLACE(REPLACE(Address1, CHAR(13),''),CHAR(10),'')  + REPLACE(REPLACE(Address2, CHAR(13),''),CHAR(10),'') JALAN, '-' BLOK, '-' NOMOR, '0' RT, '0' RW, 
	KecamatanDistrik KECAMATAN, KelurahanDesa KELURAHAN, KotaKabupaten KABUPATEN, 
	case when isnull(ProvinceCode,'') = '' then '-' else (isnull((select top 1 lookupvaluename from gnmstlookupdtl where codeid='PROV' and LookUpValue = ProvinceCode),'-' ))end PROPINSI, 
	ZipNo KODE_POS, PhoneNo NOMOR_TELEPON, CUSTOMERCODE
	from gnMstCustomer 
	where CustomerCode in 
	(select distinct CustomerCode from gnTaxOut WHERE
					CompanyCode = @CompanyCode
					AND BranchCode = @BranchCode
					AND ProductType = @ProductType
					--AND PeriodYear = @PeriodYear
					--AND PeriodMonth = @PeriodMonth
					AND CONVERT(VARCHAR, TaxDate, 112) BETWEEN @PeriodFrom AND @PeriodTo 
					AND IsPKP = 1)
end
if @table = 3
begin
		select * from (
						select 'OF' [OF]
						, PartNo KODE_OBJEK
						, (select replace(rtrim(Partname),',',' ') from spmstiteminfo where companycode = a.companycode and partno = a.partno) NAMA
						, isnull ((convert(decimal(12,2),a.retailprice)), 0.00) HARGA_SATUAN
						, isnull ((convert(decimal(12,2),a.QtyBill)), 0.00) JUMLAH_BARANG
						, isnull ((convert(decimal(12,2),a.SalesAmt)), 0.00)HARGA_TOTAL
						, isnull ((convert(decimal(12,2),a.DiscAmt)), 0.00) DISKON
						, isnull ((convert(decimal(12,2),a.NetSalesAmt)), 0.00) DPP
						, isnull ((convert(decimal(12,2),a.NetSalesAmt * 0.10)), 0.00) PPN
						, 0 TARIF_PPNBM
						, 0 PPNBM
						, b.CUSTOMERCODE 
						, b.FPJNO
						from spTrnSInvoicedtl a
						inner join (select companycode, branchcode, fpjno, customercode, referenceNo
									from gntaxout 
									 WHERE CompanyCode = @CompanyCode
											AND BranchCode = @BranchCode
											AND ProductType = @ProductType
											--AND PeriodYear = @PeriodYear
											--AND PeriodMonth = @PeriodMonth
											AND CONVERT(VARCHAR, TaxDate, 112) BETWEEN @PeriodFrom AND @PeriodTo 
											AND IsPKP = 1
											AND left(fpjno,3)='FPJ'
									) b
							on a.companycode = b.companycode and a.branchcode = b.branchcode
								and a.InvoiceNo = b.referenceNo
						union all
						select 'OF' [OF]
						, SalesModelCode KODE_OBJEK
						, (select SalesModelDesc from ommstmodel where companycode = a.companycode and salesmodelcode = a.salesmodelcode) NAMA
						, isnull ((convert(decimal(12,0),a.BeforeDiscDPP)), 0) HARGA_SATUAN
						, isnull ((convert(decimal(12,0),a.Quantity)), 0) JUMLAH_BARANG
						, isnull ((convert(decimal(12,0),a.BeforeDiscDPP * a.Quantity)),0) HARGA_TOTAL
						, isnull ((convert(decimal(12,0),a.DiscExcludePPn * a.Quantity)), 0) DISKON
						, isnull ((convert(decimal(12,0),a.AfterDiscDPP * a.Quantity)), 0) DPP
						, isnull ((convert(decimal(12,0),a.AfterDiscPPn * a.Quantity)), 0) PPN
						, 0 TARIF_PPNBM
						, 0 PPNBM
						, b.CUSTOMERCODE 
						, b.FPJNO
						from omTrSalesInvoicemodel a
						inner join (select companycode, branchcode, fpjno, customercode, referenceNo
									from gntaxout 
									 WHERE CompanyCode = @CompanyCode
											AND BranchCode = @BranchCode
											AND ProductType = @ProductType
											--AND PeriodYear = @PeriodYear
											--AND PeriodMonth = @PeriodMonth
											AND CONVERT(VARCHAR, TaxDate, 112) BETWEEN @PeriodFrom AND @PeriodTo 
											AND IsPKP = 1
											AND left(fpjno,3) not in ('FPJ','FPS')
									) b
							on a.companycode = b.companycode and a.branchcode = b.branchcode
								and a.InvoiceNo = b.referenceNo	
						union all
						select 'OF' [OF]
						, AccountNo KODE_OBJEK
						, Description NAMA
						, isnull ((convert(decimal(12,0),a.UnitPriceAmt)), 0) HARGA_SATUAN
						, isnull ((convert(decimal(12,0),a.Quantity)), 0) JUMLAH_BARANG
						, isnull ((convert(decimal(12,0),a.UnitPriceAmt * a.Quantity)),0) HARGA_TOTAL
						, isnull ((convert(decimal(12,0),a.DiscAmt)), 0) DISKON
						, isnull ((convert(decimal(12,0),a.UnitPriceAmt * a.Quantity)), 0) DPP
						, isnull ((convert(decimal(12,0),a.PPNAmt)), 0) PPN
						, 0 TARIF_PPNBM
						, 0 PPNBM
						, b.CUSTOMERCODE 
						, b.FPJNO
						from arTrnInvoiceDtl a
						inner join (select companycode, branchcode, fpjno, customercode, referenceNo
									from gntaxout 
									 WHERE CompanyCode = @CompanyCode
											AND BranchCode = @BranchCode
											AND ProductType = @ProductType
											--AND PeriodYear = @PeriodYear
											--AND PeriodMonth = @PeriodMonth
											AND CONVERT(VARCHAR, TaxDate, 112) BETWEEN @PeriodFrom AND @PeriodTo 
											AND IsPKP = 1
											AND left(fpjno,3) not in ('FPJ','FPS')
									) b
							on a.companycode = b.companycode and a.branchcode = b.branchcode
								and a.InvoiceNo = b.referenceNo	
						union all 
						select  
							[OF],
							KODE_OBJEK,
							NAMA,
							HARGA_SATUAN,
							JUMLAH_BARANG,
							HARGA_TOTAL,
							DISKON,
							DPP,
							convert(decimal(12,2),(dpp * 0.1)) PPN,
							TARIF_PPNBM,
							PPNBM,
							CUSTOMERCODE,
							FPJNO
						from(
						select 'OF' [OF]
						, rtrim(PartNo) KODE_OBJEK
						, (select replace(rtrim(Partname),',',' ') from spmstiteminfo where companycode = a.companycode and partno = a.partno) NAMA
						, isnull (convert(decimal(12,2),a.RetailPrice,0),0.00) HARGA_SATUAN
						, isnull (convert(decimal(12,2),a.SupplyQty,0),0.00) JUMLAH_BARANG
						, isnull (convert(decimal(12,2),a.RetailPrice * a.SupplyQty,0),0.00) HARGA_TOTAL
						, isnull (convert(decimal(12,2),a.RetailPrice * a.SupplyQty * DiscPct /100.00,0),0.00) DISKON
						, isnull (convert(decimal(12,2),(a.RetailPrice * a.SupplyQty)-(a.RetailPrice * a.SupplyQty * DiscPct /100.00),0),0.00) DPP
						--, isnull (convert(decimal(12,2),(((a.RetailPrice * a.SupplyQty)-(a.RetailPrice * a.SupplyQty * DiscPct /100.00))*0.10),0),0.00) PPN
						, 0 TARIF_PPNBM
						, 0 PPNBM
						, b.CUSTOMERCODE 
						, b.FPJNO
						from svTrnInvItem a
						inner join (select companycode, branchcode, fpjno, customercode, referenceNo
									from gntaxout 
									 WHERE CompanyCode = @CompanyCode
											AND BranchCode = @BranchCode
											AND ProductType = @ProductType
											--AND PeriodYear = @PeriodYear
											--AND PeriodMonth = @PeriodMonth
											AND CONVERT(VARCHAR, TaxDate, 112) BETWEEN @PeriodFrom AND @PeriodTo 
											AND IsPKP = 1
											AND left(fpjno,3)='FPS' 
									) b
							on a.companycode = b.companycode and a.branchcode = b.branchcode
								and a.InvoiceNo = b.referenceNo	
					)Z
-- tambahan untuk jasa service
						union all 
						select  
							[OF],
							KODE_OBJEK,
							NAMA,
							HARGA_SATUAN,
							JUMLAH_BARANG,
							HARGA_TOTAL,
							DISKON,
							DPP,
							convert(decimal(12,2),(dpp * 0.1)) PPN,
							TARIF_PPNBM,
							PPNBM,
							CUSTOMERCODE,
							FPJNO
						from(
						select 'OF' [OF]
						, rtrim(OperationNo) KODE_OBJEK
						, (select top 1 replace(rtrim(Description),',',' ') from svMstTask 
						    where CompanyCode=a.CompanyCode and BasicModel=x.BasicModel and OperationNo=a.OperationNo
						  ) NAMA
						, isnull (convert(decimal(12,2),a.OperationCost,0),0.00) HARGA_SATUAN
						, isnull (convert(decimal(12,2),a.OperationHour,0),0.00) JUMLAH_BARANG
						, isnull (convert(decimal(12,2),a.OperationCost * a.OperationHour,0),0.00) HARGA_TOTAL
						, isnull (convert(decimal(12,2),a.OperationCost * a.OperationHour * DiscPct /100.00,0),0.00) DISKON
						, isnull (convert(decimal(12,2),(a.OperationCost * a.OperationHour)-(a.OperationCost * a.OperationHour * DiscPct /100.00),0),0.00) DPP
						, isnull (convert(decimal(12,2),(((a.OperationCost * a.OperationHour)-(a.OperationCost * a.OperationHour * DiscPct /100.00))*0.10),0),0.00) PPN
						, 0 TARIF_PPNBM
						, 0 PPNBM
						, b.CUSTOMERCODE 
						, b.FPJNO
						from svTrnInvTask a
						inner join svTrnInvoice x
						    on a.CompanyCode=x.CompanyCode and a.BranchCode=x.BranchCode and a.InvoiceNo=x.InvoiceNo
						inner join (select companycode, branchcode, fpjno, customercode, referenceNo
									from gntaxout 
									 WHERE CompanyCode = @CompanyCode
											AND BranchCode = @BranchCode
											AND ProductType = @ProductType
											AND CONVERT(VARCHAR, TaxDate, 112) BETWEEN @PeriodFrom AND @PeriodTo 
											AND IsPKP = 1
											AND left(fpjno,3)='FPS' 
									) b
							on a.companycode = b.companycode and a.branchcode = b.branchcode
								and a.InvoiceNo = b.referenceNo	
					)Y
				) a
	 order by FPJNO
end

GO
ALTER procedure [dbo].[usprpt_GnGenerateCsvSkemaTaxOut]
--DECLARE
	@CompanyCode varchar(15)
	,@BranchCode varchar(15)
	,@PeriodFrom smalldatetime		
	,@PeriodTo smalldatetime
	,@ProductType varchar(2)
	,@table varchar(1)=1
as

--execute usprpt_GnGenerateCsvSkemaTaxOut '6115204','611520401','2015-07-01','2015-07-28','2W',3
--select @CompanyCode=N'6058401',@BranchCode=N'605840100',@PeriodFrom='2015-07-01',@PeriodTo='2015-07-28',@ProductType=N'4W', @table='3'
--select @CompanyCode=N'6115204',@BranchCode=N'611520401'


-- 0= Not Recalculate PPN, 1= Recalculate PPN (For FPJ Gabungan)
declare @IsRecalPPN as bit
set @IsRecalPPN=0

if @table = 1
begin
	if (select count(ParaValue) from gnMstLookUpDtl where CodeID='CPPN' and LookUpValue='STATUS') > 0
	set @IsRecalPPN= (select convert(bit,ParaValue) from gnMstLookUpDtl where CodeID='CPPN' and LookUpValue='STATUS')

SELECT FK
		, KD_JENIS_TRANSAKSI
		, FG_PENGGANTI
		, NOMOR_FAKTUR
		, MASA_PAJAK
		, TAHUN_PAJAK
		, TANGGAL_FAKTUR
		, NPWP
		, NAMA
		, ALAMAT_LENGKAP
		, SUM(JUMLAH_DPP) JUMLAH_DPP
		, SUM(JUMLAH_PPN) JUMLAH_PPN
		, SUM(JUMLAH_PPNBM) JUMLAH_PPNBM
		, ID_KETERANGAN_TAMBAHAN
		, FG_UANG_MUKA
		, UANG_MUKA_DPP
		, UANG_MUKA_PPN
		, UANG_MUKA_PPNBM
		, REFERENSI
		, CUSTOMERCODE
		FROM(
		SELECT FK
			, KD_JENIS_TRANSAKSI
			, FG_PENGGANTI
			, substring(NOMOR_FAKTUR,4,13) NOMOR_FAKTUR
			, MASA_PAJAK
			, TAHUN_PAJAK
			, TANGGAL_FAKTUR
			, NPWP
			, NAMA_LAWAN_TRANSAKSI NAMA
			, REPLACE(REPLACE(ALAMAT_LENGKAP, CHAR(13),''),CHAR(10),'') ALAMAT_LENGKAP
			, sum(JUMLAH_DPP) JUMLAH_DPP
			, CASE WHEN LEFT(FPJNO,2) = 'FP' THEN floor(sum(JUMLAH_DPP * 0.1)) else sum(a.JUMLAH_PPN) end JUMLAH_PPN
			, sum(JUMLAH_PPNBM) JUMLAH_PPNBM
			, ID_KETERANGAN_TAMBAHAN
			, FG_UANG_MUKA
			, UANG_MUKA_DPP
			, UANG_MUKA_PPN
			, UANG_MUKA_PPNBM
			, REFERENSI
			, CUSTOMERCODE
			, FPJNO
		FROM (
			SELECT
				'FK' FK
				, LEFT(TaxNo,2) KD_JENIS_TRANSAKSI
				, 0 FG_PENGGANTI
				, REPLACE(REPLACE(TaxNo, '.', ''), '-', '') NOMOR_FAKTUR
				, (case when len(PeriodMonth) = 1 then convert(varchar, PeriodMonth, 1) 
					else convert(varchar, PeriodMonth, 2) end) MASA_PAJAK
				, PeriodYear TAHUN_PAJAK
				, CONVERT(VARCHAR, TaxDate, 103) TANGGAL_FAKTUR
				, REPLACE(REPLACE(NPWP, '.', ''), '-', '') NPWP
				, CustomerName NAMA_LAWAN_TRANSAKSI
				, CASE WHEN LEFT(FPJNO,3) = 'FPJ' THEN 
				(SELECT Address1 + ' ' + Address2 FROM SPTRNSFPJINFO WHERE COMPANYCODE = gnTaxOut.COMPANYCODE AND BRANCHCODE = gnTaxOut.BRANCHCODE AND FPJNO =	gnTaxOut.FPJNO) ELSE  
				  CASE WHEN LEFT(FPJNO,3) = 'FPS' THEN 
				(SELECT Address1 + ' ' + Address2 FROM svTrnFakturPajak WHERE COMPANYCODE = gnTaxOut.COMPANYCODE AND BRANCHCODE = gnTaxOut.BRANCHCODE AND FPJNO = gnTaxOut.FPJNO) ELSE				(SELECT Address1 + ' ' + Address2 FROM GNMSTCUSTOMER WHERE COMPANYCODE = gnTaxOut.COMPANYCODE AND BRANCHCODE = gnTaxOut.BRANCHCODE 
				AND CUSTOMERCODE = gnTaxOut.CUSTOMERCODE)	
				  END END ALAMAT_LENGKAP
				, JUMLAH_DPP = (case when left(FPJNO,3) = 'FPJ'
										  then floor((select sum(isnull(NetSalesAmt,0)) from spTrnSInvoicedtl
										          where CompanyCode=gnTaxOut.CompanyCode
												    and BranchCode=gnTaxOut.BranchCode
											  	    and InvoiceNo=gnTaxOut.ReferenceNo))
									 when left(FPJNo,3) = 'FPS'
									      then floor((select isnull(sum((RetailPrice * SupplyQty)-(RetailPrice * SupplyQty * DiscPct /100.00)),0) from svTrnInvItem
										          where CompanyCode=gnTaxOut.CompanyCode
												    and BranchCode=gnTaxOut.BranchCode
												  and InvoiceNo=gnTaxOut.ReferenceNo) 
											   +(select isnull(sum((OperationCost * OperationHour)-(OperationCost * OperationHour * DiscPct /100.00)),0) from svTrnInvTask
										          where CompanyCode=gnTaxOut.CompanyCode
												    and BranchCode=gnTaxOut.BranchCode
												  and InvoiceNo=gnTaxOut.ReferenceNo))
									 else isnull(DPPAmt,0) end)
				, JUMLAH_PPN = (case when left(FPJNO,3) = 'FPJ'
										  then floor(floor((select sum(isnull(NetSalesAmt,0)) from spTrnSInvoicedtl
										          where CompanyCode=gnTaxOut.CompanyCode
												    and BranchCode=gnTaxOut.BranchCode
												  and InvoiceNo=gnTaxOut.ReferenceNo)))
									 when left(FPJNo,3) = 'FPS'
									      then floor((floor((select isnull(sum((RetailPrice * SupplyQty)-(RetailPrice * SupplyQty * DiscPct /100.00)),0) from svTrnInvItem
										          where CompanyCode=gnTaxOut.CompanyCode
												    and BranchCode=gnTaxOut.BranchCode
												    and InvoiceNo=gnTaxOut.ReferenceNo) 
											   +(select isnull(sum((OperationCost * OperationHour)-(OperationCost * OperationHour * DiscPct /100.00)),0) from svTrnInvTask
										          where CompanyCode=gnTaxOut.CompanyCode
												    and BranchCode=gnTaxOut.BranchCode
												  and InvoiceNo=gnTaxOut.ReferenceNo))))
									 else isnull(PPNAmt,0) end)
				, ISNULL(PPNBmAmt, 0) JUMLAH_PPNBM
				, '' ID_KETERANGAN_TAMBAHAN
				, 0 FG_UANG_MUKA
				, 0 UANG_MUKA_DPP
				, 0 UANG_MUKA_PPN
				, 0 UANG_MUKA_PPNBM
				--, ReferenceNo REFERENSI
				, Case when (left(FPJNo,3) = 'FPS' or left(FPJNo,3) = 'FPJ' ) then 'No Invoice : ' + ReferenceNo + ' Tanggal Jatuh Tempo : ' + convert(varchar(20),SubmissionDate,106)
				       when left(FPJNo,3) = 'FPH' then 'No FPJ : ' + FPJNo
				       else 'No Invoice : ' + ReferenceNo + ' Tanggal Jatuh Tempo : ' + convert(varchar(20),SubmissionDate,106)
				  end REFERENSI     
					--'Tanggal Jatuh Tempo : ' + convert(varchar(20),SubmissionDate,106) 
					--else 'Tanggal Expire : '+ convert(varchar(20),SubmissionDate,106) 
					--+' Nomor Rangka : '+ (select ChassisCode from omTrSalesInvoiceVin where invoiceNo = ReferenceNo) + 
					--(select convert(varchar(50),ChassisNo) from omTrSalesInvoiceVin where invoiceNo = ReferenceNo) 
					--+' Nomor Mesin : '+ (select EngineCode from omTrSalesInvoiceVin where invoiceNo = ReferenceNo) + 
					--(select convert(varchar(50),EngineNo) from omTrSalesInvoiceVin where invoiceNo = ReferenceNo)
					--end REFERENSI
				, CUSTOMERCODE
				, FPJNO
			FROM 
				gnTaxOut 
			WHERE
				CompanyCode = @CompanyCode
				AND BranchCode = @BranchCode
				AND ProductType = @ProductType
				--AND PeriodYear = @PeriodYear
				--AND PeriodMonth = @PeriodMonth
				AND CONVERT(VARCHAR, TaxDate, 112) BETWEEN @PeriodFrom AND @PeriodTo 
				AND IsPKP = 1
				AND DocumentType = 'F'
				)A
				group by FK, KD_JENIS_TRANSAKSI, FG_PENGGANTI
							, substring(NOMOR_FAKTUR,4,13)
							, MASA_PAJAK, TAHUN_PAJAK, tanggal_faktur, npwp, NAMA_LAWAN_TRANSAKSI, ALAMAT_LENGKAP--, JUMLAH_DPP, JUMLAH_PPNBM
							, ID_KETERANGAN_TAMBAHAN
							, FG_UANG_MUKA
							, UANG_MUKA_DPP
							, UANG_MUKA_PPN
							, UANG_MUKA_PPNBM
							, REFERENSI
							, CUSTOMERCODE
							, FPJNO
			)B
GROUP BY FK
		, KD_JENIS_TRANSAKSI
		, FG_PENGGANTI
		, NOMOR_FAKTUR
		, MASA_PAJAK
		, TAHUN_PAJAK
		, TANGGAL_FAKTUR
		, NPWP
		, NAMA
		, ALAMAT_LENGKAP
		, ID_KETERANGAN_TAMBAHAN
		, FG_UANG_MUKA
		, UANG_MUKA_DPP
		, UANG_MUKA_PPN
		, UANG_MUKA_PPNBM
		, REFERENSI
		, CUSTOMERCODE

end
if @table = 2
begin
	select  'LT' LT, REPLACE(REPLACE(NPWPNo, '.', ''), '-', '') NPWP, CustomerName NAMA, REPLACE(REPLACE(Address1, CHAR(13),''),CHAR(10),'')  + REPLACE(REPLACE(Address2, CHAR(13),''),CHAR(10),'') JALAN, '-' BLOK, '-' NOMOR, '0' RT, '0' RW, 
	KecamatanDistrik KECAMATAN, KelurahanDesa KELURAHAN, KotaKabupaten KABUPATEN, 
	case when isnull(ProvinceCode,'') = '' then '-' else (isnull((select top 1 lookupvaluename from gnmstlookupdtl where codeid='PROV' and LookUpValue = ProvinceCode),'-' ))end PROPINSI, 
	ZipNo KODE_POS, PhoneNo NOMOR_TELEPON, CUSTOMERCODE
	from gnMstCustomer 
	where CustomerCode in 
	(select distinct CustomerCode from gnTaxOut WHERE
					CompanyCode = @CompanyCode
					AND BranchCode = @BranchCode
					AND ProductType = @ProductType
					--AND PeriodYear = @PeriodYear
					--AND PeriodMonth = @PeriodMonth
					AND CONVERT(VARCHAR, TaxDate, 112) BETWEEN @PeriodFrom AND @PeriodTo 
					AND IsPKP = 1)
end
if @table = 3
begin
		select * from (
						select 'OF' [OF]
						, PartNo KODE_OBJEK
						, (select replace(rtrim(Partname),',',' ') from spmstiteminfo where companycode = a.companycode and partno = a.partno) NAMA
						, isnull ((convert(decimal(12,2),a.retailprice)), 0.00) HARGA_SATUAN
						, isnull ((convert(decimal(12,2),a.QtyBill)), 0.00) JUMLAH_BARANG
						, isnull ((convert(decimal(12,2),a.SalesAmt)), 0.00)HARGA_TOTAL
						, isnull ((convert(decimal(12,2),a.DiscAmt)), 0.00) DISKON
						, isnull ((convert(decimal(12,2),a.NetSalesAmt)), 0.00) DPP
						, isnull ((convert(decimal(12,2),a.NetSalesAmt * 0.10)), 0.00) PPN
						, 0 TARIF_PPNBM
						, 0 PPNBM
						, b.CUSTOMERCODE 
						, substring(REPLACE(REPLACE(b.TaxNo, '.', ''), '-', ''),4,13) NOMOR_FAKTUR
						, b.FPJNo
						from spTrnSInvoicedtl a
						inner join (select companycode, branchcode, fpjno, customercode, referenceNo, TaxNo
									from gntaxout 
									 WHERE CompanyCode = @CompanyCode
											AND BranchCode = @BranchCode
											AND ProductType = @ProductType
											--AND PeriodYear = @PeriodYear
											--AND PeriodMonth = @PeriodMonth
											AND CONVERT(VARCHAR, TaxDate, 112) BETWEEN @PeriodFrom AND @PeriodTo 
											AND IsPKP = 1
											AND left(fpjno,3)='FPJ'
									) b
							on a.companycode = b.companycode and a.branchcode = b.branchcode
								and a.InvoiceNo = b.referenceNo
						union all
						select 'OF' [OF]
						, SalesModelCode KODE_OBJEK
						, (select SalesModelDesc from ommstmodel where companycode = a.companycode and salesmodelcode = a.salesmodelcode) NAMA
						--, 'Sales Model Desc : '+ (select SalesModelDesc from ommstmodel where companycode = a.companycode and salesmodelcode = a.salesmodelcode) + 
						--  +' Nomor Rangka : '+ (select CONVERT(varchar, ChassisNo, 100) from omTrSalesInvoiceVin where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and 
						--						InvoiceNo = a.InvoiceNo and BPKNo = a.BPKNo and SalesModelCode = a.SalesModelCode and SalesModelYear = a.SalesModelYear)
						--  +' Nomor Mesin : '+ (select CONVERT(varchar, EngineNo, 100) from omTrSalesInvoiceVin where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and 
						--						InvoiceNo = a.InvoiceNo and BPKNo = a.BPKNo and SalesModelCode = a.SalesModelCode and SalesModelYear = a.SalesModelYear) NAMA
						, isnull ((convert(decimal(12,0),a.BeforeDiscDPP)), 0) HARGA_SATUAN
						, isnull ((convert(decimal(12,0),a.Quantity)), 0) JUMLAH_BARANG
						, isnull ((convert(decimal(12,0),a.BeforeDiscDPP * a.Quantity)),0) HARGA_TOTAL
						, isnull ((convert(decimal(12,0),a.DiscExcludePPn * a.Quantity)), 0) DISKON
						, isnull ((convert(decimal(12,0),a.AfterDiscDPP * a.Quantity)), 0) DPP
						, isnull ((convert(decimal(12,0),a.AfterDiscPPn * a.Quantity)), 0) PPN
						, 0 TARIF_PPNBM
						, 0 PPNBM
						, b.CUSTOMERCODE 
						, substring(REPLACE(REPLACE(b.TaxNo, '.', ''), '-', ''),4,13) TaxNo
						, b.FPJNo
						from omTrSalesInvoicemodel a
						inner join (select companycode, branchcode, fpjno, customercode, referenceNo, TaxNo
									from gntaxout 
									 WHERE CompanyCode = @CompanyCode
											AND BranchCode = @BranchCode
											AND ProductType = @ProductType
											--AND PeriodYear = @PeriodYear
											--AND PeriodMonth = @PeriodMonth
											AND CONVERT(VARCHAR, TaxDate, 112) BETWEEN @PeriodFrom AND @PeriodTo 
											AND IsPKP = 1
											AND left(fpjno,3) not in ('FPJ','FPS')
									) b
							on a.companycode = b.companycode and a.branchcode = b.branchcode
								and a.InvoiceNo = b.referenceNo	
						union all
						select 'OF' [OF]
						, AccountNo KODE_OBJEK
						, Description NAMA
						, isnull ((convert(decimal(12,0),a.UnitPriceAmt)), 0) HARGA_SATUAN
						, isnull ((convert(decimal(12,0),a.Quantity)), 0) JUMLAH_BARANG
						, isnull ((convert(decimal(12,0),a.UnitPriceAmt * a.Quantity)),0) HARGA_TOTAL
						, isnull ((convert(decimal(12,0),a.DiscAmt)), 0) DISKON
						, isnull ((convert(decimal(12,0),a.UnitPriceAmt * a.Quantity)), 0) DPP
						, isnull ((convert(decimal(12,0),a.PPNAmt)), 0) PPN
						, 0 TARIF_PPNBM
						, 0 PPNBM
						, b.CUSTOMERCODE 
						, substring(REPLACE(REPLACE(b.TaxNo, '.', ''), '-', ''),4,13) NOMOR_FAKTUR
						, b.FPJNo
						from arTrnInvoiceDtl a
						inner join (select companycode, branchcode, fpjno, customercode, referenceNo, TaxNo
									from gntaxout 
									 WHERE CompanyCode = @CompanyCode
											AND BranchCode = @BranchCode
											AND ProductType = @ProductType
											--AND PeriodYear = @PeriodYear
											--AND PeriodMonth = @PeriodMonth
											AND CONVERT(VARCHAR, TaxDate, 112) BETWEEN @PeriodFrom AND @PeriodTo 
											AND IsPKP = 1
											AND left(fpjno,3) not in ('FPJ','FPS')
									) b
							on a.companycode = b.companycode and a.branchcode = b.branchcode
								and a.InvoiceNo = b.referenceNo	
						union all 
						select  
							[OF],
							KODE_OBJEK,
							NAMA,
							HARGA_SATUAN,
							JUMLAH_BARANG,
							HARGA_TOTAL,
							DISKON,
							DPP,
							convert(decimal(12,2),(dpp * 0.1)) PPN,
							TARIF_PPNBM,
							PPNBM,
							CUSTOMERCODE,
							NOMOR_FAKTUR,
							FPJNO
						from(
						select 'OF' [OF]
						, rtrim(PartNo) KODE_OBJEK
						, (select replace(rtrim(Partname),',',' ') from spmstiteminfo where companycode = a.companycode and partno = a.partno) NAMA
						, isnull (convert(decimal(12,2),a.RetailPrice,0),0.00) HARGA_SATUAN
						, isnull (convert(decimal(12,2),a.SupplyQty,0),0.00) JUMLAH_BARANG
						, isnull (convert(decimal(12,2),a.RetailPrice * a.SupplyQty,0),0.00) HARGA_TOTAL
						, isnull (convert(decimal(12,2),a.RetailPrice * a.SupplyQty * DiscPct /100.00,0),0.00) DISKON
						, isnull (convert(decimal(12,2),(a.RetailPrice * a.SupplyQty)-(a.RetailPrice * a.SupplyQty * DiscPct /100.00),0),0.00) DPP
						--, isnull (convert(decimal(12,2),(((a.RetailPrice * a.SupplyQty)-(a.RetailPrice * a.SupplyQty * DiscPct /100.00))*0.10),0),0.00) PPN
						, 0 TARIF_PPNBM
						, 0 PPNBM
						, b.CUSTOMERCODE 
						, substring(REPLACE(REPLACE(b.TaxNo, '.', ''), '-', ''),4,13) NOMOR_FAKTUR
						, b.FPJNo
						from svTrnInvItem a
						inner join (select companycode, branchcode, fpjno, customercode, referenceNo, TaxNo
									from gntaxout 
									 WHERE CompanyCode = @CompanyCode
											AND BranchCode = @BranchCode
											AND ProductType = @ProductType
											--AND PeriodYear = @PeriodYear
											--AND PeriodMonth = @PeriodMonth
											AND CONVERT(VARCHAR, TaxDate, 112) BETWEEN @PeriodFrom AND @PeriodTo 
											AND IsPKP = 1
											AND (left(fpjno,3)='FPS' OR LEFT(FPJNo,3)='FPH')
									) b
							on a.companycode = b.companycode and a.branchcode = b.branchcode
								and a.InvoiceNo = b.referenceNo	
					)Z
-- tambahan untuk jasa service
						union all 
						select  
							[OF],
							KODE_OBJEK,
							NAMA,
							HARGA_SATUAN,
							JUMLAH_BARANG,
							HARGA_TOTAL,
							DISKON,
							DPP,
							convert(decimal(12,2),(dpp * 0.1)) PPN,
							TARIF_PPNBM,
							PPNBM,
							CUSTOMERCODE,
							NOMOR_FAKTUR,
							FPJNO
						from(
						select 'OF' [OF]
						, rtrim(OperationNo) KODE_OBJEK
						, (select top 1 replace(rtrim(Description),',',' ') from svMstTask 
						    where CompanyCode=a.CompanyCode and BasicModel=x.BasicModel and OperationNo=a.OperationNo
						  ) NAMA
						, isnull (convert(decimal(12,2),a.OperationCost,0),0.00) HARGA_SATUAN
						, isnull (convert(decimal(12,2),a.OperationHour,0),0.00) JUMLAH_BARANG
						, isnull (convert(decimal(12,2),a.OperationCost * a.OperationHour,0),0.00) HARGA_TOTAL
						, isnull (convert(decimal(12,2),a.OperationCost * a.OperationHour * DiscPct /100.00,0),0.00) DISKON
						, isnull (convert(decimal(12,2),(a.OperationCost * a.OperationHour)-(a.OperationCost * a.OperationHour * DiscPct /100.00),0),0.00) DPP
						, isnull (convert(decimal(12,2),(((a.OperationCost * a.OperationHour)-(a.OperationCost * a.OperationHour * DiscPct /100.00))*0.10),0),0.00) PPN
						, 0 TARIF_PPNBM
						, 0 PPNBM
						, b.CUSTOMERCODE 
						, substring(REPLACE(REPLACE(b.TaxNo, '.', ''), '-', ''),4,13) NOMOR_FAKTUR
						, b.FPJNo
						from svTrnInvTask a
						inner join svTrnInvoice x
						    on a.CompanyCode=x.CompanyCode and a.BranchCode=x.BranchCode and a.InvoiceNo=x.InvoiceNo
						inner join (select companycode, branchcode, fpjno, customercode, referenceNo, TaxNo
									from gntaxout 
									 WHERE CompanyCode = @CompanyCode
											AND BranchCode = @BranchCode
											AND ProductType = @ProductType
											AND CONVERT(VARCHAR, TaxDate, 112) BETWEEN @PeriodFrom AND @PeriodTo 
											AND IsPKP = 1
											AND (left(fpjno,3)='FPS' OR LEFT(FPJNo,3)='FPH')
									) b
							on a.companycode = b.companycode and a.branchcode = b.branchcode
								and a.InvoiceNo = b.referenceNo	
					)Y
				) a
	 order by NOMOR_FAKTUR
end

GO

ALTER procedure [dbo].[usprpt_GnGenerateCsvSkemaTaxOut]
--DECLARE
	@CompanyCode varchar(15)
	,@BranchCode varchar(15)
	,@PeriodFrom smalldatetime		
	,@PeriodTo smalldatetime
	,@ProductType varchar(2)
	,@table varchar(1)=1
as

--execute usprpt_GnGenerateCsvSkemaTaxOut '6115204','611520401','2015-07-01','2015-07-28','2W',3
--select @CompanyCode=N'6058401',@BranchCode=N'605840100',@PeriodFrom='2015-07-01',@PeriodTo='2015-07-30',@ProductType=N'2W', @table='1'

 --0= Not Recalculate PPN, 1= Recalculate PPN (For FPJ Gabungan)
declare @IsRecalPPN as bit
set @IsRecalPPN=0

if @table = 1
begin
	if (select count(ParaValue) from gnMstLookUpDtl where CodeID='CPPN' and LookUpValue='STATUS') > 0
	set @IsRecalPPN= (select convert(bit,ParaValue) from gnMstLookUpDtl where CodeID='CPPN' and LookUpValue='STATUS')

SELECT FK
		, KD_JENIS_TRANSAKSI
		, FG_PENGGANTI
		, NOMOR_FAKTUR
		, MASA_PAJAK
		, TAHUN_PAJAK
		, TANGGAL_FAKTUR
		, NPWP
		, NAMA
		, ALAMAT_LENGKAP
		, SUM(JUMLAH_DPP) JUMLAH_DPP
		, CASE WHEN ProfitCenter != '300' THEN sum(JUMLAH_PPN)  else floor(sum(JUMLAH_DPP * 0.1)) end JUMLAH_PPN
		--, floor(sum(JUMLAH_DPP * 0.1)) JUMLAH_PPN
		--, sum(JUMLAH_PPN) JUMLAH_PPN
		, SUM(JUMLAH_PPNBM) JUMLAH_PPNBM
		, ID_KETERANGAN_TAMBAHAN
		, FG_UANG_MUKA
		, UANG_MUKA_DPP
		, UANG_MUKA_PPN
		, UANG_MUKA_PPNBM
		, REFERENSI
		, CUSTOMERCODE
		FROM(
		SELECT FK
			, CompanyCode
			, BranchCode
			, KD_JENIS_TRANSAKSI
			, FG_PENGGANTI
			, substring(NOMOR_FAKTUR,4,13) NOMOR_FAKTUR
			, MASA_PAJAK
			, TAHUN_PAJAK
			, TANGGAL_FAKTUR
			, NPWP
			, NAMA_LAWAN_TRANSAKSI NAMA
			, REPLACE(REPLACE(ALAMAT_LENGKAP, CHAR(13),''),CHAR(10),'') ALAMAT_LENGKAP
			, sum(JUMLAH_DPP) JUMLAH_DPP
			, CASE WHEN LEFT(FPJNO,2) = 'FP' THEN floor(sum(JUMLAH_DPP * 0.1)) else sum(a.JUMLAH_PPN) end JUMLAH_PPN
			, sum(JUMLAH_PPNBM) JUMLAH_PPNBM
			, ID_KETERANGAN_TAMBAHAN
			, FG_UANG_MUKA
			, UANG_MUKA_DPP
			, UANG_MUKA_PPN
			, UANG_MUKA_PPNBM
			, REFERENSI
			, CUSTOMERCODE
			, FPJNO
			, ProfitCenter
		FROM (
			SELECT
				'FK' FK
				, CompanyCode
				, BranchCode
				, LEFT(TaxNo,2) KD_JENIS_TRANSAKSI
				, 0 FG_PENGGANTI
				, REPLACE(REPLACE(TaxNo, '.', ''), '-', '') NOMOR_FAKTUR
				, (case when len(PeriodMonth) = 1 then convert(varchar, PeriodMonth, 1) 
					else convert(varchar, PeriodMonth, 2) end) MASA_PAJAK
				, PeriodYear TAHUN_PAJAK
				, CONVERT(VARCHAR, TaxDate, 103) TANGGAL_FAKTUR
				, REPLACE(REPLACE(NPWP, '.', ''), '-', '') NPWP
				, CustomerName NAMA_LAWAN_TRANSAKSI
				, CASE WHEN LEFT(FPJNO,3) = 'FPJ' THEN 
				(SELECT Address1 + ' ' + Address2 FROM SPTRNSFPJINFO WHERE COMPANYCODE = gnTaxOut.COMPANYCODE AND BRANCHCODE = gnTaxOut.BRANCHCODE AND FPJNO =	gnTaxOut.FPJNO) ELSE  
				  CASE WHEN LEFT(FPJNO,3) = 'FPS' THEN 
				(SELECT Address1 + ' ' + Address2 FROM svTrnFakturPajak WHERE COMPANYCODE = gnTaxOut.COMPANYCODE AND BRANCHCODE = gnTaxOut.BRANCHCODE AND FPJNO = gnTaxOut.FPJNO) ELSE				(SELECT Address1 + ' ' + Address2 FROM GNMSTCUSTOMER WHERE COMPANYCODE = gnTaxOut.COMPANYCODE AND BRANCHCODE = gnTaxOut.BRANCHCODE 
				AND CUSTOMERCODE = gnTaxOut.CUSTOMERCODE)	
				  END END ALAMAT_LENGKAP
				, JUMLAH_DPP = (case when left(FPJNO,3) = 'FPJ'
										  then floor((select sum(isnull(NetSalesAmt,0)) from spTrnSInvoicedtl
										          where CompanyCode=gnTaxOut.CompanyCode
												    and BranchCode=gnTaxOut.BranchCode
											  	    and InvoiceNo=gnTaxOut.ReferenceNo))
									 when left(FPJNo,3) = 'FPS'
									      then floor((select isnull(sum((RetailPrice * SupplyQty)-(RetailPrice * SupplyQty * DiscPct /100.00)),0) from svTrnInvItem
										          where CompanyCode=gnTaxOut.CompanyCode
												    and BranchCode=gnTaxOut.BranchCode
												  and InvoiceNo=gnTaxOut.ReferenceNo) 
											   +(select isnull(sum((OperationCost * OperationHour)-(OperationCost * OperationHour * DiscPct /100.00)),0) from svTrnInvTask
										          where CompanyCode=gnTaxOut.CompanyCode
												    and BranchCode=gnTaxOut.BranchCode
												  and InvoiceNo=gnTaxOut.ReferenceNo))
									 else DPPAmt end)
				, JUMLAH_PPN = (case when left(FPJNO,3) = 'FPJ'
										  then floor(floor((select sum(isnull(NetSalesAmt,0)) from spTrnSInvoicedtl
										          where CompanyCode=gnTaxOut.CompanyCode
												    and BranchCode=gnTaxOut.BranchCode
												  and InvoiceNo=gnTaxOut.ReferenceNo)))
									 when left(FPJNo,3) = 'FPS'
									      then floor((floor((select isnull(sum((RetailPrice * SupplyQty)-(RetailPrice * SupplyQty * DiscPct /100.00)),0) from svTrnInvItem
										          where CompanyCode=gnTaxOut.CompanyCode
												    and BranchCode=gnTaxOut.BranchCode
												    and InvoiceNo=gnTaxOut.ReferenceNo) 
											   +(select isnull(sum((OperationCost * OperationHour)-(OperationCost * OperationHour * DiscPct /100.00)),0) from svTrnInvTask
										          where CompanyCode=gnTaxOut.CompanyCode
												    and BranchCode=gnTaxOut.BranchCode
												  and InvoiceNo=gnTaxOut.ReferenceNo))))
									 else PPNAmt end)
				, PPNBmAmt JUMLAH_PPNBM --, (select sum(PPNBmAmt) from gnTaxOut a where a.CompanyCode = CompanyCode and a.BranchCode = BranchCode and a.TaxNo = TaxNo) JUMLAH_PPNBM
				, '' ID_KETERANGAN_TAMBAHAN
				, 0 FG_UANG_MUKA
				, 0 UANG_MUKA_DPP
				, 0 UANG_MUKA_PPN
				, 0 UANG_MUKA_PPNBM
				, Case when left(FPJNo,3) = 'FPS'
							then (case when (SELECT COUNT(a.TaxNo) FROM gnTaxOut a WHERE a.CompanyCode = gnTaxOut.CompanyCode AND a.BranchCode = gnTaxOut.BranchCode AND a.TaxNo = gnTaxOut.TaxNo) > 1  then 'Tanggal Jatuh Tempo : ' + convert(varchar(20),SubmissionDate,106) else 'No Invoice : ' + ReferenceNo + ' Tanggal Jatuh Tempo : ' + convert(varchar(20),SubmissionDate,106) end)
				       when left(FPJNo,3) = 'FPJ' 
							then (case when (SELECT COUNT(a.TaxNo) FROM gnTaxOut a WHERE a.CompanyCode = gnTaxOut.CompanyCode AND a.BranchCode = gnTaxOut.BranchCode AND a.TaxNo = gnTaxOut.TaxNo) > 1  then 'Tanggal Jatuh Tempo : ' + convert(varchar(20),SubmissionDate,106) else 'No Invoice : ' + ReferenceNo + ' (' + FPJNo + ')' + ' Tanggal Jatuh Tempo : ' + convert(varchar(20),SubmissionDate,106) end) -- Penambahan
				       when left(FPJNo,3) = 'FPH' then 'No FPJ : ' + FPJNo
				       else (case when (SELECT COUNT(a.TaxNo) FROM gnTaxOut a WHERE a.CompanyCode = gnTaxOut.CompanyCode AND a.BranchCode = gnTaxOut.BranchCode AND a.TaxNo = gnTaxOut.TaxNo) > 1 then 'Tanggal Jatuh Tempo : ' + convert(varchar(20),SubmissionDate,106) else 'No Invoice : ' + ReferenceNo + ' Tanggal Jatuh Tempo : ' + convert(varchar(20),SubmissionDate,106) end)
				  end + ' - Cabang ' + SUBSTRING(BranchCode,len(branchcode)-1,2) REFERENSI   
				, CUSTOMERCODE
				, FPJNO
				, ProfitCenter
			FROM 
				gnTaxOut 
			WHERE
				CompanyCode = @CompanyCode
				AND BranchCode = @BranchCode
				AND CONVERT(VARCHAR, TaxDate, 112) BETWEEN @PeriodFrom AND @PeriodTo 
				AND IsPKP = 1
				AND DocumentType = 'F'
				)A
				group by FK, CompanyCode
							, BranchCode,KD_JENIS_TRANSAKSI, FG_PENGGANTI
							, substring(NOMOR_FAKTUR,4,13)
							, MASA_PAJAK, TAHUN_PAJAK, tanggal_faktur, npwp, NAMA_LAWAN_TRANSAKSI, ALAMAT_LENGKAP--, JUMLAH_DPP, JUMLAH_PPNBM
							, ID_KETERANGAN_TAMBAHAN
							, FG_UANG_MUKA
							, UANG_MUKA_DPP
							, UANG_MUKA_PPN
							, UANG_MUKA_PPNBM
							, REFERENSI
							, CUSTOMERCODE
							, FPJNO
							, ProfitCenter
			)B 
GROUP BY FK
		, CompanyCode
		, BranchCode
		, KD_JENIS_TRANSAKSI
		, FG_PENGGANTI
		, NOMOR_FAKTUR
		, MASA_PAJAK
		, TAHUN_PAJAK
		, TANGGAL_FAKTUR
		, NPWP
		, NAMA
		, ALAMAT_LENGKAP
		, ID_KETERANGAN_TAMBAHAN
		, FG_UANG_MUKA
		, UANG_MUKA_DPP
		, UANG_MUKA_PPN
		, UANG_MUKA_PPNBM
		, REFERENSI
		, CUSTOMERCODE
		, ProfitCenter

end
if @table = 2
begin
	select  'LT' LT, REPLACE(REPLACE(NPWPNo, '.', ''), '-', '') NPWP, CustomerName NAMA, REPLACE(REPLACE(Address1, CHAR(13),''),CHAR(10),'')  + REPLACE(REPLACE(Address2, CHAR(13),''),CHAR(10),'') JALAN, '-' BLOK, '-' NOMOR, '0' RT, '0' RW, 
	KecamatanDistrik KECAMATAN, KelurahanDesa KELURAHAN, KotaKabupaten KABUPATEN, 
	case when isnull(ProvinceCode,'') = '' then '-' else (isnull((select top 1 lookupvaluename from gnmstlookupdtl where codeid='PROV' and LookUpValue = ProvinceCode),'-' ))end PROPINSI, 
	ZipNo KODE_POS, PhoneNo NOMOR_TELEPON, CUSTOMERCODE
	from gnMstCustomer 
	where CustomerCode in 
	(select distinct CustomerCode from gnTaxOut WHERE
					CompanyCode = @CompanyCode
					AND BranchCode = @BranchCode
					AND ProductType = @ProductType
					--AND PeriodYear = @PeriodYear
					--AND PeriodMonth = @PeriodMonth
					AND CONVERT(VARCHAR, TaxDate, 112) BETWEEN @PeriodFrom AND @PeriodTo 
					AND IsPKP = 1)
end
if @table = 3
begin
		select * from (
						select 'OF' [OF]
						, PartNo KODE_OBJEK
						, (select replace(rtrim(Partname),',',' ') from spmstiteminfo where companycode = a.companycode and partno = a.partno) NAMA
						, isnull ((convert(decimal(12,2),a.retailprice)), 0.00) HARGA_SATUAN
						, isnull ((convert(decimal(12,2),a.QtyBill)), 0.00) JUMLAH_BARANG
						, isnull ((convert(decimal(12,2),a.SalesAmt)), 0.00)HARGA_TOTAL
						, isnull ((convert(decimal(12,2),a.DiscAmt)), 0.00) DISKON
						, isnull ((convert(decimal(12,2),a.NetSalesAmt)), 0.00) DPP
						, isnull ((convert(decimal(12,2),a.NetSalesAmt * 0.10)), 0.00) PPN
						, 0 TARIF_PPNBM
						, 0 PPNBM
						, b.CUSTOMERCODE 
						, substring(REPLACE(REPLACE(b.TaxNo, '.', ''), '-', ''),4,13) NOMOR_FAKTUR
						, b.FPJNo
						from spTrnSInvoicedtl a
						inner join (select companycode, branchcode, fpjno, customercode, referenceNo, TaxNo
									from gntaxout 
									 WHERE CompanyCode = @CompanyCode
											AND BranchCode = @BranchCode
											AND ProductType = @ProductType
											--AND PeriodYear = @PeriodYear
											--AND PeriodMonth = @PeriodMonth
											AND CONVERT(VARCHAR, TaxDate, 112) BETWEEN @PeriodFrom AND @PeriodTo 
											AND IsPKP = 1
											AND left(fpjno,3)='FPJ'
									) b
							on a.companycode = b.companycode and a.branchcode = b.branchcode
								and a.InvoiceNo = b.referenceNo
						union all
						select 'OF' [OF]
						, SalesModelCode KODE_OBJEK
						, (select SalesModelDesc from ommstmodel where companycode = a.companycode and salesmodelcode = a.salesmodelcode) NAMA
						--, 'Sales Model Desc : '+ (select SalesModelDesc from ommstmodel where companycode = a.companycode and salesmodelcode = a.salesmodelcode) + 
						--  +' Nomor Rangka : '+ (select CONVERT(varchar, ChassisNo, 100) from omTrSalesInvoiceVin where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and 
						--						InvoiceNo = a.InvoiceNo and BPKNo = a.BPKNo and SalesModelCode = a.SalesModelCode and SalesModelYear = a.SalesModelYear)
						--  +' Nomor Mesin : '+ (select CONVERT(varchar, EngineNo, 100) from omTrSalesInvoiceVin where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and 
						--						InvoiceNo = a.InvoiceNo and BPKNo = a.BPKNo and SalesModelCode = a.SalesModelCode and SalesModelYear = a.SalesModelYear) NAMA
						, isnull ((convert(decimal(12,0),a.BeforeDiscDPP)), 0) HARGA_SATUAN
						, isnull ((convert(decimal(12,0),a.Quantity)), 0) JUMLAH_BARANG
						, isnull ((convert(decimal(12,0),a.BeforeDiscDPP * a.Quantity)),0) HARGA_TOTAL
						, isnull ((convert(decimal(12,0),a.DiscExcludePPn * a.Quantity)), 0) DISKON
						--, isnull ((convert(decimal(12,0),a.AfterDiscDPP * a.Quantity)), 0) DPP
						, DPPAmt
						, isnull ((convert(decimal(12,0),a.AfterDiscPPn * a.Quantity)), 0) PPN
						, 0 TARIF_PPNBM
						, 0 PPNBM
						, b.CUSTOMERCODE 
						, substring(REPLACE(REPLACE(b.TaxNo, '.', ''), '-', ''),4,13) TaxNo
						, b.FPJNo
						from omTrSalesInvoicemodel a
						inner join (select companycode, branchcode, fpjno, customercode, referenceNo, TaxNo, DPPAmt
									from gntaxout 
									 WHERE CompanyCode = @CompanyCode
											AND BranchCode = @BranchCode
											AND ProductType = @ProductType
											--AND PeriodYear = @PeriodYear
											--AND PeriodMonth = @PeriodMonth
											AND CONVERT(VARCHAR, TaxDate, 112) BETWEEN @PeriodFrom AND @PeriodTo 
											AND IsPKP = 1
											AND left(fpjno,3) not in ('FPJ','FPS')
									) b
							on a.companycode = b.companycode and a.branchcode = b.branchcode
								and a.InvoiceNo = b.referenceNo	
						union all
						select 'OF' [OF]
						, AccountNo KODE_OBJEK
						, Description NAMA
						, isnull ((convert(decimal(12,0),a.UnitPriceAmt)), 0) HARGA_SATUAN
						, isnull ((convert(decimal(12,0),a.Quantity)), 0) JUMLAH_BARANG
						, isnull ((convert(decimal(12,0),a.UnitPriceAmt * a.Quantity)),0) HARGA_TOTAL
						, isnull ((convert(decimal(12,0),a.DiscAmt)), 0) DISKON
						, isnull ((convert(decimal(12,0),a.UnitPriceAmt * a.Quantity)), 0) DPP
						, isnull ((convert(decimal(12,0),a.PPNAmt)), 0) PPN
						, 0 TARIF_PPNBM
						, 0 PPNBM
						, b.CUSTOMERCODE 
						, substring(REPLACE(REPLACE(b.TaxNo, '.', ''), '-', ''),4,13) NOMOR_FAKTUR
						, b.FPJNo
						from arTrnInvoiceDtl a
						inner join (select companycode, branchcode, fpjno, customercode, referenceNo, TaxNo
									from gntaxout 
									 WHERE CompanyCode = @CompanyCode
											AND BranchCode = @BranchCode
											AND ProductType = @ProductType
											--AND PeriodYear = @PeriodYear
											--AND PeriodMonth = @PeriodMonth
											AND CONVERT(VARCHAR, TaxDate, 112) BETWEEN @PeriodFrom AND @PeriodTo 
											AND IsPKP = 1
											AND left(fpjno,3) not in ('FPJ','FPS')
									) b
							on a.companycode = b.companycode and a.branchcode = b.branchcode
								and a.InvoiceNo = b.referenceNo	
						union all 
						select  
							[OF],
							KODE_OBJEK,
							NAMA,
							HARGA_SATUAN,
							JUMLAH_BARANG,
							HARGA_TOTAL,
							DISKON,
							DPP,
							convert(decimal(12,2),(dpp * 0.1)) PPN,
							TARIF_PPNBM,
							PPNBM,
							CUSTOMERCODE,
							NOMOR_FAKTUR,
							FPJNO
						from(
						select 'OF' [OF]
						, rtrim(PartNo) KODE_OBJEK
						, (select replace(rtrim(Partname),',',' ') from spmstiteminfo where companycode = a.companycode and partno = a.partno) NAMA
						, isnull (convert(decimal(12,2),a.RetailPrice,0),0.00) HARGA_SATUAN
						, isnull (convert(decimal(12,2),a.SupplyQty,0),0.00) JUMLAH_BARANG
						, isnull (convert(decimal(12,2),a.RetailPrice * a.SupplyQty,0),0.00) HARGA_TOTAL
						, isnull (convert(decimal(12,2),a.RetailPrice * a.SupplyQty * DiscPct /100.00,0),0.00) DISKON
						, isnull (convert(decimal(12,2),(a.RetailPrice * a.SupplyQty)-(a.RetailPrice * a.SupplyQty * DiscPct /100.00),0),0.00) DPP
						--, isnull (convert(decimal(12,2),(((a.RetailPrice * a.SupplyQty)-(a.RetailPrice * a.SupplyQty * DiscPct /100.00))*0.10),0),0.00) PPN
						, 0 TARIF_PPNBM
						, 0 PPNBM
						, b.CUSTOMERCODE 
						, substring(REPLACE(REPLACE(b.TaxNo, '.', ''), '-', ''),4,13) NOMOR_FAKTUR
						, b.FPJNo
						from svTrnInvItem a
						inner join (select companycode, branchcode, fpjno, customercode, referenceNo, TaxNo
									from gntaxout 
									 WHERE CompanyCode = @CompanyCode
											AND BranchCode = @BranchCode
											AND ProductType = @ProductType
											--AND PeriodYear = @PeriodYear
											--AND PeriodMonth = @PeriodMonth
											AND CONVERT(VARCHAR, TaxDate, 112) BETWEEN @PeriodFrom AND @PeriodTo 
											AND IsPKP = 1
											AND (left(fpjno,3)='FPS' OR LEFT(FPJNo,3)='FPH')
									) b
							on a.companycode = b.companycode and a.branchcode = b.branchcode
								and a.InvoiceNo = b.referenceNo	
					)Z
-- tambahan untuk jasa service
						union all 
						select  
							[OF],
							KODE_OBJEK,
							NAMA,
							HARGA_SATUAN,
							JUMLAH_BARANG,
							HARGA_TOTAL,
							DISKON,
							DPP,
							convert(decimal(12,2),(dpp * 0.1)) PPN,
							TARIF_PPNBM,
							PPNBM,
							CUSTOMERCODE,
							NOMOR_FAKTUR,
							FPJNO
						from(
						select 'OF' [OF]
						, rtrim(OperationNo) KODE_OBJEK
						, (select top 1 replace(rtrim(Description),',',' ') from svMstTask 
						    where CompanyCode=a.CompanyCode and BasicModel=x.BasicModel and OperationNo=a.OperationNo
						  ) NAMA
						, isnull (convert(decimal(12,2),a.OperationCost,0),0.00) HARGA_SATUAN
						, isnull (convert(decimal(12,2),a.OperationHour,0),0.00) JUMLAH_BARANG
						, isnull (convert(decimal(12,2),a.OperationCost * a.OperationHour,0),0.00) HARGA_TOTAL
						, isnull (convert(decimal(12,2),a.OperationCost * a.OperationHour * DiscPct /100.00,0),0.00) DISKON
						, isnull (convert(decimal(12,2),(a.OperationCost * a.OperationHour)-(a.OperationCost * a.OperationHour * DiscPct /100.00),0),0.00) DPP
						, isnull (convert(decimal(12,2),(((a.OperationCost * a.OperationHour)-(a.OperationCost * a.OperationHour * DiscPct /100.00))*0.10),0),0.00) PPN
						, 0 TARIF_PPNBM
						, 0 PPNBM
						, b.CUSTOMERCODE 
						, substring(REPLACE(REPLACE(b.TaxNo, '.', ''), '-', ''),4,13) NOMOR_FAKTUR
						, b.FPJNo
						from svTrnInvTask a
						inner join svTrnInvoice x
						    on a.CompanyCode=x.CompanyCode and a.BranchCode=x.BranchCode and a.InvoiceNo=x.InvoiceNo
						inner join (select companycode, branchcode, fpjno, customercode, referenceNo, TaxNo
									from gntaxout 
									 WHERE CompanyCode = @CompanyCode
											AND BranchCode = @BranchCode
											AND ProductType = @ProductType
											AND CONVERT(VARCHAR, TaxDate, 112) BETWEEN @PeriodFrom AND @PeriodTo 
											AND IsPKP = 1
											AND (left(fpjno,3)='FPS' OR LEFT(FPJNo,3)='FPH')
									) b
							on a.companycode = b.companycode and a.branchcode = b.branchcode
								and a.InvoiceNo = b.referenceNo	
					)Y
				) a
	 order by NOMOR_FAKTUR
end

GO
