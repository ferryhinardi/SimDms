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
ALTER procedure [dbo].[usprpt_PostingMultiCompanySparePartReturn]
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
		declare @TotalRetur					numeric(18,0)
		declare @TotalDiscAmt				numeric(18,0)
		declare @TotalCostAmt				numeric(18,0)

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
		declare @DiscPct 					numeric(06,2)
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
			   and not exists (select top 1 1 from gnMstCompanyMapping 
			                    where gnMstCompanyMapping.CompanyMD=svSDMovement.CompanyCode)
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
										 '@PartCategory=PartCategory, @TypeOfGoods=TypeOfGoods, ' +
										 '@DiscPct=PurchDiscPct from ' 
										  +@DBName+ '..spMstItems where CompanyCode=''' 
										  +@curCompanyCode+ ''' and BranchCode=''' 
										  +@curBranchCode+''' and PartNo=''' 
										  +@curPartNo + ''''
					execute sp_executesql @sqlString, 
							N'@MovingCode		varchar(15)	 output,
							  @ABCClass			char(1)		 output,
							  @PartCategory		varchar(3)	 output,
							  @TypeOfGoods		varchar(15)  output
							  @DiscPct          numeric(6,2) output', 
							  @MovingCode		output,
							  @ABCClass			output,
							  @PartCategory		output,
							  @TypeOfGoods		output,
							  @DiscPct          output

			 -- SD: Location Code
				set @sqlString = N'select @LocationCode=LocationCode from ' +@DBName+ 
										  '..spMstItemLoc where CompanyCode=''' +@curCompanyCode+ 
										  ''' and BranchCode=''' +@curBranchCode+ ''' and PartNo=''' 
										  +@curPartNo + ''' and WarehouseCode=''00'''
					execute sp_executesql @sqlString, 
							N'@LocationCode		varchar(10) output', 
							  @LocationCode		output

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
										 +@DBNameMD+ '..gnMstCustomerClass c, '
										 +@DBNameMD+ '..gnMstCustomerProfitCenter p' +
										 ' where c.CompanyCode     =p.CompanyCode' +
										 '   and c.BranchCode      =p.BranchCode' +
										 '   and c.CustomerClass   =p.CustomerClass' +
										 '   and c.TypeOfGoods     ='''+@curTypeOfGoods+
										 ''' and c.ProfitCenterCode=''300''' +
										 '   and p.CompanyCode     ='''+@curCompanyMD+ 
										 ''' and p.BranchCode      ='''+@curBranchMD+
										 ''' and p.CustomerCode    ='''+@curBranchCode+ 
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
				set @TopCodeMD   = isnull(@TopCodeMD,'')
				set @SalesCodeMD = isnull(@SalesCodeMD,'')
				
			 -- MD: Top Days
				set @sqlString = N'select @TopDaysMD=ParaValue from ' 
										  +@DBNameMD+ '..gnMstLookUpDtl where CompanyCode=''' 
										  +@curCompanyMD+ ''' and CodeID=''TOPC'' and LookUpValue=''' 
										  +@TopCodeMD+ ''''
					execute sp_executesql @sqlString, 
										N'@TopDaysMD 		integer	output', 
										  @TopDaysMD 		output
				set @DueDateMD         = dateadd(day,isnull(@TopDaysMD,0),@curDocDate)

--select top 1 FPJNo=r.FPJNo, FPJDate=r.FPJDate, WRSNo=d.WRSNo, WRSDate=h.WRSDate, HPPNo=h.HPPNo, HPPDate=h.HPPDate 
--  from smgsmr..spTrnSRturHdr r
--  left join smgsmr..spTrnSFPJHdr f on f.CompanyCode=r.CompanyCode and f.BranchCode=r.BranchCode and f.FPJNo=r.FPJNo
--  left join smgsmr..spTrnPPOSHdr p on p.CompanyCode=f.CompanyCode and p.BranchCode=f.BranchCode and p.Remark=f.InvoiceNo
--  left join smgsmr..spTrnPRcvDtl d on d.CompanyCode=p.CompanyCode and d.BranchCode=p.BranchCode and d.DocNo=p.POSNo and d.PartNo=@curPartNo
--  left join smgsmr..spTrnPHPP h    on h.CompanyCode=d.CompanyCode and h.BranchCode=d.BranchCode and h.WRSNo=d.WRSNo
-- where r.CompanyCode=@curCompanyCode and r.BranchCode=@curBranchCode and r.ReturnNo=@curDocNo

--select top 1 FPJNoMD=h.FPJNo, FPJDateMD=h.FPJDate, FPJCentralNoMD=j.FPJCentralNo, FPJCentralDateMD=j.FPJCentralDate, SONoMD=o.DocNo, NPWPNoMD=j.FPJGovNo 
--  from smgsmr..spTrnSRturHdr r
--  left join smgsmr..spTrnSFPJHdr f  on f.CompanyCode=r.CompanyCode and f.BranchCode=r.BranchCode and f.FPJNo=r.FPJNo
--  left join smgsmr..spTrnPPOSHdr p  on p.CompanyCode=f.CompanyCode and p.BranchCode=f.BranchCode and p.Remark=f.InvoiceNo
--  left join isg..spTrnSORDHdr o     on o.CompanyCode=@curCompanyMD and o.BranchCode=@curBranchMD and o.OrderNo=p.POSNo
--  left join isg..spTrnSInvoiceDtl d on d.CompanyCode=o.CompanyCode and d.BranchCode=o.BranchCode and d.DocNo=o.DocNo and d.PartNo=@curPartNo
--  left join isg..spTrnSInvoiceHdr h on h.CompanyCode=d.CompanyCode and h.BranchCode=d.BranchCode and h.InvoiceNo=d.InvoiceNo
--  left join isg..spTrnSFPJHdr j     on j.CompanyCode=h.CompanyCode and j.BranchCode=h.BranchCode and j.FPJNo=h.FPJNo
-- where r.CompanyCode=@curCompanyCode and r.BranchCode=@curBranchCode and r.ReturnNo=@curDocNo

			 -- SD: FPJNo & FPJDate
				if left(@curDocNo,3) = 'RTR'  -- Retur Sparepart  >>spTrnSRturHdr (ReferenceNo=POS#)
					begin
						set @sqlString = N'select top 1 @FPJNo=r.FPJNo, @FPJDate=r.FPJDate, @WRSNo=d.WRSNo, ' +
											'@WRSDate=h.WRSDate, @HPPNo=h.HPPNo, @HPPDate=h.HPPDate from '
											+@DBName+ '..spTrnSRturHdr r left join ' +@DBName+ '..spTrnSFPJHdr f ' +
											'on f.CompanyCode=r.CompanyCode and f.BranchCode=r.BranchCode and ' + 
											'f.FPJNo=r.FPJNo left join ' +@DBName+ '..spTrnPPOSHdr p on ' +
											'p.CompanyCode=f.CompanyCode and p.BranchCode=f.BranchCode and ' +
											'p.Remark=f.InvoiceNo left join ' +@DBName+ '..spTrnPRcvDtl d on ' +
											'd.CompanyCode=p.CompanyCode and d.BranchCode=p.BranchCode and ' +
											'd.DocNo=p.POSNo and d.PartNo=''' +@curPartNo+ ''' left join ' +@DBName+ 
											'..spTrnPHPP h on h.CompanyCode=d.CompanyCode and h.BranchCode=d.BranchCode ' + 
											'and h.WRSNo=d.WRSNo where r.CompanyCode=''' +@curCompanyCode+ 
											'''and r.BranchCode=''' +@curBranchCode+ ''' and r.ReturnNo=''' +@curDocNo+ ''''
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

						--set @sqlString = N'select top 1 FPJNoMD=h.FPJNo, FPJDateMD=h.FPJDate, FPJCentralNoMD=j.FPJCentralNo, FPJCentralDateMD=j.FPJCentralDate, SONoMD=o.DocNo, NPWPNoMD=j.FPJGovNo 
						set @sqlString = N'select top 1 @FPJNoMD=h.FPJNo, @FPJDateMD=h.FPJDate, @FPJCentralNoMD=j.FPJCentralNo, ' +
											'@FPJCentralDateMD=j.FPJCentralDate, @SONoMD=o.DocNo, @NPWPNoMD=j.FPJGovNo from ' 
											+@DBName+ '..spTrnSRturHdr r left join ' +@DBName+ '..spTrnSFPJHdr f  on ' + 
											'f.CompanyCode=r.CompanyCode and f.BranchCode=r.BranchCode and f.FPJNo=r.FPJNo ' +
											'left join ' +@DBName+ '..spTrnPPOSHdr p  on p.CompanyCode=f.CompanyCode and ' +
											'p.BranchCode=f.BranchCode and p.Remark=f.InvoiceNo left join ' +@DBNameMD+ 
											'..spTrnSORDHdr o on o.CompanyCode=''' +@curCompanyMD+ ''' and o.BranchCode=''' +@curBranchMD+ 
											''' and o.OrderNo=p.POSNo left join ' +@DBNameMD+ '..spTrnSInvoiceDtl d on ' +
											'd.CompanyCode=o.CompanyCode and d.BranchCode=o.BranchCode and d.DocNo=o.DocNo ' +
											'and d.PartNo=''' +@curPartNo+ ''' left join ' +@DBNameMD+ '..spTrnSInvoiceHdr h on ' +
											'h.CompanyCode=d.CompanyCode and h.BranchCode=d.BranchCode and ' +
											'h.InvoiceNo=d.InvoiceNo left join ' +@DBNameMD+ '..spTrnSFPJHdr j on ' +
											'j.CompanyCode=h.CompanyCode and j.BranchCode=h.BranchCode and j.FPJNo=h.FPJNo '+
											'where r.CompanyCode=''' +@curCompanyCode+ ''' and r.BranchCode= '''
											+@curBranchCode+ ''' and r.ReturnNo=''' +@curDocNo+ ''''
						execute sp_executesql @sqlString, 
										 N'@FPJNoMD 			varchar(15)	output,
										   @FPJDateMD			datetime	output,
										   @FPJCentralNoMD		varchar(15) output,
										   @FPJCentralDateMD	datetime    output, 
										   @SONoMD				varchar(15) output,
										   @NPWPNoMD			varchar(50) output', 
										   @FPJNoMD 			output,
										   @FPJDateMD			output,
										   @FPJCentralNoMD		output,
										   @FPJCentralDateMD	output,
										   @SONoMD				output,
										   @NPWPNoMD			output
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

						set @sqlString = N'select top 1 @FPJNoMD=h.FPJNo, @FPJDateMD=h.FPJDate, @FPJCentralNoMD=f.FPJCentralNo, @FPJCentralDateMD=f.FPJCentralDate, @SONoMD=d.DocNo, @NPWPNoMD=f.FPJGovNo from ' 
						                  +@DBName+ '..spTrnSRturSrvHdr r left join ' +@DBNameMD+ '..spTrnSInvoiceDtl d on d.CompanyCode=''' +@curCompanyMD+ ''' and d.BranchCode=''' 
										  +@curBranchMD+ ''' and d.ReferenceNo=r.ReferenceNo and d.PartNo=''' +@curPartNo+ ''' left join ' +@DBNameMD+ 
										  '..spTrnSInvoiceHdr h on h.CompanyCode=d.CompanyCode and h.BranchCode=d.BranchCode and h.InvoiceNo=d.InvoiceNo left join ' 
										  +@DbNameMD+ '..spTrnSFPJHdr f on f.CompanyCode=h.CompanyCode and f.BranchCode=h.BranchCode and f.FPJNo=h.FPJNo where r.CompanyCode=''' 
										  +@curCompanyCode+ ''' and r.BranchCode=''' +@curBranchCode+ ''' and r.ReturnNo=''' +@curDocNo+ ''''
							execute sp_executesql @sqlString, 
										 N'@FPJNoMD 			varchar(15)	output,
										   @FPJDateMD			datetime	output,
										   @FPJCentralNoMD		varchar(15) output,
										   @FPJCentralDateMD	datetime    output, 
										   @SONoMD				varchar(15) output,
										   @NPWPNoMD			varchar(50) output', 
										   @FPJNoMD 			output,
										   @FPJDateMD			output,
										   @FPJCentralNoMD		output,
										   @FPJCentralDateMD	output,
										   @SONoMD				output,
										   @NPWPNoMD			output
					end

				set @FPJNo			  = isnull(@FPJNo,'')
				set @FPJDate		  = isnull(@FPJDate,'1900/01/01')
				set @FPJCentralNoMD   = isnull(@FPJCentralNoMD,'')
				set @FPJCentralDateMD = isnull(@FPJCentralDateMD,'1900/01/01')
				set @WRSNo			  = isnull(@WRSNo,'')
				set @WRSDate		  = isnull(@WRSDate,'1900/01/01')
				set @HPPNo			  = isnull(@HPPNo,'')
				set @HPPDate		  = isnull(@HPPDate,'1900/01/01')
				set @FPJNoMD		  = isnull(@FPJNoMD,'')
				set @FPJDateMD		  = isnull(@FPJDateMD,'1900/01/01')
				set @SONoMD			  = isnull(@SONoMD,'')
				set @NPWPNoMD		  = isnull(@NPWPNoMD,'')

				if @swCompanyCode <> @curCompanyCode or
				   @swBranchCode  <> @curBranchCode  or
				   @swDocNo		  <> @curDocNo
					begin
						set @swCompanyCode    = @curCompanyCode
						set @swBranchCode     = @curBranchCode
						set @swDocNo          = @curDocNo
						set @SeqNo		      = 0
						set @TotalNetReturAmt = 0
						set @TotalRetur       = 0
						set @TotalDiscAmt     = 0
						set @TotalCostAmt     = 0
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
/*
select * from smgslo..spTrnSRturHdr    where CompanyCode='6115204001' and BranchCode='6115204501' and ReturnNo in ('RTR/15/000052','RTR/15/000053','RTR/15/000054')
select * from smgslo..spTrnSInvoiceHdr where CompanyCode='6115204001' and BranchCode='6115204501' and FPJNo in ('FPJ/15/001775','FPJ/15/001829','FPJ/15/001623')
select * from smgslo..spTrnPPOSDtl     where CompanyCode='6115204001' and BranchCode='6115204501' and Note in ('INV/15/001623','INV/15/001775','INV/15/001829') and PartNo in ('59211B47E00N000','21120B09J10N000','34910-45F01-000')
					
select p.PartNo, p.DiscPct from smgslo..spTrnSRturHdr r, smgslo..spTrnSRturDtl d, smgslo..spTrnSInvoiceHdr i, smgslo..spTrnPPOSDtl p
 where r.CompanyCode=d.CompanyCode and r.BranchCode=d.BranchCode and r.ReturnNo=d.ReturnNo and r.CompanyCode=i.CompanyCode and r.BranchCode=i.BranchCode 
   and r.FPJNo=i.FPJNo and i.CompanyCode=p.CompanyCode and i.BranchCode=p.BranchCode and i.InvoiceNo=p.Note and d.PartNo=p.PartNo
   and r.CompanyCode='6115204001' 
   and r.BranchCode='6115204501' 
   and r.ReturnNo in ('RTR/15/000052','RTR/15/000053','RTR/15/000054')
   and d.PartNo in ('59211B47E00N000','21120B09J10N000','34910-45F01-000')
*/
					
				set @SeqNo            = @SeqNo + 1
				set @DiscPct          = isnull(@DiscPct,0.0)
				set @ReturAmt         = isnull((@curQty*@curRetailPriceMD),0)
			  --Gunakan @DiscPct karena mengambil discount dari spMstItems
			  --set @DiscAmt          = isnull(((@ReturAmt*@DiscPctMD)/100),0)
				set @DiscAmt          = isnull(((@ReturAmt*@DiscPct)/100),0)
				set @NetReturAmt      = @ReturAmt - @DiscAmt
				set @PPNAmt           = round((@NetReturAmt*0.1),0)
				set @TotReturAmt      = @NetReturAmt + @PPNAmt
				set @CostAmt          = isnull((@curQty * @curCostPriceMD),0)
				set @TotalNetReturAmt = @TotalNetReturAmt + @NetReturAmt
				set @TotalPPnAmt      = @TotalNetReturAmt * 0.1
				set @TotalReturAmt    = @TotalNetReturAmt + @TotalPPnAmt
				set @TotalRetur       = @TotalRetur   + @ReturAmt
				set @TotalDiscAmt     = @TotalDiscAmt + @DiscAmt
				set @TotalCostAmt     = @TotalCostAmt + @CostAmt

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
				set @sqlString = 'merge into ' +@DBName+ '..glInterface as gl using ( values(''' 
								 +@curCompanyCode+ ''',''' +@curBranchCode+ ''',''' +@RTPNo+ ''',''1'',''' 
								 +convert(varchar,@curDocDate,121)+ ''',''300'',''' 
								 +convert(varchar,@curDocDate,121)+ ''',''' +@ReturnPybAccNo+
								 ''',''SPAREPART'',''PURCHASERETURN'',''' +@RTPNo+ ''',''' 
								 +convert(varchar,@TotalReturAmt)+ ''',0,''PSEMENTARA'',''' +@HPPNo+ 
								 ''',NULL,0,''POSTING'',''' +convert(varchar,@PostingDate,111)+ 
								 ''',''POSTING'',''' +@currentDate+ '''))
						as SRC (NewCompany, NewBranch, NewDocNo, NewSeqNo, NewDocDate, NewProfitCenterCode, 
								NewAccDate, NewAccountNo, NewJournalCode, NewTypeJournal, NewApplyTo, 
								NewAmountDb, NewAmountCr, NewTypeTrans, NewBatchNo, NewBatchDate, 
								NewStatusFlag, NewCreateBy, NewCreateDate, NewLastUpdateBy, NewLastUpdateDate)
						on gl.CompanyCode=SRC.NewCompany
					   and gl.BranchCode =SRC.NewBranch
					   and gl.DocNo		 =SRC.NewDocNo
					   and gl.SeqNo      =SRC.NewSeqNo
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
				set @sqlString = 'merge into ' +@DBName+ '..glInterface as gl using ( values(''' 
								 +@curCompanyCode+ ''',''' +@curBranchCode+ ''',''' +@RTPNo+ ''',''2'',''' 
								 +convert(varchar,@curDocDate,121)+ ''',''300'',''' 
								 +convert(varchar,@curDocDate,121)+ ''',''' +@InventoryAccNo+
								 ''',''SPAREPART'',''PURCHASERETURN'',''' +@RTPNo+ ''',0,''' 
								 +convert(varchar,@TotalNetReturAmt)+ ''',''INVENTORY'',''' +@HPPNo+ 
								 ''',NULL,0,''POSTING'',''' +convert(varchar,@PostingDate,111)+ 
								 ''',''POSTING'',''' +@currentDate+ '''))
						as SRC (NewCompany, NewBranch, NewDocNo, NewSeqNo, NewDocDate, NewProfitCenterCode, 
								NewAccDate, NewAccountNo, NewJournalCode, NewTypeJournal, NewApplyTo, 
								NewAmountDb, NewAmountCr, NewTypeTrans, NewBatchNo, NewBatchDate, 
								NewStatusFlag, NewCreateBy, NewCreateDate, NewLastUpdateBy, NewLastUpdateDate)
						on gl.CompanyCode=SRC.NewCompany
					   and gl.BranchCode =SRC.NewBranch
					   and gl.DocNo		 =SRC.NewDocNo
					   and gl.SeqNo      =SRC.NewSeqNo
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
				set @sqlString = 'merge into ' +@DBName+ '..glInterface as gl using ( values(''' 
								 +@curCompanyCode+ ''',''' +@curBranchCode+ ''',''' +@RTPNo+ ''',''3'','''  
								 +convert(varchar,@curDocDate,121)+ ''',''300'',''' 
								 +convert(varchar,@curDocDate,121)+ ''',''' +@TaxInAccNo+
								 ''',''SPAREPART'',''PURCHASERETURN'',''' +@RTPNo+ ''',0,''' 
								 +convert(varchar,@TotalPPnAmt)+ ''',''PPN'',''' +@HPPNo+ 
								 ''',NULL,0,''POSTING'',''' +convert(varchar,@PostingDate,111)+ 
								 ''',''POSTING'',''' +@currentDate+ '''))
						as SRC (NewCompany, NewBranch, NewDocNo, NewSeqNo, NewDocDate, NewProfitCenterCode, 
								NewAccDate, NewAccountNo, NewJournalCode, NewTypeJournal, NewApplyTo, 
								NewAmountDb, NewAmountCr, NewTypeTrans, NewBatchNo, NewBatchDate, 
								NewStatusFlag, NewCreateBy, NewCreateDate, NewLastUpdateBy, NewLastUpdateDate)
						on gl.CompanyCode=SRC.NewCompany
					   and gl.BranchCode =SRC.NewBranch
					   and gl.DocNo		 =SRC.NewDocNo
					   and gl.SeqNo      =SRC.NewSeqNo
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

			 -- MD: Update data to table spTrnSRturHdr
				set @sqlString = 'update ' +@DBNameMD+ '..spTrnSRturHdr set' +
										   '  TotReturQty=TotReturQty+' +convert(varchar,@curQty)+
										   ', TotReturAmt=ToTReturAmt+' +convert(varchar,@ReturAmt)+
										   ', TotDiscAmt=TotDiscAmt+'   +convert(varchar,@DiscAmt)+
										   ', TotDPPAmt='				+convert(varchar,@TotalNetReturAmt)+
										   ', TotPPNAmt='				+convert(varchar,@TotalPPnAmt)+
										   ', TotFinalReturAmt='		+convert(varchar,@TotalReturAmt)+
										   ', TotCostAmt=TotCostAmt+'   +convert(varchar,@CostAmt)+
										   ' where CompanyCode='''+@curCompanyMD+''' and BranchCode='''
										   +@curBranchMD+''' and ReturnNo=''' +@RTSNo+ ''''
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
				set @sqlString = 'merge into ' +@DBNameMD+ '..glInterface as gl using ( values(''' 
											   +@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' +@RTSNo+ 
										       ''',''1'',''' +convert(varchar,@curDocDate,121)+ 
											   ''',''300'',''' +convert(varchar,@curDocDate,121)+ 
											   ''',''' +@AccNoReturnMD+ ''',''SPAREPART'',''RETURN'',''' 
											   +@FPJNo+ ''',''' +convert(varchar,@TotalRetur)+ 
											   ''',0,''RETURN'',NULL,NULL,0,''POSTING'',''' 
											   +convert(varchar,@PostingDate,111)+ 
											   ''',''POSTING'',''' +@currentDate+ '''))
						as SRC (NewCompany, NewBranch, NewDocNo, NewSeqNo, NewDocDate, NewProfitCenterCode, 
								NewAccDate, NewAccountNo, NewJournalCode, NewTypeJournal, NewApplyTo, 
								NewAmountDb, NewAmountCr, NewTypeTrans, NewBatchNo, NewBatchDate, 
								NewStatusFlag, NewCreateBy, NewCreateDate, NewLastUpdateBy, NewLastUpdateDate)
						on gl.CompanyCode=SRC.NewCompany
					   and gl.BranchCode =SRC.NewBranch
					   and gl.DocNo		 =SRC.NewDocNo
					   and gl.SeqNo      =SRC.NewSeqNo
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
				set @sqlString = 'merge into ' +@DBNameMD+ '..glInterface as gl using ( values(''' 
								+@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' +@RTSNo+ 
										       ''',''2'',''' +convert(varchar,@curDocDate,121)+ 
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
						on gl.CompanyCode=SRC.NewCompany
					   and gl.BranchCode =SRC.NewBranch
					   and gl.DocNo		 =SRC.NewDocNo
					   and gl.SeqNo      =SRC.NewSeqNo
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
				set @sqlString = 'merge into ' +@DBNameMD+ '..glInterface as gl using ( values(''' 
								+@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' +@RTSNo+ 
										       ''',''3'',''' +convert(varchar,@curDocDate,121)+ 
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
						on gl.CompanyCode=SRC.NewCompany
					   and gl.BranchCode =SRC.NewBranch
					   and gl.DocNo		 =SRC.NewDocNo
					   and gl.SeqNo      =SRC.NewSeqNo
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
				set @sqlString = 'merge into ' +@DBNameMD+ '..glInterface as gl using ( values(''' 
								+@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' +@RTSNo+ 
										       ''',''4'',''' +convert(varchar,@curDocDate,121)+ 
											   ''',''300'',''' +convert(varchar,@curDocDate,121)+ 
											   ''',''' +@AccNoDisc1MD+ ''',''SPAREPART'',''RETURN'',''' 
											   +@FPJNo+ ''',0,''' +convert(varchar,@TotalDiscAmt)+ 
											   ''',''DISC1'',NULL,NULL,0,''POSTING'',''' 
											   +convert(varchar,@PostingDate,111)+ 
											   ''',''POSTING'',''' +@currentDate+ '''))
						as SRC (NewCompany, NewBranch, NewDocNo, NewSeqNo, NewDocDate, NewProfitCenterCode, 
								NewAccDate, NewAccountNo, NewJournalCode, NewTypeJournal, NewApplyTo, 
								NewAmountDb, NewAmountCr, NewTypeTrans, NewBatchNo, NewBatchDate, 
								NewStatusFlag, NewCreateBy, NewCreateDate, NewLastUpdateBy, NewLastUpdateDate)
						on gl.CompanyCode=SRC.NewCompany
					   and gl.BranchCode =SRC.NewBranch
					   and gl.DocNo		 =SRC.NewDocNo
					   and gl.SeqNo      =SRC.NewSeqNo
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

				-- glInterface - INVENTORY
				set @sqlString = 'merge into ' +@DBNameMD+ '..glInterface as gl using ( values(''' 
								+@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' +@RTSNo+ 
										       ''',''5'',''' +convert(varchar,@curDocDate,121)+ 
											   ''',''300'',''' +convert(varchar,@curDocDate,121)+ 
											   ''',''' +@AccNoInventoryMD+ ''',''SPAREPART'',''RETURN'',''' 
											   +@FPJNo+ ''',''' +convert(varchar,@TotalCostAmt)+ 
											   ''',0,''INVENTORY'',NULL,NULL,0,''POSTING'',''' 
											   +convert(varchar,@PostingDate,111)+ 
											   ''',''POSTING'',''' +@currentDate+ '''))
						as SRC (NewCompany, NewBranch, NewDocNo, NewSeqNo, NewDocDate, NewProfitCenterCode, 
								NewAccDate, NewAccountNo, NewJournalCode, NewTypeJournal, NewApplyTo, 
								NewAmountDb, NewAmountCr, NewTypeTrans, NewBatchNo, NewBatchDate, 
								NewStatusFlag, NewCreateBy, NewCreateDate, NewLastUpdateBy, NewLastUpdateDate)
						on gl.CompanyCode=SRC.NewCompany
					   and gl.BranchCode =SRC.NewBranch
					   and gl.DocNo		 =SRC.NewDocNo
					   and gl.SeqNo      =SRC.NewSeqNo
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

				-- glInterface - COGS
				set @sqlString = 'merge into ' +@DBNameMD+ '..glInterface as gl using ( values(''' 
								+@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' +@RTSNo+ 
										       ''',''6'',''' +convert(varchar,@curDocDate,121)+ 
											   ''',''300'',''' +convert(varchar,@curDocDate,121)+ 
											   ''',''' +@AccNoCogsMD+ ''',''SPAREPART'',''RETURN'',''' 
											   +@FPJNo+ ''',0,''' +convert(varchar,@TotalCostAmt)+ 
											   ''',''COGS'',NULL,NULL,0,''POSTING'',''' 
											   +convert(varchar,@PostingDate,111)+ 
											   ''',''POSTING'',''' +@currentDate+ '''))
						as SRC (NewCompany, NewBranch, NewDocNo, NewSeqNo, NewDocDate, NewProfitCenterCode, 
								NewAccDate, NewAccountNo, NewJournalCode, NewTypeJournal, NewApplyTo, 
								NewAmountDb, NewAmountCr, NewTypeTrans, NewBatchNo, NewBatchDate, 
								NewStatusFlag, NewCreateBy, NewCreateDate, NewLastUpdateBy, NewLastUpdateDate)
						on gl.CompanyCode=SRC.NewCompany
					   and gl.BranchCode =SRC.NewBranch
					   and gl.DocNo		 =SRC.NewDocNo
					   and gl.SeqNo      =SRC.NewSeqNo
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


