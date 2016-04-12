
-- =============================================
-- Author:		fhy
-- Create date: 30122015
-- Description:	CsDataTDaysCallDO export
-- =============================================
CREATE PROCEDURE [dbo].[uspfn_CsDataTDaysCallDOexport]
	@BranchCode varchar(50),
	@Periode    varchar(50),
	@Year        int,
	@Month       int 
AS
BEGIN

declare @t_CsDataTDaysCallDO as table
(
	[Kode Dealer] varchar(25),
	[Bulan DO] varchar(25),
	[Kode Outlet] varchar(25),
	[Data DO       ] int,
	[Delivered     ] int,
	[3 Days Call (By DO)] int,
	[Nama Outlet                             ]   varchar(150),
	[3 Days Call (By Input)]  int
)

declare @t_CsDataTDaysCallDOrpt as table
(
	[Periode          ] varchar(50),
	[Nama Outlet                             ]   varchar(150),
	[Data DO       ] int,
	[Delivered     ] int,
	[3 Days Call (By DO)] int,
	[3 Days Call (By Input)]  int
)

;with r as (
select a.CompanyCode
     , a.BranchCode
	 , convert(datetime, a.DODate) as DoDate
	 , count(*) as DoData
  from CsCustomerVehicleView a
 where 
 a.BranchCode = @BranchCode
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
  where 
  s.BranchCode = @BranchCode
   and Year(s.DoDate) = @Year
   and Month(s.DoDate) = @Month
 group by s.CompanyCode, s.BranchCode, s.DoMonth
)

insert into @t_CsDataTDaysCallDO
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
 where 
 b.OutletCode = @BranchCode
 order by CompanyCode, DoMonth, BranchCode
 
 select [Kode Outlet],[Nama Outlet                             ] from @t_CsDataTDaysCallDO

 insert into @t_CsDataTDaysCallDOrpt
 select 
	@Periode as  [Periode          ],
	[Nama Outlet                             ],
	[Data DO       ],
	[Delivered     ],
	[3 Days Call (By DO)],
	[3 Days Call (By Input)] 
 from @t_CsDataTDaysCallDO

 select * from @t_CsDataTDaysCallDOrpt
 union All
 select
	'Total' 
	,''
	,ISNULL([Data DO       ],0)
	,ISNULL([Delivered     ],0)
	,ISNULL([3 Days Call (By DO)],0)
	,ISNULL([3 Days Call (By Input)] ,0)
 from @t_CsDataTDaysCallDOrpt
 
delete @t_CsDataTDaysCallDO
delete @t_CsDataTDaysCallDOrpt

END


GO


