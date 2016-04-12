using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace SimDms.DataWarehouse.Helpers
{
    public class OmRpSalRgs046Xcl
    {
        DataSet dt0;
        Dictionary<int, string> parameter = new Dictionary<int, string>();

        #region Properties
        private string area, dealer, outlet, salesHead, salesCoordinator, salesMan;
        private int rowIndex = 0;

        private int totalSalesHead = 0, totalSalesCo = 0, totalDealer = 0, totalDealerJabodetabek = 0, totalOutlet = 0, totalMonth = 0, totalDealerGT = 0, totalSalesman = 0, totalArea = 0;
        private int totalRowSalesCo = 0, totalRowSalesHead = 0, totalRowBranchHead = 0, totalRowDealer = 0;
        private int dealerWidth = 0, outletWidth = 0, salesHeadWidth = 0, salesCoordinatorWidth = 0, salesManWidth = 0, modelWidth = 0, totalDetailWidth = 0, gradeWidth = 0, dataWidth = 0, totalWidth = 0, avgWidth = 0;

        private ExcelCellStyle totalColor = ExcelCellStyle.RightBorderedStandardNumber;
        private ExcelCellStyle totalRedColor = ExcelCellStyle.RightBorderedStandardRedNumber;

        private string fileName = "";
        private decimal averageMonth = 0, averageSupplier = 0, averageSalesHead = 0, averageBranch = 0, averageCompany = 0, averageArea = 0, averageAreaJabodetabek = 0, averageGrandTotal = 0;
        #endregion

        public OmRpSalRgs046Xcl(DataSet ds, Dictionary<int, string> param, string saveFileName)
        {
            dt0 = ds;
            parameter = param;
            fileName = saveFileName;
        }

        public void CreateReport(string pserver)
        {
            DataTable dtHeader = dt0.Tables[0];
            DataTable dtSalesmanFullGrade = dt0.Tables[1];
            DataTable dtGradeList = dt0.Tables[2];
            DataTable dtData = dt0.Tables[3];
            DataTable dtNSDS = new DataTable();
            //if (parameter[9] == "RETAIL" || parameter[9] == "SALES" || parameter[9] == "FPOL")
                //dtNSDS = dt0.Tables[4];

            area = dtData.Rows[0]["Area"].ToString();
            dealer = dtData.Rows[0]["CompanyName"].ToString();
            outlet = dtData.Rows[0]["BranchName"].ToString();
            salesHead = dtData.Rows[0]["SalesHeadName"].ToString();
            salesCoordinator = dtData.Rows[0]["SalesCoordinatorName"].ToString();
            salesMan = dtData.Rows[0]["SalesmanName"].ToString();

            StringBuilder sb = new StringBuilder();
            sb.Append("(by Outlet, Sales Head, Sales Coordinator, Wiraniaga, Grade)");
            sb.Append(" - " + parameter[9].ToString());

            string title = sb.ToString();

            //Pemetaan ukuran column
            dealerWidth = 1;
            outletWidth = 1;
            salesHeadWidth = 1;
            salesCoordinatorWidth = 1;
            salesManWidth = 1;
            gradeWidth = 1;
            dataWidth = 1;
            totalWidth = 1;
            avgWidth = 1;


            totalDetailWidth = dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth;

            int indentTitleDesc = 6;

            ExcelFileWriter excelReport = new ExcelFileWriter(fileName, "PRODUCTIVITY TREND REPORT", "OmRpSalRgs046", pserver);
            excelReport.SettingColumnWidth(1); //dealer
            excelReport.SettingColumnWidth(2); // Outlet
            excelReport.SettingColumnWidth(2); // SalesHead
            excelReport.SettingColumnWidth(2); // SalesCoordinator
            excelReport.SettingColumnWidth(2); // Salesman
            excelReport.SettingColumnWidth(1); // Grade

            #region Isi Header Complete
            excelReport.SetCellValue("PRODUCTIVITY TREND REPORT ", 0, 0, 1, dealerWidth + outletWidth + modelWidth + indentTitleDesc + indentTitleDesc + 3, ExcelCellStyle.Header, false, "20");
            excelReport.SetCellValue(title, 1, 0, 1, dealerWidth + outletWidth + modelWidth + indentTitleDesc + indentTitleDesc + 3, ExcelCellStyle.Header2, false);
            excelReport.SetCellValue("Periode ", 2, 0, 1, dealerWidth, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue(": " + parameter[1], 2, dealerWidth, 1, indentTitleDesc, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue("Branch Manager", 2, dealerWidth + outletWidth + modelWidth + indentTitleDesc, 1, 3, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue(": " + parameter[2], 2, dealerWidth + outletWidth + modelWidth + indentTitleDesc + 3, 1, indentTitleDesc, ExcelCellStyle.LeftBold);

            excelReport.SetCellValue("Area", 3, 0, 1, dealerWidth, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue(": " + parameter[3], 3, dealerWidth, 1, indentTitleDesc, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue("Sales Head", 3, dealerWidth + outletWidth + modelWidth + indentTitleDesc, 1, 3, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue(": " + parameter[4], 3, dealerWidth + outletWidth + modelWidth + indentTitleDesc + 3, 1, indentTitleDesc, ExcelCellStyle.LeftBold);

            excelReport.SetCellValue("Dealer", 4, 0, 1, dealerWidth, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue(": " + parameter[5], 4, dealerWidth, 1, indentTitleDesc, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue("Sales Coordinator", 4, dealerWidth + outletWidth + modelWidth + indentTitleDesc, 1, 3, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue(": " + parameter[6], 4, dealerWidth + outletWidth + modelWidth + indentTitleDesc + 3, 1, indentTitleDesc, ExcelCellStyle.LeftBold);

            excelReport.SetCellValue("Outlet", 5, 0, 1, dealerWidth, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue(": " + parameter[7], 5, dealerWidth, 1, indentTitleDesc, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue("Wiraniaga", 5, dealerWidth + outletWidth + modelWidth + indentTitleDesc, 1, 3, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue(": " + parameter[8], 5, dealerWidth + outletWidth + modelWidth + indentTitleDesc + 3, 1, indentTitleDesc, ExcelCellStyle.LeftBold);

            excelReport.SetCellValue("Dealer", 6, 0, 2, dealerWidth, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Outlet", 6, 0 + dealerWidth, 2, outletWidth, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Sales Head", 6, 0 + dealerWidth + outletWidth, 2, salesHeadWidth, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Sales Coordinator", 6, 0 + dealerWidth + outletWidth + salesHeadWidth, 2, salesCoordinatorWidth, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Wiraniaga", 6, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth, 2, salesManWidth, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Grade", 6, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth, 2, gradeWidth, ExcelCellStyle.CenterBorderedBold);


            #region Isi header tahun
            string yearMonth = dtHeader.Rows[0]["YearMonth"].ToString().Substring(0, 4);
            int yearMonthWidth = 0;
            int lastYearMonthWidth = 0;
            foreach (DataRow row in dtHeader.Rows)
            {
                if (row["YearMonth"].ToString().Substring(0, 4) == yearMonth)
                    yearMonthWidth += dataWidth;
                else
                {
                    excelReport.SetCellValue(yearMonth.ToString(), 6, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + lastYearMonthWidth, 1, yearMonthWidth, ExcelCellStyle.CenterBorderedBold, false);
                    yearMonth = row["YearMonth"].ToString().Substring(0, 4);
                    lastYearMonthWidth += yearMonthWidth;
                    yearMonthWidth = 1;
                }
            }

            excelReport.SetCellValue(yearMonth.ToString(), 6, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + lastYearMonthWidth, 1, yearMonthWidth, ExcelCellStyle.CenterBorderedBold, false);
            #endregion

            excelReport.SetCellValue("Total", 6, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + lastYearMonthWidth + yearMonthWidth, 2, totalWidth, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("AVG", 6, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + lastYearMonthWidth + yearMonthWidth + totalWidth, 2, avgWidth, ExcelCellStyle.CenterBorderedBold);

            #region Isi Header Bulan
            yearMonth = dtHeader.Rows[0]["YearMonth"].ToString().Substring(4, 2);
            yearMonthWidth = 0;
            foreach (DataRow row in dtHeader.Rows)
            {
                DateTime bulan = new DateTime(Convert.ToInt32(row["YearMonth"].ToString().Substring(0, 4)), Convert.ToInt32(row["YearMonth"].ToString().Substring(4, 2)), 1);
                excelReport.SetCellValue(bulan.ToString("MM"), 7, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + yearMonthWidth, 1, dataWidth, ExcelCellStyle.CenterBorderedBold, false);
                yearMonthWidth += dataWidth;
            }
            #endregion
            #endregion

            Dictionary<int, decimal> totalGradeCo = new Dictionary<int, decimal>();
            Dictionary<int, decimal> totalGradeSH = new Dictionary<int, decimal>();
            Dictionary<int, decimal> totalGradeOu = new Dictionary<int, decimal>();
            Dictionary<int, decimal> totalGradeDl = new Dictionary<int, decimal>();
            Dictionary<int, decimal> totalGradeAr = new Dictionary<int, decimal>();
            Dictionary<int, decimal> totalGradeArJabodetabek = new Dictionary<int, decimal>();
            Dictionary<int, decimal> totalGradeGT = new Dictionary<int, decimal>();

            int currentGrade = 1;
            int index = 0;

            List<List<int>> gradeEnable = new List<List<int>>();

            totalSalesCo++;
            totalSalesHead++;
            totalOutlet++;
            totalDealer++;

            excelReport.RestorePoint(8 + rowIndex, RestorePoint.CompanyCodeRestorePoint.ToString());
            excelReport.RestorePoint(8 + rowIndex, RestorePoint.OutletRestorePoint.ToString());
            excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesHeadRestorePoint.ToString());
            excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesCoordinatorrestorePoint.ToString());

            foreach (DataRow row in dtData.Rows)
            {
                int joinMonth = Convert.ToDateTime(row["JoinDate"].ToString()).Month;
                int joinYear = Convert.ToDateTime(row["JoinDate"].ToString()).Year;
                int resignMonth = Convert.ToDateTime(row["ResignDate"].ToString()).Month;
                int resignYear = Convert.ToDateTime(row["ResignDate"].ToString()).Year;

                totalMonth = (resignYear - joinYear == 1) ? (13 - joinMonth) + resignMonth :
                    (resignYear - joinYear >= 2) ? (13 - joinMonth) + (12 * (resignYear - joinYear)) + resignMonth :
                    resignMonth - joinMonth + 1;
                if (totalMonth > 5)
                    averageSupplier = 0;
                gradeEnable = new List<List<int>>();
                currentGrade = 1;
                index = 0;
                DataRow[] gradeListEmployee = dtGradeList.Select(string.Format("CompanyCode = '{0}' and BranchCode = '{1}' and EmployeeId = '{2}'",
                    row["CompanyCode"].ToString(), row["BranchCode"].ToString(), row["SalesmanID"].ToString()));

                //Untuk buat restore point seluruh grade yang ada
                foreach (DataRow rowGrade in dtSalesmanFullGrade.Rows)
                {
                    gradeEnable.Add(new List<int>());
                }

                if (area == row["Area"].ToString() &&
        dealer == row["CompanyName"].ToString() &&
        outlet == row["BranchName"].ToString() &&
        salesHead == row["SalesHeadName"].ToString() &&
        salesCoordinator != row["SalesCoordinatorName"].ToString())
                {
                    #region SalesCoordinator Total
                    excelReport.SetCellValue("Total", 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth, 1, salesManWidth + gradeWidth, ExcelCellStyle.YellowTotal);

                    int total = 0, avg = 0;
                    for (int j = 0; j < totalGradeCo.Count; j++)
                        total += Convert.ToInt32(totalGradeCo[j].ToString());
                    avg = Convert.ToInt32(total / (averageMonth != 0 ? averageMonth : 1));

                    for (int i = 0; i < totalGradeCo.Count; i++)
                    {
                        excelReport.SetCellValue(totalGradeCo[i].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + (dataWidth * i), 1, dataWidth, (Convert.ToInt32(totalGradeCo[i]) < avg && totalGradeCo[i] != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                    }
                    excelReport.SetCellValue(total.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count, 1, dataWidth, ExcelCellStyle.YellowTotalNumber, true);
                    excelReport.SetCellValue(avg.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count + dataWidth, 1, dataWidth, ExcelCellStyle.YellowTotalNumber, true);
                    totalRowSalesCo++;
                    totalRowSalesHead++;
                    totalRowBranchHead++;
                    totalRowDealer++;
                    rowIndex++;

                    total = 0;

                    for (int j = 0; j < totalGradeCo.Count; j++)
                        total += Convert.ToInt32(totalGradeCo[j] / (totalSalesman != 0 ? totalSalesman : 1));

                    avg = Convert.ToInt32(total / (averageMonth != 0 ? averageMonth : 1));
                    excelReport.SetCellValue("AVG", 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth, 1, salesManWidth + gradeWidth, ExcelCellStyle.PurpleTotal);
                    for (int i = 0; i < totalGradeCo.Count; i++)
                    {
                        decimal average = totalGradeCo[i] / (totalSalesman != 0 ? totalSalesman : 1);
                        excelReport.SetCellValue(Convert.ToInt32(average).ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + (dataWidth * i), 1, dataWidth, (Convert.ToInt32(average) < avg && average != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        //total += Convert.ToInt32(average);
                    }
                    excelReport.SetCellValue(total.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count, 1, dataWidth, ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(avg.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count + dataWidth, 1, dataWidth, ExcelCellStyle.PurpleTotalNumber, true);

                    totalRowSalesCo++;
                    totalRowSalesHead++;
                    totalRowBranchHead++;
                    totalRowDealer++;
                    rowIndex++;
                    averageMonth = 0;
                    excelReport.ReplaceRestorePoint(RestorePoint.SalesCoordinatorrestorePoint.ToString(), salesCoordinator.ToString(), 0 + dealerWidth + outletWidth + salesHeadWidth, totalRowSalesCo, salesManWidth, ExcelCellStyle.LeftBorderedTop);
                    #endregion

                    excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesCoordinatorrestorePoint.ToString());
                    totalGradeCo = new Dictionary<int, decimal>();
                    totalRowSalesCo = 0;
                    totalSalesman = 0;
                    totalSalesCo++;
                    averageMonth = 0;
                    salesCoordinator = row["SalesCoordinatorName"].ToString();
                }
                else if (area == row["Area"].ToString() &&
    dealer == row["CompanyName"].ToString() &&
    outlet == row["BranchName"].ToString() &&
    salesHead != row["SalesHeadName"].ToString())
                {
                    #region SalesCoordinator Total
                    excelReport.SetCellValue("Total", 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth, 1, salesManWidth + gradeWidth, ExcelCellStyle.YellowTotal);

                    int total = 0, avg = 0;
                    for (int j = 0; j < totalGradeCo.Count; j++)
                        total += Convert.ToInt32(totalGradeCo[j].ToString());
                    avg = Convert.ToInt32(total / (averageMonth != 0 ? averageMonth : 1));

                    for (int i = 0; i < totalGradeCo.Count; i++)
                    {
                        excelReport.SetCellValue(totalGradeCo[i].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + (dataWidth * i), 1, dataWidth, (Convert.ToInt32(totalGradeCo[i]) < avg && totalGradeCo[i] != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                    }
                    excelReport.SetCellValue(total.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count, 1, dataWidth, ExcelCellStyle.YellowTotalNumber, true);
                    excelReport.SetCellValue(avg.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count + dataWidth, 1, dataWidth, ExcelCellStyle.YellowTotalNumber, true);
                    totalRowSalesCo++;
                    totalRowSalesHead++;
                    totalRowBranchHead++;
                    totalRowDealer++;
                    rowIndex++;

                    total = 0;
                    for (int j = 0; j < totalGradeCo.Count; j++)
                        total += Convert.ToInt32(totalGradeCo[j] / (totalSalesman != 0 ? totalSalesman : 1));

                    avg = Convert.ToInt32(total / (averageMonth != 0 ? averageMonth : 1));
                    excelReport.SetCellValue("AVG", 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth, 1, salesManWidth + gradeWidth, ExcelCellStyle.PurpleTotal);
                    for (int i = 0; i < totalGradeCo.Count; i++)
                    {
                        decimal average = totalGradeCo[i] / (totalSalesman != 0 ? totalSalesman : 1);
                        excelReport.SetCellValue(Convert.ToInt32(average).ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + (dataWidth * i), 1, dataWidth, (Convert.ToInt32(average) < avg && average != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        //total += Convert.ToInt32(average);
                    }
                    excelReport.SetCellValue(total.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count, 1, dataWidth, ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(avg.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count + dataWidth, 1, dataWidth, ExcelCellStyle.PurpleTotalNumber, true);

                    totalRowSalesCo++;
                    totalRowSalesHead++;
                    totalRowBranchHead++;
                    totalRowDealer++;
                    rowIndex++;
                    averageMonth = 0;
                    excelReport.ReplaceRestorePoint(RestorePoint.SalesCoordinatorrestorePoint.ToString(), salesCoordinator.ToString(), 0 + dealerWidth + outletWidth + salesHeadWidth, totalRowSalesCo, salesManWidth, ExcelCellStyle.LeftBorderedTop);
                    #endregion

                    #region SalesHead
                    excelReport.SetCellValue("Total", 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth, 1, salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.YellowTotal);
                    total = avg = 0;
                    for (int j = 0; j < totalGradeSH.Count; j++)
                        total += Convert.ToInt32(totalGradeSH[j].ToString());
                    avg = Convert.ToInt32(total / (averageSalesHead != 0 ? averageSalesHead : 1));

                    for (int i = 0; i < totalGradeSH.Count; i++)
                    {
                        excelReport.SetCellValue(totalGradeSH[i].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + (dataWidth * i), 1, dataWidth, (Convert.ToInt32(totalGradeSH[i]) < avg && totalGradeSH[i] != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                    }
                    excelReport.SetCellValue(total.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count, 1, dataWidth, ExcelCellStyle.YellowTotalNumber, true);
                    excelReport.SetCellValue(avg.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count + dataWidth, 1, dataWidth, ExcelCellStyle.YellowTotalNumber, true);

                    totalRowSalesCo++;
                    totalRowSalesHead++;
                    totalRowBranchHead++;
                    totalRowDealer++;
                    rowIndex++;

                    total = 0;
                    for (int j = 0; j < totalGradeSH.Count; j++)
                        total += Convert.ToInt32(totalGradeSH[j] / (totalSalesCo != 0 ? totalSalesCo : 1));

                    avg = Convert.ToInt32(total / (averageSalesHead != 0 ? averageSalesHead : 1));
                    excelReport.SetCellValue("AVG", 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth, 1, salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.PurpleTotal);
                    for (int i = 0; i < totalGradeSH.Count; i++)
                    {
                        decimal average = totalGradeSH[i] / (totalSalesCo != 0 ? totalSalesCo : 1);
                        excelReport.SetCellValue(Convert.ToInt32(average).ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + (dataWidth * i), 1, dataWidth, (Convert.ToInt32(average) < avg && average != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        //total += Convert.ToInt32(average);
                    }
                    excelReport.SetCellValue(total.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count, 1, dataWidth, ExcelCellStyle.PurpleTotalNumber, true);

                    excelReport.SetCellValue(avg.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count + dataWidth, 1, dataWidth, ExcelCellStyle.PurpleTotalNumber, true);

                    totalRowSalesCo++;
                    totalRowSalesHead++;
                    totalRowBranchHead++;
                    totalRowDealer++;
                    rowIndex++;
                    averageSalesHead = 0;
                    excelReport.ReplaceRestorePoint(RestorePoint.SalesHeadRestorePoint.ToString(), salesHead.ToString(), 0 + dealerWidth + outletWidth, totalRowSalesHead, salesHeadWidth, ExcelCellStyle.LeftBorderedTop);
                    totalSalesHead++;
                    #endregion

                    excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesHeadRestorePoint.ToString());
                    excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesCoordinatorrestorePoint.ToString());

                    totalGradeCo = new Dictionary<int, decimal>();
                    totalGradeSH = new Dictionary<int, decimal>();
                    totalRowSalesCo = 0;
                    totalRowSalesHead = 0;
                    totalSalesman = 0;
                    totalSalesCo = 0;
                    totalSalesCo++;
                    totalSalesHead++;
                    averageSalesHead = 0;
                    averageMonth = 0;
                    salesHead = row["SalesHeadName"].ToString();
                    salesCoordinator = row["SalesCoordinatorName"].ToString();
                }
                else if (area == row["Area"].ToString() &&
                    dealer == row["CompanyName"].ToString() &&
                    outlet != row["BranchName"].ToString())
                {
                    #region SalesCoordinator Total
                    excelReport.SetCellValue("Total", 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth, 1, salesManWidth + gradeWidth, ExcelCellStyle.YellowTotal);

                    int total = 0, avg = 0;
                    for (int j = 0; j < totalGradeCo.Count; j++)
                        total += Convert.ToInt32(totalGradeCo[j].ToString());
                    avg = Convert.ToInt32(total / (averageMonth != 0 ? averageMonth : 1));

                    for (int i = 0; i < totalGradeCo.Count; i++)
                    {
                        excelReport.SetCellValue(totalGradeCo[i].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + (dataWidth * i), 1, dataWidth, (Convert.ToInt32(totalGradeCo[i]) < avg && totalGradeCo[i] != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                    }
                    excelReport.SetCellValue(total.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count, 1, dataWidth, ExcelCellStyle.YellowTotalNumber, true);
                    excelReport.SetCellValue(avg.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count + dataWidth, 1, dataWidth, ExcelCellStyle.YellowTotalNumber, true);
                    totalRowSalesCo++;
                    totalRowSalesHead++;
                    totalRowBranchHead++;
                    totalRowDealer++;
                    rowIndex++;

                    total = 0;
                    for (int j = 0; j < totalGradeCo.Count; j++)
                        total += Convert.ToInt32(totalGradeCo[j] / (totalSalesman != 0 ? totalSalesman : 1));

                    avg = Convert.ToInt32(total / (averageMonth != 0 ? averageMonth : 1));
                    excelReport.SetCellValue("AVG", 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth, 1, salesManWidth + gradeWidth, ExcelCellStyle.PurpleTotal);
                    for (int i = 0; i < totalGradeCo.Count; i++)
                    {
                        decimal average = totalGradeCo[i] / totalSalesman;
                        excelReport.SetCellValue(Convert.ToInt32(average).ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + (dataWidth * i), 1, dataWidth, (Convert.ToInt32(average) < avg && average != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        //total += Convert.ToInt32(average);
                    }
                    excelReport.SetCellValue(total.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count, 1, dataWidth, ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(avg.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count + dataWidth, 1, dataWidth, ExcelCellStyle.PurpleTotalNumber, true);

                    totalRowSalesCo++;
                    totalRowSalesHead++;
                    totalRowBranchHead++;
                    totalRowDealer++;
                    rowIndex++;
                    averageMonth = 0;
                    excelReport.ReplaceRestorePoint(RestorePoint.SalesCoordinatorrestorePoint.ToString(), salesCoordinator.ToString(), 0 + dealerWidth + outletWidth + salesHeadWidth, totalRowSalesCo, salesManWidth, ExcelCellStyle.LeftBorderedTop);
                    #endregion

                    #region SalesHead
                    excelReport.SetCellValue("Total", 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth, 1, salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.YellowTotal);
                    total = avg = 0;
                    for (int j = 0; j < totalGradeSH.Count; j++)
                        total += Convert.ToInt32(totalGradeSH[j].ToString());
                    avg = Convert.ToInt32(total / averageSalesHead);

                    for (int i = 0; i < totalGradeSH.Count; i++)
                    {
                        excelReport.SetCellValue(totalGradeSH[i].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + (dataWidth * i), 1, dataWidth, (Convert.ToInt32(totalGradeSH[i]) < avg && totalGradeSH[i] != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                    }
                    excelReport.SetCellValue(total.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count, 1, dataWidth, ExcelCellStyle.YellowTotalNumber, true);
                    excelReport.SetCellValue(avg.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count + dataWidth, 1, dataWidth, ExcelCellStyle.YellowTotalNumber, true);

                    totalRowSalesCo++;
                    totalRowSalesHead++;
                    totalRowBranchHead++;
                    totalRowDealer++;
                    rowIndex++;

                    total = 0;
                    for (int j = 0; j < totalGradeSH.Count; j++)
                        total += Convert.ToInt32(totalGradeSH[j] / totalSalesCo);

                    avg = Convert.ToInt32(total / averageSalesHead);
                    excelReport.SetCellValue("AVG", 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth, 1, salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.PurpleTotal);
                    for (int i = 0; i < totalGradeSH.Count; i++)
                    {
                        decimal average = totalGradeSH[i] / totalSalesCo;
                        excelReport.SetCellValue(Convert.ToInt32(average).ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + (dataWidth * i), 1, dataWidth, (Convert.ToInt32(average) < avg && average != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        //total += Convert.ToInt32(average);
                    }
                    excelReport.SetCellValue(total.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count, 1, dataWidth, ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(avg.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count + dataWidth, 1, dataWidth, ExcelCellStyle.PurpleTotalNumber, true);

                    totalRowSalesCo++;
                    totalRowSalesHead++;
                    totalRowBranchHead++;
                    totalRowDealer++;
                    rowIndex++;
                    averageSalesHead = 0;
                    excelReport.ReplaceRestorePoint(RestorePoint.SalesHeadRestorePoint.ToString(), salesHead.ToString(), 0 + dealerWidth + outletWidth, totalRowSalesHead, salesHeadWidth, ExcelCellStyle.LeftBorderedTop);
                    #endregion

                    #region Outlet Total
                    excelReport.SetCellValue("Total", 8 + rowIndex, 0 + dealerWidth + outletWidth, 1, salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.YellowTotal);
                    total = avg = 0;
                    for (int j = 0; j < totalGradeOu.Count; j++)
                        total += Convert.ToInt32(totalGradeOu[j].ToString());
                    avg = Convert.ToInt32(total / (averageBranch != 0 ? averageBranch : 1));

                    for (int i = 0; i < totalGradeOu.Count; i++)
                    {
                        excelReport.SetCellValue(totalGradeOu[i].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + (dataWidth * i), 1, dataWidth, (Convert.ToInt32(totalGradeOu[i]) < avg && totalGradeOu[i] != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                    }
                    excelReport.SetCellValue(total.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count, 1, dataWidth, ExcelCellStyle.YellowTotalNumber, true);
                    excelReport.SetCellValue(avg.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count + dataWidth, 1, dataWidth, ExcelCellStyle.YellowTotalNumber, true);

                    totalRowSalesCo++;
                    totalRowSalesHead++;
                    totalRowBranchHead++;
                    totalRowDealer++;
                    rowIndex++;

                    total = 0;
                    for (int j = 0; j < totalGradeOu.Count; j++)
                        total += Convert.ToInt32(totalGradeOu[j] / (totalSalesHead != 0 ? totalSalesHead : 1));

                    avg = Convert.ToInt32(total / (averageBranch != 0 ? averageBranch : 1));
                    excelReport.SetCellValue("AVG", 8 + rowIndex, 0 + dealerWidth + outletWidth, 1, salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.PurpleTotal);
                    for (int i = 0; i < totalGradeOu.Count; i++)
                    {
                        decimal average = totalGradeOu[i] / (totalSalesHead != 0 ? totalSalesHead : 1);
                        excelReport.SetCellValue(Convert.ToInt32(average).ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + (dataWidth * i), 1, dataWidth, (Convert.ToInt32(average) < avg && average != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        //total += Convert.ToInt32(average);
                    }
                    excelReport.SetCellValue(total.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count, 1, dataWidth, ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(avg.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count + dataWidth, 1, dataWidth, ExcelCellStyle.PurpleTotalNumber, true);

                    totalRowSalesCo++;
                    totalRowSalesHead++;
                    totalRowBranchHead++;
                    totalRowDealer++;
                    rowIndex++;
                    averageBranch = 0;
                    excelReport.ReplaceRestorePoint(RestorePoint.OutletRestorePoint.ToString(), outlet.ToString(), 0 + dealerWidth, totalRowBranchHead, outletWidth, ExcelCellStyle.LeftBorderedTop);
                    totalDealer++;
                    #endregion

                    excelReport.RestorePoint(8 + rowIndex, RestorePoint.OutletRestorePoint.ToString());
                    excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesHeadRestorePoint.ToString());
                    excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesCoordinatorrestorePoint.ToString());

                    totalGradeOu = new Dictionary<int, decimal>();
                    totalGradeCo = new Dictionary<int, decimal>();
                    totalGradeSH = new Dictionary<int, decimal>();
                    totalRowBranchHead = 0;
                    totalSalesHead = 0;
                    totalRowSalesCo = 0;
                    totalRowSalesHead = 0;
                    totalSalesman = 0;
                    totalSalesCo++;
                    totalSalesHead++;
                    totalOutlet++;
                    averageSalesHead = 0;
                    averageMonth = 0;
                    averageBranch = 0;
                    salesHead = row["SalesHeadName"].ToString();
                    salesCoordinator = row["SalesCoordinatorName"].ToString();
                    outlet = row["BranchName"].ToString();
                }
                else if (area == row["Area"].ToString() &&
               dealer != row["CompanyName"].ToString())
                {
                    #region SalesCoordinator Total
                    excelReport.SetCellValue("Total", 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth, 1, salesManWidth + gradeWidth, ExcelCellStyle.YellowTotal);

                    int total = 0, avg = 0;
                    for (int j = 0; j < totalGradeCo.Count; j++)
                        total += Convert.ToInt32(totalGradeCo[j].ToString());
                    avg = Convert.ToInt32(total / (averageMonth != 0 ? averageMonth : 1));

                    for (int i = 0; i < totalGradeCo.Count; i++)
                    {
                        excelReport.SetCellValue(totalGradeCo[i].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + (dataWidth * i), 1, dataWidth, (Convert.ToInt32(totalGradeCo[i]) < avg && totalGradeCo[i] != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                    }
                    excelReport.SetCellValue(total.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count, 1, dataWidth, ExcelCellStyle.YellowTotalNumber, true);
                    excelReport.SetCellValue(avg.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count + dataWidth, 1, dataWidth, ExcelCellStyle.YellowTotalNumber, true);
                    totalRowSalesCo++;
                    totalRowSalesHead++;
                    totalRowBranchHead++;
                    totalRowDealer++;
                    rowIndex++;

                    total = 0;
                    for (int j = 0; j < totalGradeCo.Count; j++)
                        total += Convert.ToInt32(totalGradeCo[j] / (totalSalesman != 0 ? totalSalesman : 1));

                    avg = Convert.ToInt32(total / (averageMonth != 0 ? averageMonth : 1));

                    excelReport.SetCellValue("AVG", 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth, 1, salesManWidth + gradeWidth, ExcelCellStyle.PurpleTotal);
                    for (int i = 0; i < totalGradeCo.Count; i++)
                    {
                        decimal average = totalGradeCo[i] / (totalSalesman != 0 ? totalSalesman : 1);
                        excelReport.SetCellValue(Convert.ToInt32(average).ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + (dataWidth * i), 1, dataWidth, (Convert.ToInt32(average) < avg && average != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        //total += Convert.ToInt32(average);
                    }
                    excelReport.SetCellValue(total.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count, 1, dataWidth, ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(avg.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count + dataWidth, 1, dataWidth, ExcelCellStyle.PurpleTotalNumber, true);

                    totalRowSalesCo++;
                    totalRowSalesHead++;
                    totalRowBranchHead++;
                    totalRowDealer++;
                    rowIndex++;
                    averageMonth = 0;
                    excelReport.ReplaceRestorePoint(RestorePoint.SalesCoordinatorrestorePoint.ToString(), salesCoordinator.ToString(), 0 + dealerWidth + outletWidth + salesHeadWidth, totalRowSalesCo, salesManWidth, ExcelCellStyle.LeftBorderedTop);
                    #endregion

                    #region SalesHead
                    excelReport.SetCellValue("Total", 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth, 1, salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.YellowTotal);
                    total = avg = 0;
                    for (int j = 0; j < totalGradeSH.Count; j++)
                        total += Convert.ToInt32(totalGradeSH[j].ToString());
                    avg = Convert.ToInt32(total / (averageSalesHead != 0 ? averageSalesHead : 1));

                    for (int i = 0; i < totalGradeSH.Count; i++)
                    {
                        excelReport.SetCellValue(totalGradeSH[i].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + (dataWidth * i), 1, dataWidth, (Convert.ToInt32(totalGradeSH[i]) < avg && totalGradeSH[i] != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                    }
                    excelReport.SetCellValue(total.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count, 1, dataWidth, ExcelCellStyle.YellowTotalNumber, true);
                    excelReport.SetCellValue(avg.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count + dataWidth, 1, dataWidth, ExcelCellStyle.YellowTotalNumber, true);

                    totalRowSalesCo++;
                    totalRowSalesHead++;
                    totalRowBranchHead++;
                    totalRowDealer++;
                    rowIndex++;

                    total = 0;
                    for (int j = 0; j < totalGradeSH.Count; j++)
                        total += Convert.ToInt32(totalGradeSH[j] / (totalSalesCo != 0 ? totalSalesCo : 1));

                    avg = Convert.ToInt32(total / (averageSalesHead != 0 ? averageSalesHead : 1));
                    excelReport.SetCellValue("AVG", 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth, 1, salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.PurpleTotal);
                    for (int i = 0; i < totalGradeSH.Count; i++)
                    {
                        decimal average = totalGradeSH[i] / (totalSalesCo != 0 ? totalSalesCo : 1);
                        excelReport.SetCellValue(Convert.ToInt32(average).ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + (dataWidth * i), 1, dataWidth, (Convert.ToInt32(average) < avg && average != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        //total += Convert.ToInt32(average);
                    }
                    excelReport.SetCellValue(total.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count, 1, dataWidth, ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(avg.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count + dataWidth, 1, dataWidth, ExcelCellStyle.PurpleTotalNumber, true);

                    totalRowSalesCo++;
                    totalRowSalesHead++;
                    totalRowBranchHead++;
                    totalRowDealer++;
                    rowIndex++;
                    averageSalesHead = 0;
                    excelReport.ReplaceRestorePoint(RestorePoint.SalesHeadRestorePoint.ToString(), salesHead.ToString(), 0 + dealerWidth + outletWidth, totalRowSalesHead, salesHeadWidth, ExcelCellStyle.LeftBorderedTop);
                    #endregion

                    #region Outlet Total
                    excelReport.SetCellValue("Total", 8 + rowIndex, 0 + dealerWidth + outletWidth, 1, salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.YellowTotal);
                    total = avg = 0;
                    for (int j = 0; j < totalGradeOu.Count; j++)
                        total += Convert.ToInt32(totalGradeOu[j].ToString());
                    avg = Convert.ToInt32(total / (averageBranch != 0 ? averageBranch : 1));

                    for (int i = 0; i < totalGradeOu.Count; i++)
                    {
                        excelReport.SetCellValue(totalGradeOu[i].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + (dataWidth * i), 1, dataWidth, (Convert.ToInt32(totalGradeOu[i]) < avg && totalGradeOu[i] != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                    }
                    excelReport.SetCellValue(total.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count, 1, dataWidth, ExcelCellStyle.YellowTotalNumber, true);
                    excelReport.SetCellValue(avg.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count + dataWidth, 1, dataWidth, ExcelCellStyle.YellowTotalNumber, true);

                    totalRowSalesCo++;
                    totalRowSalesHead++;
                    totalRowBranchHead++;
                    totalRowDealer++;
                    rowIndex++;

                    total = 0;
                    for (int j = 0; j < totalGradeOu.Count; j++)
                        total += Convert.ToInt32(totalGradeOu[j] / (totalSalesHead != 0 ? totalSalesHead : 1));

                    avg = Convert.ToInt32(total / (averageBranch != 0 ? averageBranch : 1));
                    excelReport.SetCellValue("AVG", 8 + rowIndex, 0 + dealerWidth + outletWidth, 1, salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.PurpleTotal);
                    for (int i = 0; i < totalGradeOu.Count; i++)
                    {
                        decimal average = totalGradeOu[i] / (totalSalesHead != 0 ? totalSalesHead : 1);
                        excelReport.SetCellValue(Convert.ToInt32(average).ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + (dataWidth * i), 1, dataWidth, (Convert.ToInt32(average) < avg && average != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        //total += Convert.ToInt32(average);
                    }
                    excelReport.SetCellValue(total.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count, 1, dataWidth, ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(avg.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count + dataWidth, 1, dataWidth, ExcelCellStyle.PurpleTotalNumber, true);

                    totalRowSalesCo++;
                    totalRowSalesHead++;
                    totalRowBranchHead++;
                    totalRowDealer++;
                    rowIndex++;
                    averageBranch = 0;
                    excelReport.ReplaceRestorePoint(RestorePoint.OutletRestorePoint.ToString(), outlet.ToString(), 0 + dealerWidth, totalRowBranchHead, outletWidth, ExcelCellStyle.LeftBorderedTop);
                    totalDealer++;
                    #endregion

                    #region Dealer Total
                    excelReport.SetCellValue("Total", 8 + rowIndex, 0 + dealerWidth, 1, outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.YellowTotal);
                    total = avg = 0;
                    for (int j = 0; j < totalGradeDl.Count; j++)
                        total += Convert.ToInt32(totalGradeDl[j].ToString());
                    avg = Convert.ToInt32(total / (averageCompany != 0 ? averageCompany : 1));

                    for (int i = 0; i < totalGradeDl.Count; i++)
                    {
                        excelReport.SetCellValue(totalGradeDl[i].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + (dataWidth * i), 1, dataWidth, (Convert.ToInt32(totalGradeDl[i]) < avg && totalGradeDl[i] != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                    }
                    excelReport.SetCellValue(total.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count, 1, dataWidth, ExcelCellStyle.YellowTotalNumber, true);
                    excelReport.SetCellValue(avg.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count + dataWidth, 1, dataWidth, ExcelCellStyle.YellowTotalNumber, true);

                    totalRowSalesCo++;
                    totalRowSalesHead++;
                    totalRowBranchHead++;
                    totalRowDealer++;
                    rowIndex++;

                    total = 0;
                    for (int j = 0; j < totalGradeDl.Count; j++)
                        total += Convert.ToInt32(totalGradeDl[j] / (totalOutlet != 0 ? totalSalesHead : 1));

                    avg = Convert.ToInt32(total / (averageCompany != 0 ? averageCompany : 1));
                    excelReport.SetCellValue("AVG", 8 + rowIndex, 0 + dealerWidth, 1, outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.PurpleTotal);
                    for (int i = 0; i < totalGradeDl.Count; i++)
                    {
                        decimal average = totalGradeDl[i] / (totalOutlet != 0 ? totalOutlet : 1);
                        excelReport.SetCellValue(Convert.ToInt32(average).ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + (dataWidth * i), 1, dataWidth, (Convert.ToInt32(average) < avg && average != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        //total += Convert.ToInt32(average);
                    }
                    excelReport.SetCellValue(total.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count, 1, dataWidth, ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(avg.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count + dataWidth, 1, dataWidth, ExcelCellStyle.PurpleTotalNumber, true);

                    totalRowSalesCo++;
                    totalRowSalesHead++;
                    totalRowBranchHead++;
                    totalRowDealer++;
                    rowIndex++;
                    averageCompany = 0;
                    excelReport.ReplaceRestorePoint(RestorePoint.CompanyCodeRestorePoint.ToString(), dealer.ToString(), 0, totalRowDealer, dealerWidth, ExcelCellStyle.LeftBorderedTop);
                    totalDealer++;
                    #endregion

                    excelReport.RestorePoint(8 + rowIndex, RestorePoint.CompanyCodeRestorePoint.ToString());
                    excelReport.RestorePoint(8 + rowIndex, RestorePoint.OutletRestorePoint.ToString());
                    excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesHeadRestorePoint.ToString());
                    excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesCoordinatorrestorePoint.ToString());

                    totalGradeDl = new Dictionary<int, decimal>();
                    totalGradeOu = new Dictionary<int, decimal>();
                    totalGradeCo = new Dictionary<int, decimal>();
                    totalGradeSH = new Dictionary<int, decimal>();
                    totalRowBranchHead = 0;
                    totalSalesHead = 0;
                    totalRowSalesCo = 0;
                    totalRowSalesHead = 0;
                    totalRowDealer = 0;
                    totalSalesman = 0;
                    totalOutlet = 0;
                    totalSalesCo++;
                    totalSalesHead++;
                    totalOutlet++;
                    totalDealer++;
                    totalDealerGT++;
                    averageCompany = 0;
                    averageMonth = 0;
                    averageSalesHead = 0;
                    salesHead = row["SalesHeadName"].ToString();
                    salesCoordinator = row["SalesCoordinatorName"].ToString();
                    outlet = row["BranchName"].ToString();
                    dealer = row["CompanyName"].ToString();
                }
                else if (area != row["Area"].ToString())
                {
                    #region SalesCoordinator Total
                    excelReport.SetCellValue("Total", 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth, 1, salesManWidth + gradeWidth, ExcelCellStyle.YellowTotal);

                    int total = 0, avg = 0;
                    for (int j = 0; j < totalGradeCo.Count; j++)
                        total += Convert.ToInt32(totalGradeCo[j].ToString());
                    avg = Convert.ToInt32(total / (averageMonth != 0 ? averageMonth : 1));

                    for (int i = 0; i < totalGradeCo.Count; i++)
                    {
                        excelReport.SetCellValue(totalGradeCo[i].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + (dataWidth * i), 1, dataWidth, (Convert.ToInt32(totalGradeCo[i]) < avg && totalGradeCo[i] != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                    }
                    excelReport.SetCellValue(total.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count, 1, dataWidth, ExcelCellStyle.YellowTotalNumber, true);
                    excelReport.SetCellValue(avg.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count + dataWidth, 1, dataWidth, ExcelCellStyle.YellowTotalNumber, true);
                    totalRowSalesCo++;
                    totalRowSalesHead++;
                    totalRowBranchHead++;
                    totalRowDealer++;
                    rowIndex++;

                    total = 0;
                    for (int j = 0; j < totalGradeCo.Count; j++)
                        total += Convert.ToInt32(totalGradeCo[j] / (totalSalesman != 0 ? totalSalesman : 1));

                    avg = Convert.ToInt32(total / (averageMonth != 0 ? averageMonth : 1));
                    excelReport.SetCellValue("AVG", 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth, 1, salesManWidth + gradeWidth, ExcelCellStyle.PurpleTotal);
                    for (int i = 0; i < totalGradeCo.Count; i++)
                    {
                        decimal average = totalGradeCo[i] / (totalSalesman != 0 ? totalSalesman : 1);
                        excelReport.SetCellValue(Convert.ToInt32(average).ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + (dataWidth * i), 1, dataWidth, (Convert.ToInt32(average) < avg && average != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        //total += Convert.ToInt32(average);
                    }
                    excelReport.SetCellValue(total.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count, 1, dataWidth, ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(avg.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count + dataWidth, 1, dataWidth, ExcelCellStyle.PurpleTotalNumber, true);

                    totalRowSalesCo++;
                    totalRowSalesHead++;
                    totalRowBranchHead++;
                    totalRowDealer++;
                    rowIndex++;
                    averageMonth = 0;
                    excelReport.ReplaceRestorePoint(RestorePoint.SalesCoordinatorrestorePoint.ToString(), salesCoordinator.ToString(), 0 + dealerWidth + outletWidth + salesHeadWidth, totalRowSalesCo, salesManWidth, ExcelCellStyle.LeftBorderedTop);
                    #endregion

                    #region SalesHead
                    excelReport.SetCellValue("Total", 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth, 1, salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.YellowTotal);
                    total = avg = 0;
                    for (int j = 0; j < totalGradeSH.Count; j++)
                        total += Convert.ToInt32(totalGradeSH[j].ToString());
                    avg = Convert.ToInt32(total / (averageSalesHead != 0 ? averageSalesHead : 1));

                    for (int i = 0; i < totalGradeSH.Count; i++)
                    {
                        excelReport.SetCellValue(totalGradeSH[i].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + (dataWidth * i), 1, dataWidth, (Convert.ToInt32(totalGradeSH[i]) < avg && totalGradeSH[i] != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                    }
                    excelReport.SetCellValue(total.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count, 1, dataWidth, ExcelCellStyle.YellowTotalNumber, true);
                    excelReport.SetCellValue(avg.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count + dataWidth, 1, dataWidth, ExcelCellStyle.YellowTotalNumber, true);

                    totalRowSalesCo++;
                    totalRowSalesHead++;
                    totalRowBranchHead++;
                    totalRowDealer++;
                    rowIndex++;

                    total = 0;
                    for (int j = 0; j < totalGradeSH.Count; j++)
                        total += Convert.ToInt32(totalGradeSH[j] / (totalSalesCo != 0 ? totalSalesCo : 1));

                    avg = Convert.ToInt32(total / (averageSalesHead != 0 ? averageSalesHead : 1));
                    excelReport.SetCellValue("AVG", 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth, 1, salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.PurpleTotal);
                    for (int i = 0; i < totalGradeSH.Count; i++)
                    {
                        decimal average = totalGradeSH[i] / (totalSalesCo != 0 ? totalSalesCo : 1);
                        excelReport.SetCellValue(Convert.ToInt32(average).ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + (dataWidth * i), 1, dataWidth, (Convert.ToInt32(average) < avg && average != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        //total += Convert.ToInt32(average);
                    }
                    excelReport.SetCellValue(total.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count, 1, dataWidth, ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(avg.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count + dataWidth, 1, dataWidth, ExcelCellStyle.PurpleTotalNumber, true);

                    totalRowSalesCo++;
                    totalRowSalesHead++;
                    totalRowBranchHead++;
                    totalRowDealer++;
                    rowIndex++;
                    averageSalesHead = 0;
                    excelReport.ReplaceRestorePoint(RestorePoint.SalesHeadRestorePoint.ToString(), salesHead.ToString(), 0 + dealerWidth + outletWidth, totalRowSalesHead, salesHeadWidth, ExcelCellStyle.LeftBorderedTop);
                    #endregion

                    #region Outlet Total
                    excelReport.SetCellValue("Total", 8 + rowIndex, 0 + dealerWidth + outletWidth, 1, salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.YellowTotal);
                    total = avg = 0;
                    for (int j = 0; j < totalGradeOu.Count; j++)
                        total += Convert.ToInt32(totalGradeOu[j].ToString());
                    avg = Convert.ToInt32(total / (averageBranch != 0 ? averageBranch : 1));

                    for (int i = 0; i < totalGradeOu.Count; i++)
                    {
                        excelReport.SetCellValue(totalGradeOu[i].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + (dataWidth * i), 1, dataWidth, (Convert.ToInt32(totalGradeOu[i]) < avg && totalGradeOu[i] != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                    }
                    excelReport.SetCellValue(total.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count, 1, dataWidth, ExcelCellStyle.YellowTotalNumber, true);
                    excelReport.SetCellValue(avg.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count + dataWidth, 1, dataWidth, ExcelCellStyle.YellowTotalNumber, true);

                    totalRowSalesCo++;
                    totalRowSalesHead++;
                    totalRowBranchHead++;
                    totalRowDealer++;
                    rowIndex++;

                    total = 0;
                    for (int j = 0; j < totalGradeOu.Count; j++)
                        total += Convert.ToInt32(totalGradeOu[j] / (totalSalesHead != 0 ? totalSalesHead : 1));

                    avg = Convert.ToInt32(total / (averageBranch != 0 ? averageBranch : 1));
                    excelReport.SetCellValue("AVG", 8 + rowIndex, 0 + dealerWidth + outletWidth, 1, salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.PurpleTotal);
                    for (int i = 0; i < totalGradeOu.Count; i++)
                    {
                        decimal average = totalGradeOu[i] / (totalSalesHead != 0 ? totalSalesHead : 1);
                        excelReport.SetCellValue(Convert.ToInt32(average).ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + (dataWidth * i), 1, dataWidth, (Convert.ToInt32(average) < avg && average != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        //total += Convert.ToInt32(average);
                    }
                    excelReport.SetCellValue(total.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count, 1, dataWidth, ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(avg.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count + dataWidth, 1, dataWidth, ExcelCellStyle.PurpleTotalNumber, true);

                    totalRowSalesCo++;
                    totalRowSalesHead++;
                    totalRowBranchHead++;
                    totalRowDealer++;
                    rowIndex++;
                    averageBranch = 0;
                    excelReport.ReplaceRestorePoint(RestorePoint.OutletRestorePoint.ToString(), outlet.ToString(), 0 + dealerWidth, totalRowBranchHead, outletWidth, ExcelCellStyle.LeftBorderedTop);
                    #endregion

                    #region Dealer Total
                    excelReport.SetCellValue("Total", 8 + rowIndex, 0 + dealerWidth, 1, outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.YellowTotal);
                    total = avg = 0;
                    for (int j = 0; j < totalGradeDl.Count; j++)
                        total += Convert.ToInt32(totalGradeDl[j].ToString());
                    avg = Convert.ToInt32(total / (averageCompany != 0 ? averageCompany : 1));

                    for (int i = 0; i < totalGradeDl.Count; i++)
                    {
                        excelReport.SetCellValue(totalGradeDl[i].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + (dataWidth * i), 1, dataWidth, (Convert.ToInt32(totalGradeDl[i]) < avg && totalGradeDl[i] != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                    }
                    excelReport.SetCellValue(total.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count, 1, dataWidth, ExcelCellStyle.YellowTotalNumber, true);
                    excelReport.SetCellValue(avg.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count + dataWidth, 1, dataWidth, ExcelCellStyle.YellowTotalNumber, true);

                    totalRowSalesCo++;
                    totalRowSalesHead++;
                    totalRowBranchHead++;
                    totalRowDealer++;
                    rowIndex++;

                    total = 0;
                    for (int j = 0; j < totalGradeDl.Count; j++)
                        total += Convert.ToInt32(totalGradeDl[j] / (totalOutlet != 0 ? totalOutlet : 1));

                    avg = Convert.ToInt32(total / (averageCompany != 0 ? averageCompany : 1));
                    excelReport.SetCellValue("AVG", 8 + rowIndex, 0 + dealerWidth, 1, outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.PurpleTotal);
                    for (int i = 0; i < totalGradeDl.Count; i++)
                    {
                        decimal average = totalGradeDl[i] / (totalOutlet != 0 ? totalOutlet : 1);
                        excelReport.SetCellValue(Convert.ToInt32(average).ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + (dataWidth * i), 1, dataWidth, (Convert.ToInt32(average) < avg && average != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        //total += Convert.ToInt32(average);
                    }
                    excelReport.SetCellValue(total.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count, 1, dataWidth, ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(avg.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count + dataWidth, 1, dataWidth, ExcelCellStyle.PurpleTotalNumber, true);

                    totalRowSalesCo++;
                    totalRowSalesHead++;
                    totalRowBranchHead++;
                    totalRowDealer++;
                    rowIndex++;
                    averageCompany = 0;
                    excelReport.ReplaceRestorePoint(RestorePoint.CompanyCodeRestorePoint.ToString(), dealer.ToString(), 0, totalRowDealer, dealerWidth, ExcelCellStyle.LeftBorderedTop);
                    #endregion

                    #region Area Total
                    if (parameter[5].ToString() == "ALL")
                    {
                        if (area != "JABODETABEK")
                            excelReport.SetCellValue("Total " + area.ToString(), 8 + rowIndex, 0, 1, dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.YellowTotal);
                        else
                            excelReport.SetCellValue("Total DEALER " + area.ToString(), 8 + rowIndex, 0, 1, dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.YellowTotal);

                        total = avg = 0;
                        for (int j = 0; j < totalGradeAr.Count; j++)
                            total += Convert.ToInt32(totalGradeAr[j].ToString());
                        avg = Convert.ToInt32(total / (averageArea != 0 ? averageArea : 1));

                        for (int i = 0; i < totalGradeAr.Count; i++)
                        {
                            excelReport.SetCellValue(totalGradeAr[i].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + (dataWidth * i), 1, dataWidth, (Convert.ToInt32(totalGradeAr[i]) < avg && totalGradeAr[i] != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                        }
                        excelReport.SetCellValue(total.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count, 1, dataWidth, ExcelCellStyle.YellowTotalNumber, true);
                        excelReport.SetCellValue(avg.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count + dataWidth, 1, dataWidth, ExcelCellStyle.YellowTotalNumber, true);

                        totalRowSalesCo++;
                        totalRowSalesHead++;
                        totalRowBranchHead++;
                        totalRowDealer++;
                        rowIndex++;

                        total = 0;
                        for (int j = 0; j < totalGradeAr.Count; j++)
                            total += Convert.ToInt32(totalGradeAr[j] / (totalDealer != 0 ? totalDealer : 1));

                        avg = Convert.ToInt32(total / (averageArea != 0 ? averageArea : 1));
                        if (area != "JABODETABEK")
                            excelReport.SetCellValue("AVG " + area.ToString(), 8 + rowIndex, 0, 1, dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.PurpleTotal);
                        else
                            excelReport.SetCellValue("AVG DEALER " + area.ToString(), 8 + rowIndex, 0, 1, dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.PurpleTotal);

                        for (int i = 0; i < totalGradeAr.Count; i++)
                        {
                            decimal average = totalGradeAr[i] / (totalDealer != 0 ? totalDealer : 1);
                            excelReport.SetCellValue(Convert.ToInt32(average).ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + (dataWidth * i), 1, dataWidth, (Convert.ToInt32(average) < avg && average != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                            //total += Convert.ToInt32(average);
                        }
                        excelReport.SetCellValue(total.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count, 1, dataWidth, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(avg.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count + dataWidth, 1, dataWidth, ExcelCellStyle.PurpleTotalNumber, true);

                        totalRowSalesCo++;
                        totalRowSalesHead++;
                        totalRowBranchHead++;
                        totalRowDealer++;
                        rowIndex++;
                        averageArea = 0;
                        totalArea++;
                    }
                    #endregion

                    #region Total Cabang Jabodetabek
                    if (parameter[2].ToString() == "ALL" && parameter[4].ToString() == "ALL" && parameter[6].ToString() == "ALL" && area == "JABODETABEK")
                    {
                        excelReport.SetCellValue("Total " + area.ToString(), 8 + rowIndex, 0, 1, dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.YellowTotal);

                        total = avg = 0;
                        for (int j = 0; j < totalGradeArJabodetabek.Count; j++)
                            total += Convert.ToInt32(totalGradeArJabodetabek[j].ToString());
                        avg = Convert.ToInt32(total / (averageAreaJabodetabek != 0 ? averageAreaJabodetabek : 1));

                        for (int i = 0; i < totalGradeArJabodetabek.Count; i++)
                        {
                            excelReport.SetCellValue(totalGradeArJabodetabek[i].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + (dataWidth * i), 1, dataWidth, (Convert.ToInt32(totalGradeAr[i]) < avg && totalGradeAr[i] != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                        }
                        excelReport.SetCellValue(total.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count, 1, dataWidth, ExcelCellStyle.YellowTotalNumber, true);
                        excelReport.SetCellValue(avg.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count + dataWidth, 1, dataWidth, ExcelCellStyle.YellowTotalNumber, true);

                        totalRowSalesCo++;
                        totalRowSalesHead++;
                        totalRowBranchHead++;
                        totalRowDealer++;
                        rowIndex++;

                        total = 0;
                        for (int j = 0; j < totalGradeArJabodetabek.Count; j++)
                            total += Convert.ToInt32(totalGradeArJabodetabek[j] / (totalDealer != 0 ? totalDealer : 1));

                        avg = Convert.ToInt32(total / (averageAreaJabodetabek != 0 ? averageAreaJabodetabek : 1));

                        excelReport.SetCellValue("AVG " + area.ToString(), 8 + rowIndex, 0, 1, dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.PurpleTotal);

                        for (int i = 0; i < totalGradeArJabodetabek.Count; i++)
                        {
                            decimal average = totalGradeArJabodetabek[i] / (totalDealer != 0 ? totalDealer : 1);
                            excelReport.SetCellValue(Convert.ToInt32(average).ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + (dataWidth * i), 1, dataWidth, (Convert.ToInt32(average) < avg && average != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                            //total += Convert.ToInt32(average);
                        }
                        excelReport.SetCellValue(total.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count, 1, dataWidth, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(avg.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count + dataWidth, 1, dataWidth, ExcelCellStyle.PurpleTotalNumber, true);

                        totalRowSalesCo++;
                        totalRowSalesHead++;
                        totalRowBranchHead++;
                        totalRowDealer++;
                        rowIndex++;
                        averageAreaJabodetabek = 0;
                        totalArea++;
                        totalGradeArJabodetabek = new Dictionary<int, decimal>();
                    }
                    #endregion
                    excelReport.RestorePoint(8 + rowIndex, RestorePoint.CompanyCodeRestorePoint.ToString());
                    excelReport.RestorePoint(8 + rowIndex, RestorePoint.OutletRestorePoint.ToString());
                    excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesHeadRestorePoint.ToString());
                    excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesCoordinatorrestorePoint.ToString());

                    totalGradeAr = new Dictionary<int, decimal>();
                    totalGradeDl = new Dictionary<int, decimal>();
                    totalGradeOu = new Dictionary<int, decimal>();
                    totalGradeCo = new Dictionary<int, decimal>();
                    totalGradeSH = new Dictionary<int, decimal>();
                    totalRowBranchHead = 0;
                    totalSalesHead = 0;
                    totalRowSalesCo = 0;
                    totalRowDealer = 0;
                    totalRowSalesHead = 0;
                    totalSalesman = 0;
                    totalOutlet = 0;
                    totalDealer = 0;
                    totalSalesCo++;
                    totalSalesHead++;
                    totalOutlet++;
                    totalDealer++;
                    totalDealerGT++;
                    averageArea = 0;
                    averageBranch = 0;
                    averageCompany = 0;
                    averageMonth = 0;
                    averageSalesHead = 0;
                    salesHead = row["SalesHeadName"].ToString();
                    salesCoordinator = row["SalesCoordinatorName"].ToString();
                    outlet = row["BranchName"].ToString();
                    dealer = row["CompanyName"].ToString();
                    area = row["Area"].ToString();
                }

                if (gradeListEmployee.Length != 0)
                {
                    #region Kalo Salesman ada Grade
                    #region Isi Data
                    object[] isiData = row.ItemArray;
                    index = 0;
                    for (int i = 12; i < isiData.Length - 2; i++)
                    {
                        if (index < gradeListEmployee.Length)
                        {
                            if (Convert.ToInt32(dtHeader.Rows[i - 12]["YearMonth"].ToString()) < Convert.ToInt32(gradeListEmployee[index]["YearMonth"].ToString()))
                            {
                                for (int j = 0; j < dtSalesmanFullGrade.Rows.Count; j++)
                                {
                                    if (j == currentGrade)
                                        gradeEnable[j].Add(Convert.ToInt32(isiData[i].ToString()));
                                    else
                                        gradeEnable[j].Add(0);
                                }
                            }
                            else
                            {
                                var sgradde = gradeListEmployee[index]["Grade"].ToString();
                                currentGrade = Convert.ToInt32(string.IsNullOrEmpty(sgradde)?"0":sgradde);
                                index++;
                                for (int j = 0; j < dtSalesmanFullGrade.Rows.Count; j++)
                                {
                                    if (j == currentGrade)
                                        gradeEnable[j].Add(Convert.ToInt32(isiData[i].ToString()));
                                    else
                                        gradeEnable[j].Add(0);
                                }
                            }
                        }
                        else
                        {
                            for (int j = 0; j < dtSalesmanFullGrade.Rows.Count; j++)
                            {
                                if (j == currentGrade)
                                    gradeEnable[j].Add(Convert.ToInt32(isiData[i].ToString()));
                                else
                                    gradeEnable[j].Add(0);
                            }
                        }

                        if (totalGradeCo.ContainsKey(i - 12))
                            totalGradeCo[i - 12] += Convert.ToInt32(isiData[i].ToString());
                        else
                            totalGradeCo.Add(i - 12, Convert.ToInt32(isiData[i].ToString()));

                        if (totalGradeSH.ContainsKey(i - 12))
                            totalGradeSH[i - 12] += Convert.ToInt32(isiData[i].ToString());
                        else
                            totalGradeSH.Add(i - 12, Convert.ToInt32(isiData[i].ToString()));

                        if (totalGradeOu.ContainsKey(i - 12))
                            totalGradeOu[i - 12] += Convert.ToInt32(isiData[i].ToString());
                        else
                            totalGradeOu.Add(i - 12, Convert.ToInt32(isiData[i].ToString()));

                        if (totalGradeDl.ContainsKey(i - 12))
                            totalGradeDl[i - 12] += Convert.ToInt32(isiData[i].ToString());
                        else
                            totalGradeDl.Add(i - 12, Convert.ToInt32(isiData[i].ToString()));

                        if (totalGradeAr.ContainsKey(i - 12))
                            totalGradeAr[i - 12] += Convert.ToInt32(isiData[i].ToString());
                        else
                            totalGradeAr.Add(i - 12, Convert.ToInt32(isiData[i].ToString()));

                        if (totalGradeArJabodetabek.ContainsKey(i - 12))
                            totalGradeArJabodetabek[i - 12] += Convert.ToInt32(isiData[i].ToString());
                        else
                            totalGradeArJabodetabek.Add(i - 12, Convert.ToInt32(isiData[i].ToString()));

                        if (totalGradeGT.ContainsKey(i - 12))
                            totalGradeGT[i - 12] += Convert.ToInt32(isiData[i].ToString());
                        else
                            totalGradeGT.Add(i - 12, Convert.ToInt32(isiData[i].ToString()));
                    }
                    #endregion

                    excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesmanRestorePoint.ToString());

                    #region untuk isi grade ama nilainya

                    for (int j = 0; j < dtSalesmanFullGrade.Rows.Count; j++)
                    {
                        int total = 0;
                        int avg = 0;
                        foreach (int nilai in gradeEnable[j])
                            total += nilai;
                        avg = Convert.ToInt32(total / (totalMonth != 0 ? totalMonth : 1));

                        excelReport.SetCellValue(dtSalesmanFullGrade.Rows[j]["GradeName"].ToString(), 8 + rowIndex + j, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth, 1, gradeWidth, ExcelCellStyle.LeftBorderedStandardWrap);

                        for (int i = 12; i < isiData.Length - 2; i++)
                        {
                            excelReport.SetCellValue(gradeEnable[j][i - 12].ToString(), 8 + rowIndex + j, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + (dataWidth * (i - 12)), 1, dataWidth, (gradeEnable[j][i - 12] < avg && gradeEnable[j][i - 12] != 0) ? ExcelCellStyle.RightBorderedStandardRedNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        }
                        excelReport.SetCellValue(total.ToString(), 8 + rowIndex + j, 0 + isiData.Length - 8, 1, dataWidth, ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(avg.ToString(), 8 + rowIndex + j, 0 + isiData.Length - 8 + dataWidth, 1, dataWidth, ExcelCellStyle.RightBorderedStandardNumber, true);
                    }

                    #endregion

                    excelReport.ReplaceRestorePoint(RestorePoint.SalesmanRestorePoint.ToString(), row["SalesmanName"].ToString(), 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth, 7, salesManWidth, ExcelCellStyle.LeftBorderedTop);

                    #endregion
                }
                else
                {
                    #region Kalo Salesman tidak memiliki Grade
                    #region Isi Data
                    object[] isiData = row.ItemArray;
                    index = 0;
                    for (int i = 12; i < isiData.Length - 2; i++)
                    {

                        gradeEnable[0].Add(Convert.ToInt32(isiData[i].ToString()));
                        for (int j = 1; j < dtSalesmanFullGrade.Rows.Count; j++)
                        {
                            gradeEnable[j].Add(0);
                        }

                        if (totalGradeCo.ContainsKey(i - 12))
                            totalGradeCo[i - 12] += Convert.ToInt32(isiData[i].ToString());
                        else
                            totalGradeCo.Add(i - 12, Convert.ToInt32(isiData[i].ToString()));

                        if (totalGradeSH.ContainsKey(i - 12))
                            totalGradeSH[i - 12] += Convert.ToInt32(isiData[i].ToString());
                        else
                            totalGradeSH.Add(i - 12, Convert.ToInt32(isiData[i].ToString()));

                        if (totalGradeOu.ContainsKey(i - 12))
                            totalGradeOu[i - 12] += Convert.ToInt32(isiData[i].ToString());
                        else
                            totalGradeOu.Add(i - 12, Convert.ToInt32(isiData[i].ToString()));

                        if (totalGradeDl.ContainsKey(i - 12))
                            totalGradeDl[i - 12] += Convert.ToInt32(isiData[i].ToString());
                        else
                            totalGradeDl.Add(i - 12, Convert.ToInt32(isiData[i].ToString()));

                        if (totalGradeAr.ContainsKey(i - 12))
                            totalGradeAr[i - 12] += Convert.ToInt32(isiData[i].ToString());
                        else
                            totalGradeAr.Add(i - 12, Convert.ToInt32(isiData[i].ToString()));

                        if (totalGradeArJabodetabek.ContainsKey(i - 12))
                            totalGradeArJabodetabek[i - 12] += Convert.ToInt32(isiData[i].ToString());
                        else
                            totalGradeArJabodetabek.Add(i - 12, Convert.ToInt32(isiData[i].ToString()));

                        if (totalGradeGT.ContainsKey(i - 12))
                            totalGradeGT[i - 12] += Convert.ToInt32(isiData[i].ToString());
                        else
                            totalGradeGT.Add(i - 12, Convert.ToInt32(isiData[i].ToString()));
                    }
                    #endregion

                    excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesmanRestorePoint.ToString());

                    #region untuk isi grade ama nilainya
                    for (int j = 0; j < dtSalesmanFullGrade.Rows.Count; j++)
                    {
                        int total = 0;
                        int avg = 0;
                        foreach (int nilai in gradeEnable[j])
                            total += nilai;
                        avg = Convert.ToInt32(total / totalMonth);

                        excelReport.SetCellValue(dtSalesmanFullGrade.Rows[j]["GradeName"].ToString(), 8 + rowIndex + j, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth, 1, gradeWidth, ExcelCellStyle.LeftBorderedStandardWrap);

                        for (int i = 12; i < isiData.Length - 2; i++)
                        {
                            excelReport.SetCellValue(gradeEnable[j][i - 12].ToString(), 8 + rowIndex + j, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + (dataWidth * (i - 12)), 1, dataWidth, (gradeEnable[j][i - 12] < Convert.ToInt32(avg) && gradeEnable[j][i - 12] != 0) ? ExcelCellStyle.RightBorderedStandardRedNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                        }
                        excelReport.SetCellValue(Convert.ToInt32(total).ToString(), 8 + rowIndex + j, 0 + isiData.Length - 8, 1, dataWidth, ExcelCellStyle.RightBorderedStandardNumber, true);
                        excelReport.SetCellValue(Convert.ToInt32(avg).ToString(), 8 + rowIndex + j, 0 + isiData.Length - 8 + dataWidth, 1, dataWidth, ExcelCellStyle.RightBorderedStandardNumber, true);
                    }
                    #endregion

                    excelReport.ReplaceRestorePoint(RestorePoint.SalesmanRestorePoint.ToString(), row["SalesmanName"].ToString(), 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth, 7, salesManWidth, ExcelCellStyle.LeftBorderedTop);

                    #endregion
                }

                averageMonth = averageMonth < totalMonth ? totalMonth : averageMonth;
                averageSalesHead = averageSalesHead < totalMonth ? totalMonth : averageSalesHead;
                averageBranch = averageBranch < totalMonth ? totalMonth : averageBranch;
                averageCompany = averageCompany < totalMonth ? totalMonth : averageCompany;
                averageArea = averageArea < totalMonth ? totalMonth : averageArea;
                averageAreaJabodetabek = averageAreaJabodetabek < totalMonth ? totalMonth : averageAreaJabodetabek;
                averageGrandTotal = averageGrandTotal < totalMonth ? totalMonth : averageGrandTotal;
                totalSalesman++;
                totalRowSalesCo += 7;
                totalRowSalesHead += 7;
                totalRowBranchHead += 7;
                totalRowDealer += 7;
                rowIndex += 7;
            }

            #region SalesCoordinator Total
            excelReport.SetCellValue("Total", 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth, 1, salesManWidth + gradeWidth, ExcelCellStyle.YellowTotal);

            decimal totalGT = 0;
            decimal avgGT = 0;
            for (int j = 0; j < totalGradeCo.Count; j++)
                totalGT += Convert.ToInt32(totalGradeCo[j].ToString());
            avgGT = Convert.ToInt32(totalGT / averageMonth);

            for (int i = 0; i < totalGradeCo.Count; i++)
            {
                excelReport.SetCellValue(totalGradeCo[i].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + (dataWidth * i), 1, dataWidth, (Convert.ToInt32(totalGradeCo[i]) < avgGT && totalGradeCo[i] != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
            }
            excelReport.SetCellValue(totalGT.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count, 1, dataWidth, ExcelCellStyle.YellowTotalNumber, true);
            excelReport.SetCellValue(avgGT.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count + dataWidth, 1, dataWidth, ExcelCellStyle.YellowTotalNumber, true);
            totalRowSalesCo++;
            totalRowSalesHead++;
            totalRowBranchHead++;
            totalRowDealer++;
            rowIndex++;

            totalGT = 0;
            for (int j = 0; j < totalGradeCo.Count; j++)
                totalGT += Convert.ToInt32(totalGradeCo[j] / totalSalesman);

            avgGT = Convert.ToInt32(totalGT / averageMonth);
            excelReport.SetCellValue("AVG", 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth, 1, salesManWidth + gradeWidth, ExcelCellStyle.PurpleTotal);
            for (int i = 0; i < totalGradeCo.Count; i++)
            {
                decimal average = totalGradeCo[i] / totalSalesman;
                excelReport.SetCellValue(Convert.ToInt32(average).ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + (dataWidth * i), 1, dataWidth, (Convert.ToInt32(average) < avgGT && average != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
            }
            excelReport.SetCellValue(totalGT.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count, 1, dataWidth, ExcelCellStyle.PurpleTotalNumber, true);
            excelReport.SetCellValue(avgGT.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count + dataWidth, 1, dataWidth, ExcelCellStyle.PurpleTotalNumber, true);

            totalRowSalesCo++;
            totalRowSalesHead++;
            totalRowBranchHead++;
            totalRowDealer++;
            rowIndex++;

            excelReport.ReplaceRestorePoint(RestorePoint.SalesCoordinatorrestorePoint.ToString(), salesCoordinator.ToString(), 0 + dealerWidth + outletWidth + salesHeadWidth, totalRowSalesCo, salesManWidth, ExcelCellStyle.LeftBorderedTop);
            #endregion

            #region SalesHead
            excelReport.SetCellValue("Total", 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth, 1, salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.YellowTotal);
            totalGT = avgGT = 0;
            for (int j = 0; j < totalGradeSH.Count; j++)
                totalGT += Convert.ToInt32(totalGradeSH[j].ToString());
            avgGT = Convert.ToInt32(totalGT / averageSalesHead);

            for (int i = 0; i < totalGradeSH.Count; i++)
            {
                excelReport.SetCellValue(totalGradeSH[i].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + (dataWidth * i), 1, dataWidth, (Convert.ToInt32(totalGradeSH[i]) < avgGT && totalGradeSH[i] != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
            }
            excelReport.SetCellValue(totalGT.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count, 1, dataWidth, ExcelCellStyle.YellowTotalNumber, true);
            excelReport.SetCellValue(avgGT.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count + dataWidth, 1, dataWidth, ExcelCellStyle.YellowTotalNumber, true);

            totalRowSalesCo++;
            totalRowSalesHead++;
            totalRowBranchHead++;
            totalRowDealer++;
            rowIndex++;

            totalGT = 0;
            for (int j = 0; j < totalGradeSH.Count; j++)
                totalGT += Convert.ToInt32(totalGradeSH[j] / totalSalesCo);

            avgGT = Convert.ToInt32(totalGT / averageSalesHead);
            excelReport.SetCellValue("AVG", 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth, 1, salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.PurpleTotal);
            for (int i = 0; i < totalGradeSH.Count; i++)
            {
                decimal average = totalGradeSH[i] / totalSalesCo;
                excelReport.SetCellValue(Convert.ToInt32(average).ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + (dataWidth * i), 1, dataWidth, (Convert.ToInt32(average) < avgGT && average != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
            }
            excelReport.SetCellValue(totalGT.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count, 1, dataWidth, ExcelCellStyle.PurpleTotalNumber, true);
            excelReport.SetCellValue(avgGT.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count + dataWidth, 1, dataWidth, ExcelCellStyle.PurpleTotalNumber, true);

            totalRowSalesCo++;
            totalRowSalesHead++;
            totalRowBranchHead++;
            totalRowDealer++;
            rowIndex++;

            excelReport.ReplaceRestorePoint(RestorePoint.SalesHeadRestorePoint.ToString(), salesHead.ToString(), 0 + dealerWidth + outletWidth, totalRowSalesHead, salesHeadWidth, ExcelCellStyle.LeftBorderedTop);
            #endregion

            #region Outlet Total
            excelReport.SetCellValue("Total", 8 + rowIndex, 0 + dealerWidth + outletWidth, 1, salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.YellowTotal);
            totalGT = avgGT = 0;
            for (int j = 0; j < totalGradeOu.Count; j++)
                totalGT += Convert.ToInt32(totalGradeOu[j].ToString());
            avgGT = Convert.ToInt32(totalGT / averageBranch);

            for (int i = 0; i < totalGradeOu.Count; i++)
            {
                excelReport.SetCellValue(totalGradeOu[i].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + (dataWidth * i), 1, dataWidth, (Convert.ToInt32(totalGradeOu[i]) < avgGT && totalGradeOu[i] != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
            }
            excelReport.SetCellValue(totalGT.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count, 1, dataWidth, ExcelCellStyle.YellowTotalNumber, true);
            excelReport.SetCellValue(avgGT.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count + dataWidth, 1, dataWidth, ExcelCellStyle.YellowTotalNumber, true);

            totalRowSalesCo++;
            totalRowSalesHead++;
            totalRowBranchHead++;
            totalRowDealer++;
            rowIndex++;

            totalGT = 0;
            for (int j = 0; j < totalGradeOu.Count; j++)
                totalGT += Convert.ToInt32(totalGradeOu[j] / totalSalesHead);

            avgGT = Convert.ToInt32(totalGT / averageBranch);

            excelReport.SetCellValue("AVG", 8 + rowIndex, 0 + dealerWidth + outletWidth, 1, salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.PurpleTotal);
            for (int i = 0; i < totalGradeOu.Count; i++)
            {
                decimal average = totalGradeOu[i] / totalSalesHead;
                excelReport.SetCellValue(Convert.ToInt32(average).ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + (dataWidth * i), 1, dataWidth, (Convert.ToInt32(average) < avgGT && average != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
            }
            excelReport.SetCellValue(totalGT.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count, 1, dataWidth, ExcelCellStyle.PurpleTotalNumber, true);
            excelReport.SetCellValue(avgGT.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count + dataWidth, 1, dataWidth, ExcelCellStyle.PurpleTotalNumber, true);

            totalRowSalesCo++;
            totalRowSalesHead++;
            totalRowBranchHead++;
            totalRowDealer++;
            rowIndex++;

            excelReport.ReplaceRestorePoint(RestorePoint.OutletRestorePoint.ToString(), outlet.ToString(), 0 + dealerWidth, totalRowBranchHead, outletWidth, ExcelCellStyle.LeftBorderedTop);
            #endregion

            #region Dealer Total
            excelReport.SetCellValue("Total", 8 + rowIndex, 0 + dealerWidth, 1, dealerWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.YellowTotal);
            totalGT = avgGT = 0;
            for (int j = 0; j < totalGradeDl.Count; j++)
                totalGT += Convert.ToInt32(totalGradeDl[j].ToString());
            avgGT = Convert.ToInt32(totalGT / averageCompany);

            for (int i = 0; i < totalGradeDl.Count; i++)
            {
                excelReport.SetCellValue(totalGradeDl[i].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + (dataWidth * i), 1, dataWidth, (Convert.ToInt32(totalGradeDl[i]) < avgGT && totalGradeDl[i] != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
            }
            excelReport.SetCellValue(totalGT.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count, 1, dataWidth, ExcelCellStyle.YellowTotalNumber, true);
            excelReport.SetCellValue(avgGT.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count + dataWidth, 1, dataWidth, ExcelCellStyle.YellowTotalNumber, true);

            totalRowSalesCo++;
            totalRowSalesHead++;
            totalRowBranchHead++;
            totalRowDealer++;
            rowIndex++;

            totalGT = 0;
            for (int j = 0; j < totalGradeDl.Count; j++)
                totalGT += Convert.ToInt32(totalGradeDl[j] / totalOutlet);

            avgGT = Convert.ToInt32(totalGT / averageCompany);
            excelReport.SetCellValue("AVG", 8 + rowIndex, 0 + dealerWidth, 1, dealerWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.PurpleTotal);
            for (int i = 0; i < totalGradeDl.Count; i++)
            {
                decimal average = totalGradeDl[i] / totalOutlet;
                excelReport.SetCellValue(Convert.ToInt32(average).ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + (dataWidth * i), 1, dataWidth, (Convert.ToInt32(average) < avgGT && average != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
            }
            excelReport.SetCellValue(totalGT.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count, 1, dataWidth, ExcelCellStyle.PurpleTotalNumber, true);
            excelReport.SetCellValue(avgGT.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count + dataWidth, 1, dataWidth, ExcelCellStyle.PurpleTotalNumber, true);

            totalRowSalesCo++;
            totalRowSalesHead++;
            totalRowBranchHead++;
            totalRowDealer++;
            rowIndex++;

            excelReport.ReplaceRestorePoint(RestorePoint.CompanyCodeRestorePoint.ToString(), dealer.ToString(), 0, totalRowDealer, dealerWidth, ExcelCellStyle.LeftBorderedTop);
            #endregion

            #region Area Total
            if (parameter[5].ToString() == "ALL")
            {
                excelReport.SetCellValue("Total " + area.ToString(), 8 + rowIndex, 0, 1, dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.YellowTotal);
                totalGT = avgGT = 0;
                for (int j = 0; j < totalGradeAr.Count; j++)
                    totalGT += Convert.ToInt32(totalGradeAr[j].ToString());
                avgGT = Convert.ToInt32(totalGT / averageArea);

                for (int i = 0; i < totalGradeAr.Count; i++)
                {
                    excelReport.SetCellValue(totalGradeAr[i].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + (dataWidth * i), 1, dataWidth, (Convert.ToInt32(totalGradeAr[i]) < avgGT && totalGradeAr[i] != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                }
                excelReport.SetCellValue(totalGT.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count, 1, dataWidth, ExcelCellStyle.YellowTotalNumber, true);
                excelReport.SetCellValue(avgGT.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count + dataWidth, 1, dataWidth, ExcelCellStyle.YellowTotalNumber, true);

                totalRowSalesCo++;
                totalRowSalesHead++;
                totalRowBranchHead++;
                totalRowDealer++;
                rowIndex++;

                totalGT = 0;
                for (int j = 0; j < totalGradeAr.Count; j++)
                    totalGT += Convert.ToInt32(totalGradeAr[j] / totalDealer);

                avgGT = Convert.ToInt32(totalGT / averageArea);
                excelReport.SetCellValue("AVG " + area.ToString(), 8 + rowIndex, 0, 1, dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.PurpleTotal);
                for (int i = 0; i < totalGradeAr.Count; i++)
                {
                    decimal average = totalGradeAr[i] / totalDealer;
                    excelReport.SetCellValue(Convert.ToInt32(average).ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + (dataWidth * i), 1, dataWidth, (Convert.ToInt32(average) < avgGT && average != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                }
                excelReport.SetCellValue(totalGT.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count, 1, dataWidth, ExcelCellStyle.PurpleTotalNumber, true);
                excelReport.SetCellValue(avgGT.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count + dataWidth, 1, dataWidth, ExcelCellStyle.PurpleTotalNumber, true);

                totalRowSalesCo++;
                totalRowSalesHead++;
                totalRowBranchHead++;
                totalRowDealer++;
                rowIndex++;
                totalDealerGT++;
                averageArea = 0;
            }
            #endregion

            #region Total SIS
            //if (parameter[9] == "RETAIL" || parameter[9] == "SALES" || parameter[9] == "FPOL")
            //{
            //    excelReport.SetCellValue("TOTAL SIS RETAIL SALES ", 8 + rowIndex, 0, 1, dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.BrownTotal);
            //    totalGT = avgGT = 0;
            //    for (int j = 0; j < dtHeader.Rows.Count; j++)
            //        totalGT += Convert.ToInt32(dtNSDS.Rows[0][j].ToString());
            //    avgGT = Convert.ToInt32(totalGT / dtNSDS.Rows[0].ItemArray.Length);

            //    for (int i = 0; i < dtHeader.Rows.Count; i++)
            //    {
            //        excelReport.SetCellValue(dtNSDS.Rows[0][i].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + (dataWidth * i), 1, dataWidth, ExcelCellStyle.BrownTotalNumber, true);
            //        totalGradeGT[i] += Convert.ToInt32(dtNSDS.Rows[0][i].ToString());
            //    }
            //    excelReport.SetCellValue(totalGT.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count, 1, dataWidth, ExcelCellStyle.BrownTotalNumber, true);
            //    excelReport.SetCellValue(avgGT.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count + dataWidth, 1, dataWidth, ExcelCellStyle.BrownTotalNumber, true);

            //    totalRowSalesCo++;
            //    totalRowSalesHead++;
            //    totalRowBranchHead++;
            //    totalRowDealer++;
            //    rowIndex++;

            //}
            #endregion

            #region Grand Total
            if (parameter[3].ToString() == "ALL")
            {
                excelReport.SetCellValue("Total NATIONAL ", 8 + rowIndex, 0, 1, dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.YellowTotal);
                totalGT = avgGT = 0;
                for (int j = 0; j < totalGradeAr.Count; j++)
                    totalGT += Convert.ToInt32(totalGradeGT[j].ToString());
                avgGT = Convert.ToInt32(totalGT / averageGrandTotal);

                for (int i = 0; i < totalGradeGT.Count; i++)
                {
                    excelReport.SetCellValue(totalGradeGT[i].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + (dataWidth * i), 1, dataWidth, (Convert.ToInt32(totalGradeGT[i]) < avgGT && totalGradeGT[i] != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                }
                excelReport.SetCellValue(totalGT.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count, 1, dataWidth, ExcelCellStyle.YellowTotalNumber, true);
                excelReport.SetCellValue(avgGT.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count + dataWidth, 1, dataWidth, ExcelCellStyle.YellowTotalNumber, true);

                totalRowSalesCo++;
                totalRowSalesHead++;
                totalRowBranchHead++;
                totalRowDealer++;
                rowIndex++;

                totalGT = 0;
                for (int j = 0; j < totalGradeAr.Count; j++)
                    totalGT += Convert.ToInt32(totalGradeGT[j] / totalDealerGT);

                avgGT = Convert.ToInt32(totalGT / averageGrandTotal);
                excelReport.SetCellValue("AVG NATIONAL", 8 + rowIndex, 0, 1, dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.PurpleTotal);
                for (int i = 0; i < totalGradeGT.Count; i++)
                {
                    decimal average = totalGradeGT[i] / totalDealerGT;
                    excelReport.SetCellValue(Convert.ToInt32(average).ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + (dataWidth * i), 1, dataWidth, (Convert.ToInt32(average) < avgGT && average != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                }
                excelReport.SetCellValue(totalGT.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count, 1, dataWidth, ExcelCellStyle.PurpleTotalNumber, true);
                excelReport.SetCellValue(avgGT.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + dtHeader.Rows.Count + dataWidth, 1, dataWidth, ExcelCellStyle.PurpleTotalNumber, true);

                totalRowSalesCo++;
                totalRowSalesHead++;
                totalRowBranchHead++;
                totalRowDealer++;
                rowIndex++;
                averageArea = 0;
            }
            #endregion

            excelReport.CloseExcelFileWriter();
        }
    }
}