if object_id('usprpt_OmRpLabaRugi002') is not null
	drop PROCEDURE usprpt_OmRpLabaRugi002
GO
create procedure [dbo].[usprpt_OmRpLabaRugi002] 
--DECLARE    
	@CompanyCode varchar ( 20 ) , 
    @BranchCode varchar ( 20 ) , 
    @DateStart datetime , 
    @DateEnd datetime , 
    @SalesType char ( 1 ) , 
    @SalesFrom varchar ( 20 ) , 
    @SalesTo varchar ( 20 ) , 
    @param char ( 1 ) 

--SELECT @CompanyCode='6115204001',@BranchCode='%',@DateStart='20150501',@DateEnd='20150531',@SalesType='',@SalesFrom='',@SalesTo= '',@param ='1'


AS
BEGIN

select * into #t1
from (
	SELECT 
		a.CompanyCode , 
		a.BranchCode
		,( 
			SELECT CompanyName FROM gnMstCoProfile 
			WHERE CompanyCode = a.CompanyCode 
			AND BranchCode = a.BranchCode 
		) BranchName,
		d.SalesType , 
		g.SalesModelCode + ' - ' + g.SalesModelDesc AS SalesModelCode , 
		count(c.ChassisNo) AS Unit,
		isnull (sum(b.BeforeDiscDPP) , 0 ) AS PenjualanBrutto , 
		isnull (sum(b.AfterDiscDPP) , 0 ) AS penjualan , 
		--isnull (isnull (sum(b.BeforeDiscDPP) , 0 )- isnull (sum(b.DiscExcludePPN ), 0 ), 0 ) AS penjualan , 
		isnull (sum(b.DiscExcludePPN ), 0 ) AS Discount , 
		isnull (sum(c.COGS) , 0 ) AS Biaya ,
		isnull ( (sum(b.AfterDiscDPP) - sum(c.COGS) ) , 0 ) AS LabaRugi 
		,((isnull(sum(b.AfterDiscDPP),0)-isnull(sum(c.COGS),0)) / isnull(sum(b.AfterDiscDPP),0 ))* 100 Percentage
		,'Invoice' TypeTrans
	FROM OmTrSalesInvoice a 
	INNER JOIN OmTrSalesInvoiceModel b ON a.CompanyCode = b.CompanyCode 
		AND a.BranchCode = b.BranchCode 
		AND a.InvoiceNo = b.InvoiceNo 
	INNER JOIN OmTrSalesInvoiceVin c ON c.CompanyCode = a.CompanyCode 
		AND c.BranchCode = a.BranchCode 
		AND c.InvoiceNo = a.InvoiceNo 
		AND c.BPKNo=b.BPKNo
		AND c.SalesModelCode = b.SalesModelCode 
		AND c.SalesModelYear = b.SalesModelYear 
	INNER JOIN OmTrSalesSO d ON d.CompanyCode = a.CompanyCode 
		AND d.BranchCode = a.BranchCode 
		AND d.SONo = a.SONo 
	LEFT JOIN omMstModel g ON g.CompanyCode = a.CompanyCode 
		AND g.SalesModelCode = b.SalesModelCode 
	WHERE a.CompanyCode = @CompanyCode 
		AND a.BranchCode LIKE @BranchCode 
		AND ((case when @param = '0' then a.InvoiceDate end) <> ''
			 or (case when @param = '1' then convert ( varchar , a.InvoiceDate , 112 ) end) 
				BETWEEN convert ( varchar , @DateStart , 112 ) AND convert ( varchar , @DateEnd , 112 ) 
		)
		AND 
		( ( CASE WHEN @SalesType = '' THEN d.SalesType END ) <> '' 
			OR ( CASE WHEN @SalesType <> '' THEN d.SalesType END ) = @SalesType 
		) 
		AND 
		( ( CASE WHEN @SalesFrom = '' THEN d.SalesCode END ) <> '' 
			OR ( CASE WHEN @SalesFrom <> '' THEN d.SalesCode END ) BETWEEN @SalesFrom AND @SalesTo 
		) 
	group by
		a.CompanyCode
		,a.BranchCode
		,d.SalesType
		,g.SalesModelCode + ' - ' + g.SalesModelDesc
) #t1

insert into #t1
SELECT 
		a.CompanyCode , 
		a.BranchCode
		,( 
			SELECT CompanyName FROM gnMstCoProfile 
			WHERE CompanyCode = a.CompanyCode 
			AND BranchCode = a.BranchCode 
		) BranchName,
		d.SalesType , 
		g.SalesModelCode + ' - ' + g.SalesModelDesc AS SalesModelCode , 
		count(c.ChassisNo) * -1 AS Unit,
		isnull (sum(b.BeforeDiscDPP) , 0 ) * -1 AS PenjualanBrutto , 
		--isnull (sum(b.AfterDiscDPP) , 0 ) * -1 AS penjualan ,
		isnull (isnull (sum(b.BeforeDiscDPP) , 0 )- isnull (sum(b.DiscExcludePPN ), 0 ), 0 ) * -1 AS penjualan ,  
		isnull (sum(b.DiscExcludePPN ), 0 ) * -1 AS Discount , 
		isnull (sum(i.COGS) , 0 ) * -1 AS Biaya ,
		isnull ( (sum(b.AfterDiscDPP) - sum(i.COGS) ) , 0 ) * -1 AS LabaRugi 
		,((isnull(sum(b.AfterDiscDPP),0)-isnull(sum(i.COGS),0)) / isnull(sum(b.AfterDiscDPP),0 ))* 100 * -1 Percentage
		,'Retur' TypeTrans
	FROM OmTrSalesReturn a 
	INNER JOIN OmTrSalesReturnDetailModel b ON a.CompanyCode = b.CompanyCode 
		AND a.BranchCode = b.BranchCode 
		AND a.ReturnNo = b.ReturnNo 
	INNER JOIN OmTrSalesReturnVin c ON c.CompanyCode = a.CompanyCode 
		AND c.BranchCode = a.BranchCode 
		AND c.ReturnNo = a.ReturnNo 
		AND c.BPKNo=b.BPKNo
		AND c.SalesModelCode = b.SalesModelCode 
		AND c.SalesModelYear = b.SalesModelYear 
	left join omTrSalesInvoice h on a.CompanyCode=h.CompanyCode and a.BranchCode=h.BranchCode and a.InvoiceNo=h.InvoiceNo
	left join omTrSalesInvoiceVin i on h.CompanyCode=i.CompanyCode and h.BranchCode=i.BranchCode and h.InvoiceNo=i.InvoiceNo
		and c.ChassisCode=i.ChassisCode and c.ChassisNo=i.ChassisNo
	INNER JOIN OmTrSalesSO d ON d.CompanyCode = a.CompanyCode 
		AND d.BranchCode = a.BranchCode 
		AND d.SONo = h.SONo 
	LEFT JOIN omMstModel g ON g.CompanyCode = a.CompanyCode 
		AND g.SalesModelCode = b.SalesModelCode 
	WHERE a.CompanyCode = @CompanyCode 
		AND a.BranchCode LIKE @BranchCode 
		AND ((case when @param = '0' then a.ReturnDate end) <> ''
			 or (case when @param = '1' then convert ( varchar , a.ReturnDate , 112 ) end) 
				BETWEEN convert ( varchar , @DateStart , 112 ) AND convert ( varchar , @DateEnd , 112 ) 
		)
		AND 
		( ( CASE WHEN @SalesType = '' THEN d.SalesType END ) <> '' 
			OR ( CASE WHEN @SalesType <> '' THEN d.SalesType END ) = @SalesType 
		) 
		AND 
		( ( CASE WHEN @SalesFrom = '' THEN d.SalesCode END ) <> '' 
			OR ( CASE WHEN @SalesFrom <> '' THEN d.SalesCode END ) BETWEEN @SalesFrom AND @SalesTo 
		) 
	group by
		a.CompanyCode
		,a.BranchCode
		,d.SalesType
		,g.SalesModelCode + ' - ' + g.SalesModelDesc

select * from #t1
drop table #t1

END
GO
