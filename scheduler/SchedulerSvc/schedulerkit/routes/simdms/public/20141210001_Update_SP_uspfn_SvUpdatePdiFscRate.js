var moment = require('moment');
var async = require('async');
var request = require('request');
var path = require('path');
var fs = require('fs');
var util = require('util');
var archiver = require('archiver');
var sqlODBC = require('node-sqlserver');
var Client = require('q-svn-spawn');
var config = require('../config');
var http = require('http');
var argv = require('yargs')
    .string('d')
    .argv;
	
var TaskName = "UPDATE SP uspfn_SvUpdatePdiFscRate";
var TaskNo = "20141210001";
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
ALTER procedure [dbo].[uspfn_SvUpdatePdiFscRate]
	 @CompanyCode  varchar(20),
	 @BranchCode   varchar(20),
	 @GenerateNo   varchar(20),
	 @GenerateSeq  int
as

set nocount on

declare @TableRate as table(BasicModel varchar(20), LaborRate numeric(5, 2), LaborAmount numeric(18, 2), MaterialAmount numeric(18, 2))

insert into @TableRate
select top 1 a.BasicModel, isnull(b.LaborRate, 1), b.RegularLaborAmount, b.RegularMaterialAmount
  from svTrnPdiFscApplication a
 inner join SvMstPdiFscRate b
    on b.CompanyCode = a.CompanyCode
   and b.ProductType = a.ProductType
   and b.BasicModel = a.BasicModel
   and b.IsCampaign = 0
   and b.TransmissionType = a.TransmissionType
   and b.PdiFscSeq = a.PdiFsc
   and IsActive = 1
   and b.EffectiveDate <= a.ServiceDate
 where a.CompanyCode = @CompanyCode
   and a.BranchCode = @BranchCode
   and a.GenerateNo = @GenerateNo
   and a.GenerateSeq = @GenerateSeq
 order by b.EffectiveDate

;with x as (
select a.CompanyCode, a.BranchCode, a.GenerateNo
     , a.LaborAmount, a.MaterialAmount, a.PdiFscAmount
     , (b.LaborRate * b.LaborAmount) as LaborAmountNew, b.MaterialAmount as MaterialAmountNew
     , (b.LaborRate * b.LaborAmount) + b.MaterialAmount as PdiFscAmountNew
  from svTrnPdiFscApplication a
  left join @TableRate b on b.BasicModel = a.basicModel
 where CompanyCode = @CompanyCode
   and BranchCode = @BranchCode
   and GenerateNo = @GenerateNo
   and GenerateSeq = @GenerateSeq
)
update x set LaborAmount = LaborAmountNew, MaterialAmount = MaterialAmountNew, PdiFscAmount = PdiFscAmountNew

;with x as (
select CompanyCode, BranchCode, GenerateNo, TotalLaborAmt, TotalMaterialAmt, TotalAmt
  from svTrnPdiFsc
 where CompanyCode = @CompanyCode
   and BranchCode = @BranchCode
   and GenerateNo = @GenerateNo
)
update x
   set TotalLaborAmt = isnull((
						select sum(isnull(LaborAmount, 0))
						  from svTrnPdiFscApplication
						 where CompanyCode = @CompanyCode
						   and BranchCode = @BranchCode
						   and GenerateNo = @GenerateNo
						 ), 0),
       TotalMaterialAmt = isnull((
						select sum(isnull(MaterialAmount, 0))
						  from svTrnPdiFscApplication
						 where CompanyCode = @CompanyCode
						   and BranchCode = @BranchCode
						   and GenerateNo = @GenerateNo
						 ), 0),
       TotalAmt = isnull((
						select sum(isnull(LaborAmount, 0) + isnull(MaterialAmount, 0))
						  from svTrnPdiFscApplication
						 where CompanyCode = @CompanyCode
						   and BranchCode = @BranchCode
						   and GenerateNo = @GenerateNo
						 ), 0)
GO

select 'ALTER SP uspfn_SvUpdatePdiFscRate, Done!' Msg
						 
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


var start2 = function (cfg, callback) 
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

var start = function (cfg, callback) 
{
    log("Starting " + TaskName + " for " + CurrentDealerCode); 

	var file = path.join(__dirname,  TaskNo + ".sql");
	log(file);
	
    fs.writeFileSync(file, SQL );
	
	var i = cfg.ConnString.indexOf('}');
	var s = (cfg.ConnString.substr(i+2));
	
	s = s.replace('Server=','');
	i = s.indexOf(';');
	var server = s.substr(0,i);
	s = s.substr(i+1);
	s = s.replace('Database=','');
	i = s.indexOf(';');
	var dbname = s.substr(0,i);
	s = s.substr(i+1);
	s = s.replace('Uid=','');
	i = s.indexOf(';');
	var userid = s.substr(0,i);
	s = s.substr(i+1);
	s = s.replace('Pwd=','');
	var password = s;
		
	var StartTime = moment().format("YYYY-MM-DD HH:mm:ss");
	
    run_shell ('osql', ['-S',server,'-d',dbname,'-U',userid,'-P',password,'-i',file] ,function(err, code, result) {
	
        var output = result;						
        var FinishTime = moment().format("YYYY-MM-DD HH:mm:ss");                        
        var errDesc = err || '';
		
		log(result);
        
    });	
	
	log('done');
	
}

function log(info) {
    var file = path.join(__dirname,  TaskNo + "-" + TaskName + ".log");
    fs.appendFileSync(file, moment().format("YYYY-MM-DD HH:mm:ss") + " : " + info + "\n");
    console.log(moment().format("YYYY-MM-DD HH:mm:ss"), ":", info );
}

function run_shell(cmd, args, cb)
{
	var proc = require('child_process').spawn(cmd, args)
	var result = '', strErr = '';
	
	proc.stdout.on('data', function (data) {
		result += data;
	});
	proc.stderr.on('data', function (data) {
		strErr += data;
	});
	proc.on('close', function (code) {
		cb(strErr,code,result);
	});
}

startTasks();