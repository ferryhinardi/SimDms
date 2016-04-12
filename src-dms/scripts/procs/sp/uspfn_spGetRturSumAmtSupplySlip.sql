
/****** Object:  StoredProcedure [dbo].[uspfn_spGetRturSumAmtSupplySlip]    Script Date: 6/19/2014 11:15:08 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[uspfn_spGetRturSumAmtSupplySlip] @CompanyCode varchar(15), @BranchCode varchar(15), @ReturnNo varchar(15)  
as  
SELECT ReturnNo, sum(QtyReturn) as TotReturQty, sum(ReturAmt) as TotReturAmt,  
                        sum(DiscAmt) as TotDiscAmt, sum(NetReturAmt) as TotDPPAmt,  
                        sum(PPNAmt) as TotPPNAmt, sum(TotReturAmt) as TotFinalReturAmt,  
                        sum(CostAmt) as TotCostAmt   
                FROM spTrnSRturSSDtl with (nolock, nowait)  
                WHERE CompanyCode = @CompanyCode  
                        and BranchCode = @BranchCode  
                        and ReturnNo = @ReturnNo  
                GROUP BY ReturnNo  