alter procedure uspfn_SysDataInfoStatusSuzukiR4
as

;with x as (
select 'OmHstInquirySales' as TableName
     , a.CompanyCode
	 , count(1) as RecordCount
	 , LastUpdateDate = isnull((
			select top 1 LastUpdateDate
			  from OmHstInquirySales
			 where CompanyCode = a.CompanyCode
			 order by LastUpdateDate desc), null)
  from OmHstInquirySales a
 group by a.CompanyCode
)
select * from x order by LastUpdateDate desc 

;with x as (
select 'PmActivities' as TableName
     , a.CompanyCode
	 , count(1) as RecordCount
	 , LastUpdateDate = isnull((
			select top 1 LastUpdateDate
			  from PmActivities
			 where CompanyCode = a.CompanyCode
			 order by LastUpdateDate desc), null)
  from PmActivities a
 group by a.CompanyCode
)
select * from x order by LastUpdateDate desc 


;with x as (
select 'PmKdpAdditional' as TableName
     , a.CompanyCode
	 , count(1) as RecordCount
	 , LastUpdateDate = isnull((
			select top 1 LastUpdateDate
			  from PmKdpAdditional
			 where CompanyCode = a.CompanyCode
			 order by LastUpdateDate desc), null)
  from PmKdpAdditional a
 group by a.CompanyCode
)
select * from x order by LastUpdateDate desc 



;with x as (
select 'PmBranchOutlets' as TableName
     , a.CompanyCode
	 , count(1) as RecordCount
	 , LastUpdateDate = isnull((
			select top 1 LastUpdateDate
			  from PmBranchOutlets
			 where CompanyCode = a.CompanyCode
			 order by LastUpdateDate desc), null)
  from PmBranchOutlets a
 group by a.CompanyCode
)
select * from x order by LastUpdateDate desc 

