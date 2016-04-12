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
	
var TaskName = "Create_Default_User_CS";

var CurrentDealerCode = argv.d;

function hereDoc(f) {
  return f.toString().
	  replace(/^[^\/]+\/\*!?/, '').
	  replace(/\*\/[^\/]+$/, '');
}

var SQL = hereDoc(function(){/*!
CREATE PROCEDURE uspfn_createDefaultUserCS
AS
BEGIN
declare @CompanyCode varchar(20)
declare @BranchCode varchar(20)

select @CompanyCode = (select top 1 CompanyCode from gnMstCoProfile)
select @BranchCode = (select top 1 BranchCode from gnMstCoProfile)

delete SysRole where RoleId in ('CS','CS-ADMIN')
insert into sysRole (RoleId, RoleName, IsActive) values ('CS', 'CS USER', 1)
insert into sysRole (RoleId, RoleName, IsActive) values ('CS-ADMIN', 'CS ADMIN', 1)

delete SysUser where UserId in ('CS-ADM')
insert into SysUser (UserId, Password, FullName, CompanyCode, BranchCode, IsActive, Email) 
values ('CS-ADM', '202CB962AC59075B964B07152D234B70', 'CS ADMIN', @CompanyCode, @BranchCode, 1, '')

delete SysRoleModule where RoleId = 'CS-ADMIN'
insert into SysRoleModule (RoleId, ModuleID) values ('CS-ADMIN', 'gn')

delete SysRoleMenu where RoleId = 'CS-ADMIN'
insert into SysRoleMenu (RoleId, MenuId) values ('CS-ADMIN', 'gnmember')
insert into SysRoleMenu (RoleId, MenuId) values ('CS-ADMIN', 'gnuser')

END
GO
EXEC uspfn_createDefaultUserCS
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