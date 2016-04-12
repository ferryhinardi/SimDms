ALTER procedure [dbo].[uspfn_SvTrnServiceSaveItem]  
 @CompanyCode varchar(15),  
 @BranchCode varchar(15),  
 @ProductType varchar(15),  
 @ServiceNo bigint,  
 @BillType varchar(15),  
    @PartNo varchar(20),  
    @DemandQty numeric(18,2),  
    @PartSeq numeric(5,2),  
 @UserID varchar(15),  
 @DiscPct numeric(5,2)  
  
as        
  
--declare  @CompanyCode varchar(15),  
-- @BranchCode varchar(15),  
-- @ProductType varchar(15),  
-- @ServiceNo bigint,  
-- @BillType varchar(15),  
--    @PartNo varchar(20),  
--    @DemandQty numeric(18,2),  
--    @PartSeq numeric(5,2),  
-- @UserID varchar(15),  
-- @DiscPct numeric(5,2)  
  
--set @CompanyCode = '6006400001'  
--set @BranchCode = '6006400101'  
--set @ProductType = '4W'  
--set @ServiceNo = 43545  
--set @BillType = 'C'  
--set @PartNo = '99000-990C5-A03'  
--set @DemandQty = 2  
--set @PartSeq = -1  
--set @UserID = 'demo'  
--set @DiscPct = 0  
  
  
declare @errmsg varchar(max)  
declare @QueryTemp as varchar(max)  
  
begin try  
 -- select data svTrnService  
 select * into #srv from (  
   select a.* from svTrnService a  
  where 1 = 1  
    and a.CompanyCode = @CompanyCode  
    and a.BranchCode  = @BranchCode  
    and a.ProductType = @ProductType  
    and a.ServiceNo   = @ServiceNo  
 )#srv  
   
 declare @CostPrice as decimal  
 declare @RetailPrice as decimal  
 declare @TypeOfGoods as varchar(2)  
 declare @CostPriceMD as decimal  
 declare @RetailPriceMD as decimal  
 declare @RetailPriceInclTaxMD as decimal  
   
 declare @DealerCode as varchar(2)  
 declare @CompanyMD as varchar(15)  
 declare @BranchMD as varchar(15)  
 declare @WarehouseMD as varchar(15)  
  
 set @DealerCode = 'MD'  
 set @CompanyMD = (select CompanyMD from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)  
 set @BranchMD = (select BranchMD from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)  
 set @WarehouseMD = (select WarehouseMD from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)  
 
if object_id('#tmpSvSDMovement') is not null drop table #tmpSvSDMovement
   
 if (@CompanyCode != @CompanyMD and @BranchCode != @BranchMD)   
 begin  
  set @DealerCode = 'SD'  
  
  declare @DbName as varchar(50)  
  set @DbName = (select DbMD from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)  
  
  declare @PurchaseDisc as decimal  
  set @PurchaseDisc = (select isnull(DiscPct, 0) from gnMstSupplierProfitCenter   
       where CompanyCode = @CompanyCode   
       and BranchCode = @BranchCode  
       and SupplierCode = @CompanyMD 
       and ProfitCenterCode = '200')  
         
  if (@PurchaseDisc is null) raiserror ('Purchase Discount belum di-setting untuk Part tersebut!',16,1);            
       
  declare @tblTemp as table  
  (  
   CostPrice decimal(18,2),  
   RetailPrice decimal(18,2),  
   RetailPriceInclTax decimal(18,2),  
   TypeOfGoods varchar (2)  
  )  
       
  set @QueryTemp = 'select   
    a.CostPrice   
   ,a.RetailPrice  
   ,a.RetailPriceInclTax  
   ,b.TypeOfGoods  
     from ' + @DbName + '..spMstItemPrice a, ' + @DbName +'..spMstItems b  
   where 1 = 1  
     and a.CompanyCode = b.CompanyCode  
     and a.BranchCode = b.BranchCode  
     and a.PartNo = b.PartNo  
     and a.CompanyCode = ''' + @CompanyMD + '''  
     and a.BranchCode  = ''' + @BranchMD + '''  
     and a.PartNo      = ''' + @PartNo + ''''  
          
  insert into @tblTemp    
  exec (@QueryTemp)  
  
  --select * into #part1 from (  
  --select * from @tblTemp  
  --)#part1  
    
  set @CostPrice = ((select RetailPrice from @tblTemp) / 1.1 * ((100 - @PurchaseDisc) / 100))  
  --select @CostPrice  
  set @RetailPrice = (select a.RetailPrice from spMstItemPrice a where a.CompanyCode = @CompanyCode and a.BranchCode = @BranchCode and a.PartNo = @PartNo)    
  set @TypeOfGoods = (select TypeOfGoods from @tblTemp)  
    
  set @CostPriceMD = (select CostPrice from @tblTemp)  
  set @RetailPriceMD = (select RetailPrice from @tblTemp)  
  set @RetailPriceInclTaxMD = (select RetailPriceInclTax from @tblTemp)  
    
  -- select @PurchaseDisc  
 end   
 else  
 begin  
  select * into #part from (  
  select   
    a.CostPrice   
   ,a.RetailPrice  
    from spMstItemPrice a  
   where 1 = 1  
     and a.CompanyCode = @CompanyCode  
     and a.BranchCode  = @BranchCode  
     and a.PartNo      = @PartNo  
  )#part  
    
  set @CostPrice = (select CostPrice from #part)  
  set @RetailPrice = (select RetailPrice from #part)  
 end  
      
 if (@PartSeq > 0)  
 begin    
  -- select data mst job  
  select * into #job from (  
  select b.*  
    from svTrnService a, svMstJob b  
   where 1 = 1  
     and b.CompanyCode = a.CompanyCode  
     and b.ProductType = a.ProductType  
     and b.BasicModel = a.BasicModel  
     and b.JobType = a.JobType  
        and a.CompanyCode = @CompanyCode  
     and a.BranchCode  = @BranchCode  
     and a.ServiceNo   = @ServiceNo  
     and b.GroupJobType = 'FSC'  
  )#  
  if exists (select * from #job)  
  begin  
   -- update svTrnSrvItem  
   update svTrnSrvItem set  
     DemandQty      = @DemandQty  
    ,CostPrice      = @CostPrice  
    ,RetailPrice    = isnull((  
         select top 1 b.RetailPrice from #srv a, svMstTaskPart b  
          where b.CompanyCode = a.CompanyCode  
            and b.ProductType = a.ProductType  
            and b.BasicModel = a.BasicModel  
            and b.JobType = a.JobType  
            and b.PartNo = @PartNo  
            and b.BillType = 'F'  
         ), (  
          select top 1 isnull(RetailPrice, 0) RetailPrice  
            from spMstItemPrice  
           where 1 = 1  
             and CompanyCode = @CompanyCode  
             and BranchCode = @BranchCode  
             and PartNo = @PartNo  
          )  
         )  
    ,LastupdateBy   = (select LastupdateBy from #srv)  
    ,LastupdateDate = (select LastupdateDate from #srv)  
    ,BillType       = @BillType  
    ,DiscPct        = @DiscPct  
    where 1 = 1  
      and CompanyCode  = @CompanyCode  
      and BranchCode   = @BranchCode  
      and ProductType  = (select ProductType from #srv)  
      and ServiceNo    = (select ServiceNo from #srv)  
      and PartNo       = @PartNo  
      and PartSeq      = @PartSeq  
  end  
  else  
  begin  
   -- update svTrnSrvItem  
   update svTrnSrvItem set  
     DemandQty      = @DemandQty  
    ,CostPrice      = @CostPrice  
    ,RetailPrice    = @RetailPrice  
    ,LastupdateBy   = (select LastupdateBy from #srv)  
    ,LastupdateDate = (select LastupdateDate from #srv)  
    ,BillType       = @BillType  
    ,DiscPct        = @DiscPct  
    where 1 = 1  
      and CompanyCode  = @CompanyCode  
      and BranchCode   = @BranchCode  
      and ProductType  = (select ProductType from #srv)  
      and ServiceNo    = (select ServiceNo from #srv)  
      and PartNo       = @PartNo  
      and PartSeq      = @PartSeq           
  end   
    
  --update svSDMovement  
   update svSDMovement set    
    QtyOrder    = @DemandQty  
    ,DiscPct    = @DiscPct  
    ,CostPrice    = @CostPrice  
    ,RetailPrice   = @RetailPrice  
    ,CostPriceMD   = @CostPriceMD  
    ,RetailPriceMD   = @RetailPriceMD  
    ,RetailPriceInclTaxMD = @RetailPriceInclTaxMD  
    ,[Status]  = (select ServiceStatus from #srv)  
    ,LastupdateBy   = (select LastupdateBy from #srv)  
    ,LastupdateDate = (select LastupdateDate from #srv)  
   where CompanyCode = @CompanyCode  
      and BranchCode = @BranchCode  
      and DocNo  = (select JobOrderNo from #srv)  
      and PartNo       = @PartNo  
      and PartSeq      = @PartSeq  
 end  
 else  
 begin  
  if((select count(*) from svTrnSrvItem  
    where 1 = 1  
      and CompanyCode  = @CompanyCode  
      and BranchCode   = @BranchCode  
      and ProductType  = (select ProductType from #srv)  
      and ServiceNo    = (select ServiceNo from #srv)  
      and PartNo       = @PartNo  
      and (isnull(SupplySlipNo,'') = '')  
   ) > 0)  
  begin  
   raiserror ('Part yang sama sudah diproses di Entry SPK namun belum dapat No SSS, mohon diselesaikan dahulu!',16,1);  
  end  
  
  declare @PartSeqNew as int  
  set @PartSeqNew = (isnull((select isnull(max(PartSeq), 0) + 1    
      from svTrnSrvItem   
        where CompanyCode = @CompanyCode  
       and BranchCode  = @BranchCode   
       and ProductType = @ProductType  
       and ServiceNo   = @ServiceNo  
       and PartNo      = @PartNo), 1))  
         
  -- insert svTrnSrvItem  
  insert into svTrnSrvItem (CompanyCode, BranchCode, ProductType, ServiceNo, PartNo, PartSeq, DemandQty, SupplyQty, ReturnQty, CostPrice, RetailPrice, TypeOfGoods, BillType, SupplySlipNo, SupplySlipDate, SSReturnNo, SSReturnDate, CreatedBy, CreatedDate, LastupdateBy, LastupdateDate, DiscPct, MechanicID)  
  select   
    @CompanyCode CompanyCode  
   ,@BranchCode BranchCode  
   ,@ProductType ProductType  
   ,@ServiceNo ServiceNo  
   ,a.PartNo  
   ,@PartSeqNew  
   --,(row_number() over (order by a.PartNo)) PartSeq  
   ,@DemandQty DemandQty  
   ,'0' SupplyQty  
   ,'0' ReturnQty  
   ,@CostPrice  
   ,a.RetailPrice   
   ,b.TypeOfGoods  
   ,@BillType BillType  
   ,null SupplySlipNo  
   ,null SupplySlipDate  
   ,null SSReturnNo  
   ,null SSReturnDate  
   ,c.LastupdateBy CreatedBy  
   ,c.LastupdateDate CreatedDate  
   ,c.LastupdateBy  
   ,c.LastupdateDate  
   ,@DiscPct  
   ,(select MechanicID from svTrnService where CompanyCode = @CompanyCode and BranchCode = @BranchCode and ServiceNo = @ServiceNo)  
    from spMstItemPrice a,spMstItems b, #srv c  
   where 1 = 1  
        and a.CompanyCode = b.CompanyCode  
     and a.BranchCode  = b.BranchCode  
        and b.CompanyCode = c.CompanyCode  
     and b.BranchCode  = c.BranchCode  
     and b.PartNo      = a.PartNo  
        and a.CompanyCode = @CompanyCode  
     and a.BranchCode  = @BranchCode  
     and a.PartNo      = @PartNo  
       
  --select   @CostPrice   
  if (@DealerCode = 'SD')  
  begin
	create table #tmpSvSDMovement(
		CompanyCode varchar(15)
		,BranchCode varchar(15)
		,JobOrderNo varchar(20)   
		,JobOrderDate datetime  
		,PartNo varchar(20)
		,PartSeqNew int
		,WarehouseMD varchar(20)   
		,DemandQty numeric(18,2)
		,Qty numeric(18,2)
		,DiscPct numeric(18,2)
		,CostPrice numeric(18,2)
		,RetailPrice numeric(18,2) 
		,TypeOfGoods varchar(15) 
		,CompanyMD varchar(15)
		,BranchMD varchar(15)   
		,WarehouseMD2 varchar(15)
		,RetailPriceInclTaxMD numeric(18,2) 
		,RetailPriceMD numeric(18,2) 
		,CostPriceMD numeric(18,2)  
		,QtyFlag char(1)
		,ProductType varchar(15) 
		,ProfitCenterCode varchar(15)
		,Status char(1)
		,ProcessStatus char(1)
		,ProcessDate datetime 
		,CreatedBy varchar(15) 
		,CreatedDate datetime 
		,LastUpdateBy varchar(15) 
		,LastUpdateDate datetime	
	);
  
  
	insert into #tmpSvSDMovement 
		select case when @CompanyCode is null then '' else @CompanyCode end 
		,case when @BranchCode is null then '' else @BranchCode end
		,case when (select JobOrderNo from #srv) is null then '' else (select JobOrderNo from #srv) end
		,case when (select JobOrderDate from #srv) is null then '1900/01/01' else (select JobOrderDate from #srv) end 
		,case when @PartNo is null then '' else  @PartNo end 
		,case when @PartSeqNew is null then '0' else convert(varchar, @PartSeqNew) end
		,case when @WarehouseMD is null then '' else @WarehouseMD end  
		,case when @DemandQty  is null then '0' else convert(varchar, @DemandQty) end
		,'0'  
		,case when @DiscPct is null then '0' else convert(varchar, @DiscPct) end  
		,case when @CostPrice is null then '0' else convert(varchar, @CostPrice) end 
		,case when @RetailPrice is null then '0' else convert(varchar, @RetailPrice) end  
		,case when @TypeOfGoods is null then '' else @TypeOfGoods end 
		,case when @CompanyMD is null then '' else @CompanyMD end   
		,case when @BranchMD is null then '' else @BranchMD end  
		,case when @WarehouseMD is null then '' else @WarehouseMD end  
		,case when @RetailPriceInclTaxMD is null then '0' else convert(varchar, @RetailPriceInclTaxMD) end  
		,case when @RetailPriceMD is null then '0' else convert(varchar, @RetailPriceMD) end   
		,case when @CostPriceMD is null then '0' else convert(varchar, @CostPriceMD) end
		,'x'
		,case when @ProductType is null then '' else @ProductType end  
		,'300'  
		,'0' 
		,'0'
		,'1900/01/01'  
		,case when (select CreatedBy from #srv) is null then '' else (select CreatedBy from #srv) end     
		,case when (select CreatedDate from #srv) is null then '1900/01/01' else convert(varchar,(select CreatedDate from #srv)) end 
		,case when (select LastUpdateBy from #srv) is null then '' else (select LastUpdateBy from #srv) end
		,case when (select LastUpdateDate from #srv) is null then '1900/01/01' else convert(varchar,(select LastUpdateDate from #srv)) end
     
	set @QueryTemp = '
	insert into ' + @DbName + '..svSDMovement (CompanyCode, BranchCode, DocNo, DocDate, PartNo, PartSeq, WarehouseCode, QtyOrder, Qty, DiscPct, CostPrice, RetailPrice,   
	TypeOfGoods, CompanyMD, BranchMD, WarehouseMD, RetailPriceInclTaxMD, RetailPriceMD, CostPriceMD, QtyFlag, ProductType, ProfitCenterCode, 
	Status, ProcessStatus, ProcessDate, CreatedBy,   
	CreatedDate, LastUpdateBy, LastUpdateDate)  
	select * from #tmpSvSDMovement';
	exec(@QueryTemp);
	
	drop table #tmpSvSDMovement;     
  
  end   
 end  
   
 update svTrnSrvItem  
    set DiscPct = @DiscPct  
  where 1 = 1  
    and CompanyCode = @CompanyCode  
    and BranchCode = @BranchCode  
    and ProductType = @ProductType  
    and ServiceNo = @ServiceNo  
    and PartNo = @PartNo  
   
 if (@DealerCode = 'SD')  
 begin    
	set @QueryTemp = 'update ' + @DbName + '..svSDMovement   
	  set DiscPct = ' + convert(varchar,@DiscPct) 
	  + ' where 1 = 1'  
	  +	' and CompanyCode = ''' + case when @CompanyMD is null then '''' else  @CompanyMD end + ''''
	  + ' and BranchCode = ''' + case when @BranchMD is null then '''' else  @BranchMD end + ''''
	  + ' and DocNo = ''' + case when (select JobOrderNo from #srv) is null then '''' else (select JobOrderNo from #srv) end  + ''''
	  + ' and PartNo = ''' + case when @PartNo is null then '''' else @PartNo end + ''''  
	  + ' and PartSeq = ' + convert(varchar,@PartSeq) + '';
  
	exec (@QueryTemp)  
 end  
   
	drop table #srv  
end try  
begin catch  
 set @errmsg = error_message()  
 raiserror (@errmsg,16,1);  
end catch  