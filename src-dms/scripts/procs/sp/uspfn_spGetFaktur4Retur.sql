create procedure uspfn_spGetFaktur4Retur @CompanyCode varchar(15), @BranchCode varchar(15), 
@TypeOfGoods varchar(5)
as  
select a.FpjNo
    , a.FpjDate
    , a.CustomerCode
    , isnull((
			select CustomerName from gnMstCustomer
			 where CompanyCode = a.CompanyCode and CustomerCode = a.CustomerCode
			), '') as CustomerName  
 from spTrnSFpjHdr a, gnMstCoProfileSpare b
where 1 = 1
  and b.CompanyCode = a.CompanyCode
  and b.BranchCode = a.BranchCode
  and a.CompanyCode = @CompanyCode
  and a.BranchCode  = @BranchCode
  and a.TypeOfGoods = @TypeOfGoods
  ORDER BY CustomerName ASC