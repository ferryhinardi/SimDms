go
if object_id('uspfn_HrChartEmplByGender') is not null
	drop procedure uspfn_HrChartEmplByGender

go
create procedure uspfn_HrChartEmplByGender
	@CompanyCode varchar(15)

as

select gender as category, count(*) as value
  from HrEmployee
 where CompanyCode = @CompanyCode 
 group by gender

