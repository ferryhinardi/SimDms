
declare @CompanyCode varchar(25);
set @CompanyCode = ( select top 1 CompanyCode from gnMstLookUpHdr );




delete HrLookupMapping

INSERT [dbo].[HrLookupMapping] ([CompanyCode], [CodeID], [CodeDescription], [IsDeleted]) VALUES (@CompanyCode, N'BLOOD', N'GOLONGAN DARAH', 0)
INSERT [dbo].[HrLookupMapping] ([CompanyCode], [CodeID], [CodeDescription], [IsDeleted]) VALUES (@CompanyCode, N'FEDU', N'FORMAL EDUCATION', 0)
INSERT [dbo].[HrLookupMapping] ([CompanyCode], [CodeID], [CodeDescription], [IsDeleted]) VALUES (@CompanyCode, N'MRTL', N'MARITAL STATUS', 0)
INSERT [dbo].[HrLookupMapping] ([CompanyCode], [CodeID], [CodeDescription], [IsDeleted]) VALUES (@CompanyCode, N'MRTLK', N'SUDAH MENIKAH DAN PUNYA ANAK', 0)
INSERT [dbo].[HrLookupMapping] ([CompanyCode], [CodeID], [CodeDescription], [IsDeleted]) VALUES (@CompanyCode, N'MRTLK0', N'SUDAH MENIKAH DAN BELUM PUNYA ANAK', 0)
INSERT [dbo].[HrLookupMapping] ([CompanyCode], [CodeID], [CodeDescription], [IsDeleted]) VALUES (@CompanyCode, N'PERS', N'PERSONNEL STATUS', 0)
INSERT [dbo].[HrLookupMapping] ([CompanyCode], [CodeID], [CodeDescription], [IsDeleted]) VALUES (@CompanyCode, N'REASONCTG', N'RESIGN REASON ', 0)
INSERT [dbo].[HrLookupMapping] ([CompanyCode], [CodeID], [CodeDescription], [IsDeleted]) VALUES (@CompanyCode, N'RESIGNCTG', N'RESIGN CATEGORY', 0)
INSERT [dbo].[HrLookupMapping] ([CompanyCode], [CodeID], [CodeDescription], [IsDeleted]) VALUES (@CompanyCode, N'RLGN', N'RELIGION', 0)
INSERT [dbo].[HrLookupMapping] ([CompanyCode], [CodeID], [CodeDescription], [IsDeleted]) VALUES (@CompanyCode, N'SHOESSIZE', N'UKURAN SEPATU', 0)
INSERT [dbo].[HrLookupMapping] ([CompanyCode], [CodeID], [CodeDescription], [IsDeleted]) VALUES (@CompanyCode, N'SIZE', N'UNIFORM SIZE', 0)
INSERT [dbo].[HrLookupMapping] ([CompanyCode], [CodeID], [CodeDescription], [IsDeleted]) VALUES (@CompanyCode, N'SIZEALT', N'UNIFORM ALTERNATIVE SIZE', 0)



select * from HrLookupMapping