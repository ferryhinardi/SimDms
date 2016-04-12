ALTER procedure [dbo].[uspfn_SvGetFirstBillType]
	@CompanyCode varchar(20),
	@BranchCode  varchar(20),
	@ProductType varchar(20),
	@ServiceNo   bigint
as  

--declare	@CompanyCode varchar(15)
--declare	@BranchCode  varchar(15)
--declare	@ProductType varchar(15)
--declare	@ServiceNo   bigint
--
--set	@CompanyCode = '6092401'
--set	@BranchCode  = '609240102'
--set	@ProductType = '4W'
--set @ServiceNo   = '416'

begin

if (exists (select * from svMstBillingType where isnull(LockingBy, '') = ''))
begin
	-- update
	update svMstBillingType set LockingBy = 1 where BillType = 'C'
	update svMstBillingType set LockingBy = 2 where BillType = 'I'
	update svMstBillingType set LockingBy = 3 where BillType = 'F'
	update svMstBillingType set LockingBy = 4 where BillType = 'W'
	update svMstBillingType set LockingBy = 5 where BillType = 'A'
	
	--update svMstBillingType set LockingBy = 1 where BillType = 'C'
	--update svMstBillingType set LockingBy = 2 where BillType = 'F'
	--update svMstBillingType set LockingBy = 3 where BillType = 'W'
	--update svMstBillingType set LockingBy = 4 where BillType = 'A'
	--update svMstBillingType set LockingBy = 5 where BillType = 'I'
end

select BillType into #t1 from (
select BillType from svTrnSrvItem
 where 1 = 1
   and CompanyCode = @CompanyCode
   and BranchCode  = @BranchCode
   and ProductType = @ProductType
   and ServiceNo   = @ServiceNo
union
select BillType from svTrnSrvTask
 where 1 = 1
   and CompanyCode = @CompanyCode
   and BranchCode  = @BranchCode
   and ProductType = @ProductType
   and ServiceNo   = @ServiceNo
)#


select top 1 a.BillType from svMstBillingType a, #t1 b
 where b.BillType = a.BillType
 order by a.LockingBy

drop table #t1

end