
ALTER procedure [dbo].[uspfn_SvTrnJobOrderCancel]
	@CompanyCode varchar(15),
	@BranchCode varchar(15),
	@ProductType varchar(15),
	@ServiceNo bigint,
	@UserID varchar(15)
as      

declare @errmsg varchar(max)

declare @ServiceStatus varchar(10)
    set @ServiceStatus = isnull((
						 select ServiceStatus
						   from svTrnService
						  where 1 = 1
						    and CompanyCode = @CompanyCode
						    and BranchCode  = @BranchCode
						    and ProductType = @ProductType
						    and ServiceNo   = @ServiceNo
						 ),0)

if (@ServiceStatus >= '5') 
begin
	set @errmsg = N'Dear ' + isnull((select FullName from sysuser where userid = @UserID), @UserID) + ',' + char(13)
				+ N'SPK ini sudah tidak dapat di cancel lagi karena status sudah berubah.'
				+ char(13) + 'Status SPK Terakhir = ' + @ServiceStatus + ', tolong di check lagi'
				+ char(13) + 'Terima kasih'
	raiserror (@errmsg,16,1);
	return
end

-- cek Supply - Return Part
if (select count(*) from svtrnsrvitem where CompanyCode = @CompanyCode and BranchCode = @BranchCode and ProductType = @ProductType
		and ServiceNo = @ServiceNo and SupplyQty-ReturnQty > 0 ) > 0
begin
	set @errmsg = N'Dear ' + isnull((select FullName from sysuser where userid = @UserID), @UserID) + ',' + char(13)
				+ N'SPK ini tidak dapat di cancel. Disebabkan part harus di return semua !'
				+ char(13) + 'Lakukan return Sparepart dahulu, Terima kasih.'
	raiserror (@errmsg,16,1);
	return
end


begin try
	update svTrnService
	   set ServiceStatus  = '6'
          ,LastUpdateBy  = @UserID
          ,LastUpdateDate= getdate()
	 where 1 = 1
	   and CompanyCode = @CompanyCode
	   and BranchCode = @BranchCode
	   and ProductType = @ProductType
	   and ServiceNo = @ServiceNo
	
	declare @CompanyMD as varchar(15)
	declare @BranchMD as varchar(15)

	set @CompanyMD = (select CompanyMD from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)
	set @BranchMD = (select BranchMD from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)

	if (@CompanyCode != @CompanyMD and @BranchCode != @BranchMD)
	begin	
	update svSDMovement
	   set Status  = '6'
          ,LastUpdateBy  = @UserID
          ,LastUpdateDate= getdate()
	 where 1 = 1
	   and CompanyCode = @CompanyCode
	   and BranchCode = @BranchCode	  
	   and DocNo = (select JobOrderNo from svTrnService where CompanyCode = @CompanyCode and BranchCode = @BranchCode and ServiceNo = @ServiceNo)
	end
end try
begin catch
	set @errmsg = N'tidak dapat konversi ke SPK pada ServiceNo = '
				+ convert(varchar,@ServiceNo)
				+ char(13) + error_message()
	raiserror (@errmsg,16,1);
end catch
