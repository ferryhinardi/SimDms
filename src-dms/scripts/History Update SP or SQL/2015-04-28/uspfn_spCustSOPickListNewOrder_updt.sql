USE [SAT]
GO

/****** Object:  StoredProcedure [dbo].[uspfn_spCustSOPickListNewOrder]    Script Date: 04/28/2015 16:36:56 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER procedure [dbo].[uspfn_spCustSOPickListNewOrder]   
--DECLARE
@CompanyCode varchar(15),  
@BranchCode varchar(15),  
@ProfitCenterCode varchar(3),  
@TypeOfGoods varchar(2)  
as  
--SET @CompanyCode = '6006410'
--SET @BranchCode = '600641001'
----SET @ProfitCenterCode = '000' --
--SET @TypeOfGoods = '0'
SELECT x.CustomerCode,  
                    (  
                     SELECT c.CustomerName   
                     FROM gnMstCustomer c with(nolock, nowait)  
                     where  c.CompanyCode=x.CompanyCode  
                     AND c.CustomerCode= x.CustomerCode   
                     AND c.CustomerCode=x.CustomerCode  
                    ) AS CustomerName,  
                    (  
                     SELECT c.Address1+' '+c.Address2+' '+c.Address3+' '+c.Address4   
                     FROM gnMstCustomer c with(nolock, nowait)  
                     where  c.CompanyCode=x.CompanyCode  
                     AND c.CustomerCode= x.CustomerCode   
                     AND c.CustomerCode=x.CustomerCode  
  
                    ) as Address  
                    , z.LookUpValueName as ProfitCenter  
                    FROM   
                    (  
                     SELECT DISTINCT a.CompanyCode, a.BranchCode,  
                     b.CustomerCode   
                     FROM spTrnSOSupply a WITH(nolock, nowait) INNER JOIN   
                        spTrnSOrdHdr b ON a.CompanyCode=b.CompanyCode  
                     and a.BranchCode=b.BranchCode  
                     and a.DocNo=b.DocNo  
                        and b.TypeOfGoods=@TypeOfGoods  
                     WHERE pickingslipno = ''  
                     and a.Status=0  
                    ) x   
                    INNER JOIN gnMstCustomerProfitCenter y WITH(nolock, nowait)  
                    ON y.CompanyCode = x.CompanyCode   
                       AND y.BranchCode = x.BranchCode  
                       AND y.CustomerCode = x.CustomerCode  
                    INNER JOIN gnMstLookUpDtl z WITH(nolock, nowait)  
                    ON z.CompanyCode= x.CompanyCode  
                       AND z.CodeID='PFCN'  
                       AND z.LookupValue=y.ProfitCenterCode  
                    WHERE x.CompanyCode=@CompanyCode  
                        AND x.BranchCode=@BranchCode  
                       AND y.ProfitCenterCode=@ProfitCenterCode


GO


