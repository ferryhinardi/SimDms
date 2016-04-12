CREATE procedure uspfn_spGetReturnSSDtl @CompanyCode varchar(15), @BranchCode varchar(15), @ReturnNo varchar(20)    
  as    
  select a.CompanyCode, a.BranchCode,a.ReturnNo,a.WarehouseCode , a.PartNo, a.PartNoOriginal, a.DocNo, c.QtyBill as QtyLmp, a.QtyReturn as QtyBill   from spTrnSRturSSDtl a    
  join spTrnSRturSSHdr b     
  ON a.CompanyCode = b.CompanyCode    
  and a.BranchCode = b.BranchCode    
  and a.ReturnNo = b.ReturnNo    
  join spTrnSLmpDtl c    
  on c.CompanyCode = b.CompanyCode    
  and c.BranchCode = b.BranchCode    
  and c.LmpNo = b.DocNo    
  and c.WarehouseCode = a.WarehouseCode  
  and c.PartNo = a.PartNo  
  and c.PartNoOriginal = a.PartNoOriginal  
  where a.CompanyCode = @CompanyCode    
  and a.BranchCode = @BranchCode    
  and a.ReturnNo = @ReturnNo