create procedure uspfn_GetPTSeq @CompanyCode varchar(15), @BranchCode varchar(15), @DocNo varchar(15),
@supSeq int
as
SELECT COUNT (*) 
FROM spTrnSOSupply
WHERE CompanyCode = @CompanyCode 
                AND BranchCode = @BranchCode
                AND DocNo = @DocNo
                AND SupSeq = @SupSeq