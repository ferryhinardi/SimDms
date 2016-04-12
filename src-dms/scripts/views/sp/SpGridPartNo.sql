

create view [dbo].[SpGridPartNo]  as 
   
SELECT CompanyCode, BranchCode, ClaimNo, PartNo, PartNoWrong, ClaimType, DocNo, DocDate, SeqNo,OvertageQty, ReceivedQty, 
ShortageQty, DemageQty, WrongQty, TotClaimQty,PurchasePrice,CostPrice, ReasonCode, 
CASE ReasonCode WHEN 'CORR' THEN 'KOREKSI STOK' ELSE CASE ReasonCode WHEN 'DMG' THEN 'RUSAK STOK' 
ELSE 'DIHANCURKAN' END END as ReasonCodeStr, Status, ABCClass, MovingCode, ProductType, PartCategory
from spTrnPClaimDtl


GO


