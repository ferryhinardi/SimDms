alter procedure uspfn_GnDailyPosting
	@CompanyCode varchar(20),
	@UserId      varchar(50)
as

set nocount on

declare @DocDate    date 
declare @BranchCode varchar(20)
declare @SqlQry	    varchar(max)

begin transaction
	begin try
		set @DocDate = isnull((select top 1 dateadd(d, 1, PostingDate) from GnDailyPostingHist where CompanyCode = @CompanyCode order by PostingDate desc), (select top 1 DocDate from SvSdMovement order by DocDate))
		set @BranchCode = isnull((select top 1 BranchCode from GnMstCompanyMapping order by BranchCode), '')

		while @BranchCode != ''
		begin
			exec uspfn_GnDailyPostingBranch @CompanyCode = @CompanyCode, @BranchCode = @BranchCode, @DocDate = @DocDate
			set @BranchCode = isnull((select top 1 BranchCode from GnMstCompanyMapping where BranchCode > @BranchCode order by BranchCode), '')
		end

		insert into GnDailyPostingHist (CompanyCode, PostingDate, PostingSeq, PostingLog, CreatedBy, CreatedDate)
		select @CompanyCode as CompanyCode
			 , @DocDate as PostingDate
			 , isnull((
				 select top 1 PostingSeq
				   from GnDailyPostingHist
				  where CompanyCode = @CompanyCode
					and convert(date, PostingDate) = @DocDate
				  order by PostingSeq desc), 0) + 1 as PostingSeq
			 , convert(varchar, count(*)) + ' data terproses' as PostingLog
			 , @UserId as CreatedBy
			 , getdate() as CreatedDate
		  from SvSDMovement where CompanyCode = @CompanyCode and convert(date, DocDate) = @DocDate

		select * from GnDailyPostingHist where convert(date, PostingDate) = @DocDate
	commit transaction
end try
begin catch
  select error_number() as ErrorNumber, error_message() as ErrorMessage;
  rollback transaction
end catch 

--go

--exec uspfn_GnDailyPosting '6006400001', 'demo'
