alter procedure uspfn_PmDashboardByDay2
	@Periode1   varchar(10),
	@Periode2   varchar(10),
	@GroupModel varchar(20) = 'ALL'

as

select left(DashDate, 6) as Periode, DashDate, GroupModel, InqValue, SpkValue, FakturValue
  from PmDashboardByDay
 where 1 = 1
   and GroupModel = @GroupModel
   and left(DashDate, 6) between @Periode1 and @Periode2
  order by Periode desc, DashDate

go

exec uspfn_PmDashboardByDay2 '201404', '201405', 'ALL'


