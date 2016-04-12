
ALTER procedure [dbo].[usppvt_SvMsiP44]
   @CompanyCode varchar(15),  
   @BranchCode  varchar(15),  
   @StartDate   datetime,  
   @EndDate     datetime  
as    
  
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
  
insert into @t1 values(00,'FscGroup','FscGroup','',1,120,0,0,0,0)  
insert into @t1 values(01,'ChassisCode','Chassis Code','',1,110,0,0,0,0)  
insert into @t1 values(02,'ChassisNo','Chassis No','',1,110,0,2,0,0)  
insert into @t1 values(03,'Odometer','Odometer','',1,120,0,2,0,0)  
insert into @t1 values(04,'InvoiceDate','Invoice Date','',1,110,0,2,0,0)  
insert into @t1 values(10,'ChassisCode','Unit','',1,110,0,3,0,0)  
  
select * from @t1 order by RecNo  
  
declare @t_unit_fsc_clm as table
(
	InvoiceDate  varchar(20),
	ChassisCode  varchar(50), 
	ChassisNo    varchar(50),
	GroupJobType varchar(50),
	JobType      varchar(50),
	Odometer     numeric(10),
	InvoiceNo	varchar(50)
)	
insert into @t_unit_fsc_clm
select distinct convert(varchar, a.InvoiceDate, 112) InvoiceDate
	 , a.ChassisCode, convert(varchar, a.ChassisNo) ChassisNo
	 , b.GroupJobType, a.JobType
	 , b.WarrantyOdometer as Odometer
	 , a.InvoiceNo
  from svTrnInvoice a
 inner join svMstJob b
	on b.CompanyCode = a.CompanyCode
   and b.ProductType = a.ProductType
   and b.BasicModel = a.BasicModel
   and b.JobType = a.JobType
 where a.CompanyCode = @CompanyCode
   and a.BranchCode = @BranchCode
   and convert(varchar, InvoiceDate, 112)  
       between convert(varchar, @StartDate, 112)   
       and convert(varchar, @EndDate, 112)  
   and b.GroupJobType in ('FSC')
   and not exists (
	select 1 from svTrnInvoice
	 where 1 = 1
	   and CompanyCode = a.CompanyCode
	   and BranchCode = a.BranchCode
	   and ChassisCode = a.ChassisCode
	   and ChassisNo = a.ChassisNo
	   and convert(varchar, InvoiceDate, 112) = convert(varchar, a.InvoiceDate, 112)
	   --and JobType = 'PDI'
	   and a.JobType = 'PDI'
   )

select InvoiceDate, ChassisCode, ChassisNo, GroupJobType, Odometer
     , FscGroup = GroupJobType + convert(varchar, Odometer)
  from @t_unit_fsc_clm 
  -- add condition
  where InvoiceNo like 'INF%'
  
end

GO


