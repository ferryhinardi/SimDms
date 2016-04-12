         alter procedure [dbo].[uspfn_spTrnPOrderBalancePart] (  @CompanyCode varchar(15) ,  @BranchCode varchar(15), @PosNo varchar(35) )
 as
    
			    SELECT 
                    a.POSNo, a.PartNo, b.PartName, CAST(a.OrderQty as decimal(18,2)) as OrderQty, 
                    a.OnOrder, a.Intransit, a.Received,a.DiscPct, a.PurchasePrice, 
                    Convert(varchar(10),a.SeqNo) SeqNo, a.SupplierCode, a.OnOrder, a.PartNoOriginal, 
                    a.TypeOfGoods ,a.CompanyCode ,a.BranchCode
                FROM 
                    spTrnPOrderBalance a 
                INNER JOIN spMstItemInfo b
                   ON b.PartNo      = a.PartNo
                  AND b.CompanyCode = a.CompanyCode
                WHERE a.CompanyCode = @CompanyCode
                  AND a.BranchCode  = @BranchCode
                  AND a.PosNo    like @PosNo
                ORDER BY a.POSNo DESC, a.SeqNo