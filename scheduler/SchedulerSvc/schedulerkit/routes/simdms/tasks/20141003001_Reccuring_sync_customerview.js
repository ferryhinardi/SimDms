var moment = require('moment');
var async = require('async');
var request = require('request');
var path = require('path');
var fs = require('fs');
var util = require('util');
var archiver = require('archiver');
var sqlODBC = require('node-sqlserver');
var config = require('../config');
var argv = require('yargs')
    .string('d')
    .argv;
	
var TaskName = "Exec uspfn_SyncCsCustomerView";

var CurrentDealerCode = argv.d;

function hereDoc(f) {
  return f.toString().
	  replace(/^[^\/]+\/\*!?/, '').
	  replace(/\*\/[^\/]+$/, '');
}

var SQL = hereDoc(function(){/*!
if object_id('uspfn_SyncCsCustomerView') is  null
BEGIN
create procedure uspfn_SyncCsCustomerView
as
begin
	;with x as (
	select a.CompanyCode
		 , BranchCode = isnull((
			select top 1 BranchCode from OmTrSalesSo
			 where CompanyCode = a.CompanyCode
			  and CustomerCode = a.CustomerCode
			order by SODate desc), '')
		 , a.CustomerCode
		 , a.CustomerName
		 , a.CustomerType
		 , rtrim(a.Address1) + ' ' + rtrim(a.Address2) + rtrim(a.Address3) as Address
		 , a.PhoneNo
		 , a.HPNo
		 , b.AddPhone1
		 , b.AddPhone2
		 , a.BirthDate
		 , b.ReligionCode
		 , a.CreatedDate
		 , a.LastUpdateDate
	  from GnMstCustomer a
	  left join CsCustData b
		on b.CompanyCode = a.CompanyCode
	   and b.CustomerCode = a.CustomerCode
	 where 1 = 1
	   and a.CustomerType = 'I'
	   and a.BirthDate is not null
	   and a.BirthDate > '1900-01-01'
	   and (year(getdate() - year(a.BirthDate))) > 5
	   and year(a.LastUpdateDate) = year(getdate())
	)
	select * into #t1 from (select * from x where BranchCode != '')#

	delete CsCustomerView
	 where exists (
		select top 1 1 from #t1
		 where #t1.CompanyCode = CsCustomerView.CompanyCode
		   and #t1.BranchCode = CsCustomerView.BranchCode
		   and #t1.CustomerCode = CsCustomerView.CustomerCode
	 )
	insert into CsCustomerView (CompanyCode, BranchCode, CustomerCode, CustomerName, CustomerType, Address, PhoneNo, HPNo, AddPhone1, AddPhone2, BirthDate, ReligionCode, CreatedDate, LastUpdateDate)
	select * from #t1

	--drop table CsCustomerView
	--select * into CsCustomerView from #t1


	drop table #t1
end


go
if object_id('uspfn_SyncCsCustomerViewInitialize') is not null
	drop procedure uspfn_SyncCsCustomerViewInitialize

go
create procedure uspfn_SyncCsCustomerViewInitialize
as
begin
	;with x as (
	select a.CompanyCode
		 , BranchCode = isnull((
			select top 1 BranchCode from OmTrSalesSo
			 where CompanyCode = a.CompanyCode
			  and CustomerCode = a.CustomerCode
			order by SODate desc), '')
		 , a.CustomerCode
		 , a.CustomerName
		 , a.CustomerType
		 , rtrim(a.Address1) + ' ' + rtrim(a.Address2) + rtrim(a.Address3) as Address
		 , a.PhoneNo
		 , a.HPNo
		 , b.AddPhone1
		 , b.AddPhone2
		 , a.BirthDate
		 , b.ReligionCode
		 , a.CreatedDate
		 , a.LastUpdateDate
	  from GnMstCustomer a
	  left join CsCustData b
		on b.CompanyCode = a.CompanyCode
	   and b.CustomerCode = a.CustomerCode
	 where 1 = 1
	   and a.CustomerType = 'I'
	   and a.BirthDate is not null
	   and a.BirthDate > '1900-01-01'
	   and (year(getdate() - year(a.BirthDate))) > 5
	   and year(a.LastUpdateDate) = year(getdate())
	)
	select * into #t1 from (select * from x where BranchCode != '')#

	select * into CsCustomerView from #t1

	drop table #t1
end
END
GO
select 'EXEC uspfn_SyncCsCustomerView' info
GO
exec uspfn_SyncCsCustomerView
GO
if object_id('uspfn_SyncCsCustomerVehicleView') is null
BEGIN
create procedure uspfn_SyncCsCustomerVehicleView
as
;with r as (
select a.CompanyCode
     , a.ChassisCode
	 , a.ChassisNo
	 , BranchCode = (select top 1 x.BranchCode from omTrSalesDODetail x, omTrSalesDO y
					  where x.CompanyCode = y.CompanyCode
					    and x.BranchCode = y.BranchCode
					    and x.DONo = y.DONo
					    and x.CompanyCode = a.CompanyCode
					    and x.ChassisCode = a.ChassisCode
						and x.ChassisNo = a.ChassisNo
					  order by x.LastUpdateDate desc)
	 , DoNo = (select top 1 x.DONo from omTrSalesDODetail x, omTrSalesDO y
					  where x.CompanyCode = y.CompanyCode
					    and x.BranchCode = y.BranchCode
					    and x.DONo = y.DONo
					    and x.CompanyCode = a.CompanyCode
					    and x.ChassisCode = a.ChassisCode
						and x.ChassisNo = a.ChassisNo
					  order by x.LastUpdateDate desc)
	 , DoSeq = (select top 1 x.DOSeq from omTrSalesDODetail x, omTrSalesDO y
					  where x.CompanyCode = y.CompanyCode
					    and x.BranchCode = y.BranchCode
					    and x.DONo = y.DONo
					    and x.CompanyCode = a.CompanyCode
					    and x.ChassisCode = a.ChassisCode
						and x.ChassisNo = a.ChassisNo
					  order by x.LastUpdateDate desc)
  from omTrSalesDODetail a
 where 1 = 1
   and (year(a.LastUpdateDate) = year(getdate()) or (year(a.LastUpdateDate) + 1) = year(getdate()))
 group by a.CompanyCode, a.ChassisCode, a.ChassisNo
),
s as (
select r.CompanyCode
	 , r.BranchCode
	 , c.CustomerCode
	 , r.ChassisCode + convert(varchar, r.ChassisNo) as Chassis
	 , b.EngineCode + convert(varchar, b.EngineNo) as Engine
	 , c.SONo
	 , c.DONo
	 , c.DODate
	 , BpkNo = isnull((select top 1 BPKNo from OmTrSalesBpk
	                    where CompanyCode = r.CompanyCode
						  and BranchCode = r.BranchCode
						  and DONo = r.DoNo
						  and SONo = c.SONo
						order by LastUpdateDate desc), '')
	 , SalesmanCode = isnull((select top 1 Salesman from omTrSalesSO
	                    where CompanyCode = r.CompanyCode
						  and BranchCode = r.BranchCode
						  and SONo = c.SONo
						order by LastUpdateDate desc), '')
     , b.SalesModelCode
     , b.SalesModelYear
     , b.ColourCode
	 , r.ChassisCode
	 , r.ChassisNo
  from r
  join omTrSalesDODetail b
    on b.CompanyCode = r.CompanyCode
   and b.BranchCode = r.BranchCode
   and b.DONo = r.DoNo
   and b.DOSeq = r.DoSeq
  join omTrSalesDO c
    on c.CompanyCode = b.CompanyCode
   and c.BranchCode = b.BranchCode
   and c.DONo = b.DONo
 where b.StatusBPK != '3'
),
t as (
select s.CompanyCode
     , s.BranchCode 
	 , s.CustomerCode
	 , s.Chassis
	 , s.Engine
	 , s.SONo
	 , s.DoNo
	 , s.DoDate
	 , s.BpkNo
     , s.SalesModelCode as CarType
     , s.ColourCode as Color
	 , s.SalesmanCode
	 , b.EmployeeName as SalesmanName
	 , c.PoliceRegNo
	 , s.DODate as DeliveryDate
     , s.SalesModelCode
     , s.SalesModelYear
     , s.ColourCode
	 , d.BpkDate
	 , e.isLeasing
	 , e.LeasingCo
	 , f.CustomerName as LeasingName
	 , e.Installment
  from s with (nolock, nowait)
  left join HrEmployee b
    on b.CompanyCode = s.CompanyCode
   and b.EmployeeID = s.SalesmanCode
  left join svMstCustomerVehicle c
    on c.CompanyCode = s.CompanyCode
   and c.ChassisCode = s.ChassisCode
   and c.ChassisNo = s.ChassisNo
  join omTrSalesBPK d
    on d.CompanyCode = s.CompanyCode
   and d.BranchCode = s.BranchCode
   and d.BpkNo = s.BpkNo
  join omTrSalesSO e
    on e.CompanyCode = s.CompanyCode
   and e.BranchCode = s.BranchCode
   and e.SONo = s.SONo
  left join gnMstCustomer f
    on f.CompanyCode = e.CompanyCode
   and f.CustomerCode = e.LeasingCo
 where isnull(d.Status, 3) != '3'
)
select * into #t1 from (select * from t)#

delete CsCustomerVehicleView
 where exists (
	select top 1 1 from #t1
	 where #t1.CompanyCode = CsCustomerVehicleView.CompanyCode
	   and #t1.BranchCode = CsCustomerVehicleView.BranchCode
	   and #t1.Chassis = CsCustomerVehicleView.Chassis
 )
insert into CsCustomerVehicleView (CompanyCode, BranchCode, CustomerCode, Chassis, Engine, SONo, DONo, DoDate, BpkNo, CarType, Color, SalesmanCode, SalesmanName, PoliceRegNo, DeliveryDate, SalesModelCode, SalesModelYear, ColourCode, BpkDate, IsLeasing, LeasingCo, LeasingName, Installment)
select * from #t1

drop table #t1
END
GO
select 'EXEC uspfn_SyncCsCustomerVehicleView' info
GO
exec uspfn_SyncCsCustomerVehicleView
GO
*/});

var startTasks= function(callback)
{
    var taskJobs = [];		
    config.conn().forEach(listWorker);		
    async.series(taskJobs, function (err, docs) {
        if (err) config.log("Tasks", err);			
        if (callback) callback();
    });
    function listWorker(cfg){
        if(cfg.DealerCode == CurrentDealerCode)
        {
            taskJobs.push(function(callback){start(cfg, callback);});
        }			 
    }				
}
var start = function (cfg, callback) {

    config.log("Tasks", "Starting " + TaskName + " for " + CurrentDealerCode); 

    var xSQL= [], sql = SQL.split('\nGO');	
    sql.forEach(ExecuteSQL);

    async.series(xSQL, function (err, docs) {
        if (err) config.log("ERROR", err);	
        config.log("Tasks", TaskName + " has been executed"); 			
        if (callback) callback();
    });	

    function ExecuteSQL(s){
        xSQL.push(function(callback){
            sqlODBC.query(cfg.ConnString,s , function (err, data) {    
                if (err) config.log("Tasks", err);                  
                callback(); 
            });		
        });
    }	
}
startTasks();