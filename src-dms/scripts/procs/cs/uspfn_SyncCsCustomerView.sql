go
if object_id('uspfn_SyncCsCustomerView') is not null
	drop procedure uspfn_SyncCsCustomerView

go
create procedure uspfn_SyncCsCustomerView
as
begin
	;with x as (
	select a.CompanyCode
		 , BranchCode = isnull((
			select top 1 BranchCode from OmTrSalesSo
			 where CompanyCode = a.CompanyCode
			  and CustomerCode = a.CustomerCode
			order by SODate desc), '')
		 , a.CustomerCode
		 , a.CustomerName
		 , a.CustomerType
		 , rtrim(a.Address1) + ' ' + rtrim(a.Address2) + rtrim(a.Address3) as Address
		 , a.PhoneNo
		 , a.HPNo
		 , b.AddPhone1
		 , b.AddPhone2
		 , a.BirthDate
		 , b.ReligionCode
		 , a.CreatedDate
		 , a.LastUpdateDate
	  from GnMstCustomer a
	  left join CsCustData b
		on b.CompanyCode = a.CompanyCode
	   and b.CustomerCode = a.CustomerCode
	 where 1 = 1
	   and a.CustomerType = 'I'
	   and a.BirthDate is not null
	   and a.BirthDate > '1900-01-01'
	   and (year(getdate() - year(a.BirthDate))) > 5
	   and year(a.LastUpdateDate) = year(getdate())
	)
	select * into #t1 from (select * from x where BranchCode != '')#

	delete CsCustomerView
	 where exists (
		select top 1 1 from #t1
		 where #t1.CompanyCode = CsCustomerView.CompanyCode
		   and #t1.BranchCode = CsCustomerView.BranchCode
		   and #t1.CustomerCode = CsCustomerView.CustomerCode
	 )
	insert into CsCustomerView (CompanyCode, BranchCode, CustomerCode, CustomerName, CustomerType, Address, PhoneNo, HPNo, AddPhone1, AddPhone2, BirthDate, ReligionCode, CreatedDate, LastUpdateDate)
	select * from #t1

	--drop table CsCustomerView
	--select * into CsCustomerView from #t1


	drop table #t1
end


go
if object_id('uspfn_SyncCsCustomerViewInitialize') is not null
	drop procedure uspfn_SyncCsCustomerViewInitialize

go
create procedure uspfn_SyncCsCustomerViewInitialize
as
begin
	;with x as (
	select a.CompanyCode
		 , BranchCode = isnull((
			select top 1 BranchCode from OmTrSalesSo
			 where CompanyCode = a.CompanyCode
			  and CustomerCode = a.CustomerCode
			order by SODate desc), '')
		 , a.CustomerCode
		 , a.CustomerName
		 , a.CustomerType
		 , rtrim(a.Address1) + ' ' + rtrim(a.Address2) + rtrim(a.Address3) as Address
		 , a.PhoneNo
		 , a.HPNo
		 , b.AddPhone1
		 , b.AddPhone2
		 , a.BirthDate
		 , b.ReligionCode
		 , a.CreatedDate
		 , a.LastUpdateDate
	  from GnMstCustomer a
	  left join CsCustData b
		on b.CompanyCode = a.CompanyCode
	   and b.CustomerCode = a.CustomerCode
	 where 1 = 1
	   and a.CustomerType = 'I'
	   and a.BirthDate is not null
	   and a.BirthDate > '1900-01-01'
	   and (year(getdate() - year(a.BirthDate))) > 5
	   and year(a.LastUpdateDate) = year(getdate())
	)
	select * into #t1 from (select * from x where BranchCode != '')#

--	delete CsCustomerView
--	 where exists (
--		select top 1 1 from #t1
--		 where #t1.CompanyCode = CsCustomerView.CompanyCode
--		   and #t1.BranchCode = CsCustomerView.BranchCode
--		   and #t1.CustomerCode = CsCustomerView.CustomerCode
--	 )
--	insert into CsCustomerView (CompanyCode, BranchCode, CustomerCode, CustomerName, CustomerType, Address, PhoneNo, HPNo, AddPhone1, AddPhone2, BirthDate, ReligionCode, CreatedDate, LastUpdateDate)
--	select * from #t1

--	drop table CsCustomerView
	select * into CsCustomerView from #t1


	drop table #t1
end

 


go
if exists ( select * from Sys.Views where Name = 'CsCustomerView' )
	drop view CsCustomerView

if exists ( select * from Sys.Tables where Name = 'CsCustomerView' )
	drop table CsCustomerView


exec uspfn_SyncCsCustomerViewInitialize
exec uspfn_SyncCsCustomerView
