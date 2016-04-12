alter procedure uspfn_SysDealerHistGet
	@length int = 100000
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
          --when (DealerCode = '6093401') then 0
		  when (TableName = 'gnMstCustDealer') then 1
          when FileSize > 50000 then (10 + (FileSize / 10000))
		  when (TableName = 'omTrInventQtyVehicle') then 1
		  when (TableName = 'spTrnSInvoiceHdr') then 1
	      when (TableName = 'PmHstITS') then 2
	      when (TableName = 'PmStatusHistory') then 2
		else 3 end
	 ) 
	 , a.FileName, a.FilePath, a.FileSize, a.FileType
  from SysDealerHist a
  left join gnMstOrganizationHdr b
    on b.CompanyCode = a.DealerCode
 where a.Status = 'UPLOADED'
   and a.FileSize > 0

   --and a.DealerCode = '6093401'
)
select top(@length) * from x order by PriorityOrder, SequenceDate

go

exec uspfn_SysDealerHistGet
