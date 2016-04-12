/*select distinct a.CompanyCode   
  , c.BranchCode  
  , a.ChassisCode  
     , a.ChassisNo  
     , (a.ChassisCode + convert(varchar, a.ChassisNo)) as VinNo  
     , a.BasicModel  
     , a.PoliceRegNo  
     , a.ServiceBookNo  
     , a.CustomerCode  
     , b.CustomerName  
     , a.ContractNo  
     , a.IsContractStatus  
     , case d.ContractDate  
         when ('19000101') then null  
        else d.ContractDate  
       end ContractDate  
     , a.ClubNo  
     , case a.ClubDateFinish  
         when ('19000101') then null  
         else a.ClubDateFinish  
       end ClubDateFinish  
     , a.IsClubStatus  
     , b.Address1  
     , b.Address2  
     , b.Address3   
     , b.Address4  
     , b.CityCode  
     , e.LookUpValueName as CityName  
     , b.PhoneNo  
     , b.HPNo  
     , b.FaxNo  
     , a.TransmissionType  
     , a.TechnicalModelCode  
     , case a.LastServiceDate  
         when ('19000101') then null  
         else a.LastServiceDate  
       end LastServiceDate  
     , a.LastJobType  
     , a.ColourCode  
     , a.EngineCode  
     , a.EngineNo  
     , c.MaterialDiscPct  
     , c.PartDiscPct  
     , c.LaborDiscPct  
  from svMstCustomerVehicle a  
  left join gnMstCustomer b  
    on b.CompanyCode = a.CompanyCode  
   and b.CustomerCode = a.CustomerCode  
 inner join gnMstCustomerProfitCenter c  
    on c.CustomerCode = b.CustomerCode   
   and c.CompanyCode = b.CompanyCode  
   and c.ProfitCenterCode = '200'  
 LEFT JOIN svMstContract d  
 ON d.CompanyCode = b.CompanyCode  
 AND d.CustomerCode = b.CustomerCode  
 LEFT JOIN gnMstLookUpDtl e  
 ON e.CompanyCode = b.CompanyCode  
 AND e.CodeID = 'CITY'  
 AND e.LookUpValue = b.CityCode  
 where isnull(a.BasicModel, '') <> ''  */

 if object_id('SvCustomerVehicleView') is not null
	drop view SvCustomerVehicleView
GO
CREATE VIEW [dbo].[SvCustomerVehicleView]
AS
SELECT DISTINCT 
                         a.CompanyCode, e.BranchCode, a.ChassisCode + CONVERT(varchar, a.ChassisNo) AS VinNo, a.PoliceRegNo, ISNULL(b.CustomerName, '') AS CustomerName, 
                         RTRIM(RTRIM(b.Address1) + ' ' + RTRIM(b.Address2) + ' ' + RTRIM(b.Address3) + ' ' + RTRIM(b.Address4)) AS CustomerAddr, b.Address1, b.Address2, b.Address3, 
                         b.Address4, b.CityCode,
                             (SELECT        LookUpValueName
                               FROM            dbo.gnMstLookUpDtl
                               WHERE        (CodeID = 'CItY') AND (LookUpValue = b.CityCode)) AS CityName, b.PhoneNo, b.HPNo, b.FaxNo, a.ClubCode, a.ClubNo, 

							   case a.ClubDateFinish when '1900-01-01' then '' else 

                         ISNULL(REPLACE(CONVERT(varchar, a.ClubDateFinish, 6), ' ', '-'), '') end AS ClubEndPeriod, 
						 
						 
						 
						 d.IsActive AS ClubStatus, 
                         CASE a.IsClubStatus WHEN 1 THEN (CASE d .IsActive WHEN 1 THEN 'Aktif' ELSE 'Tidak Aktif' END) ELSE '' END AS ClubStatusDesc, a.ContractNo, 
                         CASE a.IsContractStatus WHEN 1 THEN (CASE c.IsActive WHEN 1 THEN 'Aktif' ELSE 'Tidak Aktif' END) ELSE '' END AS ContractStatusDesc, 
                         CASE a.IsContractStatus WHEN 1 THEN (ISNULL(replace(CONVERT(varchar, a.RemainderDate, 6), ' ', '-'), '')) ELSE '' END AS ContractEndPeriod, a.BasicModel, 
                         a.ChassisNo, a.ChassisCode, a.EngineCode, a.EngineNo, a.TransmissionType, a.ServiceBookNo, CASE a.LastServiceDate WHEN ('19000101') 
                         THEN '' ELSE a.LastServiceDate END AS LastServiceDate, a.LastJobType, a.ColourCode, a.ColourCode AS ColorCode, a.CustomerCode
FROM            dbo.svMstCustomerVehicle AS a LEFT OUTER JOIN
                         dbo.gnMstCustomer AS b ON b.CompanyCode = a.CompanyCode AND b.CustomerCode = a.CustomerCode INNER JOIN
                         dbo.gnMstCustomerProfitCenter AS e ON e.CompanyCode = a.CompanyCode AND e.CustomerCode = a.CustomerCode LEFT OUTER JOIN
                         dbo.svMstContract AS c WITH (nowait, nolock) ON c.CompanyCode = a.CompanyCode AND c.ContractNo = a.ContractNo AND a.IsContractStatus = 1 LEFT OUTER JOIN
                         dbo.svMstClub AS d WITH (nowait, nolock) ON d.CompanyCode = a.CompanyCode AND d.ClubCode = a.ClubCode
WHERE        (a.IsActive = 1) AND (e.ProfitCenterCode = '200')

GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "a"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 135
               Right = 244
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "b"
            Begin Extent = 
               Top = 138
               Left = 38
               Bottom = 267
               Right = 237
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "e"
            Begin Extent = 
               Top = 6
               Left = 282
               Bottom = 135
               Right = 468
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "c"
            Begin Extent = 
               Top = 6
               Left = 506
               Bottom = 135
               Right = 677
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "d"
            Begin Extent = 
               Top = 6
               Left = 715
               Bottom = 135
               Right = 886
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 36
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
  ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'SvCustomerVehicleView'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'       Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'SvCustomerVehicleView'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'SvCustomerVehicleView'
GO


