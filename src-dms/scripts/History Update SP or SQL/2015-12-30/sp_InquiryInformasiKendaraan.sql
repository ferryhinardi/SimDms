alter  procedure [dbo].[sp_InquiryInformasiKendaraan]   
(            
 @CompanyCode varchar(15),  
 @BranchCode varchar(15),  
 @FromBPKNo varchar(100),  
 @ToBPKNo varchar(100),  
 @FromBPKDate datetime,  
 @ToBPKDate datetime,  
 @CustomerCode varchar(100)  
)  
  
AS  
BEGIN  
  
declare @pQuery varchar(max)  
set @pQuery =  
'  
SELECT BPKNo, convert(varchar(15),BPKDate,103) BPKDate, a.SONo, a.CustomerCode  
 --, LockingDate as DeliveryDate  
 ,CASE WHEN convert(varchar(15),ISNULL(a.LockingDate,''19000101''),112) = ''19000101'' THEN '' '' ELSE convert(varchar(15),a.LockingDate,103) end DeliveryDate  
 ,b.ChassisNo,b.EngineNo
 ,c.CustomerName
from omTrSalesBPK a
inner join omTrSalesSOVin b on a.sono=b.sono and a.companycode=b.companycode and a.branchcode=b.branchcode
inner join gnMstCustomer c on c.customercode=a.customercode and a.companycode=b.companycode 
where 1=1  
'  
  
if len(rtrim(@FromBPKNo)) > 0  
   set @pQuery = @pQuery + ' and BPKNo between ''' + rtrim(@FromBPKNo) + '''' + ' and ' + '''' + rtrim(@ToBPKNo) + ''''  
  
if year(@FromBPKDate) <> '1900'  
   set @pQuery = @pQuery + ' and BPKDate between ''' + convert(varchar(50),@FromBPKDate) + '''' + ' and ' + '''' + convert(varchar(50),@ToBPKDate) + ''''  
  
if len(rtrim(@CustomerCode)) > 0  
   set @pQuery = @pQuery + ' and a.CustomerCode = ''' + rtrim(@CustomerCode) + ''''  
  
set @pQuery = @pQuery + ' and a.CompanyCode = ''' + rtrim(@CompanyCode) + '''' + ' and a.BranchCode = ''' + rtrim(@BranchCode) + ''''  
set @pQuery = @pQuery + ' ORDER BY BPKNo '  
  
print(@pQuery)  
exec(@pQuery)  
END  
--------------------------------------------------- BATAS ----------------------------------------------------------  