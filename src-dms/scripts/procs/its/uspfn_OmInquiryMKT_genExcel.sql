if object_id('uspfn_OmInquiryMKT_genExcel') is not null
	drop procedure uspfn_OmInquiryMKT_genExcel
GO
-- ==============================================================================
-- Author			: Seandy A.K
-- Create date		: 30-1-2013
-- Description		: Inquiry ITS
-- Query Activation : uspfn_InquiryITS '2013-03-12','2013-03-12','CABANG','',''
-- Query Activation : uspfn_OmInquiryMKT '2013-03-12','2013-03-12','CABANG','',''
-- update by fhi    : fhi 17-03-2015
-- Query Activation : uspfn_OmInquiryMKT_genExcel '2013-03-12','2013-03-12','CABANG','6006406',''
-- ==============================================================================
Create PROCEDURE [dbo].[uspfn_OmInquiryMKT_genExcel]
	@StartDate	datetime,
	@EndDate	datetime,
	@Area		nvarchar(100),
	@DealerCode	nvarchar(15),
	@OutletCode	nvarchar(15)
AS
declare @National varchar(10)
set @National = (select top 1 ISNULL(ParaValue,0) from gnMstLookUpDtl
                  where CodeID='QSLS' and LookUpValue='NATIONAL')

Declare @MainTable table
(
	DealerCode			varchar(15),
	DealerAbbreviation	varchar(50),
	OutletCode			varchar(15),
	OutletAbbreviation	varchar(50),
	TipeKendaraan		varchar(50),
	Variant				varchar(50),
	OutsINQ				numeric(18,0),
	NewINQ				numeric(18,0),
	OutsSPK				numeric(18,0),
	NewSPK				numeric(18,0),
	CancelSPK			numeric(18,0),
	FakturPolisi		numeric(18,0),
	Balance				numeric(18,0),
	ATTestDrive			int,
	MTTestDrive			int
)

if(@National = 1)
begin	
	Select * into #t1 from(
		Select distinct b.Area
			 , b.DealerCode
			 , b.DealerAbbreviation
			 , c.OutletCode
			 , c.OutletAbbreviation
			 , a.TipeKendaraan
			 , a.Transmisi
			 , a.Variant
			 , a.InquiryDate
			 , a.SPKDate
			 , case when isnull(a.TestDrive,'') = '' then 0 else case when isnull(a.TestDrive,'') = 'YA' then 1 else 0 end end TestDrive
			 , a.InquiryNumber
			 , a.LastProgress
		from pmHstITS a
		left join GnMstDealerMapping b on a.CompanyCode = b.DealerCode
		left join GnMstDealerOutletMapping c on a.CompanyCode = c.DealerCode
			and a.BranchCode = c.OutletCode
		where convert(varchar,InquiryDate,112) <= convert(varchar,@EndDate,112)
		  and (b.Area like Case when @Area <> '' 
						then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
								then 'JABODETABEK'
								else @Area end
						else '%%' end
		   or b.Area like Case when @Area <> '' 
						then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
								then 'CABANG'
								else @Area end
						else '%%' end) 	
		  and b.DealerCode like case when @DealerCode = '' then '%%' else @DealerCode end
		  and c.OutletCode like case when @OutletCode = '' then '%%' else @OutletCode end
	)#t1  
	
	select * into #TempStock from(
		select distinct a.CompanyCode
			 , a.BranchCode
			 , a.TipeKendaraan
			 , a.Variant
			 , a.ColourCode
			 --, isnull(f.UnitQuantity,0) UnitQuantity
			 , (select TOP 1 UnitQuantity
			      from pmHstStock
			    where CompanyCode = a.CompanyCode
			      and BranchCode = a.BranchCode
			      and Year(TanggalStock) = YEAR(@EndDate)
			      and MONTH(TanggalStock) = MONTH(@EndDate)
			      and TipeKendaraan = a.TipeKendaraan
			      and Variant = a.Variant
			      and ColourCode = a.ColourCode
			 order by a.TanggalStock Desc) UnitQuantity
			from pmHstStock a
			where YEAR(a.TanggalStock) = YEAR(@EndDate)
			  and MONTH(a.TanggalStock) = MONTH(@EndDate)
	)#TempStock
	
	Insert into @MainTable
	Select distinct a.DealerCode
		 , a.DealerAbbreviation
		 , a.OutletCode
		 , a.OutletAbbreviation
		 , a.TipeKendaraan
		 , a.Variant
		 , isnull((select Count(*) 
			  from #t1 e
			  where convert(varchar,e.InquiryDate,112) < convert(varchar,@StartDate,112)
				and a.Area = e.Area
				and a.DealerCode = e.DealerCode
				and a.OutletCode = e.OutletCode
				and a.TipeKendaraan = e.TipeKendaraan
			    and e.Variant = a.Variant
				and e.LastProgress in ('P','HP','SPK')),0) OutsINQ
		 , isnull((select Count(*) 
			  from #t1
			  where convert(varchar,InquiryDate,112) between convert(varchar,@StartDate,112) and convert(varchar,@EndDate,112)
				and Area = a.Area
				and DealerCode = a.DealerCode
				and OutletCode = a.OutletCode
				and TipeKendaraan = a.TipeKendaraan
			    and Variant = a.Variant),0) NewINQ
		 , (select COUNT(*) 
			  from #t1 
			 where DealerCode = a.DealerCode
			   and OutletCode = a.OutletCode
			   and convert(varchar,InquiryDate,112) < convert(varchar,@StartDate,112)
			   and convert(varchar,SPKDate,112) between convert(varchar,@StartDate,112) and convert(varchar,@EndDate,112)
			   and TipeKendaraan = a.TipeKendaraan
			   and Variant = a.Variant
			   and LastProgress in ('SPK')) OutsSPK
		 , (select COUNT(*) 
			  from #t1 
			 where DealerCode = a.DealerCode
			   and OutletCode = a.OutletCode
			   and convert(varchar,InquiryDate,112) between convert(varchar,@StartDate,112) and convert(varchar,@EndDate,112)
			   and convert(varchar,SPKDate,112) between convert(varchar,@StartDate,112) and convert(varchar,@EndDate,112)
			   and TipeKendaraan = a.TipeKendaraan
			   and Variant = a.Variant
			   and LastProgress in ('SPK')) NewSPK
		 , (select COUNT(*) 
			  from #t1 e
			 where e.DealerCode = a.DealerCode
			   and e.OutletCode = a.OutletCode
			   and convert(varchar,e.InquiryDate,112) <= convert(varchar,@EndDate,112)
			   and e.TipeKendaraan = a.TipeKendaraan
			   and e.Variant = a.Variant
			   and exists (select  1 
							  from pmHstITS 
							 where InquiryNumber = e.InquiryNumber
							   and LastProgress in ('LOST')
							   and LostCaseCategory = 'KONTRAK TIDAK DISETUJUI')) CancelSPK
		 , isnull((select Count(*)
			  from OmHstInquirySales d
			  left join OmMstModel e on d.CompanyCode = e.CompanyCode
				and d.SalesModelCode = e.SalesModelCode
			  where convert(varchar,d.SuzukiFpolDate,112) between convert(varchar,@StartDate,112) and convert(varchar,@EndDate,112)
				and d.CompanyCode = a.DealerCode
				and d.BranchCode = a.OutletCode
				and e.GroupCode = a.TipeKendaraan
				and Variant = a.Variant
				and e.TypeCode = a.Variant),0) FakturPolisi
		 --, isnull((select isnull(SUM(f.UnitQuantity),0)
			--		from pmHstStock f
			--		where f.CompanyCode = a.DealerCode
			--		  and f.BranchCode = a.OutletCode
			--		  and YEAR(f.TanggalStock) = YEAR(@EndDate)
			--		  and MONTH(f.TanggalStock) = MONTH(@EndDate)
			--		  and f.TipeKendaraan = a.TipeKendaraan
			--		  and f.Variant = a.Variant),0) Balance
		 , isnull((select SUM(f.UnitQuantity)
					from #TempStock f
					where f.CompanyCode = a.DealerCode
					  and f.BranchCode = a.OutletCode
					  and f.TipeKendaraan = a.TipeKendaraan
					  and f.Variant = a.Variant
					  ),0) Balance
		 , isnull((select SUM(TestDrive) 
			  from #t1
			  where convert(varchar,InquiryDate,112) between convert(varchar,@StartDate,112) and convert(varchar,@EndDate,112)
			    and Area = a.Area
			    and DealerCode = a.DealerCode
			    and OutletCode = a.OutletCode
			    and TipeKendaraan = a.TipeKendaraan
			    and Transmisi = 'AT'
			    and Variant = a.Variant),0) ATTestDrive
		 , isnull((select SUM(TestDrive) 
			  from #t1
			  where convert(varchar,InquiryDate,112) between convert(varchar,@StartDate,112) and convert(varchar,@EndDate,112)
			    and Area = a.Area
			    and DealerCode = a.DealerCode
			    and OutletCode = a.OutletCode
			    and TipeKendaraan = a.TipeKendaraan
			    and Transmisi = 'MT'
			    and Variant = a.Variant),0) MTTestDrive
	from #t1 a
	UNION
	select a.CompanyCode
		 , b.DealerAbbreviation
		 , a.BranchCode
		 , c.OutletAbbreviation
		 , isnull(e.GroupCode,'') TipeKendaraan
		 , isnull(e.TypeCode,'') Variant
		 , 0 
		 , 0 
		 , 0
		 , 0
		 , 0
		 , 0
		 , COUNT(*)
		 , 0
		 , 0
	from OmHstInquirySales a
	left join OmMstModel e on e.CompanyCode = a.CompanyCode
		and e.SalesModelCode = a.SalesModelCode
	left join gnMstDealerMapping b on b.DealerCode = a.CompanyCode
	left join gnMstDealerOutletMapping c on c.DealerCode = a.CompanyCode
		and c.OutletCode = a.BranchCode
	where Convert(varchar,a.SuzukiFPOLDate,112) between convert(varchar,@StartDate,112) and convert(varchar,@EndDate,112)
	  and (b.Area like Case when @Area <> '' 
					then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
							then 'JABODETABEK'
							else @Area end
					else '%%' end
	   or b.Area like Case when @Area <> '' 
					then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
							then 'CABANG'
							else @Area end
					else '%%' end) 	
	  and a.CompanyCode like case when @DealerCode = '' then '%%' else @DealerCode end
	  and a.BranchCode like case when @OutletCode = '' then '%%' else @OutletCode end
	  and not exists(select 1 
					   from #t1 d 
					  where d.DealerCode = a.CompanyCode
					    and d.OutletCode = a.BranchCode
					    and e.GroupCode = d.TipeKendaraan
					    and e.TypeCode = d.Variant)
	group by a.CompanyCode,b.DealerAbbreviation,a.BranchCode,c.OutletAbbreviation,e.GroupCode,e.TypeCode
	drop table #t1
END
else
begin
	Select * into #t3 from(
		Select distinct b.Area
			 , b.DealerCode
			 , b.DealerAbbreviation
			 , c.OutletCode
			 , c.OutletAbbreviation
			 , a.TipeKendaraan
			 , a.Variant
			 , a.InquiryDate
			 , a.SPKDate
			 , a.TestDrive
			 , a.Transmisi
			 , a.InquiryNumber
			 , a.LastProgress
		from pmKDP a
		left join GnMstDealerMapping b on a.CompanyCode = b.DealerCode
		left join GnMstDealerOutletMapping c on a.CompanyCode = c.DealerCode
			and a.BranchCode = c.OutletCode
		where convert(varchar,InquiryDate,112) <= convert(varchar,@EndDate,112)
		  and (b.Area like Case when @Area <> '' 
						then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
								then 'JABODETABEK'
								else @Area end
						else '%%' end
		   or b.Area like Case when @Area <> '' 
						then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
								then 'CABANG'
								else @Area end
						else '%%' end) 	
		  and b.DealerCode like case when @DealerCode = '' then '%%' else @DealerCode end
		  and c.OutletCode like case when @OutletCode = '' then '%%' else @OutletCode end
	)#t3
	
	--select * into #tempQty from(
	--	select f.CompanyCode
	--		 , e.BranchCode
	--		 , e.Year
	--		 , e.Month
	--		 , f.GroupCode
	--		 , f.TypeCode
	--		 , SUM(isnull(e.EndingOH,0)) EndingOH
	--	from OmMstModel f
	--	LEFT JOIN OmTrInventQtyVehicle e on f.CompanyCode = e.CompanyCode
	--	   and f.SalesModelCode = e.SalesModelCode
	--	where isnull(e.CompanyCode, '') <> ''
	--	  and isnull(e.BranchCode, '') <> ''
	--	  and e.Year = YEAR(@EndDate)
	--	  and e.Month = MONTH(@EndDate)
	--	group by f.CompanyCode
	--		 , e.BranchCode
	--		 , e.Year
	--		 , e.Month
	--		 , f.GroupCode
	--		 , f.TypeCode
	--)#tempQty
		
	select * into #tempQty from(
		select distinct f.CompanyCode
			 , e.BranchCode
			 , f.GroupCode
			 , f.TypeCode
			 , e.ColourCode
			 , (select TOP 1 b.EndingOH
			      from OmMstModel a
		     LEFT JOIN OmTrInventQtyVehicle b on a.CompanyCode = b.CompanyCode
				   and a.SalesModelCode = b.SalesModelCode
			    where a.CompanyCode = f.CompanyCode
			      and b.BranchCode = e.BranchCode
			      and b.Year = YEAR(@EndDate)
			      and b.Month = MONTH(@EndDate)
			      and a.GroupCode = f.GroupCode
			      and a.TypeCode = f.TypeCode
			      and b.ColourCode = e.ColourCode
			 order by a.CreatedDate,a.LastUpdateDate Desc) EndingOH
		from OmMstModel f
		LEFT JOIN OmTrInventQtyVehicle e on f.CompanyCode = e.CompanyCode
		   and f.SalesModelCode = e.SalesModelCode
		where isnull(e.CompanyCode, '') <> ''
		  and isnull(e.BranchCode, '') <> ''
		  and e.Year = YEAR(@EndDate)
		  and e.Month = MONTH(@EndDate)
	)#tempQty
-- Query Activation : uspfn_OmInquiryMKT '2013-03-12','2013-03-12','CABANG','',''

	Insert into @MainTable
	Select distinct a.DealerCode
		 , a.DealerAbbreviation
		 , a.OutletCode
		 , a.OutletAbbreviation
		 , a.TipeKendaraan
		 , a.Variant
		 , (select Count(*) 
			  from #t3 e
			  where convert(varchar,e.InquiryDate,112) < convert(varchar,@StartDate,112)
			    and a.Area = e.Area
			    and a.DealerCode = e.DealerCode
			    and a.OutletCode = e.OutletCode
			    and a.TipeKendaraan = e.TipeKendaraan
			    and a.Variant = e.Variant
			    and exists (select  1 
							  from pmKDP 
							 where InquiryNumber = e.InquiryNumber
							   and LastProgress in ('P','HP','SPK'))) OutsINQ
		 , (select Count(*) 
			  from #t3
			  where convert(varchar,InquiryDate,112) between convert(varchar,@StartDate,112) and convert(varchar,@EndDate,112)
			    and Area = a.Area
			    and DealerCode = a.DealerCode
			    and OutletCode = a.OutletCode
			    and TipeKendaraan = a.TipeKendaraan
			    and Variant = a.Variant) NewINQ
		 , (select COUNT(*) 
			  from #t3 
			 where DealerCode = a.DealerCode
			   and OutletCode = a.OutletCode
			   and convert(varchar,InquiryDate,112) < convert(varchar,@StartDate,112)
			   and TipeKendaraan = a.TipeKendaraan
			   and Variant = a.Variant
			   and LastProgress in ('SPK')) OutsSPK
		 , (select COUNT(*) 
			  from #t3 
			 where DealerCode = a.DealerCode
			   and OutletCode = a.OutletCode
			   and convert(varchar,InquiryDate,112) between convert(varchar,@StartDate,112) and convert(varchar,@EndDate,112)
			   and TipeKendaraan = a.TipeKendaraan
			   and Variant = a.Variant
			   and LastProgress in ('SPK')) NewSPK
		 , (select COUNT(*) 
			  from #t3 e
			 where e.DealerCode = a.DealerCode
			   and e.OutletCode = a.OutletCode
			   and convert(varchar,e.InquiryDate,112) <= convert(varchar,@EndDate,112)
			   and e.TipeKendaraan = a.TipeKendaraan
			   and e.Variant = a.Variant
			   and exists (select  1 
							  from pmKDP 
							 where InquiryNumber = e.InquiryNumber
							   and LastProgress in ('LOST')
							   and LostCaseCategory = 'D')) CancelSPK
		 , (select COUNT(*) 
			  from #t3 
			 where DealerCode = a.DealerCode
			   and OutletCode = a.OutletCode
			   and convert(varchar,InquiryDate,112) between convert(varchar,@StartDate,112) and convert(varchar,@EndDate,112)
			   and TipeKendaraan = a.TipeKendaraan
			   and Variant = a.Variant
			   and LastProgress in ('DO')) FakturPolisi
 		-- , (select isnull(SUM(f.EndingOH),0)
			--from #TempQty f
			--where f.CompanyCode = a.DealerCode
			--  and f.BranchCode = a.OutletCode
			--  and f.Year = YEAR(@EndDate)
			--  and f.Month = MONTH(@EndDate)
			--  and f.GroupCode = a.TipeKendaraan
			--  and f.TypeCode = a.Variant) Balance
		 , (select isnull(SUM(f.EndingOH),0)
					from #TempQty f
					where f.CompanyCode = a.DealerCode
					  and f.BranchCode = a.OutletCode
					  and f.GroupCode = a.TipeKendaraan
					  and f.TypeCode = a.Variant) Balance
		 , (select Count(*) 
			  from #t3
			  where convert(varchar,InquiryDate,112) between convert(varchar,@StartDate,112) and convert(varchar,@EndDate,112)
			    and Area = a.Area
			    and DealerCode = a.DealerCode
			    and OutletCode = a.OutletCode
			    and TipeKendaraan = a.TipeKendaraan
			    and Variant = a.Variant
			    and Transmisi = 'AT'
			    and TestDrive = '10') ATTestDrive
		, (select Count(*) 
			  from #t3
			  where convert(varchar,InquiryDate,112) between convert(varchar,@StartDate,112) and convert(varchar,@EndDate,112)
			    and Area = a.Area
			    and DealerCode = a.DealerCode
			    and OutletCode = a.OutletCode
			    and TipeKendaraan = a.TipeKendaraan
			    and Variant = a.Variant
			    and Transmisi = 'MT'
			    and TestDrive = '10') MTTestDrive
	from #t3 a
	UNION
	select a.CompanyCode
		 , b.DealerAbbreviation
		 , a.BranchCode
		 , c.OutletAbbreviation
		 , isnull(e.GroupCode,'') TipeKendaraan
		 , isnull(e.TypeCode,'') Variant
		 , 0
		 , 0 
		 , 0
		 , 0
		 , 0
		 , 0
		 , COUNT(e.SalesModelCode)
		 , 0
		 , 0
	from OmTrSalesReqDetail a
	left join OmMstVehicle f on a.CompanyCode = f.CompanyCode
		and a.ChassisCode = f.ChassisCode
		and a.ChassisNo = f.ChassisNo
	left join OmMstModel e on e.CompanyCode = a.CompanyCode
		and e.SalesModelCode = f.SalesModelCode
	left join gnMstDealerMapping b on b.DealerCode = a.CompanyCode
	left join gnMstDealerOutletMapping c on c.DealerCode = a.CompanyCode
		and c.OutletCode = a.BranchCode
	where Convert(varchar,a.FakturPolisiDate,112) between convert(varchar,@StartDate,112) and convert(varchar,@EndDate,112)
	  and (b.Area like Case when @Area <> '' 
					then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
							then 'JABODETABEK'
							else @Area end
					else '%%' end
	   or b.Area like Case when @Area <> '' 
					then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
							then 'CABANG'
							else @Area end
					else '%%' end) 	
	  and b.DealerCode like case when @DealerCode = '' then '%%' else @DealerCode end
	  and c.OutletCode like case when @OutletCode = '' then '%%' else @OutletCode end
	  and not exists(select 1 
					   from #t3 d 
					  where d.DealerCode = a.CompanyCode
					    and d.OutletCode = a.BranchCode
					    and e.GroupCode = d.TipeKendaraan
					    and e.TypeCode = d.Variant
					    and e.TransmissionType = d.Transmisi)
	group by a.CompanyCode, b.DealerAbbreviation, a.BranchCode, c.OutletAbbreviation, e.GroupCode, e.TypeCode,e.TransmissionType

	drop table #t3
end

if(@DealerCode = '' and @OutletCode = '')
begin
	select TipeKendaraan
		 , Variant
		 , SUM(OutsINQ) OutsINQ
		 , SUM(NewINQ) NewINQ
		 , SUM(OutsSPK) OutsSPK
		 , SUM(NewSPK) NewSPK
		 , SUM(CancelSPK) CancelSPK
		 , SUM(FakturPolisi) FakturPolisi
		 , SUM(Balance) Balance
		 , SUM(ATTestDrive) ATTestDrive
		 , SUM(MTTestDrive) MTTestDrive
	from @MainTable
	group by TipeKendaraan, Variant
	having (SUM(MTTestDrive) + SUM(ATTestDrive) + SUM(OutsINQ) + SUM(NewINQ) + SUM(OutsSPK) + SUM(NewSPK)  + SUM(FakturPolisi) + SUM(Balance)) > 0
	order by TipeKendaraan
end

select distinct
DealerCode
	 , DealerAbbreviation
	 , OutletCode
	 , OutletAbbreviation
from @MainTable
group by DealerCode, DealerAbbreviation, OutletCode, OutletAbbreviation, TipeKendaraan, Variant, OutsINQ , NewINQ , OutsSPK, NewSPK, CancelSPK, FakturPolisi , ATTestDrive, MTTestDrive
having (SUM(MTTestDrive) + SUM(ATTestDrive) + SUM(NewINQ) + SUM(NewSPK) + SUM(OutsINQ) + SUM(OutsSPK) + SUM(FakturPolisi) + SUM(Balance)) > 0
order by DealerAbbreviation, OutletAbbreviation

select DealerCode
	 , DealerAbbreviation
	 , OutletCode
	 , OutletAbbreviation
	 , TipeKendaraan
	 , Variant
	 , OutsINQ 
	 , NewINQ
	 , OutsSPK
	 , NewSPK
	 , CancelSPK
	 , FakturPolisi
	 , case when SUM(Balance) < 0 then 0 else SUM(Balance) end Balance
	 , ATTestDrive
	 , MTTestDrive
from @MainTable
group by DealerCode, DealerAbbreviation, OutletCode, OutletAbbreviation, TipeKendaraan, Variant, OutsINQ , NewINQ , OutsSPK, NewSPK, CancelSPK, FakturPolisi , ATTestDrive, MTTestDrive
having (SUM(MTTestDrive) + SUM(ATTestDrive) + SUM(NewINQ) + SUM(NewSPK) + SUM(OutsINQ) + SUM(OutsSPK) + SUM(FakturPolisi) + SUM(Balance)) > 0
order by DealerAbbreviation, OutletAbbreviation, TipeKendaraan, Variant


GO


