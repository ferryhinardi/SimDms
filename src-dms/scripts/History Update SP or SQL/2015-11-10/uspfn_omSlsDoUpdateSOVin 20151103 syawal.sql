if object_id('uspfn_omSlsDoUpdateSOVin') is not null
	drop procedure uspfn_omSlsDoUpdateSOVin
GO
CREATE procedure [dbo].[uspfn_omSlsDoUpdateSOVin]   
(  
 @CompanyCode varchar(15),  
 @BranchCode varchar(15),  
 @DONo varchar(15) 
)  
AS  

DECLARE 
	@QRYTemp as varchar(max),
	@CompanyMD as varchar(15),
	@DBMD as varchar(15)

SELECT @DBMD = DB_NAME(), @CompanyMD=@CompanyCode
if (isnull((select paravalue from gnMstLookUpDtl where CodeID='otom'),1)) = 1
begin 
	set @CompanyMD = (SELECT TOP 1 CompanyMD FROM gnMstCompanyMapping WHERE CompanyCode=@CompanyCode AND BranchCode=@BranchCode) 
	set @DBMD = (SELECT TOP 1 DbMD FROM gnMstCompanyMapping WHERE CompanyCode=@CompanyCode AND BranchCode=@BranchCode)
end

begin
--exec uspfn_omSlsDoUpdateSOVin 6006410,600641001,'DOS/14/000025'

set @QRYTemp =
'select a.*,b.SONo,c.ServiceBookNo,c.KeyNo from OmTrSalesDODetail a inner join OmTrSalesDO b 
	on a.companyCode = b.companyCode and a.branchCode = b.branchCode and a.DONo = b.DONo
	inner join ' + @DBMD + '.dbo.OmMstVehicle c on a.chassisCode = c.chassisCode and a.chassisNo = c.chassisNo
where a.companyCode = ''' + @CompanyCode + '''
and a.branchCode = ''' + @BranchCode + ''' and a.DONo = ''' + @DONo + ''''
				
	exec (@QRYTemp);	 
end
GO