
go
if object_id('SvJobTypeView') is not null
	drop view SvJobTypeView

go
create view SvJobTypeView
as 
select a.CompanyCode
     , a.BasicModel
     , a.JobType
     , b.Description
  from SvMstJob a
  left join svMstRefferenceService b
    on b.CompanyCode = a.CompanyCode
   and b.ProductType = a.ProductType
   and b.RefferenceCode = a.JobType
   and b.RefferenceType = 'JOBSTYPE'

