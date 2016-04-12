-- POSTING TRANSACTION MULTI COMPANY - SALES RETURN
-- ------------------------------------------------
-- Created  by : HTO, 2014
-- Revision by : HTO, January 2015
-- ------------------------------------------------
-- This procedure used for 
--		4-Wheeler : BIT-SBT, SBSM-SSBT, BAT-SAT, SIT-SST, UIB-SIT
--		2-Wheeler : IJMG,SVMG,ISG,ISMG-SMG
-- -----------------------------------------------------------------------------
-- execute [usprpt_PostingMultiCompanySalesReturn] '6006400001','2014/11/30','0'
-- -----------------------------------------------------------------------------

ALTER procedure [dbo].[usprpt_PostingMultiCompanySalesReturn]
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
             where sd.CompanyCode      =doc.CompanyCode
			   and sd.BranchCode       =doc.BranchCode
			   and doc.DocumentType    ='RTS' 
			   and doc.ProfitCenterCode='100'
			   and left(sd.DocNo,len(doc.DocumentPrefix))=doc.DocumentPrefix
			   and convert(varchar,DocDate,111)<=convert(varchar,@PostingDate,111)
			   and ProcessStatus=0
             order by sd.CompanyCode, sd.BranchCode, sd.DocNo, sd.SalesModelCode, 
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
				select @DBNameMD=DBMD, @DBName=DBName from gnMstCompanyMapping where CompanyCode=@CurCompanyCode and BranchCode =@curBranchCode

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
					 	execute [usprpt_PostingMultiCompany4DocNo] @curCompanyMD, @curBranchMD, @DBNameMD, 'RTS', @RTSNo output
						set @sqlString = 'insert into ' +@DBNameMD+ '..omTrSalesReturn 
												(CompanyCode, BranchCode, ReturnNo, ReturnDate, ReferenceNo, ReferenceDate, 
												 InvoiceNo, InvoiceDate, CustomerCode, FakturPajakNo, FakturPajakDate, 
												 WareHouseCode, Remark, Status, CreatedBy, CreatedDate, LastUpdateBy, 
												 LastUpdateDate, isLocked, LockedBy, LockedDate)
										  values(''' +@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' 
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
											   +@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' 
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
											   +@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' +@RTSNo+ ''',''' +@BPKNo+ ''','''
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

			 -- SD: Collect price information from table omTrPurchaseHPP
				set @sqlString = N'select @BPKNo=BPKNo from ' +@DBNameMD+ '..omTrSalesInvoiceVin where CompanyCode='''
										  +@curCompanyMD+ ''' and BranchCode=''' +@curBranchMD+ ''' and InvoiceNo=''' 
										  +@INVNo+ ''' and ChassisCode=''' +@curChassisCode+ ''' and ChassisNo='''
										  +convert(varchar,@curChassisNo) + ''''
						execute sp_executesql @sqlString, 
								N'@BPKNo	varchar(15) output', @BPKNo	output

			 -- MD: Insert data to table omTrSalesReturnVIN
				set @sqlString = 'insert into ' +@DBNameMD+ '..omTrSalesReturnVIN
										(CompanyCode, BranchCode, ReturnNo, BPKNo, SalesModelCode, SalesModelYear, 
										 ReturnSeq, ColourCode, ChassisCode, ChassisNo, EngineCode, EngineNo, 
										 Remark, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
								  values(''' +@curCompanyMD+ ''',''' +@curBranchMD+ ''',''' 
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
