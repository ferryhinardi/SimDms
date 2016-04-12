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
	
var TaskName = "Alter View SysUserView";

var CurrentDealerCode = argv.d;

function hereDoc(f) {
  return f.toString().
	  replace(/^[^\/]+\/\*!?/, '').
	  replace(/\*\/[^\/]+$/, '');
}

var SQL = hereDoc(function(){/*!
alter view SysUserView
as
select a.UserId
     , a.FullName
     , a.Email
     , a.CompanyCode
     , a.BranchCode
     , d.CompanyName as BranchName
     , a.TypeOfGoods
     , a.IsActive
     , b.RoleId
     , c.RoleName
     , convert(bit,0) AS RequiredChange
     , ProfitCenter = ISNULL((select ProfitCenter from SysUserProfitCenter where UserId = a.UserId), '')
  from SysUser a
  left join SysRoleuser b 
    on b.UserId = a.UserId
  left join sysRole c 
    on c.RoleId = b.RoleId
  left join gnMstCoProfile d
    on d.CompanyCode = a.CompanyCode
   and d.BranchCode = a.BranchCode
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