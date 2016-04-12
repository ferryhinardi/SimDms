
/****** Object:  StoredProcedure [dbo].[uspfn_SppartView]    Script Date: 6/19/2014 10:42:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 create procedure [dbo].[uspfn_SppartView] (  @CompanyCode varchar(10) ,@BranchCode varchar(10),@PosNo varchar(20))
   as
 SELECT 
                    a.POSNo, a.PartNo, b.PartName, CAST(a.OrderQty as decimal(18,2)) as OrderQty, 
                    a.OnOrder, a.Intransit, a.Received,a.DiscPct, a.PurchasePrice, 
                    Convert(varchar(10),a.SeqNo) SeqNo, a.SupplierCode, a.OnOrder, a.PartNoOriginal, 
                    a.TypeOfGoods 
                FROM 
                    spTrnPOrderBalance a 
                INNER JOIN spMstItemInfo b
                   ON b.PartNo      = a.PartNo
                  AND b.CompanyCode = a.CompanyCode
                WHERE a.CompanyCode = @CompanyCode
                  AND a.BranchCode  = @BranchCode
                  AND a.PosNo    like @PosNo
                ORDER BY a.POSNo DESC, a.SeqNo