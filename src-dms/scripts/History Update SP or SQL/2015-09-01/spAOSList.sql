IF OBJECT_ID('[dbo].[uspfn_spAutomaticOrderSparepartList]') IS NOT NULL DROP PROCEDURE [dbo].[uspfn_spAutomaticOrderSparepartList]

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



------------------------------
-- AOS ITEM SPARE PART LIST --
------------------------------

CREATE procedure [dbo].[uspfn_spAutomaticOrderSparepartList]
	@CompanyCode		varchar(15),
	@BranchCode			varchar(15),
	@TypeOfGoods		varchar(15)
as

BEGIN
	declare @PartNo				varchar(20)
	declare @NewPartNo			varchar(20)
	declare @OldPart			varchar(20)
	declare @NewPart			varchar(20)
	declare @StartDate			varchar(06)
	declare @EndDate			varchar(06)
	declare @SupplierCode 		varchar(15)
	declare @SupplierCode0		varchar(15)
	declare @SupplierCode1		varchar(15)
	declare @SupplierCode2		varchar(15)
	declare @TPGO				varchar(15)
	declare @SuggorNo			varchar(15)
	declare @POSNo				varchar(15)
	declare @DocPre				varchar(15)
	declare @AOS1               varchar(50)
	declare @AOS2               varchar(50)
	declare @MessageText        varchar(50)
	declare @DocNum				integer
	declare @DocYear			integer
	declare @Counter     		integer
	declare @Switch      		integer
	declare @PeriodYear  		integer
	declare @PeriodMonth		integer
	declare @PeriodDate			date
	declare @SuggorDate			datetime
	declare @SeqNo              numeric( 3,0)
	declare @DAvgFac            numeric(18,2)
	declare @DevFac             numeric(18,2)
	declare @DiscPct			numeric(18,2)
	declare @PurchasePriceNett	numeric(18,0)

	--set @SuggorDate  = getdate()
	set @SuggorDate  = '2014-02-09'
	--set @PeriodYear  = year(getdate())
	--set @PeriodMonth = month(getdate())
	set @PeriodYear  = year('2014-02-09')
	set @PeriodMonth = month('2014-02-09')
	set @Counter     = 0
	set @PeriodDate  = RIGHT('0000'+convert(varchar(4),@PeriodYear ),4) + '/'
                     + RIGHT('00'+convert(varchar(2),@PeriodMonth),2) + '/01'

    if isnull((select ParaValue from gnMstLookUpDtl
                where CompanyCode=@CompanyCode and CodeID='AOS' and LookUpValue='AUTO'),'0') = 0
       return

    if isnull((select ParaValue from gnMstLookUpDtl 
                where CompanyCode=@CompanyCode and CodeID='ORTP' and LookUpValue='8'),'')=''
       begin
          set @SeqNo = isnull((select max(SeqNo) from gnMstLookUpDtl where CompanyCode=@CompanyCode and CodeID='ORTP'),0) + 1
          insert into gnMstLookUpDtl
                     (CompanyCode, CodeID, LookUpValue, SeqNo, ParaValue, LookUpValueName, 
                      CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
               values(@CompanyCode, 'ORTP', '8', @SeqNo, '0', 'AOS (AUTOMATIC ORDER SPAREPART)',
                      'SIM',@SuggorDate,'SIM',@SuggorDate)
       end
                  
	if isnull((select top 1 1 from spTrnPSUGGORHdr
                where CompanyCode=@CompanyCode and BranchCode=@BranchCode and OrderType='8'
                  and convert(varchar,SuggorDate,111)=convert(varchar,@SuggorDate,111)),0) = 1
       return
	   
    set @DocYear     = (select isnull(DocumentYear,9988) from gnMstDocument with(nolock,nowait)
                         where CompanyCode=@CompanyCode and BranchCode=@BranchCode and DocumentType='SGR')

    if @DocYear<>YEAR(@SuggorDate) return

    set @SupplierCode0 = isnull((select ParaValue from gnMstLookupDtl
                                  where CompanyCode = @CompanyCode
	 							    and CodeID      = 'AOS'
	 							    and LookupValue = '0'),'')
    set @SupplierCode1 = isnull((select ParaValue from gnMstLookupDtl
                                  where CompanyCode = @CompanyCode
 								    and CodeID      = 'AOS'
 								    and LookupValue = '1'),'')
    set @SupplierCode2 = isnull((select ParaValue from gnMstLookupDtl
                                  where CompanyCode = @CompanyCode
 								    and CodeID      = 'AOS'
 								    and LookupValue = '2'),'')
    set @DAvgFac       = isnull((select convert(numeric(6,2),ParaValue) from gnMstLookupDtl
               where CompanyCode = @CompanyCode
      							    and CodeID      = 'AOS'
    								and LookupValue = 'DAVGFAC'),0.00)
    set @DevFac        = isnull((select convert(numeric(6,2),ParaValue) from gnMstLookupDtl
                                  where CompanyCode = @CompanyCode
    							    and CodeID      = 'AOS'
    								and LookupValue = 'DevFac'),0.60)

    create table #Suggor 
 	    ( [CompanyCode]		[varchar](15  ),
	      [BranchCode]    	[varchar](15  ),
	      [PartNo]        	[varchar](20  ),
	      [NewPartNo]     	[varchar](20  ),
	      [SupplierCode]  	[varchar](20  ),
	      [ProductType]   	[varchar](15  ),
	      [PartCategory]  	[varchar]( 3  ),
	      [TypeOfGoods]   	[varchar]( 5  ),
	      [MovingCode]    	[varchar](15  ),
	      [ABCClass]      	[char]   ( 1  ),
	      [OnHand]        	[numeric](12,2),
	      [OnOrder]       	[numeric](12,2),
	      [InTransit]     	[numeric](12,2),
	      [AllocationSP]  	[numeric](12,2),
	      [AllocationSR]  	[numeric](12,2),
	      [AllocationSL]  	[numeric](12,2),
	      [BackOrderSP]   	[numeric](12,2),
	      [BackOrderSR]   	[numeric](12,2),
	      [BackOrderSL]   	[numeric](12,2),
	      [ReservedSP]    	[numeric](12,2),
	      [ReservedSR]    	[numeric](12,2),
	      [ReservedSL]    	[numeric](12,2),
	      [DemandAvg]     	[numeric](15,5),
	      [OrderPoint]    	[numeric](12,0),
	      [SafetyStock]   	[numeric](12,0),
	      [AvailableQty]  	[numeric](12,2),
	      [OrderUnit]     	[numeric](12,2),
	      [PurchasePrice] 	[numeric](18,0),
	      [CostPrice]     	[numeric](18,0)
        )

    insert into #Suggor
              ( CompanyCode, BranchCode, PartNo, NewPartNo, SupplierCode, ProductType, PartCategory, 
	 		    TypeOfGoods, MovingCode, ABCClass, OnHand, OnOrder, InTransit, AllocationSP, AllocationSR, 
	 		    AllocationSL, BackOrderSP, BackOrderSR, BackOrderSL, ReservedSP, ReservedSR, ReservedSL, 
	 		    DemandAvg, OrderPoint, SafetyStock, AvailableQty, OrderUnit, PurchasePrice, CostPrice)
         select i.CompanyCode, i.BranchCode, i.PartNo, i.PartNo NewPartNo, f.SupplierCode, i.ProductType, 
		        i.PartCategory, i.TypeOfGoods, i.MovingCode, i.ABCClass, i.OnHand, i.OnOrder, i.InTransit, 
			    i.AllocationSP, i.AllocationSR, i.AllocationSL, i.BackOrderSP, i.BackOrderSR, i.BackOrderSL, 
			    i.ReservedSP, i.ReservedSR, i.ReservedSL, i.DemandAverage DemandAvg, i.OrderPointQty OrderPoint, 
			    i.SafetyStockQty SafetyStock,(i.OnHand-i.AllocationSP-i.AllocationSR-i.AllocationSL) AvailableQty, 
			    OrderUnit = case when isnull(i.OrderUnit,0.00)=0.00 then 1.00 else i.OrderUnit end, 
			    p.PurchasePrice, p.CostPrice
           from spMstItems i
                inner join spMstItemInfo f
			   	        on f.CompanyCode = i.CompanyCode
		               and f.PartNo      = i.PartNo
				       and f.Status      = 1
				       and f.SupplierCode in (@SupplierCode0, @SupplierCode1, @SupplierCode2)
	            inner join spMstItemPrice p
				        on p.CompanyCode = i.CompanyCode
				       and p.BranchCode  = i.BranchCode
		               and p.PartNo      = i.PartNo 
		  where i.CompanyCode = @CompanyCode
		    and i.BranchCode  = @BranchCode
		    and i.Status      = 1
		  --and i.TypeOfGoods in ('0','1','2') -- 0:SGP, 1:SGO, 2:SGA
		  --and i.TypeOfGoods = '0' -- 0:SGP
		    and i.TypeOfGoods = @TypeOfGoods

    create table #t1 
	    ( [CompanyCode] 	[varchar](15  ),
	      [BranchCode]  	[varchar](15  ),
	      [PartNo]      	[varchar](20  ),
	      [NewPartNo]   	[varchar](20  ),
	      [Year]        	[numeric]( 4,0),
	      [Month]       	[numeric]( 2,0),
	      [DemandQty]     	[numeric](18,2)
        )

    while @Counter < 6
      begin
         set @Counter = @Counter + 1
         if @PeriodMonth > 1
            begin
               set @PeriodYear  = @PeriodYear
               set @PeriodMonth = @PeriodMonth-1
            end
         else 
            begin
               set @PeriodYear  = @PeriodYear-1
               set @PeriodMonth = 12
            end

         insert into #t1
              select CompanyCode, BranchCode, PartNo, PartNo NewPartNo, 
                     @PeriodYear Year, @PeriodMonth Month, 
                     isnull((select DemandQty from spHstDemandItem
                              where CompanyCode = #Suggor.CompanyCode
                                and BranchCode  = #Suggor.BranchCode
                                and Year        = @PeriodYear
                                and Month       = @PeriodMonth
                                and PartNo      = #Suggor.PartNo),0.00) as DemandQty
                from #Suggor     -- spMstItems
               where CompanyCode = @CompanyCode
                 and BranchCode  = @BranchCode
      end

    declare ITEM   cursor for
            select distinct CompanyCode, BranchCode, PartNo, NewPartNo from #t1
             where exists (select top 1 1 from spMstItemMod
                            where CompanyCode = #t1.CompanyCode
                              and BranchCode  = #t1.BranchCode
                              and PartNo      = #t1.PartNo
                              and NewPartNo  <> #t1.PartNo
                              and InterChangeCode in ('11','21'))
             order by PartNo
    open  ITEM
    fetch next from ITEM into @CompanyCode, @BranchCode, @PartNo, @NewPartNo

    while @@fetch_status=0
      begin
         set @OldPart  = @PartNo
         set @Switch   = 0
         set @Counter  = 0
         set @NewPart = ''
         while @NewPart = '' and @Switch=0
            begin
               set @Counter = @Counter + 1
               if @Counter > 25
				   begin
						set @NewPart = @PartNo
						set @Switch  = 1
				   end
               else
                   begin
						set @NewPart = isnull((select top 1 NewPartNo from spMstItemMod x
                                                where PartNo=@OldPart 
                                                  and InterChangeCode in ('11','21')
												  and exists (select 1 from spMstItems
															   where CompanyCode=x.CompanyCode
																 and BranchCode =@BranchCode
																 and PartNo=x.NewPartNo)
												  and not exists (select 1 from spMstItemMod
																   where CompanyCode=x.CompanyCode
																	 and PartNo=x.NewPartNo
																	 and NewPartNo=x.PartNo)),'')
						if  @NewPart = ''
							begin 
								set @NewPart = @OldPart
								set @Switch  = 1
							end
						else
							begin
								set @OldPart = @NewPart
								set @NewPart = ''
							end
					end
            end
		 update #t1     set NewPartNo=@NewPart where PartNo=@PartNo
		 update #Suggor set NewPartNo=@NewPart where PartNo=@PartNo
         fetch next from ITEM into @CompanyCode, @BranchCode, @PartNo, @NewPartNo
      end
    close ITEM
    deallocate ITEM

    select * into #t2
      from ( select CompanyCode, BranchCode, NewPartNo, Year, Month, sum(DemandQty) DemandQty
               from #t1 
              group by CompanyCode, BranchCode, NewPartNo, Year, Month) #t2

    select * into #t3
      from ( select CompanyCode, BranchCode, NewPartNo, 
                    sum  (isnull(DemandQty,0.0))    DmnQty, 
                   (sum  (isnull(DemandQty,0.0)))/6 DmnAvg, 
                    stdev(isnull(DemandQty,0.0))    StdDev,
                    case when sum  (isnull(DemandQty,0.0)) = 0.0
                         then 0.0
                         else round((stdev(isnull(DemandQty,0.0)) / 
                                   ((sum  (isnull(DemandQty,0.0)))/6)),2)
                    end as DevFac,
                    max  (isnull(DemandQty,0.0))    MaxQty,
                    min  (isnull(DemandQty,0.0))    MinQty
               from #t2
              group by CompanyCode, BranchCode, NewPartNo ) #t3

    select * into #t4
      from ( select #t3.CompanyCode, #t3.BranchCode, #t3.NewPartNo, #Suggor.MovingCode, 
                    #Suggor.ABCClass, #Suggor.ProductType, #Suggor.PartCategory, 
                    #Suggor.TypeOfGoods, #Suggor.SupplierCode, 
                    #Suggor.CostPrice, #Suggor.PurchasePrice,
                    DmnQty6      = (select sum(isnull(DemandQty,0.00)) from #t2
                                     where CompanyCode=#t3.CompanyCode
                                       and BranchCode =#t3.BranchCode
                                       and Year       =year (dateadd(mm,-6,@PeriodDate))
                                       and Month      =month(dateadd(mm,-6,@PeriodDate))
                                       and NewPartNo  =#t3.NewPartNo),
                    DmnQty5      = (select sum(isnull(DemandQty,0.00)) from #t2
                                     where CompanyCode=#t3.CompanyCode
                                       and BranchCode =#t3.BranchCode
                                       and Year       =year (dateadd(mm,-5,@PeriodDate))
                                       and Month      =month(dateadd(mm,-5,@PeriodDate))
                                       and NewPartNo  =#t3.NewPartNo),
                    DmnQty4      = (select sum(isnull(DemandQty,0.00)) from #t2
                                     where CompanyCode=#t3.CompanyCode
                                       and BranchCode =#t3.BranchCode
                                       and Year       =year (dateadd(mm,-4,@PeriodDate))
                                       and Month      =month(dateadd(mm,-4,@PeriodDate))
                                       and NewPartNo  =#t3.NewPartNo),
                    DmnQty3      = (select sum(isnull(DemandQty,0.00)) from #t2
                                     where CompanyCode=#t3.CompanyCode
                                       and BranchCode =#t3.BranchCode
                                       and Year       =year (dateadd(mm,-3,@PeriodDate))
                                       and Month      =month(dateadd(mm,-3,@PeriodDate))
                                       and NewPartNo  =#t3.NewPartNo),
                    DmnQty2      = (select sum(isnull(DemandQty,0.00)) from #t2
                                     where CompanyCode=#t3.CompanyCode
                                       and BranchCode =#t3.BranchCode
                                       and Year       =year (dateadd(mm,-2,@PeriodDate))
                                       and Month      =month(dateadd(mm,-2,@PeriodDate))
                                       and NewPartNo  =#t3.NewPartNo),
                    DmnQty1      = (select sum(isnull(DemandQty,0.00)) from #t2
                                     where CompanyCode=#t3.CompanyCode
                                       and BranchCode =#t3.BranchCode
                                       and Year       =year (dateadd(mm,-1,@PeriodDate))
                                       and Month      =month(dateadd(mm,-1,@PeriodDate))
                                       and NewPartNo  =#t3.NewPartNo),
                    OnHand       = (select sum(isnull(OnHand,0.00)) from #Suggor
                                     where CompanyCode=#t3.CompanyCode
                                       and BranchCode =#t3.BranchCode
                                       and NewPartNo  =#t3.NewPartNo),
                    OnOrder      = (select sum(isnull(OnOrder,0.00)) from #Suggor
                                     where CompanyCode=#t3.CompanyCode
                                       and BranchCode =#t3.BranchCode
                                       and NewPartNo  =#t3.NewPartNo),
                    InTransit    = (select sum(isnull(InTransit,0.00)) from #Suggor
                                     where CompanyCode=#t3.CompanyCode
                                       and BranchCode =#t3.BranchCode
                                       and NewPartNo  =#t3.NewPartNo),
                    AllocationSP = (select sum(isnull(AllocationSP,0.00)) from #Suggor
                                     where CompanyCode=#t3.CompanyCode
                                       and BranchCode =#t3.BranchCode
                                       and NewPartNo  =#t3.NewPartNo),                                
                    AllocationSR = (select sum(isnull(AllocationSR,0.00)) from #Suggor
                                     where CompanyCode=#t3.CompanyCode
                                       and BranchCode =#t3.BranchCode
                                       and NewPartNo  =#t3.NewPartNo),
                    AllocationSL = (select sum(isnull(AllocationSL,0.00)) from #Suggor
                                     where CompanyCode=#t3.CompanyCode
                                       and BranchCode =#t3.BranchCode
                                       and NewPartNo  =#t3.NewPartNo),
                    BackOrderSP  = (select sum(isnull(BackOrderSP,0.00)) from #Suggor
                                     where CompanyCode=#t3.CompanyCode
                                       and BranchCode =#t3.BranchCode
                                       and NewPartNo  =#t3.NewPartNo),
                    BackOrderSR  = (select sum(isnull(BackOrderSR,0.00)) from #Suggor
                                     where CompanyCode=#t3.CompanyCode
                                       and BranchCode =#t3.BranchCode
                                       and NewPartNo  =#t3.NewPartNo),
                    BackOrderSL  = (select sum(isnull(BackOrderSL,0.00)) from #Suggor
                                     where CompanyCode=#t3.CompanyCode
                                       and BranchCode =#t3.BranchCode
                                       and NewPartNo  =#t3.NewPartNo),
                    ReservedSP   = (select sum(isnull(ReservedSP,0.00)) from #Suggor
                                     where CompanyCode=#t3.CompanyCode
                                       and BranchCode =#t3.BranchCode
                                       and NewPartNo  =#t3.NewPartNo),
                    ReservedSR   = (select sum(isnull(ReservedSR,0.00)) from #Suggor
                                     where CompanyCode=#t3.CompanyCode
                                       and BranchCode =#t3.BranchCode
                                       and NewPartNo  =#t3.NewPartNo),
                    ReservedSL   = (select sum(isnull(ReservedSL,0.00)) from #Suggor
                                     where CompanyCode=#t3.CompanyCode
                                       and BranchCode =#t3.BranchCode
                                       and NewPartNo  =#t3.NewPartNo),
                    999999999999.99 AvailableQty, #Suggor.OrderUnit, 
                    DmnQty, DmnAvg, StdDev, DevFac, MaxQty, MinQty,
                    LeadTime     = (select isnull(LeadTime,0.00) from spMstOrderParam
                                     where CompanyCode  = #t3.CompanyCode
                                       and BranchCode   = #t3.BranchCode
                                       and SupplierCode = case #Suggor.TypeOfGoods
                                                               when '0' then @SupplierCode0
                                                               when '1' then @SupplierCode1
                                                               when '2' then @SupplierCode2
                                                               else '***NO-ORDER***'
                                                          end
                                       and MovingCode   = 1),
                    OrderCycle   = (select isnull(OrderCycle,0.00) from spMstOrderParam
                                     where CompanyCode  = #t3.CompanyCode
                                       and BranchCode   = #t3.BranchCode
                                       and SupplierCode = case #Suggor.TypeOfGoods
                                                               when '0' then @SupplierCode0
                                                               when '1' then @SupplierCode1
                                                               when '2' then @SupplierCode2
                                                               else '***NO-ORDER***'
                                                          end
                                       and MovingCode   = 1),
                    SafetyStock  = (select isnull(SafetyStock,0.00) from spMstOrderParam
                                     where CompanyCode  = #t3.CompanyCode
                                       and BranchCode   = #t3.BranchCode
                                       and SupplierCode = case #Suggor.TypeOfGoods
                                                               when '0' then @SupplierCode0
                                                               when '1' then @SupplierCode1
                                                               when '2' then @SupplierCode2
                                                               else '***NO-ORDER***'
                                                          end
                                       and MovingCode   = 1),
                    999999999999.99 OrderPoint, 999999999999.99 SafetyStokPoint, 
                    999999999999.99 SuggorQty, 0 Status
               from #t3, #Suggor
              where #t3.CompanyCode=#Suggor.CompanyCode
                and #t3.BranchCode =#Suggor.BranchCode
                and #t3.NewPartNo  =#Suggor.PartNo
                and #Suggor.TypeOfGoods in ('0','1','2')) #t4 -- 0:SGP, 1:SGO, 2:SGA

    update #t4
       set AvailableQty    = (OnHand+OnOrder+InTransit)
                           - (AllocationSP+AllocationSR+AllocationSL
                           +  BackOrderSP +BackOrderSR +BackOrderSL
                           +  ReservedSP  +ReservedSR  +ReservedSL),
           OrderPoint      = ceiling(DmnAvg/30 * isnull((LeadTime+OrderCycle+SafetyStock),0.00)),
           SafetyStokPoint = ceiling(DmnAvg/30 * isnull(SafetyStock,0.00))

    update #t4 
       set SuggorQty       = case when OrderPoint>AvailableQty  --AvailableQty>0.00 and OrderPoint>AvailableQty 
                                  then ceiling((OrderPoint-AvailableQty)/OrderUnit) * OrderUnit
                                  else 0.00 
                             end

    update #t4
       set Status          = 1
      where SuggorQty>0.00
        and DevFac<=@DevFac
        and StdDev<=DmnAvg
        and DmnAvg>0.00
		
    ----select * from #t4 order by TypeOfGoods, SupplierCode, NewPartNo
    --select * from #t4 where Status=1 order by TypeOfGoods, SupplierCode, NewPartNo

	select 
		a.NewPartNo as part_no,b.PartName as part_name,a.MovingCode as moving_code,
		a.DmnAvg as d_avg_day,a.OnHand as on_hand,a.OnOrder as on_order,a.intransit,
		sum(a.AllocationSL+a.AllocationSP+a.AllocationSR) as allocation,
		sum(a.BackOrderSL+a.BackOrderSP+a.BackOrderSR) as back_order,
		a.OrderPoint as order_point, 'Y' is_aos, c.orderCycle as aos_frequency
	from #t4 a
	inner join spMstItemInfo b on b.PartNo=a.NewPartNo
	inner join spMstOrderParam c on c.CompanyCode=a.CompanyCode and c.BranchCode=a.BranchCode 
				and c.SupplierCode=a.SupplierCode 
				and c.MovingCode=a.MovingCode
	where a.Status=1 
	group by a.NewPartNo,b.PartName,a.MovingCode,a.DmnAvg,a.OnHand
	,a.OnOrder,a.intransit,a.OrderPoint,c.orderCycle
	,a.TypeOfGoods, a.SupplierCode, a.NewPartNo
	order by a.TypeOfGoods, a.SupplierCode, a.NewPartNo

    drop table #Suggor
    drop table #t1
    drop table #t2
    drop table #t3
    drop table #t4

END



GO


