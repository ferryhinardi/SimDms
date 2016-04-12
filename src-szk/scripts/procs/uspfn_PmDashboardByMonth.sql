alter procedure uspfn_PmDashboardByDay
	@Periode varchar(10)

as

select DashDate, InqValue, SpkValue, FakturValue
  from PmDashboardByDay
 where left(DashDate, 6) = @Periode

go


exec uspfn_PmDashboardByDay '201405'

