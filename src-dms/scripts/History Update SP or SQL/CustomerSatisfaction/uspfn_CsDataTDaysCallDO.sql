--uspfn_CsDataTDaysCallDO '6006400131', 2015, 9
ALTER procedure [dbo].[uspfn_CsDataTDaysCallDO]
	@BranchCode varchar(50),
	@Year        int,
	@Month       int 

as

;with r as (
select a.CompanyCode
     , a.BranchCode
	 , convert(datetime, a.DODate) as DoDate
	 , count(*) as DoData
  from CsCustomerVehicleView a
 where a.BranchCode = @BranchCode
   and a.DODate is not null
   and convert(varchar(7), a.DODate, 121) = convert(varchar, @Year) + '-' + right('0' + convert(varchar, @Month), 2)
 group by a.CompanyCode, a.BranchCode, convert(datetime, a.DODate)
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
			   join CsTDayCall b
				 on b.CompanyCode	= a.CompanyCode
				and b.CustomerCode = a.CustomerCode
				and b.Chassis		= a.Chassis
	          where a.CompanyCode = r.CompanyCode
			    and a.BranchCode = r.BranchCode
				and convert(datetime, a.DoDate) = r.DoDate
				and convert(varchar, b.CreatedDate, 121)
					between convert(varchar, @Year) + '-' + right('0' + convert(varchar, @Month), 2) + '-' + '01'
						and convert(varchar(7), dateadd(month, 1, b.CreatedDate), 121) + '07'
					--and convert(varchar(7), CreatedDate, 121) = convert(varchar(4),@year) + '-' + convert(varchar(2), @month)
			 ), 0)
 from r
), t as (

select s.CompanyCode
     , s.DoMonth
	 , s.BranchCode
	 , sum(s.DoData) as DoData
	 , sum(s.TDaysCallData) as TDaysCallData
  from s
  where s.BranchCode = @BranchCode
   and Year(s.DoDate) = @Year
   and Month(s.DoDate) = @Month
 group by s.CompanyCode, s.BranchCode, s.DoMonth
)

select b.DealerCode CompanyCode
     , t.DoMonth
	 , b.OutletCode BranchCode
	 , isnull(DoData, 0) DoData
	 , isnull((select count(i.CustomerCode) from CsCustomerVehicleView i
			    where i.BranchCode = @BranchCode and i.DeliveryDate is not null
				and convert(varchar(7), i.DoDate, 121) = convert(varchar, @Year) + '-' + right('0' + convert(varchar, @Month), 2)		
		),0) DeliveryDate
	 , isnull(TDaysCallData, 0) TDaysCallData
	 , b.OutletAbbreviation BranchName
	 , TDaysCallByInput = isnull((
			 select count(x.CustomerCode) from CsLkuTDaysCallView x
			 where x.Outstanding = 'N'
			   and x.BranchCode = @BranchCode
			   and Year(x.DeliveryDate) = @Year
			   and Month(x.DeliveryDate) = @Month
	  --     select count(x.Chassis) 
		 --    from CsTDayCall x
			-- left join CsCustomerVehicleView y
			--   on y.CompanyCode = x.CompanyCode
			--  and y.CustomerCode = x.CustomerCode
			--  and y.Chassis = x.Chassis
			--where 1 = 1
			--  and y.BranchCode = @BranchCode
			--  and Year(x.CreatedDate) = @Year
			--  and Month(x.CreatedDate) = @Month
			), 0)
 from t
 right join gnMstDealerOutletMapping b
    on b.DealerCode = t.CompanyCode
   and b.OutletCode = t.BranchCode
 where b.OutletCode = @BranchCode
 order by CompanyCode, DoMonth, BranchCode

--select * from SysDealer where DealerCode = @BranchCode and TableName = 'CsTDayCall'
select getdate() LastUpdate

