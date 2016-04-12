go
if object_id('uspfn_SvChartMonitoringByPeriode') is not null
	drop procedure uspfn_SvChartMonitoringByPeriode

go
create procedure uspfn_SvChartMonitoringByPeriode
	@Area varchar(5),
	@Dealer varchar(25),
	@Outlet varchar(25),
	@Year int,
	@Month int
as
begin
	if @Area is null or @Area = ''
	begin
		set @Area = '%';
	end

	if @Dealer is null or @Dealer = ''
	begin
		set @Dealer = '%';
	end

	if @Outlet is null or @Outlet = ''
	begin
		set @Outlet = '%';
	end

	declare @Start varchar(6);
	declare @End varchar(6);

	set @Start = convert(varchar(6), @Year) + right(convert(varchar(3), '01'), 2);
	set @End = convert(varchar(6), @Year) + right(convert(varchar(3), '0' + convert(varchar(2), @Month)), 2);

	select * into #t_temp
	from (
		select [Day] = right(convert(varchar(8), a.JobOrderClosed, 112), 2)
			 , [Periode] = convert(varchar(6), a.JobOrderClosed, 112)
			 , LastUpdateDate
			 --, [DataCount] = count(a.JobOrderClosed)
		  from SvUnitIntake a
		 where a.AreaCode like @Area
		   and a.CompanyCode like @Dealer
		   and a.BranchCode like @Outlet
		   and a.JobOrderClosed is not null
		   and convert(varchar(6), a.JobOrderClosed, 112) = @End 
		 --group by convert(varchar(6), a.JobOrderClosed, 112)
		 --order by right(convert(varchar(6), a.JobOrderClosed, 112), 2) asc
	) as temp;

	select Day = a.Day
	     , DataCount = count(a.Day)
	  from #t_temp a
	 group by a.Day
	 order by a.Day asc

	select top 1 
		   a.LastUpdateDate
	  from #t_temp a
	 order by a.LastUpdateDate desc;

end

go
exec uspfn_SvChartMonitoringByPeriode '200', '', '', 2014, 6

