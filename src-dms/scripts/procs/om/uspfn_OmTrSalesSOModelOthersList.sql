
go
if object_id('uspfn_OmTrSalesSOModelOthersList') is not  null
	drop procedure uspfn_OmTrSalesSOModelOthersList

go
create procedure uspfn_OmTrSalesSOModelOthersList
	@CompanyCode varchar(13),
	@BranchCode varchar(13),
	@SONumber varchar(25),
	@SalesModelCode varchar(25),
	@SalesModelYear decimal
as
begin
	SELECT a.OtherCode
		 , (
				SELECT b.RefferenceDesc1
				  FROM omMstRefference b
			     WHERE a.CompanyCode = b.CompanyCode
				   and a.OtherCode = b.RefferenceCode 
	               and b.RefferenceType='OTHS')  AS AccsName
         , a.DPP
		 , a.PPn
		 , a.Remark
         , a.BeforeDiscTotal
		 , a.AfterDiscTotal
		 , a.AfterDiscDPP
		 , a.AfterDiscPPn
		 , a.CompanyCode
		 , a.BranchCode
		 , a.SONo
		 , a.SalesModelCode
		 , a.SalesModelYear
      FROM omTrSalesSOModelOthers a
     WHERE a.CompanyCode = @CompanyCode
       AND a.BranchCode = @BranchCode
       AND a.SONo = @SONumber
       AND a.SalesModelCode = @SalesModelCode
       AND a.SalesModelYear = @SalesModelYear
end