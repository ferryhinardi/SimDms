go
if object_id('uspfn_InqHrEmployeeSales') is not null
	drop procedure uspfn_InqHrEmployeeSales

go
CREATE procedure uspfn_InqHrEmployeeSales
as
declare @TableName varchar(50)
select @TableName = 'HrEmployeeSales'

;with x as (
    select a.CompanyCode
         , a.CompanyName
         , LastUpdateDate = isnull(( select top 1 LastUpdateDate from GnMstCustomer where CompanyCode = a.CompanyCode order by LastUpdateDate desc), '1900-01-01' )
         , LastUploadDate = isnull((select top 1 UploadedDate from SysDealerHist where DealerCode = a.CompanyCode and TableName = @TableName order by UploadedDate desc), '1900-01-01')
      from gnMstOrganizationHdr a
     where isnull(a.DealerType, '') != '2W'
 ),

 y as (
    select CompanyCode
         , CompanyName
         , LastUpdateDate
         , LastUploadDate
         , LastSendDate = (
		      case when (LastUpdateDate > LastUploadDate)
			  then (case when (LastUpdateDate > getdate()) then getdate() else LastUpdateDate end)
			  else LastUploadDate
			  end)
      from x
),

z as (
    select CompanyCode
         , CompanyName
		 , LastUpdateDate
		 , LastUploadDate
		 , LastSendDate
		 , DelayDate = datediff(day, LastSendDate, getdate())
      from y

)

select CompanyCode
     , CompanyName
     , @TableName as TableName
	 , LastUpdateDate
	 , LastUploadDate
	 , LastSendDate
	 , RealDelayDate = DelayDate 
	 , DelayDate = (case when (DelayDate > 1000) then 30 when (DelayDate > 7) then 7 else DelayDate end)
  from z
 where DelayDate < 2000
   and len(rtrim(CompanyCode)) = 7
 order by DelayDate desc


