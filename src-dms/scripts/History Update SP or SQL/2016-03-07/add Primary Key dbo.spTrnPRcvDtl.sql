DECLARE @PKName varchar(50)
DECLARE @IndexName varchar(50)
DECLARE @DropIndex varchar(Max)
DECLARE @AlterTable varchar(max)

SELECT @PKName = (select OBJECT_NAME(OBJECT_ID)
FROM sys.objects
WHERE OBJECT_NAME(parent_object_id)='spTrnPRcvDtl'
and type = 'PK')

IF EXISTS (SELECT name FROM sys.indexes WHERE object_id = OBJECT_ID('dbo.spTrnPRcvDtl') and name like 'IX%')
BEGIN
SELECT @IndexName = (SELECT name FROM sys.indexes WHERE object_id = OBJECT_ID('dbo.spTrnPRcvDtl') and name like 'IX%')

SET @DropIndex = '
DROP INDEX dbo.spTrnPRcvDtl.'+@IndexName+'
'
END

SET @AlterTable ='

ALTER TABLE dbo.spTrnPRcvDtl DROP CONSTRAINT '+ @PKName +'

ALTER TABLE dbo.spTrnPRcvDtl ALTER COLUMN BoxNo varchar(20) NOT NULL

ALTER TABLE dbo.spTrnPRcvDtl 
ADD CONSTRAINT '+ @PKName +' PRIMARY KEY NONCLUSTERED (CompanyCode, BranchCode, WRSNo, PartNo, DocNo, BoxNo);
'

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE 
WHERE CONSTRAINT_NAME = @PKName and COLUMN_NAME = 'BoxNo')
BEGIN

EXEC (@DropIndex)
EXEC (@AlterTable)

END