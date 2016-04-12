USE [BIT201310]
GO

/****** Object:  StoredProcedure [dbo].[uspfn_SvGetClaimApp]    Script Date: 3/17/2014 11:18:32 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<DL>
-- Create date: <March 5 2014>
-- Description:	<Untuk Menampilkan Claim App>
-- =============================================

-- uspfn_SvGetClaimApp '6006406','6006401','4W','CLA/13/000060'
CREATE procedure [dbo].[uspfn_SvGetClaimApp]
--declare 
	 @CompanyCode  varchar(20),
	 @BranchCode   varchar(20),
	 @ProductType  varchar(20),
	 @GenerateNo   varchar(20)
--
as


   select 
	(row_number() over (order by claimApp.GenerateSeq)) as No
	,claimApp.CategoryCode 
	,(select Description from svMstRefferenceService WHERE RefferenceType='CLAIMCAT' and RefferenceCode = claimApp.CategoryCode) as CategoryName
	,claimApp.IssueNo 
	,claimApp.IssueDate 
	,claimApp.ServiceBookNo  
	,claimApp.ChassisCode
	,claimApp.ChassisNo
	,claimApp.EngineCode
	,claimApp.EngineNo
	,claimApp.IsCBU
	,claimApp.BasicModel
	,claimApp.RegisteredDate
	,claimApp.RepairedDate
	,claimApp.Odometer
	,claimApp.ComplainCode
    ,(select Description from svMstRefferenceService WHERE RefferenceType='COMPLNCD' and RefferenceCode = claimApp.ComplainCode) as ComplainDesc
	,claimApp.DefectCode
    ,(select Description from svMstRefferenceService WHERE RefferenceType='DEFECTCD' and RefferenceCode = claimApp.DefectCode) as DefectDesc
	,claimApp.SubletHour
	,substring(claimApp.OperationNo, 1, 6) as BasicCode
	,Description
	,substring(claimApp.OperationNo, 7, 9) as VarCom
	,claimApp.OperationNo
	,claimApp.OperationHour as Hours
    ,claimApp.GenerateSeq
    ,claimApp.TroubleDescription
    ,claimApp.ProblemExplanation
from svTrnClaimApplication claimApp
JOIN SvBasicCodeView on claimApp.BasicModel = SvBasicCodeView.BasicModel and claimApp.OperationNo = SvBasicCodeView.OperationNo and VarCom = VarCom
where 
    claimApp.CompanyCode = @CompanyCode and
    claimApp.BranchCode = @BranchCode and
    claimApp.ProductType = @ProductType and
    claimApp.GenerateNo = @GenerateNo
order by GenerateSeq Asc

GO


