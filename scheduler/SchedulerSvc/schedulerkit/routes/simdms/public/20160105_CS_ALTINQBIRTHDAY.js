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
	
var TaskName = "ALTER INQ CUST BIRTHDAY";
var TaskNo = "CS_ALTINQBIRTHDAY";
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
IF (OBJECT_ID('uspfn_CsInqCustomerBirthday') IS NOT NULL)
DROP proc [dbo].[uspfn_CsInqCustomerBirthday]
GO

CREATE procedure [dbo].[uspfn_CsInqCustomerBirthday]
	@CompanyCode nvarchar(20),
	@BranchCode varchar(20),
	@Year int,
	@MonthFrom int,
	@MonthTo int,
	@Outstanding char(1),
	@Status varchar(10)
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

	-- REMBDAYSCALL
	set @Param1 = isnull((select SettingParam1 from CsSettings where CompanyCode = @CompanyCode and SettingCode = 'REMBDAYSCALL'), '0')
	insert into @t_rem (RemCode, RemDate) values('REMBDAYSCALL', left(convert(varchar, dateadd(month, -convert(int, @Param1), @CurrDate), 112), 6) + '01')

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
  
	  set @DateComparison = (select RemDate from @t_rem where RemCode = 'REMBDAYSCALL');
	  set @CurrentMonth = datepart(month, @DateComparison);
	  set @NextMonth = @CurrentMonth + 1;
	  set @PreviousMonth = @CurrentMonth - 1;

	if @Status = 'Inquiry'
	begin
		   if isnull(@Outstanding, '') = '' or rtrim(@Outstanding) = '' 
		   begin
				select *
	   			  from CsLkuBirthdayView a
				 where a.CompanyCode like @CompanyCode
				   and a.BranchCode like @BranchCode
				   and datepart(month, a.CustomerBirthDate) = datepart(month, getdate()) 
				   --and datepart(month, a.CustomerBirthDate) >= @MonthFrom 
				   --and datepart(month, a.CustomerBirthDate) <= @MonthTo
				   and a.PeriodOfYear = @Year
		   end
		   else
		   begin
				if @Outstanding = 'N'
				begin
					select *
	   				  from CsLkuBirthdayView a
					 where a.CompanyCode like @CompanyCode
					   and a.BranchCode like @BranchCode
					   and datepart(month, a.CustomerBirthDate) = datepart(month, getdate()) 
					   --and datepart(month, a.CustomerBirthDate) >= @MonthFrom 
					   --and datepart(month, a.CustomerBirthDate) <= @MonthTo
					   and a.PeriodOfYear = @Year
					   and a.Outstanding = @Outstanding;
				end
				else if @Outstanding = 'Y'
				begin
					select *
	   				  from CsLkuBirthdayView a
					 where a.CompanyCode like @CompanyCode
					   and a.BranchCode like @BranchCode
					   and datepart(month, a.CustomerBirthDate) = datepart(month, getdate()) 
					   --and datepart(month, a.CustomerBirthDate) >= @MonthFrom 
					   --and datepart(month, a.CustomerBirthDate) <= @MonthTo
					   and a.Outstanding = @Outstanding;
				end 
		   end
	end
	else if @Status = 'Lookup'
	begin
		if @Outstanding = 'Y'
		begin
			select *	
			  from CsLkuBirthdayView a
			 where a.CompanyCode like @CompanyCode
			   and a.BranchCode like @BranchCode
			   and a.Outstanding = @Outstanding
			   and datepart(month, a.CustomerBirthDate) = datepart(month, getdate()) 
			   --and datepart(month, a.CustomerBirthDate) >= @PreviousMonth
			   --and datepart(month, a.CustomerBirthDate) <= @CurrentMonth
		end
		else if @Outstanding = 'N'
		begin
			select *	
			  from CsLkuBirthdayView a
			 where a.CompanyCode like @CompanyCode
			   and a.BranchCode like @BranchCode
			   and a.Outstanding = @Outstanding
			   --and a.PeriodOfYear = @Year
			   and datepart(month, a.CustomerBirthDate) = datepart(month, getdate()) 
			   --and datepart(month, a.CustomerBirthDate) >= @PreviousMonth
			   --and datepart(month, a.CustomerBirthDate) <= @CurrentMonth
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