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
	
var TaskName = "UPDATE SP CS DASHBOARD";
var TaskNo = "20141204001";
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
if object_id('uspfn_CsDashSummary') is not null
	drop procedure uspfn_CsDashSummary
GO
CREATE procedure [dbo].[uspfn_CsDashSummary]
	@CompanyCode nvarchar(20),
	@BranchCode varchar(20)
as
declare @CurrDate datetime, @Param1 as varchar(20)
declare @t_rem as table
(
	RemCode varchar(20),
	RemDate datetime,
	RemValue int
)

set @CurrDate = getdate()

-- REM3DAYSCALL
set @Param1 = isnull((select SettingParam1 from CsSettings where CompanyCode = @CompanyCode and SettingCode = 'REM3DAYSCALL'), '0')
insert into @t_rem (RemCode, RemDate) values('REM3DAYSCALL', case when len(@Param1)=10 then @Param1 else left(convert(varchar, dateadd(month, -convert(int, @Param1), @CurrDate), 112), 6) + '01' END )

-- REMBDAYSCALL
set @Param1 = isnull((select SettingParam1 from CsSettings where CompanyCode = @CompanyCode and SettingCode = 'REMBDAYSCALL'), '0')
insert into @t_rem (RemCode, RemDate) values('REMBDAYSCALL', case when len(@Param1)=10 then @Param1 else left(convert(varchar, dateadd(month, -convert(int, @Param1), @CurrDate), 112), 6) + '01' END )

-- REMHOLIDAYS
set @Param1 = isnull((select SettingParam1 from CsSettings where CompanyCode = @CompanyCode and SettingCode = 'REMHOLIDAYS'), '0')
insert into @t_rem (RemCode, RemDate) values('REMHOLIDAYS', case when len(@Param1)=10 then @Param1 else left(convert(varchar, dateadd(month, -convert(int, @Param1), @CurrDate), 112), 6) + '01' END )

-- REMSTNKEXT
set @Param1 = isnull((select SettingParam1 from CsSettings where CompanyCode = @CompanyCode and SettingCode = 'REMSTNKEXT'), '0')
insert into @t_rem (RemCode, RemDate) values('REMSTNKEXT', case when len(@Param1)=10 then @Param1 else left(convert(varchar, dateadd(month, -convert(int, @Param1), @CurrDate), 112), 6) + '01' END )

-- REMBPKB
set @Param1 = isnull((select SettingParam1 from CsSettings where CompanyCode = @CompanyCode and SettingCode = 'REMBPKB'), '0')
insert into @t_rem (RemCode, RemDate) values('REMBPKB', case when len(@Param1)=10 then @Param1 else left(convert(varchar, dateadd(month, -convert(int, @Param1), @CurrDate), 112), 6) + '01' END )
   
declare @CurrentMonth tinyint;
declare @PreviousMonth tinyint;
declare @NextMonth tinyint;
declare @CurrentDay tinyint;
declare @DateComparison datetime;


set @CurrentDay = datepart(day, getdate());
set @CurrentMonth = DATEPART(month, getdate());

if @CurrentMonth = 1 
	set @PreviousMonth=12
else
	set @PreviousMonth=@CurrentMonth-1;
if @CurrentMonth = 12 
	set @NextMonth=1
else
	set @NextMonth=@CurrentMonth+1;

--update @t_rem set RemValue = (select count(CustomerCode) from CsLkuTDayCallView where CompanyCode = @CompanyCode and BranchCode like @BranchCode and OutStanding = 'Y' and DODate >= (select RemDate from @t_rem where RemCode = 'REM3DAYSCALL')) where RemCode = 'REM3DAYSCALL'
--update @t_rem set RemValue = (select count(CustomerCode) from CsLkuStnkExtView where CompanyCode = @CompanyCode and BranchCode like @BranchCode and OutStanding = 'Y' and StnkExpiredDate >= (select RemDate from @t_rem where RemCode = 'REMSTNKEXT')) where RemCode = 'REMSTNKEXT'
--update @t_rem set RemValue = (select count(CustomerCode) from CsLkuBpkbView where CompanyCode = @CompanyCode and BranchCode like @BranchCode and OutStanding = 'Y' and BpkbDate >= (select RemDate from @t_rem where RemCode = 'REMBPKB')) where RemCode = 'REMBPKB'
--update @t_rem set RemValue = (select count(CustomerCode) from CsLkuBirthdayView where CompanyCode = @CompanyCode and BranchCode like @BranchCode and OutStanding = 'Y' ) where RemCode = 'REMBDAYSCALL'

update @t_rem set RemValue = (
						select count(a.CustomerCode)
						  from CsCustomerVehicleView a
						 inner join CsCustomerView b 
						    on b.CompanyCode = a.CompanyCode
						   and b.CustomerCode = a.CustomerCode
						  left join CsStnkExt c
						    on c.CompanyCode = a.CompanyCode
						   and c.CustomerCode = a.CustomerCode
						   and c.Chassis = a.Chassis
						 where a.CompanyCode like @CompanyCode
						   and a.BranchCode like @BranchCode
						   and isnull(c.Chassis, '') = ''
						   and isnull(c.StnkExpiredDate, isnull(c.StnkDate, a.BpkDate)) >= (select RemDate from @t_rem where RemCode = 'REMSTNKEXT')
				)
 where RemCode = 'REMSTNKEXT';

 update @t_rem set RemValue = (
						select count(a.CustomerCode)
						  from CsCustomerVehicleView a
						 inner join CsCustomerView b 
						    on b.CompanyCode = a.CompanyCode
						   and b.CustomerCode = a.CustomerCode
						  left join CsTdayCall c
						    on c.CompanyCode = a.CompanyCode
						   and c.CustomerCode = a.CustomerCode
						   and c.Chassis = a.Chassis
						 where a.CompanyCode like @CompanyCode
						   and a.BranchCode like @BranchCode
						   and isnull(c.Chassis, '') = ''
						   and a.DoDate >=  (select RemDate from @t_rem where RemCode = 'REM3DAYSCALL')
				)
 where RemCode = 'REM3DAYSCALL';

  update @t_rem set RemValue = (
						select count(a.CustomerCode)
						  from CsCustomerVehicleView a
						 inner join CsCustomerView b 
						    on b.CompanyCode = a.CompanyCode
						   and b.CustomerCode = a.CustomerCode
						  left join CsCustBpkb c
						    on c.CompanyCode = a.CompanyCode
						   and c.CustomerCode = a.CustomerCode
						   and c.Chassis = a.Chassis
						 where a.CompanyCode like @CompanyCode
						   and a.BranchCode like @BranchCode
						   and ( 
									isnull(c.Chassis, '') = ''
									or 
									isnull(c.BpkbReadyDate, '') = ''
									or
									isnull(isnull(c.BpkbPickUp, (select top 1 x.RetrievalEstimationDate from CsBpkbRetrievalInformation x where x.CompanyCode=a.CompanyCode and x.CustomerCode=c.CustomerCode and (x.IsDeleted = 0 or x.IsDeleted is null) order by x.RetrievalEstimationDate desc)), '') = ''
									or
									isnull(isnull(c.BpkbPickUp, (select top 1 x.RetrievalEstimationDate from CsBpkbRetrievalInformation x where x.CompanyCode=a.CompanyCode and x.CustomerCode=c.CustomerCode order by x.RetrievalEstimationDate desc)), '') != '' and isnull(c.BpkbPickUp, (select top 1 x.RetrievalEstimationDate from CsBpkbRetrievalInformation x where x.CompanyCode=a.CompanyCode and x.CustomerCode=c.CustomerCode and (x.IsDeleted = 0 or x.IsDeleted is null) order by x.RetrievalEstimationDate desc)) < getdate() 
									or
									isnull(c.BpkbReadyDate, '') != '' and isnull(isnull(c.BpkbPickUp, (select top 1 x.RetrievalEstimationDate from CsBpkbRetrievalInformation x where x.CompanyCode=a.CompanyCode and x.CustomerCode=c.CustomerCode and (x.IsDeleted = 0 or x.IsDeleted is null) order by x.RetrievalEstimationDate desc)), '') = ''
							   )
						   and a.DoDate >=  (select RemDate from @t_rem where RemCode = 'REMBPKB')
				)
  where RemCode = 'REMBPKB';
  
  set @DateComparison = (select RemDate from @t_rem where RemCode = 'REMBDAYSCALL');
  set @CurrentMonth = datepart(month, @DateComparison);
  set @NextMonth = @CurrentMonth + 1;
  set @PreviousMonth = @CurrentMonth - 1;

 update @t_rem set RemValue = (
						select count(a.CustomerCode)
						  from CsCustomerVehicleView a
						 inner join CsCustomerView b 
						    on b.CompanyCode = a.CompanyCode
						   and b.CustomerCode = a.CustomerCode
						  left join CsCustBirthday c
						    on c.CompanyCode = a.CompanyCode
						   and c.CustomerCode = a.CustomerCode
						 where a.CompanyCode like @CompanyCode
						   and a.BranchCode like @BranchCode
						   and isnull(c.CustomerCode, '') = ''
						   and datepart(month, b.BirthDate) >= @PreviousMonth
						   and datepart(month, b.BirthDate) <= @CurrentMonth
				)
 where RemCode = 'REMBDAYSCALL';

select a.RemCode, a.RemDate, a.RemValue, b.SettingLink1 as ControlLink
  from @t_rem a
  join CsSettings b
    on b.CompanyCode = @CompanyCode
   and b.SettingCode = a.RemCode
GO
if object_id('uspfn_CsDashSummaryWithSync') is not null
	drop procedure uspfn_CsDashSummaryWithSync
GO
CREATE procedure [dbo].[uspfn_CsDashSummaryWithSync]
	@CompanyCode nvarchar(20),
	@BranchCode varchar(20)
as

exec uspfn_SyncCsCustomerView

exec uspfn_SyncCsCustomerVehicleView

--set @CompanyCode = '6006406'

declare @CurrDate datetime, @Param1 as varchar(20)
declare @t_rem as table
(
	RemCode varchar(20),
	RemDate datetime,
	RemValue int
)

set @CurrDate = getdate()

-- REM3DAYSCALL
set @Param1 = isnull((select SettingParam1 from CsSettings where CompanyCode = @CompanyCode and SettingCode = 'REM3DAYSCALL'), '0')
insert into @t_rem (RemCode, RemDate) values('REM3DAYSCALL', case when len(@Param1)=10 then @Param1 else left(convert(varchar, dateadd(month, -convert(int, @Param1), @CurrDate), 112), 6) + '01' END )

-- REMBDAYSCALL
set @Param1 = isnull((select SettingParam1 from CsSettings where CompanyCode = @CompanyCode and SettingCode = 'REMBDAYSCALL'), '0')
insert into @t_rem (RemCode, RemDate) values('REMBDAYSCALL', case when len(@Param1)=10 then @Param1 else left(convert(varchar, dateadd(month, -convert(int, @Param1), @CurrDate), 112), 6) + '01' END )

-- REMHOLIDAYS
set @Param1 = isnull((select SettingParam1 from CsSettings where CompanyCode = @CompanyCode and SettingCode = 'REMHOLIDAYS'), '0')
insert into @t_rem (RemCode, RemDate) values('REMHOLIDAYS', case when len(@Param1)=10 then @Param1 else left(convert(varchar, dateadd(month, -convert(int, @Param1), @CurrDate), 112), 6) + '01' END )

-- REMSTNKEXT
set @Param1 = isnull((select SettingParam1 from CsSettings where CompanyCode = @CompanyCode and SettingCode = 'REMSTNKEXT'), '0')
insert into @t_rem (RemCode, RemDate) values('REMSTNKEXT', case when len(@Param1)=10 then @Param1 else left(convert(varchar, dateadd(month, -convert(int, @Param1), @CurrDate), 112), 6) + '01' END )

-- REMBPKB
set @Param1 = isnull((select SettingParam1 from CsSettings where CompanyCode = @CompanyCode and SettingCode = 'REMBPKB'), '0')
insert into @t_rem (RemCode, RemDate) values('REMBPKB', case when len(@Param1)=10 then @Param1 else left(convert(varchar, dateadd(month, -convert(int, @Param1), @CurrDate), 112), 6) + '01' END )
   
declare @CurrentMonth tinyint;
declare @PreviousMonth tinyint;
declare @NextMonth tinyint;
declare @CurrentDay tinyint;
declare @DateComparison datetime;


set @CurrentDay = datepart(day, getdate());
set @CurrentMonth = DATEPART(month, getdate());
if @CurrentMonth = 1 
	set @PreviousMonth=12
else
	set @PreviousMonth=@CurrentMonth-1;
if @CurrentMonth = 12 
	set @NextMonth=1
else
	set @NextMonth=@CurrentMonth+1;

--update @t_rem set RemValue = (select count(CustomerCode) from CsLkuTDayCallView where CompanyCode = @CompanyCode and BranchCode like @BranchCode and OutStanding = 'Y' and DODate >= (select RemDate from @t_rem where RemCode = 'REM3DAYSCALL')) where RemCode = 'REM3DAYSCALL'
--update @t_rem set RemValue = (select count(CustomerCode) from CsLkuStnkExtView where CompanyCode = @CompanyCode and BranchCode like @BranchCode and OutStanding = 'Y' and StnkExpiredDate >= (select RemDate from @t_rem where RemCode = 'REMSTNKEXT')) where RemCode = 'REMSTNKEXT'
--update @t_rem set RemValue = (select count(CustomerCode) from CsLkuBpkbView where CompanyCode = @CompanyCode and BranchCode like @BranchCode and OutStanding = 'Y' and BpkbDate >= (select RemDate from @t_rem where RemCode = 'REMBPKB')) where RemCode = 'REMBPKB'
--update @t_rem set RemValue = (select count(CustomerCode) from CsLkuBirthdayView where CompanyCode = @CompanyCode and BranchCode like @BranchCode and OutStanding = 'Y' ) where RemCode = 'REMBDAYSCALL'

declare @TDaysCallCutOffPeriod varchar(20);
declare @TDaysCallSettingParam3 varchar(20);

set @TDaysCallCutOffPeriod = ( select top 1 a.SettingParam1 from CsSettings a where a.CompanyCode = @CompanyCode);
set @TDaysCallSettingParam3 = ( select top 1 a.SettingParam3 from CsSettings a where a.CompanyCode = @CompanyCode);

if @TDaysCallSettingParam3 != 'CUTOFF'
begin
	update @t_rem set RemValue = (
							select count(a.CustomerCode)
							  from CsCustomerVehicleView a
							 inner join CsCustomerView b 
								on b.CompanyCode = a.CompanyCode
							   and b.CustomerCode = a.CustomerCode
							  left join CsStnkExt c
								on c.CompanyCode = a.CompanyCode
							   and c.CustomerCode = a.CustomerCode
							   and c.Chassis = a.Chassis
							 where a.CompanyCode like @CompanyCode
							   and a.BranchCode like @BranchCode
							   and isnull(c.Chassis, '') = ''
							   and isnull(c.StnkExpiredDate, isnull(c.StnkDate, a.BpkDate)) >= (select RemDate from @t_rem where RemCode = 'REMSTNKEXT')
					)
	 where RemCode = 'REMSTNKEXT';

	 update @t_rem set RemValue = (
							select count(a.CustomerCode)
							  from CsCustomerVehicleView a
							 inner join CsCustomerView b 
								on b.CompanyCode = a.CompanyCode
							   and b.CustomerCode = a.CustomerCode
							  left join CsTdayCall c
								on c.CompanyCode = a.CompanyCode
							   and c.CustomerCode = a.CustomerCode
							   and c.Chassis = a.Chassis
							 where a.CompanyCode like @CompanyCode
							   and a.BranchCode like @BranchCode
							   and isnull(c.Chassis, '') = ''
							   and a.DoDate >=  (select RemDate from @t_rem where RemCode = 'REM3DAYSCALL')
					)
	 where RemCode = 'REM3DAYSCALL';
end
else
begin
	insert into @t_rem (RemCode, RemDate) values('REM3DAYSCALL', @TDaysCallCutOffPeriod);

	update @t_rem set RemValue = (
							select count(a.CustomerCode)
							  from CsCustomerVehicleView a
							 inner join CsCustomerView b 
								on b.CompanyCode = a.CompanyCode
							   and b.CustomerCode = a.CustomerCode
							  left join CsStnkExt c
								on c.CompanyCode = a.CompanyCode
							   and c.CustomerCode = a.CustomerCode
							   and c.Chassis = a.Chassis
							 where a.CompanyCode like @CompanyCode
							   and a.BranchCode like @BranchCode
							   and isnull(c.Chassis, '') = ''
							   and isnull(c.StnkExpiredDate, isnull(c.StnkDate, a.BpkDate)) >= (select RemDate from @t_rem where RemCode = 'REMSTNKEXT')
					)
	 where RemCode = 'REMSTNKEXT';

	 update @t_rem set RemValue = (
							select count(a.CustomerCode)
							  from CsCustomerVehicleView a
							 inner join CsCustomerView b 
								on b.CompanyCode = a.CompanyCode
							   and b.CustomerCode = a.CustomerCode
							  left join CsTdayCall c
								on c.CompanyCode = a.CompanyCode
							   and c.CustomerCode = a.CustomerCode
							   and c.Chassis = a.Chassis
							 where a.CompanyCode like @CompanyCode
							   and a.BranchCode like @BranchCode
							   and isnull(c.Chassis, '') = ''
							   and a.DoDate >=  convert(datetime, @TDaysCallCutOffPeriod)
					)
	 where RemCode = 'REM3DAYSCALL';
end;
  update @t_rem set RemValue = (
						select count(a.CustomerCode)
						  from CsCustomerVehicleView a
						 inner join CsCustomerView b 
						    on b.CompanyCode = a.CompanyCode
						   and b.CustomerCode = a.CustomerCode
						  left join CsCustBpkb c
						    on c.CompanyCode = a.CompanyCode
						   and c.CustomerCode = a.CustomerCode
						   and c.Chassis = a.Chassis
						 where a.CompanyCode like @CompanyCode
						   and a.BranchCode like @BranchCode
						   and ( 
									isnull(c.Chassis, '') = ''
									or 
									isnull(c.BpkbReadyDate, '') = ''
									or
									isnull(isnull(c.BpkbPickUp, (select top 1 x.RetrievalEstimationDate from CsBpkbRetrievalInformation x where x.CompanyCode=a.CompanyCode and x.CustomerCode=c.CustomerCode and (x.IsDeleted = 0 or x.IsDeleted is null) order by x.RetrievalEstimationDate desc)), '') = ''
									or
									isnull(isnull(c.BpkbPickUp, (select top 1 x.RetrievalEstimationDate from CsBpkbRetrievalInformation x where x.CompanyCode=a.CompanyCode and x.CustomerCode=c.CustomerCode order by x.RetrievalEstimationDate desc)), '') != '' and isnull(c.BpkbPickUp, (select top 1 x.RetrievalEstimationDate from CsBpkbRetrievalInformation x where x.CompanyCode=a.CompanyCode and x.CustomerCode=c.CustomerCode and (x.IsDeleted = 0 or x.IsDeleted is null) order by x.RetrievalEstimationDate desc)) < getdate() 
									or
									isnull(c.BpkbReadyDate, '') != '' and isnull(isnull(c.BpkbPickUp, (select top 1 x.RetrievalEstimationDate from CsBpkbRetrievalInformation x where x.CompanyCode=a.CompanyCode and x.CustomerCode=c.CustomerCode and (x.IsDeleted = 0 or x.IsDeleted is null) order by x.RetrievalEstimationDate desc)), '') = ''
							   )
						   and a.DoDate >=  (select RemDate from @t_rem where RemCode = 'REMBPKB')
				)
  where RemCode = 'REMBPKB';
  
  set @DateComparison = (select RemDate from @t_rem where RemCode = 'REMBDAYSCALL');
  set @CurrentMonth = datepart(month, @DateComparison);
  set @NextMonth = @CurrentMonth + 1;
  set @PreviousMonth = @CurrentMonth - 1;

 update @t_rem set RemValue = (
						select count(a.CustomerCode)
						  from CsCustomerVehicleView a
						 inner join CsCustomerView b 
						    on b.CompanyCode = a.CompanyCode
						   and b.CustomerCode = a.CustomerCode
						  left join CsCustBirthday c
						    on c.CompanyCode = a.CompanyCode
						   and c.CustomerCode = a.CustomerCode
						 where a.CompanyCode like @CompanyCode
						   and a.BranchCode like @BranchCode
						   and isnull(c.CustomerCode, '') = ''
						   and datepart(month, b.BirthDate) >= @PreviousMonth
						   and datepart(month, b.BirthDate) <= @CurrentMonth
				)
 where RemCode = 'REMBDAYSCALL';

select a.RemCode, a.RemDate, a.RemValue, b.SettingLink1 as ControlLink
  from @t_rem a
  join CsSettings b
    on b.CompanyCode = @CompanyCode
   and b.SettingCode = a.RemCode
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

var start1 = function (cfg, callback) 
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