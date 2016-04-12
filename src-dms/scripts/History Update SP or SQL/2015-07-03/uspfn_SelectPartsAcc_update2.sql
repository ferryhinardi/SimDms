ALTER procedure [dbo].[uspfn_SelectPartsAcc]
	@CompanyCode varchar(25),
	@BranchCode varchar(25),
	@SONo varchar(25)
as

begin
	declare @CodeID varchar(25), @sqlstr varchar(max),@DBMD varchar(75), @CompanyMD varchar(75), @BranchMD varchar(75) ;
	set @CodeID = 'TPGO';
    set @DBMD = (select TOP 1 DbMD from gnMstCompanyMapping where CompanyCode=@CompanyCode and BranchCode=@BranchCode)
    set @CompanyMD = (select TOP 1 CompanyMD from gnMstCompanyMapping where CompanyCode=@CompanyCode and BranchCode=@BranchCode)
	set @BranchMD = (select TOP 1 BranchMD from gnMstCompanyMapping where CompanyCode=@CompanyCode and BranchCode=@BranchCode)
	--set @CompanySD = (select CompanyCode from gnMstCompanyMapping where CompanyMD=@CompanyCode and BranchMD=@BranchCode)
	--set @BranchSD = (select BranchCode from gnMstCompanyMapping where CompanyMD=@CompanyCode and BranchMD=@BranchCode)
	
	if @DBMD IS NULL
	begin
		SELECT itemInfo.PartNo
		, (item.OnHand - (item.AllocationSP + item.AllocationSR + item.AllocationSL)- (item.ReservedSP + item.ReservedSR + item.ReservedSL))  AS Available
		, itemPrice.RetailPriceInclTax
		, itemInfo.PartName
		, (CASE itemInfo.Status
			WHEN 1 THEN 'Aktif' ELSE 'Tidak Aktif'
			END)  AS Status
		, dtl.LookUpValueName as JenisPart
		, itemPrice.RetailPrice  AS NilaiPart
		FROM spMstItemInfo itemInfo                    
		INNER JOIN spMstItems item ON item.CompanyCode = itemInfo.CompanyCode
			AND item.BranchCode = @BranchCode
			AND item.PartNo = itemInfo.PartNo
		INNER JOIN spMstItemPrice itemPrice ON itemPrice.CompanyCode = @CompanyCode
			AND itemPrice.BranchCode = @BranchCode
			AND itemPrice.PartNo = item.PartNo
		LEFT JOIN GnMstLookUpDtl dtl ON item.companyCode = dtl.companyCode 
			AND dtl.CodeId = @CodeID
			AND dtl.LookUpValue = item.TypeOfGoods                    
		WHERE itemInfo.CompanyCode = @CompanyCode
			AND itemInfo.Status = '1'
			AND (item.TypeOfGoods = '2' OR item.TypeOfGoods = '5')
			AND (item.OnHand - (item.AllocationSP + item.AllocationSR + item.AllocationSL)
				- (item.ReservedSP + item.ReservedSR + item.ReservedSL)) > 0 
			AND itemInfo.PartNo NOT IN (SELECT x.PartNo FROM OmTrSalesSOAccsSeq x WHERE x.PartNo=itemInfo.PartNo AND x.SONo=@SONo
					AND x.CompanyCode=@CompanyCode AND BranchCode=@BranchCode)
		order by  item.TypeOfGoods desc
	end
	else
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
			FROM ' + @DBMD + '..spMstItemInfo itemInfo                    
			INNER JOIN ' + @DBMD + '..spMstItems item ON item.CompanyCode = itemInfo.CompanyCode
				AND item.BranchCode = '''+ @BranchMD +'''
				AND item.PartNo = itemInfo.PartNo
			INNER JOIN ' + @DBMD + '..spMstItemPrice itemPrice ON itemPrice.CompanyCode = '''+ @CompanyMD + '''
				AND itemPrice.BranchCode = '''+ @BranchMD +'''
				AND itemPrice.PartNo = item.PartNo
			LEFT JOIN ' + @DBMD + '..GnMstLookUpDtl dtl ON item.companyCode = dtl.companyCode 
				AND dtl.CodeId = '''+ @CodeID +'''
				AND dtl.LookUpValue = item.TypeOfGoods                    
			WHERE itemInfo.CompanyCode = '''+ @CompanyMD +'''
				AND itemInfo.Status = ''1''
				AND (item.TypeOfGoods = ''2'' OR item.TypeOfGoods = ''5'')
				AND (item.OnHand - (item.AllocationSP + item.AllocationSR + item.AllocationSL)
					- (item.ReservedSP + item.ReservedSR + item.ReservedSL)) > 0
				AND itemInfo.PartNo NOT IN (SELECT x.PartNo FROM OmTrSalesSOAccsSeq x WHERE x.PartNo=itemInfo.PartNo AND x.SONo=''' + @SONo + '''
					AND x.CompanyCode = ''' + @CompanyCode + ''' AND BranchCode = ''' + @BranchCode + ''')  
			order by  item.TypeOfGoods desc
		'
	--print @sqlstr
	exec(@sqlstr)
end
