CREATE procedure [dbo].[sp_InquiryStokKendaraan] 
(
 @p_CompanyCode varchar(15),
 @p_BranchCode varchar(15),
 @year varchar(10),
 @Month varchar(10),
 @WarehouseCode varchar(100),
 @WarehouseCodeTo Varchar(100),
 @SalesModelCode Varchar(100),
 @SalesModelCodeTo Varchar(100),
 @SalesModelYear Varchar(100),
 @SalesModelYearTo Varchar(100),
 @ColourCode Varchar(100),
 @ColourCodeTo Varchar(100),
 @DB varchar(50)
)
AS
BEGIN

declare @pQuery varchar(max)
set @pQuery =
'

SELECT CONVERT(Varchar,a.Year) as Year
     , CASE WHEN a.Month = 1 THEN ''Januari''
            WHEN a.Month = 2 THEN ''Februari''
            WHEN a.Month = 3 THEN ''Maret'' 
            WHEN a.Month = 4 THEN ''April'' 
            WHEN a.Month = 5 THEN ''Mei'' 
            WHEN a.Month = 6 THEN ''Juni''
            WHEN a.Month = 7 THEN ''Juli'' 
            WHEN a.Month = 8 THEN ''Agustus'' 
            WHEN a.Month = 9 THEN ''September'' 
            WHEN a.Month = 10 THEN ''Oktober''
            WHEN a.Month = 11 THEN ''November'' 
            WHEN a.Month = 12 THEN ''Desember'' 
        END AS Month
     , (select top 1 RefferenceDesc1
          from '+@DB+'.dbo.omMstRefference
         where CompanyCode = a.CompanyCode
           and RefferenceType = ''WARE''
           and RefferenceCode = a.WarehouseCode
         ) as WareHouseName
     , a.SalesModelCode
     , b.SalesModelDesc
     , CONVERT(Varchar,a.SalesModelYear) as ModelYear
     , (c.RefferenceCode + '' - '' + c.RefferenceDesc1) as ColourName
     , a.BeginningAV
     , a.QtyIn
     , a.Alocation
     , a.QtyOut
     , a.EndingAV
  FROM '+@DB+'.dbo.OmTrInventQtyVehicle a 
 INNER JOIN '+@DB+'.dbo.omMstModel b
    ON a.CompanyCode = b.CompanyCode
   AND a.SalesModelCode = b.SalesModelCode 
 INNER JOIN '+@DB+'.dbo.omMstRefference c
    ON a.CompanyCode = c.CompanyCode  
   AND a.ColourCode = c.RefferenceCode                                       
WHERE 1 = 1
'

if len(rtrim(@year)) > 0
   set @pQuery = @pQuery + ' and a.Year = ''' + rtrim(@year) + ''''

if len(rtrim(@Month)) > 0
   set @pQuery = @pQuery + ' and a.Month = ''' + rtrim(@Month) + ''''

if len(rtrim(@WarehouseCode)) > 0
   set @pQuery = @pQuery + ' and a.WarehouseCode between ''' + rtrim(@WarehouseCode) + '''' + ' and ' + '''' + rtrim(@WarehouseCodeTo) + ''''

if len(rtrim(@SalesModelCode)) > 0
   set @pQuery = @pQuery + ' and a.SalesModelCode between ''' + rtrim(@SalesModelCode) + '''' + ' and ' + '''' + rtrim(@SalesModelCodeTo) + ''''

if len(rtrim(@SalesModelYear)) > 0
   set @pQuery = @pQuery + ' and a.SalesModelYear between ''' + rtrim(@SalesModelYear) + '''' + ' and ' + '''' + rtrim(@SalesModelYearTo) + ''''

if len(rtrim(@ColourCode)) > 0
   set @pQuery = @pQuery + ' and a.ColourCode between ''' + rtrim(@ColourCode) + '''' + ' and ' + '''' + rtrim(@ColourCodeTo) + ''''

set @pQuery = @pQuery + ' and a.CompanyCode = ''' + rtrim(@p_CompanyCode) + '''' + ' and a.BranchCode = ''' + rtrim(@p_BranchCode) + ''''
set @pQuery = @pQuery + ' ORDER BY a.Year, a.Month, a.SalesModelCode '

print(@pQuery)
exec(@pQuery)
END
--------------------------------------------------- BATAS ----------------------------------------------------------
