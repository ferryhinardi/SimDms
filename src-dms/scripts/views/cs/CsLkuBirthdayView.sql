

go
if object_id('CsLkuBirthdayView') is not null
	drop view CsLkuBirthdayView

go 
create view CsLkuBirthdayView
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
					when isnull(c.ChildName1, 0) = 0 then 0
					else 1
				end
			)
			+
			(
				case 
					when isnull(c.ChildName2, 0) = 0 then 0
					else 1
				end
			)
			+
			(
				case 
					when isnull(c.ChildName3, 0) = 0 then 0
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
  from CsCustomerView a
  left join CsCustomerVehicleView b
    on b.CompanyCode = a.CompanyCode
   and b.BranchCode = a.BranchCode
   and b.CustomerCode = a.CustomerCode
  left join CsCustBirthday c
    on c.CompanyCode = a.CompanyCode
   and c.CustomerCode = a.CustomerCode


go
select * from CsLkuBirthdayView order by Outstanding asc

