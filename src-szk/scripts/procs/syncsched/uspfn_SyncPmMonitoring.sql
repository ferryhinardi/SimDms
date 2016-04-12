alter procedure uspfn_SyncPmMonitoring
	@Interval int = 3
as

declare @Periode char(6)
	set @Periode = left(convert(varchar, dateadd(M, -@Interval, getdate()), 112), 6)

delete PmMonitoring where InqType = 'SPK' and left(convert(varchar, InqDate, 112), 6) >= @Periode
insert into PmMonitoring (InqType, InqDate, InputDate, InqData)
select 'SPK' as InqType
     , convert(varchar, b.InquiryDate, 112) InquiryDate
     , convert(varchar, a.UpdateDate, 112) SystemDate
	 , count(1) InqData
  from suzukir4..pmStatusHistory a
  join suzukir4..pmHstITS b
    on b.CompanyCode = a.CompanyCode
   and b.BranchCode = a.BranchCode
   and b.InquiryNumber = a.InquiryNumber
  left join suzukir4..MsMstGroupModel c
    on c.ModelType = b.TipeKendaraan
 where 1 = 1
   and a.LastProgress = 'SPK'
   and left(convert(varchar, b.InquiryDate, 112), 6) >= @Periode
 group by convert(varchar, b.InquiryDate, 112), convert(varchar, a.UpdateDate, 112)
 order by convert(varchar, b.InquiryDate, 112), convert(varchar, a.UpdateDate, 112)

delete PmMonitoring where InqType = 'INQ' and left(convert(varchar, InqDate, 112), 6) >= @Periode
insert into PmMonitoring (InqType, InqDate, InputDate, InqData)
select 'INQ' as InqType
     , convert(varchar, b.InquiryDate, 112) InquiryDate
     , convert(varchar, b.CreatedDate, 112) SystemDate
	 , count(1) Value
  from suzukir4..pmHstITS b
  left join suzukir4..MsMstGroupModel c
    on c.ModelType = b.TipeKendaraan
 where 1 = 1
   and left(convert(varchar, b.InquiryDate, 112), 6) >= @Periode
 group by convert(varchar, b.InquiryDate, 112), convert(varchar, b.CreatedDate, 112)
 order by convert(varchar, b.InquiryDate, 112), convert(varchar, b.CreatedDate, 112)

go

exec uspfn_SyncPmMonitoring 


