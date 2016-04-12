USE [BITPKU]
GO
/****** Object:  StoredProcedure [dbo].[uspfn_GetLmpDoc]    Script Date: 02/23/2015 17:11:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[uspfn_GetLmpDoc] @CompanyCode varchar(15), @BranchCode varchar(15), @TypeOfGoods varchar(5), @TransType varchar(5), @CodeID varchar(6),@BeginDate datetime, @EndDate datetime  
as  
--declare @CompanyCode varchar(15)  
--declare @BranchCode varchar(15)  
--declare @TypeOfGoods varchar(15)  
--declare @TransType varchar(5)  
--declare @BeginDate Datetime  
--declare @EndDate datetime  
--set @CompanyCode = '6006406'  
--set @BranchCode = '6006401'  
--set @TypeOfGoods = '0'  
--set @TransType = '1%'  
--set @BeginDate = '2014/03/01'  
--set @EndDate = '2014/05/30'  
SELECT  
  
 spTrnSLmpHdr.CompanyCode,  
 spTrnSLmpHdr.BranchCode,  
 spTrnSLmpHdr.LmpNo  
,spTrnSLmpHdr.LmpDate  
,spTrnSLmpHdr.BPSFNo  
,spTrnSLmpHdr.BPSFDate  
,spTrnSLmpHdr.PickingSlipNo  
,spTrnSLmpHdr.PickingSlipDate  
,spTrnSLmpHdr.TypeOfGoods  
,spTrnSLmpHdr.CustomerCodeShip  
,spTrnSLmpHdr.CustomerCode  
,spTrnSLmpHdr.CustomerCodeBill  
,spTrnSLmpHdr.TotSalesQty  
,spTrnSLmpHdr.TotSalesAmt  
,spTrnSLmpHdr.TotDiscAmt  
,spTrnSLmpHdr.TotDPPAmt  
,spTrnSLmpHdr.TotPPNAmt  
,spTrnSLmpHdr.CreatedBy  
,spTrnSLmpHdr.CreatedDate  
,spTrnSLmpHdr.LastUpdateBy  
,spTrnSLmpHdr.LastUpdateDate  
,spTrnSLmpHdr.LastUpdateBy  
,spTrnSLmpHdr.LastUpdateDate  
,spTrnSLmpHdr.isPKP  
,spTrnSLmpHdr.isLocked  
,spTrnSLmpHdr.LockingBy  
,spTrnSLmpHdr.LockingDate,  
 spTrnSLmpHdr.CustomerCode,  
 spTrnSLmpHdr.CustomerCodeShip,
 spTrnSLmpHdr.CustomerCodeBill as CustomerCodeTagih,
 b.CustomerName,  
 b.Address1,  
 b.Address2,  
 b.Address3,  
 b.Address4,  
 b.CustomerName CustomerNameTagih,  
 b.Address1 Address1Tagih,  
 b.Address2 Address2Tagih,  
 b.Address3 Address3Tagih,  
 b.Address4 Address4Tagih,  
 c.LookUpValueName TransType    
  
FROM spTrnSLmpHdr  
left join gnMstCustomer b  
ON spTrnSLmpHdr.CompanyCode = b.CompanyCode  
AND spTrnSLmpHdr.CustomerCode = b.CustomerCode   
left join gnMstLookupDtl c on  
spTrnSLmpHdr.CompanyCode = c.CompanyCode  
and spTrnSLmpHdr.TransType= c.LookupValue   
AND c.CodeID = @CodeID  
WHERE spTrnSLmpHdr.CompanyCode=@CompanyCode  
  AND spTrnSLmpHdr.BranchCode=@BranchCode  
  AND spTrnSLmpHdr.TypeOfGoods = @TypeOfGoods   
  AND Convert(Varchar, spTrnSLmpHdr.LmpDate, 111) between @BeginDate and @EndDate  
  AND TransType LIKE @TransType  
ORDER BY spTrnSLmpHdr.LmpNo DESC