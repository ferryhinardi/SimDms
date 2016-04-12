if object_id('usprpt_SpRpRgs002_web1') is not null
       drop procedure usprpt_SpRpRgs002_web1
GO

-- =============================================
-- Author:		fhy
-- Create date: 27102015
-- Description: sp register outstanding Supply slip new
-- =============================================
Create PROCEDURE [dbo].[usprpt_SpRpRgs002_web1]
		@CompanyCode	VARCHAR(15),
		@BranchCodeFrom	VARCHAR(15),
		@BranchCodeTo	VARCHAR(15),
		@StartDate		VARCHAR(30),
		@EndDate		VARCHAR(30),
		@TransType		VARCHAR(15),
		@TipePart		VARCHAR(15),
		@IsHolding		BIT =0
AS
BEGIN
	declare
	@CompanyCodeTemp	VARCHAR(15),
	@BranchCodeFromTemp	VARCHAR(15),
	@BranchCodeToTemp	VARCHAR(15),
	@dbNameTemp	VARCHAR(25)='',
	@qryTemp varchar(max),
	@flag1 int,
	@flag2 int,
	@flag3 varchar(5)	

	set @flag3 = convert(varchar,@IsHolding)

	set @flag1 = (select count(dbName) from gnMstCompanyMapping where CompanyCode=@CompanyCode and BranchCode=@BranchCodeFrom)

	--select  @flag1,@flag3

	if(@flag1=0)
	begin
		 set @CompanyCodeTemp=(select CompanyCode from gnMstCompanyMapping where CompanyMD=@CompanyCode and BranchMD=@BranchCodeFrom)
		 set @BranchCodeFromTemp=(select BranchCode from gnMstCompanyMapping where CompanyMD=@CompanyCode and BranchMD=@BranchCodeFrom)
		 set @BranchCodeToTemp=(select BranchCode from gnMstCompanyMapping where CompanyMD=@CompanyCode and BranchMD=@BranchCodeTo)
		 set @dbNameTemp =(select dbname from gnMstCompanyMapping where CompanyMD=@CompanyCode and BranchMD=@BranchCodeFrom)
	end
	else
	begin
		 set @CompanyCodeTemp=(select CompanyCode from gnMstCompanyMapping where CompanyCode=@CompanyCode and BranchCode=@BranchCodeFrom)
		 set @BranchCodeFromTemp=(select BranchCode from gnMstCompanyMapping where CompanyCode=@CompanyCode and BranchCode=@BranchCodeFrom)
		 set @BranchCodeToTemp=(select BranchCode from gnMstCompanyMapping where CompanyCode=@CompanyCode and BranchCode=@BranchCodeTo)
		 set @dbNameTemp =(select dbname from gnMstCompanyMapping where CompanyCode=@CompanyCode and BranchCode=@BranchCodeFrom)
	end

	--select @CompanyCodeTemp,@BranchCodeFromTemp,@BranchCodeToTemp,@dbNameTemp

	set @qryTemp='SELECT 
		a.DocNo,
		a.DocDate,
		a.UsageDocNo,
		a.UsageDocDate,
		(SELECT CustomerName FROM '+@dbNameTemp+'..gnMstCustomer WHERE CompanyCode = a.CompanyCode
		AND CustomerCode = a.CustomerCode) as CustomerName,
		''SELURUH JENIS TRANSAKSI'' as TransTypeDescHeader,
		b.PartNo,
		(SELECT PartNAme FROM '+@dbNameTemp+'..spMstItemInfo WHERE CompanyCode = a.CompanyCode
		AND PartNo = b.PartNo) as PartName,
		(SELECT LookUpValueName FROM '+@dbNameTemp+'..gnMstLookUpDtl WHERE CompanyCode = a.CompanyCode
		AND CodeID = ''TTSR'' AND LookUpValue = a.TransType) as TransTypeDesc, 
		(e.SupplyQty - e.ReturnQty) as QtyOrder,
		b.RetailPrice,
		b.DiscAmt,
		b.SalesAmt HargaKotor,
		f.TotPPNAmt,
		f.TotFinalSalesAmt as HargaSS,
		(b1.CostPrice * b1.QtyBill) AS HargaPokok,
		a.BranchCode		
		,'+@flag3+' IsHolding
	FROM
		'+@dbNameTemp+'..spTrnSORDHdr a
	INNER JOIN '+@dbNameTemp+'..spTrnSORDDtl b ON 
		b.CompanyCode = a.CompanyCode AND 
		b.BranchCode = a.BranchCode AND 
		b.DocNo = a.DocNo
	LEFT JOIN '+@dbNameTemp+'..spTrnSLmpDtl b1 ON 
		b.CompanyCode = b1.CompanyCode AND 
		b.BranchCode = b1.BranchCode AND 
		b.DocNo = b1.DocNo AND 
		b.PartNo = b1.PartNo	
	INNER JOIN '+@dbNameTemp+'..spTrnSLmpHdr f ON
		f.CompanyCode = a.CompanyCode AND
		f.BranchCode = a.BranchCode AND
		f.LmpNo = b1.LmpNo
	LEFT JOIN 
	(
		SELECT c.CompanyCode,c.BranchCode,d.SupplySlipNo as DocNo,c.JobOrderNo as UsageDocNo,d.PartNo,ISNULL(c.ServiceStatus,''0'') as ServiceStatus,
		SUM(ISNULL(DemandQty,0)) as DemandQty,
		SUM(ISNULL(SupplyQty,0)) as SupplyQty,
		SUM(ISNULL(ReturnQty,0)) as ReturnQty 
		FROM '+@dbNameTemp+'..svTrnService c
		inner join '+@dbNameTemp+'..svTrnSrvItem d on c.CompanyCode=d.CompanyCode
		and c.BranchCode=d.BranchCode
		and c.ProductType=d.ProductType
		and c.ServiceNo=d.ServiceNo
		and d.SupplyQty>0
		GROUP BY c.CompanyCode,c.BranchCode,d.SupplySlipNo,c.JobOrderNo,d.PartNo,c.ServiceStatus
	) e ON e.CompanyCode=a.CompanyCode and e.BranchCode=a.BranchCode and e.DocNo=a.DocNo
	and e.UsageDocNo=a.UsageDocNo and e.PartNo=b.PartNo
	WHERE
		a.CompanyCode = '''+@CompanyCodeTemp+''' AND
		(case when '''+@BranchCodeFromTemp+''' = '''' then '''' else a.BranchCode end) between '''+@BranchCodeFromTemp+''' and '''+@BranchCodeToTemp+''' and
		convert(varchar, convert(datetime, (CASE WHEN '''+@StartDate+''' = '''' THEN '''' ELSE a.DocDate END)), 112) BETWEEN convert(varchar, convert(datetime, '''+@StartDate+'''), 112) 
			AND convert(varchar, convert(datetime, '''+@EndDate+'''), 112) AND 
		(CASE WHEN '''+@TransType+''' = ''ALL'' THEN '''+@TransType+''' ELSE a.TransType END) = '''+@TransType+''' AND
		a.SalesType = ''2'' AND
		e.ServiceStatus < ''6'' AND
		a.TypeOfGoods = (CASE WHEN '''+@TipePart+''' = '''' THEN a.TypeOfGoods ELSE '''+@TipePart+''' END) 
	ORDER BY
		a.BranchCode,
		a.DocNo'

	exec (@qryTemp)
	print(@qryTemp)
END

GO


