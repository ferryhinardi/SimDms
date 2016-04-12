declare @CompanyCode as varchar(15)
set @CompanyCode = (select top 1 CompanyCode from gnMstCoProfile)

declare @CodeID as varchar(15)
set @CodeID = 'FLPG'

INSERT INTO gnMstLookupDtl VALUES (@CompanyCode, @CodeID, 'FPJ_NAME', 3, '0', '0 : Customer Name in FPJInfo not displayed, 1 : Customer Name in FPJInfo displayed', 'SIM', getdate(), 'sa', getdate())
INSERT INTO gnMstLookupDtl VALUES (@CompanyCode, @CodeID, 'FPJ_ADDR', 4, '0', '0 : Customer Address base on Master Customer, 1 : Customer Address base on FPJInfo', 'SIM', getdate(), 'sa', getdate())
