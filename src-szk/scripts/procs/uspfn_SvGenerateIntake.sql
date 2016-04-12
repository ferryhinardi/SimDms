
go
if object_id('uspfn_SvGenerateUnitIntake') is not null
	drop procedure uspfn_SvGenerateUnitIntake;

go
create procedure uspfn_SvGenerateUnitIntake
	@AreaCode varchar(25),
	@DealerCode varchar(15),
	@BranchCode varchar(25),
	@StartDate datetime,
	@EndDate datetime
as
begin
	select Chassis = a.ChassisCode + convert(varchar, a.ChassisNo)
	     , ClosedSPKDate = isnull(a1.JobOrderClosed, a1.InvoiceDate)
		 , DealerCode_Purcahase = a.CompanyCode
		 , DealerName_Purchase = b.DealerName
		 , DealerCode_Service = a1.CompanyCode
		 , DealerName_Service = a1.CompanyName
	  from omTrSalesDODetail a
	 inner join DealerInfo b
	    on b.DealerCode = a.CompanyCode
	 outer apply (
				select top 1
				       CompanyCode = x.CompanyCode
					 , CompanyName = y.DealerName	
					 , x.JobOrderClosed 		
					 , z.InvoiceDate
				  from svTrnService x
				 inner join DealerInfo y
				    on x.CompanyCode = y.DealerCode
				 inner join svTrnInvoice z
				    on x.InvoiceNo = x.InvoiceNo
				 where x.ChassisCode = a.ChassisCode
				   and x.ChassisNo = a.ChassisNo
				 order by x.JobOrderClosed desc
		   ) a1
	 where a.CompanyCode = '6158401'
end


-- svTrnService, svMstCustomerVehicle, gnMstCustomer


go
exec uspfn_SvGenerateUnitIntake
