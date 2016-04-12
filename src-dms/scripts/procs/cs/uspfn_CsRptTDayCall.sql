
go
if object_id('uspfn_CsRptTDayCall') is not null
	drop procedure uspfn_CsRptTDayCall

go
--uspfn_CsRptTDayCall '%','9-9-2010','9-9-2013','4'
create proc uspfn_CsRptTDayCall
@CompanyCode varchar(20),
@pDateFrom datetime,
@pDateTo datetime,
@pStatus char(1)
as
begin
select a.CompanyCode 
     , a.CustomerCode
     , b.CustomerName
     , b.Address1
     , b.PhoneNo
     , BPKDate = convert(varchar(11),getdate(),106)
	 , STNKDate = convert(varchar(11),getdate(),106)
     , c.SalesModelCode as CarType
     , c.ColourCode as Color
     , f.PoliceRegNo 
     , c.EngineCode + convert(varchar, c.EngineNo) as Engine 
     , a.Chassis 
     , d.Salesman as SalesmanCode
     , e.EmployeeName as SalesmanName
     , a.IsDeliveredA 
     , a.IsDeliveredB 
     , a.IsDeliveredC 
     , a.IsDeliveredD 
     , a.IsDeliveredE 
     , a.IsDeliveredF 
     , a.Comment 
     , a.Additional 
     , a.Status 
     , (case a.Status when 0 then 'In Progress' else 'Finish' end) as StatusInfo
     , a.CreatedDate as InputDate
     , b.CustomerName
     , left(b.Address1, 40) as Address
     , c.BranchCode
     , (select top 1 BranchName from gnMstOrganizationDtl where branchcode = c.BranchCode) BranchName
     , c.CompanyCode
     , (select top 1 CompanyName from gnMstOrganizationHdr where companycode = c.CompanyCode) CompanyName
     , a.FinishDate
  from CsTDayCall a
  left join gnMstCustomer b
    on b.CompanyCode = a.CompanyCode
   and b.CustomerCode = a.CustomerCode
  left join omTrSalesSOVin c
    on c.CompanyCode = a.CompanyCode
   and (c.ChassisCode + convert(varchar, c.ChassisNo)) = a.Chassis
  left join omTrSalesSO d
    on d.CompanyCode = c.CompanyCode
   and d.BranchCode = c.BranchCode
   and d.SONo = c.SONo
  left join gnMstEmployee e
    on e.CompanyCode = d.CompanyCode
   and e.BranchCode = d.BranchCode
   and e.EmployeeID = d.Salesman
  left join svMstCustomerVehicle f
    on f.CompanyCode = a.CompanyCode
   and f.ChassisCode + convert(varchar, f.ChassisNo) = a.Chassis
  where 1=1
   and a.Status like case when @pStatus = 0 then @pStatus else '%' end
   and a.Status not like case when @pStatus = 0 then '' else '0' end
	--and convert(varchar, @pDateFrom, 112) < left(convert(varchar, BPKDate, 112), 6) + convert(varchar, convert(int, right(convert(varchar, BPKDate, 112), 2)) + 3)
	--and convert(varchar, @pDateTo, 112) > left(convert(varchar, BPKDate, 112), 6) + convert(varchar, convert(int, right(convert(varchar, BPKDate, 112), 2)) + 3)

end

GO


