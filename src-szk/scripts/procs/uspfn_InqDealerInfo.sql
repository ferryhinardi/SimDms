alter procedure uspfn_InqDealerInfo
	@ProductType varchar(2) = ''
as

select a.DealerCode, a.DealerName, a.ProductType
     , b.BranchCode as OutletCode
	 , b.BranchName as OutletName
     , c.TransDate as SalesDate
	 , d.TransDate as ServiceDate
	 , e.TransDate as SparepartDate
	 , ArDate = (select top 1 LastUpdateDate from GnMstDocument where CompanyCode = a.DealerCode and BranchCode = b.BranchCode and DocumentName like 'AR%' order by LastUpdateDate desc)
	 , ApDate = (select top 1 LastUpdateDate from GnMstDocument where CompanyCode = a.DealerCode and BranchCode = b.BranchCode and DocumentName like 'AP%' order by LastUpdateDate desc)
	 , GlDate = (select top 1 LastUpdateDate from GnMstDocument where CompanyCode = a.DealerCode and BranchCode = b.BranchCode and DocumentName like 'GL%' order by LastUpdateDate desc)
  from DealerInfo a
  left join OutletInfo b
    on b.CompanyCode = a.DealerCode
  left join GnMstCoProfileSales c
    on c.CompanyCode = b.CompanyCode
   and c.BranchCode = b.BranchCode
  left join GnMstCoProfileService d
    on d.CompanyCode = b.CompanyCode
   and d.BranchCode = b.BranchCode
  left join GnMstCoProfileSpare e
    on e.CompanyCode = b.CompanyCode
   and e.BranchCode = b.BranchCode
  left join GnMstCoProfileFinance f
    on f.CompanyCode = b.CompanyCode
   and f.BranchCode = b.BranchCode
 where 1 = 1
   and a.ProductType = (case rtrim(@ProductType) when '' then a.ProductType else @ProductType end)
   and (c.TransDate is not null
     or d.TransDate is not null
     or e.TransDate is not null
   )

go

exec uspfn_InqDealerInfo '2W'
