alter procedure uspfn_SelectSalesModel
	@CompanyCode varchar(17),
	@BranchCode varchar(17),
	@InquiryNumber varchar(17)
as

begin
	if @InquiryNumber is null or @InquiryNumber = ''
	begin
		select distinct a.SalesModelCode, a.SalesModelDesc
          from omMstModelYear a 
	     where a.CompanyCode = @CompanyCode AND a.Status IN ('1', '2')
		 order by a.SalesModelCode asc
	end
	else
	begin
		select a.SalesModelCode, a.SalesModelDesc 
          from omMstModel a
         inner join pmKDP b on a.CompanyCode = a.CompanyCode
	       and b.BranchCode = @BranchCode	                            
	       and a.GroupCode = b.TipeKendaraan
	       and a.Transmissiontype = b.Transmisi
	       and a.TypeCode = b.Variant
         where a.CompanyCode = @CompanyCode
           and b.InquiryNumber = @InquiryNumber
	end
end	





go
exec uspfn_SelectSalesModel '6115204', '611520402', '31146'
--select * from pmKDP where InquiryNumber='31146'
--select distinct GroupCode from omMstModel


