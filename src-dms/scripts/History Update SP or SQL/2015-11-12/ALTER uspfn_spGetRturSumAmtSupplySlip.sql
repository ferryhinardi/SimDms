ALTER procedure [dbo].[uspfn_spGetRturSumAmtSupplySlip] @CompanyCode varchar(15), @BranchCode varchar(15), @ReturnNo varchar(15)  
as  
SELECT ReturnNo, isnull(sum(QtyReturn),0) as TotReturQty, isnull(sum(ReturAmt),0) as TotReturAmt,  
                        isnull(sum(DiscAmt),0) as TotDiscAmt, isnull(sum(NetReturAmt),0) as TotDPPAmt,  
                        isnull(sum(PPNAmt),0) as TotPPNAmt, isnull(sum(TotReturAmt),0) as TotFinalReturAmt,  
                        isnull(sum(CostAmt),0) as TotCostAmt   
                FROM spTrnSRturSSDtl with (nolock, nowait)  
                WHERE CompanyCode = @CompanyCode  
                        and BranchCode = @BranchCode  
                        and ReturnNo = @ReturnNo  
                GROUP BY ReturnNo
