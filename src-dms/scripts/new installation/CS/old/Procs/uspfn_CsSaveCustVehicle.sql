alter proc uspfn_CsSaveCustVehicle
	@CompanyCode varchar(20),
	@CustomerCode varchar(50),
	@Chassis varchar(50),
	@StnkDate datetime,
	@BpkbDate datetime,
	@UserID varchar(20)
as

if exists ( select 1 from CsCustomerVehicle
			 where CompanyCode = @CompanyCode 
			   and Chassis = @Chassis)
begin
	update CsCustomerVehicle
	   set CustomerCode = @CustomerCode
	     , StnkDate = @StnkDate
	     , BpkbDate = @BpkbDate
	     , UpdatedBy = @UserID
	     , UpdatedDate = getdate()
	 where CompanyCode = @CompanyCode
	   and Chassis = @Chassis    
end
else
begin
	insert into CsCustomerVehicle (CompanyCode, CustomerCode, Chassis, StnkDate, BpkbDate, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
	values (@CompanyCode, @CustomerCode, @Chassis, @StnkDate, @BpkbDate, @UserID, getdate(), @UserID, getdate())
end

			   
