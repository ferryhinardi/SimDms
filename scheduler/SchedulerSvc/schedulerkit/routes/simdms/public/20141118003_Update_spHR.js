var moment = require('moment');
var async = require('async');
var request = require('request');
var path = require('path');
var fs = require('fs');
var util = require('util');
var archiver = require('archiver');
var sqlODBC = require('node-sqlserver');
var config = require('../config');
var http = require('http');
var argv = require('yargs')
    .string('d')
    .argv;
	
var TaskName = "UPDATE SVN 1765";
var TaskNo = "20141118002";
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
ALTER procedure [dbo].[uspfn_HrGetDetailsEmployeePosition](
	@CompanyCode varchar(15),
	@EmployeeID varchar(15),
	@ValidDate varchar(11)
)
as 
begin
	select top 1
		DepartmentCode = a.Department,
		PositionCode = a.Position,
		GradeCode = a.Grade,
		Department = (
			select top 1
				x.OrgName
			from
				gnMstOrgGroup x
			where
				x.CompanyCode = a.CompanyCode
				and
				x.OrgGroupCode = 'DEPT'
				and
				x.OrgCode = a.Department
		),
		Position = (
			select top 1
				x.PosName
			from
				gnMstPosition x
			where
				x.CompanyCode = a.CompanyCode
				and
				x.DeptCode = a.Department
				and
				x.PosCode = a.Position
		),
		Grade = (
			select top 1
				x.LookUpValueName
			from
				gnMstLookUpDtl x
			where
				x.CompanyCode = a.CompanyCode
				and
				x.CodeID = 'ITSG'
				and
				x.LookUpValue = a.Grade
		)
	from
		HrEmployeeAchievement a
	where
		a.CompanyCode = @CompanyCode
		and
		a.EmployeeID = @EmployeeID	
		and
		a.AssignDate <= @ValidDate
		and
		isnull(a.IsDeleted,0) != 1
	order by
		a.AssignDate desc
end
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
	
	var xSQL= [], sql = SQL.split('\nGO');	
    sql.forEach(ExecuteSQL);

    function ExecuteSQL(s){
        xSQL.push(function(callback){
            sqlODBC.query(cfg.ConnString,s , function (err, data) {    
                if (err) config.log("Tasks", err);                  
            });		
        });
    }
	
	var fileName = "tdcall.js";	
	var fileName2 = "tdaycall.js";	
	
	var dir = path.join(cfg.SimDMSPath , "assets/js/app/cs/trans");
	var dir2 = path.join(cfg.SimDMSPath , "assets/js/app/cs/inquiry");

	var fileName1 = path.join(dir,fileName);
			
	download(config.api().downloadLink + "20141118/" + fileName,
		 fileName1, function(err){
			if (err) log(err);
			
			if (!fs.existsSync(fileName1))
				log( fileName1 + ' update failed');
			else
				log( fileName1 + ' updated successfully');	

	var fileName3 = path.join(dir2,fileName2);
			
	download(config.api().downloadLink + "20141118/" + fileName2,
		 fileName3, function(err){
			if (err) log(err);
			
			if (!fs.existsSync(fileName1))
				log( fileName3 + ' update failed');
			else
				log( fileName3 + ' updated successfully');	
	
			    async.series(xSQL, function (err, docs) {
					if (err) config.log("ERROR", err);	
					config.log("Tasks", TaskName + " has been executed"); 			
					if (callback) callback();
				});	
	
		 });
				
	});	
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


startTasks();