

create view [dbo].[RptWRSBrowse]  as
    
SELECT CompanyCode, BranchCode, WRSNo, WRSDate, BinningNo, BinningDate,
TypeOfGoods, TransType
FROM SpTrnPRcvHdr



GO


