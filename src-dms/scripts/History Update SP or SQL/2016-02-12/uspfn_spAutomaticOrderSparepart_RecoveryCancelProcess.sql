ALTER procedure [dbo].[uspfn_spAutomaticOrderSparepart_RecoveryCancelProcess]
@CompanyCode varchar(15),
@BranchCode varchar(15),
@POSNo varchar(15)
as

BEGIN TRANSACTION
BEGIN TRY
declare @SysDate  datetime
set @SysDate = getdate()

if isnull((select 1 from spTrnPPOSHdr
        where CompanyCode=@CompanyCode 
                      and BranchCode=@BranchCode
                      and POSNo=@POSNo
                      and OrderType='8' 
                      and Status=3 
                      and LastUpdateBy='CANCEL-AOS'
                      and Remark='MACHINE ORDER' 
                      and Transportation='AOS'),0) = 1

begin
update spMstItems 
set OnOrder = OnOrder + (select OrderQty from spTrnPPOSDtl 
where spTrnPPOSDtl.CompanyCode=@CompanyCode
and spTrnPPOSDtl.BranchCode=@BranchCode
            and spTrnPPOSDtl.POSNo=@POSNo
            and spTrnPPOSDtl.CompanyCode=spMstItems.CompanyCode
            and spTrnPPOSDtl.BranchCode=spMstItems.BranchCode
            and spTrnPPOSDtl.PartNo=spMstItems.PartNo)
where exists              (select 1 from spTrnPPOSDtl 
where spTrnPPOSDtl.CompanyCode=@CompanyCode
            and spTrnPPOSDtl.BranchCode=@BranchCode
            and spTrnPPOSDtl.POSNo=@POSNo
            and spTrnPPOSDtl.CompanyCode=spMstItems.CompanyCode
            and spTrnPPOSDtl.BranchCode=spMstItems.BranchCode
            and spTrnPPOSDtl.PartNo=spMstItems.PartNo)

insert into spTrnPOrderBalance
select d.CompanyCode, d.BranchCode, d.POSNo, h.SupplierCode, d.PartNo, 1,
	d.PartNo, h.POSDate, d.OrderQty, d.OrderQty, 0.00, 0.00, 0.00, d.DiscPct,
	d.PurchasePrice, d.CostPrice, d.ABCClass, d.MovingCode, NULL, NULL, 
	h.TypeOfGoods, d.CreatedBy, d.CreatedDate, d.LastUpdateBy, d.LastUpdateDate
from spTrnPPOSDtl d, spTrnPPOSHdr h
where d.CompanyCode=@CompanyCode
	and d.BranchCode=@BranchCode
	and d.POSNo=@POSNo
	and d.CompanyCode=h.CompanyCode
	and d.BranchCode=h.BranchCode
	and d.POSNo=h.POSNo

update spTrnPPOSHdr
set Status='2', isGenPORDD=1, LastUpdateBy='RECOVERY-AOS', LastUpdateDate=getdate()
where CompanyCode=@CompanyCode and BranchCode=@BranchCode and POSNo=@POSNo
end
END TRY

BEGIN CATCH
if @@TRANCOUNT > 0
begin
    select '0' [STATUS], 'Proses recovery gagal !!!   ' + ERROR_MESSAGE() [INFO]
    ROLLBACK TRANSACTION
    return
end
END CATCH

COMMIT TRANSACTION
