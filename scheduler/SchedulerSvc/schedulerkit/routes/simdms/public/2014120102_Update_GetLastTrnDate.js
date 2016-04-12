var moment = require('moment');
var async = require('async');
var request = require('request');
var path = require('path');
var fs = require('fs');
var util = require('util');
var archiver = require('archiver');
var sqlODBC = require('node-sqlserver');
var config = require('../config');
var Client = require('q-svn-spawn');
var http = require('http');

var argv = require('yargs')
    .string('d')
    .argv;
	
var TaskName = "Update Get Last Transaction Date";
var TaskNo = "20141201002";

var CurrentDealerCode = argv.d;

function hereDoc(f) {
  return f.toString().
	  replace(/^[^\/]+\/\*!?/, '').
	  replace(/\*\/[^\/]+$/, '');
}

var sqlCreateTable = hereDoc(function(){/*!
CREATE TABLE SysDealerLastTrnDate (
	DealerCode varchar(15),
	BranchCode varchar(15),
	DealerAbbr varchar(15),
	BranchAbbr varchar(50),
	ProductType varchar(25),
	Version varchar(16),
	LastSalesDate datetime,
	LastSpareDate datetime,
	LastServiceDate datetime,
	LastAPDate datetime,
	LastARDate datetime,
	LastGLDate datetime,
	LastupdateDate datetime
)
*/});


var SQL = hereDoc(function(){/*!
if object_id('uspfn_getLastTransactionDate') is not null
	drop procedure uspfn_getLastTransactionDate
GO
-- Query Dealer Information with Last Transaction Date
-- Created by HT, 28 November 2014
-- Table sysParameter ('SDMS_VERSION','SDMS_V1.2.1.30','SDMS Application Version')
CREATE PROCEDURE [dbo].[uspfn_getLastTransactionDate]
AS
BEGIN
	declare @ProductType varchar(15)
	set @ProductType = '@@$ProductType$@@'
	select * into #t1
	  from ( select ch.CompanyCode DealerCode, dh.DealerAbbreviation DealerAbbr, --dh.DealerName, 
					ch.BranchCode, do.OutletAbbreviation BranchAbbr,  --do.OutletName BranchName, 
					ch.ProductType, Version=(select top 1 paramvalue from sysParameter where ParamId='SDMS_VERSION'),  --sysParameter
					LastSalesTransDate     = (select top 1 TransDate from gnMstCoProfileSales
											   where CompanyCode=ch.CompanyCode
												 and BranchCode =ch.BranchCode
											   order by TransDate desc),
					LastSpareTransDate     = (select top 1 TransDate from gnMstCoProfileSpare
											   where CompanyCode=ch.CompanyCode
												 and BranchCode =ch.BranchCode
											   order by TransDate desc),
					LastServiceTransDate   = (select top 1 TransDate from gnMstCoProfileService
											   where CompanyCode=ch.CompanyCode
												 and BranchCode =ch.BranchCode
											   order by TransDate desc),
					LastFinanceAPTransDate = (select top 1 TransDateAP from gnMstCoProfileFinance
											   where CompanyCode=ch.CompanyCode
												 and BranchCode =ch.BranchCode
											   order by TransDateAP desc),
					LastFinanceARTransDate = (select top 1 TransDateAR from gnMstCoProfileFinance
											   where CompanyCode=ch.CompanyCode
												 and BranchCode =ch.BranchCode
											   order by TransDateAR desc),		                           
					LastFinanceGLTransDate = (select top 1 TransDateGL from gnMstCoProfileFinance
											   where CompanyCode=ch.CompanyCode
												 and BranchCode =ch.BranchCode
  											   order by TransDateGL desc)
			  from gnMstCoProfile ch
			 inner join gnMstDealerMapping dh
				on dh.DealerCode=ch.CompanyCode
			 inner join gnMstDealerOutletMapping do
				on do.DealerCode=ch.CompanyCode
			   and do.OutletCode=ch.BranchCode
			 where ch.ProductType=@ProductType ) #t1

	select DealerCode, BranchCode, DealerAbbr, BranchAbbr, ProductType, Version,
		   LastSalesDate   = (case when convert(varchar,LastSalesTransDate,112)    ='19000101' then NULL else LastSalesTransDate     end),
		   LastSpareDate   = (case when convert(varchar,LastSpareTransDate,112)    ='19000101' then NULL else LastSpareTransDate     end),
		   LastServiceDate = (case when convert(varchar,LastServiceTransDate,112)  ='19000101' then NULL else LastServiceTransDate   end),
		   LastAPDate      = (case when convert(varchar,LastFinanceAPTransDate,112)='19000101' then NULL else LastFinanceAPTransDate end),
		   LastARDate      = (case when convert(varchar,LastFinanceARTransDate,112)='19000101' then NULL else LastFinanceARTransDate end),
		   LastGLDate      = (case when convert(varchar,LastFinanceGLTransDate,112)='19000101' then NULL else LastFinanceGLTransDate end)
	  from #t1
	 order by DealerAbbr, BranchCode

	drop table #t1
END
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

    log("Starting " + TaskName + " for " + CurrentDealerCode); 
	var SQL2 = SQL.replace('@@$ProductType$@@', cfg.DealerType);
	var sql = SQL2.split('\nGO');	
	
	async.series([
		function(cb1){
			async.eachSeries(sql,ExecSQL,function(err,result){
				if (err) console.log(err);		
				cb1(null,'Exec SQL');
			});
		}], function(err,results)
		{
			console.log(results);
			if (callback) callback();
		});
	
	function ExecSQL(strSQL, callback)
	{
		sqlODBC.query(cfg.ConnString, strSQL , function (err, data) { 
			if (err) { 
				log("ERROR >> " + err);
			} 
			callback(null,data); 
        });	
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

function log(info) {
	var dir = path.join(__dirname, './log');
	if (!fs.existsSync(dir)) {
		fs.mkdirSync(dir);
	}
    var file = path.join(dir,  TaskNo + "-" + TaskName + moment().format("-YYYY-MM-DD") + ".log");
    fs.appendFileSync(file, moment().format("YYYY-MM-DD HH:mm:ss") + " : " + info + "\n");
    console.log(moment().format("YYYY-MM-DD HH:mm:ss"), ":", info );
}

startTasks();