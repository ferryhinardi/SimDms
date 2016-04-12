
if object_id('uspfn_SysDealerHistGet') is not null
	drop procedure uspfn_SysDealerHistGet

go
CREATE procedure uspfn_SysDealerHistGet
as
declare @t_itsseq as table
(
	CompanyCode varchar(20),
	CompanyName varchar(100),
	LastUpdateDate datetime,
	LastUploadDate datetime,
	LastSendDate datetime,
	RealDelayDate int,
	DelayDate int
)

;with x as (
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
		 , LastUpdateDate
		 , LastUploadDate
		 , LastSendDate
		 , DelayDate = datediff(day, LastSendDate, getdate())
      from y
)

insert into @t_itsseq
select CompanyCode
     , CompanyName
	 , LastUpdateDate
	 , LastUploadDate
	 , LastSendDate
	 , RealDelayDate = DelayDate 
	 , DelayDate = (case when DelayDate > 7 then 7 else DelayDate end)
  from z

;with x as (
select a.UploadID, a.DealerCode
     , b.CompanyName
     , a.UploadCode, a.TableName, UploadedDate
	 , SequenceDate = (
		case TableName
	      when 'PmHstITS' then isnull((select max(LastSendDate) from @t_itsseq where CompanyCode = a.DealerCode), '2000-01-01')
		else UploadedDate end
	 ) 
	 , PriorityOrder = (
		case 
	      when (TableName = 'HrEmployee' and (convert(varchar, UploadedDate, 112) < convert(varchar, getdate(), 112))) then 1
	      when (TableName = 'HrEmployeeAchievement' and (convert(varchar, UploadedDate, 112) < convert(varchar, getdate(), 112))) then 2
	      when (TableName = 'HrEmployeeTraining' and (convert(varchar, UploadedDate, 112) < convert(varchar, getdate(), 112))) then 3
	      when (TableName = 'HrEmployeePosition' and (convert(varchar, UploadedDate, 112) < convert(varchar, getdate(), 112))) then 4
	      when (TableName = 'HrEmployeeSales' and (convert(varchar, UploadedDate, 112) < convert(varchar, getdate(), 112))) then 5
	      when (TableName = 'PmHstITS' and (convert(varchar, UploadedDate, 112) < convert(varchar, getdate(), 112))) then 6
		  else 7 
		end
	 ) 
	 , a.FileName
	 , a.FilePath
	 , a.FileSize
	 , a.FileType
  from SysDealerHist a
  left join gnMstOrganizationHdr b
    on b.CompanyCode = a.DealerCode
 where a.Status not in ('PROCESSED','ERROR')
   and a.FileSize > 0
)

select * from x order by PriorityOrder, SequenceDate
