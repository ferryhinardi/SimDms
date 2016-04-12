
declare @CompanyCode varchar(25);
set @CompanyCode = ( select top 1 CompanyCode from gnMstOrganizationHdr );



delete HrDepartmentTraining;

INSERT [dbo].[HrDepartmentTraining] ([CompanyCode], [Department], [Position], [Grade], [TrainingCode], [IsRequired], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsDeleted]) VALUES (@CompanyCode, N'SALES', N'S', N'1', N'STDP1', NULL, NULL, NULL, NULL, NULL, 0)
INSERT [dbo].[HrDepartmentTraining] ([CompanyCode], [Department], [Position], [Grade], [TrainingCode], [IsRequired], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsDeleted]) VALUES (@CompanyCode, N'SALES', N'S', N'1', N'STDP2', NULL, NULL, NULL, NULL, NULL, 0)
INSERT [dbo].[HrDepartmentTraining] ([CompanyCode], [Department], [Position], [Grade], [TrainingCode], [IsRequired], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsDeleted]) VALUES (@CompanyCode, N'SALES', N'S', N'1', N'STDP3', NULL, NULL, NULL, NULL, NULL, 0)
INSERT [dbo].[HrDepartmentTraining] ([CompanyCode], [Department], [Position], [Grade], [TrainingCode], [IsRequired], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsDeleted]) VALUES (@CompanyCode, N'SALES', N'S', N'1', N'STDP4', NULL, NULL, NULL, NULL, NULL, 0)
INSERT [dbo].[HrDepartmentTraining] ([CompanyCode], [Department], [Position], [Grade], [TrainingCode], [IsRequired], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsDeleted]) VALUES (@CompanyCode, N'SALES', N'S', N'1', N'STDP5', NULL, NULL, NULL, NULL, NULL, 0)
INSERT [dbo].[HrDepartmentTraining] ([CompanyCode], [Department], [Position], [Grade], [TrainingCode], [IsRequired], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsDeleted]) VALUES (@CompanyCode, N'SALES', N'S', N'1', N'STDP6', NULL, NULL, NULL, NULL, NULL, 0)
INSERT [dbo].[HrDepartmentTraining] ([CompanyCode], [Department], [Position], [Grade], [TrainingCode], [IsRequired], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsDeleted]) VALUES (@CompanyCode, N'SALES', N'S', N'1', N'STDP7', NULL, NULL, NULL, NULL, NULL, 0)
INSERT [dbo].[HrDepartmentTraining] ([CompanyCode], [Department], [Position], [Grade], [TrainingCode], [IsRequired], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsDeleted]) VALUES (@CompanyCode, N'SALES', N'S', N'2', N'SPSS', NULL, NULL, NULL, NULL, NULL, 0)
INSERT [dbo].[HrDepartmentTraining] ([CompanyCode], [Department], [Position], [Grade], [TrainingCode], [IsRequired], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsDeleted]) VALUES (@CompanyCode, N'SALES', N'S', N'3', N'SPSG', NULL, NULL, NULL, NULL, NULL, 0)
INSERT [dbo].[HrDepartmentTraining] ([CompanyCode], [Department], [Position], [Grade], [TrainingCode], [IsRequired], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsDeleted]) VALUES (@CompanyCode, N'SALES', N'S', N'4', N'SPSP', NULL, NULL, NULL, NULL, NULL, 0)
INSERT [dbo].[HrDepartmentTraining] ([CompanyCode], [Department], [Position], [Grade], [TrainingCode], [IsRequired], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsDeleted]) VALUES (@CompanyCode, N'SALES', N'SC', N'.', N'SCA', NULL, NULL, NULL, NULL, NULL, 0)
INSERT [dbo].[HrDepartmentTraining] ([CompanyCode], [Department], [Position], [Grade], [TrainingCode], [IsRequired], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsDeleted]) VALUES (@CompanyCode, N'SALES', N'SC', N'.', N'SCB', NULL, NULL, NULL, NULL, NULL, 0)
INSERT [dbo].[HrDepartmentTraining] ([CompanyCode], [Department], [Position], [Grade], [TrainingCode], [IsRequired], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsDeleted]) VALUES (@CompanyCode, N'SALES', N'SH', N'.', N'SHB', NULL, NULL, NULL, NULL, NULL, 0)
INSERT [dbo].[HrDepartmentTraining] ([CompanyCode], [Department], [Position], [Grade], [TrainingCode], [IsRequired], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [IsDeleted]) VALUES (@CompanyCode, N'SALES', N'SH', N'.', N'SHI', NULL, NULL, NULL, NULL, NULL, 0)

