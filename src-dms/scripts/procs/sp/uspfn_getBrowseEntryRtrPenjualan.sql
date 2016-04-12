create procedure uspfn_getBrowseEntryRtrPenjualan @CompanyCode varchar(15), @BranchCode varchar(15), @TypeOfGoods varchar(5)
as
select	 a.CompanyCode, a.BranchCode,   a.ReturnNo, a.ReturnDate, a.FPJNo, b.FPJDate, a.ReferenceNo, a.ReferenceDate, a.CustomerCode
                from	    spTrnSRturHdr a
                left join  spTrnSFPJHdr b on
	                a.CompanyCode = b.CompanyCode
	                and a.BranchCode = b.BranchCode
	                and a.FPJNo = b.FPJNo
                where	    a.CompanyCode = @CompanyCode
                            and a.BranchCode = @BranchCode	
                            and a.FPJNo = b.FPJNo
                            and a.TypeOfGoods = @TypeOfGoods
                            and a.Status in ('0', '1')
                order by    a.ReturnNo desc