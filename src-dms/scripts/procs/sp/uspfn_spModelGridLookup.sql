
/****** Object:  StoredProcedure [dbo].[uspfn_spModelGridLookup]    Script Date: 6/19/2014 10:58:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[uspfn_spModelGridLookup]
@CompanyCode varchar(15),
@CodeId varchar(15),
@PartNo varchar(30) = '',
@SEARCH varchar(30) = ''
AS
IF @PartNo <> ''
SELECT 
	 a.ModelCode, 
	 b.LookUpValueName as ModelName 
FROM spMstItemModel a
LEFT JOIN gnMstLookUpDtl b ON b.CompanyCode = @CompanyCode
						  AND b.CodeId = @CodeId
						  AND b.LookUpValue = a.ModelCode
WHERE a.CompanyCode=@CompanyCode  AND a.PartNo=@PartNo
ELSE IF @SEARCH <> ''
SELECT 
	 LookUpValue ModelCode, 
	 LookUpValueName as ModelName 
FROM gnMstLookUpDtl 
WHERE CompanyCode=@CompanyCode AND CodeId = @CodeId AND (
	LookUpValue LIKE @SEARCH + '%' OR
	LookUpValueName LIKE @SEARCH + '%'
)
ORDER By SeqNo
ELSE
SELECT 
	 LookUpValue ModelCode, 
	 LookUpValueName as ModelName 
FROM gnMstLookUpDtl 
WHERE CompanyCode=@CompanyCode AND CodeId = @CodeId
ORDER By SeqNo