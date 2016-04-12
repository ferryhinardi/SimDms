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
	
var TaskName = "UPDATE SP ApprovalPdiFsc";
var TaskNo = "20141208001";
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

ALTER PROCEDURE [dbo].[uspfn_GetSPKForUnApprovalPdiFsc]
	@CompanyCode varchar(15),
	@BranchCode varchar(15),
	@ProductType varchar(15),
	@IsPdi bit
AS
BEGIN
	--declare @CompanyCode as varchar(15)
	--declare @BranchCode as varchar(15)
	--declare @ProductType as varchar(15)
	--declare @IsPDI as bit

	--set @CompanyCode = '6058401'
	--set @BranchCode = '605840103'
	--set @ProductType = '4W'
	--set @IsPDI = 0
	
	select convert(bit, 0) IsSelected, row_number() over(order by a.ServiceNo asc) as No,a.BranchCode, a.ServiceNo, a.JobOrderNo, a.JobOrderDate
	, a.ServiceBookNo, a.ChassisNo, a.BasicModel, a.JobType, (isnull(srv.ValItem, 0) + isnull(task.valTask,0)) TotalApprove
	from SvTrnService a
	left join (select CompanyCode, BranchCode, ServiceNo, sum((OperationHour * OperationCost)) valTask
		from svTrnSrvTask
		where BillType = 'F'
		group by CompanyCode, BranchCode, ServiceNo) task on task.CompanyCode = a.CompanyCode
											and task.BranchCode = a.BranchCode
											and task.ServiceNo = a.ServiceNo
	left join (select CompanyCode, BranchCode, ServiceNo, sum(((SupplyQty - ReturnQty) * RetailPrice)) valItem 
		from svTrnSrvItem 
		where BillType = 'F'
		group by CompanyCode, BranchCode, ServiceNo) srv on srv.CompanyCode = a.CompanyCode
											and srv.BranchCode = a.BranchCode
											and srv.ServiceNo = a.ServiceNo
	where a.CompanyCode = @CompanyCode
	and a.BranchCode = @BranchCode
	and a.ProductType = @ProductType
	and a.ServiceStatus = '5 '
	and a.IsLocked = 1
	and a.JobType like (case when @IsPDI = 1 then 'PDI%' else 'FS%' end )
END
GO

ALTER PROCEDURE [dbo].[uspfn_GetSPKForApprovalPdiFsc]
	@CompanyCode varchar(15),
	@BranchCode varchar(15),
	@ProductType varchar(15),
	@IsPdi bit
AS
BEGIN
--	declare @CompanyCode as varchar(15)
--	declare @BranchCode as varchar(15)
--	declare @ProductType as varchar(15)
--	declare @IsPDI as bit
--
--	set @CompanyCode = '6014401'
--	set @BranchCode = '601440100'
--	set @ProductType = '4W'
--	set @IsPDI = 0
	
	select convert(bit, 0) IsSelected, row_number() over(order by a.ServiceNo asc) as No,a.BranchCode, a.ServiceNo, a.JobOrderNo, a.JobOrderDate
	, a.ServiceBookNo, a.ChassisNo, a.BasicModel, a.JobType, (isnull(srv.ValItem, 0) + isnull(task.valTask,0)) TotalApprove
	from SvTrnService a
	left join (select CompanyCode, BranchCode, ServiceNo, sum((OperationHour * OperationCost)) valTask
		from svTrnSrvTask
		where BillType = 'F'
		group by CompanyCode, BranchCode, ServiceNo) task on task.CompanyCode = a.CompanyCode
											and task.BranchCode = a.BranchCode
											and task.ServiceNo = a.ServiceNo
	left join (select CompanyCode, BranchCode, ServiceNo, sum(((SupplyQty - ReturnQty) * RetailPrice)) valItem 
		from svTrnSrvItem 
		where BillType = 'F'
		group by CompanyCode, BranchCode, ServiceNo) srv on srv.CompanyCode = a.CompanyCode
											and srv.BranchCode = a.BranchCode
											and srv.ServiceNo = a.ServiceNo
	where a.CompanyCode = @CompanyCode
	and a.BranchCode = @BranchCode
	and a.ProductType = @ProductType
	and a.ServiceStatus = '5 '
	and a.IsLocked = 0
	and a.JobType like (case when @IsPDI = 1 then 'PDI%' else 'FS%' end )
	
END
GO
PRINT 'ALTER SP DONE'
SET NOEXEC OFF --RESTORES NOEXEC SETTING TO ITS DEFAULT
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


var start1 = function (cfg, callback) 
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