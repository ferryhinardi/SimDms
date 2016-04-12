USE [BITPKU]
GO

/****** Object:  View [dbo].[SvStallView]    Script Date: 1/14/2015 7:22:43 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER VIEW [dbo].[SvStallView]
AS
SELECT        CompanyCode, BranchCode, ProductType, StallCode, Description, HaveLift, IsActive, CASE HaveLift WHEN 1 THEN 'Ya' ELSE 'Tidak' END AS HaveLiftString, 
                         CASE IsActive WHEN 1 THEN 'Aktif' ELSE 'Tidak Aktif' END AS IsActiveString
FROM            dbo.svMstStall

GO


