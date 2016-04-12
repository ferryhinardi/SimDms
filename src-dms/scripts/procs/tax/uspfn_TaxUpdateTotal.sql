 CREATE procedure [dbo].[uspfn_TaxUpdateTotal]
	@CompanyCode nvarchar(25),
	@BranchCode nvarchar(25),
	@ProductType nvarchar(2),
	@Period datetime 

  as
  begin

update gnTaxPPN
set PPNStd= (
                isnull((	select	case when PPNStd > 0 then 0 else PPNStd end PPNStd
			                from	gnTaxPPN
			                where	CompanyCode= a.CompanyCode and BranchCode= a.BranchCode
					                and ProductType= a.ProductType
					                and PeriodYear = year(dateadd(month,-1,@Period))
					                and PeriodMonth = month(dateadd(month,-1,@Period)) 
					                and TaxType= '5'),0) +
                isnull((	select	sum(PPNStd)
							from	gnTaxPPN 
							where	CompanyCode= a.CompanyCode and BranchCode= a.BranchCode
					                and ProductType= a.ProductType
					                and PeriodYear = year(@Period)
					                and PeriodMonth = month(@Period) 
									and TaxType in ('1','2')),0)-
                isnull((	select	sum(PPNStd) 
							from	gnTaxPPN 
							where	CompanyCode= a.CompanyCode and BranchCode= a.BranchCode
					                and ProductType= a.ProductType
					                and PeriodYear = year(@Period)
					                and PeriodMonth = month(@Period) 
									and TaxType in ('3','4')),0)
            )
,PPNSdh= (
                isnull((	select	case when PPNSdh > 0 then 0 else PPNSdh end PPNSdh
			                from	gnTaxPPN
			                where	CompanyCode= a.CompanyCode and BranchCode= a.BranchCode
					                and ProductType= a.ProductType
					                and PeriodYear = year(dateadd(month,-1,@Period))
					                and PeriodMonth = month(dateadd(month,-1,@Period)) 
					                and TaxType= '5'),0) +
                isnull((	select	sum(PPNSdh)
							from	gnTaxPPN 
							where	CompanyCode= a.CompanyCode and BranchCode= a.BranchCode
					                and ProductType= a.ProductType
					                and PeriodYear = year(@Period)
					                and PeriodMonth = month(@Period) 
									and TaxType in ('1','2')),0)-
                isnull((	select	sum(PPNSdh) 
							from	gnTaxPPN 
							where	CompanyCode= a.CompanyCode and BranchCode= a.BranchCode
					                and ProductType= a.ProductType
					                and PeriodYear = year(@Period)
					                and PeriodMonth = month(@Period) 
									and TaxType in ('3','4')),0)
            )
from gnTaxPPN a
where
    CompanyCode= @CompanyCode
	AND BranchCode = case @BranchCode when '' then BranchCode else @BranchCode end 
    and ProductType= @ProductType and PeriodYear = year(@Period)
    and PeriodMonth = month(@Period) and TaxType = '5'
	end