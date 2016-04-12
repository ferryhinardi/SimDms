alter procedure uspfn_PmMonitoring
	@Periode     datetime,
	@InquiryType varchar(10)

as

;with x as (
select InqType
     , substring(InqDate, 7,2) + substring(InputDate, 7,2) as 'key'
	 , InqData as value
	 , InqDate
     , substring(InputDate, 7,2) + substring(InqDate, 7,2) as 'input_inq'
  from PmMonitoring
 where InqType = @InquiryType
   and InqDate 
	between convert(varchar, dateadd(day, -7, @Periode), 112)
		and convert(varchar, @Periode, 112)
   and InputDate
	between convert(varchar, dateadd(day, -6, @Periode), 112)
		and convert(varchar, dateadd(day, 7, @Periode), 112)
union all
select InqType
     , substring(InqDate, 7,2) + substring(convert(varchar, dateadd(day, -7, @Periode), 112), 7,2) as 'key'
	 , sum(InqData) as value
	 , InqDate
     , substring(convert(varchar, dateadd(day, -7, @Periode), 112), 7,2) + substring(InqDate, 7,2) as 'input_inq'
  from PmMonitoring
 where InqType = @InquiryType
   and InqDate 
	between convert(varchar, dateadd(day, -7, @Periode), 112)
		and convert(varchar, @Periode, 112)
   and InputDate <= convert(varchar, dateadd(day, -7, @Periode), 112)
 group by InqType, InqDate
)

select * from x order by InqType, InqDate

go

--select getdate(), dateadd(day, -7, getdate()) 

exec uspfn_PmMonitoring '2014-07-04', 'INQ'

