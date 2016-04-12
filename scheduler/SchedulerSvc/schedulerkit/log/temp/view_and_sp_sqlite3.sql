CREATE VIEW [dbo].PmSalesGradeHistoryView
AS
SELECT [CompanyCode]
      ,[BranchCode]
      ,[EmployeeID]
      ,[TransactionID]
      ,[Grade]
	  ,CASE ISDATE(substring([AssignDate],1,11)) WHEN 1 THEN
       CONVERT(datetime, substring([AssignDate],1,11)) END [AssignDate]
      ,[AssignBy]
FROM [pmSlsGradeHistory]
GO

CREATE PROCEDURE [dbo].[uspfn_CreateSQLiteTable]
@TableName varchar(100)
AS

declare @name varchar(100), @typeId INT -- u/ parsing parameter value
declare @sqlCreateTable varchar(5000) -- u/ DDL
declare @sqlInsertTableD varchar(5000) -- u/ DML -- table/field definition
declare @sqlInsertTableV varchar(5000) -- u/ DML -- value definition
declare @sqlColumnList varchar(5000) -- u/ DML -- value definition

declare genTableScript cursor for 
SELECT name, system_type_id  FROM sys.columns  WHERE [object_id] = OBJECT_ID(@TableName)

select  @sqlCreateTable='CREATE TABLE IF NOT EXISTS ' + @TableName + ' ( ',
		@sqlInsertTableD='INSERT INTO '  + @TableName + ' ( ',
		@sqlInsertTableV='', @sqlColumnList = ''

OPEN genTableScript
FETCH NEXT FROM genTableScript
INTO @name, @typeId

WHILE @@FETCH_STATUS = 0
BEGIN

	select @sqlCreateTable = @sqlCreateTable + @name + ' ' + case when @typeId IN (34,173) then 'BLOB '
		when @typeId in (40, 41, 42,58, 61) then 'DATETIME '
		when @typeId in (48, 52, 56, 127 ) then 'BIGINT '
		when @typeId = 104 then 'BOOL '
		when @typeId in (59,60,62,106,108,122) then 'FLOAT ' else 'TEXT ' end,
		@sqlInsertTableD = @sqlInsertTableD + @name,
		@sqlInsertTableV = @sqlInsertTableV + ' ?', @sqlColumnList = @sqlColumnList + @name

	FETCH NEXT FROM genTableScript
	INTO @name, @typeId

	IF @@FETCH_STATUS = 0
	BEGIN
		select @sqlCreateTable = @sqlCreateTable + ', ', @sqlInsertTableD = @sqlInsertTableD + ', ', @sqlInsertTableV = @sqlInsertTableV + ', ', @sqlColumnList = @sqlColumnList + ', '
	END

END

select @sqlCreateTable = @sqlCreateTable + ') ' , @sqlInsertTableD = @sqlInsertTableD + ') VALUES ( ' + @sqlInsertTableV + ') '

CLOSE genTableScript
DEALLOCATE genTableScript

SELECT @sqlCreateTable as DDL, @sqlInsertTableD as DML, @sqlColumnList as ColumnList