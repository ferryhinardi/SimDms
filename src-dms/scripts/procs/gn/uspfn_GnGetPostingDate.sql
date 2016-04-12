alter procedure uspfn_GnGetPostingDate
	@CompanyCode varchar(20)

as

select isnull((select top 1 dateadd(d, 1, PostingDate) from GnDailyPostingHist where CompanyCode = @CompanyCode order by PostingDate desc), (select top 1 DocDate from SvSdMovement order by DocDate)) as PostingDate

go

uspfn_GnGetPostingDate '6006400001' 