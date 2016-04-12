if object_id('usprpt_GnGenerateSeqTaxOnline') is not null
	drop PROCEDURE usprpt_GnGenerateSeqTaxOnline
GO
create procedure [dbo].[usprpt_GnGenerateSeqTaxOnline]
--DECLARE 
	@CompanyCode as varchar(15)
	,@BranchCode as varchar(15)
	,@StartDate as varchar(8)
	,@FPJDate as varchar(8)
	,@ProfitCenterCode as varchar(3)
	,@UserId as varchar(15)
	,@DocNo as varchar(5000)
	,@LastSeqNo as decimal
	,@TaxCabCode as varchar(3)
AS
BEGIN

declare @t_tax table(
	CompanyCode varchar(15)
	,BranchCode varchar(15)
	,ProfitCenterCode varchar(3)
	,DocNo varchar(15)
	,DocDate varchar(15)
	,DueDate datetime
	,RefNo varchar(15)
	,RefDate datetime
	,TaxTransCode varchar(2)
	,CustomerCodeBill varchar(15)
)

if @ProfitCenterCode='' or @ProfitCenterCode='300'
begin
	insert into @t_tax
	SELECT	CompanyCode, BranchCode, '300' AS ProfitCenterCode, InvoiceNo AS DocNo, convert(varchar,InvoiceDate,112) AS DocDate, convert(varchar,DueDate,112) DueDate
			, '' AS RefNo, NULL AS RefDate
			,(
				SELECT	ISNULL(TaxTransCode, '') 
				FROM	GnMstCustomerProfitCenter 
				WHERE	CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode 
						AND CustomerCode = a.CustomerCodeBill AND ProfitCenterCode = '300'
			) AS TaxTransCode
			, CustomerCodeBill
	FROM	SpTrnSFPJHdr a
	WHERE	CompanyCode = @CompanyCode  
			--AND BranchCode like @BranchCode
			AND isPKP = 1 AND ISNULL(FPJGovNo, '') = ''
			AND CONVERT(VARCHAR, InvoiceDate, 112) BETWEEN @StartDate AND @FPJDate
			AND ((case when @ProfitCenterCode='' then BranchCode+' '+InvoiceNo end) = BranchCode+' '+InvoiceNo
				or (case when @ProfitCenterCode<>'' then @DocNo end) like '%|'+BranchCode+' '+InvoiceNo+'|%')
	GROUP BY CompanyCode, BranchCode,InvoiceNo, CustomerCodeBill,convert(varchar,InvoiceDate,112),convert(varchar,DueDate,112)
end

if @ProfitCenterCode='' or @ProfitCenterCode='200'
begin
	insert into @t_tax
	SELECT	CompanyCode, BranchCode, '200' AS ProfitCenterCode, FPJNo AS DocNo, convert(varchar,FPJDate,112) AS DocDate, DueDate
			, '' AS RefNo, NULL AS RefDate
			,(
				SELECT	ISNULL(TaxTransCode, '') 
				FROM	GnMstCustomerProfitCenter 
				WHERE	CompanyCode = SVTrnFakturPajak.CompanyCode AND BranchCode = SVTrnFakturPajak.BranchCode 
						AND CustomerCode = SVTrnFakturPajak.CustomerCodeBill AND ProfitCenterCode = '200'
			) AS TaxTransCode
			, CustomerCodeBill
	FROM	SVTrnFakturPajak
	WHERE	CompanyCode = @CompanyCode 
			--AND BranchCode like @BranchCode
			AND isPKP = 1 AND ISNULL(FPJGovNo, '') = ''
			AND IsLocked= 0
			AND CONVERT(VARCHAR, FPJDate, 112) BETWEEN @StartDate AND @FPJDate
			AND ((case when @ProfitCenterCode='' then BranchCode+' '+FPJNo end)=BranchCode+' '+FPJNo
				or (case when @ProfitCenterCode<>'' then @DocNo end) like '%|' + BranchCode+' '+FPJNo + '|%'
			)
end

if @ProfitCenterCode='' or @ProfitCenterCode='100'
begin
	insert into @t_tax
	SELECT	CompanyCode, BranchCode, '100' AS ProfitCenterCode, InvoiceNo AS DocNo, convert(varchar,InvoiceDate,112) AS DocDate, DueDate
			, '' AS RefNo, NULL AS RefDate
			,(
				SELECT	ISNULL(TaxTransCode, '') 
				FROM	GnMstCustomerProfitCenter 
				WHERE	CompanyCode = OmFakturPajakHdr.CompanyCode AND BranchCode = OmFakturPajakHdr.BranchCode 
						AND CustomerCode = OmFakturPajakHdr.CustomerCode AND ProfitCenterCode = '100') AS TaxTransCode
			, CustomerCode
	FROM	OmFakturPajakHdr
	WHERE	CompanyCode = @CompanyCode 
			--AND BranchCode like @BranchCode
			AND TaxType = 'Standard' AND ISNULL(FakturPajakNo, '') = ''
			AND CONVERT(VARCHAR, InvoiceDate, 112) BETWEEN @StartDate AND @FPJDate
			AND ((case when @ProfitCenterCode='' then BranchCode+' '+InvoiceNo end)=BranchCode+' '+InvoiceNo
				or (case when @ProfitCenterCode<>'' then @DocNo end) like '%|' + BranchCode+' '+InvoiceNo + '|%'
			)
end

if @ProfitCenterCode='' or @ProfitCenterCode='000'
begin
	insert into @t_tax
	SELECT	CompanyCode, BranchCode, '000' AS ProfitCenterCode, InvoiceNo AS DocNo, convert(varchar,InvoiceDate,112) AS DocDate, DueDate
			, FPJNo AS RefNo, FPJDate AS RefDate
			,(
				SELECT	ISNULL(TaxTransCode, '') 
				FROM	GnMstCustomerProfitCenter 
				WHERE	CompanyCode = ARFakturPajakHdr.CompanyCode AND BranchCode = ARFakturPajakHdr.BranchCode 
						AND CustomerCode = ARFakturPajakHdr.CustomerCode AND ProfitCenterCode = '000'
			) AS TaxTransCode
			, CustomerCode
	FROM	ARFakturPajakHdr
	WHERE	CompanyCode = @CompanyCode 
			--AND BranchCode like @BranchCode
			AND TaxType = 'Standard' AND ISNULL(FakturPajakNo, '') = ''
			AND CONVERT(VARCHAR, InvoiceDate, 112) BETWEEN @StartDate AND @FPJDate
			AND ((case when @ProfitCenterCode='' then BranchCode+' '+InvoiceNo end)=BranchCode+' '+InvoiceNo
				or (case when @ProfitCenterCode<>'' then @DocNo end) like '%|' + BranchCode+' '+InvoiceNo + '|%'
			)
end

select * into #f1
from (
	select row_number() over(order by a.BranchCode,a.DocDate,b.LookupValue,a.CustomerCodeBill,a.ProfitCenterCode asc) OrderNo,a.*,isnull(b.LookupValue,'')LookupValue
	from @t_tax a
	left join gnMstLookupDtl b on b.CompanyCode = a.CompanyCode
		and b.CodeID = 'FPJG'
		and b.LookupValue = a.CustomerCodeBill	
) #f1  order by LookupValue desc

if(Convert(varchar,@FPJDate,112) < '20130401')
begin
	-- create FPJGovNo
	select * into #f3 
	from (
		select 
			a.OrderNo,a.CompanyCode,a.BranchCode,year(DocDate) PeriodTaxYear
			,month(DocDate) PeriodTaxMonth,ProfitCenterCode
			,left(TaxTransCode+'000',3)+'.'+
			right('000'+@TaxCabCode,3)+'-'+
			right( isnull(convert(varchar(4),year(DocDate)),year(getDate())),2 )+'.'+ 
			right( '00000000'+convert(varchar(8),@LastSeqNo+a.OrderNo),8 ) FPJGovNo
			,DocDate as FPJGovDate,DocNo,convert(datetime,DocDate) DocDate,RefNo,RefDate,@UserId CreatedBy,getDate() CreatedDate
		from 
			#f1 a
	) #f3


	-- insert to tabel GenerateTax
	insert into
		GnGenerateTax(
			CompanyCode, BranchCode, PeriodTaxYear, PeriodTaxMonth, ProfitCenterCode, FPJGovNo, 
			FPJGovDate, DocNo, DocDate , RefNo, RefDate, CreatedBy, CreatedDate
	) 
	select 
		CompanyCode, BranchCode, PeriodTaxYear, PeriodTaxMonth, ProfitCenterCode, FPJGovNo, 
		FPJGovDate, DocNo, DocDate , RefNo, RefDate, CreatedBy, CreatedDate
	from 
		#f3

	drop table #f1

	-- update Sparepart
	update	SPTrnSFPJHdr
	set		FPJGovNo= a.FPJGovNo
			,FPJSignature= a.FPJGovDate
	from	#f3 a, SPTrnSFPJHdr b
	where	a.CompanyCode= b.CompanyCode and a.BranchCode= b.BranchCode and a.DocNo= b.InvoiceNo

	-- update Service
	update	SVTrnFakturPajak
	set		FPJGovNo= a.FPJGovNo
			,SignedDate= a.FPJGovDate
	from	#f3 a, SVTrnFakturPajak b
	where	a.CompanyCode= b.CompanyCode and a.BranchCode= b.BranchCode and a.DocNo= b.FPJNo

	-- update Sales
	update	OmFakturPajakHdr
	set		FakturPajakNo= a.FPJGovNo
			,FakturPajakDate= a.FPJGovDate 
	from	#f3 a, OmFakturPajakHdr b
	where	a.CompanyCode= b.CompanyCode and a.BranchCode= b.BranchCode and a.DocNo= b.InvoiceNo

	-- update Finance
	update	ArFakturPajakHdr
	set		FakturPajakNo= a.FPJGovNo
			,FakturPajakDate= a.FPJGovDate
	from	#f3 a, ArFakturPajakHdr b
	where	a.CompanyCode= b.CompanyCode and a.BranchCode= b.BranchCode and a.DocNo= b.InvoiceNo

	select top 1 convert(decimal,right(FPJGovNo,8)) from #f3 order by right(FPJGovNo,8) desc
	drop table #f3
end
else
begin
---------------------------------------------------------------------------------------------------
 --									Region Setelah Tanggal 1 April 2013                           --
 ---------------------------------------------------------------------------------------------------
	Declare @TotalFPJ				varchar(25)
	Declare @TotalFPJX				varchar(25)
	Declare @EndFPJ					varchar(25)
	Declare @CurrentFPJ				varchar(25)
	Declare @CurrentDocNo			varchar(100)
	Declare @CurrentCompanyCode		varchar(15)
	Declare @CurrentBranchCode		varchar(15)
	Declare @CurrentTaxTransCode	varchar(2)
	Declare @OrderNo				varchar(10)
	Declare @CurrentCustCodeBill	varchar(15)
	
	declare @IntOrderNo as int
	set @IntOrderNo = 0
	
	DECLARE temp CURSOR FOR
	SELECT	CompanyCode, BranchCode, DocNo, TaxTransCode, OrderNo, CustomerCodeBill
	FROM	#f1

	OPEN temp
	FETCH NEXT FROM temp INTO @CurrentCompanyCode,@CurrentBranchCode,@CurrentDocNo,@CurrentTaxTransCode,@OrderNo,@CurrentCustCodeBill
	WHILE @@FETCH_STATUS = 0
		BEGIN
		--!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		--									Penambahan no Faktur Pajak
		--!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
									--set @TotalFPJ =   @LastSeqNo + @OrderNo
		--!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
			
			if @CurrentCustCodeBill = (select top 1 LookupValue from gnMstLookupDtl where CompanyCode = @CurrentCompanyCode and CodeID = 'FPJG' and LookupValue = @CurrentCustCodeBill)
			begin
				print @CurrentFPJ + ' for ' + @CurrentCustCodeBill
				set @TotalFPJ =   @LastSeqNo + convert(varchar, @IntOrderNo,10)
			end
			else
			begin
				set @IntOrderNo = @IntOrderNo + 1
				set @TotalFPJ =   @LastSeqNo + convert(varchar, @IntOrderNo,10)
			end									   
            --update by Hasim (6 Jan 2014), because could be problem if length of tax no less than 11...
            --original...
			--set @CurrentFPJ = (select LEFT (convert(varchar,@CurrentTaxTransCode) + '000',3)+'.'+
			--							LEFT (convert(varchar,@TotalFPJ),3) + '-' +
			--							RIGHT(convert(varchar,YEAR(getdate())),2) + '.' +
			--							RIGHT(convert(bigint,@TotalFPJ),8))
            --revised...
            set @TotalFPJX  = RIGHT('00000000000'+convert(varchar(11),@TotalFPJ),11)
			set @CurrentFPJ = (select LEFT (convert(varchar,@CurrentTaxTransCode) + '000',3)+'.'+
									  LEFT (convert(varchar,@TotalFPJX),3) + '-' +
									  RIGHT(convert(varchar,YEAR(getdate())),2) + '.' +
									  RIGHT(convert(varchar,@TotalFPJX),8))

			--insert to tabel GenerateTax
			insert into
				GnGenerateTax(
					CompanyCode, BranchCode, PeriodTaxYear, PeriodTaxMonth, ProfitCenterCode, FPJGovNo, 
					FPJGovDate, DocNo, DocDate , RefNo, RefDate, CreatedBy, CreatedDate
			) 
			select 
				CompanyCode, BranchCode,year(DocDate) PeriodTaxYear, month(DocDate) PeriodTaxMonth, ProfitCenterCode
				, @CurrentFPJ FPJGovNo
				, DocDate as FPJGovDate, DocNo, DocDate , RefNo, RefDate, @UserId CreatedBy,getDate() CreatedDate
			from 
				#f1
			where CompanyCode = @CurrentCompanyCode
			  and BranchCode = @CurrentBranchCode
			  and DocNo = @CurrentDocNo
			  and TaxTransCode = @CurrentTaxTransCode

			 --update Sparepart
			update	SPTrnSFPJHdr
			set		FPJGovNo= @CurrentFPJ
					,FPJSignature= a.DocDate
			from	#f1 a, SPTrnSFPJHdr b
			where	a.CompanyCode= b.CompanyCode and a.BranchCode= b.BranchCode and a.DocNo= b.InvoiceNo 
				and b.CompanyCode = @CurrentCompanyCode and b.BranchCode = @CurrentBranchCode and b.InvoiceNo = @CurrentDocNo

			-- update Service
			update	SVTrnFakturPajak
			set		FPJGovNo= @CurrentFPJ
					,SignedDate= a.DocDate
			from	#f1 a, SVTrnFakturPajak b
			where	a.CompanyCode = b.CompanyCode and a.BranchCode = b.BranchCode and a.DocNo= b.FPJNo
				and b.CompanyCode = @CurrentCompanyCode  and b.BranchCode = @CurrentBranchCode  and b.FPJNo = @CurrentDocNo

			-- update Sales
			update	OmFakturPajakHdr
			set		FakturPajakNo= @CurrentFPJ
					,FakturPajakDate= a.DocDate 
			from	#f1 a, OmFakturPajakHdr b
			where	a.CompanyCode = b.CompanyCode and a.BranchCode = b.BranchCode and a.DocNo= b.InvoiceNo
				and b.CompanyCode = @CurrentCompanyCode  and b.BranchCode = @CurrentBranchCode  and b.InvoiceNo = @CurrentDocNo

			-- update Finance
			update	ArFakturPajakHdr
			set		FakturPajakNo= @CurrentFPJ
					,FakturPajakDate= a.DocDate
			from	#f1 a, ArFakturPajakHdr b
			where	a.CompanyCode = b.CompanyCode and a.BranchCode = b.BranchCode and a.DocNo= b.InvoiceNo
				and b.CompanyCode = @CurrentCompanyCode  and b.BranchCode = @CurrentBranchCode  and b.InvoiceNo = @CurrentDocNo
				
			FETCH NEXT FROM temp INTO @CurrentCompanyCode,@CurrentBranchCode,@CurrentDocNo,@CurrentTaxTransCode,@OrderNo,@CurrentCustCodeBill
		END
	CLOSE temp
	DEALLOCATE temp
	
	drop table #f1

	select @TotalFPJ FPJGovNo
end

-- update TransDate Sparepart
update GnMstCoProfileSpare 
set TransDate = convert(datetime, @FPJDate)
WHERE CompanyCode = @CompanyCode  
	--AND BranchCode like @BranchCode
	and convert(datetime, TransDate, 112) < @FPJDate

-- update TransDate Service
update GnMstCoProfileService 
set TransDate = convert(datetime, @FPJDate) 
WHERE CompanyCode = @CompanyCode 
	--AND BranchCode like @BranchCode
	and convert(datetime, TransDate, 112) < @FPJDate 

-- update TransDate Sales
update GnMstCoProfileSales 
set TransDate = convert(datetime, @FPJDate) 
WHERE CompanyCode = @CompanyCode 
	--AND BranchCode like @BranchCode
	and convert(datetime, TransDate, 112) < @FPJDate 

-- update TransDate Finance
update GnMstCoProfileFinance 
set TransDateAR = convert(datetime, @FPJDate) 
WHERE CompanyCode = @CompanyCode 
	--AND BranchCode like @BranchCode
	and convert(datetime, TransDateAR, 112) < @FPJDate


END

GO
