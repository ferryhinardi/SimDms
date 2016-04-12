

/****** Object:  StoredProcedure [dbo].[uspfn_SvTrnPDIFSC]    Script Date: 04/29/2015 13:09:00 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

if object_id('uspfn_SvTrnPDIFSC') is not null
	drop PROCEDURE uspfn_SvTrnPDIFSC
GO
CREATE PROCEDURE [dbo].[uspfn_SvTrnPDIFSC]
 @CompanyCode varchar(20),   
 @ProductType varchar(15),
 @GenerateNoFrom    varchar(15),  
 @GenerateNoTo    varchar(15) ,
 @SourceData char(1)
as        

select 
a.BranchCode
,a.GenerateNo
,a.GenerateDate
,b.GenerateSeq
,a.SenderDealerCode
,a.SenderDealerName
,a.FpjNo
,case convert(varchar(10), a.FpjDate, 121) when '1900-01-01' then null else a.FpjDate end FpjDate
,a.RefferenceNo
,case convert(varchar(10), a.RefferenceDate, 121) when '1900-01-01' then null else a.RefferenceDate end RefferenceDate
,coalesce(b.SuzukiRefferenceNo, '') as SuzukiRefferenceNo
,coalesce(b.PaymentNo, '') as PaymentNo
,case convert(varchar(10), b.PaymentDate, 121) when '1900-01-01' then null else b.PaymentDate end PaymentDate
,coalesce(b.InvoiceNo,'') as SPKNo
,a.IsCampaign
,b.ServiceBookNo
,b.PdiFsc
,b.Odometer
,b.ServiceDate
,b.DeliveryDate
,b.RegisteredDate
,b.BasicModel
,b.TransmissionType
,b.ChassisCode
,b.ChassisNo
,b.EngineCode
,b.EngineNo
,b.LaborAmount
,b.MaterialAmount
,b.PdiFscAmount
,coalesce(b.LaborPaymentAmount, 0) as LaborPaymentAmount
,coalesce(b.MaterialPaymentAmount, 0) as MaterialPaymentAmount
,coalesce(b.PdiFscPaymentAmount, 0) as PdiFscPaymentAmount
,case a.SourceData
  when '0' then 'Internal Faktur Penjualan'
  when '1' then 'Manual Input'
  when '2' then 'Sub Dealer / Branches'
 end as SourceData
,b.Remarks
from svTrnPdiFsc a
left join svTrnPdiFscApplication b on b.CompanyCode = a.CompanyCode
 and b.BranchCode = a.BranchCode
 and b.ProductType = a.ProductType
 and b.GenerateNo = a.GenerateNo
where a.CompanyCode = @CompanyCode
  and a.ProductType = @ProductType
  and a.GenerateNo between @GenerateNoFrom and @GenerateNoTo
  and a.SourceData = @SourceData
order by a.GenerateNo, b.GenerateSeq
GO


