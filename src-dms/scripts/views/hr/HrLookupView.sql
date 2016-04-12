
go
if object_id('HrLookupView') is not null
	drop view HrLookupView

go
create view HrLookupView
as

select a.CompanyCode, a.CodeID, b.CodeDescription, a.LookUpValue, a.LookUpValueName, a.ParaValue, a.SeqNo
  from gnMstLookUpDtl a, HrLookupMapping b
 where b.CompanyCode = a.CompanyCode
   and b.CodeID = a.CodeID 
  

