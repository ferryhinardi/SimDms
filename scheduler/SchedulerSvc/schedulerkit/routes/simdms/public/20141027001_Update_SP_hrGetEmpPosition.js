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
	
var TaskName = "UPDATE uspfn_HrGetDetailsEmployeePosition";
var TaskNo = "20141027001";
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
ALTER procedure [dbo].[uspfn_HrGetDetailsEmployeePosition]
	@CompanyCode varchar(15),
	@EmployeeID varchar(15),
	@ValidDate varchar(11)
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
		ISNULL(a.IsDeleted,0) != 1
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