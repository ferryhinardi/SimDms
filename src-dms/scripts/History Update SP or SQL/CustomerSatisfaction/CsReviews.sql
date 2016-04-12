--PROCEED WITH CAUTION (JGN DI DROP BILA SUDAH ADA DATA)
--if (object_id('CsReviews') is not null)
----drop table CsReviews
--go

if (object_id('CsReviews') is null)
begin
create table CsReviews
(
	[CompanyCode] [varchar](25) NOT NULL,
	[BranchCode] [varchar](25) NULL,
	[EmployeeID] [nvarchar](15) NOT NULL,
	[DateFrom] [datetime] NOT NULL,
	[DateTo] [datetime] NOT NULL,
	[Plan] [nvarchar](50) NULL,
	[Do] [nvarchar](50) NULL,
	[Check] [nvarchar](300) NULL,
	[Action] [nvarchar](300) NULL,
	[PIC] [nvarchar](15) NULL,
	[CommentbyGM] [nvarchar](300) NULL,
	[CommentbySIS] [nvarchar](300) NULL,
	[CreatedBy] [nvarchar](15) NULL,
	[CreatedDate] [datetime] NULL,
	[LastupdateBy] [nvarchar](15) NULL,
	[LastupdateDate] [datetime] NULL,
	[isDeleted] [int] NULL,
	primary key (CompanyCode, EmployeeID, DateFrom, DateTo)
)
end
GO

alter table cssettings
alter column SettingLink3 nvarchar(200)

GO

UPDATE CsSettings
SET settinglink2 = '3 Days Call', SettingLink3 = '3 Days Call Monitoring'
where SettingCode = 'REM3DAYSCALL'

UPDATE CsSettings
SET settinglink2 = 'BirthDay Call', SettingLink3 = 'BirthDay Call Monitoring'
where SettingCode = 'REMBDAYSCALL'

UPDATE CsSettings
SET settinglink2 = 'STNK Extension', SettingLink3 = 'STNK Extension Monitoring'
where SettingCode = 'REMSTNKEXT'

UPDATE CsSettings
SET settinglink2 = 'BPKB Reminder', SettingLink3 = 'BPKB Reminder Monitoring'
where SettingCode = 'REMBPKB'

GO
