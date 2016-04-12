if object_id('usprpt_ItsInqAchievement') is not null
	drop procedure usprpt_ItsInqAchievement
GO

CREATE procedure [dbo].[usprpt_ItsInqAchievement]     
(    
 @CompanyCode  VARCHAR(15),    
 @BMEmployeeID  VARCHAR(15),    
 @SHEmployeeID  VARCHAR(15),    
 @SCEmployeeID  VARCHAR(15),    
 @SMEmployeeID  VARCHAR(15),    
 @Year    INT    
)    
AS    
  
--declare @companycode  varchar(15)    
--set @companycode = '6006406'  
--declare @bmemployeeid  varchar(15)  
--set @bmemployeeid = 'bm189'    
--declare @shemployeeid  varchar(15)   
--set @shemployeeid = '225'   
--declare @scemployeeid  varchar(15)  
--set @scemployeeid = '50395'    
--declare @smemployeeid  varchar(15)  
--set @smemployeeid ='%'  
--declare  @year    int    
--set @year = 2013  
  
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
   
   
-- ===========================================================    
-- ====== Searching for KDP based on parameters (List of Salesman) and PmMstTeamMembers,     
-- ====== Then Transpose to Monthly View ==========================    
-- ===========================================================     
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
    
--=======================================================================================    
-- SALES SOURCE OF DATA    
--=======================================================================================    
 select * into #SSD from (    
  select CompanyCode, '' BranchCode, TypeOf1, TypeOf2    
   , isnull(Jan, 0) Jan, isnull(Feb, 0) Feb, isnull(Mar, 0) Mar, isnull(Apr, 0) Apr, isnull(May, 0) May, isnull(Jun, 0) Jun    
   , isnull(Jul, 0) Jul, isnull(Aug, 0) Aug, isnull(Sep, 0) Sep, isnull(Oct, 0) Oct, isnull(Nov, 0) Nov, isnull(Dec, 0) Dec    
  from (    
   select kdp.CompanyCode, '' BranchCode, kdp.PerolehanData TypeOf1, '' TypeOf2    
    , substring(Convert(varchar, kdp.InquiryDate, 109), 1, 3) InquiryMonth, count(kdp.EmployeeID) InquiryCount    
   from PmKDP kdp    
   where kdp.CompanyCode = @CompanyCode and year(kdp.InquiryDate) = @Year    
    and kdp.BranchCode in (select BranchCode from #ListOfSalesman)          
    and kdp.EmployeeID in (select EmployeeID from #ListOfSalesman)    
   group by kdp.CompanyCode, kdp.PerolehanData, substring(Convert(varchar, kdp.InquiryDate, 109), 1, 3)) as Header    
  pivot    
  (    
   sum (inquiryCount)    
   for InquiryMonth in (Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, Dec)    
  ) pvt    
 ) #SSD    
    
--=======================================================================================    
-- SALES BY TYPE    
--=======================================================================================    
 select * into #ST from (    
  select CompanyCode, '' BranchCode, TypeOf1, TypeOf2    
   , isnull(Jan, 0) Jan, isnull(Feb, 0) Feb, isnull(Mar, 0) Mar, isnull(Apr, 0) Apr, isnull(May, 0) May, isnull(Jun, 0) Jun    
   , isnull(Jul, 0) Jul, isnull(Aug, 0) Aug, isnull(Sep, 0) Sep, isnull(Oct, 0) Oct, isnull(Nov, 0) Nov, isnull(Dec, 0) Dec    
  from (    
   select kdp.CompanyCode, '' BranchCode, kdp.TipeKendaraan TypeOf1, kdp.Variant TypeOf2    
    , substring(Convert(varchar, kdp.InquiryDate, 109), 1, 3) InquiryMonth    
    , count(kdp.EmployeeID) InquiryCount    
   from PmKDP kdp    
   where kdp.CompanyCode = @CompanyCode and year(kdp.InquiryDate) = @Year    
    and kdp.BranchCode in (select BranchCode from #ListOfSalesman)          
    and kdp.EmployeeID in (select EmployeeID from #ListOfSalesman)         
   group by kdp.CompanyCode, kdp.TipeKendaraan, kdp.Variant     
    , substring(Convert(varchar, kdp.InquiryDate, 109), 1, 3)) as Header    
  pivot    
  (    
   sum(InquiryCount)    
   for InquiryMonth in (Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, Dec)    
  ) pvt    
 ) #ST    
    
--=======================================================================================    
-- PROSPECT STATUS    
--=======================================================================================    
 select * into #PS from (    
  select CompanyCode, '' BranchCode, TypeOf1, TypeOf2    
   , isnull(Jan, 0) Jan, isnull(Feb, 0) Feb, isnull(Mar, 0) Mar, isnull(Apr, 0) Apr, isnull(May, 0) May, isnull(Jun, 0) Jun    
   , isnull(Jul, 0) Jul, isnull(Aug, 0) Aug, isnull(Sep, 0) Sep, isnull(Oct, 0) Oct, isnull(Nov, 0) Nov, isnull(Dec, 0) Dec     
  from (    
   select kdp.CompanyCode, '' BranchCode, kdp.LastProgress TypeOf1, '' TypeOf2    
    , substring(Convert(varchar, kdp.InquiryDate, 109), 1, 3) InquiryMonth    
    , count(kdp.EmployeeID) InquiryCount    
   from PmKDP kdp    
   where kdp.CompanyCode = @CompanyCode and year(kdp.InquiryDate) = @Year     
    and kdp.BranchCode in (select BranchCode from #ListOfSalesman)          
    and kdp.EmployeeID in (select EmployeeID from #ListOfSalesman)          
   group by kdp.CompanyCode, kdp.BranchCode, kdp.LastProgress    
    , substring(Convert(varchar, kdp.InquiryDate, 109), 1, 3)) as Header    
  pivot    
  (    
   sum(InquiryCount)    
   for InquiryMonth in (Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, Dec)    
  ) pvt    
 ) #PS    
   
 -- Salesman ----------    
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
   
 select      
    '1' inc, 'Sales Source of Data' initial, tempSSD.CompanyCode, '' BranchCode, lkpDtl.LookUpValue TypeOf1, tempSSD.TypeOf2    
   , isnull(tempSSD.Jan, 0) Jan, isnull(tempSSD.Feb, 0) Feb, isnull(tempSSD.Mar, 0) Mar    
   , isnull(tempSSD.Apr, 0) Apr, isnull(tempSSD.May, 0) May, isnull(tempSSD.Jun, 0) Jun    
   , isnull(tempSSD.Jul, 0) Jul, isnull(tempSSD.Aug, 0) Aug, isnull(tempSSD.Sep, 0) Sep    
   , isnull(tempSSD.Oct, 0) Oct, isnull(tempSSD.Nov, 0) Nov, isnull(tempSSD.Dec, 0) Dec    
   , isnull((tempSSD.Jan + tempSSD.Feb + tempSSD.Mar + tempSSD.Apr + tempSSD.May + tempSSD.Jun), 0) Sem1    
   , isnull((tempSSD.Jul + tempSSD.Aug + tempSSD.Sep + tempSSD.Oct + tempSSD.Nov+ tempSSD.Dec), 0) Sem2    
   , isnull((tempSSD.Jan + tempSSD.Feb + tempSSD.Mar + tempSSD.Apr + tempSSD.May + tempSSD.Jun    
    + tempSSD.Jul + tempSSD.Aug + tempSSD.Sep + tempSSD.Oct + tempSSD.Nov    
    + tempSSD.Dec), 0) Total    
  from     
   GnMstLookUpDtl lkpDtl    
    left join #SSD tempSSD on lkpDtl.CompanyCode = tempSSD.CompanyCode    
    and lkpDtl.LookUpValue = tempSSD.TypeOf1     
  where     
   lkpDtl.CodeID = 'PSRC' and lkpDtl.CompanyCode = @CompanyCode   
     
   select      
   '2' inc, 'Sales by Type' initial, tempST.CompanyCode, tempST.BranchCode, GTS.GroupCode TypeOf1, GTS.typeCode TypeOf2-- tempST.TypeOf1, tempST.TypeOf2    
   , isnull(tempST.Jan, 0) Jan, isnull(tempST.Feb, 0) Feb, isnull(tempST.Mar, 0) Mar    
   , isnull(tempST.Apr, 0) Apr, isnull(tempST.May, 0) May, isnull(tempST.Jun, 0) Jun    
   , isnull(tempST.Jul, 0) Jul, isnull(tempST.Aug, 0) Aug, isnull(tempST.Sep, 0) Sep    
   , isnull(tempST.Oct, 0) Oct, isnull(tempST.Nov, 0) Nov, isnull(tempST.Dec, 0) Dec    
   , isnull((tempST.Jan + tempST.Feb + tempST.Mar + tempST.Apr + tempST.May + tempST.Jun), 0) Sem1    
   , isnull((tempST.Jul + tempST.Aug + tempST.Sep + tempST.Oct + tempST.Nov+ tempST.Dec), 0) Sem2    
   , isnull((tempST.Jan + tempST.Feb + tempST.Mar + tempST.Apr + tempST.May + tempST.Jun    
    + tempST.Jul + tempST.Aug + tempST.Sep + tempST.Oct + tempST.Nov    
    + tempST.Dec), 0) Total    
  from     
   (select Distinct CompanyCode, GroupCode, TypeCode      
   from pmGroupTypeSeq     
   group by CompanyCode, GroupCode ,typeCode) GTS    
    left join #ST tempST on GTS.CompanyCode = tempST.CompanyCode and GTS.GroupCode = tempST.TypeOf1 and GTS.TypeCode = tempST.TypeOf2    
         
  select     
   '3' inc, 'Prospect Status' initial, tempPS.CompanyCode, tempPS.BranchCode, lkpDtl.LookUpValueName TypeOf1, tempPS.TypeOf2    
   , isnull(tempPS.Jan, 0) Jan, isnull(tempPS.Feb, 0) Feb, isnull(tempPS.Mar, 0) Mar    
   , isnull(tempPS.Apr, 0) Apr, isnull(tempPS.May, 0) May, isnull(tempPS.Jun, 0) Jun    
   , isnull(tempPS.Jul, 0) Jul, isnull(tempPS.Aug, 0) Aug, isnull(tempPS.Sep, 0) Sep    
   , isnull(tempPS.Oct, 0) Oct, isnull(tempPS.Nov, 0) Nov, isnull(tempPS.Dec, 0) Dec    
   , isnull((tempPS.Jan + tempPS.Feb + tempPS.Mar + tempPS.Apr + tempPS.May + tempPS.Jun), 0) Sem1    
   , isnull((tempPS.Jul + tempPS.Aug + tempPS.Sep + tempPS.Oct + tempPS.Nov+ tempPS.Dec), 0) Sem2    
   , isnull((tempPS.Jan + tempPS.Feb + tempPS.Mar + tempPS.Apr + tempPS.May + tempPS.Jun    
    + tempPS.Jul + tempPS.Aug + tempPS.Sep + tempPS.Oct + tempPS.Nov    
    + tempPS.Dec), 0) Total    
  from    
   GnMstLookUpDtl lkpDtl    
    left join #PS tempPS on lkpDtl.CompanyCode = tempPS.CompanyCode    
    and lkpDtl.LookUpValue = tempPS.TypeOf1     
  where lkpDtl.CodeID = 'PSTS' and lkpDtl.CompanyCode = @CompanyCode   
  
 drop table #TempSM    
 drop table #TempSC    
 drop table #TempSH    
 drop table #TempBM    
 drop table #SSD    
 drop table #ST    
 drop table #PS    
    
 drop table #ListOfSalesman     
GO

if object_id('usprpt_InquiryITSProd') is not null
	drop procedure usprpt_InquiryITSProd
GO
-- =============================================
-- Author:		<yo>
-- Create date: <30 Oct 2013>
-- Last Update date: <01 Apr 2014> 
-- Description:	<Inquiry ITS Productivity>
-- =============================================

CREATE PROCEDURE [dbo].[usprpt_InquiryITSProd]
  @StartDate			DATETIME,
  @EndDate				DATETIME,
  @Area					varchar(100),
  @DealerCode			varchar(100),
  @OutletCode			varchar(100),
  @BranchHead			varchar(100),
  @SalesHead			varchar(100),
  @SalesCoordinator		varchar(100),
  @Salesman				varchar(100),
  @TypeReport			varchar(1),
  @ProductivityBy		varchar(1)
AS
BEGIN

--usprpt_InquiryITSProd '01-Nov-2013','30-Nov-2013','JAWA BARAT','6058401','605840100','','','','','1','1'
--usprpt_InquiryITSProd '01-Oct-2014','09-Oct-2014','CABANG','6006400001','','','','','','0','0'
--declare @StartDate	DATETIME
--declare @EndDate	DATETIME
--declare @Area		varchar(100)
--declare @DealerCode	varchar(100)
--declare @OutletCode	varchar(100)
--declare @BranchHead	varchar(100)
--declare @SalesHead	varchar(100)
--declare @SalesCoordinator varchar(100)
--declare @Salesman	varchar(100)
--declare @TypeReport	varchar(1)
--declare @ProductivityBy varchar(1)

--set @StartDate = '01-Mar-2014'
--set @EndDate	= '19-Mar-2014'
--set @Area = 'CABANG'
--set @DealerCode = '6006406'
--set @OutletCode = '6006404'
--set @BranchHead = ''
--set @SalesHead  = ''
--set @SalesCoordinator = ''
--set @Salesman = ''
--set @TypeReport = '1' -- 0 : SUMMARY, 1 : SALDO
--set @ProductivityBy = '0'	-- 0 : SALESMAN, 1 : VEHICLE TYPE, 2 : SOURCE DATA			

DECLARE @National AS VARCHAR(1)
SET @National = (SELECT ParaValue FROM SuzukiR4..GnMstLookUpDtl WHERE CodeID = 'QSLS' AND LookUpValue = 'NATIONAL')					

--#region PRODUCTIVITY BY SALESMAN (0)	
IF @ProductivityBy = '0'
BEGIN	
--#region SELECT SALESMAN 	
		
	IF @National = '1' 
	BEGIN
	SELECT * INTO #tSPK1 FROM(
		SELECT Hist.CompanyCode, Hist.BranchCode, ISNULL(Wiraniaga, '[BLANK]') Wiraniaga, 
			COUNT(Hist.LastProgress) SPK, ISNULL(SalesCoordinator, '[BLANK]') SalesCoordinator
			FROM SuzukiR4..pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
			LEFT JOIN SuzukiR4..pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON 
				HstITS.CompanyCode = Hist.CompanyCode AND
				HstITS.BranchCode = Hist.BranchCode AND
				HstITS.InquiryNumber = Hist.InquiryNumber 
			 WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE Hist.CompanyCode END) = @DealerCode
				AND (CASE WHEN @OutletCode = '' THEN '' ELSE Hist.BranchCode END) = @OutletCode	
				AND Hist.LastProgress = 'SPK' 
				AND HstITS.LastProgress = 'SPK' -- penambahan 1 April 2014
				AND (CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)				
				 or (convert(varchar,Hist.UpdateDate,112) < @StartDate 
				and (Hist.LastProgress='P'   and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
														   where h.CompanyCode=Hist.CompanyCode
															 and h.BranchCode=Hist.BranchCode
															 and h.InquiryNumber=Hist.InquiryNumber
															 and h.LastProgress<>'P'
															 and convert(varchar,h.UpdateDate,112)<@StartDate)
				 or  Hist.LastProgress='HP'  and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
														   where h.CompanyCode=Hist.CompanyCode
															 and h.BranchCode=Hist.BranchCode
															 and h.InquiryNumber=Hist.InquiryNumber
															 and h.LastProgress not in ('P','HP')
															 and convert(varchar,h.UpdateDate,112)<@StartDate)
				 or Hist.LastProgress='SPK' and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
														   where h.CompanyCode=Hist.CompanyCode
															 and h.BranchCode=Hist.BranchCode
															 and h.InquiryNumber=Hist.InquiryNumber
															 and h.LastProgress not in ('P','HP','SPK')
															 and convert(varchar,h.UpdateDate,112)<@StartDate))))	 
															 and HstITS.LastProgress not in ('DO','DELIVERY','LOST')	
			 AND CONVERT(VARCHAR, HstITS.LastUpdateStatus , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)																 
			 GROUP BY  Hist.CompanyCode, Hist.BranchCode, Wiraniaga, SalesCoordinator) #tSPK1

	SELECT * INTO #tSPK2 FROM(
		SELECT Hist.CompanyCode, Hist.BranchCode, ISNULL(Wiraniaga, '[BLANK]') Wiraniaga, 
			COUNT(Hist.LastProgress) SPK, ISNULL(SalesCoordinator, '[BLANK]') SalesCoordinator
			FROM SuzukiR4..pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
			LEFT JOIN SuzukiR4..pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON 
				HstITS.CompanyCode = Hist.CompanyCode AND
				HstITS.BranchCode = Hist.BranchCode AND
				HstITS.InquiryNumber = Hist.InquiryNumber 
			 WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE Hist.CompanyCode END) = @DealerCode
				AND (CASE WHEN @OutletCode = '' THEN '' ELSE Hist.BranchCode END) = @OutletCode	
				AND Hist.LastProgress = 'SPK'
				AND HstITS.LastProgress = 'SPK' -- penambahan 1 April 2014
				AND (CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)								
				 or (convert(varchar,Hist.UpdateDate,112) < @StartDate 
				 and (Hist.LastProgress='P'   and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
														   where h.CompanyCode=Hist.CompanyCode
															 and h.BranchCode=Hist.BranchCode
															 and h.InquiryNumber=Hist.InquiryNumber
															 and h.LastProgress<>'P'
															 and convert(varchar,h.UpdateDate,112)<@StartDate)
				 or  Hist.LastProgress='HP'  and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
														   where h.CompanyCode=Hist.CompanyCode
															 and h.BranchCode=Hist.BranchCode
															 and h.InquiryNumber=Hist.InquiryNumber
															 and h.LastProgress not in ('P','HP')
															 and convert(varchar,h.UpdateDate,112)<@StartDate)
				 or Hist.LastProgress='SPK' and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
														   where h.CompanyCode=Hist.CompanyCode
															 and h.BranchCode=Hist.BranchCode
															 and h.InquiryNumber=Hist.InquiryNumber
															 and h.LastProgress not in ('P','HP','SPK')
															 and convert(varchar,h.UpdateDate,112)<@StartDate)))
			  AND CONVERT(VARCHAR, HstITS.LastUpdateStatus, 112) <= CONVERT(VARCHAR, @EndDate, 112))
			   and HstITS.LastProgress not in ('DO','DELIVERY','LOST')	 
			 GROUP BY  Hist.CompanyCode, Hist.BranchCode, Wiraniaga, SalesCoordinator) #tSPK2
			 
	SELECT * INTO #tDO FROM (
		SELECT Hist.CompanyCode, Hist.BranchCode, Wiraniaga, COUNT(Hist.LastProgress) DO, SalesCoordinator
				FROM SuzukiR4..pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
				INNER JOIN SuzukiR4..pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON 
					HstITS.CompanyCode = Hist.CompanyCode AND
					HstITS.BranchCode = Hist.BranchCode AND
					HstITS.InquiryNumber = Hist.InquiryNumber 
				 WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE Hist.CompanyCode END) = @DealerCode
					AND (CASE WHEN @OutletCode = '' THEN '' ELSE Hist.BranchCode END) = @OutletCode	
					AND Hist.LastProgress = 'DO'
					AND HstITS.LastProgress = 'DO' -- penambahan 1 April 2014
					AND CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112) 					
				GROUP BY Hist.CompanyCode, Hist.BranchCode, Wiraniaga, SalesCoordinator) #tDO
		
	SELECT * INTO #tDELIVERY FROM (
			SELECT Hist.CompanyCode, Hist.BranchCode, Wiraniaga, COUNT(Hist.LastProgress) DELIVERY, SalesCoordinator
					FROM SuzukiR4..pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
					INNER JOIN SuzukiR4..pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON 
						HstITS.CompanyCode = Hist.CompanyCode AND
						HstITS.BranchCode = Hist.BranchCode AND
						HstITS.InquiryNumber = Hist.InquiryNumber 
					 WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE Hist.CompanyCode END) = @DealerCode
						AND (CASE WHEN @OutletCode = '' THEN '' ELSE Hist.BranchCode END) = @OutletCode	
						AND Hist.LastProgress = 'DELIVERY'
						AND HstITS.LastProgress = 'DELIVERY' -- penambahan 1 April 2014
						AND CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112) 
					GROUP BY Hist.CompanyCode, Hist.BranchCode, Wiraniaga, SalesCoordinator) #tDELIVERY

	SELECT * INTO #tHP1 FROM (
			SELECT Hist.CompanyCode, Hist.BranchCode, ISNULL(Wiraniaga, '[BLANK]') Wiraniaga, 
			COUNT(Hist.LastProgress) HP, ISNULL(SalesCoordinator, '[BLANK]') SalesCoordinator
			FROM SuzukiR4..pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
			LEFT JOIN SuzukiR4..pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON 
				HstITS.CompanyCode = Hist.CompanyCode AND
				HstITS.BranchCode = Hist.BranchCode AND
				HstITS.InquiryNumber = Hist.InquiryNumber 
			 WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE Hist.CompanyCode END) = @DealerCode
				AND (CASE WHEN @OutletCode = '' THEN '' ELSE Hist.BranchCode END) = @OutletCode	
				AND Hist.LastProgress = 'HP'
				AND HstITS.LastProgress = 'HP' -- penambahan 1 April 2014
				AND (CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)				
				 or (convert(varchar,Hist.UpdateDate,112) < @StartDate 
				and (Hist.LastProgress='P'   and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
														   where h.CompanyCode=Hist.CompanyCode
															 and h.BranchCode=Hist.BranchCode
															 and h.InquiryNumber=Hist.InquiryNumber
															 and h.LastProgress<>'P'
															 and convert(varchar,h.UpdateDate,112)<@StartDate)
				 or  Hist.LastProgress='HP'  and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
														   where h.CompanyCode=Hist.CompanyCode
															 and h.BranchCode=Hist.BranchCode
															 and h.InquiryNumber=Hist.InquiryNumber
															 and h.LastProgress not in ('P','HP')
															 and convert(varchar,h.UpdateDate,112)<@StartDate)
				 or Hist.LastProgress='SPK' and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
														   where h.CompanyCode=Hist.CompanyCode
															 and h.BranchCode=Hist.BranchCode
															 and h.InquiryNumber=Hist.InquiryNumber
															 and h.LastProgress not in ('P','HP','SPK')
															 and convert(varchar,h.UpdateDate,112)<@StartDate))))	 
															  and HstITS.LastProgress not in ('DO','DELIVERY','LOST')	
			 AND CONVERT(VARCHAR, HstITS.LastUpdateStatus , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)	
			 GROUP BY  Hist.CompanyCode, Hist.BranchCode, Wiraniaga, SalesCoordinator) #tHP		

	SELECT * INTO #tHP2 FROM (
					SELECT Hist.CompanyCode, Hist.BranchCode, ISNULL(Wiraniaga, '[BLANK]') Wiraniaga, 
			COUNT(Hist.LastProgress) HP, ISNULL(SalesCoordinator, '[BLANK]') SalesCoordinator
			FROM SuzukiR4..pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
			LEFT JOIN SuzukiR4..pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON 
				HstITS.CompanyCode = Hist.CompanyCode AND
				HstITS.BranchCode = Hist.BranchCode AND
				HstITS.InquiryNumber = Hist.InquiryNumber 
			 WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE Hist.CompanyCode END) = @DealerCode
				AND (CASE WHEN @OutletCode = '' THEN '' ELSE Hist.BranchCode END) = @OutletCode	
				AND Hist.LastProgress = 'HP'
				AND HstITS.LastProgress = 'HP' -- penambahan 1 April 2014
				AND (CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)				
				 or (convert(varchar,Hist.UpdateDate,112) < @StartDate 
				and (Hist.LastProgress='P'   and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
														   where h.CompanyCode=Hist.CompanyCode
															 and h.BranchCode=Hist.BranchCode
															 and h.InquiryNumber=Hist.InquiryNumber
															 and h.LastProgress<>'P'
															 and convert(varchar,h.UpdateDate,112)<@StartDate)
				 or  Hist.LastProgress='HP'  and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
														   where h.CompanyCode=Hist.CompanyCode
															 and h.BranchCode=Hist.BranchCode
															 and h.InquiryNumber=Hist.InquiryNumber
															 and h.LastProgress not in ('P','HP')
															 and convert(varchar,h.UpdateDate,112)<@StartDate)
				 or Hist.LastProgress='SPK' and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
														   where h.CompanyCode=Hist.CompanyCode
															 and h.BranchCode=Hist.BranchCode
															 and h.InquiryNumber=Hist.InquiryNumber
															 and h.LastProgress not in ('P','HP','SPK')
															 and convert(varchar,h.UpdateDate,112)<@StartDate)))
				AND CONVERT(VARCHAR, HstITS.LastUpdateStatus, 112) <= CONVERT(VARCHAR, @EndDate, 112))				
				and HstITS.LastProgress not in ('DO','DELIVERY','LOST')											 	 
			 GROUP BY  Hist.CompanyCode, Hist.BranchCode, Wiraniaga, SalesCoordinator) #tHP2
		
	SELECT * INTO #tP1 FROM (
			SELECT Hist.CompanyCode, Hist.BranchCode, ISNULL(Wiraniaga, '[BLANK]') Wiraniaga, 
			COUNT(Hist.LastProgress) P, ISNULL(SalesCoordinator, '[BLANK]') SalesCoordinator
			FROM SuzukiR4..pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
			LEFT JOIN SuzukiR4..pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON 
				HstITS.CompanyCode = Hist.CompanyCode AND
				HstITS.BranchCode = Hist.BranchCode AND
				HstITS.InquiryNumber = Hist.InquiryNumber 
			 WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE Hist.CompanyCode END) = @DealerCode
				AND (CASE WHEN @OutletCode = '' THEN '' ELSE Hist.BranchCode END) = @OutletCode	
				AND Hist.LastProgress = 'P'
				AND HstITS.LastProgress = 'P' -- penambahan 1 April 2014
				AND (CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)				
				 or (convert(varchar,Hist.UpdateDate,112) < @StartDate 
				and (Hist.LastProgress='P'   and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
														   where h.CompanyCode=Hist.CompanyCode
															 and h.BranchCode=Hist.BranchCode
															 and h.InquiryNumber=Hist.InquiryNumber
															 and h.LastProgress<>'P'
															 and convert(varchar,h.UpdateDate,112)<@StartDate)
				 or  Hist.LastProgress='HP'  and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
														   where h.CompanyCode=Hist.CompanyCode
															 and h.BranchCode=Hist.BranchCode
															 and h.InquiryNumber=Hist.InquiryNumber
															 and h.LastProgress not in ('P','HP')
															 and convert(varchar,h.UpdateDate,112)<@StartDate)
				 or Hist.LastProgress='SPK' and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
														   where h.CompanyCode=Hist.CompanyCode
															 and h.BranchCode=Hist.BranchCode
															 and h.InquiryNumber=Hist.InquiryNumber
															 and h.LastProgress not in ('P','HP','SPK')
															 and convert(varchar,h.UpdateDate,112)<@StartDate))))
															 and HstITS.LastProgress not in ('DO','DELIVERY','LOST')		
															  AND CONVERT(VARCHAR, HstITS.LastUpdateStatus , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)	 
			 GROUP BY  Hist.CompanyCode, Hist.BranchCode, Wiraniaga, SalesCoordinator) #tP	
	
	SELECT * INTO #tP2 FROM (
		SELECT Hist.CompanyCode, Hist.BranchCode, ISNULL(Wiraniaga, '[BLANK]') Wiraniaga, 
			COUNT(Hist.LastProgress) P, ISNULL(SalesCoordinator, '[BLANK]') SalesCoordinator
			FROM SuzukiR4..pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
			LEFT JOIN SuzukiR4..pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON 
				HstITS.CompanyCode = Hist.CompanyCode AND
				HstITS.BranchCode = Hist.BranchCode AND
				HstITS.InquiryNumber = Hist.InquiryNumber 
			 WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE Hist.CompanyCode END) = @DealerCode
				AND (CASE WHEN @OutletCode = '' THEN '' ELSE Hist.BranchCode END) = @OutletCode	
				AND Hist.LastProgress = 'P'
				AND HstITS.LastProgress = 'P' -- penambahan 1 April 2014
				AND (CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)				
				 or (convert(varchar,Hist.UpdateDate,112) < @StartDate 
				and (Hist.LastProgress='P'   and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
														   where h.CompanyCode=Hist.CompanyCode
															 and h.BranchCode=Hist.BranchCode
															 and h.InquiryNumber=Hist.InquiryNumber
															 and h.LastProgress<>'P'
															 and convert(varchar,h.UpdateDate,112)<@StartDate)
				 or  Hist.LastProgress='HP'  and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
														   where h.CompanyCode=Hist.CompanyCode
															 and h.BranchCode=Hist.BranchCode
															 and h.InquiryNumber=Hist.InquiryNumber
															 and h.LastProgress not in ('P','HP')
															 and convert(varchar,h.UpdateDate,112)<@StartDate)
				 or Hist.LastProgress='SPK' and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
														   where h.CompanyCode=Hist.CompanyCode
															 and h.BranchCode=Hist.BranchCode
															 and h.InquiryNumber=Hist.InquiryNumber
															 and h.LastProgress not in ('P','HP','SPK')
															 and convert(varchar,h.UpdateDate,112)<@StartDate)))	 
				AND CONVERT(VARCHAR, HstITS.LastUpdateDate, 112) <= CONVERT(VARCHAR, @EndDate, 112))		
				 and HstITS.LastProgress not in ('DO','DELIVERY','LOST')															 
			 GROUP BY  Hist.CompanyCode, Hist.BranchCode, Wiraniaga, SalesCoordinator) #tP2

	SELECT * INTO #tLOST FROM (
			SELECT Hist.CompanyCode, Hist.BranchCode, Wiraniaga, COUNT(Hist.LastProgress) LOST, SalesCoordinator  
					FROM SuzukiR4..pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
					INNER JOIN SuzukiR4..pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON 
						HstITS.CompanyCode = Hist.CompanyCode AND
						HstITS.BranchCode = Hist.BranchCode AND
						HstITS.InquiryNumber = Hist.InquiryNumber 
					 WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE Hist.CompanyCode END) = @DealerCode
						AND (CASE WHEN @OutletCode = '' THEN '' ELSE Hist.BranchCode END) = @OutletCode	
						AND Hist.LastProgress = 'LOST'
						AND HstITS.LastProgress = 'LOST' -- penambahan 1 April 2014
						AND CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112) 
					GROUP BY Hist.CompanyCode, Hist.BranchCode, Wiraniaga, SalesCoordinator) #tLOST		
	
	SELECT * INTO #tNEW FROM (
		SELECT CompanyCode, BranchCode, Wiraniaga, COUNT(LastProgress) NEW, SalesCoordinator
		FROM SuzukiR4..pmHstITS WITH (NOLOCK, NOWAIT)
		WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE CompanyCode END) = @DealerCode
			AND (CASE WHEN @OutletCode = '' THEN '' ELSE BranchCode END) = @OutletCode	 
			AND CONVERT(VARCHAR, InquiryDate, 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)
		GROUP BY  CompanyCode, BranchCode, Wiraniaga, SalesCoordinator) #tNEW	
	
	SELECT * INTO #tSalesmanNational1 FROM(
		SELECT DISTINCT a.CompanyCode, a.BranchCode
		,ISNULL(case when HstITS.WiraNiaga = '' then 'SC-'+HstITS.SalesCoordinator else HstITS.Wiraniaga end, '') EmployeeID
		,'10' PositionID, ISNULL(HstITS.SalesCoordinator,'') SpvEmployeeID, ISNULL(HstITS.SalesHead, '') SalesHead, ISNULL(HstITS.BranchHead, '') BranchHead
		,ISNULL(NEW.NEW, 0) NEW		
		--, 0 NEW
		,ISNULL(CASE WHEN @TypeReport = '0' THEN P1.P ELSE P2.P END, (CASE WHEN HstITS.WiraNiaga IS NOT NULL THEN 0 ELSE (CASE WHEN @TypeReport = '0' THEN ISNULL((SELECT SUM(P) FROM #tP1 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND Wiraniaga = '[BLANK]'), 0) ELSE ISNULL((SELECT SUM(P) FROM #tP2 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND Wiraniaga = '[BLANK]'), 0) END) END)) P		 
		,ISNULL(CASE WHEN @TypeReport = '0' THEN HP1.HP ELSE HP2.HP END, (CASE WHEN HstITS.WiraNiaga IS NOT NULL THEN 0 ELSE (CASE WHEN @TypeReport = '0' THEN ISNULL((SELECT SUM(HP) FROM #tHP1 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND Wiraniaga = '[BLANK]'),0) ELSE ISNULL((SELECT SUM(HP) FROM #tHP2 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND Wiraniaga = '[BLANK]'),0) END) END)) HP		
		,ISNULL(CASE WHEN @TypeReport = '0' THEN SPK1.SPK ELSE SPK2.SPK END, (CASE WHEN HstITS.WiraNiaga IS NOT NULL THEN 0 ELSE (CASE WHEN @TypeReport = '0' THEN ISNULL((SELECT SUM(SPK) FROM #tSPK1 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND Wiraniaga = '[BLANK]'),0) ELSE ISNULL((SELECT SUM(SPK) FROM #tSPK2 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND Wiraniaga = '[BLANK]'),0) END) END)) SPK

		,(ISNULL(CASE WHEN @TypeReport = '0' THEN P1.P ELSE P2.P END, (CASE WHEN HstITS.WiraNiaga IS NOT NULL THEN 0 ELSE (CASE WHEN @TypeReport = '0' THEN ISNULL((SELECT SUM(P) FROM #tP1 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND Wiraniaga = '[BLANK]'), 0) ELSE ISNULL((SELECT SUM(P) FROM #tP2 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND Wiraniaga = '[BLANK]'), 0) END) END))
			+ISNULL(CASE WHEN @TypeReport = '0' THEN HP1.HP ELSE HP2.HP END, (CASE WHEN HstITS.WiraNiaga IS NOT NULL THEN 0 ELSE (CASE WHEN @TypeReport = '0' THEN ISNULL((SELECT SUM(HP) FROM #tHP1 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND Wiraniaga = '[BLANK]'),0) ELSE ISNULL((SELECT SUM(HP) FROM #tHP2 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND Wiraniaga = '[BLANK]'),0) END) END))
			+ISNULL(CASE WHEN @TypeReport = '0' THEN SPK1.SPK ELSE SPK2.SPK END, (CASE WHEN HstITS.WiraNiaga IS NOT NULL THEN 0 ELSE (CASE WHEN @TypeReport = '0' THEN ISNULL((SELECT SUM(SPK) FROM #tSPK1 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND Wiraniaga = '[BLANK]'),0) ELSE ISNULL((SELECT SUM(SPK) FROM #tSPK2 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND Wiraniaga = '[BLANK]'),0) END) END))) SumOuts

		,ISNULL(DO.DO, 0) DO, ISNULL(DELIVERY.DELIVERY, 0) DELIVERY, ISNULL(LOST.LOST, 0) LOST			
		FROM SuzukiR4..pmStatusHistory a WITH (NOLOCK, NOWAIT)
		LEFT JOIN SuzukiR4..pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON HstITS.CompanyCode = a.CompanyCode AND HstITS.BranchCode = a.BranchCode AND HstITS.InquiryNumber = a.InquiryNumber 
		LEFT JOIN SuzukiR4..gnMstDealerMapping b WITH (NOLOCK, NOWAIT) on a.CompanyCode = b.DealerCode	
		LEFT JOIN #tNEW NEW WITH (NOLOCK, NOWAIT) ON NEW.CompanyCode = a.CompanyCode AND NEW.BranchCode = a.BranchCode AND NEW.Wiraniaga = HstITS.Wiraniaga AND NEW.SalesCoordinator = HstITS.SalesCoordinator	
		LEFT JOIN #tP1 P1 WITH (NOLOCK, NOWAIT) ON P1.CompanyCode = a.CompanyCode AND P1.BranchCode = a.BranchCode AND P1.Wiraniaga = HstITS.Wiraniaga AND P1.SalesCoordinator = HstITS.SalesCoordinator	
		LEFT JOIN #tP2 P2 WITH (NOLOCK, NOWAIT)	ON P2.CompanyCode = a.CompanyCode AND P2.BranchCode = a.BranchCode AND P2.Wiraniaga = HstITS.Wiraniaga AND P2.SalesCoordinator = HstITS.SalesCoordinator				
		LEFT JOIN #tHP1 HP1 WITH (NOLOCK, NOWAIT) ON HP1.CompanyCode = a.CompanyCode AND HP1.BranchCode = a.BranchCode AND HP1.Wiraniaga = HstITS.Wiraniaga AND HP1.SalesCoordinator = HstITS.SalesCoordinator 			
		LEFT JOIN #tHP2 HP2 WITH (NOLOCK, NOWAIT) ON HP2.CompanyCode = a.CompanyCode AND HP2.BranchCode = a.BranchCode AND HP2.Wiraniaga = HstITS.Wiraniaga	AND HP2.SalesCoordinator = HstITS.SalesCoordinator 			
		LEFT JOIN #tSPK1 SPK1 WITH (NOLOCK, NOWAIT) ON SPK1.CompanyCode = a.CompanyCode AND SPK1.BranchCode = a.BranchCode AND SPK1.Wiraniaga = HstITS.Wiraniaga AND SPK1.SalesCoordinator = HstITS.SalesCoordinator 
		LEFT JOIN #tSPK2 SPK2 WITH (NOLOCK, NOWAIT) ON SPK2.CompanyCode = a.CompanyCode AND SPK2.BranchCode = a.BranchCode AND SPK2.Wiraniaga = HstITS.Wiraniaga AND SPK2.SalesCoordinator = HstITS.SalesCoordinator 	
		LEFT JOIN #tDO DO WITH (NOLOCK, NOWAIT) ON DO.CompanyCode = a.CompanyCode AND DO.BranchCode = a.BranchCode AND DO.Wiraniaga = HstITS.Wiraniaga AND DO.SalesCoordinator = HstITS.SalesCoordinator 	
		LEFT JOIN #tDELIVERY DELIVERY WITH (NOLOCK, NOWAIT) ON DELIVERY.CompanyCode = a.CompanyCode AND DELIVERY.BranchCode = a.BranchCode AND DELIVERY.Wiraniaga = HstITS.Wiraniaga AND DELIVERY.SalesCoordinator = HstITS.SalesCoordinator 			
		LEFT JOIN #tLOST LOST WITH (NOLOCK, NOWAIT) ON LOST.CompanyCode = a.CompanyCode AND LOST.BranchCode = a.BranchCode AND LOST.Wiraniaga = HstITS.Wiraniaga AND LOST.SalesCoordinator = HstITS.SalesCoordinator						   					   		   			   		
		WHERE (CASE WHEN @Area = '' THEN '' ELSE b.Area END) = @Area				
			  AND (CASE WHEN @DealerCode = '' THEN '' ELSE a.CompanyCode END) = @DealerCode
			  AND (CASE WHEN @OutletCode = '' THEN '' ELSE a.BranchCode END) = @OutletCode	 
	) #tSalesmanNational1
	
	SELECT * INTO #tSalesmanNational FROM(
		select NEW.CompanyCode, NEW.BranchCode, NEW.Wiraniaga EmployeeID, ISNULL(a.PositionID, '10') PositionID, NEW.SalesCoordinator SpvEmployeeID, isnull(a.SalesHead, '') SalesHead
		, isnull(a.BranchHead, '') BranchHead, ISNULL(NEW.NEW, 0) NEW, isnull(a.P, 0) P, isnull(a.HP, 0) HP, isnull(a.SPK, 0) SPK
		, ISNULL(a.SumOuts, 0) SumOuts, ISNULL(a.do, 0) DO, ISNULL(a.delivery, '') delivery, ISNULL(a.lost, 0) lost
		from #tNEW NEW
		left join #tSalesmanNational1 a WITH (NOLOCK, NOWAIT) ON NEW.CompanyCode = a.CompanyCode AND NEW.BranchCode = a.BranchCode AND NEW.Wiraniaga = a.EmployeeID AND NEW.SalesCoordinator = a.SpvEmployeeID
		where a.EmployeeID is null
		union
		select distinct CompanyCode, BranchCode, EmployeeID, PositionID, SpvEmployeeID,  
			(Select top 1 SalesHead from #tSalesmanNational1 a where a.CompanyCode = #tSalesmanNational1.CompanyCode and a.BranchCode = #tSalesmanNational1.BranchCode and a.EmployeeID = #tSalesmanNational1.EmployeeID) SalesHead, 
			BranchHead, NEW, P, HP, SPK,SumOuts, DO, Delivery, LOST
			from #tSalesmanNational1
			where BranchHead != ''
	) #tSalesmanNational	
	--select * from #tSalesmanNational
		
	END	 	
	ELSE
	BEGIN
	SELECT * INTO #tSalesman FROM(
	SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, a.PositionID, 
	(SELECT Distinct(EmployeeID) FROM SuzukiR4..PmMstTeamMembers 
	WHERE CompanyCode = a.CompanyCode AND 
		BranchCode = a.BranchCode AND 
		TeamID IN (SELECT Distinct(TeamID) FROM SuzukiR4..PmMstTeamMembers 
					WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE CompanyCode END) = @DealerCode
					AND (CASE WHEN @OutletCode = '' THEN '' ELSE BranchCode END) = @OutletCode	 
					AND EmployeeID = a.EmployeeID AND IsSupervisor = 0)
		AND IsSupervisor = 1) SpvEmployeeID,
	(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
	WHERE CompanyCode = a.CompanyCode AND
		BranchCode = a.BranchCode AND
		EmployeeID = a.EmployeeID AND
		CONVERT(VARCHAR, InquiryDate, 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)) NEW,
		
	CASE WHEN @TypeReport = '0' THEN
		(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
		WHERE CompanyCode = a.CompanyCode AND
			BranchCode = a.BranchCode AND
			EmployeeID = a.EmployeeID AND
			LastProgress = 'P' AND 
			CONVERT(VARCHAR, LastUpdateStatus, 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)) 
	ELSE
		(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
		WHERE CompanyCode = a.CompanyCode AND
			BranchCode = a.BranchCode AND
			EmployeeID = a.EmployeeID AND
			LastProgress = 'P' AND 
			CONVERT(VARCHAR, LastUpdateStatus, 112) <= CONVERT(VARCHAR, @EndDate, 112)) END P, 
			
	CASE WHEN @TypeReport = '0' THEN			
		(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
		WHERE CompanyCode = a.CompanyCode AND
			BranchCode = a.BranchCode AND
			EmployeeID = a.EmployeeID and
			LastProgress = 'HP' AND 
			CONVERT(VARCHAR, LastUpdateStatus, 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112))
	ELSE
		(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
		WHERE CompanyCode = a.CompanyCode AND
			BranchCode = a.BranchCode AND
			EmployeeID = a.EmployeeID and
			LastProgress = 'HP' AND 
			CONVERT(VARCHAR, LastUpdateStatus, 112) <= CONVERT(VARCHAR, @EndDate, 112)) END HP, 
			
	(CASE WHEN @TypeReport = '0' THEN
		(SELECT COUNT(Hist.LastProgress) FROM SuzukiR4..pmStatusHistory Hist
			INNER JOIN SuzukiR4..pmKdp KDP ON 
			KDP.CompanyCode = Hist.CompanyCode AND
			KDP.BranchCode = Hist.BranchCode AND
			KDP.InquiryNumber = Hist.InquiryNumber 
		 WHERE Hist.CompanyCode = a.CompanyCode AND
			Hist.BranchCode = a.BranchCode AND
			KDP.EmployeeID = a.EmployeeID AND
			CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112) AND
			Hist.LastProgress = 'SPK')
		ELSE
		(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
			WHERE CompanyCode = a.CompanyCode AND
			BranchCode = a.BranchCode AND
			EmployeeID = a.EmployeeID and
			LastProgress = 'SPK' AND 
			CONVERT(VARCHAR, LastUpdateStatus, 112) <= CONVERT(VARCHAR, @EndDate, 112)) 
			END) SPK,
			
	(CASE WHEN @TypeReport = '0' THEN			  
		(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
		WHERE CompanyCode = a.CompanyCode AND
			BranchCode = a.BranchCode AND
			EmployeeID = a.EmployeeID AND
			LastProgress = 'P' AND 
			CONVERT(VARCHAR, LastUpdateStatus, 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)) +
		(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
		WHERE CompanyCode = a.CompanyCode AND
			BranchCode = a.BranchCode AND
			EmployeeID = a.EmployeeID and
			LastProgress = 'HP' AND 
			CONVERT(VARCHAR, LastUpdateStatus, 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)) +		  
		(SELECT COUNT(Hist.LastProgress) FROM SuzukiR4..pmStatusHistory Hist
			INNER JOIN SuzukiR4..pmKdp KDP ON 
			KDP.CompanyCode = Hist.CompanyCode AND
			KDP.BranchCode = Hist.BranchCode AND
			KDP.InquiryNumber = Hist.InquiryNumber 
		 WHERE Hist.CompanyCode = a.CompanyCode AND
			Hist.BranchCode = a.BranchCode AND
			KDP.EmployeeID = a.EmployeeID AND
			CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112) AND
			Hist.LastProgress = 'SPK')
	ELSE
		(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
		WHERE CompanyCode = a.CompanyCode AND
			BranchCode = a.BranchCode AND
			EmployeeID = a.EmployeeID AND
			LastProgress = 'P' AND 
			CONVERT(VARCHAR, LastUpdateStatus, 112) <= CONVERT(VARCHAR, @EndDate, 112)) +
		(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
		WHERE CompanyCode = a.CompanyCode AND
			BranchCode = a.BranchCode AND
			EmployeeID = a.EmployeeID AND
			LastProgress = 'HP' AND 
			CONVERT(VARCHAR, LastUpdateStatus, 112) <= CONVERT(VARCHAR, @EndDate, 112)) +				
		(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
		WHERE CompanyCode = a.CompanyCode AND
			BranchCode = a.BranchCode AND
			EmployeeID = a.EmployeeID and
			LastProgress = 'SPK' AND 
			CONVERT(VARCHAR, LastUpdateStatus, 112) <= CONVERT(VARCHAR, @EndDate, 112)) 
	 END) SumOuts
	 
	, (SELECT COUNT(Hist.LastProgress) FROM SuzukiR4..pmStatusHistory Hist
		INNER JOIN SuzukiR4..pmKdp KDP ON 
		KDP.CompanyCode = Hist.CompanyCode AND
		KDP.BranchCode = Hist.BranchCode AND
		KDP.InquiryNumber = Hist.InquiryNumber 
	 WHERE Hist.CompanyCode = a.CompanyCode AND
		Hist.BranchCode = a.BranchCode AND
		KDP.EmployeeID = a.EmployeeID AND
		CONVERT(VARCHAR, Hist.UpdateDate, 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112) AND
		Hist.LastProgress = 'DO') DO
	, (SELECT COUNT(Hist.LastProgress) FROM SuzukiR4..pmStatusHistory Hist
		INNER JOIN SuzukiR4..pmKdp KDP ON 
		KDP.CompanyCode = Hist.CompanyCode AND
		KDP.BranchCode = Hist.BranchCode AND
		KDP.InquiryNumber = Hist.InquiryNumber 
	 WHERE Hist.CompanyCode = a.CompanyCode AND
		Hist.BranchCode = a.BranchCode AND
		KDP.EmployeeID = a.EmployeeID AND
		CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112) AND
		Hist.LastProgress = 'DELIVERY') DELIVERY
	, (SELECT COUNT(Hist.LastProgress) FROM SuzukiR4..pmStatusHistory Hist
		INNER JOIN SuzukiR4..pmKdp KDP ON 
		KDP.CompanyCode = Hist.CompanyCode AND
		KDP.BranchCode = Hist.BranchCode AND
		KDP.InquiryNumber = Hist.InquiryNumber 
	 WHERE Hist.CompanyCode = a.CompanyCode AND
		Hist.BranchCode = a.BranchCode AND
		KDP.EmployeeID = a.EmployeeID AND
		CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112) AND
		Hist.LastProgress = 'LOST') LOST
FROM SuzukiR4..PmPosition a
LEFT JOIN SuzukiR4..gnMstDealerMapping b on a.CompanyCode = b.DealerCode	
WHERE (b.Area like Case when @Area <> ''
			then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
					then 'JABODETABEK'
					else @Area end
			else '' end
	   or b.Area like Case when @Area <> '' 
					then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
							then 'CABANG'
							else @Area end
					else '' end) 						
	  AND (CASE WHEN @DealerCode = '' THEN '' ELSE b.DealerCode END) = @DealerCode
	  AND (CASE WHEN @OutletCode = '' THEN '' ELSE a.BranchCode END) = @OutletCode	 
	  AND a.PositionID = 10 		
      ) #tSalesman
END  
--#endregion

--#region  SELECT COORD 
IF @National = '1'
BEGIN
	SELECT * INTO #tSalesCoNational  FROM(	 
	SELECT DISTINCT
		a.CompanyCode,
		a.BranchCode,
		a.SpvEmployeeID EmployeeID,
		'20' PositionID,
		SalesHead ShEmployeeID,
		BranchHead BMEmployeeID,
		ISNULL(SUM(a.NEW),0) NEW,
		ISNULL(SUM(a.P),0) P,
		ISNULL(SUM(a.HP),0) HP,
		ISNULL(SUM(a.SPK),0) SPK,
		ISNULL(SUM(a.SumOuts),0) SumOuts,
		ISNULL(SUM(a.DO),0) DO,
		ISNULL(SUM(a.DELIVERY),0) DELIVERY,
		ISNULL(SUM(a.LOST),0) LOST
	FROM #tSalesmanNational  a
	--WHERE a.SpvEmployeeID <> ''
	GROUP BY a.CompanyCode,
		a.BranchCode,
		a.SpvEmployeeID,
		a.SalesHead,
		a.BranchHead
	) #tSalesCoNational 		
END
ELSE
BEGIN
	SELECT * INTO #tSalesCo FROM(	 
	SELECT
		a.CompanyCode,
		a.BranchCode,
		a.SpvEmployeeID EmployeeID,
		'20' PositionID,
		(SELECT Distinct(EmployeeID) FROM SuzukiR4..PmMstTeamMembers 
		WHERE CompanyCode = a.CompanyCode AND 
			BranchCode = a.BranchCode AND 
			TeamID IN (SELECT Distinct(TeamID) FROM SuzukiR4..PmMstTeamMembers 
						WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE CompanyCode END) = @DealerCode
						AND (CASE WHEN @OutletCode = '' THEN '' ELSE BranchCode END) = @OutletCode	 
						AND EmployeeID = a.SpvEmployeeID AND IsSupervisor = 0)
			AND IsSupervisor = 1) ShEmployeeID,
		ISNULL(SUM(a.NEW),0) NEW,
		ISNULL(SUM(a.P),0) P,
		ISNULL(SUM(a.HP),0) HP,
		ISNULL(SUM(a.SPK),0) SPK,
		ISNULL(SUM(a.SumOuts),0) SumOuts,
		ISNULL(SUM(a.DO),0) DO,
		ISNULL(SUM(a.DELIVERY),0) DELIVERY,
		ISNULL(SUM(a.LOST),0) LOST
	FROM #tSalesman a
	GROUP BY a.CompanyCode,
		a.BranchCode,
		a.SpvEmployeeID
	) #tSalesCo
END	
--#endregion

--#region  SELECT SALES HEAD 	 
IF @National = '1'
BEGIN
	SELECT * INTO #tSalesHeadNational FROM(			
	SELECT DISTINCT
		a.CompanyCode,
		a.BranchCode,
		a.ShEmployeeID EmployeeID,
		'30' PositionID,
		a.BMEmployeeID,
		ISNULL(SUM(a.NEW),0) NEW,
		ISNULL(SUM(a.P),0) P,
		ISNULL(SUM(a.HP),0) HP,
		ISNULL(SUM(a.SPK),0) SPK,
		ISNULL(SUM(a.SumOuts),0) SumOuts,
		ISNULL(SUM(a.DO),0) DO,
		ISNULL(SUM(a.DELIVERY),0) DELIVERY,
		ISNULL(SUM(a.LOST),0) LOST
	FROM #tSalesCoNational a
	--WHERE a.ShEmployeeID <> ''
	GROUP BY a.CompanyCode,
		a.BranchCode,
		a.ShEmployeeID,
		a.BMEmployeeID
	) #tSalesHeadNational
END
ELSE
BEGIN
	SELECT * INTO #tSalesHead FROM(			
	SELECT
		a.CompanyCode,
		a.BranchCode,
		a.ShEmployeeID EmployeeID,
		'30' PositionID,
		(SELECT Distinct(EmployeeID) FROM SuzukiR4..PmMstTeamMembers 
		WHERE CompanyCode = a.CompanyCode AND 
			BranchCode = a.BranchCode AND 
			TeamID IN (SELECT Distinct(TeamID) FROM SuzukiR4..PmMstTeamMembers 
						WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE CompanyCode END) = @DealerCode
						AND (CASE WHEN @OutletCode = '' THEN '' ELSE BranchCode END) = @OutletCode	 
						AND EmployeeID = a.ShEmployeeID AND IsSupervisor = 0)
			AND IsSupervisor = 1) BMEmployeeID,
		ISNULL(SUM(a.NEW),0) NEW,
		ISNULL(SUM(a.P),0) P,
		ISNULL(SUM(a.HP),0) HP,
		ISNULL(SUM(a.SPK),0) SPK,
		ISNULL(SUM(a.SumOuts),0) SumOuts,
		ISNULL(SUM(a.DO),0) DO,
		ISNULL(SUM(a.DELIVERY),0) DELIVERY,
		ISNULL(SUM(a.LOST),0) LOST
	FROM #tSalesCo a
	GROUP BY a.CompanyCode,
		a.BranchCode,
		a.ShEmployeeID
	) #tSalesHead
END
--#endregion

--#region  SELECT BRANCH MANAGER 	 
IF @National = '1'
BEGIN
	SELECT * INTO #tSalesBMNational FROM(			
	SELECT DISTINCT
		a.CompanyCode,
		a.BranchCode,
		a.BMEmployeeID EmployeeID,
		'40' PositionID,
		'' SpvEmployeeID,
		ISNULL(SUM(a.NEW),0) NEW,
		ISNULL(SUM(a.P),0) P,
		ISNULL(SUM(a.HP),0) HP,
		ISNULL(SUM(a.SPK),0) SPK,
		ISNULL(SUM(a.SumOuts),0) SumOuts,
		ISNULL(SUM(a.DO),0) DO,
		ISNULL(SUM(a.DELIVERY),0) DELIVERY,
		ISNULL(SUM(a.LOST),0) LOST
		FROM #tSalesHeadNational a
		--WHERE a.BMEmployeeID <> ''
	GROUP BY a.CompanyCode,
		a.BranchCode,
		a.BMEmployeeID
	) #tSalesBMNational			
END
ELSE
BEGIN
	SELECT * INTO #tSalesBM FROM(			
	SELECT
		a.CompanyCode,
		a.BranchCode,
		a.BMEmployeeID EmployeeID,
		'40' PositionID,
		'' SpvEmployeeID,
		ISNULL(SUM(a.NEW),0) NEW,
		ISNULL(SUM(a.P),0) P,
		ISNULL(SUM(a.HP),0) HP,
		ISNULL(SUM(a.SPK),0) SPK,
		ISNULL(SUM(a.SumOuts),0) SumOuts,
		ISNULL(SUM(a.DO),0) DO,
		ISNULL(SUM(a.DELIVERY),0) DELIVERY,
		ISNULL(SUM(a.LOST),0) LOST
		FROM #tSalesHead a
	GROUP BY a.CompanyCode,
		a.BranchCode,
		a.BMEmployeeID
	) #tSalesBM	
END	
--#endregion

--#region  SELECT UNION ALL
IF @National = '1'
BEGIN
	SELECT * INTO #tUnionNational FROM(
	SELECT DISTINCT CompanyCode, BranchCode, EmployeeID, PositionID, NEW, P, HP, SPK, SumOuts, DO, DELIVERY, LOST FROM #tSalesBMNational
	WHERE EmployeeID != NULL UNION
	SELECT DISTINCT CompanyCode, BranchCode, EmployeeID, PositionID, NEW, P, HP, SPK, SumOuts, DO, DELIVERY, LOST FROM #tSalesHeadNational 
	WHERE (CASE WHEN @SalesHead  = '' THEN '' ELSE EmployeeID END) = @SalesHead UNION
	SELECT DISTINCT CompanyCode, BranchCode, EmployeeID, PositionID, NEW, P, HP, SPK, SumOuts, DO, DELIVERY, LOST FROM #tSalesCoNational 
	WHERE (CASE WHEN @SalesCoordinator  = '' THEN '' ELSE EmployeeID END) = @SalesCoordinator
		AND (CASE WHEN @SalesHead  = '' THEN '' ELSE ShEmployeeID END) = @SalesHead
	UNION
	SELECT DISTINCT CompanyCode, BranchCode, EmployeeID, PositionID, NEW, P, HP, SPK, SumOuts, DO, DELIVERY, LOST FROM #tSalesmanNational
	WHERE (CASE WHEN @SalesHead  = '' THEN '' ELSE SalesHead END) = @SalesHead
		AND (CASE WHEN @SalesCoordinator  = '' THEN '' ELSE SpvEmployeeID END) = @SalesCoordinator
		AND (CASE WHEN @Salesman  = '' THEN '' ELSE EmployeeID END) = @Salesman
	) #tUnionNational
	ORDER BY PositionID DESC		
END
ELSE
BEGIN	
	SELECT * INTO #tUnion FROM(
	SELECT * FROM #tSalesBM UNION
	SELECT * FROM #tSalesHead 
	WHERE (CASE WHEN @SalesHead  = '' THEN '' ELSE EmployeeID END) = @SalesHead UNION
	SELECT * FROM #tSalesCo 
	WHERE (CASE WHEN @SalesCoordinator  = '' THEN '' ELSE EmployeeID END) = @SalesCoordinator
	UNION
	SELECT * FROM #tSalesman
	WHERE (CASE WHEN @SalesCoordinator  = '' THEN '' ELSE SpvEmployeeID END) = @SalesCoordinator
		AND (CASE WHEN @Salesman  = '' THEN '' ELSE EmployeeID END) = @Salesman
	) #tUnion
	ORDER BY PositionID DESC
END
--#endregion

--#region SELECT ALL
IF @National = '1'
	BEGIN		
		SELECT CASE WHEN @TypeReport = '0' THEN 'Summary' ELSE 'Saldo' END TypeReport,
			CASE @ProductivityBy 
				WHEN '0' THEN 'Salesman'
				WHEN '1' THEN 'Vehicle Type'
				WHEN '2' THEN 'Source Data'
			END ProductivityBy,
			CONVERT(VARCHAR, @EndDate, 105) PerDate, 
			CASE WHEN @Area = '' THEN 'ALL' ELSE @Area END Area,
			CONVERT(VARCHAR, @StartDate, 105) + ' s/d ' + CONVERT(VARCHAR, @EndDate, 105) PeriodeDO,
			CASE WHEN @SalesHead = '' THEN 'ALL' ELSE @SalesHead END SalesHead,
			CASE WHEN @SalesCoordinator = '' THEN 'ALL' ELSE @SalesCoordinator END SalesCoordinator,
			CASE WHEN @Salesman = '' THEN 'ALL' ELSE @Salesman END Salesman,			
			(SELECT LookupValueName FROM SuzukiR4..GnMstLookUpDtl WHERE CodeID = 'PGRD' AND LookUpValue = a.PositionID) Position,
			CASE WHEN @DealerCode = '' THEN 'ALL' ELSE (SELECT DealerName FROM SuzukiR4..gnMstDealerMapping WHERE DealerCode = @DealerCode and Area = @Area ) END Dealer,
			CASE WHEN @OutletCode = '' THEN 'ALL' ELSE (SELECT OutletName FROM SuzukiR4..gnMstDealerOutletMapping WHERE CompanyCode = @DealerCode AND OutletCode = @OutletCode) END Outlet,
			a.EmployeeID, a.EmployeeID EmployeeName,		
			ISNULL(SUM(a.P), 0) NEW,
			ISNULL(SUM(a.P), 0) P,
			ISNULL(SUM(a.HP), 0) HP,
			ISNULL(SUM(a.SPK), 0) SPK,
			ISNULL(SUM(a.SumOuts), 0) SumOuts,
			ISNULL(SUM(a.DO), 0) DO,
			ISNULL(SUM(a.DELIVERY), 0) DELIVERY,
			ISNULL(SUM(a.LOST), 0) LOST	
			
			--ISNULL(CASE CAST(a.NEW AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(a.NEW AS VARCHAR) END, '-') NEW,
			--ISNULL(CASE CAST(a.P AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(a.P AS VARCHAR) END, '-') P,
			--ISNULL(CASE CAST(a.HP AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(a.HP AS VARCHAR) END, '-') HP,
			--ISNULL(CASE CAST(a.SPK AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(a.SPK AS VARCHAR) END, '-') SPK,
			--ISNULL(CASE CAST(a.SumOuts AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(a.SumOuts AS VARCHAR) END, '-') SumOuts,
			--ISNULL(CASE CAST(a.DO AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(a.DO AS VARCHAR) END, '-') DO,
			--ISNULL(CASE CAST(a.DELIVERY AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(a.DELIVERY AS VARCHAR) END, '-') DELIVERY,
			--ISNULL(CASE CAST(a.LOST AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(a.LOST AS VARCHAR) END, '-') LOST	
		FROM #tUnionNational a
		LEFT JOIN SuzukiR4..gnMstDealerMapping c on a.CompanyCode = c.DealerCode	
		WHERE 
		 (CASE WHEN @DealerCode = '' THEN '' ELSE c.DealerCode END) = @DealerCode
		  AND (CASE WHEN @OutletCode = '' THEN '' ELSE a.BranchCode END) = @OutletCode	 
		  AND a.PositionID <> 60 
		  AND a.EmployeeID != 'NULL' 
		GROUP BY a.CompanyCode, a.BranchCode, a.PositionID, a.EmployeeID
		ORDER BY  a.CompanyCode, a.BranchCode, a.PositionID DESC
		
		DROP TABLE #tSalesmanNational, #tSalesCoNational, #tSalesHeadNational, #tSalesBMNational, #tUnionNational, #tSPK1, #tSPK2, #tDO, #tDELIVERY, #tLOST, #tHP1, #tHP2, #tP1, #tNEW, #tP2, #tSalesmanNational1
	END
	ELSE 
	BEGIN
		SELECT CASE WHEN @TypeReport = '0' THEN 'Summary' ELSE 'Saldo' END TypeReport,
			CASE @ProductivityBy 
				WHEN '0' THEN 'Salesman'
				WHEN '1' THEN 'Vehicle Type'
				WHEN '2' THEN 'Source Data'
			END ProductivityBy,
			CONVERT(VARCHAR, @EndDate, 105) PerDate,  CASE WHEN @Area = '' THEN 'ALL' ELSE @Area END Area, CONVERT(VARCHAR, @StartDate, 105) + ' s/d ' + CONVERT(VARCHAR, @EndDate, 105) PeriodeDO,
			CASE WHEN @SalesHead = '' THEN 'ALL' ELSE (SELECT EmployeeName FROM SuzukiR4..GnMstEmployee WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND EmployeeID = @SalesHead) END SalesHead,
			CASE WHEN @SalesCoordinator = '' THEN 'ALL' ELSE (SELECT EmployeeName FROM SuzukiR4..GnMstEmployee WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND EmployeeID = @SalesCoordinator) END SalesCoordinator,
			CASE WHEN @Salesman = '' THEN 'ALL' ELSE (SELECT EmployeeName FROM SuzukiR4..GnMstEmployee WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND EmployeeID = @Salesman) END Salesman,
			(SELECT LookupValueName FROM SuzukiR4..GnMstLookUpDtl WHERE CompanyCode = a.CompanyCode AND CodeID = 'PGRD' AND LookUpValue = a.PositionID) Position,
			CASE WHEN @DealerCode = '' THEN 'ALL' ELSE (SELECT DealerName FROM SuzukiR4..gnMstDealerMapping WHERE DealerCode = @DealerCode) END Dealer,
			CASE WHEN @OutletCode = '' THEN 'ALL' ELSE (SELECT BranchName FROM SuzukiR4..gnMstOrganizationDtl WHERE CompanyCode = @DealerCode AND BranchCode = @OutletCode) END Outlet,
			b.EmployeeID,
			(SELECT EmployeeName FROM SuzukiR4..GnMstEmployee WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND EmployeeID = a.EmployeeID) EmployeeName,			
			ISNULL(SUM(a.P), 0) NEW,
			ISNULL(SUM(a.P), 0) P,
			ISNULL(SUM(a.HP), 0) HP,
			ISNULL(SUM(a.SPK), 0) SPK,
			ISNULL(SUM(a.SumOuts), 0) SumOuts,
			ISNULL(SUM(a.DO), 0) DO,
			ISNULL(SUM(a.DELIVERY), 0) DELIVERY,
			ISNULL(SUM(a.LOST), 0) LOST	
			
			--ISNULL(CASE CAST(b.NEW AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(b.NEW AS VARCHAR) END, '-') NEW,
			--ISNULL(CASE CAST(b.P AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(b.P AS VARCHAR) END, '-') P,
			--ISNULL(CASE CAST(b.HP AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(b.HP AS VARCHAR) END, '-') HP,
			--ISNULL(CASE CAST(b.SPK AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(b.SPK AS VARCHAR) END, '-') SPK,
			--ISNULL(CASE CAST(b.SumOuts AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(b.SumOuts AS VARCHAR) END, '-') SumOuts,
			--ISNULL(CASE CAST(b.DO AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(b.DO AS VARCHAR) END, '-') DO,
			--ISNULL(CASE CAST(b.DELIVERY AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(b.DELIVERY AS VARCHAR) END, '-') DELIVERY,
			--ISNULL(CASE CAST(b.LOST AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(b.LOST AS VARCHAR) END, '-') LOST	
		FROM SuzukiR4..PmPosition a
		INNER JOIN #tUnion b ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.EmployeeID = b.EmployeeID AND a.PositionID = b.PositionID
		LEFT JOIN SuzukiR4..gnMstDealerMapping c on a.CompanyCode = c.DealerCode	
		WHERE 
		 (CASE WHEN @DealerCode = '' THEN '' ELSE c.DealerCode END) = @DealerCode
		  AND (CASE WHEN @OutletCode = '' THEN '' ELSE a.BranchCode END) = @OutletCode	 
		  AND a.PositionID <> 60  
		  AND b.SpvEmployeeID IS NOT NULL
		GROUP BY a.CompanyCode, a.BranchCode, a.PositionID, a.EmployeeID
		ORDER BY a.BranchCode, a.PositionID DESC
		DROP TABLE #tSalesman, #tSalesCo, #tSalesHead, #tSalesBM, #tUnion	
	END	
--#endregion

END
--#endregion

--#region PRODUCTIVITY BY VEHICLE TYPE (1)
ELSE IF @ProductivityBy	= '1'
BEGIN				
	IF @National = '1'
	BEGIN					
	select * into #tNEW2 from(
	SELECT CompanyCode, BranchCode, TipeKendaraan, Variant, COUNT(LastProgress) NEW
		FROM SuzukiR4..pmHstITS WITH (NOLOCK, NOWAIT)
		WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE CompanyCode END) = @DealerCode
			AND (CASE WHEN @OutletCode = '' THEN '' ELSE BranchCode END) = @OutletCode	 
			AND CONVERT(VARCHAR, InquiryDate, 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)
		GROUP BY CompanyCode, BranchCode, TipeKendaraan, Variant) #tNEW2									
					
	SELECT * INTO #tP21 FROM (
		SELECT Hist.CompanyCode, Hist.BranchCode,  ISNULL(TipeKendaraan, '[BLANK]') TipeKendaraan, ISNULL(Variant, '[BLANK]') Variant,
		COUNT(Hist.LastProgress) P
		FROM SuzukiR4..pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
		LEFT JOIN SuzukiR4..pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON 
		HstITS.CompanyCode = Hist.CompanyCode AND
		HstITS.BranchCode = Hist.BranchCode AND
		HstITS.InquiryNumber = Hist.InquiryNumber 
		WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE Hist.CompanyCode END) = @DealerCode
		AND (CASE WHEN @OutletCode = '' THEN '' ELSE Hist.BranchCode END) = @OutletCode	
		AND Hist.LastProgress = 'P'
		AND HstITS.LastProgress = 'P' -- penambahan 1 April 2014
		AND (CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)				
		 or (convert(varchar,Hist.UpdateDate,112) < @StartDate 
		and (Hist.LastProgress='P'   and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
												   where h.CompanyCode=Hist.CompanyCode
													 and h.BranchCode=Hist.BranchCode
													 and h.InquiryNumber=Hist.InquiryNumber
													 and h.LastProgress<>'P'
													 and convert(varchar,h.UpdateDate,112)<@StartDate)
		 or  Hist.LastProgress='HP'  and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
												   where h.CompanyCode=Hist.CompanyCode
													 and h.BranchCode=Hist.BranchCode
													 and h.InquiryNumber=Hist.InquiryNumber
													 and h.LastProgress not in ('P','HP')
													 and convert(varchar,h.UpdateDate,112)<@StartDate)
		 or Hist.LastProgress='SPK' and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
												   where h.CompanyCode=Hist.CompanyCode
													 and h.BranchCode=Hist.BranchCode
													 and h.InquiryNumber=Hist.InquiryNumber
													 and h.LastProgress not in ('P','HP','SPK')
													 and convert(varchar,h.UpdateDate,112)<@StartDate))))	 
													  and HstITS.LastProgress not in ('DO','DELIVERY','LOST')		
													  AND CONVERT(VARCHAR, HstITS.LastUpdateStatus , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)	
		GROUP BY  Hist.CompanyCode, Hist.BranchCode, TipeKendaraan, Variant) #tP21	

	SELECT * INTO #tP22 FROM(
		SELECT Hist.CompanyCode, Hist.BranchCode,  ISNULL(TipeKendaraan, '[BLANK]') TipeKendaraan, ISNULL(Variant, '[BLANK]') Variant,
		COUNT(Hist.LastProgress) P
		FROM SuzukiR4..pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
		LEFT JOIN SuzukiR4..pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON 
		HstITS.CompanyCode = Hist.CompanyCode AND
		HstITS.BranchCode = Hist.BranchCode AND
		HstITS.InquiryNumber = Hist.InquiryNumber 
		WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE Hist.CompanyCode END) = @DealerCode
		AND (CASE WHEN @OutletCode = '' THEN '' ELSE Hist.BranchCode END) = @OutletCode	
		AND Hist.LastProgress = 'P'
		AND HstITS.LastProgress = 'P' -- penambahan 1 April 2014
		AND (CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)				
		 or (convert(varchar,Hist.UpdateDate,112) < @StartDate 
		and (Hist.LastProgress='P'   and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
												   where h.CompanyCode=Hist.CompanyCode
													 and h.BranchCode=Hist.BranchCode
													 and h.InquiryNumber=Hist.InquiryNumber
													 and h.LastProgress<>'P'
													 and convert(varchar,h.UpdateDate,112)<@StartDate)
		 or  Hist.LastProgress='HP'  and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
												   where h.CompanyCode=Hist.CompanyCode
													 and h.BranchCode=Hist.BranchCode
													 and h.InquiryNumber=Hist.InquiryNumber
													 and h.LastProgress not in ('P','HP')
													 and convert(varchar,h.UpdateDate,112)<@StartDate)
		 or Hist.LastProgress='SPK' and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
												   where h.CompanyCode=Hist.CompanyCode
													 and h.BranchCode=Hist.BranchCode
													 and h.InquiryNumber=Hist.InquiryNumber
													 and h.LastProgress not in ('P','HP','SPK')
													 and convert(varchar,h.UpdateDate,112)<@StartDate))))	 
		AND CONVERT(VARCHAR, HstITS.LastUpdateStatus, 112) <= CONVERT(VARCHAR, @EndDate, 112)
		 and HstITS.LastProgress not in ('DO','DELIVERY','LOST')												
		GROUP BY  Hist.CompanyCode, Hist.BranchCode, TipeKendaraan, Variant) #tP22

	SELECT * INTO #tHP21 FROM(
		SELECT Hist.CompanyCode, Hist.BranchCode, ISNULL(TipeKendaraan, '[BLANK]') TipeKendaraan, ISNULL(Variant, '[BLANK]') Variant,
				COUNT(Hist.LastProgress) HP
				FROM SuzukiR4..pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
				LEFT JOIN SuzukiR4..pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON 
					HstITS.CompanyCode = Hist.CompanyCode AND
					HstITS.BranchCode = Hist.BranchCode AND
					HstITS.InquiryNumber = Hist.InquiryNumber 
				 WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE Hist.CompanyCode END) = @DealerCode
					AND (CASE WHEN @OutletCode = '' THEN '' ELSE Hist.BranchCode END) = @OutletCode	
					AND Hist.LastProgress = 'HP'
					AND HstITS.LastProgress = 'HP' -- penambahan 1 April 2014
					AND (CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)				
					 or (convert(varchar,Hist.UpdateDate,112) < @StartDate 
					and (Hist.LastProgress='P'   and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
															   where h.CompanyCode=Hist.CompanyCode
																 and h.BranchCode=Hist.BranchCode
																 and h.InquiryNumber=Hist.InquiryNumber
																 and h.LastProgress<>'P'
																 and convert(varchar,h.UpdateDate,112)<@StartDate)
					 or  Hist.LastProgress='HP'  and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
															   where h.CompanyCode=Hist.CompanyCode
																 and h.BranchCode=Hist.BranchCode
																 and h.InquiryNumber=Hist.InquiryNumber
																 and h.LastProgress not in ('P','HP')
																 and convert(varchar,h.UpdateDate,112)<@StartDate)
					 or Hist.LastProgress='SPK' and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
															   where h.CompanyCode=Hist.CompanyCode
																 and h.BranchCode=Hist.BranchCode
																 and h.InquiryNumber=Hist.InquiryNumber
																 and h.LastProgress not in ('P','HP','SPK')
																 and convert(varchar,h.UpdateDate,112)<@StartDate))))	
																  and HstITS.LastProgress not in ('DO','DELIVERY','LOST')		 
																  AND CONVERT(VARCHAR, HstITS.LastUpdateStatus , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)	
				 GROUP BY  Hist.CompanyCode, Hist.BranchCode, TipeKendaraan, Variant) #tHP21


	SELECT * INTO #tHP22 FROM(
		SELECT Hist.CompanyCode, Hist.BranchCode, ISNULL(TipeKendaraan, '[BLANK]') TipeKendaraan, ISNULL(Variant, '[BLANK]') Variant,
				COUNT(Hist.LastProgress) HP
				FROM SuzukiR4..pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
				LEFT JOIN SuzukiR4..pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON 
					HstITS.CompanyCode = Hist.CompanyCode AND
					HstITS.BranchCode = Hist.BranchCode AND
					HstITS.InquiryNumber = Hist.InquiryNumber 
				 WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE Hist.CompanyCode END) = @DealerCode
					AND (CASE WHEN @OutletCode = '' THEN '' ELSE Hist.BranchCode END) = @OutletCode	
					AND Hist.LastProgress = 'HP'
					AND HstITS.LastProgress = 'HP' -- penambahan 1 April 2014
					AND (CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)				
					 or (convert(varchar,Hist.UpdateDate,112) < @StartDate 
					and (Hist.LastProgress='P'   and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
															   where h.CompanyCode=Hist.CompanyCode
																 and h.BranchCode=Hist.BranchCode
																 and h.InquiryNumber=Hist.InquiryNumber
																 and h.LastProgress<>'P'
																 and convert(varchar,h.UpdateDate,112)<@StartDate)
					 or  Hist.LastProgress='HP'  and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
															   where h.CompanyCode=Hist.CompanyCode
																 and h.BranchCode=Hist.BranchCode
																 and h.InquiryNumber=Hist.InquiryNumber
																 and h.LastProgress not in ('P','HP')
																 and convert(varchar,h.UpdateDate,112)<@StartDate)
					 or Hist.LastProgress='SPK' and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
															   where h.CompanyCode=Hist.CompanyCode
																 and h.BranchCode=Hist.BranchCode
																 and h.InquiryNumber=Hist.InquiryNumber
																 and h.LastProgress not in ('P','HP','SPK')
																 and convert(varchar,h.UpdateDate,112)<@StartDate))))	 
				 AND CONVERT(VARCHAR, HstITS.LastUpdateStatus, 112) <= CONVERT(VARCHAR, @EndDate, 112)			
				  and HstITS.LastProgress not in ('DO','DELIVERY','LOST')														 
				 GROUP BY  Hist.CompanyCode, Hist.BranchCode, TipeKendaraan, Variant) #tHP22

	SELECT * INTO #tSPK21 FROM(
	SELECT Hist.CompanyCode, Hist.BranchCode, ISNULL(TipeKendaraan, '[BLANK]') TipeKendaraan, ISNULL(Variant, '[BLANK]') Variant,
				COUNT(Hist.LastProgress) SPK
				FROM SuzukiR4..pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
				LEFT JOIN SuzukiR4..pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON 
					HstITS.CompanyCode = Hist.CompanyCode AND
					HstITS.BranchCode = Hist.BranchCode AND
					HstITS.InquiryNumber = Hist.InquiryNumber 
				 WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE Hist.CompanyCode END) = @DealerCode
					AND (CASE WHEN @OutletCode = '' THEN '' ELSE Hist.BranchCode END) = @OutletCode	
					AND Hist.LastProgress = 'SPK'
					AND HstITS.LastProgress = 'SPK' -- penambahan 1 April 2014
					AND (CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)				
					 or (convert(varchar,Hist.UpdateDate,112) < @StartDate 
					and (Hist.LastProgress='P'   and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
															   where h.CompanyCode=Hist.CompanyCode
																 and h.BranchCode=Hist.BranchCode
																 and h.InquiryNumber=Hist.InquiryNumber
																 and h.LastProgress<>'P'
																 and convert(varchar,h.UpdateDate,112)<@StartDate)
					 or  Hist.LastProgress='HP'  and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
															   where h.CompanyCode=Hist.CompanyCode
																 and h.BranchCode=Hist.BranchCode
																 and h.InquiryNumber=Hist.InquiryNumber
																 and h.LastProgress not in ('P','HP')
																 and convert(varchar,h.UpdateDate,112)<@StartDate)
					 or Hist.LastProgress='SPK' and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
															   where h.CompanyCode=Hist.CompanyCode
																 and h.BranchCode=Hist.BranchCode
																 and h.InquiryNumber=Hist.InquiryNumber
																 and h.LastProgress not in ('P','HP','SPK')
																 and convert(varchar,h.UpdateDate,112)<@StartDate))))
																  and HstITS.LastProgress not in ('DO','DELIVERY','LOST')	 
																  AND CONVERT(VARCHAR, HstITS.LastUpdateStatus , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)		 
				 GROUP BY  Hist.CompanyCode, Hist.BranchCode, TipeKendaraan, Variant) #tSPK21

	SELECT * INTO #tSPK22 FROM(
		SELECT Hist.CompanyCode, Hist.BranchCode, ISNULL(TipeKendaraan, '[BLANK]') TipeKendaraan, ISNULL(Variant, '[BLANK]') Variant,
				COUNT(Hist.LastProgress) SPK
				FROM SuzukiR4..pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
				LEFT JOIN SuzukiR4..pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON 
					HstITS.CompanyCode = Hist.CompanyCode AND
					HstITS.BranchCode = Hist.BranchCode AND
					HstITS.InquiryNumber = Hist.InquiryNumber 
				 WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE Hist.CompanyCode END) = @DealerCode
					AND (CASE WHEN @OutletCode = '' THEN '' ELSE Hist.BranchCode END) = @OutletCode	
					AND Hist.LastProgress = 'SPK'
					AND HstITS.LastProgress = 'SPK' -- penambahan 1 April 2014
					AND (CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)				
					 or (convert(varchar,Hist.UpdateDate,112) < @StartDate 
					and (Hist.LastProgress='P'   and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
															   where h.CompanyCode=Hist.CompanyCode
																 and h.BranchCode=Hist.BranchCode
																 and h.InquiryNumber=Hist.InquiryNumber
																 and h.LastProgress<>'P'
																 and convert(varchar,h.UpdateDate,112)<@StartDate)
					 or  Hist.LastProgress='HP'  and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
															   where h.CompanyCode=Hist.CompanyCode
																 and h.BranchCode=Hist.BranchCode
																 and h.InquiryNumber=Hist.InquiryNumber
																 and h.LastProgress not in ('P','HP')
																 and convert(varchar,h.UpdateDate,112)<@StartDate)
					 or Hist.LastProgress='SPK' and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
															   where h.CompanyCode=Hist.CompanyCode
																 and h.BranchCode=Hist.BranchCode
																 and h.InquiryNumber=Hist.InquiryNumber
																 and h.LastProgress not in ('P','HP','SPK')
																 and convert(varchar,h.UpdateDate,112)<@StartDate))))	 
					AND CONVERT(VARCHAR, HstITS.LastUpdateStatus, 112) <= CONVERT(VARCHAR, @EndDate, 112)			 
					 and HstITS.LastProgress not in ('DO','DELIVERY','LOST')	 
				 GROUP BY  Hist.CompanyCode, Hist.BranchCode, TipeKendaraan, Variant) #tSPK22

	SELECT * INTO #tDO2 FROM(
	SELECT Hist.CompanyCode, Hist.BranchCode, TipeKendaraan, Variant, COUNT(Hist.LastProgress) DO
	FROM SuzukiR4..pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
	INNER JOIN SuzukiR4..pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON 
		HstITS.CompanyCode = Hist.CompanyCode AND
		HstITS.BranchCode = Hist.BranchCode AND
		HstITS.InquiryNumber = Hist.InquiryNumber 
	 WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE Hist.CompanyCode END) = @DealerCode
		AND (CASE WHEN @OutletCode = '' THEN '' ELSE Hist.BranchCode END) = @OutletCode	
		AND Hist.LastProgress = 'DO'
		AND HstITS.LastProgress = 'DO' -- penambahan 1 April 2014
		AND CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112) 
	GROUP BY Hist.CompanyCode, Hist.BranchCode, TipeKendaraan, Variant) #tDO2
		
	SELECT * INTO #tDELIVERY2 FROM(
	SELECT Hist.CompanyCode, Hist.BranchCode, TipeKendaraan, Variant, COUNT(Hist.LastProgress) DELIVERY
	FROM SuzukiR4..pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
	INNER JOIN SuzukiR4..pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON 
		HstITS.CompanyCode = Hist.CompanyCode AND
		HstITS.BranchCode = Hist.BranchCode AND
		HstITS.InquiryNumber = Hist.InquiryNumber 
	 WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE Hist.CompanyCode END) = @DealerCode
		AND (CASE WHEN @OutletCode = '' THEN '' ELSE Hist.BranchCode END) = @OutletCode	
		AND Hist.LastProgress = 'DELIVERY'
		AND HstITS.LastProgress = 'DELIVERY' -- penambahan 1 April 2014
		AND CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112) 
	GROUP BY Hist.CompanyCode, Hist.BranchCode, TipeKendaraan, Variant) #tDELIVERY2
		
	SELECT * INTO #tLOST2 FROM(
	SELECT Hist.CompanyCode, Hist.BranchCode, TipeKendaraan, Variant, COUNT(Hist.LastProgress) LOST  
	FROM SuzukiR4..pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
	INNER JOIN SuzukiR4..pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON 
		HstITS.CompanyCode = Hist.CompanyCode AND
		HstITS.BranchCode = Hist.BranchCode AND
		HstITS.InquiryNumber = Hist.InquiryNumber 
	 WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE Hist.CompanyCode END) = @DealerCode
		AND (CASE WHEN @OutletCode = '' THEN '' ELSE Hist.BranchCode END) = @OutletCode	
		AND Hist.LastProgress = 'LOST'
		AND HstITS.LastProgress = 'LOST' -- penambahan 1 April 2014
		AND CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112) 
	GROUP BY Hist.CompanyCode, Hist.BranchCode, TipeKendaraan, Variant) #tLOST2

	SELECT * INTO #Temp2 FROM(
	SELECT DISTINCT ISNULL(HstITS.TipeKendaraan, '') TipeKendaraan,
			ISNULL(HstITS.Variant,'') TypeCode,
			ISNULL(NEW.NEW, 0) NEW,		
			ISNULL(CASE WHEN @TypeReport = '0' THEN P1.P ELSE P2.P END, (CASE WHEN HstITS.TipeKendaraan IS NOT NULL THEN 0 ELSE (CASE WHEN @TypeReport = '0' THEN ISNULL((SELECT P FROM #tP21 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND TipeKendaraan = '[BLANK]'), 0) ELSE ISNULL((SELECT P FROM #tP22 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND TipeKendaraan = '[BLANK]'), 0) END) END)) P,		 
			ISNULL(CASE WHEN @TypeReport = '0' THEN HP1.HP ELSE HP2.HP END, (CASE WHEN HstITS.TipeKendaraan IS NOT NULL THEN 0 ELSE (CASE WHEN @TypeReport = '0' THEN ISNULL((SELECT HP FROM #tHP21 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND TipeKendaraan = '[BLANK]'),0) ELSE ISNULL((SELECT HP FROM #tHP22 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND TipeKendaraan = '[BLANK]'),0)  END) END)) HP,		
			ISNULL(CASE WHEN @TypeReport = '0' THEN SPK1.SPK ELSE SPK2.SPK END, (CASE WHEN HstITS.TipeKendaraan IS NOT NULL THEN 0 ELSE (CASE WHEN @TypeReport = '0' THEN ISNULL((SELECT SPK FROM #tSPK21 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND TipeKendaraan = '[BLANK]'),0) ELSE ISNULL((SELECT SPK FROM #tSPK22 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND TipeKendaraan = '[BLANK]'),0) END) END)) SPK,

			(ISNULL(CASE WHEN @TypeReport = '0' THEN P1.P ELSE P2.P END, (CASE WHEN HstITS.TipeKendaraan IS NOT NULL THEN 0 ELSE (CASE WHEN @TypeReport = '0' THEN ISNULL((SELECT P FROM #tP21 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND TipeKendaraan = '[BLANK]'), 0) ELSE ISNULL((SELECT P FROM #tP22 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND TipeKendaraan = '[BLANK]'), 0) END) END))
				+ISNULL(CASE WHEN @TypeReport = '0' THEN HP1.HP ELSE HP2.HP END, (CASE WHEN HstITS.TipeKendaraan IS NOT NULL THEN 0 ELSE (CASE WHEN @TypeReport = '0' THEN ISNULL((SELECT HP FROM #tHP21 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND TipeKendaraan = '[BLANK]'),0) ELSE ISNULL((SELECT HP FROM #tHP22 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND TipeKendaraan = '[BLANK]'),0) END) END))
				+ISNULL(CASE WHEN @TypeReport = '0' THEN SPK1.SPK ELSE SPK2.SPK END, (CASE WHEN HstITS.TipeKendaraan IS NOT NULL THEN 0 ELSE (CASE WHEN @TypeReport = '0' THEN ISNULL((SELECT SPK FROM #tSPK21 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND TipeKendaraan = '[BLANK]'),0) ELSE ISNULL((SELECT SPK FROM #tSPK22 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND TipeKendaraan = '[BLANK]'),0) END) END))) SumOuts,

			ISNULL(DO.DO, 0) DO, ISNULL(DELIVERY.DELIVERY, 0) DELIVERY, ISNULL(LOST.LOST, 0) LOST			
		FROM SuzukiR4..pmStatusHistory a WITH (NOLOCK, NOWAIT)
		LEFT JOIN SuzukiR4..pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON HstITS.CompanyCode = a.CompanyCode AND HstITS.BranchCode = a.BranchCode AND HstITS.InquiryNumber = a.InquiryNumber 
		LEFT JOIN SuzukiR4..gnMstDealerMapping b WITH (NOLOCK, NOWAIT) on a.CompanyCode = b.DealerCode	
		LEFT JOIN #tNEW2 NEW WITH (NOLOCK, NOWAIT) ON NEW.CompanyCode = a.CompanyCode AND NEW.BranchCode = a.BranchCode AND NEW.TipeKendaraan = HstITS.TipeKendaraan AND NEW.Variant = HstITS.Variant
		LEFT JOIN #tP21 P1 WITH (NOLOCK, NOWAIT) ON P1.CompanyCode = a.CompanyCode AND P1.BranchCode = a.BranchCode AND P1.TipeKendaraan = HstITS.TipeKendaraan AND P1.Variant = HstITS.Variant	
		LEFT JOIN #tP22 P2 WITH (NOLOCK, NOWAIT) ON P2.CompanyCode = a.CompanyCode AND P2.BranchCode = a.BranchCode AND P2.TipeKendaraan = HstITS.TipeKendaraan AND P2.Variant = HstITS.Variant				
		LEFT JOIN #tHP21 HP1 WITH (NOLOCK, NOWAIT) ON HP1.CompanyCode = a.CompanyCode AND HP1.BranchCode = a.BranchCode AND HP1.TipeKendaraan = HstITS.TipeKendaraan AND HP1.Variant = HstITS.Variant 			
		LEFT JOIN #tHP22 HP2 WITH (NOLOCK, NOWAIT) ON HP2.CompanyCode = a.CompanyCode AND HP2.BranchCode = a.BranchCode AND HP2.TipeKendaraan = HstITS.TipeKendaraan	AND HP2.Variant = HstITS.Variant 			
		LEFT JOIN #tSPK21 SPK1 WITH (NOLOCK, NOWAIT) ON SPK1.CompanyCode = a.CompanyCode AND SPK1.BranchCode = a.BranchCode AND SPK1.TipeKendaraan = HstITS.TipeKendaraan AND SPK1.Variant = HstITS.Variant 
		LEFT JOIN #tSPK22 SPK2 WITH (NOLOCK, NOWAIT) ON SPK2.CompanyCode = a.CompanyCode AND SPK2.BranchCode = a.BranchCode AND SPK2.TipeKendaraan = HstITS.TipeKendaraan AND SPK2.Variant = HstITS.Variant 	
		LEFT JOIN #tDO2 DO WITH (NOLOCK, NOWAIT) ON DO.CompanyCode = a.CompanyCode AND DO.BranchCode = a.BranchCode AND DO.TipeKendaraan = HstITS.TipeKendaraan AND DO.Variant = HstITS.Variant	 
		LEFT JOIN #tDELIVERY2 DELIVERY WITH (NOLOCK, NOWAIT) ON DELIVERY.CompanyCode = a.CompanyCode AND DELIVERY.BranchCode = a.BranchCode AND DELIVERY.TipeKendaraan = HstITS.TipeKendaraan AND DELIVERY.Variant = HstITS.Variant		
		LEFT JOIN #tLOST2 LOST WITH (NOLOCK, NOWAIT) ON LOST.CompanyCode = a.CompanyCode AND LOST.BranchCode = a.BranchCode AND LOST.TipeKendaraan = HstITS.TipeKendaraan AND LOST.Variant = HstITS.Variant						   					   		   			   		
		WHERE (CASE WHEN @Area = '' THEN '' ELSE b.Area END) = @Area
		AND (CASE WHEN @DealerCode = '' THEN '' ELSE a.CompanyCode END) = @DealerCode
		AND (CASE WHEN @OutletCode = '' THEN '' ELSE a.BranchCode END) = @OutletCode) #Temp2	

	SELECT 
	CASE WHEN @TypeReport = '0' THEN 'Summary' ELSE 'Saldo' END TypeReport,
		CASE @ProductivityBy 
			WHEN '0' THEN 'Salesman'
			WHEN '1' THEN 'Vehicle Type'
			WHEN '2' THEN 'Source Data'
		END ProductivityBy,
		CONVERT(VARCHAR, @EndDate, 105) PerDate, CASE WHEN @Area = '' THEN 'ALL' ELSE @Area END Area, CONVERT(VARCHAR, @StartDate, 105) + ' s/d ' + CONVERT(VARCHAR, @EndDate, 105) PeriodeDO,
		CASE WHEN @SalesHead = '' THEN 'ALL' ELSE @SalesHead END SalesHead,
		CASE WHEN @SalesCoordinator = '' THEN 'ALL' ELSE @SalesCoordinator END SalesCoordinator,
		CASE WHEN @Salesman = '' THEN 'ALL' ELSE @Salesman END Salesman,
		CASE WHEN @DealerCode = '' THEN 'ALL' ELSE (SELECT DealerName FROM SuzukiR4..gnMstDealerMapping WHERE DealerCode = @DealerCode and Area = @Area) END Dealer,
		CASE WHEN @OutletCode = '' THEN 'ALL' ELSE (SELECT OutletName FROM SuzukiR4..gnMstDealerOutletMapping WHERE DealerCode = @DealerCode AND OutletCode = @OutletCode) END Outlet,
		b.TipeKendaraan,
		ISNULL(SUM(b.P), 0) NEW,
		ISNULL(SUM(b.P), 0) P,
		ISNULL(SUM(b.HP), 0) HP,
		ISNULL(SUM(b.SPK), 0) SPK,
		ISNULL(SUM(b.SumOuts), 0) SumOuts,
		ISNULL(SUM(b.DO), 0) DO,
		ISNULL(SUM(b.DELIVERY), 0) DELIVERY,
		ISNULL(SUM(b.LOST), 0) LOST	
			
	--CASE CAST(ISNULL(b.NEW, 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(b.NEW AS VARCHAR) END NEW,
	--CASE CAST(ISNULL(b.P, 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(b.P AS VARCHAR) END P,
	--CASE CAST(ISNULL(b.HP, 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(b.HP AS VARCHAR) END HP,
	--CASE CAST(ISNULL(b.SPK, 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(b.SPK AS VARCHAR) END SPK,
	--CASE CAST(ISNULL(b.SumOuts, 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(b.SumOuts AS VARCHAR) END SumOuts,
	--CASE CAST(ISNULL(b.DO, 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(b.DO AS VARCHAR) END DO,
	--CASE CAST(ISNULL(b.DELIVERY, 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(b.DELIVERY AS VARCHAR) END DELIVERY,
	--CASE CAST(ISNULL(b.LOST, 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(b.LOST AS VARCHAR) END LOST
	FROM #Temp2 b 	
	GROUP BY b.TipeKendaraan
	
	DROP TABLE #tNEW2, #tP21, #tP22, #tHP21, #tHP22, #tSPK21, #tSPK22, #tDO2, #tDELIVERY2, #tLOST2, #Temp2	
	END					
	ELSE
	BEGIN
		SELECT DISTINCT
	CASE WHEN @TypeReport = '0' THEN 'Summary' ELSE 'Saldo' END TypeReport,
		CASE @ProductivityBy 
			WHEN '0' THEN 'Salesman'
			WHEN '1' THEN 'Vehicle Type'
			WHEN '2' THEN 'Source Data'
		END ProductivityBy,
		CONVERT(VARCHAR, @EndDate, 105) PerDate, @Area Area, CONVERT(VARCHAR, @StartDate, 105) + ' s/d ' + CONVERT(VARCHAR, @EndDate, 105) PeriodeDO,
		CASE WHEN @SalesHead = '' THEN 'ALL' ELSE (SELECT EmployeeName FROM SuzukiR4..GnMstEmployee WHERE CompanyCode = a.CompanyCode AND BranchCode = @OutletCode AND EmployeeID = @SalesHead) END SalesHead,
		CASE WHEN @SalesCoordinator = '' THEN 'ALL' ELSE (SELECT EmployeeName FROM SuzukiR4..GnMstEmployee WHERE CompanyCode = a.CompanyCode AND BranchCode = @OutletCode AND EmployeeID = @SalesCoordinator) END SalesCoordinator,
		CASE WHEN @Salesman = '' THEN 'ALL' ELSE (SELECT EmployeeName FROM SuzukiR4..GnMstEmployee WHERE CompanyCode = a.CompanyCode AND BranchCode = @OutletCode AND EmployeeID = @Salesman) END Salesman,
		CASE WHEN @DealerCode = '' THEN 'ALL' ELSE (SELECT DealerName FROM SuzukiR4..gnMstDealerMapping WHERE DealerCode = @DealerCode and Area = @Area) END Dealer,
		CASE WHEN @OutletCode = '' THEN 'ALL' ELSE (SELECT BranchName FROM SuzukiR4..gnMstOrganizationDtl WHERE CompanyCode = @DealerCode AND BranchCode = @OutletCode) END Outlet,
	(a.GroupCode + ' ' + a.TypeCode) TipeKendaraan,
	ISNULL(b.P, 0) NEW,
	ISNULL(b.P, 0) P,
	ISNULL(b.HP, 0) HP,
	ISNULL(b.SPK, 0) SPK,
	ISNULL(b.SumOuts, 0) SumOuts,
	ISNULL(b.DO, 0) DO,
	ISNULL(b.DELIVERY, 0) DELIVERY,
	ISNULL(b.LOST, 0) LOST	
			
	--CASE CAST(ISNULL(b.NEW, 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(b.NEW AS VARCHAR) END NEW,
	--CASE CAST(ISNULL(b.P, 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(b.P AS VARCHAR) END P,
	--CASE CAST(ISNULL(b.HP, 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(b.HP AS VARCHAR) END HP,
	--CASE CAST(ISNULL(b.SPK, 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(b.SPK AS VARCHAR) END SPK,
	--CASE CAST(ISNULL(b.SumOuts, 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(b.SumOuts AS VARCHAR) END SumOuts,
	--CASE CAST(ISNULL(b.DO, 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(b.DO AS VARCHAR) END DO,
	--CASE CAST(ISNULL(b.DELIVERY, 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(b.DELIVERY AS VARCHAR) END DELIVERY,
	--CASE CAST(ISNULL(b.LOST, 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(b.LOST AS VARCHAR) END LOST
	FROM SuzukiR4..pmGroupTypeSeq a
	LEFT JOIN 
	(
	SELECT a.TipeKendaraan, 
		   a.Variant TypeCode,
		   (SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
			WHERE CompanyCode = a.CompanyCode
				AND BranchCode = a.BranchCode
				AND TipeKendaraan = a.TipeKendaraan 
				AND Variant = a.Variant 
				AND CONVERT(VARCHAR, InquiryDate, 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)) NEW,
			(CASE WHEN @TypeReport = '0' THEN
				(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
				WHERE CompanyCode = a.CompanyCode
					AND BranchCode = a.BranchCode
					AND TipeKendaraan = a.TipeKendaraan  
					AND Variant = a.Variant 
					AND LastProgress = 'P'  
					AND CONVERT(VARCHAR, LastUpdateStatus, 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)) 
			ELSE		
				(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
				WHERE CompanyCode = a.CompanyCode
					AND BranchCode = a.BranchCode	
					AND TipeKendaraan = a.TipeKendaraan  
					AND Variant = a.Variant 
					AND LastProgress = 'P'  
					AND CONVERT(VARCHAR, LastUpdateStatus, 112) <= CONVERT(VARCHAR, @EndDate, 112)) END) P,	
									
			(CASE WHEN @TypeReport = '0' THEN			
				(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
				WHERE  CompanyCode = a.CompanyCode
					AND BranchCode = a.BranchCode		
					AND TipeKendaraan = a.TipeKendaraan  
					AND Variant = a.Variant 
					AND LastProgress = 'HP'  
					AND CONVERT(VARCHAR, LastUpdateStatus, 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112))
			ELSE	
				(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
				WHERE CompanyCode = a.CompanyCode
					AND BranchCode = a.BranchCode
					AND TipeKendaraan = a.TipeKendaraan  
					AND Variant = a.Variant 
					AND LastProgress = 'HP'  
					AND CONVERT(VARCHAR, LastUpdateStatus, 112) <= CONVERT(VARCHAR, @EndDate, 112)) END) HP,
					
			(CASE WHEN @TypeReport = '0' THEN
				(SELECT COUNT(Hist.LastProgress) FROM SuzukiR4..pmStatusHistory Hist
					INNER JOIN SuzukiR4..pmKdp KDP ON 
					KDP.CompanyCode = Hist.CompanyCode AND
					KDP.BranchCode = Hist.BranchCode AND
					KDP.InquiryNumber = Hist.InquiryNumber 
				 WHERE  Hist.CompanyCode = a.CompanyCode
					AND Hist.BranchCode = a.BranchCode
					AND KDP.TipeKendaraan = a.TipeKendaraan  
					AND KDP.Variant = a.Variant 				
					AND CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112) 
					AND Hist.LastProgress = 'SPK')
			ELSE
			(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
				WHERE CompanyCode = a.CompanyCode
					AND BranchCode = a.BranchCode
					AND TipeKendaraan = a.TipeKendaraan  
					AND Variant = a.Variant 	
					AND LastProgress = 'SPK'  
					AND CONVERT(VARCHAR, LastUpdateStatus, 112) <= CONVERT(VARCHAR, @EndDate, 112)) 
				END) SPK,
			
			(CASE WHEN @TypeReport = '0' THEN	
				(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
				WHERE CompanyCode = a.CompanyCode
					AND BranchCode = a.BranchCode
					AND TipeKendaraan = a.TipeKendaraan  
					AND Variant = a.Variant 
					AND LastProgress = 'P'  
					AND CONVERT(VARCHAR, LastUpdateStatus, 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)) +
				(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
				WHERE  CompanyCode = a.CompanyCode
					AND BranchCode = a.BranchCode		
					AND TipeKendaraan = a.TipeKendaraan  
					AND Variant = a.Variant 
					AND LastProgress = 'HP'  
					AND CONVERT(VARCHAR, LastUpdateStatus, 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)) +
				(SELECT COUNT(Hist.LastProgress) FROM SuzukiR4..pmStatusHistory Hist
					INNER JOIN SuzukiR4..pmKdp KDP ON 
					KDP.CompanyCode = Hist.CompanyCode AND
					KDP.BranchCode = Hist.BranchCode AND
					KDP.InquiryNumber = Hist.InquiryNumber 
				 WHERE Hist.CompanyCode = @DealerCode AND
					Hist.BranchCode = @OutletCode AND
					KDP.TipeKendaraan = a.TipeKendaraan AND 
					KDP.Variant = a.Variant AND				
					CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112) AND
					Hist.LastProgress = 'SPK')
			ELSE
				(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
				WHERE CompanyCode = a.CompanyCode
					AND BranchCode = a.BranchCode				
					AND TipeKendaraan = a.TipeKendaraan  
					AND Variant = a.Variant 
					AND LastProgress = 'P'  
					AND CONVERT(VARCHAR, LastUpdateStatus, 112) <= CONVERT(VARCHAR, @EndDate, 112)) +
				(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
				WHERE CompanyCode = a.CompanyCode
					AND BranchCode = a.BranchCode			
					AND TipeKendaraan = a.TipeKendaraan  
					AND Variant = a.Variant 
					AND LastProgress = 'HP'  
					AND CONVERT(VARCHAR, LastUpdateStatus, 112) <= CONVERT(VARCHAR, @EndDate, 112)) + 			
				(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
					WHERE CompanyCode = a.CompanyCode
					AND BranchCode = a.BranchCode	
					AND TipeKendaraan = a.TipeKendaraan  
					AND Variant = a.Variant 	
					AND LastProgress = 'SPK'  
					AND CONVERT(VARCHAR, LastUpdateStatus, 112) <= CONVERT(VARCHAR, @EndDate, 112)) 
					END) SumOuts,
					
			(SELECT COUNT(Hist.LastProgress) FROM SuzukiR4..pmStatusHistory Hist
			INNER JOIN SuzukiR4..pmKdp KDP ON 
				KDP.CompanyCode = Hist.CompanyCode AND
				KDP.BranchCode = Hist.BranchCode AND
				KDP.InquiryNumber = Hist.InquiryNumber 
			 WHERE Hist.CompanyCode = a.CompanyCode AND
				Hist.BranchCode = a.BranchCode AND
				KDP.TipeKendaraan = a.TipeKendaraan AND 
				KDP.Variant = a.Variant AND			
				CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112) AND
				Hist.LastProgress = 'DO') DO,
			(SELECT COUNT(Hist.LastProgress) FROM SuzukiR4..pmStatusHistory Hist
				INNER JOIN SuzukiR4..pmKdp KDP ON 
				KDP.CompanyCode = Hist.CompanyCode AND
				KDP.BranchCode = Hist.BranchCode AND
				KDP.InquiryNumber = Hist.InquiryNumber 
			 WHERE Hist.CompanyCode = a.CompanyCode AND
				Hist.BranchCode = a.BranchCode AND
				KDP.TipeKendaraan = a.TipeKendaraan AND 
				KDP.Variant = a.Variant AND		
				CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112) AND
				Hist.LastProgress = 'DELIVERY') DELIVERY,
			(SELECT COUNT(Hist.LastProgress) FROM SuzukiR4..pmStatusHistory Hist
				INNER JOIN SuzukiR4..pmKdp KDP ON 
				KDP.CompanyCode = Hist.CompanyCode AND
				KDP.BranchCode = Hist.BranchCode AND
				KDP.InquiryNumber = Hist.InquiryNumber 
			 WHERE Hist.CompanyCode = a.CompanyCode AND
				Hist.BranchCode = a.BranchCode AND
				KDP.TipeKendaraan = a.TipeKendaraan AND 
				KDP.Variant = a.Variant AND		
				CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112) AND
				Hist.LastProgress = 'LOST') LOST
	FROM SuzukiR4..pmKdp a
	WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE a.CompanyCode END) = @DealerCode
		AND (CASE WHEN @OutletCode = '' THEN '' ELSE a.BranchCode END) = @OutletCode
	GROUP BY a.CompanyCode, a.BranchCode, a.TipeKendaraan, a.Variant
	) b ON a.GroupCode = b.TipeKendaraan AND a.TypeCode = b.TypeCode
	WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE a.CompanyCode END) = @DealerCode
	END	
END	
--#endregion

--#region PRODUCTIVITY BY SOURCE DATA (2)	
ELSE IF @ProductivityBy = '2'
BEGIN
	SELECT * INTO #employee_src FROM (
		SELECT CompanyCode, LookUpValue
		FROM SuzukiR4..GnMstLookUpDtl 
		WHERE CodeID = 'PSRC'
	) #employee_src	
				
	IF @National = '1' 
	BEGIN	
		SELECT * INTO #employee_src_2_n FROM
		(
			SELECT DISTINCT Saleshead
			FROM SuzukiR4..pmHstITS a
			LEFT JOIN SuzukiR4..gnMstDealerMapping b on a.CompanyCode = b.DealerCode	
			WHERE (CASE WHEN @Area = '' THEN '' ELSE b.Area END) = @Area				
				  AND (CASE WHEN @DealerCode = '' THEN '' ELSE b.DealerCode END) = @DealerCode
				  AND (CASE WHEN @OutletCode = '' THEN '' ELSE a.BranchCode END) = @OutletCode	
		) #employee_src_2_n
		
SELECT * INTO #employee_src_3_n FROM
		(
			SELECT DISTINCT SalesCoordinator
				FROM SuzukiR4..pmHstITS a
				LEFT JOIN SuzukiR4..gnMstDealerMapping b on a.CompanyCode = b.DealerCode	
				WHERE (CASE WHEN @Area = '' THEN '' ELSE b.Area END) = @Area				
					  AND (CASE WHEN @DealerCode = '' THEN '' ELSE b.DealerCode END) = @DealerCode
					  AND (CASE WHEN @OutletCode = '' THEN '' ELSE a.BranchCode END) = @OutletCode	
					  AND SalesHead in (SELECT SalesHead FROM #employee_src_2_n)
		)#employee_src_3_n

SELECT * INTO #tNEW3 FROM (
	SELECT CompanyCode, BranchCode, PerolehanData, SalesCoordinator, COUNT(LastProgress) NEW
	FROM SuzukiR4..pmHstITS WITH (NOLOCK, NOWAIT)
	WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE CompanyCode END) = @DealerCode
		AND (CASE WHEN @OutletCode = '' THEN '' ELSE BranchCode END) = @OutletCode	 
		AND CONVERT(VARCHAR, InquiryDate, 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)
	GROUP BY CompanyCode, BranchCode, PerolehanData, SalesCoordinator
) #tNEW3							  		

SELECT * INTO #tP31 FROM (
		SELECT Hist.CompanyCode, Hist.BranchCode,  ISNULL(PerolehanData, '[BLANK]') PerolehanData, ISNULL(SalesCoordinator, '[BLANK]') SalesCoordinator,
		COUNT(Hist.LastProgress) P
		FROM SuzukiR4..pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
		LEFT JOIN SuzukiR4..pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON 
		HstITS.CompanyCode = Hist.CompanyCode AND
		HstITS.BranchCode = Hist.BranchCode AND
		HstITS.InquiryNumber = Hist.InquiryNumber 
		WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE Hist.CompanyCode END) = @DealerCode
		AND (CASE WHEN @OutletCode = '' THEN '' ELSE Hist.BranchCode END) = @OutletCode	
		AND Hist.LastProgress = 'P'
		AND HstITS.LastProgress = 'P' -- penambahan 1 April 2014
		AND (CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)				
		 or (convert(varchar,Hist.UpdateDate,112) < @StartDate 
		and (Hist.LastProgress='P'   and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
												   where h.CompanyCode=Hist.CompanyCode
													 and h.BranchCode=Hist.BranchCode
													 and h.InquiryNumber=Hist.InquiryNumber
													 and h.LastProgress<>'P'
													 and convert(varchar,h.UpdateDate,112)<@StartDate)
		 or  Hist.LastProgress='HP'  and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
												   where h.CompanyCode=Hist.CompanyCode
													 and h.BranchCode=Hist.BranchCode
													 and h.InquiryNumber=Hist.InquiryNumber
													 and h.LastProgress not in ('P','HP')
													 and convert(varchar,h.UpdateDate,112)<@StartDate)
		 or Hist.LastProgress='SPK' and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
												   where h.CompanyCode=Hist.CompanyCode
													 and h.BranchCode=Hist.BranchCode
													 and h.InquiryNumber=Hist.InquiryNumber
													 and h.LastProgress not in ('P','HP','SPK')
													 and convert(varchar,h.UpdateDate,112)<@StartDate))))	
													  and HstITS.LastProgress not in ('DO','DELIVERY','LOST')		
													  AND CONVERT(VARCHAR, HstITS.LastUpdateStatus , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)	 
		GROUP BY  Hist.CompanyCode, Hist.BranchCode, PerolehanData, SalesCoordinator) #tP31	

SELECT * INTO #tP32 FROM(
		SELECT Hist.CompanyCode, Hist.BranchCode,  ISNULL(PerolehanData, '[BLANK]') PerolehanData, ISNULL(SalesCoordinator, '[BLANK]') SalesCoordinator,
		COUNT(Hist.LastProgress) P
		FROM SuzukiR4..pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
		LEFT JOIN SuzukiR4..pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON 
		HstITS.CompanyCode = Hist.CompanyCode AND
		HstITS.BranchCode = Hist.BranchCode AND
		HstITS.InquiryNumber = Hist.InquiryNumber 
		WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE Hist.CompanyCode END) = @DealerCode
		AND (CASE WHEN @OutletCode = '' THEN '' ELSE Hist.BranchCode END) = @OutletCode	
		AND Hist.LastProgress = 'P'
		AND HstITS.LastProgress = 'P' -- penambahan 1 April 2014
		AND (CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)				
		 or (convert(varchar,Hist.UpdateDate,112) < @StartDate 
		and (Hist.LastProgress='P'   and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
												   where h.CompanyCode=Hist.CompanyCode
													 and h.BranchCode=Hist.BranchCode
													 and h.InquiryNumber=Hist.InquiryNumber
													 and h.LastProgress<>'P'
													 and convert(varchar,h.UpdateDate,112)<@StartDate)
		 or  Hist.LastProgress='HP'  and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
												   where h.CompanyCode=Hist.CompanyCode
													 and h.BranchCode=Hist.BranchCode
													 and h.InquiryNumber=Hist.InquiryNumber
													 and h.LastProgress not in ('P','HP')
													 and convert(varchar,h.UpdateDate,112)<@StartDate)
		 or Hist.LastProgress='SPK' and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
												   where h.CompanyCode=Hist.CompanyCode
													 and h.BranchCode=Hist.BranchCode
													 and h.InquiryNumber=Hist.InquiryNumber
													 and h.LastProgress not in ('P','HP','SPK')
													 and convert(varchar,h.UpdateDate,112)<@StartDate))))	 
		AND CONVERT(VARCHAR, HstITS.LastUpdateStatus, 112) <= CONVERT(VARCHAR, @EndDate, 112)				
		 and HstITS.LastProgress not in ('DO','DELIVERY','LOST')								
		GROUP BY  Hist.CompanyCode, Hist.BranchCode, PerolehanData, SalesCoordinator) #tP32

SELECT * INTO #tHP31 FROM(
		SELECT Hist.CompanyCode, Hist.BranchCode, ISNULL(PerolehanData, '[BLANK]') PerolehanData, ISNULL(SalesCoordinator, '[BLANK]') SalesCoordinator,
				COUNT(Hist.LastProgress) HP
				FROM SuzukiR4..pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
				LEFT JOIN SuzukiR4..pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON 
					HstITS.CompanyCode = Hist.CompanyCode AND
					HstITS.BranchCode = Hist.BranchCode AND
					HstITS.InquiryNumber = Hist.InquiryNumber 
				 WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE Hist.CompanyCode END) = @DealerCode
					AND (CASE WHEN @OutletCode = '' THEN '' ELSE Hist.BranchCode END) = @OutletCode	
					AND Hist.LastProgress = 'HP'
					AND HstITS.LastProgress = 'HP' -- penambahan 1 April 2014
					AND (CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)				
					 or (convert(varchar,Hist.UpdateDate,112) < @StartDate 
					and (Hist.LastProgress='P'   and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
															   where h.CompanyCode=Hist.CompanyCode
																 and h.BranchCode=Hist.BranchCode
																 and h.InquiryNumber=Hist.InquiryNumber
																 and h.LastProgress<>'P'
																 and convert(varchar,h.UpdateDate,112)<@StartDate)
					 or  Hist.LastProgress='HP'  and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
															   where h.CompanyCode=Hist.CompanyCode
																 and h.BranchCode=Hist.BranchCode
																 and h.InquiryNumber=Hist.InquiryNumber
																 and h.LastProgress not in ('P','HP')
																 and convert(varchar,h.UpdateDate,112)<@StartDate)
					 or Hist.LastProgress='SPK' and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
															   where h.CompanyCode=Hist.CompanyCode
																 and h.BranchCode=Hist.BranchCode
																 and h.InquiryNumber=Hist.InquiryNumber
																 and h.LastProgress not in ('P','HP','SPK')
																 and convert(varchar,h.UpdateDate,112)<@StartDate))))	 
																  and HstITS.LastProgress not in ('DO','DELIVERY','LOST')		
																  AND CONVERT(VARCHAR, HstITS.LastUpdateStatus , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)	
				 GROUP BY  Hist.CompanyCode, Hist.BranchCode, PerolehanData, SalesCoordinator) #tHP31

SELECT * INTO #tHP32 FROM(
		SELECT Hist.CompanyCode, Hist.BranchCode, ISNULL(PerolehanData, '[BLANK]') PerolehanData, ISNULL(SalesCoordinator, '[BLANK]') SalesCoordinator,
				COUNT(Hist.LastProgress) HP
				FROM SuzukiR4..pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
				LEFT JOIN SuzukiR4..pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON 
					HstITS.CompanyCode = Hist.CompanyCode AND
					HstITS.BranchCode = Hist.BranchCode AND
					HstITS.InquiryNumber = Hist.InquiryNumber 
				 WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE Hist.CompanyCode END) = @DealerCode
					AND (CASE WHEN @OutletCode = '' THEN '' ELSE Hist.BranchCode END) = @OutletCode	
					AND Hist.LastProgress = 'HP'
					AND HstITS.LastProgress = 'HP' -- penambahan 1 April 2014
					AND (CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)				
					 or (convert(varchar,Hist.UpdateDate,112) < @StartDate 
					and (Hist.LastProgress='P'   and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
															   where h.CompanyCode=Hist.CompanyCode
																 and h.BranchCode=Hist.BranchCode
																 and h.InquiryNumber=Hist.InquiryNumber
																 and h.LastProgress<>'P'
																 and convert(varchar,h.UpdateDate,112)<@StartDate)
					 or  Hist.LastProgress='HP'  and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
															   where h.CompanyCode=Hist.CompanyCode
																 and h.BranchCode=Hist.BranchCode
																 and h.InquiryNumber=Hist.InquiryNumber
																 and h.LastProgress not in ('P','HP')
																 and convert(varchar,h.UpdateDate,112)<@StartDate)
					 or Hist.LastProgress='SPK' and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
															   where h.CompanyCode=Hist.CompanyCode
																 and h.BranchCode=Hist.BranchCode
																 and h.InquiryNumber=Hist.InquiryNumber
																 and h.LastProgress not in ('P','HP','SPK')
																 and convert(varchar,h.UpdateDate,112)<@StartDate))))	 
				 AND CONVERT(VARCHAR, HstITS.LastUpdateStatus, 112) <= CONVERT(VARCHAR, @EndDate, 112)					
				  and HstITS.LastProgress not in ('DO','DELIVERY','LOST')												 
				 GROUP BY  Hist.CompanyCode, Hist.BranchCode, PerolehanData, SalesCoordinator) #tHP32				 				 

	SELECT * INTO #tSPK31 FROM(
	SELECT Hist.CompanyCode, Hist.BranchCode, ISNULL(PerolehanData, '[BLANK]') PerolehanData, ISNULL(SalesCoordinator, '[BLANK]') SalesCoordinator,
				COUNT(Hist.LastProgress) SPK
				FROM SuzukiR4..pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
				LEFT JOIN SuzukiR4..pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON 
					HstITS.CompanyCode = Hist.CompanyCode AND
					HstITS.BranchCode = Hist.BranchCode AND
					HstITS.InquiryNumber = Hist.InquiryNumber 
				 WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE Hist.CompanyCode END) = @DealerCode
					AND (CASE WHEN @OutletCode = '' THEN '' ELSE Hist.BranchCode END) = @OutletCode	
					AND Hist.LastProgress = 'SPK'
					AND HstITS.LastProgress = 'SPK' -- penambahan 1 April 2014
					AND (CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)				
					 or (convert(varchar,Hist.UpdateDate,112) < @StartDate 
					and (Hist.LastProgress='P'   and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
															   where h.CompanyCode=Hist.CompanyCode
																 and h.BranchCode=Hist.BranchCode
																 and h.InquiryNumber=Hist.InquiryNumber
																 and h.LastProgress<>'P'
																 and convert(varchar,h.UpdateDate,112)<@StartDate)
					 or  Hist.LastProgress='HP'  and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
															   where h.CompanyCode=Hist.CompanyCode
																 and h.BranchCode=Hist.BranchCode
																 and h.InquiryNumber=Hist.InquiryNumber
																 and h.LastProgress not in ('P','HP')
																 and convert(varchar,h.UpdateDate,112)<@StartDate)
					 or Hist.LastProgress='SPK' and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
															   where h.CompanyCode=Hist.CompanyCode
																 and h.BranchCode=Hist.BranchCode
																 and h.InquiryNumber=Hist.InquiryNumber
																 and h.LastProgress not in ('P','HP','SPK')
																 and convert(varchar,h.UpdateDate,112)<@StartDate))))	 
																  and HstITS.LastProgress not in ('DO','DELIVERY','LOST')
																  AND CONVERT(VARCHAR, HstITS.LastUpdateStatus , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)		 
				 GROUP BY  Hist.CompanyCode, Hist.BranchCode, PerolehanData, SalesCoordinator) #tSPK31				 


	SELECT * INTO #tSPK32 FROM(
		SELECT Hist.CompanyCode, Hist.BranchCode, ISNULL(PerolehanData, '[BLANK]') PerolehanData, ISNULL(SalesCoordinator, '[BLANK]') SalesCoordinator,
				COUNT(Hist.LastProgress) SPK
				FROM SuzukiR4..pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
				LEFT JOIN SuzukiR4..pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON 
					HstITS.CompanyCode = Hist.CompanyCode AND
					HstITS.BranchCode = Hist.BranchCode AND
					HstITS.InquiryNumber = Hist.InquiryNumber 
				 WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE Hist.CompanyCode END) = @DealerCode
					AND (CASE WHEN @OutletCode = '' THEN '' ELSE Hist.BranchCode END) = @OutletCode	
					AND Hist.LastProgress = 'SPK'
					AND HstITS.LastProgress = 'SPK' -- penambahan 1 April 2014
					AND (CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)				
					 or (convert(varchar,Hist.UpdateDate,112) < @StartDate 
					and (Hist.LastProgress='P'   and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
															   where h.CompanyCode=Hist.CompanyCode
																 and h.BranchCode=Hist.BranchCode
																 and h.InquiryNumber=Hist.InquiryNumber
																 and h.LastProgress<>'P'
																 and convert(varchar,h.UpdateDate,112)<@StartDate)
					 or  Hist.LastProgress='HP'  and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
															   where h.CompanyCode=Hist.CompanyCode
																 and h.BranchCode=Hist.BranchCode
																 and h.InquiryNumber=Hist.InquiryNumber
																 and h.LastProgress not in ('P','HP')
																 and convert(varchar,h.UpdateDate,112)<@StartDate)
					 or Hist.LastProgress='SPK' and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
															   where h.CompanyCode=Hist.CompanyCode
																 and h.BranchCode=Hist.BranchCode
																 and h.InquiryNumber=Hist.InquiryNumber
																 and h.LastProgress not in ('P','HP','SPK')
																 and convert(varchar,h.UpdateDate,112)<@StartDate))))	 
					AND CONVERT(VARCHAR, HstITS.LastUpdateStatus, 112) <= CONVERT(VARCHAR, @EndDate, 112)			 
					 and HstITS.LastProgress not in ('DO','DELIVERY','LOST')	 
				 GROUP BY  Hist.CompanyCode, Hist.BranchCode, PerolehanData, SalesCoordinator) #tSPK32
				 

SELECT * INTO #tDO3 FROM(
SELECT Hist.CompanyCode, Hist.BranchCode, hstITS.PerolehanData, hstITS.SalesCoordinator, COUNT(Hist.LastProgress) DO
FROM SuzukiR4..pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
INNER JOIN SuzukiR4..pmHstITS hstITS WITH (NOLOCK, NOWAIT) ON 
	hstITS.CompanyCode = Hist.CompanyCode AND
	hstITS.BranchCode = Hist.BranchCode AND
	hstITS.InquiryNumber = Hist.InquiryNumber 
 WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE Hist.CompanyCode END) = @DealerCode
		AND (CASE WHEN @OutletCode = '' THEN '' ELSE Hist.BranchCode END) = @OutletCode	 	
		AND CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112) 
		AND Hist.LastProgress = 'DO'
		AND HstITS.LastProgress = 'DO' -- penambahan 1 April 2014
	GROUP BY Hist.CompanyCode, Hist.BranchCode, hstITS.PerolehanData, hstITS.SalesCoordinator) #tDO3

SELECT * INTO #tDELIVERY3 FROM(
SELECT Hist.CompanyCode, Hist.BranchCode, hstITS.PerolehanData, hstITS.SalesCoordinator, COUNT(Hist.LastProgress) DELIVERY
FROM SuzukiR4..pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
INNER JOIN SuzukiR4..pmHstITS hstITS WITH (NOLOCK, NOWAIT) ON 
	hstITS.CompanyCode = Hist.CompanyCode AND
	hstITS.BranchCode = Hist.BranchCode AND
	hstITS.InquiryNumber = Hist.InquiryNumber 
 WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE Hist.CompanyCode END) = @DealerCode
		AND (CASE WHEN @OutletCode = '' THEN '' ELSE Hist.BranchCode END) = @OutletCode	 	
		AND CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112) 
		AND Hist.LastProgress = 'DELIVERY'
		AND HstITS.LastProgress = 'DELIVERY' -- penambahan 1 April 2014
	GROUP BY Hist.CompanyCode, Hist.BranchCode, hstITS.PerolehanData, hstITS.SalesCoordinator) #tDELIVERY3

SELECT * INTO #tLOST3 FROM(
SELECT Hist.CompanyCode, Hist.BranchCode, hstITS.PerolehanData, hstITS.SalesCoordinator, COUNT(Hist.LastProgress) LOST
FROM SuzukiR4..pmStatusHistory Hist WITH (NOLOCK, NOWAIT)
INNER JOIN SuzukiR4..pmHstITS hstITS WITH (NOLOCK, NOWAIT) ON 
	hstITS.CompanyCode = Hist.CompanyCode AND
	hstITS.BranchCode = Hist.BranchCode AND
	hstITS.InquiryNumber = Hist.InquiryNumber 
 WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE Hist.CompanyCode END) = @DealerCode
		AND (CASE WHEN @OutletCode = '' THEN '' ELSE Hist.BranchCode END) = @OutletCode	 	
		AND CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112) 
		AND Hist.LastProgress = 'LOST'
		AND HstITS.LastProgress = 'LOST' -- penambahan 1 April 2014
	GROUP BY Hist.CompanyCode, Hist.BranchCode, hstITS.PerolehanData, hstITS.SalesCoordinator) #tLOST3

SELECT * INTO #Temp3 FROM
(
SELECT DISTINCT HstITS.SalesCoordinator, ISNULL(HstITS.PerolehanData, '') PerolehanData,			
			ISNULL(NEW.NEW, 0) NEW,		
						
			ISNULL(CASE WHEN @TypeReport = '0' THEN P1.P ELSE P2.P END, (CASE WHEN HstITS.PerolehanData IS NOT NULL THEN 0 ELSE (CASE WHEN @TypeReport = '0' THEN ISNULL((SELECT P FROM #tP31 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND PerolehanData = '[BLANK]'), 0) ELSE ISNULL((SELECT P FROM #tP32 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND PerolehanData = '[BLANK]'), 0) END) END)) P,		 
			ISNULL(CASE WHEN @TypeReport = '0' THEN HP1.HP ELSE HP2.HP END, (CASE WHEN HstITS.PerolehanData IS NOT NULL THEN 0 ELSE (CASE WHEN @TypeReport = '0' THEN ISNULL((SELECT HP FROM #tHP31 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND PerolehanData = '[BLANK]'),0) ELSE ISNULL((SELECT HP FROM #tHP32 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND PerolehanData = '[BLANK]'),0)  END) END)) HP,		
			ISNULL(CASE WHEN @TypeReport = '0' THEN SPK1.SPK ELSE SPK2.SPK END, (CASE WHEN HstITS.PerolehanData IS NOT NULL THEN 0 ELSE (CASE WHEN @TypeReport = '0' THEN ISNULL((SELECT SPK FROM #tSPK31 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND PerolehanData = '[BLANK]'),0) ELSE ISNULL((SELECT SPK FROM #tSPK32 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND PerolehanData = '[BLANK]'),0) END) END)) SPK,

			(ISNULL(CASE WHEN @TypeReport = '0' THEN P1.P ELSE P2.P END, (CASE WHEN HstITS.PerolehanData IS NOT NULL THEN 0 ELSE (CASE WHEN @TypeReport = '0' THEN ISNULL((SELECT P FROM #tP31 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND PerolehanData = '[BLANK]'), 0) ELSE ISNULL((SELECT P FROM #tP32 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND PerolehanData = '[BLANK]'), 0) END) END))
				+ISNULL(CASE WHEN @TypeReport = '0' THEN HP1.HP ELSE HP2.HP END, (CASE WHEN HstITS.PerolehanData IS NOT NULL THEN 0 ELSE (CASE WHEN @TypeReport = '0' THEN ISNULL((SELECT HP FROM #tHP31 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND PerolehanData = '[BLANK]'),0) ELSE ISNULL((SELECT HP FROM #tHP32 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND PerolehanData = '[BLANK]'),0) END) END))
				+ISNULL(CASE WHEN @TypeReport = '0' THEN SPK1.SPK ELSE SPK2.SPK END, (CASE WHEN HstITS.PerolehanData IS NOT NULL THEN 0 ELSE (CASE WHEN @TypeReport = '0' THEN ISNULL((SELECT SPK FROM #tSPK31 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND PerolehanData = '[BLANK]'),0) ELSE ISNULL((SELECT SPK FROM #tSPK32 WITH(NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND PerolehanData = '[BLANK]'),0) END) END))) SumOuts,

			ISNULL(DO.DO, 0) DO, ISNULL(DELIVERY.DELIVERY, 0) DELIVERY, ISNULL(LOST.LOST, 0) LOST			
		FROM SuzukiR4..pmStatusHistory a WITH (NOLOCK, NOWAIT)
		LEFT JOIN SuzukiR4..pmHstITS HstITS WITH (NOLOCK, NOWAIT) ON HstITS.CompanyCode = a.CompanyCode AND HstITS.BranchCode = a.BranchCode AND HstITS.InquiryNumber = a.InquiryNumber 
		LEFT JOIN SuzukiR4..gnMstDealerMapping b WITH (NOLOCK, NOWAIT) on a.CompanyCode = b.DealerCode	
		LEFT JOIN #tNEW3 NEW WITH (NOLOCK, NOWAIT) ON NEW.CompanyCode = a.CompanyCode AND NEW.BranchCode = a.BranchCode AND NEW.PerolehanData = HstITS.PerolehanData AND NEW.SalesCoordinator = HstITS.SalesCoordinator
		LEFT JOIN #tP31 P1 WITH (NOLOCK, NOWAIT) ON P1.CompanyCode = a.CompanyCode AND P1.BranchCode = a.BranchCode AND P1.PerolehanData = HstITS.PerolehanData AND P1.SalesCoordinator = HstITS.SalesCoordinator	
		LEFT JOIN #tP32 P2 WITH (NOLOCK, NOWAIT) ON P2.CompanyCode = a.CompanyCode AND P2.BranchCode = a.BranchCode AND P2.PerolehanData = HstITS.PerolehanData AND P2.SalesCoordinator = HstITS.SalesCoordinator				
		LEFT JOIN #tHP31 HP1 WITH (NOLOCK, NOWAIT) ON HP1.CompanyCode = a.CompanyCode AND HP1.BranchCode = a.BranchCode AND HP1.PerolehanData = HstITS.PerolehanData AND HP1.SalesCoordinator = HstITS.SalesCoordinator 			
		LEFT JOIN #tHP32 HP2 WITH (NOLOCK, NOWAIT) ON HP2.CompanyCode = a.CompanyCode AND HP2.BranchCode = a.BranchCode AND HP2.PerolehanData = HstITS.PerolehanData	AND HP2.SalesCoordinator = HstITS.SalesCoordinator 			
		LEFT JOIN #tSPK31 SPK1 WITH (NOLOCK, NOWAIT) ON SPK1.CompanyCode = a.CompanyCode AND SPK1.BranchCode = a.BranchCode AND SPK1.PerolehanData = HstITS.PerolehanData AND SPK1.SalesCoordinator = HstITS.SalesCoordinator 
		LEFT JOIN #tSPK32 SPK2 WITH (NOLOCK, NOWAIT) ON SPK2.CompanyCode = a.CompanyCode AND SPK2.BranchCode = a.BranchCode AND SPK2.PerolehanData = HstITS.PerolehanData AND SPK2.SalesCoordinator = HstITS.SalesCoordinator 	
		LEFT JOIN #tDO3 DO WITH (NOLOCK, NOWAIT) ON DO.CompanyCode = a.CompanyCode AND DO.BranchCode = a.BranchCode AND DO.PerolehanData = HstITS.PerolehanData AND DO.SalesCoordinator = HstITS.SalesCoordinator	 
		LEFT JOIN #tDELIVERY3 DELIVERY WITH (NOLOCK, NOWAIT) ON DELIVERY.CompanyCode = a.CompanyCode AND DELIVERY.BranchCode = a.BranchCode AND DELIVERY.PerolehanData = HstITS.PerolehanData AND DELIVERY.SalesCoordinator = HstITS.SalesCoordinator		
		LEFT JOIN #tLOST3 LOST WITH (NOLOCK, NOWAIT) ON LOST.CompanyCode = a.CompanyCode AND LOST.BranchCode = a.BranchCode AND LOST.PerolehanData = HstITS.PerolehanData AND LOST.SalesCoordinator = HstITS.SalesCoordinator						   					   		   			   		
		WHERE (CASE WHEN @Area = '' THEN '' ELSE b.Area END) = @Area
		AND (CASE WHEN @DealerCode = '' THEN '' ELSE a.CompanyCode END) = @DealerCode
		AND (CASE WHEN @OutletCode = '' THEN '' ELSE a.BranchCode END) = @OutletCode) #Temp3						
								
SELECT 
		CASE WHEN @TypeReport = '0' THEN 'Summary' ELSE 'Saldo' END TypeReport,
		CASE @ProductivityBy 
			WHEN '0' THEN 'Salesman'
			WHEN '1' THEN 'Vehicle Type'
			WHEN '2' THEN 'Source Data'
		END ProductivityBy,
		CONVERT(VARCHAR, @EndDate, 105) PerDate, CASE WHEN @Area = '' THEN 'ALL' ELSE @Area END Area, 
		CONVERT(VARCHAR, @StartDate, 105) + ' s/d ' + CONVERT(VARCHAR, @EndDate, 105) PeriodeDO,
		CASE WHEN @SalesHead = '' THEN 'ALL' ELSE @SalesHead END SalesHead,
		CASE WHEN @SalesCoordinator = '' THEN 'ALL' ELSE @SalesCoordinator END SalesCoordinator,
		CASE WHEN @Salesman = '' THEN 'ALL' ELSE @Salesman END Salesman,
		CASE WHEN @DealerCode = '' THEN 'ALL' ELSE (SELECT DealerName FROM SuzukiR4..gnMstDealerMapping WHERE DealerCode = @DealerCode and Area = @Area) END Dealer,
		CASE WHEN @OutletCode = '' THEN 'ALL' ELSE (SELECT OutletName FROM SuzukiR4..gnMstDealerOutletMapping WHERE DealerCode = @DealerCode AND OutletCode = @OutletCode) END Outlet,
		PerolehanData Source,
		
		ISNULL(SUM(P), 0) NEW,
		ISNULL(SUM(P), 0) P,
		ISNULL(SUM(HP), 0) HP,
		ISNULL(SUM(SPK), 0) SPK,
		ISNULL(SUM(SumOuts), 0)  SumOuts,
		ISNULL(SUM(DO), 0) DO,
		ISNULL(SUM(DELIVERY), 0) DELIVERY,
		ISNULL(SUM(LOST), 0) LOST
		
		--CASE CAST(ISNULL(SUM(NEW), 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(SUM(NEW) AS VARCHAR) END NEW,
		--CASE CAST(ISNULL(SUM(P), 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(SUM(P) AS VARCHAR) END P,
		--CASE CAST(ISNULL(SUM(HP), 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(SUM(HP) AS VARCHAR) END HP,
		--CASE CAST(ISNULL(SUM(SPK), 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(SUM(SPK) AS VARCHAR) END SPK,
		--CASE CAST(ISNULL(SUM(SumOuts), 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(SUM(SumOuts) AS VARCHAR) END SumOuts,
		--CASE CAST(ISNULL(SUM(DO), 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(SUM(DO) AS VARCHAR) END DO,
		--CASE CAST(ISNULL(SUM(DELIVERY), 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(SUM(DELIVERY) AS VARCHAR) END DELIVERY,
		--CASE CAST(ISNULL(SUM(LOST), 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(SUM(LOST) AS VARCHAR) END LOST
	FROM #Temp3 a
	GROUP BY PerolehanData
						  		
	DROP TABLE #employee_src_2_n, #employee_src_3_n, #employee_src, #tNEW3, #tP31, #tP32, #tHP31, #tHP32, #tSPK31, #tSPK32, #tDO3, #tDELIVERY3, #tLOST3, #Temp3
	END
	ELSE
	BEGIN
		SELECT * INTO #employee_src_2 FROM
		(
			SELECT EmployeeID FROM SuzukiR4..PmMstTeamMembers WHERE 
				(CASE WHEN @DealerCode = '' THEN '' ELSE CompanyCode END) = @DealerCode 
				AND (CASE WHEN @OutletCode = '' THEN '' ELSE BranchCode END) = @OutletCode
				AND TeamID IN (SELECT TeamID FROM SuzukiR4..PmMstTeamMembers WHERE 
								(CASE WHEN @DealerCode = '' THEN '' ELSE CompanyCode END) = @DealerCode 
								AND (CASE WHEN @OutletCode = '' THEN '' ELSE BranchCode END) = @OutletCode
								AND EmployeeID = @BranchHead) 
				AND EmployeeID <> @BranchHead		
		) #employee_src_2
		
		SELECT * INTO #employee_src_3 FROM
		(
			SELECT EmployeeID FROM SuzukiR4..PmMstTeamMembers WHERE 
				(CASE WHEN @DealerCode = '' THEN '' ELSE CompanyCode END) = @DealerCode 
				AND (CASE WHEN @OutletCode = '' THEN '' ELSE BranchCode END) = @OutletCode
				AND TeamID IN (SELECT TeamID FROM SuzukiR4..PmMstTeamMembers WHERE 
								(CASE WHEN @DealerCode = '' THEN '' ELSE CompanyCode END) = @DealerCode 
								AND (CASE WHEN @OutletCode = '' THEN '' ELSE BranchCode END) = @OutletCode
								AND EmployeeID IN (SELECT EmployeeID FROM #employee_src_2) 
								AND IsSupervisor = 1) 
				AND EmployeeID NOT IN (SELECT EmployeeID FROM #employee_src_2)
		 )#employee_src_3
			
			SELECT 
		CASE WHEN @TypeReport = '0' THEN 'Summary' ELSE 'Saldo' END TypeReport,
		CASE @ProductivityBy 
			WHEN '0' THEN 'Salesman'
			WHEN '1' THEN 'Vehicle Type'
			WHEN '2' THEN 'Source Data'
		END ProductivityBy,
		CONVERT(VARCHAR, @EndDate, 105) PerDate, @Area Area, CONVERT(VARCHAR, @StartDate, 105) + ' s/d ' + CONVERT(VARCHAR, @EndDate, 105) PeriodeDO,
		CASE WHEN @SalesHead = '' THEN 'ALL' ELSE (SELECT EmployeeName FROM SuzukiR4..GnMstEmployee WHERE CompanyCode = a.CompanyCode AND BranchCode = @OutletCode AND EmployeeID = @SalesHead) END SalesHead,
		CASE WHEN @SalesCoordinator = '' THEN 'ALL' ELSE (SELECT EmployeeName FROM SuzukiR4..GnMstEmployee WHERE CompanyCode = a.CompanyCode AND BranchCode = @OutletCode AND EmployeeID = @SalesCoordinator) END SalesCoordinator,
		CASE WHEN @Salesman = '' THEN 'ALL' ELSE (SELECT EmployeeName FROM SuzukiR4..GnMstEmployee WHERE CompanyCode = a.CompanyCode AND BranchCode = @OutletCode AND EmployeeID = @Salesman) END Salesman,
		CASE WHEN @DealerCode = '' THEN 'ALL' ELSE (SELECT DealerName FROM SuzukiR4..gnMstDealerMapping WHERE DealerCode = @DealerCode) END Dealer,
		CASE WHEN @OutletCode = '' THEN 'ALL' ELSE (SELECT BranchName FROM SuzukiR4..gnMstOrganizationDtl WHERE CompanyCode = @DealerCode AND BranchCode = @OutletCode) END Outlet,
		a.LookupValue Source,
		CASE CAST(ISNULL(SUM(P), 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(SUM(P) AS VARCHAR) END NEW,
		CASE CAST(ISNULL(SUM(P), 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(SUM(P) AS VARCHAR) END P,
		CASE CAST(ISNULL(SUM(HP), 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(SUM(HP) AS VARCHAR) END HP,
		CASE CAST(ISNULL(SUM(SPK), 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(SUM(SPK) AS VARCHAR) END SPK,
		CASE CAST(ISNULL(SUM(SumOuts), 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(SUM(SumOuts) AS VARCHAR) END SumOuts,
		CASE CAST(ISNULL(SUM(DO), 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(SUM(DO) AS VARCHAR) END DO,
		CASE CAST(ISNULL(SUM(DELIVERY), 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(SUM(DELIVERY) AS VARCHAR) END DELIVERY,
		CASE CAST(ISNULL(SUM(LOST), 0) AS VARCHAR) WHEN '0' THEN '-' ELSE CAST(SUM(LOST) AS VARCHAR) END LOST
	FROM #employee_src a
	LEFT JOIN 
	(
		SELECT 
			a.CompanyCode,
			a.BranchCode,
			a.PerolehanData,

			(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
			WHERE CompanyCode = @DealerCode AND
				BranchCode = @OutletCode AND 
				PerolehanData = a.PerolehanData AND 
				SpvEmployeeID = a.SpvEmployeeID AND
				CONVERT(VARCHAR, InquiryDate, 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)) NEW,

			(CASE WHEN @TypeReport = '0' THEN	
				(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
				WHERE CompanyCode = @DealerCode AND
					BranchCode = @OutletCode AND 
					PerolehanData = a.PerolehanData AND 
					SpvEmployeeID = a.SpvEmployeeID AND
					LastProgress = 'P' AND 
					CONVERT(VARCHAR, LastUpdateStatus, 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112))
			ELSE													
				(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
				WHERE CompanyCode = @DealerCode AND
					BranchCode = @OutletCode AND 
					PerolehanData = a.PerolehanData AND 
					SpvEmployeeID = a.SpvEmployeeID AND
					LastProgress = 'P' AND 
					CONVERT(VARCHAR, LastUpdateStatus, 112) <= CONVERT(VARCHAR, @EndDate, 112)) END) P,
				
			(CASE WHEN @TypeReport = '0' THEN	
				(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
				WHERE CompanyCode = @DealerCode AND
					BranchCode = @OutletCode AND 
					PerolehanData = a.PerolehanData AND 
					SpvEmployeeID = a.SpvEmployeeID AND
					LastProgress = 'HP' AND 
					CONVERT(VARCHAR, LastUpdateStatus, 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112))
			ELSE
				(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
				WHERE CompanyCode = @DealerCode AND
					BranchCode = @OutletCode AND 
					PerolehanData = a.PerolehanData AND 
					SpvEmployeeID = a.SpvEmployeeID AND
					LastProgress = 'HP' AND 
					CONVERT(VARCHAR, LastUpdateStatus, 112) <= CONVERT(VARCHAR, @EndDate, 112)) END) HP,
			
			(CASE WHEN @TypeReport = '0' THEN			
				(SELECT COUNT(Hist.LastProgress) FROM SuzukiR4..pmStatusHistory Hist
					INNER JOIN SuzukiR4..pmKdp KDP ON 
					KDP.CompanyCode = Hist.CompanyCode AND
					KDP.BranchCode = Hist.BranchCode AND
					KDP.InquiryNumber = Hist.InquiryNumber 
				 WHERE Hist.CompanyCode = @DealerCode AND
					Hist.BranchCode = @OutletCode AND 
					KDP.PerolehanData = a.PerolehanData AND 
					KDP.SpvEmployeeID = a.SpvEmployeeID AND		
					CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112) AND
					Hist.LastProgress = 'SPK')
			ELSE
			(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
				WHERE CompanyCode = @DealerCode AND
				BranchCode = @OutletCode AND 
				PerolehanData = a.PerolehanData AND 
				SpvEmployeeID = a.SpvEmployeeID AND		
				LastProgress = 'SPK' AND 
				CONVERT(VARCHAR, LastUpdateStatus, 112) <= CONVERT(VARCHAR, @EndDate, 112)) 
				END) SPK,
				
			(CASE WHEN @TypeReport = '0' THEN	
				(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
				WHERE CompanyCode = @DealerCode AND
					BranchCode = @OutletCode AND 
					PerolehanData = a.PerolehanData AND 
					SpvEmployeeID = a.SpvEmployeeID AND
					LastProgress = 'P' AND 
					CONVERT(VARCHAR, LastUpdateStatus, 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)) +
				(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
				WHERE CompanyCode = @DealerCode AND
					BranchCode = @OutletCode AND 
					PerolehanData = a.PerolehanData AND 
					SpvEmployeeID = a.SpvEmployeeID AND
					LastProgress = 'HP' AND 
					CONVERT(VARCHAR, LastUpdateStatus, 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112)) +
				(SELECT COUNT(Hist.LastProgress) FROM SuzukiR4..pmStatusHistory Hist
					INNER JOIN SuzukiR4..pmKdp KDP ON 
					KDP.CompanyCode = Hist.CompanyCode AND
					KDP.BranchCode = Hist.BranchCode AND
					KDP.InquiryNumber = Hist.InquiryNumber 
				 WHERE Hist.CompanyCode = @DealerCode AND
					Hist.BranchCode = @OutletCode AND 
					KDP.PerolehanData = a.PerolehanData AND 
					KDP.SpvEmployeeID = a.SpvEmployeeID AND		
					CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112) AND
					Hist.LastProgress = 'SPK')					
			ELSE													
				(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
				WHERE CompanyCode = @DealerCode AND
					BranchCode = @OutletCode AND 
					PerolehanData = a.PerolehanData AND 
					SpvEmployeeID = a.SpvEmployeeID AND
					LastProgress = 'P' AND 
					CONVERT(VARCHAR, LastUpdateStatus, 112) <= CONVERT(VARCHAR, @EndDate, 112)) + 
				(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
				WHERE CompanyCode = @DealerCode AND
					BranchCode = @OutletCode AND 
					PerolehanData = a.PerolehanData AND 
					SpvEmployeeID = a.SpvEmployeeID AND
					LastProgress = 'HP' AND 
					CONVERT(VARCHAR, LastUpdateStatus, 112) <= CONVERT(VARCHAR, @EndDate, 112)) +				
				(SELECT COUNT(LastProgress) FROM SuzukiR4..pmKdp 
					WHERE CompanyCode = @DealerCode AND
					BranchCode = @OutletCode AND 
					PerolehanData = a.PerolehanData AND 
					SpvEmployeeID = a.SpvEmployeeID AND		
					LastProgress = 'SPK' AND 
					CONVERT(VARCHAR, LastUpdateStatus, 112) <= CONVERT(VARCHAR, @EndDate, 112)) END) SumOuts,
				
			(SELECT COUNT(Hist.LastProgress) FROM SuzukiR4..pmStatusHistory Hist
			INNER JOIN SuzukiR4..pmKdp KDP ON 
				KDP.CompanyCode = Hist.CompanyCode AND
				KDP.BranchCode = Hist.BranchCode AND
				KDP.InquiryNumber = Hist.InquiryNumber 
			 WHERE Hist.CompanyCode = @DealerCode AND
				Hist.BranchCode = @OutletCode AND 
				KDP.PerolehanData = a.PerolehanData AND 
				KDP.SpvEmployeeID = a.SpvEmployeeID AND				
				CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112) AND
				Hist.LastProgress = 'DO') DO,
				
			(SELECT COUNT(Hist.LastProgress) FROM SuzukiR4..pmStatusHistory Hist
				INNER JOIN SuzukiR4..pmKdp KDP ON 
				KDP.CompanyCode = Hist.CompanyCode AND
				KDP.BranchCode = Hist.BranchCode AND
				KDP.InquiryNumber = Hist.InquiryNumber 
			 WHERE Hist.CompanyCode = @DealerCode AND
				Hist.BranchCode = @OutletCode AND 
				KDP.PerolehanData = a.PerolehanData AND 
				KDP.SpvEmployeeID = a.SpvEmployeeID AND			
				CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112) AND
				Hist.LastProgress = 'DELIVERY') DELIVERY,
				
			(SELECT COUNT(Hist.LastProgress) FROM SuzukiR4..pmStatusHistory Hist
				INNER JOIN SuzukiR4..pmKdp KDP ON 
				KDP.CompanyCode = Hist.CompanyCode AND
				KDP.BranchCode = Hist.BranchCode AND
				KDP.InquiryNumber = Hist.InquiryNumber 
			 WHERE Hist.CompanyCode = @DealerCode AND
				Hist.BranchCode = @OutletCode AND 
				KDP.PerolehanData = a.PerolehanData AND 
				KDP.SpvEmployeeID = a.SpvEmployeeID AND			
				CONVERT(VARCHAR, Hist.UpdateDate , 112) BETWEEN CONVERT(VARCHAR, @StartDate, 112) AND CONVERT(VARCHAR, @EndDate, 112) AND
				Hist.LastProgress = 'LOST') LOST					
		FROM SuzukiR4..pmKdp a
		WHERE (CASE WHEN @DealerCode = '' THEN '' ELSE a.CompanyCode END) = @DealerCode
			AND (CASE WHEN @OutletCode = '' THEN '' ELSE a.BranchCode END) = @OutletCode
			AND a.SpvEmployeeID IN (SELECT EmployeeID FROM #employee_src_3)
		GROUP BY a.PerolehanData, a.CompanyCode,
			a.BranchCode, a.SpvEmployeeID
	) b ON a.CompanyCode = b.CompanyCode AND a.LookupValue = b.PerolehanData
	GROUP BY a.CompanyCode, a.LookupValue

	DROP TABLE #employee_src, #employee_src_2, #employee_src_3
	END		
END					 	 	     
--#endregion

END

GO

if object_id('uspfn_itsinqAchievementSourceData') is not null
	drop procedure uspfn_itsinqAchievementSourceData
GO

CREATE procedure [dbo].[uspfn_itsinqAchievementSourceData]
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
 
select * into #SSD from (  
  select CompanyCode, '' BranchCode, TypeOf1, TypeOf2  
   , isnull(Jan, 0) Jan, isnull(Feb, 0) Feb, isnull(Mar, 0) Mar, isnull(Apr, 0) Apr, isnull(May, 0) May, isnull(Jun, 0) Jun  
   , isnull(Jul, 0) Jul, isnull(Aug, 0) Aug, isnull(Sep, 0) Sep, isnull(Oct, 0) Oct, isnull(Nov, 0) Nov, isnull(Dec, 0) Dec  
  from (  
   select kdp.CompanyCode, '' BranchCode, kdp.PerolehanData TypeOf1, '' TypeOf2  
    , substring(Convert(varchar, kdp.InquiryDate, 109), 1, 3) InquiryMonth, count(kdp.EmployeeID) InquiryCount  
   from PmKDP kdp  
   where kdp.CompanyCode = @CompanyCode and year(kdp.InquiryDate) = @Year  
    and kdp.BranchCode in (select BranchCode from #ListOfSalesman)        
    and kdp.EmployeeID in (select EmployeeID from #ListOfSalesman)  
   group by kdp.CompanyCode, kdp.PerolehanData, substring(Convert(varchar, kdp.InquiryDate, 109), 1, 3)) as Header  
  pivot  
  (  
   sum (inquiryCount)  
   for InquiryMonth in (Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, Dec)  
  ) pvt  
 ) #SSD 
 
select    
    '1' inc, 'Sales Source of Data' initial, tempSSD.CompanyCode, '' BranchCode, lkpDtl.LookUpValue TypeOf1, tempSSD.TypeOf2  
   , isnull(tempSSD.Jan, 0) Jan, isnull(tempSSD.Feb, 0) Feb, isnull(tempSSD.Mar, 0) Mar  
   , isnull(tempSSD.Apr, 0) Apr, isnull(tempSSD.May, 0) May, isnull(tempSSD.Jun, 0) Jun  
   , isnull(tempSSD.Jul, 0) Jul, isnull(tempSSD.Aug, 0) Aug, isnull(tempSSD.Sep, 0) Sep  
   , isnull(tempSSD.Oct, 0) Oct, isnull(tempSSD.Nov, 0) Nov, isnull(tempSSD.Dec, 0) Dec  
   , isnull((tempSSD.Jan + tempSSD.Feb + tempSSD.Mar + tempSSD.Apr + tempSSD.May + tempSSD.Jun), 0) Sem1  
   , isnull((tempSSD.Jul + tempSSD.Aug + tempSSD.Sep + tempSSD.Oct + tempSSD.Nov+ tempSSD.Dec), 0) Sem2  
   , isnull((tempSSD.Jan + tempSSD.Feb + tempSSD.Mar + tempSSD.Apr + tempSSD.May + tempSSD.Jun  
    + tempSSD.Jul + tempSSD.Aug + tempSSD.Sep + tempSSD.Oct + tempSSD.Nov  
    + tempSSD.Dec), 0) Total  
  from   
   GnMstLookUpDtl lkpDtl  
    left join #SSD tempSSD on lkpDtl.CompanyCode = tempSSD.CompanyCode  
    and lkpDtl.LookUpValue = tempSSD.TypeOf1   
  where   
   lkpDtl.CodeID = 'PSRC' and lkpDtl.CompanyCode = @CompanyCode  
  
 
 drop TABLE #LIstOfSalesman
 drop TABLE #SSD
 end
GO
 
if object_id('uspfn_itsinqAchievementSalesType') is not null
	drop procedure uspfn_itsinqAchievementSalesType
GO
CREATE procedure [dbo].[uspfn_itsinqAchievementSalesType]
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

-- declare @CompanyCode  VARCHAR(15)  
-- SET @CompanyCode = '6006406'
-- declare @BMEmployeeID  VARCHAR(15)
-- SET @BMEmployeeID = '%'  
-- declare @SHEmployeeID  VARCHAR(15) 
-- SET @SHEmployeeID = '%' 
-- declare @SCEmployeeID  VARCHAR(15)
-- SET @SCEmployeeID = '%'  
-- declare @SMEmployeeID  VARCHAR(15)
-- SET @SMEmployeeID ='%'
-- DECLARE  @Year    INT  
-- SET @Year = 2013

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
 
  select * into #ST from (  
  select CompanyCode, '' BranchCode, TypeOf1, TypeOf2  
   , isnull(Jan, 0) Jan, isnull(Feb, 0) Feb, isnull(Mar, 0) Mar, isnull(Apr, 0) Apr, isnull(May, 0) May, isnull(Jun, 0) Jun  
   , isnull(Jul, 0) Jul, isnull(Aug, 0) Aug, isnull(Sep, 0) Sep, isnull(Oct, 0) Oct, isnull(Nov, 0) Nov, isnull(Dec, 0) Dec  
  from (  
   select kdp.CompanyCode, '' BranchCode, kdp.TipeKendaraan TypeOf1, kdp.Variant TypeOf2  
    , substring(Convert(varchar, kdp.InquiryDate, 109), 1, 3) InquiryMonth  
    , count(kdp.EmployeeID) InquiryCount  
   from PmKDP kdp  
   where kdp.CompanyCode = @CompanyCode and year(kdp.InquiryDate) = @Year  
    and kdp.BranchCode in (select BranchCode from #ListOfSalesman)        
    and kdp.EmployeeID in (select EmployeeID from #ListOfSalesman)       
   group by kdp.CompanyCode, kdp.TipeKendaraan, kdp.Variant   
    , substring(Convert(varchar, kdp.InquiryDate, 109), 1, 3)) as Header  
  pivot  
  (  
   sum(InquiryCount)  
   for InquiryMonth in (Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, Dec)  
  ) pvt  
 ) #ST 

select    
   '2' inc, 'Sales by Type' initial, tempST.CompanyCode, tempST.BranchCode, GTS.GroupCode, GTS.typeCode-- tempST.TypeOf1, tempST.TypeOf2  
   , isnull(tempST.Jan, 0) Jan, isnull(tempST.Feb, 0) Feb, isnull(tempST.Mar, 0) Mar  
   , isnull(tempST.Apr, 0) Apr, isnull(tempST.May, 0) May, isnull(tempST.Jun, 0) Jun  
   , isnull(tempST.Jul, 0) Jul, isnull(tempST.Aug, 0) Aug, isnull(tempST.Sep, 0) Sep  
   , isnull(tempST.Oct, 0) Oct, isnull(tempST.Nov, 0) Nov, isnull(tempST.Dec, 0) Dec  
   , isnull((tempST.Jan + tempST.Feb + tempST.Mar + tempST.Apr + tempST.May + tempST.Jun), 0) Sem1  
   , isnull((tempST.Jul + tempST.Aug + tempST.Sep + tempST.Oct + tempST.Nov+ tempST.Dec), 0) Sem2  
   , isnull((tempST.Jan + tempST.Feb + tempST.Mar + tempST.Apr + tempST.May + tempST.Jun  
    + tempST.Jul + tempST.Aug + tempST.Sep + tempST.Oct + tempST.Nov  
    + tempST.Dec), 0) Total  
  from   
   (select Distinct CompanyCode, GroupCode, TypeCode    
   from pmGroupTypeSeq   
   group by CompanyCode, GroupCode ,typeCode) GTS  
    left join #ST tempST on GTS.CompanyCode = tempST.CompanyCode and GTS.GroupCode = tempST.TypeOf1 and GTS.TypeCode = tempST.TypeOf2  
       

DROP TABLE #ListOfSalesman
DROP TABLE #ST

end
GO

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

if object_id('uspfn_itsinqAchievementProsStat') is not null
	drop procedure uspfn_itsinqAchievementProsStat
GO

CREATE procedure [dbo].[uspfn_itsinqAchievementProsStat]
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

 select * into #PS from (  
  select CompanyCode, '' BranchCode, TypeOf1, TypeOf2  
   , isnull(Jan, 0) Jan, isnull(Feb, 0) Feb, isnull(Mar, 0) Mar, isnull(Apr, 0) Apr, isnull(May, 0) May, isnull(Jun, 0) Jun  
   , isnull(Jul, 0) Jul, isnull(Aug, 0) Aug, isnull(Sep, 0) Sep, isnull(Oct, 0) Oct, isnull(Nov, 0) Nov, isnull(Dec, 0) Dec   
  from (  
   select kdp.CompanyCode, '' BranchCode, kdp.LastProgress TypeOf1, '' TypeOf2  
    , substring(Convert(varchar, kdp.InquiryDate, 109), 1, 3) InquiryMonth  
    , count(kdp.EmployeeID) InquiryCount  
   from PmKDP kdp  
   where kdp.CompanyCode = @CompanyCode and year(kdp.InquiryDate) = @Year   
    and kdp.BranchCode in (select BranchCode from #ListOfSalesman)        
    and kdp.EmployeeID in (select EmployeeID from #ListOfSalesman)        
   group by kdp.CompanyCode, kdp.BranchCode, kdp.LastProgress  
    , substring(Convert(varchar, kdp.InquiryDate, 109), 1, 3)) as Header  
  pivot  
  (  
   sum(InquiryCount)  
   for InquiryMonth in (Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, Dec)  
  ) pvt  
 ) #PS
 
select   
   '3' inc, 'Prospect Status' initial, tempPS.CompanyCode, tempPS.BranchCode, lkpDtl.LookUpValueName TypeOf1, tempPS.TypeOf2  
   , isnull(tempPS.Jan, 0) Jan, isnull(tempPS.Feb, 0) Feb, isnull(tempPS.Mar, 0) Mar  
   , isnull(tempPS.Apr, 0) Apr, isnull(tempPS.May, 0) May, isnull(tempPS.Jun, 0) Jun  
   , isnull(tempPS.Jul, 0) Jul, isnull(tempPS.Aug, 0) Aug, isnull(tempPS.Sep, 0) Sep  
   , isnull(tempPS.Oct, 0) Oct, isnull(tempPS.Nov, 0) Nov, isnull(tempPS.Dec, 0) Dec  
   , isnull((tempPS.Jan + tempPS.Feb + tempPS.Mar + tempPS.Apr + tempPS.May + tempPS.Jun), 0) Sem1  
   , isnull((tempPS.Jul + tempPS.Aug + tempPS.Sep + tempPS.Oct + tempPS.Nov+ tempPS.Dec), 0) Sem2  
   , isnull((tempPS.Jan + tempPS.Feb + tempPS.Mar + tempPS.Apr + tempPS.May + tempPS.Jun  
    + tempPS.Jul + tempPS.Aug + tempPS.Sep + tempPS.Oct + tempPS.Nov  
    + tempPS.Dec), 0) Total  
  from  
   GnMstLookUpDtl lkpDtl  
    left join #PS tempPS on lkpDtl.CompanyCode = tempPS.CompanyCode  
    and lkpDtl.LookUpValue = tempPS.TypeOf1   
  where lkpDtl.CodeID = 'PSTS' and lkpDtl.CompanyCode = @CompanyCode  
  
  
 DROP TABLE #ListOfSalesMan
 Drop TABLE #PS
 
 end
 
GO

if object_id('uspfn_itsinqfollowup2') is not null
	drop procedure uspfn_itsinqfollowup2
GO
CREATE Procedure uspfn_itsinqfollowup2
(        
 @CompanyCode VARCHAR(15),        
 @BranchCode  VARCHAR(15),        
 @DateAwal  VARCHAR(10),        
 @DateAkhir  VARCHAR(10),        
 @Outlet   VARCHAR(max),        
 @SPV   VARCHAR(max),        
 @EMP   VARCHAR(max),           
 @Head   VARCHAR(max)        
)        
AS        
BEGIN        
    
--declare @CompanyCode VARCHAR(15)    
--SET @CompanyCode = '6006406'    
--declare @BranchCode  VARCHAR(15)    
--SET @BranchCode = '6006406'    
--declare @DateAwal  VARCHAR(10)        
--SET @DateAwal = '20100101'    
--declare @DateAkhir  VARCHAR(10)      
--SET @DateAkhir ='20140117'    
--declare @Outlet   VARCHAR(max)        
--SET @Outlet = '0601'    
--declare @SPV   VARCHAR(max)        
--SET @SPV ='50438'    
--declare @EMP   VARCHAR(max)        
--SET @EMP = '52153'    
--declare @Head   VARCHAR(max)        
--SET @Head = ''    
   
 

SELECT * INTO #t1 FROM (
SELECT
    f.OutletName, a.InquiryNumber, a.NamaProspek Pelanggan, a.InquiryDate, a.TipeKendaraan,
    a.Variant, a.Transmisi, b.RefferenceDesc1 Warna, a.PerolehanData,
    Emp1.EmployeeName Employee, Emp2.EmployeeName Supervisor, e.NextFollowUpDate, a.LastProgress, e.ActivityDetail, a.SpvEmployeeId
FROM
    PmKDP a
LEFT JOIN OmMstRefference b
    ON b.CompanyCode = a.CompanyCode AND b.RefferenceType='COLO' AND b.RefferenceCode=a.ColourCode
left join HrEmployee emp1 on emp1.CompanyCode = a.CompanyCode  
     and emp1.EmployeeID = a.EmployeeID  
--LEFT JOIN GnMstEmployee c
--    ON c.CompanyCode = a.CompanyCode AND c.BranchCode = a.BranchCode AND c.EmployeeID = a.EmployeeID
left join HrEmployee emp2 on a.CompanyCode = emp2.CompanyCode  
     and a.EmployeeID = emp2.EmployeeID  
--LEFT JOIN GnMstEmployee d
--    ON d.CompanyCode = a.CompanyCode AND d.BranchCode = a.BranchCode AND d.EmployeeID = a.SpvEmployeeID
LEFT JOIN PmActivities e
    ON e.CompanyCode = a.CompanyCode AND e.BranchCode = a.BranchCode AND e.InquiryNumber=a.InquiryNumber
    AND e.ActivityID = (SELECT TOP 1 ActivityID FROM PmActivities WHERE CompanyCode = a.CompanyCode 
	AND BranchCode = a.BranchCode AND InquiryNumber=a.InquiryNumber ORDER BY ActivityID DESC) 
LEFT JOIN PmBranchOutlets f
	ON f.CompanyCode = a.CompanyCode AND f.BranchCode = a.BranchCode AND f.OutletID = a.OutletID

WHERE
    a.CompanyCode = @CompanyCode 
	AND ((CASE WHEN @BranchCode='' THEN a.BranchCode END)<>''OR (CASE WHEN @BranchCode<>'' THEN a.BranchCode END)=@BranchCode)
    AND CONVERT(VARCHAR, e.NextFollowUpDate, 112) BETWEEN @DateAwal AND @DateAkhir
    AND ((CASE WHEN @Outlet='' THEN a.OutletID END)<>'' OR (CASE WHEN @Outlet<>'' THEN a.OutletID END)=@Outlet)
    AND ((CASE WHEN @SPV='' THEN a.SpvEmployeeID END)<>'' OR (CASE WHEN @SPV<>'' THEN a.SpvEmployeeID END)=@SPV)
    AND ((CASE WHEN @EMP='' THEN a.EmployeeID END)<>'' OR (CASE WHEN @EMP<>'' THEN a.EmployeeID END)=@EMP)
) #t1

IF (@HEAD='')
BEGIN
    SELECT * FROM #t1 ORDER BY InquiryNumber 
END
ELSE
BEGIN
	declare @teamid varchar(max)
	set @teamid = (select teamid from pmmstteammembers where companycode=@CompanyCode 
		and branchcode=case @branchcode when '' then branchcode else @branchcode end
		and employeeid=@HEAD 
		and issupervisor='1')
	
	SELECT * FROM #t1 WHERE SpvEmployeeID IN (select employeeid from pmmstteammembers where companycode=@companycode 
		and branchcode=case @branchcode when '' then branchcode else @branchcode end
		and teamid=@teamid
		and issupervisor='0') 
	ORDER BY InquiryNumber 
END
DROP TABLE #t1
end
GO

if object_id('uspfn_itsinqfollowup') is not null
	drop procedure uspfn_itsinqfollowup
GO

CREATE Procedure uspfn_itsinqfollowup  
(      
 @CompanyCode VARCHAR(15),      
 @BranchCode  VARCHAR(15),      
 @DateAwal  VARCHAR(10),      
 @DateAkhir  VARCHAR(10),      
 @Outlet   VARCHAR(max),      
 @SPV   VARCHAR(max),      
 @EMP   VARCHAR(max),         
 @Head   VARCHAR(max)      
)      
AS      
BEGIN      
  
SELECT * INTO #t1 FROM (
SELECT
    f.OutletName, a.InquiryNumber, a.NamaProspek Pelanggan, a.InquiryDate, a.TipeKendaraan,
    a.Variant, a.Transmisi, b.RefferenceDesc1 Warna, a.PerolehanData,
    emp1.EmployeeName Employee, emp2.EmployeeName Supervisor, e.NextFollowUpDate, a.LastProgress, e.ActivityDetail, a.SpvEmployeeId
FROM
    PmKDP a
LEFT JOIN OmMstRefference b
    ON b.CompanyCode = a.CompanyCode AND b.RefferenceType='COLO' AND b.RefferenceCode=a.ColourCode
left join HrEmployee emp1 on emp1.CompanyCode = a.CompanyCode  
     and emp1.EmployeeID = a.EmployeeID  
--LEFT JOIN GnMstEmployee c
--    ON c.CompanyCode = a.CompanyCode AND c.BranchCode = a.BranchCode AND c.EmployeeID = a.EmployeeID
left join HrEmployee emp2 on emp2.CompanyCode = a.CompanyCode  
     and emp2.EmployeeID = a.EmployeeID  
--LEFT JOIN GnMstEmployee d
--    ON d.CompanyCode = a.CompanyCode AND d.BranchCode = a.BranchCode AND d.EmployeeID = a.SpvEmployeeID
LEFT JOIN PmActivities e
    ON e.CompanyCode = a.CompanyCode AND e.BranchCode = a.BranchCode AND e.InquiryNumber=a.InquiryNumber
    AND e.ActivityID = (SELECT TOP 1 ActivityID FROM PmActivities WHERE CompanyCode = a.CompanyCode 
	AND BranchCode = a.BranchCode AND InquiryNumber=a.InquiryNumber ORDER BY ActivityID DESC) 
LEFT JOIN PmBranchOutlets f
	ON f.CompanyCode = a.CompanyCode AND f.BranchCode = a.BranchCode AND f.OutletID = a.OutletID
WHERE
    a.CompanyCode = @CompanyCode 
	AND ((CASE WHEN @BranchCode='' THEN a.BranchCode END)<>''OR (CASE WHEN @BranchCode<>'' THEN a.BranchCode END)=@BranchCode)
    AND CONVERT(VARCHAR, e.NextFollowUpDate, 112) BETWEEN @DateAwal AND @DateAkhir
    AND ((CASE WHEN @Outlet='' THEN a.OutletID END)<>'' OR (CASE WHEN @Outlet<>'' THEN a.OutletID END)=@Outlet)
    AND ((CASE WHEN @SPV='' THEN a.SpvEmployeeID END)<>'' OR (CASE WHEN @SPV<>'' THEN a.SpvEmployeeID END)=@SPV)
    AND ((CASE WHEN @EMP='' THEN a.EmployeeID END)<>'' OR (CASE WHEN @EMP<>'' THEN a.EmployeeID END)=@EMP)
) #t1

IF (@HEAD='')
BEGIN
    SELECT * FROM #t1 ORDER BY InquiryNumber 
END
ELSE
BEGIN
	declare @teamid varchar(max)
	set @teamid = (select teamid from pmmstteammembers where companycode=@CompanyCode 
		and branchcode=case @branchcode when '' then branchcode else @branchcode end
		and employeeid=@HEAD 
		and issupervisor='1')
	
	SELECT * FROM #t1 WHERE SpvEmployeeID IN (select employeeid from pmmstteammembers where companycode=@companycode 
		and branchcode=case @branchcode when '' then branchcode else @branchcode end
		and teamid=@teamid
		and issupervisor='0') 
	ORDER BY InquiryNumber 
END
DROP TABLE #t1 

END
GO


if object_id('uspfn_pmSelectMembers') is not null
	drop procedure uspfn_pmSelectMembers
GO
--Created by Benedict 13 Mar 2015
CREATE PROCEDURE uspfn_pmSelectMembers
	@CompanyCode varchar(20),
	@BranchCode  varchar(20),
	@EmployeeID varchar(20)
--DECLARE	
--	@CompanyCode varchar(20) = '6006406',
--	@BranchCode  varchar(20) = '6006401',
--	@EmployeeID varchar(20) = '51343'
AS
BEGIN
DECLARE 
	@pos1 varchar(5) = (SELECT Position FROM HrEmployee WHERE CompanyCode = @CompanyCode AND EmployeeID = @EmployeeID)

IF (@pos1 = 'S' )
BEGIN

SELECT BranchCode, CONVERT(varchar, InquiryNumber) AS KeyID, NamaProspek, PerolehanData
	 , CONVERT(varchar, InquiryNumber) + ' - ' + RTRIM(NamaProspek)
	 + CASE WHEN RTRIM(PerolehanData) = '' THEN '' ELSE 
	 ' (' + RTRIM(PerolehanData) + ')' END AS Member
  FROM pmKdp
 WHERE CompanyCode = @CompanyCode
   AND BranchCode = @BranchCode
   AND EmployeeID = @EmployeeID
END

IF (@pos1 <> 'S')
BEGIN

SELECT a.BranchCode, a.EmployeeID AS KeyID, 
	CONVERT(varchar, a.EmployeeID) + ' - ' + RTRIM(b.EmployeeName)
	 + CASE WHEN RTRIM(e.PosName) = '' THEN '' ELSE 
	 ' (' + RTRIM(e.PosName) + ')' END AS Member
FROM hrEmployeeMutation a
JOIN (
	SELECT c.EmployeeId, c.EmployeeName, c.Position, MAX(d.MutationDate) AS MutationDate
	FROM hrEmployee c
	JOIN hrEmployeeMutation d
	ON c.EmployeeId = d.EmployeeId AND c.PersonnelStatus = 1 AND c.IsDeleted = 0 AND d.IsDeleted = 0
	WHERE c.Department = 'SALES' AND c.TeamLeader = @EmployeeID
	GROUP BY c.EmployeeId, c.EmployeeName, c.Position
) b
ON a.EmployeeId = b.EmployeeId AND a.MutationDate = b.MutationDate
JOIN gnMstPosition e
ON a.CompanyCode = e.CompanyCode AND e.DeptCode = 'SALES' AND b.Position = e.PosCode
WHERE a.CompanyCode = @CompanyCode AND a.BranchCode = @BranchCode

END
END
GO

if object_id('uspfn_OmInquiryMKT_genExcel') is not null
	drop procedure uspfn_OmInquiryMKT_genExcel
GO
-- ==============================================================================
-- Author			: Seandy A.K
-- Create date		: 30-1-2013
-- Description		: Inquiry ITS
-- Query Activation : uspfn_InquiryITS '2013-03-12','2013-03-12','CABANG','',''
-- Query Activation : uspfn_OmInquiryMKT '2013-03-12','2013-03-12','CABANG','',''
-- update by fhi    : fhi 17-03-2015
-- Query Activation : uspfn_OmInquiryMKT_genExcel '2013-03-12','2013-03-12','CABANG','6006406',''
-- ==============================================================================
Create PROCEDURE [dbo].[uspfn_OmInquiryMKT_genExcel]
	@StartDate	datetime,
	@EndDate	datetime,
	@Area		nvarchar(100),
	@DealerCode	nvarchar(15),
	@OutletCode	nvarchar(15)
AS
declare @National varchar(10)
set @National = (select top 1 ISNULL(ParaValue,0) from gnMstLookUpDtl
                  where CodeID='QSLS' and LookUpValue='NATIONAL')

Declare @MainTable table
(
	DealerCode			varchar(15),
	DealerAbbreviation	varchar(50),
	OutletCode			varchar(15),
	OutletAbbreviation	varchar(50),
	TipeKendaraan		varchar(50),
	Variant				varchar(50),
	OutsINQ				numeric(18,0),
	NewINQ				numeric(18,0),
	OutsSPK				numeric(18,0),
	NewSPK				numeric(18,0),
	CancelSPK			numeric(18,0),
	FakturPolisi		numeric(18,0),
	Balance				numeric(18,0),
	ATTestDrive			int,
	MTTestDrive			int
)

if(@National = 1)
begin	
	Select * into #t1 from(
		Select distinct b.Area
			 , b.DealerCode
			 , b.DealerAbbreviation
			 , c.OutletCode
			 , c.OutletAbbreviation
			 , a.TipeKendaraan
			 , a.Transmisi
			 , a.Variant
			 , a.InquiryDate
			 , a.SPKDate
			 , case when isnull(a.TestDrive,'') = '' then 0 else case when isnull(a.TestDrive,'') = 'YA' then 1 else 0 end end TestDrive
			 , a.InquiryNumber
			 , a.LastProgress
		from pmHstITS a
		left join GnMstDealerMapping b on a.CompanyCode = b.DealerCode
		left join GnMstDealerOutletMapping c on a.CompanyCode = c.DealerCode
			and a.BranchCode = c.OutletCode
		where convert(varchar,InquiryDate,112) <= convert(varchar,@EndDate,112)
		  and (b.Area like Case when @Area <> '' 
						then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
								then 'JABODETABEK'
								else @Area end
						else '%%' end
		   or b.Area like Case when @Area <> '' 
						then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
								then 'CABANG'
								else @Area end
						else '%%' end) 	
		  and b.DealerCode like case when @DealerCode = '' then '%%' else @DealerCode end
		  and c.OutletCode like case when @OutletCode = '' then '%%' else @OutletCode end
	)#t1  
	
	select * into #TempStock from(
		select distinct a.CompanyCode
			 , a.BranchCode
			 , a.TipeKendaraan
			 , a.Variant
			 , a.ColourCode
			 --, isnull(f.UnitQuantity,0) UnitQuantity
			 , (select TOP 1 UnitQuantity
			      from pmHstStock
			    where CompanyCode = a.CompanyCode
			      and BranchCode = a.BranchCode
			      and Year(TanggalStock) = YEAR(@EndDate)
			      and MONTH(TanggalStock) = MONTH(@EndDate)
			      and TipeKendaraan = a.TipeKendaraan
			      and Variant = a.Variant
			      and ColourCode = a.ColourCode
			 order by a.TanggalStock Desc) UnitQuantity
			from pmHstStock a
			where YEAR(a.TanggalStock) = YEAR(@EndDate)
			  and MONTH(a.TanggalStock) = MONTH(@EndDate)
	)#TempStock
	
	Insert into @MainTable
	Select distinct a.DealerCode
		 , a.DealerAbbreviation
		 , a.OutletCode
		 , a.OutletAbbreviation
		 , a.TipeKendaraan
		 , a.Variant
		 , isnull((select Count(*) 
			  from #t1 e
			  where convert(varchar,e.InquiryDate,112) < convert(varchar,@StartDate,112)
				and a.Area = e.Area
				and a.DealerCode = e.DealerCode
				and a.OutletCode = e.OutletCode
				and a.TipeKendaraan = e.TipeKendaraan
			    and e.Variant = a.Variant
				and e.LastProgress in ('P','HP','SPK')),0) OutsINQ
		 , isnull((select Count(*) 
			  from #t1
			  where convert(varchar,InquiryDate,112) between convert(varchar,@StartDate,112) and convert(varchar,@EndDate,112)
				and Area = a.Area
				and DealerCode = a.DealerCode
				and OutletCode = a.OutletCode
				and TipeKendaraan = a.TipeKendaraan
			    and Variant = a.Variant),0) NewINQ
		 , (select COUNT(*) 
			  from #t1 
			 where DealerCode = a.DealerCode
			   and OutletCode = a.OutletCode
			   and convert(varchar,InquiryDate,112) < convert(varchar,@StartDate,112)
			   and convert(varchar,SPKDate,112) between convert(varchar,@StartDate,112) and convert(varchar,@EndDate,112)
			   and TipeKendaraan = a.TipeKendaraan
			   and Variant = a.Variant
			   and LastProgress in ('SPK')) OutsSPK
		 , (select COUNT(*) 
			  from #t1 
			 where DealerCode = a.DealerCode
			   and OutletCode = a.OutletCode
			   and convert(varchar,InquiryDate,112) between convert(varchar,@StartDate,112) and convert(varchar,@EndDate,112)
			   and convert(varchar,SPKDate,112) between convert(varchar,@StartDate,112) and convert(varchar,@EndDate,112)
			   and TipeKendaraan = a.TipeKendaraan
			   and Variant = a.Variant
			   and LastProgress in ('SPK')) NewSPK
		 , (select COUNT(*) 
			  from #t1 e
			 where e.DealerCode = a.DealerCode
			   and e.OutletCode = a.OutletCode
			   and convert(varchar,e.InquiryDate,112) <= convert(varchar,@EndDate,112)
			   and e.TipeKendaraan = a.TipeKendaraan
			   and e.Variant = a.Variant
			   and exists (select  1 
							  from pmHstITS 
							 where InquiryNumber = e.InquiryNumber
							   and LastProgress in ('LOST')
							   and LostCaseCategory = 'KONTRAK TIDAK DISETUJUI')) CancelSPK
		 , isnull((select Count(*)
			  from OmHstInquirySales d
			  left join OmMstModel e on d.CompanyCode = e.CompanyCode
				and d.SalesModelCode = e.SalesModelCode
			  where convert(varchar,d.SuzukiFpolDate,112) between convert(varchar,@StartDate,112) and convert(varchar,@EndDate,112)
				and d.CompanyCode = a.DealerCode
				and d.BranchCode = a.OutletCode
				and e.GroupCode = a.TipeKendaraan
				and Variant = a.Variant
				and e.TypeCode = a.Variant),0) FakturPolisi
		 --, isnull((select isnull(SUM(f.UnitQuantity),0)
			--		from pmHstStock f
			--		where f.CompanyCode = a.DealerCode
			--		  and f.BranchCode = a.OutletCode
			--		  and YEAR(f.TanggalStock) = YEAR(@EndDate)
			--		  and MONTH(f.TanggalStock) = MONTH(@EndDate)
			--		  and f.TipeKendaraan = a.TipeKendaraan
			--		  and f.Variant = a.Variant),0) Balance
		 , isnull((select SUM(f.UnitQuantity)
					from #TempStock f
					where f.CompanyCode = a.DealerCode
					  and f.BranchCode = a.OutletCode
					  and f.TipeKendaraan = a.TipeKendaraan
					  and f.Variant = a.Variant
					  ),0) Balance
		 , isnull((select SUM(TestDrive) 
			  from #t1
			  where convert(varchar,InquiryDate,112) between convert(varchar,@StartDate,112) and convert(varchar,@EndDate,112)
			    and Area = a.Area
			    and DealerCode = a.DealerCode
			    and OutletCode = a.OutletCode
			    and TipeKendaraan = a.TipeKendaraan
			    and Transmisi = 'AT'
			    and Variant = a.Variant),0) ATTestDrive
		 , isnull((select SUM(TestDrive) 
			  from #t1
			  where convert(varchar,InquiryDate,112) between convert(varchar,@StartDate,112) and convert(varchar,@EndDate,112)
			    and Area = a.Area
			    and DealerCode = a.DealerCode
			    and OutletCode = a.OutletCode
			    and TipeKendaraan = a.TipeKendaraan
			    and Transmisi = 'MT'
			    and Variant = a.Variant),0) MTTestDrive
	from #t1 a
	UNION
	select a.CompanyCode
		 , b.DealerAbbreviation
		 , a.BranchCode
		 , c.OutletAbbreviation
		 , isnull(e.GroupCode,'') TipeKendaraan
		 , isnull(e.TypeCode,'') Variant
		 , 0 
		 , 0 
		 , 0
		 , 0
		 , 0
		 , 0
		 , COUNT(*)
		 , 0
		 , 0
	from OmHstInquirySales a
	left join OmMstModel e on e.CompanyCode = a.CompanyCode
		and e.SalesModelCode = a.SalesModelCode
	left join gnMstDealerMapping b on b.DealerCode = a.CompanyCode
	left join gnMstDealerOutletMapping c on c.DealerCode = a.CompanyCode
		and c.OutletCode = a.BranchCode
	where Convert(varchar,a.SuzukiFPOLDate,112) between convert(varchar,@StartDate,112) and convert(varchar,@EndDate,112)
	  and (b.Area like Case when @Area <> '' 
					then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
							then 'JABODETABEK'
							else @Area end
					else '%%' end
	   or b.Area like Case when @Area <> '' 
					then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
							then 'CABANG'
							else @Area end
					else '%%' end) 	
	  and a.CompanyCode like case when @DealerCode = '' then '%%' else @DealerCode end
	  and a.BranchCode like case when @OutletCode = '' then '%%' else @OutletCode end
	  and not exists(select 1 
					   from #t1 d 
					  where d.DealerCode = a.CompanyCode
					    and d.OutletCode = a.BranchCode
					    and e.GroupCode = d.TipeKendaraan
					    and e.TypeCode = d.Variant)
	group by a.CompanyCode,b.DealerAbbreviation,a.BranchCode,c.OutletAbbreviation,e.GroupCode,e.TypeCode
	drop table #t1
END
else
begin
	Select * into #t3 from(
		Select distinct b.Area
			 , b.DealerCode
			 , b.DealerAbbreviation
			 , c.OutletCode
			 , c.OutletAbbreviation
			 , a.TipeKendaraan
			 , a.Variant
			 , a.InquiryDate
			 , a.SPKDate
			 , a.TestDrive
			 , a.Transmisi
			 , a.InquiryNumber
			 , a.LastProgress
		from pmKDP a
		left join GnMstDealerMapping b on a.CompanyCode = b.DealerCode
		left join GnMstDealerOutletMapping c on a.CompanyCode = c.DealerCode
			and a.BranchCode = c.OutletCode
		where convert(varchar,InquiryDate,112) <= convert(varchar,@EndDate,112)
		  and (b.Area like Case when @Area <> '' 
						then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
								then 'JABODETABEK'
								else @Area end
						else '%%' end
		   or b.Area like Case when @Area <> '' 
						then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
								then 'CABANG'
								else @Area end
						else '%%' end) 	
		  and b.DealerCode like case when @DealerCode = '' then '%%' else @DealerCode end
		  and c.OutletCode like case when @OutletCode = '' then '%%' else @OutletCode end
	)#t3
	
	--select * into #tempQty from(
	--	select f.CompanyCode
	--		 , e.BranchCode
	--		 , e.Year
	--		 , e.Month
	--		 , f.GroupCode
	--		 , f.TypeCode
	--		 , SUM(isnull(e.EndingOH,0)) EndingOH
	--	from OmMstModel f
	--	LEFT JOIN OmTrInventQtyVehicle e on f.CompanyCode = e.CompanyCode
	--	   and f.SalesModelCode = e.SalesModelCode
	--	where isnull(e.CompanyCode, '') <> ''
	--	  and isnull(e.BranchCode, '') <> ''
	--	  and e.Year = YEAR(@EndDate)
	--	  and e.Month = MONTH(@EndDate)
	--	group by f.CompanyCode
	--		 , e.BranchCode
	--		 , e.Year
	--		 , e.Month
	--		 , f.GroupCode
	--		 , f.TypeCode
	--)#tempQty
		
	select * into #tempQty from(
		select distinct f.CompanyCode
			 , e.BranchCode
			 , f.GroupCode
			 , f.TypeCode
			 , e.ColourCode
			 , (select TOP 1 b.EndingOH
			      from OmMstModel a
		     LEFT JOIN OmTrInventQtyVehicle b on a.CompanyCode = b.CompanyCode
				   and a.SalesModelCode = b.SalesModelCode
			    where a.CompanyCode = f.CompanyCode
			      and b.BranchCode = e.BranchCode
			      and b.Year = YEAR(@EndDate)
			      and b.Month = MONTH(@EndDate)
			      and a.GroupCode = f.GroupCode
			      and a.TypeCode = f.TypeCode
			      and b.ColourCode = e.ColourCode
			 order by a.CreatedDate,a.LastUpdateDate Desc) EndingOH
		from OmMstModel f
		LEFT JOIN OmTrInventQtyVehicle e on f.CompanyCode = e.CompanyCode
		   and f.SalesModelCode = e.SalesModelCode
		where isnull(e.CompanyCode, '') <> ''
		  and isnull(e.BranchCode, '') <> ''
		  and e.Year = YEAR(@EndDate)
		  and e.Month = MONTH(@EndDate)
	)#tempQty
-- Query Activation : uspfn_OmInquiryMKT '2013-03-12','2013-03-12','CABANG','',''

	Insert into @MainTable
	Select distinct a.DealerCode
		 , a.DealerAbbreviation
		 , a.OutletCode
		 , a.OutletAbbreviation
		 , a.TipeKendaraan
		 , a.Variant
		 , (select Count(*) 
			  from #t3 e
			  where convert(varchar,e.InquiryDate,112) < convert(varchar,@StartDate,112)
			    and a.Area = e.Area
			    and a.DealerCode = e.DealerCode
			    and a.OutletCode = e.OutletCode
			    and a.TipeKendaraan = e.TipeKendaraan
			    and a.Variant = e.Variant
			    and exists (select  1 
							  from pmKDP 
							 where InquiryNumber = e.InquiryNumber
							   and LastProgress in ('P','HP','SPK'))) OutsINQ
		 , (select Count(*) 
			  from #t3
			  where convert(varchar,InquiryDate,112) between convert(varchar,@StartDate,112) and convert(varchar,@EndDate,112)
			    and Area = a.Area
			    and DealerCode = a.DealerCode
			    and OutletCode = a.OutletCode
			    and TipeKendaraan = a.TipeKendaraan
			    and Variant = a.Variant) NewINQ
		 , (select COUNT(*) 
			  from #t3 
			 where DealerCode = a.DealerCode
			   and OutletCode = a.OutletCode
			   and convert(varchar,InquiryDate,112) < convert(varchar,@StartDate,112)
			   and TipeKendaraan = a.TipeKendaraan
			   and Variant = a.Variant
			   and LastProgress in ('SPK')) OutsSPK
		 , (select COUNT(*) 
			  from #t3 
			 where DealerCode = a.DealerCode
			   and OutletCode = a.OutletCode
			   and convert(varchar,InquiryDate,112) between convert(varchar,@StartDate,112) and convert(varchar,@EndDate,112)
			   and TipeKendaraan = a.TipeKendaraan
			   and Variant = a.Variant
			   and LastProgress in ('SPK')) NewSPK
		 , (select COUNT(*) 
			  from #t3 e
			 where e.DealerCode = a.DealerCode
			   and e.OutletCode = a.OutletCode
			   and convert(varchar,e.InquiryDate,112) <= convert(varchar,@EndDate,112)
			   and e.TipeKendaraan = a.TipeKendaraan
			   and e.Variant = a.Variant
			   and exists (select  1 
							  from pmKDP 
							 where InquiryNumber = e.InquiryNumber
							   and LastProgress in ('LOST')
							   and LostCaseCategory = 'D')) CancelSPK
		 , (select COUNT(*) 
			  from #t3 
			 where DealerCode = a.DealerCode
			   and OutletCode = a.OutletCode
			   and convert(varchar,InquiryDate,112) between convert(varchar,@StartDate,112) and convert(varchar,@EndDate,112)
			   and TipeKendaraan = a.TipeKendaraan
			   and Variant = a.Variant
			   and LastProgress in ('DO')) FakturPolisi
 		-- , (select isnull(SUM(f.EndingOH),0)
			--from #TempQty f
			--where f.CompanyCode = a.DealerCode
			--  and f.BranchCode = a.OutletCode
			--  and f.Year = YEAR(@EndDate)
			--  and f.Month = MONTH(@EndDate)
			--  and f.GroupCode = a.TipeKendaraan
			--  and f.TypeCode = a.Variant) Balance
		 , (select isnull(SUM(f.EndingOH),0)
					from #TempQty f
					where f.CompanyCode = a.DealerCode
					  and f.BranchCode = a.OutletCode
					  and f.GroupCode = a.TipeKendaraan
					  and f.TypeCode = a.Variant) Balance
		 , (select Count(*) 
			  from #t3
			  where convert(varchar,InquiryDate,112) between convert(varchar,@StartDate,112) and convert(varchar,@EndDate,112)
			    and Area = a.Area
			    and DealerCode = a.DealerCode
			    and OutletCode = a.OutletCode
			    and TipeKendaraan = a.TipeKendaraan
			    and Variant = a.Variant
			    and Transmisi = 'AT'
			    and TestDrive = '10') ATTestDrive
		, (select Count(*) 
			  from #t3
			  where convert(varchar,InquiryDate,112) between convert(varchar,@StartDate,112) and convert(varchar,@EndDate,112)
			    and Area = a.Area
			    and DealerCode = a.DealerCode
			    and OutletCode = a.OutletCode
			    and TipeKendaraan = a.TipeKendaraan
			    and Variant = a.Variant
			    and Transmisi = 'MT'
			    and TestDrive = '10') MTTestDrive
	from #t3 a
	UNION
	select a.CompanyCode
		 , b.DealerAbbreviation
		 , a.BranchCode
		 , c.OutletAbbreviation
		 , isnull(e.GroupCode,'') TipeKendaraan
		 , isnull(e.TypeCode,'') Variant
		 , 0
		 , 0 
		 , 0
		 , 0
		 , 0
		 , 0
		 , COUNT(e.SalesModelCode)
		 , 0
		 , 0
	from OmTrSalesReqDetail a
	left join OmMstVehicle f on a.CompanyCode = f.CompanyCode
		and a.ChassisCode = f.ChassisCode
		and a.ChassisNo = f.ChassisNo
	left join OmMstModel e on e.CompanyCode = a.CompanyCode
		and e.SalesModelCode = f.SalesModelCode
	left join gnMstDealerMapping b on b.DealerCode = a.CompanyCode
	left join gnMstDealerOutletMapping c on c.DealerCode = a.CompanyCode
		and c.OutletCode = a.BranchCode
	where Convert(varchar,a.FakturPolisiDate,112) between convert(varchar,@StartDate,112) and convert(varchar,@EndDate,112)
	  and (b.Area like Case when @Area <> '' 
					then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
							then 'JABODETABEK'
							else @Area end
					else '%%' end
	   or b.Area like Case when @Area <> '' 
					then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
							then 'CABANG'
							else @Area end
					else '%%' end) 	
	  and b.DealerCode like case when @DealerCode = '' then '%%' else @DealerCode end
	  and c.OutletCode like case when @OutletCode = '' then '%%' else @OutletCode end
	  and not exists(select 1 
					   from #t3 d 
					  where d.DealerCode = a.CompanyCode
					    and d.OutletCode = a.BranchCode
					    and e.GroupCode = d.TipeKendaraan
					    and e.TypeCode = d.Variant
					    and e.TransmissionType = d.Transmisi)
	group by a.CompanyCode, b.DealerAbbreviation, a.BranchCode, c.OutletAbbreviation, e.GroupCode, e.TypeCode,e.TransmissionType

	drop table #t3
end

if(@DealerCode = '' and @OutletCode = '')
begin
	select TipeKendaraan
		 , Variant
		 , SUM(OutsINQ) OutsINQ
		 , SUM(NewINQ) NewINQ
		 , SUM(OutsSPK) OutsSPK
		 , SUM(NewSPK) NewSPK
		 , SUM(CancelSPK) CancelSPK
		 , SUM(FakturPolisi) FakturPolisi
		 , SUM(Balance) Balance
		 , SUM(ATTestDrive) ATTestDrive
		 , SUM(MTTestDrive) MTTestDrive
	from @MainTable
	group by TipeKendaraan, Variant
	having (SUM(MTTestDrive) + SUM(ATTestDrive) + SUM(OutsINQ) + SUM(NewINQ) + SUM(OutsSPK) + SUM(NewSPK)  + SUM(FakturPolisi) + SUM(Balance)) > 0
	order by TipeKendaraan
end

select distinct
DealerCode
	 , DealerAbbreviation
	 , OutletCode
	 , OutletAbbreviation
from @MainTable
group by DealerCode, DealerAbbreviation, OutletCode, OutletAbbreviation, TipeKendaraan, Variant, OutsINQ , NewINQ , OutsSPK, NewSPK, CancelSPK, FakturPolisi , ATTestDrive, MTTestDrive
having (SUM(MTTestDrive) + SUM(ATTestDrive) + SUM(NewINQ) + SUM(NewSPK) + SUM(OutsINQ) + SUM(OutsSPK) + SUM(FakturPolisi) + SUM(Balance)) > 0
order by DealerAbbreviation, OutletAbbreviation

select DealerCode
	 , DealerAbbreviation
	 , OutletCode
	 , OutletAbbreviation
	 , TipeKendaraan
	 , Variant
	 , OutsINQ 
	 , NewINQ
	 , OutsSPK
	 , NewSPK
	 , CancelSPK
	 , FakturPolisi
	 , case when SUM(Balance) < 0 then 0 else SUM(Balance) end Balance
	 , ATTestDrive
	 , MTTestDrive
from @MainTable
group by DealerCode, DealerAbbreviation, OutletCode, OutletAbbreviation, TipeKendaraan, Variant, OutsINQ , NewINQ , OutsSPK, NewSPK, CancelSPK, FakturPolisi , ATTestDrive, MTTestDrive
having (SUM(MTTestDrive) + SUM(ATTestDrive) + SUM(NewINQ) + SUM(NewSPK) + SUM(OutsINQ) + SUM(OutsSPK) + SUM(FakturPolisi) + SUM(Balance)) > 0
order by DealerAbbreviation, OutletAbbreviation, TipeKendaraan, Variant


GO

if object_id('uspfn_pmApplyMemberTransfer') is not null
	drop procedure uspfn_pmApplyMemberTransfer
GO
-- Created by Benedict 16 Mar 2015
CREATE PROCEDURE uspfn_pmApplyMemberTransfer
	@p0 varchar(20),  --CompanyCode
	@p1 varchar(20),  --BranchCode
	@p2 varchar(20),  --InquiryNumber IF 'S' || EmployeeID IF NOT 'S'
	@p3 varchar(20),  --EmployeeID IF 'S' || TeamLeader IF NOT 'S'
	@p4 varchar(20)	  --UserID

--DECLARE
--	@p0 varchar(20) = '6006406', --CompanyCode
--	@p1 varchar(20) = '6006401', --BranchCode
--	@p2 varchar(20) = '421468',  --InquiryNumber IF 'S' || EmployeeID IF NOT 'S'
--	@p3 varchar(20) = '52259',   --EmployeeID IF 'S' || TeamLeader IF NOT 'S'
--	@p4 varchar(20) = 'bent'     --UserID
AS
BEGIN

DECLARE @pos1 varchar(5) = (SELECT Position FROM HrEmployee WHERE CompanyCode = @p0 AND EmployeeID = @p3)
IF (@pos1 = 'S')
BEGIN
	IF (@p3 <> (SELECT TOP 1 EmployeeID 
					FROM pmKDP 
					WHERE CompanyCode = @p0
					AND BranchCode = @p1
					AND InquiryNumber = @p2))
	BEGIN
	DECLARE @spvID varchar(15) = (SELECT TeamLeader 
									FROM HrEmployee 
									WHERE CompanyCode = @p0 AND EmployeeID = @p3)
	UPDATE pmKDP
	SET EmployeeID = @p3,
		SpvEmployeeID = @spvID,
		OutletID = @p1,
		LastUpdateBy = @p4,
		LastUpdateDate = getdate()
	WHERE CompanyCode = @p0
		AND BranchCode = @p1
		AND InquiryNumber = @p2
	END
END
ELSE
	IF (@p3 <> (SELECT TeamLeader
				FROM HrEmployee
				WHERE CompanyCode = @p0 AND EmployeeID = @p2))
	BEGIN
		UPDATE HrEmployee
		SET TeamLeader = @p3,
			UpdatedBy = @p4,
			UpdatedDate = getdate()
		WHERE CompanyCode = @p0 AND EmployeeID = @p2
	END


END
GO

if object_id('usprpt_ItsInqAchievement') is not null
	drop procedure usprpt_ItsInqAchievement
GO
CREATE procedure [dbo].[usprpt_ItsInqAchievement]     
(    
 @CompanyCode  VARCHAR(15),    
 @BMEmployeeID  VARCHAR(15),    
 @SHEmployeeID  VARCHAR(15),    
 @SCEmployeeID  VARCHAR(15),    
 @SMEmployeeID  VARCHAR(15),    
 @Year    INT    
)    
AS    
  
--declare @companycode  varchar(15)    
--set @companycode = '6006406'  
--declare @bmemployeeid  varchar(15)  
--set @bmemployeeid = 'bm189'    
--declare @shemployeeid  varchar(15)   
--set @shemployeeid = '225'   
--declare @scemployeeid  varchar(15)  
--set @scemployeeid = '50395'    
--declare @smemployeeid  varchar(15)  
--set @smemployeeid ='%'  
--declare  @year    int    
--set @year = 2013  
  
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
   
   
-- ===========================================================    
-- ====== Searching for KDP based on parameters (List of Salesman) and PmMstTeamMembers,     
-- ====== Then Transpose to Monthly View ==========================    
-- ===========================================================     
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
    
--=======================================================================================    
-- SALES SOURCE OF DATA    
--=======================================================================================    
 select * into #SSD from (    
  select CompanyCode, '' BranchCode, TypeOf1, TypeOf2    
   , isnull(Jan, 0) Jan, isnull(Feb, 0) Feb, isnull(Mar, 0) Mar, isnull(Apr, 0) Apr, isnull(May, 0) May, isnull(Jun, 0) Jun    
   , isnull(Jul, 0) Jul, isnull(Aug, 0) Aug, isnull(Sep, 0) Sep, isnull(Oct, 0) Oct, isnull(Nov, 0) Nov, isnull(Dec, 0) Dec    
  from (    
   select kdp.CompanyCode, '' BranchCode, kdp.PerolehanData TypeOf1, '' TypeOf2    
    , substring(Convert(varchar, kdp.InquiryDate, 109), 1, 3) InquiryMonth, count(kdp.EmployeeID) InquiryCount    
   from PmKDP kdp    
   where kdp.CompanyCode = @CompanyCode and year(kdp.InquiryDate) = @Year    
    and kdp.BranchCode in (select BranchCode from #ListOfSalesman)          
    and kdp.EmployeeID in (select EmployeeID from #ListOfSalesman)    
   group by kdp.CompanyCode, kdp.PerolehanData, substring(Convert(varchar, kdp.InquiryDate, 109), 1, 3)) as Header    
  pivot    
  (    
   sum (inquiryCount)    
   for InquiryMonth in (Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, Dec)    
  ) pvt    
 ) #SSD    
    
--=======================================================================================    
-- SALES BY TYPE    
--=======================================================================================    
 select * into #ST from (    
  select CompanyCode, '' BranchCode, TypeOf1, TypeOf2    
   , isnull(Jan, 0) Jan, isnull(Feb, 0) Feb, isnull(Mar, 0) Mar, isnull(Apr, 0) Apr, isnull(May, 0) May, isnull(Jun, 0) Jun    
   , isnull(Jul, 0) Jul, isnull(Aug, 0) Aug, isnull(Sep, 0) Sep, isnull(Oct, 0) Oct, isnull(Nov, 0) Nov, isnull(Dec, 0) Dec    
  from (    
   select kdp.CompanyCode, '' BranchCode, kdp.TipeKendaraan TypeOf1, kdp.Variant TypeOf2    
    , substring(Convert(varchar, kdp.InquiryDate, 109), 1, 3) InquiryMonth    
    , count(kdp.EmployeeID) InquiryCount    
   from PmKDP kdp    
   where kdp.CompanyCode = @CompanyCode and year(kdp.InquiryDate) = @Year    
    and kdp.BranchCode in (select BranchCode from #ListOfSalesman)          
    and kdp.EmployeeID in (select EmployeeID from #ListOfSalesman)         
   group by kdp.CompanyCode, kdp.TipeKendaraan, kdp.Variant     
    , substring(Convert(varchar, kdp.InquiryDate, 109), 1, 3)) as Header    
  pivot    
  (    
   sum(InquiryCount)    
   for InquiryMonth in (Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, Dec)    
  ) pvt    
 ) #ST    
    
--=======================================================================================    
-- PROSPECT STATUS    
--=======================================================================================    
 select * into #PS from (    
  select CompanyCode, '' BranchCode, TypeOf1, TypeOf2    
   , isnull(Jan, 0) Jan, isnull(Feb, 0) Feb, isnull(Mar, 0) Mar, isnull(Apr, 0) Apr, isnull(May, 0) May, isnull(Jun, 0) Jun    
   , isnull(Jul, 0) Jul, isnull(Aug, 0) Aug, isnull(Sep, 0) Sep, isnull(Oct, 0) Oct, isnull(Nov, 0) Nov, isnull(Dec, 0) Dec     
  from (    
   select kdp.CompanyCode, '' BranchCode, kdp.LastProgress TypeOf1, '' TypeOf2    
    , substring(Convert(varchar, kdp.InquiryDate, 109), 1, 3) InquiryMonth    
    , count(kdp.EmployeeID) InquiryCount    
   from PmKDP kdp    
   where kdp.CompanyCode = @CompanyCode and year(kdp.InquiryDate) = @Year     
    and kdp.BranchCode in (select BranchCode from #ListOfSalesman)          
    and kdp.EmployeeID in (select EmployeeID from #ListOfSalesman)          
   group by kdp.CompanyCode, kdp.BranchCode, kdp.LastProgress    
    , substring(Convert(varchar, kdp.InquiryDate, 109), 1, 3)) as Header    
  pivot    
  (    
   sum(InquiryCount)    
   for InquiryMonth in (Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, Dec)    
  ) pvt    
 ) #PS    
   
 -- Salesman ----------    
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
   
 select      
    '1' inc, 'Sales Source of Data' initial, tempSSD.CompanyCode, '' BranchCode, lkpDtl.LookUpValue TypeOf1, tempSSD.TypeOf2    
   , isnull(tempSSD.Jan, 0) Jan, isnull(tempSSD.Feb, 0) Feb, isnull(tempSSD.Mar, 0) Mar    
   , isnull(tempSSD.Apr, 0) Apr, isnull(tempSSD.May, 0) May, isnull(tempSSD.Jun, 0) Jun    
   , isnull(tempSSD.Jul, 0) Jul, isnull(tempSSD.Aug, 0) Aug, isnull(tempSSD.Sep, 0) Sep    
   , isnull(tempSSD.Oct, 0) Oct, isnull(tempSSD.Nov, 0) Nov, isnull(tempSSD.Dec, 0) Dec    
   , isnull((tempSSD.Jan + tempSSD.Feb + tempSSD.Mar + tempSSD.Apr + tempSSD.May + tempSSD.Jun), 0) Sem1    
   , isnull((tempSSD.Jul + tempSSD.Aug + tempSSD.Sep + tempSSD.Oct + tempSSD.Nov+ tempSSD.Dec), 0) Sem2    
   , isnull((tempSSD.Jan + tempSSD.Feb + tempSSD.Mar + tempSSD.Apr + tempSSD.May + tempSSD.Jun    
    + tempSSD.Jul + tempSSD.Aug + tempSSD.Sep + tempSSD.Oct + tempSSD.Nov    
    + tempSSD.Dec), 0) Total    
  from     
   GnMstLookUpDtl lkpDtl    
    left join #SSD tempSSD on lkpDtl.CompanyCode = tempSSD.CompanyCode    
    and lkpDtl.LookUpValue = tempSSD.TypeOf1     
  where     
   lkpDtl.CodeID = 'PSRC' and lkpDtl.CompanyCode = @CompanyCode   
     
   select      
   '2' inc, 'Sales by Type' initial, tempST.CompanyCode, tempST.BranchCode, GTS.GroupCode TypeOf1, GTS.typeCode TypeOf2-- tempST.TypeOf1, tempST.TypeOf2    
   , isnull(tempST.Jan, 0) Jan, isnull(tempST.Feb, 0) Feb, isnull(tempST.Mar, 0) Mar    
   , isnull(tempST.Apr, 0) Apr, isnull(tempST.May, 0) May, isnull(tempST.Jun, 0) Jun    
   , isnull(tempST.Jul, 0) Jul, isnull(tempST.Aug, 0) Aug, isnull(tempST.Sep, 0) Sep    
   , isnull(tempST.Oct, 0) Oct, isnull(tempST.Nov, 0) Nov, isnull(tempST.Dec, 0) Dec    
   , isnull((tempST.Jan + tempST.Feb + tempST.Mar + tempST.Apr + tempST.May + tempST.Jun), 0) Sem1    
   , isnull((tempST.Jul + tempST.Aug + tempST.Sep + tempST.Oct + tempST.Nov+ tempST.Dec), 0) Sem2    
   , isnull((tempST.Jan + tempST.Feb + tempST.Mar + tempST.Apr + tempST.May + tempST.Jun    
    + tempST.Jul + tempST.Aug + tempST.Sep + tempST.Oct + tempST.Nov    
    + tempST.Dec), 0) Total    
  from     
   (select Distinct CompanyCode, GroupCode, TypeCode      
   from pmGroupTypeSeq     
   group by CompanyCode, GroupCode ,typeCode) GTS    
    left join #ST tempST on GTS.CompanyCode = tempST.CompanyCode and GTS.GroupCode = tempST.TypeOf1 and GTS.TypeCode = tempST.TypeOf2    
         
  select     
   '3' inc, 'Prospect Status' initial, tempPS.CompanyCode, tempPS.BranchCode, lkpDtl.LookUpValueName TypeOf1, tempPS.TypeOf2    
   , isnull(tempPS.Jan, 0) Jan, isnull(tempPS.Feb, 0) Feb, isnull(tempPS.Mar, 0) Mar    
   , isnull(tempPS.Apr, 0) Apr, isnull(tempPS.May, 0) May, isnull(tempPS.Jun, 0) Jun    
   , isnull(tempPS.Jul, 0) Jul, isnull(tempPS.Aug, 0) Aug, isnull(tempPS.Sep, 0) Sep    
   , isnull(tempPS.Oct, 0) Oct, isnull(tempPS.Nov, 0) Nov, isnull(tempPS.Dec, 0) Dec    
   , isnull((tempPS.Jan + tempPS.Feb + tempPS.Mar + tempPS.Apr + tempPS.May + tempPS.Jun), 0) Sem1    
   , isnull((tempPS.Jul + tempPS.Aug + tempPS.Sep + tempPS.Oct + tempPS.Nov+ tempPS.Dec), 0) Sem2    
   , isnull((tempPS.Jan + tempPS.Feb + tempPS.Mar + tempPS.Apr + tempPS.May + tempPS.Jun    
    + tempPS.Jul + tempPS.Aug + tempPS.Sep + tempPS.Oct + tempPS.Nov    
    + tempPS.Dec), 0) Total    
  from    
   GnMstLookUpDtl lkpDtl    
    left join #PS tempPS on lkpDtl.CompanyCode = tempPS.CompanyCode    
    and lkpDtl.LookUpValue = tempPS.TypeOf1     
  where lkpDtl.CodeID = 'PSTS' and lkpDtl.CompanyCode = @CompanyCode   
  
 drop table #TempSM    
 drop table #TempSC    
 drop table #TempSH    
 drop table #TempBM    
 drop table #SSD    
 drop table #ST    
 drop table #PS    
    
 drop table #ListOfSalesman     
 GO
 
 if object_id('uspfn_InquiryITSWithStatusByType') is not null
	drop procedure uspfn_InquiryITSWithStatusByType
GO
create PROCEDURE [dbo].[uspfn_InquiryITSWithStatusByType]
	@CompanyCode		varchar(15),
	@BranchCode			varchar(15),
	@StartDate			varchar(20),
	@EndDate			varchar(20),
	@LastStartDate		varchar(20),
	@LastEndDate		varchar(20),
	@Area				varchar(100),
	@GroupModel			varchar(100),
	@TipeKendaraan		varchar(100),
	@Variant			varchar(100),
	@Summary			bit
AS
begin

--exec uspfn_InquiryITSWithStatusByType '', '', '20140401', '20140415', '20140301', '20140328', '', '', '', '', 1

--Declare @CompanyCode	varchar(15)
--declare @BranchCOde		varchar(15)
--Declare @StartDate		varchar(20)
--Declare @EndDate		varchar(20)
--Declare @LastStartDate	varchar(20)
--Declare @LastEndDate	varchar(20)
--Declare @Area			varchar(100)
--declare @GroupModel		varchar(100)
--declare @TipeKendaraan	varchar(100)
--declare @Variant		varchar(100)
--declare @Summary		bit

--set @CompanyCode = ''
--set @BranchCode = ''
--set @StartDate = '20140401'
--set @EndDate = '20140415'
--set @LastStartDate = '20140301'
--set @LastEndDate = '20140328'
--set @Area = ''
--set @GroupModel = ''
--set @TipeKendaraan = ''
--set @Variant = ''
--set @Summary = 0

select * into #tThis from(
select c.CompanyCode, c.BranchCode 	
	, c.LastProgress
	, c.InquiryNumber
	, convert(varchar,c.UpdateDate,112)	UpdateDate
	, CASE 
		 WHEN day(c.UpdateDate) >= 1 and day(c.UpdateDate) <= 7  THEN 1
		 WHEN day(c.UpdateDate) >= 8 and day(c.UpdateDate) <= 14  THEN 2
		 WHEN day(c.UpdateDate) >= 15 and day(c.UpdateDate) <= 21  THEN 3
		 WHEN day(c.UpdateDate) >= 22 and day(c.UpdateDate) <= 28  THEN 4
		 WHEN day(c.UpdateDate) >= 29 and day(c.UpdateDate) <= 31  THEN 5
	   END WeekInt	
from SuzukiR4..pmStatusHistory c with (nolock, nowait) 
INNER JOIN SuzukiR4..pmHstITS a with (nolock, nowait)  ON -- penambahan 1 Apr 14
				a.CompanyCode = c.CompanyCode AND
				a.BranchCode = c.BranchCode AND
				a.InquiryNumber = c.InquiryNumber
where
	(case when c.CompanyCode='6015402' then '6015401' when c.CompanyCode='6051402' then '6051401' else c.CompanyCode end) like 
				case when @CompanyCode = ''       then '%%'  when @CompanyCode ='6015402' then '6015401' when @CompanyCode ='6051402' then '6051401' else @CompanyCode end		   
	and c.BranchCode  like case when @BranchCode='' then '%%' else @BranchCode end
	and c.LastProgress in (select LookUpValue from SuzukiR4..gnMstLookupDtl where CodeID = 'PSTS')	
	and (convert(varchar,c.UpdateDate,112) between @StartDate and @EndDate
	or (convert(varchar,c.UpdateDate,112) < @StartDate 
	and (c.LastProgress='P'   and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
											  where h.CompanyCode=c.CompanyCode
												and h.BranchCode=c.BranchCode
												and h.InquiryNumber=c.InquiryNumber
												and h.LastProgress<>'P'
												and convert(varchar,h.UpdateDate,112)<@StartDate)
	or  c.LastProgress='HP'  and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
											  where h.CompanyCode=c.CompanyCode
												and h.BranchCode=c.BranchCode
												and h.InquiryNumber=c.InquiryNumber
												and h.LastProgress not in ('P','HP')
												and convert(varchar,h.UpdateDate,112)<@StartDate)
	or  c.LastProgress='SPK' and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
											  where h.CompanyCode=c.CompanyCode
												and h.BranchCode=c.BranchCode
												and h.InquiryNumber=c.InquiryNumber
												and h.LastProgress not in ('P','HP','SPK')
												and convert(varchar,h.UpdateDate,112)<@StartDate))))	
union all
select   
h.CompanyCode
, h.BranchCode 
, 'SPK' LastProgress
, h.InquiryNumber
, convert(varchar,h.SPKDate,112)	SPKDate
, CASE 
	 WHEN day(h.SPKDate) >= 1 and day(h.SPKDate) <= 7  THEN 1
	 WHEN day(h.SPKDate) >= 8 and day(h.SPKDate) <= 14  THEN 2
	 WHEN day(h.SPKDate) >= 15 and day(h.SPKDate) <= 21  THEN 3
	 WHEN day(h.SPKDate) >= 22 and day(h.SPKDate) <= 28  THEN 4
	 WHEN day(h.SPKDate) >= 29 and day(h.SPKDate) <= 31  THEN 5
   END WeekInt	
from SuzukiR4..pmHstITS h
where 
(case when h.CompanyCode='6015402' then '6015401' when h.CompanyCode='6051402' then '6051401' else h.CompanyCode end) like 
				case when @CompanyCode = ''       then '%%'  when @CompanyCode ='6015402' then '6015401' when @CompanyCode ='6051402' then '6051401' else @CompanyCode end		   
	and h.BranchCode  like case when @BranchCode='' then '%%' else @BranchCode end
	and h.LastProgress in (select LookUpValue from SuzukiR4..gnMstLookupDtl where CodeID = 'PSTS')
 and convert(varchar,h.SPKDate,112) between @StartDate and @EndDate 
 and not exists (select top 1 1 from SuzukiR4..pmStatusHistory
                  where CompanyCode=h.CompanyCode
                    and BranchCode=h.BranchCode
                    and InquiryNumber=h.InquiryNumber)    
) #tThis order by CompanyCode, BranchCode

--select * into #tThis from(  --- penambahan 23 april 14
--select * from #tThis1
--union all
--select c.CompanyCode, c.BranchCode 	
--	, 'LOST' LastProgress
--	, c.InquiryNumber
--	, convert(varchar,c.UpdateDate,112)	UpdateDate
--	, CASE 
--		 WHEN day(c.UpdateDate) >= 1 and day(c.UpdateDate) <= 7  THEN 1
--		 WHEN day(c.UpdateDate) >= 8 and day(c.UpdateDate) <= 14  THEN 2
--		 WHEN day(c.UpdateDate) >= 15 and day(c.UpdateDate) <= 21  THEN 3
--		 WHEN day(c.UpdateDate) >= 22 and day(c.UpdateDate) <= 28  THEN 4
--		 WHEN day(c.UpdateDate) >= 29 and day(c.UpdateDate) <= 31  THEN 5
--	   END WeekInt	
--from SuzukiR4..pmStatusHistory c with (nolock, nowait) 
--inner join #tThis1 on #tThis1.CompanyCode = c.CompanyCode
--	and #tThis1.BranchCode = c.BranchCode
--	and #tThis1.InquiryNumber = c.InquiryNumber
--	and #tThis1.LastProgress = 'SPK' 
--where c.LastProgress = 'LOST'		
--and convert(varchar, #tThis1.UpdateDate, 112) between @StartDate and @EndDate
--) #tThis order by CompanyCode, BranchCode

select * into #tLast from(
select c.CompanyCode, c.BranchCode 
	, c.LastProgress
	, c.InquiryNumber		 
	, convert(varchar,c.UpdateDate,112)	UpdateDate
	, CASE 
		 WHEN day(c.UpdateDate) >= 1 and day(c.UpdateDate) <= 7  THEN 1
		 WHEN day(c.UpdateDate) >= 8 and day(c.UpdateDate) <= 14  THEN 2
		 WHEN day(c.UpdateDate) >= 15 and day(c.UpdateDate) <= 21  THEN 3
		 WHEN day(c.UpdateDate) >= 22 and day(c.UpdateDate) <= 28  THEN 4
		 WHEN day(c.UpdateDate) >= 29 and day(c.UpdateDate) <= 31  THEN 5
	   END WeekInt		   
from SuzukiR4..pmStatusHistory c with (nolock, nowait) 
INNER JOIN SuzukiR4..pmHstITS a WITH (NOLOCK, NOWAIT) ON -- penambahan 1 Apr 14
				a.CompanyCode = c.CompanyCode AND
				a.BranchCode = c.BranchCode AND
				a.InquiryNumber = c.InquiryNumber -- join Last Progress dihapus, 23 Apr 14
where
	(case when c.CompanyCode='6015402' then '6015401' when c.CompanyCode='6051402' then '6051401' else c.CompanyCode end) like 
				case when @CompanyCode = ''       then '%%'  when @CompanyCode ='6015402' then '6015401' when @CompanyCode ='6051402' then '6051401' else @CompanyCode end		   
	and c.BranchCode  like case when @BranchCode='' then '%%' else @BranchCode end
	and c.LastProgress in (select LookUpValue from SuzukiR4..gnMstLookupDtl where CodeID = 'PSTS')
	and (convert(varchar,c.UpdateDate,112) between @LastStartDate and @LastEndDate
	or (convert(varchar,c.UpdateDate,112) < @LastStartDate 
	and (c.LastProgress='P'   and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
											  where h.CompanyCode=c.CompanyCode
												and h.BranchCode=c.BranchCode
												and h.InquiryNumber=c.InquiryNumber
												and h.LastProgress<>'P'
												and convert(varchar,h.UpdateDate,112)<@LastStartDate)
	or  c.LastProgress='HP'  and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
											  where h.CompanyCode=c.CompanyCode
												and h.BranchCode=c.BranchCode
												and h.InquiryNumber=c.InquiryNumber
												and h.LastProgress not in ('P','HP')
												and convert(varchar,h.UpdateDate,112)<@LastStartDate)
	or  c.LastProgress='SPK' and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
											  where h.CompanyCode=c.CompanyCode
												and h.BranchCode=c.BranchCode
												and h.InquiryNumber=c.InquiryNumber
												and h.LastProgress not in ('P','HP','SPK')
												and convert(varchar,h.UpdateDate,112)<@LastStartDate))))	
union all
select   
h.CompanyCode
, h.BranchCode 
, 'SPK' LastProgress
, h.InquiryNumber
, convert(varchar,h.SPKDate,112) SPKDate
, CASE 
	 WHEN day(h.SPKDate) >= 1 and day(h.SPKDate) <= 7  THEN 1
	 WHEN day(h.SPKDate) >= 8 and day(h.SPKDate) <= 14  THEN 2
	 WHEN day(h.SPKDate) >= 15 and day(h.SPKDate) <= 21  THEN 3
	 WHEN day(h.SPKDate) >= 22 and day(h.SPKDate) <= 28  THEN 4
	 WHEN day(h.SPKDate) >= 29 and day(h.SPKDate) <= 31  THEN 5
   END WeekInt	
from SuzukiR4..pmHstITS h
where 
(case when h.CompanyCode='6015402' then '6015401' when h.CompanyCode='6051402' then '6051401' else h.CompanyCode end) like 
				case when @CompanyCode = ''       then '%%'  when @CompanyCode ='6015402' then '6015401' when @CompanyCode ='6051402' then '6051401' else @CompanyCode end		   
	and h.BranchCode  like case when @BranchCode='' then '%%' else @BranchCode end
	and h.LastProgress in (select LookUpValue from SuzukiR4..gnMstLookupDtl where CodeID = 'PSTS')
and convert(varchar,h.SPKDate,112) between @LastStartDate and @LastEndDate 
 and not exists (select top 1 1 from SuzukiR4..pmStatusHistory
                  where CompanyCode=h.CompanyCode
                    and BranchCode=h.BranchCode
                    and InquiryNumber=h.InquiryNumber)     																					
) #tLast order by CompanyCode, BranchCode

select * into #t1 from(
select 
	a.CompanyCode
	, a.BranchCode 
	, isnull((select TOP 1 GroupModel 
						 from SuzukiR4..msMstGroupModel with (nolock,nowait)
						where ModelType = b.TipeKendaraan),'OTHERS') TipeKendaraan
	, isnull(b.Variant,'') Variant
	, convert(varchar, b.InquiryDate, 112) InquiryDate
	, a.UpdateDate
	, a.LastProgress	
	, a.WeekInt
	, a.InquiryNumber
from #tThis a with (nolock, nowait)												
inner join SuzukiR4..pmHstITS b with (nolock, nowait) on b.CompanyCode = a.CompanyCode and b.BranchCode = a.BranchCode and b.InquiryNumber = a.InquiryNumber  
where
	(case when @TipeKendaraan <> '' then b.TipeKendaraan else @TipeKendaraan end) = @TipeKendaraan 
	and (case when @Variant <> '' then b.Variant else @Variant end) = @Variant 
	--and (convert(varchar, b.InquiryDate, 112) < @StartDate or convert(varchar, a.UpdateDate, 112) between @StartDate and @EndDate)
	) #t1
order by CompanyCode, BranchCode, TipeKendaraan, Variant
	
select * into #t4 from(
select 	a.CompanyCode
	, a.BranchCode 
	, isnull((select TOP 1 GroupModel 
						 from SuzukiR4..msMstGroupModel with (nolock,nowait)
						where ModelType = b.TipeKendaraan),'OTHERS') TipeKendaraan
	, isnull(b.Variant,'') Variant
	, convert(varchar, b.InquiryDate, 112) InquiryDate
	, a.UpdateDate
	, a.LastProgress
	, a.WeekInt	 
	, a.InquiryNumber
from #tLast a with (nolock, nowait) 
left join SuzukiR4..pmHstITS b with (nolock, nowait) on b.CompanyCode = a.CompanyCode and b.BranchCode = a.BranchCode and b.InquiryNumber = a.InquiryNumber  
where
	(case when @TipeKendaraan <> '' then b.TipeKendaraan else @TipeKendaraan end) = @TipeKendaraan 
	and (case when @Variant <> '' then b.Variant else @Variant end) = @Variant 
	--and (convert(varchar, b.InquiryDate, 112) < @LastStartDate or convert(varchar, a.UpdateDate, 112) between @LastStartDate and @LastEndDate)
	) #t4 order by CompanyCode, BranchCode, TipeKendaraan, Variant

select * into #tVehicle from(
select distinct CompanyCode, BranchCode, TipeKendaraan
from #t1 
where (case when @GroupModel <> '' then TipeKendaraan else @GroupModel end) = @GroupModel
group by CompanyCode, BranchCode, TipeKendaraan
union all
select distinct CompanyCode, BranchCode, TipeKendaraan
from #t4 
where (case when @GroupModel <> '' then TipeKendaraan else @GroupModel end) = @GroupModel
group by CompanyCode, BranchCode, TipeKendaraan
) #tVehicle

select * into #t2 from(
select SeqNo, LookupValue LastProgress
from SuzukiR4..gnMstLookupDtl
where CodeID = 'PSTS'
) #t2 order by SeqNo
	
select * into #t3 from(
select 1 WeekInt union select 2 WeekInt union select 3 WeekInt union select 4 WeekInt union select 5 WeekInt
) #t3
	
select * into #tUnion1 from(
select CompanyCode, BranchCode, TipeKendaraan--, InquiryNumber
, LastProgress 

-- Outs
, (select count(*) from #t1
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan 
	and LastProgress = a.LastProgress and InquiryDate < @StartDate and UpdateDate < @StartDate ) SaldoAwal
, (select count(*) from #t1 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan 
	and LastProgress = a.LastProgress and WeekInt = 1 and InquiryDate < @StartDate and UpdateDate between @StartDate and @EndDate) WeekOuts1	
, (select count(*) from #t1 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan 
	and LastProgress = a.LastProgress and WeekInt = 2 and InquiryDate < @StartDate and UpdateDate between @StartDate and @EndDate) WeekOuts2		
, (select count(*) from #t1 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan 
	and LastProgress = a.LastProgress and WeekInt = 3 and InquiryDate < @StartDate and UpdateDate between @StartDate and @EndDate) WeekOuts3		
, (select count(*) from #t1 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan 
	and LastProgress = a.LastProgress and WeekInt = 4 and InquiryDate < @StartDate and UpdateDate between @StartDate and @EndDate) WeekOuts4
,(select count(*) from #t1 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan 
	and LastProgress = a.LastProgress and WeekInt = 5 and InquiryDate < @StartDate and UpdateDate between @StartDate and @EndDate) WeekOuts5
,(select count(*) from #t1 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan 
	and LastProgress = a.LastProgress and InquiryDate < @StartDate and UpdateDate between @StartDate and @EndDate) TotalWeekOuts

-- New
,(select count(*) from #t1 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan 
	and LastProgress = a.LastProgress 
	and WeekInt = 1 and UpdateDate between @StartDate and @EndDate 
	and (InquiryDate between @StartDate and @EndDate or InquiryDate > @EndDate)) Week1	
, (select count(*) from #t1 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan 
	and LastProgress = a.LastProgress 
	and WeekInt = 2 and UpdateDate between @StartDate and @EndDate 
	and (InquiryDate between @StartDate and @EndDate or InquiryDate > @EndDate)) Week2		
, (select count(*) from #t1 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan 
	and LastProgress = a.LastProgress 
	and WeekInt = 3 and UpdateDate between @StartDate and @EndDate 
	and (InquiryDate between @StartDate and @EndDate or InquiryDate > @EndDate)) Week3		
, (select count(*) from #t1 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan 
	and LastProgress = a.LastProgress 
	and WeekInt = 4 and UpdateDate between @StartDate and @EndDate 
	and (InquiryDate between @StartDate and @EndDate or InquiryDate > @EndDate)) Week4
,(select count(*) from #t1 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan 
	and LastProgress = a.LastProgress 
	and WeekInt = 5 and UpdateDate between @StartDate and @EndDate 
	and (InquiryDate between @StartDate and @EndDate or InquiryDate > @EndDate)) Week5
, (select count(*) from #t1 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan 
	and LastProgress = a.LastProgress
	and UpdateDate between @StartDate and @EndDate 
	and (InquiryDate between @StartDate and @EndDate or InquiryDate > @EndDate)) TotalWeek
from #t1 a
group by CompanyCode, BranchCode, TipeKendaraan, LastProgress--, InquiryNumber
) #tUnion1

select * into #tUnion2 from(
select CompanyCode, BranchCode, TipeKendaraan
, LastProgress 

-- Outs
, (select count(*) from #t4
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan 
	and LastProgress = a.LastProgress and InquiryDate < @LastStartDate and UpdateDate < @LastStartDate) SaldoAwalLast
, (select count(*) from #t4 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan 
	and LastProgress = a.LastProgress and WeekInt = 1 and InquiryDate < @LastStartDate and UpdateDate between @LastStartDate and @LastEndDate) WeekOuts1Last	
, (select count(*) from #t4 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan  
	and LastProgress = a.LastProgress and WeekInt = 2 and InquiryDate < @LastStartDate and UpdateDate between @LastStartDate and @LastEndDate) WeekOuts2Last		
, (select count(*) from #t4 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan  
	and LastProgress = a.LastProgress and WeekInt = 3 and InquiryDate < @LastStartDate and UpdateDate between @LastStartDate and @LastEndDate) WeekOuts3Last		
, (select count(*) from #t4 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan  
	and LastProgress = a.LastProgress and WeekInt = 4 and InquiryDate < @LastStartDate and UpdateDate between @LastStartDate and @LastEndDate) WeekOuts4Last
, (select count(*) from #t4 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan 
	and LastProgress = a.LastProgress and WeekInt = 5 and InquiryDate < @LastStartDate and UpdateDate between @LastStartDate and @LastEndDate) WeekOuts5Last
, (select count(*) from #t4 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan  
	and LastProgress = a.LastProgress and InquiryDate < @LastStartDate and UpdateDate between @LastStartDate and @LastEndDate) TotalWeekOutsLast

-- New
, (select count(*) from #t4 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan 
	and LastProgress = a.LastProgress and WeekInt = 1  
	and UpdateDate between @LastStartDate and @LastEndDate
	and (InquiryDate between @LastStartDate and @LastEndDate or InquiryDate > @LastEndDate)) Week1Last	-- perhitungan InquiryDate between dihapus (23 apr 14)
, (select count(*) from #t4 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan 
	and LastProgress = a.LastProgress and WeekInt = 2 
	and UpdateDate between @LastStartDate and @LastEndDate
	and (InquiryDate between @LastStartDate and @LastEndDate or InquiryDate > @LastEndDate)) Week2Last		
, (select count(*) from #t4 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan 
	and LastProgress = a.LastProgress and WeekInt = 3 
	and UpdateDate between @LastStartDate and @LastEndDate
	and (InquiryDate between @LastStartDate and @LastEndDate or InquiryDate > @LastEndDate)) Week3Last		
, (select count(*) from #t4
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan  
	and LastProgress = a.LastProgress and WeekInt = 4 
	and UpdateDate between @LastStartDate and @LastEndDate
	and (InquiryDate between @LastStartDate and @LastEndDate or InquiryDate > @LastEndDate)) Week4Last
, (select count(*) from #t4
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan  
	and LastProgress = a.LastProgress and WeekInt = 5 
	and UpdateDate between @LastStartDate and @LastEndDate
	and (InquiryDate between @LastStartDate and @LastEndDate or InquiryDate > @LastEndDate)) Week5Last
,(select count(*) from #t4
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan
	and LastProgress = a.LastProgress 
	and UpdateDate between @LastStartDate and @LastEndDate
	and (InquiryDate between @LastStartDate and @LastEndDate or InquiryDate > @LastEndDate)) TotalWeekLast
		
from #t4 a

group by CompanyCode, BranchCode, TipeKendaraan
, LastProgress 	
) #tUnion2
			
-- This Month
select * into #tGabung from(	
select distinct
	a.GroupNo
	, a.Area
	, a.DealerCode CompanyCode
	, a.DealerAbbreviation CompanyName
	, b.OutletCode BranchCode 
	, b.OutletAbbreviation BranchName
	, d.TipeKendaraan
	
	, e.SeqNo
	, e.LastProgress		
	
	, isnull(g.SaldoAwal, 0) SaldoAwal
	, isnull(g.WeekOuts1, 0) WeekOuts1
	, isnull(g.WeekOuts2, 0) WeekOuts2
	, isnull(g.WeekOuts3, 0) WeekOuts3
	, isnull(g.WeekOuts4, 0) WeekOuts4
	, isnull(g.WeekOuts5, 0) WeekOuts5
	, isnull(g.TotalWeekOuts, 0) TotalWeekOuts
	
	, isnull(g.Week1, 0) Week1
	, isnull(g.Week2, 0) Week2
	, isnull(g.Week3, 0) Week3
	, isnull(g.Week4, 0) Week4
	, isnull(g.Week5, 0) Week5
	, isnull(g.TotalWeek, 0) TotalWeek
	
	, isnull(g.TotalWeekOuts, 0) + isnull(g.TotalWeek, 0) Total
	
	, isnull(h.SaldoAwalLast, 0) SaldoAwalLast
	, isnull(h.WeekOuts1Last, 0) WeekOuts1Last
	, isnull(h.WeekOuts2Last, 0) WeekOuts2Last
	, isnull(h.WeekOuts3Last, 0) WeekOuts3Last
	, isnull(h.WeekOuts4Last, 0) WeekOuts4Last
	, isnull(h.WeekOuts5Last, 0) WeekOuts5Last
	, isnull(h.TotalWeekOutsLast, 0) TotalWeekOutsLast
	
	, isnull(h.Week1Last, 0) Week1Last
	, isnull(h.Week2Last, 0) Week2Last
	, isnull(h.Week3Last, 0) Week3Last
	, isnull(h.Week4Last, 0) Week4Last
	, isnull(h.Week5Last, 0) Week5Last
	, isnull(h.TotalWeekLast, 0) TotalWeekLast
	
	, isnull(h.TotalWeekOutsLast, 0) + isnull(h.TotalWeekLast, 0) TotalLast
	
	,cast((case when isnull(h.TotalWeekOutsLast, 0) + isnull(h.TotalWeekLast, 0) = 0 then 0 else (isnull(g.TotalWeekOuts, 0) + isnull(g.TotalWeek, 0))/ cast((isnull(h.TotalWeekOutsLast, 0) + isnull(h.TotalWeekLast, 0)) as decimal(38,2)) end) * 100 as decimal(8,1)) TotPercent

from SuzukiR4..gnMstDealerMapping a with (nolock, nowait)
left join SuzukiR4..gnMstDealerOutletMapping b with (nolock, nowait) on a.DealerCode = b.DealerCode
inner join #tVehicle d with (nolock, nowait) on d.CompanyCode = a.DealerCode and d.BranchCode = b.OutletCode
left join #t2 e with (nolock, nowait) on e.SeqNo > 0 
left join #tUnion1 g with (nolock, nowait) on g.CompanyCode = a.DealerCode and g.BranchCode = b.OutletCode and g.TipeKendaraan =  d.TipeKendaraan and g.LastProgress = e.LastProgress
left join #tUnion2 h with (nolock, nowait) on h.CompanyCode = a.DealerCode and h.BranchCode = b.OutletCode and h.TipeKendaraan =  d.TipeKendaraan and h.LastProgress = e.LastProgress
where 
	a.IsActive = 1
	and (a.Area like Case when @Area <> '' then case when (@Area = 'JABODETABEK' or @Area = 'CABANG') then 'JABODETABEK' else @Area end else '%%' end
			or  a.Area like Case when @Area <> '' then case when (@Area='JABODETABEK' or @Area='CABANG') then 'CABANG' else @Area end else '%%' end)		  
	and (case when a.DealerCode='6015402' then '6015401' when a.DealerCode='6051402' then '6051401' else a.DealerCode end) like 
				case when @CompanyCode = '' then '%%'  when @CompanyCode ='6015402' then '6015401' when @CompanyCode ='6051402' then '6051401' else @CompanyCode end		   
	and b.OutletCode  like case when @BranchCode='' then '%%' else @BranchCode end
	) #tGabung
order by GroupNo, CompanyCode, BranchCode, TipeKendaraan, SeqNo
	
-- Level 0
select * into #tFinal from(
select 
	1 OrderNo3
	, 0 OrderNo
	, 0 OrderNo1
	, 0 OrderNo2
	
	,  GroupNo
	,  Area
	,  CompanyCode
	,  CompanyName
	,  BranchCode 
	,  BranchName
	,  TipeKendaraan
	,  SeqNo
	,  LastProgress		
	
	, SaldoAwalLast	
	, WeekOuts1Last	
	, WeekOuts2Last		
	, WeekOuts3Last		
	, WeekOuts4Last
	, WeekOuts5Last
	, TotalWeekOutsLast
	, Week1Last
	, Week2Last
	, Week3Last
	, Week4Last
	, Week5Last
	, TotalWeekLast
	, TotalLast
	
	,  SaldoAwal	
	,  WeekOuts1	
	,  WeekOuts2		
	,  WeekOuts3		
	,  WeekOuts4
	,  WeekOuts5
	,  TotalWeekOuts
	,  Week1
	,  Week2
	,  Week3
	,  Week4
	,  Week5
	,  TotalWeek	
	,  Total
	
	, TotPercent

	from #tGabung
union
-- Group per Tipe Kendaraan
select 
	1 OrderNo3
	, 1 OrderNo
	, 0 OrderNo1
	, 0 OrderNo2
	
	,  GroupNo
	,  Area
	,  CompanyCode
	,  CompanyName
	,  '' BranchCode 
	,  '' BranchName
	,  TipeKendaraan
	,  SeqNo
	,  LastProgress		
	
	, sum(SaldoAwalLast) SaldoAwalLast	
	, sum(WeekOuts1Last) WeekOuts1Last	
	, sum(WeekOuts2Last) WeekOuts2Last	
	, sum(WeekOuts3Last) WeekOuts3Last		
	, sum(WeekOuts4Last) WeekOuts4Last
	, sum(WeekOuts5Last) WeekOuts5Last
	, sum(TotalWeekOutsLast) TotalWeekOutsLast
	, sum(Week1Last) Week1Last
	, sum(Week2Last) Week2Last
	, sum(Week3Last) Week3Last
	, sum(Week4Last) Week4Last
	, sum(Week5Last) Week5Last
	, sum(TotalWeekLast) TotalWeekLast
	, sum(TotalLast) TotalLast
	
	,  sum(SaldoAwal) SaldoAwal
	,  sum(WeekOuts1) WeekOuts1
	,  sum(WeekOuts2) WeekOuts2
	,  sum(WeekOuts3) WeekOuts3
	,  sum(WeekOuts4) WeekOuts4
	,  sum(WeekOuts5) WeekOuts5
	,  sum(TotalWeekOuts) TotalWeekOuts
	,  sum(Week1) Week1
	,  sum(Week2) Week2
	,  sum(Week3) Week3
	,  sum(Week4) Week4
	,  sum(Week5) Week5
	,  sum(TotalWeek) TotalWeek
	,  sum(Total) Total
	
	,  cast((case when sum(TotalLast) = 0 then 0 else sum(Total)/ cast(sum(TotalLast) as decimal(38,2)) end) * 100 as decimal(8,1)) TotPercent 

	from #tGabung
group by 
	 GroupNo
	,   Area
	,   CompanyCode
	,   CompanyName
	,   BranchCode 
	,   BranchName
	,   TipeKendaraan	
	,   SeqNo
	,   LastProgress	
union
-- Group per Company
select 
	1 OrderNo3
	, 1 OrderNo
	, 1 OrderNo1
	, 0 OrderNo2

	,  GroupNo
	,  Area
	,  '' CompanyCode
	,  'ZTOTAL' CompanyName
	,  '' BranchCode 
	,  '' BranchName
	,  TipeKendaraan
	,  SeqNo
	,  LastProgress		
	
	, sum(SaldoAwalLast) SaldoAwalLast	
	, sum(WeekOuts1Last) WeekOuts1Last	
	, sum(WeekOuts2Last) WeekOuts2Last	
	, sum(WeekOuts3Last) WeekOuts3Last		
	, sum(WeekOuts4Last) WeekOuts4Last
	, sum(WeekOuts5Last) WeekOuts5Last
	, sum(TotalWeekOutsLast) TotalWeekOutsLast
	, sum(Week1Last) Week1Last
	, sum(Week2Last) Week2Last
	, sum(Week3Last) Week3Last
	, sum(Week4Last) Week4Last
	, sum(Week5Last) Week5Last
	, sum(TotalWeekLast) TotalWeekLast
	, sum(TotalLast) TotalLast
	
	,  sum(SaldoAwal) SaldoAwal
	,  sum(WeekOuts1) WeekOuts1
	,  sum(WeekOuts2) WeekOuts2
	,  sum(WeekOuts3) WeekOuts3
	,  sum(WeekOuts4) WeekOuts4
	,  sum(WeekOuts5) WeekOuts5
	,  sum(TotalWeekOuts) TotalWeekOuts
	,  sum(Week1) Week1
	,  sum(Week2) Week2
	,  sum(Week3) Week3
	,  sum(Week4) Week4
	,  sum(Week5) Week5
	,  sum(TotalWeek) TotalWeek
	,  sum(Total) Total
	
	,  cast((case when sum(TotalLast) = 0 then 0 else sum(Total)/ cast(sum(TotalLast) as decimal(38,2)) end) * 100 as decimal(8,1)) TotPercent 

	from #tGabung
group by  GroupNo
	,   Area	
	,  TipeKendaraan
	,   SeqNo
	,   LastProgress				
union
-- Group per Area
select 
	1 OrderNo3
	, 1 OrderNo
	, 1 OrderNo1
	, 1 OrderNo2
	
	,  999999 GroupNo	
	,  'TOTAL' Area	
	,  'TOTAL' CompanyCode
	,  'ZTOTAL' CompanyName	
	,  '' BranchCode 
	,  '' BranchName		
	,  TipeKendaraan
	,  SeqNo
	,  LastProgress		
	
	, sum(SaldoAwalLast) SaldoAwalLast	
	, sum(WeekOuts1Last) WeekOuts1Last	
	, sum(WeekOuts2Last) WeekOuts2Last	
	, sum(WeekOuts3Last) WeekOuts3Last		
	, sum(WeekOuts4Last) WeekOuts4Last
	, sum(WeekOuts5Last) WeekOuts5Last
	, sum(TotalWeekOutsLast) TotalWeekOutsLast
	, sum(Week1Last) Week1Last
	, sum(Week2Last) Week2Last
	, sum(Week3Last) Week3Last
	, sum(Week4Last) Week4Last
	, sum(Week5Last) Week5Last
	, sum(TotalWeekLast) TotalWeekLast
	, sum(TotalLast) TotalLast
	
	,  sum(SaldoAwal) SaldoAwal
	,  sum(WeekOuts1) WeekOuts1
	,  sum(WeekOuts2) WeekOuts2
	,  sum(WeekOuts3) WeekOuts3
	,  sum(WeekOuts4) WeekOuts4
	,  sum(WeekOuts5) WeekOuts5
	,  sum(TotalWeekOuts) TotalWeekOuts
	,  sum(Week1) Week1
	,  sum(Week2) Week2
	,  sum(Week3) Week3
	,  sum(Week4) Week4
	,  sum(Week5) Week5
	,  sum(TotalWeek) TotalWeek
	,  sum(Total) Total
	
	,  cast((case when sum(TotalLast) = 0 then 0 else sum(Total)/ cast(sum(TotalLast) as decimal(38,2)) end) * 100 as decimal(8,1)) TotPercent 

	from #tGabung
group by TipeKendaraan
	,  SeqNo
	,  LastProgress
union
-- TOTAL
select 
	2 OrderNo3
	, 1 OrderNo
	, 1 OrderNo1
	, 2 OrderNo2
	
	,  0 GroupNo	
	,  'TOTAL' Area	
	,  '' CompanyCode
	,  'ZTOTAL' CompanyName	
	,  '' BranchCode 
	,  '' BranchName		
	,  'TOTAL' TipeKendaraan
	,  SeqNo
	,  LastProgress		
	
	, sum(SaldoAwalLast) SaldoAwalLast	
	, sum(WeekOuts1Last) WeekOuts1Last	
	, sum(WeekOuts2Last) WeekOuts2Last	
	, sum(WeekOuts3Last) WeekOuts3Last		
	, sum(WeekOuts4Last) WeekOuts4Last
	, sum(WeekOuts5Last) WeekOuts5Last
	, sum(TotalWeekOutsLast) TotalWeekOutsLast
	, sum(Week1Last) Week1Last
	, sum(Week2Last) Week2Last
	, sum(Week3Last) Week3Last
	, sum(Week4Last) Week4Last
	, sum(Week5Last) Week5Last
	, sum(TotalWeekLast) TotalWeekLast
	, sum(TotalLast) TotalLast
	
	,  sum(SaldoAwal) SaldoAwal
	,  sum(WeekOuts1) WeekOuts1
	,  sum(WeekOuts2) WeekOuts2
	,  sum(WeekOuts3) WeekOuts3
	,  sum(WeekOuts4) WeekOuts4
	,  sum(WeekOuts5) WeekOuts5
	,  sum(TotalWeekOuts) TotalWeekOuts
	,  sum(Week1) Week1
	,  sum(Week2) Week2
	,  sum(Week3) Week3
	,  sum(Week4) Week4
	,  sum(Week5) Week5
	,  sum(TotalWeek) TotalWeek
	,  sum(TotalLast) + sum(TotalWeek) Total
	
	,  cast((case when sum(TotalLast) = 0 then 0 else sum(Total)/ cast(sum(TotalLast) as decimal(38,2)) end) * 100 as decimal(8,1)) TotPercent 

	from #tGabung
group by SeqNo
	,  LastProgress) #tFinal

if(@Summary = 0)
begin	
select 
		  OrderNo3
		 ,  OrderNo
		 ,  OrderNo1
		 ,  OrderNo2
		 ,  GroupNo
		 ,  Area
		 ,  CompanyCode
		 ,  CompanyName
		 ,  BranchCode
		 ,  BranchName
		 ,  SeqNo
		 ,  LastProgress		 		
		 
		 ---- LAST MONTH	
		 , sum(SaldoAwalLast) SaldoAwalLast
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(WeekOuts1Last) END WeekOuts1Last
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(WeekOuts2Last) END WeekOuts2Last  
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(WeekOuts3Last) END WeekOuts3Last
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(WeekOuts4Last) END WeekOuts4Last
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(WeekOuts5Last) END WeekOuts5Last		 
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(TotalWeekOutsLast) END TotalWeekOutsLast
		 , sum(Week1Last) Week1Last
		 , sum(Week2Last) Week2Last
		 , sum(Week3Last) Week3Last
		 , sum(Week4Last) Week4Last
		 , sum(Week5Last) Week5Last
		 , sum(TotalWeekLast) TotalWeekLast
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(TotalWeekOutsLast) END + sum(TotalWeekLast) TotalLast	 
		 
		 ---- THIS MONTH	
		 , sum(SaldoAwal) SaldoAwal
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(WeekOuts1) END WeekOuts1
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(WeekOuts2) END WeekOuts2
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(WeekOuts3) END WeekOuts3
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(WeekOuts4) END WeekOuts4
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(WeekOuts5) END WeekOuts5		 
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(TotalWeekOuts) END TotalWeekOuts
		 , sum(Week1) Week1
		 , sum(Week2) Week2 
		 , sum(Week3) Week3
		 , sum(Week4) Week4
		 , sum(Week5) Week5
		 , sum(TotalWeek) TotalWeek
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(TotalWeekOuts) END + sum(TotalWeek) Total
		 
		 , cast((case when (CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(TotalWeekOutsLast) END + sum(TotalWeekLast)) = 0 then 0 else 
		   (CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(TotalWeekOuts) END + sum(TotalWeek))/ cast((CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(TotalWeekOutsLast) END + sum(TotalWeekLast)) as decimal(38,2)) end) * 100 as decimal(8,1)) TotPercent
 from #tFinal
 where OrderNo2 <> 1
 group by  OrderNo3
		 ,  OrderNo
		 ,  OrderNo1
		 ,  OrderNo2
		 ,  GroupNo
		 ,  Area
		 ,  CompanyCode
		 ,  CompanyName
		 ,  BranchCode
		 ,  BranchName
		 ,  SeqNo
		 ,  LastProgress	
	order by OrderNo3 Asc,GroupNo asc, CompanyName asc,Area asc,OrderNo1 asc,OrderNo asc,BranchCode asc,OrderNo2 Asc,SeqNo Asc

select 
		  OrderNo3
		 ,  OrderNo
		 ,  OrderNo1
		 ,  OrderNo2
		 ,  GroupNo
		 ,  Area
		 ,  CompanyCode
		 ,  CompanyName
		 ,  BranchCode
		 ,  BranchName
		 ,  TipeKendaraan
		 ,  SeqNo
		 ,  LastProgress		 		
		 
		 ---- LAST MONTH	
		 , sum(SaldoAwalLast) SaldoAwalLast
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(WeekOuts1Last) END WeekOuts1Last
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(WeekOuts2Last) END WeekOuts2Last  
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(WeekOuts3Last) END WeekOuts3Last
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(WeekOuts4Last) END WeekOuts4Last
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(WeekOuts5Last) END WeekOuts5Last		 
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(TotalWeekOutsLast) END TotalWeekOutsLast
		 , sum(Week1Last) Week1Last
		 , sum(Week2Last) Week2Last
		 , sum(Week3Last) Week3Last
		 , sum(Week4Last) Week4Last
		 , sum(Week5Last) Week5Last
		 , sum(TotalWeekLast) TotalWeekLast
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(TotalWeekOutsLast) END + sum(TotalWeekLast) TotalLast	 
		 
		 ---- THIS MONTH	
		 , sum(SaldoAwal) SaldoAwal
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(WeekOuts1) END WeekOuts1
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(WeekOuts2) END WeekOuts2
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(WeekOuts3) END WeekOuts3
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(WeekOuts4) END WeekOuts4
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(WeekOuts5) END WeekOuts5		 
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(TotalWeekOuts) END TotalWeekOuts
		 , sum(Week1) Week1
		 , sum(Week2) Week2 
		 , sum(Week3) Week3
		 , sum(Week4) Week4
		 , sum(Week5) Week5
		 , sum(TotalWeek) TotalWeek
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(TotalWeekOuts) END + sum(TotalWeek) Total
		 
		 , cast((case when (CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(TotalWeekOutsLast) END + sum(TotalWeekLast)) = 0 then 0 else 
		   (CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(TotalWeekOuts) END + sum(TotalWeek))/ cast((CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(TotalWeekOutsLast) END + sum(TotalWeekLast)) as decimal(38,2)) end) * 100 as decimal(8,1)) TotPercent
 from #tFinal
 where GroupNo <> 2 and OrderNo2 <> 2
 group by  OrderNo3
		 ,  OrderNo
		 ,  OrderNo1
		 ,  OrderNo2
		 ,  GroupNo
		 ,  Area
		 ,  CompanyCode
		 ,  CompanyName
		 ,  BranchCode
		 ,  BranchName
		 ,  TipeKendaraan
		 ,  SeqNo
		 ,  LastProgress
	 order by  OrderNo3 Asc,  TipeKendaraan Asc, GroupNo asc,  CompanyName asc, OrderNo asc, OrderNo1 asc, BranchCode asc, OrderNo2 Asc, Area asc, SeqNo Asc 	 
end
else if (@Summary = 1)
begin
	select 
		  OrderNo3
		 ,  OrderNo
		 ,  OrderNo1
		 ,  OrderNo2
		 ,  GroupNo
		 ,  Area
		 ,  CompanyCode
		 ,  CompanyName
		 ,  BranchCode
		 ,  BranchName
		 ,  TipeKendaraan
		 ,  SeqNo
		 ,  LastProgress		 		
		 
		 ---- LAST MONTH	
		 , sum(SaldoAwalLast) SaldoAwalLast
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(WeekOuts1Last) END WeekOuts1Last
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(WeekOuts2Last) END WeekOuts2Last  
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(WeekOuts3Last) END WeekOuts3Last
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(WeekOuts4Last) END WeekOuts4Last
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(WeekOuts5Last) END WeekOuts5Last		 
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(TotalWeekOutsLast) END TotalWeekOutsLast
		 , sum(Week1Last) Week1Last
		 , sum(Week2Last) Week2Last
		 , sum(Week3Last) Week3Last
		 , sum(Week4Last) Week4Last
		 , sum(Week5Last) Week5Last
		 , sum(TotalWeekLast) TotalWeekLast
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(TotalWeekOutsLast) END + sum(TotalWeekLast) TotalLast	 
		 
		 ---- THIS MONTH	
		 , sum(SaldoAwal) SaldoAwal
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(WeekOuts1) END WeekOuts1
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(WeekOuts2) END WeekOuts2
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(WeekOuts3) END WeekOuts3
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(WeekOuts4) END WeekOuts4
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(WeekOuts5) END WeekOuts5		 
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(TotalWeekOuts) END TotalWeekOuts
		 , sum(Week1) Week1
		 , sum(Week2) Week2 
		 , sum(Week3) Week3
		 , sum(Week4) Week4
		 , sum(Week5) Week5
		 , sum(TotalWeek) TotalWeek
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(TotalWeekOuts) END + sum(TotalWeek) Total
		 
		 , cast((case when (CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(TotalWeekOutsLast) END + sum(TotalWeekLast)) = 0 then 0 else 
		   (CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(TotalWeekOuts) END + sum(TotalWeek))/ cast((CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(TotalWeekOutsLast) END + sum(TotalWeekLast)) as decimal(38,2)) end) * 100 as decimal(8,1)) TotPercent
 from #tFinal
 group by  OrderNo3
		 ,  OrderNo
		 ,  OrderNo1
		 ,  OrderNo2
		 ,  GroupNo
		 ,  Area
		 ,  CompanyCode
		 ,  CompanyName
		 ,  BranchCode
		 ,  BranchName
		 ,  TipeKendaraan
		 ,  SeqNo
		 ,  LastProgress
	 order by  OrderNo3 Asc,  TipeKendaraan Asc, GroupNo asc,  CompanyName asc, OrderNo asc, OrderNo1 asc, BranchCode asc, OrderNo2 Asc, Area asc, SeqNo Asc 	 
end

drop table #tThis,  #t1, #t2, #t3, #t4, #tLast, #tUnion1, #tUnion2, #tVehicle, #tGabung, #tFinal

end
GO

if object_id('uspfn_InquiryITSWithStatusByDealer') is not null
	drop procedure uspfn_InquiryITSWithStatusByDealer
GO
create PROCEDURE [dbo].[uspfn_InquiryITSWithStatusByDealer]
	@CompanyCode		varchar(15),
	@BranchCode			varchar(15),
	@StartDate			varchar(20),
	@EndDate			varchar(20),
	@LastStartDate		varchar(20),
	@LastEndDate		varchar(20),
	@Area				varchar(100),
	@GroupModel			varchar(100),
	@TipeKendaraan		varchar(100),
	@Variant			varchar(100),
	@Summary			bit
AS
begin

--exec uspfn_InquiryITSWithStatusByDealer '', '', '20140401', '20140415', '20140301', '20140328', '', '', '', '', 0

--Declare @CompanyCode	varchar(15)
--declare @BranchCOde		varchar(15)
--Declare @StartDate		varchar(20)
--Declare @EndDate		varchar(20)
--Declare @LastStartDate	varchar(20)
--Declare @LastEndDate	varchar(20)
--Declare @Area			varchar(100)
--declare @GroupModel		varchar(100)
--declare @TipeKendaraan	varchar(100)
--declare @Variant		varchar(100)
--declare @Summary		bit

--set @CompanyCode = ''
--set @BranchCode = ''
--set @StartDate = '20140401'
--set @EndDate = '20140415'
--set @LastStartDate = '20140301'
--set @LastEndDate = '20140328'
--set @Area = ''
--set @GroupModel = ''
--set @TipeKendaraan = ''
--set @Variant = ''
--set @Summary = 1

select * into #tThis from(
select c.CompanyCode, c.BranchCode 
	--, c.SequenceNo
	, c.LastProgress	
	, c.InquiryNumber
	, convert(varchar,c.UpdateDate,112)	UpdateDate
	, CASE 
		 WHEN day(c.UpdateDate) >= 1 and day(c.UpdateDate) <= 7  THEN 1
		 WHEN day(c.UpdateDate) >= 8 and day(c.UpdateDate) <= 14  THEN 2
		 WHEN day(c.UpdateDate) >= 15 and day(c.UpdateDate) <= 21  THEN 3
		 WHEN day(c.UpdateDate) >= 22 and day(c.UpdateDate) <= 28  THEN 4
		 WHEN day(c.UpdateDate) >= 29 and day(c.UpdateDate) <= 31  THEN 5
	   END WeekInt	
from SuzukiR4..pmStatusHistory c with (nolock, nowait) 
INNER JOIN SuzukiR4..pmHstITS a WITH (NOLOCK, NOWAIT) ON -- penambahan 1 Apr 14
				a.CompanyCode = c.CompanyCode AND
				a.BranchCode = c.BranchCode AND
				a.InquiryNumber = c.InquiryNumber --AND
				--a.LastProgress = c.LastProgress
where
	(case when c.CompanyCode='6015402' then '6015401' when c.CompanyCode='6051402' then '6051401' else c.CompanyCode end) like 
				case when @CompanyCode = ''       then '%%'  when @CompanyCode ='6015402' then '6015401' when @CompanyCode ='6051402' then '6051401' else @CompanyCode end		   
	and c.BranchCode  like case when @BranchCode='' then '%%' else @BranchCode end
	and c.LastProgress in (select LookUpValue from SuzukiR4..gnMstLookupDtl where CodeID = 'PSTS')
	and (convert(varchar,c.UpdateDate,112) between @StartDate and @EndDate
	or (convert(varchar,c.UpdateDate,112) < @StartDate 		
	and (c.LastProgress='P'   and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
											  where h.CompanyCode=c.CompanyCode
												and h.BranchCode=c.BranchCode
												and h.InquiryNumber=c.InquiryNumber
												and h.LastProgress<>'P'
												and convert(varchar,h.UpdateDate,112)<@StartDate)
	or  c.LastProgress='HP'  and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
											  where h.CompanyCode=c.CompanyCode
												and h.BranchCode=c.BranchCode
												and h.InquiryNumber=c.InquiryNumber
												and h.LastProgress not in ('P','HP')
												and convert(varchar,h.UpdateDate,112)<@StartDate)
	or  c.LastProgress='SPK' and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
											  where h.CompanyCode=c.CompanyCode
												and h.BranchCode=c.BranchCode
												and h.InquiryNumber=c.InquiryNumber
												and h.LastProgress not in ('P','HP','SPK')
												and convert(varchar,h.UpdateDate,112)<@StartDate))))	
union all
select   
h.CompanyCode
, h.BranchCode 
, 'SPK' LastProgress
, h.InquiryNumber
, convert(varchar,h.SPKDate,112)	SPKDate
, CASE 
	 WHEN day(h.SPKDate) >= 1 and day(h.SPKDate) <= 7  THEN 1
	 WHEN day(h.SPKDate) >= 8 and day(h.SPKDate) <= 14  THEN 2
	 WHEN day(h.SPKDate) >= 15 and day(h.SPKDate) <= 21  THEN 3
	 WHEN day(h.SPKDate) >= 22 and day(h.SPKDate) <= 28  THEN 4
	 WHEN day(h.SPKDate) >= 29 and day(h.SPKDate) <= 31  THEN 5
   END WeekInt	
from SuzukiR4..pmHstITS h
where 
(case when h.CompanyCode='6015402' then '6015401' when h.CompanyCode='6051402' then '6051401' else h.CompanyCode end) like 
				case when @CompanyCode = ''       then '%%'  when @CompanyCode ='6015402' then '6015401' when @CompanyCode ='6051402' then '6051401' else @CompanyCode end		   
	and h.BranchCode  like case when @BranchCode='' then '%%' else @BranchCode end
	and h.LastProgress in (select LookUpValue from SuzukiR4..gnMstLookupDtl where CodeID = 'PSTS')
 and convert(varchar,h.SPKDate,112) between @StartDate and @EndDate 
 and not exists (select top 1 1 from SuzukiR4..pmStatusHistory
                  where CompanyCode=h.CompanyCode
                    and BranchCode=h.BranchCode
                    and InquiryNumber=h.InquiryNumber)    							
) #tThis order by CompanyCode, BranchCode

select * into #tLast from(
select c.CompanyCode, c.BranchCode 
	--, c.SequenceNo
	, c.LastProgress
	, c.InquiryNumber		 
	, convert(varchar,c.UpdateDate,112)	UpdateDate
	, CASE 
		 WHEN day(c.UpdateDate) >= 1 and day(c.UpdateDate) <= 7  THEN 1
		 WHEN day(c.UpdateDate) >= 8 and day(c.UpdateDate) <= 14  THEN 2
		 WHEN day(c.UpdateDate) >= 15 and day(c.UpdateDate) <= 21  THEN 3
		 WHEN day(c.UpdateDate) >= 22 and day(c.UpdateDate) <= 28  THEN 4
		 WHEN day(c.UpdateDate) >= 29 and day(c.UpdateDate) <= 31  THEN 5
	   END WeekInt		   
from SuzukiR4..pmStatusHistory c with (nolock, nowait) 
INNER JOIN SuzukiR4..pmHstITS a WITH (NOLOCK, NOWAIT) ON -- penambahan 1 Apr 14
				a.CompanyCode = c.CompanyCode AND
				a.BranchCode = c.BranchCode AND
				a.InquiryNumber = c.InquiryNumber --AND
				--a.LastProgress = c.LastProgress
where
	(case when c.CompanyCode='6015402' then '6015401' when c.CompanyCode='6051402' then '6051401' else c.CompanyCode end) like 
				case when @CompanyCode = ''       then '%%'  when @CompanyCode ='6015402' then '6015401' when @CompanyCode ='6051402' then '6051401' else @CompanyCode end		   
	and c.BranchCode  like case when @BranchCode='' then '%%' else @BranchCode end
	and c.LastProgress in (select LookUpValue from SuzukiR4..gnMstLookupDtl where CodeID = 'PSTS')
	and (convert(varchar,c.UpdateDate,112) between @LastStartDate and @LastEndDate
	or (convert(varchar,c.UpdateDate,112) < @LastStartDate 
	and (c.LastProgress='P'   and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
											  where h.CompanyCode=c.CompanyCode
												and h.BranchCode=c.BranchCode
												and h.InquiryNumber=c.InquiryNumber
												and h.LastProgress<>'P'
												and convert(varchar,h.UpdateDate,112)<@LastStartDate)
	or  c.LastProgress='HP'  and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
											  where h.CompanyCode=c.CompanyCode
												and h.BranchCode=c.BranchCode
												and h.InquiryNumber=c.InquiryNumber
												and h.LastProgress not in ('P','HP')
												and convert(varchar,h.UpdateDate,112)<@LastStartDate)
	or  c.LastProgress='SPK' and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
											  where h.CompanyCode=c.CompanyCode
												and h.BranchCode=c.BranchCode
												and h.InquiryNumber=c.InquiryNumber
												and h.LastProgress not in ('P','HP','SPK')
												and convert(varchar,h.UpdateDate,112)<@LastStartDate))))			
union all
select
h.CompanyCode
, h.BranchCode 
, 'SPK' LastProgress
, h.InquiryNumber
, convert(varchar,h.SPKDate,112) SPKDate
, CASE 
	 WHEN day(h.SPKDate) >= 1 and day(h.SPKDate) <= 7  THEN 1
	 WHEN day(h.SPKDate) >= 8 and day(h.SPKDate) <= 14  THEN 2
	 WHEN day(h.SPKDate) >= 15 and day(h.SPKDate) <= 21  THEN 3
	 WHEN day(h.SPKDate) >= 22 and day(h.SPKDate) <= 28  THEN 4
	 WHEN day(h.SPKDate) >= 29 and day(h.SPKDate) <= 31  THEN 5
   END WeekInt	
from SuzukiR4..pmHstITS h
where 
(case when h.CompanyCode='6015402' then '6015401' when h.CompanyCode='6051402' then '6051401' else h.CompanyCode end) like 
				case when @CompanyCode = ''       then '%%'  when @CompanyCode ='6015402' then '6015401' when @CompanyCode ='6051402' then '6051401' else @CompanyCode end		   
	and h.BranchCode  like case when @BranchCode='' then '%%' else @BranchCode end
	and h.LastProgress in (select LookUpValue from SuzukiR4..gnMstLookupDtl where CodeID = 'PSTS')
and convert(varchar,h.SPKDate,112) between @LastStartDate and @LastEndDate 
 and not exists (select top 1 1 from SuzukiR4..pmStatusHistory
                  where CompanyCode=h.CompanyCode
                    and BranchCode=h.BranchCode
                    and InquiryNumber=h.InquiryNumber)     																					
) #tLast order by CompanyCode, BranchCode

select * into #t1 from(
select 
	a.CompanyCode
	, a.BranchCode 
	, isnull((select TOP 1 GroupModel 
						 from SuzukiR4..msMstGroupModel with (nolock,nowait)
						where ModelType = b.TipeKendaraan),'OTHERS') GroupModel
	, isnull(b.TipeKendaraan,'') TipeKendaraan
	, isnull(b.Variant,'') Variant
	, convert(varchar, b.InquiryDate, 112) InquiryDate
	, a.UpdateDate
	, a.LastProgress	
	, a.WeekInt	 
from #tThis a with (nolock, nowait)												
inner join SuzukiR4..pmHstITS b with (nolock, nowait) on b.CompanyCode = a.CompanyCode and b.BranchCode = a.BranchCode and b.InquiryNumber = a.InquiryNumber  
where
	(case when @TipeKendaraan <> '' then b.TipeKendaraan else @TipeKendaraan end) = @TipeKendaraan 
	and (case when @Variant <> '' then b.Variant else @Variant end) = @Variant 
	--and convert(varchar, b.InquiryDate, 112) < @StartDate
	--or convert(varchar, b.InquiryDate, 112) between @StartDate and @EndDate
	) #t1
order by CompanyCode, BranchCode, TipeKendaraan, Variant

select * into #t4 from(
select 	a.CompanyCode
	, a.BranchCode 
	, isnull((select TOP 1 GroupModel 
						 from SuzukiR4..msMstGroupModel with (nolock,nowait)
						where ModelType = b.TipeKendaraan),'OTHERS') GroupModel
	, isnull(b.TipeKendaraan,'') TipeKendaraan
	, isnull(b.Variant,'') Variant
	, convert(varchar, b.InquiryDate, 112) InquiryDate
	, a.UpdateDate
	, a.LastProgress
	, a.WeekInt	 
from #tLast a with (nolock, nowait) 
inner join SuzukiR4..pmHstITS b with (nolock, nowait) on b.CompanyCode = a.CompanyCode and b.BranchCode = a.BranchCode and b.InquiryNumber = a.InquiryNumber  
where
	(case when @TipeKendaraan <> '' then b.TipeKendaraan else @TipeKendaraan end) = @TipeKendaraan 
	and (case when @Variant <> '' then b.Variant else @Variant end) = @Variant 
	--and convert(varchar, b.InquiryDate, 112) < @LastStartDate
	--or convert(varchar, b.InquiryDate, 112) between @LastStartDate and @LastEndDate
	) #t4 order by CompanyCode, BranchCode, TipeKendaraan, Variant

select * into #tVehicle from(
select distinct CompanyCode, BranchCode, TipeKendaraan, Variant
from #t1 
where (case when @GroupModel <> '' then GroupModel else @GroupModel end) = @GroupModel
	and (case when @TipeKendaraan <> '' then TipeKendaraan else @TipeKendaraan end) = @TipeKendaraan 
	and (case when @Variant <> '' then Variant else @Variant end) = @Variant 
group by CompanyCode, BranchCode, TipeKendaraan, Variant
union 
select distinct CompanyCode, BranchCode, TipeKendaraan, Variant
from #t4 
where (case when @GroupModel <> '' then GroupModel else @GroupModel end) = @GroupModel
	and (case when @TipeKendaraan <> '' then TipeKendaraan else @TipeKendaraan end) = @TipeKendaraan 
	and (case when @Variant <> '' then Variant else @Variant end) = @Variant 
group by CompanyCode, BranchCode, TipeKendaraan, Variant
) #tVehicle

select * into #t2 from(
select SeqNo, LookupValue LastProgress
from SuzukiR4..gnMstLookupDtl
where CodeID = 'PSTS'
) #t2 order by SeqNo

select * into #t3 from(
select 1 WeekInt union select 2 WeekInt union select 3 WeekInt union select 4 WeekInt union select 5 WeekInt
) #t3
	
select * into #tUnion1 from(
select CompanyCode, BranchCode, TipeKendaraan, Variant
, LastProgress 

-- Outs
, (select count(*) from #t1
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan and Variant = a.Variant 
	and LastProgress = a.LastProgress and InquiryDate < @StartDate and UpdateDate < @StartDate ) SaldoAwal
, (select count(*) from #t1 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan and Variant = a.Variant 
	and LastProgress = a.LastProgress and WeekInt = 1 and InquiryDate < @StartDate and UpdateDate between @StartDate and @EndDate) WeekOuts1	
, (select count(*) from #t1 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan and Variant = a.Variant 
	and LastProgress = a.LastProgress and WeekInt = 2 and InquiryDate < @StartDate and UpdateDate between @StartDate and @EndDate) WeekOuts2		
, (select count(*) from #t1 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan and Variant = a.Variant 
	and LastProgress = a.LastProgress and WeekInt = 3 and InquiryDate < @StartDate and UpdateDate between @StartDate and @EndDate) WeekOuts3		
, (select count(*) from #t1 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan and Variant = a.Variant 
	and LastProgress = a.LastProgress and WeekInt = 4 and InquiryDate < @StartDate and UpdateDate between @StartDate and @EndDate) WeekOuts4
, (select count(*) from #t1 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan and Variant = a.Variant 
	and LastProgress = a.LastProgress and WeekInt = 5 and InquiryDate < @StartDate and UpdateDate between @StartDate and @EndDate) WeekOuts5
, (select count(*) from #t1 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan and Variant = a.Variant 
	and LastProgress = a.LastProgress and InquiryDate < @StartDate and UpdateDate between @StartDate and @EndDate) TotalWeekOuts

-- New
, (select count(*) from #t1 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan and Variant = a.Variant 
	and LastProgress = a.LastProgress and WeekInt = 1 and UpdateDate between @StartDate and @EndDate 
	and (InquiryDate between @StartDate and @EndDate or InquiryDate > @EndDate)) Week1	
,  (select count(*) from #t1 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan and Variant = a.Variant 
	and LastProgress = a.LastProgress and WeekInt = 2 and UpdateDate between @StartDate and @EndDate 
	and (InquiryDate between @StartDate and @EndDate or InquiryDate > @EndDate)) Week2		
, (select count(*) from #t1 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan and Variant = a.Variant 
	and LastProgress = a.LastProgress and WeekInt = 3 and UpdateDate between @StartDate and @EndDate 
	and (InquiryDate between @StartDate and @EndDate or InquiryDate > @EndDate)) Week3		
, (select count(*) from #t1 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan and Variant = a.Variant 
	and LastProgress = a.LastProgress and WeekInt = 4 and UpdateDate between @StartDate and @EndDate 
	and (InquiryDate between @StartDate and @EndDate or InquiryDate > @EndDate)) Week4
, (select count(*) from #t1 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan and Variant = a.Variant 
	and LastProgress = a.LastProgress and WeekInt = 5 and UpdateDate between @StartDate and @EndDate 
	and (InquiryDate between @StartDate and @EndDate or InquiryDate > @EndDate)) Week5
, (select count(*) from #t1 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan and Variant = a.Variant 
	and LastProgress = a.LastProgress and UpdateDate between @StartDate and @EndDate 
	and (InquiryDate between @StartDate and @EndDate or InquiryDate > @EndDate)) TotalWeek
		
from #t1 a

group by CompanyCode, BranchCode, TipeKendaraan, Variant
, LastProgress 	
) #tUnion1

select * into #tUnion2 from(
select CompanyCode, BranchCode, TipeKendaraan, Variant
, LastProgress 

-- Outs
, (select count(*) from #t4
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan and Variant = a.Variant 
	and LastProgress = a.LastProgress and InquiryDate < @LastStartDate and UpdateDate < @LastStartDate) SaldoAwalLast
, (select count(*) from #t4 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan and Variant = a.Variant 
	and LastProgress = a.LastProgress and WeekInt = 1 and InquiryDate < @LastStartDate and UpdateDate between @LastStartDate and @LastEndDate) WeekOuts1Last	
, (select count(*) from #t4 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan and Variant = a.Variant 
	and LastProgress = a.LastProgress and WeekInt = 2 and InquiryDate < @LastStartDate and UpdateDate between @LastStartDate and @LastEndDate) WeekOuts2Last		
, (select count(*) from #t4 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan and Variant = a.Variant 
	and LastProgress = a.LastProgress and WeekInt = 3 and InquiryDate < @LastStartDate and UpdateDate between @LastStartDate and @LastEndDate) WeekOuts3Last		
, (select count(*) from #t4 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan and Variant = a.Variant 
	and LastProgress = a.LastProgress and WeekInt = 4 and InquiryDate < @LastStartDate and UpdateDate between @LastStartDate and @LastEndDate) WeekOuts4Last
, (select count(*) from #t4 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan and Variant = a.Variant 
	and LastProgress = a.LastProgress and WeekInt = 5 and InquiryDate < @LastStartDate and UpdateDate between @LastStartDate and @LastEndDate) WeekOuts5Last
, (select count(*) from #t4 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan and Variant = a.Variant 
	and LastProgress = a.LastProgress and InquiryDate < @LastStartDate and UpdateDate between @LastStartDate and @LastEndDate) TotalWeekOutsLast

-- New
, (select count(*) from #t4 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan and Variant = a.Variant 
	and LastProgress = a.LastProgress and WeekInt = 1 and UpdateDate between @LastStartDate and @LastEndDate
	and (InquiryDate between @LastStartDate and @LastEndDate or InquiryDate > @LastEndDate)) Week1Last	
, (select count(*) from #t4 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan and Variant = a.Variant 
	and LastProgress = a.LastProgress and WeekInt = 2 and UpdateDate between @LastStartDate and @LastEndDate
	and (InquiryDate between @LastStartDate and @LastEndDate or InquiryDate > @LastEndDate))  Week2Last		
,  (select count(*) from #t4 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan and Variant = a.Variant 
	and LastProgress = a.LastProgress and WeekInt = 3 and UpdateDate between @LastStartDate and @LastEndDate
	and (InquiryDate between @LastStartDate and @LastEndDate or InquiryDate > @LastEndDate)) Week3Last		
,  (select count(*) from #t4
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan and Variant = a.Variant 
	and LastProgress = a.LastProgress and WeekInt = 4 and UpdateDate between @LastStartDate and @LastEndDate
	and (InquiryDate between @LastStartDate and @LastEndDate or InquiryDate > @LastEndDate)) Week4Last
,  (select count(*) from #t4
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan and Variant = a.Variant 
	and LastProgress = a.LastProgress and WeekInt = 5 and UpdateDate between @LastStartDate and @LastEndDate
	and (InquiryDate between @LastStartDate and @LastEndDate or InquiryDate > @LastEndDate)) Week5Last
, (select count(*) from #t4
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan and Variant = a.Variant 
	and LastProgress = a.LastProgress and UpdateDate between @LastStartDate and @LastEndDate
	and (InquiryDate between @LastStartDate and @LastEndDate or InquiryDate > @LastEndDate)) TotalWeekLast
		
from #t4 a

group by CompanyCode, BranchCode, TipeKendaraan, Variant
, LastProgress 	
) #tUnion2
			
-- This Month
select * into #tGabung from(	
select distinct
	a.GroupNo
	, a.Area
	, a.DealerCode CompanyCode
	, a.DealerAbbreviation CompanyName
	, b.OutletCode BranchCode 
	, b.OutletAbbreviation BranchName
	, d.TipeKendaraan
	, d.Variant
	, e.SeqNo
	, e.LastProgress		
	
	, isnull(g.SaldoAwal, 0) SaldoAwal
	, isnull(g.WeekOuts1, 0) WeekOuts1
	, isnull(g.WeekOuts2, 0) WeekOuts2
	, isnull(g.WeekOuts3, 0) WeekOuts3
	, isnull(g.WeekOuts4, 0) WeekOuts4
	, isnull(g.WeekOuts5, 0) WeekOuts5
	, isnull(g.TotalWeekOuts, 0) TotalWeekOuts
	
	, isnull(g.Week1, 0) Week1
	, isnull(g.Week2, 0) Week2
	, isnull(g.Week3, 0) Week3
	, isnull(g.Week4, 0) Week4
	, isnull(g.Week5, 0) Week5
	, isnull(g.TotalWeek, 0) TotalWeek
	
	, isnull(g.TotalWeekOuts, 0) + isnull(g.TotalWeek, 0) Total
	
	, isnull(h.SaldoAwalLast, 0) SaldoAwalLast
	, isnull(h.WeekOuts1Last, 0) WeekOuts1Last
	, isnull(h.WeekOuts2Last, 0) WeekOuts2Last
	, isnull(h.WeekOuts3Last, 0) WeekOuts3Last
	, isnull(h.WeekOuts4Last, 0) WeekOuts4Last
	, isnull(h.WeekOuts5Last, 0) WeekOuts5Last
	, isnull(h.TotalWeekOutsLast, 0) TotalWeekOutsLast
	
	, isnull(h.Week1Last, 0) Week1Last
	, isnull(h.Week2Last, 0) Week2Last
	, isnull(h.Week3Last, 0) Week3Last
	, isnull(h.Week4Last, 0) Week4Last
	, isnull(h.Week5Last, 0) Week5Last
	, isnull(h.TotalWeekLast, 0) TotalWeekLast
	
	, isnull(h.TotalWeekOutsLast, 0) + isnull(h.TotalWeekLast, 0) TotalLast
	
	,cast((case when isnull(h.TotalWeekOutsLast, 0) + isnull(h.TotalWeekLast, 0) = 0 then 0 else (isnull(g.TotalWeekOuts, 0) + isnull(g.TotalWeek, 0))/ cast((isnull(h.TotalWeekOutsLast, 0) + isnull(h.TotalWeekLast, 0)) as decimal(38,2)) end) * 100 as decimal(8,1)) TotPercent

from SuzukiR4..gnMstDealerMapping a with (nolock, nowait)
left join SuzukiR4..gnMstDealerOutletMapping b with (nolock, nowait) on a.DealerCode = b.DealerCode
inner join #tVehicle d with (nolock, nowait) on d.CompanyCode = a.DealerCode and d.BranchCode = b.OutletCode
left join #t2 e with (nolock, nowait) on e.SeqNo > 0 
left join #t3 f with (nolock, nowait) on f.WeekInt > 0
left join #tUnion1 g with (nolock, nowait) on g.CompanyCode = a.DealerCode and g.BranchCode = b.OutletCode and g.TipeKendaraan =  d.TipeKendaraan
	and g.Variant = d.Variant 
	and g.LastProgress = e.LastProgress
left join #tUnion2 h with (nolock, nowait) on h.CompanyCode = a.DealerCode and h.BranchCode = b.OutletCode and h.TipeKendaraan =  d.TipeKendaraan
	and h.Variant = d.Variant 
	and h.LastProgress = e.LastProgress
where 
	a.IsActive = 1
	and (a.Area like Case when @Area <> '' then case when (@Area = 'JABODETABEK' or @Area = 'CABANG') then 'JABODETABEK' else @Area end else '%%' end
			or  a.Area like Case when @Area <> '' then case when (@Area='JABODETABEK' or @Area='CABANG') then 'CABANG' else @Area end else '%%' end)		  
	and (case when a.DealerCode='6015402' then '6015401' when a.DealerCode='6051402' then '6051401' else a.DealerCode end) like 
				case when @CompanyCode = ''       then '%%'  when @CompanyCode ='6015402' then '6015401' when @CompanyCode ='6051402' then '6051401' else @CompanyCode end		   
	and b.OutletCode  like case when @BranchCode='' then '%%' else @BranchCode end
	) #tGabung
order by GroupNo, CompanyCode, BranchCode, TipeKendaraan, Variant, SeqNo

-- Level 0
select * into #tFinal from(
select 
	1 OrderNo3
	, 0 OrderNo
	, 0 OrderNo1
	, 0 OrderNo2
	
	,  GroupNo
	,  Area
	,  CompanyCode
	,  CompanyName
	,  BranchCode 
	,  BranchName
	,  TipeKendaraan
	,  Variant
	,  SeqNo
	,  LastProgress		
	
	, SaldoAwalLast	
	, WeekOuts1Last	
	, WeekOuts2Last		
	, WeekOuts3Last		
	, WeekOuts4Last
	, WeekOuts5Last
	, TotalWeekOutsLast
	, Week1Last
	, Week2Last
	, Week3Last
	, Week4Last
	, Week5Last
	, TotalWeekLast
	, TotalLast
	
	,  SaldoAwal	
	,  WeekOuts1	
	,  WeekOuts2		
	,  WeekOuts3		
	,  WeekOuts4
	,  WeekOuts5
	,  TotalWeekOuts
	,  Week1
	,  Week2
	,  Week3
	,  Week4
	,  Week5
	,  TotalWeek	
	,  Total
	
	, TotPercent

	from #tGabung
union
---- Group per Tipe Kendaraan
select 
	1 OrderNo3
	, 1 OrderNo
	, 0 OrderNo1
	, 0 OrderNo2
	
	,  GroupNo
	,  Area
	,  CompanyCode
	,  CompanyName
	,  BranchCode 
	,  BranchName
	,  TipeKendaraan
	,  '' Variant
	,  SeqNo
	,  LastProgress		
	
	, sum(SaldoAwalLast) SaldoAwalLast	
	, sum(WeekOuts1Last) WeekOuts1Last	
	, sum(WeekOuts2Last) WeekOuts2Last	
	, sum(WeekOuts3Last) WeekOuts3Last		
	, sum(WeekOuts4Last) WeekOuts4Last
	, sum(WeekOuts5Last) WeekOuts5Last
	, sum(TotalWeekOutsLast) TotalWeekOutsLast
	, sum(Week1Last) Week1Last
	, sum(Week2Last) Week2Last
	, sum(Week3Last) Week3Last
	, sum(Week4Last) Week4Last
	, sum(Week5Last) Week5Last
	, sum(TotalWeekLast) TotalWeekLast
	, sum(TotalLast) TotalLast
	
	,  sum(SaldoAwal) SaldoAwal
	,  sum(WeekOuts1) WeekOuts1
	,  sum(WeekOuts2) WeekOuts2
	,  sum(WeekOuts3) WeekOuts3
	,  sum(WeekOuts4) WeekOuts4
	,  sum(WeekOuts5) WeekOuts5
	,  sum(TotalWeekOuts) TotalWeekOuts
	,  sum(Week1) Week1
	,  sum(Week2) Week2
	,  sum(Week3) Week3
	,  sum(Week4) Week4
	,  sum(Week5) Week5
	,  sum(TotalWeek) TotalWeek
	,  sum(Total) Total
	
	,  cast((case when sum(TotalLast) = 0 then 0 else sum(Total)/ cast(sum(TotalLast) as decimal(38,2)) end) * 100 as decimal(8,1)) TotPercent 

	from #tGabung
group by 
	 GroupNo
	,   Area
	,   CompanyCode
	,   CompanyName
	,   BranchCode 
	,   BranchName
	,   TipeKendaraan		
	,   SeqNo
	,   LastProgress	
union
-- Group per Company
select 
	1 OrderNo3
	, 2 OrderNo
	, 1 OrderNo1
	, 0 OrderNo2

	,  GroupNo
	,  Area
	,  CompanyCode
	,  CompanyName
	,  BranchCode 
	,  BranchName
	,  '' TipeKendaraan
	,  '' Variant
	,  SeqNo
	,  LastProgress		
	
	, sum(SaldoAwalLast) SaldoAwalLast	
	, sum(WeekOuts1Last) WeekOuts1Last	
	, sum(WeekOuts2Last) WeekOuts2Last	
	, sum(WeekOuts3Last) WeekOuts3Last		
	, sum(WeekOuts4Last) WeekOuts4Last
	, sum(WeekOuts5Last) WeekOuts5Last
	, sum(TotalWeekOutsLast) TotalWeekOutsLast
	, sum(Week1Last) Week1Last
	, sum(Week2Last) Week2Last
	, sum(Week3Last) Week3Last
	, sum(Week4Last) Week4Last
	, sum(Week5Last) Week5Last
	, sum(TotalWeekLast) TotalWeekLast
	, sum(TotalLast) TotalLast
	
	,  sum(SaldoAwal) SaldoAwal
	,  sum(WeekOuts1) WeekOuts1
	,  sum(WeekOuts2) WeekOuts2
	,  sum(WeekOuts3) WeekOuts3
	,  sum(WeekOuts4) WeekOuts4
	,  sum(WeekOuts5) WeekOuts5
	,  sum(TotalWeekOuts) TotalWeekOuts
	,  sum(Week1) Week1
	,  sum(Week2) Week2
	,  sum(Week3) Week3
	,  sum(Week4) Week4
	,  sum(Week5) Week5
	,  sum(TotalWeek) TotalWeek
	,  sum(Total) Total
	
	,  cast((case when sum(TotalLast) = 0 then 0 else sum(Total)/ cast(sum(TotalLast) as decimal(38,2)) end) * 100 as decimal(8,1)) TotPercent 

	from #tGabung
group by  GroupNo
	,  Area
	,  CompanyCode
	,  CompanyName
	,  BranchCode 
	,  BranchName	
	,  SeqNo
	,  LastProgress		
union
select 
	1 OrderNo3
	, 2 OrderNo
	, 2 OrderNo1
	, 0 OrderNo2

	,  GroupNo
	,  Area
	,  '' CompanyCode
	,  'TOTAL' CompanyName
	,  '' BranchCode 
	,  '' BranchName
	,  '' TipeKendaraan
	,  '' Variant
	,  SeqNo
	,  LastProgress		
	
	, sum(SaldoAwalLast) SaldoAwalLast	
	, sum(WeekOuts1Last) WeekOuts1Last	
	, sum(WeekOuts2Last) WeekOuts2Last	
	, sum(WeekOuts3Last) WeekOuts3Last		
	, sum(WeekOuts4Last) WeekOuts4Last
	, sum(WeekOuts5Last) WeekOuts5Last
	, sum(TotalWeekOutsLast) TotalWeekOutsLast
	, sum(Week1Last) Week1Last
	, sum(Week2Last) Week2Last
	, sum(Week3Last) Week3Last
	, sum(Week4Last) Week4Last
	, sum(Week5Last) Week5Last
	, sum(TotalWeekLast) TotalWeekLast
	, sum(TotalLast) TotalLast
	
	,  sum(SaldoAwal) SaldoAwal
	,  sum(WeekOuts1) WeekOuts1
	,  sum(WeekOuts2) WeekOuts2
	,  sum(WeekOuts3) WeekOuts3
	,  sum(WeekOuts4) WeekOuts4
	,  sum(WeekOuts5) WeekOuts5
	,  sum(TotalWeekOuts) TotalWeekOuts
	,  sum(Week1) Week1
	,  sum(Week2) Week2
	,  sum(Week3) Week3
	,  sum(Week4) Week4
	,  sum(Week5) Week5
	,  sum(TotalWeek) TotalWeek
	,  sum(Total) Total
	
	,  cast((case when sum(TotalLast) = 0 then 0 else sum(Total)/ cast(sum(TotalLast) as decimal(38,2)) end) * 100 as decimal(8,1)) TotPercent 

	from #tGabung
group by  GroupNo
	,  Area	
	,  SeqNo
	,  LastProgress			
union
-- Group per Area
select 
	1 OrderNo3
	, 2 OrderNo
	, 1 OrderNo1
	, 1 OrderNo2
	
	,  GroupNo	
	,  Area	
	,  CompanyCode
	,  CompanyName	
	,  'TOTAL' BranchCode 
	,  '' BranchName		
	,  '' TipeKendaraan
	,  '' Variant
	,  SeqNo
	,  LastProgress		
	
	, sum(SaldoAwalLast) SaldoAwalLast	
	, sum(WeekOuts1Last) WeekOuts1Last	
	, sum(WeekOuts2Last) WeekOuts2Last	
	, sum(WeekOuts3Last) WeekOuts3Last		
	, sum(WeekOuts4Last) WeekOuts4Last
	, sum(WeekOuts5Last) WeekOuts5Last
	, sum(TotalWeekOutsLast) TotalWeekOutsLast
	, sum(Week1Last) Week1Last
	, sum(Week2Last) Week2Last
	, sum(Week3Last) Week3Last
	, sum(Week4Last) Week4Last
	, sum(Week5Last) Week5Last
	, sum(TotalWeekLast) TotalWeekLast
	, sum(TotalLast) TotalLast
	
	,  sum(SaldoAwal) SaldoAwal
	,  sum(WeekOuts1) WeekOuts1
	,  sum(WeekOuts2) WeekOuts2
	,  sum(WeekOuts3) WeekOuts3
	,  sum(WeekOuts4) WeekOuts4
	,  sum(WeekOuts5) WeekOuts5
	,  sum(TotalWeekOuts) TotalWeekOuts
	,  sum(Week1) Week1
	,  sum(Week2) Week2
	,  sum(Week3) Week3
	,  sum(Week4) Week4
	,  sum(Week5) Week5
	,  sum(TotalWeek) TotalWeek
	,  sum(Total) Total
	
	,  cast((case when sum(TotalLast) = 0 then 0 else sum(Total)/ cast(sum(TotalLast) as decimal(38,2)) end) * 100 as decimal(8,1)) TotPercent 

	from #tGabung
group by GroupNo	
	,  Area	
	,  CompanyCode
	,  CompanyName	
	, BranchName
	,  SeqNo
	,  LastProgress		
union
-- TOTAL
select 
	2 OrderNo3
	, 2 OrderNo
	, 1 OrderNo1
	, 1 OrderNo2
	
	,  999999 GroupNo	
	,  'TOTAL' Area	
	,  '' CompanyCode
	,  'TOTAL' CompanyName	
	,  '' BranchCode 
	,  '' BranchName		
	,  '' TipeKendaraan
	,  '' Variant
	,  SeqNo
	,  LastProgress		
	
	, sum(SaldoAwalLast) SaldoAwalLast	
	, sum(WeekOuts1Last) WeekOuts1Last	
	, sum(WeekOuts2Last) WeekOuts2Last	
	, sum(WeekOuts3Last) WeekOuts3Last		
	, sum(WeekOuts4Last) WeekOuts4Last
	, sum(WeekOuts5Last) WeekOuts5Last
	, sum(TotalWeekOutsLast) TotalWeekOutsLast
	, sum(Week1Last) Week1Last
	, sum(Week2Last) Week2Last
	, sum(Week3Last) Week3Last
	, sum(Week4Last) Week4Last
	, sum(Week5Last) Week5Last
	, sum(TotalWeekLast) TotalWeekLast
	, sum(TotalLast) TotalLast
	
	,  sum(SaldoAwal) SaldoAwal
	,  sum(WeekOuts1) WeekOuts1
	,  sum(WeekOuts2) WeekOuts2
	,  sum(WeekOuts3) WeekOuts3
	,  sum(WeekOuts4) WeekOuts4
	,  sum(WeekOuts5) WeekOuts5
	,  sum(TotalWeekOuts) TotalWeekOuts
	,  sum(Week1) Week1
	,  sum(Week2) Week2
	,  sum(Week3) Week3
	,  sum(Week4) Week4
	,  sum(Week5) Week5
	,  sum(TotalWeek) TotalWeek
	,  sum(Total) Total
	
	,  cast((case when sum(TotalLast) = 0 then 0 else sum(Total)/ cast(sum(TotalLast) as decimal(38,2)) end) * 100 as decimal(8,1)) TotPercent 

	from #tGabung
group by SeqNo
	,  LastProgress) #tFinal

if(@Summary = 0)
begin	
select 
		  OrderNo3
		 ,  OrderNo
		 ,  OrderNo1
		 ,  OrderNo2
		 ,  GroupNo
		 ,  Area
		 ,  CompanyCode
		 ,  CompanyName
		 ,  BranchCode
		 ,  BranchName
		 ,  SeqNo
		 ,  LastProgress		 		
		 
		 ---- LAST MONTH	
		 , sum(SaldoAwalLast) SaldoAwalLast
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(WeekOuts1Last) END WeekOuts1Last
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(WeekOuts2Last) END WeekOuts2Last  
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(WeekOuts3Last) END WeekOuts3Last
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(WeekOuts4Last) END WeekOuts4Last
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(WeekOuts5Last) END WeekOuts5Last		 
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(TotalWeekOutsLast) END TotalWeekOutsLast
		 , sum(Week1Last) Week1Last
		 , sum(Week2Last) Week2Last
		 , sum(Week3Last) Week3Last
		 , sum(Week4Last) Week4Last
		 , sum(Week5Last) Week5Last
		 , sum(TotalWeekLast) TotalWeekLast
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(TotalWeekOutsLast) END + sum(TotalWeekLast) TotalLast	 
		 
		 ---- THIS MONTH	
		 , sum(SaldoAwal) SaldoAwal
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(WeekOuts1) END WeekOuts1
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(WeekOuts2) END WeekOuts2
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(WeekOuts3) END WeekOuts3
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(WeekOuts4) END WeekOuts4
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(WeekOuts5) END WeekOuts5		 
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(TotalWeekOuts) END TotalWeekOuts
		 , sum(Week1) Week1
		 , sum(Week2) Week2 
		 , sum(Week3) Week3
		 , sum(Week4) Week4
		 , sum(Week5) Week5
		 , sum(TotalWeek) TotalWeek
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(TotalWeekOuts) END + sum(TotalWeek) Total
		 
		 , cast((case when (CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(TotalWeekOutsLast) END + sum(TotalWeekLast)) = 0 then 0 else 
		   (CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(TotalWeekOuts) END + sum(TotalWeek))/ cast((CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(TotalWeekOutsLast) END + sum(TotalWeekLast)) as decimal(38,2)) end) * 100 as decimal(8,1)) TotPercent
 from #tFinal
 --where OrderNo2 <> 1
 group by  OrderNo3
		 ,  OrderNo
		 ,  OrderNo1
		 ,  OrderNo2
		 ,  GroupNo
		 ,  Area
		 ,  CompanyCode
		 ,  CompanyName
		 ,  BranchCode
		 ,  BranchName
		 ,  SeqNo
		 ,  LastProgress	
	--order by OrderNo3 Asc,GroupNo asc, CompanyName asc,Area asc,OrderNo1 asc,OrderNo asc,BranchCode asc,OrderNo2 Asc,SeqNo Asc
	order by  OrderNo3 Asc, GroupNo asc,  CompanyName asc, Area asc, OrderNo2 asc, OrderNo1 asc, BranchCode asc,  OrderNo Asc,  SeqNo Asc

select 
		  OrderNo3
		 ,  OrderNo
		 ,  OrderNo1
		 ,  OrderNo2
		 ,  GroupNo
		 ,  Area
		 ,  CompanyCode
		 ,  CompanyName
		 ,  BranchCode
		 ,  BranchName
		 ,  TipeKendaraan
		 ,  Variant
		 ,  SeqNo
		 ,  LastProgress		 		
		 
		 ---- LAST MONTH	
		 , sum(SaldoAwalLast) SaldoAwalLast
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(WeekOuts1Last) END WeekOuts1Last
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(WeekOuts2Last) END WeekOuts2Last  
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(WeekOuts3Last) END WeekOuts3Last
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(WeekOuts4Last) END WeekOuts4Last
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(WeekOuts5Last) END WeekOuts5Last		 
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(TotalWeekOutsLast) END TotalWeekOutsLast
		 , sum(Week1Last) Week1Last
		 , sum(Week2Last) Week2Last
		 , sum(Week3Last) Week3Last
		 , sum(Week4Last) Week4Last
		 , sum(Week5Last) Week5Last
		 , sum(TotalWeekLast) TotalWeekLast
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(TotalWeekOutsLast) END + sum(TotalWeekLast) TotalLast	
		 
		 ---- THIS MONTH	
		 , sum(SaldoAwal) SaldoAwal
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(WeekOuts1) END WeekOuts1
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(WeekOuts2) END WeekOuts2
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(WeekOuts3) END WeekOuts3
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(WeekOuts4) END WeekOuts4
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(WeekOuts5) END WeekOuts5		 
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(TotalWeekOuts) END TotalWeekOuts
		 , sum(Week1) Week1
		 , sum(Week2) Week2 
		 , sum(Week3) Week3
		 , sum(Week4) Week4
		 , sum(Week5) Week5
		 , sum(TotalWeek) TotalWeek
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(TotalWeekOuts) END + sum(TotalWeek) Total
		 
		 , cast((case when (CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(TotalWeekOutsLast) END + sum(TotalWeekLast)) = 0 then 0 else 
		   (CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(TotalWeekOuts) END + sum(TotalWeek))/ cast((CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(TotalWeekOutsLast) END + sum(TotalWeekLast)) as decimal(38,2)) end) * 100 as decimal(8,1)) TotPercent
 from #tFinal
 where OrderNo1 <> 2
 group by  OrderNo3
		 ,  OrderNo
		 ,  OrderNo1
		 ,  OrderNo2
		 ,  GroupNo
		 ,  Area
		 ,  CompanyCode
		 ,  CompanyName
		 ,  BranchCode
		 ,  BranchName
		 ,  TipeKendaraan
		 ,  Variant
		 ,  SeqNo
		 ,  LastProgress
	 order by  OrderNo3 Asc, GroupNo asc,  CompanyName asc, Area asc, OrderNo2 asc, BranchCode asc, OrderNo1 asc,  TipeKendaraan Asc,  OrderNo Asc,  Variant Asc,  SeqNo Asc
	 --order by  OrderNo3 Asc, GroupNo asc,  CompanyName asc, Area asc, OrderNo2 asc, BranchCode asc, OrderNo1 asc,  TipeKendaraan Asc,  OrderNo Asc,  Variant Asc,  SeqNo Asc
	 --order by  OrderNo3 Asc,  TipeKendaraan Asc, GroupNo asc,  CompanyName asc, OrderNo asc, OrderNo1 asc, BranchCode asc, OrderNo2 Asc, Area asc, SeqNo Asc 	 
end
else if (@Summary = 1)
begin
	select 
		  OrderNo3
		 ,  OrderNo
		 ,  OrderNo1
		 ,  OrderNo2
		 ,  GroupNo
		 ,  Area
		 ,  CompanyCode
		 ,  CompanyName
		 ,  BranchCode
		 ,  BranchName
		 ,  TipeKendaraan
		 ,  Variant
		 ,  SeqNo
		 ,  LastProgress		 		
		 
		 ---- LAST MONTH	
		 , sum(SaldoAwalLast) SaldoAwalLast
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(WeekOuts1Last) END WeekOuts1Last
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(WeekOuts2Last) END WeekOuts2Last  
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(WeekOuts3Last) END WeekOuts3Last
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(WeekOuts4Last) END WeekOuts4Last
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(WeekOuts5Last) END WeekOuts5Last		 
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(TotalWeekOutsLast) END TotalWeekOutsLast
		 , sum(Week1Last) Week1Last
		 , sum(Week2Last) Week2Last
		 , sum(Week3Last) Week3Last
		 , sum(Week4Last) Week4Last
		 , sum(Week5Last) Week5Last
		 , sum(TotalWeekLast) TotalWeekLast
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(TotalWeekOutsLast) END + sum(TotalWeekLast) TotalLast	
		 
		 ---- THIS MONTH	
		 , sum(SaldoAwal) SaldoAwal
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(WeekOuts1) END WeekOuts1
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(WeekOuts2) END WeekOuts2
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(WeekOuts3) END WeekOuts3
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(WeekOuts4) END WeekOuts4
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(WeekOuts5) END WeekOuts5		 
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(TotalWeekOuts) END TotalWeekOuts
		 , sum(Week1) Week1
		 , sum(Week2) Week2 
		 , sum(Week3) Week3
		 , sum(Week4) Week4
		 , sum(Week5) Week5
		 , sum(TotalWeek) TotalWeek
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(TotalWeekOuts) END + sum(TotalWeek) Total
		 
		 , cast((case when (CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(TotalWeekOutsLast) END + sum(TotalWeekLast)) = 0 then 0 else 
		   (CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(TotalWeekOuts) END + sum(TotalWeek))/ cast((CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(TotalWeekOutsLast) END + sum(TotalWeekLast)) as decimal(38,2)) end) * 100 as decimal(8,1)) TotPercent
 from #tFinal
 group by  OrderNo3
		 ,  OrderNo
		 ,  OrderNo1
		 ,  OrderNo2
		 ,  GroupNo
		 ,  Area
		 ,  CompanyCode
		 ,  CompanyName
		 ,  BranchCode
		 ,  BranchName
		 ,  TipeKendaraan
		 ,  Variant
		 ,  SeqNo
		 ,  LastProgress
	order by  OrderNo3 Asc, GroupNo asc,  CompanyName asc, Area asc, OrderNo2 asc, BranchCode asc, OrderNo1 asc,  TipeKendaraan Asc,  OrderNo Asc,  Variant Asc,  SeqNo Asc	 
	 --order by  OrderNo3 Asc,  TipeKendaraan Asc, GroupNo asc,  CompanyName asc, OrderNo asc, OrderNo1 asc, BranchCode asc, OrderNo2 Asc, Area asc, SeqNo Asc 	 
end

drop table #tThis, #t1, #t2, #t3, #t4, #tLast, #tUnion1, #tUnion2, #tVehicle, #tGabung, #tFinal

end
GO

if object_id('usp_itsinqlostcase') is not null
	drop procedure usp_itsinqlostcase
GO
create procedure usp_itsinqlostcase
	@CompanyCode varchar(20),
	@EmployeeID varchar(20),
	@DateFrom varchar(10),
	@DateTo varchar(10)
as

select a.InquiryNumber
     --, a.BranchCode
	 --, a.EmployeeID
	 --, a.OutletID
	 , a.NamaProspek
	 , a.InquiryDate
	 , a.TipeKendaraan
	 , a.Variant
	 , a.PerolehanData
	 , d.LookUpValueName as PerolehanDataDesc
	 , b.UpdateDate as LostDate
	 , a.LostCaseCategory as LostCaseCategoryCode
	 , c.LookUpValueName as LostCaseCategoryDesc
	 , a.LostCaseOtherReason
	 , a.LostCaseVoiceOfCustomer
	 , b.LastProgress
	 , b.UpdateDate
  from PmKDP a 
  left join PmStatusHistory b
    on b.CompanyCode = a.CompanyCode
   and b.BranchCode = a.BranchCode 
   and b.InquiryNumber = a.InquiryNumber
   and b.SequenceNo = (select top 1 SequenceNo from PmStatusHistory where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InquiryNumber = a.InquiryNumber order by SequenceNo desc)
  left join GnMstLookUpDtl c
    on c.CompanyCode = a.CompanyCode
   and c.CodeID = 'PLCC'
   and c.LookUpValue = a.LostCaseCategory
  left join GnMstLookUpDtl d
    on d.CompanyCode = a.CompanyCode
   and d.CodeID = 'PSRC'
   and d.LookUpValue = a.PerolehanData
 where a.CompanyCode = @CompanyCode
   and a.EmployeeID = @EmployeeID
   and a.LastProgress = 'LOST'
   and convert(varchar, b.UpdateDate, 112) between @DateFrom and @DateTo
 order by a.InquiryNumber

--select * from PmStatusHistory where InquiryNumber = '387904'

go

if object_id('usp_itsinqlostbytype') is not null
	drop procedure usp_itsinqlostbytype
GO
create procedure usp_itsinqlostbytype
	@CompanyCode varchar(20),
	@EmployeeID varchar(20),
	@DateFrom varchar(10),
	@DateTo varchar(10)
as

select a.TipeKendaraan as name
	 , count(a.TipeKendaraan) as value
  from PmKDP a 
  left join PmStatusHistory b
    on b.CompanyCode = a.CompanyCode
   and b.BranchCode = a.BranchCode 
   and b.InquiryNumber = a.InquiryNumber
   and b.SequenceNo = (select top 1 SequenceNo from PmStatusHistory where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InquiryNumber = a.InquiryNumber order by SequenceNo desc)
 where a.CompanyCode = @CompanyCode
   and a.EmployeeID = @EmployeeID
   and a.LastProgress = 'LOST'
   and convert(varchar, b.UpdateDate, 112) between @DateFrom and @DateTo
 group by a.TipeKendaraan

--select * from PmStatusHistory where InquiryNumber = '387904'

go

if object_id('uspfn_pmSelectOrganizationTree') is not null
	drop procedure uspfn_pmSelectOrganizationTree
GO
--created by BENEDICT 11/Mar/2015

CREATE PROCEDURE uspfn_pmSelectOrganizationTree
@CompanyCode varchar(15),
@BranchCode varchar(15)

--DECLARE @CompanyCode varchar(15) = '6006406'
--DECLARE @BranchCode varchar(15) = '6006401'

AS BEGIN
DECLARE @HighestPos varchar(15) = 
	CASE (SELECT COUNT(CompanyCode) FROM gnMstPosition
									WHERE CompanyCode = @CompanyCode
									AND DeptCode = 'SALES'
									AND (PosHeader IS NULL OR PosHeader = ''))
	WHEN 0 THEN (SELECT PosCode
				FROM gnMstPosition 
				WHERE CompanyCode = @CompanyCode AND DeptCode = 'SALES'
				AND PosHeader = (SELECT PosCode 
								FROM gnMstPosition
								WHERE DeptCode = 'SALES'
								AND PosHeader IS NULL OR PosHeader = ''))
	ELSE (SELECT PosCode FROM gnMstPosition
									WHERE CompanyCode = @CompanyCode
									AND DeptCode = 'SALES'
									AND (PosHeader IS NULL OR PosHeader = ''))
	END

SELECT * INTO #test1 FROM(
	SELECT a.BranchCode, a.EmployeeID, b.EmployeeName, e.PosLevel AS PositionID, b.Position, e.PosName AS PositionName,
	(rtrim(a.EmployeeID) + ' - ' + rtrim(b.EmployeeName)) Employee,
	isnull((
			select count(*) from PmKDP
			 where CompanyCode  = a.CompanyCode
			   and BranchCode   = a.BranchCode
			   and EmployeeID   = a.EmployeeID
			), 0) CountKDP, b.TeamLeader, ISNULL(f.OutletAbbreviation, a.BranchCode) AS BranchAbv
	FROM hrEmployeeMutation a
	JOIN (
		SELECT c.EmployeeId, c.EmployeeName, c.Position, ISNULL(c.TeamLeader, '') AS TeamLeader, MAX(d.MutationDate) AS MutationDate
		FROM hrEmployee c
		JOIN hrEmployeeMutation d
		ON c.EmployeeId = d.EmployeeId
		WHERE c.Department = 'SALES' AND c.PersonnelStatus = 1 AND c.IsDeleted = 0 AND d.IsDeleted = 0
		GROUP BY c.EmployeeId, c.EmployeeName, c.Position, c.TeamLeader
	) b
	ON a.EmployeeId = b.EmployeeId AND a.MutationDate = b.MutationDate
	JOIN gnMstPosition e
	ON a.CompanyCode = e.CompanyCode AND e.DeptCode = 'SALES' AND b.Position = e.PosCode
	JOIN gnMstDealerOutletMapping f
	ON a.CompanyCode = f.DealerCode AND a.BranchCode = f.OutletCode
	WHERE a.CompanyCode = @CompanyCode AND a.BranchCode = CASE @BranchCode WHEN '' THEN a.BranchCode ELSE @BranchCode END
UNION
	SELECT a.BranchCode, a.EmployeeID, b.EmployeeName, e.PosLevel AS PositionID, b.Position, e.PosName AS PositionName,
	(rtrim(a.EmployeeID) + ' - ' + rtrim(b.EmployeeName)) Employee,
	isnull((
			select count(*) from PmKDP
			 where CompanyCode  = a.CompanyCode
			   and BranchCode   = a.BranchCode
			   and EmployeeID   = a.EmployeeID
			), 0) CountKDP, b.TeamLeader, ISNULL(f.OutletAbbreviation, a.BranchCode) AS BranchAbv
	FROM hrEmployeeMutation a
	JOIN (
		SELECT c.EmployeeId, c.EmployeeName, c.Position, ISNULL(c.TeamLeader, '') AS TeamLeader, MAX(d.MutationDate) AS MutationDate
		FROM hrEmployee c
		JOIN hrEmployeeMutation d
		ON c.EmployeeId = d.EmployeeId
		WHERE c.Department = 'SALES' AND c.PersonnelStatus = 1 AND c.IsDeleted = 0 AND d.IsDeleted = 0
		AND c.Position = @HighestPos
		GROUP BY c.EmployeeId, c.EmployeeName, c.Position, c.TeamLeader
	) b
	ON a.EmployeeId = b.EmployeeId AND a.MutationDate = b.MutationDate
	JOIN gnMstPosition e
	ON a.CompanyCode = e.CompanyCode AND e.DeptCode = 'SALES' AND b.Position = e.PosCode
	JOIN gnMstDealerOutletMapping f
	ON a.CompanyCode = f.DealerCode AND a.BranchCode = f.OutletCode
	WHERE a.CompanyCode = @CompanyCode
)#test1

;WITH N(id, lvl, BranchCode, EmployeeID, EmployeeName, PositionID, Position, PositionName, Employee, CountKDP, TeamLeader, BranchAbv)
AS
(
	SELECT 
		CAST(row_number() OVER(ORDER BY a.EmployeeID) AS varchar) as id,
		0 AS lvl,
		a.BranchCode, a.EmployeeID, a.EmployeeName, a.PositionID, a.Position, a.PositionName, a.Employee, a.CountKDP, a.TeamLeader, a.BranchAbv
	FROM #test1 a
	WHERE a.Position = CASE (SELECT COUNT(c.EmployeeID) FROM #test1 c WHERE c.Position = @HighestPos ) 
							WHEN 0 THEN 
								(SELECT d.PosCode FROM gnMstPosition d 
								WHERE d.CompanyCode = @CompanyCode AND d.DeptCode = 'SALES'
								AND d.PosHeader = @HighestPos)
							ELSE @HighestPos
						END
	--TeamLeader = '' 
	--AND EXISTS (SELECT * FROM #test1 b WHERE b.TeamLeader = a.EmployeeID)
	UNION ALL
	SELECT 
		CAST(N.id + '.' + CAST(row_number() OVER(ORDER BY b.EmployeeID) AS varchar) AS varchar) as id,
		N.lvl + 1 AS lvl,
		b.BranchCode, b.EmployeeID, b.EmployeeName, b.PositionID, b.Position, b.PositionName, b.Employee, b.CountKDP, b.TeamLeader, b.BranchAbv
	FROM #test1 b JOIN N ON N.EmployeeID = b.TeamLeader
)
SELECT * FROM N ORDER BY PositionID DESC, BranchCode, id

DROP TABLE #test1
END
GO

DELETE FROM SysMenuDms WHERE menuid in ('inqsumm','itsclnupkdp','itsinpkdp','itsinq','itsinqbyperiode','itsinqfollowup','itsinqits','itsinqitsmkt','itsinqitsstatus','itsinqlostcase','itsinqoutstandingprospek','itsinqprod','itsinqsalesachievement','ItsInqSisHistory','itsmaster','itsmstorganization','itsmstOutlets','itsmstteammember','itstrans','itsutility','itsutlgenkdp','itsutltransferkdp','itsutluploadfile')
GO

INSERT INTO [SysMenuDms]([MenuId],[MenuCaption],[MenuHeader],[MenuIndex],[MenuLevel],[MenuUrl],[MenuIcon]) VALUES ('inqsumm','Inquiry Summary','itsinq',3,2,'inquiry/summary',NULL);
INSERT INTO [SysMenuDms]([MenuId],[MenuCaption],[MenuHeader],[MenuIndex],[MenuLevel],[MenuUrl],[MenuIcon]) VALUES ('itsclnupkdp','Clean Up ITS','itstrans',2,2,'trans/clnupkdp',NULL);
INSERT INTO [SysMenuDms]([MenuId],[MenuCaption],[MenuHeader],[MenuIndex],[MenuLevel],[MenuUrl],[MenuIcon]) VALUES ('itsinpkdp','Input KDP','itstrans',3,2,'trans/inputkdp',NULL);
INSERT INTO [SysMenuDms]([MenuId],[MenuCaption],[MenuHeader],[MenuIndex],[MenuLevel],[MenuUrl],[MenuIcon]) VALUES ('itsinq','Inquiry Prospect','its',2,1,'',NULL);
INSERT INTO [SysMenuDms]([MenuId],[MenuCaption],[MenuHeader],[MenuIndex],[MenuLevel],[MenuUrl],[MenuIcon]) VALUES ('itsinqbyperiode','Inquiry By Periode','itsinq',5,2,'inquiry/periode',NULL);
INSERT INTO [SysMenuDms]([MenuId],[MenuCaption],[MenuHeader],[MenuIndex],[MenuLevel],[MenuUrl],[MenuIcon]) VALUES ('itsinqfollowup','Inquiry Follow Up','itsinq',0,2,'inquiry/followup',NULL);
INSERT INTO [SysMenuDms]([MenuId],[MenuCaption],[MenuHeader],[MenuIndex],[MenuLevel],[MenuUrl],[MenuIcon]) VALUES ('itsinqits','Inquiry ITS','itsinq',6,2,'inquiry/InqIts',NULL);
INSERT INTO [SysMenuDms]([MenuId],[MenuCaption],[MenuHeader],[MenuIndex],[MenuLevel],[MenuUrl],[MenuIcon]) VALUES ('itsinqitsmkt','Inquiry ITS (Management)','itsinq',7,2,'inquiry/inqitsmkt',NULL);
INSERT INTO [SysMenuDms]([MenuId],[MenuCaption],[MenuHeader],[MenuIndex],[MenuLevel],[MenuUrl],[MenuIcon]) VALUES ('itsinqitsstatus','Inquiry ITS With Status','itsinq',8,2,'inquiry/inqitsstatus',NULL);
INSERT INTO [SysMenuDms]([MenuId],[MenuCaption],[MenuHeader],[MenuIndex],[MenuLevel],[MenuUrl],[MenuIcon]) VALUES ('itsinqlostcase','Inquiry Analisa Lost Case','itsinq',2,2,'inquiry/lostcase',NULL);
INSERT INTO [SysMenuDms]([MenuId],[MenuCaption],[MenuHeader],[MenuIndex],[MenuLevel],[MenuUrl],[MenuIcon]) VALUES ('itsinqoutstandingprospek','inquiry Outstanding Prospek','itsinq',4,2,'inquiry/outstandingprospek',NULL);
INSERT INTO [SysMenuDms]([MenuId],[MenuCaption],[MenuHeader],[MenuIndex],[MenuLevel],[MenuUrl],[MenuIcon]) VALUES ('itsinqprod','Inquiry Productivity','itsinq',1,2,'report/inqprod',NULL);
INSERT INTO [SysMenuDms]([MenuId],[MenuCaption],[MenuHeader],[MenuIndex],[MenuLevel],[MenuUrl],[MenuIcon]) VALUES ('itsinqsalesachievement','Inquiry Sales Achievement','itsinq',3,2,'inquiry/salesachievement',NULL);
INSERT INTO [SysMenuDms]([MenuId],[MenuCaption],[MenuHeader],[MenuIndex],[MenuLevel],[MenuUrl],[MenuIcon]) VALUES ('ItsInqSisHistory','SIS History','itsinq',5,2,'inquiry/sishistory',NULL);
INSERT INTO [SysMenuDms]([MenuId],[MenuCaption],[MenuHeader],[MenuIndex],[MenuLevel],[MenuUrl],[MenuIcon]) VALUES ('itsmaster','Master Prospect','its',0,1,'NULL',NULL);
INSERT INTO [SysMenuDms]([MenuId],[MenuCaption],[MenuHeader],[MenuIndex],[MenuLevel],[MenuUrl],[MenuIcon]) VALUES ('itsmstorganization','Organization','itsmaster',5,2,'master/organization',NULL);
INSERT INTO [SysMenuDms]([MenuId],[MenuCaption],[MenuHeader],[MenuIndex],[MenuLevel],[MenuUrl],[MenuIcon]) VALUES ('itsmstOutlets','Outlets','itsmaster',2,2,'master/outlets',NULL);
INSERT INTO [SysMenuDms]([MenuId],[MenuCaption],[MenuHeader],[MenuIndex],[MenuLevel],[MenuUrl],[MenuIcon]) VALUES ('itsmstteammember','Team Members','itsmaster',1,2,'master/teammember',NULL);
INSERT INTO [SysMenuDms]([MenuId],[MenuCaption],[MenuHeader],[MenuIndex],[MenuLevel],[MenuUrl],[MenuIcon]) VALUES ('itstrans','Transaction','its',1,1,'NULL',NULL);
INSERT INTO [SysMenuDms]([MenuId],[MenuCaption],[MenuHeader],[MenuIndex],[MenuLevel],[MenuUrl],[MenuIcon]) VALUES ('itsutility','Utility','its',3,1,'NULL',NULL);
INSERT INTO [SysMenuDms]([MenuId],[MenuCaption],[MenuHeader],[MenuIndex],[MenuLevel],[MenuUrl],[MenuIcon]) VALUES ('itsutlgenkdp','Generate KDP','itsutility',0,2,'utility/generatekdp',NULL);
INSERT INTO [SysMenuDms]([MenuId],[MenuCaption],[MenuHeader],[MenuIndex],[MenuLevel],[MenuUrl],[MenuIcon]) VALUES ('itsutltransferkdp','Transfer KDP','itsutility',3,2,'utility/transferkdp',NULL);
INSERT INTO [SysMenuDms]([MenuId],[MenuCaption],[MenuHeader],[MenuIndex],[MenuLevel],[MenuUrl],[MenuIcon]) VALUES ('itsutluploadfile','Upload File','itsutility',1,2,'utility/uploadfile',NULL);
GO

IF NOT EXISTS(select 1 from sysrole where roleid='ITS')
BEGIN
	INSERT INTO [dbo].[sysRole] ([RoleId],[RoleName],[Themes],[IsActive],[IsAdmin],[IsChangeBranchCode]) VALUES ('ITS','ITS','ITS',1,0,0)
END

IF NOT EXISTS(select 1 from [SysModule] where [ModuleId]='ITS')
BEGIN
	INSERT INTO [SysModule]([ModuleId],[ModuleCaption],[ModuleIndex],[ModuleUrl],[InternalLink],[IsPublish],[Icon])
	VALUES('its',	'Inquiry Tracking System',2,'',1,1,'fa fa-lg fa-fw fa-search')
END

IF NOT EXISTS(select 1 from [SysRoleModule] where RoleID='ITS' and [ModuleId]='ITS')
BEGIN
	insert into [SysRoleModule] values ('ITS','its')
END

delete from sysrolemenu where menuid in ('inqsumm','itsclnupkdp','itsinpkdp','itsinq','itsinqbyperiode','itsinqfollowup','itsinqits','itsinqitsmkt','itsinqitsstatus','itsinqlostcase','itsinqoutstandingprospek','itsinqprod','itsinqsalesachievement','ItsInqSisHistory','itsmaster','itsmstorganization','itsmstOutlets','itsmstteammember','itstrans','itsutility','itsutlgenkdp','itsutltransferkdp','itsutluploadfile')
GO

insert into sysRoleMenu (RoleId, MenuId) 
select 'ITS', Menuid from sysmenudms
where menuid in ('inqsumm','itsclnupkdp','itsinpkdp','itsinq','itsinqbyperiode','itsinqfollowup','itsinqits','itsinqitsmkt','itsinqitsstatus','itsinqlostcase','itsinqoutstandingprospek','itsinqprod','itsinqsalesachievement','ItsInqSisHistory','itsmaster','itsmstorganization','itsmstOutlets','itsmstteammember','itstrans','itsutility','itsutlgenkdp','itsutltransferkdp','itsutluploadfile')
GO

insert into sysRoleMenu (RoleId, MenuId) 
select 'ADMIN', Menuid from sysmenudms
where menuid in ('inqsumm','itsclnupkdp','itsinpkdp','itsinq','itsinqbyperiode','itsinqfollowup','itsinqits','itsinqitsmkt','itsinqitsstatus','itsinqlostcase','itsinqoutstandingprospek','itsinqprod','itsinqsalesachievement','ItsInqSisHistory','itsmaster','itsmstorganization','itsmstOutlets','itsmstteammember','itstrans','itsutility','itsutlgenkdp','itsutltransferkdp','itsutluploadfile')
GO

if object_id('uspfn_SysGenerateUser') is not null
	drop procedure uspfn_SysGenerateUser
GO
create procedure [dbo].[uspfn_SysGenerateUser]
AS
BEGIN
DECLARE @UNIQUEX UNIQUEIDENTIFIER,
	@UserID varchar(32),
	@CompanyCode varchar(20),
	@BranchCode varchar(20)

	declare CR_USER cursor for
	select BranchCode + '-ITS' a, CompanyCode, BranchCode
	from gnMstCoProfile 
	where CompanyCode = (Select top 1 companycode from gnMstOrganizationHdr)

	open CR_USER
	FETCH NEXT FROM CR_USER
	INTO @UserID, @CompanyCode, @BranchCode

	WHILE @@FETCH_STATUS = 0
	BEGIN

		INSERT INTO [dbo].[sysUser]([UserId],[Password],[FullName],[Email],[CompanyCode],[BranchCode],[TypeOfGoods],[IsActive],[RequiredChange])
		VALUES (@UserID,'202CB962AC59075B964B07152D234B70',@UserID,'',@CompanyCode, @BranchCode,'',1,0)
		
		insert [dbo].[SysRoleUser]([UserId], [RoleId]) values (@UserID, 'ITS')
				
		FETCH NEXT FROM CR_USER
		INTO @UserID, @CompanyCode, @BranchCode
	END

	CLOSE CR_USER
	DEALLOCATE CR_USER

END
GO

SELECT 'PREPARE SP for ITS DONE!!!'