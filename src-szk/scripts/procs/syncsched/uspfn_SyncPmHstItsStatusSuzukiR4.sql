alter procedure uspfn_SyncPmHstItsStatusSuzukiR4

as

insert into SuzukiR4..PmStatusHistory (InquiryNumber, CompanyCode, BranchCode, SequenceNo, LastProgress, UpdateDate, UpdateUser)
select top 100000 InquiryNumber, CompanyCode, BranchCode, SequenceNo, LastProgress, UpdateDate, UpdateUser
  from PmStatusHistory a
 where not exists (
	select 1 from SuzukiR4..PmStatusHistory
	 where CompanyCode = a.CompanyCode
	   and BranchCode = a.BranchCode
	   and InquiryNumber = a.InquiryNumber
	   and SequenceNo = a.SequenceNo
 )

select 'done' as status
