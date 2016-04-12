CREATE PROCEDURe uspfn_spGetTotDPPAmt @CompanyCode varchar(15), @BranchCode varchar(15),
@PickingSlipNo varchar(25)
as
SELECT SUM(NetSalesAmt)
FROM spTrnSPickingDtl WITH (nolock, nowait)
WHERE CompanyCode = @CompanyCode
  AND BranchCode = @BranchCode
  AND PickingSlipNo = @PickingSlipNo