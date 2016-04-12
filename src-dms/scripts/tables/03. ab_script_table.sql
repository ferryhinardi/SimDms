--DROP TABLE [HrDepartmentTraining]
--GO
--DROP TABLE [HrEmployee]
--GO
--DROP TABLE [HrEmployeeAchievement]
--GO
--DROP TABLE [HrEmployeeAdditionalBranch]
--GO
--DROP TABLE [HrEmployeeAdditionalJob]
--GO
--DROP TABLE [HrEmployeeEducation]
--GO
--DROP TABLE [HrEmployeeExperience]
--GO
--DROP TABLE [HrEmployeeMutation]
--GO
--DROP TABLE [HrEmployeeSales]
--GO
--DROP TABLE [HrEmployeeShift]
--GO
--ALTER TABLE [HrEmployeeTraining] DROP CONSTRAINT [DF_HrEmployeeTraining_TrainingSeq]
--GO
--DROP TABLE [HrEmployeeTraining]
--GO
--DROP TABLE [HrEmployeeVehicle]
--GO
--DROP TABLE [HrHoliday]
--GO
--DROP TABLE [HrMstTraining]
--GO
--DROP TABLE [HrSetting]
--GO
--DROP TABLE [HrShift]
--GO
--DROP TABLE [HrTrnAttendanceFileDtl]
--GO
--DROP TABLE [HrTrnAttendanceFileHdr]
--GO
--DROP TABLE [HrUploadedFile]
--GO


GO

/****** Object:  Table [dbo].[simDmsIterator]    Script Date: 2/17/2014 8:56:06 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[simDmsIterator](
	[AttendanceFlatFileExtractionProcessed] [int] NULL,
	[AttendanceFlatFileExtractionTotal] [nchar](10) NULL
) ON [PRIMARY]

GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [HrTrnAttendanceFileHdr](
	[CompanyCode] [varchar](20) NOT NULL,
	[GenerateId] [varchar](36) NOT NULL,
	[FileID] [varchar](200) NULL,
	[IsTransfered] [int] NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedBy] [varchar](20) NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_HrTrnAttendanceFileHdr] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[GenerateId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [HrTrnAttendanceFileDtl](
	[CompanyCode] [varchar](20) NOT NULL,
	[GenerateId] [varchar](36) NOT NULL,
	[SequenceNo] [int] NOT NULL,
	[FileID] [varchar](59) NULL,
	[EmployeeID] [varchar](10) NULL,
	[EmployeeName] [varchar](250) NULL,
	[AttendanceTime] [datetime] NULL,
	[MachineCode] [varchar](10) NULL,
	[IdentityCode] [varchar](10) NULL,
	[IsTransfered] [bit] NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedBy] [varchar](20) NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_HrTrnAttendanceFileDtl] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[GenerateId] ASC,
	[SequenceNo] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [HrShift](
	[CompanyCode] [varchar](20) NOT NULL,
	[ShiftCode] [varchar](20) NOT NULL,
	[ShiftName] [varchar](50) NULL,
	[OnDutyTime] [char](5) NULL,
	[OffDutyTime] [char](5) NULL,
	[OnRestTime] [char](5) NULL,
	[OffRestTime] [char](5) NULL,
	[WorkingHour] [int] NULL,
	[IsActive] [bit] NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedBy] [varchar](20) NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_HrShift] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[ShiftCode] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [HrSetting](
	[Name] [varchar](50) NOT NULL,
	[Value] [varchar](50) NULL,
 CONSTRAINT [PK_HrSetting] PRIMARY KEY CLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [HrMstTraining](
	[CompanyCode] [varchar](20) NOT NULL,
	[TrainingCode] [varchar](20) NOT NULL,
	[TrainingName] [varchar](150) NULL,
	[TrainingDescription] [varchar](500) NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedBy] [varchar](20) NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_HrMstTraining] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[TrainingCode] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [HrHoliday](
	[CompanyCode] [varchar](20) NOT NULL,
	[HolidayYear] [int] NOT NULL,
	[HolidayCode] [varchar](20) NOT NULL,
	[HolidayDesc] [varchar](150) NULL,
	[DateFrom] [datetime] NULL,
	[DateTo] [datetime] NULL,
	[IsHoliday] [bit] NULL,
	[ReligionCode] [varchar](20) NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedBy] [varchar](20) NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK__HrHoliday__38A52304] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[HolidayYear] ASC,
	[HolidayCode] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [HrEmployeeVehicle](
	[CompanyCode] [varchar](20) NOT NULL,
	[EmployeeID] [varchar](20) NOT NULL,
	[VehSeq] [int] NOT NULL,
	[Type] [varchar](20) NULL,
	[Brand] [varchar](20) NULL,
	[Model] [varchar](20) NULL,
	[PoliceRegNo] [varchar](20) NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedBy] [varchar](20) NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_HrEmployeeVehicle_1] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[EmployeeID] ASC,
	[VehSeq] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [HrEmployeeTraining](
	[CompanyCode] [varchar](20) NOT NULL,
	[EmployeeID] [varchar](20) NOT NULL,
	[TrainingCode] [varchar](20) NOT NULL,
	[TrainingDate] [datetime] NOT NULL,
	[TrainingSeq] [int] NULL CONSTRAINT [DF_HrEmployeeTraining_TrainingSeq]  DEFAULT ((0)),
	[TrainingDuration] [int] NULL,
	[PreTest] [int] NULL,
	[PreTestAlt] [varchar](10) NULL,
	[PostTest] [int] NULL,
	[PostTestAlt] [varchar](10) NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedBy] [varchar](20) NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_HrEmployeeTraining_1] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[EmployeeID] ASC,
	[TrainingCode] ASC,
	[TrainingDate] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [HrEmployeeShift](
	[CompanyCode] [varchar](20) NOT NULL,
	[EmployeeID] [varchar](20) NOT NULL,
	[AttdDate] [char](8) NOT NULL,
	[ShiftCode] [varchar](20) NOT NULL,
	[ClockInTime] [char](5) NULL,
	[ClockOutTime] [char](5) NULL,
	[OnDutyTime] [char](5) NULL,
	[OffDutyTime] [char](5) NULL,
	[OnRestTime] [char](5) NULL,
	[OffRestTime] [char](5) NULL,
	[CalcOvertime] [numeric](8, 0) NULL,
	[ApprOvertime] [numeric](8, 0) NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedBy] [varchar](20) NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_HrEmployeeShift_1] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[EmployeeID] ASC,
	[AttdDate] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [HrEmployeeSales](
	[CompanyCode] [varchar](20) NOT NULL,
	[EmployeeID] [varchar](20) NOT NULL,
	[SalesID] [varchar](50) NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[IsSyncronized] [bit] NULL,
	[IsTransfered] [bit] NULL,
	[SyncronizedDate] [datetime] NULL,
	[TransferedDate] [datetime] NULL,
 CONSTRAINT [PK_HrEmployeeSales] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[EmployeeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [HrEmployeeMutation](
	[CompanyCode] [varchar](20) NOT NULL,
	[EmployeeID] [varchar](20) NOT NULL,
	[MutationDate] [datetime] NOT NULL,
	[BranchCode] [varchar](20) NULL,
	[IsJoinDate] [bit] NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedBy] [varchar](20) NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_HrEmployeeMutation] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[EmployeeID] ASC,
	[MutationDate] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [HrEmployeeExperience](
	[CompanyCode] [varchar](20) NOT NULL,
	[EmployeeID] [varchar](20) NOT NULL,
	[ExpSeq] [int] NOT NULL,
	[NameOfCompany] [varchar](50) NULL,
	[JoinDate] [datetime] NULL,
	[ResignDate] [datetime] NULL,
	[ReasonOfResign] [varchar](500) NULL,
	[LeaderName] [varchar](50) NULL,
	[LeaderPhone] [varchar](50) NULL,
	[LeaderHP] [varchar](50) NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[UpdateBy] [varchar](20) NULL,
	[UpdateDate] [datetime] NULL,
 CONSTRAINT [PK_HrEmployeeExperience] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[EmployeeID] ASC,
	[ExpSeq] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [HrEmployeeEducation](
	[CompanyCode] [varchar](20) NOT NULL,
	[EmployeeID] [varchar](20) NOT NULL,
	[EduSeq] [int] NOT NULL,
	[College] [varchar](60) NULL,
	[Education] [varchar](60) NULL,
	[YearBegin] [varchar](5) NULL,
	[YearFinish] [varchar](5) NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[UpdateBy] [varchar](20) NULL,
	[UpdateDate] [datetime] NULL,
 CONSTRAINT [PK_HrEmployeeEducation] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[EmployeeID] ASC,
	[EduSeq] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [HrEmployeeAdditionalJob](
	[CompanyCode] [varchar](20) NOT NULL,
	[EmployeeID] [varchar](20) NOT NULL,
	[SeqNo] [int] NOT NULL,
	[Department] [varchar](50) NULL,
	[Position] [varchar](50) NULL,
	[Grade] [varchar](50) NULL,
	[AssignDate] [datetime] NULL,
	[ExpiredDate] [datetime] NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedBy] [varchar](20) NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_HrEmployeeAdditionalJob] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[EmployeeID] ASC,
	[SeqNo] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [HrEmployeeAdditionalBranch](
	[CompanyCode] [varchar](20) NOT NULL,
	[EmployeeID] [varchar](20) NOT NULL,
	[SeqNo] [int] NOT NULL,
	[BranchCode] [varchar](50) NULL,
	[AssignDate] [datetime] NULL,
	[ExpiredDate] [datetime] NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedBy] [varchar](20) NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_HrEmployeeAdditionalBranch] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[EmployeeID] ASC,
	[SeqNo] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [HrEmployeeAchievement](
	[CompanyCode] [varchar](20) NOT NULL,
	[EmployeeID] [varchar](20) NOT NULL,
	[AssignDate] [datetime] NOT NULL,
	[Department] [varchar](50) NULL,
	[Position] [varchar](50) NULL,
	[Grade] [varchar](50) NULL,
	[IsJoinDate] [bit] NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedBy] [varchar](20) NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_HrEmployeeAchievement] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[EmployeeID] ASC,
	[AssignDate] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [HrEmployee](
	[CompanyCode] [varchar](20) NOT NULL,
	[EmployeeID] [varchar](20) NOT NULL,
	[EmployeeName] [varchar](150) NULL,
	[Email] [varchar](50) NULL,
	[FaxNo] [varchar](50) NULL,
	[Handphone1] [varchar](50) NULL,
	[Handphone2] [varchar](50) NULL,
	[Handphone3] [varchar](50) NULL,
	[Handphone4] [varchar](50) NULL,
	[Telephone1] [varchar](50) NULL,
	[Telephone2] [varchar](50) NULL,
	[OfficeLocation] [varchar](250) NULL,
	[IsLinkedUser] [bit] NULL,
	[RelatedUser] [varchar](20) NULL,
	[JoinDate] [datetime] NULL,
	[Department] [varchar](50) NULL,
	[Position] [varchar](50) NULL,
	[Grade] [varchar](50) NULL,
	[Rank] [varchar](50) NULL,
	[Gender] [varchar](10) NULL,
	[TeamLeader] [varchar](20) NULL,
	[PersonnelStatus] [varchar](10) NULL,
	[ResignDate] [datetime] NULL,
	[ResignDescription] [varchar](500) NULL,
	[IdentityNo] [varchar](80) NULL,
	[NPWPNo] [varchar](50) NULL,
	[NPWPDate] [datetime] NULL,
	[BirthDate] [datetime] NULL,
	[BirthPlace] [varchar](80) NULL,
	[Address1] [varchar](150) NULL,
	[Address2] [varchar](150) NULL,
	[Address3] [varchar](150) NULL,
	[Address4] [varchar](150) NULL,
	[Province] [varchar](80) NULL,
	[District] [varchar](80) NULL,
	[SubDistrict] [varchar](80) NULL,
	[Village] [varchar](80) NULL,
	[ZipCode] [varchar](10) NULL,
	[DrivingLicense1] [varchar](50) NULL,
	[DrivingLicense2] [varchar](50) NULL,
	[MaritalStatus] [varchar](20) NULL,
	[MaritalStatusCode] [varchar](20) NULL,
	[Height] [numeric](6, 2) NULL,
	[Weight] [numeric](6, 2) NULL,
	[UniformSize] [numeric](6, 0) NULL,
	[UniformSizeAlt] [varchar](50) NULL,
	[ShoesSize] [numeric](6, 0) NULL,
	[FormalEducation] [varchar](20) NULL,
	[BloodCode] [varchar](10) NULL,
	[OtherInformation] [varchar](500) NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedBy] [varchar](20) NULL,
	[UpdatedDate] [datetime] NULL,
	[Religion] [varchar](10) NULL,
	[SelfPhoto] [varchar](80) NULL,
	[IdentityCardPhoto] [varchar](80) NULL,
 CONSTRAINT [PK_HrEmployee] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[EmployeeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [HrDepartmentTraining](
	[CompanyCode] [varchar](20) NOT NULL,
	[Department] [varchar](20) NOT NULL,
	[Position] [varchar](20) NOT NULL,
	[Grade] [varchar](20) NOT NULL,
	[TrainingCode] [varchar](20) NOT NULL,
	[IsRequired] [bit] NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedBy] [varchar](20) NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_HrPositionTraining] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[Department] ASC,
	[Position] ASC,
	[Grade] ASC,
	[TrainingCode] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO


CREATE TABLE [dbo].[HrLookupMapping](
	[CompanyCode] [varchar](50) NOT NULL,
	[CodeID] [varchar](20) NOT NULL,
	[CodeDescription] [varchar](350) NULL,
 CONSTRAINT [PK_HrLookupMapping] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[CodeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[HrUploadedFile](
	[Checksum] [varchar](59) NOT NULL,
	[FileType] [varchar](35) NOT NULL,
	[FileName] [varchar](100) NOT NULL,
	[FileSize] [int] NOT NULL,
	[Contents] [varbinary](max) NOT NULL,
	[UploadedBy] [nchar](10) NOT NULL,
	[UploadedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_HrUploadedFile] PRIMARY KEY CLUSTERED 
(
	[Checksum] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

GO

/****** Object:  Table [dbo].[HrEmployeeService]    Script Date: 10/28/2013 8:03:45 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[HrEmployeeService](
	[CompanyCode] [varchar](20) NOT NULL,
	[EmployeeID] [varchar](20) NOT NULL,
	[ServiceID] [varchar](50) NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[IsTransfered] [bit] NULL,
	[SyncronizedDate] [datetime] NULL,
	[TransferedDate] [datetime] NULL,
	[UpdatedBy] [varchar](20) NULL,
	[UpdatedDate] [datetime] NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_HrEmployeeService] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[EmployeeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO



alter table 
	HrEmployee
add
	ResignCategory varchar(25) 

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[HrEmployeeIDChangedHistory](
	[CompanyCode] [varchar](50) NOT NULL,
	[OldEmployeeID] [nchar](10) NOT NULL,
	[NewEmployeeID] [nchar](10) NOT NULL,
	[CreatedBy] [nchar](10) NOT NULL,
	[CreatedDate] [nchar](10) NOT NULL,
 CONSTRAINT [PK_HrEmployeeIDChangedHistory_1] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[OldEmployeeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING OFF
GO
CREATE TABLE [dbo].[gnMstPosition](
	[CompanyCode] [varchar](20) NOT NULL,
	[DeptCode] [varchar](20) NOT NULL,
	[PosCode] [varchar](20) NOT NULL,
	[PosName] [varchar](150) NULL,
	[PosHeader] [varchar](20) NULL,
	[PosLevel] [int] NULL,
	[StartDate] [datetime] NULL,
	[FinishDate] [datetime] NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedBy] [varchar](20) NULL,
	[UpdatedDate] [datetime] NULL,
	[TitleCode] [varchar](20) NULL,
	[IsGrade] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[DeptCode] ASC,
	[PosCode] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 70) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF