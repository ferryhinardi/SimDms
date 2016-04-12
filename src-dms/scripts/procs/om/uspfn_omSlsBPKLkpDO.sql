USE [SAT_UAT]
GO

/****** Object:  StoredProcedure [dbo].[uspfn_omSlsBPKLkpDO]    Script Date: 3/12/2015 9:04:55 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE procedure [dbo].[uspfn_omSlsBPKLkpDO]   
(  
 @CompanyCode varchar(15),  
 @BranchCode varchar(15),
 @ProfitCenterCode varchar(15)
 )  
AS 

DECLARE 
	@QRYTemp as varchar(max),
	@CompanyMD as varchar(15),
	@DBMD as varchar(15)
 
set @CompanyMD = (SELECT CompanyMD FROM gnMstCompanyMapping WHERE CompanyCode=@CompanyCode AND BranchCode=@BranchCode) 
set @DBMD = (SELECT DbMD FROM gnMstCompanyMapping WHERE CompanyCode=@CompanyCode AND BranchCode=@BranchCode) 
 
BEGIN  
-- exec uspfn_omSlsBPKLkpDO  6006410,600641001,'100'
SET @QRYTemp =
	'SELECT Distinct a.DONo, a.DODate, a.SONo, g.SKPKNo, g.RefferenceNo, a.CustomerCode ,c.CustomerName, 
            c.Address1 + '' '' + c.Address2 + '' '' + c.Address3 + '' '' + c.Address4 as Address,
            a.ShipTo, c1.CustomerName as ShipName, 
            a.WareHouseCode, f.LookUpValueName as WareHouseName, a.Expedition, e.SupplierName as ExpeditionName,
            b.SalesType,(CASE ISNULL(b.SalesType, 0) WHEN 0 THEN ''WholeSales'' ELSE ''Direct'' END) AS SalesTypeDsc, a.Remark
            FROM omTrSalesDO a
            LEFT JOIN gnMstCustomerProfitCenter b ON b.CompanyCode = a.CompanyCode AND b.BranchCode = a.BranchCode AND b.CustomerCode = a.CustomerCode
			LEFT JOIN gnMstCustomer c ON c.CompanyCode = a.CompanyCode AND c.CustomerCode = a.CustomerCode
            LEFT JOIN gnMstCustomer c1 ON c1.CompanyCode = a.CompanyCode AND c1.CustomerCode = a.ShipTo
			LEFT JOIN  omTrSalesDODetail d ON d.CompanyCode = a.CompanyCode AND d.BranchCode = a.BranchCode AND d.DoNo = a.DoNo
			LEFT JOIN gnMstSupplier e ON e.CompanyCode = a.CompanyCode AND e.SupplierCode = a.Expedition 
			LEFT JOIN ' + @DBMD + '.dbo.gnMstLookUpDtl f ON f.CompanyCode = ''' + @CompanyMD + ''' AND f.LookUpValue = a.WareHouseCode and f.CodeID =''MPWH''
            LEFT JOIN omTrSalesSO g ON a.CompanyCode = g.CompanyCode AND a.BranchCode = g.BranchCode AND a.SONo = g.SONo            
            WHERE a.Status = ''2''
            and d.StatusBPK <> ''1''  
            AND a.CompanyCode = ''' + @CompanyCode + '''
            AND b.BranchCode = ''' + @BranchCode + '''
			AND b.ProfitCenterCode = ''' + @ProfitCenterCode + '''                   
            ORDER BY a.DONo ASC'

	EXEC (@QRYTemp);
END      




GO


