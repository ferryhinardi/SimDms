if object_id('sp_BBNKIRBrowse') is not null
	drop procedure sp_BBNKIRBrowse
GO

CREATE procedure [dbo].[sp_BBNKIRBrowse]  @CompanyCode varchar(10), @BranchCode varchar(10)
 as
 
SELECT a.CompanyCode, a.BranchCode, a.SupplierCode, b.SupplierName, a.CityCode, c.LookUpValueName as CityName
		, a.SalesModelCode, d.SalesModelDesc, a.SalesModelYear, d.SalesModelDesc as SalesModelYearDesc, CAST(a.Status as bit) as Status
		, a.BBN, a.KIR, a.Remark
FROM omMstBBNKIR a
INNER JOIN gnMstSupplier b
	ON a.SupplierCode = b.SupplierCode
INNER JOIN gnMstLookUpDtl c
	ON a.CityCode = c.LookUpValue AND c.CodeID = 'CITY'
INNER JOIN omMstModel d
	ON a.SalesModelCode = d.SalesModelCode
WHERE a.CompanyCode = @CompanyCode AND a.BranchCode = @BranchCode 
GO

if object_id('sp_CheckValidTrans') is not null
	drop procedure sp_CheckValidTrans
GO





CREATE procedure [dbo].[sp_CheckValidTrans] (  

@CompanyCode varchar(10) ,
@BranchCode varchar(10),
@BegDate varchar(10),
@EndDate varchar(10))


as

SELECT CompanyCode, BranchCode, POSNo AS DocNo, Status, TypeOfGoods, TableName = 'spTrnPPOSHdr'
FROM spTrnPPOSHdr
WHERE CompanyCode = @CompanyCode 
AND BranchCode = @BranchCode
AND (Status NOT IN (2, 3, 4, 5, 6, 7))
AND (CONVERT(VARCHAR,POSDate,111) BETWEEN @BegDate AND @EndDate)

SELECT CompanyCode, BranchCode, BinningNo AS DocNo, Status, TypeOfGoods, TableName = 'spTrnPBinnHdr'
FROM spTrnPBinnHdr
WHERE CompanyCode = @CompanyCode 
AND BranchCode = @BranchCode
AND (Status NOT IN (3, 4))
AND (CONVERT(VARCHAR,BinningDate,111) BETWEEN @BegDate AND @EndDate)

SELECT CompanyCode, BranchCode, WRSNo AS DocNo, Status, TypeOfGoods, TableName = 'spTrnPRcvHdr' 
FROM spTrnPRcvHdr
WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
AND (Status  NOT IN (3, 4))
--AND (ReceivingType NOT IN (3))
AND ReceivingType = 1  -- add 30/10/2010  by dRU
AND TransType = 4      -- add 30/10/2010  by dRU
AND (CONVERT(VARCHAR,WRSDate,111) BETWEEN @BegDate AND @EndDate)

SELECT CompanyCode, BranchCode, WRSNo AS DocNo, Status, TypeOfGoods, TableName = 'spTrnPRcvHdr' 
FROM spTrnPRcvHdr
WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
AND (Status NOT IN (2,3))
AND (ReceivingType IN (3))
AND (CONVERT(VARCHAR,WRSDate,111) BETWEEN @BegDate AND @EndDate)

SELECT CompanyCode, BranchCode, WRSNo AS DocNo, Status, TypeOfGoods, TableName = 'spTrnPHPP' 
FROM spTrnPHPP
WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
AND (Status NOT IN (2, 3))
AND (CONVERT(VARCHAR,WRSDate,111) BETWEEN @BegDate AND @EndDate)

SELECT CompanyCode, BranchCode, ClaimNo AS DocNo, Status, TypeOfGoods, TableName = 'spTrnPClaimHdr'
FROM spTrnPClaimHdr
WHERE CompanyCode = @CompanyCode 
AND BranchCode = @BranchCode
AND (Status NOT IN (2, 3, 4, 5, 6, 7))
AND (CONVERT(VARCHAR,ClaimDate,111) BETWEEN @BegDate AND @EndDate)

SELECT CompanyCode, BranchCode, ClaimReceivedNo AS DocNo, Status, TypeOfGoods, TableName = 'spTrnPRcvClaimHdr'
FROM spTrnPRcvClaimHdr
WHERE CompanyCode = @CompanyCode 
AND BranchCode = @BranchCode
AND (Status NOT IN (2, 3, 4, 5, 6, 7, 8))
AND (CONVERT(VARCHAR,ClaimReceivedDate,111) BETWEEN @BegDate AND @EndDate)

SELECT a.CompanyCode, a.BranchCode, a.LampiranNo AS DocNo, a.Status, b.TypeOfGoods, TableName = 'spUtlStockTrfHdr' 
FROM spUtlStockTrfHdr a
	INNER JOIN SpTrnSLmpHdr b ON a.CompanyCode = b.CompanyCode AND a.DealerCode = b.BranchCode AND a.LampiranNo = b.LmpNo
WHERE a.CompanyCode = @CompanyCode 
    AND a.BranchCode = @BranchCode
	AND (a.Status <> 2)
	AND (CONVERT(VARCHAR,b.LmpDate,111) BETWEEN @BegDate AND @EndDate)

SELECT CompanyCode, BranchCode, DocNo AS DocNo, Status, TypeOfGoods, TableName = 'spTrnSORDHdr' 
FROM spTrnSORDHdr
WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
AND (Status NOT IN (2, 3, 4, 5))
AND (CONVERT(VARCHAR,DocDate,111) BETWEEN @BegDate AND @EndDate)

SELECT DISTINCT a.CompanyCode, a.BranchCode, a.DocNo, b.Status, b.TypeOfGoods, TableName = 'spTrnSOSupply' 
FROM spTrnSOSupply a
	 INNER JOIN spTrnSORDHdr b ON a.CompanyCode = b.CompanyCode
	 AND a.BranchCode = b.BranchCode
	 AND a.DocNo = b.DocNo
WHERE a.CompanyCode = @CompanyCode AND a.BranchCode = @BranchCode
AND (LEN(ISNULL(PickingSlipNo, '')) = 0)
AND (CONVERT(VARCHAR, b.DocDate, 111) BETWEEN @BegDate AND @EndDate)

SELECT CompanyCode, BranchCode, PickingSlipNo AS DocNo, Status, TypeOfGoods, TableName = 'spTrnSPickingHdr' 
FROM spTrnSPickingHdr
WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
AND (Status NOT IN (2, 3))
AND (CONVERT(VARCHAR,PickingSlipDate,111) BETWEEN @BegDate AND @EndDate)

SELECT CompanyCode, BranchCode, InvoiceNo AS DocNo, Status, TypeOfGoods, TableName = 'spTrnSInvoiceHdr'
FROM spTrnSInvoiceHdr 
WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
AND (Status NOT IN (2, 3)) 
AND (CONVERT(VARCHAR,InvoiceDate,111) BETWEEN @BegDate AND @EndDate)
                
SELECT CompanyCode, BranchCode, BPSFNo AS DocNo, Status, TypeOfGoods, TableName = 'spTrnSBPSFHdr'
FROM spTrnSBPSFHdr 
WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
AND (Status NOT IN (2, 3))
AND (CONVERT(VARCHAR,BPSFDate,111) BETWEEN @BegDate AND @EndDate)

SELECT CompanyCode, BranchCode, FPJNo AS DocNo, Status, TypeOfGoods, TableName = 'spTrnSFPJHdr'
FROM spTrnSFPJHdr 
WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
AND (Status NOT IN (1, 2, 3))
AND ISNULL(FpjGovNo,'') = ''
AND (CONVERT(VARCHAR,FPJDate,111) BETWEEN @BegDate AND @EndDate)

SELECT CompanyCode, BranchCode, LmpNo AS DocNo, Status, TypeOfGoods, TableName = 'spTrnSLmpHdr'
FROM spTrnSLmpHdr 
WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
AND (Status NOT IN (1, 2, 3))
AND (CONVERT(VARCHAR,LmpDate,111) BETWEEN @BegDate AND @EndDate)

SELECT CompanyCode, BranchCode, ReturnNo AS DocNo, Status, TypeOfGoods, TableName = 'spTrnSRturHdr' 
FROM spTrnSRturHdr
WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
AND (Status NOT IN (2, 3))
AND (CONVERT(VARCHAR,ReturnDate,111) BETWEEN @BegDate AND @EndDate)

SELECT CompanyCode, BranchCode, ReturnNo AS DocNo, Status, '-' TypeOfGoods, TableName = 'spTrnSRturSrvHdr' 
FROM spTrnSRturSrvHdr
WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
AND (Status NOT IN (2, 3))
AND (CONVERT(VARCHAR,ReturnDate,111) BETWEEN @BegDate AND @EndDate)

SELECT CompanyCode, BranchCode, ReturnNo AS DocNo, Status, TypeOfGoods, TableName = 'spTrnSRturSSHdr' 
FROM spTrnSRturSSHdr                           
WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
AND (Status NOT IN (2, 3))
AND (CONVERT(VARCHAR,ReturnDate,111) BETWEEN @BegDate AND @EndDate)

SELECT CompanyCode, BranchCode, AdjustmentNo AS DocNo, Status, TypeOfGoods, TableName = 'spTrnIAdjustHdr'
FROM spTrnIAdjustHdr
WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
AND (Status NOT IN (2, 3))
AND (CONVERT(VARCHAR,AdjustmentDate,111) BETWEEN @BegDate AND @EndDate)

SELECT CompanyCode, BranchCode, WHTrfNo AS DocNo, Status, TypeOfGoods, TableName = 'spTrnIWHTrfHdr'
FROM spTrnIWHTrfHdr
WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
AND (Status NOT IN (2, 3))
AND (CONVERT(VARCHAR,WHTrfDate,111) BETWEEN @BegDate AND @EndDate)

SELECT CompanyCode, BranchCode, ReservedNo AS DocNo, Status, TypeOfGoods, TableName = 'spTrnIReservedHdr'
FROM spTrnIReservedHdr
WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
AND (Status NOT IN (2, 3))
AND (CONVERT(VARCHAR,ReservedDate,111) BETWEEN @BegDate AND @EndDate)

SELECT CompanyCode, BranchCode, STHdrNo AS DocNo, Status, TypeOfGoods, TableName = 'spTrnStockTakingHdr'
FROM spTrnStockTakingHdr
WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
AND (Status NOT IN (2, 3))
AND (CONVERT(VARCHAR,CreatedDate,111) BETWEEN @BegDate AND @EndDate)



GO

if object_id('sp_columns') is not null
	drop procedure sp_columns
GO
create procedure [dbo].[sp_columns]
(
    @table_name         nvarchar(384),
    @table_owner        nvarchar(384) = null,
    @table_qualifier    sysname = null,
    @column_name        nvarchar(384) = null,
    @ODBCVer            int = 2
)
as
    declare @full_table_name    nvarchar(769) -- 384 + 1 + 384
    declare @table_id           int
    declare @fUsePattern        bit

    select @fUsePattern = 1

    if (@ODBCVer is null) or (@ODBCVer <> 3)
        select @ODBCVer = 2

    if @table_qualifier is not null
    begin
        if db_name() <> @table_qualifier
        begin   -- If qualifier doesn't match current database
            raiserror (15250, -1,-1)
            return
        end
    end

    -- "ALL" is represented by NULL value.
    if @table_name = '%'
        select @table_name = null
    if @table_owner = '%'
        select @table_owner = null
    if @table_qualifier = '%'
        select @table_qualifier = null
    if @column_name = '%'
        select @column_name = null

    -- Empty string means nothing, so use invalid identifier.
    -- A quoted space will never match any object name.
    if @table_owner = ''
        select @table_owner = ' '

    select @full_table_name = isnull(quotename(@table_owner), '') + '.' + isnull(quotename(@table_name), '')
    select @table_id = object_id(@full_table_name)

    if (@fUsePattern = 1) -- Does the user want it?
    begin
        if ((isnull(charindex('%', @full_table_name),0) = 0) and
            (isnull(charindex('_', @full_table_name),0) = 0) and
            (isnull(charindex('[', @table_name),0) = 0) and
            (isnull(charindex('[', @table_owner),0) = 0) and
            (isnull(charindex('%', @column_name),0) = 0) and
            (isnull(charindex('_', @column_name),0) = 0) and
            (@table_id <> 0))
        begin
            select @fUsePattern = 0 -- not a single wild char, so go the fast way.
        end
    end

    if @fUsePattern = 0
    begin
        /* -- Debug output, do not remove it.
        print '*************'
        print 'No pattern matching.'
        print @fUsePattern
        print isnull(convert(sysname, @table_id), '@table_id = null')
        print isnull(@full_table_name, '@full_table_name = null')
        print isnull(@table_owner, '@table_owner = null')
        print isnull(@table_name, '@table_name = null')
        print isnull(@column_name, '@column_name = null')
        print '*************'
        */
        select
            TABLE_QUALIFIER             = s_cov.TABLE_QUALIFIER,
            TABLE_OWNER                 = s_cov.TABLE_OWNER,
            TABLE_NAME                  = s_cov.TABLE_NAME,
            COLUMN_NAME                 = s_cov.COLUMN_NAME,
            DATA_TYPE                   = s_cov.DATA_TYPE_28,
            TYPE_NAME                   = s_cov.TYPE_NAME_28,
            "PRECISION"                 = s_cov.PRECISION_28,
            "LENGTH"                    = s_cov.LENGTH_28,
            SCALE                       = s_cov.SCALE_90,
            RADIX                       = s_cov.RADIX,
            NULLABLE                    = s_cov.NULLABLE,
            REMARKS                     = s_cov.REMARKS,
            COLUMN_DEF                  = s_cov.COLUMN_DEF,
            SQL_DATA_TYPE               = s_cov.SQL_DATA_TYPE_28,
            SQL_DATETIME_SUB            = s_cov.SQL_DATETIME_SUB_90,
            CHAR_OCTET_LENGTH           = s_cov.CHAR_OCTET_LENGTH_28,
            ORDINAL_POSITION            = s_cov.ORDINAL_POSITION,
            IS_NULLABLE                 = s_cov.IS_NULLABLE,
            SS_DATA_TYPE                = s_cov.SS_DATA_TYPE

        from
            sys.spt_columns_odbc_view s_cov

        where
            s_cov.object_id = @table_id -- (2nd) (@table_name is null or o.name like @table_name)
            -- (2nd) and (@table_owner is null or schema_name(o.schema_id) like @table_owner)
            and (@column_name is null or s_cov.COLUMN_NAME = @column_name) -- (2nd)             and (@column_name is NULL or c.name like @column_name)
            and s_cov.ODBCVER = @ODBCVer
            and s_cov.OBJECT_TYPE <> 'TT'
            and ( s_cov.SS_IS_SPARSE = 0 OR objectproperty ( s_cov.OBJECT_ID, 'tablehascolumnset' ) = 0 )
        order by 17
    end
    else
    begin
        /* -- Debug output, do not remove it.
        print '*************'
        print 'THERE IS pattern matching!'
        print @fUsePattern
        print isnull(convert(sysname, @table_id), '@table_id = null')
        print isnull(@full_table_name, '@full_table_name = null')
        print isnull(@table_owner, '@table_owner = null')
        print isnull(@table_name, '@table_name = null')
        print isnull(@column_name, '@column_name = null')
        print '*************'
    */
        select
            TABLE_QUALIFIER             = s_cov.TABLE_QUALIFIER,
            TABLE_OWNER                 = s_cov.TABLE_OWNER,
            TABLE_NAME                  = s_cov.TABLE_NAME,
            COLUMN_NAME                 = s_cov.COLUMN_NAME,
            DATA_TYPE                   = s_cov.DATA_TYPE_28,
            TYPE_NAME                   = s_cov.TYPE_NAME_28,
            "PRECISION"                 = s_cov.PRECISION_28,
            "LENGTH"                    = s_cov.LENGTH_28,
            SCALE                       = s_cov.SCALE_90,
            RADIX                       = s_cov.RADIX,
            NULLABLE                    = s_cov.NULLABLE,
            REMARKS                     = s_cov.REMARKS,
            COLUMN_DEF                  = s_cov.COLUMN_DEF,
            SQL_DATA_TYPE               = s_cov.SQL_DATA_TYPE_28,
            SQL_DATETIME_SUB            = s_cov.SQL_DATETIME_SUB_90,
            CHAR_OCTET_LENGTH           = s_cov.CHAR_OCTET_LENGTH_28,
            ORDINAL_POSITION            = s_cov.ORDINAL_POSITION,
            IS_NULLABLE                 = s_cov.IS_NULLABLE,
            SS_DATA_TYPE                = s_cov.SS_DATA_TYPE

        from
            sys.spt_columns_odbc_view s_cov

        where
            s_cov.ODBCVER = @ODBCVer and
            s_cov.OBJECT_TYPE <> 'TT' and
            (@table_name is null or s_cov.TABLE_NAME like @table_name) and
            (@table_owner is null or schema_name(s_cov.SCHEMA_ID) like @table_owner) and
            (@column_name is null or s_cov.COLUMN_NAME like @column_name) and
            ( s_cov.SS_IS_SPARSE = 0 OR objectproperty ( s_cov.OBJECT_ID, 'tablehascolumnset' ) = 0 )

        order by 2, 3, 17
    end

GO

if object_id('sp_EdpBpsNoBrowse') is not null
	drop procedure sp_EdpBpsNoBrowse
GO





CREATE procedure [dbo].[sp_EdpBpsNoBrowse] (  

@CompanyCode varchar(10) ,
@BranchCode varchar(10),
@TypeOfGoods varchar(10),
@ProductType varchar(10),
@CustomerCode varchar(30))


as

SELECT 
 a.BPSFNo
,a.BPSFDate
,a.PickingSlipNo
,a.PickingSlipDate
FROM spTrnSBPSFHdr a
WHERE a.CompanyCode=@CompanyCode
  AND a.BranchCode=@BranchCode
  AND a.CustomerCode=@CustomerCode
  AND a.TypeOfGoods=@TypeOfGoods
GROUP BY  a.BPSFNo, a.BPSFDate, a.PickingSlipNo, a.PickingSlipDate
ORDER BY  a.BPSFNo DESC





GO

if object_id('sp_EdpDocNoBrowse') is not null
	drop procedure sp_EdpDocNoBrowse
GO






CREATE procedure [dbo].[sp_EdpDocNoBrowse] (  

@CompanyCode varchar(10) ,
@BranchCode varchar(10),
@TypeOfGoods varchar(10),
@ProductType varchar(10),
@SupplierCode varchar(30))


as

SELECT 
 a.POSNo
,a.PosDate
,a.SupplierCode
FROM SpTrnPOrderBalance a with(nolock, nowait)
INNER JOIN SpTrnPPOSHdr b ON b.POSNo = a.POSNo AND b.CompanyCode = a.CompanyCode AND b.BranchCode = a.BranchCode
WHERE a.OnOrder > 0
  AND a.CompanyCode=@CompanyCode
  AND a.BranchCode=@BranchCode
  AND a.TypeOfGoods=@TypeOfGoods
  AND b.ProductType=@ProductType
  AND a.SupplierCode=@SupplierCode
  --and b.Status = 2
GROUP BY  a.POSNo, a.PosDate, a.SupplierCode 
ORDER BY  a.POSNo DESC






GO

if object_id('sp_EdpPartNo_Pembelian') is not null
	drop procedure sp_EdpPartNo_Pembelian
GO




CREATE procedure [dbo].[sp_EdpPartNo_Pembelian]  (  
@CompanyCode varchar(10),
@BranchCode varchar(10),
@DocNo varchar(20)
--@BinningNo varchar(20),
--@Opt varchar(10)
)
 as 
 
--SELECT DISTINCT a.POSNo, a.PartNo, c.PartName, CASE when e.ReceiveQty is null then a.OnOrder else a.OrderQty-e.ReceiveQty end as ReminQty,
--d.PurchasePrice, a.DiscPct, 
--a.OnOrder as MaxReceived
--from spTrnPOrderBalance a 
--LEFT JOIN spTrnPBinnDtl b ON
--a.CompanyCode = b.CompanyCode
--and a.BranchCode = b.BranchCode
--and a.POSNo = b.DocNo
--AND a.PartNo = b.PartNo
--LEFT JOIN spMstItemInfo c ON
--a.CompanyCode = c.CompanyCode
--and a.PartNo = c.PartNo
--LEFT JOIN SpMstItemPrice d ON
--a.CompanyCode = d.CompanyCode
--and a.BranchCode = d.BranchCode
--AND a.PartNo = d.PartNo LEFT JOIN
--(SELECT PartNo, DocNo, sum(ReceivedQty) as ReceiveQty from spTrnPBinnDtl WHERE CompanyCode = @CompanyCode
--AND BranchCode = @BranchCode
--and DocNo = @DocNo GROUP BY PartNo, DocNo) e
--ON a.PartNo = e.PartNo
--AND a.POSNo = e.DocNo
--where
--a.CompanyCode = @CompanyCode
--AND a.BranchCode = @BranchCode
--and a.POSNo = @DocNo

SELECT c.PartNo, c.PartName, c.PurchasePrice, c.DiscPct, SUM(c.OnOrder) AS MaxReceived
FROM
(
    SELECT
     a.PartNo
    ,b.PartName 
    ,ISNULL((SELECT round(x.PurchasePrice + (x.PurchasePrice * (select (TaxPct/100) from gnMstTax
where TaxCode = (select ParaValue from gnMstLookUpDtl where CodeID = 'BINS' and SeqNo = 3))),0)
FROM SpMstItemPrice x where x.CompanyCode = a.CompanyCode AND
     x.BranchCode = a.BranchCode AND x.PartNo = a.PartNo),0) AS PurchasePrice
    ,a.OnOrder
    ,a.DiscPct
    FROM spTrnPOrderBalance a 
    INNER JOIN spMstItemInfo b
       ON b.CompanyCode = a.CompanyCode
      AND b.PartNo      = a.PartNo
      WHERE a.CompanyCode = @CompanyCode
      AND a.BranchCode  = @BranchCode
      AND a.PosNo       = @DocNo      
) c
GROUP BY c.PartNo, c.PartName, c.PurchasePrice, c.DiscPct
HAVING sum(c.OnOrder) > 0




GO

if object_id('sp_EdpPelangganBrowse') is not null
	drop procedure sp_EdpPelangganBrowse
GO





CREATE procedure [dbo].[sp_EdpPelangganBrowse] (  

@CompanyCode varchar(10),
@BranchCode varchar(10),
@ProfitCenter varchar(30))


as

SELECT a.CustomerCode, a.CustomerName,
       a.Address1+' '+a.Address2+' '+a.Address3+' '+a.Address4 as Address,
       c.LookUpValueName as ProfitCenter
  FROM gnMstCustomer a with(nolock, nowait)
    INNER JOIN gnMstCustomerProfitCenter b with(nolock, nowait) ON 
        a.CompanyCode = b.CompanyCode 
        AND b.BranchCode = @BranchCode
        AND b.ProfitCenterCode = @ProfitCenter
        AND b.CustomerCode = a.CustomerCode
        AND b.isBlackList=0
    INNER JOIN gnMstLookUpDtl c ON c.CompanyCode= a.CompanyCode
         AND c.LookupValue= b.ProfitCenterCode
         AND c.CodeID= 'PFCN'
 WHERE  a.CompanyCode=@CompanyCode
    AND a.status = 1   
    ORDER BY a.CustomerCode
    






GO

if object_id('sp_EdpTransNo') is not null
	drop procedure sp_EdpTransNo
GO







CREATE procedure [dbo].[sp_EdpTransNo] (  

@CompanyCode varchar(10),
@BranchCode varchar(10),
@TypeOfGoods varchar(10),
@LampiranNo varchar(10)
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
  AND a.Status in ('0','1') ) #t1
  
SELECT * FROM #t1 WHERE TypeofGoods = @TypeOfGoods
    AND LampiranNo = CASE @LampiranNo WHEN '' THEN LampiranNo ELSE @LampiranNo END

DROP TABLE #t1
    








GO

if object_id('sp_EntryHPPBrowse') is not null
	drop procedure sp_EntryHPPBrowse
GO




CREATE procedure [dbo].[sp_EntryHPPBrowse] (  

@CompanyCode varchar(10) ,
@BranchCode varchar(10),
@TypeOfGoods varchar(10),
@PeriodBeg varchar(30),
@PeriodEnd varchar(30))


as

SELECT a.*, c.DNSupplierNo,
    d.SupplierName,b.BinningNo, b.BinningDate, b.SupplierCode, 
	(
		select LookUpValueName
		from gnMstLookUpDtl
		where CompanyCode=a.CompanyCode and CodeID='STAT' and LookUpValue=a.Status
	) StatusStr
FROM 
    SpTrnPHPP a WITH(NoLock,NoWait)
LEFT JOIN SpTrnPRcvHdr b ON b.CompanyCode = a.CompanyCOde AND b.BranchCode = a.BranchCode
    AND b.WRSNo = a. WRSNo
LEFT JOIN SpTrnPBinnHdr c ON c.CompanyCode = a.CompanyCode AND c.BranchCode = a.BranchCode
    AND c.BinningNo = b.BinningNo 
LEFT JOIN gnMstSupplier d ON d.CompanyCode = a.CompanyCode AND d.SupplierCode = c.SupplierCode
WHERE a.CompanyCode=@CompanyCode
      AND a.BranchCode=@BranchCode
      AND a.TypeOfGoods=@TypeOfGoods
      AND Convert(Varchar, a.HPPDate, 112) between @PeriodBeg and @PeriodEnd
      ORDER BY a.HPPNO DESC





GO

if object_id('sp_GetMaxQtyBinning') is not null
	drop procedure sp_GetMaxQtyBinning
GO

CREATE procedure [dbo].[sp_GetMaxQtyBinning]  (  
@CompanyCode varchar(10),
@BranchCode varchar(10),
@DocNo varchar(20),
@SupplierCode varchar(10),
--@CustomerCode varchar(10),
@PartNo varchar(20),
@BinningNo varchar(20),
@Opt varchar(10)
)
 as 
 
IF @Opt = 'P'
BEGIN
 
SELECT 
OrderQty, ReceivedQty
FROM
(
SELECT ISNULL(SUM(OnOrder),0) AS OrderQty
FROM SpTrnPOrderBalance with(nolock, nowait)
WHERE CompanyCode=@CompanyCode
AND BranchCode=@BranchCode
AND POSNo=@DocNo
AND SupplierCode=@SupplierCode
AND PartNo=@PartNo	                
) AS PO,
(
SELECT ISNULL(SUM(ReceivedQty),0) AS ReceivedQty
FROM spTrnPBinnDtl
INNER JOIN spTrnPBinnHdr ON spTrnPBinnDtl.CompanyCode = spTrnPBinnHdr.CompanyCode
AND spTrnPBinnDtl.BranchCode = spTrnPBinnHdr.BranchCode
AND spTrnPBinnDtl.BinningNo = spTrnPBinnHdr.BinningNo
WHERE spTrnPBinnDtl.CompanyCode = @CompanyCode 
AND spTrnPBinnDtl.BranchCode = @BranchCode AND DocNo = @DocNo AND PartNo = @PartNo
AND spTrnPBinnDtl.BinningNo NOT IN(@BinningNo) AND Status IN ('0', '1')
GROUP BY DocNo
) AS BINN            
END

ELSE 
BEGIN
SELECT 
OrderQty, ReceivedQty
FROM(
SELECT ISNULL(SUM(a.QtyBill),0) OrderQty
FROM SpTrnSBPSFDtl a with(nolock, nowait)
INNER JOIN SpTrnSBPSFHdr b ON b.CompanyCode = a.CompanyCode AND b.BranchCode = a.BranchCode AND b.BPSFNo = a.BPSFNo
WHERE a.CompanyCode = @CompanyCode
AND a.BranchCode = @BranchCode
AND a.BPSFNo = @DocNo
AND b.CustomerCode = @SupplierCode
AND a.PartNo = @PartNo                
) AS PO,
(
SELECT ISNULL(SUM(ReceivedQty),0) AS ReceivedQty
FROM spTrnPBinnDtl
INNER JOIN spTrnPBinnHdr ON spTrnPBinnDtl.CompanyCode = spTrnPBinnHdr.CompanyCode
AND spTrnPBinnDtl.BranchCode = spTrnPBinnHdr.BranchCode
AND spTrnPBinnDtl.BinningNo = spTrnPBinnHdr.BinningNo
WHERE spTrnPBinnDtl.CompanyCode = @CompanyCode 
AND spTrnPBinnDtl.BranchCode = @BranchCode AND DocNo = @DocNo AND PartNo = @PartNo	         
AND spTrnPBinnDtl.BinningNo NOT IN(@BinningNo)
) AS BINN  
END
                 

GO

if object_id('sp_gnMstSupplierView') is not null
	drop procedure sp_gnMstSupplierView
GO
create procedure [dbo].[sp_gnMstSupplierView]  (  @CompanyCode varchar(10) ,@BranchCode varchar(10))
as
select 
CompanyCode,
SupplierCode,
BranchCode,
ProfitCenterCode,
SupplierName,
Alamat,
Address1,
Address2,
Address3,
Address4,
Phone,
DiscPct,
[Status],
ProfitCenterName,
TOPDays,
CityCode,
CityName
from gnMstSupplierView
where CompanyCode=@CompanyCode and   BranchCode=@BranchCode

GO

if object_id('sp_InquirDataKendaraan') is not null
	drop procedure sp_InquirDataKendaraan
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- =============================================
CREATE procedure [dbo].[sp_InquirDataKendaraan] 
(
 @p_CompanyCode varchar(15),
 @p_BranchCode varchar(15),
 @p_Status varchar(2),
 @WarehouseCode varchar(100),
 @WarehouseCodeTo varchar(100),
 @SalesModelCode varchar(100),
 @SalesModelCodeTo varchar(100),
 @SalesModelYear varchar(100),
 @SalesModelYearTo varchar(100),
 @ChassisCode varchar(100),
 @ChassisCodeTo varchar(100),
 @ChassisNo varchar(100),
 @ChassisNoTo varchar(100)
)
AS
BEGIN

declare @pQuery varchar(max)
set @pQuery =
'

select a.salesModelCode, (select b.SalesModelDesc from OmMstModel b
where a.CompanyCode = b.CompanyCode and a.SalesModelCode = b.SalesModelCode) as SalesModelDesc,
a.salesModelYear, a.chassisCode, convert(varchar, a.chassisNo) as chassisNo, a.engineCode, 
convert(varchar, a.engineNo) as engineNo,
CASE a.Status 
WHEN 0 THEN ''Ready''
WHEN 1 THEN ''Karoseri''
WHEN 2 THEN ''Return''
WHEN 3 THEN ''Order''
WHEN 4 THEN ''DO''
WHEN 5 THEN ''BPK''
WHEN 6 THEN ''Sales''
WHEN 7 THEN ''Transfer''
END as Status,
case a.isActive when 1 then ''Ya'' else ''Tidak'' end as isActive
from ommstvehicle a
Where 1=1
'

if len(rtrim(@p_Status)) > 0
   set @pQuery = @pQuery + ' and a.Status = ''' + rtrim(@p_Status) + ''''

if len(rtrim(@WarehouseCode)) > 0
   set @pQuery = @pQuery + ' and a.WarehouseCode between ''' + rtrim(@WarehouseCode) + '''' + ' and ' + '''' + rtrim(@WarehouseCodeTo) + ''''

if len(rtrim(@SalesModelCode)) > 0
   set @pQuery = @pQuery + ' and a.SalesModelCode between ''' + rtrim(@SalesModelCode) + '''' + ' and ' + '''' + rtrim(@SalesModelCodeTo) + ''''

if len(rtrim(@SalesModelYear)) > 0
   set @pQuery = @pQuery + ' and a.SalesModelYear between ''' + rtrim(@SalesModelYear) + '''' + ' and ' + '''' + rtrim(@SalesModelYearTo) + ''''

if len(rtrim(@ChassisCode)) > 0
   set @pQuery = @pQuery + ' and a.ChassisCode between ''' + rtrim(@ChassisCode) + '''' + ' and ' + '''' + rtrim(@ChassisCodeTo) + ''''

if len(rtrim(@ChassisNo)) > 0
   set @pQuery = @pQuery + ' and a.ChassisNo between ''' + rtrim(@ChassisNo) + '''' + ' and ' + '''' + rtrim(@ChassisNoTo) + ''''

set @pQuery = @pQuery + ' and a.CompanyCode = ''' + rtrim(@p_CompanyCode) + '''' 
set @pQuery = @pQuery + ' ORDER BY a.SalesModelCode '

print(@pQuery)
exec(@pQuery)
END
--------------------------------------------------- BATAS ----------------------------------------------------------



GO

if object_id('sp_InquirDetailDataKendaraan') is not null
	drop procedure sp_InquirDetailDataKendaraan
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- =============================================
CREATE procedure [dbo].[sp_InquirDetailDataKendaraan] 
(
 @CompanyCode varchar(15),
 @BranchCode varchar(15),
 @ChassisCode varchar(100),
 @ChassisNo varchar(100)
)
AS
BEGIN

select 
	(select b.refferenceDesc1 from ommstrefference b where b.companyCode = a.companyCode
	and b.refferencetype = 'WARE' and b.refferenceCode = a.warehouseCode) as warehouseName
	,'('+a.ColourCode+') '+(select c.refferenceDesc1 from ommstrefference c where c.companyCode = a.companyCode
	and c.refferencetype = 'COLO' and c.refferenceCode = a.ColourCode) as ColourName
	, a.servicebookno
	, a.keyno
	, a.cogsunit
	, a.cogsOthers
	, a.cogsKaroseri
    , o.afterdiscdpp dpp
    , o.afterdiscppn ppn
    , p.bbn
	, j.pono
    , convert(varchar, j.podate, 106) podate
	, h.bpuno+' ('+h.RefferenceDONo+')' bpuno
    , convert(varchar, h.bpudate, 106) bpudate
	, d.sono
    , convert(varchar, d.sodate, 106) sodate
	, k.dono
    , convert(varchar, k.dodate, 106) dodate
    , case d.SKPKNo when '' then d.RefferenceNo else d.SKPKNo end as SKPKNo
    , convert(varchar, d.sodate, 106)  SKPKDate
    , l.bpkno
    , convert(varchar, l.bpkdate, 106) bpkdate
	, m.invoiceNo
    , convert(varchar, m.invoicedate, 106) invoicedate
    , q.RefferenceSJNo
	, convert(varchar, q.RefferenceSJDate, 106) RefferenceSJDate
    , i.hppno
    , convert(varchar, i.hppdate, 106) hppdate
	, n.reqNo reqOutNo
    , convert(varchar, n.reqDate, 106) reqdate
    , i.RefferenceInvoiceNo reffinv
    , convert(varchar, i.RefferenceInvoiceDate, 106) reffinvdate
    , i.RefferenceFakturPajakNo refffp
    , convert(varchar, i.RefferenceFakturPajakDate , 106) refffpdate
	, s.PoliceRegistrationNo policeregno
    , convert(varchar, s.PoliceRegistrationDate, 106) policeregdate
	, isnull(b.CustomerCode + ' - ' + c.CustomerName, 
		k.CustomerCode + ' - ' + (select CustomerName from gnMstCustomer where CompanyCode = @CompanyCode and CustomerCode = k.CustomerCode)
		) as Customer
	, isnull(c.Address1 + ' ' + c.Address2 + ' ' + c.Address3 + ' ' + c.Address4,
		(select Address1 + ' ' + Address2 + ' ' + Address3 + ' ' + Address4 from gnMstCustomer where CompanyCode = @CompanyCode and CustomerCode = k.CustomerCode)
		) as Address
	, d.Salesman + ' - ' + f.EmployeeName as Salesman
	, d.LeasingCo + ' - ' + g.CustomerName as Leasing
	, d.SalesCode + ' - ' + e.LookUpValueName as KelAR
    , s.BPKBNo
	, s.SPKNo
from 
	ommstvehicle a
left join omTrSalesInvoice b on a.CompanyCode = b.CompanyCode and b.BranchCode like @BranchCode 
    and a.InvoiceNo = b.InvoiceNo
left join gnMstCustomer c on a.CompanyCode = c.CompanyCode and b.Customercode = c.CustomerCode
left join omTrSalesSO d on a.CompanyCode = d.CompanyCode and d.BranchCode like @BranchCode and a.SONo = d.SONo
left join GnMstLookUpDtl e on a.CompanyCode = e.CompanyCode and CodeID = 'GPAR' and e.LookUpValue = d.SalesCode
left join gnMstEmployee f on a.CompanyCode  = f.Companycode and f.BranchCode like @BranchCode 
    and f.EmployeeID = d.Salesman
left join gnMstCustomer g on a.CompanyCode = g.CompanyCode and g.CustomerCode = d.LeasingCo
left join omTrPurchaseBPU h on a.CompanyCode= h.CompanyCode and h.BranchCode like @BranchCode 
    and a.PONo = h.PONo and a.BPUNo=h.BPUNo
left join omTrPurchaseHPP i on a.CompanyCode= i.CompanyCode and i.BranchCode like @BranchCode and a.HPPNo= i.HPPNo
left join omTrPurchasePO j on a.CompanyCode = j.CompanyCode and j.BranchCode like @BranchCode and a.PONo = j.PONo
left join omTrSalesDO k on a.CompanyCode = k.CompanyCode and k.BranchCode like @BranchCode and a.DONo = k.DONo and a.SONo= k.SONo
left join omTrSalesBPK l on a.CompanyCode = l.CompanyCode and l.BranchCode like @BranchCode and a.BPKNo = l.BPKNo
left join omTrSalesInvoice m on a.CompanyCode = m.CompanyCode and m.BranchCode like @BranchCode 
    and a.InvoiceNo = m.InvoiceNo
left join omTrSalesReq n on a.CompanyCode = n.CompanyCode and n.BranchCode like @BranchCode and a.ReqOutNo = n.ReqNo
left join omTrSalesSOModel o on a.CompanyCode = o.CompanyCode and o.BranchCode like @BranchCode and a.SONo = o.SONo 
    and a.SalesModelCode = o.SalesModelCode and a.SalesModelYear = o.SalesModelYear and a.ChassisCode = o.ChassisCode
left join omTrSalesSOVin p on a.CompanyCode = p.CompanyCode and p.BranchCode like @BranchCode and a.SONo = p.SONo
    and a.SalesModelCode = p.SalesModelCode and a.SalesModelYear = p.SalesModelYear and a.ColourCode = p.ColourCode
    and a.ChassisNo = p.ChassisNo and a.ChassisCode = p.ChassisCode
left join omTrPurchaseBPU q on a.CompanyCode = q.CompanyCode and q.BranchCode like @BranchCode and q.PONo = j.PONO 
	and q.BPUNo = a.BPUNo
left join omTrSalesSPKDetail s on s.CompanyCode = a.CompanyCode and s.BranchCode like @BranchCode
	and s.ChassisCode = a.ChassisCode and s.ChassisNo = a.ChassisNo
where 
	a.companyCode = @CompanyCode and a.chassisCode = @ChassisCode and a.chassisNo = @ChassisNo

END
GO

if object_id('sp_InquiryBPK') is not null
	drop procedure sp_InquiryBPK
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- =============================================
CREATE procedure [dbo].[sp_InquiryBPK] 
(
 @p_CompanyCode varchar(15),
 @p_BranchCode varchar(15),
 @p_Status varchar(2),
 @SODateFrom DateTime,
 @SODateTo DateTime,
 @NoSOFrom varchar(100),
 @NoSOTo varchar(100),
 @NoDOFrom varchar(100),
 @NoDOTo varchar(100),
 @NoBPKFrom varchar(100),
 @NoBPKTo varchar(100)
)
AS
BEGIN

declare @pQuery varchar(max)
set @pQuery =
'

SELECT a.BPKNo, convert(varchar(20),a.BPKDate,106) as BPKDate, a.SONo, a.DONo, b.CustomerName, 
a.CustomerCode, b.Address1 + '' '' + b.Address2 + '' '' + b.Address3 + '' '' + b.Address4 as Address,
c.SupplierName AS Expedition, case pso.SalesType when ''0'' then ''W'' when ''1'' then ''D'' end as SalesType,
d.RefferenceDesc1 AS WareHouseName, e.CustomerName AS ShipTo, 
a.Remark, 
CASE
	WHEN a.Status = 0 THEN ''Open''
	WHEN a.Status = 1 THEN ''Printed''
	WHEN a.Status = 2 THEN ''Approved''
	WHEN a.Status = 3 THEN ''Canceled''
	WHEN a.Status = 9 THEN ''Finished''
END as Status 
FROM omTrSalesBPK a
LEFT JOIN GnMstCustomer b ON b.CompanyCode = a.CompanyCode AND b.CustomerCode = a.CustomerCode
LEFT JOIN GnMstSupplier c ON c.CompanyCode = a.CompanyCode AND c.SupplierCode = a.Expedition
LEFT JOIN OmMstRefference d ON d.CompanyCode = a.CompanyCode AND d.RefferenceCode = a.WareHouseCode AND d.RefferenceType = ''WARE''
LEFT JOIN GnMstCustomer e ON e.CompanyCode = a.CompanyCode AND e.CustomerCode = a.ShipTo
LEFT JOIN omTrSalesSO pso ON a.CompanyCode = pso.CompanyCode AND a.BranchCode = pso.BranchCode AND a.SONo = pso.SONo
Where 1=1
'

if len(rtrim(@p_Status)) > 0
   set @pQuery = @pQuery + ' and a.Status = ''' + rtrim(@p_Status) + ''''

if year(@SODateFrom) <> '1900'
   set @pQuery = @pQuery + ' and a.BPKDate between ''' + convert(varchar(50),@SODateFrom) + '''' + ' and ' + '''' + convert(varchar(50),@SODateTo) + ''''

if len(rtrim(@NoSOFrom)) > 0
   set @pQuery = @pQuery + ' and a.SONo between ''' + rtrim(@NoSOFrom) + '''' + ' and ' + '''' + rtrim(@NoSOTo) + ''''

if len(rtrim(@NoDOFrom)) > 0
   set @pQuery = @pQuery + ' and a.DONo between ''' + rtrim(@NoDOFrom) + '''' + ' and ' + '''' + rtrim(@NoDOTo) + ''''

if len(rtrim(@NoBPKFrom)) > 0
   set @pQuery = @pQuery + ' and a.BPKNo between ''' + rtrim(@NoBPKFrom) + '''' + ' and ' + '''' + rtrim(@NoBPKTo) + ''''

set @pQuery = @pQuery + ' and a.CompanyCode = ''' + rtrim(@p_CompanyCode) + '''' + ' and a.BranchCode = ''' + rtrim(@p_BranchCode) + ''''
set @pQuery = @pQuery + ' ORDER BY a.BPKNo '

print(@pQuery)
exec(@pQuery)
END
--------------------------------------------------- BATAS ----------------------------------------------------------

GO

if object_id('sp_InquiryBPKDetail') is not null
	drop procedure sp_InquiryBPKDetail
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- =============================================
CREATE procedure [dbo].[sp_InquiryBPKDetail] 
(
 @p_CompanyCode varchar(15),
 @p_BranchCode varchar(15),
 @NoBPK varchar(15)
)
AS
BEGIN

SELECT a.SalesModelCode, a.SalesModelYear, b.SalesModelDesc, 
	a.ChassisCode, Convert(varchar,a.ChassisNo) as ChassisNo, 
	a.EngineCode, Convert(varchar,a.EngineNo) as EngineNo, 
	a.ColourCode, c.RefferenceDesc1 as ColourName, a.Remark, a.StatusPDI,
	a.BPKSeq
FROM   omTrSalesBPKDetail a	   
	LEFT JOIN
	omMstModelYear b
	ON a.CompanyCode = b.CompanyCode
	AND a.SalesModelCode = b.SalesModelCode      
	AND a.ChassisCode = b.ChassisCode
	AND a.SalesModelYear = b.SalesModelYear	   
	LEFT JOIN
	omMstRefference c
	ON a.CompanyCode = c.CompanyCode
	AND a.ColourCode = c.RefferenceCode
	AND c.RefferenceType = 'COLO'
WHERE a.CompanyCode= @p_CompanyCode 
	AND a.BranchCode= @p_BranchCode 
	AND a.BPKNo= @NoBPK
ORDER BY a.ChassisNo ASC

END
GO

if object_id('sp_InquiryBPU') is not null
	drop procedure sp_InquiryBPU
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- =============================================
CREATE procedure [dbo].[sp_InquiryBPU] 
(
 @p_CompanyCode varchar(15),
 @p_BranchCode varchar(15),
 @p_Status varchar(2),
 @BPUType varchar(2),
 @BPUDateFrom DateTime,
 @BPUDateTo DateTime,
 @PONoFrom varchar(100),
 @PONoTo varchar(100),
 @NoRefDOFrom varchar(100),
 @NoRefDOTo varchar(100),
 @NoRefSJFrom varchar(100),
 @NoRefSJTo varchar(100),
 @NoBPUFrom varchar(100),
 @NoBPUTo varchar(100)
)
AS
BEGIN

declare @pQuery varchar(max)
set @pQuery =
'
SELECT 
case when a.BPUType = ''0'' then ''DO'' 
when a.BPUType = ''1'' then ''SJ''
when a.BPUType = ''2'' then ''DO & SJ'' 
when a.BPUType = ''3'' then ''SJ Booking''
end as BPUType, a.BPUNo,
convert(varchar(20),a.BPUDate,106) as BPUDate,
a.RefferenceDONo, 
convert(varchar(20),a.RefferenceDODate,106) as RefferenceDODate, 
a.RefferenceSJNo, 
case when year(a.RefferenceSJDate) = ''1900'' then '''' else convert(varchar(20),a.RefferenceSJDate,106) end as RefferenceSJDate, 
a.SupplierCode, 
(SELECT z.SupplierName FROM gnMstSupplier z 
WHERE z.CompanyCode = a.CompanyCode
AND z.SupplierCode = a.SupplierCode)  AS SupplierName, 
a.ShipTo, a.Expedition, 
(SELECT x.SupplierName
FROM gnMstSupplier x
WHERE x.CompanyCode = a.CompanyCode
AND x.SupplierCode = a.Expedition)  AS ExpeditionName, 
a.WarehouseCode, 
(SELECT y.RefferenceDesc1
FROM omMstRefference y
WHERE y.CompanyCode = a.CompanyCode
AND y.RefferenceCode = a.WarehouseCode
AND y.RefferenceType = ''WARE'')  AS WareHouseName, 
a.Remark, 
CASE WHEN a.Status = 0 THEN ''Open''
WHEN a.Status = 1 THEN ''Printed''
WHEN a.Status = 2 THEN ''Approved''
WHEN a.Status = 3 THEN ''Canceled''
WHEN a.Status = 9 THEN ''Finished''
END  AS Status
FROM omTrPurchaseBPU a
where 1=1
'

if len(rtrim(@p_Status)) > 0
   set @pQuery = @pQuery + ' and a.Status = ''' + rtrim(@p_Status) + ''''

if len(rtrim(@BPUType)) > 0
   set @pQuery = @pQuery + ' and a.BPUType = ''' + rtrim(@BPUType) + ''''

if year(@BPUDateFrom) <> '1900'
   set @pQuery = @pQuery + ' and a.BPUDate between ''' + convert(varchar(50),@BPUDateFrom) + '''' + ' and ' + '''' + convert(varchar(50),@BPUDateTo) + ''''

if len(rtrim(@PONoFrom)) > 0
   set @pQuery = @pQuery + ' and a.PONo between ''' + rtrim(@PONoFrom) + '''' + ' and ' + '''' + rtrim(@PONoTo) + ''''

if len(rtrim(@NoRefDOFrom)) > 0
   set @pQuery = @pQuery + ' and a.RefferenceDONo between ''' + rtrim(@NoRefDOFrom) + '''' + ' and ' + '''' + rtrim(@NoRefDOTo) + ''''

if len(rtrim(@NoRefSJFrom)) > 0
   set @pQuery = @pQuery + ' and a.RefferenceSJNo between ''' + rtrim(@NoRefSJFrom) + '''' + ' and ' + '''' + rtrim(@NoRefSJTo) + ''''

if len(rtrim(@NoBPUFrom)) > 0
   set @pQuery = @pQuery + ' and a.BPUNo between ''' + rtrim(@NoBPUFrom) + '''' + ' and ' + '''' + rtrim(@NoBPUTo) + ''''

set @pQuery = @pQuery + ' and a.CompanyCode = ''' + rtrim(@p_CompanyCode) + '''' + ' and a.BranchCode = ''' + rtrim(@p_BranchCode) + ''''
set @pQuery = @pQuery + ' ORDER BY a.BPUNo '

print(@pQuery)
exec(@pQuery)
END
--------------------------------------------------- BATAS ----------------------------------------------------------
GO

if object_id('sp_InquiryDebetNote') is not null
	drop procedure sp_InquiryDebetNote
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- =============================================
CREATE procedure [dbo].[sp_InquiryDebetNote] 
(
 @p_CompanyCode varchar(15),
 @p_BranchCode varchar(15),
 @p_Status varchar(2),
 @InvDateFrom DateTime,
 @InvDateTo DateTime,
 @NoSOFrom varchar(100),
 @NoSOTo varchar(100),
 @CustomerFrom varchar(100),
 @CustomerTo varchar(100),
 @NoInvFrom varchar(100),
 @NoInvtTo varchar(100),
 @NoSKPKFrom varchar(100),
 @NoSKPKTo varchar(100)
)
AS
BEGIN

declare @pQuery varchar(max)
set @pQuery =
'

SELECT 
a.DNNo as InvoiceNo, a.CustomerCode, b.Address1 + '' '' + b.Address2 + '' '' + b.Address3 + '' '' + b.Address4 as Address,
convert(varchar(20),a.DNDate,106) as InvoiceDate, 
case pso.SalesType when ''0'' then ''W'' when ''1'' then ''D'' end as Type,
a.SONo, b.CustomerName, c.CustomerName AS BillTo, a.InvoiceNo as InvNo, 
case when year(a.DueDate) = ''1900'' then '''' else convert(varchar(20),a.DueDate,106) end as DueDate, a.Remark, 
pso.SKPKNo, convert(varchar(20),pso.SODate,106) as SODate, pdo.DONo,convert(varchar(20),pdo.DODate,106) as DODate, pbpk.BPKNo,
convert(varchar(20),pbpk.BPKDate,106) as BPKDate, pso.RefferenceNo,
CASE
	WHEN a.Status = 0 THEN ''Open''
	WHEN a.Status = 1 THEN ''Printed''
	WHEN a.Status = 2 THEN ''Approved''
	WHEN a.Status = 3 THEN ''Deleted''
	WHEN a.Status = 9 THEN ''Finished''
END as Status 
FROM omTrSalesDN a
LEFT JOIN GnMstCustomer b ON b.CompanyCode = a.CompanyCode AND b.CustomerCode = a.CustomerCode
LEFT JOIN GnMstCustomer c ON c.CompanyCode = a.CompanyCode AND c.CustomerCode = a.BillTo
LEFT JOIN omTrSalesSO pso ON a.CompanyCode = pso.CompanyCode AND a.BranchCode = pso.BranchCode AND a.SONo = pso.SONo
LEFT JOIN omTrSalesDO pdo ON a.CompanyCode = pdo.CompanyCode AND a.BranchCode = pdo.BranchCode AND a.SONo = pdo.SONo
LEFT JOIN omTrSalesBPK pbpk ON a.CompanyCode = pbpk.CompanyCode AND a.BranchCode = pbpk.BranchCode AND a.SONO = pbpk.SONo AND pdo.DONo = pbpk.DONo
Where 1=1
'

if len(rtrim(@p_Status)) > 0
   set @pQuery = @pQuery + ' and a.Status = ''' + rtrim(@p_Status) + ''''

if year(@InvDateFrom) <> '1900'
   set @pQuery = @pQuery + ' and a.DNDate between ''' + convert(varchar(50),@InvDateFrom) + '''' + ' and ' + '''' + convert(varchar(50),@InvDateTo) + ''''

if len(rtrim(@NoSOFrom)) > 0
   set @pQuery = @pQuery + ' and a.SONo between ''' + rtrim(@NoSOFrom) + '''' + ' and ' + '''' + rtrim(@NoSOTo) + ''''

if len(rtrim(@CustomerFrom)) > 0
   set @pQuery = @pQuery + ' and a.CustomerCode between ''' + rtrim(@CustomerFrom) + '''' + ' and ' + '''' + rtrim(@CustomerTo) + ''''

if len(rtrim(@NoInvFrom)) > 0
   set @pQuery = @pQuery + ' and a.DNNo between ''' + rtrim(@NoInvFrom) + '''' + ' and ' + '''' + rtrim(@NoInvFrom) + ''''

if len(rtrim(@NoSKPKFrom)) > 0
   set @pQuery = @pQuery + ' and pso.SKPKNo between ''' + rtrim(@NoSKPKFrom) + '''' + ' and ' + '''' + rtrim(@NoSKPKTo) + ''''

set @pQuery = @pQuery + ' and a.CompanyCode = ''' + rtrim(@p_CompanyCode) + ''''
set @pQuery = @pQuery + ' ORDER BY a.DNNo '

print(@pQuery)
exec(@pQuery)
END
--------------------------------------------------- BATAS ----------------------------------------------------------
GO

if object_id('sp_InquiryDeliveryOrder') is not null
	drop procedure sp_InquiryDeliveryOrder
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- =============================================
CREATE procedure [dbo].[sp_InquiryDeliveryOrder] 
(
 @p_CompanyCode varchar(15),
 @p_BranchCode varchar(15),
 @p_Status varchar(2),
 @SODateFrom DateTime,
 @SODateTo DateTime,
 @NoSOFrom varchar(100),
 @NoSOTo varchar(100),
 @NoDOFrom varchar(100),
 @NoDOTo varchar(100)
)
AS
BEGIN

declare @pQuery varchar(max)
set @pQuery =
'

SELECT a.DONo, convert(varchar(20),a.DODate,106) as DODate, a.SONo, a.CustomerCode, b.CustomerName, 
b.CustomerName, b.Address1 + '' '' + b.Address2 + '' '' + b.Address3 + '' '' + b.Address4 as Address,
c.SupplierName AS Expedition, case pso.SalesType when ''0'' then ''W'' when ''1'' then ''D'' end as SalesType,
d.RefferenceDesc1 AS WareHouseName, e.CustomerName AS ShipTo, 
a.Remark, 
CASE
WHEN a.Status = 0 THEN ''Open''
WHEN a.Status = 1 THEN ''Printed''
WHEN a.Status = 2 THEN ''Approved''
WHEN a.Status = 3 THEN ''Canceled''
WHEN a.Status = 9 THEN ''Finished''
END as Status 
FROM omTrSalesDO a
LEFT JOIN GnMstCustomer b ON b.CompanyCode = a.CompanyCode AND b.CustomerCode = a.CustomerCode
LEFT JOIN GnMstSupplier c ON c.CompanyCode = a.CompanyCode AND c.SupplierCode = a.Expedition
LEFT JOIN OmMstRefference d ON d.CompanyCode = a.CompanyCode AND d.RefferenceCode = a.WareHouseCode AND d.RefferenceType = ''WARE''
LEFT JOIN GnMstCustomer e ON e.CompanyCode = a.CompanyCode AND e.CustomerCode = a.ShipTo
LEFT JOIN omTrSalesSO pso ON a.CompanyCode = pso.CompanyCode AND a.BranchCode = pso.BranchCode AND a.SONo = pso.SONo
Where 1=1
'

if len(rtrim(@p_Status)) > 0
   set @pQuery = @pQuery + ' and a.Status = ''' + rtrim(@p_Status) + ''''

if year(@SODateFrom) <> '1900'
   set @pQuery = @pQuery + ' and a.DODate between ''' + convert(varchar(50),@SODateFrom) + '''' + ' and ' + '''' + convert(varchar(50),@SODateTo) + ''''

if len(rtrim(@NoSOFrom)) > 0
   set @pQuery = @pQuery + ' and a.SONo between ''' + rtrim(@NoSOFrom) + '''' + ' and ' + '''' + rtrim(@NoSOTo) + ''''

if len(rtrim(@NoDOFrom)) > 0
   set @pQuery = @pQuery + ' and a.DONo between ''' + rtrim(@NoDOFrom) + '''' + ' and ' + '''' + rtrim(@NoDOTo) + ''''

set @pQuery = @pQuery + ' and a.CompanyCode = ''' + rtrim(@p_CompanyCode) + '''' + ' and a.BranchCode = ''' + rtrim(@p_BranchCode) + ''''
set @pQuery = @pQuery + ' ORDER BY a.DONo '

print(@pQuery)
exec(@pQuery)
END
--------------------------------------------------- BATAS ----------------------------------------------------------

GO

if object_id('sp_InquiryDeliveryOrderDetail') is not null
	drop procedure sp_InquiryDeliveryOrderDetail
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- =============================================
CREATE procedure [dbo].[sp_InquiryDeliveryOrderDetail] 
(
 @p_CompanyCode varchar(15),
 @p_BranchCode varchar(15),
 @NoDO varchar(15)
)
AS
BEGIN

SELECT a.SalesModelCode, a.SalesModelYear, b.SalesModelDesc, 
a.ChassisCode, Convert(varchar,a.ChassisNo) as ChassisNo, 
a.EngineCode, Convert(varchar,a.EngineNo) as EngineNo, 
a.ColourCode, c.RefferenceDesc1 as ColourName, a.Remark, a.DOSeq
FROM   omTrSalesDODetail a	   
LEFT JOIN
omMstModelYear b
ON a.CompanyCode = b.CompanyCode
AND a.SalesModelCode = b.SalesModelCode
AND a.SalesModelYear = b.SalesModelYear      
AND a.ChassisCode = b.ChassisCode	   
LEFT JOIN
omMstRefference c
ON a.CompanyCode = c.CompanyCode
AND a.ColourCode = c.RefferenceCode
AND c.RefferenceType = 'COLO'
WHERE a.CompanyCode= @p_CompanyCode 
AND a.BranchCode= @p_BranchCode  
AND a.DONo= @NoDO
ORDER BY a.ChassisNo ASC

END
GO

if object_id('sp_InquiryHPP') is not null
	drop procedure sp_InquiryHPP
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- =============================================
CREATE procedure [dbo].[sp_InquiryHPP] 
(
 @p_CompanyCode varchar(15),
 @p_BranchCode varchar(15),
 @p_Status varchar(2),
 @HPPDateFrom DateTime,
 @HPPDateTo DateTime,
 @refNoFrom varchar(100),
 @refNoTo varchar(100),
 @HPPNoFrom varchar(100),
 @HPPNoTo varchar(100),
 @PONoFrom varchar(100),
 @PONoTo varchar(100),
 @supplierCodeFrom varchar(100),
 @supplierCodeTo varchar(100)
)
AS
BEGIN

declare @pQuery varchar(max)
set @pQuery =
'
SELECT a.HPPNo, convert(varchar(20),a.HPPDate,106) as HPPDate, a.PONo, a.RefferenceInvoiceNo, convert(varchar(20),a.RefferenceInvoiceDate,106) as RefferenceInvoiceDate,
a.RefferenceFakturPajakNo, convert(varchar(20),a.RefferenceFakturPajakDate,106) as RefferenceFakturPajakDate, a.SupplierCode, 
(select z.SupplierName from gnMstSupplier z where a.CompanyCode = z.CompanyCode and
a.SupplierCode = z.SupplierCode) as SupplierName,
a.BillTo, convert(varchar(20),a.DueDate,106) as DueDate, a.Remark, CASE
WHEN a.Status = 0 THEN ''Open''
WHEN a.Status = 1 THEN ''Printed''
WHEN a.Status = 2 THEN ''Approved''
WHEN a.Status = 3 THEN ''Canceled''
WHEN a.Status = 9 THEN ''Finished''
END  AS Status
FROM omTrPurchaseHPP a
where 1=1
'

if len(rtrim(@p_Status)) > 0
   set @pQuery = @pQuery + ' and a.Status = ''' + rtrim(@p_Status) + ''''

if year(@HPPDateFrom) <> '1900'
   set @pQuery = @pQuery + ' and a.HPPDate between ''' + convert(varchar(50),@HPPDateFrom) + '''' + ' and ' + '''' + convert(varchar(50),@HPPDateTo) + ''''

if len(rtrim(@refNoFrom)) > 0
   set @pQuery = @pQuery + ' and a.RefferenceInvoiceNo between ''' + rtrim(@refNoFrom) + '''' + ' and ' + '''' + rtrim(@refNoTo) + ''''

if len(rtrim(@HPPNoFrom)) > 0
   set @pQuery = @pQuery + ' and a.HPPNo between ''' + rtrim(@HPPNoFrom) + '''' + ' and ' + '''' + rtrim(@HPPNoTo) + ''''

if len(rtrim(@PONoFrom)) > 0
   set @pQuery = @pQuery + ' and a.PONo between ''' + rtrim(@PONoFrom) + '''' + ' and ' + '''' + rtrim(@PONoTo) + ''''

if len(rtrim(@supplierCodeFrom)) > 0
   set @pQuery = @pQuery + ' and a.SupplierCode between ''' + rtrim(@supplierCodeFrom) + '''' + ' and ' + '''' + rtrim(@supplierCodeTo) + ''''

set @pQuery = @pQuery + ' and a.CompanyCode = ''' + rtrim(@p_CompanyCode) + '''' + ' and a.BranchCode = ''' + rtrim(@p_BranchCode) + ''''
set @pQuery = @pQuery + ' ORDER BY a.HPPNo '

print(@pQuery)
exec(@pQuery)
END
--------------------------------------------------- BATAS ----------------------------------------------------------
GO

if object_id('sp_InquiryInvoice') is not null
	drop procedure sp_InquiryInvoice
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- =============================================
CREATE procedure [dbo].[sp_InquiryInvoice] 
(
 @p_CompanyCode varchar(15),
 @p_BranchCode varchar(15),
 @p_Status varchar(2),
 @InvDateFrom DateTime,
 @InvDateTo DateTime,
 @NoSOFrom varchar(100),
 @NoSOTo varchar(100),
 @CustomerFrom varchar(100),
 @CustomerTo varchar(100),
 @NoInvFrom varchar(100),
 @NoInvTo varchar(100),
 @NoSKPKFrom varchar(100),
 @NoSKPKTo varchar(100)
)
AS
BEGIN

declare @pQuery varchar(max)
set @pQuery =
'

SELECT 
a.InvoiceNo, a.CustomerCode, b.Address1 + '' '' + b.Address2 + '' '' + b.Address3 + '' '' + b.Address4 as Address,
convert(varchar(20),a.InvoiceDate,106) as InvoiceDate, 
case pso.SalesType when ''0'' then ''W'' when ''1'' then ''D'' end as SalesType,
a.SONo, b.CustomerName, c.CustomerName AS BillTo, a.FakturPajakNo, isnull(pdn.DNNo,'''') as DNNo,
case when year(a.FakturPajakDate) = ''1900'' then '''' else convert(varchar(20),a.FakturPajakDate,106) end as FakturPajakDate, 
case when year(a.DueDate) = ''1900'' then '''' else convert(varchar(20),a.DueDate,106) end as DueDate, a.Remark, 
pso.SKPKNo, convert(varchar(20),pso.SODate,106) as SODate, pdo.DONo,convert(varchar(20),pdo.DODate,106) as DODate, pbpk.BPKNo,
convert(varchar(20),pbpk.BPKDate,106) as BPKDate, pso.RefferenceNo,
CASE
	WHEN a.Status = 0 THEN ''Open''
	WHEN a.Status = 1 THEN ''Printed''
	WHEN a.Status = 2 THEN ''Approved''
	WHEN a.Status = 3 THEN ''Deleted''
	WHEN a.Status = 9 THEN ''Finished''
	END as Status 
FROM omTrSalesInvoice a
LEFT JOIN GnMstCustomer b ON b.CompanyCode = a.CompanyCode AND b.CustomerCode = a.CustomerCode
LEFT JOIN GnMstCustomer c ON c.CompanyCode = a.CompanyCode AND c.CustomerCode = a.BillTo
LEFT JOIN omTrSalesSO pso ON a.CompanyCode = pso.CompanyCode AND a.BranchCode = pso.BranchCode AND a.SONo = pso.SONo
LEFT JOIN omTrSalesDO pdo ON a.CompanyCode = pdo.CompanyCode AND a.BranchCode = pdo.BranchCode AND a.SONo = pdo.SONo
LEFT JOIN omTrSalesBPK pbpk ON a.CompanyCode = pbpk.CompanyCode AND a.BranchCode = pbpk.BranchCode AND a.SONO = pbpk.SONo AND pdo.DONo = pbpk.DONo
LEFT JOIN omTrSalesDN pdn ON a.CompanyCode = pdn.CompanyCode AND a.BranchCode = pdn.BranchCode AND a.InvoiceNo = pdn.InvoiceNo
Where 1=1
'

if len(rtrim(@p_Status)) > 0
   set @pQuery = @pQuery + ' and a.Status = ''' + rtrim(@p_Status) + ''''

if year(@InvDateFrom) <> '1900'
   set @pQuery = @pQuery + ' and a.InvoiceDate between ''' + convert(varchar(50),@InvDateFrom) + '''' + ' and ' + '''' + convert(varchar(50),@InvDateTo) + ''''

if len(rtrim(@NoSOFrom)) > 0
   set @pQuery = @pQuery + ' and a.SONo between ''' + rtrim(@NoSOFrom) + '''' + ' and ' + '''' + rtrim(@NoSOTo) + ''''

if len(rtrim(@CustomerFrom)) > 0
   set @pQuery = @pQuery + ' and a.CustomerCode between ''' + rtrim(@CustomerFrom) + '''' + ' and ' + '''' + rtrim(@CustomerTo) + ''''

if len(rtrim(@NoInvFrom)) > 0
   set @pQuery = @pQuery + ' and a.InvoiceNo between ''' + rtrim(@NoInvFrom) + '''' + ' and ' + '''' + rtrim(@NoInvTo) + ''''

if len(rtrim(@NoSKPKFrom)) > 0
   set @pQuery = @pQuery + ' and pso.SKPKNo between ''' + rtrim(@NoSKPKFrom) + '''' + ' and ' + '''' + rtrim(@NoSKPKTo) + ''''

set @pQuery = @pQuery + ' and a.CompanyCode = ''' + rtrim(@p_CompanyCode) + '''' + ' and a.BranchCode = ''' + rtrim(@p_BranchCode) + ''''
set @pQuery = @pQuery + ' ORDER BY a.InvoiceNo '

print(@pQuery)
exec(@pQuery)
END
--------------------------------------------------- BATAS ----------------------------------------------------------
GO

if object_id('sp_InquiryKaroseri') is not null
	drop procedure sp_InquiryKaroseri
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- =============================================
CREATE procedure [dbo].[sp_InquiryKaroseri] 
(
 @p_CompanyCode varchar(15),
 @p_BranchCode varchar(15),
 @p_Status varchar(2),
 @KaroseriDateFrom DateTime,
 @KaroseriDateTo DateTime,
 @SalesModelCodeOldFrom varchar(100),
 @SalesModelCodeOldTo varchar(100),
 @SalesModelYearFrom varchar(100),
 @SalesModelYearTo varchar(100),
 @NoSPKKaroseriFrom varchar(100),
 @NoSPKKaroseriTo varchar(100),
 @SupplierCodeFrom varchar(100),
 @SupplierCodeTo varchar(100)
)
AS
BEGIN

declare @pQuery varchar(max)
set @pQuery =
'

SELECT 
a.KaroseriSPKNo, 
convert(varchar(20),a.KaroseriSPKDate,106) as KaroseriSPKDate, 
a.RefferenceNo, 
case when year(a.RefferenceDate) = ''1900'' then '''' else convert(varchar(20),a.RefferenceDate,106) end as RefferenceDate, 
a.SupplierCode,b.SupplierName, a.SalesModelCodeOld, a.SalesModelYear, a.SalesModelCodeNew, 
a.Quantity, a.DPPMaterial, a.DPPFee, a.DPPOthers, a.PPn, a.Total, a.DurationDays, 
a.Remark, CASE ISNULL(a.Status, 0) WHEN 0 THEN ''Open'' WHEN 1 THEN ''Printed'' WHEN 2 THEN ''Approved'' WHEN 3 THEN ''Canceled'' END AS Status        
FROM 
OmTrPurchaseKaroseri a
LEFT JOIN 
gnMstSupplier b 
ON a.CompanyCode = b.CompanyCode 
AND a.SupplierCode =  b.SupplierCode
Where 1=1
'

if len(rtrim(@p_Status)) > 0
   set @pQuery = @pQuery + ' and a.Status = ''' + rtrim(@p_Status) + ''''

if year(@KaroseriDateFrom) <> '1900'
   set @pQuery = @pQuery + ' and a.KaroseriSPKDate between ''' + convert(varchar(50),@KaroseriDateFrom) + '''' + ' and ' + '''' + convert(varchar(50),@KaroseriDateTo) + ''''

if len(rtrim(@SalesModelCodeOldFrom)) > 0
   set @pQuery = @pQuery + ' and a.SalesModelCodeOld between ''' + rtrim(@SalesModelCodeOldFrom) + '''' + ' and ' + '''' + rtrim(@SalesModelCodeOldFrom) + ''''

if len(rtrim(@SalesModelYearFrom)) > 0
   set @pQuery = @pQuery + ' and a.SalesModelYear between ''' + rtrim(@SalesModelYearFrom) + '''' + ' and ' + '''' + rtrim(@SalesModelYearTo) + ''''

if len(rtrim(@NoSPKKaroseriFrom)) > 0
   set @pQuery = @pQuery + ' and a.KaroseriSPKNo between ''' + rtrim(@NoSPKKaroseriFrom) + '''' + ' and ' + '''' + rtrim(@NoSPKKaroseriTo) + ''''

if len(rtrim(@SupplierCodeFrom)) > 0
   set @pQuery = @pQuery + ' and a.SupplierCode between ''' + rtrim(@SupplierCodeFrom) + '''' + ' and ' + '''' + rtrim(@SupplierCodeTo) + ''''

set @pQuery = @pQuery + ' and a.CompanyCode = ''' + rtrim(@p_CompanyCode) + ''''+ ' and a.BranchCode = ''' + rtrim(@p_BranchCode) + ''''
set @pQuery = @pQuery + ' ORDER BY a.KaroseriSPKNo '

print(@pQuery)
exec(@pQuery)
END
--------------------------------------------------- BATAS ----------------------------------------------------------
GO

if object_id('sp_InquiryKaroseriTerima') is not null
	drop procedure sp_InquiryKaroseriTerima
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- =============================================
CREATE procedure [dbo].[sp_InquiryKaroseriTerima] 
(
 @p_CompanyCode varchar(15),
 @p_BranchCode varchar(15),
 @p_Status varchar(2),
 @KaroseriDateFrom DateTime,
 @KaroseriDateTo DateTime,
 @SalesModelCodeOldFrom varchar(100),
 @SalesModelCodeOldTo varchar(100),
 @SalesModelYearFrom varchar(100),
 @SalesModelYearTo varchar(100),
 @NoSPKKaroseriFrom varchar(100),
 @NoSPKKaroseriTo varchar(100),
 @NoKaroseriTerimaFrom varchar(100),
 @NoKaroseriTerimaTo varchar(100)
)
AS
BEGIN

declare @pQuery varchar(max)
set @pQuery =
'

SELECT 
a.KaroseriTerimaNo, 
convert(varchar(20),a.KaroseriTerimaDate,106) as KaroseriTerimaDate,
a.RefferenceInvoiceNo,
case when year(a.RefferenceInvoiceDate) = ''1900'' then '''' else convert(varchar(20),a.RefferenceInvoiceDate,106) end as RefferenceInvoiceDate,
a.RefferenceFakturPajakNo, 
case when year(a.RefferenceFakturPajakDate) = ''1900'' then '''' else convert(varchar(20),a.RefferenceFakturPajakDate,106) end  as RefferenceFakturPajakDate,
b.SupplierName, a.SalesModelCodeOld, a.SalesModelYear, a.SalesModelCodeNew, a.Quantity, 
a.DPPMaterial, a.DPPFee, a.DPPOthers, a.PPn, a.PPh, a.Total, case when year(a.DueDate) = ''1900'' then '''' else convert(varchar(20),a.DueDate,106) end as DueDate, a.Remark, 
CASE ISNULL(a.Status, 0) WHEN 0 THEN ''Open'' WHEN 1 THEN ''Printed'' WHEN 2 THEN ''Closed'' WHEN 3 THEN ''Canceled'' WHEN 9 THEN ''Finished'' END AS Status
FROM 
OmTrPurchaseKaroseriTerima a
LEFT JOIN 
gnMstSupplier b 
ON a.CompanyCode = b.CompanyCode 
AND a.SupplierCode =  b.SupplierCode  
Where 1=1
'

if len(rtrim(@p_Status)) > 0
   set @pQuery = @pQuery + ' and a.Status = ''' + rtrim(@p_Status) + ''''

if year(@KaroseriDateFrom) <> '1900'
   set @pQuery = @pQuery + ' and a.KaroseriSPKDate between ''' + convert(varchar(50),@KaroseriDateFrom) + '''' + ' and ' + '''' + convert(varchar(50),@KaroseriDateTo) + ''''

if len(rtrim(@SalesModelCodeOldFrom)) > 0
   set @pQuery = @pQuery + ' and a.SalesModelCodeOld between ''' + rtrim(@SalesModelCodeOldFrom) + '''' + ' and ' + '''' + rtrim(@SalesModelCodeOldFrom) + ''''

if len(rtrim(@SalesModelYearFrom)) > 0
   set @pQuery = @pQuery + ' and a.SalesModelYear between ''' + rtrim(@SalesModelYearFrom) + '''' + ' and ' + '''' + rtrim(@SalesModelYearTo) + ''''

if len(rtrim(@NoSPKKaroseriFrom)) > 0
   set @pQuery = @pQuery + ' and a.KaroseriSPKNo between ''' + rtrim(@NoSPKKaroseriFrom) + '''' + ' and ' + '''' + rtrim(@NoSPKKaroseriTo) + ''''

if len(rtrim(@NoKaroseriTerimaFrom)) > 0
   set @pQuery = @pQuery + ' and a.KaroseriTerimaNo between ''' + rtrim(@NoKaroseriTerimaFrom) + '''' + ' and ' + '''' + rtrim(@NoKaroseriTerimaTo) + ''''

set @pQuery = @pQuery + ' and a.CompanyCode = ''' + rtrim(@p_CompanyCode) + ''''+ ' and a.BranchCode = ''' + rtrim(@p_BranchCode) + ''''
set @pQuery = @pQuery + ' ORDER BY a.KaroseriTerimaNo '

print(@pQuery)
exec(@pQuery)
END
--------------------------------------------------- BATAS ----------------------------------------------------------
GO

if object_id('sp_InquiryPerlengkapanAdjustent') is not null
	drop procedure sp_InquiryPerlengkapanAdjustent
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- =============================================
CREATE procedure [dbo].[sp_InquiryPerlengkapanAdjustent] 
(
 @p_CompanyCode varchar(15),
 @p_BranchCode varchar(15),
 @p_Status varchar(2),
 @PerlengkapanDateFrom DateTime,
 @PerlengkapanDateTo DateTime,
 @NoReffFrom varchar(100),
 @NoReffTo varchar(100),
 @NoAdjustmentFrom varchar(100),
 @NoAdjustmentTo varchar(100)
)
AS
BEGIN

declare @pQuery varchar(max)
set @pQuery =
'

SELECT 
a.AdjustmentNo, 
convert(varchar(20),a.AdjustmentNo,106) as AdjustmentDate,
a.RefferenceNo,
case when year(a.RefferenceDate) = ''1900'' then '''' else convert(varchar(20),a.RefferenceDate,106) end as RefferenceDate,
CASE a.PerlengkapanType WHEN 1 THEN ''BPU'' WHEN 2 THEN ''Transfer'' WHEN 3 THEN ''Return'' ELSE a.PerlengkapanType END AS PerlengkapanType , a.Remark,
CASE ISNULL(a.Status, 0) WHEN 0 THEN ''Open'' WHEN 1 THEN ''Printed'' WHEN 2 THEN ''Deleted'' WHEN 3 THEN ''Canceled'' WHEN 9 THEN ''Finished'' END AS Status
FROM 
OmTrPurchasePerlengkapanAdjustment a
where 1=1
'

if len(rtrim(@p_Status)) > 0
   set @pQuery = @pQuery + ' and a.Status = ''' + rtrim(@p_Status) + ''''

if year(@PerlengkapanDateFrom) <> '1900'
   set @pQuery = @pQuery + ' and a.PerlengkapanDate between ''' + convert(varchar(50),@PerlengkapanDateFrom) + '''' + ' and ' + '''' + convert(varchar(50),@PerlengkapanDateTo) + ''''

if len(rtrim(@NoReffFrom)) > 0
   set @pQuery = @pQuery + ' and a.RefferenceNo between ''' + rtrim(@NoReffFrom) + '''' + ' and ' + '''' + rtrim(@NoReffTo) + ''''

if len(rtrim(@NoAdjustmentFrom)) > 0
   set @pQuery = @pQuery + ' and a.AdjustmentNo between ''' + rtrim(@NoAdjustmentFrom) + '''' + ' and ' + '''' + rtrim(@NoAdjustmentTo) + ''''

set @pQuery = @pQuery + ' and a.CompanyCode = ''' + rtrim(@p_CompanyCode) + ''''+ ' and a.BranchCode = ''' + rtrim(@p_BranchCode) + ''''
set @pQuery = @pQuery + ' ORDER BY a.AdjustmentNo '

print(@pQuery)
exec(@pQuery)
END
--------------------------------------------------- BATAS ----------------------------------------------------------
GO

if object_id('sp_InquiryPerlengkapanIn') is not null
	drop procedure sp_InquiryPerlengkapanIn
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- =============================================
CREATE procedure [dbo].[sp_InquiryPerlengkapanIn] 
(
 @p_CompanyCode varchar(15),
 @p_BranchCode varchar(15),
 @p_Status varchar(2),
 @PerlengkapanType varchar(2),
 @PerlengkapanDateFrom DateTime,
 @PerlengkapanDateTo DateTime,
 @NoReffFrom varchar(100),
 @NoReffTo varchar(100),
 @NoPerlengkapanFrom varchar(100),
 @NoPerlengkapanTo varchar(100)
)
AS
BEGIN

declare @pQuery varchar(max)
set @pQuery =
'

SELECT 
a.PerlengkapanNo, 
convert(varchar(20),a.PerlengkapanDate,106) as PerlengkapanDate,
a.RefferenceNo,
case when year(a.RefferenceDate) = ''1900'' then '''' else convert(varchar(20),a.RefferenceDate,106) end as RefferenceDate,
CASE a.PerlengkapanType WHEN 1 THEN ''BPU'' WHEN 2 THEN ''Transfer'' WHEN 3 THEN ''Return'' ELSE a.PerlengkapanType END AS PerlengkapanType , 
a.SourceDoc, a.Remark, 
CASE ISNULL(a.Status, 0) WHEN 0 THEN ''Open'' WHEN 1 THEN ''Printed'' WHEN 2 THEN ''Deleted'' WHEN 3 THEN ''Canceled'' WHEN 9 THEN ''Finished'' END AS Status
FROM 
OmTrPurchasePerlengkapanIn a
where 1=1
'

if len(rtrim(@p_Status)) > 0
   set @pQuery = @pQuery + ' and a.Status = ''' + rtrim(@p_Status) + ''''

if len(rtrim(@PerlengkapanType)) > 0
   set @pQuery = @pQuery + ' and a.PerlengkapanType = ''' + rtrim(@PerlengkapanType) + ''''

if year(@PerlengkapanDateFrom) <> '1900'
   set @pQuery = @pQuery + ' and a.PerlengkapanDate between ''' + convert(varchar(50),@PerlengkapanDateFrom) + '''' + ' and ' + '''' + convert(varchar(50),@PerlengkapanDateTo) + ''''

if len(rtrim(@NoReffFrom)) > 0
   set @pQuery = @pQuery + ' and a.RefferenceNo between ''' + rtrim(@NoReffFrom) + '''' + ' and ' + '''' + rtrim(@NoReffTo) + ''''

if len(rtrim(@NoPerlengkapanFrom)) > 0
   set @pQuery = @pQuery + ' and a.PerlengkapanNo between ''' + rtrim(@NoPerlengkapanFrom) + '''' + ' and ' + '''' + rtrim(@NoPerlengkapanTo) + ''''

set @pQuery = @pQuery + ' and a.CompanyCode = ''' + rtrim(@p_CompanyCode) + ''''+ ' and a.BranchCode = ''' + rtrim(@p_BranchCode) + ''''
set @pQuery = @pQuery + ' ORDER BY a.PerlengkapanNo '

print(@pQuery)
exec(@pQuery)
END
--------------------------------------------------- BATAS ----------------------------------------------------------
GO

if object_id('sp_InquiryPerlengkapanOut') is not null
	drop procedure sp_InquiryPerlengkapanOut
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- =============================================
CREATE procedure [dbo].[sp_InquiryPerlengkapanOut] 
(
 @p_CompanyCode varchar(15),
 @p_BranchCode varchar(15),
 @p_Status varchar(2),
 @PerlengkapanType varchar(2),
 @PerlengkapanDateFrom DateTime,
 @PerlengkapanDateTo DateTime,
 @NoPerlengkapanFrom varchar(100),
 @NoPerlengkapanTo varchar(100)
)
AS
BEGIN

declare @pQuery varchar(max)
set @pQuery =
'

SELECT 
a.PerlengkapanNo, a.CustomerCode, b.Address1 + '' '' + b.Address2 + '' '' + b.Address3 + '' '' + b.Address4 as Address,
convert(varchar(20),a.PerlengkapanDate,106) as PerlengkapanDate, 
a.ReferenceNo, 
case when year(a.ReferenceDate) = ''1900'' then '''' else convert(varchar(20),a.ReferenceDate,106) end as ReferenceDate, 
a.SourceDoc, b.CustomerName, CASE ISNULL(PerlengkapanType, ''0'') WHEN ''1'' THEN ''BPK'' WHEN ''2'' THEN ''TRANSFER'' WHEN 3 THEN ''RETURN'' ELSE ''Unknown Type'' END AS PerlengkapanType, 
a.Remark, 
CASE
	WHEN a.Status = 0 THEN ''Open''
	WHEN a.Status = 1 THEN ''Printed''
	WHEN a.Status = 2 THEN ''Closed''
	WHEN a.Status = 3 THEN ''Canceled''
	WHEN a.Status = 9 THEN ''Finished''
	END as Status 
FROM 
	omTrSalesPerlengkapanOut a
LEFT JOIN 
	GnMstCustomer b ON b.CompanyCode = a.CompanyCode AND b.CustomerCode = a.CustomerCode
where 1=1
'

if len(rtrim(@p_Status)) > 0
   set @pQuery = @pQuery + ' and a.Status = ''' + rtrim(@p_Status) + ''''

if len(rtrim(@PerlengkapanType)) > 0
   set @pQuery = @pQuery + ' and a.PerlengkapanType = ''' + rtrim(@PerlengkapanType) + ''''

if year(@PerlengkapanDateFrom) <> '1900'
   set @pQuery = @pQuery + ' and a.PerlengkapanDate between ''' + convert(varchar(50),@PerlengkapanDateFrom) + '''' + ' and ' + '''' + convert(varchar(50),@PerlengkapanDateTo) + ''''

if len(rtrim(@NoPerlengkapanFrom)) > 0
   set @pQuery = @pQuery + ' and a.PerlengkapanNo between ''' + rtrim(@NoPerlengkapanFrom) + '''' + ' and ' + '''' + rtrim(@NoPerlengkapanTo) + ''''

set @pQuery = @pQuery + ' and a.CompanyCode = ''' + rtrim(@p_CompanyCode) + '''' + ' and a.BranchCode = ''' + rtrim(@p_BranchCode) + ''''
set @pQuery = @pQuery + ' ORDER BY a.PerlengkapanNo '

print(@pQuery)
exec(@pQuery)
END
--------------------------------------------------- BATAS ----------------------------------------------------------
GO

if object_id('sp_InquiryPurchaseOrder') is not null
	drop procedure sp_InquiryPurchaseOrder
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- =============================================
CREATE procedure [dbo].[sp_InquiryPurchaseOrder] 
(
 @p_CompanyCode varchar(15),
 @p_BranchCode varchar(15),
 @p_Status varchar(2),
 @PODateFrom DateTime,
 @PODateTo DateTime,
 @refNoFrom varchar(100),
 @refNoTo varchar(100),
 @PONoFrom varchar(100),
 @PONoTo varchar(100),
 @supplierCodeFrom varchar(100),
 @supplierCodeTo varchar(100)
)
AS
BEGIN

declare @pQuery varchar(max)
set @pQuery =
'
SELECT 
a.PONo, 
 case Year(a.PODate) when (1900) then null else convert(varchar(20),a.PODate,106) end as PODate, 
a.RefferenceNo, 
	case Year(a.RefferenceDate) when (1900) then null else convert(varchar(20),a.RefferenceDate,106) end as RefferenceDate, 
a.SupplierCode, ( SELECT sup.SupplierName FROM gnMstSupplier sup 
WHERE sup.SupplierCode = a.SupplierCode AND a.CompanyCode = sup.CompanyCode) AS SupplierName, 
a.BillTo, a.ShipTo, a.Remark, 
CASE WHEN a.Status = 0 THEN ''Open''
WHEN a.Status = 1 THEN ''Printed''
WHEN a.Status = 2 THEN ''Approved''
WHEN a.Status = 3 THEN ''Canceled''
WHEN a.Status = 9 THEN ''Finished''
END  AS Status
FROM 
omTrPurchasePO a
where 1=1
'

if len(rtrim(@p_Status)) > 0
   set @pQuery = @pQuery + ' and a.Status = ''' + rtrim(@p_Status) + ''''

if year(@PODateFrom) <> '1900'
   set @pQuery = @pQuery + ' and a.PODate between ''' + convert(varchar(50),@PODateFrom) + '''' + ' and ' + '''' + convert(varchar(50),@PODateTo) + ''''

if len(rtrim(@supplierCodeFrom)) > 0
   set @pQuery = @pQuery + ' and a.SupplierCode between ''' + rtrim(@supplierCodeFrom) + '''' + ' and ' + '''' + rtrim(@supplierCodeTo) + ''''

if len(rtrim(@refNoFrom)) > 0
   set @pQuery = @pQuery + ' and a.RefferenceNo between ''' + rtrim(@refNoFrom) + '''' + ' and ' + '''' + rtrim(@refNoTo) + ''''

if len(rtrim(@PONoFrom)) > 0
   set @pQuery = @pQuery + ' and a.PONo between ''' + rtrim(@PONoFrom) + '''' + ' and ' + '''' + rtrim(@PONoTo) + ''''

set @pQuery = @pQuery + ' and a.CompanyCode = ''' + rtrim(@p_CompanyCode) + '''' + ' and a.BranchCode = ''' + rtrim(@p_BranchCode) + ''''
set @pQuery = @pQuery + ' ORDER BY a.PONo '

print(@pQuery)
exec(@pQuery)
END
--------------------------------------------------- BATAS ----------------------------------------------------------


GO

if object_id('sp_InquiryPurchaseReturn') is not null
	drop procedure sp_InquiryPurchaseReturn
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- =============================================
CREATE procedure [dbo].[sp_InquiryPurchaseReturn] 
(
 @p_CompanyCode varchar(15),
 @p_BranchCode varchar(15),
 @p_Status varchar(2),
 @ReturnDateFrom DateTime,
 @ReturnDateTo DateTime,
 @HPPNoFrom varchar(100),
 @HPPNoTo varchar(100),
 @NoReturnFrom varchar(100),
 @NoReturnTo varchar(100)
)
AS
BEGIN

declare @pQuery varchar(max)
set @pQuery =
'
SELECT DISTINCT
a.ReturnNo, 
convert(varchar(20),a.ReturnDate,106) as ReturnDate, 
a.RefferenceNo, 
case when year(a.RefferenceDate) = ''1900'' then '''' else convert(varchar(20),a.RefferenceDate,106) end as RefferenceDate, 
a.HPPNo, a.RefferenceFakturPajakNo, a.Remark, 
CASE WHEN a.Status = 0 THEN ''Open''
WHEN a.Status = 1 THEN ''Printed''
WHEN a.Status = 2 THEN ''Approved''
WHEN a.Status = 3 THEN ''Canceled''
WHEN a.Status = 9 THEN ''Finished''
END  AS Status
FROM 
omTrPurchaseReturn a
where 1=1
'

if len(rtrim(@p_Status)) > 0
   set @pQuery = @pQuery + ' and a.Status = ''' + rtrim(@p_Status) + ''''

if year(@ReturnDateFrom) <> '1900'
   set @pQuery = @pQuery + ' and a.ReturnDate between ''' + convert(varchar(50),@ReturnDateFrom) + '''' + ' and ' + '''' + convert(varchar(50),@ReturnDateTo) + ''''

if len(rtrim(@HPPNoFrom)) > 0
   set @pQuery = @pQuery + ' and a.HPPNo between ''' + rtrim(@HPPNoFrom) + '''' + ' and ' + '''' + rtrim(@HPPNoTo) + ''''

if len(rtrim(@NoReturnFrom)) > 0
   set @pQuery = @pQuery + ' and a.ReturnNo between ''' + rtrim(@NoReturnFrom) + '''' + ' and ' + '''' + rtrim(@NoReturnTo) + ''''

set @pQuery = @pQuery + ' and a.CompanyCode = ''' + rtrim(@p_CompanyCode) + '''' + ' and a.BranchCode = ''' + rtrim(@p_BranchCode) + ''''
set @pQuery = @pQuery + ' ORDER BY a.ReturnNo '

print(@pQuery)
exec(@pQuery)
END
--------------------------------------------------- BATAS ----------------------------------------------------------
GO

if object_id('sp_InquirySalesOrder') is not null
	drop procedure sp_InquirySalesOrder
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- =============================================
CREATE procedure [dbo].[sp_InquirySalesOrder] 
(
 @p_CompanyCode varchar(15),
 @p_BranchCode varchar(15),
 @p_Status varchar(2),
 @SODateFrom DateTime,
 @SODateTo DateTime,
 @NoSOFrom varchar(100),
 @NoSOTo varchar(100),
 @CustomerCodeFrom varchar(100),
 @CustomerCodeTo varchar(100),
 @SalesmanFrom varchar(100),
 @SalesmanTo varchar(100)
)
AS
BEGIN

declare @pQuery varchar(max)
set @pQuery =
'

SELECT 
a.SONo, 
convert(varchar(20),a.SODate,106) as SODate, 
a.RefferenceNo, a.CustomerCode,  case a.SalesType when ''0'' then ''W'' when ''1'' then ''D'' end as SalesType,
case when year(a.RefferenceDate) = ''1900'' then '''' else convert(varchar(20),a.RefferenceDate,106) end as RefferenceDate, 
b.CustomerName, b.Address1 + '' '' + b.Address2 + '' '' + b.Address3 + '' '' + b.Address4 as Address, 
c.EmployeeName AS SalesmanName, d.RefferenceDesc1 AS WareHouseName, 
e.CustomerName AS BillTo, f.CustomerName AS ShipTo, g.RefferenceDesc1 AS GroupPriceName, a.Remark, 
CASE
WHEN a.Status = 0 THEN ''Open''
WHEN a.Status = 1 THEN ''Printed''
WHEN a.Status = 2 THEN ''Approved''
WHEN a.Status = 3 THEN ''Canceled''
WHEN a.Status = 4 THEN ''Rejected''
WHEN a.Status = 9 THEN ''Finished''
END as Status 
FROM 
	omTrSalesSO a
LEFT JOIN 
	GnMstCustomer b ON b.CompanyCode = a.CompanyCode AND b.CustomerCode = a.CustomerCode
LEFT JOIN 
	GnMstEmployee c ON c.CompanyCode = a.CompanyCode AND c.BranchCode = a.BranchCode AND c.EmployeeID = a.Salesman
LEFT JOIN 
	OmMstRefference d ON d.CompanyCode = a.CompanyCode AND d.RefferenceCode = a.WareHouseCode AND d.RefferenceType = ''WARE''
LEFT JOIN 
	GnMstCustomer e ON e.CompanyCode = a.CompanyCode AND e.CustomerCode = a.BillTo
LEFT JOIN 
	GnMstCustomer f ON f.CompanyCode = a.CompanyCode AND f.CustomerCode = a.ShipTo
LEFT JOIN 
	OmMstRefference g ON g.CompanyCode = a.CompanyCode AND g.RefferenceCode = a.GroupPriceCode AND g.RefferenceType = ''GRPR''
Where 1=1
'

if len(rtrim(@p_Status)) > 0
   set @pQuery = @pQuery + ' and a.Status = ''' + rtrim(@p_Status) + ''''

if year(@SODateFrom) <> '1900'
   set @pQuery = @pQuery + ' and a.SODate between ''' + convert(varchar(50),@SODateFrom) + '''' + ' and ' + '''' + convert(varchar(50),@SODateTo) + ''''

if len(rtrim(@NoSOFrom)) > 0
   set @pQuery = @pQuery + ' and a.SONo between ''' + rtrim(@NoSOFrom) + '''' + ' and ' + '''' + rtrim(@NoSOTo) + ''''

if len(rtrim(@CustomerCodeFrom)) > 0
   set @pQuery = @pQuery + ' and a.CustomerCode between ''' + rtrim(@CustomerCodeFrom) + '''' + ' and ' + '''' + rtrim(@CustomerCodeTo) + ''''

if len(rtrim(@SalesmanFrom)) > 0
   set @pQuery = @pQuery + ' and a.Salesman between ''' + rtrim(@SalesmanFrom) + '''' + ' and ' + '''' + rtrim(@SalesmanTo) + ''''

set @pQuery = @pQuery + ' and a.CompanyCode = ''' + rtrim(@p_CompanyCode) + ''''+ ' and a.BranchCode = ''' + rtrim(@p_BranchCode) + ''''
set @pQuery = @pQuery + ' ORDER BY a.SONo '

print(@pQuery)
exec(@pQuery)
END
--------------------------------------------------- BATAS ----------------------------------------------------------

					
GO

if object_id('sp_InquirySalesReq') is not null
	drop procedure sp_InquirySalesReq
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- =============================================
CREATE procedure [dbo].[sp_InquirySalesReq] 
(
 @p_CompanyCode varchar(15),
 @p_BranchCode varchar(15),
 @p_Status varchar(2),
 @Jenis varchar(10),
 @PermohonanDateFrom DateTime,
 @PermohonanDateTo DateTime,
 @NoPermohonanFrom varchar(100),
 @NoPermohonanTo varchar(100),
 @CustomerCode varchar(100),
 @CustomerCodeTo varchar(100)
)
AS
BEGIN

declare @pQuery varchar(max)
set @pQuery =
'

SELECT a.ReqNo,convert(varchar(20),a.ReqDate,106) as ReqDate, a.ReffNo, case when year(a.ReffDate) = ''1900'' then '''' else convert(varchar(20),a.ReffDate,106) end as ReffDate, 
b.CustomerName AS SubDealerName, CASE ISNULL(StatusFaktur, ''0'') WHEN ''0'' THEN ''NON FAKTUR'' WHEN ''1'' THEN ''FAKTUR'' END AS StatusFaktur,
a.Remark, 
CASE
	WHEN a.Status = 0 THEN ''Open''
	WHEN a.Status = 1 THEN ''Printed''
	WHEN a.Status = 2 THEN ''Approved''
	WHEN a.Status = 3 THEN ''Deleted''
	WHEN a.Status = 9 THEN ''Finished''
	END as Status 
FROM omTrSalesReq a
LEFT JOIN gnMstCustomer b ON b.CompanyCode = a.CompanyCode AND b.CustomerCode = a.SubDealerCode
where 1=1
'

if len(rtrim(@p_Status)) > 0
   set @pQuery = @pQuery + ' and a.Status = ''' + rtrim(@p_Status) + ''''

if len(rtrim(@Jenis)) > 0
   set @pQuery = @pQuery + ' and a.StatusFaktur = ''' + rtrim(@Jenis) + ''''

if year(@PermohonanDateFrom) <> '1900'
   set @pQuery = @pQuery + ' and a.ReqDate between ''' + convert(varchar(50),@PermohonanDateFrom) + '''' + ' and ' + '''' + convert(varchar(50),@PermohonanDateTo) + ''''

if len(rtrim(@NoPermohonanFrom)) > 0
   set @pQuery = @pQuery + ' and a.ReqNo between ''' + rtrim(@NoPermohonanFrom) + '''' + ' and ' + '''' + rtrim(@NoPermohonanTO) + ''''

if len(rtrim(@CustomerCode)) > 0
   set @pQuery = @pQuery + ' and a.SubDealerCode between ''' + rtrim(@CustomerCode) + '''' + ' and ' + '''' + rtrim(@CustomerCodeTo) + ''''

set @pQuery = @pQuery + ' and a.CompanyCode = ''' + rtrim(@p_CompanyCode) + '''' + ' and a.BranchCode = ''' + rtrim(@p_BranchCode) + ''''
set @pQuery = @pQuery + ' ORDER BY a.ReqNo '

print(@pQuery)
exec(@pQuery)
END
--------------------------------------------------- BATAS ----------------------------------------------------------
GO

if object_id('sp_InquirySalesReturn') is not null
	drop procedure sp_InquirySalesReturn
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- =============================================
CREATE procedure [dbo].[sp_InquirySalesReturn] 
(
 @p_CompanyCode varchar(15),
 @p_BranchCode varchar(15),
 @p_Status varchar(2),
 @ReturnDateFrom DateTime,
 @ReturnDateTo DateTime,
 @NoInvFrom varchar(100),
 @NoInvTo varchar(100),
 @NoReturnFrom varchar(100),
 @NoReturnTo varchar(100),
 @CustomerFrom varchar(100),
 @CustomerTo varchar(100),
 @WarehouseFrom varchar(100),
 @WarehouseFromTo varchar(100)
)
AS
BEGIN

declare @pQuery varchar(max)
set @pQuery =
'

SELECT 
a.ReturnNo, b.CustomerCode, b.Address1 + '' '' + b.Address2 + '' '' + b.Address3 + '' '' + b.Address4 as Address,
convert(varchar(20),a.ReturnDate,106) as ReturnDate, 
a.ReferenceNo, 
case when year(a.ReferenceDate) = ''1900'' then '''' else convert(varchar(20),a.ReferenceDate,106) end as ReferenceDate, 
a.InvoiceNo, 
case when year(a.InvoiceDate) = ''1900'' then '''' else convert(varchar(20),a.InvoiceDate,106) end as InvoiceDate, 
b.CustomerName, a.FakturPajakNo, 
case when year(a.FakturPajakDate) = ''1900'' then '''' else convert(varchar(20),a.FakturPajakDate,106) end as FakturPajakDate, 
c.RefferenceDesc1 AS WareHouseName, a.Remark, 
CASE
WHEN a.Status = 0 THEN ''Open''
WHEN a.Status = 1 THEN ''Printed''
WHEN a.Status = 2 THEN ''Approved''
WHEN a.Status = 3 THEN ''Deleted''
WHEN a.Status = 9 THEN ''Finished''
END as Status 
FROM 
	omTrSalesReturn a
LEFT JOIN 
	GnMstCustomer b ON b.CompanyCode = a.CompanyCode AND b.CustomerCode = a.CustomerCode
LEFT JOIN 
	OmMstRefference c ON c.CompanyCode = a.CompanyCode AND c.RefferenceCode = a.WareHouseCode AND c.RefferenceType = ''WARE''
Where 1=1
'

if len(rtrim(@p_Status)) > 0
   set @pQuery = @pQuery + ' and a.Status = ''' + rtrim(@p_Status) + ''''

if year(@ReturnDateFrom) <> '1900'
   set @pQuery = @pQuery + ' and a.ReturnDate between ''' + convert(varchar(50),@ReturnDateFrom) + '''' + ' and ' + '''' + convert(varchar(50),@ReturnDateTo) + ''''

if len(rtrim(@NoInvFrom)) > 0
   set @pQuery = @pQuery + ' and a.InvoiceNo between ''' + rtrim(@NoInvFrom) + '''' + ' and ' + '''' + rtrim(@NoInvTo) + ''''

if len(rtrim(@NoReturnFrom)) > 0
   set @pQuery = @pQuery + ' and a.ReturnNo between ''' + rtrim(@NoReturnFrom) + '''' + ' and ' + '''' + rtrim(@NoReturnTo) + ''''

if len(rtrim(@CustomerFrom)) > 0
   set @pQuery = @pQuery + ' and a.CustomerCode between ''' + rtrim(@CustomerFrom) + '''' + ' and ' + '''' + rtrim(@CustomerTo) + ''''

if len(rtrim(@WarehouseFrom)) > 0
   set @pQuery = @pQuery + ' and a.WareHouseCode between ''' + rtrim(@WarehouseFrom) + '''' + ' and ' + '''' + rtrim(@WarehouseFrom) + ''''

set @pQuery = @pQuery + ' and a.CompanyCode = ''' + rtrim(@p_CompanyCode) + '''' + ' and a.BranchCode = ''' + rtrim(@p_BranchCode) + ''''
set @pQuery = @pQuery + ' ORDER BY a.InvoiceNo '

print(@pQuery)
exec(@pQuery)
END
--------------------------------------------------- BATAS ----------------------------------------------------------
GO

if object_id('sp_InquirySalesReturnDetailModel') is not null
	drop procedure sp_InquirySalesReturnDetailModel
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- =============================================
CREATE procedure [dbo].[sp_InquirySalesReturnDetailModel] 
(
 @p_CompanyCode varchar(15),
 @p_BranchCode varchar(15),
 @ReturnNo varchar(100)
)
AS
BEGIN

SELECT *, (SELECT z.SalesModelDesc
			FROM omMstModelYear z
			WHERE a.CompanyCode = z.CompanyCode
			AND a.SalesModelCode = z.SalesModelCode
			AND a.salesmodelyear = z.salesmodelyear)  AS salesModelDesc
FROM    omTrSalesReturnDetailModel a
INNER JOIN
	omTrSalesReturnVIN b
	ON a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.BPKNo = b.BPKNo
	AND a.ReturnNo = b.ReturnNo
	AND a.SalesModelCode = b.SalesModelCode
	AND a.SalesModelYear = b.SalesModelYear
WHERE a.CompanyCode = @p_CompanyCode
	AND a.BranchCode = @p_BranchCode
	AND a.ReturnNo = @ReturnNo

END
GO

if object_id('sp_InquirySPK') is not null
	drop procedure sp_InquirySPK
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- =============================================
CREATE procedure [dbo].[sp_InquirySPK] 
(
 @p_CompanyCode varchar(15),
 @p_BranchCode varchar(15),
 @p_Status varchar(2),
 @SPKDateFrom DateTime,
 @SPKDateTo DateTime,
 @NoSPKFrom varchar(100),
 @NoSPKTo varchar(100),
 @NoChassisFrom varchar(100),
 @NoChassisTo varchar(100),
 @SupplierCode varchar(100),
 @SupplierCodeTo varchar(100)
)
AS
BEGIN

declare @pQuery varchar(max)
set @pQuery =
'

SELECT 
a.SPKNo, 
convert(varchar(20),a.SPKDate,106) as SPKDate, 
a.RefferenceNo, 
case when year(a.RefferenceDate) = ''1900'' then '''' else convert(varchar(20),a.RefferenceDate,106) end as RefferenceDate, 
b.SupplierName AS SupplierName, a.Remark, 
	CASE
	WHEN a.Status = 0 THEN ''Open''
	WHEN a.Status = 1 THEN ''Printed''
	WHEN a.Status = 2 THEN ''Approved''
	WHEN a.Status = 3 THEN ''Deleted''
	WHEN a.Status = 9 THEN ''Finished''
	END as Status 
FROM 
	omTrSalesSPK a
LEFT JOIN 
	gnMstSupplier b ON b.CompanyCode = a.CompanyCode AND b.SupplierCode = a.SupplierCode
LEFT JOIN omTrSalesSPKDetail c
	ON a.CompanyCode = c.CompanyCode
	AND a.BranchCode = c.BranchCode
	AND a.SPKNo = c.SPKNo
where 1=1
'

if len(rtrim(@p_Status)) > 0
   set @pQuery = @pQuery + ' and a.Status = ''' + rtrim(@p_Status) + ''''

if year(@SPKDateFrom) <> '1900'
   set @pQuery = @pQuery + ' and a.SPKDate between ''' + convert(varchar(50),@SPKDateFrom) + '''' + ' and ' + '''' + convert(varchar(50),@SPKDateTo) + ''''

if len(rtrim(@NoSPKFrom)) > 0
   set @pQuery = @pQuery + ' and a.SPKNo between ''' + rtrim(@NoSPKFrom) + '''' + ' and ' + '''' + rtrim(@NoSPKTo) + ''''

if len(rtrim(@NoChassisFrom)) > 0
   set @pQuery = @pQuery + ' and c.ChassisNo between ''' + rtrim(@NoChassisFrom) + '''' + ' and ' + '''' + rtrim(@NoChassisTo) + ''''

if len(rtrim(@SupplierCode)) > 0
   set @pQuery = @pQuery + ' and a.SupplierCode between ''' + rtrim(@SupplierCode) + '''' + ' and ' + '''' + rtrim(@SupplierCodeTo) + ''''

set @pQuery = @pQuery + ' and a.CompanyCode = ''' + rtrim(@p_CompanyCode) + '''' + ' and a.BranchCode = ''' + rtrim(@p_BranchCode) + ''''
set @pQuery = @pQuery + ' ORDER BY a.SPKNo '

print(@pQuery)
exec(@pQuery)
END
--------------------------------------------------- BATAS ----------------------------------------------------------
GO

if object_id('sp_InquiryStokKendaraan') is not null
	drop procedure sp_InquiryStokKendaraan
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- =============================================
CREATE procedure [dbo].[sp_InquiryStokKendaraan] 
(
 @p_CompanyCode varchar(15),
 @p_BranchCode varchar(15),
 @year varchar(10),
 @Month varchar(10),
 @WarehouseCode varchar(100),
 @WarehouseCodeTo Varchar(100),
 @SalesModelCode Varchar(100),
 @SalesModelCodeTo Varchar(100),
 @SalesModelYear Varchar(100),
 @SalesModelYearTo Varchar(100),
 @ColourCode Varchar(100),
 @ColourCodeTo Varchar(100)
)
AS
BEGIN

declare @pQuery varchar(max)
set @pQuery =
'

SELECT CONVERT(Varchar,a.Year) as Year
     , CASE WHEN a.Month = 1 THEN ''Januari''
            WHEN a.Month = 2 THEN ''Februari''
            WHEN a.Month = 3 THEN ''Maret'' 
            WHEN a.Month = 4 THEN ''April'' 
            WHEN a.Month = 5 THEN ''Mei'' 
            WHEN a.Month = 6 THEN ''Juni''
            WHEN a.Month = 7 THEN ''Juli'' 
            WHEN a.Month = 8 THEN ''Agustus'' 
            WHEN a.Month = 9 THEN ''September'' 
            WHEN a.Month = 10 THEN ''Oktober''
            WHEN a.Month = 11 THEN ''November'' 
            WHEN a.Month = 12 THEN ''Desember'' 
        END AS Month
     , (select top 1 RefferenceDesc1
          from omMstRefference
         where CompanyCode = a.CompanyCode
           and RefferenceType = ''WARE''
           and RefferenceCode = a.WarehouseCode
         ) as WareHouseName
     , a.SalesModelCode
     , b.SalesModelDesc
     , CONVERT(Varchar,a.SalesModelYear) as ModelYear
     , (c.RefferenceCode + '' - '' + c.RefferenceDesc1) as ColourName
     , a.BeginningAV
     , a.QtyIn
     , a.Alocation
     , a.QtyOut
     , a.EndingAV
  FROM OmTrInventQtyVehicle a 
 INNER JOIN omMstModel b
    ON a.CompanyCode = b.CompanyCode
   AND a.SalesModelCode = b.SalesModelCode 
 INNER JOIN omMstRefference c
    ON a.CompanyCode = c.CompanyCode  
   AND a.ColourCode = c.RefferenceCode                                       
WHERE 1 = 1
'

if len(rtrim(@year)) > 0
   set @pQuery = @pQuery + ' and a.Year = ''' + rtrim(@year) + ''''

if len(rtrim(@Month)) > 0
   set @pQuery = @pQuery + ' and a.Month = ''' + rtrim(@Month) + ''''

if len(rtrim(@WarehouseCode)) > 0
   set @pQuery = @pQuery + ' and a.WarehouseCode between ''' + rtrim(@WarehouseCode) + '''' + ' and ' + '''' + rtrim(@WarehouseCodeTo) + ''''

if len(rtrim(@SalesModelCode)) > 0
   set @pQuery = @pQuery + ' and a.SalesModelCode between ''' + rtrim(@SalesModelCode) + '''' + ' and ' + '''' + rtrim(@SalesModelCodeTo) + ''''

if len(rtrim(@SalesModelYear)) > 0
   set @pQuery = @pQuery + ' and a.SalesModelYear between ''' + rtrim(@SalesModelYear) + '''' + ' and ' + '''' + rtrim(@SalesModelYearTo) + ''''

if len(rtrim(@ColourCode)) > 0
   set @pQuery = @pQuery + ' and a.ColourCode between ''' + rtrim(@ColourCode) + '''' + ' and ' + '''' + rtrim(@ColourCodeTo) + ''''

set @pQuery = @pQuery + ' and a.CompanyCode = ''' + rtrim(@p_CompanyCode) + '''' + ' and a.BranchCode = ''' + rtrim(@p_BranchCode) + ''''
set @pQuery = @pQuery + ' ORDER BY a.Year, a.Month, a.SalesModelCode '

print(@pQuery)
exec(@pQuery)
END
--------------------------------------------------- BATAS ----------------------------------------------------------
GO

if object_id('sp_InquiryStokPerlengkapan') is not null
	drop procedure sp_InquiryStokPerlengkapan
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- =============================================
CREATE procedure [dbo].[sp_InquiryStokPerlengkapan] 
(
 @p_CompanyCode varchar(15),
 @p_BranchCode varchar(15),
 @Status varchar(10),
 @Year varchar(10),
 @Month varchar(10),
 @PerlengkapanCode varchar(100),
 @PerlengkapanCodeTo Varchar(100)
)
AS
BEGIN

declare @pQuery varchar(max)
set @pQuery =
'

SELECT CONVERT(Varchar,a.Year) as Year, 
CASE WHEN a.Month = 1 THEN ''Januari'' 
	WHEN a.Month = 2 THEN ''Februari''
	WHEN a.Month = 3 THEN ''Maret'' 
	WHEN a.Month = 4 THEN ''April'' 
	WHEN a.Month = 5 THEN ''Mei'' 
	WHEN a.Month = 6 THEN ''Juni''
	WHEN a.Month = 7 THEN ''Juli'' 
	WHEN a.Month = 8 THEN ''Agustus'' 
	WHEN a.Month = 9 THEN ''September'' 
	WHEN a.Month = 10 THEN ''Oktober''
	WHEN a.Month = 11 THEN ''November'' 
	WHEN a.Month = 12 THEN ''Desember'' 
END AS Month
, a.PerlengkapanCode, b.PerlengkapanName, a.QuantityBeginning, a.QuantityIn, 
a.QuantityOut, a.QuantityEnding, CASE WHEN b.Status = 0 THEN ''Not Active'' WHEN b.Status = 1 THEN ''Active'' END AS Status 
FROM OMTrInventQtyPerlengkapan a 
INNER JOIN OMMstPerlengkapan b
ON a.CompanyCode = b.CompanyCode 
AND a.BranchCode = b.BranchCode
AND a.PerlengkapanCode = b.PerlengkapanCode

WHERE 1 = 1
'
if len(rtrim(@Status)) > 0
   set @pQuery = @pQuery + ' and b.Status = ''' + rtrim(@Status) + ''''

if len(rtrim(@Year)) > 0
   set @pQuery = @pQuery + ' and a.Year = ''' + rtrim(@year) + ''''

if len(rtrim(@Month)) > 0
   set @pQuery = @pQuery + ' and a.Month = ''' + rtrim(@Month) + ''''

if len(rtrim(@PerlengkapanCode)) > 0
   set @pQuery = @pQuery + ' and a.PerlengkapanCode between ''' + rtrim(@PerlengkapanCode) + '''' + ' and ' + '''' + rtrim(@PerlengkapanCodeTo) + ''''

set @pQuery = @pQuery + ' and a.CompanyCode = ''' + rtrim(@p_CompanyCode) + '''' + ' and a.BranchCode = ''' + rtrim(@p_BranchCode) + ''''
set @pQuery = @pQuery + ' ORDER BY a.Year, a.Month, a.PerlengkapanCode '

print(@pQuery)
exec(@pQuery)
END
--------------------------------------------------- BATAS ----------------------------------------------------------
GO

if object_id('sp_InquiryTransferIn') is not null
	drop procedure sp_InquiryTransferIn
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- =============================================
CREATE procedure [dbo].[sp_InquiryTransferIn] 
(
 @p_CompanyCode varchar(15),
 @p_BranchCode varchar(15),
 @p_Status varchar(2),
 @TransferInDateFrom DateTime,
 @TransferInDateTo DateTime,
 @NoTransferInFrom varchar(100),
 @NoTransferInTo varchar(100),
 @NoTransferOutFrom varchar(100),
 @NoTransferOutTo varchar(100),
 @GudangTujuanAwal varchar(100),
 @GudangTujuanAkhir varchar(100)
)
AS
BEGIN

declare @pQuery varchar(max)
set @pQuery =
'


SELECT 
	a.TransferInNo, 
	convert(varchar(20),a.TransferInDate,106) as TransferInDate,
	a.TransferOutNo, a.ReferenceNo, 
	case when year(a.ReferenceDate) = ''1900'' then '''' else convert(varchar(20),a.ReferenceDate,106) end as ReferenceDate, 
	c.BranchName as BranchNameFrom, 
	(select b.RefferenceDesc1  from OmTrInventTransferIn x INNER JOIN omMstRefference b 
	ON x.CompanyCode = b.CompanyCode AND x.BranchCode = a.BranchCode
	AND x.WareHouseCodeFrom = b.RefferenceCode AND x.TransferInNo= a.TransferInNo)as WareHouseNameFrom , 
	d.BranchName as BranchNameTo, 
	(select b.RefferenceDesc1  from OmTrInventTransferIn x INNER JOIN omMstRefference b 
	ON x.CompanyCode = b.CompanyCode AND x.BranchCode = a.BranchCode
	AND x.WareHouseCodeTo = b.RefferenceCode AND x.TransferInNo= a.TransferInNo)as WareHouseNameTo, 
	case when year(a.ReturnDate) = ''1900'' then '''' else convert(varchar(20),a.ReturnDate,106) end as ReturnDate, 
	a.Remark, CASE WHEN a.Status = 0 THEN ''Open'' WHEN a.Status = 1 THEN ''Printed''
	WHEN a.Status = 2 THEN ''Closed'' WHEN a.Status = 3 THEN ''Canceled'' END AS Status
FROM 
	OmTrInventTransferIn a 
INNER JOIN 
	gnMstOrganizationDtl c 
	ON a.CompanyCode = c.CompanyCode
	AND a.BranchCodeFrom = c.BranchCode
INNER JOIN 
	gnMstOrganizationDtl d 
	ON a.CompanyCode = d.CompanyCode
	AND a.BranchCodeTo = d.BranchCode
WHERE 1 = 1
'

if len(rtrim(@p_Status)) > 0
   set @pQuery = @pQuery + ' and a.Status = ''' + rtrim(@p_Status) + ''''

if year(@TransferInDateFrom) <> '1900'
   set @pQuery = @pQuery + ' and a.TransferInDate between ''' + convert(varchar(50),@TransferInDateFrom) + '''' + ' and ' + '''' + convert(varchar(50),@TransferInDateTo) + ''''

if len(rtrim(@NoTransferOutFrom)) > 0
   set @pQuery = @pQuery + ' and a.TransferOutNo between ''' + rtrim(@NoTransferOutFrom) + '''' + ' and ' + '''' + rtrim(@NoTransferOutTo) + ''''

if len(rtrim(@NoTransferInFrom)) > 0
   set @pQuery = @pQuery + ' and a.TransferInNo between ''' + rtrim(@NoTransferInFrom) + '''' + ' and ' + '''' + rtrim(@NoTransferInTo) + ''''

if len(rtrim(@GudangTujuanAwal)) > 0
   set @pQuery = @pQuery + ' and a.WareHouseCodeTo between ''' + rtrim(@GudangTujuanAwal) + '''' + ' and ' + '''' + rtrim(@GudangTujuanAkhir) + ''''

set @pQuery = @pQuery + ' and a.CompanyCode = ''' + rtrim(@p_CompanyCode) + '''' + ' and a.BranchCode = ''' + rtrim(@p_BranchCode) + ''''
set @pQuery = @pQuery + ' ORDER BY a.TransferInNo '

print(@pQuery)
exec(@pQuery)
END
--------------------------------------------------- BATAS ----------------------------------------------------------
GO

if object_id('sp_InquiryTransferInDetail') is not null
	drop procedure sp_InquiryTransferInDetail
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- =============================================
CREATE procedure [dbo].[sp_InquiryTransferInDetail] 
(
 @p_CompanyCode varchar(15),
 @p_BranchCode varchar(15),
 @TransferInNo varchar(15)
)
AS
BEGIN

SELECT a.SalesModelCode,Convert(varchar,a.SalesModelYear) as SalesModelYear,b.SalesModelDesc,
a.ChassisCode,Convert(varchar,a.ChassisNo) as ChassisNo,a.EngineCode,Convert(varchar,a.EngineNo) as EngineNo,a.ColourCode,
c.RefferenceDesc1 as ColourName,a.Remark
FROM OmTrInventTransferInDetail a
LEFT JOIN omMstModel b
ON a.CompanyCode = b.CompanyCode
AND a.SalesModelCode = b.SalesModelCode
LEFT JOIN omMstRefference c
ON a.CompanyCode = c.CompanyCode
AND a.ColourCode = c.RefferenceCode
WHERE c.RefferenceType = 'COLO'
AND a.CompanyCode = @p_CompanyCode
AND a.BranchCode = @p_BranchCode
AND a.TransferInNo = @TransferInNo
ORDER BY a.SalesModelCode,a.SalesModelYear,a.ChassisNo ASC

END
GO

if object_id('sp_InquiryTransferOut') is not null
	drop procedure sp_InquiryTransferOut
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- =============================================
CREATE procedure [dbo].[sp_InquiryTransferOut] 
(
 @p_CompanyCode varchar(15),
 @p_BranchCode varchar(15),
 @p_Status varchar(2),
 @TransferOutDateFrom DateTime,
 @TransferOutDateTo DateTime,
 @NoTransferOutFrom varchar(100),
 @NoTransferOutTo varchar(100),
 @GudangAsalAwal varchar(100),
 @GudangAsalAkhir varchar(100),
 @CabangTujuanAwal varchar(100),
 @CabangTujuanAkhir varchar(100),
 @GudangTujuanAwal varchar(100),
 @GudangTujuanAkhir varchar(100)
)
AS
BEGIN

declare @pQuery varchar(max)
set @pQuery =
'

SELECT 
	a.TransferOutNo, convert(varchar(20),a.TransferOutDate,106) as TransferOutDate,
	a.ReferenceNo, case when year(a.ReferenceDate) = ''1900'' then '''' else convert(varchar(20),a.ReferenceDate,106) end as ReferenceDate,
	c.BranchName as BranchNameFrom, 
	(select b.RefferenceDesc1  from OmTrInventTransferOut x 
	INNER JOIN omMstRefference b 
	ON x.CompanyCode = b.CompanyCode 
	AND x.BranchCode = a.BranchCode
	AND x.WareHouseCodeFrom = b.RefferenceCode 
	AND x.TransferOutNo= a.TransferOutNo)as WareHouseNameFrom,
	d.BranchName as BranchNameTo, 
	(select b.RefferenceDesc1  from OmTrInventTransferOut x 
	INNER JOIN omMstRefference b 
	ON x.CompanyCode = b.CompanyCode 
	AND x.BranchCode = a.BranchCode
	AND x.WareHouseCodeTo = b.RefferenceCode 
	AND x.TransferOutNo= a.TransferOutNo)as WareHouseNameTo,
	case when year(a.ReturnDate) = ''1900'' then '''' else convert(varchar(20),a.ReturnDate,106) end as ReturnDate,
	a.Remark, CASE WHEN a.Status = 0 THEN ''Open'' WHEN a.Status = 1 THEN ''Printed''
	WHEN a.Status = 2 THEN ''Closed'' WHEN a.Status = 3 THEN ''Canceled'' END AS Status
FROM 
	OmTrInventTransferOut a 
	INNER JOIN gnMstOrganizationDtl c 
	ON a.CompanyCode = c.CompanyCode 
	AND a.BranchCodeFrom = c.BranchCode
	INNER JOIN gnMstOrganizationDtl d 
	ON a.CompanyCode = d.CompanyCode 
	AND a.BranchCodeTo = d.BranchCode
WHERE 1 = 1
'

if len(rtrim(@p_Status)) > 0
   set @pQuery = @pQuery + ' and a.Status = ''' + rtrim(@p_Status) + ''''

if year(@TransferOutDateFrom) <> '1900'
   set @pQuery = @pQuery + ' and a.TransferOutDate between ''' + convert(varchar(50),@TransferOutDateFrom) + '''' + ' and ' + '''' + convert(varchar(50),@TransferOutDateTo) + ''''

if len(rtrim(@NoTransferOutFrom)) > 0
   set @pQuery = @pQuery + ' and a.TransferOutNo between ''' + rtrim(@NoTransferOutFrom) + '''' + ' and ' + '''' + rtrim(@NoTransferOutTo) + ''''

if len(rtrim(@GudangAsalAwal)) > 0
   set @pQuery = @pQuery + ' and a.WareHouseCodeFrom between ''' + rtrim(@GudangAsalAwal) + '''' + ' and ' + '''' + rtrim(@GudangAsalAkhir) + ''''

if len(rtrim(@CabangTujuanAwal)) > 0
   set @pQuery = @pQuery + ' and a.BranchCodeTo between ''' + rtrim(@CabangTujuanAwal) + '''' + ' and ' + '''' + rtrim(@CabangTujuanAkhir) + ''''

if len(rtrim(@GudangTujuanAwal)) > 0
   set @pQuery = @pQuery + ' and a.WareHouseCodeTo between ''' + rtrim(@GudangTujuanAwal) + '''' + ' and ' + '''' + rtrim(@GudangTujuanAkhir) + ''''

set @pQuery = @pQuery + ' and a.CompanyCode = ''' + rtrim(@p_CompanyCode) + '''' + ' and a.BranchCode = ''' + rtrim(@p_BranchCode) + ''''
set @pQuery = @pQuery + ' ORDER BY a.TransferOutNo '

print(@pQuery)
exec(@pQuery)
END
--------------------------------------------------- BATAS ----------------------------------------------------------
GO

if object_id('sp_InquiryTransferOutDetail') is not null
	drop procedure sp_InquiryTransferOutDetail
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- =============================================
CREATE procedure [dbo].[sp_InquiryTransferOutDetail] 
(
 @p_CompanyCode varchar(15),
 @p_BranchCode varchar(15),
 @TransferOutNo varchar(15)
)
AS
BEGIN

SELECT Convert(varchar,a.TransferOutSeq) as TransferOutSeq,a.SalesModelCode,Convert(varchar,a.SalesModelYear) as SalesModelYear,b.SalesModelDesc,
a.ChassisCode,Convert(varchar,a.ChassisNo) as ChassisNo,a.EngineCode,Convert(varchar,a.EngineNo) as EngineNo,a.ColourCode,
c.RefferenceDesc1 as ColourName,a.Remark
FROM OmTrInventTransferOutDetail a
LEFT JOIN omMstModel b
ON a.CompanyCode = b.CompanyCode
AND a.SalesModelCode = b.SalesModelCode
LEFT JOIN omMstRefference c
ON a.CompanyCode = c.CompanyCode
AND a.ColourCode = c.RefferenceCode
WHERE c.RefferenceType = 'COLO'
AND a.CompanyCode = @p_CompanyCode
AND a.BranchCode = @p_BranchCode
AND a.TransferOutNo = @TransferOutNo
ORDER BY a.TransferOutSeq, a.SalesModelCode,a.SalesModelYear,a.ChassisNo ASC

END
GO

if object_id('sp_InqVehicleHistory4Lookup') is not null
	drop procedure sp_InqVehicleHistory4Lookup
GO
CREATE procedure [dbo].[sp_InqVehicleHistory4Lookup] (
	@CompanyCode varchar(15),
	@BranchCode varchar(15),
	@ProfitCenterCode varchar(15)
)
as
begin
	 select distinct
                     a.PoliceRegNo
                    ,a.CustomerCode
                    ,b.CustomerName
                    ,  rtrim(rtrim(a.CustomerCode) + ' - ' + rtrim(b.CustomerName)) as CustomerDesc
                    ,  rtrim(rtrim(b.Address1) + ' ' + rtrim(b.Address2) + ' ' + rtrim(b.Address3) + ' ' + rtrim(b.Address4)) as CustomerAddr
                    , b.Address1, b.Address2, b.Address3 + ' ' + b.Address4 Address3
                    ,a.DealerCode
                    ,c.CustomerName DealerName
                    , (a.CustomerCode + ' - ' + c.CustomerName) as DealerDesc
                    ,a.ChassisCode
                    ,  cast(a.ChassisNo as varchar) ChassisNo
                    ,a.EngineCode
                    ,  cast(a.EngineNo as varchar) EngineNo
                    ,a.ServiceBookNo
                    ,a.ClubCode
                    ,a.ColourCode
                    ,case a.FakturPolisiDate when ('19000101') then null else a.FakturPolisiDate end FakturPolisiDate
                    ,a.ClubNo
                    ,case a.ClubDateStart when ('19000101') then null else a.ClubDateStart end ClubDateStart
                    ,case a.ClubDateFinish when ('19000101') then null else a.ClubDateFinish end ClubDateFinish
                    ,case a.ClubSince when ('19000101') then null else a.ClubSince end ClubSince
                    , (case a.IsClubStatus when 1 then 'Aktif' else 'Tidak Aktif' end) as IsClubStatusDesc
                    ,a.IsClubStatus
                    , (case a.IsContractStatus when 1 then 'Aktif' else 'Tidak Aktif' end) as IsContractStatusDesc
                    ,a.IsActive
                    , (case a.IsActive when 1 then 'Aktif' else 'Tidak Aktif' end) as IsActiveDesc
                    ,a.BasicModel
                    ,a.TransmissionType
                    ,case a.LastServiceDate when ('19000101') then null else a.LastServiceDate end LastServiceDate
                    ,a.LastJobType
                    ,a.ChassisNo
                    ,a.ContractNo
                    ,a.ContactName
                    ,b.CityCode
                    ,f.LookUpValueName CityName
                    from svMstCustomerVehicle a
                    left join gnMstCustomer b on b.CompanyCode = a.CompanyCode 
	                    and b.CustomerCode = a.CustomerCode
                    left join gnMstCustomer c on c.CompanyCode = a.CompanyCode 
	                    and c.CustomerCode = a.DealerCode
                    left join svMstJob d on 
	                    d.CompanyCode = a.CompanyCode and
	                    d.BasicModel = a.BasicModel 	
                    inner join gnMstCustomerProfitCenter e on 
                        e.CompanyCode = a.CompanyCode and
                        e.CustomerCode = a.CustomerCode
                    left join gnMstLookupDtl f on 
                        f.CompanyCode = a.CompanyCode and
                        f.CodeID = 'CITY' and
                        f.LookUpValue = b.CityCode
                    where a.CompanyCode=@CompanyCode and a.IsActive = 1 and e.BranchCode = @BranchCode and e.ProfitCenterCode = @ProfitCenterCode
END
GO

if object_id('sp_InsertHistoryItemStatusTuning') is not null
	drop procedure sp_InsertHistoryItemStatusTuning
GO





CREATE procedure [dbo].[sp_InsertHistoryItemStatusTuning] (  

@CompanyCode varchar(10) ,
@BranchCode varchar(10),
@Year numeric(10),
@Month numeric(10),
@CreatedBy varchar(10))


as

DELETE spHstItemStatus WHERE YEAR = @Year AND MONTH = @Month and CompanyCode = @CompanyCode and BranchCode = @BranchCode
SELECT
	a.CompanyCode     
	, a.BranchCode      
	, @Year Year                                    
	, @Month Month                                   
	, a.PartNo   
	, a.WarehouseCode            
	, b.MovingCode      
	, b.ABCClass 
	, ISNULL(c.PurchasePrice, 0) PurchasePrice                           
	, ISNULL(c.CostPrice, 0) CostPrice                               
	, ISNULL(c.RetailPrice, 0) RetailPrice                             
	, ISNULL(c.RetailPriceInclTax, 0) RetailPriceInclTax                     
	, ISNULL(b.LeadTime, 0) LeadTime                                
	, ISNULL(b.OrderPointQty, 0) OrderPoint                              
	, ISNULL(b.SafetyStock, 0) SafetyStock                             
	, ISNULL(b.OrderCycle, 0) OrderCycle                             
	, CASE WHEN a.WarehouseCode BETWEEN '00' AND '96' THEN ISNULL(b.OnOrder, 0) ELSE 0 END OnOrder 
	, CASE WHEN a.WarehouseCode BETWEEN '00' AND '96' THEN ISNULL(b.InTransit, 0) ELSE 0 END InTransit                              
	, ISNULL(a.OnHand, 0) OnHand                                  
	, ISNULL(a.AllocationSP, 0) AllocationSP                           
	, ISNULL(a.AllocationSR, 0) AllocationSR                            
	, ISNULL(a.AllocationSL, 0) AllocationSL                           
	, ISNULL(a.ReservedSP, 0) ReservedSP                             
	, ISNULL(a.ReservedSR, 0) ReservedSR                             
	, ISNULL(a.ReservedSL, 0) ReservedSL                             
	, ISNULL(a.BackOrderSP, 0) BackOrderSP                            
	, ISNULL(a.BackOrderSR, 0) BackOrderSR                           
	, ISNULL(a.BackOrderSL, 0) BackOrderSL                            
	, ISNULL(b.BorrowQty, 0) BorrowQty                              
	, ISNULL(b.BorrowedQty, 0) BorrowedQty                             
	, CASE WHEN a.WarehouseCode BETWEEN '00' AND '96' THEN ISNULL(b.DemandAverage, 0) ELSE 0 END DemandAverage
	, (SELECT ISNULL(DiscPct, 0) FROM spMstItemInfo WHERE CompanyCode = a.CompanyCode AND PartNo = a.PartNo) DiscPct                                
	, CASE WHEN a.WarehouseCode BETWEEN '00' AND '96' THEN b.LastPurchaseDate ELSE '' END LastPurchaseDate       
	, CASE WHEN a.WarehouseCode BETWEEN '00' AND '96' THEN b.LastDemandDate ELSE '' END LastDemandDate        
	, CASE WHEN a.WarehouseCode BETWEEN '00' AND '96' THEN b.LastSalesDate ELSE '' END LastSalesDate         
	, b.Status 
	, b.ProductType 
	, b.PartCategory 
	, b.TypeOfGoods 
	, @CreatedBy CreatedBy       
	, GETDATE() CreatedDate
INTO #TempHstItemStatus	
FROM
	spMstItemLoc a
	LEFT JOIN spMstItems b WITH(NOWAIT, NOLOCK) ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.PartNo = b.PartNo
	LEFT JOIN spMstItemPrice c WITH(NOWAIT, NOLOCK) ON a.CompanyCode = c.CompanyCode AND a.BranchCode = c.BranchCode AND a.PartNo = c.PartNo	
WHERE
	a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode

INSERT INTO SpHstItemStatus SELECT * FROM #TempHstItemStatus
DROP TABLE #TempHstItemStatus






GO

if object_id('sp_InsertHistoryMovementTuning') is not null
	drop procedure sp_InsertHistoryMovementTuning
GO






CREATE procedure [dbo].[sp_InsertHistoryMovementTuning] (  

@CompanyCode varchar(10) ,
@BranchCode varchar(10),
@Year int,
@Month int,
@PID varchar(10)
)


as

-- delete spHstStockMovement based on Year and Month
-- Include all coloumn into one than insert spHstStockMovement

delete spHstStockMovement where Year = @Year AND Month = @Month and CompanyCode = @CompanyCode and BranchCode = @BranchCode

SELECT * INTO #A1 FROM
(
SELECT a.CompanyCode, a.BranchCode, a.PartNo PartNo, a.WarehouseCode WareHouseCode, 
a.LocationCode LocationCode,  
a.OnHand OnHand,
a.BOMInvQty BOMInvQty,
(SELECT ISNULL(SUM(x.ReceivedQty), 0) RcvQty FROM spTrnPRcvDtl x WITH(NOLOCK, NOWAIT) JOIN spTrnPRcvHdr b WITH(NOLOCK, NOWAIT)
 ON x.CompanyCode = b.CompanyCode
	AND x.BranchCode = b.BranchCode
    AND x.WRSNo = b.WRSNo
	WHERE Year(b.WRSDate) = @Year
	  AND Month(b.WRSDate) = @Month
	  AND b.CompanyCode = @CompanyCode
      AND b.BranchCode = @BranchCode
	  AND x.WarehouseCode = a.WarehouseCode
	  AND x.PartNo = a.PartNo
      AND b.Status NOT IN (0, 1, 3)) AS RcvQtyIN,
(SELECT ISNULL(SUM(x.QtyAdjustment), 0) AdjQty FROM spTrnIAdjustDtl x WITH(NOLOCK, NOWAIT) JOIN spTrnIAdjustHdr b WITH(NOLOCK, NOWAIT)
 ON x.CompanyCode = b.CompanyCode
	AND x.BranchCode = b.BranchCode
    AND x.AdjustmentNo = b.AdjustmentNo
	WHERE YEAR(b.AdjustmentDate) = @Year
      AND MONTH(b.AdjustmentDate) = @Month
      AND b.CompanyCode = @CompanyCode
      AND b.BranchCode = @BranchCode
      AND x.WarehouseCode = a.WarehouseCode
      AND x.PartNo = a.PartNo
      AND x.AdjustmentCode = '+'
      AND b.Status = 2) AS AdjQtyIN,
(SELECT ISNULL(SUM(x.Qty),0) FROM spTrnIWHTrfDtl x WITH(NOLOCK, NOWAIT) JOIN spTrnIWHTrfHDR b WITH(NOLOCK, NOWAIT)
 ON x.CompanyCode = b.CompanyCode
	AND x.BranchCode = b.BranchCode
    AND x.WHTrfNo = b.WHTrfNo 
	WHERE YEAR(b.WHTrfDate) = @Year
	  AND MONTH(b.WHTrfDate) = @Month
	  AND b.CompanyCode =@CompanyCode
	  AND b.BranchCode = @BranchCode
	  AND x.ToWarehouseCode = a.WarehouseCode
	  AND x.PartNo = a.PartNo
	  AND b.Status = 2
) AS WhtQtyIN,
(SELECT ISNULL(SUM(x.QtyReturn), 0) RturQty FROM spTrnSRturDtl x WITH(NOLOCK, NOWAIT) JOIN spTrnSRturHdr b WITH(NOLOCK, NOWAIT)
 ON x.CompanyCode = b.CompanyCode
	AND x.BranchCode = b.BranchCode
    AND x.ReturnNo = b.ReturnNo
	WHERE YEAR(b.ReturnDate) = @Year
	  AND MONTH(b.ReturnDate) = @Month
	  AND b.CompanyCode = @CompanyCode
	  AND b.BranchCode = @BranchCode
	  AND x.WarehouseCode = a.WarehouseCode
	  AND x.PartNo = a.PartNo
	  AND b.Status = 2) AS RtrQtyIN,
(SELECT ISNULL(SUM(x.QtyReturn),0) RturQty FROM spTrnSRturSSDtl x WITH(NOLOCK, NOWAIT) JOIN spTrnSRturSSHdr b WITH(NOLOCK, NOWAIT)
 ON x.CompanyCode = b.CompanyCode
	AND x.BranchCode = b.BranchCode
    AND x.ReturnNo = b.ReturnNo
	WHERE YEAR(b.ReturnDate) = @Year
	  AND MONTH(b.ReturnDate) = @Month
	  AND b.CompanyCode = @CompanyCode
	  AND b.BranchCode = @BranchCode
	  AND x.WarehouseCode = a.WarehouseCode
	  AND x.PartNo = a.PartNo	
	  AND b.Status = 2) AS SSQtyIN,
(SELECT ISNULL(SUM(x.QtyBill)  ,0) FROM spTrnSFPJDtl x WITH(NOLOCK, NOWAIT)JOIN spTrnSFPJHdr b WITH(NOLOCK, NOWAIT)
 ON  x.CompanyCode = b.CompanyCode
	AND x.BranchCode = b.BranchCode
    AND x.FPJNo = b.FPJNo
	WHERE YEAR(b.FPJDate) = @Year
	  AND MONTH(b.FPJDate) = @Month
	  AND b.CompanyCode = @CompanyCode
	  AND b.BranchCode = @BranchCode
	  AND x.WarehouseCode = a.WarehouseCode
	  AND x.PartNo = a.PartNo) AS FpjQtyOUT,
(SELECT ISNULL(SUM(x.QtyBill) ,0)FROM spTrnSLmpDtl x WITH(NOLOCK, NOWAIT) JOIN spTrnSLmpHdr b WITH(NOLOCK, NOWAIT)
 ON x.CompanyCode = b.CompanyCode
	AND x.BranchCode = b.BranchCode
    AND x.LmpNo = b.LmpNo 
	WHERE YEAR(b.LmpDate) = @Year
	  AND MONTH(b.LmpDate) = @Month
	  AND b.CompanyCode = @CompanyCode
	  AND b.BranchCode = @BranchCode
	  AND x.WarehouseCode = a.WarehouseCode
	  AND x.PartNo = a.PartNo) AS LmpQtyOUT,
(SELECT ISNULL(SUM(x.QtyAdjustment), 0) AdjQty FROM spTrnIAdjustDtl x WITH(NOLOCK, NOWAIT) JOIN spTrnIAdjustHdr b WITH(NOLOCK, NOWAIT)
 ON x.CompanyCode = b.CompanyCode
	AND x.BranchCode = b.BranchCode
    AND x.AdjustmentNo = b.AdjustmentNo 
	WHERE YEAR(b.AdjustmentDate) = @Year
	  AND MONTH(b.AdjustmentDate) = @Month
	  AND b.CompanyCode = @CompanyCode
	  AND b.BranchCode = @BranchCode
	  AND x.WarehouseCode = a.WarehouseCode
	  AND x.PartNo = a.PartNo
	  AND x.AdjustmentCode = '-'
	  AND b.Status = 2) AS AdjQtyOUT,
(SELECT ISNULL(SUM(x.Qty),0) FROM spTrnIWHTrfDtl x WITH(NOLOCK, NOWAIT) JOIN spTrnIWHTrfHDR b WITH(NOLOCK, NOWAIT)
 ON x.CompanyCode = b.CompanyCode
	AND x.BranchCode = b.BranchCode
    AND x.WHTrfNo = b.WHTrfNo 
	WHERE YEAR(b.WHTrfDate) = @Year
	  AND MONTH(b.WHTrfDate) = @Month
	  AND b.CompanyCode =@CompanyCode
	  AND b.BranchCode = @BranchCode
	  AND x.FromWarehouseCode = a.WarehouseCode
	  AND x.PartNo = a.PartNo
	  AND b.Status = 2) AS WhtQtyOUT 
FROM spMstItemLoc a WITH(NOLOCK, NOWAIT)
WHERE a.CompanyCode = @CompanyCode
  AND a.BranchCode = @BranchCode
) a1

SELECT * INTO #A2 FROM
(
	SELECT a.CompanyCode, a.BranchCode, @Year AS Year, @Month AS Month, a.WarehouseCode, a.LocationCode, a.PartNo, 
        BOMInvQty AS BOMStock, 
		(RcvQtyIN + AdjQtyIN + WhtQtyIN + RtrQtyIN + SSQtyIN) AS MoveIn,
        (FpjQtyOUT + LmpQtyOUT + AdjQtyOUT + WhtQtyOUT ) AS MoveOut,
        (BOMInvQty + ((RcvQtyIN + AdjQtyIN + WhtQtyIN + RtrQtyIN + SSQtyIN ) - 
        (FpjQtyOUT + LmpQtyOUT + AdjQtyOUT + WhtQtyOUT))) AS EOMStock, 
		ISNULL(b.CostPrice,0) AS CostPrice,
        @PID AS CreatedBy, GetDate() AS CreatedDate,
		@PID AS LastUpdateBy, GetDate() AS LastUpdateDate
	FROM #A1 a WITH(NOLOCK, NOWAIT)
		LEFT JOIN SpMstItemPrice b WITH(NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.PartNo = b.PartNo
) b1

insert into spHstStockMovement select * from #A2
drop table #A1
drop table #A2


GO

if object_id('sp_InsertTransactionHistoryTuning') is not null
	drop procedure sp_InsertTransactionHistoryTuning
GO






CREATE procedure [dbo].[sp_InsertTransactionHistoryTuning] (  

@CompanyCode varchar(10) ,
@BranchCode varchar(10),
@Month int,
@Year int,
@CreatedBy varchar(10),
@CreatedDate datetime
)


as

DELETE spHstTransaction WHERE YEAR = @Year AND MONTH = @Month and CompanyCode = @CompanyCode and BranchCode = @BranchCode
SELECT 
	itemLoc.CompanyCode
    , itemLoc.BranchCode
    , itemLoc.PartNo
	, @Year Year
	, @Month Month
    , itemLoc.WarehouseCode
    , ProductType
    , PartCategory
    , MovingCode
    , ABCClass
    , TypeOfGoods
    , ISNULL(CostPrice, 0) CostPrice        
	, CASE WHEN itemLoc.WarehouseCode BETWEEN '00' AND '96' THEN ISNULL(Receiving.ReceivingForPurchaseQty, 0) ELSE 0 END ReceivingForPurchaseQty
	, CASE WHEN itemLoc.WarehouseCode BETWEEN '00' AND '96' THEN ISNULL(Receiving.ReceivingForNPurchaseQty, 0) ELSE 0 END ReceivingForNPurchaseQty
	, ISNULL(Adjustment.AdjustmentPlusQty, 0) AdjustmentPlusQty
	, ISNULL(Adjustment.AdjustmentMinusQty, 0) AdjustmentMinusQty
	, ISNULL(WHTrf.WTHOutQty, 0) WTHOutQty
	, ISNULL(WHTrf.WTHInQty, 0) WTHInQty
	, CASE WHEN itemLoc.WarehouseCode BETWEEN '00' AND '96' THEN ISNULL(FPJ.SalesForCreditQty, 0) ELSE 0 END SalesForCreditQty
	, CASE WHEN itemLoc.WarehouseCode BETWEEN '00' AND '96' THEN ISNULL(FPJ.SalesForCashQty, 0) ELSE 0 END SalesForCashQty
	, CASE WHEN itemLoc.WarehouseCode BETWEEN '00' AND '96' THEN ISNULL(Lmp.SalesForBPSQty, 0) ELSE 0 END SalesForBPSQty
	, CASE WHEN itemLoc.WarehouseCode BETWEEN '00' AND '96' THEN ISNULL(Lmp.SalesForServiceQty, 0) ELSE 0 END SalesForServiceQty
	, CASE WHEN itemLoc.WarehouseCode BETWEEN '00' AND '96' THEN ISNULL(Lmp.SalesForUnitQty, 0) ELSE 0 END SalesForUnitQty
	, CASE WHEN itemLoc.WarehouseCode BETWEEN '00' AND '96' THEN ISNULL(SRtur.ReturForSalesCreditQty, 0) ELSE 0 END ReturForSalesCreditQty
	, CASE WHEN itemLoc.WarehouseCode BETWEEN '00' AND '96' THEN ISNULL(SRtur.ReturForSalesCashQty, 0) ELSE 0 END ReturForSalesCashQty
	, CASE WHEN itemLoc.WarehouseCode BETWEEN '00' AND '96' THEN ISNULL(SRturSS.ReturnForBPSQty, 0) ELSE 0 END ReturnForBPSQty
	, CASE WHEN itemLoc.WarehouseCode BETWEEN '00' AND '96' THEN ISNULL(SRturSS.ReturForServiceQty, 0) ELSE 0 END ReturForServiceQty
	, CASE WHEN itemLoc.WarehouseCode BETWEEN '00' AND '96' THEN ISNULL(SRturSS.ReturForUnitQty, 0) ELSE 0 END ReturForUnitQty
	, CASE WHEN itemLoc.WarehouseCode BETWEEN '00' AND '96' THEN ISNULL(POS.PurchaseOrderQty, 0) ELSE 0 END PurchaseOrderQty
	, CASE WHEN itemLoc.WarehouseCode BETWEEN '00' AND '96' THEN ISNULL(Balance.OnOrderQty, 0) ELSE 0 END OnOrderQty
	, CASE WHEN itemLoc.WarehouseCode BETWEEN '00' AND '96' THEN ISNULL(Balance.InTransitQty, 0) ELSE 0 END InTransitQty
	, CASE WHEN itemLoc.WarehouseCode BETWEEN '00' AND '96' THEN ISNULL(BO.BackOrderQty, 0) ELSE 0 END BackOrderQty
	, ISNULL(itemLoc.OnHand, 0) OnHand
	, CASE WHEN itemLoc.WarehouseCode BETWEEN '00' AND '96' THEN ISNULL(Receiving.ReceivingForPurchaseAmt, 0) ELSE 0 END ReceivingForPurchaseAmt
	, CASE WHEN itemLoc.WarehouseCode BETWEEN '00' AND '96' THEN ISNULL(Receiving.ReceivingForNPurchaseAmt, 0) ELSE 0 END ReceivingForNPurchaseAmt
	, ISNULL(Adjustment.AdjustmentPlusAmt, 0) AdjustmentPlusAmt
	, ISNULL(Adjustment.AdjustmentMinusAmt, 0) AdjustmentMinusAmt
	, ISNULL(WHTrf.WTHOutAmt, 0) WTHOutAmt
	, ISNULL(WHTrf.WTHInAmt, 0) WTHInAmt
	, CASE WHEN itemLoc.WarehouseCode BETWEEN '00' AND '96' THEN ISNULL(FPJ.SalesForCreditAmt, 0) ELSE 0 END SalesForCreditAmt
	, CASE WHEN itemLoc.WarehouseCode BETWEEN '00' AND '96' THEN ISNULL(FPJ.SalesForCashAmt, 0) ELSE 0 END SalesForCashAmt
	, CASE WHEN itemLoc.WarehouseCode BETWEEN '00' AND '96' THEN ISNULL(Lmp.SalesForBPSAmt, 0) ELSE 0 END SalesForBPSAmt
	, CASE WHEN itemLoc.WarehouseCode BETWEEN '00' AND '96' THEN ISNULL(Lmp.SalesForServiceAmt, 0) ELSE 0 END SalesForServiceAmt
	, CASE WHEN itemLoc.WarehouseCode BETWEEN '00' AND '96' THEN ISNULL(Lmp.SalesForUnitAmt, 0) ELSE 0 END SalesForUnitAmt
	, CASE WHEN itemLoc.WarehouseCode BETWEEN '00' AND '96' THEN ISNULL(SRtur.ReturForSalesCreditAmt, 0) ELSE 0 END ReturForSalesCreditAmt
	, CASE WHEN itemLoc.WarehouseCode BETWEEN '00' AND '96' THEN ISNULL(SRtur.ReturForSalesCashAmt, 0) ELSE 0 END ReturForSalesCashAmt
	, CASE WHEN itemLoc.WarehouseCode BETWEEN '00' AND '96' THEN ISNULL(SRturSS.ReturnForBPSAmt, 0) ELSE 0 END ReturnForBPSAmt
	, CASE WHEN itemLoc.WarehouseCode BETWEEN '00' AND '96' THEN ISNULL(SRturSS.ReturForServiceAmt, 0) ELSE 0 END ReturForServiceAmt
	, CASE WHEN itemLoc.WarehouseCode BETWEEN '00' AND '96' THEN ISNULL(SRturSS.ReturForUnitAmt, 0) ELSE 0 END ReturForUnitAmt
	, CASE WHEN itemLoc.WarehouseCode BETWEEN '00' AND '96' THEN ISNULL(POS.PurchaseOrderAmt, 0) ELSE 0 END PurchaseOrderAmt
	, CASE WHEN itemLoc.WarehouseCode BETWEEN '00' AND '96' THEN ISNULL(Balance.OnOrderAmt, 0) ELSE 0 END OnOrderAmt
	, CASE WHEN itemLoc.WarehouseCode BETWEEN '00' AND '96' THEN ISNULL(Balance.InTransitAmt, 0) ELSE 0 END InTransitAmt
	, CASE WHEN itemLoc.WarehouseCode BETWEEN '00' AND '96' THEN ISNULL(BO.BackOrderAmt, 0) ELSE 0 END BackOrderAmt
	, ISNULL(itemLoc.OnHand, 0) * ISNULL(itemPrice.CostPrice, 0) OnHandAmt
	, @CreatedBy CreatedBy
	, @CreatedDate CreatedDate
INTO #HstTransaction
FROM spMstItemLoc itemLoc WITH(NOLOCK, NOWAIT)
	LEFT JOIN (SELECT rcvDtl.PartNo, rcvDtl.WarehouseCode 
		    , SUM(CASE WHEN (rcvHdr.ReceivingType in (1,2) AND rcvHdr.TransType = 4) THEN ISNULL(rcvDtl.ReceivedQty, 0) ELSE 0 END) ReceivingForPurchaseQty
            , SUM(CASE WHEN NOT (rcvHdr.ReceivingType in (1,2) AND rcvHdr.TransType = 4) THEN ISNULL(rcvDtl.ReceivedQty, 0) ELSE 0 END) ReceivingForNPurchaseQty	
            , SUM(CASE WHEN (rcvHdr.ReceivingType in (1,2) AND rcvHdr.TransType = 4) THEN ISNULL(rcvDtl.ReceivedQty, 0) * ISNULL(rcvDtl.PurchasePrice, 0) - ROUND(ISNULL(rcvDtl.ReceivedQty, 0) * ISNULL(rcvDtl.PurchasePrice, 0) * ISNULL(rcvDtl.DiscPct, 0) / 100, 0) ELSE 0 END) ReceivingForPurchaseAmt
            , SUM(CASE WHEN NOT (rcvHdr.ReceivingType in (1,2) AND rcvHdr.TransType = 4) THEN ISNULL(rcvDtl.ReceivedQty, 0) * ISNULL(rcvDtl.PurchasePrice, 0) - ROUND(ISNULL(rcvDtl.ReceivedQty, 0) * ISNULL(rcvDtl.PurchasePrice, 0) * ISNULL(rcvDtl.DiscPct, 0) / 100, 0) ELSE 0 END) ReceivingForNPurchaseAmt			
		FROM spTrnPRcvHdr rcvHdr WITH(NOLOCK, NOWAIT)
			INNER JOIN spTrnPRcvDtl rcvDtl with (nolock, nowait) ON rcvHdr.WRSNo = rcvDtl.WrsNo
				AND rcvHdr.CompanyCode = rcvDtl.CompanyCode
				AND rcvHdr.BranchCode = rcvDtl.BranchCode				                
		WHERE	rcvHdr.CompanyCode = @CompanyCode
			AND rcvHdr.BranchCode = @BranchCode
			AND MONTH(rcvHdr.WRSDate) = @Month 
			AND YEAR(rcvHdr.WRSDate) = @Year
			AND rcvHdr.Status NOT IN (0, 1, 3)
		GROUP BY rcvDtl.PartNo, rcvDtl.WarehouseCode			
	) Receiving ON itemLoc.PartNo = Receiving.PartNo AND itemloc.WarehouseCode = Receiving.WarehouseCode
	LEFT JOIN (SELECT adjDtl.PartNo, adjDtl.WarehouseCode
			, SUM(CASE adjustmentCode WHEN '+' THEN ISNULL(QtyAdjustment, 0) ELSE 0 END) AdjustmentPlusQty
			, SUM(CASE adjustmentCode WHEN '-' THEN ISNULL(QtyAdjustment, 0) ELSE 0 END) AdjustmentMinusQty	
            , SUM(CASE adjustmentCode WHEN '+' THEN ISNULL(QtyAdjustment, 0) * ISNULL(CostPrice, 0) ELSE 0 END) AdjustmentPlusAmt
			, SUM(CASE adjustmentCode WHEN '-' THEN ISNULL(QtyAdjustment, 0) * ISNULL(CostPrice, 0) ELSE 0 END) AdjustmentMinusAmt	
		FROM spTrnIAdjustHdr adjHdr WITH(NOLOCK, NOWAIT)
			INNER JOIN spTrnIAdjustDtl adjDtl WITH(NOLOCK, NOWAIT) ON adjHdr.AdjustmentNo = adjDtl.AdjustmentNo
				AND adjHdr.CompanyCode = adjDtl.CompanyCode 
				AND adjHdr.BranchCode = adjDtl.BranchCode				                
		WHERE adjHdr.CompanyCode = @CompanyCode
			AND adjHdr.BranchCode  = @BranchCode
			AND MONTH(adjHdr.AdjustmentDate) = @Month
			AND YEAR(adjHdr.AdjustmentDate) = @Year
			AND adjHdr.Status = 2
		GROUP BY adjDtl.PartNo, adjDtl.WarehouseCode
	) Adjustment ON itemLoc.PartNo = Adjustment.PartNo AND itemloc.WarehouseCode = Adjustment.WarehouseCode
	LEFT JOIN (SELECT aWHTrf.PartNo, aWHTrf.warehouseCode 
			, SUM(ISNULL(aWHTrf.WTHOutQty, 0))WTHOutQty , SUM(ISNULL(aWHTrf.WTHInQty, 0))WTHInQty  
            , SUM(ISNULL(aWHTrf.WTHOutAmt, 0))WTHOutAmt , SUM(ISNULL(aWHTrf.WTHInAmt, 0))WTHInAmt
        FROM 
        (SELECT whTrfDtl.PartNo, whTrfDtl.FromWarehouseCode warehouseCode 
			    , SUM(ISNULL(Qty,0)) WTHOutQty, 0 WTHInQty 
                , SUM(ISNULL(Qty, 0) * ISNULL(CostPrice, 0)) WTHOutAmt, 0 WTHInAmt
	        FROM spTrnIWhTrfHdr whTrfHdr WITH(NOLOCK, NOWAIT)
				INNER JOIN spTrnIWhTrfDtl whTrfDtl WITH(NOLOCK, NOWAIT) ON whTrfHdr.WHTrfNo = whTrfDtl.WHTrfNo
				AND whTrfHdr.CompanyCode = whTrfDtl.CompanyCode
				AND whTrfHdr.BranchCode = whTrfDtl.BranchCode			                
			WHERE whTrfHdr.CompanyCode = @CompanyCode
				AND whTrfHdr.BranchCode = @BranchCode
				AND MONTH(whTrfHdr.WHTrfDate) = @Month
				AND YEAR(whTrfHdr.WHTrfDate) = @Year
				AND whTrfHdr.Status = 2
			GROUP BY whTrfDtl.PartNo, whTrfDtl.FromWarehouseCode
			UNION ALL
			SELECT whTrfDtl.PartNo, whTrfDtl.ToWarehouseCode warehouseCode 
				,0 WTHOutQty, SUM(ISNULL(Qty, 0)) WTHInQty 
                ,0 WTHOutAmt, SUM(ISNULL(Qty, 0) * ISNULL(CostPrice, 0)) WTHInAmt 
			FROM spTrnIWhTrfHdr whTrfHdr WITH(NOLOCK, NOWAIT)
				INNER JOIN spTrnIWhTrfDtl whTrfDtl WITH(NOLOCK, NOWAIT) ON whTrfHdr.WHTrfNo = whTrfDtl.WHTrfNo
				AND whTrfHdr.CompanyCode = whTrfDtl.CompanyCode
				AND whTrfHdr.BranchCode = whTrfDtl.BranchCode			                
			WHERE whTrfHdr.CompanyCode = @CompanyCode
				AND whTrfHdr.BranchCode = @BranchCode
				AND MONTH(whTrfHdr.WHTrfDate) = @Month
				AND YEAR(whTrfHdr.WHTrfDate) = @Year
				AND whTrfHdr.Status = 2
			GROUP BY whTrfDtl.PartNo, whTrfDtl.ToWarehouseCode 
        ) aWHTrf
        GROUP BY aWHTrf.PartNo, aWHTrf.WarehouseCode
	) WHTrf ON itemLoc.PartNo = WHTrf.PartNo AND itemloc.WarehouseCode = WHTrf.WarehouseCode
	LEFT JOIN (SELECT fpjDtl.PartNo, fpjDtl.WarehouseCode
            , SUM(CASE WHEN (fpjHdr.TOPDays = 0) THEN ISNULL(QtyBill, 0) ELSE 0 END) as SalesForCashQty
            , SUM(CASE WHEN (fpjHdr.TOPDays > 0) THEN ISNULL(QtyBill, 0) ELSE 0 END) as SalesForCreditQty
            , SUM(CASE WHEN (fpjHdr.TOPDays = 0) THEN ISNULL(QtyBill, 0) * ISNULL(CostPrice, 0) ELSE 0 END) as SalesForCashAmt
            , SUM(CASE WHEN (fpjHdr.TOPDays > 0) THEN ISNULL(QtyBill, 0) * ISNULL(CostPrice, 0) ELSE 0 END) as SalesForCreditAmt
        FROM spTrnSFPJHdr fpjHdr WITH(NOLOCK, NOWAIT)
            INNER JOIN spTrnSFPJDtl fpjDtl WITH(NOLOCK, NOWAIT) ON fpjHdr.FPJNo = fpjDtl.FPJNo
                AND fpjHdr.CompanyCode = fpjDtl.CompanyCode
                AND fpjHdr.BranchCode = fpjDtl.BranchCode				                
        WHERE fpjHdr.CompanyCode = @CompanyCode
            AND fpjHdr.BranchCode = @BranchCode
            AND MONTH(fpjHdr.FPJDate) = @Month
            AND YEAR(fpjHdr.FPJDate) = @Year
        GROUP BY fpjDtl.PartNo, fpjDtl.WarehouseCode
	) FPJ ON itemLoc.PartNo = FPJ.PartNo AND itemloc.WarehouseCode = FPJ.WarehouseCode
	LEFT JOIN (SELECT lmpDtl.PartNo, lmpDtl.WarehouseCode
            , SUM(CASE bpsHdr.SalesType WHEN 1 THEN ISNULL(lmpDtl.QtyBill, 0) ELSE 0 END) as SalesForBPSQty
            , SUM(CASE bpsHdr.SalesType WHEN 2 THEN ISNULL(lmpDtl.QtyBill, 0) ELSE 0 END) as SalesForServiceQty
            , SUM(CASE bpsHdr.SalesType WHEN 3 THEN ISNULL(lmpDtl.QtyBill, 0) ELSE 0 END) as SalesForUnitQty
            , SUM(CASE bpsHdr.SalesType WHEN 1 THEN ISNULL(lmpDtl.QtyBill, 0) * ISNULL(lmpDtl.CostPrice, 0) ELSE 0 END) as SalesForBPSAmt
            , SUM(CASE bpsHdr.SalesType WHEN 2 THEN ISNULL(lmpDtl.QtyBill, 0) * ISNULL(lmpDtl.CostPrice, 0) ELSE 0 END) as SalesForServiceAmt
            , SUM(CASE bpsHdr.SalesType WHEN 3 THEN ISNULL(lmpDtl.QtyBill, 0) * ISNULL(lmpDtl.CostPrice, 0) ELSE 0 END) as SalesForUnitAmt
        FROM spTrnSLmpHdr lmpHdr WITH(NOLOCK, NOWAIT)
            INNER JOIN spTrnSBPSFHdr bpsHdr WITH(NOLOCK, NOWAIT) ON lmpHdr.BPSFNo = bpsHdr.BPSFNo
                AND lmpHdr.CompanyCode = bpsHdr.CompanyCode
                AND lmpHdr.BranchCode = bpsHdr.BranchCode
            INNER JOIN spTrnSLmpDtl lmpDtl WITH(NOLOCK, NOWAIT) ON lmpHdr.LmpNo = lmpDtl.LmpNo
                AND lmpHdr.CompanyCode = lmpDtl.CompanyCode
                AND lmpHdr.BranchCode = lmpDtl.BranchCode				                
        WHERE lmpHdr.CompanyCode = @CompanyCode
            AND lmpHdr.BranchCode = @BranchCode
            AND MONTH(lmpHdr.LmpDate) = @Month
            AND YEAR(lmpHdr.LmpDate) = @Year
        GROUP BY lmpDtl.PartNo, lmpDtl.WarehouseCode
	) Lmp ON itemLoc.PartNo = Lmp.PartNo AND itemloc.WarehouseCode = Lmp.WarehouseCode
	LEFT JOIN (SELECT rtrDtl.PartNo, rtrDtl.WarehouseCode
            , SUM(CASE WHEN (fpjHdr.TOPDays = 0) THEN ISNULL(rtrDtl.QtyReturn, 0) ELSE 0 END) as ReturForSalesCashQty
            , SUM(CASE WHEN (fpjHdr.TOPDays > 0) THEN ISNULL(rtrDtl.QtyReturn, 0) ELSE 0 END) as ReturForSalesCreditQty
            , SUM(CASE WHEN (fpjHdr.TOPDays = 0) THEN ISNULL(rtrDtl.QtyReturn, 0) * ISNULL(rtrDtl.CostPrice, 0) ELSE 0 END) as ReturForSalesCashAmt
            , SUM(CASE WHEN (fpjHdr.TOPDays > 0) THEN ISNULL(rtrDtl.QtyReturn, 0) * ISNULL(rtrDtl.CostPrice, 0) ELSE 0 END) as ReturForSalesCreditAmt
        FROM spTrnSRturHdr rtrHdr WITH(NOLOCK, NOWAIT)
            INNER JOIN spTrnSFPJHdr fpjHdr WITH(NOLOCK, NOWAIT) ON rtrHdr.FPJNo = fpjHdr.FPJNo
                AND rtrHdr.CompanyCode = fpjHdr.CompanyCode
                AND rtrHdr.BranchCode = fpjHdr.BranchCode
            INNER JOIN spTrnSRturDtl rtrDtl WITH(NOLOCK, NOWAIT) ON rtrHdr.ReturnNo = rtrDtl.ReturnNo
                AND rtrHdr.CompanyCode = rtrDtl.CompanyCode 
                AND rtrHdr.BranchCode = rtrDtl.BranchCode 				                
        WHERE	rtrHdr.CompanyCode = @CompanyCode
            AND rtrHdr.BranchCode = @BranchCode
            AND MONTH(rtrHdr.ReturnDate) = @Month
            AND YEAR(rtrHdr.ReturnDate) = @Year
            AND rtrHdr.Status = 2
        GROUP BY rtrDtl.PartNo, rtrDtl.WarehouseCode
	) SRtur ON itemLoc.PartNo = SRtur.PartNo AND itemloc.WarehouseCode = SRtur.WarehouseCode
	LEFT JOIN (SELECT rtrSSDtl.PartNo, rtrSSDtl.WarehouseCode
            , SUM(CASE rtrSSHdr.SalesType WHEN 1 THEN ISNULL(QtyReturn, 0) ELSE 0 END) as ReturnForBPSQty
            , SUM(CASE rtrSSHdr.SalesType WHEN 2 THEN ISNULL(QtyReturn, 0) ELSE 0 END) as ReturForServiceQty
            , SUM(CASE rtrSSHdr.SalesType WHEN 3 THEN ISNULL(QtyReturn, 0) ELSE 0 END) as ReturForUnitQty
            , SUM(CASE rtrSSHdr.SalesType WHEN 1 THEN ISNULL(QtyReturn, 0) * ISNULL(CostPrice, 0) ELSE 0 END) as ReturnForBPSAmt
            , SUM(CASE rtrSSHdr.SalesType WHEN 2 THEN ISNULL(QtyReturn, 0) * ISNULL(CostPrice, 0) ELSE 0 END) as ReturForServiceAmt
            , SUM(CASE rtrSSHdr.SalesType WHEN 3 THEN ISNULL(QtyReturn, 0) * ISNULL(CostPrice, 0) ELSE 0 END) as ReturForUnitAmt
        FROM spTrnSRturSSHdr rtrSSHdr WITH(NOLOCK, NOWAIT)
            INNER JOIN spTrnSRturSSDtl rtrSSDtl WITH(NOLOCK, NOWAIT) ON rtrSSHdr.ReturnNo = rtrSSDtl.ReturnNo
                AND rtrSSHdr.CompanyCode = rtrSSDtl.CompanyCode
                AND rtrSSHdr.BranchCode = rtrSSDtl.BranchCode				                
        WHERE	rtrSSHdr.CompanyCode = @CompanyCode
            AND rtrSSHdr.BranchCode = @BranchCode
            AND MONTH(rtrSSHdr.ReturnDate) = @Month
            AND YEAR(rtrSSHdr.ReturnDate) = @Year
            AND rtrSSHdr.Status = 2
        GROUP BY rtrSSDtl.PartNo, rtrSSDtl.WarehouseCode
	) SRturSS ON itemLoc.PartNo = SRturSS.PartNo AND itemloc.WarehouseCode = SRturSS.WarehouseCode
	LEFT JOIN (SELECT posDtl.PartNo, SUM(ISNULL(posDtl.OrderQty, 0)) PurchaseOrderQty,
            SUM(ISNULL(posDtl.OrderQty, 0) * ISNULL(posDtl.PurchasePrice, 0) - ROUND(ISNULL(posDtl.OrderQty, 0) * ISNULL(posDtl.PurchasePrice, 0) * ISNULL(posDtl.DiscPct, 0) / 100, 0)) PurchaseOrderAmt
        FROM spTrnPPOSHdr posHDr WITH(NOLOCK, NOWAIT)
            INNER JOIN spTrnPPOSDtl posDtl WITH(NOLOCK, NOWAIT) ON posHdr.POSNo = posDtl.POSNo
                AND posHdr.CompanyCode = posDtl.CompanyCode
                AND posHdr.BranchCode = posDtl.BranchCode				                
        WHERE posHdr.CompanyCode = @CompanyCode 
            AND posHdr.BranchCode = @BranchCode                                
            AND MONTH(posHdr.POSDate) = @Month
            AND YEAR(posHdr.POSDate) = @Year
            AND posHdr.Status NOT IN (0, 1, 3)
        GROUP BY posDtl.PartNo
	) POS ON itemLoc.PartNo = POS.PartNo
	LEFT JOIN (SELECT PartNo, SUM(ISNULL(OnOrder, 0)) OnOrderQty, SUM(ISNULL(InTransit, 0)) InTransitQty,
            SUM(ISNULL(OnOrder, 0) * ISNULL(PurchasePrice, 0) - ROUND(ISNULL(OnOrder, 0) * ISNULL(PurchasePrice, 0) * ISNULL(DiscPct, 0) / 100, 0)) OnOrderAmt,
            SUM(ISNULL(InTransit, 0) * ISNULL(PurchasePrice, 0) - ROUND(ISNULL(InTransit, 0) * ISNULL(PurchasePrice, 0) * ISNULL(DiscPct, 0) / 100, 0)) InTransitAmt
        FROM spTrnPOrderBalance WITH(NOLOCK, NOWAIT)
        WHERE MONTH(POSDate) = @Month
            AND YEAR(POSDate) = @Year
            AND CompanyCode = @CompanyCode 
            AND BranchCode = @BranchCode
        GROUP BY PartNo
	) Balance ON itemLoc.PartNo = Balance.PartNo
	LEFT JOIN (SELECT ordDtl.PartNo, ordDtl.WarehouseCode
            , SUM(ISNULL(ordDtl.QtyBO, 0) - ISNULL(ordDtl.QtyBOSupply, 0) - ISNULL(ordDtl.QtyBOCancel, 0)) BackOrderQty
            , SUM((ISNULL(ordDtl.QtyBO, 0) - ISNULL(ordDtl.QtyBOSupply, 0) - ISNULL(ordDtl.QtyBOCancel, 0)) * ISNULL(ordDtl.CostPrice, 0)) BackOrderAmt
        FROM spTrnSORDHdr ordHdr WITH(NOLOCK, NOWAIT)
            INNER JOIN spTrnSORDDtl ordDtl WITH(NOLOCK, NOWAIT) ON ordHdr.DocNo = ordDtl.DocNo 
                AND ordHdr.CompanyCode = ordDtl.CompanyCode		
                AND ordHdr.BranchCode = ordDtl.BranchCode				                
        WHERE ordHdr.CompanyCode = @CompanyCode
            AND ordHdr.BranchCode = @BranchCode
            AND MONTH(ordHdr.DocDate) = @Month
            AND YEAR(ordHdr.DocDate) = @Year
            AND ordHdr.Status NOT IN (0, 1, 3)
        GROUP BY ordDtl.PartNo, ordDtl.WarehouseCode
	) BO ON itemLoc.PartNo = BO.PartNo AND itemloc.WarehouseCode = BO.WarehouseCode 
    LEFT JOIN spMstItems items WITH(NOLOCK, NOWAIT) ON itemLoc.PartNo = items.PartNo
        AND itemLoc.CompanyCode = items.CompanyCode
        AND itemLoc.BranchCode = items.BranchCode
    LEFT JOIN spMstItemPrice itemPrice WITH(NOLOCK, NOWAIT) ON itemLoc.PartNo = itemPrice.PartNo
        AND itemLoc.CompanyCode = itemPrice.CompanyCode
        AND itemLoc.BranchCode = itemPrice.BranchCode		
WHERE itemLoc.CompanyCode = @CompanyCode 
	AND itemLoc.BranchCode = @BranchCode		

INSERT INTO spHstTransaction SELECT * FROM #HstTransaction
DROP TABLE #HstTransaction


GO

if object_id('sp_IsValidSTAnalyze') is not null
	drop procedure sp_IsValidSTAnalyze
GO





CREATE procedure [dbo].[sp_IsValidSTAnalyze] (  

@CompanyCode varchar(10) ,
@BranchCode varchar(10),
@STHdrNo varchar(20))


as

SELECT * INTO #a2 FROM(
SELECT
	a.PartNo, a.STNo, a.SEqNo,  CASE WHEN b.Partno is null THEN 'LOKASI UTAMA BELUM DIENTRY' ELSE '' END Status
FROM SpTrnStockTakingDtl a
LEFT JOIN 
(
	SELECT
		x.CompanyCode,
		x.BranchCode,
		x.StHdrNo,
		x.PartNo
	FROM SpTrnStockTakingDtl x
	WHERE x.CompanyCode = @CompanyCode
		AND x.BranchCode = @BranchCode
		AND x.StHdrNo = @StHdrNo
		AND x.IsMainLocation = 1

) b ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.StHdrNo = b.StHdrNo AND a.PartNo = b.PartNo
WHERE a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND a.StHdrNo = @StHdrNo
GROUP BY a.PartNo, b.PartNo, a.STNo, a.SeqNo
) #a2

SELECT * FROM #a2 WHERE Status <> ''
UNION
SELECT
	a.PartNo, a.STNo, a.SEqNo,'BELUM TERDAFTAR PADA LIST TAG/FORM' Status
FROM SpTrnStockTakingTemp a
WHERE CompanyCode = @CompanyCode 
	AND BranchCode = @BranchCode
	AND StHdrNo = @StHdrNo
	AND PartNo NOT IN (
SELECT 
DISTINCT(a.PartNo)
FROM SpTrnStockTakingDtl a
WHERE CompanyCode = @CompanyCode
	  AND BranchCode = @BranchCode
	  AND StHdrNo = @StHdrNo)
	AND PartNo <> ''
GROUP BY a.PartNo, a.STNo, a.SeqNo
UNION
SELECT
	a.PartNo, a.STNo, a.SEqNo, 'BLANK TAG/FORM BELUM TERPAKAI/DIBATALKAN' Status
FROM SpTrnStockTakingTemp a
WHERE CompanyCode = @CompanyCode 
	AND BranchCode = @BranchCode
	AND StHdrNo = @StHdrNo
	AND STNo NOT IN (
SELECT 
DISTINCT(a.StNo)
FROM SpTrnStockTakingDtl a
WHERE CompanyCode = @CompanyCode
	  AND BranchCode = @BranchCode
	  AND StHdrNo = @StHdrNo)
	AND PartNo = '' AND a.Status IN ('0','1')
GROUP BY a.PartNo, a.STNo, a.SeqNo
UNION
SELECT PartNo, STNo, SeqNo, 'LOKASI UTAMA DI-ENTRY LEBIH DARI BATAS' Status
FROM SpTrnStocktakingDtl
WHERE CompanyCode = @CompanyCode 
	AND BranchCode = @BranchCode
	AND StHdrNo = @StHdrNo
	AND PartNo IN (
SELECT
	PartNo
FROM spTrnStockTakingDtl
WHERE CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND StHdrNo = @StHdrNo
	AND IsMainLocation = 1
Group By PartNo
HAVING
	Count(PartNo) > 1)
DROP TABLE #a2





GO

if object_id('sp_KaroseriBrowse') is not null
	drop procedure sp_KaroseriBrowse
GO

CREATE procedure [dbo].[sp_KaroseriBrowse]  @CompanyCode varchar(10), @BranchCode varchar(10)
 as

select a.CompanyCode, a.BranchCode, a.SupplierCode, b.SupplierName, a.SalesModelCode, c.SalesModelDesc, a.SalesModelCodeNew, d.SalesModelDesc as SalesModelDescNew
		,a.DPPMaterial, a.DPPFee, a.DPPOthers, a.PPn, a.Total, a.Remark, cast(a.Status AS bit) as Status
from omMstKaroseri a
INNER JOIN gnMstSupplier b
	ON a.SupplierCode = b.SupplierCode
INNER JOIN omMstModel c
	ON a.SalesModelCode = c.SalesModelCode
INNER JOIN omMstModel d
	ON a.SalesModelCodeNew = d.SalesModelCode
WHERE a.CompanyCode = @CompanyCode AND a.BranchCode = @BranchCode 
GO

if object_id('sp_MaintainAvgCostItem') is not null
	drop procedure sp_MaintainAvgCostItem
GO







CREATE procedure [dbo].[sp_MaintainAvgCostItem] (  

@CompanyCode varchar(10),
@BranchCode varchar(10),
@ProductType varchar(10),
@PartNo varchar (20),
@Option varchar (2)
)


as

IF @Option = 'A'
BEGIN

SELECT TOP 1500
 Items.PartNo
,Items.ProductType
,(SELECT LookupValueName 
    FROM gnMstLookupDtl 
   WHERE CodeID = 'PRCT' AND 
         LookUpValue = Items.PartCategory AND 
         CompanyCode = @CompanyCode) AS CategoryName
,Items.PartCategory
,ItemInfo.PartName
,CASE ItemInfo.IsGenuinePart WHEN 1 THEN 'Ya' ELSE 'Tidak' END AS IsGenuinePart
,CASE Items.Status WHEN 1 THEN 'Aktif' ELSE 'Tidak' END AS IsActive
,ItemInfo.OrderUnit
,ItemInfo.SupplierCode
,Supplier.SupplierName
,(SELECT LookupValueName 
    FROM gnMstLookupDtl 
  WHERE CodeID = 'TPGO' AND 
        LookUpValue = Items.TypeOfGoods AND 
        CompanyCode = @CompanyCode) AS TypeOfGoods
,ISNULL(ItemLoc.WarehouseCode,0) WarehouseCode
,ISNULL(ItemLoc.LocationCode,0) LocationCode
,(ISNULL(ItemLoc.OnHand,0) - (ISNULL(ItemLoc.AllocationSP,0) + ISNULL(ItemLoc.AllocationSR,0) + ISNULL(ItemLoc.AllocationSL,0) + ISNULL(ItemLoc.ReservedSP,0) + ISNULL(ItemLoc.ReservedSR,0) + ISNULL(ItemLoc.ReservedSL,0))) AS QtyAvail
,ISNULL(ItemPrice.RetailPrice,0) RetailPrice
,ISNULL(ItemPrice.RetailPriceInclTax,0) RetailPriceInclTax
FROM SpMstItems Items
LEFT JOIN SpMstItemInfo ItemInfo   ON Items.CompanyCode  = ItemInfo.CompanyCode                          
                         AND Items.PartNo = ItemInfo.PartNo
LEFT JOIN SpMstItemLoc ItemLoc ON Items.CompanyCode  = ItemLoc.CompanyCode
	AND Items.BranchCode = ItemLoc.BranchCode	
	AND Items.PartNo = ItemLoc.PartNo
LEFT JOIN SpMstItemPrice ItemPrice ON Items.CompanyCode  = ItemPrice.CompanyCode
	AND Items.BranchCode = ItemPrice.BranchCode	
	AND Items.PartNo = ItemPrice.PartNo		 
LEFT JOIN GnMstSupplier Supplier ON Supplier.CompanyCode  = Items.CompanyCode 
                         AND Supplier.SupplierCode = ItemInfo.SupplierCode
WHERE Items.CompanyCode = @CompanyCode
  AND Items.BranchCode  = @BranchCode    
  AND Items.ProductType = @ProductType
  AND ItemLoc.WarehouseCode = '00'
END
ELSE
BEGIN

SELECT TOP 1500
 Items.PartNo
,Items.ProductType
,(SELECT LookupValueName 
    FROM gnMstLookupDtl 
   WHERE CodeID = 'PRCT' AND 
         LookUpValue = Items.PartCategory AND 
         CompanyCode = @CompanyCode) AS CategoryName
,Items.PartCategory
,ItemInfo.PartName
,CASE ItemInfo.IsGenuinePart WHEN 1 THEN 'Ya' ELSE 'Tidak' END AS IsGenuinePart
,CASE Items.Status WHEN 1 THEN 'Aktif' ELSE 'Tidak' END AS IsActive
,ItemInfo.OrderUnit
,Items.Onhand
,ItemInfo.SupplierCode
,Supplier.SupplierName
,(SELECT LookupValueName 
    FROM gnMstLookupDtl 
  WHERE CodeID = 'TPGO' AND 
        LookUpValue = Items.TypeOfGoods AND 
        CompanyCode = @CompanyCode) AS TypeOfGoods
,ItemLoc.WarehouseCode
,ItemLoc.LocationCode
,(ItemLoc.OnHand - (ItemLoc.AllocationSP + ItemLoc.AllocationSR + ItemLoc.AllocationSL + ItemLoc.ReservedSP + ItemLoc.ReservedSR + ItemLoc.ReservedSL)) AS QtyAvail
,ItemPrice.RetailPrice
,ItemPrice.CostPrice
,ItemPrice.RetailPriceInclTax
FROM SpMstItems Items with (nolock, nowait)
LEFT JOIN SpMstItemInfo ItemInfo   ON Items.CompanyCode  = ItemInfo.CompanyCode                          
                         AND Items.PartNo = ItemInfo.PartNo
LEFT JOIN SpMstItemLoc ItemLoc ON Items.CompanyCode  = ItemLoc.CompanyCode
	AND Items.BranchCode = ItemLoc.BranchCode	
	AND Items.PartNo = ItemLoc.PartNo
LEFT JOIN SpMstItemPrice ItemPrice ON Items.CompanyCode  = ItemPrice.CompanyCode
	AND Items.BranchCode = ItemPrice.BranchCode	
	AND Items.PartNo = ItemPrice.PartNo		 
LEFT JOIN GnMstSupplier Supplier ON Supplier.CompanyCode  = Items.CompanyCode 
                         AND Supplier.SupplierCode = ItemInfo.SupplierCode
WHERE Items.CompanyCode = @CompanyCode
  AND Items.BranchCode  = @BranchCode    
  AND Items.ProductType = @ProductType
  AND Items.PartNo      = @PartNo
  AND ItemLoc.WarehouseCode = '00'
  END


GO

if object_id('sp_ModelAccountBrowse') is not null
	drop procedure sp_ModelAccountBrowse
GO

CREATE procedure [dbo].[sp_ModelAccountBrowse]  @CompanyCode varchar(10), @BranchCode varchar(10)
 as

 SELECT a.CompanyCode, a.BranchCode, a.SalesModelCode, z.SalesModelDesc
		, SalesAccNo, DiscountAccNo, ReturnAccNo, COGsAccNo,HReturnAccNo
		, b.Description as SalesAccDesc, c.Description as DiscountAccDesc, d.Description as ReturnAccDesc, e.Description as COGsAccDesc, f.Description as HReturnAccDesc
		, SalesAccNoAks, ReturnAccNoAks, DiscountAccNoAks
		, g.Description as SalesAccDescAks, h.Description as ReturnAccDescAks, i.Description as DiscountAccDescAks
		, ShipAccNo, DepositAccNo, OthersAccNo, BBNAccNo, KIRAccNo
		,j.Description as ShipAccDesc, k.Description as DepositAccDesc, l.Description as OthersAccDesc, m.Description as BBNAccDesc, n.Description as KIRAccDesc
		, PReturnAccNo, InTransitTransferStockAccNo
		, o.Description as PReturnAccDesc, p.Description as IntransitAccDesc
		, a.Remark, IsActive, a.InventoryAccNo, q.Description as InventoryAccDesc
-- select *
FROM omMstModelAccount a
LEFT JOIN GnMstAccount b
	ON a.SalesAccNo = b.AccountNo
LEFT JOIN GnMstAccount c
	ON a.DiscountAccNo = c.AccountNo
LEFT JOIN GnMstAccount d
	ON a.ReturnAccNo = d.AccountNo
LEFT JOIN GnMstAccount e
	ON a.COGsAccNo = e.AccountNo
LEFT JOIN GnMstAccount f
	ON a.HReturnAccNo = f.AccountNo
LEFT JOIN GnMstAccount g
	ON a.SalesAccNoAks = g.AccountNo
LEFT JOIN GnMstAccount h
	ON a.ReturnAccNoAks = h.AccountNo
LEFT JOIN GnMstAccount i
	ON a.DiscountAccNoAks = i.AccountNo
LEFT JOIN GnMstAccount j
	ON a.ShipAccNo = j.AccountNo
LEFT JOIN GnMstAccount k
	ON a.DepositAccNo = k.AccountNo
LEFT JOIN GnMstAccount l
	ON a.OthersAccNo = l.AccountNo
LEFT JOIN GnMstAccount m
	ON a.BBNAccNo = m.AccountNo
LEFT JOIN GnMstAccount n
	ON a.KIRAccNo = n.AccountNo
LEFT JOIN GnMstAccount o
	ON a.PReturnAccNo = o.AccountNo AND o.BranchCode = @BranchCode
LEFT JOIN GnMstAccount p
	ON a.InTransitTransferStockAccNo = p.AccountNo AND p.BranchCode = @BranchCode
LEFT JOIN GnMstAccount q
	ON a.InventoryAccNo = q.AccountNo
INNER JOIN omMstModel z
	ON a.SalesModelCode = z.SalesModelCode
WHERE a.CompanyCode = @CompanyCode AND a.BranchCode = @BranchCode 
GO

if object_id('sp_ModelAccountLookup') is not null
	drop procedure sp_ModelAccountLookup
GO

CREATE procedure [dbo].[sp_ModelAccountLookup]  @CompanyCode varchar(10), @BranchCode varchar(10), @SalesModelCode varchar(100)
 as

 SELECT a.CompanyCode, a.BranchCode, a.SalesModelCode, z.SalesModelDesc
		, SalesAccNo, DiscountAccNo, ReturnAccNo, COGsAccNo,HReturnAccNo
		, b.Description as SalesAccDesc, c.Description as DiscountAccDesc, d.Description as ReturnAccDesc, e.Description as COGsAccDesc, f.Description as HReturnAccDesc
		, SalesAccNoAks, ReturnAccNoAks, DiscountAccNoAks
		, g.Description as SalesAccDescAks, h.Description as ReturnAccDescAks, i.Description as DiscountAccDescAks
		, ShipAccNo, DepositAccNo, OthersAccNo, BBNAccNo, KIRAccNo
		,j.Description as ShipAccDesc, k.Description as DepositAccDesc, l.Description as OthersAccDesc, m.Description as BBNAccDesc, n.Description as KIRAccDesc
		, PReturnAccNo, InTransitTransferStockAccNo
		, o.Description as PReturnAccDesc, p.Description as IntransitAccDesc
		, a.Remark, IsActive, a.InventoryAccNo, q.Description as InventoryAccDesc
-- select *
FROM omMstModelAccount a
LEFT JOIN GnMstAccount b
	ON a.SalesAccNo = b.AccountNo
LEFT JOIN GnMstAccount c
	ON a.DiscountAccNo = c.AccountNo
LEFT JOIN GnMstAccount d
	ON a.ReturnAccNo = d.AccountNo
LEFT JOIN GnMstAccount e
	ON a.COGsAccNo = e.AccountNo
LEFT JOIN GnMstAccount f
	ON a.HReturnAccNo = f.AccountNo
LEFT JOIN GnMstAccount g
	ON a.SalesAccNoAks = g.AccountNo
LEFT JOIN GnMstAccount h
	ON a.ReturnAccNoAks = h.AccountNo
LEFT JOIN GnMstAccount i
	ON a.DiscountAccNoAks = i.AccountNo
LEFT JOIN GnMstAccount j
	ON a.ShipAccNo = j.AccountNo
LEFT JOIN GnMstAccount k
	ON a.DepositAccNo = k.AccountNo
LEFT JOIN GnMstAccount l
	ON a.OthersAccNo = l.AccountNo
LEFT JOIN GnMstAccount m
	ON a.BBNAccNo = m.AccountNo
LEFT JOIN GnMstAccount n
	ON a.KIRAccNo = n.AccountNo
LEFT JOIN GnMstAccount o
	ON a.PReturnAccNo = o.AccountNo AND o.BranchCode = @BranchCode
LEFT JOIN GnMstAccount p
	ON a.InTransitTransferStockAccNo = p.AccountNo AND p.BranchCode = @BranchCode
LEFT JOIN GnMstAccount q
	ON a.InventoryAccNo = q.AccountNo
INNER JOIN omMstModel z
	ON a.SalesModelCode = z.SalesModelCode
WHERE a.CompanyCode = @CompanyCode AND a.BranchCode = @BranchCode and a.SalesModelCode = @SalesModelCode
GO

if object_id('sp_MstOthersInventoryBrowse') is not null
	drop procedure sp_MstOthersInventoryBrowse
GO
CREATE procedure sp_MstOthersInventoryBrowse @CompanyCode varchar(10), @BranchCode varchar(10)
as

SELECT a.OthersNonInventory, a.OthersNonInventoryDesc, a.OthersNonInventoryAccNo, b.Description, a.Remark, a.IsActive
FROM omMstOthersNonInventory a
INNER JOIN GnMstAccount b
ON a.OthersNonInventoryAccNo=b.AccountNo
WHERE a.CompanyCode=@CompanyCode AND a.BranchCode=@BranchCode
GO

if object_id('sp_MstPriceListBeliBrowse') is not null
	drop procedure sp_MstPriceListBeliBrowse
GO
CREATE procedure [dbo].[sp_MstPriceListBeliBrowse] @CompanyCode varchar(10) , @BranchCode varchar(100)
 as

SELECT a.CompanyCode, a.BranchCode, a.SupplierCode, b.SupplierName, a.SalesModelCode, c.SalesModelDesc, a.SalesModelYear
		,a.PPnBMPaid, a.DPP, a.PPn, a.PPnBM, a.Total, a.Remark, cast(a.Status AS bit) as Status
FROM omMstPricelistBuy a
LEFT JOIN gnMstSupplier b
	ON a.SupplierCode = b.SupplierCode
LEFT JOIN omMstModel c
	ON a.SalesModelCode = c.SalesModelCode
WHERE a.CompanyCode = @CompanyCode and a.BranchCode = @BranchCode
GO

if object_id('sp_MstPriceListJualBrowse') is not null
	drop procedure sp_MstPriceListJualBrowse
GO
CREATE procedure [dbo].[sp_MstPriceListJualBrowse] @CompanyCode varchar(10) , @BranchCode varchar(100)
 as

SELECT a.CompanyCode, a.BranchCode, a.GroupPriceCode, b.RefferenceDesc1 as GroupPriceName, a.SalesModelCode, c.SalesModelDesc, a.SalesModelYear
		, a.TotalMinStaff, a.DPP, a.PPn, a.PPnBM, a.Total, a.Remark, cast(a.Status AS bit) as Status, a.TaxCode, a.TaxPct
FROM omMstPricelistSell a
LEFT JOIN OmMstRefference b
	ON a.GroupPriceCode = b.RefferenceCode AND b.RefferenceType='GRPR'
LEFT JOIN omMstModel c
	ON a.SalesModelCode = c.SalesModelCode
WHERE a.CompanyCode = @CompanyCode and a.BranchCode = @BranchCode

GO

if object_id('sp_MstPriceListJualView') is not null
	drop procedure sp_MstPriceListJualView
GO
CREATE procedure [dbo].[sp_MstPriceListJualView] @CompanyCode varchar(10) , @BranchCode varchar(100)
													, @GroupPriceCode varchar(10),@SalesModelCode varchar(100), @SalesModelYear varchar(10)
 as

SELECT a.CompanyCode, a.BranchCode, a.GroupPriceCode, b.RefferenceDesc1 as GroupPriceName, a.SalesModelCode, c.SalesModelDesc, a.SalesModelYear
		, a.*
FROM omMstPricelistSell a
LEFT JOIN OmMstRefference b
	ON a.GroupPriceCode = b.RefferenceCode AND b.RefferenceType='GRPR'
LEFT JOIN omMstModel c
	ON a.SalesModelCode = c.SalesModelCode
WHERE a.CompanyCode = @CompanyCode and a.BranchCode = @BranchCode
		and a.GroupPriceCode = @GroupPriceCode and a.SalesModelCode = @SalesModelCode and a.SalesModelYear = @SalesModelYear

GO

if object_id('sp_omMstModelColour') is not null
	drop procedure sp_omMstModelColour
GO
CREATE procedure [dbo].[sp_omMstModelColour] @CompanyCode varchar(10) , @SalesModelCode varchar(100)
 as

SELECT a.CompanyCode, a.SalesModelCode, b.SalesModelDesc, ColourCode, RefferenceDesc1, a.Remark, cast(a.Status AS bit) as [Status]
FROM omMstModelColourView a
INNER JOIN omMstModel b
	ON a.SalesModelCode = b.SalesModelCode
where a.CompanyCode=@CompanyCode and a.SalesModelCode=@SalesModelCode
GO

if object_id('sp_omMstModelPerlengkapan') is not null
	drop procedure sp_omMstModelPerlengkapan
GO
CREATE procedure [dbo].[sp_omMstModelPerlengkapan] @CompanyCode varchar(10), @BranchCode varchar(10), @SalesModelCode varchar(100)
 as

SELECT a.CompanyCode
		, a.BranchCode
		, a.SalesModelCode
		, a.PerlengkapanCode
		, b.PerlengkapanName
		, a.Quantity
		, a.Remark
		, CAST(a.Status AS bit) as [Status]

FROM omMstModelPerlengkapan a
LEFT JOIN omMstPerlengkapan b
	ON a.PerlengkapanCode = b.PerlengkapanCode
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
where a.CompanyCode=@CompanyCode and a.BranchCode = @BranchCode and SalesModelCode=@SalesModelCode
GO

if object_id('sp_omMstModelYear') is not null
	drop procedure sp_omMstModelYear
GO
CREATE procedure [dbo].[sp_omMstModelYear] @CompanyCode varchar(10) , @SalesModelCode varchar(100)
 as

SELECT CompanyCode, SalesModelCode, SalesModelYear, SalesModelDesc, ChassisCode, Remark, cast(Status AS bit) as [Status]
FROM omMstModelYear
where CompanyCode=@CompanyCode and SalesModelCode=@SalesModelCode
GO

if object_id('sp_pkeys') is not null
	drop procedure sp_pkeys
GO
CREATE procedure [dbo].[sp_pkeys]
(
    @table_name      sysname,
    @table_owner     sysname = null,
    @table_qualifier sysname = null
)
as
    declare @table_id           int
    -- quotename() returns up to 258 chars
    declare @full_table_name    nvarchar(517) -- 258 + 1 + 258

    if @table_qualifier is not null
    begin
        if db_name() <> @table_qualifier
        begin   -- If qualifier doesn't match current database
            raiserror (15250, -1,-1)
            return
        end
    end

    if @table_owner is null
    begin   -- If unqualified table name
        select @full_table_name = quotename(@table_name)
    end
    else
    begin   -- Qualified table name
        if @table_owner = ''
        begin   -- If empty owner name
            select @full_table_name = quotename(@table_owner)
        end
        else
        begin
            select @full_table_name = quotename(@table_owner) + '.' + quotename(@table_name)
        end
    end

    select @table_id = object_id(@full_table_name)

    select
        TABLE_QUALIFIER = convert(sysname,db_name()),
        TABLE_OWNER = convert(sysname,schema_name(o.schema_id)),
        TABLE_NAME = convert(sysname,o.name),
        COLUMN_NAME = convert(sysname,c.name),
        KEY_SEQ = convert (smallint,
            case
                when c.name = index_col(@full_table_name, i.index_id,  1) then 1
                when c.name = index_col(@full_table_name, i.index_id,  2) then 2
                when c.name = index_col(@full_table_name, i.index_id,  3) then 3
                when c.name = index_col(@full_table_name, i.index_id,  4) then 4
                when c.name = index_col(@full_table_name, i.index_id,  5) then 5
                when c.name = index_col(@full_table_name, i.index_id,  6) then 6
                when c.name = index_col(@full_table_name, i.index_id,  7) then 7
                when c.name = index_col(@full_table_name, i.index_id,  8) then 8
                when c.name = index_col(@full_table_name, i.index_id,  9) then 9
                when c.name = index_col(@full_table_name, i.index_id, 10) then 10
                when c.name = index_col(@full_table_name, i.index_id, 11) then 11
                when c.name = index_col(@full_table_name, i.index_id, 12) then 12
                when c.name = index_col(@full_table_name, i.index_id, 13) then 13
                when c.name = index_col(@full_table_name, i.index_id, 14) then 14
                when c.name = index_col(@full_table_name, i.index_id, 15) then 15
                when c.name = index_col(@full_table_name, i.index_id, 16) then 16
            end),
        PK_NAME = convert(sysname,i.name)
    from
        sys.indexes i,
        sys.all_columns c,
        sys.all_objects o
    where
        o.object_id = @table_id and
        o.object_id = c.object_id and
        o.object_id = i.object_id and
        i.is_primary_key = 1 and
        (c.name = index_col (@full_table_name, i.index_id,  1) or
         c.name = index_col (@full_table_name, i.index_id,  2) or
         c.name = index_col (@full_table_name, i.index_id,  3) or
         c.name = index_col (@full_table_name, i.index_id,  4) or
         c.name = index_col (@full_table_name, i.index_id,  5) or
         c.name = index_col (@full_table_name, i.index_id,  6) or
         c.name = index_col (@full_table_name, i.index_id,  7) or
         c.name = index_col (@full_table_name, i.index_id,  8) or
         c.name = index_col (@full_table_name, i.index_id,  9) or
         c.name = index_col (@full_table_name, i.index_id, 10) or
         c.name = index_col (@full_table_name, i.index_id, 11) or
         c.name = index_col (@full_table_name, i.index_id, 12) or
         c.name = index_col (@full_table_name, i.index_id, 13) or
         c.name = index_col (@full_table_name, i.index_id, 14) or
         c.name = index_col (@full_table_name, i.index_id, 15) or
         c.name = index_col (@full_table_name, i.index_id, 16))
    order by 1, 2, 3, 5

GO

if object_id('sp_PurchaseReturnBPULookup') is not null
	drop procedure sp_PurchaseReturnBPULookup
GO
CREATE procedure [dbo].[sp_PurchaseReturnBPULookup] @CompanyCode varchar(10) , @BranchCode varchar(100), @ReturnNo nvarchar(100), @HPPNo nvarchar(100)
 as

select a.BPUNo,a.BPUDate,a.SupplierCode + ' - ' + b.SupplierName AS SupplierCode
from omTrPurchaseBPU a
	left join gnMstSupplier b
		on a.CompanyCode = b.CompanyCode
		and a.SupplierCode = b.SupplierCode
	inner join omTrPurchaseHPPDetail c on a.CompanyCode=c.CompanyCode
		and a.BranchCode=c.BranchCode and c.HPPNo=@HPPNo and a.BPUNo=c.BPUNo
		where not exists (
		select 1 from omTrPurchaseReturnDetail
		where CompanyCode= a.CompanyCode
			and BranchCode= a.BranchCode
			and ReturnNo= @ReturnNo
			and BPUNo= a.BPUNo
	)
order by  BPUNo asc
GO

if object_id('sp_RefferenceTypeLookup') is not null
	drop procedure sp_RefferenceTypeLookup
GO

CREATE procedure [dbo].[sp_RefferenceTypeLookup]  @CompanyCode varchar(10)
 as
SELECT DISTINCT  * 
FROM (
	SELECT distinct(a.RefferenceType)  AS RefferenceType
	FROM dbo.omMstRefference a
	WHERE a.CompanyCode = '6006410'
	UNION
	SELECT distinct(b.RefferenceCode)  AS RefferenceType
	FROM dbo.omMstRefference b
	WHERE b.CompanyCode = '6006410' AND b.RefferenceType = 'REFF'
	) tab
ORDER BY RefferenceType ASC




GO

if object_id('sp_select4LookupCustomer') is not null
	drop procedure sp_select4LookupCustomer
GO
CREATE PROCEDURE [dbo].sp_Select4LookupCustomer (@CompanyCode varchar(10) , @BranchCode varchar(10),
	@ProfitCenterCode varchar(10), @TOPC varchar(10))
AS
 SELECT TableA.CustomerCode, TableA.CustomerName, TableA.TopCode, TableA.TOPCD, TableA.GroupPriceCode, TableA.RefferenceDesc1 as GroupPriceDesc
  FROM    (SELECT a.CustomerCode, a.CustomerName, 
                  a.Address1 + ' ' + a.Address2 + ' ' + a.Address3 + ' ' + a.Address4 as Address,
                  b.TOPCode + '||'
                  + (SELECT c.LookUpValueName
                  FROM gnMstLookUpDtl c
                  WHERE c.CodeID = @TOPC
                  AND c.LookUpValue = b.TOPCode)  AS TopCode, b.TOPCode  AS
                  TOPCD, b.CreditLimit, a.CompanyCode, b.BranchCode, b.
                  ProfitCenterCode, b.GroupPriceCode, c.RefferenceDesc1
             FROM gnMstCustomer a
            LEFT JOIN gnMstCustomerProfitCenter b ON b.CompanyCode = a.CompanyCode AND b.CustomerCode = a.CustomerCode
            LEFT JOIN omMstRefference c ON a.CompanyCode = c.CompanyCode AND c.RefferenceType = 'GRPR' AND c.RefferenceCode = b.GroupPriceCode
            WHERE a.CompanyCode = b.CompanyCode
                  AND a.CompanyCode = b.CompanyCode
                  AND b.CompanyCode = @CompanyCode
                  AND b.BranchCode = @BranchCode
                  AND b.ProfitCenterCode = @ProfitCenterCode
                  AND a.Status = '1'
                  AND b.SalesType = '0'
                  AND b.isBlackList = 0
                  AND b.CreditLimit > 0) TableA
       LEFT JOIN
          gnTrnBankBook c
       ON TableA.CompanyCode = c.CompanyCode
          AND TableA.BranchCode = c.BranchCode
          AND TableA.ProfitCenterCode = c.ProfitCenterCode
          AND TableA.CustomerCode = c.CustomerCode
 WHERE TableA.CreditLimit >
          (ISNULL (c.SalesAmt, 0) - ISNULL (c.ReceivedAmt, 0))
ORDER BY TableA.CustomerCode ASC
GO

if object_id('sp_Select4LookupCustomer2') is not null
	drop procedure sp_Select4LookupCustomer2
GO
CREATE PROCEDURE [dbo].sp_Select4LookupCustomer2 (@CompanyCode varchar(10) , @BranchCode varchar(10),
	@ProfitCenterCode varchar(10), @TOPC varchar(10))
AS
	SELECT a.CustomerCode
		, a.CustomerName
		, a.Address1 + ' ' + a.Address2 + ' ' + a.Address3 + ' ' + a.Address4 as Address
		, ISNULL(c.LookUpValueName,'') AS TopCode
		, b.TopCode as TOPCD
		, b.GroupPriceCode
		, d.RefferenceDesc1 as GroupPriceDesc
		, b.SalesCode
	FROM gnMstCustomer a
	LEFT JOIN gnMstCustomerProfitCenter b ON b.CompanyCode = a.CompanyCode 
		AND b.CustomerCode = a.CustomerCode
	LEFT JOIN gnMstLookUpDtl c ON c.CompanyCode=a.CompanyCode
		AND c.LookUpValue = b.TOPCode    
		AND c.CodeID = @TOPC
	LEFT JOIN omMstRefference d ON a.CompanyCode = d.CompanyCode 
		AND d.RefferenceType = 'GRPR' 
		AND d.RefferenceCode = b.GroupPriceCode
	WHERE a.CompanyCode = @CompanyCode
		AND b.BranchCode = @BranchCode
		AND b.ProfitCenterCode = @ProfitCenterCode
		AND a.Status = '1'
		AND b.SalesType = '1'
		AND b.isBlackList = 0
	ORDER BY CustomerCode;
GO

if object_id('sp_spCategoryCodeview') is not null
	drop procedure sp_spCategoryCodeview
GO
         
		 create procedure [dbo].[sp_spCategoryCodeview] ( @CompanyCode varchar(10))

		 as
		 
		    select 
                a.LookupValue 'CategoryCode', a.LookupValueName 'CategoryName', a.CompanyCode
            from 
                gnMstLookupdtl a
				            where 
                          
                a.CodeID='CSCT'  and a.CompanyCode=@CompanyCode
GO

if object_id('sp_spgnMstAccountView') is not null
	drop procedure sp_spgnMstAccountView
GO

CREATE procedure [dbo].[sp_spgnMstAccountView] (  @CompanyCode varchar(10) ,@BranchCode varchar(10), @Search varchar(50) = '')
 as
 IF @Search <> ''
	SELECT   AccountNo, [Description], CompanyCode,  BranchCode  FROM gnMstAccount 
	where CompanyCode=@CompanyCode and BranchCode=@BranchCode
	and (AccountNo like '%' + @Search + '%' or [Description] like '%' + @Search + '%')

 ELSE
	SELECT   AccountNo, [Description], CompanyCode,  BranchCode  FROM gnMstAccount 
	where CompanyCode=@CompanyCode and BranchCode=@BranchCode





GO

if object_id('sp_SpItemPriceView') is not null
	drop procedure sp_SpItemPriceView
GO

CREATE procedure [dbo].[sp_SpItemPriceView] (  @CompanyCode varchar(10) ,@BranchCode varchar(10))
 as
SELECT 
 Items.CompanyCode 
 ,Items.BranchCode
 ,ItemInfo.PartNo
,ItemInfo.PartName
,ItemPrice.PurchasePrice
,ItemInfo.SupplierCode
,ItemPrice.RetailPriceInclTax
,CASE ItemInfo.IsGenuinePart WHEN 1 THEN 'Ya' ELSE 'Tidak' END AS IsGenuinePart
,Items.ProductType
,Items.PartCategory
,u.LookupValueName 
 as CategoryName
 ,ItemPrice.CostPrice
 ,ItemPrice.RetailPrice
 ,ItemPrice.LastPurchaseUpdate
 ,ItemPrice.LastRetailPriceUpdate
,ItemPrice.OldCostPrice
,ItemPrice.OldPurchasePrice
,ItemPrice.OldRetailPrice

FROM spMstItemPrice ItemPrice 
INNER JOIN spMstItems Items 
    ON ItemPrice.CompanyCode=Items.CompanyCode 
    AND ItemPrice.BranchCode=Items.BranchCode
    AND ItemPrice.PartNo=Items.PartNo
right JOIN spMstItemInfo ItemInfo 
    ON ItemPrice.CompanyCode = ItemInfo.CompanyCode 
    AND ItemPrice.PartNo = ItemInfo.PartNo
	inner join  gnMstLookUpDtl u on (Items.PartCategory =u.ParaValue)
WHERE  u.CodeID='PRCT' 
and Items.CompanyCode=@CompanyCode and Items.BranchCode=@BranchCode





GO

if object_id('sp_SpMasteritemStockAlokasiView') is not null
	drop procedure sp_SpMasteritemStockAlokasiView
GO

create procedure [dbo].[sp_SpMasteritemStockAlokasiView] (  @CompanyCode varchar(10) ,@BranchCode varchar(10))
 as
SELECT  
 
 distinct
--StockAlokasi
d.PartNo
,d.WarehouseCode
,a.LookUpValueName as 'WarehouseName'
,d.LocationCode
,d.OnHand
,d.AllocationSP
,d.AllocationSR
,d.AllocationSL

,d.CompanyCode 
,d.BranchCode 
FROM  spMstItemLoc d 
inner join  gnMstLookUpDtl a on (d.WarehouseCode=a.ParaValue)

WHERE a.CodeID='WRCD' 
 
and a.CompanyCode=@CompanyCode and d.BranchCode=@BranchCode





GO

if object_id('sp_SpMasteritemview') is not null
	drop procedure sp_SpMasteritemview
GO

create procedure [dbo].[sp_SpMasteritemview] (  @CompanyCode varchar(10) ,@BranchCode varchar(10))
 as


 SELECT  
 a.PartNo 
,a.ProductType
,a.PartCategory
,u.LookupValueName  as CategoryName
,a.MovingCode
,a.OnHand - (a.AllocationSL + a.AllocationSP + a.AllocationSR + a.ReservedSL + a.ReservedSP + a.ReservedSR)  AS AvailableItems
,b.PartName
,b.IsGenuinePart
,b.OrderUnit
,b.SupplierCode
,c.SupplierName
,a.BornDate
,a.Status

,a.SalesUnit
,a.PurcDiscPct
,a.LastPurchaseDate
,b.DiscPct
,a.LastDemandDate
--,a.ProductTypedesc
,a.LastSalesDate
,a.ABCClass
,b.UOMCode
,a.DemandAverage

,a.Utility1
,a.Utility2
,a.Utility3
,a.Utility4


--ItemInventory
,a.OnOrder
,a.InTransit
,a.BorrowQty
,a.BorrowedQty
,a.BackOrderSR
,a.ReservedSR
,a.BackOrderSP
,a.ReservedSP
,a.BackOrderSL
,a.ReservedSL

--StockAlokasi
,d.WarehouseCode
--,a.WarehouseName
,d.LocationCode
,d.OnHand
,d.AllocationSP
,d.AllocationSR
,d.AllocationSL


--HPemPenj
--HargaSupplier
--PotonganBeli
--PotonganBelipercen
--HargaBeli
--HargaJual
--HargaJualPPn
--AvarageCost

--OrderParam
,a.OrderPointQty
,a.OrderCycle
,a.SafetyStock
,a.LeadTime





,a.CompanyCode 
,d.BranchCode 
FROM SpMstItems a
INNER JOIN SpMstItemInfo b   ON a.CompanyCode  = b.CompanyCode                          
                         AND a.PartNo = b.PartNo
LEFT JOIN GnMstSupplier c ON c.CompanyCode  = a.CompanyCode 
                         AND c.SupplierCode = b.SupplierCode
INNER JOIN spMstItemLoc d ON a.CompanyCode  = d.CompanyCode   
                         AND a.BranchCode = d.BranchCode                       
                         AND a.PartNo = d.PartNo
						 inner join  gnMstLookUpDtl u on (a.PartCategory =u.ParaValue)
WHERE  u.CodeID='PRCT'
and a.Status > 0
 
and a.CompanyCode=@CompanyCode and a.BranchCode=@BranchCode





GO

if object_id('sp_spmasterpart') is not null
	drop procedure sp_spmasterpart
GO
CREATE procedure [dbo].[sp_spmasterpart] (  @CompanyCode varchar(10) ,@BranchCode varchar(10),@TypeOfGoods varchar(10),@ProductType varchar(10))
as
            SELECT TOP 1500
             ItemPrice.PartNo
            ,ItemInfo.PartName,ItemInfo.SupplierCode,ItemInfo.ProductType,ItemPrice.LastPurchaseUpdate,ItemPrice.LastRetailPriceUpdate
            ,ItemPrice.PurchasePrice
            ,ItemPrice.RetailPriceInclTax
            ,CASE ItemInfo.IsGenuinePart WHEN 1 THEN 'Ya' ELSE 'Tidak' END AS IsGenuinePart
			,ItemPrice.OldPurchasePrice
			,ItemPrice.OldCostPrice
			,ItemPrice.OldRetailPrice
			,OldRetailPrice
            ,Items.PartCategory
            ,(SELECT LookupValueName 
                FROM gnMstLookupDtl 
                WHERE CodeID = 'PRCT' AND 
                LookUpValue = Items.PartCategory AND 
                CompanyCode ='6004001') CategoryName
            FROM spMstItemPrice ItemPrice 
            INNER JOIN spMstItems Items 
                ON ItemPrice.CompanyCode=Items.CompanyCode 
                AND ItemPrice.BranchCode=Items.BranchCode
                AND ItemPrice.PartNo=Items.PartNo
            INNER JOIN spMstItemInfo ItemInfo 
                ON ItemPrice.CompanyCode = ItemInfo.CompanyCode 
                AND ItemPrice.PartNo = ItemInfo.PartNo
            WHERE ItemPrice.CompanyCode=@CompanyCode
                AND ItemPrice.BranchCode= @BranchCode
                AND Items.TypeOfGoods=@TypeOfGoods
                AND Items.ProductType= @ProductType
GO

if object_id('sp_spMasterPartSelect4Lookup') is not null
	drop procedure sp_spMasterPartSelect4Lookup
GO

CREATE PROCEDURE [dbo].[sp_spMasterPartSelect4Lookup] ( 
	@CompanyCode varchar(15)
	,@BranchCode varchar(15)
	,@TypeOfGoods varchar(15)
	,@ProductType varchar(15)
)
AS
	SELECT 
	 Items.PartNo
	,Items.ProductType
	,(SELECT LookupValueName 
		FROM gnMstLookupDtl 
	   WHERE CodeID = 'PRCT' AND 
			 LookUpValue = Items.PartCategory AND 
			 CompanyCode = @CompanyCode) AS CategoryName
	,Items.PartCategory
	,ItemInfo.PartName
	,CASE ItemInfo.IsGenuinePart WHEN 1 THEN 'Ya' ELSE 'Tidak' END AS IsGenuinePart
	,CASE Items.Status WHEN 1 THEN 'Aktif' ELSE 'Tidak' END AS IsActive
	,ItemInfo.OrderUnit
	,ItemInfo.SupplierCode
	,Supplier.SupplierName
	,(SELECT LookupValueName 
		FROM gnMstLookupDtl 
	  WHERE CodeID = 'TPGO' AND 
			LookUpValue = Items.TypeOfGoods AND 
			CompanyCode = @CompanyCode) AS TypeOfGoods
	FROM SpMstItems Items
	INNER JOIN SpMstItemInfo ItemInfo   ON Items.CompanyCode  = ItemInfo.CompanyCode                          
							 AND Items.PartNo = ItemInfo.PartNo
	LEFT JOIN GnMstSupplier Supplier ON Supplier.CompanyCode  = Items.CompanyCode 
							 AND Supplier.SupplierCode = ItemInfo.SupplierCode
	WHERE Items.CompanyCode = @CompanyCode
	  AND Items.BranchCode  = @BranchCode    
	  AND Items.TypeOfGoods = @TypeOfGoods
	  AND Items.ProductType = @ProductType
GO

if object_id('sp_spmasterpartview') is not null
	drop procedure sp_spmasterpartview
GO

CREATE procedure [dbo].[sp_spmasterpartview] (  @CompanyCode varchar(10) ,@BranchCode varchar(10))
 as

 select a.CompanyCode,ItemPrice.BranchCode,a.PartNo,a.PartName,a.SupplierCode,gnMstSupplier.SupplierName,
a.IsGenuinePart,a.ProductType,a.PartCategory ,u.LookUpValueName as CategoryName
,a.OrderUnit as 'FromQty'
,ItemPrice.PurchasePrice
,ItemPrice.RetailPriceInclTax
 ,ItemPrice.CostPrice
 ,ItemPrice.RetailPrice
 ,ItemPrice.LastPurchaseUpdate
 ,ItemPrice.LastRetailPriceUpdate
,ItemPrice.OldCostPrice
,ItemPrice.OldPurchasePrice
,ItemPrice.OldRetailPrice

from SpMstItemInfo a 
inner join  gnMstLookUpDtl u on (a.PartCategory =u.ParaValue)
left join  spMstItemPrice ItemPrice       ON ItemPrice.CompanyCode=a.CompanyCode 
    AND ItemPrice.PartNo=a.PartNo 
	 LEFT JOIN gnMstSupplier ON gnMstSupplier.SupplierCode = a.SupplierCode AND
        gnMstSupplier.CompanyCode = a.CompanyCode
WHERE  u.CodeID='PRCT'  and a.Status > 0  

and a.CompanyCode=@CompanyCode and ItemPrice.BranchCode=@BranchCode





GO

if object_id('sp_spMstAccountView') is not null
	drop procedure sp_spMstAccountView
GO

create procedure [dbo].[sp_spMstAccountView] (  @CompanyCode varchar(10) ,@BranchCode varchar(10))
 as

 SELECT a.TypeOfGoods, 
                           b.LookUpValueName as NameOfGoods,  
                           a.SalesAccNo,  a.COGSAccNo,  a.InventoryAccNo,  a.DiscAccNo,  a.ReturnAccNo, 
                            a.ReturnPybAccNo,  a.OtherIncomeAccNo,  a.OtherReceivableAccNo,  a.InTransitAccNo, a.CompanyCode, a.BranchCode
                    FROM   spMstAccount a 
					inner join gnMstLookUpDtl b on b.LookUpValue=a.TypeOfGoods
                    WHERE  B.CodeID='TPGO' 

and a.CompanyCode=@CompanyCode and a.BranchCode=@BranchCode





GO

if object_id('sp_spMstCompanyAccount') is not null
	drop procedure sp_spMstCompanyAccount
GO
create procedure  [dbo].[sp_spMstCompanyAccount]  (  @CompanyCode varchar(10)  )
as
select 
CompanyCode,
CompanyCodeTo,
CompanyCodeToDesc,
BranchCodeTo,
BranchCodeToDesc,
WarehouseCodeTo,
WarehouseCodeToDesc,
UrlAddress,
isActive
from spMstCompanyAccount
where CompanyCode= @CompanyCode
GO

if object_id('sp_SpMstItemcondition') is not null
	drop procedure sp_SpMstItemcondition
GO
     CREATE procedure [dbo].[sp_SpMstItemcondition] (  @CompanyCode varchar(10) )  
	 as
                SELECT 
                    a.PartNo, a.NewPartNo, a.InterchangeCode, a.ProductType, a.PartCategory, a.UnitConversion,EndMark , a.CompanyCode
                FROM 
                    spMstItemMod a 
                WHERE a.CompanyCode=@CompanyCode
                    ORDER BY a.PartNo, a.NewPartNo
             
GO

if object_id('sp_SpMstItemConversionview') is not null
	drop procedure sp_SpMstItemConversionview
GO
CREATE procedure [dbo].[sp_SpMstItemConversionview]    (  @CompanyCode varchar(10)  )
as
select   a.CompanyCode,a.PartNo,a.ProductType, b.PartName, FromQty, ToQty ,IsActive
from SpMstItemConversion a
left join spMstItemInfo b on
    a.CompanyCode = b.CompanyCode
    and a.ProductType = b.ProductType
    and a.PartNo = b.PartNo
 where   a.CompanyCode=@CompanyCode

GO

if object_id('sp_SpMstItemLocItemLookupView') is not null
	drop procedure sp_SpMstItemLocItemLookupView
GO

create procedure [dbo].[sp_SpMstItemLocItemLookupView] (  @CompanyCode varchar(10) ,@BranchCode varchar(10))
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
	ItemPrice.RetailPrice,
	ItemPrice.RetailPriceInclTax,
	ItemPrice.PurchasePrice
	,Items.CompanyCode
	,Items.BranchCode   
	,Items.ProductType
	,Items.TypeOfGoods
FROM SpMstItems Items
INNER JOIN SpMstItemInfo ItemInfo ON Items.CompanyCode  = ItemInfo.CompanyCode                          
                         AND Items.PartNo = ItemInfo.PartNo
INNER JOIN spMstItemPrice ItemPrice ON Items.CompanyCode = ItemPrice.CompanyCode
                        AND Items.BranchCode = ItemPrice.BranchCode AND Items.PartNo = ItemPrice.PartNo
INNER JOIN spMstItemLoc ItemLoc ON Items.CompanyCode = ItemLoc.CompanyCode AND Items.BranchCode = ItemLoc.BranchCode
                        AND Items.PartNo = ItemLoc.PartNo
WHERE Items.Status > 0
  AND ItemLoc.WarehouseCode = '00'

and Items.CompanyCode=@CompanyCode and Items.BranchCode=@BranchCode





GO

if object_id('sp_SpMstItemLocView') is not null
	drop procedure sp_SpMstItemLocView
GO

create procedure [dbo].[sp_SpMstItemLocView] (  @CompanyCode varchar(10) ,@BranchCode varchar(10))
 as
SELECT 
	 ItemLoc.PartNo
	,ItemInfo.PartName
	,ItemInfo.SupplierCode
	,ItemLoc.WarehouseCode
	,ItemLoc.LocationCode
	,Items.PartCategory
	,Items.CompanyCode
	,Items.BranchCode   
	,Items.ProductType
	,Items.TypeOfGoods
FROM spMstItemLoc ItemLoc
INNER JOIN spMstItems Items 
    ON ItemLoc.CompanyCode=Items.CompanyCode
    AND ItemLoc.BranchCode=Items.BranchCode
    AND ItemLoc.PartNo=Items.PartNo
INNER JOIN spMstItemInfo ItemInfo 
    ON ItemLoc.CompanyCode=ItemInfo.CompanyCode
    AND ItemLoc.PartNo=ItemInfo.PartNo
WHERE
	ItemLoc.WarehouseCode NOT LIKE 'X%'


and Items.CompanyCode=@CompanyCode and Items.BranchCode=@BranchCode





GO

if object_id('sp_SpMstItemModifInfo') is not null
	drop procedure sp_SpMstItemModifInfo
GO
CREATE procedure [dbo].[sp_SpMstItemModifInfo]  (  @CompanyCode varchar(10))
 as

SELECT distinct
                     ItemInfo.PartNo
                    ,ItemInfo.ProductType
					   ,(SELECT LookupValueName 
                        FROM gnMstLookupDtl 
                       WHERE CodeID = 'PRDT' AND 
                             LookUpValue = ItemInfo.ProductType AND 
                             CompanyCode = @CompanyCode) AS ProductTypeName
                    ,(SELECT LookupValueName 
                        FROM gnMstLookupDtl 
                       WHERE CodeID = 'PRCT' AND 
                             LookUpValue = ItemInfo.PartCategory AND 
                             CompanyCode = @CompanyCode) AS CategoryName
                    ,ItemInfo.PartCategory
                    ,ItemInfo.PartName
                    ,CASE ItemInfo.IsGenuinePart WHEN 1 THEN 'Ya' ELSE 'Tidak' END AS IsGenuinePart
                    ,ItemInfo.OrderUnit
                    ,ItemInfo.SupplierCode
                    ,Supplier.SupplierName ,ItemInfo.CompanyCode 
                FROM SpMstItemInfo ItemInfo
                LEFT JOIN GnMstSupplier Supplier ON 
                    Supplier.CompanyCode  = ItemInfo.CompanyCode 
                AND Supplier.SupplierCode = ItemInfo.SupplierCode

				where  ItemInfo.CompanyCode= @CompanyCode
				
     
                 
GO

if object_id('sp_SpMstItemModifSelect') is not null
	drop procedure sp_SpMstItemModifSelect
GO
CREATE procedure [dbo].[sp_SpMstItemModifSelect]  (  @CompanyCode varchar(10),@PartNo varchar(50))
 as

SELECT 
                     ItemInfo.PartNo
                    ,ItemInfo.ProductType
					   ,(SELECT LookupValueName 
                        FROM gnMstLookupDtl 
                       WHERE CodeID = 'PRDT' AND 
                             LookUpValue = ItemInfo.ProductType AND 
                             CompanyCode = @CompanyCode) AS ProductTypeName
                    ,(SELECT LookupValueName 
                        FROM gnMstLookupDtl 
                       WHERE CodeID = 'PRCT' AND 
                             LookUpValue = ItemInfo.PartCategory AND 
                             CompanyCode = @CompanyCode) AS CategoryName
                    ,ItemInfo.PartCategory
                    ,ItemInfo.PartName
                    ,CASE ItemInfo.IsGenuinePart WHEN 1 THEN 'Ya' ELSE 'Tidak' END AS IsGenuinePart
                    ,ItemInfo.OrderUnit
                    ,ItemInfo.SupplierCode
                    ,Supplier.SupplierName ,ItemInfo.CompanyCode 
                FROM SpMstItemInfo ItemInfo
                LEFT JOIN GnMstSupplier Supplier ON 
                    Supplier.CompanyCode  = ItemInfo.CompanyCode 
                AND Supplier.SupplierCode = ItemInfo.SupplierCode

				where  ItemInfo.CompanyCode= @CompanyCode
				and ItemInfo.PartNo=@PartNo
     
                 
GO

if object_id('sp_spMstMovingCodeView') is not null
	drop procedure sp_spMstMovingCodeView
GO

CREATE procedure [dbo].[sp_spMstMovingCodeView]  (@CompanyCode varchar(15))
 as
SELECT MovingCode, MovingCodeName,
Formula = Cast(Param1 as varchar(10)) + Sign1 + Variable + Sign2 + Cast(Param2 as varchar(10)),
Param1, Sign1, Variable, Param2, Sign2,CompanyCode
FROM spMstMovingCode
where CompanyCode=@CompanyCode







GO

if object_id('sp_spMstOrderParamLookup') is not null
	drop procedure sp_spMstOrderParamLookup
GO
       create procedure [dbo].[sp_spMstOrderParamLookup] (  @CompanyCode varchar(10) ,@BranchCode varchar(10),@SupplierCode varchar(20))
 as   
		    SELECT distinct a.SupplierCode,b.SupplierName,
                        a.MovingCode,
						c.MovingCodeName,
                   a.LeadTime, 
                   a.OrderCycle, 
                  a.SafetyStock,
				    a.CompanyCode,
					a.BranchCode
            FROM spMstOrderParam a
			inner join gnMstSupplier b on  b.CompanyCode=a.CompanyCode AND
                      b.SupplierCode=a.SupplierCode
					  inner join SpMstMovingCode c on                      
					   c.CompanyCode=a.CompanyCode AND
                      c.MovingCode=a.MovingCode
 

            WHERE a.CompanyCode=@CompanyCode
            AND a.BranchCode=@BranchCode and a.SupplierCode=@SupplierCode
GO

if object_id('sp_spMstOrderParamView') is not null
	drop procedure sp_spMstOrderParamView
GO
       create procedure [dbo].[sp_spMstOrderParamView] (  @CompanyCode varchar(10) ,@BranchCode varchar(10))
 as   
		    SELECT a.SupplierCode,b.SupplierName,
                        a.MovingCode,
						c.MovingCodeName,
                   a.LeadTime, 
                   a.OrderCycle, 
                  a.SafetyStock,
				    a.CompanyCode,
					a.BranchCode
            FROM spMstOrderParam a
			inner join gnMstSupplier b on  b.CompanyCode=a.CompanyCode AND
                      b.SupplierCode=a.SupplierCode
					  inner join SpMstMovingCode c on                      
					   c.CompanyCode=a.CompanyCode AND
                      c.MovingCode=a.MovingCode
 

            WHERE a.CompanyCode=@CompanyCode
            AND a.BranchCode=@BranchCode
GO

if object_id('sp_spMstPurchCampaignView') is not null
	drop procedure sp_spMstPurchCampaignView
GO

create procedure [dbo].[sp_spMstPurchCampaignView] (  @CompanyCode varchar(10) ,@BranchCode varchar(10))
 as
SELECT 
spMstPurchCampaign.PartNo, 
spMstItemInfo.PartName, 
spMstPurchCampaign.DiscPct, 
spMstPurchCampaign.BegDate,
spMstPurchCampaign.EndDate,
spMstPurchCampaign.SupplierCode, 
gnMstSupplier.SupplierName,
 spMstPurchCampaign.CompanyCode,
  spMstPurchCampaign.BranchCode
  FROM spMstPurchCampaign
  LEFT JOIN spMstItemInfo ON spMstItemInfo.PartNo = spMstPurchCampaign.PartNo AND	
        spMstItemInfo.CompanyCode = spMstPurchCampaign.CompanyCode
  LEFT JOIN gnMstSupplier ON gnMstSupplier.SupplierCode = spMstPurchCampaign.SupplierCode AND
        gnMstSupplier.CompanyCode = spMstPurchCampaign.CompanyCode
  INNER JOIN spMstItems ON spMstItems.CompanyCode=spMstPurchCampaign.CompanyCode AND 
	      spMstItems.BranchCode=spMstPurchCampaign.BranchCode AND 
				spMstItems.PartNo=spMstPurchCampaign.PartNo
where  spMstPurchCampaign.CompanyCode=@CompanyCode and spMstPurchCampaign.BranchCode=@BranchCode





GO

if object_id('sp_spMstSalesCampaignView') is not null
	drop procedure sp_spMstSalesCampaignView
GO

create procedure [dbo].[sp_spMstSalesCampaignView] (  @CompanyCode varchar(10) ,@BranchCode varchar(10))
 as
SELECT 
spMstSalesCampaign.PartNo, 
spMstItemInfo.PartName, 
spMstSalesCampaign.DiscPct, 
spMstSalesCampaign.BegDate,
spMstSalesCampaign.EndDate,
spMstSalesCampaign.SupplierCode, 
gnMstSupplier.SupplierName,
 spMstSalesCampaign.CompanyCode,
  spMstSalesCampaign.BranchCode
  FROM spMstSalesCampaign
  LEFT JOIN spMstItemInfo ON spMstItemInfo.PartNo = spMstSalesCampaign.PartNo AND spMstItemInfo.CompanyCode = spMstSalesCampaign.CompanyCode
  LEFT JOIN gnMstSupplier ON gnMstSupplier.SupplierCode = spMstItemInfo.SupplierCode AND gnMstSupplier.CompanyCode = spMstItemInfo.CompanyCode
  INNER JOIN spMstItems ON spMstItems.CompanyCode=spMstSalesCampaign.CompanyCode AND spMstItems.BranchCode=spMstSalesCampaign.BranchCode 
        AND spMstItems.PartNo=spMstSalesCampaign.PartNo
 
where  spMstSalesCampaign.CompanyCode=@CompanyCode and spMstSalesCampaign.BranchCode=@BranchCode





GO

if object_id('sp_spMstSalesTargetDetil') is not null
	drop procedure sp_spMstSalesTargetDetil
GO
CREATE procedure [dbo].[sp_spMstSalesTargetDetil] (
	@CompanyCode varchar(10),
	@BranchCode varchar(10)
)
as
select 
	a.CompanyCode,
	a.BranchCode,
	a.[Year],
	a.[Month],
	a.CategoryCode,
	(select LookUpValueName from gnMstLookUpDtl where CodeID='CSCT' and CompanyCode= a.CompanyCode and LookUpValue = a.CategoryCode) CategoryName,
	a.QtyTarget,
	a.AmountTarget,
	a.TotalJaringan
from spMstSalesTargetDtl a
WHERE a.CompanyCode=@CompanyCode and a.BranchCode=@BranchCode

GO

if object_id('sp_spMstSalesTargetDtl') is not null
	drop procedure sp_spMstSalesTargetDtl
GO
CREATE procedure [dbo].[sp_spMstSalesTargetDtl]  (  @CompanyCode varchar(10) ,@BranchCode varchar(10),@Year Varchar(4),@Month varchar(2))
as
select 
a.CompanyCode,
a.BranchCode,
 a.[Year],
a.[Month],
a.CategoryCode,
   b.LookUpValueName  CategoryName,
a.QtyTarget,
a.AmountTarget,
a.TotalJaringan
from spMstSalesTargetDtl a
inner join gnMstLookUpDtl b on 
a.CompanyCode=b.CompanyCode 
and a.CategoryCode=b.LookUpValue
WHERE  b.CodeID='CSCT' and a.CompanyCode=@CompanyCode and  a.BranchCode=@BranchCode
and a.[Year]=@Year and a.[Month]=@Month
GO

if object_id('sp_spMstSalesTargetDtlSum') is not null
	drop procedure sp_spMstSalesTargetDtlSum
GO
CREATE procedure [dbo].[sp_spMstSalesTargetDtlSum]  (  @CompanyCode varchar(10) ,@BranchCode varchar(10),@Year Varchar(4),@Month varchar(2))
as
select 

sum(a.QtyTarget) as sumQtyTarget,
sum(a.AmountTarget) as sumAmountTarget,
sum(a.TotalJaringan) as sumTotalJaringan
from spMstSalesTargetDtl a
 WHERE a.CompanyCode=@CompanyCode and  a.BranchCode=@BranchCode
and a.[Year]=@Year and a.[Month]=@Month
GO

if object_id('sp_spMstSalesTargetview') is not null
	drop procedure sp_spMstSalesTargetview
GO

CREATE procedure [dbo].[sp_spMstSalesTargetview] (  @CompanyCode varchar(10) ,@BranchCode varchar(10))
 as
SELECT  CompanyCode,BranchCode,  [Year]  , 
                               [Month]  , QtyTarget, AmountTarget, TotalJaringan 
                                FROM spMstSalesTarget
WHERE  CompanyCode=@CompanyCode and  BranchCode=@BranchCode





GO

if object_id('sp_SpPosLkp') is not null
	drop procedure sp_SpPosLkp
GO

create procedure sp_SpPosLkp
@CompanyCode varchar(15),  
@BranchCode varchar(15),   
@TypeOfGoods  varchar(15)
as

SELECT 
 a.POSNo
,a.PosDate
,ISNULL(a.IsDeleted, 0) IsDeleted
,a.SupplierCode
,b.SupplierName
FROM spTrnPPOSHdr a
INNER JOIN gnMstSupplier b ON b.SupplierCode = a.SupplierCode and b.CompanyCode = a.CompanyCode
WHERE a.CompanyCode=@CompanyCode 
	  AND a.BranchCode=@BranchCode
      AND a.IsDeleted=0 
      AND TypeOfGoods = @TypeOfGoods
ORDER BY a.POSNo ASC
        

GO

if object_id('sp_SpReceiveClaimNo') is not null
	drop procedure sp_SpReceiveClaimNo
GO


CREATE procedure [dbo].[sp_SpReceiveClaimNo] (  

@CompanyCode varchar(10) ,
@BranchCode varchar(10),
@TypeOfGoods varchar(10))

as
SELECT DISTINCT a.CompanyCode, a.BranchCode, a.ClaimNo
,a.ClaimDate , TypeOfGoods            
FROM spTrnPClaimHdr a
INNER JOIN spTrnPClaimDtl b ON b.CompanyCode = a.CompanyCode
AND b.BranchCode = a.BranchCode
AND b.ClaimNo = a.ClaimNo
WHERE a.CompanyCode = @CompanyCode
AND a.BranchCode = @BranchCode   
AND a.Status = '2'
AND a.TypeOfGoods = @TypeOfGoods
AND 
(SELECT ISNULL(SUM(OvertageQty) + SUM(ShortageQty) + SUM(DemageQty) + SUM(WrongQty),0) FROM spTrnPClaimDtl 
WHERE CompanyCode =  @CompanyCode
AND BranchCode = @BranchCode   
AND ClaimNo = a.ClaimNo) >
(SELECT  ISNULL(SUM(c.RcvOvertageQty) + SUM(c.RcvShortageQty) + SUM(c.RcvDamageQty) + SUM(c.RcvWrongQty),0) FROM spTrnPRcvClaimDtl c
INNER JOIN spTrnPRcvClaimHdr b ON c.CompanyCode = b.CompanyCode AND c.BranchCode = b.BranchCode AND c.ClaimNo = b.ClaimNo AND c.ClaimReceivedNo = b.ClaimReceivedNo
WHERE c.CompanyCode =  @CompanyCode
AND c.BranchCode = @BranchCode   
AND c.ClaimNo = a.ClaimNo AND b.Status NOT IN (3))
ORDER BY a.ClaimNo DESC


GO

if object_id('sp_SpSOSelectforEntry') is not null
	drop procedure sp_SpSOSelectforEntry
GO

 CREATE PROCEDURE [dbo].[sp_SpSOSelectforEntry]   
@CompanyCode varchar(15),  
@BranchCode varchar(15),   
@ProductType  varchar(15),
@WarehouseCode  varchar(15)   

AS  
 SELECT
 distinct
	    a.PartNo,
	    (SELECT PartName FROM spMstItemInfo WHERE CompanyCode = a.CompanyCode AND PartNo = a.PartNo) AS PartName
    FROM
	    spMstItems a
        INNER JOIN spMstItemLoc b ON a.CompanyCode = b.CompanyCode
            AND a.BranchCode = b.BranchCode
            AND a.PartNo = b.PartNo
    WHERE
	    a.CompanyCode = @CompanyCode
	    AND a.BranchCode = @BranchCode	
	    AND a.ProductType = @ProductType
        AND b.WarehouseCode = @WarehouseCode
    ORDER BY
        PartNo ASC
        
        
GO

if object_id('sp_SpSOSelectforInsert') is not null
	drop procedure sp_SpSOSelectforInsert
GO

CREATE procedure [dbo].[sp_SpSOSelectforInsert] (  @CompanyCode varchar(10) ,@BranchCode varchar(10),@LocationCode varchar(10))
 as
SELECT 
	 ItemLoc.PartNo	
	,ItemLoc.WarehouseCode
	,ItemLoc.LocationCode
	,Items.PartCategory
	,Items.CompanyCode
	,Items.BranchCode   
	,Items.ProductType
	,Items.TypeOfGoods
	,itemloc.OnHand
FROM spMstItemLoc ItemLoc
INNER JOIN spMstItems Items 
    ON ItemLoc.CompanyCode=Items.CompanyCode
    AND ItemLoc.BranchCode=Items.BranchCode
    AND ItemLoc.PartNo=Items.PartNo
WHERE	
	 Items.CompanyCode=@CompanyCode and Items.BranchCode=@BranchCode and ItemLoc.LocationCode like '%' +@LocationCode

GO

if object_id('sp_SpSOSelectforLookup') is not null
	drop procedure sp_SpSOSelectforLookup
GO

CREATE procedure [dbo].[sp_SpSOSelectforLookup] (  @CompanyCode varchar(10) ,@BranchCode varchar(10))  
 as  
select distinct(a.STNo)  
  , b.WarehouseCode  
  , b.STHdrNo  
  --, a.PartNo  
  from spTrnStockTakingTemp a with(nolock, nowait)  
       left join spTrnStockTakingHdr b with(nolock, nowait) on a.CompanyCode = b.CompanyCode  
   and a.Branchcode = b.Branchcode   
   and a.STHdrNo = b.STHdrNo   
 where a.status IN ('0', '1')  
   and b.status < 2  
   and a.CompanyCode = @CompanyCode  
   and a.Branchcode = @BranchCode
GO

if object_id('sp_SpSrvLampLkp') is not null
	drop procedure sp_SpSrvLampLkp
GO

CREATE procedure [dbo].[sp_SpSrvLampLkp]
@CompanyCode varchar(15),  
@BranchCode varchar(15),   
@SalesType char(1)

--exec sp_SpSrvLampLkp '6006410','600641001',1
as
SELECT
    DISTINCT LmpNo
    , LmpDate
    , (SELECT TOP 1 DocNo FROM spTrnSPickingDtl
		WHERE spTrnSPickingDtl.CompanyCode = spTrnSLmpHdr.CompanyCode
			AND spTrnSPickingDtl.BranchCode = spTrnSLmpHdr.BranchCode
			AND spTrnSPickingDtl.PickingSlipNo = spTrnSLmpHdr.PickingSlipNo
		) AS SSNo
	, (SELECT TOP 1 DocDate FROM spTrnSPickingDtl
		WHERE spTrnSPickingDtl.CompanyCode = spTrnSLmpHdr.CompanyCode
			AND spTrnSPickingDtl.BranchCode = spTrnSLmpHdr.BranchCode
			AND spTrnSPickingDtl.PickingSlipNo = spTrnSLmpHdr.PickingSlipNo
		) AS SSDate
    , spTrnSLmpHdr.BPSFNo
    , spTrnSLmpHdr.BPSFDate
    , spTrnSLmpHdr.PickingSlipNo
    , spTrnSLmpHdr.PickingSlipDate
FROM 
    spTrnSLmpHdr
    INNER JOIN spTrnSBPSFHdr ON spTrnSBPSFHdr.Companycode = spTrnSLmpHdr.CompanyCode 
            AND spTrnSBPSFHdr.BranchCode = spTrnSLmpHdr.BranchCode 
            AND spTrnSBPSFHdr.BPSFNo = spTrnSLmpHdr.BPSFNo 
WHERE 
    spTrnSLmpHdr.CompanyCode = @CompanyCode
    AND spTrnSLmpHdr.BranchCode = @BranchCode
    AND spTrnSBPSFHdr.SalesType = @SalesType
ORDER BY 
    LmpNo DESC
        
GO

if object_id('sp_UpdateABCClassTuning') is not null
	drop procedure sp_UpdateABCClassTuning
GO





CREATE procedure [dbo].[sp_UpdateABCClassTuning] (  

@CompanyCode varchar(10) ,
@BranchCode varchar(10),
@Year int,
@Month int,
@PID varchar(10))


as

/*SET @Year = '{0}'
SET @Month = '{1}'
SET @CompanyCode = '{2}'
SET @BranchCode = '{3}'
SET @PID = '{4}'*/

SELECT * into #A1 from(
SELECT a.CompanyCode, a.BranchCode, a.PartNo, ISNULL(b.CostPrice, 0) AS HargaPokok, ISNULL(c.SalesQty, 0) AS SalesQty, ISNULL(b.CostPrice * c.SalesQty, 0) as TotalPokok
FROM 
spMstItems a
LEFT JOIN spMstItemPrice b On a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode and a.PartNo = b.PartNo
LEFT JOIN 
(SELECT CompanyCode, BranchCode, PartNo, SalesQty FROM SpHstDemandItem a
  WHERE a.Year  = @Year
    AND a.Month = @Month) c ON a.CompanyCode = c.CompanyCode AND c.BranchCode = b.BranchCode AND a.PartNo = c.PartNo
LEFT JOIN
(SELECT ABCClassAPct, ABCClassBPct, ABCClassCPct, CompanyCode, BranchCode
                        FROM gnMstCoProfileSpare
                        WHERE CompanyCode = @CompanyCode
                        AND BranchCode = @BranchCode) d ON a.CompanyCode = d.CompanyCode AND a.BranchCode = d.BranchCode
WHERE a.CompanyCode = @CompanyCode
    AND a.BranchCode = @BranchCode
) #A1
--==========================================

SELECT partno, TotalPokok, Total into #A2 from 
(
SELECT TOP 1000000000 a.CompanyCode, a.BranchCode, a.PartNo, a.TotalPokok, (SELECT Sum(b.TotalPokok) FROM #A1 b WHERE a.TotalPokok <= b.TotalPokok) AS Total 
FROM #A1 a 
ORDER BY a.TotalPokok DESC
 ) #A2 WHERE TotalPokok > 0
--==========================================

select * into #A3
FROM(
SELECT CompanyCode, BranchCode, SUM(TotalPokok) AS TotalFinal FROM #A1 GROUP BY CompanyCode, BranchCode
) #A3
--==========================================

SELECT * INTO #A4
FROM(
SELECT a.CompanyCode, a.BranchCode, a.PartNo, b.Total, c.TotalFinal,
CASE WHEN c.TotalFinal = 0 THEN 'C' ELSE CASE WHEN ((b.Total/c.TotalFinal) * 100) <= ABCClassAPct THEN 'A' ELSE CASE WHEN ISNULL(((b.Total/c.TotalFinal) * 100),0) > ABCClassAPct AND ((b.Total/c.TotalFinal) * 100) <= ABCClassBPct THEN 'B' ELSE CASE WHEN ISNULL(((b.Total/c.TotalFinal) * 100),0) > ABCClassBPct THEN 'C' END END END END AS ABCClass
FROM #A1 a  
INNER JOIN #A2 b ON  a.PartNo = b.PartNo
LEFT JOIN #A3 c ON  a.CompanyCode = c.CompanyCode AND a.BranchCode = c.BranchCode
LEFT JOIN (SELECT ABCClassAPct, ABCClassBPct, ABCClassCPct, CompanyCode, BranchCode
                        FROM gnMstCoProfileSpare
                        WHERE CompanyCode = @CompanyCode
                        AND BranchCode = @BranchCode) d ON a.CompanyCode = d.CompanyCode AND a.BranchCode = d.BranchCode
)#A4 

--update spMstItems set ABCClass = isnull((select ABCClass from #A4 where PartNo = spMstItems.PartNo and CompanyCode = spMstItems.CompanyCode and BranchCode = spMstItems.BranchCode), spMstItems.ABCClass)
--update spMstItems set LastUpdateBy = @PID, LastUpdateDate = GetDate()

UPDATE spMstItems SET ABCClass = isnull(b.ABCClass, a.ABCClass),
LastUpdateBy  = @PID, 
LastUpdateDate = GetDate() 
FROM spMstItems a, #A4 b
WHERE a.CompanyCode = b.CompanyCode 
AND a.BranchCode = b.BranchCode
AND a.PartNo = b.PartNo

DROP table #A1
DROP table #A2
DROP table #A3
DROP table #A4

GO

if object_id('sp_UpdateBOMTuning') is not null
	drop procedure sp_UpdateBOMTuning
GO






CREATE procedure [dbo].[sp_UpdateBOMTuning] (  

@CompanyCode varchar(10) ,
@BranchCode varchar(10),
@User varchar(10)
)


as

UPDATE spMstItems
SET spMstItems.BOMInvCostPrice= ISNULL((SELECT CostPrice FROM spMstItemPrice WITH(NOWAIT, NOLOCK)
						WHERE spMstItemPrice.CompanyCode=spMstItems.CompanyCode
							AND spMstItemPrice.BranchCode=spMstItems.BranchCode
							AND spMstItemPrice.PartNo=spMstItems.PartNo), spMstItems.BOMInvCostPrice),
	spMstItems.BOMInvQty= spMstItems.OnHand,
	spMstItems.BOMInvAmt= ISNULL((SELECT CostPrice FROM spMstItemPrice WITH(NOWAIT, NOLOCK)
						WHERE spMstItemPrice.CompanyCode=spMstItems.CompanyCode
							AND spMstItemPrice.BranchCode=spMstItems.BranchCode
							AND spMstItemPrice.PartNo=spMstItems.PartNo), spMstItems.BOMInvCostPrice)
                            * spMstItems.OnHand,
	spMstItems.LastUpdateBy=@User,	
	spMstItems.LastUpdateDate=GetDate()	
WHERE spMstItems.CompanyCode=@CompanyCode AND spMstItems.BranchCode=@BranchCode

UPDATE spMstItemLoc
SET spMstItemLoc.BOMInvCostPrice= ISNULL((SELECT CostPrice FROM spMstItemPrice WITH(NOWAIT, NOLOCK) 
						WHERE spMstItemPrice.CompanyCode=spMstItemLoc.CompanyCode
							AND spMstItemPrice.BranchCode=spMstItemLoc.BranchCode
							AND spMstItemPrice.PartNo=spMstItemLoc.PartNo 
							AND spMstItemLoc.WarehouseCode=spMstItemLoc.WarehouseCode), spMstItemLoc.BOMInvCostPrice),
	spMstItemLoc.BOMInvQty= spMstItemLoc.OnHand,
	spMstItemLoc.BOMInvAmount= ISNULL((SELECT CostPrice FROM spMstItemPrice WITH(NOWAIT, NOLOCK) 
						WHERE spMstItemPrice.CompanyCode=spMstItemLoc.CompanyCode
							AND spMstItemPrice.BranchCode=spMstItemLoc.BranchCode
							AND spMstItemPrice.PartNo=spMstItemLoc.PartNo 
							AND spMstItemLoc.WarehouseCode=spMstItemLoc.WarehouseCode), spMstItemLoc.BOMInvCostPrice)
                            * spMstItemLoc.OnHand,
	spMstItemLoc.LastUpdateBy=@User,	
	spMstItemLoc.LastUpdateDate=GetDate()	
WHERE spMstItemLoc.CompanyCode=@CompanyCode AND spMstItemLoc.BranchCode=@BranchCode

GO

if object_id('sp_UpdateBORegisterTuning') is not null
	drop procedure sp_UpdateBORegisterTuning
GO





CREATE procedure [dbo].[sp_UpdateBORegisterTuning] (  

@CompanyCode varchar(10) ,
@BranchCode varchar(10),
@LastUpdateBy varchar(10))


as

UPDATE spMstItems
SET BackOrderSP = 0, BackOrderSR = 0, BackOrderSL = 0
WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode

UPDATE spMstItemLoc
SET BackOrderSP = 0, BackOrderSR = 0, BackOrderSL = 0
WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode

SELECT A.CompanyCode, A.BranchCode, A.WarehouseCode, A.Partno, B.SalesType, SUM(A.QtyBO -(A.QtyBOSupply + A.QtyBOCancel)) as BOAvail 
INTO #TempBo
FROM spTrnSORDDtl A WITH(NOWAIT, NOLOCK) INNER JOIN spTrnSORDHdr B WITH(NOWAIT, NOLOCK) 
ON A.CompanyCode = B.CompanyCode 
AND A.BranchCode = B.BranchCode
AND A.DocNo = B.DocNo
WHERE A.CompanyCode = @CompanyCode 
AND A.BranchCode = @BranchCode
GROUP BY A.CompanyCode, A.BranchCode, WarehouseCode, A.Partno, B.SalesType
HAVING SUM(A.QtyBO -(A.QtyBOSupply + A.QtyBOCancel)) > 0

UPDATE spMstItems 
SET LastUpdateBy = @LastUpdateBy, LastUpdateDate = getDate(),
    BackOrderSP = ISNULL((SELECT SUM(a.BOAvail) from #TempBo a 
    WHERE a.CompanyCode = spMstItems.CompanyCode
    AND a.BranchCode = spMstItems.BranchCode
    AND a.PartNo = spMstItems.PartNo
    AND a.SalesType in ('0','1')), spMstItems.BackOrderSP),
 BackOrderSR = ISNULL((SELECT SUM(a.BOAvail) from #TempBo a 
    WHERE a.CompanyCode = spMstItems.CompanyCode
    AND a.BranchCode = spMstItems.BranchCode
    AND a.PartNo = spMstItems.PartNo
    AND a.SalesType='2'), spMstItems.BackOrderSR),
 BackOrderSL = ISNULL((SELECT SUM(a.BOAvail) from #TempBo a
    WHERE a.CompanyCode = spMstItems.CompanyCode
    AND a.BranchCode = spMstItems.BranchCode 
    AND a.PartNo = spMstItems.PartNo
    AND a.SalesType='3'), spMstItems.BackOrderSL)
WHERE spMstItems.CompanyCode=@CompanyCode
AND spMstItems.BranchCode=@BranchCode 

UPDATE spMstItemLoc 
SET LastUpdateBy = @LastUpdateBy, LastUpdateDate = getDate(),
 BackOrderSP = ISNULL((SELECT SUM(a.BOAvail) from #TempBo a 
    WHERE a.CompanyCode = spMstItemLoc.CompanyCode
    AND a.BranchCode = spMstItemLoc.BranchCode
    AND a.WarehouseCode = spMstItemLoc.WarehouseCode
    AND a.PartNo = spMstItemLoc.PartNo 
    AND a.SalesType in ('0','1')), spMstItemLoc.BackOrderSP),
 BackOrderSR = ISNULL((SELECT SUM(a.BOAvail) from #TempBo a  
    WHERE a.CompanyCode = spMstItemLoc.CompanyCode
    AND a.BranchCode = spMstItemLoc.BranchCode
    AND a.WarehouseCode = spMstItemLoc.WarehouseCode
    AND a.PartNo = spMstItemLoc.PartNo 
    AND a.SalesType='2'), spMstItemLoc.BackOrderSR),
 BackOrderSL = ISNULL((SELECT SUM(a.BOAvail) from #TempBo a 
    WHERE a.CompanyCode = spMstItemLoc.CompanyCode
    AND a.BranchCode = spMstItemLoc.BranchCode
    AND a.WarehouseCode = spMstItemLoc.WarehouseCode
    AND a.PartNo = spMstItemLoc.PartNo 
    AND a.SalesType='3'), spMstItemLoc.BackOrderSL)
WHERE spMstItemLoc.CompanyCode=@CompanyCode
AND spMstItemLoc.BranchCode=@BranchCode 
AND spMstItemLoc.WarehouseCode = '00'

DROP TABLE #TempBo






GO

if object_id('sp_UpdateDemandAverageTuning') is not null
	drop procedure sp_UpdateDemandAverageTuning
GO





CREATE procedure [dbo].[sp_UpdateDemandAverageTuning] (  

@CompanyCode varchar(10) ,
@BranchCode varchar(10),
@TransDate datetime)


as

--declare @TransDate as datetime
--set @TransDate = '{0}'

select * into #t1 from (
select a.CompanyCode
,a.BranchCode
,a.PartNo
,a.DemandQty
,a.Year
,a.Month
,convert(varchar,a.Year) + right('0' + convert(varchar,a.Month),2) as date0
,convert(varchar(6), dateadd(m,-5,@TransDate), 112) date1
,convert(varchar(6), @TransDate, 112) date2
from spHstDemandItem a WITH(NOWAIT, NOLOCK) where a.CompanyCode = @CompanyCode AND a.BranchCode = @BranchCode
) #T1

select * into #T2 from (
select 
a.* 
,case when date0 between date1 and date2 then 1 else 0 end as IsValid
from #t1 a
) #t2

select * into #t3 from (
select CompanyCode, BranchCode, PartNo, Sum(DemandQty) DemandQty, Sum(DemandQty)/(6*30) DemandAvg from #t2 
where IsValid = 1 and DemandQty > 0
group by CompanyCode, BranchCode, PartNo
) #t3

select * into #t4 from (
select a.companycode, a.branchcode, a.partno, 0 as DemandAvg 
from spMstItems a
left join #t3 b on a.companycode = b.companycode and a.branchcode = b.branchcode and a.partno = b.partno
where  a.CompanyCode = @CompanyCode AND a.BranchCode = @BranchCode and b.partno is null
) #t4


update spMstItems set DemandAverage = isnull(b.DemandAvg, 0)
from spMstItems a, #t4 b
where a.CompanyCode=b.CompanyCode
and a.BranchCode=b.BranchCode
and a.PartNo=b.PartNo

update spMstItems set DemandAverage = isnull(b.DemandAvg, a.DemandAverage)
from spMstItems a, #t3 b
where a.CompanyCode=b.CompanyCode
and a.BranchCode=b.BranchCode
and a.PartNo=b.PartNo

drop table #t4
drop table #t3
drop table #t2
drop table #t1

GO

if object_id('sp_UpdateDemandHistoryTuning') is not null
	drop procedure sp_UpdateDemandHistoryTuning
GO






CREATE procedure [dbo].[sp_UpdateDemandHistoryTuning] (  

@CompanyCode varchar(10) ,
@BranchCode varchar(10),
@Year int,
@Month int,
@ProfitCenterCode varchar(10),
@LastUpdateBy varchar(10)
)


as

DELETE SpHstDemandItem 
                    WHERE Year=@Year AND Month=@Month AND CompanyCode = @CompanyCode AND BranchCode = @BranchCode

                    SELECT 
                        x.CompanyCode
                        , x.BranchCode
                        , @Year Year
                        , @Month Month
                        , x.PartNo
                        , 0 AS DemandFreq
                        , 0 AS DemandQty
                        , 0 AS SalesFreq
                        , 0 AS SalesQty    
                        , x.MovingCode
                        , x.ProductType
                        , x.PartCategory
                        , x.ABCClass
                        , @LastUpdateBy LastUpdateBy
                        , getdate() LastUpdateDate			
                    INTO #SpHstDemandItem 
                    FROM spMstItems x WITH(NOWAIT,NOLOCK) 
                    WHERE x.CompanyCode = @CompanyCode
                        AND x.BranchCode = @BranchCode
                    
                    INSERT INTO SpHstDemandItem SELECT * FROM #SpHstDemandItem

                    DROP TABLE #SpHstDemandItem


GO

if object_id('sp_UpdateMovingCodeTuningV2') is not null
	drop procedure sp_UpdateMovingCodeTuningV2
GO





CREATE procedure [dbo].[sp_UpdateMovingCodeTuningV2] (  

@CompanyCode varchar(10) ,
@BranchCode varchar(10),
@TransDate datetime)


as


select * into #t1 from (
select 
 a.PartNo
,a.DemandFreq
,a.DemandQty
,convert(varchar,a.Year) + right('0' + convert(varchar,a.Month),2) as date0
,convert(varchar(6), dateadd(m,-5,@TransDate), 112) date1
,convert(varchar(6), dateadd(m,-4,@TransDate), 112) date2
,convert(varchar(6), dateadd(m,-3,@TransDate), 112) date3
,convert(varchar(6), dateadd(m,-2,@TransDate), 112) date4
,convert(varchar(6), dateadd(m,-1,@TransDate), 112) date5
,convert(varchar(6), dateadd(m,-0,@TransDate), 112) date6
from spHstDemandItem a WITH(NOWAIT, NOLOCK) 
where a.CompanyCode=@CompanyCode and a.BranchCode=@BranchCode
 and convert(varchar,a.Year) + right('0' + convert(varchar,a.Month),2) >= convert(varchar(6), dateadd(m,-6,@TransDate), 112)
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
from #t4
where NewPartNo not in (select PartNo from #t3)

select * into #t5 from(
select distinct PartNo, D1, D2, D3, D4, D5, D6 from #t3)#t5


select * into #t6 from (
select 
	PartNo
	, CASE WHEN ISNULL((SELECT D1 FROM #t5 WHERE PartnO = a.PartNo),0) + ISNULL((SELECT D1 FROM #t5 WHERE PartnO = a.NewPartNo),0) > 0 THEN 1 ELSE 0 END D1
	, CASE WHEN ISNULL((SELECT D2 FROM #t5 WHERE PartnO = a.PartNo),0) + ISNULL((SELECT D2 FROM #t5 WHERE PartnO = a.NewPartNo),0) > 0 THEN 1 ELSE 0 END D2
	, CASE WHEN ISNULL((SELECT D3 FROM #t5 WHERE PartnO = a.PartNo),0) + ISNULL((SELECT D3 FROM #t5 WHERE PartnO = a.NewPartNo),0) > 0 THEN 1 ELSE 0 END D3
	, CASE WHEN ISNULL((SELECT D4 FROM #t5 WHERE PartnO = a.PartNo),0) + ISNULL((SELECT D4 FROM #t5 WHERE PartnO = a.NewPartNo),0) > 0 THEN 1 ELSE 0 END D4
	, CASE WHEN ISNULL((SELECT D5 FROM #t5 WHERE PartnO = a.PartNo),0) + ISNULL((SELECT D5 FROM #t5 WHERE PartnO = a.NewPartNo),0) > 0 THEN 1 ELSE 0 END D5
	, CASE WHEN ISNULL((SELECT D6 FROM #t5 WHERE PartnO = a.PartNo),0) + ISNULL((SELECT D6 FROM #t5 WHERE PartnO = a.NewPartNo),0) > 0 THEN 1 ELSE 0 END D6
from #t4 a) #t6

update #t5
set d1 = b.d1
	, d2 = b.d2
	, d3 = b.d3
	, d4 = b.d4
	, d5 = b.d5
	, d6 = b.d6
from #t5 a, #t6 b
where a.partno = b.partno


select * into #t7 from (
select @CompanyCode CompanyCode, @BranchCode BranchCode, partno, d1 + d2 + d3 + d4 + d5 + d6  dTotal from #t5) #t7

update spMstItems 
set MovingCode = CASE WHEN b.dTotal = 0 THEN 5
			ELSE CASE WHEN b.dTotal = 1 THEN 4 
			ELSE CASE WHEN b.dTotal = 2 THEN 4
			ELSE CASE WHEN b.dTotal = 3 THEN 3 
			ELSE CASE WHEN b.dTotal = 4 THEN 3 
			ELSE CASE WHEN b.dTotal = 5 THEN 2 
			ELSE CASE WHEN b.dTotal = 6 THEN 1 
			END END END END END END END 
from spMstItems a, #t7 b
where 
	a.CompanyCode = b.CompanyCode
	and a.branchcode = b.branchcode
	and a.partno = b.partno
	and (datediff(mm, a.BornDate, @transdate) + 1) >= 6 

-- SET MOVING CODE : 0 FOR ITEM THAT BORN DATE < 6 MONTHS
update spMstItems set MovingCode = 0
where CompanyCode = @CompanyCode
  and BranchCode = @BranchCode
  and (datediff(mm, BornDate, @TransDate) + 1) < 6

drop table #t7
drop table #t6
drop table #t5
drop table #t4
drop table #t3
drop table #t2
drop table #t1


GO

if object_id('sp_UpdateOrderPointTuning') is not null
	drop procedure sp_UpdateOrderPointTuning
GO






CREATE procedure [dbo].[sp_UpdateOrderPointTuning] (  

@CompanyCode varchar(10) ,
@BranchCode varchar(10),
@LastUpdateBy varchar(10)
)


as

SELECT
	a.CompanyCode
	, a.BranchCode
	, a.PartNo
	, (ISNULL(a.DemandAverage, 0) * (ISNULL(c.LeadTime, 0) + ISNULL(c.OrderCycle, 0))) + 
	  (ISNULL(a.DemandAverage, 0) * ISNULL(c.SafetyStock, 0)) OrderPointQty
    , (ISNULL(a.DemandAverage, 0) * ISNULL(c.SafetyStock, 0)) SafetyStockQty
	, ISNULL(c.LeadTime, 0) LeadTime
	, ISNULL(c.OrderCycle, 0) OrderCycle
	, ISNULL(c.SafetyStock, 0) SafetyStock
INTO
	#OrderPointQty
FROM
	spMstItems a		
	LEFT JOIN spMstItemInfo b ON a.CompanyCode = b.CompanyCode
		AND a.PartNo = b.PartNo		
	LEFT JOIN spMstOrderParam c ON a.CompanyCode = c.CompanyCode
		AND a.BranchCode = c.BranchCode
		AND a.MovingCode = c.MovingCode
		AND b.SupplierCode = c.SupplierCode
WHERE	
	a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode

UPDATE spMstItems
SET OrderPointQty = b.OrderPointQty
    , SafetyStockQty = b.SafetyStockQty
	, LeadTime = b.LeadTime
	, OrderCycle = b.OrderCycle
	, SafetyStock = b.SafetyStock
	, LastUpdateBy = @LastUpdateBy
	, LastUpdateDate = GETDATE()
FROM
	spMstItems a, #OrderPointQty b
WHERE
	a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode 
	AND a.PartNo = b.PartNo

DROP TABLE #OrderPointQty


GO

if object_id('uspfn_AbExtractData_II') is not null
	drop procedure uspfn_AbExtractData_II
GO
create procedure uspfn_AbExtractData_II
	@FileID varchar(100),
	@FileContent varchar(max),
	@UserId varchar(25)
as
begin
	--select @FileContent

	update SimDmsIterator 
	   set AttendanceFlatFileExtractionProcessed=0
	     , AttendanceFlatFileExtractionTotal=100

	select * into #a 
	from uspfn_AbSplitString(@FileContent, char(10)+char(13))

	--begin try
		declare c cursor for 
			select * from #a
		declare @Record varchar(100);
		declare @EmployeeID varchar(17);
		declare @AttendanceTime varchar(20);
		declare @MachineCode varchar(15);
		declare @IdentityCode varchar(12);
		declare @CurrentDate datetime;
		declare @CompanyCode varchar(17);
		declare @TrailingZeroIndex tinyint;
		set @CompanyCode = (
			select top 1
			       CompanyCode
			  from gnMstOrganizationHdr 
		);

		declare @NumberOfIteratorRecords int;
		declare @NumberOfRecord int;
		set @NumberOfIteratorRecords = ( select count(*) from SimDmsIterator );
		set @NumberOfRecord = (select count(*) from #a);

		if @NumberOfIteratorRecords = 0
		begin
			insert into SimDmsIterator (AttendanceFlatFileExtractionProcessed, AttendanceFlatFileExtractionTotal)
			values (0, 100)
		end

		update SimDmsIterator
		   set AttendanceFlatFileExtractionProcessed = 0
		     , AttendanceFlatFileExtractionTotal = @NumberOfRecord
			 




		open c
			fetch next from c into @Record

			declare @iterator int;
			set @iterator=1;
			while @@FETCH_STATUS=0
			begin
				if @iterator>2
				begin
					set @CompanyCode = (select top 1 CompanyCode from gnMstOrganizationHdr );
					set @EmployeeID = (select * from dbo.SplitString(@Record, ',', 1));
					--set @AttendanceTime = convert(datetime,  (select * from dbo.SplitString(@Record, ',', 3)) + ' ' + (select * from dbo.SplitString(@Record, ',', 4)));
					set @AttendanceTime = (select * from dbo.SplitString(@Record, ',', 3)) + ' ' + (select * from dbo.SplitString(@Record, ',', 4));
					set @MachineCode = (select * from dbo.SplitString(@Record, ',', 5));
					set @IdentityCode = ( 
						case (select * from dbo.SplitString(@Record, ',', 8) )
							when 'scan masuk' then 'I' 
							else 'O' 
						end);


					--select @AttendanceTime as AttendanceTime;
					--select * from dbo.SplitString(@Record, ',', 3);
					exec uspfn_AbInsertAttendanceData 
						 @CompanyCode = @CompanyCode
					   , @FileID = @FileID
					   , @Iterator = @Iterator
					   , @EmployeeID = @EmployeeID
					   , @AttendanceTime = @AttendanceTime
					   , @MachineCode = @MachineCode
					   , @IdentityCode = @IdentityCode
					   , @UserId = @UserID

					update SimDmsIterator 
					   set AttendanceFlatFileExtractionProcessed = @iterator					 
				end

				set @iterator = @iterator + 1;
				fetch next from c into @Record
			end
		close c
		deallocate c

		select Convert(bit, 1);
	--end try
	--begin catch
	--	select Convert(bit, 0);
	--end catch
end



GO

if object_id('uspfn_GetTaxInPeriod') is not null
	drop procedure uspfn_GetTaxInPeriod
GO
 CREATE procedure [dbo].[uspfn_GetTaxInPeriod]
	@CompanyCode nvarchar(25),
	@BranchCode nvarchar(25),
	@ProductType nvarchar(2),
	@PeriodYear int,
	@PeriodMonth int
  
  as
  begin
--DECLARE @CompanyCode nvarchar(25) set @CompanyCode ='6006410'
--DECLARE @BranchCode nvarchar(25) set @BranchCode ='600641001'
--DECLARE @ProductType nvarchar(2) set @ProductType ='4W'
--DECLARE @PeriodYear int set @PeriodYear = 2014
--DECLARE @PeriodMonth int set @PeriodMonth =12

  --exec uspfn_GetTaxInPeriod 6006410,600641001,'4W',2014,12


SELECT * INTO #1 FROM (
SELECT BranchCode, TaxNo, SupplierCode FROM gnTaxIn WITH(NOLOCK, NOWAIT)
WHERE CompanyCode = @CompanyCode 
    AND BranchCode = case @BranchCode when '' then BranchCode else @BranchCode end 
    AND ProductType = @ProductType 
	AND PeriodYear = @PeriodYear 
    AND PeriodMonth = @PeriodMonth
UNION
SELECT BranchCode, TaxNo, SupplierCode FROM gnTaxInHistory WITH(NOLOCK, NOWAIT)
WHERE CompanyCode = @CompanyCode 
    AND BranchCode = case @BranchCode when '' then BranchCode else @BranchCode end 
    AND ProductType = @ProductType 
	AND PeriodYear = @PeriodYear 
    AND PeriodMonth = @PeriodMonth 
    AND IsDeleted = '1'
) #1

/* AMBIL SEMUA DATA HPP SPARE */
select * into #t_1 from (
select b.SupplierCode, a.* 
from spTrnPHpp a
left join spTrnPRcvHdr b on b.CompanyCode = a.CompanyCode 
	and b.BranchCode = a.BranchCode
	and b.WRSNo = a.WRSNo
where
	a.CompanyCode	 = @CompanyCode
	and a.BranchCode = case @BranchCode when '' then a.BranchCode else @BranchCode end
	and a.YearTax	 = @PeriodYear
	and a.MonthTax	 = @PeriodMonth
)#t_1 

/* AMBIL SEMUA DATA BTT OTHER (AP) */
select * into #t_2 from (
select a.* 
from apTrnBTTOtherHdr a
where 
	a.CompanyCode = @CompanyCode
	and a.BranchCode = case @BranchCode when '' then a.BranchCode else @BranchCode end
	and a.FPJNo <> ''
	and SUBSTRING(a.FPJPeriod, 1, 4) = @PeriodYear
    and RIGHT(a.FPJPeriod, 2) = @PeriodMonth
)#t_2 

/* DATA HPP SPARE YANG MEMILIKI SUPPLIER DAN TAXNO YANG SAMA DENGAN BTT OTHER (NILAI DIJUMLAHKAN) */
SELECT * INTO #t_3 FROM (
SELECT
  a.CompanyCode
, a.BranchCode
, d.ProductType
, a.YearTax PeriodYear
, a.MonthTax PeriodMonth
, '300' ProfitCenter
, a.TypeOfGoods
, 'B' TaxCode
, '2' TransactionCode
, '1' StatusCode
, '1' DocumentCode
, 'F' DocumentType
, b.SupplierCode
, c.SupplierGovName SupplierName
, c.IsPKP
, c.NPWPNo NPWP
, a.ReferenceNo FPJNo
, a.ReferenceDate FPJDate
, a.HPPNo + ' (' + a.ReferenceNo + ')' ReferenceNo
, a.WRSDate ReferenceDate
, a.TaxNo TaxNo
, a.TaxDate TaxDate
, a.DueDate SubmissionDate
, ISNULL((ISNULL(a.TotNetPurchAmt, 0) + ISNULL(a.DiffNetPurchAmt, 0)) + g.DppAmt,0) DPPAmt
, ISNULL((ISNULL(a.TotTaxAmt, 0) + ISNULL(a.DiffTaxAmt, 0)) + g.PPNAmt, 0) PPNAmt
, ISNULL(0 + g.PPNBmAmt, 0) PPNBmAmt
, 'PEMBELIAN SPARE PART' Description
, ISNULL(ISNULL(b.TotItem, 0)+(SELECT COUNT(*) FROM apTrnBTTOtherDtl WHERE CompanyCode = a.CompanyCode 
							AND BranchCode = a.BranchCode AND BTTNo = g.BTTNo), 0) Quantity	
FROM
	spTrnPHPP a	WITH(NOLOCK, NOWAIT)
	LEFT JOIN spTrnPRcvHdr b WITH(NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode
		AND a.BranchCode = b.BranchCode
		AND a.WRSNo = b.WRSNo
	LEFT JOIN gnMstSupplier c WITH(NOLOCK, NOWAIT) ON a.CompanyCode = c.CompanyCode
		AND b.SupplierCode = c.SupplierCode
	LEFT JOIN gnMstCoProfile d WITH(NOLOCK, NOWAIT) ON a.CompanyCode = d.CompanyCode
		AND a.BranchCode = d.BranchCode
    INNER JOIN gnMstSupplierProfitCenter e WITH(NOLOCK, NOWAIT) ON a.CompanyCode = e.CompanyCode
	    AND a.BranchCode = e.BranchCode
        AND b.SupplierCode = e.SupplierCode
    INNER JOIN gnMstTax f WITH(NOLOCK, NOWAIT) ON a.CompanyCode = f.CompanyCode
        AND e.TaxCode = f.TaxCode
	INNER JOIN #t_2 g on g.BranchCode=a.BranchCode AND g.SupplierCode = b.SupplierCode
		AND g.FPJNo = a.TaxNo
WHERE
	a.CompanyCode = @CompanyCode
	AND a.BranchCode = case @BranchCode when '' then a.BranchCode else @BranchCode end
	AND ProductType = @ProductType
	AND a.YearTax = @PeriodYear
	AND a.MonthTax = @PeriodMonth
	AND a.Status = '2'
    AND e.ProfitCenterCode = '300'
    AND f.TaxPct > 0
) #t_3

-----------------------------------------------------------

/* QUERY UTAMA */
SELECT * INTO Query3S FROM (
-- SALES : PURCHASE
SELECT
  a.CompanyCode
, a.BranchCode
, e.ProductType
, YEAR(a.LockingDate) PeriodYear
, MONTH(a.LockingDate) PeriodMonth
, '100' ProfitCenter
, NULL TypeOfGoods
, 'B' TaxCode
, '2' TransactionCode
, '1' StatusCode
, '1' DocumentCode
, 'F' DocumentType
, a.SupplierCode 
, d.SupplierGovName SupplierName
, d.IsPKP 
, d.NPWPNo NPWP
, a.RefferenceFakturPajakNo FPJNo
, a.RefferenceFakturPajakDate FPJDate
, a.HPPNo + ' (' + a.RefferenceInvoiceNo + ')' ReferenceNo
, a.RefferenceInvoiceDate ReferenceDate
, a.RefferenceFakturPajakNo TaxNo
, a.RefferenceFakturPajakDate TaxDate
, a.DueDate SubmissionDate
, (SELECT SUM(ISNULL(Quantity, 0) * (ISNULL(AfterDiscDPP, 0) + ISNULL(OthersDPP, 0))) FROM omTrPurchaseHPPDetailModel 
    WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND HPPNo = a.HPPNo ) DPPAmt
, (SELECT SUM(ISNULL(Quantity, 0) * (ISNULL(AfterDiscPPn, 0) + ISNULL(OthersPPn, 0))) FROM omTrPurchaseHPPDetailModel 
    WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND HPPNo = a.HPPNo ) PPNAmt
, (SELECT SUM(ISNULL(Quantity, 0) * ISNULL(AfterDiscPPnBM, 0)) FROM omTrPurchaseHPPDetailModel 
    WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND HPPNo = a.HPPNo ) PPNBmAmt
, (SELECT TOP 1 SalesModelCode + ', NO. CHS. ' + CONVERT(VARCHAR, ChassisNo) FROM omTrPurchaseHPPSubDetail 
	WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND HPPNo = a.HPPNo) Description
, (SELECT COUNT(HPPSeq) FROM omTrPurchaseHPPSubDetail c 
    LEFT JOIN omTrPurchaseHPPDetailModel b ON c.CompanyCode = b.CompanyCode AND c.BranchCode = b.BranchCode 
    AND c.HPPNo = b.HPPNo AND c.BPUNo = b.BPUNo
    WHERE c.CompanyCode = a.CompanyCode AND c.BranchCode = a.BranchCode AND c.HPPNo = a.HPPNo) Quantity
FROM
    omTrPurchaseHPP a WITH(NOLOCK, NOWAIT)
    LEFT JOIN gnMstSupplier d WITH(NOLOCK, NOWAIT) ON a.CompanyCode = d.CompanyCode
	    AND a.SupplierCode = d.SupplierCode
    LEFT JOIN gnMstCoProfile e WITH(NOLOCK, NOWAIT) ON a.CompanyCode = e.CompanyCode
	    AND a.BranchCode = e.BranchCode
    INNER JOIN gnMstSupplierProfitCenter f WITH(NOLOCK, NOWAIT) ON a.CompanyCode = f.CompanyCode
	    AND a.BranchCode = f.BranchCode
        AND a.SupplierCode = f.SupplierCode
    INNER JOIN gnMstTax g WITH(NOLOCK, NOWAIT) ON a.CompanyCode = g.CompanyCode
        AND f.TaxCode = g.TaxCode        
WHERE
    a.CompanyCode = @CompanyCode
    AND a.BranchCode = case @BranchCode when '' then a.BranchCode else @BranchCode end
    AND ProductType = @ProductType
    AND YEAR(a.LockingDate) = @PeriodYear
    AND MONTH(a.LockingDate) = @PeriodMonth
    AND a.Status NOT IN ('3')
    AND f.ProfitCenterCode = '100'    
    AND g.TaxPct > 0
-------------------------------------------------------------------------------------
UNION
-- SALES : KAROSERI
SELECT
  a.CompanyCode
, a.BranchCode
, ProductType
, YEAR(a.LockingDate) PeriodYear
, MONTH(a.LockingDate) PeriodMonth
, '100' ProfitCenter
, NULL TypeOfGoods
, 'B' TaxCode
, '2' TransactionCode
, '1' StatusCode
, '1' DocumentCode
, 'F' DocumentType
, a.SupplierCode
, b.SupplierGovName SupplierName
, b.IsPKP
, b.NPWPNo NPWP
, a.RefferenceFakturPajakNo FPJNo
, a.RefferenceFakturPajakDate FPJDate
, a.KaroseriTerimaNo + ' (' + a.RefferenceInvoiceNo + ')' ReferenceNo
, a.RefferenceInvoiceDate ReferenceDate
, a.RefferenceFakturPajakNo TaxNo
, a.RefferenceFakturPajakDate TaxDate
, a.DueDate SubmissionDate
, ISNULL(a.Quantity, 0) * (ISNULL(a.DPPMaterial, 0) + ISNULL(a.DPPFee, 0) + ISNULL(a.DPPOthers, 0)) DPPAmt
, ISNULL(a.Quantity, 0) * ISNULL(a.PPn, 0) PPNAmt
, 0 PPNBmAmt
, 'Karoseri SPK No: ' + a.KaroseriSPKNo Description
, ISNULL(a.Quantity, 0) Quantity
FROM
    omTrPurchaseKaroseriTerima a WITH(NOLOCK, NOWAIT)
    LEFT JOIN gnMstSupplier b WITH(NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode
	    AND a.SupplierCode = b.SupplierCode
    LEFT JOIN gnMstCoProfile c WITH(NOLOCK, NOWAIT) ON a.CompanyCode = c.CompanyCode
	    AND a.BranchCode = c.BranchCode
    INNER JOIN gnMstSupplierProfitCenter d WITH(NOLOCK, NOWAIT) ON a.CompanyCode = d.CompanyCode
	    AND a.BranchCode = d.BranchCode
        AND a.SupplierCode = d.SupplierCode  
    INNER JOIN gnMstTax e WITH(NOLOCK, NOWAIT) ON a.CompanyCode = e.CompanyCode
        AND d.TaxCode = e.TaxCode              
WHERE
    a.CompanyCode = @CompanyCode
    AND a.BranchCode = case @BranchCode when '' then a.BranchCode else @BranchCode end
    AND ProductType = @ProductType
    AND YEAR(a.LockingDate) = @PeriodYear
    AND MONTH(a.LockingDate) = @PeriodMonth
    AND a.Status NOT IN ('3')
    AND d.ProfitCenterCode = '100'
    AND e.TaxPct > 0
-----------------------------------------------------------------------------------------
UNION
-- SALES : PURCHASE RETURN
SELECT
  a.CompanyCode
, a.BranchCode
, d.ProductType
, YEAR(ReturnDate) PeriodYear
, MONTH(ReturnDate) PeriodMonth
, '100' ProfitCenter
, NULL TypeOfGoods
, 'B' TaxCode
, '2' TransactionCode
, '1' StatusCode
, '1' DocumentCode
, 'R' DocumentType
, c.SupplierCode SupplierCode
, c.SupplierGovName SupplierName
, c.IsPKP IsPKP
, c.NPWPNo NPWP
, replace(a.RefferenceNo,substring(a.RefferenceNo,1,1),'9') FPJNo
, a.RefferenceDate FPJDate
, a.ReturnNo + ' (' + a.RefferenceNo + ')' ReferenceNo
, a.RefferenceDate ReferenceDate
, replace(a.RefferenceNo,substring(a.RefferenceNo,1,1),'9') TaxNo  
, a.RefferenceDate TaxDate
, a.ReturnDate SubmissionDate
,(SELECT SUM(ISNULL(Quantity, 0) * (ISNULL(AfterDiscDPP, 0) + ISNULL(OthersDPP, 0))) FROM omTrPurchaseReturnDetailModel 
    WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND ReturnNo = a.ReturnNo) * -1 DPPAmt
,(SELECT SUM(ISNULL(Quantity, 0) * (ISNULL(AfterDiscPPn, 0) + ISNULL(OthersPPn, 0)))FROM omTrPurchaseReturnDetailModel 
    WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND ReturnNo = a.ReturnNo) * -1 PPNAmt
,(SELECT SUM(ISNULL(Quantity, 0) * ISNULL(AfterDiscPPnBM, 0)) FROM omTrPurchaseReturnDetailModel 
    WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND ReturnNo = a.ReturnNo) * -1 PPNBmAmt
, 'RTR-SLS-NO: ' + a.ReturnNo Description
, (SELECT SUM(ISNULL(Quantity, 0))FROM omTrPurchaseReturnDetailModel 
    WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND ReturnNo = a.ReturnNo) * -1 Quantity
FROM
    omTrPurchaseReturn a WITH(NOLOCK, NOWAIT)
    LEFT JOIN omTrPurchaseHPP b WITH(NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode
	    AND a.BranchCode = b.BranchCode
	    AND a.HPPNo = b.HPPNo
    LEFT JOIN gnMstSupplier c WITH(NOLOCK, NOWAIT) ON a.CompanyCode = c.CompanyCode
	    AND b.SupplierCode = c.SupplierCode
    LEFT JOIN gnMstCoProfile d WITH(NOLOCK, NOWAIT) ON a.CompanyCode = d.CompanyCode
	    AND a.BranchCode = d.BranchCode
    INNER JOIN gnMstSupplierProfitCenter e WITH(NOLOCK, NOWAIT) ON a.CompanyCode = e.CompanyCode
	    AND a.BranchCode = e.BranchCode
        AND b.SupplierCode = e.SupplierCode  
    INNER JOIN gnMstTax f WITH(NOLOCK, NOWAIT) ON a.CompanyCode = f.CompanyCode
        AND e.TaxCode = f.TaxCode              
WHERE
    a.CompanyCode = @CompanyCode
    AND a.BranchCode = case @BranchCode when '' then a.BranchCode else @BranchCode end
    AND ProductType = @ProductType
    AND YEAR(ReturnDate) = @PeriodYear
    AND MONTH(ReturnDate) = @PeriodMonth
    AND a.Status NOT IN ('3')
    AND e.ProfitCenterCode = '100'
    AND f.TaxPct > 0
---------------------------------------------------------------------------------------
UNION
-- SERVICE
SELECT
 a.CompanyCode
, a.BranchCode
, a.ProductType
, YEAR(RecDate) PeriodYear
, MONTH(RecDate) PeriodMonth
, '200' ProfitCenter
, NULL TypeOfGoods
, 'B' TaxCode
, '2' TransactionCode
, '1' StatusCode
, '1' DocumentCode
, 'F' DocumentType
, a.SupplierCode SupplierCode
, b.SupplierGovName SupplierName
, b.IsPKP IsPKP
, b.NPWPNo NPWP
, a.FPJNo FPJNo
, a.FPJDate FPJDate
, a.PONo + ' (' + a.JobOrderNo + ')' ReferenceNo
, a.RecDate RefferenceDate
, a.FPJGovNo TaxNo
, a.FPJDate TaxDate
, a.DueDate SubmissionDate
, ISNULL(a.DPPAmt, 0) DPPAmt
, ISNULL(a.PPnAmt, 0) PPNAmt
, 0 PPNBmAmt
, 'REC-SV-NO: ' + a.RecNo Description
, 1 Quantity
FROM
    svTrnPOSubCon a	WITH(NOLOCK, NOWAIT)
    LEFT JOIN gnMstSupplier b WITH(NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode
	    AND a.SupplierCode = b.SupplierCode
    INNER JOIN gnMstSupplierProfitCenter c WITH(NOLOCK, NOWAIT) ON a.CompanyCode = c.CompanyCode
	    AND a.BranchCode = c.BranchCode
        AND a.SupplierCode = c.SupplierCode      
    INNER JOIN gnMstTax d WITH(NOLOCK, NOWAIT) ON a.CompanyCode = d.CompanyCode
        AND c.TaxCode = d.TaxCode          
WHERE
    a.CompanyCode = @CompanyCode
    AND a.BranchCode = case @BranchCode when '' then a.BranchCode else @BranchCode end
    AND a.ProductType = @ProductType
    AND YEAR(RecDate) = @PeriodYear
    AND MONTH(RecDate) = @PeriodMonth
    AND c.ProfitCenterCode = '200'
    AND d.TaxPct > 0
---------------------------------------------------------------------------------------
UNION 
-- SPAREPART
SELECT
  a.CompanyCode
, a.BranchCode
, d.ProductType
, a.YearTax PeriodYear
, a.MonthTax PeriodMonth
, '300' ProfitCenter
, a.TypeOfGoods
, 'B' TaxCode
, '2' TransactionCode
, '1' StatusCode
, '1' DocumentCode
, 'F' DocumentType
, b.SupplierCode
, c.SupplierGovName SupplierName
, c.IsPKP
, c.NPWPNo NPWP
, a.ReferenceNo FPJNo
, a.ReferenceDate FPJDate
, a.HPPNo + ' (' + a.ReferenceNo + ')' ReferenceNo
, a.WRSDate ReferenceDate
, a.TaxNo TaxNo
, a.TaxDate TaxDate
, a.DueDate SubmissionDate
, ISNULL(a.TotNetPurchAmt, 0) + ISNULL(a.DiffNetPurchAmt, 0) DPPAmt
, ISNULL(a.TotTaxAmt, 0) + ISNULL(a.DiffTaxAmt, 0) PPNAmt
, 0 PPNBmAmt
, 'PEMBELIAN SPARE PART' Description
, ISNULL(b.TotItem, 0) Quantity	
FROM
	spTrnPHPP a	WITH(NOLOCK, NOWAIT)
	LEFT JOIN spTrnPRcvHdr b WITH(NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode
		AND a.BranchCode = b.BranchCode
		AND a.WRSNo = b.WRSNo
	LEFT JOIN gnMstSupplier c WITH(NOLOCK, NOWAIT) ON a.CompanyCode = c.CompanyCode
		AND b.SupplierCode = c.SupplierCode
	LEFT JOIN gnMstCoProfile d WITH(NOLOCK, NOWAIT) ON a.CompanyCode = d.CompanyCode
		AND a.BranchCode = d.BranchCode
    INNER JOIN gnMstSupplierProfitCenter e WITH(NOLOCK, NOWAIT) ON a.CompanyCode = e.CompanyCode
	    AND a.BranchCode = e.BranchCode
        AND b.SupplierCode = e.SupplierCode
    INNER JOIN gnMstTax f WITH(NOLOCK, NOWAIT) ON a.CompanyCode = f.CompanyCode
        AND e.TaxCode = f.TaxCode
WHERE
	a.CompanyCode = @CompanyCode
	AND a.BranchCode = case @BranchCode when '' then a.BranchCode else @BranchCode end
	AND ProductType = @ProductType
	AND a.YearTax = @PeriodYear
	AND a.MonthTax = @PeriodMonth
	AND a.Status = '2'
    AND e.ProfitCenterCode = '300'
    AND f.TaxPct > 0
	AND b.BranchCode+'-'+b.SupplierCode+'-'+a.TaxNo NOT IN (SELECT BranchCode+'-'+SupplierCode+'-'+TaxNo FROM #t_3)
-------------------------------------------------------------------------------
UNION
-- FINANCE
SELECT
  a.CompanyCode
, a.BranchCode
, c.ProductType
, SUBSTRING(a.FPJPeriod, 1, 4) PeriodYear
, RIGHT(a.FPJPeriod, 2) PeriodMonth
, '000' ProfitCenter
, 'NULL' TypeOfGoods
, 'B' TaxCode
, '2' TransactionCode
, '1' StatusCode
, '1' DocumentCode
, 'F' DocumentType
, a.SupplierCode
, b.SupplierGovName SupplierName
, b.IsPKP
, b.NPWPNo NPWP
, a.FPJNo
, a.FPJDate
, a.BTTNo + ' (' + a.ReffNo + ')' ReferenceNo
, a.ReffDate ReferenceDate
, a.FPJNo TaxNo
, a.FPJDate TaxDate
, a.DueDate SubmissionDate
, a.DPPAmt
, a.PPNAmt
, a.PPnBMAmt
, (SELECT TOP 1 Description FROM apTrnBTTOtherDtl WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND BTTNo = a.BTTNo) Description
, (SELECT COUNT(*) FROM apTrnBTTOtherDtl WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND BTTNo = a.BTTNo) Quantity	
FROM
    apTrnBTTOtherHdr a	WITH(NOLOCK, NOWAIT)    
    LEFT JOIN gnMstSupplier b WITH(NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode
	    AND a.SupplierCode = b.SupplierCode
    LEFT JOIN gnMstCoProfile c WITH(NOLOCK, NOWAIT) ON a.CompanyCode = c.CompanyCode
	    AND a.BranchCode = c.BranchCode
    INNER JOIN gnMstSupplierProfitCenter d WITH(NOLOCK, NOWAIT) ON a.CompanyCode = d.CompanyCode
	    AND a.BranchCode = d.BranchCode
        AND a.SupplierCode = d.SupplierCode
    INNER JOIN gnMstTax e WITH(NOLOCK, NOWAIT) ON a.CompanyCode = e.CompanyCode
        AND d.TaxCode = e.TaxCode               
WHERE
    a.CompanyCode = @CompanyCode
    AND a.BranchCode = case @BranchCode when '' then a.BranchCode else @BranchCode end
    AND ProductType = @ProductType
    AND SUBSTRING(a.FPJPeriod, 1, 4) = @PeriodYear
    AND RIGHT(a.FPJPeriod, 2) = @PeriodMonth
    AND a.Category = 'INV'
    AND d.ProfitCenterCode = '000'
    AND e.TaxPct > 0
    AND a.BranchCode+'-'+a.SupplierCode+'-'+a.FPJNo NOT IN (SELECT BranchCode+'-'+SupplierCode+'-'+TaxNo FROM #t_3)
-------------------------------------------------------------------------------
UNION
-- SPARE PART DAN BTT YANG MEMILIKI SUPPLIER DAN TAXNO YANG SAMA
SELECT * FROM #t_3
) Query3S

select ROW_NUMBER() OVER(ORDER BY q1.BranchCode, q1.ProfitCenter ASC, q1.TaxNo)  SeqNo
    , q1.CompanyCode
	, q1.BranchCode
	, q1.ProductType
	, q1.PeriodYear
	, q1.PeriodMonth
	, q1.ProfitCenter
	, ISNULL((SELECT TOP 1 TypeOfGoods FROM Query3S WHERE CompanyCode = q1.CompanyCode AND BranchCode = q1.BranchCode AND 
		ProductType = q1.ProductType AND PeriodYear = q1.PeriodYear AND PeriodMonth = q1.PeriodMonth AND 
		SupplierCode = q1.SupplierCode AND TaxNo = q1.TaxNo), '') TypeOfGoods
	, q1.TaxCode
	, q1.TransactionCode
	, q1.StatusCode
	, q1.DocumentCode
	, q1.DocumentType
	, q1.SupplierCode
	, q1.SupplierName
	, q1.IsPKP
	, q1.NPWP
	, (SELECT TOP 1 FPJNo FROM Query3S WHERE CompanyCode = q1.CompanyCode AND BranchCode = q1.BranchCode AND 
		ProductType = q1.ProductType AND PeriodYear = q1.PeriodYear AND PeriodMonth = q1.PeriodMonth AND 
		SupplierCode = q1.SupplierCode AND TaxNo = q1.TaxNo) FPJNo
	, (SELECT TOP 1 FPJDate FROM Query3S WHERE CompanyCode = q1.CompanyCode AND BranchCode = q1.BranchCode AND 
		ProductType = q1.ProductType AND PeriodYear = q1.PeriodYear AND PeriodMonth = q1.PeriodMonth AND 
		SupplierCode = q1.SupplierCode AND TaxNo = q1.TaxNo) FPJDate
	, (SELECT TOP 1 ReferenceNo FROM Query3S WHERE CompanyCode = q1.CompanyCode AND BranchCode = q1.BranchCode AND 
		ProductType = q1.ProductType AND PeriodYear = q1.PeriodYear AND PeriodMonth = q1.PeriodMonth AND 
		SupplierCode = q1.SupplierCode AND TaxNo = q1.TaxNo) ReferenceNo
	, (SELECT TOP 1 ReferenceDate FROM Query3S WHERE CompanyCode = q1.CompanyCode AND BranchCode = q1.BranchCode AND 
		ProductType = q1.ProductType AND PeriodYear = q1.PeriodYear AND PeriodMonth = q1.PeriodMonth AND 
		SupplierCode = q1.SupplierCode AND TaxNo = q1.TaxNo) ReferenceDate
	, q1.TaxNo
	, (SELECT TOP 1 TaxDate FROM Query3S WHERE CompanyCode = q1.CompanyCode AND BranchCode = q1.BranchCode AND 
		ProductType = q1.ProductType AND PeriodYear = q1.PeriodYear AND PeriodMonth = q1.PeriodMonth AND 
		SupplierCode = q1.SupplierCode AND TaxNo = q1.TaxNo) TaxDate
	, (SELECT TOP 1 SubmissionDate FROM Query3S WHERE CompanyCode = q1.CompanyCode AND BranchCode = q1.BranchCode AND 
		ProductType = q1.ProductType AND PeriodYear = q1.PeriodYear AND PeriodMonth = q1.PeriodMonth AND 
		SupplierCode = q1.SupplierCode AND TaxNo = q1.TaxNo) SubmissionDate
	, SUM(q1.DPPAmt) DPPAmt
	, SUM(q1.PPNAmt) PPNAmt
	, SUM(q1.PPnBMAmt) PPnBMAmt
	, (SELECT TOP 1 Description FROM Query3S WHERE CompanyCode = q1.CompanyCode AND BranchCode = q1.BranchCode AND 
		ProductType = q1.ProductType AND PeriodYear = q1.PeriodYear AND PeriodMonth = q1.PeriodMonth AND 
		SupplierCode = q1.SupplierCode AND TaxNo = q1.TaxNo) Description
	, SUM(q1.Quantity) Quantity
from Query3S q1
group by
	q1.CompanyCode
	, q1.BranchCode
	, q1.ProductType
	, q1.PeriodYear
	, q1.PeriodMonth
	, q1.ProfitCenter
	, q1.TaxCode
	, q1.TransactionCode
	, q1.StatusCode
	, q1.DocumentCode
	, q1.DocumentType
	, q1.SupplierCode
	, q1.SupplierName
	, q1.IsPKP
	, q1.NPWP
	, q1.TaxNo 

select
    ISNULL(SUM(DPPAmt),0) DPPAmt
    ,ISNULL(SUM(PPnAmt),0) PPnAmt
	,ISNULL((SELECT SUM(DPPAmt) FROM Query3S WHERE IsPKP='1'),0) SumDPPStd 
	,ISNULL((SELECT SUM(DPPAmt) FROM Query3S WHERE IsPKP='0'),0) SumDPPSdh 
	,ISNULL((SELECT SUM(PPnAmt) FROM Query3S WHERE IsPKP='1'),0) SumPPNStd
	,ISNULL((SELECT SUM(PPnAmt) FROM Query3S WHERE IsPKP='0'),0) SumPPNSdh
	,ISNULL(SUM(PPnBMAmt),0) SumPPnBMAmt
from Query3S

drop table Query3S
drop table #1
drop table #t_1
drop table #t_2
drop table #t_3
end
GO

if object_id('uspfn_GnGetDataSHISTTest') is not null
	drop procedure uspfn_GnGetDataSHISTTest
GO
CREATE Procedure [dbo].[uspfn_GnGetDataSHISTTest]    
as    
declare @CompanyCode varchar(20)    
declare @BranchCode  varchar(20)    
declare @DealerCode  char(10)    
declare @TotalItem  int    
declare @LastDate  datetime    
declare @ProductType varchar(100)    
    
set @CompanyCode = (select top 1 CompanyCode from gnMstOrganizationHdr);    
set @BranchCode  = (select top 1 BranchCode from gnMstOrganizationDtl where IsBranch = 0);    
set @DealerCode  = isnull((select LockingBy from gnMstCoProfileService where CompanyCode = @CompanyCode and BranchCode = @BranchCode), @CompanyCode);    
set @ProductType = (select top 1 ProductType from gnMstCoProfile where CompanyCode = @CompanyCode and BranchCode = @BranchCode)    
    
declare @T_SHIST  table (Line varchar(max), SeqNo varchar(max), SeqNo1 varchar(MAX))    
declare @T_SHIST_DTL table    
(    
 Year     varchar(4),    
 Month     varchar(2),    
 BranchCode    varchar(15),    
 BranchName    varchar(100),    
 Area     varchar(100),    
 BranchHeadID   varchar(15),    
 BranchHeadName   varchar(100),    
 SalesHeadID    varchar(15),    
 SalesHeadName   varchar(100),    
 SalesCoordinatorID  varchar(15),    
 SalesCoordinatorName varchar(100),    
 SalesmanID    varchar(15),    
 SalesmanName   varchar(100),    
 JoinDate    varchar(8),    
 ResignDate    varchar(8),    
 GradeDate    varchar(8),    
 Grade     varchar(50),    
 ModelCatagory   varchar(100),    
 SalesType    varchar(100),    
 InvoiceNo    varchar(15),    
 InvoiceDate    varchar(8),    
 SoNo     varchar(15),    
 SalesModelCode   varchar(20),    
 SalesModelYear   varchar(4),    
 SalesModelDesc   varchar(100),    
 FakturPolisiDesc  varchar(100),    
 MarketModel    varchar(100),    
 ColourCode    varchar(15),    
 ColourName    varchar(100),    
 --ColourNameInd   varchar(100),    
 GroupMarketModel  varchar(100),    
 ColumnMarketModel  varchar(100),    
 ChassisCode    varchar(15),    
 ChassisNo    varchar(10),    
 EngineCode    varchar(15),    
 EngineNo    varchar(10),    
 FakturPolisiNo   varchar(15),    
 FakturPolisiDate  varchar(8),    
 Status     varchar(1),    
 CreatedBy    varchar(15),    
 CreatedDate    varchar(8),    
 LastUpdateBy   varchar(15),    
 LastUpdateDate   varchar(8)    
)    
    
insert into @T_SHIST_DTL    
select TOP 2000 a.Year    
 , a.Month    
 , a.BranchCode    
 , a.BranchName    
 , a.Area    
 , a.BranchHeadID    
 , replace(a.BranchHeadName,',','.') BranchHeadName    
 , a.SalesHeadID    
 , replace(a.SalesHeadName,',','.') SalesHeadName    
 , a.SalesCoordinatorID    
 , replace(a.SalesCoordinatorName,',','.') SalesCoordinatorName    
 , a.SalesmanID    
 , replace(replace(a.SalesmanName,',','.'),'''','`') SalesmanName    
 , convert(varchar, isnull(a.JoinDate, '19000101'),112)    
 , convert(varchar, isnull(a.ResignDate, '19000101'),112)    
 , convert(varchar, isnull(a.GradeDate, '19000101'),112)    
 , isnull(a.Grade,1)    
 , a.ModelCatagory    
 , a.SalesType    
 , a.InvoiceNo    
 , convert(varchar, isnull(a.InvoiceDate, '19000101'),112)    
 , a.SoNo    
 , a.SalesModelCode    
 , a.SalesModelYear    
 , a.SalesModelDesc    
 , a.FakturPolisiDesc    
 , a.MarketModel    
 , a.ColourCode    
 , a.ColourName    
 --, a.ColourNameInd  
 , a.GroupMarketModel    
 , a.ColumnMarketModel    
 , isnull(a.ChassisCode, '') ChassisCode    
 , convert(varchar,isnull(a.ChassisNo, 0)) ChassisNo    
 , isnull(a.EngineCode, '') EngineCode    
 , convert(varchar,isnull(a.EngineNo, 0)) EngineNo    
 , isnull(a.FakturPolisiNo, '') FakturPolisiNo    
 , convert(varchar, isnull(a.FakturPolisiDate, '19000101'),112) FakturPolisiDate    
 , a.Status    
 , a.CreatedBy    
 , convert(varchar, isnull(a.CreatedDate, '19000101'),112)    
 , a.LastUpdateBy    
 , convert(varchar, isnull(a.LastUpdateDate, '19000101'),112)    
from omHstInquirySales a    
where a.CompanyCode = @CompanyCode    
-- and a.LastUpdateBy not like 'TR#%'    
 and SalesHeadID = '00421'    
 --and isnull(a.CreatedDate, convert(datetime, '19000101'))    
 --> isnull((    
 --   select top 1 LastSendDate    
 --  from gnMstSendSchedule    
 --    where DataType = 'SHIST'    
 --    order by LastSendDate desc    
 --   ), convert(datetime, '18990101'))    
order by a.CreatedDate    
    
set @TotalItem = (select count(*) from @T_SHIST_DTL)     
set @LastDate  = (select top 1 CreatedDate from @T_SHIST_DTL order by CreatedDate desc)    
    
insert into @T_SHIST     
select 'HSHIST'     
+ left(@DealerCode + replicate(' ',10),10)    
+ left('2000000' + replicate(' ',10),10)    
+ left((select BranchName     
   from GnMstOrganizationDtl     
   where CompanyCode = @CompanyCode     
    and BranchCode = @BranchCode)     
  + replicate(' ',50),50)    
+ right(replicate('0', 6) + convert(varchar, @TotalItem), 6)    
+ (select top 1 case ProductType    
  when '2W' then 'A'     
  when '4W' then 'B'     
  else 'C'      
  end     
   from gnMstCoProfile    
  where CompanyCode = @CompanyCode    
    and BranchCode = @BranchCode)    
+ replicate(' ', 480)    
, 0    
, 0    
    
insert into @T_SHIST    
select distinct '1'    
 + LEFT(isnull(a.Year, '') + replicate(' ', 4), 4)    
 + RIGHT(replicate('0',2) + isnull(a.Month, ''), 2)    
 + LEFT(isnull(a.BranchCode,'') + replicate(' ', 15), 15)    
 + LEFT(isnull(a.BranchName,'') + replicate(' ', 100), 100)    
 + LEFT(isnull(a.Area,'') + replicate(' ', 100), 100)    
 + LEFT(isnull(a.BranchHeadID,'-') + replicate(' ', 15), 15)    
 + LEFT(isnull(a.BranchHeadName,'-') + replicate(' ', 100), 100)    
 + replicate(' ', 226)    
, LEFT(isnull(a.Year, '') + replicate(' ', 4), 4)    
 + RIGHT(replicate('0',2) + isnull(a.Month, ''), 2)    
 + LEFT(isnull(a.BranchCode,'') + replicate(' ', 15), 15)    
 + LEFT(isnull(a.BranchHeadID,'-') + replicate(' ', 15), 15)    
, '1'    
    
from @T_SHIST_DTL a    
group by a.Year    
 , a.Month    
 , a.BranchCode    
 , a.BranchName    
 , a.Area    
 , a.BranchHeadID    
 , a.BranchHeadName    
     
insert into  @T_SHIST    
select distinct '2'    
 + LEFT(isnull(a.SalesHeadID, '-') + replicate(' ', 15), 15)    
 + LEFT(isnull(a.SalesHeadName, '-') + replicate(' ', 100), 100)    
 + LEFT(isnull(a.SalesCoordinatorID, '-') + replicate(' ', 15), 15)    
 + LEFT(isnull(a.SalesCoordinatorName, '-') + replicate(' ', 100), 100)    
 + LEFT(isnull(a.SalesmanID, '-') + replicate(' ', 15), 15)    
 + LEFT(isnull(a.SalesmanName, '-') + replicate(' ', 100), 100)    
 + convert(varchar, isnull(a.JoinDate, '19000101'),112)    
 + convert(varchar, isnull(a.ResignDate, '19000101'),112)    
 + convert(varchar, isnull(a.GradeDate, '19000101'),112)    
 + LEFT(isnull(a.Grade, '') + replicate(' ', 50), 50)    
 + replicate(' ', 143)    
, LEFT(isnull(a.Year, '') + replicate(' ', 4), 4)    
 + RIGHT(replicate('0',2) + isnull(a.Month, ''), 2)    
 + LEFT(isnull(a.BranchCode,'') + replicate(' ', 15), 15)    
 + LEFT(isnull(a.BranchHeadID,'-') + replicate(' ', 15), 15)    
 + LEFT(isnull(a.SalesHeadID, '-') + replicate(' ', 15), 15)    
 + LEFT(isnull(a.SalesCoordinatorID, '-') + replicate(' ', 15), 15)    
 + LEFT(isnull(a.SalesmanID, '-') + replicate(' ', 15), 15)    
, '2'    
from @T_SHIST_DTL a    
group by a.Year    
 , a.Month    
 , a.BranchCode    
 , a.BranchName    
 , a.Area    
 , a.BranchHeadID    
 , a.BranchHeadName    
 , a.SalesHeadID    
 , a.SalesHeadName    
 , a.SalesCoordinatorID    
 , a.SalesCoordinatorName    
 , a.SalesmanID    
 , a.SalesmanName    
 , a.JoinDate    
 , a.ResignDate    
 , a.GradeDate    
 , a.Grade    
     
insert into @T_SHIST    
select distinct '3'    
 + LEFT(isnull(a.ModelCatagory, '') + replicate(' ', 100), 100)    
 + LEFT(isnull(a.SalesType, '') + replicate(' ', 100), 100)    
 + LEFT(isnull(a.InvoiceNo, '') + replicate(' ', 15), 15)    
 + convert(varchar, isnull(a.InvoiceDate, '19000101'),112)    
 + LEFT(isnull(a.SoNo, '') + replicate(' ', 15), 15)    
 + LEFT(isnull(a.SalesModelCode, '') + replicate(' ', 20), 20)    
 + RIGHT(replicate('0', 4) + isnull(a.SalesModelYear, '') , 4)    
 + LEFT(isnull(a.SalesModelDesc, '') + replicate(' ', 100), 100)    
 + LEFT(isnull(a.FakturPolisiDesc, '') + replicate(' ', 100), 100)    
 + LEFT(isnull(a.MarketModel, '') + replicate(' ', 100), 100)    
, LEFT(isnull(a.Year, '') + replicate(' ', 4), 4)    
 + RIGHT(replicate('0',2) + isnull(a.Month, ''), 2)    
 + LEFT(isnull(a.BranchCode,'') + replicate(' ', 15), 15)    
 + LEFT(isnull(a.BranchHeadID,'-') + replicate(' ', 15), 15)    
 + LEFT(isnull(a.SalesHeadID, '-') + replicate(' ', 15), 15)    
 + LEFT(isnull(a.SalesCoordinatorID, '-') + replicate(' ', 15), 15)    
 + LEFT(isnull(a.SalesmanID, '-') + replicate(' ', 15), 15)    
 + LEFT(isnull(a.ChassisCode, '') + replicate(' ', 15), 15)    
 + LEFT(isnull(a.ChassisNo, '') + replicate(' ', 10), 10)    
, LEFT(isnull(a.SoNo, '') + replicate(' ', 15), 15) + '(1)'    
from @T_SHIST_DTL a    
    
insert into @T_SHIST    
select distinct '4'    
 + LEFT(isnull(a.ColourCode, '') + replicate(' ', 15), 15)    
 + LEFT(isnull(a.ColourName, '') + replicate(' ', 100), 100)    
-- + LEFT(isnull(a.ColourNameInd, '') + replicate(' ', 100), 100)    
 + LEFT(isnull(a.GroupMarketModel, '') + replicate(' ', 100), 100)    
 + LEFT(isnull(a.ColumnMarketModel, '') + replicate(' ', 100), 100)    
 + isnull(a.Status, '0')    
 + LEFT(isnull(a.CreatedBy, '') + replicate(' ', 15), 15)    
 + convert(varchar, isnull(a.CreatedDate, '19000101'),112)    
 + LEFT(isnull(a.LastUpdateBy, '') + replicate(' ', 15), 15)    
 + convert(varchar, isnull(a.LastUpdateDate, '19000101'), 112)    
 + LEFT(isnull(a.ChassisCode, '') + replicate(' ', 15), 15)    
 + LEFT(isnull(a.ChassisNo, '') + replicate(' ', 10), 10)    
 + LEFT(isnull(a.EngineCode, '') + replicate(' ', 15), 15)    
 + LEFT(isnull(a.EngineNo, '') + replicate(' ', 10), 10)    
 + LEFT(isnull(a.FakturPolisiNo, '') + replicate(' ', 15), 15)    
 + convert(varchar, isnull(a.FakturPolisiDate, '19000101'), 112)    
 + replicate(' ', 27)    
, LEFT(isnull(a.Year, '') + replicate(' ', 4), 4)    
 + RIGHT(replicate('0',2) + isnull(a.Month, ''), 2)    
 + LEFT(isnull(a.BranchCode,'') + replicate(' ', 15), 15)    
 + LEFT(isnull(a.BranchHeadID,'-') + replicate(' ', 15), 15)    
 + LEFT(isnull(a.SalesHeadID, '-') + replicate(' ', 15), 15)    
 + LEFT(isnull(a.SalesCoordinatorID, '-') + replicate(' ', 15), 15)    
 + LEFT(isnull(a.SalesmanID, '-') + replicate(' ', 15), 15)    
 + LEFT(isnull(a.ChassisCode, '') + replicate(' ', 15), 15)    
 + LEFT(isnull(a.ChassisNo, '') + replicate(' ', 10), 10)    
,LEFT(isnull(a.SoNo, '') + replicate(' ', 15), 15) + '(2)'    
from @T_SHIST_DTL a    
    
select @LastDate as LastDate    
    
select line from @T_SHIST    
order by SeqNo,SeqNo1    
    
--;with x as(select TOP 2000 * from omHstInquirySales a    
--   where a.CompanyCode = @CompanyCode    
--   and a.LastUpdateBy not like 'TR#%')    
--update x set LastUpdateBy=('TR#' + LastUpdateBy) , LastUpdateDate = getdate() 
GO

if object_id('uspfn_HrGetTeamLeader222') is not null
	drop procedure uspfn_HrGetTeamLeader222
GO
CREATE procedure [dbo].[uspfn_HrGetTeamLeader222]
	@CompanyCode varchar(25),
	@DeptCode varchar(25),
	@PosCode varchar(25)
as

declare @table as table(value varchar(200), text varchar(200))
declare @curpos as varchar(200)
select Dept = @DeptCode
if(@DeptCode like '%SALES & MKT%')
	select 'true'
else
	select 'false'

select 
	a.*
from
	gnMstPosition a
where
	a.CompanyCode=@CompanyCode
	and
	a.DeptCode='SALES & MKT'

set @curpos = isnull((
				select top 1 PosHeader
				  from GnMstPosition
				 where CompanyCode = @CompanyCode
				   and DeptCode = @DeptCode
				   and PosCode = @PosCode
				  ), '') 

while (@curpos != '' and @DeptCode != 'COM')
begin
	insert into @table
	select a.EmployeeID, a.EmployeeName + ' (' + @curpos + ')' 
	  from HrEmployee a
	 where CompanyCode = @CompanyCode
	   and (Department = @DeptCode or Department = 'COM')
	   and Position = @curpos
   
	set @curpos = isnull((
					select top 1 PosHeader
					  from GnMstPosition
					 where CompanyCode = @CompanyCode
					   and (DeptCode = @DeptCode or DeptCode = 'COM')
					   and PosCode = @curpos
					  ), '') 
end

select * from @table

GO

if object_id('uspfn_OmFakturPajakDtlAccsSeqSaveDetail') is not null
	drop procedure uspfn_OmFakturPajakDtlAccsSeqSaveDetail
GO
Create procedure [dbo].[uspfn_OmFakturPajakDtlAccsSeqSaveDetail]   
(  
 @CompanyCode varchar(15),  
 @BranchCode varchar(15), 
 @InvoiceNo varchar(30) 
)  
AS  
BEGIN 
-- exec uspfn_OmFakturPajakDtlAccsSeqSaveDetail 6006410,600641001,''
INSERT INTO OmFakturPajakDetailAccsSeq(CompanyCode, BranchCode, InvoiceNo, PartNo, PartName, Quantity, 
    RetailPrice, DiscExcludePPn, DPP, PPn, Total)
SELECT a.CompanyCode
    , a.BranchCode
    , a.InvoiceNo
    , a.PartNo
    , isnull(b.PartName,'') PartName
    , a.Quantity Quantity
    , a.RetailPrice
    , a.DiscExcludePPn
    , a.DPP
    , a.PPn
    , a.Total
FROM omTrSalesInvoiceAccsSeq a
LEFT JOIN spMstItemInfo b ON b.CompanyCode = a.CompanyCode 
    AND b.PartNo = a.PartNo
WHERE a.CompanyCode = @CompanyCode 
    AND a.BranchCode = @BranchCode 
    AND a.InvoiceNo = @InvoiceNo
    AND a.PartNo NOT IN(SELECT PartNo FROM omFakturPajakDetailAccsSeq WHERE CompanyCode = a.CompanyCode 
        AND BranchCode = a.BranchCode AND InvoiceNo = a.InvoiceNo AND PartNo = a.PartNo)  
end     
GO

if object_id('uspfn_OmFakturPajakDtlDoSaveDO') is not null
	drop procedure uspfn_OmFakturPajakDtlDoSaveDO
GO
Create procedure [dbo].[uspfn_OmFakturPajakDtlDoSaveDO]   
(  
 @CompanyCode varchar(15),  
 @BranchCode varchar(15), 
 @InvoiceNo varchar(30) 
)  
AS  
BEGIN 
-- exec uspfn_OmFakturPajakDtlSaveDO 6006410,600641001,''
INSERT INTO OmFakturPajakDetailDO(CompanyCode, BranchCode, InvoiceNo, DONo)
 SELECT a.CompanyCode, a.BranchCode, a.InvoiceNo, b.DONo FROM omTrSalesInvoiceModel a
 INNER JOIN omTrSalesBPK b ON b.CompanyCode = a.CompanyCode AND b.BranchCode = a.BranchCode AND a.BPKNo = b.BPKNo
 WHERE a.CompanyCode = @CompanyCode AND a.BranchCode = @BranchCode AND a.InvoiceNo = @InvoiceNo
 AND b.DONo NOT IN(SELECT DONo FROM omFakturPajakDetailDO WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode
 AND InvoiceNo = a.InvoiceNo)
 GROUP BY a.CompanyCode, a.BranchCode, a.InvoiceNo, b.DONo
end     

GO

if object_id('uspfn_OmFakturPajakDtlOthersSaveDetail') is not null
	drop procedure uspfn_OmFakturPajakDtlOthersSaveDetail
GO

Create procedure [dbo].[uspfn_OmFakturPajakDtlOthersSaveDetail]   
(  
 @CompanyCode varchar(15),  
 @BranchCode varchar(15), 
 @InvoiceNo varchar(30),
 @ReffType varchar(15)
)  
AS  
BEGIN 
-- exec uspfn_OmFakturPajakDtlAccsSeqSaveDetail 6006410,600641001,''
INSERT INTO OmFakturPajakDetailOthers(CompanyCode, BranchCode, InvoiceNo, SalesModelCode, SalesModelYear, OtherCode, OtherName, Quantity, DPP, PPn, Total)
SELECT a.CompanyCode, a.BranchCode, a.InvoiceNo, a.SalesModelCode, a.SalesModelYear, a.OtherCode, b.RefferenceDesc1 AS OtherName, c.Quantity, a.AfterDiscDPP, a.AfterDiscPPn, a.AfterDiscTotal
FROM omTrSalesInvoiceOthers a
LEFT JOIN omMstRefference b ON b.CompanyCode = a.CompanyCode AND b.RefferenceCode = a.OtherCode AND b.RefferenceType = @ReffType
LEFT JOIN omTrSalesInvoiceModel c ON c.CompanyCode = a.CompanyCode AND c.BranchCode = a.BranchCode AND c.InvoiceNo = a.InvoiceNo AND c.BPKNo = a.BPKNo
AND c.SalesModelCode = a.SalesModelCode AND c.SalesModelYear = a.SalesModelYear
WHERE a.CompanyCode = @CompanyCode AND a.BranchCode = @BranchCode AND a.InvoiceNo = @InvoiceNo
AND a.OtherCode NOT IN(SELECT OtherCode FROM OmFakturPajakDetailOthers WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode
AND InvoiceNo = a.InvoiceNo AND SalesModelCode = a.SalesModelCode AND SalesModelYear = a.SalesModelYear AND OtherCode = a.OtherCode)
           
end     
GO

if object_id('uspfn_OmFakturPajakDtlSaveDetail') is not null
	drop procedure uspfn_OmFakturPajakDtlSaveDetail
GO

Create procedure [dbo].[uspfn_OmFakturPajakDtlSaveDetail]   
(  
 @CompanyCode varchar(15),  
 @BranchCode varchar(15), 
 @InvoiceNo varchar(30) 
)  
AS  
BEGIN 
-- exec uspfn_OmFakturPajakDtlSaveDetail 6006410,600641001,''
INSERT INTO OmFakturPajakDetail(CompanyCode, BranchCode, InvoiceNo, BPKNo, SalesModelCode, SalesModelYear, ChassisCode
, ChassisNo, EngineNo, BeforeDiscDPP, DiscExcludePPn, AfterDiscPPn, AfterDiscPPnBM, PPnBMPaid, OthersDPP, OthersPPN)
SELECT a.CompanyCode, a.BranchCode, a.InvoiceNo, a.BPKNo, a.SalesModelCode, a.SalesModelYear, a.ChassisCode, a.ChassisNo
, a.EngineNo, b.BeforeDiscDPP, b.DiscExcludePPn, b.AfterDiscPPn, b.AfterDiscPPnBM, b.PPnBMPaid, b.OthersDPP, b.OthersPPN
FROM omTrSalesInvoiceVin a
LEFT JOIN omTrSalesInvoiceModel b ON b.CompanyCode = a.CompanyCode AND b.BranchCode = a.BranchCode AND b.InvoiceNo = a.InvoiceNo
AND b.BPKNo = a.BPKNo AND b.SalesModelCode = a.SalesModelCode AND b.SalesModelYear = a.SalesModelYear
WHERE a.CompanyCode = @CompanyCode AND a.BranchCode = @BranchCode AND a.InvoiceNo = @InvoiceNo
AND a.ChassisNo NOT IN(SELECT ChassisNo FROM omFakturPajakDetail WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode
AND InvoiceNo = a.InvoiceNo AND SalesModelCode = a.SalesModelCode AND ChassisCode = a.ChassisCode)
            
end     
GO

if object_id('uspfn_OmGetJournalDebetCredit') is not null
	drop procedure uspfn_OmGetJournalDebetCredit
GO
--declare @CompanyCode varchar(20)
--declare @BranchCode  varchar(20)
--declare @TypeJournal  varchar(20)
--declare @DocNo   varchar(20)

--set @CompanyCode = '6558201'
--set @BranchCode  = '655820100'
--set @TypeJournal = 'invoice'
--set @DocNo       = 'IVU/13/001280'

-- =============================================
-- Author:		<xxxxxx>
-- Create date: <xxxxxx>
-- Description:	<Get Journal>
-- Last Update By:	<yo - 29 Nov 2013>
-- =============================================

CREATE procedure [dbo].[uspfn_OmGetJournalDebetCredit]
	@CompanyCode varchar(20),
	@BranchCode  varchar(20),
	@TypeJournal varchar(50),
	@DocNo       varchar(50)
as 

declare @t_journal as table (
	SeqCode     varchar(50),
	TypeTrans   varchar(50),
	AccountNo   varchar(50),
	AccountDesc varchar(100),
	AmountDb    decimal,
	AmountCr    decimal
)

--#region TypeJournal = 'TRANSFEROUT'
if @TypeJournal = 'TRANSFEROUT'
begin
	declare @t_trans_out as table (
		CompanyCode varchar(50),
		BranchCode  varchar(50),
		DocInfo     varchar(50),
		Amount      decimal
	)

	insert into @t_trans_out
	select a.CompanyCode, a.BranchCode, a.SalesModelCode 
		 , isnull(b.CogsUnit, 0) + isnull(b.COGSKaroseri, 0) + isnull(b.COGSOthers, 0)
	  from omTrInventTransferOutDetail a
	  left join omMstVehicle b on 1 = 1
	   and b.CompanyCode = a.CompanyCode
	   and b.ChassisCode = a.ChassisCode
	   and b.ChassisNo   = a.ChassisNo
	   and b.EngineCode  = a.EngineCode
	   and b.EngineNo    = a.EngineNo
	   and b.SalesModelCode = a.SalesModelCode
	   and b.SalesModelYear = a.SalesModelYear
	 where 1 = 1 
	   and a.CompanyCode = @CompanyCode
	   and a.BranchCode  = @BranchCode
	   and a.TransferOutNo = @DocNo

	insert into @t_journal
	select '01', 'PSEMENTARA'
		 , isnull(b.InTransitTransferStockAccNo,'')
		 , isnull(c.Description,'')
		 , a.Amount
		 , 0
	  from @t_trans_out a
		left join omMstModelAccount b on a.CompanyCode=b.CompanyCode and a.BranchCode=b.BranchCode
			and a.DocInfo=b.SalesModelCode
		left join gnMstAccount c on b.CompanyCode=c.CompanyCode and b.BranchCode=c.BranchCode
			and b.InTransitTransferStockAccNo= c.AccountNo

	insert into @t_journal
	select '02', 'INVENTORY'
		 , isnull(b.InventoryAccNo,'')
		 , isnull(c.Description,'')
		 , 0
		 , a.Amount
	  from @t_trans_out a
		left join omMstModelAccount b on a.CompanyCode=b.CompanyCode and a.BranchCode=b.BranchCode
			and a.DocInfo=b.SalesModelCode
		left join gnMstAccount c on b.CompanyCode=c.CompanyCode and b.BranchCode=c.BranchCode
			and b.InventoryAccNo= c.AccountNo
end

--#region TypeJournal = 'TRANSFERIN'
if @TypeJournal = 'TRANSFERIN'
begin
	declare @t_trans_in as table (
		CompanyCode varchar(50),
		BranchCode  varchar(50),
		BranchCodeFrom varchar(50),
		BranchCodeTo varchar(50),
		DocInfo     varchar(50),
		Amount      decimal
	)

	insert into @t_trans_in
	select a.CompanyCode, a.BranchCode, c.BranchCodeFrom, c.BranchCodeTo, a.SalesModelCode 
		 , isnull(b.CogsUnit, 0) + isnull(b.COGSKaroseri, 0) + isnull(b.COGSOthers, 0)
	  from omTrInventTransferInDetail a
	  left join omMstVehicle b on 1 = 1
	   and b.CompanyCode = a.CompanyCode
	   and b.ChassisCode = a.ChassisCode
	   and b.ChassisNo   = a.ChassisNo
	   and b.EngineCode  = a.EngineCode
	   and b.EngineNo    = a.EngineNo
	   and b.SalesModelCode = a.SalesModelCode
	   and b.SalesModelYear = a.SalesModelYear
	  left join omTrInventTransferIn c on 1 = 1
	   and c.CompanyCode = a.CompanyCode
	   and c.BranchCode  = a.BranchCode
	   and c.TransferInNo = a.TransferInNo
	 where 1 = 1 
	   and a.CompanyCode = @CompanyCode
	   and a.BranchCode  = @BranchCode
	   and a.TransferInNo = @DocNo

	insert into @t_journal
	select '01', 'INVENTORY'
		 , isnull(b.InventoryAccNo,'')
		 , isnull(c.Description,'')
		 , a.Amount
		 , 0
	  from @t_trans_in a
		left join omMstModelAccount b on a.CompanyCode=b.CompanyCode and a.BranchCodeTo=b.BranchCode
			and a.DocInfo=b.SalesModelCode
		left join gnMstAccount c on b.CompanyCode=c.CompanyCode and b.BranchCode=c.BranchCode
			and b.InventoryAccNo= c.AccountNo

	insert into @t_journal
	select '02', 'PSEMENTARA'
		 , isnull(b.InTransitTransferStockAccNo,'')
		 , isnull(c.Description,'')
		 , 0
		 , a.Amount
	  from @t_trans_in a
		left join omMstModelAccount b on a.CompanyCode=b.CompanyCode and a.BranchCodeFrom=b.BranchCode
			and a.DocInfo=b.SalesModelCode
		left join gnMstAccount c on b.CompanyCode=c.CompanyCode and b.BranchCode=c.BranchCode
			and b.InTransitTransferStockAccNo= c.AccountNo

end
--#endregion

--#region TypeJournal = 'TRANSFEROUTMULTIBRANCH'
if @TypeJournal = 'TRANSFEROUTMULTIBRANCH'
begin
	declare @t_trans_outMB as table (
	CompanyCode		varchar(50),
	BranchCode		varchar(50),
	CompanyCodeTo	varchar(50),
	DocInfo			varchar(50),
	Amount			decimal
	)

	insert into @t_trans_outMB
		select a.CompanyCode
			, a.BranchCode
			, b.CompanyCodeTo
			, a.SalesModelCode 
			, isnull(a.CogsUnit, 0) + isnull(a.COGSKaroseri, 0) + isnull(a.COGSOthers, 0)
		from omTrInventTransferOutDetailMultiBranch a
		left join omTrInventTransferOutMultiBranch b on b.CompanyCode = a.CompanyCode
			and b.BranchCode = a.BranchCode
			and b.TransferOutNo = a.TransferOutNo
		where 1 = 1 
		   and a.CompanyCode = @CompanyCode
		   and a.BranchCode  = @BranchCode
		   and a.TransferOutNo = @DocNo
		   
	insert into @t_journal
	select '01', 'PSEMENTARA'
		 , isnull(b.InterCompanyAccNoTo,'')
		 , isnull(c.Description,'')
		 , a.Amount
		 , 0
	from @t_trans_outMB a
	left join omMstCompanyAccount b on b.CompanyCode = a.CompanyCode
		and b.CompanyCodeTo = a.CompanyCodeTo
	left join gnMstAccount c on b.CompanyCode=c.CompanyCode and a.BranchCode=c.BranchCode
		and b.InterCompanyAccNoTo = c.AccountNo

	insert into @t_journal
	select '02', 'INVENTORY'
		 , isnull(b.InventoryAccNo,'')
		 , isnull(c.Description,'')
		 , 0
		 , a.Amount
	from @t_trans_outMB a
	left join omMstModelAccount b on a.CompanyCode=b.CompanyCode and a.BranchCode=b.BranchCode
		and a.DocInfo=b.SalesModelCode
	left join gnMstAccount c on b.CompanyCode=c.CompanyCode and b.BranchCode=c.BranchCode
		and b.InventoryAccNo= c.AccountNo
end
--#endregion

--#region TypeJournal = 'TRANSFERINMULTIBRANCH'
if @TypeJournal = 'TRANSFERINMULTIBRANCH'
begin
	declare @t_trans_inMB as table (
	CompanyCode		varchar(50),
	BranchCode		varchar(50),
	CompanyCodeFrom	varchar(50),
	BranchCodeFrom	varchar(50),
	BranchCodeTo	varchar(50),
	DocInfo			varchar(50),
	Amount			decimal
)

insert into @t_trans_inMB
	select a.CompanyCode
		, a.BranchCode
		, b.CompanyCodeFrom
		, b.BranchCodeFrom
		, b.BranchCodeTo
		, a.SalesModelCode 
		, isnull(a.CogsUnit, 0) + isnull(a.COGSKaroseri, 0) + isnull(a.COGSOthers, 0)
	from omTrInventTransferInDetailMultiBranch a
	left join omTrInventTransferInMultiBranch b on 1 = 1
	   and b.CompanyCode	= a.CompanyCode
	   and b.BranchCode		= a.BranchCode
	   and b.TransferInNo	= a.TransferInNo
	where 1 = 1 
	   and a.CompanyCode	= @CompanyCode
	   and a.BranchCode		= @BranchCode
	   and a.TransferInNo	= @DocNo

	insert into @t_journal
	select '01', 'INVENTORY'
		 , isnull(b.InventoryAccNo,'')
		 , isnull(c.Description,'')
		 , a.Amount
		 , 0
	  from @t_trans_inMB a
		left join omMstModelAccount b on a.CompanyCode=b.CompanyCode and a.BranchCodeTo=b.BranchCode
			and a.DocInfo=b.SalesModelCode
		left join gnMstAccount c on b.CompanyCode=c.CompanyCode and b.BranchCode=c.BranchCode
			and b.InventoryAccNo= c.AccountNo
			
	insert into @t_journal
	select '02', 'PSEMENTARA'
		 , isnull(b.InterCompanyAccNoTo,'')
		 , isnull(c.Description,'')
		 , 0
		 , a.Amount
	from @t_trans_inMB a
	left join omMstCompanyAccount b on b.CompanyCode = a.CompanyCode
		and b.CompanyCodeTo = a.CompanyCodeFrom
	left join gnMstAccount c on b.CompanyCode=c.CompanyCode and a.BranchCode=c.BranchCode
		and b.InterCompanyAccNoTo = c.AccountNo

end
--#endregion

--#region TypeJournal = 'KAROSERI'
if @TypeJournal = 'KAROSERI'
begin
	declare @t_karoseri as table (
		SeqCode     varchar(50),
		TypeTrans   varchar(50),
		AccountNo   varchar(50),
		AmountDb    decimal,
		AmountCr    decimal
	)
	
	insert into @t_karoseri
	select '01', 'INVENTORY'
		 , isnull((
			select acc.InventoryAccNo
			  from omMstModelAccount acc
			 where 1 = 1
			   and acc.CompanyCode    = a.CompanyCode
			   and acc.BranchCode     = a.BranchCode
			   and acc.SalesModelCode = a.SalesModelCodeNew
			), '')
		 , sum(isnull(c.COGSKaroseri, 0) + (isnull(c.COGsUnit, 0) + isnull(c.COGsOthers, 0)))
		 , 0
	  from OmTrPurchaseKaroseriTerima a, OmTrPurchaseKaroseriTerimaDetail b, OmMstVehicle c
	 where 1 = 1
	   and c.CompanyCode = b.CompanyCode 
	   and c.ChassisCode = b.ChassisCode
	   and c.ChassisNo = b.ChassisNo 
	   and b.CompanyCode = a.CompanyCode 
	   and b.BranchCode = a.BranchCode
	   and b.KaroseriTerimaNo = a.KaroseriTerimaNo
	   and a.CompanyCode = @CompanyCode 
	   and a.BranchCode  = @BranchCode
	   and a.KaroseriTerimaNo = @DocNo 
	 group by a.CompanyCode, a.BranchCode, a.KaroseriTerimaNo, a.SalesModelCodeNew
	having sum(isnull(a.Quantity, 0) * (isnull(a.Total, 0) - isnull(a.PPn, 0))) > 0

	insert into @t_karoseri
	select '02', 'PPN'
		 , isnull((
			select cls.TaxInAccNo
			  from gnMstSupplierProfitCenter sup, gnMstSupplierClass cls
			 where 1 = 1
			   and cls.CompanyCode   = sup.CompanyCode
			   and cls.BranchCode    = sup.BranchCode
			   and cls.SupplierClass = sup.SupplierClass
			   and sup.CompanyCode   = a.CompanyCode
			   and sup.BranchCode    = a.BranchCode
			   and sup.SupplierCode  = a.SupplierCode
			   and sup.ProfitCenterCode = '100'
			), '')
		 , sum(isnull(a.Quantity, 0) * isnull(a.PPn, 0))
		 , 0
	  from OmTrPurchaseKaroseriTerima a
	 where 1 = 1
	   and a.CompanyCode = @CompanyCode 
	   and a.BranchCode  = @BranchCode
	   and a.KaroseriTerimaNo = @DocNo 
	 group by a.CompanyCode, a.BranchCode, a.KaroseriTerimaNo, a.SalesModelCodeNew, a.SupplierCode
	having sum(isnull(a.Quantity, 0) * (isnull(a.Total, 0) - isnull(a.PPn, 0))) > 0

	insert into @t_karoseri
	select '03', 'INVENTORY'
		 , isnull((
			select acc.InventoryAccNo
			  from omMstModelAccount acc
			 where 1 = 1
			   and acc.CompanyCode    = a.CompanyCode
			   and acc.BranchCode     = a.BranchCode
			   and acc.SalesModelCode = a.SalesModelCodeOld
			), '')
		 , 0
		 , sum((isnull (c.COGsUnit, 0) + isnull (c.COGsOthers, 0 )))
	  from OmTrPurchaseKaroseriTerima a, OmTrPurchaseKaroseriTerimaDetail b, OmMstVehicle c
	 where 1 = 1
	   and c.CompanyCode = b.CompanyCode 
	   and c.ChassisCode = b.ChassisCode
	   and c.ChassisNo = b.ChassisNo 
	   and b.CompanyCode = a.CompanyCode 
	   and b.BranchCode = a.BranchCode
	   and b.KaroseriTerimaNo = a.KaroseriTerimaNo
	   and a.CompanyCode = @CompanyCode 
	   and a.BranchCode  = @BranchCode
	   and a.KaroseriTerimaNo = @DocNo 
	 group by a.CompanyCode, a.BranchCode, a.KaroseriTerimaNo, a.SalesModelCodeOld
	having sum(isnull(a.Quantity, 0) * (isnull(a.Total, 0) - isnull(a.PPn, 0))) > 0

	insert into @t_karoseri
	select '04', 'AP'
		 , isnull((
			select cls.PayableAccNo
			  from gnMstSupplierProfitCenter sup, gnMstSupplierClass cls
			 where 1 = 1
			   and cls.CompanyCode   = sup.CompanyCode
			   and cls.BranchCode    = sup.BranchCode
			   and cls.SupplierClass = sup.SupplierClass
			   and sup.CompanyCode   = a.CompanyCode
			   and sup.BranchCode    = a.BranchCode
			   and sup.SupplierCode  = a.SupplierCode
			   and sup.ProfitCenterCode = '100'
			), '')
		 , 0
		 , sum(isnull(a.Quantity, 0) * isnull(a.Total, 0))
	  from OmTrPurchaseKaroseriTerima a
	 where 1 = 1
	   and a.CompanyCode = @CompanyCode 
	   and a.BranchCode  = @BranchCode
	   and a.KaroseriTerimaNo = @DocNo 
	 group by a.CompanyCode, a.BranchCode, a.KaroseriTerimaNo, a.SupplierCode
	having sum(isnull(a.Quantity, 0) * (isnull(a.Total, 0) - isnull(a.PPn, 0))) > 0

	insert into @t_journal
	select a.SeqCode, a.TypeTrans, a.AccountNo
		 , b.Description AccountDesc, a.AmountDb, a.AmountCr    
	  from @t_karoseri a, gnMstAccount b
	 where b.CompanyCode = @CompanyCode
	   and b.BranchCode = @BranchCode 
	   and b.AccountNo = a.AccountNo
end
--#endregion

--#region TRANS TYPE PURCHASE
IF @TypeJournal = 'PURCHASE'
	BEGIN	
	Select * into #t1 from
	(
		select distinct a.CompanyCode
				, a.BranchCode
				, a.HPPNo
				, a.BPUNo
				, a.SalesModelCode
				, a.SalesModelYear
				, a.OthersCode
				, isnull(b.OthersNonInventoryAccNo,'') AccountNo
				, SUM(a.OthersDPP) DPP
				, SUM(a.OthersPPN) PPN
		from omTrPurchaseHPPDetailModelOthers a
		left join omMstOthersNonInventory b on a.CompanyCode = b.CompanyCode
			and a.BranchCode = b.BranchCode
			--and a.OthersCode = b.OthersNonInventory
		where a.CompanyCode = @CompanyCode
			and a.BranchCode = @BrancHCode
			and a.HPPNo = @DocNo
			and isnull(b.OthersNonInventoryAccNo,'') <> ''
		group by a.CompanyCode, a.BranchCode, a.HPPNo, a.BPUNo, a.SalesModelCode, a.SalesModelYear, a.OthersCode,b.OthersNonInventoryAccNo		
	)#t1


	select * into #Inventory from(
	select 'INVENTORY' CodeTrans
		 , @DocNo DocNo
		 , a.SalesModelCode DocInfo
		 , isnull((
			select acc.InventoryAccNo
			  from omMstModelAccount acc
			 where 1 = 1
			   and acc.CompanyCode    = a.CompanyCode
			   and acc.BranchCode     = a.BranchCode
			   and acc.SalesModelCode = a.SalesModelCode
			), '') AccountNo
		 , isnull(a.Quantity, 0) Quantity
		 , isnull(a.AfterDiscDPP, 0) AfterDiscDPP	 
		 , case when (select COUNT(*) from #t1 where HPPNo = a.HppNo and BPUNo = a.BPUNo and SalesModelCode = a.SalesModelCode and SalesModelYear = SalesModelYear) > 0 
			   then isnull((select distinct (b.DPP)
					from #t1 b
					where b.CompanyCode = a.CompanyCode
						and b.BranchCode = a.BranchCode
						and b.HPPNo = a.HPPNo
						and b.SalesModelCode = a.SalesModelCode
						and b.SalesModelYear = a.SalesModelYear
						and b.OthersCode not in (select distinct OthersNonInventory 
								from omMstOthersNonInventory))
				, 0) else a.OthersDPP end OthersDPP
		 , isnull(a.AfterDiscPPnBM, 0) AfterDiscPPnBM
		 , 0 AmountCr
	  from omTrPurchaseHPPDetailModel a
	 where 1 = 1
	   and a.CompanyCode = @CompanyCode 
	   and a.BranchCode  = @BranchCode
	   and a.HPPNo       = @DocNo 
	)#Inventory

	insert into @t_journal
	select	1
			, a.CodeTrans
			, a.AccountNo
			, '' AccountDesc
			, sum(isnull(a.Quantity, 0) * (isnull(a.AfterDiscDPP, 0) + isnull(a.OthersDPP, 0) + isnull(a.AfterDiscPPnBM, 0))) as DPP
			, 0
	  from #Inventory a
	 group by a.CodeTrans, a.DocNo, a.DocInfo, a.AccountNo
	having sum(isnull(a.Quantity, 0) * (isnull(a.AfterDiscDPP, 0) + isnull(a.OthersDPP, 0) + isnull(a.AfterDiscPPnBM, 0))) > 0

	select * into #OthersInv from(
	select 'OTHERS' CodeTrans 		
		 , isnull((
			select acc.OthersNonInventoryAccNo
			  from omMstOthersNonInventory acc
			 where 1 = 1
			   and acc.CompanyCode    = a.CompanyCode
			   and acc.BranchCode     = a.BranchCode
			   and acc.OthersNonInventory = b.OthersCode
			), '') AccountNo
		 , '' AccountDesc
		 , sum(isnull(a.Quantity, 0) * isnull(b.DPP, 0)) as DPP
		 , 0 AmountCr
	  from omTrPurchaseHPPDetailModel a, #t1 b
	 where 1 = 1
	   and a.CompanyCode = @CompanyCode 
	   and a.BranchCode  = @BranchCode
	   and a.HPPNo       = @DocNo 
	   and a.SalesModelCode = b.SalesModelCode
	   and a.SalesModelYear = b.SalesModelYear
	   and a.BPUNo = b.BPUNo
	   and b.OthersCode in (select distinct OthersNonInventory 
									from omMstOthersNonInventory) 
	 group by a.CompanyCode, a.BranchCode, a.HPPNo, a.SalesModelCode,a.SalesModelYear,b.OthersCode,b.AccountNo
	having sum(isnull(a.Quantity, 0) * isnull(b.DPP, 0)) > 0
	)#OthersInv
	
	insert into @t_journal
	select 2, CodeTrans			
			, AccountNo
			, '' AccountDesc
			, SUM(DPP)
			, AmountCr
	  from #OthersInv a
	 group by a.CodeTrans, a.AccountNo,a.AmountCr
	having sum(DPP) > 0

	insert into @t_journal
	select 3, 'PPN'
		 , isnull((
			select cls.TaxInAccNo
			  from omTrPurchaseHPP pur, gnMstSupplierProfitCenter sup, gnMstSupplierClass cls
			 where 1 = 1
			   and sup.CompanyCode   = pur.CompanyCode
			   and sup.BranchCode    = pur.BranchCode
			   and sup.SupplierCode  = pur.SupplierCode
			   and sup.ProfitCenterCode = '100'
			   and cls.CompanyCode   = pur.CompanyCode
			   and cls.BranchCode    = pur.BranchCode
			   and cls.SupplierClass = sup.SupplierClass
			   and sup.CompanyCode   = @CompanyCode
			   and sup.BranchCode    = @BranchCode
			   and pur.HPPNo         = @DocNo
			), '')
		 , '' AccountDesc
		 , sum(isnull(a.Quantity, 0) * (isnull(a.AfterDiscPPn, 0) + isnull(a.OthersPPn, 0))) as PPN
		 , 0
	  from omTrPurchaseHPPDetailModel a, omTrPurchaseHPP b
	 where 1 = 1
	   and b.CompanyCode = a.CompanyCode 
	   and b.BranchCode  = a.BranchCode
	   and b.HPPNo       = a.HPPNo
	   and a.CompanyCode = @CompanyCode 
	   and a.BranchCode  = @BranchCode
	   and a.HPPNo       = @DocNo 
	 group by a.CompanyCode, a.BranchCode, a.HPPNo, b.SupplierCode
	having sum(isnull(a.Quantity, 0) * (isnull(a.AfterDiscDPP, 0) + isnull(a.OthersDPP, 0) + isnull(a.AfterDiscPPnBM, 0))) > 0

	insert into @t_journal
	select 4, 'AP'
		 , isnull((
			select cls.PayableAccNo
			  from omTrPurchaseHPP pur, gnMstSupplierProfitCenter sup, gnMstSupplierClass cls
			 where 1 = 1
			   and sup.CompanyCode   = pur.CompanyCode
			   and sup.BranchCode    = pur.BranchCode
			   and sup.SupplierCode  = pur.SupplierCode
			   and sup.ProfitCenterCode = '100'
			   and cls.CompanyCode   = pur.CompanyCode
			   and cls.BranchCode    = pur.BranchCode
			   and cls.SupplierClass = sup.SupplierClass
			   and sup.CompanyCode   = @CompanyCode
			   and sup.BranchCode    = @BranchCode
			   and pur.HPPNo         = @DocNo
			), '')
		 , '' AccountDesc
		 , 0
		 , sum(isnull(a.Quantity, 0) * (isnull(a.AfterDiscDPP, 0) + isnull(a.OthersDPP, 0) + isnull(a.AfterDiscPPnBM, 0)))
		 + sum(isnull(a.Quantity, 0) * (isnull(a.AfterDiscPPn, 0) + isnull(a.OthersPPn, 0))) as TotalTransAmt
	  from omTrPurchaseHPPDetailModel a, omTrPurchaseHPP b
	 where 1 = 1
	   and b.CompanyCode = a.CompanyCode 
	   and b.BranchCode  = a.BranchCode
	   and b.HPPNo       = a.HPPNo
	   and a.CompanyCode = @CompanyCode 
	   and a.BranchCode  = @BranchCode
	   and a.HPPNo       = @DocNo 
	 group by a.CompanyCode, a.BranchCode, a.HPPNo, b.SupplierCode
	having sum(isnull(a.Quantity, 0) * (isnull(a.AfterDiscDPP, 0) + isnull(a.OthersDPP, 0) + isnull(a.AfterDiscPPnBM, 0))) > 0	

	drop table #t1
	drop table #Inventory
	drop table #OthersInv
		
	END
--#endregion

--#region TypeJournal = 'INVOICE'
IF @TypeJournal = 'INVOICE'
BEGIN
	insert into @t_journal
		select 1, 'AR'
			 , isnull((
				select cls.ReceivableAccNo
				  from omTrSalesInvoice ivu, GnMstCustomerProfitCenter cus, GnMstCustomerClass cls
				 where 1 = 1
				   and cus.CompanyCode   = ivu.CompanyCode
				   and cus.BranchCode    = ivu.BranchCode
				   and cus.CustomerCode  = ivu.CustomerCode
				   and cus.ProfitCenterCode = '100'
				   and cls.CompanyCode   = ivu.CompanyCode
				   and cls.BranchCode    = ivu.BranchCode
				   and cls.CustomerClass = cus.CustomerClass
				   and cus.CompanyCode   = @CompanyCode
				   and cus.BranchCode    = @BranchCode
				   and ivu.InvoiceNo     = @DocNo
				), '') AccounNo
			 , '' AccountDesc
			 , isnull((
				select sum(isnull(Quantity, 0) * (AfterDiscDPP + AfterDiscPPn + AfterDiscPPnBm))
				  from omTrSalesInvoiceModel
				 where 1 = 1
				   and CompanyCode = @CompanyCode 
				   and BranchCode  = @BranchCode
				   and InvoiceNo   = @DocNo
				), 0)
			 + isnull((
				select sum(mdl.Quantity * (oth.AfterDiscDPP + oth.AfterDiscPPn))
				  from omTrSalesInvoiceOthers oth left join omTrSalesInvoiceModel mdl
					on oth.BranchCode = mdl.BranchCode
					and oth.InvoiceNo = mdl.InvoiceNo
					and oth.BPKNo = mdl.BPKNo
					and oth.SalesModelCode = mdl.SalesModelCode
				 where 1 = 1
				   and oth.CompanyCode = @CompanyCode 
				   and oth.BranchCode  = @BranchCode
				   and oth.InvoiceNo   = @DocNo
				), 0)
			 + isnull((
				select sum(DPP + PPN)
				  from omTrSalesInvoiceAccs
				 where 1 = 1
				   and CompanyCode = @CompanyCode 
				   and BranchCode  = @BranchCode
				   and InvoiceNo   = @DocNo
				), 0)
			 + (select isnull(sum(isnull(Quantity,0)*isnull(Total,0)),0) from omTrSalesInvoiceAccsSeq where CompanyCode=@CompanyCode
				   and BranchCode=@BranchCode and InvoiceNo=@DocNo) 
			 , 0

insert into @t_journal
select 2, 'DISCOUNT UNIT'
	 , isnull((
		select acc.DiscountAccNo
		  from omMstModelAccount acc
		 where 1 = 1
		   and acc.CompanyCode    = a.CompanyCode
		   and acc.BranchCode     = a.BranchCode
		   and acc.SalesModelCode = a.SalesModelCode
		), '') AccountNo
	 , '' AccountDesc
	 , sum(isnull(a.Quantity, 0) * isnull (a.DiscExcludePPn, 0)) as Discount
	 , 0
  from omTrSalesInvoiceModel a
 where 1 = 1
   and a.CompanyCode = @CompanyCode 
   and a.BranchCode  = @BranchCode
   and a.InvoiceNo   = @DocNo 
 group by a.CompanyCode, a.BranchCode, a.InvoiceNo, a.BPKNo, a.SalesModelCode, a.SalesModelYear
having sum(isnull(a.Quantity, 0) * isnull (a.DiscExcludePPn, 0)) > 0

insert into @t_journal
select 3, 'DISCOUNT AKSESORIS'
	 , isnull((
		select acc.DiscountAccNoAks
		  from omMstModelAccount acc
		 where 1 = 1
		   and acc.CompanyCode    = a.CompanyCode
		   and acc.BranchCode     = a.BranchCode
		   and acc.SalesModelCode = a.SalesModelCode
		), '') AccountNo
	 , '' AccountDesc
	 , sum(isnull(a.DiscExcludePPn, 0)) as Discount
	 , 0
  from omTrSalesInvoiceOthers a
 where 1 = 1
   and a.CompanyCode = @CompanyCode 
   and a.BranchCode  = @BranchCode
   and a.InvoiceNo   = @DocNo 
 group by a.CompanyCode, a.BranchCode, a.InvoiceNo, a.BPKNo, a.SalesModelCode, a.SalesModelYear
having sum(isnull(a.DiscExcludePPn, 0)) > 0

insert into @t_journal
select distinct 4, 'DISCOUNT SPAREPART['+a.TypeOfGoods+']'
	, (select top 1 DiscAccNo from spMstAccount where companycode=a.companycode and branchcode=a.branchcode and typeofgoods=a.typeofgoods) AccountNo
	, '' AccountDesc
	, (select sum(isnull(Quantity,0)*isnull(DiscExcludePPn,0)) from omtrsalesinvoiceaccsseq where companycode=a.companycode and branchcode=a.branchcode 
		and invoiceno=a.invoiceno and typeofgoods=a.typeofgoods group by typeofgoods) AmountDb
	, 0 AmountCr
from omTrSalesInvoiceAccsSeq a 
inner join omTrSalesInvoice b on b.CompanyCode=a.CompanyCode
	and b.BranchCode=a.BranchCode
	and b.InvoiceNo=a.InvoiceNo
where a.companyCode=@CompanyCode 
	and a.BranchCode=@BranchCode 
	and a.InvoiceNo=@DocNo

insert into @t_journal
select 5, 'SALES UNIT'
	 , isnull((
		select acc.SalesAccNo
		  from omMstModelAccount acc
		 where 1 = 1
		   and acc.CompanyCode    = a.CompanyCode
		   and acc.BranchCode     = a.BranchCode
		   and acc.SalesModelCode = a.SalesModelCode
		), '') AccountNo
	 , '' AccountDesc
	 , 0
	 , sum(isnull(a.Quantity, 0) * isnull (a.AfterDiscDPP, 0))
	 + sum(isnull(a.Quantity, 0) * isnull (a.DiscExcludePPn, 0)) 
  from omTrSalesInvoiceModel a
 where 1 = 1
   and a.CompanyCode = @CompanyCode 
   and a.BranchCode  = @BranchCode
   and a.InvoiceNo   = @DocNo 
 group by a.CompanyCode, a.BranchCode, a.InvoiceNo, a.BPKNo, a.SalesModelCode, a.SalesModelYear
having (sum(isnull(a.Quantity, 0) * isnull (a.AfterDiscDPP, 0)) +
	    sum(isnull(a.Quantity, 0) * isnull (a.DiscExcludePPn, 0))) > 0

insert into @t_journal
select 6, 'SALES AKSESORIS'
	 , isnull((
		select acc.SalesAccNoAks
		  from omMstModelAccount acc
		 where 1 = 1
		   and acc.CompanyCode    = a.CompanyCode
		   and acc.BranchCode     = a.BranchCode
		   and acc.SalesModelCode = a.SalesModelCode
		), '') AccountNo
	 , '' AccountDesc
	 , 0
	 , sum(isnull(b.Quantity, 0) * isnull (a.AfterDiscDPP, 0))
	 + sum(isnull(b.Quantity, 0) * isnull (a.DiscExcludePPn, 0)) 
  from omTrSalesInvoiceOthers a, omTrSalesInvoiceModel b
 where 1 = 1
   and b.BranchCode = a.BranchCode 
   and b.InvoiceNo = a.InvoiceNo 
   and b.BPKNo = a.BPKNo 
   and b.SalesModelCode = a.SalesModelCode 
   and b.SalesModelYear = a.SalesModelYear 
   and a.CompanyCode = @CompanyCode 
   and a.BranchCode  = @BranchCode
   and a.InvoiceNo   = @DocNo 
 group by a.CompanyCode, a.BranchCode, a.InvoiceNo, a.BPKNo, a.SalesModelCode, a.SalesModelYear
having (sum(isnull(b.Quantity, 0) * isnull (a.AfterDiscDPP, 0)) +
	    sum(isnull(b.Quantity, 0) * isnull (a.DiscExcludePPn, 0))) > 0

insert into @t_journal
select distinct 7, 'SALES SPAREPART ['+a.typeOfGoods+']'
	, (select top 1 SalesAccNo from spMstAccount where companycode=a.companycode and branchcode=a.branchcode and typeofgoods=a.typeofgoods) AccountNo
	, '' AccountDesc
	, 0 AmountDb
	, (select sum(isnull(Quantity,0) * isnull(RetailPrice,0)) from omtrsalesinvoiceaccsseq where companycode=a.companycode and branchcode=a.branchcode 
		and invoiceno=a.invoiceno and typeofgoods=a.typeofgoods group by typeofgoods) AmountCr
from omTrSalesInvoiceAccsSeq a 
inner join omTrSalesInvoice b on b.CompanyCode=a.CompanyCode
	and b.BranchCode=a.BranchCode
	and b.InvoiceNo=a.InvoiceNo
 where 1 = 1
   and a.CompanyCode = @CompanyCode 
   and a.BranchCode  = @BranchCode
   and a.InvoiceNo   = @DocNo 
 group by a.CompanyCode, a.BranchCode, a.InvoiceNo, a.TypeOfGoods

insert into @t_journal
select 8, 'PPN'
	 , isnull((
		select cls.TaxOutAccNo
		  from omTrSalesInvoice ivu, GnMstCustomerProfitCenter cus, GnMstCustomerClass cls
		 where 1 = 1
		   and cus.CompanyCode   = ivu.CompanyCode
		   and cus.BranchCode    = ivu.BranchCode
		   and cus.CustomerCode  = ivu.CustomerCode
		   and cus.ProfitCenterCode = '100'
		   and cls.CompanyCode   = ivu.CompanyCode
		   and cls.BranchCode    = ivu.BranchCode
		   and cls.CustomerClass = cus.CustomerClass
		   and cus.CompanyCode   = @CompanyCode
		   and cus.BranchCode    = @BranchCode
		   and ivu.InvoiceNo     = @DocNo
		), '') AccountNo
	 , '' AccountDesc
	 , 0
	 , isnull((
		select sum(Quantity * AfterDiscPPn)
		  from omTrSalesInvoiceModel
		 where 1 = 1
		   and CompanyCode = @CompanyCode 
		   and BranchCode  = @BranchCode
		   and InvoiceNo   = @DocNo
		), 0)
	 + isnull((
		select sum(mdl.Quantity * oth.AfterDiscPPn)
		  from omTrSalesInvoiceOthers oth left join omTrSalesInvoiceModel mdl
			on oth.BranchCode = mdl.BranchCode
			and oth.InvoiceNo = mdl.InvoiceNo
			and oth.BPKNo = mdl.BPKNo
			and oth.SalesModelCode = mdl.SalesModelCode
		 where 1 = 1
		   and oth.CompanyCode = @CompanyCode 
		   and oth.BranchCode  = @BranchCode
		   and oth.InvoiceNo   = @DocNo
		), 0)
	 + isnull((
		select sum(PPN)
		  from omTrSalesInvoiceAccs
		 where 1 = 1
		   and CompanyCode = @CompanyCode 
		   and BranchCode  = @BranchCode
		   and InvoiceNo   = @DocNo
		), 0)
	 + (select isnull(sum(isnull(Quantity,0)*isnull(PPN,0)),0) from omTrSalesInvoiceAccsSeq where companyCode = @CompanyCode 
		   and BranchCode=@BranchCode and InvoiceNo=@DocNo)
where (isnull((
		select sum(Quantity * AfterDiscPPn)
		  from omTrSalesInvoiceModel
		 where 1 = 1
		   and CompanyCode = @CompanyCode 
		   and BranchCode  = @BranchCode
		   and InvoiceNo   = @DocNo
		), 0)
	 + isnull((
		select sum(AfterDiscPPn)
		  from omTrSalesInvoiceOthers
		 where 1 = 1
		   and CompanyCode = @CompanyCode 
		   and BranchCode  = @BranchCode
		   and InvoiceNo   = @DocNo
		), 0)
	 + isnull((
		select sum(PPN)
		  from omTrSalesInvoiceAccs
		 where 1 = 1
		   and CompanyCode = @CompanyCode 
		   and BranchCode  = @BranchCode
		   and InvoiceNo   = @DocNo
		), 0)) 
	 +(select isnull(sum(isnull(quantity,0)*isnull(PPN,0)),0) from omTrSalesInvoiceAccsSeq where companyCode = @CompanyCode 
		   and BranchCode=@BranchCode and InvoiceNo=@DocNo) > 0

insert into @t_journal
select 9, 'PPN BM'
	 , isnull((
		select cls.LuxuryTaxAccNo
		  from omTrSalesInvoice ivu, GnMstCustomerProfitCenter cus, GnMstCustomerClass cls
		 where 1 = 1
		   and cus.CompanyCode   = ivu.CompanyCode
		   and cus.BranchCode    = ivu.BranchCode
		   and cus.CustomerCode  = ivu.CustomerCode
		   and cus.ProfitCenterCode = '100'
		   and cls.CompanyCode   = ivu.CompanyCode
		   and cls.BranchCode    = ivu.BranchCode
		   and cls.CustomerClass = cus.CustomerClass
		   and cus.CompanyCode   = @CompanyCode
		   and cus.BranchCode    = @BranchCode
		   and ivu.InvoiceNo     = @DocNo
		), '') AccountNo
	 , '' AccountDesc
	 , 0
	 , isnull((
		select sum(Quantity * AfterDiscPPnBm)
		  from omTrSalesInvoiceModel
		 where 1 = 1
		   and CompanyCode = @CompanyCode 
		   and BranchCode  = @BranchCode
		   and InvoiceNo   = @DocNo
		), 0)
where isnull((
		select sum(Quantity * AfterDiscPPnBm)
		  from omTrSalesInvoiceModel
		 where 1 = 1
		   and CompanyCode = @CompanyCode 
		   and BranchCode  = @BranchCode
		   and InvoiceNo   = @DocNo
		), 0) > 0

insert into @t_journal
select 10, 'HPP Unit'
	 , isnull((
		select acc.COGSAccNo
		  from omMstModelAccount acc
		 where 1 = 1
		   and acc.CompanyCode    = a.CompanyCode
		   and acc.BranchCode     = a.BranchCode
		   and acc.SalesModelCode = a.SalesModelCode
		), '') AccountNo
	 , '' AccountDesc
	 , sum(isnull (a.COGS, 0)) as COGS
	 , 0
  from OmTrSalesInvoiceVin a
 where 1 = 1
   and a.CompanyCode = @CompanyCode 
   and a.BranchCode  = @BranchCode
   and a.InvoiceNo   = @DocNo 
 group by a.CompanyCode, a.BranchCode, a.InvoiceNo, a.BPKNo, a.SalesModelCode, a.SalesModelYear
having sum(isnull (a.COGS, 0)) > 0

insert into @t_journal
select 11, 'INVENTORY UNIT'
	 , isnull((
		select acc.InventoryAccNo
		  from omMstModelAccount acc
		 where 1 = 1
		   and acc.CompanyCode    = a.CompanyCode
		   and acc.BranchCode     = a.BranchCode
		   and acc.SalesModelCode = a.SalesModelCode
		), '') AccountNo
	 , '' AccountDesc
	 , 0
	 , sum(isnull (a.COGS, 0)) as COGS
  from OmTrSalesInvoiceVin a
 where 1 = 1
   and a.CompanyCode = @CompanyCode 
   and a.BranchCode  = @BranchCode
   and a.InvoiceNo   = @DocNo 
 group by a.CompanyCode, a.BranchCode, a.InvoiceNo, a.BPKNo, a.SalesModelCode, a.SalesModelYear
having sum(isnull (a.COGS, 0)) > 0

insert into @t_journal
select distinct 12, 'COGS SPAREPART ['+a.TypeOfGoods+']'
	, (select top 1 COGSAccNo from spMstAccount where companycode=a.companycode and branchcode=a.branchcode and typeofgoods=a.typeofgoods) AccountNo
	, '' AccountDesc
	, (select sum(isnull(Quantity,0)*isnull(COGS,0)) from omtrsalesinvoiceaccsseq where companycode=a.companycode and branchcode=a.branchcode 
			and invoiceno=a.invoiceno and typeofgoods=a.typeofgoods group by typeofgoods) AmountDb
	, 0 AmountCr
from omTrSalesInvoiceAccsSeq a 
inner join omTrSalesInvoice b on b.CompanyCode=a.CompanyCode
	and b.BranchCode=a.BranchCode
	and b.InvoiceNo=a.InvoiceNo
where a.companyCode=@CompanyCode 
	and a.BranchCode=@BranchCode 
	and a.InvoiceNo=@DocNo

insert into @t_journal
select distinct 13, 'INVENTORY AKSESORIES ['+a.TypeOfGoods+']'
	, (select top 1 InventoryAccNo from spMstAccount where companycode=a.companycode and branchcode=a.branchcode and typeofgoods=a.typeofgoods) AccountNo
	, '' AccountDesc
	, 0 AmountDb
	, (select sum(isnull(Quantity,0)*isnull(COGS,0)) from omtrsalesinvoiceaccsseq where companycode=a.companycode and branchcode=a.branchcode and invoiceno=a.invoiceno
		and typeofgoods=a.typeofgoods group by typeofgoods) AmountCr
from omTrSalesInvoiceAccsSeq a 
inner join omTrSalesInvoice b on b.CompanyCode=a.CompanyCode

	and b.BranchCode=a.BranchCode
	and b.InvoiceNo=a.InvoiceNo
WHERE a.companyCode=@CompanyCode 
	and a.BranchCode=@BranchCode 
	and a.InvoiceNo=@DocNo

END
--#endregion

select sum(a.AmountDb) as AmountDb, sum(a.AmountCr) as AmountCr
  from @t_journal a

GO

if object_id('uspfn_OmGetStockDataTable') is not null
	drop procedure uspfn_OmGetStockDataTable
GO
CREATE procedure [dbo].[uspfn_OmGetStockDataTable]
	@CompanyCode varchar(20),
	@DateBegin datetime,
	@DateEnd datetime
AS
--exec uspfn_OmGetStockDataTable '6006406','20120101','20121230'
BEGIN
	SELECT '1'  AS RecordID, b.BPUDate  AS transactionDate, CASE
			  WHEN (SELECT StandardCode FROM GnMstSupplier WHERE CompanyCode = b.CompanyCode AND SupplierCode = b.SupplierCode)  = '2000000'
		   THEN
				 'B1'
			  ELSE
				 'B2'
		   END  AS transactionType, '' AS ReasonCode, a.ChassisCode  AS
		   ChassisCode, a.ChassisNo  AS ChassisNo, a.EngineCode  AS EngineCode, a.EngineNo  AS EngineNo, b.RefferenceDONo AS DONo,

		   (CASE WHEN (SELECT StandardCode FROM GnMstSupplier WHERE CompanyCode = b.CompanyCode AND SupplierCode = b.SupplierCode)  = '2000000'
			THEN '' ELSE b.SupplierCode END)  AS 'Supplier_CustomerCode', (
		   SELECT c.SupplierName
		   FROM gnMstSupplier c
		   WHERE b.SupplierCode = c.SupplierCode)  AS 'Supplier_CustomerName'
	  FROM omMstVehicle a
		LEFT JOIN omTrPurchaseBPUDetail c on a.CompanyCode = c.CompanyCode and a.ChassisCode = c.ChassisCode and a.ChassisNo = c.ChassisNo
		LEFT JOIN omTrPurchaseBPU b on b.CompanyCode = c.CompanyCode and b.BranchCode = c.BranchCode and b.BPUNo = c.BPUNo
	 WHERE a.CompanyCode = b.CompanyCode
		   AND a.BPUNo = b.BPUNo
		   AND CONVERT(VARCHAR, b.BPUDate, 112) BETWEEN @DateBegin AND @DateEnd
		   AND b.CompanyCode = @CompanyCode 

	UNION
	SELECT '1'  AS RecordID, b.BPUDate  AS transactionDate, CASE
			  WHEN (SELECT StandardCode FROM GnMstSupplier WHERE CompanyCode = b.CompanyCode AND SupplierCode = b.SupplierCode)  = '2000000'
		   THEN
				 'B1'
			  ELSE
				 'B2'
		   END  AS transactionType, '' AS ReasonCode, a.ChassisCode  AS
		   ChassisCode, a.ChassisNo  AS ChassisNo, a.EngineCode  AS EngineCode, a.EngineNo  AS EngineNo, b.RefferenceDONo AS DONo,
			(CASE WHEN (SELECT StandardCode FROM GnMstSupplier WHERE CompanyCode = b.CompanyCode AND SupplierCode = b.SupplierCode)  = '2000000'
			THEN '' ELSE b.SupplierCode END) AS 'Supplier_CustomerCode', (
		   SELECT c.SupplierName
		   FROM gnMstSupplier c
		   WHERE b.SupplierCode = c.SupplierCode)  AS 'Supplier_CustomerName'
	  FROM omMstVehicleTemp a, omTrPurchaseBPU b
	 WHERE a.CompanyCode = b.CompanyCode
		   AND a.BPUNo = b.BPUNo AND a.IsActive = 1
		   AND CONVERT(VARCHAR, b.BPUDate, 112) BETWEEN @DateBegin AND @DateEnd
		   AND b.CompanyCode = @CompanyCode 


	UNION
	SELECT '1'  AS RecordID, b.ReturnDate  AS transactionDate, CASE
			  WHEN (SELECT StandardCode FROM GnMstSupplier WHERE CompanyCode = x.CompanyCode AND SupplierCode = x.SupplierCode) = '2000000'
		   THEN
				 'R1'
			  ELSE
				 'R2'
		   END  AS transactionType, '' AS ReasonCode, a.ChassisCode  AS
		   ChassisCode, a.ChassisNo  AS ChassisNo, a.EngineCode  AS EngineCode, a.EngineNo  AS EngineNo, x.RefferenceDoNo AS DONo,
			(CASE WHEN (SELECT StandardCode FROM GnMstSupplier WHERE CompanyCode = b.CompanyCode AND SupplierCode = x.SupplierCode)  = '2000000'
			THEN '' ELSE x.SupplierCode END) AS 'Supplier_CustomerCode', (
		   SELECT c.SupplierName
		   FROM gnMstSupplier c
		   WHERE x.SupplierCode = c.SupplierCode)  AS 'Supplier_CustomerName'
	 FROM omMstVehicle a
	 INNER JOIN omTrPurchaseReturn b ON b.CompanyCode = a.CompanyCode AND b.ReturnNo = a.POReturnNo AND b.HPPNo = a.HPPNo
	 LEFT JOIN omTrPurchaseBPU x ON x.CompanyCode = a.CompanyCode AND x.BranchCode = b.BranchCode AND x.BPUNo = a.BPUNo
	 WHERE CONVERT(VARCHAR, b.ReturnDate, 112) BETWEEN @DateBegin AND @DateEnd
		   AND a.CompanyCode = @CompanyCode 
	UNION
	SELECT '1'  AS RecordID, b.ReturnDate  AS transactionDate, CASE
			  WHEN (SELECT StandardCode FROM GnMstSupplier WHERE CompanyCode = x.CompanyCode AND SupplierCode = x.SupplierCode) = '2000000'
		   THEN
				 'R1'
			  ELSE
				 'R2'
		   END  AS transactionType, '' AS ReasonCode, a.ChassisCode  AS
		   ChassisCode, a.ChassisNo  AS ChassisNo, a.EngineCode  AS EngineCode, a.EngineNo  AS EngineNo, 
		   (CASE WHEN charindex('-', a.RefDoNo) = 0 THEN a.RefDONo ELSE  SUBSTRING(a.RefDoNo, 0, charindex('-', a.RefDoNo) ) END) AS DONo,
			(CASE WHEN (SELECT StandardCode FROM GnMstSupplier WHERE CompanyCode = b.CompanyCode AND SupplierCode = x.SupplierCode)  = '2000000'
			THEN '' ELSE x.SupplierCode END) AS 'Supplier_CustomerCode', (
		   SELECT c.SupplierName
		   FROM gnMstSupplier c
		   WHERE x.SupplierCode = c.SupplierCode)  AS 'Supplier_CustomerName'
	  FROM omMstVehicleTemp a
	 INNER JOIN omTrPurchaseReturn b ON b.CompanyCode = a.CompanyCode AND b.ReturnNo = a.POReturnNo AND b.HPPNo = a.HPPNo
	 LEFT JOIN omTrPurchaseBPU x ON x.CompanyCode = a.CompanyCode AND x.BranchCode = b.BranchCode AND x.BPUNo = a.BPUNo
	 WHERE CONVERT(VARCHAR, b.ReturnDate, 112) BETWEEN @DateBegin AND @DateEnd
		   AND a.CompanyCode = @CompanyCode 

	UNION
	SELECT '1'  AS RecordID, b.SODate  AS transactionDate, 
		   CASE WHEN b.SalesType = '1' THEN 'S1' ELSE 'S2' END AS transactionType, ''  AS ReasonCode, a.ChassisCode  AS
		   ChassisCode, a.ChassisNo  AS ChassisNo, a.EngineCode  AS EngineCode, a.EngineNo  AS EngineNo, x.RefferenceDoNo AS DONo,
		   ''  AS 'Supplier_CustomerCode', (
		   SELECT c.CustomerName
		   FROM gnMstCustomer c
		   WHERE b.CustomerCode = c.CustomerCode)  AS 'Supplier_CustomerName'
	  FROM omMstVehicle a 
	  INNER JOIN omTrSalesSO b
	  ON a.CompanyCode = b.CompanyCode
	  AND a.SONo = b.SONo
	  LEFT JOIN omTrPurchaseBPUDetail y ON a.CompanyCode = y.CompanyCode and a.ChassisCOde = y.ChassisCode AND a.ChassisNo = y.ChassisNo
	  LEFT JOIN omTrPurchaseBPU x ON x.CompanyCode = y.CompanyCode AND x.BranchCode = y.BranchCode AND x.BPUNo = y.BPUNo
	  WHERE CONVERT(VARCHAR, b.SODate, 112) BETWEEN @DateBegin AND @DateEnd  
	  AND b.CompanyCode = @CompanyCode 
	UNION
	SELECT '1'  AS RecordID, b.ReqDate  AS transactionDate, 
	CASE WHEN b.StatusFaktur = '1' AND b.SubDealerCode = b.CompanyCode THEN 'F1' ELSE 
	(CASE WHEN b.StatusFaktur = '1' AND b.SubDealerCode <> b.CompanyCode THEN 'F2' ELSE 
	(CASE WHEN b.StatusFaktur <> '1' AND b.SubDealerCode = b.CompanyCode THEN 'F3' ELSE 'F4' END) END) END
	AS transactionType, (SELECT TOP 1 z.ReasonCode
		   FROM omTrSalesReqDetail z
		   WHERE z.ChassisNo = a.ChassisNo
		   AND z.ChassisCode = a.ChassisCode)  AS ReasonCode, a.ChassisCode  AS
		   ChassisCode, a.ChassisNo  AS ChassisNo, a.EngineCode  AS EngineCode, a.EngineNo  AS EngineNo, x.RefferenceDoNo AS DONo,
			(CASE WHEN b.SubDealerCode  = a.CompanyCode THEN '' ELSE b.SubDealerCode END) AS 'Supplier_CustomerCode'
		   , (
		   SELECT c.CustomerName
		   FROM gnMstCustomer c
		   WHERE b.SubDealerCode = c.CustomerCode)  AS 'Supplier_CustomerName'
	  FROM omMstVehicle a
	  INNER JOIN omTrSalesReqDetail z ON a.CompanyCode = z.CompanyCode AND a.ChassisCode = z.ChassisCode AND a.ChassisNo = z.ChassisNo
	  INNER JOIN omTrSalesReq b	ON b.CompanyCode = z.CompanyCode AND b.BranchCode = z.BranchCode AND b.ReqNo = z.ReqNo
	  LEFT JOIN omTrPurchaseBPUDetail y
		ON y.CompanyCode = a.CompanyCode AND y.ChassisCode = a.ChassisCode AND y.ChassisNo = a.ChassisNo
	  LEFT JOIN omTrPurchaseBPU x 
		ON x.CompanyCode = y.CompanyCode AND x.BranchCode = y.BranchCode AND x.BPUNo = y.BPUNo
	  WHERE CONVERT(VARCHAR, b.ReqDate, 112) BETWEEN @DateBegin AND @DateEnd
	  AND b.CompanyCode = @CompanyCode 
	UNION
	SELECT '1'  AS RecordID, b.ReturnDate  AS transactionDate, 'U1'  AS
		   transactionType, ''  AS ReasonCode, a.ChassisCode  AS
		   ChassisCode, a.ChassisNo  AS ChassisNo, a.EngineCode  AS EngineCode, a.EngineNo  AS EngineNo, x.RefferenceDoNo AS DONo,
		   ''  AS 'Supplier_CustomerCode', (
		   SELECT c.CustomerName
		   FROM gnMstCustomer c
		   WHERE b.CustomerCode = c.CustomerCode)  AS 'Supplier_CustomerName'
	  FROM omMstVehicle a
	  INNER JOIN omTrSalesReturn b
	  ON a.CompanyCode = b.CompanyCode
	  AND a.SOReturnNo = b.ReturnNo
	LEFT JOIN omTrPurchaseBPUDetail y ON y.CompanyCode = a.CompanyCode AND y.ChassisCode = a.ChassisCode AND y.ChassisNo = a.ChassisNo
	LEFT JOIN omTrPurchaseBPU x ON x.CompanyCode = y.CompanyCode AND x.BranchCode = y.BranchCode AND x.BPUNo = y.BPUNo
	  WHERE CONVERT(VARCHAR, b.ReturnDate, 112) BETWEEN @DateBegin AND @DateEnd
	  AND b.CompanyCode = @CompanyCode;
END
GO

if object_id('uspfn_OmRpSalesTrn001B') is not null
	drop procedure uspfn_OmRpSalesTrn001B
GO
CREATE procedure uspfn_OmRpSalesTrn001B 

(
	@CompanyCode VARCHAR(15),
	@BranchCode	VARCHAR(15),
	@SONoFrom VARCHAR(15),
	@SONoEnd VARCHAR(15),
	@param bit
)

AS

BEGIN
--	declare	@CompanyCode varchar(15)
--	declare @BranchCode varchar(15)
--	declare @SONoFrom varchar(15)
--	declare @SONoEnd varchar(15)
--	declare @param bit
--
--	set @CompanyCode ='6114201'
--	set @BranchCode ='611420100'
--	set @SONoFrom ='SOA/12/000537'
--	set @SONoEnd ='SOA/12/000537'
--	set @param ='1'
--	
	declare @tabData as table 
	(
		CompanyCode		varchar(max),
		BranchCode		varchar(max),
		SoNo			varchar(max),
		Prefix			char(1),
		Model			varchar(max),
		Remark			varchar(max),
		Satuan			decimal(18,0),		
		Qty				decimal(6,0),
		Total			decimal(18,0),		
		BBN				decimal(18,0),		
		Accs			decimal(18,0),		
		Diskon			decimal(18,0),		
		Lain			decimal(18,0),		
		Jumlah			decimal(18,0),
		ChassisNo		int,
		EngineNo		int
	)

	-- INITIAL TABLE FOR UNIT --
	----------------------------
	SELECT * INTO #tUnit FROM (
		SELECT  
			a.CompanyCode
			, a.BranchCode
			, a.SONo
			, b.SalesModelCode Model
			, b.SalesModelYear ModelYear
			, c.ColourCode
			, b.BeforeDiscTotal Satuan
			, case when isnull(d.ChassisNo,0) = 0 then c.Quantity else 1 end Qty
			, SUM(ISNULL(d.bbn, 0))+ SUM(ISNULL(d.kir, 0))as BBN
			, (b.OthersDPP+b.OthersPPn) * (case when isnull(d.ChassisNo,0) = 0 then c.Quantity else 1 end) as Accesories
			, b.DiscIncludePPN * (case when isnull(d.ChassisNo,0) = 0 then c.Quantity else 1 end) as Potongan
			, (b.ShipAmt+b.DepositAmt+b.OthersAmt) * (case when isnull(d.ChassisNo,0) = 0 then c.Quantity else 1 end) as Lainlain 
			, isnull(d.ChassisNo,0) ChassisNo
			, isnull(d.EngineNo,0) EngineNo
		FROM OmTrSalesSO a
		INNER JOIN OmTrSalesSOModel b on a.companyCode = b.companyCode 
			AND a.branchCode = b.branchCode 
			AND a.SONo = b.SONo
		INNER JOIN OmTrSalesSOModelColour c on c.companyCode = b.companyCode 
			AND c.branchCode = b.branchCode 
			AND c.SONo = b.SONo
			AND c.salesModelCode = b.salesModelCode 
			AND c.salesModelYear = b.salesModelYear
		LEFT JOIN OmTrSalesSOVin d on d.companyCode = c.companyCode 
			AND d.branchCode = c.branchCode
			AND d.SONo = c.SONo 
			AND d.salesModelCode = c.salesModelCode 
			AND d.salesModelYear = c.salesModelYear 
			AND c.colourCode = d.colourCode
		WHERE a.companyCode = @CompanyCode 
			AND a.branchCode = @BranchCode 
			AND a.SONo BETWEEN @SONoFrom AND @SONoEnd
		GROUP BY a.CompanyCode
			, a.BranchCode
			, a.SONo
			, b.SalesModelCode
			, b.SalesModelYear
			, c.ColourCode
			, b.BeforeDiscTotal
			, case when isnull(d.ChassisNo,0) = 0 then c.Quantity else 1 end
			, b.BeforeDiscTotal * (case when isnull(d.ChassisNo,0) = 0 then c.Quantity else 1 end)
			, (b.OthersDPP+b.OthersPPn) * (case when isnull(d.ChassisNo,0) = 0 then c.Quantity else 1 end)
			, b.DiscIncludePPN * (case when isnull(d.ChassisNo,0) = 0 then c.Quantity else 1 end)
			, (b.ShipAmt+b.DepositAmt+b.OthersAmt) * (case when isnull(d.ChassisNo,0) = 0 then c.Quantity else 1 end)	
			, d.ChassisNo
			, d.EngineNo

		-- SISA UNIT YANG BELUM ADA VIN

		UNION 

		SELECT 
			a.CompanyCode
			, a.BranchCode
			, a.SONo
			, b.SalesModelCode Model
			, b.SalesModelYear ModelYear
			, c.ColourCode
			, b.BeforeDiscTotal Satuan
			, c.quantity - (select count(*) from OmTrSalesSOVin where companycode=a.companycode and branchcode=a.branchcode and sono=a.sono 
				and salesmodelcode=b.salesmodelcode and salesmodelyear=b.salesmodelyear and colourcode=c.colourcode)as Qty
			, 0 BBN
			, (b.OthersDPP+b.OthersPPn) * (c.quantity - (select count(*) from OmTrSalesSOVin where companycode=a.companycode 
				and branchcode=a.branchcode and sono=a.sono and salesmodelcode=b.salesmodelcode and salesmodelyear=b.salesmodelyear 
				and colourcode=c.colourcode)) as Accessories
			, b.DiscIncludePPN * (c.quantity - (select count(*) from OmTrSalesSOVin where companycode=a.companycode and branchcode=a.branchcode 
				and sono=a.sono and salesmodelcode=b.salesmodelcode and salesmodelyear=b.salesmodelyear and colourcode=c.colourcode)) as Potongan
			, (b.ShipAmt+b.DepositAmt+b.OthersAmt) * (c.quantity - (select count(*) from OmTrSalesSOVin where companycode=a.companycode 
				and branchcode=a.branchcode and sono=a.sono and salesmodelcode=b.salesmodelcode and salesmodelyear=b.salesmodelyear 
				and colourcode=c.colourcode)) as Lainlain
			, 0 ChassisNo
			, 0 EngineNo
		FROM OmTrSalesSO a
		INNER JOIN OmTrSalesSOModel b on a.companyCode = b.companyCode 
			AND a.branchCode = b.branchCode 
			AND a.SONo = b.SONo
		INNER JOIN OmTrSalesSOModelColour c on a.companyCode = c.companyCode 
			AND a.branchCode = c.branchCode 
			AND a.SONo = c.SONo
			AND c.SalesModelCode=b.SalesModelCode
			AND c.SalesModelYear=b.SalesModelYear
		WHERE a.CompanyCode=@COmpanyCode
			AND a.BranchCode=@BranchCode
			AND a.SONo BETWEEN @SONoFrom AND @SONoEnd
			AND c.quantity-(select count(*) from OmTrSalesSOVin where companycode=a.companycode and branchcode=a.branchcode and sono=a.sono 
					and salesmodelcode=b.salesmodelcode and salesmodelyear=b.salesmodelyear and colourcode=c.colourcode)>0
	) #tUnit


	set @param = 1;


	IF (@param=1)
	BEGIN
		-- DETAIL PART DISEMBUNYIKAN --
		-------------------------------

		SELECT * INTO #t1 FROM (
		SELECT 
			a.CompanyCode
			, a.BranchCode
			, a.SONo
			, sum((floor((isnull(a.retailprice,0) * (100+isnull(d.TaxPct,0)))/100)) * isnull(a.qty,0)) Total
			, sum((floor((isnull(a.retailprice,0) * (100+isnull(d.TaxPct,0)))/100) - isnull(a.Afterdisctotal,0)) * isnull(a.qty,0)) Potongan
			, sum((floor((isnull(a.retailprice,0) * (100+isnull(d.TaxPct,0)))/100) - 
				(floor((isnull(a.retailprice,0) * (100+isnull(d.TaxPct,0)))/100) - isnull(a.Afterdisctotal,0))) * isnull(a.qty,0)) Jumlah
		FROM omTrSalesSOAccsSeq a
		INNER JOIN omTrSalesSO b ON b.CompanyCode=a.CompanyCode
			AND b.BranchCode=a.BranchCode
			AND b.SONo=a.SONo
		LEFT JOIN gnMstCustomerProfitCenter c ON c.CompanyCode=a.CompanyCode
			AND c.BranchCode=a.BranchCode
			AND c.CustomerCode=b.CustomerCode
			AND c.ProfitCenterCode='100'
		LEFT JOIN gnMstTax d ON d.CompanyCode=a.CompanyCode
			AND d.TaxCode=c.TaxCode
		WHERE a.CompanyCode=@CompanyCode
			AND a.BranchCode=@BranchCode
			AND a.SONo BETWEEN @SONoFrom AND @SONoEnd 
			AND a.partSeq=1
		GROUP BY a.CompanyCode, a.BranchCode, a.SONo
		) #t1

		INSERT INTO @tabData
		SELECT  
			a.CompanyCode
			, a.BranchCode
			, a.SONo
			, 'A'
			, b.Model
			, b.ColourCode+' - '+isnull(e.RefferenceDesc1,'') Remark
			, b.Satuan+isnull(f.Total,0) AS Satuan
			, b.Qty
			, (b.Satuan+isnull(f.Total,0)) * b.Qty AS Total
			, b.BBN
			, b.Accesories
			, b.Potongan+(isnull(f.Potongan,0)*b.Qty)
			, b.Lainlain 
			, (b.Satuan*b.Qty) + b.BBN + b.Accesories - b.Potongan + b.LainLain + (isnull(f.Jumlah,0)*b.Qty) AS Jumlah
			, b.ChassisNo
			, b.EngineNo
		FROM OmTrSalesSO a
		INNER JOIN #tUnit b on b.companyCode = a.companyCode 
			AND b.branchCode = a.branchCode 
			AND b.SONo = a.SONo
		LEFT JOIN omMstRefference e ON e.CompanyCode = b.CompanyCode
			AND e.RefferenceType = 'COLO'
			AND e.RefferenceCode = b.ColourCode
		LEFT JOIN #t1 f ON f.CompanyCode=a.CompanyCode
			AND f.BranchCode=a.BranchCode
			AND f.SONo=a.SONo
		WHERE a.companyCode = @CompanyCode 
			AND a.branchCode = @BranchCode 
			AND a.SONo BETWEEN @SONoFrom AND @SONoEnd	
		UNION ALL

		SELECT DISTINCT
			a.CompanyCode
			, a.BranchCode
			, a.SONo
			, 'B'
			, 'SPAREPART/ACCS :'
			, ''
			, 0
			, 0
			, 0
			, 0
			, 0
			, 0
			, 0
			, 0
			, 0
			, 0
		FROM OmTrSalesSO a
		INNER JOIN omTrSalesSOAccsSeq b ON b.CompanyCode=a.CompanyCode
			AND b.BranchCode=a.BranchCode
			AND b.SONo=a.SONo
			AND b.PartSeq=1
		WHERE a.companyCode = @CompanyCode 
			AND a.branchCode = @BranchCode 
			AND a.SONo BETWEEN @SONoFrom AND @SONoEnd

		UNION ALL

		SELECT 
			  a.CompanyCode
			, a.BranchCode
			, a.SONo
			, 'C'
			, a.PartNo Model
			, isnull(c.PartName,'') Remark
			, 0 Satuan
			, a.demandqty Qty
			, 0 Total
			, 0 BBN
			, 0 Accesories
			, 0 Potongan
			, 0 LainLain
			, 0 Jumlah
			, 0 Rangka
			, 0 Mesin
		FROM omTrSalesSOAccsSeq a
		INNER JOIN omTrSalesSO b ON b.CompanyCode=a.CompanyCode
			AND b.BranchCode=a.BranchCode
			AND b.SONo=a.SONo
		LEFT JOIN SpMStItemInfo c ON c.CompanyCode=a.CompanyCode
			AND c.PartNo=a.PartNo
		LEFT JOIN gnMstCustomerProfitCenter d ON d.CompanyCode=a.CompanyCode
			AND d.BranchCode=a.BranchCode
			AND d.CustomerCode=b.CustomerCode
			AND ProfitCenterCode='100'
		LEFT JOIN gnMstTax e ON e.CompanyCode=a.CompanyCode
			AND e.TaxCode=d.TaxCode
		WHERE a.CompanyCode=@CompanyCode
			AND a.BranchCode=@BranchCode
			AND a.SONo BETWEEN @SONoFrom AND @SONoEnd 
			AND a.partSeq=1	

		DROP TABLE #t1

	END

	ELSE

	BEGIN
		-- TAMPILKAN PART DETAIL --
		---------------------------		

		SELECT * INTO #tPart FROM (
		SELECT 
			a.CompanyCode
			, a.BranchCode
			, a.SONo
			, a.PartNo Model
			, isnull(c.PartName,'') Remark
			, floor((a.retailprice*(100+isnull(e.TaxPct,0)))/100) Satuan
			, a.demandqty Qty
			, (floor((a.retailprice*(100+isnull(e.TaxPct,0)))/100))*a.demandqty Total
			, 0 BBN
			, 0 Accesories
			, (floor((a.retailprice*(100+isnull(e.TaxPct,0)))/100)-a.Afterdisctotal)*a.demandqty Potongan
			, 0 LainLain
			, (floor((a.retailprice*(100+isnull(e.TaxPct,0)))/100)-(floor((a.retailprice*(100+isnull(e.TaxPct,0)))/100)-a.Afterdisctotal))*a.demandqty Jumlah
		FROM omTrSalesSOAccsSeq a
		INNER JOIN omTrSalesSO b ON b.CompanyCode=a.CompanyCode
			AND b.BranchCode=a.BranchCode
			AND b.SONo=a.SONo
		LEFT JOIN SpMStItemInfo c ON c.CompanyCode=a.CompanyCode
			AND c.PartNo=a.PartNo
		LEFT JOIN gnMstCustomerProfitCenter d ON d.CompanyCode=a.CompanyCode
			AND d.BranchCode=a.BranchCode
			AND d.CustomerCode=b.CustomerCode
			AND ProfitCenterCode='100'
		LEFT JOIN gnMstTax e ON e.CompanyCode=a.CompanyCode
			AND e.TaxCode=d.TaxCode
		WHERE a.CompanyCode=@CompanyCode
			AND a.BranchCode=@BranchCode
			AND a.SONo BETWEEN @SONoFrom AND @SONoEnd 
			AND a.partSeq=1
		) #tPart

		

		INSERT INTO @tabData
		SELECT  
			a.CompanyCode
			, a.BranchCode
			, a.SONo
			, 'A'
			, b.Model
			, b.ColourCode+' - '+isnull(e.RefferenceDesc1,'') Remark
			, b.Satuan
			, b.Qty
			, b.Satuan*b.Qty AS Total
			, b.BBN
			, b.Accesories
			, b.Potongan
			, b.Lainlain 
			, (b.Satuan*b.Qty) + b.BBN + b.Accesories - b.Potongan + b.LainLain Jumlah
			, b.ChassisNo
			, b.EngineNo
		FROM OmTrSalesSO a
		INNER JOIN #tUnit b on b.companyCode = a.companyCode 
			AND b.branchCode = a.branchCode 
			AND b.SONo = a.SONo
		LEFT JOIN omMstRefference e ON e.CompanyCode = b.CompanyCode
			AND e.RefferenceType = 'COLO'
			AND e.RefferenceCode = b.ColourCode
		WHERE a.companyCode = @CompanyCode 
			AND a.branchCode = @BranchCode 
			AND a.SONo BETWEEN @SONoFrom AND @SONoEnd

		UNION ALL

		SELECT DISTINCT
			a.CompanyCode
			, a.BranchCode
			, a.SONo
			, 'B'
			, 'SPAREPART/ACCS :'
			, ''
			, 0
			, 0
			, 0
			, 0
			, 0
			, 0
			, 0
			, 0
			, 0
			, 0
		FROM OmTrSalesSO a
		INNER JOIN omTrSalesSOAccsSeq b ON b.CompanyCode=a.CompanyCode
			AND b.BranchCode=a.BranchCode
			AND b.SONo=a.SONo
			AND b.PartSeq=1
		WHERE a.companyCode = @CompanyCode 
			AND a.branchCode = @BranchCode 
			AND a.SONo BETWEEN @SONoFrom AND @SONoEnd
		UNION ALL
		SELECT
			CompanyCode
			, BranchCode
			, SONo
			, 'C'
			, Model
			, Remark
			, Satuan
			, Qty
			, Total
			, BBN
			, Accesories
			, Potongan
			, LainLain
			, Jumlah
			, 0
			, 0
		FROM #tPart
		DROP TABLE #tPart
	END



--select * from @tabData
	-- DATA RESULT --
	-----------------
   SELECT  
		  a.SONo + case a.SalesType when '0' then '-W' when '1' then '-D' end  AS SONo
		, a.SKPKNo
		, a.RefferenceNo
		, a.SODate
		, b.Model
		, b.Model SalesModelCode
		, b.Remark
		, b.Remark ColourCode
		, b.Satuan 
		, b.Satuan BeforeDiscTotal
		, b.Qty
		, b.Qty Quantity
		, b.Total
		, b.BBN
		, b.Accs
		, b.Accs Accesories
		, b.Diskon Diskon
		, b.Diskon Potongan
		, b.Lain
		, b.Lain LainLain
		, b.Jumlah
		, b.Jumlah SubTotal
		, d.EmployeeName +' ['+a.Salesman+']' as Sales
		, CASE a.SalesType WHEN '0' THEN 'WHOLESALE' WHEN '1' THEN 'DIRECT' END AS TipeSales
		, a.RefferenceNo
		, c.CustomerName +' ['+a.CustomerCode+']' as Pelanggan
		, a.RequestDate
		, dateadd(day, convert(int, e.ParaValue), a.SODate) as JatuhTempo
		, e.LookUpValueName as TOPCode
		, a.shipto
		, g.CustomerName as ShipName
		, a.PrePaymentAmt
		, f.CustomerName as Leasing
		, a.Remark as Ket
		, a.SKPKNo
		, upper(sign1.SignName) AS SignName
		, upper(sign1.TitleSign) AS TitleSign
		, b.ChassisNo
		, b.EngineNo
		, b.Prefix
		, @param HidePart
	FROM OmTrSalesSO a
	INNER JOIN @tabData b on b.companyCode = a.companyCode 
		AND b.branchCode = a.branchCode 
		AND b.SONo = a.SONo
	LEFT JOIN gnMstCustomer c ON c.CompanyCode = a.CompanyCode
		AND c.CustomerCode = a.CustomerCode
	LEFT JOIN HrEmployee d ON d.companyCode = a.companyCode
		--AND d.branchCode = a.branchCode
		AND d.EmployeeID= a.salesman
	LEFT JOIN gnMstLookUpDtl e ON e.CompanyCode = a.CompanyCode
		AND e.CodeID = 'TOPC'
		AND e.LookUpValue = a.TOPCode
	LEFT JOIN gnMstCustomer f ON f.CompanyCode = a.CompanyCode
		AND f.CustomerCode = a.LeasingCo
	LEFT JOIN gnMstCustomer g ON g.CompanyCode = a.CompanyCode
		AND g.CustomerCode = a.shipTo
	LEFT JOIN gnMstSignature AS sign1 ON sign1.CompanyCode = a.CompanyCode
		AND sign1.BranchCode = a.BranchCode
		AND sign1.ProfitCenterCode = '100'
		AND sign1.DocumentType = 'SON'
		AND sign1.SeqNo = '1'
	WHERE a.companyCode = @CompanyCode 
		AND a.branchCode = @BranchCode 
		AND a.SONo BETWEEN @SONoFrom AND @SONoEnd
	ORDER BY a.SONo, b.Prefix, b.Model

	

	DROP TABLE #tUnit
END




GO

if object_id('uspfn_omSlsBPKBrowse') is not null
	drop procedure uspfn_omSlsBPKBrowse
GO

CREATE procedure [dbo].[uspfn_omSlsBPKBrowse]   
(  
 @CompanyCode varchar(15),  
 @BranchCode varchar(15) 
)  
AS  
BEGIN  
--exec uspfn_omSlsBPKBrowse 6006410,600641001
 SELECT Distinct a.BPKNo, a.BPKDate, a.DONo, a.SONo, d.SKPKNo, d.RefferenceNo, a.CustomerCode  , c.CustomerName , a.ShipTo , e.CustomerName as ShipToDsc,
            c.Address1 + ' ' + c.Address2 + ' ' + c.Address3 + ' ' + c.Address4 as Address, a.WareHouseCode, f.LookUpValueName as WrhDsc,a.Expedition,g.CustomerName as ExpeditionDsc,a.Status,
            CASE a.Status WHEN '0' THEN 'Open' WHEN '1' THEN 'Printed' WHEN '2' THEN 'Approved' WHEN '3' THEN 'Canceled' WHEN '9' THEN 'Finished' END as StatusDsc       
            ,b.SalesType, CASE ISNULL(b.SalesType, 0) WHEN 0 THEN 'Wholesales' ELSE 'Direct' END AS TypeSales,a.Remark
            FROM omTrSalesBPK a
            LEFT JOIN gnMstCustomerProfitCenter b ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.CustomerCode = b.CustomerCode AND b.ProfitCenterCode = '100'
            LEFT JOIN gnMstCustomer c ON a.CompanyCode = c.CompanyCode AND a.CustomerCode = c.CustomerCode
            LEFT JOIN omTrSalesSO d ON a.CompanyCode = d.CompanyCode AND a.BranchCode = d.BranchCode AND a.SONo = d.SONo
            LEFT JOIN gnMstCustomer e ON a.CompanyCode = e.CompanyCode AND a.shipto = e.CustomerCode
            Left join gnMstLookUpDtl f on a.CompanyCode=f.CompanyCode and a.BranchCode=f.ParaValue and a.WarehouseCode=f.LookUpValue and f.CodeID='MPWH'
            LEFT JOIN gnMstCustomer g ON a.CompanyCode = g.CompanyCode AND a.Expedition = g.CustomerCode
            WHERE a.CompanyCode = @CompanyCode
               AND a.BranchCode = @BranchCode                              
            ORDER BY a.BPKNo DESC
End

GO

if object_id('uspfn_omSlsBPKBrwDtl') is not null
	drop procedure uspfn_omSlsBPKBrwDtl
GO

CREATE procedure [dbo].[uspfn_omSlsBPKBrwDtl]   
(  
 @CompanyCode varchar(15),  
 @BranchCode varchar(15),
 @RefType varchar(15),
 @BPKNo varchar(15)
)  
AS  
BEGIN  
--exec uspfn_omSlsBPKBrwDtl 6006410,600641001,'CLCD','BSJ/14/000001'
SELECT a.SalesModelCode, a.SalesModelYear, b.SalesModelDesc, 
	           a.ChassisCode, Convert(varchar,a.ChassisNo) as ChassisNo, 
	           a.EngineCode, Convert(varchar,a.EngineNo) as EngineNo, 
	           a.ColourCode, case when c.RefferenceDesc1 is null then '' else c.RefferenceDesc1 end RefferenceDesc1, 
	           a.Remark, a.StatusPDI,
               a.BPKSeq
        FROM   omTrSalesBPKDetail a	   
	           LEFT JOIN
	           omMstModelYear b
	           ON a.CompanyCode = b.CompanyCode
	           AND a.SalesModelCode = b.SalesModelCode      
	           AND a.ChassisCode = b.ChassisCode
               AND a.SalesModelYear = b.SalesModelYear	   
	           LEFT JOIN
	           omMstRefference c
	           ON a.CompanyCode = c.CompanyCode
	           AND a.ColourCode = c.RefferenceCode
	           AND c.RefferenceType = @RefType
       WHERE a.CompanyCode= @CompanyCode 
               AND a.BranchCode= @BranchCode 
               AND a.BPKNo= @BPKNo
       ORDER BY a.ChassisNo ASC
            
end

GO

if object_id('uspfn_omSlsBPKLkpChassisNo') is not null
	drop procedure uspfn_omSlsBPKLkpChassisNo
GO

CREATE procedure [dbo].[uspfn_omSlsBPKLkpChassisNo]   
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
AND a.ChassisNo not in (select ChassisNo from omTrSalesBPKDetail z
where z.CompanyCode = a.CompanyCode
and z.BranchCode = a.BranchCode
--and z.BPKNo = @BPKNo
and z.ChassisCode = a.ChassisCode
AND not exists (select 1 from omTrSalesReturnVIN where CompanyCode=a.CompanyCode and BranchCode=a.BranchCode
		and ChassisCode=a.ChassisCode and ChassisNo=a.ChassisNo))
END		
GO

if object_id('uspfn_omSlsBPKLkpDO') is not null
	drop procedure uspfn_omSlsBPKLkpDO
GO


CREATE procedure [dbo].[uspfn_omSlsBPKLkpDO]   
(  
 @CompanyCode varchar(15),  
 @BranchCode varchar(15),
 @ProfitCenterCode varchar(15)
 )  
AS  
BEGIN  
-- exec uspfn_omSlsBPKLkpDO  6006410,600641001,'100'
 SELECT Distinct a.DONo, a.DODate, a.SONo, g.SKPKNo, g.RefferenceNo, a.CustomerCode ,c.CustomerName, 
            c.Address1 + ' ' + c.Address2 + ' ' + c.Address3 + ' ' + c.Address4 as Address,
            a.ShipTo, c1.CustomerName as ShipName, 
            a.WareHouseCode,f.RefferenceDesc1 as WareHouseName, a.Expedition, e.SupplierName as ExpeditionName,
            b.SalesType,(CASE ISNULL(b.SalesType, 0) WHEN 0 THEN 'WholeSales' ELSE 'Direct' END) AS SalesTypeDsc
            FROM omTrSalesDO a
            LEFT JOIN gnMstCustomerProfitCenter b ON b.CompanyCode = a.CompanyCode AND b.BranchCode = a.BranchCode AND b.CustomerCode = a.CustomerCode
			LEFT JOIN gnMstCustomer c ON c.CompanyCode = a.CompanyCode AND c.CustomerCode = a.CustomerCode
            LEFT JOIN gnMstCustomer c1 ON c1.CompanyCode = a.CompanyCode AND c1.CustomerCode = a.ShipTo
			LEFT JOIN  omTrSalesDODetail d ON d.CompanyCode = a.CompanyCode AND d.BranchCode = a.BranchCode AND d.DoNo = a.DoNo
			LEFT JOIN gnMstSupplier e ON e.CompanyCode = a.CompanyCode AND e.SupplierCode = a.Expedition 
			LEFT JOIN  omMstRefference f ON f.CompanyCode = a.CompanyCode AND f.RefferenceCode = a.WareHouseCode and f.RefferenceType = 'WARE'
            LEFT JOIN omTrSalesSO g ON a.CompanyCode = g.CompanyCode AND a.BranchCode = g.BranchCode AND a.SONo = g.SONo            
            WHERE a.Status = '2'
            and d.StatusBPK <> '1'  
            AND a.CompanyCode = @CompanyCode
            AND b.BranchCode = @BranchCode
			AND b.ProfitCenterCode = @ProfitCenterCode                   
            ORDER BY a.DONo ASC
END      



GO

if object_id('uspfn_omSlsBPKLkpMdlCode') is not null
	drop procedure uspfn_omSlsBPKLkpMdlCode
GO


create procedure [dbo].[uspfn_omSlsBPKLkpMdlCode]   
(  
 @CompanyCode varchar(15),  
 @BranchCode varchar(15),  
 @DONo varchar(15) 
)  
AS  
BEGIN  
--exec uspfn_omSlsDoLkpMdlCode 6006410,600641001,''
SELECT  
	a.CompanyCode
	,a.SalesModelCode
	,a.SalesModelDesc
	,a.FakturPolisiDesc
	,a.EngineCode
	,a.PpnBmCodeBuy
	,a.PpnBmPctBuy
	,a.PpnBmCodeSell
	,a.PpnBmPctSell
	,a.PpnBmCodePrincipal
	,a.PpnBmPctPrincipal
	,a.Remark
	,a.BasicModel
	,a.TechnicalModelCode
	,a.ProductType
	,a.TransmissionType
	,a.IsChassis
	,a.IsCbu
	,a.SMCModelCode
	,a.GroupCode
	,a.TypeCode
	,a.CylinderCapacity
	,a.fuel
	,a.ModelPrincipal
	,a.Specification
	,a.ModelLine
	,a.Status
	,a.CreatedBy
	,a.CreatedDate
	,a.LastUpdateBy
	,a.LastUpdateDate
	,a.IsLocked
	,a.LockedBy
	,a.LockedDate
	,a.MarketModelCode
	,a.GroupMarketModel
	,a.ColumnMarketModel
FROM omMstModel a
INNER JOIN OmTrSalesDODetail b
ON b.CompanyCode = a.CompanyCode
AND b.SalesModelCode = a.SalesModelCode
WHERE a.CompanyCode = @CompanyCode
AND b.BranchCode = @BranchCode
AND b.DONo = @DONo
AND b.StatusBPK = '0'
ORDER BY a.SalesModelCode ASC
end    
GO

if object_id('uspfn_omSlsBPKLkpMdlYear') is not null
	drop procedure uspfn_omSlsBPKLkpMdlYear
GO

CREATE procedure [dbo].[uspfn_omSlsBPKLkpMdlYear]   
(  
 @CompanyCode varchar(15),  
 @BranchCode varchar(15),  
 @DONo varchar(15),
 @SalesModelCode varchar(20)
)  
AS  
begin
	SELECT a.*
FROM omMstModelYear a
INNER JOIN OmTrSalesDODetail b
ON b.CompanyCode = a.CompanyCode
AND b.SalesModelCode = a.SalesModelCode
AND b.salesModelYear = a.SalesModelYear
WHERE a.CompanyCode = @CompanyCode
AND b.BranchCode = @BranchCode
AND a.Status = '1'
AND b.StatusBPK = '0'
AND b.DONo = @DONo
AND b.SalesModelCode = @SalesModelCode					 
end			


GO

if object_id('uspfn_omSlsDoBrowse') is not null
	drop procedure uspfn_omSlsDoBrowse
GO

CREATE procedure [dbo].[uspfn_omSlsDoBrowse]   
(  
 @CompanyCode varchar(15),  
 @BranchCode varchar(15) 
)  
AS  
BEGIN  
--exec uspfn_omSlsDoBrowse 6006410,600641001
 SELECT Distinct a.DONo, a.DODate, d.SKPKNo, d.RefferenceNo, a.CustomerCode, c.CustomerName , a.ShipTo ,c.CustomerName as ShipToDsc,
            c.Address1 + ' ' + c.Address2 + ' ' + c.Address3 + ' ' + c.Address4 as Address,
            a.WareHouseCode, a.Expedition, a.SONo, f.CustomerName as ExpeditionDsc,a.Remark,
            CASE a.Status WHEN '0' THEN 'OPEN' WHEN '1' THEN 'PRINT' WHEN '2' THEN 'APPROVED' WHEN '3' THEN 'CANCEL' WHEN '9' THEN 'FINISH' END as StatusDsc,a.Status
            , CASE ISNULL(b.SalesType, 0) WHEN 0 THEN 'Wholesales' ELSE 'Direct' END AS TypeSales,e.LookUpValueName as WrhDsc
            FROM omTrSalesDO a
            LEFT JOIN gnMstCustomerProfitCenter b ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.CustomerCode = b.CustomerCode AND b.ProfitCenterCode = '100'
            LEFT JOIN gnMstCustomer c ON a.CompanyCode = c.CompanyCode AND a.CustomerCode = c.CustomerCode
            LEFT JOIN omTrSalesSO d ON a.CompanyCode = d.CompanyCode AND a.BranchCode = d.BranchCode AND a.SONo = d.SONo      
            Left join gnMstLookUpDtl e on a.CompanyCode=e.CompanyCode and a.BranchCode=e.ParaValue and a.WarehouseCode=e.LookUpValue and e.CodeID='MPWH'
            LEFT JOIN gnMstCustomer f ON a.CompanyCode = c.CompanyCode AND a.Expedition = c.CustomerCode
            WHERE a.CompanyCode = @CompanyCode
               AND a.BranchCode = @BranchCode                              
            ORDER BY a.DONo DESC
end         
GO

if object_id('uspfn_omSlsDoDtl') is not null
	drop procedure uspfn_omSlsDoDtl
GO

create procedure [dbo].[uspfn_omSlsDoDtl]   
(  
 @CompanyCode varchar(15),  
 @BranchCode varchar(15),
 @DONo varchar(15)
)  
AS  
BEGIN  
-- exec uspfn_omSlsDoDtl 6006410,600641001,'DOS/14/000001'
 SELECT a.SalesModelCode, a.SalesModelYear, b.SalesModelDesc, 
	           a.ChassisCode, Convert(varchar,a.ChassisNo) as ChassisNo, 
	           a.EngineCode, Convert(varchar,a.EngineNo) as EngineNo, 
	           a.ColourCode, c.RefferenceDesc1, a.Remark, a.DOSeq
        FROM   omTrSalesDODetail a	   
	           LEFT JOIN
	           omMstModelYear b
	           ON a.CompanyCode = b.CompanyCode
	           AND a.SalesModelCode = b.SalesModelCode
               AND a.SalesModelYear = b.SalesModelYear      
	           AND a.ChassisCode = b.ChassisCode	   
	           LEFT JOIN
	           omMstRefference c
	           ON a.CompanyCode = c.CompanyCode
	           AND a.ColourCode = c.RefferenceCode
	           AND c.RefferenceType = 'COLO'
       WHERE a.CompanyCode= @CompanyCode 
               AND a.BranchCode= @BranchCode 
               AND a.DONo= @DONo
       ORDER BY a.ChassisNo ASC;
END
GO

if object_id('uspfn_omSlsDoLkpExpdtn') is not null
	drop procedure uspfn_omSlsDoLkpExpdtn
GO
CREATE procedure [dbo].[uspfn_omSlsDoLkpExpdtn]   
(  
 @CompanyCode varchar(15),  
 @BranchCode varchar(15),  
 @Status varchar(1),
 @ProfitCenterCode varchar(3) 
)  
AS  
BEGIN  
--exec uspfn_omSlsDoLkpExpdtn 6006410,600641001,1,100
SELECT 
                    a.SupplierCode, a.Suppliername, (a.address1+' '+a.address2+' '+a.address3+' '+a.address4) as Alamat,
                    b.DiscPct as Diskon, (Case a.Status when 0 then 'Tidak Aktif' else 'Aktif' end) as [Status],
                    (SELECT Lookupvaluename FROM gnmstlookupdtl WHERE codeid='PFCN' 
                     AND lookupvalue = b.ProfitCentercode) as Profit
                FROM 
                    gnMstSupplier a
                LEFT JOIN gnmstSupplierProfitCenter b ON a.CompanyCode= b.CompanyCode
	                AND a.SupplierCode = b.SupplierCode
                LEFT JOIN gnMstCustomer c ON a.SupplierCode = c.customercode
                WHERE 
                    a.CompanyCode           = @CompanyCode
                    and b.BranchCode        = @BranchCode
                    AND a.Status            = @Status
                    AND b.ProfitCenterCode  = @ProfitCenterCode                
                ORDER BY 
                    a.SupplierCode 
end

GO

if object_id('uspfn_omSlsDoLkpMdlCode') is not null
	drop procedure uspfn_omSlsDoLkpMdlCode
GO

CREATE procedure [dbo].[uspfn_omSlsDoLkpMdlCode]   
(  
 @CompanyCode varchar(15),  
 @BranchCode varchar(15),  
 @SONo varchar(15) 
)  
AS  
BEGIN  
--exec uspfn_omSlsDoLkpMdlCode 6006410,600641001,''
SELECT  
	a.CompanyCode
	,a.SalesModelCode
	,a.SalesModelDesc
	,a.FakturPolisiDesc
	,a.EngineCode
	,a.PpnBmCodeBuy
	,a.PpnBmPctBuy
	,a.PpnBmCodeSell
	,a.PpnBmPctSell
	,a.PpnBmCodePrincipal
	,a.PpnBmPctPrincipal
	,a.Remark
	,a.BasicModel
	,a.TechnicalModelCode
	,a.ProductType
	,a.TransmissionType
	,a.IsChassis
	,a.IsCbu
	,a.SMCModelCode
	,a.GroupCode
	,a.TypeCode
	,a.CylinderCapacity
	,a.fuel
	,a.ModelPrincipal
	,a.Specification
	,a.ModelLine
	,a.Status
	,a.CreatedBy
	,a.CreatedDate
	,a.LastUpdateBy
	,a.LastUpdateDate
	,a.IsLocked
	,a.LockedBy
	,a.LockedDate
	,a.MarketModelCode
	,a.GroupMarketModel
	,a.ColumnMarketModel
FROM omMstModel a
INNER JOIN omTrSalesSOModel b
ON b.CompanyCode = a.CompanyCode
AND b.SalesModelCode = a.SalesModelCode	
WHERE a.CompanyCode = @CompanyCode
AND b.BranchCode = @BranchCode
AND b.SONo = @SONo
AND b.QuantityDO < b.QuantitySO
ORDER BY a.SalesModelCode ASC
end


GO

if object_id('uspfn_omSlsDoLkpMdlYear') is not null
	drop procedure uspfn_omSlsDoLkpMdlYear
GO
create procedure [dbo].[uspfn_omSlsDoLkpMdlYear]   
(  
 @CompanyCode varchar(15),  
 @BranchCode varchar(15),  
 @SONo varchar(15),
 @SalesModelCode varchar(20)
)  
AS  

begin
--exec uspfn_omSlsDoLkpMdlYear 6006410,600641001,'',''
SELECT * FROM omMstModelYear a 
			 WHERE a.CompanyCode = @CompanyCode  
			 AND a.SalesModelCode =@SalesModelCode
			 AND a.Status = '1' 
			 AND a.SalesModelYear IN (SELECT b.SalesModelYear FROM omTrSalesSOModel b 
									  WHERE b.CompanyCode = a.CompanyCode 
									  AND b.SalesModelCode = a.SalesModelCode
									 AND b.SONo = @SONo	
								 AND b.BranchCode = @BranchCode)							 
end			
GO

if object_id('uspfn_omSlsDoLkpShipto') is not null
	drop procedure uspfn_omSlsDoLkpShipto
GO

CREATE procedure [dbo].[uspfn_omSlsDoLkpShipto]   
(  
 @CompanyCode varchar(15),  
 @BranchCode varchar(15),  
 @ProfitCenterCode varchar(3)
) 
as begin
--exec uspfn_omSlsDoLkpShipto 6006410,600641001,300
SELECT a.CompanyCode
	,a.CustomerCode
	,a.StandardCode
	,a.CustomerName
	,a.CustomerAbbrName
	,a.CustomerGovName
	,a.CustomerType
	,a.CategoryCode
	,a.Address1
	,a.Address2
	,a.Address3
	,a.Address4
	,a.PhoneNo
	,a.HPNo
	,a.FaxNo
	,a.isPKP
	,a.NPWPNo
	,a.NPWPDate
	,a.SKPNo
	,a.SKPDate
	,a.ProvinceCode
	,a.AreaCode
	,a.CityCode
	,a.ZipNo
	,a.Status
	,a.CreatedBy
	,a.CreatedDate
	,a.LastUpdateBy
	,a.LastUpdateDate
	,a.isLocked
	,a.LockingBy
	,a.LockingDate
	,a.Email
	,a.BirthDate
	,a.Spare01
	,a.Spare02
	,a.Spare03
	,a.Spare04
	,a.Spare05
	,a.Gender
	,a.OfficePhoneNo
	,a.KelurahanDesa
	,a.KecamatanDistrik
	,a.KotaKabupaten
	,a.IbuKota
	,a.CustomerStatus
	  FROM gnMstCustomer a 
	INNER JOIN gnMstCustomerProfitCenter b
	  ON a.CompanyCode = b.CompanyCode AND 
		 a.CustomerCode = b.CustomerCode AND
		 b.BranchCode = @BranchCode
	WHERE a.CompanyCode = @CompanyCode AND 
		  b.ProfitCenterCode = @ProfitCenterCode                      
end                

GO

if object_id('uspfn_omSlsDoLkpSO') is not null
	drop procedure uspfn_omSlsDoLkpSO
GO

                
CREATE procedure [dbo].[uspfn_omSlsDoLkpSO]   
(  
 @CompanyCode varchar(15),  
 @BranchCode varchar(15),  
 @RefType varchar(8)
)  
AS  
BEGIN  
--exec uspfn_omSlsDoLkpSO 6006410,600641001,'WARE'
 SELECT Distinct a.SONo, a.SODate, a.SKPKNo, a.RefferenceNo, c.CustomerName, a.CustomerCode, 
                c.Address1 + ' ' + c.Address2 + ' ' + c.Address3 + ' ' + c.Address4 as Address,
                a.WareHouseCode, e.RefferenceDesc1 AS WareHouseName
                , (CASE ISNULL(a.SalesType, 0) WHEN 0 THEN 'WholeSales' ELSE 'Direct' END) AS TypeSales
                FROM omTrSalesSO a
                INNER JOIN omTrSalesSOModel b ON b.CompanyCode = a.CompanyCode AND b.BranchCode = a.BranchCode AND b.SONo = a.SONo
                LEFT JOIN gnMstCustomer c ON c.CompanyCode = a.CompanyCode AND c.CustomerCode = a.CustomerCode
                LEFT JOIN gnMstCustomer d ON d.CompanyCode = a.CompanyCode AND d.CustomerCode = a.ShipTo
                LEFT JOIN OmMstRefference e ON e.CompanyCode = a.CompanyCode AND e.RefferenceType = @RefType AND e.RefferenceCode = a.WareHouseCode
                WHERE a.Status = '2'
                AND a.CompanyCode = @CompanyCode
                AND b.BranchCode = @BranchCode
                AND b.QuantityDO < b.QuantitySO
                ORDER BY a.SONo ASC
                
end 

GO

if object_id('uspfn_omSlsDoUpdateSOVin') is not null
	drop procedure uspfn_omSlsDoUpdateSOVin
GO
CREATE procedure [dbo].[uspfn_omSlsDoUpdateSOVin]   
(  
 @CompanyCode varchar(15),  
 @BranchCode varchar(15),  
 @DONo varchar(15) 
)  
AS  

begin
--exec uspfn_omSlsDoUpdateSOVin 6006410,600641001,'DOS/14/000025'
select a.*,b.SONo,c.ServiceBookNo,c.KeyNo from OmTrSalesDODetail a inner join OmTrSalesDO b 
	on a.companyCode = b.companyCode and a.branchCode = b.branchCode and a.DONo = b.DONo
	inner join OmMstVehicle c on a.companyCode = c.companyCode and 
	a.chassisCode = c.chassisCode and a.chassisNo = c.chassisNo
where a.companyCode = @CompanyCode
and a.branchCode = @BranchCode and a.DONo = @DONo
					 
end	
GO

if object_id('uspfn_omSlsInvBrowse') is not null
	drop procedure uspfn_omSlsInvBrowse
GO

CREATE procedure [dbo].[uspfn_omSlsInvBrowse]   
(  
 @CompanyCode varchar(15),  
 @BranchCode varchar(15) 
)  
AS  
BEGIN  
-- exec uspfn_omSlsInvBrowse 6006410,600641001,'DOS/14/000002'
SELECT a.BranchCode
    , a.InvoiceNo
    , a.InvoiceDate
    , a.SONo
    , d.SKPKNo
    , d.RefferenceNo
    , a.CustomerCode
    , b.CustomerName
    , a.BillTo
    , b.CustomerName AS BillName
    , b.Address1 + ' ' + b.Address2 + ' ' + b.Address3 + ' ' + b.Address4 as Address
    ,a.Status
    , CASE 
        WHEN a.Status = 0 THEN 'OPEN'
        WHEN a.Status = 1 THEN 'PRINTED'
        WHEN a.Status = 2 THEN 'APPROVED'
        WHEN a.Status = 3 THEN 'DELETED'
        WHEN a.Status = 5 THEN 'POSTED'
        WHEN a.Status = 9 THEN 'FINISHED'
      END as StatusDsc
    , c.SalesType
    , CASE ISNULL(c.SalesType, 0) 
        WHEN 0 THEN 'Wholesales' 
        ELSE 'Direct' 
      END AS SalesTypeDsc
    ,a.Remark
     
FROM omTrSalesInvoice a
LEFT JOIN gnMstCustomer b ON a.CompanyCode = b.CompanyCode 
    AND a.CustomerCode = b.CustomerCode
LEFT JOIN gnMstCustomerProfitCenter c ON a.CompanyCode = c.CompanyCode 
    AND a.BranchCode = c.BranchCode 
    AND a.CustomerCode = c.CustomerCode 
    AND c.ProfitCenterCode = '100'
LEFT JOIN omTrSalesSO d ON a.CompanyCode = d.CompanyCode 
    AND a.BranchCode = d.BranchCode 
    AND a.SONo = d.SONo
WHERE a.CompanyCode = @companyCode 
    AND a.BranchCode like @branchCode
ORDER BY a.InvoiceNo DESC  
END


GO

if object_id('uspfn_omSlsInvGetAccsSeqTotal4Tax') is not null
	drop procedure uspfn_omSlsInvGetAccsSeqTotal4Tax
GO
Create procedure [dbo].[uspfn_omSlsInvGetAccsSeqTotal4Tax]   
(  
 @CompanyCode varchar(15),  
 @BranchCode varchar(15), 
 @InvoiceNo varchar(30) 
)  
AS  
BEGIN 
-- exec uspfn_omSlsInvGetTotal4Tax 6006410,600641001,''
SELECT * INTO #f1 FROM(
    SELECT (ISNULL(SUM(a.Quantity), 0) * ISNULL(SUM(a.DPP), 0)) AS DPPAmt
        , ISNULL(SUM(a.Quantity), 0) AS TotQuantity
        , (ISNULL(SUM(a.Quantity), 0) * ISNULL(SUM(a.PPn), 0)) AS PPnAmt
        , (ISNULL(SUM(a.Quantity), 0) * ISNULL(SUM(a.Total), 0)) AS TotalAmt
    FROM omTrSalesInvoiceAccsSeq a
    WHERE a.CompanyCode = @CompanyCode 
        AND a.BranchCode = @BranchCode 
        AND a.InvoiceNo = @InvoiceNo
    GROUP BY a.PartNo   
) #f1

SELECT ISNULL(SUM(DPPAmt), 0) AS DPPAmt
    , ISNULL(SUM(TotQuantity), 0) AS TotQuantity
    , ISNULL(SUM(PPnAmt), 0) AS PPnAmt
    , ISNULL(SUM(TotalAmt), 0) AS TotalAmt
FROM #f1
DROP TABLE #f1  
end                

GO

if object_id('uspfn_omSlsInvLkpBillTo') is not null
	drop procedure uspfn_omSlsInvLkpBillTo
GO

CREATE procedure [dbo].[uspfn_omSlsInvLkpBillTo]   
(  
 @CompanyCode varchar(15),  
 @BranchCode varchar(15),
 @ProfitCenterCode varchar(3)
)  
AS  
BEGIN  
-- exec [uspfn_omSlsInvLkpBillTo] 6006410,600641001,100
 SELECT a.CustomerCode, a.CustomerName
                  FROM gnMstCustomer a 
                INNER JOIN gnMstCustomerProfitCenter b
                  ON a.CompanyCode = b.CompanyCode AND 
                     a.CustomerCode = b.CustomerCode AND
                     b.BranchCode = @BranchCode
                WHERE a.CompanyCode = @CompanyCode AND 
                      b.ProfitCenterCode = @ProfitCenterCode

end
GO

if object_id('uspfn_omSlsInvLkpBPK') is not null
	drop procedure uspfn_omSlsInvLkpBPK
GO

CREATE procedure [dbo].[uspfn_omSlsInvLkpBPK]   
(  
 @CompanyCode varchar(15),  
 @BranchCode varchar(15),
 @SONo varchar(15),
 @INVDate datetime
)  
AS  
BEGIN  
-- exec [uspfn_omSlsInvLkpBPK] 6006410,600641001,'','2012-01-01'
SELECT distinct a.BPKNo,a.BPKDate,a.DONo,a.SONo
                  FROM omTrSalesBPK a inner join omTrSalesBPKDetail b
	                on a.companyCode = b.companyCode and a.branchCode = b.branchCode and a.BPKNo = b.BPKNo
                 WHERE a.CompanyCode = @CompanyCode
                       AND a.BranchCode = @BranchCode
                       --AND MONTH(a.BPKDate) = @INVDate
                        AND a.BPKDate <= @INVDate
                        AND b.StatusInvoice = '0'
                        AND a.Status = '2'
                        AND SONO = @SONo
                ORDER BY a.BPKNo
END      

GO

if object_id('uspfn_omSlsInvLkpSlsMdlCd') is not null
	drop procedure uspfn_omSlsInvLkpSlsMdlCd
GO
Create procedure [dbo].[uspfn_omSlsInvLkpSlsMdlCd]   
(  
 @CompanyCode varchar(15),  
 @BranchCode varchar(15),
 @BPKNo varchar(15)
)  
AS  
BEGIN  

SELECT a.*
FROM omMstModel a
WHERE EXISTS
      (SELECT 1
         FROM omTrSalesBPKModel b
        WHERE (b.QuantityBPK - b.QuantityInvoice) > 0
              AND a.SalesModelCode = b.SalesModelCode
              AND a.CompanyCode = b.CompanyCode
              AND b.BranchCode = @BranchCode
              AND b.BPKNo = @BPKNo)
   AND a.CompanyCode = @CompanyCode
ORDER BY a.SalesModelCode
                
END
GO

if object_id('uspfn_omSlsInvLkpSlsMdlYear') is not null
	drop procedure uspfn_omSlsInvLkpSlsMdlYear
GO

Create procedure [dbo].[uspfn_omSlsInvLkpSlsMdlYear]   
(  
 @CompanyCode varchar(15),  
 @BranchCode varchar(15),
 @BPKNo varchar(15),
 @SalesModelCode varchar(30)
)  
AS  
BEGIN  
SELECT * 
                  FROM omMstModelYear a
                 WHERE a.CompanyCode = @CompanyCode
                       AND a.Status = '1'
                       AND a.SalesModelCode = @SalesModelCode
                       AND a.SalesModelYear IN 
                             (SELECT b.SalesModelYear
                                FROM omTrSalesBPKModel b
                               WHERE b.CompanyCode = a.CompanyCode
                                     AND b.BranchCode = @BranchCode
                                     AND b.BPKNo = @BPKNo
                                     AND b.SalesModelCode = a.SalesModelCode
                                     AND (b.QuantityBPK - b.QuantityInvoice) > 0)                        
End
GO

if object_id('uspfn_omSlsInvLkpSO') is not null
	drop procedure uspfn_omSlsInvLkpSO
GO

CREATE procedure [dbo].[uspfn_omSlsInvLkpSO]   
(  
 @CompanyCode varchar(15),  
 @BranchCode varchar(15) 
)  
AS  
BEGIN  
-- exec uspfn_omSlsInvLkpSO 6006410,600641001
    SELECT tableA.SONo,tableA.QtyBPK,tableA.QtyInvoice, tableB.CustomerCode, tableB.CustomerName, tableB.BillTo, tableB.BillName,
				tableB.Address,tableB.SalesType,tableB.SalesTypeDsc,tableB.TOPDays, tableB.SKPKNo, tableB.RefferenceNo				
      FROM (SELECT a.SONo, sum (b.QuantityBPK)  AS QtyBPK, sum (b.QuantityInvoice)  AS QtyInvoice                   
              FROM omTrSalesBPK a, omTrSalesBPKModel b
             WHERE a.CompanyCode = b.CompanyCode
                   AND a.BranchCode = b.BranchCode
                   AND a.BPKNo = b.BPKNo
                   AND a.CompanyCode = @CompanyCode
                   AND a.BranchCode = @BranchCode
                   AND a.Status = '2' 
             GROUP BY a.SONo) tableA,
           (SELECT a.SONo, a.CustomerCode, b.CustomerName, a.BillTo, b.CustomerName as BillName,
			b.Address1 + ' ' + b.Address2 + ' ' + b.Address3 + ' ' + b.Address4 as Address,a.SalesType
            , (CASE ISNULL(a.SalesType, 0) WHEN 0 THEN 'WholeSales' ELSE 'Direct' END) AS SalesTypeDsc
            , ISNULL(a.TOPDays, 0) AS TOPDays, a.SKPKNo, a.RefferenceNo
              FROM omTrSalesSO a
			  LEFT JOIN gnMstCustomer b ON a.CompanyCode = b.CompanyCode AND a.CustomerCode = b.CustomerCode
             WHERE a.CompanyCode = @CompanyCode
                   AND a.BranchCode = @BranchCode
                   AND a.Status = '2') tableB
     WHERE tableA.QtyBPK > tableA.QtyInvoice AND tableA.SONo = tableB.SONo
    ORDER BY tableA.SONo
END

GO

if object_id('uspfn_omSlsInvModelGetTotal4Tax') is not null
	drop procedure uspfn_omSlsInvModelGetTotal4Tax
GO

Create procedure [dbo].[uspfn_omSlsInvModelGetTotal4Tax]   
(  
 @CompanyCode varchar(15),  
 @BranchCode varchar(15), 
 @InvoiceNo varchar(30) 
)  
AS  
BEGIN 
		 SELECT * INTO #f1 FROM (
             SELECT ISNULL(SUM(a.Quantity) * SUM(a.BeforeDiscDPP), 0) AS DPPAmt, ISNULL(SUM(a.Quantity) * SUM(a.DiscExcludePPn), 0) AS DiscAmt
            , (ISNULL(SUM(a.Quantity) * SUM(a.BeforeDiscDPP), 0) - ISNULL(SUM(a.Quantity) * SUM(a.DiscExcludePPn), 0) + ISNULL(SUM(a.Quantity) * SUM(a.AfterDiscPPn), 0)) AS TotalAmt, ISNULL(SUM(a.Quantity) * SUM(a.AfterDiscPPn), 0) AS PPnAmt
            , ISNULL(SUM(a.Quantity), 0) AS TotQuantity, ISNULL(SUM(a.Quantity) * SUM(a.PPnBMPaid), 0) AS PPnBMPaid FROM omTrSalesInvoiceModel a
             WHERE a.CompanyCode = @CompanyCode AND a.BranchCode = @BranchCode AND a.InvoiceNo = @InvoiceNo
             GROUP BY a.InvoiceNo, a.BPKNo, a.SalesModelCode, a.SalesModelYear
             ) #f1
            
            
         SELECT ISNULL(SUM(DPPAmt), 0) AS DPPAmt, ISNULL(SUM(DiscAmt), 0) AS DiscAmt
            , ISNULL(SUM(TotalAmt), 0) AS TotalAmt, ISNULL(SUM(PPnAmt), 0) AS PPnAmt
             , ISNULL(SUM(TotQuantity), 0) AS TotQuantity, ISNULL(SUM(PPnBMPaid), 0) AS PPnBMPaid FROM #f1
            
			DROP TABLE #f1

end          
GO

if object_id('uspfn_omSlsInvOtherGetTotal4Tax') is not null
	drop procedure uspfn_omSlsInvOtherGetTotal4Tax
GO

Create procedure [dbo].[uspfn_omSlsInvOtherGetTotal4Tax]   
(  
 @CompanyCode varchar(15),  
 @BranchCode varchar(15), 
 @InvoiceNo varchar(30) 
)  
AS  
BEGIN 
SELECT * INTO #f1 FROM(
          SELECT (ISNULL(SUM(b.Quantity), 0) * ISNULL(SUM(a.AfterDiscDPP), 0)) AS DPPAmt, ISNULL(SUM(b.Quantity), 0) AS TotQuantity
         , (ISNULL(SUM(b.Quantity), 0) * ISNULL(SUM(a.AfterDiscPPn), 0)) AS PPnAmt, (ISNULL(SUM(b.Quantity), 0)  * ISNULL(SUM(a.AfterDiscTotal), 0)) AS TotalAmt
          FROM omTrSalesInvoiceOthers a
          LEFT JOIN omTrSalesInvoiceModel b ON b.CompanyCode = a.CompanyCode AND b.BranchCode = a.BranchCode
          AND b.InvoiceNo = a.InvoiceNo AND b.SalesModelCode = a.SalesModelCode AND b.SalesModelYear = a.SalesModelYear
          WHERE a.CompanyCode = @CompanyCode AND a.BranchCode = @BranchCode AND a.InvoiceNo = @InvoiceNo
          GROUP BY a.SalesModelCode, a.SalesModelYear, a.OtherCode
          ) #f1

          SELECT ISNULL(SUM(DPPAmt), 0) AS DPPAmt, ISNULL(SUM(TotQuantity), 0) AS TotQuantity, ISNULL(SUM(PPnAmt), 0) AS PPnAmt, ISNULL(SUM(TotalAmt), 0) AS TotalAmt
          FROM #f1
          DROP TABLE #f1
          
end      
GO

if object_id('uspfn_omSlsInvSlctFrTblInvAccSeq') is not null
	drop procedure uspfn_omSlsInvSlctFrTblInvAccSeq
GO

Create procedure [dbo].[uspfn_omSlsInvSlctFrTblInvAccSeq]   
(  
 @CompanyCode varchar(15),  
 @BranchCode varchar(15), 
 @InvoiceNo varchar(30) 
)  
AS  
BEGIN  
SELECT a.PartNo
    , ISNULL(b.PartName,'') AS PartName
    , a.SupplySlipNo
    , a.Quantity
    , a.DPP
    , a.PPn
    , a.Total * a.Quantity as Total
FROM omTrSalesInvoiceAccsSeq a
LEFT JOIN spMstItemInfo b ON b.CompanyCode=a.CompanyCode
    AND b.PartNo=a.PartNo
WHERE a.CompanyCode = @CompanyCode
   AND a.BranchCode = @BranchCode
   AND a.InvoiceNo = @InvoiceNo
End   

GO

if object_id('uspfn_omSlsInvSlctMdlYrVldt') is not null
	drop procedure uspfn_omSlsInvSlctMdlYrVldt
GO
create procedure [dbo].[uspfn_omSlsInvSlctMdlYrVldt]   
(  
 @CompanyCode varchar(15),  
 @BranchCode varchar(15),
 @BPKNo varchar(15),
 @InvoiceNo varchar(30),
 @SalesModelCode varchar(30)
)  
AS  
BEGIN  
 SELECT a.SalesModelYear, a.SalesModelDesc
                  FROM omMstModelYear a
                 WHERE a.CompanyCode = @CompanyCode
                       AND a.Status = '1'
                       AND a.SalesModelCode = @SalesModelCode
                       AND a.SalesModelYear IN
                             (SELECT b.SalesModelYear
                                FROM omTrSalesBPKModel b
                               WHERE b.CompanyCode = a.CompanyCode
                                     AND b.BranchCode = @BranchCode
                                     AND b.BPKNo = @BPKNo
                                     AND b.SalesModelCode = a.SalesModelCode
                                     AND (b.QuantityBPK - b.QuantityInvoice) > 0)
                UNION
                SELECT z.SalesModelYear, (SELECT x.SalesModelDesc 
                       FROM omMstModelYear x
                       WHERE x.SalesModelYear = z.SalesModelYear and 
			                 x.SalesModelCode = z.SalesModelCode)  AS SalesModelDesc
                  FROM omTrSalesInvoiceModel z
                 WHERE z.CompanyCode = @CompanyCode
                       AND z.BranchCode = @BranchCode
                       AND z.InvoiceNo = @InvoiceNo
                       AND z.BPKNo = @BPKNo
                       AND z.SalesModelCode = @SalesModelCode
                ORDER BY a.SalesModelYear 
                
end

GO

if object_id('uspfn_omSlsInvSlctModelVldt') is not null
	drop procedure uspfn_omSlsInvSlctModelVldt
GO

Create procedure [dbo].[uspfn_omSlsInvSlctModelVldt]   
(  
 @CompanyCode varchar(15),  
 @BranchCode varchar(15),
 @BPKNo varchar(15),
 @InvoiceNo varchar(30)
)  
AS  
BEGIN  
 SELECT a.SalesModelCode, a.SalesModelDesc
                  FROM omMstModel a
                 WHERE EXISTS
                          (SELECT 1
                             FROM omTrSalesBPKModel b
                            WHERE (b.QuantityBPK - b.QuantityInvoice) > 0
                                  AND a.SalesModelCode = b.SalesModelCode
                                  AND a.CompanyCode = b.CompanyCode
                                  AND b.BranchCode = @BranchCode
                                  AND b.BPKNo = @BPKNo)
                       AND a.CompanyCode = @CompanyCode
                UNION
                SELECT z.SalesModelCode, (SELECT x.SalesModelDesc
                       FROM omMstModel x
                       WHERE z.SalesModelCode = x.SalesModelCode)  AS SalesModelDesc
                  FROM omTrSalesInvoiceModel z
                 WHERE z.CompanyCode = @CompanyCode
                       AND z.BranchCode = @BranchCode
                       AND z.InvoiceNo = @InvoiceNo
                       AND z.BPKNo = @BPKNo
                ORDER BY a.SalesModelCode
                
end
GO

if object_id('uspfn_omSlsInvSlsMdlYearVldt') is not null
	drop procedure uspfn_omSlsInvSlsMdlYearVldt
GO

Create procedure [dbo].[uspfn_omSlsInvSlsMdlYearVldt]   
(  
 @CompanyCode varchar(15),  
 @BranchCode varchar(15),
 @InvoiceNo varchar(30),
 @BPKNo varchar(15),
 @SalesModelCode varchar(30)
)  
AS  
BEGIN 
SELECT a.SalesModelYear, a.SalesModelDesc
                  FROM omMstModelYear a
                 WHERE a.CompanyCode = @CompanyCode
                       AND a.Status = '1'
                       AND a.SalesModelCode = @SalesModelCode
                       AND a.SalesModelYear IN
                             (SELECT b.SalesModelYear
                                FROM omTrSalesBPKModel b
                               WHERE b.CompanyCode = a.CompanyCode
                                     AND b.BranchCode = @BranchCode
                                     AND b.BPKNo = @BPKNo
                                     AND b.SalesModelCode = a.SalesModelCode
                                     AND (b.QuantityBPK - b.QuantityInvoice) > 0)
                UNION
                SELECT z.SalesModelYear, (SELECT x.SalesModelDesc 
                       FROM omMstModelYear x
                       WHERE x.SalesModelYear = z.SalesModelYear and 
			                 x.SalesModelCode = z.SalesModelCode)  AS SalesModelDesc
                  FROM omTrSalesInvoiceModel z
                 WHERE z.CompanyCode = @CompanyCode
                       AND z.BranchCode = @BranchCode
                       AND z.InvoiceNo = @InvoiceNo
                       AND z.BPKNo = @BPKNo
                       AND z.SalesModelCode = @SalesModelCode
                ORDER BY a.SalesModelYear
END                
          
GO

if object_id('uspfn_omSlsPrlgkpnOutBrwDocBPK') is not null
	drop procedure uspfn_omSlsPrlgkpnOutBrwDocBPK
GO

create procedure [dbo].[uspfn_omSlsPrlgkpnOutBrwDocBPK]   
(  
 @CompanyCode varchar(15),  
 @BranchCode varchar(15),
 @ProfitCenterCode varchar(15)
 )  
AS  
BEGIN  
SELECT Distinct a.BPKNo, a.CustomerCode ,c.CustomerName,a.CustomerCode  + ' - ' + c.CustomerName as Customer,
                c.Address1 + ' ' + c.Address2 + ' ' + c.Address3 + ' ' + c.Address4 as Address 
                    FROM omTrSalesBPK a, gnMstCustomerProfitCenter b, gnMstCustomer c
                 WHERE a.CompanyCode = b.CompanyCode
                       AND a.CompanyCode = @CompanyCode
                       AND a.BranchCode = @BranchCode
                       AND b.ProfitCenterCode = @ProfitCenterCode                    
					   AND a.CompanyCode = c.CompanyCode
					   AND a.CustomerCode = c.CustomerCode
                       AND a.Status = '2'                      
                ORDER BY a.BPKNo ASC                
END                
            
            
            
GO

if object_id('uspfn_omSlsPrlgkpnOutBrwDocTransfer') is not null
	drop procedure uspfn_omSlsPrlgkpnOutBrwDocTransfer
GO
CREATE procedure [dbo].[uspfn_omSlsPrlgkpnOutBrwDocTransfer]   
(  
 @CompanyCode varchar(15),  
 @BranchCode varchar(15)
 )  
AS  
BEGIN  
SELECT 
a.TransferOutNo, 
a.TransferOutDate, 
a.ReferenceNo, 
a.ReferenceDate, 
a.BranchCodeFrom, 
b.BranchName AS BranchNameFrom, 
a.BranchCodeTo, 
c.BranchName AS BranchNameTo, 
a.WareHouseCodeFrom, 
d.RefferenceDesc1 AS WareHouseNameFrom, 
a.WareHouseCodeTo, 
e.RefferenceDesc1 AS WareHouseNameTo, 
a.ReturnDate, 
a.Remark, 
a.Status,
CASE a.Status 
	WHEN '0' THEN 'OPEN' 
	WHEN '1' THEN 'PRINTED' 
	WHEN '2' THEN 'APPROVED' 
	WHEN '3' THEN 'DELETED' 
	WHEN '5' THEN 'TRANSFERED' 
	WHEN '9' THEN 'FINISHED' 
	ELSE '' 
END 
AS StatusDsc
FROM  OmTrInventTransferOut a 
LEFT JOIN gnMstOrganizationDtl b
ON    a.CompanyCode = b.CompanyCode 
AND   a.BranchCodeFrom = b.BranchCode 
LEFT JOIN gnMstOrganizationDtl c
ON    a.CompanyCode = c.CompanyCode 
AND   a.BranchCodeTo = c.BranchCode 
LEFT JOIN omMstRefference d
ON    a.CompanyCode = d.CompanyCode 
AND   a.WareHouseCodeFrom = d.RefferenceCode 
AND   d.RefferenceType = 'WARE' 
LEFT JOIN omMstRefference e
ON    e.CompanyCode = a.CompanyCode 
AND   e.RefferenceCode = a.WareHouseCodeTo 
AND   e.RefferenceType = 'WARE' 
WHERE a.CompanyCode = @CompanyCode
AND   a.BranchCode = @BranchCode
ORDER BY a.TransferOutNo DESC,a.TransferOutDate DESC        
END     
GO

if object_id('uspfn_omSlsPrlgkpnOutDtl') is not null
	drop procedure uspfn_omSlsPrlgkpnOutDtl
GO

CREATE procedure [dbo].[uspfn_omSlsPrlgkpnOutDtl]   
(  
 @CompanyCode varchar(15),  
 @BranchCode varchar(15),
 @PerlengkapanNo varchar(15),
 @SalesModelCode varchar(15)
 )  
AS  
BEGIN  
	SELECT 
		a.PerlengkapanNo, 
		b.PerlengkapanName, 
		a.SalesModelCode,  
		a.PerlengkapanCode,  
		a.QuantityStd,  
		a.Quantity,  
		a.Remark        	
	FROM OmTrSalesPerlengkapanOutDetail a
	LEFT JOIN OmMstPerlengkapan b
	ON a.CompanyCode = b.CompanyCode
	AND a.Branchcode = b.BranchCode
	AND a.PerlengkapanCode = b.PerlengkapanCode
	where a.CompanyCode = @CompanyCode
	and a.BranchCode = @BranchCode
	and a.PerlengkapanNo = @PerlengkapanNo and a.SalesModelCode = @SalesModelCode
	ORDER BY a.PerlengkapanCode ASC         
END            
            
GO

if object_id('uspfn_omSlsPrlgkpnOutLkpMdlDtl') is not null
	drop procedure uspfn_omSlsPrlgkpnOutLkpMdlDtl
GO

CREATE procedure [dbo].[uspfn_omSlsPrlgkpnOutLkpMdlDtl]   
(  
 @CompanyCode varchar(15),  
 @BranchCode varchar(15),
 @SalesModelCode varchar(20) 
 )  
AS  
BEGIN       
 SELECT DISTINCT a.PerlengkapanCode, a.PerlengkapanName,b.Remark,b.Quantity
            FROM OmMstPerlengkapan a
            INNER JOIN omMstModelPerlengkapan b ON
	            a.CompanyCode = b.CompanyCode
	            AND a.BranchCode = b.BranchCode
	            AND a.PerlengkapanCode = b.PerlengkapanCode
	            AND a.Status = '1'
	            AND b.salesModelCode = @SalesModelCode
            INNER JOIN omTrInventQtyPerlengkapan c
	            ON a.companyCode = c.companyCode
	            AND a.branchCode = c.branchCode
	            AND a.perlengkapanCode = c.perlengkapanCode
            WHERE a.CompanyCode = @CompanyCode
	            AND a.BranchCode = @BranchCode
	            --AND c.Year = year(getdate())
	            --AND c.Month = month(getdate())
                AND c.QuantityEnding > 0
            ORDER BY a.PerlengkapanCode ASC            
END         


   
GO

if object_id('uspfn_omSlsReturnBrowse') is not null
	drop procedure uspfn_omSlsReturnBrowse
GO

CREATE procedure uspfn_omSlsReturnBrowse
(    
 @CompanyCode varchar(15),    
 @BranchCode varchar(15)   
)    
AS    
BEGIN    
--exec uspfn_omSlsReturnBrowse 6006410,600641001  
SELECT 
		a.CompanyCode ,
		a.BranchCode ,
		a.ReturnNo ,
		a.ReturnDate ,
		a.ReferenceNo ,
		a.ReferenceDate ,
		a.InvoiceNo ,
		a.InvoiceDate ,
		a.CustomerCode ,
		a.FakturPajakNo ,
		a.FakturPajakDate ,
		a.WareHouseCode ,
		a.Remark ,
		a.Status ,
		a.WareHouseCode,
		b.CustomerName, 
		b.Address1 + ' ' + b.Address2 + ' ' + b.Address3 + ' ' + b.Address4 as Address,
		(
			SELECT 
					c.RefferenceDesc1
			FROM 
					omMstRefference c
			WHERE 
					a.WareHouseCode = c.RefferenceCode
		)  AS WarehouseName, 
		CASE
			WHEN a.Status = '0' THEN 'OPEN'
			WHEN a.Status = '1' THEN 'PRINTED'
			WHEN a.Status = '2' THEN 'APPROVED'
			WHEN a.Status = '3' THEN 'DELETED'
			WHEN a.Status = '5' THEN 'POSTED'
			WHEN a.Status = '9' THEN 'FINISHED'
		END  AS StatusDsc
		, CASE ISNULL(c.SalesType, 0) WHEN 0 THEN 'Wholesales' ELSE 'Direct' END AS SalesTypeDsc
		,c.SalesType
FROM 
		omTrSalesReturn a
		LEFT JOIN gnMstCustomer b ON a.CompanyCode = b.CompanyCode AND a.CustomerCode = b.CustomerCode
		LEFT JOIN gnMstCustomerProfitCenter c ON a.CompanyCode = c.CompanyCode AND a.BranchCode = c.BranchCode AND a.CustomerCode = c.CustomerCode AND c.ProfitCenterCode = '100'
WHERE 
		a.CompanyCode = @CompanyCode
		AND a.BranchCode = @BranchCode
order by 
		a.ReturnNo desc
End               
        
GO

if object_id('uspfn_omSlsReturnLkpInvoice') is not null
	drop procedure uspfn_omSlsReturnLkpInvoice
GO

CREATE procedure [dbo].[uspfn_omSlsReturnLkpInvoice]   
(  
 @CompanyCode varchar(15),  
 @BranchCode varchar(15)
)  
AS  
BEGIN  
	SELECT 
			a.CompanyCode,a.BranchCode	,a.InvoiceNo	,a.InvoiceDate	,a.SONo	,a.CustomerCode	,			
			a.BillTo	,a.FakturPajakNo	
			,a.FakturPajakDate	,a.DueDate	,a.isStandard	,a.Remark	,a.Status,b.CustomerName,
			b.Address1 + ' ' + b.Address2 + ' ' + b.Address3 + ' ' + b.Address4 as Address,			
			c.SalesType, CASE ISNULL(c.SalesType, 0) WHEN 0 THEN 'Wholesales' ELSE 'Direct' END AS SalesTypeDsc
	FROM 
			omTrSalesInvoice a
			LEFT JOIN gnMstCustomer b
			ON a.CompanyCode = b.CompanyCode AND a.CustomerCode = b.CustomerCode
			LEFT JOIN gnMstCustomerProfitCenter c
			ON a.CompanyCode = c.CompanyCode AND a.BranchCode = c.BranchCode AND a.CustomerCode = c.CustomerCode AND c.ProfitCenterCode = '100'                  
			
	WHERE
			a.CompanyCode = @CompanyCode AND a.BranchCode = @BranchCode
			AND a.Status IN ('2', '5')
			AND a.FakturPajakNo != '' 
	ORDER BY 
			a.InvoiceNo ASC                
END
GO

if object_id('uspfn_omSlsReturnLkpMdlYear') is not null
	drop procedure uspfn_omSlsReturnLkpMdlYear
GO
create procedure [dbo].[uspfn_omSlsReturnLkpMdlYear]     
(    
 @CompanyCode varchar(15),    
 @BranchCode varchar(15),    
 @InvoiceNo varchar(15),  
 @SalesModelCode varchar(20)  
)    
AS   
begin  
SELECT a.SalesModelCode, a.SalesModelYear, a.SalesModelDesc, a.ChassisCode, b.BPKNo, b.
                       BeforeDiscDPP, b.AfterDiscDPP, b.AfterDiscPPn, b.AfterDiscPPnBM, b.
                       OthersDPP, b.OthersPPn, b.DiscExcludePPn 
                  FROM omMstModelYear a, omTrSalesInvoiceModel b
                 WHERE a.CompanyCode = b.companyCode
                       AND a.SalesModelCode = b.salesModelCode
                       AND a.SalesModelYear = b.salesModelYear
                       AND b.companyCode = @CompanyCode
                       AND b.branchCode = @BranchCode
                       AND b.invoiceNo = @InvoiceNo
                       AND b.salesModelCode = @SalesModelCode
                       AND (b.quantity - b.quantityReturn) > 0
                ORDER BY a.SalesModelYear ASC         
end   
GO

if object_id('uspfn_omSlsReturnLkpSlsMdlCd') is not null
	drop procedure uspfn_omSlsReturnLkpSlsMdlCd
GO
Create procedure [dbo].[uspfn_omSlsReturnLkpSlsMdlCd]   
(  
 @CompanyCode varchar(15),  
 @BranchCode varchar(15),
 @InvoiceNo varchar(15)
)  
AS  
BEGIN  
 SELECT * 
                  FROM omMstModel a, omTrSalesInvoiceModel b
                 WHERE a.CompanyCode = b.CompanyCode
                       AND a.SalesModelCode = b.SalesModelCode
                       AND a.CompanyCode = @CompanyCode
                       AND b.BranchCode = @BranchCode
                       AND b.InvoiceNo = @InvoiceNo
                       AND (b.Quantity - b.QuantityReturn) > 0        
                
END

GO

if object_id('uspfn_PenjualFPLookup') is not null
	drop procedure uspfn_PenjualFPLookup
GO
CREATE procedure [dbo].[uspfn_PenjualFPLookup]
	@CompanyCode as varchar(15)
	,@BranchCode as varchar(15)
	,@ProfitCenterCode as varchar(15)
	,@CustomerCode as varchar(15)
as

declare @isBranch int
set @isBranch=(select isBranch from gnMstOrganizationDtl where CompanyCode=@CompanyCode and BranchCode=@BranchCode)

select a.CustomerCode, a.CustomerName,
       a.Address1+' '+a.Address2+' '+a.Address3+' '+a.Address4 as Address,
       c.LookUpValueName as ProfitCenter
from gnMstCustomer a 
	inner join gnMstCustomerProfitCenter b on 
		b.CustomerCode= b.CustomerCode and b.CustomerCode=a.CustomerCode
	inner join gnMstLookUpDtl c on c.CompanyCode= a.CompanyCode
		and c.LookupValue= b.ProfitCenterCode
		and c.CodeID= 'PFCN'
where a.CompanyCode=@CompanyCode
	and b.BranchCode=@BranchCode
	and b.ProfitCenterCode= @ProfitCenterCode
	and b.isBlackList=0
	and a.Status = 1
	and b.SalesType = '0' 
	and a.CustomerCode= (case when @isBranch=1 then @BranchCode else @CustomerCode end)
order by a.CustomerCode
GO

if object_id('uspfn_PostingTaxIn') is not null
	drop procedure uspfn_PostingTaxIn
GO
 CREATE procedure [dbo].[uspfn_PostingTaxIn]
	@CompanyCode nvarchar(25),
	@BranchCode nvarchar(25),
	@ProductType nvarchar(2),
	@PeriodYear int,
	@PeriodMonth int,
	@LastSeqNo int,
	@UserId varchar(max)
  
  as
  begin

SELECT * INTO #1 FROM (
SELECT BranchCode, TaxNo, SupplierCode FROM gnTaxIn WITH(NOLOCK, NOWAIT)
WHERE CompanyCode = @CompanyCode 
    AND BranchCode = case @BranchCode when '' then BranchCode else @BranchCode end 
    AND ProductType = @ProductType 
	AND PeriodYear = @PeriodYear 
    AND PeriodMonth = @PeriodMonth
UNION
SELECT BranchCode, TaxNo, SupplierCode FROM gnTaxInHistory WITH(NOLOCK, NOWAIT)
WHERE CompanyCode = @CompanyCode 
    AND BranchCode = case @BranchCode when '' then BranchCode else @BranchCode end 
    AND ProductType = @ProductType 
	AND PeriodYear = @PeriodYear 
    AND PeriodMonth = @PeriodMonth 
    AND IsDeleted = '1'
) Temp

/* AMBIL SEMUA DATA HPP SPARE */
select * into #t_1 from (
select b.SupplierCode, a.* 
from spTrnPHpp a
left join spTrnPRcvHdr b on b.CompanyCode = a.CompanyCode 
	and b.BranchCode = a.BranchCode
	and b.WRSNo = a.WRSNo
where
	a.CompanyCode	 = @CompanyCode
	and a.BranchCode = case @BranchCode when '' then a.BranchCode else @BranchCode end 
	and a.YearTax	 = @PeriodYear
	and a.MonthTax	 = @PeriodMonth
)#t_1 

/* AMBIL SEMUA DATA BTT OTHER (AP) */
select * into #t_2 from (
select a.* 
from apTrnBTTOtherHdr a
where 
	a.CompanyCode = @CompanyCode
	and a.BranchCode = case @BranchCode when '' then a.BranchCode else @BranchCode end 
	and a.FPJNo <> ''
	and SUBSTRING(a.FPJPeriod, 1, 4) = @PeriodYear
    and RIGHT(a.FPJPeriod, 2) = @PeriodMonth
)#t_2 

/* DATA HPP SPARE YANG MEMILIKI SUPPLIER DAN TAXNO YANG SAMA DENGAN BTT OTHER (NILAI DIJUMLAHKAN) */
SELECT * INTO #t_3 FROM (
SELECT
  a.CompanyCode
, a.BranchCode
, d.ProductType
, a.YearTax PeriodYear
, a.MonthTax PeriodMonth
, '300' ProfitCenter
, a.TypeOfGoods
, 'B' TaxCode
, '2' TransactionCode
, '1' StatusCode
, '1' DocumentCode
, 'F' DocumentType
, b.SupplierCode
, c.SupplierGovName SupplierName
, c.IsPKP
, c.NPWPNo NPWP
, a.ReferenceNo FPJNo
, a.ReferenceDate FPJDate
, a.HPPNo + ' (' + a.ReferenceNo + ')' ReferenceNo
, a.WRSDate ReferenceDate
, a.TaxNo TaxNo
, a.TaxDate TaxDate
, a.DueDate SubmissionDate
, ISNULL((ISNULL(a.TotNetPurchAmt, 0) + ISNULL(a.DiffNetPurchAmt, 0)) + g.DppAmt,0) DPPAmt
, ISNULL((ISNULL(a.TotTaxAmt, 0) + ISNULL(a.DiffTaxAmt, 0)) + g.PPNAmt, 0) PPNAmt
, ISNULL(0 + g.PPNBmAmt, 0) PPNBmAmt
, 'PEMBELIAN SPARE PART' Description
, ISNULL(ISNULL(b.TotItem, 0)+(SELECT COUNT(*) FROM apTrnBTTOtherDtl WHERE CompanyCode = a.CompanyCode 
							AND BranchCode = a.BranchCode AND BTTNo = g.BTTNo), 0) Quantity	
FROM
	spTrnPHPP a	WITH(NOLOCK, NOWAIT)
	LEFT JOIN spTrnPRcvHdr b WITH(NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode
		AND a.BranchCode = b.BranchCode
		AND a.WRSNo = b.WRSNo
	LEFT JOIN gnMstSupplier c WITH(NOLOCK, NOWAIT) ON a.CompanyCode = c.CompanyCode
		AND b.SupplierCode = c.SupplierCode
	LEFT JOIN gnMstCoProfile d WITH(NOLOCK, NOWAIT) ON a.CompanyCode = d.CompanyCode
		AND a.BranchCode = d.BranchCode
    INNER JOIN gnMstSupplierProfitCenter e WITH(NOLOCK, NOWAIT) ON a.CompanyCode = e.CompanyCode
	    AND a.BranchCode = e.BranchCode
        AND b.SupplierCode = e.SupplierCode
    INNER JOIN gnMstTax f WITH(NOLOCK, NOWAIT) ON a.CompanyCode = f.CompanyCode
        AND e.TaxCode = f.TaxCode
	INNER JOIN #t_2 g on g.BranchCode = a.BranchCode AND g.SupplierCode = b.SupplierCode
		AND g.FPJNo = a.TaxNo
WHERE
	a.CompanyCode = @CompanyCode
	AND a.BranchCode = case @BranchCode when '' then a.BranchCode else @BranchCode end 
	AND ProductType = @ProductType
	AND a.YearTax = @PeriodYear
	AND a.MonthTax = @PeriodMonth
	AND a.Status = '2'
    AND e.ProfitCenterCode = '300'
    AND f.TaxPct > 0
) #t_3

-----------------------------------------------------------


SELECT * INTO #TaxIn FROM (
-- SALES : PURCHASE
SELECT
  a.CompanyCode
, a.BranchCode
, e.ProductType
, YEAR(a.LockingDate) PeriodYear
, MONTH(a.LockingDate) PeriodMonth
, '100' ProfitCenter
, NULL TypeOfGoods
, 'B' TaxCode
, '2' TransactionCode
, '1' StatusCode
, '1' DocumentCode
, 'F' DocumentType
, a.SupplierCode 
, d.SupplierGovName SupplierName
, d.IsPKP 
, d.NPWPNo NPWP
, a.RefferenceFakturPajakNo FPJNo
, a.RefferenceFakturPajakDate FPJDate
, a.HPPNo + ' (' + a.RefferenceInvoiceNo + ')' ReferenceNo
, a.RefferenceInvoiceDate ReferenceDate
, a.RefferenceFakturPajakNo TaxNo
, a.RefferenceFakturPajakDate TaxDate
, a.DueDate SubmissionDate
, ISNULL((SELECT SUM(Quantity * (AfterDiscDPP + OthersDPP)) FROM omTrPurchaseHPPDetailModel 
	WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND HPPNo = a.HPPNo ), 0) DPPAmt
, ISNULL((SELECT SUM(Quantity * (AfterDiscPPn + OthersPPn)) FROM omTrPurchaseHPPDetailModel 
	WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND HPPNo = a.HPPNo ), 0) PPNAmt
, ISNULL((SELECT SUM(Quantity * AfterDiscPPnBM) FROM omTrPurchaseHPPDetailModel 
	WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND HPPNo = a.HPPNo ), 0) PPNBmAmt
, (SELECT TOP 1 SalesModelCode + ', NO. CHS. ' + CONVERT(VARCHAR, ChassisNo) FROM omTrPurchaseHPPSubDetail 
	WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND HPPNo = a.HPPNo) Description
, (SELECT COUNT(HPPSeq) FROM omTrPurchaseHPPSubDetail c 
	LEFT JOIN omTrPurchaseHPPDetailModel b ON c.CompanyCode = b.CompanyCode AND c.BranchCode = b.BranchCode 
	AND c.HPPNo = b.HPPNo AND c.BPUNo = b.BPUNo
	WHERE c.CompanyCode = a.CompanyCode AND c.BranchCode = a.BranchCode AND c.HPPNo = a.HPPNo) Quantity
FROM
	omTrPurchaseHPP a WITH(NOLOCK, NOWAIT)
	LEFT JOIN gnMstSupplier d WITH(NOLOCK, NOWAIT) ON a.CompanyCode = d.CompanyCode
		AND a.SupplierCode = d.SupplierCode
	LEFT JOIN gnMstCoProfile e WITH(NOLOCK, NOWAIT) ON a.CompanyCode = e.CompanyCode
		AND a.BranchCode = e.BranchCode
    INNER JOIN gnMstSupplierProfitCenter f WITH(NOLOCK, NOWAIT) ON a.CompanyCode = f.CompanyCode
	    AND a.BranchCode = f.BranchCode
        AND a.SupplierCode = f.SupplierCode
    INNER JOIN gnMstTax g WITH(NOLOCK, NOWAIT) ON a.CompanyCode = g.CompanyCode
        AND f.TaxCode = g.TaxCode
WHERE
	a.CompanyCode = @CompanyCode
	AND a.BranchCode = case @BranchCode when '' then a.BranchCode else @BranchCode end 
	AND ProductType = @ProductType
	AND YEAR(a.LockingDate) = @PeriodYear
	AND MONTH(a.LockingDate) = @PeriodMonth
	AND a.Status NOT IN ('3')
    AND f.ProfitCenterCode = '100'
    AND g.TaxPct > 0
-------------------------------------------------------------------------------------
UNION
-- SALES : KAROSERI
SELECT
  a.CompanyCode
, a.BranchCode
, ProductType
, YEAR(a.LockingDate) PeriodYear
, MONTH(a.LockingDate) PeriodMonth
, '100' ProfitCenter
, NULL TypeOfGoods
, 'B' TaxCode
, '2' TransactionCode
, '1' StatusCode
, '1' DocumentCode
, 'F' DocumentType
, a.SupplierCode
, b.SupplierGovName SupplierName
, b.IsPKP
, b.NPWPNo NPWP
, a.RefferenceFakturPajakNo FPJNo
, a.RefferenceFakturPajakDate FPJDate
, a.KaroseriTerimaNo + ' (' + a.RefferenceInvoiceNo + ')' ReferenceNo
, a.RefferenceInvoiceDate ReferenceDate
, a.RefferenceFakturPajakNo TaxNo
, a.RefferenceFakturPajakDate TaxDate
, a.DueDate SubmissionDate
, ISNULL(a.Quantity, 0) * (ISNULL(a.DPPMaterial, 0) + ISNULL(a.DPPFee, 0) + ISNULL(a.DPPOthers, 0)) DPPAmt
, ISNULL(a.Quantity, 0) * ISNULL(a.PPn, 0) PPNAmt
, 0 PPNBmAmt
, a.KaroseriSPKNo Description
, ISNULL(a.Quantity, 0) Quantity
FROM
	omTrPurchaseKaroseriTerima a WITH(NOLOCK, NOWAIT)
	LEFT JOIN gnMstSupplier b WITH(NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode
		AND a.SupplierCode = b.SupplierCode
	LEFT JOIN gnMstCoProfile c WITH(NOLOCK, NOWAIT) ON a.CompanyCode = c.CompanyCode
		AND a.BranchCode = c.BranchCode
    INNER JOIN gnMstSupplierProfitCenter d WITH(NOLOCK, NOWAIT) ON a.CompanyCode = d.CompanyCode
	    AND a.BranchCode = d.BranchCode
        AND a.SupplierCode = d.SupplierCode
    INNER JOIN gnMstTax e WITH(NOLOCK, NOWAIT) ON a.CompanyCode = e.CompanyCode
        AND d.TaxCode = e.TaxCode
WHERE
	a.CompanyCode = @CompanyCode
	AND a.BranchCode = case @BranchCode when '' then a.BranchCode else @BranchCode end 
	AND ProductType = @ProductType
	AND YEAR(a.LockingDate) = @PeriodYear
	AND MONTH(a.LockingDate) = @PeriodMonth
	AND a.Status NOT IN ('3')
    AND d.ProfitCenterCode = '100'
    AND e.TaxPct > 0
-----------------------------------------------------------------------------------------
UNION
-- SALES : PURCHASE RETURN
SELECT
  a.CompanyCode
, a.BranchCode
, d.ProductType
, YEAR(ReturnDate) PeriodYear
, MONTH(ReturnDate) PeriodMonth
, '100' ProfitCenter
, NULL TypeOfGoods
, 'B' TaxCode
, '2' TransactionCode
, '1' StatusCode
, '1' DocumentCode
, 'R' DocumentType
, c.SupplierCode SupplierCode
, c.SupplierGovName SupplierName
, c.IsPKP IsPKP
, c.NPWPNo NPWP
, replace(a.RefferenceNo,substring(a.RefferenceNo,1,1),'9') FPJNo
, a.RefferenceDate FPJDate
, a.ReturnNo + ' (' + a.RefferenceNo + ')' ReferenceNo
, a.RefferenceDate ReferenceDate
, replace(a.RefferenceNo,substring(a.RefferenceNo,1,1),'9') TaxNo  
, a.RefferenceDate TaxDate
, a.ReturnDate SubmissionDate
, ISNULL((SELECT SUM(Quantity * (AfterDiscDPP + OthersDPP))FROM omTrPurchaseReturnDetailModel 
	WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND ReturnNo = a.ReturnNo), 0) * -1 DPPAmt
, ISNULL((SELECT SUM(Quantity * (AfterDiscPPn + OthersPPn))FROM omTrPurchaseReturnDetailModel 
	WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND ReturnNo = a.ReturnNo), 0) * -1 PPNAmt
, ISNULL((SELECT SUM(Quantity * AfterDiscPPnBM)FROM omTrPurchaseReturnDetailModel 
	WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND ReturnNo = a.ReturnNo), 0) * -1 PPNBmAmt
, a.ReturnNo Description
, ISNULL((SELECT SUM(Quantity)FROM omTrPurchaseReturnDetailModel 
	WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND ReturnNo = a.ReturnNo), 0) * -1 Quantity
FROM
	omTrPurchaseReturn a WITH(NOLOCK, NOWAIT)
	LEFT JOIN omTrPurchaseHPP b WITH(NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode
		AND a.BranchCode = b.BranchCode
		AND a.HPPNo = b.HPPNo
	LEFT JOIN gnMstSupplier c WITH(NOLOCK, NOWAIT) ON a.CompanyCode = c.CompanyCode
		AND b.SupplierCode = c.SupplierCode
	LEFT JOIN gnMstCoProfile d WITH(NOLOCK, NOWAIT) ON a.CompanyCode = d.CompanyCode
		AND a.BranchCode = d.BranchCode
    INNER JOIN gnMstSupplierProfitCenter e WITH(NOLOCK, NOWAIT) ON a.CompanyCode = e.CompanyCode
	    AND a.BranchCode = e.BranchCode
        AND b.SupplierCode = e.SupplierCode
    INNER JOIN gnMstTax f WITH(NOLOCK, NOWAIT) ON a.CompanyCode = f.CompanyCode
        AND e.TaxCode = f.TaxCode
WHERE
	a.CompanyCode = @CompanyCode
	AND a.BranchCode = case @BranchCode when '' then a.BranchCode else @BranchCode end 
	AND ProductType = @ProductType
	AND YEAR(ReturnDate) = @PeriodYear
	AND MONTH(ReturnDate) = @PeriodMonth
	AND a.Status NOT IN ('3')
    AND e.ProfitCenterCode = '100'
    AND f.TaxPct > 0
---------------------------------------------------------------------------------------
UNION
-- SERVICE
SELECT
  a.CompanyCode
, a.BranchCode
, a.ProductType
, YEAR(RecDate) PeriodYear
, MONTH(RecDate) PeriodMonth
, '200' ProfitCenter
, NULL TypeOfGoods
, 'B' TaxCode
, '2' TransactionCode
, '1' StatusCode
, '1' DocumentCode
, 'F' DocumentType
, a.SupplierCode SupplierCode
, b.SupplierGovName SupplierName
, b.IsPKP IsPKP
, b.NPWPNo NPWP
, a.FPJNo FPJNo
, a.FPJDate FPJDate
, a.PONo + ' (' + a.JobOrderNo + ')' ReferenceNo
, a.JobOrderDate RefferenceDate
, a.FPJGovNo TaxNo
, a.FPJDate TaxDate
, a.DueDate SubmissionDate
, ISNULL(a.DPPAmt, 0) DPPAmt
, ISNULL(a.PPnAmt, 0) PPNAmt
, 0 PPNBmAmt
, a.RecNo Description
, 1 Quantity
FROM
	svTrnPOSubCon a	WITH(NOLOCK, NOWAIT)
	LEFT JOIN gnMstSupplier b WITH(NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode
		AND a.SupplierCode = b.SupplierCode
    INNER JOIN gnMstSupplierProfitCenter c WITH(NOLOCK, NOWAIT) ON a.CompanyCode = c.CompanyCode
	    AND a.BranchCode = c.BranchCode
        AND a.SupplierCode = c.SupplierCode
    INNER JOIN gnMstTax d WITH(NOLOCK, NOWAIT) ON a.CompanyCode = d.CompanyCode
        AND c.TaxCode = d.TaxCode
WHERE
	a.CompanyCode = @CompanyCode
	AND a.BranchCode = case @BranchCode when '' then a.BranchCode else @BranchCode end 
	AND a.ProductType = @ProductType
	AND YEAR(RecDate) = @PeriodYear
	AND MONTH(RecDate) = @PeriodMonth
    AND c.ProfitCenterCode = '200'
    AND d.TaxPct > 0
---------------------------------------------------------------------------------------
UNION 
-- SPAREPART
SELECT
  a.CompanyCode
, a.BranchCode
, d.ProductType
, a.YearTax PeriodYear
, a.MonthTax PeriodMonth
, '300' ProfitCenter
, a.TypeOfGoods
, 'B' TaxCode
, '2' TransactionCode
, '1' StatusCode
, '1' DocumentCode
, 'F' DocumentType
, b.SupplierCode
, c.SupplierGovName SupplierName
, c.IsPKP
, c.NPWPNo NPWP
, a.ReferenceNo FPJNo
, a.ReferenceDate FPJDate
, a.HPPNo + ' (' + a.ReferenceNo + ')' ReferenceNo
, a.WRSDate ReferenceDate
, a.TaxNo TaxNo
, a.TaxDate TaxDate
, a.DueDate SubmissionDate
, ISNULL(a.TotNetPurchAmt, 0) + ISNULL(a.DiffNetPurchAmt, 0) DPPAmt
, ISNULL(a.TotTaxAmt, 0) + ISNULL(a.DiffTaxAmt, 0) PPNAmt
, 0 PPNBmAmt
, 'PEMB. S`PART' Description
, ISNULL(b.TotItem, 0) Quantity	
FROM
	spTrnPHPP a	WITH(NOLOCK, NOWAIT)
	LEFT JOIN spTrnPRcvHdr b WITH(NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode
		AND a.BranchCode = b.BranchCode
		AND a.WRSNo = b.WRSNo
	LEFT JOIN gnMstSupplier c WITH(NOLOCK, NOWAIT) ON a.CompanyCode = c.CompanyCode
		AND b.SupplierCode = c.SupplierCode
	LEFT JOIN gnMstCoProfile d WITH(NOLOCK, NOWAIT) ON a.CompanyCode = d.CompanyCode
		AND a.BranchCode = d.BranchCode
    INNER JOIN gnMstSupplierProfitCenter e WITH(NOLOCK, NOWAIT) ON a.CompanyCode = e.CompanyCode
	    AND a.BranchCode = e.BranchCode
        AND b.SupplierCode = e.SupplierCode
    INNER JOIN gnMstTax f WITH(NOLOCK, NOWAIT) ON a.CompanyCode = f.CompanyCode
        AND e.TaxCode = f.TaxCode
WHERE
	a.CompanyCode = @CompanyCode
	AND a.BranchCode = case @BranchCode when '' then a.BranchCode else @BranchCode end 
	AND ProductType = @ProductType
	AND a.YearTax = @PeriodYear
	AND a.MonthTax = @PeriodMonth
	AND a.Status = '2'
    AND e.ProfitCenterCode = '300'
    AND f.TaxPct > 0
    AND a.BranchCode+'-'+b.SupplierCode+'-'+a.TaxNo NOT IN (SELECT BranchCode+'-'+SupplierCode+'-'+TaxNo FROM #t_3)
-------------------------------------------------------------------------------
UNION
-- FINANCE
SELECT
  a.CompanyCode
, a.BranchCode
, c.ProductType
, SUBSTRING(a.FPJPeriod, 1, 4) PeriodYear
, RIGHT(a.FPJPeriod, 2) PeriodMonth
, '000' ProfitCenter
, NULL TypeOfGoods
, 'B' TaxCode
, '2' TransactionCode
, '1' StatusCode
, '1' DocumentCode
, 'F' DocumentType
, a.SupplierCode
, b.SupplierGovName SupplierName
, b.IsPKP
, b.NPWPNo NPWP
, a.FPJNo
, a.FPJDate
, a.BTTNo + ' (' + a.ReffNo + ')' ReferenceNo
, a.ReffDate ReferenceDate
, a.FPJNo TaxNo
, a.FPJDate TaxDate
, a.DueDate SubmissionDate
, a.DPPAmt
, a.PPNAmt
, a.PPnBMAmt
, (SELECT TOP 1 Description FROM apTrnBTTOtherDtl WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND BTTNo = a.BTTNo) Description
, (SELECT COUNT(*) FROM apTrnBTTOtherDtl WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND BTTNo = a.BTTNo) Quantity	
FROM
    apTrnBTTOtherHdr a	WITH(NOLOCK, NOWAIT)    
    LEFT JOIN gnMstSupplier b WITH(NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode
	    AND a.SupplierCode = b.SupplierCode
    LEFT JOIN gnMstCoProfile c WITH(NOLOCK, NOWAIT) ON a.CompanyCode = c.CompanyCode
	    AND a.BranchCode = c.BranchCode
    INNER JOIN gnMstSupplierProfitCenter d WITH(NOLOCK, NOWAIT) ON a.CompanyCode = d.CompanyCode
	    AND a.BranchCode = d.BranchCode
        AND a.SupplierCode = d.SupplierCode
    INNER JOIN gnMstTax e WITH(NOLOCK, NOWAIT) ON a.CompanyCode = e.CompanyCode
        AND d.TaxCode = e.TaxCode
WHERE
    a.CompanyCode = @CompanyCode
    AND a.BranchCode = case @BranchCode when '' then a.BranchCode else @BranchCode end 
    AND ProductType = @ProductType
    AND SUBSTRING(a.FPJPeriod, 1, 4) = @PeriodYear
    AND RIGHT(a.FPJPeriod, 2) = @PeriodMonth
    AND a.Category = 'INV'
    AND d.ProfitCenterCode = '000'
    AND e.TaxPct > 0
    AND a.BranchCode+'-'+b.SupplierCode+'-'+a.FPJNo NOT IN (SELECT BranchCode+'-'+SupplierCode+'-'+TaxNo FROM #t_3)
-------------------------------------------------------------------------------
UNION
-- SPARE PART DAN BTT YANG MEMILIKI SUPPLIER DAN TAXNO YANG SAMA
SELECT * FROM #t_3
) #TaxIn

select ROW_NUMBER() OVER(ORDER BY #TaxIn2.BranchCode ASC, #TaxIn2.ProfitCenter ASC, #TaxIn2.TaxNo ASC) + @LastSeqNo SeqNo, * into #TaxIn2 from(
select 
	q1.CompanyCode
	, q1.BranchCode
	, q1.ProductType
	, q1.PeriodYear
	, q1.PeriodMonth
	, q1.ProfitCenter
	, ISNULL((SELECT TOP 1 TypeOfGoods FROM #TaxIn WHERE CompanyCode = q1.CompanyCode AND BranchCode = q1.BranchCode AND 
		ProductType = q1.ProductType AND PeriodYear = q1.PeriodYear AND PeriodMonth = q1.PeriodMonth AND 
		SupplierCode = q1.SupplierCode AND TaxNo = q1.TaxNo),'') TypeOfGoods
	, q1.TaxCode
	, q1.TransactionCode
	, q1.StatusCode
	, q1.DocumentCode
	, q1.DocumentType
	, q1.SupplierCode
	, q1.SupplierName
	, q1.IsPKP
	, q1.NPWP
	, (SELECT TOP 1 FPJNo FROM #TaxIn WHERE CompanyCode = q1.CompanyCode AND BranchCode = q1.BranchCode AND 
		ProductType = q1.ProductType AND PeriodYear = q1.PeriodYear AND PeriodMonth = q1.PeriodMonth AND 
		SupplierCode = q1.SupplierCode AND TaxNo = q1.TaxNo) FPJNo
	, (SELECT TOP 1 FPJDate FROM #TaxIn WHERE CompanyCode = q1.CompanyCode AND BranchCode = q1.BranchCode AND 
		ProductType = q1.ProductType AND PeriodYear = q1.PeriodYear AND PeriodMonth = q1.PeriodMonth AND 
		SupplierCode = q1.SupplierCode AND TaxNo = q1.TaxNo) FPJDate
	, (SELECT TOP 1 ReferenceNo FROM #TaxIn WHERE CompanyCode = q1.CompanyCode AND BranchCode = q1.BranchCode AND 
		ProductType = q1.ProductType AND PeriodYear = q1.PeriodYear AND PeriodMonth = q1.PeriodMonth AND 
		SupplierCode = q1.SupplierCode AND TaxNo = q1.TaxNo) ReferenceNo
	, (SELECT TOP 1 ReferenceDate FROM #TaxIn WHERE CompanyCode = q1.CompanyCode AND BranchCode = q1.BranchCode AND 
		ProductType = q1.ProductType AND PeriodYear = q1.PeriodYear AND PeriodMonth = q1.PeriodMonth AND 
		SupplierCode = q1.SupplierCode AND TaxNo = q1.TaxNo) ReferenceDate
	, q1.TaxNo
	, (SELECT TOP 1 TaxDate FROM #TaxIn WHERE CompanyCode = q1.CompanyCode AND BranchCode = q1.BranchCode AND 
		ProductType = q1.ProductType AND PeriodYear = q1.PeriodYear AND PeriodMonth = q1.PeriodMonth AND 
		SupplierCode = q1.SupplierCode AND TaxNo = q1.TaxNo) TaxDate
	, (SELECT TOP 1 SubmissionDate FROM #TaxIn WHERE CompanyCode = q1.CompanyCode AND BranchCode = q1.BranchCode AND 
		ProductType = q1.ProductType AND PeriodYear = q1.PeriodYear AND PeriodMonth = q1.PeriodMonth AND 
		SupplierCode = q1.SupplierCode AND TaxNo = q1.TaxNo) SubmissionDate
	, sum(q1.DPPAmt) DPPAmt
	, sum(q1.PPNAmt) PPNAmt
	, sum(q1.PPnBMAmt) PPnBMAmt
	, (SELECT TOP 1 Description FROM #TaxIn WHERE CompanyCode = q1.CompanyCode AND BranchCode = q1.BranchCode AND 
		ProductType = q1.ProductType AND PeriodYear = q1.PeriodYear AND PeriodMonth = q1.PeriodMonth AND 
		SupplierCode = q1.SupplierCode AND TaxNo = q1.TaxNo) Description
	, sum(q1.Quantity) Quantity
from #TaxIn q1
group by
	q1.CompanyCode
	, q1.BranchCode
	, q1.ProductType
	, q1.PeriodYear
	, q1.PeriodMonth
	, q1.ProfitCenter
	, q1.TaxCode
	, q1.TransactionCode
	, q1.StatusCode
	, q1.DocumentCode
	, q1.DocumentType
	, q1.SupplierCode
	, q1.SupplierName
	, q1.IsPKP
	, q1.NPWP
	, q1.TaxNo
) #TaxIn2
WHERE 
    #TaxIn2.BranchCode+'-'+#TaxIn2.TaxNo+'-'+#TaxIn2.SupplierCode 
        NOT IN (SELECT BranchCode+'-'+TaxNo+'-'+SupplierCode FROM #1)

INSERT INTO gnTaxIn SELECT CompanyCode, BranchCode, ProductType, PeriodYear, PeriodMonth, SeqNo, ProfitCenter, 
    TypeOfGoods, TaxCode, TransactionCode, StatusCode, DocumentCode, DocumentType, SupplierCode, SupplierName, 
    IsPKP, NPWP, FPJNo, FPJDate, ReferenceNo, ReferenceDate, TaxNo, TaxDate, SubmissionDate, DPPAmt, PPNAmt, 
    PPNBmAmt, Description, Quantity, 0 IsLocked, null LockingBy, null LockingDate, @UserId CreatedBy, 
    GETDATE() CreatedDate, @UserId LastupdateBy, GETDATE() LastupdateDate
    FROM #TaxIn2

DROP TABLE #1
DROP TABLE #TaxIn
DROP TABLE #TaxIn2
DROP TABLE #t_1
DROP TABLE #t_2
DROP TABLE #t_3
END
GO

if object_id('uspfn_Select4NoPartOrderBalance') is not null
	drop procedure uspfn_Select4NoPartOrderBalance
GO

Create procedure [dbo].[uspfn_Select4NoPartOrderBalance] @CompanyCode varchar(15),
@BranchCode varchar(15), @PosNo varchar(20)
as
SELECT 
	a.POSNo, a.PartNo, b.PartName, CAST(a.OrderQty as decimal(18,2)) as OrderQty, 
	a.OnOrder, a.Intransit, a.Received,a.DiscPct, a.PurchasePrice, 
	Convert(varchar(10),a.SeqNo) SeqNo, a.SupplierCode, a.OnOrder, a.PartNoOriginal, 
	a.TypeOfGoods 
FROM 
	spTrnPOrderBalance a 
INNER JOIN spMstItemInfo b
   ON b.PartNo      = a.PartNo
  AND b.CompanyCode = a.CompanyCode
WHERE a.CompanyCode = @CompanyCode
  AND a.BranchCode  = @BranchCode
  AND a.PosNo    like @PosNo
ORDER BY a.POSNo DESC, a.SeqNo
GO

if object_id('uspfn_SelectAccOther') is not null
	drop procedure uspfn_SelectAccOther
GO
create procedure [dbo].[uspfn_SelectAccOther]
	@CompanyCode varchar(25),
	@Reff varchar(25)
as
begin
	SELECT a.RefferenceCode
	     , a.RefferenceDesc
      FROM dbo.omMstRefference a
     WHERE a.CompanyCode = @CompanyCode
       AND a.RefferenceType = @Reff
       AND a.Status != '0'
end

GO

if object_id('uspfn_SelectInventory4All') is not null
	drop procedure uspfn_SelectInventory4All
GO
--declare @CompanyCode varchar(20)
--declare @BranchCode  varchar(20)
--declare @TypeJournal  varchar(20)
--declare @DocNo   varchar(20)

--set @CompanyCode = '6558201'
--set @BranchCode  = '655820100'
--set @TypeJournal = 'invoice'
--set @DocNo       = 'IVU/13/001280'

-- =============================================
-- Author:		<xxxxxx>
-- Create date: <xxxxxx>
-- Description:	<Get Journal>
-- Last Update By:	<yo - 29 Nov 2013>
-- =============================================

CREATE procedure [dbo].[uspfn_SelectInventory4All]
	@CompanyCode varchar(20),
	@BranchCode  varchar(20),
	@Status		varchar(50),
	@StartDate	smalldatetime,
	@EndDate	smalldatetime
as 

select convert(bit, (case Status when '5' then '1' else '0' end)) as IsSelected
     , case isnull(Status, 0) when '5' then 'Posted' else 'UnPosted' end as Status
	 , CompanyCode
	 , BranchCode
	 , 'TRANSFEROUT' as TypeJournal
	 , TransferOutNo as DocNo
	 , TransferOutDate as DocDate
	 , ReferenceNo as ReffNo
	 , BranchCodeFrom + '-' + WareHouseCodeFrom as BranchCodeFrom
	 , BranchCodeTo + '-' + WareHouseCodeTo as BranchCodeTo
  from omTrInventTransferOut
 where 1 = 1
   and CompanyCode = @CompanyCode
   and BranchCode  = @BranchCode
   and Status      = @Status
   and BranchCodeFrom <> BranchCodeTo
   and TransferOutDate between @StartDate and @EndDate
UNION
select convert(bit, (case Status when '5' then '1' else '0' end)) as IsSelected
     , case isnull(Status, 0) when '5' then 'Posted' else 'UnPosted' end as Status
	 , CompanyCode
	 , BranchCode
	 , 'TRANSFERIN' as TypeJournal
	 , TransferInNo as DocNo
	 , TransferInDate as DocDate
	 , ReferenceNo as ReffNo
	 , BranchCodeFrom + '-' + WareHouseCodeFrom as BranchCodeFrom
	 , BranchCodeTo + '-' + WareHouseCodeTo as BranchCodeTo
  from omTrInventTransferIn
 where 1 = 1
   and CompanyCode = @CompanyCode
   and BranchCode  = @BranchCode
   and Status      = @Status
   and BranchCodeFrom <> BranchCodeTo
   and TransferInDate between @StartDate and @EndDate
UNION
select convert(bit, (case Status when '5' then '1' else '0' end)) as IsSelected
     , case isnull(Status, 0) when '5' then 'Posted' else 'UnPosted' end as Status
	 , CompanyCode
	 , BranchCode
	 , 'TRANSFEROUTMULTIBRANCH' as TypeJournal
	 , TransferOutNo as DocNo
	 , TransferOutDate as DocDate
	 , ReferenceNo as ReffNo
	 , BranchCodeFrom + '-' + WareHouseCodeFrom as BranchCodeFrom
	 , BranchCodeTo + '-' + WareHouseCodeTo as BranchCodeTo
  from omTrInventTransferOutMultiBranch
 where 1 = 1
   and CompanyCode = @CompanyCode
   and BranchCode  = @BranchCode
   and Status      = @Status
   and BranchCodeFrom <> BranchCodeTo
   and convert(varchar, TransferOutDate, 112) between @StartDate and @EndDate
UNION
select convert(bit, (case Status when '5' then '1' else '0' end)) as IsSelected
     , case isnull(Status, 0) when '5' then 'Posted' else 'UnPosted' end as Status
	 , CompanyCode
	 , BranchCode
	 , 'TRANSFERINMULTIBRANCH' as TypeJournal
	 , TransferInNo as DocNo
	 , TransferInDate as DocDate
	 , ReferenceNo as ReffNo
	 , BranchCodeFrom + '-' + WareHouseCodeFrom as BranchCodeFrom
	 , BranchCodeTo + '-' + WareHouseCodeTo as BranchCodeTo
  from omTrInventTransferInMultiBranch
 where 1 = 1
   and CompanyCode = @CompanyCode
   and BranchCode  = @BranchCode
   and Status      = @Status
   and BranchCodeFrom <> BranchCodeTo
   and TransferInDate between @StartDate and @EndDate
GO

if object_id('uspfn_SelectLMPDtl') is not null
	drop procedure uspfn_SelectLMPDtl
GO
CREATE procedure [dbo].[uspfn_SelectLMPDtl] @CompanyCode varchar(15), @BranchCode varchar(15), @LmpNo varchar(15)  
as  
SELECT     
	row_number () OVER (ORDER BY spTrnSLmpDtl.CreatedDate ASC) AS NoUrut,
	spTrnSLmpDtl.PartNo,
	spTrnSLmpDtl.PartNoOriginal,
	spTrnSLmpDtl.DocNo, 
	CONVERT(VARCHAR, spTrnSLmpDtl.DocDate, 106) AS DocDate, 
	spTrnSLmpDtl.ReferenceNo, 
	spTrnSLmpDtl.QtyBill
	FROM spTrnSLmpDtl
	WHERE
	spTrnSLmpDtl.CompanyCode = @CompanyCode AND
	spTrnSLmpDtl.BranchCode = @BranchCode AND
	spTrnSLmpDtl.LmpNo = @LmpNo
GO

if object_id('uspfn_spAutomaticOrderSparepart') is not null
	drop procedure uspfn_spAutomaticOrderSparepart
GO
---------------------------------------------------------
-- AUTOMATIC ORDER SPAREPART, by Hasim... 24 January 2014
---------------------------------------------------------
-- uspfn_spAutomaticOrderSparepart '6006406','6006406'
--
CREATE procedure [dbo].[uspfn_spAutomaticOrderSparepart]
	@CompanyCode		varchar(15),
	@BranchCode			varchar(15)
as

BEGIN
  --declare @CompanyCode		varchar(15)
  --declare @BranchCode			varchar(15)
	declare @PartNo				varchar(20)
	declare @NewPartNo			varchar(20)
	declare @OldPart			varchar(20)
	declare @NewPart			varchar(20)
	declare @StartDate			varchar(06)
	declare @EndDate			varchar(06)
	declare @SupplierCode 		varchar(15)
	declare @SupplierCode0		varchar(15)
	declare @SupplierCode1		varchar(15)
	declare @SupplierCode2		varchar(15)
	declare @TPGO				varchar(15)
	declare @SuggorNo			varchar(15)
	declare @POSNo				varchar(15)
	declare @DocPre				varchar(15)
	declare @AOS1               varchar(50)
	declare @AOS2               varchar(50)
	declare @MessageText        varchar(50)
	declare @DocNum				integer
	declare @DocYear			integer
	declare @Counter     		integer
	declare @Switch      		integer
	declare @PeriodYear  		integer
	declare @PeriodMonth		integer
	declare @PeriodDate			date
	declare @SuggorDate			datetime
	declare @SeqNo              numeric( 3,0)
	declare @DAvgFac            numeric(07,2)
	declare @DevFac             numeric(07,2)
	declare @DiscPct			numeric(07,2)
	declare @PurchasePriceNett	numeric(18,0)
  --set @CompanyCode = '6006406'
  --set @BranchCode  = '6006406'
	set @SuggorDate  = getdate()
	set @PeriodYear  = year(getdate())
	set @PeriodMonth = month(getdate())
	set @Counter     = 0
	set @PeriodDate  = RIGHT('0000'+convert(varchar(4),@PeriodYear ),4) + '/'
                     + RIGHT('00'+convert(varchar(2),@PeriodMonth),2) + '/01'

    if isnull((select ParaValue from gnMstLookUpDtl
                where CompanyCode=@CompanyCode and CodeID='AOS' and LookUpValue='AUTO'),'0') = 0
       return

    if isnull((select ParaValue from gnMstLookUpDtl 
                where CompanyCode=@CompanyCode and CodeID='ORTP' and LookUpValue='8'),'')=''
       begin
          set @SeqNo = isnull((select max(SeqNo) from gnMstLookUpDtl where CompanyCode=@CompanyCode and CodeID='ORTP'),0) + 1
          insert into gnMstLookUpDtl
                     (CompanyCode, CodeID, LookUpValue, SeqNo, ParaValue, LookUpValueName, 
                      CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
               values(@CompanyCode, 'ORTP', '8', @SeqNo, '0', 'AOS (AUTOMATIC ORDER SPAREPART)',
                      'SIM',@SuggorDate,'SIM',@SuggorDate)
       end
                  
	if isnull((select top 1 1 from spTrnPSUGGORHdr
                where CompanyCode=@CompanyCode and BranchCode=@BranchCode and OrderType='8'
                  and convert(varchar,SuggorDate,111)=convert(varchar,@SuggorDate,111)),0) = 1
       return
	   
    set @DocYear     = (select isnull(DocumentYear,9988) from gnMstDocument with(nolock,nowait)
                         where CompanyCode=@CompanyCode and BranchCode=@BranchCode and DocumentType='SGR')

    if @DocYear<>YEAR(@SuggorDate) return

    set @SupplierCode0 = isnull((select ParaValue from gnMstLookupDtl
                                  where CompanyCode = @CompanyCode
	 							    and CodeID      = 'AOS'
	 							    and LookupValue = '0'),'')
    set @SupplierCode1 = isnull((select ParaValue from gnMstLookupDtl
                                  where CompanyCode = @CompanyCode
 								    and CodeID      = 'AOS'
 								    and LookupValue = '1'),'')
    set @SupplierCode2 = isnull((select ParaValue from gnMstLookupDtl
                                  where CompanyCode = @CompanyCode
 								    and CodeID      = 'AOS'
 								    and LookupValue = '2'),'')
    set @DAvgFac       = isnull((select convert(numeric(6,2),ParaValue) from gnMstLookupDtl
                                  where CompanyCode = @CompanyCode
      							    and CodeID      = 'AOS'
    								and LookupValue = 'DAVGFAC'),50.00)
    set @DevFac        = isnull((select convert(numeric(6,2),ParaValue) from gnMstLookupDtl
                                  where CompanyCode = @CompanyCode
    							    and CodeID      = 'AOS'
    								and LookupValue = 'DevFac'),0.60)
  --set @DAvgFac       = 50.00
  --set @DevFac        = 0.60

    create table #Suggor 
 	    ( [CompanyCode]		[varchar](15  ),
	      [BranchCode]    	[varchar](15  ),
	      [PartNo]        	[varchar](20  ),
	      [NewPartNo]     	[varchar](20  ),
	      [SupplierCode]  	[varchar](20  ),
	      [ProductType]   	[varchar](15  ),
	      [PartCategory]  	[varchar]( 3  ),
	      [TypeOfGoods]   	[varchar]( 5  ),
	      [MovingCode]    	[varchar](15  ),
	      [ABCClass]      	[char]   ( 1  ),
	      [OnHand]        	[numeric](12,2),
	      [OnOrder]       	[numeric](12,2),
	      [InTransit]     	[numeric](12,2),
	      [AllocationSP]  	[numeric](12,2),
	      [AllocationSR]  	[numeric](12,2),
	      [AllocationSL]  	[numeric](12,2),
	      [BackOrderSP]   	[numeric](12,2),
	      [BackOrderSR]   	[numeric](12,2),
	      [BackOrderSL]   	[numeric](12,2),
	      [ReservedSP]    	[numeric](12,2),
	      [ReservedSR]    	[numeric](12,2),
	      [ReservedSL]    	[numeric](12,2),
	      [DemandAvg]     	[numeric](15,5),
	      [OrderPoint]    	[numeric](12,0),
	      [SafetyStock]   	[numeric](12,0),
	      [AvailableQty]  	[numeric](12,2),
	      [OrderUnit]     	[numeric](12,2),
	      [PurchasePrice] 	[numeric](18,0),
	      [CostPrice]     	[numeric](18,0)
        )

    insert into #Suggor
              ( CompanyCode, BranchCode, PartNo, NewPartNo, SupplierCode, ProductType, PartCategory, 
	 		    TypeOfGoods, MovingCode, ABCClass, OnHand, OnOrder, InTransit, AllocationSP, AllocationSR, 
	 		    AllocationSL, BackOrderSP, BackOrderSR, BackOrderSL, ReservedSP, ReservedSR, ReservedSL, 
	 		    DemandAvg, OrderPoint, SafetyStock, AvailableQty, OrderUnit, PurchasePrice, CostPrice)
         select i.CompanyCode, i.BranchCode, i.PartNo, i.PartNo NewPartNo, f.SupplierCode, i.ProductType, 
		        i.PartCategory, i.TypeOfGoods, i.MovingCode, i.ABCClass, i.OnHand, i.OnOrder, i.InTransit, 
			    i.AllocationSP, i.AllocationSR, i.AllocationSL, i.BackOrderSP, i.BackOrderSR, i.BackOrderSL, 
			    i.ReservedSP, i.ReservedSR, i.ReservedSL, i.DemandAverage DemandAvg, i.OrderPointQty OrderPoint, 
			    i.SafetyStockQty SafetyStock,(i.OnHand-i.AllocationSP-i.AllocationSR-i.AllocationSL) AvailableQty, 
			    OrderUnit = case when isnull(i.OrderUnit,0.00)=0.00 then 1.00 else i.OrderUnit end, 
			    p.PurchasePrice, p.CostPrice
           from spMstItems i
                inner join spMstItemInfo f
			   	        on f.CompanyCode = i.CompanyCode
		               and f.PartNo      = i.PartNo
				       and f.Status      = 1
				       and f.SupplierCode in (@SupplierCode0, @SupplierCode1, @SupplierCode2)
	            inner join spMstItemPrice p
				        on p.CompanyCode = i.CompanyCode
				       and p.BranchCode  = i.BranchCode
		               and p.PartNo      = i.PartNo 
		  where i.CompanyCode = @CompanyCode
		    and i.BranchCode  = @BranchCode
		    and i.Status      = 1
		    and i.TypeOfGoods in ('0','1','2') -- 0:SGP, 1:SGO, 2:SGA

    create table #t1 
	    ( [CompanyCode] 	[varchar](15  ),
	      [BranchCode]  	[varchar](15  ),
	      [PartNo]      	[varchar](20  ),
	      [NewPartNo]   	[varchar](20  ),
	      [Year]        	[numeric]( 4,0),
	      [Month]       	[numeric]( 2,0),
	      [DemandQty]     	[numeric](18,2)
        )

    while @Counter < 6
      begin
         set @Counter = @Counter + 1
         if @PeriodMonth > 1
            begin
               set @PeriodYear  = @PeriodYear
               set @PeriodMonth = @PeriodMonth-1
            end
         else 
            begin
               set @PeriodYear  = @PeriodYear-1
               set @PeriodMonth = 12
            end

         insert into #t1
              select CompanyCode, BranchCode, PartNo, PartNo NewPartNo, 
                     @PeriodYear Year, @PeriodMonth Month, 
                     isnull((select DemandQty from spHstDemandItem
                              where CompanyCode = #Suggor.CompanyCode
                                and BranchCode  = #Suggor.BranchCode
                                and Year        = @PeriodYear
                                and Month       = @PeriodMonth
                                and PartNo      = #Suggor.PartNo),0.00) as DemandQty
                from #Suggor     -- spMstItems
               where CompanyCode = @CompanyCode
                 and BranchCode  = @BranchCode
      end
      
    declare ITEM   cursor for
            select CompanyCode, BranchCode, PartNo, NewPartNo from #t1
             where exists (select top 1 1 from spMstItemMod
                            where CompanyCode = #t1.CompanyCode
                              and BranchCode  = #t1.BranchCode
                              and PartNo      = #t1.PartNo
                              and NewPartNo  <> #t1.PartNo
                              and InterChangeCode in ('11','21'))
             order by PartNo
    open  ITEM
    fetch next from ITEM into @CompanyCode, @BranchCode, @PartNo, @NewPartNo

    while @@fetch_status=0
      begin
         set @OldPart  = @PartNo
         set @Switch   = 0
         while @Switch = 0
            begin
               set @NewPart = (select top 1 NewPartNo from spMstItemMod x
                                where PartNo=@OldPart and InterChangeCode in ('11','21')
                                  and not exists (select 1 from spMstItemMod
                                                   where CompanyCode=x.CompanyCode
                                                     and PartNo=x.NewPartNo
                                                     and NewPartNo=x.PartNo))
               if  @NewPart is NULL
                   begin
                      set @Switch = 1
                      set @NewPart = @OldPart
                   end
               else
                      set @OldPart = @NewPart
            end
         update #t1     set NewPartNo=@NewPart where PartNo=@PartNo
         update #Suggor set NewPartNo=@NewPart where PartNo=@PartNo
         fetch next from ITEM into @CompanyCode, @BranchCode, @PartNo, @NewPartNo
      end
    close ITEM
    deallocate ITEM

    select * into #t2
      from ( select CompanyCode, BranchCode, NewPartNo, Year, Month, sum(DemandQty) DemandQty
               from #t1 
              group by CompanyCode, BranchCode, NewPartNo, Year, Month) #t2

    select * into #t3
      from ( select CompanyCode, BranchCode, NewPartNo, 
                    sum  (isnull(DemandQty,0.0))    DmnQty, 
                   (sum  (isnull(DemandQty,0.0)))/6 DmnAvg, 
                    stdev(isnull(DemandQty,0.0))    StdDev,
                    case when sum  (isnull(DemandQty,0.0)) = 0.0
                         then 0.0
                         else round((stdev(isnull(DemandQty,0.0)) / 
                                   ((sum  (isnull(DemandQty,0.0)))/6)),2)
                    end as DevFac,
                    max  (isnull(DemandQty,0.0))    MaxQty,
                    min  (isnull(DemandQty,0.0))    MinQty
               from #t2
              group by CompanyCode, BranchCode, NewPartNo ) #t3

    select * into #t4
      from ( select #t3.CompanyCode, #t3.BranchCode, #t3.NewPartNo, #Suggor.MovingCode, 
                    #Suggor.ABCClass, #Suggor.ProductType, #Suggor.PartCategory, 
                    #Suggor.TypeOfGoods, #Suggor.SupplierCode, 
                    #Suggor.CostPrice, #Suggor.PurchasePrice,
                    DmnQty6      = (select sum(isnull(DemandQty,0.00)) from #t2
                                     where CompanyCode=#t3.CompanyCode
                                       and BranchCode =#t3.BranchCode
                                       and Year       =year (dateadd(mm,-6,@PeriodDate))
                                       and Month      =month(dateadd(mm,-6,@PeriodDate))
                                       and NewPartNo  =#t3.NewPartNo),
                    DmnQty5      = (select sum(isnull(DemandQty,0.00)) from #t2
                                     where CompanyCode=#t3.CompanyCode
                                       and BranchCode =#t3.BranchCode
                                       and Year       =year (dateadd(mm,-5,@PeriodDate))
                                       and Month      =month(dateadd(mm,-5,@PeriodDate))
                                       and NewPartNo  =#t3.NewPartNo),
                    DmnQty4      = (select sum(isnull(DemandQty,0.00)) from #t2
                                     where CompanyCode=#t3.CompanyCode
                                       and BranchCode =#t3.BranchCode
                                       and Year       =year (dateadd(mm,-4,@PeriodDate))
                                       and Month      =month(dateadd(mm,-4,@PeriodDate))
                                       and NewPartNo  =#t3.NewPartNo),
                    DmnQty3      = (select sum(isnull(DemandQty,0.00)) from #t2
                                     where CompanyCode=#t3.CompanyCode
                                       and BranchCode =#t3.BranchCode
                                       and Year       =year (dateadd(mm,-3,@PeriodDate))
                                       and Month      =month(dateadd(mm,-3,@PeriodDate))
                                       and NewPartNo  =#t3.NewPartNo),
                    DmnQty2      = (select sum(isnull(DemandQty,0.00)) from #t2
                                     where CompanyCode=#t3.CompanyCode
                                       and BranchCode =#t3.BranchCode
                                       and Year       =year (dateadd(mm,-2,@PeriodDate))
                                       and Month      =month(dateadd(mm,-2,@PeriodDate))
                                       and NewPartNo  =#t3.NewPartNo),
                    DmnQty1      = (select sum(isnull(DemandQty,0.00)) from #t2
                                     where CompanyCode=#t3.CompanyCode
                                       and BranchCode =#t3.BranchCode
                                       and Year       =year (dateadd(mm,-1,@PeriodDate))
                                       and Month      =month(dateadd(mm,-1,@PeriodDate))
                                       and NewPartNo  =#t3.NewPartNo),
                    OnHand       = (select sum(isnull(OnHand,0.00)) from #Suggor
                                     where CompanyCode=#t3.CompanyCode
                                       and BranchCode =#t3.BranchCode
                                       and NewPartNo  =#t3.NewPartNo),
                    OnOrder      = (select sum(isnull(OnOrder,0.00)) from #Suggor
                                     where CompanyCode=#t3.CompanyCode
                                       and BranchCode =#t3.BranchCode
                                       and NewPartNo  =#t3.NewPartNo),
                    InTransit    = (select sum(isnull(InTransit,0.00)) from #Suggor
                                     where CompanyCode=#t3.CompanyCode
                                       and BranchCode =#t3.BranchCode
                                       and NewPartNo  =#t3.NewPartNo),
                    AllocationSP = (select sum(isnull(AllocationSP,0.00)) from #Suggor
                                     where CompanyCode=#t3.CompanyCode
                                       and BranchCode =#t3.BranchCode
                                       and NewPartNo  =#t3.NewPartNo),                                
                    AllocationSR = (select sum(isnull(AllocationSR,0.00)) from #Suggor
                                     where CompanyCode=#t3.CompanyCode
                                       and BranchCode =#t3.BranchCode
                                       and NewPartNo  =#t3.NewPartNo),
                    AllocationSL = (select sum(isnull(AllocationSL,0.00)) from #Suggor
                                     where CompanyCode=#t3.CompanyCode
                                       and BranchCode =#t3.BranchCode
                                       and NewPartNo  =#t3.NewPartNo),
                    BackOrderSP  = (select sum(isnull(BackOrderSP,0.00)) from #Suggor
                                     where CompanyCode=#t3.CompanyCode
                                       and BranchCode =#t3.BranchCode
                                       and NewPartNo  =#t3.NewPartNo),
                    BackOrderSR  = (select sum(isnull(BackOrderSR,0.00)) from #Suggor
                                     where CompanyCode=#t3.CompanyCode
                                       and BranchCode =#t3.BranchCode
                                       and NewPartNo  =#t3.NewPartNo),
                    BackOrderSL  = (select sum(isnull(BackOrderSL,0.00)) from #Suggor
                                     where CompanyCode=#t3.CompanyCode
                                       and BranchCode =#t3.BranchCode
                                       and NewPartNo  =#t3.NewPartNo),
                    ReservedSP   = (select sum(isnull(ReservedSP,0.00)) from #Suggor
                                     where CompanyCode=#t3.CompanyCode
                                       and BranchCode =#t3.BranchCode
                                       and NewPartNo  =#t3.NewPartNo),
                    ReservedSR   = (select sum(isnull(ReservedSR,0.00)) from #Suggor
                                     where CompanyCode=#t3.CompanyCode
                                       and BranchCode =#t3.BranchCode
                                       and NewPartNo  =#t3.NewPartNo),
                    ReservedSL   = (select sum(isnull(ReservedSL,0.00)) from #Suggor
                                     where CompanyCode=#t3.CompanyCode
                                       and BranchCode =#t3.BranchCode
                                       and NewPartNo  =#t3.NewPartNo),
                    999999999999.99 AvailableQty, #Suggor.OrderUnit, 
                    DmnQty, DmnAvg, StdDev, DevFac, MaxQty, MinQty,
                    LeadTime     = (select isnull(LeadTime,0.00) from spMstOrderParam
                                     where CompanyCode  = #t3.CompanyCode
                                       and BranchCode   = #t3.BranchCode
                                       and SupplierCode = case #Suggor.TypeOfGoods
                                                               when '0' then @SupplierCode0
                                                               when '1' then @SupplierCode1
                                                               when '2' then @SupplierCode2
                                                               else '***NO-ORDER***'
                                                          end
                                       and MovingCode   = 1),
                    OrderCycle   = (select isnull(OrderCycle,0.00) from spMstOrderParam
                                     where CompanyCode  = #t3.CompanyCode
                                       and BranchCode   = #t3.BranchCode
                                       and SupplierCode = case #Suggor.TypeOfGoods
                                                               when '0' then @SupplierCode0
                                                               when '1' then @SupplierCode1
                                                               when '2' then @SupplierCode2
                                                               else '***NO-ORDER***'
                                                          end
                                       and MovingCode   = 1),
                    SafetyStock  = (select isnull(SafetyStock,0.00) from spMstOrderParam
                                     where CompanyCode  = #t3.CompanyCode
                                       and BranchCode   = #t3.BranchCode
                                       and SupplierCode = case #Suggor.TypeOfGoods
                                                               when '0' then @SupplierCode0
                                                               when '1' then @SupplierCode1
                                                               when '2' then @SupplierCode2
                                                               else '***NO-ORDER***'
                                                          end
                                       and MovingCode   = 1),
                    999999999999.99 OrderPoint, 999999999999.99 SafetyStokPoint, 
                    999999999999.99 SuggorQty, 0 Status
               from #t3, #Suggor
              where #t3.CompanyCode=#Suggor.CompanyCode
                and #t3.BranchCode =#Suggor.BranchCode
                and #t3.NewPartNo  =#Suggor.PartNo
                and #Suggor.TypeOfGoods in ('0','1','2')) #t4 -- 0:SGP, 1:SGO, 2:SGA

    update #t4
       set AvailableQty    = (OnHand+OnOrder+InTransit)
                           - (AllocationSP+AllocationSR+AllocationSL
                           +  BackOrderSP +BackOrderSR +BackOrderSL
                           +  ReservedSP  +ReservedSR  +ReservedSL),
           OrderPoint      = ceiling(DmnAvg/30 * isnull((LeadTime+OrderCycle+SafetyStock),0.00)),
           SafetyStokPoint = ceiling(DmnAvg/30 * isnull(SafetyStock,0.00)),
           Status          = 1
    --where DmnAvg>=50.0 and DevFac<=0.6
      where DmnAvg>=@DAvgFac and DevFac<=@DevFac

    update #t4 
       set SuggorQty       = case when AvailableQty>0.00 and OrderPoint>AvailableQty 
                                  then ceiling((OrderPoint-AvailableQty)/OrderUnit) * OrderUnit
                                  else 0.00 
                             end,
           Status          = case when AvailableQty>0.00 and OrderPoint>AvailableQty 
                                  then 1 
                                  else 0 
                             end
     where Status = 1 

    if isnull((select COUNT(*) from #t4 where Status=1),0) = 0
       begin
           drop table #Suggor
	  	   drop table #t1
           drop table #t2
           drop table #t3
           drop table #t4
           return
       end
    select * from #t4 where Status=1 order by TypeOfGoods, SupplierCode, NewPartNo
      
  --insert to SUGGOR table   
    set @TPGO = ''
    set @AOS1 = ''
    set @AOS2 = ''

    declare @cur_NewPartNo       varchar(20)
    declare @cur_MovingCode      varchar(15)
    declare @cur_ABCClass        varchar(01)
    declare @cur_ProductType		varchar(15)
    declare @cur_PartCategory	varchar(03)
    declare @cur_TypeOfGoods		varchar(05)
    declare @cur_SupplierCode    varchar(20)
    declare @cur_CostPrice		numeric(18,2)
    declare @cur_PurchasePrice	numeric(18,2)
    declare @cur_OnHand          numeric(12,2)
    declare @cur_OnOrder         numeric(12,2)
    declare @cur_InTransit       numeric(12,2)
    declare @cur_AllocationSP    numeric(12,2)
    declare @cur_AllocationSR    numeric(12,2)
    declare @cur_AllocationSL    numeric(12,2)
    declare @cur_BackOrderSP     numeric(12,2)
    declare @cur_BackOrderSR     numeric(12,2)
    declare @cur_BackOrderSL     numeric(12,2)
    declare @cur_ReservedSP      numeric(12,2)
    declare @cur_ReservedSR      numeric(12,2)
    declare @cur_ReservedSL      numeric(12,2)
    declare @cur_AvailableQty    numeric(12,2)
    declare @cur_DmnAvg          numeric(15,5)
    declare @cur_OrderPoint      numeric(12,0)
    declare @cur_SafetyStokPoint numeric(12,0)
    declare @cur_SuggorQty       numeric(12,0)

    declare SUGGOR cursor for
            select NewPartNo, MovingCode, ABCClass, ProductType, PartCategory, TypeOfGoods, SupplierCode, 
                   CostPrice, PurchasePrice, OnHand, OnOrder, InTransit, AllocationSP, AllocationSR, 
                   AllocationSL, BackOrderSP, BackOrderSR, BackOrderSL, ReservedSP, ReservedSR, 
                   ReservedSL, AvailableQty, DmnAvg, OrderPoint, SafetyStokPoint, SuggorQty
              from #t4 where Status=1 order by TypeOfGoods, SupplierCode, NewPartNo
    open  SUGGOR
    fetch next from SUGGOR 
               into @cur_NewPartNo, @cur_MovingCode, @cur_ABCClass, @cur_ProductType, @cur_PartCategory, 
                    @cur_TypeOfGoods, @cur_SupplierCode, @cur_CostPrice, @cur_PurchasePrice, @cur_OnHand, 
                    @cur_OnOrder, @cur_InTransit, @cur_AllocationSP, @cur_AllocationSR, @cur_AllocationSL, 
                    @cur_BackOrderSP, @cur_BackOrderSR, @cur_BackOrderSL, @cur_ReservedSP, @cur_ReservedSR, 
                    @cur_ReservedSL, @cur_AvailableQty, @cur_DmnAvg, @cur_OrderPoint, @cur_SafetyStokPoint, 
                    @cur_SuggorQty

    while @@fetch_status=0
      begin
         if @TPGO<>@cur_TypeOfGoods or @SupplierCode<>@cur_SupplierCode
            begin
                set @Counter      = 0
                set @TPGO         = @cur_TypeOfGoods
                set @SupplierCode = @cur_SupplierCode
                -- setup nomor document for SUGGOR
                   set @DocPre    = (select isnull(DocumentPrefix,'XYZ') from gnMstDocument
                                      where CompanyCode=@CompanyCode and BranchCode=@BranchCode and DocumentType='SGR')
                   set @DocNum    = (select isnull(DocumentSequence,999888) from gnMstDocument
                                      where CompanyCode=@CompanyCode and BranchCode=@BranchCode and DocumentType='SGR')
                   if  @DocNum    = 999888  return
                   set @DocNum    = @DocNum + 1
                   update gnMstDocument set DocumentSequence = @DocNum
                    where CompanyCode=@CompanyCode and BranchCode=@BranchCode and DocumentType='SGR'
                   set @SuggorNo  = @DocPre + '/' + right('00'+right(convert(varchar(4),year(@SuggorDate)),2),2)
                                            + '/' + right('000000'+(convert(varchar(6),@DocNum,6)),6)
                 --set @AOS1      = @AOS1 + case when @AOS1='' then @SuggorNo else ', ' + @SuggorNo end
                   set @AOS1      = @AOS1 + case when @AOS1='' then @SuggorNo else ', ' + right(@SuggorNo,6) end

                -- setup nomor document POS
                   set @DocPre    = (select isnull(DocumentPrefix,'XYZ') from gnMstDocument
                                      where CompanyCode=@CompanyCode and BranchCode=@BranchCode and DocumentType='POS')
                   set @DocNum    = (select isnull(DocumentSequence,999888) from gnMstDocument
                                      where CompanyCode=@CompanyCode and BranchCode=@BranchCode and DocumentType='POS')
                   if  @DocNum    = 999888  return
                   set @DocNum    = @DocNum + 1
                   update gnMstDocument set DocumentSequence = @DocNum
                    where CompanyCode=@CompanyCode and BranchCode=@BranchCode and DocumentType='POS'
                   set @POSNo     = @DocPre + '/' + right('00'+right(convert(varchar(4),year(@SuggorDate)),2),2)
                                            + '/' + right('000000'+(convert(varchar(6),@DocNum,6)),6)
                 --set @AOS2      = @AOS2 + case when @AOS2='' then @POSNo else ', ' + @POSNo end
                   set @AOS2      = @AOS2 + case when @AOS2='' then @POSNo else ', ' + right(@POSNo,6) end

                -- insert Suggor Header table
                   insert into spTrnPSUGGORHdr
                              (CompanyCode, BranchCode, SuggorNo, SuggorDate, TypeOfGoods, POSNo, 
                               POSDate, SupplierCode, ProductType, MovingCode, OrderType, Status, 
                               PrintSeq, IsVoid, CreatedBy, CreatedDate, LastUpdateBy, 
                               LastUpdateDate, isLocked, LockingBy, LockingDate)
                        values(@CompanyCode, @BranchCode, @SuggorNo, @SuggorDate, @TPGO, @POSNo, 
                               @SuggorDate, @SupplierCode, @cur_ProductType, 1, '8', 1, 0, 0,  -- OrderType = 8 - AOS
                               'AOS', @SuggorDate, 'AOS', @SuggorDate, '0', NULL, NULL)

                -- insert POS Header table
                   insert into spTrnPPOSHdr
                              (CompanyCode, BranchCode, POSNo, POSDate, SupplierCode, OrderType, IsBO, 
                               IsSubstution, isSuggorProcess, Remark, ProductType, PrintSeq, ExPickingSlipNo, 
                               ExPickingSlipDate, Status, Transportation, TypeOfGoods, isGenPORDD, isDeleted, 
                               CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate, isLocked, LockingBy, 
                               LockingDate, isDropSign, DropSignReffNo)
                        values(@CompanyCode, @BranchCode, @POSNo, @SuggorDate, @SupplierCode, '8', 1, 
                               1, 1, 'MACHINE ORDER', @cur_ProductType, 1, NULL, NULL, 2, 'AOS', @TPGO, 
                               0, 0, 'AOS', @SuggorDate, 'AOS', @SuggorDate, 0, NULL, NULL, 0, NULL)
            end

         -- insert Suggor Detail table
            set @Counter = @Counter + 1
            insert into spTrnPSUGGORDtl
                       (CompanyCode, BranchCode, SuggorNo, PartNo, SeqNo, OnHand, OnOrder, InTransit, 
                        AllocationSP, AllocationSR, AllocationSL, BackOrderSP, BackOrderSR, BackOrderSL, 
                        ReservedSP, ReservedSR, ReservedSL, DemandAvg, OrderPoint, SafetyStock, AvailableQty, 
                        SuggorQty, SuggorCorrecQty, ProductType, PartCategory, PurchasePrice, CostPrice, 
                        isExistInItems, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
                 values(@CompanyCode, @BranchCode, @SuggorNo, @cur_NewPartNo, @Counter, @cur_OnHand, 
                        @cur_OnOrder, @cur_InTransit, @cur_AllocationSP, @cur_AllocationSR, @cur_AllocationSL, 
                        @cur_BackOrderSP, @cur_BackOrderSR, @cur_BackOrderSL, @cur_ReservedSP, 
                        @cur_ReservedSR, @cur_ReservedSL, @cur_DmnAvg, @cur_OrderPoint, 
                        @cur_SafetyStokPoint, @cur_AvailableQty, @cur_SuggorQty, @cur_SuggorQty,
                        @cur_ProductType, @cur_PartCategory, @cur_PurchasePrice, @cur_CostPrice, 
                        1, 'AOS', @SuggorDate, 'AOS', @SuggorDate)

         -- insert POS Detail table
            set @DiscPct = (select isnull(DiscPct,0.00) from gnMstSupplierProfitCenter 
                             where CompanyCode      = @CompanyCode
                               and BranchCode       = @BranchCode 
                               and SupplierCode     = @cur_SupplierCode 
                               and ProfitCenterCode = '300')
            set @PurchasePriceNett = floor(@cur_PurchasePrice * (100.00 - @DiscPct) / 100)
            insert into spTrnPPOSDtl
                       (CompanyCode, BranchCode, POSNo, PartNo, SeqNo, OrderQty, SuggorQty, 
                        PurchasePrice, DiscPct, PurchasePriceNett, CostPrice, TotalAmount, 
                        ABCClass, MovingCode, ProductType, PartCategory, CreatedBy, 
                        CreatedDate, LastUpdateBy, LastUpdateDate, Note)
                 values(@CompanyCode, @BranchCode, @POSNo, @cur_NewPartNo, @Counter, 
                        @cur_SuggorQty, @cur_SuggorQty, @cur_PurchasePrice, @DiscPct, 
                        @PurchasePriceNett, @cur_CostPrice, @cur_SuggorQty * @PurchasePriceNett,
                        @cur_ABCClass, @cur_MovingCode, @cur_ProductType, @cur_PartCategory, 
                        'AOS', @SuggorDate, 'AOS', @SuggorDate, 'AOS')

         -- insert Suggor Sub-detail table
            insert into spTrnPSUGGORSubDtl
                 select @CompanyCode, @BranchCode, @SuggorNo, @cur_NewPartNo, PartNo,
                        ROW_NUMBER() over (order by PartNo) as row,
                        I   = (select DemandQty from #t1
                                where CompanyCode = @CompanyCode
                                  and BranchCode  = @BranchCode
                                  and Year        = year (dateadd(mm,-6,@PeriodDate))
                                  and Month       = month(dateadd(mm,-6,@PeriodDate))
                                  and NewPartNo   = @cur_NewPartNo
                                  and PartNo      = #Suggor.PartNo),
                        II  = (select DemandQty from #t1
                                where CompanyCode = @CompanyCode
                                  and BranchCode  = @BranchCode
                                  and Year        = year (dateadd(mm,-5,@PeriodDate))
                                  and Month       = month(dateadd(mm,-5,@PeriodDate))
                                  and NewPartNo   = @cur_NewPartNo
                                  and PartNo      = #Suggor.PartNo),
                        III = (select DemandQty from #t1
                                where CompanyCode = @CompanyCode
                                  and BranchCode  = @BranchCode
                                  and Year        = year (dateadd(mm,-4,@PeriodDate))
                                  and Month       = month(dateadd(mm,-4,@PeriodDate))
                                  and NewPartNo   = @cur_NewPartNo
                                  and PartNo      = #Suggor.PartNo),
                        IV  = (select DemandQty from #t1
                                where CompanyCode = @CompanyCode
                                  and BranchCode  = @BranchCode
                                  and Year        = year (dateadd(mm,-3,@PeriodDate))
                                  and Month       = month(dateadd(mm,-3,@PeriodDate))
                                  and NewPartNo   = @cur_NewPartNo
                                  and PartNo      = #Suggor.PartNo),
                        V   = (select DemandQty from #t1
                                where CompanyCode = @CompanyCode
                                  and BranchCode  = @BranchCode
                                  and Year        = year (dateadd(mm,-2,@PeriodDate))
                                  and Month       = month(dateadd(mm,-2,@PeriodDate))
                                  and NewPartNo   = @cur_NewPartNo
                                  and PartNo      = #Suggor.PartNo),
                        VI  = (select DemandQty from #t1
                                where CompanyCode = @CompanyCode
                                  and BranchCode  = @BranchCode
                                  and Year        = year (dateadd(mm,-1,@PeriodDate))
                                  and Month       = month(dateadd(mm,-1,@PeriodDate))
                                  and NewPartNo   = @cur_NewPartNo
                                  and PartNo      = #Suggor.PartNo),
                        'AOS',@SuggorDate
                   from #Suggor
                  where CompanyCode = @CompanyCode
                    and BranchCode  = @BranchCode
                    and NewPartNo   = @cur_NewPartNo
                    
         -- insert order balance table
            insert into spTrnPOrderBalance
                       (CompanyCode, BranchCode, POSNo, SupplierCode, PartNo, SeqNo, PartNoOriginal, 
                        POSDate, OrderQty, OnOrder, InTransit, Received, Located, DiscPct, 
                        PurchasePrice, CostPrice, ABCClass, MovingCode, WRSNo, WRSDate, TypeOfGoods, 
                        CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
                 values(@CompanyCode, @BranchCode, @POSNo, @cur_SupplierCode, @cur_NewPartNo, @Counter, 
                        @cur_NewPartNo, @SuggorDate, @cur_SuggorQty, @cur_SuggorQty, 0.00, 0.00, 0.00, 
                        @DiscPct, @cur_PurchasePrice, @cur_CostPrice, @cur_ABCClass, @cur_MovingCode, 
                        NULL, NULL, @TPGO, 'AOS', @SuggorDate, 'AOS', @SuggorDate)

         -- update item master table
            update spMstItems 
               set OnOrder = OnOrder + @cur_SuggorQty
             where CompanyCode = @CompanyCode
               and BranchCode  = @BranchCode
               and PartNo      = @cur_NewPartNo

         fetch next from SUGGOR 
               into @cur_NewPartNo, @cur_MovingCode, @cur_ABCClass, @cur_ProductType, @cur_PartCategory, 
                    @cur_TypeOfGoods, @cur_SupplierCode, @cur_CostPrice, @cur_PurchasePrice, @cur_OnHand, 
                    @cur_OnOrder, @cur_InTransit, @cur_AllocationSP, @cur_AllocationSR, @cur_AllocationSL, 
                    @cur_BackOrderSP, @cur_BackOrderSR, @cur_BackOrderSL, @cur_ReservedSP, @cur_ReservedSR, 
                    @cur_ReservedSL, @cur_AvailableQty, @cur_DmnAvg, @cur_OrderPoint, @cur_SafetyStokPoint, 
                    @cur_SuggorQty
      end
    close SUGGOR
    deallocate SUGGOR
	 
    -- alert machine order
       set @Counter = isnull((select max(MessageID) from SysMessageBoards),0) + 1
       set @MessageText = 'AOS# ' + @AOS1 + '. ' + @AOS2
       insert into SysMessageBoards
                  (MessageID, MessageHeader, MessageText, MessageTo, MessageTarget, MessageParams, 
                   DateFrom, DateTo, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
            values(@Counter, 'MACHINE ORDER', @MessageText, 'ALL', '', NULL,
                   NULL, NULL, 'AOS', @SuggorDate, 'AOS', @SuggorDate)
       select top 1 * from SysMessageBoards order by MessageID desc
    
    drop table #Suggor
    drop table #t1
    drop table #t2
    drop table #t3
    drop table #t4

END

GO

if object_id('uspfn_spCheckOutsQty') is not null
	drop procedure uspfn_spCheckOutsQty
GO


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[uspfn_spCheckOutsQty]
	(
		@CompanyCode varchar(50),
		@BranchCode varchar(50),
		@CustCode varchar(50),
		@TransType varchar(50),
		@SalesType varchar(50),
		@PartNoOri varchar(50)

	)
AS
BEGIN
	SELECT 
		spTrnSORDDtl.DocNo, 
		CONVERT(varchar,spTrnSORDHdr.DocDate, 106) AS DocDate,
		spTrnSORDDtl.PartNo,
		CONVERT(NUMERIC(18,2), 
		ISNULL(spTrnSORDDtl.QtyBO, 0)) AS QtyBO, 
		CONVERT(NUMERIC(18,2), ISNULL(spTrnSORDDtl.QtyBoSupply, 0)) AS QtyBoSupply,
		CONVERT(NUMERIC(18,2), ISNULL(spTrnSORDDtl.QtyBO, 0)) - CONVERT(NUMERIC(18,2), ISNULL(spTrnSORDDtl.QtyBoSupply, 0)) - CONVERT(NUMERIC(18,2), ISNULL(spTrnSORDDtl.QtyBOCancel, 0)) AS QtyBOOts,
		CONVERT(NUMERIC(18,2), ISNULL(spTrnSORDDtl.QtyBOCancel, 0)) AS QtyBOCancel, 
		spTrnSORDDtl.PartNoOriginal 
	FROM spTrnSORDDtl 
	INNER JOIN spTrnSORDHdr ON spTrnSORDDtl.DocNo = spTrnSORDHdr.DocNo 
				AND spTrnSORDHdr.CustomerCode=@CustCode AND  spTrnSORDHdr.TransType = @TransType 
				AND spTrnSORDHdr.SalesType = @SalesType AND spTrnSORDHdr.Status >= 2 
				AND spTrnSORDHdr.CompanyCode=@CompanyCode AND spTrnSORDHdr.BranchCode = @BranchCode
	WHERE 
	ISNULL(spTrnSORDDtl.QtyBO, 0) - ISNULL(spTrnSORDDtl.QtyBOSupply, 0) - ISNULL(spTrnSORDDtl.QtyBOCancel, 0) > 0 
	AND spTrnSORDDtl.CompanyCode=@CompanyCode AND spTrnSORDDtl.BranchCode = @BranchCode AND spTrnSORDDtl.PartNoOriginal = @PartNoOri  
	ORDER BY spTrnSORDHdr.DocDate DESC 
END



GO

if object_id('uspfn_spCustRptRgs011') is not null
	drop procedure uspfn_spCustRptRgs011
GO
-- =============================================
-- Author:		David L.
-- Create date: 22 Sep 2014
-- Description:	Get Customer For Report Register 011 
--              (Report Register Penjualan Per Pelanggan (Tunai&Kredit))
-- =============================================
CREATE PROCEDURE uspfn_spCustRptRgs011 
	@CompanyCode varchar(15),
	@BranchCode varchar(15),
	@DateFrom datetime,
	@DateTo datetime,
	@PartType varchar(2),
	@PaymentCode nchar(1)
AS
BEGIN
	declare @TOPC varchar(15)
	set @TOPC = 'TOPC'

	if(@PaymentCode = '0')
	begin	
	SELECT DISTINCT
			    a.CustomerCode,
			    UPPER(b.CustomerName) CustomerName
		      FROM 
			    SpTrnSFPJHdr a,
			    GnMstCustomer b,
			    (
				    SELECT 
					    row_number()OVER(ORDER BY SpTrnSFPJHdr.customerCode)rowNum,
					    SpTrnSFPJHdr.customerCode 
				    FROM 
					    SpTrnSFPJHdr 
					    left join gnMstLookupDtl on SpTrnSFPJHdr.CompanyCode = gnMstLookupDtl.CompanyCode
						    AND gnMstLookupDtl.CodeId = @TOPC
				    WHERE
					    SpTrnSFPJHdr.CompanyCode	= @CompanyCode
					    AND SpTrnSFPJHdr.BranchCode = @BranchCode
					    AND convert(varchar,SpTrnSFPJHdr.FPJDate, 112) 
						    BETWEEN convert(varchar, convert(datetime, @DateFrom), 112) 
						    AND convert(varchar, convert(datetime, @DateTo), 112)
					    AND gnMstLookupDtl.ParaValue = 0
				    GROUP BY customerCode
			    )c,
			    gnMstLookupDtl d	
		     WHERE 
			    a.CompanyCode		= b.CompanyCode
			    AND a.CustomerCode	= b.CustomerCode
			    AND a.CustomerCode	= c.CustomerCode
			    AND a.CompanyCode	= @CompanyCode
			    AND a.BranchCode	= @BranchCode
			    AND convert(varchar, a.FPJDate, 112) 
					    BETWEEN convert(varchar, convert(datetime, @DateFrom), 112) 
					    AND convert(varchar, convert(datetime, @DateTo), 112)
			    AND a.CompanyCode	= d.CompanyCode
			    AND d.CodeId		= @TOPC
			    AND a.TOPCode		= d.LookupValue	
			    AND d.ParaValue     = 0 			 		
                AND (CASE WHEN @PartType = '' THEN '' ELSE a.TypeOfGoods END) = @PartType
		     ORDER BY CustomerCode
	end
	else if(@PaymentCode = '1')
	begin 
	SELECT DISTINCT
			    a.CustomerCode,
			    UPPER(b.CustomerName) CustomerName
		      FROM 
			    SpTrnSFPJHdr a,
			    GnMstCustomer b,
			    (
				    SELECT 
					    row_number()OVER(ORDER BY SpTrnSFPJHdr.customerCode)rowNum,
					    SpTrnSFPJHdr.customerCode 
				    FROM 
					    SpTrnSFPJHdr 
					    left join gnMstLookupDtl on SpTrnSFPJHdr.CompanyCode = gnMstLookupDtl.CompanyCode
						    AND gnMstLookupDtl.CodeId = @TOPC
				    WHERE
					    SpTrnSFPJHdr.CompanyCode	= @CompanyCode
					    AND SpTrnSFPJHdr.BranchCode = @BranchCode
					    AND convert(varchar,SpTrnSFPJHdr.FPJDate, 112) 
						    BETWEEN convert(varchar, convert(datetime, @DateFrom), 112) 
						    AND convert(varchar, convert(datetime, @DateTo), 112)
					    AND gnMstLookupDtl.ParaValue = 0
				    GROUP BY customerCode
			    )c,
			    gnMstLookupDtl d	
		     WHERE 
			    a.CompanyCode		= b.CompanyCode
			    AND a.CustomerCode	= b.CustomerCode
			    AND a.CustomerCode	= c.CustomerCode
			    AND a.CompanyCode	= @CompanyCode
			    AND a.BranchCode	= @BranchCode
			    AND convert(varchar, a.FPJDate, 112) 
					    BETWEEN convert(varchar, convert(datetime, @DateFrom), 112) 
					    AND convert(varchar, convert(datetime, @DateTo), 112)
			    AND a.CompanyCode	= d.CompanyCode
			    AND d.CodeId		= @TOPC
			    AND a.TOPCode		= d.LookupValue	
			    AND d.ParaValue     > 0 			 		
                AND (CASE WHEN @PartType = '' THEN '' ELSE a.TypeOfGoods END) = @PartType
		     ORDER BY CustomerCode
	end
	else
	begin
	SELECT DISTINCT
			    a.CustomerCode,
			    UPPER(b.CustomerName) CustomerName
		      FROM 
			    SpTrnSFPJHdr a,
			    GnMstCustomer b,
			    (
				    SELECT 
					    row_number()OVER(ORDER BY SpTrnSFPJHdr.customerCode)rowNum,
					    SpTrnSFPJHdr.customerCode 
				    FROM 
					    SpTrnSFPJHdr 
					    left join gnMstLookupDtl on SpTrnSFPJHdr.CompanyCode = gnMstLookupDtl.CompanyCode
						    AND gnMstLookupDtl.CodeId = @TOPC
				    WHERE
					    SpTrnSFPJHdr.CompanyCode	= @CompanyCode
					    AND SpTrnSFPJHdr.BranchCode = @BranchCode
					    AND convert(varchar,SpTrnSFPJHdr.FPJDate, 112) 
						    BETWEEN convert(varchar, convert(datetime, @DateFrom), 112) 
						    AND convert(varchar, convert(datetime, @DateTo), 112)
					    AND gnMstLookupDtl.ParaValue = 0
				    GROUP BY customerCode
			    )c,
			    gnMstLookupDtl d	
		     WHERE 
			    a.CompanyCode		= b.CompanyCode
			    AND a.CustomerCode	= b.CustomerCode
			    AND a.CustomerCode	= c.CustomerCode
			    AND a.CompanyCode	= @CompanyCode
			    AND a.BranchCode	= @BranchCode
			    AND convert(varchar, a.FPJDate, 112) 
					    BETWEEN convert(varchar, convert(datetime, @DateFrom), 112) 
					    AND convert(varchar, convert(datetime, @DateTo), 112)
			    AND a.CompanyCode	= d.CompanyCode
			    AND d.CodeId		= @TOPC
			    AND a.TOPCode		= d.LookupValue	
                AND (CASE WHEN @PartType = '' THEN '' ELSE a.TypeOfGoods END) = @PartType
		     ORDER BY CustomerCode
	end
END

GO

if object_id('uspfn_spGetDataOutstandingBO') is not null
	drop procedure uspfn_spGetDataOutstandingBO
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[uspfn_spGetDataOutstandingBO]
	(
	@CompanyCode varchar(50),
	@BranchCode varchar(50),
	@CustCode varchar(50),
	@TransType varchar(50),
	@TypeOfGoods varchar(50),
	@SalesType varchar(50),
	@ProductType varchar(50)
	)
AS
BEGIN
	SELECT 
		Convert(bit, 0) chkSelect 
		, spTrnSORDDtl.DocNo
		, CONVERT(NUMERIC(18,2),0) AS QtyBOProc
		, spTrnSORDHdr.DocDate
		, spTrnSORDDtl.PartNo
		, CONVERT(NUMERIC(18,2)
		, ISNULL(spTrnSORDDtl.QtyBO, 0)) AS QtyBO
		, CONVERT(NUMERIC(18,2)
		, ISNULL(spTrnSORDDtl.QtyBoSupply, 0)) AS QtyBoSupply
		, CONVERT(NUMERIC(18,2), ISNULL(spTrnSORDDtl.QtyBO, 0)) - CONVERT(NUMERIC(18,2), ISNULL(spTrnSORDDtl.QtyBoSupply, 0)) - CONVERT(NUMERIC(18,2), ISNULL(spTrnSORDDtl.QtyBOCancel, 0)) AS QtyBOOts
		, CONVERT(NUMERIC(18,2), ISNULL(spTrnSORDDtl.QtyBOCancel, 0)) AS QtyBOCancel
		, spTrnSORDDtl.PartNoOriginal 
	FROM spTrnSORDDtl 
	INNER JOIN spTrnSORDHdr ON spTrnSORDDtl.DocNo = spTrnSORDHdr.DocNo AND spTrnSORDHdr.CustomerCode=@CustCode AND
				spTrnSORDHdr.TransType = @TransType AND spTrnSORDHdr.SalesType = @SalesType AND spTrnSORDHdr.Status >= 2 AND
				spTrnSORDHdr.TypeOfGoods = @TypeOfGoods AND spTrnSORDHdr.CompanyCode=@CompanyCode AND spTrnSORDHdr.BranchCode = @BranchCode
    WHERE 
	ISNULL(spTrnSORDDtl.QtyBO, 0) -  ISNULL(spTrnSORDDtl.QtyBOSupply, 0) - ISNULL(spTrnSORDDtl.QtyBOCancel, 0) > 0 
	AND spTrnSORDDtl.CompanyCode=@CompanyCode AND spTrnSORDDtl.BranchCode = @BranchCode AND spTrnSORDDtl.ProductType = @ProductType  
	ORDER BY spTrnSORDHdr.DocDate DESC 
END


GO

if object_id('uspfn_spGetEmployeeAllBranchs') is not null
	drop procedure uspfn_spGetEmployeeAllBranchs
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[uspfn_spGetEmployeeAllBranchs]
	@CompanyCode varchar(15)
AS
BEGIN
	SELECT EmployeeID, EmployeeName
	FROM gnMstEmployee 
	WHERE CompanyCode = @CompanyCode
	AND TitleCode IN ('7','58')
	AND PersonnelStatus = '1'
	ORDER BY EmployeeName ASC
END


GO

if object_id('uspfn_spGetEmployeeBranchs') is not null
	drop procedure uspfn_spGetEmployeeBranchs
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[uspfn_spGetEmployeeBranchs]
	@CompanyCode varchar(15), 
	@BranchCode varchar(15)
AS
BEGIN
	 SELECT EmployeeID, EmployeeName
	FROM gnMstEmployee 
	WHERE CompanyCode =  @CompanyCode 
	AND BranchCode = @BranchCode
	AND TitleCode IN  ('7','58')
	AND PersonnelStatus = '1'
	ORDER BY EmployeeName ASC
END


GO

if object_id('uspfn_spGetSpTrnSBPSFDtl') is not null
	drop procedure uspfn_spGetSpTrnSBPSFDtl
GO
CREATE PROCEDURE uspfn_spGetSpTrnSBPSFDtl   
@CompanyCode varchar(15), @BranchCode varchar(15), @BPSFNo varchar(15)  
as  
select PartNo, PartNoOriginal, DocNo, DocDate, ReferenceNo, QtyBill from spTrnSBPSFDtl  
where CompanyCode = @CompanyCode and BranchCode = @BranchCode and BPSFNo = @BPSFNo  
GO

if object_id('uspfn_spMstCompanyAccountDtl2') is not null
	drop procedure uspfn_spMstCompanyAccountDtl2
GO
create procedure [dbo].[uspfn_spMstCompanyAccountDtl2] (@CompanyCode varchar(10) , @CompanyCodeTo varchar(10))
 as
SELECT [CompanyCode]
      ,[CompanyCodeTo]
      ,[TPGO]
      ,[TPGOName]
      ,[AccountNo]
      ,[AccountName]
FROM [sp_spMstCompanyAccountDtl]
where CompanyCode=@CompanyCode and CompanyCodeTo=@CompanyCodeTo
GO

if object_id('uspfn_SvClosingMonth') is not null
	drop procedure uspfn_SvClosingMonth
GO
-- =============================================
-- Author:		SDMS - David
-- Create date: 5 Feb 2015
-- Description:	Menututup bulan service
-- =============================================
CREATE PROCEDURE uspfn_SvClosingMonth
	@CompanyCode varchar(20),
	@BranchCode varchar(20),
	@FiscalYear int,
	@FiscalMonth int,
	@PeriodeNum int,
	@NextFiscalYear int,
	@NextPeriodeNum int,
	@ProfitCenterCode varchar(20),
	@Month int
AS
BEGIN

    update gnMstPeriode
   set StatusService = '2'
 where CompanyCode = @CompanyCode
   and BranchCode = @BranchCode
   and FiscalYear = @FiscalYear
   and FiscalMonth = @FiscalMonth
   and PeriodeNum = @PeriodeNum

update gnMstPeriode
   set StatusService = '1'
 where CompanyCode = @CompanyCode
   and BranchCode = @BranchCode
   and FiscalYear = @NextFiscalYear
   and PeriodeNum = @NextPeriodeNum
   and FiscalMonth = @FiscalMonth

update gnMstCoProfileService 
   set FiscalYear = @NextFiscalYear
      ,FiscalPeriod = @NextPeriodeNum
      ,PeriodBeg = isnull((
            select FromDate from gnMstPeriode
             where CompanyCode = @CompanyCode
               and BranchCode = @BranchCode
               and FiscalYear = @NextFiscalYear
               and PeriodeNum = @NextPeriodeNum
               and FiscalMonth = @FiscalMonth
           ),0)
      ,PeriodEnd = isnull((
            select EndDate from gnMstPeriode
             where CompanyCode = @CompanyCode
               and BranchCode = @BranchCode
               and FiscalYear = @NextFiscalYear
               and PeriodeNum = @NextPeriodeNum
               and FiscalMonth = @FiscalMonth
           ),0)
 where CompanyCode = @CompanyCode
   and BranchCode = @BranchCode

   IF (@Month = 12)
   BEGIN
   update gnMstDocument
   set DocumentYear = convert(int, @FiscalYear) + 1
      ,DocumentSequence = 0
      ,LastUpdateBy = 'closing_month'
      ,LastUpdateDate = GetDate()
 where CompanyCode = @CompanyCode
   and BranchCode = @BranchCode
   and ProfitCenterCode = @ProfitCenterCode
   and DocumentType <> 'LOT'
   END 
   
END

GO

if object_id('uspfn_SvInqFpjHQList') is not null
	drop procedure uspfn_SvInqFpjHQList
GO
Create procedure [dbo].[uspfn_SvInqFpjHQList]
	@CompanyCode nvarchar(20),
	@BranchCode nvarchar(20)

as

 select distinct
	trnFakturPajak.FPJNo
	, trnFakturPajak.FPJDate
	, (
		(	
			select top 1 InvoiceNo+' ('+substring(BranchCode,len(BranchCode)-1,3)+')'
			from svTrnInvoice 
			where CompanyCode = trnFakturPajak.CompanyCode 
				and FPJNo = trnFakturPajak.FPJNo 
				and isLocked = 1
			order by BranchCode,InvoiceNo
		) 
		+ ' s/d ' + 
		(	
			select top 1 InvoiceNo+' ('+substring(BranchCode,len(BranchCode)-1,3)+')'
			from svTrnInvoice 
			where CompanyCode = trnFakturPajak.CompanyCode 
				and FPJNo = trnFakturPajak.FPJNo 
				and isLocked = 1
			order by InvoiceNo,BranchCode desc
		)
	) as Invoice	
	, (
		select top 1 BranchCode 
		from svTrnInvoice 
		where CompanyCode = trnFakturPajak.CompanyCode  
			and FPJNo = trnFakturPajak.FPJNo 
			and isLocked = 1
		order by BranchCode, InvoiceNo
	) as BranchStart
    , (
		select top 1 BranchCode 
		from svTrnInvoice	
		where CompanyCode = trnFakturPajak.CompanyCode  
			and FPJNo = trnFakturPajak.FPJNo 
			and isLocked = 1
		order by BranchCode desc
	) as BranchEnd
	, (	trnFakturPajak.CustomerCode + ' - ' + 
		(
			select CustomerName 
			from gnMstCustomer
			where CompanyCode = trnFakturPajak.CompanyCode  and CustomerCode = trnFakturPajak.CustomerCode
		)
	) as Customer
	, (	trnFakturPajak.CustomerCodeBill + ' - ' + 
		(
			select CustomerName 
			from gnMstCustomer 
			where CompanyCode = trnFakturPajak.CompanyCode  and CustomerCode = trnFakturPajak.CustomerCodeBill
		)
	) as CustomerBill
from svTrnFakturPajak trnFakturPajak
left join svTrnInvoice trnInvoice on 
	trnInvoice.CompanyCode = trnFakturPajak.CompanyCode 
	--and trnInvoice.BranchCode = trnFakturPajak.BranchCode
	and trnInvoice.FPJNo = trnFakturPajak.FPJNo
	and trnInvoice.IsLocked=1
where 
    trnFakturPajak.CompanyCode = @CompanyCode
	and trnFakturPajak.BranchCode = @BranchCode
	and trnFakturPajak.FPJNo like 'FPH%' 
order by trnFakturPajak.FPJNo desc
 
 
  
  
GO

if object_id('uspfn_SvInqVehicleHistoryWSDS') is not null
	drop procedure uspfn_SvInqVehicleHistoryWSDS
GO
CREATE procedure [dbo].[uspfn_SvInqVehicleHistoryWSDS]
	 @CompanyCode varchar(20),
	 @BranchCode  varchar(20),
	 @PoliceRegNo varchar(20) = '',
	 @BasicModel  varchar(20) = '',
	 @CustomerCode varchar(20) = '',
	 @CustomerName varchar(20) = '',
	 @ChassisCode  varchar(20) = '',
	 @ChassisNo    varchar(20) = '',
	 @EngineCode  varchar(20) = '',
	 @EngineNo    varchar(20) = ''

as

select distinct convert(bit, '0') as IsSelect
     , a.PoliceRegNo
     , a.BasicModel
     , '' as TransmissionType
     , ltrim(rtrim(a.ChassisCode)) + ' ' + cast(a.ChassisNo as char) Chassis 
     , a.ChassisCode
     , cast(a.ChassisNo as varchar) as ChassisNo
     , ltrim(rtrim(a.EngineCode)) + ' ' + cast(a.EngineNo as char) Engine
     , '' as ServiceBookNo
     , '' as ColourCode
     , a.CustomerCode + ' - ' + b.CustomerName Customer
     , '' as FakturPolisiDate	
     , '' as LastServiceDate
     , '' as LastServiceOdometer
     , '' as Dealer
     , b.CustomerName
 from svHstVehicle a
 left join gnMstCustomer b with(nolock, nowait)
   on a.CompanyCode = b.CompanyCode
  and a.CustomerCode = b.CustomerCode
   and a.PoliceRegNo like ('%' + @PoliceRegNo + '%')
   and a.BasicModel like ('%' + @BasicModel + '%')
   and a.CustomerCode like ('%' + @CustomerCode + '%')
   and b.CustomerName like ('%' + @CustomerName + '%')
   and a.ChassisCode like ('%' + @ChassisCode + '%')
   and convert(varchar, a.ChassisNo) like ('%' + @ChassisNo + '%')
   and a.EngineCode like ('%' + @EngineCode + '%')
   and convert(varchar, a.EngineNo) like ('%' + @EngineNo  + '%')


GO

if object_id('uspfn_SvTrnCustVehicle') is not null
	drop procedure uspfn_SvTrnCustVehicle
GO
CREATE procedure [dbo].[uspfn_SvTrnCustVehicle]
	@CompanyCode varchar(15),
	@BranchCode varchar(15),
	@ChassisCode varchar(15),
	@ChassisNo varchar (15),
	@TransPeriod DateTime
as  

select
    a.PoliceRegNo
   ,a.ServiceBookNo
   ,a.BasicModel
   ,a.TransmissionType
   ,a.ChassisCode
   ,a.ChassisNo
   ,a.EngineCode
   ,a.EngineNo
   ,a.ColourCode ColorCode
   ,rtrim(rtrim(a.ColourCode) +
    case isnull(f.RefferenceDesc2,'') when '' then '' else ' - ' end +
    isnull(f.RefferenceDesc2,'')) as ColorCodeDesc
   ,b.CustomerCode CustCode
   ,b.CustomerName CustName
   ,b.Address1 CustAddr1
   ,b.Address2 CustAddr2
   ,b.Address3 CustAddr3
   ,b.Address4 CustAddr4
   ,b.CityCode CustCityCode
   ,g.LaborDiscPct
   ,g.PartDiscPct
   ,g.MaterialDiscPct
   ,e.LookupValueName CustCityName
   ,b.PhoneNo
   ,b.FaxNo
   ,b.HPNo
   -- Contract No
   ,isnull(c.ContractNo,'') ContractNo
   ,case isnull(c.ContractNo,'') when '' then '' else c.EndPeriod end ContractEndPeriod
   ,c.IsActive ContractStatus
   ,case isnull(c.ContractNo,'')
      when '' then ''
      else (case c.IsActive when 1 then 'Aktif' else 'Tidak Aktif' end)
    end ContractStatusDesc
   -- Club Code
   ,d.ClubCode
   ,case isnull(d.ClubCode,'') when '' then '' else a.ClubDateFinish end ClubEndPeriod
   ,d.IsActive ClubStatus
   ,case isnull(d.ClubCode,'')
      when '' then ''
      else (case d.IsActive when 1 then 'Aktif' else 'Tidak Aktif' end)
    end ClubStatusDesc
   ,isnull(i.TaxCode,'') TaxCode
   ,isnull(j.TaxPct,0) TaxPct
  from svMstCustomerVehicle a with (nowait,nolock)
  left join gnMstCustomer b with (nowait,nolock)
    on b.CompanyCode = a.CompanyCode
   and b.CustomerCode = a.CustomerCode
  left join svMstContract c with (nowait,nolock)
    on c.IsActive = 1
   and c.CompanyCode = a.CompanyCode
   and c.ContractNo = a.ContractNo
   and c.BeginPeriod <= @TransPeriod
  left join svMstClub d with (nowait,nolock)
    on d.IsActive = 1
   and d.CompanyCode = a.CompanyCode
   and d.ClubCode = a.ClubCode
  left join gnMstLookupDtl e with (nowait,nolock)
    on e.CompanyCode = a.CompanyCode
   and e.CodeID = 'CITY'
   and e.LookUpValue = b.CityCode
  left join omMstRefference f with (nowait,nolock)
    on f.CompanyCode = a.CompanyCode
   and f.RefferenceCode = 'COLO'
   and f.RefferenceCode = a.ColourCode
  left join gnMstCustomerProfitCenter g with (nowait,nolock)
    on g.CompanyCode = a.CompanyCode
   and g.BranchCode = @BranchCode
   and g.CustomerCode = a.CustomerCode
   and g.ProfitCenterCode = '200'
  left join gnMstCustomerProfitCenter i with (nowait,nolock)
    on i.CompanyCode = a.CompanyCode
   and i.BranchCode = @BranchCode
   and i.CustomerCode = a.CustomerCode
   and i.ProfitCenterCode = '200'
  left join gnMstTax j with (nowait,nolock)
    on j.CompanyCode = a.CompanyCode
   and j.TaxCode = i.TaxCode
 where 1 = 1
   and a.CompanyCode = @CompanyCode
   and a.ChassisCode = @ChassisCode
   and a.ChassisNo = @ChassisNo
   and a.ClubDateStart <= @TransPeriod
GO

if object_id('uspfn_TaxRecalculatePPN') is not null
	drop procedure uspfn_TaxRecalculatePPN
GO
 CREATE procedure [dbo].[uspfn_TaxRecalculatePPN]
	@CompanyCode nvarchar(15),
	@BranchCode nvarchar(15),
	@ProductType nvarchar(2),
	@PeriodYear int,
	@PeriodMonth int,
	@ProfitCenter nvarchar(15),
	@DocumentType nvarchar(2)

  as
  begin

update gnTaxPPN
set DPPStd=	(
            select 
                isnull(sum(DPPAmt),0) 
            from 
                gnTaxIn
            where
                CompanyCode= @CompanyCode and BranchCode= @BranchCode
                and ProductType= @ProductType and PeriodYear= @PeriodYear
                and PeriodMonth= @PeriodMonth and ProfitCenter= @ProfitCenter
                and DocumentType= @DocumentType and IsPKP= 1
        )
,DPPSdh= (
            select 
                isnull(sum(DPPAmt),0) 
            from 
                gnTaxIn
            where
                CompanyCode= @CompanyCode and BranchCode= @BranchCode
                and ProductType= @ProductType and PeriodYear= @PeriodYear
                and PeriodMonth= @PeriodMonth and ProfitCenter= @ProfitCenter
                and DocumentType= @DocumentType and IsPKP= 0
        )
,PPNStd= (
            select 
                isnull(sum(PPNAmt),0) 
            from 
                gnTaxIn
            where
                CompanyCode= @CompanyCode and BranchCode= @BranchCode
                and ProductType= @ProductType and PeriodYear= @PeriodYear
                and PeriodMonth= @PeriodMonth and ProfitCenter= @ProfitCenter
                and DocumentType= @DocumentType and IsPKP= 1
        )
,PPNSdh= (
            select 
                isnull(sum(PPNAmt),0) 
            from 
                gnTaxIn
            where
                CompanyCode= @CompanyCode and BranchCode= @BranchCode
                and ProductType= @ProductType and PeriodYear= @PeriodYear
                and PeriodMonth= @PeriodMonth and ProfitCenter= @ProfitCenter
                and DocumentType= @DocumentType and IsPKP= 0
        )
where
CompanyCode= @CompanyCode and BranchCode= @BranchCode
and ProductType= @ProductType and PeriodYear= @PeriodYear
and PeriodMonth= @PeriodMonth and ProfitCenter= @ProfitCenter
and TaxType= case when @DocumentType= 'F' then '3' else '4' end
end
GO

if object_id('uspfn_TaxUpdateTotal') is not null
	drop procedure uspfn_TaxUpdateTotal
GO
 CREATE procedure [dbo].[uspfn_TaxUpdateTotal]
	@CompanyCode nvarchar(25),
	@BranchCode nvarchar(25),
	@ProductType nvarchar(2),
	@Period datetime 

  as
  begin

update gnTaxPPN
set PPNStd= (
                isnull((	select	case when PPNStd > 0 then 0 else PPNStd end PPNStd
			                from	gnTaxPPN
			                where	CompanyCode= a.CompanyCode and BranchCode= a.BranchCode
					                and ProductType= a.ProductType
					                and PeriodYear = year(dateadd(month,-1,@Period))
					                and PeriodMonth = month(dateadd(month,-1,@Period)) 
					                and TaxType= '5'),0) +
                isnull((	select	sum(PPNStd)
							from	gnTaxPPN 
							where	CompanyCode= a.CompanyCode and BranchCode= a.BranchCode
					                and ProductType= a.ProductType
					                and PeriodYear = year(@Period)
					                and PeriodMonth = month(@Period) 
									and TaxType in ('1','2')),0)-
                isnull((	select	sum(PPNStd) 
							from	gnTaxPPN 
							where	CompanyCode= a.CompanyCode and BranchCode= a.BranchCode
					                and ProductType= a.ProductType
					                and PeriodYear = year(@Period)
					                and PeriodMonth = month(@Period) 
									and TaxType in ('3','4')),0)
            )
,PPNSdh= (
                isnull((	select	case when PPNSdh > 0 then 0 else PPNSdh end PPNSdh
			                from	gnTaxPPN
			                where	CompanyCode= a.CompanyCode and BranchCode= a.BranchCode
					                and ProductType= a.ProductType
					                and PeriodYear = year(dateadd(month,-1,@Period))
					                and PeriodMonth = month(dateadd(month,-1,@Period)) 
					                and TaxType= '5'),0) +
                isnull((	select	sum(PPNSdh)
							from	gnTaxPPN 
							where	CompanyCode= a.CompanyCode and BranchCode= a.BranchCode
					                and ProductType= a.ProductType
					                and PeriodYear = year(@Period)
					                and PeriodMonth = month(@Period) 
									and TaxType in ('1','2')),0)-
                isnull((	select	sum(PPNSdh) 
							from	gnTaxPPN 
							where	CompanyCode= a.CompanyCode and BranchCode= a.BranchCode
					                and ProductType= a.ProductType
					                and PeriodYear = year(@Period)
					                and PeriodMonth = month(@Period) 
									and TaxType in ('3','4')),0)
            )
from gnTaxPPN a
where
    CompanyCode= @CompanyCode
	AND BranchCode = case @BranchCode when '' then BranchCode else @BranchCode end 
    and ProductType= @ProductType and PeriodYear = year(@Period)
    and PeriodMonth = month(@Period) and TaxType = '5'
	end
GO

if object_id('usprpt_GnGenerateSeqTaxWoBranchUnion') is not null
	drop procedure usprpt_GnGenerateSeqTaxWoBranchUnion
GO
CREATE procedure [dbo].[usprpt_GnGenerateSeqTaxWoBranchUnion]
	@CompanyCode as varchar(15)
	,@BranchCode as varchar(15)
	,@StartDate as varchar(8)
	,@FPJDate as varchar(8)
	,@ProfitCenterCode as varchar(3)
	,@UserId as varchar(15)
	,@DocNo as varchar(5000)
AS
BEGIN

--declare @CompanyCode as varchar(15)
--	,@BranchCode as varchar(15)
--	,@StartDate as varchar(8)
--	,@FPJDate as varchar(8)
--	,@ProfitCenterCode as varchar(3)
--	,@UserId as varchar(15)
--	,@DocNo as varchar(5000)

--set @CompanyCode = '6115204001'
--set @BranchCode = '6115204301'
--set @StartDate = '20140901'
--set @FPJDate = '20140909'
--set @ProfitCenterCode = '300'
--set @UserId = 'ga'
--set @DocNo = '|6115204301 INV/14/102569|'

declare @IsUnion as varchar(1)
set @IsUnion = '0'

if(select count(LookUpValue) from gnMstLookUpDtl where CompanyCode= @CompanyCode and LookUpValue=@BranchCode and CodeID='BRANCH') > 0
	set @IsUnion = '1'

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
			AND isPKP = 1 AND ISNULL(FPJGovNo, '') = ''
			AND CONVERT(VARCHAR, InvoiceDate, 112) BETWEEN @StartDate AND @FPJDate
			AND ((case when @ProfitCenterCode='' then BranchCode+' '+InvoiceNo end) = BranchCode+' '+InvoiceNo
				or (case when @ProfitCenterCode<>'' then @DocNo end) like '%|'+BranchCode+' '+InvoiceNo+'|%')
	GROUP BY CompanyCode, BranchCode, InvoiceNo, CustomerCodeBill,convert(varchar,InvoiceDate,112),convert(varchar,DueDate,112), CustomerCodeBill
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
		
SELECT	* INTO #f2 
FROM 
	( 
		SELECT  TaxCabCode 
		FROM	GnMstCoProfile  
		WHERE	CompanyCode = @CompanyCode AND BranchCode = @BranchCode 
) #f2 

-- create FPJGovNo
select * into #f3 
from (
	select 
		a.OrderNo,a.CompanyCode,a.BranchCode,year(DocDate) PeriodTaxYear
		,month(DocDate) PeriodTaxMonth,ProfitCenterCode
		,DocDate as FPJGovDate,DocNo,convert(datetime,DocDate) DocDate,RefNo,RefDate,@UserId CreatedBy,getDate() CreatedDate
		,TaxTransCode
		,CustomerCodeBill
		,LookupValue
	from 
		#f1 a	
) #f3

--drop table #f3, #f1, #f2

if convert(varchar,@FPJDate,112) < '20130401'
begin
	select * into #f4 from(
		select * 		
			, LEFT(TaxTransCode+'000',3)+'.'+
			RIGHT('000'+isnull((select TaxCabCode from #f2),'000'),3)+'-'+
			RIGHT( isnull(convert(varchar(4),year(DocDate)),year(getDate())),2 )+'.'+ 
			RIGHT( '00000000'+convert(varchar,isnull(
												(select	isnull(FPJSeqNo,0)
												from	GnMstFPJSeqNo
												where	CompanyCode= a.CompanyCode and BranchCode= @BranchCode
														and Year= year(DocDate)
														and convert(varchar,EffectiveDate,112) < '20130401'), 0)+a.OrderNo
												),8 )FPJGovNo
			from #f3 a
	)#f4
	
	--insert to tabel GenerateTax
	insert into
		GnGenerateTax(
			CompanyCode, BranchCode, PeriodTaxYear, PeriodTaxMonth, ProfitCenterCode, FPJGovNo, 
			FPJGovDate, DocNo, DocDate , RefNo, RefDate, CreatedBy, CreatedDate
	) 
	select 
		CompanyCode, BranchCode, PeriodTaxYear, PeriodTaxMonth, ProfitCenterCode, FPJGovNo, 
		FPJGovDate, DocNo, DocDate , RefNo, RefDate, CreatedBy, CreatedDate
	from 
		#f4
	
	drop table #f1
	drop table #f2

	-- update Master Sequence
	update	GnMstFPJSeqNo
	set		FPJSeqNo = FPJSeqNo + isnull((select max(OrderNo) from #f4),0)
	from	GnMstFPJSeqNo
	where	CompanyCode= @CompanyCode and BranchCode= @BranchCode and Year= (select top 1 PeriodTaxYear from #f3)
	
	-- update Sparepart
	update	SPTrnSFPJHdr
	set		FPJGovNo= a.FPJGovNo
			,FPJSignature= a.FPJGovDate
	from	#f4 a, SPTrnSFPJHdr b
	where	a.CompanyCode= b.CompanyCode and a.BranchCode= b.BranchCode and a.DocNo= b.InvoiceNo

	-- update Service
	update	SVTrnFakturPajak
	set		FPJGovNo= a.FPJGovNo
			,SignedDate= a.FPJGovDate
	from	#f4 a, SVTrnFakturPajak b
	where	a.CompanyCode= b.CompanyCode and a.BranchCode= b.BranchCode and a.DocNo= b.FPJNo

	-- update Sales
	update	OmFakturPajakHdr
	set		FakturPajakNo= a.FPJGovNo
			,FakturPajakDate= a.FPJGovDate 
	from	#f4 a, OmFakturPajakHdr b
	where	a.CompanyCode= b.CompanyCode and a.BranchCode= b.BranchCode and a.DocNo= b.InvoiceNo

	-- update Finance
	update	ArFakturPajakHdr
	set		FakturPajakNo= a.FPJGovNo
			,FakturPajakDate= a.FPJGovDate
	from	#f4 a, ArFakturPajakHdr b
	where	a.CompanyCode= b.CompanyCode and a.BranchCode= b.BranchCode and a.DocNo= b.InvoiceNo

	--select top 1 convert(decimal,right(FPJGovNo,8)) from #f4 order by right(FPJGovNo,8) desc
	drop table #f3,#f4
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
	Declare @CountFPJNo				int
	Declare @CountUnValidFPJ		int
	Declare @CurrentDocNo			varchar(100)
	Declare @CurrentCompanyCode		varchar(15)
	Declare @CurrentBranchCode		varchar(15)
	Declare @CurrentTaxTransCode	varchar(2)
	Declare @CurrentCustCodeBill	varchar(15)			
			
	select * into #tblSeqNoFPJ from (
		select row_number() over(order by CompanyCode, BranchCode ,Year ,SeqNo asc) OrderNo, *
		  from GnMstFPJSeqNo with (NOLOCK, NOWAIT)
		 where CompanyCode = @CompanyCode 
			and 
			(
				(	case when @IsUnion = 0 then BranchCode end)= @BranchCode
					or (case when @IsUnion = 1 then BranchCode end) = 
						(select ParaValue from gnMstLookUpDtl where CompanyCode= @CompanyCode and LookUpValue=@BranchCode and CodeID='BRANCH')
			)
			and Year = YEAR(getdate()) 
			and isActive = 1
			and EffectiveDate <= getdate()
			and isnull(BeginTaxNo,'') <> '' 
			and isnull(EndTaxNo,'') <> ''
	)#tblSeqNoFPJ	
				
	set @TotalFPJ = isnull((select TOP 1 FPJSeqNo from #tblSeqNoFPJ order by SeqNo Asc),'')
	set @EndFPJ = isnull((select TOP 1 EndTaxNo from #tblSeqNoFPJ order by SeqNo asc),'')
	set @CountFPJNo = (select SUM(convert(bigint,EndTaxNo) - convert(bigint,FPJSeqNo)) from #tblSeqNoFPJ where LEN(FPJSeqNo) <= 11) 			
						
	if (@TotalFPJ = '0' or @TotalFPJ = '')
	begin
		drop table #f1, #f2, #f3, #tblSeqNoFPJ
		raiserror('Tidak terdapat No Faktur Pajak yang aktif. Silahkan setting No Faktur Pajak terlebih dahulu',16 ,1);
		return
	end
	else
	begin
		if @CountFPJNo - isnull((select max(OrderNo) from #f1),0) < 0
		begin
			drop table #f1, #f2, #f3, #tblSeqNoFPJ
			raiserror('Faktur Pajak Aktif tidak mencukupi jumlah dokumen yang diperlukan. Silahkan setting No Faktur Pajak dengan no seq aktif selanjutnya.',16 ,1);
			return
		end
	end
	
	declare @Awal as bit
	declare @CCodeBill as varchar(15)
	set @Awal = 1	
									
	DECLARE temp CURSOR FOR
	SELECT	CompanyCode, BranchCode, DocNo, TaxTransCode, CustomerCodeBill
	FROM	#f3 

	OPEN temp
	FETCH NEXT FROM temp INTO @CurrentCompanyCode,@CurrentBranchCode,@CurrentDocNo,@CurrentTaxTransCode,@CurrentCustCodeBill
	WHILE @@FETCH_STATUS = 0
	BEGIN	
		if @Awal = 1
		begin -- -1
			print 'Nothing to do'
			set @Awal = 0
			set @CCodeBill = @CurrentCustCodeBill
		end	--	-1		
		else
		begin	-- 0			
			if @CurrentCustCodeBill = (select top 1 LookupValue from gnMstLookupDtl where CompanyCode = @CurrentCompanyCode and CodeID = 'FPJG' and LookupValue = @CurrentCustCodeBill)
			begin --1
				if @CCodeBill = @CurrentCustCodeBill
				begin
					print @CurrentFPJ + ' for ' + @CurrentCustCodeBill				
				end
				else
				begin
					print 'update ' + convert(varchar, convert(bigint,@TotalFPJ) + 1)
					-- update Master Sequence
					if convert(bigint,@TotalFPJ) + 1 < convert(bigint,@EndFPJ)
					begin
						update	GnMstFPJSeqNo
						set		FPJSeqNo = convert(bigint,@TotalFPJ) + 1
						where	CompanyCode= @CompanyCode
							and 
							(
								(	case when @IsUnion = 0 then BranchCode end)= @BranchCode
									or (case when @IsUnion = 1 then BranchCode end) = 
										(select ParaValue from gnMstLookUpDtl where CompanyCode= @CompanyCode and LookUpValue=@BranchCode and CodeID='BRANCH')
							)
						  and	Year = YEAR(getdate())
						  and	SeqNo = (select MIN(SeqNo)
										   from GnMstFPJSeqNo 
										  where CompanyCode = @CompanyCode 
											and 
											(
												(	case when @IsUnion = 0 then BranchCode end)= @BranchCode
													or (case when @IsUnion = 1 then BranchCode end) = 
														(select ParaValue from gnMstLookUpDtl where CompanyCode= @CompanyCode and LookUpValue=@BranchCode and CodeID='BRANCH')
											)
											and Year = YEAR(getdate())
											and isActive = 1
											and EffectiveDate <= getdate()
											and isnull(BeginTaxNo,0) <> 0 
											and isnull(EndTaxNo,0) <> 0)
					end
					else
					begin
						update GnMstFPJSeqNo
						set FPJSeqNo = convert(bigint,@TotalFPJ) + 1, isActive = 0
						where CompanyCode = @CompanyCode
							and 
							(
								(	case when @IsUnion = 0 then BranchCode end)= @BranchCode
									or (case when @IsUnion = 1 then BranchCode end) = 
										(select ParaValue from gnMstLookUpDtl where CompanyCode= @CompanyCode and LookUpValue=@BranchCode and CodeID='BRANCH')
							)
						  and Year = YEAR(getdate())
						  and SeqNo = (select MIN(SeqNo)
										   from GnMstFPJSeqNo 
										  where CompanyCode = @CompanyCode 
											and 
											(
												(	case when @IsUnion = 0 then BranchCode end)= @BranchCode
													or (case when @IsUnion = 1 then BranchCode end) = 
														(select ParaValue from gnMstLookUpDtl where CompanyCode= @CompanyCode and LookUpValue=@BranchCode and CodeID='BRANCH')
											)
											and Year = YEAR(getdate())
											and isActive = 1
											and EffectiveDate <= getdate()
											and isnull(BeginTaxNo,0) <> 0 
											and isnull(EndTaxNo,0) <> 0)
					end
				end
			end
			else
			begin
				if @CCodeBill = @CurrentCustCodeBill
				begin
					print @CurrentFPJ + ' for ' + @CurrentCustCodeBill					
				end
				else
				begin					
					print 'update ' + convert(varchar, convert(bigint,@TotalFPJ) + 1)
					 --update Master Sequence
					if convert(bigint,@TotalFPJ) + 1 < convert(bigint,@EndFPJ)
					begin --3
						update	GnMstFPJSeqNo
						set		FPJSeqNo = convert(bigint,@TotalFPJ) + 1
						where	CompanyCode= @CompanyCode
						  and	(
									(case when @IsUnion = 0 then BranchCode end)= @BranchCode
									or (case when @IsUnion = 1 then BranchCode end) = 
										(select ParaValue from gnMstLookUpDtl where CompanyCode= @CompanyCode and LookUpValue=@BranchCode and CodeID='BRANCH')
								) 
						  and	Year = YEAR(getdate())
						  and	SeqNo = (select MIN(SeqNo)
										   from GnMstFPJSeqNo 
										  where CompanyCode = @CompanyCode 
										  and	(
													(case when @IsUnion = 0 then BranchCode end)= @BranchCode
													or (case when @IsUnion = 1 then BranchCode end) = 
														(select ParaValue from gnMstLookUpDtl where CompanyCode= @CompanyCode and LookUpValue=@BranchCode and CodeID='BRANCH')
												) 
											and Year = YEAR(getdate())
											and isActive = 1
											and EffectiveDate <= getdate()
											and isnull(BeginTaxNo,0) <> 0 
											and isnull(EndTaxNo,0) <> 0)
					end --3
					else
					begin --4
						update GnMstFPJSeqNo
						set FPJSeqNo = convert(bigint,@TotalFPJ) + 1, isActive = 0
						where CompanyCode = @CompanyCode
						  and	(
									(case when @IsUnion = 0 then BranchCode end)= @BranchCode
									or (case when @IsUnion = 1 then BranchCode end) = 
										(select ParaValue from gnMstLookUpDtl where CompanyCode= @CompanyCode and LookUpValue=@BranchCode and CodeID='BRANCH')
								) 
						  and Year = YEAR(getdate())
						  and SeqNo = (select MIN(SeqNo)
										   from GnMstFPJSeqNo 
										  where CompanyCode = @CompanyCode 
										  and	(
													(case when @IsUnion = 0 then BranchCode end)= @BranchCode
													or (case when @IsUnion = 1 then BranchCode end) = 
														(select ParaValue from gnMstLookUpDtl where CompanyCode= @CompanyCode and LookUpValue=@BranchCode and CodeID='BRANCH')
												) 
											and Year = YEAR(getdate())
											and isActive = 1
											and EffectiveDate <= getdate()
											and isnull(BeginTaxNo,0) <> 0 
											and isnull(EndTaxNo,0) <> 0)
					end --4
				end				
			end
		end
		
		set @CCodeBill = @CurrentCustCodeBill
		set @TotalFPJ =   (select FPJSeqNo
							  from GnMstFPJSeqNo
							 where CompanyCode = @CompanyCode 
								and 
								(
									(	case when @IsUnion = 0 then BranchCode end)= @BranchCode
										or (case when @IsUnion = 1 then BranchCode end) = 
											(select ParaValue from gnMstLookUpDtl where CompanyCode= @CompanyCode and LookUpValue=@BranchCode and CodeID='BRANCH')
								)
							   and Year = YEAR(getdate()) 
							   and isActive = 1
							   and EffectiveDate <= getdate()
							   and isnull(BeginTaxNo,'') <> '' 
							   and isnull(EndTaxNo,'') <> ''
							   and SeqNo = (select MIN(SeqNo)
											  from GnMstFPJSeqNo 
											 where CompanyCode = @CompanyCode 
												and 
												(
													(	case when @IsUnion = 0 then BranchCode end)= @BranchCode
														or (case when @IsUnion = 1 then BranchCode end) = 
															(select ParaValue from gnMstLookUpDtl where CompanyCode= @CompanyCode and LookUpValue=@BranchCode and CodeID='BRANCH')
												)
												and Year = YEAR(getdate()) 
												and isActive = 1 
												and EffectiveDate <= getdate()
												and isnull(BeginTaxNo,0) <> 0 
												and isnull(EndTaxNo,0) <> 0))
		set @EndFPJ = (select EndTaxNo
							  from GnMstFPJSeqNo
							 where CompanyCode = @CompanyCode 
								and 
								(
									(	case when @IsUnion = 0 then BranchCode end)= @BranchCode
										or (case when @IsUnion = 1 then BranchCode end) = 
											(select ParaValue from gnMstLookUpDtl where CompanyCode= @CompanyCode and LookUpValue=@BranchCode and CodeID='BRANCH')
								)
								and Year = YEAR(getdate()) 
							   and isActive = 1
							   and EffectiveDate <= getdate()
							   and isnull(BeginTaxNo,'') <> '' 
							   and isnull(EndTaxNo,'') <> ''
							   and SeqNo = (select MIN(SeqNo)
											  from GnMstFPJSeqNo 
											 where CompanyCode = @CompanyCode 
											   and BranchCode = @BranchCode
											   and Year = YEAR(getdate()) 
											   and isActive = 1 
											   and EffectiveDate <= getdate()
											   and isnull(BeginTaxNo,0) <> 0 
											   and isnull(EndTaxNo,0) <> 0))
											   
     
        set @TotalFPJX  = RIGHT('00000000000'+convert(varchar(11),@TotalFPJ),11)
		set @CurrentFPJ = (select LEFT (convert(varchar,@CurrentTaxTransCode) + '000',3)+'.'+
						   	      LEFT (convert(varchar,@TotalFPJX),3) + '-' +
								  RIGHT(convert(varchar,YEAR(getdate())),2) + '.' +
								  RIGHT('00000000'+convert(varchar,(convert(bigint,@TotalFPJ)+1)),8))											  		
		
		--insert to tabel GenerateTax
		insert into
			GnGenerateTax(
				CompanyCode, BranchCode, PeriodTaxYear, PeriodTaxMonth, ProfitCenterCode, FPJGovNo, 
				FPJGovDate, DocNo, DocDate , RefNo, RefDate, CreatedBy, CreatedDate
		) 
		select 
			CompanyCode, BranchCode, PeriodTaxYear, PeriodTaxMonth, ProfitCenterCode
			, @CurrentFPJ FPJGovNo
			, FPJGovDate, DocNo, DocDate , RefNo, RefDate, CreatedBy, CreatedDate
		from 
			#f3
		where CompanyCode = @CurrentCompanyCode
		  and BranchCode = @CurrentBranchCode
		  and DocNo = @CurrentDocNo
		  and TaxTransCode = @CurrentTaxTransCode			
			  						
			---- update Sparepart
			update	SPTrnSFPJHdr
			set		FPJGovNo= @CurrentFPJ
					,FPJSignature= a.FPJGovDate
			from	#f3 a, SPTrnSFPJHdr b
			where	a.CompanyCode= b.CompanyCode and a.BranchCode= b.BranchCode and a.DocNo= b.InvoiceNo 
				and b.CompanyCode = @CurrentCompanyCode and b.BranchCode = @CurrentBranchCode and b.InvoiceNo = @CurrentDocNo

			-- update Service
			update	SVTrnFakturPajak
			set		FPJGovNo= @CurrentFPJ
					,SignedDate= a.FPJGovDate
			from	#f3 a, SVTrnFakturPajak b
			where	a.CompanyCode = b.CompanyCode and a.BranchCode = b.BranchCode and a.DocNo= b.FPJNo
				and b.CompanyCode = @CurrentCompanyCode  and b.BranchCode = @CurrentBranchCode  and b.FPJNo = @CurrentDocNo

			-- update Sales
			update	OmFakturPajakHdr
			set		FakturPajakNo= @CurrentFPJ
					,FakturPajakDate= a.FPJGovDate 
			from	#f3 a, OmFakturPajakHdr b
			where	a.CompanyCode = b.CompanyCode and a.BranchCode = b.BranchCode and a.DocNo= b.InvoiceNo
				and b.CompanyCode = @CurrentCompanyCode  and b.BranchCode = @CurrentBranchCode  and b.InvoiceNo = @CurrentDocNo

			-- update Finance
			update	ArFakturPajakHdr
			set		FakturPajakNo= @CurrentFPJ
					,FakturPajakDate= a.FPJGovDate
			from	#f3 a, ArFakturPajakHdr b
			where	a.CompanyCode = b.CompanyCode and a.BranchCode = b.BranchCode and a.DocNo= b.InvoiceNo
				and b.CompanyCode = @CurrentCompanyCode  and b.BranchCode = @CurrentBranchCode  and b.InvoiceNo = @CurrentDocNo
				
			FETCH NEXT FROM temp INTO @CurrentCompanyCode,@CurrentBranchCode,@CurrentDocNo,@CurrentTaxTransCode,@CurrentCustCodeBill
		END
	CLOSE temp
	DEALLOCATE temp

	declare @countLoop as int = 0
	set @countLoop = (SELECT COUNT(*) FROM #f3)
	IF @countLoop = 1
	begin --4
		update GnMstFPJSeqNo
		set FPJSeqNo = convert(bigint,@TotalFPJ) + 1, isActive = 0
		where CompanyCode = @CompanyCode
		  and	(
					(case when @IsUnion = 0 then BranchCode end)= @BranchCode
					or (case when @IsUnion = 1 then BranchCode end) = 
						(select ParaValue from gnMstLookUpDtl where CompanyCode= @CompanyCode and LookUpValue=@BranchCode and CodeID='BRANCH')
				) 
		  and Year = YEAR(getdate())
		  and SeqNo = (select MIN(SeqNo)
						   from GnMstFPJSeqNo 
						  where CompanyCode = @CompanyCode 
						  and	(
									(case when @IsUnion = 0 then BranchCode end)= @BranchCode
									or (case when @IsUnion = 1 then BranchCode end) = 
										(select ParaValue from gnMstLookUpDtl where CompanyCode= @CompanyCode and LookUpValue=@BranchCode and CodeID='BRANCH')
								) 
							and Year = YEAR(getdate())
							and isActive = 1
							and EffectiveDate <= getdate()
							and isnull(BeginTaxNo,0) <> 0 
							and isnull(EndTaxNo,0) <> 0)
	end --4
					
	--select convert(varchar,convert(bigint,@TotalFPJ) + 1) FPJGovNo	
	drop table #f1, #f2, #f3, #tblSeqNoFPJ		
end	
	-- update TransDate Sparepart
	update GnMstCoProfileSpare 
	set TransDate = convert(datetime, @FPJDate) 
	WHERE CompanyCode= @CompanyCode and convert(datetime, TransDate, 112) < @FPJDate

	-- update TransDate Service
	update GnMstCoProfileService 
	set TransDate = convert(datetime, @FPJDate) 
	WHERE CompanyCode= @CompanyCode and convert(datetime, TransDate, 112) < @FPJDate 

	-- update TransDate Sales
	update GnMstCoProfileSales 
	set TransDate = convert(datetime, @FPJDate) 
	WHERE CompanyCode= @CompanyCode and convert(datetime, TransDate, 112) < @FPJDate 

	-- update TransDate Finance
	update GnMstCoProfileFinance 
	set TransDateAR = convert(datetime, @FPJDate) 
	WHERE CompanyCode= @CompanyCode and convert(datetime, TransDateAR, 112) < @FPJDate
	
END

GO

if object_id('usprpt_OmRpSalRgs033C') is not null
	drop procedure usprpt_OmRpSalRgs033C
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE usprpt_OmRpSalRgs033C 
	@companycode varchar(max),
	@branchcode varchar(max),
	@period1 varchar(max),
	@period2 varchar(max),
	@tipe varchar(1)
AS
BEGIN
	declare @1 varchar(max)
	declare @2 int
	declare @seq int
	declare @columns varchar(max)
	declare @columns2 varchar(max)
	declare @columns3 varchar(max)
	declare @query varchar(max)

	-- TABLE I --
	select companygovname company, isnull(address1,'-') addr1, isnull(address2,'-') addr2, isnull(address3,'-') addr3, phoneno telp 
	from gnmstcoprofile where companycode = @companycode and branchcode = @branchcode

	-- TABLE II --
	select distinct convert(varchar, right(branchcode,2)) code from gnmstorganizationdtl where companycode = @companycode
	
	if(@tipe='A')

	begin
	-- TABLE III --

	--declare @1 varchar(max)
	--declare @2 int
	--declare @seq int
	--declare @columns varchar(max)
	--declare @columns2 varchar(max)
	--declare @columns3 varchar(max)
	--declare @query varchar(max)

	set @columns = ''
	set @columns2 = ''
	set @columns3 = ''

	set @2 = (select count(*) from gnmstorganizationdtl)
	set @seq = 1

	declare data cursor for
		select distinct cast(right(branchcode,2) as varchar) code from gnmstorganizationdtl
	open data
	fetch next from data into @1
	while @@fetch_status = 0 
	begin
		set @columns = @columns+'['+@1+']'	
		set @columns2 = @columns2+'isnull(['+@1+'],0) ['+@1+']'
		set @columns3 = @columns3+'isnull(['+@1+'],0)'

		set @seq = @seq+1
		if (@seq <= @2)
		begin
			set @columns = @columns+', '
			set @columns2 = @columns2+', '
			set @columns3 = @columns3+'+'
		end
	 
		fetch next from data into @1
	end
	close data
	deallocate data

	set @columns2 = @columns2+', '+@columns3+' as Total'

	set @query = '
	select model, '+@columns2+'
	from (
		select code,model,sum(nilai) nilai
		from (
			select cast(right(a.branchcode,2) as varchar) code
				, ltrim(c.salesmodeldesc) model
				, isnull(count(a.invoiceno),0) nilai
			from omtrsalesinvoicevin a
			inner join omtrsalesinvoice b on b.companycode = a.companycode 
				and b.branchcode = a.branchcode
				and b.invoiceno = a.invoiceno 
			left join ommstmodel c on c.companycode = a.companycode 
				and c.salesmodelcode = a.salesmodelcode	
			where a.companycode = '''+@companycode+''' 
				and b.status in (2,5)
				and convert(varchar, b.invoicedate, 112) between '''+@period1+''' and '''+@period2+'''
			group by a.BranchCode, ltrim(c.salesmodeldesc)
			union ALL
			select cast(right(e.branchcode,2) as varchar) code
				, ltrim(c.salesmodeldesc) model
				, isnull(sum(e.quantity),0)*-1 nilai
			from omtrsalesreturn d
			left join omtrsalesreturndetailmodel e on d.companycode = e.companycode 
				and d.branchcode = e.branchcode
				and d.returnno = e.returnno 
			left join ommstmodel c on c.companycode = e.companycode 
				and c.salesmodelcode = e.salesmodelcode	
			where d.companycode = '''+@companycode+''' 
				and d.status in (2,5)
				and convert(varchar, d.returndate, 112) between '''+@period1+''' and '''+@period2+'''
			group by e.BranchCode, ltrim(c.salesmodeldesc)
		) x
		group by code,model
	)p pivot
	(
	sum(nilai) for code in ('+@columns+')
	) as pvt 
	'
	print (@query)
	exec (@query)
	end
	
	else
	begin
		-- TABLE III --
	--declare @1 varchar(max)
	--declare @2 int
	--declare @seq int
	--declare @columns varchar(max)
	--declare @columns2 varchar(max)
	--declare @columns3 varchar(max)
	--declare @query varchar(max)

	set @columns = ''
	set @columns2 = ''
	set @columns3 = ''

	set @2 = (select count(*) from gnmstorganizationdtl)
	set @seq = 1

	declare data cursor for
		select distinct cast(right(branchcode,2) as varchar) code from gnmstorganizationdtl
	open data
	fetch next from data into @1
	while @@fetch_status = 0 
	begin
		set @columns = @columns+'['+@1+']'	
		set @columns2 = @columns2+'isnull(['+@1+'],0) ['+@1+']'
		set @columns3 = @columns3+'isnull(['+@1+'],0)'

		set @seq = @seq+1
		if (@seq <= @2)
		begin
			set @columns = @columns+', '
			set @columns2 = @columns2+', '
			set @columns3 = @columns3+'+'
		end
	 
		fetch next from data into @1
	end
	close data
	deallocate data

	set @columns2 = @columns2+', '+@columns3+' as Total'

	set @query = '
	select model, '+@columns2+'
	from (
		select code,model,sum(nilai) nilai
		from (
			select cast(right(a.branchcode,2) as varchar) code
				, ltrim(a.salesmodelcode) model
				, isnull(count(a.invoiceno),0) nilai
			from omtrsalesinvoicevin a
			inner join omtrsalesinvoice b on b.companycode = a.companycode 
				and b.branchcode = a.branchcode
				and b.invoiceno = a.invoiceno 
			left join ommstmodel c on c.companycode = a.companycode 
				and c.salesmodelcode = a.salesmodelcode	
			where a.companycode = '''+@companycode+''' 
				and b.status in (2,5)
				and convert(varchar, b.invoicedate, 112) between '''+@period1+''' and '''+@period2+'''
			group by a.BranchCode, a.salesmodelcode
			union
			select cast(right(e.branchcode,2) as varchar) code
				, ltrim(e.salesmodelcode) model
				, isnull(sum(e.quantity),0)*-1 nilai
			from omtrsalesreturn d
			left join omtrsalesreturndetailmodel e on d.companycode = e.companycode 
				and d.branchcode = e.branchcode
				and d.returnno = e.returnno 
			left join ommstmodel c on c.companycode = e.companycode 
				and c.salesmodelcode = e.salesmodelcode	
			where d.companycode = '''+@companycode+''' 
				and d.status in (2,5)
				and convert(varchar, d.returndate, 112) between '''+@period1+''' and '''+@period2+'''
			group by e.BranchCode, e.salesmodelcode
		) x
		group by code,model
	)p pivot
	(
	sum(nilai) for code in ('+@columns+')
	) as pvt 
	'
	print (@query)
	exec (@query) 
	end
	
	
	-- Table IV --
	select distinct cast(right(branchcode,2) as varchar)+' : '+branchname keterangan from gnmstorganizationdtl 
		where companycode = @companycode

	-- Table V -- Total Per Branch
	set @query = '
	select '+@columns2+'
	from (
		select code,sum(nilai) nilai
		from (
			select cast(right(a.branchcode,2) as varchar) code
				, ltrim(c.salesmodeldesc) model
				, isnull(count(a.invoiceno),0) nilai
			from omtrsalesinvoicevin a
			inner join omtrsalesinvoice b on b.companycode = a.companycode 
				and b.branchcode = a.branchcode
				and b.invoiceno = a.invoiceno 
			left join ommstmodel c on c.companycode = a.companycode 
				and c.salesmodelcode = a.salesmodelcode	
			where a.companycode = '''+@companycode+''' 
				and b.status in (2,5)
				and convert(varchar, b.invoicedate, 112) between '''+@period1+''' and '''+@period2+'''
			group by a.BranchCode, a.salesmodelcode, c.salesmodeldesc
			union ALL
			select cast(right(e.branchcode,2) as varchar) code
				, ltrim(c.salesmodeldesc) model
				, isnull(sum(e.quantity),0)*-1 nilai
			from omtrsalesreturn d
			left join omtrsalesreturndetailmodel e on d.companycode = e.companycode 
				and d.branchcode = e.branchcode
				and d.returnno = e.returnno 
			left join ommstmodel c on c.companycode = e.companycode 
				and c.salesmodelcode = e.salesmodelcode	
			where d.companycode = '''+@companycode+''' 
				and d.status in (2,5)
				and convert(varchar, d.returndate, 112) between '''+@period1+''' and '''+@period2+'''
			group by e.BranchCode, e.salesmodelcode, c.salesmodeldesc
		) x
		group by code
	)p pivot
	(
	sum(nilai) for code in ('+@columns+')
	) as pvt 
	'
	print (@query)
	exec (@query)
END

GO

if object_id('usprpt_OmRpSalRgs033D') is not null
	drop procedure usprpt_OmRpSalRgs033D
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usprpt_OmRpSalRgs033D]
	(
	@companycode varchar(max),
	@branchcode varchar(max),
	@period1 varchar(max),
	@period2 varchar(max),
	@ID int,
	@tipe varchar(1)
	)
AS
BEGIN
	
	if(@tipe='A')
		begin
			--table 1--
			insert into TempDailySales1
			select @ID, CompanyCode Company, BranchCode Branch, BranchName 
			from gnmstorganizationdtl where companycode = @companycode order by BranchCode;

			--table 2--
			insert into TempDailySales2
			select @ID ,a.CompanyCode Company
							, a.branchcode Branch
							, ltrim(isnull(c.salesmodeldesc,'')) Model
							, isnull(count(a.invoiceno),0) Nilai
						from omtrsalesinvoicevin a
						inner join omtrsalesinvoice b on b.companycode = a.companycode 
							and b.branchcode = a.branchcode
							and b.invoiceno = a.invoiceno 
						left join ommstmodel c on c.companycode = a.companycode 
							and c.salesmodelcode = a.salesmodelcode	
						where a.companycode = @companycode
							and b.status in (2,5)
							and convert(varchar, b.invoicedate, 112) between @period1 and @period2
						group by a.CompanyCode, a.BranchCode, ltrim(isnull(c.salesmodeldesc,''))
						union all
						select @ID,d.CompanyCode Company
							, e.branchcode Branch
							, ltrim(isnull(c.salesmodeldesc,'')) Model
							, isnull(sum(e.quantity),0)*-1 Nilai
						from omtrsalesreturn d
						left join omtrsalesreturndetailmodel e on d.companycode = e.companycode 
							and d.branchcode = e.branchcode
							and d.returnno = e.returnno 
						left join ommstmodel c on c.companycode = e.companycode 
							and c.salesmodelcode = e.salesmodelcode	
						where d.companycode =  @companycode
							and d.status in (2,5)
							and convert(varchar, d.returndate, 112) between @period1 and @period2
						group by d.CompanyCode, e.BranchCode, ltrim(isnull(c.salesmodeldesc,'')) ;
			end
	else
		begin
			--table 1--
			insert into TempDailySales1
			select @ID, CompanyCode Company, BranchCode Branch, BranchName 
			from gnmstorganizationdtl where companycode = @companycode order by BranchCode;

			--table 2--
			insert into TempDailySales2
			select @ID,a.CompanyCode Company
                        , a.branchcode Branch
                        , ltrim(a.salesmodelcode) Model
                        , isnull(count(a.invoiceno),0) Nilai
                    from omtrsalesinvoicevin a
                    inner join omtrsalesinvoice b on b.companycode = a.companycode 
                        and b.branchcode = a.branchcode
                        and b.invoiceno = a.invoiceno 
                    left join ommstmodel c on c.companycode = a.companycode 
                        and c.salesmodelcode = a.salesmodelcode	
                    where a.companycode = @companycode
                        and b.status in (2,5)
                        and convert(varchar, b.invoicedate, 112)  between @period1 and @period2
                    group by a.CompanyCode, a.BranchCode, ltrim(a.salesmodelcode)
                    union all
                    select @ID , d.CompanyCode Company
                        , e.branchcode Branch
                        , ltrim(e.salesmodelcode) Model
                        , isnull(sum(e.quantity),0)*-1 Nilai
                    from omtrsalesreturn d
                    left join omtrsalesreturndetailmodel e on d.companycode = e.companycode 
                        and d.branchcode = e.branchcode
                        and d.returnno = e.returnno 
                    left join ommstmodel c on c.companycode = e.companycode 
                        and c.salesmodelcode = e.salesmodelcode	
                    where d.companycode = @companycode
                        and d.status in (2,5)
                        and convert(varchar, d.returndate, 112) between @period1 and @period2
                    group by d.CompanyCode, e.BranchCode, ltrim(e.salesmodelcode)  ;
		end

	-- TABLE I --
	select companygovname company, isnull(address1,'-') addr1, isnull(address2,'-') addr2, isnull(address3,'-') addr3, phoneno telp 
	from gnmstcoprofile where companycode = @companycode and branchcode = @branchcode

	-- TABLE II --
	select * into #tBranch from (
	select distinct companycode company, branchcode branch from gnmstorganizationdtl where companycode = @companycode
	union all
	select company, branch from TempDailySales1 where ID=@ID) #tBranch
	
	select * from #tBranch order by company, branch 
	
	declare @tabModel AS table
	(
		company varchar(50),
		branch varchar(50),
		model varchar(max),
		nilai int
	)
	
	if (@tipe ='A')
	begin
		insert into @tabModel 
			select a.CompanyCode company
				, a.branchcode branch
				, ltrim(isnull(c.salesmodeldesc,'')) model
				, isnull(count(a.invoiceno),0) nilai
			from omtrsalesinvoicevin a
			inner join omtrsalesinvoice b on b.companycode = a.companycode 
				and b.branchcode = a.branchcode
				and b.invoiceno = a.invoiceno 
			left join ommstmodel c on c.companycode = a.companycode 
				and c.salesmodelcode = a.salesmodelcode	
			where a.companycode = @companycode
				and b.status in (2,5)
				and convert(varchar, b.invoicedate, 112) between @period1 and @period2
			group by a.CompanyCode, a.BranchCode, ltrim(isnull(c.salesmodeldesc,''))
			union all
			select d.CompanyCode company
				, e.branchcode branch
				, ltrim(isnull(c.salesmodeldesc,'')) model
				, isnull(sum(e.quantity),0)*-1 nilai
			from omtrsalesreturn d
			left join omtrsalesreturndetailmodel e on d.companycode = e.companycode 
				and d.branchcode = e.branchcode
				and d.returnno = e.returnno 
			left join ommstmodel c on c.companycode = e.companycode 
				and c.salesmodelcode = e.salesmodelcode	
			where d.companycode = @companycode
				and d.status in (2,5)
				and convert(varchar, d.returndate, 112) between @period1 and @period2
			group by d.CompanyCode, e.BranchCode, ltrim(isnull(c.salesmodeldesc,''))
				
		insert into @tabModel
			select company, branch, model, nilai from TempDailySales2 where ID=@ID		
	end
	else
	begin
		insert into @tabModel 
			select a.CompanyCode company
				, a.branchcode branch
				, ltrim(a.salesmodelcode) model
				, isnull(count(a.invoiceno),0) nilai
			from omtrsalesinvoicevin a
			inner join omtrsalesinvoice b on b.companycode = a.companycode 
				and b.branchcode = a.branchcode
				and b.invoiceno = a.invoiceno 
			left join ommstmodel c on c.companycode = a.companycode 
				and c.salesmodelcode = a.salesmodelcode	
			where a.companycode = @companycode
				and b.status in (2,5)
				and convert(varchar, b.invoicedate, 112) between @period1 and @period2
			group by a.CompanyCode, a.BranchCode, ltrim(a.salesmodelcode)
			union all
			select d.CompanyCode company
				, e.branchcode branch
				, ltrim(e.salesmodelcode) model
				, isnull(sum(e.quantity),0)*-1 nilai
			from omtrsalesreturn d
			left join omtrsalesreturndetailmodel e on d.companycode = e.companycode 
				and d.branchcode = e.branchcode
				and d.returnno = e.returnno 
			left join ommstmodel c on c.companycode = e.companycode 
				and c.salesmodelcode = e.salesmodelcode	
			where d.companycode = @companycode
				and d.status in (2,5)
				and convert(varchar, d.returndate, 112) between @period1 and @period2
			group by d.CompanyCode, e.BranchCode, ltrim(e.salesmodelcode)
	
		insert into @tabModel
			select company, branch, model, nilai from TempDailySales2 where ID=@ID
	end
	
	select * into #tModel from (
		select * from @tabModel 
	) #tModel	
	
	-- Per Branch --
	----------------
	declare @1 varchar(max)
	declare @2 int
	declare @seq int
	declare @columns varchar(max)
	
	declare @colResult varchar(max)
	declare @columnsResult2 varchar(max)
	
	declare @columns2 varchar(max)
	declare @columns3 varchar(max)
	declare @columns4 varchar(max)
	declare @query varchar(max)

	set @columns = ''
	set @columns2 = ''
	set @columns3 = ''
	set @columns4 = ''

	set @2 = (select count(*) from #tBranch)
	set @seq = 1

	declare data cursor for
		select distinct branch from #tBranch
	open data
	fetch next from data into @1
	while @@fetch_status = 0 
	begin
		set @columns = @columns+'['+@1+']'	
		set @columns2 = @columns2+'isnull(['+@1+'],0) ['+@1+']'
		set @columns3 = @columns3+'isnull(['+@1+'],0)'
		set @columns4 = @columns4+'isnull(a.['+@1+'],0)'

		set @seq = @seq+1
		if (@seq <= @2)
		begin
			set @columns = @columns+','
			set @columns2 = @columns2+', '
			set @columns3 = @columns3+'+'
			set @columns4 = @columns4+'+'
		end	 
		fetch next from data into @1
	end
	close data
	deallocate data;

	--set @columns2 = @columns3
	set @columns2 = @columns3+' AS GRAND TOTAL'+','+@columns2
	
	-- Per Company --
	-----------------
	declare @columnsSub varchar(max)
	declare @columnsSubDesc varchar(max)
	declare @columnsSub2 varchar(max)
	
	set @columnsSub = ''
	set @columnsSubDesc = ''
	set @columnsSub2 = ''
	
	set @2 = (select count( distinct company) from #tBranch)
	set @seq = 1

	declare dataCom cursor for
		select distinct company from #tBranch
	open dataCom
	
	fetch next from dataCom into @1
	while @@fetch_status = 0 
	begin
		set @columnsSub = @columnsSub+'['+@1+']'	
		set @columnsSubDesc = @columnsSubDesc+'['+@1+'] as Total ('+@1+')'	
		set @columnsSub2 = @columnsSub2+'isnull(['+@1+'],0) ['+@1+']'

		set @seq = @seq+1
		if (@seq <= @2)
		begin
			set @columnsSub = @columnsSub+', '
			set @columnsSubDesc = @columnsSubDesc+', '
			set @columnsSub2 = @columnsSub2+', '
		end	 
		fetch next from dataCom into @1
	end
	close dataCom
	deallocate dataCom
	
	declare data1 cursor for
		select distinct company from #tBranch order by company
	
	declare @var1 varchar(max)
	declare @var2 varchar(max)
	set @colResult=''
	
	open data1
	fetch next from data1 into @var1
	while @@fetch_status=0
	begin
		declare @tot int
		set @tot = (select count(branch) from #tBranch where company=@var1)		
		set @seq=1
		
		declare data2 cursor for
			select branch from #tBranch where company=@var1 order by branch			
		open data2
		fetch next from data2 into @var2
		while @@fetch_status=0
		begin
			set @colResult = @colResult+'a.['+@var2+']'
			
			set @seq=@seq+1			
			if (@seq<=@tot)
				set @colresult=@colresult+','
			else
				set @colresult=@colresult+',b.['+@var1+'] as TOTAL ['+@var1+'],'
				
			fetch next from data2 into @var2
		end
		close data2
		deallocate data2		
		fetch next from data1 into @var1
	end
	
	close data1
	deallocate data1
	
	set @colResult = (select left(@colResult,len(@colResult)-1))
	
	-- TABLE III --	
	set @query = '
	select * into #tbl1 from (
	select model, '''+@columns2+null+'''
	from (
		select branch, model, sum(nilai) nilai
		from (
			select * from #tModel
		) x
		group by branch, model
	)p pivot
	(
		sum(nilai) for branch in ('''+@columns+null+''')
	)AS pvt 
	)#tbl1	
	
	select * into #tbl2 from (
	select model, '''+@columnsSub2+'''
	from (
		select company, model, sum(nilai) nilai
		from (
			select * from #tModel
		) x
		group by company, model 
	)p pivot
	(
		sum(nilai) for company in ('''+@columnsSub+''')
	) AS pvt 
	)#tbl2
	
	select a.model, '''+@columns4+''' as ''GRAND TOTAL'''+', '''+@colResult+'''
	from #tbl1 a
	left join #tbl2 b on a.model=b.model	
	
	drop table #tbl1
	drop table #tbl2
	'
	print (@query)
	exec (@query)
	
	-- Table IV --
	select branchcode+' : '+branchname keterangan from gnmstorganizationdtl 
		where companycode = @companycode
	union
	select Branch+' : '+BranchName keterangan from TempDailySales1 

	-- Table V -- Total Per Branch
	set @query = 'select '''+@columns2+null+'''
	from (
		select branch, sum(nilai) nilai
		from (
			select * from #tModel
		) x
		group by branch
	)p pivot
	(
	sum(nilai) for branch in ('''+@columns+null+''')
	) AS pvt '
	print (@query)
	exec (@query)
	
	-- Table VI --
	select company, sum(nilai) nilai from #tModel group by company order by company
	
	drop table #tBranch
	drop table #tModel
	
	delete TempDailySales1 where ID=@ID
	delete TempDailySales2 where ID=@ID

END


GO

if object_id('usprpt_SpCheckType') is not null
	drop procedure usprpt_SpCheckType
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE usprpt_SpCheckType
	@Type varchar(50)
AS
BEGIN
	declare @Query varchar(MAX)
	set @Query='
	select LookUpValue,LookUpValueName  
	from GnMstLookUpDtl 
	where CodeID=''GTGO'' and LookUpValue in ('+@Type+')'

	print(@Query)
	exec(@Query)
END

GO

if object_id('usprpt_SpRpSum024_Rev01') is not null
	drop procedure usprpt_SpRpSum024_Rev01
GO
-- =========================================================================
-- Author:		Seandy A.K, Revision By: Osen Kusnadi (Convert to PivotGrid)
-- Create date: 19-7-2012, Revision On 2014-07-01
-- Description:	Sparepart Analysis Report
-- Query Activation : usprpt_SpRpSum024_Rev01 '','','','2012','1'
-- exec usprpt_SpRpSum024_Rev01 @Area=N'',@CompanyCode=N'', 
-- @BranchCode=N'',@Year=N'2013',@S1=N'''5'',''0'',''1'',''2'',''3'',''4'''
-- =========================================================================
CREATE procedure [dbo].[usprpt_SpRpSum024_Rev01]
	@Area			varchar(100),
	@CompanyCode	varchar(15),
	@BranchCode		varchar(15),
	@Year			varchar(4),
	@S1				varchar(50)
AS
BEGIN

declare @Query varchar(MAX), @Query1 varchar(MAX), @Query2 varchar(MAX)
declare @code varchar(15), @name varchar(250), @nCount INT, @nNum INT, @sDate varchar(10)

CREATE TABLE #TmpFilterEnum(
	[display]	[varchar](120),
	[value]		[varchar](32),
	[iconCls]	[varchar](22),
	[type]		[varchar](12) 
)

CREATE TABLE #TmpSalesSummary(
	id int,
	AccountID int,
	Account varchar(32),
	BranchCode varchar(15),
	BranchName varchar(64),
	[JAN] [decimal](18, 2) NOT NULL DEFAULT(0),
	[FEB] [decimal](18, 2) NOT NULL DEFAULT(0),
	[MAR] [decimal](18, 2) NOT NULL DEFAULT(0),
	[APR] [decimal](18, 2) NOT NULL DEFAULT(0),
	[MAY] [decimal](18, 2) NOT NULL DEFAULT(0),
	[JUN] [decimal](18, 2) NOT NULL DEFAULT(0),
	[JUL] [decimal](18, 2) NOT NULL DEFAULT(0),
	[AUG] [decimal](18, 2) NOT NULL DEFAULT(0),
	[SEP] [decimal](18, 2) NOT NULL DEFAULT(0),
	[OCT] [decimal](18, 2) NOT NULL DEFAULT(0),
	[NOV] [decimal](18, 2) NOT NULL DEFAULT(0),
	[DEC] [decimal](18, 2) NOT NULL DEFAULT(0)
)

CREATE TABLE #RawData(
	PeriodYear int,
	PeriodMonth int,
	BranchCode varchar(15),
	[N01] [decimal](18, 2) NOT NULL DEFAULT(0),
	[N02] [decimal](18, 2) NOT NULL DEFAULT(0),
	[N03] [decimal](18, 2) NOT NULL DEFAULT(0),
	[N04] [decimal](18, 2) NOT NULL DEFAULT(0),
	[N05] [decimal](18, 2) NOT NULL DEFAULT(0),
	[N06] [decimal](18, 2) NOT NULL DEFAULT(0),
	[N07] [decimal](18, 2) NOT NULL DEFAULT(0),
	[N08] [decimal](18, 2) NOT NULL DEFAULT(0),
	[N09] [decimal](18, 2) NOT NULL DEFAULT(0),
	[N10] [decimal](18, 2) NOT NULL DEFAULT(0),
	[N11] [decimal](18, 2) NOT NULL DEFAULT(0),
	[N12] [decimal](18, 2) NOT NULL DEFAULT(0),
	[N13] [decimal](18, 2) NOT NULL DEFAULT(0),
	[N14] [decimal](18, 2) NOT NULL DEFAULT(0),
	[N15] [decimal](18, 2) NOT NULL DEFAULT(0),
	[N16] [decimal](18, 2) NOT NULL DEFAULT(0),
	[N17] [decimal](18, 2) NOT NULL DEFAULT(0),
	[N18] [decimal](18, 2) NOT NULL DEFAULT(0),
	[N19] [decimal](18, 2) NOT NULL DEFAULT(0),
	[N20] [decimal](18, 2) NOT NULL DEFAULT(0),
	[N21] [decimal](18, 2) NOT NULL DEFAULT(0),
	[N22] [decimal](18, 2) NOT NULL DEFAULT(0),
	[N23] [decimal](18, 2) NOT NULL DEFAULT(0),
	[N24] [decimal](18, 2) NOT NULL DEFAULT(0)
)

-- STARTING TO PREPARE METADATA --

set @Query = '
	select distinct 
	a.BranchCode,
	b.OutletAbbreviation, ''active'',''outlet''
	from spHstSparePartAnalysis a
	left join GnMstDealerMapping c on a.CompanyCode = c.DealerCode
	left join GnMStDealerOutletMapping b on a.CompanyCode = b.DealerCode
		and a.BranchCode = b.OutletCode
	where a.PeriodYear = ''' + @Year + ''' and convert(varchar,a.TypeOfGoods) in (' + @S1 + ') '

	if @Area <> '' 
	begin
		IF (@Area = 'JABODETABEK' or @Area = 'CABANG')
			set @Query = @Query + ' AND c.Area IN (''JABODETABEK'',''CABANG'') '
		else
			set @Query = @Query + ' AND c.Area like ''' + @Area + ''' '
	end

	if @CompanyCode <> ''
	BEGIN
		set @Query = @Query + ' AND a.CompanyCode like ''' + @CompanyCode + ''' '
	END

	IF @BranchCode <> ''
	BEGIN
		set @Query = @Query + ' AND a.BranchCode like ''' + @BranchCode + ''' '
	END
									
	set @Query = @Query + ' order by a.BranchCode '

	select @Query1 = 'INSERT INTO #TmpFilterEnum ([value],display,iconCls,[type])  ' + @Query , @nNum=0

	print(@Query1)
	exec(@Query1)


WHILE @nNum < 12
BEGIN
	SET @nNum=@nNum+1
	SET @sDate = @Year + '-' +  RIGHT('0' + convert(varchar,@nnum),2) + '-01'
	SET @name = upper(datename(mm,@sDate))
	SET @code = SUBSTRING(@name,1,3)
	INSERT INTO #TmpFilterEnum ([value],display,iconCls,[type]) values ( @code,@name + ', ' + @Year,'active','period')	
END
-- PREPARING METADATA DONE

-- PREPARING ACCOUNT LIST --
DECLARE @AccountList table(id INT,name VARCHAR(32))
INSERT INTO @AccountList VALUES 
(1,'Penjualan Kotor'),(2,'Penjualan Bersih'),(3,'Penjualan ke 3S + 2S'),(4,'Penjualan ke Parts Shop'),
(5,'Penjualan ke Lain-lain'),(6,'Harga Pokok'),(7,'Penerimaan Pembelian'),(8,'Nilai Stock'),(9,'ITO'),
(10,'ITO (AVG)'),(11,'Ratio'),(12,'Ration Suzuki'),(13,'Demand Line'),(14,'Demand Qty'),
(15,'Demand Nilai'),(16,'Supply Line'),(17,'Supply Qty'),(18,'Supply Nilai'),(19,'Service Ratio Line'),
(20,'Service Ratio Qty'),(21,'Service Ratio Nilai'),(22,'Stock MC4'),(23,'Stock MC5'),(24,'Slow Moving')
-- ACCOUNT LIST DONE --

-- PREPARING PIVOT GRID --
select @Query1 = 'DECLARE BRANCH_CR CURSOR FOR ' + @Query
print(@Query1)
exec(@Query1)

DECLARE ACCOUNT_CR CURSOR FOR SELECT ID,NAME FROM @AccountList
DECLARE @gBranchCode varchar(15), @gBranchName varchar(108), @iconCls varchar(7), @gType varchar(17)

SET @nNum = 0

OPEN ACCOUNT_CR
FETCH NEXT FROM ACCOUNT_CR
INTO @code, @name

WHILE @@FETCH_STATUS = 0
BEGIN

	OPEN BRANCH_CR
	FETCH NEXT FROM BRANCH_CR
	INTO  @gBranchCode,@gBranchName,@iconCls,@gType 

	WHILE @@FETCH_STATUS = 0
	BEGIN
		
		SET @nNum = @nNum + 1

		INSERT INTO #TmpSalesSummary(id,AccountID,Account,BranchCode,BranchName)
		VALUES (@nNum,@code, @name,@gBranchCode,@gBranchName)

		FETCH NEXT FROM BRANCH_CR 
		INTO @gBranchCode,@gBranchName,@iconCls,@gType 
	END

	CLOSE BRANCH_CR

	FETCH NEXT FROM ACCOUNT_CR 
	INTO @code, @name
END

CLOSE ACCOUNT_CR
DEALLOCATE ACCOUNT_CR
DEALLOCATE BRANCH_CR

-- PIVOT GRID TEMPLATE DONE --

-- STARTING TO PREPARE DATA SUMMARY
set @Query = ' select * into #Summary from(
select c.GroupNo
		, c.Area
		, a.CompanyCode
		, c.DealerAbbreviation
		, a.BranchCode
		, b.OutletAbbreviation
		, a.PeriodYear
		, a.PeriodMonth
		, JumlahJaringan
		, SUM(isnull(PenjualanKotor,0)) PenjualanKotor
		, SUM(isnull(PenjualanBersih,0)) PenjualanBersih
		, isnull((select SUM(isnull(PenjualanBersih,0))
			 from spHstSparePartAnalysisDtl
			where CompanyCode = a.CompanyCode
			  and BranchCode = a.BranchCode
			  and PeriodYear = a.PeriodYear
			  and PeriodMonth = a.PeriodMonth
			  and TypeOfGoods = a.TypeOfGoods
			  and CustomerCategory in (''00'',''01'')),0) Penjualan3S2S
		, isnull((select SUM(isnull(PenjualanBersih,0))
			 from spHstSparePartAnalysisDtl
			where CompanyCode = a.CompanyCode
			  and BranchCode = a.BranchCode
			  and PeriodYear = a.PeriodYear
			  and PeriodMonth = a.PeriodMonth
			  and TypeOfGoods = a.TypeOfGoods
			  and CustomerCategory in (''15'')),0) PenjualanPartShop
		, isnull((select SUM(isnull(PenjualanBersih,0))
			 from spHstSparePartAnalysisDtl
			where CompanyCode = a.CompanyCode
			  and BranchCode = a.BranchCode
			  and PeriodYear = a.PeriodYear
			  and PeriodMonth = a.PeriodMonth
			  and TypeOfGoods = a.TypeOfGoods
			  and CustomerCategory not in (''00'',''01'',''15'')),0) PenjualanOthers
		, SUM(isnull(HargaPokok,0)) HargaPokok
		, SUM(isnull(PenerimaanPembelian,0))PenerimaanPembelian
		, SUM(isnull(NilaiStock,0)) NilaiStock
		, (select SUM(isnull(e.NilaiStock,0))
				 from spHstSparepartAnalysis e
				where e.CompanyCode = a.CompanyCode
				  and e.BranchCode = a.BranchCode
				  and e.PeriodYear = a.PeriodYear
				  and e.PeriodMonth = a.PeriodMonth
				  '
set @Query = @Query + ' and convert(varchar,e.TypeOfGoods) in (' + @S1 + ')
				Group by CompanyCode, BranchCode,PeriodYear,PeriodMonth
				  ) test
		, case when SUM(isnull(HargaPokok,0)) = 0 
				then 0
				else (SUM(isnull(NilaiStock,0)) / case when SUM(isnull(HargaPokok,0)) = 0 then 1 else SUM(isnull(HargaPokok,0)) end) 
				end ITO
		, isnull((select case when SUM(isnull(d.HargaPokok,0)) = 0 
				then 0
				else (SUM(isnull(a.NilaiStock,0))
					/ (SUM(isnull(d.HargaPokok,0)) / (select distinct Count(d.PeriodYear)
					from spHstSparepartAnalysis e
								where e.CompanyCode = a.CompanyCode
								  and e.BranchCode = a.BranchCode
								  and convert(int,convert(varchar,e.PeriodYear) + RIGHT(''0'' + convert(varchar,e.PeriodMonth),2))
										between convert(int,convert(varchar, case when a.PeriodMonth < 7 
																	  then  
																			(a.PeriodYear - 1)
																	  else 
																			a.PeriodYear
																	  end) +
												RIGHT(''0'' + convert(varchar, case when a.PeriodMonth < 7
																	  then 
																		(12 + (a.PeriodMonth - 5))
																	  else
																		(a.PeriodMonth - 5)
																	  end), 2))
										and convert(int,convert(varchar,a.PeriodYear) + RIGHT(''0'' + convert(varchar,a.PeriodMonth),2))
										'
					set @Query = @Query + ' and TypeOfGoods in (' + @S1 + ') Group by CompanyCode,BranchCode)
					 ))
				end
			from spHstSparepartAnalysis d
			where d.CompanyCode = a.CompanyCode
			  and d.BranchCode = a.BranchCode
			  and convert(int,convert(varchar,d.PeriodYear) + RIGHT(''0'' + convert(varchar,d.PeriodMonth),2))
										between convert(int,convert(varchar, case when a.PeriodMonth < 7 
																	  then  
																			(a.PeriodYear - 1)
																	  else 
																			a.PeriodYear
																	  end) +
												RIGHT(''0'' + convert(varchar, case when a.PeriodMonth < 7
																	  then 
																		(12 + (a.PeriodMonth - 5))
																	  else
																		(a.PeriodMonth - 5)
																	  end), 2))
										and convert(int,convert(varchar,a.PeriodYear) + RIGHT(''0'' + convert(varchar,a.PeriodMonth),2))
										'
					set @Query = @Query + ' and d.TypeOfGoods in (' + @S1 + ') Group by CompanyCode,BranchCode
		  ),0) AVGITO
		, SUM(isnull(PenjualanBersih,0) - isnull(HargaPokok,0)) / case when SUM(isnull(HargaPokok,0)) = 0 then 1 else SUM(HargaPokok) end * 100 Ratio
		, SUM(isnull(PenjualanBersih,0) - isnull(HargaPokok,0)) / case when SUM(isnull(PenjualanBersih,0)) = 0 then 1 else SUM(PenjualanBersih) end * 100 RatioSuzuki
		, SUM(isnull(DemandLine,0)) DemandLine
		, SUM(isnull(DemandQuantity,0)) DemandQuantity
		, SUM(isnull(DemandNilai,0)) DemandNilai
		, SUM(isnull(SupplyLine,0)) SupplyLine
		, SUM(isnull(SupplyQuantity,0)) SupplyQuantity
		, SUM(isnull(SupplyNilai,0)) SupplyNilai
		, (SUM(isnull(SupplyLine,0)) / case when SUM(isnull(DemandLine,0)) = 0 then 1 else SUM(isnull(DemandLine,0)) end) * 100 ServiceRatioLine
		, (SUM(isnull(SupplyQuantity,0)) / case when SUM(isnull(DemandQuantity,0)) = 0 then 1 else SUM(isnull(DemandQuantity,0)) end) * 100 ServiceRatioQuantity
		, (SUM(isnull(SupplyNilai,0)) / case when SUM(isnull(DemandNilai,0)) = 0 then 1 else SUM(isnull(DemandNilai,0)) end) * 100 ServiceRatioNilai
		, SUM(isnull(DataStockMC4,0)) DataStockMC4
		, SUM(isnull(DataStockMC5,0)) DataStockMC5
		, (SUM(isnull(SlowMoving,0)) / case when SUM(isnull(NilaiStock,0)) = 0 then 1 else SUM(isnull(NilaiStock,0)) end) * 100 SlowMoving
	from spHstSparePartAnalysis a
	left join GnMstDealerMapping c on a.CompanyCode = c.DealerCode
	left join GnMStDealerOutletMapping b on a.CompanyCode = b.DealerCode
		and a.BranchCode = b.OutletCode
	where (c.Area like '
	set @Query = @Query + Case when @Area <> '' 
								then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
										then '''JABODETABEK'''
										else @Area end
								else '''%%''' end
	set @Query = @Query + ' or c.Area like ' + Case when @Area <> '' 
													then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
															then '''CABANG'''
															else @Area end
													else '''%%''' end
set @Query = @Query + ') and a.CompanyCode like ' 
set @Query = @Query + case when @CompanyCode <> '' then '''' + @CompanyCode + '''' else '''%%''' end
set @Query = @Query + ' and a.BranchCode like '
set @Query = @Query + case when @BranchCode <> '' then '''' + @BranchCode + '''' else '''%%''' end
set @Query = @Query + ' and a.PeriodYear = ''' + @Year + ''''
set @Query = @Query + ' and convert(varchar,a.TypeOfGoods) in (' + @S1 + ')'
set @Query = @Query + ' group by c.GroupNo
		, c.Area
		, a.CompanyCode
		, c.DealerAbbreviation
		, a.BranchCode
		, b.OutletAbbreviation
		, a.PeriodYear
		, a.PeriodMonth
		, a.JumlahJaringan
		, a.TypeOfGoods
)#Summary

select * into #PivotData from (
select PeriodYear
	, PeriodMonth
	, BranchCode
	, SUM(PenjualanKotor) PenjualanKotor
	, SUM(PenjualanBersih) PenjualanBersih
	, SUM(Penjualan3S2S) Penjualan3S2S
	, SUM(PenjualanPartShop) PenjualanPartShop
	, SUM(PenjualanOthers) PenjualanOthers
	, SUM(HargaPokok) HargaPokok
	, SUM(PenerimaanPembelian) PenerimaanPembelian
	, SUM(NilaiStock) NilaiStock
	, SUM(ITO) ITO
	, isnull((case when SUM(isnull(HargaPokok,0)) = 0 
				then 0
				else (SUM(isnull(NilaiStock,0))/ (SUM(isnull(HargaPokok,0)) / 6))
				end
		  ),0) AVGITO
	, SUM(isnull(PenjualanBersih,0) - isnull(HargaPokok,0)) / case when SUM(isnull(HargaPokok,0)) = 0 then 1 else SUM(HargaPokok) end * 100 Ratio
	, SUM(isnull(PenjualanBersih,0) - isnull(HargaPokok,0)) / case when SUM(isnull(PenjualanBersih,0)) = 0 then 1 else SUM(PenjualanBersih) end * 100 RatioSuzuki
	, SUM(DemandLine) DemandLine
	, SUM(DemandQuantity) DemandQuantity
	, SUM(DemandNilai) DemandNilai
	, SUM(SupplyLine) SupplyLine
	, SUM(SupplyQuantity) SupplyQuantity
	, SUM(SupplyNilai) SupplyNilai
	, (SUM(isnull(SupplyLine,0)) / case when SUM(isnull(DemandLine,0)) = 0 then 1 else SUM(isnull(DemandLine,0)) end) * 100 ServiceRatioLine
	, (SUM(isnull(SupplyQuantity,0)) / case when SUM(isnull(DemandQuantity,0)) = 0 then 1 else SUM(isnull(DemandQuantity,0)) end) * 100 ServiceRatioQuantity
	, (SUM(isnull(SupplyNilai,0)) / case when SUM(isnull(DemandNilai,0)) = 0 then 1 else SUM(isnull(DemandNilai,0)) end) * 100 ServiceRatioNilai
	, SUM(DataStockMC4) DataStockMC4
	, SUM(DataStockMC5) DataStockMC5
	, (SUM(isnull(SlowMoving,0)) / case when SUM(isnull(NilaiStock,0)) = 0 then 1 else SUM(isnull(NilaiStock,0)) end) * 100 SlowMoving
from #Summary
group by 
	 PeriodYear
	, PeriodMonth
	, BranchCode) a

insert into #RawData select * from #PivotData

'
print(@Query)
exec(@Query)

select * from #TmpFilterEnum
select* from #TmpSalesSummary
select* from #RawData





END


GO

if object_id('usprpt_SvRpCrm002V3') is not null
	drop procedure usprpt_SvRpCrm002V3
GO
-- ===================================================
-- Query Activation : 
-- usprpt_SvRpCrm002V2 '6006406','6006405','20110706','REMINDER',3,0
-- ===================================================

CREATE PROCEDURE [dbo].[usprpt_SvRpCrm002V3]
	@CompanyCode	Varchar(15),
	@BranchCode		Varchar(15),
	@DateParam		Datetime,
	@OptionType		Varchar(100), 
	@Range			int, 
	@Interval		int
	
AS
BEGIN
-- SET NOCOUNT ON added to prevent extra result sets from
-- interfering with SELECT statements.
SET NOCOUNT ON;

IF (@OptionType = 'REMINDER')
BEGIN
select ROW_NUMBER()OVER(order BY a.PoliceRegNo) as No, * from (
	select distinct 
		  case when b.LastServiceDate is null then 'New' else case when (convert(varchar, b.LastServiceDate, 112) = '19000101') then 'NEW' else 'Walk In' end end KategoriPelanggan
		 , b.BasicModel typeA
		 , b.PoliceRegNo
		 , case when b.TransmissionType is null then 'MT' else case when b.TransmissionType = ' ' then 'MT' else b.TransmissionType end end TM
		 , b.ProductionYear ProdYear
		 , b.EngineCode
		 , b.EngineNo
		 , b.ChassisCode
		 , b.ChassisNo
		 , c.CustomerName
		 , c.PhoneNo
		 , c.OfficePhoneNo
		 , c.HPNo
		 , case when (convert(varchar, b.LastServiceDate, 112) = '19000101') then null else b.LastServiceDate end VisitDate
		 , case when (convert(varchar, c.BirthDate, 112) = '19000101') then null else c.BirthDate end BirthDate
		 , case(c.Gender) when 'M' then 'Pria' else 'Wanita' end Gender
		 , isnull(c.Address1, '')+' '+isnull(c.Address2, '')+' '+isnull(c.Address3, '')+' '+isnull(c.Address4, '') Address
		 , case when b.ContactName is null then c.CustomerName else b.ContactName end ContactName
		 , case when b.ContactAddress is null then isnull(c.Address1, '')+' '+isnull(c.Address2, '')+' '+isnull(c.Address3, '')+' '+isnull(c.Address4, '') else b.ContactAddress end ContactAddress
		 , case when b.ContactPhone is null then c.PhoneNo else b.ContactPhone end ContactPhone
		 , b.RemainderDescription LastRemark
		 , ISNULL((SELECT TOP 1 CompanyName FROM gnMstCoProfile WHERE CompanyCode = a.CompanyCode and BranchCode = a.BranchCode) ,'') CompanyName
		 , YEAR(getdate()) PeriodYear
		 , DateName( month , DateAdd( month , month(GetDate()) , 0 ) - 1 ) PeriodMonth 
		 , c.Address1
		 , c.Address2
		 , c.Address3
		 , c.Address4
	  from svTrnService a
	  left join SvMstCustomerVehicle b	on a.Companycode = b.CompanyCode 
			and a.ChassisCode = b.ChassisCode 
			and a.ChassisNo = b.ChassisNo
	  left join GnMstCustomer c on a.CompanyCode = c.CompanyCode
			and a.CustomerCode = c.CustomerCode
	  left join svTrnDailyRetention d on d.CompanyCode = a.CompanyCode
			and d.BranchCode = a.BranchCode and d.PeriodYear = year(a.JobOrderDate)
			and d.PeriodMonth = month(a.JobOrderDate) and d.CustomerCode = a.CustomerCode
	  left join gnMstLookupDtl e on e.CompanyCode = a.CompanyCode
			and e.CodeID = 'CIRS'
			and e.LookupValue = d.VisitInitial
	  left join gnMstLookUpDtl f on b.CompanyCode = a.CompanyCode
			and f.CodeId = 'CCRS'
			and f.LookUpValue = d.CustomerCategory
	 where 1 = 1
	   and a.CompanyCode = @CompanyCode
	   and a.BranchCode	= @BranchCode
	   ) a
END
ELSE
BEGIN
select ROW_NUMBER()OVER(order BY b.PoliceRegNo) as No, * from (
	select distinct 
		case when b.LastServiceDate is null then 'New' else case when (convert(varchar, b.LastServiceDate, 112) = '19000101') then 'NEW' else 'Walk In' end end KategoriPelanggan
		 , b.BasicModel typeA
		 , b.PoliceRegNo
		 , case when b.TransmissionType is null then 'MT' else case when b.TransmissionType = ' ' then 'MT' else b.TransmissionType end end TM 
		 , b.ProductionYear ProdYear
		 , b.EngineCode
		 , b.EngineNo
		 , b.ChassisCode
		 , b.ChassisNo
		 , c.CustomerName
		 , c.PhoneNo
		 , c.OfficePhoneNo
		 , c.HPNo
		 , case when (convert(varchar, b.LastServiceDate, 112) = '19000101') then null else b.LastServiceDate end VisitDate
		 , case when (convert(varchar, c.BirthDate, 112) = '19000101') then null else c.BirthDate end BirthDate
		 , case(c.Gender) when 'M' then 'Pria' else 'Wanita' end Gender
		 , isnull(c.Address1, '')+' '+isnull(c.Address2, '')+' '+isnull(c.Address3, '')+' '+isnull(c.Address4, '') Address
		 , case when b.ContactName is null then c.CustomerName else b.ContactName end ContactName
		 , case when b.ContactAddress is null then isnull(c.Address1, '')+' '+isnull(c.Address2, '')+' '+isnull(c.Address3, '')+' '+isnull(c.Address4, '') else b.ContactAddress end ContactAddress
		 , case when b.ContactPhone is null then c.PhoneNo else b.ContactPhone end ContactPhone
		 , b.RemainderDescription LastRemark
		 , ISNULL((SELECT TOP 1 CompanyName FROM gnMstCoProfile WHERE CompanyCode = a.CompanyCode and BranchCode = a.BranchCode) ,'') CompanyName
		 , YEAR(getdate()) PeriodYear
		 , DateName( month , DateAdd( month , month(GetDate()) , 0 ) - 1 ) PeriodMonth
		 , c.Address1
		 , c.Address2
		 , c.Address3
		 , c.Address4
	  from svTrnService a
	  left join SvMstCustomerVehicle b	on a.Companycode = b.CompanyCode 
			and a.ChassisCode = b.ChassisCode 
			and a.ChassisNo = b.ChassisNo
	  left join GnMstCustomer c on a.CompanyCode = c.CompanyCode
			and a.CustomerCode = c.CustomerCode
	  left join svTrnDailyRetention d on d.CompanyCode = a.CompanyCode
			and d.BranchCode = a.BranchCode and d.PeriodYear = year(a.JobOrderDate)
			and d.PeriodMonth = month(a.JobOrderDate) and d.CustomerCode = a.CustomerCode
	  left join gnMstLookupDtl e on e.CompanyCode = a.CompanyCode
			and e.CodeID = 'CIRS'
			and e.LookupValue = d.VisitInitial
	  left join gnMstLookUpDtl f on b.CompanyCode = a.CompanyCode
			and f.CodeId = 'CCRS'
			and f.LookUpValue = d.CustomerCategory
	 where 1 = 1
	   and a.CompanyCode = @CompanyCode
	   and a.BranchCode	= @BranchCode
	   ) b
END
END

GO

if object_id('uspfn_GenerateSSPickingslipNew') is not null
	drop procedure uspfn_GenerateSSPickingslipNew
GO
create procedure [uspfn_GenerateSSPickingslipNew]
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

--set	@CompanyCode	= '6006400001'
--set	@BranchCode		= '6006400101'
--set	@JobOrderNo		= 'SPK/14/101589'
--set	@ProductType	= '4W'
--set	@CustomerCode	= '2105885'
--set	@TransType		= '20'
--set	@UserID			= '9a'
--set	@DocDate		= '3/2/2015 4:03:01 PM'

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
		, c.CostPrice
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
		LEFT JOIN SpMstItemPrice c WITH (NOLOCK, NOWAIT) ON a.CompanyCode = c.CompanyCode 
			AND a.BranchCode = c.BranchCode 
			AND a.PartNo = c.PartNo
	WHERE
		a.CompanyCode = @CompanyCode
		AND a.BranchCode = @BranchCode
		AND a.ProductType = @ProductType
		AND Item.QtyOrder > 0
		AND JobOrderNo = @JobOrderNo
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
	, @DocDate OrderNo
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
SET @Query = 
'SELECT PartNo, (Onhand - AllocationSP - AllocationSL - AllocationSR - ReservedSP - ReservedSL - ReservedSR) AvailStock
FROM ' + @DbMD + '..SpMstItemLoc WITH (NOLOCK, NOWAIT) 
WHERE CompanyCode = '+''''+@CompanyMD+''''+' AND BranchCode ='+''''+@BranchMD +''''+' AND WarehouseCode = '+''''+@WarehouseMD+''''+''

INSERT INTO @TempAvailStock
exec(@query)

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
	@CompanyCode CompanyCode
	, @BranchCode BranchCode
	, @TempDocNo DocNo 
	, a.PartNo
	, '00' WarehouseCode
	, a.PartNo PartNoOriginal
	, @JobOrderNo ReferenceNo
	, (SELECT JobOrderDate FROM SvTrnService WHERE 1 =1 AND CompanyCode = @CompanyCode AND BranchCode = @BranchCode 
		AND ProductType = @ProductType AND JobOrderNo = @JobOrderNo) ReferenceDate
	, (SELECT LocationCode FROM SpMstItemLoc WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND WarehouseCode = '00'
		AND PartNo = a.PartNo) LocationCode
	, a.QtyOrder
	, CASE WHEN 
		ISNULL((SELECT AvailStock FROM @TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN a.QtyOrder 
		ELSE ISNULL((SELECT AvailStock FROM @TempAvailStock WHERE PartNo = a.PartNo),0)
		END AS QtySupply
	, CASE WHEN 
		ISNULL((SELECT AvailStock FROM @TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN 0 
		ELSE a.QtyOrder - ISNULL((SELECT AvailStock FROM @TempAvailStock WHERE PartNo = a.PartNo),0)
		END AS QtyBO
	, 0 QtyBOSupply
	, CASE WHEN 
		ISNULL((SELECT AvailStock FROM @TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN 0 
		ELSE a.QtyOrder - ISNULL((SELECT AvailStock FROM @TempAvailStock WHERE PartNo = a.PartNo),0)
		END AS QtyBOCancel
	, 0 QtyBill
	, a.RetailPrice + FLOOR(a.RetailPrice * 10 /100) RetailPriceIncltax
	, a.RetailPrice
	, b.CostPrice
	, a.DiscPct
	, CASE WHEN 
		ISNULL((SELECT AvailStock FROM @TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN a.QtyOrder * a.RetailPrice
		ELSE ISNULL((SELECT AvailStock FROM @TempAvailStock WHERE PartNo = a.PartNo),0) * a.RetailPrice
		END AS SalesAmt
	, CASE WHEN 
		ISNULL((SELECT AvailStock FROM @TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN floor((a.QtyOrder * a.RetailPrice) * a.DiscPct/100)
		ELSE floor((ISNULL((SELECT AvailStock FROM @TempAvailStock WHERE PartNo = a.PartNo),0) * a.RetailPrice) * a.DiscPct/100)
		END AS DiscAmt
	, CASE WHEN 
		ISNULL((SELECT AvailStock FROM @TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN floor((a.QtyOrder * a.RetailPrice)- ((a.QtyOrder * a.RetailPrice) * a.DiscPct/100))
		ELSE floor((ISNULL((SELECT AvailStock FROM @TempAvailStock WHERE PartNo = a.PartNo),0) * a.RetailPrice) - 
			(ISNULL((SELECT AvailStock FROM @TempAvailStock WHERE PartNo = a.PartNo),0) * a.RetailPrice * a.DiscPct/100))
		END AS NetSalesAmt
	, 0 PPNAmt
	,  CASE WHEN 
		ISNULL((SELECT AvailStock FROM @TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN floor((a.QtyOrder * a.RetailPrice)- ((a.QtyOrder * a.RetailPrice) * a.DiscPct/100))
		ELSE floor((ISNULL((SELECT AvailStock FROM @TempAvailStock WHERE PartNo = a.PartNo),0) * a.RetailPrice) - 
			(ISNULL((SELECT AvailStock FROM @TempAvailStock WHERE PartNo = a.PartNo),0) * a.RetailPrice * a.DiscPct/100))
		END AS TotSalesAmt
	, (SELECT MovingCode FROM SpMstItems WITH (NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND PartNo = a.PartNo) 
		MovingCode
	, (SELECT ABCClass FROM SpMstItems WITH (NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND PartNo = a.PartNo) 
		ABCClass
	, @ProductType ProductType
	, (SELECT PartCategory FROM SpMstItems WITH (NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND PartNo = a.PartNo) 
		PartCategory
	, '2' Status
	, @UserID CreatedBy
	, GetDate() CreatedDate
	, @UserID LastUpdateBy
	, GetDate() LastUpdateDate
	, @UserID StockAllocatedBy
	, GetDate() StockAllocatedDate
	, a.QtyOrder FirstDemandQty
FROM #SrvOrder a
inner join spMstItemPrice b ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.PartNo = b.PartNo
WHERE a.TypeOfGoods = @TypeOfGoods

--===============================================================================================================================
-- INSERT SO SUPPLY
-- ==============================================================================================================================

SELECT * INTO #TempSOSupply FROM (
SELECT
	@CompanyCode CompanyCode
	, @BranchCode BranchCode
	, @TempDocNo DocNo 
	, 0 SupSeq
	, a.PartNo 
	, a.PartNo PartNoOriginal
	, '' PickingSlipNo	
	, @JobOrderNo ReferenceNo
	, @DefaultDate ReferenceDate
	, '00' WarehouseCode
	, (SELECT LocationCode FROM SpMstItemLoc WITH (NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND WarehouseCode = '00'
		AND PartNo = a.PartNo) LocationCode
	, CASE WHEN 
		ISNULL((SELECT AvailStock FROM @TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN a.QtyOrder 
		ELSE ISNULL((SELECT AvailStock FROM @TempAvailStock WHERE PartNo = a.PartNo),0)
		END AS QtySupply
	, 0 QtyPicked
	, 0 QtyBill
	, a.RetailPrice + FLOOR(a.RetailPrice *10 /100) RetailPriceIncltax
	, a.RetailPrice
	, b.CostPrice
	, a.DiscPct
	, (SELECT MovingCode FROM SpMstItems WITH (NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND PartNo = a.PartNo) 
		MovingCode
	, (SELECT ABCClass FROM SpMstItems WITH (NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND PartNo = a.PartNo) 
		ABCClass
	, @ProductType ProductType
	, (SELECT PartCategory FROM SpMstItems WITH (NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND PartNo = a.PartNo) 
		PartCategory
	, '1' Status
	, @UserID CreatedBy
	, GetDate() CreatedDate
	, @UserID LastUpdateBy
	, GetDate() LastUpdateDate
	, @UserID StockAllocatedBy
	, GetDate() StockAllocatedDate
FROM #SrvOrder a
inner join spMstItemPrice b WITH (NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.PartNo = b.PartNo
WHERE a.TypeOfGoods = @TypeOfGoods
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
	, LastUpdateBy = @UserID
	, LastUpdateDate = GetDate()
FROM SpHstDemandItem a, SpTrnSordDtl b
WHERE 
	1 = 1
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.Year = Year(GetDate())
	AND a.Month  = Month(GetDate())
	AND a.PartNo = b.PartNo
	AND b.DocNo = @TempDocNo

UPDATE SpHstDemandCust
SET DemandFreq = DemandFreq + 1
	, DemandQty = DemandQty + b.QtyOrder
	, LastUpdateBy = @UserID
	, LastUpdateDate = GetDate()
FROM SpHstDemandCust a, SpTrnSordDtl b
WHERE 
	1 = 1
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.Year = Year(GetDate())
	AND a.Month  = Month(GetDate())
	AND a.PartNo = b.PartNo
	AND a.CustomerCode = @CustomerCode
	AND b.DocNo = @TempDocNo

INSERT INTO SpHstDemandItem
SELECT 
	CompanyCode
	, BranchCode
	, Year(GetDate()) Year
	, Month(GetDate()) Month
	, PartNo
	, 1 DemandFreq
	, QtyOrder DemandQty
	, 0 SalesFreq
	, 0 SalesQty
	, MovingCode
	, ProductType
	, PartCategory
	, ABCClass
	, @UserID LastUpdateBy
	, GetDate() LastUpdateDate
FROM SpTrnSordDtl a WITH (NOLOCK, NOWAIT)
WHERE a.CompanyCode= @CompanyCode AND a.BranchCode= @BranchCode AND a.DocNo = @TempDocNo -- add CompanyCode and BranchCode 13 Des 2010
 AND NOT EXISTS
( SELECT 1 FROM SpHstDemandItem WITH (NOLOCK, NOWAIT) WHERE 
	1 = 1 
	AND CompanyCode = a.CompanyCode 
	AND BranchCode = a.BranchCode
	AND Month = Month(GetDate())
	AND Year = Year(GetDate())
	AND PartNo = a.PartNo
)

INSERT INTO SpHstDemandCust
SELECT 
	CompanyCode
	, BranchCode
	, Year(GetDate()) Year
	, Month(GetDate()) Month
	, @CustomerCode CustomerCode
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
	, @UserID LastUpdateBy
	, GetDate() LastUpdateDate
FROM SpTrnSordDtl a WITH (NOLOCK, NOWAIT)
WHERE a.CompanyCode= @CompanyCode and a.BranchCode= @BranchCode AND a.DocNo = @TempDocNo -- add CompanyCode and BranchCode 13 Des 2010
AND NOT EXISTS
( SELECT PartNo FROM SpHstDemandCust WITH (NOLOCK, NOWAIT) WHERE 
	1 = 1 
	AND CompanyCode = a.CompanyCode 
	AND BranchCode = a.BranchCode
	AND Month = Month(GetDate())
	AND Year = Year(GetDate())
	AND PartNo = a.PartNo
)

--===============================================================================================================================
-- UPDATE LAST DEMAND DATE MASTER
--===============================================================================================================================
UPDATE SpMstItems 
SET LastDemandDate = GetDate()
FROM SpMstItems a, SpTrnSordDtl b
WHERE 
	1 = 1
	AND a.CompanyCode = @CompanyMD
	AND a.BranchCode = @BranchMD
	AND a.PartNo = b.PartNo
	AND b.DocNo = @TempDocNo

--===============================================================================================================================
-- UPDATE STOCK AND MOVEMENT
--===============================================================================================================================

UPDATE spMstItems
SET AllocationSR = AllocationSR + b.QtySupply
	, LastUpdateBy = @UserID
	, LastUpdateDate = GetDate()
FROM SpMstItems a, #TempSOSupply b
WHERE 
	1 = 1
	AND a.CompanyCode = @CompanyMD
	AND a.BranchCode = @BranchMD
	AND a.PartNo = b.PartNo

UPDATE spMstItemloc
SET AllocationSR = AllocationSR + b.QtySupply
	, LastUpdateBy = @UserID
	, LastUpdateDate = GetDate()
FROM SpMstItemLoc a, #TempSOSupply b
WHERE 
	1 = 1
	AND a.CompanyCode = @CompanyMD 
	AND a.BranchCode = @BranchMD
	AND a.WarehouseCode = @WarehouseMD
	AND a.PartNo = b.PartNo

INSERT INTO SpTrnIMovement
SELECT
	@CompanyCode CompanyCode
	, @BranchCode BranchCode
	, a.DocNo
	, (SELECT DocDate FROM SpTrnSORDHdr WITH (NOLOCK, NOWAIT) WHERE CompanyCode = @CompanyCode 
		AND BranchCode = @BranchCode AND DocNo = a.DocNo) 
	  DocDate
	, dateadd(s,ROW_NUMBER() OVER(Order by a.PartNo),getdate()) CreatedDate 
	, '00' WarehouseCode
	, (SELECT LocationCode FROM SpTrnSORDDtl WITH (NOLOCK, NOWAIT) WHERE CompanyCode =  @CompanyCode 
		AND BranchCode = @BranchCode AND DocNo = @TempDocNo AND PartNo = a.PartNo)
	  LocationCode
	, a.PartNo
	, 'OUT' SignCode
	, 'SA-NPJUAL' SubSignCode
	, a.QtySupply
	, a.RetailPrice
	, a.CostPrice
	, a.ABCClass
	, a.MovingCode
	, a.ProductType
	, a.PartCategory
	, @UserID CreatedBy
FROM #TempSOSupply a

--===============================================================================================================================
-- UPDATE SUPPLY SLIP TO SPK
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
	AND a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND a.ProductType = @ProductType
	AND a.ServiceNo = @ServiceNo
	AND a.PartSeq IN (SELECT MAX(PartSeq) FROM SvTrnSrvItem WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode
		AND ProductType = @ProductType AND ServiceNo = a.ServiceNo AND PartNo = a.PartNo)
	AND (a.SupplySlipNo = '' OR a.SupplySlipNo IS NULL)
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
	AND a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND a.ProductType = @ProductType
	AND a.ServiceNo = @ServiceNo
	AND a.PartSeq IN (SELECT MAX(PartSeq) FROM SvTrnSrvItem WITH (NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode
		AND ProductType = @ProductType AND ServiceNo = a.ServiceNo AND PartNo = a.PartNo)
	AND (a.SupplySlipNo <> '' OR a.SupplySlipNo IS NOT NULL)
) #TempServiceItemIns

UPDATE svTrnSrvItem
SET SupplySlipNo = b.DocNo
	, SupplySlipDate = ISNULL((SELECT DocDate FROM SpTrnSORDHdr WITH (NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode
							AND DocNo = b.DocNo),@DefaultDate)
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
	, @UserID CreatedBy
	, GetDate() CreatedDate
	, @UserID LastUpdateBy
	, GetDate() LastUpdateDate
	, a.DiscPct
FROM #TempServiceItemIns a WITH (NOLOCK, NOWAIT)
WHERE
	1 = 1
	AND a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND a.ProductType = @ProductType	

--===============================================================================================================================
DROP TABLE #TempServiceItem 
DROP TABLE #TempServiceItemIns
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

DROP TABLE #TempSOSupply

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
END
GO

if object_id('uspfn_GenerateBPSLampiranNew') is not null
	drop procedure uspfn_GenerateBPSLampiranNew
GO
create procedure [uspfn_GenerateBPSLampiranNew]
	@CompanyCode	VARCHAR(MAX),
	@BranchCode		VARCHAR(MAX),
	@JobOrderNo		VARCHAR(MAX),
	@ProductType	VARCHAR(MAX),
	@CustomerCode	VARCHAR(MAX),
	@UserID			VARCHAR(MAX),
	@PickedBy		VARCHAR(MAX)
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

--SET	@CompanyCode	= '6006400001'
--SET	@BranchCode		= '6006400101'
--SET	@JobOrderNo		= 'SPK/14/101625'
--SET	@ProductType	= '4W'
--SET	@CustomerCode	= 'JKT-1852626'
--SET	@UserID			= 'ga'
--SET	@PickedBy		= '00001'
		
--exec uspfn_GenerateBPSLampiranNew '6006400001','6006400101','SPK/14/101625','4W','JKT-1852626','ga','00001'

--===============================================================================================================================
-- RE-CALCULATE AMOUNT DETAIL
--===============================================================================================================================
DECLARE @DefaultDate		DATETIME

SET @DefaultDate = '1900-01-01 00:00:00.000'

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
	, c.CostPrice
	, a.RetailPrice
	, a.TypeOfGoods
	, a.BillType
	, a.DiscPct
FROM SvTrnSrvItem a WITH (NOLOCK, NOWAIT)
INNER JOIN SpTrnSPickingDtl b WITH (NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.PartNo = b.PartNo AND a.SupplySlipNo = b.DocNo
INNER JOIN SpMstItemPrice c ON a.CompanyCode = c.CompanyCode AND a.BranchCode = c.BranchCode AND a.PartNo = c.PartNo
WHERE
	1 = 1
	AND a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND a.ProductType = @ProductType
	AND a.ServiceNo = @ServiceNo
	AND a.PartSeq IN (SELECT MAX(PartSeq) FROM SvTrnSrvItem WITH (NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode
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
	, ISNULL((SELECT CostPrice FROM SpMstItemPrice WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND PartNo = a.PartNo),0) CostPrice
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

	SELECT * INTO #Valid_2 FROM(
	SELECT a.PartNo
		, a.AllocationSR - b.QtyBill QtyValidSR
		, a.Onhand - b.QtyBill QtyValidOnhand
	FROM SpMstItems a, SpTrnSPickingDtl b
	WHERE 1 = 1
		AND a.CompanyCode = @CompanyMD
		AND a.BranchCode = @BranchMD
		AND b.PickingSlipNo = @PickingSlipNo
		AND a.CompanyCode = b.CompanyCode
		AND a.BranchCode = b.BranchCode
		AND a.PartNo = b.PartNo) #Valid_2

	SET @Allocation_SRValid = ISNULL((SELECT TOP 1 QtyValidSR FROM #Valid_2 WHERE QtyValidSR < 0),0)
	SET @Onhand_SRValid = ISNULL((SELECT TOP 1 QtyValidOnhand FROM #Valid_2 WHERE QtyValidOnhand < 0),0)

	DROP TABLE #Valid_2

	IF (@Onhand_SRValid < 0 OR @Allocation_SRValid < 0)
	BEGIN
		SET @errmsg = 'PEMBUATAN DOKUMEN LAMPIRAN GAGAL ! ' + char(13) + char(13) + 'Error Message: Terdapat part dengan quantity Onhand atau alokasi kurang dari nol !'
		RAISERROR (@errmsg,16,1);
		RETURN
	END
--===============================================================================================================================

DECLARE @DbMD AS VARCHAR(15)
SET @DbMD = (SELECT DbMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)

DECLARE @Query AS VARCHAR(MAX)
SET @Query = 
'UPDATE SpMstItems
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
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.PartNo = b.PartNo
UPDATE SpMstItemLoc
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
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.PartNo = b.PartNo'

EXEC(@query)
	
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
FETCH NEXT FROM db_cursor INTO @PickingSlipNo
END
CLOSE -- ==============================================================================================================================

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
END


-- ==============================================================================================================================
-- Author:		<Author,,Name>
-- Create date: 2015-03-07
-- Description: Select data transaksi service tipe estimasi untuk digunakan pada lookup input spk admin
-- ==============================================================================================================================

CREATE procedure [dbo].[uspfn_SvTrnServiceSelectEstimationData]
	@CompanyCode varchar(15),
	@BranchCode  varchar(15),
	@ProductType  varchar(15)
AS
-- EXEC uspfn_SvTrnServiceSelectEstimationData '6156401000', '6156401001', '4W'
SELECT DISTINCT 
    svTrnService.InvoiceNo
    , svTrnService.ServiceNo
    , svTrnService.ServiceType
    , ForemanID
    , EstimationNo
    , EstimationDate
    , BookingNo
    , BookingDate
    , svTrnService.JobOrderNo
    , svTrnService.JobOrderDate
    , svTrnService.PoliceRegNo
    , ServiceBookNo
    , svTrnService.BasicModel
    , TransmissionType
    , svTrnService.ChassisCode
    , svTrnService.ChassisNo
    , svTrnService.EngineCode
    , svTrnService.EngineNo
    , svTrnService.ChassisCode + ' ' + cast(svTrnService.ChassisNo as  varchar) KodeRangka
    , svTrnService.EngineCode + ' ' + cast(svTrnService.EngineNo as varchar) KodeMesin
    , ColorCode
    , (svTrnService.CustomerCode + ' - ' + gnMstCustomer.CustomerName) as Customer
    , (svTrnService.CustomerCodeBill + ' - ' + custBill.CustomerName) as CustomerBill
    , svTrnService.CustomerCode
    , svTrnService.CustomerCodeBill
    , svTrnService.Odometer
    , svTrnService.JobType
    , case when svTrnService.ServiceStatus='4' then
            case when @ProductType='4W' then reffService.Description
                else reffService.LockingBy
            end
        else reffService.Description 
    end ServiceStatus
    --, svTrnService.PoliceRegNo
	--, svTrnService.CustomerCode
    , InsurancePayFlag
    , InsuranceOwnRisk
    , InsuranceNo
    , InsuranceJobOrderNo
    --, svTrnService.CustomerCodeBill
    , svTrnService.LaborDiscPct
    , PartDiscPct
    , svTrnService.MaterialDiscPct
    , svTrnService.PPNPct
    , svTrnService.ServiceRequestDesc
    , ConfirmChangingPart
    --, ForemanID
    , svTrnService.MechanicID
    , EstimateFinishDate
    , svTrnService.LaborDPPAmt
    , svTrnService.PartsDPPAmt
    , svTrnService.MaterialDPPAmt
    , TotalDPPAmount
    , TotalPpnAmount
    , TotalPphAmount
    , TotalSrvAmount
    , employee.EmployeeName
    , (custBill.Address1 + '' + custBill.Address2 + '' + custBill.Address3 + '' + custBill.Address4) as AddressBill
    , custBill.NPWPNo
    , custBill.PhoneNo
    , custBill.HPNo
FROM svTrnService WITH(NOLOCK, NOWAIT)
LEFT JOIN gnMstCustomer 
    ON gnMstCustomer.CompanyCode = svTrnService.CompanyCode 
    AND gnMstCustomer.CustomerCode = svTrnService.CustomerCode
LEFT JOIN gnMstCustomer custBill 
    ON custBill.CompanyCode = svTrnService.CompanyCode
    AND custBill.CustomerCode = svTrnService.CustomerCodeBill
LEFT JOIN gnMstEmployee employee
    ON employee.CompanyCode = svTrnService.CompanyCode
    AND employee.BranchCode = svTrnService.BranchCode
	AND employee.EmployeeID = svTrnService.ForemanID
LEFT JOIN svTrnSrvItem srvItem 
    ON srvItem.CompanyCode = svTrnService.CompanyCode
    AND srvItem.BranchCode = svTrnService.BranchCode
    AND srvItem.ProductType = svTrnService.ProductType
    AND srvItem.ServiceNo = svTrnService.ServiceNo
LEFT JOIN svTrnSrvTask srvTask
    ON srvTask.CompanyCode = svTrnService.CompanyCode
    AND srvTask.BranchCode = svTrnService.BranchCode
    AND srvTask.ProductType = svTrnService.ProductType
    AND srvTask.ServiceNo = svTrnService.ServiceNo
LEFT JOIN svMstRefferenceService reffService
    ON reffService.CompanyCode = svTrnService.CompanyCode
    AND reffService.ProductType = svTrnService.ProductType    
    AND reffService.RefferenceCode = svTrnService.ServiceStatus
    AND reffService.RefferenceType = 'SERVSTAS'
LEFT JOIN svTrnInvoice invoice
	ON invoice.CompanyCode = svTrnService.CompanyCode
	AND invoice.BranchCode = svTrnService.BranchCode
	AND invoice.ProductType = svTrnService.ProductType
	AND invoice.JobOrderNo = svTrnService.JobOrderNo
LEFT JOIN svTrnSrvVOR VOR
    ON VOR.CompanyCode = svTrnService.CompanyCode
	AND VOR.BranchCode = svTrnService.BranchCode
    AND VOR.ServiceNo = svTrnService.ServiceNo
WHERE svTrnService.CompanyCode = @CompanyCode 
    AND svTrnService.BranchCode = @BranchCode
 AND svTrnService.ServiceType in ('0','2') and svTrnService.EstimationNo <> ''
 AND svTrnService.ServiceStatus IN (0,1,2,3,4,5) ORDER BY svTrnService.EstimationNo DESC
 
 GO