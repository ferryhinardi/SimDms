

create view [dbo].[sp_spMstCompanyAccountDtl]
as
select  distinct  a.CompanyCode,a.CompanyCodeTo,d.LookUpValue as TPGOID, d.LookUpValueName as TPGO
,a.InterCompanyAccNoTo AccountNo,b.[Description] as AccountName
from spMstCompanyAccountdtl a
inner join gnMstAccount b
on a.CompanyCode=b.CompanyCode and
a.InterCompanyAccNoTo=b.AccountNo
inner join  spMstCompanyAccount c 
on a.CompanyCode=c.CompanyCode and a.CompanyCodeTo=c.CompanyCodeTo 
inner join gnMstLookUpDtl d
	on d.CompanyCode=b.CompanyCode and d.LookUpValue=a.TPGO 
	 where d.CodeID='TPGO'
GO


