--begin tran
if exists (select * from svMstRefferenceService where RefferenceType = 'JOBSTYPE' and (RefferenceCode like 'PHMN%' or RefferenceCode like 'FBSK%' or RefferenceCode like 'ESK%'))
delete from svMstRefferenceService where RefferenceType = 'JOBSTYPE' and (RefferenceCode like 'PHMN%' or RefferenceCode like 'FBSK%' or RefferenceCode like 'ESK%')
go
if exists (select * from svMstJob where (JobType like 'PHMN%' or JobType like 'FBSK%' or JobType like 'ESK%'))
delete from svMstJob where (JobType like 'PHMN%' or JobType like 'FBSK%' or JobType like 'ESK%')
go
if exists (select * from svMstTaskPart where (JobType like 'PHMN%' or JobType like 'FBSK%' or JobType like 'ESK%'))
delete from svMstTaskPart where (JobType like 'PHMN%' or JobType like 'FBSK%' or JobType like 'ESK%')
go
if exists (select * from svMstTask where (JobType like 'PHMN%' or JobType like 'FBSK%' or JobType like 'ESK%'))
delete from svMstTask where (JobType like 'PHMN%' or JobType like 'FBSK%' or JobType like 'ESK%')
go
if exists (select * from svMstTaskPrice where (JobType like 'PHMN%' or JobType like 'FBSK%' or JobType like 'ESK%'))
delete from svMstTaskPrice where (JobType like 'PHMN%' or JobType like 'FBSK%' or JobType like 'ESK%')
go

insert into svMstRefferenceService
select CompanyCode, ProductType, RefferenceType, 'PHMN_RTN 5000', Description + ' (PAKET HEMAT MOBIL NIAGA)', DescriptionEng + ' (PAKET HEMAT MOBIL NIAGA)', 1, 'ga', GETDATE(), 'ga', GETDATE(), 0, '', ''
from svMstRefferenceService where RefferenceType = 'JOBSTYPE' and RefferenceCode = 'PB5000'
insert into svMstRefferenceService
select CompanyCode, ProductType, RefferenceType, 'PHMN_RTN 20000', Description + ' (PAKET HEMAT MOBIL NIAGA)', DescriptionEng + ' (PAKET HEMAT MOBIL NIAGA)', 1, 'ga', GETDATE(), 'ga', GETDATE(), 0, '', ''
from svMstRefferenceService where RefferenceType = 'JOBSTYPE' and RefferenceCode = 'PB20000'
insert into svMstRefferenceService
select CompanyCode, ProductType, RefferenceType, 'PHMN_RTN 40000', Description + ' (PAKET HEMAT MOBIL NIAGA)', DescriptionEng + ' (PAKET HEMAT MOBIL NIAGA)', 1, 'ga', GETDATE(), 'ga', GETDATE(), 0, '', ''
from svMstRefferenceService where RefferenceType = 'JOBSTYPE' and RefferenceCode = 'PB40000'
insert into svMstRefferenceService
select CompanyCode, ProductType, RefferenceType, 'ESK_RTN 40000', Description + ' (ENGINE SERVICE KIT)', DescriptionEng + ' (ENGINE SERVICE KIT)', 1, 'ga', GETDATE(), 'ga', GETDATE(), 0, '', ''
from svMstRefferenceService where RefferenceType = 'JOBSTYPE' and RefferenceCode = 'PB40000'
insert into svMstRefferenceService
select CompanyCode, ProductType, RefferenceType, 'FBSK_RTN 40000', Description + ' (FRONT BRAKE SERVICE KIT)', DescriptionEng + ' (FRON BRAKE SERVICE KIT)', 1, 'ga', GETDATE(), 'ga', GETDATE(), 0, '', ''
from svMstRefferenceService where RefferenceType = 'JOBSTYPE' and RefferenceCode = 'PB40000'

insert into svMstRefferenceService
select CompanyCode, ProductType, RefferenceType, 'PHMN_FS 5000', Description + ' (PAKET HEMAT MOBIL NIAGA)', DescriptionEng + ' (PAKET HEMAT MOBIL NIAGA)', 1, 'ga', GETDATE(), 'ga', GETDATE(), 0, '', ''
from svMstRefferenceService where RefferenceType = 'JOBSTYPE' and RefferenceCode = 'FSC 5000'
insert into svMstRefferenceService
select CompanyCode, ProductType, RefferenceType, 'PHMN_FS 20000', Description + ' (PAKET HEMAT MOBIL NIAGA)', DescriptionEng + ' (PAKET HEMAT MOBIL NIAGA)', 1, 'ga', GETDATE(), 'ga', GETDATE(), 0, '', ''
from svMstRefferenceService where RefferenceType = 'JOBSTYPE' and RefferenceCode = 'FSC 20000'
insert into svMstRefferenceService
select CompanyCode, ProductType, RefferenceType, 'PHMN_FS 40000', Description + ' (PAKET HEMAT MOBIL NIAGA)', DescriptionEng + ' (PAKET HEMAT MOBIL NIAGA)', 1, 'ga', GETDATE(), 'ga', GETDATE(), 0, '', ''
from svMstRefferenceService where RefferenceType = 'JOBSTYPE' and RefferenceCode = 'FSC 40000'
insert into svMstRefferenceService
select CompanyCode, ProductType, RefferenceType, 'ESK_FS 40000', Description + ' (ENGINE SERVICE KIT)', DescriptionEng + ' (ENGINE SERVICE KIT)', 1, 'ga', GETDATE(), 'ga', GETDATE(), 0, '', ''
from svMstRefferenceService where RefferenceType = 'JOBSTYPE' and RefferenceCode = 'FSC 40000'
insert into svMstRefferenceService
select CompanyCode, ProductType, RefferenceType, 'FBSK_FS 40000', Description + ' (FRONT BRAKE SERVICE KIT)', DescriptionEng + ' (FRONT BRAKE SERVICE KIT)', 1, 'ga', GETDATE(), 'ga', GETDATE(), 0, '', ''
from svMstRefferenceService where RefferenceType = 'JOBSTYPE' and RefferenceCode = 'FSC 40000'

select * into #t1
from (select 'GC415-BV' BasicModel union select 'GC415-BX' union select 'GC415-CH' union select 'GC415-PU' union select 'SL415-2CH' 
union select 'SL415-2PU' union select 'SL415-PU' union select 'SL415CH' union select 'ST150-PUWD' union select 'JB420' union select 'JB424') #t1

declare @BasicModel as varchar(20)

declare myCursor cursor for
select BasicModel from #t1

open myCursor
fetch next from myCursor into @BasicModel

while @@FETCH_STATUS = 0
begin

 insert into svMstTaskPrice
select CompanyCode, BranchCode, ProductType, BasicModel, 'PHMN_RTN 5000', 'PHMN_RTN 5000', OperationHour, ClaimHour, LaborCost, LaborPrice
from svMstTaskPrice
where 1 = 1
and BasicModel  = @BasicModel
and JobType     = 'PB5000'
and OperationNo = 'PB5000'

 insert into svMstTaskPrice
select CompanyCode, BranchCode, ProductType, BasicModel, 'PHMN_RTN 20000', 'PHMN_RTN 20000', OperationHour, ClaimHour, LaborCost, LaborPrice
from svMstTaskPrice
where 1 = 1
and BasicModel  = @BasicModel
and JobType     = 'PB20000'
and OperationNo = 'PB 20000'

 insert into svMstTaskPrice
select CompanyCode, BranchCode, ProductType, BasicModel, 'PHMN_RTN 40000', 'PHMN_RTN 40000', OperationHour, ClaimHour, LaborCost, LaborPrice
from svMstTaskPrice
where 1 = 1
and BasicModel  = @BasicModel
and JobType     = 'PB40000'
and OperationNo = 'PB40000'

 insert into svMstTaskPrice
select CompanyCode, BranchCode, ProductType, BasicModel, 'ESK_RTN 40000', 'ESK_RTN 40000', OperationHour, ClaimHour, LaborCost, LaborPrice
from svMstTaskPrice
where 1 = 1
and BasicModel  = @BasicModel
and JobType     = 'PB40000'
and OperationNo = 'PB40000'

 insert into svMstTaskPrice
select CompanyCode, BranchCode, ProductType, BasicModel, 'FBSK_RTN 40000', 'FBSK_RTN 40000', OperationHour, ClaimHour, LaborCost, LaborPrice
from svMstTaskPrice
where 1 = 1
and BasicModel  = @BasicModel
and JobType     = 'PB40000'
and OperationNo = 'PB40000'

 insert into svMstTaskPrice
select CompanyCode, BranchCode, ProductType, BasicModel, 'PHMN_FS 5000', 'PHMN_FS 5000', OperationHour, ClaimHour, LaborCost, LaborPrice
from svMstTaskPrice
where 1 = 1
and BasicModel  = @BasicModel
and JobType     = 'FSC 5000'
and OperationNo = 'FSC 5000'

 insert into svMstTaskPrice
select CompanyCode, BranchCode, ProductType, BasicModel, 'PHMN_FS 20000', 'PHMN_FS 20000', OperationHour, ClaimHour, LaborCost, LaborPrice
from svMstTaskPrice
where 1 = 1
and BasicModel  = @BasicModel
and JobType     = 'FSC 20000'
and OperationNo = 'FSC 20000'

 insert into svMstTaskPrice
select CompanyCode, BranchCode, ProductType, BasicModel, 'PHMN_FS 40000', 'PHMN_FS 40000', OperationHour, ClaimHour, LaborCost, LaborPrice
from svMstTaskPrice
where 1 = 1
and BasicModel  = @BasicModel
and JobType     = 'FSC 40000'
and OperationNo = 'FSC 40000'

 insert into svMstTaskPrice
select CompanyCode, BranchCode, ProductType, BasicModel, 'ESK_FS 40000', 'ESK_FS 40000', OperationHour, ClaimHour, LaborCost, LaborPrice
from svMstTaskPrice
where 1 = 1
and BasicModel  = @BasicModel
and JobType     = 'FSC 40000'
and OperationNo = 'FSC 40000'

 insert into svMstTaskPrice
select CompanyCode, BranchCode, ProductType, BasicModel, 'FBSK_FS 40000', 'FBSK_FS 40000', OperationHour, ClaimHour, LaborCost, LaborPrice
from svMstTaskPrice
where 1 = 1
and BasicModel  = @BasicModel
and JobType     = 'FSC 40000'
and OperationNo = 'FSC 40000'

if @BasicModel like 'JB%'
begin
if @BasicModel = 'JB420'
begin
insert into svMstJob
select CompanyCode, ProductType, BasicModel, 'FBSK_RTN 40000', GroupJobType, IsPdiFsc, PdiFscSeq, WarrantyOdometer, WarrantyTimePeriod, WarrantyTimeDim, CounterOperationNo,ReceivableAccountNo,  1, 0,'', '', 'ga', GETDATE(), 'ga', GETDATE()
from svMstJob where JobType = 'PB40000' and BasicModel = @BasicModel
insert into svMstJob
select CompanyCode, ProductType, BasicModel, 'FBSK_FS 40000', GroupJobType, IsPdiFsc, PdiFscSeq, WarrantyOdometer, WarrantyTimePeriod, WarrantyTimeDim, CounterOperationNo,ReceivableAccountNo,  1, 0,'', '', 'ga', GETDATE(), 'ga', GETDATE()
from svMstJob where JobType = 'FSC 40000' and BasicModel = @BasicModel
insert into svMstTask
select CompanyCode, ProductType, BasicModel, 'FBSK_RTN 40000', 'FRONT BREAK CALIPER ASSY', 'OVER HAULL (FRONT BRAKE SERVICE KIT)', 1.9, 1.9, 0, 2.6 * LaborPrice, 'JB 420', 0, 0, 1, 'ga', GETDATE(), 'ga', GETDATE(), 0, '', '', 'C' 
from svMstTask where BasicModel = @BasicModel and JobType = 'PB40000'
insert into svMstTask
select CompanyCode, ProductType, BasicModel, 'FBSK_RTN 40000', 'DISC,FRONT BRAKE', 'REPLACE & REASSEMBLY (FRONT BRAKE SERVICE KIT)', 0.7, 0.7, 0, 2.6 * LaborPrice, 'JB 420', 0, 0, 1, 'ga', GETDATE(), 'ga', GETDATE(), 0, '', '', 'C' 
from svMstTask where BasicModel = @BasicModel and JobType = 'PB40000'
insert into svMstTask
select CompanyCode, ProductType, BasicModel, 'FBSK_FS 40000', 'FRONT BREAK CALIPER ASSY', 'OVER HAULL (FRONT BRAKE SERVICE KIT)', 1.9, 1.9, 0, 2.6 * LaborPrice, 'JB 420', 0, 0, 1, 'ga', GETDATE(), 'ga', GETDATE(), 0, '', '', 'C' 
from svMstTask where BasicModel = @BasicModel and JobType = 'FSC 40000'
insert into svMstTask
select CompanyCode, ProductType, BasicModel, 'FBSK_FS 40000', 'DISC,FRONT BRAKE', 'REPLACE & REASSEMBLY (FRONT BRAKE SERVICE KIT)', 0.7, 0.7, 0, 2.6 * LaborPrice, 'JB 420', 0, 0, 1, 'ga', GETDATE(), 'ga', GETDATE(), 0, '', '', 'C' 
from svMstTask where BasicModel = @BasicModel and JobType = 'FSC 40000'
end
else
begin
insert into svMstJob
select CompanyCode, ProductType, BasicModel, 'ESK_RTN 40000', GroupJobType, IsPdiFsc, PdiFscSeq, WarrantyOdometer, WarrantyTimePeriod, WarrantyTimeDim, CounterOperationNo, ReceivableAccountNo,  1, 0,'', '', 'ga', GETDATE(), 'ga', GETDATE()
from svMstJob where JobType = 'PB40000' and BasicModel = @BasicModel
insert into svMstJob
select CompanyCode, ProductType, BasicModel, 'ESK_FS 40000', GroupJobType, IsPdiFsc, PdiFscSeq, WarrantyOdometer, WarrantyTimePeriod, WarrantyTimeDim, CounterOperationNo,ReceivableAccountNo,  1, 0,'', '', 'ga', GETDATE(), 'ga', GETDATE()
from svMstJob where JobType = 'FSC 40000' and BasicModel = @BasicModel
insert into svMstJob
select CompanyCode, ProductType, BasicModel, 'FBSK_RTN 40000', GroupJobType, IsPdiFsc, PdiFscSeq, WarrantyOdometer, WarrantyTimePeriod, WarrantyTimeDim, CounterOperationNo,ReceivableAccountNo,  1, 0,'', '', 'ga', GETDATE(), 'ga', GETDATE()
from svMstJob where JobType = 'PB40000' and BasicModel = @BasicModel
insert into svMstJob
select CompanyCode, ProductType, BasicModel, 'FBSK_FS 40000', GroupJobType, IsPdiFsc, PdiFscSeq, WarrantyOdometer, WarrantyTimePeriod, WarrantyTimeDim, CounterOperationNo,ReceivableAccountNo,  1, 0,'', '', 'ga', GETDATE(), 'ga', GETDATE()
from svMstJob where JobType = 'FSC 40000' and BasicModel = @BasicModel

insert into svMstTask
select CompanyCode, ProductType, BasicModel, 'FBSK_RTN 40000', 'FRONT BREAK CALIPER ASSY', 'OVER HAULL (FRONT BRAKE SERVICE KIT)', 1.9, 1.9, 0, 2.6 * LaborPrice, 'JB 424', 0, 0, 1, 'ga', GETDATE(), 'ga', GETDATE(), 0, '', '', 'C' 
from svMstTask where BasicModel = @BasicModel and JobType = 'PB40000'
insert into svMstTask
select CompanyCode, ProductType, BasicModel, 'FBSK_RTN 40000', 'DISC,FRONT BRAKE', 'REPLACE & REASSEMBLY (FRONT BRAKE SERVICE KIT)', 0.7, 0.7, 0, 2.6 * LaborPrice, 'JB 424', 0, 0, 1, 'ga', GETDATE(), 'ga', GETDATE(), 0, '', '', 'C' 
from svMstTask where BasicModel = @BasicModel and JobType = 'PB40000'
insert into svMstTask
select CompanyCode, ProductType, BasicModel, 'FBSK_FS 40000', 'FRONT BREAK CALIPER ASSY', 'OVER HAULL (FRONT BRAKE SERVICE KIT)', 1.9, 1.9, 0, 2.6 * LaborPrice, 'JB 424', 0, 0, 1, 'ga', GETDATE(), 'ga', GETDATE(), 0, '', '', 'C' 
from svMstTask where BasicModel = @BasicModel and JobType = 'FSC 40000'
insert into svMstTask
select CompanyCode, ProductType, BasicModel, 'FBSK_FS 40000', 'DISC,FRONT BRAKE', 'REPLACE & REASSEMBLY (FRONT BRAKE SERVICE KIT)', 0.7, 0.7, 0, 2.6 * LaborPrice, 'JB 424', 0, 0, 1, 'ga', GETDATE(), 'ga', GETDATE(), 0, '', '', 'C' 
from svMstTask where BasicModel = @BasicModel and JobType = 'FSC 40000'

insert into svMstTask
select CompanyCode, ProductType, BasicModel, 'ESK_RTN 40000', 'WATER PUMP BELT', 'REPLACE & REASSEMBLY (ENGINE SERVICE KIT)', 0.6, 0.6, 0, 2.6 * LaborPrice, 'JB 424', 0, 0, 1, 'ga', GETDATE(), 'ga', GETDATE(), 0, '', '', 'C' 
from svMstTask where BasicModel = @BasicModel and JobType = 'PB40000'
insert into svMstTask
select CompanyCode, ProductType, BasicModel, 'ESK_RTN 40000', 'PERAWATAN BERKALA 40000 KM', 'PERIODIC MAINTENANCE 40000 (ENGINE SERVICE KIT)', 2.7, 2.7, 0, 2.6 * LaborPrice, 'JB 424', 0, 0, 1, 'ga', GETDATE(), 'ga', GETDATE(), 0, '', '', 'F' 
from svMstTask where BasicModel = @BasicModel and JobType = 'PB40000'
insert into svMstTask
select CompanyCode, ProductType, BasicModel, 'ESK_FS 40000', 'WATER PUMP BELT', 'REPLACE & REASSEMBLY (ENGINE SERVICE KIT)', 0.6, 0.6, 0, 3.3 * LaborPrice, 'JB 424', 0, 0, 1, 'ga', GETDATE(), 'ga', GETDATE(), 0, '', '', 'C' 
from svMstTask where BasicModel = @BasicModel and JobType = 'FSC 40000'
insert into svMstTask
select CompanyCode, ProductType, BasicModel, 'ESK_FS 40000', 'PERAWATAN BERKALA 40000 KM', 'PERIODIC MAINTENANCE 40000 (ENGINE SERVICE KIT)', 2.7, 2.7, 0, 3.3 * LaborPrice, 'JB 424', 0, 0, 1, 'ga', GETDATE(), 'ga', GETDATE(), 0, '', '', 'C' 
from svMstTask where BasicModel = @BasicModel and JobType = 'FSC 40000'
end
end
else
begin
insert into svMstJob
select CompanyCode, ProductType, BasicModel, 'PHMN_RTN 5000', GroupJobType, IsPdiFsc, PdiFscSeq, WarrantyOdometer, WarrantyTimePeriod, WarrantyTimeDim, CounterOperationNo,ReceivableAccountNo, 1, 0,'', '', 'ga', GETDATE(), 'ga', GETDATE()
from svMstJob where JobType = 'PB5000' and BasicModel = @BasicModel
insert into svMstJob
select CompanyCode, ProductType, BasicModel, 'PHMN_RTN 20000', GroupJobType, IsPdiFsc, PdiFscSeq, WarrantyOdometer, WarrantyTimePeriod, WarrantyTimeDim, CounterOperationNo,ReceivableAccountNo, 1, 0,'', '', 'ga', GETDATE(), 'ga', GETDATE()
from svMstJob where JobType = 'PB20000' and BasicModel = @BasicModel
insert into svMstJob
select CompanyCode, ProductType, BasicModel, 'PHMN_RTN 40000', GroupJobType, IsPdiFsc, PdiFscSeq, WarrantyOdometer, WarrantyTimePeriod, WarrantyTimeDim, CounterOperationNo,ReceivableAccountNo, 1, 0,'', '', 'ga', GETDATE(), 'ga', GETDATE()
from svMstJob where JobType = 'PB40000' and BasicModel = @BasicModel

insert into svMstJob
select CompanyCode, ProductType, BasicModel, 'PHMN_FS 5000', GroupJobType, IsPdiFsc, PdiFscSeq, WarrantyOdometer, WarrantyTimePeriod, WarrantyTimeDim, CounterOperationNo,ReceivableAccountNo, 1, 0,'', '', 'ga', GETDATE(), 'ga', GETDATE()
from svMstJob where JobType = 'FSC 5000' and BasicModel = @BasicModel
insert into svMstJob
select CompanyCode, ProductType, BasicModel, 'PHMN_FS 20000', GroupJobType, IsPdiFsc, PdiFscSeq, WarrantyOdometer, WarrantyTimePeriod, WarrantyTimeDim, CounterOperationNo,ReceivableAccountNo, 1, 0,'', '', 'ga', GETDATE(), 'ga', GETDATE()
from svMstJob where JobType = 'FSC 20000' and BasicModel = @BasicModel
insert into svMstJob
select CompanyCode, ProductType, BasicModel, 'PHMN_FS 40000', GroupJobType, IsPdiFsc, PdiFscSeq, WarrantyOdometer, WarrantyTimePeriod, WarrantyTimeDim, CounterOperationNo,ReceivableAccountNo, 1, 0,'', '', 'ga', GETDATE(), 'ga', GETDATE()
from svMstJob where JobType = 'FSC 40000' and BasicModel = @BasicModel

insert into svMstTask
select CompanyCode, ProductType, BasicModel, 'PHMN_RTN 5000', 'PHMN_RTN 5000', Description + ' (PAKET HEMAT MOBIL NIAGA)', OperationHour, ClaimHour, LaborCost, LaborPrice, TechnicalModelCode, IsSubCon, IsCampaign, 1, 'ga', GETDATE(), 'ga', GETDATE(), 0, '', '', BillType 
from svMstTask where BasicModel = @BasicModel and JobType = 'PB5000'
insert into svMstTask
select top 1 CompanyCode, ProductType, BasicModel, 'PHMN_RTN 20000', 'PHMN_RTN 20000', Description + ' (PAKET HEMAT MOBIL NIAGA)', OperationHour, ClaimHour, LaborCost, LaborPrice, TechnicalModelCode, IsSubCon, IsCampaign, 1, 'ga', GETDATE(), 'ga', GETDATE(), 0, '', '', BillType 
from svMstTask where BasicModel = @BasicModel and JobType = 'PB20000'
insert into svMstTask
select CompanyCode, ProductType, BasicModel, 'PHMN_RTN 40000', 'PHMN_RTN 40000', Description + ' (PAKET HEMAT MOBIL NIAGA)', OperationHour, ClaimHour, LaborCost, 0, TechnicalModelCode, IsSubCon, IsCampaign, 1, 'ga', GETDATE(), 'ga', GETDATE(), 0, '', '', BillType 
from svMstTask where BasicModel = @BasicModel and JobType = 'PB40000'

insert into svMstTask
select CompanyCode, ProductType, BasicModel, 'PHMN_FS 5000', 'PHMN_FS 5000', Description + ' (PAKET HEMAT MOBIL NIAGA)', OperationHour, ClaimHour, LaborCost, 0, TechnicalModelCode, IsSubCon, IsCampaign, 1, 'ga', GETDATE(), 'ga', GETDATE(), 0, '', '', BillType 
from svMstTask where BasicModel = @BasicModel and JobType = 'FSC 5000'
insert into svMstTask
select top 1 CompanyCode, ProductType, BasicModel, 'PHMN_FS 20000', 'PHMN_FS 20000', Description + ' (PAKET HEMAT MOBIL NIAGA)', OperationHour, ClaimHour, LaborCost, LaborPrice, TechnicalModelCode, IsSubCon, IsCampaign, 1, 'ga', GETDATE(), 'ga', GETDATE(), 0, '', '', BillType 
from svMstTask where BasicModel = @BasicModel and JobType = 'FSC 20000'
insert into svMstTask
select CompanyCode, ProductType, BasicModel, 'PHMN_FS 40000', 'PHMN_FS 40000', Description + ' (PAKET HEMAT MOBIL NIAGA)', OperationHour, ClaimHour, LaborCost, LaborPrice, TechnicalModelCode, IsSubCon, IsCampaign, 1, 'ga', GETDATE(), 'ga', GETDATE(), 0, '', '', BillType 
from svMstTask where BasicModel = @BasicModel and JobType = 'FSC 40000'

insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'PHMN_RTN 5000', 'PHMN_RTN 5000', '16510-61J00-000', 1, 35000, 'ga', GETDATE(), 'ga', GETDATE(), 'C'
from svMstJob where JobType = 'PB5000' and BasicModel = @BasicModel		
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'PHMN_RTN 5000', 'PHMN_RTN 5000', '09168M14015-000', 1, 5000, 'ga', GETDATE(), 'ga', GETDATE(), 'C'
from svMstJob where JobType = 'PB5000' and BasicModel = @BasicModel		
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'PHMN_RTN 5000', 'PHMN_RTN 5000', '99000B10W40N040', 1, 222000, 'ga', GETDATE(), 'ga', GETDATE(), 'C'
from svMstJob where JobType = 'PB5000' and BasicModel = @BasicModel		

insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'PHMN_FS 5000', 'PHMN_FS 5000', '16510-61J00-000', 1, 35000, 'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'FSC 5000' and BasicModel = @BasicModel		
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'PHMN_FS 5000', 'PHMN_FS 5000', '09168M14015-000', 1, 5000, 'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'FSC 5000' and BasicModel = @BasicModel		
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'PHMN_FS 5000', 'PHMN_FS 5000', '99000B10W40N040', 1, 222000, 'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'FSC 5000' and BasicModel = @BasicModel		

insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'PHMN_RTN 20000', 'PHMN_RTN 20000', '16510-61J00-000', 1, 35000, 'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'PB20000' and BasicModel = @BasicModel		
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'PHMN_RTN 20000', 'PHMN_RTN 20000', '09168M14015-000', 1, 5000, 'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'PB20000' and BasicModel = @BasicModel		
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'PHMN_RTN 20000', 'PHMN_RTN 20000', '09482-00740L000', 4, 18200, 'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'PB20000' and BasicModel = @BasicModel		
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'PHMN_RTN 20000', 'PHMN_RTN 20000', '99000B10W40N040', 1, 222000, 'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'PB20000' and BasicModel = @BasicModel		

insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'PHMN_FS 20000', 'PHMN_FS 20000', '16510-61J00-000', 1, 35000, 'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'FSC 20000' and BasicModel = @BasicModel		
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'PHMN_FS 20000', 'PHMN_FS 20000', '09168M14015-000', 1, 5000, 'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'FSC 20000' and BasicModel = @BasicModel		
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'PHMN_FS 20000', 'PHMN_FS 20000', '09482-00740L000', 4, 18200, 'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'FSC 20000' and BasicModel = @BasicModel		
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'PHMN_FS 20000', 'PHMN_FS 20000', '99000B10W40N040', 1, 222000, 'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'FSC 20000' and BasicModel = @BasicModel		
end

if @BasicModel like 'GC%'	
begin
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'PHMN_RTN 40000', 'PHMN_RTN 40000', '16510-61J00-000', 1, 35000, 'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'PB40000' and BasicModel = @BasicModel	
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'PHMN_RTN 40000', 'PHMN_RTN 40000', '09482-00740L000', 4, 18200,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'PB40000' and BasicModel = @BasicModel		
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'PHMN_RTN 40000', 'PHMN_RTN 40000', '13780-61J00-000', 1, 89000,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'PB40000' and BasicModel = @BasicModel		
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'PHMN_RTN 40000', 'PHMN_RTN 40000', '09168M14015-000', 1, 5000,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'PB40000' and BasicModel = @BasicModel			
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'PHMN_RTN 40000', 'PHMN_RTN 40000', '99000B10W40N040', 1, 222000,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'PB40000' and BasicModel = @BasicModel			
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'PHMN_RTN 40000', 'PHMN_RTN 40000', '99000B22BFDN000', 2, 28000,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'PB40000' and BasicModel = @BasicModel		
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'PHMN_RTN 40000', 'PHMN_RTN 40000', '99000B24COLN000', 1, 115000,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'PB40000' and BasicModel = @BasicModel			

insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'PHMN_FS 40000', 'PHMN_FS 40000', '16510-61J00-000', 1, 35000,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'FSC 40000' and BasicModel = @BasicModel	
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'PHMN_FS 40000', 'PHMN_FS 40000', '09482-00740L000', 4, 18200,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'FSC 40000' and BasicModel = @BasicModel		
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'PHMN_FS 40000', 'PHMN_FS 40000', '13780-61J00-000', 1, 89000,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'FSC 40000' and BasicModel = @BasicModel		
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'PHMN_FS 40000', 'PHMN_FS 40000', '09168M14015-000', 1, 5000,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'FSC 40000' and BasicModel = @BasicModel			
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'PHMN_FS 40000', 'PHMN_FS 40000', '99000B10W40N040', 1, 222000,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'FSC 40000' and BasicModel = @BasicModel			
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'PHMN_FS 40000', 'PHMN_FS 40000', '99000B22BFDN000', 2, 28000,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'FSC 40000' and BasicModel = @BasicModel		
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'PHMN_FS 40000', 'PHMN_FS 40000', '99000B24COLN000', 1, 115000,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'FSC 40000' and BasicModel = @BasicModel			
end

if @BasicModel like 'SL%' or @BasicModel like 'ST%' 
begin
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'PHMN_RTN 40000', 'PHMN_RTN 40000', '16510-61J00-000', 1, 35000,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'PB40000' and BasicModel = @BasicModel	
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'PHMN_RTN 40000', 'PHMN_RTN 40000', '09482-00740L000', 4, 18200,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'PB40000' and BasicModel = @BasicModel	
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'PHMN_RTN 40000', 'PHMN_RTN 40000', '13780B775A0N000', 1, 102400,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'PB40000' and BasicModel = @BasicModel	
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'PHMN_RTN 40000', 'PHMN_RTN 40000', '09168M14015-000', 1, 5000,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'PB40000' and BasicModel = @BasicModel	
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'PHMN_RTN 40000', 'PHMN_RTN 40000', '99000B10W40N040', 1, 222000,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'PB40000' and BasicModel = @BasicModel	
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'PHMN_RTN 40000', 'PHMN_RTN 40000', '99000B22BFDN000', 2, 28000,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'PB40000' and BasicModel = @BasicModel	
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'PHMN_RTN 40000', 'PHMN_RTN 40000', '99000B24COLN000', 1, 115000,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'PB40000' and BasicModel = @BasicModel

insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'PHMN_FS 40000', 'PHMN_FS 40000', '16510-61J00-000', 1, 35000,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'FSC 40000' and BasicModel = @BasicModel	
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'PHMN_FS 40000', 'PHMN_FS 40000', '09482-00740L000', 4, 18200,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'FSC 40000' and BasicModel = @BasicModel	
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'PHMN_FS 40000', 'PHMN_FS 40000', '13780B775A0N000', 1, 102400,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'FSC 40000' and BasicModel = @BasicModel	
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'PHMN_FS 40000', 'PHMN_FS 40000', '09168M14015-000', 1, 5000,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'FSC 40000' and BasicModel = @BasicModel	
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'PHMN_FS 40000', 'PHMN_FS 40000', '99000B10W40N040', 1, 222000,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'FSC 40000' and BasicModel = @BasicModel	
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'PHMN_FS 40000', 'PHMN_FS 40000', '99000B22BFDN000', 2, 28000,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'FSC 40000' and BasicModel = @BasicModel	
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'PHMN_FS 40000', 'PHMN_FS 40000', '99000B24COLN000', 1, 115000,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'FSC 40000' and BasicModel = @BasicModel

end

if @BasicModel like 'JB%'
begin
if @BasicModel = 'JB424'
begin
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'ESK_FS 40000', 'WATER PUMP BELT', '17521-54L31-000', 1, 566000,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'FSC 40000' and BasicModel = @BasicModel
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'ESK_FS 40000', 'WATER PUMP BELT', '16510-61A31-000', 1, 101600,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'FSC 40000' and BasicModel = @BasicModel
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'ESK_FS 40000', 'WATER PUMP BELT', '09168-14015-000', 1, 7500 ,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'FSC 40000' and BasicModel = @BasicModel
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'ESK_FS 40000', 'WATER PUMP BELT', '11518-63J10-000', 1, 11600 ,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'FSC 40000' and BasicModel = @BasicModel
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'ESK_FS 40000', 'WATER PUMP BELT', '09482-00605-000', 4, 59400 ,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'FSC 40000' and BasicModel = @BasicModel
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'ESK_FS 40000', 'WATER PUMP BELT', '13780-78K00-000', 1, 230600 ,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'FSC 40000' and BasicModel = @BasicModel

insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'ESK_RTN 40000', 'WATER PUMP BELT', '17521-54L31-000', 1, 566000,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'PB40000' and BasicModel = @BasicModel
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'ESK_RTN 40000', 'WATER PUMP BELT', '16510-61A31-000', 1, 101600,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'PB40000' and BasicModel = @BasicModel
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'ESK_RTN 40000', 'WATER PUMP BELT', '09168-14015-000', 1, 7500 ,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'PB40000' and BasicModel = @BasicModel
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'ESK_RTN 40000', 'WATER PUMP BELT', '11518-63J10-000', 1, 11600 ,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'PB40000' and BasicModel = @BasicModel
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'ESK_RTN 40000', 'WATER PUMP BELT', '09482-00605-000', 4, 59400 ,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'PB40000' and BasicModel = @BasicModel
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'ESK_RTN 40000', 'WATER PUMP BELT', '13780-78K00-000', 1, 230600 ,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'PB40000' and BasicModel = @BasicModel

insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'FBSK_FS 40000', 'DISC,FRONT BRAKE', '55191-65J00-000', 2, 70100,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'FSC 40000' and BasicModel = @BasicModel
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'FBSK_FS 40000', 'DISC,FRONT BRAKE', '55192-50J00-000', 2, 74100,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'FSC 40000' and BasicModel = @BasicModel
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'FBSK_FS 40000', 'DISC,FRONT BRAKE', '55200-65J25-000', 1, 1272200 ,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'FSC 40000' and BasicModel = @BasicModel
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'FBSK_FS 40000', 'DISC,FRONT BRAKE', '55201-50J00-000', 4, 35100 ,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'FSC 40000' and BasicModel = @BasicModel
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'FBSK_FS 40000', 'DISC,FRONT BRAKE', '55206-50J00-000', 4, 35100 ,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'FSC 40000' and BasicModel = @BasicModel
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'FBSK_FS 40000', 'DISC,FRONT BRAKE', '55840-65J00-000', 2, 253400 ,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'FSC 40000' and BasicModel = @BasicModel
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'FBSK_FS 40000', 'DISC,FRONT BRAKE', '55211-65J02-000', 2, 1162500 ,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'FSC 40000' and BasicModel = @BasicModel

insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'FBSK_RTN 40000', 'DISC,FRONT BRAKE', '55191-65J00-000', 2, 70100,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'PB40000' and BasicModel = @BasicModel
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'FBSK_RTN 40000', 'DISC,FRONT BRAKE', '55192-50J00-000', 2, 74100,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'PB40000' and BasicModel = @BasicModel
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'FBSK_RTN 40000', 'DISC,FRONT BRAKE', '55200-65J25-000', 1, 1272200 ,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'PB40000' and BasicModel = @BasicModel
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'FBSK_RTN 40000', 'DISC,FRONT BRAKE', '55201-50J00-000', 4, 35100 ,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'PB40000' and BasicModel = @BasicModel
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'FBSK_RTN 40000', 'DISC,FRONT BRAKE', '55206-50J00-000', 4, 35100 ,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'PB40000' and BasicModel = @BasicModel
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'FBSK_RTN 40000', 'DISC,FRONT BRAKE', '55840-65J00-000', 2, 253400 ,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'PB40000' and BasicModel = @BasicModel
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'FBSK_RTN 40000', 'DISC,FRONT BRAKE', '55211-65J02-000', 2, 1162500 ,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'PB40000' and BasicModel = @BasicModel

end
if @BasicModel = 'JB420'
begin
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'FBSK_FS 40000', 'DISC,FRONT BRAKE', '55191-65J00-000', 2, 70100,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'FSC 40000' and BasicModel = @BasicModel
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'FBSK_FS 40000', 'DISC,FRONT BRAKE', '55192-50J00-000', 2, 74100,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'FSC 40000' and BasicModel = @BasicModel
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'FBSK_FS 40000', 'DISC,FRONT BRAKE', '55200-65J25-000', 1, 1272200 ,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'FSC 40000' and BasicModel = @BasicModel
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'FBSK_FS 40000', 'DISC,FRONT BRAKE', '55201-50J00-000', 4, 35100 ,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'FSC 40000' and BasicModel = @BasicModel
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'FBSK_FS 40000', 'DISC,FRONT BRAKE', '55206-50J00-000', 4, 35100 ,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'FSC 40000' and BasicModel = @BasicModel
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'FBSK_FS 40000', 'DISC,FRONT BRAKE', '55840-65J00-000', 2, 253400 ,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'FSC 40000' and BasicModel = @BasicModel
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'FBSK_FS 40000', 'DISC,FRONT BRAKE', '55211-65J02-000', 2, 1162500 ,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'FSC 40000' and BasicModel = @BasicModel

insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'FBSK_RTN 40000', 'DISC,FRONT BRAKE', '55191-65J00-000', 2, 70100,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'PB40000' and BasicModel = @BasicModel
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'FBSK_RTN 40000', 'DISC,FRONT BRAKE', '55192-50J00-000', 2, 74100,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'PB40000' and BasicModel = @BasicModel
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'FBSK_RTN 40000', 'DISC,FRONT BRAKE', '55200-65J25-000', 1, 1272200 ,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'PB40000' and BasicModel = @BasicModel
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'FBSK_RTN 40000', 'DISC,FRONT BRAKE', '55201-50J00-000', 4, 35100 ,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'PB40000' and BasicModel = @BasicModel
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'FBSK_RTN 40000', 'DISC,FRONT BRAKE', '55206-50J00-000', 4, 35100 ,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'PB40000' and BasicModel = @BasicModel
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'FBSK_RTN 40000', 'DISC,FRONT BRAKE', '55840-65J00-000', 2, 253400 ,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'PB40000' and BasicModel = @BasicModel
insert into svMstTaskPart
select CompanyCode, ProductType, BasicModel, 'FBSK_RTN 40000', 'DISC,FRONT BRAKE', '55211-65J02-000', 2, 1162500 ,'ga', GETDATE(), 'ga', GETDATE() , 'C'
from svMstJob where JobType = 'PB40000' and BasicModel = @BasicModel
end
end

fetch next from myCursor into @BasicModel

end

close myCursor
deallocate myCursor

select * from svMstRefferenceService where RefferenceType = 'JOBSTYPE' and (RefferenceCode like 'PHMN%' or RefferenceCode like 'FBSK%' or RefferenceCode like 'ESK%')
select * from svMstJob where (JobType like 'PHMN%' or JobType like 'FBSK%' or JobType like 'ESK%')
select * from svMstTask where (JobType like 'PHMN%' or JobType like 'FBSK%' or JobType like 'ESK%')
select * from svMstTaskPart where (JobType like 'PHMN%' or JobType like 'FBSK%' or JobType like 'ESK%')
select * from svMstTaskPrice where (JobType like 'PHMN%' or JobType like 'FBSK%' or JobType like 'ESK%')

drop table #t1
--rollback