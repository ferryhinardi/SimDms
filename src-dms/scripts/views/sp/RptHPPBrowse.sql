
create view [dbo].[RptHPPBrowse]  as    

  SELECT a.CompanyCode, a.BranchCode, 
    a.HPPNo
    , a.HPPDate
    , a.WRSNo
    , a.WRSDate 
    ,(
        select LookUpValueName
		from gnMstLookUpDtl
		where CompanyCode=a.CompanyCode and CodeID='STAT' and LookUpValue=a.Status
    ) Status, a.TypeOfGoods
FROM 
    spTrnPHPP a


GO


