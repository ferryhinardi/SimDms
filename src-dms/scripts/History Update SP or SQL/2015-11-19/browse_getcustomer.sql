if object_id('uspfn_getCustomerBrowse') is not null
       drop procedure uspfn_getCustomerBrowse
go

-- =============================================
-- Author:		fhy
-- Create date: 16112015
-- Description:	Get Customer Browse
-- =============================================
CREATE PROCEDURE [dbo].[uspfn_getCustomerBrowse]
	@dynamicfilters varchar(max)=''
	
AS
BEGIN
declare
@query varchar(max);

	set @query= 'SELECT  distinct top 500
    CASE WHEN ([Extent1].[Address1] IS NULL) THEN N'''' ELSE [Extent1].[Address1] END + N'' '' + CASE WHEN ([Extent1].[Address2] IS NULL) THEN N'''' ELSE [Extent1].[Address2] END + N'' '' + CASE WHEN ([Extent1].[Address3] IS NULL) THEN N'''' ELSE [Extent1].[Address3] END + N'' '' + CASE WHEN ([Extent1].[Address4] IS NULL) THEN N'''' ELSE [Extent1].[Address4] END AS AddressGab, 
    [Extent2].[LookUpValueName] AS [LookUpValueName], 
    [Extent3].[KelurahanDesa] AS [KelurahanDesa], 
    [Extent1].[CustomerCode] AS [CustomerCode], 
    [Extent1].[StandardCode] AS [StandardCode], 
    [Extent1].[CustomerName] AS [CustomerName], 
    [Extent1].[CustomerAbbrName] AS [CustomerAbbrName], 
    [Extent1].[CustomerGovName] AS [CustomerGovName], 
    [Extent1].[CustomerType] AS [CustomerType], 
    [Extent1].[CategoryCode] AS [CategoryCode], 
    [Extent1].[Address1] AS [Address1], 
    [Extent1].[Address2] AS [Address2], 
    [Extent1].[Address3] AS [Address3], 
    [Extent1].[Address4] AS [Address4], 
    [Extent1].[PhoneNo] AS [PhoneNo], 
    [Extent1].[HPNo] AS [HPNo], 
    [Extent1].[FaxNo] AS [FaxNo], 
    [Extent1].[isPKP] AS [isPKP], 
    [Extent1].[NPWPNo] AS [NPWPNo], 
    [Extent1].[NPWPDate] AS [NPWPDate], 
    [Extent1].[SKPNo] AS [SKPNo], 
    [Extent1].[SKPDate] AS [SKPDate], 
    [Extent1].[IbuKota] AS [IbuKota], 
    [Extent1].[AreaCode] AS [AreaCode], 
    [Extent1].[CityCode] AS [CityCode], 
    [Extent1].[ZipNo] AS [ZipNo], 
    [Extent1].[Status] AS [Status], 
    [Extent1].[Email] AS [Email], 
    [Extent1].[BirthDate] AS [BirthDate], 
    [Extent1].[Gender] AS [Gender], 
    [Extent1].[OfficePhoneNo] AS [OfficePhoneNo], 
    [Extent1].[KelurahanDesa] AS [KelurahanDesa1], 
    [Extent1].[KecamatanDistrik] AS [KecamatanDistrik], 
    [Extent1].[KotaKabupaten] AS [KotaKabupaten], 
    [Extent1].[CustomerStatus] AS [CustomerStatus]
    FROM   [dbo].[GnMstCustomer] AS [Extent1]
    INNER JOIN [dbo].[gnMstLookUpDtl] AS [Extent2] ON [Extent1].[CategoryCode] = [Extent2].[LookUpValue]
    LEFT OUTER JOIN [dbo].[GnMstZipCode] AS [Extent3] ON ([Extent1].[KecamatanDistrik] = [Extent3].[KecamatanDistrik]) AND ([Extent1].[KotaKabupaten] = [Extent3].[KotaKabupaten]) AND ([Extent1].[IbuKota] = [Extent3].[IbuKota]) AND ([Extent1].[ZipNo] = [Extent3].[ZipCode]) AND ([Extent1].[KelurahanDesa] = [Extent3].[KelurahanDesa])
    WHERE 1=1
	and [Extent2].[CodeID]=''CSCT'' '+@dynamicfilters+'' 

print(@query)
exec (@query)

END


GO


