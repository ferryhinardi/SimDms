-- =============================================
-- Author:		<yo>
-- Create date: <5 Agust 14>
-- Description:	<Query FPJ Generated>
-- =============================================

create procedure uspfn_TaxQueryFPJGenerated
@CompanyCode varchar(15),
@BranchCode varchar(15),
@StartDate varchar(10),
@FPJDate varchar(10),
@ProfitCenter varchar(10),
@IsFPJCentral bit

--declare @CompanyCode as varchar(15)
--declare @BranchCode as varchar(15)
--declare @StartDate as varchar(10)
--declare @FPJDate as varchar(10)
--declare @ProfitCenter as varchar(10)
--declare @IsFPJCentral as bit

--set @CompanyCode = '6006410'
--set @BranchCode  = '600641001'
--set @StartDate  = '20140501'
--set @FPJDate  = '20140513'
--set @ProfitCenter  = '300'
--set @IsFPJCentral  = '1'

as
begin

select 
    y.No,y.chkSelect,y.CompanyCode,y.BranchCode,y.ProfitCenter,y.FPJGovNo
    ,isnull(y.FPJGovDate,'')FPJGovDate,y.DocNo, convert(varchar, convert(datetime,y.DocDate), 106) DocDate
from(
    select 
        row_number() over(order by DocDate, BranchCode,ProfitCenter asc) as No
        , Convert(bit, 0) chkSelect, x.CompanyCode, x.BranchCode
        , a.LookUpValueName ProfitCenter, x.FPJGovNo, x.FPJGovDate
        , x.DocNo, x.DocDate
    from (
	    SELECT	CompanyCode, BranchCode,'300' AS ProfitCenter,FPJGovNo
			    ,NULL AS FPJGovDate,InvoiceNo as DocNo,convert(varchar,InvoiceDate,112) AS DocDate
	    FROM	SpTrnSFPJHdr
	    WHERE	CompanyCode = @CompanyCode 
                AND ((case when @IsFPJCentral = '1' then BranchCode end) <> ''
			        or (case when @IsFPJCentral = '0' then BranchCode end) in (@BranchCode)
		        )
                AND isPKP = 1 
                AND ISNULL(FPJGovNo, '') = ''
			    AND CONVERT(VARCHAR, InvoiceDate, 112) BETWEEN @StartDate AND @FPJDate
	    UNION
	    SELECT	CompanyCode, BranchCode,'200' AS ProfitCenter,FPJGovNo
			    ,NULL AS FPJGovDate,FPJNo as DocNo,convert(varchar,FPJDate,112) AS DocDate
	    FROM	SvTrnFakturPajak
	    WHERE	CompanyCode = @CompanyCode 
                AND ((case when @IsFPJCentral = '1' then BranchCode end) <> ''
			        or (case when @IsFPJCentral = '0' then BranchCode end) in (@BranchCode)
		        )
                AND isPKP = 1 AND ISNULL(FPJGovNo, '') = ''
			    AND CONVERT(VARCHAR, FPJDate, 112) BETWEEN @StartDate AND @FPJDate
	    UNION
	    SELECT	CompanyCode, BranchCode,'100' AS ProfitCenter,FakturPajakNo FPJGovNo
			    ,NULL AS FPJGovDate,InvoiceNo as DocNo,convert(varchar,InvoiceDate,112) AS DocDate
	    FROM	OmFakturPajakHdr
	    WHERE	CompanyCode = @CompanyCode 
                AND ((case when @IsFPJCentral = '1' then BranchCode end) <> ''
			        or (case when @IsFPJCentral = '0' then BranchCode end) in (@BranchCode)
		        ) 
                AND TaxType = 'Standard' AND ISNULL(FakturPajakNo, '') = ''
			    AND CONVERT(VARCHAR, InvoiceDate, 112) BETWEEN @StartDate AND @FPJDate
	    UNION
	    SELECT	CompanyCode, BranchCode,'000' AS ProfitCenter,FakturPajakNo FPJGovNo
			    ,NULL AS FPJGovDate,InvoiceNo as DocNo,convert(varchar,InvoiceDate,112) AS DocDate
	    FROM	ArFakturPajakHdr
	    WHERE	CompanyCode = @CompanyCode 
                AND ((case when @IsFPJCentral = '1' then BranchCode end) <> ''
			        or (case when @IsFPJCentral = '0' then BranchCode end) in (@BranchCode)
		        )
                AND TaxType = 'Standard' AND ISNULL(FakturPajakNo, '') = ''
			    AND CONVERT(VARCHAR, InvoiceDate, 112) BETWEEN @StartDate AND @FPJDate
                AND DPPAMt > 0
    ) x 
    left join gnMstLookUpDtl a on a.CompanyCode= x.CompanyCode and a.CodeID='PFCN' and a.LookUpValue= x.ProfitCenter
    where x.ProfitCenter like ''+@ProfitCenter+''
) y
end