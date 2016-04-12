
ALTER procedure [dbo].[uspfn_SvTrnInvoiceDraft]
--DECLARE
	@CompanyCode varchar(15),
	@BranchCode  varchar(15),
	@JobOrderNo  varchar(15)
as  

--SELECT @CompanyCode = '6006400001', @BranchCode  ='6006400103', @JobOrderNo  = 'SPK/15/001546'
	

declare @errmsg   varchar(max)
declare @BillType varchar(10)

begin try
--set nocount on

-- get data from SvTrnService
select * into #srv from (
  select * from svTrnService with(nowait,nolock) 
    where 1 = 1  
      and CompanyCode  = @CompanyCode  
      and BranchCode   = @BranchCode  
      and EstimationNo = @JobOrderNo  
      and EstimationNo!= ''
  union all
  select * from svTrnService with(nowait,nolock) 
    where 1 = 1  
      and CompanyCode = @CompanyCode  
      and BranchCode  = @BranchCode  
      and BookingNo   = @JobOrderNo  
      and BookingNo  != ''
  union all
  select * from svTrnService with(nowait,nolock) 
    where 1 = 1  
      and CompanyCode = @CompanyCode  
      and BranchCode  = @BranchCode  
      and JobOrderNo  = @JobOrderNo  
      and JobOrderNo != ''
)#srv

select BillType into #t1 from (
select b.BillType from #srv a, svTrnSrvItem b with(nowait,nolock) 
 where 1 = 1
   and b.CompanyCode = a.CompanyCode
   and b.BranchCode  = a.BranchCode
   and b.ProductType = a.ProductType
   and b.ServiceNo   = a.ServiceNo
union
select b.BillType from #srv a, svTrnSrvTask b with(nowait,nolock) 
 where 1 = 1
   and b.CompanyCode = a.CompanyCode
   and b.BranchCode  = a.BranchCode
   and b.ProductType = a.ProductType
   and b.ServiceNo   = a.ServiceNo
)#

set @BillType = (select top 1 a.BillType from svMstBillingType a with(nowait,nolock), #t1 b where b.BillType = a.BillType order by a.LockingBy)
drop table #t1

-- get dicount from Service
declare @ProductType     varchar(20)  set @ProductType     = isnull((select top 1 ProductType     from #srv),0)
declare @ServiceNo       bigint       set @ServiceNo       = isnull((select top 1 ServiceNo       from #srv),0)

-- get ppn & pph dicount from Service
declare @PPnPct decimal
    set @PPnPct = isnull((select a.TaxPct
 						    from gnMstTax a with(nowait,nolock), gnMstCustomerProfitCenter b with(nowait,nolock) , #srv c
						   where c.CompanyCode  = b.CompanyCode
						     and c.BranchCode   = b.BranchCode
						     and c.CustomerCodeBill = b.CustomerCode
						     and b.CompanyCode  = a.CompanyCode
						     and b.TaxCode      = a.TaxCode
						     and b.ProfitCenterCode = '200'
						     and b.TaxCode      = 'PPN'
							),0)

declare @PPhPct decimal
    set @PPhPct = isnull((select a.TaxPct
							from gnMstTax a with(nowait,nolock), gnMstCustomerProfitCenter b with(nowait,nolock) , #srv c
						   where c.CompanyCode  = b.CompanyCode
						     and c.BranchCode   = b.BranchCode
						     and c.CustomerCodeBill = b.CustomerCode
						     and b.CompanyCode  = a.CompanyCode
						     and b.TaxCode      = a.TaxCode
						     and b.ProfitCenterCode = '200'
						     and b.TaxCode      = 'PPH'
							),0)

-- get data gross amount
declare @LaborGrossAmt decimal
    set @LaborGrossAmt = isnull((
						select ceiling(sum(a.OperationHour * a.OperationCost))
						  from svTrnSrvTask a with(nowait,nolock), #srv b
						 where a.CompanyCode = b.CompanyCode
						   and a.BranchCode  = b.BranchCode
						   and a.ProductType = b.ProductType
						   and a.ServiceNo   = b.ServiceNo
						   and a.BillType    = @BillType
						),0)

declare @PartsGrossAmt decimal
    set @PartsGrossAmt = isnull((
						--select ceiling(sum((i.SupplyQty - i.ReturnQty) * i.RetailPrice))--Sebelum Perubahan
						select ceiling(sum(Round((i.SupplyQty - i.ReturnQty) * i.RetailPrice,0)))--Sesudah Perubahan
						  from svTrnSrvItem i with(nowait,nolock), gnMstLookUpDtl g with(nowait,nolock)
						 where g.CompanyCode = i.CompanyCode
					 	   and g.LookUpValue = i.TypeOfGoods
						   and g.CodeID      = 'GTGO'
						   and g.ParaValue   = 'SPAREPART'
						   and i.CompanyCode = @CompanyCode
						   and i.BranchCode  = @BranchCode
						   and i.ProductType = @ProductType
						   and i.ServiceNo   = @ServiceNo
						   and i.BillType    = @BillType
						),0)

declare @MaterialGrossAmt decimal
    set @MaterialGrossAmt = isnull((
						 --select ceiling(sum((i.SupplyQty - i.ReturnQty) * i.RetailPrice))--Sebelum Perubahan
						 select ceiling(sum(Round((i.SupplyQty - i.ReturnQty) * i.RetailPrice,0)))--Sesudah Perubahan
						   from svTrnSrvItem i with(nowait,nolock), gnMstLookUpDtl g with(nowait,nolock)
						  where g.CompanyCode = i.CompanyCode
							and g.LookUpValue = i.TypeOfGoods
							and g.CodeID      = 'GTGO'
							and g.ParaValue  in ('OLI','MATERIAL')
							and i.CompanyCode = @CompanyCode
							and i.BranchCode  = @BranchCode
							and i.ProductType = @ProductType
							and i.ServiceNo   = @ServiceNo
						    and i.BillType    = @BillType
						  ),0)

-- calculate discount
declare @LaborDiscAmt decimal
    set @LaborDiscAmt = isnull((
						 --select ceiling(sum(OperationHour * OperationCost * (DiscPct/100.0))) -- sebelum perbaikan
						 select (sum(OperationHour * OperationCost * (DiscPct/100.0))) --setelah perbaikan
						   from svTrnSrvTask with(nowait,nolock)
						  where CompanyCode = @CompanyCode
							and BranchCode = @BranchCode
							and ProductType = @ProductType
							and ServiceNo = @ServiceNo
						    and BillType    = @BillType
						  ),0)

declare @PartsDiscAmt decimal
    set @PartsDiscAmt = isnull((
						 --select ceiling(sum((i.SupplyQty - i.ReturnQty) * i.RetailPrice * (i.DiscPct/100.0)))--Sebelum Perubahan
						 select ceiling(sum(Round((i.SupplyQty - i.ReturnQty) * i.RetailPrice * (i.DiscPct/100.0),0)))--Sesudah Perubahan
						   from svTrnSrvItem i with(nowait,nolock), gnMstLookUpDtl g with(nowait,nolock)
						 where g.CompanyCode = i.CompanyCode
					 	   and g.LookUpValue = i.TypeOfGoods
						   and g.CodeID      = 'GTGO'
						   and g.ParaValue   = 'SPAREPART'
						   and i.CompanyCode = @CompanyCode
						   and i.BranchCode  = @BranchCode
						   and i.ProductType = @ProductType
						   and i.ServiceNo   = @ServiceNo
						   and i.BillType    = @BillType
						  ),0)

declare @MaterialDiscAmt decimal
    set @MaterialDiscAmt = isnull((
						 --select ceiling(sum((i.SupplyQty - i.ReturnQty) * i.RetailPrice * (i.DiscPct/100.0)))--Sebelum Perubahan
						 select ceiling(sum(Round((i.SupplyQty - i.ReturnQty) * i.RetailPrice * (i.DiscPct/100.0),0)))--Sesudah Perubahan
						   from svTrnSrvItem i with(nowait,nolock), gnMstLookUpDtl g with(nowait,nolock)
						  where g.CompanyCode = i.CompanyCode
							and g.LookUpValue = i.TypeOfGoods
							and g.CodeID      = 'GTGO'
							and g.ParaValue  in ('OLI','MATERIAL')
							and i.CompanyCode = @CompanyCode
							and i.BranchCode  = @BranchCode
							and i.ProductType = @ProductType
							and i.ServiceNo   = @ServiceNo
						    and i.BillType    = @BillType
						  ),0)

-- calculate DPP (dasar pengenaan pajak)
--declare @LaborDppAmt     decimal	set @LaborDppAmt     = floor(@LaborGrossAmt    - @LaborDiscAmt)--Sebelum Perubahan
--declare @PartsDppAmt     decimal	set @PartsDppAmt     = floor(@PartsGrossAmt    - @PartsDiscAmt)--Sebelum Perubahan
--declare @MaterialDppAmt  decimal	set @MaterialDppAmt  = floor(@MaterialGrossAmt - @MaterialDiscAmt)--Sebelum Perubahan
--declare @TotalDppAmt     decimal	set @TotalDppAmt     = floor(@LaborDppAmt + @PartsDppAmt + @MaterialDppAmt)--Sebelum Perubahan
declare @LaborDppAmt     decimal	set @LaborDppAmt     = Round(@LaborGrossAmt    - @LaborDiscAmt,0)--Sesudah Perubahan
declare @PartsDppAmt     decimal	set @PartsDppAmt     = Round(@PartsGrossAmt    - @PartsDiscAmt,0)--Sesudah Perubahan
declare @MaterialDppAmt  decimal	set @MaterialDppAmt  = Round(@MaterialGrossAmt - @MaterialDiscAmt,0)--Sesudah Perubahan
declare @TotalDppAmt     decimal	set @TotalDppAmt     = Round(@LaborDppAmt + @PartsDppAmt + @MaterialDppAmt,0)--Sesudah Perubahan

-- calculate ppn & service amount
declare @TotalPpnAmt     decimal	set @TotalPpnAmt = floor(@TotalDppAmt * (@PpnPct / 100.0))
declare @TotalPphAmt     decimal	set @TotalPphAmt = floor(@TotalDppAmt * (@PphPct / 100.0))
declare @TotalSrvAmt     decimal	set @TotalSrvAmt = floor(@TotalDppAmt + @TotalPphAmt + @TotalPpnAmt)

    
;with t1 as (
select a.CompanyCode, a.BranchCode, a.ProductType, a.ServiceNo
     , a.EstimationNo, a.EstimationDate, a.BookingNo, a.BookingDate, a.JobOrderNo, a.JobOrderDate, a.ServiceType, a.IsSparepartClaim
     , a.PoliceRegNo, a.ServiceBookNo, a.BasicModel, a.TransmissionType
     , a.ChassisCode, a.ChassisNo, a.EngineCode, a.EngineNo, a.ColorCode
     , rtrim(rtrim(a.ColorCode)
     + case isnull(b.RefferenceDesc2,'') when '' then '' else ' - ' end
     + isnull(b.RefferenceDesc2,'')) as ColorCodeDesc
     , a.Odometer
     , a.CustomerCode, c.CustomerName, c.Address1 CustAddr1
     , c.Address2 CustAddr2, c.Address3 CustAddr3, c.Address4 CustAddr4
     , c.CityCode CityCode, d.LookupValueName CityName
     , a.InsurancePayFlag, a.InsuranceOwnRisk, a.InsuranceNo, a.InsuranceJobOrderNo
     , a.CustomerCodeBill, e.CustomerName CustomerNameBill
     , e.Address1 CustAddr1Bill, e.Address2 CustAddr2Bill
     , e.Address3 CustAddr3Bill, e.Address4 CustAddr4Bill
     , e.CityCode CityCodeBill, f.LookupValueName CityNameBill
     , e.PhoneNo, e.FaxNo, e.HPNo, a.LaborDiscPct, a.PartDiscPct
     , a.ServiceRequestDesc, a.ConfirmChangingPart, a.EstimateFinishDate
     , a.MaterialDiscPct, a.JobType, a.ForemanID, a.MechanicID
     , a.ServiceStatus
	 , @LaborDppAmt LaborDppAmt, @PartsDppAmt PartsDppAmt, @MaterialDppAmt MaterialDppAmt
	 , @TotalDppAmt TotalDppAmt, @TotalPpnAmt TotalPpnAmt
	 , @TotalSrvAmt TotalSrvAmt
	 , a.LaborDppAmt SrvLaborDppAmt, a.PartsDppAmt SrvPartsDppAmt, a.MaterialDppAmt SrvMaterialDppAmt
	 , a.TotalDppAmount SrvTotalDppAmt, a.TotalPpnAmount SrvTotalPpnAmt
	 , a.TotalSrvAmount SrvTotalSrvAmt
  from svTrnService a with (nowait,nolock)
  left join omMstRefference b with (nowait,nolock)
    on b.CompanyCode = a.CompanyCode
   and b.RefferenceType = 'COLO'
   and b.RefferenceCode = a.ColorCode
  left join gnMstCustomer c with (nowait,nolock)
    on c.CompanyCode = a.CompanyCode
   and c.CustomerCode = a.CustomerCode
  left join gnMstLookupDtl d with (nowait,nolock)
    on d.CompanyCode = a.CompanyCode
   and d.CodeID = 'CITY'
   and d.LookUpValue = c.CityCode
  left join gnMstCustomer e with (nowait,nolock)
    on e.CompanyCode = a.CompanyCode
   and e.CustomerCode = a.CustomerCodeBill
  left join gnMstLookupDtl f with (nowait,nolock)
    on f.CompanyCode = a.CompanyCode
   and f.CodeID = 'CITY'
   and f.LookUpValue = e.CityCode
 where 1 = 1
   and a.CompanyCode = @CompanyCode
   and a.BranchCode  = @BranchCode
   and a.ServiceNo   = (select ServiceNo from #srv)
) 
select a.CompanyCode, a.BranchCode, a.ProductType --, JobOrderNo, 
	 , a.ServiceNo, a.ServiceType
     , a.EstimationNo, a.EstimationDate, a.BookingNo, a.BookingDate, a.JobOrderNo, a.JobOrderDate
     , '' InvoiceNo, z.Remarks 
     -- Data Kendaraan
     , a.PoliceRegNo, a.ServiceBookNo, a.BasicModel, a.TransmissionType
     , a.ChassisCode, a.ChassisNo, a.EngineCode, a.EngineNo
     , a.ColorCode, a.ColorCodeDesc, a.Odometer
     -- Data Contract
     , b.IsContractStatus IsContract
     , b.ContractNo
	 , c.EndPeriod ContractEndPeriod
	 , c.IsActive ContractStatus
	 , case b.IsContractStatus 
		when 1 then (case c.IsActive when 1 then 'Aktif' else 'Tidak Aktif' end)
		else ''
	   end ContractStatusDesc
     -- Data Contract
	 , b.IsClubStatus IsClub
	 , b.ClubCode
	 , b.ClubDateFinish ClubEndPeriod
	 , d.IsActive ClubStatus
	 , case b.IsClubStatus
		when 1 then (case d.IsActive when 1 then 'Aktif' else 'Tidak Aktif' end)
		else ''
	   end ClubStatusDesc
     -- Data Customer
     , a.CustomerCode, a.CustomerName
     , a.CustAddr1, a.CustAddr2, a.CustAddr3, a.CustAddr4
     , a.CityCode, a.CityName
     -- Data Customer Bill
     , a.InsurancePayFlag, a.InsuranceOwnRisk, a.InsuranceNo, a.InsuranceJobOrderNo
     , a.CustomerCodeBill, a.CustomerNameBill
     , a.CustAddr1Bill, a.CustAddr2Bill, a.CustAddr3Bill, a.CustAddr4Bill
     , a.CityCodeBill, a.CityNameBill
     , a.PhoneNo, a.FaxNo, a.HPNo
     , a.LaborDiscPct, a.PartDiscPct, a.PartDiscPct PartsDiscPct, a.MaterialDiscPct
     --, IsPPnBill
     -- Data Pekerjaan
     , a.ServiceRequestDesc
     , a.JobType, e.Description JobTypeDesc
     , a.ConfirmChangingPart, a.EstimateFinishDate
     , a.ForemanID, g.EmployeeName ForemanName
	 , a.MechanicID, h.EmployeeName MechanicName, a.IsSparepartClaim
	 -- Data Total Biaya Perawatan
     , a.LaborDppAmt, a.PartsDppAmt, a.MaterialDppAmt, a.TotalDppAmt
     , a.TotalPpnAmt, a.TotalSrvAmt
     , a.SrvLaborDppAmt, a.SrvPartsDppAmt, a.SrvMaterialDppAmt, a.SrvTotalDppAmt
     , a.SrvTotalPpnAmt, a.SrvTotalSrvAmt

     , a.ServiceStatus
	 , f.Description ServiceStatusDesc
	 , isnull(i.TaxCode,'') TaxCode
	 , isnull(j.TaxPct,0) TaxPct
  from t1 a
  left join svMstCustomerVehicle b with (nowait,nolock)
    on b.CompanyCode = a.CompanyCode
   and b.ChassisCode = a.ChassisCode
   and b.ChassisNo = a.ChassisNo
  left join svMstContract c with (nowait,nolock)
    on c.CompanyCode = a.CompanyCode
   and c.ContractNo = b.ContractNo
   and b.IsContractStatus = 1
  left join svMstClub d with (nowait,nolock)
    on d.CompanyCode = a.CompanyCode
   and d.ClubCode = b.ClubCode
  left join SvMstRefferenceService e with (nowait,nolock)
    on e.CompanyCode = a.CompanyCode
   and e.ProductType = a.ProductType
   and e.RefferenceCode = a.JobType
   and e.RefferenceType = 'JOBSTYPE'
  left join SvMstRefferenceService f with (nowait,nolock)
    on f.CompanyCode = a.CompanyCode
   and f.ProductType = a.ProductType
   and f.RefferenceCode = a.ServiceStatus
   and f.RefferenceType = 'SERVSTAS'
  left join gnMstEmployee g with (nowait,nolock)
    on g.CompanyCode = a.CompanyCode
   and g.BranchCode = a.BranchCode
   and g.EmployeeID = a.ForemanID
  left join gnMstEmployee h with (nowait,nolock)
    on h.CompanyCode = a.CompanyCode
   and h.BranchCode = a.BranchCode
   and h.EmployeeID = a.MechanicID
  left join gnMstCustomerProfitCenter i with (nowait,nolock)
    on i.CompanyCode = a.CompanyCode
   and i.BranchCode = a.BranchCode
   and i.CustomerCode = a.CustomerCode
   and i.ProfitCenterCode = '200'
  left join gnMstTax j with (nowait,nolock)
    on j.CompanyCode = a.CompanyCode
   and j.TaxCode = i.TaxCode
  left join svTrnInvoice z with (nowait, nolock)
   on a.JobOrderNo = z.JobOrderNo AND a.CompanyCode = z.CompanyCode AND a.BranchCode = z.BranchCode

end try
begin catch
    set @errmsg = 'Error Message:' + char(13) + error_message()
    raiserror (@errmsg,16,1);
	drop table #srv
end catch


