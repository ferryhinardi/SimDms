/*
	uspfn_CsInqCustBirthday '100', '6159401000', '', 2012, 1, 2, 0
*/
CREATE procedure uspfn_CsInqCustBirthday
	@GroupNo VARCHAR(10),
	@CompanyCode varchar(20),
	@BranchCode varchar(20),
	@PeriodYear int,
	@ParMonth1 int,
	@ParMonth2 int,
	@ParStatus int     -- 0: All, 1: Not Inputted, 2: Inputted
as

--declare
--	@CompanyCode varchar(20),
--	@BranchCode varchar(20),
--	@PeriodYear int,
--	@ParMonth int,
--	@ParStatus int     -- 0: All, 1: Not Inputted, 2: Inputted

--select
--	@CompanyCode = '6006406',
--	@BranchCode = '6006406', 
--	@PeriodYear = 2014,
--	@ParMonth = '3', 
--	@ParStatus = 0     -- 0: All, 1: Not Reminder Yet, 2: Reminder


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
  join gnMstDealerOutletMapping d
	on d.DealerCode = a.CompanyCode
   and d.OutletCode = a.BranchCode
 where 1 = 1
   and d.GroupNo = (CASE ISNULL(@GroupNo , '') WHEN '' THEN d.GroupNo ELSE @GroupNo  END)
   and a.CompanyCode = (case isnull(@CompanyCode, '') when '' then a.CompanyCode else @CompanyCode end)
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