/****** Object:  Table [dbo].[SysModule]    Script Date: 10/23/2013 16:10:17 ******/
DELETE [dbo].[SysModule] 
INSERT [dbo].[SysModule] ([ModuleId], [ModuleCaption], [ModuleIndex], [ModuleUrl], [InternalLink], [IsPublish]) VALUES (N'gn', N'GN - General', 0, N'', 1, 1)
INSERT [dbo].[SysModule] ([ModuleId], [ModuleCaption], [ModuleIndex], [ModuleUrl], [InternalLink], [IsPublish]) VALUES (N'sv', N'SV - Service', 3, N'', 1, 0)
INSERT [dbo].[SysModule] ([ModuleId], [ModuleCaption], [ModuleIndex], [ModuleUrl], [InternalLink], [IsPublish]) VALUES (N'its', N'ITS - Inquiry Tracking System', 4, N'', 1, 0)
INSERT [dbo].[SysModule] ([ModuleId], [ModuleCaption], [ModuleIndex], [ModuleUrl], [InternalLink], [IsPublish]) VALUES (N'cs', N'CS - Customer Statisfaction', 6, N'', 1, 0)
INSERT [dbo].[SysModule] ([ModuleId], [ModuleCaption], [ModuleIndex], [ModuleUrl], [InternalLink], [IsPublish]) VALUES (N'ab', N'AB - Absence', 7, N'', 1, 1)

/****** Object:  Table [dbo].[SysMenuDms]    Script Date: 10/23/2013 16:10:17 ******/
DELETE [dbo].[SysMenuDms] WHERE MenuHeader in ('gn','gnmember','ab','abmaster','abempl','abtrans','abreport')

-- MENU GENERAL
INSERT [dbo].[SysMenuDms] ([MenuId], [MenuCaption], [MenuHeader], [MenuIndex], [MenuLevel], [MenuUrl]) VALUES (N'gnmember', N'User Akses', N'gn', 1, 1, N'')

INSERT [dbo].[SysMenuDms] ([MenuId], [MenuCaption], [MenuHeader], [MenuIndex], [MenuLevel], [MenuUrl]) VALUES (N'gnuser', N'User', N'gnmember', 1, 2, N'member/user')
INSERT [dbo].[SysMenuDms] ([MenuId], [MenuCaption], [MenuHeader], [MenuIndex], [MenuLevel], [MenuUrl]) VALUES (N'gnrole', N'Role', N'gnmember', 2, 2, N'member/role')
INSERT [dbo].[SysMenuDms] ([MenuId], [MenuCaption], [MenuHeader], [MenuIndex], [MenuLevel], [MenuUrl]) VALUES (N'gnmenu', N'Menu', N'gnmember', 3, 2, N'member/menu')
INSERT [dbo].[SysMenuDms] ([MenuId], [MenuCaption], [MenuHeader], [MenuIndex], [MenuLevel], [MenuUrl]) VALUES (N'gnrmnu', N'Role Menu', N'gnmember', 4, 2, N'member/rolemenu')

-- MENU HR / ATTENDANCE
INSERT [dbo].[SysMenuDms] ([MenuId], [MenuCaption], [MenuHeader], [MenuIndex], [MenuLevel], [MenuUrl]) VALUES (N'abmaster', N'Master', N'ab', 1, 1, N'')
INSERT [dbo].[SysMenuDms] ([MenuId], [MenuCaption], [MenuHeader], [MenuIndex], [MenuLevel], [MenuUrl]) VALUES (N'abempl', N'Employee', N'ab', 2, 1, N'')
INSERT [dbo].[SysMenuDms] ([MenuId], [MenuCaption], [MenuHeader], [MenuIndex], [MenuLevel], [MenuUrl]) VALUES (N'abtrans', N'Trancation', N'ab', 3, 1, N'')
INSERT [dbo].[SysMenuDms] ([MenuId], [MenuCaption], [MenuHeader], [MenuIndex], [MenuLevel], [MenuUrl]) VALUES (N'abreport', N'Report', N'ab', 4, 1, N'')

INSERT [dbo].[SysMenuDms] ([MenuId], [MenuCaption], [MenuHeader], [MenuIndex], [MenuLevel], [MenuUrl]) VALUES (N'ablookup', N'Master Lookup', N'abmaster', 1, 2, N'master/lookup')
INSERT [dbo].[SysMenuDms] ([MenuId], [MenuCaption], [MenuHeader], [MenuIndex], [MenuLevel], [MenuUrl]) VALUES (N'abholiday', N'Master Holiday', N'abmaster', 2, 2, N'master/holiday')
INSERT [dbo].[SysMenuDms] ([MenuId], [MenuCaption], [MenuHeader], [MenuIndex], [MenuLevel], [MenuUrl]) VALUES (N'abshift', N'Master Shift', N'abmaster', 3, 2, N'master/shift')
INSERT [dbo].[SysMenuDms] ([MenuId], [MenuCaption], [MenuHeader], [MenuIndex], [MenuLevel], [MenuUrl]) VALUES (N'abdept', N'Master Department', N'abmaster', 4, 2, N'master/dept')
INSERT [dbo].[SysMenuDms] ([MenuId], [MenuCaption], [MenuHeader], [MenuIndex], [MenuLevel], [MenuUrl]) VALUES (N'abposition', N'Master Position', N'abmaster', 5, 2, N'master/position')

INSERT [dbo].[SysMenuDms] ([MenuId], [MenuCaption], [MenuHeader], [MenuIndex], [MenuLevel], [MenuUrl]) VALUES (N'abpersinfo', N'Personal Information', N'abempl', 1, 1, N'empl/persinfo')
INSERT [dbo].[SysMenuDms] ([MenuId], [MenuCaption], [MenuHeader], [MenuIndex], [MenuLevel], [MenuUrl]) VALUES (N'absalesinfo', N'Sales Information', N'abempl', 2, 1, N'empl/salesinfo')
INSERT [dbo].[SysMenuDms] ([MenuId], [MenuCaption], [MenuHeader], [MenuIndex], [MenuLevel], [MenuUrl]) VALUES (N'abemplexp', N'Working Experience', N'abempl', 3, 1, N'empl/workexp')
INSERT [dbo].[SysMenuDms] ([MenuId], [MenuCaption], [MenuHeader], [MenuIndex], [MenuLevel], [MenuUrl]) VALUES (N'abemplmuta', N'History Mutation', N'abempl', 4, 1, N'empl/mutation')
INSERT [dbo].[SysMenuDms] ([MenuId], [MenuCaption], [MenuHeader], [MenuIndex], [MenuLevel], [MenuUrl]) VALUES (N'abempledu', N'History Education', N'abempl', 5, 1, N'empl/education')

INSERT [dbo].[SysMenuDms] ([MenuId], [MenuCaption], [MenuHeader], [MenuIndex], [MenuLevel], [MenuUrl]) VALUES (N'abshiftmapping', N'Mapping Shift', N'abtrans', 1, 2, N'trans/shiftmap')
INSERT [dbo].[SysMenuDms] ([MenuId], [MenuCaption], [MenuHeader], [MenuIndex], [MenuLevel], [MenuUrl]) VALUES (N'abupload', N'Upload Data', N'abtrans', 2, 2, N'trans/upload')
INSERT [dbo].[SysMenuDms] ([MenuId], [MenuCaption], [MenuHeader], [MenuIndex], [MenuLevel], [MenuUrl]) VALUES (N'abovertime', N'Maintain Overtime', N'abtrans', 3, 2, N'trans/overtime')

INSERT [dbo].[SysMenuDms] ([MenuId], [MenuCaption], [MenuHeader], [MenuIndex], [MenuLevel], [MenuUrl]) VALUES (N'abrptholiday', N'Master Holiday', N'abreport', 1, 2, N'report/holiday')
INSERT [dbo].[SysMenuDms] ([MenuId], [MenuCaption], [MenuHeader], [MenuIndex], [MenuLevel], [MenuUrl]) VALUES (N'abrptshift', N'Master Shift', N'abreport', 2, 2, N'report/shift')
INSERT [dbo].[SysMenuDms] ([MenuId], [MenuCaption], [MenuHeader], [MenuIndex], [MenuLevel], [MenuUrl]) VALUES (N'abrptposition', N'Master Position', N'abreport', 3, 2, N'report/position')

/****** Object:  Table [dbo].[sysGroupDms]    Script Date: 10/23/2013 16:10:17 ******/
DELETE [dbo].[sysGroupDms] 
INSERT [dbo].[sysGroupDms] ([GroupId], [GroupName], [IsActive], [Themes], [IsAdmin]) VALUES (N'ADM', N'ADMIN', 1, N'', 1)
INSERT [dbo].[sysGroupDms] ([GroupId], [GroupName], [IsActive], [Themes], [IsAdmin]) VALUES (N'CS-USR', N'CS USER', 1, N'', 0)
INSERT [dbo].[sysGroupDms] ([GroupId], [GroupName], [IsActive], [Themes], [IsAdmin]) VALUES (N'HRD-USR', N'HRD USER', 1, N'', 0)
INSERT [dbo].[sysGroupDms] ([GroupId], [GroupName], [IsActive], [Themes], [IsAdmin]) VALUES (N'OTH', N'OTHERS', 1, N'', 0)
INSERT [dbo].[sysGroupDms] ([GroupId], [GroupName], [IsActive], [Themes], [IsAdmin]) VALUES (N'SFM-USR', N'SFM USER', 1, N'', 0)

/****** Object:  Table [dbo].[SysReportDms]    Script Date: 10/24/2013 09:19:20 ******/
DELETE [dbo].[SysReportDms] 
INSERT [dbo].[SysReportDms] ([ReportID], [ReportPath], [ReportProc]) VALUES (N'CsBirthday', N'cs/CsBirthday.rdlc', N'uspfn_CsRptBirthday')
INSERT [dbo].[SysReportDms] ([ReportID], [ReportPath], [ReportProc]) VALUES (N'CsBPKBNOF', N'cs/CsBPKBNOF.rdlc', N'uspfn_CsRptBPKBNOF')
INSERT [dbo].[SysReportDms] ([ReportID], [ReportPath], [ReportProc]) VALUES (N'CsHoliday', N'cs/CsHoliday.rdlc', N'uspfn_CsRptHoliday')
INSERT [dbo].[SysReportDms] ([ReportID], [ReportPath], [ReportProc]) VALUES (N'CsStnkExt', N'cs/CsStnkExt.rdlc', N'uspfn_CsRptStnkExt')
INSERT [dbo].[SysReportDms] ([ReportID], [ReportPath], [ReportProc]) VALUES (N'CsTDayCall', N'cs/CsTDayCall.rdlc', N'uspfn_CsRptTDayCall')
INSERT [dbo].[SysReportDms] ([ReportID], [ReportPath], [ReportProc]) VALUES (N'HrMstEmployee', N'hr/HrMstHoliday.rdlc', N'usprpt_HrMstEmployee')
INSERT [dbo].[SysReportDms] ([ReportID], [ReportPath], [ReportProc]) VALUES (N'HrMstShift', N'hr/HrMstShift.rdlc', N'usprpt_HrMstShift')
INSERT [dbo].[SysReportDms] ([ReportID], [ReportPath], [ReportProc]) VALUES (N'SvRpReport014', N'sv/SvRpReport014', N'usprpt_SvRpReport014')
