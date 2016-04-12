ALTER procedure [dbo].[usprpt_OmRpLabaRugi001] 
(
	@CompanyCode Varchar(20),
	@BranchCodeFrom	VARCHAR(15),
	@BranchCodeTo	VARCHAR(15),
	@DateStart Datetime,
	@DateEnd Datetime,
	@SalesType char(1),
	@SalesFrom Varchar(20),
	@SalesTo Varchar(20),
	@param Char(1),
	@pDok CHAR(1)
)
as

--declare	@CompanyCode as varchar(20),
--	@BranchCode as varchar(20),
--	@DateStart as datetime,
--	@DateEnd as datetime,
--	@SalesType as char(1),
--	@SalesFrom as varchar(20),
--	@SalesTo as varchar(20),
--	@param as char(1),
--	@pDok as char(1)
--
--set @CompanyCode = '6058401'
--set	@BranchCode = '605840100'
--set	@DateStart = '20110901'
--set	@DateEnd = '20111130'
--set	@SalesType = ''
--set	@SalesFrom = ''
--set	@SalesTo = ''
--set	@param = '1'
--set	@pDok = '0'

select * into #t1
from (
	select distinct
		a.CompanyCode,a.BranchCode
		, (SELECT CompanyName FROM gnMstCoprofile WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode) BranchName  
		, a.InvoiceNo,a.InvoiceDate, a.CustomerCode
		, e.SalesType, e.SalesCode, e.SalesCode + ' - ' + h.LookupValueName SalesName
		, (case when e.SalesType = '0' then 'W - ' + i.CustomerName else 'D - ' + i.CustomerName end) as CustomerName
		, (e.Salesman + ' - ' + 
			(select EmployeeName 
			from gnMstEmployee 
			where CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND EmployeeID = e.Salesman)
		) Salesman
		,(select top 1 DoNo from OmTrSalesDO f where f.CompanyCode = a.CompanyCode AND f.BranchCode = a.BranchCode AND f.SONo = e.SONo ) DONo
		,(select top 1 DoDate from OmTrSalesDO f where f.CompanyCode = a.CompanyCode AND f.BranchCode = a.BranchCode AND f.SONo = e.SONo ) DODate
		,e.SONo, e.SODate
		, d.SalesModelCode, d.SalesModelYear, d.ColourCode, d.ChassisCode, d.ChassisNo, d.EngineNo
		, 1 QtyUnit
		, isnull((d.COGSUnit + d.COGSOthers +d.COGSKaroseri),0) as COGS
		, isnull(b.AfterDiscDPP,0) AfterDiscDPP	
		, isnull((b.AfterDiscDPP - (d.COGSUnit + d.COGSOthers +d.COGSKaroseri)),0) as LabaRugi
		, isnull(((b.AfterDiscDPP - (d.COGSUnit + d.COGSOthers +d.COGSKaroseri)) / b.AfterDiscDPP),0) * 100 as Percentage		
		, (case @pDok when '0' then d.BPUDate when '1' then g.RefferenceDODate when '2' then g.RefferenceSJDate end) as BPUDate
		, datediff(day, (case @pDok when '0' then d.BPUDate when '1' then g.RefferenceDODate when '2' then g.RefferenceSJDate end), e.SODate) as Lama
		,'Invoice' TypeTrans
		, (select Top 1 CustomerName from GnMstCustomer where CustomerCode = e.LeasingCo) LeasingName
	from
		OmTrSalesInvoice a
		left join OmTrSalesInvoiceModel b on b.CompanyCode = a.CompanyCode and b.BranchCode = a.BranchCode and b.InvoiceNo = a.InvoiceNo
		left join OmTrSalesInvoiceVin c on c.CompanyCode = a.CompanyCode and c.BranchCode = a.BranchCode and c.InvoiceNo = a.InvoiceNo
			and c.BPKNo= b.BPKNo and c.SalesModelCode=b.SalesModelCode and c.SalesModelYear=b.SalesModelYear
		left join omMstVehicle d on d.CompanyCode = a.CompanyCode and d.SalesModelCode=c.SalesModelCode and d.SalesModelYear=c.SalesModelYear
			and d.ChassisCode=c.ChassisCode and d.ChassisNo=c.ChassisNo
		left join OmTrSalesSO e on e.CompanyCode = a.CompanyCode and e.BranchCode = a.BranchCode and e.SONo = a.SONo	
		left join OmTrPurchaseBPU g on g.CompanyCode = a.CompanyCode AND g.BranchCode = a.BranchCode AND g.PONo= d.PONo and g.BPUNo = d.BPUNo 
		left join GnMstLookUpDtl h on a.CompanyCode = h.CompanyCode and e.SalesCode = h.LookUpValue and h.CodeID = 'GPAR' 
		left join GnMstCustomer i on a.CompanyCode = i.CompanyCode and a.CustomerCode = i.CustomerCode
	where a.CompanyCode = @CompanyCode 
		AND (CASE WHEN @BranchCodeFrom = '' THEN '' ELSE a.BranchCode END) BETWEEN @BranchCodeFrom AND @BranchCodeTo 		
		and ((case when @param = '0' then convert(varchar, a.InvoiceDate, 112) end)= convert(varchar, a.InvoiceDate, 112)
			or (case when @param = '1' then convert(varchar, a.InvoiceDate, 112) end) between convert(varchar, @DateStart, 112) and convert(varchar, @DateEnd, 112))
		and ((case when @SalesType = '' then e.SalesType end) <> ''
			or (case when @SalesType <> '' then e.SalesType end) = @SalesType)
		and ((case when @SalesFrom = '' then e.SalesCode end) <> ''
			or (case when @SalesFrom <> '' then e.SalesCode end) between @SalesFrom and @SalesTo)
		and a.Status <> '3'
) #t1

insert into #t1
select distinct
	a.CompanyCode,a.BranchCode
	, (SELECT CompanyName FROM gnMstCoprofile WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode) BranchName  
	, a.ReturnNo,a.ReturnDate, a.CustomerCode
	, e.SalesType, e.SalesCode, e.SalesCode + ' - ' + h.LookupValueName SalesName
	, (case when e.SalesType = 0 then 'W - ' + i.CustomerName else 'D - ' + i.CustomerName end) as CustomerName
	, (e.Salesman + ' - ' + 
		(select EmployeeName
		from gnMstEmployee 
		where CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND EmployeeID = e.Salesman)
	) Salesman
	,j.InvoiceNo DONo
	,j.InvoiceDate DODate
	,e.SONo, e.SODate
	, d.SalesModelCode, d.SalesModelYear, d.ColourCode, d.ChassisCode, d.ChassisNo, d.EngineNo
	, -1 QtyUnit
	, isnull((d.COGSUnit + d.COGSOthers + d.COGSKaroseri),0) as COGS
	, isnull(b.AfterDiscDPP,0) AfterDiscDPP
	, isnull((b.AfterDiscDPP - (d.COGSUnit + d.COGSOthers +d.COGSKaroseri)),0) as LabaRugi
	, isnull(((b.AfterDiscDPP - (d.COGSUnit + d.COGSOthers +d.COGSKaroseri)) / b.AfterDiscDPP),0) * 100 as Percentage		
	, (case @pDok when '0' then d.BPUDate when '1' then g.RefferenceDODate when '2' then g.RefferenceSJDate end) as BPUDate
	, datediff(day, (case @pDok when '0' then d.BPUDate when '1' then g.RefferenceDODate when '2' then g.RefferenceSJDate end), e.SODate) as Lama
	,'Retur' TypeTrans
	, (select Top 1 CustomerName from GnMstCustomer where CustomerCode = e.LeasingCo) LeasingName
	
from omTrSalesReturn a
	inner join omTrSalesReturnDetailModel b on a.CompanyCode=b.CompanyCode and a.BranchCode=b.BranchCode
		and a.ReturnNo=b.ReturnNo
	inner join omTrSalesReturnVin c on a.CompanyCode=c.CompanyCode and a.BranchCode=c.BranchCode and a.ReturnNo=c.ReturnNo 
		and c.BPKNo= b.BPKNo and b.SalesModelCode=c.SalesModelCode and b.SalesModelYear=c.SalesModelYear
	left join omMstVehicle d on d.CompanyCode = a.CompanyCode and d.SalesModelCode=c.SalesModelCode and d.SalesModelYear=c.SalesModelYear
		and d.ChassisCode=c.ChassisCode and d.ChassisNo=c.ChassisNo
	left join omTrSalesInvoice j on a.CompanyCode=j.CompanyCode and a.BranchCode=j.BranchCode and a.InvoiceNo=j.InvoiceNo
	left join OmTrSalesSO e on e.CompanyCode = a.CompanyCode and e.BranchCode = a.BranchCode and e.SONo = j.SONo	
	left join OmTrPurchaseBPU g on g.CompanyCode = a.CompanyCode AND g.BranchCode = a.BranchCode AND g.PONo= d.PONo and g.BPUNo = d.BPUNo 
	left join GnMstLookUpDtl h on a.CompanyCode = h.CompanyCode and e.SalesCode = h.LookUpValue and h.CodeID = 'GPAR' 
	left join GnMstCustomer i on a.CompanyCode = i.CompanyCode and a.CustomerCode = i.CustomerCode
where a.CompanyCode = @CompanyCode 
	AND (CASE WHEN @BranchCodeFrom = '' THEN '' ELSE a.BranchCode END) BETWEEN @BranchCodeFrom AND @BranchCodeTo 		
	and ((case when @param = '0' then convert(varchar, a.ReturnDate, 112) end)= convert(varchar, a.ReturnDate, 112)
		or (case when @param = '1' then convert(varchar, a.ReturnDate, 112) end) between convert(varchar, @DateStart, 112) and convert(varchar, @DateEnd, 112))
	and ((case when @SalesType = '' then e.SalesType end) <> ''
		or (case when @SalesType <> '' then e.SalesType end) = @SalesType)
	and ((case when @SalesFrom = '' then e.SalesCode end) <> ''
		or (case when @SalesFrom <> '' then e.SalesCode end) between @SalesFrom and @SalesTo)
	and a.Status <> '3'

select * from #t1

drop table #t1
	
