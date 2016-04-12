  
  
CREATE procedure [dbo].[uspfn_SyncOmInquirySales]      
 @MonthInterval int = 6      
as      
      
    
 declare @StartDate varchar(8)      
 declare @EndDate   varchar(8)      
       
   
 set @EndDate = convert(varchar(8), getdate(), 112);      
 set @StartDate = convert(varchar(8), dateadd(month, -(@MonthInterval), getdate()), 112);      
   
 delete from omInquirySales where Processdate  between @StartDate and @EndDate  
      
insert into  omInquirySales  
  select                              
   -- map.GroupNo                     
   --, map.Area                    
    inv.CompanyCode                    
   --, map.DealerName CompanyName                    
   , vin.BranchCode                    
   --, otl.Outletname BranchName                    
   , bhd.EmployeeId BranchHeadID                    
   --, bhd.EmployeeName BranchHeadName                    
   , spv.EmployeeId SalesHeadID                    
   --, spv.EmployeeName SalesHeadName                    
   , spv.EmployeeId SalesCoordinatorID                    
   --, spv.EmployeeName SalesCoordinatorName                    
   , emp.EmployeeId SalesmanID                    
   --, emp.EmployeeName SalesmanName                    
   , case when mdl.SalesmodelCode like '%CH%' or mdl.SalesModelCode like '%FD%' or mdl.SalesModelCode like '%PU%' or mdl.SalesModelCode  like '%WD%' then 'COMERCIAL' else 'PASSENGER' end ModelCatagory                   , sso.SalesType                    
   , inv.InvoiceNo                    
   , inv.InvoiceDate                    
   , sso.SoNo    
   , inq.SalesModelCode                    
   , vin.SalesModelYear                    
   , mdl.SalesModelDesc                    
   , inq.FPNo FakturPolisiNo                    
   , fpl.FakturPolisiDate                    
   , mdl.FakturPolisiDesc                    
   ,  isnull(mdl.GroupCode,'') MarketModel                    
   , vin.ColourCode                    
   , isnull(clr.RefferenceDesc1,'') ColourName                    
   , isnull(mdl.GroupMarketModel,'')   GroupMarketModel                   
   , isnull(mdl.ColumnMarketModel,'')   ColumnMarketModel                     
   , emp.JoinDate                    
   , emp.ResignDate             
   , null GradeDate               
   , case when isnull(grdDtl.LookUpValueName,'') <> '' then grdDtl.LookUpValueName else emp.Grade end Grade                    
   , inq.ChassisCode                      
   , inq.ChassisNo                      
   , inq.EngineCode                      
   , inq.EngineNo                  
   , inq.DODate                  
   , inq.GroupAreaCode                
   , inq.Processdate                
   , inq.isexists              
   --, map.LastUpdateBy OtlMapLastUpdateBy                                          
   from suzukir4..omhstinquirysalesnsds inq with (nolock, nowait)                
   inner join simdms..omtrsalesinvoicevin vin with (nolock, nowait)  on inq.ChassisCode= vin.ChassisCode  and inq.ChassisNo=vin.ChassisNo   and inq.EngineCode=vin.EngineCode and  inq.EngineNo =cast(vin.EngineNo as varchar)               
   inner join simdms..omtrsalesinvoice inv  with (nolock, nowait) on inv.companycode=vin.companycode and inv.branchcode=vin.branchcode and  vin.invoiceno=inv.invoiceno              
   inner join simdms..omtrsalesso sso  with (nolock, nowait) on sso.companycode=inv.companycode and sso.branchcode=inv.branchcode and inv.SONo=sso.SONo                   
   left join simdms..ommstmodel mdl   on mdl.companycode=inv.companycode and mdl.salesmodelcode=vin.salesmodelcode                    
   left join simdms..omtrsalesfakturpolisi fpl  with (nolock, nowait) on fpl.CompanyCode=vin.CompanyCode and  fpl.BranchCode=vin.BranchCode and fpl.FakturPolisiNo =inq.FPNo              
   left join simdms..hremployee  emp   on sso.companycode=emp.companycode and emp.employeeid=sso.salesman                  
   left join simdms..hremployee  spv   on spv.companycode=emp.companycode and spv.employeeid=emp.teamleader               
   left join simdms..hremployee  bhd on bhd.companycode=spv.companycode and bhd.employeeid=spv.teamleader                 
   Left Join GnMstLookUpDtl grdDtl  on vin.CompanyCode = grdDtl.CompanyCode                     
    and grdDtl.CodeID = 'ITSG'                    
    and grdDtl.LookUpValue = emp.Grade                 
   left join omMstRefference clr   on clr.companycode=vin.companycode and clr.RefferenceCode=vin.colourcode and clr.RefferenceType='COLO'                     
   where 1=1                   
   and   convert(varchar,inq.processdate,112) between @StartDate and @EndDate  
   order by inq.processdate,CompanyCode, vin.BranchCode,  MarketModel                                
     
     