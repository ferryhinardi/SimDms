DROP FUNCTION [dbo].[GetRetrievalEstimationDate]
GO

CREATE function [dbo].[GetRetrievalEstimationDate] (@CompanyCode nvarchar(100), @CustomerCode nvarchar(100))
returns datetime
as
begin
	return	(select top 1 x.RetrievalEstimationDate 
			  from CsBpkbRetrievalInformation x 
			 where isnull(x.IsDeleted, 0) = 0
			   and x.CompanyCode = @CompanyCode
			   and x.CustomerCode = @CustomerCode
			 order by x.RetrievalEstimationDate desc)
end
GO

DROP VIEW [dbo].[CsLkuBirthdayView]
GO

CREATE view [dbo].[CsLkuBirthdayView]
as
select a.CompanyCode
	 , a.BranchCode
	 , a.CustomerCode
	 , a.CustomerName
	 , CustomerAddress = a.Address
	 , PeriodOfYear = c.PeriodYear
	 , c.AdditionalInquiries
	 , Status = c.Status
	 , CustomerTelephone = a.PhoneNo
	 , CustomerHandphone = a.HPNo
	 , CustomerAddPhone1 = a.AddPhone1
	 , CustomerAddPhone2 = a.AddPhone2
	 , CustomerBirthDate = a.BirthDate
	 , CustomerGiftSentDate = c.SentGiftDate 
	 , CustomerGiftReceivedDate = c.ReceivedGiftDate
	 , CustomerTypeOfGift = c.TypeOfGift
	 , CustomerComment = c.Comment
	 , SpouseName = c.SpouseName
	 , SpouseTelephone = c.SpouseTelephone
	 , SpouseRelation = c.SpouseRelation
	 , SpouseBirthDate = c.SpouseBirthday
	 , SpouseGiftSentDate = c.SpouseGiftSentDate
	 , SpouseGiftReceivedDate = c.SpouseGiftReceivedDate
	 , SpouseTypeOfGift = c.SpouseTypeOfGift
	 , SpouseComment = c.SpouseComment
	 , ChildName1 = c.ChildName1
	 , ChildTelephone1 = c.ChildTelephone1
	 , ChildRelation1 = c.ChildRelation1
	 , ChildBirthDate1 = c.ChildBirthday1
	 , ChildGiftSentDate1 = c.ChildGiftSentDate1
	 , ChildGiftReceivedDate1 = c.ChildGiftReceivedDate1
	 , ChildTypeOfGift1 = c.ChildTypeOfGift1
	 , ChildComment1 = c.ChildComment1
	 , ChildName2 = c.ChildName2
	 , ChildTelephone2 = c.ChildTelephone2
	 , ChildRelation2 = c.ChildRelation2
	 , ChildBirthDate2 = c.ChildBirthday2
	 , ChildGiftSentDate2 = c.ChildGiftSentDate2
	 , ChildGiftReceivedDate2 = c.ChildGiftReceivedDate2
	 , ChildTypeOfGift2 = c.ChildTypeOfGift2
	 , ChildComment2 = c.ChildComment2
	 , ChildName3 = c.ChildName3
	 , ChildTelephone3 = c.ChildTelephone3
	 , ChildRelation3 = c.ChildRelation3
	 , ChildBirthDate3 = c.ChildBirthday3
	 , ChildGiftSentDate3 = c.ChildGiftSentDate3
	 , ChildGiftReceivedDate3 = c.ChildGiftReceivedDate3
	 , ChildTypeOfGift3 = c.ChildTypeOfGift3
	 , ChildComment3 = c.ChildComment3
	 , NumberOfChildren = (
			(
				case 
					when isnull(len(c.ChildName1), 0) = 0 then 0
					else 1
				end
			)
			+
			(
				case 
					when isnull(len(c.ChildName2), 0) = 0 then 0
					else 1
				end
			)
			+
			(
				case 
					when isnull(len(c.ChildName3), 0) = 0 then 0
					else 1
				end
			)
	   )
	 , NumberOfSpouse = ( 
			select count(x.CustomerCode) 
			  from CsCustBirthday x 
			 where x.CompanyCode = a.CompanyCode
			   and x.CustomerCode = a.CustomerCode
			   and isnull(x.SpouseName, '') != ''
	   )
	 , Outstanding = (
			case
				when isnull(c.CustomerCode, '') = '' then 'Y'
				else 'N'
			end
	   )
	 , InputDate = c.CreatedDate
	 , c.Reason
	 , c.TypeOfGift
  from CsCustomerView a
  left join CsCustBirthday c
    on c.CompanyCode = a.CompanyCode
   and c.CustomerCode = a.CustomerCode

GO

DROP VIEW [dbo].[CsLkuBpkbReminderView]
GO

CREATE view [dbo].[CsLkuBpkbReminderView]  
as  
select a.CompanyCode  
     , a.BranchCode  
  , a.CustomerCode  
  , a.CustomerName  
  , b.DODate  
  , b.Chassis  
  , b.Engine  
  , b.SalesModelCode  
  , b.SalesModelYear  
  , b.ColourCode  
  , BpkbReadyDate = b.BPKBInDate
  , BpkbPickUp = b.BPKBOutDate
  , b.BPKDate  
  , b.IsLeasing  
  , b.LeasingCo  
  , b.Installment  
  , case isnull(b.isLeasing, 0) when 0 then 'Cash' else 'Leasing' end as LeasingDesc  
  , case isnull(b.isLeasing, 0) when 0 then '-' else b.LeasingName end as LeasingName  
  , case isnull(b.isLeasing, 0) when 0 then '-' else (convert(varchar, isnull(b.Installment, 0)) + ' Month') end as Tenor  
  , a.Address  
  , a.PhoneNo  
  , a.HpNo  
  , a.AddPhone1  
  , a.AddPhone2  
  , b.SalesmanCode   
  , b.SalesmanName  
  , c.ReqInfoLeasing
  , c.ReqInfoCust
  , c.ReqKtp  
     , c.ReqStnk  
     , c.ReqSuratKuasa  
     , c.Comment  
     , c.Additional  
  , BpkbDate = d.BPKBDate  
  , StnkDate = d.StnkDate  
  , Category = (   
   case   
     when isnull(b.isLeasing, 0) = 0 then 'Tunai'   
     else 'Leasing'  
   end  
    )  
  , c.Status  
  , (case c.Status when 1 then 'Finish' else 'In Progress' end) as StatusInfo  
  , b.PoliceRegNo  
  , InputDate = c.CreatedDate  
  , CustCreatedDate = a.CreatedDate  
  , DelayedRetrievalDate = (  
   select top 1 x.RetrievalEstimationDate  
     from CsBpkbRetrievalInformation x  
    where x.CompanyCode = a.CompanyCode  
      and x.CustomerCode = a.CustomerCode  
    order by x.RetrievalEstimationDate desc  
    )  
  , DelayedRetrievalNote = (  
   select top 1 x.Notes  
     from CsBpkbRetrievalInformation x  
    where x.CompanyCode = a.CompanyCode  
      and x.CustomerCode = a.CustomerCode  
    order by x.RetrievalEstimationDate desc  
    )  
  , Outstanding = (
   case
		--when isnull(b.isLeasing, 0) = 0 and (b.BPKBOutDate is null)
		--  --OR isnull(c.Status, 0) = 1
		--then 'N'
		when isnull(b.isLeasing, 0) = 0 and dbo.GetRetrievalEstimationDate(a.CompanyCode, a.CustomerCode) <= getdate() and b.BPKBOutDate is null
		then 'Y'
		when isnull(b.isLeasing, 0) = 0 and b.BPKBOutDate is not null
		then 'N'
		when isnull(c.CustomerCode, '') = ''
		then 'Y'
		else 'N'
   end   
    ),  
    c.Reason  
  from CsCustomerView a  
  left join CsCustomerVehicleView b  
    on b.CompanyCode = a.CompanyCode  
   and b.BranchCode = a.BranchCode  
   and b.CustomerCode = a.CustomerCode  
  left join CsCustBpkb c  
    on c.CompanyCode = b.CompanyCode  
   and c.CustomerCode = b.CustomerCode
   and c.Chassis = b.Chassis
  left join CsCustomerVehicle d  
    on d.CompanyCode = c.CompanyCode  
   and d.Chassis = b.Chassis  
 where (case when isnull(c.CustomerCode, '') = '' and isnull(b.isLeasing, 0) = 0 then b.BPKBOutDate else null end) is null
 --tunai bpkb sdh diserahkan tdk muncul

GO

DROP VIEW [dbo].[CsLkuFeedbackView]
GO

create view [dbo].[CsLkuFeedbackView]
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
     , PoliceRegNo = isnull(m.PoliceRegNo, l.PoliceRegNo)
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
  left join svMstCustomerVehicle m
	on m.CompanyCode = a.CompanyCode
   and m.ChassisCode = a.ChassisCode
   and m.ChassisNo = a.ChassisNo

 where c.CustomerType = 'I'

GO

DROP VIEW [dbo].[CsLkuStnkExtensionView]
GO

CREATE view [dbo].[CsLkuStnkExtensionView]
as
select a.CompanyCode
     , a.BranchCode
	 , a.CustomerCode
	 , a.CustomerName 
	 , b.Chassis
	 , b.Engine
	 , b.SalesModelCode
	 , b.SalesModelYear
	 , b.ColourCode
	 , b.DODate
	 , BpkbDate = b.BPKBOutDate--isnull(c.BpkbDate, b.BPKDate) 
	 , StnkDate = b.StnkDate
	 , b.BPKDate
	 , b.IsLeasing
	 , b.LeasingCo
	 , b.Installment
	 , LeasingDesc = case isnull(b.isLeasing, 0) when 0 then 'Cash' else 'Leasing' end
	 , LeasingName = case isnull(b.isLeasing, 0) when 0 then '-' else b.LeasingName end
	 , Tenor = case isnull(b.isLeasing, 0) when 0 then '-' else (convert(varchar, isnull(b.Installment, 0)) + ' Month') end
	 , Address = isnull(a.Address, '-')
	 , PhoneNo = isnull(a.PhoneNo, '-')
	 , HpNo = isnull(a.HpNo, '-')
	 , AddPhone1 = isnull(a.AddPhone1, '-')
	 , AddPhone2 = isnull(a.AddPhone2, '-')
	 , SalesmanCode = isnull(b.SalesmanCode, '-')
	 , SalesmanName = isnull(b.SalesmanName, '-')
	 , IsStnkExtend = isnull(c.IsStnkExtend, 0)
	 , CustomerCreatedDate = a.CreatedDate
	 , InputDate = c.CreatedDate
	 , Outstanding = (
			case
				when isnull(c.CustomerCode, '') = '' then 'Y'
				--when c.Status = 0 then 'Y'
--				when left(convert(varchar, c.stnkexpireddate, 121), 10) != '2000-01-01' then 'Y'
				when c.Ownership = 0 then 'N'
				when isnull(c.Ownership, '') = '' or c.Ownership = 0 then 'N'
				else 'N'
			end
	   )
	 , Category = ( 
			case 
				 when isnull(b.isLeasing, 0) = 0 then 'Tunai' 
				 else 'Leasing'
			end
	   )
	 , Salesman = isnull(b.SalesmanName, '-')
	 , StnkExpiredDate = --isnull(c.StnkExpiredDate,
						 convert(datetime, convert(varchar, (case when year(getdate()) - year(isnull(b.StnkDate, '')) >= 0
							   then b.StnkDate
							   when getdate() < dateadd(year, year(getdate()) - year(b.StnkDate), b.StnkDate)
							   then dateadd(year, year(getdate()) - year(b.StnkDate), b.StnkDate)
							   else dateadd(year, year(getdate()) - year(b.StnkDate) + 1, b.StnkDate)
						 end), 112))
	 --, c.StnkExpiredDate
	 , c.ReqKtp
	 , c.ReqStnk
	 , c.ReqBpkb
	 , c.ReqSuratKuasa
	 , c.Comment
	 , c.Additional
	 , c.Status
	 , c.Ownership
	 , StatusInfo = ''
	 , b.PoliceRegNo
	 , c.Reason
  from CsCustomerView a
  left join CsCustomerVehicleView b
    on b.CompanyCode = a.CompanyCode
   and b.BranchCode = a.BranchCode
   and b.CustomerCode = a.CustomerCode
  left join CsStnkExt c
    on c.CompanyCode = b.CompanyCode
   and c.CustomerCode = b.CustomerCode
   and c.Chassis = b.Chassis
   and c.StnkExpiredDate = convert(datetime, convert(varchar, (case when year(getdate()) - year(isnull(b.StnkDate, '')) >= 0
							   then b.StnkDate
							   when getdate() < dateadd(year, year(getdate()) - year(b.StnkDate), b.StnkDate)
							   then dateadd(year, year(getdate()) - year(b.StnkDate), b.StnkDate)
							   else dateadd(year, year(getdate()) - year(b.StnkDate) + 1, b.StnkDate)
						 end), 112))
 where year(b.bpkdate) != year(getdate())
GO

DROP VIEW [dbo].[CsLkuTDaysCallView]
GO

CREATE view [dbo].[CsLkuTDaysCallView]
as
select a.CompanyCode
     , a.BranchCode
	 , a.CustomerCode
	 , a.CustomerName
	 , a.Address 
	 , a.PhoneNo
	 , a.HPNo
	 , AddPhone1 = isnull(a.AddPhone1, '-')
	 , AddPhone2 = isnull(a.AddPhone2, '-')
	 , CreatedDate = c.CreatedDate
	 , Outstanding = (
			case
				when isnull(c.CustomerCode, '') = '' then 'Y'
				--when c.Status = 0 then 'Y'
				else 'N'
			end
	   )
     , a.BirthDate,
	   b.CarType + ' - ' + mo.[SalesModelDesc] CarType, 
	   b.Color + ' - ' + col.[RefferenceDesc1] [Color]
     --, b.CarType
     --, b.Color
     , b.PoliceRegNo
     , b.Chassis
     , b.Engine 
     , b.SONo
     , b.SalesmanCode
     , b.SalesmanName
     , b.SalesmanName as Salesman
     , a.ReligionCode
	 , b.SalesModelCode
	 , b.SalesModelYear
	 , b.DODate
	 , b.BPKDate
     , case c.IsDeliveredA when 1 then 'Ya' else 'Tidak' end IsDeliveredA
     , case c.IsDeliveredB when 1 then 'Ya' else 'Tidak' end IsDeliveredB
     , case c.IsDeliveredC when 1 then 'Ya' else 'Tidak' end IsDeliveredC
     , case c.IsDeliveredD when 1 then 'Ya' else 'Tidak' end IsDeliveredD
     , case c.IsDeliveredE when 1 then 'Ya' else 'Tidak' end IsDeliveredE
     , case c.IsDeliveredF when 1 then 'Ya' else 'Tidak' end IsDeliveredF
     , case c.IsDeliveredG when 1 then 'Ya' else 'Tidak' end IsDeliveredG
     , c.Comment
	 , c.Additional
	 , c.Status
	 , DeliveryDate = case when convert(varchar, b.DeliveryDate, 112) = '19000101' then null else b.DeliveryDate end
	 , d.LookUpValueName Religion
	 , c.Reason
  from CsCustomerView a
  left join CsCustomerVehicleView b
    on b.CompanyCode = a.CompanyCode
   and b.BranchCode = a.BranchCode
   and b.CustomerCode = a.CustomerCode
  left join CsTDayCall c
    on c.CompanyCode = b.CompanyCode
   and c.CustomerCode = b.CustomerCode
   and c.Chassis = b.Chassis
   left join gnMstLookUpDtl d ON  (d.CodeID='RLGN' and d.CompanyCode = a.CompanyCode and d.LookUpValue=a.ReligionCode)
    LEFT OUTER JOIN [dbo].[omMstRefference] col on (b.CompanyCode=col.CompanyCode and b.Color = col.[RefferenceCode] and col.[RefferenceType]='COLO')
	LEFT OUTER JOIN [dbo].[omMstModel] mo on (b.CompanyCode=mo.CompanyCode and b.Cartype = mo.[SalesModelCode])

GO
