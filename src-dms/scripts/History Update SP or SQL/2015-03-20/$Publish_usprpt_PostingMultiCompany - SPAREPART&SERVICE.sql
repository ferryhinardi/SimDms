

if object_id('usprpt_PostingMultiCompanySparePartService') is not null
	drop procedure usprpt_PostingMultiCompanySparePartService
GO
-- POSTING TRANSACTION MULTI COMPANY - SPARE PART & SERVICE
-- --------------------------------------------------------------
-- Created  by : HTO, 2014
-- Revision by : HTO, January 2015
-- --------------------------------------------------------------
-- This procedure used for 
--		4-Wheeler : BIT-SBT, SBSM-SSBT, BAT-SAT, SIT-SST, UIB-SIT
--		2-Wheeler : IJMG,SVMG,ISG,ISMG-SMG
-- ----------------------------------------------------------------------------------
-- execute [usprpt_PostingMultiCompanySparePartService] '6006400001','2014/11/08','0'
-- ----------------------------------------------------------------------------------
CREATE procedure [dbo].[usprpt_PostingMultiCompanySparePartService]
	@CompanyCode	varchar(15),
	@PostingDate	datetime,
	@Status			varchar(1)	output
AS	

BEGIN 
  -- Check Tax/Seri Pajak online
		declare @TaxCompany					varchar(15)
		declare @TaxBranch					varchar(15)
		declare @TaxDB						varchar(50)
		declare @TaxTransCode				varchar(3)
		declare @swTaxBranch				varchar(15) = ''
		declare @swTaxDoc					varchar(15) = ''
		declare @TaxSeq						bigint
		declare @TaxSeqNo					int
		declare @SeriPajakNo				varchar(50) = ''

		set @TaxCompany=isnull((select ParaValue from gnMstLookupDtl where CodeID='TXOL' and LookupValue='COMPANYCODE'),'')
		set @TaxBranch =isnull((select ParaValue from gnMstLookupDtl where CodeID='TXOL' and LookupValue='BRANCHCODE'),'')
		set @TaxDB     =isnull((select ParaValue from gnMstLookupDtl where CodeID='TXOL' and LookupValue='FROM_DB'),'')
		--select @TaxCompany, @TaxBranch, @TaxDB
		if @TaxCompany='' or @TaxBranch='' or @TaxDB=''
			return

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
		declare @sqlString					nvarchar(max)
		declare @sqlString					nvarchar(max)
		declare @sqlString					nvarchar(max)
		declare @swCompanyCode				varchar(15) = ''
		declare @swBranchCode				varchar(15) = ''
		declare @swDocNo 					varchar(15) = ''
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
			select * from svSDMovement
             where left(DocNo,3) in ('LMP','FPJ') 
			   and convert(varchar,DocDate,111)<=convert(varchar,@PostingDate,111)
			   and ProcessStatus=0
             order by CompanyCode, BranchCode, DocNo, TypeOfGoods, PartNo, PartSeq
		open cursvSDMovement

		fetch next from cursvSDMovement
			  into @curCompanyCode, @curBranchCode, @curDocNo, @curDocDate, @curPartNo, @curPartSeq, @curWarehouseCode, @curQtyOrder, 
			       @curQty, @curDiscPct, @curCostPrice, @curRetailPrice, @curTypeOfGoods, @curCompanyMD, @curBranchMD, @curWarehouseMD, 
				   @curRetailPriceInclTaxMD, @curRetailPriceMD, @curCostPriceMD, @curQtyFlag, @curProductType, @curProfitCenterCode, 
				   @curStatus, @curProcessStatus, @curProcessDate, @curCreatedBy, @curCreatedDate, @curLastUpdateBy, @curLastUpdateDate

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

			 -- MD: Top Code, Payment Code, Sales Code
				set @sqlString = N'select @TopCodeMD=TopCode, @PaymentCodeMD=PaymentCode, @SalesCodeMD from ' 
										  +@DBNameMD+ '..gnMstCustomerProfitCenter where CompanyCode=''' 
										  +@curCompanyMD+ ''' and BranchCode=''' 
										  +@curBranchMD+ ''' and CustomerCode=''' 
										  +@curBranchCode+ ''' and ProfitCenterCode=''300'''
					execute sp_executesql @sqlString, 
										N'@TopCodeMD 		varchar(15) output,
										  @PaymentCodeMD 	varchar(15) output,
										  @SalesCodeMD 		varchar(15) output', 
										  @TopCodeMD 		output,
										  @PaymentCodeMD 	output,
										  @SalesCodeMD 		output

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
					execute sp_executesql @sqlString
										N'@AccNoDisc1MD 	varchar(50) output,
										  @AccNoCogsMD 		varchar(50) output,
										  @AccNoInventoryMD varchar(50) output', 
										  @AccNoDisc1MD 	output,
										  @AccNoCogsMD 		output,
										  @AccNoInventoryMD output
										  
				set @SalesAmt    = @curQty*@curRetailPriceInclTaxMD 
				set @DiscAmt     = round((@SalesAmt*@curDiscPct / 100),0)
				set @NetSales    = @SalesAmt-@DiscAmt
				set @PPNAmt      = round((@NetSales*0.1),0)
				set @TotSalesAmt = @NetSales+@PPNAmt
				set @CostAmt     = @curQty*@curCostPriceMD 
				set @DueDateMD   = dateadd(day,isnull(@TopDaysMD,0),@curDocDate)

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
									  and p.SupplierCode    =''' +@curCompanyMD+ ''''
					execute sp_executesql @sqlString, 
										N'@AccNoAP 			varchar(50) output,
										  @AccNoTaxIn 		varchar(50) output', 
										  @AccNoAP 			output,
										  @AccNoTaxIn 		output

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

				set @RetailPriceNet    = round(@curRetailPriceMD * (100.00-@curDiscPct) / 100.00,0)
				set @TotPurchaseNetAmt = @CurQty * @RetailPriceNet
				set @TotTaxAmt         = round(@TotPurchaseNetAmt * 0.1,0)
			    set @TotPurchaseAmt    = @TotPurchaseNetAmt + @TotTaxAmt

				if @swCompanyCode <> @curCompanyCode or
				   @swBranchCode  <> @curBranchCode  or
				   @swDocNo		  <> @curDocNo
					begin
						set @swCompanyCode = @curCompanyCode
						set @swBranchCode  = @curBranchCode
						set @swDocNo	   = @curDocNo
						set @SeqNo		   = 0

					 -- Discount Flag
						set @sqlString = N'select top 1 @DiscFlag=1 from ' 
												  +@DBName+ '..svSDMovement where CompanyCode=''' 
												  +@curCompanyCode+ ''' and BranchCode=''' 
												  +@curBranchCode+ ''' and DocNo=''' 
												  +@curDocNo+ ''''
							execute sp_executesql @sqlString, 
												N'@DiscFlag 	integer output', 
												  @DiscFlag  	output

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
						execute [usprpt_PostingMultiCompany4DocNo] @curCompanyCode, @curBranchCode, @DBName, 'SOC', @SONo output
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
						execute [usprpt_PostingMultiCompany4DocNo] @curCompanyCode, @curBranchCode, @DBName, 'PLS', @PLNo output
						set @sqlString = 'insert into ' +@DBNameMD+ '..spTrnSPickingHdr 
										(CompanyCode, BranchCode, PickingSlipNo, PickingSlipDate, CustomerCode, CustomerCodeBill, 
										 CustomerCodeShip, PickedBy, isBORelease, isSubstitution, isIncludePPN, TransType, SalesType, 
										 TotSalesQty, TotSalesAmt, TotDiscAmt, TotDppAmt, TotPPNAmt, TotFinalSalesAmt, Remark, Status, 
										 PrintSeq, TypeOfGoods, CreatedBy, CreatedDate, LastupdateBy, LastUpdateDate, isLocked, LockingBy, LockingDate)
										values(''' +@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' +@PLNo+ 
										       ''',''' +convert(varchar,@curDocDate,121)+ ''',''' +@curBranchCode+
											   ''',''' +@curBranchCode+ ''',''' +@curBranchCode+ ''',''POSTING'',0,0,1,''00'',''0'',0,0,0,0,0,0,NULL,2,1,'''
											           +@TypeOfGoodsMD+ ''',''POSTING'',''' +convert(varchar,@PostingDate,111)+ ''',''POSTING'',''' 
												   +@currentDate+ ''',0,NULL,NULL)'
							execute sp_executesql @sqlString

					 -- MD: Insert Data to table spTrnSInvoiceHdr 
						execute [usprpt_PostingMultiCompany4DocNo] @curCompanyCode, @curBranchCode, @DBName, 'INV', @INVNo output
						execute [usprpt_PostingMultiCompany4DocNo] @curCompanyCode, @curBranchCode, @DBName, 'FPJ', @FPJNo output
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

						if @curBranchCode<>@swTaxBranch or left(@curDocNo,3)<>left(@swTaxDoc,3)
							begin
								set @swTaxBranch = @curBranchCode
								set @swTaxDoc	 = @curDocNo

								set @sqlString = N'select top 1 @TaxSeq=FPJSeqNo, @TaxSeqNo=SeqNo from ' 
														  +@TaxDb+ '..gnMstFPJSeqNo where CompanyCode=''' 
														  +@TaxCompany+ ''' and BranchCode=''' 
														  +@TaxBranch+ ''' and Year=''' 
														  +convert(varchar,year(@PostingDate))+ ''' and convert(varchar,EffectiveDate,111)<=''' 
														  +convert(varchar,@PostingDate,111)+ ''' order by CompanyCode, BranchCode, Year, SeqNo'
									execute sp_executesql @sqlString, 
														N'@TaxSeq 		bigint  	output
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

								set @SeriPajakNo = @TaxTransCode + '0.' +isnull(replicate('0',11-len(convert(varchar,@TaxSeq))),'') + 
													+left(convert(varchar,@TaxSeq),len(convert(varchar,@TaxSeq))-8) + '-' +
													+right(convert(varchar,year(@PostingDate)),2)+ '.' +right(convert(varchar,@TaxSeq),8)
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
												  '@SKPNoMD=SKPNo, @SKPDateMD=SKPDate, @NPWPDateMD=NPWPDate  from ' 
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
												   +@curCompanyMD+ ''',''' +@FPJNo+ ''','''
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

				set @SeqNo = @SeqNo + 1
			 -- SD: Insert data to table spTrnPPOSDtl 
				set @sqlString = 'insert into ' +@DBName+ '..spTrnPPOSDtl
										(CompanyCode, BranchCode, POSNo, PartNo, SeqNo, OrderQty, SuggorQty, PurchasePrice, DiscPct,
										 PurchasePriceNett, CostPrice, TotalAmount, ABCCLass, MovingCode, ProductType, PartCategory,
										 CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate, Note)
								   values(''' +@curCompanyCode+ ''',''' +@curBranchCode+ ''',''' +@POSNo+ ''',''' +@curPartNo+ 
										  ''',' +convert(varchar,@SeqNo)+ ',' +convert(varchar,@curQty)+ 
										  ',' +convert(varchar,@curQtyOrder)+ ',' +convert(varchar,@curRetailPrice)+ 
										  ',' +convert(varchar,@curDiscPct)+ ',' +convert(varchar,@RetailPriceNet)+ 
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
											  +convert(varchar,@curQty)+ ',' +convert(varchar,@curDiscPct)+ ','
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
										  +convert(varchar,@curDiscPct)+ ''',''' +convert(varchar,@SalesAmt)+ ''','''
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
										  ''',''' +convert(varchar,@curDiscPct)+ ''',''' +@MovingCode+ ''',''' +@ABCClass+ 
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
										  ''',''' +convert(varchar,@curDiscPct)+ ''',''' +convert(varchar,@SalesAmt)+ 
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
										  ''',''' +convert(varchar,@curDiscPct)+ ''',''' +convert(varchar,@SalesAmt)+ 
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
								      ', TotPpnAmt = TotPpnAmt + ' +convert(varchar,@PpnAmt)+ ', TotFinalSalesAmt = TotFinalSalesAmt + ' +convert(varchar,@TotSalesAmt)+ 
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
										  ''',''' +convert(varchar,@curDiscPct)+ ''',''' +convert(varchar,@SalesAmt)+ 
										  ''',''' +convert(varchar,@DiscAmt)+ ''',''' +convert(varchar,@NetSales)+ 
										  ''',''' +convert(varchar,@PPNAmt)+ ''',''' +convert(varchar,@TotSalesAmt)+ 
										  ''',''' +@ProductTypeMD+ ''',''' +@PartCategory+ ''',''' +@MovingCodeMD+ 
										  ''',''' +@ABCClassMD+ ''',''' +@curCreatedBy+ ''',''' +convert(varchar,@curCreatedDate,121)+ 
										  ''',''' +@curLastUpdateBy+ ''',''' +convert(varchar,@curLastUpdateDate,121)+ ''')'
					execute sp_executesql @sqlString

			 -- MD: Update data to table spTrnSFPJHdr
				set @sqlString = 'update ' +@DBNameMD+ '..spTrnSFPJHdr
								     set TotSalesQty = TotSalesQty + ' +convert(varchar,@CurQty)+ ', TotSalesAmt = TotSalesAmt + ' +convert(varchar,@SalesAmt)+ 
								      ', TotDiscAmt = TotDiscAmt + ' +convert(varchar,@DiscAmt)+ ', TotDppAmt = TotDppAmt + ' +convert(varchar,@NetSales)+ 
								      ', TotPpnAmt = TotPpnAmt + ' +convert(varchar,@PpnAmt)+ ', TotFinalSalesAmt = TotFinalSalesAmt + ' +convert(varchar,@TotSalesAmt)+ 
								 ' where CompanyCode=''' +@curCompanyMD+ ''' and BranchCode=''' +@curBranchMD+ ''' and PickingSlipNo=''' +@PLNo + ''''
					execute sp_executesql @sqlString 

			 -- MD: Insert data to table spTrnIMovement  (KEY: CompanyCode, BranchCode, DocNo, DocDate, CreatedDate => PartNo)
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
				set @sqlString = 'merge into ' +@DBNameMD+ '..arInterface as ar using ( values(''' 
								 +@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' +@FPJNo+ ''',''' 
								 +convert(varchar,@curDocDate,121)+ ''',''300'',''' 
								 +convert(varchar,@TotSalesAmt)+ ''',0,''' 
								 +convert(varchar,@curBranchCode)+ ''',''' +@TopCodeMD+ ''',''' 
								 +convert(varchar,@DueDateMD,111)+ ''',''INVOICE'',0,0,0,'''
								 +@SalesCodeMD+ ''',NULL,0,''POSTING'',''' 
								 +convert(varchar,@PostingDate,111)+ ''',''' 
								 +@AccNoArMD+ ''',NULL,''' +convert(varchar,@curDocDate,111)+ '''))
						as SRC (NewCompany, NewBranch, NewDocNo, NewDocDate, NewProfitCenterCode, NewNettAmt, 
								NewReceiveAmt, NewCustomerCode, NewTOPCode, NewDueDate, NewTypeTrans, NewBlockAmt, 
								NewDebetAmt, NewCreditAmt, NewSalesCode, NewLeasingCode, NewStatusFlag, NewCreateBy, 
								NewCreateDate, NewAccountNo, NewFakturPajakNo, NewFakturPajakDate)
						on ar.CompanyCode = SRC.NewCompany
					   and ar.BranchCode  = SRC.NewBranch
					   and ar.DocNo       = SRC.NewDocNo
					  when matched 
						   then update set NettAmt = NettAmt + NewNettAmt
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
				set @sqlString = 'merge into ' +@DBNameMD+ '..glInterface as gl using ( values(''' 
								+@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' +@FPJNo+ 
										       ''',''' +convert(varchar,@SeqNoGlMD)+ 
											   ''',''' +convert(varchar,@curDocDate,121)+ 
											   ''',''300'',''' +convert(varchar,@curDocDate,121)+ 
											   ''',''' +@AccNoArMD+ ''',''SPAREPART'',''INVOICE'',''' 
											   +@FPJNo+ ''',''' +convert(varchar,@TotSalesAmt)+ 
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
				set @sqlString = 'merge into ' +@DBNameMD+ '..glInterface as gl using ( values(''' 
								+@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' +@FPJNo+ 
										       ''',''' +convert(varchar,@SeqNoGlMD)+ 
											   ''',''' +convert(varchar,@curDocDate,121)+ 
											   ''',''300'',''' +convert(varchar,@curDocDate,121)+ 
											   ''',''' +@AccNoTaxOutMD+ ''',''SPAREPART'',''INVOICE'',''' 
											   +@FPJNo+ ''',0,''' +convert(varchar,@PPNAmt)+ 
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
										  ',' +convert(varchar,@RetailPriceNet)+ ',' +convert(varchar,@RetailPriceNet)+
										  ',' +convert(varchar,@curDiscPct)+ ',''' +@ABCClass+ ''',''' +@MovingCode+ 
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
										  ',' +convert(varchar,@RetailPriceNet)+ ',' +convert(varchar,@RetailPriceNet)+
										  ',' +convert(varchar,@curDiscPct)+ ',''' +@ABCClass+ ''',''' +@MovingCode+ 
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
				set @sqlString = 'update ' +@DBName+ '..spTrnPHPP
								     set TotPurchAmt     = TotPurchAmt     + ' +convert(varchar,@TotPurchaseAmt)+ 
									  ', TotNetPurchAmt  = TotNetPurchAmt  + ' +convert(varchar,@TotPurchaseNetAmt)+ 
									  ', TotTaxAmt       = TotTaxAmt       + ' +convert(varchar,@TotTaxAmt)+ 
									  ', DiffNetPurchAmt = DiffNetPurchAmt   ' +
									  ', DiffTaxAmt      = DiffTaxAmt        ' +
									  ', TotHPPAmt       = TotHPPAmt       + ' +convert(varchar,@TotPurchaseAmt)+ 
								 ' where CompanyCode=''' +@curCompanyCode+ ''' and BranchCode=''' +@curBranchCode+ ''' and HPPNo=''' +@HPPNo + ''''
					execute sp_executesql @sqlString 

				set @NetSales    = @SalesAmt-@DiscAmt
				set @PPNAmt      = round((@NetSales*0.1),0)
				set @TotSalesAmt = @NetSales+@PPNAmt

			 -- SD: Insert/Update data to table apInterface
				set @sqlString = 'merge into ' +@DBName+ '..apInterface as ap using ( values(''' 
								 +@curCompanyCode+ ''',''' +@curBranchCode+ ''',''' +@HPPNo+ ''',''' 
								 +@curProfitCenterCode+ ''',''' +convert(varchar,@curDocDate,121)+ ''',''' 
								 +@WRSNo+ ''',''' +convert(varchar,@curDocDate,121)+ ''',''' 
								 +convert(varchar,@TotPurchaseNetAmt)+ ''',0,''' +@curCompanyMD+ ''',''' 
								 +convert(varchar,@TotTaxAmt)+ ''',0,''' +@AccNoAP+ ''',''' 
								 +convert(varchar,@DueDateMD,111)+ ''',NULL,0,0,''POSTING'','''
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
						   then update set NetAmt = NetAmt + NewNetAmt
										 , PPNAmt = PPNAmt + NewPPNAmt
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
				set @sqlString = 'merge into ' +@DBName+ '..glInterface as gl using ( values(''' 
								 +@curCompanyCode+ ''',''' +@curBranchCode+ ''',''' +@HPPNo+ ''',''' 
								 +convert(varchar,@SeqNoGl)+ ''',''' +convert(varchar,@curDocDate,121)+ 
								 ''',''300'',''' +convert(varchar,@curDocDate,121)+ ''',''' +@AccNoTaxIn+
								 ''',''SPAREPART'',''PURCHASE'',''' +@HPPNo+ ''',''' 
								 +convert(varchar,@TotTaxAmt)+ ''',0,''TAX IN'',''' +@APNo+ 
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

				-- glInterface - AP
				set @SeqNoGL = @SeqNoGl + 1
				set @sqlString = 'merge into ' +@DBName+ '..glInterface as gl using ( values(''' 
								 +@curCompanyCode+ ''',''' +@curBranchCode+ ''',''' +@HPPNo+ ''',''' 
								 +convert(varchar,@SeqNoGl)+ ''',''' +convert(varchar,@curDocDate,121)+ 
								 ''',''300'',''' +convert(varchar,@curDocDate,121)+ ''',''' +@AccNoAP+
								 ''',''SPAREPART'',''PURCHASE'',''' +@HPPNo+ ''',0,'''
								 +convert(varchar,@TotPurchaseAmt)+ ''',''AP'',''' +@APNo+ 
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
							     or  (left(docno,3) in ('INC','INF','INI','INW','PLS','SOC','SPK','SSS','SSU') --'STR'
                                and  convert(varchar,DocDate,111)<=convert(varchar,@PostingDate,111))

		delete svSDMovement   where	 ProcessStatus = 1
							     or  (left(docno,3) in ('INC','INF','INI','INW','PLS','SOC','SPK','SSS','SSU') --'STR'
                                and  convert(varchar,DocDate,111)<=convert(varchar,@PostingDate,111))

		close cursvSDMovement
		deallocate cursvSDMovement
END
GO
