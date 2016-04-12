ALTER procedure [dbo].[uspfn_omSlsInvLkpBPK]   
--declare
 @CompanyCode varchar(15),  
 @BranchCode varchar(15),
 @SONo varchar(15),
 @INVDate datetime
AS  
BEGIN  
 --exec [uspfn_omSlsInvLkpBPK] '6115204001','6115204502','SO507/15/000153','20150522'
 --select @CompanyCode= '6115204001',
	--	@BranchCode= '6115204507',
	--	@SONo= 'SO507/15/000153',
	--	@INVDate= '20150522'
 
SELECT distinct a.BPKNo,a.BPKDate,a.DONo,a.SONo,a.BPKDate
                  FROM omTrSalesBPK a inner join omTrSalesBPKDetail b
	                on a.companyCode = b.companyCode and a.branchCode = b.branchCode and a.BPKNo = b.BPKNo
                 WHERE a.CompanyCode = @CompanyCode
                       AND a.BranchCode = @BranchCode
                       --AND MONTH(a.BPKDate) = @INVDate
                        AND CONVERT(varchar,a.BPKDate,112) <= CONVERT(varchar, @INVDate,112)
                        AND b.StatusInvoice = '0'
                        AND a.Status = '2'
                        AND SONO = @SONo
                ORDER BY a.BPKNo
END
