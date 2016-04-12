create procedure uspfn_SelectCustomerDO
--declare 
@CompanyCode as varchar(20),
@ChassisCode as varchar(20),
@ChassisNo as varchar(15)
		
--select @CompanyCode = '6051401', @ChassisCode = 'MHYGDN42VFJ', @ChassisNo = '400809'
	 		
as
begin
SELECT a.CustomerCode
into #t1
FROM omMstVehicle c
      left join omTrSalesSOVin b
            ON c.CompanyCode = b.CompanyCode
                  and c.ChassisCode = b.ChassisCode
                  and c.ChassisNo= b.ChassisNo
                  and c.SONo = b.SONo
      left join omTrSalesSO a
            on a.CompanyCode = b.CompanyCode
                  and a.BranchCode=b.BranchCode
                  and a.SONo=b.SONo
WHERE a.CompanyCode = @CompanyCode 
      AND a.Status = 2
      AND b.ChassisCode = @ChassisCode
      AND b.ChassisNo = @ChassisNo

if ((select COUNT(*) from #t1) = 0)
SELECT a.CustomerCode
FROM omMstVehicle c
inner join omTrSalesDO b
ON b.CompanyCode = c.CompanyCode
	and b.SONo = c.SONo
inner join omTrSalesSO a
on a.CompanyCode = c.CompanyCode
      and a.BranchCode=b.BranchCode
      and a.SONo=c.SONo
WHERE a.CompanyCode = @CompanyCode
      AND a.Status = 2
      AND c.ChassisCode = @ChassisCode
      AND c.ChassisNo = @ChassisNo
      
drop table #t1
end
