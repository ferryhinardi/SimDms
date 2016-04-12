GO
/****** Object:  StoredProcedure [dbo].[sp_Select4LookupCustomer2]    Script Date: 01/16/2015 17:50:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].sp_Select4LookupCustomer2 (@CompanyCode varchar(10) , @BranchCode varchar(10),
	@ProfitCenterCode varchar(10), @TOPC varchar(10))
AS
	SELECT a.CustomerCode
		, a.CustomerName
		, a.Address1 + ' ' + a.Address2 + ' ' + a.Address3 + ' ' + a.Address4 as Address
		, ISNULL(c.LookUpValueName,'') AS TopCode
		, b.TopCode as TOPCD
		, b.GroupPriceCode
		, d.RefferenceDesc1 as GroupPriceDesc
		, b.SalesCode
	FROM gnMstCustomer a
	LEFT JOIN gnMstCustomerProfitCenter b ON b.CompanyCode = a.CompanyCode 
		AND b.CustomerCode = a.CustomerCode
	LEFT JOIN gnMstLookUpDtl c ON c.CompanyCode=a.CompanyCode
		AND c.LookUpValue = b.TOPCode    
		AND c.CodeID = @TOPC
	LEFT JOIN omMstRefference d ON a.CompanyCode = d.CompanyCode 
		AND d.RefferenceType = 'GRPR' 
		AND d.RefferenceCode = b.GroupPriceCode
	WHERE a.CompanyCode = @CompanyCode
		AND b.BranchCode = @BranchCode
		AND b.ProfitCenterCode = @ProfitCenterCode
		AND a.Status = '1'
		AND b.SalesType = '1'
		AND b.isBlackList = 0
	ORDER BY CustomerCode;