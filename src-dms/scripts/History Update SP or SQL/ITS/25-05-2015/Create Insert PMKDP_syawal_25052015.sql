Create procedure [dbo].[uspfn_InsertPmkdp]
	 @CompanyCode as varchar(20),
	 @BranchCode as varchar(20),
	 @InquiryNumber int,
	 @InquiryNumberDuplicate int,
	 @duplicate int=0
as

declare @intflag int, @parameter int
set @intflag = @InquiryNumber
set @parameter = @intflag + @duplicate
print 'ini adalah parameter' + convert(varchar,@parameter)
if (@duplicate=0) begin
	print 'Print 0'
	insert into pmKDP
	select * from pmKDPTemp
	 where CompanyCode = @CompanyCode
	   and BranchCode = @BranchCode
	   and InquiryNumber = @InquiryNumber
end else begin
	while (@intflag < @parameter)
	begin --begin while
		print 'flag didalam while'+ convert(varchar,@intflag)
		insert into pmKDP
		select * from pmKDPTemp
		 where CompanyCode = @CompanyCode
		   and BranchCode = @BranchCode
		   and InquiryNumber = @intflag
		
		exec uspfn_DuplicateDetailKDP @InquiryNumberDuplicate, @intflag
		set @intflag = @intflag + 1
	end --end while
end





