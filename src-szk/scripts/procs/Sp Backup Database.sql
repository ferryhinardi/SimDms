

go
if object_id('uspfn_SPGetBackupFileName') is not null
	drop function uspfn_SPGetBackupFileName

go
create function uspfn_SPGetBackupFileName (
		@DatabaseName varchar(100)
	)
	returns varchar(1000)
as
begin
	declare @CurrentTime datetime;
	declare @Year varchar(4);
	declare @Month varchar(5);
	declare @Day varchar(2);
	
	set @CurrentTime = getdate();
	set @Year = convert(varchar(4), datepart(year, @CurrentTime));
	set @Month = right(convert(varchar(5), replicate('0', 2) + convert(varchar(2), datepart(month, @CurrentTime))), 2);
	set @Day = right(convert(varchar(5), replicate('0', 2) + convert(varchar(2), datepart(day, @CurrentTime))), 2);

	return 'D:\Backup\Database\' 
		   + @DatabaseName + '_' 
		   + @Year + '_' 
		   + @Month + '_' 
		   + @Day + '.bak' ;
end




go
if object_id('uspfn_SPBackupDatabaseSimDms') is not null
	drop procedure uspfn_SPBackupDatabaseSimDms

go
create procedure uspfn_SPBackupDatabaseSimDms
as
begin
	declare @FileName varchar(200);
	set @FileName = (select dbo.uspfn_SPGetBackupFileName('SimDms'));

	select @FileName as FileName;

	backup database SimDms
		to disk = @FileName
		with format, compression,
			medianame = 'ServerDBBackup',
			name = 'Full database backup SimDms'
end



go
if object_id('uspfn_SPBackupDatabaseSimDmsData') is not null
	drop procedure uspfn_SPBackupDatabaseSimDmsData

go
create procedure uspfn_SPBackupDatabaseSimDmsData
as
begin
	declare @FileName varchar(200);
	set @FileName = (select dbo.uspfn_SPGetBackupFileName('SimDmsData'));

	select @FileName as FileName;

	backup database SimDmsData
		to disk = @FileName
		with format, compression,
			medianame = 'ServerDBBackup',
			name = 'Full database backup SimDmsData'
end






go
if object_id('uspfn_SPBackupDatabaseSMarketShare') is not null
	drop procedure uspfn_SPBackupDatabaseSMarketShare

go
create procedure uspfn_SPBackupDatabaseSMarketShare
as
begin
	declare @FileName varchar(200);
	set @FileName = (select dbo.uspfn_SPGetBackupFileName('SMarketShare'));

	select @FileName as FileName;

	backup database SMarketShare
		to disk = @FileName
		with format, compression,
			medianame = 'ServerDBBackup',
			name = 'Full database backup SMarketShare'
end





go
if object_id('uspfn_SPBackupDatabaseSuzukiR2') is not null
	drop procedure uspfn_SPBackupDatabaseSuzukiR2

go
create procedure uspfn_SPBackupDatabaseSuzukiR2
as
begin
	declare @FileName varchar(200);
	set @FileName = (select dbo.uspfn_SPGetBackupFileName('SuzukiR2'));

	select @FileName as FileName;

	backup database SuzukiR2
		to disk = @FileName
		with format, compression,
			medianame = 'ServerDBBackup',
			name = 'Full database backup SuzukiR2'
end





go
if object_id('uspfn_SPBackupDatabaseSuzukiR4') is not null
	drop procedure uspfn_SPBackupDatabaseSuzukiR4

go
create procedure uspfn_SPBackupDatabaseSuzukiR4
as
begin
	declare @FileName varchar(200);
	set @FileName = (select dbo.uspfn_SPGetBackupFileName('SuzukiR4'));

	select @FileName as FileName;

	backup database SuzukiR4
		to disk = @FileName
		with format, compression,
			medianame = 'ServerDBBackup',
			name = 'Full database backup SuzukiR4'
end




go
if object_id('uspfn_SPBackupDatabaseSdmsLink') is not null
	drop procedure uspfn_SPBackupDatabaseSdmsLink

go
create procedure uspfn_SPBackupDatabaseSdmsLink
as
begin
	declare @FileName varchar(200);
	set @FileName = (select dbo.uspfn_SPGetBackupFileName('SdmsLink'));

	select @FileName as FileName;
	
	backup database SdmsLink
		to disk = @FileName
		with format, compression,
			medianame = 'ServerDBBackup',
			name = 'Full database backup SdmsLink'
end




go
if object_id('uspfn_SPBackupDatabaseSimDmsDocument') is not null
	drop procedure uspfn_SPBackupDatabaseSimDmsDocument

go
create procedure uspfn_SPBackupDatabaseSimDmsDocument
as
begin
	declare @FileName varchar(200);
	set @FileName = (select dbo.uspfn_SPGetBackupFileName('SimDmsDocument'));

	select @FileName as FileName;

	backup database SimDmsDocument
		to disk = @FileName
		with format, compression,
			medianame = 'ServerDBBackup',
			name = 'Full database backup SimDmsDocument'
end





go
if object_id('uspfn_SPBackupDatabaseSdms_Documentation') is not null
	drop procedure uspfn_SPBackupDatabaseSdms_Documentation

go
create procedure uspfn_SPBackupDatabaseSdms_Documentation
as
begin
	declare @FileName varchar(200);
	set @FileName = (select dbo.uspfn_SPGetBackupFileName('sdms_documentation'));

	select @FileName as FileName;

	backup database sdms_documentation
		to disk = @FileName
		with format, compression,
			medianame = 'ServerDBBackup',
			name = 'Full database backup sdms_document'
end




go
if object_id('uspfn_SPBackupDatabaseSdmsCis') is not null
	drop procedure uspfn_SPBackupDatabaseSdmsCis

go
create procedure uspfn_SPBackupDatabaseSdmsCis
as
begin
	declare @FileName varchar(200);
	set @FileName = (select dbo.uspfn_SPGetBackupFileName('SdmsCis'));

	select @FileName as FileName;

	backup database SdmsCis
		to disk = @FileName
		with format, compression,
			medianame = 'ServerDBBackup',
			name = 'Full database backup SdmsCis'
end





go
if object_id('uspfn_SPBackupDatabaseSdmsCisDoc') is not null
	drop procedure uspfn_SPBackupDatabaseSdmsCisDoc

go
create procedure uspfn_SPBackupDatabaseSdmsCisDoc
as
begin
	declare @FileName varchar(200);
	set @FileName = (select dbo.uspfn_SPGetBackupFileName('SdmsCisDoc'));

	select @FileName as FileName;

	backup database SdmsCisDoc
		to disk = @FileName
		with format, compression,
			medianame = 'ServerDBBackup',
			name = 'Full database backup SdmsCisDoc'
end




