USE [BITPKU]
GO
/****** Object:  StoredProcedure [dbo].[uspfn_SvGetWRcvClaimDtlFile]    Script Date: 11/24/2014 14:01:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[uspfn_SvGetWRcvClaimDtlFile]
	@CompanyCode  varchar(15),
	@BranchCode   varchar(15),
	@ProductType varchar(15),
	@Reimbursement varchar(15),
	@ReceivedDate datetime,
	@ReceivedDealerCode varchar(15) 	
as
set nocount on
begin
	select 
		a.CompanyCode, a.BranchCode, a.ProductType, a.GenerateNo, a.GenerateSeq
		,a.SuzukiRefferenceNo, a.ReceivedDate, a.DivisionCode, a.JudgementCode
		,a.PaymentOprAmt, a.PaymentOprHour, a.PaymentSubletAmt, a.PaymentSubletHour
		, ActualLaborTime = right('000'+convert(varchar(3),convert(int,a.PaymentOprHour * 10)),3)
		, SubletWorkTime = right('000'+convert(varchar(3),convert(int,a.PaymentSubletHour * 10)),3)
		,(a.PaymentOprAmt + a.PaymentSubletAmt + d.PaymentTotalPrice) TotalClaimAmt
		,d.PaymentTotalPrice PartCost
		,b.SenderDealerCode, b.LotNo 	
		,SUBSTRING(c.IssueNo, 1, PATINDEX('%-%', c.IssueNo) - 1) IssueNo
		, c.ServiceBookNo, c.ChassisCode, c.ChassisNo, c.EngineCode
		,c.EngineNo, c.OperationNo, c.TechnicalModel
	from 
		SvTrnClaimJudgement a
		inner join SvTrnClaim b on a.CompanyCode = b.CompanyCode 
			and a.BranchCode = b.BranchCode and a.ProductType = b.ProductType
			and a.GenerateNo = b.GenerateNo
		inner join SvTrnClaimApplication c on a.CompanyCode = c.CompanyCode
			and a.BranchCode = c.BranchCode and a.ProductType = c.ProductType 
			and a.GenerateNo = c.GenerateNo and a.GenerateSeq = c.GenerateSeq		
		inner join (	
			Select 
				a.CompanyCode, a.BranchCode, a.ProductType, a.GenerateNo, a.GenerateSeq
				,sum(b.PaymentTotalPrice) PaymentTotalPrice
			from 
			SvTrnClaimJudgement a
				inner join SvTrnClaimPart b on a.CompanyCode = b.CompanyCode 
					and a.BranchCode = b.BranchCode and a.ProductType = b.ProductType
					and a.GenerateNo = b.GenerateNo and a.GenerateSeq = b.GenerateSeq
				inner join SvTrnClaim c on a.CompanyCode = c.CompanyCode 
					and a.BranchCode = b.BranchCode and a.ProductType = c.ProductType
					and a.GenerateNo = c.GenerateNo
			where 
				a.CompanyCode = @CompanyCode
				and a.BranchCode = @BranchCode
				and a.ProductType = @ProductType
				and a.SuzukiRefferenceNo = @Reimbursement
				and Convert(varchar, a.ReceivedDate, 110) = @ReceivedDate
				and c.SenderDealerCode = @ReceivedDealerCode			
			group by a.CompanyCode, a.BranchCode, a.ProductType, a.GenerateNo, a.GenerateSeq
		) as d on a.CompanyCode = d.CompanyCode and a.BranchCode = d.BranchCode 
			and a.ProductType = d.ProductType and a.GenerateNo = d.GenerateNo 
			and a.GenerateSeq = d.GenerateSeq
	where 
		a.CompanyCode = @CompanyCode
		and a.BranchCode = @BranchCode
		and a.ProductType = @ProductType
		and a.SuzukiRefferenceNo = @Reimbursement
		and Convert(varchar, a.ReceivedDate, 110) = @ReceivedDate
		and b.SenderDealerCode = @ReceivedDealerCode
end