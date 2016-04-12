update CsStnkExt set StnkExpiredDate = '2000-01-01' where StnkExpiredDate is null
go


declare @SQL nvarchar(4000)
set @SQL = 'alter table csstnkext drop constraint |ConstraintName|'

SET @SQL = REPLACE(@SQL, '|ConstraintName|', (select name 
												from sysobjects
											   where xtype = 'PK'
											     and parent_obj = object_id('csstnkext')
											  ))

EXEC(@SQL)
go

alter table csstnkext
alter column StnkExpiredDate datetime not null

go

alter table csstnkext
add constraint PK_CsStnkExt_1 primary key clustered (CompanyCode, CustomerCode, Chassis, StnkExpiredDate)
go