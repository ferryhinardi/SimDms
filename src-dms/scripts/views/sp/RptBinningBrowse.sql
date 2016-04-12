ALTER view [dbo].[RptBinningBrowse]  as    
SELECT CompanyCode, BranchCode, BinningNo, BinningDate, Status,ReferenceNo,DNSupplierNo, TypeOfGoods
FROM spTrnPBinnHdr WITH(NOLOCK, NOWAIT)


GO


