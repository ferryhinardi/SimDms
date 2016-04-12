CREATE PROCEDURe uspfn_spGetTotDiscAmt @CompanyCode varchar(15), @BranchCode varchar(15),
@PickingSlipNo varchar(25)
as
SELECT SUM(DiscAmt)
FROM spTrnSPickingDtl WITH (nolock, nowait)
WHERE CompanyCode = @CompanyCode
  AND BranchCode = @BranchCode
  AND PickingSlipNo = @PickingSlipNo
  
