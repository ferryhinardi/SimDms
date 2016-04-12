ALTER view [dbo].[CsLkuBpkbReminderView]  
as  
select a.CompanyCode  
     , a.BranchCode  
  , a.CustomerCode  
  , a.CustomerName  
  , b.DODate  
  , b.Chassis  
  , b.Engine  
  , b.SalesModelCode  
  , b.SalesModelYear  
  , b.ColourCode  
  , c.BpkbReadyDate  
  , c.BpkbPickUp  
  , b.BPKDate  
  , b.IsLeasing  
  , b.LeasingCo  
  , b.Installment  
  , case isnull(b.isLeasing, 0) when 0 then 'Cash' else 'Leasing' end as LeasingDesc  
  , case isnull(b.isLeasing, 0) when 0 then '-' else b.LeasingName end as LeasingName  
  , case isnull(b.isLeasing, 0) when 0 then '-' else (convert(varchar, isnull(b.Installment, 0)) + ' Month') end as Tenor  
  , a.Address  
  , a.PhoneNo  
  , a.HpNo  
  , a.AddPhone1  
  , a.AddPhone2  
  , b.SalesmanCode   
  , b.SalesmanName  
  , c.ReqKtp  
     , c.ReqStnk  
     , c.ReqSuratKuasa  
     , c.Comment  
     , c.Additional  
  , BpkbDate = d.BPKBDate  
  , StnkDate = d.StnkDate  
  , Category = (   
   case   
     when isnull(b.isLeasing, 0) = 0 then 'Tunai'   
     else 'Leasing'  
   end  
    )  
     , c.Status  
  , (case c.Status when 1 then 'Finish' else 'In Progress' end) as StatusInfo  
  , b.PoliceRegNo  
  , InputDate = c.CreatedDate  
  , CustCreatedDate = a.CreatedDate  
  , DelayedRetrievalDate = (  
   select top 1 x.RetrievalEstimationDate  
     from CsBpkbRetrievalInformation x  
    where x.CompanyCode = a.CompanyCode  
      and x.CustomerCode = a.CustomerCode  
    order by x.RetrievalEstimationDate desc  
    )  
  , DelayedRetrievalNote = (  
   select top 1 x.Notes  
     from CsBpkbRetrievalInformation x  
    where x.CompanyCode = a.CompanyCode  
      and x.CustomerCode = a.CustomerCode  
    order by x.RetrievalEstimationDate desc  
    )  
  , Outstanding = (  
   case   
    when isnull(c.CustomerCode, '') = '' then 'Y'
	when ( 
			select top 1 x.RetrievalEstimationDate 
			  from CsBpkbRetrievalInformation x 
			 where x.CompanyCode = a.CompanyCode
			   and x.CustomerCode = a.CustomerCode
			 order by x.RetrievalEstimationDate desc
		 ) > getdate() then 'Y'
	when b.isLeasing = 1 and (b.BPKBOutDate is null or b.BPKBInDate is null) then 'Y'
	/*when c.Status = 0 then 'Y'*/
	else 'N'
   end   
    ),  
    c.Reason  
  from CsCustomerView a  
  left join CsCustomerVehicleView b  
    on b.CompanyCode = a.CompanyCode  
   and b.BranchCode = a.BranchCode  
   and b.CustomerCode = a.CustomerCode  
  left join CsCustBpkb c  
    on c.CompanyCode = b.CompanyCode  
   and c.CustomerCode = b.CustomerCode  
  left join CsCustomerVehicle d  
    on d.CompanyCode = c.CompanyCode  
   and d.Chassis = b.Chassis  
  
  