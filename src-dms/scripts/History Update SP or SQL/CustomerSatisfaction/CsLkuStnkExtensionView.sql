IF(OBJECT_ID('CsLkuStnkExtensionView') is not null)
	drop view dbo.CsLkuStnkExtensionView
GO

create view [dbo].[CsLkuStnkExtensionView]
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
				when c.Status = 0 then 'Y'
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
						 (case when year(getdate()) - year(isnull(b.StnkDate, '')) >= 0
							   then b.StnkDate
							   when getdate() < dateadd(year, year(getdate()) - year(b.StnkDate), b.StnkDate)
							   then dateadd(year, year(getdate()) - year(b.StnkDate), b.StnkDate)
							   else dateadd(year, year(getdate()) - year(b.StnkDate) + 1, b.StnkDate)
						 end)
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

GO


