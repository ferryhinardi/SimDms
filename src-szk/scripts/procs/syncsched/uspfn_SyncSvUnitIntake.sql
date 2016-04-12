alter procedure uspfn_SyncSvUnitIntake
    @MonthRange1 int = 0, 
    @MonthRange2 int = 0, 
    @CompanyCode varchar(20) = ''
as

declare @StartDate   date
declare @EndDate     date

set @StartDate   = left(convert(varchar, (dateadd(m, -@MonthRange1, getdate())), 112), 6) + '01'
set @EndDate     = left(convert(varchar, (dateadd(m, -@MonthRange1 + @MonthRange2 + 1, getdate())), 112), 6) + '01'

;with x as (
select CompanyCode, BranchCode, ServiceNo, JobOrderNo, JobOrderDate
  from SvUnitIntake
 where CompanyCode = (case isnull(@CompanyCode, '') when '' then CompanyCode else @CompanyCode end)
   and convert(date, JobOrderDate) >= @StartDate
   and convert(date, JobOrderDate) < @EndDate
)
delete x 

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
 where a.ServiceStatus in ('5', '7', '8', '9', 'A', 'B')
   and a.CompanyCode = (case isnull(@CompanyCode, '') when '' then a.CompanyCode else @CompanyCode end)
   and a.ServiceType = '2'
   and convert(date, a.JobOrderDate) >= @StartDate
   and convert(date, a.JobOrderDate) < @EndDate
)
, y as (
select x.*
     , rtrim(x.ChassisCode) + convert(varchar, x.ChassisNo) as VinNo 
     , isnull((select top 1 CompanyCode from omMstVehicle where CompanyCode = x.CompanyCode and ChassisCode = x.ChassisCode and ChassisNo = x.ChassisNo), '') as DealerCode
     --, isnull(x.JobOrderClosedDraft, (select top 1 InvoiceDate from SvTrnInvoice where CompanyCode = x.CompanyCode and BranchCode = x.BranchCode and JobOrderNo = x.JobOrderNo order by InvoiceDate)) JobOrderClosed
     , (select top 1 InvoiceDate from SvTrnInvoice where CompanyCode = x.CompanyCode and BranchCode = x.BranchCode and JobOrderNo = x.JobOrderNo order by InvoiceDate) JobOrderClosed
     , isnull((select top 1 SalesModelCode from OmMstVehicle where ChassisCode = x.ChassisCode and ChassisNo = x.ChassisNo), '') as SalesModelCode
     , isnull((select top 1 SuzukiDODate from OmMstVehicle where CompanyCode = x.CompanyCode and ChassisCode = x.ChassisCode and ChassisNo = x.ChassisNo), null) as DoDate
     , a.GroupNo as AreaCode
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
     , case f.IsLocked when 0 then 'Passenger' else 'Commercial' end as GroupType 
     , e.GroupJobType
     , g.Description as GroupJobTypeDesc
  from x
  left join GnMstDealerOutletMapping a
    on a.DealerCode = x.CompanyCode
   and a.OutletCode = x.BranchCode
  left join gnMstDealerMapping b
    on b.DealerCode = x.CompanyCode
   and b.GroupNo = a.GroupNo
  left join SvMstCustomerVehicle c
    on c.CompanyCode = x.CompanyCode
   and c.ChassisCode = x.ChassisCode
   and c.ChassisNo = x.ChassisNo
  left join gnMstCustomer d
    on d.CompanyCode = x.CompanyCode
   and d.CustomerCode = x.CustomerCode 
  left join svMstJob e
    on e.CompanyCode = '0000000'
   and e.BasicModel = x.BasicModel
   and e.ProductType = x.ProductType
   and e.JobType = x.JobType
  left join SvMstRefferenceService f
    on f.CompanyCode = '0000000'
   and f.RefferenceType = 'JOBSTYPE'
   and f.RefferenceCode = x.JobType
  left join SvMstRefferenceService g
    on g.CompanyCode = '0000000'
   and g.RefferenceType = 'GRPJOBTY'
   and g.RefferenceCode = e.GroupJobType
)
, z as (
select y.CompanyCode, y.BranchCode, y.ServiceNo, y.JobOrderNo, y.VinNo, y.JobOrderClosed
     , y.DealerCode, isnull(a.DealerName, '') as DealerName, isnull(b.DealerName, '') as CompanyName
     , y.Area, y.AreaCode, y.Odometer, c.SalesModelDesc, y.SalesModelCode, y.ProductionYear, y.DoDate
     , y.PoliceRegNo, y.EngineNo, y.ChassisNo, y.CustomerCode, y.CustomerName
     , y.PhoneNo, y.OfficePhoneNo, y.HPNo, y.ContactName, y.Email, y.BirthDate
     , (
            case
                when y.Gender = 'F' then 'Female'
                when y.Gender = 'M' then 'Male'
                when y.Gender = 'L' then 'Male'
                else '-'
            end
       ) as Gender
     , isnull(y.Address, '') Address, y.JobType, y.JobTypeDesc, y.GroupJobType, y.GroupJobTypeDesc, d.EmployeeName as SaName, y.SaNik
     , y.BasicModel, y.GroupType, y.CreatedBy, y.CreatedDate, y.LastUpdateBy, y.LastUpdateDate, y.JobOrderDate
  from y
  left join DealerInfo a
    on a.DealerCode = y.DealerCode
  left join DealerInfo b
    on b.DealerCode = y.CompanyCode
  left join omMstModel c
    on c.CompanyCode = '0000000'
   and c.SalesModelDesc = y.SalesModelCode
  left join gnMstEmployee d
    on d.CompanyCode = y.CompanyCode
   and d.BranchCode = y.BranchCode
   and d.EmployeeID = y.SaNik
)
insert into SvUnitIntake (CompanyCode, BranchCode, ServiceNo, JobOrderNo, VinNo, JobOrderClosed, DealerCode, DealerName, CompanyName, Area, AreaCode, Odometer, SalesModelDesc, SalesModelCode, ProductionYear, DoDate, PoliceRegNo, EngineNo, ChassisNo, CustomerCode, CustomerName, PhoneNo, OfficePhoneNo, HPNo, ContactName, Email, BirthDate, Gender, Address, JobType, JobTypeDesc, GroupJobType, GroupJobTypeDesc, SaName, SaNik, BasicModel, GroupType, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate, JobOrderDate)
select * from z where not exists (select top 1 1 from SvUnitIntake where CompanyCode = z.CompanyCode and BranchCode = z.BranchCode and ServiceNo = z.ServiceNo)

-- update SalesModelCode from NSDS
;with x as (
select a.CompanyCode, a.BranchCode, a.VinNo, a.SalesModelCode
     , b.SalesModelCode SalesModelCodeNsds
  from SvUnitIntake a
  left join SuzukiR4..omHstInquirySalesNSDS b
    on b.ChassisCode + convert(varchar, b.ChassisNo) = a.VinNo
 where isnull(a.SalesModelCode, '') = ''
   and isnull(b.SalesModelCode, '') != ''
)
update x set SalesModelCode = SalesModelCodeNsds

-- update DealerCode and DealerName from NSDS
;with x as (
select a.CompanyCode, a.BranchCode, a.VinNo, a.DealerCode, a.DealerName
     , b.CustomerCode DealerCodeNsds, c.DealerName DealerNameNsds
  from SvUnitIntake a
  left join SuzukiR4..omHstInquirySalesNSDS b
    on b.ChassisCode + convert(varchar, b.ChassisNo) = a.VinNo
  left join DealerInfo c
    on c.DealerCode = b.CustomerCode
 where isnull(a.DealerCode, '') = ''
   and isnull(b.CustomerCode, '') != ''
   and isnull(c.DealerName, '') != ''
)
update x set DealerCode = DealerCodeNsds, DealerName = DealerNameNsds

