---------------------------------------------------------
-- AUTOMATIC ORDER SPAREPART, by Hasim... 24 January 2014
-- Revisi 
---------------------------------------------------------
-- uspfn_spAutomaticOrderSparepart '6115204','611520401' 
-- 6 Oct 2014: 
--		di OMIT ==> Demand Average 50
--		PO hanya SGP part, TypeOfGoods = '0'
-- 24 Oct 2014: Tuning
-- 27 Oct 2014: ditambahkan kondisi StdDev<=DmnAvg
-- 16 Oct 2015: penambahan pembatalan AOS khususnya AOS yang belum dikirim ke DCS
-- 16 Dec 2015: perhitungan demand selama 12 bulan
--

ALTER procedure [dbo].[uspfn_spAutomaticOrderSparepart]
	@CompanyCode		varchar(15),
	@BranchCode			varchar(15)
as

BEGIN
  --declare @CompanyCode		varchar(15)
  --declare @BranchCode			varchar(15)
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
	declare @PeriodDate			datetime
	declare @SuggorDate			datetime
	declare @SeqNo              numeric( 3,0)
	declare @DAvgFac            numeric(18,2)
	declare @DevFac             numeric(18,2)
	declare @DiscPct			numeric(18,2)
	declare @PurchasePriceNett	numeric(18,0)
  --set @CompanyCode = '6006406'
  --set @BranchCode  = '6006406'
	set @SuggorDate  = getdate()
	set @PeriodYear  = year(getdate())
	set @PeriodMonth = month(getdate())
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

  --Cancel old AOS (AOS not send to DCS yet), 16 Oct 2015
	execute uspfn_spAutomaticOrderSparepart_CancelProcess @CompanyCode, @BranchCode
  --end...

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
  --set @DAvgFac       = 50.00
  --set @DevFac        = 0.60

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
		    and i.TypeOfGoods = '0' -- 0:SGP
			and isnull(i.Utility4,' ')<>'W'

    create table #t1 
	    ( [CompanyCode] 	[varchar](15  ),
	      [BranchCode]  	[varchar](15  ),
	      [PartNo]      	[varchar](20  ),
	      [NewPartNo]   	[varchar](20  ),
	      [Year]        	[numeric]( 4,0),
	      [Month]       	[numeric]( 2,0),
	      [DemandQty]     	[numeric](18,2)
        )

-- demand 12 months
--  while @Counter < 6
    while @Counter < 12
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
               if @Counter > 20
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
                    sum  (isnull(DemandQty,0.0))     DmnQty, 
-- demand 12 months
--                 (sum  (isnull(DemandQty,0.0)))/6  DmnAvg, 
                   (sum  (isnull(DemandQty,0.0)))/12 DmnAvg, 
                    stdev(isnull(DemandQty,0.0))     StdDev,
                    case when sum  (isnull(DemandQty,0.0)) = 0.0
                         then 0.0
                         else round((stdev(isnull(DemandQty,0.0)) / 
-- demand 12 months
--                                 ((sum  (isnull(DemandQty,0.0)))/6)),2)
                                   ((sum  (isnull(DemandQty,0.0)))/12)),2)
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
-- demand 12 months
                    DmnQty12     = (select sum(isnull(DemandQty,0.00)) from #t2
                                     where CompanyCode=#t3.CompanyCode
                                       and BranchCode =#t3.BranchCode
                                       and Year       =year (dateadd(mm,-12,@PeriodDate))
                                       and Month      =month(dateadd(mm,-12,@PeriodDate))
                                       and NewPartNo  =#t3.NewPartNo),
                    DmnQty11     = (select sum(isnull(DemandQty,0.00)) from #t2
                                     where CompanyCode=#t3.CompanyCode
                                       and BranchCode =#t3.BranchCode
                                       and Year       =year (dateadd(mm,-11,@PeriodDate))
                                       and Month      =month(dateadd(mm,-11,@PeriodDate))
                                       and NewPartNo  =#t3.NewPartNo),
                    DmnQty10     = (select sum(isnull(DemandQty,0.00)) from #t2
                                     where CompanyCode=#t3.CompanyCode
                                       and BranchCode =#t3.BranchCode
                                       and Year       =year (dateadd(mm,-10,@PeriodDate))
                                       and Month      =month(dateadd(mm,-10,@PeriodDate))
                                       and NewPartNo  =#t3.NewPartNo),
                    DmnQty09     = (select sum(isnull(DemandQty,0.00)) from #t2
                                     where CompanyCode=#t3.CompanyCode
                                       and BranchCode =#t3.BranchCode
                                       and Year       =year (dateadd(mm,-09,@PeriodDate))
                                       and Month      =month(dateadd(mm,-09,@PeriodDate))
                                       and NewPartNo  =#t3.NewPartNo),
                    DmnQty08     = (select sum(isnull(DemandQty,0.00)) from #t2
                                     where CompanyCode=#t3.CompanyCode
                                       and BranchCode =#t3.BranchCode
                                       and Year       =year (dateadd(mm,-08,@PeriodDate))
                                       and Month      =month(dateadd(mm,-08,@PeriodDate))
                                       and NewPartNo  =#t3.NewPartNo),
                    DmnQty07     = (select sum(isnull(DemandQty,0.00)) from #t2
                                     where CompanyCode=#t3.CompanyCode
                                       and BranchCode =#t3.BranchCode
                                       and Year       =year (dateadd(mm,-07,@PeriodDate))
                                       and Month      =month(dateadd(mm,-07,@PeriodDate))
                                       and NewPartNo  =#t3.NewPartNo),
                    DmnQty06     = (select sum(isnull(DemandQty,0.00)) from #t2
                                     where CompanyCode=#t3.CompanyCode
                                       and BranchCode =#t3.BranchCode
                                       and Year       =year (dateadd(mm,-06,@PeriodDate))
                                       and Month      =month(dateadd(mm,-06,@PeriodDate))
                                       and NewPartNo  =#t3.NewPartNo),
                    DmnQty05     = (select sum(isnull(DemandQty,0.00)) from #t2
                                     where CompanyCode=#t3.CompanyCode
                                       and BranchCode =#t3.BranchCode
                                       and Year       =year (dateadd(mm,-05,@PeriodDate))
                                       and Month      =month(dateadd(mm,-05,@PeriodDate))
                                       and NewPartNo  =#t3.NewPartNo),
                    DmnQty04     = (select sum(isnull(DemandQty,0.00)) from #t2
                                     where CompanyCode=#t3.CompanyCode
                                       and BranchCode =#t3.BranchCode
                                       and Year       =year (dateadd(mm,-04,@PeriodDate))
                                       and Month      =month(dateadd(mm,-04,@PeriodDate))
                                       and NewPartNo  =#t3.NewPartNo),
                    DmnQty03     = (select sum(isnull(DemandQty,0.00)) from #t2
                                     where CompanyCode=#t3.CompanyCode
                                       and BranchCode =#t3.BranchCode
                                       and Year       =year (dateadd(mm,-03,@PeriodDate))
                                       and Month      =month(dateadd(mm,-03,@PeriodDate))
                                       and NewPartNo  =#t3.NewPartNo),
                    DmnQty02     = (select sum(isnull(DemandQty,0.00)) from #t2
                                     where CompanyCode=#t3.CompanyCode
                                       and BranchCode =#t3.BranchCode
                                       and Year       =year (dateadd(mm,-02,@PeriodDate))
                                       and Month      =month(dateadd(mm,-02,@PeriodDate))
                                       and NewPartNo  =#t3.NewPartNo),
                    DmnQty01     = (select sum(isnull(DemandQty,0.00)) from #t2
                                     where CompanyCode=#t3.CompanyCode
                                       and BranchCode =#t3.BranchCode
                                       and Year       =year (dateadd(mm,-01,@PeriodDate))
                                       and Month      =month(dateadd(mm,-01,@PeriodDate))
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
           --Status          = case when OrderPoint>AvailableQty  --AvailableQty>0.00 and OrderPoint>AvailableQty 
           --                       then 1 
           --                       else 0 
           --                  end
     --where Status = 1 

    update #t4
       set Status          = 1
    --where DmnAvg>=50.0 and DevFac<=0.6
    --where DmnAvg>=@DAvgFac and DevFac<=@DevFac
      where SuggorQty>0.00
        and DevFac<=@DevFac
        and StdDev<=DmnAvg
        and DmnAvg>0.00
		
--  select * from #t4 order by TypeOfGoods, SupplierCode, NewPartNo
    select * from #t4 where Status=1 order by TypeOfGoods, SupplierCode, NewPartNo

    if isnull((select COUNT(*) from #t4 where Status=1),0) = 0
       begin
           drop table #Suggor
	  	   drop table #t1
           drop table #t2
           drop table #t3
           drop table #t4
           return
       end

  --insert to SUGGOR table   
    set @TPGO = ''
    set @AOS1 = ''
    set @AOS2 = ''

    declare @cur_NewPartNo       varchar(20)
    declare @cur_MovingCode      varchar(15)
    declare @cur_ABCClass        varchar(01)
    declare @cur_ProductType	 varchar(15)
    declare @cur_PartCategory	 varchar(03)
    declare @cur_TypeOfGoods	 varchar(05)
    declare @cur_SupplierCode    varchar(20)
    declare @cur_CostPrice		 numeric(18,2)
    declare @cur_PurchasePrice	 numeric(18,2)
    declare @cur_OnHand          numeric(12,2)
    declare @cur_OnOrder         numeric(12,2)
    declare @cur_InTransit       numeric(12,2)
    declare @cur_AllocationSP    numeric(12,2)
    declare @cur_AllocationSR    numeric(12,2)
    declare @cur_AllocationSL    numeric(12,2)
    declare @cur_BackOrderSP     numeric(12,2)
    declare @cur_BackOrderSR     numeric(12,2)
    declare @cur_BackOrderSL     numeric(12,2)
    declare @cur_ReservedSP      numeric(12,2)
    declare @cur_ReservedSR      numeric(12,2)
    declare @cur_ReservedSL      numeric(12,2)
    declare @cur_AvailableQty    numeric(12,2)
    declare @cur_DmnAvg          numeric(15,5)
    declare @cur_OrderPoint      numeric(12,0)
    declare @cur_SafetyStokPoint numeric(12,0)
    declare @cur_SuggorQty       numeric(12,0)

    declare SUGGOR cursor for
            select NewPartNo, MovingCode, ABCClass, ProductType, PartCategory, TypeOfGoods, SupplierCode, 
                   CostPrice, PurchasePrice, OnHand, OnOrder, InTransit, AllocationSP, AllocationSR, 
                   AllocationSL, BackOrderSP, BackOrderSR, BackOrderSL, ReservedSP, ReservedSR, 
                   ReservedSL, AvailableQty, DmnAvg, OrderPoint, SafetyStokPoint, SuggorQty
              from #t4 where Status=1 order by TypeOfGoods, SupplierCode, NewPartNo
    open  SUGGOR
    fetch next from SUGGOR 
               into @cur_NewPartNo, @cur_MovingCode, @cur_ABCClass, @cur_ProductType, @cur_PartCategory, 
                    @cur_TypeOfGoods, @cur_SupplierCode, @cur_CostPrice, @cur_PurchasePrice, @cur_OnHand, 
                    @cur_OnOrder, @cur_InTransit, @cur_AllocationSP, @cur_AllocationSR, @cur_AllocationSL, 
                    @cur_BackOrderSP, @cur_BackOrderSR, @cur_BackOrderSL, @cur_ReservedSP, @cur_ReservedSR, 
                    @cur_ReservedSL, @cur_AvailableQty, @cur_DmnAvg, @cur_OrderPoint, @cur_SafetyStokPoint, 
                    @cur_SuggorQty

    while @@fetch_status=0
      begin
         if @TPGO<>@cur_TypeOfGoods or @SupplierCode<>@cur_SupplierCode
            begin
                set @Counter      = 0
                set @TPGO         = @cur_TypeOfGoods
                set @SupplierCode = @cur_SupplierCode
                -- setup nomor document for SUGGOR
                   set @DocPre    = (select isnull(DocumentPrefix,'XYZ') from gnMstDocument
                                      where CompanyCode=@CompanyCode and BranchCode=@BranchCode and DocumentType='SGR')
                   set @DocNum    = (select isnull(DocumentSequence,999888) from gnMstDocument
                                      where CompanyCode=@CompanyCode and BranchCode=@BranchCode and DocumentType='SGR')
                   if  @DocNum    = 999888  return
                   set @DocNum    = @DocNum + 1
                   update gnMstDocument set DocumentSequence = @DocNum
                    where CompanyCode=@CompanyCode and BranchCode=@BranchCode and DocumentType='SGR'
                   set @SuggorNo  = @DocPre + '/' + right('00'+right(convert(varchar(4),year(@SuggorDate)),2),2)
                                            + '/' + right('000000'+(convert(varchar(6),@DocNum,6)),6)
                 --set @AOS1      = @AOS1 + case when @AOS1='' then @SuggorNo else ', ' + @SuggorNo end
                   set @AOS1      = @AOS1 + case when @AOS1='' then @SuggorNo else ', ' + right(@SuggorNo,6) end

                -- setup nomor document POS
                   set @DocPre    = (select isnull(DocumentPrefix,'XYZ') from gnMstDocument
                                      where CompanyCode=@CompanyCode and BranchCode=@BranchCode and DocumentType='POS')
                   set @DocNum    = (select isnull(DocumentSequence,999888) from gnMstDocument
                                      where CompanyCode=@CompanyCode and BranchCode=@BranchCode and DocumentType='POS')
                   if  @DocNum    = 999888  return
                   set @DocNum    = @DocNum + 1
                   update gnMstDocument set DocumentSequence = @DocNum
                    where CompanyCode=@CompanyCode and BranchCode=@BranchCode and DocumentType='POS'
                   set @POSNo     = @DocPre + '/' + right('00'+right(convert(varchar(4),year(@SuggorDate)),2),2)
                                            + '/' + right('000000'+(convert(varchar(6),@DocNum,6)),6)
                 --set @AOS2      = @AOS2 + case when @AOS2='' then @POSNo else ', ' + @POSNo end
                   set @AOS2      = @AOS2 + case when @AOS2='' then @POSNo else ', ' + right(@POSNo,6) end

                -- insert Suggor Header table
                   insert into spTrnPSUGGORHdr
                              (CompanyCode, BranchCode, SuggorNo, SuggorDate, TypeOfGoods, POSNo, 
                               POSDate, SupplierCode, ProductType, MovingCode, OrderType, Status, 
                               PrintSeq, IsVoid, CreatedBy, CreatedDate, LastUpdateBy, 
                               LastUpdateDate, isLocked, LockingBy, LockingDate)
                        values(@CompanyCode, @BranchCode, @SuggorNo, @SuggorDate, @TPGO, @POSNo, 
                               @SuggorDate, @SupplierCode, @cur_ProductType, 1, '8', 1, 0, 0,  -- OrderType = 8 - AOS
                               'AOS', @SuggorDate, 'AOS', @SuggorDate, '0', NULL, NULL)

                -- insert POS Header table
                   insert into spTrnPPOSHdr
                              (CompanyCode, BranchCode, POSNo, POSDate, SupplierCode, OrderType, IsBO, 
                               IsSubstution, isSuggorProcess, Remark, ProductType, PrintSeq, ExPickingSlipNo, 
                               ExPickingSlipDate, Status, Transportation, TypeOfGoods, isGenPORDD, isDeleted, 
                               CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate, isLocked, LockingBy, 
                               LockingDate, isDropSign, DropSignReffNo)
                        values(@CompanyCode, @BranchCode, @POSNo, @SuggorDate, @SupplierCode, '8', 1, 
                               1, 1, 'MACHINE ORDER', @cur_ProductType, 1, NULL, NULL, 2, 'AOS', @TPGO, 
                               0, 0, 'AOS', @SuggorDate, 'AOS', @SuggorDate, 0, NULL, NULL, 0, NULL)
            end

         -- insert Suggor Detail table
            set @Counter = @Counter + 1
            insert into spTrnPSUGGORDtl
                       (CompanyCode, BranchCode, SuggorNo, PartNo, SeqNo, OnHand, OnOrder, InTransit, 
                        AllocationSP, AllocationSR, AllocationSL, BackOrderSP, BackOrderSR, BackOrderSL, 
                        ReservedSP, ReservedSR, ReservedSL, DemandAvg, OrderPoint, SafetyStock, AvailableQty, 
                        SuggorQty, SuggorCorrecQty, ProductType, PartCategory, PurchasePrice, CostPrice, 
                        isExistInItems, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
                 values(@CompanyCode, @BranchCode, @SuggorNo, @cur_NewPartNo, @Counter, @cur_OnHand, 
                        @cur_OnOrder, @cur_InTransit, @cur_AllocationSP, @cur_AllocationSR, @cur_AllocationSL, 
                        @cur_BackOrderSP, @cur_BackOrderSR, @cur_BackOrderSL, @cur_ReservedSP, 
                        @cur_ReservedSR, @cur_ReservedSL, @cur_DmnAvg, @cur_OrderPoint, 
                        @cur_SafetyStokPoint, @cur_AvailableQty, @cur_SuggorQty, @cur_SuggorQty,
                        @cur_ProductType, @cur_PartCategory, @cur_PurchasePrice, @cur_CostPrice, 
                        1, 'AOS', @SuggorDate, 'AOS', @SuggorDate)

         -- insert POS Detail table
            set @DiscPct = (select isnull(DiscPct,0.00) from gnMstSupplierProfitCenter 
                             where CompanyCode      = @CompanyCode
                               and BranchCode       = @BranchCode 
                               and SupplierCode     = @cur_SupplierCode 
                               and ProfitCenterCode = '300')
            set @PurchasePriceNett = floor(@cur_PurchasePrice * (100.00 - @DiscPct) / 100)
            insert into spTrnPPOSDtl
                       (CompanyCode, BranchCode, POSNo, PartNo, SeqNo, OrderQty, SuggorQty, 
                        PurchasePrice, DiscPct, PurchasePriceNett, CostPrice, TotalAmount, 
                        ABCClass, MovingCode, ProductType, PartCategory, CreatedBy, 
                        CreatedDate, LastUpdateBy, LastUpdateDate, Note)
                 values(@CompanyCode, @BranchCode, @POSNo, @cur_NewPartNo, @Counter, 
               @cur_SuggorQty, @cur_SuggorQty, @cur_PurchasePrice, @DiscPct, 
                        @PurchasePriceNett, @cur_CostPrice, @cur_SuggorQty * @PurchasePriceNett,
                        @cur_ABCClass, @cur_MovingCode, @cur_ProductType, @cur_PartCategory, 
                        'AOS', @SuggorDate, 'AOS', @SuggorDate, 'AOS')

         -- insert Suggor Sub-detail table
            insert into spTrnPSUGGORSubDtl
                 select @CompanyCode, @BranchCode, @SuggorNo, @cur_NewPartNo, PartNo,
                        ROW_NUMBER() over (order by PartNo) as row,
-- demand 12 months
                        I   = (select DemandQty from #t1
                                where CompanyCode = @CompanyCode
                                  and BranchCode  = @BranchCode
                                  and Year        = year (dateadd(mm,-12,@PeriodDate))
                                  and Month       = month(dateadd(mm,-12,@PeriodDate))
                                  and NewPartNo   = @cur_NewPartNo
                                  and PartNo      = #Suggor.PartNo),
                        II  = (select DemandQty from #t1
                                where CompanyCode = @CompanyCode
                                  and BranchCode  = @BranchCode
                                  and Year        = year (dateadd(mm,-11,@PeriodDate))
                                  and Month       = month(dateadd(mm,-11,@PeriodDate))
                                  and NewPartNo   = @cur_NewPartNo
                                  and PartNo      = #Suggor.PartNo),
                        III = (select DemandQty from #t1
                                where CompanyCode = @CompanyCode
                                  and BranchCode  = @BranchCode
                                  and Year        = year (dateadd(mm,-10,@PeriodDate))
                                  and Month       = month(dateadd(mm,-10,@PeriodDate))
                                  and NewPartNo   = @cur_NewPartNo
                                  and PartNo      = #Suggor.PartNo),
                        IV  = (select DemandQty from #t1
                                where CompanyCode = @CompanyCode
                                  and BranchCode  = @BranchCode
                                  and Year        = year (dateadd(mm,-09,@PeriodDate))
                                  and Month       = month(dateadd(mm,-09,@PeriodDate))
                                  and NewPartNo   = @cur_NewPartNo
                                  and PartNo      = #Suggor.PartNo),
                        V   = (select DemandQty from #t1
                                where CompanyCode = @CompanyCode
                                  and BranchCode  = @BranchCode
                                  and Year        = year (dateadd(mm,-08,@PeriodDate))
                                  and Month       = month(dateadd(mm,-08,@PeriodDate))
                                  and NewPartNo   = @cur_NewPartNo
                                  and PartNo      = #Suggor.PartNo),
                        VI  = (select DemandQty from #t1
                                where CompanyCode = @CompanyCode
                                  and BranchCode  = @BranchCode
                                  and Year        = year (dateadd(mm,-07,@PeriodDate))
                                  and Month       = month(dateadd(mm,-07,@PeriodDate))
                                  and NewPartNo   = @cur_NewPartNo
                                  and PartNo      = #Suggor.PartNo),
                        'AOS',@SuggorDate,
                        VII = (select DemandQty from #t1
                                where CompanyCode = @CompanyCode
                                  and BranchCode  = @BranchCode
                                  and Year        = year (dateadd(mm,-06,@PeriodDate))
                                  and Month       = month(dateadd(mm,-06,@PeriodDate))
                                  and NewPartNo   = @cur_NewPartNo
                                  and PartNo      = #Suggor.PartNo),
                        VIII= (select DemandQty from #t1
                                where CompanyCode = @CompanyCode
                                  and BranchCode  = @BranchCode
                                  and Year        = year (dateadd(mm,-05,@PeriodDate))
                                  and Month       = month(dateadd(mm,-05,@PeriodDate))
                                  and NewPartNo   = @cur_NewPartNo
                                  and PartNo      = #Suggor.PartNo),
                        IX  = (select DemandQty from #t1
                                where CompanyCode = @CompanyCode
                                  and BranchCode  = @BranchCode
                                  and Year        = year (dateadd(mm,-04,@PeriodDate))
                                  and Month       = month(dateadd(mm,-04,@PeriodDate))
                                  and NewPartNo   = @cur_NewPartNo
                                  and PartNo      = #Suggor.PartNo),
                        X   = (select DemandQty from #t1
                                where CompanyCode = @CompanyCode
                                  and BranchCode  = @BranchCode
                                  and Year        = year (dateadd(mm,-03,@PeriodDate))
                                  and Month       = month(dateadd(mm,-03,@PeriodDate))
                                  and NewPartNo   = @cur_NewPartNo
                                  and PartNo      = #Suggor.PartNo),
                        XI  = (select DemandQty from #t1
                                where CompanyCode = @CompanyCode
                                  and BranchCode  = @BranchCode
                                  and Year        = year (dateadd(mm,-02,@PeriodDate))
                                  and Month       = month(dateadd(mm,-02,@PeriodDate))
                                  and NewPartNo   = @cur_NewPartNo
                                  and PartNo      = #Suggor.PartNo),
                        XII = (select DemandQty from #t1
                                where CompanyCode = @CompanyCode
                                  and BranchCode  = @BranchCode
                                  and Year        = year (dateadd(mm,-01,@PeriodDate))
                                  and Month       = month(dateadd(mm,-01,@PeriodDate))
                                  and NewPartNo   = @cur_NewPartNo
                                  and PartNo      = #Suggor.PartNo)                        
                   from #Suggor
                  where CompanyCode = @CompanyCode
                    and BranchCode  = @BranchCode
                    and NewPartNo   = @cur_NewPartNo
                    
         -- insert order balance table
            insert into spTrnPOrderBalance
                       (CompanyCode, BranchCode, POSNo, SupplierCode, PartNo, SeqNo, PartNoOriginal, 
                        POSDate, OrderQty, OnOrder, InTransit, Received, Located, DiscPct, 
                        PurchasePrice, CostPrice, ABCClass, MovingCode, WRSNo, WRSDate, TypeOfGoods, 
                        CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
                 values(@CompanyCode, @BranchCode, @POSNo, @cur_SupplierCode, @cur_NewPartNo, @Counter, 
                        @cur_NewPartNo, @SuggorDate, @cur_SuggorQty, @cur_SuggorQty, 0.00, 0.00, 0.00, 
                        @DiscPct, @cur_PurchasePrice, @cur_CostPrice, @cur_ABCClass, @cur_MovingCode, 
                        NULL, NULL, @TPGO, 'AOS', @SuggorDate, 'AOS', @SuggorDate)

         -- update item master table
            update spMstItems 
               set OnOrder = OnOrder + @cur_SuggorQty
             where CompanyCode = @CompanyCode
               and BranchCode  = @BranchCode
               and PartNo      = @cur_NewPartNo

         fetch next from SUGGOR 
               into @cur_NewPartNo, @cur_MovingCode, @cur_ABCClass, @cur_ProductType, @cur_PartCategory, 
                    @cur_TypeOfGoods, @cur_SupplierCode, @cur_CostPrice, @cur_PurchasePrice, @cur_OnHand, 
                    @cur_OnOrder, @cur_InTransit, @cur_AllocationSP, @cur_AllocationSR, @cur_AllocationSL, 
                    @cur_BackOrderSP, @cur_BackOrderSR, @cur_BackOrderSL, @cur_ReservedSP, @cur_ReservedSR, 
                    @cur_ReservedSL, @cur_AvailableQty, @cur_DmnAvg, @cur_OrderPoint, @cur_SafetyStokPoint, 
                    @cur_SuggorQty
      end
    close SUGGOR
    deallocate SUGGOR
 
    -- alert machine order
       set @Counter = isnull((select max(MessageID) from SysMessageBoards),0) + 1
    -- set @MessageText = 'AOS# ' + @AOS1 + '. ' + @AOS2
       set @MessageText = 'AOS-' + right(@BranchCode,2) + '#:' + @AOS1 + '. ' + @AOS2
       insert into SysMessageBoards
                  (MessageID, MessageHeader, MessageText, MessageTo, MessageTarget, MessageParams, 
                   DateFrom, DateTo, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
            values(@Counter, 'MACHINE ORDER', @MessageText, 'ALL', '', NULL,
                   NULL, NULL, 'AOS', @SuggorDate, 'AOS', @SuggorDate)
       select top 1 * from SysMessageBoards order by MessageID desc
	   
    drop table #Suggor
    drop table #t1
    drop table #t2
    drop table #t3
    drop table #t4

END

