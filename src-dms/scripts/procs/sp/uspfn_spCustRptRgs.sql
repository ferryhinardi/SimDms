SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		David L.
-- Create date: 22 Sep 2014
-- Description:	Get Customer For Report Register 011 & 017B
--              (Report Register Penjualan Per Pelanggan (Tunai&Kredit))
-- =============================================
ALTER PROCEDURE [dbo].[uspfn_spCustRptRgs] 
	@CompanyCode varchar(15),
	@BranchCode varchar(15),
	@DateFrom datetime,
	@DateTo datetime,
	@PartType varchar(2),
	@PaymentCode nchar(1)
AS
BEGIN
	declare @TOPC varchar(15)
	set @TOPC = 'TOPC'

	if(@PaymentCode = '0')
	begin	
	SELECT DISTINCT
			    a.CustomerCode,
			    UPPER(b.CustomerName) CustomerName
		      FROM 
			    SpTrnSFPJHdr a,
			    GnMstCustomer b,
			    (
				    SELECT 
					    row_number()OVER(ORDER BY SpTrnSFPJHdr.customerCode)rowNum,
					    SpTrnSFPJHdr.customerCode 
				    FROM 
					    SpTrnSFPJHdr 
					    left join gnMstLookupDtl on SpTrnSFPJHdr.CompanyCode = gnMstLookupDtl.CompanyCode
						    AND gnMstLookupDtl.CodeId = @TOPC
				    WHERE
					    SpTrnSFPJHdr.CompanyCode	= @CompanyCode
					    AND SpTrnSFPJHdr.BranchCode = @BranchCode
					    AND convert(varchar,SpTrnSFPJHdr.FPJDate, 112) 
						    BETWEEN convert(varchar, convert(datetime, @DateFrom), 112) 
						    AND convert(varchar, convert(datetime, @DateTo), 112)
					    AND gnMstLookupDtl.ParaValue = 0
				    GROUP BY customerCode
			    )c,
			    gnMstLookupDtl d	
		     WHERE 
			    a.CompanyCode		= b.CompanyCode
			    AND a.CustomerCode	= b.CustomerCode
			    AND a.CustomerCode	= c.CustomerCode
			    AND a.CompanyCode	= @CompanyCode
			    AND a.BranchCode	= @BranchCode
			    AND convert(varchar, a.FPJDate, 112) 
					    BETWEEN convert(varchar, convert(datetime, @DateFrom), 112) 
					    AND convert(varchar, convert(datetime, @DateTo), 112)
			    AND a.CompanyCode	= d.CompanyCode
			    AND d.CodeId		= @TOPC
			    AND a.TOPCode		= d.LookupValue	
			    AND d.ParaValue     = 0 			 		
                AND (CASE WHEN @PartType = '' THEN '' ELSE a.TypeOfGoods END) = @PartType
		     ORDER BY CustomerCode
	end
	else if(@PaymentCode = '1')
	begin 
	SELECT DISTINCT
			    a.CustomerCode,
			    UPPER(b.CustomerName) CustomerName
		      FROM 
			    SpTrnSFPJHdr a,
			    GnMstCustomer b,
			    (
				    SELECT 
					    row_number()OVER(ORDER BY SpTrnSFPJHdr.customerCode)rowNum,
					    SpTrnSFPJHdr.customerCode 
				    FROM 
					    SpTrnSFPJHdr 
					    left join gnMstLookupDtl on SpTrnSFPJHdr.CompanyCode = gnMstLookupDtl.CompanyCode
						    AND gnMstLookupDtl.CodeId = @TOPC
				    WHERE
					    SpTrnSFPJHdr.CompanyCode	= @CompanyCode
					    AND SpTrnSFPJHdr.BranchCode = @BranchCode
					    AND convert(varchar,SpTrnSFPJHdr.FPJDate, 112) 
						    BETWEEN convert(varchar, convert(datetime, @DateFrom), 112) 
						    AND convert(varchar, convert(datetime, @DateTo), 112)
					    AND gnMstLookupDtl.ParaValue = 0
				    GROUP BY customerCode
			    )c,
			    gnMstLookupDtl d	
		     WHERE 
			    a.CompanyCode		= b.CompanyCode
			    AND a.CustomerCode	= b.CustomerCode
			    AND a.CustomerCode	= c.CustomerCode
			    AND a.CompanyCode	= @CompanyCode
			    AND a.BranchCode	= @BranchCode
			    AND convert(varchar, a.FPJDate, 112) 
					    BETWEEN convert(varchar, convert(datetime, @DateFrom), 112) 
					    AND convert(varchar, convert(datetime, @DateTo), 112)
			    AND a.CompanyCode	= d.CompanyCode
			    AND d.CodeId		= @TOPC
			    AND a.TOPCode		= d.LookupValue	
			    AND d.ParaValue     > 0 			 		
                AND (CASE WHEN @PartType = '' THEN '' ELSE a.TypeOfGoods END) = @PartType
		     ORDER BY CustomerCode
	end
	else
	begin
	SELECT DISTINCT
			    a.CustomerCode,
			    UPPER(b.CustomerName) CustomerName
		      FROM 
			    SpTrnSFPJHdr a,
			    GnMstCustomer b,
			    (
				    SELECT 
					    row_number()OVER(ORDER BY SpTrnSFPJHdr.customerCode)rowNum,
					    SpTrnSFPJHdr.customerCode 
				    FROM 
					    SpTrnSFPJHdr 
					    left join gnMstLookupDtl on SpTrnSFPJHdr.CompanyCode = gnMstLookupDtl.CompanyCode
						    AND gnMstLookupDtl.CodeId = @TOPC
				    WHERE
					    SpTrnSFPJHdr.CompanyCode	= @CompanyCode
					    AND SpTrnSFPJHdr.BranchCode = @BranchCode
					    AND convert(varchar,SpTrnSFPJHdr.FPJDate, 112) 
						    BETWEEN convert(varchar, convert(datetime, @DateFrom), 112) 
						    AND convert(varchar, convert(datetime, @DateTo), 112)
					    AND gnMstLookupDtl.ParaValue = 0
				    GROUP BY customerCode
			    )c,
			    gnMstLookupDtl d	
		     WHERE 
			    a.CompanyCode		= b.CompanyCode
			    AND a.CustomerCode	= b.CustomerCode
			    AND a.CustomerCode	= c.CustomerCode
			    AND a.CompanyCode	= @CompanyCode
			    AND a.BranchCode	= @BranchCode
			    AND convert(varchar, a.FPJDate, 112) 
					    BETWEEN convert(varchar, convert(datetime, @DateFrom), 112) 
					    AND convert(varchar, convert(datetime, @DateTo), 112)
			    AND a.CompanyCode	= d.CompanyCode
			    AND d.CodeId		= @TOPC
			    AND a.TOPCode		= d.LookupValue	
                AND (CASE WHEN @PartType = '' THEN '' ELSE a.TypeOfGoods END) = @PartType
		     ORDER BY CustomerCode
	end
END
