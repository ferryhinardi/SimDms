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
	
var TaskName = "Update Sync CS";
var TaskNo = "20141010003";
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
DECLARE @OBJT varchar(32)
select @OBJT=TABLE_TYPE from INFORMATION_SCHEMA.TABLES
where TABLE_NAME = 'CsSettings'
IF @OBJT IS NULL
BEGIN
	CREATE TABLE [dbo].[CsSettings](
		[CompanyCode] [varchar](20) NOT NULL,
		[SettingCode] [varchar](20) NOT NULL,
		[SettingDesc] [varchar](250) NULL,
		[SettingParam1] [varchar](20) NULL,
		[SettingParam2] [varchar](20) NULL,
		[SettingParam3] [varchar](20) NULL,
		[SettingParam4] [varchar](20) NULL,
		[SettingParam5] [varchar](20) NULL,
		[SettingLink1] [varchar](20) NULL,
		[SettingLink2] [varchar](20) NULL,
		[SettingLink3] [varchar](20) NULL,
		[IsDeleted] [bit] NULL,
		[CreatedBy] [varchar](36) NULL,
		[CreatedDate] [datetime] NULL,
		[UpdatedBy] [varchar](36) NULL,
		[UpdatedDate] [datetime] NULL,
	 CONSTRAINT [PK_CsSettings] PRIMARY KEY CLUSTERED 
	(
		[CompanyCode] ASC,
		[SettingCode] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
END	
GO
EXEC uspfn_createDefaultUserCS02
GO
select 'Initialize createddate and updateddate on CsSettings' info
GO
;with x as (select * from CsSettings where CreatedDate is null)
update x set CreatedDate = getdate()
GO
;with x as (select * from CsSettings where UpdatedDate is null)
update x set UpdatedDate = getdate()
GO
select 'Checking for CsCustomerView' info
GO
DECLARE @OBJT varchar(32)
select @OBJT=TABLE_TYPE from INFORMATION_SCHEMA.TABLES
where TABLE_NAME = 'CsCustomerView'
IF NOT @OBJT IS NULL
BEGIN
	IF @OBJT='VIEW'
		drop view CsCustomerView
	ELSE
		drop table CsCustomerView
END	
GO	
select 'Generate Table CsCustomerView' info
GO
CREATE TABLE [dbo].[CsCustomerView](
	[CompanyCode] [varchar](15) NOT NULL,
	[BranchCode] [varchar](15) NOT NULL,
	[CustomerCode] [varchar](15) NOT NULL,
	[CustomerName] [varchar](100) NULL,
	[CustomerType] [varchar](15) NULL,
	[Address] [varchar](301) NULL,
	[PhoneNo] [varchar](15) NULL,
	[HPNo] [varchar](15) NULL,
	[AddPhone1] [varchar](20) NULL,
	[AddPhone2] [varchar](20) NULL,
	[BirthDate] [datetime] NULL,
	[ReligionCode] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[LastUpdateDate] [datetime] NULL
) ON [PRIMARY]
GO
DECLARE @OBJT varchar(32)
select @OBJT=TABLE_TYPE from INFORMATION_SCHEMA.TABLES
where TABLE_NAME = 'CsCustomerVehicleView'
IF NOT @OBJT IS NULL
BEGIN
	IF @OBJT='VIEW'
		drop view CsCustomerVehicleView
	ELSE
		drop table CsCustomerVehicleView
END	
GO
select 'Generate Table CsCustomerVehicleView' info
GO
CREATE TABLE [dbo].[CsCustomerVehicleView](
	[CompanyCode] [varchar](15) NOT NULL,
	[BranchCode] [varchar](15) NOT NULL,
	[CustomerCode] [varchar](15) NOT NULL,
	[Chassis] [varchar](45) NULL,
	[Engine] [varchar](45) NULL,
	[SONo] [varchar](15) NOT NULL,
	[DONo] [varchar](15) NULL,
	[DODate] [datetime] NULL,
	[BpkNo] [varchar](15) NULL,
	[CarType] [varchar](20) NULL,
	[Color] [varchar](15) NULL,
	[SalesmanCode] [varchar](15) NULL,
	[SalesmanName] [varchar](150) NULL,
	[PoliceRegNo] [varchar](55) NULL,
	[DeliveryDate] [datetime] NULL,
	[SalesModelCode] [varchar](20) NULL,
	[SalesModelYear] [numeric](4, 0) NULL,
	[ColourCode] [varchar](15) NULL,
	[BPKDate] [datetime] NULL,
	[IsLeasing] [bit] NULL,
	[LeasingCo] [varchar](15) NULL,
	[LeasingName] [varchar](100) NULL,
	[Installment] [numeric](3, 0) NULL
) ON [PRIMARY]

GO
select 'Checking for view CsLkuBirthdayView' info
GO
DECLARE @OBJT varchar(32)
select @OBJT=TABLE_TYPE from INFORMATION_SCHEMA.TABLES
where TABLE_NAME = 'CsLkuBirthdayView'
IF NOT @OBJT IS NULL
BEGIN
	IF @OBJT='VIEW'
		drop view CsLkuBirthdayView
	ELSE
		drop table CsLkuBirthdayView
END	
GO	
select 'Regenerate view CsLkuBirthdayView' info
GO
create view CsLkuBirthdayView
as
select a.CompanyCode
	 , a.BranchCode
	 , a.CustomerCode
	 , a.CustomerName
	 , CustomerAddress = a.Address
	 , PeriodOfYear = c.PeriodYear
	 , c.AdditionalInquiries
	 , Status = c.Status
	 , CustomerTelephone = a.PhoneNo
	 , CustomerHandphone = a.HPNo
	 , CustomerAddPhone1 = a.AddPhone1
	 , CustomerAddPhone2 = a.AddPhone2
	 , CustomerBirthDate = a.BirthDate
	 , CustomerGiftSentDate = c.SentGiftDate 
	 , CustomerGiftReceivedDate = c.ReceivedGiftDate
	 , CustomerTypeOfGift = c.TypeOfGift
	 , CustomerComment = c.Comment
	 , SpouseName = c.SpouseName
	 , SpouseTelephone = c.SpouseTelephone
	 , SpouseRelation = c.SpouseRelation
	 , SpouseBirthDate = c.SpouseBirthday
	 , SpouseGiftSentDate = c.SpouseGiftSentDate
	 , SpouseGiftReceivedDate = c.SpouseGiftReceivedDate
	 , SpouseTypeOfGift = c.SpouseTypeOfGift
	 , SpouseComment = c.SpouseComment
	 , ChildName1 = c.ChildName1
	 , ChildTelephone1 = c.ChildTelephone1
	 , ChildRelation1 = c.ChildRelation1
	 , ChildBirthDate1 = c.ChildBirthday1
	 , ChildGiftSentDate1 = c.ChildGiftSentDate1
	 , ChildGiftReceivedDate1 = c.ChildGiftReceivedDate1
	 , ChildTypeOfGift1 = c.ChildTypeOfGift1
	 , ChildComment1 = c.ChildComment1
	 , ChildName2 = c.ChildName2
	 , ChildTelephone2 = c.ChildTelephone2
	 , ChildRelation2 = c.ChildRelation2
	 , ChildBirthDate2 = c.ChildBirthday2
	 , ChildGiftSentDate2 = c.ChildGiftSentDate2
	 , ChildGiftReceivedDate2 = c.ChildGiftReceivedDate2
	 , ChildTypeOfGift2 = c.ChildTypeOfGift2
	 , ChildComment2 = c.ChildComment2
	 , ChildName3 = c.ChildName3
	 , ChildTelephone3 = c.ChildTelephone3
	 , ChildRelation3 = c.ChildRelation3
	 , ChildBirthDate3 = c.ChildBirthday3
	 , ChildGiftSentDate3 = c.ChildGiftSentDate3
	 , ChildGiftReceivedDate3 = c.ChildGiftReceivedDate3
	 , ChildTypeOfGift3 = c.ChildTypeOfGift3
	 , ChildComment3 = c.ChildComment3
	 , NumberOfChildren = (
			(
				case 
					when isnull(c.ChildName1, 0) = 0 then 0
					else 1
				end
			)
			+
			(
				case 
					when isnull(c.ChildName2, 0) = 0 then 0
					else 1
				end
			)
			+
			(
				case 
					when isnull(c.ChildName3, 0) = 0 then 0
					else 1
				end
			)
	   )
	 , NumberOfSpouse = ( 
			select count(x.CustomerCode) 
			  from CsCustBirthday x 
			 where x.CompanyCode = a.CompanyCode
			   and x.CustomerCode = a.CustomerCode
			   and isnull(x.SpouseName, '') != ''
	   )
	 , Outstanding = (
			case
				when isnull(c.CustomerCode, '') = '' then 'Y'
				else 'N'
			end
	   )
	 , InputDate = c.CreatedDate
  from CsCustomerView a
  left join CsCustomerVehicleView b
    on b.CompanyCode = a.CompanyCode
   and b.BranchCode = a.BranchCode
   and b.CustomerCode = a.CustomerCode
  left join CsCustBirthday c
    on c.CompanyCode = a.CompanyCode
   and c.CustomerCode = a.CustomerCode
GO
select 'Checking for view CsLkuBpkbReminderView' info
GO
DECLARE @OBJT varchar(32)
select @OBJT=TABLE_TYPE from INFORMATION_SCHEMA.TABLES
where TABLE_NAME = 'CsLkuBpkbReminderView'
IF NOT @OBJT IS NULL
BEGIN	
	IF @OBJT='VIEW'
		drop view CsLkuBpkbReminderView
	ELSE
		drop table CsLkuBpkbReminderView
END
GO
select 'Regenerate view CsLkuBpkbReminderView' info
GO
create view CsLkuBpkbReminderView
as
select a.CompanyCode
     , a.BranchCode
	 , a.CustomerCode
	 , a.CustomerName
	 , b.DODate
	 , b.Chassis
	 , b.Engine
	 , b.SalesModelCode
	 , b.SalesModelYear
	 , b.ColourCode
	 , c.BpkbReadyDate
	 , c.BpkbPickUp
	 , b.BPKDate
	 , b.IsLeasing
	 , b.LeasingCo
	 , b.Installment
	 , case isnull(b.isLeasing, 0) when 0 then 'Cash' else 'Leasing' end as LeasingDesc
	 , case isnull(b.isLeasing, 0) when 0 then '-' else b.LeasingName end as LeasingName
	 , case isnull(b.isLeasing, 0) when 0 then '-' else (convert(varchar, isnull(b.Installment, 0)) + ' Month') end as Tenor
	 , a.Address
	 , a.PhoneNo
	 , a.HpNo
	 , a.AddPhone1
	 , a.AddPhone2
	 , b.SalesmanCode 
	 , b.SalesmanName
	 , c.ReqKtp
     , c.ReqStnk
     , c.ReqSuratKuasa
     , c.Comment
     , c.Additional
	 , BpkbDate = isnull(d.BPKBDate, b.BPKDate)
	 , StnkDate = isnull(d.StnkDate, b.BPKDate)
	 , Category = ( 
			case 
				 when isnull(b.isLeasing, 0) = 0 then 'Tunai' 
				 else 'Leasing'
			end
	   )
     , c.Status
	 , (case c.Status when 1 then 'Finish' else 'In Progress' end) as StatusInfo
	 , b.PoliceRegNo
	 , InputDate = a.CreatedDate
	 , DelayedRetrievalDate = (
			select top 1 x.RetrievalEstimationDate
			  from CsBpkbRetrievalInformation x
			 where x.CompanyCode = a.CompanyCode
			   and x.CustomerCode = a.CustomerCode
			 order by x.RetrievalEstimationDate desc
	   )
	 , DelayedRetrievalNote = (
			select top 1 x.Notes
			  from CsBpkbRetrievalInformation x
			 where x.CompanyCode = a.CompanyCode
			   and x.CustomerCode = a.CustomerCode
			 order by x.RetrievalEstimationDate desc
	   )
	 , Outstanding = (
			case 
				when isnull(c.CustomerCode, '') = '' then 'Y'
				when c.BpkbPickUp >= c.BpkbReadyDate then 'N'
				when ( 
						select top 1 x.RetrievalEstimationDate 
						  from CsBpkbRetrievalInformation x 
						 where x.CompanyCode = a.CompanyCode
						   and x.CustomerCode = a.CustomerCode
						 order by x.RetrievalEstimationDate desc
					 ) > getdate() then 'N'
				else 'Y'
			end	
	   )
  from CsCustomerView a
  left join CsCustomerVehicleView b
    on b.CompanyCode = a.CompanyCode
   and b.BranchCode = a.BranchCode
   and b.CustomerCode = a.CustomerCode
  left join CsCustBpkb c
    on c.CompanyCode = b.CompanyCode
   and c.CustomerCode = b.CustomerCode
  left join CsCustomerVehicle d
    on d.CompanyCode = c.CompanyCode
   and d.Chassis = b.Chassis
GO
select 'Checking for view CsLkuStnkExtensionView' info
GO
DECLARE @OBJT varchar(32)
select @OBJT=TABLE_TYPE from INFORMATION_SCHEMA.TABLES
where TABLE_NAME = 'CsLkuStnkExtensionView'
IF NOT @OBJT IS NULL
BEGIN
	IF @OBJT='VIEW'
		drop view CsLkuStnkExtensionView
	ELSE
		drop table CsLkuStnkExtensionView
END
GO
select 'Regenerate view CsLkuStnkExtensionView' info
GO
create view CsLkuStnkExtensionView
as
select a.CompanyCode
     , a.BranchCode
	 , a.CustomerCode
	 , a.CustomerName 
	 , b.Chassis
	 , b.Engine
	 , b.SalesModelCode
	 , b.SalesModelYear
	 , b.ColourCode
	 , b.DODate
	 , BpkbDate = isnull(c.BpkbDate, b.BPKDate) 
	 , StnkDate = isnull(c.StnkDate, b.BPKDate) 
	 , b.BPKDate
	 , b.IsLeasing
	 , b.LeasingCo
	 , b.Installment
	 , LeasingDesc = case isnull(b.isLeasing, 0) when 0 then 'Cash' else 'Leasing' end
	 , LeasingName = case isnull(b.isLeasing, 0) when 0 then '-' else b.LeasingName end
	 , Tenor = case isnull(b.isLeasing, 0) when 0 then '-' else (convert(varchar, isnull(b.Installment, 0)) + ' Month') end
	 , Address = isnull(a.Address, '-')
	 , PhoneNo = isnull(a.PhoneNo, '-')
	 , HpNo = isnull(a.HpNo, '-')
	 , AddPhone1 = isnull(a.AddPhone1, '-')
	 , AddPhone2 = isnull(a.AddPhone2, '-')
	 , SalesmanCode = isnull(b.SalesmanCode, '-')
	 , SalesmanName = isnull(b.SalesmanName, '-')
	 , IsStnkExtend = isnull(c.IsStnkExtend, 0)
	 , InputDate = c.CreatedDate
	 , Outstanding = (
			case
				when isnull(c.CustomerCode, '') = '' then 'Y'
				when c.Ownership = 0 then 'N'
				when isnull(c.Ownership, '') = '' or c.Ownership = 0 then 'N'
				else 'N'
			end
	   )
	 , Category = ( 
			case 
				 when isnull(b.isLeasing, 0) = 0 then 'Tunai' 
				 else 'Leasing'
			end
	   )
	 , Salesman = isnull(b.SalesmanName, '-')
	 , c.StnkExpiredDate
	 , c.ReqKtp
	 , c.ReqStnk
	 , c.ReqBpkb
	 , c.ReqSuratKuasa
	 , c.Comment
	 , c.Additional
	 , c.Status
	 , c.Ownership
	 , StatusInfo = ''
	 , b.PoliceRegNo
  from CsCustomerView a
  left join CsCustomerVehicleView b
    on b.CompanyCode = a.CompanyCode
   and b.BranchCode = a.BranchCode
   and b.CustomerCode = a.CustomerCode
  left join CsStnkExt c
    on c.CompanyCode = b.CompanyCode
   and c.CustomerCode = b.CustomerCode
GO
select 'Checking for view CsLkuTDaysCallView' info
GO
DECLARE @OBJT varchar(32)
select @OBJT=TABLE_TYPE from INFORMATION_SCHEMA.TABLES
where TABLE_NAME = 'CsLkuTDaysCallView'
IF NOT @OBJT IS NULL
BEGIN	
	IF @OBJT='VIEW'
		drop view CsLkuTDaysCallView
	ELSE
		drop table CsLkuTDaysCallView
END
GO
select 'Regenerate view CsLkuTDaysCallView' info
GO
create view CsLkuTDaysCallView
as
select a.CompanyCode
     , a.BranchCode
	 , a.CustomerCode
	 , a.CustomerName
	 , a.Address 
	 , a.PhoneNo
	 , a.HPNo
	 , AddPhone1 = isnull(a.AddPhone1, '-')
	 , AddPhone2 = isnull(a.AddPhone2, '-')
	 , CreatedDate = c.CreatedDate
	 , Outstanding = (
			case
				when isnull(c.CustomerCode, '') = '' then 'Y'
				else 'N'
			end
	   )
     , a.BirthDate
     , b.CarType
     , b.Color
     , b.PoliceRegNo
     , b.Chassis
     , b.Engine 
     , b.SONo
     , b.SalesmanCode
     , b.SalesmanName
     , b.SalesmanName as Salesman
     , a.ReligionCode
	 , b.SalesModelCode
	 , b.SalesModelYear
	 , b.DODate
	 , b.BPKDate
     , case c.IsDeliveredA when 1 then 'Ya' else 'Tidak' end IsDeliveredA
     , case c.IsDeliveredB when 1 then 'Ya' else 'Tidak' end IsDeliveredB
     , case c.IsDeliveredC when 1 then 'Ya' else 'Tidak' end IsDeliveredC
     , case c.IsDeliveredD when 1 then 'Ya' else 'Tidak' end IsDeliveredD
     , case c.IsDeliveredE when 1 then 'Ya' else 'Tidak' end IsDeliveredE
     , case c.IsDeliveredF when 1 then 'Ya' else 'Tidak' end IsDeliveredF
     , case c.IsDeliveredG when 1 then 'Ya' else 'Tidak' end IsDeliveredG
     , c.Comment
	 , c.Additional
	 , c.Status
	 , b.DeliveryDate
  from CsCustomerView a
  left join CsCustomerVehicleView b
    on b.CompanyCode = a.CompanyCode
   and b.BranchCode = a.BranchCode
   and b.CustomerCode = a.CustomerCode
  left join CsTDayCall c
    on c.CompanyCode = b.CompanyCode
   and c.CustomerCode = b.CustomerCode
GO
if object_id('uspfn_CsInqCustomerBirthday') is not null
	drop procedure uspfn_CsInqCustomerBirthday
GO
select 'Create SP uspfn_CsInqCustomerBirthday' info
GO
create procedure uspfn_CsInqCustomerBirthday
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
				   and datepart(month, a.CustomerBirthDate) >= @MonthFrom 
				   and datepart(month, a.CustomerBirthDate) <= @MonthTo
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
					   and datepart(month, a.CustomerBirthDate) >= @MonthFrom 
					   and datepart(month, a.CustomerBirthDate) <= @MonthTo
					   and a.PeriodOfYear = @Year
					   and a.Outstanding = @Outstanding;
				end
				else if @Outstanding = 'Y'
				begin
					select *
	   				  from CsLkuBirthdayView a
					 where a.CompanyCode like @CompanyCode
					   and a.BranchCode like @BranchCode
					   and datepart(month, a.CustomerBirthDate) >= @MonthFrom 
					   and datepart(month, a.CustomerBirthDate) <= @MonthTo
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
			   and datepart(month, a.CustomerBirthDate) >= @PreviousMonth
			   and datepart(month, a.CustomerBirthDate) <= @CurrentMonth
		end
		else if @Outstanding = 'N'
		begin
			select *	
			  from CsLkuBirthdayView a
			 where a.CompanyCode like @CompanyCode
			   and a.BranchCode like @BranchCode
			   and a.Outstanding = @Outstanding
			   and a.PeriodOfYear = @Year
			   and datepart(month, a.CustomerBirthDate) >= @PreviousMonth
			   and datepart(month, a.CustomerBirthDate) <= @CurrentMonth
		end
		
	end
 end
GO
select 'Create SP uspfn_CsInqTDaysCall' info
GO
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
select 'Create SP uspfn_CsStnkExtension' info
GO
if object_id('uspfn_CsStnkExtension') is not null
	drop procedure uspfn_CsStnkExtension
GO
create procedure uspfn_CsStnkExtension
	@CompanyCode nvarchar(20),
	@BranchCode varchar(20),
	@IsStnkExtension bit,
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

	-- REMSTNKEXT
	set @Param1 = isnull((select SettingParam1 from CsSettings where CompanyCode = @CompanyCode and SettingCode = 'REMSTNKEXT'), '0')
	insert into @t_rem (RemCode, RemDate) values('REMSTNKEXT', left(convert(varchar, dateadd(month, -convert(int, @Param1), @CurrDate), 112), 6) + '01')

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
		if isnull(@IsStnkExtension, '') = '' 
		begin
			select *
			  from CsLkuStnkExtensionView a
			 where a.CompanyCode like @CompanyCode
			   and a.BranchCode like @BranchCode
			   and isnull(a.StnkExpiredDate, isnull(a.StnkDate, a.BpkDate)) >= @DateFrom
			   and isnull(a.StnkExpiredDate, isnull(a.StnkDate, a.BpkDate)) <= @DateTo
			   and a.Outstanding = @Outstanding
		end
		else
		begin
			select *
			  from CsLkuStnkExtensionView a
			 where a.CompanyCode like @CompanyCode
			   and a.BranchCode like @BranchCode
			   and isnull(a.StnkExpiredDate, isnull(a.StnkDate, a.BpkDate)) >= @DateFrom
			   and isnull(a.StnkExpiredDate, isnull(a.StnkDate, a.BpkDate)) <= @DateTo
			   and a.Outstanding = @Outstanding
			   and a.IsStnkExtend = @IsStnkExtension
		end
	end
	else if @Status = 'Lookup'
	begin
		select *
		  from CsLkuStnkExtensionView a
		 where a.CompanyCode like @CompanyCode
		   and a.BranchCode like @BranchCode
		   and isnull(a.StnkExpiredDate, isnull(a.StnkDate, a.BpkDate)) >= (select RemDate from @t_rem where RemCode = 'REMSTNKEXT')
		   and a.Outstanding = @Outstanding
	end
 end
GO

if object_id('uspfn_SyncCsCustomerVehicleView') is not null
	drop procedure uspfn_SyncCsCustomerVehicleView
GO
select 'Create SP uspfn_SyncCsCustomerVehicleView' info
GO
create procedure uspfn_SyncCsCustomerVehicleView
as
;with r as (
select a.CompanyCode
     , a.ChassisCode
	 , a.ChassisNo
	 , BranchCode = (select top 1 x.BranchCode from omTrSalesDODetail x, omTrSalesDO y
					  where x.CompanyCode = y.CompanyCode
					    and x.BranchCode = y.BranchCode
					    and x.DONo = y.DONo
					    and x.CompanyCode = a.CompanyCode
					    and x.ChassisCode = a.ChassisCode
						and x.ChassisNo = a.ChassisNo
					  order by x.LastUpdateDate desc)
	 , DoNo = (select top 1 x.DONo from omTrSalesDODetail x, omTrSalesDO y
					  where x.CompanyCode = y.CompanyCode
					    and x.BranchCode = y.BranchCode
					    and x.DONo = y.DONo
					    and x.CompanyCode = a.CompanyCode
					    and x.ChassisCode = a.ChassisCode
						and x.ChassisNo = a.ChassisNo
					  order by x.LastUpdateDate desc)
	 , DoSeq = (select top 1 x.DOSeq from omTrSalesDODetail x, omTrSalesDO y
					  where x.CompanyCode = y.CompanyCode
					    and x.BranchCode = y.BranchCode
					    and x.DONo = y.DONo
					    and x.CompanyCode = a.CompanyCode
					    and x.ChassisCode = a.ChassisCode
						and x.ChassisNo = a.ChassisNo
					  order by x.LastUpdateDate desc)
  from omTrSalesDODetail a
 where 1 = 1
   and (year(a.LastUpdateDate) = year(getdate()) or (year(a.LastUpdateDate) + 1) = year(getdate()))
 group by a.CompanyCode, a.ChassisCode, a.ChassisNo
),
s as (
select r.CompanyCode
	 , r.BranchCode
	 , c.CustomerCode
	 , r.ChassisCode + convert(varchar, r.ChassisNo) as Chassis
	 , b.EngineCode + convert(varchar, b.EngineNo) as Engine
	 , c.SONo
	 , c.DONo
	 , c.DODate
	 , BpkNo = isnull((select top 1 BPKNo from OmTrSalesBpk
	                    where CompanyCode = r.CompanyCode
						  and BranchCode = r.BranchCode
						  and DONo = r.DoNo
						  and SONo = c.SONo
						order by LastUpdateDate desc), '')
	 , SalesmanCode = isnull((select top 1 Salesman from omTrSalesSO
	                    where CompanyCode = r.CompanyCode
						  and BranchCode = r.BranchCode
						  and SONo = c.SONo
						order by LastUpdateDate desc), '')
     , b.SalesModelCode
     , b.SalesModelYear
     , b.ColourCode
	 , r.ChassisCode
	 , r.ChassisNo
  from r
  join omTrSalesDODetail b
    on b.CompanyCode = r.CompanyCode
   and b.BranchCode = r.BranchCode
   and b.DONo = r.DoNo
   and b.DOSeq = r.DoSeq
  join omTrSalesDO c
    on c.CompanyCode = b.CompanyCode
   and c.BranchCode = b.BranchCode
   and c.DONo = b.DONo
 where b.StatusBPK != '3'
),
t as (
select s.CompanyCode
     , s.BranchCode 
	 , s.CustomerCode
	 , s.Chassis
	 , s.Engine
	 , s.SONo
	 , s.DoNo
	 , s.DoDate
	 , s.BpkNo
     , s.SalesModelCode as CarType
     , s.ColourCode as Color
	 , s.SalesmanCode
	 , b.EmployeeName as SalesmanName
	 , c.PoliceRegNo
	 , s.DODate as DeliveryDate
     , s.SalesModelCode
     , s.SalesModelYear
     , s.ColourCode
	 , d.BpkDate
	 , e.isLeasing
	 , e.LeasingCo
	 , f.CustomerName as LeasingName
	 , e.Installment
  from s with (nolock, nowait)
  left join HrEmployee b
    on b.CompanyCode = s.CompanyCode
   and b.EmployeeID = s.SalesmanCode
  left join svMstCustomerVehicle c
    on c.CompanyCode = s.CompanyCode
   and c.ChassisCode = s.ChassisCode
   and c.ChassisNo = s.ChassisNo
  join omTrSalesBPK d
    on d.CompanyCode = s.CompanyCode
   and d.BranchCode = s.BranchCode
   and d.BpkNo = s.BpkNo
  join omTrSalesSO e
    on e.CompanyCode = s.CompanyCode
   and e.BranchCode = s.BranchCode
   and e.SONo = s.SONo
  left join gnMstCustomer f
    on f.CompanyCode = e.CompanyCode
   and f.CustomerCode = e.LeasingCo
 where isnull(d.Status, 3) != '3'
)
select * into #t1 from (select * from t)#

delete CsCustomerVehicleView
 where exists (
	select top 1 1 from #t1
	 where #t1.CompanyCode = CsCustomerVehicleView.CompanyCode
	   and #t1.BranchCode = CsCustomerVehicleView.BranchCode
	   and #t1.Chassis = CsCustomerVehicleView.Chassis
 )
insert into CsCustomerVehicleView (CompanyCode, BranchCode, CustomerCode, Chassis, Engine, SONo, DONo, DoDate, BpkNo, CarType, Color, SalesmanCode, SalesmanName, PoliceRegNo, DeliveryDate, SalesModelCode, SalesModelYear, ColourCode, BpkDate, IsLeasing, LeasingCo, LeasingName, Installment)
select * from #t1

drop table #t1
GO
if object_id('uspfn_SyncCsCustomerView') is not null
	drop procedure uspfn_SyncCsCustomerView
GO
create procedure uspfn_SyncCsCustomerView
as
begin
	;with x as (
	select a.CompanyCode
		 , BranchCode = isnull((
			select top 1 BranchCode from OmTrSalesSo
			 where CompanyCode = a.CompanyCode
			  and CustomerCode = a.CustomerCode
			order by SODate desc), '')
		 , a.CustomerCode
		 , a.CustomerName
		 , a.CustomerType
		 , rtrim(a.Address1) + ' ' + rtrim(a.Address2) + rtrim(a.Address3) as Address
		 , a.PhoneNo
		 , a.HPNo
		 , b.AddPhone1
		 , b.AddPhone2
		 , a.BirthDate
		 , b.ReligionCode
		 , a.CreatedDate
		 , a.LastUpdateDate
	  from GnMstCustomer a
	  left join CsCustData b
		on b.CompanyCode = a.CompanyCode
	   and b.CustomerCode = a.CustomerCode
	 where 1 = 1
	   and a.CustomerType = 'I'
	   and a.BirthDate is not null
	   and a.BirthDate > '1900-01-01'
	   and (year(getdate() - year(a.BirthDate))) > 5
	   and year(a.LastUpdateDate) = year(getdate())
	)
	select * into #t1 from (select * from x where BranchCode != '')#

	delete CsCustomerView
	 where exists (
		select top 1 1 from #t1
		 where #t1.CompanyCode = CsCustomerView.CompanyCode
		   and #t1.BranchCode = CsCustomerView.BranchCode
		   and #t1.CustomerCode = CsCustomerView.CustomerCode
	 )
	insert into CsCustomerView (CompanyCode, BranchCode, CustomerCode, CustomerName, CustomerType, Address, PhoneNo, HPNo, AddPhone1, AddPhone2, BirthDate, ReligionCode, CreatedDate, LastUpdateDate)
	select * from #t1

	--drop table CsCustomerView
	--select * into CsCustomerView from #t1

	drop table #t1
end
GO
if object_id('uspfn_SyncCsCustomerViewInitialize') is not null
	drop procedure uspfn_SyncCsCustomerViewInitialize
GO
select 'Create SP uspfn_SyncCsCustomerViewInitialize' info
GO
create procedure uspfn_SyncCsCustomerViewInitialize
as
begin
	;with x as (
	select a.CompanyCode
		 , BranchCode = isnull((
			select top 1 BranchCode from OmTrSalesSo
			 where CompanyCode = a.CompanyCode
			  and CustomerCode = a.CustomerCode
			order by SODate desc), '')
		 , a.CustomerCode
		 , a.CustomerName
		 , a.CustomerType
		 , rtrim(a.Address1) + ' ' + rtrim(a.Address2) + rtrim(a.Address3) as Address
		 , a.PhoneNo
		 , a.HPNo
		 , b.AddPhone1
		 , b.AddPhone2
		 , a.BirthDate
		 , b.ReligionCode
		 , a.CreatedDate
		 , a.LastUpdateDate
	  from GnMstCustomer a
	  left join CsCustData b
		on b.CompanyCode = a.CompanyCode
	   and b.CustomerCode = a.CustomerCode
	 where 1 = 1
	   and a.CustomerType = 'I'
	   and a.BirthDate is not null
	   and a.BirthDate > '1900-01-01'
	   and (year(getdate() - year(a.BirthDate))) > 5
	   and year(a.LastUpdateDate) = year(getdate())
	)
	select * into #t1 from (select * from x where BranchCode != '')#

--	delete CsCustomerView
--	 where exists (
--		select top 1 1 from #t1
--		 where #t1.CompanyCode = CsCustomerView.CompanyCode
--		   and #t1.BranchCode = CsCustomerView.BranchCode
--		   and #t1.CustomerCode = CsCustomerView.CustomerCode
--	 )
--	insert into CsCustomerView (CompanyCode, BranchCode, CustomerCode, CustomerName, CustomerType, Address, PhoneNo, HPNo, AddPhone1, AddPhone2, BirthDate, ReligionCode, CreatedDate, LastUpdateDate)
--	select * from #t1

--	drop table CsCustomerView
	select * into CsCustomerView from #t1
	drop table #t1
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

var AlterTableByCmpWServer = function (cfg, callback)
{
        var params = {
            uri: config.api().downloadLink + 'api/synctablecs',
            method: "POST",
            form: {                    
                DealerCode: cfg.DealerCode
            }
        };	
		
		log("Get Alter Table Definition from server if available");
        request(params, function (e, r, bodyX) {
			if (e) {
				log("ERROR >> " + e);
				callback();
			}
			else {
				if (bodyX.length > 10) {
					var dataX = bodyX.substr(1, bodyX.length - 2);				
					var response = JSON.parse(dataX);  
					log("Your tables is out of date, please wait ... your tables will be altering by scheduler task service");
					
					async.each(response, installScript, function (err) {
						if (err)  log("ERROR >> " + err);
						log("Your tables has been modified");
						callback();
					});	
					
					function installScript(row, callback) {
						log("Name: " + row.Name);
						log("SQL: " + row.SQL);
						sqlODBC.query(cfg.ConnString, row.SQL, function (err, data) {    
							if (err) log("ERROR >> " + err);
							log("Modification for " + row.Name + " has been executed");
							
							var sql2 = SQLCheck.replace('#TABLENAME#', row.Name).replace('#TABLENAME#', row.Name);
							
							sqlODBC.query(cfg.ConnString, sql2, function (err2, data2) {    
								if (err2) log("ERROR >> " + err2);
								
								if (data2) {
									var ParamFeeedback = {
										DealerCode : CurrentDealerCode,
										TableName : row.Name,
										Cols: data2[0].total,
										Colsinfo: data2[0].list
									}
									var url = config.api().downloadLink + 'api/synctablelogger';
									request.post(url, {form:ParamFeeedback}, function (e, r, body){
										if (e) console.log(e);	
										console.log(body);
										callback();
									});		
								} else	
									callback();
							});
						});														
					}		
					
				} else {
					log("Your tables is up to date");
					callback();
				}
			}			
        });
}

var installScriptSQLfromList = function (cfg, callback)
{
	log("Install script to update view and store procedure");
			
	var xSQL= [], sql = SQL.split('\nGO');	
	
	sql.forEach(ExecuteSQL);
	
	function ExecuteSQL(s){
		xSQL.push(function(err, callback){
			sqlODBC.query(cfg.ConnString, s , function (err, data) {  
				var result = "";
				if (err) { 
					log("ERROR >> " + err);
				} else {
					if (data && data.length == 1) {
						if (data[0].info !== undefined) 
						{
							result = data[0].info
							log(data[0].info);
						}
					}
				}
				if(callback) callback(err,result); 
			});		
		});
	}
	
	async.series(xSQL, function (err, docs) {
		if (err) log("ERROR >> " + err);			 
		callback();
	});
															
	
}

var start = function (cfg, callback) 
{

    log("Starting " + TaskName + " for " + CurrentDealerCode); 

	AlterTableByCmpWServer(cfg, function(Err){
		if (Err) log("ERROR: " + Err);
		installScriptSQLfromList(cfg, function(err){
			if (err) log("ERROR: " + err);
			var url = config.api().downloadLink + 'uploadtasklog';
			var file = path.join(__dirname,  TaskNo + "-" + TaskName + ".log");
			var req = request.post(url, function (e, r, body) {
				if (e) console.log("ERROR", e);
				else {
					if (body !== undefined) {
						fs.unlinkSync(file);
					}
					console.log(body);
				}
				if (callback) callback();
			});		
						
			var form = req.form();
			form.append("DealerCode", CurrentDealerCode);
			form.append("UploadCode", "UPLCD");
			form.append('file', fs.createReadStream(file));
		});
	});
}

function log(info) {
    var file = path.join(__dirname,  TaskNo + "-" + TaskName + ".log");
    fs.appendFileSync(file, moment().format("YYYY-MM-DD HH:mm:ss") + " : " + info + "\n");
    console.log(moment().format("YYYY-MM-DD HH:mm:ss"), ":", info );
}

startTasks();