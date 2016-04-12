alter procedure uspfn_CsDataTDaysCallDO
	@CompanyCode varchar(50),
	@Year        int,
	@Month       int 

as

;with r as (
select a.CompanyCode
     , a.BranchCode
	 , convert(date, a.DODate) as DoDate
	 , count(*) as DoData
  from CsCustomerVehicleView a
 where 1 = 1
   and a.DODate is not null
 group by a.CompanyCode, a.BranchCode, convert(date, a.DODate)
)
, s as (
select CompanyCode
     , BranchCode
	 , Month(DoDate) as DoMonth
	 , DoDate
	 , DoData 
	 , TDaysCallData = isnull((
	         select count(a.Chassis)
			   from CsCustomerVehicleView a
	          where a.CompanyCode = r.CompanyCode
			    and a.BranchCode = r.BranchCode
				and convert(date, a.DoDate) = r.DoDate
				and exists (
				    select top 1 1 from CsTDayCall
					 where CompanyCode = a.CompanyCode
					   and CustomerCode = a.CustomerCode
					   and Chassis = a.Chassis
				)
			 ), 0)
 from r
)

select s.CompanyCode
     , s.DoMonth
	 , s.BranchCode
	 , b.BranchName
	 , sum(s.DoData) as DoData
	 , sum(s.TDaysCallData) as TDaysCallData
	 , TDaysCallByInput = isnull((
	       select count(x.Chassis) 
		     from CsTDayCall x
			 left join CsCustomerVehicleView y
			   on y.CompanyCode = x.CompanyCode
			  and y.CustomerCode = x.CustomerCode
			  and y.Chassis = x.Chassis
			where x.CompanyCode = @CompanyCode
			  and y.BranchCode = s.BranchCode
			  and Year(x.CreatedDate) = @Year
			  and Month(x.CreatedDate) = @Month
			), 0)
  from s
  join OutletInfo b
    on b.CompanyCode = s.CompanyCode
   and b.BranchCode = s.BranchCode
 where s.CompanyCode = @CompanyCode
   and Year(s.DoDate) = @Year
   and Month(s.DoDate) = @Month
 group by s.CompanyCode, s.BranchCode, b.BranchName, s.DoMonth
 order by CompanyCode, DoMonth, BranchCode

select * from SysDealer where DealerCode = @CompanyCode and TableName = 'CsTDayCall'

go

--uspfn_CsChartMonitoring '3DaysCall', '2014-06-01', '2014-06-06'
--select count(*) from CsTDayCall where CompanyCode = '6021406' and convert(varchar, CreatedDate, 112) like '201406%'
select * from SysDealer where DealerCode = '6021406' and TableName = 'CsTDayCall'

go
uspfn_CsDataTDaysCallDO '6021406', '2014', '6'


--select * from CsCustBpkb
