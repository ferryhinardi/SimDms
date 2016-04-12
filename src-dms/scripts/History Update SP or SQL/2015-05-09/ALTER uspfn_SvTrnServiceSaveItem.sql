ALTER procedure [dbo].[uspfn_SvTrnServiceSaveItem]  
--DECLARE
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
  
--set @CompanyCode = '6115204001'  
--set @BranchCode = '6115204102'  
--set @ProductType = '2W'  
--set @ServiceNo = 16455  
--set @BillType = 'C'  
--set @PartNo = 'K1200-50002-000'  
--set @DemandQty = 1 
--set @PartSeq = -1  
--set @UserID = 'yo'  
--set @DiscPct = 0  

declare @errmsg varchar(max)  
declare @QueryTemp as varchar(max)  
declare @IsSPK as char(1)
  
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
 
 -- Check MD or SD
	-- If SD  
 if (@CompanyCode != @CompanyMD and @BranchCode != @BranchMD)   
 begin  
	  set @DealerCode = 'SD'  

	  set @IsSPK = isnull((select a.ServiceType from #srv a where a.ServiceType = '2'),0)
	  
	  declare @DbName as varchar(50)  
	  set @DbName = (select DbMD from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)  
	  
	  declare @PurchaseDisc as decimal  
	  set @PurchaseDisc = (select isnull(DiscPct, 0) from gnMstSupplierProfitCenter   
		   where CompanyCode = @CompanyCode   
		   and BranchCode = @BranchCode  
		   and SupplierCode = @BranchMD
		   and ProfitCenterCode = '300')  
	         
	  if (@PurchaseDisc = 0) raiserror ('Purchase Discount belum di-setting untuk Part tersebut!',16,1);            
	       
	  declare @tblTemp as table  
	  (  
	   CostPrice decimal(18,2),  
	   RetailPrice decimal(18,2),  
	   RetailPriceInclTax decimal(18,2),  
	   TypeOfGoods varchar (2)  
	  )
	  
	  declare @tblTemp1 as table  
	  (  
	   CostPrice decimal(18,2),  
	   RetailPrice decimal(18,2),  
	   RetailPriceInclTax decimal(18,2),  
	   TypeOfGoods varchar (2)  
	  )
	    
	  -- Untuk ItemPrice Mengambil dari masing-masing dealer	
		set @QueryTemp = 'select   
			  b.CostPrice   
			 ,b.RetailPrice  
			 ,b.RetailPriceInclTax  
			 ,a.TypeOfGoods 
			from (select
				i.PartNo   
				,i.TypeOfGoods  
				 from ' + @DbName +'..spMstItems i  
				 where i.CompanyCode = ''' + @CompanyMD + '''  
				 and i.BranchCode  = ''' + @BranchMD + '''  
				 and i.PartNo      = ''' + @PartNo + '''
			) a inner join spMstItemPrice b on b.PartNo = a.PartNo
		 where b.CompanyCode = ''' + @CompanyCode + '''
		 and b.BranchCode = ''' + @BranchCode + ''''
		          
	  insert into @tblTemp    
	  exec (@QueryTemp)  
	  
		set @QueryTemp = 'select   
			  b.CostPrice   
			 ,b.RetailPrice  
			 ,b.RetailPriceInclTax  
			 ,a.TypeOfGoods 
			from (select
				i.PartNo   
				,i.TypeOfGoods  
				 from ' + @DbName +'..spMstItems i  
				 where i.CompanyCode = ''' + @CompanyMD + '''  
				 and i.BranchCode  = ''' + @BranchMD + '''  
				 and i.PartNo      = ''' + @PartNo + '''
			) a inner join ' + @DbName +'..spMstItemPrice b on b.PartNo = a.PartNo
		 where b.CompanyCode = ''' + @CompanyMD + '''
		 and b.BranchCode = ''' + @BranchMD + ''''
		 
  	  insert into @tblTemp1
	  exec (@QueryTemp)  
	  print (@QueryTemp)  
	  
	  set @CostPrice = ((select RetailPriceInclTax from @tblTemp1) - ((select RetailPriceInclTax from @tblTemp1) * @PurchaseDisc * 0.01))  
	  select @CostPrice  
	  set @RetailPrice = (select RetailPrice from @tblTemp)
	  --select a.RetailPrice from spMstItemPrice a where a.CompanyCode = @CompanyCode and a.BranchCode = @BranchCode and a.PartNo = @PartNo)    
	  set @TypeOfGoods = (select TypeOfGoods from @tblTemp)  
	    
	  set @CostPriceMD = (select CostPrice from @tblTemp)  
	  set @RetailPriceMD = (select RetailPrice from @tblTemp)  
	  set @RetailPriceInclTaxMD = (select RetailPriceInclTax from @tblTemp)  
	    
	  -- select @PurchaseDisc  
 end   
 -- If MD
 else  
 begin
	 declare @tblTempPart as table  
	  (  
	   CostPrice decimal(18,2),  
	   RetailPrice decimal(18,2),  
	   RetailPriceInclTax decimal(18,2),  
	   TypeOfGoods varchar (2)  
	  )  

	  set @QueryTemp = 'select   
		a.CostPrice   
	   ,a.RetailPrice  
		 from ' + @DbName + '..spMstItemPrice a  
	   where 1 = 1  
		 and a.CompanyCode = ''' + @CompanyMD + '''  
		 and a.BranchCode  = ''' + @BranchMD + '''  
		 and a.PartNo      = ''' + @PartNo + ''''  
	          
	  insert into @tblTempPart    
	  exec (@QueryTemp)  
	   
	  --select * into #part from (  
	  --select   
	  --  a.CostPrice   
	  -- ,a.RetailPrice  
	  --  from spMstItemPrice a  
	  -- where 1 = 1  
	  --   and a.CompanyCode = @CompanyCode  
	  --   and a.BranchCode  = @BranchCode  
	  --   and a.PartNo      = @PartNo  
	  --)#part  
	    
	  set @CostPrice = (select CostPrice from @tblTempPart)  
	  set @RetailPrice = (select RetailPrice from @tblTempPart)  
 end  
 -- EOF Check MD or SD
  
 
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
	   set @Querytemp ='
	   update svTrnSrvItem set  
		 DemandQty      = '+ convert(varchar,@DemandQty) +'
		,CostPrice      = '+ convert(varchar,@CostPrice) +' 
		,RetailPrice    = isnull((  
			 select top 1 b.RetailPrice from #srv a, svMstTaskPart b  
			  where b.CompanyCode = a.CompanyCode  
				and b.ProductType = a.ProductType  
				and b.BasicModel = a.BasicModel  
				and b.JobType = a.JobType  
				and b.PartNo = '''+ @PartNo +''' 
				and b.BillType = ''F'' 
			 ), (  
			  select top 1 isnull(RetailPrice, 0) RetailPrice  
				from spMstItemPrice  
			   where 1 = 1  
				 and CompanyCode = '''+ @CompanyCode +'''
				 and BranchCode = '''+ @BranchCode +'''
				 and PartNo = '''+ @PartNo  +'''
			  )  
			 )  
		,LastupdateBy   = (select LastupdateBy from #srv)  
		,LastupdateDate = (select LastupdateDate from #srv)  
		,BillType       = '''+ @BillType +'''
		,DiscPct        = '+ convert(varchar,@DiscPct) +'  
		where 1 = 1  
		  and CompanyCode  = '''+ @CompanyCode +''' 
		  and BranchCode   = '''+ @BranchCode +''' 
		  and ProductType  = (select ProductType from #srv)  
		  and ServiceNo    = (select ServiceNo from #srv)  
		  and PartNo       = '''+ @PartNo +''' 
		  and PartSeq      = '+ convert(varchar,@PartSeq) +'' 
		  
		  exec(@QueryTemp) 
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
	if (@DealerCode = 'SD' and @IsSPK = '2')  
	begin    
		set @QueryTemp = 'update ' + @DbName + '..svSDMovement set    
		QtyOrder    = ' + case when @DemandQty is null then '0' else convert(varchar, @DemandQty) end + ' 
		,DiscPct    = ' + case when  @DiscPct is null then '0' else convert(varchar, @DiscPct) end + '
		,CostPrice    = ' + case when @CostPrice is null then '0' else convert(varchar, @CostPrice) end + '  
		,RetailPrice   = ' + case when @RetailPrice is null then '0' else convert(varchar, @RetailPrice) end + '  
		,CostPriceMD   = ' + case when @CostPriceMD is null then '0' else convert(varchar, @CostPriceMD) end + '  
		,RetailPriceMD   = ' + case when @RetailPriceMD is null then '0' else convert(varchar, @RetailPriceMD) end + '  
		,RetailPriceInclTaxMD = ' + case when @RetailPriceInclTaxMD is null then '0' else convert(varchar, @RetailPriceInclTaxMD) end + '  
		,[Status]  = ''' + case when (select ServiceStatus from #srv) is null then '''' else (select ServiceStatus from #srv) end + '''  
		,LastupdateBy   = ''' + case when (select LastupdateBy from #srv) is null then '''' else (select LastupdateBy from #srv) end + '''  
		,LastupdateDate = ''' + case when (select LastupdateDate from #srv) is null then '''' else convert(varchar,(select LastupdateDate from #srv)) end + '''  
		where CompanyCode = ''' + case when @CompanyCode is null then '''' else @CompanyCode end + '''
		  and BranchCode = ''' + case when @BranchCode is null then '''' else @BranchCode end + '''
		  and DocNo  = ''' + case when (select JobOrderNo from #srv) is null then '''' else (select JobOrderNo from #srv) end + '''  
		  and PartNo       =  ''' + case when @PartNo is null then '''' else @PartNo end  + '''
		  and PartSeq      = ' + case when @PartSeq is null then '0' else convert(varchar, @PartSeq) end + '';  
		  
		  --print @QueryTemp;  
		exec 	(@QueryTemp);
	end
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
	set @QueryTemp=' insert into svTrnSrvItem (CompanyCode, BranchCode, ProductType, ServiceNo, PartNo, PartSeq, DemandQty, SupplyQty, ReturnQty, CostPrice, RetailPrice, TypeOfGoods, BillType, SupplySlipNo, SupplySlipDate, SSReturnNo, SSReturnDate, CreatedBy, CreatedDate, LastupdateBy, LastupdateDate, DiscPct, MechanicID)  
	select   
	'''+ @CompanyCode +''' CompanyCode  
	,''' + @BranchCode +''' BranchCode  
	,'''+ @ProductType +''' ProductType  
	,'+ convert(varchar,@ServiceNo) +' ServiceNo  
	,a.PartNo  
	,'''+ convert(varchar,@PartSeqNew)  +'''
	--,(row_number() over (order by a.PartNo)) PartSeq  
	,'+ convert(varchar,@DemandQty )+' DemandQty  
	,''0'' SupplyQty  
	,''0'' ReturnQty  
	,'+ convert(varchar,isnull(@CostPrice,0))  +'
	,a.RetailPrice   
	,b.TypeOfGoods  
	,'''+ @BillType +''' BillType  
	,null SupplySlipNo  
	,null SupplySlipDate  
	,null SSReturnNo  
	,null SSReturnDate  
	,c.LastupdateBy CreatedBy  
	,c.LastupdateDate CreatedDate  
	,c.LastupdateBy  
	,c.LastupdateDate  
	,'+ convert(varchar,isnull(@DiscPct,0))  +'
	,(select MechanicID from svTrnService where CompanyCode = '''+ @CompanyCode +''' and BranchCode = '''+ @BranchCode +''' and ServiceNo = '+ convert(varchar,@ServiceNo) +')  
    from spMstItemPrice a, '+ @DbName +'..spMstItems b, 
    #srv c, gnmstcompanymapping d 
   where 1 = 1  
	 and d.CompanyMd = b.CompanyCode
	 and d.BranchMD = b.BranchCode
        and d.CompanyCode = c.CompanyCode  
     and d.BranchCode  = c.BranchCode  
     and b.PartNo      = a.PartNo  
        and (b.CompanyCode = '''+ @CompanyMD +'''
     and b.BranchCode  = '''+ @BranchMD +'''
     and b.PartNo      = '''+ @PartNo +''')
     and (a.CompanyCode = '''+ @CompanyCode +'''
     and a.BranchCode  = '''+ @BranchCode +'''
     and a.PartNo      = '''+ @PartNo +''')' 
		   
	exec(@QueryTemp)

	--print(@QueryTemp)

	--select   @CostPrice   
	--xxx

	if (@DealerCode = 'SD' and @IsSPK = '2')  
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
			,case when (select JobOrderNo from #srv) is null then convert(varchar,@ServiceNo) else (select JobOrderNo from #srv) end
			,case when (select JobOrderDate from #srv) is null then '1900/01/01' else (select JobOrderDate from #srv) end 
			,case when @PartNo is null then '' else  @PartNo end 
			,case when @PartSeqNew is null then '0' else convert(varchar, @PartSeqNew) end
			,case when @WarehouseMD is null then '' else @WarehouseMD end  
			,case when @DemandQty  is null then '0' else convert(varchar, @DemandQty) end
 			,case when @DemandQty  is null then '0' else convert(varchar, @DemandQty) end
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
		 
		declare @intCountTemp int
		set @intCountTemp = (select count(isnull(JobOrderNo,'')) DocNo from #tmpSvSDMovement)
		if (@intCountTemp > 0 ) begin 
			declare @intStringEmpty int
			set @intStringEmpty = (select count(isnull(JobOrderNo,'')) DocNo from #tmpSvSDMovement where JobOrderNo = '' or JobOrderNo is null)
			select @intCountTemp
			select @intStringEmpty
			if (@intStringEmpty < 1) begin
				set @QueryTemp = '
					insert into ' + @DbName + '..svSDMovement (CompanyCode, BranchCode, DocNo, DocDate, PartNo, PartSeq, WarehouseCode, QtyOrder, Qty, DiscPct, CostPrice, RetailPrice,   
					TypeOfGoods, CompanyMD, BranchMD, WarehouseMD, RetailPriceInclTaxMD, RetailPriceMD, CostPriceMD, QtyFlag, ProductType, ProfitCenterCode, 
					Status, ProcessStatus, ProcessDate, CreatedBy,   
					CreatedDate, LastUpdateBy, LastUpdateDate)  
					select * from #tmpSvSDMovement';
				exec(@QueryTemp);
			end
		end
		 
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
   
 if (@DealerCode = 'SD' and @IsSPK = '2')  
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

--rollback tran