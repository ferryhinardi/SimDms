CREATE procedure uspfn_spGetNoPartsReturnSS @CompanyCode varchar(15), @BranchCode varchar(15),         
@ProductType varchar(5), @TypeOfGoods varchar(2), @DocNo varchar(25)        
as        
select distinct a.CompanyCode, a.BranchCode, a.DocNo, a.WarehouseCode, a.PartNo, a.PartNoOriginal, a.QtyBill as QtyLmp, a.DocNo,    
    c.LmpNo, c.LmpDate        
                from spTrnSLmpDtl a, spTrnSORDHdr b, spTrnSLmpHdr c with(nolock, nowait)        
                where a.CompanyCode = @CompanyCode        
                        and a.BranchCode = @BranchCode        
                        and b.CompanyCode = @CompanyCode        
                  and b.BranchCode = @BranchCode        
      and a.DocNo = b.DocNo         
      and b.Salestype IN ('2', '3')        
                  and c.CompanyCode = @CompanyCode        
                  and c.BranchCode = @BranchCode        
                        and a.LmpNo = c.LmpNo        
                        and a.LmpNo = @DocNo        
      and ProductType = @ProductType        
      and c.TypeOfGoods = @TypeOfGoods        
                        and (a.QtyBill -         
(select ISNULL(SUM (QtyReturn),0) AS MaxQtyRetur FROM spTrnSRturSSdtl        
            LEFT JOIN spTrnSRturSSHdr ON spTrnSRturSSHdr.ReturnNo = spTrnSRturSSdtl.ReturnNo AND        
                spTrnSRturSSHdr.CompanyCode = spTrnSRturSSdtl.CompanyCode AND        
                spTrnSRturSSHdr.BranchCode = spTrnSRturSSdtl.BranchCode        
            WHERE spTrnSRturSSdtl.CompanyCode = @CompanyCode        
                AND spTrnSRturSSdtl.BranchCode = @BranchCode        
                AND spTrnSRturSSHdr.DocNo = a.DocNo        
                AND spTrnSRturSSdtl.PartNo = a.PartNo         
                AND spTrnSRturSSdtl.PartNoOriginal = a.PartNoOriginal         
    AND spTrnSRturSSDtl.DocNo = a.DocNo         
                AND spTrnSRturSSHdr.Status = 2        
    AND ProductType = @ProductType        
    AND TypeOfGoods = @TypeOfGoods)) > 0  