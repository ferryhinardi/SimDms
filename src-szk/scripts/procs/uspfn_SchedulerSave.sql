
go
if object_id('uspfn_SchedulerSave') is not null
	drop procedure uspfn_SchedulerSave;

go
create procedure uspfn_SchedulerSave
	@DealerCode varchar(17),
	@ScheduleName varchar(150),
	@DateStart datetime,
	@DateFinish datetime,
	@RunningTimes varchar(50),
	@IsError bit,
	@ErrorMessage varchar(max),
	@Info varchar(max)
as

begin
	declare @TransactionName varchar(25) = 'SaveSchedulerLog';

	begin transaction @TransactionName;
	
	begin try
		insert into GnSchedulerLog ( ScheduleID, DealerCode, SchduleName, DateStart, DateFinish, RunningTimes, IsError, ErrorMessage, Info )
		values (newid(), @DealerCode, @ScheduleName, @DateStart, @DateFinish, @RunningTimes, @IsError, @ErrorMessage, @Info);

		commit transaction @TransactionName;
	end try
	begin catch
		rollback transaction @TransactionName;
	end catch;
end

go
--exec uspfn_SchedulerSave '6006406', 'Test Save', '2014-07-15 13:00:15', '2014-07-15 13:59:59', 'Start from the end', 0, '-', '-'