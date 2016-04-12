create procedure uspfn_spGetRturSumAmt @CompanyCode varchar(15), @BranchCode varchar(15), @ReturnNo varchar(20)
as  
 SELECT	ReturnNo, sum(QtyReturn) as TotReturQty, sum(ReturAmt) as TotReturAmt,
                        sum(DiscAmt) as TotDiscAmt, sum(NetReturAmt) as TotDPPAmt,
                        sum(PPNAmt) as TotPPNAmt, sum(TotReturAmt) as TotFinalReturAmt,
                        sum(CostAmt) as TotCostAmt 
                FROM	spTrnSRturDtl with (nolock, nowait)
                WHERE	CompanyCode = @CompanyCode
                        and BranchCode = @BranchCode
                        and ReturnNo = @ReturnNo
                GROUP BY ReturnNo
