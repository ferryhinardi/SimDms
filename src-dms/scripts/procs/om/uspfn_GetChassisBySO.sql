go
if object_id('uspfn_GetChassisBySO') is not null
	drop procedure uspfn_GetChassisBySO

go
create procedure uspfn_GetChassisBySO
	@CompanyCode varchar(13),
	@BranchCode varchar(13),
	@SONumber varchar(25)
as
begin
	select b.ChassisCode
	     , b.ChassisNo
      from omTrSalesBPK a
     inner join omTrSalesBPKDetail b
        on a.CompanyCode = b.CompanyCode
       and a.BranchCode = b.BranchCode
       and a.BPKNo = b.BPKNo
     where a.CompanyCode = @CompanyCode
       and a.BranchCode  = @BranchCode
       and a.SONo = @SONumber
      
	  UNION 
            
     select b.ChassisCode
	      , b.ChassisNo
       from omTrSalesDO a
      inner join omTrSalesDODetail b
         on a.CompanyCode = b.CompanyCode
        and a.BranchCode = b.BranchCode
        and a.DONo = b.DONo
      where a.CompanyCode = @CompanyCode
        and a.BranchCode  = @BranchCode
        and a.SONo = @SONumber;
end