USE [BIT_SBY]
GO

/****** Object:  View [dbo].[SvTaxInvoiceHQView]    Script Date: 10/9/2013 4:28:52 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[SvTaxInvoiceHQView]
AS
SELECT DISTINCT TOP (100) PERCENT trnFakturPajak.CompanyCode, trnFakturPajak.BranchCode, trnFakturPajak.FPJNo, trnFakturPajak.FPJDate,
                             (SELECT        TOP (1) InvoiceNo + ' (' + SUBSTRING(BranchCode, LEN(BranchCode) - 1, 3) + ')' AS Expr1
                               FROM            dbo.svTrnInvoice
                               WHERE        (CompanyCode = trnFakturPajak.CompanyCode) AND (FPJNo = trnFakturPajak.FPJNo) AND (IsLocked = 1)
                               ORDER BY BranchCode, InvoiceNo) + ' s/d ' +
                             (SELECT        TOP (1) InvoiceNo + ' (' + SUBSTRING(BranchCode, LEN(BranchCode) - 1, 3) + ')' AS Expr1
                               FROM            dbo.svTrnInvoice AS svTrnInvoice_3
                               WHERE        (CompanyCode = trnFakturPajak.CompanyCode) AND (FPJNo = trnFakturPajak.FPJNo) AND (IsLocked = 1)
                               ORDER BY InvoiceNo, BranchCode DESC) AS Invoice,
                             (SELECT        TOP (1) BranchCode
                               FROM            dbo.svTrnInvoice AS svTrnInvoice_2
                               WHERE        (CompanyCode = trnFakturPajak.CompanyCode) AND (FPJNo = trnFakturPajak.FPJNo) AND (IsLocked = 1)
                               ORDER BY BranchCode, InvoiceNo) AS BranchStart,
                             (SELECT        TOP (1) BranchCode
                               FROM            dbo.svTrnInvoice AS svTrnInvoice_1
                               WHERE        (CompanyCode = trnFakturPajak.CompanyCode) AND (FPJNo = trnFakturPajak.FPJNo) AND (IsLocked = 1)
                               ORDER BY BranchCode DESC) AS BranchEnd, trnFakturPajak.CustomerCode, trnFakturPajak.CustomerName, trnFakturPajak.CustomerCodeBill, 
                         trnFakturPajak.CustomerCode + ' - ' +
                             (SELECT        CustomerName
                               FROM            dbo.gnMstCustomer
                               WHERE        (CompanyCode = trnFakturPajak.CompanyCode) AND (CustomerCode = trnFakturPajak.CustomerCode)) AS Customer, 
                         trnFakturPajak.CustomerCodeBill + ' - ' +
                             (SELECT        CustomerName
                               FROM            dbo.gnMstCustomer AS gnMstCustomer_1
                               WHERE        (CompanyCode = trnFakturPajak.CompanyCode) AND (CustomerCode = trnFakturPajak.CustomerCodeBill)) AS CustomerBill
FROM            dbo.svTrnFakturPajak AS trnFakturPajak LEFT OUTER JOIN
                         dbo.svTrnInvoice AS trnInvoice ON trnInvoice.CompanyCode = trnFakturPajak.CompanyCode AND trnInvoice.FPJNo = trnFakturPajak.FPJNo AND 
                         trnInvoice.IsLocked = 1
WHERE        (trnFakturPajak.FPJNo LIKE 'FPH%')

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
         Begin Table = "trnFakturPajak"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 135
               Right = 223
            End
            DisplayFlags = 280
            TopColumn = 9
         End
         Begin Table = "trnInvoice"
            Begin Extent = 
               Top = 6
               Left = 261
               Bottom = 135
               Right = 454
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
      Begin ColumnWidths = 9
         Width = 284
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'SvTaxInvoiceHQView'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'SvTaxInvoiceHQView'
GO

