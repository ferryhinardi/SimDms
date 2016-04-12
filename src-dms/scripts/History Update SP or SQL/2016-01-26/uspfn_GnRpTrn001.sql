IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[uspfn_GnRpTrn001]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[uspfn_GnRpTrn001]
GO

CREATE procedure [dbo].[uspfn_GnRpTrn001]
	@CompanyCode as varchar(15)
	,@BranchCode as varchar(15)
	,@StartRtrNo as varchar(15)
	,@EndRtrNo as varchar(15)
	,@ProfitCenterCode as varchar(5)
	,@TypeOfGoods    VARCHAR(2)
as

--declare @CompanyCode as varchar(15)
--	,@BranchCode as varchar(15)
--	,@StartRtrNo as varchar(15)
--	,@EndRtrNo as varchar(15)
--
--set @CompanyCode='6058401'
--set @BranchCode='605840100'
--set @StartRtrNo='RTR/11/000001'
--set @EndRtrNo='RTR/11/000001'
declare @cek varchar(max)
set @cek = (select isFPJCentralized from gnMstCoProfile where CompanyCode=@CompanyCode and BranchCode=@BranchCode)

declare @add1 varchar(max)
declare @add2 varchar(max)

if (@cek = 1)
begin
	set @add1 = (select isnull(Address1,'')+' '+isnull(Address2,'') from gnmstcoprofile a 
					where exists (select 1 from gnmstorganizationdtl where companycode=a.companycode and branchcode=a.branchcode and isbranch=0)
					and CompanyCode=@CompanyCode )

	set @add2 = (select isnull(Address3,'')+' '+isnull(Address4,'') from gnmstcoprofile a 
					where exists (select 1 from gnmstorganizationdtl where companycode=a.companycode and branchcode=a.branchcode and isbranch=0)
					and CompanyCode=@CompanyCode )
end
else
begin
	set @add1 = (select isnull(Address1,'')+' '+isnull(Address2,'') from gnmstcoprofile a 
					where CompanyCode=@CompanyCode and BranchCode=@BranchCode )

	set @add2 = (select isnull(Address3,'')+' '+isnull(Address4,'') from gnmstcoprofile a 
					where CompanyCode=@CompanyCode and BranchCode=@BranchCode )
end

select a.CompanyCode,a.BranchCode
	,a.ReturnNo+'('+substring(a.BranchCode,8,2)+')' ReturnNo
	,a.ReturnDate,a.CustomerCode,a.FPJNo,a.FPJDate
	,a.TotReturQty,a.TotReturAmt,a.TotDPPAmt,a.TotPPNAmt,a.TotDiscAmt,a.TotFinalReturAmt
	,c.FPJGovNo,c.FPJSignature
	,b.PartNo,b.QtyReturn,b.RetailPrice,b.ReturAmt
	,d.CompanyGovName
	,@add1 CompanyAddress
	,@add2 CompanyAddress1
	,d.NPWPNo CompanyNPWP
	,e.CustomerGovName
	,isnull(e.Address1,'')+' '+isnull(e.Address2,'') CustAddress
	,isnull(e.Address3,'')+' '+isnull(e.Address4,'') CustAddress1
	,e.NPWPNo CustNPWP
	,'' DONo, '' DODate
	,'' SKPNo, '' SKPDate
	,'' SJNo, '' SJDate
	,f.PartName
	,h.TaxPct
	,i.LookUpValueName CityName
	,(select count(PartNo) from spTrnSRturDtl where CompanyCode=a.CompanyCode and BranchCode=a.BranchCode
		and ReturnNo=b.ReturnNo) MaxRow
	,8 PageBreak
from spTrnSRturHdr a
	inner join spTrnSRturDtl b on a.CompanyCode=b.CompanyCode and a.BranchCode=b.BranchCode
		and a.ReturnNo=b.ReturnNo
	inner join spTrnSFPJHdr c on a.CompanyCode=c.CompanyCode and a.BranchCode=c.BranchCode
		and a.FPJNo=c.FPJNo
	left join gnMstCoProfile d on a.CompanyCode=d.CompanyCode and a.BranchCode=d.BranchCode
	left join gnMstCustomer e on a.CompanyCode=e.CompanyCode and a.CustomerCode=e.CustomerCode
	left join spMstItemInfo f on a.CompanyCode=f.CompanyCode and b.PartNo=f.PartNo
	left join gnMstCustomerProfitcenter g on a.CompanyCode=g.CompanyCode and a.BranchCode=g.BranchCode
		and a.CustomerCode=g.CustomerCode and g.ProfitCenterCode=@ProfitCenterCode
	left join gnMstTax h on a.CompanyCode=h.CompanyCode and g.TaxCode=h.TaxCode
	left join gnMstLookUpDtl i on a.CompanyCode=i.CompanyCode and i.CodeID='CITY' and d.CityCode=i.LookUpValue
where a.CompanyCode=@CompanyCode 
	and a.BranchCode=@BranchCode
	and a.ReturnNo between @StartRtrNo and @EndRtrNo
	and a.TypeOfGoods=@TypeOfGoods

GO


