
go
if object_id('uspfn_spMstCompanyAccountDtl') is not null	
	drop procedure uspfn_spMstCompanyAccountDtl

go
create procedure uspfn_spMstCompanyAccountDtl (  
	@CompanyCode varchar(10) , 
    @Search varchar(50) = ''
)
as
begin
  IF @Search <> ''
		  select  distinct
		(select   LookUpValueName from gnMstLookUpDtl 
		inner join  gnMstAccount b
		on gnMstLookUpDtl.CompanyCode=b.CompanyCode  where CodeID='TPGO' and LookUpValue=a.TPGO ) as TPGO
		,a.InterCompanyAccNoTo AccountNo,b.[Description] as AccountName,a.CompanyCode,a.CompanyCodeTo
		from spMstCompanyAccountdtl a
		inner join gnMstAccount b
		on a.CompanyCode=b.CompanyCode and
		a.InterCompanyAccNoTo=b.AccountNo
		inner join  spMstCompanyAccount c 
		on a.CompanyCode=c.CompanyCode and a.CompanyCodeTo=c.CompanyCodeTo 
		where a.CompanyCode=@CompanyCode 
		and (b.[Description] like '%' + @Search + '%' or
		     [TPGO] like '%' + @Search + '%' or
			  a.InterCompanyAccNoTo like '%' + @Search + '%' or
			 a.CompanyCodeTo like '%' + @Search + '%'

		)
  else
		select    
		(select  LookUpValueName from gnMstLookUpDtl  where CodeID='TPGO' and LookUpValue=a.TPGO ) as 'TPGO'
		,a.InterCompanyAccNoTo  'AccountNo',b.[Description]  as 'AccountName',a.CompanyCode,a.CompanyCodeTo
		from spMstCompanyAccountdtl a
		inner join gnMstAccount b
		on a.CompanyCode=b.CompanyCode and
			a.InterCompanyAccNoTo=b.AccountNo
		inner join  spMstCompanyAccount c 
		on a.CompanyCode=c.CompanyCode and a.CompanyCodeTo=c.CompanyCodeTo 
			where a.CompanyCode=@CompanyCode 
end