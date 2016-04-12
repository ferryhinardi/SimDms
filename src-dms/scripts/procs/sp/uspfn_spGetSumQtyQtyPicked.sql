create procedure uspfn_spGetSumQtyQtyPicked @CompanyCode varchar(15), @BranchCode varchar(15), @PickingSlipNo varchar(25)
as
select sum(QtyPicked) as QtyPicked 
            from spTrnSPickingDtl with (nolock, nowait)
            where 
                CompanyCode = @CompanyCode and
                BranchCode = @BranchCode and
                PickingSlipNo = @PickingSlipNo 