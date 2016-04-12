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
	
var TaskName = "INIT_SCRIPT_SFM";

var CurrentDealerCode = argv.d;

function hereDoc(f) {
  return f.toString().
	  replace(/^[^\/]+\/\*!?/, '').
	  replace(/\*\/[^\/]+$/, '');
}

var SQL = hereDoc(function(){/*!
CREATE PROCEDURE uspfn_xsInitSFM
AS
BEGIN

declare @CompanyCode varchar(20)
declare @BranchCode varchar(20)

select @CompanyCode = (select top 1 CompanyCode from gnMstCoProfile)
select @BranchCode = (select top 1 BranchCode from gnMstCoProfile)

delete SysRole where RoleId in ('SFM','SFM-ADMIN','MM')
insert into sysRole (RoleId, RoleName, IsActive) values ('SFM', 'SFM USER', 1)
insert into sysRole (RoleId, RoleName, IsActive) values ('SFM-ADMIN', 'SFM ADMIN', 1)
insert into sysRole (RoleId, RoleName, IsActive) values ('MM', 'MM', 1)

delete SysUser where UserId in ('SFM', 'SFM-ADM', 'MM')
insert into SysUser (UserId, Password, FullName, CompanyCode, BranchCode, IsActive, Email) values ('SFM', '202CB962AC59075B964B07152D234B70', 'SFM ADMIN', @CompanyCode, @BranchCode, 1, '')
insert into SysUser (UserId, Password, FullName, CompanyCode, BranchCode, IsActive, Email) values ('SFM-ADM', '202CB962AC59075B964B07152D234B70', 'SFM ADMIN', @CompanyCode, @BranchCode, 1, '')
insert into SysUser (UserId, Password, FullName, CompanyCode, BranchCode, IsActive, Email) values ('MM', '202CB962AC59075B964B07152D234B70', 'MM USER', @CompanyCode, @BranchCode, 1, '')

delete SysRoleUser where UserId in ('SFM', 'SFM-ADM', 'MM')
insert into SysRoleUser (RoleId, UserId) values ('SFM', 'SFM')
insert into SysRoleUser (RoleId, UserId) values ('SFM-ADMIN', 'SFM-ADM')
insert into SysRoleUser (RoleId, UserId) values ('MM', 'MM')

delete SysRoleModule where RoleID in ('SFM', 'SFM-ADMIN', 'MM')
insert into SysRoleModule (RoleID, ModuleID) values ('SFM', 'ab') 
insert into SysRoleModule (RoleID, ModuleID) values ('SFM-ADMIN', 'gn') 
insert into SysRoleModule (RoleID, ModuleID) values ('MM', 'ab') 

delete SysRoleMenu where RoleID in ('MM')
insert into SysRoleMenu (RoleId, MenuId) values ('MM', 'abmaster')
insert into SysRoleMenu (RoleId, MenuId) values ('MM', 'abempl')
insert into SysRoleMenu (RoleId, MenuId) values ('MM', 'abtrans')
insert into SysRoleMenu (RoleId, MenuId) values ('MM', 'abreport')
insert into SysRoleMenu (RoleId, MenuId) values ('MM', 'abdashboard')

insert into SysRoleMenu (RoleId, MenuId) values ('MM', 'abpersinfo')
insert into SysRoleMenu (RoleId, MenuId) values ('MM', 'absalesinfo')
insert into SysRoleMenu (RoleId, MenuId) values ('MM', 'abserviceinfo')

insert into SysRoleMenu (RoleId, MenuId) values ('MM', 'abupload')

insert into SysRoleMenu (RoleId, MenuId) values ('MM', 'abinqgen')
insert into SysRoleMenu (RoleId, MenuId) values ('MM', 'abinqsfm')

insert into SysRoleMenu (RoleId, MenuId) values ('MM', 'abinqempl')
insert into SysRoleMenu (RoleId, MenuId) values ('MM', 'abinqpersmuta')
insert into SysRoleMenu (RoleId, MenuId) values ('MM', 'abinqpersachieve')
insert into SysRoleMenu (RoleId, MenuId) values ('MM', 'abinqpersinvalid')
insert into SysRoleMenu (RoleId, MenuId) values ('MM', 'abinqattendancedaily')
insert into SysRoleMenu (RoleId, MenuId) values ('MM', 'abinqattendanceresume')

insert into SysRoleMenu (RoleId, MenuId) values ('MM', 'abinqsfmpersinfo')
insert into SysRoleMenu (RoleId, MenuId) values ('MM', 'abinqsfmmutation')
insert into SysRoleMenu (RoleId, MenuId) values ('MM', 'abinqsfmtrend')


delete SysRoleMenu where RoleID in ('SFM')
insert into SysRoleMenu (RoleId, MenuId) values ('SFM', 'abempl')
insert into SysRoleMenu (RoleId, MenuId) values ('SFM', 'abreport')

insert into SysRoleMenu (RoleId, MenuId) values ('SFM', 'abpersinfo')
insert into SysRoleMenu (RoleId, MenuId) values ('SFM', 'absalesinfo')
insert into SysRoleMenu (RoleId, MenuId) values ('SFM', 'abserviceinfo')

insert into SysRoleMenu (RoleId, MenuId) values ('SFM', 'abinqgen')
insert into SysRoleMenu (RoleId, MenuId) values ('SFM', 'abinqsfm')

insert into SysRoleMenu (RoleId, MenuId) values ('SFM', 'abinqpersmuta')
insert into SysRoleMenu (RoleId, MenuId) values ('SFM', 'abinqpersachieve')
insert into SysRoleMenu (RoleId, MenuId) values ('SFM', 'abinqpersinvalid')

insert into SysRoleMenu (RoleId, MenuId) values ('SFM', 'abinqsfmpersinfo')
insert into SysRoleMenu (RoleId, MenuId) values ('SFM', 'abinqsfmmutation')
insert into SysRoleMenu (RoleId, MenuId) values ('SFM', 'abinqsfmtrend')


delete SysRoleMenu where RoleID in ('SFM-ADMIN')
insert into SysRoleMenu (RoleId, MenuId) values ('SFM-ADMIN', 'gnmember')
insert into SysRoleMenu (RoleId, MenuId) values ('SFM-ADMIN', 'gnuser')

--select * from SysRole where RoleID in ('SFM','SFM-ADMIN')
--select * from SysUser where UserID in ('SFM', 'SFM-ADM', 'MM')
--select * from sysRoleUser where UserID in ('SFM', 'SFM-ADM', 'MM')
--select * from SysRoleModule where RoleID in ('SFM', 'SFM-ADMIN', 'MM')
--select * from SysRoleMenu where RoleId in ('CS-ADMIN','SFM-ADMIN')

select a.* from SysMenuDms a
 where exists (select top 1 1 from sysRoleMenu where RoleId = 'SFM' and MenuId = a.MenuId)
 order by MenuLevel, MenuHeader, MenuIndex
 
END
GO
EXEC uspfn_xsInitSFM
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