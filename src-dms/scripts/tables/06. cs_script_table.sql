/****** Object:  Table [dbo].[CsBpkbRetrievalInformation]    Script Date: 4/11/2014 8:33:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CsBpkbRetrievalInformation](
	[CompanyCode] [nchar](10) NOT NULL,
	[CustomerCode] [nchar](10) NOT NULL,
	[RetrievalEstimationDate] [datetime] NOT NULL,
	[Notes] [varchar](250) NULL,
	[CreatedBy] [varchar](50) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](50) NULL,
	[UpdatedDate] [datetime] NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_CsBpkbRetrievalInformation] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[CustomerCode] ASC,
	[RetrievalEstimationDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CsCustBirthDay]    Script Date: 4/11/2014 8:33:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CsCustBirthDay](
	[CompanyCode] [varchar](20) NOT NULL,
	[CustomerCode] [varchar](20) NOT NULL,
	[PeriodYear] [int] NOT NULL,
	[TypeOfGift] [varchar](25) NOT NULL,
	[SentGiftDate] [datetime] NULL,
	[ReceivedGiftDate] [datetime] NULL,
	[Comment] [varchar](max) NULL,
	[AdditionalInquiries] [varchar](max) NULL,
	[Status] [bit] NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedBy] [varchar](20) NULL,
	[UpdatedDate] [datetime] NULL,
	[SpouseName] [varchar](100) NULL,
	[ChildName1] [varchar](100) NULL,
	[ChildName2] [varchar](100) NULL,
	[ChildName3] [varchar](100) NULL,
	[SpouseBirthday] [datetime] NULL,
	[ChildBirthday1] [datetime] NULL,
	[ChildBirthday2] [datetime] NULL,
	[ChildBirthday3] [datetime] NULL,
	[SpouseComment] [varchar](500) NULL,
	[ChildComment1] [varchar](500) NULL,
	[ChildComment2] [varchar](500) NULL,
	[ChildComment3] [varchar](500) NULL,
	[SpouseGiftSentDate] [datetime] NULL,
	[ChildGiftSentDate1] [datetime] NULL,
	[ChildGiftSentDate2] [datetime] NULL,
	[ChildGiftSentDate3] [datetime] NULL,
	[SpouseTypeOfGift] [varchar](100) NULL,
	[ChildTypeOfGift1] [varchar](100) NULL,
	[ChildTypeOfGift2] [varchar](100) NULL,
	[ChildTypeOfGift3] [varchar](100) NULL,
	[SpouseTelephone] [varchar](50) NULL,
	[ChildGiftReceivedDate1] [datetime] NULL,
	[ChildGiftReceivedDate2] [datetime] NULL,
	[ChildGiftReceivedDate3] [datetime] NULL,
	[SpouseGiftReceivedDate] [datetime] NULL,
	[SpouseRelation] [varchar](100) NULL,
	[ChildRelation1] [varchar](100) NULL,
	[ChildRelation2] [varchar](100) NULL,
	[ChildRelation3] [varchar](100) NULL,
	[ChildTelephone1] [varchar](100) NULL,
	[ChildTelephone2] [varchar](100) NULL,
	[ChildTelephone3] [varchar](100) NULL,
 CONSTRAINT [PK_CsCustBirthDay] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[CustomerCode] ASC,
	[PeriodYear] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CsCustBpkb]    Script Date: 4/11/2014 8:33:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CsCustBpkb](
	[CompanyCode] [varchar](50) NOT NULL,
	[CustomerCode] [varchar](20) NOT NULL,
	[Chassis] [varchar](50) NOT NULL,
	[BpkbReadyDate] [datetime] NULL,
	[BpkbPickUp] [datetime] NULL,
	[ReqInfoLeasing] [bit] NULL,
	[ReqInfoCust] [bit] NULL,
	[ReqKtp] [bit] NULL,
	[ReqStnk] [bit] NULL,
	[ReqSuratKuasa] [bit] NULL,
	[Comment] [varchar](250) NULL,
	[Additional] [varchar](250) NULL,
	[Status] [int] NULL,
	[FinishDate] [datetime] NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedBy] [varchar](20) NULL,
	[UpdatedDate] [datetime] NULL,
	[LeasingCode] [varchar](17) NULL,
	[Tenor] [int] NULL,
	[CustomerCategory] [varchar](17) NULL,
	[IsDeleted] [bit] NULL,
	[IsStnkExt] [bit] NULL,
 CONSTRAINT [PK_CsCustBpkb] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[CustomerCode] ASC,
	[Chassis] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CsCustData]    Script Date: 4/11/2014 8:33:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CsCustData](
	[CompanyCode] [varchar](20) NOT NULL,
	[CustomerCode] [varchar](20) NOT NULL,
	[AddPhone1] [varchar](20) NULL,
	[AddPhone2] [varchar](20) NULL,
	[ReligionCode] [varchar](20) NULL,
	[IsDeleted] [bit] NULL,
	[CreatedBy] [varchar](36) NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedBy] [varchar](36) NULL,
	[UpdatedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[CustomerCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CsCustFeedback]    Script Date: 4/11/2014 8:33:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CsCustFeedback](
	[CompanyCode] [varchar](20) NOT NULL,
	[CustomerCode] [varchar](20) NOT NULL,
	[Chassis] [varchar](50) NOT NULL,
	[IsManual] [bit] NULL,
	[FeedbackA] [varchar](max) NULL,
	[FeedbackB] [varchar](max) NULL,
	[FeedbackC] [varchar](max) NULL,
	[FeedbackD] [varchar](max) NULL,
	[CreatedBy] [varchar](36) NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedBy] [varchar](36) NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_CsCustFeedback] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[CustomerCode] ASC,
	[Chassis] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CsCustHoliday]    Script Date: 4/11/2014 8:33:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CsCustHoliday](
	[CompanyCode] [varchar](20) NOT NULL,
	[CustomerCode] [varchar](20) NOT NULL,
	[PeriodYear] [int] NOT NULL,
	[GiftSeq] [int] NOT NULL,
	[ReligionCode] [varchar](20) NULL,
	[HolidayCode] [varchar](20) NULL,
	[IsGiftCard] [bit] NULL,
	[IsGiftLetter] [bit] NULL,
	[IsGiftSms] [bit] NULL,
	[IsGiftSouvenir] [bit] NULL,
	[SouvenirSent] [datetime] NULL,
	[SouvenirReceived] [datetime] NULL,
	[Comment] [varchar](250) NULL,
	[Additional] [varchar](250) NULL,
	[Status] [int] NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedBy] [varchar](20) NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_CsHoliday_1] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[CustomerCode] ASC,
	[PeriodYear] ASC,
	[GiftSeq] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CsCustomerVehicle]    Script Date: 4/11/2014 8:33:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CsCustomerVehicle](
	[CompanyCode] [varchar](20) NOT NULL,
	[CustomerCode] [varchar](25) NOT NULL,
	[Chassis] [varchar](50) NOT NULL,
	[StnkDate] [datetime] NULL,
	[BpkbDate] [datetime] NULL,
	[CreatedBy] [varchar](20) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](20) NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_CsCustomerVehicle] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[CustomerCode] ASC,
	[Chassis] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CsCustomerVehicleView]    Script Date: 4/11/2014 8:33:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CsCustomerVehicleView](
	[CompanyCode] [varchar](20) NOT NULL,
	[BranchCode] [varchar](20) NOT NULL,
	[CustomerCode] [varchar](20) NOT NULL,
	[Chassis] [varchar](80) NOT NULL,
	[Engine] [varchar](80) NOT NULL,
	[SONo] [varchar](20) NOT NULL,
	[DONo] [varchar](20) NOT NULL,
	[BpkNo] [varchar](20) NOT NULL,
	[CarType] [varchar](20) NULL,
	[Color] [varchar](20) NULL,
	[SalesmanCode] [varchar](20) NULL,
	[SalesmanName] [varchar](150) NULL,
	[PoliceRegNo] [varchar](20) NULL,
	[DeliveryDate] [datetime] NULL,
	[SalesModelCode] [varchar](20) NULL,
	[SalesModelYear] [int] NULL,
	[ColourCode] [varchar](20) NULL,
	[BpkDate] [datetime] NULL,
	[IsLeasing] [bit] NULL,
	[LeasingCo] [varchar](20) NULL,
	[LeasingName] [varchar](250) NULL,
	[Installment] [int] NULL,
 CONSTRAINT [PK_CsCustomerVehicleView] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[BranchCode] ASC,
	[CustomerCode] ASC,
	[Chassis] ASC,
	[Engine] ASC,
	[SONo] ASC,
	[DONo] ASC,
	[BpkNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CsCustomerView]    Script Date: 4/11/2014 8:33:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CsCustomerView](
	[CompanyCode] [varchar](15) NOT NULL,
	[BranchCode] [varchar](15) NOT NULL,
	[CustomerCode] [varchar](15) NOT NULL,
	[CustomerName] [varchar](100) NULL,
	[CustomerType] [varchar](20) NULL,
	[Address] [varchar](301) NULL,
	[PhoneNo] [varchar](15) NULL,
	[HPNo] [varchar](15) NULL,
	[AddPhone1] [varchar](20) NULL,
	[AddPhone2] [varchar](20) NULL,
	[BirthDate] [datetime] NULL,
	[ReligionCode] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[LastUpdateDate] [datetime] NULL,
 CONSTRAINT [PK_CsCustomerView] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[BranchCode] ASC,
	[CustomerCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CsCustRelation]    Script Date: 4/11/2014 8:33:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CsCustRelation](
	[CompanyCode] [varchar](50) NOT NULL,
	[CustomerCode] [varchar](20) NOT NULL,
	[RelationType] [varchar](20) NOT NULL,
	[FullName] [varchar](50) NULL,
	[PhoneNo] [varchar](20) NULL,
	[RelationInfo] [varchar](150) NULL,
	[BirthDate] [datetime] NULL,
	[TypeOfGift] [varchar](30) NULL,
	[SentGiftDate] [datetime] NULL,
	[ReceivedGiftDate] [datetime] NULL,
	[Comment] [varchar](max) NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedBy] [varchar](20) NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_CsCustRelation] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[CustomerCode] ASC,
	[RelationType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CsDealer]    Script Date: 4/11/2014 8:33:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CsDealer](
	[CompanyCode] [varchar](20) NOT NULL,
	[IsActive] [bit] NULL,
 CONSTRAINT [PK_CsDealer] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CsMstHoliday]    Script Date: 4/11/2014 8:33:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CsMstHoliday](
	[CompanyCode] [varchar](20) NOT NULL,
	[HolidayYear] [int] NOT NULL,
	[HolidayCode] [varchar](20) NOT NULL,
	[HolidayDesc] [varchar](150) NULL,
	[DateFrom] [datetime] NULL,
	[DateTo] [datetime] NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedBy] [varchar](20) NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_CsMstHoliday] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[HolidayYear] ASC,
	[HolidayCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CsSettings]    Script Date: 4/11/2014 8:33:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CsSettings](
	[CompanyCode] [varchar](20) NOT NULL,
	[SettingCode] [varchar](20) NOT NULL,
	[SettingDesc] [varchar](250) NULL,
	[SettingParam1] [varchar](20) NULL,
	[SettingParam2] [varchar](20) NULL,
	[SettingParam3] [varchar](20) NULL,
	[SettingParam4] [varchar](20) NULL,
	[SettingParam5] [varchar](20) NULL,
	[SettingLink1] [varchar](20) NULL,
	[SettingLink2] [varchar](20) NULL,
	[SettingLink3] [varchar](20) NULL,
	[IsDeleted] [bit] NULL,
	[CreatedBy] [varchar](36) NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedBy] [varchar](36) NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_CsSettings] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[SettingCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CsStnkExt]    Script Date: 4/11/2014 8:33:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CsStnkExt](
	[CompanyCode] [varchar](50) NOT NULL,
	[CustomerCode] [varchar](20) NOT NULL,
	[Chassis] [varchar](50) NOT NULL,
	[IsStnkExtend] [bit] NULL,
	[StnkExpiredDate] [datetime] NULL,
	[ReqKtp] [bit] NULL,
	[ReqStnk] [bit] NULL,
	[ReqBpkb] [bit] NULL,
	[ReqSuratKuasa] [bit] NULL,
	[Comment] [varchar](250) NULL,
	[Additional] [varchar](250) NULL,
	[Status] [int] NULL,
	[FinishDate] [datetime] NULL,
	[CreatedBy] [varchar](25) NULL,
	[CreatedDate] [datetime] NULL,
	[LastUpdatedBy] [varchar](25) NULL,
	[LastUpdatedDate] [datetime] NULL,
	[Tenor] [int] NULL,
	[LeasingCode] [varchar](17) NULL,
	[CustomerCategory] [varchar](17) NULL,
	[Ownership] [bit] NULL,
	[StnkReadyDate] [datetime] NULL,
	[StnkPickUp] [datetime] NULL,
	[StnkFee] [numeric](18, 2) NULL,
	[StnkDate] [datetime] NULL,
	[BpkbDate] [datetime] NULL,
 CONSTRAINT [PK_CsStnkExt_1] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[CustomerCode] ASC,
	[Chassis] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CsTDayCall]    Script Date: 4/11/2014 8:33:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CsTDayCall](
	[CompanyCode] [varchar](20) NOT NULL,
	[CustomerCode] [varchar](20) NOT NULL,
	[Chassis] [varchar](50) NOT NULL,
	[IsDeliveredA] [bit] NULL,
	[IsDeliveredB] [bit] NULL,
	[IsDeliveredC] [bit] NULL,
	[IsDeliveredD] [bit] NULL,
	[IsDeliveredE] [bit] NULL,
	[IsDeliveredF] [bit] NULL,
	[IsDeliveredG] [bit] NULL,
	[Comment] [varchar](250) NULL,
	[Additional] [varchar](250) NULL,
	[Status] [int] NULL,
	[FinishDate] [datetime] NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedBy] [varchar](20) NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_CsTDayCall_1] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[CustomerCode] ASC,
	[Chassis] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
