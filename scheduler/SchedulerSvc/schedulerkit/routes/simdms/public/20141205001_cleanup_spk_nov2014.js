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
	
var TaskName = "CLEAN UP SPK NOV-2014";
var TaskNo = "20141205001";
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
BEGIN
	declare @sysDate    datetime
	declare @LmtCleanUp datetime
	declare @CleanUp    integer
	declare @Day        integer
	declare @SequenceNo integer

	set          @sysDate    = '2014/12/01'  --getdate()
	set @Day                 = -1 * (day(@sysDate) - 1)
	set @CleanUp    = -2  -- batasan bulan yang akan di cleanup
	set @LmtCleanUp = dateadd(day,@Day,dateadd(month,@CleanUp,@sysDate))
	set @SequenceNo = 921

	select * into #t1
					from (select distinct CompanyCode, BranchCode, InquiryNumber from pmKDP i
									 --where not (TipeKendaraan='ERTIGA 2' and (Variant like '%SPORTY%' or Variant like '%ELEGANT%'))
									   where exists (select top 1 1 from pmStatusHistory h
																									  where h.CompanyCode=i.CompanyCode
																										and h.BranchCode =i.BranchCode
																										and h.InquiryNumber=i.InquiryNumber
																										and h.LastProgress ='SPK'
																										and convert(varchar,h.UpdateDate,112)<convert(varchar,@LmtCleanUp,112)
																										and not exists (select top 1 1 from pmStatusHistory x
																														 where x.CompanyCode  =h.CompanyCode
																														   and x.BranchCode   =h.BranchCode
																														   and x.InquiryNumber=h.InquiryNumber
																														   and x.LastProgress in ('DO','DELIVERY','LOST'))
																									 )
									 ) #t1

	 select * into pmKDP_SPK_20141205
	   from ( select * from pmKDP where exists (select 1 from #t1
																		 where #t1.InquiryNumber=pmKDP.InquiryNumber
																																													   and #t1.BranchCode=pmKDP.BranchCode
																																													   and #t1.CompanyCode=pmKDP.CompanyCode)) pmKDP_SPK_20141205

	 declare @CompanyCode   varchar(15)
	 declare @BranchCode    varchar(15)
	 declare @InquiryNumber integer
	 declare KDP cursor for
	 select * from #t1 order by CompanyCode, BranchCode, InquiryNumber

     open KDP
     fetch next from KDP into @CompanyCode, @BranchCode, @InquiryNumber

     while @@fetch_status=0
        begin
         begin try
                                   update pmKDP
                                      set LastProgress='LOST', 
                                                      LastUpdateStatus='2014/12/05', --@SysDate, 
                                                                  LostCaseDate='2014/12/05',     --@SysDate, 
                                                                  LostCaseCategory='F', LostCaseReasonID='70', 
                                                      LostCaseOtherReason='AUTOMATIC LOST FOR SPK OVER 2 MONTHS', 
                                                      LastUpdateBy='SYSTEM', 
                                                                  LastUpdateDate='2014/12/05'    --@SysDate
            where InquiryNumber = @InquiryNumber
                                                  and BranchCode    = @BranchCode
                                                  and CompanyCode   = @CompanyCode

                                   insert into pmStatusHistory
                                 (  InquiryNumber,  CompanyCode,  BranchCode,  
                                                                                    SequenceNo, LastProgress, UpdateDate, UpdateUser )
                                       values( @InquiryNumber, @CompanyCode, @BranchCode, 
                                                           @SequenceNo, 'LOST', 
                                                           '2014/12/05', --@SysDate, 
                                                                                   'SYSTEM')
                       fetch next from KDP into @CompanyCode, @BranchCode, @InquiryNumber
         end try
         begin catch
                       fetch next from KDP into @CompanyCode, @BranchCode, @InquiryNumber
         end catch

        end 
        close KDP
        deallocate KDP
        
                drop table #t1
                
                --select 'Before: ' BEF, count(*) Total 
                --  from (select distinct CompanyCode, BranchCode, InquiryNumber from pmKDP_SPK_20141205) a
                --select 'After : ' AFT, count(*) Total from pmKDP 
                -- where LastProgress='LOST' 
                --   and LostCaseOtherReason='AUTOMATIC LOST FOR SPK OVER 2 MONTHS' 
                --   and LastUpdateDate='2014/12/05' --@sysDate

    select * from pmStatusHistory s
     where exists (select top 1 1 from pmKDP 
                    where LastProgress='LOST' 
                      and LostCaseOtherReason='AUTOMATIC LOST FOR SPK OVER 2 MONTHS'
                      and CompanyCode=s.CompanyCode
                      and BranchCode=s.BranchCode
                      and InquiryNumber=s.InquiryNumber)
       and UpdateDate='2014/12/05' --@sysDate
     order by s.InquiryNumber, s.SequenceNo
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


var start1 = function (cfg, callback) 
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