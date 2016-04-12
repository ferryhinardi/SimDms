

declare @CompanyCode varchar(25);
set @CompanyCode = (
	select top 1
	       a.CompanyCode
	  from gnMstOrganizationHdr a
)



INSERT [dbo].[GnMstSendScheduleDms] ([CompanyCode], [DataType], [LastSendDate], [SegmentNo], [SPName], [Priority], [Status], [ColumnLastUpdate]) VALUES (@CompanyCode, N'EMACH', CAST(0x0000A286010E11F8 AS DateTime), 500, N'uspfn_HrListEmployeeAchieveSend', 13, N'S', N'UpdatedDate')
INSERT [dbo].[GnMstSendScheduleDms] ([CompanyCode], [DataType], [LastSendDate], [SegmentNo], [SPName], [Priority], [Status], [ColumnLastUpdate]) VALUES (@CompanyCode, N'EMPLY', CAST(0x0000A28500FE4FC1 AS DateTime), 500, N'uspfn_HrListEmployeeSend', 11, N'S', N'UpdatedDate')
INSERT [dbo].[GnMstSendScheduleDms] ([CompanyCode], [DataType], [LastSendDate], [SegmentNo], [SPName], [Priority], [Status], [ColumnLastUpdate]) VALUES (@CompanyCode, N'EMSFM', CAST(0x0000A26E00B7BF8A AS DateTime), 500, N'uspfn_HrListEmployeeSalesSend', 14, N'S', N'CreatedDate')
INSERT [dbo].[GnMstSendScheduleDms] ([CompanyCode], [DataType], [LastSendDate], [SegmentNo], [SPName], [Priority], [Status], [ColumnLastUpdate]) VALUES (@CompanyCode, N'EMUTA', CAST(0x0000A286010E996C AS DateTime), 500, N'uspfn_HrListEmployeeMutationSend', 12, N'S', N'UpdatedDate')
INSERT [dbo].[GnMstSendScheduleDms] ([CompanyCode], [DataType], [LastSendDate], [SegmentNo], [SPName], [Priority], [Status], [ColumnLastUpdate]) VALUES (@CompanyCode, N'PMACT', CAST(0x0000A288009226C8 AS DateTime), 1000, N'uspfn_PmActivities', 33, N'S', N'LastUpdateDate')
INSERT [dbo].[GnMstSendScheduleDms] ([CompanyCode], [DataType], [LastSendDate], [SegmentNo], [SPName], [Priority], [Status], [ColumnLastUpdate]) VALUES (@CompanyCode, N'PMKDP', CAST(0x0000A28800922934 AS DateTime), 1000, N'uspfn_PmKdpSend', 39, N'S', N'LastUpdateDate')
INSERT [dbo].[GnMstSendScheduleDms] ([CompanyCode], [DataType], [LastSendDate], [SegmentNo], [SPName], [Priority], [Status], [ColumnLastUpdate]) VALUES (@CompanyCode, N'PMSHS', CAST(0x0000A2E500C9B328 AS DateTime), 1000, N'uspfn_PmStatusHistory', 32, N'S', N'UpdateDate')
INSERT [dbo].[GnMstSendScheduleDms] ([CompanyCode], [DataType], [LastSendDate], [SegmentNo], [SPName], [Priority], [Status], [ColumnLastUpdate]) VALUES (@CompanyCode, N'SVINV', CAST(0x0000A288009208A9 AS DateTime), 1000, N'uspfn_SvInvoiceSend', 22, N'S', N'LastUpdateDate')
INSERT [dbo].[GnMstSendScheduleDms] ([CompanyCode], [DataType], [LastSendDate], [SegmentNo], [SPName], [Priority], [Status], [ColumnLastUpdate]) VALUES (@CompanyCode, N'SVMSI', CAST(0x0000A288008EC4F3 AS DateTime), 1000, N'uspfn_SvMsiSend', 23, N'S', N'CreatedDate')
INSERT [dbo].[GnMstSendScheduleDms] ([CompanyCode], [DataType], [LastSendDate], [SegmentNo], [SPName], [Priority], [Status], [ColumnLastUpdate]) VALUES (@CompanyCode, N'SVSPK', CAST(0x0000A2880092180A AS DateTime), 1000, N'uspfn_SvJobOrderSend', 21, N'S', N'LastUpdateDate')
GO
