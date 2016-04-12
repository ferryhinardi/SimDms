

INSERT [dbo].[SysMenuDms] ([MenuId], [MenuCaption], [MenuHeader], [MenuIndex], [MenuLevel], [MenuUrl]) VALUES (N'gnmember', N'User Access', N'gn', 1, 1, NULL)
INSERT [dbo].[SysMenuDms] ([MenuId], [MenuCaption], [MenuHeader], [MenuIndex], [MenuLevel], [MenuUrl]) VALUES (N'gnmenu', N'Menu', N'gnmember', 3, 2, N'member/menu')
INSERT [dbo].[SysMenuDms] ([MenuId], [MenuCaption], [MenuHeader], [MenuIndex], [MenuLevel], [MenuUrl]) VALUES (N'gnmodule', N'Module', N'gnmember', 5, 1, N'Member/Module')
INSERT [dbo].[SysMenuDms] ([MenuId], [MenuCaption], [MenuHeader], [MenuIndex], [MenuLevel], [MenuUrl]) VALUES (N'gnrmnu', N'Role Menu', N'gnmember', 4, 2, N'member/rolemenu')
INSERT [dbo].[SysMenuDms] ([MenuId], [MenuCaption], [MenuHeader], [MenuIndex], [MenuLevel], [MenuUrl]) VALUES (N'gnrole', N'Role', N'gnmember', 2, 2, N'member/role')
INSERT [dbo].[SysMenuDms] ([MenuId], [MenuCaption], [MenuHeader], [MenuIndex], [MenuLevel], [MenuUrl]) VALUES (N'gnRoleModule', N'Role Module', N'gnmember', 6, 2, N'Member/RoleModule')
INSERT [dbo].[SysMenuDms] ([MenuId], [MenuCaption], [MenuHeader], [MenuIndex], [MenuLevel], [MenuUrl]) VALUES (N'gnuser', N'User', N'gnmember', 1, 2, N'member/user')

-- Posting Menu
INSERT [dbo].[SysMenuDms] ([MenuId], [MenuCaption], [MenuHeader], [MenuIndex], [MenuLevel], [MenuUrl]) VALUES (N'gnposting', N'Posting', N'gn', 3, 1, NULL)
INSERT [dbo].[SysMenuDms] ([MenuId], [MenuCaption], [MenuHeader], [MenuIndex], [MenuLevel], [MenuUrl]) VALUES (N'gnpostdaily', N'Daily Posting', N'gnposting', 1, 2, 'posting/daily')

