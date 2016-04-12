USE [SMGBLI]
GO
/****** Object:  StoredProcedure [dbo].[usprpt_SpRpRgs017]    Script Date: 5/20/2015 10:05:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[usprpt_SpRpRgs017]
 @CompanyCode VARCHAR(15),  
 @BranchCode VARCHAR(15),   
 @StartDate VARCHAR(30),  
 @EndDate VARCHAR(30),  
 @TransType VARCHAR(2),  
 @TypeOfGoods VARCHAR(1)   
AS  
  

select a.CompanyCode, a.BranchCode
     , a.FPJNo, a.PickingSlipNo
     , replace(convert(varchar, a.FPJDate, 106), ' ', '-') as FPJDate
     , a.TypeOfGoods
     , OrderNoFpj = isnull((
                   select top 1 ReferenceNo
                     from spTrnSFpjDtl
                    where CompanyCode = a.CompanyCode
                      and BranchCode = a.BranchCode
                      and FpjNo = b.FpjNo
                    order by CreatedDate desc
                     ), '')
     , OrderNo = isnull((
                   select top 1 OrderNo
                     from spTrnSORDHdr
                    where CompanyCode = a.CompanyCode
                      and BranchCode = a.BranchCode
                      and DocNo = b.DocNo
                    order by CreatedDate desc
                     ), '')
     , isnull(c.CustomerName, '-') as CustomerName
     , b.PartNo
     , (select PartName from SpMstItemInfo 
         where CompanyCode = a.CompanyCode
           and PartNo = b.PartNo) as PartName
     , b.QtyBill
     , b.RetailPrice * b.QtyBill as HargaJualKotor
     , b.DiscPct
     , b.DiscAmt
     , b.NetSalesAmt
     , a.TotPPNAmt
     , a.TotFinalSalesAmt
     , b.CostPrice * QtyBill as HargaPokok
  from SpTrnSFPJHdr a
 inner join SpTrnSFPJDtl b
    on a.CompanyCode = b.CompanyCode
   and a.BranchCode = b.BranchCode  
   and a.FPJNo = b.FPJNo  
  left join GnMstCustomer c
    on a.CompanyCode = c.CompanyCode  
   and a.CustomerCode = c.CustomerCode  
 where 1 = 1 
   and a.CompanyCode = @CompanyCode
   and a.BranchCode = @BranchCode
   --and a.Transtype like @TransType
   and a.Transtype = (case rtrim(isnull(@Transtype, '')) when '' then a.Transtype else @Transtype end)
   and convert(varchar, a.FPJDate, 112) 
	between convert(varchar, convert(datetime,@StartDate ), 112) 
	    and convert(varchar,convert(datetime,@EndDate ), 112)  
   and a.TypeOfGoods = (case rtrim(isnull(@TypeOfGoods, '')) when '' then a.TypeOfGoods else @TypeOfGoods end)
 order by a.CreatedDate, a.FPJNo asc   
 
