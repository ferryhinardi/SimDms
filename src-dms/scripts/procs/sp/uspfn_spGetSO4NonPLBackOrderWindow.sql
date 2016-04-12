CREATE PROCEDURE uspfn_spGetSO4NonPLBackOrderWindow @CompanyCode varchar(15),
@BranchCode varchar(15),@CustomerCode varchar(15),
@SalesType varchar(2),@TransType varchar(3),
@TypeOfGoods varchar(2),@ProductType varchar(2)
as
SELECT DISTINCT
    spTrnSORDHdr.DocNo, 
    DocDate, 
    CustomerCode,
    OrderNo AS ReferenceNo, 
    ReferenceDate, 
    Convert(bit, 0) chkSelect  
FROM spTrnSORDHdr
INNER JOIN spTrnSORDDtl ON spTrnSORDDtl.DocNo = spTrnSORDHdr.DocNo AND
    spTrnSORDDtl.CompanyCode = spTrnSORDHdr.CompanyCode AND
    spTrnSORDDtl.BranchCode = spTrnSORDHdr.BranchCode
WHERE  
    (spTrnSORDDtl.QtyBO - spTrnSORDDtl.QtyBOSupply - spTrnSORDDtl.QtyBOCancel > 0)
    AND spTrnSORDHdr.CompanyCode = @CompanyCode
    AND spTrnSORDHdr.BranchCode = @BranchCode 
    AND spTrnSORDHdr.CustomerCode = @CustomerCode
    AND spTrnSORDHdr.SalesType = @SalesType
    AND spTrnSORDHdr.TransType = @TransType
    AND spTrnSORDHdr.TypeOfGoods = @TypeOfGoods
    AND spTrnSORDDtl.ProductType = @ProductType
    AND spTrnSORDHdr.DocNo not in 
        (SELECT spTrnSOSupply.DocNo from spTrnSOSupply 
            WHERE 
        spTrnSOSupply.CompanyCode = spTrnSORDHdr.CompanyCode
        AND spTrnSOSupply.BranchCode = spTrnSORDHdr.BranchCode
        AND spTrnSOSupply.DocNo = spTrnSORDHdr.DocNo
        AND spTrnSOSupply.Status = 0)