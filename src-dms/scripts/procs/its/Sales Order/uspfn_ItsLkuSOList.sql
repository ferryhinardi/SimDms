
go
if object_id('uspfn_ItsLkuSOList') is not null
	drop procedure uspfn_ItsLkuSOList

go
create procedure uspfn_ItsLkuSOList
	@CompanyCode varchar(13),
	@BranchCode varchar(13)
as

begin
	select a.CompanyCode as CompanyCode
	     , a.BranchCode as BranchCode
		 , a.SONo as SONumber
		 , a.SODate as SODate
		 , a.SalesType as SalesType
		 , a.RefferenceNo as ReffNumber
		 , a.RefferenceDate as ReffDate
		 , a.CustomerCode  as CustomerCode
		 , b.CustomerName
		 , Customer = b.CustomerCode + ' || ' + b.CustomerName
		 , CustomerAddress = b.Address1 + ' ' + ltrim(b.Address2) + ' ' + ltrim(b.Address3) + ' ' + ltrim(b.Address4)
		 , a.TOPCode as TOPCode
		 , a.TOPDays as TOPDays
		 , a.TOPDays as TOPName
		 , a.BillTo as ChargedToCode
		 , b.CustomerName as ChargedToName
		 , a.ShipTo as ShipToCode
		 , g.CustomerName as ShipToName
		 , a.ProspectNo as ITSNumber
		 , a.SKPKNo as SKPKNumber
		 , a.SalesCode as SalesmanCode
		 , isnull(c.EmployeeName, '-') as SalesmanName
		 , Salesman = isnull((c.EmployeeID + ' || ' + c.EmployeeName), '-')
		 , a.WareHouseCode as WarehouseCode
		 , f.LookUpValueName as WarehouseName
		 , a.isLeasing as IsLeasing
		 , a.LeasingCo as LeasingCode
		 , a.GroupPriceCode as GroupPriceCode
		 , e.RefferenceDesc1 as GroupPriceName
		 , a.Insurance as Insurance
		 , a.PaymentType as TOPPaidWith
		 , a.PrePaymentAmt as Advance 
		 , a.PrePaymentDate as ReceivingDate
		 , a.PrePaymentBy as PayeeCode
		 , PayeeName = (
				select top 1
				       x.EmployeeName
				  from HrEmployee x
				 where x.CompanyCode = a.CompanyCode
				   and x.EmployeeID = a.PrePaymentBy
		   )
		 , a.CommissionBy as MediatorName
		 , a.CommissionAmt as MediatorComission
		 , a.PONo as PONumber
		 , a.ContractNo as ContractNumber
		 , a.RequestDate as ContractDate
		 , a.Remark as ContractNote
		 , a.Status 
		 , a.ApproveBy
		 , a.ApproveDate 
		 , a.RejectBy 
		 , a.RejectDate 
		 , a.CreatedBy 
		 , a.CreatedDate 
		 , a.LastUpdateBy 
		 , a.LastUpdateDate 
		 , a.isLocked 
		 , a.LockingBy 
		 , a.LockingDate 
		 , a.Installment as Tenor
		 , a.FinalPaymentDate as PaidPaymentDate
		 , a.FinalPaymentDate
		 , a.SalesCoordinator 
		 , a.SalesHead 
		 , a.BranchManager 
		 , VehicleType = (select top 1 x.TipeKendaraan from pmKDP x where x.InquiryNumber = a.ProspectNo)
		 , IsDirectSales = (
				case
					when a.ProspectNo is null or rtrim(ltrim(a.ProspectNo)) = '' then convert(bit, 0)
					else convert(bit, 1)
				end
		   )
		 , SOStatus = (
				case 
					when a.Status = '0' then 'OPEN'
					when a.Status = '1' then 'PRINTED'
					when a.Status = '2' then 'APRROVED'
					when a.Status = '4' then 'REJECTED'
				end
		   )
		 , SaleType = (
				case 
					when a.SalesType = '1' then 'Direct'
					else 'Whole'
				end
		   )
		 , LeasingName = (
				select top 1
				       x.CustomerName
				  from gnMstCustomer x
				  where x.CompanyCode = a.CompanyCode
				    and x.CustomerCode = a.LeasingCo
		   )
	  from omTrSalesSO a
	  left join gnMstCustomer b
	    on b.CompanyCode = a.CompanyCode
	   and b.CustomerCode = a.CustomerCode
	  left join HrEmployee c
	    on c.CompanyCode = a.CompanyCode
	   and c.EmployeeID = a.SalesCode
	  left join gnMstLookupDtl d
	    on d.CodeID = 'TOPC'
	   and d.LookUpValue = a.TOPCode
	  left join omMstRefference e
	    on e.CompanyCode = a.CompanyCode
	   and e.RefferenceType = 'GRPR'
	   and e.RefferenceCode = a.GroupPriceCode
	  left join gnMstLookUpDtl f
	    on f.CompanyCode = a.CompanyCode
	   and f.CodeID = 'MPWH'
	   and f.ParaValue = @BranchCode
	  left join gnMstCustomer g
	    on g.CompanyCode = a.CompanyCode
	   and g.CustomerCode = a.ShipTo
	 where a.CompanyCode = @CompanyCode
	   and a.BranchCode = @BranchCode
	 order by a.SODate desc
end


go
exec uspfn_ItsLkuSOList '6115204', '611520402'

--select * from omTrSalesSO where BranchCode = '611520402'



