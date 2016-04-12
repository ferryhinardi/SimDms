-- =============================================
-- Author:		<yo>
-- Create date: <24 Feb 2014>
-- Description:	<Generate ITS>
-- =============================================
ALTER PROCEDURE [dbo].[uspfn_GenerateITS]
  @StartDate			DATETIME,
  @EndDate				DATETIME,
  @Area					varchar(100),
  @DealerCode			varchar(100),
  @OutletCode			varchar(100), 
  @FilterBy		varchar(1)
AS
BEGIN

--declare @StartDate	DATETIME
--declare @EndDate	DATETIME
--declare @Area		varchar(100)
--declare @DealerCode	varchar(100)
--declare @OutletCode	varchar(100)
--declare @FilterBy	varchar(1)

--set @StartDate = '20140201'
--set @EndDate	= '20140228'
--set @Area = 'JABODETABEK'
--set @DealerCode = '6158401'
--set @OutletCode = ''
--set @FilterBy = '0'	-- 0 : Inquiry Date, 1 : Next Follow Up Date

if @FilterBy = '0'
begin
	select h.InquiryNumber, convert(varchar, h.InquiryDate, 103) InquiryDate, d.Area, h.CompanyCode, d.DealerName, o.OutletAbbreviation, 
       h.TipeKendaraan, h.Variant, h.Transmisi, h.ColourCode, h.Wiraniaga, h.SalesCoordinator, 
       h.SalesHead, h.LastProgress, h.LostCaseCategory, h.LostCaseReasonID, convert(varchar, h.SPKDate, 103) SPKDate, 
       h.QuantityInquiry, 'W' WiraniagaFlag, ' ' Grading, h.BranchHead, h.PerolehanData, 
       h.TestDrive, h.CaraPembayaran, isnull(h.Leasing, '') Leasing, isnull(h.DownPayment, '') DownPayment, h.Tenor, h.NamaProspek, 
       replace(h.AlamatProspek,'<',' ') AlamatProspek, h.TelpRumah, 
       --convert(varchar, h.LastUpdateStatus, 103) NextFollowUpdate, 
        convert(varchar, (select top 1 NextFollowUpDate from SuzukiR4..pmActivities 
		where CompanyCode = h.CompanyCode and BranchCode = h.BranchCode and InquiryNumber = h.InquiryNumber
		order by NextFollowUpDate desc), 103) NextFollowUpdate,
       h.NamaPerusahaan, 
       h.AlamatPerusahaan, h.Handphone, h.MerkLain
  from SuzukiR4..pmHstITS h with(nolock, nowait)
  inner join SuzukiR4..gnMstDealerMapping d with(nolock, nowait) on h.CompanyCode=d.DealerCode
  inner join SuzukiR4..gnMstDealerOutletMapping o with(nolock, nowait) on h.CompanyCode=o.DealerCode
   and h.BranchCode =o.OutletCode
   and o.GroupNo = d.GroupNo
where (CASE WHEN @Area = '' THEN '' ELSE d.Area END) = @Area
	AND (CASE WHEN @DealerCode = '' THEN '' ELSE h.CompanyCode END) = @DealerCode
	AND (CASE WHEN @OutletCode = '' THEN '' ELSE h.BranchCode END) = @OutletCode	 
	and convert(varchar,h.InquiryDate,112) between convert(varchar,@StartDate,112) and convert(varchar,@EndDate,112)
order by d.DealerName, o.OutletCode, h.InquiryNumber

end
else if @FilterBy = '1'
begin
	select h.InquiryNumber, convert(varchar, h.InquiryDate, 103) InquiryDate, d.Area, h.CompanyCode, d.DealerName, o.OutletAbbreviation, 
       h.TipeKendaraan, h.Variant, h.Transmisi, h.ColourCode, h.Wiraniaga, h.SalesCoordinator, 
       h.SalesHead, h.LastProgress, h.LostCaseCategory, h.LostCaseReasonID, convert(varchar, h.SPKDate, 103) SPKDate, 
       h.QuantityInquiry, 'W' WiraniagaFlag, ' ' Grading, h.BranchHead, h.PerolehanData, 
       h.TestDrive, h.CaraPembayaran, isnull(h.Leasing, '') Leasing, isnull(h.DownPayment, '') DownPayment, h.Tenor, h.NamaProspek, 
       replace(h.AlamatProspek,'<',' ') AlamatProspek, h.TelpRumah, 
       convert(varchar, (select top 1 NextFollowUpDate from SuzukiR4..pmActivities 
		where CompanyCode = h.CompanyCode and BranchCode = h.BranchCode and InquiryNumber = h.InquiryNumber
		order by NextFollowUpDate desc), 103) NextFollowUpdate
       --convert(varchar, h.LastUpdateStatus, 103) NextFollowUpdate
       , h.NamaPerusahaan, 
       h.AlamatPerusahaan, h.Handphone, h.MerkLain
  from SuzukiR4..pmHstITS h with(nolock, nowait)
	inner join SuzukiR4..gnMstDealerMapping d with(nolock, nowait) on h.CompanyCode=d.DealerCode
	inner join SuzukiR4..gnMstDealerOutletMapping o with(nolock, nowait) on h.CompanyCode=o.DealerCode and h.BranchCode =o.OutletCode 
	and o.GroupNo = d.GroupNo
where (CASE WHEN @Area = '' THEN '' ELSE d.Area END) = @Area
	AND (CASE WHEN @DealerCode = '' THEN '' ELSE h.CompanyCode END) = @DealerCode
	AND (CASE WHEN @OutletCode = '' THEN '' ELSE h.BranchCode END) = @OutletCode	
	and convert(varchar, (select top 1 NextFollowUpDate from SuzukiR4..pmActivities 
		where CompanyCode = h.CompanyCode and BranchCode = h.BranchCode and InquiryNumber = h.InquiryNumber
		order by NextFollowUpDate desc), 112) between convert(varchar,@StartDate,112) and convert(varchar,@EndDate,112)
	--and convert(varchar,h.LastUpdateStatus,112) between convert(varchar,@StartDate,112) and convert(varchar,@EndDate,112)
order by d.DealerName, o.OutletCode
end

END
