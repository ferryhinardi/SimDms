-- exec usprpt_OmRpSalRgs039Web '20151201','20151231','JABODETABEK','6021406','602140604','010724.556','010722.318','010728.048','060715.036','FPOL'    
-- exec usprpt_OmRpSalRgs039Web '20151201','20151231','JABODETABEK','','','','','','','SALES'    
--select top 10 * from suzukir4..omhstinquirysalesnsds    
--select top 10 * from omInquirySales where fakturpolisidate is not null    
--drop table omInquirySales    
    
    
CREATE procedure [dbo].[usprpt_OmRpSalRgs039Web]            
 @StartDate Datetime,            
 @EndDate Datetime,            
 @Area varchar(100),            
 @Dealer varchar(100),            
 @Outlet varchar(100),            
 @BranchHead varchar(100),            
 @SalesHead varchar(100),            
 --@SalesCoordinator varchar(100),            
 @Salesman varchar(100),            
 @SalesType varchar(100)            
as        
    
if (@SalesType = 'FPOL')            
begin      
print 'FPOL';    
select     
 map.GroupNo           
   , map.Area          
   , inv.CompanyCode          
   , map.DealerName CompanyName          
   , vin.BranchCode          
   , otl.Outletname BranchName          
   , bhd.EmployeeId BranchHeadID          
   , bhd.EmployeeName BranchHeadName          
   , spv.EmployeeId SalesHeadID          
   , spv.EmployeeName SalesHeadName          
   , spv.EmployeeId SalesCoordinatorID          
   , spv.EmployeeName SalesCoordinatorName          
   , emp.EmployeeId SalesmanID          
   , emp.EmployeeName SalesmanName          
   , case when mdl.SalesmodelCode like '%CH%' or mdl.SalesModelCode like '%FD%' or mdl.SalesModelCode like '%PU%' or mdl.SalesModelCode  like '%WD%' then 'COMERCIAL' else 'PASSENGER' end ModelCatagory          
   , sso.SalesType          
   , inv.InvoiceNo          
   , inv.InvoiceDate          
   , sso.SoNo -- substring(inq.SONo,1,13) SoNo          
   , inq.SalesModelCode          
   , vin.SalesModelYear          
   , mdl.SalesModelDesc          
   , inq.FPNo FakturPolisiNo          
   , fpl.FakturPolisiDate          
   , mdl.FakturPolisiDesc          
   , mdl.SalesModelDesc MarketModel          
   , vin.ColourCode          
   , clr.RefferenceDesc1 ColourName          
   , mdl.GroupMarketModel       --inq.GroupMarketModel          
   , mdl.ColumnMarketModel      --inq.ColumnMarketModel          
   , emp.JoinDate          
   , emp.ResignDate          
   , null GradeDate       --emp.GradeDate          
   , case when isnull(grdDtl.LookUpValueName,'') <> '' then grdDtl.LookUpValueName else emp.Grade end Grade          
   , inq.ChassisCode            
   , inq.ChassisNo            
   , inq.EngineCode            
   , inq.EngineNo        
   , inq.DODate        
   , inq.GroupAreaCode       
   , inq.Processdate      
   , inq.isexists    
   , map.LastUpdateBy OtlMapLastUpdateBy    
   --into omInquirySales     
   from suzukir4..omhstinquirysalesnsds inq with (nolock, nowait)      
   inner join simdms..omtrsalesinvoicevin vin with (nolock, nowait)  on inq.ChassisCode= vin.ChassisCode  and inq.ChassisNo=vin.ChassisNo   and inq.EngineCode=vin.EngineCode and  inq.EngineNo =cast(vin.EngineNo as varchar)     
   inner join simdms..omtrsalesinvoice inv  with (nolock, nowait) on inv.companycode=vin.companycode and inv.branchcode=vin.branchcode and  vin.invoiceno=inv.invoiceno    
   inner join simdms..omtrsalesso sso  with (nolock, nowait) on sso.companycode=inv.companycode and sso.branchcode=inv.branchcode and inv.SONo=sso.SONo         
   left join simdms..ommstmodel mdl   on mdl.companycode=inv.companycode and mdl.salesmodelcode=vin.salesmodelcode       
   --left join simdms..omtrsalesDo sdo with(nolock,nowait) on sdo.companycode=sso.companycode and sdo.branchcode=sso.branchcode and sdo.sono=sso.sono    
   left join simdms..omtrsalesfakturpolisi fpl  with (nolock, nowait) on fpl.CompanyCode=vin.CompanyCode and  fpl.BranchCode=vin.BranchCode and fpl.FakturPolisiNo =inq.FPNo    
   left join simdms..hremployee  emp   on sso.companycode=emp.companycode and emp.employeeid=sso.salesman        
   left join simdms..hremployee  spv   on spv.companycode=emp.companycode and spv.employeeid=emp.teamleader     
   left join simdms..hremployee  bhd on bhd.companycode=spv.companycode and bhd.employeeid=spv.teamleader       
   Left Join GnMstLookUpDtl grdDtl  on vin.CompanyCode = grdDtl.CompanyCode           
    and grdDtl.CodeID = 'ITSG'          
    and grdDtl.LookUpValue = emp.Grade       
   left join omMstRefference clr   on clr.companycode=vin.companycode and clr.RefferenceCode=vin.colourcode and clr.RefferenceType='COLO'        
   left join gnMstDealerOutletMapping otl  on otl.DealerCode=vin.CompanyCode and otl.OutletCode = vin.BranchCode                                  
   left join gnMstDealerMapping map   on  map.DealerCode = vin.CompanyCode and map.GroupNo = otl.GroupNo                    
   where 1=1          
   and convert(varchar, inq.processdate, 112) between @StartDate and @EndDate       
   --and convert(varchar, inq.processdate, 112) between '20151201' and '20151231'    
   and inv.CompanyCode = case when @Dealer <> '' then @Dealer else  inv.CompanyCode end            
   and inv.BranchCode = case when @Outlet <> '' then @Outlet else inv.BranchCode end                
   and (map.Area = Case when @Area <> ''             
        then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')            
          then 'JABODETABEK'            
          else @Area end            
        else map.Area end            
   or map.Area = Case when @Area <> ''             
        then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')            
          then 'CABANG'            
          else @Area end            
        else map.Area end)             
   and bhd.employeeid = case when @BranchHead <> '' then @BranchHead else bhd.employeeid end              
   and spv.employeeid = case when @SalesHead <> '' then @SalesHead else spv.employeeid end                  
   --and (spv.employeeid = case when @SalesHead <> '' then @SalesHead else spv.employeeid end               
   --or spv.employeeid = case when @SalesCoordinator <> '' then @SalesCoordinator else spv.employeeid end)    
   and emp.employeeid = case when @Salesman <> '' then @Salesman else emp.employeeid  end               
   and isnull(mdl.SalesModelDesc,'') <> ''            
   ----   and inq.Status = '1'            
   --and map.isActive = 1             
   --order by inq.processdate,map.GroupNo,CompanyCode, vin.BranchCode, BranchHeadName, SalesHeadName, SalesCoordinatorName, SalesmanName, MarketModel                   
end    
else    
begin    
print 'NOT FPOL';    
select     
 map.GroupNo           
   , map.Area          
   , inv.CompanyCode          
   , map.DealerName CompanyName          
   , vin.BranchCode          
   , otl.Outletname BranchName          
   , bhd.EmployeeId BranchHeadID          
   , bhd.EmployeeName BranchHeadName          
   , spv.EmployeeId SalesHeadID          
   , spv.EmployeeName SalesHeadName          
   , spv.EmployeeId SalesCoordinatorID          
   , spv.EmployeeName SalesCoordinatorName          
   , emp.EmployeeId SalesmanID          
   , emp.EmployeeName SalesmanName          
   , case when mdl.SalesmodelCode like '%CH%' or mdl.SalesModelCode like '%FD%' or mdl.SalesModelCode like '%PU%' or mdl.SalesModelCode  like '%WD%' then 'COMERCIAL' else 'PASSENGER' end ModelCatagory          
   , sso.SalesType          
   , inv.InvoiceNo          
   , inv.InvoiceDate          
   , sso.SoNo -- substring(inq.SONo,1,13) SoNo          
   , inq.SalesModelCode          
   , vin.SalesModelYear          
   , mdl.SalesModelDesc          
   , inq.FPNo FakturPolisiNo          
   , fpl.FakturPolisiDate          
   , mdl.FakturPolisiDesc          
   , mdl.SalesModelDesc MarketModel          
   , vin.ColourCode          
   , clr.RefferenceDesc1 ColourName          
   , mdl.GroupMarketModel       --inq.GroupMarketModel          
   , mdl.ColumnMarketModel      --inq.ColumnMarketModel          
   , emp.JoinDate          
   , emp.ResignDate          
   , null GradeDate       --emp.GradeDate          
   , case when isnull(grdDtl.LookUpValueName,'') <> '' then grdDtl.LookUpValueName else emp.Grade end Grade          
   , inq.ChassisCode            
   , inq.ChassisNo            
   , inq.EngineCode            
   , inq.EngineNo        
   , inq.DODate        
   , inq.GroupAreaCode      
   , inq.Processdate      
   , inq.isexists    
   , map.LastUpdateBy OtlMapLastUpdateBy    
   --into omInquirySales     
   from suzukir4..omhstinquirysalesnsds inq with (nolock, nowait)      
   inner join simdms..omtrsalesinvoicevin vin with (nolock, nowait)  on inq.ChassisCode= vin.ChassisCode  and inq.ChassisNo=vin.ChassisNo   and inq.EngineCode=vin.EngineCode and  inq.EngineNo =cast(vin.EngineNo as varchar)     
   inner join simdms..omtrsalesinvoice inv  with (nolock, nowait) on inv.companycode=vin.companycode and inv.branchcode=vin.branchcode and  vin.invoiceno=inv.invoiceno    
   inner join simdms..omtrsalesso sso  with (nolock, nowait) on sso.companycode=inv.companycode and sso.branchcode=inv.branchcode and inv.SONo=sso.SONo         
   left join simdms..ommstmodel mdl   on mdl.companycode=inv.companycode and mdl.salesmodelcode=vin.salesmodelcode       
   --left join simdms..omtrsalesDo sdo with(nolock,nowait) on sdo.companycode=sso.companycode and sdo.branchcode=sso.branchcode and sdo.sono=sso.sono    
   left join simdms..omtrsalesfakturpolisi fpl  with (nolock, nowait) on fpl.CompanyCode=vin.CompanyCode and  fpl.BranchCode=vin.BranchCode and fpl.FakturPolisiNo =inq.FPNo    
   left join simdms..hremployee  emp   on sso.companycode=emp.companycode and emp.employeeid=sso.salesman        
   left join simdms..hremployee  spv   on spv.companycode=emp.companycode and spv.employeeid=emp.teamleader     
   left join simdms..hremployee  bhd on bhd.companycode=spv.companycode and bhd.employeeid=spv.teamleader       
   Left Join GnMstLookUpDtl grdDtl  on vin.CompanyCode = grdDtl.CompanyCode           
    and grdDtl.CodeID = 'ITSG'          
    and grdDtl.LookUpValue = emp.Grade       
   left join omMstRefference clr   on clr.companycode=vin.companycode and clr.RefferenceCode=vin.colourcode and clr.RefferenceType='COLO'        
   left join gnMstDealerOutletMapping otl  on otl.DealerCode=vin.CompanyCode and otl.OutletCode = vin.BranchCode                                  
   left join gnMstDealerMapping map   on  map.DealerCode = vin.CompanyCode and map.GroupNo = otl.GroupNo                    
   where 1=1               
   and convert(varchar, inv.InvoiceDate, 112) between @StartDate and @EndDate     
   and inv.CompanyCode = case when @Dealer <> '' then @Dealer else  inv.CompanyCode end            
   and inv.BranchCode = case when @Outlet <> '' then @Outlet else inv.BranchCode end                
   and (map.Area = Case when @Area <> ''             
        then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')            
          then 'JABODETABEK'            
          else @Area end            
        else map.Area end            
   or map.Area = Case when @Area <> ''             
        then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')            
          then 'CABANG'            
          else @Area end            
        else map.Area end)             
   and bhd.employeeid = case when @BranchHead <> '' then @BranchHead else bhd.employeeid end              
   and spv.employeeid = case when @SalesHead <> '' then @SalesHead else spv.employeeid end                  
   --and (spv.employeeid = case when @SalesHead <> '' then @SalesHead else spv.employeeid end               
   --or spv.employeeid = case when @SalesCoordinator <> '' then @SalesCoordinator else spv.employeeid end)    
   and emp.employeeid = case when @Salesman <> '' then @Salesman else emp.employeeid  end               
   and sso.SalesType= case when  @SalesType = 'WHOLESALE' then  0 else case when  @SalesType = 'SALES'  then 1 else sso.SalesType end end    
   and isnull(mdl.SalesModelDesc,'') <> ''            
   --and inq.Status = '1'            
   --and map.isActive = 1             
   order by inq.processdate,map.GroupNo,CompanyCode, vin.BranchCode, BranchHeadName, SalesHeadName, SalesCoordinatorName, SalesmanName, MarketModel                      
end    
     
    
        
    
       
    