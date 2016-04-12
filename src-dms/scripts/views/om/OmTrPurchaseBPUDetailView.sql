CREATE VIEW OmTrPurchaseBPUDetailView
AS
SELECT dtl.CompanyCode, dtl.BranchCode, dtl.BPUNo, dtl.PONo,dtl.BPUSeq, dtl.SalesModelCode, dtl.SalesModelYear, mdl.SalesModelDesc
    , dtl.ColourCode, ref.RefferenceDesc1 AS ColourName, dtl.ChassisCode
    , (CASE dtl.ChassisNo WHEN 0 THEN '' ELSE CONVERT(Varchar, dtl.ChassisNo) END) AS ChassisNo 
    , dtl.EngineCode, (CASE dtl.EngineNo WHEN 0 THEN '' ELSE CONVERT(Varchar, dtl.EngineNo) END) AS EngineNo
    , dtl.ServiceBookNo, dtl.KeyNo, dtl.Remark, dtl.isReturn
FROM OmTrPurchaseBPUDetail dtl
LEFT JOIN OmMstModelYear mdl ON dtl.CompanyCode = mdl.CompanyCode AND dtl.SalesModelCode = mdl.SalesModelCode
    AND dtl.SalesModelYear = mdl.SalesModelYear
LEFT JOIN OmMstRefference ref ON dtl.CompanyCode = ref.CompanyCode AND dtl.ColourCode = ref.RefferenceCode
    AND ref.RefferenceType = 'COLO'
