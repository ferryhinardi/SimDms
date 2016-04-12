CREATE VIEW [dbo].[SvReffServiceView]    
AS    
SELECT     CompanyCode, ProductType, RefferenceType, RefferenceCode, Description, DescriptionEng, CASE WHEN IsActive = 1 THEN 'Aktif' ELSE 'Tidak Aktif' END AS IsActiveDesc, IsActive,     
                      CreatedBy, CreatedDate, LastupdateBy, LastupdateDate, IsLocked, LockingBy, LockingDate    
FROM         dbo.svMstRefferenceService