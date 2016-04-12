alter procedure uspfn_SyncDataInsToSuzukiR4

as

--insert data SuzukiR4..PmHstIts 
insert into SuzukiR4..PmHstIts
     ( CompanyCode, BranchCode, InquiryNumber, InquiryDate
	 , OutletID, BranchHead, SalesHead, SalesCoordinator, Wiraniaga
	 , StatusProspek, PerolehanData, NamaProspek, AlamatProspek, TelpRumah
	 , City, NamaPerusahaan, AlamatPerusahaan, Jabatan, Handphone, Faximile
	 , Email, TipeKendaraan, Variant, Transmisi, ColourCode, ColourDescription
	 , CaraPembayaran, TestDrive, QuantityInquiry, LastProgress, LastUpdateStatus
	 , ProspectDate, HotDate, SPKDate, DeliveryDate, Leasing, DownPayment, Tenor
	 , LostCaseDate, LostCaseCategory, LostCaseReasonID, LostCaseOtherReason
	 , LostCaseVoiceOfCustomer, MerkLain, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
select top 10000
       CompanyCode, BranchCode, InquiryNumber, InquiryDate
	 , OutletID, BranchHead, SalesHead, SalesCoordinator, Wiraniaga
	 , StatusProspek, PerolehanData, NamaProspek, AlamatProspek, TelpRumah
	 , City, NamaPerusahaan, AlamatPerusahaan, Jabatan, Handphone, Faximile
	 , Email, TipeKendaraan, Variant, Transmisi, ColourCode, ColourDescription
	 , CaraPembayaran, TestDrive, QuantityInquiry, LastProgress, LastUpdateStatus
	 , ProspectDate, HotDate, SPKDate, DeliveryDate, Leasing, DownPayment, Tenor
	 , LostCaseDate, LostCaseCategory, LostCaseReasonID, LostCaseOtherReason
	 , LostCaseVoiceOfCustomer, MerkLain, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate
  from PmHstIts a
 where not exists (
	select 1 from SuzukiR4..PmHstIts
	 where CompanyCode = a.CompanyCode
	   and BranchCode = a.BranchCode
	   and InquiryNumber = a.InquiryNumber
 )

--insert data SuzukiR4..PmStatusHistory 
 insert into SuzukiR4..PmStatusHistory ( InquiryNumber, CompanyCode, BranchCode, SequenceNo, LastProgress, UpdateDate, UpdateUser)
select top 100000 InquiryNumber, CompanyCode, BranchCode, SequenceNo, LastProgress, UpdateDate, UpdateUser
  from PmStatusHistory a
 where not exists (
	select 1 from SuzukiR4..PmStatusHistory
	 where CompanyCode = a.CompanyCode
	   and BranchCode = a.BranchCode
	   and InquiryNumber = a.InquiryNumber
	   and SequenceNo = a.SequenceNo
 )

--insert data SuzukiR4..omHstInquirySales 
insert into SuzukiR4..omHstInquirySales 
     ( Year, Month, CompanyCode, BranchCode
     , CompanyName, BranchName, Area, BranchHeadID
	 , BranchHeadName, SalesHeadID, SalesHeadName, SalesCoordinatorID
	 , SalesCoordinatorName, SalesmanID, SalesmanName, JoinDate, ResignDate
	 , GradeDate, Grade, ModelCatagory, SalesType, SoNo, SODate
	 , InvoiceNo, InvoiceDate, FakturPolisiNo, FakturPolisiDate
	 , SalesModelCode, SalesModelYear, SalesModelDesc
	 , FakturPolisiDesc, MarketModel, GroupMarketModel
	 , ColumnMarketModel, ChassisCode, ChassisNo
	 , EngineCode, EngineNo, ColourCode, ColourName
	 , COGS, BeforeDiscDPP, DiscExcludePPn, DiscIncludePPn
	 , AfterDiscDPP, AfterDiscPPn, AfterDiscPPnBM, AfterDiscTotal
	 , PPnBMPaid, OthersDPP, OthersPPn, ShipAmt, DepositAmt, OthersAmt
	 , Status, DCSStatus, DCSDate, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate
	 , CategoryCode, SuzukiDODate, SuzukiFPolDate)
select top 10000 
       a.Year, a.Month, a.CompanyCode, a.BranchCode
     , a.CompanyName, a.BranchName, a.Area, a.BranchHeadID
	 , a.BranchHeadName, a.SalesHeadID, a.SalesHeadName, a.SalesCoordinatorID
	 , a.SalesCoordinatorName, a.SalesmanID, a.SalesmanName, a.JoinDate, a.ResignDate
	 , a.GradeDate, a.Grade, a.ModelCatagory, a.SalesType, a.SoNo, a.SODate
	 , a.InvoiceNo, a.InvoiceDate, a.FakturPolisiNo, a.FakturPolisiDate
	 , a.SalesModelCode, a.SalesModelYear, a.SalesModelDesc
	 , a.FakturPolisiDesc, a.MarketModel, a.GroupMarketModel
	 , a.ColumnMarketModel, a.ChassisCode, a.ChassisNo
	 , a.EngineCode, a.EngineNo, a.ColourCode, a.ColourName
	 , a.COGS, a.BeforeDiscDPP, a.DiscExcludePPn, a.DiscIncludePPn
	 , a.AfterDiscDPP, a.AfterDiscPPn, a.AfterDiscPPnBM, a.AfterDiscTotal
	 , a.PPnBMPaid, a.OthersDPP, a.OthersPPn, a.ShipAmt, a.DepositAmt, a.OthersAmt
	 , a.Status, a.DCSStatus, a.DCSDate, a.CreatedBy, a.CreatedDate, a.LastUpdateBy, a.LastUpdateDate
	 , a.CategoryCode, a.SuzukiDODate, a.SuzukiFPolDate
  from omHstInquirySales a
 where not exists (
		select top 1 1 from SuzukiR4..omHstInquirySales
		 where Year = a.Year
		   and Month = a.Month
		   and CompanyCode = a.CompanyCode
		   and BranchCode = a.BranchCode
		   and SoNo = a.SoNo
		   and InvoiceNo = a.InvoiceNo
		   and ChassisCode = a.ChassisCode
		   and ChassisNo = a.ChassisNo
	)

--insert data SuzukiR4..pmActivities 
insert into SuzukiR4..pmActivities 
     ( CompanyCode, BranchCode, InquiryNumber, ActivityID, ActivityDate, ActivityType, ActivityDetail
	 , NextFollowUpDate, CreationDate, CreatedBy, LastUpdateBy, LastUpdateDate)
select top 10000
       CompanyCode, BranchCode, InquiryNumber, ActivityID, ActivityDate, ActivityType, ActivityDetail
	 , NextFollowUpDate, CreationDate, CreatedBy, LastUpdateBy, LastUpdateDate
  from pmActivities a
 where not exists (
	select top 1 1 from SuzukiR4..pmActivities
	 where CompanyCode = a.CompanyCode
	   and BranchCode = a.BranchCode
	   and InquiryNumber = a.InquiryNumber
	   and ActivityID = a.ActivityID
 )

--insert data SuzukiR4..PmBranchOutlets 
insert into SuzukiR4..PmBranchOutlets (CompanyCode, BranchCode, OutletID, OutletName, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
select top 10000 CompanyCode, BranchCode, OutletID, OutletName, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate
  from PmBranchOutlets a
 where not exists (
	select top 1 1 from SuzukiR4..PmBranchOutlets
	 where CompanyCode = a.CompanyCode
	   and BranchCode = a.BranchCode
	   and OutletID = a.OutletID
 )

 --insert data SuzukiR4..PmKdpAdditional 
insert into SuzukiR4..PmKdpAdditional (CompanyCode, BranchCode, InquiryNumber, StatusVehicle, OthersBrand, OthersType, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
select top 10000 CompanyCode, BranchCode, InquiryNumber, StatusVehicle, OthersBrand, OthersType, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate
  from PmKdpAdditional a
 where not exists (
	select top 1 1 from SuzukiR4..PmKdpAdditional
	 where CompanyCode = a.CompanyCode
	   and BranchCode = a.BranchCode
	   and InquiryNumber = a.InquiryNumber
 )
