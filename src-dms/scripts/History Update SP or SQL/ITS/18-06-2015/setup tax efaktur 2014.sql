

if not exists (Select * from SysMenuDms where menuid = 'GnRpSkemaTaxOut')
begin 
       INSERT INTO [dbo].[SysMenuDms]
                        ([MenuId]
                        ,[MenuCaption]
                        ,[MenuHeader]
                        ,[MenuIndex]
                        ,[MenuLevel]
                        ,[MenuUrl]
                        ,[MenuIcon])
               VALUES
                        ('GnRpSkemaTaxOut'
                        ,'Laporan eFaktur Pajak Keluaran'
                        ,'taxReport'
                        ,'7'
                        ,'2'
                        ,'report/skemataxout'
                        ,'')
if not exists (Select * from SysModule where ModuleID = 'tax')
insert SysModule
values ('tax','Tax','8','','1','1', null)
                        
if not exists (Select * from SysRoleModule where roleid='Admin' and ModuleID = 'tax')
insert SysRoleModule
values ('ADMIN','tax')

if not exists (Select * from SysMenuDms where menuid = 'taxReport')
INSERT INTO [dbo].[SysMenuDms]
                        ([MenuId]
                        ,[MenuCaption]
                        ,[MenuHeader]
                        ,[MenuIndex]
                        ,[MenuLevel]
                        ,[MenuUrl]
                        ,[MenuIcon])
               VALUES
                        ('taxReport'
                        ,'Reports'
                        ,'tax'
                        ,'3'
                        ,'1'
                        , NULL
                        , NULL)
                        
if not exists (Select * from sysrolemenu where roleid='Admin' and menuid = 'taxReport')
insert sysrolemenu
values ('ADMIN','taxReport') 

if not exists (Select * from sysrolemenu where roleid='Admin' and menuid = 'GnRpSkemaTaxOut')
insert sysrolemenu
values ('ADMIN','GnRpSkemaTaxOut')


end