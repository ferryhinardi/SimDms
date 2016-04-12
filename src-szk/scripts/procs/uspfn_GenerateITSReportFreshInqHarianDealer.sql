CREATE procedure [dbo].[uspfn_GenerateITSReportFreshInqHarianDealer]                        
 @StartDate varchar(8),                        
 @EndDate varchar(8),                    
 @StartDatePrevMonth1 varchar(8),                        
 @EndDatePrevMonth1 varchar(8),                  
 @StartDatePrevMonth2 varchar(8),                        
 @EndDatePrevMonth2 varchar(8),          
 @TipeKendaraan varchar(20) =''          
          
                  
as                        
begin                         
 --exec uspfn_GenerateITSReportFreshInqHarianDealer '20141001', '20141027','20140801','20140831','20140901','20140930'                  
 --exec uspfn_GenerateITSReportFreshInqHarianDealer '20150801', '20150831','20140601','20140630','20140701','20140731'                
 --declare @StartDate varchar(8)                        
 --declare @EndDate   varchar(8)                        
                        
 --set @StartDate = '20141201'                        
 --set @EndDate   = '20141201'                        
                        
                      
                      
                      
 select * into #INQ                        
   from (                      
     select                       
     CompanyCode, GroupNo, InqDate --,BranchCode ,TipeKendaraan ,Variant , ColourCode                      
     , count(InqDate) INQ                          
     from (                       
    select                       
     h.CompanyCode,  h.BranchCode ,mp.GroupNo --,h.TipeKendaraan ,h.Variant, h.ColourCode,                        
     ,convert(varchar,InquiryDate,112) InqDate                      
    from                       
     pmKdp h                        
     inner join     gnMstDealerOutletMapping mp on h.companycode=mp.dealercode and h.branchcode=mp.outletcode                  
    where                       
     convert(varchar,InquiryDate,112) between @StartDatePrevMonth1 and @EndDate                      
     and  case tipekendaraan  when '' then 'OTHERS' else isnull(tipekendaraan,'OTHERS') end =case @TipeKendaraan when '' then case tipekendaraan  when '' then 'OTHERS' else isnull(tipekendaraan,'OTHERS') end else @TipeKendaraan end          
     ) a                        
     group by                       
    CompanyCode, GroupNo,InqDate --,BranchCode ,TipeKendaraan ,Variant, ColourCode                         
      
     ) #INQ                      
                           
                           
                           
                        
select * into #SPK                        
from (                       
 select                       
   CompanyCode,  GroupNo,SPKDate --,TipeKendaraan ,BranchCode, Variant ,ColourCode                      
   , count(SPKDate) SPK                      
                         
    from (                       
   select                       
     h.CompanyCode, h.BranchCode,GroupNo --,h.TipeKendaraan --, h.Variant, h.ColourCode,                        
     ,convert(varchar,s.UpdateDate,112) SPKDate                          
   from                       
     pmKdp h, pmStatusHistory s  ,gnMstDealerOutletMapping mp                       
   where                       
     h.CompanyCode=s.CompanyCode                        
     and h.BranchCode=s.BranchCode                        
     and h.InquiryNumber=s.InquiryNumber                        
     and s.LastProgress='SPK'                        
     and h.companycode=mp.dealercode and h.branchcode=mp.outletcode                
     and convert(varchar,s.UpdateDate,112) between @StartDatePrevMonth1 and @EndDate                      
     and  case tipekendaraan  when '' then 'OTHERS' else isnull(tipekendaraan,'OTHERS') end = case @TipeKendaraan when '' then TipeKendaraan else @TipeKendaraan end          
     ) a                        
     group by CompanyCode,GroupNo, SPKDate --,BranchCode, TipeKendaraan ,Variant ,ColourCode                        
) #SPK                        
                        
     
select *                       
into                       
 #QRY                        
from (                      
   select                       
   distinct a.area,a.dealername,a.dealercode,a.GroupNo,              
   convert(varchar, b.thedate,112) InqDate,0 INQ, 0 SPK,isnull(sf.QtySF,0) QtySF from gnMstDealerMapping a                     
   left join (     
select a.CompanyCode,a.GroupNo,count(a.employeeid) QtySF    
  from (     
select h.joindate,h.CompanyCode,h.EmployeeID,h.Department,h.Position, h.PersonnelStatus,h.grade ,o.DealerCode,o.GroupNo    
, DeptCode = isnull((select top 1 HrEmployeeAchievement.Department from HrEmployeeAchievement where CompanyCode = h.CompanyCode and EmployeeID = h.EmployeeID and isDeleted != 1 and AssignDate <= getdate() order by AssignDate desc), '')            
from HrEmployee h    
inner join hremployeemutation e      
  on e.CompanyCode = h.CompanyCode       
    and e.EmployeeID = h.EmployeeID      
inner join (select max(MutationDate) as mutadate, CompanyCode, EmployeeID from hremployeemutation e      
group by CompanyCode, EmployeeID) t      
  on e.CompanyCode = t.CompanyCode       
    and e.EmployeeID = t.EmployeeID      
    and e.MutationDate = t.mutadate      
inner join gnMstDealerOutletMapping o      
  on o.DealerCode = e.CompanyCode      
    and o.OutletCode = e.BranchCode      
  where isnull(h.isDeleted, 0) = 0         
  and h.Position is not null     
  and h.Department = 'SALES'         
  and h.PersonnelStatus = '1'    
  and convert(varchar,h.joindate,112)<@EndDate    
  ) a              
  left join HrEmployeeSales e            
    on e.CompanyCode = a.CompanyCode            
   and e.EmployeeID = a.EmployeeID            
   where 1=1    
   and DeptCode='SALES'    
   and a.grade in('1','2','3','4')       
   group by a.companycode,a.groupno    
  )  sf on sf.companycode=a.dealercode and sf.GroupNo=a.GroupNo  and a.isactive=1                    
 cross join                      
   dbo.ExplodeDates(@StartDatePrevMonth1,@EndDate) b            
   where isActive=1             
)#QRY                      
          
                        
                        
                        
 Update #QRY                          
    set INQ          = isnull(( select INQ                       
        from #INQ                        
        where                       
         CompanyCode  =#QRY.DealerCode                
         and GroupNo= #QRY.GroupNo                                
         and InqDate      =#QRY.InqDate                        
         ),0)                                  
 where exists (select 1 from #INQ                        
     where CompanyCode  =#QRY.DealerCode               
     and GroupNo=#QRY.GroupNo                            
     and InqDate      =#QRY.InqDate                        
    )                        
                        
                        
 Update #QRY                        
     set SPK          = isnull(( select SPK                       
        from #SPK                        
        where                       
         CompanyCode  =#QRY.DealerCode                
          and GroupNo= #QRY.GroupNo                                  
         and SPkDate      =#QRY.InqDate                        
         ),0)                                 
 where exists (select 1 from #SPK                        
     where CompanyCOde  =#QRY.DealerCode               
     and GroupNo= #QRY.GroupNo                        
     and SPkDate      =#QRY.InqDate                        
    )                        
                          
                      
Select * from #QRY where inqDate  between @StartDate and @EndDate order by area,dealercode,groupno,InqDate  
select InqDate,sum(INQ) INQ,sum(SPK) SPK from #QRY where inqdate between @StartDatePrevMonth2 and @EndDatePrevMonth2 group by inqdate order by inqdate                  
select InqDate,sum(INQ) INQ,sum(SPK) SPK from #QRY where inqdate between @StartDatePrevMonth1 and @EndDatePrevMonth1 group by inqdate order by inqdate                  
                  
--select distinct InqDate from #QRY order by InqDate Desc                  
                          
 drop table #INQ, #SPK, #QRY                        
                         
                        
end 