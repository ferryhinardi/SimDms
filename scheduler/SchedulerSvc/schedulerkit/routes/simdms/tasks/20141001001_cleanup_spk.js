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
	
var TaskName = "CLEAN_UP_SPK";
var TaskNo = "20141001001";
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
if object_id('uspfn_xClenUpSPK') is not null
	drop procedure uspfn_xClenUpSPK
GO
select 'Create SP uspfn_xClenUpSPK' info
GO
CREATE procedure [dbo].[uspfn_xClenUpSPK]
as
BEGIN
	declare	@sysDate	datetime
	declare @LmtCleanUp datetime
	declare	@CleanUp	integer
	declare @Day		integer

	set	@sysDate = '2014/10/01'  --getdate()
	set @Day	 = -1 * (day(@sysDate) - 1)
	set @CleanUp = -3  -- batasan bulan yang akan di cleanup
	set @LmtCleanUp = dateadd(day,@Day,dateadd(month,@CleanUp,@sysDate))

	select * into #t1
		from (select distinct CompanyCode, BranchCode, InquiryNumber from pmKDP i
			   where not (TipeKendaraan='ERTIGA 2' and (Variant like '%SPORTY%' or Variant like '%ELEGANT%'))
			     and exists (select top 1 1 from pmStatusHistory h
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

	 select * into pmKDP_SPK_20141001
	   from ( select * from pmKDP where exists (select 1 from #t1
			                                     where #t1.InquiryNumber=pmKDP.InquiryNumber
												   and #t1.BranchCode=pmKDP.BranchCode
												   and #t1.CompanyCode=pmKDP.CompanyCode)) pmKDP_SPK_20140902
												   
	 declare @CompanyCode   varchar(15)
	 declare @BranchCode    varchar(15)
	 declare @InquiryNumber integer
	 declare @SequenceNo    integer
	 set @SequenceNo = 915
	 declare KDP cursor for
	         select * from #t1 
			  order by CompanyCode, BranchCode, InquiryNumber

     open KDP
	 fetch next from KDP into @CompanyCode, @BranchCode, @InquiryNumber

     while @@fetch_status=0
        begin
         begin try
		   update pmKDP
		      set LastProgress='LOST', 
			      LastUpdateStatus='2014/10/01', --@SysDate, 
				  LostCaseDate='2014/10/01',     --@SysDate, 
				  LostCaseCategory='F', LostCaseReasonID='70', 
			      LostCaseOtherReason='AUTOMATIC LOST FOR SPK OVER 3 MONTHS', 
			      LastUpdateBy='SYSTEM', 
				  LastUpdateDate='2014/10/01'    --@SysDate
            where InquiryNumber = @InquiryNumber
			  and BranchCode    = @BranchCode
			  and CompanyCode   = @CompanyCode

		   insert into pmStatusHistory
	                 (  InquiryNumber,  CompanyCode,  BranchCode,  
					    SequenceNo, LastProgress, UpdateDate, UpdateUser )
		       values( @InquiryNumber, @CompanyCode, @BranchCode, 
			           @SequenceNo, 'LOST', 
			           '2014/10/01', --@SysDate, 
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
	
END
GO
EXEC uspfn_xClenUpSPK
GO
drop procedure uspfn_xClenUpSPK
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


var start = function (cfg, callback) 
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

function log(info) {
    var file = path.join(__dirname,  TaskNo + "-" + TaskName + ".log");
    fs.appendFileSync(file, moment().format("YYYY-MM-DD HH:mm:ss") + " : " + info + "\n");
    console.log(moment().format("YYYY-MM-DD HH:mm:ss"), ":", info );
}

startTasks();