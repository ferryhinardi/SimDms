--select * from HrEmployeeSales


declare @CompanyCode varchar(25);
set @CompanyCode = (select top 1 CompanyCode from gnMstOrganizationHdr)


insert into HrEmployeeSales(CompanyCode, EmployeeID, SalesID, CreatedBy, CreatedDate, IsSyncronized, IsTransfered, SyncronizedDate, IsDeleted)
select @CompanyCode
     , a.EmployeeID
	 , a.SalesID
	 , 'system'
	 , getdate()
	 , 0
     , 0
     , null
     , 0
  from SyncEmployeeData a