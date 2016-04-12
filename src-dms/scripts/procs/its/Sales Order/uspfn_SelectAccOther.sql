go
if object_id('uspfn_SelectAccOther') is not null
	drop procedure uspfn_SelectAccOther

go
create procedure uspfn_SelectAccOther
	@CompanyCode varchar(25),
	@Reff varchar(25)
as
begin
	SELECT a.RefferenceCode
	     , a.RefferenceDesc1 as RefferenceDesc
      FROM dbo.omMstRefference a
     WHERE a.CompanyCode = @CompanyCode
       AND a.RefferenceType = @Reff
       AND a.Status != '0'
end

