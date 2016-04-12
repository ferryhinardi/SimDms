if object_id('[dbo].[usprpt_omRevRegFakPol]') is not null drop procedure [dbo].[usprpt_omRevRegFakPol]
go

create procedure usprpt_omRevRegFakPol
	@CompanyCode varchar(15)
,	@BranchCode varchar(15)
,	@DateFrom date
,	@DateTo date
as

-- created by Benedict 27 Jan 2016
-- exec usprpt_omRevRegFakPol '6006400001', '6006400101', '20160101', '20160126'

--declare 
--	@CompanyCode varchar(15)
--,	@BranchCode varchar(15)
--,	@DateFrom date
--,	@DateTo date
--select @CompanyCode = '6006400001', @BranchCode = '6006400101', @DateFrom = '20160101', @DateTo = '20160126'

select 
	   row_number() over(order by RevisionDate) [No]
     , RevisionNo
	 , RevisionDate
	 , RevisionSeq
	 , RevisionCode
	 , d.LookUpValueName as RevisionName
	 , ChassisCode
	 , ChassisNo
	 , FakturPolisiName
	 , FakturPolisiAddress1
	 , FakturPolisiAddress2
	 , FakturPolisiAddress3
	 , PostalCode
	 , PostalCodeDesc
	 , c.LookUpValueName as FakturPolisiCity
	 , FakturPolisiTelp1
	 , FakturPolisiTelp2
	 , FakturPolisiHP
	 , FakturPolisiBirthday
	 , FakturPolisiNo
	 , FakturPolisiDate
	 , IDNo
from omTrSalesFPolRevision a
inner join gnMstDealerMapping b on a.CompanyCode = b.DealerCode
inner join gnMstLookUpDtl c on a.FakturPolisiCity = c.LookUpValue
inner join gnMstLookUpDtl d on a.RevisionCode = d.LookUpValue
where a.CompanyCode = @CompanyCode
and a.BranchCode = @BranchCode
and c.CodeID = 'CITY'
and d.CodeID = 'REVI'
and a.RevisionDate between @DateFrom and @DateTo
and b.isActive = 1


