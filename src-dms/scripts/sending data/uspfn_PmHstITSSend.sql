alter procedure uspfn_PmHstITSSend
--declare 
	@LastUpdateDate datetime,
	@Segment int  
as  
--select @LastUpdateDate='1990-01-01 00:00:00',@Segment=500  
  
select * into #t1 from (  
select top (@Segment) CompanyCode, BranchCode, InquiryNumber, InquiryDate, OutletID
     , BranchHead, SalesHead, SalesCoordinator, Wiraniaga, StatusProspek, PerolehanData
	 , NamaProspek, AlamatProspek, TelpRumah, City, NamaPerusahaan, AlamatPerusahaan
	 , Jabatan, Handphone, Faximile, Email, TipeKendaraan, Variant, Transmisi, ColourCode
	 , ColourDescription, CaraPembayaran, TestDrive, QuantityInquiry, LastProgress, LastUpdateStatus
	 , ProspectDate, HotDate, SPKDate, DeliveryDate, Leasing, DownPayment, Tenor
	 , LostCaseDate, LostCaseCategory, LostCaseReasonID, LostCaseOtherReason, LostCaseVoiceOfCustomer
	 , MerkLain, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate
  from pmHstITS  
 where LastUpdateDate is not null  
   and LastUpdateDate > @LastUpdateDate  
 order by LastUpdateDate asc )#t1  
   
declare @LastUpdateQry datetime  
    set @LastUpdateQry = (select top 1 LastUpdateDate from #t1 order by LastUpdateDate desc)  
  
select * from #t1  
 union 
select top 100 CompanyCode, BranchCode, InquiryNumber, InquiryDate, OutletID
     , BranchHead, SalesHead, SalesCoordinator, Wiraniaga, StatusProspek, PerolehanData
	 , NamaProspek, AlamatProspek, TelpRumah, City, NamaPerusahaan, AlamatPerusahaan
	 , Jabatan, Handphone, Faximile, Email, TipeKendaraan, Variant, Transmisi, ColourCode
	 , ColourDescription, CaraPembayaran, TestDrive, QuantityInquiry, LastProgress, LastUpdateStatus
	 , ProspectDate, HotDate, SPKDate, DeliveryDate, Leasing, DownPayment, Tenor
	 , LostCaseDate, LostCaseCategory, LostCaseReasonID, LostCaseOtherReason, LostCaseVoiceOfCustomer
	 , MerkLain, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate
  from pmHstITS  
 where LastUpdateDate = @LastUpdateQry  
   
  drop table #t1  