if object_id('svSDMovement') is not null
	drop TABLE svSDMovement
GO

CREATE TABLE [dbo].[svSDMovement](
	[CompanyCode] [varchar](15) NOT NULL,
	[BranchCode] [varchar](15) NOT NULL,
	[DocNo] [varchar](15) NOT NULL,
	[DocDate] [datetime] NOT NULL,
	[PartNo] [varchar](20) NOT NULL,
	[PartSeq] [int] NOT NULL,
	[WarehouseCode] [varchar](15) NOT NULL,
	[QtyOrder] [numeric](18, 2) NOT NULL,
	[Qty] [numeric](18, 2) NOT NULL,
	[DiscPct] [numeric](5, 2) NOT NULL,
	[CostPrice] [numeric](18, 2) NOT NULL,
	[RetailPrice] [numeric](18, 2) NOT NULL,
	[TypeOfGoods] [varchar](5) NOT NULL,
	[CompanyMD] [varchar](15) NOT NULL,
	[BranchMD] [varchar](15) NOT NULL,
	[WarehouseMD] [varchar](15) NOT NULL,
	[RetailPriceInclTaxMD] [numeric](18, 2) NOT NULL,
	[RetailPriceMD] [numeric](18, 2) NOT NULL,
	[CostPriceMD] [numeric](18, 2) NOT NULL,
	[QtyFlag] [char](1) NOT NULL,
	[ProductType] [varchar](15) NOT NULL,
	[ProfitCenterCode] [varchar](15) NOT NULL,
	[Status] [char](1) NOT NULL,
	[ProcessStatus] [char](1) NOT NULL,
	[ProcessDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](15) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[LastUpdateBy] [varchar](15) NULL,
	[LastUpdateDate] [datetime] NULL,
 CONSTRAINT [PK_svSDMovement1] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[BranchCode] ASC,
	[DocNo] ASC,
	[PartNo] ASC,
	[PartSeq] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

if object_id('omSDMovement') is not null
	drop TABLE omSDMovement
GO

CREATE TABLE [dbo].[omSDMovement](
	[CompanyCode] [varchar](15) NOT NULL,
	[BranchCode] [varchar](15) NOT NULL,
	[DocNo] [varchar](15) NOT NULL,
	[DocDate] [datetime] NOT NULL,
	[Seq] [int] NOT NULL,
	[SalesModelCode] [varchar](20) NOT NULL,
	[SalesModelYear] [numeric](4, 0) NOT NULL,
	[ChassisCode] [varchar](15) NOT NULL,
	[ChassisNo] [numeric](10, 0) NOT NULL,
	[EngineCode] [varchar](15) NOT NULL,
	[EngineNo] [numeric](10, 0) NOT NULL,
	[ColourCode] [varchar](15) NOT NULL,
	[WarehouseCode] [varchar](15) NOT NULL,
	[CustomerCode] [varchar](15) NOT NULL,
	[QtyFlag] [char](1) NOT NULL,
	[CompanyMD] [varchar](15) NOT NULL,
	[BranchMD] [varchar](15) NOT NULL,
	[WarehouseMD] [varchar](15) NOT NULL,
	[Status] [char](1) NOT NULL,
	[ProcessStatus] [char](1) NOT NULL,
	[ProcessDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](15) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[LastUpdateBy] [varchar](15) NOT NULL,
	[LastUpdateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_omSDMovement] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[BranchCode] ASC,
	[DocNo] ASC,
	[DocDate] ASC,
	[ChassisCode] ASC,
	[ChassisNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

if object_id('svHstSDMovement') is not null
	drop TABLE svHstSDMovement
GO

CREATE TABLE [dbo].[svHstSDMovement](
	[CompanyCode] [varchar](15) NOT NULL,
	[BranchCode] [varchar](15) NOT NULL,
	[DocNo] [varchar](15) NOT NULL,
	[DocDate] [datetime] NOT NULL,
	[PartNo] [varchar](20) NOT NULL,
	[PartSeq] [int] NOT NULL,
	[WarehouseCode] [varchar](15) NOT NULL,
	[QtyOrder] [numeric](18, 2) NOT NULL,
	[Qty] [numeric](18, 2) NOT NULL,
	[DiscPct] [numeric](5, 2) NOT NULL,
	[CostPrice] [numeric](18, 2) NOT NULL,
	[RetailPrice] [numeric](18, 2) NOT NULL,
	[TypeOfGoods] [varchar](5) NOT NULL,
	[CompanyMD] [varchar](15) NOT NULL,
	[BranchMD] [varchar](15) NOT NULL,
	[WarehouseMD] [varchar](15) NOT NULL,
	[RetailPriceInclTaxMD] [numeric](18, 2) NOT NULL,
	[RetailPriceMD] [numeric](18, 2) NOT NULL,
	[CostPriceMD] [numeric](18, 2) NOT NULL,
	[QtyFlag] [char](1) NOT NULL,
	[ProductType] [varchar](15) NOT NULL,
	[ProfitCenterCode] [varchar](15) NOT NULL,
	[Status] [char](1) NOT NULL,
	[ProcessStatus] [char](1) NOT NULL,
	[ProcessDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](15) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[LastUpdateBy] [varchar](15) NULL,
	[LastUpdateDate] [datetime] NULL
) ON [PRIMARY]

GO
if object_id('omHstSDMovement') is not null
	drop TABLE omHstSDMovement
GO

CREATE TABLE [dbo].[omHstSDMovement](
	[CompanyCode] [varchar](15) NOT NULL,
	[BranchCode] [varchar](15) NOT NULL,
	[DocNo] [varchar](15) NOT NULL,
	[DocDate] [datetime] NOT NULL,
	[Seq] [int] NOT NULL,
	[SalesModelCode] [varchar](20) NOT NULL,
	[SalesModelYear] [numeric](4, 0) NOT NULL,
	[ChassisCode] [varchar](15) NOT NULL,
	[ChassisNo] [numeric](10, 0) NOT NULL,
	[EngineCode] [varchar](15) NOT NULL,
	[EngineNo] [numeric](10, 0) NOT NULL,
	[ColourCode] [varchar](15) NOT NULL,
	[WarehouseCode] [varchar](15) NOT NULL,
	[CustomerCode] [varchar](15) NOT NULL,
	[QtyFlag] [char](1) NOT NULL,
	[CompanyMD] [varchar](15) NOT NULL,
	[BranchMD] [varchar](15) NOT NULL,
	[WarehouseMD] [varchar](15) NOT NULL,
	[Status] [char](1) NOT NULL,
	[ProcessStatus] [char](1) NOT NULL,
	[ProcessDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](15) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[LastUpdateBy] [varchar](15) NOT NULL,
	[LastUpdateDate] [datetime] NOT NULL
) ON [PRIMARY]

GO



if object_id('uspfn_omGetPriceList') is not null
	drop procedure uspfn_omGetPriceList
GO
CREATE procedure [dbo].[uspfn_omGetPriceList]
	@CompanyCode varchar(20)='',
	@BranchCode varchar(20)='',
	@salesmodelcode varchar(32)='',
	@salesmodelyear int=0,
	@Status int=2,
	@SupplierCode varchar(20)=''
AS
SELECT 
       a.[SalesModelCode]
      ,a.[SalesModelYear]
      ,b.[SalesModelDesc]
      ,a.[GroupPrice]
      ,a.[RetailPriceIncludePPN]
      ,a.[DiscPriceIncludePPN]
      ,a.[NetSalesIncludePPN]
      ,a.[RetailPriceExcludePPN]
      ,a.[DiscPriceExcludePPN]
      ,a.[NetSalesExcludePPN]
      ,a.[PPNBeforeDisc]
      ,a.[PPNAfterDisc]
      ,a.OthersDPP
      ,a.OthersPPn [OthersPPN]
      ,a.[PPNBMPaid]
      ,a.[EffectiveDate]
      ,a.[isStatus]
  FROM [dbo].[omPriceListBranches] a inner join omMstModel b 
  on (a.CompanyCode=b.CompanyCode and a.SalesModelCode=b.SalesModelCode)
where a.companycode=@CompanyCode and a.BranchCode=@BranchCode
and a.SalesModelCode like case when @salesmodelcode='' then '%%' else @salesmodelcode end
and a.SalesModelYear = case when @salesmodelyear=0 then a.SalesModelYear else @salesmodelyear end
and a.isStatus = case when @Status=2 then a.isstatus else @Status end
and a.SupplierCode = @SupplierCode
GO

if object_id('uspfn_omUpdatePriceList') is not null
	drop procedure uspfn_omUpdatePriceList
GO
CREATE Procedure [dbo].[uspfn_omUpdatePriceList]
  @CompanyCode varchar(15)='' ,
  @BranchCode varchar(15)='' ,
  @SupplierCode varchar(15)='' ,
  @GroupPrice varchar(15)='' ,
  @SalesModelCode varchar(20) ='',
  @SalesModelYear numeric(4, 0)=2015 ,
  @EffectiveDate datetime=null ,
  @RetailPriceIncludePPN numeric(18, 0)=0 ,
  @DiscPriceIncludePPN numeric(18, 0) =0,
  @NetSalesIncludePPN numeric(18, 0)=0 ,
  @RetailPriceExcludePPN numeric(18, 0)=0 ,
  @DiscPriceExcludePPN numeric(18, 0) =0,
  @NetSalesExcludePPN numeric(18, 0) =0,
  @PPNBeforeDisc numeric(18, 0) =0,
  @PPNAfterDisc numeric(18, 0) =0,
  @PPNBMPaid numeric(18, 0)=0 ,
  @isStatus bit = 1,
  @UserId varchar(20)='',
  @OthersDPP numeric(18, 0)=0 ,
  @OthersPPN numeric(18, 0)=0 
AS
BEGIN
  
  IF @BranchCode=''
  BEGIN
    declare @loopbranch varchar(20)
    declare crBranch  cursor local for
    select branchcode from gnMstCoProfile
    where companycode=@companycode

    open crBranch
    fetch next from crBranch into @loopbranch

    while @@FETCH_STATUS=0
    begin
      exec uspfn_omUpdatePriceList @companycode,@loopbranch,@SupplierCode,@GroupPrice,@SalesModelCode,@SalesModelYear,@EffectiveDate,@RetailPriceIncludePPN,@DiscPriceIncludePPN,@NetSalesIncludePPN,@RetailPriceExcludePPN,@DiscPriceExcludePPN,@NetSalesExcludePPN,@PPNBeforeDisc,@PPNAfterDisc,@PPNBMPaid,@isStatus,@UserId, @OthersDPP, @OthersPPN
      fetch next from crBranch into @loopbranch
    end

    close crbranch
    deallocate crbranch
  END
  ELSE
  BEGIN

    INSERT INTO [dbo].[omPriceListBranchesLog]
        ([CompanyCode],[BranchCode],[SupplierCode],[GroupPrice],[SalesModelCode],[SalesModelYear]
        ,[EffectiveDate],[RetailPriceIncludePPN],[DiscPriceIncludePPN],[NetSalesIncludePPN]
        ,[RetailPriceExcludePPN],[DiscPriceExcludePPN],[NetSalesExcludePPN],[PPNBeforeDisc],[PPNAfterDisc]
        ,[PPNBMPaid],[isStatus],[LastUpdateBy],[LastUpdateDate])
      VALUES
        (@companycode,@BranchCode,@SupplierCode,@GroupPrice
        ,@SalesModelCode,@SalesModelYear,@EffectiveDate,@RetailPriceIncludePPN,@DiscPriceIncludePPN,@NetSalesIncludePPN,@RetailPriceExcludePPN
        ,@DiscPriceExcludePPN,@NetSalesExcludePPN,@PPNBeforeDisc,@PPNAfterDisc,@PPNBMPaid,@isStatus,@UserId,getdate()) 

    IF  EXISTS(select * from omPriceListBranches  
    where companycode=@companycode and branchcode=@branchcode
    and suppliercode=@suppliercode and groupprice=@groupprice
    and SalesModelCode=@SalesModelCode and SalesModelYear=@SalesModelYear
    and EffectiveDate = @EffectiveDate)
    BEGIN
      UPDATE [omPriceListBranches] 
      SET 
        RetailPriceIncludePPN=@RetailPriceIncludePPN,
        DiscPriceIncludePPN=@DiscPriceIncludePPN,NetSalesIncludePPN=@NetSalesIncludePPN,
        RetailPriceExcludePPN=@RetailPriceExcludePPN,DiscPriceExcludePPN=@DiscPriceExcludePPN,
        NetSalesExcludePPN=@NetSalesExcludePPN,PPNBeforeDisc=@PPNBeforeDisc,
        PPNAfterDisc=@PPNAfterDisc,PPNBMPaid=@PPNBMPaid,isStatus=@isStatus,
        [LastUpdateBy]=@userid,LastUpdateDate=getdate(),
        OthersDPP=@OthersDPP,
        OthersPPN=@OthersPPN
      where companycode=@companycode and branchcode=@branchcode
      and suppliercode=@suppliercode and groupprice=@groupprice
      and SalesModelCode=@SalesModelCode and SalesModelYear=@SalesModelYear
      and EffectiveDate = @EffectiveDate
    END
    ELSE
    BEGIN
      INSERT INTO [dbo].[omPriceListBranches]
           ([CompanyCode],[BranchCode],[SupplierCode],[GroupPrice],[SalesModelCode],[SalesModelYear]
           ,[EffectiveDate],[RetailPriceIncludePPN],[DiscPriceIncludePPN],[NetSalesIncludePPN]
           ,[RetailPriceExcludePPN],[DiscPriceExcludePPN],[NetSalesExcludePPN],[PPNBeforeDisc],[PPNAfterDisc]
           ,[PPNBMPaid],[isStatus],CreatedBy,[CreatedDate],OthersDPP, OthersPPn)
       VALUES
           (@companycode,@BranchCode,@SupplierCode,@GroupPrice
           ,@SalesModelCode,@SalesModelYear,@EffectiveDate,@RetailPriceIncludePPN,@DiscPriceIncludePPN,@NetSalesIncludePPN,@RetailPriceExcludePPN
           ,@DiscPriceExcludePPN,@NetSalesExcludePPN,@PPNBeforeDisc,@PPNAfterDisc,@PPNBMPaid,@isStatus,@UserId,getdate(), @OthersDPP,@OthersPPN) 

    END


  END

END
GO

if object_id('usprpt_PostingMultiCompany4DocNo') is not null
	drop procedure usprpt_PostingMultiCompany4DocNo
GO

-- POSTING TRANSACTION MULTI COMPANY - DOCUMENT NUMBER
-- --------------------------------------------------------------
-- Created  by : HTO, 2014
-- Revision by : HTO, January 2015
-- --------------------------------------------------------------
-- This procedure used for 
--		4-Wheeler : BIT-SBT, SBSM-SSBT, BAT-SAT, SIT-SST, UIB-SIT
--		2-Wheeler : IJMG,SVMG,ISG,ISMG-SMG
-- -------------------------------------------------------------------------------------------------
-- declare @DocNo varchar(15)  
-- execute [usprpt_PostingMultiCompany4DocNo] '6006400001','6006400131','SBTSBY','INV',@DocNo output
-- -------------------------------------------------------------------------------------------------

CREATE procedure [dbo].[usprpt_PostingMultiCompany4DocNo]
	@Company	varchar(15),
	@Branch		varchar(15),
	@DBName		varchar(50),
	@DocID		varchar(15),
	@DocNo		varchar(15) output
AS	

--BEGIN TRANSACTION
--BEGIN TRY

BEGIN
 -- Document Number sequence
  --declare @Company		varchar(15)  = '6006400001'
  --declare @Branch			varchar(15)  = '6006400131'
  --declare @DBName			varchar(50)  = 'SBTSBY'
  --declare @DocID			varchar(15)  = 'INV'
  --declare @DocNo			varchar(15)
	declare @DocPrefix		varchar(15)
	declare @DocYear		integer
	declare @DocSeq			integer
	declare @sqlString		nvarchar(max)

	set @sqlString = N'select @DocPrefix=DocumentPrefix, @DocYear=DocumentYear, @DocSeq=DocumentSequence from '+@DBName+'..gnMstDocument ' +
						'where CompanyCode='''+@Company+''' and BranchCode='''+@Branch+''' and DocumentType='''+@DocID+''''

		execute sp_executesql @sqlString, 
			N'@DocPrefix varchar(15) output, @DocYear integer output, @DocSeq integer output', @DocPrefix output, @DocYear output, @DocSeq output

	set @DocSeq = @DocSeq + 1
	set @sqlString = 'update ' +@DBName+ '..gnMstDocument' +
						' set DocumentSequence = ' +convert(varchar,@DocSeq)+ 
						' where CompanyCode=''' +@Company+ ''' and BranchCode=''' +@Branch+ ''' and DocumentType='''+@DocID+''''
		execute sp_executesql @sqlString 

	set @DocNo = @DocPrefix + '/' + right(convert(varchar,@DocYear),2) + '/' + 
					replicate('0',6-len(convert(varchar,@DocSeq))) + convert(varchar,@DocSeq)
/*
END TRY

BEGIN CATCH
    select ERROR_NUMBER()    AS ErrorNumber,    ERROR_SEVERITY() AS ErrorSeverity, ERROR_STATE()   AS ErrorState,
		   ERROR_PROCEDURE() AS ErrorProcedure, ERROR_LINE()     AS ErrorLine    , ERROR_MESSAGE() AS ErrorMessage
	if @@TRANCOUNT > 0
		rollback transaction
	select '0' [STATUS], 'Posting fail !!!' [INFO]
	return
END CATCH

IF @@TRANCOUNT > 0
	begin
		select '1' [STATUS], 'Posting Done !!!' [INFO]
		--rollback transaction
		commit transaction
	end
*/
END
GO

if object_id('usprpt_PostingMultiCompanySparePartService') is not null
	drop procedure usprpt_PostingMultiCompanySparePartService
GO
-- POSTING TRANSACTION MULTI COMPANY - SPARE PART & SERVICE
-- -------------------------------------------------------------------------------
-- Created  by : HTO, 2014
-- Revision-01 : HTO, January 2015
-- Revision-02 : HTO, April 2015 (Process by Date & LMP => INV Service)
-- -------------------------------------------------------------------------------
-- This procedure used for 
--		4-Wheeler : BIT-SBT, SBSM-SSBT, BAT-SAT, SIT-SST, UIB-SIT
--		2-Wheeler : IJMG,SVMG,ISG,ISMG-SMG
-- -------------------------------------------------------------------------------
-- execute [usprpt_PostingMultiCompanySparePartService] '6006406','2014/11/30','0'
-- -------------------------------------------------------------------------------

CREATE procedure [dbo].[usprpt_PostingMultiCompanySparePartService]
	@CompanyCode	varchar(15),
	@PostingDate	datetime,
	@Status			varchar(1)	output
AS	

--BEGIN TRANSACTION
--BEGIN TRY
BEGIN 
		--declare @CompanyCode				varchar(15) = (select top 1 CompanyCode from gnMstOrganizationHdr)
		--declare @PostingDate				datetime    = '2015/04/30' --getdate()
		--declare @Status					varchar(1)  = '0'

  -- Check Tax/Seri Pajak online
		declare @TaxCompany					varchar(15)
		declare @TaxBranch					varchar(15)
		declare @TaxDB						varchar(50)
		declare @TaxTransCode				varchar(3)
		declare @swTaxBranch				varchar(15) = ''
		declare @swTaxDoc					varchar(15) = ''
		declare @swTaxDocDate 				varchar(10) = ''
		declare @swCompanyCode				varchar(15) = ''
		declare @swBranchCode				varchar(15) = ''
		declare @swDocNo 					varchar(15) = ''
		declare @swDocDate 					varchar(10) = ''
		declare @swTypeOfGoods				varchar(15) = ''
		declare @swDocSort					integer		= 0
		declare @TaxSeq						bigint
		declare @TaxSeqNo					integer
		declare @SeriPajakNo				varchar(50) = ''
		declare @sqlString					nvarchar(max)

		set @TaxCompany=isnull((select ParaValue from gnMstLookupDtl where CodeID='TXOL' and LookupValue='COMPANYCODE'),'')
		set @TaxBranch =isnull((select ParaValue from gnMstLookupDtl where CodeID='TXOL' and LookupValue='BRANCHCODE'),'')
		set @TaxDB     =isnull((select ParaValue from gnMstLookupDtl where CodeID='TXOL' and LookupValue='FROM_DB'),'')
		--select @TaxCompany, @TaxBranch, @TaxDB
		if @TaxCompany='' or @TaxBranch='' or @TaxDB=''
			begin
				set @Status = '1'
				return
			end
		
		set @sqlString = N'select top 1 @TaxSeq=FPJSeqNo, @TaxSeqNo=SeqNo from ' 
									   +@TaxDb+ '..gnMstFPJSeqNo where CompanyCode=''' 
									   +@TaxCompany+ ''' and BranchCode=''' 
									   +@TaxBranch+ ''' and Year=''' 
									   +convert(varchar,year(@PostingDate))+ ''' and isActive=1 and convert(varchar,EffectiveDate,111)<=''' 
									   +convert(varchar,@PostingDate,111)+ ''' order by CompanyCode, BranchCode, Year, SeqNo'
			execute sp_executesql @sqlString, 
								N'@TaxSeq 		bigint  	output,
								  @TaxSeqNo 	int 		output', 
								  @TaxSeq 		output,
								  @TaxSeqNo 	output
		if isnull(@TaxSeq,0)=0 and isnull(@TaxSeqNo,0)=0
			begin
				set @Status = '1'
				return
			end
		set @TaxSeq   = isnull(@TaxSeq,0)
		set @TaxSeqNo = isnull(@TaxSeqNo,0)

 -- Posting process : insert data to MD & SD table
		declare @curCompanyCode				varchar(15)
		declare @curBranchCode				varchar(15)
		declare @curDocNo					varchar(20)
		declare @curDocDate					datetime
		declare @curPartNo					varchar(20)
		declare @curPartSeq					integer
		declare @curWarehouseCode			varchar(15)
		declare @curQtyOrder				numeric(18,2)
		declare @curQty						numeric(18,2)
		declare @curDiscPct					numeric(06,2)
		declare @curCostPrice				numeric(18,2)
		declare @curRetailPrice				numeric(18,2)
		declare @curTypeOfGoods				varchar(15)
		declare @curCompanyMD				varchar(15)
		declare @curBranchMD				varchar(15)
		declare @curWarehouseMD				varchar(15)
		declare @curRetailPriceInclTaxMD	numeric(18,2)
		declare @curRetailPriceMD			numeric(18,2)
		declare @curCostPriceMD				numeric(18,2)
		declare @curQtyFlag					char(1)
		declare @curProductType				varchar(15)
		declare @curProfitCenterCode		varchar(15)
		declare @curStatus					char(1)
		declare @curProcessStatus			char(1)
		declare @curProcessDate				datetime
		declare @curCreatedBy				varchar(15)
		declare @curCreatedDate				datetime
		declare @curLastUpdateBy			varchar(15)
		declare @curLastUpdateDate			datetime
		declare @curDocSort					integer
		declare @DocPrefix					varchar(15)
		declare @SONo						varchar(15)
		declare @PLNo						varchar(15)
		declare @INVNo						varchar(15)
		declare @FPJNo						varchar(15)
		declare @POSNo						varchar(15)
		declare @BINNo						varchar(15)
		declare @WRSNo						varchar(15)
		declare @HPPNo						varchar(15)
		declare @APNo						varchar(15)
		declare @DocYear					numeric(4,0)
		declare @DocSeq						numeric(15,0)
		declare @SeqNo						integer
		declare @DBName						varchar(50)
		declare @DBNameMD					varchar(50)
		declare @TotPurchaseAmt				numeric(18,0)
		declare @TotPurchaseNetAmt			numeric(18,0)
		declare @TotTaxAmt					numeric(18,0)
		declare @RetailPriceNet				numeric(18,0)
		declare @SalesAmt					numeric(18,0)
		declare @DiscAmt					numeric(18,0)
		declare @NetSales					numeric(18,0)
		declare @PPNAmt						numeric(18,0)
		declare @TotSalesAmt				numeric(18,0)
		declare @CostAmt 					numeric(18,0)
		declare @TotalNetSales				numeric(18,0)
		declare @TotalPPNAmt				numeric(18,0)
		declare @TotalSalesAmt				numeric(18,0)
		declare @DiscPct  					numeric(06,2)
		declare @DiscPctSD					numeric(06,2)
		declare @DiscPctMD					numeric(06,2)
		declare @MovingCode					varchar(15)
		declare @ABCClass					char(1)
		declare @PartCategory				varchar(15)
		declare @LocationCode				varchar(10)
		declare @MovingCodeMD				varchar(15)
		declare @ABCClassMD					char(1)
		declare @PartCategoryMD				varchar(3)
		declare @LocationCodeMD				varchar(10)
		declare @ProductTypeMD				varchar(15)	
		declare @TypeOfGoodsMD				varchar(5)
		declare @PaymentCodeMD				varchar(15)
		declare @SalesCodeMD				varchar(15)
		declare @CustomerNameMD				varchar(100)
		declare @Address1MD					varchar(100)
		declare @Address2MD					varchar(100)
		declare @Address3MD					varchar(100)
		declare @Address4MD					varchar(100)
		declare @isPKPMD					varchar(100)
		declare @NPWPNoMD					varchar(100)
		declare @SKPNoMD					varchar(100)
		declare @SKPDateMD					datetime
		declare @NPWPDateMD					datetime
		declare @TopCodeMD					varchar(15)
		declare @TopDaysMD					integer
		declare @DueDateMD					date
		declare @AccNoArMD					varchar(50)
		declare @AccNoSalesMD				varchar(50)
		declare @AccNoTaxOutMD				varchar(50)
		declare @AccNoDisc1MD				varchar(50)
		declare @AccNoCogsMD				varchar(50)
		declare @AccNoInventoryMD			varchar(50)
		declare @AccNoInventory				varchar(50)
		declare @AccNoTaxIn					varchar(50)
		declare @AccNoAP					varchar(50)
		declare @SeqNoGlMD					numeric(10,0)
		declare @SeqNoGl					numeric(10,0)
		declare @DiscFlag					integer
		declare @CurrentDate				varchar(100) = convert(varchar,getdate(),121)

        declare cursvSDMovement cursor for
			select *, DocSort = (case when left(DocNo,3)='INV' then 1 else 2 end)
			  from svSDMovement
           --where left(DocNo,3) in ('LMP','INV') 
             where left(DocNo,2) = 'IN'  -- INV: Invoice Part; INA, INC, INF, INI, INW, INP: Invoice Service 
			   and convert(varchar,DocDate,111)<=convert(varchar,@PostingDate,111)
			   and Qty>0
			   and ProcessStatus='0'
             order by convert(varchar,DocDate,111), CompanyCode, BranchCode, DocSort, DocNo, TypeOfGoods, PartNo, PartSeq
		open cursvSDMovement

		fetch next from cursvSDMovement
			  into @curCompanyCode, @curBranchCode, @curDocNo, @curDocDate, @curPartNo, @curPartSeq, @curWarehouseCode, @curQtyOrder, 
			       @curQty, @curDiscPct, @curCostPrice, @curRetailPrice, @curTypeOfGoods, @curCompanyMD, @curBranchMD, @curWarehouseMD, 
				   @curRetailPriceInclTaxMD, @curRetailPriceMD, @curCostPriceMD, @curQtyFlag, @curProductType, @curProfitCenterCode, 
				   @curStatus, @curProcessStatus, @curProcessDate, @curCreatedBy, @curCreatedDate, @curLastUpdateBy, @curLastUpdateDate,
				   @curDocSort

		while (@@FETCH_STATUS =0)
			begin
			 -- MD : Database Name, Company Code & Branch Code MD
				set @DBNameMD = (select DbMD   from gnMstCompanyMapping where CompanyCode=@CurCompanyCode and BranchCode =@curBranchCode)

			 -- SD : Database Name 
				set @DBName   = (select DbName from gnMstCompanyMapping where CompanyCode=@curCompanyCode and BranchCode=@curBranchCode)

			 -- MD: MovingCode, ABCClass, PartCategory
				set @sqlString = N'select @MovingCodeMD=MovingCode, @ABCClassMD=ABCClass, @PartCategoryMD=PartCategory, @TypeOfGoodsMD=TypeOfGoods from ' 
										  +@DBNameMD+ '..spMstItems where CompanyCode=''' 
										  +@curCompanyMD+ ''' and BranchCode=''' 
										  +@curBranchMD+ ''' and PartNo=''' 
										  +@curPartNo + ''''
					execute sp_executesql @sqlString, 
										N'@MovingCodeMD		varchar(15) output,
										  @ABCClassMD 		char(1) 	output,
										  @PartCategoryMD 	varchar(3) 	output,
										  @TypeOfGoodsMD 	varchar(15) output', 
										  @MovingCodeMD 	output,
										  @ABCClassMD 		output,
										  @PartCategoryMD 	output,
										  @TypeOfGoodsMD 	output
										  
			 -- MD: Location Code
				set @sqlString = N'select @LocationCodeMD=LocationCode from ' 
										  +@DBNameMD+ '..spMstItemLoc where CompanyCode=''' 
										  +@curCompanyMD+ ''' and BranchCode=''' 
										  +@curBranchMD+ ''' and PartNo=''' 
										  +@curPartNo + ''' and WarehouseCode=''00'''
					execute sp_executesql @sqlString, 
										N'@LocationCodeMD 	varchar(10) output', 
										  @LocationCodeMD 	output

			 -- MD: ProducType
				set @sqlString = N'select @ProductTypeMD=ProductType from ' 
										  +@DBNameMD+ '..gnMstCoProfile where CompanyCode=''' 
										  +@curCompanyMD+ ''' and BranchCode=''' +@curBranchMD+ ''''
					execute sp_executesql @sqlString, 
										N'@ProductTypeMD 	varchar(15) output', 
										  @ProductTypeMD 	output

			 -- MD: Top Code, Payment Code, Sales Code, Discount 
				set @sqlString = N'select @TopCodeMD=TopCode, @PaymentCodeMD=PaymentCode, @SalesCodeMD=SalesCode, @DiscPctMD=DiscPct from ' 
										  +@DBNameMD+ '..gnMstCustomerProfitCenter where CompanyCode=''' 
										  +@curCompanyMD+ ''' and BranchCode=''' 
										  +@curBranchMD+ ''' and CustomerCode=''' 
										  +@curBranchCode+ ''' and ProfitCenterCode=''300'''
					execute sp_executesql @sqlString, 
										N'@TopCodeMD 		varchar(15)  output,
										  @PaymentCodeMD 	varchar(15)  output,
										  @SalesCodeMD 		varchar(15)  output,
										  @DiscPctMD        numeric(5,2) output', 
										  @TopCodeMD 		output,
										  @PaymentCodeMD 	output,
										  @SalesCodeMD 		output,
										  @DiscPctMD     	output

			 -- MD: Top Days
				set @sqlString = N'select @TopDaysMD=ParaValue from ' 
										  +@DBNameMD+ '..gnMstLookUpDtl where CompanyCode=''' 
										  +@curCompanyMD+ ''' and CodeID=''TOPC'' and LookUpValue=''' 
										  +@TopCodeMD+ ''''
					execute sp_executesql @sqlString, 
										N'@TopDaysMD 		integer	output', 
										  @TopDaysMD 		output
										  
			 -- MD: AR Account
				set @sqlString = N'select @AccNoArMD=c.ReceivableAccNo from ' 
				                          +@DBNameMD+ '..gnMstCustomerClass c,' 
										  +@DBNameMD+ '..gnMstCustomerProfitCenter p
									where c.CompanyCode     =p.CompanyCode
									  and c.BranchCode      =p.BranchCode
									  and c.CustomerClass   =p.CustomerClass
									  and c.ProfitCenterCode=p.ProfitCenterCode
									  and c.CompanyCode     =''' +@curCompanyMD+ 
								  ''' and c.BranchCode      =''' +@curBranchMD+ 
								  ''' and c.ProfitCenterCode=''300''
								      and c.TypeOfGoods     =''' +@TypeOfGoodsMD+
								  ''' and p.CustomerCode    =''' +@curBranchCode+ ''''
					execute sp_executesql @sqlString, 
										N'@AccNoArMD 		varchar(50) output', 
										  @AccNoArMD 		output
										  
			 -- MD: Sales Account
				set @sqlString = N'select @AccNoSalesMD=SalesAccNo from ' 
										  +@DBNameMD+ '..spMstAccount where CompanyCode=''' 
										  +@curCompanyMD+ ''' and BranchCode =''' 
										  +@curBranchMD+ ''' and TypeOfGoods=''' 
										  +@TypeOfGoodsMD+ ''''
					execute sp_executesql @sqlString, 
										N'@AccNoSalesMD 	varchar(50) output', 
										  @AccNoSalesMD 	output
										  
			 -- MD: Tax Account
				set @sqlString = N'select @AccNoTaxOutMD=c.TaxOutAccNo from ' 
										  +@DBNameMD+ '..gnMstCustomerClass c,' 
										  +@DBNameMD+ '..gnMstCustomerProfitCenter p
									where c.CompanyCode     =p.CompanyCode
									  and c.BranchCode      =p.BranchCode
									  and c.CustomerClass   =p.CustomerClass
									  and c.ProfitCenterCode=p.ProfitCenterCode
									  and c.CompanyCode     =''' +@curCompanyMD+ ''' 
									  and c.BranchCode      =''' +@curBranchMD+ '''
									  and c.ProfitCenterCode=''300''
									  and c.TypeOfGoods     =''' +@TypeOfGoodsMD+ '''
									  and p.CustomerCode    =''' +@curBranchCode+ ''''
					execute sp_executesql @sqlString, 
										N'@AccNoTaxOutMD 	varchar(50) output', 
										  @AccNoTaxOutMD 	output
										  
			 -- MD: Discount, COGS & Inventory Account
				set @sqlString = N'select @AccNoDisc1MD=DiscAccNo, @AccNoCogsMD=COGSAccNo, @AccNoInventoryMD=InventoryAccNo from ' 
										  +@DBNameMD+ '..spMstAccount where CompanyCode=''' 
										  +@curCompanyMD+ ''' and BranchCode =''' 
										  +@curBranchMD+ ''' and TypeOfGoods=''' 
										  +@TypeOfGoodsMD+ ''''
					execute sp_executesql @sqlString,
										N'@AccNoDisc1MD 	varchar(50) output,
										  @AccNoCogsMD 		varchar(50) output,
										  @AccNoInventoryMD varchar(50) output', 
										  @AccNoDisc1MD 	output,
										  @AccNoCogsMD 		output,
										  @AccNoInventoryMD output
										  
			 -- SD: Purchase Discount
				set @sqlString = N'select @DiscPctSD=PurcDiscPct from ' 
										  +@DBName+ '..spMstItems where CompanyCode=''' 
										  +@curCompanyCode+ ''' and BranchCode=''' 
										  +@curBranchCode+ ''' and PartNo=''' 
										  +@curPartNo + ''''
					execute sp_executesql @sqlString, 
										N'@DiscPctSD		numeric(6,2) output',
										  @DiscPctSD		output

				--if isnull(@DiscPctSD,0.00) > 0.00
				--	set @DiscPct = isnull(@DiscPctSD,0.00)
				--else
				--	set @DiscPct = isnull(@DiscPctMD,0.00)
				if @DiscPctSD is null
					set @DiscPct = isnull(@DiscPctMD,0.00)
				else
					set @DiscPct = @DiscPctSD

				set @SalesAmt          = @curQty * @curRetailPriceMD 
				set @DiscAmt           = round( (@SalesAmt * @DiscPct / 100),0)
				set @NetSales          = @SalesAmt - @DiscAmt
				set @PPNAmt            = round((@NetSales*0.1),0)
				set @TotSalesAmt       = @NetSales + @PPNAmt
				set @CostAmt           = @curQty * @curCostPriceMD 
				set @RetailPriceNet    = round(@curRetailPriceMD * (100.00-@DiscPct) / 100.00,0)
				set @TotPurchaseNetAmt = @NetSales
				set @TotTaxAmt         = @PPNAmt
			    set @TotPurchaseAmt    = @TotSalesAmt

				set @DueDateMD         = dateadd(day,isnull(@TopDaysMD,0),@curDocDate)

				-- SD: A/P, TAX IN  Account
				set @sqlString = N'select @AccNoAP=c.PayableAccNo, @AccNoTaxIn=c.TaxInAccNo from ' 
										  +@DBName+ '..gnMstSupplierClass c,' 
										  +@DBName+ '..gnMstSupplierProfitCenter p
									where c.CompanyCode     =p.CompanyCode
									  and c.BranchCode      =p.BranchCode
									  and c.SupplierClass   =p.SupplierClass
									  and c.ProfitCenterCode=p.ProfitCenterCode
									  and c.CompanyCode     =''' +@curCompanyCode+ ''' 
									  and c.BranchCode      =''' +@curBranchCode+ '''
									  and c.ProfitCenterCode=''300''
									  and c.TypeOfGoods     =''' +@curTypeOfGoods+ '''
									  and p.SupplierCode    =''' +@curBranchMD+ ''''
					execute sp_executesql @sqlString, 
										N'@AccNoAP 			varchar(50) output,
										  @AccNoTaxIn 		varchar(50) output', 
										  @AccNoAP 			output,
										  @AccNoTaxIn 		output

				set @AccNoAP    = isnull(@AccNoAP,'')
				set @AccNoTaxIn = isnull(@AccNoTaxIn,'')
				
				-- SD: Inventory Account
				set @sqlString = N'select @AccNoInventory=InventoryAccNo from ' 
										  +@DBName+ '..spMstAccount
									where CompanyCode=''' +@curCompanyCode+ ''' 
									  and BranchCode =''' +@curBranchCode+ '''
									  and TypeOfGoods=''' +@CurTypeOfGoods+ ''''
					execute sp_executesql @sqlString, 
										N'@AccNoInventory 	varchar(50) output', 
										  @AccNoInventory 	output

			 -- SD: MovingCode, ABCClass, PartCategory
				set @sqlString = N'select @MovingCode=MovingCode, @ABCClass=ABCClass, @PartCategory=PartCategory from ' 
										  +@DBName+ '..spMstItems where CompanyCode=''' 
										  +@curCompanyCode+ ''' and BranchCode=''' 
										  +@curBranchCode+ ''' and PartNo=''' 
										  +@curPartNo + ''''
					execute sp_executesql @sqlString, 
										N'@MovingCode		varchar(15) output,
										  @ABCClass 		char(1) 	output,
										  @PartCategory 	varchar(3) 	output',
										  @MovingCode 		output,
										  @ABCClass 		output,
										  @PartCategory 	output

			 -- SD: Location Code
				set @sqlString = N'select @LocationCode=LocationCode from ' 
										  +@DBName+ '..spMstItemLoc where CompanyCode=''' 
										  +@curCompanyCode+ ''' and BranchCode=''' 
										  +@curBranchCode+ ''' and PartNo=''' 
										  +@curPartNo + ''' and WarehouseCode=''00'''
					execute sp_executesql @sqlString, 
										N'@LocationCode 	varchar(15) output', 
										  @LocationCode 	output

				if @swDocDate     <> convert(varchar,@curDocDate,111) or
				   @swCompanyCode <> @curCompanyCode or
				   @swBranchCode  <> @curBranchCode  or
				   @swDocNo		  <> @curDocNo       or
				   @swTypeOfGoods <> @curTypeOfGoods
					begin
						set @swDocDate     = convert(varchar,@curDocDate,111)
						set @swCompanyCode = @curCompanyCode
						set @swBranchCode  = @curBranchCode
						set @swDocNo	   = @curDocNo
						set @swTypeOfGoods = @curTypeOfGoods
						set @SeqNo		   = 0

					 -- Discount Flag
						--set @sqlString = N'select top 1 @DiscFlag=1 from ' 
						--						  +@DBName+ '..svSDMovement where CompanyCode=''' 
						--						  +@curCompanyCode+ ''' and BranchCode=''' 
						--						  +@curBranchCode+ ''' and DocNo=''' 
						--						  +@curDocNo+ ''''
						--	execute sp_executesql @sqlString, 
						--						N'@DiscFlag 	integer output', 
						--						  @DiscFlag  	output
						set @DiscFlag      = 1
						set @TotalNetSales = 0
					 -- SD : Insert data to table spTrnPPOSHdr
						execute [usprpt_PostingMultiCompany4DocNo] @curCompanyCode, @curBranchCode, @DBName, 'POS', @POSNo output
						set @sqlString = 'insert into ' +@DBName+ '..spTrnPPOSHdr 
										(CompanyCode, BranchCode, POSNo, POSDate, SupplierCode, OrderType, isBO, isSubstution, isSuggorProcess, 
										 Remark, ProductType, PrintSeq, ExPickingSlipNo, ExPickingSlipDate, Status, Transportation, TypeOfGoods, 
										 isGenPORDD, isDeleted, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate, isLocked, LockingBy, 
										 LockingDate, isDropSign, DropSignReffNo)
										values(''' +@curCompanyCode+ ''',''' +@curBranchCode+ ''',''' +@POSNo+ ''',''' 
										           +convert(varchar,@curDocDate,121)+ ''',''' +@curBranchMD+ ''',''S'',0,0,0,''' 
												   +@curDocNo+ ''',''' +@curProductType+ ''',1,'''',NULL,7,NULL,''' 
												   +@curTypeOfGoods+ ''',0,0,''POSTING'',''' +convert(varchar,@PostingDate,111)+ 
											   ''',''POSTING'',''' +@currentDate+ ''',0,NULL,NULL,0,NULL)'
							execute sp_executesql @sqlString

					 -- MD : Insert data to table spTrnSORDHdr 
						execute [usprpt_PostingMultiCompany4DocNo] @curCompanyMD, @curBranchMD, @DBNameMD, 'SOC', @SONo output
						set @sqlString = 'insert into ' +@DBNameMD+ '..spTrnSORDHdr 
										(CompanyCode, BranchCode, DocNo, DocDate, UsageDocNo, UsageDocDate, CustomerCode, CustomerCodeBill,
										 CustomerCodeShip, isBO, isSubstitution, isIncludePPN, TransType, SalesType, isPORDD, OrderNo, OrderDate,
										 TOPCode, TOPDays, PaymentCode, PaymentRefNo, TotSalesQty, TotSalesAmt, TotDiscAmt, TotDPPAmt, TotPPNAmt,
										 TotFinalSalesAmt, isPKP, ExPickingSlipNo, ExPickingSlipDate, Status, PrintSeq, TypeOfGoods, 
										 CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate, isLocked, LockingBy, LockingDate, isDropSign)
										values(''' +@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' +@SONo+ ''',''' 
										           +convert(varchar,@curDocDate,121)+ ''',NULL,NULL,''' +@curBranchCode+ ''',''' 
												   +@curBranchCode+ ''',''' +@curBranchCode+ ''',0,0,1,''00'',''0'',0,''' 
												   +@POSNo+ ''',''' +convert(varchar,@curDocDate,121)+ ''',''' +@TopCodeMD+ ''',''' 
												   +convert(varchar,@TopDaysMD)+ ''',''' +@PaymentCodeMD+ ''',NULL,0.00,0,0,0,0,0,1,NULL,NULL,5,1,''' 
												   +@TypeOfGoodsMD+ ''',''POSTING'',''' +convert(varchar,@PostingDate,111)+ ''',''POSTING'',''' 
												   +@currentDate+ ''',0,''' +@curDocNo+ ''',''' +convert(varchar,@curDocDate,121)+ ''',0)'
							execute sp_executesql @sqlString

					 -- MD: Insert Data to table spTrnSPickingHdr 
						execute [usprpt_PostingMultiCompany4DocNo] @curCompanyMD, @curBranchMD, @DBNameMD, 'PLS', @PLNo output
						set @sqlString = 'insert into ' +@DBNameMD+ '..spTrnSPickingHdr 
										(CompanyCode, BranchCode, PickingSlipNo, PickingSlipDate, CustomerCode, CustomerCodeBill, 
										 CustomerCodeShip, PickedBy, isBORelease, isSubstitution, isIncludePPN, TransType, SalesType, 
										 TotSalesQty, TotSalesAmt, TotDiscAmt, TotDppAmt, TotPPNAmt, TotFinalSalesAmt, Remark, Status, 
										 PrintSeq, TypeOfGoods, CreatedBy, CreatedDate, LastupdateBy, LastUpdateDate, isLocked, LockingBy, LockingDate)
										values(''' +@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' +@PLNo+ 
										       ''',''' +convert(varchar,@curDocDate,121)+ ''',''' +@curBranchCode+
											   ''',''' +@curBranchCode+ ''',''' +@curBranchCode+ ''',''POSTING'',0,0,1,''00'',''0'',0,0,0,0,0,0,''' 
											   +@curDocNo+ ''',2,1,''' +@TypeOfGoodsMD+ ''',''POSTING'',''' +convert(varchar,@PostingDate,111)+ 
											   ''',''POSTING'',''' +@currentDate+ ''',0,NULL,NULL)'
							execute sp_executesql @sqlString

					 -- MD: Insert Data to table spTrnSInvoiceHdr 
						execute [usprpt_PostingMultiCompany4DocNo] @curCompanyMD, @curBranchMD, @DBNameMD, 'INV', @INVNo output
						execute [usprpt_PostingMultiCompany4DocNo] @curCompanyMD, @curBranchMD, @DBNameMD, 'FPJ', @FPJNo output
						set @sqlString = 'insert into ' +@DBNameMD+ '..spTrnSInvoiceHdr 
										(CompanyCode, BranchCode, InvoiceNo, InvoiceDate, PickingSlipNo, PickingSlipDate, FPJNo, FPJDate,
										 TransType, SalesType, CustomerCode, CustomerCodeBill, CustomerCodeShip, TotSalesQty, TotSalesAmt, 
										 TotDiscAmt, TotDppAmt, TotPPNAmt, TotFinalSalesAmt, Status, PrintSeq, TypeOfGoods, 
										 CreatedBy, CreatedDate, LastupdateBy, LastUpdateDate, isLocked, LockingBy, LockingDate)
										values(''' +@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' +@INVNo+ 
										       ''',''' +convert(varchar,@curDocDate,121)+ ''',''' +@PLNo+ 
											   ''',''' +convert(varchar,@curDocDate,121)+ ''',''' +@FPJNo+
											   ''',''' +convert(varchar,@curDocDate,121)+ ''',''00'',''0'','''
													   +@curBranchCode+ ''',''' +@curBranchCode+ ''',''' +@curBranchCode+ 
											   ''',0,0,0,0,0,0,''2'',1,''' +left(@TypeOfGoodsMD,1)+
											   ''',''POSTING'',''' +convert(varchar,@PostingDate,111)+ 
											   ''',''POSTING'',''' +@currentDate+ ''',0,NULL,NULL)'
							execute sp_executesql @sqlString

					 -- MD: Insert Data to table spTrnSFPJHdr 
						--Tax / Seri Pajak Numbering

					  --if @curBranchCode<>@swTaxBranch or left(@curDocNo,2)<>left(@swTaxDoc,2)
						if convert(varchar,@curDocDate,111)<>@swTaxDocDate or @curBranchCode<>@swTaxBranch or @curDocSort<>@swDocSort
							begin
								set @swTaxDocDate = convert(varchar,@curDocDate,111)
								set @swTaxBranch  = @curBranchCode
								set @swDocSort    = @curDocSort
							  --set @swTaxDoc	  = @curDocNo

								set @sqlString = N'select top 1 @TaxSeq=FPJSeqNo, @TaxSeqNo=SeqNo from ' 
																+@TaxDb+ '..gnMstFPJSeqNo where CompanyCode=''' 
																+@TaxCompany+ ''' and BranchCode=''' 
																+@TaxBranch+ ''' and Year=''' 
																+convert(varchar,year(@PostingDate))+ 
																''' and isActive=1 and convert(varchar,EffectiveDate,111)<=''' 
																+convert(varchar,@PostingDate,111)+ 
																''' order by CompanyCode, BranchCode, Year, SeqNo'
									execute sp_executesql @sqlString, 
														N'@TaxSeq 		bigint  	output,
														  @TaxSeqNo 	int 		output', 
														  @TaxSeq 		output,
														  @TaxSeqNo 	output

								set @sqlString = N'select top 1 @TaxTransCode=TaxTransCode from ' 
														  +@TaxDb+ '..gnMstCoProfile where CompanyCode=''' 
														  +@TaxCompany + ''' and BranchCode=''' 
														  +@TaxBranch+ ''''
									execute sp_executesql @sqlString, 
														N'@TaxTransCode varchar(3) 	output', 
														  @TaxTransCode output

								set @TaxSeq = @TaxSeq + 1

								set @sqlString = 'update ' +@TaxDb+ '..gnMstFPJSeqNo
										  			 set FPJSeqNo = ' +convert(varchar,@TaxSeq)+ 
												 ' where CompanyCode=''' +@TaxCompany + ''' and BranchCode=''' 
														+@TaxBranch + ''' and Year= ''' 
														+convert(varchar,year(@PostingDate))+ ''' and SeqNo= ''' 
														+convert(varchar,@TaxSeqNo)+ ''''
									execute sp_executesql @sqlString 

								--set @SeriPajakNo = @TaxTransCode + '0.' +isnull(replicate('0',11-len(convert(varchar,@TaxSeq))),'') + 
								--					+left(convert(varchar,@TaxSeq),len(convert(varchar,@TaxSeq))-8) + '-' +
								--					+right(convert(varchar,year(@PostingDate)),2)+ '.' +right(convert(varchar,@TaxSeq),8)
								if len(convert(varchar,@TaxSeq))>8
									set @SeriPajakNo =  @TaxTransCode + '0.' + replicate('0', 3-(len(convert(varchar,@TaxSeq))-8)) +
														left(convert(varchar,@TaxSeq),len(convert(varchar,@TaxSeq))-8) +
														'-' +right(convert(varchar,year(@PostingDate)),2)+ '.' +right(convert(varchar,@TaxSeq),8)
								else
									set @SeriPajakNo =  @TaxTransCode + '0.000-'+right(convert(varchar,year(@PostingDate)),2)+ '.'
														+replicate('0',8-len(convert(varchar,@TaxSeq)))+convert(varchar,@TaxSeq)
							end

						set @sqlString = 'insert into ' +@DBNameMD+ '..spTrnSFPJHdr 
										(CompanyCode, BranchCode, FPJNo, FPJDate, TPTrans, FPJGovNo, FPJSignature, 
										 FPJCentralNo, FPJCentralDate, DeliveryNo, InvoiceNo, InvoiceDate, PickingSlipNo, 
										 PickingSlipDate, TransType, CustomerCode, CustomerCodeBill, CustomerCodeShip, 
										 TOPCode, TOPDays, DueDate, TotSalesQty, TotSalesAmt, TotDiscAmt, TotDppAmt, TotPPNAmt, 
										 TotFinalSalesAmt, isPKP, Status, PrintSeq, TypeOfGoods, CreatedBy, CreatedDate, 
										 LastupdateBy, LastUpdateDate, isLocked, LockingBy, LockingDate)
										values(''' +@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' +@FPJNo+ ''',''' 
										           +convert(varchar,@curDocDate,121)+ ''',''P'',''' +@SeriPajakNo+ ''','''
											       +convert(varchar,@curDocDate,121)+ ''',NULL,'''
												   +convert(varchar,@curDocDate,121)+ ''',''' +@FPJNo+ ''',''' 
												   +@INVNo+ ''',''' +convert(varchar,@curDocDate,121)+ ''',''' 
												   +@PLNo+ ''',''' +convert(varchar,@curDocDate,121)+ ''',''00'',''' 
												   +@curBranchCode+ ''',''' +@curBranchCode+ ''',''' +@curBranchCode+ ''',''' 
												   +@TopCodeMD+ ''',''' +convert(varchar,@TopDaysMD)+ ''',''' 
												   +convert(varchar,@DueDateMD,121)+ ''',0,0,0,0,0,0,1,1,1,''' 
												   +@TypeOfGoodsMD+ ''',''POSTING'',''' +convert(varchar,@PostingDate,111)+ 
												   ''',''POSTING'',''' +@currentDate+ ''',0,NULL,NULL)'
							execute sp_executesql @sqlString

					 -- MD: Insert Data to table spTrnSFPJInfo
						set @sqlString = N'select @CustomerNameMD=CustomerGovName, @Address1MD=Address1, @Address2MD=Address2, ' +
												  '@Address3MD=Address3, @Address4MD=Address4, @isPKPMD=isPKP, @NPWPNoMD=NPWPNo, ' +
												  '@SKPNoMD=SKPNo, @SKPDateMD=SKPDate, @NPWPDateMD=NPWPDate from ' 
												  +@DBNameMD+ '..gnMstCustomer where CompanyCode=''' 
												  +@curCompanyMD+ ''' and CustomerCode=''' 
												  +@curBranchCode+ ''''
							execute sp_executesql @sqlString, 
												N'@CustomerNameMD 	varchar(100) output,
												  @Address1MD 		varchar(100) output,
												  @Address2MD 		varchar(100) output,
												  @Address3MD 		varchar(100) output,
												  @Address4MD 		varchar(100) output,
												  @isPKPMD 			bit 		 output,
												  @NPWPNoMD 		varchar(100) output,
												  @SKPNoMD 			varchar(100) output,
												  @SKPDateMD 		datetime 	 output,
												  @NPWPDateMD 		datetime     output', 
												  @CustomerNameMD 	output,
												  @Address1MD 		output,
												  @Address2MD 		output,
												  @Address3MD 		output,
												  @Address4MD 		output,
												  @isPKPMD 			output,
												  @NPWPNoMD 		output,
												  @SKPNoMD 			output,
												  @SKPDateMD 		output,
												  @NPWPDateMD 		output

						set @CustomerNameMD = isnull(@CustomerNameMD,'')
						set @Address1MD 	= isnull(@Address1MD,'')
						set @Address2MD 	= isnull(@Address2MD,'')
						set @Address3MD 	= isnull(@Address3MD,'')
						set @Address4MD 	= isnull(@Address4MD,'')
						set @isPKPMD     	= isnull(@isPKPMD,'')
						set @NPWPNoMD     	= isnull(@NPWPNoMD,'')
						set @SKPNoMD     	= isnull(@SKPNoMD,'')
						set @SKPDateMD     	= isnull(@SKPDateMD,'1900/01/01')
						set @NPWPDateMD     = isnull(@NPWPDateMD,'1900/01/01')

						set @sqlString = 'insert into ' +@DBNameMD+ '..spTrnSFPJInfo 
										(CompanyCode, BranchCode, FPJNo, CustomerName, Address1, Address2, 
										 Address3, Address4, isPKP, NPWPNo, SKPNo, SKPDate, NPWPDate, 
										 CreatedBy, CreatedDate, LastupdateBy, LastUpdateDate)
										values(''' +@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' +@FPJNo+ 
										       ''',''' +@CustomerNameMD+ ''',''' +@Address1MD+ ''',''' 
											   +@Address2MD+ ''',''' +@Address3MD+ ''',''' +@Address4MD+ 
											   ''',''' +@isPKPMD+ ''',''' +@NPWPNoMD+ ''',''' +@SKPNoMD+ 
											   ''',''' +convert(varchar,@SKPDateMD,121)+ ''',''' 
											   +convert(varchar,@NPWPDateMD,121)+ ''',''POSTING'',''' 
											   +convert(varchar,@PostingDate,121)+ ''',''POSTING'',''' 
											   +@currentDate+ ''')'
							execute sp_executesql @sqlString

					 -- MD: Insert data to table gnGenerateTax 
						set @sqlString = 'insert into ' +@DBNameMD+ '..gnGenerateTax
													   (CompanyCode, BranchCode, PeriodTaxYear, PeriodTaxMonth, 
														ProfitCenterCode, FPJGovNo, FPJGovDate, DocNo, DocDate, 
														RefNo, RefDate, CreatedBy, CreatedDate)
								                 values(''' +@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' 
														+convert(varchar,year(@curDocDate))+ ''','''
														+convert(varchar,month(@curDocDate))+ ''',300,'''
														+@SeriPajakNo+ ''',''' +convert(varchar,@curDocDate,121)+ ''',''' 
														+@INVNo+ ''',''' +convert(varchar,@curDocDate,121)+ ''','''
														+@FPJNo+ ''',''' +convert(varchar,@curDocDate,121)+ 
														''',''POSTING'',''' +@CurrentDate+ ''')'
														--''',''POSTING'',''' +convert(varchar,@curDocDate,121)+ ''')'
							execute sp_executesql @sqlString

					 -- SD : Insert data to table spTrnPBinnHdr 
						execute [usprpt_PostingMultiCompany4DocNo] @curCompanyCode, @curBranchCode, @DBName, 'BNL', @BINNo output
						set @sqlString = 'insert into ' +@DBName+ '..spTrnPBinnHdr 
										(CompanyCode, BranchCode, BinningNo, BinningDate, ReceivingType, DNSupplierNo, DNSupplierDate, TransType, 
										 SupplierCode, ReferenceNo, ReferenceDate, TotItem, TotBinningAmt, Status, PrintSeq, TypeOfGoods, 
										 CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate, isLocked, LockingBy, LockingDate)
										values(''' +@curCompanyCode+ ''',''' +@curBranchCode+ ''',''' +@BINNo+ ''',''' 
										           +convert(varchar,@curDocDate,121)+ ''',''1'',''' +@PLNo+ ''','''
												   +convert(varchar,@curDocDate,121)+ ''',''4'',''' +@curBranchMD+ ''','''
												   +@FPJNo+ ''',''' +convert(varchar,@curDocDate,121)+ ''',0,0,4,1,''' 
												   +@curTypeOfGoods+ ''',''POSTING'',''' +convert(varchar,@PostingDate,111)+ 
												   ''',''POSTING'',''' +@currentDate+ ''',0,NULL,NULL)'
							execute sp_executesql @sqlString
					
					 -- SD : Insert data to table spTrnPRcvHdr 
						execute [usprpt_PostingMultiCompany4DocNo] @curCompanyCode, @curBranchCode, @DBName, 'WRL', @WRSNo output
						set @sqlString = 'insert into ' +@DBName+ '..spTrnPRcvHdr 
										(CompanyCode, BranchCode, WRSNo, WRSDate, BinningNo, BinningDate, ReceivingType, 
										 DNSupplierNo, DNSupplierDate, TransType, SupplierCode, ReferenceNo, ReferenceDate, 
										 TotItem, TotWRSAmt, Status, PrintSeq, TypeOfGoods, 
										 CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate, isLocked, LockingBy, LockingDate)
										values(''' +@curCompanyCode+ ''',''' +@curBranchCode+ ''',''' +@WRSNo+ ''','''
												   +convert(varchar,@curDocDate,121)+ ''',''' +@BINNo+ ''',''' 
										           +convert(varchar,@curDocDate,121)+ ''',''1'',''' +@PLNo+ ''','''
												   +convert(varchar,@curDocDate,121)+ ''',''4'',''' 
												   +@curBranchMD+ ''',''' +@FPJNo+ ''','''
												   +convert(varchar,@curDocDate,121)+ ''',0,0,4,1,''' +@curTypeOfGoods+ 
											   ''',''POSTING'',''' +convert(varchar,@PostingDate,111)+ ''',''POSTING'',''' 
											       +@currentDate+ ''',0,NULL,NULL)'
							execute sp_executesql @sqlString

					 -- SD : Insert data to spTrnPHPP 
						execute [usprpt_PostingMultiCompany4DocNo] @curCompanyCode, @curBranchCode, @DBName, 'HPP', @HPPNo output
						execute [usprpt_PostingMultiCompany4DocNo] @curCompanyCode, @curBranchCode, @DBName, 'BNP', @APNo output
						set @sqlString = 'insert into ' +@DBName+ '..spTrnPHPP 
										(CompanyCode, BranchCode, HPPNo, HPPDate, WRSNo, WRSDate, ReferenceNo, ReferenceDate,
										 TotPurchAmt, TotNetPurchAmt, TotTaxAmt, TaxNo, TaxDate, MonthTax, YearTax, DueDate,
										 DiffNetPurchAmt, DiffTaxAmt, TotHppAmt, CostPrice, PrintSeq, TypeOfGoods, Status, 
										 CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
										values(''' +@curCompanyCode+ ''',''' +@curBranchCode+ ''',''' +@HPPNo+ ''',''' 
												   +convert(varchar,@curDocDate,121)+ ''',''' +@WRSNo+ ''','''
												   +convert(varchar,@curDocDate,121)+ ''',''' +@FPJNo+ ''','''
												   +convert(varchar,@curDocDate,121)+ ''',0,0,0,''' +@SeriPajakNo+ ''','''
												   +convert(varchar,@curDocDate,121)+ ''',''' 
												   +convert(varchar,month(@curDocDate),121)+ ''',''' 
												   +convert(varchar,year(@curDocDate),121)+ ''',''' 
												   +convert(varchar,@DueDateMD,111)+ ''',0,0,0,0,1,''' 
												   +@curTypeOfGoods+ ''',2,''POSTING'',''' 
												   +convert(varchar,@PostingDate,111)+ ''',''POSTING'',''' 
												   +@currentDate+ ''')'
							execute sp_executesql @sqlString
					end   

				set @SeqNo         = @SeqNo         + 1
				set @TotalNetSales = @TotalNetSales + @NetSales
				set @TotalPPNAmt   = @TotalNetSales * 0.1
				set @TotalSalesAmt = @TotalNetSales + @TotalPPNAmt

			 -- SD: Insert data to table spTrnPPOSDtl 
				set @sqlString = 'insert into ' +@DBName+ '..spTrnPPOSDtl
										(CompanyCode, BranchCode, POSNo, PartNo, SeqNo, OrderQty, SuggorQty, PurchasePrice, DiscPct,
										 PurchasePriceNett, CostPrice, TotalAmount, ABCCLass, MovingCode, ProductType, PartCategory,
										 CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate, Note)
								   values(''' +@curCompanyCode+ ''',''' +@curBranchCode+ ''',''' +@POSNo+ ''',''' +@curPartNo+ 
										  ''',' +convert(varchar,@SeqNo)+ ',' +convert(varchar,@curQty)+ 
										  ',' +convert(varchar,@curQtyOrder)+ ',' +convert(varchar,@curRetailPrice)+ 
										  ',' +convert(varchar,@DiscPct)+ ',' +convert(varchar,@RetailPriceNet)+ 
										  ',' +convert(varchar,@RetailPriceNet)+ ',' +convert(varchar,@TotPurchaseNetAmt)+ 
										  ',''' +@ABCClass+ ''',''' +@MovingCode+ ''',''' +left(@curProductType,3)+
										  ''',''' +@PartCategory+ ''',''' +@curCreatedBy+ ''',''' +convert(varchar,@curCreatedDate,121)+ 
										  ''',''' +@curLastUpdateBy+ ''',''' +convert(varchar,@curLastUpdateDate,121)+ 
										  ''',''' +@curDocNo+ ''')'
					execute sp_executesql @sqlString  

			 -- SD: Insert data to table spTrnPOrderBalance
				set @sqlString = 'insert into ' +@DBName+ '..spTrnPOrderBalance
										(CompanyCode, BranchCode, POSNo, SupplierCode, PartNo, SeqNo, PartNoOriginal, 
										 POSDate, OrderQty, OnOrder, Intransit, Received, Located, DiscPct, PurchasePrice, 
										 CostPrice, ABCClass, MovingCode, WRSNo, WRSDate, TypeOfGoods, 
										 CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
								   values(''' +@curCompanyCode+ ''',''' +@curBranchCode+ ''',''' +@POSNo+ ''',''' 
											  +@curBranchMD+ ''',''' +@curPartNo+ ''',' +convert(varchar,@SeqNo)+ ','''
											  +@curPartNo+ ''',''' +convert(varchar,@curDocDate,121)+ ''','
											  +convert(varchar,@curQty)+ ',0,0,' +convert(varchar,@curQty)+ ','
											  +convert(varchar,@curQty)+ ',' +convert(varchar,@DiscPct)+ ','
											  +convert(varchar,@RetailPriceNet)+ ',' +convert(varchar,@RetailPriceNet)+ ','''
											  +@ABCClass+ ''',''' +@MovingCode+ ''',''' +@WRSNo+ ''',''' 
											  +convert(varchar,@curDocDate,121)+ ''',''' +@curTypeOfGoods+ ''',''' 
											  +@curCreatedBy+ ''',''' +convert(varchar,@curCreatedDate,121)+ ''',''' 
											  +@curLastUpdateBy+ ''',''' +convert(varchar,@curLastUpdateDate,121)+ ''')'
					execute sp_executesql @sqlString

			 -- MD: Insert data to table spTrnSORDDtl
				set @sqlString = 'insert into ' +@DBNameMD+ '..spTrnSORDDtl
										(CompanyCode, BranchCode, DocNo, PartNo, WarehouseCode, PartNoOriginal, 
										 ReferenceNo, ReferenceDate, LocationCode, QtyOrder, QtySupply, QtyBO, 
										 QtyBOSupply, QtyBOCancel, QtyBill, RetailPriceInclTax, RetailPrice,
										 CostPrice, DiscPct, SalesAmt, DiscAmt, NetSalesAmt, PPNAmt, TotSalesAmt, 
										 MovingCode, ABCCLass, ProductType, PartCategory, Status,
										 CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate, 
										 StockAllocatedBy, StockAllocatedDate, FirstDemandQty)
								   values(''' +@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' +@SONo+ 
								          ''',''' +@curPartNo+ ''',''' +@curWarehouseCode+ ''',''' +@curPartNo+ 
										  ''',''' +@SONo+ ''',''' +convert(varchar,@curDocDate,121)+ 
										  ''',''' +@LocationCodeMD+ ''',''' +convert(varchar,@curQty)+ 
										  ''',''' +convert(varchar,@curQty)+ ''',0,0,0,'''
										  +convert(varchar,@curQty)+ ''',''' +convert(varchar,@curRetailPriceInclTaxMD)+
										  ''',''' +convert(varchar,@curRetailPriceMD)+ ''',''' +convert(varchar,@curCostPriceMD)+ ''','''
										  +convert(varchar,@DiscPct)+ ''',''' +convert(varchar,@SalesAmt)+ ''','''
										  +convert(varchar,@DiscAmt)+ ''',''' +convert(varchar,@NetSales)+ ''','''
										  +convert(varchar,@PPNAmt)+ ''',''' +convert(varchar,@TotSalesAmt)+ ''','''
										  +@MovingCode+ ''',''' +left(@ABCClass,1)+ ''',''' +left(@curProductType,3)+
										  ''',''' +@PartCategory+ ''',5,''' +@curCreatedBy+ 
										  ''',''' +convert(varchar,@curCreatedDate,121)+ 
										  ''',''' +@curLastUpdateBy+ ''',''' +convert(varchar,@curLastUpdateDate,121)+ 
										  ''',''' +@curCreatedBy+ ''',''' +convert(varchar,@curCreatedDate,121)+ 
										  ''',''' +convert(varchar,@curQtyOrder)+ ''')'
					execute sp_executesql @sqlString

			 -- MD: Insert data to table spTrnSOSupply 
				set @sqlString = 'insert into ' +@DBNameMD+ '..spTrnSOSupply
										(CompanyCode, BranchCode, DocNo, SupSeq, PartNo, PartNoOriginal, PTSeq, 
										 PickingSlipNo, ReferenceNo, ReferenceDate, WarehouseCode, LocationCode, 
										 QtySupply, QtyPicked, QtyBill, RetailPriceInclTax, RetailPrice, CostPrice, 
										 DiscPct, MovingCode, ABCCLass, ProductType, PartCategory, Status,
										 CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
								   values(''' +@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' +@SONo+ 
								          ''',0,''' +@curPartNo+ ''',''' +@curPartNo+ 
										  ''',''' +convert(varchar,@SeqNo)+ ''',''' +@PLNo+
										  ''',''' +@POSNo+ ''',''' +convert(varchar,@curCreatedDate,121)+
										  ''',''' +@curWarehouseCode+ ''',''' +@LocationCodeMD+ 
										  ''',''' +convert(varchar,@curQty)+ ''',''' +convert(varchar,@curQty)+ 
										  ''',''' +convert(varchar,@curQty)+ ''',''' +convert(varchar,@curRetailPriceInclTaxMD)+
										  ''',''' +convert(varchar,@curRetailPriceMD)+ ''',''' +convert(varchar,@curCostPriceMD)+ 
										  ''',''' +convert(varchar,@DiscPct)+ ''',''' +@MovingCode+ ''',''' +@ABCClass+ 
										  ''',''' +@ProductTypeMD+ ''',''' +@PartCategory+ ''',2,''' +@curCreatedBy+ 
										  ''',''' +convert(varchar,@curCreatedDate,121)+ 
										  ''',''' +@curLastUpdateBy+ ''',''' +convert(varchar,@curLastUpdateDate,121)+ ''')'
					execute sp_executesql @sqlString

			 -- MD: Update data to table spTrnSORDHdr
				set @sqlString = 'update ' +@DBNameMD+ '..spTrnSORDHdr
								     set TotSalesQty = TotSalesQty + ' +convert(varchar,@CurQty)+ ', TotSalesAmt = TotSalesAmt + ' +convert(varchar,@SalesAmt)+ 
								      ', TotDiscAmt = TotDiscAmt + ' +convert(varchar,@DiscAmt)+ ', TotDppAmt = TotDppAmt + ' +convert(varchar,@NetSales)+ 
								      ', TotPpnAmt = TotPpnAmt + ' +convert(varchar,@PpnAmt)+ ', TotFinalSalesAmt = TotFinalSalesAmt + ' +convert(varchar,@TotSalesAmt)+ 
								 ' where CompanyCode=''' +@curCompanyMD+ ''' and BranchCode=''' +@curBranchMD+ ''' and DocNo=''' +@SONo + ''''
					execute sp_executesql @sqlString 

			 -- MD: Insert data to table spTrnIMovement  (KEY: CompanyCode, BranchCode, DocNo, DocDate, CreatedDate => PartNo)
				set @sqlString = 'insert into ' +@DBNameMD+ '..spTrnIMovement
										(CompanyCode, BranchCode, DocNo, DocDate, CreatedDate, WarehouseCode, 
										 LocationCode, PartNo, SignCode, SubSignCode, Qty, Price, CostPrice, 
										 ABCClass, MovingCode, ProductType, PartCategory, CreatedBy)
								   values(''' +@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' +@SONo+ 
								          ''',''' +convert(varchar,@curDocDate,121)+ 
								          ''',''' +convert(varchar,getdate(),121)+ 
										  ''',''' +@curWarehouseCode+ ''',''' +@LocationCodeMD+ 
										  ''',''' +@curPartNo+ ''',''OUT'',''SA-PJUAL'','''
										  +convert(varchar,@curQty)+ ''',''' +convert(varchar,@curRetailPriceMD)+
										  ''',''' +convert(varchar,@curCostPriceMD)+ ''',''' +@ABCClassMD+
										  ''',''' +@MovingCodeMD+ ''',''' +@ProductTypeMD+
										  ''',''' +@PartCategoryMD+ ''',''POSTING'')'
					execute sp_executesql @sqlString

			 -- MD: Insert/Update data to table spHstDemandItem 
				set @sqlString = 'merge into ' +@DBNameMD+ '..spHstDemandItem as DMN using (values(''' 
							     +@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' +convert(varchar,year(@curDocDate))+ ''',''' 
								 +convert(varchar,month(@curDocDate))+ ''',''' +@curPartNo+ ''',1,''' 
								 +convert(varchar,@curQtyOrder)+ ''',1,''' +convert(varchar,@curQty)+ ''',''' 
								 +@MovingCodeMD+ ''',''' +@ProductTypeMD+ ''',''' +@PartCategoryMD+ ''',''' 
								 +@ABCClassMD+ ''',''POSTING'',''' +convert(varchar,@curLastUpdateDate,121)+ '''))
					    as SRC (NewCompany, NewBranch, NewYear, NewMonth, NewPart, NewDemandFreq, 
								NewDemandQty, NewSalesFreq, NewSalesQty, NewMovingCode, NewProductType, 
								NewPartCategory, NewABCClass, NewLastUpdateBy, NewLastUpdateDate)
						on DMN.CompanyCode = SRC.NewCompany
					   and DMN.BranchCode  = SRC.NewBranch
					   and DMN.Year        = SRC.NewYear
					   and DMN.Month       = SRC.NewMonth
					   and DMN.PartNo      = SRC.NewPart
					  when matched 
						   then update set DemandFreq     = DemandFreq + SRC.NewDemandFreq
						                 , DemandQty      = DemandQty  + SRC.NewDemandQty
										 , SalesFreq      = SalesFreq  + SRC.NewSalesFreq
										 , SalesQty       = SalesQty   + SRC.NewSalesQty
										 , LastUpdateBy   = ''POSTING''
										 , LastUpdateDate = ''' +convert(varchar,@curLastUpdateDate,121)+ '''
					  when not matched by target 
						   then insert (CompanyCode, BranchCode, Year, Month, PartNo, DemandFreq, DemandQty, SalesFreq, SalesQty,
						                MovingCode, ProductType, PartCategory, ABCCLass, LastUpdateBy, LastUpdateDate)
								values (NewCompany, NewBranch, NewYear, NewMonth, NewPart, NewDemandFreq, NewDemandQty, 
								        NewSalesFreq, NewSalesQty, NewMovingCode, NewProductType, NewPartCategory, 
										NewABCClass, NewLastUpdateBy, NewLastUpdateDate);'
					execute sp_executesql @sqlString
			 --MD: Insert/Update data to table spHstDemandcust 
				set @sqlString = 'merge into ' +@DBNameMD+ '..spHstDemandcust as DMN using (values(''' 
							     +@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' +convert(varchar,year(@curDocDate))+ ''',''' 
								 +convert(varchar,month(@curDocDate))+ ''',''' +@curBranchCode+ ''',''' +@curPartNo+ ''',1,''' 
								 +convert(varchar,@curQtyOrder)+ ''',1,''' +convert(varchar,@curQty)+ ''',''' 
								 +@MovingCodeMD+ ''',''' +@ProductTypeMD+ ''',''' +@PartCategoryMD+ ''',''' 
								 +@ABCClassMD+ ''',''POSTING'',''' +convert(varchar,@curLastUpdateDate,121)+ '''))
					    as SRC (NewCompany, NewBranch, NewYear, NewMonth, NewCustomer, NewPart, NewDemandFreq, 
								NewDemandQty, NewSalesFreq, NewSalesQty, NewMovingCode, NewProductType, 
								NewPartCategory, NewABCClass, NewLastUpdateBy, NewLastUpdateDate)
						on DMN.CompanyCode = SRC.NewCompany
					   and DMN.BranchCode  = SRC.NewBranch
					   and DMN.Year        = SRC.NewYear
					   and DMN.Month       = SRC.NewMonth
					   and DMN.CustomerCode= SRC.NewCustomer
					   and DMN.PartNo      = SRC.NewPart
					  when matched 
						   then update set DemandFreq     = DemandFreq + SRC.NewDemandFreq
						                 , DemandQty      = DemandQty  + SRC.NewDemandQty
										 , SalesFreq      = SalesFreq  + SRC.NewSalesFreq
										 , SalesQty       = SalesQty   + SRC.NewSalesQty
										 , LastUpdateBy   = ''POSTING''
										 , LastUpdateDate = ''' +convert(varchar,@curLastUpdateDate,121)+ '''
					  when not matched by target 
						   then insert (CompanyCode, BranchCode, Year, Month, CustomerCode, PartNo, DemandFreq, DemandQty, SalesFreq, 
									    SalesQty, MovingCode, ProductType, PartCategory, ABCCLass, LastUpdateBy, LastUpdateDate)
								values (NewCompany, NewBranch, NewYear, NewMonth, NewCustomer, NewPart, NewDemandFreq, NewDemandQty, 
								        NewSalesFreq, NewSalesQty, NewMovingCode, NewProductType, NewPartCategory, 
										NewABCClass, NewLastUpdateBy, NewLastUpdateDate);'
					execute sp_executesql @sqlString

			 -- MD: Insert Data to table spTrnSPickingDtl
				set @sqlString = 'insert into ' +@DBNameMD+ '..spTrnSPickingDtl
										(CompanyCode, BranchCode, PickingSlipNo, WarehouseCode, PartNo, PartNoOriginal, 
										 DocNo, DocDate, ReferenceNo, ReferenceDate, LocationCode, QtyOrder, QtySupply, 
										 QtyPicked, QtyBill, RetailPriceInclTax, RetailPrice, CostPrice, DiscPct, 
										 SalesAmt, DiscAmt, NetSalesAmt, TotSalesAmt, MovingCode, ABCClass, ProductType, 
										 PartCategory, ExPickingSlipNo, ExPickingSlipDate, isClosed, 
										 CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
								   values(''' +@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' +@PLNo+ 
								          ''',''' +@curWarehouseCode+ ''',''' +@curPartNo+ ''',''' +@curPartNo+ 
										  ''',''' +@SONo+ ''',''' +convert(varchar,@curDocDate,121)+ 
										  ''',''' +@POSNo+ ''',''' +convert(varchar,@curDocDate,121)+ 
										  ''',''' +@LocationCodeMD+ ''',''' +convert(varchar,@curQtyOrder)+ 
										  ''',''' +convert(varchar,@curQty)+ ''',''' +convert(varchar,@curQty)+
										  ''',''' +convert(varchar,@curQty)+ ''',''' +convert(varchar,@curRetailPriceInclTaxMD)+
										  ''',''' +convert(varchar,@curRetailPriceMD)+ ''',''' +convert(varchar,@curCostPriceMD)+ 
										  ''',''' +convert(varchar,@DiscPct)+ ''',''' +convert(varchar,@SalesAmt)+ 
										  ''',''' +convert(varchar,@DiscAmt)+ ''',''' +convert(varchar,@NetSales)+ 
										  ''',''' +convert(varchar,@TotSalesAmt)+ ''',''' +@MovingCodeMD+ 
										  ''',''' +@ABCClassMD+ ''',''' +@ProductTypeMD+ ''',''' +@PartCategory+ 
										  ''',NULL,NULL,0,''' +@curCreatedBy+ ''',''' +convert(varchar,@curCreatedDate,121)+ 
										  ''',''' +@curLastUpdateBy+ ''',''' +convert(varchar,@curLastUpdateDate,121)+ ''')'
					execute sp_executesql @sqlString

			 -- MD: Update data to table spTrnSPickingHdr
				set @sqlString = 'update ' +@DBNameMD+ '..spTrnSPickingHdr
								     set TotSalesQty = TotSalesQty + ' +convert(varchar,@CurQty)+ ', TotSalesAmt = TotSalesAmt + ' +convert(varchar,@SalesAmt)+ 
								      ', TotDiscAmt = TotDiscAmt + ' +convert(varchar,@DiscAmt)+ ', TotDppAmt = TotDppAmt + ' +convert(varchar,@NetSales)+ 
								      ', TotPpnAmt = TotPpnAmt + ' +convert(varchar,@PpnAmt)+ ', TotFinalSalesAmt = TotFinalSalesAmt + ' +convert(varchar,@TotSalesAmt)+ 
								 ' where CompanyCode=''' +@curCompanyMD+ ''' and BranchCode=''' +@curBranchMD+ ''' and PickingSlipNo=''' +@PLNo + ''''
					execute sp_executesql @sqlString 

			 -- MD: Insert Data to table spTrnSInvoiceDtl
				set @sqlString = 'insert into ' +@DBNameMD+ '..spTrnSInvoiceDtl
										(CompanyCode, BranchCode, InvoiceNo, WarehouseCode, PartNo, PartNoOriginal, 
										 DocNo, DocDate, ReferenceNo, ReferenceDate, LocationCode, QtyBill, 
										 RetailPriceInclTax, RetailPrice, CostPrice, DiscPct, SalesAmt, DiscAmt, 
										 NetSalesAmt, PPNAmt, TotSalesAmt, ProductType, PartCategory, 
										 MovingCode, ABCClass, ExPickingListNo, ExPickingListDate, 
										 CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
								   values(''' +@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' +@INVNo+ 
								          ''',''' +@curWarehouseCode+ ''',''' +@curPartNo+ ''',''' +@curPartNo+ 
										  ''',''' +@SONo+ ''',''' +convert(varchar,@curDocDate,121)+ 
										  ''',''' +@POSNo+ ''',''' +convert(varchar,@curDocDate,121)+ 
										  ''',''' +@LocationCodeMD+ ''',''' ++convert(varchar,@curQty)+ 
										  ''',''' +convert(varchar,@curRetailPriceInclTaxMD)+
										  ''',''' +convert(varchar,@curRetailPriceMD)+ ''',''' +convert(varchar,@curCostPriceMD)+ 
										  ''',''' +convert(varchar,@DiscPct)+ ''',''' +convert(varchar,@SalesAmt)+ 
										  ''',''' +convert(varchar,@DiscAmt)+ ''',''' +convert(varchar,@NetSales)+ 
										  ''',''' +convert(varchar,@PPNAmt)+ ''',''' +convert(varchar,@TotSalesAmt)+ 
										  ''',''' +@ProductTypeMD+ ''',''' +@PartCategory+ ''',''' +@MovingCodeMD+ 
										  ''',''' +@ABCClassMD+ ''',NULL,NULL,''' 
										          +@curCreatedBy+ ''',''' +convert(varchar,@curCreatedDate,121)+ 
										  ''',''' +@curLastUpdateBy+ ''',''' +convert(varchar,@curLastUpdateDate,121)+ ''')'
					execute sp_executesql @sqlString

			 -- MD: Update data to table spTrnSInvoiceHdr
				set @sqlString = 'update ' +@DBNameMD+ '..spTrnSInvoiceHdr
								     set TotSalesQty = TotSalesQty + ' +convert(varchar,@CurQty)+ ', TotSalesAmt = TotSalesAmt + ' +convert(varchar,@SalesAmt)+ 
								      ', TotDiscAmt = TotDiscAmt + ' +convert(varchar,@DiscAmt)+ ', TotDppAmt = TotDppAmt + ' +convert(varchar,@NetSales)+ 
								      ', TotPpnAmt = ' +convert(varchar,@TotalPPNAmt)+ ', TotFinalSalesAmt = ' +convert(varchar,@TotalSalesAmt)+ 
								 ' where CompanyCode=''' +@curCompanyMD+ ''' and BranchCode=''' +@curBranchMD+ ''' and PickingSlipNo=''' +@PLNo + ''''
					execute sp_executesql @sqlString 

			 -- MD: Insert Data to table spTrnSFPJDtl
				set @sqlString = 'insert into ' +@DBNameMD+ '..spTrnSFPJDtl
										(CompanyCode, BranchCode, FPJNo, WarehouseCode, PartNo, PartNoOriginal, 
										 DocNo, DocDate, ReferenceNo, ReferenceDate, LocationCode, QtyBill, 
										 RetailPriceInclTax, RetailPrice, CostPrice, DiscPct, SalesAmt, DiscAmt, 
										 NetSalesAmt, PPNAmt, TotSalesAmt, ProductType, PartCategory, MovingCode, 
										 ABCClass, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
								   values(''' +@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' +@FPJNo+ 
								          ''',''' +@curWarehouseCode+ ''',''' +@curPartNo+ ''',''' +@curPartNo+ 
										  ''',''' +@SONo+ ''',''' +convert(varchar,@curDocDate,121)+ 
										  ''',''' +@POSNo+ ''',''' +convert(varchar,@curDocDate,121)+ 
										  ''',''' +@LocationCodeMD+ ''',''' ++convert(varchar,@curQty)+ 
										  ''',''' +convert(varchar,@curRetailPriceInclTaxMD)+
										  ''',''' +convert(varchar,@curRetailPriceMD)+ ''',''' +convert(varchar,@curCostPriceMD)+ 
										  ''',''' +convert(varchar,@DiscPct)+ ''',''' +convert(varchar,@SalesAmt)+ 
										  ''',''' +convert(varchar,@DiscAmt)+ ''',''' +convert(varchar,@NetSales)+ 
										  ''',''' +convert(varchar,@PPNAmt)+ ''',''' +convert(varchar,@TotSalesAmt)+ 
										  ''',''' +@ProductTypeMD+ ''',''' +@PartCategory+ ''',''' +@MovingCodeMD+ 
										  ''',''' +@ABCClassMD+ ''',''' +@curCreatedBy+ ''',''' +convert(varchar,@curCreatedDate,121)+ 
										  ''',''' +@curLastUpdateBy+ ''',''' +convert(varchar,@curLastUpdateDate,121)+ ''')'
					execute sp_executesql @sqlString

			 -- MD: Update data to table spTrnSFPJHdr
				set @sqlString = 'update ' +@DBNameMD+ '..spTrnSFPJHdr
								     set TotSalesQty=TotSalesQty + ' +convert(varchar,@CurQty)+ ', TotSalesAmt=TotSalesAmt + ' +convert(varchar,@SalesAmt)+ 
								      ', TotDiscAmt=TotDiscAmt + ' +convert(varchar,@DiscAmt)+ ', TotDppAmt=TotDppAmt + ' +convert(varchar,@NetSales)+ 
								      ', TotPpnAmt=' +convert(varchar,@TotalPPNAmt)+ ', TotFinalSalesAmt=' +convert(varchar,@TotalSalesAmt)+ 
								 ' where CompanyCode=''' +@curCompanyMD+ ''' and BranchCode=''' +@curBranchMD+ ''' and PickingSlipNo=''' +@PLNo + ''''
					execute sp_executesql @sqlString 

			 -- MD: Insert data to table spTrnIMovement  
				set @sqlString = 'insert into ' +@DBNameMD+ '..spTrnIMovement
										(CompanyCode, BranchCode, DocNo, DocDate, CreatedDate, WarehouseCode, 
										 LocationCode, PartNo, SignCode, SubSignCode, Qty, Price, CostPrice, 
										 ABCClass, MovingCode, ProductType, PartCategory, CreatedBy)
								   values(''' +@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' +@FPJNo+ 
								          ''',''' +convert(varchar,@curDocDate,121)+ 
								          ''',''' +convert(varchar,getdate(),121)+ 
										  ''',''' +@curWarehouseCode+ ''',''' +@LocationCodeMD+ 
										  ''',''' +@curPartNo+ ''',''OUT'',''FAKTUR'','''
										  +convert(varchar,@curQty)+ ''',''' +convert(varchar,@curRetailPriceMD)+
										  ''',''' +convert(varchar,@curCostPriceMD)+ ''',''' +@ABCClassMD+
										  ''',''' +@MovingCodeMD+ ''',''' +@ProductTypeMD+
										  ''',''' +@PartCategoryMD+ ''',''POSTING'')'
					execute sp_executesql @sqlString

			 -- MD: Insert/Update data to table arInterface
				--set @sqlString = 'merge into ' +@DBNameMD+ '..arInterface as ar using ( values(''' 
				--				 +@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' +@FPJNo+ ''',''' 
				--				 +convert(varchar,@curDocDate,121)+ ''',''300'',''' 
				--				 +convert(varchar,@TotSalesAmt)+ ''',0,''' 
				--				 +convert(varchar,@curBranchCode)+ ''',''' +@TopCodeMD+ ''',''' 
				--				 +convert(varchar,@DueDateMD,111)+ ''',''INVOICE'',0,0,0,'''
				--				 +@SalesCodeMD+ ''',NULL,0,''POSTING'',''' 
				--				 +convert(varchar,@PostingDate,111)+ ''',''' 
				--				 +@AccNoArMD+ ''',NULL,''' +convert(varchar,@curDocDate,111)+ '''))
				--		as SRC (NewCompany, NewBranch, NewDocNo, NewDocDate, NewProfitCenterCode, NewNettAmt, 
				--				NewReceiveAmt, NewCustomerCode, NewTOPCode, NewDueDate, NewTypeTrans, NewBlockAmt, 
				--				NewDebetAmt, NewCreditAmt, NewSalesCode, NewLeasingCode, NewStatusFlag, NewCreateBy, 
				--				NewCreateDate, NewAccountNo, NewFakturPajakNo, NewFakturPajakDate)
				--		on ar.CompanyCode = SRC.NewCompany
				--	   and ar.BranchCode  = SRC.NewBranch
				--	   and ar.DocNo       = SRC.NewDocNo
				--	  when matched 
				--		   then update set NettAmt = NettAmt + NewNettAmt
				--	  when not matched by target 
				--		   then insert (CompanyCode, BranchCode, DocNo, DocDate, ProfitCenterCode, NettAmt, ReceiveAmt, 
				--						 CustomerCode, TOPCode, DueDate, TypeTrans, BlockAmt, DebetAmt, CreditAmt, SalesCode, 
				--						 LeasingCode, StatusFlag, CreateBy, CreateDate, AccountNo, FakturPajakNo, FakturPajakDate)
				--				values (NewCompany, NewBranch, NewDocNo, NewDocDate, NewProfitCenterCode, NewNettAmt, 
				--						NewReceiveAmt, NewCustomerCode, NewTOPCode, NewDueDate, NewTypeTrans, NewBlockAmt, 
				--						NewDebetAmt, NewCreditAmt, NewSalesCode, NewLeasingCode, NewStatusFlag, NewCreateBy, 
				--						NewCreateDate, NewAccountNo, NewFakturPajakNo, NewFakturPajakDate);'
				set @sqlString = 'merge into ' +@DBNameMD+ '..arInterface as ar using ( values(''' 
								 +@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' +@FPJNo+ ''',''' 
								 +convert(varchar,@curDocDate,121)+ ''',''300'',''' 
								 +convert(varchar,@TotalSalesAmt)+ ''',0,''' 
								 +convert(varchar,@curBranchCode)+ ''',''' +@TopCodeMD+ ''',''' 
								 +convert(varchar,@DueDateMD,111)+ ''',''INVOICE'',0,0,0,'''
								 +@SalesCodeMD+ ''','' '',0,''POSTING'',''' 
								 +convert(varchar,@PostingDate,111)+ ''',''' +@AccNoArMD+ ''',''' 
								 +@SeriPajakNo+ ''',''' +convert(varchar,@curDocDate,111)+ '''))
						as SRC (NewCompany, NewBranch, NewDocNo, NewDocDate, NewProfitCenterCode, NewNettAmt, 
								NewReceiveAmt, NewCustomerCode, NewTOPCode, NewDueDate, NewTypeTrans, NewBlockAmt, 
								NewDebetAmt, NewCreditAmt, NewSalesCode, NewLeasingCode, NewStatusFlag, NewCreateBy, 
								NewCreateDate, NewAccountNo, NewFakturPajakNo, NewFakturPajakDate)
						on ar.CompanyCode = SRC.NewCompany
					   and ar.BranchCode  = SRC.NewBranch
					   and ar.DocNo       = SRC.NewDocNo
					  when matched 
						   then update set NettAmt = NewNettAmt
					  when not matched by target 
						   then insert (CompanyCode, BranchCode, DocNo, DocDate, ProfitCenterCode, NettAmt, ReceiveAmt, 
										 CustomerCode, TOPCode, DueDate, TypeTrans, BlockAmt, DebetAmt, CreditAmt, SalesCode, 
										 LeasingCode, StatusFlag, CreateBy, CreateDate, AccountNo, FakturPajakNo, FakturPajakDate)
								values (NewCompany, NewBranch, NewDocNo, NewDocDate, NewProfitCenterCode, NewNettAmt, 
										NewReceiveAmt, NewCustomerCode, NewTOPCode, NewDueDate, NewTypeTrans, NewBlockAmt, 
										NewDebetAmt, NewCreditAmt, NewSalesCode, NewLeasingCode, NewStatusFlag, NewCreateBy, 
										NewCreateDate, NewAccountNo, NewFakturPajakNo, NewFakturPajakDate);'
					execute sp_executesql @sqlString

			 -- MD: Insert/Update data to table glInterface
				-- glInterface - AR
				set @SeqNoGLMD = 1
				--set @sqlString = 'merge into ' +@DBNameMD+ '..glInterface as gl using ( values(''' 
				--				+@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' +@FPJNo+ 
				--						       ''',''' +convert(varchar,@SeqNoGlMD)+ 
				--							   ''',''' +convert(varchar,@curDocDate,121)+ 
				--							   ''',''300'',''' +convert(varchar,@curDocDate,121)+ 
				--							   ''',''' +@AccNoArMD+ ''',''SPAREPART'',''INVOICE'',''' 
				--							   +@FPJNo+ ''',''' +convert(varchar,@TotSalesAmt)+ 
				--							   ''',0,''AR'',NULL,NULL,0,''POSTING'',''' 
				--							   +convert(varchar,@PostingDate,111)+ 
				--							   ''',''POSTING'',''' +@currentDate+ '''))
				--		as SRC (NewCompany, NewBranch, NewDocNo, NewSeqNo, NewDocDate, NewProfitCenterCode, 
				--				NewAccDate, NewAccountNo, NewJournalCode, NewTypeJournal, NewApplyTo, 
				--				NewAmountDb, NewAmountCr, NewTypeTrans, NewBatchNo, NewBatchDate, 
				--				NewStatusFlag, NewCreateBy, NewCreateDate, NewLastUpdateBy, NewLastUpdateDate)
				--		on gl.CompanyCode      = SRC.NewCompany
				--	   and gl.BranchCode	   = SRC.NewBranch
				--	   and gl.DocNo			   = SRC.NewDocNo
				--	   and gl.ProfitCenterCode = SRC.NewProfitCenterCode
				--	   and gl.TypeTrans        = ''AR''
				--	  when matched 
				--		   then update set AmountDb = AmountDb + convert(numeric(18,2),NewAmountDb)
				--	  when not matched by target 
				--		   then insert (CompanyCode, BranchCode, DocNo, SeqNo, DocDate, ProfitCenterCode, AccDate, AccountNo,
				--						 JournalCode, TypeJournal, ApplyTo, AmountDb, AmountCr, TypeTrans, BatchNo, BatchDate,
				--						 StatusFlag, CreateBy, CreateDate, LastUpdateBy, LastUpdateDate)
				--				values (NewCompany, NewBranch, NewDocNo, NewSeqNo, NewDocDate, NewProfitCenterCode, NewAccDate, 
				--				        NewAccountNo, NewJournalCode, NewTypeJournal, NewApplyTo, NewAmountDb, NewAmountCr, 
				--						NewTypeTrans, NewBatchNo, NewBatchDate, NewStatusFlag, NewCreateBy, NewCreateDate, 
				--						NewLastUpdateBy, NewLastUpdateDate);'
				set @sqlString = 'merge into ' +@DBNameMD+ '..glInterface as gl using ( values(''' 
								+@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' +@FPJNo+ 
										       ''',''' +convert(varchar,@SeqNoGlMD)+ 
											   ''',''' +convert(varchar,@curDocDate,121)+ 
											   ''',''300'',''' +convert(varchar,@curDocDate,121)+ 
											   ''',''' +@AccNoArMD+ ''',''SPAREPART'',''INVOICE'',''' 
											   +@FPJNo+ ''',''' +convert(varchar,@TotalSalesAmt)+ 
											   ''',0,''AR'',NULL,NULL,0,''POSTING'',''' 
											   +convert(varchar,@PostingDate,111)+ 
											   ''',''POSTING'',''' +@currentDate+ '''))
						as SRC (NewCompany, NewBranch, NewDocNo, NewSeqNo, NewDocDate, NewProfitCenterCode, 
								NewAccDate, NewAccountNo, NewJournalCode, NewTypeJournal, NewApplyTo, 
								NewAmountDb, NewAmountCr, NewTypeTrans, NewBatchNo, NewBatchDate, 
								NewStatusFlag, NewCreateBy, NewCreateDate, NewLastUpdateBy, NewLastUpdateDate)
						on gl.CompanyCode      = SRC.NewCompany
					   and gl.BranchCode	   = SRC.NewBranch
					   and gl.DocNo			   = SRC.NewDocNo
					   and gl.ProfitCenterCode = SRC.NewProfitCenterCode
					   and gl.TypeTrans        = ''AR''
					  when matched 
						   then update set AmountDb = convert(numeric(18,2),NewAmountDb)
					  when not matched by target 
						   then insert (CompanyCode, BranchCode, DocNo, SeqNo, DocDate, ProfitCenterCode, AccDate, AccountNo,
										 JournalCode, TypeJournal, ApplyTo, AmountDb, AmountCr, TypeTrans, BatchNo, BatchDate,
										 StatusFlag, CreateBy, CreateDate, LastUpdateBy, LastUpdateDate)
								values (NewCompany, NewBranch, NewDocNo, NewSeqNo, NewDocDate, NewProfitCenterCode, NewAccDate, 
								        NewAccountNo, NewJournalCode, NewTypeJournal, NewApplyTo, NewAmountDb, NewAmountCr, 
										NewTypeTrans, NewBatchNo, NewBatchDate, NewStatusFlag, NewCreateBy, NewCreateDate, 
										NewLastUpdateBy, NewLastUpdateDate);'
					execute sp_executesql @sqlString

				-- glInterface - SALES
				set @SeqNoGLMD = @SeqNoGlMD + 1
				set @sqlString = 'merge into ' +@DBNameMD+ '..glInterface as gl using ( values(''' 
								+@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' +@FPJNo+ 
										       ''',''' +convert(varchar,@SeqNoGlMD)+ 
											   ''',''' +convert(varchar,@curDocDate,121)+ 
											   ''',''300'',''' +convert(varchar,@curDocDate,121)+ 
											   ''',''' +@AccNoSalesMD+ ''',''SPAREPART'',''INVOICE'',''' 
											   +@FPJNo+ ''',0,''' +convert(varchar,@SalesAmt)+ 
											   ''',''SALES'',NULL,NULL,0,''POSTING'',''' 
											   +convert(varchar,@PostingDate,111)+ 
											   ''',''POSTING'',''' +@currentDate+ '''))
						as SRC (NewCompany, NewBranch, NewDocNo, NewSeqNo, NewDocDate, NewProfitCenterCode, 
								NewAccDate, NewAccountNo, NewJournalCode, NewTypeJournal, NewApplyTo, 
								NewAmountDb, NewAmountCr, NewTypeTrans, NewBatchNo, NewBatchDate, 
								NewStatusFlag, NewCreateBy, NewCreateDate, NewLastUpdateBy, NewLastUpdateDate)
						on gl.CompanyCode      = SRC.NewCompany
					   and gl.BranchCode	   = SRC.NewBranch
					   and gl.DocNo			   = SRC.NewDocNo
					   and gl.ProfitCenterCode = SRC.NewProfitCenterCode
					   and gl.TypeTrans        = ''SALES''
					  when matched 
						   then update set AmountCr = AmountCr + convert(numeric(18,2),NewAmountCr)
					  when not matched by target 
						   then insert (CompanyCode, BranchCode, DocNo, SeqNo, DocDate, ProfitCenterCode, AccDate, AccountNo,
										 JournalCode, TypeJournal, ApplyTo, AmountDb, AmountCr, TypeTrans, BatchNo, BatchDate,
										 StatusFlag, CreateBy, CreateDate, LastUpdateBy, LastUpdateDate)
								values (NewCompany, NewBranch, NewDocNo, NewSeqNo, NewDocDate, NewProfitCenterCode, NewAccDate, 
								        NewAccountNo, NewJournalCode, NewTypeJournal, NewApplyTo, NewAmountDb, NewAmountCr, 
										NewTypeTrans, NewBatchNo, NewBatchDate, NewStatusFlag, NewCreateBy, NewCreateDate, 
										NewLastUpdateBy, NewLastUpdateDate);'
					execute sp_executesql @sqlString

				-- glInterface - TAX OUT
				set @SeqNoGLMD = @SeqNoGlMD + 1
				--set @sqlString = 'merge into ' +@DBNameMD+ '..glInterface as gl using ( values(''' 
				--				+@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' +@FPJNo+ 
				--						       ''',''' +convert(varchar,@SeqNoGlMD)+ 
				--							   ''',''' +convert(varchar,@curDocDate,121)+ 
				--							   ''',''300'',''' +convert(varchar,@curDocDate,121)+ 
				--							   ''',''' +@AccNoTaxOutMD+ ''',''SPAREPART'',''INVOICE'',''' 
				--							   +@FPJNo+ ''',0,''' +convert(varchar,@PPNAmt)+ 
				--							   ''',''TAX OUT'',NULL,NULL,0,''POSTING'',''' 
				--							   +convert(varchar,@PostingDate,111)+ 
				--							   ''',''POSTING'',''' +@currentDate+ '''))
				--		as SRC (NewCompany, NewBranch, NewDocNo, NewSeqNo, NewDocDate, NewProfitCenterCode, 
				--				NewAccDate, NewAccountNo, NewJournalCode, NewTypeJournal, NewApplyTo, 
				--				NewAmountDb, NewAmountCr, NewTypeTrans, NewBatchNo, NewBatchDate, 
				--				NewStatusFlag, NewCreateBy, NewCreateDate, NewLastUpdateBy, NewLastUpdateDate)
				--		on gl.CompanyCode      = SRC.NewCompany
				--	   and gl.BranchCode	   = SRC.NewBranch
				--	   and gl.DocNo			   = SRC.NewDocNo
				--	   and gl.ProfitCenterCode = SRC.NewProfitCenterCode
				--	   and gl.TypeTrans        = ''TAX OUT''
				--	  when matched 
				--		   then update set AmountCr = AmountCr + convert(numeric(18,2),NewAmountCr)
				--	  when not matched by target 
				--		   then insert (CompanyCode, BranchCode, DocNo, SeqNo, DocDate, ProfitCenterCode, AccDate, AccountNo,
				--						 JournalCode, TypeJournal, ApplyTo, AmountDb, AmountCr, TypeTrans, BatchNo, BatchDate,
				--						 StatusFlag, CreateBy, CreateDate, LastUpdateBy, LastUpdateDate)
				--				values (NewCompany, NewBranch, NewDocNo, NewSeqNo, NewDocDate, NewProfitCenterCode, NewAccDate, 
				--				        NewAccountNo, NewJournalCode, NewTypeJournal, NewApplyTo, NewAmountDb, NewAmountCr, 
				--						NewTypeTrans, NewBatchNo, NewBatchDate, NewStatusFlag, NewCreateBy, NewCreateDate, 
				--						NewLastUpdateBy, NewLastUpdateDate);'
				set @sqlString = 'merge into ' +@DBNameMD+ '..glInterface as gl using ( values(''' 
								+@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' +@FPJNo+ 
										       ''',''' +convert(varchar,@SeqNoGlMD)+ 
											   ''',''' +convert(varchar,@curDocDate,121)+ 
											   ''',''300'',''' +convert(varchar,@curDocDate,121)+ 
											   ''',''' +@AccNoTaxOutMD+ ''',''SPAREPART'',''INVOICE'',''' 
											   +@FPJNo+ ''',0,''' +convert(varchar,@TotalPPNAmt)+ 
											   ''',''TAX OUT'',NULL,NULL,0,''POSTING'',''' 
											   +convert(varchar,@PostingDate,111)+ 
											   ''',''POSTING'',''' +@currentDate+ '''))
						as SRC (NewCompany, NewBranch, NewDocNo, NewSeqNo, NewDocDate, NewProfitCenterCode, 
								NewAccDate, NewAccountNo, NewJournalCode, NewTypeJournal, NewApplyTo, 
								NewAmountDb, NewAmountCr, NewTypeTrans, NewBatchNo, NewBatchDate, 
								NewStatusFlag, NewCreateBy, NewCreateDate, NewLastUpdateBy, NewLastUpdateDate)
						on gl.CompanyCode      = SRC.NewCompany
					   and gl.BranchCode	   = SRC.NewBranch
					   and gl.DocNo			   = SRC.NewDocNo
					   and gl.ProfitCenterCode = SRC.NewProfitCenterCode
					   and gl.TypeTrans        = ''TAX OUT''
					  when matched 
						   then update set AmountCr = convert(numeric(18,2),NewAmountCr)
					  when not matched by target 
						   then insert (CompanyCode, BranchCode, DocNo, SeqNo, DocDate, ProfitCenterCode, AccDate, AccountNo,
										 JournalCode, TypeJournal, ApplyTo, AmountDb, AmountCr, TypeTrans, BatchNo, BatchDate,
										 StatusFlag, CreateBy, CreateDate, LastUpdateBy, LastUpdateDate)
								values (NewCompany, NewBranch, NewDocNo, NewSeqNo, NewDocDate, NewProfitCenterCode, NewAccDate, 
								        NewAccountNo, NewJournalCode, NewTypeJournal, NewApplyTo, NewAmountDb, NewAmountCr, 
										NewTypeTrans, NewBatchNo, NewBatchDate, NewStatusFlag, NewCreateBy, NewCreateDate, 
										NewLastUpdateBy, NewLastUpdateDate);'
					execute sp_executesql @sqlString

				-- glInterface - DISC1
				if isnull(@DiscFlag,0) = 1
				begin
				set @SeqNoGLMD = @SeqNoGLMD + 1
				set @sqlString = 'merge into ' +@DBNameMD+ '..glInterface as gl using ( values(''' 
								+@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' +@FPJNo+ 
										       ''',''' +convert(varchar,@SeqNoGlMD)+ 
											   ''',''' +convert(varchar,@curDocDate,121)+ 
											   ''',''300'',''' +convert(varchar,@curDocDate,121)+ 
											   ''',''' +@AccNoDisc1MD+ ''',''SPAREPART'',''INVOICE'',''' 
											   +@FPJNo+ ''',''' +convert(varchar,@DiscAmt)+ 
											   ''',0,''DISC1'',NULL,NULL,0,''POSTING'',''' 
											   +convert(varchar,@PostingDate,111)+ 
											   ''',''POSTING'',''' +@currentDate+ '''))
						as SRC (NewCompany, NewBranch, NewDocNo, NewSeqNo, NewDocDate, NewProfitCenterCode, 
								NewAccDate, NewAccountNo, NewJournalCode, NewTypeJournal, NewApplyTo, 
								NewAmountDb, NewAmountCr, NewTypeTrans, NewBatchNo, NewBatchDate, 
								NewStatusFlag, NewCreateBy, NewCreateDate, NewLastUpdateBy, NewLastUpdateDate)
						on gl.CompanyCode      = SRC.NewCompany
					   and gl.BranchCode	   = SRC.NewBranch
					   and gl.DocNo			   = SRC.NewDocNo
					   and gl.ProfitCenterCode = SRC.NewProfitCenterCode
					   and gl.TypeTrans        = ''DISC1''
					  when matched 
						   then update set AmountDb = AmountDb + convert(numeric(18,2),NewAmountDb)
					  when not matched by target 
						   then insert (CompanyCode, BranchCode, DocNo, SeqNo, DocDate, ProfitCenterCode, AccDate, AccountNo,
										 JournalCode, TypeJournal, ApplyTo, AmountDb, AmountCr, TypeTrans, BatchNo, BatchDate,
										 StatusFlag, CreateBy, CreateDate, LastUpdateBy, LastUpdateDate)
								values (NewCompany, NewBranch, NewDocNo, NewSeqNo, NewDocDate, NewProfitCenterCode, NewAccDate, 
								        NewAccountNo, NewJournalCode, NewTypeJournal, NewApplyTo, NewAmountDb, NewAmountCr, 
										NewTypeTrans, NewBatchNo, NewBatchDate, NewStatusFlag, NewCreateBy, NewCreateDate, 
										NewLastUpdateBy, NewLastUpdateDate);'
					execute sp_executesql @sqlString
				end

				-- glInterface - COGS
				set @SeqNoGLMD = @SeqNoGLMD + 1
				set @sqlString = 'merge into ' +@DBNameMD+ '..glInterface as gl using ( values(''' 
								+@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' +@FPJNo+ 
										       ''',''' +convert(varchar,@SeqNoGlMD)+ 
											   ''',''' +convert(varchar,@curDocDate,121)+ 
											   ''',''300'',''' +convert(varchar,@curDocDate,121)+ 
											   ''',''' +@AccNoCogsMD+ ''',''SPAREPART'',''INVOICE'',''' 
											   +@FPJNo+ ''',''' +convert(varchar,@CostAmt)+ 
											   ''',0,''COGS'',NULL,NULL,0,''POSTING'',''' 
											   +convert(varchar,@PostingDate,111)+ 
											   ''',''POSTING'',''' +@currentDate+ '''))
						as SRC (NewCompany, NewBranch, NewDocNo, NewSeqNo, NewDocDate, NewProfitCenterCode, 
								NewAccDate, NewAccountNo, NewJournalCode, NewTypeJournal, NewApplyTo, 
								NewAmountDb, NewAmountCr, NewTypeTrans, NewBatchNo, NewBatchDate, 
								NewStatusFlag, NewCreateBy, NewCreateDate, NewLastUpdateBy, NewLastUpdateDate)
						on gl.CompanyCode      = SRC.NewCompany
					   and gl.BranchCode	   = SRC.NewBranch
					   and gl.DocNo			   = SRC.NewDocNo
					   and gl.ProfitCenterCode = SRC.NewProfitCenterCode
					   and gl.TypeTrans        = ''COGS''
					  when matched 
						   then update set AmountDb = AmountDb + convert(numeric(18,2),NewAmountDb)
					  when not matched by target 
						   then insert (CompanyCode, BranchCode, DocNo, SeqNo, DocDate, ProfitCenterCode, AccDate, AccountNo,
										 JournalCode, TypeJournal, ApplyTo, AmountDb, AmountCr, TypeTrans, BatchNo, BatchDate,
										 StatusFlag, CreateBy, CreateDate, LastUpdateBy, LastUpdateDate)
								values (NewCompany, NewBranch, NewDocNo, NewSeqNo, NewDocDate, NewProfitCenterCode, NewAccDate, 
								        NewAccountNo, NewJournalCode, NewTypeJournal, NewApplyTo, NewAmountDb, NewAmountCr, 
										NewTypeTrans, NewBatchNo, NewBatchDate, NewStatusFlag, NewCreateBy, NewCreateDate, 
										NewLastUpdateBy, NewLastUpdateDate);'
					execute sp_executesql @sqlString

				-- glInterface - INVENTORY
				set @SeqNoGLMD = @SeqNoGlMD + 1
				set @sqlString = 'merge into ' +@DBNameMD+ '..glInterface as gl using ( values(''' 
								+@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' +@FPJNo+ 
										       ''',''' +convert(varchar,@SeqNoGlMD)+ 
											   ''',''' +convert(varchar,@curDocDate,121)+ 
											   ''',''300'',''' +convert(varchar,@curDocDate,121)+ 
											   ''',''' +@AccNoInventoryMD+ ''',''SPAREPART'',''INVOICE'',''' 
											   +@FPJNo+ ''',0,''' +convert(varchar,@CostAmt)+ 
											   ''',''INVENTORY'',NULL,NULL,0,''POSTING'',''' 
											   +convert(varchar,@PostingDate,111)+ 
											   ''',''POSTING'',''' +@currentDate+ '''))
						as SRC (NewCompany, NewBranch, NewDocNo, NewSeqNo, NewDocDate, NewProfitCenterCode, 
								NewAccDate, NewAccountNo, NewJournalCode, NewTypeJournal, NewApplyTo, 
								NewAmountDb, NewAmountCr, NewTypeTrans, NewBatchNo, NewBatchDate, 
								NewStatusFlag, NewCreateBy, NewCreateDate, NewLastUpdateBy, NewLastUpdateDate)
						on gl.CompanyCode      = SRC.NewCompany
					   and gl.BranchCode	   = SRC.NewBranch
					   and gl.DocNo			   = SRC.NewDocNo
					   and gl.ProfitCenterCode = SRC.NewProfitCenterCode
					   and gl.TypeTrans        = ''INVENTORY''
					  when matched 
						   then update set AmountCr = AmountCr + convert(numeric(18,2),NewAmountCr)
					  when not matched by target 
						   then insert (CompanyCode, BranchCode, DocNo, SeqNo, DocDate, ProfitCenterCode, AccDate, AccountNo,
										 JournalCode, TypeJournal, ApplyTo, AmountDb, AmountCr, TypeTrans, BatchNo, BatchDate,
										 StatusFlag, CreateBy, CreateDate, LastUpdateBy, LastUpdateDate)
								values (NewCompany, NewBranch, NewDocNo, NewSeqNo, NewDocDate, NewProfitCenterCode, NewAccDate, 
								        NewAccountNo, NewJournalCode, NewTypeJournal, NewApplyTo, NewAmountDb, NewAmountCr, 
										NewTypeTrans, NewBatchNo, NewBatchDate, NewStatusFlag, NewCreateBy, NewCreateDate, 
										NewLastUpdateBy, NewLastUpdateDate);'
					execute sp_executesql @sqlString

			 -- SD: Insert data to table spTrnPBinnDtl 
				set @sqlString = 'insert into ' +@DBName+ '..spTrnPBinnDtl
										(CompanyCode, BranchCode, BinningNo, PartNo, DocNo, DocDate, 
										 WarehouseCode, LocationCode, BoxNo, ReceivedQty, PurchasePrice, 
										 CostPrice, DiscPct, ABCCLass, MovingCode, ProductType, PartCategory,
										 CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
								   values(''' +@curCompanyCode+ ''',''' +@curBranchCode+ ''',''' +@BINNo+ ''',''' 
											  +@curPartNo+ ''',''' +@POSNo+ ''',''' +convert(varchar,@curDocDate,121)+
										  ''',''00'',''' +@LocationCode+ ''',''00'',' +convert(varchar,@curQty)+
										  ',' +convert(varchar,@curRetailPriceMD)+ ',' +convert(varchar,@RetailPriceNet)+
										  ',' +convert(varchar,@DiscPct)+ ',''' +@ABCClass+ ''',''' +@MovingCode+ 
										  ''',''' +left(@curProductType,3)+ ''',''' +@PartCategory+ 
										  ''',''' +@curCreatedBy+ ''',''' +convert(varchar,@curCreatedDate,121)+ 
										  ''',''' +@curLastUpdateBy+ ''',''' +convert(varchar,@curLastUpdateDate,121)+ ''')'
					execute sp_executesql @sqlString  

			 -- SD: Update data to table spTrnPBinnHdr
				set @sqlString = 'update ' +@DBName+ '..spTrnPBinnHdr
								     set TotItem = ' +convert(varchar,@SeqNo)+ ', TotBinningAmt = TotBinningAmt + ' +convert(varchar,@curQty*@RetailPriceNet)+ 
								 ' where CompanyCode=''' +@curCompanyCode+ ''' and BranchCode=''' +@curBranchCode+ ''' and BinningNo=''' +@BINNo + ''''
					execute sp_executesql @sqlString 

			 -- SD: Insert data to table spTrnPRcvDtl 
				set @sqlString = 'insert into ' +@DBName+ '..spTrnPRcvDtl
										(CompanyCode, BranchCode, WRSNo, PartNo, DocNo, DocDate, 
										 WarehouseCode, LocationCode, BoxNo, ReceivedQty, PurchasePrice, 
										 CostPrice, DiscPct, ABCCLass, MovingCode, ProductType, PartCategory,
										 CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
								   values(''' +@curCompanyCode+ ''',''' +@curBranchCode+ ''',''' +@WRSNo+ ''',''' 
											  +@curPartNo+ ''',''' +@POSNo+ ''',''' +convert(varchar,@curDocDate,121)+
										  ''',''00'',''' +@LocationCode+ ''',''00'',' +convert(varchar,@curQty)+
										  ',' +convert(varchar,@curRetailPriceMD)+ ',' +convert(varchar,@RetailPriceNet)+
										  ',' +convert(varchar,@DiscPct)+ ',''' +@ABCClass+ ''',''' +@MovingCode+ 
										  ''',''' +left(@curProductType,3)+ ''',''' +@PartCategory+ 
										  ''',''' +@curCreatedBy+ ''',''' +convert(varchar,@curCreatedDate,121)+ 
										  ''',''' +@curLastUpdateBy+ ''',''' +convert(varchar,@curLastUpdateDate,121)+ ''')'
					execute sp_executesql @sqlString  

			 -- SD: Update data to table spTrnPRcvHdr
				set @sqlString = 'update ' +@DBName+ '..spTrnPRcvHdr
								     set TotItem = ' +convert(varchar,@SeqNo)+ ', TotWRSAmt = TotWRSAmt + ' +convert(varchar,@TotPurchaseNetAmt)+ 
								 ' where CompanyCode=''' +@curCompanyCode+ ''' and BranchCode=''' +@curBranchCode+ ''' and BinningNo=''' +@BINNo + ''''
					execute sp_executesql @sqlString 

			 -- SD: Update data to table spTrnPHPP
				--set @sqlString = 'update ' +@DBName+ '..spTrnPHPP
				--				     set TotPurchAmt     = TotPurchAmt     + ' +convert(varchar,@TotPurchaseAmt)+ 
				--					  ', TotNetPurchAmt  = TotNetPurchAmt  + ' +convert(varchar,@TotPurchaseNetAmt)+ 
				--					  ', TotTaxAmt       = TotTaxAmt       + ' +convert(varchar,@TotTaxAmt)+ 
				--					  ', DiffNetPurchAmt = DiffNetPurchAmt   ' +
				--					  ', DiffTaxAmt      = DiffTaxAmt        ' +
				--					  ', TotHPPAmt       = TotHPPAmt       + ' +convert(varchar,@TotPurchaseAmt)+ 
				--				 ' where CompanyCode=''' +@curCompanyCode+ ''' and BranchCode=''' +@curBranchCode+ ''' and HPPNo=''' +@HPPNo + ''''
				set @sqlString = 'update ' +@DBName+ '..spTrnPHPP
								     set TotPurchAmt     = TotPurchAmt     + ' +convert(varchar,@TotPurchaseAmt)+ 
									  ', TotNetPurchAmt  = TotNetPurchAmt  + ' +convert(varchar,@TotPurchaseNetAmt)+ 
									  ', TotTaxAmt       = ' +convert(varchar,@TotalPPNAmt)+ 
									  ', DiffNetPurchAmt = DiffNetPurchAmt   ' +
									  ', DiffTaxAmt      = DiffTaxAmt        ' +
									  ', TotHPPAmt       = ' +convert(varchar,@TotalSalesAmt)+ 
								 ' where CompanyCode=''' +@curCompanyCode+ ''' and BranchCode=''' +@curBranchCode+ ''' and HPPNo=''' +@HPPNo + ''''
					execute sp_executesql @sqlString 


			 -- SD: Insert/Update data to table apInterface
				--set @sqlString = 'merge into ' +@DBName+ '..apInterface as ap using ( values(''' 
				--				 +@curCompanyCode+ ''',''' +@curBranchCode+ ''',''' +@HPPNo+ ''',''' 
				--				 +@curProfitCenterCode+ ''',''' +convert(varchar,@curDocDate,121)+ ''',''' 
				--				 +@WRSNo+ ''',''' +convert(varchar,@curDocDate,121)+ ''',''' 
				--				 +convert(varchar,@TotPurchaseNetAmt)+ ''',0,''' +@curBranchMD+ ''',''' 
				--				 +convert(varchar,@TotTaxAmt)+ ''',0,''' +@AccNoAP+ ''',''' 
				--				 +convert(varchar,@DueDateMD,111)+ ''','' '',''' 
				--				 +convert(varchar,@TotPurchaseAmt)+ ''',0,''POSTING'','''
				--				 +convert(varchar,@PostingDate,111)+ ''',0,''' +@SeriPajakNo+ ''','''
				--				 +convert(varchar,@curDocDate,121)+ ''',''' +@FPJNo+ '''))
				--		as SRC (NewCompany, NewBranch, NewDocNo, NewProfitCenterCode, NewDocDate, NewReference, 
				--				NewReferenceDate, NewNetAmt, NewPPHAmt, NewSupplierCode, NewPPNAmt, NewPPnBM, 
				--				NewAccountNo, NewTermsDate, NewTermsName, NewTotalAmt, NewStatusFlag, NewCreateBy, 
				--				NewCreateDate, NewReceiveAmt, NewFakturPajakNo, NewFakturPajakDate, NewRefNo)
				--		on ap.CompanyCode = SRC.NewCompany
				--	   and ap.BranchCode  = SRC.NewBranch
				--	   and ap.DocNo       = SRC.NewDocNo
				--	  when matched 
				--		   then update set NetAmt   = NetAmt   + NewNetAmt
				--						 , PPNAmt   = PPNAmt   + NewPPNAmt
				--						 , TotalAmt = TotalAmt + NewTotalAmt
				--	  when not matched by target 
				--		   then insert (CompanyCode, BranchCode, DocNo, ProfitCenterCode, DocDate, Reference, 
				--						ReferenceDate, NetAmt, PPHAmt, SupplierCode, PPNAmt, PPnBM, AccountNo, 
				--						TermsDate, TermsName, TotalAmt, StatusFlag, CreateBy, CreateDate, 
				--						ReceiveAmt, FakturPajakNo, FakturPajakDate, RefNo)
				--				values (NewCompany, NewBranch, NewDocNo, NewProfitCenterCode, NewDocDate, NewReference, 
				--						NewReferenceDate, NewNetAmt, NewPPHAmt, NewSupplierCode, NewPPNAmt, NewPPnBM, 
				--						NewAccountNo, NewTermsDate, NewTermsName, NewTotalAmt, NewStatusFlag, NewCreateBy, 
				--						NewCreateDate, NewReceiveAmt, NewFakturPajakNo, NewFakturPajakDate, NewRefNo);'
				set @sqlString = 'merge into ' +@DBName+ '..apInterface as ap using ( values(''' 
								 +@curCompanyCode+ ''',''' +@curBranchCode+ ''',''' +@HPPNo+ ''',''' 
								 +@curProfitCenterCode+ ''',''' +convert(varchar,@curDocDate,121)+ ''',''' 
								 +@WRSNo+ ''',''' +convert(varchar,@curDocDate,121)+ ''',''' 
								 +convert(varchar,@TotPurchaseNetAmt)+ ''',0,''' +@curBranchMD+ ''',''' 
								 +convert(varchar,@TotalPPNAmt)+ ''',0,''' +@AccNoAP+ ''',''' 
								 +convert(varchar,@DueDateMD,111)+ ''','' '',''' 
								 +convert(varchar,@TotalSalesAmt)+ ''',0,''POSTING'','''
								 +convert(varchar,@PostingDate,111)+ ''',0,''' +@SeriPajakNo+ ''','''
								 +convert(varchar,@curDocDate,121)+ ''',''' +@FPJNo+ '''))
						as SRC (NewCompany, NewBranch, NewDocNo, NewProfitCenterCode, NewDocDate, NewReference, 
								NewReferenceDate, NewNetAmt, NewPPHAmt, NewSupplierCode, NewPPNAmt, NewPPnBM, 
								NewAccountNo, NewTermsDate, NewTermsName, NewTotalAmt, NewStatusFlag, NewCreateBy, 
								NewCreateDate, NewReceiveAmt, NewFakturPajakNo, NewFakturPajakDate, NewRefNo)
						on ap.CompanyCode = SRC.NewCompany
					   and ap.BranchCode  = SRC.NewBranch
					   and ap.DocNo       = SRC.NewDocNo
					  when matched 
						   then update set NetAmt   = NetAmt   + NewNetAmt
										 , PPNAmt   = NewPPNAmt
										 , TotalAmt = NewTotalAmt
					  when not matched by target 
						   then insert (CompanyCode, BranchCode, DocNo, ProfitCenterCode, DocDate, Reference, 
										ReferenceDate, NetAmt, PPHAmt, SupplierCode, PPNAmt, PPnBM, AccountNo, 
										TermsDate, TermsName, TotalAmt, StatusFlag, CreateBy, CreateDate, 
										ReceiveAmt, FakturPajakNo, FakturPajakDate, RefNo)
								values (NewCompany, NewBranch, NewDocNo, NewProfitCenterCode, NewDocDate, NewReference, 
										NewReferenceDate, NewNetAmt, NewPPHAmt, NewSupplierCode, NewPPNAmt, NewPPnBM, 
										NewAccountNo, NewTermsDate, NewTermsName, NewTotalAmt, NewStatusFlag, NewCreateBy, 
										NewCreateDate, NewReceiveAmt, NewFakturPajakNo, NewFakturPajakDate, NewRefNo);'
					execute sp_executesql @sqlString

			 -- SD: Insert/Update data to table glInterface
				-- glInterface - INVENTORY
				set @SeqNoGL = 1
				set @sqlString = 'merge into ' +@DBName+ '..glInterface as gl using ( values(''' 
								 +@curCompanyCode+ ''',''' +@curBranchCode+ ''',''' +@HPPNo+ ''',''' 
								 +convert(varchar,@SeqNoGl)+ ''',''' +convert(varchar,@curDocDate,121)+ 
								 ''',''300'',''' +convert(varchar,@curDocDate,121)+ ''',''' +@AccNoInventory+
								 ''',''SPAREPART'',''PURCHASE'',''' +@HPPNo+ ''',''' 
								 +convert(varchar,@TotPurchaseNetAmt)+ ''',0,''INVENTORY'',''' +@APNo+ 
								 ''',NULL,0,''POSTING'',''' +convert(varchar,@PostingDate,111)+ 
								 ''',''POSTING'',''' +@currentDate+ '''))
						as SRC (NewCompany, NewBranch, NewDocNo, NewSeqNo, NewDocDate, NewProfitCenterCode, 
								NewAccDate, NewAccountNo, NewJournalCode, NewTypeJournal, NewApplyTo, 
								NewAmountDb, NewAmountCr, NewTypeTrans, NewBatchNo, NewBatchDate, 
								NewStatusFlag, NewCreateBy, NewCreateDate, NewLastUpdateBy, NewLastUpdateDate)
						on gl.CompanyCode      = SRC.NewCompany
					   and gl.BranchCode	   = SRC.NewBranch
					   and gl.DocNo			   = SRC.NewDocNo
					   and gl.ProfitCenterCode = SRC.NewProfitCenterCode
					   and gl.TypeTrans        = ''INVENTORY''
					  when matched 
						   then update set AmountDb = AmountDb + convert(numeric(18,2),NewAmountDb)
					  when not matched by target 
						   then insert (CompanyCode, BranchCode, DocNo, SeqNo, DocDate, ProfitCenterCode, AccDate, AccountNo,
										 JournalCode, TypeJournal, ApplyTo, AmountDb, AmountCr, TypeTrans, BatchNo, BatchDate,
										 StatusFlag, CreateBy, CreateDate, LastUpdateBy, LastUpdateDate)
								values (NewCompany, NewBranch, NewDocNo, NewSeqNo, NewDocDate, NewProfitCenterCode, NewAccDate, 
								        NewAccountNo, NewJournalCode, NewTypeJournal, NewApplyTo, NewAmountDb, NewAmountCr, 
										NewTypeTrans, NewBatchNo, NewBatchDate, NewStatusFlag, NewCreateBy, NewCreateDate, 
										NewLastUpdateBy, NewLastUpdateDate);'
					execute sp_executesql @sqlString

				-- glInterface - TAX IN
				set @SeqNoGL = @SeqNoGl + 1
				--set @sqlString = 'merge into ' +@DBName+ '..glInterface as gl using ( values(''' 
				--				 +@curCompanyCode+ ''',''' +@curBranchCode+ ''',''' +@HPPNo+ ''',''' 
				--				 +convert(varchar,@SeqNoGl)+ ''',''' +convert(varchar,@curDocDate,121)+ 
				--				 ''',''300'',''' +convert(varchar,@curDocDate,121)+ ''',''' +@AccNoTaxIn+
				--				 ''',''SPAREPART'',''PURCHASE'',''' +@HPPNo+ ''',''' 
				--				 +convert(varchar,@TotTaxAmt)+ ''',0,''TAX IN'',''' +@APNo+ 
				--				 ''',NULL,0,''POSTING'',''' +convert(varchar,@PostingDate,111)+ 
				--				 ''',''POSTING'',''' +@currentDate+ '''))
				--		as SRC (NewCompany, NewBranch, NewDocNo, NewSeqNo, NewDocDate, NewProfitCenterCode, 
				--				NewAccDate, NewAccountNo, NewJournalCode, NewTypeJournal, NewApplyTo, 
				--				NewAmountDb, NewAmountCr, NewTypeTrans, NewBatchNo, NewBatchDate, 
				--				NewStatusFlag, NewCreateBy, NewCreateDate, NewLastUpdateBy, NewLastUpdateDate)
				--		on gl.CompanyCode      = SRC.NewCompany
				--	   and gl.BranchCode	   = SRC.NewBranch
				--	   and gl.DocNo			   = SRC.NewDocNo
				--	   and gl.ProfitCenterCode = SRC.NewProfitCenterCode
				--	   and gl.TypeTrans        = ''TAX IN''
				--	  when matched 
				--		   then update set AmountDb = AmountDb + convert(numeric(18,2),NewAmountDb)
				--	  when not matched by target 
				--		   then insert (CompanyCode, BranchCode, DocNo, SeqNo, DocDate, ProfitCenterCode, AccDate, AccountNo,
				--						 JournalCode, TypeJournal, ApplyTo, AmountDb, AmountCr, TypeTrans, BatchNo, BatchDate,
				--						 StatusFlag, CreateBy, CreateDate, LastUpdateBy, LastUpdateDate)
				--				values (NewCompany, NewBranch, NewDocNo, NewSeqNo, NewDocDate, NewProfitCenterCode, NewAccDate, 
				--				        NewAccountNo, NewJournalCode, NewTypeJournal, NewApplyTo, NewAmountDb, NewAmountCr, 
				--						NewTypeTrans, NewBatchNo, NewBatchDate, NewStatusFlag, NewCreateBy, NewCreateDate, 
				--						NewLastUpdateBy, NewLastUpdateDate);'
				set @sqlString = 'merge into ' +@DBName+ '..glInterface as gl using ( values(''' 
								 +@curCompanyCode+ ''',''' +@curBranchCode+ ''',''' +@HPPNo+ ''',''' 
								 +convert(varchar,@SeqNoGl)+ ''',''' +convert(varchar,@curDocDate,121)+ 
								 ''',''300'',''' +convert(varchar,@curDocDate,121)+ ''',''' +@AccNoTaxIn+
								 ''',''SPAREPART'',''PURCHASE'',''' +@HPPNo+ ''',''' 
								 +convert(varchar,@TotalPPNAmt)+ ''',0,''TAX IN'',''' +@APNo+ 
								 ''',NULL,0,''POSTING'',''' +convert(varchar,@PostingDate,111)+ 
								 ''',''POSTING'',''' +@currentDate+ '''))
						as SRC (NewCompany, NewBranch, NewDocNo, NewSeqNo, NewDocDate, NewProfitCenterCode, 
								NewAccDate, NewAccountNo, NewJournalCode, NewTypeJournal, NewApplyTo, 
								NewAmountDb, NewAmountCr, NewTypeTrans, NewBatchNo, NewBatchDate, 
								NewStatusFlag, NewCreateBy, NewCreateDate, NewLastUpdateBy, NewLastUpdateDate)
						on gl.CompanyCode      = SRC.NewCompany
					   and gl.BranchCode	   = SRC.NewBranch
					   and gl.DocNo			   = SRC.NewDocNo
					   and gl.ProfitCenterCode = SRC.NewProfitCenterCode
					   and gl.TypeTrans        = ''TAX IN''
					  when matched 
						   then update set AmountDb = convert(numeric(18,2),NewAmountDb)
					  when not matched by target 
						   then insert (CompanyCode, BranchCode, DocNo, SeqNo, DocDate, ProfitCenterCode, AccDate, AccountNo,
										 JournalCode, TypeJournal, ApplyTo, AmountDb, AmountCr, TypeTrans, BatchNo, BatchDate,
										 StatusFlag, CreateBy, CreateDate, LastUpdateBy, LastUpdateDate)
								values (NewCompany, NewBranch, NewDocNo, NewSeqNo, NewDocDate, NewProfitCenterCode, NewAccDate, 
								        NewAccountNo, NewJournalCode, NewTypeJournal, NewApplyTo, NewAmountDb, NewAmountCr, 
										NewTypeTrans, NewBatchNo, NewBatchDate, NewStatusFlag, NewCreateBy, NewCreateDate, 
										NewLastUpdateBy, NewLastUpdateDate);'
					execute sp_executesql @sqlString

				-- glInterface - AP
				set @SeqNoGL = @SeqNoGl + 1
				--set @sqlString = 'merge into ' +@DBName+ '..glInterface as gl using ( values(''' 
				--				 +@curCompanyCode+ ''',''' +@curBranchCode+ ''',''' +@HPPNo+ ''',''' 
				--				 +convert(varchar,@SeqNoGl)+ ''',''' +convert(varchar,@curDocDate,121)+ 
				--				 ''',''300'',''' +convert(varchar,@curDocDate,121)+ ''',''' +@AccNoAP+
				--				 ''',''SPAREPART'',''PURCHASE'',''' +@HPPNo+ ''',0,'''
				--				 +convert(varchar,@TotPurchaseAmt)+ ''',''AP'',''' +@APNo+ 
				--				 ''',NULL,0,''POSTING'',''' +convert(varchar,@PostingDate,111)+ 
				--				 ''',''POSTING'',''' +@currentDate+ '''))
				--		as SRC (NewCompany, NewBranch, NewDocNo, NewSeqNo, NewDocDate, NewProfitCenterCode, 
				--				NewAccDate, NewAccountNo, NewJournalCode, NewTypeJournal, NewApplyTo, 
				--				NewAmountDb, NewAmountCr, NewTypeTrans, NewBatchNo, NewBatchDate, 
				--				NewStatusFlag, NewCreateBy, NewCreateDate, NewLastUpdateBy, NewLastUpdateDate)
				--		on gl.CompanyCode      = SRC.NewCompany
				--	   and gl.BranchCode	   = SRC.NewBranch
				--	   and gl.DocNo			   = SRC.NewDocNo
				--	   and gl.ProfitCenterCode = SRC.NewProfitCenterCode
				--	   and gl.TypeTrans        = ''AP''
				--	  when matched 
				--		   then update set AmountCr = AmountCr + convert(numeric(18,2),NewAmountCr)
				--	  when not matched by target 
				--		   then insert (CompanyCode, BranchCode, DocNo, SeqNo, DocDate, ProfitCenterCode, AccDate, AccountNo,
				--						 JournalCode, TypeJournal, ApplyTo, AmountDb, AmountCr, TypeTrans, BatchNo, BatchDate,
				--						 StatusFlag, CreateBy, CreateDate, LastUpdateBy, LastUpdateDate)
				--				values (NewCompany, NewBranch, NewDocNo, NewSeqNo, NewDocDate, NewProfitCenterCode, NewAccDate, 
				--				        NewAccountNo, NewJournalCode, NewTypeJournal, NewApplyTo, NewAmountDb, NewAmountCr, 
				--						NewTypeTrans, NewBatchNo, NewBatchDate, NewStatusFlag, NewCreateBy, NewCreateDate, 
				--						NewLastUpdateBy, NewLastUpdateDate);'
				set @sqlString = 'merge into ' +@DBName+ '..glInterface as gl using ( values(''' 
								 +@curCompanyCode+ ''',''' +@curBranchCode+ ''',''' +@HPPNo+ ''',''' 
								 +convert(varchar,@SeqNoGl)+ ''',''' +convert(varchar,@curDocDate,121)+ 
								 ''',''300'',''' +convert(varchar,@curDocDate,121)+ ''',''' +@AccNoAP+
								 ''',''SPAREPART'',''PURCHASE'',''' +@HPPNo+ ''',0,'''
								 +convert(varchar,@TotalSalesAmt)+ ''',''AP'',''' +@APNo+ 
								 ''',NULL,0,''POSTING'',''' +convert(varchar,@PostingDate,111)+ 
								 ''',''POSTING'',''' +@currentDate+ '''))
						as SRC (NewCompany, NewBranch, NewDocNo, NewSeqNo, NewDocDate, NewProfitCenterCode, 
								NewAccDate, NewAccountNo, NewJournalCode, NewTypeJournal, NewApplyTo, 
								NewAmountDb, NewAmountCr, NewTypeTrans, NewBatchNo, NewBatchDate, 
								NewStatusFlag, NewCreateBy, NewCreateDate, NewLastUpdateBy, NewLastUpdateDate)
						on gl.CompanyCode      = SRC.NewCompany
					   and gl.BranchCode	   = SRC.NewBranch
					   and gl.DocNo			   = SRC.NewDocNo
					   and gl.ProfitCenterCode = SRC.NewProfitCenterCode
					   and gl.TypeTrans        = ''AP''
					  when matched 
						   then update set AmountCr = convert(numeric(18,2),NewAmountCr)
					  when not matched by target 
						   then insert (CompanyCode, BranchCode, DocNo, SeqNo, DocDate, ProfitCenterCode, AccDate, AccountNo,
										 JournalCode, TypeJournal, ApplyTo, AmountDb, AmountCr, TypeTrans, BatchNo, BatchDate,
										 StatusFlag, CreateBy, CreateDate, LastUpdateBy, LastUpdateDate)
								values (NewCompany, NewBranch, NewDocNo, NewSeqNo, NewDocDate, NewProfitCenterCode, NewAccDate, 
								        NewAccountNo, NewJournalCode, NewTypeJournal, NewApplyTo, NewAmountDb, NewAmountCr, 
										NewTypeTrans, NewBatchNo, NewBatchDate, NewStatusFlag, NewCreateBy, NewCreateDate, 
										NewLastUpdateBy, NewLastUpdateDate);'
					execute sp_executesql @sqlString

			 -- SD: Insert data to table glJournal 
			 -- SD: Insert data to table glJournalDtl
				--select * from spTrnPRcvHdr where CompanyCode='6006400001' and BranchCode='6006400131' and WRSNo in ('WRL/14/102818','WRL/14/102819')
				--select * from spTrnPRcvDtl where CompanyCode='6006400001' and BranchCode='6006400131' and WRSNo in ('WRL/14/102818','WRL/14/102819')
				--select * from spTrnPHPP    where CompanyCode='6006400001' and BranchCode='6006400131' and WRSNo in ('WRL/14/102818','WRL/14/102819')
				--select * from apInterface  where CompanyCode='6006400001' and BranchCode='6006400131' and Reference in ('WRL/14/102818','WRL/14/102819') --DocNo='HPP/14/102793'
				--select * from glInterface  where CompanyCode='6006400001' and BranchCode='6006400131' and DocNo in ('HPP/14/102808','HPP/14/102815')
				--select * from glJournal    where CompanyCode='6006400001' and BranchCode='6006400131' and ProfitCenterCode='300' order by JournalNo desc --and ReffNo in ('HPP/14/102808','HPP/14/102815')
				--select * from glJournalDtl where CompanyCode='6006400001' and BranchCode='6006400131' and Description in ('HPP/14/102808','HPP/14/102815')
				--select * from glInterface  where CompanyCode='6006400001' and BranchCode='6006400131' and left(DocNo,3) in ('FPJ','HPP') order by BatchNo desc

			 -- Update Daily Posting Process Status
				update svSDMovement
				   set ProcessStatus=1
				     , ProcessDate  =@CurrentDate
					where CompanyCode=@curCompanyCode
					  and BranchCode =@curBranchCode
					  and DocNo      =@curDocNo
					  and PartNo     =@curPartNo
					  and PartSeq    =@curPartSeq

			 -- Read next record
				fetch next from cursvSDMovement
					into @curCompanyCode, @curBranchCode, @curDocNo, @curDocDate, @curPartNo, @curPartSeq, @curWarehouseCode, @curQtyOrder, 
						 @curQty, @curDiscPct, @curCostPrice, @curRetailPrice, @curTypeOfGoods, @curCompanyMD, @curBranchMD, @curWarehouseMD, 
						 @curRetailPriceInclTaxMD, @curRetailPriceMD, @curCostPriceMD, @curQtyFlag, @curProductType, @curProfitCenterCode, 
						 @curStatus, @curProcessStatus, @curProcessDate, @curCreatedBy, @curCreatedDate, @curLastUpdateBy, @curLastUpdateDate,
						 @curDocSort
			end 

	 -- Move data which already processed from table svSDMovement to table svHstSDMovement
	    if not exists (select * from sys.objects where object_id = object_id(N'[dbo].[svHstSDMovement]') and type in (N'U'))
			CREATE TABLE [dbo].[svHstSDMovement](
				[CompanyCode] [varchar](15) NOT NULL,
				[BranchCode] [varchar](15) NOT NULL,
				[DocNo] [varchar](15) NOT NULL,
				[DocDate] [datetime] NOT NULL,
				[PartNo] [varchar](20) NOT NULL,
				[PartSeq] [int] NOT NULL,
				[WarehouseCode] [varchar](15) NOT NULL,
				[QtyOrder] [numeric](18, 2) NOT NULL,
				[Qty] [numeric](18, 2) NOT NULL,
				[DiscPct] [numeric](5, 2) NOT NULL,
				[CostPrice] [numeric](18, 2) NOT NULL,
				[RetailPrice] [numeric](18, 2) NOT NULL,
				[TypeOfGoods] [varchar](5) NOT NULL,
				[CompanyMD] [varchar](15) NOT NULL,
				[BranchMD] [varchar](15) NOT NULL,
				[WarehouseMD] [varchar](15) NOT NULL,
				[RetailPriceInclTaxMD] [numeric](18, 2) NOT NULL,
				[RetailPriceMD] [numeric](18, 2) NOT NULL,
				[CostPriceMD] [numeric](18, 2) NOT NULL,
				[QtyFlag] [char](1) NOT NULL,
				[ProductType] [varchar](15) NOT NULL,
				[ProfitCenterCode] [varchar](15) NOT NULL,
				[Status] [char](1) NOT NULL,
				[ProcessStatus] [char](1) NOT NULL,
				[ProcessDate] [datetime] NOT NULL,
				[CreatedBy] [varchar](15) NOT NULL,
				[CreatedDate] [datetime] NOT NULL,
				[LastUpdateBy] [varchar](15) NULL,
				[LastUpdateDate] [datetime] NULL)

		insert into svHstSDMovement (CompanyCode, BranchCode, DocNo, DocDate, PartNo, PartSeq, WarehouseCode,
								     QtyOrder, Qty, DiscPct, CostPrice, RetailPrice, TypeOfGoods, CompanyMD, 
									 BranchMD, WarehouseMD, RetailPriceInclTaxMD, RetailPriceMD, CostPriceMD,
									 QtyFlag, ProductType, ProfitCenterCode, Status, ProcessStatus, ProcessDate,
									 CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
							 select  CompanyCode, BranchCode, DocNo, DocDate, PartNo, PartSeq, WarehouseCode,
								     QtyOrder, Qty, DiscPct, CostPrice, RetailPrice, TypeOfGoods, CompanyMD, 
									 BranchMD, WarehouseMD, RetailPriceInclTaxMD, RetailPriceMD, CostPriceMD,
									 QtyFlag, ProductType, ProfitCenterCode, Status, ProcessStatus, ProcessDate,
									 CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate
							   from  svSDMovement
							  where	 ProcessStatus = 1
							   --or  (left(docno,3) in ('INC','INF','INI','INW','INP','PLS','SOC','SPK','SSS','SSU','FPJ','STR')
							     or  (left(docno,3) in ('LMP','PLS','SOC','SPK','SSS','SSU','FPJ','STR')
                                and  convert(varchar,DocDate,111)<=convert(varchar,@PostingDate,111))
                                 or  convert(varchar,DocDate,111)='1900/01/01'
                                 or  Qty<=0

		delete svSDMovement   where	 ProcessStatus = 1
							   --or  (left(docno,3) in ('INC','INF','INI','INW','PLS','SOC','SPK','SSS','SSU','FPJ','STR')
							     or  (left(docno,3) in ('LMP','PLS','SOC','SPK','SSS','SSU','FPJ','STR')
                                and  convert(varchar,DocDate,111)<=convert(varchar,@PostingDate,111))
                                 or  convert(varchar,DocDate,111)='1900/01/01'
                                 or  Qty<=0

		close cursvSDMovement
		deallocate cursvSDMovement

END
--END TRY

--BEGIN CATCH
--    --select ERROR_NUMBER()    AS ErrorNumber,    ERROR_SEVERITY() AS ErrorSeverity, ERROR_STATE()   AS ErrorState,
--		  -- ERROR_PROCEDURE() AS ErrorProcedure, ERROR_LINE()     AS ErrorLine    , ERROR_MESSAGE() AS ErrorMessage
--	if @@TRANCOUNT > 0
--		begin
--			select '0' [STATUS], 'Posting gagal !!!   ' + ERROR_MESSAGE() [INFO]
--			rollback transaction
--			return
--		end
--END CATCH

--IF @@TRANCOUNT > 0
--	begin
--		select '1' [STATUS], 'Posting berhasil !!!' [INFO]
--		rollback transaction
--		--commit transaction
--	end

GO

if object_id('usprpt_PostingMultiCompanySparePartReturn') is not null
	drop procedure usprpt_PostingMultiCompanySparePartReturn
GO
-- POSTING TRANSACTION MULTI COMPANY - SPARE PART RETURN
-- ----------------------------------------------------------------------------------
-- Created  by : HTO, 2014
-- Revision-01 : HTO, January 2015
-- Revision-02 : HTO, April 2015 (Process by Date & LMP => INV Service)
-- ----------------------------------------------------------------------------------
-- This procedure used for 
--		4-Wheeler : BIT-SBT, SBSM-SSBT, BAT-SAT, SIT-SST, UIB-SIT
--		2-Wheeler : IJMG,SVMG,ISG,ISMG-SMG
-- ----------------------------------------------------------------------------------
-- execute [usprpt_PostingMultiCompanySparePartReturn] '6006400001','2014/12/31','0'
-- ----------------------------------------------------------------------------------

CREATE procedure [dbo].[usprpt_PostingMultiCompanySparePartReturn]
	@CompanyCode	varchar(15),
	@PostingDate	datetime,
	@Status			varchar(1)	output
AS	

--BEGIN TRANSACTION
--BEGIN TRY
BEGIN 
		--declare @CompanyCode				varchar(15) = (select top 1 CompanyCode from gnMstOrganizationHdr)
		--declare @PostingDate				datetime    = '2015/04/30' --getdate()
		--declare @Status					varchar(1)  = '0'

 -- Posting process : insert data to MD & SD table
		declare @curCompanyCode				varchar(15)
		declare @curBranchCode				varchar(15)
		declare @curDocNo					varchar(20)
		declare @curDocDate					datetime
		declare @curPartNo					varchar(20)
		declare @curPartSeq					integer
		declare @curWarehouseCode			varchar(15)
		declare @curQtyOrder				numeric(18,2)
		declare @curQty						numeric(18,2)
		declare @curDiscPct					numeric(06,2)
		declare @curCostPrice				numeric(18,2)
		declare @curRetailPrice				numeric(18,2)
		declare @curTypeOfGoods				varchar(15)
		declare @curCompanyMD				varchar(15)
		declare @curBranchMD				varchar(15)
		declare @curWarehouseMD				varchar(15)
		declare @curRetailPriceInclTaxMD	numeric(18,0)
		declare @curRetailPriceMD			numeric(18,0)
		declare @curCostPriceMD				numeric(18,0)
		declare @curQtyFlag					char(1)
		declare @curProductType				varchar(15)
		declare @curProfitCenterCode		varchar(15)
		declare @curStatus					char(1)
		declare @curProcessStatus			char(1)
		declare @curProcessDate				datetime
		declare @curCreatedBy				varchar(15)
		declare @curCreatedDate				datetime
		declare @curLastUpdateBy			varchar(15)
		declare @curLastUpdateDate			datetime

		declare @RetailPriceInclTax         numeric(18,0)
		declare @ReturAmt					numeric(18,0)
		declare @DiscAmt					numeric(18,0)
		declare @NetReturAmt				numeric(18,0)
		declare @PPNAmt						numeric(18,0)
		declare @TotReturAmt				numeric(18,0)
		declare @CostAmt					numeric(18,0)
		declare @TotalNetReturAmt			numeric(18,0)
		declare @TotalPPnAmt				numeric(18,0)
		declare @TotalReturAmt				numeric(18,0)

		declare @MovingCode					varchar(15)
		declare @ABCClass					char(1)
		declare @PartCategory				varchar(3)
		declare @TypeOfGoods				varchar(15)
		declare @LocationCode				varchar(10)
		declare @MovingCodeMD				varchar(15)
		declare @ABCClassMD					char(1)
		declare @PartCategoryMD				varchar(3)
		declare @TypeOfGoodsMD				varchar(15)
		declare @LocationCodeMD				varchar(10)
		declare @ProductTypeMD				varchar(15)
		declare @SONoMD 					varchar(15) = ''
		declare @NPWPNoMD					varchar(50) = ''
		declare @FPJNoMD					varchar(15) = ''
		declare @FPJDateMD					datetime    = '1900/01/01'
		declare @FPJCentralNoMD				varchar(15) = ''
		declare @FPJCentralDateMD			datetime    = '1900/01/01'
		declare @SeqNo						integer
		declare @RTPNo						varchar(15)
		declare @RTSNo						varchar(15)
		declare @FPJNo						varchar(15)
		declare @FPJDate					datetime
		declare @WRSNo						varchar(15)
		declare @WRSDate					datetime
		declare @HPPNo						varchar(15)
		declare @HPPDate					datetime
		declare @ReturnPybAccNo				varchar(100)
		declare @InventoryAccNo				varchar(100)
		declare @TaxInAccNo					varchar(100)
		declare @AccNoReturnMD				varchar(100)
		declare @AccNoTaxOutMD				varchar(100)
		declare @AccNoHReturnMD				varchar(100)
		declare @AccNoDisc1MD				varchar(100)
		declare @AccNoInventoryMD			varchar(100)
		declare @AccNoCogsMD				varchar(100)
		declare @TopCodeMD					varchar(15)
		declare @PaymentCodeMD				varchar(15)
		declare @SalesCodeMD				varchar(15)
		declare @DiscPctMD					numeric(06,2)
		declare @DiscFlag					integer
		declare @TopDaysMD					integer
		declare @DueDateMD					date

		declare @DBName						varchar(50)
		declare @DBNameMD					varchar(50)
		declare @sqlString					nvarchar(max)
		declare @swCompanyCode				varchar(15)   = ''
		declare @swBranchCode				varchar(15)   = ''
		declare @swDocNo					varchar(15)   = ''
		declare @CurrentDate				varchar(100)  = convert(varchar,getdate(),121)
		declare	@SeqNoGL					numeric(10,0) = 0
		declare	@SeqNoGLMD					numeric(10,0) = 0

        declare cursvSDMovement cursor for
			select * from svSDMovement
             where left(DocNo,3) in ('RTR','RTN') 
			   and convert(varchar,DocDate,111)<=convert(varchar,@PostingDate,111)
			   and Qty>0
			   and ProcessStatus=0
             order by convert(varchar,DocDate,111), CompanyCode, BranchCode, DocNo, TypeOfGoods, PartNo, PartSeq
		open cursvSDMovement

		fetch next from cursvSDMovement
			  into @curCompanyCode, @curBranchCode, @curDocNo, @curDocDate, @curPartNo, @curPartSeq, @curWarehouseCode, @curQtyOrder, 
			       @curQty, @curDiscPct, @curCostPrice, @curRetailPrice, @curTypeOfGoods, @curCompanyMD, @curBranchMD, @curWarehouseMD, 
				   @curRetailPriceInclTaxMD, @curRetailPriceMD, @curCostPriceMD, @curQtyFlag, @curProductType, @curProfitCenterCode, 
				   @curStatus, @curProcessStatus, @curProcessDate, @curCreatedBy, @curCreatedDate, @curLastUpdateBy, @curLastUpdateDate

		while (@@FETCH_STATUS =0)
			begin

			 -- Collect MD & SD Database Name from gnMstCompanyMapping
				select @DBNameMD=DbMD, @DBName=DbName from gnMstCompanyMapping 
				 where CompanyCode=@CurCompanyCode and BranchCode =@curBranchCode

			 -- MD: MovingCode, ABCClass, PartCategory, Type of Goods, Location Code, ProducType
				set @sqlString = N'select @MovingCodeMD=MovingCode, @ABCClassMD=ABCClass, ' +
										 '@PartCategoryMD=PartCategory, @TypeOfGoodsMD=TypeOfGoods from ' 
										  +@DBNameMD+ '..spMstItems where CompanyCode=''' 
										  +@curCompanyMD+ ''' and BranchCode=''' 
										  +@curBranchMD+ ''' and PartNo=''' 
										  +@curPartNo + ''''
					execute sp_executesql @sqlString, 
							N'@MovingCodeMD		varchar(15)	output,
							  @ABCClassMD		char(1)		output,
							  @PartCategoryMD	varchar(3)	output,
							  @TypeOfGoodsMD	varchar(15) output', 
							  @MovingCodeMD		output,
							  @ABCClassMD		output,
							  @PartCategoryMD	output,
							  @TypeOfGoodsMD	output

				set @sqlString = N'select @LocationCodeMD=LocationCode from ' +@DBNameMD+ 
										  '..spMstItemLoc where CompanyCode=''' +@curCompanyMD+ 
										  ''' and BranchCode=''' +@curBranchMD+ ''' and PartNo=''' 
										  +@curPartNo + ''' and WarehouseCode=''00'''
					execute sp_executesql @sqlString, 
							N'@LocationCodeMD	varchar(10) output', 
							  @LocationCodeMD	output

				set @sqlString = N'select @ProductTypeMD=ProductType from ' +@DBNameMD+ 
										  '..gnMstCoProfile where CompanyCode=''' +@curCompanyMD+ 
										  ''' and BranchCode=''' +@curBranchMD+ ''''
					execute sp_executesql @sqlString, 
							N'@ProductTypeMD	varchar(15) output', 
							  @ProductTypeMD	output

			 -- SD: MovingCode, ABCClass, PartCategory, Type of Goods
				set @sqlString = N'select @MovingCode=MovingCode, @ABCClass=ABCClass, ' +
										 '@PartCategory=PartCategory, @TypeOfGoods=TypeOfGoods from ' 
										  +@DBName+ '..spMstItems where CompanyCode=''' 
										  +@curCompanyCode+ ''' and BranchCode=''' 
										  +@curBranchCode+''' and PartNo=''' 
										  +@curPartNo + ''''
					execute sp_executesql @sqlString, 
							N'@MovingCode		varchar(15)	output,
							  @ABCClass			char(1)		output,
							  @PartCategory		varchar(3)	output,
							  @TypeOfGoods		varchar(15) output', 
							  @MovingCode		output,
							  @ABCClass			output,
							  @PartCategory		output,
							  @TypeOfGoods		output

			 -- SD: Location Code
				set @sqlString = N'select @LocationCode=LocationCode from ' +@DBName+ 
										  '..spMstItemLoc where CompanyCode=''' +@curCompanyCode+ 
										  ''' and BranchCode=''' +@curBranchCode+ ''' and PartNo=''' 
										  +@curPartNo + ''' and WarehouseCode=''00'''
					execute sp_executesql @sqlString, 
							N'@LocationCode		varchar(10) output', 
							  @LocationCode		output

			 -- SD: ProducType
				--set @sqlString = N'select @ProductType=ProductType from ' +@DBName+ 
				--						  '..gnMstCoProfile where CompanyCode=''' +@curCompanyCode+ 
				--						  ''' and BranchCode=''' +@curBranchCode+ ''''
				--	execute sp_executesql @sqlString, 
				--			N'@ProductType		varchar(15) output', 
				--			  @ProductType		output

			 -- SD: RetailPriceInclTax
				set @sqlString = N'select @RetailPriceInclTax=RetailPriceInclTax from ' 
										  +@DBName+ '..spMstItemPrice where CompanyCode=''' 
										  +@curCompanyCode+ ''' and BranchCode=''' 
										  +@curBranchCode+''' and PartNo=''' 
										  +@curPartNo + ''''
					execute sp_executesql @sqlString, 
							N'@RetailPriceInclTax	numeric(18,2)	output', 
							  @RetailPriceInclTax	output
				set @RetailPriceInclTax = isnull(@RetailPriceInclTax,0.00)

			 -- SD: InventoryAccNo (Inventory Acc No) & ReturnPybAccNo (Return Purchase Acc No)
				set @sqlString = N'select @InventoryAccNo=InventoryAccNo, @ReturnPybAccNo=ReturnPybAccNo from ' 
										 +@DBName+ '..spMstAccount where CompanyCode=''' 
										 +@curCompanyCode+ ''' and BranchCode=''' 
										 +@curBranchCode+''' and TypeOfGoods=''' 
										 +@curTypeOfGoods + ''''
					execute sp_executesql @sqlString, 
							N'@InventoryAccNo	varchar(100) output,
							  @ReturnPybAccNo	varchar(100) output', 
							  @InventoryAccNo	output,
							  @ReturnPybAccNo	output
				set @InventoryAccNo = isnull(@InventoryAccNo,'*** InventoryAccNo ***')
				set @ReturnPybAccNo = isnull(@ReturnPybAccNo,'*** ReturnPybAccNo ***')

			 -- SD: TaxInAccNo (Pajak Masukan Account No)
				set @sqlString = N'select @TaxInAccNo=c.TaxInAccNo from ' 
										 +@DBName+ '..gnMstSupplierClass c, '
										 +@DBName+ '..gnMstSupplierProfitCenter p' +
										 ' where c.CompanyCode     =p.CompanyCode' +
										 '   and c.BranchCode      =p.BranchCode' +
										 '   and c.SupplierClass   =p.SupplierClass' +
										 '   and c.TypeOfGoods     ='''+@curTypeOfGoods+
										 ''' and c.ProfitCenterCode=''300''' +
										 '   and p.CompanyCode     ='''+@curCompanyCode+ 
										 ''' and p.BranchCode      ='''+@curBranchCode+
										 ''' and p.SupplierCode    ='''+@curBranchMD+ 
										 ''' and p.ProfitCenterCode=''300'''
					execute sp_executesql @sqlString, 
							N'@TaxInAccNo	varchar(100) output',
							  @TaxInAccNo	output
				set @TaxInAccNo = isnull(@TaxInAccNo,'*** TaxInAccNo ***')

			 -- MD: AccNoCogsMD, AccNoInventoryMD, AccNoDisc1MD, AccNoReturnMD, AccNoHReturnMD
				set @sqlString = N'select @AccNoCogsMD=COGSAccNo, @AccNoInventoryMD=InventoryAccNo, @AccNoDisc1MD=DiscAccNo, 
										  @AccNoReturnMD=ReturnAccNo, @AccNoHReturnMD=ReturnPybAccNo from ' 
										 +@DBNameMD+ '..spMstAccount where CompanyCode=''' 
										 +@curCompanyMD+ ''' and BranchCode=''' 
										 +@curBranchMD+''' and TypeOfGoods=''' 
										 +@TypeOfGoodsMD + ''''
					execute sp_executesql @sqlString, 
							N'@AccNoCogsMD		varchar(100) output,
							  @AccNoInventoryMD	varchar(100) output,
							  @AccNoDisc1MD		varchar(100) output,
							  @AccNoReturnMD	varchar(100) output,
							  @AccNoHReturnMD	varchar(100) output', 
							  @AccNoCogsMD		output,
							  @AccNoInventoryMD	output,
							  @AccNoDisc1MD		output,
							  @AccNoReturnMD	output,
							  @AccNoHReturnMD	output
				set @AccNoCogsMD	  = isnull(@AccNoCogsMD,     '*** @AccNoCogsMD ***')
				set @AccNoInventoryMD = isnull(@AccNoInventoryMD,'*** @AccNoInventoryMD ***')
				set @AccNoDisc1MD	  = isnull(@AccNoDisc1MD,	 '*** @AccNoDisc1MD ***')
				set @AccNoReturnMD	  = isnull(@AccNoReturnMD,   '*** @AccNoReturnMD ***')
				set @AccNoHReturnMD   = isnull(@AccNoHReturnMD,  '*** @AccNoHReturnMD ***')

			 -- MD: AccNoTaxOutMD (Pajak Keluaran Account No)
				set @sqlString = N'select @AccNoTaxOutMD=c.TaxOutAccNo from ' 
										 +@DBName+ '..gnMstCustomerClass c, '
										 +@DBName+ '..gnMstCustomerProfitCenter p' +
										 ' where c.CompanyCode     =p.CompanyCode' +
										 '   and c.BranchCode      =p.BranchCode' +
										 '   and c.CustomerClass   =p.CustomerClass' +
										 '   and c.TypeOfGoods     ='''+@curTypeOfGoods+
										 ''' and c.ProfitCenterCode=''300''' +
										 '   and p.CompanyCode     ='''+@curCompanyCode+ 
										 ''' and p.BranchCode      ='''+@curBranchCode+
										 ''' and p.CustomerCode    ='''+@curBranchMD+ 
										 ''' and p.ProfitCenterCode=''300'''
					execute sp_executesql @sqlString, 
							N'@AccNoTaxOutMD	varchar(100) output',
							  @AccNoTaxOutMD	output
				set @AccNoTaxOutMD = isnull(@AccNoTaxOutMD,'*** @AccNoTaxOutMD ***')

			 -- MD: Top Code, Payment Code, Sales Code, Discount 
				set @sqlString = N'select @TopCodeMD=TopCode, @PaymentCodeMD=PaymentCode, @SalesCodeMD=SalesCode, @DiscPctMD=DiscPct from ' 
										  +@DBNameMD+ '..gnMstCustomerProfitCenter where CompanyCode=''' 
										  +@curCompanyMD+ ''' and BranchCode=''' 
										  +@curBranchMD+ ''' and CustomerCode=''' 
										  +@curBranchCode+ ''' and ProfitCenterCode=''300'''
					execute sp_executesql @sqlString, 
										N'@TopCodeMD 		varchar(15)  output,
										  @PaymentCodeMD 	varchar(15)  output,
										  @SalesCodeMD 		varchar(15)  output,
										  @DiscPctMD        numeric(5,2) output', 
										  @TopCodeMD 		output,
										  @PaymentCodeMD 	output,
										  @SalesCodeMD 		output,
										  @DiscPctMD     	output

			 -- MD: Top Days
				set @sqlString = N'select @TopDaysMD=ParaValue from ' 
										  +@DBNameMD+ '..gnMstLookUpDtl where CompanyCode=''' 
										  +@curCompanyMD+ ''' and CodeID=''TOPC'' and LookUpValue=''' 
										  +@TopCodeMD+ ''''
					execute sp_executesql @sqlString, 
										N'@TopDaysMD 		integer	output', 
										  @TopDaysMD 		output

						set @sqlString = N'select @FPJNoMD=h.FPJNo, @FPJDateMD=h.FPJDate, @SONoMD=d.DocNo from ' 
										  +@DBName+ '..spTrnSRturHdr r left join ' +@DBNameMD+ '..spTrnSInvoiceDtl d on d.CompanyCode=''' +@curCompanyMD+ 
										  ''' and d.BranchCode=''' +@curBranchMD+ ''' and d.ReferenceNo=r.ReferenceNo and d.PartNo=''' +@curPartNo+ ''' left join ' +@DBNameMD+ 
										  '..spTrnSInvoiceHdr h on h.CompanyCode=d.CompanyCode and h.BranchCode=d.BranchCode and h.InvoiceNo=d.InvoiceNo where r.CompanyCode=''' 
										  +@curCompanyCode+ ''' and r.BranchCode=''' +@curBranchCode+ ''' and r.ReturnNo=''' +@curDocNo+ ''''
						execute sp_executesql @sqlString, 
										 N'@FPJNoMD 	varchar(15)	output,
										   @FPJDateMD	datetime	output,
										   @SONoMD		varchar(15) output', 
										   @FPJNoMD 	output,
										   @FPJDateMD	output,
										   @SONoMD

			 -- SD: FPJNo & FPJDate
				if left(@curDocNo,3) = 'RTR'  -- Retur Sparepart  >>spTrnSRturHdr (ReferenceNo=POS#)
					begin
						set @sqlString = N'select top 1 @FPJNo=r.FPJNo, @FPJDate=r.FPJDate, @WRSNo=d.WRSNo, ' +
						                  '@WRSDate=h.WRSDate, @HPPNo=h.HPPNo, @HPPDate=h.HPPDate from '
										  +@DBName+ '..spTrnSRturHdr r left join ' +@DBName+ '..spTrnPRcvDtl d on d.CompanyCode='''
										  +@curCompanyCode+ ''' and d.BranchCode=''' +@curBranchCode+ ''' and d.DocNo=r.ReferenceNo and d.PartNo='''
										  +@curPartNo+ ''' left join ' +@DBName+ '..spTrnPHPP h on h.CompanyCode=d.CompanyCode ' +
										  'and h.BranchCode=d.BranchCode and h.WRSNo=d.WRSNo where r.CompanyCode='''
										  +@curCompanyCode+ ''' and r.BranchCode=''' +@curBranchCode+ ''' and r.ReturnNo=''' +@curDocNo+ ''''
							execute sp_executesql @sqlString, 
										 N'@FPJNo	varchar(15)	output,
										   @FPJDate	datetime	output,
										   @WRSNo	varchar(15) output,
										   @WRSDate datetime    output,
										   @HPPNo   varchar(15) output,
										   @HPPDate datetime    output', 
										   @FPJNo	output,
										   @FPJDate output,
										   @WRSNo	output,
										   @WRSDate output,
										   @HPPNo	output,
										   @HPPDate	output

						set @sqlString = N'select top 1 @FPJNoMD=h.FPJNo, @FPJDateMD=h.FPJDate, @SONoMD=d.DocNo, @NPWPNoMD=f.FPJGovNo from ' 
						                  +@DBName+ '..spTrnSRturHdr r left join ' +@DBNameMD+ '..spTrnSInvoiceDtl d on d.CompanyCode=''' 
										  +@curCompanyMD+ ''' and d.BranchCode=''' +@curBranchMD+ ''' and d.ReferenceNo=r.ReferenceNo and d.PartNo=''' 
										  +@curPartNo+ ''' left join ' +@DBNameMD+ '..spTrnSInvoiceHdr h on h.CompanyCode=d.CompanyCode and ' +
										  'h.BranchCode=d.BranchCode and h.InvoiceNo=d.InvoiceNo left join ' +@DbNameMD+ '..spTrnSFPJHdr f ' +
										  'on f.CompanyCode=h.CompanyCode and f.BranchCode=h.BranchCode and f.FPJNo=h.FPJNo where r.CompanyCode='''
										  +@curCompanyCode+ ''' and r.BranchCode=''' +@curBranchCode+ ''' and r.ReturnNo=''' +@curDocNo+ ''''
						execute sp_executesql @sqlString, 
										 N'@FPJNoMD 	varchar(15)	output,
										   @FPJDateMD	datetime	output,
										   @SONoMD		varchar(15) output,
										   @NPWPNoMD	varchar(50) output', 
										   @FPJNoMD 	output,
										   @FPJDateMD	output,
										   @SONoMD		output,
										   @NPWPNoMD	output
					end

				else

				if left(@CurDocNo,3) = 'RTN'  -- Retur Service   >>spTrnSRturSrvHdr (ReferenceNo=POS#)
					begin
						set @sqlString = N'select @FPJNo=rh.InvoiceNo, @FPJDate=rh.InvoiceDate, @WRSNo=wh.WRSNo, ' +
						                  '@WRSDate=hp.WRSDate, @HPPNo=hp.HPPNo, @HPPDate=hp.HPPDate from ' 
										  +@DBName+ '..spTrnSRturSrvHdr rh left join ' 
										  +@DBName+ '..spTrnSRturSrvDtl rd on rd.CompanyCode=rh.CompanyCode ' +
										  'and rd.BranchCode=rh.BranchCode and rd.ReturnNo=rh.ReturnNo ' +
										  'and rd.PartNo='''+@curPartNo+''' left join '
										  +@DBName+ '..svTrnInvoice iv on iv.CompanyCode=rh.CompanyCode ' +
										  'and iv.BranchCode=rh.BranchCode and iv.InvoiceNo=rh.InvoiceNo left join '
										  +@DBName+ '..spTrnSLmpDtl ld on ld.CompanyCode=rh.CompanyCode ' +
										  'and ld.BranchCode=rh.BranchCode and ld.PartNo='''+@curPartNo+
										  ''' and ld.ReferenceNo=iv.JobOrderNo left join '
										  +@DBName+ '..spTrnPPOSHdr po on po.CompanyCode=rh.CompanyCode ' +
										  'and po.BranchCode=rh.BranchCode and po.Remark=ld.LmpNo left join ' 
										  +@DBName+ '..spTrnPRcvDtl wh on wh.CompanyCode=rh.CompanyCode ' +
										  'and wh.BranchCode=rh.BranchCode and wh.PartNo=rd.PartNo ' +
										  'and wh.DocNo=po.POSNo left join '
										  +@DBName+ '..spTrnPHPP hp on hp.CompanyCode=rh.CompanyCode ' +
										  'and hp.BranchCode=rh.BranchCode and hp.WRSNo=wh.WRSNo ' +
										  'where rh.CompanyCode='''+@curCompanyCode+''' and rh.BranchCode='''
										  +@curBranchCode+''' and rh.ReturnNo='''+@curDocNo+''''
							execute sp_executesql @sqlString, 
										 N'@FPJNo	varchar(15)	output,
										   @FPJDate	datetime	output,
										   @WRSNo	varchar(15) output,
										   @WRSDate datetime    output,
										   @HPPNo   varchar(15) output,
										   @HPPDate datetime    output', 
										   @FPJNo	output,
										   @FPJDate output,
										   @WRSNo	output,
										   @WRSDate output,
										   @HPPNo	output,
										   @HPPDate	output

						set @sqlString = N'select top 1 @FPJNoMD=h.FPJNo, @FPJDateMD=h.FPJDate, @SONoMD=d.DocNo, @NPWPNoMD=f.FPJGovNo from ' +@DBName+ '..spTrnSRturSrvHdr r left join ' +@DBNameMD+ 
										  '..spTrnSInvoiceDtl d on d.CompanyCode=''' +@curCompanyMD+ ''' and d.BranchCode=''' +@curBranchMD+ ''' and d.ReferenceNo=r.ReferenceNo and d.PartNo='''
										  +@curPartNo+ ''' left join ' +@DBNameMD+ '..spTrnSInvoiceHdr h on h.CompanyCode=d.CompanyCode and h.BranchCode=d.BranchCode and h.InvoiceNo=d.InvoiceNo ' +
										  'left join ' +@DbNameMD+ '..spTrnSFPJHdr f on f.CompanyCode=h.CompanyCode and f.BranchCode=h.BranchCode and f.FPJNo=h.FPJNo where r.CompanyCode=''' 
										  +@curCompanyCode+ ''' and r.BranchCode=''' +@curBranchCode+ ''' and r.ReturnNo=''' +@curDocNo+ ''''
						execute sp_executesql @sqlString, 
										 N'@FPJNoMD 	varchar(15)	output,
										   @FPJDateMD	datetime	output,
										   @SONoMD		varchar(15) output,
										   @NPWPNoMD	varchar(50) output', 
										   @FPJNoMD 	output,
										   @FPJDateMD	output,
										   @SONoMD		output,
										   @NPWPNoMD	output
					end

				set @FPJNo     = isnull(@FPJNo,'')
				set @FPJDate   = isnull(@FPJDate,'1900/01/01')
				set @WRSNo     = isnull(@WRSNo,'')
				set @WRSDate   = isnull(@WRSDate,'1900/01/01')
				set @HPPNo     = isnull(@HPPNo,'')
				set @HPPDate   = isnull(@HPPDate,'1900/01/01')
				set @FPJNoMD   = isnull(@FPJNoMD,'')
				set @FPJDateMD = isnull(@FPJDateMD,'')
				set @SONoMD	   = isnull(@SONoMD,'')
				set @NPWPNoMD  = isnull(@NPWPNoMD,'')

				if @swCompanyCode <> @curCompanyCode or
				   @swBranchCode  <> @curBranchCode  or
				   @swDocNo		  <> @curDocNo
					begin
						set @swCompanyCode    = @curCompanyCode
						set @swBranchCode     = @curBranchCode
						set @swDocNo          = @curDocNo
						set @SeqNo		      = 0
						set @TotalNetReturAmt = 0
						set @DiscFlag         = 1

					 -- SD : Insert data to table spTrnPRturHdr for Purchase Return
						execute [usprpt_PostingMultiCompany4DocNo] @curCompanyCode, @curBranchCode, @DBName, 'RTB', @RTPNo output
						set @sqlString = 'insert into ' +@DBName+ '..spTrnPRturHdr 
												(CompanyCode, BranchCode, ReturnNo, ReturnDate, SupplierCode, 
												 ReferenceNo, ReferenceDate, TotReturQty, TotReturAmt, TotDiscAmt,
												 TotDPPAmt, TotPPNAmt, TotFinalReturAmt, TotCostAmt, PrintSeq, 
												 Status, Remark, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
										values(''' +@curCompanyCode+ ''',''' +@curBranchCode+ ''',''' +@RTPNo+ ''',''' 
										           +convert(varchar,@curDocDate,121)+ ''',''' +@curBranchMD+ 
												   ''',''DAILY-POSTING'',''' +convert(varchar,@curDocDate,121)+
												   ''',0,0,0,0,0,0,0,1,2,NULL,'''
												   +@curCreatedBy+ ''',''' +convert(varchar,@curCreatedDate,121)+ ''','''
												   +@curLastUpdateBy+ ''',''' +convert(varchar,@curLastUpdateDate,121)+ ''')'
							execute sp_executesql @sqlString

					 -- MD : Collect FPJNo, FPJDate, FPJCentralNo, FPJCentralDate from 
					 --      SD table : spTrnSRturHdr 
					 --      MD table : spTrnSORDHdr, spTrnSFPJDtl, spTrnSFPJHdr

						set @sqlString = N'select top 1 @FPJNoMD=fd.FPJNo, @FPJDateMD=fh.FPJDate, ' +
										  '@FPJCentralNoMD=fh.FPJCentralNo, @FPJCentralDateMD=fh.FPJCentralDate from ' 
										  +@DBName+ '..spTrnSRturHdr rh left join ' 
										  +@DBNameMD+ '..spTrnSORDHdr sh on sh.CompanyCode=''' +@curCompanyMD+
										  ''' and sh.BranchCode=''' +@curBranchMD+ 
										  ''' and sh.LockingBy=rh.FPJNo left join spTrnSFPJDtl fd ' +
										  'on fd.CompanyCode=sh.CompanyCode and fd.BranchCode=sh.BranchCode '+
										  'and fd.DocNo=sh.DocNo and fd.PartNo=''' +@curPartNo+
										  ''' left join spTrnSFPJHdr fh on fh.CompanyCode=fd.CompanyCode ' +
										  'and fh.BranchCode=fd.BranchCode and fh.FPJNo=fd.FPJNo ' +
										  'where rh.CompanyCode='''+@curCompanyCode+''' and rh.BranchCode='''
										  +@curBranchCode+''' and rh.ReturnNo='''+@curDocNo+''''
							execute sp_executesql @sqlString, 
										 N'@FPJNoMD				varchar(15)	output,
										   @FPJDateMD			datetime	output,
										   @FPJCentralNoMD		varchar(15) output,
										   @FPJCentralDateMD	datetime    output', 
										   @FPJNoMD				output,
										   @FPJDateMD			output,
										   @FPJCentralNoMD		output,
										   @FPJCentralDateMD	output
						set @FPJNoMD          = isnull(@FPJNoMD,'')
						set @FPJDateMD        = isnull(@FPJDateMD,'1900/01/01')
						set @FPJCentralNoMD   = isnull(@FPJCentralNoMD,'')
						set @FPJCentralDateMD = isnull(@FPJCentralDateMD,'1900/01/01')
 
					 -- MD : Insert data to table spTrnSRturHdr for Sales Return 
						execute [usprpt_PostingMultiCompany4DocNo] @curCompanyMD, @curBranchMD, @DBNameMD, 'RTR', @RTSNo output
						set @sqlString = 'insert into ' +@DBNameMD+ '..spTrnSRturHdr 
												(CompanyCode, BranchCode, ReturnNo, ReturnDate, CustomerCode, 
												 FPJNo, FPJDate, FPJCentralNo, FPJCentralDate, ReferenceNo, 
												 ReferenceDate, TotReturQty, TotReturAmt, TotDiscAmt,
												 TotDPPAmt, TotPPNAmt, TotFinalReturAmt, TotCostAmt, isPKP,
												 NPWPNo, PrintSeq, Status, TypeOfGoods, CreatedBy, CreatedDate, 
												 LastUpdateBy, LastUpdateDate, isLocked, LockingBy, LockingDate)
										values(''' +@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' +@RTSNo+ ''',''' 
										           +convert(varchar,@curDocDate,121)+ ''',''' +@curBranchCode+
												   ''',''' +@FPJNoMD+ ''',''' +convert(varchar,@FPJDateMD,121)+
												   ''',''' +@FPJCentralNoMD+ ''',''' +convert(varchar,@FPJCentralDateMD,121)+ 
												   ''',''' +@RTPNo+ ''',''' +convert(varchar,@curDocDate,121)+ 
												   ''',0,0,0,0,0,0,0,1,''' +@NPWPNoMD+ ''',''1'',''2'',''' +@TypeOfGoodsMD+
												   ''',''POSTING'',''' +convert(varchar,@PostingDate,111)+ 
												   ''',''POSTING'',''' +@currentDate+ 
												   ''',0,''' +@curDocNo+ ''',''' +convert(varchar,@curDocDate,121)+ ''')'
							execute sp_executesql @sqlString
					end   

				set @SeqNo            = @SeqNo + 1
				set @ReturAmt         = isnull((@curQty*@curRetailPriceMD),0)
				set @DiscAmt          = isnull(((@ReturAmt*@DiscPctMD)/100),0)
				set @NetReturAmt      = @ReturAmt - @DiscAmt
				set @PPNAmt           = round((@NetReturAmt*0.1),0)
				set @TotReturAmt      = @NetReturAmt + @PPNAmt
				set @CostAmt          = isnull((@curQty * @curCostPriceMD),0)
				set @TotalNetReturAmt = @TotalNetReturAmt + @NetReturAmt
				set @TotalPPnAmt      = @TotalNetReturAmt * 0.1
				set @TotalReturAmt    = @TotalNetReturAmt + @TotalPPnAmt

			 -- SD: Insert data to table spTrnPRturDtl
				set @sqlString = 'insert into ' +@DBName+ '..spTrnPRturDtl
										(CompanyCode, BranchCode, ReturnNo, SeqNo, PartNo, WarehouseCode, QtyReturn, 
										 RetailPriceInclTax, RetailPrice, CostPrice, DiscPct, ReturAmt, DiscAmt, 
										 NetReturAmt, PPNAmt, TotReturAmt, CostAmt, LocationCode, ProductType, 
										 PartCategory, MovingCode, ABCCLass, TypeOfGoods, SalesReturnNo, 
										 SalesReturnDate, DocNo, DocDate, WRSNo, WRSDate, HPPNo, HPPDate, 
										 CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
								   values(''' +@curCompanyCode+ ''',''' +@curBranchCode+ ''',''' +@RTPNo+ 
										  ''',' +convert(varchar,@SeqNo)+ ',''' +@curPartNo+  
										  ''',''' +@curWarehouseCode+ ''',' +convert(varchar,@curQty)+ 
										  ',' +convert(varchar,@RetailPriceInclTax)+ ',' +convert(varchar,@curRetailPrice)+ 
										  ',' +convert(varchar,@curCostPrice)+ ',' +convert(varchar,@curDiscPct)+ 
										  ',' +convert(varchar,@ReturAmt)+ ',' +convert(varchar,@DiscAmt)+ 
										  ',' +convert(varchar,@NetReturAmt)+ ',' +convert(varchar,@PPNAmt)+ 
										  ',' +convert(varchar,@TotReturAmt)+ ',' +convert(varchar,@CostAmt)+
										  ',''' +@LocationCode+ ''',''' +@curProductType+ ''',''' +@PartCategory+
										  ''',''' +@MovingCode+ ''',''' +@ABCClass+ ''',''' +@TypeOfGoods+
										  ''',''' +@curDocNo+ ''',''' +convert(varchar,@curDocDate,121)+ 
										  ''',''' +@FPJNo+ ''',''' +convert(varchar,@FPJDate,121)+
										  ''',''' +@WRSNo+ ''',''' +convert(varchar,@WRSDate,121)+
										  ''',''' +@HPPNo+ ''',''' +convert(varchar,@HPPDate,121)+
										  ''',''' +@curCreatedBy+ ''',''' +convert(varchar,@curCreatedDate,121)+ 
										  ''',''' +@curLastUpdateBy+ ''',''' +convert(varchar,@curLastUpdateDate,121)+ ''')'
					execute sp_executesql @sqlString  

			 -- SD: Update data to table spTrnPRturHdr
				set @sqlString = 'update ' +@DBName+ '..spTrnPRturHdr set' +
										   '  TotReturQty=TotReturQty+' +convert(varchar,@curQty)+
										   ', TotReturAmt=ToTReturAmt+' +convert(varchar,@ReturAmt)+
										   ', TotDiscAmt=TotDiscAmt+'   +convert(varchar,@DiscAmt)+
										   ', TotDPPAmt='				+convert(varchar,@TotalNetReturAmt)+
										   ', TotPPNAmt='				+convert(varchar,@TotalPPnAmt)+
										   ', TotFinalReturAmt='		+convert(varchar,@TotalReturAmt)+
										   ', TotCostAmt=TotCostAmt+'   +convert(varchar,@CostAmt)+
										   ' where CompanyCode='''+@curCompanyCode+''' and BranchCode='''
										   +@curBranchCode+''' and ReturnNo=''' +@RTPNo+ ''''
					execute sp_executesql @sqlString

			 -- SD: Insert data to table spTrnIMovement
				set @sqlString = 'insert into ' +@DBName+ '..spTrnIMovement
										(CompanyCode, BranchCode, DocNo, DocDate, CreatedDate, WarehouseCode, 
										 LocationCode, PartNo, SignCode, SubSignCode, Qty, Price, CostPrice, 
										 ABCClass, MovingCode, ProductType, PartCategory, CreatedBy)
								   values(''' +@curCompanyCode+ ''',''' +@curBranchCode+ ''',''' 
										  +@RTPNo+ ''',''' +convert(varchar,@curDocDate,121)+ 
								          ''',''' +convert(varchar,getdate(),121)+ ''',''' 
										  +@curWarehouseCode+ ''',''' +@LocationCode+ ''',''' 
										  +@curPartNo+ ''',''OUT'',''RETUR PURCHASE'','''
										  +convert(varchar,@curQty)+ ''',''' 
										  +convert(varchar,@curRetailPrice)+ ''',''' 
										  +convert(varchar,@curCostPrice)+ ''',''' 
										  +@ABCClass+ ''',''' +@MovingCode+ ''',''' 
										  +@curProductType+ ''',''' +@PartCategory+ ''',''POSTING'')'
					execute sp_executesql @sqlString

			 -- SD: Insert/Update data to table apInterface
				set @sqlString = 'merge into ' +@DBName+ '..apInterface as ap using ( values(''' 
								 +@curCompanyCode+ ''',''' +@curBranchCode+ ''',''' +@RTPNo+ ''',''' 
								 +@curProfitCenterCode+ ''',''' +convert(varchar,@curDocDate,121)+ 
								 ''',''PURCHASERETURN'',''' +convert(varchar,@curDocDate,121)+ ''',' 
								 +convert(varchar,@TotalNetReturAmt)+ ',0,''' +@curBranchMD+ ''',' 
								 +convert(varchar,@TotalPPnAmt)+ ',0,''' +@ReturnPybAccNo+ ''','''
								 +convert(varchar,@curDocDate,121)+ ''',NULL,' 
								 +convert(varchar,@TotalReturAmt)+ ',0,''' +@curCreatedBy+ ''','''
								 +convert(varchar,@curCreatedDate,121)+ ''',0,''' +@FPJNoMD+ ''',''' 
								 +convert(varchar,@FPJDateMD,121)+ ''',''' +@HPPNo+ '''))
						as SRC (NewCompany, NewBranch, NewDocNo, NewProfitCenterCode, NewDocDate, NewReference, 
								NewReferenceDate, NewNetAmt, NewPPHAmt, NewSupplierCode, NewPPNAmt, NewPPnBM, 
								NewAccountNo, NewTermsDate, NewTermsName, NewTotalAmt, NewStatusFlag, NewCreateBy, 
								NewCreateDate, NewReceiveAmt, NewFakturPajakNo, NewFakturPajakDate, NewRefNo)
						on ap.CompanyCode = SRC.NewCompany
					   and ap.BranchCode  = SRC.NewBranch
					   and ap.DocNo       = SRC.NewDocNo
					  when matched 
						   then update set NetAmt   = NewNetAmt
										 , PPNAmt   = NewPPNAmt
										 , TotalAmt = NewTotalAmt
					  when not matched by target 
						   then insert (CompanyCode, BranchCode, DocNo, ProfitCenterCode, DocDate, Reference, 
										ReferenceDate, NetAmt, PPHAmt, SupplierCode, PPNAmt, PPnBM, AccountNo, 
										TermsDate, TermsName, TotalAmt, StatusFlag, CreateBy, CreateDate, 
										ReceiveAmt, FakturPajakNo, FakturPajakDate, RefNo)
								values (NewCompany, NewBranch, NewDocNo, NewProfitCenterCode, NewDocDate, NewReference, 
										NewReferenceDate, NewNetAmt, NewPPHAmt, NewSupplierCode, NewPPNAmt, NewPPnBM, 
										NewAccountNo, NewTermsDate, NewTermsName, NewTotalAmt, NewStatusFlag, NewCreateBy, 
										NewCreateDate, NewReceiveAmt, NewFakturPajakNo, NewFakturPajakDate, NewRefNo);'
					execute sp_executesql @sqlString

			 -- SD: Insert/Update data to table glInterface
				-- glInterface - PSEMENTARA 
				set @SeqNoGL = 1
				set @sqlString = 'merge into ' +@DBName+ '..glInterface as gl using ( values(''' 
								 +@curCompanyCode+ ''',''' +@curBranchCode+ ''',''' +@RTPNo+ ''',''' 
								 +convert(varchar,@SeqNoGl)+ ''',''' +convert(varchar,@curDocDate,121)+ 
								 ''',''300'',''' +convert(varchar,@curDocDate,121)+ ''',''' +@ReturnPybAccNo+
								 ''',''SPAREPART'',''PURCHASERETURN'',''' +@RTPNo+ ''',''' 
								 +convert(varchar,@TotalReturAmt)+ ''',0,''PSEMENTARA'',''' +@HPPNo+ 
								 ''',NULL,0,''POSTING'',''' +convert(varchar,@PostingDate,111)+ 
								 ''',''POSTING'',''' +@currentDate+ '''))
						as SRC (NewCompany, NewBranch, NewDocNo, NewSeqNo, NewDocDate, NewProfitCenterCode, 
								NewAccDate, NewAccountNo, NewJournalCode, NewTypeJournal, NewApplyTo, 
								NewAmountDb, NewAmountCr, NewTypeTrans, NewBatchNo, NewBatchDate, 
								NewStatusFlag, NewCreateBy, NewCreateDate, NewLastUpdateBy, NewLastUpdateDate)
						on gl.CompanyCode      = SRC.NewCompany
					   and gl.BranchCode	   = SRC.NewBranch
					   and gl.DocNo			   = SRC.NewDocNo
					   and gl.ProfitCenterCode = SRC.NewProfitCenterCode
					   and gl.TypeTrans        = ''PSEMENTARA''
					  when matched 
						   then update set AmountDb = NewAmountDb
					  when not matched by target 
						   then insert (CompanyCode, BranchCode, DocNo, SeqNo, DocDate, ProfitCenterCode, AccDate, AccountNo,
										 JournalCode, TypeJournal, ApplyTo, AmountDb, AmountCr, TypeTrans, BatchNo, BatchDate,
										 StatusFlag, CreateBy, CreateDate, LastUpdateBy, LastUpdateDate)
								values (NewCompany, NewBranch, NewDocNo, NewSeqNo, NewDocDate, NewProfitCenterCode, NewAccDate, 
								        NewAccountNo, NewJournalCode, NewTypeJournal, NewApplyTo, NewAmountDb, NewAmountCr, 
										NewTypeTrans, NewBatchNo, NewBatchDate, NewStatusFlag, NewCreateBy, NewCreateDate, 
										NewLastUpdateBy, NewLastUpdateDate);'
					execute sp_executesql @sqlString

 			 -- glInterface - INVENTORY
				set @SeqNoGL = @SeqNoGl + 1
				set @sqlString = 'merge into ' +@DBName+ '..glInterface as gl using ( values(''' 
								 +@curCompanyCode+ ''',''' +@curBranchCode+ ''',''' +@RTPNo+ ''',''' 
								 +convert(varchar,@SeqNoGl)+ ''',''' +convert(varchar,@curDocDate,121)+ 
								 ''',''300'',''' +convert(varchar,@curDocDate,121)+ ''',''' +@InventoryAccNo+
								 ''',''SPAREPART'',''PURCHASERETURN'',''' +@RTPNo+ ''',0,''' 
								 +convert(varchar,@TotalNetReturAmt)+ ''',''INVENTORY'',''' +@HPPNo+ 
								 ''',NULL,0,''POSTING'',''' +convert(varchar,@PostingDate,111)+ 
								 ''',''POSTING'',''' +@currentDate+ '''))
						as SRC (NewCompany, NewBranch, NewDocNo, NewSeqNo, NewDocDate, NewProfitCenterCode, 
								NewAccDate, NewAccountNo, NewJournalCode, NewTypeJournal, NewApplyTo, 
								NewAmountDb, NewAmountCr, NewTypeTrans, NewBatchNo, NewBatchDate, 
								NewStatusFlag, NewCreateBy, NewCreateDate, NewLastUpdateBy, NewLastUpdateDate)
						on gl.CompanyCode      = SRC.NewCompany
					   and gl.BranchCode	   = SRC.NewBranch
					   and gl.DocNo			   = SRC.NewDocNo
					   and gl.ProfitCenterCode = SRC.NewProfitCenterCode
					   and gl.TypeTrans        = ''INVENTORY''
					  when matched 
						   then update set AmountCr = NewAmountCr
					  when not matched by target 
						   then insert (CompanyCode, BranchCode, DocNo, SeqNo, DocDate, ProfitCenterCode, AccDate, AccountNo,
										 JournalCode, TypeJournal, ApplyTo, AmountDb, AmountCr, TypeTrans, BatchNo, BatchDate,
										 StatusFlag, CreateBy, CreateDate, LastUpdateBy, LastUpdateDate)
								values (NewCompany, NewBranch, NewDocNo, NewSeqNo, NewDocDate, NewProfitCenterCode, NewAccDate, 
								        NewAccountNo, NewJournalCode, NewTypeJournal, NewApplyTo, NewAmountDb, NewAmountCr, 
										NewTypeTrans, NewBatchNo, NewBatchDate, NewStatusFlag, NewCreateBy, NewCreateDate, 
										NewLastUpdateBy, NewLastUpdateDate);'
					execute sp_executesql @sqlString

				-- glInterface - TAX IN
				set @SeqNoGL = @SeqNoGl + 1
				set @sqlString = 'merge into ' +@DBName+ '..glInterface as gl using ( values(''' 
								 +@curCompanyCode+ ''',''' +@curBranchCode+ ''',''' +@RTPNo+ ''',''' 
								 +convert(varchar,@SeqNoGl)+ ''',''' +convert(varchar,@curDocDate,121)+ 
								 ''',''300'',''' +convert(varchar,@curDocDate,121)+ ''',''' +@TaxInAccNo+
								 ''',''SPAREPART'',''PURCHASERETURN'',''' +@RTPNo+ ''',0,''' 
								 +convert(varchar,@TotalPPnAmt)+ ''',''PPN'',''' +@HPPNo+ 
								 ''',NULL,0,''POSTING'',''' +convert(varchar,@PostingDate,111)+ 
								 ''',''POSTING'',''' +@currentDate+ '''))
						as SRC (NewCompany, NewBranch, NewDocNo, NewSeqNo, NewDocDate, NewProfitCenterCode, 
								NewAccDate, NewAccountNo, NewJournalCode, NewTypeJournal, NewApplyTo, 
								NewAmountDb, NewAmountCr, NewTypeTrans, NewBatchNo, NewBatchDate, 
								NewStatusFlag, NewCreateBy, NewCreateDate, NewLastUpdateBy, NewLastUpdateDate)
						on gl.CompanyCode      = SRC.NewCompany
					   and gl.BranchCode	   = SRC.NewBranch
					   and gl.DocNo			   = SRC.NewDocNo
					   and gl.ProfitCenterCode = SRC.NewProfitCenterCode
					   and gl.TypeTrans        = ''PPN''
					  when matched 
						   then update set AmountCr = NewAmountCr
					  when not matched by target 
						   then insert (CompanyCode, BranchCode, DocNo, SeqNo, DocDate, ProfitCenterCode, AccDate, AccountNo,
										 JournalCode, TypeJournal, ApplyTo, AmountDb, AmountCr, TypeTrans, BatchNo, BatchDate,
										 StatusFlag, CreateBy, CreateDate, LastUpdateBy, LastUpdateDate)
								values (NewCompany, NewBranch, NewDocNo, NewSeqNo, NewDocDate, NewProfitCenterCode, NewAccDate, 
								        NewAccountNo, NewJournalCode, NewTypeJournal, NewApplyTo, NewAmountDb, NewAmountCr, 
										NewTypeTrans, NewBatchNo, NewBatchDate, NewStatusFlag, NewCreateBy, NewCreateDate, 
										NewLastUpdateBy, NewLastUpdateDate);'
					execute sp_executesql @sqlString

			 -- MD: Insert data to table spTrnSRturDtl
				set @sqlString = 'insert into ' +@DBNameMD+ '..spTrnSRturDtl
										(CompanyCode, BranchCode, ReturnNo, PartNo, PartNoOriginal, WarehouseCode, 
										 DocNo, ReturnDate, QtyReturn, RetailPriceInclTax, RetailPrice, CostPrice, 
										 DiscPct, ReturAmt, DiscAmt, NetReturAmt, PPNAmt, TotReturAmt, CostAmt, 
										 LocationCode, ProductType, PartCategory, MovingCode, ABCCLass, 
										 CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
								   values(''' +@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' +@RTSNo+ 
										  ''',''' +@curPartNo+ ''',''' +@curPartNo+ ''',''' +@curWarehouseMD+ 
										  ''',''' +@SONoMD+ ''',''' +convert(varchar,@curDocDate,121)+ 
										  ''',' +convert(varchar,@CurQty)+ ',' +convert(varchar,@curRetailPriceInclTaxMD)+
										  ',' +convert(varchar,@curRetailPriceMD)+ ',' +convert(varchar,@curCostPriceMD)+
										  ',' +convert(varchar,@curDiscPct)+ ',' +convert(varchar,@ReturAmt)+
										  ',' +convert(varchar,@DiscAmt)+ ',' +convert(varchar,@NetReturAmt)+
										  ',' +convert(varchar,@PPNAmt)+ ',' +convert(varchar,@TotReturAmt)+
										  ',' +convert(varchar,@CostAmt)+ ',''' +@LocationCodeMD+ 
										  ''',''' +@ProductTypeMD+ ''',''' +@PartCategoryMD+ ''',''' +@MovingCodeMD+
										  ''',''' +@ABCClassMD+ ''',''' +@curCreatedBy+ 
										  ''',''' +convert(varchar,@curCreatedDate,121)+ 
										  ''',''' +@curLastUpdateBy+ 
										  ''',''' +convert(varchar,@curLastUpdateDate,121)+ ''')'
					execute sp_executesql @sqlString  

			 -- MD: Insert data to table spTrnIMovement
				set @sqlString = 'insert into ' +@DBNameMD+ '..spTrnIMovement
										(CompanyCode, BranchCode, DocNo, DocDate, CreatedDate, WarehouseCode, 
										 LocationCode, PartNo, SignCode, SubSignCode, Qty, Price, CostPrice, 
										 ABCClass, MovingCode, ProductType, PartCategory, CreatedBy)
								   values(''' +@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' +@RTSNo+ 
								          ''',''' +convert(varchar,@curDocDate,121)+ 
								          ''',''' +convert(varchar,getdate(),121)+ 
										  ''',''' +@curWarehouseMD+ ''',''' +@LocationCodeMD+ 
										  ''',''' +@curPartNo+ ''',''IN'',''RETUR'','''
										  +convert(varchar,@curQty)+ ''',''' +convert(varchar,@curRetailPriceMD)+
										  ''',''' +convert(varchar,@curCostPriceMD)+ ''',''' +@ABCClassMD+
										  ''',''' +@MovingCodeMD+ ''',''' +@ProductTypeMD+
										  ''',''' +@PartCategoryMD+ ''',''POSTING'')'
					execute sp_executesql @sqlString

			 -- MD: Insert/Update data to table arInterface
				set @sqlString = 'merge into ' +@DBNameMD+ '..arInterface as ar using ( values(''' 
								 +@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' +@RTSNo+ ''',''' 
								 +convert(varchar,@curDocDate,121)+ ''',''300'',''' 
								 +convert(varchar,@TotalReturAmt)+ ''',0,''' 
								 +convert(varchar,@curBranchCode)+ ''',''' +@TopCodeMD+ ''',''' 
								 +convert(varchar,@DueDateMD,111)+ ''',''RETURN'',0,0,0,'''
								 +@SalesCodeMD+ ''',NULL,0,''POSTING'',''' 
								 +convert(varchar,@PostingDate,111)+ ''',''' 
								 +@AccNoHReturnMD+ ''',''' +@FPJNoMD+ ''',''' 
								 +convert(varchar,@FPJDateMD,111)+ '''))
						as SRC (NewCompany, NewBranch, NewDocNo, NewDocDate, NewProfitCenterCode, NewNettAmt, 
								NewReceiveAmt, NewCustomerCode, NewTOPCode, NewDueDate, NewTypeTrans, NewBlockAmt, 
								NewDebetAmt, NewCreditAmt, NewSalesCode, NewLeasingCode, NewStatusFlag, NewCreateBy, 
								NewCreateDate, NewAccountNo, NewFakturPajakNo, NewFakturPajakDate)
						on ar.CompanyCode = SRC.NewCompany
					   and ar.BranchCode  = SRC.NewBranch
					   and ar.DocNo       = SRC.NewDocNo
					  when matched 
						   then update set NettAmt = NewNettAmt
					  when not matched by target 
						   then insert (CompanyCode, BranchCode, DocNo, DocDate, ProfitCenterCode, NettAmt, ReceiveAmt, 
										 CustomerCode, TOPCode, DueDate, TypeTrans, BlockAmt, DebetAmt, CreditAmt, SalesCode, 
										 LeasingCode, StatusFlag, CreateBy, CreateDate, AccountNo, FakturPajakNo, FakturPajakDate)
								values (NewCompany, NewBranch, NewDocNo, NewDocDate, NewProfitCenterCode, NewNettAmt, 
										NewReceiveAmt, NewCustomerCode, NewTOPCode, NewDueDate, NewTypeTrans, NewBlockAmt, 
										NewDebetAmt, NewCreditAmt, NewSalesCode, NewLeasingCode, NewStatusFlag, NewCreateBy, 
										NewCreateDate, NewAccountNo, NewFakturPajakNo, NewFakturPajakDate);'
					execute sp_executesql @sqlString

			 -- MD: Insert/Update data to table glInterface
				-- glInterface - RETURN
				set @SeqNoGLMD = 1
				set @sqlString = 'merge into ' +@DBNameMD+ '..glInterface as gl using ( values(''' 
											   +@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' +@FPJNo+ 
										       ''',''' +convert(varchar,@SeqNoGlMD)+ 
											   ''',''' +convert(varchar,@curDocDate,121)+ 
											   ''',''300'',''' +convert(varchar,@curDocDate,121)+ 
											   ''',''' +@AccNoReturnMD+ ''',''SPAREPART'',''RETURN'',''' 
											   +@FPJNo+ ''',''' +convert(varchar,@TotalNetReturAmt)+ 
											   ''',0,''RETURN'',NULL,NULL,0,''POSTING'',''' 
											   +convert(varchar,@PostingDate,111)+ 
											   ''',''POSTING'',''' +@currentDate+ '''))
						as SRC (NewCompany, NewBranch, NewDocNo, NewSeqNo, NewDocDate, NewProfitCenterCode, 
								NewAccDate, NewAccountNo, NewJournalCode, NewTypeJournal, NewApplyTo, 
								NewAmountDb, NewAmountCr, NewTypeTrans, NewBatchNo, NewBatchDate, 
								NewStatusFlag, NewCreateBy, NewCreateDate, NewLastUpdateBy, NewLastUpdateDate)
						on gl.CompanyCode      = SRC.NewCompany
					   and gl.BranchCode	   = SRC.NewBranch
					   and gl.DocNo			   = SRC.NewDocNo
					   and gl.ProfitCenterCode = SRC.NewProfitCenterCode
					   and gl.TypeTrans        = ''RETURN''
					  when matched 
						   then update set AmountDb = NewAmountDb
					  when not matched by target 
						   then insert (CompanyCode, BranchCode, DocNo, SeqNo, DocDate, ProfitCenterCode, AccDate, AccountNo,
										 JournalCode, TypeJournal, ApplyTo, AmountDb, AmountCr, TypeTrans, BatchNo, BatchDate,
										 StatusFlag, CreateBy, CreateDate, LastUpdateBy, LastUpdateDate)
								values (NewCompany, NewBranch, NewDocNo, NewSeqNo, NewDocDate, NewProfitCenterCode, NewAccDate, 
								        NewAccountNo, NewJournalCode, NewTypeJournal, NewApplyTo, NewAmountDb, NewAmountCr, 
										NewTypeTrans, NewBatchNo, NewBatchDate, NewStatusFlag, NewCreateBy, NewCreateDate, 
										NewLastUpdateBy, NewLastUpdateDate);'
					execute sp_executesql @sqlString

				-- glInterface - TAX OUT
				set @SeqNoGLMD = @SeqNoGlMD + 1
				set @sqlString = 'merge into ' +@DBNameMD+ '..glInterface as gl using ( values(''' 
								+@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' +@FPJNo+ 
										       ''',''' +convert(varchar,@SeqNoGlMD)+ 
											   ''',''' +convert(varchar,@curDocDate,121)+ 
											   ''',''300'',''' +convert(varchar,@curDocDate,121)+ 
											   ''',''' +@AccNoTaxOutMD+ ''',''SPAREPART'',''RETURN'',''' 
											   +@FPJNo+ ''',''' +convert(varchar,@TotalPPnAmt)+ 
											   ''',0,''TAXOUT'',NULL,NULL,0,''POSTING'',''' 
											   +convert(varchar,@PostingDate,111)+ 
											   ''',''POSTING'',''' +@currentDate+ '''))
						as SRC (NewCompany, NewBranch, NewDocNo, NewSeqNo, NewDocDate, NewProfitCenterCode, 
								NewAccDate, NewAccountNo, NewJournalCode, NewTypeJournal, NewApplyTo, 
								NewAmountDb, NewAmountCr, NewTypeTrans, NewBatchNo, NewBatchDate, 
								NewStatusFlag, NewCreateBy, NewCreateDate, NewLastUpdateBy, NewLastUpdateDate)
						on gl.CompanyCode      = SRC.NewCompany
					   and gl.BranchCode	   = SRC.NewBranch
					   and gl.DocNo			   = SRC.NewDocNo
					   and gl.ProfitCenterCode = SRC.NewProfitCenterCode
					   and gl.TypeTrans        = ''TAXOUT''
					  when matched 
						   then update set AmountDb = NewAmountDb
					  when not matched by target 
						   then insert (CompanyCode, BranchCode, DocNo, SeqNo, DocDate, ProfitCenterCode, AccDate, AccountNo,
										 JournalCode, TypeJournal, ApplyTo, AmountDb, AmountCr, TypeTrans, BatchNo, BatchDate,
										 StatusFlag, CreateBy, CreateDate, LastUpdateBy, LastUpdateDate)
								values (NewCompany, NewBranch, NewDocNo, NewSeqNo, NewDocDate, NewProfitCenterCode, NewAccDate, 
								        NewAccountNo, NewJournalCode, NewTypeJournal, NewApplyTo, NewAmountDb, NewAmountCr, 
										NewTypeTrans, NewBatchNo, NewBatchDate, NewStatusFlag, NewCreateBy, NewCreateDate, 
										NewLastUpdateBy, NewLastUpdateDate);'
					execute sp_executesql @sqlString

				-- glInterface - HRETURN
				set @SeqNoGLMD = @SeqNoGlMD + 1
				set @sqlString = 'merge into ' +@DBNameMD+ '..glInterface as gl using ( values(''' 
								+@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' +@FPJNo+ 
										       ''',''' +convert(varchar,@SeqNoGlMD)+ 
											   ''',''' +convert(varchar,@curDocDate,121)+ 
											   ''',''300'',''' +convert(varchar,@curDocDate,121)+ 
											   ''',''' +@AccNoHReturnMD+ ''',''SPAREPART'',''RETURN'',''' 
											   +@FPJNo+ ''',0,''' +convert(varchar,@TotalReturAmt)+ 
											   ''',''HRETURN'',NULL,NULL,0,''POSTING'',''' 
											   +convert(varchar,@PostingDate,111)+ 
											   ''',''POSTING'',''' +@currentDate+ '''))
						as SRC (NewCompany, NewBranch, NewDocNo, NewSeqNo, NewDocDate, NewProfitCenterCode, 
								NewAccDate, NewAccountNo, NewJournalCode, NewTypeJournal, NewApplyTo, 
								NewAmountDb, NewAmountCr, NewTypeTrans, NewBatchNo, NewBatchDate, 
								NewStatusFlag, NewCreateBy, NewCreateDate, NewLastUpdateBy, NewLastUpdateDate)
						on gl.CompanyCode      = SRC.NewCompany
					   and gl.BranchCode	   = SRC.NewBranch
					   and gl.DocNo			   = SRC.NewDocNo
					   and gl.ProfitCenterCode = SRC.NewProfitCenterCode
					   and gl.TypeTrans        = ''HRETURN''
					  when matched 
						   then update set AmountCr = NewAmountCr
					  when not matched by target 
						   then insert (CompanyCode, BranchCode, DocNo, SeqNo, DocDate, ProfitCenterCode, AccDate, AccountNo,
										 JournalCode, TypeJournal, ApplyTo, AmountDb, AmountCr, TypeTrans, BatchNo, BatchDate,
										 StatusFlag, CreateBy, CreateDate, LastUpdateBy, LastUpdateDate)
								values (NewCompany, NewBranch, NewDocNo, NewSeqNo, NewDocDate, NewProfitCenterCode, NewAccDate, 
								        NewAccountNo, NewJournalCode, NewTypeJournal, NewApplyTo, NewAmountDb, NewAmountCr, 
										NewTypeTrans, NewBatchNo, NewBatchDate, NewStatusFlag, NewCreateBy, NewCreateDate, 
										NewLastUpdateBy, NewLastUpdateDate);'
					execute sp_executesql @sqlString

				-- glInterface - DISC1
				if isnull(@DiscFlag,0) = 1
				begin
				set @SeqNoGLMD = @SeqNoGLMD + 1
				set @sqlString = 'merge into ' +@DBNameMD+ '..glInterface as gl using ( values(''' 
								+@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' +@FPJNo+ 
										       ''',''' +convert(varchar,@SeqNoGlMD)+ 
											   ''',''' +convert(varchar,@curDocDate,121)+ 
											   ''',''300'',''' +convert(varchar,@curDocDate,121)+ 
											   ''',''' +@AccNoDisc1MD+ ''',''SPAREPART'',''RETURN'',''' 
											   +@FPJNo+ ''',0,''' +convert(varchar,@DiscAmt)+ 
											   ''',''DISC1'',NULL,NULL,0,''POSTING'',''' 
											   +convert(varchar,@PostingDate,111)+ 
											   ''',''POSTING'',''' +@currentDate+ '''))
						as SRC (NewCompany, NewBranch, NewDocNo, NewSeqNo, NewDocDate, NewProfitCenterCode, 
								NewAccDate, NewAccountNo, NewJournalCode, NewTypeJournal, NewApplyTo, 
								NewAmountDb, NewAmountCr, NewTypeTrans, NewBatchNo, NewBatchDate, 
								NewStatusFlag, NewCreateBy, NewCreateDate, NewLastUpdateBy, NewLastUpdateDate)
						on gl.CompanyCode      = SRC.NewCompany
					   and gl.BranchCode	   = SRC.NewBranch
					   and gl.DocNo			   = SRC.NewDocNo
					   and gl.ProfitCenterCode = SRC.NewProfitCenterCode
					   and gl.TypeTrans        = ''DISC1''
					  when matched 
						   then update set AmountCr = NewAmountCr
					  when not matched by target 
						   then insert (CompanyCode, BranchCode, DocNo, SeqNo, DocDate, ProfitCenterCode, AccDate, AccountNo,
										 JournalCode, TypeJournal, ApplyTo, AmountDb, AmountCr, TypeTrans, BatchNo, BatchDate,
										 StatusFlag, CreateBy, CreateDate, LastUpdateBy, LastUpdateDate)
								values (NewCompany, NewBranch, NewDocNo, NewSeqNo, NewDocDate, NewProfitCenterCode, NewAccDate, 
								        NewAccountNo, NewJournalCode, NewTypeJournal, NewApplyTo, NewAmountDb, NewAmountCr, 
										NewTypeTrans, NewBatchNo, NewBatchDate, NewStatusFlag, NewCreateBy, NewCreateDate, 
										NewLastUpdateBy, NewLastUpdateDate);'
					execute sp_executesql @sqlString
				end

				-- glInterface - INVENTORY
				set @SeqNoGLMD = @SeqNoGlMD + 1
				set @sqlString = 'merge into ' +@DBNameMD+ '..glInterface as gl using ( values(''' 
								+@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' +@FPJNo+ 
										       ''',''' +convert(varchar,@SeqNoGlMD)+ 
											   ''',''' +convert(varchar,@curDocDate,121)+ 
											   ''',''300'',''' +convert(varchar,@curDocDate,121)+ 
											   ''',''' +@AccNoInventoryMD+ ''',''SPAREPART'',''RETURN'',''' 
											   +@FPJNo+ ''',''' +convert(varchar,@CostAmt)+ 
											   ''',0,''INVENTORY'',NULL,NULL,0,''POSTING'',''' 
											   +convert(varchar,@PostingDate,111)+ 
											   ''',''POSTING'',''' +@currentDate+ '''))
						as SRC (NewCompany, NewBranch, NewDocNo, NewSeqNo, NewDocDate, NewProfitCenterCode, 
								NewAccDate, NewAccountNo, NewJournalCode, NewTypeJournal, NewApplyTo, 
								NewAmountDb, NewAmountCr, NewTypeTrans, NewBatchNo, NewBatchDate, 
								NewStatusFlag, NewCreateBy, NewCreateDate, NewLastUpdateBy, NewLastUpdateDate)
						on gl.CompanyCode      = SRC.NewCompany
					   and gl.BranchCode	   = SRC.NewBranch
					   and gl.DocNo			   = SRC.NewDocNo
					   and gl.ProfitCenterCode = SRC.NewProfitCenterCode
					   and gl.TypeTrans        = ''INVENTORY''
					  when matched 
						   then update set AmountDb = AmountDb + NewAmountDb
					  when not matched by target 
						   then insert (CompanyCode, BranchCode, DocNo, SeqNo, DocDate, ProfitCenterCode, AccDate, AccountNo,
										 JournalCode, TypeJournal, ApplyTo, AmountDb, AmountCr, TypeTrans, BatchNo, BatchDate,
										 StatusFlag, CreateBy, CreateDate, LastUpdateBy, LastUpdateDate)
								values (NewCompany, NewBranch, NewDocNo, NewSeqNo, NewDocDate, NewProfitCenterCode, NewAccDate, 
								        NewAccountNo, NewJournalCode, NewTypeJournal, NewApplyTo, NewAmountDb, NewAmountCr, 
										NewTypeTrans, NewBatchNo, NewBatchDate, NewStatusFlag, NewCreateBy, NewCreateDate, 
										NewLastUpdateBy, NewLastUpdateDate);'
					execute sp_executesql @sqlString

				-- glInterface - COGS
				set @SeqNoGLMD = @SeqNoGLMD + 1
				set @sqlString = 'merge into ' +@DBNameMD+ '..glInterface as gl using ( values(''' 
								+@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' +@FPJNo+ 
										       ''',''' +convert(varchar,@SeqNoGlMD)+ 
											   ''',''' +convert(varchar,@curDocDate,121)+ 
											   ''',''300'',''' +convert(varchar,@curDocDate,121)+ 
											   ''',''' +@AccNoCogsMD+ ''',''SPAREPART'',''RETURN'',''' 
											   +@FPJNo+ ''',0,''' +convert(varchar,@CostAmt)+ 
											   ''',''COGS'',NULL,NULL,0,''POSTING'',''' 
											   +convert(varchar,@PostingDate,111)+ 
											   ''',''POSTING'',''' +@currentDate+ '''))
						as SRC (NewCompany, NewBranch, NewDocNo, NewSeqNo, NewDocDate, NewProfitCenterCode, 
								NewAccDate, NewAccountNo, NewJournalCode, NewTypeJournal, NewApplyTo, 
								NewAmountDb, NewAmountCr, NewTypeTrans, NewBatchNo, NewBatchDate, 
								NewStatusFlag, NewCreateBy, NewCreateDate, NewLastUpdateBy, NewLastUpdateDate)
						on gl.CompanyCode      = SRC.NewCompany
					   and gl.BranchCode	   = SRC.NewBranch
					   and gl.DocNo			   = SRC.NewDocNo
					   and gl.ProfitCenterCode = SRC.NewProfitCenterCode
					   and gl.TypeTrans        = ''COGS''
					  when matched 
						   then update set AmountCr = AmountCr + NewAmountCr
					  when not matched by target 
						   then insert (CompanyCode, BranchCode, DocNo, SeqNo, DocDate, ProfitCenterCode, AccDate, AccountNo,
										 JournalCode, TypeJournal, ApplyTo, AmountDb, AmountCr, TypeTrans, BatchNo, BatchDate,
										 StatusFlag, CreateBy, CreateDate, LastUpdateBy, LastUpdateDate)
								values (NewCompany, NewBranch, NewDocNo, NewSeqNo, NewDocDate, NewProfitCenterCode, NewAccDate, 
								        NewAccountNo, NewJournalCode, NewTypeJournal, NewApplyTo, NewAmountDb, NewAmountCr, 
										NewTypeTrans, NewBatchNo, NewBatchDate, NewStatusFlag, NewCreateBy, NewCreateDate, 
										NewLastUpdateBy, NewLastUpdateDate);'
					execute sp_executesql @sqlString

			 -- Update Daily Posting Process Status
				update svSDMovement
				   set ProcessStatus=1
				     , ProcessDate  =@CurrentDate
					where CompanyCode=@curCompanyCode
					  and BranchCode =@curBranchCode
					  and DocNo      =@curDocNo
					  and PartNo     =@curPartNo
					  and PartSeq    =@curPartSeq

			 -- Read next record
				fetch next from cursvSDMovement
					into @curCompanyCode, @curBranchCode, @curDocNo, @curDocDate, @curPartNo, @curPartSeq, @curWarehouseCode, @curQtyOrder, 
						 @curQty, @curDiscPct, @curCostPrice, @curRetailPrice, @curTypeOfGoods, @curCompanyMD, @curBranchMD, @curWarehouseMD, 
						 @curRetailPriceInclTaxMD, @curRetailPriceMD, @curCostPriceMD, @curQtyFlag, @curProductType, @curProfitCenterCode, 
						 @curStatus, @curProcessStatus, @curProcessDate, @curCreatedBy, @curCreatedDate, @curLastUpdateBy, @curLastUpdateDate
			end 

	 -- Move data which already processed from table svSDMovement to table svHstSDMovement
	    if not exists (select * from sys.objects where object_id = object_id(N'[dbo].[svHstSDMovement]') and type in (N'U'))
			CREATE TABLE [dbo].[svHstSDMovement](
				[CompanyCode] [varchar](15) NOT NULL,
				[BranchCode] [varchar](15) NOT NULL,
				[DocNo] [varchar](15) NOT NULL,
				[DocDate] [datetime] NOT NULL,
				[PartNo] [varchar](20) NOT NULL,
				[PartSeq] [int] NOT NULL,
				[WarehouseCode] [varchar](15) NOT NULL,
				[QtyOrder] [numeric](18, 2) NOT NULL,
				[Qty] [numeric](18, 2) NOT NULL,
				[DiscPct] [numeric](5, 2) NOT NULL,
				[CostPrice] [numeric](18, 2) NOT NULL,
				[RetailPrice] [numeric](18, 2) NOT NULL,
				[TypeOfGoods] [varchar](5) NOT NULL,
				[CompanyMD] [varchar](15) NOT NULL,
				[BranchMD] [varchar](15) NOT NULL,
				[WarehouseMD] [varchar](15) NOT NULL,
				[RetailPriceInclTaxMD] [numeric](18, 2) NOT NULL,
				[RetailPriceMD] [numeric](18, 2) NOT NULL,
				[CostPriceMD] [numeric](18, 2) NOT NULL,
				[QtyFlag] [char](1) NOT NULL,
				[ProductType] [varchar](15) NOT NULL,
				[ProfitCenterCode] [varchar](15) NOT NULL,
				[Status] [char](1) NOT NULL,
				[ProcessStatus] [char](1) NOT NULL,
				[ProcessDate] [datetime] NOT NULL,
				[CreatedBy] [varchar](15) NOT NULL,
				[CreatedDate] [datetime] NOT NULL,
				[LastUpdateBy] [varchar](15) NULL,
				[LastUpdateDate] [datetime] NULL)

		insert into svHstSDMovement (CompanyCode, BranchCode, DocNo, DocDate, PartNo, PartSeq, WarehouseCode,
								     QtyOrder, Qty, DiscPct, CostPrice, RetailPrice, TypeOfGoods, CompanyMD, 
									 BranchMD, WarehouseMD, RetailPriceInclTaxMD, RetailPriceMD, CostPriceMD,
									 QtyFlag, ProductType, ProfitCenterCode, Status, ProcessStatus, ProcessDate,
									 CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
							 select  CompanyCode, BranchCode, DocNo, DocDate, PartNo, PartSeq, WarehouseCode,
								     QtyOrder, Qty, DiscPct, CostPrice, RetailPrice, TypeOfGoods, CompanyMD, 
									 BranchMD, WarehouseMD, RetailPriceInclTaxMD, RetailPriceMD, CostPriceMD,
									 QtyFlag, ProductType, ProfitCenterCode, Status, ProcessStatus, ProcessDate,
									 CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate
							   from  svSDMovement
							  where	 ProcessStatus = 1
							     or  (left(docno,3) in ('RTR','RTN')
                                and  convert(varchar,DocDate,111)<=convert(varchar,@PostingDate,111))

		delete svSDMovement   where	 ProcessStatus = 1
							     or  (left(docno,3) in ('RTR','RTN')
                                and  convert(varchar,DocDate,111)<=convert(varchar,@PostingDate,111))

		close cursvSDMovement
		deallocate cursvSDMovement
END

--END TRY

--BEGIN CATCH
--    --select ERROR_NUMBER()    AS ErrorNumber,    ERROR_SEVERITY() AS ErrorSeverity, ERROR_STATE()   AS ErrorState,
--		  -- ERROR_PROCEDURE() AS ErrorProcedure, ERROR_LINE()     AS ErrorLine    , ERROR_MESSAGE() AS ErrorMessage
--	if @@TRANCOUNT > 0
--		begin
--			select '0' [STATUS], 'Posting gagal !!!   ' + ERROR_MESSAGE() [INFO]
--			select '0' [STATUS], 'Posting gagal !!!   ' + ERROR_PROCEDURE() [INFO]
--			rollback transaction
--			return
--		end
--END CATCH

--IF @@TRANCOUNT > 0
--	begin
--		select '1' [STATUS], 'Posting berhasil !!!' [INFO]
		--rollback transaction
		--commit transaction
	--end

GO
if object_id('usprpt_PostingMultiCompanySales') is not null
	drop procedure usprpt_PostingMultiCompanySales
GO
-- POSTING TRANSACTION MULTI COMPANY - SALES
-- -----------------------------------------------------------------------
-- Created  by : HTO, 2014
-- Revision-01 : HTO, January 2015
-- Revision-02 : HTO, April 2015 (Process by Date)
-- -----------------------------------------------------------------------
-- This procedure used for 
--		4-Wheeler : BIT-SBT, SBSM-SSBT, BAT-SAT, SIT-SST, UIB-SIT
--		2-Wheeler : IJMG,SVMG,ISG,ISMG-SMG
-- -----------------------------------------------------------------------
-- execute [usprpt_PostingMultiCompanySales] '6006400001','2014/11/08','0'
-- -----------------------------------------------------------------------

CREATE procedure [dbo].[usprpt_PostingMultiCompanySales]
	@CompanyCode	varchar(15),
	@PostingDate	datetime,
	@Status			varchar(1)	output
AS	

--BEGIN TRANSACTION
--BEGIN TRY

BEGIN 
		--declare @CompanyCode			varchar(15) = (select top 1 CompanyCode from gnMstOrganizationHdr)
		--declare @PostingDate			datetime    = '2015/04/30' --getdate()
		--declare @Status				varchar(1)  = '0'

  -- Check Tax/Seri Pajak online
		declare @TaxCompany				varchar(15)
		declare @TaxBranch				varchar(15)
		declare @TaxDB					varchar(50)
		declare @TaxTransCode			varchar(3)
		declare @swTaxBranch			varchar(15) = ''
		declare @swTaxDoc				varchar(15) = ''
		declare @swTaxDocDate			varchar(10) = ''
		declare @swCompanyCode			varchar(15) = ' '
		declare @swBranchCode			varchar(15) = ' '
		declare @swDocNo 				varchar(15) = ' '
		declare @swDocDate				varchar(10) = ''
		declare @TaxSeq					bigint
		declare @TaxSeqNo				int
		declare @SeriPajakNo			varchar(50) = ''
		declare @sqlString				nvarchar(max)

		set @TaxCompany=isnull((select ParaValue from gnMstLookupDtl where CodeID='TXOL' and LookupValue='COMPANYCODE'),'')
		set @TaxBranch =isnull((select ParaValue from gnMstLookupDtl where CodeID='TXOL' and LookupValue='BRANCHCODE'),'')
		set @TaxDB     =isnull((select ParaValue from gnMstLookupDtl where CodeID='TXOL' and LookupValue='FROM_DB'),'')

		if @TaxCompany='' or @TaxBranch='' or @TaxDB=''
			begin
				set @Status = '1'
				rollback commit
				return
			end
		
		set @sqlString = N'select top 1 @TaxSeq=FPJSeqNo, @TaxSeqNo=SeqNo from ' 
									   +@TaxDb+ '..gnMstFPJSeqNo where CompanyCode=''' 
									   +@TaxCompany+ ''' and BranchCode=''' 
									   +@TaxBranch+ ''' and Year=''' 
									   +convert(varchar,year(@PostingDate))+ ''' and isActive=1 and convert(varchar,EffectiveDate,111)<=''' 
									   +convert(varchar,@PostingDate,111)+ ''' order by CompanyCode, BranchCode, Year, SeqNo'
			execute sp_executesql @sqlString, 
								N'@TaxSeq 		bigint  	output,
								  @TaxSeqNo 	int 		output', 
								  @TaxSeq 		output,
								  @TaxSeqNo 	output

		if isnull(@TaxSeq,0)=0 and isnull(@TaxSeqNo,0)=0
			begin
				set @Status = '1'
				return
			end
		set @TaxSeq   = isnull(@TaxSeq,0)
		set @TaxSeqNo = isnull(@TaxSeqNo,0)

 -- Posting process : insert data to MD & SD table
		declare @curCompanyCode			varchar(15) --= '6006400001'
		declare @curBranchCode			varchar(15) --= '6006400131'
		declare @curDocNo				varchar(15)	--= 'IU131/14/100010'
		declare @curDocDate				datetime    --= '2015/03/08'
		declare @curSeq					int			
		declare @curSalesModelCode		varchar(20)	
		declare @curSalesModelYear		numeric(4,0)	
		declare @curChassisCode			varchar(15)	
		declare @curChassisNo			numeric(10,0) 
		declare @curEngineCode			varchar(15)	
		declare @curEngineNo			numeric(10,0) 
		declare @curColourCode			varchar(15)	
		declare @curWarehouseCode		varchar(15)	
		declare @curCustomerCode		varchar(15)	
		declare @curQtyFlag				char(1)		
		declare @curCompanyMD			varchar(15)
		declare @curBranchMD			varchar(15)	
		declare @curUnitBranchMD		varchar(15)	
		declare @curWarehouseMD			varchar(15)	
		declare @curStatus				char(1)		
		declare @curProcessStatus		char(1)		
		declare @curProcessDate			datetime		
		declare @curCreatedBy			varchar(15)	
		declare @curCreatedDate			datetime		
		declare @curLastUpdateBy		varchar(15)	
		declare @curLastUpdateDate		datetime

		declare @DBName					varchar(50)
		declare @DBNameMD				varchar(50)
		declare @CentralBranch			varchar(15)
		declare @PONo					varchar(15)
		declare @SONo					varchar(15)
		declare @DONo					varchar(15)
		declare @BPKNo					varchar(15)
		declare @INVNo					varchar(15)
		declare @BPUNo					varchar(15)
		declare @HPPNo					varchar(15)
		declare @RTPNo					varchar(15) = ''
		declare @RTNNo					varchar(15) = ''
		declare @VTONo					varchar(15) = ''
		declare @VTINo					varchar(15) = ''
		declare @WHFrom					varchar(15) = ''
		declare @WHTo					varchar(15) = ''
		declare @VehBPUDate				datetime    = '1900/01/01'
		declare @DueDate				datetime
		declare @SeqNo					integer
		declare @VTSeq					integer

		declare @VehServiceBookNo		varchar(15)
		declare @VehKeyNo				varchar(15)
		declare @VehWH					varchar(15) = 'HLD'
		declare @VehCOGS				numeric(18,0)
		declare @VehCOGSOthers			numeric(18,0)
		declare @VehCOGSKaroseri		numeric(18,0)
		declare @VehPpnBmBuyPaid		numeric(18,0)
		declare @VehPpnBmBuy			numeric(18,0)
		declare @VehSalesNetAmt			numeric(18,0)
		declare @VehPpnBmSellPaid		numeric(18,0)
		declare @VehPpnBmSell			numeric(18,0)
		declare @VehWHMD				varchar(15)
		declare @VehFakturPolisiNo		varchar(15)
		declare @VehFakturPolisiDate	datetime
		declare @VehSONo				varchar(15)
		declare @VehDONo				varchar(15)
		declare @VehBPKNo				varchar(15)
		declare @VehINVNo				varchar(15)
		declare @VehREQNo				varchar(15)
		declare @VehRTNNo				varchar(15)
		declare @VehVTONo				varchar(15)
		declare @VehVTINo				varchar(15)
		declare @VehBPKDate				datetime
		declare @VehSuzukiDONo			varchar(15)
		declare @VehSuzukiDODate		datetime
		declare @VehSuzukiSJNo			varchar(15)
		declare @VehSuzukiSJDate		datetime
		declare @CurrentDate			varchar(100) = convert(varchar,getdate(),121)

		declare @RetailPriceIncludePPN	numeric(18,0) = 0
		declare @DiscPriceIncludePPN	numeric(18,0) = 0
		declare @NetSalesIncludePPN		numeric(18,0) = 0
		declare @RetailPriceExcludePPN	numeric(18,0) = 0
		declare @DiscPriceExcludePPN	numeric(18,0) = 0
		declare @NetSalesExcludePPN		numeric(18,0) = 0
		declare @PPNBeforeDisc			numeric(18,0) = 0
		declare @PPNAfterDisc			numeric(18,0) = 0
		declare @PPnBMPaid				numeric(18,0) = 0
		declare @OthersDPP				numeric(18,0) = 0
		declare @OthersPPn				numeric(18,0) = 0

		declare @EndUserName			varchar(100)
		declare @EndUserAddress1		varchar(100)
		declare @EndUserAddress2		varchar(100)
		declare @EndUserAddress3		varchar(100)
		declare @CityCode				varchar(15)
		declare @CustomerClass			varchar(15)
		declare @TopCode				varchar(15) 
		declare @GroupPriceCode			varchar(15)
		declare @SalesCode				varchar(15) 
		declare @SalesType				char(1)	
		declare @TopDays				integer
		declare @WHSD					varchar(15)

        declare curomSDMovement cursor for
			select sd.* 
			  from omSDMovement sd--, gnMstDocument doc
      --       where sd.CompanyMD        =doc.CompanyCode
			   --and sd.BranchMD         =doc.BranchCode
			   --and doc.DocumentType    ='IVU' 
			   --and doc.ProfitCenterCode='100'
			   --and left(sd.DocNo,len(doc.DocumentPrefix))=doc.DocumentPrefix
             where left(DocNo,2) in ('IV','IU') 
			   and convert(varchar,DocDate,111)<=convert(varchar,@PostingDate,111)
			   and ProcessStatus=0
             order by convert(varchar,sd.DocDate,111), sd.CompanyCode, sd.BranchCode, 
					  sd.DocNo, sd.SalesModelCode, sd.SalesModelYear, sd.ColourCode
		open curomSDMovement

		fetch next from curomSDMovement
			  into @curCompanyCode, @curBranchCode, @curDocNo, @curDocDate, @curSeq, @curSalesModelCode,
				   @curSalesModelYear, @curChassisCode, @curChassisNo, @curEngineCode, @curEngineNo, 
				   @curColourCode, @curWarehouseCode, @curCustomerCode, @curQtyFlag, @curCompanyMD, 
				   @curBranchMD, @curWarehouseMD, @curStatus, @curProcessStatus, @curProcessDate, 
				   @curCreatedBy, @curCreatedDate, @curLastUpdateBy, @curLastUpdateDate

		while (@@FETCH_STATUS =0)
			begin

			-- MD Database & SD Database from gnMstCompanyMapping
				select @DBNameMD=DBMD, @DBName=DBName, @curUnitBranchMD=UnitBranchMD  from gnMstCompanyMapping where CompanyCode=@CurCompanyCode and BranchCode=@curBranchCode

			 -- Centralize unit purchasing for SBT, SMG, SIT, SST dealer
				set @sqlString = N'select @CentralBranch=BranchCode from ' +@DBName+ '..gnMstOrganizationDtl ' +  
								   'where CompanyCode=''' +@curCompanyCode+ ''' and isBranch=''0'''
					execute sp_executesql @sqlString, 
							N'@CentralBranch varchar(15) output', @CentralBranch output

			 -- MD: Collect vehicle information from omMstVehicle (Main Dealer)
				set @sqlString = N'select @VehServiceBookNo=ServiceBookNo, @VehKeyNo=KeyNo, @VehCOGS=COGSUnit, 
								 @VehCOGSOthers=COGSOthers, @VehCOGSKaroseri=COGSKaroseri, @VehPpnBmBuyPaid=PpnBmBuyPaid, 
								 @VehPpnBmBuy=PpnBmBuy, @VehSalesNetAmt=SalesNetAmt, @VehPpnBmSellPaid=PpnBmSellPaid, 
								 @VehPpnBmSell=PpnBmSell, @VehWHMD=WarehouseCode, @VehFakturPolisiNo=FakturPolisiNo, 
								 @VehFakturPolisiDate=FakturPolisiDate, @VehSONo=SONo, @VehDONo=DONo, @VehBPKNo=BPKNo, 
								 @VehINVNo=InvoiceNo, @VehREQNo=ReqOutNo, @VehRTNNo=SOReturnNo, @VehVTONo=TransferOutNo, 
								 @VehVTINo=TransferInNo, @VehBPKDate=BPKDate, @VehSuzukiDONo=SuzukiDONo, 
								 @VehSuzukiDODate=SuzukiDODate, @VehSuzukiSJNo=SuzukiSJNo, 
								 @VehSuzukiSJDate=SuzukiSJDate from ' +@DBNameMD+ '..omMstVehicle where CompanyCode=''' 
								 +@curCompanyMD+ ''' and ChassisCode=''' +@curChassisCode+ 
								 ''' and ChassisNo=''' +convert(varchar,@curChassisNo)+ ''''
					execute sp_executesql @sqlString, 
							N'@VehServiceBookNo		varchar(15)		output,
							  @VehKeyNo				varchar(15)		output,
							  @VehCOGS				numeric(18,0)	output,
							  @VehCOGSOthers		numeric(18,0)	output,
							  @VehCOGSKaroseri		numeric(18,0)	output,
							  @VehPpnBmBuyPaid		numeric(18,0)	output,
							  @VehPpnBmBuy			numeric(18,0)	output,
							  @VehSalesNetAmt		numeric(18,0)	output,
							  @VehPpnBmSellPaid		numeric(18,0)	output, 
							  @VehPpnBmSell			numeric(18,0)	output,
							  @VehWHMD				varchar(15)		output,
							  @VehFakturPolisiNo	varchar(15)		output,
							  @VehFakturPolisiDate	datetime		output,
							  @VehSONo				varchar(15)		output,
							  @VehDONo				varchar(15)		output,
							  @VehBPKNo				varchar(15)		output,
							  @VehINVNo				varchar(15)		output,
							  @VehREQNo				varchar(15)		output,
							  @VehRTNNo				varchar(15)		output,
							  @VehVTONo				varchar(15)		output,
							  @VehVTINo				varchar(15)		output,
							  @VehBPKDate			datetime		output,
							  @VehSuzukiDONo		varchar(15)		output,
							  @VehSuzukiDODate		datetime		output,
							  @VehSuzukiSJNo		varchar(15)		output,
							  @VehSuzukiSJDate		datetime		output',
							  @VehServiceBookNo		output,  @VehKeyNo			output,
							  @VehCOGS				output,	 @VehCOGSOthers		output,
							  @VehCOGSKaroseri		output,	 @VehPpnBmBuyPaid	output,
							  @VehPpnBmBuy			output,  @VehSalesNetAmt	output,
							  @VehPpnBmSellPaid		output,  @VehPpnBmSell		output,
							  @VehWHMD				output,  @VehFakturPolisiNo	output,  
							  @VehFakturPolisiDate	output,  @VehSONo			output,  
							  @VehDONo				output,  @VehBPKNo			output,  
							  @VehINVNo				output,  @VehREQNo			output,
							  @VehRTNNo				output,  @VehVTONo			output,  
							  @VehVTINo				output,  @VehBPKDate		output,  
							  @VehSuzukiDONo		output,  @VehSuzukiDODate	output,  
							  @VehSuzukiSJNo		output,  @VehSuzukiSJDate	output

--print @sqlString
--select @VehServiceBookNo ServiceBookNo, @VehKeyNo KeyNo, @VehCOGS COGSUnit, @VehCOGSOthers COGSOthers, @VehCOGSKaroseri COGSKaroseri, 
--       @VehPpnBmBuyPaid PpnBmBuyPaid, @VehPpnBmBuy PpnBmBuy, @VehSalesNetAmt SalesNetAmt, @VehPpnBmSellPaid PpnBmSellPaid, 
--	   @VehPpnBmSell PpnBmSell, @VehWHMD WarehouseCode, @VehFakturPolisiNo FakturPolisiNo, @VehFakturPolisiDate FakturPolisiDate, 
--	   @VehSONo SONo, @VehDONo DONo, @VehBPKNo BPKNo, @VehINVNo InvoiceNo, @VehREQNo ReqOutNo, @VehRTNNo SOReturnNo, 
--	   @VehVTONo TransferOutNo, @VehVTINo TransferInNo, @VehBPKDate BPKDate, @VehSuzukiDONo SuzukiDONo, @VehSuzukiDODate SuzukiDODate, 
--	   @VehSuzukiSJNo SuzukiSJNo, @VehSuzukiSJDate SuzukiSJDate, @DBNameMD DBNameMD, @curCompanyMD CompanyMD, 
--	   @curChassisCode ChassisCode, @curChassisNo curChassisNo

				set @VehServiceBookNo	 = isnull(@VehServiceBookNo,'')
				set @VehKeyNo			 = isnull(@VehKeyNo,'')
				set @VehCOGS			 = isnull(@VehCOGS,0)
				set @VehCOGSOthers		 = isnull(@VehCOGSOthers,0) 
				set @VehCOGSKaroseri	 = isnull(@VehCOGSKaroseri,0)
				set @VehPpnBmBuyPaid	 = isnull(@VehPpnBmBuyPaid,0)
				set @VehPpnBmBuy		 = isnull(@VehPpnBmBuy,0) 
				set @VehSalesNetAmt		 = isnull(@VehSalesNetAmt,0) 
				set @VehPpnBmSellPaid    = isnull(@VehPpnBmSellPaid,0)
				set @VehPpnBmSell		 = isnull(@VehPpnBmSell,0)
				set @VehWHMD			 = isnull(@VehWHMD,'')
				set @VehFakturPolisiNo	 = isnull(@VehFakturPolisiNo,'')
				set @VehFakturPolisiDate = isnull(@VehFakturPolisiDate,'1900/01/01')
				set @VehSONo			 = isnull(@VehSONo,'')
				set @VehDONo			 = isnull(@VehDONo,'') 
				set @VehBPKNo			 = isnull(@VehBPKNo,'')
				set @VehINVNo			 = isnull(@VehINVNo,'') 
				set @VehREQNo			 = isnull(@VehREQNo,'') 
				set @VehRTNNo			 = isnull(@VehRTNNo,'')
				set @VehVTONo			 = isnull(@VehVTONo,'') 
				set @VehVTINo			 = isnull(@VehVTINo,'') 
				set @VehBPKDate			 = isnull(@VehBPKDate,'1900/01/01')
				set @VehSuzukiDONo		 = isnull(@VehSuzukiDONo,'') 
				set @VehSuzukiDODate	 = isnull(@VehSuzukiDODate,'1900/01/01')
				set @VehSuzukiSJNo		 = isnull(@VehSuzukiSJNo,'')
				set @VehSuzukiSJDate	 = isnull(@VehSuzukiSJDate,'1900/01/01')

				-- Collect Vehicle Price from omPriceListBranches
				set @GroupPriceCode = 'W' + substring(convert(varchar,@curDocDate,112),3,4)
			    set @sqlString = N'select top 1 @RetailPriceIncludePPN=RetailPriceIncludePPN, @DiscPriceIncludePPN=DiscPriceIncludePPN,
												@NetSalesIncludePPN=NetSalesIncludePPN, @RetailPriceExcludePPN=RetailPriceExcludePPN,
												@DiscPriceExcludePPN=DiscPriceExcludePPN, @NetSalesExcludePPN=NetSalesExcludePPN,
												@PPNBeforeDisc=PPNBeforeDisc, @PPNAfterDisc=PPNAfterDisc, @PPnBMPaid=PPnBMPaid,
												@OthersDPP=OthersDPP, @OthersPPn=OthersPPn from '
												+@DBNameMD+ '..omPriceListBranches where CompanyCode=''' +@curCompanyMD+ 
												''' and BranchCode=''' +@curUnitBranchMD+ ''' and SupplierCode=''' +@curUnitBranchMD+ 
												''' and GroupPrice=''' +@GroupPriceCode+ ''' and SalesModelCode=''' +@curSalesModelCode+
												''' and SalesModelYear=''' +convert(varchar,@curSalesModelYear)+ 
												''' and EffectiveDate<='''+convert(varchar,@curDocDate,111)+ 
												''' and isStatus=1 order by EffectiveDate desc'
						execute sp_executesql @sqlString, 
								N'@RetailPriceIncludePPN 	numeric(18,0)	output, 
								  @DiscPriceIncludePPN 		numeric(18,0)	output,
								  @NetSalesIncludePPN 		numeric(18,0)	output, 
								  @RetailPriceExcludePPN 	numeric(18,0)	output, 
								  @DiscPriceExcludePPN 		numeric(18,0)	output, 
								  @NetSalesExcludePPN 		numeric(18,0)	output, 
								  @PPNBeforeDisc 			numeric(18,0)	output, 
								  @PPNAfterDisc 			numeric(18,0)	output,
								  @PPnBMPaid 				numeric(18,0)	output,
								  @OthersDPP				numeric(18,0)	output,
								  @OthersPPn				numeric(18,0)	output', 
								  @RetailPriceIncludePPN	output, 
								  @DiscPriceIncludePPN		output,
								  @NetSalesIncludePPN 		output, 
								  @RetailPriceExcludePPN	output, 
								  @DiscPriceExcludePPN		output, 
								  @NetSalesExcludePPN		output, 
								  @PPNBeforeDisc			output, 
								  @PPNAfterDisc				output,
								  @PPnBMPaid				output,
								  @OthersDPP				output,
								  @OthersPPn				output

				set @RetailPriceIncludePPN = isnull(@RetailPriceIncludePPN,0)
				set @DiscPriceIncludePPN   = isnull(@DiscPriceIncludePPN,0)
				set @NetSalesIncludePPN    = isnull(@NetSalesIncludePPN,0)
				set @RetailPriceExcludePPN = isnull(@RetailPriceExcludePPN,0)
				set @DiscPriceExcludePPN   = isnull(@DiscPriceExcludePPN,0)
				set @NetSalesExcludePPN	   = isnull(@NetSalesExcludePPN,0)
				set @PPNBeforeDisc		   = isnull(@PPNBeforeDisc,0)
				set @PPNAfterDisc		   = isnull(@PPNAfterDisc,0)
				set @PPnBMPaid			   = isnull(@PPnBMPaid,0)
				set @OthersDPP			   = isnull(@OthersDPP,0)
				set @OthersPPn			   = isnull(@OthersPPn,0)

				if @swDocDate     <> convert(varchar,@curDocDate,111) or
				   @swCompanyCode <> @curCompanyCode or
				   @swBranchCode  <> @curBranchCode  or
				   @swDocNo		  <> @curDocNo
					begin
						set @swDocDate     = convert(varchar,@curDocDate,111)
						set @swCompanyCode = @curCompanyCode
						set @swBranchCode  = @curBranchCode
						set @swDocNo	   = @curDocNo
						set @SeqNo		   = 0
						set @VTSeq		   = 0
		
					 -- MD: Customer information in gnMstCustomer & gnMstCustomerProfitCenter
						set @sqlString = N'select @EndUserName=CustomerName, @EndUserAddress1=Address1, @EndUserAddress2=Address2, 
										 @EndUserAddress3=Address3, @CityCode=CityCode from ' 
										 +@DBNameMD+ '..gnMstCustomer where CompanyCode='''
										 +@curCompanyMD+ ''' and CustomerCode=''' +@CentralBranch+ ''''
							execute sp_executesql @sqlString, 
									N'@EndUserName		varchar(100) output,  @EndUserAddress1	varchar(100) output,
									  @EndUserAddress2	varchar(100) output,  @EndUserAddress3	varchar(100) output,
									  @CityCode			varchar(10)  output', 
									  @EndUserName		output, @EndUserAddress1	output, @EndUserAddress2 output, 
									  @EndUserAddress3	output, @CityCode           output

					  --set @sqlString = N'select @CustomerClass=CustomerClass, @TaxTransCode=TaxTransCode, @TopCode=TopCode, 
					  --				 @GroupPriceCode=GroupPriceCode, @SalesCode=SalesCode, @SalesType=SalesType from ' 
					  --				 +@DBNameMD+ '..gnMstCustomerProfitCenter where CompanyCode='''
					  --				 +@curCompanyMD+ ''' and BranchCode=''' +@curBranchMD+ ''' and CustomerCode='''
					  --				 +@CentralBranch+ ''' and ProfitCenterCode=''100'''
					  --	execute sp_executesql @sqlString, 
					  --			N'@CustomerClass	varchar(15) output,  @TaxTransCode		varchar(3)  output,
					  --			  @TopCode			varchar(15) output,  @GroupPriceCode	varchar(15) output,
					  --			  @SalesCode		varchar(15) output,  @SalesType		char(1)		output', 
					  --			  @CustomerClass  output, @TaxTransCode output, @TopCode   output, 
					  --			  @GroupPriceCode output, @SalesCode    output, @SalesType output
						set @sqlString = N'select @CustomerClass=CustomerClass, @TaxTransCode=TaxTransCode, @TopCode=TopCode, 
										 @SalesCode=SalesCode, @SalesType=SalesType from ' +@DBNameMD+ 
										 '..gnMstCustomerProfitCenter where CompanyCode=''' +@curCompanyMD+ 
										 ''' and BranchCode=''' +@curUnitBranchMD+ ''' and CustomerCode='''
										 +@CentralBranch+ ''' and ProfitCenterCode=''100'''
							execute sp_executesql @sqlString, 
									N'@CustomerClass	varchar(15) output,  @TaxTransCode	varchar(3)  output,
									  @TopCode			varchar(15) output,  @SalesCode		varchar(15) output,  
									  @SalesType		char(1)		output', 
									  @CustomerClass  	output, 	@TaxTransCode 	output, 	@TopCode	output, 
									  @SalesCode    	output, 	@SalesType 		output

					 -- MD: Calculate Top of Days 
						set @sqlString = N'select @TopDays=ParaValue from ' +@DBNameMD+ '..gnMstLookUpDtl 
											where CompanyCode=''' +@curCompanyMD+ ''' and CodeID=''TOPC'' and LookUpValue=''' +@TopCode+ ''''
							execute sp_executesql @sqlString, N'@TopDays integer output', @TopDays output

						set @EndUserName     = isnull(@EndUserName,'')
						set @EndUserAddress1 = isnull(@EndUserAddress1,'')
						set @EndUserAddress2 = isnull(@EndUserAddress2,'')
						set @EndUserAddress3 = isnull(@EndUserAddress3,'')
						set @CityCode        = isnull(@CityCode,'')
						set @CustomerClass   = isnull(@CustomerClass,'')
						set @TaxTransCode    = isnull(@TaxTransCode,'')
						set @TopCode         = isnull(@TopCode,'')
						set @SalesCode       = isnull(@SalesCode,'')
						set @SalesType	  	 = isnull(@SalesType,'')
						set @SalesType		 = isnull(@TopDays,0)
						set @DueDate		 = dateadd(day,isnull(@TopDays,0),@curDocDate)

					 -- MD: Generate Sales Order No for MD
						execute [usprpt_PostingMultiCompany4DocNo] @curCompanyMD, @curUnitBranchMD, @DBNameMD, 'SOR', @SONo output

					 -- SD: Insert data to table omTrPurchasePO
						execute [usprpt_PostingMultiCompany4DocNo] @curCompanyCode, @CentralBranch, @DBName, 'PUR', @PONo output
						set @sqlString = 'insert into ' +@DBName+ '..omTrPurchasePO 
												(CompanyCode, BranchCode, PONo, PODate, RefferenceNo, RefferenceDate, SupplierCode, 
												 BillTo, ShipTo, Remark, Status, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate, 
												 isLocked, LockingBy, LockingDate)
											values(''' +@curCompanyCode+ ''',''' +@CentralBranch+ ''',''' +@PONo+ ''',''' 
										           +convert(varchar,@curDocDate,121)+ ''',''' +@SONo+ ''',''' 
												   +convert(varchar,@curDocDate,121)+ ''',''' +@curUnitBranchMD+ ''',''' 
												   +@CentralBranch+ ''',''' +@CentralBranch+ ''',''' +@curDocNo+ 
												   ''',''2'',''POSTING'',''' +convert(varchar,@PostingDate,111)+ 
												   ''',''POSTING'',''' +@CurrentDate+ ''',0,NULL,NULL)'
							execute sp_executesql @sqlString

					 -- MD: Insert data to table omTrSalesSO
						set @sqlString = 'insert into ' +@DBNameMD+ '..omTrSalesSO 
												(CompanyCode, BranchCode, SONo, SODate, SalesType, RefferenceNo, RefferenceDate, CustomerCode, 
												 TOPCode, TOPDays, BillTo, ShipTo, ProspectNo, SKPKNo, Salesman, WareHouseCode, isLeasing, 
												 LeasingCo, GroupPriceCode, Insurance, PaymentType, PrePaymentAmt, PrePaymentDate, 
												 CommissionBy, CommissionAmt, PONo, ContractNo, RequestDate, Remark, Status, ApproveBy,
												 ApproveDate, RejectBy, RejectDate, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate, 
												 isLocked, LockingBy, LockingDate, SalesCode, Installment, FinalPaymentDate, SalesCoordinator,
												 SalesHead, BranchManager)
											values(''' +@curCompanyMD+ ''',''' +@curUnitBranchMD+ ''',''' +@SONo+ ''',''' +convert(varchar,@curDocDate,121)+ 
												   ''',''' +@SalesType+ ''',''' +@PONo+ ''',''' +convert(varchar,@curDocDate,121)+ ''','''
												   +@CentralBranch+ ''',''' +@TopCode+ ''',''' +convert(varchar,@TopDays)+ ''',''' +@CentralBranch+ ''',''' 
												   +@curBranchCode+ ''',NULL,NULL,''POSTING'',''' +@VehWHMD+ ''',0,NULL,''' +@GroupPriceCode+
												   ''',NULL,NULL,0,NULL,NULL,0,''' +@PONo+ 
												   ''',NULL,NULL,NULL,2,''POSTING'',''' +convert(varchar,@curDocDate,121)+ ''',NULL,NULL,''POSTING'','''
												   +convert(varchar,@curDocDate,121)+ ''',''POSTING'',''' +convert(varchar,@curDocDate,121)+ 
												   ''',0,NULL,NULL,''' +@SalesCode+ ''',0,NULL,''POSTING'',''POSTING'',''POSTING'')'
							execute sp_executesql @sqlString

					 -- MD: Insert data to table omTrSalesDO
						execute [usprpt_PostingMultiCompany4DocNo] @curCompanyMD, @curUnitBranchMD, @DBNameMD, 'DOS', @DONo output
						set @sqlString = 'insert into ' +@DBNameMD+ '..omTrSalesDO 
												(CompanyCode, BranchCode, DONo, DODate, SONo, CustomerCode, ShipTo,
												 WareHouseCode, Expedition, Remark, Status, CreatedBy, CreatedDate, 
												 LastUpdateBy, LastUpdateDate, isLocked, LockingBy, LockingDate)
											values(''' +@curCompanyMD+ ''',''' +@curUnitBranchMD+ ''',''' +@DONo+ ''',''' 
												   +convert(varchar,@curDocDate,121)+ ''',''' +@SONo+ ''',''' 
												   +@CentralBranch+ ''',''' +@CentralBranch+ ''',''' +@VehWHMD+ ''',NULL,'''
												   +@curDocNo+ ''',''2'',''POSTING'',''' +convert(varchar,@curDocDate,121)+ 
												   ''',''POSTING'',''' +@CurrentDate+ ''',0,NULL,NULL)'
							execute sp_executesql @sqlString

					 -- MD: Insert data to table omTrSalesBPK
						execute [usprpt_PostingMultiCompany4DocNo] @curCompanyMD, @curUnitBranchMD, @DBNameMD, 'BPK', @BPKNo output
						set @sqlString = 'insert into ' +@DBNameMD+ '..omTrSalesBPK 
												(CompanyCode, BranchCode, BPKNo, BPKDate, DONo, SONo, CustomerCode, 
												 ShipTo, WareHouseCode, Expedition, Remark, Status, CreatedBy, CreatedDate, 
												 LastUpdateBy, LastUpdateDate, isLocked, LockingBy, LockingDate)
											values(''' +@curCompanyMD+ ''',''' +@curUnitBranchMD+ ''',''' +@BPKNo+ ''',''' 
												   +convert(varchar,@curDocDate,121)+ ''',''' +@DONo+ ''',''' +@SONo+ 
												   ''',''' +@CentralBranch+ ''',''' +@CentralBranch+ ''',''' +@VehWHMD+ ''',NULL,'''
												   +@curDocNo+ ''',''2'',''POSTING'',''' +convert(varchar,@curDocDate,121)+ 
												   ''',''POSTING'',''' +@CurrentDate+ ''',0,NULL,NULL)'
							execute sp_executesql @sqlString

					 -- Tax / Seri Pajak Numbering
						if convert(varchar,@curDocDate,111)<>@swTaxDocDate or 
						   @curBranchCode<>@swTaxBranch or left(@curDocNo,3)<>left(@swTaxDoc,3)
							begin
								set @swTaxDocDate = convert(varchar,@curDocDate,111)
								set @swTaxBranch  = @curBranchCode
								set @swTaxDoc	  = @curDocNo

								set @sqlString = N'select top 1 @TaxSeq=FPJSeqNo, @TaxSeqNo=SeqNo from ' 
																+@TaxDb+ '..gnMstFPJSeqNo where CompanyCode=''' 
																+@TaxCompany+ ''' and BranchCode=''' 
																+@TaxBranch+ ''' and Year=''' 
																+convert(varchar,year(@PostingDate))+ 
																''' and isActive=1 and convert(varchar,EffectiveDate,111)<=''' 
																+convert(varchar,@PostingDate,111)+ 
																''' order by CompanyCode, BranchCode, Year, SeqNo'
									execute sp_executesql @sqlString, 
														N'@TaxSeq 		bigint  	output,
														  @TaxSeqNo 	int 		output', 
														  @TaxSeq 		output,
														  @TaxSeqNo 	output

								set @sqlString = N'select top 1 @TaxTransCode=TaxTransCode from ' 
												+@TaxDb+ '..gnMstCoProfile where CompanyCode=''' +@TaxCompany + ''' and BranchCode=''' +@TaxBranch+ ''''
									execute sp_executesql @query=@sqlString, @params= N'@TaxTransCode varchar(3) output', @TaxTransCode = @TaxTransCode output

								set @TaxSeq = @TaxSeq + 1

								set @sqlString = 'update ' +@TaxDb+ '..gnMstFPJSeqNo
										  			 set FPJSeqNo = ' +convert(varchar,@TaxSeq)+ 
												 ' where CompanyCode=''' +@TaxCompany + ''' and BranchCode=''' +@TaxBranch + ''' and Year= ''' 
														+convert(varchar,year(@PostingDate))+ ''' and SeqNo= ''' 
														+convert(varchar,@TaxSeqNo)+ ''''
									execute sp_executesql @sqlString 

								--set @SeriPajakNo = @TaxTransCode + '0.' +isnull(replicate('0',11-len(convert(varchar,@TaxSeq))),'') + 
								--					+left(convert(varchar,@TaxSeq),len(convert(varchar,@TaxSeq))-8) + '-' +
								--					+right(convert(varchar,year(@PostingDate)),2)+ '.' +right(convert(varchar,@TaxSeq),8)
								if len(convert(varchar,@TaxSeq))>8
									set @SeriPajakNo =  @TaxTransCode + '0.' + replicate('0', 3-(len(convert(varchar,@TaxSeq))-8)) +
														left(convert(varchar,@TaxSeq),len(convert(varchar,@TaxSeq))-8) +
														'-' +right(convert(varchar,year(@PostingDate)),2)+ '.' +right(convert(varchar,@TaxSeq),8)
								else
									set @SeriPajakNo =  @TaxTransCode + '0.000-'+right(convert(varchar,year(@PostingDate)),2)+ '.'
														+replicate('0',8-len(convert(varchar,@TaxSeq)))+convert(varchar,@TaxSeq)
							end

					 -- MD: Insert data to table omTrSalesInvoice
						execute [usprpt_PostingMultiCompany4DocNo] @curCompanyMD, @curUnitBranchMD, @DBNameMD, 'IVU', @INVNo output
						set @sqlString = 'insert into ' +@DBNameMD+ '..omTrSalesInvoice 
												(CompanyCode, BranchCode, InvoiceNo, InvoiceDate, SONo, CustomerCode, 
												 BillTo, FakturPajakNo, FakturPajakDate, DueDate, isStandard, Remark, 
												 Status, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate, 
												 isLocked, LockingBy, LockingDate)
											values(''' +@curCompanyMD+ ''',''' +@curUnitBranchMD+ ''',''' +@INVNo+ 
												   ''',''' +convert(varchar,@curDocDate,121)+ ''',''' +@SONo+ 
												   ''',''' +@CentralBranch+ ''','''  +@CentralBranch+ ''',''' 
												   +@SeriPajakNo+ ''',''' +convert(varchar,@curDocDate,121)+ ''',''' 
												   +convert(varchar,@DueDate)+ ''',1,''' +@curDocNo+ 
												   ''',''2'',''POSTING'',''' +convert(varchar,@curDocDate,121)+ 
												   ''',''POSTING'',''' +@CurrentDate+ ''',0,NULL,NULL)'
							execute sp_executesql @sqlString

					 -- MD: Insert data to table omFakturPajakHdr
						set @sqlString = 'insert into ' +@DBNameMD+ '..omFakturPajakHdr 
												(CompanyCode, BranchCode, InvoiceNo, InvoiceDate, FakturPajakNo, 
												 FakturPajakDate, DueDate, CustomerCode, DPPAmt, DiscAmt, PPnAmt, 
												 TotalAmt, TotQuantity, PPnBMPaid, TaxType, PrintSeq, Status, 
												 CreatedBy, CreatedDate, FakturPajakCentralNo, FakturPajakCentralDate)
											values(''' +@curCompanyMD+ ''',''' +@curUnitBranchMD+ ''',''' +@INVNo+ ''',''' 
											       +convert(varchar,@curDocDate,121)+ ''',''' +@SeriPajakNo+ ''',''' 
												   +convert(varchar,@curDocDate,121)+ ''',''' 
												   +convert(varchar,@DueDate)+ ''',''' +@CentralBranch+ 
												   ''',''0'',''0'',''0'',''0'',''0'',''0'',''Standard'',''1'','' '',''POSTING'',''' 
												   +convert(varchar,@curDocDate,121)+ ''','' '',NULL)'
							execute sp_executesql @sqlString

				     -- SD: Collect Warehouse Information from gnMstLookupDtl
					  --set @sqlString = N'select top 1 @WHSD=LookUpValue from ' +@DBName+ '..gnMstLookUpDtl 
					  --					where CompanyCode=''' +@curCompanyCode+ ''' and CodeID=''MPWH'' and ParaValue=''' +@CentralBranch+ ''''
						set @sqlString = N'select top 1 @WHSD=LookUpValue from ' +@DBName+ '..gnMstLookUpDtl where CompanyCode=''' 
						                    +@curCompanyCode+ ''' and CodeID=''MPWH'' and ParaValue=''' +@CentralBranch+ ''' order by SeqNo'
							execute sp_executesql @sqlString, N'@WHSD varchar(15) output', @WHSD output

					 -- SD: Insert data to table omTrPurchaseBPU  
						execute [usprpt_PostingMultiCompany4DocNo] @curCompanyCode, @CentralBranch, @DBName, 'BPU', @BPUNo output
						set @sqlString = 'insert into ' +@DBName+ '..omTrPurchaseBPU
												(CompanyCode, BranchCode, PONo, BPUNo, BPUDate, SupplierCode, ShipTo, RefferenceDONo,
												 RefferenceDODate, RefferenceSJNo, RefferenceSJDate, WarehouseCode, Expedition, 
												 BPUType, Remark, Status, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate, 
												 isLocked, LockingBy, LockingDate, BPUSJDate)
											values(''' +@curCompanyCode+ ''',''' +@CentralBranch+ ''',''' +@PONo+ 
												   ''',''' +@BPUNo+ ''',''' +convert(varchar,@curDocDate,121)+ ''',''' 
												   +@curUnitBranchMD+ ''',''' +@CentralBranch+ ''',''' +@DONo+ ''',''' 
												   +convert(varchar,@curDocDate,121)+ ''',''' +@BPKNo+ ''','''
												   +convert(varchar,@curDocDate,121)+ ''',''' +@WHSD+ ''','''
												   +@curUnitBranchMD+ ''',''2'',''' +@curDocNo+ ''',''2'',''POSTING'',''' 
												   +convert(varchar,@PostingDate,111)+ ''',''POSTING'',''' 
												   +@currentDate+ ''',0,NULL,NULL,''' +convert(varchar,@curDocDate,121)+ ''')' 
							execute sp_executesql @sqlString

					 -- SD: Insert data to table omTrPurchaseHPP
						execute [usprpt_PostingMultiCompany4DocNo] @curCompanyCode, @CentralBranch, @DBName, 'HPU', @HPPNo output
						set @sqlString = 'insert into ' +@DBName+ '..omTrPurchaseHPP
												(CompanyCode, BranchCode, HPPNo, HPPDate, PONo, SupplierCode, BillTo, 
												 RefferenceInvoiceNo, RefferenceInvoiceDate, RefferenceFakturPajakNo,
												 RefferenceFakturPajakDate, DueDate, Remark, Status, CreatedBy, CreatedDate, 
												 LastUpdateBy, LastUpdateDate, isLocked, LockingBy, LockingDate)
											values(''' +@curCompanyCode+ ''',''' +@CentralBranch+ ''',''' +@HPPNo+ ''',''' 
												   +convert(varchar,@curDocDate,121)+ ''',''' +@PONo+ ''',''' +@curUnitBranchMD+ 
												   ''',''' +@CentralBranch+ ''',''' +@INVNo+ ''',''' 
												   +convert(varchar,@curDocDate,121)+ ''',''' +@SeriPajakNo+ ''','''
												   +convert(varchar,@curDocDate,121)+ ''',''' +convert(varchar,@DueDate)+ 
												   ''',NULL,''2'',''POSTING'',''' +convert(varchar,@PostingDate,111)+ 
												   ''',''POSTING'',''' +@currentDate+ ''',0,NULL,NULL)' 
							execute sp_executesql @sqlString

				     -- SD: Collect Warehouse To Information from gnMstLookupDtl
						set @sqlString = N'select top 1 @WHFROM=LookUpValue from ' +@DBName+ '..gnMstLookUpDtl 
											where CompanyCode=''' +@curCompanyCode+ ''' and CodeID=''MPWH'' and ParaValue=''' 
											+@CentralBranch+ ''' order by SeqNo'
							execute sp_executesql @sqlString, N'@WHFROM varchar(15) output', @WHFROM output

						set @sqlString = N'select top 1 @WHTO=LookUpValue from ' +@DBName+ '..gnMstLookUpDtl 
											where CompanyCode=''' +@curCompanyCode+ ''' and CodeID=''MPWH'' and ParaValue=''' 
											+@curBranchCode+ ''' order by SeqNo'
							execute sp_executesql @sqlString, N'@WHTO varchar(15) output', @WHTO output

						set @WHFrom    = isnull(@WHFrom,@CentralBranch)
						set @WHTo      = isnull(@WHTo,@curBranchCode)

					 -- SD: Insert data to table omTrInventTransferOut (Transfer Unit Out from Holding to branch)
						execute [usprpt_PostingMultiCompany4DocNo] @curCompanyCode, @CentralBranch, @DBName, 'VTO', @VTONo output
						set @sqlString = 'insert into ' +@DBName+ '..omTrInventTransferOut
												(CompanyCode, BranchCode, TransferOutNo, TransferOutDate, ReferenceNo, 
												 ReferenceDate, BranchCodeFrom, WareHouseCodeFrom, BranchCodeTo, 
												 WareHouseCodeTo, ReturnDate, Remark, Status, CreatedBy, CreatedDate, 
												 LastUpdateBy, LastUpdateDate, isLocked, LockedBy, LockedDate)
											values(''' +@curCompanyCode+ ''',''' +@CentralBranch+ ''',''' +@VTONo+ ''',''' 
												   +convert(varchar,@curDocDate,121)+ ''',''' +@PONo+ ''',''' 
												   +convert(varchar,@curDocDate,121)+ ''',''' +@CentralBranch+ 
												   ''',''' +@WHFrom+ ''',''' +@curBranchCode+ ''',''' +@WHTo+
												   ''',''1900/01/01'',''' +@INVNo+ ''',2,''POSTING'',''' 
												   +convert(varchar,@PostingDate,111)+ ''',''POSTING'',''' 
												   +@currentDate+ ''',0,NULL,NULL)' 
							execute sp_executesql @sqlString

					 -- SD: Insert data to table omTrInventTransferIn (Transfer Unit In from Holding to branch)
						execute [usprpt_PostingMultiCompany4DocNo] @curCompanyCode, @CentralBranch, @DBName, 'VTI', @VTINo output
						set @sqlString = 'insert into ' +@DBName+ '..omTrInventTransferIn
												(CompanyCode, BranchCode, TransferInNo, TransferInDate, TransferOutNo,
												 ReferenceNo, ReferenceDate, BranchCodeFrom, WareHouseCodeFrom, BranchCodeTo, 
												 WareHouseCodeTo, ReturnDate, Remark, Status, CreatedBy, CreatedDate, 
												 LastUpdateBy, LastUpdateDate, isLocked, LockedBy, LockedDate)
											values(''' +@curCompanyCode+ ''',''' +@curBranchCode+ ''',''' +@VTINo+ ''',''' 
												   +convert(varchar,@curDocDate,121)+ ''',''' +@VTONo+ ''',''' +@PONo+ 
												   ''',''' +convert(varchar,@curDocDate,121)+ ''',''' +@CentralBranch+ 
												   ''',''' +@WHFrom+ ''',''' +@curBranchCode+ ''',''' +@WHTo+
												   ''',''1900/01/01'',''' +@INVNo+ ''',2,''POSTING'',''' 
												   +convert(varchar,@PostingDate,111)+ ''',''POSTING'',''' 
												   +@currentDate+ ''',0,NULL,NULL)' 
							execute sp_executesql @sqlString

					end

				set @SeqNo = @SeqNo + 1

			 -- SD: Insert data to table omTrPurchasePOModel
				set @sqlString = 'merge into ' +@DBName+ '..omTrPurchasePOModel as POM using (values(''' 
									+@curCompanyCode+ ''',''' +@CentralBranch+ ''',''' +@PONo+ ''',''' 
									+@curSalesModelCode+ ''',''' +convert(varchar,@curSalesModelYear)+ ''','''
									+convert(varchar,@RetailPriceExcludePPN)+ ''',''' +convert(varchar,@PPNBeforeDisc)+ 
									''',''0'',''' +convert(varchar,@RetailPriceIncludePPN)+ ''',''' 
									+convert(varchar,@DiscPriceExcludePPN)+ ''',''' +convert(varchar,@DiscPriceIncludePPN)+
									''',''' +convert(varchar,@NetSalesExcludePPN)+ ''',''' +convert(varchar,@PPNAfterDisc)+
									''',''0'',''' +convert(varchar,@NetSalesIncludePPN)+ ''',''' +convert(varchar,@PPNBMPaid)+
									''',''' +convert(varchar,@OthersDPP)+ ''',''' +convert(varchar,@OthersPPn)+ ''',1,1,''' 
									+@SONo+ ''',''' +@curCreatedBy+ ''',''' +convert(varchar,@curCreatedDate,121)+ ''',''' 
									+@curLastUpdateBy+ ''',''' +convert(varchar,@curLastUpdateDate,121)+ '''))
					    as SRC (NewCompany, NewBranch, NewPONo, NewSalesModelCode, NewSalesModelYear, 
								NewBeforeDiscDPP, NewBeforeDiscPPn, NewBeforeDiscPPnBM, NewBeforeDiscTotal, 
								NewDiscExcludePPn, NewDiscIncludePPn, NewAfterDiscDPP, NewAfterDiscPPn, 
								NewAfterDiscPPnBM, NewAfterDiscTotal, NewPPnBMPaid, NewOthersDPP, 
								NewOthersPPn, NewQuantityPO, NewQuantityBPU, NewRemark, NewCreatedBy, 
								NewCreatedDate, NewLastUpdateBy, NewLastUpdateDate)
						on POM.CompanyCode    = SRC.NewCompany
					   and POM.BranchCode     = SRC.NewBranch
					   and POM.PONo           = SRC.NewPONo
					   and POM.SalesModelCode = SRC.NewSalesModelCode
					   and POM.SalesModelYear = SRC.NewSalesModelYear
					  when matched 
						   then update set QuantityPO     = QuantityPO  + SRC.NewQuantityPO
						                 , QuantityBPU    = QuantityBPU + SRC.NewQuantityBPU
										 , LastUpdateBy   = ''' +@curLastUpdateBy+ '''
										 , LastUpdateDate = ''' +convert(varchar,@curLastUpdateDate,121)+ '''
					  when not matched by target 
						   then insert (CompanyCode, BranchCode, PONo, SalesModelCode, SalesModelYear, 
										BeforeDiscDPP, BeforeDiscPPn, BeforeDiscPPnBM, BeforeDiscTotal, 
										DiscExcludePPn, DiscIncludePPn, AfterDiscDPP, AfterDiscPPn, 
										AfterDiscPPnBM, AfterDiscTotal, PPnBMPaid, OthersDPP, 
										OthersPPn, QuantityPO, QuantityBPU, Remark, CreatedBy, 
										CreatedDate, LastUpdateBy, LastUpdateDate)
								values (NewCompany, NewBranch, NewPONo, NewSalesModelCode, NewSalesModelYear, 
										NewBeforeDiscDPP, NewBeforeDiscPPn, NewBeforeDiscPPnBM, NewBeforeDiscTotal, 
										NewDiscExcludePPn, NewDiscIncludePPn, NewAfterDiscDPP, NewAfterDiscPPn, 
										NewAfterDiscPPnBM, NewAfterDiscTotal, NewPPnBMPaid, NewOthersDPP, 
										NewOthersPPn, NewQuantityPO, NewQuantityBPU, NewRemark, NewCreatedBy, 
										NewCreatedDate, NewLastUpdateBy, NewLastUpdateDate);'
					execute sp_executesql @sqlString

			 -- SD: Insert data to table omTrPurchasePOModelColour
				set @sqlString = 'merge into ' +@DBName+ '..omTrPurchasePOModelColour as POC using (values(''' 
									+@curCompanyCode+ ''',''' +@CentralBranch+ ''',''' +@PONo+ ''',''' 
									+@curSalesModelCode+ ''',''' +convert(varchar,@curSalesModelYear)+ 
									''',''' +@curColourCode+ ''',1,''' +@curDocNo+ ''','''+@curCreatedBy+ ''',''' 
									+convert(varchar,@curCreatedDate,121)+ ''',''' +@curLastUpdateBy+ 
									''',''' +convert(varchar,@curLastUpdateDate,121)+ '''))
					    as SRC (NewCompany, NewBranch, NewPONo, NewSalesModelCode, NewSalesModelYear, NewColourCode, 
								NewQuantity, NewRemark, NewCreatedBy, NewCreatedDate, NewLastUpdateBy, NewLastUpdateDate)
						on POC.CompanyCode    = SRC.NewCompany
					   and POC.BranchCode     = SRC.NewBranch
					   and POC.PONo           = SRC.NewPONo
					   and POC.SalesModelCode = SRC.NewSalesModelCode
					   and POC.SalesModelYear = SRC.NewSalesModelYear
					   and POC.ColourCode     = SRC.NewColourCode
					  when matched 
						   then update set Quantity       = Quantity + SRC.NewQuantity
										 , LastUpdateBy   = ''' +@curLastUpdateBy+ '''
										 , LastUpdateDate = ''' +convert(varchar,@curLastUpdateDate,121)+ '''
					  when not matched by target 
						   then insert (CompanyCode, BranchCode, PONo, SalesModelCode, SalesModelYear, ColourCode, 
										Quantity, Remark, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
								values (NewCompany, NewBranch, NewPONo, NewSalesModelCode, NewSalesModelYear, 
										NewColourCode, NewQuantity, NewRemark, NewCreatedBy, NewCreatedDate, 
										NewLastUpdateBy, NewLastUpdateDate);'
					execute sp_executesql @sqlString

			 -- MD: Insert data to table omTrSalesSOModel
				set @sqlString = 'merge into ' +@DBNameMD+ '..omTrSalesSOModel as SO using (values(''' 
									+@curCompanyMD+ ''',''' +@curUnitBranchMD+ ''',''' +@SONo+ ''',''' 
									+@curSalesModelCode+ ''',''' +convert(varchar,@curSalesModelYear)+ ''','''
									+@curChassisCode+ ''','''+convert(varchar,@RetailPriceExcludePPN)+ ''',''' 
									+convert(varchar,@PPNBeforeDisc)+ ''',''0'',''' +convert(varchar,@RetailPriceIncludePPN)+ ''',''' 
									+convert(varchar,@DiscPriceExcludePPN)+ ''',''' +convert(varchar,@DiscPriceIncludePPN)+ ''',''' 
									+convert(varchar,@NetSalesExcludePPN)+ ''',''' +convert(varchar,@PPNAfterDisc)+ ''',''0'',''' 
									+convert(varchar,@NetSalesIncludePPN)+ ''',''' +convert(varchar,@OthersDPP)+ ''',''' 
									+convert(varchar,@OthersPPn)+ ''',1,1,''' +@curDocNo+ ''',''' +@curCreatedBy+ ''',''' 
									+convert(varchar,@curCreatedDate,121)+ ''',''' +@curLastUpdateBy+ ''',''' 
									+convert(varchar,@curLastUpdateDate,121)+ ''',0,0,0))
					    as SRC (NewCompany, NewBranch, NewSONo, NewSalesModelCode, NewSalesModelYear, 
								NewChassisCode, NewBeforeDiscDPP, NewBeforeDiscPPn, NewBeforeDiscPPnBM, 
								NewBeforeDiscTotal, NewDiscExcludePPn, NewDiscIncludePPn, NewAfterDiscDPP, 
								NewAfterDiscPPn, NewAfterDiscPPnBM, NewAfterDiscTotal, NewOthersDPP, 
								NewOthersPPn, NewQuantitySO, NewQuantityDO, NewRemark, NewCreatedBy, 
								NewCreatedDate, NewLastUpdateBy, NewLastUpdateDate, NewShipAmt, 
								NewDepositAmt, NewOthersAmt)
						on SO.CompanyCode    = SRC.NewCompany
					   and SO.BranchCode     = SRC.NewBranch
					   and SO.SONo           = SRC.NewSONo
					   and SO.SalesModelCode = SRC.NewSalesModelCode
					   and SO.SalesModelYear = SRC.NewSalesModelYear
					  when matched 
						   then update set QuantitySO     = QuantitySO + SRC.NewQuantitySO
						                 , QuantityDO     = QuantityDO + SRC.NewQuantityDO
										 , LastUpdateBy   = ''' +@curLastUpdateBy+ '''
										 , LastUpdateDate = ''' +convert(varchar,@curLastUpdateDate,121)+ '''
					  when not matched by target 
						   then insert (CompanyCode, BranchCode, SONo, SalesModelCode, SalesModelYear, 
										ChassisCode, BeforeDiscDPP, BeforeDiscPPn, BeforeDiscPPnBM, 
										BeforeDiscTotal, DiscExcludePPn, DiscIncludePPn, AfterDiscDPP, 
										AfterDiscPPn, AfterDiscPPnBM, AfterDiscTotal, OthersDPP, 
										OthersPPn, QuantitySO, QuantityDO, Remark, CreatedBy, 
										CreatedDate, LastUpdateBy, LastUpdateDate, ShipAmt, 
										DepositAmt, OthersAmt)
								values (NewCompany, NewBranch, NewSONo, NewSalesModelCode, NewSalesModelYear, 
										NewChassisCode, NewBeforeDiscDPP, NewBeforeDiscPPn, NewBeforeDiscPPnBM, 
										NewBeforeDiscTotal, NewDiscExcludePPn, NewDiscIncludePPn, NewAfterDiscDPP, 
										NewAfterDiscPPn, NewAfterDiscPPnBM, NewAfterDiscTotal, NewOthersDPP, 
										NewOthersPPn, NewQuantitySO, NewQuantityDO, NewRemark, NewCreatedBy, 
										NewCreatedDate, NewLastUpdateBy, NewLastUpdateDate, NewShipAmt, 
										NewDepositAmt, NewOthersAmt);'
					execute sp_executesql @sqlString

			 -- MD: Insert data to table omTrSalesSOModelColour
				set @sqlString = 'merge into ' +@DBNameMD+ '..omTrSalesSOModelColour as SO using (values(''' 
									+@curCompanyMD+ ''',''' +@curUnitBranchMD+ ''',''' +@SONo+ ''',''' 
									+@curSalesModelCode+ ''',''' +convert(varchar,@curSalesModelYear)+ 
									''',''' +@curColourCode+ ''',1,''' +@curDocNo+ ''','''+@curCreatedBy+ ''',''' 
									+convert(varchar,@curCreatedDate,121)+ ''',''' +@curLastUpdateBy+ 
									''',''' +convert(varchar,@curLastUpdateDate,121)+ '''))
					    as SRC (NewCompany, NewBranch, NewSONo, NewSalesModelCode, NewSalesModelYear, NewColourCode, 
								NewQuantity, NewRemark, NewCreatedBy, NewCreatedDate, NewLastUpdateBy, NewLastUpdateDate)
						on SO.CompanyCode    = SRC.NewCompany
					   and SO.BranchCode     = SRC.NewBranch
					   and SO.SONo           = SRC.NewSONo
					   and SO.SalesModelCode = SRC.NewSalesModelCode
					   and SO.SalesModelYear = SRC.NewSalesModelYear
					   and SO.ColourCode     = SRC.NewColourCode
					  when matched 
						   then update set Quantity       = Quantity + SRC.NewQuantity
										 , LastUpdateBy   = ''' +@curLastUpdateBy+ '''
										 , LastUpdateDate = ''' +convert(varchar,@curLastUpdateDate,121)+ '''
					  when not matched by target 
						   then insert (CompanyCode, BranchCode, SONo, SalesModelCode, SalesModelYear, ColourCode, 
										Quantity, Remark, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
								values (NewCompany, NewBranch, NewSONo, NewSalesModelCode, NewSalesModelYear, 
										NewColourCode, NewQuantity, NewRemark, NewCreatedBy, NewCreatedDate, 
										NewLastUpdateBy, NewLastUpdateDate);'
					execute sp_executesql @sqlString

			 -- MD: Insert data to table omTrSalesSOVin
				set @sqlString = 'insert into ' +@DBNameMD+ '..omTrSalesSOVin
										(CompanyCode, BranchCode, SONo, SalesModelCode, SalesModelYear, 
									     ColourCode, SOSeq, ChassisCode, ChassisNo, EngineCode, EngineNo, 
										 ServiceBookNo, KeyNo, EndUserName, EndUserAddress1, EndUserAddress2, 
										 EndUserAddress3, SupplierBBN, CityCode, BBN, KIR, Remark, StatusReq, 
										 CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
								  values(''' +@curCompanyMD+ ''',''' +@curUnitBranchMD+ ''',''' +@SONo+ ''',''' 
								         +@curSalesModelCode+ ''',''' +convert(varchar,@curSalesModelYear)+ 
										 ''',''' +@curColourCode+ ''',''' +Convert(varchar,@SeqNo)+ ''','''
										 +@curChassisCode+ ''',''' +convert(varchar,@curChassisNo)+ ''',''' 
										 +@curEngineCode+ ''',''' +convert(varchar,@curEngineNo)+ ''',''' 
										 +@VehServiceBookNo+ ''',''' +@VehKeyNo+ ''',''' +@EndUserName+ ''',''' 
										 +@EndUserAddress1+ ''',''' +@EndUserAddress2+ ''','''
										 +@EndUserAddress3+ ''',NULL,''' +@CityCode+ ''',0,0,NULL,0,''' 
										 +@curCreatedBy+ ''',''' +convert(varchar,@curCreatedDate,121)+ ''','''
										 +@curLastUpdateBy+ ''',''' +convert(varchar,@curLastUpdateDate,121)+ ''')'
					execute sp_executesql @sqlString

			 -- MD: Insert data to table omTrSalesSOModelAdditional
				set @sqlString = 'merge into ' +@DBNameMD+ '..omTrSalesSOModelAdditional as SO using (values(''' 
									+@curCompanyMD+ ''',''' +@curUnitBranchMD+ ''',''' +@SONo+ ''',''' 
									+@curSalesModelCode+ ''',''' +convert(varchar,@curSalesModelYear)+ 
									''',''A'',NULL,NULL,''' +@curCreatedBy+ ''',''' +convert(varchar,@curCreatedDate,121)+ 
									''',''' +@curLastUpdateBy+ ''',''' +convert(varchar,@curLastUpdateDate,121)+ '''))
					    as SRC (NewCompany, NewBranch, NewSONo, NewSalesModelCode, 
								NewSalesModelYear, NewStatusVehicle, NewOthersBrand, NewOthersType, 
								NewCreatedBy, NewCreatedDate, NewLastUpdateBy, NewLastUpdateDate)
						on SO.CompanyCode    = SRC.NewCompany
					   and SO.BranchCode     = SRC.NewBranch
					   and SO.SONo           = SRC.NewSONo
					   and SO.SalesModelCode = SRC.NewSalesModelCode
					   and SO.SalesModelYear = SRC.NewSalesModelYear
					  when matched 
						   then update set LastUpdateBy   = ''' +@curLastUpdateBy+ '''
										 , LastUpdateDate = ''' +convert(varchar,@curLastUpdateDate,121)+ '''
					  when not matched by target 
						   then insert (CompanyCode, BranchCode, SONo, SalesModelCode, 
										SalesModelYear, StatusVehicle, OthersBrand, OthersType, 
										CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
								values (NewCompany, NewBranch, NewSONo, NewSalesModelCode, 
										NewSalesModelYear, NewStatusVehicle, NewOthersBrand, NewOthersType, 
										NewCreatedBy, NewCreatedDate, NewLastUpdateBy, NewLastUpdateDate);'
					execute sp_executesql @sqlString

			 -- MD: Insert data to table omTrSalesDODetail
				set @sqlString = 'insert into ' +@DBNameMD+ '..omTrSalesDODetail
										(CompanyCode, BranchCode, DONo, DOSeq, SalesModelCode, SalesModelYear, 
									     ChassisCode, ChassisNo, EngineCode, EngineNo, ColourCode, Remark, 
										 StatusBPK, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
								  values(''' +@curCompanyMD+ ''',''' +@curUnitBranchMD+ ''',''' +@DONo+ ''',''' 
								         +convert(varchar,@SeqNo)+ ''',''' +@curSalesModelCode+ ''',''' 
										 +convert(varchar,@curSalesModelYear)+ ''',''' +@curChassisCode+ 
										 ''',''' +convert(varchar,@curChassisNo)+ ''',''' +@curEngineCode+ 
										 ''',''' +convert(varchar,@curEngineNo)+ ''',''' +@curColourCode+ 
										 ''',NULL,''1'',''' +@curCreatedBy+ ''',''' 
										 +convert(varchar,@curCreatedDate,121)+ ''',''' +@curLastUpdateBy+ 
										 ''',''' +convert(varchar,@curLastUpdateDate,121)+ ''')'
					execute sp_executesql @sqlString

			 -- MD: Insert data to table omTrSalesBPKModel
				set @sqlString = 'merge into ' +@DBNameMD+ '..omTrSalesBPKModel as BPK using (values(''' 
									+@curCompanyMD+ ''',''' +@curUnitBranchMD+ ''',''' +@BPKNo+ ''',''' 
									+@curSalesModelCode+ ''',''' +convert(varchar,@curSalesModelYear)+ 
									''',1,1,''' +@curCreatedBy+ ''',''' +convert(varchar,@curCreatedDate,121)+ 
									''',''' +@curLastUpdateBy+ ''',''' +convert(varchar,@curLastUpdateDate,121)+ '''))
					    as SRC (NewCompany, NewBranch, NewBPKNo, NewSalesModelCode, 
								NewSalesModelYear, NewQuantityBPK, NewQuantityInvoice,
								NewCreatedBy, NewCreatedDate, NewLastUpdateBy, NewLastUpdateDate)
						on BPK.CompanyCode    = SRC.NewCompany
					   and BPK.BranchCode     = SRC.NewBranch
					   and BPK.BPKNo           = SRC.NewBPKNo
					   and BPK.SalesModelCode = SRC.NewSalesModelCode
					   and BPK.SalesModelYear = SRC.NewSalesModelYear
					  when matched 
						   then update set QuantityBPK     = QuantityBPK     + SRC.NewQuantityBPK
						                 , QuantityInvoice = QuantityInvoice + SRC.NewQuantityInvoice
										 , LastUpdateBy    = ''' +@curLastUpdateBy+ '''
										 , LastUpdateDate  = ''' +convert(varchar,@curLastUpdateDate,121)+ '''
					  when not matched by target 
						   then insert (CompanyCode, BranchCode, BPKNo, SalesModelCode, 
										SalesModelYear, QuantityBPK, QuantityInvoice, 
										CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
								values (NewCompany, NewBranch, NewBPKNo, NewSalesModelCode, 
										NewSalesModelYear, NewQuantityBPK, NewQuantityInvoice,
										NewCreatedBy, NewCreatedDate, NewLastUpdateBy, NewLastUpdateDate);'
					execute sp_executesql @sqlString

			 -- MD: Insert data to table omTrSalesBPKDetail
				set @sqlString = 'insert into ' +@DBNameMD+ '..omTrSalesBPKDetail
										(CompanyCode, BranchCode, BPKNo, BPKSeq, SalesModelCode, SalesModelYear, 
									     ChassisCode, ChassisNo, EngineCode, EngineNo, ColourCode, ServiceBookNo,
										 KeyNo, ReqOutNo, Remark, StatusPDI, StatusInvoice, 
										 CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
								  values(''' +@curCompanyMD+ ''',''' +@curUnitBranchMD+ ''',''' +@BPKNo+ ''',''' 
								         +convert(varchar,@SeqNo)+ ''',''' +@curSalesModelCode+ ''',''' 
										 +convert(varchar,@curSalesModelYear)+ ''',''' +@curChassisCode+ 
										 ''',''' +convert(varchar,@curChassisNo)+ ''',''' +@curEngineCode+ 
										 ''',''' +convert(varchar,@curEngineNo)+ ''',''' +@curColourCode+ 
										 ''',''' +@VehServiceBookNo+ ''',''' +@VehKeyNo+ ''',NULL,NULL,''0'',''1'','''
										 +@curCreatedBy+ ''',''' +convert(varchar,@curCreatedDate,121)+ ''',''' 
										 +@curLastUpdateBy+ ''',''' +convert(varchar,@curLastUpdateDate,121)+ ''')'
					execute sp_executesql @sqlString

			 -- MD: Insert data to table omTrSalesInvoiceBPK
				set @sqlString = 'merge into ' +@DBNameMD+ '..omTrSalesInvoiceBPK as INV using (values(''' 
									+@curCompanyMD+ ''',''' +@curUnitBranchMD+ ''',''' 
									+@INVNo+ ''',''' +@BPKNo+ ''',NULL,''' 
									+@curCreatedBy+ ''',''' +convert(varchar,@curCreatedDate,121)+ ''',''' 
									+@curLastUpdateBy+ ''',''' +convert(varchar,@curLastUpdateDate,121)+ '''))
					    as SRC (NewCompany, NewBranch, NewInvoiceNo, NewBPKNo, NewRemark,
								NewCreatedBy, NewCreatedDate, NewLastUpdateBy, NewLastUpdateDate)
						on INV.CompanyCode    = SRC.NewCompany
					   and INV.BranchCode     = SRC.NewBranch
					   and INV.InvoiceNo      = SRC.NewInvoiceNo
					   and INV.BPKNo		  = SRC.NewBPKNo
					  when matched 
						   then update set LastUpdateBy   = ''' +@curLastUpdateBy+ '''
										 , LastUpdateDate = ''' +convert(varchar,@curLastUpdateDate,121)+ '''
					  when not matched by target 
						   then insert (CompanyCode, BranchCode, InvoiceNo, BPKNo, Remark, 
										CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
								values (NewCompany, NewBranch, NewInvoiceNo, NewBPKNo, NewRemark,
										NewCreatedBy, NewCreatedDate, NewLastUpdateBy, NewLastUpdateDate);'
					execute sp_executesql @sqlString

			 -- MD: Insert data to table omTrSalesInvoiceModel
				set @sqlString = 'merge into ' +@DBNameMD+ '..omTrSalesInvoiceModel as INV using (values(''' 
									+@curCompanyMD+ ''',''' +@curUnitBranchMD+ ''',''' +@INVNo+ ''',''' 
									+@BPKNo+ ''',''' +@curSalesModelCode+ ''',''' +convert(varchar,@curSalesModelYear)+ ''',1,'''
									+convert(varchar,@RetailPriceExcludePPN)+ ''',''' +convert(varchar,@DiscPriceExcludePPN)+ ''',''' 
									+convert(varchar,@DiscPriceIncludePPN)+ ''',''' +convert(varchar,@NetSalesExcludePPN)+ ''',''' 
									+convert(varchar,@PPNAfterDisc)+ ''',''0'',''' +convert(varchar,@NetSalesIncludePPN)+ ''','''
									+convert(varchar,@PPNBMPaid)+ ''',''' +convert(varchar,@OthersDPP)+ ''',''' 
									+convert(varchar,@OthersPPn)+ ''',0,''' +@curDocNo+ ''',''' +@curCreatedBy+ ''',''' 
									+convert(varchar,@curCreatedDate,121)+ ''',''' +@curLastUpdateBy+ ''',''' 
									+convert(varchar,@curLastUpdateDate,121)+ ''',0,0,0))
					    as SRC (NewCompany, NewBranch, NewInvoiceNo, NewBPKNo, NewSalesModelCode, 
								NewSalesModelYear, NewQuantity, NewBeforeDiscDPP, NewDiscExcludePPn, 
								NewDiscIncludePPn, NewAfterDiscDPP, NewAfterDiscPPn, NewAfterDiscPPnBM, 
								NewAfterDiscTotal, NewPPnBMPaid, NewOthersDPP, NewOthersPPn, 
								NewQuantityReturn, NewRemark, NewCreatedBy, NewCreatedDate, 
								NewLastUpdateBy, NewLastUpdateDate, NewShipAmt, 
								NewDepositAmt, NewOthersAmt)
						on INV.CompanyCode    = SRC.NewCompany
					   and INV.BranchCode     = SRC.NewBranch
					   and INV.InvoiceNo      = SRC.NewInvoiceNo
					   and INV.BPKNo		  = SRC.NewBPKNo
					   and INV.SalesModelCode = SRC.NewSalesModelCode
					   and INV.SalesModelYear = SRC.NewSalesModelYear
					  when matched 
						   then update set Quantity       = Quantity + SRC.NewQuantity
										 , LastUpdateBy   = ''' +@curLastUpdateBy+ '''
										 , LastUpdateDate = ''' +convert(varchar,@curLastUpdateDate,121)+ '''
					  when not matched by target 
						   then insert (CompanyCode, BranchCode, InvoiceNo, BPKNo, SalesModelCode, SalesModelYear, 
										Quantity, BeforeDiscDPP,  DiscExcludePPn, DiscIncludePPn, AfterDiscDPP, 
										AfterDiscPPn, AfterDiscPPnBM, AfterDiscTotal, PPnBMPaid, OthersDPP, 
										OthersPPn, QuantityReturn, Remark, CreatedBy, CreatedDate, 
										LastUpdateBy, LastUpdateDate, ShipAmt, DepositAmt, OthersAmt)
								values (NewCompany, NewBranch, NewInvoiceNo, NewBPKNo, NewSalesModelCode, 
										NewSalesModelYear, NewQuantity, NewBeforeDiscDPP, NewDiscExcludePPn, 
										NewDiscIncludePPn, NewAfterDiscDPP, NewAfterDiscPPn, NewAfterDiscPPnBM, 
										NewAfterDiscTotal, NewPPnBMPaid, NewOthersDPP, NewOthersPPn, 
										NewQuantityReturn, NewRemark, NewCreatedBy, NewCreatedDate, 
										NewLastUpdateBy, NewLastUpdateDate, NewShipAmt, 
										NewDepositAmt, NewOthersAmt);'
					execute sp_executesql @sqlString

			 -- MD: Insert data to table omTrSalesInvoiceVIN
				set @sqlString = 'insert into ' +@DBNameMD+ '..omTrSalesInvoiceVIN
										(CompanyCode, BranchCode, InvoiceNo, BPKNo, SalesModelCode, 
										 SalesModelYear, InvoiceSeq, ColourCode, ChassisCode, ChassisNo,
										 EngineCode, EngineNo, COGS, IsReturn, CreatedBy, CreatedDate, 
										 LastUpdateBy, LastUpdateDate)
								  values(''' +@curCompanyMD+ ''',''' +@curUnitBranchMD+ ''',''' +@INVNo+ ''',''' +@BPKNo+ ''',''' 
										 +@curSalesModelCode+ ''',''' +convert(varchar,@curSalesModelYear)+ ''','''  
										 +convert(varchar,@SeqNo)+ ''',''' +convert(varchar,@curColourCode)+ ''',''' 
										 +convert(varchar,@curChassisCode)+ ''',''' +convert(varchar,@curChassisNo)+ 
										 ''',''' +@curEngineCode+ ''',''' +convert(varchar,@curEngineNo)+ ''',''' 
										 +convert(varchar,@VehCOGS)+ ''',0,''' +@curCreatedBy+ ''',''' 
										 +convert(varchar,@curCreatedDate,121)+ ''',''' +@curLastUpdateBy+ ''',''' 
										 +convert(varchar,@curLastUpdateDate,121)+ ''')'
					execute sp_executesql @sqlString

			 -- MD: Insert data to table omFakturPajakDetailDO
				set @sqlString = 'merge into ' +@DBNameMD+ '..omFakturPajakDetailDO as FPJ using (values(''' 
									           +@curCompanyMD+ ''',''' +@curUnitBranchMD+ ''',''' +@INVNo+ ''',''' +@DONo+ '''))
										as SRC (NewCompany, NewBranch, NewInvoiceNo, NewDONo)
											on FPJ.CompanyCode = SRC.NewCompany
										   and FPJ.BranchCode  = SRC.NewBranch
										   and FPJ.InvoiceNo   = SRC.NewInvoiceNo
										   and FPJ.DONo		   = SRC.NewDONo
										  when not matched by target 
											   then insert (CompanyCode, BranchCode, InvoiceNo, DONo)
													values (NewCompany, NewBranch, NewInvoiceNo, NewDONo);'
					execute sp_executesql @sqlString

			 -- MD: Insert data to table omFakturPajakDetail
				set @sqlString = 'insert into ' +@DBNameMD+ '..omFakturPajakDetail
										(CompanyCode, BranchCode, InvoiceNo, BPKNo, SalesModelCode, SalesModelYear, 
										 ChassisCode, ChassisNo, EngineNo, BeforeDiscDPP, DiscExcludePPn, 
										 AfterDiscPPn, AfterDiscPPnBM, PPnBMPaid, OthersDPP, OthersPPn)
								  values(''' +@curCompanyMD+ ''',''' +@curUnitBranchMD+ ''',''' +@INVNo+ ''',''' +@BPKNo+ 
								         ''',''' +@curSalesModelCode+ ''',''' +convert(varchar,@curSalesModelYear)+ 
										 ''',''' +convert(varchar,@curChassisCode)+ ''',''' +convert(varchar,@curChassisNo)+ 
										 ''',''' +convert(varchar,@curEngineNo)+ ''',''' +convert(varchar,@RetailPriceExcludePPN)+ 
										 ''',''' +convert(varchar,@DiscPriceExcludePPN)+ ''',''' +convert(varchar,@PPNAfterDisc)+ 
										 ''',''0'',''0'',''' +convert(varchar,@OthersDPP)+ ''',''' +convert(varchar,@OthersPPn)+ ''')'
					execute sp_executesql @sqlString

			 -- MD: Update data to table omFakturPajakHdr
				set @sqlString = 'update ' +@DBNameMD+ '..omFakturPajakHdr ' +
									   'set DPPAmt=DPPAmt+' +convert(varchar,@RetailPriceExcludePPN)+ 
									   ', DiscAmt=DiscAmt+' +convert(varchar,@DiscPriceExcludePPN)+
									   ', PPnAmt=PPnAmt+'   +convert(varchar,@PPnAfterDisc)+
									   ', TotalAmt=TotalAmt+' +convert(varchar,@NetSalesIncludePPN)+
									   ', TotQuantity=TotQuantity+1 ' +
									   ' where CompanyCode='''+@curCompanyMD+''' and BranchCode='''
											+@curUnitBranchMD+ ''' and InvoiceNo=''' +@INVNo+''''
					execute sp_executesql @sqlString

			 -- MD: Insert data to table gnGenerateTax 
				--set @sqlString = 'insert into ' +@DBNameMD+ '..gnGenerateTax
				--						(CompanyCode, BranchCode, PeriodTaxYear, PeriodTaxMonth, 
				--						 ProfitCenterCode, FPJGovNo, FPJGovDate, DocNo, DocDate, 
				--						 RefNo, RefDate, CreatedBy, CreatedDate)
				--				  values(''' +@curCompanyMD+ ''',''' +@curUnitBranchMD+ ''',''' 
				--						 +convert(varchar,year(@curDocDate))+ ''','''
				--						 +convert(varchar,month(@curDocDate))+ ''',100,'''
				--						 +@SeriPajakNo+ ''',''' +convert(varchar,@curDocDate,121)+ ''',''' 
				--						 +@INVNo+ ''',''' +convert(varchar,@curDocDate,121)+ ''','''
				--						 +@INVNo+ ''',''' +convert(varchar,@curDocDate,121)+ 
				--						 ''',''POSTING'',''' +@CurrentDate+ ''')'
				--	execute sp_executesql @sqlString

				set @sqlString = 'merge into ' +@DBNameMD+ '..gnGenerateTax as TAX using (values(''' 
				                         +@curCompanyMD+ ''',''' +@curUnitBranchMD+ ''',''' 
										 +convert(varchar,year(@curDocDate))+ ''','''
										 +convert(varchar,month(@curDocDate))+ ''',100,'''
										 +@SeriPajakNo+ ''',''' +convert(varchar,@curDocDate,121)+ ''',''' 
										 +@INVNo+ ''',''' +convert(varchar,@curDocDate,121)+ ''','''
										 +@INVNo+ ''',''' +convert(varchar,@curDocDate,121)+ 
										 ''',''POSTING'',''' +@CurrentDate+ '''))
					    as SRC (NewCompany, NewBranch, NewPeriodTaxYear, NewPeriodTaxMonth, 
								NewProfitCenterCode, NewFPJGovNo, NewFPJGovDate, NewDocNo, 
								NewDocDate, NewRefNo, NewRefDate, NewCreatedBy, NewCreatedDate)
						on TAX.CompanyCode      = SRC.NewCompany
					   and TAX.BranchCode       = SRC.NewBranch
					   and TAX.PeriodTaxYear    = SRC.NewPeriodTaxYear
					   and TAX.PeriodTaxMonth	= SRC.NewPeriodTaxMonth
					   and TAX.ProfitCenterCode = SRC.NewProfitCenterCode
					   and TAX.FPJGovNo         = SRC.NewFPJGovNo
					   and TAX.DocNo            = SRC.NewDocNo
					  when matched 
						   then update set CreatedBy   = ''' +@curLastUpdateBy+ '''
										 , CreatedDate = ''' +convert(varchar,@curLastUpdateDate,121)+ '''
					  when not matched by target 
						   then insert (CompanyCode, BranchCode, PeriodTaxYear, PeriodTaxMonth, 
										ProfitCenterCode, FPJGovNo, FPJGovDate, DocNo, DocDate, 
										RefNo, RefDate, CreatedBy, CreatedDate)
								values (NewCompany, NewBranch, NewPeriodTaxYear, NewPeriodTaxMonth, 
										NewProfitCenterCode, NewFPJGovNo, NewFPJGovDate, NewDocNo, 
										NewDocDate, NewRefNo, NewRefDate, NewCreatedBy, NewCreatedDate);'
					execute sp_executesql @sqlString

			 -- SD: Insert data to table omTrPurchaseBPUDetailModel
				set @sqlString = 'merge into ' +@DBName+ '..omTrPurchaseBPUDetailModel as BPU using (values(''' 
									+@curCompanyCode+ ''',''' +@CentralBranch+ ''',''' +@PONo+ ''',''' 
									+@BPUNo+ ''',''' +@curSalesModelCode+ ''',''' +convert(varchar,@curSalesModelYear)+ 
									''',1,1,''' +@curCreatedBy+ ''',''' +convert(varchar,@curCreatedDate,121)+ 
									''',''' +@curLastUpdateBy+ ''',''' +convert(varchar,@curLastUpdateDate,121)+ '''))
					    as SRC (NewCompany, NewBranch, NewPONo, NewBPUNo, NewSalesModelCode, NewSalesModelYear, NewQuantityBPU, 
								NewQuantityHPP, NewCreatedBy, NewCreatedDate, NewLastUpdateBy, NewLastUpdateDate)
						on BPU.CompanyCode    = SRC.NewCompany
					   and BPU.BranchCode     = SRC.NewBranch
					   and BPU.PONo           = SRC.NewPONo
					   and BPU.BPUNo		  = SRC.NewBPUNo
					   and BPU.SalesModelCode = SRC.NewSalesModelCode
					   and BPU.SalesModelYear = SRC.NewSalesModelYear
					  when matched 
						   then update set QuantityBPU    = QuantityBPU + SRC.NewQuantityBPU
										 , QuantityHPP    = QuantityHPP + SRC.NewQuantityHPP
										 , LastUpdateBy   = ''' +@curLastUpdateBy+ '''
										 , LastUpdateDate = ''' +convert(varchar,@curLastUpdateDate,121)+ '''
					  when not matched by target 
						   then insert (CompanyCode, BranchCode, PONo, BPUNo, SalesModelCode, SalesModelYear, QuantityBPU, 
										QuantityHPP, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
								values (NewCompany, NewBranch, NewPONo, NewBPUNo, NewSalesModelCode, NewSalesModelYear, NewQuantityBPU, 
										NewQuantityHPP, NewCreatedBy, NewCreatedDate, NewLastUpdateBy, NewLastUpdateDate);'
					execute sp_executesql @sqlString
							
			 -- SD: Insert data to table omTrPurchaseBPUDetail
				set @sqlString = 'insert into ' +@DBName+ '..omTrPurchaseBPUDetail 
										(CompanyCode, BranchCode, PONo, BPUNo, BPUSeq, SalesModelCode, SalesModelYear, ColourCode,
										 ChassisCode, ChassisNo, EngineCode, EngineNo, ServiceBookNo, KeyNo, Remark, StatusSJRel,
										 SJRelNo, SJRelDate, SJRelReff, SJRelReffDate, StatusDORel, DORelNo, DORelDate, StatusHPP,
										 isReturn, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
								  values(''' +@curCompanyCode+ ''',''' +@CentralBranch+ ''',''' +@PONo+ ''',''' +@BPUNo+ ''','''
										 +convert(varchar,@SeqNo)+ ''',''' +@curSalesModelCode+ ''',''' 
										 +convert(varchar,@curSalesModelYear)+ ''',''' +@curColourCode+ ''','''
										 +@curChassisCode+ ''',''' +convert(varchar,@curChassisNo)+ ''','''
										 +@curEngineCode+ ''',''' +convert(varchar,@curEngineNo)+ ''','''
										 +@VehServiceBookNo+ ''',''' +@VehKeyNo+ ''',''' +@curDocNo+ 
										 ''',0,NULL,NULL,NULL,NULL,0,NULL,NULL,1,0,'''
										 +@curCreatedBy+ ''',''' +convert(varchar,@curCreatedDate,121)+ 
									''',''' +@curLastUpdateBy+ ''',''' +convert(varchar,@curLastUpdateDate,121)+''')'
					execute sp_executesql @sqlString

			 -- SD: Insert data to table omTrPurchaseHPPDetail
				set @sqlString = 'merge into ' +@DBName+ '..omTrPurchaseHPPDetail as HPP using (values(''' 
									+@curCompanyCode+ ''',''' +@CentralBranch+ ''',''' +@HPPNo+ ''',''' 
									+@BPUNo+ ''',NULL,''' +@curCreatedBy+ ''',''' +convert(varchar,@curCreatedDate,121)+ 
									''',''' +@curLastUpdateBy+ ''',''' +convert(varchar,@curLastUpdateDate,121)+ '''))
					    as SRC (NewCompany, NewBranch, NewHPPNo, NewBPUNo, NewRemark, 
								NewCreatedBy, NewCreatedDate, NewLastUpdateBy, NewLastUpdateDate)
						on HPP.CompanyCode = SRC.NewCompany
					   and HPP.BranchCode  = SRC.NewBranch
					   and HPP.HPPNo       = SRC.NewHPPNo
					   and HPP.BPUNo	   = SRC.NewBPUNo
					  when matched 
						   then update set LastUpdateBy   = ''' +@curLastUpdateBy+ '''
										 , LastUpdateDate = ''' +convert(varchar,@curLastUpdateDate,121)+ '''
					  when not matched by target 
						   then insert (CompanyCode, BranchCode, HPPNo, BPUNo, Remark,
										CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
								values (NewCompany, NewBranch, NewHPPNo, NewBPUNo, NewRemark, 
										NewCreatedBy, NewCreatedDate, NewLastUpdateBy, NewLastUpdateDate);'
					execute sp_executesql @sqlString
							
			 -- SD: Insert data to table omTrPurchaseHPPDetailModel
				set @sqlString = 'merge into ' +@DBName+ '..omTrPurchaseHPPDetailModel as HPP using (values(''' 
									+@curCompanyCode+ ''',''' +@CentralBranch+ ''',''' +@HPPNo+ ''',''' +@BPUNo+ 
									''',''' +@curSalesModelCode+ ''',''' +convert(varchar,@curSalesModelYear)+ 
									''',1,''' +convert(varchar,@RetailPriceExcludePPN)+ ''',''' 
									+convert(varchar,@DiscPriceExcludePPN)+ ''',''' +convert(varchar,@NetSalesExcludePPN)+ 
									''',''' +convert(varchar,@PPNAfterDisc)+ ''',0,''' +convert(varchar,@NetSalesIncludePPN)+ 
									''',''' +convert(varchar,@PPnBMPaid)+ ''',''' +convert(varchar,@OthersDPP)+ ''','''
									+convert(varchar,@OthersPPn)+ ''',''' +@curDocNo+ ''',''' +@curCreatedBy+ ''',''' 
									+convert(varchar,@curCreatedDate,121)+ ''',''' +@curLastUpdateBy+ ''',''' 
									+convert(varchar,@curLastUpdateDate,121)+ '''))
					    as SRC (NewCompany, NewBranch, NewHPPNo, NewBPUNo, NewSalesModelCode, NewSalesModelYear,
								NewQuantity, NewBeforeDiscDPP, NewDiscExcludePPn, NewAfterDiscDPP, NewAfterDiscPPn,
								NewAfterDiscPPnBM, NewAfterDiscTotal, NewPPnBMPaid, NewOthersDPP, NewOthersPPn,
								NewRemark, NewCreatedBy, NewCreatedDate, NewLastUpdateBy, NewLastUpdateDate)
						on HPP.CompanyCode    = SRC.NewCompany
					   and HPP.BranchCode     = SRC.NewBranch
					   and HPP.HPPNo          = SRC.NewHPPNo
					   and HPP.BPUNo	      = SRC.NewBPUNo
					   and HPP.SalesModelCode = SRC.NewSalesModelCode
					   and HPP.SalesModelYear = SRC.NewSalesModelYear
					  when matched 
						   then update set Quantity       = Quantity + 1
										 , LastUpdateBy   = ''' +@curLastUpdateBy+ '''
										 , LastUpdateDate = ''' +convert(varchar,@curLastUpdateDate,121)+ '''
					  when not matched by target 
						   then insert (CompanyCode, BranchCode, HPPNo, BPUNo, SalesModelCode, SalesModelYear,
										Quantity, BeforeDiscDPP, DiscExcludePPn, AfterDiscDPP, AfterDiscPPn,
										AfterDiscPPnBM, AfterDiscTotal, PPnBMPaid, OthersDPP, OthersPPn,
										Remark, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
								values (NewCompany, NewBranch, NewHPPNo, NewBPUNo, NewSalesModelCode, NewSalesModelYear,
										NewQuantity, NewBeforeDiscDPP, NewDiscExcludePPn, NewAfterDiscDPP, NewAfterDiscPPn,
										NewAfterDiscPPnBM, NewAfterDiscTotal, NewPPnBMPaid, NewOthersDPP, NewOthersPPn,
										NewRemark, NewCreatedBy, NewCreatedDate, NewLastUpdateBy, NewLastUpdateDate);'
					execute sp_executesql @sqlString

			 -- SD: Insert data to table omTrPurchaseHPPSubDetail
				set @sqlString = 'insert into ' +@DBName+ '..omTrPurchaseHPPSubDetail
										(CompanyCode, BranchCode, HPPNo, BPUNo, HPPSeq, SalesModelCode, SalesModelYear,
										ColourCode, ChassisCode, ChassisNo, EngineCode, EngineNo, Remark, isReturn, 
										CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
								  values(''' +@curCompanyCode+ ''',''' +@curBranchCode+ ''',''' +@HPPNo+ ''',''' 
										 +@BPUNo+ ''',''' +convert(varchar,@SeqNo)+ ''',''' +@curSalesModelCode+ ''',''' 
										 +convert(varchar,@curSalesModelYear)+ ''',''' +convert(varchar,@curColourCode)+ 
										 ''',''' +convert(varchar,@curChassisCode)+ ''',''' +convert(varchar,@curChassisNo)+ 
										 ''',''' +@curEngineCode+ ''',''' +convert(varchar,@curEngineNo)+ ''',NULL,0,''' 
										 +@curCreatedBy+ ''',''' +convert(varchar,@curCreatedDate,121)+ ''',''' +
										 @curLastUpdateBy+ ''',''' +convert(varchar,@curLastUpdateDate,121)+ ''')'
					execute sp_executesql @sqlString

			-- SD: Insert data to table omTrInventTransferOutDetail (Transfer Unit Out from Holding to branch)
			    set @VTSeq = @VTSeq + 1
				set @sqlString = 'insert into ' +@DBName+ '..omTrInventTransferOutDetail
										(CompanyCode, BranchCode, TransferOutNo, TransferOutSeq, SalesModelCode, 
												 SalesModelYear, ChassisCode, ChassisNo, EngineCode, EngineNo,
												 ColourCode, Remark, StatusTransferIn, CreatedBy, CreatedDate, 
												 LastUpdateBy, LastUpdateDate)
											values(''' +@curCompanyCode+ ''',''' +@CentralBranch+ ''',''' +@VTONo+ 
											       ''',''' +convert(varchar,@VTSeq)+ ''',''' +@curSalesModelCode+ ''',''' 
												   +convert(varchar,@curSalesModelYear)+ ''',''' +@curChassisCode+
												   ''',''' +convert(varchar,@curChassisNo)+ ''',''' +@curEngineCode+
												   ''',''' +convert(varchar,@curEngineNo)+ ''',''' +@curColourCode+
												   ''',''' +@SeriPajakNo+  ''',1,''POSTING'',''' 
												   +convert(varchar,@PostingDate,111)+ ''',''POSTING'',''' 
												   +@currentDate+ ''')' 
					execute sp_executesql @sqlString

			-- SD: Insert data to table omTrInventTransferIn (Transfer Unit In from Holding to branch)
				set @sqlString = 'insert into ' +@DBName+ '..omTrInventTransferInDetail
										(CompanyCode, BranchCode, TransferInNo, TransferInSeq, SalesModelCode, 
												 SalesModelYear, ChassisCode, ChassisNo, EngineCode, EngineNo,
												 ColourCode, Remark, StatusTransferOut, CreatedBy, CreatedDate, 
												 LastUpdateBy, LastUpdateDate)
											values(''' +@curCompanyCode+ ''',''' +@curBranchCode+ ''',''' +@VTINo+ 
												   ''',''' +convert(varchar,@VTSeq)+ ''',''' +@curSalesModelCode+ ''',''' 
												   +convert(varchar,@curSalesModelYear)+ ''',''' +@curChassisCode+
												   ''',''' +convert(varchar,@curChassisNo)+ ''',''' +@curEngineCode+
												   ''',''' +convert(varchar,@curEngineNo)+ ''',''' +@curColourCode+
												   ''',''' +@SeriPajakNo+  ''','' '',''POSTING'',''' 
												   +convert(varchar,@PostingDate,111)+ ''',''POSTING'',''' 
												   +@currentDate+ ''')'  
					execute sp_executesql @sqlString

			 -- MD: Update data to table omMstVehicle
				--set @sqlString = 'update ' +@DBNameMD+ '..omMstVehicle
				--						set SONo='''+@SONo+ ''', SOReturnNo='''+@RTNNo+ ''', DONo='''+@DONo+ ''', BPKNo='''
				--							+@BPKNo+ ''', InvoiceNo='''+@INVNo+ ''', TransferOutNo='''+@VTONo+ 
				--							''', TransferInNo='''+@VTINo+ ''', BPKDate='''+convert(varchar,@curDocDate,121)+
				--					''' where CompanyCode='''+@curCompanyMD+''' and ChassisCode='''
				--							+@curChassisCode+ ''' and ChassisNo='+convert(varchar,@curChassisNo)
				set @sqlString = 'update ' +@DBNameMD+ '..omMstVehicle
										set SONo='''+@SONo+ ''', SOReturnNo='''+@RTNNo+ ''', DONo='''+@DONo+ ''', BPKNo='''
											+@BPKNo+ ''', InvoiceNo='''+@INVNo+ ''', TransferOutNo='''+@VehVTONo+ 
											''', TransferInNo='''+@VehVTINo+ ''', BPKDate='''+convert(varchar,@curDocDate,121)+
									''' where CompanyCode='''+@curCompanyMD+''' and ChassisCode='''
											+@curChassisCode+ ''' and ChassisNo='+convert(varchar,@curChassisNo)
					execute sp_executesql @sqlString

			 -- SD: Insert / Update data to table omMstVehicle
				set @VehCOGS = isnull(@NetSalesExcludePPN,0) + isnull(@OthersDPP,0)
				--set @sqlString = 'merge into ' +@DBName+ '..omMstVehicle as VEH using (values(''' 
				--					+@curCompanyCode+ ''',''' +@curChassisCode+ ''',''' +convert(varchar,@curChassisNo)+ ''',''' +@curEngineCode+ ''',''' +convert(varchar,@curEngineNo)+ ''',''' +@curSalesModelCode+ ''',''' +convert(varchar,@curSalesModelYear)+ ''',''' 
				--					+@curColourCode+ ''',''' +@VehServiceBookNo+ ''',''' +@VehKeyNo+ ''',''' +convert(varchar,@VehCOGS)+ ''',''' +convert(varchar,@VehCOGSOthers)+ ''',''' +convert(varchar,@VehCOGSKaroseri)+ ''',''' +convert(varchar,@VehPpnBMBuyPaid)+ 
				--					''',''' +convert(varchar,@VehPpnBmBuy)+ ''',''' +convert(varchar,@VehSalesNetAmt)+ ''',''' +convert(varchar,@VehPpnBmSellPaid)+ ''',''' +convert(varchar,@VehPpnBmSell)+ ''',''' +@PONo+ ''',''' +@RTPNo+ ''',''' +@BPUNo+ ''',''' +@HPPNo+ 
				--					''','''','''',''' +@VehSONo+ ''',''' +@VehRTNNo+ ''',''' +@VehDONo+ ''',''' +@VehBPKNo+ ''',''' +@VehINVNo+ ''',''' +@VehREQNo+ ''',''' +@VehVTONo+ ''',''' +@VehVTINo+ ''',''' +@WHSD+ ''',''' +@VehINVNo+ ''',''6'',0,''' 
				--					+convert(varchar,@curDocDate,121)+ ''',''' +@VehFakturPolisiNo+ ''',''' +convert(varchar,@VehFakturPolisiDate,121)+ ''','''','''',1,0,1,''' +@curCreatedBy+ ''',''' +convert(varchar,@curCreatedDate,121)+ ''',''' +@curLastUpdateBy+ 
				--					''',''' +convert(varchar,@curLastUpdateDate,121)+ ''',0,'''',''' +convert(varchar,@curDocDate,121)+ ''',0,''' +convert(varchar,@curDocDate,121)+ ''',''' +@VehSuzukiDONo+ ''',''' +convert(varchar,@VehSuzukiDODate,121)+ ''','''
				--					+@VehSuzukiSJNo+ ''',''' +convert(varchar,@VehSuzukiSJDate,121)+ ''','''',''''))
				--	    as SRC (NCompany, NChassisCode, NChassisNo, NEngineCode, NEngineNo, NSalesModelCode, NSalesModelYear, NColourCode, NServiceBookNo, NKeyNo, NCOGSUnit, NCOGSOthers, NCOGSKaroseri, NPpnBMBuyPaid, NPpnBmBuy, NSalesNetAmt, NPpnBmSellPaid, NPpnBmSell, NPONo, NPOReturnNo, NBPUNo, NHPPNo, NKaroseriSPKNo, NKaroseriTerimaNo, NSONo, NSOReturnNo, NDONo, NBPKNo, NInvoiceNo, NReqOutNo, NTransferOutNo, NTransferInNo, NWarehouseCode, 
				--	            NRemark, NStatus, NIsAlreadyPDI, NBPKDate, NFakturPolisiNo, NFakturPolisiDate, NPoliceRegistrationNo, NPoliceRegistrationDate, NIsProfitCenterSales, NIsProfitCenterService, NIsActive, NCreatedBy, NCreatedDate, NLastUpdateBy, NLastUpdateDate, NIsLocked, NLockedBy, NLockedDate, NIsNonRegister, NBPUDate, NSuzukiDONo, NSuzukiDODate, NSuzukiSJNo, NSuzukiSJDate, NTransferOutMultiBranchNo, NTransferInMultiBranchNo)
				--		on VEH.CompanyCode = SRC.NCompany and VEH.ChassisCode = SRC.NChassisCode and VEH.ChassisNo   = SRC.NChassisNo
				--	  when matched then update set PONO='''+@PONo+''', POReturnNo='''+@RTPNo+''', BPUNo='''+@BPUNo+''', HPPNo='''+@HPPNo+''', SONo='''+@VehSONo+''', SOReturnNo='''+@VehRTNNo+''', DONo='''+@VehDONo+''', BPKNo='''+@VehBPKNo+''', InvoiceNo='''+@VehINVNo+''', TransferOutNo='''+@VehVTONo+''', TransferInNo='''+@VehVTINo+'''
				--						 , BPKDate='''+convert(varchar,@VehBPKDate,121)+''', BPUDate='''+convert(varchar,@VehBPUDate,121)+''', SuzukiDONo='''+@VehSuzukiDONo+''', SuzukiDODate='''+convert(varchar,@VehSuzukiDODate,121)+''', SuzukiSJNo='''+@VehSuzukiSJNo+''', SuzukiSJDate='''+convert(varchar,@VehSuzukiSJDate,121)+
				--						 ''', FakturPolisiNo='''+@VehFakturPolisiNo+''', FakturPolisiDate='''+convert(varchar,@VehFakturPolisiDate,121)+''', LastUpdateBy=''' +@curLastUpdateBy+ ''', LastUpdateDate=''' +convert(varchar,@curLastUpdateDate,121)+ '''
				--	  when not matched by target 
				--		   then insert (CompanyCode, ChassisCode, ChassisNo, EngineCode, EngineNo, SalesModelCode, SalesModelYear, ColourCode, ServiceBookNo, KeyNo, COGSUnit, COGSOthers, COGSKaroseri, PpnBMBuyPaid, PpnBmBuy, SalesNetAmt, PpnBmSellPaid, PpnBmSell, PONo, POReturnNo, BPUNo, HPPNo, KaroseriSPKNo, KaroseriTerimaNo, SONo, SOReturnNo, DONo, BPKNo, InvoiceNo, ReqOutNo, TransferOutNo, TransferInNo, WarehouseCode, Remark, Status, IsAlreadyPDI, BPKDate, FakturPolisiNo, 
				--						FakturPolisiDate, PoliceRegistrationNo, PoliceRegistrationDate, IsProfitCenterSales, IsProfitCenterService, IsActive, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate, IsLocked, LockedBy, LockedDate, IsNonRegister, BPUDate, SuzukiDONo, SuzukiDODate, SuzukiSJNo, SuzukiSJDate, TransferOutMultiBranchNo, TransferInMultiBranchNo)
				--				values (NCompany, NChassisCode, NChassisNo, NEngineCode, NEngineNo, NSalesModelCode, NSalesModelYear, NColourCode, NServiceBookNo, NKeyNo, NCOGSUnit, NCOGSOthers, NCOGSKaroseri, NPpnBMBuyPaid, NPpnBmBuy, NSalesNetAmt, NPpnBmSellPaid, NPpnBmSell, NPONo, NPOReturnNo, NBPUNo, NHPPNo, NKaroseriSPKNo, NKaroseriTerimaNo, NSONo, NSOReturnNo, NDONo, NBPKNo, NInvoiceNo, NReqOutNo, NTransferOutNo, NTransferInNo, NWarehouseCode, 
				--						NRemark, NStatus, NIsAlreadyPDI, NBPKDate, NFakturPolisiNo, NFakturPolisiDate, NPoliceRegistrationNo, NPoliceRegistrationDate, NIsProfitCenterSales, NIsProfitCenterService, NIsActive, NCreatedBy, NCreatedDate, NLastUpdateBy, NLastUpdateDate, NIsLocked, NLockedBy, NLockedDate, NIsNonRegister, NBPUDate, NSuzukiDONo, NSuzukiDODate, NSuzukiSJNo, NSuzukiSJDate, NTransferOutMultiBranchNo, NTransferInMultiBranchNo);'
				set @sqlString = 'merge into ' +@DBName+ '..omMstVehicle as VEH using (values(''' 
									+@curCompanyCode+ ''',''' +@curChassisCode+ ''',''' +convert(varchar,@curChassisNo)+ ''',''' +@curEngineCode+ ''',''' +convert(varchar,@curEngineNo)+ ''',''' +@curSalesModelCode+ ''',''' +convert(varchar,@curSalesModelYear)+ ''',''' 
									+@curColourCode+ ''',''' +@VehServiceBookNo+ ''',''' +@VehKeyNo+ ''',''' +convert(varchar,@VehCOGS)+ ''',''' +convert(varchar,@VehCOGSOthers)+ ''',''' +convert(varchar,@VehCOGSKaroseri)+ ''',''' +convert(varchar,@VehPpnBMBuyPaid)+ 
									''',''' +convert(varchar,@VehPpnBmBuy)+ ''',''' +convert(varchar,@VehSalesNetAmt)+ ''',''' +convert(varchar,@VehPpnBmSellPaid)+ ''',''' +convert(varchar,@VehPpnBmSell)+ ''',''' +@PONo+ ''',''' +@RTPNo+ ''',''' +@BPUNo+ ''',''' +@HPPNo+ 
									''','''','''',''' +@VehSONo+ ''',''' +@VehRTNNo+ ''',''' +@VehDONo+ ''',''' +@VehBPKNo+ ''',''' +@VehINVNo+ ''',''' +@VehREQNo+ ''',''' +@VTONo+ ''',''' +@VTINo+ ''',''' +@WHSD+ ''',''' +@VehINVNo+ ''',''6'',0,''' 
									+convert(varchar,@curDocDate,121)+ ''',''' +@VehFakturPolisiNo+ ''',''' +convert(varchar,@VehFakturPolisiDate,121)+ ''','''','''',1,0,1,''' +@curCreatedBy+ ''',''' +convert(varchar,@curCreatedDate,121)+ ''',''' +@curLastUpdateBy+ 
									''',''' +convert(varchar,@curLastUpdateDate,121)+ ''',0,'''',''' +convert(varchar,@curDocDate,121)+ ''',0,''' +convert(varchar,@curDocDate,121)+ ''',''' +@VehSuzukiDONo+ ''',''' +convert(varchar,@VehSuzukiDODate,121)+ ''','''
									+@VehSuzukiSJNo+ ''',''' +convert(varchar,@VehSuzukiSJDate,121)+ ''','''',''''))
					    as SRC (NCompany, NChassisCode, NChassisNo, NEngineCode, NEngineNo, NSalesModelCode, NSalesModelYear, NColourCode, NServiceBookNo, NKeyNo, NCOGSUnit, NCOGSOthers, NCOGSKaroseri, NPpnBMBuyPaid, NPpnBmBuy, NSalesNetAmt, NPpnBmSellPaid, NPpnBmSell, NPONo, NPOReturnNo, NBPUNo, NHPPNo, NKaroseriSPKNo, NKaroseriTerimaNo, NSONo, NSOReturnNo, NDONo, NBPKNo, NInvoiceNo, NReqOutNo, NTransferOutNo, NTransferInNo, NWarehouseCode, 
					            NRemark, NStatus, NIsAlreadyPDI, NBPKDate, NFakturPolisiNo, NFakturPolisiDate, NPoliceRegistrationNo, NPoliceRegistrationDate, NIsProfitCenterSales, NIsProfitCenterService, NIsActive, NCreatedBy, NCreatedDate, NLastUpdateBy, NLastUpdateDate, NIsLocked, NLockedBy, NLockedDate, NIsNonRegister, NBPUDate, NSuzukiDONo, NSuzukiDODate, NSuzukiSJNo, NSuzukiSJDate, NTransferOutMultiBranchNo, NTransferInMultiBranchNo)
						on VEH.CompanyCode = SRC.NCompany and VEH.ChassisCode = SRC.NChassisCode and VEH.ChassisNo   = SRC.NChassisNo
					  when matched then update set PONO='''+@PONo+''', POReturnNo='''+@RTPNo+''', BPUNo='''+@BPUNo+''', HPPNo='''+@HPPNo+''', SONo='''+@VehSONo+''', SOReturnNo='''+@VehRTNNo+''', DONo='''+@VehDONo+''', BPKNo='''+@VehBPKNo+''', InvoiceNo='''+@VehINVNo+''', TransferOutNo='''+@VTONo+''', TransferInNo='''+@VTINo+'''
										 , BPKDate='''+convert(varchar,@VehBPKDate,121)+''', BPUDate='''+convert(varchar,@VehBPUDate,121)+''', SuzukiDONo='''+@VehSuzukiDONo+''', SuzukiDODate='''+convert(varchar,@VehSuzukiDODate,121)+''', SuzukiSJNo='''+@VehSuzukiSJNo+''', SuzukiSJDate='''+convert(varchar,@VehSuzukiSJDate,121)+
										 ''', FakturPolisiNo='''+@VehFakturPolisiNo+''', FakturPolisiDate='''+convert(varchar,@VehFakturPolisiDate,121)+''', LastUpdateBy=''' +@curLastUpdateBy+ ''', LastUpdateDate=''' +convert(varchar,@curLastUpdateDate,121)+ '''
					  when not matched by target 
						   then insert (CompanyCode, ChassisCode, ChassisNo, EngineCode, EngineNo, SalesModelCode, SalesModelYear, ColourCode, ServiceBookNo, KeyNo, COGSUnit, COGSOthers, COGSKaroseri, PpnBMBuyPaid, PpnBmBuy, SalesNetAmt, PpnBmSellPaid, PpnBmSell, PONo, POReturnNo, BPUNo, HPPNo, KaroseriSPKNo, KaroseriTerimaNo, SONo, SOReturnNo, DONo, BPKNo, InvoiceNo, ReqOutNo, TransferOutNo, TransferInNo, WarehouseCode, Remark, Status, IsAlreadyPDI, BPKDate, FakturPolisiNo, 
										FakturPolisiDate, PoliceRegistrationNo, PoliceRegistrationDate, IsProfitCenterSales, IsProfitCenterService, IsActive, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate, IsLocked, LockedBy, LockedDate, IsNonRegister, BPUDate, SuzukiDONo, SuzukiDODate, SuzukiSJNo, SuzukiSJDate, TransferOutMultiBranchNo, TransferInMultiBranchNo)
								values (NCompany, NChassisCode, NChassisNo, NEngineCode, NEngineNo, NSalesModelCode, NSalesModelYear, NColourCode, NServiceBookNo, NKeyNo, NCOGSUnit, NCOGSOthers, NCOGSKaroseri, NPpnBMBuyPaid, NPpnBmBuy, NSalesNetAmt, NPpnBmSellPaid, NPpnBmSell, NPONo, NPOReturnNo, NBPUNo, NHPPNo, NKaroseriSPKNo, NKaroseriTerimaNo, NSONo, NSOReturnNo, NDONo, NBPKNo, NInvoiceNo, NReqOutNo, NTransferOutNo, NTransferInNo, NWarehouseCode, 
										NRemark, NStatus, NIsAlreadyPDI, NBPKDate, NFakturPolisiNo, NFakturPolisiDate, NPoliceRegistrationNo, NPoliceRegistrationDate, NIsProfitCenterSales, NIsProfitCenterService, NIsActive, NCreatedBy, NCreatedDate, NLastUpdateBy, NLastUpdateDate, NIsLocked, NLockedBy, NLockedDate, NIsNonRegister, NBPUDate, NSuzukiDONo, NSuzukiDODate, NSuzukiSJNo, NSuzukiSJDate, NTransferOutMultiBranchNo, NTransferInMultiBranchNo);'
					execute sp_executesql @sqlString
--print @sqlString
--select @DBName DBName, @curCompanyCode CompanyCode, @curChassisCode ChassisCode, @curChassisNo ChassisNo,
--	   @curEngineCode EngineCode, @curEngineNo EngineNo, @curSalesModelCode SalesModelCode, 
--	   @curSalesModelYear SalesModelYear, @curColourCode ColourCode, @VehServiceBookNo VehServiceBookNo, 
--	   @VehKeyNo VehKeyNo, @VehCOGS VehCOGS, @VehCOGSOthers VehCOGSOthers, @VehCOGSKaroseri VehCOGSKaroseri,
--	   @VehPpnBMBuyPaid VehPpnBMBuyPaid, @VehPpnBmBuy VehPpnBmBuy, @VehSalesNetAmt VehSalesNetAmt,
--	   @VehPpnBmSellPaid VehPpnBmSellPaid, @VehPpnBmSell VehPpnBmSell, @PONo PONo, @RTPNo RTPNo, @BPUNo BPUNo,
--	   @HPPNo HPPNo, @VehSONo VehSONo, @VehRTNNo VehRTNNo, @VehDONo VehDONo, @VehBPKNo VehBPKNo, 
--	   @VehINVNo VehINVNo, @VehREQNo VehREQNo, @VehVTONo VehVTONo, @VehVTINo VehVTINo, @WHSD WHSD, 
--	   @VehINVNo VehINVNo, @curDocDate DocDate, @VehFakturPolisiNo VehFakturPolisiNo, 
--	   @VehFakturPolisiDate VehFakturPolisiDate, @curCreatedBy CreatedBy, @curCreatedDate CreatedDate,
--	   @curLastUpdateBy LastUpdateBy, @curLastUpdateDate LastUpdateDate, @VehSuzukiDONo VehSuzukiDONo, 
--	   @VehSuzukiDODate VehSuzukiDODate, @VehSuzukiSJNo VehSuzukiSJNo, @VehSuzukiSJDate VehSuzukiSJDate
					
					
			 -- SD: Insert / Update data to table omTrInventQtyVehicle
				set @sqlString = 'merge into ' +@DBName+ '..omTrInventQtyVehicle as VEH using (values(''' 
									+@curCompanyCode+ ''',''' +@curBranchCode+ ''',''' 
									+convert(varchar,year(@curDocDate))+ ''',''' 
									+convert(varchar,month(@curDocDate))+  ''',''' 
									+@curSalesModelCode+ ''',''' 
									+convert(varchar,@curSalesModelYear)+ ''',''' 
									+@curColourCode+ ''',''' +@WHSD+ 
									''',1,0,1,0,0,0,0,NULL,1,''' 
									+@curCreatedBy+ ''',''' 
									+convert(varchar,@curCreatedDate,121)+ ''',''' 
									+@curLastUpdateBy+ ''',''' 
									+convert(varchar,@curLastUpdateDate,121)+ 
									''',0,NULL,NULL))
					    as SRC (NewCompany, NewBranch, NewYear, NewMonth, NewSalesModelCode, 
								NewSalesModelYear, NewColourCode, NewWarehouseCode, NewQtyIn, NewAlocation, 
								NewQtyOut, NewBeginningOH, NewEndingOH, NewBeginningAV, NewEndingAV, NewRemark, 
								NewStatus, NewCreatedBy, NewCreatedDate, NewLastUpdateBy, NewLastUpdateDate, 
								NewIsLocked, NewLockedBy, NewLockedDate)
						on VEH.CompanyCode    = SRC.NewCompany
					   and VEH.BranchCode	  = SRC.NewBranch
					   and VEH.Year			  = SRC.NewYear
					   and VEH.Month		  = SRC.NewMonth
					   and VEH.SalesModelCode = SRC.NewSalesModelCode
					   and VEH.SalesModelYear = SRC.NewSalesModelYear 
					   and VEH.ColourCode	  = SRC.NewColourCode
					   and VEH.WarehouseCode  = SRC.NewWarehouseCode
					  when matched
						   then update set QtyIn  = QtyIn  + SRC.NewQtyIn
										 , QtyOut = QtyOut + SRC.NewQtyOut
										 , LastUpdateBy    = ''' +@curLastUpdateBy+ '''
										 , LastUpdateDate  = ''' +convert(varchar,@curLastUpdateDate,121)+ '''
					  when not matched by target 
						   then insert (CompanyCode, BranchCode, Year, Month, SalesModelCode, SalesModelYear, 
										ColourCode, WarehouseCode, QtyIn, Alocation, QtyOut, BeginningOH, 
										EndingOH, BeginningAV, EndingAV, Remark, Status, CreatedBy, 
										CreatedDate, LastUpdateBy, LastUpdateDate, IsLocked, LockedBy, LockedDate)
								values (NewCompany, NewBranch, NewYear, NewMonth, NewSalesModelCode, 
										NewSalesModelYear, NewColourCode, NewWarehouseCode, NewQtyIn, NewAlocation, 
										NewQtyOut, NewBeginningOH, NewEndingOH, NewBeginningAV, NewEndingAV, NewRemark, 
										NewStatus, NewCreatedBy, NewCreatedDate, NewLastUpdateBy, NewLastUpdateDate, 
										NewIsLocked, NewLockedBy, NewLockedDate);'
					execute sp_executesql @sqlString

   			 -- Update Daily Posting Process Status
				update omSDMovement
				   set ProcessStatus=1
				     , ProcessDate  =@CurrentDate
					where CompanyCode=@curCompanyCode
					  and BranchCode =@curBranchCode
					  and DocNo      =@curDocNo
					  and DocDate    =@curDocDate
					  and Seq        =@curSeq

			 -- Read next record
				fetch next from curomSDMovement
					  into  @curCompanyCode, @curBranchCode, @curDocNo, @curDocDate, @curSeq, @curSalesModelCode,
							@curSalesModelYear, @curChassisCode, @curChassisNo, @curEngineCode, @curEngineNo, 
							@curColourCode, @curWarehouseCode, @curCustomerCode, @curQtyFlag, @curCompanyMD, 
							@curBranchMD, @curWarehouseMD, @curStatus, @curProcessStatus, @curProcessDate, 
							@curCreatedBy, @curCreatedDate, @curLastUpdateBy, @curLastUpdateDate
			end 

	 -- Move data which already processed from table omSDMovement to table omHstSDMovement
	    if not exists (select * from sys.objects where object_id = object_id(N'[dbo].[omHstSDMovement]') and type in (N'U'))
			CREATE TABLE [dbo].[omHstSDMovement](
				[CompanyCode] [varchar](15) NOT NULL,
				[BranchCode] [varchar](15) NOT NULL,
				[DocNo] [varchar](15) NOT NULL,
				[DocDate] [datetime] NOT NULL,
				[Seq] [int] NOT NULL,
				[SalesModelCode] [varchar](20) NOT NULL,
				[SalesModelYear] [numeric](4, 0) NOT NULL,
				[ChassisCode] [varchar](15) NOT NULL,
				[ChassisNo] [numeric](10, 0) NOT NULL,
				[EngineCode] [varchar](15) NOT NULL,
				[EngineNo] [numeric](10, 0) NOT NULL,
				[ColourCode] [varchar](15) NOT NULL,
				[WarehouseCode] [varchar](15) NOT NULL,
				[CustomerCode] [varchar](15) NOT NULL,
				[QtyFlag] [char](1) NOT NULL,
				[CompanyMD] [varchar](15) NOT NULL,
				[BranchMD] [varchar](15) NOT NULL,
				[WarehouseMD] [varchar](15) NOT NULL,
				[Status] [char](1) NOT NULL,
				[ProcessStatus] [char](1) NOT NULL,
				[ProcessDate] [datetime] NOT NULL,
				[CreatedBy] [varchar](15) NOT NULL,
				[CreatedDate] [datetime] NOT NULL,
				[LastUpdateBy] [varchar](15) NOT NULL,
				[LastUpdateDate] [datetime] NOT NULL)
			
		insert into omHstSDMovement (CompanyCode, BranchCode, DocNo, DocDate, Seq, SalesModelCode,
								     SalesModelYear, ChassisCode, ChassisNo, EngineCode, EngineNo,
									 ColourCode, WarehouseCode, CustomerCode, QtyFlag, CompanyMD, 
									 BranchMD, WarehouseMD, Status, ProcessStatus, ProcessDate,
									 CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
							 select  CompanyCode, BranchCode, DocNo, DocDate, Seq, SalesModelCode,
								     SalesModelYear, ChassisCode, ChassisNo, EngineCode, EngineNo,
									 ColourCode, WarehouseCode, CustomerCode, QtyFlag, CompanyMD, 
									 BranchMD, WarehouseMD, Status, ProcessStatus, ProcessDate,
									 CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate
							   from  omSDMovement
							  where	 ProcessStatus = 1
							     or  (left(docno,2) not in ('RT','RS')
                                and  convert(varchar,DocDate,111)<=convert(varchar,@PostingDate,111))

		delete omSDMovement   where	 ProcessStatus = 1
							     or  (left(docno,2) not in ('RT','RS')
                                and  convert(varchar,DocDate,111)<=convert(varchar,@PostingDate,111))

		close curomSDMovement
		deallocate curomSDMovement

END

GO
if object_id('usprpt_PostingMultiCompanySalesReturn') is not null
	drop procedure usprpt_PostingMultiCompanySalesReturn
GO
-- POSTING TRANSACTION MULTI COMPANY - SALES RETURN
-- -----------------------------------------------------------------------------
-- Created  by : HTO, 2014
-- Revision-01 : HTO, January 2015
-- Revision-02 : HTO, April 2015 (Process by Date)
-- -----------------------------------------------------------------------------
-- This procedure used for 
--		4-Wheeler : BIT-SBT, SBSM-SSBT, BAT-SAT, SIT-SST, UIB-SIT
--		2-Wheeler : IJMG,SVMG,ISG,ISMG-SMG
-- -----------------------------------------------------------------------------
-- execute [usprpt_PostingMultiCompanySalesReturn] '6006400001','2014/11/30','0'
-- -----------------------------------------------------------------------------

CREATE procedure [dbo].[usprpt_PostingMultiCompanySalesReturn]
	@CompanyCode	varchar(15),
	@PostingDate	datetime,
	@Status			varchar(1)	output
AS	

BEGIN

	  --declare @CompanyCode			varchar(15) = '6006400001'
	  --declare @PostingDate			datetime	= '2014/04/30'
	  --declare @Status					varchar(1)

 -- Posting process : insert data to MD & SD table
		declare @curCompanyCode			varchar(15) --= '6006400001'
		declare @curBranchCode			varchar(15) --= '6006400131'
		declare @curDocNo				varchar(15)	--= 'IU131/14/100010'
		declare @curDocDate				datetime    --= '2015/03/08'
		declare @curSeq					int			
		declare @curSalesModelCode		varchar(20)	
		declare @curSalesModelYear		numeric(4,0)	
		declare @curChassisCode			varchar(15)	
		declare @curChassisNo			numeric(10,0) 
		declare @curEngineCode			varchar(15)	
		declare @curEngineNo			numeric(10,0) 
		declare @curColourCode			varchar(15)	
		declare @curWarehouseCode		varchar(15)	
		declare @curCustomerCode		varchar(15)	
		declare @curQtyFlag				char(1)		
		declare @curCompanyMD			varchar(15)
		declare @curBranchMD			varchar(15)	
		declare @curUnitBranchMD		varchar(15)	
		declare @curWarehouseMD			varchar(15)	
		declare @curStatus				char(1)		
		declare @curProcessStatus		char(1)		
		declare @curProcessDate			datetime		
		declare @curCreatedBy			varchar(15)	
		declare @curCreatedDate			datetime		
		declare @curLastUpdateBy		varchar(15)	
		declare @curLastUpdateDate		datetime
		declare @sqlString				nvarchar(max)

	 -- MD
		declare @HPPNo					varchar(15)
		declare @INVNo					varchar(15)
		declare @FPJNo					varchar(25)
		declare @BPKNo					varchar(15)
		declare @BPUNo					varchar(15)
		declare @HPPDate				datetime
		declare @INVDate				datetime
		declare @FPJDate				datetime
	 -- SD
		declare @VTONo					varchar(15)
		declare @VTINo					varchar(15)
		declare @RTPNo					varchar(15)
		declare @RTSNo					varchar(15)

		declare @WHFrom					varchar(15)
		declare @WHTo					varchar(15)
		declare @RemarkHdr				varchar(100)
		declare @RemarkDtl				varchar(100)
		declare @BeforeDiscDPP			numeric(18,0)
		declare @DiscExcludePPn			numeric(18,0)
		declare @AfterDiscDPP			numeric(18,0)
		declare @AfterDiscPPn			numeric(18,0)
		declare @AfterDiscPPnBM			numeric(18,0)
		declare @AfterDiscTotal			numeric(18,0)
		declare @OthersDPP				numeric(18,0)
		declare @OthersPPn				numeric(18,0)

		declare @DBName					varchar(50)
		declare @DBNameMD				varchar(50)
		declare @CentralBranch			varchar(15)
		declare @SeqNo					integer
		declare @swCompanyCode			varchar(15)  = ' '
		declare @swBranchCode			varchar(15)  = ' '
		declare @swDocNo 				varchar(15)  = ' '
		declare @CurrentDate			varchar(100) = convert(varchar,getdate(),121)

        declare curomSDMovement cursor for
			select sd.* 
			  from omSDMovement sd, gnMstDocument doc
      --       where sd.CompanyMD        =doc.CompanyCode
			   --and sd.BranchMD         =doc.BranchCode
			   --and doc.DocumentType    ='RTS' 
			   --and doc.ProfitCenterCode='100'
			   --and left(sd.DocNo,len(doc.DocumentPrefix))=doc.DocumentPrefix
             where left(DocNo,2) in ('RT','RS') 
			   and convert(varchar,DocDate,111)<=convert(varchar,@PostingDate,111)
			   and ProcessStatus=0
             order by convert(varchar,sd.DocDate,111), sd.CompanyCode, sd.BranchCode, sd.DocNo, sd.SalesModelCode, 
					  sd.SalesModelYear, sd.ColourCode, sd.ChassisCode, sd.ChassisNo
		open curomSDMovement

		fetch next from curomSDMovement
			  into @curCompanyCode, @curBranchCode, @curDocNo, @curDocDate, @curSeq, @curSalesModelCode,
				   @curSalesModelYear, @curChassisCode, @curChassisNo, @curEngineCode, @curEngineNo, 
				   @curColourCode, @curWarehouseCode, @curCustomerCode, @curQtyFlag, @curCompanyMD, 
				   @curBranchMD, @curWarehouseMD, @curStatus, @curProcessStatus, @curProcessDate, 
				   @curCreatedBy, @curCreatedDate, @curLastUpdateBy, @curLastUpdateDate
				  
		while (@@FETCH_STATUS =0)
			begin

			 -- MD Database & SD Database from gnMstCompanyMapping
				select @DBNameMD=DBMD, @DBName=DBName, @curUnitBranchMD=UnitBranchMD from gnMstCompanyMapping where CompanyCode=@CurCompanyCode and BranchCode =@curBranchCode

			 -- Centralize unit Purchasing & Transfer for SBT, SMG, SIT, SST dealer
				set @sqlString = N'select @CentralBranch=BranchCode from ' +@DBName+ '..gnMstOrganizationDtl ' +  
								   'where CompanyCode=''' +@curCompanyCode+ ''' and isBranch=''0'''
					execute sp_executesql @sqlString, 
							N'@CentralBranch varchar(15) output', @CentralBranch output
										
			 -- SD: Collect HPP, FPJ & INV (No & Date) information from omTrPurchaseHPP, omTrPurchaseHPPSubDetail & omTrSalesInvoice
				set @sqlString = N'select @HPPNo=h.HPPNo, @HPPDate=h.HPPDate, @FPJNo=h.RefferenceFakturPajakNo, ' +
										 '@FPJDate=h.RefferenceFakturPajakDate, @INVNo=h.RefferenceInvoiceNo, ' +
										 '@INVDate=h.RefferenceInvoiceDate, @BPUNo=d.BPUNo from ' +
										 +@DBName+ '..omTrPurchaseHPP h, ' +@DBName+ '..omTrPurchaseHPPSubDetail d ' +
										 'where h.CompanyCode=d.CompanyCode and h.BranchCode=d.BranchCode and h.HPPNo=d.HPPNo ' +
										   'and d.CompanyCode=''' +@curCompanyCode+ ''' and d.BranchCode=''' +@CentralBranch+ 
										''' and d.ChassisCode=''' +@curChassisCode+ ''' and d.ChassisNo=''' +convert(varchar,@curChassisNo)+ ''''
					execute sp_executesql @sqlString, 
							N'@HPPNo varchar(15) output, @HPPDate datetime output, 
							  @FPJNo varchar(25) output, @FPJDate datetime output,
							  @INVNo varchar(15) output, @INVDate datetime output,
							  @BPUNo varchar(15) output', 
							  @HPPNo output,             @HPPDate output, 
							  @FPJNo output,             @FPJDate output,
							  @INVNo output,			 @INVDate output,
							  @BPUNo output

				set @HPPNo   = isnull(@HPPNo,'')
				set @FPJNo   = isnull(@FPJNo,'')
				set @INVNo   = isnull(@INVNo,'')
				set @BPUNo   = isnull(@BPUNo,'')
				set @HPPDate = isnull(@HPPDate,'1900/01/01')
				set @FPJDate = isnull(@FPJDate,'1900/01/01')
				set @INVDate = isnull(@INVDate,'1900/01/01')

				if @swCompanyCode <> @curCompanyCode or
				   @swBranchCode  <> @curBranchCode  or
				   @swDocNo		  <> @curDocNo
					begin
						set @swCompanyCode = @curCompanyCode
						set @swBranchCode  = @curBranchCode
						set @swDocNo	   = @curDocNo
						set @SeqNo		   = 0
		
					 -- SD: Collect Warehouse From & Remark Header information from Retur Sales
						set @sqlString = N'select @WHFrom=WareHouseCode, @RemarkHdr=Remark from ' +@DBName+ '..omTrSalesReturn ' +  
											'where CompanyCode=''' +@curCompanyCode+ ''' and BranchCode=''' +@curBranchCode+
											''' and ReturnNo=''' +@curDocNo+''''
							execute sp_executesql @sqlString, 
									N'@WHFrom varchar(15) output, @RemarkHdr varchar(100) output', @WHFrom output, @RemarkHdr output

				     -- SD: Collect Warehouse To Information from gnMstLookupDtl
						set @sqlString = N'select @WHTO=LookUpValue from ' +@DBName+ '..gnMstLookUpDtl 
											where CompanyCode=''' +@curCompanyCode+ ''' and CodeID=''MPWH'' and ParaValue=''' 
											+@CentralBranch+ ''''
							execute sp_executesql @sqlString, N'@WHTO varchar(15) output', @WHTO output

						set @RemarkHdr = isnull(@RemarkHdr,'')
						set @WHFrom    = isnull(@WHFrom,'')
						set @WHTo      = isnull(@WHTo,'')

						if @curBranchCode <> @CentralBranch
							begin
							 -- SD: Insert data to table omTrInventTransferOut
								execute [usprpt_PostingMultiCompany4DocNo] @curCompanyCode, @curBranchCode, @DBName, 'VTO', @VTONo output

								set @sqlString = 'insert into ' +@DBName+ '..omTrInventTransferOut 
														(CompanyCode, BranchCode, TransferOutNo, TransferOutDate, ReferenceNo, 
														 ReferenceDate, BranchCodeFrom, WareHouseCodeFrom, BranchCodeTo, 
														 WareHouseCodeTo, ReturnDate, Remark, Status, CreatedBy, CreatedDate, 
														 LastUpdateBy, LastUpdateDate, isLocked, LockedBy, LockedDate)
												  values(''' +@curCompanyCode+ ''',''' +@curBranchCode+ ''',''' 
														 +@VTONo+ ''',''' +convert(varchar,@curDocDate,121)+ ''',''' 
														 +@curDocNo+ ''',''' +convert(varchar,@curDocDate,121)+ ''',''' 
														 +@curBranchCode+ ''',''' +@WHFrom+ ''','''
														 +@CentralBranch+ ''',''' +@WHTO+ ''','''
														 +convert(varchar,@curDocDate,121)+ ''',''' 
														 +@RemarkHdr+ ''',''5'',''POSTING'',''' 
														 +convert(varchar,@PostingDate,111)+ ''',''POSTING'',''' 
														 +@CurrentDate+ ''',0,NULL,NULL)'
									execute sp_executesql @sqlString

							 -- SD: Insert data to table omTrInventTransferIn
								execute [usprpt_PostingMultiCompany4DocNo] @curCompanyCode, @CentralBranch, @DBName, 'VTI', @VTINo output
								set @sqlString = 'insert into ' +@DBName+ '..omTrInventTransferIn 
														(CompanyCode, BranchCode, TransferInNo, TransferInDate, TransferOutNo, 
													     ReferenceNo, ReferenceDate, BranchCodeFrom, WareHouseCodeFrom, 
														 BranchCodeTo, WareHouseCodeTo, ReturnDate, Remark, Status, CreatedBy, 
														 CreatedDate, LastUpdateBy, LastUpdateDate, isLocked, LockedBy, LockedDate)
												  values(''' +@curCompanyCode+ ''',''' +@CentralBranch+ ''','''
														 +@VTINo+ ''',''' +convert(varchar,@curDocDate,121)+ ''',''' 
														 +@VTONo+ ''',''' +@curDocNo+ ''',''' 
														 +convert(varchar,@curDocDate,121)+ ''',''' 
														 +@curBranchCode+ ''',''' +@WHFrom+ ''','''
														 +@CentralBranch+ ''',''' +@WHTO+ ''','''
														 +convert(varchar,@curDocDate,121)+ ''',''' 
														 +@RemarkHdr+ ''',''2'',''POSTING'',''' 
														 +convert(varchar,@PostingDate,111)+ ''',''POSTING'',''' 
														 +@CurrentDate+ ''',0,NULL,NULL)'
									execute sp_executesql @sqlString
							end

					 -- SD: Insert data to table omTrPurchaseReturn
					 	execute [usprpt_PostingMultiCompany4DocNo] @curCompanyCode, @CentralBranch, @DBName, 'RTP', @RTPNo output
						set @sqlString = 'insert into ' +@DBName+ '..omTrPurchaseReturn 
												(CompanyCode, BranchCode, ReturnNo, ReturnDate, RefferenceNo, RefferenceDate, 
												 HPPNo, RefferenceFakturPajakNo, Remark, Status, CreatedBy, CreatedDate, 
												 LastUpdateBy, LastUpdateDate, isLocked, LockingBy, LockingDate)
										  values(''' +@curCompanyCode+ ''',''' +@CentralBranch+ ''',''' 
												 +@RTPNo+ ''',''' +convert(varchar,@curDocDate,121)+ ''',''' 
												 +@HPPNo+ ''',''' +convert(varchar,@HPPDate,121)+ ''',''' 
												 +@HPPNo+ ''',''' +@FPJNo+ ''',''' +@RemarkHdr+ 
												 ''',''5'',''POSTING'',''' +convert(varchar,@PostingDate,111)+ 
												 ''',''POSTING'',''' +@CurrentDate+ ''',0,NULL,NULL)'
							execute sp_executesql @sqlString

					 -- MD: Insert data to table omTrSalesReturn
					 	execute [usprpt_PostingMultiCompany4DocNo] @curCompanyMD, @curUnitBranchMD, @DBNameMD, 'RTS', @RTSNo output
						set @sqlString = 'insert into ' +@DBNameMD+ '..omTrSalesReturn 
												(CompanyCode, BranchCode, ReturnNo, ReturnDate, ReferenceNo, ReferenceDate, 
												 InvoiceNo, InvoiceDate, CustomerCode, FakturPajakNo, FakturPajakDate, 
												 WareHouseCode, Remark, Status, CreatedBy, CreatedDate, LastUpdateBy, 
												 LastUpdateDate, isLocked, LockedBy, LockedDate)
										  values(''' +@curCompanyMD+ ''',''' +@curUnitBranchMD+ ''',''' 
												 +@RTSNo+ ''',''' +convert(varchar,@curDocDate,121)+ ''',''' 
												 +@RTPNo+ ''',''' +convert(varchar,@curDocDate,121)+ ''',''' 
												 +@INVNo+ ''',''' +convert(varchar,@INVDate,121)+ ''','''
												 +@CentralBranch+ ''',''' +@FPJNo+ ''',''' 
												 +convert(varchar,@FPJDate,121)+ ''',''' 
												 +@WHTo+ ''',''' +@RemarkHdr+ ''',''5'',''POSTING'',''' 
												 +convert(varchar,@PostingDate,111)+ ''',''POSTING'',''' 
												 +@CurrentDate+ ''',0,NULL,NULL)'
							execute sp_executesql @sqlString
					end

-----------------------------------------------------------------------------------------  DETAIL PROCESS
				set @SeqNo = @SeqNo + 1

			 -- MD: Update data to table omMstVehicle
				set @sqlString = 'update ' +@DBNameMD+ '..omMstVehicle
										set SOReturnNo='''+@RTSNo+ 
										''', Status=''0'', isActive=''1'' where CompanyCode='''
										+@curCompanyMD+ ''' and ChassisCode=''' 
										+@curChassisCode+ ''' and ChassisNo='
										+convert(varchar,@curChassisNo)+ ''
					execute sp_executesql @sqlString

			 -- SD: Update data to table omMstVehicle
				set @sqlString = 'update ' +@DBName+ '..omMstVehicle
										set POReturnNo='''+@RTPNo+ 
										''', TransferOutNo='''+@VTONo+ 
										''', TransferInNo='''+@VTINo+ 
										''', Status=''2'', isActive=''0''  where CompanyCode='''+@curCompanyMD+
										''' and ChassisCode=''' +@curChassisCode+ 
										''' and ChassisNo='+convert(varchar,@curChassisNo) +''
					execute sp_executesql @sqlString

			 -- SD: Collect Remark Detail information table omTrSalesReturnVIN
				set @sqlString = N'select top 1 @RemarkDtl=Remark from ' +@DBName+ '..omTrSalesReturnVIN ' +  
											'where CompanyCode=''' +@curCompanyCode+ ''' and BranchCode=''' +@curBranchCode+
											''' and ReturnNo=''' +@curDocNo+ ''' and ChassisCode=''' +@curChassisCode+
											''' and ChassisNo=''' +convert(varchar,@curChassisNo)+ ''' order by ReturnNo desc'
							execute sp_executesql @sqlString, 
									N'@RemarkDtl varchar(100) output', @RemarkDtl output
				set @RemarkDtl = isnull(@RemarkDtl,'')

			 -- SD: Transfer unit from branch to holding
				if @curBranchCode <> @CentralBranch
					begin
					 -- SD: Insert data to table omTrInventTransferOutDetail
						set @sqlString = 'insert into ' +@DBName+ '..omTrInventTransferOutDetail 
												(CompanyCode, BranchCode, TransferOutNo, TransferOutSeq, 
												 SalesModelCode, SalesModelYear, ChassisCode, ChassisNo, 
												 EngineCode, EngineNo, ColourCode, Remark, StatusTransferIn, 
												 CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
										  values(''' +@curCompanyCode+ ''',''' +@curBranchCode+ ''',''' 
												 +@VTONo+ ''',''' +convert(varchar,@SeqNo)+ ''','''
												 +@curSalesModelCode+ ''',''' +convert(varchar,@curSalesModelYear)+ ''','''
												 +@curChassisCode+ ''',''' +convert(varchar,@curChassisNo)+ ''','''
												 +@curEngineCode+ ''',''' +convert(varchar,@curEngineNo)+ ''','''
												 +@curColourCode+ ''',''' +@RemarkDtl+ ''',''1'',''' 
												 +@curCreatedBy+ ''',''' +convert(varchar,@curCreatedDate,121)+ ''','''
												 +@curLastUpdateBy+ ''',''' +convert(varchar,@curLastUpdateDate,121)+ ''')'
							execute sp_executesql @sqlString

					 -- SD: Insert data to table omTrInventTransferInDetail
						set @sqlString = 'insert into ' +@DBName+ '..omTrInventTransferInDetail 
												(CompanyCode, BranchCode, TransferInNo, TransferInSeq, 
												 SalesModelCode, SalesModelYear, ChassisCode, ChassisNo, 
												 EngineCode, EngineNo, ColourCode, Remark, StatusTransferOut, 
												 CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
										  values(''' +@curCompanyCode+ ''',''' +@CentralBranch+ ''',''' 
												 +@VTINo+ ''',''' +convert(varchar,@SeqNo)+ ''','''
												 +@curSalesModelCode+ ''',''' +convert(varchar,@curSalesModelYear)+ ''','''
												 +@curChassisCode+ ''',''' +convert(varchar,@curChassisNo)+ ''','''
												 +@curEngineCode+ ''',''' +convert(varchar,@curEngineNo)+ ''','''
												 +@curColourCode+ ''',''' +@RemarkDtl+ ''',''1'',''' 
												 +@curCreatedBy+ ''',''' +convert(varchar,@curCreatedDate,121)+ ''','''
												 +@curLastUpdateBy+ ''',''' +convert(varchar,@curLastUpdateDate,121)+ ''')'
							execute sp_executesql @sqlString

					 -- SD: Insert / Update data to table omTrInventQtyVehicle - @WHFrom
						set @sqlString = 'merge into ' +@DBName+ '..omTrInventQtyVehicle as VEH using (values(''' 
													   +@curCompanyCode+ ''',''' +@curBranchCode+ ''',''' 
													   +convert(varchar,year(@curDocDate))+ ''',''' 
													   +convert(varchar,month(@curDocDate))+  ''',''' 
													   +@curSalesModelCode+ ''',''' +convert(varchar,@curSalesModelYear)+ ''',''' 
													   +@curColourCode+ ''',''' +@WHFrom+ ''',1,0,1,0,0,0,0,NULL,1,''' 
													   +@curCreatedBy+ ''',''' +convert(varchar,@curCreatedDate,121)+ ''',''' 
													   +@curLastUpdateBy+ ''',''' +convert(varchar,@curLastUpdateDate,121)+ 
													   ''',0,NULL,NULL))
											   as SRC (NewCompany, NewBranch, NewYear, NewMonth, NewSalesModelCode, 
													   NewSalesModelYear, NewColourCode, NewWarehouseCode, NewQtyIn, 
													   NewAlocation, NewQtyOut, NewBeginningOH, NewEndingOH, NewBeginningAV, 
													   NewEndingAV, NewRemark, NewStatus, NewCreatedBy, NewCreatedDate, 
													   NewLastUpdateBy, NewLastUpdateDate, NewIsLocked, NewLockedBy, NewLockedDate)
												   on  VEH.CompanyCode    = SRC.NewCompany
												  and  VEH.BranchCode	  = SRC.NewBranch
												  and  VEH.Year		      = SRC.NewYear
												  and  VEH.Month		  = SRC.NewMonth
												  and  VEH.SalesModelCode = SRC.NewSalesModelCode
												  and  VEH.SalesModelYear = SRC.NewSalesModelYear 
												  and  VEH.ColourCode	  = SRC.NewColourCode
												  and  VEH.WarehouseCode  = SRC.NewWarehouseCode
										 when matched
												 then update set QtyIn  = QtyIn  - SRC.NewQtyIn
															   , QtyOut = QtyOut - SRC.NewQtyOut
															   , LastUpdateBy    = ''' +@curLastUpdateBy+ '''
															   , LastUpdateDate  = ''' +convert(varchar,@curLastUpdateDate,121)+ '''
									 when not matched by target 
												 then insert (CompanyCode, BranchCode, Year, Month, SalesModelCode, SalesModelYear, 
															  ColourCode, WarehouseCode, QtyIn, Alocation, QtyOut, BeginningOH, 
															  EndingOH, BeginningAV, EndingAV, Remark, Status, CreatedBy, CreatedDate, 
															  LastUpdateBy, LastUpdateDate, IsLocked, LockedBy, LockedDate)
													  values (NewCompany, NewBranch, NewYear, NewMonth, NewSalesModelCode, 
															  NewSalesModelYear, NewColourCode, NewWarehouseCode, NewQtyIn, 
															  NewAlocation, NewQtyOut, NewBeginningOH, NewEndingOH, NewBeginningAV, 
															  NewEndingAV, NewRemark, NewStatus, NewCreatedBy, NewCreatedDate, 
															  NewLastUpdateBy, NewLastUpdateDate, NewIsLocked, NewLockedBy, 
															  NewLockedDate);'
							execute sp_executesql @sqlString
					end

			 -- SD: Insert / Update data to table omTrInventQtyVehicle - @WHTo
				set @sqlString = 'merge into ' +@DBName+ '..omTrInventQtyVehicle as VEH using (values(''' 
											   +@curCompanyCode+ ''',''' +@CentralBranch+ ''',''' 
											   +convert(varchar,year(@curDocDate))+ ''',''' 
											   +convert(varchar,month(@curDocDate))+  ''',''' 
											   +@curSalesModelCode+ ''',''' +convert(varchar,@curSalesModelYear)+ ''',''' 
											   +@curColourCode+ ''',''' +@WHTo+ ''',1,0,1,0,0,0,0,NULL,1,''' 
											   +@curCreatedBy+ ''',''' +convert(varchar,@curCreatedDate,121)+ ''',''' 
											   +@curLastUpdateBy+ ''',''' +convert(varchar,@curLastUpdateDate,121)+ 
											   ''',0,NULL,NULL))
									  as SRC  (NewCompany, NewBranch, NewYear, NewMonth, NewSalesModelCode, 
											   NewSalesModelYear, NewColourCode, NewWarehouseCode, NewQtyIn, 
											   NewAlocation, NewQtyOut, NewBeginningOH, NewEndingOH, NewBeginningAV, 
											   NewEndingAV, NewRemark, NewStatus, NewCreatedBy, NewCreatedDate, 
											   NewLastUpdateBy, NewLastUpdateDate, NewIsLocked, NewLockedBy, NewLockedDate)
										  on   VEH.CompanyCode    = SRC.NewCompany
										 and   VEH.BranchCode	  = SRC.NewBranch
										 and   VEH.Year			  = SRC.NewYear
										 and   VEH.Month		  = SRC.NewMonth
										 and   VEH.SalesModelCode = SRC.NewSalesModelCode
										 and   VEH.SalesModelYear = SRC.NewSalesModelYear 
										 and   VEH.ColourCode	  = SRC.NewColourCode
										 and   VEH.WarehouseCode  = SRC.NewWarehouseCode
								when matched
										then  update set QtyIn  = QtyIn  + SRC.NewQtyIn
													   , QtyOut = QtyOut + SRC.NewQtyOut
													   , LastUpdateBy    = ''' +@curLastUpdateBy+ '''
													   , LastUpdateDate  = ''' +convert(varchar,@curLastUpdateDate,121)+ '''
						    when not matched by target 
										then insert (CompanyCode, BranchCode, Year, Month, SalesModelCode, SalesModelYear, 
													 ColourCode, WarehouseCode, QtyIn, Alocation, QtyOut, BeginningOH, 
													 EndingOH, BeginningAV, EndingAV, Remark, Status, CreatedBy, 
													 CreatedDate, LastUpdateBy, LastUpdateDate, IsLocked, LockedBy, LockedDate)
											 values (NewCompany, NewBranch, NewYear, NewMonth, NewSalesModelCode, 
													 NewSalesModelYear, NewColourCode, NewWarehouseCode, NewQtyIn, NewAlocation, 
													 NewQtyOut, NewBeginningOH, NewEndingOH, NewBeginningAV, NewEndingAV, NewRemark, 
													 NewStatus, NewCreatedBy, NewCreatedDate, NewLastUpdateBy, NewLastUpdateDate, 
													 NewIsLocked, NewLockedBy, NewLockedDate);'
						execute sp_executesql @sqlString

			 -- SD: Insert data to table omTrPurchaseReturnDetail
				set @sqlString = 'merge into ' +@DBName+ '..omTrPurchaseReturnDetail as RTP using (values(''' 
											   +@curCompanyCode+ ''',''' +@CentralBranch+ ''',''' 
											   +@RTPNo+ ''',''' +@BPUNo+ ''',''' +@RemarkDtl+ ''','''
											   +@curCreatedBy+ ''',''' +convert(varchar,@curCreatedDate,121)+ ''',''' 
											   +@curLastUpdateBy+ ''',''' +convert(varchar,@curLastUpdateDate,121)+ '''))
									   as SRC (NewCompany, NewBranch, NewReturnNo, NewBPUNo,  NewRemark,
											   NewCreatedBy, NewCreatedDate, NewLastUpdateBy, NewLastUpdateDate)
										   on  RTP.CompanyCode = SRC.NewCompany
										  and  RTP.BranchCode  = SRC.NewBranch
										  and  RTP.ReturnNo    = SRC.NewReturnNo
										  and  RTP.BPUNo       = SRC.NewBPUNo
										 when  matched 
											   then update set Remark         = SRC.NewRemark
															 , LastUpdateBy   = ''' +@curLastUpdateBy+ '''
															 , LastUpdateDate = ''' +convert(varchar,@curLastUpdateDate,121)+ '''
										 when  not matched by target 
											   then insert (CompanyCode, BranchCode, ReturnNo, BPUNo, Remark, 
															CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
													values (NewCompany, NewBranch, NewReturnNo, NewBPUNo,  NewRemark,
															NewCreatedBy, NewCreatedDate, NewLastUpdateBy, NewLastUpdateDate);'
					execute sp_executesql @sqlString


			 -- SD: Collect price information from table omTrPurchaseHPP
				set @sqlString = N'select @BeforeDiscDPP=p.BeforeDiscDPP, @DiscExcludePPn=p.DiscExcludePPn, ' +
								  '@AfterDiscDPP=p.AfterDiscDPP, @AfterDiscPPn=p.AfterDiscPPn, ' +
								  '@AfterDiscPPnBM=p.AfterDiscPPnBM, @AfterDiscTotal=p.AfterDiscTotal, ' +
								  '@OthersDPP=p.OthersDPP, @OthersPPn=p.OthersPPn from ' +@DBName+ 
								  '..omTrPurchaseHPPSubDetail v, ' +@DBName+ '..omTrPurchaseHPPDetailModel p ' +
								  'where v.CompanyCode=p.CompanyCode and v.BranchCode=p.BranchCode ' +
									'and v.HPPNo=p.HPPNo and v.BPUNo=p.BPUNo and v.SalesModelCode=p.SalesModelCode ' +
									'and v.SalesModelYear=p.SalesModelYear and v.CompanyCode=''' +@curCompanyCode+ 
								  ''' and v.BranchCode=''' +@CentralBranch+ ''' and v.BPUNo=''' +@BPUNo+
								  ''' and ChassisCode=''' +@curChassisCode+ ''' and ChassisNo='
								  +convert(varchar,@curChassisNo)+ ''
						execute sp_executesql @sqlString, 
								N'@BeforeDiscDPP	numeric(18)	output,
								  @DiscExcludePPn	numeric(18)	output,
								  @AfterDiscDPP		numeric(18)	output,
								  @AfterDiscPPn		numeric(18) output,
								  @AfterDiscPPnBM	numeric(18) output,
								  @AfterDiscTotal	numeric(18) output,
								  @OthersDPP		numeric(18) output,
								  @OthersPPn		numeric(18) output',
								  @BeforeDiscDPP	output,
								  @DiscExcludePPn	output,
								  @AfterDiscDPP		output,
								  @AfterDiscPPn		output,
								  @AfterDiscPPnBM	output,
								  @AfterDiscTotal	output,
								  @OthersDPP		output,
								  @OthersPPn		output

			 -- SD: Insert data to table omTrPurchaseReturnDetailModel
				set @sqlString = 'merge into ' +@DBName+ '..omTrPurchaseReturnDetailModel as RTP using (values(''' 
											   +@curCompanyCode+ ''',''' +@CentralBranch+ ''',''' +@RTPNo+ ''',''' +@BPUNo+ 
											   ''',''' +@curSalesModelCode+ ''',''' +convert(varchar,@curSalesModelYear)+ ''',1,''' 
											   +convert(varchar,@BeforeDiscDPP)+ ''',''' +convert(varchar,@DiscExcludePPn)+ ''','''
											   +convert(varchar,@AfterDiscDPP)+ ''',''' +convert(varchar,@AfterDiscPPn)+ ''','''
											   +convert(varchar,@AfterDiscPPnBM)+ ''',''' +convert(varchar,@AfterDiscTotal)+ ''','''
											   +convert(varchar,@OthersDPP)+ ''',''' +convert(varchar,@OthersPPn)+ ''','''
											   +@curCreatedBy+ ''',''' +convert(varchar,@curCreatedDate,121)+ ''',''' 
											   +@curLastUpdateBy+ ''',''' +convert(varchar,@curLastUpdateDate,121)+ '''))
									   as SRC (NewCompany, NewBranch, NewReturnNo, NewBPUNo, 
											   NewSalesModelCode, NewSalesModelYear, NewQuantity, 
											   NewBeforeDiscDPP, NewDiscExcludePPn, NewAfterDiscDPP, 
											   NewAfterDiscPPn, NewAfterDiscPPnBM, NewAfterDiscTotal, 
											   NewOthersDPP, NewOthersPPn, NewCreatedBy, NewCreatedDate, 
											   NewLastUpdateBy, NewLastUpdateDate)
										   on  RTP.CompanyCode    = SRC.NewCompany
										  and  RTP.BranchCode     = SRC.NewBranch
										  and  RTP.ReturnNo       = SRC.NewReturnNo
										  and  RTP.BPUNo          = SRC.NewBPUNo
										  and  RTP.SalesModelCode = SRC.NewSalesModelCode
										  and  RTP.SalesModelYear = SRC.NewSalesModelYear
										 when  matched 
											   then update set Quantity       = Quantity + SRC.NewQuantity
															 , BeforeDiscDPP  = BeforeDiscDPP  + SRC.NewBeforeDiscDPP
															 , DiscExcludePPn = DiscExcludePPn + SRC.NewDiscExcludePPn
															 , AfterDiscDPP   = AfterDiscDPP   + SRC.NewAfterDiscDPP
															 , AfterDiscPPn   = AfterDiscPPn   + SRC.NewAfterDiscPPn
															 , AfterDiscPPnBM = AfterDiscPPnBM + SRC.NewAfterDiscPPnBM
															 , AfterDiscTotal = AfterDiscTotal + SRC.NewAfterDiscTotal
															 , OthersDPP      = OthersDPP      + SRC.NewOthersDPP
															 , OthersPPn      = OthersPPn      + SRC.NewOthersPPn
															 , LastUpdateBy   = ''' +@curLastUpdateBy+ '''
															 , LastUpdateDate = ''' +convert(varchar,@curLastUpdateDate,121)+ '''
										when  not matched by target 
											  then insert (CompanyCode, BranchCode, ReturnNo, BPUNo, SalesModelCode,
														   SalesModelYear, Quantity, BeforeDiscDPP, DiscExcludePPn,
														   AfterDiscDPP, AfterDiscPPn, AfterDiscPPnBM, AfterDiscTotal, 
														   OthersDPP, OthersPPn, CreatedBy, CreatedDate, LastUpdateBy, 
														   LastUpdateDate)
												   values (NewCompany, NewBranch, NewReturnNo, NewBPUNo, 
														   NewSalesModelCode, NewSalesModelYear, NewQuantity, 
														   NewBeforeDiscDPP, NewDiscExcludePPn, NewAfterDiscDPP, 
														   NewAfterDiscPPn, NewAfterDiscPPnBM, NewAfterDiscTotal, 
														   NewOthersDPP, NewOthersPPn, NewCreatedBy, NewCreatedDate, 
														   NewLastUpdateBy, NewLastUpdateDate);'
					execute sp_executesql @sqlString

			 -- SD: Insert data to table omTrPurchaseReturnSubDetail
				set @sqlString = 'insert into ' +@DBName+ '..omTrPurchaseReturnSubDetail
										(CompanyCode, BranchCode, ReturnNo, BPUNo, ReturnSeq, 
										 SalesModelCode, SalesModelYear, ColourCode, ChassisCode, 
										 ChassisNo, EngineCode, EngineNo, Remark, CreatedBy, 
										 CreatedDate, LastUpdateBy, LastUpdateDate)
								  values(''' +@curCompanyCode+ ''',''' +@CentralBranch+ ''',''' +@RTPNo+ ''',''' 
										 +@BPUNo+ ''',''' +Convert(varchar,@SeqNo)+ ''',''' +@curSalesModelCode+ ''','''
								         +convert(varchar,@curSalesModelYear)+ ''',''' +@curColourCode+ ''',''' 
										 +@curChassisCode+ ''',''' +convert(varchar,@curChassisNo)+ ''',''' 
										 +@curEngineCode+ ''',''' +convert(varchar,@curEngineNo)+ ''',''' 
										 +@RemarkDtl+ ''',''' +@curCreatedBy+ ''',''' +convert(varchar,@curCreatedDate,121)+ ''','''
										 +@curLastUpdateBy+ ''',''' +convert(varchar,@curLastUpdateDate,121)+ ''')'
					execute sp_executesql @sqlString

			 -- SD: Update data to table omTrPurchaseHPPSubDetail
				set @sqlString = 'update ' +@DBName+ '..omTrPurchaseHPPSubDetail ' +
										'set isReturn = 1 where CompanyCode=''' +@curCompanyCode+ 
										''' and BranchCode=''' +@CentralBranch+ ''' and HPPNo=''' +@HPPNo+ 
										''' and BPUNo=''' +@BPUNo+ ''' and ChassisCode=''' +@curChassisCode+ 
										''' and ChassisNo=''' +convert(varchar,@curChassisNo)+''''
					execute sp_executesql @sqlString

			 -- MD: Insert data to table omTrSalesReturnBPK
				set @sqlString = 'merge into ' +@DBNameMD+ '..omTrSalesReturnBPK as RTS using (values(''' 
											   +@curCompanyMD+ ''',''' +@curUnitBranchMD+ ''',''' 
											   +@RTSNo+ ''',''' +@BPKNo+ ''','''
											   +@curCreatedBy+ ''',''' +convert(varchar,@curCreatedDate,121)+ ''',''' 
											   +@curLastUpdateBy+ ''',''' +convert(varchar,@curLastUpdateDate,121)+ '''))
									   as SRC (NewCompany, NewBranch, NewReturnNo, NewBPKNo, NewCreatedBy, 
											   NewCreatedDate, NewLastUpdateBy, NewLastUpdateDate)
										   on RTS.CompanyCode = SRC.NewCompany
										  and RTS.BranchCode  = SRC.NewBranch
										  and RTS.ReturnNo    = SRC.NewReturnNo
										  and RTS.BPKNo       = SRC.NewBPKNo
										 when matched 
											  then update set LastUpdateBy   = ''' +@curLastUpdateBy+ '''
															, LastUpdateDate = ''' +convert(varchar,@curLastUpdateDate,121)+ '''
										 when not matched by target 
											  then insert (CompanyCode, BranchCode, ReturnNo, BPKNo, CreatedBy, 
														   CreatedDate, LastUpdateBy, LastUpdateDate)
												   values (NewCompany, NewBranch, NewReturnNo, NewBPKNo, NewCreatedBy, 
														   NewCreatedDate, NewLastUpdateBy, NewLastUpdateDate);'
						execute sp_executesql @sqlString

			 -- MD: Insert data to table omTrSalesReturnDetailModel
				set @sqlString = 'merge into ' +@DBNameMD+ '..omTrSalesReturnDetailModel as RTS using (values(''' 
											   +@curCompanyMD+ ''',''' +@curUnitBranchMD+ ''',''' +@RTSNo+ ''',''' +@BPKNo+ ''','''
											   +@curSalesModelCode+ ''',''' +convert(varchar,@curSalesModelYear)+ ''',''' 
											   +convert(varchar,@BeforeDiscDPP)+ ''',''' +convert(varchar,@DiscExcludePPn)+ ''','''
											   +convert(varchar,@AfterDiscDPP)+ ''',''' +convert(varchar,@AfterDiscPPn)+ ''','''
											   +convert(varchar,@AfterDiscPPnBM)+ ''',''' +convert(varchar,@AfterDiscTotal)+ ''','''
											   +convert(varchar,@OthersDPP)+ ''',''' +convert(varchar,@OthersPPn)+ ''',1,'''
											   +@curCreatedBy+ ''',''' +convert(varchar,@curCreatedDate,121)+ ''',''' 
											   +@curLastUpdateBy+ ''',''' +convert(varchar,@curLastUpdateDate,121)+ '''))
									   as SRC (NewCompany, NewBranch, NewReturnNo, NewBPKNo,  
											   NewSalesModelCode, NewSalesModelYear, 
											   NewBeforeDiscDPP, NewDiscExcludePPn, NewAfterDiscDPP, 
											   NewAfterDiscPPn, NewAfterDiscPPnBM, NewAfterDiscTotal, 
											   NewOthersDPP, NewOthersPPn, NewQuantity, 
											   NewCreatedBy, NewCreatedDate, 
											   NewLastUpdateBy, NewLastUpdateDate)
										   on RTS.CompanyCode    = SRC.NewCompany
										  and RTS.BranchCode     = SRC.NewBranch
										  and RTS.ReturnNo       = SRC.NewReturnNo
									      and RTS.BPKNo          = SRC.NewBPKNo
										  and RTS.SalesModelCode = SRC.NewSalesModelCode
										  and RTS.SalesModelYear = SRC.NewSalesModelYear
										 when matched 
											  then update set Quantity       = Quantity       + SRC.NewQuantity
															, BeforeDiscDPP  = BeforeDiscDPP  + SRC.NewBeforeDiscDPP
															, DiscExcludePPn = DiscExcludePPn + SRC.NewDiscExcludePPn
															, AfterDiscDPP   = AfterDiscDPP   + SRC.NewAfterDiscDPP
															, AfterDiscPPn   = AfterDiscPPn   + SRC.NewAfterDiscPPn
															, AfterDiscPPnBM = AfterDiscPPnBM + SRC.NewAfterDiscPPnBM
															, AfterDiscTotal = AfterDiscTotal + SRC.NewAfterDiscTotal
															, OthersDPP      = OthersDPP      + SRC.NewOthersDPP
															, OthersPPn      = OthersPPn      + SRC.NewOthersPPn
															, LastUpdateBy   = ''' +@curLastUpdateBy+ '''
															, LastUpdateDate = ''' +convert(varchar,@curLastUpdateDate,121)+ '''
										when not matched by target 
											 then insert (CompanyCode, BranchCode, ReturnNo, BPKNo, 
														  SalesModelCode, SalesModelYear, 
														  BeforeDiscDPP, DiscExcludePPn, AfterDiscDPP,
														  AfterDiscPPn, AfterDiscPPnBM, AfterDiscTotal, 
														  OthersDPP, OthersPPn, Quantity, 
														  CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
												  values (NewCompany, NewBranch, NewReturnNo, NewBPKNo,  
														  NewSalesModelCode, NewSalesModelYear, 
														  NewBeforeDiscDPP, NewDiscExcludePPn, NewAfterDiscDPP, 
														  NewAfterDiscPPn, NewAfterDiscPPnBM, NewAfterDiscTotal, 
														  NewOthersDPP, NewOthersPPn, NewQuantity, NewCreatedBy, 
														  NewCreatedDate, NewLastUpdateBy, NewLastUpdateDate);'
						execute sp_executesql @sqlString

			 -- MD: Collect BPK information from table omTrSalesInvoiceVIN
				set @sqlString = N'select @BPKNo=BPKNo from ' +@DBNameMD+ '..omTrSalesInvoiceVin where CompanyCode='''
										  +@curCompanyMD+ ''' and BranchCode=''' +@curUnitBranchMD+ ''' and InvoiceNo=''' 
										  +@INVNo+ ''' and ChassisCode=''' +@curChassisCode+ ''' and ChassisNo='''
										  +convert(varchar,@curChassisNo) + ''''
						execute sp_executesql @sqlString, 
								N'@BPKNo	varchar(15) output', @BPKNo	output

			 -- MD: Insert data to table omTrSalesReturnVIN
				set @sqlString = 'insert into ' +@DBNameMD+ '..omTrSalesReturnVIN
										(CompanyCode, BranchCode, ReturnNo, BPKNo, SalesModelCode, SalesModelYear, 
										 ReturnSeq, ColourCode, ChassisCode, ChassisNo, EngineCode, EngineNo, 
										 Remark, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
								  values(''' +@curCompanyMD+ ''',''' +@curUnitBranchMD+ ''',''' 
										 +@RTSNo+ ''',''' +@BPKNo+ ''',''' 
										 +@curSalesModelCode+ ''',''' +convert(varchar,@curSalesModelYear)+ ''','''
										 +convert(varchar,@SeqNo)+ ''',''' +@curColourCode+ ''',''' 
										 +@curChassisCode+ ''',''' +convert(varchar,@curChassisNo)+ ''',''' 
										 +@curEngineCode+ ''',''' +convert(varchar,@curEngineNo)+ ''',''' 
										 +@RemarkDtl+ ''','''  
										 +@curCreatedBy+ ''',''' +convert(varchar,@curCreatedDate,121)+ ''','''
										 +@curLastUpdateBy+ ''',''' +convert(varchar,@curLastUpdateDate,121)+ ''')'
					execute sp_executesql @sqlString

/*				
			USE POSTING JOURNAL IN UTILITY SALES MODULE 
			FOR UPDATE DATA TO TABLE ARINTERFACE & GLINTERFACE
			 -- MD: Update data to table arInterface
			 -- MD: Update data to table glInterface
			 -- SD: Update data to table apInterface
			 -- SD: Update data to table glInterface
*/

   			 -- Update Daily Posting Process Status
				update omSDMovement
				   set ProcessStatus=1
				     , ProcessDate  =@CurrentDate
					where CompanyCode=@curCompanyCode
					  and BranchCode =@curBranchCode
					  and DocNo      =@curDocNo
					  and DocDate    =@curDocDate
					  and Seq        =@curSeq

			 -- Read next record
				fetch next from curomSDMovement
					  into  @curCompanyCode, @curBranchCode, @curDocNo, @curDocDate, @curSeq, @curSalesModelCode,
							@curSalesModelYear, @curChassisCode, @curChassisNo, @curEngineCode, @curEngineNo, 
							@curColourCode, @curWarehouseCode, @curCustomerCode, @curQtyFlag, @curCompanyMD, 
							@curBranchMD, @curWarehouseMD, @curStatus, @curProcessStatus, @curProcessDate, 
							@curCreatedBy, @curCreatedDate, @curLastUpdateBy, @curLastUpdateDate
			end 

	 -- Move data which already processed from table omSDMovement to table omHstSDMovement
	    if not exists (select * from sys.objects where object_id = object_id(N'[dbo].[omHstSDMovement]') and type in (N'U'))
			CREATE TABLE [dbo].[omHstSDMovement](
				[CompanyCode] [varchar](15) NOT NULL,
				[BranchCode] [varchar](15) NOT NULL,
				[DocNo] [varchar](15) NOT NULL,
				[DocDate] [datetime] NOT NULL,
				[Seq] [int] NOT NULL,
				[SalesModelCode] [varchar](20) NOT NULL,
				[SalesModelYear] [numeric](4, 0) NOT NULL,
				[ChassisCode] [varchar](15) NOT NULL,
				[ChassisNo] [numeric](10, 0) NOT NULL,
				[EngineCode] [varchar](15) NOT NULL,
				[EngineNo] [numeric](10, 0) NOT NULL,
				[ColourCode] [varchar](15) NOT NULL,
				[WarehouseCode] [varchar](15) NOT NULL,
				[CustomerCode] [varchar](15) NOT NULL,
				[QtyFlag] [char](1) NOT NULL,
				[CompanyMD] [varchar](15) NOT NULL,
				[BranchMD] [varchar](15) NOT NULL,
				[WarehouseMD] [varchar](15) NOT NULL,
				[Status] [char](1) NOT NULL,
				[ProcessStatus] [char](1) NOT NULL,
				[ProcessDate] [datetime] NOT NULL,
				[CreatedBy] [varchar](15) NOT NULL,
				[CreatedDate] [datetime] NOT NULL,
				[LastUpdateBy] [varchar](15) NOT NULL,
				[LastUpdateDate] [datetime] NOT NULL)
			
		insert into omHstSDMovement (CompanyCode, BranchCode, DocNo, DocDate, Seq, SalesModelCode,
								     SalesModelYear, ChassisCode, ChassisNo, EngineCode, EngineNo,
									 ColourCode, WarehouseCode, CustomerCode, QtyFlag, CompanyMD, 
									 BranchMD, WarehouseMD, Status, ProcessStatus, ProcessDate,
									 CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
							 select  CompanyCode, BranchCode, DocNo, DocDate, Seq, SalesModelCode,
								     SalesModelYear, ChassisCode, ChassisNo, EngineCode, EngineNo,
									 ColourCode, WarehouseCode, CustomerCode, QtyFlag, CompanyMD, 
									 BranchMD, WarehouseMD, Status, ProcessStatus, ProcessDate,
									 CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate
							   from  omSDMovement
							  where	 ProcessStatus = 1
                                 or  convert(varchar,DocDate,111)<=convert(varchar,@PostingDate,111)

		delete omSDMovement   where	 ProcessStatus = 1
                                 or  convert(varchar,DocDate,111)<=convert(varchar,@PostingDate,111)

		close curomSDMovement
		deallocate curomSDMovement

END
GO

if object_id('usprpt_PostingMultiCompanyMainProcess') is not null
	drop procedure usprpt_PostingMultiCompanyMainProcess
GO
-- POSTING TRANSACTION MULTI COMPANY - MAIN PROCESS
-- ---------------------------------------------------------------
-- Created  by : HTO, 2014
-- Revision by : HTO, January 2015
-- ---------------------------------------------------------------
-- This procedure used for 
--		4-Wheeler : BIT-SBT, SBSM-SSBT, BAT-SAT, SIT-SST , UIB-SIT
--		2-Wheeler : IJMG,SVMG,ISG,ISMG-SMG
-- -------------------------------------------------------------------------------
-- execute [usprpt_PostingMultiCompanyMainProcess] '6006400001','2014/11/14','HTO'
-- update sysParaMeter set ParamValue='2014/11/01' where ParamId='POSTING_STATUS'
-- -------------------------------------------------------------------------------

CREATE procedure [dbo].[usprpt_PostingMultiCompanyMainProcess]
	@CompanyCode	varchar(15),
	@PostingDate	datetime,
	@UserId			varchar(20)
AS	

BEGIN TRANSACTION
BEGIN TRY

BEGIN
 -- Check Posting Multi Company Date in table sysParameter
	declare @PostDate	varchar(10)
	declare @PostStatus	integer
	set @PostDate   = (select ParamValue from sysParaMeter where ParamId='POSTING_STATUS')
	set @PostStatus = (case when @PostDate is null                             then 0
	                        when @PostDate < convert(varchar,@PostingDate,111) then 1
					        else                                                    2
					   end)
    if (select 1 from sysParaMeter where ParamId='POSTING_STATUS')=1 and @PostStatus=0
	    set @PostStatus=1

	if @PostStatus = 0
		insert sysParaMeter values('POSTING_STATUS',convert(varchar,@PostingDate,111),'Posting Multi Company')
	else
		if @PostStatus = 1
			update sysParaMeter set ParamValue=convert(varchar,@PostingDate,111) where ParamId='POSTING_STATUS'
		else
			begin
				select '0' [STATUS], 'Daily Posting tertanggal ' + convert(varchar,@PostingDate,106) + ' sudah pernah dilakukan sebelumnya....' [INFO]
				return
			end

	declare @Status	varchar(1)

	execute [usprpt_PostingMultiCompanySales] @CompanyCode, @PostingDate, @Status OUTPUT
	if @Status = '1'
		begin
			select '0' [STATUS], 'Daily Posting Sales fail...' [INFO]
			return
		end

	execute [usprpt_PostingMultiCompanySalesReturn] @CompanyCode, @PostingDate, @Status OUTPUT
	if @Status = '1'
		begin
			select '0' [STATUS], 'Daily Posting Sales Return fail...' [INFO]
			return
		end

	execute [usprpt_PostingMultiCompanySparepartService] @CompanyCode, @PostingDate, @Status OUTPUT
	if @Status = '1'
		begin
			select '0' [STATUS], 'Daily Posting SparePart & Service fail...' [INFO]
			return
		end

	execute [usprpt_PostingMultiCompanySparePartReturn] @CompanyCode, @PostingDate, @Status OUTPUT
	if @Status = '1'
		begin
			select '0' [STATUS], 'Daily Posting SparePart & Service Return fail...' [INFO]
			return
		end
END		

END TRY

BEGIN CATCH
    --select ERROR_NUMBER()    AS ErrorNumber,    ERROR_SEVERITY() AS ErrorSeverity, ERROR_STATE()   AS ErrorState,
		  -- ERROR_PROCEDURE() AS ErrorProcedure, ERROR_LINE()     AS ErrorLine    , ERROR_MESSAGE() AS ErrorMessage
	if @@TRANCOUNT > 0
		begin
			select '0' [STATUS], 'Posting gagal !!!   ' + ERROR_MESSAGE() [INFO]
			rollback transaction
			return
		end
END CATCH

IF @@TRANCOUNT > 0
	begin
		select '1' [STATUS], 'Posting berhasil !!!' [INFO]
		--rollback transaction
		commit transaction
	end

GO

if object_id('uspfn_gnCheckPostingStatus') is not null
	drop procedure uspfn_gnCheckPostingStatus
GO

CREATE procedure [dbo].[uspfn_gnCheckPostingStatus]
AS
BEGIN
	declare @checklasttrans datetime, @retValue int, @tax int,
			@procStatus int,@retValue2 int, @prmValue varchar(20),
			@sql nvarchar(2000)	

	select @prmValue=ParamValue from sysParameter where paramid='SPSRV'
	
	declare @dbmd varchar(20)
	select top 1 @dbmd=dbmd from gnMstCompanyMapping

	set @tax = 1

	IF @prmValue='ON'
	BEGIN

		set @sql = 'SELECT top 1 @checklasttrans=[DocDate], @procStatus = ProcessStatus 
		FROM ' + @dbmd + '..[svSDMovement] with(nolock,nowait) where left(DocNo,2) in (''IN'',''RT'') order by docdate desc'

		exec sp_Executesql @sql, N'@checklasttrans datetime output, @procStatus int output', @checklasttrans output, @procStatus output
	
		IF (@checklasttrans IS NULL)
			set @retValue=1
		ELSE
		BEGIN
			IF  (convert(varchar(10),@checklasttrans,120) <  convert(varchar(10),getdate(),120)) AND @procStatus=0
				select @retValue=0, @tax = 0
			ELSE
				set @retValue=1
		END
	END
	ELSE
		set @retValue=2

	SELECT @checklasttrans = NULL, @procStatus = 0

	select @prmValue=ParamValue from sysParameter where paramid='SLS'
	IF @prmValue='ON'
	BEGIN

		set @sql = 'SELECT top 1 @checklasttrans=[DocDate], @procStatus = ProcessStatus 
		FROM  ' + @dbmd + '..[omSDMovement] with(nolock,nowait) where left(DocNo,2) in(''IV'',''IU'',''RT'',''RS'') order by docdate desc'

		exec sp_Executesql @sql, N'@checklasttrans datetime output, @procStatus int output', @checklasttrans output, @procStatus output

		IF (@checklasttrans IS NULL)
			set @retValue2=1
		ELSE
		BEGIN
			IF  (convert(varchar(10),@checklasttrans,120) <  convert(varchar(10),getdate(),120)) AND @procStatus=0
				BEGIN
					select @retValue2=0, @tax = 0	
					IF @retValue <> 2
						set @retValue=0
				END
			ELSE
				BEGIN
					set @retValue2=1
				END
		END
		
		IF @retValue2=1 and @retValue=0 
			set @retValue2=0
		
	END
	ELSE
		set @retValue2=1
		
	IF @retValue = 2
		set @retValue=1
		
	SELECT @retValue [SPSRV], @retValue2 [SALES], 
	(select top 1 replace(ParamDescription, char(13),'</BR>') from sysParameter where ParamId='POSTING_STATUS') INFO, 
	(select top 1 case when ISNULL(ParamValue,'OFF')='OFF' then 1 else @tax end from sysParameter where paramid='PAJAK') TAX

END
GO

IF NOT EXISTS(select * from sysParameter where paramid='POSTING_STATUS')
BEGIN
	insert into sysParameter
	values ('POSTING_STATUS', '','')
END

update sysparameter
set ParamDescription='SETIAP HARI WAJIB MELAKUKAN DAILY POSTING . . .
DAILY POSTING DILAKUKAN SETELAH BERAKHIRNYA SELURUH TRANSAKSI HARIAN (SALES, SPAREPART & SERVICE).

TERIMA KASIH'
where ParamId='POSTING_STATUS'
GO

IF NOT EXISTS(select * from sysParameter where paramid='SPSRV')
BEGIN
	insert into sysParameter
	values ('SPSRV','OFF','SPAREPART & SERVICE DAILY POSTING PROTECTION')
END

IF NOT EXISTS(select * from sysParameter where paramid='PAJAK')
BEGIN
	insert into sysParameter
	values ('PAJAK','OFF','TAX MODULE')
END

IF NOT EXISTS(select * from sysParameter where paramid='SLS')
BEGIN
	insert into sysParameter
	values ('SLS','OFF','SALES DAILY POSTING PROTECTION')
END
GO

