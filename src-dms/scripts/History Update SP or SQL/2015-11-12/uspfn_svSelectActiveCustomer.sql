CREATE PROCEDURE [dbo].[uspfn_svSelectActiveCustomer]  
@CompanyCode varchar(15), @DynamicFilter varchar(4000) = ''  
AS  
begin  
  
DECLARE @Query varchar(max);  
 SET @Query = 'select top 500 * from GnMstCustomer  
 WHERE CompanyCode = ''' + @CompanyCode + ''' AND  Status=''1'' '   
            +@DynamicFilter  
 --print(@Query);  
 exec (@Query)  
END