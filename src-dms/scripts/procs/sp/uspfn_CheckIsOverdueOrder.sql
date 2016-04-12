CREATE procedure uspfn_CheckIsOverdueOrder @CompanyCode nvarchar(15),
@BranchCode nvarchar(15),@CustomerCode nvarchar(15),
@DueDate datetime,@ProfitCenterCode nvarchar(3)
as
SELECT top 1 * 
                FROM arinterface a with(nolock, nowait)
                INNER JOIN spTrnSORDHdr b with(nolock, nowait) on a.CustomerCode = b.CustomerCode
                                        AND a.CompanyCode = b.CompanyCode
                                        AND a.BranchCode = b.BranchCode
		        WHERE b.CompanyCode = @CompanyCode
                AND b.BranchCode = @BranchCode	
                AND b.CustomerCode = @CustomerCode
                AND a.ProfitCenterCode = @ProfitCenterCode
                AND Convert(varchar, a.DueDate,112) <= convert(varchar,@DueDate,112)
                AND NettAmt > (ReceiveAmt+CreditAmt-DebetAmt)