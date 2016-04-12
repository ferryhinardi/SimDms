-- POSTING TRANSACTION MULTI COMPANY - SALES
-- -----------------------------------------
-- Created  by : HTO, 2014
-- Revision by : HTO, January 2015
-- -----------------------------------------
-- This procedure used for 
--		4-Wheeler : BIT-SBT, SBSM-SSBT, BAT-SAT, SIT-SST, UIB-SIT
--		2-Wheeler : IJMG,SVMG,ISG,ISMG-SMG
-- -----------------------------------------------------------------------
-- execute [usprpt_PostingMultiCompanySales] '6006400001','2014/11/08','0'
-- -----------------------------------------------------------------------

ALTER procedure [dbo].[usprpt_PostingMultiCompanySales]
	@CompanyCode	varchar(15),
	@PostingDate	datetime,
	@Status			varchar(1)	output
AS	

BEGIN

	  --declare @CompanyCode				varchar(15) = '6006400001'
	  --declare @PostingDate				datetime	= '2015/03/08'
	  --declare @Status						varchar(1)

  -- Check Tax/Seri Pajak online
		declare @TaxCompany				varchar(15)
		declare @TaxBranch				varchar(15)
		declare @TaxDB					varchar(50)
		declare @TaxTransCode			varchar(3)
		declare @swTaxBranch			varchar(15) = ''
		declare @swTaxDoc				varchar(15) = ''
		declare @TaxSeq					bigint
		declare @TaxSeqNo				int
		declare @SeriPajakNo			varchar(50) = ''
		declare @sqlString				nvarchar(max)

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
		declare @RTPNo					varchar(15)
		declare @RTNNo					varchar(15) = ''
		declare @VTONo					varchar(15) = ''
		declare @VTINo					varchar(15) = ''
		declare @DueDate				datetime
		declare @SeqNo					integer

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

		declare @swCompanyCode			varchar(15)  = ' '
		declare @swBranchCode			varchar(15)  = ' '
		declare @swDocNo 				varchar(15)  = ' '
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
			  from omSDMovement sd, gnMstDocument doc
             where sd.CompanyMD        =doc.CompanyCode
			   and sd.BranchMD         =doc.BranchCode
			   and doc.DocumentType    ='IVU' 
			   and doc.ProfitCenterCode='100'
			   and left(sd.DocNo,len(doc.DocumentPrefix))=doc.DocumentPrefix
			   and convert(varchar,DocDate,111)<=convert(varchar,@PostingDate,111)
			   and ProcessStatus=0
             order by sd.CompanyCode, sd.BranchCode, sd.DocNo, 
					  sd.SalesModelCode, sd.SalesModelYear, sd.ColourCode
		open curomSDMovement

		fetch next from curomSDMovement
			  into @curCompanyCode, @curBranchCode, @curDocNo, @curDocDate, @curSeq, @curSalesModelCode,
				   @curSalesModelYear, @curChassisCode, @curChassisNo, @curEngineCode, @curEngineNo, 
				   @curColourCode, @curWarehouseCode, @curCustomerCode, @curQtyFlag, @curCompanyMD, 
				   @curBranchMD, @curWarehouseMD, @curStatus, @curProcessStatus, @curProcessDate, 
				   @curCreatedBy, @curCreatedDate, @curLastUpdateBy, @curLastUpdateDate

		while (@@FETCH_STATUS =0)
			begin

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

				if @swCompanyCode <> @curCompanyCode or
				   @swBranchCode  <> @curBranchCode  or
				   @swDocNo		  <> @curDocNo
					begin
						set @swCompanyCode = @curCompanyCode
						set @swBranchCode  = @curBranchCode
						set @swDocNo	   = @curDocNo
						set @SeqNo		   = 0
		
					 -- MD Database & SD Database from gnMstCompanyMapping
						select @DBNameMD=DBMD, @DBName=DBName from gnMstCompanyMapping where CompanyCode=@CurCompanyCode and BranchCode =@curBranchCode

					 -- Centralize unit purchasing for SBT, SMG, SIT, SST dealer
						set @sqlString = N'select @CentralBranch=BranchCode from ' +@DBName+ '..gnMstOrganizationDtl ' +  
											'where CompanyCode=''' +@curCompanyCode+ ''' and isBranch=''0'''
							execute sp_executesql @sqlString, 
									N'@CentralBranch varchar(15) output', @CentralBranch output

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

						set @sqlString = N'select @CustomerClass=CustomerClass, @TaxTransCode=TaxTransCode, @TopCode=TopCode, 
										 @GroupPriceCode=GroupPriceCode, @SalesCode=SalesCode, @SalesType=SalesType from ' 
										 +@DBNameMD+ '..gnMstCustomerProfitCenter where CompanyCode='''
										 +@curCompanyMD+ ''' and BranchCode=''' +@curBranchMD+ ''' and CustomerCode='''
										 +@CentralBranch+ ''' and ProfitCenterCode=''100'''
							execute sp_executesql @sqlString, 
									N'@CustomerClass	varchar(15) output,  @TaxTransCode		varchar(3)  output,
									  @TopCode			varchar(15) output,  @GroupPriceCode	varchar(15) output,
									  @SalesCode		varchar(15) output,  @SalesType		char(1)		output', 
									  @CustomerClass  output, @TaxTransCode output, @TopCode   output, 
									  @GroupPriceCode output, @SalesCode    output, @SalesType output

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
						set @GroupPriceCode  = isnull(@GroupPriceCode,'')
						set @SalesCode       = isnull(@SalesCode,'')
						set @SalesType	  	 = isnull(@SalesType,'')
						set @SalesType		 = isnull(@TopDays,0)
						set @DueDate		 = dateadd(day,isnull(@TopDays,0),@curDocDate)

					 -- MD: Generate Sales Order No for MD
						execute [usprpt_PostingMultiCompany4DocNo] @curCompanyMD, @curBranchMD, @DBNameMD, 'SOR', @SONo output

					 -- SD: Insert data to table omTrPurchasePO
						execute [usprpt_PostingMultiCompany4DocNo] @curCompanyCode, @CentralBranch, @DBName, 'PUR', @PONo output
						set @sqlString = 'insert into ' +@DBName+ '..omTrPurchasePO 
												(CompanyCode, BranchCode, PONo, PODate, RefferenceNo, RefferenceDate, SupplierCode, 
												 BillTo, ShipTo, Remark, Status, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate, 
												 isLocked, LockingBy, LockingDate)
											values(''' +@curCompanyCode+ ''',''' +@CentralBranch+ ''',''' +@PONo+ ''',''' 
										           +convert(varchar,@curDocDate,121)+ ''',''' +@SONo+ ''',''' 
												   +convert(varchar,@curDocDate,121)+ ''',''' +@curBranchMD+ ''',''' 
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
											values(''' +@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' +@SONo+ ''',''' +convert(varchar,@curDocDate,121)+ 
												   ''',''' +@SalesType+ ''',''' +@PONo+ ''',''' +convert(varchar,@curDocDate,121)+ ''','''
												   +@CentralBranch+ ''',''' +@TopCode+ ''',''' +convert(varchar,@TopDays)+ ''',''' +@CentralBranch+ ''',''' 
												   +@curBranchCode+ ''',NULL,NULL,''POSTING'',''' +@VehWHMD+ ''',0,NULL,''' +@GroupPriceCode+
												   ''',NULL,NULL,0,NULL,NULL,0,''' +@PONo+ 
												   ''',NULL,NULL,NULL,2,''POSTING'',''' +convert(varchar,@curDocDate,121)+ ''',NULL,NULL,''POSTING'','''
												   +convert(varchar,@curDocDate,121)+ ''',''POSTING'',''' +convert(varchar,@curDocDate,121)+ 
												   ''',0,NULL,NULL,''' +@SalesCode+ ''',0,NULL,''POSTING'',''POSTING'',''POSTING'')'
							execute sp_executesql @sqlString

					 -- MD: Insert data to table omTrSalesDO
						execute [usprpt_PostingMultiCompany4DocNo] @curCompanyMD, @curBranchMD, @DBNameMD, 'DOS', @DONo output
						set @sqlString = 'insert into ' +@DBNameMD+ '..omTrSalesDO 
												(CompanyCode, BranchCode, DONo, DODate, SONo, CustomerCode, ShipTo,
												 WareHouseCode, Expedition, Remark, Status, CreatedBy, CreatedDate, 
												 LastUpdateBy, LastUpdateDate, isLocked, LockingBy, LockingDate)
											values(''' +@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' +@DONo+ ''',''' 
												   +convert(varchar,@curDocDate,121)+ ''',''' +@SONo+ ''',''' 
												   +@CentralBranch+ ''',''' +@CentralBranch+ ''',''' +@VehWHMD+ ''',NULL,'''
												   +@curDocNo+ ''',''2'',''POSTING'',''' +convert(varchar,@curDocDate,121)+ 
												   ''',''POSTING'',''' +@CurrentDate+ ''',0,NULL,NULL)'
							execute sp_executesql @sqlString

					 -- MD: Insert data to table omTrSalesBPK
						execute [usprpt_PostingMultiCompany4DocNo] @curCompanyMD, @curBranchMD, @DBNameMD, 'BPK', @BPKNo output
						set @sqlString = 'insert into ' +@DBNameMD+ '..omTrSalesBPK 
												(CompanyCode, BranchCode, BPKNo, BPKDate, DONo, SONo, CustomerCode, 
												 ShipTo, WareHouseCode, Expedition, Remark, Status, CreatedBy, CreatedDate, 
												 LastUpdateBy, LastUpdateDate, isLocked, LockingBy, LockingDate)
											values(''' +@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' +@BPKNo+ ''',''' 
												   +convert(varchar,@curDocDate,121)+ ''',''' +@DONo+ ''',''' +@SONo+ 
												   ''',''' +@CentralBranch+ ''',''' +@CentralBranch+ ''',''' +@VehWHMD+ ''',NULL,'''
												   +@curDocNo+ ''',''2'',''POSTING'',''' +convert(varchar,@curDocDate,121)+ 
												   ''',''POSTING'',''' +@CurrentDate+ ''',0,NULL,NULL)'
							execute sp_executesql @sqlString

					 -- MD: Insert data to table omTrSalesInvoice

						--Tax / Seri Pajak Numbering
						if @curBranchCode<>@swTaxBranch or left(@curDocNo,3)<>left(@swTaxDoc,3)
							begin
								set @swTaxBranch = @curBranchCode
								set @swTaxDoc	 = @curDocNo

								set @sqlString = N'select top 1 @TaxSeq=FPJSeqNo from ' 
												+@TaxDb+ '..gnMstFPJSeqNo where CompanyCode=''' +@TaxCompany+ ''' and BranchCode=''' 
												+@TaxBranch+ ''' and Year=''' +convert(varchar,year(@PostingDate))+ ''' and convert(varchar,EffectiveDate,111)<=''' 
												+convert(varchar,@PostingDate,111)+ ''' order by CompanyCode, BranchCode, Year, SeqNo'
									execute sp_executesql @query=@sqlString, @params= N'@TaxSeq bigint output', @TaxSeq = @TaxSeq output

								set @sqlString = N'select top 1 @TaxSeqNo=SeqNo from ' 
												+@TaxDb+ '..gnMstFPJSeqNo where CompanyCode=''' +@TaxCompany+ ''' and BranchCode=''' 
												+@TaxBranch+ ''' and Year=''' +convert(varchar,year(@PostingDate))+ '''and convert(varchar,EffectiveDate,111)<=''' 
												+convert(varchar,@PostingDate,111)+ ''' order by CompanyCode, BranchCode, Year, SeqNo'
									execute sp_executesql @query=@sqlString, @params= N'@TaxSeqNo int output', @TaxSeqNo = @TaxSeqNo output

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

						execute [usprpt_PostingMultiCompany4DocNo] @curCompanyMD, @curBranchMD, @DBNameMD, 'IVU', @INVNo output
						set @sqlString = 'insert into ' +@DBNameMD+ '..omTrSalesInvoice 
												(CompanyCode, BranchCode, InvoiceNo, InvoiceDate, SONo, CustomerCode, 
												 BillTo, FakturPajakNo, FakturPajakDate, DueDate, isStandard, Remark, 
												 Status, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate, 
												 isLocked, LockingBy, LockingDate)
											values(''' +@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' +@INVNo+ 
												   ''',''' +convert(varchar,@curDocDate,121)+ ''',''' +@SONo+ 
												   ''',''' +@CentralBranch+ ''','''  +@CentralBranch+ ''',''' 
												   +@SeriPajakNo+ ''',''' +convert(varchar,@curDocDate,121)+ ''',''' 
												   +convert(varchar,@DueDate)+ ''',1,''' +@curDocNo+ 
												   ''',''5'',''POSTING'',''' +convert(varchar,@curDocDate,121)+ 
												   ''',''POSTING'',''' +@CurrentDate+ ''',0,NULL,NULL)'
							execute sp_executesql @sqlString

					 -- MD: Insert data to table omTrSalesReturn, Transfer In, Transfer Out
					    --@RTNNo, @VTINo, @VTONo
						--execute [usprpt_PostingMultiCompany4DocNo] @curCompanyMD, @curBranchMD, @DBNameMD, 'RTS', @RTNNo output
						--execute [usprpt_PostingMultiCompany4DocNo] @curCompanyMD, @curBranchMD, @DBNameMD, 'RTS', @VTINo output
						--execute [usprpt_PostingMultiCompany4DocNo] @curCompanyMD, @curBranchMD, @DBNameMD, 'RTS', @VTONo output

				     -- SD: Collect Warehouse Information from gnMstLookupDtl
						set @sqlString = N'select @WHSD=LookUpValue from ' +@DBName+ '..gnMstLookUpDtl 
											where CompanyCode=''' +@curCompanyCode+ ''' and CodeID=''MPWH'' and ParaValue=''' +@CentralBranch+ ''''
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
												   +@curBranchMD+ ''',''' +@CentralBranch+ ''',''' +@DONo+ ''',''' 
												   +convert(varchar,@curDocDate,121)+ ''',''' +@BPKNo+ ''','''
												   +convert(varchar,@curDocDate,121)+ ''',''' +@WHSD+ ''','''
												   +@curBranchMD+ ''',''2'',''' +@curDocNo+ ''',''2'',''POSTING'',''' 
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
												   +convert(varchar,@curDocDate,121)+ ''',''' +@PONo+ ''',''' +@curBranchMD+ 
												   ''',''' +@CentralBranch+ ''',''' +@INVNo+ ''',''' 
												   +convert(varchar,@curDocDate,121)+ ''',''' +@SeriPajakNo+ ''','''
												   +convert(varchar,@curDocDate,121)+ ''',''' +convert(varchar,@DueDate)+ 
												   ''',NULL,''5'',''POSTING'',''' +convert(varchar,@PostingDate,111)+ 
												   ''',''POSTING'',''' +@currentDate+ ''',0,NULL,NULL)' 
							execute sp_executesql @sqlString
					end

				set @SeqNo = @SeqNo + 1
			 -- Collect Vehicle Price from omPriceListBranches
			    set @sqlString = N'select top 1 
										@RetailPriceIncludePPN = RetailPriceIncludePPN, 
										@DiscPriceIncludePPN   = DiscPriceIncludePPN,
										@NetSalesIncludePPN    = NetSalesIncludePPN, 
										@RetailPriceExcludePPN = RetailPriceExcludePPN,
										@DiscPriceExcludePPN   = DiscPriceExcludePPN, 
										@NetSalesExcludePPN    = NetSalesExcludePPN,
										@PPNBeforeDisc         = PPNBeforeDisc, 
										@PPNAfterDisc          = PPNAfterDisc, 
										@PPnBMPaid             = PPnBMPaid from '
										+@DBName+ '..omPriceListBranches where CompanyCode=''' +@curCompanyCode+ 
										''' and BranchCode=''' +@CentralBranch+ ''' and SupplierCode=''' +@curBranchMD+ 
										''' and GroupPrice=''D'' and SalesModelCode=''' +@curSalesModelCode+
										''' and SalesModelYear=''' +convert(varchar,@curSalesModelYear)+ 
										''' and EffectiveDate<='''+convert(varchar,@curDocDate,111)+ 
										''' and isStatus=1 order by EffectiveDate desc'
						execute sp_executesql @sqlString, 
								N'@RetailPriceIncludePPN numeric(18,0)	output, 
								  @DiscPriceIncludePPN numeric(18,0)	output,
								  @NetSalesIncludePPN numeric(18,0)		output, 
								  @RetailPriceExcludePPN numeric(18,0)	output, 
								  @DiscPriceExcludePPN numeric(18,0)	output, 
								  @NetSalesExcludePPN numeric(18,0)		output, 
								  @PPNBeforeDisc numeric(18,0)			output, 
								  @PPNAfterDisc numeric(18,0)			output,
								  @PPnBMPaid numeric(18,0)				output', 
								  @RetailPriceIncludePPN				output, 
								  @DiscPriceIncludePPN					output,
								  @NetSalesIncludePPN 					output, 
								  @RetailPriceExcludePPN				output, 
								  @DiscPriceExcludePPN					output, 
								  @NetSalesExcludePPN					output, 
								  @PPNBeforeDisc						output, 
								  @PPNAfterDisc							output,
								  @PPnBMPaid							output

				set @RetailPriceIncludePPN = isnull(@RetailPriceIncludePPN,0)
				set @DiscPriceIncludePPN   = isnull(@DiscPriceIncludePPN,0)
				set @NetSalesIncludePPN    = isnull(@NetSalesIncludePPN,0)
				set @RetailPriceExcludePPN = isnull(@RetailPriceExcludePPN,0)
				set @DiscPriceExcludePPN   = isnull(@DiscPriceExcludePPN,0)
				set @NetSalesExcludePPN	   = isnull(@NetSalesExcludePPN,0)
				set @PPNBeforeDisc		   = isnull(@PPNBeforeDisc,0)
				set @PPNAfterDisc		   = isnull(@PPNAfterDisc,0)
				set @PPnBMPaid			   = isnull(@PPnBMPaid,0)

			 -- SD: Insert data to table omTrPurchasePOModel
				set @sqlString = 'merge into ' +@DBName+ '..omTrPurchasePOModel as POM using (values(''' 
									+@curCompanyCode+ ''',''' +@CentralBranch+ ''',''' +@PONo+ ''',''' 
									+@curSalesModelCode+ ''',''' +convert(varchar,@curSalesModelYear)+ ''','''
									+convert(varchar,@RetailPriceExcludePPN)+ ''',''' +convert(varchar,@PPNBeforeDisc)+ 
									''',''0'',''' +convert(varchar,@RetailPriceIncludePPN)+ ''',''' 
									+convert(varchar,@DiscPriceExcludePPN)+ ''',''' +convert(varchar,@DiscPriceIncludePPN)+
									''',''' +convert(varchar,@NetSalesExcludePPN)+ ''',''' +convert(varchar,@PPNAfterDisc)+
									''',''0'',''' +convert(varchar,@NetSalesIncludePPN)+ ''',''' +convert(varchar,@PPNBMPaid)+
									''',''0'',''0'',1,1,''' +@SONo+ ''',''' +@curCreatedBy+ ''',''' 
									+convert(varchar,@curCreatedDate,121)+ ''',''' +@curLastUpdateBy+ ''',''' 
									+convert(varchar,@curLastUpdateDate,121)+ '''))
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
									+@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' +@SONo+ ''',''' 
									+@curSalesModelCode+ ''',''' +convert(varchar,@curSalesModelYear)+ ''','''
									+@curChassisCode+ ''','''+convert(varchar,@RetailPriceExcludePPN)+ ''',''' 
									+convert(varchar,@PPNBeforeDisc)+ ''',''0'',''' 
									+convert(varchar,@RetailPriceIncludePPN)+ ''',''' 
									+convert(varchar,@DiscPriceExcludePPN)+ ''',''' 
									+convert(varchar,@DiscPriceIncludePPN)+ ''',''' 
									+convert(varchar,@NetSalesExcludePPN)+ ''',''' 
									+convert(varchar,@PPNAfterDisc)+ ''',''0'',''' 
									+convert(varchar,@NetSalesIncludePPN)+ ''',''0'',''0'',1,1,''' 
									+@curDocNo+ ''',''' +@curCreatedBy+ ''',''' 
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
									+@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' +@SONo+ ''',''' 
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
								  values(''' +@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' +@SONo+ ''',''' 
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
									+@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' +@SONo+ ''',''' 
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
								  values(''' +@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' +@DONo+ ''',''' 
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
									+@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' +@BPKNo+ ''',''' 
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
								  values(''' +@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' +@BPKNo+ ''',''' 
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
									+@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' 
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
									+@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' +@INVNo+ ''',''' 
									+@BPKNo+ ''',''' +@curSalesModelCode+ ''',''' 
									+convert(varchar,@curSalesModelYear)+ ''',1,'''
									+convert(varchar,@RetailPriceExcludePPN)+ ''',''' 
									+convert(varchar,@DiscPriceExcludePPN)+ ''',''' 
									+convert(varchar,@DiscPriceIncludePPN)+ ''',''' 
									+convert(varchar,@NetSalesExcludePPN)+ ''',''' 
									+convert(varchar,@PPNAfterDisc)+ ''',''0'',''' 
									+convert(varchar,@NetSalesIncludePPN)+ ''','''
									+convert(varchar,@PPNBMPaid) + ''',0,0,0,'''
									+@curDocNo+ ''',''' +@curCreatedBy+ ''',''' 
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
								  values(''' +@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' +@INVNo+ ''',''' +@BPKNo+ ''',''' 
										 +@curSalesModelCode+ ''',''' +convert(varchar,@curSalesModelYear)+ ''','''  
										 +convert(varchar,@SeqNo)+ ''',''' +convert(varchar,@curColourCode)+ ''',''' 
										 +convert(varchar,@curChassisCode)+ ''',''' +convert(varchar,@curChassisNo)+ 
										 ''',''' +@curEngineCode+ ''',''' +convert(varchar,@curEngineNo)+ ''',''' 
										 +convert(varchar,@VehCOGS)+ ''',0,''' +@curCreatedBy+ ''',''' 
										 +convert(varchar,@curCreatedDate,121)+ ''',''' +@curLastUpdateBy+ ''',''' 
										 +convert(varchar,@curLastUpdateDate,121)+ ''')'
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
									+convert(varchar,@DiscPriceExcludePPN)+ ''',''' 
									+convert(varchar,@NetSalesExcludePPN)+ ''',''' 
									+convert(varchar,@PPNAfterDisc)+ ''',0,''' 
									+convert(varchar,@NetSalesIncludePPN)+ ''',''' 
									+convert(varchar,@PPnBMPaid)+ ''',0,0,NULL,''' +@curCreatedBy+ ''',''' 
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

/*				
			 -- SD: Insert data to table omTrPurchaseReturn
			 -- SD: Insert data to table omTrPurchaseReturnDetail
			 -- SD: Insert data to table omTrPurchaseReturnDetailModel
			 -- SD: Insert data to table omTrPurchaseReturnSubDetail

			 -- MD: Insert data to table omTrSalesReturn
			 -- MD: Insert data to table omTrSalesReturnBPK
			 -- MD: Insert data to table omTrSalesReturnDetailModel
			 -- MD: Insert data to table omTrSalesReturnVIN

			USE POSTING JOURNAL IN UTILITY SALES MODULE 
			FOR UPDATE DATA TO TABLE ARINTERFACE & GLINTERFACE
			 -- MD: Update data to table arInterface
			 -- MD: Update data to table glInterface
			 -- SD: Update data to table apInterface
			 -- SD: Update data to table glInterface
*/

			 -- MD: Update data to table omMstVehicle
				set @sqlString = 'update ' +@DBNameMD+ '..omMstVehicle
										set SONo='''+@SONo+ ''', SOReturnNo='''+@RTNNo+ ''', DONo='''+@DONo+ ''', BPKNo='''
											+@BPKNo+ ''', InvoiceNo='''+@INVNo+ ''', TransferOutNo='''+@VTONo+ 
											''', TransferInNo='''+@VTINo+ ''', BPKDate='''+convert(varchar,@curDocDate,121)+
									''' where CompanyCode='''+@curCompanyMD+''' and ChassisCode='''
											+@curChassisCode+ ''' and ChassisNo='+convert(varchar,@curChassisNo)
					execute sp_executesql @sqlString

			 -- SD: Insert / Update data to table omMstVehicle
				set @sqlString = 'merge into ' +@DBName+ '..omMstVehicle as VEH using (values(''' 
									+@curCompanyCode+ ''',''' +@curChassisCode+ ''',''' +convert(varchar,@curChassisNo)+ 
									''',''' +@curEngineCode+ ''',''' +convert(varchar,@curEngineNo)+ ''',''' 
									+@curSalesModelCode+ ''',''' +convert(varchar,@curSalesModelYear)+ ''',''' 
									+@curColourCode+ ''',''' +@VehServiceBookNo+ ''',''' +@VehKeyNo+ ''',''' 
									+convert(varchar,@VehCOGS)+ ''',''' +convert(varchar,@VehCOGSOthers)+ ''',''' 
									+convert(varchar,@VehCOGSKaroseri)+ ''',''' +convert(varchar,@VehPpnBMBuyPaid)+ 
									''',''' +convert(varchar,@VehPpnBmBuy)+ ''',''' +convert(varchar,@VehSalesNetAmt)+ 
									''',''' +convert(varchar,@VehPpnBmSellPaid)+ ''',''' +convert(varchar,@VehPpnBmSell)+ 
									''',''' +@PONo+ ''',''' +@RTPNo+ ''',''' +@BPUNo+ ''',''' +@HPPNo+ ''',NULL,NULL,'''
									+@VehSONo+ ''',''' +@VehRTNNo+ ''',''' +@VehDONo+ ''',''' +@VehBPKNo+ ''','''
									+@VehINVNo+ ''',''' +@VehREQNo+ ''',''' +@VehVTONo+ 
									''',''' +@VehVTINo+ ''',''' +@WHSD+ ''',''' +@VehINVNo+ ''',''6'',0,''' 
									+convert(varchar,@curDocDate,121)+ ''',''' +@vEHFakturPolisiNo+ ''','''
									+convert(varchar,@VehFakturPolisiDate,121)+ ''',NULL,NULL,1,0,1,'''
									+@curCreatedBy+ ''',''' +convert(varchar,@curCreatedDate,121)+ 
									''',''' +@curLastUpdateBy+ ''',''' +convert(varchar,@curLastUpdateDate,121)+
									''',0,NULL,NULL,0,''' +convert(varchar,@curDocDate,121)+ ''','''
									+@VehSuzukiDONo+ ''',''' +convert(varchar,@VehSuzukiDODate,121)+ ''','''
									+@VehSuzukiSJNo+ ''',''' +convert(varchar,@VehSuzukiSJDate,121)+ ''',NULL,NULL))
					    as SRC (NewCompany, NewChassisCode, NewChassisNo, NewEngineCode, NewEngineNo, 
								NewSalesModelCode, NewSalesModelYear, NewColourCode, NewServiceBookNo, 
								NewKeyNo, NewCOGSUnit, NewCOGSOthers, NewCOGSKaroseri, NewPpnBMBuyPaid, 
								NewPpnBmBuy, NewSalesNetAmt, NewPpnBmSellPaid, NewPpnBmSell, NewPONo, 
								NewPOReturnNo, NewBPUNo, NewHPPNo, NewKaroseriSPKNo, NewKaroseriTerimaNo, 
								NewSONo, NewSOReturnNo, NewDONo, NewBPKNo, NewInvoiceNo, NewReqOutNo, 
								NewTransferOutNo, NewTransferInNo, NewWarehouseCode, NewRemark, NewStatus, 
								NewIsAlreadyPDI, NewBPKDate, NewFakturPolisiNo, NewFakturPolisiDate, 
								NewPoliceRegistrationNo, NewPoliceRegistrationDate, NewIsProfitCenterSales, 
								NewIsProfitCenterService, NewIsActive, NewCreatedBy, NewCreatedDate, 
								NewLastUpdateBy, NewLastUpdateDate, NewIsLocked, NewLockedBy, NewLockedDate, 
								NewIsNonRegister, NewBPUDate, NewSuzukiDONo, NewSuzukiDODate, NewSuzukiSJNo, 
								NewSuzukiSJDate, NewTransferOutMultiBranchNo, NewTransferInMultiBranchNo)
						on VEH.CompanyCode = SRC.NewCompany
					   and VEH.ChassisCode = SRC.NewChassisCode
					   and VEH.ChassisNo   = SRC.NewChassisNo
					  when matched
						   then update set PONO				= isnull(@PONo,'')
										 , POReturnNo		= isnull(@RTPNo,'')
										 , BPUNo			= isnull(@BPUNo,'')
										 , HPPNo			= isnull(@HPPNo,'')
										 , SONo             = isnull(@VehSONo,'')
										 , SOReturnNo       = isnull(@VehRTNNo,'')
										 , DONo		        = isnull(@VehDONo,'')
										 , BPKNo            = isnull(@VehBPKNo,'')
										 , InvoiceNo        = isnull(@VehINVNo,'')
										 , TransferOutNo    = isnull(@VehVTONo,'')
										 , TransferInNo     = isnull(@VehVTINo,'')
										 , BPKDate		    = isnull(@VehBPKDate,'')
										 , BPUDate			= isnull(@VehBPUDate,'')
										 , SuzukiDONo       = isnull(@VehSuzukiDONo,'')
										 , SuzukiDODate     = isnull(@VehSuzukiDODate,'')
										 , SuzukiSJNo       = isnull(@VehSuzukiSJNo,'')
										 , SuzukiSJDate     = isnull(@VehSuzukiSJDate,'')
										 , FakturPolisiNo   = isnull(FakturPolisiNo,@VehFakturPolisiNo)
										 , FakturPolisiDate = isnull(FakturPolisiDate,@VehFakturPolisiDate)
						                 , QuantityInvoice = QuantityInvoice + isnull(SRC.NewQuantityInvoice,0)
										 , LastUpdateBy    = ''' +@curLastUpdateBy+ '''
										 , LastUpdateDate  = ''' +convert(varchar,@curLastUpdateDate,121)+ '''
					  when not matched by target 
						   then insert (CompanyCode, ChassisCode, ChassisNo, EngineCode, EngineNo, SalesModelCode, 
										SalesModelYear, ColourCode, ServiceBookNo, KeyNo, COGSUnit, COGSOthers, 
										COGSKaroseri, PpnBMBuyPaid, PpnBmBuy, SalesNetAmt, PpnBmSellPaid, PpnBmSell,
										PONo, POReturnNo, BPUNo, HPPNo, KaroseriSPKNo, KaroseriTerimaNo, SONo, 
										SOReturnNo, DONo, BPKNo, InvoiceNo, ReqOutNo, TransferOutNo, TransferInNo,
										WarehouseCode, Remark, Status, IsAlreadyPDI, BPKDate, FakturPolisiNo, 
										FakturPolisiDate, PoliceRegistrationNo, PoliceRegistrationDate, 
										IsProfitCenterSales, IsProfitCenterService, IsActive, CreatedBy, CreatedDate, 
										LastUpdateBy, LastUpdateDate, IsLocked, LockedBy, LockedDate, IsNonRegister,
										BPUDate, SuzukiDONo, SuzukiDODate, SuzukiSJNo, SuzukiSJDate, 
										TransferOutMultiBranchNo, TransferInMultiBranchNo)
								values (NewCompany, NewChassisCode, NewChassisNo, NewEngineCode, NewEngineNo, 
										NewSalesModelCode, NewSalesModelYear, NewColourCode, NewServiceBookNo, 
										NewKeyNo, NewCOGSUnit, NewCOGSOthers, NewCOGSKaroseri, NewPpnBMBuyPaid, 
										NewPpnBmBuy, NewSalesNetAmt, NewPpnBmSellPaid, NewPpnBmSell, NewPONo, 
										NewPOReturnNo, NewBPUNo, NewHPPNo, NewKaroseriSPKNo, NewKaroseriTerimaNo, 
										NewSONo, NewSOReturnNo, NewDONo, NewBPKNo, NewInvoiceNo, NewReqOutNo, 
										NewTransferOutNo, NewTransferInNo, NewWarehouseCode, NewRemark, NewStatus, 
										NewIsAlreadyPDI, NewBPKDate, NewFakturPolisiNo, NewFakturPolisiDate, 
										NewPoliceRegistrationNo, NewPoliceRegistrationDate, NewIsProfitCenterSales, 
										NewIsProfitCenterService, NewIsActive, NewCreatedBy, NewCreatedDate, 
										NewLastUpdateBy, NewLastUpdateDate, NewIsLocked, NewLockedBy, NewLockedDate, 
										NewIsNonRegister, NewBPUDate, NewSuzukiDONo, NewSuzukiDODate, NewSuzukiSJNo, 
										NewSuzukiSJDate, NewTransferOutMultiBranchNo, NewTransferInMultiBranchNo);'
					execute sp_executesql @sqlString

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

			 -- SD: insert omMstVehicleTemp

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
                              -- or  convert(varchar,DocDate,111)<=convert(varchar,@PostingDate,111)

		delete omSDMovement   where	 ProcessStatus = 1
                              -- or  convert(varchar,DocDate,111)<=convert(varchar,@PostingDate,111)

		close curomSDMovement
		deallocate curomSDMovement

END
