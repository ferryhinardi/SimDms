BEGIN
	declare @Company varchar(15)
	set @Company = (select top 1 CompanyCode from gnMstOrganizationHdr)
	update omMstVehicle
	   set ServiceBookNo = (select ServiceBookNo from omHstServiceBook a
	                         where omMstVehicle.CompanyCode=@Company
							   and omMstVehicle.ChassisCode=left(a.VIN,11)
							   and omMstVehicle.ChassisNo=right(a.VIN,6))
         , LastUpdateBy   = 'SRVBOOKNO'
		 , LastUpdateDate = getdate()
     where exists          (select 1 from omHstServiceBook a
	                         where omMstVehicle.CompanyCode=@Company
							   and omMstVehicle.ChassisCode=left(a.VIN,11)
							   and omMstVehicle.ChassisNo=right(a.VIN,6))
END
