/****** Object:  StoredProcedure [dbo].[uspfn_CsRptBirthday]    Script Date: 11/11/2013 11:40:41 AM ******/
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


