
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspfn_HrListEmployeeSendEMPLY] (
	@LastUpdateDate datetime,
	@Segment int
)
as
begin
	select top (@Segment)
		a.*
	from
		HrEmployee a
	where
		a.UpdatedDate is not null
	order by 
		a.UpdatedDate asc
end

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[GnMstSendScheduleDms](
	[CompanyCode] [varchar](15) NOT NULL,
	[DataType] [varchar](10) NOT NULL,
	[LastSendDate] [datetime] NULL,
	[SegmentNo] [int] NULL,
	[SPName] [varchar](50) NULL,
	[Priority] [int] NULL,
	[Status] [char](1) NULL,
	[ColumnLastUpdate] [varchar](20) NULL,
 CONSTRAINT [pk_SendSheduleDms] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[DataType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

declare @CompanyCode varchar(25);
set @CompanyCode = (
	select top 1
           a.CompanyCode
	  from gnMstOrganizationHdr a
    
)


select @CompanyCode
INSERT [dbo].[GnMstSendScheduleDms] ([CompanyCode], [DataType], [LastSendDate], [SegmentNo], [SPName], [Priority], [Status], [ColumnLastUpdate]) VALUES (@CompanyCode, N'EMPLY', CAST(0x0000A26200820353 AS DateTime), 10, N'uspfn_HrListEmployeeSendEMPLY', 1, N'S', N'UpdatedDate')
GO


