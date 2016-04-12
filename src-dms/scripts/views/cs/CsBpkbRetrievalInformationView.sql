
go
if object_id('CsBpkbRetrievalInformationView') is not null
	drop view CsBpkbRetrievalInformationView

go
create view CsBpkbRetrievalInformationView
as
select a.CompanyCode
     , a.CustomerCode
	 , a.RetrievalEstimationDate
	 , a.Notes
  from CsBpkbRetrievalInformation a



go
select * from CsBpkbRetrievalInformationView