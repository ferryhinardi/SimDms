
declare @CompanyCode varchar(25);
set @CompanyCode = ( select top 1 CompanyCode from gnMstOrganizationHdr );





delete gnMstOrgGroup;

--INSERT [dbo].[gnMstOrgGroup] ([CompanyCode], [OrgGroupCode], [OrgCode], [OrgName], [OrgHeader], [OrgSeq], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (@CompanyCode, N'DEPT', N'COM', N'BIT GROUP', N'COM', 0, NULL, NULL, N'ga', CAST(0x0000A2650088460A AS DateTime), N'walmi', CAST(0x0000A26600A2657C AS DateTime))
INSERT [dbo].[gnMstOrgGroup] ([CompanyCode], [OrgGroupCode], [OrgCode], [OrgName], [OrgHeader], [OrgSeq], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (@CompanyCode, N'DEPT', N'FAD', N'FAD', N'COM', 10, NULL, NULL, NULL, NULL, N'ga', CAST(0x0000A26500885F9A AS DateTime))
INSERT [dbo].[gnMstOrgGroup] ([CompanyCode], [OrgGroupCode], [OrgCode], [OrgName], [OrgHeader], [OrgSeq], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (@CompanyCode, N'DEPT', N'HRD-GA', N'HRD-GA', N'COM', 10, NULL, NULL, NULL, NULL, N'ga', CAST(0x0000A26500886494 AS DateTime))
INSERT [dbo].[gnMstOrgGroup] ([CompanyCode], [OrgGroupCode], [OrgCode], [OrgName], [OrgHeader], [OrgSeq], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (@CompanyCode, N'DEPT', N'IT', N'IT', N'COM', 10, NULL, NULL, N'ga', CAST(0x0000A26200963062 AS DateTime), N'ga', CAST(0x0000A26A00ADA2C9 AS DateTime))
INSERT [dbo].[gnMstOrgGroup] ([CompanyCode], [OrgGroupCode], [OrgCode], [OrgName], [OrgHeader], [OrgSeq], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (@CompanyCode, N'DEPT', N'PART & ACC', N'PARTS & ACCESSORIES', N'COM', 10, NULL, NULL, N'walmi', CAST(0x0000A26500F78ED5 AS DateTime), N'walmi', CAST(0x0000A26500FB5560 AS DateTime))
INSERT [dbo].[gnMstOrgGroup] ([CompanyCode], [OrgGroupCode], [OrgCode], [OrgName], [OrgHeader], [OrgSeq], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (@CompanyCode, N'DEPT', N'PDC', N'PDC', N'COM', 10, NULL, NULL, NULL, NULL, N'ga', CAST(0x0000A265008871B9 AS DateTime))
INSERT [dbo].[gnMstOrgGroup] ([CompanyCode], [OrgGroupCode], [OrgCode], [OrgName], [OrgHeader], [OrgSeq], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (@CompanyCode, N'DEPT', N'SALES', N'SALES', N'COM', 10, NULL, NULL, NULL, NULL, N'ga', CAST(0x0000A26A00ADA906 AS DateTime))
INSERT [dbo].[gnMstOrgGroup] ([CompanyCode], [OrgGroupCode], [OrgCode], [OrgName], [OrgHeader], [OrgSeq], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (@CompanyCode, N'DEPT', N'SALES & MKT', N'SALES & MARKETING', N'COM', 10, NULL, NULL, N'walmi', CAST(0x0000A26500F7D585 AS DateTime), N'walmi', CAST(0x0000A26500FB635F AS DateTime))
INSERT [dbo].[gnMstOrgGroup] ([CompanyCode], [OrgGroupCode], [OrgCode], [OrgName], [OrgHeader], [OrgSeq], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (@CompanyCode, N'DEPT', N'SERVICE', N'SERVICE', N'COM', 10, NULL, NULL, N'ga', CAST(0x0000A2620098E9BD AS DateTime), N'ga', CAST(0x0000A26500887DF7 AS DateTime))
INSERT [dbo].[gnMstOrgGroup] ([CompanyCode], [OrgGroupCode], [OrgCode], [OrgName], [OrgHeader], [OrgSeq], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (@CompanyCode, N'RANK', N'AM', N'ASST MANAGER', N'M', 50, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[gnMstOrgGroup] ([CompanyCode], [OrgGroupCode], [OrgCode], [OrgName], [OrgHeader], [OrgSeq], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (@CompanyCode, N'RANK', N'F', N'FOREMAN', N'SF', 20, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[gnMstOrgGroup] ([CompanyCode], [OrgGroupCode], [OrgCode], [OrgName], [OrgHeader], [OrgSeq], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (@CompanyCode, N'RANK', N'M', N'MANAGER', N'', 60, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[gnMstOrgGroup] ([CompanyCode], [OrgGroupCode], [OrgCode], [OrgName], [OrgHeader], [OrgSeq], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (@CompanyCode, N'RANK', N'S', N'STAFF', N'F', 10, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[gnMstOrgGroup] ([CompanyCode], [OrgGroupCode], [OrgCode], [OrgName], [OrgHeader], [OrgSeq], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (@CompanyCode, N'RANK', N'SF', N'SENIOR FOREMAN', N'SPV', 30, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[gnMstOrgGroup] ([CompanyCode], [OrgGroupCode], [OrgCode], [OrgName], [OrgHeader], [OrgSeq], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (@CompanyCode, N'RANK', N'SPV', N'SUPERVISOR', N'AM', 40, NULL, NULL, NULL, NULL, NULL, NULL)

