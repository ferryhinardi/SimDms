alter procedure uspfn_InqDataIts
as

with x as (
    select a.CompanyCode
            , a.CompanyName
            , LastUpdateDate = isnull(( select top 1 x.LastUpdateDate from pmHstITS x where x.CompanyCode=a.CompanyCode order by x.LastUpdateDate desc), '1900-01-01' )
            , LastUploadDate = isnull((select top 1 UploadedDate from SysDealerHist where DealerCode = a.CompanyCode and TableName = 'PmHstIts' order by UploadedDate desc), '1900-01-01')
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
         , 'SITSH' as DataType
		 , LastUpdateDate
		 , LastUploadDate
		 , LastSendDate
		 , DelayDate = datediff(day, LastSendDate, getdate())
      from y
)
select CompanyCode
     , CompanyName
	 , DataType
	 , LastUpdateDate
	 , LastUploadDate
	 , LastSendDate
	 , RealDelayDate = DelayDate 
	 , DelayDate = (case when DelayDate > 7 then 7 else DelayDate end)
	 , 'PmHstIts' as TableName
  from z
 where DelayDate > 0
   and len(rtrim(CompanyCode)) = 7
 order by DelayDate desc
 