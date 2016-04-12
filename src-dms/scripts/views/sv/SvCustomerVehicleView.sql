if object_id('SvCustomerVehicleView') is not null
	drop view SvCustomerVehicleView

go
CREATE VIEW [dbo].[SvCustomerVehicleView]
AS
SELECT  
                         a.CompanyCode, c.BranchCode, a.ChassisCode, a.ChassisNo, a.ChassisCode + CONVERT(varchar, a.ChassisNo) AS VinNo, a.BasicModel, a.PoliceRegNo, 
                         a.ServiceBookNo, a.CustomerCode, b.CustomerName, a.ContractNo, a.IsContractStatus, CASE d .ContractDate WHEN ('19000101') THEN NULL 
                         ELSE d .ContractDate END AS ContractDate, a.ClubNo, CASE a.ClubDateFinish WHEN ('19000101') THEN NULL ELSE a.ClubDateFinish END AS ClubDateFinish, 
                         a.IsClubStatus, b.Address1, b.Address2, b.Address3, b.Address4, b.CityCode, e.LookUpValueName AS CityName, b.PhoneNo, b.HPNo, b.FaxNo, a.TransmissionType, 
                         a.TechnicalModelCode, CASE a.LastServiceDate WHEN ('19000101') THEN NULL ELSE a.LastServiceDate END AS LastServiceDate, a.LastJobType, a.ColourCode, 
                         a.EngineCode, a.EngineNo, c.MaterialDiscPct, c.PartDiscPct, c.LaborDiscPct
FROM            dbo.svMstCustomerVehicle AS a LEFT OUTER JOIN
                         dbo.gnMstCustomer AS b ON b.CompanyCode = a.CompanyCode AND b.CustomerCode = a.CustomerCode INNER JOIN
                         dbo.gnMstCustomerProfitCenter AS c ON c.CompanyCode = b.CompanyCode AND c.CustomerCode = b.CustomerCode AND 
                         c.ProfitCenterCode = '200' LEFT OUTER JOIN
                         dbo.svMstContract AS d ON d.CompanyCode = b.CompanyCode AND d.CustomerCode = b.CustomerCode LEFT OUTER JOIN
                         dbo.gnMstLookUpDtl AS e ON e.CompanyCode = b.CompanyCode AND e.CodeID = 'CITY' AND e.LookUpValue = b.CityCode
WHERE        (ISNULL(a.BasicModel, '') <> '') AND (a.IsActive = 1)


--create view SvCustomerVehicleView
--as 
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

