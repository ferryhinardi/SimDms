if object_id('SvCustomerVehicleView') is not null
	drop procedure SvCustomerVehicleView
GO
CREATE view [dbo].[SvCustomerVehicleView]
as 

select distinct a.CompanyCode
,e.BranchCode
, (a.ChassisCode + convert(varchar, a.ChassisNo)) as VinNo
,a.PoliceRegNo
,isnull(b.CustomerName, '') CustomerName
,rtrim(rtrim(b.Address1) + ' ' + rtrim(b.Address2) + ' ' + rtrim(b.Address3) + ' ' + rtrim(b.Address4)) as CustomerAddr
, b.Address1
, b.Address2
, b.Address3 
, b.Address4
,a.BasicModel
,a.ChassisNo
,a.ChassisCode
,a.EngineCode
,a.EngineNo
,a.TransmissionType
,a.ServiceBookNo
,case a.LastServiceDate when ('19000101') then '' else a.LastServiceDate end LastServiceDate
,a.LastJobType
, a.ColourCode
, a.CustomerCode
from svMstCustomerVehicle a
left join gnMstCustomer b on b.CompanyCode = a.CompanyCode 
	and b.CustomerCode = a.CustomerCode	
inner join gnMstCustomerProfitCenter e on 
    e.CompanyCode = a.CompanyCode and
    e.CustomerCode = a.CustomerCode
where a.IsActive = 1 and e.ProfitCenterCode = '200'

--select distinct a.CompanyCode 
--	 , c.BranchCode
--	 , a.ChassisCode
--     , a.ChassisNo
--     , (a.ChassisCode + convert(varchar, a.ChassisNo)) as VinNo
--     , a.BasicModel
--     , a.PoliceRegNo
--     , a.ServiceBookNo
--     , a.CustomerCode
--     , b.CustomerName
--     , a.ContractNo
--     , a.IsContractStatus
--     , case d.ContractDate
--         when ('19000101') then null
--        else d.ContractDate
--       end ContractDate
--     , a.ClubNo
--     , case a.ClubDateFinish
--         when ('19000101') then null
--         else a.ClubDateFinish
--       end ClubDateFinish
--     , a.IsClubStatus
--     , b.Address1
--     , b.Address2
--     , b.Address3 
--     , b.Address4
--     , b.CityCode
--     , e.LookUpValueName as CityName
--     , b.PhoneNo
--     , b.HPNo
--     , b.FaxNo
--     , a.TransmissionType
--     , a.TechnicalModelCode
--     , case a.LastServiceDate
--         when ('19000101') then null
--         else a.LastServiceDate
--       end LastServiceDate
--     , a.LastJobType
--     , a.ColourCode
--     , a.EngineCode
--     , a.EngineNo
--     , c.MaterialDiscPct
--     , c.PartDiscPct
--     , c.LaborDiscPct
--  from svMstCustomerVehicle a
--  left join gnMstCustomer b
--    on b.CompanyCode = a.CompanyCode
--   and b.CustomerCode = a.CustomerCode
-- inner join gnMstCustomerProfitCenter c
--    on c.CustomerCode = b.CustomerCode 
--   and c.CompanyCode = b.CompanyCode
--   and c.ProfitCenterCode = '200'
--	LEFT JOIN svMstContract d
--	ON d.CompanyCode = b.CompanyCode
--	AND d.CustomerCode = b.CustomerCode
--	LEFT JOIN gnMstLookUpDtl e
--	ON e.CompanyCode = b.CompanyCode
--	AND e.CodeID = 'CITY'
--	AND e.LookUpValue = b.CityCode
-- where isnull(a.BasicModel, '') <> ''
 

GO


