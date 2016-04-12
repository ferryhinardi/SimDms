alter procedure uspfn_InqSvTrnService
as

declare @TableName varchar(50)
select @TableName = 'SvTrnService'

;with x as (
    select a.CompanyCode
         , a.CompanyName
         --, LastUpdateDate = isnull(( select top 1 LastUpdateDate from SvTrnService where CompanyCode = a.CompanyCode order by LastUpdateDate desc), '1900-01-01' )
         , LastUpdateDate = isnull((select top 1 LastUpdate from SysDealer where DealerCode = a.CompanyCode and TableName = @TableName order by LastUpdate desc), '1900-01-01')
         , LastUploadDate = isnull((select top 1 LastUpdate from SysDealer where DealerCode = a.CompanyCode and TableName = @TableName order by LastUpdate desc), '1900-01-01')
      from gnMstOrganizationHdr a
     where isnull(a.DealerType, '') != '2W'
       and CompanyCode not in ('6006406','6006408','6006410','6007402')
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
	 , DelayDate = (case when (DelayDate > 30) then DelayDate when (DelayDate > 7) then DelayDate else DelayDate end)
  from z
 where DelayDate > -1
   and len(rtrim(CompanyCode)) = 7
   --and DelayDate < 500
 order by DelayDate desc


