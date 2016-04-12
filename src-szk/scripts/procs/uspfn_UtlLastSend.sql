alter procedure uspfn_UtlLastSend
--declare
	@CompanyCode varchar (20),
	@DataType varchar(20),
	@DataStatus varchar(20)
as

--select @CompanyCode = '',@DataType = '', @DataStatus = ''

if @CompanyCode = ''
begin
	select distinct a.CompanyCode
		 , CompanyName = (select top 1 CompanyName from GnMstOrganizationHdr where CompanyCode = a.CompanyCode)
		 , DataType = isnull((
				select top 1 DataType
				  from GnMstScheduleData
				 where CompanyCode = a.CompanyCode
				   and DataType = (case isnull(@DataType, '') when '' then DataType else @DataType end)
				   and Status = (case isnull(@DataStatus, '') when '' then Status else @DataStatus end)
				 order by LastSendDate desc 
				), '')
		 , LastSendDate = isnull((
				select top 1 LastSendDate
				  from GnMstScheduleData
				 where CompanyCode = a.CompanyCode
				   and DataType = (case isnull(@DataType, '') when '' then DataType else @DataType end)
				   and Status = (case isnull(@DataStatus, '') when '' then Status else @DataStatus end)
				 order by LastSendDate desc 
				), '')
		 , UpdatedDate = isnull((
				select top 1 LastSendDate
				  from GnMstScheduleData
				  where CompanyCode = a.CompanyCode
				   and DataType = (case isnull(@DataType, '') when '' then DataType else @DataType end)
				   and Status = (case isnull(@DataStatus, '') when '' then Status else @DataStatus end)
				 order by LastSendDate desc 
				 ), '')
	  from GnMstScheduleData a
	 where 1 = 1
	   and a.DataType = (case isnull(@DataType, '') when '' then a.DataType else @DataType end)
	   and a.Status = (case isnull(@DataStatus, '') when '' then a.Status else @DataStatus end)
end
else
begin
	select distinct a.CompanyCode
		 , CompanyName = (select top 1 CompanyName from GnMstOrganizationHdr where CompanyCode = a.CompanyCode)
		 , a.DataType
		 , LastSendDate = isnull((
				select top 1 LastSendDate
				  from GnMstScheduleData
				 where CompanyCode = a.CompanyCode
				   and DataType = a.DataType
				   and Status = (case isnull(@DataStatus, '') when '' then Status else @DataStatus end)
				 order by LastSendDate desc 
				), '')
		 , UpdatedDate = isnull((
				select top 1 LastSendDate
				  from GnMstScheduleData
				  where CompanyCode = a.CompanyCode
				   and DataType = a.DataType
				   and Status = (case isnull(@DataStatus, '') when '' then Status else @DataStatus end)
				 order by LastSendDate desc 
				 ), '')
	  from GnMstScheduleData a
	 where CompanyCode = @CompanyCode
	   and a.DataType = (case isnull(@DataType, '') when '' then a.DataType else @DataType end)
	   and a.Status = (case isnull(@DataStatus, '') when '' then a.Status else @DataStatus end)
end
