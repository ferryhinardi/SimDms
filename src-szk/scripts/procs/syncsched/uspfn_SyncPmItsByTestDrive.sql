alter procedure uspfn_SyncPmItsByTestDrive
	@MonthInterval int = 3
as

select * into #INQ
	from ( select d.Area, h.CompanyCode, d.DealerAbbreviation, h.BranchCode, o.OutletAbbreviation, 
				h.InquiryNumber, convert(date,h.InquiryDate) InquiryDate, convert(date,'1900/01/01') SPKDate,
				isnull(m.GroupModel,h.TipeKendaraan) TipeKendaraan, h.Variant, h.ColourCode, h.Transmisi, 1 Inq,
				InqTestDrive = case when isnull(h.TestDrive,'')='YA' then 1 else 0 end,
				0 SPK, 0 SPKTestDrive, h.CreatedDate, h.CreatedBy, h.LastUpdateDate, h.LastUpdateBy
			from SuzukiR4..pmHstITS h
			left join SuzukiR4..gnMstDealerOutletMapping o
			  on o.DealerCode = h.CompanyCode
			 and o.OutletCode = h.BranchCode
			left join SuzukiR4..gnMstDealerMapping d
			  on d.DealerCode = h.CompanyCode
		     and d.GroupNo    = o.GroupNo
			left join SuzukiR4..msMstGroupModel m
			  on m.ModelType  = h.TipeKendaraan
			where 1 = 1
			  and isnull(d.IsActive, 0) = 1
			  and h.LastUpdateDate >= dateadd(month, -@MonthInterval, getdate()) 
		) #INQ

select * into #SPK
	  from ( select d.Area, h.CompanyCode, d.DealerAbbreviation, h.BranchCode, o.OutletAbbreviation, h.InquiryNumber, 
	                convert(date,h.InquiryDate) InquiryDate, convert(date,s.UpdateDate,112) SPKDate, 
					isnull(m.GroupModel,h.TipeKendaraan) TipeKendaraan, h.Variant, h.ColourCode, 
					h.Transmisi, 0 Inq, 0 InqTestDrive, 1 SPK,
					SPKTestDrive = case when isnull(h.TestDrive,'') = 'YA' then 1 else 0 end,
					h.CreatedDate, h.CreatedBy, h.LastUpdateDate, h.LastUpdateBy
			   from SuzukiR4..pmHstITS h
			  inner join SuzukiR4..pmStatusHistory s
				 on h.CompanyCode   = s.CompanyCode
				and h.BranchCode    = s.BranchCode
				and h.InquiryNumber = s.InquiryNumber
				and s.LastProgress  ='SPK'
				and s.SequenceNo    = (select top 1 SequenceNo from pmStatusHistory
			                            where CompanyCode  =s.CompanyCode
			                              and BranchCode   =s.BranchCode
			                              and InquiryNumber=s.InquiryNumber
			                              and LastProgress ='SPK'
			                            order by UpdateDate desc)
			   left	join SuzukiR4..gnMstDealerOutletMapping o
				 on o.DealerCode    = h.CompanyCode
				and o.OutletCode    = h.BranchCode
			   left join SuzukiR4..gnMstDealerMapping d
				 on d.DealerCode    = h.CompanyCode
				and d.GroupNo       = o.GroupNo
			   left join SuzukiR4..msMstGroupModel m
			     on m.ModelType     = h.TipeKendaraan
              where 1 = 1
			    and isnull(d.IsActive, 0) = 1
 			    and s.UpdateDate >= dateadd(month, -@MonthInterval, getdate()) 
   				--and convert(varchar,s.UpdateDate,112) between @StartDate and @EndDate
		   ) #SPK

update #INQ
   set SPKDate = s.SPKDate
     , SPK = s.SPK
	 , SPKTestDrive = s.SPKTestDrive
  from #INQ h
 inner join #SPK s
    on h.CompanyCode=s.CompanyCode
   and h.BranchCode=s.BranchCode
   and h.InquiryNumber=s.InquiryNumber

;with x as (
select * from #SPK
 where not exists (
	select 1 from #INQ
	 where #INQ.CompanyCode  = #SPK.CompanyCode
	   and #INQ.BranchCode   = #SPK.BranchCode
	   and #INQ.InquiryNumber= #SPK.InquiryNumber)
)
insert into #INQ select * from x

;with x as (
select Area, CompanyCode, BranchCode, InquiryNumber, DealerAbbreviation, OutletAbbreviation, InquiryDate
     , SPKDate, TipeKendaraan, Variant, ColourCode, Transmisi, Inq, InqTestDrive
	 , OutsSPK           = isnull((case when left(convert(varchar,InquiryDate,112),6)
		                                    < left(convert(varchar,SPKDate,112),6)
										then SPK else 0 end),0)
	 , OutsSPKTestDrive  = isnull((case when left(convert(varchar,InquiryDate,112),6)
											< left(convert(varchar,SPKDate,112),6)
										then SPKTestDrive else 0 end),0)
	 , NewSPK            = isnull((case when left(convert(varchar,InquiryDate,112),6)
											>= left(convert(varchar,SPKDate,112),6)
										then SPK else 0 end),0)
	 , NewSPKTestDrive   = isnull((case when left(convert(varchar,InquiryDate,112),6)
											>= left(convert(varchar,SPKDate,112),6)
										then SPKTestDrive else 0 end),0)
	 , TotalSPK          = isnull(SPK,0)
	 , TotalSPKTestDrive = isnull(SPKTestDrive,0)
	 , CreatedDate, CreatedBy, LastUpdateDate, LastUpdateBy
  from #INQ
)
select * into #t1 from x

delete PmItsByTestDrive where exists 
(select 1 from #t1 where #t1.Area = PmItsByTestDrive.Area and #t1.CompanyCode = PmItsByTestDrive.CompanyCode and #t1.BranchCode = PmItsByTestDrive.BranchCode and #t1.InquiryNumber = PmItsByTestDrive.InquiryNumber)

insert into PmItsByTestDrive (Area, CompanyCode, BranchCode, InquiryNumber, DealerAbbreviation, OutletAbbreviation, InquiryDate, SPKDate, TipeKendaraan, Variant, ColourCode, Transmisi, Inq, InqTestDrive, OutsSPK, OutsSPKTestDrive, NewSPK, NewSPKTestDrive, TotalSPK, TotalSPKTestDrive, CreatedDate, CreatedBy, LastUpdateDate, LastUpdateBy)
select Area, CompanyCode, BranchCode, InquiryNumber, DealerAbbreviation, OutletAbbreviation, InquiryDate, SPKDate, TipeKendaraan, Variant, ColourCode, Transmisi, Inq, InqTestDrive, OutsSPK, OutsSPKTestDrive, NewSPK, NewSPKTestDrive, TotalSPK, TotalSPKTestDrive, CreatedDate, CreatedBy, LastUpdateDate, LastUpdateBy from #t1

go

uspfn_SyncPmItsByTestDrive 
