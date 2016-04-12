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
	
var TaskName = "Update SP CsInqTDaysCall";

var CurrentDealerCode = argv.d;

function hereDoc(f) {
  return f.toString().
	  replace(/^[^\/]+\/\*!?/, '').
	  replace(/\*\/[^\/]+$/, '');
}

var SQL = hereDoc(function(){/*!
if object_id('uspfn_CsInqTDaysCall') is not null
	drop procedure uspfn_CsInqTDaysCall
GO
create procedure uspfn_CsInqTDaysCall
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
	declare @TDaysCallCutOffPeriod varchar(20);
	declare @TDaysCallSettingParam3 varchar(20);

	set @TDaysCallCutOffPeriod = '';
	set @TDaysCallSettingParam3 = '';

	set @CurrDate = getdate()

	-- REM3DAYSCALL
	set @Param1 = isnull((select SettingParam1 from CsSettings where CompanyCode = @CompanyCode and SettingCode = 'REM3DAYSCALL'), '0')
	begin try
		insert into @t_rem (RemCode, RemDate) values('REM3DAYSCALL', left(convert(varchar, dateadd(month, -convert(int, @Param1), @CurrDate), 112), 6) + '01')
	end try
	begin catch
		

		set @TDaysCallCutOffPeriod = ( select top 1 a.SettingParam1 from CsSettings a where a.CompanyCode = @CompanyCode);
		set @TDaysCallSettingParam3 = ( select top 1 a.SettingParam3 from CsSettings a where a.CompanyCode = @CompanyCode);
		insert into @t_rem (RemCode, RemDate) values('REM3DAYSCALL', convert(datetime, @TDaysCallCutOffPeriod));
	end catch

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
	
	if @Status = 'Inquiry' 
	begin
		select *
		  from CsLkuTDaysCallView a
		 where a.CompanyCode like @CompanyCode
		   and a.BranchCode like @BranchCode
		   and a.Outstanding = @Outstanding
		   and convert(varchar, a.CreatedDate, 112) between @DateFrom and @DateTo
	end
	else if @Status = 'Lookup'
	begin
		if @Outstanding = 'Y'
		begin
			if @TDaysCallSettingParam3 != 'CUTOFF'
			begin
				select *
				  from CsLkuTDaysCallView a
				 where a.CompanyCode like @CompanyCode
				   and a.BranchCode like @BranchCode
				   and a.DoDate >=  (select RemDate from @t_rem where RemCode = 'REM3DAYSCALL')
				   and a.Outstanding = 'Y'
			end
			else
			begin
				select *
				  from CsLkuTDaysCallView a
				 where a.CompanyCode like @CompanyCode
				   and a.BranchCode like @BranchCode
				   and a.DoDate >=  convert(datetime, @TDaysCallCutOffPeriod)
				   and a.Outstanding = 'Y'
			end
		end
		else
		begin
			select *
			  from CsLkuTDaysCallView a
			 where a.CompanyCode like @CompanyCode
			   and a.BranchCode like @BranchCode
			   and a.DoDate >=  (select RemDate from @t_rem where RemCode = 'REM3DAYSCALL')
			   and a.Outstanding = 'N'
		end
	end
 end
GO
 if object_id('uspfn_CsDashSummary') is not null
	drop procedure uspfn_CsDashSummary
GO
 create procedure uspfn_CsDashSummary
	@CompanyCode nvarchar(20),
	@BranchCode varchar(20)
as

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

begin try
	insert into @t_rem (RemCode, RemDate) values('REM3DAYSCALL', left(convert(varchar, dateadd(month, -convert(int, @Param1), @CurrDate), 112), 6) + '01')
end try
begin catch
end catch

-- REMBDAYSCALL
set @Param1 = isnull((select SettingParam1 from CsSettings where CompanyCode = @CompanyCode and SettingCode = 'REMBDAYSCALL'), '0')
insert into @t_rem (RemCode, RemDate) values('REMBDAYSCALL', left(convert(varchar, dateadd(month, -convert(int, @Param1), @CurrDate), 112), 6) + '01')

-- REMHOLIDAYS
set @Param1 = isnull((select SettingParam1 from CsSettings where CompanyCode = @CompanyCode and SettingCode = 'REMHOLIDAYS'), '0')
insert into @t_rem (RemCode, RemDate) values('REMHOLIDAYS', left(convert(varchar, dateadd(month, -convert(int, @Param1), @CurrDate), 112), 6) + '01')

-- REMSTNKEXT
set @Param1 = isnull((select SettingParam1 from CsSettings where CompanyCode = @CompanyCode and SettingCode = 'REMSTNKEXT'), '0')
insert into @t_rem (RemCode, RemDate) values('REMSTNKEXT', left(convert(varchar, dateadd(month, -convert(int, @Param1), @CurrDate), 112), 6) + '01')

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