alter procedure uspfn_CsInqBpkbReminderSim
	@CompanyCode varchar(20),
	@BranchCode varchar(20),
	@DateFrom datetime,
	@DateTo datetime
as

select a.CompanyCode
     , b.BranchCode
     , a.CustomerCode
	 , c.CustomerName
	 , b.Chassis
	 , b.Engine
	 , b.SalesModelCode
	 , b.SalesModelYear
	 , b.ColourCode
	 , a.BpkbReadyDate
	 , a.BpkbPickUp
	 , b.BPKDate
	 , b.IsLeasing
	 , b.LeasingCo
	 , b.Installment
	 , case isnull(b.isLeasing, 0) when 0 then 'Cash' else 'Leasing' end as LeasingDesc
	 , case isnull(b.isLeasing, 0) when 0 then '-' else b.LeasingName end as LeasingName
	 , case isnull(b.isLeasing, 0) when 0 then '-' else (convert(varchar, isnull(b.Installment, 0)) + ' Month') end as Tenor
	 , c.Address
	 , c.PhoneNo
	 , c.HpNo
	 , c.AddPhone1
	 , c.AddPhone2
	 , b.SalesmanCode 
	 , b.SalesmanName
	 , a.ReqKtp
     , a.ReqStnk
     , a.ReqSuratKuasa
     , a.Chassis
     , a.Comment
     , a.Additional
     , a.Status
	 , (case a.Status when 1 then 'Finish' else 'In Progress' end) as StatusInfo
	 , b.PoliceRegNo
	 , a.CreatedDate
  from CsCustBpkb a
  join CsCustomerVehicleView b
    on b.CompanyCode = a.CompanyCode
   and b.CustomerCode = a.CustomerCode
   and b.Chassis = a.Chassis
  join CsCustomerView c
    on c.CompanyCode = a.CompanyCode
   and c.CustomerCode = a.CustomerCode
 where a.CompanyCode = @CompanyCode
   and b.BranchCode = (case when (isnull(@BranchCode, '') = '') then b.BranchCode else @BranchCode end)
   and convert(varchar, a.CreatedDate, 112) between @DateFrom and @DateTo
 order by a.CreatedDate




