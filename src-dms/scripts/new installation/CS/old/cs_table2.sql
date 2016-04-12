CREATE TABLE CsSettings(
	CompanyCode varchar(20) NOT NULL,
	SettingCode varchar(20) NOT NULL,
	SettingDesc varchar(250) NULL,
	SettingParam1 varchar(20) NULL,
	SettingParam2 varchar(20) NULL,
	SettingParam3 varchar(20) NULL,
	SettingParam4 varchar(20) NULL,
	SettingParam5 varchar(20) NULL,
	SettingLink1 varchar(20) NULL,
	SettingLink2 varchar(20) NULL,
	SettingLink3 varchar(20) NULL,
	IsDeleted bit NULL,
	CreatedBy varchar(36) NULL,
	CreatedDate datetime NULL,
	UpdatedBy varchar(36) NULL,
	UpdatedDate datetime NULL,
 CONSTRAINT PK_CsSettings PRIMARY KEY CLUSTERED 
(
	CompanyCode ASC,
	SettingCode ASC
)
) 

GO

SET ANSI_PADDING OFF
GO
