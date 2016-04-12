/****** Object:  StoredProcedure [dbo].[usprpt_OmRpSalRgs039]    Script Date: 04/30/2012 10:31:11 ******/           
      
--exec usprpt_OmRpSalRgs040web '20160101','20161231','CABANG','','', '','','','1','1','0','0','0','0','','SALES'         
      
CREATE procedure [dbo].[usprpt_OmRpSalRgs040web]          
 @StartDate Datetime,          
 @EndDate Datetime,          
 @Area varchar(100),          
 @Dealer varchar(100),          
 @Outlet varchar(100),          
 @BranchHead varchar(100),          
 @SalesHead varchar(100),          
 --@SalesCoordinator varchar(100),          
 @Salesman varchar(100),          
 @PenjualanArea bit,          
 @PenjualanDealer bit,          
 @PenjualanOutlet bit,          
 @PenjualanModel bit,          
 @PenjualanWarna bit,          
 @PenjualanPerModel bit,          
 @MarketModel varchar(25),          
 @SalesType varchar(50)          
as           
    
--select * from tempinqsales40    
--select * from tempinqsalespivot    
          
declare @Query varchar(MAX)          
declare @Union varchar(MAX)          
declare @AVGNum int          
Declare @National varchar(10)          
set @National = (select top 1 ISNULL(ParaValue,0) from gnMstLookUpDtl          
                  where CodeID='QSLS' and LookUpValue='NATIONAL')          
          
set @AVGNum = MONTH(@EndDate) - Month(@StartDate) + 1          
print(@AVGNum)          
set @Union = ''          
set @Query = '          
Declare @MainTable table          
(          
 GroupNo     varchar(100)          
 , Area     varchar(100)          
 , CompanyCode   varchar(100)          
 , CompanyName   varchar(100)          
 , BranchCode   varchar(100)          
 , BranchName   varchar(100)          
 , BranchHeadID   varchar(100)          
 , BranchHeadName  varchar(100)          
 , SalesHeadID   varchar(100)          
 , SalesHeadName   varchar(100)          
 , SalesCoordinatorID varchar(100)          
 , SalesCoordinatorName varchar(100)          
 , SalesmanID   varchar(100)          
 , SalesmanName   varchar(100)          
 , ModelCatagory   varchar(100)          
 , SalesType    varchar(100)          
 , InvoiceNo    varchar(100)          
 , InvoiceDate   datetime          
 , SONo     varchar(100)          
 , SalesModelCode  varchar(100)          
 , Year     numeric(4,0)          
 , SalesModelDesc  varchar(150)          
 , FakturPolisiNo  varchar(100)          
 , FakturPolisiDate  datetime          
 , FakturPolisiDesc  varchar(150)          
 , MarketModel   varchar(100)          
 , ColourCode   varchar(100)          
 , ColourName   varchar(100)          
 , GroupMarketModel  varchar(100)          
 , ColumnMarketModel  varchar(100)          
 , JoinDate    datetime          
 , ResignDate   datetime          
 , GradeDate    datetime          
 , Grade     varchar(100)          
 , ChassisCode   varchar(100)          
 , ChassisNo    varchar(100)          
 , EngineCode   varchar(100)           
 , EngineNo    varchar(100)         
 , DODate     datetime          
 , GroupAreaCode      varchar(150)          
 , Processdate      datetime          
 , isexists    varchar(1)          
 , OtlMapLastUpdateBy      varchar(150)        
 --, COGS     numeric(18,0)          
 --, BeforeDiscDPP   numeric(18,0)          
 --, DiscExcludePPn  numeric(18,0)          
 --, DiscIncludePPn  numeric(18,0)          
 --, AfterDiscDPP   numeric(18,0)          
 --, AfterDiscPPn   numeric(18,0)          
 --, AfterDiscPPnBM  numeric(18,0)          
 --, AfterDiscTotal  numeric(18,0)          
 --, PPnBMPaid    numeric(18,0)          
 --, OthersDPP    numeric(18,0)          
 --, OthersPPn    numeric(18,0)          
 --, ShipAmt    numeric(18,0)          
 --, DepositAmt   numeric(18,0)          
 --, OthersAmt    numeric(18,0)          
)          
          
insert into @MainTable          
exec uspfn_OmInquirySalesWeb ''' + convert(varchar,@StartDate,112) + ''',''' + convert(varchar,@EndDate,112) + ''',''' + @Area + ''',''' + @Dealer + ''',''' + @Outlet + ''',''' + @BranchHead + ''',''' + @SalesHead + ''',''' +         
@Salesman + ''', ''' + @SalesType + '''          
          
select * into #t6 from(          
select isnull(GroupNo,9999) GroupNo          
  , isnull(Area, ''UN-SIGNED'') Area          
  , CompanyCode          
  , CompanyName          
  , BranchCode          
  , BranchName          
  , BranchHeadID          
  , BranchHeadName          
  , SalesHeadID          
  , SalesHeadName          
  , SalesCoordinatorID          
  , SalesCoordinatorName          
  , SalesmanID          
  , SalesmanName          
  , ModelCatagory          
  , MarketModel        
  , ColourCode          
  , ColourName          
  , InvoiceDate          
  , YEAR(InvoiceDate) Year           
  , MONTH(InvoiceDate) Month          
  , COUNT(SalesModelCode) SoldTotal           
  from @MainTable          
  group by GroupNo          
  , Area           
  , CompanyCode          
  , CompanyName          
  , BranchCode          
  , BranchName          
  , BranchHeadID          
  , BranchHeadName          
  , SalesHeadID          
  , SalesHeadName          
  , SalesCoordinatorID          
  , SalesCoordinatorName          
  , SalesmanID          
  , SalesmanName          
  , ModelCatagory          
  , MarketModel          
  , ColourCode          
  , ColourName          
  , InvoiceDate          
  , YEAR(InvoiceDate)           
  , MONTH(InvoiceDate)          
)#t6'          
-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------  
    
      
        
-------------------------------------          
-- If Area Check..          
-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------  
    
      
        
-------------------------------------          
if(@PenjualanArea = 1)          
begin          
 set @Query = @Query + '          
  select * into #Area from(          
   select GroupNo, Area'           
 set @Query = @Query + case when @PenjualanModel = 1 then ', ModelCatagory, MarketModel' else '' end          
 set @Query = @Query + case when @PenjualanWarna = 1 then ', ColourName' else '' end          
 set @Query = @Query + '          
     , Year           
     , isnull([1],0) Jan, isnull([2],0) Feb, isnull([3],0) Mar, isnull([4],0) Apr,isnull([5],0) May, isnull([6],0) Jun, isnull([7],0) Jul,isnull([8],0) Aug, isnull([9],0) Sep, isnull([10],0) Oct,isnull([11],0) Nov,isnull([12],0) Dec          
   from(          
    select GroupNo, Area'          
 set @Query = @Query + case when @PenjualanModel = 1 then ', ModelCatagory, MarketModel' else '' end          
 set @Query = @Query + case when @PenjualanWarna = 1 then ', ColourName' else '' end          
 set @Query = @Query + '          
     , Year           
     , Month          
     , SoldTotal          
    from #t6          
   ) as Header          
   pivot(SUM(SoldTotal)          
    for Month in ([1],[2],[3],[4],[5],[6],[7],[8],[9],[10],[11],[12])          
   )pvt          
  )#Area'          
           
 if(@Dealer = '')          
 begin          
  --Setting untuk Union terkahir          
  Set @Union = @Union + '          
       select ''1'' seq          
        ,''1'' seq2          
        , a.GroupNo          
        , a.Area'          
  set @Union = @Union + case when @PenjualanDealer = 1 then ', ''zzzz'' + a.Area CompanyName' else '' end          
  set @Union = @Union + case when @PenjualanOutlet = 1 then ', ''zzzz'' + a.Area BranchName' else '' end          
  set @Union = @Union + case when @PenjualanModel = 1 then ', a.ModelCatagory, a.MarketModel' else '' end          
  set @Union = @Union + case when @PenjualanWarna = 1 then ', a.ColourName' else '' end          
  set @Union = @Union + ', a.Year           
        , a.Jan, a.Feb, a.Mar, a.Apr, a.May, a.Jun, a.Jul, a.Aug, a.Sep, a.Oct, a.Nov, a.Dec          
        , (a.Jan + a.Feb + a.Mar + a.Apr + a.May + a.Jun + a.Jul + a.Aug + a.Sep + a.Oct + a.Nov + a.Dec) Total          
        , (a.Jan + a.Feb + a.Mar + a.Apr + a.May + a.Jun + a.Jul + a.Aug + a.Sep + a.Oct + a.Nov + a.Dec) / '          
  set @Union = @Union + convert(varchar,@AVGNum)          
  set @Union = @Union +' AVG          
        from #Area a          
       '          
  set @Union = @Union + case when @PenjualanPerModel = 1 then ' where a.MarketModel = '''+ @MarketModel + '''' else '' end          
 end          
end          
          
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- 
 
    
      
        
-------------------------------------          
-- If Penjualan Dealer Checked          
-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------  
    
      
        
-------------------------------------          
if(@PenjualanDealer = 1)          
begin          
 set @Query = @Query + '          
  select * into #Dealer from(          
   select GroupNo'          
 set @Query = @Query + case when @PenjualanArea = 1 then ', Area' else '' end          
 set @Query = @Query + case when @PenjualanArea = 1 then ', CompanyCode' else 'CompanyCode' end          
 set @Query = @Query + case when @PenjualanModel = 1 then ', ModelCatagory, MarketModel' else '' end          
 set @Query = @Query + case when @PenjualanWarna = 1 then ', ColourName' else '' end          
 set @Query = @Query + '          
    , Year           
    , isnull([1],0) Jan, isnull([2],0) Feb, isnull([3],0) Mar, isnull([4],0) Apr,isnull([5],0) May, isnull([6],0) Jun, isnull([7],0) Jul,isnull([8],0) Aug, isnull([9],0) Sep, isnull([10],0) Oct,isnull([11],0) Nov,isnull([12],0) Dec          
    from(          
     select GroupNo'          
 set @Query = @Query + case when @PenjualanArea = 1 then ', Area' else '' end          
 set @Query = @Query + case when @PenjualanArea = 1 then ', CompanyCode' else 'CompanyCode' end          
 set @Query = @Query + case when @PenjualanModel = 1 then ', ModelCatagory, MarketModel' else '' end          
 set @Query = @Query + case when @PenjualanWarna = 1 then ', ColourName' else '' end          
 set @Query = @Query + '          
    , Year           
    , Month          
    , SoldTotal          
   from #t6          
  ) as Header          
  pivot(SUM(SoldTotal)          
   for Month in ([1],[2],[3],[4],[5],[6],[7],[8],[9],[10],[11],[12])          
  )pvt          
 )#Dealer'          
          
 --Setting untuk Union terkahir          
 set @Union = @Union + case when @PenjualanArea = 1 and @Dealer = '' then ' UNION ' else '' end          
 Set @Union = @Union + ' select ''2'' seq          
       ,''1'' seq2          
       , a.GroupNo'          
 set @Union = @Union + case when @PenjualanArea = 1 then ', a.Area' else '' end          
 set @Union = @Union +', isnull(b.DealerAbbreviation, a.CompanyCode) CompanyName'          
 set @Union = @Union + case when @PenjualanOutlet = 1 then ', isnull(b.DealerAbbreviation, a.CompanyCode) BranchName' else '' end          
 set @Union = @Union + case when @PenjualanModel = 1 then ', a.ModelCatagory, a.MarketModel' else '' end          
 set @Union = @Union + case when @PenjualanWarna = 1 then ', a.ColourName' else '' end          
 set @Union = @Union + ', a.Year           
       , a.Jan, a.Feb, a.Mar, a.Apr, a.May, a.Jun, a.Jul, a.Aug, a.Sep, a.Oct, a.Nov, a.Dec          
       , (a.Jan + a.Feb + a.Mar + a.Apr + a.May + a.Jun + a.Jul + a.Aug + a.Sep + a.Oct + a.Nov + a.Dec) Total           
       , (a.Jan + a.Feb + a.Mar + a.Apr + a.May + a.Jun + a.Jul + a.Aug + a.Sep + a.Oct + a.Nov + a.Dec) / '          
 set @Union = @Union + convert(varchar,@AVGNum)          
 set @Union = @Union +' AVG          
      from #Dealer a          
      Left Join GnMstDealerMapping b on a.CompanyCode = b.DealerCode          
      '          
 set @Union = @Union + case when @PenjualanPerModel = 1 then ' where a.MarketModel = '''+ @MarketModel + '''' else '' end          
          
end          
          
-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------  
    
      
        
-------------------------------------          
-- If Penjualan Outlet Checked          
-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------  
    
      
        
-------------------------------------          
if(@PenjualanOutlet = 1)          
begin          
 set @Query = @Query + '       
  select * into #Outlet from(          
   select GroupNo'          
 set @Query = @Query + case when @PenjualanArea = 1 then ', Area' else '' end          
 set @Query = @Query + case when @PenjualanArea = 1          
        then ', CompanyCode'           
        else  case when @PenjualanDealer = 1           
           then 'CompanyCode'           
           else ''          
           end          
        end          
 set @Query = @Query + case when @PenjualanArea = 1 or @PenjualanDealer = 1 then ', BranchCode' else 'BranchCode' end          
 set @Query = @Query + case when @PenjualanModel = 1 then ', ModelCatagory, MarketModel' else '' end          
 set @Query = @Query + case when @PenjualanWarna = 1 then ', ColourName' else '' end          
 set @Query = @Query + '          
     , Year           
     , isnull([1],0) Jan, isnull([2],0) Feb, isnull([3],0) Mar, isnull([4],0) Apr,isnull([5],0) May, isnull([6],0) Jun, isnull([7],0) Jul,isnull([8],0) Aug, isnull([9],0) Sep, isnull([10],0) Oct,isnull([11],0) Nov,isnull([12],0) Dec          
   from(          
    select GroupNo'          
 set @Query = @Query + case when @PenjualanArea = 1 then ', Area' else '' end          
 set @Query = @Query + case when @PenjualanArea = 1          
        then ', CompanyCode'           
        else  case when @PenjualanDealer = 1           
           then 'CompanyCode'           
           else ''          
           end          
        end          
 set @Query = @Query + case when @PenjualanArea = 1 or @PenjualanDealer = 1 then ', BranchCode' else 'BranchCode' end          
 set @Query = @Query + case when @PenjualanModel = 1 then ', ModelCatagory, MarketModel' else '' end          
 set @Query = @Query + case when @PenjualanWarna = 1 then ', ColourName' else '' end          
 set @Query = @Query + '          
     , Year           
     , Month          
     , SoldTotal          
    from #t6          
   ) as Header          
   pivot(SUM(SoldTotal)          
    for Month in ([1],[2],[3],[4],[5],[6],[7],[8],[9],[10],[11],[12])          
   )pvt          
  )#Outlet'          
          
 --Setting untuk Union terkahir          
 set @Union = @Union + case when @PenjualanArea = 1 or @PenjualanDealer = 1 then ' UNION ' else '' end          
 Set @Union = @Union + ' select ''3'' seq          
       ,''1'' seq2          
       , a.GroupNo'          
 set @Union = @Union + case when @PenjualanArea = 1 then ', a.Area' else '' end          
 set @Union = @Union + case when @PenjualanDealer = 1 then ', isnull(c.DealerAbbreviation, a.CompanyCode) DealerAbbreviation' else '' end          
 set @Union = @Union + ', isnull(b.OutletAbbreviation, a.BranchCode) OutletAbbreviation'          
 set @Union = @Union + case when @PenjualanModel = 1 then ', a.ModelCatagory, a.MarketModel' else '' end          
 set @Union = @Union + case when @PenjualanWarna = 1 then ', a.ColourName' else '' end          
 set @Union = @Union + ', a.Year           
       , a.Jan, a.Feb, a.Mar, a.Apr, a.May, a.Jun, a.Jul, a.Aug, a.Sep, a.Oct, a.Nov, a.Dec          
       , (a.Jan + a.Feb + a.Mar + a.Apr + a.May + a.Jun + a.Jul + a.Aug + a.Sep + a.Oct + a.Nov + a.Dec) Total          
       , (a.Jan + a.Feb + a.Mar + a.Apr + a.May + a.Jun + a.Jul + a.Aug + a.Sep + a.Oct + a.Nov + a.Dec) / '          
 set @Union = @Union + convert(varchar,@AVGNum)          
 set @Union = @Union +' AVG          
      from #Outlet a          
      Left Join GnMstDealerOutletMapping b on a.CompanyCode = b.DealerCode          
          and a.BranchCode = b.OutletCode          
      '          
 set @Union = @Union + case when @PenjualanDealer = 1           
       then 'Left Join GnMstDealerMapping c on a.CompanyCode = c.DealerCode'          
       else ''          
       end          
 set @Union = @Union + case when @PenjualanPerModel = 1 then ' where a.MarketModel = '''+ @MarketModel + '''' else '' end          
          
end          
          
-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------  
      
-------------------------------------          
-- NSDS Count          
-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------  
    
      
        
-------------------------------------          
--if @National = 1 and (@SalesType in ('RETAIL', 'SALES', 'FPOL'))          
 --begin          
 --set @Query = @Query + '          
 --select * into #NSDSMain from(          
 -- SELECT case when '''+ @SalesType +''' in (''RETAIL'',''SALES'') then YEAR(a.DODate) else YEAR(a.ProcessDate) end YEAR          
 --  , case when '''+ @SalesType +''' in (''RETAIL'',''SALES'') then MONTH(a.DODate) else MONTH(a.ProcessDate) end MONTH          
 --  , COUNT(a.SalesModelCode) SoldTotal          
 -- FROM omHstInquirySalesNSDS a          
 -- where case when '''+ @SalesType + ''' in (''RETAIL'',''SALES'') then a.DoDate else a.ProcessDate end between ''' + convert(varchar,@StartDate,112) + ''' and ''' + convert(varchar,@EndDate,112) + '''          
 --  and a.GroupAreaCode = ''3''          
 -- group by a.SalesModelCode          
 --  , case when '''+ @SalesType +''' in (''RETAIL'',''SALES'') then YEAR(a.DODate) else YEAR(a.ProcessDate) end          
 --  , case when '''+ @SalesType +''' in (''RETAIL'',''SALES'') then MONTH(a.DODate) else MONTH(a.ProcessDate) end          
 --)#NSDSMain          
           
 --select * into #NSDSPivot from(          
 -- select '          
 --set @Query = @Query + case when @PenjualanPerModel = 1 then '   , MarketModel' else '' end          
 --set @Query = @Query + ' Year          
 --  , isnull([1],0) Jan, isnull([2],0) Feb, isnull([3],0) Mar, isnull([4],0) Apr,isnull([5],0) May, isnull([6],0) Jun, isnull([7],0) Jul,isnull([8],0) Aug, isnull([9],0) Sep, isnull([10],0) Oct,isnull([11],0) Nov,isnull([12],0) Dec          
 -- from(          
 --  select Year          
 --   , Month'          
 --set @Query = @Query + case when @PenjualanPerModel = 1 then '   , MarketModel' else '' end          
 --set @Query = @Query +'   , SoldTotal          
 --  from #NSDSMain          
 -- ) as Header          
 --  pivot(SUM(SoldTotal)          
 --   for MONTH in ([1],[2],[3],[4],[5],[6],[7],[8],[9],[10],[11],[12])          
 -- )pvt          
 --)#NSDSPivot'          
           
 ----Setting untuk Union terkahir          
 --set @Union = @Union + case when @PenjualanArea = 1 or @PenjualanDealer = 1 then ' UNION ' else '' end          
 --Set @Union = @Union + ' select ''9'' seq          
 --      ,''9'' seq2          
 --      , ''NSDS'''          
 --set @Union = @Union + case when @PenjualanArea = 1 then ', ''NSDS''' else '' end      
 --set @Union = @Union + case when @PenjualanDealer = 1 then ', ''NSDS''' else '' end          
 --set @Union = @Union + case when @PenjualanOutlet = 1 then ', ''NSDS''' else '' end          
 --set @Union = @Union + case when @PenjualanModel = 1 then ', ''NSDS'', ''NSDS''' else '' end          
 --set @Union = @Union + case when @PenjualanWarna = 1 then ', ''NSDS''' else '' end          
 --set @Union = @Union + ', a.Year           
 --      , a.Jan, a.Feb, a.Mar, a.Apr, a.May, a.Jun, a.Jul, a.Aug, a.Sep, a.Oct, a.Nov, a.Dec          
 --      , (a.Jan + a.Feb + a.Mar + a.Apr + a.May + a.Jun + a.Jul + a.Aug + a.Sep + a.Oct + a.Nov + a.Dec) Total          
 --      , (a.Jan + a.Feb + a.Mar + a.Apr + a.May + a.Jun + a.Jul + a.Aug + a.Sep + a.Oct + a.Nov + a.Dec) / '          
 --set @Union = @Union + convert(varchar,@AVGNum)          
 --set @Union = @Union +' AVG          
 --     from #NSDSPivot a          
 --     '          
 --set @Union = @Union + case when @PenjualanPerModel = 1 then ' where a.MarketModel = '''+ @MarketModel + '''' else '' end          
--end          
          
          
set @Query = @Query + ' Select * into #Union from (' + @Union + ')#Union'          
set @Query = @Query + ' select a.*'          
set @Query = @Query + ',(select count(*) from #union b where a.GroupNo = b.GroupNo and a.Year = b.Year and a.Seq = b.Seq and '           
set @Query = @Query + case when @PenjualanArea = 1           
       then ' b.Area = a.area'          
       else ''          
       end          
set @Query = @Query + case when @PenjualanDealer = 1           
       then ' and b.CompanyName = a.CompanyName'          
       else ''          
       end          
set @Query = @Query + case when @PenjualanOutlet = 1          
       then ' and b.BranchName = a.BranchName'          
       else ''          
       end          
set @Query = @Query + ') rowType'          
set @Query = @Query + ', isnull((Select TOP 1 isnull(Convert(varchar,Year) + case when Month < 10 then ''0'' + convert(varchar,month) else convert(varchar,month) end,''' + convert(varchar,Year(@StartDate)) + case when Month(@StartDate) <10 then '0' +    
       
convert(varchar,MONTH(@StartDate)) else convert(varchar,MONTH(@StartDate)) end + ''')          
       from #t6 where GroupNo = a.GroupNo          
          and CompanyName = a.CompanyName'          
set @Query = @Query + case when @PenjualanOutlet = 1           
       then ' and BranchName = a.BranchName'          
       else ''          
       end          
set @Query = @Query + case when @PenjualanModel = 1           
       then ' and ModelCatagory  = a.ModelCatagory'          
       else ''          
       end          
set @Query = @Query + ' order by Year asc,Month asc'          
set @Query = @Query + ' ),''' + convert(varchar,Year(@StartDate)) + case when Month(@StartDate) <10 then '0' + convert(varchar,MONTH(@StartDate)) else convert(varchar,MONTH(@StartDate)) end + ''') StartDate'          
set @Query = @Query + ', isnull((Select TOP 1 Convert(varchar,Year) + case when Month < 10 then ''0'' + convert(varchar,month) else convert(varchar,month) end          
       from #t6 where GroupNo = a.GroupNo          
          and CompanyName = a.CompanyName'          
set @Query = @Query + case when @PenjualanOutlet = 1           
       then ' and BranchName = a.BranchName'          
       else ''          
       end          
set @Query = @Query + case when @PenjualanModel = 1           
       then ' and ModelCatagory  = a.ModelCatagory'          
       else ''          
       end          
set @Query = @Query + ' order by Year desc,Month desc'          
set @Query = @Query + ' ),''' + convert(varchar,Year(@EndDate)) + case when Month(@EndDate) <10 then '0' + convert(varchar,MONTH(@EndDate)) else convert(varchar,MONTH(@EndDate)) end + ''') EndDate'          
set @Query = @Query + ' from #Union a order by a.Year,a.GroupNo, a.Area asc ,CompanyName,Seq desc'          
set @Query = @Query + case when @PenjualanOutlet = 1           
       then ' ,BranchName asc'          
       else ''          
       end          
set @Query = @Query + case when @PenjualanModel = 1           
       then ' , ModelCatagory asc, MarketModel asc'          
       else ''          
       end          
set @Query = @Query + ',Seq2 asc'          
          
set @Query = @Query + '          
select * into #DealerPivot from          
(          
 Select convert(varchar,GroupNo) + ''0'' GroupNo          
   , Area          
   , DealerAbbreviation          
   , Year          
   , isnull([1],0) Jan, isnull([2],0) Feb, isnull([3],0) Mar, isnull([4],0) Apr,isnull([5],0) May, isnull([6],0) Jun, isnull([7],0) Jul,isnull([8],0) Aug, isnull([9],0) Sep, isnull([10],0) Oct,isnull([11],0) Nov,isnull([12],0) Dec             
 from(          
  select a.GroupNo          
   , a.Area          
   , isnull(a.DealerAbbreviation, a.DealerCode) DealerAbbreviation          
   , b.OutletAbbreviation          
   , case when (a.CreatedDate is null or a.CreatedDate < convert(datetime,''' + convert(varchar,@StartDate,102) + '''))          
     then YEAR(convert(datetime,''' + convert(varchar, @StartDate, 102) + '''))           
     else YEAR(a.CreatedDate)           
     end Year          
   , case when (a.CreatedDate is null or a.CreatedDate < convert(datetime,''' + convert(varchar,@StartDate,102) + '''))          
     then MONTH(convert(datetime,''' + convert(varchar, @StartDate, 102) + '''))          
     else MONTH(a.CreatedDate)           
     end Month          
  from gnMstDealerMapping a          
  left join gnMstDealerOutletMapping b on a.DealerCode = b.DealerCode          
   where a.DealerCode like case when ''' + @Dealer + ''' <> '''' then ''' + @Dealer + ''' else ''%%'' end          
    and b.OutletCode like case when ''' + @Outlet + ''' <> '''' then ''' + @Outlet + ''' else ''%%'' end           
    and isnull(a.Area,'''') like case when ''' + @Area + ''' <> '''' then ''' + @Area + ''' else ''%%'' end           
   and '+ case when @PenjualanOutlet = 1 then 'b.OutletAbbreviation' else  'a.DealerAbbreviation' end + ' in (select distinct ' + case when @PenjualanOutlet = 1 then 'BranchName' else 'CompanyName' end + ' from #Union)          
 )as Header          
 pivot(COUNT(OutletAbbreviation)          
  for Month in ([1],[2],[3],[4],[5],[6],[7],[8],[9],[10],[11],[12])          
 )pvt          
)#DealerPivot          
          
select * into #AreaPivot from          
(          
 Select  convert(varchar,GroupNo) + ''1'' GroupNo          
   , Area          
   , Area DealerAbbreviation           
   , Year          
   , isnull([1],0) Jan, isnull([2],0) Feb, isnull([3],0) Mar, isnull([4],0) Apr,isnull([5],0) May, isnull([6],0) Jun, isnull([7],0) Jul,isnull([8],0) Aug, isnull([9],0) Sep, isnull([10],0) Oct,isnull([11],0) Nov,isnull([12],0) Dec             
 from(          
  select a.GroupNo          
   , a.Area          
   , a.DealerAbbreviation          
   , case when (a.CreatedDate is null or a.CreatedDate <  convert(datetime,''' + convert(varchar,@StartDate,102) + '''))          
     then  YEAR(convert(datetime,''' + convert(varchar,@StartDate,102) + '''))           
     else YEAR(a.CreatedDate)           
     end Year          
   , case when (a.CreatedDate is null or a.CreatedDate <  convert(datetime,''' + convert(varchar,@StartDate,102) + '''))          
     then MONTH(convert(datetime,''' + convert(varchar,@StartDate,102) + '''))          
     else MONTH(a.CreatedDate)          
     end Month          
  from gnMstDealerMapping a          
  where DealerCode in (          
    select a.DealerCode          
   from gnMstDealerMapping a          
   left join gnMstDealerOutletMapping b on a.DealerCode = b.DealerCode          
   where a.DealerCode like case when ''' + @Dealer + ''' <> '''' then ''' + @Dealer + ''' else ''%%'' end          
    and b.OutletCode like case when ''' + @Outlet + ''' <> '''' then ''' + @Outlet + ''' else ''%%'' end           
    and isnull(a.Area,'''') like case when ''' + @Area + ''' <> '''' then ''' + @Area + ''' else ''%%'' end           
   and '+ case when @PenjualanOutlet = 1 then 'b.OutletAbbreviation' else  'a.DealerAbbreviation' end + ' in (select distinct ' + case when @PenjualanOutlet = 1 then 'BranchName' else 'CompanyName' end + ' from #Union)          
  )          
 )as Header          
 pivot(COUNT(DealerAbbreviation)          
  for Month in ([1],[2],[3],[4],[5],[6],[7],[8],[9],[10],[11],[12])          
 )pvt          
)#AreaPivot          
          
select * from #DealerPivot          
union          
select * from #AreaPivot          
order by Year,GroupNo, Area asc'          
          
print (@Query)          
exec (@Query)          
          
