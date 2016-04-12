USE [BIT_20130325]
GO

/****** Object:  View [dbo].[SvTrnMaintainSPKView]    Script Date: 11/13/2013 8:45:34 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[SvTrnMaintainSPKView]
AS
SELECT DISTINCT 
                         dbo.svTrnService.CompanyCode, dbo.svTrnService.BranchCode, dbo.svTrnService.ServiceNo, dbo.svTrnService.JobOrderNo, dbo.svTrnService.JobOrderDate, 
                         dbo.svTrnService.PoliceRegNo, dbo.svTrnService.ServiceBookNo, dbo.svTrnService.BasicModel, dbo.svTrnService.TransmissionType, 
                         dbo.svTrnService.ChassisCode, dbo.svTrnService.ChassisNo, dbo.svTrnService.EngineCode, dbo.svTrnService.EngineNo, dbo.svTrnService.ColorCode, 
                         dbo.svTrnService.CustomerCode, dbo.gnMstCustomer.CustomerName, dbo.svTrnService.CustomerCodeBill, custBill.CustomerName AS CustomerNameBill, 
                         dbo.svTrnService.Odometer, dbo.svTrnService.JobType, dbo.svTrnService.ServiceStatus, reffService.Description AS ServiceName, dbo.svTrnService.ForemanID, 
                         employee.EmployeeName AS ForemanName
FROM            dbo.svTrnService WITH (NOLOCK, NOWAIT) LEFT OUTER JOIN
                         dbo.gnMstCustomer ON dbo.gnMstCustomer.CompanyCode = dbo.svTrnService.CompanyCode AND 
                         dbo.gnMstCustomer.CustomerCode = dbo.svTrnService.CustomerCode LEFT OUTER JOIN
                         dbo.gnMstCustomer AS custBill ON custBill.CompanyCode = dbo.svTrnService.CompanyCode AND 
                         custBill.CustomerCode = dbo.svTrnService.CustomerCodeBill LEFT OUTER JOIN
                         dbo.gnMstEmployee AS employee ON employee.CompanyCode = dbo.svTrnService.CompanyCode AND employee.BranchCode = dbo.svTrnService.BranchCode AND 
                         employee.EmployeeID = dbo.svTrnService.ForemanID LEFT OUTER JOIN
                         dbo.svTrnSrvItem AS srvItem ON srvItem.CompanyCode = dbo.svTrnService.CompanyCode AND srvItem.BranchCode = dbo.svTrnService.BranchCode AND 
                         srvItem.ProductType = dbo.svTrnService.ProductType AND srvItem.ServiceNo = dbo.svTrnService.ServiceNo LEFT OUTER JOIN
                         dbo.svTrnSrvTask AS srvTask ON srvTask.CompanyCode = dbo.svTrnService.CompanyCode AND srvTask.BranchCode = dbo.svTrnService.BranchCode AND 
                         srvTask.ProductType = dbo.svTrnService.ProductType AND srvTask.ServiceNo = dbo.svTrnService.ServiceNo LEFT OUTER JOIN
                         dbo.svMstRefferenceService AS reffService ON reffService.CompanyCode = dbo.svTrnService.CompanyCode AND 
                         reffService.ProductType = dbo.svTrnService.ProductType AND reffService.RefferenceCode = dbo.svTrnService.ServiceStatus AND 
                         reffService.RefferenceType = 'SERVSTAS' LEFT OUTER JOIN
                         dbo.svTrnInvoice AS invoice ON invoice.CompanyCode = dbo.svTrnService.CompanyCode AND invoice.BranchCode = dbo.svTrnService.BranchCode AND 
                         invoice.ProductType = dbo.svTrnService.ProductType AND invoice.JobOrderNo = dbo.svTrnService.JobOrderNo LEFT OUTER JOIN
                         dbo.svTrnSrvVOR AS VOR ON VOR.CompanyCode = dbo.svTrnService.CompanyCode AND VOR.BranchCode = dbo.svTrnService.BranchCode AND 
                         VOR.ServiceNo = dbo.svTrnService.ServiceNo
WHERE        (dbo.svTrnService.ServiceType = 2) AND (dbo.svTrnService.ServiceStatus IN ('0', '1', '2', '3', '4', '5'))

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
         Begin Table = "svTrnService"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 135
               Right = 244
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "gnMstCustomer"
            Begin Extent = 
               Top = 6
               Left = 282
               Bottom = 135
               Right = 481
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "custBill"
            Begin Extent = 
               Top = 6
               Left = 519
               Bottom = 135
               Right = 718
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "employee"
            Begin Extent = 
               Top = 6
               Left = 756
               Bottom = 135
               Right = 978
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "srvItem"
            Begin Extent = 
               Top = 6
               Left = 1016
               Bottom = 135
               Right = 1187
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "srvTask"
            Begin Extent = 
               Top = 138
               Left = 38
               Bottom = 267
               Right = 209
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "reffService"
            Begin Extent = 
               Top = 138
               Left = 247
               Bottom = 267
               Right = 420
            End
            Displ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'SvTrnMaintainSPKView'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'ayFlags = 280
            TopColumn = 0
         End
         Begin Table = "invoice"
            Begin Extent = 
               Top = 138
               Left = 458
               Bottom = 267
               Right = 651
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "VOR"
            Begin Extent = 
               Top = 138
               Left = 689
               Bottom = 267
               Right = 861
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
      Begin ColumnWidths = 25
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'SvTrnMaintainSPKView'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'SvTrnMaintainSPKView'
GO


