IF(OBJECT_ID('uspfn_CsDataTDaysCallDO') is not null)
	drop proc dbo.uspfn_CsDataTDaysCallDO
GO

/****** Object:  StoredProcedure [dbo].[uspfn_CsDataTDaysCallDO]    Script Date: 8/27/2015 11:19:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--uspfn_CsDataTDaysCallDO '6006400131', 2015, 2
CREATE procedure [dbo].[uspfn_CsDataTDaysCallDO]
	@BranchCode varchar(50),
	@Year        int,
	@Month       int 

as

;with r as (
select a.CompanyCode
     , a.BranchCode
	 , convert(date, a.DODate) as DoDate
	 , count(*) as DoData
  from CsCustomerVehicleView a
 where a.BranchCode = @BranchCode
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
	 , b.OutletAbbreviation BranchName
	 , sum(s.DoData) as DoData
	 , sum(s.TDaysCallData) as TDaysCallData
	 , TDaysCallByInput = isnull((
	       select count(x.Chassis) 
		     from CsTDayCall x
			 left join CsCustomerVehicleView y
			   on y.CompanyCode = x.CompanyCode
			  and y.CustomerCode = x.CustomerCode
			  and y.Chassis = x.Chassis
			where 1 = 1
			  and y.BranchCode = @BranchCode
			  and Year(x.CreatedDate) = @Year
			  and Month(x.CreatedDate) = @Month
			), 0)
  from s
  join gnMstDealerOutletMapping b
    on b.DealerCode = s.CompanyCode
   and b.OutletCode = s.BranchCode
 where s.BranchCode = @BranchCode
   and Year(s.DoDate) = @Year
   and Month(s.DoDate) = @Month
 group by s.CompanyCode, s.BranchCode, b.OutletAbbreviation, s.DoMonth
 order by CompanyCode, DoMonth, BranchCode

--select * from SysDealer where DealerCode = @BranchCode and TableName = 'CsTDayCall'
select getdate() LastUpdate
