-- usprpt_OmRpSalRgs041 '2012-1-1','2012-4-30','','','','','','','',0,0,''    
-- 1-Apr-2012 s/d 1-Dec-2012,ALL,Cabang,ALL,PT. BUANA INDOMOBIL TRADA - BUMI SERPONG DAMAI,ALL,ALL,ALL,0,    
-- !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!ATENTION!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!    
-- Untuk semua yg ingin merubah code ini..Harap diperhatikan n harap di pelajari dulu maksud dan tujuan seluruh code di bawah ini    
-- Karena SP ini untuk fleksible report!!!and it's like hell to build..please be gentle and don't forget to look at designer too.    
-- -------------------------------------------------------------------------------------------------------------------------------    
-- Created By : Seandy A.K.    
-- Created Date : 7-5-2012 s/d 15-5-2012    
-- !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!    
-- usprpt_OmRpSalRgs041web '20160101','20160229','CABANG','','','','','','',0,0,''    
     
alter procedure [dbo].[usprpt_OmRpSalRgs041web]    
 @StartDate Datetime,    
 @EndDate Datetime,    
 @Area varchar(15),    
 @Dealer varchar(15),    
 @Outlet varchar(15),    
 @BranchHead varchar(100),    
 @SalesHead varchar(100),    
 @SalesCoordinator varchar(100),    
 @Salesman varchar(100),    
 @ProdOutlet bit,    
 @ProdModel bit,    
 @MarketModel varchar(25)    
    
as     
  
--select * from tempinqsales41  

Declare @TransferTable table    
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
    
insert into @TransferTable    
exec uspfn_OmInquirySalesweb @StartDate, @EndDate, @Area, @Dealer, @Outlet, @BranchHead, @SalesHead,  @Salesman, 'WHOLESALE'    
    
select * into #t5 from(    
select isnull(GroupNo, 9999) GroupNo    
  , isnull(Area, 'UN-SIGNED') Area    
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
  , InvoiceDate    
  , FakturPolisiNo    
  , FakturPolisiDate    
  , SalesModelCode    
  , ModelCatagory    
  , SalesType    
  , MarketModel    
  , ColourName    
  from @TransferTable    
)#t5    
    
Declare @RetailTransferTable table    
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
    
insert into @RetailTransferTable    
exec uspfn_OmInquirySalesweb @StartDate, @EndDate, @Area, @Dealer, @Outlet, @BranchHead, @SalesHead,  @Salesman, 'RETAIL'    
    
select * into #RtlTbl from(    
select isnull(GroupNo, 0) GroupNo    
  , isnull(Area, 'UN-SIGNED') Area    
  , CompanyCode    
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
  , InvoiceDate    
  , FakturPolisiNo    
  , FakturPolisiDate    
  , SalesModelCode    
  , ModelCatagory    
  , SalesType    
  , MarketModel    
  , ColourName    
  from @RetailTransferTable    
)#RtlTbl    
    
select * into #t6 from    
(    
 Select GroupNo    
  , Area    
  , CompanyCode    
  , BranchCode    
  , BranchHeadID    
  , BranchHeadName    
  , SalesHeadID    
  , SalesHeadName    
  , SalesCoordinatorID    
  , SalesCoordinatorName    
  , SalesmanID    
  , SalesmanName    
  , InvoiceDate    
  , FakturPolisiNo    
  , FakturPolisiDate    
  , SalesModelCode    
  , ModelCatagory    
  , SalesType    
  , MarketModel    
  , ColourName    
  , YEAR(InvoiceDate) Year     
  , MONTH(InvoiceDate) Month    
  , COUNT(SalesModelCode) SoldTotal    
 from #t5    
   group by GroupNo    
  , Area    
  , CompanyCode    
  , BranchCode    
  , BranchHeadID    
  , BranchHeadName    
  , SalesHeadID    
  , SalesHeadName    
  , SalesCoordinatorID    
  , SalesCoordinatorName    
  , SalesmanID    
  , SalesmanName    
  , InvoiceDate    
  , FakturPolisiNo    
  , FakturPolisiDate    
  , SalesModelCode    
  , ModelCatagory    
  , SalesType    
  , MarketModel    
  , ColourName    
)#t6    
    
select * into #t7 from    
(    
 select GroupNo    
  , Area    
  , CompanyCode    
  , BranchCode    
  , BranchHeadID    
  , BranchHeadName    
  , SalesHeadID    
  , SalesHeadName    
  , SalesCoordinatorID    
  , SalesCoordinatorName    
  , SalesmanID    
  , SalesmanName    
  , InvoiceDate    
  , FakturPolisiNo    
  , FakturPolisiDate    
  , SalesModelCode    
  , ModelCatagory    
  , SalesType    
  , MarketModel    
  , ColourName    
  , YEAR(FakturPolisiDate) Year    
  , MONTH(FakturPolisiDate) Month    
  , COUNT(SalesModelCode) SoldTotal    
 from #RtlTbl    
   group by GroupNo    
  , Area    
  , CompanyCode    
  , BranchCode    
  , BranchHeadID    
  , BranchHeadName    
  , SalesHeadID    
  , SalesHeadName    
  , SalesCoordinatorID    
  , SalesCoordinatorName    
  , SalesmanID    
  , SalesmanName    
  , InvoiceDate    
  , FakturPolisiNo    
  , FakturPolisiDate    
  , SalesModelCode    
  , ModelCatagory    
  , SalesType    
  , MarketModel    
  , ColourName    
)#t7    
    
select * into #Wholesale from(    
 select GroupNo    
   , Area    
   , CompanyCode    
   , case when @ProdOutlet = 1 then BranchCode else '' end BranchCode    
   , (case when @ProdModel = 1 then MarketModel else '' end) MarketModel    
   , isnull([1],0) Jan, isnull([2],0) Feb, isnull([3],0) Mar, isnull([4],0) Apr,isnull([5],0) May, isnull([6],0) Jun, isnull([7],0) Jul,isnull([8],0) Aug, isnull([9],0) Sep, isnull([10],0) Oct,isnull([11],0) Nov,isnull([12],0) Dec    
 from(    
  select GroupNo    
   , Area    
   , CompanyCode    
   , case when @ProdOutlet = 1 then BranchCode else '' end BranchCode    
   , (case when @ProdModel = 1 then MarketModel else '' end) MarketModel    
   , Year     
   , Month    
   , SoldTotal    
  from #t6    
 ) as Header    
 pivot(SUM(SoldTotal)    
  for Month in ([1],[2],[3],[4],[5],[6],[7],[8],[9],[10],[11],[12])    
 )pvt    
)#Wholesale    
    
select * into #Retail from(    
 select GroupNo    
   , Area    
   , CompanyCode    
   , case when @ProdOutlet = 1 then BranchCode else '' end BranchCode    
   , (case when @ProdModel = 1 then MarketModel else '' end) MarketModel    
   , isnull([1],0) Jan, isnull([2],0) Feb, isnull([3],0) Mar, isnull([4],0) Apr,isnull([5],0) May, isnull([6],0) Jun, isnull([7],0) Jul,isnull([8],0) Aug, isnull([9],0) Sep, isnull([10],0) Oct,isnull([11],0) Nov,isnull([12],0) Dec    
 from(    
  select GroupNo    
   , Area    
   , CompanyCode    
   , case when @ProdOutlet = 1 then BranchCode else '' end BranchCode    
   , (case when @ProdModel = 1 then MarketModel else '' end) MarketModel    
   , Year     
   , Month    
   , SoldTotal    
  from #t7    
 ) as Header    
 pivot(SUM(SoldTotal)    
  for Month in ([1],[2],[3],[4],[5],[6],[7],[8],[9],[10],[11],[12])    
 )pvt    
)#Retail    
    
    
Declare @ResultTable table    
(    
 GroupNo     varchar(100)    
 , Area     varchar(100)    
 , CompanyCode   varchar(15)    
 , CompanyName   varchar(100)    
 , BranchCode   varchar(15)    
 , BranchName   varchar(100)    
 , SalesType    varchar(25)    
 , MarketModel   varchar(255)    
 , Jan     int    
 , Feb     int    
 , Mar     int    
 , Apr     int    
 , May     int    
 , Jun     int    
 , Jul     int    
 , Aug     int    
 , Sep     int    
 , Oct     int    
 , Nov     int    
 , Dec     int    
 , Total     int    
)    
    
if(@ProdModel = 1)    
begin    
 insert into @ResultTable    
 select a.GroupNo    
   , a.Area    
   , a.CompanyCode    
   , isnull(b.DealerAbbreviation, a.CompanyCode) CompanyName    
   , a.BranchCode    
   , isnull(c.OutletAbbreviation, a.BranchCode) BranchName    
   , 'WHOLESALE' SalesType    
   , a.MarketModel    
   , a.Jan, a.Feb, a.Mar, a.Apr,a.May, a.Jun, a.Jul,a.Aug, a.Sep, a.Oct,a.Nov,a.Dec    
   , (a.Jan + a.Feb + a.Mar + a.Apr + a.May + a.Jun + a.Jul + a.Aug + a.Sep + a.Oct + a.Nov + a.Dec) Total     
  from #Wholesale a    
  left join gnMstDealerMapping b on a.CompanyCode = b.DealerCode    
  left join gnMstDealerOutletMapping c on a.CompanyCode = c.DealerCode    
   and a.BranchCode = c.OutletCode    
  where a.MarketModel = @MarketModel    
  order by a.GroupNo,a.Area,a.CompanyCode,a.BranchCode    
     
 insert into @ResultTable    
 select a.GroupNo    
   , a.Area    
   , a.CompanyCode    
   , isnull(b.DealerAbbreviation, a.CompanyCode) CompanyName    
   , a.BranchCode    
   , isnull(c.OutletAbbreviation, a.BranchCode) BranchName    
   , 'RETAIL' SalesType    
   , a.MarketModel    
   , a.Jan, a.Feb, a.Mar, a.Apr,a.May, a.Jun, a.Jul,a.Aug, a.Sep, a.Oct,a.Nov,a.Dec    
   , (a.Jan + a.Feb + a.Mar + a.Apr + a.May + a.Jun + a.Jul + a.Aug + a.Sep + a.Oct + a.Nov + a.Dec) Total     
  from #Retail a    
  left join gnMstDealerMapping b on a.CompanyCode = b.DealerCode    
  left join gnMstDealerOutletMapping c on a.CompanyCode = c.DealerCode    
   and a.BranchCode = c.OutletCode    
  where a.MarketModel = @MarketModel    
  order by a.GroupNo,a.Area,a.CompanyCode,a.BranchCode    
end    
else    
begin    
 insert into @ResultTable    
 select a.GroupNo    
   , a.Area    
   , a.CompanyCode    
   , isnull(b.DealerAbbreviation, a.CompanyCode) CompanyName    
   , a.BranchCode    
   , isnull(c.OutletAbbreviation, a.BranchCode) BranchName    
   , 'WHOLESALE' SalesType    
   , a.MarketModel    
   , a.Jan, a.Feb, a.Mar, a.Apr,a.May, a.Jun, a.Jul,a.Aug, a.Sep, a.Oct,a.Nov,a.Dec    
   , (a.Jan + a.Feb + a.Mar + a.Apr + a.May + a.Jun + a.Jul + a.Aug + a.Sep + a.Oct + a.Nov + a.Dec) Total     
  from #Wholesale a    
  left join gnMstDealerMapping b on a.CompanyCode = b.DealerCode    
  left join gnMstDealerOutletMapping c on a.CompanyCode = c.DealerCode    
   and a.BranchCode = c.OutletCode    
  order by a.GroupNo,a.Area,a.CompanyCode,a.BranchCode    
    
 insert into @ResultTable    
 select a.GroupNo    
   , a.Area    
   , a.CompanyCode    
   , isnull(b.DealerAbbreviation, a.CompanyCode) CompanyName    
   , a.BranchCode    
   , isnull(c.OutletAbbreviation, a.BranchCode) BranchName    
   , 'RETAIL' SalesType    
   , a.MarketModel    
   , a.Jan, a.Feb, a.Mar, a.Apr,a.May, a.Jun, a.Jul,a.Aug, a.Sep, a.Oct,a.Nov,a.Dec    
   , (a.Jan + a.Feb + a.Mar + a.Apr + a.May + a.Jun + a.Jul + a.Aug + a.Sep + a.Oct + a.Nov + a.Dec) Total     
 from #Retail a    
 left join gnMstDealerMapping b on a.CompanyCode = b.DealerCode    
 left join gnMstDealerOutletMapping c on a.CompanyCode = c.DealerCode    
  and a.BranchCode = c.OutletCode    
  order by a.GroupNo,a.Area,a.CompanyCode,a.BranchCode    
end    
    
select *     
from @ResultTable a    
order by a.GroupNo    
   , a.Area    
   , a.CompanyCode    
   , a.CompanyName    
   , a.BranchCode    
   , a.BranchName    
   , a.SalesType desc
   
   