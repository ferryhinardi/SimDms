ALTER procedure [dbo].[uspfn_SvTrnInvoiceCreate]
	@CompanyCode varchar(15),
	@BranchCode  varchar(15),
	@ProductType varchar(15),
	@ServiceNo   int,
	@BillType    char(1),
	@InvoiceNo   varchar(15),
	@Remarks     varchar(max),
	@UserID      varchar(15)
as  

declare @errmsg varchar(max)
--raiserror ('test error',16,1);

DECLARE @CompanyMD AS VARCHAR(15)
DECLARE @BranchMD AS VARCHAR(15)
DECLARE @WarehouseMD AS VARCHAR(15)
DECLARE @DbMD AS VARCHAR(15)
declare @md bit

SET @CompanyMD = (SELECT CompanyMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)
SET @BranchMD = (SELECT BranchMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)
SET @WarehouseMD = (SELECT WarehouseMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)
SET @DbMD = (SELECT DbMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)
set @md = (select case WHEN EXISTS(select * from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode and CompanyMD = @CompanyCode and BranchMD = @BranchCode) then cast(1 as bit) ELSE cast(0 as bit) END)

select BillType as BillType
              from svTrnSrvTask
             where CompanyCode = @companycode
               and BranchCode  = @branchcode
               and ProductType = @productType
               and ServiceNo   = @serviceno
            union
            select BillType as BillType
              from svTrnSrvItem b
             where CompanyCode = @companycode
               and BranchCode  = @branchcode
               and ProductType = @productType
               and ServiceNo   = @serviceno
               and  (SupplyQty - ReturnQty) > 0


-- get data from service
select * into #srv from(
  select * from svTrnService
   where 1 = 1
     and CompanyCode = @CompanyCode
     and BranchCode  = @BranchCode
     and ProductType = @ProductType
     and ServiceNo   = @ServiceNo
 )#srv

 select * from #srv
 select * from svTrnSrvItem where serviceno = @serviceno
 select * from svTrnSrvTask where serviceno = @serviceno

-- get data from task
select * into #tsk from(
  select a.* from svTrnSrvTask a, #srv b
   where 1 = 1
     and a.CompanyCode = b.CompanyCode
     and a.BranchCode  = b.BranchCode
     and a.ProductType = b.ProductType
     and a.ServiceNo   = b.ServiceNo
     and a.BillType    = @BillType
 )#tsk

 select * from #tsk

-- get data from item
select * into #mec from(
  select a.* from svTrnSrvMechanic a, #tsk b
   where 1 = 1
     and a.CompanyCode = b.CompanyCode
     and a.BranchCode  = b.BranchCode
     and a.ProductType = b.ProductType
     and a.ServiceNo   = b.ServiceNo
     and a.OperationNo = b.OperationNo
     and a.OperationNo <> ''
 )#mec

 select * from #mec

-- get data from item
select * into #itm from(
  select a.* from svTrnSrvItem a, #srv b
   where 1 = 1
     and a.CompanyCode = b.CompanyCode
     and a.BranchCode  = b.BranchCode
     and a.ProductType = b.ProductType
     and a.ServiceNo   = b.ServiceNo
     and a.BillType    = @BillType
 )#itm

-- create temporary table detail
create table #pre_dtl(
	BillType char(1),
	TaskPartType char(1),
	TaskPartNo varchar(20),
	TaskPartQty numeric(10,2),
	SupplySlipNo varchar(20)
)

insert into #pre_dtl
select BillType, 'L', OperationNo, OperationHour, ''
  from #tsk

insert into #pre_dtl
select BillType, TypeOfGoods, PartNo
	 , sum(SupplyQty - ReturnQty)
	 , SupplySlipNo
  from #itm
 where BillType = @BillType
   and (SupplyQty - ReturnQty) > 0
 group by BillType, TypeOfGoods, PartNo, SupplySlipNo

-- insert to table svTrnInvoice
declare @CustomerCode varchar(20)
if @BillType = 'C'
begin
	set @CustomerCode = (select CustomerCodeBill from #srv)
end
else if @BillType = 'P'
begin
	set @CustomerCode = (select top 1 a.BillTo from svMstPackage a
				 inner join svMstPackageTask b
					on b.CompanyCode = a.CompanyCode
				   and b.PackageCode = a.PackageCode
				 inner join svMstPackageContract c
					on c.CompanyCode = a.CompanyCode
				   and c.PackageCode = a.PackageCode
				 inner join #srv d
					on d.CompanyCode = a.CompanyCode
				   and d.JobType = a.JobType
				   and d.ChassisCode = c.ChassisCode
				   and d.ChassisNo = c.ChassisNo)
end
else if @BillType in ('F', 'W', 'S')
begin
	set @CustomerCode = (select CustomerCode from svMstBillingType
				 where BillType in ('F', 'W', 'S')
				   and CompanyCode = @CompanyCode
				   and BillType = @BillType)
end
else
begin
	set @CustomerCode = (select CustomerCodeBill from #srv)
end

--set @CustomerCode = isnull((
--				select top 1 a.BillTo from svMstPackage a
--				 inner join svMstPackageTask b
--					on b.CompanyCode = a.CompanyCode
--				   and b.PackageCode = a.PackageCode
--				 inner join svMstPackageContract c
--					on c.CompanyCode = a.CompanyCode
--				   and c.PackageCode = a.PackageCode
--				 inner join #srv d
--					on d.CompanyCode = a.CompanyCode
--				   and d.JobType = a.JobType
--				   and d.ChassisCode = c.ChassisCode
--				   and d.ChassisNo = c.ChassisNo
--				), isnull((
--				select CustomerCode from svMstBillingType
--				 where BillType in ('F')
--				   and CompanyCode = @CompanyCode
--				   and BillType = @BillType
--				), isnull((select CustomerCodeBill from #srv), '')))


if ((select count(*) from #tsk) = 0 and (select count(*) from #itm) = 0)
begin
	drop table #srv
	drop table #tsk
	drop table #mec
	drop table #itm
	drop table #pre_dtl
	return
end

if (@CustomerCode = '')
begin
	set @errmsg = N'Customer Code Bill belum di define...'
				+ char(13) + 'Tolong di check lagi'
				+ char(13) + 'Terima kasih'
	raiserror (@errmsg,16,1);
	return
end

select * into #cus from (
select a.CompanyCode, a.IsPkp, b.CustomerCode, b.LaborDiscPct, b.PartDiscPct, b.MaterialDiscPct, b.TopCode, b.TaxCode
  from gnMstCustomer a, gnMstCustomerProfitCenter b
 where 1 = 1
   and b.CompanyCode  = a.CompanyCode
   and b.CustomerCode = a.CustomerCode
   and b.CompanyCode  = @CompanyCode
   and b.BranchCode   = @BranchCode
   and b.CustomerCode = @CustomerCode
   and b.ProfitCenterCode = '200'
)#cus

if (select count(*) from #cus) <> 1
begin
	set @errmsg = N'Customer ProfitCenter belum di define...'
				+ char(13) + 'Tolong di check lagi'
				+ char(13) + 'Terima kasih'
	raiserror (@errmsg,16,1);
	return
end

declare @IsPKP bit
    set @IsPKP = isnull((
				 select IsPKP from gnMstCustomer
				  where CompanyCode  = @CompanyCode
				    and CustomerCode = @CustomerCode
				  ), 0)

declare @PPnPct decimal
    set @PPnPct = isnull((
				  select a.TaxPct
				    from gnMstTax a, #cus b
				   where 1 = 1
				     and b.TaxCode     = 'PPN'
				     and a.CompanyCode = b.CompanyCode
				     and a.TaxCode     = b.TaxCode
				  ), 0)

declare @PPhPct decimal
    set @PPhPct = isnull((
				  select a.TaxPct
				    from gnMstTax a, #cus b
				   where 1 = 1
				     and b.TaxCode     = 'PPH'
				     and a.CompanyCode = b.CompanyCode
				     and a.TaxCode     = b.TaxCode
				  ), 0)


-- Insert Into svTrnInvoice
-----------------------------------------------------------------------------------------
insert into svTrnInvoice(
  CompanyCode, BranchCode, ProductType
, InvoiceNo, InvoiceDate, InvoiceStatus
, FPJNo, FPJDate, JobOrderNo, JobOrderDate, JobType
, ServiceRequestDesc, ChassisCode, ChassisNo, EngineCode, EngineNo
, PoliceRegNo, BasicModel, CustomerCode, CustomerCodeBill, Odometer
, IsPKP, TOPCode, TOPDays, DueDate, SignedDate
, LaborDiscPct, PartsDiscPct, MaterialDiscPct, PphPct, PpnPct, Remarks
, PrintSeq, PostingFlag, IsLocked, CreatedBy, CreatedDate
) 
select
  @CompanyCode CompanyCode
, @BranchCode BranchCode
, @ProductType ProductType
, @InvoiceNo InvoiceNo
, getdate() InvoiceDate
, case @IsPKP
	when '0' then '1'
	else (case @BillType when 'F' then '0' when 'W' then '0' else '1' end)
  end as InvoiceStatus
, '' FPJNo
, null FPJDate
, (select JobOrderNo from #srv) JobOrderNo
, (select JobOrderDate from #srv) JobOrderDate
, (select JobType from #srv) JobType
, (select ServiceRequestDesc from #srv) ServiceRequestDesc
, (select ChassisCode from #srv) ChassisCode
, (select ChassisNo from #srv) ChassisNo
, (select EngineCode from #srv) EngineCode
, (select EngineNo from #srv) EngineNo
, (select PoliceRegNo from #srv) PoliceRegNo
, (select BasicModel from #srv) BasicModel
, (select CustomerCode from #srv) CustomerCode
, @CustomerCode as CustomerCodeBill
, (select Odometer from #srv) Odometer
, (select IsPKP from #cus) as IsPKP
, (select TopCode from #cus) as TOPCode
, isnull((
	select b.ParaValue
	  from gnMstCustomerProfitCenter a, GnMstLookUpDtl b
	 where a.CompanyCode  = @CompanyCode
	   and a.BranchCode   = @BranchCode
	   and a.CustomerCode = @CustomerCode
	   and a.ProfitCenterCode = '200'
	   and b.CompanyCode  = a.CompanyCode
	   and b.CodeID = 'TOPC'
	   and b.LookUpValue = a.TopCode
	), null) as TOPDays
, isnull((
	select dateadd(day, convert(int,b.ParaValue), convert(varchar, getdate(), 112))
	  from gnMstCustomerProfitCenter a, GnMstLookUpDtl b
	 where a.CompanyCode  = @CompanyCode
	   and a.BranchCode   = @BranchCode
	   and a.CustomerCode = @CustomerCode
	   and a.ProfitCenterCode = '200'
	   and b.CompanyCode  = a.CompanyCode
	   and b.CodeID = 'TOPC'
	   and b.LookUpValue  = a.TopCode
	), null) as DueDate
, convert(varchar, getdate(), 112) SignedDate
, case @BillType
	when 'F' then (select LaborDiscPct from #cus) 
    when 'W' then (select LaborDiscPct from #cus) 
    else (select LaborDiscPct from #srv) 
  end as LaborDiscPct
, case @BillType
	when 'F' then (select PartDiscPct from #cus) 
    when 'W' then (select PartDiscPct from #cus) 
    else (select PartDiscPct from #srv) 
  end as PartsDiscPct
, case @BillType
	when 'F' then (select MaterialDiscPct from #cus) 
    when 'W' then (select MaterialDiscPct from #cus) 
    else (select MaterialDiscPct from #srv) 
  end as MaterialDiscPct
, @PPnPct as PPhPct
, @PPnPct as PPnPct
, @Remarks as Remarks
, '0' PrintSeq
, '0' PostingFlag
, '0' IsLocked
, @UserID CreatedBy
, getdate() CreatedDate

-- Insert Into svTrnInvTask
-----------------------------------------------------------------------------------------
insert into svTrnInvTask (
  CompanyCode, BranchCode, ProductType, InvoiceNo, OperationNo
, OperationHour, ClaimHour, OperationCost, SubConPrice
, IsSubCon, SharingTask, DiscPct
)
select CompanyCode, BranchCode, ProductType, @InvoiceNo as InvoiceNo, OperationNo
, isnull(OperationHour, 0) OperationHour, isnull(ClaimHour, 0) ClaimHour
, isnull(OperationCost, 0) OperationCost, isnull(SubConPrice, 0) SubConPrice
, isnull(IsSubCon, 0) IsSubCon, isnull(SharingTask, 0) SharingTask
, isnull(DiscPct, 0)
from #tsk

-- Insert Into svTrnInvMechanic
-----------------------------------------------------------------------------------------
insert into svTrnInvMechanic (
  CompanyCode, BranchCode, ProductType, InvoiceNo, OperationNo
, MechanicID, ChiefMechanicID, StartService, FinishService
)
select CompanyCode, BranchCode, ProductType, @InvoiceNo as InvoiceNo, OperationNo
, MechanicID, ChiefMechanicID, StartService, FinishService
from #mec

-- Insert Into svTrnInvItem
-----------------------------------------------------------------------------------------
Declare @Query varchar(max)

set @Query = 'select * into #itm1 from (
select CompanyCode, BranchCode, ProductType, '''+ @InvoiceNo +''' as InvoiceNo, PartNo
	 , isnull((
		select MovingCode from '+ @DbMD +'..spMstItems
		 where CompanyCode = '''+ @CompanyMD +'''
		   and BranchCode = '''+ @BranchMD +'''
		   and PartNo = #itm.PartNo
		), '''') as MovingCode
	 , isnull((
		select ABCClass from '+ @DbMD +' ..spMstItems
		 where CompanyCode = '''+ @CompanyMD +'''
		   and BranchCode = '''+ @BranchMD +'''
		   and PartNo = #itm.PartNo
		), '''') as ABCClass
	 , sum(SupplyQty - ReturnQty) as SupplyQty
	 , isnull((
		select 
		  case (sum(b.SupplyQty - b.ReturnQty))
			 when 0 then 0
			 else sum(a.CostPrice * (b.SupplyQty - b.ReturnQty)) / sum(b.SupplyQty - b.ReturnQty)
		  end 
	from SpTrnSLmpDtl a
	left join SvTrnSrvItem b on 1 = 1
	 and b.CompanyCode  = a.CompanyCode
	 and b.BranchCode   = a.BranchCode
	 and b.ProductType  = a.ProductType
	 and b.SupplySlipNo = a.DocNo
	 and b.PartNo = #itm.PartNo
	where 1 = 1
	 and a.CompanyCode = '''+ @CompanyCode +'''
	 and a.BranchCode  = '''+ @BranchCode +'''
	 and a.ProductType = '''+ @ProductType +'''
	 and a.PartNo = #itm.PartNo
	 and a.DocNo in (
			select SupplySlipNo
			 from SvTrnSrvItem
			where 1 = 1
			  and CompanyCode = '''+ @CompanyCode +'''
			  and BranchCode  = '''+ @BranchCode +'''
			  and ProductType = '''+ @ProductType +'''
			  and ServiceNo = '''+ Convert(varchar,@ServiceNo) +'''
			  and PartNo = #itm.PartNo
			)
	), 0) as CostPrice
, RetailPrice
, TypeOfGoods
from #itm
group by CompanyCode, BranchCode, ProductType, PartNo, RetailPrice, TypeOfGoods
)#

insert into svTrnInvItem (
  CompanyCode, BranchCode, ProductType, InvoiceNo, PartNo
, MovingCode, ABCClass, SupplyQty, ReturnQty, CostPrice, RetailPrice
, TypeOfGoods, DiscPct
)
select a.CompanyCode, a.BranchCode, a.ProductType, a.InvoiceNo, a.PartNo
	 , MovingCode = (select top 1 MovingCode from #itm1 where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InvoiceNo = a.InvoiceNo and PartNo = a.PartNo)
	 , ABCClass = (select top 1 ABCClass from #itm1 where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InvoiceNo = a.InvoiceNo and PartNo = a.PartNo)
	 , sum(SupplyQty) as SupplyQty, 0 as ReturnQty
	 , CostPrice = (select top 1 CostPrice from #itm1 where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InvoiceNo = a.InvoiceNo and PartNo = a.PartNo order by CostPrice desc)
	 , RetailPrice = (select top 1 RetailPrice from #itm1 where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InvoiceNo = a.InvoiceNo and PartNo = a.PartNo order by RetailPrice desc)
	 , TypeOfGoods = (select top 1 TypeOfGoods from #itm1 where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InvoiceNo = a.InvoiceNo and PartNo = a.PartNo)
	 , DiscPct = (select top 1 DiscPct from #itm where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InvoiceNo = a.InvoiceNo and PartNo = a.PartNo)
  from #itm1 a
 where a.SupplyQty > 0
 group by a.CompanyCode, a.BranchCode, a.ProductType, a.InvoiceNo, a.PartNo'

 exec(@Query)

-- Insert Into svTrnInvItemDtl
-----------------------------------------------------------------------------------------
insert into svTrnInvItemDtl (
  CompanyCode, BranchCode, ProductType, InvoiceNo, PartNo, SupplySlipNo
, SupplyQty, CostPrice, CreatedBy, CreatedDate
)
select y.* from (
select CompanyCode, BranchCode, ProductType, @InvoiceNo as InvoiceNo, PartNo, SupplySlipNo
, sum(SupplyQty - ReturnQty) as SupplyQty, CostPrice
, @UserID as CreatedBy, getdate() as CreatedDate
from #itm
group by CompanyCode, BranchCode, ProductType, PartNo, SupplySlipNo, CostPrice
) y
where y.SupplyQty > 0

-- Re Calculate Invoice

-----------------------------------------------------------------------------------------
exec uspfn_SvTrnInvoiceReCalculate @CompanyCode=@CompanyCode, @BranchCode=@BranchCode, @ProductType=@ProductType, @InvoiceNo=@InvoiceNo, @UserId=@UserId
-- Insert svsdmovement
-----------------------------------------------------------------------------------------

 if(@md = 0)
 begin

 set @Query ='insert into '+ @DbMD +'..svSDMovement
select a.CompanyCode, a.BranchCode, a.InvoiceNo,a.InvoiceDate, b.PartNo
, Seq = convert(integer, ROW_NUMBER() OVER (PARTITION BY b.PartNo order by b.PartNo)) ,
''00'', b.SupplyQty, b.SupplyQty, b.DiscPct, b.CostPrice, b.RetailPrice, b.TypeOfGoods
, '''+ @CompanyMD +''','''+ @BranchMD +''','''+ @WarehouseMD +''', p.RetailPriceInclTax, p.RetailPrice, p.CostPrice
,''x'','''+ @producttype +''',''300'',''8'',''0'','''+ convert(varchar,GETDATE()) +''','''+ @UserID +''','''+ convert(varchar,GETDATE()) +'''
,'''+ @UserID +''','''+ convert(varchar,GETDATE()) +'''
from svTrnInvoice a
join svTrnInvItem b on a.CompanyCode = b.CompanyCode and a.BranchCode =  b.BranchCode and a.ProductType = b.ProductType and a.InvoiceNo = b.InvoiceNo 
join '+ @DbMD +'..spmstitemprice p
on p.CompanyCode = '''+ @CompanyMD +''' and p.BranchCode = '''+ @BranchMD +''' and p.PartNo = b.PartNo
where a.CompanyCode = '''+ @CompanyCode +'''
and a.branchcode = '''+ @BranchCode +'''
and a.InvoiceNo = '''+ convert(varchar,@InvoiceNo) +''''

exec (@Query)

end

drop table #srv
drop table #tsk
drop table #mec
drop table #itm
drop table #cus

drop table #pre_dtl
--rollback tran

