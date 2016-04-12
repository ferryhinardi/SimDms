CREATE PROCEDURe uspfn_spGetTotSupplyQty @CompanyCode varchar(15), @BranchCode varchar(15),
@PickingSlipNo varchar(25)
as
SELECT SUM(QtySupply)
FROM spTrnSPickingDtl WITH (nolock, nowait)
WHERE CompanyCode = @CompanyCode
  AND BranchCode = @BranchCode
  AND PickingSlipNo = @PickingSlipNo 
  
  
  
