create procedure uspfn_spGetMaxQtyReturSupplySlip @CompanyCode varchar(15), @BranchCode varchar(15), @PartNo varchar(25),  
@PartNoOriginal varchar(25), @DocNo varchar(20), @LmpNo varchar(25), @ReturnNo varchar(25)  
as  
            SELECT   
            ISNULL((SELECT QtyBill FROM spTrnSLmpDtl WHERE LmpNo = @LmpNo AND   
                PartNo = @PartNo AND   
                PartNoOriginal = @PartNoOriginal AND      
                CompanyCode = @CompanyCode AND  
                BranchCode = @BranchCode  
             ) -  
            ISNULL(SUM (QtyReturn),0), 0) AS MaxQtyRetur FROM spTrnSRturSSdtl  
            LEFT JOIN spTrnSRturSSHdr ON spTrnSRturSSHdr.ReturnNo = spTrnSRturSSdtl.ReturnNo AND  
                spTrnSRturSSHdr.CompanyCode = spTrnSRturSSdtl.CompanyCode AND  
                spTrnSRturSSHdr.BranchCode = spTrnSRturSSdtl.BranchCode  
            WHERE spTrnSRturSSdtl.CompanyCode = @CompanyCode  
                AND spTrnSRturSSdtl.BranchCode = @BranchCode  
                --AND spTrnSRturSSHdr.DocNo = @DocNo  
                AND spTrnSRturSSdtl.PartNo = @PartNo  
                AND spTrnSRturSSdtl.PartNoOriginal = @PartNoOriginal  
    AND spTrnSRturSSDtl.DocNo = @DocNo  
                AND spTrnSRturSSHdr.Status NOT IN ('3')  
                AND spTrnSRturSSHdr.ReturnNo <> @ReturnNo