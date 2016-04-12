-- usprpt_OmRpSalesTrn001B '6115204','611520401','SOB/12/000540','SOB/12/000540', '0'

-- usprpt_OmRpSalesTrn001B '6006406','6006406','SOG/12/000476','SOG/12/000476', 0

if object_id('uspfn_OmRpSalesTrn001B') is not null
	drop procedure uspfn_OmRpSalesTrn001B


go
CREATE procedure uspfn_OmRpSalesTrn001B 

(
	@CompanyCode VARCHAR(15),
	@BranchCode	VARCHAR(15),
	@SONoFrom VARCHAR(15),
	@SONoEnd VARCHAR(15),
	@param bit
)

AS

BEGIN
--	declare	@CompanyCode varchar(15)
--	declare @BranchCode varchar(15)
--	declare @SONoFrom varchar(15)
--	declare @SONoEnd varchar(15)
--	declare @param bit
--
--	set @CompanyCode ='6114201'
--	set @BranchCode ='611420100'
--	set @SONoFrom ='SOA/12/000537'
--	set @SONoEnd ='SOA/12/000537'
--	set @param ='1'
--	
	declare @tabData as table 
	(
		CompanyCode		varchar(max),
		BranchCode		varchar(max),
		SoNo			varchar(max),
		Prefix			char(1),
		Model			varchar(max),
		Remark			varchar(max),
		Satuan			decimal(18,0),		
		Qty				decimal(6,0),
		Total			decimal(18,0),		
		BBN				decimal(18,0),		
		Accs			decimal(18,0),		
		Diskon			decimal(18,0),		
		Lain			decimal(18,0),		
		Jumlah			decimal(18,0),
		ChassisNo		int,
		EngineNo		int
	)

	-- INITIAL TABLE FOR UNIT --
	----------------------------
	SELECT * INTO #tUnit FROM (
		SELECT  
			a.CompanyCode
			, a.BranchCode
			, a.SONo
			, b.SalesModelCode Model
			, b.SalesModelYear ModelYear
			, c.ColourCode
			, b.BeforeDiscTotal Satuan
			, case when isnull(d.ChassisNo,0) = 0 then c.Quantity else 1 end Qty
			, SUM(ISNULL(d.bbn, 0))+ SUM(ISNULL(d.kir, 0))as BBN
			, (b.OthersDPP+b.OthersPPn) * (case when isnull(d.ChassisNo,0) = 0 then c.Quantity else 1 end) as Accesories
			, b.DiscIncludePPN * (case when isnull(d.ChassisNo,0) = 0 then c.Quantity else 1 end) as Potongan
			, (b.ShipAmt+b.DepositAmt+b.OthersAmt) * (case when isnull(d.ChassisNo,0) = 0 then c.Quantity else 1 end) as Lainlain 
			, isnull(d.ChassisNo,0) ChassisNo
			, isnull(d.EngineNo,0) EngineNo
		FROM OmTrSalesSO a
		INNER JOIN OmTrSalesSOModel b on a.companyCode = b.companyCode 
			AND a.branchCode = b.branchCode 
			AND a.SONo = b.SONo
		INNER JOIN OmTrSalesSOModelColour c on c.companyCode = b.companyCode 
			AND c.branchCode = b.branchCode 
			AND c.SONo = b.SONo
			AND c.salesModelCode = b.salesModelCode 
			AND c.salesModelYear = b.salesModelYear
		LEFT JOIN OmTrSalesSOVin d on d.companyCode = c.companyCode 
			AND d.branchCode = c.branchCode
			AND d.SONo = c.SONo 
			AND d.salesModelCode = c.salesModelCode 
			AND d.salesModelYear = c.salesModelYear 
			AND c.colourCode = d.colourCode
		WHERE a.companyCode = @CompanyCode 
			AND a.branchCode = @BranchCode 
			AND a.SONo BETWEEN @SONoFrom AND @SONoEnd
		GROUP BY a.CompanyCode
			, a.BranchCode
			, a.SONo
			, b.SalesModelCode
			, b.SalesModelYear
			, c.ColourCode
			, b.BeforeDiscTotal
			, case when isnull(d.ChassisNo,0) = 0 then c.Quantity else 1 end
			, b.BeforeDiscTotal * (case when isnull(d.ChassisNo,0) = 0 then c.Quantity else 1 end)
			, (b.OthersDPP+b.OthersPPn) * (case when isnull(d.ChassisNo,0) = 0 then c.Quantity else 1 end)
			, b.DiscIncludePPN * (case when isnull(d.ChassisNo,0) = 0 then c.Quantity else 1 end)
			, (b.ShipAmt+b.DepositAmt+b.OthersAmt) * (case when isnull(d.ChassisNo,0) = 0 then c.Quantity else 1 end)	
			, d.ChassisNo
			, d.EngineNo

		-- SISA UNIT YANG BELUM ADA VIN

		UNION 

		SELECT 
			a.CompanyCode
			, a.BranchCode
			, a.SONo
			, b.SalesModelCode Model
			, b.SalesModelYear ModelYear
			, c.ColourCode
			, b.BeforeDiscTotal Satuan
			, c.quantity - (select count(*) from OmTrSalesSOVin where companycode=a.companycode and branchcode=a.branchcode and sono=a.sono 
				and salesmodelcode=b.salesmodelcode and salesmodelyear=b.salesmodelyear and colourcode=c.colourcode)as Qty
			, 0 BBN
			, (b.OthersDPP+b.OthersPPn) * (c.quantity - (select count(*) from OmTrSalesSOVin where companycode=a.companycode 
				and branchcode=a.branchcode and sono=a.sono and salesmodelcode=b.salesmodelcode and salesmodelyear=b.salesmodelyear 
				and colourcode=c.colourcode)) as Accessories
			, b.DiscIncludePPN * (c.quantity - (select count(*) from OmTrSalesSOVin where companycode=a.companycode and branchcode=a.branchcode 
				and sono=a.sono and salesmodelcode=b.salesmodelcode and salesmodelyear=b.salesmodelyear and colourcode=c.colourcode)) as Potongan
			, (b.ShipAmt+b.DepositAmt+b.OthersAmt) * (c.quantity - (select count(*) from OmTrSalesSOVin where companycode=a.companycode 
				and branchcode=a.branchcode and sono=a.sono and salesmodelcode=b.salesmodelcode and salesmodelyear=b.salesmodelyear 
				and colourcode=c.colourcode)) as Lainlain
			, 0 ChassisNo
			, 0 EngineNo
		FROM OmTrSalesSO a
		INNER JOIN OmTrSalesSOModel b on a.companyCode = b.companyCode 
			AND a.branchCode = b.branchCode 
			AND a.SONo = b.SONo
		INNER JOIN OmTrSalesSOModelColour c on a.companyCode = c.companyCode 
			AND a.branchCode = c.branchCode 
			AND a.SONo = c.SONo
			AND c.SalesModelCode=b.SalesModelCode
			AND c.SalesModelYear=b.SalesModelYear
		WHERE a.CompanyCode=@COmpanyCode
			AND a.BranchCode=@BranchCode
			AND a.SONo BETWEEN @SONoFrom AND @SONoEnd
			AND c.quantity-(select count(*) from OmTrSalesSOVin where companycode=a.companycode and branchcode=a.branchcode and sono=a.sono 
					and salesmodelcode=b.salesmodelcode and salesmodelyear=b.salesmodelyear and colourcode=c.colourcode)>0
	) #tUnit


	set @param = 1;


	IF (@param=1)
	BEGIN
		-- DETAIL PART DISEMBUNYIKAN --
		-------------------------------

		SELECT * INTO #t1 FROM (
		SELECT 
			a.CompanyCode
			, a.BranchCode
			, a.SONo
			, sum((floor((isnull(a.retailprice,0) * (100+isnull(d.TaxPct,0)))/100)) * isnull(a.qty,0)) Total
			, sum((floor((isnull(a.retailprice,0) * (100+isnull(d.TaxPct,0)))/100) - isnull(a.Afterdisctotal,0)) * isnull(a.qty,0)) Potongan
			, sum((floor((isnull(a.retailprice,0) * (100+isnull(d.TaxPct,0)))/100) - 
				(floor((isnull(a.retailprice,0) * (100+isnull(d.TaxPct,0)))/100) - isnull(a.Afterdisctotal,0))) * isnull(a.qty,0)) Jumlah
		FROM omTrSalesSOAccsSeq a
		INNER JOIN omTrSalesSO b ON b.CompanyCode=a.CompanyCode
			AND b.BranchCode=a.BranchCode
			AND b.SONo=a.SONo
		LEFT JOIN gnMstCustomerProfitCenter c ON c.CompanyCode=a.CompanyCode
			AND c.BranchCode=a.BranchCode
			AND c.CustomerCode=b.CustomerCode
			AND c.ProfitCenterCode='100'
		LEFT JOIN gnMstTax d ON d.CompanyCode=a.CompanyCode
			AND d.TaxCode=c.TaxCode
		WHERE a.CompanyCode=@CompanyCode
			AND a.BranchCode=@BranchCode
			AND a.SONo BETWEEN @SONoFrom AND @SONoEnd 
			AND a.partSeq=1
		GROUP BY a.CompanyCode, a.BranchCode, a.SONo
		) #t1

		INSERT INTO @tabData
		SELECT  
			a.CompanyCode
			, a.BranchCode
			, a.SONo
			, 'A'
			, b.Model
			, b.ColourCode+' - '+isnull(e.RefferenceDesc1,'') Remark
			, b.Satuan+isnull(f.Total,0) AS Satuan
			, b.Qty
			, (b.Satuan+isnull(f.Total,0)) * b.Qty AS Total
			, b.BBN
			, b.Accesories
			, b.Potongan+(isnull(f.Potongan,0)*b.Qty)
			, b.Lainlain 
			, (b.Satuan*b.Qty) + b.BBN + b.Accesories - b.Potongan + b.LainLain + (isnull(f.Jumlah,0)*b.Qty) AS Jumlah
			, b.ChassisNo
			, b.EngineNo
		FROM OmTrSalesSO a
		INNER JOIN #tUnit b on b.companyCode = a.companyCode 
			AND b.branchCode = a.branchCode 
			AND b.SONo = a.SONo
		LEFT JOIN omMstRefference e ON e.CompanyCode = b.CompanyCode
			AND e.RefferenceType = 'COLO'
			AND e.RefferenceCode = b.ColourCode
		LEFT JOIN #t1 f ON f.CompanyCode=a.CompanyCode
			AND f.BranchCode=a.BranchCode
			AND f.SONo=a.SONo
		WHERE a.companyCode = @CompanyCode 
			AND a.branchCode = @BranchCode 
			AND a.SONo BETWEEN @SONoFrom AND @SONoEnd	
		UNION ALL

		SELECT DISTINCT
			a.CompanyCode
			, a.BranchCode
			, a.SONo
			, 'B'
			, 'SPAREPART/ACCS :'
			, ''
			, 0
			, 0
			, 0
			, 0
			, 0
			, 0
			, 0
			, 0
			, 0
			, 0
		FROM OmTrSalesSO a
		INNER JOIN omTrSalesSOAccsSeq b ON b.CompanyCode=a.CompanyCode
			AND b.BranchCode=a.BranchCode
			AND b.SONo=a.SONo
			AND b.PartSeq=1
		WHERE a.companyCode = @CompanyCode 
			AND a.branchCode = @BranchCode 
			AND a.SONo BETWEEN @SONoFrom AND @SONoEnd

		UNION ALL

		SELECT 
			  a.CompanyCode
			, a.BranchCode
			, a.SONo
			, 'C'
			, a.PartNo Model
			, isnull(c.PartName,'') Remark
			, 0 Satuan
			, a.demandqty Qty
			, 0 Total
			, 0 BBN
			, 0 Accesories
			, 0 Potongan
			, 0 LainLain
			, 0 Jumlah
			, 0 Rangka
			, 0 Mesin
		FROM omTrSalesSOAccsSeq a
		INNER JOIN omTrSalesSO b ON b.CompanyCode=a.CompanyCode
			AND b.BranchCode=a.BranchCode
			AND b.SONo=a.SONo
		LEFT JOIN SpMStItemInfo c ON c.CompanyCode=a.CompanyCode
			AND c.PartNo=a.PartNo
		LEFT JOIN gnMstCustomerProfitCenter d ON d.CompanyCode=a.CompanyCode
			AND d.BranchCode=a.BranchCode
			AND d.CustomerCode=b.CustomerCode
			AND ProfitCenterCode='100'
		LEFT JOIN gnMstTax e ON e.CompanyCode=a.CompanyCode
			AND e.TaxCode=d.TaxCode
		WHERE a.CompanyCode=@CompanyCode
			AND a.BranchCode=@BranchCode
			AND a.SONo BETWEEN @SONoFrom AND @SONoEnd 
			AND a.partSeq=1	

		DROP TABLE #t1

	END

	ELSE

	BEGIN
		-- TAMPILKAN PART DETAIL --
		---------------------------		

		SELECT * INTO #tPart FROM (
		SELECT 
			a.CompanyCode
			, a.BranchCode
			, a.SONo
			, a.PartNo Model
			, isnull(c.PartName,'') Remark
			, floor((a.retailprice*(100+isnull(e.TaxPct,0)))/100) Satuan
			, a.demandqty Qty
			, (floor((a.retailprice*(100+isnull(e.TaxPct,0)))/100))*a.demandqty Total
			, 0 BBN
			, 0 Accesories
			, (floor((a.retailprice*(100+isnull(e.TaxPct,0)))/100)-a.Afterdisctotal)*a.demandqty Potongan
			, 0 LainLain
			, (floor((a.retailprice*(100+isnull(e.TaxPct,0)))/100)-(floor((a.retailprice*(100+isnull(e.TaxPct,0)))/100)-a.Afterdisctotal))*a.demandqty Jumlah
		FROM omTrSalesSOAccsSeq a
		INNER JOIN omTrSalesSO b ON b.CompanyCode=a.CompanyCode
			AND b.BranchCode=a.BranchCode
			AND b.SONo=a.SONo
		LEFT JOIN SpMStItemInfo c ON c.CompanyCode=a.CompanyCode
			AND c.PartNo=a.PartNo
		LEFT JOIN gnMstCustomerProfitCenter d ON d.CompanyCode=a.CompanyCode
			AND d.BranchCode=a.BranchCode
			AND d.CustomerCode=b.CustomerCode
			AND ProfitCenterCode='100'
		LEFT JOIN gnMstTax e ON e.CompanyCode=a.CompanyCode
			AND e.TaxCode=d.TaxCode
		WHERE a.CompanyCode=@CompanyCode
			AND a.BranchCode=@BranchCode
			AND a.SONo BETWEEN @SONoFrom AND @SONoEnd 
			AND a.partSeq=1
		) #tPart

		

		INSERT INTO @tabData
		SELECT  
			a.CompanyCode
			, a.BranchCode
			, a.SONo
			, 'A'
			, b.Model
			, b.ColourCode+' - '+isnull(e.RefferenceDesc1,'') Remark
			, b.Satuan
			, b.Qty
			, b.Satuan*b.Qty AS Total
			, b.BBN
			, b.Accesories
			, b.Potongan
			, b.Lainlain 
			, (b.Satuan*b.Qty) + b.BBN + b.Accesories - b.Potongan + b.LainLain Jumlah
			, b.ChassisNo
			, b.EngineNo
		FROM OmTrSalesSO a
		INNER JOIN #tUnit b on b.companyCode = a.companyCode 
			AND b.branchCode = a.branchCode 
			AND b.SONo = a.SONo
		LEFT JOIN omMstRefference e ON e.CompanyCode = b.CompanyCode
			AND e.RefferenceType = 'COLO'
			AND e.RefferenceCode = b.ColourCode
		WHERE a.companyCode = @CompanyCode 
			AND a.branchCode = @BranchCode 
			AND a.SONo BETWEEN @SONoFrom AND @SONoEnd

		UNION ALL

		SELECT DISTINCT
			a.CompanyCode
			, a.BranchCode
			, a.SONo
			, 'B'
			, 'SPAREPART/ACCS :'
			, ''
			, 0
			, 0
			, 0
			, 0
			, 0
			, 0
			, 0
			, 0
			, 0
			, 0
		FROM OmTrSalesSO a
		INNER JOIN omTrSalesSOAccsSeq b ON b.CompanyCode=a.CompanyCode
			AND b.BranchCode=a.BranchCode
			AND b.SONo=a.SONo
			AND b.PartSeq=1
		WHERE a.companyCode = @CompanyCode 
			AND a.branchCode = @BranchCode 
			AND a.SONo BETWEEN @SONoFrom AND @SONoEnd
		UNION ALL
		SELECT
			CompanyCode
			, BranchCode
			, SONo
			, 'C'
			, Model
			, Remark
			, Satuan
			, Qty
			, Total
			, BBN
			, Accesories
			, Potongan
			, LainLain
			, Jumlah
			, 0
			, 0
		FROM #tPart
		DROP TABLE #tPart
	END



--select * from @tabData
	-- DATA RESULT --
	-----------------
   SELECT  
		  a.SONo + case a.SalesType when '0' then '-W' when '1' then '-D' end  AS SONo
		, a.SKPKNo
		, a.RefferenceNo
		, a.SODate
		, b.Model
		, b.Model SalesModelCode
		, b.Remark
		, b.Remark ColourCode
		, b.Satuan 
		, b.Satuan BeforeDiscTotal
		, b.Qty
		, b.Qty Quantity
		, b.Total
		, b.BBN
		, b.Accs
		, b.Accs Accesories
		, b.Diskon Diskon
		, b.Diskon Potongan
		, b.Lain
		, b.Lain LainLain
		, b.Jumlah
		, b.Jumlah SubTotal
		, d.EmployeeName +' ['+a.Salesman+']' as Sales
		, CASE a.SalesType WHEN '0' THEN 'WHOLESALE' WHEN '1' THEN 'DIRECT' END AS TipeSales
		, a.RefferenceNo
		, c.CustomerName +' ['+a.CustomerCode+']' as Pelanggan
		, a.RequestDate
		, dateadd(day, convert(int, e.ParaValue), a.SODate) as JatuhTempo
		, e.LookUpValueName as TOPCode
		, a.shipto
		, g.CustomerName as ShipName
		, a.PrePaymentAmt
		, f.CustomerName as Leasing
		, a.Remark as Ket
		, a.SKPKNo
		, upper(sign1.SignName) AS SignName
		, upper(sign1.TitleSign) AS TitleSign
		, b.ChassisNo
		, b.EngineNo
		, b.Prefix
		, @param HidePart
	FROM OmTrSalesSO a
	INNER JOIN @tabData b on b.companyCode = a.companyCode 
		AND b.branchCode = a.branchCode 
		AND b.SONo = a.SONo
	LEFT JOIN gnMstCustomer c ON c.CompanyCode = a.CompanyCode
		AND c.CustomerCode = a.CustomerCode
	LEFT JOIN HrEmployee d ON d.companyCode = a.companyCode
		--AND d.branchCode = a.branchCode
		AND d.EmployeeID= a.salesman
	LEFT JOIN gnMstLookUpDtl e ON e.CompanyCode = a.CompanyCode
		AND e.CodeID = 'TOPC'
		AND e.LookUpValue = a.TOPCode
	LEFT JOIN gnMstCustomer f ON f.CompanyCode = a.CompanyCode
		AND f.CustomerCode = a.LeasingCo
	LEFT JOIN gnMstCustomer g ON g.CompanyCode = a.CompanyCode
		AND g.CustomerCode = a.shipTo
	LEFT JOIN gnMstSignature AS sign1 ON sign1.CompanyCode = a.CompanyCode
		AND sign1.BranchCode = a.BranchCode
		AND sign1.ProfitCenterCode = '100'
		AND sign1.DocumentType = 'SON'
		AND sign1.SeqNo = '1'
	WHERE a.companyCode = @CompanyCode 
		AND a.branchCode = @BranchCode 
		AND a.SONo BETWEEN @SONoFrom AND @SONoEnd
	ORDER BY a.SONo, b.Prefix, b.Model

	

	DROP TABLE #tUnit
END



go
 usprpt_OmRpSalesTrn001B '6006406','6006406','SOG/12/000476','SOG/12/000476', 0

