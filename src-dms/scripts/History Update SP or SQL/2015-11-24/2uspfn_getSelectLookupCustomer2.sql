if object_id('uspfn_getSelectLookupCustomer2') is not null
       drop procedure uspfn_getSelectLookupCustomer2
go


-- =============================================
-- Author:		fhy
-- Create date: 23112015
-- Description:	get lookup customer
-- =============================================
CREATE PROCEDURE [dbo].[uspfn_getSelectLookupCustomer2] 	
	@companyCode varchar(25),
	@branchCode varchar(25),
	@profitCenterCode varchar(15),
	@dynamicfilters varchar(max)=''
AS
BEGIN
declare
	@query varchar(max);

set @query= 'SELECT top 500 a.CustomerCode
                    , a.CustomerName
                    , isnull(a.Address1,'''') + '' '' + isnull(a.Address2,'''') + '' '' + isnull(a.Address3,'''') + '' '' + isnull(a.Address4,'''') as Address
                    , isnull(a.Address1,'''') Address1
                    , isnull(a.Address2,'''') Address2
                    , isnull(a.Address3,'''') Address3
                    , isnull(a.Address4,'''') Address4
                    , ISNULL(c.ParaValue,'''') AS TopCode
                    , ISNULL(c.ParaValue,'''') as TOPDesc
                    , b.TopCode as TOPCD
                    , b.GroupPriceCode
                    , d.RefferenceDesc1 as GroupPriceDesc
                    , b.SalesCode
                FROM gnMstCustomer a
                LEFT JOIN gnMstCustomerProfitCenter b ON b.CompanyCode = a.CompanyCode 
                    AND b.CustomerCode = a.CustomerCode
                LEFT JOIN gnMstLookUpDtl c ON c.CompanyCode=a.CompanyCode
                    AND c.LookUpValue = b.TOPCode    
                    AND c.CodeID = ''TOPC''
                LEFT JOIN omMstRefference d ON a.CompanyCode = d.CompanyCode 
                    AND d.RefferenceType = ''GRPR''
                    AND d.RefferenceCode = b.GroupPriceCode
                WHERE 
					a.CompanyCode = '+@companyCode+'
                    AND b.BranchCode = '+@branchCode+'
                    AND b.ProfitCenterCode ='+@profitCenterCode+' '+@dynamicfilters+'

	                AND a.Status = ''1''
                    AND b.SalesType = ''1''
	                AND b.isBlackList = 0
                ORDER BY CustomerCode'

print(@query)
exec (@query)

END




GO


