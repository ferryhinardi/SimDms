alter procedure uspfn_SyncDataUpdToSuzukiR4

as

;with x as ( 
select CompanyCode, BranchCode, InquiryNumber
  from SuzukiR4..PmHstIts
 where exists (
	select 1 from PmHstIts a
	 where a.CompanyCode = CompanyCode
	   and a.BranchCode = BranchCode
	   and a.InquiryNumber = InquiryNumber
	   and a.LastUpdateDate != lastUpdateDate)
 )
delete x

;with x as (
select CompanyCode, BranchCode, InquiryNumber, SequenceNo
  from SuzukiR4..PmStatusHistory a
 where exists (
	select 1 from PmStatusHistory a
	 where a.CompanyCode = CompanyCode
	   and a.BranchCode = BranchCode
	   and a.InquiryNumber = InquiryNumber
	   and a.SequenceNo = SequenceNo
	   and a.UpdateDate != UpdateDate)
 )
delete x

;with x as (
select CompanyCode, BranchCode, Year, Month
     , CompanyName, BranchName, SoNo, InvoiceNo
  from SuzukiR4..omHstInquirySales
 where exists (
		select top 1 1 from omHstInquirySales a
		 where CompanyCode = a.CompanyCode
		   and BranchCode = a.BranchCode
		   and Year = a.Year
		   and Month = a.Month
		   and SoNo = a.SoNo
		   and InvoiceNo = a.InvoiceNo
		   and ChassisCode = a.ChassisCode
		   and ChassisNo = a.ChassisNo
		   and LastUpdateDate != a.LastUpdateDate)
 )
delete x

;with x as (
select CompanyCode, BranchCode, InquiryNumber, ActivityID
  from SuzukiR4..pmActivities 
 where exists (
	select top 1 1 from pmActivities a
	 where CompanyCode = a.CompanyCode
	   and BranchCode = a.BranchCode
	   and InquiryNumber = a.InquiryNumber
	   and ActivityID = a.ActivityID
	   and LastUpdateDate != a.LastUpdateDate)
 )
delete x

;with x as (
select CompanyCode, BranchCode, OutletID
  from SuzukiR4..PmBranchOutlets
 where exists (
	select top 1 1 from PmBranchOutlets a
	 where CompanyCode = a.CompanyCode
	   and BranchCode = a.BranchCode
	   and OutletID = a.OutletID
	   and LastUpdateDate != a.LastUpdateDate)
 )
delete x

;with x as (
select CompanyCode, BranchCode, InquiryNumber, LastUpdateDate
  from PmKdpAdditional a
 where exists (
	select top 1 1 from SuzukiR4..PmKdpAdditional
	 where CompanyCode = a.CompanyCode
	   and BranchCode = a.BranchCode
	   and InquiryNumber = a.InquiryNumber
	   and LastUpdateDate != a.LastUpdateDate)
 )
delete x 


exec uspfn_SyncDataInsToSuzukiR4


