SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<irfan>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[uspfn_SvTrnUpdateFPJHeader] 
	-- Add the parameters for the stored procedure here
	@CompanyCode as varchar(15),
	@BranchCode as varchar(15),
	@FpjNo as varchar(20),
	@InvNo as varchar(20),
	@UsrID as varchar(25)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	declare @disc decimal(18,2)
    set @disc = (select sum(discamt) from sptrnsfpjdtl where companycode=@CompanyCode and branchcode=@BranchCode and fpjno=@FpjNo )
	declare @taxPct decimal(5,2),
			@AmountDB decimal(18,0),
			@TaxVal decimal (18,0),
			@TotSalesAmount decimal(18,2),
			@AccNo varchar (35),
			@TotDiscAmt decimal(18,2)

    set @TotSalesAmount = (select TotSalesAmt from sptrnsfpjhdr where companycode=@CompanyCode and branchcode=@BranchCode and fpjno=@FpjNo)

	if exists (select isnull(c.taxpct,0) from sptrnsfpjhdr a
		inner join gnmstcustomerprofitcenter b on b.companycode=a.companycode
			and b.branchcode=a.branchcode
			and b.customercode=a.customercode
			and b.profitcentercode='300'
		inner join gnmsttax c on c.companycode=a.companycode
			and c.taxcode=b.taxcode
		where a.companycode=@CompanyCode and a.branchcode=@BranchCode and a.fpjno=@FpjNo)
		begin
			set @taxPct = (
			select isnull(c.taxpct,0) 
			from sptrnsfpjhdr a
			inner join gnmstcustomerprofitcenter b on b.companycode=a.companycode
			and b.branchcode=a.branchcode
			and b.customercode=a.customercode
            and b.profitcentercode='300'
			inner join gnmsttax c on c.companycode=a.companycode
            and c.taxcode=b.taxcode
			where a.companycode=@CompanyCode and a.branchcode=@BranchCode and a.fpjno=@FpjNo)
		end
	else
	begin
		set @taxPct = 0
	end 

	-- Insert Log
	insert into sptrnsfpjhdrlog 
		select companycode, branchcode, fpjno, invoiceno, customercodebill, totdiscamt, totdppamt, totppnamt,
		totfinalsalesamt, @UsrID, getdate()
		from sptrnsfpjhdr
	where companycode=@CompanyCode and branchcode=@BranchCode and fpjno=@FpjNo

	-- Update Value
	update sptrnsfpjhdr 
	set TotDiscAmt = @disc,
		TotDPPAmt = TotSalesAmt-@disc,
		TotPPNAmt = (TotSalesAmt-@disc)*(@taxpct/100),
		TotFinalSalesAmt = (TotSalesAmt-@disc)+((TotSalesAmt-@disc)*(@taxpct/100)),
		LastUpdateBy = @UsrID,
		LastUpdateDate = getdate()
	where companycode=@CompanyCode and branchcode=@BranchCode and fpjno=@FpjNo

	-- Insert Log
	insert into sptrnsinvoicehdrlog 
	select companycode, branchcode, invoiceno, customercodebill, totdiscamt, totdppamt, totppnamt,
		totfinalsalesamt, @UsrID, getdate()
	from sptrnsinvoicehdr
	where companycode=@CompanyCode and branchcode=@BranchCode and invoiceno=@InvNo

	-- Update Value
	update sptrnsinvoicehdr
	set TotDiscAmt = @disc,
		TotDPPAmt = TotSalesAmt-@disc,
		TotPPNAmt = (TotSalesAmt-@disc)*(@taxpct/100),
		TotFinalSalesAmt = (TotSalesAmt-@disc)+((TotSalesAmt-@disc)*(@taxpct/100)),
		LastUpdateBy = @UsrID,
		LastUpdateDate = getdate()	
	where companycode=@CompanyCode and branchcode=@BranchCode and invoiceno=@InvNo

	--Update ar & gl Interface
	set @TaxVal = isnull((select TotPPNAmt from sptrnsfpjhdr where companycode=@CompanyCode and branchcode=@BranchCode and fpjno = @FpjNo), 0)
	set @AmountDB = (@TotSalesAmount-@disc)+((@TotSalesAmount-@disc)*(@taxpct/100))

	update arInterface set NettAmt = @AmountDB
	where companycode=@CompanyCode and branchcode=@BranchCode and docno = @FpjNo
	update glInterface set AmountDb = @AmountDB
	where companycode=@CompanyCode and branchcode=@BranchCode and docno = @FpjNo and TypeTrans='AR'
	update glInterface set AmountCr = @TotSalesAmount
	where companycode=@CompanyCode and branchcode=@BranchCode and docno = @FpjNo and TypeTrans='SALES'
	update glInterface set AmountCr = @TaxVal
	where companycode=@CompanyCode and branchcode=@BranchCode and docno = @FpjNo and TypeTrans like 'TAX%'

	set @TotDiscAmt = (select totdiscamt from sptrnsfpjhdr WHERE companycode=@CompanyCode and branchcode=@BranchCode and fpjno = @FpjNo)

	if ((select count(*) from glInterface WHERE companycode=@CompanyCode and branchcode=@BranchCode and docno=@FpjNo and typetrans='DISC1') = 0 ) 
		if (@disc > 0 )
			begin
				insert into glInterface
				select TOP 1 companycode, branchcode, docno, 
				(ISNULL((select Count(*) from glInterface where companycode=@CompanyCode and branchcode=@BranchCode and docno=@FpjNo),0)+1)
				seqno, docdate, profitcentercode, accdate, (ISNULL((select discaccno from spMstAccount WHERE companycode=@CompanyCode and branchcode=@BranchCode and TypeOfGoods=(
				select typeofgoods from sptrnsfpjhdr where companycode=@CompanyCode and branchcode=@BranchCode and fpjno=@FpjNo)),0)) accountno, 
				journalcode, typejournal, applyto, @disc amountdb, 0 amountcr,
				'DISC1' typetrans, batchno, batchdate, statusflag, @UsrID createby, getdate() createdate, @UsrID lastupdateby, getdate() lastupdatedate from glInterface
				where companycode=@CompanyCode and branchcode=@BranchCode and docno=@FpjNo
			end
	else
		set @disc = (select totdiscamt from sptrnsfpjhdr WHERE companycode=@CompanyCode and branchcode=@BranchCode and fpjno = @FpjNo)
		if (@disc > 0) 	
			begin
				update glInterface set AmountDb = @disc
				where companycode=@CompanyCode and branchcode=@BranchCode and docno = @FpjNo and TypeTrans='DISC1'
			end
		else
			begin
				delete from glInterface where companycode=@CompanyCode and branchcode=@BranchCode and docno = @FpjNo and TypeTrans='DISC1'
			end	
END
GO
