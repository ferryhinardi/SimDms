alter procedure uspfn_SyncPmItsByLeadTime
	@MonthInterval int = 3
as

;with x as (
select d.Area
     , h.CompanyCode
	 , h.BranchCode
	 , h.InquiryNumber
	 , h.InquiryDate
	 , d.DealerAbbreviation
	 , o.OutletAbbreviation
	 , isnull(m.GroupModel,h.TipeKendaraan) TipeKendaraan
	 , h.Variant
	 , h.Transmisi
	 , left(convert(varchar,h.InquiryDate,112),6) Period
	 , PDate = (select top 1 UpdateDate from pmStatusHistory where CompanyCode=h.CompanyCode and BranchCode=h.BranchCode and InquiryNumber=h.InquiryNumber and LastProgress='P' order by SequenceNo)
	 , HPDate = (select top 1 UpdateDate from pmStatusHistory where CompanyCode=h.CompanyCode and BranchCode=h.BranchCode and InquiryNumber=h.InquiryNumber and LastProgress='HP' order by SequenceNo)
	 , SPKDate = (select top 1 UpdateDate from pmStatusHistory where CompanyCode=h.CompanyCode and BranchCode=h.BranchCode and InquiryNumber=h.InquiryNumber and LastProgress='SPK' order by SequenceNo)
	 , LastProgress = isnull((select top 1 LastProgress from pmStatusHistory where CompanyCode=h.CompanyCode and BranchCode=h.BranchCode and InquiryNumber=h.InquiryNumber order by SequenceNo desc),'P')
	 , h.CreatedDate
	 , h.CreatedBy
	 , h.LastUpdateDate
	 , h.LastUpdateBy
  from SuzukiR4..pmHstITS h
  left join SuzukiR4..gnMstDealerOutletMapping o
    on o.DealerCode = h.CompanyCode
   and o.OutletCode = h.BranchCode
  left join SuzukiR4..gnMstDealerMapping d
    on d.DealerCode = h.CompanyCode
   and d.GroupNo = o.GroupNo
   left join SuzukiR4..msMstGroupModel m
    on m.ModelType =h.TipeKendaraan
  where 1 = 1
	and h.LastUpdateDate >= dateadd(month, -@MonthInterval, getdate()) 
	and isnull(d.IsActive, 0) = 1
)
, y as (
select x.Area
     , x.CompanyCode
	 , x.BranchCode
	 , x.InquiryNumber
	 , x.InquiryDate
	 , x.DealerAbbreviation
	 , x.OutletAbbreviation
	 , x.TipeKendaraan
	 , x.Variant
	 , x.Transmisi
	 , x.Period
	 , x.PDate
	 , x.HPDate
	 , x.SPKDate
	 , LeadTimeHp = datediff(day,convert(date, x.InquiryDate),convert(date, x.HPDate))
	 , LeadTimeSpk = datediff(day,convert(date, x.InquiryDate),convert(date, x.SPKDate))
	 , x.LastProgress
	 , x.CreatedDate
	 , x.CreatedBy
	 , x.LastUpdateDate
	 , x.LastUpdateBy
  from x
)
select * into #t1 from y

delete PmItsByLeadTime where exists 
(select 1 from #t1 where #t1.Area = PmItsByLeadTime.Area and #t1.CompanyCode = PmItsByLeadTime.CompanyCode and #t1.BranchCode = PmItsByLeadTime.BranchCode and #t1.InquiryNumber = PmItsByLeadTime.InquiryNumber)

insert into PmItsByLeadTime (Area, CompanyCode, BranchCode, InquiryNumber, InquiryDate, DealerAbbreviation, OutletAbbreviation, TipeKendaraan, Variant, Transmisi, Period, PDate, HPDate, SPKDate, LeadTimeHp, LeadTimeSpk, LastProgress, CreatedDate, CreatedBy, LastUpdateDate, LastUpdateBy)
select Area, CompanyCode, BranchCode, InquiryNumber, InquiryDate, DealerAbbreviation, OutletAbbreviation, TipeKendaraan, Variant, Transmisi, Period, PDate, HPDate, SPKDate, LeadTimeHp, LeadTimeSpk, LastProgress, CreatedDate, CreatedBy, LastUpdateDate, LastUpdateBy from #t1

select convert(varchar, count(*)) + ' rows ITS data by Lead Time was executed' as Info from #t1

go

uspfn_SyncPmItsByLeadTime 
