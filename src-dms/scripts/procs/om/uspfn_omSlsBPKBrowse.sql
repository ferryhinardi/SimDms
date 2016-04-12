USE [SAT_UAT]
GO

/****** Object:  StoredProcedure [dbo].[uspfn_omSlsBPKBrowse]    Script Date: 3/12/2015 9:04:05 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [dbo].[uspfn_omSlsBPKBrowse]   
(  
 @CompanyCode varchar(15),  
 @BranchCode varchar(15) 
)  
AS

DECLARE 
	@QRYTemp as varchar(max),
	@CompanyMD as varchar(15),
	@DBMD as varchar(15)
 
set @CompanyMD = (SELECT CompanyMD FROM gnMstCompanyMapping WHERE CompanyCode=@CompanyCode AND BranchCode=@BranchCode) 
set @DBMD = (SELECT DbMD FROM gnMstCompanyMapping WHERE CompanyCode=@CompanyCode AND BranchCode=@BranchCode) 
 
BEGIN  
--exec uspfn_omSlsBPKBrowse 6006410,600641001
 SET @QRYTemp = 
 'SELECT Distinct a.BPKNo, a.BPKDate, a.DONo, a.SONo, d.SKPKNo, d.RefferenceNo, a.CustomerCode  , c.CustomerName , a.ShipTo , e.CustomerName as ShipToDsc,
            c.Address1 + '' '' + c.Address2 + '' '' + c.Address3 + '' '' + c.Address4 as Address, a.WareHouseCode, f.LookUpValueName as WrhDsc, a.Expedition,g.SupplierName as ExpeditionDsc,a.Status,
            CASE a.Status WHEN ''0'' THEN ''Open'' WHEN ''1'' THEN ''Printed'' WHEN ''2'' THEN ''Approved'' WHEN ''3'' THEN ''Canceled'' WHEN ''9'' THEN ''Finished'' END as StatusDsc       
            ,b.SalesType, CASE ISNULL(b.SalesType, 0) WHEN 0 THEN ''Wholesales'' ELSE ''Direct'' END AS TypeSales, a.Remark
            FROM omTrSalesBPK a
            LEFT JOIN gnMstCustomerProfitCenter b ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.CustomerCode = b.CustomerCode AND b.ProfitCenterCode = ''100''
            LEFT JOIN gnMstCustomer c ON a.CompanyCode = c.CompanyCode AND a.CustomerCode = c.CustomerCode
            LEFT JOIN omTrSalesSO d ON a.CompanyCode = d.CompanyCode AND a.BranchCode = d.BranchCode AND a.SONo = d.SONo
            LEFT JOIN gnMstCustomer e ON a.CompanyCode = e.CompanyCode AND a.shipto = e.CustomerCode
            Left join ' + @DBMD + '.dbo.gnMstLookUpDtl f on f.CompanyCode = ''' + @CompanyMD + ''' and a.WarehouseCode=f.LookUpValue and f.CodeID=''MPWH''
            LEFT JOIN gnMstSupplier g ON a.CompanyCode = g.CompanyCode AND a.Expedition = g.SupplierCode
            WHERE a.CompanyCode = ''' + @CompanyCode + '''
               AND a.BranchCode = ''' + @BranchCode + '''                             
            ORDER BY a.BPKNo DESC'

	Exec (@QRYTemp);
End


GO


