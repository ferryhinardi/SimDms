declare @table2 as table (
	FieldName varchar(20),
	FieldDesc varchar(220),
	FieldType varchar(10),
	InqValue1 int,
	InqValue2 int,
	InqValue3 numeric(10, 2),
	InqValAll int,
	SpkValue1 int,
	SpkValue2 int,
	SpkValue3 numeric(10, 2)
)

insert into @table2 (FieldName, FieldDesc, FieldType, InqValAll)
select 'AllModel' as Name, 'All Model' as Caption, 'H', (select count(1) from pmHstITS)
 union all
select 'Ertiga' as Name, 'Ertiga' as Caption, 'H', (select count(1) from pmHstITS where TipeKendaraan in (select ModelType from MsMstGroupModel where GroupModel = 'ERTIGA'))
 union all
select 'WagonR' as Name, 'Karimun Wagon R' as Caption, 'H', (select count(1) from pmHstITS where TipeKendaraan in (select ModelType from MsMstGroupModel where GroupModel = 'WAGON R'))

declare @date1     datetime -- first date
declare @date2     datetime -- last date 
declare @date3     datetime -- last month date 
declare @dateCount int

set @date1 = left(convert(varchar, dateadd(month, -1, getdate()), 112), 6) + '01'
set @date2 = left(convert(varchar, getdate(), 112), 6) + '01'
set @date3 = convert(varchar, dateadd(month, -1, getdate()), 112)
set @dateCount = day(dateadd(day, -1, @date2))

select @date1, @date2, @date3


insert into @table2 (FieldName, FieldDesc, FieldType, InqValue1, InqValue2, InqValue3, SpkValue1, SpkValue2, SpkValue3)
select 'AllModel' as Name, 'All Model' as Caption, 'D'
     , Val1 = (select count(1)/@dateCount from pmHstITS where InquiryDate >= @date1 and InquiryDate < @date2)
	 , Val2 = (select count(1) from pmHstITS where convert(varchar, InquiryDate, 112) = convert(varchar, @date3, 112))
	 , Val3 = 0
	 , Val4 = (select count(1)/@dateCount from pmHstITS a, pmStatusHistory b where a.InquiryNumber = b.InquiryNumber and b.LastProgress = 'SPK' and a.SPKDate >= @date1 and a.SPKDate < @date2)
	 , Val5 = (select count(1) from pmHstITS a, pmStatusHistory b where a.InquiryNumber = b.InquiryNumber and b.LastProgress = 'SPK' and convert(varchar, a.SPKDate, 112) = convert(varchar, @date3, 112))
	 , Val6 = 0

 union all
select 'Ertiga' as Name, 'Ertiga' as Caption, 'D'
     , Val1 = (select count(1)/@dateCount from pmHstITS where InquiryDate >= @date1 and InquiryDate < @date2 and TipeKendaraan in (select ModelType from MsMstGroupModel where GroupModel = 'ERTIGA'))
	 , Val2 = (select count(1) from pmHstITS where convert(varchar, InquiryDate, 112) = convert(varchar, @date3, 112) and TipeKendaraan in (select ModelType from MsMstGroupModel where GroupModel = 'ERTIGA'))
	 , Val3 = 0
	 , Val4 = (select count(1)/@dateCount from pmHstITS a, pmStatusHistory b where a.InquiryNumber = b.InquiryNumber and b.LastProgress = 'SPK' and a.SPKDate >= @date1 and a.SPKDate < @date2 and TipeKendaraan in (select ModelType from MsMstGroupModel where GroupModel = 'ERTIGA'))
	 , Val5 = (select count(1) from pmHstITS a, pmStatusHistory b where a.InquiryNumber = b.InquiryNumber and b.LastProgress = 'SPK' and convert(varchar, a.SPKDate, 112) = convert(varchar, @date3, 112) and TipeKendaraan in (select ModelType from MsMstGroupModel where GroupModel = 'ERTIGA'))
	 , Val6 = 0

 union all
select 'WagonR' as Name, 'Karimun Wagon R' as Caption, 'D'
     , Val1 = (select count(1)/@dateCount from pmHstITS where InquiryDate >= @date1 and InquiryDate < @date2 and TipeKendaraan in (select ModelType from MsMstGroupModel where GroupModel = 'WAGON R'))
	 , Val2 = (select count(1) from pmHstITS where convert(varchar, InquiryDate, 112) = convert(varchar, @date3, 112) and TipeKendaraan in (select ModelType from MsMstGroupModel where GroupModel = 'WAGON R'))
	 , Val3 = 0
	 , Val4 = (select count(1)/@dateCount from pmHstITS a, pmStatusHistory b where a.InquiryNumber = b.InquiryNumber and b.LastProgress = 'SPK' and a.SPKDate >= @date1 and a.SPKDate < @date2 and TipeKendaraan in (select ModelType from MsMstGroupModel where GroupModel = 'WAGON R'))
	 , Val5 = (select count(1) from pmHstITS a, pmStatusHistory b where a.InquiryNumber = b.InquiryNumber and b.LastProgress = 'SPK' and convert(varchar, a.SPKDate, 112) = convert(varchar, @date3, 112) and TipeKendaraan in (select ModelType from MsMstGroupModel where GroupModel = 'ERTIGA'))
	 , Val6 = 0


update @table2 
   set InqValue3 = (InqValue1 / 1.0 * InqValue2)
     , SpkValue3 = (SpkValue1 / 1.0 * SpkValue2)
 where FieldType = 'D'

select * from @table2




