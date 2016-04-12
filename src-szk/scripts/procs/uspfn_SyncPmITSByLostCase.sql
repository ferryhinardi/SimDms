ALTER procedure [dbo].[uspfn_SyncPmITSByLostCase]
	@MonthInterval int = 3
as

BEGIN
	declare @StartDate varchar(8)
	declare @EndDate   varchar(8)
	declare @Filter    char(1)		-- 0:InquiryDate, 1:LostCaseDate
	
	set @Filter    = '0'
	set @EndDate = convert(varchar(8), getdate(), 112);
	set @StartDate = convert(varchar(8), dateadd(month, -(@MonthInterval), getdate()), 112);
	set @StartDate = '19000101';

	select * into #its from (
			select	AreaCode = d.GroupNo, d.Area, h.CompanyCode, d.DealerAbbreviation, h.BranchCode, o.OutletAbbreviation, h.InquiryNumber, 
					InquiryDate      = convert(date,h.InquiryDate),
					ProspectDate     = (case when convert(date,h.ProspectDate) > convert(date,'1900/01/01')
												  then convert(date,h.ProspectDate) else NULL end),
					HotProspectDate  = (case when convert(date,h.HotDate) > convert(date,'1900/01/01') 
												  then convert(date,h.HotDate) else NULL end),
					SPKDate          = (case when convert(date,h.SPKDate) > convert(date,'1900/01/01') 
								 				  then convert(date,SPKDate) else NULL end),
				  --LostCaseDate     = (case when convert(date,LostCaseDate) > convert(date,'1900/01/01') 
				  --							  then convert(date,LostCaseDate) else NULL end),
				    LostCaseDate     = (select top 1 UpdateDate from SuzukiR4..pmStatusHistory 
				                         where CompanyCode=h.CompanyCode
				                           and BranchCode =h.BranchCode
				                           and InquiryNumber=h.InquiryNumber
				                           and LastProgress ='LOST'
				                         order by UpdateDate desc),
					StatusbeforeLOST = (case when convert(date,h.SPKDate) > convert(date,'1900/01/01') then 'SPK'
											 when convert(date,h.HotDate) > convert(date,'1900/01/01') then 'HOT PROSPECT'
											 else															'PROSPECT' end),
	                P_Outs           = (case when convert(date,h.SPKDate)      <= convert(date,'1900/01/01') 
					                          and convert(date,h.HotDate)      <= convert(date,'1900/01/01')
					                          and convert(date,h.ProspectDate) >  convert(date,'1900/01/01')
					                          and left(convert(varchar,h.ProspectDate,112),6) <  left(convert(varchar,h.LostCaseDate,112),6)
					                         then 1 else 0 end),
	                P_New            = (case when convert(date,h.SPKDate)      <= convert(date,'1900/01/01') 
					                          and convert(date,h.HotDate)      <= convert(date,'1900/01/01')
					                          and convert(date,h.ProspectDate) >  convert(date,'1900/01/01')
					                          and left(convert(varchar,h.ProspectDate,112),6) >= left(convert(varchar,h.LostCaseDate,112),6)
					                         then 1 else 0 end),
					HP_Outs          = (case when convert(date,h.SPKDate) <= convert(date,'1900/01/01') 
					                          and convert(date,h.HotDate) >  convert(date,'1900/01/01')
					                          and left(convert(varchar,h.HotDate,112),6) <  left(convert(varchar,h.LostCaseDate,112),6)
					                         then 1 else 0 end),
					HP_New           = (case when convert(date,h.SPKDate) <= convert(date,'1900/01/01') 
					                          and convert(date,h.HotDate) >  convert(date,'1900/01/01')
					                          and left(convert(varchar,h.HotDate,112),6) >= left(convert(varchar,h.LostCaseDate,112),6)
					                         then 1 else 0 end),
					SPK_Outs         = (case when convert(date,h.SPKDate) > convert(date,'1900/01/01') 
					                          and left(convert(varchar,h.SPKDate,112),6) <  left(convert(varchar,h.LostCaseDate,112),6)
					                         then 1 else 0 end),
					SPK_New          = (case when convert(date,h.SPKDate) > convert(date,'1900/01/01') 
					                          and left(convert(varchar,h.SPKDate,112),6) >= left(convert(varchar,h.LostCaseDate,112),6)
					                         then 1 else 0 end),
					h.TipeKendaraan, h.Variant, h.Transmisi, h.ColourCode, 
					h.LostCaseCategory, c.LookupValueName LostCaseCategoryDesc,
					h.LostCaseReasonID, r.LookupValueName LostCaseReasonDesc,
					LostCaseOtherReason=isnull(h.LostCaseOtherReason,''), 
					LostCaseVoiceOfCustomer=isnull(h.LostCaseVoiceOfCustomer,'')
			  from	SuzukiR4..pmHstITS h
			  left	join SuzukiR4..gnMstDealerOutletMapping o
					  on o.DealerCode=h.CompanyCode
					 and o.OutletCode=h.BranchCode
			  left  join SuzukiR4..gnMstDealerMapping d
					  on d.DealerCode=h.CompanyCode
					 and d.GroupNo   =o.GroupNo
			  left  join SuzukiR4..gnMstLookUpDtl c
					  on c.CompanyCode='6006406'
					 and c.CodeID     ='PLCC'
					 and c.LookUpValue=h.LostCaseCategory
			  left	join SuzukiR4..gnMstLookUpDtl r
					  on r.CompanyCode='6006406'
					 and r.CodeID     ='ITLR'
					 and r.LookUpValue=h.LostCaseReasonID
		   --where  h.LastProgress='LOST'
			 where  exists (select top 1 1 from pmStatusHistory s
			                 where CompanyCode=h.CompanyCode
			                   and BranchCode=h.BranchCode
			                   and InquiryNumber=h.InquiryNumber
			                   and LastProgress='LOST')
			   --and convert( varchar, h.LastUpdateDate, 112) between @StartDate and @EndDate
			  -- and  case @Filter 
					--when '0' then convert(varchar,h.InquiryDate, 112) else convert(varchar,h.LostCaseDate,112) end
					--		 between @StartDate and @EndDate
	    ) #its;

	--select	* from #its order by DealerAbbreviation, BranchCode, InquiryNumber;

	delete PmITSByLostCase0
	 where convert(varchar, InquiryDate,112) between @StartDate and @EndDate;

	delete PmITSByLostCase1
	 where convert(varchar, LostCaseDate,112) between @StartDate and @EndDate;

	  insert into PmITSByLostCase0
	  select * from (
				select *
				  from #its a
				 where convert(varchar, a.InquiryDate,112) between @StartDate and @EndDate
		   ) as x;

	  insert into PmITSByLostCase1
	  select * from (
				select *
				  from #its a
				 where convert(varchar, a.LostCaseDate,112) between @StartDate and @EndDate
		   ) as x;

	drop table #its;
END




