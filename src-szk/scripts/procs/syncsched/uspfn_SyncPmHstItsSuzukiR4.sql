alter procedure uspfn_SyncPmHstItsSuzukiR4

as

insert into SuzukiR4..PmHstIts (CompanyCode, BranchCode, InquiryNumber, InquiryDate, OutletID, BranchHead, SalesHead, SalesCoordinator, Wiraniaga, StatusProspek, PerolehanData, NamaProspek, AlamatProspek, TelpRumah, City, NamaPerusahaan, AlamatPerusahaan, Jabatan, Handphone, Faximile, Email, TipeKendaraan, Variant, Transmisi, ColourCode, ColourDescription, CaraPembayaran, TestDrive, QuantityInquiry, LastProgress, LastUpdateStatus, ProspectDate, HotDate, SPKDate, DeliveryDate, Leasing, DownPayment, Tenor, LostCaseDate, LostCaseCategory, LostCaseReasonID, LostCaseOtherReason, LostCaseVoiceOfCustomer, MerkLain, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
select top 10000 CompanyCode, BranchCode, InquiryNumber, InquiryDate, OutletID, BranchHead, SalesHead, SalesCoordinator, Wiraniaga, StatusProspek, PerolehanData, NamaProspek, AlamatProspek, TelpRumah, City, NamaPerusahaan, AlamatPerusahaan, Jabatan, Handphone, Faximile, Email, TipeKendaraan, Variant, Transmisi, ColourCode, ColourDescription, CaraPembayaran, TestDrive, QuantityInquiry, LastProgress, LastUpdateStatus, ProspectDate, HotDate, SPKDate, DeliveryDate, Leasing, DownPayment, Tenor, LostCaseDate, LostCaseCategory, LostCaseReasonID, LostCaseOtherReason, LostCaseVoiceOfCustomer, MerkLain, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate
  from PmHstIts a
 where not exists (
	select 1 from SuzukiR4..PmHstIts
	 where CompanyCode = a.CompanyCode
	   and BranchCode = a.BranchCode
	   and InquiryNumber = a.InquiryNumber
 )

 select 'done' as status