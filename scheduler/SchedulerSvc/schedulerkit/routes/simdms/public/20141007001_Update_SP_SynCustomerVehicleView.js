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
	
var TaskName = "UPDATE uspfn_SyncCsCustomerVehicleView";
var TaskNo = "20141007001";
var CurrentDealerCode = argv.d;

function hereDoc(f) {
  return f.toString().
	  replace(/^[^\/]+\/\*!?/, '').
	  replace(/\*\/[^\/]+$/, '');
}

var SQLCheck = hereDoc(function(){/*!
declare @column_list varchar(MAX)
SELECT @column_list = COALESCE(@column_list + ', ', '') + COLUMN_NAME 
FROM INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='#TABLENAME#'
SELECT @column_list list, COUNT(*) total
FROM INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='#TABLENAME#'
*/});

var SQL = hereDoc(function(){/*!
ALTER procedure [dbo].[uspfn_SyncCsCustomerVehicleView]
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

declare @ccode varchar(20), @bday datetime, @companyCode varchar(20)

declare UpdateBirthDay cursor for
select a.CustomerCode, b.skpkbirthday, b.CompanyCode from #t1 a
inner join omTrSalesReqDetail b on (a.bpkno=b.bpkno and a.CompanyCode=b.CompanyCode and a.BranchCode=b.BranchCode and a.sono=b.sono)

OPEN UpdateBirthDay
FETCH NEXT FROM UpdateBirthDay
INTO @ccode, @bday, @companyCode

WHILE @@FETCH_STATUS = 0
BEGIN

	update gnMstCustomer
	set BirthDate=@bday
	where CompanyCode=@companyCode and CustomerCode=@ccode and BirthDate <> @bday

	update CsCustomerView
	set BirthDate=@bday
	where CompanyCode=@companyCode and CustomerCode=@ccode and BirthDate <> @bday

	FETCH NEXT FROM UpdateBirthDay
	INTO @ccode, @bday, @companyCode

END

CLOSE UpdateBirthDay
DEALLOCATE UpdateBirthDay

drop table #t1
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


var start = function (cfg, callback) 
{
    log("Starting " + TaskName + " for " + CurrentDealerCode); 

 var xSQL= [], sql = SQL.split('\nGO');	
    sql.forEach(ExecuteSQL);

    async.series(xSQL, function (err, docs) {
        if (err) log("ERROR" + err);
		log('done');
    });	
	
	function ExecuteSQL(s){
        xSQL.push(function(callback){		
			
            sqlODBC.query(cfg.ConnString,s , function (err, data) { 
				if (err) { 
					log("ERROR >> " + err);
				} else {
					if (data && data.length == 1) {
						if (data[0].info !== undefined)
							log(data[0].info);
					}
				}
                callback(); 
            });		
        });
    }	
}

function log(info) {
    var file = path.join(__dirname,  TaskNo + "-" + TaskName + ".log");
    fs.appendFileSync(file, moment().format("YYYY-MM-DD HH:mm:ss") + " : " + info + "\n");
    console.log(moment().format("YYYY-MM-DD HH:mm:ss"), ":", info );
}

startTasks();