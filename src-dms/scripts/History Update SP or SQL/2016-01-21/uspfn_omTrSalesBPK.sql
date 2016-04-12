ALTER Procedure [dbo].[uspfn_omTrSalesBPK]
(
 @CompanyCode varchar(15),
 @BranchCode varchar(15),
 @BPKNo varchar(50),
 @DeliveryDate varchar(50),
 @UserId varchar(50)
)
as
BEGIN

IF exists (SELECT * FROM omTrSalesBPK 
			where CompanyCode = @CompanyCode and BranchCode = @BranchCode 
--			and BPKNo = @BPKNo AND CONVERT(nvarchar(15),LockingDate,112) = '19000101' )
			and BPKNo = @BPKNo AND CONVERT(nvarchar(15),BPKDate,112) <= CONVERT(Datetime, @DeliveryDate, 101) )
	Update omTrSalesBPK
	SET LockingDate = CONVERT(Datetime, @DeliveryDate, 101),
		LockingBy = @UserId
	WHERE CompanyCode = @CompanyCode 
	and BranchCode = @BranchCode 
	and BPKNo = @BPKNo
ELSE
	Update omTrSalesBPK
	SET LockingDate = LockingDate,
		LockingBy = LockingBy
	WHERE CompanyCode = @CompanyCode 
	and BranchCode = @BranchCode 
	and BPKNo = @BPKNo
END

--exec uspfn_omTrSalesBPK '6006400001','6006400101','SJ101/14/100009',null,'ga'

