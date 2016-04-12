
go
if object_id('uspfn_CsInqCustFeedback') is not null
	drop procedure uspfn_CsInqCustFeedback

go
create procedure uspfn_CsInqCustFeedback
	@CompanyCode varchar(20),
	@DateFrom varchar(10),
	@DateTo varchar(10)
as

select distinct a.CompanyCode
     , a.CustomerCode
     , c.CustomerName
     , rtrim(c.Address1) + ' ' + rtrim(c.Address2) + rtrim(c.Address3) as Address
     , c.HPNo
     , b.BpkbDate
     , b.StnkDate
     , g.DODate 
     , e.SalesModelCode
     , e.SalesModelYear
     , h.PoliceRegNo
     , a.Chassis
     , a.IsManual
     , a.FeedbackA
     , a.FeedbackB
     , a.FeedbackC
     , a.FeedbackD
     , case a.IsManual when 1 then 'Manual' else 'System' end Feedback
     , case len(rtrim(isnull(a.FeedbackA,''))) when 0 then '-' else 'Ya' end Feedback01
     , case len(rtrim(isnull(a.FeedbackB,''))) when 0 then '-' else 'Ya' end Feedback02
     , case len(rtrim(isnull(a.FeedbackC,''))) when 0 then '-' else 'Ya' end Feedback03
     , case len(rtrim(isnull(a.FeedbackD,''))) when 0 then '-' else 'Ya' end Feedback04
  from CsCustFeedback a
  left join CsCustomerVehicle b
    on b.CompanyCode = a.CompanyCode
   and b.CustomerCode = a.CustomerCode
   and b.Chassis = a.Chassis
  left join GnMstCustomer c
    on c.CompanyCode = a.CompanyCode
   and c.CustomerCode = a.CustomerCode
  left join omTrSalesSOVin e          
    on e.CompanyCode = a.CompanyCode          
   and e.ChassisCode + convert(varchar, e.ChassisNo) = a.Chassis 
  left join omTrSalesSO f
    on f.CompanyCode = e.CompanyCode          
   and f.BranchCode = e.BranchCode
   and f.SONo = e.SONo
  left join omTrSalesDO g
    on g.CompanyCode = f.CompanyCode          
   and g.BranchCode = f.BranchCode
   and g.SONo = f.SONo
  left join GnMstCustomer d
    on d.CompanyCode = f.CompanyCode
   and d.CustomerCode = f.LeasingCo
  left join svMstCustomerVehicle h
    on h.CompanyCode = a.CompanyCode          
   and h.ChassisCode + convert(varchar, h.ChassisNo) = a.Chassis 
 where a.CompanyCode = @CompanyCode
   and convert(varchar, g.DODate, 112) between @DateFrom and @DateTo   
