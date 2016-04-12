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
