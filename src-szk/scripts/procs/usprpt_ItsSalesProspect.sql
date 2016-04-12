/****** Object:  StoredProcedure [dbo].[uspRpt_ItsSalesProspect]    Script Date: 8/29/2014 10:15:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[uspRpt_ItsSalesProspect] @groupno varchar(50), @CompanyCode varchar(50), @BranchCode varchar(50), @Groupmodel varchar(50), @ModelType varchar(50), @year int
as

select case month
			when 1 then 'Jan'  
			when 2 then 'Feb' 
			when 3 then 'Mar' 
			when 4 then 'Apr' 
			when 5 then 'May' 
			when 6 then 'Jun' 
			when 7 then 'Jul' 
			when 8 then 'Aug' 
			when 9 then 'Sep' 
			when 10 then 'Oct' 
			when 11 then 'Nov' 
			when 12 then 'Dec' 
			end as Bulan, sum(new) as Newer, sum([carry over]) as CarryOver, sum(Hpnew) as HpNewer, sum([Hpcarry over]) as HpCarryOver, Sum(sales) as Sales, sum(lost) as Lost, 
			
		cast (case when sum([total P]) > 0 then 
		cast ( sum(Hpnew)*100.0/sum([total P]) as decimal(12,1)) 
			else 0 end as varchar(50)) NewHpProspect,

		cast (case when sum([total P]) > 0 then 
		cast (Sum(sales)*100.0/sum([total P]) as decimal(12,1)) 
			else 0 end as varchar(50)) SalesProspect,

		cast (case when sum([total HP]) > 0 then 
		cast (Sum(sales)*100.0/sum([total HP]) as decimal(12,1)) 
			else 0 end as varchar(50)) SalesHProspect,
			
		k,
		sum(BrandRelated) BrandRelated,
		sum (Price) Price,
		sum (ProductFeature) ProductFeature,
		sum(HigherDiscount) HigherDiscount,
		sum(UnitAvailabilty) UnitAvailabilty,
		sum(BetterService) BetterService,
		sum(Others) Others
 from (select month(UpdateDate) as month,
		sum(case when a.LastProgress='p' and month(a.UpdateDate) = month(b.InquiryDate) then 1 else 0 end) as new,
		sum(case when a.LastProgress='p' and month(a.UpdateDate) <> month(b.InquiryDate) then 1 else 0 end) as [carry over],
		sum(case when a.LastProgress='p' then 1 else 0 end) as [total P],
		sum(case when a.LastProgress='hp' and month(a.UpdateDate) = month(b.InquiryDate) then 1 else 0 end) as Hpnew,
		sum(case when a.LastProgress='hp' and month(a.UpdateDate) <> month(b.InquiryDate) then 1 else 0 end) as [Hpcarry over],
		sum(case when a.LastProgress='hp' then 1 else 0 end) as [total HP],
		'' sales,
		sum(case when a.LastProgress='Lost' then 1 else 0 end) as [Lost],
		'' K,
		sum(case when b.LostCaseReasonID=10 and a.LastProgress='Lost' then 1 else 0 end) BrandRelated,
		sum(case when b.LostCaseReasonID=20 and a.LastProgress='Lost' then 1 else 0 end) Price,
		sum(case when b.LostCaseReasonID=30 and a.LastProgress='Lost' then 1 else 0 end) ProductFeature,
		sum(case when b.LostCaseReasonID=40 and a.LastProgress='Lost' then 1 else 0 end) HigherDiscount,
		sum(case when b.LostCaseReasonID=50 and a.LastProgress='Lost' then 1 else 0 end) UnitAvailabilty,
		sum(case when b.LostCaseReasonID=60 and a.LastProgress='Lost' then 1 else 0 end) BetterService,
		sum(case when b.LostCaseReasonID=70 and a.LastProgress='Lost' then 1 else 0 end) Others
			from pmStatusHistory a
			inner join pmHstITS b
				on a.CompanyCode=b.CompanyCode and a.BranchCode=b.BranchCode and a.InquiryNumber=b.InquiryNumber
			inner join CompaniesGroupMappingView c
				on a.CompanyCode=c.DealerCode
			where 1=1 
			and c.GroupNo = case when isnull(@GroupNo,'')<>'' then @GroupNo else c.GroupNo end
			and a.CompanyCode = case when isnull(@CompanyCode,'')<>'' then @CompanyCode else a.CompanyCode end
			and a.BranchCode = case when isnull(@BranchCode,'')<>'' then @BranchCode else a.BranchCode end
			and substring(b.TipeKendaraan,1,2)=case when isnull(@Groupmodel,'')<>'' then @Groupmodel else substring(b.TipeKendaraan,1,2) end
			and replace((b.TipeKendaraan+b.Variant),' ','')=case when isnull(@ModelType,'')<>'' then @ModelType else replace((b.TipeKendaraan+b.Variant),' ','') end
			and a.LastProgress in ('p','hp','lost')
			and Year(UpdateDate)=@year
			group by month(UpdateDate)
			union
select month(a.InvoiceDate) 
			 as month, 
			 0 as new,
			0 as [carry over],
			0 as [total P],
			0 as Hpnew,
			0 as [Hpcarry over],
			0 as [total HP],
			count(a.InvoiceNo) [Sales],
			0 as [Lost],
			0 K,
			0 BrandRelated,
			0 Price,
			0 ProductFeature,
			0 HigherDiscount,
			0 UnitAvailabilty,
			0 BetterService,
			0 Others
				from omTrSalesInvoice a 
				Inner join omTrSalesInvoiceVin b
					on a.CompanyCode = b.CompanyCode and a.BranchCode = b.BranchCode and a.InvoiceNo = b.InvoiceNo
				inner join CompaniesGroupMappingView c
					on a.CompanyCode=c.DealerCode 
				where 1=1 
				and Year(invoicedate)=@year
				and a.Status=5
				and c.GroupNo = case when isnull(@GroupNo,'')<>'' then @GroupNo else c.GroupNo end
				and a.CompanyCode = case when isnull(@CompanyCode,'')<>'' then @CompanyCode else a.CompanyCode end
				and a.BranchCode = case when isnull(@BranchCode,'')<>'' then @BranchCode else a.BranchCode end
				and substring(b.SalesModelCode,1,2) = case when isnull(@Groupmodel,'')<>'' then @Groupmodel else substring(b.SalesModelCode,1,2) end 
				and b.SalesModelCode = case when isnull(@ModelType,'')<>'' then @ModelType else b.SalesModelCode end
				group by month(InvoiceDate)
				) x
				group by MONTH, k
				order by MONTH

--

----test month(a.UpdateDate)=month(b.InquiryDate)
--select a.UpdateDate, b.InquiryDate 
--from pmStatusHistory a
--inner join pmHstITS b
--	on a.CompanyCode=b.CompanyCode
--	and a.BranchCode=b.BranchCode
--	and a.InquiryNumber=b.InquiryNumber
--where month(UpdateDate)=1 
--and year(UpdateDate)=2014
--and a.LastProgress='lost' and month(a.UpdateDate)=month(b.InquiryDate) order by UpdateDate --129165

----test month(a.UpdateDate) tidak sama dengan month(b.InquiryDate)
--select a.UpdateDate, b.InquiryDate 
--from pmStatusHistory a
--inner join pmHstITS b
--	on a.CompanyCode=b.CompanyCode
--	and a.BranchCode=b.BranchCode
--	and a.InquiryNumber=b.InquiryNumber
--where month(UpdateDate)=1 
--and year(UpdateDate)=2014
--and a.LastProgress='p' and month(a.UpdateDate)<>month(b.InquiryDate) order by UpdateDate --129165

--select * from pmStatusHistory a
----inner join pmHstITS b
----	on a.UpdateDate=b.InquiryDate
--where month(UpdateDate)=1 
--and year(UpdateDate)=2014
--and a.LastProgress='p' 
			


			

					
