ALTER procedure [dbo].[uspfn_OmFPDetailCustomer]
--DECLARE
	@ChassisCode as varchar(15),
	@ChassisNo as varchar(15)
as

SELECT a.CompanyCode, a.BranchCode, d.BPKNo, a.SONo, a.EndUserName, b.RefferenceNo SKPKNo, a.EndUserAddress1, a.EndUserAddress2, a.EndUserAddress3, c.CustomerName, c.Address1, c.Address2, c.Address3,
	c.CityCode,(SELECT LookUpValueName FROM gnMstLookUpDtl where CodeID = 'CITY' AND ParaValue = c.CityCode) as CityName, 
	c.PhoneNo, c.HPNo, c.birthDate ,b.Salesman, (SELECT EmployeeName FROM gnMstEmployee where EmployeeID = b.Salesman and BranchCode = b.BranchCode) SalesmanName,
	b.SalesType
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
