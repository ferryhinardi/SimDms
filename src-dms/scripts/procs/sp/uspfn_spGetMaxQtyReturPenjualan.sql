create procedure uspfn_spGetMaxQtyReturPenjualan 
@CompanyCode varchar(15), @BranchCode varchar(15), @PartNo varchar(25), 
@PartNoOriginal  varchar(25), @ReturnNo varchar(15), @DocNo varchar(25), @FPJNo varchar(15)
as
SELECT 
            ISNULL((SELECT QtyBill FROM spTrnSFPJDtl WHERE FPJNo = @FPJNo AND 
                PartNo = @PartNo AND 
                PartNoOriginal = @PartNoOriginal AND
				DocNo = @DocNo AND
                CompanyCode = @CompanyCode AND
                BranchCode = @BranchCode
             ) -
            ISNULL(SUM (QtyReturn),0), 0) AS MaxQtyRetur FROM spTrnSRturdtl
            LEFT JOIN spTrnSRturHdr ON spTrnSRturHdr.ReturnNo = spTrnSRturdtl.ReturnNo AND
                spTrnSRturHdr.CompanyCode = spTrnSRturdtl.CompanyCode AND
                spTrnSRturHdr.BranchCode = spTrnSRturdtl.BranchCode
            WHERE spTrnSRturdtl.CompanyCode = @CompanyCode
                AND spTrnSRturdtl.BranchCode = @BranchCode
                AND spTrnSRturHdr.FPJNo = @FPJNo
                AND spTrnSRturdtl.PartNo = @PartNo
                AND spTrnSRturdtl.PartNoOriginal = @PartNoOriginal
				AND spTrnSRturDtl.DocNo = @DocNo
                AND spTrnSRturHdr.Status NOT IN ('3')
                AND spTrnSRturdtl.ReturnNo <> @ReturnNo