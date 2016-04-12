CREATE PROCEDURE uspfn_spGetSpTrnSBPSFDtl   
@CompanyCode varchar(15), @BranchCode varchar(15), @BPSFNo varchar(15)  
as  
select PartNo, PartNoOriginal, DocNo, DocDate, ReferenceNo, QtyBill from spTrnSBPSFDtl  
where CompanyCode = @CompanyCode and BranchCode = @BranchCode and BPSFNo = @BPSFNo  