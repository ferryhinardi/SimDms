if object_id('uspfn_SvTrnServiceDeleteDtl') is not null
	drop procedure uspfn_SvTrnServiceDeleteDtl
GO
CREATE procedure [dbo].[uspfn_SvTrnServiceDeleteDtl]
	@CompanyCode varchar(15),
	@BranchCode varchar(15),
	@ProductType varchar(15),
	@ServiceNo bigint,
	@TaskPartType varchar(15),
	@TaskPartNo varchar(50),
	@PartSeq numeric(4,0)
as      
--begin tran
--declare	@CompanyCode as varchar(15)
--declare	@BranchCode as varchar(15)
--declare	@ProductType as varchar(15)
--declare	@ServiceNo as bigint
--declare	@TaskPartType as varchar(15)
--declare	@TaskPartNo as varchar(20)
--declare	@PartSeq as numeric(4,0)

--set	@CompanyCode = '6159401000'
--set	@BranchCode = '6159401001'
--set	@ProductType = '4W'
--set	@ServiceNo = '52727'
--set	@TaskPartType = 'L'
--set	@TaskPartNo = 'FSC 40000'
--set	@PartSeq = '0'

declare @errmsg varchar(max)
declare @reccount int;

declare @CompanyMD as varchar(15)
declare @BranchMD as varchar(15)

declare @Query varchar(max)
declare @DbMD varchar(max)

set @DbMD = dbo.GetDbMD(@CompanyCode, @BranchCode) 

if @TaskPartType = 'L'
begin
	if ((isnull((select count(*) from svTrnSrvTask
	 where 1 = 1
	   and CompanyCode = @CompanyCode
	   and BranchCode  = @BranchCode
	   and ProductType = @ProductType
	   and ServiceNo   = @ServiceNo),0)) > 1)
	begin
		delete svTrnSrvTask
		 where 1 = 1
		   and CompanyCode = @CompanyCode
		   and BranchCode  = @BranchCode
		   and ProductType = @ProductType
		   and ServiceNo   = @ServiceNo
		   and OperationNo = @TaskPartNo
	end
	else 
	begin
		--  add validation here to delete task
		set @reccount = isnull((
				select count(b.PartNo) from spTrnSORDHdr a, spTrnSORDDtl b
				 where 1 = 1
				   and a.CompanyCode = b.CompanyCode
				   and a.BranchCode  = b.BranchCode
				   and a.DocNo       = b.DocNo
				   and a.SalesType   = '2'
				   and a.Status     <> '3' 
				   and a.UsageDocNo  = isnull((
										select JobOrderNo from svTrnService
 										 where 1 = 1
										   and CompanyCode = @CompanyCode
										   and BranchCode  = @BranchCode
										   and ProductType = @ProductType
										   and ServiceNo   = @ServiceNo											
										),'')
					and a.CompanyCode = @CompanyCode
					and a.BranchCode  = @BranchCode
				),0)
		if (@reccount > 0) 
		begin
			set @errmsg = N'Task sudah tidak dapat dihapus karena terdapat Supply Slip sudah diproses'
						+ char(13) + 'Tolong di check lagi'
						+ char(13) + 'Terima kasih'
			raiserror (@errmsg,16,1);
			return
		end

		-- delete task
		delete svTrnSrvTask
		 where 1 = 1
		   and CompanyCode = @CompanyCode
		   and BranchCode  = @BranchCode
		   and ProductType = @ProductType
		   and ServiceNo   = @ServiceNo
		   and OperationNo = @TaskPartNo
		-- delete all item in spk
		delete svTrnSrvItem
		 where 1 = 1
		   and CompanyCode = @CompanyCode
		   and BranchCode  = @BranchCode
		   and ProductType = @ProductType
		   and ServiceNo   = @ServiceNo
	
		set @CompanyMD = (select CompanyMD from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)
		set @BranchMD = (select BranchMD from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)

		if (@CompanyCode != @CompanyMD and @BranchCode != @BranchMD)
		begin	   
			set @Query = 'delete '+ @DbMD +'..svSDMovement
    		 where 1 = 1
			   and CompanyCode = '''+ @CompanyCode +'''
			   and BranchCode  = '''+ @BranchCode +'''
			   and DocNo	   = (select JobOrderNo from svTrnService where CompanyCode = '''+ @CompanyCode +''' and BranchCode  = '''+ @BranchCode +''' and ServiceNo = '+ convert(varchar,@ServiceNo)  +')'
		exec(@Query)
		end
	end
end
else
begin
    set @reccount = isnull((
			select count(b.PartNo) from spTrnSORDHdr a, spTrnSORDDtl b
			 where 1 = 1
			   and a.CompanyCode = b.CompanyCode
			   and a.BranchCode  = b.BranchCode
			   and a.DocNo       = b.DocNo
			   and a.SalesType   = '2'
			   and a.Status     <> '3' 
			   and a.UsageDocNo  = isnull((
									select JobOrderNo from svTrnService
 									 where 1 = 1
									   and CompanyCode = @CompanyCode
									   and BranchCode  = @BranchCode
									   and ProductType = @ProductType
									   and ServiceNo   = @ServiceNo
									),'')
			  and b.PartNo      = @TaskPartNo
			   and a.CompanyCode = @CompanyCode
			   and a.BranchCode  = @BranchCode
			),0)

	declare @npartss int, @npartot int;
	set @npartot = isnull((
			select count(*) from svTrnSrvItem
			 where 1 = 1
			   and CompanyCode = @CompanyCode
			   and BranchCode  = @BranchCode
			   and ProductType = @ProductType
			   and ServiceNo   = @ServiceNo
			   and PartNo      = @TaskPartNo
			   and PartSeq     = @PartSeq
			   and isnull(SupplySlipNo, '') = ''
			), 0)
	set @npartss = isnull((
			select count(*) from svTrnSrvItem
			 where 1 = 1
			   and CompanyCode = @CompanyCode
			   and BranchCode  = @BranchCode
			   and ProductType = @ProductType
			   and ServiceNo   = @ServiceNo
			   and PartNo      = @TaskPartNo
			   and isnull(SupplySlipNo, '') <> ''
			), 0)

	if ((@reccount > 0 and @npartot <> 1 and @npartss > 0)
	 or (@reccount > 0 and @npartot  = 1 and @npartss = 0))
	begin
		set @errmsg = N'Part sudah tidak dapat dihapus karena Supply Slip sudah diproses'
					+ char(13) + 'Tolong di check lagi'
					+ char(13) + 'Terima kasih'
					+ char(13) + ''
					+ char(13) + 'Jml Part' + convert(varchar, @npartot)
					+ char(13) + 'Jml Supply Slip' + convert(varchar, @npartss)
		raiserror (@errmsg,16,1);
		return
	end

	delete svTrnSrvItem
	 where 1 = 1
	   and CompanyCode = @CompanyCode
	   and BranchCode  = @BranchCode
	   and ProductType = @ProductType
	   and ServiceNo   = @ServiceNo
	   and PartNo      = @TaskPartNo
	   and PartSeq     = @PartSeq
	
	set @CompanyMD = (select CompanyMD from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)
	set @BranchMD = (select BranchMD from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)

	if (@CompanyCode != @CompanyMD and @BranchCode != @BranchMD)
	begin	 
	set @Query =' 
	delete '+ @DbMD +'..svSDMovement
	 where 1 = 1
	   and CompanyCode = '''+ @CompanyCode +'''
	   and BranchCode  = '''+ @BranchCode +'''
	   and DocNo	   = (select JobOrderNo from svTrnService where CompanyCode = '''+ @CompanyCode +''' and BranchCode  = '''+ @BranchCode +''' and ServiceNo = '+ convert(varchar,@ServiceNo)  +')
	   and PartNo      = '''+ convert(varchar,@TaskPartNo) +'''
	   and PartSeq     = '+ convert(varchar,@PartSeq)

	   exec(@Query)
	end	   
end

exec uspfn_SvTrnServiceReCalculate @CompanyCode,@BranchCode,@ProductType,@ServiceNo,0
--rollback tran
GO
