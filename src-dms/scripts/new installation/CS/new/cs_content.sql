


-- Delete CsSettings content
delete CsSettings;

declare @CompanyCode varchar(25);
set @CompanyCode = ( select top 1 CompanyCode from gnMstOrganizationHdr )

-- Insert CsSettings new content
insert into CsSettings (
			CompanyCode
		  , SettingCode
		  , SettingDesc
		  , SettingParam1
		  , SettingParam2
		  , SettingParam3
		  , SettingParam4
		  , SettingParam5
		  , SettingLink1
		  , SettingLink2
		  , SettingLink3
		  , IsDeleted
		  , CreatedBy
		  , CreatedDate
		  , UpdatedBy
		  , UpdatedDate 
          )
values    (@CompanyCode, N'REM3DAYSCALL', N'REMINDER 3 DAYS CALL', N'2', N'MONTH', N'FULL', N'M - 2', NULL, N'3DaysCall', NULL, NULL, NULL, NULL, NULL, NULL, NULL),
          (@CompanyCode, N'REMBDAYS', N'REMINDER BIRTHDAY CALL', N'1', N'MONTH', N'FULL', N'M - 1', NULL, N'BDayCall', NULL, NULL, NULL, NULL, NULL, NULL, NULL),
          (@CompanyCode, N'REMBPKB', N'REMINDER BPKB', N'2', N'MONTH', N'ACTUAL', N'CURRENT DATE', NULL, N'BpkbRemind', NULL, NULL, NULL, NULL, NULL, NULL, NULL),
          (@CompanyCode, N'REMHOLIDAYS', N'REMINDER HOLIDAYS', N'1', N'MONTH', N'FULL', N'M - 1', NULL, N'Holiays', NULL, NULL, NULL, NULL, NULL, NULL, NULL),
          (@CompanyCode, N'REMSTNKEXT', N'REMINDER STNK EXTENSION', N'1', N'MONTH', N'FULL', N'M - 1', NULL, N'StnkExt', NULL, NULL, NULL, NULL, NULL, NULL, NULL)

-- Show CsSettings content
select * from CsSettings




--