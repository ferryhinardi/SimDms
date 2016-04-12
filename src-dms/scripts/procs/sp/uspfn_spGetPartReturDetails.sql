CREATE procedure uspfn_spGetPartReturDetails @CompanyCode varchar(15), @BranchCode varchar(15), @FPJNo varchar(25),
@TypeOfGoods varchar(3), @ProductType varchar(3)
as
select distinct a.PartNo, a.PartNoOriginal, a.QtyBill, a.DocNo
from	spTrnSFPJDtl a, spTrnSFPJHdr c with(nolock, nowait)
where	a.CompanyCode = @CompanyCode
        and a.BranchCode = @BranchCode
        and c.CompanyCode = @CompanyCode
        and c.BranchCode = @BranchCode
        and a.FPJNo = c.FPJNo
        and a.FPJNo = @FPJNo
		and TypeOfGoods = @TypeOfGoods
		and ProductType = @ProductType
        and (a.QtyBill - 
(select ISNULL(SUM (QtyReturn),0) AS MaxQtyRetur FROM spTrnSRturdtl
LEFT JOIN spTrnSRturHdr ON spTrnSRturHdr.ReturnNo = spTrnSRturdtl.ReturnNo AND
spTrnSRturHdr.CompanyCode = spTrnSRturdtl.CompanyCode AND
spTrnSRturHdr.BranchCode = spTrnSRturdtl.BranchCode
WHERE spTrnSRturdtl.CompanyCode = @CompanyCode
AND spTrnSRturdtl.BranchCode = @BranchCode
AND spTrnSRturHdr.FPJNo = a.FPJNo --''FPJ/08/000003''
AND spTrnSRturdtl.PartNo = a.PartNo --''P002''
AND spTrnSRturdtl.PartNoOriginal = a.PartNoOriginal -- ''Y-001''
AND spTrnSRturDtl.DocNo = a.DocNo --''SOC/08/000095''
AND TypeOfGoods = @TypeOfGoods
AND ProductType = @ProductType
AND spTrnSRturHdr.Status = 2)) > 0