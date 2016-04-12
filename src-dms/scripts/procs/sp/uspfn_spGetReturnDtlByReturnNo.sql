create procedure uspfn_spGetReturnDtlByReturnNo @CompanyCode varchar(15), @BranchCode varchar(15), @ReturnNo varchar(20)
as
select row_number () OVER (ORDER BY spTrnSRturDtl.CreatedDate ASC) AS NoUrut, 
spTrnSRturDtl.PartNo, spTrnSRturDtl.PartNoOriginal, spTrnSRturDtl.DocNo, spTrnSRturDtl.QtyReturn, spTrnSFPJDtl.QtyBill
                from	spTrnSRturDtl
                left join spTrnSRturHdr on spTrnSRturHdr.CompanyCode = spTrnSRturDtl.CompanyCode and
                    spTrnSRturHdr.BranchCode = spTrnSRturDtl.BranchCode and
                    spTrnSRturHdr.ReturnNo = spTrnSRturDtl.ReturnNo
                left join spTrnSFPJDtl on spTrnSFPJDtl.CompanyCode = spTrnSRturDtl.CompanyCode and
                    spTrnSFPJDtl.BranchCode = spTrnSRturDtl.BranchCode and
                    spTrnSFPJDtl.FPJNo = spTrnSRturHdr.FPJNo and
                    spTrnSFPJDtl.DocNo = spTrnSRturDtl.DocNo and
                    spTrnSFPJDtl.PartNo = spTrnSRturDtl.PartNo and
                    spTrnSFPJDtl.PartNoOriginal = spTrnSRturDtl.PartNoOriginal
                where	spTrnSRturDtl.CompanyCode = @CompanyCode and 
                        spTrnSRturDtl.BranchCode = @BranchCode and
                        spTrnSRturDtl.ReturnNo = @ReturnNo
  
  