
go
if object_id('CsCustBirthdayView') is not null
	drop view CsCustBirthdayView

go
create view CsCustBirthdayView
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
select * from CsCustBirthdayView


