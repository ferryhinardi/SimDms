

delete gnMstPosition;


declare @CompanyCode varchar(25);
set @CompanyCode = ( select top 1 CompanyCode from gnMstLookUpHdr );




INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'COM', N'CEO', N'CHIEF OPERATING OFFICER (CEO)', NULL, 0, NULL, NULL, N'walmi', CAST(0x0000A26500B82054 AS DateTime), N'walmi', CAST(0x0000A26500FC3B7C AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'FAD', N'AD', N'ACCOUNTING DEVELOPMENT', N'FAD', 20, NULL, NULL, N'walmi', CAST(0x0000A26500E180E8 AS DateTime), N'walmi', CAST(0x0000A26500E180E8 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'FAD', N'AP', N'ACCOUNT PAYABLE', N'APH', 40, NULL, NULL, N'walmi', CAST(0x0000A26500E0667C AS DateTime), N'walmi', CAST(0x0000A26500E0667C AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'FAD', N'APH', N'ACCOUNT PAYABLE HEAD', N'FD', 30, NULL, NULL, N'walmi', CAST(0x0000A26500E0379C AS DateTime), N'walmi', CAST(0x0000A26500E0379C AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'FAD', N'AR', N'ACCOUNT RECEIVABLE', N'ARH', 40, NULL, NULL, N'walmi', CAST(0x0000A26500E150DC AS DateTime), N'walmi', CAST(0x0000A26500E150DC AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'FAD', N'ARH', N'ACCOUNT RECEIVABLE HEAD', N'FD', 30, NULL, NULL, N'walmi', CAST(0x0000A26500E121FC AS DateTime), N'walmi', CAST(0x0000A26500E121FC AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'FAD', N'FAD', N'COO FINANCE & ACCOUNTING', N'CEO', 10, NULL, NULL, N'walmi', CAST(0x0000A26500DF7C1C AS DateTime), N'walmi', CAST(0x0000A26600EA19B0 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'FAD', N'FD', N'FINANCE DEVELOPMENT', N'FAD', 20, NULL, NULL, N'walmi', CAST(0x0000A26500DFF4D0 AS DateTime), N'walmi', CAST(0x0000A26500DFF4D0 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'FAD', N'GL', N'GENERAL LEDGER', N'GLH', 40, NULL, NULL, N'walmi', CAST(0x0000A26500E23560 AS DateTime), N'walmi', CAST(0x0000A26500E23560 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'FAD', N'GLH', N'GENERAL LEDGER HEAD', N'AD', 30, NULL, NULL, N'walmi', CAST(0x0000A26500E207AC AS DateTime), N'walmi', CAST(0x0000A26500E207AC AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'FAD', N'KS', N'KASIR', N'APH', 40, NULL, NULL, N'walmi', CAST(0x0000A26500E08D28 AS DateTime), N'walmi', CAST(0x0000A26500E08D28 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'FAD', N'MH', N'MESSANGER HOLDING', N'APH', 40, NULL, NULL, N'walmi', CAST(0x0000A26500E0E890 AS DateTime), N'walmi', CAST(0x0000A26500E0E890 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'FAD', N'TH', N'TAX HEAD', N'AD', 30, NULL, NULL, N'walmi', CAST(0x0000A26500E1AFC8 AS DateTime), N'walmi', CAST(0x0000A26500E1AFC8 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'FAD', N'TX', N'TAX', N'TH', 40, NULL, NULL, N'walmi', CAST(0x0000A26500E1D7A0 AS DateTime), N'walmi', CAST(0x0000A26500E1D7A0 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'HRD-GA', N'BMC', N'BUILDING & MAINTENANCE', N'GA', 30, NULL, NULL, N'walmi', CAST(0x0000A26500BA59DC AS DateTime), N'walmi', CAST(0x0000A26500BA59DC AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'HRD-GA', N'CHG', N'COO HRD-GA', N'CEO', 10, NULL, NULL, N'walmi', CAST(0x0000A26500B87964 AS DateTime), N'walmi', CAST(0x0000A26600E9C7A8 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'HRD-GA', N'DEV', N'DEVELOPMENT', N'HRD', 30, NULL, NULL, N'walmi', CAST(0x0000A26500B99754 AS DateTime), N'walmi', CAST(0x0000A26500B99754 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'HRD-GA', N'GA', N'GENERAL AFFAIR', N'CHG', 20, NULL, NULL, N'walmi', CAST(0x0000A26500B9EBB4 AS DateTime), N'walmi', CAST(0x0000A26501001F94 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'HRD-GA', N'GAADM', N'GENERAL AFFAIR ADMIN', N'GA', 40, NULL, NULL, N'walmi', CAST(0x0000A26500BAEB2C AS DateTime), N'walmi', CAST(0x0000A26500BAEB2C AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'HRD-GA', N'HRD', N'HUMAN RESOURCE DEVELOPMENT', N'CHG', 20, NULL, NULL, N'walmi', CAST(0x0000A26500B90028 AS DateTime), N'walmi', CAST(0x0000A26501001760 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'HRD-GA', N'IRP', N'INDUSTRIAL RELATION, PERSONALIA', N'HRD', 30, NULL, NULL, N'walmi', CAST(0x0000A265010095F0 AS DateTime), N'walmi', CAST(0x0000A265010095F0 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'HRD-GA', N'PS', N'PURCHASING SERVICE', N'GA', 30, NULL, NULL, N'walmi', CAST(0x0000A26500BA8790 AS DateTime), N'walmi', CAST(0x0000A26500BA8790 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'HRD-GA', N'REC', N'RECRUITMENT', N'HRD', 30, NULL, NULL, N'walmi', CAST(0x0000A26500B9BF2C AS DateTime), N'walmi', CAST(0x0000A26500B9BF2C AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'IT', N'CIT', N'COO IT', N'CEO', 10, NULL, NULL, N'walmi', CAST(0x0000A26500BB1C64 AS DateTime), N'walmi', CAST(0x0000A26600E9E3C8 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'IT', N'IT', N'IT', N'ITD', 30, NULL, NULL, N'walmi', CAST(0x0000A26500BB989C AS DateTime), N'walmi', CAST(0x0000A26500BB989C AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'IT', N'ITD', N'IT DEVELOPMENT', N'CIT', 20, NULL, NULL, N'walmi', CAST(0x0000A26500BB4EC8 AS DateTime), N'walmi', CAST(0x0000A26500BB4EC8 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'PART & ACC', N'AADM', N'ACCESSORIES ADMIN', N'AD', 30, NULL, NULL, N'walmi', CAST(0x0000A26600BA3A38 AS DateTime), N'walmi', CAST(0x0000A26600BA3A38 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'PART & ACC', N'AD', N'ACCESSORIES DEVELOPMENT', N'CPA', 20, NULL, NULL, N'walmi', CAST(0x0000A26600B859C0 AS DateTime), N'walmi', CAST(0x0000A26600B859C0 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'PART & ACC', N'CPA', N'COO PARTS & ACCESSORIES', N'CEO', 10, NULL, NULL, N'walmi', CAST(0x0000A26600B74404 AS DateTime), N'walmi', CAST(0x0000A26600B74404 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'PART & ACC', N'PADM', N'PARTS ADMIN', N'PD', 30, NULL, NULL, N'walmi', CAST(0x0000A26600BA0900 AS DateTime), N'walmi', CAST(0x0000A26600BA0900 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'PART & ACC', N'PAS', N'PARTS & ACCESSORIES SALES', N'CPA', 25, NULL, NULL, N'walmi', CAST(0x0000A26600BA74D0 AS DateTime), N'walmi', CAST(0x0000A26600BA74D0 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'PART & ACC', N'PD', N'PARTS DEVELOPMENT', N'CPA', 20, NULL, NULL, N'walmi', CAST(0x0000A26600B7D1D0 AS DateTime), N'walmi', CAST(0x0000A26600B7D1D0 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'PDC', N'CP', N'COO PRE DELIVERY CENTER (PDC)', N'CEO', 10, NULL, NULL, N'walmi', CAST(0x0000A26500E27700 AS DateTime), N'walmi', CAST(0x0000A26600EA2A18 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'PDC', N'LDADM', N'LOGISTIC & DISTRIBUTION ADMIN', N'LDH', 30, NULL, NULL, N'walmi', CAST(0x0000A26500E30148 AS DateTime), N'walmi', CAST(0x0000A26500E30148 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'PDC', N'LDH', N'LOGISTIC & DISTRIBUTION HEAD', N'CP', 20, NULL, NULL, N'walmi', CAST(0x0000A26500E2B2C4 AS DateTime), N'walmi', CAST(0x0000A26500E2B2C4 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'PDC', N'OBP', N'OFFICE BOY PDC', N'LDH', 30, NULL, NULL, N'walmi', CAST(0x0000A26500E3EE00 AS DateTime), N'walmi', CAST(0x0000A26500E3EE00 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'PDC', N'PDI', N'PRE DELIVERY INSPECTION', N'LDH', 30, NULL, NULL, N'walmi', CAST(0x0000A26500E3B818 AS DateTime), N'walmi', CAST(0x0000A26500E3B818 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'PDC', N'SU', N'STOCK UNIT', N'LDADM', 40, NULL, NULL, N'walmi', CAST(0x0000A26500E33604 AS DateTime), N'walmi', CAST(0x0000A26500E33604 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'SALES', N'BM', N'BRANCH MANAGER', N'GM', 40, NULL, NULL, NULL, NULL, NULL, NULL, N'1')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'SALES', N'GM', N'GENERAL MANAGER', NULL, 5, NULL, NULL, NULL, NULL, NULL, NULL, N'0')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'SALES', N'S', N'WIRANIAGA', N'SC', 10, NULL, NULL, NULL, NULL, NULL, NULL, N'12')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'SALES', N'SC', N'SALES KOORDINATOR', N'SH', 20, NULL, NULL, NULL, NULL, NULL, NULL, N'13')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'SALES', N'SH', N'SALES HEAD', N'BM', 30, NULL, NULL, NULL, NULL, NULL, NULL, N'20')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'SALES & MKT', N'ADM', N'ADMINISTRASI', N'SADM', 40, NULL, NULL, N'walmi', CAST(0x0000A26600C20010 AS DateTime), N'walmi', CAST(0x0000A26600C20010 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'SALES & MKT', N'BMR', N'BRANCH MANAGER', N'CMS', 15, NULL, NULL, N'walmi', CAST(0x0000A26600EBD82C AS DateTime), N'walmi', CAST(0x0000A26600EBD82C AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'SALES & MKT', N'CADM', N'CRO ADMIN', N'CRM', 40, NULL, NULL, N'walmi', CAST(0x0000A26600C5727C AS DateTime), N'walmi', CAST(0x0000A26600C5727C AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'SALES & MKT', N'CMS', N'COO SALES-MARKETING', N'CEO', 10, NULL, NULL, N'walmi', CAST(0x0000A26600C0A1AC AS DateTime), N'walmi', CAST(0x0000A26600C0A1AC AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'SALES & MKT', N'CRM', N'CRM', N'MPH', 30, NULL, NULL, N'walmi', CAST(0x0000A26600C34128 AS DateTime), N'walmi', CAST(0x0000A26600C34128 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'SALES & MKT', N'DADM', N'DISTRIBUTION ADMIN', N'SD', 40, NULL, NULL, N'walmi', CAST(0x0000A26600C1A958 AS DateTime), N'walmi', CAST(0x0000A26600C1A958 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'SALES & MKT', N'DBA', N'DATA BASE ADMIN', N'SADM', 40, NULL, NULL, N'walmi', CAST(0x0000A26600C24408 AS DateTime), N'walmi', CAST(0x0000A26600C24408 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'SALES & MKT', N'DBT', N'DATA BASE TFT', N'STD', 40, NULL, NULL, N'walmi', CAST(0x0000A26600C536B8 AS DateTime), N'walmi', CAST(0x0000A26600C536B8 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'SALES & MKT', N'DRV', N'DRIVER', N'BMR', 40, NULL, NULL, N'walmi', CAST(0x0000A26600ED83E8 AS DateTime), N'walmi', CAST(0x0000A26600ED83E8 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'SALES & MKT', N'EXB', N'EXHIBITION', N'MP', 40, NULL, NULL, N'walmi', CAST(0x0000A26600C48DF8 AS DateTime), N'walmi', CAST(0x0000A26600C48DF8 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'SALES & MKT', N'FADM', N'FINANCE ADMIN', N'BMR', 40, NULL, NULL, N'walmi', CAST(0x0000A26600EE6164 AS DateTime), N'walmi', CAST(0x0000A26600EE6164 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'SALES & MKT', N'HG', N'HRD/GA', N'BMR', 30, NULL, NULL, N'walmi', CAST(0x0000A26600EF36AC AS DateTime), N'walmi', CAST(0x0000A26600EF36AC AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'SALES & MKT', N'HGADM', N'HRD/GA ADMIN', N'HG', 40, NULL, NULL, N'walmi', CAST(0x0000A26600EF60DC AS DateTime), N'walmi', CAST(0x0000A26600EF60DC AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'SALES & MKT', N'KSB', N'KASIR BESAR', N'BMR', 40, NULL, NULL, N'walmi', CAST(0x0000A26600EE0BD8 AS DateTime), N'walmi', CAST(0x0000A26600EE0BD8 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'SALES & MKT', N'KSK', N'KASIR KECIL', N'BMR', 40, NULL, NULL, N'walmi', CAST(0x0000A26600EE3284 AS DateTime), N'walmi', CAST(0x0000A26600EE3284 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'SALES & MKT', N'MP', N'MARKETING & PROMOTION', N'MPH', 30, NULL, NULL, N'walmi', CAST(0x0000A26600C2D0A8 AS DateTime), N'walmi', CAST(0x0000A26600C2D0A8 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'SALES & MKT', N'MPADM', N'MARKETING & PROMOTION ADMIN', N'MP', 40, NULL, NULL, N'walmi', CAST(0x0000A26600C37710 AS DateTime), N'walmi', CAST(0x0000A26600C37710 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'SALES & MKT', N'MPH', N'MARKETING & PROMOTION HEAD', N'CMS', 20, NULL, NULL, N'walmi', CAST(0x0000A26600C2A420 AS DateTime), N'walmi', CAST(0x0000A26600C2A420 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'SALES & MKT', N'MR', N'MESSANGER', N'BMR', 40, NULL, NULL, N'walmi', CAST(0x0000A26600EEF50C AS DateTime), N'walmi', CAST(0x0000A26600EEF50C AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'SALES & MKT', N'OB', N'OFFICE BOY', N'HG', 40, NULL, NULL, N'walmi', CAST(0x0000A26600EF946C AS DateTime), N'walmi', CAST(0x0000A26600EF946C AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'SALES & MKT', N'SADM', N'SALES ADMINISTRASI', N'SDA', 30, NULL, NULL, N'walmi', CAST(0x0000A26600C17A78 AS DateTime), N'walmi', CAST(0x0000A26600C17A78 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'SALES & MKT', N'SD', N'SALES DISTRIBUTION', N'SDA', 30, NULL, NULL, N'walmi', CAST(0x0000A26600C14CC4 AS DateTime), N'walmi', CAST(0x0000A26600C14CC4 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'SALES & MKT', N'SDA', N'SALES DISTRIBUTION & ADMINISTRASI', N'CMS', 20, NULL, NULL, N'walmi', CAST(0x0000A26600C11484 AS DateTime), N'walmi', CAST(0x0000A26600C11484 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'SALES & MKT', N'SLS', N'SALES', N'SLSC', 40, NULL, NULL, N'walmi', CAST(0x0000A26600ECE35C AS DateTime), N'walmi', CAST(0x0000A26600ECE35C AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'SALES & MKT', N'SLSADM', N'SALES ADMIN', N'BMR', 40, NULL, NULL, N'walmi', CAST(0x0000A26600ED1818 AS DateTime), N'walmi', CAST(0x0000A26600ED1818 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'SALES & MKT', N'SLSC', N'SALES COORDINATOR', N'SLSH', 30, NULL, NULL, N'walmi', CAST(0x0000A26600ECB5A8 AS DateTime), N'walmi', CAST(0x0000A26600ECB5A8 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'SALES & MKT', N'SLSH', N'SALES HEAD', N'BMR', 20, NULL, NULL, N'walmi', CAST(0x0000A26600EC6D00 AS DateTime), N'walmi', CAST(0x0000A26600EC6D00 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'SALES & MKT', N'SLSS', N'SALES SUPPORT', N'BMR', 40, NULL, NULL, N'walmi', CAST(0x0000A26600ED4F2C AS DateTime), N'walmi', CAST(0x0000A26600ED4F2C AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'SALES & MKT', N'STD', N'SALES TRAINING DEVELOPMENT', N'MPH', 30, NULL, NULL, N'walmi', CAST(0x0000A26600C3111C AS DateTime), N'walmi', CAST(0x0000A26600C3111C AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'SALES & MKT', N'TS', N'TFT SALES', N'STD', 40, NULL, NULL, N'walmi', CAST(0x0000A26600C4BF30 AS DateTime), N'walmi', CAST(0x0000A26600C4BF30 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'SALES & MKT', N'TSADM', N'TFT SALES ADMIN', N'STD', 40, NULL, NULL, N'walmi', CAST(0x0000A26600C507D8 AS DateTime), N'walmi', CAST(0x0000A26600C507D8 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'SERVICE', N'FR', N'FOREMAN', N'SA', 40, NULL, NULL, N'walmi', CAST(0x0000A26500EB3C50 AS DateTime), N'walmi', CAST(0x0000A26500EB3C50 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'SERVICE', N'FRBDR', N'FOREMAN BODY REPAIR', N'SABDR', 40, NULL, NULL, N'walmi', CAST(0x0000A26500FDFFD4 AS DateTime), N'walmi', CAST(0x0000A26500FDFFD4 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'SERVICE', N'SA', N'SERVICE ADVISOR', N'SM', 30, NULL, NULL, NULL, NULL, N'walmi', CAST(0x0000A26500ECDFD8 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'SERVICE', N'SAADM', N'SERVICE ADVISOR ADMIN', N'SA', 40, NULL, NULL, N'walmi', CAST(0x0000A26500EB8AD4 AS DateTime), N'walmi', CAST(0x0000A26500EB8AD4 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'SERVICE', N'SABDR', N'SERVICE ADVISOR BODY REPAIR', N'SM', 30, NULL, NULL, N'walmi', CAST(0x0000A26500FD7DC0 AS DateTime), N'walmi', CAST(0x0000A26500FD7DC0 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'SERVICE', N'SDRV', N'P2K (DRIVER)', N'SA', 40, NULL, NULL, N'walmi', CAST(0x0000A26500EC12C4 AS DateTime), N'walmi', CAST(0x0000A26500EC12C4 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'SERVICE', N'SM', N'SERVICE MANAGER', N'SRM', 20, NULL, NULL, N'walmi', CAST(0x0000A26500E9AF0C AS DateTime), N'walmi', CAST(0x0000A26500E9AF0C AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'SERVICE', N'SPADM', N'SPAREPARTS ADMIN', N'SM', 30, NULL, NULL, N'walmi', CAST(0x0000A26500EADB0C AS DateTime), N'walmi', CAST(0x0000A26500EADB0C AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'SERVICE', N'SRADM', N'SERVICE ADMIN', N'SM', 30, NULL, NULL, N'walmi', CAST(0x0000A26500EA8454 AS DateTime), N'walmi', CAST(0x0000A26500EA8454 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'SERVICE', N'SRBDRADM', N'SERVICE BODY REPAIR ADMIN', N'SM', 30, NULL, NULL, N'walmi', CAST(0x0000A26500FDC8C0 AS DateTime), N'walmi', CAST(0x0000A26500FDC8C0 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'SERVICE', N'SRD', N'SERVICE REPRESENTATIVE DEVELOPMENT', N'SRM', 15, NULL, NULL, N'walmi', CAST(0x0000A26200EA7770 AS DateTime), N'walmi', CAST(0x0000A26200EA7770 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'SERVICE', N'SRDADM', N'SERVICE REPRESENTATIF DEVEOPMENT ADMIN', N'SRD', 30, NULL, NULL, N'walmi', CAST(0x0000A26200EB15A4 AS DateTime), N'walmi', CAST(0x0000A26500E6FCD0 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'SERVICE', N'SRM', N'SERVICE REPRESENTATIVE MANAGER (COO)', N'CEO', 10, NULL, NULL, N'walmi', CAST(0x0000A26200EA2B44 AS DateTime), N'walmi', CAST(0x0000A26600E9FFE8 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'SERVICE', N'SRO', N'SERVICE RELATION OFFICER (SRO)', N'SM', 30, NULL, NULL, NULL, NULL, N'walmi', CAST(0x0000A26500ECD54C AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'SERVICE', N'SSPV', N'SERVICE SUPERVISOR', N'SM', 25, NULL, NULL, N'walmi', CAST(0x0000A26500EA2B44 AS DateTime), N'walmi', CAST(0x0000A26500EA2B44 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'SERVICE', N'TK', N'TEKNISI', N'FR', 50, NULL, NULL, N'walmi', CAST(0x0000A26500EC4078 AS DateTime), N'walmi', CAST(0x0000A26500EC4078 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (@CompanyCode, N'SERVICE', N'TKBDR', N'TEKNISI BODY REPAIR', N'FRBDR', 50, NULL, NULL, N'walmi', CAST(0x0000A26500FE4C00 AS DateTime), N'walmi', CAST(0x0000A26500FE4C00 AS DateTime), NULL)

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (N'6006408', N'SALES', N'BM', N'BRANCH MANAGER', N'GM', 40, NULL, NULL, NULL, NULL, NULL, NULL, N'1')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (N'6006408', N'SALES', N'S', N'WIRANIAGA', N'SC', 10, NULL, NULL, NULL, NULL, NULL, NULL, N'12')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (N'6006408', N'SALES', N'SC', N'SALES KOORDINATOR', N'SH', 20, NULL, NULL, NULL, NULL, NULL, NULL, N'13')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (N'6006408', N'SALES', N'SH', N'SALES HEAD', N'BM', 30, NULL, NULL, NULL, NULL, NULL, NULL, N'20')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (N'6015401', N'SALES', N'BM', N'BRANCH MANAGER', N'GM', 40, NULL, NULL, NULL, NULL, NULL, NULL, N'1')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (N'6015401', N'SALES', N'S', N'WIRANIAGA', N'SC', 10, NULL, NULL, NULL, NULL, NULL, NULL, N'12')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (N'6015401', N'SALES', N'SC', N'SALES KOORDINATOR', N'SH', 20, NULL, NULL, NULL, NULL, NULL, NULL, N'13')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (N'6015401', N'SALES', N'SH', N'SALES HEAD', N'BM', 30, NULL, NULL, NULL, NULL, NULL, NULL, N'20')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (N'6021406', N'SALES', N'BM', N'BRANCH MANAGER', N'GM', 40, NULL, NULL, NULL, NULL, NULL, NULL, N'1')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (N'6021406', N'SALES', N'S', N'WIRANIAGA', N'SC', 10, NULL, NULL, NULL, NULL, NULL, NULL, N'12')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (N'6021406', N'SALES', N'SC', N'SALES KOORDINATOR', N'SH', 20, NULL, NULL, NULL, NULL, NULL, NULL, N'13')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (N'6021406', N'SALES', N'SH', N'SALES HEAD', N'BM', 30, NULL, NULL, NULL, NULL, NULL, NULL, N'20')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (N'6026401', N'SALES', N'BM', N'BRANCH MANAGER', N'GM', 40, NULL, NULL, NULL, NULL, NULL, NULL, N'1')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (N'6026401', N'SALES', N'S', N'WIRANIAGA', N'SC', 10, NULL, NULL, NULL, NULL, NULL, NULL, N'12')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (N'6026401', N'SALES', N'SC', N'SALES KOORDINATOR', N'SH', 20, NULL, NULL, NULL, NULL, NULL, NULL, N'13')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (N'6026401', N'SALES', N'SH', N'SALES HEAD', N'BM', 30, NULL, NULL, NULL, NULL, NULL, NULL, N'20')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (N'6031401', N'SALES', N'BM', N'BRANCH MANAGER', N'GM', 40, NULL, NULL, NULL, NULL, NULL, NULL, N'1')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (N'6031401', N'SALES', N'S', N'WIRANIAGA', N'SC', 10, NULL, NULL, NULL, NULL, NULL, NULL, N'12')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (N'6031401', N'SALES', N'SC', N'SALES KOORDINATOR', N'SH', 20, NULL, NULL, NULL, NULL, NULL, NULL, N'13')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (N'6031401', N'SALES', N'SH', N'SALES HEAD', N'BM', 30, NULL, NULL, NULL, NULL, NULL, NULL, N'20')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (N'6045401', N'SALES', N'BM', N'BRANCH MANAGER', N'GM', 40, NULL, NULL, NULL, NULL, NULL, NULL, N'1')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (N'6045401', N'SALES', N'S', N'WIRANIAGA', N'SC', 10, NULL, NULL, NULL, NULL, NULL, NULL, N'12')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (N'6045401', N'SALES', N'SC', N'SALES KOORDINATOR', N'SH', 20, NULL, NULL, NULL, NULL, NULL, NULL, N'13')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (N'6045401', N'SALES', N'SH', N'SALES HEAD', N'BM', 30, NULL, NULL, NULL, NULL, NULL, NULL, N'20')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (N'6078401', N'SALES', N'BM', N'BRANCH MANAGER', N'GM', 40, NULL, NULL, NULL, NULL, NULL, NULL, N'1')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (N'6078401', N'SALES', N'S', N'WIRANIAGA', N'SC', 10, NULL, NULL, NULL, NULL, NULL, NULL, N'12')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (N'6078401', N'SALES', N'SC', N'SALES KOORDINATOR', N'SH', 20, NULL, NULL, NULL, NULL, NULL, NULL, N'13')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (N'6078401', N'SALES', N'SH', N'SALES HEAD', N'BM', 30, NULL, NULL, NULL, NULL, NULL, NULL, N'20')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (N'6080401', N'SALES', N'BM', N'BRANCH MANAGER', N'GM', 40, NULL, NULL, NULL, NULL, NULL, NULL, N'1')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (N'6080401', N'SALES', N'S', N'WIRANIAGA', N'SC', 10, NULL, NULL, NULL, NULL, NULL, NULL, N'12')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (N'6080401', N'SALES', N'SC', N'SALES KOORDINATOR', N'SH', 20, NULL, NULL, NULL, NULL, NULL, NULL, N'13')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (N'6080401', N'SALES', N'SH', N'SALES HEAD', N'BM', 30, NULL, NULL, NULL, NULL, NULL, NULL, N'20')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (N'6081401', N'SALES', N'BM', N'BRANCH MANAGER', N'GM', 40, NULL, NULL, NULL, NULL, NULL, NULL, N'1')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (N'6081401', N'SALES', N'S', N'WIRANIAGA', N'SC', 10, NULL, NULL, NULL, NULL, NULL, NULL, N'12')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (N'6081401', N'SALES', N'SC', N'SALES KOORDINATOR', N'SH', 20, NULL, NULL, NULL, NULL, NULL, NULL, N'13')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (N'6081401', N'SALES', N'SH', N'SALES HEAD', N'BM', 30, NULL, NULL, NULL, NULL, NULL, NULL, N'20')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (N'6115204', N'SALES', N'BM', N'BRANCH MANAGER', N'GM', 10, NULL, NULL, NULL, NULL, NULL, NULL, N'1')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (N'6115204', N'SALES', N'CT', N'SALES COUNTER', N'BM', 30, NULL, NULL, NULL, NULL, NULL, NULL, N'31')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (N'6115204', N'SALES', N'GM', N'OPERATIONAL MANAGER', N'', 5, NULL, NULL, NULL, NULL, NULL, NULL, N'0')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (N'6115204', N'SALES', N'S', N'SALESMAN', N'SC', 30, NULL, NULL, NULL, NULL, NULL, NULL, N'20')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (N'6115204', N'SALES', N'SC', N'SALES KOORDINATOR', N'BM', 20, NULL, NULL, NULL, NULL, NULL, NULL, N'27')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (N'6156401', N'SALES', N'BM', N'BRANCH MANAGER', N'GM', 40, NULL, NULL, NULL, NULL, NULL, NULL, N'1')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (N'6156401', N'SALES', N'S', N'WIRANIAGA', N'SC', 10, NULL, NULL, NULL, NULL, NULL, NULL, N'12')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (N'6156401', N'SALES', N'SC', N'SALES KOORDINATOR', N'SH', 20, NULL, NULL, NULL, NULL, NULL, NULL, N'13')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (N'6156401', N'SALES', N'SH', N'SALES HEAD', N'BM', 30, NULL, NULL, NULL, NULL, NULL, NULL, N'20')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (N'6158401', N'SALES', N'BM', N'BRANCH MANAGER', N'GM', 40, NULL, NULL, NULL, NULL, NULL, NULL, N'1')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (N'6158401', N'SALES', N'S', N'WIRANIAGA', N'SC', 10, NULL, NULL, NULL, NULL, NULL, NULL, N'12')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (N'6158401', N'SALES', N'SC', N'SALES KOORDINATOR', N'SH', 20, NULL, NULL, NULL, NULL, NULL, NULL, N'13')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (N'6158401', N'SALES', N'SH', N'SALES HEAD', N'BM', 30, NULL, NULL, NULL, NULL, NULL, NULL, N'20')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (N'6159401', N'SALES', N'BM', N'BRANCH MANAGER', N'GM', 40, NULL, NULL, NULL, NULL, NULL, NULL, N'1')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (N'6159401', N'SALES', N'S', N'WIRANIAGA', N'SC', 10, NULL, NULL, NULL, NULL, NULL, NULL, N'12')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (N'6159401', N'SALES', N'SC', N'SALES KOORDINATOR', N'SH', 20, NULL, NULL, NULL, NULL, NULL, NULL, N'13')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (N'6159401', N'SALES', N'SH', N'SALES HEAD', N'BM', 30, NULL, NULL, NULL, NULL, NULL, NULL, N'20')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (N'6162401', N'SALES', N'BM', N'BRANCH MANAGER', N'GM', 40, NULL, NULL, NULL, NULL, NULL, NULL, N'1')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (N'6162401', N'SALES', N'S', N'WIRANIAGA', N'SC', 10, NULL, NULL, NULL, NULL, NULL, NULL, N'12')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (N'6162401', N'SALES', N'SC', N'SALES KOORDINATOR', N'SH', 20, NULL, NULL, NULL, NULL, NULL, NULL, N'13')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (N'6162401', N'SALES', N'SH', N'SALES HEAD', N'BM', 30, NULL, NULL, NULL, NULL, NULL, NULL, N'20')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (N'6419401', N'SALES', N'BM', N'BRANCH MANAGER', N'GM', 40, NULL, NULL, NULL, NULL, NULL, NULL, N'1')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (N'6419401', N'SALES', N'S', N'WIRANIAGA', N'SC', 10, NULL, NULL, NULL, NULL, NULL, NULL, N'12')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (N'6419401', N'SALES', N'SC', N'SALES KOORDINATOR', N'SH', 20, NULL, NULL, NULL, NULL, NULL, NULL, N'13')

INSERT [dbo].[gnMstPosition] ([CompanyCode], [DeptCode], [PosCode], [PosName], [PosHeader], [PosLevel], [StartDate], [FinishDate], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [TitleCode]) VALUES (N'6419401', N'SALES', N'SH', N'SALES HEAD', N'BM', 30, NULL, NULL, NULL, NULL, NULL, NULL, N'20')



go
select * from gnMstPosition