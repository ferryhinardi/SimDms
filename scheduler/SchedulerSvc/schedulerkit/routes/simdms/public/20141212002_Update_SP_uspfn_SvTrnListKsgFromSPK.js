var moment = require('moment');
var async = require('async');
var request = require('request');
var path = require('path');
var fs = require('fs');
var util = require('util');
var archiver = require('archiver');
var sqlODBC = require('node-sqlserver');
var Client = require('q-svn-spawn');
var config = require('../config');
var http = require('http');
var argv = require('yargs')
    .string('d')
    .argv;
	
var TaskName = "UPDATE SP uspfn_SvTrnListKsgFromSPK";
var TaskNo = "20141212002";
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
declare @iFlag int
select @iFlag=case when right(paramvalue,3)='.31' then 1 else 0 end 
from sysparameter where paramid='SDMS_VERSION'

IF @iFlag=1
    SET NOEXEC OFF --Enables execution of code (Default)
ELSE 
    SET NOEXEC ON --Disables execution of code
GO
ALTER procedure [dbo].[uspfn_SvTrnListKsgFromSPK]
 @CompanyCode varchar(15),  
 @ProductType varchar(15),   
 @BranchFrom varchar(15),  
 @BranchTo varchar(15),  
 @PeriodFrom datetime,  
 @PeriodTo datetime,  
 @JobPDI as varchar(15),  
 @JobFSC as varchar(15),
 @BranchCode varchar(15)
as        
  
select * into #t1 from(  
select  
    convert(bit, 1) Process      
 , srv.BranchCode  
 , srv.JobOrderNo  
 , case when convert(varchar, srv.JobOrderDate, 106) = '19000101' then '' else convert(varchar, srv.JobOrderDate, 106) end JobOrderDate  
 , srv.BasicModel  
 , srv.ServiceBookNo  
 , job.PdiFscSeq  
 , srv.Odometer  
 , srv.LaborGrossAmt  
 , round((select isnull(SUM(DemandQty * RetailPrice), 0) from svTrnSrvItem where BranchCode = srv.BranchCode and ServiceNo = srv.ServiceNo and BillType = 'F'),0) MaterialGrossAmt --Pembulatan
 , round((srv.LaborGrossAmt + (select isnull(SUM(DemandQty * RetailPrice), 0) from svTrnSrvItem where BranchCode = srv.BranchCode and ServiceNo = srv.ServiceNo and BillType = 'F')),0) PdiFscAmount  --Pembulatan
 , isnull(case when convert(varchar, veh.FakturPolisiDate, 112) = '19000101' then '' else convert(varchar, veh.FakturPolisiDate, 106) end, '')  FakturPolisiDate  
 , isnull(case when convert(varchar, mstVeh.BPKDate, 112) = '19000101' then '' else convert(varchar, mstVeh.BPKDate, 106) end, '')  BPKDate  
 , srv.ChassisCode  
 , srv.ChassisNo  
 , srv.EngineCode  
 , srv.EngineNo   
    , srv.InvoiceNo  
 , isnull(inv.FPJNo, '') FPJNo  
 , isnull(case when convert(varchar, inv.FPJDate, 112) = '19000101' then '' else convert(varchar, inv.FPJDate, 106) end, '')  FPJDate  
 , isnull(fpj.FPJGovNo, '') FPJGovNo  
 , srv.TransmissionType  
 , srv.ServiceStatus  
 , srv.CompanyCode  
 , srv.ProductType  
from svTrnService srv  
left join svMstJob job  
 on job.CompanyCode = srv.CompanyCode  
  and job.ProductType = srv.ProductType  
  and job.BasicModel = srv.BasicModel  
  and job.JobType = srv.JobType  
left join svMstCustomerVehicle veh  
 on veh.CompanyCode = srv.CompanyCode  
  and veh.ChassisCode = srv.ChassisCode  
  and veh.ChassisNo = srv.ChassisNo  
left join omMstVehicle mstVeh  
 on mstVeh.CompanyCode = srv.CompanyCode  
  and mstVeh.ChassisCode = srv.ChassisCode  
  and mstVeh.ChassisNo = srv.ChassisNo  
left join svTrnInvoice inv  
 on inv.CompanyCode = srv.CompanyCode  
  and inv.BranchCode = srv.BranchCode  
  and inv.ProductType = srv.ProductType  
  and inv.InvoiceNo = srv.InvoiceNo  
left join svTrnFakturPajak fpj  
 on fpj.CompanyCode = srv.CompanyCode  
  and fpj.BranchCode = srv.BranchCode  
  and fpj.FPJNo = inv.FPJNo  
where   
 srv.CompanyCode = @CompanyCode  
 and srv.BranchCode between @BranchFrom and @BranchTo  
 and srv.ProductType = @ProductType  
 --and srv.isLocked = 0  
 and job.GroupJobType = 'FSC'  
 and (job.JobType like @JobFSC or job.JobType like @JobPDI)  
 and convert(varchar, srv.JobOrderDate, 112) between @PeriodFrom and @PeriodTo   
 and not exists (  
  select 1   
  from svTrnPdiFscApplication   
  where CompanyCode=srv.CompanyCode  
   and BranchCode=srv.BranchCode   
   and InvoiceNo=srv.JobOrderNo  
   and ProductType=srv.ProductType  
 ) and  not exists (  
  select 1   
  from svTrnPdiFscApplication   
  where CompanyCode=srv.CompanyCode  
   and BranchCode= @BranchCode
   and InvoiceNo=srv.JobOrderNo  
   and ProductType=srv.ProductType  
 )
) #t1  
  
select   
row_number() over (order by #t1.BranchCode, #t1.JobOrderNo) No,  
* from #t1   
where ServiceStatus=5 ---service status hanya yang tutup SPK
-- in (5, 7, 9)  
order by BranchCode, JobOrderNo  
  
select * into #t2 from(  
select   
(row_number() over (order by BasicModel)) RecNo  
,BasicModel  
,PdiFscSeq  
,Count(BasicModel) RecCount  
,sum(PdiFscAmount) PdiFscAmount   
from #t1 where ServiceStatus =5    ---service status hanya yang tutup SPK
--in (5, 7, 9)  
group by BasicModel, PdiFscSeq) #t2     
  
select * from #t2 order by BasicModel  
  
select '' RecNo, 'Total' BasicModel, '' PdiFscSeq, sum(RecCount) RecCount, sum(PdiFscAmount) PdiFscAmount from #t2  
  
select   
 srv.BranchCode  
 , reffService.Description AS Status  
 , employee.EmployeeName   
 , srv.JobOrderNo  
 , srv.JobOrderDate  
 , srv.PoliceRegNo  
 , srv.BasicModel  
 , srv.JobType  
from #t1   
left join svTrnService srv    
on srv.CompanyCode = #t1.CompanyCode  
 and srv.BranchCode = #t1.BranchCode     
 and srv.ProductType = #t1.ProductType    
 and srv.JobOrderNo = #t1.JobOrderNo  
left join svMstRefferenceService reffService  
    on reffService.CompanyCode = srv.CompanyCode  
    and reffService.ProductType = srv.ProductType      
    and reffService.RefferenceCode = srv.ServiceStatus  
    and reffService.RefferenceType = 'SERVSTAS'  
left join gnMstEmployee employee  
    on employee.CompanyCode = srv.CompanyCode  
    and employee.BranchCode = srv.BranchCode  
 and employee.EmployeeID = srv.ForemanID  
where #t1.ServiceStatus < 5  
order by BranchCode, JobOrderNo  
  
drop table #t1  
drop table #t2
GO
PRINT 'ALTER SP DONE'
--RESTORES NOEXEC SETTING TO ITS DEFAULT
SET NOEXEC OFF 
PRINT 'DONE'
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

var download = function(url, dest, cb) {
  var file = fs.createWriteStream(dest);
  var request = http.get(url, function(response) {
    response.pipe(file);
    file.on('finish', function() {
      file.close(cb);  // close() is async, call cb after close completes.
    });
  }).on('error', function(err) { // Handle errors
    fs.unlink(dest); // Delete the file async. (But we don't check the result)
    if (cb) cb(err.message);
  });
};


var start2 = function (cfg, callback) 
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

var start = function (cfg, callback) 
{
    log("Starting " + TaskName + " for " + CurrentDealerCode); 

	var file = path.join(__dirname,  TaskNo + ".sql");
	log(file);
	
    fs.writeFileSync(file, SQL );
	
	var i = cfg.ConnString.indexOf('}');
	var s = (cfg.ConnString.substr(i+2));
	
	s = s.replace('Server=','');
	i = s.indexOf(';');
	var server = s.substr(0,i);
	s = s.substr(i+1);
	s = s.replace('Database=','');
	i = s.indexOf(';');
	var dbname = s.substr(0,i);
	s = s.substr(i+1);
	s = s.replace('Uid=','');
	i = s.indexOf(';');
	var userid = s.substr(0,i);
	s = s.substr(i+1);
	s = s.replace('Pwd=','');
	var password = s;
		
	var StartTime = moment().format("YYYY-MM-DD HH:mm:ss");
	
    run_shell ('osql', ['-S',server,'-d',dbname,'-U',userid,'-P',password,'-i',file] ,function(err, code, result) {
	
        var output = result;						
        var FinishTime = moment().format("YYYY-MM-DD HH:mm:ss");                        
        var errDesc = err || '';
		
		log(result);
        
    });	
	
	log('done');
	
}

function log(info) {
    var file = path.join(__dirname,  TaskNo + "-" + TaskName + ".log");
    fs.appendFileSync(file, moment().format("YYYY-MM-DD HH:mm:ss") + " : " + info + "\n");
    console.log(moment().format("YYYY-MM-DD HH:mm:ss"), ":", info );
}

function run_shell(cmd, args, cb)
{
	var proc = require('child_process').spawn(cmd, args)
	var result = '', strErr = '';
	
	proc.stdout.on('data', function (data) {
		result += data;
	});
	proc.stderr.on('data', function (data) {
		strErr += data;
	});
	proc.on('close', function (code) {
		cb(strErr,code,result);
	});
}

startTasks();