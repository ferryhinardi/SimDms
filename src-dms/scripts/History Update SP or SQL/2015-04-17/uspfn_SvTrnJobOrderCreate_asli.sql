ALTER procedure [dbo].[uspfn_SvTrnJobOrderCreate]
	@CompanyCode varchar(15),
	@BranchCode varchar(15),
	@ProductType varchar(15),
	@ServiceNo bigint,
	@UserID varchar(15)
as      

declare @errmsg varchar(max)

begin try
	begin transaction
		declare @docseq int 
        set @docseq = isnull((
			select DocumentSequence from gnMstDocument 
			 where 1 = 1
			   and CompanyCode  = @CompanyCode
			   and BranchCode   = @BranchCode
			   and DocumentType = 'SPK'),0) + 1
        declare @JobOrderNo varchar(15)
		set @JobOrderNo = 'SPK/' + (select right(convert(char(4),getdate(),112),2)) + '/' 
                                 + right((replicate('0',6) + (select convert(varchar, @docseq))),6)
		update svTrnService
		   set ServiceType    = '2'
              ,JobOrderNo     = @JobOrderNo
              ,JobOrderDate   = getdate()
              ,LastUpdateBy   = @UserID
              ,LastUpdateDate = getdate()
		 where 1 = 1
		   and CompanyCode = @CompanyCode
		   and BranchCode  = @BranchCode
		   and ProductType = @ProductType
		   and ServiceNo   = @ServiceNo
		update gnMstDocument 
		   set DocumentSequence = @docseq
              ,LastUpdateBy     = @UserID
              ,LastUpdateDate   = getdate()
		 where 1 = 1
		   and CompanyCode  = @CompanyCode
		   and BranchCode   = @BranchCode
		   and DocumentType = 'SPK'
	commit transaction

end try
begin catch
	rollback transaction
	set @errmsg = N'tidak dapat konversi ke SPK pada ServiceNo = '
				+ convert(varchar,@ServiceNo)
				+ char(13) + error_message()
	raiserror (@errmsg,16,1);
end catch
