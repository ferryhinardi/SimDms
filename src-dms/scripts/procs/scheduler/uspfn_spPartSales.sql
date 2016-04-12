-------------------------------  
-- PART SALES  
-- Request  by SMC  
-- Created  by HTO, 26 May 2014  
-------------------------------  
  
CREATE PROCEDURE uspfn_spPartSales  
AS  
  
BEGIN  
 declare @Date        datetime  
 declare @PeriodStart datetime  
 declare @PeriodEnd   datetime  
 declare @curDate  datetime  
 declare @CompanyCode varchar(15)  
  
 -- Setup collection date  
 set @Date        = getdate() --convert(date,'2014/04/30')  
    set @curDate     = getdate()  
 set @CompanyCode = isnull((select top 1 CompanyCode from gnMstOrganizationHdr),'9999999')  
 if not exists (select ParaValue from gnMstLookUpDtl  
                 where CompanyCode=@CompanyCode  
                   and CodeID     ='SEND'  
                   and LookUpValue='PARTSALES')  
  begin  
   insert into gnMstLookUpHdr  
      (CompanyCode, CodeID, CodeName, FieldLength, isNumber,   
       CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate,   
       isLocked, LockingBy, LockingDate)  
      values(@CompanyCode, 'SEND','PART SALES DATA SCHEDULE', 0, 0,  
       'AUTOMATIC', @Date, NULL, NULL, 0, NULL, NULL)  
   insert into gnMstLookUpDtl  
      (CompanyCode, CodeID, LookUpValue, SeqNo, ParaValue, LookUpValueName,  
       CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)  
      values(@CompanyCode, 'SEND', 'PARTSALES', 1, convert(varchar,@Date,111),   
       'STARTING PART SALES DATE', 'AUTOMATIC', @Date, NULL, NULL)  
   set @PeriodStart = convert(datetime,'2014/01/01')  
   set @PeriodEnd   = dateadd(day,-1,@Date)  
  end  
 else  
  begin  
   set @PeriodStart = dateadd(day,-7,@Date)    
   set @PeriodEnd   = dateadd(day,-1,@Date)  
  end  
  
 -- Setup Part Sales table  
 if not exists (select * from sys.objects   
                     where object_id = object_id(N'[spHstPartSales]') and type=N'U')  
  create table [spHstPartSales]  
   (  
   [RecordID]   [uniqueidentifier] NOT NULL,  
   [RecordDate]  [datetime]   NOT NULL,  
   [CompanyCode]  [varchar](15)  NOT NULL,  
   [BranchCode]  [varchar](15)  NOT NULL,  
   [InvoiceNo]   [varchar](15)  NOT NULL,  
   [InvoiceDate]  [datetime]   NOT NULL,  
   [FPJNo]    [varchar](15)  NOT NULL,  
   [FPJDate]   [datetime]   NOT NULL,  
   [CustomerCode]  [varchar](15)  NOT NULL,  
   [CustomerName]  [varchar](100)  NOT NULL,  
   [CustomerClass]  [varchar](15)  NOT NULL,  
   [PartNo]   [varchar](20)  NOT NULL,  
   [PartName]   [varchar](100)  NULL,  
   [TypeOfGoods]  [char]   (1)  NOT NULL,  
   [TypeOfGoodsDesc] [varchar](30)  NOT NULL,  
   [QtyBill]   [numeric]( 9,2)  NOT NULL,  
   [CostPrice]   [numeric](12,0)  NOT NULL,  
   [RetailPrice]  [numeric](12,0)  NOT NULL,  
   [DiscPct]   [numeric]( 5,2)  NOT NULL,  
   [DiscAmt]   [numeric](12,0)  NOT NULL,  
   [NetSalesAmt]  [numeric](12,0)  NOT NULL,  
   [SendDate]   [datetime]   NULL,  
   constraint [PK_spHstPartSales] primary key clustered   
   ( [RecordID] ASC)  
   with (pad_index=OFF, statistics_norecompute=OFF, ignore_dup_key=OFF,   
     allow_row_locks=ON, allow_page_locks=ON) ON [PRIMARY]  
   ) on [PRIMARY]  
  
 -- Data Collection  
 select * into #SGO  
   from ( select h.CompanyCode, h.BranchCode, h.InvoiceNo, h.InvoiceDate,   
     h.FPJNo, h.FPJDate, h.CustomerCode, c.CustomerName,   
     CustomerClass = case when c.CategoryCode='00' then 'MAIN DEALER'  
           when c.CategoryCode='01' then 'SUB DEALER'  
           when c.CategoryCode='15' then 'PART SHOP'  
           else                          'DIRECT CUSTOMER'  
         end,  
     d.PartNo, i.PartName, h.TypeOfGoods,   
     TypeOfGoodsDesc = isnull((select LookUpValueName from gnMstLookUpDtl  
              where CompanyCode=h.CompanyCode  
             and CodeID='TPGO'  
             and LookUpValue=h.TypeOfGoods),''),  
     d.QtyBill, d.CostPrice, d.RetailPrice, d.DiscPct, d.DiscAmt, d.NetSalesAmt  
      from spTrnSInvoiceHdr h  
     inner join gnMstCustomer c  
       on h.CompanyCode=c.CompanyCode  
      and h.CustomerCode=c.CustomerCode  
     inner join spTrnSInvoiceDtl d  
       on h.CompanyCode=d.CompanyCode  
      and h.BranchCode=d.BranchCode  
      and h.InvoiceNo=d.InvoiceNo  
     inner join spMstItemInfo i  
       on d.CompanyCode=i.CompanyCode  
      and d.PartNo=i.PartNo  
  where 1=1  --h.TypeOfGoods in ('1','2','5')  -- 0:SGP, 1:SGO, 2:SGA, 3:NON SGP, 4:OTHERS, 5:NON SGA  
    and convert(varchar,h.InvoiceDate,111) between @PeriodStart and @PeriodEnd  
    and not exists (select top 1 1 from spHstPartSales  
                     where CompanyCode=h.CompanyCode  
        and BranchCode =h.BranchCode  
        and InvoiceNo  =h.InvoiceNo  
        and FPJNo      =h.FPJNo  
        and PartNo     =d.PartNo)  
  union all  
 select h.CompanyCode, h.BranchCode, h.LmpNo InvoiceNo, h.CreatedDate InvoiceDate,   
     h.BPSFNo, h.BPSFDate, h.CustomerCode, c.CustomerName,   
     'SERVICE' CustomerClass, d.PartNo, i.PartName, h.TypeOfGoods,   
     TypeOfGoodsDesc = isnull((select LookUpValueName from gnMstLookUpDtl  
           where CompanyCode=h.CompanyCode  
          and CodeID='TPGO'  
          and LookUpValue=h.TypeOfGoods),''),  
     d.QtyBill, d.CostPrice, d.RetailPrice, d.DiscPct, d.DiscAmt, d.NetSalesAmt  
   from spTrnSLmpHdr h  
  inner join gnMstCustomer c  
    on h.CompanyCode=c.CompanyCode  
   and h.CustomerCode=c.CustomerCode  
  inner join spTrnSLmpDtl d  
    on h.CompanyCode=d.CompanyCode  
   and h.BranchCode=d.BranchCode  
   and h.LmpNo=d.LmpNo  
  inner join spMstItemInfo i  
    on d.CompanyCode=i.CompanyCode  
   and d.PartNo=i.PartNo  
  where 1=1 --h.TypeOfGoods in ('1','2','5')  -- 0:SGP, 1:SGO, 2:SGA, 3:NON SGP, 4:OTHERS, 5:NON SGA  
    and convert(varchar,h.LmpDate,111) between @PeriodStart and @PeriodEnd   
    and not exists (select top 1 1 from spHstPartSales  
                     where CompanyCode=h.CompanyCode  
        and BranchCode =h.BranchCode  
        and InvoiceNo  =h.LmpNo  
        and FPJNo      =h.BPSFNo  
        and PartNo     =d.PartNo)  
     ) a  
  
 -- insert to Part Sales table  
 insert into spHstPartSales  select NEWID(), @curDate, *, NULL from #SGO  
  
 -- select Part Sales data  
 -- select * from spHstPartSales  
 select h.CompanyCode, dm.DealerAbbreviation, h.BranchCode, do.OutletAbbreviation, h.InvoiceNo, h.InvoiceDate,   
     h.FPJNo, h.FPJDate, h.CustomerCode, h.CustomerName, h.CustomerClass, h.PartNo, h.PartName, h.TypeOfGoods,   
     h.TypeOfGoodsDesc, h.QtyBill, h.CostPrice, h.RetailPrice, h.DiscPct, h.DiscAmt, h.NetSalesAmt  
   from spHstPartSales h  
   left join gnMstDealerMapping dm  
   on h.CompanyCode=dm.DealerCode  
   left join gnMstDealerOutletMapping do  
   on h.CompanyCode=do.DealerCode  
     and h.BranchCode=do.OutletCode  
     where h.RecordDate=@curDate  
  order by dm.DealerAbbreviation, h.BranchCode, h.InvoiceDate, h.PartNo  
 drop table #SGO  
END  