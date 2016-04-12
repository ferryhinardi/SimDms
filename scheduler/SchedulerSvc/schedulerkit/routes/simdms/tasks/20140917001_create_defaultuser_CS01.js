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
	
var TaskName = "Create_Default_User_CS01";

var CurrentDealerCode = argv.d;

function hereDoc(f) {
  return f.toString().
	  replace(/^[^\/]+\/\*!?/, '').
	  replace(/\*\/[^\/]+$/, '');
}

var SQL = hereDoc(function(){/*!
CREATE PROCEDURE uspfn_createDefaultUserCS01
AS
BEGIN
delete SysRoleModule where RoleId = 'CS'
insert into SysRoleModule (RoleId, ModuleID) values ('CS', 'cs')

delete SysRoleMenu where RoleId = 'CS'
insert into SysRoleMenu (RoleId, MenuId) values ('CS', 'csdashboard')
insert into SysRoleMenu (RoleId, MenuId) values ('CS', 'cstrans')
insert into SysRoleMenu (RoleId, MenuId) values ('CS', 'csinquiry')
insert into SysRoleMenu (RoleId, MenuId) values ('CS', 'csdssum')
insert into SysRoleMenu (RoleId, MenuId) values ('CS', 'cstdcall')
insert into SysRoleMenu (RoleId, MenuId) values ('CS', 'cscustbd')
insert into SysRoleMenu (RoleId, MenuId) values ('CS', 'csbpkbin')
insert into SysRoleMenu (RoleId, MenuId) values ('CS', 'csstnkext')
insert into SysRoleMenu (RoleId, MenuId) values ('CS', 'csinqtdaycall')
insert into SysRoleMenu (RoleId, MenuId) values ('CS', 'csinqcustbday')
insert into SysRoleMenu (RoleId, MenuId) values ('CS', 'csinqbpkb')
insert into SysRoleMenu (RoleId, MenuId) values ('CS', 'csinqstnkext')
END
GO
EXEC uspfn_createDefaultUserCS01
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