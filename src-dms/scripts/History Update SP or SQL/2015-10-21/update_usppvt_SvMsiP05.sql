ALTER procedure [dbo].[usppvt_SvMsiP05]
--declare
   @CompanyCode varchar(15),
   @BranchCode  varchar(15),
   @StartDate   datetime,
   @EndDate     datetime
as  

--set @CompanyCode='6058401'
--set @BranchCode='605840100'
--set @StartDate='20151001'
--set @EndDate='20151031'

begin  

set nocount on;  

declare @t1 as table 
(
	RecNo        int,
	FieldName    varchar(100),
	Caption      varchar(100),
	FormatString varchar(100),
	IsVisible    bit,
	Width        int,
	HAlignment   int,        -- {0:Default,1:Near,2:Center,3:Far}
	Area         int,        -- {0:Row,1:Column,2:Filter,3:Data}
	PivotSummaryType   int,  -- {0:Count,1:Sum,2:Min,3:Max,4:Average,5:StdDev,6:StdDevp,7:Var,8:Varp,9:Custom}
	PivotGroupInterval int   -- {0:Default,1:Date,2:DateDay,3:DateDayOfWeek,4:DateDayOfYear,5:DateWeekOfMonth}
							 -- {6:DateWeekOfYear,7:DateMonth,8:DateQuarter,9:DateYear,10:YearAge,11:MonthAge}
							 -- {12:WeekAge,13:DayAge,14:Alphabetical,15:Numeric,16:Hour,17:Custom}
)

insert into @t1 values(00,'JobType','JobType','',1,110,0,2,0,0)
insert into @t1 values(01,'GroupPart','Group Sublet','',1,110,0,0,0,0)
insert into @t1 values(02,'TypeOfGoodDesc','Group TPGO','',1,110,0,0,0,0)
insert into @t1 values(03,'GroupJobType','Group','',1,110,0,0,0,0)
insert into @t1 values(07,'InvoiceNo','No Invoice','',1,110,0,2,0,0)
insert into @t1 values(09,'InvoiceDate','Tgl Invoice','dd-MMM-yyy HH:mm',1,120,0,2,0,0)
insert into @t1 values(11,'Foreman','Foreman','',1,180,0,2,0,0)
insert into @t1 values(12,'SA','SA','',1,180,0,2,0,0)
insert into @t1 values(13,'Mechanic','Mechanic','',1,110,0,2,0,0)
insert into @t1 values(20,'PartAmount','Sparepart Amt','',1,110,0,3,1,0)

select * from @t1 order by RecNo

declare @t_mek1 as table(InvoiceNo varchar(20), Mechanic varchar(80))
declare @t_mek2 as table(InvoiceNo varchar(20), Mechanic varchar(80))

insert into @t_mek1
select a.InvoiceNo, isnull(b.MechanicID, '') Mechanic
  from svTrnInvoice a
  left join svTrnInvMechanic b
    on b.CompanyCode = a.CompanyCode
   and b.BranchCode = a.BranchCode
   and b.InvoiceNo = a.InvoiceNo
  left join gnMstEmployee c
    on c.CompanyCode = b.CompanyCode
   and c.BranchCode = b.BranchCode
   and c.EmployeeID = b.MechanicID
 where a.CompanyCode = @CompanyCode
   and a.BranchCode = @BranchCode
   and convert(varchar, a.InvoiceDate, 112)
       between convert(varchar, @StartDate, 112) 
       and convert(varchar, @EndDate, 112)
  group by a.InvoiceNo, b.MechanicID

insert into @t_mek2
select InvoiceNo 
     , max(case seq when 1 then EmployeeName else '' end) 
     + max(case seq when 2 then  ', ' + EmployeeName else '' end) 
     + max(case seq when 3 then  ', ' + EmployeeName else '' end) 
     + max(case seq when 4 then  ', ' + EmployeeName else '' end) 
     + max(case seq when 5 then  ', ' + EmployeeName else '' end) 
  from (
	select a.InvoiceNo, a.Mechanic
	     , (select count(*)
	          from @t_mek1 b
	          where b.InvoiceNo = a.InvoiceNo
	            and b.Mechanic <= a.Mechanic)
	  from @t_mek1 a           
	) D (InvoiceNo, EmployeeName, seq)
 group by InvoiceNo

--select * from @t_mek2

select a.JobType, a.InvoiceNo, a.InvoiceDate
     , (ceiling((b.SupplyQty - b.ReturnQty) * b.RetailPrice * (100.0 - b.DiscPct) * 0.01)) PartAmount
     , b.TypeOfGoods, c.LookupValueName as TypeOfGoodName, c.ParaValue as TypeOfGoodDesc
     --, case when TypeofGoods in ('0','1') then 'SGP-GROUP' else 'SUBLET-GROUP' end GroupPart						
     , case when TypeofGoods ='0' then 'SGP-GROUP'						-- Perubahan
		when TypeofGoods = '1' then 'SGO-GROUP'							-- Perubahan
		when TypeofGoods = '2' then 'SGA-ACCECORIES-GROUP'				-- Perubahan 
		when TypeofGoods in ('5','6','7','8') then 'SGA-MATERIAL-GROUP'	-- Perubahan 
		else 'SUBLET-GROUP' end GroupPart	-- Perubahan
     , case 
         when left(a.InvoiceNo, 3) = 'INI' or a.JobType = 'PDI' then '03. INTERNAL, PDI'
         when left(a.InvoiceNo, 3) = 'INW' or left(a.InvoiceNo, 3) = 'INF' then '02. CLAIM, FSC1'
         else '01. CPUS'
       end GroupJobType
     , e.EmployeeName as Foreman
     , f.EmployeeName as SA
     , g.Mechanic
  from svTrnInvoice a
 inner join svTrnInvItem b
    on b.CompanyCode = a.CompanyCode
   and b.BranchCode = a.BranchCode
   and b.InvoiceNo = a.InvoiceNo
  left join gnMstLookupDtl c
    on c.CompanyCode = a.CompanyCode
   and c.CodeID = 'GTGO'
   and c.LookupValue = b.TypeOfGoods
  left join SvTrnService d
    on d.CompanyCode = a.CompanyCode
   and d.BranchCode = a.BranchCode
   and d.JobOrderNo = a.JobOrderNo
  left join GnMstEmployee e
    on e.CompanyCode = d.CompanyCode
   and e.BranchCode = d.BranchCode
   and e.EmployeeID = d.MechanicID
  left join GnMstEmployee f
    on f.CompanyCode = d.CompanyCode
   and f.BranchCode = d.BranchCode
   and f.EmployeeID = d.ForemanID
  left join @t_mek2 g
    on g.InvoiceNo = a.InvoiceNo
 where a.CompanyCode = @CompanyCode
   and a.BranchCode = @BranchCode
   and convert(varchar, a.InvoiceDate, 112)
       between convert(varchar, @StartDate, 112) 
       and convert(varchar, @EndDate, 112)

end

