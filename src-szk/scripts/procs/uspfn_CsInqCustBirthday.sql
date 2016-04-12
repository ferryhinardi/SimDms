alter procedure uspfn_CsInqCustBirthday
	@CompanyCode varchar(20),
	@BranchCode varchar(20),
	@PeriodYear int,
	@ParMonth1 int,
	@ParMonth2 int,
	@ParStatus int     -- 0: All, 1: Not Inputted, 2: Inputted
as

select a.CompanyCode
     , a.BranchCode
     , a.CustomerCode
	 , a.CustomerName
	 , a.Address
	 , a.PhoneNo
	 , a.HPNo
	 , b.AddPhone1
	 , b.AddPhone2
	 , a.BirthDate
	 , case isnull(c.CustomerCode, '') when '' then 'N' else 'Y' end IsReminder
	 , c.CreatedDate as InputDate
  from CsCustomerView a
  left join CsCustData b
	on b.CompanyCode = a.CompanyCode
   and b.CustomerCode = a.CustomerCode
  left join CsCustBirthDay c
	on c.CompanyCode = a.CompanyCode
   and c.CustomerCode = a.CustomerCode
   and c.PeriodYear = @PeriodYear
 where 1 = 1
   and a.CompanyCode = @CompanyCode
   and a.BranchCode = (case isnull(@BranchCode, '') when '' then a.BranchCode else @BranchCode end)
   and a.CustomerType = 'I'
   and a.BirthDate is not null
   and a.BirthDate > '1900-01-01'
   and (year(getdate() - year(a.BirthDate))) > 5
   and month(a.BirthDate) >= @ParMonth1
   and month(a.BirthDate) <= @ParMonth2
   and isnull(c.CustomerCode, '1900-01-01') = (case @ParStatus
		when 0 then isnull(c.CustomerCode, '1900-01-01')
		when 1 then '1900-01-01'
		else c.CustomerCode
		end)
 order by day(a.BirthDate)


 go

 uspfn_CsInqCustBirthday '6006406','6006406', 2014, 1, 5, 0
 --uspfn_CsInqCustBirthday @CompanyCode=N'6006406',@BranchCode=N'6006406',@PeriodYear=N'2014',@ParMonth=N'3',@ParStatus=N'0'

 








