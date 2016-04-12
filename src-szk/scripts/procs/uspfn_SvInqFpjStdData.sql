USE [BIT_20130325]
GO
/****** Object:  StoredProcedure [dbo].[uspfn_SvInqFpjData]    Script Date: 10/10/2013 3:39:16 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
alter procedure [dbo].[uspfn_SvInqFpjStdData]    
		 @CompanyCode nvarchar(20),    
		 @BranchCode nvarchar(20),    
		 @ProductType nvarchar(2),
		 @FPJNo nvarchar(20),
		 @InvoiceNoStart nvarchar(20),
		 @InvoiceNoEnd nvarchar(20)
		
    
as    

--set @CompanyCode = '6006406'
--set @BranchCode = '6006402'
--set @ProductType = '4W'
--set @FPJNo = 'FPS/13/002075'
--set @InvoiceNoStart = 'INF/13/000472'
--set @InvoiceNoEnd = 'INF/13/000472'
    
declare	 @InvoiceStatus char(1)
set @InvoiceStatus = 0
if(@FPJNo != '')
Set @InvoiceStatus = 1

select	row_number() over(order by inv.InvoiceNo) RowNum
                        , convert(bit, 1) IsSelected
						, inv.CompanyCode
						, inv.BranchCode
                        , inv.ProductType
						, inv.InvoiceNo
		                , inv.InvoiceDate
		                , inv.JobOrderNo
		                , inv.JobOrderDate
		                , inv.TotalDPPAmt
		                , inv.TotalPPHAmt + TotalPPNAmt as TotalPpnAmt
		                , inv.TotalSrvAmt
		                , inv.JobType
		                , inv.PoliceRegNo
		                , inv.BasicModel
		                , inv.ChassisCode
		                , inv.ChassisNo
		                , inv.EngineCode
		                , inv.EngineNo
                        , inv.TOPCode
                        , inv.TOPDays
                        , inv.DueDate
                        , inv.FPJNo
                        , inv.FPJDate
                        , inv.CustomerCodeBill
                        , inv.Odometer
                        , inv.IsPkp
                        , inv.CustomerCode
		                , inv.CustomerCode + ' - ' + cust.CustomerName Pelanggan
		                , inv.CustomerCodeBill + ' - ' + custBill.CustomerName Pembayar
                        , inv.DueDate
                from	svTrnInvoice inv with(nolock, nowait)
		                left join gnMstCustomer cust with(nolock, nowait) on inv.CompanyCode = cust.CompanyCode
			                and inv.CustomerCode = cust.CustomerCode
		                left join gnMstCustomer custBill with(nolock, nowait) on inv.CompanyCode = custBill.CompanyCode
			                and inv.CustomerCodeBill = custBill.CustomerCode
                where	inv.CompanyCode = @CompanyCode 
		                and inv.BranchCode = @BranchCode 
		                and inv.ProductType = @ProductType
		                and inv.InvoiceNo between @InvoiceNoStart and @InvoiceNoEnd
		                and inv.CustomerCodeBill = isnull((select top 1 CustomerCodeBill from svTrnInvoice where InvoiceNo = @InvoiceNoStart),'')
                       and (CASE WHEN inv.InvoiceStatus = 0 THEN @InvoiceStatus ELSE inv.InvoiceStatus END) = inv.InvoiceStatus
                        and inv.FPJNo = @FPJNo
                        and (inv.InvoiceNo like 'INW%' or inv.InvoiceNo like 'INF%')

