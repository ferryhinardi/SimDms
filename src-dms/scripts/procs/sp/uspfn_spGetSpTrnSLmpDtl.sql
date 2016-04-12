CREATE procedure uspfn_spGetSpTrnSLmpDtl  
@CompanyCode varchar(15), @BranchCode varchar(15), @LMPNo varchar(15)  
as  
select PartNo, PartNoOriginal, DocNo, DocDate, ReferenceNo, QtyBill from spTrnSLmpDtl  
where CompanyCode = @CompanyCode and BranchCode = @BranchCode  
AND LmpNo = @LMPNo