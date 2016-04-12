ALTER procedure [dbo].[uspfn_omSlsBPKLkpChassisNo]   
(  
 @CompanyCode varchar(15),  
 @BranchCode varchar(15),  
 @DONo varchar(15),
 @BPKNo varchar(15),
 @ChassisCode   varchar(15) 
)  
AS  
begin
select a.ChassisNo, a.EngineCode, a.EngineNo, 
a.ColourCode, b.RefferenceDesc1 from 
omTrSalesDODetail a
left join omMstRefference b on 
b.CompanyCode = a.CompanyCode and
b.RefferenceCode = a.ColourCode and
b.RefferenceType = 'COLO'
where
a.CompanyCode = @CompanyCode
AND a.BranchCode = @BranchCode
AND a.ChassisCode = @ChassisCode
AND a.DONo = @DONo
AND a.ChassisNo not in (select isnull(ChassisNo,0) from omTrSalesBPKDetail z
where z.CompanyCode = a.CompanyCode
and z.BranchCode = a.BranchCode
and z.BPKNo = @BPKNo
and z.ChassisCode = a.ChassisCode
AND not exists (select 1 from omTrSalesReturnVIN where CompanyCode=a.CompanyCode and BranchCode=a.BranchCode
		and ChassisCode=a.ChassisCode and ChassisNo=a.ChassisNo))
END		

