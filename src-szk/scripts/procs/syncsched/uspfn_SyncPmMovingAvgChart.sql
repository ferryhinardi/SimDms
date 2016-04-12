-----------------------------------------------------------------------
-- DATA FOR :
--   1. MOVING AVERAGE INQUIRY, SPK, FAKTUR POLISI 
--   2. TREND COMPARATION MONTH & MONTH-1 (INQ/SPK/FAKTUR POLISI & TYPE/MODEL
-- Request  by Davy san
-- Created  by HTO, 16 May 2014
-- Revision by HTO, 19 May 2014 ==> INQ: D30~D1,  SPK,FPOL: D
-- Revision by HTO, 23 May 2014 ==> INQ,SPK,FPOL : D29 ~ D
-----------------------------------------------------------------------


ALTER PROCEDURE [dbo].[uspfn_SyncPmMovingAvgChart]
AS

BEGIN
	declare	@Date	date
	declare @Start	date
	declare	@End	date
	declare @First	date
	declare @Count  integer
	declare @INQ	numeric(9,2)
	declare @SPK	numeric(9,2)
	declare @FPOL	numeric(9,2)
	
 -- Moving Average
	set @Date  = getdate()   --convert(date,'2014/01/31')
 -- set @First = dateadd(day,((-1*day(@Date))+1), @Date)
 	set @First = dateadd(day,-31,@Date)
	delete SimDms..PmDashboardByDay where DashDate>=@First

	while @First <= @Date
		begin
			set @Start = dateadd(day,-29,@First)
			set @End   = @First

		 -- Inquiry#
			set @INQ   = isnull((select count(*) from
									(select a.CompanyCode, a.BranchCode, a.InquiryNumber
									   from SuzukiR4..pmHstITS a
									  where convert(date,a.InquiryDate) between @Start and @End
									) a
							   ),0) / 30--.00

		 -- SPK#
			set @SPK   = isnull((select count(*) from
									(select a.CompanyCode, a.BranchCode, a.InquiryNumber
									   from SuzukiR4..pmStatusHistory a
									  inner join SuzukiR4..pmHstITS b
											  on b.CompanyCode  =a.CompanyCode
											 and b.BranchCode   =a.BranchCode
											 and b.InquiryNumber=a.InquiryNumber
									  where a.LastProgress='SPK'
										and convert(date,a.UpdateDate) between @Start and @End
									) a
							   ),0) / 30--.00

		 -- Faktur Polisi#
			set @FPOL  = isnull((select count(*) from
									(select distinct h.ChassisCode, h.ChassisNo, h.EngineCode, h.EngineNo
									   from SuzukiR4..omHstInquirySalesNSDS h
									  where convert(date,h.ProcessDate) between @Start and @End
									) a
							   ),0) / 30--.00

			insert into SimDMS..pmDashBoardByDay ( DashDate, GroupModel, InqValue, SpkValue, FakturValue)
						select convert(varchar,@First,112), 'MOVINGAVG', @INQ, @SPK, @FPOL
			set @First = dateadd(day,+1,@First)
		end
			
 -- TREND COMPARISON MONTH & MONTH-1
	declare @Model	table
	       (GroupNo	int primary key,
	        GroupDs	varchar(30),
	        GroupNm varchar(30),
	        INQ		numeric(9,2),
	        SPK		numeric(9,2),
	        FPOL	numeric(9,2))

 -- set @First = dateadd(day,((-1*day(@Date))+1), @Date)
 	set @First = dateadd(day,-31,@Date)

	insert into @Model
		select 1, 'ALL'			 ,'ALL'		,0.00,0.00,0.00	union all
		select 2, 'WAGON R'		 ,'WAGON R'	,0.00,0.00,0.00	union all
		select 3, 'ERTIGA'       ,'ERTIGA'	,0.00,0.00,0.00	union all
		select 4, 'PU FUTURA'    ,'SL415-PU',0.00,0.00,0.00	union all
		select 5, 'PU MEGA CARRY','APV-PU'  ,0.00,0.00,0.00	union all
		select 6, 'OTHER'        ,'OTHER'   ,0,0,0

 	while @First <= @Date
		begin
            update  @Model
					set INQ  = isnull((select count(*) from
											( select a.CompanyCode, a.BranchCode, a.InquiryNumber
												from SuzukiR4..pmHstITS a
											   where convert(date,a.InquiryDate) = @First) a),0)
					  , SPK  = isnull((select count(*) from
											( select a.CompanyCode, a.BranchCode, a.InquiryNumber
												from SuzukiR4..pmStatusHistory a
											   inner join SuzukiR4..pmHstITS b
													   on b.CompanyCode  =a.CompanyCode
													  and b.BranchCode   =a.BranchCode
													  and b.InquiryNumber=a.InquiryNumber
											   where a.LastProgress='SPK'
												 and convert(date,a.UpdateDate) = @First) a),0)
					  
					  , FPOL = isnull((select count(*) from
											( select distinct h.ChassisCode, h.ChassisNo, h.EngineCode, h.EngineNo
												from SuzukiR4..omHstInquirySalesNSDS h
											   where convert(date,h.ProcessDate) = @First) a),0)
			 where	GroupNo=1

         -- BY TYPE/MODEL
			set @Count = 1
			while @Count<5
				begin
				  set @Count = @Count + 1
				  update @Model
					 set INQ  = isnull((select count(*) from
									 		 ( select a.CompanyCode, a.BranchCode, a.InquiryNumber
											 	 from SuzukiR4..pmHstITS a
											     left join SuzukiR4..msMstGroupModel m 
														on a.TipeKendaraan=m.ModelType
											    where convert(date,a.InquiryDate) = @First
												  and m.GroupModel=(select GroupNm from @Model where GroupNo=@Count)
											 ) a),0) 
					   , SPK  = isnull((select count(*) from
											 ( select a.CompanyCode, a.BranchCode, a.InquiryNumber
												 from SuzukiR4..pmStatusHistory a
											    inner join SuzukiR4..pmHstITS b
												 	    on b.CompanyCode  =a.CompanyCode
													   and b.BranchCode   =a.BranchCode
													   and b.InquiryNumber=a.InquiryNumber
											     left join SuzukiR4..msMstGroupModel m
														on b.TipeKendaraan=m.ModelType
											    where a.LastProgress='SPK'
												  and convert(date,a.UpdateDate) = @First
												  and m.GroupModel=(select GroupNm from @Model where GroupNo=@Count)
											 ) a),0)
					   , FPOL = isnull((select count(*) from
											 ( select distinct h.ChassisCode, h.ChassisNo, h.EngineCode, h.EngineNo
												 from SuzukiR4..omHstInquirySalesNSDS h
												 left join SuzukiR4..omMstModel d
														on h.SalesModelCode=substring(d.SalesModelCode,1,15)
											     left join SuzukiR4..msMstGroupModel m
														on d.GroupCode=m.ModelType
											    where convert(date,h.ProcessDate) = @First
												  and m.GroupModel=(select GroupNm from @Model where GroupNo=@Count)
											 ) a),0)
				   where GroupNo=@Count
				end
				  
         -- OTHER MODEL
			update @Model
			   set INQ  = (select INQ  from @Model where GroupNo=1) - (select sum(INQ)  from @Model where GroupNo between 2 and 5)
			     , SPK  = (select SPK  from @Model where GroupNo=1) - (select sum(SPK)  from @Model where GroupNo between 2 and 5)
			     , FPOL = (select FPOL from @Model where GroupNo=1) - (select sum(FPOL) from @Model where GroupNo between 2 and 5)
			 where GroupNo = 6
			 
		 -- insert into SimDMS..pmDashBoardByDay ( DashDate, 'ALL', InqValue, SpkValue, FakturValue)
			insert into SimDms..pmDashBoardByDay ( DashDate, GroupModel, InqValue, SpkValue, FakturValue)
						select convert(varchar,@First,112), GroupDs, INQ, SPK, FPOL from @Model
						 where INQ>0 or SPK>0 or FPOL>0 order by GroupNo
			update @Model set INQ=0, SPK=0, FPOL=0
			set @First = dateadd(day,+1,@First)
		end

	--select * from SimDMS..pmDashBoardByDay order by DashDate desc
	select 'done' as result

END
