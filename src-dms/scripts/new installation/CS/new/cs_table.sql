
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
 CONSTRAINT [PK_CsCustBirthDay] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[CustomerCode] ASC,
	[PeriodYear] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

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
 CONSTRAINT [PK_CsCustBpkb] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[CustomerCode] ASC,
	[Chassis] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

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
 CONSTRAINT [PK_CsStnkExt_1] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[CustomerCode] ASC,
	[Chassis] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

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
