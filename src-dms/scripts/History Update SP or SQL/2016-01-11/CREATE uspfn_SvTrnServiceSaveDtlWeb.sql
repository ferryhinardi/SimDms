CREATE procedure [dbo].[uspfn_SvTrnServiceSaveDtlWeb]
	@CompanyCode varchar(15),
	@BranchCode varchar(15),
	@ProductType varchar(15),
	@ServiceNo bigint,
	@BillType varchar(15),
    @ItemType varchar(15),
    @TaskPart varchar(20),
    @HourQty numeric(18,2),
    @PartSeq numeric(5,2),
    @UserPrice bit = 0,
    @TaskPrice numeric(18,0) = 0,
    @DiscPct numeric(5,2) = 0,
	@UserID varchar(15)
as      

declare @errmsg varchar(max)
if(not exists(select 1 from gnMstLookupDtl
		   where 1 = 1
			 and CompanyCode = @CompanyCode
			 and CodeID = 'SPK_FLAG'
			 and LookUpValue = 'LOCK_NVAL'
			 and ParaValue = '0')
   and (@TaskPrice = 0))
begin
	set @errmsg = 'mohon di check bahwa harga tidak boleh nol..'
	raiserror (@errmsg,16,1);
end

if @ItemType = 'L'
begin
	exec uspfn_SvTrnServiceSaveTask 
			 @CompanyCode=@CompanyCode,@BranchCode=@BranchCode,@ProductType=@ProductType
			,@ServiceNo=@ServiceNo,@BillType=@BillType,@DiscPct=@DiscPct
			,@OperationNo=@TaskPart,@OperationHour=@HourQty,@UserID=@UserID
			,@UserPrice=@UserPrice,@TaskPrice=@TaskPrice
end
else
begin
	exec uspfn_SvTrnServiceSaveItemWeb @CompanyCode,@BranchCode,@ProductType,@ServiceNo,@BillType,@TaskPart,@HourQty,@PartSeq,@UserID,@DiscPct=@DiscPct	
end

exec uspfn_SvTrnServiceReCalculate @CompanyCode,@BranchCode,@ProductType,@ServiceNo,0
