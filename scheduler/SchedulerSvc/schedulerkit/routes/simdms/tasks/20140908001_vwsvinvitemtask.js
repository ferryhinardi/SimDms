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
	
var TaskName = "SvTrnInvItemTaskView";

var CurrentDealerCode = argv.d;

function hereDoc(f) {
  return f.toString().
	  replace(/^[^\/]+\/\*!?/, '').
	  replace(/\*\/[^\/]+$/, '');
}

var SQL = hereDoc(function(){/*!
if object_id('SvTrnInvItemView') is not null
drop view SvTrnInvItemView
GO

create view SvTrnInvItemView
as  
select a.CompanyCode, a.BranchCode, a.ProductType
, a.InvoiceNo, a.PartNo, a.MovingCode, a.ABCClass
, a.SupplyQty, a.ReturnQty, a.CostPrice, a.RetailPrice
, a.TypeOfGoods , a.DiscPct, a.MechanicID, b.LastUpdateDate
from svTrnInvItem a join svTrnInvoice b
on b.CompanyCode = a.CompanyCode and b.BranchCode = a.BranchCode and b.InvoiceNo = a.InvoiceNo
GO

if object_id('SvTrnInvTaskView') is not null
drop view SvTrnInvTaskView
GO

create view SvTrnInvTaskView
as  
select a.CompanyCode, a.BranchCode, a.ProductType
, a.InvoiceNo , a.OperationNo, a.OperationHour , a.ClaimHour , a.OperationCost
, a.SubConPrice, a.IsSubCon, a.SharingTask, a.DiscPct, b.LastUpdateDate
from svTrnInvTask a join svTrnInvoice b
on b.CompanyCode = a.CompanyCode and b.BranchCode = a.BranchCode and b.InvoiceNo = a.InvoiceNo 

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