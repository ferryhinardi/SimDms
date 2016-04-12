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
