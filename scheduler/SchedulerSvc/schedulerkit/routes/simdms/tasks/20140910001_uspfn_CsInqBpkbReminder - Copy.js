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
	
var TaskName = "alter_uspfn_CsInqBpkbReminder_20140910";

var CurrentDealerCode = argv.d;

function hereDoc(f) {
  return f.toString().
	  replace(/^[^\/]+\/\*!?/, '').
	  replace(/\*\/[^\/]+$/, '');
}

var SQL = hereDoc(function(){/*!
alter procedure uspfn_CsInqBpkbReminder
	@CompanyCode nvarchar(20),
	@BranchCode varchar(20),
	@DateFrom datetime,
	@DateTo datetime,
	@Outstanding char(1),
	@Status varchar(15)
as
begin
	declare @CurrDate datetime, @Param1 as varchar(20)
	declare @t_rem as table
	(
		RemCode varchar(20),
		RemDate datetime,
		RemValue int
	)

	set @CurrDate = getdate()

	-- REMBPKB
	set @Param1 = isnull((select SettingParam1 from CsSettings where CompanyCode = @CompanyCode and SettingCode = 'REMBPKB'), '0')
	insert into @t_rem (RemCode, RemDate) values('REMBPKB', left(convert(varchar, dateadd(month, -convert(int, @Param1), @CurrDate), 112), 6) + '01')
   
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


	  update @t_rem set RemValue = (
							select count(a.CustomerCode)
							  from CsLkuBpkbReminderView a
							 where a.CompanyCode like @CompanyCode
							   and a.BranchCode like @BranchCode
							   and a.DoDate >=  (select RemDate from @t_rem where RemCode = 'REMBPKB')
							   and a.Outstanding = 'Y'
					)
	  where RemCode = 'REMBPKB';

	  if @Status = 'Inquiry'
	  begin
			select*
			  from CsLkuBpkbReminderView a
			 where a.CompanyCode like @CompanyCode
			   and a.BranchCode like @BranchCode
			   ----and a.DoDate >=  (select RemDate from @t_rem where RemCode = 'REMBPKB')
			   and a.DoDate >= @DateFrom
			   and a.DoDate <=  @DateTo
			   and a.Outstanding = @Outstanding
	  end
	  else if @Status = 'Lookup'
	  begin
        if @Outstanding = 'Y'
        begin
			select *
			  from CsLkuBpkbReminderView a
			 where a.CompanyCode like @CompanyCode
			   and a.BranchCode like @BranchCode
			   and a.DoDate >=  (select RemDate from @t_rem where RemCode = 'REMBPKB')
			   and not exists (select top 1 1 from CsCustBpkb where CompanyCode = a.CompanyCode and CustomerCode = a.CustomerCode and Chassis = a.Chassis)
        end
        else 
        begin
			select distinct *
			  from CsLkuBpkbReminderView a
			 where a.CompanyCode like @CompanyCode
			   and a.BranchCode like @BranchCode
			   and a.DoDate >=  (select RemDate from @t_rem where RemCode = 'REMBPKB')
			   and exists (select top 1 1 from CsCustBpkb where CompanyCode = a.CompanyCode and CustomerCode = a.CustomerCode and Chassis = a.Chassis)
        end
	  end
 end
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