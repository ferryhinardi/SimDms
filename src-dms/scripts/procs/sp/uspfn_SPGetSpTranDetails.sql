Create procedure uspfn_SPGetSpTranDetails  
@CompanyCode varchar(25),  
@BranchCode varchar(25),  
@InvoiceNo varchar(25)  
as   
select PartNo, PartNoOriginal, DocNo, DocDate, ReferenceNo, QtyBill from spTrnSInvoiceDtl  
where InvoiceNo = @InvoiceNo  
and CompanyCode = @CompanyCode  
and BranchCode = @BranchCode  