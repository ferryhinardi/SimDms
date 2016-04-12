

declare @CompanyCode varchar(25);
set @CompanyCode = ( select top 1 CompanyCode from gnMstOrganizationHdr );


delete HrShift;


INSERT [dbo].[HrShift] ([CompanyCode], [ShiftCode], [ShiftName], [OnDutyTime], [OffDutyTime], [OnRestTime], [OffRestTime], [WorkingHour], [IsActive], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (@CompanyCode, N'SHIFT01', N'SHIFT PAGI', N'08:00', N'17:00', N'12:00', N'13:00', 480, 1, N'ga', CAST(0x0000A25E00E85F16 AS DateTime), N'ga', CAST(0x0000A25E00EB40AC AS DateTime))
INSERT [dbo].[HrShift] ([CompanyCode], [ShiftCode], [ShiftName], [OnDutyTime], [OffDutyTime], [OnRestTime], [OffRestTime], [WorkingHour], [IsActive], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (@CompanyCode, N'SHIFT02', N'SHIFT SIANG', N'17:00', N'02:00', N'20:00', N'21:00', 480, 1, N'ga', CAST(0x0000A24D00F58C2E AS DateTime), N'ga', CAST(0x0000A25E00E8423E AS DateTime))
INSERT [dbo].[HrShift] ([CompanyCode], [ShiftCode], [ShiftName], [OnDutyTime], [OffDutyTime], [OnRestTime], [OffRestTime], [WorkingHour], [IsActive], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (@CompanyCode, N'SHIFT03', N'SHIFT MALAM', N'02:00', N'08:00', N'04:30', N'05:30', 300, 1, N'ga', CAST(0x0000A25E00E8E268 AS DateTime), N'ga', CAST(0x0000A25E00E8ED19 AS DateTime))
