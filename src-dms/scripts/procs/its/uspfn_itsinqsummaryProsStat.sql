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
 