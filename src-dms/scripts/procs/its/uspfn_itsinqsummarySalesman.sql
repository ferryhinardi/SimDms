if object_id('uspfn_itsinqAchievementSalesman') is not null
	drop procedure uspfn_itsinqAchievementSalesman
GO
CREATE procedure [dbo].[uspfn_itsinqAchievementSalesman]
(  
 @CompanyCode  VARCHAR(15),  
 @BMEmployeeID  VARCHAR(15),  
 @SHEmployeeID  VARCHAR(15),  
 @SCEmployeeID  VARCHAR(15),  
 @SMEmployeeID  VARCHAR(15),  
 @Year    INT  
)  
AS  
BEGIN  

--declare @CompanyCode  VARCHAR(15)  
--SET @CompanyCode = '6006406'
--declare @BMEmployeeID  VARCHAR(15)
--SET @BMEmployeeID = 'BM189'  
--declare @SHEmployeeID  VARCHAR(15) 
--SET @SHEmployeeID = '225' 
--declare @SCEmployeeID  VARCHAR(15)
--SET @SCEmployeeID = '50395'  
--declare @SMEmployeeID  VARCHAR(15)
--SET @SMEmployeeID ='%'
--DECLARE  @Year    INT  
--SET @Year = 2013

 select * into #ListOfSalesman from (  
  select tmSalesman.CompanyCode, tmSalesman.BranchCode  
   , tmSalesman.EmployeeID, tmSalesman.TeamID   
   , tmSalesman.IsSupervisor    
  from   
   PmMstTeamMembers tmSalesman,  
   (  
    -- Sales Coordinator ---  
    select tmSalesmanTemp.CompanyCode, tmSalesmanTemp.BranchCode  
     , tmSalesmanTemp.EmployeeID, tmSalesmanTemp.TeamID   
     , tmSalesmanTemp.IsSupervisor   
    from   
     PmMstTeamMembers tmSalesmanTemp,  
     (  
      -- Team Members of Sales Head ----------  
      select tmSC.CompanyCode, tmSC.BranchCode  
       , tmSC.EmployeeID, tmSC.TeamID , tmSC.IsSupervisor   
      from   
       PmMstTeamMembers tmSC,   
       (  
        -- Sales Head ----------  
        select tmSH.CompanyCode, tmSH.BranchCode  
         , tmSH.EmployeeID, tmSH.TeamID , tmSH.IsSupervisor  
        from   
         PmMstTeamMembers tmSH,   
         (  
          -- Team Members of Branch Manager -----  
          select tmSHTemp.CompanyCode, tmSHTemp.BranchCode  
           , tmSHTemp.EmployeeID, tmSHTemp.TeamID, tmBM.IsSupervisor  
          from   
           PmMstTeamMembers tmSHTemp, (  
            -- Branch Manager --  
            select tmBM.CompanyCode, tmBM.BranchCode, tmBM.IsSupervisor  
             ,tmBM.EmployeeID, tmBM.TeamID  
            from PmMstTeamMembers tmBM  
             inner join PmPosition posBM on tmBM.CompanyCode = posBM.CompanyCode  
              and tmBM.EmployeeID = posBM.EmployeeID and posBM.PositionID = 40   
            where tmBM.CompanyCode = @CompanyCode and tmBM.EmployeeID like @BMEmployeeID -- employeeID of Branch Manager  
            --------------------  
           ) tmBM  
          where   
           tmBM.CompanyCode = tmSHTemp.CompanyCode   
           and tmBM.BranchCode = tmSHTemp.BranchCode  
           and tmBM.TeamID = tmSHTemp.TeamID   
           and tmSHTemp.IsSupervisor != 1   
           and tmSHTemp.EmployeeID like @SHEmployeeID -- employeeID of SalesHead  
          ----------------------------------------  
         ) tmSHTemp_NotSup  
        where tmSH.CompanyCode = tmSHTemp_NotSup.CompanyCode   
         and tmSH.BranchCode = tmSHTemp_NotSup.BranchCode  
         and tmSH.EmployeeID = tmSHTemp_NotSup.EmployeeID  
         and tmSH.IsSupervisor = 1  
        ---------------------------------  
       ) tmSCTemp  
      where  
       tmSC.CompanyCode = tmSCTemp.CompanyCode   
       and tmSC.BranchCode = tmSCTemp.BranchCode  
       and tmSC.TeamID = tmSCTemp.TeamID  
       and tmSC.IsSupervisor != 1  
       and tmSC.EmployeeID like @SCEmployeeID -- EmployeeID of Sales Coordinator  
      --------------------------------  
     )tmSCTemp_NotSup  
    where tmSalesmanTemp.CompanyCode = tmSCTemp_NotSup.CompanyCode   
     and tmSalesmanTemp.BranchCode = tmSCTemp_NotSup.BranchCode  
     and tmSalesmanTemp.EmployeeID = tmSCTemp_NotSup.EmployeeID  
     and tmSalesmanTemp.IsSupervisor = 1  
    --------------  
   ) tmSalesmanTemp  
  where  
   tmSalesman.CompanyCode = tmSalesmanTemp.CompanyCode   
   and tmSalesman.BranchCode = tmSalesmanTemp.BranchCode  
   and tmSalesman.TeamID = tmSalesmanTemp.TeamID  
   and tmSalesman.IsSupervisor != 1  
   and tmSalesman.EmployeeID like @SMEmployeeID -- EmployeeID of Salesman  
 ) #ListOfSalesman 

select * into #TempSM from (  
  select 'SM' Intial, CompanyCode, BranchCode, SpvEmployeeID, EmployeeID, isnull(Jan, 0) Jan  
  , isnull(Feb, 0) Feb, isnull(Mar, 0) Mar, isnull(Apr, 0) Apr, isnull(May, 0) May  
  , isnull(Jun, 0) Jun, isnull(Jul, 0) Jul, isnull(Aug, 0) Aug, isnull(Sep, 0) Sep  
  , isnull(Oct, 0) Oct, isnull(Nov, 0) Nov, isnull(Dec, 0) Dec from (  
   select kdp.CompanyCode, kdp.BranchCode, substring(Convert(varchar, kdp.InquiryDate, 109), 1, 3) InquiryMonth --[Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, Dec]  
    , kdp.SpvEmployeeID, kdp.EmployeeID, count(kdp.EmployeeID) InquiryCount  
   from PmKDP kdp  
   where kdp.CompanyCode = @CompanyCode and year(kdp.InquiryDate) = @Year  
    and kdp.BranchCode in (select BranchCode from #ListOfSalesman)        
    and kdp.EmployeeID in (select EmployeeID from #ListOfSalesman)        
   group by kdp.CompanyCode, kdp.BranchCode, kdp.SpvEmployeeID, kdp.EmployeeID, substring(Convert(varchar, kdp.InquiryDate, 109), 1, 3)  
  ) as Header  
  pivot(  
   sum(InquiryCount)  
   for InquiryMonth in (Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, Dec)  
  ) pvt  
 )#TempSM   
  
 select * into #TempSC from (  
  select   
  'SC' Initial, SM.CompanyCode, SM.BranchCode, SM.SpvEmployeeID EmployeeID  
  , sum(SM.Jan) Jan, sum(SM.Feb) Feb, sum(SM.Mar) Mar  
  , sum(SM.Apr) Apr, sum(SM.May) May, sum(SM.Jun) Jun  
  , sum(SM.Jul) Jul, sum(SM.Aug) Aug, sum(SM.Sep) Sep  
  , sum(SM.Oct) Oct, sum(SM.Nov) Nov, sum(SM.Dec) Dec  
  from #TempSM SM   
  group by SM.CompanyCode, SM.BranchCode, SM.SpvEmployeeID  
 ) #TempSC  
  
 select * into #TempSH from (  
  select   
   'SH' Initial, tempSH.CompanyCode, tempSH.BranchCode, tempSH.SpvEmployeeID EmployeeID  
   , sum(tempSC.Jan) Jan, sum(tempSC.Feb) Feb, sum(tempSC.Mar) Mar  
   , sum(tempSC.Apr) Apr, sum(tempSC.May) May, sum(tempSC.Jun) Jun  
   , sum(tempSC.Jul) Jul, sum(tempSC.Aug) Aug, sum(tempSC.Sep) Sep  
   , sum(tempSC.Oct) Oct, sum(tempSC.Nov) Nov, sum(tempSC.Dec) Dec   
  from #TempSC tempSC,   
   (select tMembersHead.CompanyCode, tMembersHead.BranchCode  
    , tMembersHead.EmployeeID SpvEmployeeID, tMembersCoord.EmployeeID  
   from PmMstTeamMembers tMembersHead,  
    (select tMembers.CompanyCode, tMembers.BranchCode  
     , tMembers.EmployeeID, tMembers.TeamID  
    from PmMstTeamMembers tMembers, #TempSC sHeads  
    where   
     tMembers.CompanyCode = sHeads.CompanyCode   
     and tMembers.BranchCode = sHeads.BranchCode  
     and tMembers.EmployeeID = sHeads.EmployeeID   
     and tMembers.IsSupervisor != 1) tMembersCoord   
   where tMembersHead.CompanyCode = tMembersCoord.CompanyCode  
    and tMembersHead.BranchCode = tMembersCoord.BranchCode  
    and tMembershead.TeamID = tMembersCoord.TeamID  
    and tMembersHead.IsSupervisor = 1) tempSH  
  where tempSH.CompanyCode = tempSC.CompanyCode  
   and tempSH.BranchCode = tempSC.BranchCode  
   and tempSH.EmployeeID = tempSC.EmployeeID  
  group by tempSH.CompanyCode, tempSH.BranchCode, tempSH.SpvEmployeeID  
 ) #TempSH  
  
 select * into #TempBM from (  
  select   
   'SH' Initial, tempBM.CompanyCode, tempBM.BranchCode, tempBM.SpvEmployeeID EmployeeID  
   , sum(tempSH.Jan) Jan, sum(tempSH.Feb) Feb, sum(tempSH.Mar) Mar  
   , sum(tempSH.Apr) Apr, sum(tempSH.May) May, sum(tempSH.Jun) Jun  
   , sum(tempSH.Jul) Jul, sum(tempSH.Aug) Aug, sum(tempSH.Sep) Sep  
   , sum(tempSH.Oct) Oct, sum(tempSH.Nov) Nov, sum(tempSH.Dec) Dec   
  from #TempSH tempSH,   
   (select tMembersHead.CompanyCode, tMembersHead.BranchCode  
    , tMembersHead.EmployeeID SpvEmployeeID, tMembersCoord.EmployeeID  
   from PmMstTeamMembers tMembersHead,  
    (select tMembers.CompanyCode, tMembers.BranchCode  
     , tMembers.EmployeeID, tMembers.TeamID  
    from PmMstTeamMembers tMembers, #TempSH sHeads  
    where   
     tMembers.CompanyCode = sHeads.CompanyCode   
     and tMembers.BranchCode = sHeads.BranchCode  
     and tMembers.EmployeeID = sHeads.EmployeeID   
     and tMembers.IsSupervisor != 1) tMembersCoord   
   where tMembersHead.CompanyCode = tMembersCoord.CompanyCode  
    and tMembersHead.BranchCode = tMembersCoord.BranchCode  
    and tMembershead.TeamID = tMembersCoord.TeamID  
    and tMembersHead.IsSupervisor = 1) tempBM  
  where tempBM.CompanyCode = tempSH.CompanyCode  
   and tempBM.BranchCode = tempSH.BranchCode  
   and tempBM.EmployeeID = tempSH.EmployeeID  
  group by tempBM.CompanyCode, tempBM.BranchCode, tempBM.SpvEmployeeID  
 ) #TempBM 
 
 select   
  inc, initial, CompanyCode, BranchCode, EmployeeID, EmployeeName  
  , case cast(Jan as varchar) when '0' then '-' else cast(Jan as varchar) end Jan  
  , case cast(Feb as varchar) when '0' then '-' else cast(Feb as varchar) end Feb  
  , case cast(Mar as varchar) when '0' then '-' else cast(Mar as varchar) end Mar  
  , case cast(Apr as varchar) when '0' then '-' else cast(Apr as varchar) end Apr  
  , case cast(May as varchar) when '0' then '-' else cast(May as varchar) end May  
  , case cast(Jun as varchar) when '0' then '-' else cast(Jun as varchar) end Jun  
  , case cast(Jul as varchar) when '0' then '-' else cast(Jul as varchar) end Jul  
  , case cast(Aug as varchar) when '0' then '-' else cast(Aug as varchar) end Aug  
  , case cast(Sep as varchar) when '0' then '-' else cast(Sep as varchar) end Sep  
  , case cast(Oct as varchar) when '0' then '-' else cast(Oct as varchar) end Oct  
  , case cast(Nov as varchar) when '0' then '-' else cast(Nov as varchar) end Nov  
  , case cast(Dec as varchar) when '0' then '-' else cast(Dec as varchar) end Dec  
  , case cast(Sem1 as varchar) when '0' then '-' else cast(Sem1 as varchar) end Sem1  
  , case cast(Sem2 as varchar) when '0' then '-' else cast(Sem2 as varchar) end Sem2  
  , case cast(Total as varchar) when '0' then '-' else cast(Total as varchar) end Total  
 from (  
  select    
   '1' inc, 'Salesman' initial, tempSM.CompanyCode, tempSM.BranchCode, tempSM.EmployeeID  
   , emp.EmployeeName, tempSM.Jan, tempSM.Feb, tempSM.Mar, tempSM.Apr, tempSM.May, tempSM.Jun  
   , tempSM.Jul, tempSM.Aug, tempSM.Sep, tempSM.Oct, tempSM.Nov, tempSM.Dec  
   , (tempSM.Jan + tempSM.Feb + tempSM.Mar + tempSM.Apr + tempSM.May + tempSM.Jun) Sem1  
   , (tempSM.Jul + tempSM.Aug + tempSM.Sep + tempSM.Oct + tempSM.Nov+ tempSM.Dec) Sem2  
   , (tempSM.Jan + tempSM.Feb + tempSM.Mar + tempSM.Apr + tempSM.May + tempSM.Jun  
    + tempSM.Jul + tempSM.Aug + tempSM.Sep + tempSM.Oct + tempSM.Nov  
    + tempSM.Dec) Total  
  from   
   #TempSM tempSM  
    left join HrEmployee emp on tempSM.CompanyCode = emp.CompanyCode  
     and tempSM.EmployeeID = emp.EmployeeID  
    
  union  
  -- Sales Coordinator -------   
  select    
   '2' inc, 'Sales Coordinator' initial, TempSC.CompanyCode, TempSC.BranchCode, TempSC.EmployeeID, emp.EmployeeName  
   , TempSC.Jan, TempSC.Feb, TempSC.Mar, TempSC.Apr, TempSC.May, TempSC.Jun  
   , TempSC.Jul, TempSC.Aug, TempSC.Sep, TempSC.Oct, TempSC.Nov, TempSC.Dec  
   , (TempSC.Jan + TempSC.Feb + TempSC.Mar + TempSC.Apr + TempSC.May + TempSC.Jun) Sem1  
   , (TempSC.Jul + TempSC.Aug + TempSC.Sep + TempSC.Oct + TempSC.Nov+ TempSC.Dec) Sem2  
   , (TempSC.Jan + TempSC.Feb + TempSC.Mar + TempSC.Apr + TempSC.May + TempSC.Jun  
    + TempSC.Jul + TempSC.Aug + TempSC.Sep + TempSC.Oct + TempSC.Nov  
    + TempSC.Dec) Total  
  from   
   #TempSC TempSC  
    left join HrEmployee emp on TempSC.CompanyCode = emp.CompanyCode  
     and TempSC.EmployeeID = emp.EmployeeID  
    
  union  
  -- Sales Head -------   
  select    
   '3' inc, 'Sales Head' initial, TempSH.CompanyCode, TempSH.BranchCode, TempSH.EmployeeID, emp.EmployeeName  
   , TempSH.Jan, TempSH.Feb, TempSH.Mar, TempSH.Apr, TempSH.May, TempSH.Jun  
   , TempSH.Jul, TempSH.Aug, TempSH.Sep, TempSH.Oct, TempSH.Nov, TempSH.Dec  
   , (TempSH.Jan + TempSH.Feb + TempSH.Mar + TempSH.Apr + TempSH.May + TempSH.Jun) Sem1  
   , (TempSH.Jul + TempSH.Aug + TempSH.Sep + TempSH.Oct + TempSH.Nov+ TempSH.Dec) Sem2  
   , (TempSH.Jan + TempSH.Feb + TempSH.Mar + TempSH.Apr + TempSH.May + TempSH.Jun  
    + TempSH.Jul + TempSH.Aug + TempSH.Sep + TempSH.Oct + TempSH.Nov  
    + TempSH.Dec) Total  
  from   
   #TempSH TempSH  
    left join HrEmployee emp on TempSH.CompanyCode = emp.CompanyCode  
     and TempSH.EmployeeID = emp.EmployeeID  
    
  union  
  -- Branch Manager -------  
  select    
   '4' inc, 'Branch Manager' initial, TempBM.CompanyCode, TempBM.BranchCode, TempBM.EmployeeID, emp.EmployeeName  
   , TempBM.Jan, TempBM.Feb, TempBM.Mar, TempBM.Apr, TempBM.May, TempBM.Jun  
   , TempBM.Jul, TempBM.Aug, TempBM.Sep, TempBM.Oct, TempBM.Nov, TempBM.Dec  
   , (TempBM.Jan + TempBM.Feb + TempBM.Mar + TempBM.Apr + TempBM.May + TempBM.Jun) Sem1  
   , (TempBM.Jul + TempBM.Aug + TempBM.Sep + TempBM.Oct + TempBM.Nov+ TempBM.Dec) Sem2  
   , (TempBM.Jan + TempBM.Feb + TempBM.Mar + TempBM.Apr + TempBM.May + TempBM.Jun  
    + TempBM.Jul + TempBM.Aug + TempBM.Sep + TempBM.Oct + TempBM.Nov  
    + TempBM.Dec) Total  
  from   
   #TempBM TempBM  
    left join HrEmployee emp on TempBM.CompanyCode = emp.CompanyCode  
     and TempBM.EmployeeID = emp.EmployeeID  
 ) SASalesman order by SASalesman.inc--, SASalesman.EmployeeName 
 
 DROP TABLE #ListOfSalesman
 DROP TABLE #TempBM
 DROP TABLE #TempSH
 DROP TABLE #TempSM
 DROP TABLE #TempSC
 
 end
 GO
 