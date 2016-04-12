declare
@companyCode varchar(25),
@branchCode1 varchar(25),
@userid varchar(25),
@periodYear int,
@ttlBranchCode int,
@countFlag int,
@countFlagMonth int


set @periodYear= YEAR(GETDATE())
set @userid='system'

declare @t_temp as table
(
	CompanyCode  varchar(25),
	BranchCode varchar(25)
)

declare @t_tempData as table
(
	CompanyCode  varchar(25),
	BranchCode varchar(25),
	PeriodYear  int,
	PeriodMonth int,
	userid varchar(25)
)

set @companyCode= ( select top 1 companycode from gnmstorganizationhdr)

set @ttlBranchCode =(select count(distinct branchcode) from [gnMstOrganizationDtl] where companycode=@companyCode)

set @countFlag=1
while(@countFlag <= @ttlBranchCode)
begin
	set @branchCode1  =(select top 1 BranchCode from [gnMstOrganizationDtl] where BranchCode not in (select BranchCode from @t_temp where CompanyCode=@companyCode))

	insert into @t_temp (CompanyCode,BranchCode)
	values(@companyCode,@branchCode1) 

	set @countFlagMonth=1
	
	while(@countFlagMonth <=12)
	begin 
		insert into @t_tempData (CompanyCode,BranchCode,PeriodYear,PeriodMonth,userid)
		values(@companyCode,@branchCode1,@periodYear,@countFlagMonth,@userid)

		exec [dbo].[usprpt_SvRpReport021V3SPKWeb] @companyCode,@branchCode1,'4W',@periodYear,1,@countFlagMonth,@userid
		set @countFlagMonth = @countFlagMonth+1
	end 
	
	set @countFlag= @countFlag+1;
end

delete @t_temp
delete @t_tempData