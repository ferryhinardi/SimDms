
/****** Object:  StoredProcedure [dbo].[uspRpt_SlReportSalesNStock]    Script Date: 9/24/2014 8:24:59 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[uspRpt_SlReportSalesNStock] @Groupno varchar(50), @CompanyCode varchar(50), @BranchCode varchar(50), @Groupmodel varchar(50), @ModelType varchar(50), @Year int, @debug int=0, @month int=0
as
--declare @Year int=2014
if @debug=1 begin 
-----------------------------------------------------------------------For Test--------------------------------------------------------------------
	select x.Cabang, x.Model, 
		--All Month debud For Sales And Stock
		sum(x.Sales) as Sales, 
		sum(x.stock) as Stock
from (
		select e.CompanyName cabang , (substring(b.SalesModelCode,1,2) +'/ ' + b.SalesModelCode) as Model, 
					SUM(CASE WHEN Month(a.InvoiceDate) = @month AND Year(a.InvoiceDate) = @year THEN 1 ELSE 0 END) AS [Sales],		
		0 Stock
		from omTrSalesInvoice a 
						Inner join omTrSalesInvoiceVin b
							on a.CompanyCode = b.CompanyCode and a.BranchCode = b.BranchCode and a.InvoiceNo = b.InvoiceNo
						inner join CompaniesGroupMappingView c
							on a.CompanyCode=c.DealerCode 
						inner join gnMstCoProfile e
							on a.CompanyCode=e.CompanyCode and a.BranchCode=e.BranchCode
						
		where 1=1	and Year(a.InvoiceDate)=@year
					and isnull(a.InvoiceNo,'')<>''
					and isnull(a.InvoiceDate,'')<>''
					and a.Status in ('2', '5')
					and c.groupno=case when isnull(@groupno,'')<>'' then @groupno else c.groupno end
					and a.CompanyCode=case when isnull(@CompanyCode,'')<>'' then @CompanyCode else a.CompanyCode end
					and a.BranchCode =case when isnull(@BranchCode,'')<>'' then @BranchCode else a.BranchCode end
					and substring(b.SalesModelCode,1,2)= case when isnull(@Groupmodel,'')<>'' then @Groupmodel else substring(b.SalesModelCode,1,2) end
					and b.SalesModelCode = case when isnull(@ModelType,'')<>'' then @ModelType else b.SalesModelCode end
					group by datename(month, a.invoicedate), month(a.invoicedate), e.CompanyGovName, e.CompanyName, b.SalesModelCode	
		union all
			select '' cabang,'' model,0 Sales,
				sum(case when Month=@month then EndingAV else 0 end)  as Stock
			from omTrInventQtyVehicle a
				 inner join CompaniesGroupMappingView c
						on a.CompanyCode=c.DealerCode 
			where 1=1
			and c.groupno=case when isnull(@groupno,'')<>'' then @groupno else c.groupno end
			and CompanyCode=case when isnull(@CompanyCode,'')<>'' then @CompanyCode else CompanyCode end
			and BranchCode=case when isnull(@BranchCode,'')<>'' then @BranchCode else BranchCode end
			and substring(SalesModelCode,1,2)= case when isnull(@Groupmodel,'')<>'' then @Groupmodel else substring(SalesModelCode,1,2) end
			and SalesModelCode = case when isnull(@ModelType,'')<>'' then @ModelType else SalesModelCode end
			and Year=@year 
			group by CompanyCode, BranchCode, SalesModelCode, Year) x
where isnull(cabang,'')<>''
group by cabang, model
order by cabang, model
-----------------------------------------------------------------------For Real--------------------------------------------------------------------
end else begin
	select x.Cabang, x.Model, 
		--Januari
		sum(x.Sales1) as Sales1, 
		sum(x.stock1) as Stock1,
		case when	((sum(x.Sales1) + sum(x.Salesmin1) + sum(x.Salesmin2))/3) <> 0 then
					cast(sum(x.stock1)/((sum(x.Sales1) + sum(x.Salesmin1) + sum(x.Salesmin2))/3) as decimal(5,1)) 
					else 0 end as StockRatio1,
		--Februari
		sum(x.Sales2) as Sales2, 
		sum(x.stock2) as Stock2,
		case when	((sum(x.Sales2) + sum(x.Sales1) + sum(x.Salesmin1))/3) <> 0 then
					cast(sum(x.stock2)/((sum(x.Sales2) + sum(x.Sales1) + sum(x.Salesmin1))/3) as decimal(5,1)) 
					else 0 end as StockRatio2,
		--Maret
		sum(x.Sales3) as Sales3, 
		sum(x.stock3) as Stock3,
		case when	((sum(x.Sales3) + sum(x.Sales2) + sum(x.Sales1))/3) <> 0 then
					cast(sum(x.stock3)/((sum(x.Sales3) + sum(x.Sales2) + sum(x.Sales1))/3) as decimal(5,1)) 
					else 0 end as StockRatio3,
		--April
		sum(x.Sales4) as Sales4, 
		sum(x.stock4) as Stock4,
		case when	((sum(x.Sales4) + sum(x.Sales3) + sum(x.Sales2))/3) <> 0 then
					cast(sum(x.stock4)/((sum(x.Sales4) + sum(x.Sales3) + sum(x.Sales2))/3) as decimal(5,1)) 
					else 0 end as StockRatio4,
		--Mei
		sum(x.Sales5) as Sales5, 
		sum(x.stock5) as Stock5,
		case when	((sum(x.Sales5) + sum(x.Sales4) + sum(x.Sales3))/3) <> 0 then
					cast(sum(x.stock5)/((sum(x.Sales5) + sum(x.Sales4) + sum(x.Sales3))/3) as decimal(5,1)) 
					else 0 end as StockRatio5,
		--Juni
		sum(x.Sales6) as Sales6, 
		sum(x.stock6) as Stock6,
		case when	((sum(x.Sales6) + sum(x.Sales5) + sum(x.Sales4))/3) <> 0 then
					cast(sum(x.stock6)/((sum(x.Sales6) + sum(x.Sales5) + sum(x.Sales4))/3) as decimal(6,1)) 
					else 0 end as StockRatio6,
		--JuLi
		sum(x.Sales7) as Sales7, 
		sum(x.stock7) as Stock7,
		case when	((sum(x.Sales7) + sum(x.Sales6) + sum(x.Sales5))/3) <> 0 then
					cast(sum(x.stock7)/((sum(x.Sales7) + sum(x.Sales6) + sum(x.Sales5))/3) as decimal(5,1)) 
					else 0 end as StockRatio7,
		--Agustus
		sum(x.Sales8) as Sales8, 
		sum(x.stock8) as Stock8,
		case when	((sum(x.Sales8) + sum(x.Sales7) + sum(x.Sales6))/3) <> 0 then
					cast(sum(x.stock8)/((sum(x.Sales8) + sum(x.Sales7) + sum(x.Sales6))/3) as decimal(5,1)) 
					else 0 end as StockRatio8,
		--September
		sum(x.Sales9) as Sales9, 
		sum(x.stock9) as Stock9,
		case when	((sum(x.Sales9) + sum(x.Sales8) + sum(x.Sales7))/3) <> 0 then
					cast(sum(x.stock9)/((sum(x.Sales9) + sum(x.Sales8) + sum(x.Sales7))/3) as decimal(5,1)) 
					else 0 end as StockRatio9,

		--Oktober
		sum(x.Sales10) as Sales10, 
		sum(x.stock10) as Stock10,
		case when	((sum(x.Sales10) + sum(x.Sales9) + sum(x.Sales8))/3) <> 0 then
					cast(sum(x.stock10)/((sum(x.Sales10) + sum(x.Sales9) + sum(x.Sales8))/3) as decimal(5,1)) 
					else 0 end as StockRatio10,
		--November
		sum(x.Sales11) as Sales11, 
		sum(x.stock11) as Stock11,
		case when	((sum(x.Sales11) + sum(x.Sales10) + sum(x.Sales9))/3) <> 0 then
					cast(sum(x.stock11)/((sum(x.Sales11) + sum(x.Sales10) + sum(x.Sales9))/3) as decimal(5,1)) 
					else 0 end as StockRatio11,
		--NDecember
		sum(x.Sales12) as Sales12, 
		sum(x.stock12) as Stock12,
		case when	((sum(x.Sales12) + sum(x.Sales11) + sum(x.Sales10))/3) <> 0 then
					cast(sum(x.stock12)/((sum(x.Sales12) + sum(x.Sales11) + sum(x.Sales10))/3) as decimal(5,1)) 
					else 0 end as StockRatio12

from (
		select		(c.CompanyName)  as cabang , b.SalesModelCode as Model, --(substring(b.SalesModelCode,1,2) +'/ ' + b.SalesModelCode) as Model, 
					SUM(CASE WHEN Month(a.InvoiceDate) = 11 AND Year(a.InvoiceDate) = @year-1 THEN 1 ELSE 0 END) AS [Salesmin1],
					SUM(CASE WHEN Month(a.InvoiceDate) = 12 AND Year(a.InvoiceDate) = @year-1 THEN 1 ELSE 0 END) AS [Salesmin2],
					SUM(CASE WHEN Month(a.InvoiceDate) = 1 AND Year(a.InvoiceDate) = @year THEN 1 ELSE 0 END) AS [Sales1],
					SUM(CASE WHEN Month(a.InvoiceDate) = 2 AND Year(a.InvoiceDate) = @year THEN 1 ELSE 0 END) AS [Sales2],
					SUM(CASE WHEN Month(a.InvoiceDate) = 3 AND Year(a.InvoiceDate) = @year THEN 1 ELSE 0 END) AS [Sales3],
					SUM(CASE WHEN Month(a.InvoiceDate) = 4 AND Year(a.InvoiceDate) = @year THEN 1 ELSE 0 END) AS [Sales4],
					SUM(CASE WHEN Month(a.InvoiceDate) = 5 AND Year(a.InvoiceDate) = @year THEN 1 ELSE 0 END) AS [Sales5],
					SUM(CASE WHEN Month(a.InvoiceDate) = 6 AND Year(a.InvoiceDate) = @year THEN 1 ELSE 0 END) AS [Sales6],
					SUM(CASE WHEN Month(a.InvoiceDate) = 7 AND Year(a.InvoiceDate) = @year THEN 1 ELSE 0 END) AS [Sales7],
					SUM(CASE WHEN Month(a.InvoiceDate) = 8 AND Year(a.InvoiceDate) = @year THEN 1 ELSE 0 END) AS [Sales8],
					SUM(CASE WHEN Month(a.InvoiceDate) = 9 AND Year(a.InvoiceDate) = @year THEN 1 ELSE 0 END) AS [Sales9],
					SUM(CASE WHEN Month(a.InvoiceDate) = 10 AND Year(a.InvoiceDate) = @year THEN 1 ELSE 0 END) AS [Sales10],
					SUM(CASE WHEN Month(a.InvoiceDate) = 11 AND Year(a.InvoiceDate) = @year THEN 1 ELSE 0 END) AS [Sales11],
					SUM(CASE WHEN Month(a.InvoiceDate) = 12 AND Year(a.InvoiceDate) = @year THEN 1 ELSE 0 END) AS [Sales12],
		0 Stock1,0 Stock2,0 Stock3,0 Stock4,0 Stock5,0 Stock6,0 Stock7,0 Stock8,0 Stock9,0 Stock10,0 Stock11,0 Stock12
		from omTrSalesInvoice a 
						Inner join omTrSalesInvoiceVin b
							on a.CompanyCode = b.CompanyCode and a.BranchCode = b.BranchCode and a.InvoiceNo = b.InvoiceNo
						inner join (select CompanyCode, BranchCode, a.CompanyName ,groupno from gnMstCoProfile a 
									inner join CompaniesGroupMappingView b
										on a.CompanyCode=b.DealerCode ) c
							on a.CompanyCode=c.CompanyCode and a.BranchCode=c.BranchCode
						
		where 1=1	and Year(a.InvoiceDate)=@year
					and isnull(a.InvoiceNo,'')<>''
					and isnull(a.InvoiceDate,'')<>''
					and a.Status in ('2', '5')
					and c.groupno=case when isnull(@groupno,'')<>'' then @groupno else c.groupno end
					and a.CompanyCode=case when isnull(@CompanyCode,'')<>'' then @CompanyCode else a.CompanyCode end
					and a.BranchCode =case when isnull(@BranchCode,'')<>'' then @BranchCode else a.BranchCode end
					and substring(b.SalesModelCode,1,2)= case when isnull(@Groupmodel,'')<>'' then @Groupmodel else substring(b.SalesModelCode,1,2) end
					and b.SalesModelCode = case when isnull(@ModelType,'')<>'' then @ModelType else b.SalesModelCode end
					group by c.CompanyName, b.SalesModelCode
		-------------------------------------------------------------------------------------------------------------------------------
		------------------------------------------------------union--------------------------------------------------------------------
		-------------------------------------------------------------------------------------------------------------------------------	
		union all
			select b.CompanyName cabang, SalesModelCode model,
			0 Salesmin1,0 Salesmin2,0 sales1,0 sales2,0 sales3,0 sales4,0 sales5,0 sales6,0 sales7,0 sales8,0 sales9,0 sales10,0 sales11,0 sales12,
				sum(case when Month=1 then EndingAV else 0 end)  as Stock1,
				sum(case when Month=2 then EndingAV else 0 end)  as Stock2,
				sum(case when Month=3 then EndingAV else 0 end)  as Stock3,
				sum(case when Month=4 then EndingAV else 0 end)  as Stock4,
				sum(case when Month=5 then EndingAV else 0 end)  as Stock5,
				sum(case when Month=6 then EndingAV else 0 end)  as Stock6,
				sum(case when Month=7 then EndingAV else 0 end)  as Stock7,
				sum(case when Month=8 then EndingAV else 0 end)  as Stock8,
				sum(case when Month=9 then EndingAV else 0 end)  as Stock9,
				sum(case when Month=10 then EndingAV else 0 end)  as Stock10,
				sum(case when Month=11 then EndingAV else 0 end)  as Stock11,
				sum(case when Month=12 then EndingAV else 0 end)  as Stock12
			from omTrInventQtyVehicle a
					inner join (select CompanyCode, BranchCode, a.CompanyName ,groupno from gnMstCoProfile a 
									inner join CompaniesGroupMappingView b
										on a.CompanyCode=b.DealerCode ) b
							on a.CompanyCode=b.CompanyCode and a.BranchCode=b.BranchCode
			where 1=1
			and b.groupno=case when isnull(@groupno,'')<>'' then @groupno else b.groupno end
			and a.CompanyCode=case when isnull(@CompanyCode,'')<>'' then @CompanyCode else a.CompanyCode end
			and a.BranchCode=case when isnull(@BranchCode,'')<>'' then @BranchCode else a.BranchCode end
			and substring(SalesModelCode,1,2)= case when isnull(@Groupmodel,'')<>'' then @Groupmodel else substring(SalesModelCode,1,2) end
			and SalesModelCode = case when isnull(@ModelType,'')<>'' then @ModelType else SalesModelCode end
			and Year=@year 
			group by b.CompanyName, SalesModelCode
		) x
where isnull(cabang,'')<>''
group by cabang, model
order by cabang, model
end




			-- from omTrSalesInvoice a 
			--	Inner join omTrSalesInvoiceVin b
			--		on a.CompanyCode = b.CompanyCode and a.BranchCode = b.BranchCode and a.InvoiceNo = b.InvoiceNo
			--	Inner join omMstModel c
			--		on c.SalesModelCode = b.SalesModelCode 
			--	Inner join gnMstCoProfile d
			--		on a.CompanyCode=d.CompanyCode
			--	Inner join SuzukiR4..msMstGroupModel e 
			--		on c.GroupCode=e.GroupModel
			--where 1=1 
			--and isnull(a.InvoiceNo,'')<>''
			--and isnull(a.InvoiceDate,'')<>''
			--and a.Status in ('2', '5')
			--and a.CompanyCode=case when isnull(@CompanyCode,'')<>'' then @CompanyCode else a.CompanyCode end
			--and a.BranchCode =case when isnull(@BranchCode,'')<>'' then @BranchCode else a.BranchCode end
			--and e.GroupModel = case when isnull(@Groupmodel,'')<>'' then @Groupmodel else e.GroupModel end
			--and b.SalesModelCode = case when isnull(@ModelType,'')<>'' then @ModelType else b.SalesModelCode end
			--group by d.companyname, e.GroupModel
			--order by d.companyname, e.GroupModel
			


			

					
