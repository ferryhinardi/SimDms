
go
if object_id('uspfn_spgnMstAccountView') is not null
	drop procedure uspfn_spgnMstAccountView

go
create procedure uspfn_spgnMstAccountView (  
	@CompanyCode varchar(10),
    @BranchCode varchar(10), 
    @Search varchar(50) = ''
)
as
begin
	 IF @Search <> ''
		SELECT   AccountNo, [Description], CompanyCode,  BranchCode  FROM gnMstAccount 
		where CompanyCode=@CompanyCode and BranchCode=@BranchCode
		and (AccountNo like '%' + @Search + '%' or [Description] like '%' + @Search + '%')

	 ELSE
		SELECT   AccountNo, [Description], CompanyCode,  BranchCode  FROM gnMstAccount 
		where CompanyCode=@CompanyCode and BranchCode=@BranchCode
end



