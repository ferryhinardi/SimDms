CREATE PROCEDURE [dbo].[uspfn_svSelectInvoiceForCancellation]
@CompanyCode varchar(15), @BranchCode varchar(15), @ProductType varchar(2), @InvoiceNo varchar(15) = ''
AS
begin

--declare @CompanyCode varchar(15), @BranchCode varchar(15), @ProductType varchar(2), @InvoiceNo varchar(15) = ''
--set @CompanyCode = '6159401000'
--set @BranchCode = '6159401001'
--set @ProductType = '4W'
--set @InvoiceNo = 'INC/15/002968'
--exec uspfn_svSelectInvoiceForCancellation @CompanyCode, @BranchCode, @ProductType, @InvoiceNo

	DECLARE @addWhere varchar(4000);
	if(@InvoiceNo = '') begin
		set @addWhere = ''
	end else begin
		set @addWhere = ' AND Invoice.InvoiceNo =''' + @InvoiceNo + '''';
	end

	DECLARE @Query varchar(max);
	SET @Query = 'SELECT TOP 1500
		Invoice.ProductType, Invoice.InvoiceNo, 
		case Invoice.InvoiceDate when (''19000101'') then null else Invoice.InvoiceDate end as InvoiceDate
		,Invoice.InvoiceStatus, Invoice.FPJNo
		,case Invoice.FPJDate when (''19000101'') then null else Invoice.FPJDate end as FPJDate
		,Invoice.JobOrderNo
		,case Invoice.JobOrderDate when (''19000101'') then null else Invoice.JobOrderDate end as JobOrderDate
		,Invoice.JobType, Invoice.ChassisCode, Invoice.ChassisNo, Invoice.EngineCode
		,Invoice.EngineNo, Invoice.PoliceRegNo, Invoice.BasicModel, Invoice.CustomerCode, Invoice.CustomerCodeBill
		,Invoice.Remarks, (Invoice.CustomerCode + '' - '' + Cust.CustomerName) as Customer
		,(Invoice.CustomerCodeBill + '' - '' + CustBill.CustomerName) as CustomerBill
		, vehicle.ServiceBookNo, Invoice.Odometer
		FROM svTrnInvoice Invoice
		LEFT JOIN gnMstCustomer Cust
			ON Cust.CompanyCode = Invoice.CompanyCode AND Cust.CustomerCode = Invoice.CustomerCode
		LEFT JOIN gnMstCustomer CustBill
			ON CustBill.CompanyCode = Invoice.CompanyCode AND CustBill.CustomerCode = Invoice.CustomerCodeBill
		LEFT JOIN svMstcustomerVehicle vehicle 
			ON Invoice.CompanyCode = vehicle.CompanyCode and Invoice.ChassisCode = vehicle.ChassisCode and 
			Invoice.ChassisNo = vehicle.ChassisNo and Invoice.EngineCode = vehicle.EngineCode and 
			Invoice.EngineNo = vehicle.EngineNo and Invoice.BasicModel = vehicle.BasicModel	
		WHERE Invoice.CompanyCode = ''' + @CompanyCode + ''' AND Invoice.BranchCode = ''' + @BranchCode + ''' 
			AND Invoice.ProductType = ''' + @ProductType + '''' + @addWhere + '
			AND convert(varchar, Invoice.InvoiceDate, 112) >= isnull((
					select top 1 convert(varchar, FromDate, 112) from gnMstPeriode
					 where 1 = 1
					   and CompanyCode = ''' + @CompanyCode + '''
					   and BranchCode = ''' + @BranchCode + '''
					   and FiscalYear = (select FiscalYear from gnMstCoProfileService 
							where CompanyCode = ''' + @CompanyCode + ''' and BranchCode = ''' + @BranchCode + ''')
					 order by FromDate
					), '''') 
			AND EXISTS (
		select * from glInterface
		 where CompanyCode = Invoice.CompanyCode
		   and BranchCode = Invoice.BranchCode
		   and DocNo = Invoice.InvoiceNo
		   and StatusFlag  = 0
		)
		AND (NOT EXISTS (
			select * from arInterface
			 where CompanyCode = Invoice.CompanyCode
			   and BranchCode = Invoice.BranchCode
			   and DocNo = Invoice.InvoiceNo
			) 
			OR EXISTS (
			select * from arInterface
			 where CompanyCode = Invoice.CompanyCode
			   and BranchCode = Invoice.BranchCode
			   and DocNo = Invoice.InvoiceNo
			   and StatusFlag  = 0
			   and ReceiveAmt  = 0
			   and BlockAmt    = 0
			   and DebetAmt    = 0
			   and CreditAmt   = 0
			)
		)
		AND NOT EXISTS (
		select * from svTrnFakturPajak
		 where CompanyCode = Invoice.CompanyCode
		   and BranchCode = Invoice.BranchCode
		   and FPJNo = Invoice.FPJNo
		   and isnull(FPJGovNo, '''') <> ''''
		)';
        
	--print(@Query);
	exec (@Query);
END