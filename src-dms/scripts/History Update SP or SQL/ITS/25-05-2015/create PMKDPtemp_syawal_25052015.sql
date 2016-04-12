CREATE TABLE [dbo].[pmKDPTemp](
	[InquiryNumber] [int] NOT NULL,
	[BranchCode] [varchar](15) NOT NULL,
	[CompanyCode] [varchar](15) NOT NULL,
	[EmployeeID] [varchar](15) NOT NULL,
	[SpvEmployeeID] [varchar](15) NOT NULL,
	[InquiryDate] [datetime] NOT NULL,
	[OutletID] [varchar](15) NOT NULL,
	[StatusProspek] [varchar](2) NULL,
	[PerolehanData] [varchar](15) NOT NULL,
	[NamaProspek] [varchar](30) NOT NULL,
	[AlamatProspek] [varchar](200) NULL,
	[TelpRumah] [varchar](15) NULL,
	[CityID] [varchar](15) NULL,
	[NamaPerusahaan] [varchar](50) NULL,
	[AlamatPerusahaan] [varchar](200) NULL,
	[Jabatan] [varchar](30) NULL,
	[Handphone] [varchar](15) NULL,
	[Faximile] [varchar](15) NULL,
	[Email] [varchar](50) NULL,
	[TipeKendaraan] [varchar](20) NULL,
	[Variant] [varchar](50) NULL,
	[Transmisi] [varchar](2) NULL,
	[ColourCode] [varchar](3) NULL,
	[CaraPembayaran] [varchar](2) NULL,
	[TestDrive] [varchar](2) NULL,
	[QuantityInquiry] [int] NULL,
	[LastProgress] [varchar](15) NOT NULL,
	[LastUpdateStatus] [datetime] NULL,
	[SPKDate] [datetime] NULL,
	[LostCaseDate] [datetime] NULL,
	[LostCaseCategory] [varchar](1) NULL,
	[LostCaseReasonID] [varchar](2) NULL,
	[LostCaseOtherReason] [varchar](100) NULL,
	[LostCaseVoiceOfCustomer] [varchar](200) NULL,
	[CreationDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](15) NOT NULL,
	[LastUpdateBy] [varchar](15) NOT NULL,
	[LastUpdateDate] [datetime] NOT NULL,
	[Leasing] [varchar](15) NOT NULL,
	[DownPayment] [varchar](15) NOT NULL,
	[Tenor] [varchar](15) NOT NULL,
	[MerkLain] [varchar](50) NOT NULL,
 CONSTRAINT [PKtemp] PRIMARY KEY NONCLUSTERED 
(
	[InquiryNumber] ASC,
	[CompanyCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 70) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

insert pmKDPTemp
select top 1 * from pmkdp order by InquiryNumber desc 


