delete GnMstSendScheduleDms

declare @CompanyCode varchar(25);
set @CompanyCode = (
	select top 1
	       a.CompanyCode
	  from gnMstOrganizationHdr a
)



INSERT [dbo].[GnMstSendScheduleDms] ([CompanyCode], [DataType], [LastSendDate], [SegmentNo], [SPName], [Priority], [Status], [ColumnLastUpdate]) VALUES (@CompanyCode, N'CSBDAY', CAST(0x0000A286010E11F8 AS DateTime), 500, N'uspfn_CsCustomerBirthdaySend', 13, N'S', N'UpdatedDate')
INSERT [dbo].[GnMstSendScheduleDms] ([CompanyCode], [DataType], [LastSendDate], [SegmentNo], [SPName], [Priority], [Status], [ColumnLastUpdate]) VALUES (@CompanyCode, N'CSBKPB', CAST(0x0000A28500FE4FC1 AS DateTime), 500, N'uspfn_CsCustomerBPKBSend', 11, N'S', N'UpdatedDate')
INSERT [dbo].[GnMstSendScheduleDms] ([CompanyCode], [DataType], [LastSendDate], [SegmentNo], [SPName], [Priority], [Status], [ColumnLastUpdate]) VALUES (@CompanyCode, N'CSCUSTDATA', CAST(0x0000A26E00B7BF8A AS DateTime), 500, N'uspfn_CsCustomerDataSend', 14, N'S', N'UpdatedDate')
INSERT [dbo].[GnMstSendScheduleDms] ([CompanyCode], [DataType], [LastSendDate], [SegmentNo], [SPName], [Priority], [Status], [ColumnLastUpdate]) VALUES (@CompanyCode, N'CSFEEDBACK', CAST(0x0000A286010E996C AS DateTime), 500, N'uspfn_CsFeedbackSend', 12, N'S', N'UpdatedDate')
INSERT [dbo].[GnMstSendScheduleDms] ([CompanyCode], [DataType], [LastSendDate], [SegmentNo], [SPName], [Priority], [Status], [ColumnLastUpdate]) VALUES (@CompanyCode, N'CSHLDAY', CAST(0x0000A288009226C8 AS DateTime), 1000, N'uspfn_CsHolidaySend', 33, N'S', N'UpdatedDate')
INSERT [dbo].[GnMstSendScheduleDms] ([CompanyCode], [DataType], [LastSendDate], [SegmentNo], [SPName], [Priority], [Status], [ColumnLastUpdate]) VALUES (@CompanyCode, N'CSVHCL', CAST(0x0000A28800922934 AS DateTime), 1000, N'uspfn_CsCustomerVehicleSend', 39, N'S', N'UpdatedDate')
INSERT [dbo].[GnMstSendScheduleDms] ([CompanyCode], [DataType], [LastSendDate], [SegmentNo], [SPName], [Priority], [Status], [ColumnLastUpdate]) VALUES (@CompanyCode, N'CSRLTN', CAST(0x0000A2E500C9B328 AS DateTime), 1000, N'uspfn_CsRelationSend', 32, N'S', N'UpdatedDate')
INSERT [dbo].[GnMstSendScheduleDms] ([CompanyCode], [DataType], [LastSendDate], [SegmentNo], [SPName], [Priority], [Status], [ColumnLastUpdate]) VALUES (@CompanyCode, N'CSMSTHLDAY', CAST(0x0000A288009208A9 AS DateTime), 1000, N'uspfn_CsMstHolidaySend', 22, N'S', N'UpdatedDate')
INSERT [dbo].[GnMstSendScheduleDms] ([CompanyCode], [DataType], [LastSendDate], [SegmentNo], [SPName], [Priority], [Status], [ColumnLastUpdate]) VALUES (@CompanyCode, N'CSSTNKEXT', CAST(0x0000A288008EC4F3 AS DateTime), 1000, N'uspfn_CsSTNKEstension', 23, N'S', N'LastUpdatedDate')
INSERT [dbo].[GnMstSendScheduleDms] ([CompanyCode], [DataType], [LastSendDate], [SegmentNo], [SPName], [Priority], [Status], [ColumnLastUpdate]) VALUES (@CompanyCode, N'CS3DCALL', CAST(0x0000A2880092180A AS DateTime), 1000, N'uspfn_Cs3DayCallSend', 21, N'S', N'UpdatedDate')
INSERT [dbo].[GnMstSendScheduleDms] ([CompanyCode], [DataType], [LastSendDate], [SegmentNo], [SPName], [Priority], [Status], [ColumnLastUpdate]) VALUES (@CompanyCode, N'TRSLSINVIN', CAST(0x0000A2880092180A AS DateTime), 1000, N'uspfn_OmTrSalesInvoiceVinSend', 21, N'S', N'LastUpdateDate')
INSERT [dbo].[GnMstSendScheduleDms] ([CompanyCode], [DataType], [LastSendDate], [SegmentNo], [SPName], [Priority], [Status], [ColumnLastUpdate]) VALUES (@CompanyCode, N'TRSLSINV', CAST(0x0000A2880092180A AS DateTime), 1000, N'uspfn_OmTrSalesInvoiceSend', 21, N'S', N'LastUpdateDate')
INSERT [dbo].[GnMstSendScheduleDms] ([CompanyCode], [DataType], [LastSendDate], [SegmentNo], [SPName], [Priority], [Status], [ColumnLastUpdate]) VALUES (@CompanyCode, N'MSTCUST', CAST(0x0000A2880092180A AS DateTime), 1000, N'uspfn_CSMstCustomerSend', 21, N'S', N'LastUpdateDate')
INSERT [dbo].[GnMstSendScheduleDms] ([CompanyCode], [DataType], [LastSendDate], [SegmentNo], [SPName], [Priority], [Status], [ColumnLastUpdate]) VALUES (@CompanyCode, N'TRSLSDODTL', CAST(0x0000A2880092180A AS DateTime), 1000, N'uspfn_OmTrSalesDODetailSend', 21, N'S', N'LastUpdateDate')
INSERT [dbo].[GnMstSendScheduleDms] ([CompanyCode], [DataType], [LastSendDate], [SegmentNo], [SPName], [Priority], [Status], [ColumnLastUpdate]) VALUES (@CompanyCode, N'TRSLSDO', CAST(0x0000A2880092180A AS DateTime), 1000, N'uspfn_OmTrSalesDOSend', 21, N'S', N'LastUpdateDate')
INSERT [dbo].[GnMstSendScheduleDms] ([CompanyCode], [DataType], [LastSendDate], [SegmentNo], [SPName], [Priority], [Status], [ColumnLastUpdate]) VALUES (@CompanyCode, N'SVMSTCVHCL', CAST(0x0000A2880092180A AS DateTime), 1000, N'uspfn_SvMsVehicle', 21, N'S', N'LastUpdateDate')
INSERT [dbo].[GnMstSendScheduleDms] ([CompanyCode], [DataType], [LastSendDate], [SegmentNo], [SPName], [Priority], [Status], [ColumnLastUpdate]) VALUES (@CompanyCode, N'TRSLSBPK', CAST(0x0000A2880092180A AS DateTime), 1000, N'uspfn_OmTrSalesBPKSend', 21, N'S', N'LastUpdateDate')
INSERT [dbo].[GnMstSendScheduleDms] ([CompanyCode], [DataType], [LastSendDate], [SegmentNo], [SPName], [Priority], [Status], [ColumnLastUpdate]) VALUES (@CompanyCode, N'TRSLSSO', CAST(0x0000A2880092180A AS DateTime), 1000, N'uspfn_OmTrSalesSOSend', 21, N'S', N'LastUpdateDate')
GO


go
select * From GnMstSendScheduleDms order by Priority

select * from CsStnkExt
