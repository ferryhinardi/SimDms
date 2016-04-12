USE [SAT_UAT]
GO

/****** Object:  StoredProcedure [dbo].[uspfn_omSlsDoBrowse]    Script Date: 3/12/2015 9:05:54 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [dbo].[uspfn_omSlsDoBrowse]   
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
--exec uspfn_omSlsDoBrowse 6006410,600641001

 set @QRYTemp =  'SELECT Distinct a.DONo, a.DODate, d.SKPKNo, d.RefferenceNo, a.CustomerCode, c.CustomerName , a.ShipTo ,c.CustomerName as ShipToDsc,
            c.Address1 + '' '' + c.Address2 + '' '' + c.Address3 + '' '' + c.Address4 as Address,
            a.WareHouseCode, a.Expedition, a.SONo, f.CustomerName as ExpeditionDsc,a.Remark,
            CASE a.Status WHEN 0 THEN ''OPEN'' WHEN 1 THEN ''PRINT'' WHEN 2 THEN ''APPROVED'' WHEN 3 THEN ''CANCEL'' WHEN 9 THEN ''FINISH'' END as StatusDsc,a.Status
            , CASE ISNULL(b.SalesType, 0) WHEN 0 THEN ''Wholesales'' ELSE ''Direct'' END AS TypeSales, e.LookUpValueName as WrhDsc
            FROM omTrSalesDO a
            LEFT JOIN gnMstCustomerProfitCenter b ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.CustomerCode = b.CustomerCode AND b.ProfitCenterCode = ''100''
            LEFT JOIN gnMstCustomer c ON a.CompanyCode = c.CompanyCode AND a.CustomerCode = c.CustomerCode
            LEFT JOIN omTrSalesSO d ON a.CompanyCode = d.CompanyCode AND a.BranchCode = d.BranchCode AND a.SONo = d.SONo      
            Left join ' + @DBMD + '.dbo.gnMstLookUpDtl e on a.WarehouseCode=e.LookUpValue and e.CodeID=''MPWH''
            LEFT JOIN gnMstCustomer f ON a.CompanyCode = c.CompanyCode AND a.Expedition = c.CustomerCode
            WHERE a.CompanyCode = ''' + @CompanyCode + '''
               AND a.BranchCode = ''' + @BranchCode + '''                             
            ORDER BY a.DONo DESC'

	exec (@QRYTemp);
end         

GO


