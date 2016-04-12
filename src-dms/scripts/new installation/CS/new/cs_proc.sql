USE [SimDmsDealerDemo]
GO
/****** Object:  StoredProcedure [dbo].[uspfn_CsCustLeasingList]    Script Date: 12/13/2013 9:22:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspfn_CsCustLeasingList](
	@CompanyCode varchar(17)
)
as
select 
	text = a.CustomerName,
	value = a.CustomerCode
from 
	gnMstCustomer a
where
	a.CompanyCode=@CompanyCode
	and
	a.CategoryCode='32'
	

GO
/****** Object:  StoredProcedure [dbo].[uspfn_CsDashSummary]    Script Date: 12/13/2013 9:22:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[uspfn_CsDashSummary]
	@CompanyCode varchar(20),
	@BranchCode varchar(20)
as

--set @CompanyCode = '6006406'

declare @CurrDate datetime, @Param1 as varchar(20)
declare @t_rem as table
(
	RemCode varchar(20),
	RemDate datetime,
	RemValue int
)

set @CurrDate = getdate()

-- REM3DAYSCALL
set @Param1 = isnull((select SettingParam1 from CsSettings where CompanyCode = @CompanyCode and SettingCode = 'REM3DAYSCALL'), '0')
insert into @t_rem (RemCode, RemDate) values('REM3DAYSCALL', left(convert(varchar, dateadd(month, -convert(int, @Param1), @CurrDate), 112), 6) + '01')

-- REMBDAYSCALL
set @Param1 = isnull((select SettingParam1 from CsSettings where CompanyCode = @CompanyCode and SettingCode = 'REMBDAYSCALL'), '0')
insert into @t_rem (RemCode, RemDate) values('REMBDAYSCALL', left(convert(varchar, dateadd(month, -convert(int, @Param1), @CurrDate), 112), 6) + '01')

-- REMHOLIDAYS
set @Param1 = isnull((select SettingParam1 from CsSettings where CompanyCode = @CompanyCode and SettingCode = 'REMHOLIDAYS'), '0')
insert into @t_rem (RemCode, RemDate) values('REMHOLIDAYS', left(convert(varchar, dateadd(month, -convert(int, @Param1), @CurrDate), 112), 6) + '01')

-- REMSTNKEXT
set @Param1 = isnull((select SettingParam1 from CsSettings where CompanyCode = @CompanyCode and SettingCode = 'REMSTNKEXT'), '0')
insert into @t_rem (RemCode, RemDate) values('REMSTNKEXT', left(convert(varchar, dateadd(month, -convert(int, @Param1), @CurrDate), 112), 6) + '01')

-- REMBPKB
set @Param1 = isnull((select SettingParam1 from CsSettings where CompanyCode = @CompanyCode and SettingCode = 'REMBPKB'), '0')
insert into @t_rem (RemCode, RemDate) values('REMBPKB', left(convert(varchar, dateadd(month, -convert(int, @Param1), @CurrDate), 112), 6) + '01')
   
declare @CurrentMonth tinyint;
declare @PreviousMonth tinyint;
declare @NextMonth tinyint;
declare @CurrentDay tinyint;

set @CurrentDay = datepart(day, getdate());
set @CurrentMonth = DATEPART(month, getdate());
if @CurrentMonth = 1 
	set @PreviousMonth=12
else
	set @PreviousMonth=@CurrentMonth-1;
if @CurrentMonth = 12 
	set @NextMonth=1
else
	set @NextMonth=@CurrentMonth+1;

update @t_rem set RemValue = (select count(CustomerCode) from CsLkuTDayCallView where CompanyCode = @CompanyCode and BranchCode = @BranchCode and OutStanding = 'Y' and DODate >= (select RemDate from @t_rem where RemCode = 'REM3DAYSCALL')) where RemCode = 'REM3DAYSCALL'
update @t_rem set RemValue = (select count(CustomerCode) from CsLkuStnkExtView where CompanyCode = @CompanyCode and BranchCode = @BranchCode and OutStanding = 'Y' and StnkExpiredDate >= (select RemDate from @t_rem where RemCode = 'REMSTNKEXT')) where RemCode = 'REMSTNKEXT'
update @t_rem set RemValue = (select count(CustomerCode) from CsLkuBpkbView where CompanyCode = @CompanyCode and BranchCode = @BranchCode and OutStanding = 'Y' and BpkbDate >= (select RemDate from @t_rem where RemCode = 'REMBPKB')) where RemCode = 'REMBPKB'
update @t_rem set RemValue = (select count(CustomerCode) from CsLkuBirthdayView where CompanyCode = @CompanyCode and BranchCode = @BranchCode and OutStanding = 'Y' ) where RemCode = 'REMBDAYSCALL'

select a.RemCode, a.RemDate, a.RemValue, b.SettingLink1 as ControlLink
  from @t_rem a
  join CsSettings b
    on b.CompanyCode = @CompanyCode
   and b.SettingCode = a.RemCode



GO
/****** Object:  StoredProcedure [dbo].[uspfn_CsGetBpkb]    Script Date: 12/13/2013 9:22:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[uspfn_CsGetBpkb]
	@CompanyCode  varchar(20),
	@CustomerCode varchar(20),
	@Chassis      varchar(50)
as

--select @CompanyCode = '6006406', @CustomerCode = '1000581', @Chassis = 'MHYGDN42VBJ352996'
select a.CompanyCode     
	 , a.BranchCode
	 , b.CustomerCode
	 , c.CustomerName
	 , a.ChassisCode + convert(varchar, a.ChassisNo) Chassis
	 , a.EngineCode + convert(varchar, a.EngineNo) Engine
	 , a.SalesModelCode
	 , a.SalesModelYear
	 , a.ColourCode
	 , isnull(e.BpkbDate, g.BPKDate) BpkbDate
	 , isnull(e.StnkDate, g.BPKDate) StnkDate
	 , g.BPKDate
	 , h.isLeasing as IsLeasing
	 , h.LeasingCo
	 , isnull(h.isLeasing, 0) as IsLeasing
	 , case isnull(h.isLeasing, 0) when 0 then 'Tunai' else 'Leasing' end as Category
	 , i.CustomerName as LeasingName
	 , h.Installment
	 , convert(varchar, isnull(h.Installment, 0)) + ' Month' as Tenor
     , left(c.Address1, 40) as Address
     , c.PhoneNo
     , h.Salesman
     , j.EmployeeName as SalesmanName
     , k.BpkbReadyDate
     , k.BpkbPickUp
     , k.ReqInfoLeasing
     , k.ReqInfoCust
     , k.ReqKtp
     , k.ReqStnk
     , k.ReqSuratKuasa
     , k.Comment, k.Additional, k.Status
     , (case k.Status when 1 then 'Finish' else 'In Progress' end) as StatusInfo
     , case (isnull(k.Chassis, '')) when '' then 1 else 0 end as IsNew
     , l.PoliceRegNo
  from omTrSalesInvoiceVin a    
  left join omTrSalesInvoice b    
	on b.CompanyCode = a.CompanyCode    
   and b.BranchCode = a.BranchCode    
   and b.InvoiceNo = a.InvoiceNo    
  left join gnMstCustomer c
	on c.CompanyCode = a.CompanyCode    
   and c.CustomerCode = b.CustomerCode
  left join omTrSalesDODetail d
	on d.CompanyCode = a.CompanyCode    
   and d.BranchCode = a.BranchCode
   and d.ChassisCode = a.ChassisCode
   and d.ChassisNo = a.ChassisNo
  left join CsCustomerVehicle e
	on e.CompanyCode = a.CompanyCode    
   and e.Chassis = a.ChassisCode + convert(varchar, a.ChassisNo)
  left join omTrSalesDO f
	on f.CompanyCode = d.CompanyCode    
   and f.BranchCode = d.BranchCode
   and f.DONo = d.DONo
  left join omTrSalesBPK g
	on g.CompanyCode = f.CompanyCode    
   and g.BranchCode = f.BranchCode
   and g.DONo = f.DONo
  left join omTrSalesSO h
	on h.CompanyCode = g.CompanyCode    
   and h.BranchCode = g.BranchCode
   and h.SONo = g.SONo
  left join gnMstCustomer i
	on i.CompanyCode = h.CompanyCode
   and i.CustomerCode = h.LeasingCo
  left join HrEmployee j
	on j.CompanyCode = h.CompanyCode
   and j.EmployeeID = h.Salesman
  left join CsCustBpkb k
	on k.CompanyCode = a.CompanyCode
   and k.CustomerCode = b.CustomerCode
   and k.Chassis = a.ChassisCode + convert(varchar, a.ChassisNo)
  left join svMstCustomerVehicle l
	on l.CompanyCode = a.CompanyCode
   and l.ChassisCode = a.ChassisCode
   and l.ChassisNo = a.ChassisNo
 where 1 = 1
   and a.CompanyCode = @CompanyCode
   and b.CustomerCode = @CustomerCode
   and a.ChassisCode + convert(varchar, a.ChassisNo) = @Chassis
   

GO
/****** Object:  StoredProcedure [dbo].[uspfn_CsGetFeedback]    Script Date: 12/13/2013 9:22:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[uspfn_CsGetFeedback]
	@CompanyCode  varchar(20),
	@CustomerCode varchar(20),
	@Chassis      varchar(50)
as

--select @CompanyCode = '6006406', @CustomerCode = '1000581', @Chassis = 'MHYGDN42VBJ352996'
select a.CompanyCode     
	 , a.BranchCode
	 , b.CustomerCode
	 , c.CustomerName
	 , a.ChassisCode + convert(varchar, a.ChassisNo) Chassis
	 , a.EngineCode + convert(varchar, a.EngineNo) Engine
	 , a.SalesModelCode
	 , a.SalesModelYear
	 , a.ColourCode
	 , isnull(e.BpkbDate, g.BPKDate) BpkbDate
	 , isnull(e.StnkDate, g.BPKDate) StnkDate
	 , g.BPKDate
	 , h.isLeasing as IsLeasing
	 , h.LeasingCo
	 , isnull(h.isLeasing, 0) as IsLeasing
	 , case isnull(h.isLeasing, 0) when 0 then 'Tunai' else 'Leasing' end as Category
	 , i.CustomerName as LeasingName
	 , h.Installment
	 , convert(varchar, isnull(h.Installment, 0)) + ' Month' as Tenor
     , left(c.Address1, 40) as Address
     , c.PhoneNo
     , h.Salesman
     , j.EmployeeName as SalesmanName
     , k.IsManual
     , k.FeedbackA
     , k.FeedbackB
     , k.FeedbackC
     , k.FeedbackD
     , case (isnull(k.Chassis, '')) when '' then 1 else 0 end as IsNew
     , l.PoliceRegNo
  from omTrSalesInvoiceVin a    
  left join omTrSalesInvoice b    
	on b.CompanyCode = a.CompanyCode    
   and b.BranchCode = a.BranchCode    
   and b.InvoiceNo = a.InvoiceNo    
  left join gnMstCustomer c
	on c.CompanyCode = a.CompanyCode    
   and c.CustomerCode = b.CustomerCode
  left join omTrSalesDODetail d
	on d.CompanyCode = a.CompanyCode    
   and d.BranchCode = a.BranchCode
   and d.ChassisCode = a.ChassisCode
   and d.ChassisNo = a.ChassisNo
  left join CsCustomerVehicle e
	on e.CompanyCode = a.CompanyCode    
   and e.Chassis = a.ChassisCode + convert(varchar, a.ChassisNo)
  left join omTrSalesDO f
	on f.CompanyCode = d.CompanyCode    
   and f.BranchCode = d.BranchCode
   and f.DONo = d.DONo
  left join omTrSalesBPK g
	on g.CompanyCode = f.CompanyCode    
   and g.BranchCode = f.BranchCode
   and g.DONo = f.DONo
  left join omTrSalesSO h
	on h.CompanyCode = g.CompanyCode    
   and h.BranchCode = g.BranchCode
   and h.SONo = g.SONo
  left join gnMstCustomer i
	on i.CompanyCode = h.CompanyCode
   and i.CustomerCode = h.LeasingCo
  left join HrEmployee j
	on j.CompanyCode = h.CompanyCode
   and j.EmployeeID = h.Salesman
  left join CsCustFeedback k
	on k.CompanyCode = a.CompanyCode
   and k.CustomerCode = b.CustomerCode
   and k.Chassis = a.ChassisCode + convert(varchar, a.ChassisNo)
  left join svMstCustomerVehicle l
	on l.CompanyCode = a.CompanyCode
   and l.ChassisCode = a.ChassisCode
   and l.ChassisNo = a.ChassisNo
 where 1 = 1
   and a.CompanyCode = @CompanyCode
   and b.CustomerCode = @CustomerCode
   and a.ChassisCode + convert(varchar, a.ChassisNo) = @Chassis
   

GO
/****** Object:  StoredProcedure [dbo].[uspfn_CsGetStnkExt]    Script Date: 12/13/2013 9:22:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[uspfn_CsGetStnkExt]
	@CompanyCode  varchar(20),
	@CustomerCode varchar(20),
	@Chassis      varchar(50)
as

--select @CompanyCode = '6006406', @CustomerCode = '1000581', @Chassis = 'MHYGDN42VBJ352996'
select a.CompanyCode     
	 , a.BranchCode
	 , b.CustomerCode
	 , c.CustomerName
	 , a.ChassisCode + convert(varchar, a.ChassisNo) Chassis
	 , a.EngineCode + convert(varchar, a.EngineNo) Engine
	 , a.SalesModelCode
	 , a.SalesModelYear
	 , a.ColourCode
	 , isnull(e.BpkbDate, g.BPKDate) BpkbDate
	 , isnull(e.StnkDate, g.BPKDate) StnkDate
	 , g.BPKDate
	 , h.isLeasing as IsLeasing
	 , h.LeasingCo
	 , isnull(h.isLeasing, 0) as IsLeasing
	 , case isnull(h.isLeasing, 0) when 0 then 'Tunai' else 'Leasing' end as Category
	 , i.CustomerName as LeasingName
	 , h.Installment
	 , convert(varchar, isnull(h.Installment, 0)) + ' Month' as Tenor
     , left(c.Address1, 40) as Address
     , c.PhoneNo
     , h.Salesman
     , j.EmployeeName as SalesmanName
     , k.IsStnkExtend
     , isnull(k.StnkExpiredDate, dateadd(year, 1, isnull(e.StnkDate, g.BPKDate)))  as StnkExpiredDate
     , k.ReqKtp
     , k.ReqStnk
     , k.ReqBpkb
     , k.ReqSuratKuasa
     , k.Comment, k.Additional, k.Status
     , (case k.Status when 1 then 'Finish' else 'In Progress' end) as StatusInfo
     , case (isnull(k.Chassis, '')) when '' then 1 else 0 end as IsNew
     , l.PoliceRegNo
  from omTrSalesInvoiceVin a    
  left join omTrSalesInvoice b    
	on b.CompanyCode = a.CompanyCode    
   and b.BranchCode = a.BranchCode    
   and b.InvoiceNo = a.InvoiceNo    
  left join gnMstCustomer c
	on c.CompanyCode = a.CompanyCode    
   and c.CustomerCode = b.CustomerCode
  left join omTrSalesDODetail d
	on d.CompanyCode = a.CompanyCode    
   and d.BranchCode = a.BranchCode
   and d.ChassisCode = a.ChassisCode
   and d.ChassisNo = a.ChassisNo
  left join CsCustomerVehicle e
	on e.CompanyCode = a.CompanyCode    
   and e.Chassis = a.ChassisCode + convert(varchar, a.ChassisNo)
  left join omTrSalesDO f
	on f.CompanyCode = d.CompanyCode    
   and f.BranchCode = d.BranchCode
   and f.DONo = d.DONo
  left join omTrSalesBPK g
	on g.CompanyCode = f.CompanyCode    
   and g.BranchCode = f.BranchCode
   and g.DONo = f.DONo
  left join omTrSalesSO h
	on h.CompanyCode = g.CompanyCode    
   and h.BranchCode = g.BranchCode
   and h.SONo = g.SONo
  left join gnMstCustomer i
	on i.CompanyCode = h.CompanyCode
   and i.CustomerCode = h.LeasingCo
  left join HrEmployee j
	on j.CompanyCode = h.CompanyCode
   and j.EmployeeID = h.Salesman
  left join CsStnkExt k
	on k.CompanyCode = a.CompanyCode
   and k.CustomerCode = b.CustomerCode
   and k.Chassis = a.ChassisCode + convert(varchar, a.ChassisNo)
  left join svMstCustomerVehicle l
	on l.CompanyCode = a.CompanyCode
   and l.ChassisCode = a.ChassisCode
   and l.ChassisNo = a.ChassisNo
 where 1 = 1
   and a.CompanyCode = @CompanyCode
   and b.CustomerCode = @CustomerCode
   and a.ChassisCode + convert(varchar, a.ChassisNo) = @Chassis
   

GO
/****** Object:  StoredProcedure [dbo].[uspfn_CsInqBday]    Script Date: 12/13/2013 9:22:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE proc [dbo].[uspfn_CsInqBday]
@CompanyCode varchar(20),
@DateFrom datetime,
@DateTo datetime
as
begin
select CompanyCode = a.CompanyCode
     , a.CustomerCode
	 --, PeriodOfYear = c.PeriodYear
  --   , CompanyName = b.CompanyName
     , a.CustomerName
     , CustomerAddress = a.Address1 + ' ' + rtrim(a.Address2) + ' ' + rtrim(a.Address3) + ' ' + rtrim(a.Address4)
  --   , CustomerTelephone = a.PhoneNo
  --   , CustomerBirthDate = a.BirthDate
	 --, CustomerTypeOfGift = c.TypeOfGift
	 --, CustomerGiftSentDate = c.SentGiftDate
	 --, CustomerGiftReceivedDate = c.ReceivedGiftDate
	 ----, SpouseGiftSentDate = (
		----select top 1
		----	x.SentGiftDate
		----from
		----	CsCustRelation x
		----where
		----	x.CompanyCode = a.CompanyCode
		----	and
		----	x.CustomerCode = a.CustomerCode
		----	and
		----	x.RelationType = 'SPOUSE'
	 ----)
	 ----, ChildGiftSentDate1 =(
		----select top 1
		----	x.SentGiftDate
		----from
		----	CsCustRelation x
		----where
		----	x.CompanyCode = a.CompanyCode
		----	and
		----	x.CustomerCode = a.CustomerCode
		----	and
		----	x.RelationType = 'CHILD_1'
	 ----)
	 ----, ChildGiftSentDate2 = (
		----select top 1
		----	x.SentGiftDate
		----from
		----	CsCustRelation x
		----where
		----	x.CompanyCode = a.CompanyCode
		----	and
		----	x.CustomerCode = a.CustomerCode
		----	and
		----	x.RelationType = 'CHILD_2'
	 ----)
	 ----, ChildGiftSentDate3 = (
		----select top 1
		----	x.SentGiftDate
		----from
		----	CsCustRelation x
		----where
		----	x.CompanyCode = a.CompanyCode
		----	and
		----	x.CustomerCode = a.CustomerCode
		----	and
		----	x.RelationType = 'CHILD_3'
	 ----)
	 ------, ParentGiftReceivedDate = c.SouvRcvParent
	 ----, SpouseGiftReceivedDate = (
		----select top 1
		----	x.ReceivedGiftDate
		----from
		----	CsCustRelation x
		----where
		----	x.CompanyCode = a.CompanyCode
		----	and
		----	x.CustomerCode = a.CustomerCode
		----	and
		----	x.RelationType = 'SPOUSE'
	 ----)
	 ----, ChildGiftReceivedDate1 = (
		----select top 1
		----	x.ReceivedGiftDate
		----from
		----	CsCustRelation x
		----where
		----	x.CompanyCode = a.CompanyCode
		----	and
		----	x.CustomerCode = a.CustomerCode
		----	and
		----	x.RelationType = 'CHILD_1'
	 ----)
	 ----, ChildGiftReceivedDate2 = (
		----select top 1
		----	x.ReceivedGiftDate
		----from
		----	CsCustRelation x
		----where
		----	x.CompanyCode = a.CompanyCode
		----	and
		----	x.CustomerCode = a.CustomerCode
		----	and
		----	x.RelationType = 'CHILD_2'
	 ----)
	 ----, ChildGiftReceivedDate3 = (
		----select top 1
		----	x.ReceivedGiftDate
		----from
		----	CsCustRelation x
		----where
		----	x.CompanyCode = a.CompanyCode
		----	and
		----	x.CustomerCode = a.CustomerCode
		----	and
		----	x.RelationType = 'CHILD_3'
	 ----)
	 ------ ======================================== Spouse
	 ----, SpouseName = (
		----select top 1
		----	x.FullName	
		----from 
		----	CsCustRelation x
		----where
		----	x.CompanyCode = a.CompanyCode
		----	and
		----	x.CustomerCode = a.CustomerCode
		----	and
		----	x.RelationType = 'SPOUSE'
	 ----)
	 ----, SpouseTelephone = (
		----select top 1
		----	x.PhoneNo	
		----from 
		----	CsCustRelation x
		----where
		----	x.CompanyCode = a.CompanyCode
		----	and
		----	x.CustomerCode = a.CustomerCode
		----	and
		----	x.RelationType = 'SPOUSE'
	 ----),
	 ----SpouseRelation = (
		----select top 1
		----	x.RelationInfo	
		----from 
		----	CsCustRelation x
		----where
		----	x.CompanyCode = a.CompanyCode
		----	and
		----	x.CustomerCode = a.CustomerCode
		----	and
		----	x.RelationType = 'SPOUSE'
	 ----),
	 ----SpouseBirthDate = (
		----select top 1
		----	x.BirthDate	
		----from 
		----	CsCustRelation x
		----where
		----	x.CompanyCode = a.CompanyCode
		----	and
		----	x.CustomerCode = a.CustomerCode
		----	and
		----	x.RelationType = 'SPOUSE'
	 ----)
	 ----, SpouseComment = (
		----select top 1
		----	x.Comment
		----from 
		----	CsCustRelation x
		----where
		----	x.CompanyCode = a.CompanyCode
		----	and
		----	x.CustomerCode = a.CustomerCode
		----	and
		----	x.RelationType = 'SPOUSE'
	 ----)
	 ----, SpouseTypeOfGift = (
		----select top 1
		----	x.TypeOfGift
		----from 
		----	CsCustRelation x
		----where
		----	x.CompanyCode = a.CompanyCode
		----	and
		----	x.CustomerCode = a.CustomerCode
		----	and
		----	x.RelationType = 'SPOUSE'
	 ----)
	 ------ ======================================== Child 1
	 ----, ChildName1 = (
		----select top 1
		----	x.FullName	
		----from 
		----	CsCustRelation x
		----where
		----	x.CompanyCode = a.CompanyCode
		----	and
		----	x.CustomerCode = a.CustomerCode
		----	and
		----	x.RelationType = 'CHILD_1'
	 ----)
	 ----, ChildTelephone1 = (
		----select top 1
		----	x.PhoneNo	
		----from 
		----	CsCustRelation x
		----where
		----	x.CompanyCode = a.CompanyCode
		----	and
		----	x.CustomerCode = a.CustomerCode
		----	and
		----	x.RelationType = 'CHILD_1'
	 ----)
	 ----, ChildBirthDate1 = (
		----select top 1
		----	x.BirthDate	
		----from 
		----	CsCustRelation x
		----where
		----	x.CompanyCode = a.CompanyCode
		----	and
		----	x.CustomerCode = a.CustomerCode
		----	and
		----	x.RelationType = 'CHILD_1'
	 ----)
	 ----, ChildComment1 = (
		----select top 1
		----	x.Comment
		----from 
		----	CsCustRelation x
		----where
		----	x.CompanyCode = a.CompanyCode
		----	and
		----	x.CustomerCode = a.CustomerCode
		----	and
		----	x.RelationType = 'CHILD_1'
	 ----)
	 ----, ChildTypeOfGift1 = (
		----select top 1
		----	x.TypeOfGift
		----from 
		----	CsCustRelation x
		----where
		----	x.CompanyCode = a.CompanyCode
		----	and
		----	x.CustomerCode = a.CustomerCode
		----	and
		----	x.RelationType = 'CHILD_1'
	 ----)
	 ------ ======================================== Child 2
	 ----, ChildName2 = (
		----select top 1
		----	x.FullName	
		----from 
		----	CsCustRelation x
		----where
		----	x.CompanyCode = a.CompanyCode
		----	and
		----	x.CustomerCode = a.CustomerCode
		----	and
		----	x.RelationType = 'CHILD_2'
	 ----)
	 ----, ChildTelephone2 = (
		----select top 1
		----	x.PhoneNo	
		----from 
		----	CsCustRelation x
		----where
		----	x.CompanyCode = a.CompanyCode
		----	and
		----	x.CustomerCode = a.CustomerCode
		----	and
		----	x.RelationType = 'CHILD_2'
	 ----)
	 ----, ChildBirthDate2 = (
		----select top 1
		----	x.BirthDate	
		----from 
		----	CsCustRelation x
		----where
		----	x.CompanyCode = a.CompanyCode
		----	and
		----	x.CustomerCode = a.CustomerCode
		----	and
		----	x.RelationType = 'CHILD_2'
	 ----),
	 ----ChildComment2 = (
		----select top 1
		----	x.Comment
		----from 
		----	CsCustRelation x
		----where
		----	x.CompanyCode = a.CompanyCode
		----	and
		----	x.CustomerCode = a.CustomerCode
		----	and
		----	x.RelationType = 'CHILD_2'
	 ----)
	 ----, ChildTypeOfGift2 = (
		----select top 1
		----	x.TypeOfGift
		----from 
		----	CsCustRelation x
		----where
		----	x.CompanyCode = a.CompanyCode
		----	and
		----	x.CustomerCode = a.CustomerCode
		----	and
		----	x.RelationType = 'CHILD_2'
	 ----)
	 ------ ======================================== Child 3
	 ----, ChildName3 = (
		----select top 1
		----	x.FullName	
		----from 
		----	CsCustRelation x
		----where
		----	x.CompanyCode = a.CompanyCode
		----	and
		----	x.CustomerCode = a.CustomerCode
		----	and
		----	x.RelationType = 'CHILD_3'
	 ----)
	 ----, ChildTelephone3 = (
		----select top 1
		----	x.PhoneNo	
		----from 
		----	CsCustRelation x
		----where
		----	x.CompanyCode = a.CompanyCode
		----	and
		----	x.CustomerCode = a.CustomerCode
		----	and
		----	x.RelationType = 'CHILD_3'
	 ----)
	 ----, ChildBirthDate3 = (
		----select top 1
		----	x.BirthDate	
		----from 
		----	CsCustRelation x
		----where
		----	x.CompanyCode = a.CompanyCode
		----	and
		----	x.CustomerCode = a.CustomerCode
		----	and
		----	x.RelationType = 'CHILD_3'
	 ----),
	 ----ChildComment3 = (
		----select top 1
		----	x.Comment
		----from 
		----	CsCustRelation x
		----where
		----	x.CompanyCode = a.CompanyCode
		----	and
		----	x.CustomerCode = a.CustomerCode
		----	and
		----	x.RelationType = 'CHILD_3'
	 ----)
	 ----, ChildTypeOfGift3 = (
		----select top 1
		----	x.TypeOfGift
		----from 
		----	CsCustRelation x
		----where
		----	x.CompanyCode = a.CompanyCode
		----	and
		----	x.CustomerCode = a.CustomerCode
		----	and
		----	x.RelationType = 'CHILD_3'
	 ----)
  ----   , BPKDate = convert(varchar(11),getdate(),106)
  ----   , e.SalesModelCode as CarType
  ----   , e.ColourCode as Color
  ----   , h.PoliceRegNo 
  ----   , e.EngineCode + convert(varchar, e.EngineNo) as Engine 
  ----   , (e.ChassisCode + convert(varchar, e.ChassisNo)) Chassis
  ----   , f.Salesman as SalesmanCode
  ----   , g.EmployeeName as SalesmanName
	 ----, CustomerComment = c.Comment
	 ----, c.AdditionalInquiries
	 ----, c.Status
  from gnMstCustomer a
  inner join gnMstOrganizationHdr b
    on b.CompanyCode = a.CompanyCode
  inner join csCustBirthday c
	on c.CompanyCode = a.CompanyCode
   and c.CustomerCode = a.CustomerCode
   and c.PeriodYear = DATEPART(YEAR, getdate())
 -- inner join CsTdayCall d
	--on a.CompanyCode = d.CompanyCode
 --  and a.CustomerCode = d.CustomerCode
  --left join omTrSalesSOVin e
  --  on e.CompanyCode = d.CompanyCode
  -- and (e.ChassisCode + convert(varchar, e.ChassisNo)) = d.Chassis
  --left join omTrSalesSO f
  --  on f.CompanyCode = e.CompanyCode
  -- and f.BranchCode = e.BranchCode
  -- and f.SONo = e.SONo
  --left join gnMstEmployee g
  --  on g.CompanyCode = f.CompanyCode
  -- and g.BranchCode = f.BranchCode
  -- and g.EmployeeID = f.Salesman
  --left join svMstCustomerVehicle h
  --  on h.CompanyCode = d.CompanyCode
  -- and h.ChassisCode + convert(varchar, h.ChassisNo) = d.Chassis
  where 1=1
	--and convert(varchar, @DateFrom, 112) < left(convert(varchar, a.BirthDate, 112), 6) + convert(varchar, convert(int, right(convert(varchar, a.BirthDate, 112), 2)) + 3)
	--and convert(varchar, @DateTo, 112) > left(convert(varchar, a.BirthDate, 112), 6) + convert(varchar, convert(int, right(convert(varchar, a.BirthDate, 112), 2)) + 3)
	--and datepart(MONTH, a.BirthDate) between @DateFrom and @DateTo

end


GO
/****** Object:  StoredProcedure [dbo].[uspfn_CsInqBpkbReminder]    Script Date: 12/13/2013 9:22:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[uspfn_CsInqBpkbReminder] 
	@CompanyCode varchar(15),
	@DateFrom datetime,
	@DateTo datetime
as

select distinct a.CompanyCode
     , a.CustomerCode
     , c.CustomerName
     , rtrim(c.Address1) + ' ' + rtrim(c.Address2) + rtrim(c.Address3) as Address
     , c.PhoneNo as Telephone
     , b.BpkbDate
     , b.StnkDate
     , case g.isLeasing
		when 1 then 'Leasing' 
		else 'Cash'
	   end as IsLeasing
     , FinanceInstitution = d.CustomerName
     , case g.isLeasing when 1 then (convert(varchar, g.Installment) + ' Months') else '' end as Tenor
     , BpkbReminder = 'None'
     , BpkbRequirements = 'None'
     , CarType=isnull(e.BasicModel, '-')
     , CarColor=isnull(e.ColourCode, '-')
     , PoliceNo=isnull(e.PoliceRegNo, '-')
     , Engine=e.EngineNo
     , Chassis=a.Chassis
     , SalesName=h.EmployeeName
     , CustomerComments=isnull(a.Comment, '')
     , AdditionalInquiries=isnull(a.Additional, '-')
  from CsCustBpkb a
  left join CsCustomerVehicle b
    on a.CompanyCode=b.CompanyCode
   and a.Chassis=b.Chassis
   and a.CustomerCode=b.CustomerCode
  left join gnMstCustomer c
    on a.CompanyCode=c.CompanyCode
   and a.CustomerCode=c.CustomerCode
  left join svMstCustomerVehicle e
    on e.CompanyCode = a.CompanyCode
   and e.ChassisCode + convert(varchar, e.ChassisNo) = a.Chassis
  left join omTrSalesSOVin f
    on f.CompanyCode = f.CompanyCode
   and f.ChassisCode + convert(varchar, f.ChassisNo) = a.Chassis
  left join omTrSalesSO g
    on g.CompanyCode = f.CompanyCode
   and g.BranchCode = f.BranchCode
   and g.SONo = f.SONo
  left join gnMstCustomer d
    on d.CompanyCode=g.CompanyCode
   and d.CustomerCode=g.LeasingCo
  left join HrEmployee h
    on h.CompanyCode = g.CompanyCode
   and h.EmployeeID = g.Salesman
 where a.CompanyCode=@CompanyCode
   and b.BpkbDate between @DateFrom and @DateTo


GO
/****** Object:  StoredProcedure [dbo].[uspfn_CsInqCustBDay]    Script Date: 12/13/2013 9:22:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE proc [dbo].[uspfn_CsInqCustBDay]
	@CompanyCode varchar(20),
	@BranchCode varchar(20),
	@DateFrom datetime,
	@DateTo datetime,
	@Outstanding char(1)
as

begin
	if @Outstanding = '1'
	begin
		select distinct CompanyCode = a.CompanyCode
			 , e.BranchCode
			 , a.CustomerCode
			 , b.CompanyName
			 , s.BranchName
			 , CustomerAddress = a.Address1 + ' ' + ltrim(a.Address2) + ' ' + ltrim(a.Address3) + ' ' + ltrim(a.Address4)
			 , PeriodOfYear = c.PeriodYear
			 , c.AdditionalInquiries
			 , c.Status

			 , a.CustomerName
			 , CustomerTelephone = a.PhoneNo
			 , CustomerBirthDate = a.BirthDate
			 , CustomerGiftSentDate = c.SentGiftDate
			 , CustomerGiftReceivedDate = c.ReceivedGiftDate
			 , CustomerTypeOfGift = c.TypeOfGift
			 , CustomerComment = c.Comment


			 , SpouseName = g.FullName				
			 , SpouseTelephone = g.PhoneNo
			 , SpouseRelation = g.RelationInfo
			 , SpouseBirthDate = g.BirthDate
			 , SpouseGiftSentDate = g.SentGiftDate
			 , SpouseGiftReceivedDate = g.ReceivedGiftDate
			 , SpouseTypeOfGift = g.TypeOfGift
			 , SpouseComment = g.Comment
		 

			 , ChildName1 = h.FullName				
			 , ChildTelephone1 = h.PhoneNo
			 , ChildRelation1 = h.RelationInfo
			 , ChildBirthDate1 = h.BirthDate
			 , ChildGiftSentDate1 = h.SentGiftDate
			 , ChildGiftReceivedDate1 = h.ReceivedGiftDate
			 , ChildTypeOfGift1 = h.TypeOfGift
			 , ChildComment1 = h.Comment


			 , ChildName2 = i.FullName				
			 , ChildTelephone2 = i.PhoneNo
			 , ChildRelation2 = i.RelationInfo
			 , ChildBirthDate2 = i.BirthDate
			 , ChildGiftSentDate2 = i.SentGiftDate
			 , ChildGiftReceivedDate2 = i.ReceivedGiftDate
			 , ChildTypeOfGift2 = i.TypeOfGift
			 , ChildComment2 = i.Comment


			 , ChildName3 = j.FullName				
			 , ChildTelephone3 = j.PhoneNo
			 , ChildRelation3 = j.RelationInfo
			 , ChildBirthDate3 = j.BirthDate
			 , ChildGiftSentDate3 = j.SentGiftDate
			 , ChildGiftReceivedDate3 = j.ReceivedGiftDate
			 , ChildTypeOfGift3 = j.TypeOfGift
			 , ChildComment3 = j.Comment


			 --, f.PoliceRegNo
			 --, f.EngineCode + convert(varchar, f.EngineNo) as Engine 
			 --, e.SalesModelCode as CarType
			 --, e.ColourCode as Color
			 --, Salesman = isnull((
				--select top 1 y.EmployeeName from omTrSalesSO x, gnMstEmployee y
				--	where x.CompanyCode = y.CompanyCode
				--	and x.BranchCode = y.BranchCode
				--	and x.Salesman = y.EmployeeID
				--	and x.CompanyCode = a.CompanyCode
				--	and x.SONo = d.SONo
			 --  )
			 --, '')
		 

			 , NumberOfChildren = (
					select count(xx.CustomerCode)
					  from CsCustRelation xx
					 where xx.CompanyCode=a.CompanyCode
					   and xx.CustomerCode=a.CustomerCode
					   and xx.RelationType like '%CHILD%'
			   )
			 , NumberOfSpouse = (
					select count(xx.CustomerCode)
					  from CsCustRelation xx
					 where xx.CompanyCode=a.CompanyCode
					   and xx.CustomerCode=a.CustomerCode
					   and xx.RelationType = 'SPOUSE'
			   )
		  from gnMstCustomer a
		 inner join gnMstOrganizationHdr b
			on b.CompanyCode = a.CompanyCode
		 inner join csCustBirthday c
			on c.CompanyCode = a.CompanyCode
		   and c.CustomerCode = a.CustomerCode
		   and c.PeriodYear = DATEPART(YEAR, getdate())
		  left join omTrSalesInvoice d
			on d.CompanyCode=a.CompanyCode
		   and d.CustomerCode=a.CustomerCode
		  left join omTrSalesInvoiceVin e
			on e.CompanyCode=d.CompanyCode
		   and e.BranchCode=d.BranchCode
		   and e.InvoiceNo=d.InvoiceNo
		 inner join svMstCustomerVehicle f
			on f.CompanyCode = a.CompanyCode
		   and f.ChassisNo = e.ChassisNo
		   and f.ChassisCode = e.ChassisCode
		 inner join gnMstOrganizationDtl s
			on s.CompanyCode=b.CompanyCode
		   and s.BranchCode=e.BranchCode
		  left join CsCustRelation g			/* Spouse */
			on g.CompanyCode=a.CompanyCode
		   and g.CustomerCode=a.CustomerCode
		   and g.RelationType='SPOUSE' 
		  left join CsCustRelation h			/* Child 1 */
			on h.CompanyCode=a.CompanyCode
		   and h.CustomerCode=a.CustomerCode
		   and h.RelationType='CHILD_1'  
		  left join CsCustRelation i 			/* Child 2 */
			on i.CompanyCode=a.CompanyCode
		   and i.CustomerCode=a.CustomerCode
		   and i.RelationType='CHILD_2'  
		  left join CsCustRelation j 			/* Child 3 */
			on j.CompanyCode=a.CompanyCode
		   and j.CustomerCode=a.CustomerCode
		   and j.RelationType='CHILD_3' 
		 where 1=1
		   and e.BranchCode=@BranchCode
		   and datepart(MONTH, a.BirthDate) between @DateFrom and @DateTo
		   and c.PeriodYear=DATEPART(Year, getdate())
	end
	else
	begin 
		   select distinct
				  a.CompanyCode
				, a.CustomerCode
				, b.CompanyName
				, c.BranchName
				, a.CustomerName
				, CustomerTelephone = a.PhoneNo
				, CustomerAddress = a.Address1 + ' ' + ltrim(a.Address2) + ' ' + ltrim(a.Address3) + ' ' + ltrim(a.Address4)
				, CustomerBirthDate = a.BirthDate
				, Status = 0
		   from gnMstCustomer a
		  inner join gnMstOrganizationHdr b
			 on b.CompanyCode = a.CompanyCode
		   left join omTrSalesInvoice d
			 on d.CompanyCode=a.CompanyCode
			and d.CustomerCode=a.CustomerCode
		   left join omTrSalesInvoiceVin e
			 on e.CompanyCode=d.CompanyCode
			and e.BranchCode=d.BranchCode
			and e.InvoiceNo=d.InvoiceNo
		  inner join svMstCustomerVehicle f
			 on f.CompanyCode = a.CompanyCode
			and f.ChassisNo = e.ChassisNo
			and f.ChassisCode = e.ChassisCode
		  inner join gnMstOrganizationDtl c
			 on c.CompanyCode=b.CompanyCode
			and c.BranchCode=e.BranchCode
		  where e.BranchCode=@BranchCode
		    and datepart(MONTH, a.BirthDate) = datepart(month, getdate())
			and a.CustomerCode not in (
				select xx.CustomerCode
				  from CsCustBirthDay xx
				 where xx.CompanyCode=a.CompanyCode
				   and xx.PeriodYear=datepart(year, getdate())
			)
	end
end


GO
/****** Object:  StoredProcedure [dbo].[uspfn_CsInqCustFeedback]    Script Date: 12/13/2013 9:22:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[uspfn_CsInqCustFeedback]
	@CompanyCode varchar(20),
	@DateFrom varchar(10),
	@DateTo varchar(10)
as

select distinct a.CompanyCode
     , a.CustomerCode
     , c.CustomerName
     , rtrim(c.Address1) + ' ' + rtrim(c.Address2) + rtrim(c.Address3) as Address
     , c.HPNo
     , b.BpkbDate
     , b.StnkDate
     , g.DODate 
     , e.SalesModelCode
     , e.SalesModelYear
     , h.PoliceRegNo
     , a.Chassis
     , a.IsManual
     , a.FeedbackA
     , a.FeedbackB
     , a.FeedbackC
     , a.FeedbackD
     , case a.IsManual when 1 then 'Manual' else 'System' end Feedback
     , case len(rtrim(isnull(a.FeedbackA,''))) when 0 then '-' else 'Ya' end Feedback01
     , case len(rtrim(isnull(a.FeedbackB,''))) when 0 then '-' else 'Ya' end Feedback02
     , case len(rtrim(isnull(a.FeedbackC,''))) when 0 then '-' else 'Ya' end Feedback03
     , case len(rtrim(isnull(a.FeedbackD,''))) when 0 then '-' else 'Ya' end Feedback04
  from CsCustFeedback a
  left join CsCustomerVehicle b
    on b.CompanyCode = a.CompanyCode
   and b.CustomerCode = a.CustomerCode
   and b.Chassis = a.Chassis
  left join GnMstCustomer c
    on c.CompanyCode = a.CompanyCode
   and c.CustomerCode = a.CustomerCode
  left join omTrSalesSOVin e          
    on e.CompanyCode = a.CompanyCode          
   and e.ChassisCode + convert(varchar, e.ChassisNo) = a.Chassis 
  left join omTrSalesSO f
    on f.CompanyCode = e.CompanyCode          
   and f.BranchCode = e.BranchCode
   and f.SONo = e.SONo
  left join omTrSalesDO g
    on g.CompanyCode = f.CompanyCode          
   and g.BranchCode = f.BranchCode
   and g.SONo = f.SONo
  left join GnMstCustomer d
    on d.CompanyCode = f.CompanyCode
   and d.CustomerCode = f.LeasingCo
  left join svMstCustomerVehicle h
    on h.CompanyCode = a.CompanyCode          
   and h.ChassisCode + convert(varchar, h.ChassisNo) = a.Chassis 
 where a.CompanyCode = @CompanyCode
   and convert(varchar, g.DODate, 112) between @DateFrom and @DateTo   

GO
/****** Object:  StoredProcedure [dbo].[uspfn_CsInqStnkExt]    Script Date: 12/13/2013 9:22:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[uspfn_CsInqStnkExt]
	@CompanyCode varchar(20),
	@DateFrom varchar(10),
	@DateTo varchar(10)
as

select distinct a.CompanyCode
     , a.CustomerCode
     , c.CustomerName
     , rtrim(c.Address1) + ' ' + rtrim(c.Address2) + rtrim(c.Address3) as Address
     , c.HPNo
     , b.BpkbDate
     , b.StnkDate
     , a.StnkExpiredDate
     , f.isLeasing
     , f.isLeasing as CustomerCategory
     , case f.isLeasing
		when 1 then 'Leasing' 
		else 'Cash'
	   end as CustCtgDesc
     , f.LeasingCo as LeasingCode
     , d.CustomerName LeasingDesc
     , case f.isLeasing when 1 then (convert(varchar, f.Installment) + ' Months') else '' end as Tenor
     , case a.IsStnkExtend when 1 then 'Ya' when 2 then 'Tidak' else '' end as StnkExtend
     , case a.ReqStnk when 1 then 'Ya' when 2 then 'Tidak' else '' end as ReqStnkDesc
     , e.SalesModelCode
     , e.ColourCode
     , h.PoliceRegNo
     , e.EngineCode + convert(varchar, e.EngineNo) as Engine
     , a.Chassis
     , a.Comment
     , a.Additional
     , g.EmployeeName as SalesmanName
  from CsStnkExt a
  left join CsCustomerVehicle b
    on b.CompanyCode = a.CompanyCode
   and b.CustomerCode = a.CustomerCode
   and b.Chassis = a.Chassis
  left join GnMstCustomer c
    on c.CompanyCode = a.CompanyCode
   and c.CustomerCode = a.CustomerCode
  left join omTrSalesSOVin e          
    on e.CompanyCode = a.CompanyCode          
   and e.ChassisCode + convert(varchar, e.ChassisNo) = a.Chassis 
  left join omTrSalesSO f
    on f.CompanyCode = e.CompanyCode          
   and f.BranchCode = e.BranchCode
   and f.SONo = e.SONo
  left join GnMstCustomer d
    on d.CompanyCode = f.CompanyCode
   and d.CustomerCode = f.LeasingCo
  left join HrEmployee g
    on g.CompanyCode = f.CompanyCode          
   and g.EmployeeID = f.Salesman
  left join svMstCustomerVehicle h
    on h.CompanyCode = a.CompanyCode          
   and h.ChassisCode + convert(varchar, h.ChassisNo) = a.Chassis 
 where a.CompanyCode = @CompanyCode
   and convert(varchar, a.StnkExpiredDate, 112) between @DateFrom and @DateTo   

GO
/****** Object:  StoredProcedure [dbo].[uspfn_CsInqTDayCall]    Script Date: 12/13/2013 9:22:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[uspfn_CsInqTDayCall]
--declare
	@CompanyCode varchar(20),
	@DateFrom varchar(10),
	@DateTo varchar(10)
as

;with x as (
select a.CompanyCode, a.CustomerCode, a.Chassis
     , case a.IsDeliveredA when 1 then 'Ya' else 'Tidak' end IsDeliveredA
     , case a.IsDeliveredB when 1 then 'Ya' else 'Tidak' end IsDeliveredB
     , case a.IsDeliveredC when 1 then 'Ya' else 'Tidak' end IsDeliveredC
     , case a.IsDeliveredD when 1 then 'Ya' else 'Tidak' end IsDeliveredD
     , case a.IsDeliveredE when 1 then 'Ya' else 'Tidak' end IsDeliveredE
     , a.Comment, a.Additional, a.Status
     , DeliveryDate = isnull((    
		  select top 1 y.DODate    
			from omTrSalesDODetail x    
			left join omTrSalesDO y    
			  on y.CompanyCode = x.CompanyCode    
			 and y.BranchCode = x.BranchCode    
			 and y.DONo = x.DONo    
		   where x.CompanyCode = a.CompanyCode    
			 and x.ChassisCode + convert(varchar, x.ChassisNo)= a.Chassis
          ), null)  
     , b.CustomerName, b.Address1, b.Address2, b.PhoneNo, b.HPNo
     , rtrim(b.Address1) + ' ' + rtrim(b.Address2) + rtrim(b.Address3) as Address
     , c.SalesModelCode as CarType
     , c.ColourCode as Color
     , f.PoliceRegNo
     , f.EngineCode + convert(varchar, f.EngineNo) as Engine 
     , Salesman = isnull((
			select top 1 y.EmployeeName from omTrSalesSO x, gnMstEmployee y
			 where x.CompanyCode = y.CompanyCode
			   and x.BranchCode = y.BranchCode
			   and x.Salesman = y.EmployeeID
			   and x.CompanyCode = a.CompanyCode
			   and x.SONo = c.SONo
       ), '')
  from CsTDayCall a
  left join GnMstCustomer b
    on b.CompanyCode = a.CompanyCode
   and b.CustomerCode = a.CustomerCode
  left join omTrSalesSOVin c
    on c.CompanyCode = a.CompanyCode
   and (c.ChassisCode + convert(varchar, c.ChassisNo)) = a.Chassis
  left join svMstCustomerVehicle f
    on f.CompanyCode = a.CompanyCode
   and f.ChassisCode + convert(varchar, f.ChassisNo) = a.Chassis
 where a.CompanyCode = @CompanyCode
)
select * from x where convert(varchar, x.DeliveryDate, 112) between @DateFrom and @DateTo




GO
/****** Object:  StoredProcedure [dbo].[uspfn_CsLkuBpkb]    Script Date: 12/13/2013 9:22:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[uspfn_CsLkuBpkb]
	@CompanyCode varchar(20),
	@BranchCode  varchar(20),
	@OutStanding bit
as

declare @IsHolding bit
set @IsHolding = isnull((select top 1 1 from gnMstOrganizationDtl where CompanyCode = @CompanyCode and BranchCode = @BranchCode and IsBranch = 0), 0)

if @OutStanding = 1 
begin
	select top 5000 a.CompanyCode     
		 , a.BranchCode
		 , b.CustomerCode
		 , c.CustomerName
		 , a.ChassisCode + convert(varchar, a.ChassisNo) Chassis
		 , a.EngineCode + convert(varchar, a.EngineNo) Engine
		 , a.SalesModelCode
		 , a.SalesModelYear
		 , isnull(e.BpkbDate, g.BPKDate) BpkbDate
		 , isnull(e.StnkDate, g.BPKDate) StnkDate
		 , g.BPKDate
		 , isnull(h.isLeasing, 0) as IsLeasing
		 , case isnull(h.isLeasing, 0) when 0 then 'Tunai' else 'Leasing' end as Category
		 , h.LeasingCo
		 , i.CustomerName as LeasingName
		 , h.Installment
		 , j.PoliceRegNo
	  from omTrSalesInvoiceVin a    
	  left join omTrSalesInvoice b    
		on b.CompanyCode = a.CompanyCode    
	   and b.BranchCode = a.BranchCode    
	   and b.InvoiceNo = a.InvoiceNo    
	  left join gnMstCustomer c
		on c.CompanyCode = a.CompanyCode    
	   and c.CustomerCode = b.CustomerCode
	  left join omTrSalesDODetail d
		on d.CompanyCode = a.CompanyCode    
	   and d.BranchCode = a.BranchCode
	   and d.ChassisCode = a.ChassisCode
	   and d.ChassisNo = a.ChassisNo
	  left join CsCustomerVehicle e
		on e.CompanyCode = a.CompanyCode    
	   and e.Chassis = a.ChassisCode + convert(varchar, a.ChassisNo)
	  left join omTrSalesDO f
		on f.CompanyCode = d.CompanyCode    
	   and f.BranchCode = d.BranchCode
	   and f.DONo = d.DONo
	  left join omTrSalesBPK g
		on g.CompanyCode = f.CompanyCode    
	   and g.BranchCode = f.BranchCode
	   and g.DONo = f.DONo
	  left join omTrSalesSO h
		on h.CompanyCode = g.CompanyCode    
	   and h.BranchCode = g.BranchCode
	   and h.SONo = g.SONo
	  left join gnMstCustomer i
		on i.CompanyCode = h.CompanyCode
	   and i.CustomerCode = h.LeasingCo
	  left join svMstCustomerVehicle j
		on j.CompanyCode = a.CompanyCode
	   and j.ChassisCode = a.ChassisCode
	   and j.ChassisNo = a.ChassisNo
	 where 1 = 1
	   and a.CompanyCode = @CompanyCode
	   and a.BranchCode = (case @IsHolding when 1 then a.BranchCode else @BranchCode end)
	   and isnull(a.IsReturn, 0) = 0
	   and not exists (
			select 1 from CsCustBpkb 
			 where CompanyCode = a.CompanyCode
			   and CustomerCode = b.CustomerCode
			   and Chassis = a.ChassisCode + convert(varchar, a.ChassisNo))
end   
else
begin
	select top 5000 a.CompanyCode     
		 , a.BranchCode
		 , b.CustomerCode
		 , c.CustomerName
		 , a.ChassisCode + convert(varchar, a.ChassisNo) Chassis
		 , a.EngineCode + convert(varchar, a.EngineNo) Engine
		 , a.SalesModelCode
		 , a.SalesModelYear
		 , isnull(e.BpkbDate, g.BPKDate) BpkbDate
		 , isnull(e.StnkDate, g.BPKDate) StnkDate
		 , g.BPKDate
		 , isnull(h.isLeasing, 0) as IsLeasing
		 , case isnull(h.isLeasing, 0) when 0 then 'Tunai' else 'Leasing' end as Category
		 , h.LeasingCo
		 , i.CustomerName as LeasingName
		 , h.Installment
		 , j.PoliceRegNo
	  from omTrSalesInvoiceVin a    
	  left join omTrSalesInvoice b    
		on b.CompanyCode = a.CompanyCode    
	   and b.BranchCode = a.BranchCode    
	   and b.InvoiceNo = a.InvoiceNo    
	  left join gnMstCustomer c
		on c.CompanyCode = a.CompanyCode    
	   and c.CustomerCode = b.CustomerCode
	  left join omTrSalesDODetail d
		on d.CompanyCode = a.CompanyCode    
	   and d.BranchCode = a.BranchCode
	   and d.ChassisCode = a.ChassisCode
	   and d.ChassisNo = a.ChassisNo
	  left join CsCustomerVehicle e
		on e.CompanyCode = a.CompanyCode    
	   and e.Chassis = a.ChassisCode + convert(varchar, a.ChassisNo)
	  left join omTrSalesDO f
		on f.CompanyCode = d.CompanyCode    
	   and f.BranchCode = d.BranchCode
	   and f.DONo = d.DONo
	  left join omTrSalesBPK g
		on g.CompanyCode = f.CompanyCode    
	   and g.BranchCode = f.BranchCode
	   and g.DONo = f.DONo
	  left join omTrSalesSO h
		on h.CompanyCode = g.CompanyCode    
	   and h.BranchCode = g.BranchCode
	   and h.SONo = g.SONo
	  left join gnMstCustomer i
		on i.CompanyCode = h.CompanyCode
	   and i.CustomerCode = h.LeasingCo
	  left join svMstCustomerVehicle j
		on j.CompanyCode = a.CompanyCode
	   and j.ChassisCode = a.ChassisCode
	   and j.ChassisNo = a.ChassisNo
	 where 1 = 1
	   and a.CompanyCode = @CompanyCode
	   and a.BranchCode = (case @IsHolding when 1 then a.BranchCode else @BranchCode end)
	   and isnull(a.IsReturn, 0) = 0
	   and exists (
			select 1 from CsCustBpkb 
			 where CompanyCode = a.CompanyCode
			   and CustomerCode = b.CustomerCode
			   and Chassis = a.ChassisCode + convert(varchar, a.ChassisNo))
end   

GO
/****** Object:  StoredProcedure [dbo].[uspfn_CsLkuFeedback]    Script Date: 12/13/2013 9:22:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[uspfn_CsLkuFeedback]
	@CompanyCode varchar(20),
	@BranchCode  varchar(20),
	@OutStanding bit
as

declare @IsHolding bit
set @IsHolding = isnull((select top 1 1 from gnMstOrganizationDtl where CompanyCode = @CompanyCode and BranchCode = @BranchCode and IsBranch = 0), 0)

if @OutStanding = 1 
begin
	select top 5000 a.CompanyCode     
		 , a.BranchCode
		 , b.CustomerCode
		 , c.CustomerName
		 , a.ChassisCode + convert(varchar, a.ChassisNo) Chassis
		 , a.EngineCode + convert(varchar, a.EngineNo) Engine
		 , a.SalesModelCode
		 , a.SalesModelYear
		 , j.PoliceRegNo
		 , f.DODate
		 , '' IsManual
		 , '-' Feedback
	  from omTrSalesInvoiceVin a    
	  left join omTrSalesInvoice b    
		on b.CompanyCode = a.CompanyCode    
	   and b.BranchCode = a.BranchCode    
	   and b.InvoiceNo = a.InvoiceNo    
	  left join gnMstCustomer c
		on c.CompanyCode = a.CompanyCode    
	   and c.CustomerCode = b.CustomerCode
	  left join omTrSalesDODetail d
		on d.CompanyCode = a.CompanyCode    
	   and d.BranchCode = a.BranchCode
	   and d.ChassisCode = a.ChassisCode
	   and d.ChassisNo = a.ChassisNo
	  left join CsCustomerVehicle e
		on e.CompanyCode = a.CompanyCode    
	   and e.Chassis = a.ChassisCode + convert(varchar, a.ChassisNo)
	  left join omTrSalesDO f
		on f.CompanyCode = d.CompanyCode    
	   and f.BranchCode = d.BranchCode
	   and f.DONo = d.DONo
	  left join omTrSalesBPK g
		on g.CompanyCode = f.CompanyCode    
	   and g.BranchCode = f.BranchCode
	   and g.DONo = f.DONo
	  left join omTrSalesSO h
		on h.CompanyCode = g.CompanyCode    
	   and h.BranchCode = g.BranchCode
	   and h.SONo = g.SONo
	  left join gnMstCustomer i
		on i.CompanyCode = h.CompanyCode
	   and i.CustomerCode = h.LeasingCo
	  left join svMstCustomerVehicle j
		on j.CompanyCode = a.CompanyCode
	   and j.ChassisCode = a.ChassisCode
	   and j.ChassisNo = a.ChassisNo
	 where 1 = 1
	   and a.CompanyCode = @CompanyCode
	   and a.BranchCode = (case @IsHolding when 1 then a.BranchCode else @BranchCode end)
	   and isnull(a.IsReturn, 0) = 0
	   and f.DODate is not null
	   and not exists (
			select 1 from CsCustFeedback 
			 where CompanyCode = a.CompanyCode
			   and CustomerCode = b.CustomerCode
			   and Chassis = a.ChassisCode + convert(varchar, a.ChassisNo))
end   
else
begin
	select top 5000 a.CompanyCode     
		 , a.BranchCode
		 , b.CustomerCode
		 , c.CustomerName
		 , a.ChassisCode + convert(varchar, a.ChassisNo) Chassis
		 , a.EngineCode + convert(varchar, a.EngineNo) Engine
		 , a.SalesModelCode
		 , a.SalesModelYear
		 , j.PoliceRegNo
		 , f.DODate
		 , k.IsManual
		 , case k.IsManual when 1 then 'Manual' else 'System' end Feedback
	  from omTrSalesInvoiceVin a    
	  left join omTrSalesInvoice b    
		on b.CompanyCode = a.CompanyCode    
	   and b.BranchCode = a.BranchCode    
	   and b.InvoiceNo = a.InvoiceNo    
	  left join gnMstCustomer c
		on c.CompanyCode = a.CompanyCode    
	   and c.CustomerCode = b.CustomerCode
	  left join omTrSalesDODetail d
		on d.CompanyCode = a.CompanyCode    
	   and d.BranchCode = a.BranchCode
	   and d.ChassisCode = a.ChassisCode
	   and d.ChassisNo = a.ChassisNo
	  left join CsCustomerVehicle e
		on e.CompanyCode = a.CompanyCode    
	   and e.Chassis = a.ChassisCode + convert(varchar, a.ChassisNo)
	  left join omTrSalesDO f
		on f.CompanyCode = d.CompanyCode    
	   and f.BranchCode = d.BranchCode
	   and f.DONo = d.DONo
	  left join omTrSalesBPK g
		on g.CompanyCode = f.CompanyCode    
	   and g.BranchCode = f.BranchCode
	   and g.DONo = f.DONo
	  left join omTrSalesSO h
		on h.CompanyCode = g.CompanyCode    
	   and h.BranchCode = g.BranchCode
	   and h.SONo = g.SONo
	  left join gnMstCustomer i
		on i.CompanyCode = h.CompanyCode
	   and i.CustomerCode = h.LeasingCo
	  left join svMstCustomerVehicle j
		on j.CompanyCode = a.CompanyCode
	   and j.ChassisCode = a.ChassisCode
	   and j.ChassisNo = a.ChassisNo
	  left join CsCustFeedback k
		on k.CompanyCode = a.CompanyCode
	   and k.CustomerCode = b.CustomerCode
	   and k.Chassis = a.ChassisCode + convert(varchar, a.ChassisNo)
	 where 1 = 1
	   and a.CompanyCode = @CompanyCode
	   and a.BranchCode = (case @IsHolding when 1 then a.BranchCode else @BranchCode end)
	   and isnull(a.IsReturn, 0) = 0
	   and f.DODate is not null
	   and exists (
			select 1 from CsCustFeedback 
			 where CompanyCode = a.CompanyCode
			   and CustomerCode = b.CustomerCode
			   and Chassis = a.ChassisCode + convert(varchar, a.ChassisNo))
end   

GO
/****** Object:  StoredProcedure [dbo].[uspfn_CsLkuStnkExt]    Script Date: 12/13/2013 9:22:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[uspfn_CsLkuStnkExt]
	@CompanyCode varchar(20),
	@BranchCode  varchar(20),
	@OutStanding bit
as

declare @IsHolding bit
set @IsHolding = isnull((select top 1 1 from gnMstOrganizationDtl where CompanyCode = @CompanyCode and BranchCode = @BranchCode and IsBranch = 0), 0)

if @OutStanding = 1 
begin
	select top 5000 a.CompanyCode     
		 , a.BranchCode
		 , b.CustomerCode
		 , c.CustomerName
		 , a.ChassisCode + convert(varchar, a.ChassisNo) Chassis
		 , a.EngineCode + convert(varchar, a.EngineNo) Engine
		 , a.SalesModelCode
		 , a.SalesModelYear
		 , isnull(e.BpkbDate, g.BPKDate) BpkbDate
		 , isnull(e.StnkDate, g.BPKDate) StnkDate
		 , g.BPKDate
		 , isnull(h.isLeasing, 0) as IsLeasing
		 , case isnull(h.isLeasing, 0) when 0 then 'Tunai' else 'Leasing' end as Category
		 , h.LeasingCo
		 , i.CustomerName as LeasingName
		 , h.Installment
		 , j.PoliceRegNo
	  from omTrSalesInvoiceVin a    
	  left join omTrSalesInvoice b    
		on b.CompanyCode = a.CompanyCode    
	   and b.BranchCode = a.BranchCode    
	   and b.InvoiceNo = a.InvoiceNo    
	  left join gnMstCustomer c
		on c.CompanyCode = a.CompanyCode    
	   and c.CustomerCode = b.CustomerCode
	  left join omTrSalesDODetail d
		on d.CompanyCode = a.CompanyCode    
	   and d.BranchCode = a.BranchCode
	   and d.ChassisCode = a.ChassisCode
	   and d.ChassisNo = a.ChassisNo
	  left join CsCustomerVehicle e
		on e.CompanyCode = a.CompanyCode    
	   and e.Chassis = a.ChassisCode + convert(varchar, a.ChassisNo)
	  left join omTrSalesDO f
		on f.CompanyCode = d.CompanyCode    
	   and f.BranchCode = d.BranchCode
	   and f.DONo = d.DONo
	  left join omTrSalesBPK g
		on g.CompanyCode = f.CompanyCode    
	   and g.BranchCode = f.BranchCode
	   and g.DONo = f.DONo
	  left join omTrSalesSO h
		on h.CompanyCode = g.CompanyCode    
	   and h.BranchCode = g.BranchCode
	   and h.SONo = g.SONo
	  left join gnMstCustomer i
		on i.CompanyCode = h.CompanyCode
	   and i.CustomerCode = h.LeasingCo
	  left join svMstCustomerVehicle j
		on j.CompanyCode = a.CompanyCode
	   and j.ChassisCode = a.ChassisCode
	   and j.ChassisNo = a.ChassisNo
	 where 1 = 1
	   and a.CompanyCode = @CompanyCode
	   and a.BranchCode = (case @IsHolding when 1 then a.BranchCode else @BranchCode end)
	   and isnull(a.IsReturn, 0) = 0
	   and not exists (
			select 1 from CsStnkExt 
			 where CompanyCode = a.CompanyCode
			   and CustomerCode = b.CustomerCode
			   and Chassis = a.ChassisCode + convert(varchar, a.ChassisNo)
			   and year(CreatedDate) = year(getdate()))
end   
else
begin
	select top 5000 a.CompanyCode     
		 , a.BranchCode
		 , b.CustomerCode
		 , c.CustomerName
		 , a.ChassisCode + convert(varchar, a.ChassisNo) Chassis
		 , a.EngineCode + convert(varchar, a.EngineNo) Engine
		 , a.SalesModelCode
		 , a.SalesModelYear
		 , isnull(e.BpkbDate, g.BPKDate) BpkbDate
		 , isnull(e.StnkDate, g.BPKDate) StnkDate
		 , g.BPKDate
		 , isnull(h.isLeasing, 0) as IsLeasing
		 , case isnull(h.isLeasing, 0) when 0 then 'Tunai' else 'Leasing' end as Category
		 , h.LeasingCo
		 , i.CustomerName as LeasingName
		 , h.Installment
		 , j.PoliceRegNo
	  from omTrSalesInvoiceVin a    
	  left join omTrSalesInvoice b    
		on b.CompanyCode = a.CompanyCode    
	   and b.BranchCode = a.BranchCode    
	   and b.InvoiceNo = a.InvoiceNo    
	  left join gnMstCustomer c
		on c.CompanyCode = a.CompanyCode    
	   and c.CustomerCode = b.CustomerCode
	  left join omTrSalesDODetail d
		on d.CompanyCode = a.CompanyCode    
	   and d.BranchCode = a.BranchCode
	   and d.ChassisCode = a.ChassisCode
	   and d.ChassisNo = a.ChassisNo
	  left join CsCustomerVehicle e
		on e.CompanyCode = a.CompanyCode    
	   and e.Chassis = a.ChassisCode + convert(varchar, a.ChassisNo)
	  left join omTrSalesDO f
		on f.CompanyCode = d.CompanyCode    
	   and f.BranchCode = d.BranchCode
	   and f.DONo = d.DONo
	  left join omTrSalesBPK g
		on g.CompanyCode = f.CompanyCode    
	   and g.BranchCode = f.BranchCode
	   and g.DONo = f.DONo
	  left join omTrSalesSO h
		on h.CompanyCode = g.CompanyCode    
	   and h.BranchCode = g.BranchCode
	   and h.SONo = g.SONo
	  left join gnMstCustomer i
		on i.CompanyCode = h.CompanyCode
	   and i.CustomerCode = h.LeasingCo
	  left join svMstCustomerVehicle j
		on j.CompanyCode = a.CompanyCode
	   and j.ChassisCode = a.ChassisCode
	   and j.ChassisNo = a.ChassisNo
	 where 1 = 1
	   and a.CompanyCode = @CompanyCode
	   and a.BranchCode = (case @IsHolding when 1 then a.BranchCode else @BranchCode end)
	   and isnull(a.IsReturn, 0) = 0
	   and exists (
			select 1 from CsStnkExt 
			 where CompanyCode = a.CompanyCode
			   and CustomerCode = b.CustomerCode
			   and Chassis = a.ChassisCode + convert(varchar, a.ChassisNo)
			   and year(CreatedDate) = year(getdate()))
end   

GO
/****** Object:  StoredProcedure [dbo].[uspfn_CsLkuTDayCall]    Script Date: 12/13/2013 9:22:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[uspfn_CsLkuTDayCall]
	@CompanyCode varchar(20),
	@BranchCode  varchar(20),
	@OutStanding bit
as

declare @IsHolding bit
set @IsHolding = isnull((select top 1 1 from gnMstOrganizationDtl where CompanyCode = @CompanyCode and BranchCode = @BranchCode and IsBranch = 0), 0)

if @OutStanding = 1 
begin
	select top 5000 a.CompanyCode     
		 , a.BranchCode
		 , b.CustomerCode
		 , c.CustomerName
		 , a.ChassisCode + convert(varchar, a.ChassisNo) Chassis
		 , a.EngineCode + convert(varchar, a.EngineNo) Engine
		 , a.SalesModelCode
		 , a.SalesModelYear
		 , e.DODate
		 , f.PoliceRegNo
	  from omTrSalesInvoiceVin a    
	  left join omTrSalesInvoice b    
		on b.CompanyCode = a.CompanyCode    
	   and b.BranchCode = a.BranchCode    
	   and b.InvoiceNo = a.InvoiceNo    
	  left join gnMstCustomer c
		on c.CompanyCode = a.CompanyCode    
	   and c.CustomerCode = b.CustomerCode
	  left join omTrSalesDODetail d
		on d.CompanyCode = a.CompanyCode    
	   and d.BranchCode = a.BranchCode
	   and d.ChassisCode = a.ChassisCode
	   and d.ChassisNo = a.ChassisNo
	  left join omTrSalesDO e
		on e.CompanyCode = d.CompanyCode    
	   and e.BranchCode = d.BranchCode
	   and e.DONo = d.DONo
	  left join svMstCustomerVehicle f
		on f.CompanyCode = a.CompanyCode    
	   and f.ChassisCode = a.ChassisCode
	   and f.ChassisNo = a.ChassisNo
	 where 1 = 1
	   and a.CompanyCode = @CompanyCode
	   and a.BranchCode = (case @IsHolding when 1 then a.BranchCode else @BranchCode end)
	   and isnull(a.IsReturn, 0) = 0
	   and e.DODate is not null
	   and year(e.DODate) = year(getdate())
	   and not exists (
			select 1 from CsTDayCall 
			 where CompanyCode = a.CompanyCode
			   and CustomerCode = b.CustomerCode
			   and Chassis = a.ChassisCode + convert(varchar, a.ChassisNo))
end   
else
begin
	select a.CompanyCode     
		 , a.BranchCode
		 , b.CustomerCode
		 , c.CustomerName
		 , a.ChassisCode + convert(varchar, a.ChassisNo) Chassis
		 , a.EngineCode + convert(varchar, a.EngineNo) Engine
		 , a.SalesModelCode
		 , a.SalesModelYear
		 , e.DODate
		 , f.PoliceRegNo
	  from omTrSalesInvoiceVin a    
	  left join omTrSalesInvoice b    
		on b.CompanyCode = a.CompanyCode    
	   and b.BranchCode = a.BranchCode    
	   and b.InvoiceNo = a.InvoiceNo    
	  left join gnMstCustomer c
		on c.CompanyCode = a.CompanyCode    
	   and c.CustomerCode = b.CustomerCode
	  left join omTrSalesDODetail d
		on d.CompanyCode = a.CompanyCode    
	   and d.BranchCode = a.BranchCode
	   and d.ChassisCode = a.ChassisCode
	   and d.ChassisNo = a.ChassisNo
	  left join omTrSalesDO e
		on e.CompanyCode = d.CompanyCode    
	   and e.BranchCode = d.BranchCode
	   and e.DONo = d.DONo
	  left join svMstCustomerVehicle f
		on f.CompanyCode = a.CompanyCode    
	   and f.ChassisCode = a.ChassisCode
	   and f.ChassisNo = a.ChassisNo
	 where 1 = 1
	   and a.CompanyCode = @CompanyCode
	   and a.BranchCode = (case @IsHolding when 1 then a.BranchCode else @BranchCode end)
	   and isnull(a.IsReturn, 0) = 0
	   and year(e.DODate) = year(getdate())
	   and exists (
			select 1 from CsTDayCall 
			 where CompanyCode = a.CompanyCode
			   and CustomerCode = b.CustomerCode
			   and Chassis = a.ChassisCode + convert(varchar, a.ChassisNo))
end   

GO
/****** Object:  StoredProcedure [dbo].[uspfn_CsRptBirthday]    Script Date: 12/13/2013 9:22:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--uspfn_CsRptBirthday '%','%','%'
CREATE proc [dbo].[uspfn_CsRptBirthday]
@CompanyCode varchar(20),
@pDateFrom datetime,
@pDateTo datetime
as
begin
select CompanyCode = a.CompanyCode
     , a.CustomerCode
	 , PeriodOfYear = datepart(year, a.SKPDate)
     , CompanyName = b.CompanyName
     , a.CustomerName
     , CustomerAddress = a.Address1
     , CustomerTelephone = a.PhoneNo
     , CustomerBirthDate = a.BirthDate
	 , CustomerTypeOfGift = c.TypeOfGift
	 , CustomerGiftSentDate = c.SentGiftDate
	 , CustomerGiftReceivedDate = c.ReceivedGiftDate
	 --, ParentGiftSentDate = (
		--select top 1
		--	x.SentGiftDate
		--from
		--	CsCustRelation x
		--where
		--	x.CompanyCode = a.CompanyCode
		--	and
		--	x.CustomerCode = a.CustomerCode
		--	and
		--	x.RelationType = ''
	 --)
	 , SpouseGiftSentDate = (
		select top 1
			x.SentGiftDate
		from
			CsCustRelation x
		where
			x.CompanyCode = a.CompanyCode
			and
			x.CustomerCode = a.CustomerCode
			and
			x.RelationType = 'SPOUSE'
	 )
	 , ChildGiftSentDate1 =(
		select top 1
			x.SentGiftDate
		from
			CsCustRelation x
		where
			x.CompanyCode = a.CompanyCode
			and
			x.CustomerCode = a.CustomerCode
			and
			x.RelationType = 'CHILD_1'
	 )
	 , ChildGiftSentDate2 = (
		select top 1
			x.SentGiftDate
		from
			CsCustRelation x
		where
			x.CompanyCode = a.CompanyCode
			and
			x.CustomerCode = a.CustomerCode
			and
			x.RelationType = 'CHILD_2'
	 )
	 , ChildGiftSentDate3 = (
		select top 1
			x.SentGiftDate
		from
			CsCustRelation x
		where
			x.CompanyCode = a.CompanyCode
			and
			x.CustomerCode = a.CustomerCode
			and
			x.RelationType = 'CHILD_3'
	 )
	 --, ParentGiftReceivedDate = c.SouvRcvParent
	 , SpouseGiftReceivedDate = (
		select top 1
			x.ReceivedGiftDate
		from
			CsCustRelation x
		where
			x.CompanyCode = a.CompanyCode
			and
			x.CustomerCode = a.CustomerCode
			and
			x.RelationType = 'SPOUSE'
	 )
	 , ChildGiftReceivedDate1 = (
		select top 1
			x.ReceivedGiftDate
		from
			CsCustRelation x
		where
			x.CompanyCode = a.CompanyCode
			and
			x.CustomerCode = a.CustomerCode
			and
			x.RelationType = 'CHILD_1'
	 )
	 , ChildGiftReceivedDate2 = (
		select top 1
			x.ReceivedGiftDate
		from
			CsCustRelation x
		where
			x.CompanyCode = a.CompanyCode
			and
			x.CustomerCode = a.CustomerCode
			and
			x.RelationType = 'CHILD_2'
	 )
	 , ChildGiftReceivedDate3 = (
		select top 1
			x.ReceivedGiftDate
		from
			CsCustRelation x
		where
			x.CompanyCode = a.CompanyCode
			and
			x.CustomerCode = a.CustomerCode
			and
			x.RelationType = 'CHILD_3'
	 )
	 -- ======================================== Spouse
	 , SpouseName = (
		select top 1
			x.FullName	
		from 
			CsCustRelation x
		where
			x.CompanyCode = a.CompanyCode
			and
			x.CustomerCode = a.CustomerCode
			and
			x.RelationType = 'SPOUSE'
	 )
	 , SpouseTelephone = (
		select top 1
			x.PhoneNo	
		from 
			CsCustRelation x
		where
			x.CompanyCode = a.CompanyCode
			and
			x.CustomerCode = a.CustomerCode
			and
			x.RelationType = 'SPOUSE'
	 ),
	 SpouseRelation = (
		select top 1
			x.RelationInfo	
		from 
			CsCustRelation x
		where
			x.CompanyCode = a.CompanyCode
			and
			x.CustomerCode = a.CustomerCode
			and
			x.RelationType = 'SPOUSE'
	 ),
	 SpouseBirthDate = (
		select top 1
			x.BirthDate	
		from 
			CsCustRelation x
		where
			x.CompanyCode = a.CompanyCode
			and
			x.CustomerCode = a.CustomerCode
			and
			x.RelationType = 'SPOUSE'
	 )
	 , SpouseComment = (
		select top 1
			x.Comment
		from 
			CsCustRelation x
		where
			x.CompanyCode = a.CompanyCode
			and
			x.CustomerCode = a.CustomerCode
			and
			x.RelationType = 'SPOUSE'
	 )
	 , SpouseTypeOfGift = (
		select top 1
			x.TypeOfGift
		from 
			CsCustRelation x
		where
			x.CompanyCode = a.CompanyCode
			and
			x.CustomerCode = a.CustomerCode
			and
			x.RelationType = 'SPOUSE'
	 )
	 -- ======================================== Child 1
	 , ChildName1 = (
		select top 1
			x.FullName	
		from 
			CsCustRelation x
		where
			x.CompanyCode = a.CompanyCode
			and
			x.CustomerCode = a.CustomerCode
			and
			x.RelationType = 'CHILD_1'
	 )
	 , ChildTelephone1 = (
		select top 1
			x.PhoneNo	
		from 
			CsCustRelation x
		where
			x.CompanyCode = a.CompanyCode
			and
			x.CustomerCode = a.CustomerCode
			and
			x.RelationType = 'CHILD_1'
	 )
	 , ChildBirthDate1 = (
		select top 1
			x.BirthDate	
		from 
			CsCustRelation x
		where
			x.CompanyCode = a.CompanyCode
			and
			x.CustomerCode = a.CustomerCode
			and
			x.RelationType = 'CHILD_1'
	 )
	 , ChildComment1 = (
		select top 1
			x.Comment
		from 
			CsCustRelation x
		where
			x.CompanyCode = a.CompanyCode
			and
			x.CustomerCode = a.CustomerCode
			and
			x.RelationType = 'CHILD_1'
	 )
	 , ChildTypeOfGift1 = (
		select top 1
			x.TypeOfGift
		from 
			CsCustRelation x
		where
			x.CompanyCode = a.CompanyCode
			and
			x.CustomerCode = a.CustomerCode
			and
			x.RelationType = 'CHILD_1'
	 )
	 -- ======================================== Child 2
	 , ChildName2 = (
		select top 1
			x.FullName	
		from 
			CsCustRelation x
		where
			x.CompanyCode = a.CompanyCode
			and
			x.CustomerCode = a.CustomerCode
			and
			x.RelationType = 'CHILD_2'
	 )
	 , ChildTelephone2 = (
		select top 1
			x.PhoneNo	
		from 
			CsCustRelation x
		where
			x.CompanyCode = a.CompanyCode
			and
			x.CustomerCode = a.CustomerCode
			and
			x.RelationType = 'CHILD_2'
	 )
	 , ChildBirthDate2 = (
		select top 1
			x.BirthDate	
		from 
			CsCustRelation x
		where
			x.CompanyCode = a.CompanyCode
			and
			x.CustomerCode = a.CustomerCode
			and
			x.RelationType = 'CHILD_2'
	 ),
	 ChildComment2 = (
		select top 1
			x.Comment
		from 
			CsCustRelation x
		where
			x.CompanyCode = a.CompanyCode
			and
			x.CustomerCode = a.CustomerCode
			and
			x.RelationType = 'CHILD_2'
	 )
	 , ChildTypeOfGift2 = (
		select top 1
			x.TypeOfGift
		from 
			CsCustRelation x
		where
			x.CompanyCode = a.CompanyCode
			and
			x.CustomerCode = a.CustomerCode
			and
			x.RelationType = 'CHILD_2'
	 )
	 -- ======================================== Child 3
	 , ChildName3 = (
		select top 1
			x.FullName	
		from 
			CsCustRelation x
		where
			x.CompanyCode = a.CompanyCode
			and
			x.CustomerCode = a.CustomerCode
			and
			x.RelationType = 'CHILD_3'
	 )
	 , ChildTelephone3 = (
		select top 1
			x.PhoneNo	
		from 
			CsCustRelation x
		where
			x.CompanyCode = a.CompanyCode
			and
			x.CustomerCode = a.CustomerCode
			and
			x.RelationType = 'CHILD_3'
	 )
	 , ChildBirthDate3 = (
		select top 1
			x.BirthDate	
		from 
			CsCustRelation x
		where
			x.CompanyCode = a.CompanyCode
			and
			x.CustomerCode = a.CustomerCode
			and
			x.RelationType = 'CHILD_3'
	 ),
	 ChildComment3 = (
		select top 1
			x.Comment
		from 
			CsCustRelation x
		where
			x.CompanyCode = a.CompanyCode
			and
			x.CustomerCode = a.CustomerCode
			and
			x.RelationType = 'CHILD_3'
	 )
	 , ChildTypeOfGift3 = (
		select top 1
			x.TypeOfGift
		from 
			CsCustRelation x
		where
			x.CompanyCode = a.CompanyCode
			and
			x.CustomerCode = a.CustomerCode
			and
			x.RelationType = 'CHILD_3'
	 )
     , BPKDate = convert(varchar(11),getdate(),106)
     , e.SalesModelCode as CarType
     , e.ColourCode as Color
     , h.PoliceRegNo 
     , e.EngineCode + convert(varchar, e.EngineNo) as Engine 
     , (e.ChassisCode + convert(varchar, e.ChassisNo)) Chassis
     , f.Salesman as SalesmanCode
     , g.EmployeeName as SalesmanName
	 , CustomerComment = c.Comment
	 , c.AdditionalInquiries
	 , c.Status
  from gnMstCustomer a
  inner join gnMstOrganizationHdr b
    on b.CompanyCode = a.CompanyCode
  inner join csCustBirthday c
	on c.CompanyCode = a.CompanyCode
   and c.CustomerCode = a.CustomerCode
  inner join CsTdayCall d
	on a.CompanyCode = d.CompanyCode
   and a.CustomerCode = d.CustomerCode
  left join omTrSalesSOVin e
    on e.CompanyCode = d.CompanyCode
   and (e.ChassisCode + convert(varchar, e.ChassisNo)) = d.Chassis
  left join omTrSalesSO f
    on f.CompanyCode = e.CompanyCode
   and f.BranchCode = e.BranchCode
   and f.SONo = e.SONo
  left join gnMstEmployee g
    on g.CompanyCode = f.CompanyCode
   and g.BranchCode = f.BranchCode
   and g.EmployeeID = f.Salesman
  left join svMstCustomerVehicle h
    on h.CompanyCode = d.CompanyCode
   and h.ChassisCode + convert(varchar, h.ChassisNo) = d.Chassis
  where 1=1
	and convert(varchar, @pDateFrom, 112) < left(convert(varchar, a.BirthDate, 112), 6) + convert(varchar, convert(int, right(convert(varchar, a.BirthDate, 112), 2)) + 3)
	and convert(varchar, @pDateTo, 112) > left(convert(varchar, a.BirthDate, 112), 6) + convert(varchar, convert(int, right(convert(varchar, a.BirthDate, 112), 2)) + 3)

end


GO
/****** Object:  StoredProcedure [dbo].[uspfn_CsRptBPKBNOF]    Script Date: 12/13/2013 9:22:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--uspfn_CsRptBPKBNOF '%','9-9-2006','9-9-2014'
CREATE proc [dbo].[uspfn_CsRptBPKBNOF]
@CompanyCode varchar(20),
@pDateFrom datetime,
@pDateTo datetime,
@pStatus char(1)
as
begin
select a.CompanyCode
     , b.CustomerCode
	 , i.CompanyName CompanyName
     , j.CompanyName BranchName
	 , e.BranchCode
     , c.CustomerName
     , c.PhoneNo
	 , BPKDate = convert(varchar(11),getdate(),106)
     , STNKDate = convert(varchar(11),getdate(),106)
	 , h.LeasingCode
  	 --Finance Institution
	 , h.Tenor
	 , BpkbReadyDate = convert(VARCHAR(11), h.BpkbReadyDate, 106)
	 , BpkbPickUp = convert(VARCHAR(11), h.BpkbPickUp,106)
	 , h.CustomerCategory
	 , h.ReqKtp
	 , h.ReqStnk
	 , h.ReqSuratKuasa
     , e.SalesModelCode as CarType
     , e.ColourCode as Color
     , d.PoliceRegNo
     , (a.ChassisCode + convert(varchar, a.ChassisNo)) as Chassis
     , (a.EngineCode + convert(varchar, a.EngineNo)) as Engine
     , f.Salesman as SalesmanCode
     , g.EmployeeName as SalesmanName
	 , h.Comment
	 , h.Additional
     , (case h.Status when 0 then 'In Progress' else 'Finish' end) as StatusInfo
     , left(c.Address1, 40) as Address
   , h.ReqInfoLeasing  
   , h.ReqInfoCust
   , h.Status
   , convert(varchar(15),h.CreatedDate, 106) as CreatedDate    
   , convert(varchar(15),h.FinishDate, 106) as FinishDate
  from omTrSalesInvoiceVin a      
  left join omTrSalesInvoice b      
    on b.CompanyCode = a.CompanyCode      
   and b.BranchCode = a.BranchCode      
   and b.InvoiceNo = a.InvoiceNo      
  left join gnMstCustomer c      
    on c.CompanyCode = a.CompanyCode      
   and c.CustomerCode = b.CustomerCode      
  left join svMstCustomerVehicle d      
    on d.CompanyCode = a.CompanyCode      
   and d.ChassisCode = a.ChassisCode      
   and d.ChassisNo = a.ChassisNo      
  left join omTrSalesSOVin e      
    on e.CompanyCode = a.CompanyCode      
   and e.ChassisCode = a.ChassisCode      
   and e.ChassisNo = a.ChassisNo      
  left join omTrSalesSO f      
    on f.CompanyCode = e.CompanyCode
   and f.BranchCode = e.BranchCode
   and f.SONo = e.SONo
  left join gnMstEmployee g
    on g.CompanyCode = f.CompanyCode
   and g.BranchCode = f.BranchCode
   and g.EmployeeID = f.Salesman
  LEFT JOIN CsCustBpkb h
  on h.CompanyCode = a.CompanyCode
  and h.Chassis = a.ChassisCode + convert(varchar(20), a.ChassisNo)  
  left join gnMstOrganizationHdr i          
  on i.CompanyCode = f.CompanyCode          
  left join gnMstCoProfile j          
  on j.CompanyCode = f.CompanyCode          
  and j.BranchCode = f.BranchCode 
  where 1=1
	and j.companycode like @companycode
   and h.Status like case when @pStatus = 0 then @pStatus else '%' end
   and h.Status not like case when @pStatus = 0 then '' else '0' end
end


GO
/****** Object:  StoredProcedure [dbo].[uspfn_CsRptStnkExt]    Script Date: 12/13/2013 9:22:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE proc [dbo].[uspfn_CsRptStnkExt]
@CompanyCode varchar(20),
@pDateFrom datetime,
@pDateTo datetime,
@pStatus char(1)
as
begin
select a.CompanyCode     
     , b.CustomerCode     
	 , e.BranchCode    
	 , BPKDate = convert(varchar(11),getdate(),106)      
     , STNKDate = convert(varchar(11),getdate(),106) 
  	 , h.LeasingCode
  	 --Finance Institution
  	 , h.Tenor
     , h.IsStnkExtend
  	 , h.CustomerCategory
  	 , h.ReqKtp
  	 , h.ReqStnk
  	 , h.ReqBpkb
     , e.SalesModelCode as CarType
     , e.ColourCode as Color    
     , d.PoliceRegNo     
     , (a.ChassisCode + convert(varchar, a.ChassisNo)) as Chassis     
     , (a.EngineCode + convert(varchar, a.EngineNo)) as Engine 
     , f.Salesman as SalesmanCode     
     , g.EmployeeName as SalesmanName    
     , c.CustomerName    
     , c.PhoneNo      
     , i.CompanyName CompanyName        
     , j.CompanyName BranchName     
	 , StnkExpiredDate =  Convert(varchar(11), h.StnkExpiredDate, 106)
     , (case h.Status when 0 then 'In Progress' else 'Finish' end) as StatusInfo  
     , left(c.Address1, 40) as Address
  	 , h.Comment
  	 , h.Additional
  	 , h.Status
  	 , convert(varchar(15),h.CreatedDate, 106) as CreatedDate  
  	 , convert(varchar(15),h.FinishDate, 106) as FinishDate
  from omTrSalesInvoiceVin a    
  left join omTrSalesInvoice b    
    on b.CompanyCode = a.CompanyCode    
   and b.BranchCode = a.BranchCode    
   and b.InvoiceNo = a.InvoiceNo    
  left join gnMstCustomer c    
    on c.CompanyCode = a.CompanyCode    
   and c.CustomerCode = b.CustomerCode    
  left join svMstCustomerVehicle d    
    on d.CompanyCode = a.CompanyCode    
   and d.ChassisCode = a.ChassisCode    
   and d.ChassisNo = a.ChassisNo    
  left join omTrSalesSOVin e    
    on e.CompanyCode = a.CompanyCode    
   and e.ChassisCode = a.ChassisCode    
   and e.ChassisNo = a.ChassisNo    
  left join omTrSalesSO f    
    on f.CompanyCode = e.CompanyCode    
   and f.BranchCode = e.BranchCode    
   and f.SONo = e.SONo    
  left join gnMstEmployee g    
    on g.CompanyCode = f.CompanyCode    
   and g.BranchCode = f.BranchCode    
   and g.EmployeeID = f.Salesman    
  LEFT JOIN CsStnkExt h  
  on h.CompanyCode = a.CompanyCode  
  and h.Chassis = a.ChassisCode + convert(varchar(20), a.ChassisNo)
  left join gnMstOrganizationHdr i        
  on i.CompanyCode = f.CompanyCode        
  left join gnMstCoProfile j        
  on j.CompanyCode = f.CompanyCode        
  and j.BranchCode = f.BranchCode   
	where 1=1
		and j.companycode like @companycode
   and h.Status like case when @pStatus = 0 then @pStatus else '%' end
   and h.Status not like case when @pStatus = 0 then '' else '0' end
end


GO
/****** Object:  StoredProcedure [dbo].[uspfn_CsRptTDayCall]    Script Date: 12/13/2013 9:22:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


--uspfn_CsRptTDayCall '%','9-9-2010','9-9-2013','4'
CREATE proc [dbo].[uspfn_CsRptTDayCall]
@CompanyCode varchar(20),
@pDateFrom datetime,
@pDateTo datetime,
@pStatus char(1)
as
begin
select a.CompanyCode 
     , a.CustomerCode
     , b.CustomerName
     , b.Address1
     , b.PhoneNo
     , BPKDate = convert(varchar(11),getdate(),106)
	 , STNKDate = convert(varchar(11),getdate(),106)
     , c.SalesModelCode as CarType
     , c.ColourCode as Color
     , f.PoliceRegNo 
     , c.EngineCode + convert(varchar, c.EngineNo) as Engine 
     , a.Chassis 
     , d.Salesman as SalesmanCode
     , e.EmployeeName as SalesmanName
     , a.IsDeliveredA 
     , a.IsDeliveredB 
     , a.IsDeliveredC 
     , a.IsDeliveredD 
     , a.IsDeliveredE 
     , a.IsDeliveredF 
     , a.Comment 
     , a.Additional 
     , a.Status 
     , (case a.Status when 0 then 'In Progress' else 'Finish' end) as StatusInfo
     , a.CreatedDate as InputDate
     , b.CustomerName
     , left(b.Address1, 40) as Address
     , c.BranchCode
     , (select top 1 BranchName from gnMstOrganizationDtl where branchcode = c.BranchCode) BranchName
     , c.CompanyCode
     , (select top 1 CompanyName from gnMstOrganizationHdr where companycode = c.CompanyCode) CompanyName
     , a.FinishDate
  from CsTDayCall a
  left join gnMstCustomer b
    on b.CompanyCode = a.CompanyCode
   and b.CustomerCode = a.CustomerCode
  left join omTrSalesSOVin c
    on c.CompanyCode = a.CompanyCode
   and (c.ChassisCode + convert(varchar, c.ChassisNo)) = a.Chassis
  left join omTrSalesSO d
    on d.CompanyCode = c.CompanyCode
   and d.BranchCode = c.BranchCode
   and d.SONo = c.SONo
  left join gnMstEmployee e
    on e.CompanyCode = d.CompanyCode
   and e.BranchCode = d.BranchCode
   and e.EmployeeID = d.Salesman
  left join svMstCustomerVehicle f
    on f.CompanyCode = a.CompanyCode
   and f.ChassisCode + convert(varchar, f.ChassisNo) = a.Chassis
  where 1=1
   and a.Status like case when @pStatus = 0 then @pStatus else '%' end
   and a.Status not like case when @pStatus = 0 then '' else '0' end
	--and convert(varchar, @pDateFrom, 112) < left(convert(varchar, BPKDate, 112), 6) + convert(varchar, convert(int, right(convert(varchar, BPKDate, 112), 2)) + 3)
	--and convert(varchar, @pDateTo, 112) > left(convert(varchar, BPKDate, 112), 6) + convert(varchar, convert(int, right(convert(varchar, BPKDate, 112), 2)) + 3)

end


GO
/****** Object:  StoredProcedure [dbo].[uspfn_CsSaveCustVehicle]    Script Date: 12/13/2013 9:22:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[uspfn_CsSaveCustVehicle]
	@CompanyCode varchar(20),
	@CustomerCode varchar(50),
	@Chassis varchar(50),
	@StnkDate datetime,
	@BpkbDate datetime,
	@UserID varchar(20)
as

if exists ( select 1 from CsCustomerVehicle
			 where CompanyCode = @CompanyCode 
			   and Chassis = @Chassis)
begin
	update CsCustomerVehicle
	   set CustomerCode = @CustomerCode
	     , StnkDate = @StnkDate
	     , BpkbDate = @BpkbDate
	     , UpdatedBy = @UserID
	     , UpdatedDate = getdate()
	 where CompanyCode = @CompanyCode
	   and Chassis = @Chassis    
end
else
begin
	insert into CsCustomerVehicle (CompanyCode, CustomerCode, Chassis, StnkDate, BpkbDate, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
	values (@CompanyCode, @CustomerCode, @Chassis, @StnkDate, @BpkbDate, @UserID, getdate(), @UserID, getdate())
end

			   

GO
