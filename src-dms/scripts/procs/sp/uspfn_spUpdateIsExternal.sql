create procedure uspfn_spUpdateIsExternal @CompanyCode varchar(15), @BranchCode varchar(15), @PickingSlipNo varchar(20),
@UserId varchar(25)
as
if exists   (
    select isLocked from spTrnSORDHdr
    where companycode=@CompanyCode 
        and branchcode=@BranchCode 
        and docno in (select distinct docno from spTrnSPickingDtl where companycode=@CompanyCode and branchcode=@BranchCode 
            and pickingslipno=@PickingSlipNo)
        and isLocked = '1'
            )
begin
    update spTrnSPickingHdr
    set isLocked = '1', LockingBy= @UserId, LockingDate=getdate()
    where companycode=@CompanyCode and branchcode=@BranchCode and pickingslipno=@PickingSlipNo
end 