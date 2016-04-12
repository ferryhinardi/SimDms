ALTER procedure [dbo].[uspfn_OmPostingSalesInvoice]
	@CompanyCode varchar(20),
	@BranchCode  varchar(20),
	@DocNo       varchar(20),
	@UserID      varchar(20)
AS

BEGIN

--declare @UserID      varchar(20)
--declare @CompanyCode varchar(max)
--declare @BranchCode varchar(max)
--declare @DocNo varchar(max)

--set @CompanyCode = '6558201'
--set @BranchCode = '655820100'
--set @DocNo= 'IVU/13/001280'
--set @UserID      = 'yo'

declare @JournalGL table
(
	CodeTrans      varchar(50),
	SalesModelCode varchar(50),
	AccountNo      varchar(50),
	AmountDb       decimal,
	AmountCr       decimal
)

insert into @JournalGL
select '01 AR', ''
	 , isnull((
		select cls.ReceivableAccNo
		  from omTrSalesInvoice ivu, GnMstCustomerProfitCenter cus, GnMstCustomerClass cls
		 where 1 = 1
		   and cus.CompanyCode   = ivu.CompanyCode
		   and cus.BranchCode    = ivu.BranchCode
		   and cus.CustomerCode  = ivu.CustomerCode
		   and cus.ProfitCenterCode = '100'
		   and cls.CompanyCode   = ivu.CompanyCode
		   and cls.BranchCode    = ivu.BranchCode
		   and cls.CustomerClass = cus.CustomerClass
		   and cus.CompanyCode   = @CompanyCode
		   and cus.BranchCode    = @BranchCode
		   and ivu.InvoiceNo     = @DocNo
		), '') 
	 , isnull((
		select sum(isnull(Quantity, 0) * (AfterDiscDPP + AfterDiscPPn + AfterDiscPPnBm))
		  from omTrSalesInvoiceModel
		 where 1 = 1
		   and CompanyCode = @CompanyCode 
		   and BranchCode  = @BranchCode
		   and InvoiceNo   = @DocNo
		), 0)
	 + isnull((
		select sum(mdl.Quantity * (oth.AfterDiscDPP + oth.AfterDiscPPn))
		  from omTrSalesInvoiceOthers oth left join omTrSalesInvoiceModel mdl
			on oth.BranchCode = mdl.BranchCode
			and oth.InvoiceNo = mdl.InvoiceNo
			and oth.BPKNo = mdl.BPKNo
			and oth.SalesModelCode = mdl.SalesModelCode
		 where 1 = 1
		   and oth.CompanyCode = @CompanyCode 
		   and oth.BranchCode  = @BranchCode
		   and oth.InvoiceNo   = @DocNo
		), 0)
	 + isnull((
		select sum(DPP + PPN)
		  from omTrSalesInvoiceAccs
		 where 1 = 1
		   and CompanyCode = @CompanyCode 
		   and BranchCode  = @BranchCode
		   and InvoiceNo   = @DocNo
		), 0)
	 + (select isnull(sum(isnull(Quantity,0)*isnull(Total,0)),0) from omTrSalesInvoiceAccsSeq where CompanyCode=@CompanyCode
		   and BranchCode=@BranchCode and InvoiceNo=@DocNo) 
	 , 0

insert into @JournalGL
select '02 DISCOUNT UNIT', a.SalesModelCode
	 , isnull((
		select acc.DiscountAccNo
		  from omMstModelAccount acc
		 where 1 = 1
		   and acc.CompanyCode    = a.CompanyCode
		   and acc.BranchCode     = a.BranchCode
		   and acc.SalesModelCode = a.SalesModelCode
		), '')
	 , sum(isnull(a.Quantity, 0) * isnull (a.DiscExcludePPn, 0)) as Discount
	 , 0
  from omTrSalesInvoiceModel a
 where 1 = 1
   and a.CompanyCode = @CompanyCode 
   and a.BranchCode  = @BranchCode
   and a.InvoiceNo   = @DocNo 
 group by a.CompanyCode, a.BranchCode, a.InvoiceNo, a.BPKNo, a.SalesModelCode, a.SalesModelYear
having sum(isnull(a.Quantity, 0) * isnull (a.DiscExcludePPn, 0)) > 0

insert into @JournalGL
select '03 DISCOUNT AKSESORIS', a.SalesModelCode
	 , isnull((
		select acc.DiscountAccNoAks
		  from omMstModelAccount acc
		 where 1 = 1
		   and acc.CompanyCode    = a.CompanyCode
		   and acc.BranchCode     = a.BranchCode
		   and acc.SalesModelCode = a.SalesModelCode
		), '')
	 , sum(isnull(a.DiscExcludePPn, 0)) as Discount
	 , 0
  from omTrSalesInvoiceOthers a
 where 1 = 1
   and a.CompanyCode = @CompanyCode 
   and a.BranchCode  = @BranchCode
   and a.InvoiceNo   = @DocNo 
 group by a.CompanyCode, a.BranchCode, a.InvoiceNo, a.BPKNo, a.SalesModelCode, a.SalesModelYear
having sum(isnull(a.DiscExcludePPn, 0)) > 0

insert into @JournalGL
select distinct '04 DISCOUNT SPAREPART['+a.TypeOfGoods+']', a.TypeOfGoods
	, (select top 1 DiscAccNo from spMstAccount where companycode=a.companycode and branchcode=a.branchcode and typeofgoods=a.typeofgoods) AccountNo
	, (select sum(isnull(Quantity,0)*isnull(DiscExcludePPn,0)) from omtrsalesinvoiceaccsseq where companycode=a.companycode and branchcode=a.branchcode 
		and invoiceno=a.invoiceno and typeofgoods=a.typeofgoods group by typeofgoods) AmountDb
	, 0 AmountCr
from omTrSalesInvoiceAccsSeq a 
inner join omTrSalesInvoice b on b.CompanyCode=a.CompanyCode
	and b.BranchCode=a.BranchCode
	and b.InvoiceNo=a.InvoiceNo
where a.companyCode=@CompanyCode 
	and a.BranchCode=@BranchCode 
	and a.InvoiceNo=@DocNo

insert into @JournalGL
select '05 SALES UNIT',a.SalesModelCode
	 , isnull((
		select acc.SalesAccNo
		  from omMstModelAccount acc
		 where 1 = 1
		   and acc.CompanyCode    = a.CompanyCode
		   and acc.BranchCode     = a.BranchCode
		   and acc.SalesModelCode = a.SalesModelCode
		), '')
	 , 0
	 , sum(isnull(a.Quantity, 0) * isnull (a.AfterDiscDPP, 0))
	 + sum(isnull(a.Quantity, 0) * isnull (a.DiscExcludePPn, 0)) as SalesUnit
  from omTrSalesInvoiceModel a
 where 1 = 1
   and a.CompanyCode = @CompanyCode 
   and a.BranchCode  = @BranchCode
   and a.InvoiceNo   = @DocNo 
 group by a.CompanyCode, a.BranchCode, a.InvoiceNo, a.BPKNo, a.SalesModelCode, a.SalesModelYear
having (sum(isnull(a.Quantity, 0) * isnull (a.AfterDiscDPP, 0)) +
	    sum(isnull(a.Quantity, 0) * isnull (a.DiscExcludePPn, 0))) > 0

insert into @JournalGL
select '06 SALES AKSESORIS',a.SalesModelCode
	 , isnull((
		select acc.SalesAccNoAks
		  from omMstModelAccount acc
		 where 1 = 1
		   and acc.CompanyCode    = a.CompanyCode
		   and acc.BranchCode     = a.BranchCode
		   and acc.SalesModelCode = a.SalesModelCode
		), '')
	 , 0
	 , sum(isnull(b.Quantity, 0) * isnull (a.AfterDiscDPP, 0))
	 + sum(isnull(b.Quantity, 0) * isnull (a.DiscExcludePPn, 0)) as SalesAksesoris
  from omTrSalesInvoiceOthers a, omTrSalesInvoiceModel b
 where 1 = 1
   and b.BranchCode = a.BranchCode 
   and b.InvoiceNo = a.InvoiceNo 
   and b.BPKNo = a.BPKNo 
   and b.SalesModelCode = a.SalesModelCode 
   and b.SalesModelYear = a.SalesModelYear 
   and a.CompanyCode = @CompanyCode 
   and a.BranchCode  = @BranchCode
   and a.InvoiceNo   = @DocNo 
 group by a.CompanyCode, a.BranchCode, a.InvoiceNo, a.BPKNo, a.SalesModelCode, a.SalesModelYear
having (sum(isnull(b.Quantity, 0) * isnull (a.AfterDiscDPP, 0)) +
	    sum(isnull(b.Quantity, 0) * isnull (a.DiscExcludePPn, 0))) > 0

insert into @JournalGL
select distinct '07 SALES SPAREPART ['+a.typeOfGoods+']', a.TypeOfGoods
	, (select top 1 SalesAccNo from spMstAccount where companycode=a.companycode and branchcode=a.branchcode and typeofgoods=a.typeofgoods) AccountNo
	, 0 AmountDb
	, (select sum(isnull(Quantity,0) * isnull(RetailPrice,0)) from omtrsalesinvoiceaccsseq where companycode=a.companycode and branchcode=a.branchcode 
		and invoiceno=a.invoiceno and typeofgoods=a.typeofgoods group by typeofgoods) AmountCr
from omTrSalesInvoiceAccsSeq a 
inner join omTrSalesInvoice b on b.CompanyCode=a.CompanyCode
	and b.BranchCode=a.BranchCode
	and b.InvoiceNo=a.InvoiceNo
 where 1 = 1
   and a.CompanyCode = @CompanyCode 
   and a.BranchCode  = @BranchCode
   and a.InvoiceNo   = @DocNo 
 group by a.CompanyCode, a.BranchCode, a.InvoiceNo, a.TypeOfGoods

insert into @JournalGL
select '08 PPN',''
	 , isnull((
		select cls.TaxOutAccNo
		  from omTrSalesInvoice ivu, GnMstCustomerProfitCenter cus, GnMstCustomerClass cls
		 where 1 = 1
		   and cus.CompanyCode   = ivu.CompanyCode
		   and cus.BranchCode    = ivu.BranchCode
		   and cus.CustomerCode  = ivu.CustomerCode
		   and cus.ProfitCenterCode = '100'
		   and cls.CompanyCode   = ivu.CompanyCode
		   and cls.BranchCode    = ivu.BranchCode
		   and cls.CustomerClass = cus.CustomerClass
		   and cus.CompanyCode   = @CompanyCode
		   and cus.BranchCode    = @BranchCode
		   and ivu.InvoiceNo     = @DocNo
		), '') 
	 , 0
	 , isnull((
		select sum(Quantity * AfterDiscPPn)
		  from omTrSalesInvoiceModel
		 where 1 = 1
		   and CompanyCode = @CompanyCode 
		   and BranchCode  = @BranchCode
		   and InvoiceNo   = @DocNo
		), 0)
	 + isnull((
		select sum(mdl.Quantity * oth.AfterDiscPPn)
		  from omTrSalesInvoiceOthers oth left join omTrSalesInvoiceModel mdl
			on oth.BranchCode = mdl.BranchCode
			and oth.InvoiceNo = mdl.InvoiceNo
			and oth.BPKNo = mdl.BPKNo
			and oth.SalesModelCode = mdl.SalesModelCode
		 where 1 = 1
		   and oth.CompanyCode = @CompanyCode 
		   and oth.BranchCode  = @BranchCode
		   and oth.InvoiceNo   = @DocNo
		), 0)
	 + isnull((
		select sum(PPN)
		  from omTrSalesInvoiceAccs
		 where 1 = 1
		   and CompanyCode = @CompanyCode 
		   and BranchCode  = @BranchCode
		   and InvoiceNo   = @DocNo
		), 0)
	 + (select isnull(sum(isnull(Quantity,0)*isnull(PPN,0)),0) from omTrSalesInvoiceAccsSeq where companyCode = @CompanyCode 
		   and BranchCode=@BranchCode and InvoiceNo=@DocNo)
where (isnull((
		select sum(Quantity * AfterDiscPPn)
		  from omTrSalesInvoiceModel
		 where 1 = 1
		   and CompanyCode = @CompanyCode 
		   and BranchCode  = @BranchCode
		   and InvoiceNo   = @DocNo
		), 0)
	 + isnull((
		select sum(AfterDiscPPn)
		  from omTrSalesInvoiceOthers
		 where 1 = 1
		   and CompanyCode = @CompanyCode 
		   and BranchCode  = @BranchCode
		   and InvoiceNo   = @DocNo
		), 0)
	 + isnull((
		select sum(PPN)
		  from omTrSalesInvoiceAccs
		 where 1 = 1
		   and CompanyCode = @CompanyCode 
		   and BranchCode  = @BranchCode
		   and InvoiceNo   = @DocNo
		), 0)) 
	 +(select isnull(sum(isnull(quantity,0)*isnull(PPN,0)),0) from omTrSalesInvoiceAccsSeq where companyCode = @CompanyCode 
		   and BranchCode=@BranchCode and InvoiceNo=@DocNo) > 0

insert into @JournalGL
select '09 PPN BM',''
	 , isnull((
		select cls.LuxuryTaxAccNo
		  from omTrSalesInvoice ivu, GnMstCustomerProfitCenter cus, GnMstCustomerClass cls
		 where 1 = 1
		   and cus.CompanyCode   = ivu.CompanyCode
		   and cus.BranchCode    = ivu.BranchCode
		   and cus.CustomerCode  = ivu.CustomerCode
		   and cus.ProfitCenterCode = '100'
		   and cls.CompanyCode   = ivu.CompanyCode
		   and cls.BranchCode    = ivu.BranchCode
		   and cls.CustomerClass = cus.CustomerClass
		   and cus.CompanyCode   = @CompanyCode
		   and cus.BranchCode    = @BranchCode
		   and ivu.InvoiceNo     = @DocNo
		), '') 
	 , 0
	 , isnull((
		select sum(Quantity * AfterDiscPPnBm)
		  from omTrSalesInvoiceModel
		 where 1 = 1
		   and CompanyCode = @CompanyCode 
		   and BranchCode  = @BranchCode
		   and InvoiceNo   = @DocNo
		), 0)
where isnull((
		select sum(Quantity * AfterDiscPPnBm)
		  from omTrSalesInvoiceModel
		 where 1 = 1
		   and CompanyCode = @CompanyCode 
		   and BranchCode  = @BranchCode
		   and InvoiceNo   = @DocNo
		), 0) > 0

insert into @JournalGL
select '10 HPP Unit', a.SalesModelCode
	 , isnull((
		select acc.COGSAccNo
		  from omMstModelAccount acc
		 where 1 = 1
		   and acc.CompanyCode    = a.CompanyCode
		   and acc.BranchCode     = a.BranchCode
		   and acc.SalesModelCode = a.SalesModelCode
		), '')
	 , sum(isnull (a.COGS, 0)) as COGS
	 , 0
  from OmTrSalesInvoiceVin a
 where 1 = 1
   and a.CompanyCode = @CompanyCode 
   and a.BranchCode  = @BranchCode
   and a.InvoiceNo   = @DocNo 
 group by a.CompanyCode, a.BranchCode, a.InvoiceNo, a.BPKNo, a.SalesModelCode, a.SalesModelYear
having sum(isnull (a.COGS, 0)) > 0

insert into @JournalGL
select '11 INVENTORY UNIT', a.SalesModelCode
	 , isnull((
		select acc.InventoryAccNo
		  from omMstModelAccount acc
		 where 1 = 1
		   and acc.CompanyCode    = a.CompanyCode
		   and acc.BranchCode     = a.BranchCode
		   and acc.SalesModelCode = a.SalesModelCode
		), '')
	 , 0
	 , sum(isnull (a.COGS, 0)) as COGS
  from OmTrSalesInvoiceVin a
 where 1 = 1
   and a.CompanyCode = @CompanyCode 
   and a.BranchCode  = @BranchCode
   and a.InvoiceNo   = @DocNo 
 group by a.CompanyCode, a.BranchCode, a.InvoiceNo, a.BPKNo, a.SalesModelCode, a.SalesModelYear
having sum(isnull (a.COGS, 0)) > 0

insert into @JournalGL
select distinct '12 COGS SPAREPART ['+a.TypeOfGoods+']', a.TypeOfGoods
	, (select top 1 COGSAccNo from spMstAccount where companycode=a.companycode and branchcode=a.branchcode and typeofgoods=a.typeofgoods) AccountNo
	, (select sum(isnull(Quantity,0)*isnull(COGS,0)) from omtrsalesinvoiceaccsseq where companycode=a.companycode and branchcode=a.branchcode 
			and invoiceno=a.invoiceno and typeofgoods=a.typeofgoods group by typeofgoods) AmountDb
	, 0 AmountCr
from omTrSalesInvoiceAccsSeq a 
inner join omTrSalesInvoice b on b.CompanyCode=a.CompanyCode
	and b.BranchCode=a.BranchCode
	and b.InvoiceNo=a.InvoiceNo
where a.companyCode=@CompanyCode 
	and a.BranchCode=@BranchCode 
	and a.InvoiceNo=@DocNo

insert into @JournalGL
select distinct '13 INVENTORY AKSESORIES ['+a.TypeOfGoods+']', a.TypeOfGoods
	, (select top 1 InventoryAccNo from spMstAccount where companycode=a.companycode and branchcode=a.branchcode and typeofgoods=a.typeofgoods) AccountNo
	, 0 AmountDb
	, (select sum(isnull(Quantity,0)*isnull(COGS,0)) from omtrsalesinvoiceaccsseq where companycode=a.companycode and branchcode=a.branchcode and invoiceno=a.invoiceno
		and typeofgoods=a.typeofgoods group by typeofgoods) AmountCr
from omTrSalesInvoiceAccsSeq a 
inner join omTrSalesInvoice b on b.CompanyCode=a.CompanyCode
	and b.BranchCode=a.BranchCode
	and b.InvoiceNo=a.InvoiceNo
WHERE a.companyCode=@CompanyCode 
	and a.BranchCode=@BranchCode 
	and a.InvoiceNo=@DocNo

if exists (select * from @JournalGL where isnull(AccountNo, '') = '')
begin
	raiserror('terdapat transaksi yang belum memiliki AccountNo',16 ,1 );
	return
end

if (select abs(sum(AmountDb) - sum(AmountCr)) from @JournalGL) > 0
begin
	raiserror('journal belum balance, mohon di check kembali',16 ,1 );
	return
end

--select * from arInterface where DocNo = @DocNo
--delete arInterface where DocNo = @DocNo
insert into arInterface
(CompanyCode,BranchCode,DocNo,DocDate,ProfitCenterCode
,NettAmt,ReceiveAmt,CustomerCode
,TOPCode,DueDate,TypeTrans
,BlockAmt,DebetAmt,CreditAmt,SalesCode,LeasingCode,StatusFlag
,CreateBy,CreateDate,AccountNo,FakturPajakNo,FakturPajakDate)
select @CompanyCode CompanyCode, @BranchCode BranchCode
	 , b.InvoiceNo, b.InvoiceDate 
	 , '100' as ProfitCenterCode 
	 , a.AmountDb as NettAmt, 0 ReceiceAmt
	 , b.CustomerCode
	 , c.TOPCode, b.DueDate
	 , 'INVOICE' TypeTrans
	 , 0 as BlockAmt, 0 as DbAmt, 0 CrAmt
	 , c.SalesCode, c.LeasingCo
	 , '0' StatusFlag, @UserID CreatedBy, getdate() CreatedDate
	 , a.AccountNo, b.FakturPajakNo, b.FakturPajakDate
  from @JournalGL a, omTrSalesInvoice b, omTrSalesSO c
 where substring(CodeTrans, 4, len(CodeTrans) - 3) = 'AR'
   and c.CompanyCode = b.CompanyCode
   and c.BranchCode  = b.BranchCode
   and c.SONo        = b.SONo
   and b.CompanyCode = @CompanyCode
   and b.BranchCode  = @BranchCode
   and b.InvoiceNo   = @DocNo

--select * from glInterface where DocNo = @DocNo
--delete glInterface where DocNo = @DocNo
insert into glInterface
(CompanyCode,BranchCode,DocNo,SeqNo,DocDate,ProfitCenterCode
,AccDate,AccountNo,JournalCode,TypeJournal,ApplyTo,AmountDb,AmountCr
,TypeTrans,BatchNo,BatchDate,StatusFlag
,CreateBy,CreateDate,LastUpdateBy,LastUpdateDate)
select @CompanyCode CompanyCode, @BranchCode BranchCode
	 , b.InvoiceNo, (row_number() over (order by CodeTrans)) SeqNo
	 , b.InvoiceDate, '100' ProfitCenterCode
	 , b.InvoiceDate AccDate, a.AccountNo
	 , 'UNIT' JournalCode, 'INVOICE' TypeJournal
	 , b.InvoiceNo ApplyTo, a.AmountDb, a.AmountCr
	 , substring(CodeTrans, 4, len(CodeTrans) - 3) TypeTrans
	 , '' BatchNo, null BatchDate
	 , '0' StatusFlag, @UserID CreatedBy, getdate() CreatedDate
	 , @UserID LastUpdBy, getdate() LastUpdDate
  from @JournalGL a, omTrSalesInvoice b
 where 1 = 1
   and b.CompanyCode = @CompanyCode
   and b.BranchCode  = @BranchCode
   and b.InvoiceNo   = @DocNo

update omTrSalesInvoice
   set Status = '5'
 where 1 = 1
   and CompanyCode = @CompanyCode
   and BranchCode  = @BranchCode
   and InvoiceNo   = @DocNo

END