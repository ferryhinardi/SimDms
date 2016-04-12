CREATE PROCEDURE uspfn_spGetSO4NonPLNewOrderWindow @CompanyCode varchar(15),
@BranchCode varchar(15),@CustomerCode varchar(15),
@SalesType varchar(2),@TransType varchar(3),
@TypeOfGoods varchar(2),@ProductType varchar(2)
as
SELECT DISTINCT
spTrnSOSupply.DocNo, 
DocDate, 
spTrnSORDHdr.CustomerCode,
ReferenceNo, 
ReferenceDate,
Convert(bit, 0) chkSelect
FROM spTrnSOSupply
INNER JOIN spTrnSORDHdr ON spTrnSORDHdr.DocNo = spTrnSOSupply.DocNo AND
    spTrnSORDHdr.CompanyCode = spTrnSOSupply.CompanyCode AND
    spTrnSORDHdr.BranchCode = spTrnSOSupply.BranchCode
WHERE spTrnSOSupply.SupSeq = 0
AND spTrnSORDHdr.Status = 2
AND spTrnSOSupply.DocNo NOT IN (SELECT DISTINCT DocNo FROM spTrnSPickingDtl
WHERE spTrnSPickingDtl.CompanyCode = @CompanyCode AND
          spTrnSPickingDtl.BranchCode = @BranchCode)
AND spTrnSORDHdr.CompanyCode = @CompanyCode
AND spTrnSORDHdr.BranchCode = @BranchCode 
AND spTrnSORDHdr.CustomerCode = @CustomerCode
AND spTrnSORDHdr.SalesType =  @SalesType
AND spTrnSORDHdr.TransType = @TransType
AND spTrnSORDHdr.TypeOfGoods = @TypeOfGoods
AND spTrnSOSupply.ProductType = @ProductType