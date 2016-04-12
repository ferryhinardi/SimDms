USE [SimDmsDealerDemo]
GO
/****** Object:  View [dbo].[CsTDayCallView]    Script Date: 12/13/2013 9:29:17 AM ******/
DROP VIEW [dbo].[CsTDayCallView]
GO
/****** Object:  View [dbo].[CsStnkExtView]    Script Date: 12/13/2013 9:29:17 AM ******/
DROP VIEW [dbo].[CsStnkExtView]
GO
/****** Object:  View [dbo].[CsMstCustomerView]    Script Date: 12/13/2013 9:29:17 AM ******/
DROP VIEW [dbo].[CsMstCustomerView]
GO
/****** Object:  View [dbo].[CsLkuTDayCallView]    Script Date: 12/13/2013 9:29:17 AM ******/
DROP VIEW [dbo].[CsLkuTDayCallView]
GO
/****** Object:  View [dbo].[CsLkuStnkExtView]    Script Date: 12/13/2013 9:29:17 AM ******/
DROP VIEW [dbo].[CsLkuStnkExtView]
GO
/****** Object:  View [dbo].[CsLkuFeedbackView]    Script Date: 12/13/2013 9:29:17 AM ******/
DROP VIEW [dbo].[CsLkuFeedbackView]
GO
/****** Object:  View [dbo].[CsLkuBpkbView]    Script Date: 12/13/2013 9:29:17 AM ******/
DROP VIEW [dbo].[CsLkuBpkbView]
GO
/****** Object:  View [dbo].[CsLkuBirthdayView]    Script Date: 12/13/2013 9:29:17 AM ******/
DROP VIEW [dbo].[CsLkuBirthdayView]
GO
/****** Object:  View [dbo].[CsCustomerView]    Script Date: 12/13/2013 9:29:17 AM ******/
DROP VIEW [dbo].[CsCustomerView]
GO
/****** Object:  View [dbo].[CsCustomerBuyView]    Script Date: 12/13/2013 9:29:17 AM ******/
DROP VIEW [dbo].[CsCustomerBuyView]
GO
/****** Object:  View [dbo].[CsCustHolidayView]    Script Date: 12/13/2013 9:29:17 AM ******/
DROP VIEW [dbo].[CsCustHolidayView]
GO
/****** Object:  View [dbo].[CsCustFeedbackView]    Script Date: 12/13/2013 9:29:17 AM ******/
DROP VIEW [dbo].[CsCustFeedbackView]
GO
/****** Object:  View [dbo].[CsCustBirthdayView]    Script Date: 12/13/2013 9:29:17 AM ******/
DROP VIEW [dbo].[CsCustBirthdayView]
GO
/****** Object:  View [dbo].[CsBPKBView]    Script Date: 12/13/2013 9:29:17 AM ******/
DROP VIEW [dbo].[CsBPKBView]
GO
/****** Object:  View [dbo].[CsBPKBView]    Script Date: 12/13/2013 9:29:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create view [dbo].[CsBPKBView]    
as  
select distinct a.CompanyCode
     , b.CustomerCode
	 , e.BranchCode        
     , (a.ChassisCode + convert(varchar, a.ChassisNo)) as Chassis
     , (a.EngineCode + convert(varchar, a.EngineNo)) as Engine
	 , VinNo = ((a.ChassisCode + convert(varchar, a.ChassisNo)))     
     , e.SalesModelCode as CarType        
     , e.ColourCode as Color        
     , d.PoliceRegNo
     , f.Salesman as SalesmanCode
     , g.EmployeeName as SalesmanName        
     , c.CustomerName        
     , c.PhoneNo
	 , i.CompanyName CompanyName   
     , j.CompanyName BranchName
	 , k.BpkbDate
	 , k.StnkDate
     , (case h.Status when 0 then 'In Progress' when 1 then 'Finish' else 'In Progress' end) as StatusInfo 
     , left(c.Address1, 40) as Address
	 , h.ReqKtp 
	 , h.ReqStnk 
	 , h.ReqSuratKuasa  
	 , h.ReqInfoLeasing    
	 , h.ReqInfoCust 
	 , h.Comment 
	 , h.Additional 
	 , h.Status        
	 , h.Tenor        
	 , h.LeasingCode        
	 , h.CustomerCategory  
	 , BpkbReadyDate = convert(VARCHAR(11), h.BpkbReadyDate, 106)    
	 , BpkbPickUp = convert(VARCHAR(11), h.BpkbPickUp,106)        
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
  left join CsCustomerVehicle k
    on k.CompanyCode = a.CompanyCode
   and k.CustomerCode=c.CustomerCode
   and k.Chassis = (a.ChassisCode + convert(varchar, a.ChassisNo))

GO
/****** Object:  View [dbo].[CsCustBirthdayView]    Script Date: 12/13/2013 9:29:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [dbo].[CsCustBirthdayView]
as
select distinct 
	           a.CompanyCode
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



GO
/****** Object:  View [dbo].[CsCustFeedbackView]    Script Date: 12/13/2013 9:29:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [dbo].[CsCustFeedbackView]

as 

select a.CompanyCode, a.CustomerCode, a.CustomerName, a.Address1 as [Address], a.PhoneNo
     , b.ChassisCode + convert(varchar, b.ChassisNo) Chassis
     , b.EngineCode + convert(varchar, b.EngineNo) Engine
     , b.PoliceRegNo, b.ServiceBookNo
     , a.Address1
     , c.FeedbackA
     , c.FeedbackB
     , c.FeedbackC
     , c.FeedbackD
     , b.BasicModel as CarType
     , FeedbackCode = (case isnull(c.Chassis, '') when '' then 'N' else 'Y' end)
  from GnMstCustomer a
  left join svMstCustomerVehicle b
    on b.CompanyCode = a.CompanyCode
   and b.CustomerCode = a.CustomerCode
  left join CsCustFeedback c
    on c.CompanyCode = a.CompanyCode
   and c.CustomerCode = a.CustomerCode
   and c.Chassis = b.ChassisCode + convert(varchar, b.ChassisNo)
 where 1 = 1
   and b.ChassisCode is not null

GO
/****** Object:  View [dbo].[CsCustHolidayView]    Script Date: 12/13/2013 9:29:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [dbo].[CsCustHolidayView]

as 

select a.CompanyCode
     , b.CompanyName
     , a.CustomerCode
     , a.PeriodYear
     , a.GiftSeq
     , a.ReligionCode
     , a.HolidayCode
     , d.HolidayDesc
     , a.IsGiftCard
     , a.IsGiftLetter
     , a.IsGiftSms
     , a.IsGiftSouvenir
     , a.SouvenirSent
     , a.SouvenirReceived
     , a.Comment
     , a.Additional
     , a.Status
     , (case a.Status when 0 then 'In Progress' else 'Finish' end) as StatusInfo
     , c.CustomerName
     , left(c.Address1, 40) as Address
     , c.PhoneNo
     , c.BirthDate
  from CsCustHoliday a
  left join gnMstOrganizationHdr b
    on b.CompanyCode = a.CompanyCode
  left join gnMstCustomer c
    on c.CompanyCode = a.CompanyCode
   and c.CustomerCode = a.CustomerCode
  left join CsMstHoliday d
    on d.CompanyCode = a.CompanyCode
   and d.HolidayYear = a.PeriodYear
   and d.HolidayCode = a.HolidayCode


 
   

GO
/****** Object:  View [dbo].[CsCustomerBuyView]    Script Date: 12/13/2013 9:29:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [dbo].[CsCustomerBuyView]    
    
as    
select a.CompanyCode     
     , b.CustomerCode     
     , (a.ChassisCode + convert(varchar, a.ChassisNo)) as Chassis     
     , (a.EngineCode + convert(varchar, a.EngineNo)) as Engine     
     , DeliveryDate = isnull((    
		  select top 1 y.DODate    
			from omTrSalesDODetail x    
			left join omTrSalesDO y    
			  on y.CompanyCode = x.CompanyCode    
			 and y.BranchCode = x.BranchCode    
			 and y.DONo = x.DONo    
		   where x.CompanyCode = a.CompanyCode    
			 and x.BranchCode = a.BranchCode    
			 and x.ChassisCode = a.ChassisCode    
			 and x.ChassisNo = a.ChassisNo     
       ), null)    
     , IsDeliveredA = convert(bit, 0)    
     , IsDeliveredB = convert(bit, 0)     
     , IsDeliveredC = convert(bit, 0)     
     , IsDeliveredD = convert(bit, 0)    
     , IsDeliveredE = convert(bit, 0)     
     , Comment = ''    
     , Additional = ''    
     , Status = 0    
     , e.SalesModelCode as CarType    
     , e.ColourCode as Color    
     , d.PoliceRegNo     
     , f.Salesman as SalesmanCode     
     , g.EmployeeName as SalesmanName    
     , e.BranchCode    
     , c.CustomerName    
     , left(c.Address1, 40) as Address    
     , c.PhoneNo    
     , '' as StatusInfo    
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
 where 1 = 1
   and exists ( 
select top 1 y.DODate    
  from omTrSalesDODetail x    
  left join omTrSalesDO y    
    on y.CompanyCode = x.CompanyCode    
   and y.BranchCode = x.BranchCode    
   and y.DONo = x.DONo    
 where x.CompanyCode = a.CompanyCode    
   and x.BranchCode = a.BranchCode    
   and x.ChassisCode = a.ChassisCode    
   and x.ChassisNo = a.ChassisNo  
   and y.DODate is not null
 ) 
   and not exists (    
select 1 from CsTDayCall    
 where CompanyCode = a.CompanyCode    
   and Chassis = a.ChassisCode + convert(varchar, a.ChassisNo)       
 ) 
   
GO
/****** Object:  View [dbo].[CsCustomerView]    Script Date: 12/13/2013 9:29:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [dbo].[CsCustomerView]

as 

select a.CompanyCode
     , b.CompanyName
     , a.CustomerCode
     , a.CustomerName
     , a.Address1 as Address
     , a.PhoneNo
     , a.BirthDate
  from gnMstCustomer a
  left join gnMstOrganizationHdr b
    on b.CompanyCode = a.CompanyCode


 
   

GO
/****** Object:  View [dbo].[CsLkuBirthdayView]    Script Date: 12/13/2013 9:29:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [dbo].[CsLkuBirthdayView]

as 

select a.CompanyCode 
     , a.CustomerCode 
     , a.CustomerName
     , left(a.Address1, 40) as Address
     , a.PhoneNo
     , a.BirthDate
	 , BranchCode = (select top 1 BranchCode from omTrSalesSO where CompanyCode = a.CompanyCode and CustomerCode = a.CustomerCode order by LastUpdateDate desc)
  from gnMstCustomer a
 where exists (
	select 1 from omTrSalesSO
	 where CompanyCode = a.CompanyCode
	   and CustomerCode = a.CustomerCode
 )

 --select * from CsLkuBirthdayView



GO
/****** Object:  View [dbo].[CsLkuBpkbView]    Script Date: 12/13/2013 9:29:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [dbo].[CsLkuBpkbView]
as 

select a.CompanyCode     
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
     , case isnull(k.Chassis, '') when '' then 'Y' else 'N' end OutStanding
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
  left join CsCustBpkb k
	on k.CompanyCode = a.CompanyCode
   and k.CustomerCode = b.CustomerCode
   and k.Chassis = a.ChassisCode + convert(varchar, a.ChassisNo)
 where 1 = 1
   and isnull(a.IsReturn, 0) = 0




GO
/****** Object:  View [dbo].[CsLkuFeedbackView]    Script Date: 12/13/2013 9:29:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [dbo].[CsLkuFeedbackView]
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
	 , case isnull(h.isLeasing, 0) when 0 then 'Tunai' else 'Leasing' end as Category
	 , i.CustomerName as LeasingName
	 , h.Installment
	 , convert(varchar, isnull(h.Installment, 0)) + ' Month' as Tenor
     , left(c.Address1, 40) as Address
     , c.PhoneNo
     , h.Salesman
     , j.EmployeeName as SalesmanName
     , k.IsManual
     , case (isnull(k.Chassis, '')) when '' then '-' else 
       (case k.IsManual when 1 then 'Manual' else 'System' end)
        end as Feedback
     , k.FeedbackA
     , k.FeedbackB
     , k.FeedbackC
     , k.FeedbackD
     , case (isnull(k.Chassis, '')) when '' then 1 else 0 end as IsNew
     , l.PoliceRegNo
     , case isnull(k.Chassis, '') when '' then 'Y' else 'N' end OutStanding
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
   



GO
/****** Object:  View [dbo].[CsLkuStnkExtView]    Script Date: 12/13/2013 9:29:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [dbo].[CsLkuStnkExtView]
as 

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
     , l.PoliceRegNo
     , case isnull(k.Chassis, '') when '' then 'Y' else 'N' end OutStanding
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




GO
/****** Object:  View [dbo].[CsLkuTDayCallView]    Script Date: 12/13/2013 9:29:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [dbo].[CsLkuTDayCallView]
as 

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
	 , case isnull(h.Chassis, '') when '' then 'Y' else 'N' end OutStanding
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
  left join CsTDayCall h
	on h.CompanyCode = a.CompanyCode    
   and h.CustomerCode = b.CustomerCode
   and h.Chassis = (a.ChassisCode + convert(varchar, a.ChassisNo))




GO
/****** Object:  View [dbo].[CsMstCustomerView]    Script Date: 12/13/2013 9:29:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create view [dbo].[CsMstCustomerView]
as
select distinct
       a.CompanyCode
	 , b.BranchCode
     , a.CustomerCode
	 , a.CustomerName
	 , a.BirthDate
	 , Address = (a.Address1 + ltrim(a.Address2) + ltrim(a.Address3) + ltrim(a.Address4))
	 , Outstanding = (
			case  
				when isnull(d.CustomerCode, '') != '' and d.CustomerCode = '' then '1'
				else '0'
			end
	   )
  from GnMstCustomer a
 inner join omTrSalesInvoice b
    on b.CompanyCode = a.CompanyCode
   and b.CustomerCode = a.CustomerCode
 --inner join gnMstOrganizationDtl c
 --   on c.CompanyCode = a.CompanyCode
 --  and c.BranchCode = b.BranchCode 
  left join CsCustBirthday d
    on d.CompanyCode = a.CompanyCode
   and d.CustomerCode = a.CustomerCode
 where exists (
			select x.CustomerCode
			  from svMstCustomerVehicle x
			 where x.CompanyCode = a.CompanyCode
			   and x.CustomerCode = a.CustomerCode
	   )


GO
/****** Object:  View [dbo].[CsStnkExtView]    Script Date: 12/13/2013 9:29:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [dbo].[CsStnkExtView]        
as        

select distinct a.CompanyCode           
     , b.CustomerCode
     , e.BranchCode          
     , (a.ChassisCode + convert(varchar, a.ChassisNo)) as Chassis           
     , (a.EngineCode + convert(varchar, a.EngineNo)) as Engine    
     , e.SalesModelCode as CarType          
     , e.ColourCode as Color          
     , d.PoliceRegNo           
     , f.Salesman as SalesmanCode           
     , g.EmployeeName as SalesmanName          
     , c.CustomerName          
     , c.PhoneNo
     , i.CompanyName CompanyName  
     , j.CompanyName BranchName
     , h.StnkExpiredDate
     , k.BpkbDate
	 , k.StnkDate
     , (case h.Status when 0 then 'In Progress' when 1 then 'Finish' else 'In Progress' end) as StatusInfo        
     , left(c.Address1, 40) as Address  
     , h.IsStnkExtend
     , h.ReqKtp
     , h.ReqSuratKuasa
     , h.ReqBpkb
     , h.ReqStnk
     , h.Comment
     , h.Additional
     , h.Status
     , h.Tenor
     , h.LeasingCode
     , h.CustomerCategory
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
  left join CsStnkExt h        
    on h.CompanyCode = a.CompanyCode        
   and h.Chassis = a.ChassisCode + convert(varchar(20), a.ChassisNo)      
  left join gnMstOrganizationHdr i  
    on i.CompanyCode = f.CompanyCode  
  left join gnMstCoProfile j  
    on j.CompanyCode = f.CompanyCode  
   and j.BranchCode = f.BranchCode 
  left join CsCustomerVehicle k  
    on k.CompanyCode = a.CompanyCode  
   and k.Chassis = a.ChassisCode + convert(varchar(20), a.ChassisNo) 
   
GO
/****** Object:  View [dbo].[CsTDayCallView]    Script Date: 12/13/2013 9:29:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [dbo].[CsTDayCallView]

as 

select a.CompanyCode 
     , a.CustomerCode 
     , a.Chassis 
     , a.IsDeliveredA 
     , a.IsDeliveredB 
     , a.IsDeliveredC 
     , a.IsDeliveredD 
     , a.IsDeliveredE 
     , a.IsDeliveredF
     , a.IsDeliveredG 
     , a.Comment 
     , a.Additional 
     , a.Status 
     , (case a.Status when 0 then 'In Progress' else 'Finish' end) as StatusInfo
     , a.CreatedDate as InputDate
     , b.CustomerName
     , left(b.Address1, 40) as Address
     , b.PhoneNo
     , c.BranchCode
     , c.EngineCode + convert(varchar, c.EngineNo) as Engine 
     , c.SalesModelCode as CarType
     , c.ColourCode as Color
     , d.Salesman as SalesmanCode
     , e.EmployeeName as SalesmanName
     , f.PoliceRegNo 
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




GO
