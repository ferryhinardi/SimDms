USE [SAT]
GO

/****** Object:  StoredProcedure [dbo].[uspfn_SvUtlKsgClaimBatchList]    Script Date: 04/23/2015 09:37:52 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER procedure [dbo].[uspfn_SvUtlKsgClaimBatchList]
	@CompanyCode varchar(15),
	@BranchCode  varchar(15),
	@KsgClaim    varchar(15),
	@UserID      varchar(15)
as  

declare @schema as table 
(
	Name    varchar(20),
	Caption varchar(90),
	Width   int,
	Format  varchar(20)
)

insert into @schema values ('BranchCode','Branch Code',80,'')
insert into @schema values ('BatchNo','Batch No',100,'')
insert into @schema values ('BatchDate','Batch Date',120,'dd-MMM-yyy  HH:mm:ss')
insert into @schema values ('FPJNo','FPJNo',100,'')
insert into @schema values ('FPJDate','FPJ Date',120,'dd-MMM-yyy  HH:mm:ss')
insert into @schema values ('FPJGovNo','FPJ Gov. No',130,'')

select * from @schema

if @KsgClaim = 'KSG'
begin
	-- update by fhi 23-04-2015 : penambahan kondisi jika FPJDate null FPJDate='1900-01-01 00:00:00.000'
	--*************************************************************************************************
	--select top 500 BranchCode, BatchNo, BatchDate, ReceiptNo, ReceiptDate, FPJNo, FPJDate, FPJGovNo
	--  from svTrnPdiFscBatch
	select top 500 BranchCode, BatchNo, BatchDate, ReceiptNo, ReceiptDate, FPJNo,
	case when FPJDate is null then '1900-01-01 00:00:00.000' else  FPJDate end FPJDate,
	 FPJGovNo
	  from svTrnPdiFscBatch
	 where 1 = 1
	   and CompanyCode = @CompanyCode
	   and BranchCode = @BranchCode
	 order by BatchNo desc
end
else
begin
	select top 500 BranchCode, BatchNo, BatchDate, ReceiptNo, ReceiptDate, FPJNo, FPJDate, FPJGovNo
	  from svTrnClaimBatch
	 where 1 = 1
	   and CompanyCode = @CompanyCode
	   and BranchCode = @BranchCode
	 order by BatchNo desc
end


GO


