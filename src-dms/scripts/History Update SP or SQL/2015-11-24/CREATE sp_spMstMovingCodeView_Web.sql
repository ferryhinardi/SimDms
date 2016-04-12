create procedure [dbo].[sp_spMstMovingCodeView_Web]  (@CompanyCode varchar(15), @DynamicFilter varchar(4000) = '', @top int = 100)
 as

DECLARE @Query varchar(max);
SET @Query = 'SELECT TOP ' + CONVERT(VARCHAR, @top) + ' MovingCode, MovingCodeName,
	Formula = Cast(Param1 as varchar(10)) + Sign1 + Variable + Sign2 + Cast(Param2 as varchar(10)),
	Param1, Sign1, Variable, Param2, Sign2,CompanyCode
	FROM spMstMovingCode
	Where CompanyCode = ''' + @CompanyCode + '''' 
        + @DynamicFilter
          
        
--print(@Query);
exec (@Query)
