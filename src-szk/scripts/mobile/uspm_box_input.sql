alter procedure uspm_box_input
	@BoxID varchar(50),
	@CaseNo varchar(50),
	@SaveDate datetime,
	@BranchCode varchar(50) = '',
	@UserID varchar(20)
as


insert into SpBarCodeHdr (BoxID, CaseNo, SaveDate, CreatedBy, CreatedDate, BranchCode)
select @BoxID, @CaseNo, @SaveDate, @UserID, getdate(), @BranchCode


select * from SpBarCodeHdr where BoxID = @BoxID