DELETE FROM SysMenuDms WHERE menuid in ('inqsumm','itsclnupkdp','itsinpkdp','itsinq','itsinqbyperiode','itsinqfollowup','itsinqits','itsinqitsmkt','itsinqitsstatus','itsinqlostcase','itsinqoutstandingprospek','itsinqprod','itsinqsalesachievement','ItsInqSisHistory','itsmaster','itsmstorganization','itsmstOutlets','itsmstteammember','itstrans','itsutility','itsutlgenkdp','itsutltransferkdp','itsutluploadfile')
GO

INSERT INTO [SysMenuDms]([MenuId],[MenuCaption],[MenuHeader],[MenuIndex],[MenuLevel],[MenuUrl],[MenuIcon]) VALUES ('inqsumm','Inquiry Summary','itsinq',3,2,'inquiry/summary',NULL);
INSERT INTO [SysMenuDms]([MenuId],[MenuCaption],[MenuHeader],[MenuIndex],[MenuLevel],[MenuUrl],[MenuIcon]) VALUES ('itsclnupkdp','Clean Up ITS','itstrans',2,2,'trans/clnupkdp',NULL);
INSERT INTO [SysMenuDms]([MenuId],[MenuCaption],[MenuHeader],[MenuIndex],[MenuLevel],[MenuUrl],[MenuIcon]) VALUES ('itsinpkdp','Input KDP','itstrans',3,2,'trans/inputkdp',NULL);
INSERT INTO [SysMenuDms]([MenuId],[MenuCaption],[MenuHeader],[MenuIndex],[MenuLevel],[MenuUrl],[MenuIcon]) VALUES ('itsinq','Inquiry Prospect','its',2,1,'',NULL);
INSERT INTO [SysMenuDms]([MenuId],[MenuCaption],[MenuHeader],[MenuIndex],[MenuLevel],[MenuUrl],[MenuIcon]) VALUES ('itsinqbyperiode','Inquiry By Periode','itsinq',5,2,'inquiry/periode',NULL);
INSERT INTO [SysMenuDms]([MenuId],[MenuCaption],[MenuHeader],[MenuIndex],[MenuLevel],[MenuUrl],[MenuIcon]) VALUES ('itsinqfollowup','Inquiry Follow Up','itsinq',0,2,'inquiry/followup',NULL);
INSERT INTO [SysMenuDms]([MenuId],[MenuCaption],[MenuHeader],[MenuIndex],[MenuLevel],[MenuUrl],[MenuIcon]) VALUES ('itsinqits','Inquiry ITS','itsinq',6,2,'inquiry/InqIts',NULL);
INSERT INTO [SysMenuDms]([MenuId],[MenuCaption],[MenuHeader],[MenuIndex],[MenuLevel],[MenuUrl],[MenuIcon]) VALUES ('itsinqitsmkt','Inquiry ITS (Management)','itsinq',7,2,'inquiry/inqitsmkt',NULL);
INSERT INTO [SysMenuDms]([MenuId],[MenuCaption],[MenuHeader],[MenuIndex],[MenuLevel],[MenuUrl],[MenuIcon]) VALUES ('itsinqitsstatus','Inquiry ITS With Status','itsinq',8,2,'inquiry/inqitsstatus',NULL);
INSERT INTO [SysMenuDms]([MenuId],[MenuCaption],[MenuHeader],[MenuIndex],[MenuLevel],[MenuUrl],[MenuIcon]) VALUES ('itsinqlostcase','Inquiry Analisa Lost Case','itsinq',2,2,'inquiry/lostcase',NULL);
INSERT INTO [SysMenuDms]([MenuId],[MenuCaption],[MenuHeader],[MenuIndex],[MenuLevel],[MenuUrl],[MenuIcon]) VALUES ('itsinqoutstandingprospek','inquiry Outstanding Prospek','itsinq',4,2,'inquiry/outstandingprospek',NULL);
INSERT INTO [SysMenuDms]([MenuId],[MenuCaption],[MenuHeader],[MenuIndex],[MenuLevel],[MenuUrl],[MenuIcon]) VALUES ('itsinqprod','Inquiry Productivity','itsinq',1,2,'report/inqprod',NULL);
INSERT INTO [SysMenuDms]([MenuId],[MenuCaption],[MenuHeader],[MenuIndex],[MenuLevel],[MenuUrl],[MenuIcon]) VALUES ('itsinqsalesachievement','Inquiry Sales Achievement','itsinq',3,2,'inquiry/salesachievement',NULL);
INSERT INTO [SysMenuDms]([MenuId],[MenuCaption],[MenuHeader],[MenuIndex],[MenuLevel],[MenuUrl],[MenuIcon]) VALUES ('ItsInqSisHistory','SIS History','itsinq',5,2,'inquiry/sishistory',NULL);
INSERT INTO [SysMenuDms]([MenuId],[MenuCaption],[MenuHeader],[MenuIndex],[MenuLevel],[MenuUrl],[MenuIcon]) VALUES ('itsmaster','Master Prospect','its',0,1,'NULL',NULL);
INSERT INTO [SysMenuDms]([MenuId],[MenuCaption],[MenuHeader],[MenuIndex],[MenuLevel],[MenuUrl],[MenuIcon]) VALUES ('itsmstorganization','Organization','itsmaster',5,2,'master/organization',NULL);
INSERT INTO [SysMenuDms]([MenuId],[MenuCaption],[MenuHeader],[MenuIndex],[MenuLevel],[MenuUrl],[MenuIcon]) VALUES ('itsmstOutlets','Outlets','itsmaster',2,2,'master/outlets',NULL);
INSERT INTO [SysMenuDms]([MenuId],[MenuCaption],[MenuHeader],[MenuIndex],[MenuLevel],[MenuUrl],[MenuIcon]) VALUES ('itsmstteammember','Team Members','itsmaster',1,2,'master/teammember',NULL);
INSERT INTO [SysMenuDms]([MenuId],[MenuCaption],[MenuHeader],[MenuIndex],[MenuLevel],[MenuUrl],[MenuIcon]) VALUES ('itstrans','Transaction','its',1,1,'NULL',NULL);
INSERT INTO [SysMenuDms]([MenuId],[MenuCaption],[MenuHeader],[MenuIndex],[MenuLevel],[MenuUrl],[MenuIcon]) VALUES ('itsutility','Utility','its',3,1,'NULL',NULL);
INSERT INTO [SysMenuDms]([MenuId],[MenuCaption],[MenuHeader],[MenuIndex],[MenuLevel],[MenuUrl],[MenuIcon]) VALUES ('itsutlgenkdp','Generate KDP','itsutility',0,2,'utility/generatekdp',NULL);
INSERT INTO [SysMenuDms]([MenuId],[MenuCaption],[MenuHeader],[MenuIndex],[MenuLevel],[MenuUrl],[MenuIcon]) VALUES ('itsutltransferkdp','Transfer KDP','itsutility',3,2,'utility/transferkdp',NULL);
INSERT INTO [SysMenuDms]([MenuId],[MenuCaption],[MenuHeader],[MenuIndex],[MenuLevel],[MenuUrl],[MenuIcon]) VALUES ('itsutluploadfile','Upload File','itsutility',1,2,'utility/uploadfile',NULL);
GO

IF NOT EXISTS(select 1 from sysrole where roleid='ITS')
BEGIN
	INSERT INTO [dbo].[sysRole] ([RoleId],[RoleName],[Themes],[IsActive],[IsAdmin],[IsChangeBranchCode]) VALUES ('ITS','ITS','ITS',1,0,0)
END

IF NOT EXISTS(select 1 from [SysModule] where [ModuleId]='ITS')
BEGIN
	INSERT INTO [SysModule]([ModuleId],[ModuleCaption],[ModuleIndex],[ModuleUrl],[InternalLink],[IsPublish],[Icon])
	VALUES('its',	'Inquiry Tracking System',2,'',1,1,'fa fa-lg fa-fw fa-search')
END

IF NOT EXISTS(select 1 from [SysRoleModule] where RoleID='ITS' and [ModuleId]='ITS')
BEGIN
	insert into [SysRoleModule] values ('ITS','its')
END

delete from sysrolemenu where roleid='ITS'
GO

insert into sysRoleMenu (RoleId, MenuId) 
select 'ITS', Menuid from sysmenudms
where menuid in ('inqsumm','itsclnupkdp','itsinpkdp','itsinq','itsinqbyperiode','itsinqfollowup','itsinqits','itsinqitsmkt','itsinqitsstatus','itsinqlostcase','itsinqoutstandingprospek','itsinqprod','itsinqsalesachievement','ItsInqSisHistory','itsmaster','itsmstorganization','itsmstOutlets','itsmstteammember','itstrans','itsutility','itsutlgenkdp','itsutltransferkdp','itsutluploadfile')
GO


