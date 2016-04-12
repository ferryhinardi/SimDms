if object_id('usprpt_HeaderRgOutSS') is not null
       drop procedure usprpt_HeaderRgOutSS
GO

-- =============================================
-- Author:		fhy
-- Create date: 27102015
-- Description:	sp get data md for heder report register outstanding supply slip
-- =============================================
CREATE PROCEDURE [dbo].[usprpt_HeaderRgOutSS] 
	@userid varchar(50),
	@ReportID varchar(250)
AS
BEGIN
	declare
		@companycode varchar(50),
		@branchcode varchar(50),
		@companycodetemp varchar(50),
		@branchcodetemp varchar(50),
		@dbmd varchar(50)='',
		@qrytemp varchar(max),
		@flag int
	
set @companycode = (select a.CompanyCode from gnMstCoProfile a
						inner join SysUser b on a.CompanyCode=b.CompanyCode and a.BranchCode=b.BranchCode
					where b.UserID=@userid)
set @branchcode = (select a.branchcode from gnMstCoProfile a
						inner join SysUser b on a.CompanyCode=b.CompanyCode and a.BranchCode=b.BranchCode
					where b.UserID=@userid)

--select @companycode,@branchcode

--select * from gnMstCompanyMapping where CompanyCode=@companycode and BranchCode=@branchcode

set @flag = (select count(companycode) from gnMstCompanyMapping where CompanyCode=@companycode and BranchCode=@branchcode )

--select @flag

if(@flag=0)
begin
	set @companycodetemp=@companycode
	set @branchcodetemp=@branchcode
end
else
begin
	set @companycodetemp=(select CompanyMD from gnMstCompanyMapping where CompanyCode=@companycode and BranchCode=@branchcode)
	set @branchcodetemp=(select BranchMD from gnMstCompanyMapping where CompanyCode=@companycode and BranchCode=@branchcode)
	set @dbmd=(select dbmd from gnMstCompanyMapping where CompanyCode=@companycode and BranchCode=@branchcode)
end
--select * from gnMstCompanyMapping where CompanyCode=@companycode and BranchCode=@branchcode
--select @companycodetemp,@branchcodetemp,@dbmd

set @qrytemp='select * from '+@dbmd+'..gnMstCoProfile where companycode='''+@companycodetemp+''' and branchcode='''+@branchcodetemp+''''

exec (@qrytemp)
select * from SysReport where ReportID=@ReportID

END




GO


