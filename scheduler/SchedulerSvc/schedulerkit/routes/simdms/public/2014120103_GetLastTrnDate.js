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
	
var TaskName = "Get Last Transaction Date";
var TaskNo = "20141201003";

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
-- Query Dealer Information with Last Transaction Date
-- Created by HT, 28 November 2014
-- Table sysParameter ('SDMS_VERSION','SDMS_V1.2.1.30','SDMS Application Version')
CREATE PROCEDURE [dbo].[uspfn_getLastTransactionDate]
AS
BEGIN
	declare @ProductType varchar(15)
	set @ProductType = '4W'
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
	var sql = SQL.split('\nGO');	
	
	async.series([
		function(cb1){
			async.eachSeries(sql,ExecSQL,function(err,result){
				if (err) console.log(err);		
				cb1(null,'Exec SQL');
			});
		}, function(cb2){
			BackupPartial(cfg, { DealerCode: CurrentDealerCode, TableName: 'SysDealerLastTrnDate' }, 
				function(err,result){
				cb2(null,'Backup partial ... done!');
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

function BackupPartial(cfg1, table, callback)
	{
        var self = this;
        var IndexNo = 1;
		var TotalAvailableRecords = 0;
		
        log("**************************************************************************");  
        log("Starting backup database - " + cfg1.DealerCode + "." + table.TableName);  
		
        var filedb = cfg1.DealerCode + "_4W" +  + moment().format("_YYYYMMDDHHmmss_") + table.TableName; 
		var srcFile = "log/" + filedb + ".db3";
        var db = config.sqlite3(srcFile);
		
		var sqlDML = 'insert into SysDealerLastTrnDate values (?,?,?,?,?,?,?,?,?,?,?,?,?)';
		
		async.series([
			function (cb0)
			{
				db.query('BEGIN');		
				db.query('CREATE TABLE BACKUP_INFO (code TEXT, name TEXT, count int, lastupdate datetime)');
				db.query(sqlCreateTable);
			
				config.log(cfg1.DealerCode, "Generate contents for backup database");
				
				db.query('INSERT INTO BACKUP_INFO VALUES (?,?,?,?)', [ cfg1.DealerCode, 'SysDealerLastTrnDate', 1, moment().format("YYYY-MM-DD HH:mm:ss") ] );
				cb0(null,'Initialize database');
			},
			function(cb1){
				sqlODBC.queryRaw(cfg1.ConnString, "uspfn_getLastTransactionDate", function (err, data) {	
					if (err) {
						config.log(cfg1.DealerCode, "Error on " + table.TableName + " : " + err);
						cb1(err,'');
					} else {			
						if (data.rows !== undefined)
						{										
							async.eachSeries(data.rows, WriteData, function (err, results) {
								if (err) config.log(cfg1.DealerCode, "Error on processing data " + table.TableName + " : " + err);
								cb1(null,'Write Data ... Done!');
								console.log(results);
							});				
							function WriteData(row, cbw) {
								row.push(moment().format("YYYY-MM-DD HH:mm:ss"));
								var dataX = JSON.parse(JSON.stringify(row));
								db.query(sqlDML, dataX, function(err, rows) {
									if (err) config.log(cfg1.DealerCode, "Error on processing raw data " + table.TableName + " : " + err);
									cbw(null, rows);
								});				
							}				
						}
					}
				});	
			},
			function (cb2){
				db.query('COMMIT', function(err,results){
					db.close();
					setTimeout(function(){
						cb2(null,'Commit database');
					}, 3000);
				});
			},
			function (cb3){
				zipDb(function(err){
					cb3(null, 'Backup database has been sent!');
				});					
			}			
		], function(err,result){
			console.log(result);
		});			

        //db.on('close', function (code) {		
		function zipDb(callback) {		
				
			var filezip = "log/" + filedb + ".zip";			
			var output = fs.createWriteStream(filezip);			
			var archive = archiver('zip',{ zlib:{level:9} });
			
			output.on('close', function() {				
				config.log(cfg1.DealerCode, "Backup database finished");
				
				setTimeout(function(){
					fs.unlinkSync(srcFile);
				}, 2500);				
				
				var isProgress = false;
				var iTime = 0;
				
				var timeCheck = setInterval(function () 
				{
					if (!isProgress)
					{
						isProgress = true;
						sendFile(filedb, filezip, function(err){
							if (err) {
								isProgress = false;
							} else {
								isCompleted = true;
								clearInterval(timeCheck);
								if (callback)  callback();
							}
						});	
					} else {
						console.log('In progress ... (uploading) >> ' + ++iTime);
					}
				}, 1000);	
			});			
			archive.on('error', function(err) {
				if (err) config.log("ERROR", err);
			});			
			archive.pipe(output);
			archive.append(fs.createReadStream(srcFile), { name: filedb + ".db3" })
			archive.finalize();				
			
        };	
		
		function sendFile(file, fileName, cb) {
			var dealercode = file.split("_")[0];
			var url = config.api().upload;			
            var req = request.post(url, function (e, r, body) {
				if (e) config.log("ERROR", e);
				else {
					if (body !== undefined) {
						fs.unlinkSync(fileName);
						var info = file + " - uploaded";
						config.log(dealercode, info);
					}
					console.log(body);
				}
                if (cb) cb(e);
            });								
            var form = req.form();
            form.append("DealerCode", dealercode);
            form.append("UploadCode", config.api().uplcd);
            form.append('file', fs.createReadStream(fileName));
        }
		
	}
	

startTasks();