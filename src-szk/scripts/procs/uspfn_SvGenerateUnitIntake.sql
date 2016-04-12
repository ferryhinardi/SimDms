alter procedure uspfn_SvGenerateUnitIntake
	@Area     varchar(50),
	@Dealer   varchar(50),
	@Outlet   varchar(50),
	@DateFrom date,
	@DateTo   date
        
as

select a.*, b.BranchName
  from SvUnitIntake a
  left join OutletInfo b
    on b.CompanyCode = a.CompanyCode
   and b.BranchCode = a.BranchCode
 where 1 = 1
   --and a.AreaCode = (case when (@Area = '' or @Dealer != '' or @Outlet != '') then a.Area else @Area end)
   and a.AreaCode = (case when (@Area = '') then a.AreaCode else @Area end)
   and a.CompanyCode = (case when (@Dealer = '' or @Outlet != '') then a.CompanyCode else @Dealer end)
   and a.BranchCode = (case @Outlet when '' then a.BranchCode else @Outlet end)
   and a.JobOrderClosed >= @DateFrom
   and convert(date, a.JobOrderClosed) <= @DateTo

go

exec uspfn_SvGenerateUnitIntake @Area=N'105',@Dealer=N'',@Outlet=N'',@DateFrom=N'2014-08-20',@DateTo=N'2014-09-02'

--uspfn_SvChartRegisterSpk1 '2014-01-01', '2014-06-06', 'JABODETABEK', '6021406'
--uspfn_SvChartRegisterSpk1 '2014-01-01', '2014-06-06'
