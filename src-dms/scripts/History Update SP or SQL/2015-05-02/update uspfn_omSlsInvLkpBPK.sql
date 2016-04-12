ALTER procedure [dbo].[uspfn_omSlsInvLkpBPK]   
(  
 --declare
 @CompanyCode varchar(15),  
 @BranchCode varchar(15),
 @SONo varchar(15),
 @INVDate datetime
)  
AS  
BEGIN  

SELECT distinct a.BPKDate, a.BPKNo,a.BPKDate,a.DONo,a.SONo
                  FROM omTrSalesBPK a inner join omTrSalesBPKDetail b
	                on a.companyCode = b.companyCode and a.branchCode = b.branchCode and a.BPKNo = b.BPKNo
                 WHERE a.CompanyCode = @CompanyCode
                       AND a.BranchCode = @BranchCode
                       AND MONTH(a.BPKDate) = MONTH(@INVDate)
                        AND CONVERT(varchar, a.BPKDate, 110) <= CONVERT(varchar,@INVDate, 110)
                        AND b.StatusInvoice = '0'
                        AND a.Status = '2'
                        AND SONO = @SONo
                ORDER BY a.BPKNo
END      


