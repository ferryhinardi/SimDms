CREATE procedure sp_MstOthersInventoryBrowse @CompanyCode varchar(10), @BranchCode varchar(10)
as

SELECT a.OthersNonInventory, a.OthersNonInventoryDesc, a.OthersNonInventoryAccNo, b.Description, a.Remark, a.IsActive
FROM omMstOthersNonInventory a
INNER JOIN GnMstAccount b
ON a.OthersNonInventoryAccNo=b.AccountNo
WHERE a.CompanyCode=@CompanyCode AND a.BranchCode=@BranchCode