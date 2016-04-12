

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[sp_SpMstItemConversionview]    (  @CompanyCode varchar(10)  )
as
select   a.CompanyCode,a.PartNo,a.ProductType, b.PartName, FromQty, ToQty ,IsActive
from SpMstItemConversion a
left join spMstItemInfo b on
    a.CompanyCode = b.CompanyCode
    and a.ProductType = b.ProductType
    and a.PartNo = b.PartNo
 where   a.CompanyCode=@CompanyCode
 GO
 
create procedure [dbo].[uspfn_spMstCompanyAccountDtl2] (@CompanyCode varchar(10) , @CompanyCodeTo varchar(10))
 as
SELECT [CompanyCode]
      ,[CompanyCodeTo]
      ,[TPGO]
      ,[TPGOName]
      ,[AccountNo]
      ,[AccountName]
FROM [sp_spMstCompanyAccountDtl]
where CompanyCode=@CompanyCode and CompanyCodeTo=@CompanyCodeTo
GO