CREATE procedure uspfn_spGetSpTrnSFPJDtl @CompanyCode varchar(15), @BranchCode varchar(15),@FPJNo varchar(15)    
as    
select a.PartNo, a.DocNo, a.QtyBill, a.SalesAmt, a.DiscPct, a.WarehouseCode, b.PartName, a.PartNoOriginal, a.DocNo from  spTrnSFPJDtl a    
left JOIN SpMstItemInfo b    
ON a.CompanyCode = b.CompanyCode and a.PartNo = b.PartNo    
where a.CompanyCode = @CompanyCode    
and a.BranchCode = @BranchCode    
and a.FPJNo = @FPJNo  
  
  
