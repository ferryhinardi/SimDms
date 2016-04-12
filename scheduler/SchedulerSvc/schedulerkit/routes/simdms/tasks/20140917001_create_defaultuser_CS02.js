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
	
var TaskName = "Create_Default_User_CS02";

var CurrentDealerCode = argv.d;

function hereDoc(f) {
  return f.toString().
	  replace(/^[^\/]+\/\*!?/, '').
	  replace(/\*\/[^\/]+$/, '');
}

var SQL = hereDoc(function(){/*!
CREATE PROCEDURE uspfn_createDefaultUserCS02
AS
BEGIN
declare @CompanyCode varchar(20)
declare @BranchCode varchar(20)

select @CompanyCode = (select top 1 CompanyCode from gnMstCoProfile)
select @BranchCode = (select top 1 BranchCode from gnMstCoProfile)

delete SysUser where UserId in ('CS-ADM')
insert into SysUser (UserId, Password, FullName, CompanyCode, BranchCode, IsActive, Email) 
values ('CS-ADM', '202CB962AC59075B964B07152D234B70', 'CS ADMIN', @CompanyCode, @BranchCode, 1, '')

delete SysUser where UserId in ('CS')
insert into SysUser (UserId, Password, FullName, CompanyCode, BranchCode, IsActive, Email) 
values ('CS', '202CB962AC59075B964B07152D234B70', 'CS USER', @CompanyCode, @BranchCode, 1, '')

select @CompanyCode = (select top 1 CompanyCode from gnMstCoProfile)
select @BranchCode = (select top 1 BranchCode from gnMstCoProfile)

delete CsSettings where CompanyCode = @CompanyCode
insert into CsSettings (CompanyCode, SettingCode, SettingDesc, SettingParam1, SettingParam2, SettingParam3, SettingParam4, SettingLink1)
values (@CompanyCode, 'REM3DAYSCALL', 'REMINDER 3 DAYS CALL', '2014-01-01', 'DAY', 'CUTOFF', 'M - 2', '3DaysCall')

insert into CsSettings (CompanyCode, SettingCode, SettingDesc, SettingParam1, SettingParam2, SettingParam3, SettingParam4, SettingLink1)
values (@CompanyCode, 'REMBDAYSCALL', 'REMINDER BIRTHDAY CALL', '1', 'MONTH', 'FULL', 'M - 1', 'BDayCall')

insert into CsSettings (CompanyCode, SettingCode, SettingDesc, SettingParam1, SettingParam2, SettingParam3, SettingParam4, SettingLink1)
values (@CompanyCode, 'REMBPKB', 'REMINDER BPKB', '2', 'MONTH', 'ACTUAL', 'CURRENT DATE', 'BpkbRemind')

insert into CsSettings (CompanyCode, SettingCode, SettingDesc, SettingParam1, SettingParam2, SettingParam3, SettingParam4, SettingLink1)
values (@CompanyCode, 'REMSTNKEXT', 'REMINDER STNK EXTENSION', '2', 'MONTH', 'FULL', 'M - 1', 'StnkExt')

insert into CsSettings (CompanyCode, SettingCode, SettingDesc, SettingParam1, SettingParam2, SettingParam3, SettingParam4, SettingLink1)
values (@CompanyCode, 'REMHOLIDAYS', 'REMINDER HOLIDAYS', '1', 'MONTH', 'FULL', 'M - 1', 'Holiays')

END
GO
EXEC uspfn_createDefaultUserCS02
GO
exec uspfn_SyncCsCustomerView
GO
exec uspfn_SyncCsCustomerVehicleView
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