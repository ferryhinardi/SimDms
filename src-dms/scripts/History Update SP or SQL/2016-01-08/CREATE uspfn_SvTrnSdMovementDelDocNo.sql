create procedure [dbo].[uspfn_SvTrnSdMovementDelDocNo]
	@CompanyCode   varchar(15),
	@BranchCode    varchar(15),
	@InvoiceNo     varchar(20)
as  
begin
	declare @sql varchar(4000);
	set @sql = 'DELETE FROM ' + dbo.GetDbMD(@CompanyCode, @BranchCode) + '..svSdMovement'
		+ ' WHERE CompanyCode =''' + @CompanyCode + '''' 
		+ ' AND BranchCode =''' + @BranchCode + ''''
		+ ' AND DocNo =''' + @InvoiceNo + '''' 
		+ ' AND ProcessStatus=''0''';

	--print (@sql);
	exec(@sql);
end