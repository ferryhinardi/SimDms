create procedure uspfn_spGetTotSalesQty @CompanyCode varchar(15), @BranchCode varchar(15), @PickingSlipNo varchar(25)
as
SELECT SUM(QtyBill)
FROM spTrnSPickingDtl WITH (nolock, nowait)
WHERE CompanyCode = @CompanyCode
  AND BranchCode = @BranchCode
  AND PickingSlipNo = @PickingSlipNo