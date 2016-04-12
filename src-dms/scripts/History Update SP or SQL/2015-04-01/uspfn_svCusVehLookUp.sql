
GO
/****** Object:  StoredProcedure [dbo].[uspfn_svInvoiceForLookUp]    Script Date: 4/1/2015 10:06:59 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create PROCEDURE [dbo].[uspfn_svCusVehLookUp]
@CompanyCode varchar(15), @BranchCode varchar(15), @DynamicFilter varchar(4000) = ''
AS
begin
--declare @CompanyCode varchar(15), @BranchCode varchar(15), @DynamicFilter varchar(4000) = ''
--set @CompanyCode = '6159401000'
--set @BranchCode = '6159401001'
--set @DynamicFilter = ' 
--	and InvoiceNo like ''%INC/15/0029%''
--	and JobOrderNo like ''%SPK/15/003%'''

DECLARE @Query varchar(max);
	SET @Query = 'select top 500 * from SvCustomerVehicleView
	WHERE CompanyCode = ''' + @CompanyCode + ''' AND BranchCode = ''' + @BranchCode + '''' 
            +@DynamicFilter
          
        
	--print(@Query);
	exec (@Query)
END