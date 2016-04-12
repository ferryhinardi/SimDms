USE [BIT_20130325]
GO

/****** Object:  View [dbo].[SvCustomerView]    Script Date: 10/11/2013 4:52:09 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER view [dbo].[SvCustomerView]

as 

select a.CompanyCode
     , b.CompanyName
     , a.CustomerCode
     , a.CustomerName
     , a.Address1 as Address
     , a.Address1 as Address1, a.Address1 as Address2, a.Address3 as Address3, a.Address4 as Address4
     , a.PhoneNo, a.HPNo, a.NPWPNo, isnull(a.NPWPDate,'19000101') NPWPDate, a.SKPNo, isnull(a.SKPDate,'19000101')SKPDate
     , a.BirthDate
  from gnMstCustomer a
  left join gnMstOrganizationHdr b
    on b.CompanyCode = a.CompanyCode

GO


