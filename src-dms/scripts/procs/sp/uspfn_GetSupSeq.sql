create procedure uspfn_GetSupSeq @CompanyCode varchar(15), @BranchCode varchar(15), @DocNo varchar(25)
as
SELECT MAX (SupSeq) + 1 FROM spTrnSOSupply WITH(nowait,nolock)
WHERE CompanyCode = @CompanyCode AND
BranchCode = @BranchCode AND
DocNo = @DocNo
