declare @CompanyCode varchar(25);
declare @LastSendDate datetime;
declare @CurrentTime datetime;

set @CompanyCode='6006406'
set @CurrentTime=getdate();
set @LastSendDate='2013-01-01';

insert into 
	   GnMstScheduleData (UniqueID, CompanyCode, DataType, Segment, Data, LastSendDate, Status, CreatedDate, UpdatedDate)
values (newid(), @CompanyCode, 'TRSLSINVIN', 0, null, @LastSendDate, 'A', @CurrentTime, @CurrentTime)
     , (newid(), @CompanyCode, 'TRSLSINV', 0, null, @LastSendDate, 'A', @CurrentTime, @CurrentTime)
     , (newid(), @CompanyCode, 'MSTCUST', 0, null, @LastSendDate, 'A', @CurrentTime, @CurrentTime)
     , (newid(), @CompanyCode, 'TRSLSDODTL', 0, null, @LastSendDate, 'A', @CurrentTime, @CurrentTime)
     , (newid(), @CompanyCode, 'TRSLSDO', 0, null, @LastSendDate, 'A', @CurrentTime, @CurrentTime)
     , (newid(), @CompanyCode, 'SVMSTCVHCL', 0, null, @LastSendDate, 'A', @CurrentTime, @CurrentTime)
     , (newid(), @CompanyCode, 'TRSLSBPK', 0, null, @LastSendDate, 'A', @CurrentTime, @CurrentTime)
     , (newid(), @CompanyCode, 'TRSLSSO', 0, null, @LastSendDate, 'A', @CurrentTime, @CurrentTime)
