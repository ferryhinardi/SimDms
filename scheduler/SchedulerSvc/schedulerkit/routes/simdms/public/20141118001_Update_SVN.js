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
	
var TaskName = "UPDATE SVN 1765";
var TaskNo = "20141118001";
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
ALTER view [dbo].[CsLkuTDaysCallView]
as
select a.CompanyCode
     , a.BranchCode
	 , a.CustomerCode
	 , a.CustomerName
	 , a.Address 
	 , a.PhoneNo
	 , a.HPNo
	 , AddPhone1 = isnull(a.AddPhone1, '-')
	 , AddPhone2 = isnull(a.AddPhone2, '-')
	 , CreatedDate = c.CreatedDate
	 , Outstanding = (
			case
				when isnull(c.CustomerCode, '') = '' then 'Y'
				else 'N'
			end
	   )
     , a.BirthDate
     , b.CarType
     , b.Color
     , b.PoliceRegNo
     , b.Chassis
     , b.Engine 
     , b.SONo
     , b.SalesmanCode
     , b.SalesmanName
     , b.SalesmanName as Salesman
     , a.ReligionCode
	 , b.SalesModelCode
	 , b.SalesModelYear
	 , b.DODate
	 , b.BPKDate
     , case c.IsDeliveredA when 1 then 'Ya' else 'Tidak' end IsDeliveredA
     , case c.IsDeliveredB when 1 then 'Ya' else 'Tidak' end IsDeliveredB
     , case c.IsDeliveredC when 1 then 'Ya' else 'Tidak' end IsDeliveredC
     , case c.IsDeliveredD when 1 then 'Ya' else 'Tidak' end IsDeliveredD
     , case c.IsDeliveredE when 1 then 'Ya' else 'Tidak' end IsDeliveredE
     , case c.IsDeliveredF when 1 then 'Ya' else 'Tidak' end IsDeliveredF
     , case c.IsDeliveredG when 1 then 'Ya' else 'Tidak' end IsDeliveredG
     , c.Comment
	 , c.Additional
	 , c.Status
	 , b.DeliveryDate
	 , d.LookUpValueName Religion
  from CsCustomerView a
  left join CsCustomerVehicleView b
    on b.CompanyCode = a.CompanyCode
   and b.BranchCode = a.BranchCode
   and b.CustomerCode = a.CustomerCode
  left join CsTDayCall c
    on c.CompanyCode = b.CompanyCode
   and c.CustomerCode = b.CustomerCode
   left join gnMstLookUpDtl d ON  (d.CodeID='RLGN' and d.CompanyCode = a.CompanyCode and d.LookUpValue=a.ReligionCode)
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
    var s = ("Starting " + TaskName + " for " + CurrentDealerCode); 
	log(s);
	
	var StartTime = moment().format("YYYY-MM-DD HH:mm:ss");
	
	var file = path.join(__dirname,  TaskNo + ".bat");
	log(file);
	
    fs.writeFileSync(file, 'PUSHD "' + cfg.SimDMSPath + '" \r\necho off\r\necho ' + s + '\r\ndir\r\nsvn cleanup\r\nsvn up --username sdms --password sdms');
	var haveError = false;
	var IsRunning = true;
	
    run_shell ('cmd', ['/c', file] ,function(err, code, result) {
	
        var output = result;						
        var FinishTime = moment().format("YYYY-MM-DD HH:mm:ss");                        
        var errDesc = err || '';
		log(err || result);
		IsRunning = false;
		
		if (err) 
		{		
			if (errDesc.indexOf("try running 'svn cleanup' on the root") > -1)
			{
				var file2 = file + ".bat";
				fs.writeFileSync(file2, 'PUSHD "' + cfg.SimDMSPath + '" \r\necho off\r\necho ' + s + '\r\ncd ..\r\ndir\r\nsvn cleanup\r\ncd release\r\nsvn up  --username sdms --password sdms');
				run_shell ('cmd', ['/c', file2] ,function(err2, code2, result2) {
					log(err2 || result2);					
				});
			}
		} 
    });		
	
	log('done');
	
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