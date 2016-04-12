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

-- SO Tgl Hari berjalan
set @QRYTmp =
'select * into #t1
from (
	select distinct isnull(b.BranchCode, e.BranchCode) BranchCode, isnull(c.CustomerCode, e.CustomerCode) CustomerCode
			,z.ChassisCode,z.BPKNo,a.SONo, b.SODate, e.DONo,convert(varchar,z.chassisNo) chassisNo, z.salesModelCode
			, z.salesModelYear, isnull(z.SuzukiDONo,'''') RefferenceDONo,
			isnull(z.SuzukiDODate,''19000101'') RefferenceDODate, isnull(z.SuzukiSJNo,'''') RefferenceSJNo, 
			isnull(z.SuzukiSJDate,''19000101'') RefferenceSJDate, 
			a.EndUserName, a.EndUserAddress1, a.EndUserAddress2, a.EndUserAddress3,
			c.CustomerName, c.Address1, c.Address2, c.Address3,
			c.CityCode,(SELECT LookUpValueName FROM gnMstLookUpDtl where CodeID = ''CITY'' AND ParaValue = c.CityCode) as CityName, 
			c.PhoneNo, c.HPNo, c.birthDate ,b.Salesman, (SELECT top 1 EmployeeName FROM gnMstEmployee where EmployeeID = b.Salesman AND CompanyCode=a.CompanyCode AND BranchCode=a.BranchCode) SalesmanName, b.SalesType
	from ' + @DBMD + '.dbo.omMstVehicle z 
		left join omTrSalesSOVin a 
			on a.CompanyCode = ''' + @CompanyCode + ''' 
				and z.SONo=a.SONo
				AND a.ChassisCode = z.ChassisCode
				AND a.ChassisNo = z.ChassisNo
		left join omTrSalesSO b
			on b.companyCode  = a.CompanyCode
				and b.BranchCode = a.BranchCode
				and b.SONo = a.SONo
				and b.Status = ''2'' 
		left join gnMstCustomer c 
			on c.CompanyCode = b.CompanyCode
				and c.CustomerCode = b.CustomerCode 
		left join OmTrSalesDODetail d 
			on d.CompanyCode = ''' + @CompanyCode + ''' 
				and z.DONo=d.DONo
				and d.ChassisCode = z.ChassisCode		
				and d.ChassisNo = z.ChassisNo	
		left join OmTrSalesDO e 
			on e.CompanyCode = d.CompanyCode
				and e.DONo = z.DONo
				and e.BranchCode=b.BranchCode
				and e.Status = ''2''
		inner join ' + @DBMD + '..omMstModel f
			on f.CompanyCode = ''' + @CompanyMD + '''
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

--So Tgl lalu
select * into #t2
from (
	select distinct isnull(b.BranchCode, e.BranchCode) BranchCode, isnull(c.CustomerCode, e.CustomerCode) CustomerCode
			,z.ChassisCode,z.BPKNo,z.SONo, b.SODate, e.DONo,convert(varchar,z.chassisNo) chassisNo, z.salesModelCode
			, z.salesModelYear, isnull(z.SuzukiDONo,'''') RefferenceDONo,
			isnull(z.SuzukiDODate,''19000101'') RefferenceDODate, isnull(z.SuzukiSJNo,'''') RefferenceSJNo, 
			isnull(z.SuzukiSJDate,''19000101'') RefferenceSJDate, 
			a.EndUserName, a.EndUserAddress1, a.EndUserAddress2, a.EndUserAddress3,
			c.CustomerName, c.Address1, c.Address2, c.Address3,
			c.CityCode,(SELECT LookUpValueName FROM gnMstLookUpDtl where CodeID = ''CITY'' AND ParaValue = c.CityCode) as CityName, 
			c.PhoneNo, c.HPNo, c.birthDate ,b.Salesman, (SELECT top 1 EmployeeName FROM gnMstEmployee where EmployeeID = b.Salesman AND CompanyCode=a.CompanyCode AND BranchCode=a.BranchCode) SalesmanName, b.SalesType
	from dbo.omMstVehicle z 
		left join omTrSalesSOVin a 
			on a.CompanyCode = z.CompanyCode 
			and z.SONo=a.SONo
				AND a.ChassisCode = z.ChassisCode
				AND a.ChassisNo = z.ChassisNo
		left join omTrSalesSO b
			on a.companyCode = b.CompanyCode 
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
			on f.CompanyCode = z.CompanyCode
				and f.SalesModelCode = z.SalesModelCode
	where 
		z.CompanyCode = ''' + @CompanyCode + '''
		and z.ReqOutNo = ''''
		and z.status in (''3'',''4'',''5'',''6'')
		and not exists (select 1 from omTrSalesReqdetail where ChassisCode=z.ChassisCode and ChassisNo=z.ChassisNo)
		and ((case when z.ChassisNo is not null then z.SONo end) is not null 
			or (case when z.ChassisNo is not null then z.DONo end) is not null)
		and f.IsCBU = ' + CONVERT(VARCHAR, @CBU, 1) + '
) #t2

SELECT * INTO #t3 FROM (
SELECT * FROM #t1 a 
UNION ALL
SELECT * FROM #t2 b
)#3

SELECT * FROM #t3
		WHERE ((case when ' + CONVERT(VARCHAR, @isDirect, 1) + ' = ''1'' then BranchCode end)= ''' + @Penjual + '''
		or (case when ' + CONVERT(VARCHAR, @isDirect, 1) + ' <> ''1'' then CustomerCode end)= ''' + @Penjual + ''' )

drop table #t1
drop table #t2
drop table #t3
'

Exec (@QRYTmp);
