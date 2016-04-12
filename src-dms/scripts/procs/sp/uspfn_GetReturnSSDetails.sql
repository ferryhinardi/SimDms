 CREATE procedure uspfn_spGetReturnSSDetails @CompanyCode varchar(15), @BranchCode varchar(15), @ReturnNo varchar(25)    
  as    
  SELECT a.*, b.*, a.DocNo as LmpNo, a.DocDate as LmpDate, c.CustomerCode, c.CustomerName, c.CustomerGovName, c.Address1, c.Address2, c.Address3, c.Address4, c.HPNo, c.FaxNo, c.CityCode, d.LookUpValueName as City FROM spTrnSRTurSSHdr a    
  join svTrnService b ON a.CompanyCode = b.CompanyCode     
  and a.BranchCode = b.BranchCode and a.SKPNo = b.JobOrderNo    
  left join gnMstCustomer c    
  ON a.CompanyCode = c.CompanyCode    
  and a.CustomerCode = c.CustomerCode    
  left JOIN GnMstLookupDtl d on d.CompanyCode = c.CompanyCode    
 and d.CodeID = 'City'    
 and d.LookupValue = c.CityCode    
  where a.ReturnNo = @ReturnNo    
  and a.BranchCode = @BranchCode    
  and a.CompanyCode = @CompanyCode 