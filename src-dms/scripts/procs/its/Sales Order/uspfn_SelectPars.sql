go
if object_id('uspfn_SelectParts') is not null
	drop procedure uspfn_SelectParts

go
create procedure uspfn_SelectParts
	@CompanyCode varchar(25),
	@BranchCode varchar(25)
as

begin
	declare @CodeID varchar(25);
	set @CodeID = 'TPGO';
 
	SELECT itemInfo.PartNo
         , (item.OnHand - (item.AllocationSP + item.AllocationSR + item.AllocationSL)- (item.ReservedSP + item.ReservedSR + item.ReservedSL))  AS Available
         , itemPrice.RetailPriceInclTax
         , itemInfo.PartName
         , (
				CASE itemInfo.Status
					WHEN 1 THEN 'Aktif' ELSE 'Tidak Aktif'
				END
			)  AS Status
         , dtl.LookUpValueName as JenisPart
         , itemPrice.RetailPrice  AS NilaiPart
      FROM spMstItemInfo itemInfo                    
      LEFT JOIN spMstItems item ON item.CompanyCode = itemInfo.CompanyCode
       AND item.BranchCode = @BranchCode
       AND item.PartNo = itemInfo.PartNo
      LEFT JOIN spMstItemPrice itemPrice ON itemPrice.CompanyCode = itemInfo.CompanyCode
       AND itemPrice.BranchCode = @BranchCode 
       AND itemPrice.PartNo = item.PartNo
      LEFT JOIN GnMstLookUpDtl dtl ON item.companyCode = dtl.companyCode 
       AND dtl.CodeId = @CodeId
       AND dtl.LookUpValue = item.TypeOfGoods                    
     WHERE itemInfo.CompanyCode = @CompanyCode
       AND itemInfo.Status = '1'
       AND (
				item.OnHand - 
				(item.AllocationSP + item.AllocationSR + item.AllocationSL) - 
				(item.ReservedSP + item.ReservedSR + item.ReservedSL)
		   ) > 0 
end