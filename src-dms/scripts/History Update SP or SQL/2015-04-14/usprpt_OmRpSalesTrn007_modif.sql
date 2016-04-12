
ALTER procedure [dbo].[usprpt_OmRpSalesTrn007]
	-- Add the parameters for the stored procedure here
	@CompanyCode VARCHAR(15),
	@BranchCode	 VARCHAR(15),
	@ReqNoA		 VARCHAR(15),
	@ReqNoB		 VARCHAR(15)

AS

DECLARE
	@QRYTmp		AS varchar(max),
	@DBMD		AS varchar(25),
	@CompanyMD  AS varchar(25)


BEGIN

set @CompanyMD = (SELECT TOP 1 CompanyMD FROM gnMstCompanyMapping WHERE CompanyCode=@CompanyCode AND BranchCode=@BranchCode) 
set @DBMD = (SELECT TOP 1 DbMD FROM gnMstCompanyMapping WHERE CompanyCode=@CompanyCode AND BranchCode=@BranchCode)

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;


set @QRYTmp =
'SELECT
	row_number () OVER (ORDER BY a.ReqNo) AS No
	, a.ReqNo
	, a.SKPKNo
	, a.FakturPolisiNo
	, ISNULL(c.SuzukiDONo, '''') DONo
	, (SELECT dbo.GetDateIndonesian (a.FakturPolisiDate)) AS ''Tanggal''
	, ISNULL(c.SuzukiDODate, '''') DODate
	, ISNULL(d.CompanyName, '''') CompanyName
	, ISNULL(d.Address1, '''') CoAdd1
	, ISNULL(d.Address2, '''') CoAdd2
	, ISNULL(d.Address3, '''') CoAdd3
	, case d.ProductType 
		when ''2W'' then ''Harap dibuatkan Faktur untuk motor SUZUKI :''
		when ''4W'' then ''Harap dibuatkan Faktur untuk mobil SUZUKI :''
		when ''A'' then ''Harap dibuatkan Faktur untuk motor SUZUKI :''
		when ''B'' then ''Harap dibuatkan Faktur untuk mobil SUZUKI :''
		else ''Harap dibuatkan Faktur untuk SUZUKI :''
		end as Note
	, ISNULL(c.SuzukiSJNo, '''') SJNo
	, ISNULL(c.SuzukiSJDate, '''') SJDate
	, ISNULL(c.SalesModelCode, '''') Model
	, ISNULL(f.SalesModelDesc, '''') ModelDesc
	, ISNULL(g.RefferenceDesc1, '''') Warna
	, ISNULL(c.SalesModelYear, 0) Tahun
	, a.ChassisNo
	, ISNULL(c.EngineNo, 0) EngineNo
	, ((CASE ISNULL(a.DealerCategory, '''') WHEN ''M'' THEN ''Main Dealer'' WHEN ''S'' THEN ''Sub Dealer'' WHEN ''R'' THEN ''Show Room'' END) + '' / '' + h.CustomerName) AS  Penjual
	, a.SalesmanName
	, a.SKPKName
	, a.SKPKAddress1 Alamat1
	, a.SKPKAddress2 Alamat2
	, a.SKPKAddress3 Alamat3
	, ISNULL(i.LookUpValueName, '''') City
	, a.SKPKTelp1
	, a.SKPKTelp2
	, a.SKPKHP
	, ISNULL(a.SKPKBirthday, '''') SKPKDay
	, a.FakturPolisiName
	, a.FakturPolisiAddress1
	, a.FakturPolisiAddress2
	, a.FakturPolisiAddress3
	, a.FakturPolisiTelp1
	, a.FakturPolisiTelp2
	, a.FakturPolisiHP
	, a.FakturPolisiBirthday
	, (select ISNULL(LookUpValueName, '''') from gnMstLookUpDtl where CompanyCode=a.CompanyCode and CodeID=''FPCT'' and LookUpValue=a.DealerCategory
		) AS DealerCategory
	, ISNULL(b.Remark, '''') Remark
	, ISNULL(UPPER(z.SignName), '''') AS SignName1
	, ISNULL(UPPER(z.TitleSign), '''') AS TitleSign1 
	, a.IDNo
FROM
 omTrSalesReqDetail a
JOIN
 omTrSalesReq b ON b.CompanyCode=a.CompanyCode AND b.BranchCode=a.BranchCode
 AND b.ReqNo=a.ReqNo 
LEFT JOIN
 ' + @DBMD + '..omMstVehicle c ON c.CompanyCode=''' + @CompanyMD + ''' 
 AND c.ChassisCode=a.ChassisCode
 AND c.ChassisNo=a.ChassisNo
LEFT JOIN
 gnMstCoProfile d ON d.CompanyCode=a.CompanyCode AND d.BranchCode=a.BranchCode
LEFT JOIN
 ' + @DBMD + '..omMstModel f ON f.CompanyCode=''' + @CompanyMD + ''' 
 AND f.SalesModelCode=c.SalesModelCode
LEFT JOIN
 ' + @DBMD + '..omMstRefference g ON g.CompanyCode=''' + @CompanyMD + '''
  AND g.RefferenceType=''COLO''
 AND g.RefferenceCode=c.ColourCode
LEFT JOIN
 gnMstCustomer h ON h.CompanyCode=b.CompanyCode AND h.CustomerCode=b.SubDealerCode
LEFT JOIN
 gnMstLookUpDtl i ON i.CompanyCode=a.CompanyCode AND i.CodeID=''CITY'' 
 AND i.LookUpValue=a.SKPKCity
LEFT JOIN gnMstSIgnature z
	ON z.CompanyCode = a.CompanyCode
	AND z.BranchCode = a.BranchCode
	AND z.ProfitCenterCode = ''100''
	AND z.DocumentType = ''RFP''
	AND z.SeqNo = 1
WHERE
 a.CompanyCode	  = ''' + @CompanyCode + '''
 AND a.BranchCode = ''' + @BranchCode + '''
 AND a.ReqNo BETWEEN ''' + @ReqNoA + ''' AND ''' + @ReqNoB + '''
ORDER BY ReqNo'

Exec (@QRYTmp);

END
GO

--------------------------------------------------- BATAS ----------------------------------------------------------


