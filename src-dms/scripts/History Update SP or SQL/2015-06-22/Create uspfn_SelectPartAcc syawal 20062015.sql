Create procedure [dbo].[uspfn_SelectPartsAcc]
	@CompanyCode varchar(25),
	@BranchCode varchar(25)
as

begin
	declare @CodeID varchar(25), @sqlstr varchar(max),@DBName varchar(75), @CompanySD varchar(75), @BranchSD varchar(75) ;
	set @CodeID = 'TPGO';
    set @DBName = (select DbName from gnMstCompanyMapping where CompanyMD=@CompanyCode and BranchMD=@BranchCode)
	set @CompanySD = (select CompanyCode from gnMstCompanyMapping where CompanyMD=@CompanyCode and BranchMD=@BranchCode)
	set @BranchSD = (select BranchCode from gnMstCompanyMapping where CompanyMD=@CompanyCode and BranchMD=@BranchCode)
	set  @sqlstr = '
					SELECT itemInfo.PartNo
                    , (item.OnHand - (item.AllocationSP + item.AllocationSR + item.AllocationSL)- (item.ReservedSP + item.ReservedSR + item.ReservedSL))  AS Available
                    , itemPrice.RetailPriceInclTax
                    , itemInfo.PartName
                    , (CASE itemInfo.Status
                        WHEN 1 THEN ''Aktif'' ELSE ''Tidak Aktif''
                       END)  AS Status
                    , dtl.LookUpValueName as JenisPart
                    , itemPrice.RetailPrice  AS NilaiPart
                FROM  spMstItemInfo itemInfo                    
                LEFT JOIN spMstItems item ON item.CompanyCode = itemInfo.CompanyCode
                    AND item.BranchCode = '''+ @BranchCode +'''
                    AND item.PartNo = itemInfo.PartNo
                LEFT JOIN '+@DBName+'..spMstItemPrice itemPrice ON itemPrice.CompanyCode = '''+ @CompanySD + '''
                    AND itemPrice.BranchCode = '''+ @BranchSD +'''
                    AND itemPrice.PartNo = item.PartNo
                LEFT JOIN GnMstLookUpDtl dtl ON item.companyCode = dtl.companyCode 
                    AND dtl.CodeId = '''+@CodeID +'''
                    AND dtl.LookUpValue = item.TypeOfGoods                    
                WHERE itemInfo.CompanyCode = '''+ @CompanyCode +'''
                    AND itemInfo.Status = ''1''
                    AND (item.OnHand - (item.AllocationSP + item.AllocationSR + item.AllocationSL)
                        - (item.ReservedSP + item.ReservedSR + item.ReservedSL)) > 0 
				order by  item.TypeOfGoods desc
			'
print @sqlstr
exec(@sqlstr)
end
