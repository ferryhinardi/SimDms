

-- =============================================
-- Author:		fhy
-- Create date: 10122015
-- Description:	GetData Sv Unit Intake Grid - Dealer
-- =============================================
CREATE PROCEDURE [dbo].[uspfn_SvUnitIntakeGrid]    
    @CompanyCode varchar(20),
	@BranchCode varchar(20),
	@StartDate varchar(20),
    @EndDate varchar(20),
	@filterby varchar(max)=''

AS
BEGIN

declare
@query varchar(max)

set @query='
declare @t_svUnitIntake as table
(
	CompanyCode varchar(25),
	BranchCode varchar(25),
	ServiceNo varchar(25),
	JobOrderNo varchar(25),
	VinNo varchar(25),
	JobOrderClosed datetime,
	DealerCode varchar(25),
	DealerName varchar(250),
	CompanyName varchar(250),
	Area varchar(250),
	AreaCode varchar(25),
	Odometer numeric(18,2),
	SalesModelDesc varchar(250),
	SalesModelCode varchar(250),
	ProductYear int,
	DoDate Datetime,
	PoliceRegNo varchar(25),
	EngineNo varchar(25),
	ChassisNo varchar(25),
	CustomerCode varchar(25),
	CustomerName varchar(250),
	PhoneNo varchar(25),
	OfficePhoneNo varchar(25),
	HPNo varchar(25),
	ContactName varchar(250),
	Email varchar(250),
	BirthDate Datetime,
	Gender varchar(25),
	Alamat varchar(255),
	JobType varchar(25),
	JobTypeDesc varchar(250),
	GroupJobType varchar(25),
	GroupJobTypeDesc varchar(250),
	SaName varchar(250),
	SaNik varchar(25),
	BasicModel varchar(25),
	GroupType varchar(25),
	CreatedBy varchar(25),
	CreatedDate Datetime,
	LastUpdateBy varchar(25),
	LastUpdatedDate Datetime,
	JobOrderDate Datetime
)

;with x as (
select a.CompanyCode
     , a.BranchCode
     , a.ServiceNo
     , a.JobOrderNo
     , a.JobOrderDate
     , a.JobOrderClosed JobOrderClosedDraft
     , a.Odometer
     , a.PoliceRegNo
     , a.EngineNo
     , a.ChassisCode
     , a.ChassisNo
     , a.CustomerCode
     , a.JobType
     , a.ForemanID SaNik
     , a.BasicModel
     , a.ProductType
     , a.CreatedBy
     , a.CreatedDate
     , a.LastUpdateBy
     , a.LastUpdateDate
  from SvTrnService a
 where 1=1
   and a.CompanyCode = '''+@CompanyCode+'''
   and a.BranchCode = '''+@BranchCode+'''
   and a.ServiceStatus in (''5'', ''7'', ''8'', ''9'', ''A'', ''B'')
   and a.ServiceType = ''2''
   and a.JobOrderDate between  convert(datetime, '''+@StartDate+''', 120) and convert(datetime, '''+@EndDate+''', 120)

)
, y as (
select x.*
     , rtrim(x.ChassisCode) + convert(varchar, x.ChassisNo) as VinNo 
     , isnull((select top 1 CompanyCode from omMstVehicle where CompanyCode = x.CompanyCode and ChassisCode = x.ChassisCode and ChassisNo = x.ChassisNo), '''') as DealerCode
     , (select top 1 InvoiceDate from SvTrnInvoice where CompanyCode = x.CompanyCode and BranchCode = x.BranchCode and JobOrderNo = x.JobOrderNo order by InvoiceDate) JobOrderClosed
     , isnull((select top 1 SalesModelCode from OmMstVehicle where ChassisCode = x.ChassisCode and ChassisNo = x.ChassisNo), '''') as SalesModelCode
     , isnull((select top 1 SuzukiDODate from OmMstVehicle where CompanyCode = x.CompanyCode and ChassisCode = x.ChassisCode and ChassisNo = x.ChassisNo), null) as DoDate
     , b.GroupNo as AreaCode
     , b.Area 
     , c.ProductionYear 
     , d.CustomerName
     , d.PhoneNo
     , d.OfficePhoneNo
     , d.HPNo
     , c.ContactName
     , d.Email
     , d.BirthDate
     , d.Gender
     , d.Address1 + d.Address2 + d.Address3 + d.Address4 as Address
     , f.Description as JobTypeDesc
     , case f.IsLocked when 0 then ''Passenger'' else ''Commercial'' end as GroupType 
     , e.GroupJobType
     , g.Description as GroupJobTypeDesc
  from x
  left join GnMstDealerOutletMapping a on a.DealerCode = x.CompanyCode and a.OutletCode = x.BranchCode
  left join gnMstDealerMapping b on b.DealerCode = x.CompanyCode and b.Area = a.outletArea
  left join SvMstCustomerVehicle c on c.CompanyCode = x.CompanyCode and c.ChassisCode = x.ChassisCode and c.ChassisNo = x.ChassisNo
  left join gnMstCustomer d on d.CompanyCode = x.CompanyCode and d.CustomerCode = x.CustomerCode 
  left join svMstJob e on e.CompanyCode = '''+@CompanyCode+''' and e.BasicModel = x.BasicModel
			and e.ProductType = x.ProductType and e.JobType = x.JobType
  left join SvMstRefferenceService f on f.CompanyCode = '''+@CompanyCode+''' and f.RefferenceType = ''JOBSTYPE'' and f.RefferenceCode = x.JobType
  left join SvMstRefferenceService g on g.CompanyCode = '''+@CompanyCode+''' and g.RefferenceType = ''GRPJOBTY'' and g.RefferenceCode = e.GroupJobType
)

insert into @t_svUnitIntake
select y.CompanyCode, y.BranchCode, y.ServiceNo, y.JobOrderNo, y.VinNo, y.JobOrderClosed
     , y.DealerCode, isnull(a.DealerName, '''') as DealerName, isnull(b.DealerName, '''') as CompanyName
     , y.Area, y.AreaCode, y.Odometer, SalesModelDesc = null
	 , y.SalesModelCode, y.ProductionYear, y.DoDate
     , y.PoliceRegNo, y.EngineNo, y.ChassisNo, y.CustomerCode, y.CustomerName
     , y.PhoneNo, y.OfficePhoneNo, y.HPNo, y.ContactName, y.Email, y.BirthDate
     , (
            case
                when y.Gender = ''F'' then ''Female''
                when y.Gender = ''M'' then ''Male''
                when y.Gender = ''L'' then ''Male''
                else ''-''
            end
       ) as Gender
     , isnull(y.Address, '''') Address, y.JobType, y.JobTypeDesc, y.GroupJobType, y.GroupJobTypeDesc, d.EmployeeName as SaName, y.SaNik
     , y.BasicModel, y.GroupType, y.CreatedBy, y.CreatedDate, y.LastUpdateBy, y.LastUpdateDate, y.JobOrderDate
  from y
  left join gnMstDealerMapping a
    on a.DealerCode = y.DealerCode
  left join gnMstDealerMapping b
    on b.DealerCode = y.CompanyCode
  left join gnMstEmployee d
    on d.CompanyCode = y.CompanyCode
   and d.BranchCode = y.BranchCode
   and d.EmployeeID = y.SaNik

--select * from @t_svUnitIntake

select a.VinNo		
, a.JobOrderClosed
, e.OutletCode	
--, ISNULL(c.OutletName, e.OutletName) OutletName	
,  e.OutletName OutletName	
, a.GroupJobTypeDesc
, a.Odometer

, a.SalesModelCode	
, a.ProductYear
, a.PoliceRegNo		
, a.EngineNo
, a.ChassisNo		
, a.CustomerName

, a.PhoneNo		
, a.OfficePhoneNo
, a.HPNo		
, a.Email
, a.BirthDate	
, a.Gender
, a.Alamat		
, a.JobTypeDesc
, g.Area
, a.SaNik				
, a.SaName											
, g.DealerCode CompanyCode
, g.DealerName CompanyName
, a.BasicModel		
, a.DoDate			
, a.ContactName
, a.JobType			
, g.GroupNo
, a.ServiceNo		
, a.DealerCode
, a.DealerName		
, a.BranchCode
, a.JobOrderNo		
, a.AreaCode
, a.SalesModelDesc	
, a.CustomerCode
, a.GroupJobType	
, a.GroupType	
, a.JobOrderDate
, a.CompanyCode
, a.BranchCode
from 
@t_svUnitIntake a
inner join gnMstDealerMapping g
		on a.CompanyCode = g.DealerCode
inner join gnMstDealerOutletMapping e
		on e.DealerCode = g.DealerCode
	   and e.OutletCode = a.BranchCode
where e.OutletCode in (select OutletCode from GnMstDealerOutletMapping) --OUTLETCODE SVMST SAMA DG GNMST
and ( e.DealerCode in (select DealerCode from GnMstDealerOutletMapping))--DEALERCODE SVMST SAMA DG GNMST
'+@filterby+'
delete @t_svUnitIntake
'

print(@query)
exec(@query) 


END


GO


