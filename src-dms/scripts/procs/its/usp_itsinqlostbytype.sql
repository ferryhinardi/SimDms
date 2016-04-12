if object_id('uspfn_InquiryITSWithStatusByType') is not null
	drop procedure uspfn_InquiryITSWithStatusByType
GO
create procedure usp_itsinqlostbytype
	@CompanyCode varchar(20),
	@EmployeeID varchar(20),
	@DateFrom varchar(10),
	@DateTo varchar(10)
as

select a.TipeKendaraan as name
	 , count(a.TipeKendaraan) as value
  from PmKDP a 
  left join PmStatusHistory b
    on b.CompanyCode = a.CompanyCode
   and b.BranchCode = a.BranchCode 
   and b.InquiryNumber = a.InquiryNumber
   and b.SequenceNo = (select top 1 SequenceNo from PmStatusHistory where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InquiryNumber = a.InquiryNumber order by SequenceNo desc)
 where a.CompanyCode = @CompanyCode
   and a.EmployeeID = @EmployeeID
   and a.LastProgress = 'LOST'
   and convert(varchar, b.UpdateDate, 112) between @DateFrom and @DateTo
 group by a.TipeKendaraan

--select * from PmStatusHistory where InquiryNumber = '387904'

go
