
/****** Object:  StoredProcedure [dbo].[uspfn_spGetPickingListDtl]    Script Date: 6/19/2014 11:47:07 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[uspfn_spGetPickingListDtl] @CompanyCode varchar(15), @BranchCode varchar(15), @PickingSlipNo varchar(25)
as                            
SELECT 
 row_number () OVER (ORDER BY spTrnSPickingDtl.CreatedDate ASC) AS NoUrut,
 spTrnSPickingDtl.DocNo,
 spTrnSPickingDtl.PartNo,
 spTrnSPickingDtl.PartNoOriginal,
 ExPickingSlipNo,
 QtyOrder AS QtyOrder,
 spTrnSPickingDtl.QtySupply AS QtyPick,
 spTrnSPickingDtl.QtyPicked AS QtyPicked,
 spTrnSPickingDtl.QtyBill AS QtyBill,
 spTrnSOSupply.SupSeq,
 spTrnSOSupply.PTSeq
FROM spTrnSPickingDtl with (nolock, nowait)
LEFT JOIN spTrnSOSupply ON spTrnSOSupply.PickingSlipNo = spTrnSPickingDtl.PickingSlipNo AND 
spTrnSOSupply.CompanyCode= spTrnSPickingDtl.CompanyCode AND 
spTrnSOSupply.BranchCode = spTrnSPickingDtl.BranchCode AND 
spTrnSOSupply.DocNo = spTrnSPickingDtl.DocNo AND
spTrnSOSupply.PartNo = spTrnSPickingDtl.PartNo AND
spTrnSOSupply.PartNoOriginal = spTrnSPickingDtl.PartNoOriginal
WHERE spTrnSPickingDtl.CompanyCode= @CompanyCode AND 
spTrnSPickingDtl.BranchCode= @BranchCode AND 
spTrnSPickingDtl.PickingSlipNo = @PickingSlipNo