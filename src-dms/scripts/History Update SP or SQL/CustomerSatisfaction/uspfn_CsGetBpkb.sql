IF OBJECT_ID('uspfn_CsGetBpkb') is not null
	drop proc dbo.uspfn_CsGetBpkb
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE procedure [dbo].[uspfn_CsGetBpkb]
	@CompanyCode  varchar(20),
	@CustomerCode varchar(20),
	@Chassis      varchar(50)
as

--select @CompanyCode = '6006406', @CustomerCode = '1000581', @Chassis = 'MHYGDN42VBJ352996'
select a.CompanyCode     
	 , a.BranchCode
	 , b.CustomerCode
	 , c.CustomerName
	 , a.ChassisCode + convert(varchar, a.ChassisNo) Chassis
	 , a.EngineCode + convert(varchar, a.EngineNo) Engine
	 , a.SalesModelCode
	 , SalesModel = a.SalesModelCode + '-' + (
					select SalesModelDesc
					  from omMstModel
					 where CompanyCode = a.CompanyCode
					   and SalesModelCode = a.SalesModelCode
	   )
	 , a.SalesModelYear
	 , a.ColourCode
	 , Colour = a.ColourCode + '-' + (
					select RefferenceDesc1 
					 from omMstRefference 
					where CompanyCode = a.CompanyCode
					  and RefferenceCode = a.ColourCode
					  and RefferenceType = 'COLO'
	   )
	 , g.BPKDate		BpkbDate
     , case when e.STNKInDate = '1900-01-01 00:00:00.000' then null else e.STNKInDate end StnkDate
	 , g.BPKDate
	 , h.isLeasing		IsLeasing
	 , h.LeasingCo
	 , isnull(h.isLeasing, 0) as IsLeasing
	 , case isnull(h.isLeasing, 0) when 0 then 'Tunai' else 'Leasing' end as Category
	 , i.CustomerName	LeasingName
	 , h.Installment
	 , convert(varchar, isnull(h.Installment, 0)) + ' Month' as Tenor
     , left(c.Address1, 40) as Address
     , c.PhoneNo
     , h.Salesman
     , j.EmployeeName	SalesmanName
     , case when e.BPKBInDate = '1900-01-01 00:00:00.000' then null else e.BPKBInDate end BpkbReadyDate
     , case when e.BPKBOutDate = '1900-01-01 00:00:00.000' then null else e.BPKBOutDate end BpkbPickUp
     , k.ReqInfoLeasing
     , k.ReqInfoCust
     , k.ReqKtp
     , k.ReqStnk
     , k.ReqSuratKuasa
     , k.Comment, k.Additional, k.Status
     , (case k.Status when 1 then 'Finish' else 'In Progress' end) as StatusInfo
     , case (isnull(k.Chassis, '')) when '' then 1 else 0 end as IsNew
     , l.PoliceRegNo
	 , DelayedRetrievalDate = (
			select top 1
			       x.RetrievalEstimationDate
			  from CsBpkbRetrievalInformation x
			 where x.CompanyCode = b.CompanyCode
			   and x.CustomerCode = b.CustomerCode
			 order by x.RetrievalEstimationDate desc
	   )
	 , DelayedRetrievalNote = (
			select top 1
			       x.Notes
			  from CsBpkbRetrievalInformation x
			 where x.CompanyCode = b.CompanyCode
			   and x.CustomerCode = b.CustomerCode
			 order by x.RetrievalEstimationDate desc
	   ), k.Reason

  from omTrSalesInvoiceVin a    
  left join omTrSalesInvoice b    
	on b.CompanyCode = a.CompanyCode    
   and b.BranchCode = a.BranchCode    
   and b.InvoiceNo = a.InvoiceNo    
  left join gnMstCustomer c
	on c.CompanyCode = a.CompanyCode    
   and c.CustomerCode = b.CustomerCode
  left join omTrSalesDODetail d
	on d.CompanyCode = a.CompanyCode    
   and d.BranchCode = a.BranchCode
   and d.ChassisCode = a.ChassisCode
   and d.ChassisNo = a.ChassisNo
  left join
	 ( 
	   select distinct 
		 	  a.CompanyCode
		    , a.BranchCode
		    , a.ChassisCode
		    , a.ChassisNo
		    , a.STNKInDate
		    , a.BPKBInDate
		    , isnull(b.BPKBOutDate, a.BPKBOutDate) BPKBOutDate
		 from omTrSalesSPKDetail a
		 left join omTrSalesSPKSubDetail b
	 	   on b.CompanyCode = a.CompanyCode
	      and b.BranchCode = a.BranchCode
	      and b.ChassisCode = a.ChassisCode
	      and b.ChassisNo = a.ChassisNo
	      and b.SPKNo = a.SPKNo
	 ) e
    on e.CompanyCode = a.CompanyCode
   and e.BranchCode = a.BranchCode
   and e.ChassisCode = a.ChassisCode
   and e.ChassisNo = a.ChassisNo
  left join omTrSalesDO f
	on f.CompanyCode = d.CompanyCode    
   and f.BranchCode = d.BranchCode
   and f.DONo = d.DONo
  left join omTrSalesBPK g
	on g.CompanyCode = f.CompanyCode    
   and g.BranchCode = f.BranchCode
   and g.DONo = f.DONo
  left join omTrSalesSO h
	on h.CompanyCode = g.CompanyCode    
   and h.BranchCode = g.BranchCode
   and h.SONo = g.SONo
  left join gnMstCustomer i
	on i.CompanyCode = h.CompanyCode
   and i.CustomerCode = h.LeasingCo
  left join HrEmployee j
	on j.CompanyCode = h.CompanyCode
   and j.EmployeeID = h.Salesman
  left join CsCustBpkb k
	on k.CompanyCode = a.CompanyCode
   and k.CustomerCode = b.CustomerCode
   and k.Chassis = a.ChassisCode + convert(varchar, a.ChassisNo)
  left join svMstCustomerVehicle l
	on l.CompanyCode = a.CompanyCode
   and l.ChassisCode = a.ChassisCode
   and l.ChassisNo = a.ChassisNo
 where 1 = 1
   and a.CompanyCode = @CompanyCode
   and b.CustomerCode = @CustomerCode
   and a.ChassisCode + convert(varchar, a.ChassisNo) = @Chassis

