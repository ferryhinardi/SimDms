 CREATE procedure [dbo].[uspfn_TaxRecalculatePPN]
	@CompanyCode nvarchar(25),
	@BranchCode nvarchar(25),
	@ProductType nvarchar(2),
	@PeriodYear int,
	@PeriodMonth int,
	@ProfitCenter nvarchar(15),
	@DocumentType nvarchar(15)

  as
  begin

update gnTaxPPN
set DPPStd=	(
            select 
                isnull(sum(DPPAmt),0) 
            from 
                gnTaxIn
            where
                CompanyCode= @CompanyCode and BranchCode= @BranchCode
                and ProductType= @ProductType and PeriodYear= @PeriodYear
                and PeriodMonth= @PeriodMonth and ProfitCenter= @ProfitCenter
                and DocumentType= @DocumentType and IsPKP= 1
        )
,DPPSdh= (
            select 
                isnull(sum(DPPAmt),0) 
            from 
                gnTaxIn
            where
                CompanyCode= @CompanyCode and BranchCode= @BranchCode
                and ProductType= @ProductType and PeriodYear= @PeriodYear
                and PeriodMonth= @PeriodMonth and ProfitCenter= @ProfitCenter
                and DocumentType= @DocumentType and IsPKP= 0
        )
,PPNStd= (
            select 
                isnull(sum(PPNAmt),0) 
            from 
                gnTaxIn
            where
                CompanyCode= @CompanyCode and BranchCode= @BranchCode
                and ProductType= @ProductType and PeriodYear= @PeriodYear
                and PeriodMonth= @PeriodMonth and ProfitCenter= @ProfitCenter
                and DocumentType= @DocumentType and IsPKP= 1
        )
,PPNSdh= (
            select 
                isnull(sum(PPNAmt),0) 
            from 
                gnTaxIn
            where
                CompanyCode= @CompanyCode and BranchCode= @BranchCode
                and ProductType= @ProductType and PeriodYear= @PeriodYear
                and PeriodMonth= @PeriodMonth and ProfitCenter= @ProfitCenter
                and DocumentType= @DocumentType and IsPKP= 0
        )
where
CompanyCode= @CompanyCode and BranchCode= @BranchCode
and ProductType= @ProductType and PeriodYear= @PeriodYear
and PeriodMonth= @PeriodMonth and ProfitCenter= @ProfitCenter
and TaxType= case when @DocumentType= 'F' then '3' else '4' end
end