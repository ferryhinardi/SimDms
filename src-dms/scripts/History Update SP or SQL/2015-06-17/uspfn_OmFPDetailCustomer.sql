IF OBJECT_ID('[dbo].[uspfn_OmFPDetailCustomer]') IS NOT NULL
DROP PROCEDURE [dbo].[uspfn_OmFPDetailCustomer]

GO
/****** Object:  StoredProcedure [dbo].[uspfn_OmFPDetailCustomer]    Script Date: 6/17/2015 9:33:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [dbo].[uspfn_OmFPDetailCustomer]
	@ChassisCode as varchar(15)
	,@ChassisNo as varchar(15)
as

declare @recSoVin as Int

set @recSoVin = (SELECT Count(*) FROM omTrSalesSOVin WHERE ChassisCode=@ChassisCode AND ChassisNo=@ChassisNo )

if (@recSoVin) > 0 --Sovin
begin
	SELECT a.CompanyCode, a.BranchCode, d.BPKNo, a.SONo, a.EndUserName, b.RefferenceNo SKPKNo, a.EndUserAddress1, a.EndUserAddress2, a.EndUserAddress3, c.CustomerName, c.Address1, c.Address2, c.Address3,
		c.CityCode,(SELECT LookUpValueName FROM gnMstLookUpDtl where CodeID = 'CITY' AND ParaValue = c.CityCode) as CityName, 
		c.PhoneNo, c.HPNo, c.birthDate ,b.Salesman, (SELECT EmployeeName FROM gnMstEmployee where EmployeeID = b.Salesman AND CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode) SalesmanName, b.SalesType
	FROM omTrSalesSOVin a
		left join omTrSalesSO b on a.companyCode = b.companyCode 
			and a.BranchCode= b.BranchCode
			and a.SONo = b.SONo
			and b.Status = '2'
		left join gnMstCustomer c on b.CompanyCode = c.CompanyCode
			and b.CustomerCode = c.CustomerCode
		left join omTrSalesBPK d on a.CompanyCode = d.CompanyCode
			and a.BranchCode = d.BranchCode
			and a.SONo = d.SONo
	WHERE a.ChassisCode=@ChassisCode AND a.ChassisNo=@ChassisNo
end
else --dodetail
	SELECT a.CompanyCode, a.BranchCode, d.BPKNo, e.SONo, 
		c.CustomerName EndUserName, b.RefferenceNo SKPKNo, c.Address1 EndUserAddress1, c.Address2 EndUserAddress2, c.Address3 EndUserAddress3, 
		c.CustomerName, c.Address1, c.Address2, c.Address3,
		c.CityCode,(SELECT LookUpValueName FROM gnMstLookUpDtl where CodeID = 'CITY' AND ParaValue = c.CityCode) as CityName, 
		c.PhoneNo, c.HPNo, c.birthDate ,b.Salesman, (SELECT EmployeeName FROM gnMstEmployee where EmployeeID = b.Salesman AND CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode) SalesmanName, b.SalesType
	FROM omTrSalesDODetail a
	left join omTrSalesDO e on a.CompanyCode = e.CompanyCode
		and a.BranchCode = e.BranchCode
		and a.Dono = e.Dono 
	left join omTrSalesSO b on a.companyCode = b.companyCode 
		and a.BranchCode= b.BranchCode
		and e.SONo = b.SONo
	left join gnMstCustomer c on b.CompanyCode = c.CompanyCode
		and b.CustomerCode = c.CustomerCode
	left join omTrSalesBPK d on a.CompanyCode = d.CompanyCode
		and a.BranchCode = d.BranchCode
		and e.SONo = d.SONo
	WHERE a.ChassisCode=@ChassisCode AND a.ChassisNo=@ChassisNo	
