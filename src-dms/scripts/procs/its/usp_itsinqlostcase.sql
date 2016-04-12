if object_id('uspfn_InquiryITSWithStatusByType') is not null
	drop procedure uspfn_InquiryITSWithStatusByType
GO
create procedure usp_itsinqlostcase
	@CompanyCode varchar(20),
	@EmployeeID varchar(20),
	@DateFrom varchar(10),
	@DateTo varchar(10)
as

select a.InquiryNumber
     --, a.BranchCode
	 --, a.EmployeeID
	 --, a.OutletID
	 , a.NamaProspek
	 , a.InquiryDate
	 , a.TipeKendaraan
	 , a.Variant
	 , a.PerolehanData
	 , d.LookUpValueName as PerolehanDataDesc
	 , b.UpdateDate as LostDate
	 , a.LostCaseCategory as LostCaseCategoryCode
	 , c.LookUpValueName as LostCaseCategoryDesc
	 , a.LostCaseOtherReason
	 , a.LostCaseVoiceOfCustomer
	 , b.LastProgress
	 , b.UpdateDate
  from PmKDP a 
  left join PmStatusHistory b
    on b.CompanyCode = a.CompanyCode
   and b.BranchCode = a.BranchCode 
   and b.InquiryNumber = a.InquiryNumber
   and b.SequenceNo = (select top 1 SequenceNo from PmStatusHistory where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InquiryNumber = a.InquiryNumber order by SequenceNo desc)
  left join GnMstLookUpDtl c
    on c.CompanyCode = a.CompanyCode
   and c.CodeID = 'PLCC'
   and c.LookUpValue = a.LostCaseCategory
  left join GnMstLookUpDtl d
    on d.CompanyCode = a.CompanyCode
   and d.CodeID = 'PSRC'
   and d.LookUpValue = a.PerolehanData
 where a.CompanyCode = @CompanyCode
   and a.EmployeeID = @EmployeeID
   and a.LastProgress = 'LOST'
   and convert(varchar, b.UpdateDate, 112) between @DateFrom and @DateTo
 order by a.InquiryNumber

--select * from PmStatusHistory where InquiryNumber = '387904'

go
