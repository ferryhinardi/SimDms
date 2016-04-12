ALTER procedure [dbo].[uspfn_GenerateITSIndent]
	@Date varchar(8)
as
begin
	--declare @Date varchar(8)	
	--set @Date = '20140616'

	select * into #ITS
    from ( 
            select b.CompanyCode, b.BranchCode, b.InquiryNumber, b.InquiryDate, b.OutletID, b.BranchHead, b.SalesHead, b.SalesCoordinator, b.Wiraniaga, b.StatusProspek
            , b.PerolehanData, b.NamaProspek, b.AlamatProspek, b.TelpRumah, b.City, b.NamaPerusahaan, b.AlamatPerusahaan, b.Jabatan, b.Handphone, b.Faximile, b.Email
            , b.TipeKendaraan, b.Variant, b.Transmisi, b.ColourCode, b.ColourDescription, b.CaraPembayaran, b.TestDrive, b.QuantityInquiry, a.LastProgress
            , b.LastUpdateStatus, b.ProspectDate, b.HotDate, b.SPKDate, b.DeliveryDate, b.Leasing, b.DownPayment, b.Tenor, b.LostCaseDate, b.LostCaseCategory
            , b.LostCaseReasonID, b.LostCaseOtherReason, b.LostCaseVoiceOfCustomer, b.MerkLain, b.CreatedBy, b.CreatedDate, b.LastUpdateBy, b.LastUpdateDate
      from suzukir4..pmStatusHistory a
      join suzukir4..pmHstITS b
        on b.CompanyCode = a.CompanyCode
       and b.BranchCode = a.BranchCode
       and b.InquiryNumber = a.InquiryNumber        
     inner join suzukir4..MsMstGroupModel c
        on c.ModelType = b.TipeKendaraan
     where 
       a.LastProgress = 'SPK'
       and not exists (select top 1 1 from suzukir4..pmStatusHistory 
                    where CompanyCode=a.CompanyCode
                      and BranchCode=a.BranchCode
                      and InquiryNumber=a.InquiryNumber
                      and LastProgress in ('LOST','DO','DELIVERY'))
                                                      
        ) #ITS
        
    select a.DealerAbbreviation, b.OutletAbbreviation, #ITS.* from #ITS 
	inner join  gnMstDealerMapping a on #ITS.CompanyCode = a.DealerCode 
	inner join  gnMstDealerOutletMapping b on #ITS.CompanyCode = b.DealerCode and #ITS.BranchCode = b.OutletCode and b.GroupNo = a.GroupNo              
    order by CompanyCode, BranchCode, InquiryNumber
    
    select distinct a.DealerAbbreviation, b.OutletAbbreviation, #ITS.* from #ITS 
    inner join  gnMstDealerMapping a on #ITS.CompanyCode = a.DealerCode 
	inner join  gnMstDealerOutletMapping b on #ITS.CompanyCode = b.DealerCode and #ITS.BranchCode = b.OutletCode and b.GroupNo = a.GroupNo               
    order by CompanyCode, BranchCode, InquiryNumber
    drop table #ITS
    
    exec SimDms..uspfn_SyncPmExecSummary2
end