
go
if object_id('uspfn_PmKdpSend') is not null
	drop procedure uspfn_PmKdpSend

go
create procedure uspfn_PmKdpSend 
	@LastUpdateDate datetime,
	@Segment int
as

select * into #t1 from (
select top (@Segment) InquiryNumber, BranchCode, CompanyCode, EmployeeID, SpvEmployeeID, InquiryDate, OutletID, StatusProspek, PerolehanData, NamaProspek, AlamatProspek, TelpRumah, CityID, NamaPerusahaan, AlamatPerusahaan, Jabatan, Handphone, Faximile, Email, TipeKendaraan, Variant, Transmisi, ColourCode, CaraPembayaran, TestDrive, QuantityInquiry, LastProgress, LastUpdateStatus, SPKDate, LostCaseDate, LostCaseCategory, LostCaseReasonID, LostCaseOtherReason, LostCaseVoiceOfCustomer, CreationDate, CreatedBy, LastUpdateBy, LastUpdateDate, Leasing, DownPayment, Tenor, MerkLain
  from pmKDP
 where LastUpdateDate is not null
   and LastUpdateDate > @LastUpdateDate
 order by LastUpdateDate asc )#t1
 
declare @LastUpdateQry datetime
    set @LastUpdateQry = (select top 1 LastUpdateDate from #t1 order by LastUpdateDate desc)

select * from #t1
 union
select top 1000 InquiryNumber, BranchCode, CompanyCode, EmployeeID, SpvEmployeeID, InquiryDate, OutletID, StatusProspek, PerolehanData, NamaProspek, AlamatProspek, TelpRumah, CityID, NamaPerusahaan, AlamatPerusahaan, Jabatan, Handphone, Faximile, Email, TipeKendaraan, Variant, Transmisi, ColourCode, CaraPembayaran, TestDrive, QuantityInquiry, LastProgress, LastUpdateStatus, SPKDate, LostCaseDate, LostCaseCategory, LostCaseReasonID, LostCaseOtherReason, LostCaseVoiceOfCustomer, CreationDate, CreatedBy, LastUpdateBy, LastUpdateDate, Leasing, DownPayment, Tenor, MerkLain
  from pmKDP
 where LastUpdateDate = @LastUpdateQry
 
  drop table #t1

--go

--uspfn_PmKdpSend @LastUpdateDate='2013-10-10 00:00:00',@Segment=500

