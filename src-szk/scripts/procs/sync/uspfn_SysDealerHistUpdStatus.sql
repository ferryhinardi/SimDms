alter procedure uspfn_SysDealerHistUpdStatus
	@UploadID varchar(50),
	@Status   varchar(50),
	@ErrMsg   varchar(500) = '',
	@Execute  bit = 0
as

if @Execute = 1
begin
	update SysDealerHist set Status = @Status, ErrorMessage = @ErrMsg where UploadID = @UploadID
end

select * from SysDealerHist where UploadID = @UploadID

go

uspfn_SysDealerHistUpdStatus '3b4e17e41425ebe9a443c050eb5a725f', 'UPLOADED', 0
--uspfn_SysDealerHistUpdStatus '3b4e17e41425ebe9a443c050eb5a725f', 'ERROR', 1
--uspfn_SysDealerHistUpdStatus '6eddd76367e4f16d455a49a3d8b97ba6', 'ERROR'


