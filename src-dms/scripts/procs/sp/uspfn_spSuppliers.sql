/****** Object:  StoredProcedure [dbo].[uspfn_spSuppliers]    Script Date: 7/21/2014 9:18:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
      ALTER procedure [dbo].[uspfn_spSuppliers]  (  @CompanyCode varchar(10) ,@BranchCode varchar(10), @ProfitCenterCode char(3)) 
	   as
	            SELECT distinct
                    a.SupplierCode, a.SupplierName, (a.address1+' '+a.address2+' '+a.address3+' '+a.address4) as Alamat,
                    b.DiscPct as Diskon, (Case a.Status when 0 then 'Tidak Aktif' else 'Aktif' end) as [Status],
                    (SELECT Lookupvaluename FROM gnmstlookupdtl WHERE codeid='PFCN' 
                     AND lookupvalue = b.ProfitCentercode) as Profit
                FROM 
                    gnMstSupplier a
                JOIN gnmstSupplierProfitCenter b ON a.CompanyCode= b.CompanyCode
	                AND a.SupplierCode = b.SupplierCode
                WHERE 
                    a.CompanyCode=@CompanyCode
                    AND b.BranchCode=@BranchCode
					AND b. ProfitCenterCode=@ProfitCenterCode
                    AND b.isBlackList=0
                    AND a.status = 1 