USE [BITPKU]
GO
/****** Object:  StoredProcedure [dbo].[uspfn_SvGetWRcvClaimHdrFile]    Script Date: 11/24/2014 14:01:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[uspfn_SvGetWRcvClaimHdrFile]
	@CompanyCode  varchar(15),
	@BranchCode   varchar(15),
	@ProductType varchar(15),
	@DataID varchar(15),
	@ReimbursementNo varchar(15),
	@ReceiveDealerCode varchar(15),
	@ReceiveDate datetime,
	@SenderDealerName varchar(50) 	
as

set nocount on
begin
	select
		RecordID, DataID, DealerCode, ReceivedDealerCode, ReceivedDealerName
		, DealerName, TotalItems = (		
			select 
				count(app.GenerateSeq)	
			from 
				SvTrnClaim cla 
				inner join SvTrnClaimJudgement jud on cla.CompanyCode = jud.CompanyCode 
					and cla.BranchCode = jud.BranchCode and cla.ProductType = jud.ProductType
					and cla.GenerateNo = jud.GenerateNo
				inner join SvtrnClaimApplication app on cla.CompanyCode = app.CompanyCode 
					and cla.BranchCode = app.BranchCode and cla.ProductType = app.ProductType
					and cla.GenerateNo = app.GenerateNo and app.GenerateSeq = jud.GenerateSeq
			where
				cla.CompanyCode = @CompanyCode
				and cla.BranchCode = @BranchCode
				and cla.ProductType = @ProductType
				and jud.SuzukiRefferenceNo = @ReimbursementNo
				and cla.SenderDealerCode = @ReceiveDealerCode
				and Convert(varchar, jud.ReceivedDate , 110) = @ReceiveDate	
		), ProductType = '4W' , ReimbursementNo
		, ReimbursementDate , BlankFiller = ''
	from (
		select TOP 1
			RecordID = 'H'
			, DataID = @DataID
			, DealerCode = a.CompanyCode
			, a.SenderDealerCode ReceivedDealerCode
			, a.SenderDealerName ReceivedDealerName
			, DealerName = @SenderDealerName
			, ProductType = a.ProductType
			, b.SuzukiRefferenceNo ReimbursementNo
			, SuzukiRefferenceDate ReimbursementDate
			, BlankFiller = ''
		from 
			SvTrnClaim a
			inner join SvTrnClaimJudgement b on a.CompanyCode = b.CompanyCode 
				and a.BranchCode = b.BranchCode and a.ProductType = b.ProductType
				and a.GenerateNo = b.GenerateNo
		where
			a.CompanyCode = @CompanyCode
			and a.BranchCode = @BranchCode
			and a.ProductType = @ProductType
			and b.SuzukiRefferenceNo = @ReimbursementNo
			and a.SenderDealerCode = @ReceiveDealerCode
			and Convert(varchar, b.ReceivedDate, 110) = @ReceiveDate	
	) as Header
end