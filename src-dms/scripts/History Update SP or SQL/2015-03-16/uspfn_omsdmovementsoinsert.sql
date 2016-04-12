if object_id('uspfn_omsdmovementsoinsert') is not null
	drop procedure uspfn_omsdmovementsoinsert
GO
create procedure uspfn_omsdmovementsoinsert
@CompanyCode as varchar(15),
@BranchCode as varchar(15),
@SONo as varchar(15)
as
--exec spfn_omsdmovementsoinsert '6159401000','6159401001','SOR/15/000233'
declare @sql varchar(max);

set @sql='insert into '+dbo.GetDbMD(@CompanyCode,@BranchCode)+'..omSDMovement
   select a.CompanyCode,a.BranchCode,a.SONo,a.SODate,b.SOSeq,b.SalesModelCode,b.SalesModelYear,
   b.ChassisCode,b.ChassisNo,b.enginecode, b.EngineNo,b.ColourCode,a.WareHouseCode,a.CustomerCode,''-'',
   dbo.getcompanymd(a.CompanyCode,a.BranchCode),dbo.GetBranchMD(a.companycode,a.branchcode),
   dbo.GetWarehouseMD(a.companyCode,a.branchcode),a.Status,''0'',GETDATE(),a.ApproveBy,GETDATE(),a.ApproveBy,GETDATE()   
   from omTrSalesSO a inner join omTrSalesSOVin b on a.CompanyCode=b.CompanyCode and a.BranchCode=b.BranchCode and a.SONo=b.SONo
   where a.companycode='+@CompanyCode+' and a.branchcode='+@BranchCode+' and a.SONo='''+@SONo+'''';
--print @sql;
exec(@sql)
GO
