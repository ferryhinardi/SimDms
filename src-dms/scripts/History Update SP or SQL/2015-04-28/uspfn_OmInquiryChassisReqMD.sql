
/****** Object:  StoredProcedure [dbo].[uspfn_OmInquiryChassisReqMD]    Script Date: 04/28/2015 11:45:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- uspfn_OmInquiryChassisReq '6007402','600740200'
ALTER procedure [dbo].[uspfn_OmInquiryChassisReqMD]
	@CompanyCode as varchar(15)
	,@BranchCode as varchar(15)
	,@Penjual as varchar(15)
	,@CBU as bit
as

declare @isDirect as bit,
		@QRYTmp		AS varchar(max),
		@DBMD		AS varchar(25),
		@CompanyMD  AS varchar(25)

set @isDirect=0
if exists (
	select 1
	from gnMstCoProfile
	where CompanyCode=@CompanyCode and BranchCode=@Penjual
)
set @isDirect=1
set @CompanyMD = (SELECT TOP 1 CompanyMD FROM gnMstCompanyMapping WHERE CompanyCode=@CompanyCode AND BranchCode=@BranchCode) 
set @DBMD = (SELECT TOP 1 DbMD FROM gnMstCompanyMapping WHERE CompanyCode=@CompanyCode AND BranchCode=@BranchCode) 

set @QRYTmp =
'select * into #t1
from (
	select distinct isnull(b.BranchCode, e.BranchCode) BranchCode, isnull(c.CustomerCode, e.CustomerCode) CustomerCode
			,z.ChassisCode,z.BPKNo,z.SONo,e.DONo,convert(varchar,z.chassisNo) chassisNo, z.salesModelCode
			, z.salesModelYear, isnull(z.SuzukiDONo,'''') RefferenceDONo,
			isnull(z.SuzukiDODate,''19000101'') RefferenceDODate, isnull(z.SuzukiSJNo,'''') RefferenceSJNo, 
			isnull(z.SuzukiSJDate,''19000101'') RefferenceSJDate, 
			a.EndUserName, a.EndUserAddress1, a.EndUserAddress2, a.EndUserAddress3,
			c.CustomerName, c.Address1, c.Address2, c.Address3,
			c.CityCode,(SELECT LookUpValueName FROM gnMstLookUpDtl where CodeID = ''CITY'' AND ParaValue = c.CityCode) as CityName, 
			c.PhoneNo, c.HPNo, c.birthDate ,b.Salesman, (SELECT Distinct EmployeeName FROM gnMstEmployee where EmployeeID = b.Salesman) SalesmanName, b.SalesType
	from ' + @DBMD + '.dbo.omMstVehicle z 
		left join omTrSalesSOVin a 
			on a.CompanyCode = ''' + @CompanyCode + ''' 
			and z.SONo=a.SONo
				AND a.ChassisCode = z.ChassisCode
				AND a.ChassisNo = z.ChassisNo
		left join omTrSalesSO b
			on a.companyCode = b.companyCode 
				and a.BranchCode= b.BranchCode
				and a.SONo = b.SONo
				and b.Status = ''2'' 
		left join gnMstCustomer c 
			on b.CompanyCode = c.CompanyCode
				and b.CustomerCode = c.CustomerCode 
		left join OmTrSalesDODetail d 
			on d.CompanyCode = z.CompanyCode and z.DONo=d.DONo
				and d.ChassisCode = z.ChassisCode		
				and d.ChassisNo = z.ChassisNo	
		left join OmTrSalesDO e 
			on e.CompanyCode = d.CompanyCode
				and e.DONo = d.DONo
				and e.BranchCode=d.BranchCode
				and e.Status = ''2''
		inner join omMstModel f
			on f.CompanyCode = ''' + @CompanyCode + '''
				and f.SalesModelCode = z.SalesModelCode
	where 
		z.CompanyCode = ''' + @CompanyMD + '''
		and z.ReqOutNo = ''''
		and z.status in (''3'',''4'',''5'',''6'')
		and not exists (select 1 from omTrSalesReqdetail where ChassisCode=z.ChassisCode and ChassisNo=z.ChassisNo)
		and ((case when z.ChassisNo is not null then z.SONo end) is not null 
			or (case when z.ChassisNo is not null then z.DONo end) is not null)
		and f.IsCBU = ' + CONVERT(VARCHAR, @CBU, 1) + '
) #t1

select * from #t1 

drop table #t1'

Exec (@QRYTmp);


--where ((case when ' + CONVERT(VARCHAR, @isDirect, 1) + ' = ''1'' then BranchCode end)= ''' + @Penjual + '''
--		or (case when ' + CONVERT(VARCHAR, @isDirect, 1) + ' <> ''1'' then BranchCode end)= ''' + @BranchCode + ''' )


