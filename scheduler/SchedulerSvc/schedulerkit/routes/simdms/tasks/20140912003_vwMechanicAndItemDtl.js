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
	
var TaskName = "Generate_svTrnInvMechanicViewAndItemDtl";

var CurrentDealerCode = argv.d;

function hereDoc(f) {
  return f.toString().
	  replace(/^[^\/]+\/\*!?/, '').
	  replace(/\*\/[^\/]+$/, '');
}

var SQL = hereDoc(function(){/*!
if object_id('svTrnInvMechanicView') is not null
drop view svTrnInvMechanicView
GO

CREATE view svTrnInvMechanicView
as  
SELECT  a.CompanyCode
      ,a.BranchCode
      ,a.ProductType
      ,a.InvoiceNo
      ,a.OperationNo
      ,a.MechanicID
      ,a.ChiefMechanicID
      ,a.StartService
      ,a.FinishService
      , b.LastUpdateDate
FROM svTrnInvMechanic a
join svTrnInvoice b
    on b.CompanyCode = a.CompanyCode
   and b.BranchCode = a.BranchCode
   and b.InvoiceNo = a.InvoiceNo
GO

if object_id('svTrnInvItemDtlView') is not null
drop view svTrnInvItemDtlView
GO

CREATE view svTrnInvItemDtlView
as  
SELECT a.CompanyCode
      ,a.BranchCode
      ,a.ProductType
      ,a.InvoiceNo
      ,a.PartNo
      ,a.SupplySlipNo
      ,a.SupplyQty
      ,a.CostPrice
      ,a.CreatedBy
      ,a.CreatedDate
      , b.LastUpdateDate
  FROM svTrnInvItemDtl a
join svTrnInvoice b
    on b.CompanyCode = a.CompanyCode
   and b.BranchCode = a.BranchCode
   and b.InvoiceNo = a.InvoiceNo
GO
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