create procedure [dbo].[uspfn_RepostingCancelInvoice]
  @CompanyCode  varchar(15),
  @BranchCode   varchar(15),
  @InvoiceNo	varchar(15)
as
begin

declare @CustomerCode as varchar(15)
declare @OldTotSrvAmt as decimal

set @CustomerCode = (select CustomerCodeBill from svTrnInvoice where CompanyCode = @CompanyCode and BranchCode = @BranchCode and InvoiceNo = @InvoiceNo)
set @OldTotSrvAmt = (select TotalSrvAmt from svTrnInvoice where CompanyCode = @CompanyCode and BranchCode = @BranchCode and InvoiceNo = @InvoiceNo)

delete from glInterface
 where ProfitCenterCode = '200'
   and StatusFlag = 0
   and CompanyCode = @CompanyCode
   and BranchCode = @BranchCode
   and DocNo = @InvoiceNo

delete from arInterface
 where ProfitCenterCode = '200'
   and StatusFlag = 0
   and CompanyCode = @CompanyCode
   and BranchCode = @BranchCode
   and DocNo = @InvoiceNo

update gnTrnBankBook set SalesAmt = SalesAmt - @OldTotSrvAmt
 where ProfitCenterCode = '200'
	and CompanyCode = @CompanyCode
	and BranchCode = @BranchCode
	and CustomerCode = @CustomerCode

end	