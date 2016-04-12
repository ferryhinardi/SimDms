ALTER procedure [dbo].[uspfn_SaveOmHstInquirySales2]  
--DECLARE
 @CompanyCode  varchar(15),  
 @BranchCode   varchar(15),  
 @InvoiceNo   varchar(15),  
 @UserID    varchar(15)  
as  

--SELECT @CompanyCode = '6006400001', @BranchCode = '6006400110', @InvoiceNo = 'IU110/15/000584', @UserID = 'ga'

declare @CountData int  
 
select * into #temp from(  
 Select ti.SONo , tv.ChassisCode, tv.ChassisNo  
   from omTrSalesInvoice ti  
     left join omTrSalesInvoiceVin tv  
   on ti.CompanyCode=tv.CompanyCode  
     and ti.BranchCode=tv.BranchCode  
     and ti.InvoiceNo=tv.InvoiceNo  
 where ti.CompanyCode = @CompanyCode  
  and ti.BranchCode = @BranchCode  
  and ti.InvoiceNo = @InvoiceNo  
)#temp  
  
set @CountData = (select Count(*) from omHstInquirySales   
     where CompanyCode = @CompanyCode   
      and BranchCode = @BranchCode   
      and ChassisCode = (select TOP 1 ChassisCode from #temp)  
      and ChassisNo = (select Top 1 ChassisNo from #temp)  
      and SoNo like (select TOP 1 SoNo from #temp) + '%')  
print @CountData  
if(@CountData = 0)  
begin  
select * into #t1 from(  
select map.Area, map.DealerAbbreviation CompanyName, ti.CompanyCode, ti.BranchCode, ti.InvoiceNo,   
       ti.InvoiceDate, YEAR(ti.InvoiceDate) Year ,MONTH(ti.InvoiceDate) Month, ts.SODate, 
       tv.SalesModelCode, tv.SalesModelYear, my.SalesModelDesc, mm.FakturPolisiDesc,   
       case when loo.LookUpValueName is not null then loo.LookUpValueName else mm.SalesModelCode end "MarketModel",  
       tv.ColourCode, mc.RefferenceDesc1 ColourName, mc.RefferenceDesc2 ColourNameInd,  
       SalesType     = (case ts.SalesType  
                           when 1 then 'RETAIL'  
                           ELSE        'WHOLESALE'  
                        end),  
       ModelCatagory = (case   
                           when tv.SalesModelCode like '%PU%' then 'COMMERCIAL'  
                           when tv.SalesModelCode like '%FD%' then 'COMMERCIAL'  
                           when tv.SalesModelCode like '%WD%' then 'COMMERCIAL'  
                           when tv.SalesModelCode like '%CH%' then 'COMMERCIAL'  
                           else                                    'PASSENGER'  
                        end),  
       ts.Salesman SalesmanID  
       , me.EmployeeName SalesmanName  
       , mm.GroupMarketModel  
       , mm.ColumnMarketModel  
       , pm.JoinDate   
       , pm.ResignDate  
       , (select Top 1 convert(datetime,LEFT(TransactionID,8)) from pmSlsGradeHistory   
		   where  CompanyCode = ti.CompanyCode  
			and BranchCode = ti.BranchCode  
			and EmployeeID = pm.EmployeeID  
		   order by TransactionID desc) GradeDate  
		, (select TOP 1 grdDtl.LookUpValueName 
			from GnMstLookUpDtl grdDtl 
			where ti.CompanyCode = grdDtl.CompanyCode  
				and grdDtl.CodeID = 'ITSG'  
				and grdDtl.LookUpValue = grd.CurrentGrade)Grade  
		, tv.ChassisCode  
		, tv.ChassisNo  
		, tv.EngineCode  
		, tv.EngineNo  
		, tv.COGS
		, mdl.BeforeDiscDPP
		, mdl.DiscExcludePPn
		, mdl.DiscIncludePPn
		, mdl.AfterDiscDPP
		, mdl.AfterDiscPPn
		, mdl.AfterDiscPPnBM
		, mdl.AfterDiscTotal
		, mdl.PPnBMPaid
		, mdl.OthersDPP
		, mdl.OthersPPn
		, mdl.ShipAmt
		, mdl.DepositAmt
		, mdl.OthersAmt
		, reqDet.FakturPolisiNo
		, reqDet.FakturPolisiDate
		, ti.SONo + case when cst.CategoryCode = '00' then 'MD' else case when cst.CategoryCode = '01' then 'SD' else '' end end SoNo
		, cst.CategoryCode
  from omTrSalesInvoice ti  
  left join omTrSalesInvoiceVin tv  
    on ti.CompanyCode=tv.CompanyCode  
   and ti.BranchCode=tv.BranchCode  
   and ti.InvoiceNo=tv.InvoiceNo  
  left join omTrSalesSO ts  
    on ti.CompanyCode=ts.CompanyCode  
   and ti.BranchCode=ts.BranchCode  
   and ti.SONo=ts.SONo  
  left join omMstModel mm  
    on ti.CompanyCode=mm.CompanyCode  
   and tv.SalesModelCode=mm.SalesModelCode  
  left join omMstModelYear my  
    on ti.CompanyCode=my.CompanyCode  
   and tv.SalesModelCode=my.SalesModelCode  
   and tv.SalesModelYear=my.SalesModelYear  
  left join omMstRefference mc  
    on ti.CompanyCode=mc.CompanyCode  
	and mc.RefferenceType='COLO'  
	and mc.RefferenceCode=tv.ColourCode  
  left join HrEmployee me  
    on ti.CompanyCode=me.CompanyCode  
	and ts.Salesman=me.EmployeeID  
  left join HrEmployee pm  
    on ti.CompanyCode=pm.CompanyCode  
	and ts.Salesman=pm.EmployeeID  
  left join gnMstDealerMapping map  
	on map.DealerCode = ti.CompanyCode  
  left join GnMstLookUpDtl loo on ti.CompanyCode = loo.CompanyCode  
	and loo.CodeID = 'MKTCD'  
	and loo.LookUpValue = mm.MarketModelCode  
  left join pmSalesmanGrade grd on ti.CompanyCode = grd.CompanyCode  
	and ti.BranchCode = grd.BranchCode  
	and pm.EmployeeID = grd.EmployeeID  
  Left Join omTrSalesInvoiceModel mdl on ti.CompanyCode = mdl.CompanyCode
	and ti.BranchCode = mdl.BranchCode
	and ti.InvoiceNo = mdl.InvoiceNo
	and tv.BPKNo = mdl.BPKNo
  Left Join OmTrSalesReqDetail reqDet on ti.CompanyCode = reqDet.CompanyCode
	and ti.BranchCode = reqDet.BranchCode
	and tv.ChassisCode = reqDet.ChassisCode
	and tv.ChassisNo = reqDet.ChassisNo
	and ti.SoNo = reqDet.SoNo
  Left join GnMstCustomer cst on ti.CompanyCode = cst.CompanyCode
	and ti.CustomerCode = cst.CustomerCode
 where 1=1  
   and ti.CompanyCode = @CompanyCode  
   and ti.BranchCode = @BranchCode  
   and ti.InvoiceNo = @InvoiceNo  
) #t1  

select * into #t2   
from (  
select pp.CompanyCode, pp.BranchCode, pp.EmployeeID BranchHeadID, ge.EmployeeName BranchHeadName, pt.TeamID  
  from pmPosition pp  
  left join HrEmployee ge  
    on pp.CompanyCode = ge.CompanyCode  
   and pp.EmployeeID = ge.EmployeeID  
   and ge.PersonnelStatus = '1'  
  left join pmMstTeamMembers pt  
    on pp.CompanyCode = pt.CompanyCode  
   and pp.BranchCode = pt.BranchCode  
   and pp.EmployeeID = pt.EmployeeID  
   and pt.IsSupervisor = '1'  
 where pp.PositionId = '40'  
) #t2  
  
select * into #t3  
from (  
select pp.CompanyCode, pp.BranchCode, pp.BranchHeadID, pp.BranchHeadName,  
       pt.EmployeeID SalesHeadID, ge.EmployeeName SalesHeadName, pd.TeamID  
  from #t2 pp  
  left join pmMstTeamMembers pt  
    on pp.CompanyCode=pt.CompanyCode  
   and pp.BranchCode=pt.BranchCode  
   and pp.TeamID=pt.TeamID  
   and pt.IsSupervisor=0  
  left join pmMstTeamMembers pd  
    on pp.CompanyCode = pd.CompanyCode  
   and pp.BranchCode = pd.BranchCode  
   and pt.EmployeeID = pd.EmployeeID  
   and pd.IsSupervisor = 1  
  left join HrEmployee ge  
    on pp.CompanyCode = ge.CompanyCode  
   and pt.EmployeeID = ge.EmployeeID  
   and ge.PersonnelStatus = 1  
) #t3  
  
select * into #t4  
from (  
select pp.CompanyCode, pp.BranchCode, pp.BranchHeadID, pp.BranchHeadName, pp.SalesHeadID, pp.SalesHeadName,   
       pt.EmployeeID SalesCoordinatorID, ge.EmployeeName SalesCoordinatorName, pd.TeamID  
  from #t3 pp  
  left join pmMstTeamMembers pt  
    on pp.CompanyCode=pt.CompanyCode  
   and pp.BranchCode=pt.BranchCode  
   and pp.TeamID=pt.TeamID  
   and pt.IsSupervisor='0'
  left join pmMstTeamMembers pd  
    on pp.CompanyCode = pd.CompanyCode  
   and pp.BranchCode = pd.BranchCode  
   and pt.EmployeeID = pd.EmployeeID  
   and pd.IsSupervisor = '1'  
  left join HrEmployee ge  
    on pp.CompanyCode = ge.CompanyCode  
   and pt.EmployeeID = ge.EmployeeID  
   and ge.PersonnelStatus = '1'  
) #t4  
  
select * into #t5  
from (  
select pp.CompanyCode, pp.BranchCode, pp.BranchHeadID, pp.BranchHeadName, pp.SalesHeadID, pp.SalesHeadName,   
       pp.SalesCoordinatorID, pp.SalesCoordinatorName, pt.EmployeeID SalesmanID, ge.EmployeeName SalesmanName  
  from #t4 pp  
  left join pmMstTeamMembers pt  
    on pp.CompanyCode=pt.CompanyCode  
   and pp.BranchCode=pt.BranchCode  
   and pp.TeamID=pt.TeamID  
   and pt.IsSupervisor='0'  
  left join pmMstTeamMembers pd  
    on pp.CompanyCode = pd.CompanyCode  
   and pp.BranchCode = pd.BranchCode  
   and pt.EmployeeID = pd.EmployeeID  
   and pd.IsSupervisor = '1'  
  left join HrEmployee ge  
    on pp.CompanyCode = ge.CompanyCode  
   and pt.EmployeeID = ge.EmployeeID  
   and ge.PersonnelStatus = '1' 
) #t5  
  
INSERT INTO omHstInquirySales
           ([Year]
           ,[Month]
           ,[CompanyCode]
           ,[BranchCode]
           ,[CompanyName]
           ,[BranchName]
           ,[Area]
           ,[BranchHeadID]
           ,[BranchHeadName]
           ,[SalesHeadID]
           ,[SalesHeadName]
           ,[SalesCoordinatorID]
           ,[SalesCoordinatorName]
           ,[SalesmanID]
           ,[SalesmanName]
           ,[JoinDate]
           ,[ResignDate]
           ,[GradeDate]
           ,[Grade]
           ,[ModelCatagory]
           ,[SalesType]
           ,[SoNo]
           ,[SODate]
           ,[InvoiceNo]
           ,[InvoiceDate]
           ,[FakturPolisiNo]
           ,[FakturPolisiDate]
           ,[SalesModelCode]
           ,[SalesModelYear]
           ,[SalesModelDesc]
           ,[FakturPolisiDesc]
           ,[MarketModel]
           ,[GroupMarketModel]
           ,[ColumnMarketModel]
           ,[ChassisCode]
           ,[ChassisNo]
           ,[EngineCode]
           ,[EngineNo]
           ,[ColourCode]
           ,[ColourName]
           ,[COGS]
           ,[BeforeDiscDPP]
           ,[DiscExcludePPn]
           ,[DiscIncludePPn]
           ,[AfterDiscDPP]
           ,[AfterDiscPPn]
           ,[AfterDiscPPnBM]
           ,[AfterDiscTotal]
           ,[PPnBMPaid]
           ,[OthersDPP]
           ,[OthersPPn]
           ,[ShipAmt]
           ,[DepositAmt]
           ,[OthersAmt]
           ,[Status]
           ,[DCSStatus]
           ,[DCSDate]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastUpdateBy]
           ,[LastUpdateDate]
           ,[CategoryCode]
           ,[SuzukiDODate]
           ,[SuzukiFPolDate])          
select YEAR(InvoiceDate) Year  
 , Month(InvoiceDate) Month  
 , gh.CompanyCode  
 , gm.BranchCode  
 , tr.CompanyName  
 , outlet.OutletAbbreviation BranchName  
 , map.Area Area  
 , ms.BranchHeadID 
 , ms.BranchHeadName  
 , ms.SalesHeadID  
 , ms.SalesHeadName  
 , ms.SalesCoordinatorID  
 , ms.SalesCoordinatorName  
 , tr.SalesmanID  
 , tr.SalesmanName  
 , tr.JoinDate  
 , tr.ResignDate  
 , tr.GradeDate  
 , tr.Grade  
 , tr.ModelCatagory  
 , tr.SalesType  
 , tr.SONo  
 , tr.SODate 
 , tr.InvoiceNo  
 , tr.InvoiceDate  
 , tr.FakturPolisiNo
 , tr.FakturPolisiDate
 , tr.SalesModelCode  
 , tr.SalesModelYear  
 , tr.SalesModelDesc  
 , tr.FakturPolisiDesc  
 , tr.MarketModel  
 , tr.GroupMarketModel  
 , tr.ColumnMarketModel 
 , tr.ChassisCode
 , tr.ChassisNo
 , tr.EngineCode
 , tr.EngineNo 
 , tr.ColourCode  
 , substring(tr.ColourName,1,50) ColourName 
 , tr.COGS
 , tr.BeforeDiscDPP
 , tr.DiscExcludePPn
 , tr.DiscIncludePPn
 , tr.AfterDiscDPP
 , tr.AfterDiscPPn
 , tr.AfterDiscPPnBM
 , tr.AfterDiscTotal
 , tr.PPnBMPaid
 , tr.OthersDPP
 , tr.OthersPPn
 , tr.ShipAmt
 , tr.DepositAmt
 , tr.OthersAmt
 , 1  
 , 0
 , '19000101'
 , @UserID  
 , getdate()  
 , @UserID  
 , getdate()
 , tr.CategoryCode
 , '19000101'
 , '19000101'
  from #t1 tr  
  left join gnMstOrganizationHdr gh  
    on tr.CompanyCode=gh.CompanyCode  
  left join gnMstOrganizationDtl gm  
    on tr.CompanyCode=gm.CompanyCode  
   and tr.BranchCode=gm.BranchCode  
  left join #t5 ms  
    on tr.CompanyCode=ms.CompanyCode  
   and tr.BranchCode=ms.BranchCode  
   and tr.SalesmanID=ms.SalesmanID  
  left join gnMstDealerMapping map  
 on map.DealerCode = gh.CompanyCode  
  left join gnMstDealerOutletMapping outlet  
 on tr.CompanyCode = outlet.DealerCode  
 and tr.BranchCode = outlet.OutletCode  
  
drop table #t1  
drop table #t2  
drop table #t3  
drop table #t4  
drop table #t5  
end  
else  
begin  
 select * into #t6 from(  
  select ti.InvoiceNo, ti.InvoiceDate , ti.SONo, tv.ChassisCode, tv.ChassisNo  
     from omTrSalesInvoice ti  
    left join omTrSalesInvoiceVin tv  
     on ti.CompanyCode=tv.CompanyCode  
       and ti.BranchCode=tv.BranchCode  
       and ti.InvoiceNo=tv.InvoiceNo  
   where ti.CompanyCode = @CompanyCode  
    and ti.BranchCode = @BranchCode  
    and ti.InvoiceNo = @InvoiceNo  
 )#t6
 
  update omHstInquirySales  
  set InvoiceNo = (select top 1 InvoiceNo from #t6)  
   , InvoiceDate = (select top 1 InvoiceDate from #t6)
   , LastUpdateBy = @UserID
   , LastUpdateDate = getdate()  
   , DCSStatus = '0'
  where CompanyCode = @CompanyCode   
   and BranchCode = @BranchCode  
   and ChassisCode = (select TOP 1 ChassisCode from #t6)  
   and ChassisNo = (select TOP 1 ChassisNo from #t6)  
   and SoNo like (select TOP 1 SoNo from #t6) +'%'
end 
