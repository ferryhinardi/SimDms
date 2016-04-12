
/****** Object:  StoredProcedure [dbo].[uspfn_spGetPurchaseCampaign]    Script Date: 6/19/2014 11:30:25 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[uspfn_spGetPurchaseCampaign]
@CompanyCode varchar(15),
@BranchCode varchar(15),
@SupplierCode varchar(15),
@PartCode varchar(32)
AS
SELECT * FROM SpMstPurchCampaign
WHERE CompanyCode=@CompanyCode
  AND BranchCode=@BranchCode
  AND SupplierCode=@SupplierCode
  AND PartNo=@PartCode
  AND getdate() BETWEEN begdate AND enddate