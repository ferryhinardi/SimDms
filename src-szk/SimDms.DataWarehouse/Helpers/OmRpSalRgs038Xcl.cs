using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace SimDms.DataWarehouse.Helpers
{
    public class OmRpSalRgs038Xcl
    {
        System.Data.DataTable dt0;
        Dictionary<int, string> parameter = new Dictionary<int, string>();

        #region PrintData
        #region Properties
        private string seq, seq2, area, dealer, outlet, branchHead, salesHead, salesCoordinator, salesMan, marketModel, modelCatagory, color, grade, year;
        private int rowIndex = 0, colIndex = 0;
        private decimal jan, feb, mar, apr, may, jun, jul, aug, sep, oct, nov, dec, total;
        private decimal janTtl, febTtl, marTtl, aprTtl, mayTtl, junTtl, julTtl, augTtl, sepTtl, octTtl, novTtl, decTtl, totalTtl;
        private decimal janAVG, febAVG, marAVG, aprAVG, mayAVG, junAVG, julAVG, augAVG, sepAVG, octAVG, novAVG, decAVG, totalAVG;
        private decimal janArea, febArea, marArea, aprArea, mayArea, junArea, julArea, augArea, sepArea, octArea, novArea, decArea, totalArea;
        private decimal janGT, febGT, marGT, aprGT, mayGT, junGT, julGT, augGT, sepGT, octGT, novGT, decGT, totalGT;
        private decimal janGTII, febGTII, marGTII, aprGTII, mayGTII, junGTII, julGTII, augGTII, sepGTII, octGTII, novGTII, decGTII, totalGTII;
        private decimal janJabodetabek, febJabodetabek, marJabodetabek, aprJabodetabek, mayJabodetabek, junJabodetabek, julJabodetabek, augJabodetabek, sepJabodetabek, octJabodetabek, novJabodetabek, decJabodetabek, totalJabodetabek;

        private decimal totalItem = 0, totalItemGT = 0;
        private int totalSalesHead = 0, totalSalesCo = 0, totalDealer = 0, totalOutlet = 0, totalMonth = 0, totalDealerGT = 0, totalDealerJabodetabek = 0;
        private int totalRow = 0, totalRowColor = 0, totalRowModel = 0, totalRowSalsman = 0, totalRowSalesCo = 0, totalRowSalesHead = 0, totalRowBranchHead = 0, totalRowDealer = 0, totalSubTotal = 0;
        private int dealerWidth = 0, outletWidth = 0, salesHeadWidth = 0, salesCoordinatorWidth = 0, salesManWidth = 0, modelWidth = 0, colorWidth = 0, totalDetailWidth = 0, gradeWidth = 0;
        private ExcelCellStyle totalColor = ExcelCellStyle.RightBorderedStandardNumber;
        private ExcelCellStyle totalRedColor = ExcelCellStyle.RightBorderedStandardRedNumber;
        private string fileName = "";
        int averageMonth = 0, averageYear = 0, averageMonthJBDTK = 0;
        int currentYear = 0, salesmanMonth = 0;
        #endregion

        public OmRpSalRgs038Xcl(DataTable ds, Dictionary<int, string> param, string saveFileName)
        {
            dt0 = ds;
            parameter = param;
            fileName = saveFileName;
        }

        public void CreateReport(string pserver)
        {
            seq = dt0.Rows[0]["Seq"].ToString();
            seq2 = dt0.Rows[0]["Seq2"].ToString();
            area = dt0.Rows[0]["Area"].ToString();
            dealer = dt0.Rows[0]["CompanyName"].ToString();
            outlet = dt0.Rows[0]["BranchName"].ToString();
            salesHead = dt0.Rows[0]["SalesHeadName"].ToString();
            salesCoordinator = dt0.Rows[0]["SalesCoordinatorName"].ToString();
            salesMan = parameter[9] == "1" ? dt0.Rows[0]["SalesmanName"].ToString() : "";
            grade = parameter[9] == "1" ? dt0.Rows[0]["Grade"].ToString() : "";
            modelCatagory = dt0.Columns.Contains("ModelCatagory") == true ? dt0.Rows[0]["ModelCatagory"].ToString() : "";
            marketModel = parameter[10] == "1" ? dt0.Rows[0]["MarketModel"].ToString() : "";
            color = parameter[11] == "1" ? dt0.Rows[0]["ColourName"].ToString() : "";
            currentYear = Convert.ToInt32(dt0.Rows[0]["Year"].ToString());
            year = dt0.Rows[0]["Year"].ToString();

            StringBuilder sb = new StringBuilder();
            //sb.Append("(");
            sb.Append("by Outlet, Sales Head, Sales Coordinator ");
            sb.Append(parameter[9] == "1" ? ", Salesman" : "");
            sb.Append(parameter[10] == "1" ? ", Type" : "");
            sb.Append(parameter[11] == "1" ? " & Colour" : "");
            //sb.Append(")");
            sb.Append(" - " + parameter[12].ToString());

            string title = sb.ToString();


            //Pemetaan ukuran column
            dealerWidth = 1;
            outletWidth = 1;
            salesHeadWidth = 1;
            salesCoordinatorWidth = 1;
            salesManWidth = parameter[9] == "1" ? 1 : 0;
            gradeWidth = parameter[9] == "1" ? 1 : 0;
            modelWidth = parameter[10] == "1" ? 1 : 0;
            colorWidth = parameter[11] == "1" ? 1 : 0;

            totalDetailWidth = dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + modelWidth + colorWidth + gradeWidth;

            int indentTitleDesc = 6;

            ExcelFileWriter excelReport = new ExcelFileWriter(fileName, currentYear.ToString(), "OmRpSalRgs038", pserver);
            CreateHeader(excelReport, title, rowIndex, indentTitleDesc, currentYear.ToString());

            #region Body
            foreach (DataRow row in dt0.Rows)
            {
                //totalMonth = (Convert.ToInt32(row["StartDate"].ToString().Substring(0, 4)) != currentYear
                //    && Convert.ToInt32(row["EndDate"].ToString().Substring(0, 4)) != currentYear) ? 13 :
                //    (Convert.ToInt32(row["StartDate"].ToString().Substring(0, 4)) == currentYear
                //    && Convert.ToInt32(row["EndDate"].ToString().Substring(0, 4)) != currentYear) ? 13 - Convert.ToInt32(row["StartDate"].ToString().Substring(4, 2)) :
                //    (Convert.ToInt32(row["StartDate"].ToString().Substring(0, 4)) != currentYear
                //    && Convert.ToInt32(row["EndDate"].ToString().Substring(0, 4)) == currentYear) ? Convert.ToInt32(row["EndDate"].ToString().Substring(4, 2)) :
                //    Convert.ToInt32(row["EndDate"].ToString().Substring(4, 2)) - Convert.ToInt32(row["StartDate"].ToString().Substring(4, 2)) + 1;

                salesmanMonth = (Convert.ToDateTime(row["JoinDate"].ToString()).Year != currentYear
                    && Convert.ToDateTime(row["ResignDate"].ToString()).Year != currentYear) ? 13 :
                    (Convert.ToDateTime(row["JoinDate"].ToString()).Year == currentYear
                    && Convert.ToDateTime(row["ResignDate"].ToString()).Year != currentYear) ? 13 - Convert.ToDateTime(row["JoinDate"].ToString()).Month :
                    (Convert.ToDateTime(row["JoinDate"].ToString()).Year != currentYear
                    && Convert.ToDateTime(row["ResignDate"].ToString()).Year == currentYear) ? Convert.ToDateTime(row["ResignDate"].ToString()).Month :
                    Convert.ToDateTime(row["ResignDate"].ToString()).Month - Convert.ToDateTime(row["JoinDate"].ToString()).Month + 1;

                if (year == row["Year"].ToString()
                    && seq == row["Seq"].ToString()
                    && area == row["Area"].ToString()
                    && dealer == row["CompanyName"].ToString()
                    && outlet == row["BranchName"].ToString()
                    && salesHead == row["SalesHeadName"].ToString()
                    && salesCoordinator == row["SalesCoordinatorName"].ToString())
                {
                    #region Group
                    #region parameter11 = "1"
                    if (parameter[11] == "1"
                        && marketModel == (parameter[10] == "1" ? row["MarketModel"].ToString() : "")
                        && modelCatagory == (parameter[10] == "1" ? row["ModelCatagory"].ToString() : ""))
                    {
                        #region MarketModel Model Category
                        if (parameter[9].ToString() == "1" && salesMan == (parameter[9] == "1" ? row["SalesmanName"].ToString() : ""))
                        {
                            if (rowIndex == 0)
                            {
                                excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesCoordinatorrestorePoint.ToString());
                                if (parameter[9].ToString() == "1")
                                {
                                    excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesmanRestorePoint.ToString());
                                    excelReport.RestorePoint(8 + rowIndex, RestorePoint.GradeRestorePoint.ToString());
                                }
                                excelReport.RestorePoint(8 + rowIndex, RestorePoint.MarketModelRestorePoint.ToString());
                            }
                            excelReport.SetCellValue(row["ColourName"].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + modelWidth, 1, colorWidth, ExcelCellStyle.LeftBorderedStandard);
                        }
                        else if (parameter[9].ToString() == "1" && salesMan != (parameter[9] == "1" ? row["SalesmanName"].ToString() : ""))
                        {
                            excelReport.ReplaceRestorePoint(RestorePoint.MarketModelRestorePoint.ToString(), marketModel.ToString(), 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, totalRowColor, modelWidth, ExcelCellStyle.LeftBorderedStandard);
                            //decimal average = total / salesmanMonth;
                            decimal average = total / salesmanMonth;

                            //Masukin total Model Catagory
                            excelReport.SetCellValue("Total " + modelCatagory.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth + colorWidth, ExcelCellStyle.YellowTotal);
                            excelReport.SetCellValue(jan.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (jan < Convert.ToInt32(average) && jan != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(feb.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (feb < Convert.ToInt32(average) && feb != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(mar.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (mar < Convert.ToInt32(average) && mar != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(apr.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (apr < Convert.ToInt32(average) && apr != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(may.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (may < Convert.ToInt32(average) && may != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(jun.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (jun < Convert.ToInt32(average) && jun != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(jul.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (jul < Convert.ToInt32(average) && jul != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(aug.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (aug < Convert.ToInt32(average) && aug != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(sep.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sep < Convert.ToInt32(average) && sep != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(oct.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (oct < Convert.ToInt32(average) && oct != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(nov.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (nov < Convert.ToInt32(average) && nov != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(dec.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (dec < Convert.ToInt32(average) && dec != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(total.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);

                            totalSubTotal++;
                            totalRowBranchHead++;
                            totalRowColor++;
                            totalRowDealer++;
                            totalRowModel++;
                            totalRowSalesCo++;//<-- Untuk masukkin total nilai sub total tiap Model Catagory
                            totalRowSalesHead++;
                            totalRowSalsman++;
                            rowIndex++;

                            //average = totalGT / salesmanMonth;
                            average = totalGT / salesmanMonth;

                            //Masukkin sub total model category
                            excelReport.SetCellValue("SubTotal", 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth + colorWidth, ExcelCellStyle.PurpleTotal);
                            excelReport.SetCellValue(janGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (janGT < Convert.ToInt32(average) && janGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(febGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (febGT < Convert.ToInt32(average) && febGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(marGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (marGT < Convert.ToInt32(average) && marGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(aprGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (aprGT < Convert.ToInt32(average) && aprGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(mayGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (mayGT < Convert.ToInt32(average) && mayGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(junGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (junGT < Convert.ToInt32(average) && junGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(julGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (julGT < Convert.ToInt32(average) && julGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(augGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (augGT < Convert.ToInt32(average) && augGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(sepGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sepGT < Convert.ToInt32(average) && sepGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(octGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (octGT < Convert.ToInt32(average) && octGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(novGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (novGT < Convert.ToInt32(average) && novGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(decGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (decGT < Convert.ToInt32(average) && decGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(totalGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);

                            totalSubTotal++;
                            totalRowBranchHead++;
                            totalRowColor++;
                            totalRowDealer++;
                            totalRowModel++;
                            totalRowSalesCo++;//<-- Untuk masukkin total nilai sub total tiap Model Catagory
                            totalRowSalesHead++;
                            totalRowSalsman++;
                            rowIndex++;

                            jan = feb = mar = apr = may = jun = jul = aug = sep = oct = nov = dec = total = 0;
                            janGT = febGT = marGT = aprGT = mayGT = junGT = julGT = augGT = sepGT = octGT = novGT = decGT = totalGT = 0;

                            if (row["Seq"].ToString() == "1")
                            {
                                if (row["Seq2"].ToString() == "1")
                                {
                                    excelReport.ReplaceRestorePoint(RestorePoint.SalesmanRestorePoint.ToString(), salesMan.ToString(), 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth, totalRowSalsman, salesManWidth, ExcelCellStyle.LeftBorderedStandardWrap);
                                    excelReport.ReplaceRestorePoint(RestorePoint.GradeRestorePoint.ToString(), grade.ToString(), 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth, totalRowSalsman, salesManWidth, ExcelCellStyle.LeftBorderedStandardWrap);
                                    excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesmanRestorePoint.ToString());
                                    excelReport.RestorePoint(8 + rowIndex, RestorePoint.GradeRestorePoint.ToString());

                                }
                                else if (row["Seq2"].ToString() == "2")
                                {
                                    excelReport.ReplaceRestorePoint(RestorePoint.SalesmanRestorePoint.ToString(), salesMan.ToString(), 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth, totalRowSalsman, salesManWidth, ExcelCellStyle.LeftBorderedStandardWrap);
                                    excelReport.ReplaceRestorePoint(RestorePoint.GradeRestorePoint.ToString(), grade.ToString(), 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth, totalRowSalsman, salesManWidth, ExcelCellStyle.LeftBorderedStandardWrap);
                                    excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesmanRestorePoint.ToString());
                                    excelReport.RestorePoint(8 + rowIndex, RestorePoint.GradeRestorePoint.ToString());
                                }
                                else
                                {
                                    excelReport.ReplaceRestorePoint(RestorePoint.SalesmanRestorePoint.ToString(), "TOTAL", 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth, totalRowSalsman, salesManWidth + gradeWidth, ExcelCellStyle.LeftBorderedStandardWrap);
                                    excelReport.RemoveRestorePoint(RestorePoint.GradeRestorePoint.ToString());
                                    excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesmanRestorePoint.ToString());
                                    excelReport.RestorePoint(8 + rowIndex, RestorePoint.GradeRestorePoint.ToString());
                                }
                            }
                            else if (row["Seq"].ToString() == "2")
                            {
                                excelReport.ReplaceRestorePoint(RestorePoint.SalesmanRestorePoint.ToString(), "TOTAL", 0 + dealerWidth + outletWidth + salesHeadWidth, totalRowSalsman, salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.LeftBorderedStandardWrap);
                                excelReport.RemoveRestorePoint(RestorePoint.GradeRestorePoint.ToString());
                                excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesmanRestorePoint.ToString());
                                excelReport.RestorePoint(8 + rowIndex, RestorePoint.GradeRestorePoint.ToString());
                                //excelReport.SetCellValue("TOTAL",8 + rowIndex - totalRowSalsman, 0 + dealerWidth + outletWidth + salesHeadWidth, totalRowSalsman, salesCoordinatorWidth + salesManWidth, ExcelCellStyle.Standard);
                            }
                            else if (row["Seq"].ToString() == "3")
                            {
                                excelReport.ReplaceRestorePoint(RestorePoint.SalesmanRestorePoint.ToString(), "TOTAL", 0 + dealerWidth + outletWidth, totalRowSalsman, salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.LeftBorderedStandardWrap);
                                excelReport.RemoveRestorePoint(RestorePoint.GradeRestorePoint.ToString());
                                excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesmanRestorePoint.ToString());
                                excelReport.RestorePoint(8 + rowIndex, RestorePoint.GradeRestorePoint.ToString());
                                //excelReport.SetCellValue("TOTAL",8 + rowIndex - totalRowSalsman, 0 + dealerWidth + outletWidth, totalRowSalsman, salesHeadWidth + salesCoordinatorWidth + salesManWidth, ExcelCellStyle.Standard);
                            }
                            else if (row["Seq"].ToString() == "4")
                            {
                                excelReport.ReplaceRestorePoint(RestorePoint.SalesmanRestorePoint.ToString(), "TOTAL", 0 + dealerWidth, totalRowSalsman, outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.LeftBorderedStandardWrap);
                                excelReport.RemoveRestorePoint(RestorePoint.GradeRestorePoint.ToString());
                                excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesmanRestorePoint.ToString());
                                excelReport.RestorePoint(8 + rowIndex, RestorePoint.GradeRestorePoint.ToString());
                                //excelReport.SetCellValue("TOTAL",8 + rowIndex - totalRowSalsman, 0 + dealerWidth, totalRowSalsman, outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth, ExcelCellStyle.Standard);
                            }

                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.MarketModelRestorePoint.ToString());
                            excelReport.SetCellValue(row["ColourName"].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + modelWidth, 1, colorWidth, ExcelCellStyle.LeftBorderedStandard);

                            seq = row["Seq"].ToString();
                            salesMan = row["SalesmanName"].ToString();
                            grade = row["Grade"].ToString();
                            marketModel = row["MarketModel"].ToString();
                            modelCatagory = row["ModelCatagory"].ToString();
                            totalRowColor = 0;
                            totalRowSalsman = 0;
                        }
                        else
                        {
                            if (rowIndex == 0)
                            {
                                excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesCoordinatorrestorePoint.ToString());
                                if (parameter[9].ToString() == "1")
                                {
                                    excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesmanRestorePoint.ToString());
                                    excelReport.RestorePoint(8 + rowIndex, RestorePoint.GradeRestorePoint.ToString());
                                }
                                excelReport.RestorePoint(8 + rowIndex, RestorePoint.MarketModelRestorePoint.ToString());
                            }
                            excelReport.SetCellValue(row["ColourName"].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + modelWidth, 1, colorWidth, ExcelCellStyle.LeftBorderedStandard);

                        }
                        #endregion
                    }
                    else if (parameter[11] == "1"
                    && marketModel != (parameter[10] == "1" ? row["MarketModel"].ToString() : "")
                    && modelCatagory == (parameter[10] == "1" ? row["ModelCatagory"].ToString() : ""))
                    {
                        #region Modelcategory
                        if (parameter[9].ToString() == "1" && salesMan == (parameter[9] == "1" ? row["SalesmanName"].ToString() : ""))
                        {
                            excelReport.ReplaceRestorePoint(RestorePoint.MarketModelRestorePoint.ToString(), marketModel.ToString(), 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, totalRowColor, modelWidth, ExcelCellStyle.LeftBorderedStandard);
                            //excelReport.SetCellValue(marketModel.ToString(),8 + rowIndex - totalRowColor, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth, totalRowColor, modelWidth, ExcelCellStyle.LeftBorderedStandard);

                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.MarketModelRestorePoint.ToString());
                            excelReport.SetCellValue(row["ColourName"].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + modelWidth, 1, colorWidth, ExcelCellStyle.LeftBorderedStandard);


                            marketModel = row["MarketModel"].ToString();
                            totalRowColor = 0;
                        }
                        else if (parameter[9].ToString() == "1" && salesMan != (parameter[9] == "1" ? row["SalesmanName"].ToString() : ""))
                        {
                            excelReport.ReplaceRestorePoint(RestorePoint.MarketModelRestorePoint.ToString(), marketModel.ToString(), 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, totalRowColor, modelWidth, ExcelCellStyle.LeftBorderedStandard);
                            decimal average = total / salesmanMonth;
                            //Masukin total Model Catagory
                            excelReport.SetCellValue("Total " + modelCatagory.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth + colorWidth, ExcelCellStyle.YellowTotal);
                            excelReport.SetCellValue(jan.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(feb.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(mar.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(apr.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(may.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(jun.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(jul.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(aug.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(sep.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(oct.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(nov.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(dec.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(total.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
                            totalSubTotal++;
                            totalRowBranchHead++;
                            totalRowColor++;
                            totalRowDealer++;
                            totalRowModel++;
                            totalRowSalesCo++;//<-- Untuk masukkin total nilai sub total tiap Model Catagory
                            totalRowSalesHead++;
                            totalRowSalsman++;
                            rowIndex++;

                            average = totalGT / salesmanMonth;

                            //Masukkin sub total model category
                            excelReport.SetCellValue("SubTotal", 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth + colorWidth, ExcelCellStyle.PurpleTotal);
                            excelReport.SetCellValue(janGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(febGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(marGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(aprGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(mayGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(junGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(julGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(augGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(sepGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(octGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(novGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(decGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(totalGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                            totalSubTotal++;
                            totalRowBranchHead++;
                            totalRowColor++;
                            totalRowDealer++;
                            totalRowModel++;
                            totalRowSalesCo++;//<-- Untuk masukkin total nilai sub total tiap Model Catagory
                            totalRowSalesHead++;
                            totalRowSalsman++;
                            rowIndex++;

                            jan = feb = mar = apr = may = jun = jul = aug = sep = oct = nov = dec = total = 0;
                            janGT = febGT = marGT = aprGT = mayGT = junGT = julGT = augGT = sepGT = octGT = novGT = decGT = totalGT = 0;
                            excelReport.ReplaceRestorePoint(RestorePoint.SalesmanRestorePoint.ToString(), salesMan.ToString(), 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth, totalRowSalsman, salesManWidth, ExcelCellStyle.LeftBorderedStandard);
                            excelReport.ReplaceRestorePoint(RestorePoint.GradeRestorePoint.ToString(), grade.ToString(), 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth, totalRowSalsman, gradeWidth, ExcelCellStyle.LeftBorderedStandard);

                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesmanRestorePoint.ToString());
                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.GradeRestorePoint.ToString());

                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.MarketModelRestorePoint.ToString());
                            excelReport.SetCellValue(row["ColourName"].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + modelWidth, 1, colorWidth, ExcelCellStyle.LeftBorderedStandard);

                            salesMan = row["SalesmanName"].ToString();
                            grade = row["Grade"].ToString();
                            marketModel = row["MarketModel"].ToString();
                            totalRowColor = 0;
                            totalRowSalsman = 0;
                        }
                        else if (parameter[9].ToString() == "0")
                        {
                            if (salesMan == row["SalesmanName"].ToString())
                            {
                                excelReport.ReplaceRestorePoint(RestorePoint.MarketModelRestorePoint.ToString(), marketModel.ToString(), 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, totalRowColor, modelWidth, ExcelCellStyle.LeftBorderedStandard);
                                //excelReport.SetCellValue(marketModel.ToString(),8 + rowIndex - totalRowColor, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth, totalRowColor, modelWidth, ExcelCellStyle.LeftBorderedStandard);

                                excelReport.RestorePoint(8 + rowIndex, RestorePoint.MarketModelRestorePoint.ToString());
                                excelReport.SetCellValue(row["ColourName"].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + modelWidth, 1, colorWidth, ExcelCellStyle.LeftBorderedStandard);


                                marketModel = row["MarketModel"].ToString();
                                totalRowColor = 0;
                            }
                            else if (seq != "1" && salesMan != row["SalesmanName"].ToString())
                            {
                                excelReport.ReplaceRestorePoint(RestorePoint.MarketModelRestorePoint.ToString(), marketModel.ToString(), 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, totalRowColor, modelWidth, ExcelCellStyle.LeftBorderedStandard);

                                decimal average = total / salesmanMonth;
                                //Masukin total Model Catagory
                                excelReport.SetCellValue("Total " + modelCatagory.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth + colorWidth, ExcelCellStyle.YellowTotal);
                                excelReport.SetCellValue(jan.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
                                excelReport.SetCellValue(feb.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
                                excelReport.SetCellValue(mar.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
                                excelReport.SetCellValue(apr.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
                                excelReport.SetCellValue(may.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
                                excelReport.SetCellValue(jun.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
                                excelReport.SetCellValue(jul.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
                                excelReport.SetCellValue(aug.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
                                excelReport.SetCellValue(sep.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
                                excelReport.SetCellValue(oct.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
                                excelReport.SetCellValue(nov.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
                                excelReport.SetCellValue(dec.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
                                excelReport.SetCellValue(total.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
                                excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
                                totalSubTotal++;
                                totalRowBranchHead++;
                                totalRowColor++;
                                totalRowDealer++;
                                totalRowModel++;
                                totalRowSalesCo++;//<-- Untuk masukkin total nilai sub total tiap Model Catagory
                                totalRowSalesHead++;
                                totalRowSalsman++;
                                rowIndex++;

                                average = totalGT / salesmanMonth;

                                //Masukkin sub total model category
                                excelReport.SetCellValue("SubTotal", 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth + colorWidth, ExcelCellStyle.PurpleTotal);
                                excelReport.SetCellValue(janGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                                excelReport.SetCellValue(febGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                                excelReport.SetCellValue(marGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                                excelReport.SetCellValue(aprGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                                excelReport.SetCellValue(mayGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                                excelReport.SetCellValue(junGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                                excelReport.SetCellValue(julGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                                excelReport.SetCellValue(augGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                                excelReport.SetCellValue(sepGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                                excelReport.SetCellValue(octGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                                excelReport.SetCellValue(novGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                                excelReport.SetCellValue(decGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                                excelReport.SetCellValue(totalGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                                excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                                totalSubTotal++;
                                totalRowBranchHead++;
                                totalRowColor++;
                                totalRowDealer++;
                                totalRowModel++;
                                totalRowSalesCo++;//<-- Untuk masukkin total nilai sub total tiap Model Catagory
                                totalRowSalesHead++;
                                totalRowSalsman++;
                                rowIndex++;

                                jan = feb = mar = apr = may = jun = jul = aug = sep = oct = nov = dec = total = 0;
                                janGT = febGT = marGT = aprGT = mayGT = junGT = julGT = augGT = sepGT = octGT = novGT = decGT = totalGT = 0;
                                if (row["Seq"].ToString() != "3")
                                    excelReport.ReplaceRestorePoint(RestorePoint.SalesCoordinatorrestorePoint.ToString(), "TOTAL", 0 + dealerWidth + outletWidth + salesHeadWidth, totalRowSalsman, salesCoordinatorWidth, ExcelCellStyle.LeftBorderedStandardWrap);
                                else if (row["Seq"].ToString() == "3")
                                    excelReport.ReplaceRestorePoint(RestorePoint.SalesCoordinatorrestorePoint.ToString(), "TOTAL", 0 + dealerWidth + outletWidth, totalRowSalsman, salesHeadWidth + salesCoordinatorWidth, ExcelCellStyle.LeftBorderedStandardWrap);
                                else if (row["Seq"].ToString() == "4")
                                    excelReport.ReplaceRestorePoint(RestorePoint.SalesCoordinatorrestorePoint.ToString(), "TOTAL", 0 + dealerWidth, totalRowSalsman, outletWidth + salesHeadWidth + salesCoordinatorWidth, ExcelCellStyle.LeftBorderedStandardWrap);

                                excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesCoordinatorrestorePoint.ToString());
                                excelReport.RestorePoint(8 + rowIndex, RestorePoint.MarketModelRestorePoint.ToString());
                                excelReport.SetCellValue(row["ColourName"].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + modelWidth, 1, colorWidth, ExcelCellStyle.LeftBorderedStandard);

                                marketModel = row["MarketModel"].ToString();
                                modelCatagory = row["ModelCatagory"].ToString();
                                salesMan = row["SalesmanName"].ToString();
                                grade = row["Grade"].ToString();
                                totalRowSalsman = 0;
                                totalRowSalesCo = 0;
                                totalRowColor = 0;
                            }
                            else if (salesMan != row["SalesmanName"].ToString())
                            {
                                excelReport.ReplaceRestorePoint(RestorePoint.MarketModelRestorePoint.ToString(), marketModel.ToString(), 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, totalRowColor, modelWidth, ExcelCellStyle.LeftBorderedStandard);
                                //excelReport.SetCellValue(marketModel.ToString(),8 + rowIndex - totalRowColor, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth, totalRowColor, modelWidth, ExcelCellStyle.LeftBorderedStandard);

                                excelReport.RestorePoint(8 + rowIndex, RestorePoint.MarketModelRestorePoint.ToString());
                                excelReport.SetCellValue(row["ColourName"].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + modelWidth, 1, colorWidth, ExcelCellStyle.LeftBorderedStandard);


                                marketModel = row["MarketModel"].ToString();
                                salesMan = row["SalesmanName"].ToString();
                                grade = row["Grade"].ToString();
                                totalRowColor = 0;
                            }
                            else
                            {
                                excelReport.ReplaceRestorePoint(RestorePoint.MarketModelRestorePoint.ToString(), marketModel.ToString(), 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, totalRowColor, modelWidth, ExcelCellStyle.LeftBorderedStandard);

                                decimal average = total / salesmanMonth;

                                //excelReport.SetCellValue(marketModel.ToString(),8 + rowIndex - totalRowColor, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth, totalRowColor, modelWidth, ExcelCellStyle.Standard);
                                //Masukin total Model Catagory
                                excelReport.SetCellValue("Total " + modelCatagory.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth + colorWidth, ExcelCellStyle.YellowTotal);
                                excelReport.SetCellValue(jan.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
                                excelReport.SetCellValue(feb.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
                                excelReport.SetCellValue(mar.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
                                excelReport.SetCellValue(apr.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
                                excelReport.SetCellValue(may.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
                                excelReport.SetCellValue(jun.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
                                excelReport.SetCellValue(jul.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
                                excelReport.SetCellValue(aug.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
                                excelReport.SetCellValue(sep.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
                                excelReport.SetCellValue(oct.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
                                excelReport.SetCellValue(nov.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
                                excelReport.SetCellValue(dec.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
                                excelReport.SetCellValue(total.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
                                excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
                                totalSubTotal++;
                                totalRowBranchHead++;
                                totalRowColor++;
                                totalRowDealer++;
                                totalRowModel++;
                                totalRowSalesCo++;//<-- Untuk masukkin total nilai sub total tiap Model Catagory
                                totalRowSalesHead++;
                                totalRowSalsman++;
                                rowIndex++;

                                jan = feb = mar = apr = may = jun = jul = aug = sep = oct = nov = dec = total = 0;

                                excelReport.RestorePoint(8 + rowIndex, RestorePoint.MarketModelRestorePoint.ToString());
                                excelReport.SetCellValue(row["ColourName"].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + modelWidth, 1, colorWidth, ExcelCellStyle.LeftBorderedStandard);
                                marketModel = row["MarketModel"].ToString();
                                modelCatagory = row["ModelCatagory"].ToString();
                                totalRowColor = 0;
                            }
                        }
                        #endregion
                    }
                    else if (parameter[11] == "1"
              && marketModel != (parameter[10] == "1" ? row["MarketModel"].ToString() : "")
              && modelCatagory != (parameter[10] == "1" ? row["ModelCatagory"].ToString() : ""))
                    {
                        #region group method
                        if (parameter[9].ToString() == "1" && salesMan == (parameter[9] == "1" ? row["SalesmanName"].ToString() : ""))
                        {
                            excelReport.ReplaceRestorePoint(RestorePoint.MarketModelRestorePoint.ToString(), marketModel.ToString(), 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, totalRowColor, modelWidth, ExcelCellStyle.LeftBorderedStandard);

                            decimal average = total / salesmanMonth;

                            //excelReport.SetCellValue(marketModel.ToString(),8 + rowIndex - totalRowColor, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth, totalRowColor, modelWidth, ExcelCellStyle.Standard);
                            //Masukin total Model Catagory
                            excelReport.SetCellValue("Total " + modelCatagory.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth + colorWidth, ExcelCellStyle.YellowTotal);
                            excelReport.SetCellValue(jan.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (jan < Convert.ToInt32(average) && jan != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(feb.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (feb < Convert.ToInt32(average) && feb != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(mar.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (mar < Convert.ToInt32(average) && mar != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(apr.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (apr < Convert.ToInt32(average) && apr != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(may.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (may < Convert.ToInt32(average) && may != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(jun.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (jun < Convert.ToInt32(average) && jun != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(jul.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (jul < Convert.ToInt32(average) && jul != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(aug.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (aug < Convert.ToInt32(average) && aug != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(sep.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sep < Convert.ToInt32(average) && sep != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(oct.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (oct < Convert.ToInt32(average) && oct != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(nov.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (nov < Convert.ToInt32(average) && nov != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(dec.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (dec < Convert.ToInt32(average) && dec != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(total.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);

                            totalSubTotal++;
                            totalRowBranchHead++;
                            totalRowColor++;
                            totalRowDealer++;
                            totalRowModel++;
                            totalRowSalesCo++;//<-- Untuk masukkin total nilai sub total tiap Model Catagory
                            totalRowSalesHead++;
                            totalRowSalsman++;
                            rowIndex++;

                            jan = feb = mar = apr = may = jun = jul = aug = sep = oct = nov = dec = total = 0;

                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.MarketModelRestorePoint.ToString());
                            excelReport.SetCellValue(row["ColourName"].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + modelWidth, 1, colorWidth, ExcelCellStyle.LeftBorderedStandard);
                            marketModel = row["MarketModel"].ToString();
                            modelCatagory = row["ModelCatagory"].ToString();
                            totalRowColor = 0;
                        }
                        else if (parameter[9].ToString() == "1" && salesMan != (parameter[9] == "1" ? row["SalesmanName"].ToString() : ""))
                        {
                            excelReport.ReplaceRestorePoint(RestorePoint.MarketModelRestorePoint.ToString(), marketModel.ToString(), 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, totalRowColor, modelWidth, ExcelCellStyle.LeftBorderedStandard);
                            decimal average = total / salesmanMonth;
                            //Masukin total Model Catagory
                            excelReport.SetCellValue("Total " + modelCatagory.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth + colorWidth, ExcelCellStyle.YellowTotal);
                            excelReport.SetCellValue(jan.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (jan < Convert.ToInt32(average) && jan != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(feb.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (feb < Convert.ToInt32(average) && feb != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(mar.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (mar < Convert.ToInt32(average) && mar != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(apr.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (apr < Convert.ToInt32(average) && apr != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(may.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (may < Convert.ToInt32(average) && may != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(jun.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (jun < Convert.ToInt32(average) && jun != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(jul.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (jul < Convert.ToInt32(average) && jul != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(aug.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (aug < Convert.ToInt32(average) && aug != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(sep.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sep < Convert.ToInt32(average) && sep != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(oct.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (oct < Convert.ToInt32(average) && oct != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(nov.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (nov < Convert.ToInt32(average) && nov != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(dec.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (dec < Convert.ToInt32(average) && dec != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(total.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);

                            totalSubTotal++;
                            totalRowBranchHead++;
                            totalRowColor++;
                            totalRowDealer++;
                            totalRowModel++;
                            totalRowSalesCo++;//<-- Untuk masukkin total nilai sub total tiap Model Catagory
                            totalRowSalesHead++;
                            totalRowSalsman++;
                            rowIndex++;

                            average = totalGT / salesmanMonth;

                            //Masukkin sub total model category
                            excelReport.SetCellValue("SubTotal", 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth + colorWidth, ExcelCellStyle.PurpleTotal);
                            excelReport.SetCellValue(janGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (janGT < Convert.ToInt32(average) && janGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(febGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (febGT < Convert.ToInt32(average) && febGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(marGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (marGT < Convert.ToInt32(average) && marGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(aprGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (aprGT < Convert.ToInt32(average) && aprGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(mayGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (mayGT < Convert.ToInt32(average) && mayGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(junGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (junGT < Convert.ToInt32(average) && junGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(julGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (julGT < Convert.ToInt32(average) && julGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(augGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (augGT < Convert.ToInt32(average) && augGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(sepGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sepGT < Convert.ToInt32(average) && sepGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(octGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (octGT < Convert.ToInt32(average) && octGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(novGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (novGT < Convert.ToInt32(average) && novGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(decGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (decGT < Convert.ToInt32(average) && decGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(totalGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);

                            totalSubTotal++;
                            totalRowBranchHead++;
                            totalRowColor++;
                            totalRowDealer++;
                            totalRowModel++;
                            totalRowSalesCo++;//<-- Untuk masukkin total nilai sub total tiap Model Catagory
                            totalRowSalesHead++;
                            totalRowSalsman++;
                            rowIndex++;

                            jan = feb = mar = apr = may = jun = jul = aug = sep = oct = nov = dec = total = 0;
                            janGT = febGT = marGT = aprGT = mayGT = junGT = julGT = augGT = sepGT = octGT = novGT = decGT = totalGT = 0;

                            if (row["Seq"].ToString() == "1")
                            {
                                if (row["Seq2"].ToString() == "1")
                                {
                                    excelReport.ReplaceRestorePoint(RestorePoint.SalesmanRestorePoint.ToString(), salesMan.ToString(), 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth, totalRowSalsman, salesManWidth, ExcelCellStyle.LeftBorderedStandardWrap);
                                    excelReport.ReplaceRestorePoint(RestorePoint.GradeRestorePoint.ToString(), grade.ToString(), 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth, totalRowSalsman, salesManWidth, ExcelCellStyle.LeftBorderedStandardWrap);
                                    excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesmanRestorePoint.ToString());
                                    excelReport.RestorePoint(8 + rowIndex, RestorePoint.GradeRestorePoint.ToString());

                                }
                                else if (row["Seq2"].ToString() == "2")
                                {
                                    excelReport.ReplaceRestorePoint(RestorePoint.SalesmanRestorePoint.ToString(), salesMan.ToString(), 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth, totalRowSalsman, salesManWidth, ExcelCellStyle.LeftBorderedStandardWrap);
                                    excelReport.ReplaceRestorePoint(RestorePoint.GradeRestorePoint.ToString(), grade.ToString(), 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth, totalRowSalsman, salesManWidth, ExcelCellStyle.LeftBorderedStandardWrap);
                                    excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesmanRestorePoint.ToString());
                                    excelReport.RestorePoint(8 + rowIndex, RestorePoint.GradeRestorePoint.ToString());
                                }
                                else
                                {
                                    excelReport.ReplaceRestorePoint(RestorePoint.SalesmanRestorePoint.ToString(), "TOTAL", 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth, totalRowSalsman, salesManWidth + gradeWidth, ExcelCellStyle.LeftBorderedStandardWrap);
                                    excelReport.RemoveRestorePoint(RestorePoint.GradeRestorePoint.ToString());
                                    excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesmanRestorePoint.ToString());
                                    excelReport.RestorePoint(8 + rowIndex, RestorePoint.GradeRestorePoint.ToString());
                                }
                            }
                            else if (row["Seq"].ToString() == "2")
                            {
                                excelReport.ReplaceRestorePoint(RestorePoint.SalesmanRestorePoint.ToString(), "TOTAL", 0 + dealerWidth + outletWidth + salesHeadWidth, totalRowSalsman, salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.LeftBorderedStandardWrap);
                                excelReport.RemoveRestorePoint(RestorePoint.GradeRestorePoint.ToString());
                                excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesmanRestorePoint.ToString());
                                excelReport.RestorePoint(8 + rowIndex, RestorePoint.GradeRestorePoint.ToString());
                                //excelReport.SetCellValue("TOTAL",8 + rowIndex - totalRowSalsman, 0 + dealerWidth + outletWidth + salesHeadWidth, totalRowSalsman, salesCoordinatorWidth + salesManWidth, ExcelCellStyle.Standard);
                            }
                            else if (row["Seq"].ToString() == "3")
                            {
                                excelReport.ReplaceRestorePoint(RestorePoint.SalesmanRestorePoint.ToString(), "TOTAL", 0 + dealerWidth + outletWidth, totalRowSalsman, salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.LeftBorderedStandardWrap);
                                excelReport.RemoveRestorePoint(RestorePoint.GradeRestorePoint.ToString());
                                excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesmanRestorePoint.ToString());
                                excelReport.RestorePoint(8 + rowIndex, RestorePoint.GradeRestorePoint.ToString());
                                //excelReport.SetCellValue("TOTAL",8 + rowIndex - totalRowSalsman, 0 + dealerWidth + outletWidth, totalRowSalsman, salesHeadWidth + salesCoordinatorWidth + salesManWidth, ExcelCellStyle.Standard);
                            }
                            else if (row["Seq"].ToString() == "4")
                            {
                                excelReport.ReplaceRestorePoint(RestorePoint.SalesmanRestorePoint.ToString(), "TOTAL", 0 + dealerWidth, totalRowSalsman, outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.LeftBorderedStandardWrap);
                                excelReport.RemoveRestorePoint(RestorePoint.GradeRestorePoint.ToString());
                                excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesmanRestorePoint.ToString());
                                excelReport.RestorePoint(8 + rowIndex, RestorePoint.GradeRestorePoint.ToString());
                                //excelReport.SetCellValue("TOTAL",8 + rowIndex - totalRowSalsman, 0 + dealerWidth, totalRowSalsman, outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth, ExcelCellStyle.Standard);
                            }

                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.MarketModelRestorePoint.ToString());
                            excelReport.SetCellValue(row["ColourName"].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + modelWidth, 1, colorWidth, ExcelCellStyle.LeftBorderedStandard);

                            seq = row["Seq"].ToString();
                            salesMan = row["SalesmanName"].ToString();
                            grade = row["Grade"].ToString();
                            marketModel = row["MarketModel"].ToString();
                            modelCatagory = row["ModelCatagory"].ToString();
                            totalRowColor = 0;
                            totalRowSalsman = 0;
                        }
                        else if (parameter[9].ToString() == "0")
                        {
                            if (seq != "1" && salesMan != row["SalesmanName"].ToString())
                            {
                                excelReport.ReplaceRestorePoint(RestorePoint.MarketModelRestorePoint.ToString(), marketModel.ToString(), 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, totalRowColor, modelWidth, ExcelCellStyle.LeftBorderedStandard);

                                decimal average = total / salesmanMonth;
                                //Masukin total Model Catagory
                                excelReport.SetCellValue("Total " + modelCatagory.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth + colorWidth, ExcelCellStyle.YellowTotal);
                                excelReport.SetCellValue(jan.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (jan < Convert.ToInt32(average) && jan != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                                excelReport.SetCellValue(feb.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (feb < Convert.ToInt32(average) && feb != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                                excelReport.SetCellValue(mar.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (mar < Convert.ToInt32(average) && mar != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                                excelReport.SetCellValue(apr.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (apr < Convert.ToInt32(average) && apr != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                                excelReport.SetCellValue(may.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (may < Convert.ToInt32(average) && may != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                                excelReport.SetCellValue(jun.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (jun < Convert.ToInt32(average) && jun != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                                excelReport.SetCellValue(jul.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (jul < Convert.ToInt32(average) && jul != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                                excelReport.SetCellValue(aug.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (aug < Convert.ToInt32(average) && aug != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                                excelReport.SetCellValue(sep.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sep < Convert.ToInt32(average) && sep != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                                excelReport.SetCellValue(oct.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (oct < Convert.ToInt32(average) && oct != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                                excelReport.SetCellValue(nov.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (nov < Convert.ToInt32(average) && nov != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                                excelReport.SetCellValue(dec.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (dec < Convert.ToInt32(average) && dec != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                                excelReport.SetCellValue(total.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
                                excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);

                                totalSubTotal++;
                                totalRowBranchHead++;
                                totalRowColor++;
                                totalRowDealer++;
                                totalRowModel++;
                                totalRowSalesCo++;//<-- Untuk masukkin total nilai sub total tiap Model Catagory
                                totalRowSalesHead++;
                                totalRowSalsman++;
                                rowIndex++;

                                average = totalGT / salesmanMonth;

                                //Masukkin sub total model category
                                excelReport.SetCellValue("SubTotal", 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth + colorWidth, ExcelCellStyle.PurpleTotal);
                                excelReport.SetCellValue(janGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (janGT < Convert.ToInt32(average) && janGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                                excelReport.SetCellValue(febGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (febGT < Convert.ToInt32(average) && febGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                                excelReport.SetCellValue(marGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (marGT < Convert.ToInt32(average) && marGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                                excelReport.SetCellValue(aprGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (aprGT < Convert.ToInt32(average) && aprGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                                excelReport.SetCellValue(mayGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (mayGT < Convert.ToInt32(average) && mayGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                                excelReport.SetCellValue(junGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (junGT < Convert.ToInt32(average) && junGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                                excelReport.SetCellValue(julGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (julGT < Convert.ToInt32(average) && julGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                                excelReport.SetCellValue(augGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (augGT < Convert.ToInt32(average) && augGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                                excelReport.SetCellValue(sepGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sepGT < Convert.ToInt32(average) && sepGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                                excelReport.SetCellValue(octGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (octGT < Convert.ToInt32(average) && octGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                                excelReport.SetCellValue(novGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (novGT < Convert.ToInt32(average) && novGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                                excelReport.SetCellValue(decGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (decGT < Convert.ToInt32(average) && decGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                                excelReport.SetCellValue(totalGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                                excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);

                                totalSubTotal++;
                                totalRowBranchHead++;
                                totalRowColor++;
                                totalRowDealer++;
                                totalRowModel++;
                                totalRowSalesCo++;//<-- Untuk masukkin total nilai sub total tiap Model Catagory
                                totalRowSalesHead++;
                                totalRowSalsman++;
                                rowIndex++;

                                jan = feb = mar = apr = may = jun = jul = aug = sep = oct = nov = dec = total = 0;
                                janGT = febGT = marGT = aprGT = mayGT = junGT = julGT = augGT = sepGT = octGT = novGT = decGT = totalGT = 0;
                                if (row["Seq"].ToString() == "4")
                                    excelReport.ReplaceRestorePoint(RestorePoint.SalesmanRestorePoint.ToString(), "TOTAL", 0 + dealerWidth, totalRowSalsman, outletWidth + salesHeadWidth + salesCoordinatorWidth, ExcelCellStyle.LeftBorderedStandardWrap);
                                else if (row["Seq"].ToString() == "3")
                                    excelReport.ReplaceRestorePoint(RestorePoint.SalesCoordinatorrestorePoint.ToString(), "TOTAL", 0 + dealerWidth + outletWidth, totalRowSalsman, salesHeadWidth + salesCoordinatorWidth, ExcelCellStyle.LeftBorderedStandardWrap);
                                else if (row["Seq"].ToString() != "3")
                                    excelReport.ReplaceRestorePoint(RestorePoint.SalesCoordinatorrestorePoint.ToString(), "TOTAL", 0 + dealerWidth + outletWidth + salesHeadWidth, totalRowSalsman, salesCoordinatorWidth, ExcelCellStyle.LeftBorderedStandardWrap);

                                excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesCoordinatorrestorePoint.ToString());
                                excelReport.RestorePoint(8 + rowIndex, RestorePoint.MarketModelRestorePoint.ToString());
                                excelReport.SetCellValue(row["ColourName"].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + modelWidth, 1, colorWidth, ExcelCellStyle.LeftBorderedStandard);

                                marketModel = row["MarketModel"].ToString();
                                modelCatagory = row["ModelCatagory"].ToString();
                                salesMan = row["SalesmanName"].ToString();
                                grade = row["Grade"].ToString();
                                totalRowSalsman = 0;
                                totalRowSalesCo = 0;
                                totalRowColor = 0;
                            }
                            else
                            {
                                excelReport.ReplaceRestorePoint(RestorePoint.MarketModelRestorePoint.ToString(), marketModel.ToString(), 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, totalRowColor, modelWidth, ExcelCellStyle.LeftBorderedStandard);

                                decimal average = total / salesmanMonth;
                                //excelReport.SetCellValue(marketModel.ToString(),8 + rowIndex - totalRowColor, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth, totalRowColor, modelWidth, ExcelCellStyle.Standard);
                                //Masukin total Model Catagory
                                excelReport.SetCellValue("Total " + modelCatagory.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth + colorWidth, ExcelCellStyle.YellowTotal);
                                excelReport.SetCellValue(jan.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (jan < Convert.ToInt32(average) && jan != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                                excelReport.SetCellValue(feb.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (feb < Convert.ToInt32(average) && feb != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                                excelReport.SetCellValue(mar.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (mar < Convert.ToInt32(average) && mar != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                                excelReport.SetCellValue(apr.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (apr < Convert.ToInt32(average) && apr != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                                excelReport.SetCellValue(may.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (may < Convert.ToInt32(average) && may != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                                excelReport.SetCellValue(jun.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (jun < Convert.ToInt32(average) && jun != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                                excelReport.SetCellValue(jul.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (jul < Convert.ToInt32(average) && jul != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                                excelReport.SetCellValue(aug.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (aug < Convert.ToInt32(average) && aug != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                                excelReport.SetCellValue(sep.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sep < Convert.ToInt32(average) && sep != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                                excelReport.SetCellValue(oct.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (oct < Convert.ToInt32(average) && oct != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                                excelReport.SetCellValue(nov.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (nov < Convert.ToInt32(average) && nov != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                                excelReport.SetCellValue(dec.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (dec < Convert.ToInt32(average) && dec != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                                excelReport.SetCellValue(total.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
                                excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);

                                totalSubTotal++;
                                totalRowBranchHead++;
                                totalRowColor++;
                                totalRowDealer++;
                                totalRowModel++;
                                totalRowSalesCo++;//<-- Untuk masukkin total nilai sub total tiap Model Catagory
                                totalRowSalesHead++;
                                totalRowSalsman++;
                                rowIndex++;

                                jan = feb = mar = apr = may = jun = jul = aug = sep = oct = nov = dec = total = 0;

                                excelReport.RestorePoint(8 + rowIndex, RestorePoint.MarketModelRestorePoint.ToString());
                                excelReport.SetCellValue(row["ColourName"].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + modelWidth, 1, colorWidth, ExcelCellStyle.LeftBorderedStandard);
                                marketModel = row["MarketModel"].ToString();
                                modelCatagory = row["ModelCatagory"].ToString();
                                totalRowColor = 0;
                            }
                        }
                        #endregion
                    }
                    #endregion
                    #region Parameter10 = "1"
                    else if (parameter[11] == "0" && parameter[10] == "1")
                    {
                        if (modelCatagory != row["ModelCatagory"].ToString() && salesMan == row["SalesmanName"].ToString())
                        {
                            //Masukin total Model Catagory
                            //decimal average = total / salesmanMonth;
                            decimal average = total / salesmanMonth;

                            excelReport.SetCellValue("Total " + modelCatagory.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth, ExcelCellStyle.YellowTotal);
                            excelReport.SetCellValue(jan.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (jan < Convert.ToInt32(average) && jan != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(feb.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (feb < Convert.ToInt32(average) && feb != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(mar.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (mar < Convert.ToInt32(average) && mar != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(apr.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (apr < Convert.ToInt32(average) && apr != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(may.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (may < Convert.ToInt32(average) && may != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(jun.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (jun < Convert.ToInt32(average) && jun != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(jul.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (jul < Convert.ToInt32(average) && jul != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(aug.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (aug < Convert.ToInt32(average) && aug != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(sep.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sep < Convert.ToInt32(average) && sep != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(oct.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (oct < Convert.ToInt32(average) && oct != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(nov.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (nov < Convert.ToInt32(average) && nov != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(dec.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (dec < Convert.ToInt32(average) && dec != 0) ? ExcelCellStyle.YellowTotalRedNumber : ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(total.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
                            excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
                            totalSubTotal++;
                            totalRowBranchHead++;
                            totalRowDealer++;
                            totalRowModel++;
                            totalRowSalesCo++;//<-- Untuk masukkin total nilai sub total tiap Model Catagory
                            totalRowSalesHead++;
                            totalRowSalsman++;
                            rowIndex++;

                            jan = feb = mar = apr = may = jun = jul = aug = sep = oct = nov = dec = total = 0;
                        }
                        else if (parameter[9].ToString() == "1" && salesMan != row["SalesmanName"].ToString())
                        {
                            decimal average = total / salesmanMonth;
                            //Masukin total Model Catagory
                            excelReport.SetCellValue("Total " + modelCatagory.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth, ExcelCellStyle.BlueTotal);
                            excelReport.SetCellValue(jan.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (jan < Convert.ToInt32(average) && jan != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                            excelReport.SetCellValue(feb.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (feb < Convert.ToInt32(average) && feb != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                            excelReport.SetCellValue(mar.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (mar < Convert.ToInt32(average) && mar != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                            excelReport.SetCellValue(apr.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (apr < Convert.ToInt32(average) && apr != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                            excelReport.SetCellValue(may.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (may < Convert.ToInt32(average) && may != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                            excelReport.SetCellValue(jun.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (jun < Convert.ToInt32(average) && jun != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                            excelReport.SetCellValue(jul.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (jul < Convert.ToInt32(average) && jul != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                            excelReport.SetCellValue(aug.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (aug < Convert.ToInt32(average) && aug != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                            excelReport.SetCellValue(sep.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sep < Convert.ToInt32(average) && sep != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                            excelReport.SetCellValue(oct.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (oct < Convert.ToInt32(average) && oct != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                            excelReport.SetCellValue(nov.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (nov < Convert.ToInt32(average) && nov != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                            excelReport.SetCellValue(dec.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (dec < Convert.ToInt32(average) && dec != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                            excelReport.SetCellValue(total.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                            excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);

                            totalSubTotal++;
                            totalRowBranchHead++;
                            totalRowDealer++;
                            totalRowModel++;
                            totalRowSalesCo++;//<-- Untuk masukkin total nilai sub total tiap Model Catagory
                            totalRowSalesHead++;
                            totalRowSalsman++;
                            rowIndex++;

                            average = totalGT / salesmanMonth;
                            //Masukkin sub total model category
                            excelReport.SetCellValue("SubTotal", 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth, ExcelCellStyle.PurpleTotal);
                            excelReport.SetCellValue(janGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (janGT < Convert.ToInt32(average) && janGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(febGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (febGT < Convert.ToInt32(average) && febGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(marGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (marGT < Convert.ToInt32(average) && marGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(aprGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (aprGT < Convert.ToInt32(average) && aprGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(mayGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (mayGT < Convert.ToInt32(average) && mayGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(junGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (junGT < Convert.ToInt32(average) && junGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(julGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (julGT < Convert.ToInt32(average) && julGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(augGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (augGT < Convert.ToInt32(average) && augGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(sepGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sepGT < Convert.ToInt32(average) && sepGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(octGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (octGT < Convert.ToInt32(average) && octGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(novGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (novGT < Convert.ToInt32(average) && novGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(decGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (decGT < Convert.ToInt32(average) && decGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(totalGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);

                            totalSubTotal++;
                            totalRowBranchHead++;
                            totalRowDealer++;
                            totalRowModel++;
                            totalRowSalesCo++;//<-- Untuk masukkin total nilai sub total tiap Model Catagory
                            totalRowSalesHead++;
                            totalRowSalsman++;
                            rowIndex++;

                            jan = feb = mar = apr = may = jun = jul = aug = sep = oct = nov = dec = total = 0;
                            janGT = febGT = marGT = aprGT = mayGT = junGT = julGT = augGT = sepGT = octGT = novGT = decGT = totalGT = 0;
                            if (seq == "1" && seq2 == "2")
                            {
                                excelReport.ReplaceRestorePoint(RestorePoint.SalesmanRestorePoint.ToString(), "TOTAL", 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth, totalRowModel, salesManWidth + gradeWidth, ExcelCellStyle.LeftBorderedStandard);
                                excelReport.RemoveRestorePoint(RestorePoint.GradeRestorePoint.ToString());
                                excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesmanRestorePoint.ToString());
                                excelReport.RestorePoint(8 + rowIndex, RestorePoint.GradeRestorePoint.ToString());
                                totalRowModel = 0;
                            }
                            else if (seq == "2" && seq2 == "1")
                            {
                                excelReport.ReplaceRestorePoint(RestorePoint.SalesmanRestorePoint.ToString(), "TOTAL", 0 + dealerWidth + outletWidth + salesHeadWidth, totalRowModel, salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.LeftBorderedStandard);
                                excelReport.RemoveRestorePoint(RestorePoint.GradeRestorePoint.ToString());
                                excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesmanRestorePoint.ToString());
                                excelReport.RestorePoint(8 + rowIndex, RestorePoint.GradeRestorePoint.ToString());

                                //excelReport.SetCellValue("TOTAL",8 + rowIndex - totalRowModel, 0 + dealerWidth + outletWidth + salesHeadWidth, totalRowModel, salesCoordinatorWidth + salesManWidth, ExcelCellStyle.Standard);
                            }
                            else if (seq == "3" && seq2 == "2")
                            {
                                excelReport.ReplaceRestorePoint(RestorePoint.SalesmanRestorePoint.ToString(), "TOTAL", 0 + dealerWidth + outletWidth, totalRowModel, salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.LeftBorderedStandard);
                                excelReport.RemoveRestorePoint(RestorePoint.GradeRestorePoint.ToString());
                                excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesmanRestorePoint.ToString());
                                excelReport.RestorePoint(8 + rowIndex, RestorePoint.GradeRestorePoint.ToString());
                            }
                            else if (seq == "4" && seq2 == "2")
                            {
                                excelReport.ReplaceRestorePoint(RestorePoint.SalesmanRestorePoint.ToString(), "TOTAL", 0 + dealerWidth, totalRowModel, outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.LeftBorderedStandard);
                                excelReport.RemoveRestorePoint(RestorePoint.GradeRestorePoint.ToString());
                                excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesmanRestorePoint.ToString());
                            }
                            else
                            {
                                excelReport.ReplaceRestorePoint(RestorePoint.SalesmanRestorePoint.ToString(), salesMan.ToString(), 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth, totalRowSalsman, salesManWidth, ExcelCellStyle.LeftBorderedStandard);
                                excelReport.ReplaceRestorePoint(RestorePoint.GradeRestorePoint.ToString(), grade.ToString(), 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth, totalRowSalsman, gradeWidth, ExcelCellStyle.LeftBorderedStandard);
                                excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesmanRestorePoint.ToString());
                                excelReport.RestorePoint(8 + rowIndex, RestorePoint.GradeRestorePoint.ToString());
                            }


                            totalRowSalsman = 0;
                            totalRowModel = 0;
                        }
                        else if (modelCatagory != row["ModelCatagory"].ToString() && salesMan != row["SalesmanName"].ToString())
                        {
                            decimal average = total / salesmanMonth;
                            //Masukin total Model Catagory
                            excelReport.SetCellValue("Total " + modelCatagory.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth, ExcelCellStyle.BlueTotal);
                            excelReport.SetCellValue(jan.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                            excelReport.SetCellValue(feb.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                            excelReport.SetCellValue(mar.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                            excelReport.SetCellValue(apr.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                            excelReport.SetCellValue(may.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                            excelReport.SetCellValue(jun.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                            excelReport.SetCellValue(jul.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                            excelReport.SetCellValue(aug.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                            excelReport.SetCellValue(sep.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                            excelReport.SetCellValue(oct.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                            excelReport.SetCellValue(nov.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                            excelReport.SetCellValue(dec.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                            excelReport.SetCellValue(total.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                            excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);

                            totalSubTotal++;
                            totalRowBranchHead++;
                            totalRowDealer++;
                            totalRowModel++;
                            totalRowSalesCo++;//<-- Untuk masukkin total nilai sub total tiap Model Catagory
                            totalRowSalesHead++;
                            totalRowSalsman++;
                            rowIndex++;

                            average = totalGT / salesmanMonth;
                            //Masukkin sub total model category
                            excelReport.SetCellValue("SubTotal", 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth, ExcelCellStyle.PurpleTotal);
                            excelReport.SetCellValue(janGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (janGT < Convert.ToInt32(average) && janGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(febGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (febGT < Convert.ToInt32(average) && febGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(marGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (marGT < Convert.ToInt32(average) && marGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(aprGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (aprGT < Convert.ToInt32(average) && aprGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(mayGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (mayGT < Convert.ToInt32(average) && mayGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(junGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (junGT < Convert.ToInt32(average) && junGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(julGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (julGT < Convert.ToInt32(average) && julGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(augGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (augGT < Convert.ToInt32(average) && augGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(sepGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sepGT < Convert.ToInt32(average) && sepGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(octGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (octGT < Convert.ToInt32(average) && octGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(novGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (novGT < Convert.ToInt32(average) && novGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(decGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (decGT < Convert.ToInt32(average) && decGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(totalGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                            excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);

                            totalSubTotal++;
                            totalRowBranchHead++;
                            totalRowDealer++;
                            totalRowModel++;
                            totalRowSalesCo++;//<-- Untuk masukkin total nilai sub total tiap Model Catagory
                            totalRowSalesHead++;
                            totalRowSalsman++;
                            rowIndex++;

                            jan = feb = mar = apr = may = jun = jul = aug = sep = oct = nov = dec = total = 0;
                            janGT = febGT = marGT = aprGT = mayGT = junGT = julGT = augGT = sepGT = octGT = novGT = decGT = totalGT = 0;

                            if (seq == "2" && seq2 == "1")
                            {
                                excelReport.ReplaceRestorePoint(RestorePoint.SalesCoordinatorrestorePoint.ToString(), "TOTAL", 0 + dealerWidth + outletWidth + salesHeadWidth, totalRowModel, salesCoordinatorWidth + (parameter[9].ToString() == "1" ? 2 : 0), ExcelCellStyle.LeftBorderedStandard);
                                excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesCoordinatorrestorePoint.ToString());

                                //excelReport.SetCellValue("TOTAL",8 + rowIndex - totalRowModel, 0 + dealerWidth + outletWidth + salesHeadWidth, totalRowModel, salesCoordinatorWidth + salesManWidth, ExcelCellStyle.Standard);
                            }
                            else if (seq == "3" && seq2 == "2")
                            {
                                excelReport.ReplaceRestorePoint(RestorePoint.SalesCoordinatorrestorePoint.ToString(), "TOTAL", 0 + dealerWidth + outletWidth, totalRowSalesCo, salesHeadWidth + salesCoordinatorWidth + (parameter[9].ToString() == "1" ? 2 : 0), ExcelCellStyle.LeftBorderedStandard);
                                excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesCoordinatorrestorePoint.ToString());
                            }
                            else if (seq == "4" && seq2 == "2")
                            {
                                excelReport.ReplaceRestorePoint(RestorePoint.SalesCoordinatorrestorePoint.ToString(), "TOTAL", 0 + dealerWidth, totalRowModel, outletWidth + salesHeadWidth + salesCoordinatorWidth + +(parameter[9].ToString() == "1" ? 2 : 0), ExcelCellStyle.LeftBorderedStandard);
                                excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesCoordinatorrestorePoint.ToString());
                            }
                            else
                            {
                                excelReport.ReplaceRestorePoint(RestorePoint.SalesCoordinatorrestorePoint.ToString(), salesCoordinator.ToString(), 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth, totalRowModel, salesCoordinatorWidth, ExcelCellStyle.LeftBorderedStandard);
                                excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesCoordinatorrestorePoint.ToString());
                            }


                            totalRowSalsman = 0;
                            totalRowModel = 0;
                        }
                        else if (parameter[9].ToString() == "1" && rowIndex == 0)
                        {
                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesCoordinatorrestorePoint.ToString());
                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesmanRestorePoint.ToString());
                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.GradeRestorePoint.ToString());
                        }
                        else if (rowIndex == 0)
                        {
                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesCoordinatorrestorePoint.ToString());
                        }

                        excelReport.SetCellValue(row["MarketModel"].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth, ExcelCellStyle.LeftBorderedStandard);

                        marketModel = row["MarketModel"].ToString();
                        modelCatagory = row["ModelCatagory"].ToString();
                        salesMan = row["SalesmanName"].ToString();
                        grade = row["Grade"].ToString();
                    }
                    #endregion
                    #region Parameter9 = "1"
                    else if (parameter[9] == "1")
                    {
                        totalColor = ExcelCellStyle.BrownTotalNumber;
                        totalRedColor = ExcelCellStyle.BrownTotalRedNumber;
                        if (row["Seq"].ToString() == "1")
                        {
                            if (row["Seq2"].ToString() == "2")
                            {
                                totalColor = ExcelCellStyle.YellowTotalNumber;
                                totalRedColor = ExcelCellStyle.YellowTotalRedNumber;
                                excelReport.SetCellValue("Total SC", 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth, 1, salesManWidth + gradeWidth, totalColor);
                            }
                            else if (row["Seq2"].ToString() == "3")
                            {
                                excelReport.SetCellValue("AVG SC", 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth, 1, salesManWidth + gradeWidth, totalColor);
                            }
                            else
                            {
                                totalColor = ExcelCellStyle.RightBorderedStandardNumber;
                                totalRedColor = ExcelCellStyle.RightBorderedStandardRedNumber;
                                if (rowIndex == 0)
                                    excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesCoordinatorrestorePoint.ToString());
                                excelReport.SetCellValue(row["SalesmanName"].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth, 1, salesManWidth, ExcelCellStyle.LeftBorderedStandard);
                                excelReport.SetCellValue(row["Grade"].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth, 1, gradeWidth, ExcelCellStyle.LeftBorderedStandard);

                            }
                        }
                        else if (row["Seq"].ToString() == "2" && row["Seq2"].ToString() == "2")
                            excelReport.SetCellValue("AVG SH", 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth, 1, salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.BrownTotal);
                        else if (row["Seq"].ToString() == "3" && row["Seq2"].ToString() == "3")
                            excelReport.SetCellValue("AVG", 8 + rowIndex, 0 + dealerWidth + outletWidth, 1, salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.BrownTotal);
                        else if (row["Seq"].ToString() == "4" && row["Seq2"].ToString() == "3")
                            excelReport.SetCellValue("AVG", 8 + rowIndex, 0 + dealerWidth, 1, outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.BrownTotal);
                    }
                    #endregion
                    #region Parameter  = NUll
                    else
                    {
                        totalColor = ExcelCellStyle.BrownTotalNumber;
                        totalRedColor = ExcelCellStyle.BrownTotalRedNumber;
                        if (row["Seq"].ToString() == "2" && row["Seq2"].ToString() == "2")
                            excelReport.SetCellValue("AVG SH", 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth, 1, salesCoordinatorWidth, ExcelCellStyle.BrownTotal);
                        else if (row["Seq"].ToString() == "3" && row["Seq2"].ToString() == "3")
                            excelReport.SetCellValue("AVG", 8 + rowIndex, 0 + dealerWidth + outletWidth, 1, salesHeadWidth + salesCoordinatorWidth + salesManWidth, ExcelCellStyle.BrownTotal);
                        else if (row["Seq"].ToString() == "4" && row["Seq2"].ToString() == "3")
                        {
                            excelReport.SetCellValue("AVG", 8 + rowIndex, 0 + dealerWidth, 1, outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth, ExcelCellStyle.BrownTotal);
                            totalRowSalesHead = 0;
                        }
                        else
                        {
                            totalColor = ExcelCellStyle.RightBorderedStandardNumber;
                            totalRedColor = ExcelCellStyle.RightBorderedStandardRedNumber;
                            excelReport.SetCellValue(row["SalesCoordinatorName"].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth, 1, salesCoordinatorWidth, ExcelCellStyle.LeftBorderedStandardWrap);
                        }
                    }
                    #endregion
                    #endregion
                }
                else if (year == row["Year"].ToString()
                    && seq == row["Seq"].ToString()
                    && area == row["Area"].ToString()
                    && dealer == row["CompanyName"].ToString()
                    && outlet == row["BranchName"].ToString()
                    && salesHead == row["SalesHeadName"].ToString()
                    && salesCoordinator != row["SalesCoordinatorName"].ToString())
                {
                    #region Group
                    #region Parameter11 = "1"
                    if (parameter[11] == "1")
                    {
                        excelReport.ReplaceRestorePoint(RestorePoint.MarketModelRestorePoint.ToString(), marketModel.ToString(), 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, totalRowColor, modelWidth, ExcelCellStyle.LeftBorderedStandard);

                        decimal average = total / salesmanMonth;
                        //Masukin total Model Catagory
                        excelReport.SetCellValue("Total " + modelCatagory.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth + colorWidth, ExcelCellStyle.BlueTotal);
                        excelReport.SetCellValue(jan.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (jan < Convert.ToInt32(average) && jan != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(feb.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (feb < Convert.ToInt32(average) && feb != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(mar.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (mar < Convert.ToInt32(average) && mar != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(apr.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (apr < Convert.ToInt32(average) && apr != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(may.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (may < Convert.ToInt32(average) && may != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(jun.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (jun < Convert.ToInt32(average) && jun != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(jul.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (jul < Convert.ToInt32(average) && jul != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(aug.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (aug < Convert.ToInt32(average) && aug != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(sep.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sep < Convert.ToInt32(average) && sep != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(oct.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (oct < Convert.ToInt32(average) && oct != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(nov.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (nov < Convert.ToInt32(average) && nov != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(dec.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (dec < Convert.ToInt32(average) && dec != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(total.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);

                        totalSubTotal++;
                        totalRowBranchHead++;
                        totalRowColor++;
                        totalRowDealer++;
                        totalRowModel++;
                        totalRowSalesCo++;//<-- Untuk masukkin total nilai sub total tiap Model Catagory
                        totalRowSalesHead++;
                        totalRowSalsman++;
                        rowIndex++;

                        average = total / salesmanMonth;
                        //Masukkin sub total model category
                        excelReport.SetCellValue("SubTotal", 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth + colorWidth, ExcelCellStyle.PurpleTotal);
                        excelReport.SetCellValue(janGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (janGT < Convert.ToInt32(average) && janGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(febGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (febGT < Convert.ToInt32(average) && febGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(marGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (marGT < Convert.ToInt32(average) && marGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(aprGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (aprGT < Convert.ToInt32(average) && aprGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(mayGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (mayGT < Convert.ToInt32(average) && mayGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(junGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (junGT < Convert.ToInt32(average) && junGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(julGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (julGT < Convert.ToInt32(average) && julGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(augGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (augGT < Convert.ToInt32(average) && augGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(sepGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sepGT < Convert.ToInt32(average) && sepGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(octGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (octGT < Convert.ToInt32(average) && octGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(novGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (novGT < Convert.ToInt32(average) && novGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(decGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (decGT < Convert.ToInt32(average) && decGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(totalGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);

                        totalSubTotal++;
                        totalRowBranchHead++;
                        totalRowColor++;
                        totalRowDealer++;
                        totalRowModel++;
                        totalRowSalesCo++;//<-- Untuk masukkin total nilai sub total tiap Model Catagory
                        totalRowSalesHead++;
                        totalRowSalsman++;
                        rowIndex++;

                        jan = feb = mar = apr = may = jun = jul = aug = sep = oct = nov = dec = total = 0;
                        janGT = febGT = marGT = aprGT = mayGT = junGT = julGT = augGT = sepGT = octGT = novGT = decGT = totalGT = 0;

                        if (parameter[9].ToString() == "1")
                        {
                            excelReport.ReplaceRestorePoint(RestorePoint.SalesmanRestorePoint.ToString(), "AVG", 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth, totalRowSalsman, 2, ExcelCellStyle.LeftBorderedStandardWrap);
                            excelReport.RemoveRestorePoint(RestorePoint.GradeRestorePoint.ToString());
                            excelReport.ReplaceRestorePoint(RestorePoint.SalesCoordinatorrestorePoint.ToString(), salesCoordinator.ToString(), 0 + dealerWidth + outletWidth + salesHeadWidth, totalRowSalesCo, salesCoordinatorWidth, ExcelCellStyle.LeftBorderedStandardWrap);
                        }
                        else
                        {
                            excelReport.ReplaceRestorePoint(RestorePoint.SalesCoordinatorrestorePoint.ToString(), salesCoordinator.ToString(), 0 + dealerWidth + outletWidth + salesHeadWidth, totalRowSalesCo, salesCoordinatorWidth, ExcelCellStyle.LeftBorderedStandardWrap);
                        }

                        if (parameter[9].ToString() == "1")
                        {
                            //Tambahkan 1 Row Index untuk spasi pergantian sales coordinator
                            rowIndex++;
                            totalRowSalesHead++;
                            totalRowDealer++;
                            totalRowBranchHead++;
                            excelReport.CreateBlankRow(8 + rowIndex);
                        }

                        excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesCoordinatorrestorePoint.ToString());
                        if (parameter[9].ToString() == "1")
                        {
                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesmanRestorePoint.ToString());
                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.GradeRestorePoint.ToString());
                            salesMan = row["SalesmanName"].ToString();
                            grade = row["Grade"].ToString();
                        }
                        excelReport.RestorePoint(8 + rowIndex, RestorePoint.MarketModelRestorePoint.ToString());
                        excelReport.SetCellValue(row["ColourName"].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + modelWidth, 1, colorWidth, ExcelCellStyle.LeftBorderedStandard);

                        marketModel = row["MarketModel"].ToString();
                        modelCatagory = row["ModelCatagory"].ToString();
                        salesCoordinator = row["SalesCoordinatorName"].ToString();
                        totalRowColor = 0;
                        totalRowSalsman = 0;
                        totalRowSalesCo = 0;
                    }
                    #endregion
                    #region Parameter10 = "1"
                    else if (parameter[10] == "1")
                    {
                        decimal average = total / salesmanMonth;

                        //Masukin total Model Catagory
                        excelReport.SetCellValue("Total " + modelCatagory.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth + colorWidth, ExcelCellStyle.BlueTotal);
                        excelReport.SetCellValue(jan.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (jan < Convert.ToInt32(average) && jan != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(feb.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (feb < Convert.ToInt32(average) && feb != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(mar.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (mar < Convert.ToInt32(average) && mar != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(apr.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (apr < Convert.ToInt32(average) && apr != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(may.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (may < Convert.ToInt32(average) && may != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(jun.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (jun < Convert.ToInt32(average) && jun != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(jul.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (jul < Convert.ToInt32(average) && jul != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(aug.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (aug < Convert.ToInt32(average) && aug != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(sep.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sep < Convert.ToInt32(average) && sep != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(oct.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (oct < Convert.ToInt32(average) && oct != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(nov.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (nov < Convert.ToInt32(average) && nov != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(dec.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (dec < Convert.ToInt32(average) && dec != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(total.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);

                        totalSubTotal++;
                        totalRowBranchHead++;
                        totalRowColor++;
                        totalRowDealer++;
                        totalRowModel++;
                        totalRowSalesCo++;//<-- Untuk masukkin total nilai sub total tiap Model Catagory
                        totalRowSalesHead++;
                        totalRowSalsman++;
                        rowIndex++;

                        average = totalGT / salesmanMonth;
                        //Masukkin sub total model category
                        excelReport.SetCellValue("SubTotal", 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth + colorWidth, ExcelCellStyle.PurpleTotal);
                        excelReport.SetCellValue(janGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (janGT < Convert.ToInt32(average) && janGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(febGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (febGT < Convert.ToInt32(average) && febGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(marGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (marGT < Convert.ToInt32(average) && marGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(aprGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (aprGT < Convert.ToInt32(average) && aprGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(mayGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (mayGT < Convert.ToInt32(average) && mayGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(junGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (junGT < Convert.ToInt32(average) && junGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(julGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (julGT < Convert.ToInt32(average) && julGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(augGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (augGT < Convert.ToInt32(average) && augGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(sepGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sepGT < Convert.ToInt32(average) && sepGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(octGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (octGT < Convert.ToInt32(average) && octGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(novGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (novGT < Convert.ToInt32(average) && novGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(decGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (decGT < Convert.ToInt32(average) && decGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(totalGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);

                        totalSubTotal++;
                        totalRowBranchHead++;
                        totalRowColor++;
                        totalRowDealer++;
                        totalRowModel++;
                        totalRowSalesCo++;//<-- Untuk masukkin total nilai sub total tiap Model Catagory
                        totalRowSalesHead++;
                        totalRowSalsman++;
                        rowIndex++;

                        jan = feb = mar = apr = may = jun = jul = aug = sep = oct = nov = dec = total = 0;
                        janGT = febGT = marGT = aprGT = mayGT = junGT = julGT = augGT = sepGT = octGT = novGT = decGT = totalGT = 0;

                        excelReport.ReplaceRestorePoint(RestorePoint.SalesmanRestorePoint.ToString(), "AVG", 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth, totalRowSalsman, 2, ExcelCellStyle.LeftBorderedStandardWrap);
                        excelReport.RemoveRestorePoint(RestorePoint.GradeRestorePoint.ToString());
                        excelReport.ReplaceRestorePoint(RestorePoint.SalesCoordinatorrestorePoint.ToString(), salesCoordinator.ToString(), 0 + dealerWidth + outletWidth + salesHeadWidth, totalRowSalesCo, salesCoordinatorWidth, ExcelCellStyle.LeftBorderedStandardWrap);

                        //Tambahkan 1 Row Index untuk spasi pergantian sales coordinator
                        if (parameter[9].ToString() == "1")
                        {
                            rowIndex++;
                            totalRowSalesHead++;
                            totalRowDealer++;
                            totalRowBranchHead++;
                            excelReport.CreateBlankRow(8 + rowIndex);
                        }

                        excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesCoordinatorrestorePoint.ToString());
                        if (parameter[9].ToString() == "1")
                        {
                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesmanRestorePoint.ToString());
                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.GradeRestorePoint.ToString());

                        }
                        excelReport.SetCellValue(row["MarketModel"].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth, ExcelCellStyle.LeftBorderedStandard);

                        seq = row["Seq"].ToString();
                        seq2 = row["Seq2"].ToString();
                        salesMan = row["SalesmanName"].ToString();
                        grade = row["Grade"].ToString();
                        marketModel = row["MarketModel"].ToString();
                        modelCatagory = row["ModelCatagory"].ToString();
                        salesCoordinator = row["SalesCoordinatorName"].ToString();
                        totalRowColor = 0;
                        totalRowSalsman = 0;
                        totalRowSalesCo = 0;
                        totalRowModel = 0;
                    }
                    #endregion
                    #region Parameter9 = "1"
                    else if (parameter[9] == "1")
                    {
                        totalColor = ExcelCellStyle.RightBorderedStandardNumber;
                        totalRedColor = ExcelCellStyle.RightBorderedStandardRedNumber;

                        excelReport.ReplaceRestorePoint(RestorePoint.SalesCoordinatorrestorePoint.ToString(), salesCoordinator.ToString(), 0 + dealerWidth + outletWidth + salesHeadWidth, totalRowSalesCo, salesCoordinatorWidth, ExcelCellStyle.LeftBorderedStandardWrap);
                        excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesCoordinatorrestorePoint.ToString());
                        salesCoordinator = row["SalesCoordinatorName"].ToString();

                        totalRowSalesCo = 0;

                        excelReport.SetCellValue(row["SalesmanName"].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth, 1, salesManWidth, ExcelCellStyle.LeftBorderedStandard);
                        excelReport.SetCellValue(row["Grade"].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth, 1, gradeWidth, ExcelCellStyle.LeftBorderedStandard);

                        totalRowColor = 0;
                        totalRowSalsman = 0;
                        totalRowModel = 0;
                    }
                    #endregion
                    #region Parameter = Null
                    else
                    {
                        excelReport.SetCellValue(row["SalesCoordinatorName"].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth, 1, salesCoordinatorWidth, ExcelCellStyle.LeftBorderedStandardWrap);
                    }
                    #endregion
                    salesCoordinator = row["SalesCoordinatorName"].ToString();

                    #endregion

                    if (row["Seq"].ToString() == "1" && row["Seq2"].ToString() == "1")
                        totalSalesCo++;
                }
                else if (year == row["Year"].ToString()
               && seq != row["Seq"].ToString()
               && area == row["Area"].ToString()
               && dealer == row["CompanyName"].ToString()
               && outlet == row["BranchName"].ToString()
               && salesHead == row["SalesHeadName"].ToString()
               && salesCoordinator != row["SalesCoordinatorName"].ToString())
                {
                    #region Group
                    #region Parameter11 = "1"
                    if (parameter[11] == "1")
                    {
                        excelReport.ReplaceRestorePoint(RestorePoint.MarketModelRestorePoint.ToString(), marketModel.ToString(), 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, totalRowColor, modelWidth, ExcelCellStyle.LeftBorderedStandard);
                        decimal average = total / salesmanMonth;

                        //Masukin total Model Catagory
                        excelReport.SetCellValue("Total " + modelCatagory.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth + colorWidth, ExcelCellStyle.BlueTotal);
                        excelReport.SetCellValue(jan.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (jan < Convert.ToInt32(average) && jan != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(feb.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (feb < Convert.ToInt32(average) && feb != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(mar.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (mar < Convert.ToInt32(average) && mar != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(apr.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (apr < Convert.ToInt32(average) && apr != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(may.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (may < Convert.ToInt32(average) && may != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(jun.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (jun < Convert.ToInt32(average) && jun != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(jul.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (jul < Convert.ToInt32(average) && jul != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(aug.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (aug < Convert.ToInt32(average) && aug != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(sep.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sep < Convert.ToInt32(average) && sep != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(oct.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (oct < Convert.ToInt32(average) && oct != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(nov.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (nov < Convert.ToInt32(average) && nov != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(dec.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (dec < Convert.ToInt32(average) && dec != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(total.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);

                        totalSubTotal++;
                        totalRowBranchHead++;
                        totalRowColor++;
                        totalRowDealer++;
                        totalRowModel++;
                        totalRowSalesCo++;//<-- Untuk masukkin total nilai sub total tiap Model Catagory
                        totalRowSalesHead++;
                        totalRowSalsman++;
                        rowIndex++;

                        average = totalGT / salesmanMonth;
                        //Masukkin sub total model category
                        excelReport.SetCellValue("SubTotal", 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth + colorWidth, ExcelCellStyle.PurpleTotal);
                        excelReport.SetCellValue(janGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (janGT < Convert.ToInt32(average) && janGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(febGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (febGT < Convert.ToInt32(average) && febGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(marGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (marGT < Convert.ToInt32(average) && marGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(aprGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (aprGT < Convert.ToInt32(average) && aprGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(mayGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (mayGT < Convert.ToInt32(average) && mayGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(junGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (junGT < Convert.ToInt32(average) && junGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(julGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (julGT < Convert.ToInt32(average) && julGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(augGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (augGT < Convert.ToInt32(average) && augGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(sepGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sepGT < Convert.ToInt32(average) && sepGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(octGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (octGT < Convert.ToInt32(average) && octGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(novGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (novGT < Convert.ToInt32(average) && novGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(decGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (decGT < Convert.ToInt32(average) && decGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(totalGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);

                        totalSubTotal++;
                        totalRowBranchHead++;
                        totalRowColor++;
                        totalRowDealer++;
                        totalRowModel++;
                        totalRowSalesCo++;//<-- Untuk masukkin total nilai sub total tiap Model Catagory
                        totalRowSalesHead++;
                        totalRowSalsman++;
                        rowIndex++;

                        jan = feb = mar = apr = may = jun = jul = aug = sep = oct = nov = dec = total = 0;
                        janGT = febGT = marGT = aprGT = mayGT = junGT = julGT = augGT = sepGT = octGT = novGT = decGT = totalGT = 0;

                        if (parameter[9] == "1")
                        {
                            excelReport.ReplaceRestorePoint(RestorePoint.SalesmanRestorePoint.ToString(), "AVG", 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth, totalRowSalsman, 2, ExcelCellStyle.LeftBorderedStandardWrap);
                            excelReport.RemoveRestorePoint(RestorePoint.GradeRestorePoint.ToString());
                        }
                        excelReport.ReplaceRestorePoint(RestorePoint.SalesCoordinatorrestorePoint.ToString(), salesCoordinator.ToString(), 0 + dealerWidth + outletWidth + salesHeadWidth, totalRowSalesCo, salesCoordinatorWidth, ExcelCellStyle.LeftBorderedStandardWrap);

                        if (parameter[9].ToString() == "1")
                        {
                            //Tambahkan 1 Row Index untuk spasi pergantian sales coordinator
                            rowIndex++;
                            totalRowSalesHead++;
                            totalRowDealer++;
                            totalRowBranchHead++;
                            excelReport.CreateBlankRow(8 + rowIndex);

                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesmanRestorePoint.ToString());
                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.GradeRestorePoint.ToString());
                        }
                        else
                        {
                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesCoordinatorrestorePoint.ToString());
                        }

                        salesMan = row["SalesmanName"].ToString();
                        grade = row["Grade"].ToString();

                        excelReport.RestorePoint(8 + rowIndex, RestorePoint.MarketModelRestorePoint.ToString());
                        excelReport.SetCellValue(row["ColourName"].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + modelWidth, 1, colorWidth, ExcelCellStyle.LeftBorderedStandard);

                        seq = row["Seq"].ToString();
                        seq2 = row["Seq2"].ToString();

                        marketModel = row["MarketModel"].ToString();
                        modelCatagory = row["ModelCatagory"].ToString();
                        salesCoordinator = row["SalesCoordinatorName"].ToString();
                        totalRowColor = 0;
                        totalRowSalsman = 0;
                        totalRowSalesCo = 0;
                    }
                    #endregion
                    #region Parameter10 = "1"
                    else if (parameter[10] == "1")
                    {
                        decimal average = total / salesmanMonth;

                        //Masukin total Model Catagory
                        excelReport.SetCellValue("Total " + modelCatagory.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth + colorWidth, ExcelCellStyle.BlueTotal);
                        excelReport.SetCellValue(jan.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(feb.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(mar.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(apr.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(may.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(jun.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(jul.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(aug.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(sep.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(oct.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(nov.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(dec.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(total.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);

                        totalSubTotal++;
                        totalRowBranchHead++;
                        totalRowColor++;
                        totalRowDealer++;
                        totalRowModel++;
                        totalRowSalesCo++;//<-- Untuk masukkin total nilai sub total tiap Model Catagory
                        totalRowSalesHead++;
                        totalRowSalsman++;
                        rowIndex++;

                        average = totalGT / salesmanMonth;
                        //Masukkin sub total model category
                        excelReport.SetCellValue("SubTotal", 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth + colorWidth, ExcelCellStyle.PurpleTotal);
                        excelReport.SetCellValue(janGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(febGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(marGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(aprGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(mayGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(junGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(julGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(augGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(sepGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(octGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(novGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(decGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(totalGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);

                        totalSubTotal++;
                        totalRowBranchHead++;
                        totalRowColor++;
                        totalRowDealer++;
                        totalRowModel++;
                        totalRowSalesCo++;//<-- Untuk masukkin total nilai sub total tiap Model Catagory
                        totalRowSalesHead++;
                        totalRowSalsman++;
                        rowIndex++;

                        jan = feb = mar = apr = may = jun = jul = aug = sep = oct = nov = dec = total = 0;
                        janGT = febGT = marGT = aprGT = mayGT = junGT = julGT = augGT = sepGT = octGT = novGT = decGT = totalGT = 0;

                        excelReport.ReplaceRestorePoint(RestorePoint.SalesmanRestorePoint.ToString(), "AVG", 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth, totalRowSalsman, 2, ExcelCellStyle.LeftBorderedStandardWrap);
                        excelReport.RemoveRestorePoint(RestorePoint.GradeRestorePoint.ToString());
                        excelReport.ReplaceRestorePoint(RestorePoint.SalesCoordinatorrestorePoint.ToString(), salesCoordinator.ToString(), 0 + dealerWidth + outletWidth + salesHeadWidth, totalRowSalesCo, salesCoordinatorWidth, ExcelCellStyle.LeftBorderedStandardWrap);

                        if (parameter[9].ToString() == "1")
                        {
                            //Tambahkan 1 Row Index untuk spasi pergantian sales coordinator
                            rowIndex++;
                            totalRowSalesHead++;
                            totalRowDealer++;
                            totalRowBranchHead++;
                            excelReport.CreateBlankRow(8 + rowIndex);
                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesmanRestorePoint.ToString());
                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.GradeRestorePoint.ToString());

                        }
                        else
                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesCoordinatorrestorePoint.ToString());

                        excelReport.SetCellValue(row["MarketModel"].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth, ExcelCellStyle.LeftBorderedStandard);

                        seq = row["Seq"].ToString();
                        seq2 = row["Seq2"].ToString();
                        salesMan = row["SalesmanName"].ToString();
                        grade = row["Grade"].ToString();
                        marketModel = row["MarketModel"].ToString();
                        modelCatagory = row["ModelCatagory"].ToString();
                        salesCoordinator = row["SalesCoordinatorName"].ToString();
                        totalRowColor = 0;
                        totalRowSalsman = 0;
                        totalRowSalesCo = 0;
                        totalRowModel = 0;
                    }
                    #endregion
                    #region Parameter9 = "1"
                    else if (parameter[9] == "1")
                    {
                        if (row["Seq"].ToString() == "2" && row["Seq2"].ToString() == "1")
                        {
                            excelReport.ReplaceRestorePoint(RestorePoint.SalesCoordinatorrestorePoint.ToString(), salesCoordinator.ToString(), 0 + dealerWidth + outletWidth + salesHeadWidth, totalRowSalesCo, salesCoordinatorWidth, ExcelCellStyle.LeftBorderedStandardWrap);
                            totalColor = ExcelCellStyle.YellowTotalNumber;
                            totalRedColor = ExcelCellStyle.YellowTotalRedNumber;
                            excelReport.SetCellValue("Total SH", 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth, 1, salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.YellowTotal);
                        }
                        salesCoordinator = row["SalesCoordinatorName"].ToString();
                        seq = row["Seq"].ToString();
                        totalRowSalesCo = 0;
                    }
                    #endregion
                    #region Parameter = Null
                    else
                    {
                        if (row["Seq"].ToString() == "2" && row["Seq2"].ToString() == "1")
                        {
                            totalColor = ExcelCellStyle.YellowTotalNumber;
                            totalRedColor = ExcelCellStyle.YellowTotalRedNumber;
                            excelReport.SetCellValue("Total", 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth, 1, salesCoordinatorWidth + salesManWidth, ExcelCellStyle.YellowTotal);
                        }
                        salesCoordinator = row["SalesCoordinatorName"].ToString();
                        seq = row["Seq"].ToString();
                        totalRowSalesCo = 0;
                    }
                    #endregion
                    #endregion

                    if (row["Seq"].ToString() == "1" && row["Seq2"].ToString() == "1")
                        totalSalesCo++;
                }
                else if (year == row["Year"].ToString()
              && year == row["Year"].ToString()
              && seq == row["Seq"].ToString()
              && area == row["Area"].ToString()
              && dealer == row["CompanyName"].ToString()
              && outlet == row["BranchName"].ToString()
              && salesHead != row["SalesHeadName"].ToString())
                {
                    #region Group
                    #region Parameter 11
                    if (parameter[11] == "1")
                    {
                        excelReport.ReplaceRestorePoint(RestorePoint.MarketModelRestorePoint.ToString(), marketModel.ToString(), 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, totalRowColor, modelWidth, ExcelCellStyle.LeftBorderedStandard);
                        decimal average = total / salesmanMonth;

                        //Masukin total Model Catagory
                        excelReport.SetCellValue("Total " + modelCatagory.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth + colorWidth, ExcelCellStyle.BlueTotal);
                        excelReport.SetCellValue(jan.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (jan < Convert.ToInt32(average) && jan != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(feb.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (feb < Convert.ToInt32(average) && feb != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(mar.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (mar < Convert.ToInt32(average) && mar != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(apr.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (apr < Convert.ToInt32(average) && apr != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(may.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (may < Convert.ToInt32(average) && may != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(jun.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (jun < Convert.ToInt32(average) && jun != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(jul.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (jul < Convert.ToInt32(average) && jul != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(aug.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (aug < Convert.ToInt32(average) && aug != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(sep.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sep < Convert.ToInt32(average) && sep != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(oct.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (oct < Convert.ToInt32(average) && oct != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(nov.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (nov < Convert.ToInt32(average) && nov != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(dec.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (dec < Convert.ToInt32(average) && dec != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(total.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);

                        totalSubTotal++;
                        totalRowBranchHead++;
                        totalRowColor++;
                        totalRowDealer++;
                        totalRowModel++;
                        totalRowSalesCo++;//<-- Untuk masukkin total nilai sub total tiap Model Catagory
                        totalRowSalesHead++;
                        totalRowSalsman++;
                        rowIndex++;

                        average = totalGT / salesmanMonth;
                        //Masukkin sub total model category
                        excelReport.SetCellValue("SubTotal", 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth + colorWidth, ExcelCellStyle.PurpleTotal);
                        excelReport.SetCellValue(janGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (janGT < Convert.ToInt32(average) && janGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(febGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (febGT < Convert.ToInt32(average) && febGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(marGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (marGT < Convert.ToInt32(average) && marGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(aprGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (aprGT < Convert.ToInt32(average) && aprGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(mayGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (mayGT < Convert.ToInt32(average) && mayGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(junGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (junGT < Convert.ToInt32(average) && junGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(julGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (julGT < Convert.ToInt32(average) && julGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(augGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (augGT < Convert.ToInt32(average) && augGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(sepGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sepGT < Convert.ToInt32(average) && sepGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(octGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (octGT < Convert.ToInt32(average) && octGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(novGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (novGT < Convert.ToInt32(average) && novGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(decGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (decGT < Convert.ToInt32(average) && decGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(totalGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);

                        totalSubTotal++;
                        totalRowBranchHead++;
                        totalRowColor++;
                        totalRowDealer++;
                        totalRowModel++;
                        totalRowSalesCo++;//<-- Untuk masukkin total nilai sub total tiap Model Catagory
                        totalRowSalesHead++;
                        totalRowSalsman++;
                        rowIndex++;

                        jan = feb = mar = apr = may = jun = jul = aug = sep = oct = nov = dec = total = 0;
                        janGT = febGT = marGT = aprGT = mayGT = junGT = julGT = augGT = sepGT = octGT = novGT = decGT = totalGT = 0;

                        if (parameter[9].ToString() == "1")
                        {
                            excelReport.ReplaceRestorePoint(RestorePoint.SalesmanRestorePoint.ToString(), "AVG", 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth, totalRowSalsman, 2, ExcelCellStyle.LeftBorderedStandardWrap);
                            excelReport.RemoveRestorePoint(RestorePoint.GradeRestorePoint.ToString());
                        }
                        else
                            excelReport.ReplaceRestorePoint(RestorePoint.SalesCoordinatorrestorePoint.ToString(), "AVG", 0 + dealerWidth + outletWidth + salesHeadWidth, totalRowSalsman, 1, ExcelCellStyle.LeftBorderedStandardWrap);

                        excelReport.ReplaceRestorePoint(RestorePoint.SalesHeadRestorePoint.ToString(), salesHead.ToString(), 0 + dealerWidth + outletWidth, totalRowSalesHead, salesHeadWidth, ExcelCellStyle.LeftBorderedStandardWrap);

                        //Tambahkan 1 Row Index untuk spasi pergantian sales coordinator / Head / Dealer
                        rowIndex++;
                        totalRowSalesHead++;
                        totalRowDealer++;
                        totalRowBranchHead++;
                        excelReport.CreateBlankRow(8 + rowIndex);

                        excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesHeadRestorePoint.ToString());
                        excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesCoordinatorrestorePoint.ToString());

                        if (parameter[9].ToString() == "9")
                        {
                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesmanRestorePoint.ToString());
                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.GradeRestorePoint.ToString());
                        }

                        excelReport.RestorePoint(8 + rowIndex, RestorePoint.MarketModelRestorePoint.ToString());
                        excelReport.SetCellValue(row["ColourName"].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + modelWidth, 1, colorWidth, ExcelCellStyle.LeftBorderedStandard);

                        seq = row["Seq"].ToString();
                        seq2 = row["Seq2"].ToString();

                        if (parameter[9].ToString() == "1")
                        {
                            salesMan = row["SalesmanName"].ToString();
                            grade = row["Grade"].ToString();
                        }

                        marketModel = row["MarketModel"].ToString();
                        modelCatagory = row["ModelCatagory"].ToString();
                        salesCoordinator = row["SalesCoordinatorName"].ToString();
                        salesHead = row["SalesHeadName"].ToString();

                        totalRowColor = 0;
                        totalRowSalsman = 0;
                        totalRowSalesCo = 0;
                    }
                    #endregion
                    #region Paramter 10
                    else if (parameter[10] == "1")
                    {
                        decimal average = total / salesmanMonth;
                        //Masukin total Model Catagory
                        excelReport.SetCellValue("Total " + modelCatagory.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth + colorWidth, ExcelCellStyle.BlueTotal);
                        excelReport.SetCellValue(jan.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(feb.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(mar.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(apr.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(may.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(jun.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(jul.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(aug.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(sep.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(oct.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(nov.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(dec.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(total.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);

                        totalSubTotal++;
                        totalRowBranchHead++;
                        totalRowColor++;
                        totalRowDealer++;
                        totalRowModel++;
                        totalRowSalesCo++;//<-- Untuk masukkin total nilai sub total tiap Model Catagory
                        totalRowSalesHead++;
                        totalRowSalsman++;
                        rowIndex++;

                        average = total / salesmanMonth;
                        //Masukkin sub total model category
                        excelReport.SetCellValue("SubTotal", 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth + colorWidth, ExcelCellStyle.PurpleTotal);
                        excelReport.SetCellValue(janGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(febGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(marGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(aprGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(mayGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(junGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(julGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(augGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(sepGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(octGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(novGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(decGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(totalGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);

                        totalSubTotal++;
                        totalRowBranchHead++;
                        totalRowColor++;
                        totalRowDealer++;
                        totalRowModel++;
                        totalRowSalesCo++;//<-- Untuk masukkin total nilai sub total tiap Model Catagory
                        totalRowSalesHead++;
                        totalRowSalsman++;
                        rowIndex++;

                        jan = feb = mar = apr = may = jun = jul = aug = sep = oct = nov = dec = total = 0;
                        janGT = febGT = marGT = aprGT = mayGT = junGT = julGT = augGT = sepGT = octGT = novGT = decGT = totalGT = 0;

                        excelReport.ReplaceRestorePoint(RestorePoint.SalesmanRestorePoint.ToString(), "AVG", 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth, totalRowSalsman, 2, ExcelCellStyle.LeftBorderedStandardWrap);
                        excelReport.RemoveRestorePoint(RestorePoint.GradeRestorePoint.ToString());
                        excelReport.ReplaceRestorePoint(RestorePoint.SalesHeadRestorePoint.ToString(), salesHead.ToString(), 0 + dealerWidth + outletWidth, totalRowSalesHead, salesHeadWidth, ExcelCellStyle.LeftBorderedStandardWrap);

                        //Tambahkan 1 Row Index untuk spasi pergantian sales coordinator
                        rowIndex++;
                        totalRowSalesHead++;
                        totalRowDealer++;
                        totalRowBranchHead++;
                        excelReport.CreateBlankRow(8 + rowIndex);

                        excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesHeadRestorePoint.ToString());
                        excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesCoordinatorrestorePoint.ToString());
                        excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesmanRestorePoint.ToString());
                        excelReport.RestorePoint(8 + rowIndex, RestorePoint.GradeRestorePoint.ToString());
                        excelReport.SetCellValue(row["MarketModel"].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth, ExcelCellStyle.LeftBorderedStandard);

                        seq = row["Seq"].ToString();
                        seq2 = row["Seq2"].ToString();
                        salesMan = row["SalesmanName"].ToString();
                        grade = row["Grade"].ToString();
                        marketModel = row["MarketModel"].ToString();
                        modelCatagory = row["ModelCatagory"].ToString();
                        salesCoordinator = row["SalesCoordinatorName"].ToString();
                        salesHead = row["SalesHeadName"].ToString();

                        totalRowColor = 0;
                        totalRowSalsman = 0;
                        totalRowSalesCo = 0;
                        totalRowModel = 0;
                        totalRowSalesHead = 0;
                    }
                    #endregion
                    #region Parameter 9
                    else
                    {
                        excelReport.ReplaceRestorePoint(RestorePoint.SalesCoordinatorrestorePoint.ToString(), salesCoordinator.ToString(), 0 + dealerWidth + outletWidth + salesHeadWidth, totalRowSalesCo, salesCoordinatorWidth, ExcelCellStyle.LeftBorderedStandardWrap);
                        excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesCoordinatorrestorePoint.ToString());
                        totalRowSalesCo = 0;
                        salesHead = row["SalesHeadName"].ToString();
                        salesCoordinator = row["SalesCoordinatorName"].ToString();
                        seq = row["Seq"].ToString();
                        seq2 = row["Seq2"].ToString();
                    }
                    #endregion
                    #endregion
                    if (row["Seq"].ToString() == "1" && row["Seq2"].ToString() == "1")
                    {
                        totalSalesCo = 1;
                        totalSalesHead++;
                    }
                }
                else if (year == row["Year"].ToString()
             && seq != row["Seq"].ToString()
             && area == row["Area"].ToString()
             && dealer == row["CompanyName"].ToString()
             && outlet == row["BranchName"].ToString()
             && salesHead != row["SalesHeadName"].ToString())
                {
                    #region Group
                    #region Parameter11 = "1"
                    if (parameter[11] == "1")
                    {
                        excelReport.ReplaceRestorePoint(RestorePoint.MarketModelRestorePoint.ToString(), marketModel.ToString(), 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, totalRowColor, modelWidth, ExcelCellStyle.LeftBorderedStandard);
                        decimal average = total / salesmanMonth;
                        //Masukin total Model Catagory
                        excelReport.SetCellValue("Total " + modelCatagory.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth + colorWidth, ExcelCellStyle.BlueTotal);
                        excelReport.SetCellValue(jan.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (jan < Convert.ToInt32(average) && jan != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(feb.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (feb < Convert.ToInt32(average) && feb != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(mar.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (mar < Convert.ToInt32(average) && mar != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(apr.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (apr < Convert.ToInt32(average) && apr != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(may.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (may < Convert.ToInt32(average) && may != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(jun.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (jun < Convert.ToInt32(average) && jun != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(jul.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (jul < Convert.ToInt32(average) && jul != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(aug.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (aug < Convert.ToInt32(average) && aug != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(sep.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sep < Convert.ToInt32(average) && sep != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(oct.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (oct < Convert.ToInt32(average) && oct != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(nov.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (nov < Convert.ToInt32(average) && nov != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(dec.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (dec < Convert.ToInt32(average) && dec != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(total.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);

                        totalSubTotal++;
                        totalRowBranchHead++;
                        totalRowColor++;
                        totalRowDealer++;
                        totalRowModel++;
                        totalRowSalesCo++;//<-- Untuk masukkin total nilai sub total tiap Model Catagory
                        totalRowSalesHead++;
                        totalRowSalsman++;
                        rowIndex++;

                        average = totalGT / salesmanMonth;
                        //Masukkin sub total model category
                        excelReport.SetCellValue("SubTotal", 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth + colorWidth, ExcelCellStyle.PurpleTotal);
                        excelReport.SetCellValue(janGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (janGT < Convert.ToInt32(average) && janGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(febGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (febGT < Convert.ToInt32(average) && febGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(marGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (marGT < Convert.ToInt32(average) && marGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(aprGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (aprGT < Convert.ToInt32(average) && aprGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(mayGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (mayGT < Convert.ToInt32(average) && mayGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(junGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (junGT < Convert.ToInt32(average) && junGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(julGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (julGT < Convert.ToInt32(average) && julGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(augGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (augGT < Convert.ToInt32(average) && augGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(sepGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sepGT < Convert.ToInt32(average) && sepGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(octGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (octGT < Convert.ToInt32(average) && octGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(novGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (novGT < Convert.ToInt32(average) && novGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(decGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (decGT < Convert.ToInt32(average) && decGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(totalGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);

                        totalSubTotal++;
                        totalRowBranchHead++;
                        totalRowColor++;
                        totalRowDealer++;
                        totalRowModel++;
                        totalRowSalesCo++;//<-- Untuk masukkin total nilai sub total tiap Model Catagory
                        totalRowSalesHead++;
                        totalRowSalsman++;
                        rowIndex++;

                        jan = feb = mar = apr = may = jun = jul = aug = sep = oct = nov = dec = total = 0;
                        janGT = febGT = marGT = aprGT = mayGT = junGT = julGT = augGT = sepGT = octGT = novGT = decGT = totalGT = 0;

                        if (parameter[9].ToString() == "1")
                        {
                            excelReport.ReplaceRestorePoint(RestorePoint.SalesmanRestorePoint.ToString(), "AVG", 0 + dealerWidth + outletWidth + salesHeadWidth, totalRowSalsman, salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.LeftBorderedStandardWrap);
                            excelReport.RemoveRestorePoint(RestorePoint.GradeRestorePoint.ToString());
                        }
                        else
                            excelReport.ReplaceRestorePoint(RestorePoint.SalesCoordinatorrestorePoint.ToString(), "AVG", 0 + dealerWidth + outletWidth + salesHeadWidth, totalRowSalesCo, salesCoordinatorWidth, ExcelCellStyle.LeftBorderedStandardWrap);

                        excelReport.ReplaceRestorePoint(RestorePoint.SalesHeadRestorePoint.ToString(), salesHead.ToString(), 0 + dealerWidth + outletWidth, totalRowSalesHead, salesHeadWidth, ExcelCellStyle.LeftBorderedStandardWrap);

                        //Tambahkan 1 Row Index untuk spasi pergantian sales coordinator / Head / Dealer
                        rowIndex++;
                        totalRowSalesHead++;
                        totalRowDealer++;
                        totalRowBranchHead++;
                        excelReport.CreateBlankRow(8 + rowIndex);

                        if (row["Seq"].ToString() != "3")
                        {
                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesHeadRestorePoint.ToString());
                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesCoordinatorrestorePoint.ToString());

                            if (parameter[9].ToString() == "1")
                            {
                                excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesmanRestorePoint.ToString());
                                excelReport.RestorePoint(8 + rowIndex, RestorePoint.GradeRestorePoint.ToString());
                            }

                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.MarketModelRestorePoint.ToString());
                        }
                        else
                        {
                            if (parameter[9].ToString() == "1")
                            {
                                excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesmanRestorePoint.ToString());
                                excelReport.RestorePoint(8 + rowIndex, RestorePoint.GradeRestorePoint.ToString());
                            }
                            else
                                excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesCoordinatorrestorePoint.ToString());
                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.MarketModelRestorePoint.ToString());
                        }
                        excelReport.SetCellValue(row["ColourName"].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + modelWidth, 1, colorWidth, ExcelCellStyle.LeftBorderedStandard);
                        seq = row["Seq"].ToString();
                        seq2 = row["Seq2"].ToString();

                        salesMan = row["SalesmanName"].ToString();
                        grade = row["Grade"].ToString();
                        marketModel = row["MarketModel"].ToString();
                        modelCatagory = row["ModelCatagory"].ToString();
                        salesCoordinator = row["SalesCoordinatorName"].ToString();
                        salesHead = row["SalesHeadName"].ToString();

                        totalRowColor = 0;
                        totalRowSalsman = 0;
                        totalRowSalesCo = 0;
                        totalRowSalesHead = 0;
                    }
                    #endregion
                    #region  Parameter10 = "1"
                    else if (parameter[10] == "1")
                    {
                        decimal average = total / salesmanMonth;
                        //Masukin total Model Catagory
                        excelReport.SetCellValue("Total " + modelCatagory.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth + colorWidth, ExcelCellStyle.BlueTotal);
                        excelReport.SetCellValue(jan.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(feb.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(mar.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(apr.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(may.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(jun.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(jul.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(aug.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(sep.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(oct.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(nov.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(dec.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(total.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);

                        totalSubTotal++;
                        totalRowBranchHead++;
                        totalRowColor++;
                        totalRowDealer++;
                        totalRowModel++;
                        totalRowSalesCo++;//<-- Untuk masukkin total nilai sub total tiap Model Catagory
                        totalRowSalesHead++;
                        totalRowSalsman++;
                        rowIndex++;

                        average = total / salesmanMonth;
                        //Masukkin sub total model category
                        excelReport.SetCellValue("SubTotal", 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth + colorWidth, ExcelCellStyle.PurpleTotal);
                        excelReport.SetCellValue(janGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(febGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(marGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(aprGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(mayGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(junGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(julGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(augGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(sepGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(octGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(novGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(decGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(totalGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);

                        totalSubTotal++;
                        totalRowBranchHead++;
                        totalRowColor++;
                        totalRowDealer++;
                        totalRowModel++;
                        totalRowSalesCo++;//<-- Untuk masukkin total nilai sub total tiap Model Catagory
                        totalRowSalesHead++;
                        totalRowSalsman++;
                        rowIndex++;

                        jan = feb = mar = apr = may = jun = jul = aug = sep = oct = nov = dec = total = 0;
                        janGT = febGT = marGT = aprGT = mayGT = junGT = julGT = augGT = sepGT = octGT = novGT = decGT = totalGT = 0;

                        if (parameter[9].ToString() == "1")
                        {
                            excelReport.ReplaceRestorePoint(RestorePoint.SalesmanRestorePoint.ToString(), "AVG", 0 + dealerWidth + outletWidth + salesHeadWidth, totalRowSalsman, salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.LeftBorderedStandardWrap);
                            excelReport.RemoveRestorePoint(RestorePoint.GradeRestorePoint.ToString());
                        }
                        else
                            excelReport.ReplaceRestorePoint(RestorePoint.SalesCoordinatorrestorePoint.ToString(), "AVG", 0 + dealerWidth + outletWidth + salesHeadWidth, totalRowSalsman, salesCoordinatorWidth, ExcelCellStyle.LeftBorderedStandardWrap);
                        excelReport.ReplaceRestorePoint(RestorePoint.SalesHeadRestorePoint.ToString(), salesHead.ToString(), 0 + dealerWidth + outletWidth, totalRowSalesHead, salesHeadWidth, ExcelCellStyle.LeftBorderedStandardWrap);

                        //Tambahkan 1 Row Index untuk spasi pergantian sales coordinator
                        rowIndex++;
                        totalRowSalesHead++;
                        totalRowDealer++;
                        totalRowBranchHead++;
                        excelReport.CreateBlankRow(8 + rowIndex);


                        if (row["Seq"].ToString() != "3")
                        {
                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesHeadRestorePoint.ToString());
                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesCoordinatorrestorePoint.ToString());

                            if (parameter[9].ToString() == "1")
                            {
                                excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesmanRestorePoint.ToString());
                                excelReport.RestorePoint(8 + rowIndex, RestorePoint.GradeRestorePoint.ToString());
                            }
                        }
                        else if (parameter[9].ToString() == "1")
                        {
                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesmanRestorePoint.ToString());
                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.GradeRestorePoint.ToString());
                        }
                        else
                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesCoordinatorrestorePoint.ToString());
                        excelReport.SetCellValue(row["MarketModel"].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth, ExcelCellStyle.LeftBorderedStandard);

                        seq = row["Seq"].ToString();
                        seq2 = row["Seq2"].ToString();
                        salesMan = row["SalesmanName"].ToString();
                        grade = row["Grade"].ToString();
                        marketModel = row["MarketModel"].ToString();
                        modelCatagory = row["ModelCatagory"].ToString();
                        salesCoordinator = row["SalesCoordinatorName"].ToString();
                        salesHead = row["SalesHeadName"].ToString();

                        totalRowColor = 0;
                        totalRowSalsman = 0;
                        totalRowSalesCo = 0;
                        totalRowModel = 0;
                        totalRowSalesHead = 0;
                    }
                    #endregion
                    #region Parameter9 = "1"
                    else if (parameter[9] == "1")
                    {
                        //excelReport.SetCellValue(salesHead.ToString(),8 + rowIndex - totalRowSalesHead, 0 + dealerWidth + outletWidth, totalRowSalesHead, salesHeadWidth, ExcelCellStyle.Standard);
                        excelReport.ReplaceRestorePoint(RestorePoint.SalesHeadRestorePoint.ToString(), salesHead.ToString(), 0 + dealerWidth + outletWidth, totalRowSalesHead, salesHeadWidth, ExcelCellStyle.LeftBorderedStandardWrap);

                        if (row["Seq"].ToString() == "3" && row["Seq2"].ToString() == "2")
                        {
                            totalColor = ExcelCellStyle.GreenTotalNumber;
                            totalRedColor = ExcelCellStyle.GreenTotalRedNumber;
                            excelReport.SetCellValue("Total", 8 + rowIndex, 0 + dealerWidth + outletWidth, 1, salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.GreenTotal);
                        }
                        else
                        {
                            totalColor = ExcelCellStyle.RightBorderedStandardNumber;
                            totalRedColor = ExcelCellStyle.RightBorderedStandardRedNumber;
                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesHeadRestorePoint.ToString());
                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesCoordinatorrestorePoint.ToString());

                            excelReport.SetCellValue(row["SalesmanName"].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth, 1, salesManWidth, ExcelCellStyle.LeftBorderedStandard);
                            excelReport.SetCellValue(row["Grade"].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth, 1, gradeWidth, ExcelCellStyle.LeftBorderedStandard);

                        }
                        salesCoordinator = row["SalesCoordinatorName"].ToString();
                        salesHead = row["SalesHeadName"].ToString();
                        seq = row["Seq"].ToString();
                        totalRowColor = 0;
                        totalRowSalsman = 0;
                        totalRowSalesCo = 0;
                        totalRowModel = 0;
                        totalRowSalesHead = 0;
                    }
                    #endregion
                    #region Parameter = Null
                    else
                    {
                        excelReport.ReplaceRestorePoint(RestorePoint.SalesHeadRestorePoint.ToString(), salesHead.ToString(), 0 + dealerWidth + outletWidth, totalRowSalesHead, salesHeadWidth, ExcelCellStyle.LeftBorderedTop);

                        if (row["Seq"].ToString() == "3" && row["Seq2"].ToString() == "2")
                        {
                            totalColor = ExcelCellStyle.GreenTotalNumber;
                            totalRedColor = ExcelCellStyle.GreenTotalRedNumber;
                            excelReport.SetCellValue("Total", 8 + rowIndex, 0 + dealerWidth + outletWidth, 1, salesHeadWidth + salesCoordinatorWidth + salesManWidth, ExcelCellStyle.GreenTotal);
                        }
                        else
                        {
                            totalColor = ExcelCellStyle.RightBorderedStandardNumber;
                            totalRedColor = ExcelCellStyle.RightBorderedStandardRedNumber;
                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesHeadRestorePoint.ToString());
                            excelReport.SetCellValue(row["SalesCoordinatorName"].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth, 1, salesCoordinatorWidth, ExcelCellStyle.LeftBorderedStandardWrap);
                        }

                        totalRowSalesHead = 0;

                        seq = row["Seq"].ToString();
                        salesHead = row["SalesHeadName"].ToString();
                        salesCoordinator = row["SalesCoordinatorName"].ToString();
                    }
                    #endregion
                    #endregion
                    if (row["Seq"].ToString() == "1" && row["Seq2"].ToString() == "1")
                    {
                        totalSalesCo = 1;
                        totalSalesHead++;
                    }
                }
                else if (year == row["Year"].ToString()
             && seq != row["Seq"].ToString()
             && dealer == row["CompanyName"].ToString()
             && outlet != row["BranchName"].ToString())
                {
                    #region Group
                    #region Parameter11 = "1"
                    if (parameter[11] == "1")
                    {
                        excelReport.ReplaceRestorePoint(RestorePoint.MarketModelRestorePoint.ToString(), marketModel.ToString(), 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, totalRowColor, modelWidth, ExcelCellStyle.LeftBorderedStandard);
                        decimal average = total / salesmanMonth;
                        //Masukin total Model Catagory
                        excelReport.SetCellValue("Total " + modelCatagory.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth + colorWidth, ExcelCellStyle.BlueTotal);
                        excelReport.SetCellValue(jan.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (jan < Convert.ToInt32(average) && jan != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(feb.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (feb < Convert.ToInt32(average) && feb != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(mar.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (mar < Convert.ToInt32(average) && mar != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(apr.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (apr < Convert.ToInt32(average) && apr != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(may.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (may < Convert.ToInt32(average) && may != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(jun.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (jun < Convert.ToInt32(average) && jun != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(jul.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (jul < Convert.ToInt32(average) && jul != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(aug.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (aug < Convert.ToInt32(average) && aug != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(sep.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sep < Convert.ToInt32(average) && sep != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(oct.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (oct < Convert.ToInt32(average) && oct != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(nov.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (nov < Convert.ToInt32(average) && nov != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(dec.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (dec < Convert.ToInt32(average) && dec != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(total.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);

                        totalSubTotal++;
                        totalRowBranchHead++;
                        totalRowColor++;
                        totalRowDealer++;
                        totalRowModel++;
                        totalRowSalesCo++;//<-- Untuk masukkin total nilai sub total tiap Model Catagory
                        totalRowSalesHead++;
                        totalRowSalsman++;
                        rowIndex++;

                        average = totalGT / salesmanMonth;
                        //Masukkin sub total model category
                        excelReport.SetCellValue("SubTotal", 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth + colorWidth, ExcelCellStyle.PurpleTotal);
                        excelReport.SetCellValue(janGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (janGT < Convert.ToInt32(average) && janGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(febGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (febGT < Convert.ToInt32(average) && febGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(marGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (marGT < Convert.ToInt32(average) && marGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(aprGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (aprGT < Convert.ToInt32(average) && aprGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(mayGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (mayGT < Convert.ToInt32(average) && mayGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(junGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (junGT < Convert.ToInt32(average) && junGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(julGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (julGT < Convert.ToInt32(average) && julGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(augGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (augGT < Convert.ToInt32(average) && augGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(sepGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sepGT < Convert.ToInt32(average) && sepGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(octGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (octGT < Convert.ToInt32(average) && octGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(novGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (novGT < Convert.ToInt32(average) && novGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(decGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (decGT < Convert.ToInt32(average) && decGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(totalGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);

                        totalSubTotal++;
                        totalRowBranchHead++;
                        totalRowColor++;
                        totalRowDealer++;
                        totalRowModel++;
                        totalRowSalesCo++;//<-- Untuk masukkin total nilai sub total tiap Model Catagory
                        totalRowSalesHead++;
                        totalRowSalsman++;
                        rowIndex++;

                        jan = feb = mar = apr = may = jun = jul = aug = sep = oct = nov = dec = total = 0;
                        janGT = febGT = marGT = aprGT = mayGT = junGT = julGT = augGT = sepGT = octGT = novGT = decGT = totalGT = 0;

                        if (parameter[9].ToString() == "1")
                        {
                            excelReport.ReplaceRestorePoint(RestorePoint.SalesmanRestorePoint.ToString(), "AVG", 0 + dealerWidth + outletWidth, totalRowSalsman, salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.LeftBorderedStandardWrap);
                            excelReport.RemoveRestorePoint(RestorePoint.GradeRestorePoint.ToString());
                        }
                        else
                            excelReport.ReplaceRestorePoint(RestorePoint.SalesCoordinatorrestorePoint.ToString(), "AVG", 0 + dealerWidth + outletWidth, totalRowSalsman, salesHeadWidth + salesCoordinatorWidth, ExcelCellStyle.LeftBorderedStandardWrap);
                        excelReport.ReplaceRestorePoint(RestorePoint.OutletRestorePoint.ToString(), outlet.ToString(), 0 + dealerWidth, totalRowBranchHead, outletWidth, ExcelCellStyle.LeftBorderedStandardWrap);

                        //Tambahkan 1 Row Index untuk spasi pergantian sales coordinator / Head / Dealer
                        rowIndex++;
                        totalRowSalesHead++;
                        totalRowDealer++;
                        totalRowBranchHead++;
                        excelReport.CreateBlankRow(8 + rowIndex);

                        if (row["Seq"].ToString() != "4" && row["Seq2"].ToString() != "2")
                        {
                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.OutletRestorePoint.ToString());
                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesHeadRestorePoint.ToString());
                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesCoordinatorrestorePoint.ToString());

                            if (parameter[9].ToString() == "1")
                            {
                                excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesmanRestorePoint.ToString());
                                excelReport.RestorePoint(8 + rowIndex, RestorePoint.GradeRestorePoint.ToString());
                            }
                        }
                        else
                        {
                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesmanRestorePoint.ToString());
                        }
                        excelReport.RestorePoint(8 + rowIndex, RestorePoint.MarketModelRestorePoint.ToString());
                        excelReport.SetCellValue(row["ColourName"].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + modelWidth, 1, colorWidth, ExcelCellStyle.LeftBorderedStandard);
                        seq = row["Seq"].ToString();
                        seq2 = row["Seq2"].ToString();
                        salesMan = row["SalesmanName"].ToString();
                        grade = row["Grade"].ToString();
                        marketModel = row["MarketModel"].ToString();
                        modelCatagory = row["ModelCatagory"].ToString();
                        salesCoordinator = row["SalesCoordinatorName"].ToString();
                        salesHead = row["SalesHeadName"].ToString();
                        outlet = row["BranchName"].ToString();

                        totalRowColor = 0;
                        totalRowSalsman = 0;
                        totalRowSalesCo = 0;
                        totalRowSalesHead = 0;
                        totalRowBranchHead = 0;
                    }
                    #endregion
                    #region Parameter10 = "1"
                    else if (parameter[10] == "1")
                    {
                        decimal average = total / salesmanMonth;

                        //Masukin total Model Catagory
                        excelReport.SetCellValue("Total " + modelCatagory.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth + colorWidth, ExcelCellStyle.BlueTotal);
                        excelReport.SetCellValue(jan.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(feb.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(mar.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(apr.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(may.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(jun.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(jul.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(aug.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(sep.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(oct.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(nov.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(dec.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(total.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);

                        totalSubTotal++;
                        totalRowBranchHead++;
                        totalRowColor++;
                        totalRowDealer++;
                        totalRowModel++;
                        totalRowSalesCo++;//<-- Untuk masukkin total nilai sub total tiap Model Catagory
                        totalRowSalesHead++;
                        totalRowSalsman++;
                        rowIndex++;

                        average = totalGT / salesmanMonth;
                        //Masukkin sub total model category
                        excelReport.SetCellValue("SubTotal", 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth + colorWidth, ExcelCellStyle.PurpleTotal);
                        excelReport.SetCellValue(janGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(febGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(marGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(aprGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(mayGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(junGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(julGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(augGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(sepGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(octGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(novGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(decGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(totalGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);

                        totalSubTotal++;
                        totalRowBranchHead++;
                        totalRowColor++;
                        totalRowDealer++;
                        totalRowModel++;
                        totalRowSalesCo++;//<-- Untuk masukkin total nilai sub total tiap Model Catagory
                        totalRowSalesHead++;
                        totalRowSalsman++;
                        rowIndex++;

                        jan = feb = mar = apr = may = jun = jul = aug = sep = oct = nov = dec = total = 0;
                        janGT = febGT = marGT = aprGT = mayGT = junGT = julGT = augGT = sepGT = octGT = novGT = decGT = totalGT = 0;

                        if (parameter[9].ToString() == "1")
                        {
                            excelReport.ReplaceRestorePoint(RestorePoint.SalesmanRestorePoint.ToString(), "AVG", 0 + dealerWidth + outletWidth, totalRowSalsman, salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.LeftBorderedStandardWrap);
                            excelReport.RemoveRestorePoint(RestorePoint.GradeRestorePoint.ToString());
                        }
                        else
                            excelReport.ReplaceRestorePoint(RestorePoint.SalesCoordinatorrestorePoint.ToString(), "AVG", 0 + dealerWidth + outletWidth, totalRowSalsman, salesHeadWidth + salesCoordinatorWidth, ExcelCellStyle.LeftBorderedStandardWrap);
                        excelReport.ReplaceRestorePoint(RestorePoint.OutletRestorePoint.ToString(), outlet.ToString(), 0 + dealerWidth, totalRowBranchHead, outletWidth, ExcelCellStyle.LeftBorderedStandardWrap);

                        //Tambahkan 1 Row Index untuk spasi pergantian sales coordinator
                        rowIndex++;
                        totalRowSalesHead++;
                        totalRowDealer++;
                        totalRowBranchHead++;
                        excelReport.CreateBlankRow(8 + rowIndex);
                        if (row["Seq"].ToString() != "4" && row["Seq2"].ToString() != "2")
                        {
                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.OutletRestorePoint.ToString());
                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesHeadRestorePoint.ToString());
                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesCoordinatorrestorePoint.ToString());
                        }
                        else
                        {
                            if (parameter[9] == "0" && parameter[10] == "1")
                                excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesCoordinatorrestorePoint.ToString());
                        }

                        if (parameter[9].ToString() == "1")
                        {
                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesmanRestorePoint.ToString());
                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.GradeRestorePoint.ToString());
                        }

                        excelReport.SetCellValue(row["MarketModel"].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth, ExcelCellStyle.LeftBorderedStandard);

                        seq = row["Seq"].ToString();
                        seq2 = row["Seq2"].ToString();
                        salesMan = row["SalesmanName"].ToString();
                        grade = row["Grade"].ToString();
                        marketModel = row["MarketModel"].ToString();
                        modelCatagory = row["ModelCatagory"].ToString();
                        salesCoordinator = row["SalesCoordinatorName"].ToString();
                        salesHead = row["SalesHeadName"].ToString();
                        dealer = row["CompanyName"].ToString();
                        outlet = row["BranchName"].ToString();

                        totalRowColor = 0;
                        totalRowSalsman = 0;
                        totalRowSalesCo = 0;
                        totalRowModel = 0;
                        totalRowSalesHead = 0;
                        totalRowBranchHead = 0;
                    }
                    #endregion
                    #region Parameter9 = "1"
                    else if (parameter[9] == "1")
                    {
                        excelReport.ReplaceRestorePoint(RestorePoint.OutletRestorePoint.ToString(), outlet.ToString(), 0 + dealerWidth, totalRowBranchHead, outletWidth, ExcelCellStyle.LeftBorderedStandardWrap);

                        //Tambahkan 1 Row Index untuk spasi pergantian sales coordinator
                        rowIndex++;
                        totalRowSalesHead++;
                        totalRowDealer++;
                        totalRowBranchHead++;
                        excelReport.CreateBlankRow(8 + rowIndex);

                        seq = row["Seq"].ToString();
                        outlet = row["BranchName"].ToString();
                        salesCoordinator = row["SalesCoordinatorName"].ToString();
                        salesHead = row["SalesHeadName"].ToString();

                        totalRowSalesCo = 0;
                        totalRowSalesHead = 0;
                        totalRowBranchHead = 0;

                        if (row["Seq"].ToString() == "4" && row["Seq2"].ToString() == "2")
                        {
                            totalColor = ExcelCellStyle.BlueTotalNumber;
                            totalRedColor = ExcelCellStyle.BlueTotalRedNumber;
                            excelReport.SetCellValue("Total", 8 + rowIndex, 0 + dealerWidth, 1, outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.BlueTotal);
                        }
                        else
                        {
                            totalColor = ExcelCellStyle.RightBorderedStandardNumber;
                            totalRedColor = ExcelCellStyle.RightBorderedStandardRedNumber;
                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.OutletRestorePoint.ToString());
                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesHeadRestorePoint.ToString());
                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesCoordinatorrestorePoint.ToString());

                            excelReport.SetCellValue(row["SalesmanName"].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth, 1, salesManWidth, ExcelCellStyle.LeftBorderedStandard);
                            excelReport.SetCellValue(row["Grade"].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth, 1, gradeWidth, ExcelCellStyle.LeftBorderedStandard);
                        }
                    }
                    #endregion
                    #region Parameter = Null
                    else
                    {
                        excelReport.ReplaceRestorePoint(RestorePoint.OutletRestorePoint.ToString(), outlet.ToString(), 0 + dealerWidth, totalRowBranchHead, outletWidth, ExcelCellStyle.LeftBorderedTop);
                        if (row["Seq"].ToString() == "1" && row["Seq2"].ToString() == "1")
                        {
                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.OutletRestorePoint.ToString());
                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesHeadRestorePoint.ToString());
                        }
                        //excelReport.SetCellValue(outlet.ToString(),8 + rowIndex - totalRowBranchHead, 0 + dealerWidth, totalRowBranchHead, outletWidth, ExcelCellStyle.Standard);
                        seq = row["Seq"].ToString();
                        outlet = row["BranchName"].ToString();
                        salesCoordinator = row["SalesCoordinatorName"].ToString();
                        salesHead = row["SalesHeadName"].ToString();
                        totalRowSalesCo = 0;
                        totalRowBranchHead = 0;
                        totalRowSalesHead = 0;

                        if (row["Seq"].ToString() == "4" && row["Seq2"].ToString() == "2")
                        {
                            totalColor = ExcelCellStyle.BlueTotalNumber;
                            totalRedColor = ExcelCellStyle.BlueTotalRedNumber;
                            excelReport.SetCellValue("Total", 8 + rowIndex, 0 + dealerWidth, 1, outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth, ExcelCellStyle.BlueTotal);
                        }
                        else
                        {
                            totalColor = ExcelCellStyle.RightBorderedStandardNumber;
                            totalRedColor = ExcelCellStyle.RightBorderedStandardRedNumber;
                            excelReport.SetCellValue(row["SalesCoordinatorName"].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth, 1, salesCoordinatorWidth, ExcelCellStyle.LeftBorderedStandard);
                        }
                    }
                    #endregion
                    #endregion
                    if (row["Seq"].ToString() == "1" && row["Seq2"].ToString() == "1")
                    {
                        totalSalesCo = 1;
                        totalSalesHead = 1;
                        totalOutlet++;
                    }
                }
                else if (year == row["Year"].ToString()
            && seq != row["Seq"].ToString()
            && area == row["Area"].ToString()
            && dealer != row["CompanyName"].ToString())
                {
                    #region Group
                    #region Parameter 11
                    if (parameter[11] == "1")
                    {
                        excelReport.ReplaceRestorePoint(RestorePoint.MarketModelRestorePoint.ToString(), marketModel.ToString(), 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, totalRowColor, modelWidth, ExcelCellStyle.LeftBorderedStandard);
                        decimal average = total / salesmanMonth;
                        //Masukin total Model Catagory
                        excelReport.SetCellValue("Total " + modelCatagory.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth + colorWidth, ExcelCellStyle.BlueTotal);
                        excelReport.SetCellValue(jan.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (jan < Convert.ToInt32(average) && jan != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(feb.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (feb < Convert.ToInt32(average) && feb != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(mar.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (mar < Convert.ToInt32(average) && mar != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(apr.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (apr < Convert.ToInt32(average) && apr != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(may.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (may < Convert.ToInt32(average) && may != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(jun.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (jun < Convert.ToInt32(average) && jun != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(jul.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (jul < Convert.ToInt32(average) && jul != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(aug.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (aug < Convert.ToInt32(average) && aug != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(sep.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sep < Convert.ToInt32(average) && sep != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(oct.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (oct < Convert.ToInt32(average) && oct != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(nov.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (nov < Convert.ToInt32(average) && nov != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(dec.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (dec < Convert.ToInt32(average) && dec != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(total.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber);
                        excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber);

                        totalSubTotal++;
                        totalRowBranchHead++;
                        totalRowColor++;
                        totalRowDealer++;
                        totalRowModel++;
                        totalRowSalesCo++;//<-- Untuk masukkin total nilai sub total tiap Model Catagory
                        totalRowSalesHead++;
                        totalRowSalsman++;
                        rowIndex++;

                        average = totalGT / salesmanMonth;
                        //Masukkin sub total model category
                        excelReport.SetCellValue("SubTotal", 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth + colorWidth, ExcelCellStyle.PurpleTotal);
                        excelReport.SetCellValue(janGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (janGT < Convert.ToInt32(average) && janGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(febGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (febGT < Convert.ToInt32(average) && febGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(marGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (marGT < Convert.ToInt32(average) && marGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(aprGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (aprGT < Convert.ToInt32(average) && aprGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(mayGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (mayGT < Convert.ToInt32(average) && mayGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(junGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (junGT < Convert.ToInt32(average) && junGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(julGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (julGT < Convert.ToInt32(average) && julGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(augGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (augGT < Convert.ToInt32(average) && augGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(sepGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sepGT < Convert.ToInt32(average) && sepGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(octGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (octGT < Convert.ToInt32(average) && octGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(novGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (novGT < Convert.ToInt32(average) && novGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(decGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (decGT < Convert.ToInt32(average) && decGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(totalGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber);
                        excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber);

                        totalSubTotal++;
                        totalRowBranchHead++;
                        totalRowColor++;
                        totalRowDealer++;
                        totalRowModel++;
                        totalRowSalesCo++;//<-- Untuk masukkin total nilai sub total tiap Model Catagory
                        totalRowSalesHead++;
                        totalRowSalsman++;
                        rowIndex++;

                        jan = feb = mar = apr = may = jun = jul = aug = sep = oct = nov = dec = total = 0;
                        janGT = febGT = marGT = aprGT = mayGT = junGT = julGT = augGT = sepGT = octGT = novGT = decGT = totalGT = 0;

                        if (parameter[9] == "1")
                        {
                            excelReport.ReplaceRestorePoint(RestorePoint.SalesmanRestorePoint.ToString(), "AVG", 0 + dealerWidth, totalRowSalsman, outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.LeftBorderedStandardWrap);
                            excelReport.RemoveRestorePoint(RestorePoint.GradeRestorePoint.ToString());
                        }
                        else
                        {
                            excelReport.ReplaceRestorePoint(RestorePoint.SalesCoordinatorrestorePoint.ToString(), "AVG", 0 + dealerWidth, totalRowSalsman, outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.LeftBorderedStandardWrap);
                        }
                        excelReport.ReplaceRestorePoint(RestorePoint.CompanyCodeRestorePoint.ToString(), dealer.ToString(), 0, totalRowDealer, dealerWidth, ExcelCellStyle.LeftBorderedStandardWrap);

                        totalRowColor = 0;
                        totalRowSalsman = 0;
                        totalRowSalesCo = 0;
                        totalRowModel = 0;
                        totalRowSalesHead = 0;
                        totalRowBranchHead = 0;
                        totalRowDealer = 0;

                        //Tambahkan 1 Row Index untuk spasi pergantian sales coordinator
                        rowIndex++;
                        excelReport.CreateBlankRow(8 + rowIndex);

                        excelReport.RestorePoint(8 + rowIndex, RestorePoint.CompanyCodeRestorePoint.ToString());
                        excelReport.RestorePoint(8 + rowIndex, RestorePoint.OutletRestorePoint.ToString());
                        excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesHeadRestorePoint.ToString());
                        excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesCoordinatorrestorePoint.ToString());
                        if (parameter[9] == "1")
                        {
                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesmanRestorePoint.ToString());
                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.GradeRestorePoint.ToString());
                        }

                        if (parameter[10] == "1")
                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.MarketModelRestorePoint.ToString());

                        excelReport.SetCellValue(row["ColourName"].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + modelWidth, 1, colorWidth, ExcelCellStyle.LeftBorderedStandard);
                        seq = row["Seq"].ToString();
                        seq2 = row["Seq2"].ToString();
                        salesMan = row["SalesmanName"].ToString();
                        grade = row["Grade"].ToString();
                        marketModel = row["MarketModel"].ToString();
                        modelCatagory = row["ModelCatagory"].ToString();
                        salesCoordinator = row["SalesCoordinatorName"].ToString();
                        salesHead = row["SalesHeadName"].ToString();
                        outlet = row["BranchName"].ToString();
                        dealer = row["CompanyName"].ToString();

                        totalRowColor = 0;
                        totalRowSalsman = 0;
                        totalRowSalesCo = 0;
                        totalRowSalesHead = 0;
                        totalRowBranchHead = 0;
                        totalRowDealer = 0;
                    }
                    #endregion
                    #region Parameter 10
                    else if (parameter[10] == "1")
                    {
                        decimal average = total / salesmanMonth;
                        //Masukin total Model Catagory
                        excelReport.SetCellValue("Total " + modelCatagory.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth + colorWidth, ExcelCellStyle.BlueTotal);
                        excelReport.SetCellValue(jan.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber);
                        excelReport.SetCellValue(feb.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber);
                        excelReport.SetCellValue(mar.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber);
                        excelReport.SetCellValue(apr.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber);
                        excelReport.SetCellValue(may.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber);
                        excelReport.SetCellValue(jun.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber);
                        excelReport.SetCellValue(jul.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber);
                        excelReport.SetCellValue(aug.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber);
                        excelReport.SetCellValue(sep.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber);
                        excelReport.SetCellValue(oct.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber);
                        excelReport.SetCellValue(nov.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber);
                        excelReport.SetCellValue(dec.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber);
                        excelReport.SetCellValue(total.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber);
                        excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber);

                        totalSubTotal++;
                        totalRowBranchHead++;
                        totalRowColor++;
                        totalRowDealer++;
                        totalRowModel++;
                        totalRowSalesCo++;//<-- Untuk masukkin total nilai sub total tiap Model Catagory
                        totalRowSalesHead++;
                        totalRowSalsman++;
                        rowIndex++;

                        average = total / salesmanMonth;
                        //Masukkin sub total model category
                        excelReport.SetCellValue("SubTotal", 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth + colorWidth, ExcelCellStyle.PurpleTotal);
                        excelReport.SetCellValue(janGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber);
                        excelReport.SetCellValue(febGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber);
                        excelReport.SetCellValue(marGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber);
                        excelReport.SetCellValue(aprGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber);
                        excelReport.SetCellValue(mayGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber);
                        excelReport.SetCellValue(junGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber);
                        excelReport.SetCellValue(julGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber);
                        excelReport.SetCellValue(augGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber);
                        excelReport.SetCellValue(sepGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber);
                        excelReport.SetCellValue(octGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber);
                        excelReport.SetCellValue(novGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber);
                        excelReport.SetCellValue(decGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber);
                        excelReport.SetCellValue(totalGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber);
                        excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber);

                        totalSubTotal++;
                        totalRowBranchHead++;
                        totalRowColor++;
                        totalRowDealer++;
                        totalRowModel++;
                        totalRowSalesCo++;//<-- Untuk masukkin total nilai sub total tiap Model Catagory
                        totalRowSalesHead++;
                        totalRowSalsman++;
                        rowIndex++;

                        jan = feb = mar = apr = may = jun = jul = aug = sep = oct = nov = dec = total = 0;
                        janGT = febGT = marGT = aprGT = mayGT = junGT = julGT = augGT = sepGT = octGT = novGT = decGT = totalGT = 0;

                        if (parameter[9].ToString() == "1")
                        {
                            excelReport.ReplaceRestorePoint(RestorePoint.SalesmanRestorePoint.ToString(), "AVG", 0 + dealerWidth, totalRowSalsman, outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.LeftBorderedStandardWrap);
                            excelReport.RemoveRestorePoint(RestorePoint.GradeRestorePoint.ToString());
                        }
                        else
                        {
                            if (seq == "4" && seq2 == "3" && parameter[9] == "0" && parameter[10] == "1")
                                excelReport.ReplaceRestorePoint(RestorePoint.SalesCoordinatorrestorePoint.ToString(), "AVG", 0 + dealerWidth, totalRowSalsman, outletWidth + salesHeadWidth + salesCoordinatorWidth, ExcelCellStyle.LeftBorderedStandardWrap);
                            else
                                excelReport.ReplaceRestorePoint(RestorePoint.SalesCoordinatorrestorePoint.ToString(), "AVG", 0 + dealerWidth + outletWidth + salesHeadWidth, totalRowSalsman, salesCoordinatorWidth, ExcelCellStyle.LeftBorderedStandardWrap);
                        }
                        excelReport.ReplaceRestorePoint(RestorePoint.CompanyCodeRestorePoint.ToString(), dealer.ToString(), 0, totalRowDealer, dealerWidth, ExcelCellStyle.LeftBorderedStandardWrap);

                        totalRowColor = 0;
                        totalRowSalsman = 0;
                        totalRowSalesCo = 0;
                        totalRowModel = 0;
                        totalRowSalesHead = 0;
                        totalRowBranchHead = 0;
                        totalRowDealer = 0;

                        //Tambahkan 1 Row Index untuk spasi pergantian sales coordinator
                        rowIndex++;
                        excelReport.CreateBlankRow(8 + rowIndex);


                        excelReport.RestorePoint(8 + rowIndex, RestorePoint.CompanyCodeRestorePoint.ToString());
                        excelReport.RestorePoint(8 + rowIndex, RestorePoint.OutletRestorePoint.ToString());
                        excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesHeadRestorePoint.ToString());
                        excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesCoordinatorrestorePoint.ToString());
                        if (parameter[9].ToString() == "1")
                        {
                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesmanRestorePoint.ToString());
                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.GradeRestorePoint.ToString());
                        }
                        excelReport.SetCellValue(row["MarketModel"].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth, ExcelCellStyle.LeftBorderedStandard);

                        seq = row["Seq"].ToString();
                        seq2 = row["Seq2"].ToString();
                        salesMan = row["SalesmanName"].ToString();
                        grade = row["Grade"].ToString();
                        marketModel = row["MarketModel"].ToString();
                        modelCatagory = row["ModelCatagory"].ToString();
                        salesCoordinator = row["SalesCoordinatorName"].ToString();
                        salesHead = row["SalesHeadName"].ToString();
                        dealer = row["CompanyName"].ToString();
                        outlet = row["BranchName"].ToString();


                    }
                    #endregion
                    #region Parameter9
                    else if (parameter[9] == "1")
                    {
                        excelReport.ReplaceRestorePoint(RestorePoint.CompanyCodeRestorePoint.ToString(), dealer.ToString(), 0, totalRowDealer, dealerWidth, ExcelCellStyle.LeftBorderedTop);

                        rowIndex++;

                        excelReport.RestorePoint(8 + rowIndex, RestorePoint.CompanyCodeRestorePoint.ToString());
                        excelReport.RestorePoint(8 + rowIndex, RestorePoint.OutletRestorePoint.ToString());
                        excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesHeadRestorePoint.ToString());
                        excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesCoordinatorrestorePoint.ToString());
                        excelReport.SetCellValue(row["SalesmanName"].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth, 1, salesManWidth, ExcelCellStyle.LeftBorderedStandard);
                        excelReport.SetCellValue(row["Grade"].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth, 1, salesManWidth, ExcelCellStyle.LeftBorderedStandard);
                        totalColor = ExcelCellStyle.RightBorderedStandardNumber;
                        totalRedColor = ExcelCellStyle.RightBorderedStandardRedNumber;

                        totalRowDealer = 0;
                        totalRowColor = 0;
                        totalRowBranchHead = 0;
                        totalRowModel = 0;
                        totalRowSalesCo = 0;
                        totalRowSalesHead = 0;
                        totalRowSalsman = 0;
                        seq = row["Seq"].ToString();
                        seq2 = row["Seq2"].ToString();
                        salesCoordinator = row["SalesCoordinatorName"].ToString();
                        salesHead = row["SalesHeadName"].ToString();
                        dealer = row["CompanyName"].ToString();
                        outlet = row["BranchName"].ToString();
                    }
                    #endregion
                    #region UnParameter
                    else
                    {
                        excelReport.ReplaceRestorePoint(RestorePoint.CompanyCodeRestorePoint.ToString(), dealer.ToString(), 0, totalRowDealer, dealerWidth, ExcelCellStyle.LeftBorderedTop);

                        excelReport.RestorePoint(8 + rowIndex, RestorePoint.CompanyCodeRestorePoint.ToString());
                        excelReport.RestorePoint(8 + rowIndex, RestorePoint.OutletRestorePoint.ToString());
                        excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesHeadRestorePoint.ToString());

                        excelReport.SetCellValue(row["SalesCoordinatorName"].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth, 1, salesCoordinatorWidth, ExcelCellStyle.LeftBorderedStandard);
                        totalColor = ExcelCellStyle.RightBorderedStandardNumber;
                        totalRedColor = ExcelCellStyle.RightBorderedStandardRedNumber;

                        totalRowDealer = 0;
                        totalRowColor = 0;
                        totalRowBranchHead = 0;
                        totalRowModel = 0;
                        totalRowSalesCo = 0;
                        totalRowSalesHead = 0;
                        totalRowSalsman = 0;
                        seq = row["Seq"].ToString();
                        seq2 = row["Seq2"].ToString();
                        salesCoordinator = row["SalesCoordinatorName"].ToString();
                        salesHead = row["SalesHeadName"].ToString();
                        dealer = row["CompanyName"].ToString();
                        outlet = row["BranchName"].ToString();
                    }
                    #endregion
                    #endregion
                    if (row["Seq"].ToString() == "1" && row["Seq2"].ToString() == "1")
                    {
                        totalSalesCo = 1;
                        totalSalesHead = 1;
                        totalOutlet = 1;
                        totalDealer++;
                        totalDealerJabodetabek++;
                        totalDealerGT++;
                    }
                }
                else if (year == row["Year"].ToString()
            && seq != row["Seq"].ToString()
            && area != row["Area"].ToString())
                {
                    #region Group
                    decimal average = 0;
                    if (parameter[11] == "1")
                    {
                        excelReport.ReplaceRestorePoint(RestorePoint.MarketModelRestorePoint.ToString(), marketModel.ToString(), 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, totalRowColor, modelWidth, ExcelCellStyle.LeftBorderedStandard);
                        average = total / averageMonth;
                        //Masukin total Model Catagory
                        excelReport.SetCellValue("Total " + modelCatagory.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth + colorWidth, ExcelCellStyle.BlueTotal);
                        excelReport.SetCellValue(jan.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (jan < Convert.ToInt32(average) && jan != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(feb.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (feb < Convert.ToInt32(average) && feb != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(mar.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (mar < Convert.ToInt32(average) && mar != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(apr.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (apr < Convert.ToInt32(average) && apr != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(may.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (may < Convert.ToInt32(average) && may != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(jun.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (jun < Convert.ToInt32(average) && jun != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(jul.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (jul < Convert.ToInt32(average) && jul != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(aug.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (aug < Convert.ToInt32(average) && aug != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(sep.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sep < Convert.ToInt32(average) && sep != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(oct.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (oct < Convert.ToInt32(average) && oct != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(nov.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (nov < Convert.ToInt32(average) && nov != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(dec.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (dec < Convert.ToInt32(average) && dec != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(total.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber);
                        excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber);

                        totalSubTotal++;
                        totalRowBranchHead++;
                        totalRowColor++;
                        totalRowDealer++;
                        totalRowModel++;
                        totalRowSalesCo++;//<-- Untuk masukkin total nilai sub total tiap Model Catagory
                        totalRowSalesHead++;
                        totalRowSalsman++;
                        rowIndex++;

                        average = totalGT / averageMonth;
                        //Masukkin sub total model category
                        excelReport.SetCellValue("SubTotal", 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth + colorWidth, ExcelCellStyle.PurpleTotal);
                        excelReport.SetCellValue(janGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (janGT < Convert.ToInt32(average) && janGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(febGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (febGT < Convert.ToInt32(average) && febGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(marGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (marGT < Convert.ToInt32(average) && marGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(aprGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (aprGT < Convert.ToInt32(average) && aprGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(mayGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (mayGT < Convert.ToInt32(average) && mayGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(junGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (junGT < Convert.ToInt32(average) && junGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(julGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (julGT < Convert.ToInt32(average) && julGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(augGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (augGT < Convert.ToInt32(average) && augGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(sepGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sepGT < Convert.ToInt32(average) && sepGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(octGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (octGT < Convert.ToInt32(average) && octGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(novGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (novGT < Convert.ToInt32(average) && novGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(decGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (decGT < Convert.ToInt32(average) && decGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(totalGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber);
                        excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber);

                        totalSubTotal++;
                        totalRowBranchHead++;
                        totalRowColor++;
                        totalRowDealer++;
                        totalRowModel++;
                        totalRowSalesCo++;//<-- Untuk masukkin total nilai sub total tiap Model Catagory
                        totalRowSalesHead++;
                        totalRowSalsman++;
                        rowIndex++;

                        jan = feb = mar = apr = may = jun = jul = aug = sep = oct = nov = dec = total = 0;
                        janGT = febGT = marGT = aprGT = mayGT = junGT = julGT = augGT = sepGT = octGT = novGT = decGT = totalGT = 0;

                        if (parameter[9] == "1")
                        {
                            excelReport.ReplaceRestorePoint(RestorePoint.SalesmanRestorePoint.ToString(), "AVG", 0 + dealerWidth, totalRowSalsman, outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.LeftBorderedStandardWrap);
                            excelReport.RemoveRestorePoint(RestorePoint.GradeRestorePoint.ToString());
                        }
                        else
                            excelReport.ReplaceRestorePoint(RestorePoint.SalesCoordinatorrestorePoint.ToString(), "AVG", 0 + dealerWidth, totalRowSalsman, outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.LeftBorderedStandardWrap);
                        excelReport.ReplaceRestorePoint(RestorePoint.CompanyCodeRestorePoint.ToString(), dealer.ToString(), 0, totalRowDealer, dealerWidth, ExcelCellStyle.LeftBorderedStandardWrap);

                        totalRowColor = 0;
                        totalRowSalsman = 0;
                        totalRowSalesCo = 0;
                        totalRowModel = 0;
                        totalRowSalesHead = 0;
                        totalRowBranchHead = 0;
                        totalRowDealer = 0;
                    }
                    else if (parameter[10] == "1")
                    {
                        average = total / averageMonth;

                        //Masukin total Model Catagory
                        excelReport.SetCellValue("Total " + modelCatagory.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth + colorWidth, ExcelCellStyle.BlueTotal);
                        excelReport.SetCellValue(jan.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (jan < Convert.ToInt32(average) && jan != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(feb.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (feb < Convert.ToInt32(average) && feb != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(mar.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (mar < Convert.ToInt32(average) && mar != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(apr.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (apr < Convert.ToInt32(average) && apr != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(may.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (may < Convert.ToInt32(average) && may != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(jun.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (jun < Convert.ToInt32(average) && jun != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(jul.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (jul < Convert.ToInt32(average) && jul != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(aug.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (aug < Convert.ToInt32(average) && aug != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(sep.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sep < Convert.ToInt32(average) && sep != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(oct.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (oct < Convert.ToInt32(average) && oct != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(nov.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (nov < Convert.ToInt32(average) && nov != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(dec.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (dec < Convert.ToInt32(average) && dec != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(total.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber);
                        excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber);

                        totalSubTotal++;
                        totalRowBranchHead++;
                        totalRowColor++;
                        totalRowDealer++;
                        totalRowModel++;
                        totalRowSalesCo++;//<-- Untuk masukkin total nilai sub total tiap Model Catagory
                        totalRowSalesHead++;
                        totalRowSalsman++;
                        rowIndex++;

                        average = total / averageMonth;
                        //Masukkin sub total model category
                        excelReport.SetCellValue("SubTotal", 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth + colorWidth, ExcelCellStyle.PurpleTotal);
                        excelReport.SetCellValue(janGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (janGT < Convert.ToInt32(average) && janGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(febGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (febGT < Convert.ToInt32(average) && febGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(marGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (marGT < Convert.ToInt32(average) && marGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(aprGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (aprGT < Convert.ToInt32(average) && aprGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(mayGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (mayGT < Convert.ToInt32(average) && mayGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(junGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (junGT < Convert.ToInt32(average) && junGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(julGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (julGT < Convert.ToInt32(average) && julGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(augGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (augGT < Convert.ToInt32(average) && augGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(sepGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sepGT < Convert.ToInt32(average) && sepGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(octGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (octGT < Convert.ToInt32(average) && octGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(novGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (novGT < Convert.ToInt32(average) && novGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(decGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (decGT < Convert.ToInt32(average) && decGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(totalGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber);
                        excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber);

                        totalSubTotal++;
                        totalRowBranchHead++;
                        totalRowColor++;
                        totalRowDealer++;
                        totalRowModel++;
                        totalRowSalesCo++;//<-- Untuk masukkin total nilai sub total tiap Model Catagory
                        totalRowSalesHead++;
                        totalRowSalsman++;
                        rowIndex++;
                        if (parameter[9] == "0" && parameter[10] == "1")
                            excelReport.ReplaceRestorePoint(RestorePoint.SalesCoordinatorrestorePoint.ToString(), "AVG", 0 + dealerWidth, totalRowSalsman, outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.LeftBorderedTop);
                        else
                            excelReport.ReplaceRestorePoint(RestorePoint.SalesmanRestorePoint.ToString(), "AVG", 0 + dealerWidth, totalRowSalsman, outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.LeftBorderedTop);

                        excelReport.ReplaceRestorePoint(RestorePoint.CompanyCodeRestorePoint.ToString(), dealer.ToString(), 0, totalRowDealer, dealerWidth, ExcelCellStyle.LeftBorderedTop);
                    }
                    else
                    {
                        excelReport.ReplaceRestorePoint(RestorePoint.CompanyCodeRestorePoint.ToString(), dealer.ToString(), 0, totalRowDealer, dealerWidth, ExcelCellStyle.LeftBorderedTop);
                    }

                    average = totalArea / averageMonth;

                    //Masukin total Model Catagory
                    if (area != "JABODETABEK")
                        excelReport.SetCellValue("Total " + area.ToString(), 8 + rowIndex, 0, 1, dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + modelWidth + colorWidth, ExcelCellStyle.PurpleTotal);
                    else
                        excelReport.SetCellValue("Total Dealer " + area.ToString(), 8 + rowIndex, 0, 1, dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + modelWidth + colorWidth, ExcelCellStyle.PurpleTotal);

                    excelReport.SetCellValue(janArea.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (janArea < Convert.ToInt32(average) && janArea != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(febArea.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (febArea < Convert.ToInt32(average) && febArea != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(marArea.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (marArea < Convert.ToInt32(average) && marArea != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(aprArea.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (aprArea < Convert.ToInt32(average) && aprArea != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(mayArea.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (mayArea < Convert.ToInt32(average) && mayArea != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(junArea.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (junArea < Convert.ToInt32(average) && junArea != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(julArea.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (julArea < Convert.ToInt32(average) && julArea != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(augArea.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (augArea < Convert.ToInt32(average) && augArea != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(sepArea.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sepArea < Convert.ToInt32(average) && sepArea != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(octArea.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (octArea < Convert.ToInt32(average) && octArea != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(novArea.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (novArea < Convert.ToInt32(average) && novArea != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(decArea.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (decArea < Convert.ToInt32(average) && decArea != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(totalArea.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);

                    rowIndex++;

                    janAVG = janArea / totalDealer;
                    febAVG = febArea / totalDealer;
                    marAVG = marArea / totalDealer;
                    aprAVG = aprArea / totalDealer;
                    mayAVG = mayArea / totalDealer;
                    junAVG = junArea / totalDealer;
                    julAVG = julArea / totalDealer;
                    augAVG = augArea / totalDealer;
                    sepAVG = sepArea / totalDealer;
                    octAVG = octArea / totalDealer;
                    novAVG = novArea / totalDealer;
                    decAVG = decArea / totalDealer;
                    totalAVG = (janAVG + febAVG + marAVG + aprAVG + mayAVG + junAVG + julAVG + augAVG + sepAVG + octAVG + novAVG + decAVG);
                    average = totalAVG / averageMonth;

                    if (area != "JABODETABEK")
                        excelReport.SetCellValue("AVG " + area.ToString(), 8 + rowIndex, 0, 1, dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + modelWidth + colorWidth, ExcelCellStyle.BrownTotal);
                    else
                        excelReport.SetCellValue("AVG DEALER " + area.ToString(), 8 + rowIndex, 0, 1, dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + modelWidth + colorWidth, ExcelCellStyle.BrownTotal);

                    excelReport.SetCellValue(janAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (janAVG < Convert.ToInt32(average) && janAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(febAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (febAVG < Convert.ToInt32(average) && febAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(marAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (marAVG < Convert.ToInt32(average) && marAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(aprAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (aprAVG < Convert.ToInt32(average) && aprAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(mayAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (mayAVG < Convert.ToInt32(average) && mayAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(junAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (junAVG < Convert.ToInt32(average) && junAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(julAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (julAVG < Convert.ToInt32(average) && julAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(augAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (augAVG < Convert.ToInt32(average) && augAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(sepAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sepAVG < Convert.ToInt32(average) && sepAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(octAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (octAVG < Convert.ToInt32(average) && octAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(novAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (novAVG < Convert.ToInt32(average) && novAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(decAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (decAVG < Convert.ToInt32(average) && decAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(totalAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.BrownTotalNumber, true);
                    rowIndex++;

                    if (parameter[2].ToString() == "ALL" && parameter[4].ToString() == "ALL" && parameter[6].ToString() == "ALL" && area == "JABODETABEK")
                    {

                        average = totalArea / averageMonthJBDTK;

                        //Masukin total Model Catagory
                        excelReport.SetCellValue("Total " + area.ToString(), 8 + rowIndex, 0, 1, dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + modelWidth + colorWidth, ExcelCellStyle.PurpleTotal);
                        excelReport.SetCellValue(janJabodetabek.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (janJabodetabek < Convert.ToInt32(average) && janJabodetabek != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(febJabodetabek.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (febJabodetabek < Convert.ToInt32(average) && febJabodetabek != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(marJabodetabek.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (marJabodetabek < Convert.ToInt32(average) && marJabodetabek != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(aprJabodetabek.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (aprJabodetabek < Convert.ToInt32(average) && aprJabodetabek != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(mayJabodetabek.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (mayJabodetabek < Convert.ToInt32(average) && mayJabodetabek != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(junJabodetabek.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (junJabodetabek < Convert.ToInt32(average) && junJabodetabek != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(julJabodetabek.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (julJabodetabek < Convert.ToInt32(average) && julJabodetabek != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(augJabodetabek.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (augJabodetabek < Convert.ToInt32(average) && augJabodetabek != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(sepJabodetabek.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sepJabodetabek < Convert.ToInt32(average) && sepJabodetabek != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(octJabodetabek.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (octJabodetabek < Convert.ToInt32(average) && octJabodetabek != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(novJabodetabek.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (novJabodetabek < Convert.ToInt32(average) && novJabodetabek != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(decJabodetabek.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (decJabodetabek < Convert.ToInt32(average) && decJabodetabek != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(totalJabodetabek.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);

                        rowIndex++;

                        janAVG = janJabodetabek / totalDealerJabodetabek;
                        febAVG = febJabodetabek / totalDealerJabodetabek;
                        marAVG = marJabodetabek / totalDealerJabodetabek;
                        aprAVG = aprJabodetabek / totalDealerJabodetabek;
                        mayAVG = mayJabodetabek / totalDealerJabodetabek;
                        junAVG = junJabodetabek / totalDealerJabodetabek;
                        julAVG = julJabodetabek / totalDealerJabodetabek;
                        augAVG = augJabodetabek / totalDealerJabodetabek;
                        sepAVG = sepJabodetabek / totalDealerJabodetabek;
                        octAVG = octJabodetabek / totalDealerJabodetabek;
                        novAVG = novJabodetabek / totalDealerJabodetabek;
                        decAVG = decJabodetabek / totalDealerJabodetabek;
                        totalAVG = (janAVG + febAVG + marAVG + aprAVG + mayAVG + junAVG + julAVG + augAVG + sepAVG + octAVG + novAVG + decAVG);
                        average = totalAVG / averageMonthJBDTK;

                        excelReport.SetCellValue("AVG " + area.ToString(), 8 + rowIndex, 0, 1, dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + modelWidth + colorWidth, ExcelCellStyle.BrownTotal);
                        excelReport.SetCellValue(janAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (janAVG < Convert.ToInt32(average) && janAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                        excelReport.SetCellValue(febAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (febAVG < Convert.ToInt32(average) && febAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                        excelReport.SetCellValue(marAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (marAVG < Convert.ToInt32(average) && marAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                        excelReport.SetCellValue(aprAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (aprAVG < Convert.ToInt32(average) && aprAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                        excelReport.SetCellValue(mayAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (mayAVG < Convert.ToInt32(average) && mayAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                        excelReport.SetCellValue(junAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (junAVG < Convert.ToInt32(average) && junAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                        excelReport.SetCellValue(julAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (julAVG < Convert.ToInt32(average) && julAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                        excelReport.SetCellValue(augAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (augAVG < Convert.ToInt32(average) && augAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                        excelReport.SetCellValue(sepAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sepAVG < Convert.ToInt32(average) && sepAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                        excelReport.SetCellValue(octAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (octAVG < Convert.ToInt32(average) && octAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                        excelReport.SetCellValue(novAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (novAVG < Convert.ToInt32(average) && novAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                        excelReport.SetCellValue(decAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (decAVG < Convert.ToInt32(average) && decAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                        excelReport.SetCellValue(totalAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.BrownTotalNumber, true);
                        excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.BrownTotalNumber, true);
                        rowIndex++;
                    }

                    totalRowColor = 0;
                    totalRowSalsman = 0;
                    totalRowSalesCo = 0;
                    totalRowSalesHead = 0;
                    totalRowBranchHead = 0;
                    totalRowDealer = 0;
                    averageMonth = 0;
                    janArea = febArea = marArea = aprArea = mayArea = junArea = julArea = augArea = sepArea = octArea = novArea = decArea = totalArea = 0;
                    janAVG = febAVG = marAVG = aprAVG = mayAVG = junAVG = julAVG = augAVG = sepAVG = octAVG = novAVG = decAVG = totalAVG = 0;

                    rowIndex++;

                    excelReport.RestorePoint(8 + rowIndex, RestorePoint.CompanyCodeRestorePoint.ToString());
                    excelReport.RestorePoint(8 + rowIndex, RestorePoint.OutletRestorePoint.ToString());
                    excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesHeadRestorePoint.ToString());

                    if (parameter[11] == "1")
                    {
                        excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesCoordinatorrestorePoint.ToString());

                        if (parameter[9] == "1")
                        {
                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesmanRestorePoint.ToString());
                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.GradeRestorePoint.ToString());
                        }

                        excelReport.RestorePoint(8 + rowIndex, RestorePoint.MarketModelRestorePoint.ToString());
                        excelReport.SetCellValue(row["ColourName"].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + modelWidth, 1, colorWidth, ExcelCellStyle.LeftBorderedStandard);
                    }
                    else if (parameter[10] == "1")
                    {
                        excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesCoordinatorrestorePoint.ToString());

                        if (parameter[9] == "1")
                        {
                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesmanRestorePoint.ToString());
                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.GradeRestorePoint.ToString());
                        }
                        excelReport.SetCellValue(row["MarketModel"].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth, ExcelCellStyle.LeftBorderedStandard);
                    }
                    else if (parameter[9] == "1")
                    {
                        excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesCoordinatorrestorePoint.ToString());
                        excelReport.SetCellValue(row["SalesmanName"].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth, 1, salesManWidth, ExcelCellStyle.LeftBorderedStandard);
                        excelReport.SetCellValue(row["Grade"].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth, 1, gradeWidth, ExcelCellStyle.LeftBorderedStandard);
                    }
                    else
                    {
                        excelReport.SetCellValue(row["SalesCoordinatorName"].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth, 1, salesCoordinatorWidth, ExcelCellStyle.LeftBorderedStandard);

                    }
                    #endregion

                    totalColor = ExcelCellStyle.RightBorderedStandardNumber;
                    totalRedColor = ExcelCellStyle.RightBorderedStandardRedNumber;

                    if (row["Seq"].ToString() == "1" && row["Seq2"].ToString() == "1")
                    {
                        totalDealer = 1;
                        totalDealerGT++;
                    }

                    seq = row["seq"].ToString();
                    seq2 = row["Seq2"].ToString();
                    area = row["Area"].ToString();
                    dealer = row["CompanyName"].ToString();
                    outlet = row["BranchName"].ToString();
                    salesHead = row["SalesHeadName"].ToString();
                    salesCoordinator = row["SalesCoordinatorName"].ToString();
                    salesMan = parameter[9] == "1" ? row["SalesmanName"].ToString() : "";
                    grade = parameter[9] == "1" ? row["Grade"].ToString() : "";
                    modelCatagory = parameter[10] == "1" ? row["ModelCatagory"].ToString() : "";
                    marketModel = parameter[10] == "1" ? row["MarketModel"].ToString() : "";
                }
                else if (year != row["Year"].ToString())
                {
                    #region Group
                    decimal average = 0;
                    if (parameter[11] == "1")
                    {
                        excelReport.ReplaceRestorePoint(RestorePoint.MarketModelRestorePoint.ToString(), marketModel.ToString(), 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, totalRowColor, modelWidth, ExcelCellStyle.LeftBorderedStandard);
                        average = total / averageMonth;
                        //Masukin total Model Catagory
                        excelReport.SetCellValue("Total " + modelCatagory.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth + colorWidth, ExcelCellStyle.BlueTotal);
                        excelReport.SetCellValue(jan.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (jan < Convert.ToInt32(average) && jan != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(feb.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (feb < Convert.ToInt32(average) && feb != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(mar.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (mar < Convert.ToInt32(average) && mar != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(apr.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (apr < Convert.ToInt32(average) && apr != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(may.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (may < Convert.ToInt32(average) && may != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(jun.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (jun < Convert.ToInt32(average) && jun != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(jul.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (jul < Convert.ToInt32(average) && jul != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(aug.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (aug < Convert.ToInt32(average) && aug != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(sep.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sep < Convert.ToInt32(average) && sep != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(oct.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (oct < Convert.ToInt32(average) && oct != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(nov.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (nov < Convert.ToInt32(average) && nov != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(dec.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (dec < Convert.ToInt32(average) && dec != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(total.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber);
                        excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber);

                        totalSubTotal++;
                        totalRowBranchHead++;
                        totalRowColor++;
                        totalRowDealer++;
                        totalRowModel++;
                        totalRowSalesCo++;//<-- Untuk masukkin total nilai sub total tiap Model Catagory
                        totalRowSalesHead++;
                        totalRowSalsman++;
                        rowIndex++;

                        average = totalGT / averageMonth;
                        //Masukkin sub total model category
                        excelReport.SetCellValue("SubTotal", 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth + colorWidth, ExcelCellStyle.PurpleTotal);
                        excelReport.SetCellValue(janGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (janGT < Convert.ToInt32(average) && janGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(febGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (febGT < Convert.ToInt32(average) && febGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(marGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (marGT < Convert.ToInt32(average) && marGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(aprGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (aprGT < Convert.ToInt32(average) && aprGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(mayGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (mayGT < Convert.ToInt32(average) && mayGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(junGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (junGT < Convert.ToInt32(average) && junGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(julGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (julGT < Convert.ToInt32(average) && julGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(augGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (augGT < Convert.ToInt32(average) && augGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(sepGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sepGT < Convert.ToInt32(average) && sepGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(octGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (octGT < Convert.ToInt32(average) && octGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(novGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (novGT < Convert.ToInt32(average) && novGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(decGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (decGT < Convert.ToInt32(average) && decGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(totalGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber);
                        excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber);

                        totalSubTotal++;
                        totalRowBranchHead++;
                        totalRowColor++;
                        totalRowDealer++;
                        totalRowModel++;
                        totalRowSalesCo++;//<-- Untuk masukkin total nilai sub total tiap Model Catagory
                        totalRowSalesHead++;
                        totalRowSalsman++;
                        rowIndex++;

                        jan = feb = mar = apr = may = jun = jul = aug = sep = oct = nov = dec = total = 0;
                        janGT = febGT = marGT = aprGT = mayGT = junGT = julGT = augGT = sepGT = octGT = novGT = decGT = totalGT = 0;

                        if (parameter[9] == "1")
                        {
                            excelReport.ReplaceRestorePoint(RestorePoint.SalesmanRestorePoint.ToString(), "AVG", 0 + dealerWidth, totalRowSalsman, outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.LeftBorderedStandardWrap);
                            excelReport.RemoveRestorePoint(RestorePoint.GradeRestorePoint.ToString());
                        }
                        else
                            excelReport.ReplaceRestorePoint(RestorePoint.SalesCoordinatorrestorePoint.ToString(), "AVG", 0 + dealerWidth, totalRowSalsman, outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.LeftBorderedStandardWrap);
                        excelReport.ReplaceRestorePoint(RestorePoint.CompanyCodeRestorePoint.ToString(), dealer.ToString(), 0, totalRowDealer, dealerWidth, ExcelCellStyle.LeftBorderedStandardWrap);

                        //CreateFooter(excelReport, rowIndex, indentTitleDesc, false, false);

                        //excelReport.ChangeSheet(row["Year"].ToString());
                        //CreateHeader(excelReport, title, rowIndex, indentTitleDesc, currentYear.ToString());
                        //totalRowBranchHead = 0;
                        //totalRowColor = 0;
                        //totalRowDealer = 0;
                        //totalRowModel = 0;
                        //totalRowSalesCo = 0;
                        //totalRowSalesHead = 0;
                        //totalRowSalsman = 0;
                        //totalRow = 0;
                        //rowIndex = 0;
                    }
                    else if (parameter[10] == "1")
                    {
                        average = total / averageMonth;

                        //Masukin total Model Catagory
                        excelReport.SetCellValue("Total " + modelCatagory.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth + colorWidth, ExcelCellStyle.BlueTotal);
                        excelReport.SetCellValue(jan.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (jan < Convert.ToInt32(average) && jan != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(feb.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (feb < Convert.ToInt32(average) && feb != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(mar.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (mar < Convert.ToInt32(average) && mar != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(apr.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (apr < Convert.ToInt32(average) && apr != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(may.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (may < Convert.ToInt32(average) && may != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(jun.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (jun < Convert.ToInt32(average) && jun != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(jul.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (jul < Convert.ToInt32(average) && jul != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(aug.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (aug < Convert.ToInt32(average) && aug != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(sep.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sep < Convert.ToInt32(average) && sep != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(oct.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (oct < Convert.ToInt32(average) && oct != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(nov.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (nov < Convert.ToInt32(average) && nov != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(dec.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (dec < Convert.ToInt32(average) && dec != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                        excelReport.SetCellValue(total.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber);
                        excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber);

                        totalSubTotal++;
                        totalRowBranchHead++;
                        totalRowColor++;
                        totalRowDealer++;
                        totalRowModel++;
                        totalRowSalesCo++;//<-- Untuk masukkin total nilai sub total tiap Model Catagory
                        totalRowSalesHead++;
                        totalRowSalsman++;
                        rowIndex++;

                        average = total / averageMonth;
                        //Masukkin sub total model category
                        excelReport.SetCellValue("SubTotal", 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth + colorWidth, ExcelCellStyle.PurpleTotal);
                        excelReport.SetCellValue(janGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (janGT < Convert.ToInt32(average) && janGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(febGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (febGT < Convert.ToInt32(average) && febGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(marGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (marGT < Convert.ToInt32(average) && marGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(aprGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (aprGT < Convert.ToInt32(average) && aprGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(mayGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (mayGT < Convert.ToInt32(average) && mayGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(junGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (junGT < Convert.ToInt32(average) && junGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(julGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (julGT < Convert.ToInt32(average) && julGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(augGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (augGT < Convert.ToInt32(average) && augGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(sepGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sepGT < Convert.ToInt32(average) && sepGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(octGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (octGT < Convert.ToInt32(average) && octGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(novGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (novGT < Convert.ToInt32(average) && novGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(decGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (decGT < Convert.ToInt32(average) && decGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(totalGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber);
                        excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber);

                        totalSubTotal++;
                        totalRowBranchHead++;
                        totalRowColor++;
                        totalRowDealer++;
                        totalRowModel++;
                        totalRowSalesCo++;//<-- Untuk masukkin total nilai sub total tiap Model Catagory
                        totalRowSalesHead++;
                        totalRowSalsman++;
                        rowIndex++;
                        if (parameter[9] == "0" && parameter[10] == "1")
                            excelReport.ReplaceRestorePoint(RestorePoint.SalesCoordinatorrestorePoint.ToString(), "AVG", 0 + dealerWidth, totalRowSalsman, outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.LeftBorderedTop);
                        else
                            excelReport.ReplaceRestorePoint(RestorePoint.SalesmanRestorePoint.ToString(), "AVG", 0 + dealerWidth, totalRowSalsman, outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.LeftBorderedTop);

                        excelReport.ReplaceRestorePoint(RestorePoint.CompanyCodeRestorePoint.ToString(), dealer.ToString(), 0, totalRowDealer, dealerWidth, ExcelCellStyle.LeftBorderedTop);
                    }
                    else
                    {
                        excelReport.ReplaceRestorePoint(RestorePoint.CompanyCodeRestorePoint.ToString(), dealer.ToString(), 0, totalRowDealer, dealerWidth, ExcelCellStyle.LeftBorderedTop);
                    }

                    jan = feb = mar = apr = may = jun = jul = aug = sep = oct = nov = dec = total = 0;
                    janGT = febGT = marGT = aprGT = mayGT = junGT = julGT = augGT = sepGT = octGT = novGT = decGT = totalGT = 0;

                    CreateFooter(excelReport, rowIndex, indentTitleDesc, false, true);

                    excelReport.ChangeSheet(row["Year"].ToString());
                    CreateHeader(excelReport, title, rowIndex, indentTitleDesc, row["Year"].ToString());
                    totalRowBranchHead = 0;
                    totalRowColor = 0;
                    totalRowDealer = 0;
                    totalRowModel = 0;
                    totalRowSalesCo = 0;
                    totalRowSalesHead = 0;
                    totalRowSalsman = 0;
                    totalRow = 0;
                    rowIndex = 0;

                    if (parameter[11] == "1")
                    {
                        excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesCoordinatorrestorePoint.ToString());

                        if (parameter[9] == "1")
                        {
                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesmanRestorePoint.ToString());
                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.GradeRestorePoint.ToString());
                        }

                        excelReport.RestorePoint(8 + rowIndex, RestorePoint.MarketModelRestorePoint.ToString());
                        excelReport.SetCellValue(row["ColourName"].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + modelWidth, 1, colorWidth, ExcelCellStyle.LeftBorderedStandard);
                    }
                    else if (parameter[10] == "1")
                    {
                        excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesCoordinatorrestorePoint.ToString());

                        if (parameter[9] == "1")
                        {
                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesmanRestorePoint.ToString());
                            excelReport.RestorePoint(8 + rowIndex, RestorePoint.GradeRestorePoint.ToString());
                        }
                        excelReport.SetCellValue(row["MarketModel"].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth, ExcelCellStyle.LeftBorderedStandard);
                    }
                    else if (parameter[9] == "1")
                    {
                        excelReport.RestorePoint(8 + rowIndex, RestorePoint.SalesCoordinatorrestorePoint.ToString());
                        excelReport.SetCellValue(row["SalesmanName"].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth, 1, salesManWidth, ExcelCellStyle.LeftBorderedStandard);
                        excelReport.SetCellValue(row["Grade"].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth, 1, gradeWidth, ExcelCellStyle.LeftBorderedStandard);
                    }
                    else
                    {
                        excelReport.SetCellValue(row["SalesCoordinatorName"].ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth, 1, salesCoordinatorWidth, ExcelCellStyle.LeftBorderedStandard);

                    }
                    #endregion

                    totalColor = ExcelCellStyle.RightBorderedStandardNumber;
                    totalRedColor = ExcelCellStyle.RightBorderedStandardRedNumber;

                    if (row["Seq"].ToString() == "1" && row["Seq2"].ToString() == "1")
                    {
                        totalDealer = 1;
                        totalDealerGT++;
                    }
                    year = row["Year"].ToString();
                    currentYear = Convert.ToInt32(row["Year"].ToString());
                    seq = row["seq"].ToString();
                    seq2 = row["Seq2"].ToString();
                    area = row["Area"].ToString();
                    dealer = row["CompanyName"].ToString();
                    outlet = row["BranchName"].ToString();
                    salesHead = row["SalesHeadName"].ToString();
                    salesCoordinator = row["SalesCoordinatorName"].ToString();
                    salesMan = parameter[9] == "1" ? row["SalesmanName"].ToString() : "";
                    grade = parameter[9] == "1" ? row["Grade"].ToString() : "";
                    modelCatagory = parameter[10] == "1" ? row["ModelCatagory"].ToString() : "";
                    marketModel = parameter[10] == "1" ? row["MarketModel"].ToString() : "";
                }

                #region Print Detail
                if (parameter[10] == "1")
                {
                    decimal average = Convert.ToDecimal(row["Total"].ToString()) / salesmanMonth;
                    excelReport.SetCellValue(Convert.ToDecimal(row["Jan"].ToString()).ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (Convert.ToDecimal(row["Jan"].ToString()) < Convert.ToInt32(average) && Convert.ToDecimal(row["Jan"].ToString()) != 0) ? totalRedColor : totalColor, true);
                    excelReport.SetCellValue(Convert.ToDecimal(row["Feb"].ToString()).ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (Convert.ToDecimal(row["Feb"].ToString()) < Convert.ToInt32(average) && Convert.ToDecimal(row["Feb"].ToString()) != 0) ? totalRedColor : totalColor, true);
                    excelReport.SetCellValue(Convert.ToDecimal(row["Mar"].ToString()).ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (Convert.ToDecimal(row["Mar"].ToString()) < Convert.ToInt32(average) && Convert.ToDecimal(row["Mar"].ToString()) != 0) ? totalRedColor : totalColor, true);
                    excelReport.SetCellValue(Convert.ToDecimal(row["Apr"].ToString()).ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (Convert.ToDecimal(row["Apr"].ToString()) < Convert.ToInt32(average) && Convert.ToDecimal(row["Apr"].ToString()) != 0) ? totalRedColor : totalColor, true);
                    excelReport.SetCellValue(Convert.ToDecimal(row["May"].ToString()).ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (Convert.ToDecimal(row["May"].ToString()) < Convert.ToInt32(average) && Convert.ToDecimal(row["May"].ToString()) != 0) ? totalRedColor : totalColor, true);
                    excelReport.SetCellValue(Convert.ToDecimal(row["Jun"].ToString()).ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (Convert.ToDecimal(row["Jun"].ToString()) < Convert.ToInt32(average) && Convert.ToDecimal(row["Jun"].ToString()) != 0) ? totalRedColor : totalColor, true);
                    excelReport.SetCellValue(Convert.ToDecimal(row["Jul"].ToString()).ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (Convert.ToDecimal(row["Jul"].ToString()) < Convert.ToInt32(average) && Convert.ToDecimal(row["Jul"].ToString()) != 0) ? totalRedColor : totalColor, true);
                    excelReport.SetCellValue(Convert.ToDecimal(row["Aug"].ToString()).ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (Convert.ToDecimal(row["Aug"].ToString()) < Convert.ToInt32(average) && Convert.ToDecimal(row["Aug"].ToString()) != 0) ? totalRedColor : totalColor, true);
                    excelReport.SetCellValue(Convert.ToDecimal(row["Sep"].ToString()).ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (Convert.ToDecimal(row["Sep"].ToString()) < Convert.ToInt32(average) && Convert.ToDecimal(row["Sep"].ToString()) != 0) ? totalRedColor : totalColor, true);
                    excelReport.SetCellValue(Convert.ToDecimal(row["Oct"].ToString()).ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (Convert.ToDecimal(row["Oct"].ToString()) < Convert.ToInt32(average) && Convert.ToDecimal(row["Oct"].ToString()) != 0) ? totalRedColor : totalColor, true);
                    excelReport.SetCellValue(Convert.ToDecimal(row["Nov"].ToString()).ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (Convert.ToDecimal(row["Nov"].ToString()) < Convert.ToInt32(average) && Convert.ToDecimal(row["Nov"].ToString()) != 0) ? totalRedColor : totalColor, true);
                    excelReport.SetCellValue(Convert.ToDecimal(row["Dec"].ToString()).ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (Convert.ToDecimal(row["Dec"].ToString()) < Convert.ToInt32(average) && Convert.ToDecimal(row["Dec"].ToString()) != 0) ? totalRedColor : totalColor, true);
                    excelReport.SetCellValue(Convert.ToDecimal(row["Total"].ToString()).ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, totalColor, true);
                    excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, totalColor, true);
                }
                else
                {
                    if (row["Seq"].ToString() == "2" && row["Seq2"].ToString() == "2")
                    {
                        janAVG = janTtl / totalSalesCo;
                        febAVG = febTtl / totalSalesCo;
                        marAVG = marTtl / totalSalesCo;
                        aprAVG = aprTtl / totalSalesCo;
                        mayAVG = mayTtl / totalSalesCo;
                        junAVG = junTtl / totalSalesCo;
                        julAVG = julTtl / totalSalesCo;
                        augAVG = augTtl / totalSalesCo;
                        sepAVG = sepTtl / totalSalesCo;
                        octAVG = octTtl / totalSalesCo;
                        novAVG = novTtl / totalSalesCo;
                        decAVG = decTtl / totalSalesCo;
                        totalAVG = (janAVG + febAVG + marAVG + aprAVG + mayAVG + junAVG + julAVG + augAVG + sepAVG + octAVG + novAVG + decAVG);
                        decimal avg = totalAVG / salesmanMonth;

                        excelReport.SetCellValue(janAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (janAVG < Convert.ToInt32(avg) && janAVG != 0) ? totalRedColor : totalColor, true);
                        excelReport.SetCellValue(febAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (febAVG < Convert.ToInt32(avg) && febAVG != 0) ? totalRedColor : totalColor, true);
                        excelReport.SetCellValue(marAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (marAVG < Convert.ToInt32(avg) && marAVG != 0) ? totalRedColor : totalColor, true);
                        excelReport.SetCellValue(aprAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (aprAVG < Convert.ToInt32(avg) && aprAVG != 0) ? totalRedColor : totalColor, true);
                        excelReport.SetCellValue(mayAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (mayAVG < Convert.ToInt32(avg) && mayAVG != 0) ? totalRedColor : totalColor, true);
                        excelReport.SetCellValue(junAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (junAVG < Convert.ToInt32(avg) && junAVG != 0) ? totalRedColor : totalColor, true);
                        excelReport.SetCellValue(julAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (julAVG < Convert.ToInt32(avg) && julAVG != 0) ? totalRedColor : totalColor, true);
                        excelReport.SetCellValue(augAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (augAVG < Convert.ToInt32(avg) && augAVG != 0) ? totalRedColor : totalColor, true);
                        excelReport.SetCellValue(sepAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sepAVG < Convert.ToInt32(avg) && sepAVG != 0) ? totalRedColor : totalColor, true);
                        excelReport.SetCellValue(octAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (octAVG < Convert.ToInt32(avg) && octAVG != 0) ? totalRedColor : totalColor, true);
                        excelReport.SetCellValue(novAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (novAVG < Convert.ToInt32(avg) && novAVG != 0) ? totalRedColor : totalColor, true);
                        excelReport.SetCellValue(decAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (decAVG < Convert.ToInt32(avg) && decAVG != 0) ? totalRedColor : totalColor, true);
                        excelReport.SetCellValue(totalAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, totalColor, true);
                        excelReport.SetCellValue(avg.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, totalColor, true);

                        janTtl = febTtl = marTtl = aprTtl = mayTtl = junTtl = julTtl = augTtl = sepTtl = octTtl = novTtl = decTtl = totalTtl = 0;
                        totalSalesCo = 1;
                    }
                    else if (row["Seq"].ToString() == "3" && row["Seq2"].ToString() == "3")
                    {
                        janAVG = janTtl / totalSalesHead;
                        febAVG = febTtl / totalSalesHead;
                        marAVG = marTtl / totalSalesHead;
                        aprAVG = aprTtl / totalSalesHead;
                        mayAVG = mayTtl / totalSalesHead;
                        junAVG = junTtl / totalSalesHead;
                        julAVG = julTtl / totalSalesHead;
                        augAVG = augTtl / totalSalesHead;
                        sepAVG = sepTtl / totalSalesHead;
                        octAVG = octTtl / totalSalesHead;
                        novAVG = novTtl / totalSalesHead;
                        decAVG = decTtl / totalSalesHead;
                        totalAVG = (janAVG + febAVG + marAVG + aprAVG + mayAVG + junAVG + julAVG + augAVG + sepAVG + octAVG + novAVG + decAVG);
                        decimal avg = totalAVG / salesmanMonth;

                        excelReport.SetCellValue(janAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (janAVG < Convert.ToInt32(avg) && janAVG != 0) ? totalRedColor : totalColor, true);
                        excelReport.SetCellValue(febAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (febAVG < Convert.ToInt32(avg) && febAVG != 0) ? totalRedColor : totalColor, true);
                        excelReport.SetCellValue(marAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (marAVG < Convert.ToInt32(avg) && marAVG != 0) ? totalRedColor : totalColor, true);
                        excelReport.SetCellValue(aprAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (aprAVG < Convert.ToInt32(avg) && aprAVG != 0) ? totalRedColor : totalColor, true);
                        excelReport.SetCellValue(mayAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (mayAVG < Convert.ToInt32(avg) && mayAVG != 0) ? totalRedColor : totalColor, true);
                        excelReport.SetCellValue(junAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (junAVG < Convert.ToInt32(avg) && junAVG != 0) ? totalRedColor : totalColor, true);
                        excelReport.SetCellValue(julAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (julAVG < Convert.ToInt32(avg) && julAVG != 0) ? totalRedColor : totalColor, true);
                        excelReport.SetCellValue(augAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (augAVG < Convert.ToInt32(avg) && augAVG != 0) ? totalRedColor : totalColor, true);
                        excelReport.SetCellValue(sepAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sepAVG < Convert.ToInt32(avg) && sepAVG != 0) ? totalRedColor : totalColor, true);
                        excelReport.SetCellValue(octAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (octAVG < Convert.ToInt32(avg) && octAVG != 0) ? totalRedColor : totalColor, true);
                        excelReport.SetCellValue(novAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (novAVG < Convert.ToInt32(avg) && novAVG != 0) ? totalRedColor : totalColor, true);
                        excelReport.SetCellValue(decAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (decAVG < Convert.ToInt32(avg) && decAVG != 0) ? totalRedColor : totalColor, true);
                        excelReport.SetCellValue(totalAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, totalColor, true);
                        excelReport.SetCellValue(avg.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, totalColor, true);

                        janTtl = febTtl = marTtl = aprTtl = mayTtl = junTtl = julTtl = augTtl = sepTtl = octTtl = novTtl = decTtl = totalTtl = 0;
                        totalSalesCo = 1;
                        totalSalesHead = 1;
                    }
                    else if (row["Seq"].ToString() == "4" && row["Seq2"].ToString() == "3")
                    {
                        janAVG = janTtl / totalOutlet;
                        febAVG = febTtl / totalOutlet;
                        marAVG = marTtl / totalOutlet;
                        aprAVG = aprTtl / totalOutlet;
                        mayAVG = mayTtl / totalOutlet;
                        junAVG = junTtl / totalOutlet;
                        julAVG = julTtl / totalOutlet;
                        augAVG = augTtl / totalOutlet;
                        sepAVG = sepTtl / totalOutlet;
                        octAVG = octTtl / totalOutlet;
                        novAVG = novTtl / totalOutlet;
                        decAVG = decTtl / totalOutlet;
                        totalAVG = (janAVG + febAVG + marAVG + aprAVG + mayAVG + junAVG + julAVG + augAVG + sepAVG + octAVG + novAVG + decAVG);
                        decimal avg = totalAVG / salesmanMonth;

                        excelReport.SetCellValue(janAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (janAVG < Convert.ToInt32(avg) && janAVG != 0) ? totalRedColor : totalColor, true);
                        excelReport.SetCellValue(febAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (febAVG < Convert.ToInt32(avg) && febAVG != 0) ? totalRedColor : totalColor, true);
                        excelReport.SetCellValue(marAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (marAVG < Convert.ToInt32(avg) && marAVG != 0) ? totalRedColor : totalColor, true);
                        excelReport.SetCellValue(aprAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (aprAVG < Convert.ToInt32(avg) && aprAVG != 0) ? totalRedColor : totalColor, true);
                        excelReport.SetCellValue(mayAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (mayAVG < Convert.ToInt32(avg) && mayAVG != 0) ? totalRedColor : totalColor, true);
                        excelReport.SetCellValue(junAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (junAVG < Convert.ToInt32(avg) && junAVG != 0) ? totalRedColor : totalColor, true);
                        excelReport.SetCellValue(julAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (julAVG < Convert.ToInt32(avg) && julAVG != 0) ? totalRedColor : totalColor, true);
                        excelReport.SetCellValue(augAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (augAVG < Convert.ToInt32(avg) && augAVG != 0) ? totalRedColor : totalColor, true);
                        excelReport.SetCellValue(sepAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sepAVG < Convert.ToInt32(avg) && sepAVG != 0) ? totalRedColor : totalColor, true);
                        excelReport.SetCellValue(octAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (octAVG < Convert.ToInt32(avg) && octAVG != 0) ? totalRedColor : totalColor, true);
                        excelReport.SetCellValue(novAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (novAVG < Convert.ToInt32(avg) && novAVG != 0) ? totalRedColor : totalColor, true);
                        excelReport.SetCellValue(decAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (decAVG < Convert.ToInt32(avg) && decAVG != 0) ? totalRedColor : totalColor, true);
                        excelReport.SetCellValue(totalAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, totalColor, true);
                        excelReport.SetCellValue(avg.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, totalColor, true);

                        janTtl = febTtl = marTtl = aprTtl = mayTtl = junTtl = julTtl = augTtl = sepTtl = octTtl = novTtl = decTtl = totalTtl = 0;
                        totalSalesCo = 1;
                        totalSalesHead = 1;
                        totalOutlet = 1;
                    }
                    else
                    {
                        decimal avg = Convert.ToDecimal(row["Total"].ToString()) / salesmanMonth;

                        if (row["Seq"].ToString() == "1" && row["Seq2"].ToString() == "3")
                        {
                            excelReport.SetCellValue(Convert.ToDecimal(row["Jan"].ToString()).ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (Convert.ToDecimal(row["Jan"].ToString()) < Convert.ToInt32(avg) && Convert.ToDecimal(row["Jan"].ToString()) != 0) ? totalRedColor : totalColor, true);
                            excelReport.SetCellValue(Convert.ToDecimal(row["Feb"].ToString()).ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (Convert.ToDecimal(row["Feb"].ToString()) < Convert.ToInt32(avg) && Convert.ToDecimal(row["Feb"].ToString()) != 0) ? totalRedColor : totalColor, true);
                            excelReport.SetCellValue(Convert.ToDecimal(row["Mar"].ToString()).ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (Convert.ToDecimal(row["Mar"].ToString()) < Convert.ToInt32(avg) && Convert.ToDecimal(row["Mar"].ToString()) != 0) ? totalRedColor : totalColor, true);
                            excelReport.SetCellValue(Convert.ToDecimal(row["Apr"].ToString()).ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (Convert.ToDecimal(row["Apr"].ToString()) < Convert.ToInt32(avg) && Convert.ToDecimal(row["Apr"].ToString()) != 0) ? totalRedColor : totalColor, true);
                            excelReport.SetCellValue(Convert.ToDecimal(row["May"].ToString()).ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (Convert.ToDecimal(row["May"].ToString()) < Convert.ToInt32(avg) && Convert.ToDecimal(row["May"].ToString()) != 0) ? totalRedColor : totalColor, true);
                            excelReport.SetCellValue(Convert.ToDecimal(row["Jun"].ToString()).ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (Convert.ToDecimal(row["Jun"].ToString()) < Convert.ToInt32(avg) && Convert.ToDecimal(row["Jun"].ToString()) != 0) ? totalRedColor : totalColor, true);
                            excelReport.SetCellValue(Convert.ToDecimal(row["Jul"].ToString()).ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (Convert.ToDecimal(row["Jul"].ToString()) < Convert.ToInt32(avg) && Convert.ToDecimal(row["Jul"].ToString()) != 0) ? totalRedColor : totalColor, true);
                            excelReport.SetCellValue(Convert.ToDecimal(row["Aug"].ToString()).ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (Convert.ToDecimal(row["Aug"].ToString()) < Convert.ToInt32(avg) && Convert.ToDecimal(row["Aug"].ToString()) != 0) ? totalRedColor : totalColor, true);
                            excelReport.SetCellValue(Convert.ToDecimal(row["Sep"].ToString()).ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (Convert.ToDecimal(row["Sep"].ToString()) < Convert.ToInt32(avg) && Convert.ToDecimal(row["Sep"].ToString()) != 0) ? totalRedColor : totalColor, true);
                            excelReport.SetCellValue(Convert.ToDecimal(row["Oct"].ToString()).ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (Convert.ToDecimal(row["Oct"].ToString()) < Convert.ToInt32(avg) && Convert.ToDecimal(row["Oct"].ToString()) != 0) ? totalRedColor : totalColor, true);
                            excelReport.SetCellValue(Convert.ToDecimal(row["Nov"].ToString()).ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (Convert.ToDecimal(row["Nov"].ToString()) < Convert.ToInt32(avg) && Convert.ToDecimal(row["Nov"].ToString()) != 0) ? totalRedColor : totalColor, true);
                            excelReport.SetCellValue(Convert.ToDecimal(row["Dec"].ToString()).ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (Convert.ToDecimal(row["Dec"].ToString()) < Convert.ToInt32(avg) && Convert.ToDecimal(row["Dec"].ToString()) != 0) ? totalRedColor : totalColor, true);
                            excelReport.SetCellValue(Convert.ToDecimal(row["Total"].ToString()).ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, totalColor, true);
                            excelReport.SetCellValue(avg.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, totalColor, true);

                        }
                        else
                        {
                            excelReport.SetCellValue(Convert.ToDecimal(row["Jan"].ToString()).ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (Convert.ToDecimal(row["Jan"].ToString()) < Convert.ToInt32(avg) && Convert.ToDecimal(row["Jan"].ToString()) != 0) ? totalRedColor : totalColor, true);
                            excelReport.SetCellValue(Convert.ToDecimal(row["Feb"].ToString()).ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (Convert.ToDecimal(row["Feb"].ToString()) < Convert.ToInt32(avg) && Convert.ToDecimal(row["Feb"].ToString()) != 0) ? totalRedColor : totalColor, true);
                            excelReport.SetCellValue(Convert.ToDecimal(row["Mar"].ToString()).ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (Convert.ToDecimal(row["Mar"].ToString()) < Convert.ToInt32(avg) && Convert.ToDecimal(row["Mar"].ToString()) != 0) ? totalRedColor : totalColor, true);
                            excelReport.SetCellValue(Convert.ToDecimal(row["Apr"].ToString()).ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (Convert.ToDecimal(row["Apr"].ToString()) < Convert.ToInt32(avg) && Convert.ToDecimal(row["Apr"].ToString()) != 0) ? totalRedColor : totalColor, true);
                            excelReport.SetCellValue(Convert.ToDecimal(row["May"].ToString()).ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (Convert.ToDecimal(row["May"].ToString()) < Convert.ToInt32(avg) && Convert.ToDecimal(row["May"].ToString()) != 0) ? totalRedColor : totalColor, true);
                            excelReport.SetCellValue(Convert.ToDecimal(row["Jun"].ToString()).ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (Convert.ToDecimal(row["Jun"].ToString()) < Convert.ToInt32(avg) && Convert.ToDecimal(row["Jun"].ToString()) != 0) ? totalRedColor : totalColor, true);
                            excelReport.SetCellValue(Convert.ToDecimal(row["Jul"].ToString()).ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (Convert.ToDecimal(row["Jul"].ToString()) < Convert.ToInt32(avg) && Convert.ToDecimal(row["Jul"].ToString()) != 0) ? totalRedColor : totalColor, true);
                            excelReport.SetCellValue(Convert.ToDecimal(row["Aug"].ToString()).ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (Convert.ToDecimal(row["Aug"].ToString()) < Convert.ToInt32(avg) && Convert.ToDecimal(row["Aug"].ToString()) != 0) ? totalRedColor : totalColor, true);
                            excelReport.SetCellValue(Convert.ToDecimal(row["Sep"].ToString()).ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (Convert.ToDecimal(row["Sep"].ToString()) < Convert.ToInt32(avg) && Convert.ToDecimal(row["Sep"].ToString()) != 0) ? totalRedColor : totalColor, true);
                            excelReport.SetCellValue(Convert.ToDecimal(row["Oct"].ToString()).ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (Convert.ToDecimal(row["Oct"].ToString()) < Convert.ToInt32(avg) && Convert.ToDecimal(row["Oct"].ToString()) != 0) ? totalRedColor : totalColor, true);
                            excelReport.SetCellValue(Convert.ToDecimal(row["Nov"].ToString()).ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (Convert.ToDecimal(row["Nov"].ToString()) < Convert.ToInt32(avg) && Convert.ToDecimal(row["Nov"].ToString()) != 0) ? totalRedColor : totalColor, true);
                            excelReport.SetCellValue(Convert.ToDecimal(row["Dec"].ToString()).ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (Convert.ToDecimal(row["Dec"].ToString()) < Convert.ToInt32(avg) && Convert.ToDecimal(row["Dec"].ToString()) != 0) ? totalRedColor : totalColor, true);
                            excelReport.SetCellValue(Convert.ToDecimal(row["Total"].ToString()).ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, totalColor, true);
                            excelReport.SetCellValue(avg.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, totalColor, true);
                        }
                    }
                }
                #endregion

                #region SUM TOTAL
                if (parameter[10] == "1")
                {
                    jan += Convert.ToDecimal(row["Jan"].ToString());
                    feb += Convert.ToDecimal(row["Feb"].ToString());
                    mar += Convert.ToDecimal(row["Mar"].ToString());
                    apr += Convert.ToDecimal(row["Apr"].ToString());
                    may += Convert.ToDecimal(row["May"].ToString());
                    jun += Convert.ToDecimal(row["Jun"].ToString());
                    jul += Convert.ToDecimal(row["Jul"].ToString());
                    aug += Convert.ToDecimal(row["Aug"].ToString());
                    sep += Convert.ToDecimal(row["Sep"].ToString());
                    oct += Convert.ToDecimal(row["Oct"].ToString());
                    nov += Convert.ToDecimal(row["Nov"].ToString());
                    dec += Convert.ToDecimal(row["Dec"].ToString());
                    total = jan + feb + mar + apr + may + jun + jul + aug + sep + oct + nov + dec;

                    janGT += Convert.ToDecimal(row["Jan"].ToString());
                    febGT += Convert.ToDecimal(row["Feb"].ToString());
                    marGT += Convert.ToDecimal(row["Mar"].ToString());
                    aprGT += Convert.ToDecimal(row["Apr"].ToString());
                    mayGT += Convert.ToDecimal(row["May"].ToString());
                    junGT += Convert.ToDecimal(row["Jun"].ToString());
                    julGT += Convert.ToDecimal(row["Jul"].ToString());
                    augGT += Convert.ToDecimal(row["Aug"].ToString());
                    sepGT += Convert.ToDecimal(row["Sep"].ToString());
                    octGT += Convert.ToDecimal(row["Oct"].ToString());
                    novGT += Convert.ToDecimal(row["Nov"].ToString());
                    decGT += Convert.ToDecimal(row["Dec"].ToString());
                    totalGT = janGT + febGT + marGT + aprGT + mayGT + junGT + julGT + augGT + sepGT + octGT + novGT + decGT;
                    if (row["Seq"].ToString() == "3" && row["Seq2"].ToString() == "2")
                    {
                        janJabodetabek += Convert.ToDecimal(row["Jan"].ToString());
                        febJabodetabek += Convert.ToDecimal(row["Feb"].ToString());
                        marJabodetabek += Convert.ToDecimal(row["Mar"].ToString());
                        aprJabodetabek += Convert.ToDecimal(row["Apr"].ToString());
                        mayJabodetabek += Convert.ToDecimal(row["May"].ToString());
                        junJabodetabek += Convert.ToDecimal(row["Jun"].ToString());
                        julJabodetabek += Convert.ToDecimal(row["Jul"].ToString());
                        augJabodetabek += Convert.ToDecimal(row["Aug"].ToString());
                        sepJabodetabek += Convert.ToDecimal(row["Sep"].ToString());
                        octJabodetabek += Convert.ToDecimal(row["Oct"].ToString());
                        novJabodetabek += Convert.ToDecimal(row["Nov"].ToString());
                        decJabodetabek += Convert.ToDecimal(row["Dec"].ToString());
                        totalJabodetabek = janJabodetabek + febJabodetabek + marJabodetabek + aprJabodetabek + mayJabodetabek + junJabodetabek + julJabodetabek + augJabodetabek + sepJabodetabek + octJabodetabek + novJabodetabek + decJabodetabek;

                    }

                    if (row["Seq"].ToString() == "4" && row["Seq2"].ToString() == "2")
                    {
                        janArea += Convert.ToDecimal(row["Jan"].ToString());
                        febArea += Convert.ToDecimal(row["Feb"].ToString());
                        marArea += Convert.ToDecimal(row["Mar"].ToString());
                        aprArea += Convert.ToDecimal(row["Apr"].ToString());
                        mayArea += Convert.ToDecimal(row["May"].ToString());
                        junArea += Convert.ToDecimal(row["Jun"].ToString());
                        julArea += Convert.ToDecimal(row["Jul"].ToString());
                        augArea += Convert.ToDecimal(row["Aug"].ToString());
                        sepArea += Convert.ToDecimal(row["Sep"].ToString());
                        octArea += Convert.ToDecimal(row["Oct"].ToString());
                        novArea += Convert.ToDecimal(row["Nov"].ToString());
                        decArea += Convert.ToDecimal(row["Dec"].ToString());
                        totalArea = janArea + febArea + marArea + aprArea + mayArea + junArea + julArea + augArea + sepArea + octArea + novArea + decArea;

                        janGTII += Convert.ToDecimal(row["Jan"].ToString());
                        febGTII += Convert.ToDecimal(row["Feb"].ToString());
                        marGTII += Convert.ToDecimal(row["Mar"].ToString());
                        aprGTII += Convert.ToDecimal(row["Apr"].ToString());
                        mayGTII += Convert.ToDecimal(row["May"].ToString());
                        junGTII += Convert.ToDecimal(row["Jun"].ToString());
                        julGTII += Convert.ToDecimal(row["Jul"].ToString());
                        augGTII += Convert.ToDecimal(row["Aug"].ToString());
                        sepGTII += Convert.ToDecimal(row["Sep"].ToString());
                        octGTII += Convert.ToDecimal(row["Oct"].ToString());
                        novGTII += Convert.ToDecimal(row["Nov"].ToString());
                        decGTII += Convert.ToDecimal(row["Dec"].ToString());
                        totalGTII = janGTII + febGTII + marGTII + aprGTII + mayGTII + junGTII + julGTII + augGTII + sepGTII + octGTII + novGTII + decGTII;
                    }
                }
                else
                {
                    if (row["Seq"].ToString() == "1" && row["Seq2"].ToString() == "1")
                    {
                        jan += Convert.ToDecimal(row["Jan"].ToString());
                        feb += Convert.ToDecimal(row["Feb"].ToString());
                        mar += Convert.ToDecimal(row["Mar"].ToString());
                        apr += Convert.ToDecimal(row["Apr"].ToString());
                        may += Convert.ToDecimal(row["May"].ToString());
                        jun += Convert.ToDecimal(row["Jun"].ToString());
                        jul += Convert.ToDecimal(row["Jul"].ToString());
                        aug += Convert.ToDecimal(row["Aug"].ToString());
                        sep += Convert.ToDecimal(row["Sep"].ToString());
                        oct += Convert.ToDecimal(row["Oct"].ToString());
                        nov += Convert.ToDecimal(row["Nov"].ToString());
                        dec += Convert.ToDecimal(row["Dec"].ToString());
                        total = jan + feb + mar + apr + may + jun + jul + aug + sep + oct + nov + dec;

                        janJabodetabek += Convert.ToDecimal(row["Jan"].ToString());
                        febJabodetabek += Convert.ToDecimal(row["Feb"].ToString());
                        marJabodetabek += Convert.ToDecimal(row["Mar"].ToString());
                        aprJabodetabek += Convert.ToDecimal(row["Apr"].ToString());
                        mayJabodetabek += Convert.ToDecimal(row["May"].ToString());
                        junJabodetabek += Convert.ToDecimal(row["Jun"].ToString());
                        julJabodetabek += Convert.ToDecimal(row["Jul"].ToString());
                        augJabodetabek += Convert.ToDecimal(row["Aug"].ToString());
                        sepJabodetabek += Convert.ToDecimal(row["Sep"].ToString());
                        octJabodetabek += Convert.ToDecimal(row["Oct"].ToString());
                        novJabodetabek += Convert.ToDecimal(row["Nov"].ToString());
                        decJabodetabek += Convert.ToDecimal(row["Dec"].ToString());
                        totalJabodetabek = janJabodetabek + febJabodetabek + marJabodetabek + aprJabodetabek + mayJabodetabek + junJabodetabek + julJabodetabek + augJabodetabek + sepJabodetabek + octJabodetabek + novJabodetabek + decJabodetabek;


                        janGT += Convert.ToDecimal(row["Jan"].ToString());
                        febGT += Convert.ToDecimal(row["Feb"].ToString());
                        marGT += Convert.ToDecimal(row["Mar"].ToString());
                        aprGT += Convert.ToDecimal(row["Apr"].ToString());
                        mayGT += Convert.ToDecimal(row["May"].ToString());
                        junGT += Convert.ToDecimal(row["Jun"].ToString());
                        julGT += Convert.ToDecimal(row["Jul"].ToString());
                        augGT += Convert.ToDecimal(row["Aug"].ToString());
                        sepGT += Convert.ToDecimal(row["Sep"].ToString());
                        octGT += Convert.ToDecimal(row["Oct"].ToString());
                        novGT += Convert.ToDecimal(row["Nov"].ToString());
                        decGT += Convert.ToDecimal(row["Dec"].ToString());
                        totalGT = janGT + febGT + marGT + aprGT + mayGT + junGT + julGT + augGT + sepGT + octGT + novGT + decGT;
                    }

                    if ((row["Seq"].ToString() == "2" && row["Seq2"].ToString() == "1") ||
                        (row["Seq"].ToString() == "3" && row["Seq2"].ToString() == "2") ||
                        (row["Seq"].ToString() == "4" && row["Seq2"].ToString() == "2"))
                    {
                        janTtl += Convert.ToDecimal(row["Jan"].ToString());
                        febTtl += Convert.ToDecimal(row["Feb"].ToString());
                        marTtl += Convert.ToDecimal(row["Mar"].ToString());
                        aprTtl += Convert.ToDecimal(row["Apr"].ToString());
                        mayTtl += Convert.ToDecimal(row["May"].ToString());
                        junTtl += Convert.ToDecimal(row["Jun"].ToString());
                        julTtl += Convert.ToDecimal(row["Jul"].ToString());
                        augTtl += Convert.ToDecimal(row["Aug"].ToString());
                        sepTtl += Convert.ToDecimal(row["Sep"].ToString());
                        octTtl += Convert.ToDecimal(row["Oct"].ToString());
                        novTtl += Convert.ToDecimal(row["Nov"].ToString());
                        decTtl += Convert.ToDecimal(row["Dec"].ToString());
                        totalTtl = janTtl + febTtl + marTtl + aprTtl + mayTtl + junTtl + julTtl + augTtl + sepTtl + octTtl + novTtl + decTtl;
                    }

                    if (row["Seq"].ToString() == "4" && row["Seq2"].ToString() == "2")
                    {
                        janArea += Convert.ToDecimal(row["Jan"].ToString());
                        febArea += Convert.ToDecimal(row["Feb"].ToString());
                        marArea += Convert.ToDecimal(row["Mar"].ToString());
                        aprArea += Convert.ToDecimal(row["Apr"].ToString());
                        mayArea += Convert.ToDecimal(row["May"].ToString());
                        junArea += Convert.ToDecimal(row["Jun"].ToString());
                        julArea += Convert.ToDecimal(row["Jul"].ToString());
                        augArea += Convert.ToDecimal(row["Aug"].ToString());
                        sepArea += Convert.ToDecimal(row["Sep"].ToString());
                        octArea += Convert.ToDecimal(row["Oct"].ToString());
                        novArea += Convert.ToDecimal(row["Nov"].ToString());
                        decArea += Convert.ToDecimal(row["Dec"].ToString());
                        totalArea = janArea + febArea + marArea + aprArea + mayArea + junArea + julArea + augArea + sepArea + octArea + novArea + decArea;

                        janGTII += Convert.ToDecimal(row["Jan"].ToString());
                        febGTII += Convert.ToDecimal(row["Feb"].ToString());
                        marGTII += Convert.ToDecimal(row["Mar"].ToString());
                        aprGTII += Convert.ToDecimal(row["Apr"].ToString());
                        mayGTII += Convert.ToDecimal(row["May"].ToString());
                        junGTII += Convert.ToDecimal(row["Jun"].ToString());
                        julGTII += Convert.ToDecimal(row["Jul"].ToString());
                        augGTII += Convert.ToDecimal(row["Aug"].ToString());
                        sepGTII += Convert.ToDecimal(row["Sep"].ToString());
                        octGTII += Convert.ToDecimal(row["Oct"].ToString());
                        novGTII += Convert.ToDecimal(row["Nov"].ToString());
                        decGTII += Convert.ToDecimal(row["Dec"].ToString());
                        totalGTII = janGTII + febGTII + marGTII + aprGTII + mayGTII + junGTII + julGTII + augGTII + sepGTII + octGTII + novGTII + decGTII;

                    }
                }
                #endregion

                if (row["Seq"].ToString() == "1")
                {
                    averageMonth = averageMonth < salesmanMonth ? salesmanMonth : averageMonth;
                    averageMonthJBDTK = averageMonth < salesmanMonth ? salesmanMonth : averageMonth;
                    averageYear = averageYear < salesmanMonth ? salesmanMonth : averageYear;
                }
                seq2 = row["Seq2"].ToString();
                totalRowBranchHead++;
                totalRowColor++;
                totalRowDealer++;
                totalRowModel++;
                totalRowSalesCo++;
                totalRowSalesHead++;
                totalRowSalsman++;
                totalRow++;
                rowIndex++;
            }
            CreateFooter(excelReport, rowIndex, indentTitleDesc, true, true);
            #endregion
            excelReport.CloseExcelFileWriter();
        }
        #endregion

        public void CreateHeader(ExcelFileWriter excelReport, string title, int rowIndex, int indentTitleDesc, string year)
        {
            excelReport.SettingColumnWidth(1); //dealer
            excelReport.SettingColumnWidth(2); // Outlet
            excelReport.SettingColumnWidth(2); // SalesHead
            excelReport.SettingColumnWidth(2); // SalesCoordinator
            if (parameter[9] == "1")
            {
                excelReport.SettingColumnWidth(3); // Salesman
                excelReport.SettingColumnWidth(1); // Grade
            }
            if (parameter[10] == "1")
            {
                excelReport.SettingColumnWidth(3); // marketModel
            }
            if (parameter[11] == "1")
            {
                excelReport.SettingColumnWidth(2); // Model Color
            }

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

            excelReport.SetCellValue("Dealer", 6, 0, 2, dealerWidth, ExcelCellStyle.LeftBorderedBold);
            excelReport.SetCellValue("Outlet", 6, 0 + dealerWidth, 2, outletWidth, ExcelCellStyle.LeftBorderedBold);
            excelReport.SetCellValue("Sales Head", 6, 0 + dealerWidth + outletWidth, 2, salesHeadWidth, ExcelCellStyle.LeftBorderedBold);
            excelReport.SetCellValue("Sales Coordinator", 6, 0 + dealerWidth + outletWidth + salesHeadWidth, 2, salesCoordinatorWidth, ExcelCellStyle.LeftBorderedBold);

            if (parameter[9] == "1")
            {
                excelReport.SetCellValue("Wiraniaga", 6, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth, 2, salesManWidth, ExcelCellStyle.LeftBorderedBold);
                excelReport.SetCellValue("Grade", 6, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth, 2, gradeWidth, ExcelCellStyle.LeftBorderedBold);
            }
            if (parameter[10] == "1")
            {
                excelReport.SetCellValue("Type", 6, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 2, modelWidth, ExcelCellStyle.LeftBorderedBold);
            }
            if (parameter[11] == "1")
            {
                excelReport.SetCellValue("Color", 6, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + modelWidth + gradeWidth, 2, colorWidth, ExcelCellStyle.LeftBorderedBold);
            }
            excelReport.SetCellValue("Bulan", 6, 0 + totalDetailWidth, 1, 12, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Total", 6, 12 + totalDetailWidth, 2, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("AVG", 6, 13 + totalDetailWidth, 2, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Jan", 7, 0 + totalDetailWidth, 1, 1, ExcelCellStyle.RightBorderedBold);
            excelReport.SetCellValue("Feb", 7, 1 + totalDetailWidth, 1, 1, ExcelCellStyle.RightBorderedBold);
            excelReport.SetCellValue("Mar", 7, 2 + totalDetailWidth, 1, 1, ExcelCellStyle.RightBorderedBold);
            excelReport.SetCellValue("Apr", 7, 3 + totalDetailWidth, 1, 1, ExcelCellStyle.RightBorderedBold);
            excelReport.SetCellValue("May", 7, 4 + totalDetailWidth, 1, 1, ExcelCellStyle.RightBorderedBold);
            excelReport.SetCellValue("Jun", 7, 5 + totalDetailWidth, 1, 1, ExcelCellStyle.RightBorderedBold);
            excelReport.SetCellValue("Jul", 7, 6 + totalDetailWidth, 1, 1, ExcelCellStyle.RightBorderedBold);
            excelReport.SetCellValue("Aug", 7, 7 + totalDetailWidth, 1, 1, ExcelCellStyle.RightBorderedBold);
            excelReport.SetCellValue("Sep", 7, 8 + totalDetailWidth, 1, 1, ExcelCellStyle.RightBorderedBold);
            excelReport.SetCellValue("Oct", 7, 9 + totalDetailWidth, 1, 1, ExcelCellStyle.RightBorderedBold);
            excelReport.SetCellValue("Nov", 7, 10 + totalDetailWidth, 1, 1, ExcelCellStyle.RightBorderedBold);
            excelReport.SetCellValue("Dec", 7, 11 + totalDetailWidth, 1, 1, ExcelCellStyle.RightBorderedBold);

            totalRow = totalRowBranchHead = totalRowColor = totalRowDealer = totalRowModel = totalRowSalesCo = totalRowSalesHead = totalRowSalsman = 0;
            rowIndex = 0;

            totalSalesCo = 1;
            totalSalesHead = 1;
            totalOutlet = 1;
            totalDealer = 1;
            totalDealerGT = 1;

            excelReport.RestorePoint(8, RestorePoint.CompanyCodeRestorePoint.ToString());
            excelReport.RestorePoint(8, RestorePoint.OutletRestorePoint.ToString());
            excelReport.RestorePoint(8, RestorePoint.SalesHeadRestorePoint.ToString());

        }

        public void CreateFooter(ExcelFileWriter excelReport, int rowIndex, int indentTitleDesc, bool boolModel, bool boolArea)
        {
            #region Total
            if (parameter[9] == "1" && parameter[10] == "1")
            {
                decimal average = total / averageMonth;
                if (boolModel)
                {
                    excelReport.ReplaceRestorePoint(RestorePoint.MarketModelRestorePoint.ToString(), marketModel.ToString(), 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, totalRowColor, modelWidth, ExcelCellStyle.LeftBorderedStandard);
                    //Masukin total Model Catagory
                    excelReport.SetCellValue("Total " + modelCatagory.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth + colorWidth, ExcelCellStyle.BlueTotal);
                    excelReport.SetCellValue(jan.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (jan < Convert.ToInt32(average) && jan != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                    excelReport.SetCellValue(feb.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (feb < Convert.ToInt32(average) && feb != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                    excelReport.SetCellValue(mar.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (mar < Convert.ToInt32(average) && mar != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                    excelReport.SetCellValue(apr.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (apr < Convert.ToInt32(average) && apr != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                    excelReport.SetCellValue(may.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (may < Convert.ToInt32(average) && may != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                    excelReport.SetCellValue(jun.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (jun < Convert.ToInt32(average) && jun != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                    excelReport.SetCellValue(jul.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (jul < Convert.ToInt32(average) && jul != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                    excelReport.SetCellValue(aug.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (aug < Convert.ToInt32(average) && aug != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                    excelReport.SetCellValue(sep.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sep < Convert.ToInt32(average) && sep != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                    excelReport.SetCellValue(oct.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (oct < Convert.ToInt32(average) && oct != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                    excelReport.SetCellValue(nov.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (nov < Convert.ToInt32(average) && nov != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                    excelReport.SetCellValue(dec.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (dec < Convert.ToInt32(average) && dec != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                    excelReport.SetCellValue(total.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                    excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);

                    totalSubTotal++;
                    totalRowBranchHead++;
                    totalRowColor++;
                    totalRowDealer++;
                    totalRowModel++;
                    totalRowSalesCo++;//<-- Untuk masukkin total nilai sub total tiap Model Catagory
                    totalRowSalesHead++;
                    totalRowSalsman++;
                    rowIndex++;

                    average = totalGT / averageMonth;
                    //Masukkin sub total model category
                    excelReport.SetCellValue("SubTotal", 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth + colorWidth, ExcelCellStyle.PurpleTotal);
                    excelReport.SetCellValue(janGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (janGT < Convert.ToInt32(average) && janGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(febGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (febGT < Convert.ToInt32(average) && febGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(marGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (marGT < Convert.ToInt32(average) && marGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(aprGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (aprGT < Convert.ToInt32(average) && aprGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(mayGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (mayGT < Convert.ToInt32(average) && mayGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(junGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (junGT < Convert.ToInt32(average) && junGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(julGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (julGT < Convert.ToInt32(average) && julGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(augGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (augGT < Convert.ToInt32(average) && augGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(sepGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sepGT < Convert.ToInt32(average) && sepGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(octGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (octGT < Convert.ToInt32(average) && octGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(novGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (novGT < Convert.ToInt32(average) && novGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(decGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (decGT < Convert.ToInt32(average) && decGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(totalGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                    totalSubTotal++;
                    totalRowBranchHead++;
                    totalRowColor++;
                    totalRowDealer++;
                    totalRowModel++;
                    totalRowSalesCo++;//<-- Untuk masukkin total nilai sub total tiap Model Catagory
                    totalRowSalesHead++;
                    totalRowSalsman++;
                    rowIndex++;

                    jan = feb = mar = apr = may = jun = jul = aug = sep = oct = nov = dec = total = 0;


                    excelReport.ReplaceRestorePoint(RestorePoint.SalesmanRestorePoint.ToString(), "AVG", 0 + dealerWidth, totalRowSalsman, outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, ExcelCellStyle.LeftBorderedStandardWrap);
                    excelReport.RemoveRestorePoint(RestorePoint.GradeRestorePoint.ToString());
                    excelReport.ReplaceRestorePoint(RestorePoint.CompanyCodeRestorePoint.ToString(), dealer.ToString(), 0, totalRowDealer, dealerWidth, ExcelCellStyle.LeftBorderedStandardWrap);
                }

                if (boolArea)
                {
                    if (parameter[5].ToString() == "ALL")
                    {
                        average = totalArea / averageMonth;

                        //Masukin total Model Catagory
                        excelReport.SetCellValue("Total " + area.ToString(), 8 + rowIndex, 0, 1, dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + modelWidth + colorWidth, ExcelCellStyle.PurpleTotal);
                        excelReport.SetCellValue(janArea.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (janArea < Convert.ToInt32(average) && janArea != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(febArea.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (febArea < Convert.ToInt32(average) && febArea != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(marArea.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (marArea < Convert.ToInt32(average) && marArea != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(aprArea.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (aprArea < Convert.ToInt32(average) && aprArea != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(mayArea.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (mayArea < Convert.ToInt32(average) && mayArea != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(junArea.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (junArea < Convert.ToInt32(average) && junArea != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(julArea.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (julArea < Convert.ToInt32(average) && julArea != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(augArea.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (augArea < Convert.ToInt32(average) && augArea != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(sepArea.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sepArea < Convert.ToInt32(average) && sepArea != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(octArea.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (octArea < Convert.ToInt32(average) && octArea != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(novArea.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (novArea < Convert.ToInt32(average) && novArea != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(decArea.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (decArea < Convert.ToInt32(average) && decArea != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(totalArea.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                        excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);

                        rowIndex++;

                        janAVG = janArea / totalDealer;
                        febAVG = febArea / totalDealer;
                        marAVG = marArea / totalDealer;
                        aprAVG = aprArea / totalDealer;
                        mayAVG = mayArea / totalDealer;
                        junAVG = junArea / totalDealer;
                        julAVG = julArea / totalDealer;
                        augAVG = augArea / totalDealer;
                        sepAVG = sepArea / totalDealer;
                        octAVG = octArea / totalDealer;
                        novAVG = novArea / totalDealer;
                        decAVG = decArea / totalDealer;
                        totalAVG = (janAVG + febAVG + marAVG + aprAVG + mayAVG + junAVG + julAVG + augAVG + sepAVG + octAVG + novAVG + decAVG);
                        average = totalAVG / averageMonth;

                        excelReport.SetCellValue("AVG " + area.ToString(), 8 + rowIndex, 0, 1, dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + modelWidth + colorWidth, ExcelCellStyle.BrownTotal);
                        excelReport.SetCellValue(janAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (janAVG < Convert.ToInt32(totalAVG) && janAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                        excelReport.SetCellValue(febAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (febAVG < Convert.ToInt32(totalAVG) && febAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                        excelReport.SetCellValue(marAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (marAVG < Convert.ToInt32(totalAVG) && marAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                        excelReport.SetCellValue(aprAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (aprAVG < Convert.ToInt32(totalAVG) && aprAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                        excelReport.SetCellValue(mayAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (mayAVG < Convert.ToInt32(totalAVG) && mayAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                        excelReport.SetCellValue(junAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (junAVG < Convert.ToInt32(totalAVG) && junAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                        excelReport.SetCellValue(julAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (julAVG < Convert.ToInt32(totalAVG) && julAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                        excelReport.SetCellValue(augAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (augAVG < Convert.ToInt32(totalAVG) && augAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                        excelReport.SetCellValue(sepAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sepAVG < Convert.ToInt32(totalAVG) && sepAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                        excelReport.SetCellValue(octAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (octAVG < Convert.ToInt32(totalAVG) && octAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                        excelReport.SetCellValue(novAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (novAVG < Convert.ToInt32(totalAVG) && novAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                        excelReport.SetCellValue(decAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (decAVG < Convert.ToInt32(totalAVG) && decAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                        excelReport.SetCellValue(totalAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.BrownTotalNumber, true);
                        excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.BrownTotalNumber, true);
                        rowIndex++;
                    }
                }
                if (parameter[3].ToString() == "ALL")
                {
                    average = totalGTII / averageYear;
                    excelReport.SetCellValue("TOTAL NATIONAL", 8 + rowIndex, 0, 1, dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + modelWidth + colorWidth, ExcelCellStyle.GreenTotal);
                    excelReport.SetCellValue(janGTII.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (janGTII < Convert.ToInt32(average) && janGTII != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                    excelReport.SetCellValue(febGTII.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (febGTII < Convert.ToInt32(average) && febGTII != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                    excelReport.SetCellValue(marGTII.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (marGTII < Convert.ToInt32(average) && marGTII != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                    excelReport.SetCellValue(aprGTII.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (aprGTII < Convert.ToInt32(average) && aprGTII != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                    excelReport.SetCellValue(mayGTII.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (mayGTII < Convert.ToInt32(average) && mayGTII != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                    excelReport.SetCellValue(junGTII.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (junGTII < Convert.ToInt32(average) && junGTII != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                    excelReport.SetCellValue(julGTII.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (julGTII < Convert.ToInt32(average) && julGTII != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                    excelReport.SetCellValue(augGTII.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (augGTII < Convert.ToInt32(average) && augGTII != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                    excelReport.SetCellValue(sepGTII.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sepGTII < Convert.ToInt32(average) && sepGTII != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                    excelReport.SetCellValue(octGTII.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (octGTII < Convert.ToInt32(average) && octGTII != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                    excelReport.SetCellValue(novGTII.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (novGTII < Convert.ToInt32(average) && novGTII != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                    excelReport.SetCellValue(decGTII.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (decGTII < Convert.ToInt32(average) && decGTII != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                    excelReport.SetCellValue(totalGTII.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.GreenTotalNumber, true);
                    excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.GreenTotalNumber, true);
                    rowIndex++;

                    janAVG = janGTII / totalDealerGT;
                    febAVG = febGTII / totalDealerGT;
                    marAVG = marGTII / totalDealerGT;
                    aprAVG = aprGTII / totalDealerGT;
                    mayAVG = mayGTII / totalDealerGT;
                    junAVG = junGTII / totalDealerGT;
                    julAVG = julGTII / totalDealerGT;
                    augAVG = augGTII / totalDealerGT;
                    sepAVG = sepGTII / totalDealerGT;
                    octAVG = octGTII / totalDealerGT;
                    novAVG = novGTII / totalDealerGT;
                    decAVG = decGTII / totalDealerGT;
                    totalAVG = (janAVG + febAVG + marAVG + aprAVG + mayAVG + junAVG + julAVG + augAVG + sepAVG + octAVG + novAVG + decAVG);
                    average = totalAVG / averageYear;

                    excelReport.SetCellValue("AVG NATIONAL", 8 + rowIndex, 0, 1, dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + modelWidth + colorWidth, ExcelCellStyle.BrownTotal);
                    excelReport.SetCellValue(janAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (janAVG < Convert.ToInt32(totalAVG) && janAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(febAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (febAVG < Convert.ToInt32(totalAVG) && febAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(marAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (marAVG < Convert.ToInt32(totalAVG) && marAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(aprAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (aprAVG < Convert.ToInt32(totalAVG) && aprAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(mayAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (mayAVG < Convert.ToInt32(totalAVG) && mayAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(junAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (junAVG < Convert.ToInt32(totalAVG) && junAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(julAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (julAVG < Convert.ToInt32(totalAVG) && julAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(augAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (augAVG < Convert.ToInt32(totalAVG) && augAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(sepAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sepAVG < Convert.ToInt32(totalAVG) && sepAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(octAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (octAVG < Convert.ToInt32(totalAVG) && octAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(novAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (novAVG < Convert.ToInt32(totalAVG) && novAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(decAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (decAVG < Convert.ToInt32(totalAVG) && decAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(totalAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.BrownTotalNumber, true);
                    rowIndex++;
                }
                #region Grand Total II
                //excelReport.SetCellValue("GRAND TOTAL II",8 + rowIndex, 0, 1, dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + modelWidth + colorWidth, ExcelCellStyle.PinkTotal);
                //excelReport.SetCellValue(janGTII.ToString("#,##0;0;'-';@"),8 + rowIndex, 0 + totalDetailWidth, 1, 1, (janGTII < Convert.ToInt32(average) && janGTII != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(febGTII.ToString("#,##0;0;'-';@"),8 + rowIndex, 1 + totalDetailWidth, 1, 1, (febGTII < Convert.ToInt32(average) && febGTII != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(marGTII.ToString("#,##0;0;'-';@"),8 + rowIndex, 2 + totalDetailWidth, 1, 1, (marGTII < Convert.ToInt32(average) && marGTII != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(aprGTII.ToString("#,##0;0;'-';@"),8 + rowIndex, 3 + totalDetailWidth, 1, 1, (aprGTII < Convert.ToInt32(average) && aprGTII != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(mayGTII.ToString("#,##0;0;'-';@"),8 + rowIndex, 4 + totalDetailWidth, 1, 1, (mayGTII < Convert.ToInt32(average) && mayGTII != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(junGTII.ToString("#,##0;0;'-';@"),8 + rowIndex, 5 + totalDetailWidth, 1, 1, (junGTII < Convert.ToInt32(average) && junGTII != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(julGTII.ToString("#,##0;0;'-';@"),8 + rowIndex, 6 + totalDetailWidth, 1, 1, (julGTII < Convert.ToInt32(average) && julGTII != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(augGTII.ToString("#,##0;0;'-';@"),8 + rowIndex, 7 + totalDetailWidth, 1, 1, (augGTII < Convert.ToInt32(average) && augGTII != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(sepGTII.ToString("#,##0;0;'-';@"),8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sepGTII < Convert.ToInt32(average) && sepGTII != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(octGTII.ToString("#,##0;0;'-';@"),8 + rowIndex, 9 + totalDetailWidth, 1, 1, (octGTII < Convert.ToInt32(average) && octGTII != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(novGTII.ToString("#,##0;0;'-';@"),8 + rowIndex, 10 + totalDetailWidth, 1, 1, (novGTII < Convert.ToInt32(average) && novGTII != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(decGTII.ToString("#,##0;0;'-';@"),8 + rowIndex, 11 + totalDetailWidth, 1, 1, (decGTII < Convert.ToInt32(average) && decGTII != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(totalGTII.ToString("#,##0;0;'-';@"),8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"),8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.PinkTotalNumber, true);
                //rowIndex++;

                //excelReport.SetCellValue("AVG GRAND TOTAL",8 + rowIndex, 0, 1, dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + modelWidth + colorWidth, ExcelCellStyle.PinkTotal);
                //excelReport.SetCellValue(janAVG.ToString("#,##0;0;'-';@"),8 + rowIndex, 0 + totalDetailWidth, 1, 1, (janAVG < Convert.ToInt32(totalAVG) && janAVG != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(febAVG.ToString("#,##0;0;'-';@"),8 + rowIndex, 1 + totalDetailWidth, 1, 1, (febAVG < Convert.ToInt32(totalAVG) && febAVG != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(marAVG.ToString("#,##0;0;'-';@"),8 + rowIndex, 2 + totalDetailWidth, 1, 1, (marAVG < Convert.ToInt32(totalAVG) && marAVG != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(aprAVG.ToString("#,##0;0;'-';@"),8 + rowIndex, 3 + totalDetailWidth, 1, 1, (aprAVG < Convert.ToInt32(totalAVG) && aprAVG != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(mayAVG.ToString("#,##0;0;'-';@"),8 + rowIndex, 4 + totalDetailWidth, 1, 1, (mayAVG < Convert.ToInt32(totalAVG) && mayAVG != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(junAVG.ToString("#,##0;0;'-';@"),8 + rowIndex, 5 + totalDetailWidth, 1, 1, (junAVG < Convert.ToInt32(totalAVG) && junAVG != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(julAVG.ToString("#,##0;0;'-';@"),8 + rowIndex, 6 + totalDetailWidth, 1, 1, (julAVG < Convert.ToInt32(totalAVG) && julAVG != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(augAVG.ToString("#,##0;0;'-';@"),8 + rowIndex, 7 + totalDetailWidth, 1, 1, (augAVG < Convert.ToInt32(totalAVG) && augAVG != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(sepAVG.ToString("#,##0;0;'-';@"),8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sepAVG < Convert.ToInt32(totalAVG) && sepAVG != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(octAVG.ToString("#,##0;0;'-';@"),8 + rowIndex, 9 + totalDetailWidth, 1, 1, (octAVG < Convert.ToInt32(totalAVG) && octAVG != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(novAVG.ToString("#,##0;0;'-';@"),8 + rowIndex, 10 + totalDetailWidth, 1, 1, (novAVG < Convert.ToInt32(totalAVG) && novAVG != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(decAVG.ToString("#,##0;0;'-';@"),8 + rowIndex, 11 + totalDetailWidth, 1, 1, (decAVG < Convert.ToInt32(totalAVG) && decAVG != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(totalAVG.ToString("#,##0;0;'-';@"),8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue("",8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.PinkTotal);
                #endregion
            }
            else if (parameter[9].ToString() == "0" && parameter[10].ToString() == "1")
            {
                decimal average = total / averageMonth;
                excelReport.ReplaceRestorePoint(RestorePoint.MarketModelRestorePoint.ToString(), marketModel.ToString(), 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, totalRowColor, modelWidth, ExcelCellStyle.LeftBorderedStandard);

                //Masukin total Model Catagory
                excelReport.SetCellValue("Total " + modelCatagory.ToString(), 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth + colorWidth, ExcelCellStyle.BlueTotal);
                excelReport.SetCellValue(jan.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (jan < Convert.ToInt32(average) && jan != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                excelReport.SetCellValue(feb.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (feb < Convert.ToInt32(average) && feb != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                excelReport.SetCellValue(mar.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (mar < Convert.ToInt32(average) && mar != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                excelReport.SetCellValue(apr.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (apr < Convert.ToInt32(average) && apr != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                excelReport.SetCellValue(may.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (may < Convert.ToInt32(average) && may != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                excelReport.SetCellValue(jun.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (jun < Convert.ToInt32(average) && jun != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                excelReport.SetCellValue(jul.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (jul < Convert.ToInt32(average) && jul != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                excelReport.SetCellValue(aug.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (aug < Convert.ToInt32(average) && aug != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                excelReport.SetCellValue(sep.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sep < Convert.ToInt32(average) && sep != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                excelReport.SetCellValue(oct.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (oct < Convert.ToInt32(average) && oct != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                excelReport.SetCellValue(nov.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (nov < Convert.ToInt32(average) && nov != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                excelReport.SetCellValue(dec.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (dec < Convert.ToInt32(average) && dec != 0) ? ExcelCellStyle.BlueTotalRedNumber : ExcelCellStyle.BlueTotalNumber, true);
                excelReport.SetCellValue(total.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);

                totalSubTotal++;
                totalRowBranchHead++;
                totalRowColor++;
                totalRowDealer++;
                totalRowModel++;
                totalRowSalesCo++;//<-- Untuk masukkin total nilai sub total tiap Model Catagory
                totalRowSalesHead++;
                totalRowSalsman++;
                rowIndex++;

                average = totalGT / averageMonth;
                //Masukkin sub total model category
                excelReport.SetCellValue("SubTotal", 8 + rowIndex, 0 + dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth, 1, modelWidth + colorWidth, ExcelCellStyle.PurpleTotal);
                excelReport.SetCellValue(janGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (janGT < Convert.ToInt32(average) && janGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                excelReport.SetCellValue(febGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (febGT < Convert.ToInt32(average) && febGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                excelReport.SetCellValue(marGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (marGT < Convert.ToInt32(average) && marGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                excelReport.SetCellValue(aprGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (aprGT < Convert.ToInt32(average) && aprGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                excelReport.SetCellValue(mayGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (mayGT < Convert.ToInt32(average) && mayGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                excelReport.SetCellValue(junGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (junGT < Convert.ToInt32(average) && junGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                excelReport.SetCellValue(julGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (julGT < Convert.ToInt32(average) && julGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                excelReport.SetCellValue(augGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (augGT < Convert.ToInt32(average) && augGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                excelReport.SetCellValue(sepGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sepGT < Convert.ToInt32(average) && sepGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                excelReport.SetCellValue(octGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (octGT < Convert.ToInt32(average) && octGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                excelReport.SetCellValue(novGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (novGT < Convert.ToInt32(average) && novGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                excelReport.SetCellValue(decGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (decGT < Convert.ToInt32(average) && decGT != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                excelReport.SetCellValue(totalGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);

                totalSubTotal++;
                totalRowBranchHead++;
                totalRowColor++;
                totalRowDealer++;
                totalRowModel++;
                totalRowSalesCo++;//<-- Untuk masukkin total nilai sub total tiap Model Catagory
                totalRowSalesHead++;
                totalRowSalsman++;
                rowIndex++;

                jan = feb = mar = apr = may = jun = jul = aug = sep = oct = nov = dec = total = 0;
                janGT = febGT = marGT = aprGT = mayGT = junGT = julGT = augGT = sepGT = octGT = novGT = decGT = totalGT = 0;

                excelReport.ReplaceRestorePoint(RestorePoint.SalesCoordinatorrestorePoint.ToString(), "AVG", 0 + dealerWidth, totalRowSalsman, outletWidth + salesHeadWidth + salesCoordinatorWidth, ExcelCellStyle.LeftBorderedTop);
                excelReport.ReplaceRestorePoint(RestorePoint.CompanyCodeRestorePoint.ToString(), dealer.ToString(), 0, totalRowDealer, dealerWidth, ExcelCellStyle.LeftBorderedStandardWrap);

                if (parameter[5].ToString() == "ALL")
                {
                    average = totalArea / averageMonth;
                    //Masukin total Model Catagory
                    excelReport.SetCellValue("Total " + area.ToString(), 8 + rowIndex, 0, 1, dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + modelWidth + colorWidth, ExcelCellStyle.PurpleTotal);
                    excelReport.SetCellValue(janArea.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (janArea < Convert.ToInt32(average) && janArea != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(febArea.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (febArea < Convert.ToInt32(average) && febArea != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(marArea.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (marArea < Convert.ToInt32(average) && marArea != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(aprArea.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (aprArea < Convert.ToInt32(average) && aprArea != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(mayArea.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (mayArea < Convert.ToInt32(average) && mayArea != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(junArea.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (junArea < Convert.ToInt32(average) && junArea != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(julArea.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (julArea < Convert.ToInt32(average) && julArea != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(augArea.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (augArea < Convert.ToInt32(average) && augArea != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(sepArea.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sepArea < Convert.ToInt32(average) && sepArea != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(octArea.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (octArea < Convert.ToInt32(average) && octArea != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(novArea.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (novArea < Convert.ToInt32(average) && novArea != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(decArea.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (decArea < Convert.ToInt32(average) && decArea != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(totalArea.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);

                    rowIndex++;

                    janAVG = janArea / totalDealer;
                    febAVG = febArea / totalDealer;
                    marAVG = marArea / totalDealer;
                    aprAVG = aprArea / totalDealer;
                    mayAVG = mayArea / totalDealer;
                    junAVG = junArea / totalDealer;
                    julAVG = julArea / totalDealer;
                    augAVG = augArea / totalDealer;
                    sepAVG = sepArea / totalDealer;
                    octAVG = octArea / totalDealer;
                    novAVG = novArea / totalDealer;
                    decAVG = decArea / totalDealer;
                    totalAVG = (janAVG + febAVG + marAVG + aprAVG + mayAVG + junAVG + julAVG + augAVG + sepAVG + octAVG + novAVG + decAVG);
                    average = totalAVG / averageMonth;

                    excelReport.SetCellValue("AVG " + area.ToString(), 8 + rowIndex, 0, 1, dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + modelWidth + colorWidth, ExcelCellStyle.BrownTotal);
                    excelReport.SetCellValue(janAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (janAVG < Convert.ToInt32(totalAVG) && janAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(febAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (febAVG < Convert.ToInt32(totalAVG) && febAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(marAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (marAVG < Convert.ToInt32(totalAVG) && marAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(aprAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (aprAVG < Convert.ToInt32(totalAVG) && aprAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(mayAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (mayAVG < Convert.ToInt32(totalAVG) && mayAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(junAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (junAVG < Convert.ToInt32(totalAVG) && junAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(julAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (julAVG < Convert.ToInt32(totalAVG) && julAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(augAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (augAVG < Convert.ToInt32(totalAVG) && augAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(sepAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sepAVG < Convert.ToInt32(totalAVG) && sepAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(octAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (octAVG < Convert.ToInt32(totalAVG) && octAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(novAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (novAVG < Convert.ToInt32(totalAVG) && novAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(decAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (decAVG < Convert.ToInt32(totalAVG) && decAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(totalAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.BrownTotalNumber, true);
                    rowIndex++;
                }

                if (parameter[3].ToString() == "ALL")
                {
                    average = totalGTII / averageYear;
                    excelReport.SetCellValue("TOTAL NATIONAL", 8 + rowIndex, 0, 1, dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + modelWidth + colorWidth, ExcelCellStyle.GreenTotal);
                    excelReport.SetCellValue(janGTII.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (janGTII < Convert.ToInt32(average) && janGTII != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                    excelReport.SetCellValue(febGTII.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (febGTII < Convert.ToInt32(average) && febGTII != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                    excelReport.SetCellValue(marGTII.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (marGTII < Convert.ToInt32(average) && marGTII != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                    excelReport.SetCellValue(aprGTII.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (aprGTII < Convert.ToInt32(average) && aprGTII != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                    excelReport.SetCellValue(mayGTII.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (mayGTII < Convert.ToInt32(average) && mayGTII != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                    excelReport.SetCellValue(junGTII.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (junGTII < Convert.ToInt32(average) && junGTII != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                    excelReport.SetCellValue(julGTII.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (julGTII < Convert.ToInt32(average) && julGTII != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                    excelReport.SetCellValue(augGTII.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (augGTII < Convert.ToInt32(average) && augGTII != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                    excelReport.SetCellValue(sepGTII.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sepGTII < Convert.ToInt32(average) && sepGTII != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                    excelReport.SetCellValue(octGTII.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (octGTII < Convert.ToInt32(average) && octGTII != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                    excelReport.SetCellValue(novGTII.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (novGTII < Convert.ToInt32(average) && novGTII != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                    excelReport.SetCellValue(decGTII.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (decGTII < Convert.ToInt32(average) && decGTII != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                    excelReport.SetCellValue(totalGTII.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.GreenTotalNumber, true);
                    excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.GreenTotalNumber, true);
                    rowIndex++;

                    janAVG = janGTII / totalDealerGT;
                    febAVG = febGTII / totalDealerGT;
                    marAVG = marGTII / totalDealerGT;
                    aprAVG = aprGTII / totalDealerGT;
                    mayAVG = mayGTII / totalDealerGT;
                    junAVG = junGTII / totalDealerGT;
                    julAVG = julGTII / totalDealerGT;
                    augAVG = augGTII / totalDealerGT;
                    sepAVG = sepGTII / totalDealerGT;
                    octAVG = octGTII / totalDealerGT;
                    novAVG = novGTII / totalDealerGT;
                    decAVG = decGTII / totalDealerGT;
                    totalAVG = (janAVG + febAVG + marAVG + aprAVG + mayAVG + junAVG + julAVG + augAVG + sepAVG + octAVG + novAVG + decAVG);
                    average = totalAVG / averageYear;

                    excelReport.SetCellValue("AVG NATIONAL", 8 + rowIndex, 0, 1, dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + modelWidth + colorWidth, ExcelCellStyle.BrownTotal);
                    excelReport.SetCellValue(janAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (janAVG < Convert.ToInt32(totalAVG) && janAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(febAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (febAVG < Convert.ToInt32(totalAVG) && febAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(marAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (marAVG < Convert.ToInt32(totalAVG) && marAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(aprAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (aprAVG < Convert.ToInt32(totalAVG) && aprAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(mayAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (mayAVG < Convert.ToInt32(totalAVG) && mayAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(junAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (junAVG < Convert.ToInt32(totalAVG) && junAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(julAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (julAVG < Convert.ToInt32(totalAVG) && julAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(augAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (augAVG < Convert.ToInt32(totalAVG) && augAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(sepAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sepAVG < Convert.ToInt32(totalAVG) && sepAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(octAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (octAVG < Convert.ToInt32(totalAVG) && octAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(novAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (novAVG < Convert.ToInt32(totalAVG) && novAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(decAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (decAVG < Convert.ToInt32(totalAVG) && decAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(totalAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.BrownTotalNumber, true);
                    rowIndex++;
                }
                #region Grand Total II

                //excelReport.SetCellValue("GRAND TOTAL II",8 + rowIndex, 0, 1, dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + modelWidth + colorWidth, ExcelCellStyle.PinkTotal);
                //excelReport.SetCellValue(janGTII.ToString("#,##0;0;'-';@"),8 + rowIndex, 0 + totalDetailWidth, 1, 1, (janGTII < Convert.ToInt32(average) && janGTII != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(febGTII.ToString("#,##0;0;'-';@"),8 + rowIndex, 1 + totalDetailWidth, 1, 1, (febGTII < Convert.ToInt32(average) && febGTII != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(marGTII.ToString("#,##0;0;'-';@"),8 + rowIndex, 2 + totalDetailWidth, 1, 1, (marGTII < Convert.ToInt32(average) && marGTII != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(aprGTII.ToString("#,##0;0;'-';@"),8 + rowIndex, 3 + totalDetailWidth, 1, 1, (aprGTII < Convert.ToInt32(average) && aprGTII != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(mayGTII.ToString("#,##0;0;'-';@"),8 + rowIndex, 4 + totalDetailWidth, 1, 1, (mayGTII < Convert.ToInt32(average) && mayGTII != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(junGTII.ToString("#,##0;0;'-';@"),8 + rowIndex, 5 + totalDetailWidth, 1, 1, (junGTII < Convert.ToInt32(average) && junGTII != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(julGTII.ToString("#,##0;0;'-';@"),8 + rowIndex, 6 + totalDetailWidth, 1, 1, (julGTII < Convert.ToInt32(average) && julGTII != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(augGTII.ToString("#,##0;0;'-';@"),8 + rowIndex, 7 + totalDetailWidth, 1, 1, (augGTII < Convert.ToInt32(average) && augGTII != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(sepGTII.ToString("#,##0;0;'-';@"),8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sepGTII < Convert.ToInt32(average) && sepGTII != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(octGTII.ToString("#,##0;0;'-';@"),8 + rowIndex, 9 + totalDetailWidth, 1, 1, (octGTII < Convert.ToInt32(average) && octGTII != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(novGTII.ToString("#,##0;0;'-';@"),8 + rowIndex, 10 + totalDetailWidth, 1, 1, (novGTII < Convert.ToInt32(average) && novGTII != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(decGTII.ToString("#,##0;0;'-';@"),8 + rowIndex, 11 + totalDetailWidth, 1, 1, (decGTII < Convert.ToInt32(average) && decGTII != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(totalGTII.ToString("#,##0;0;'-';@"),8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"),8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.PinkTotalNumber, true);
                //rowIndex++;

                //excelReport.SetCellValue("AVG GRAND TOTAL",8 + rowIndex, 0, 1, dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + modelWidth + colorWidth, ExcelCellStyle.PinkTotal);
                //excelReport.SetCellValue(janAVG.ToString("#,##0;0;'-';@"),8 + rowIndex, 0 + totalDetailWidth, 1, 1, (janAVG < Convert.ToInt32(totalAVG) && janAVG != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(febAVG.ToString("#,##0;0;'-';@"),8 + rowIndex, 1 + totalDetailWidth, 1, 1, (febAVG < Convert.ToInt32(totalAVG) && febAVG != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(marAVG.ToString("#,##0;0;'-';@"),8 + rowIndex, 2 + totalDetailWidth, 1, 1, (marAVG < Convert.ToInt32(totalAVG) && marAVG != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(aprAVG.ToString("#,##0;0;'-';@"),8 + rowIndex, 3 + totalDetailWidth, 1, 1, (aprAVG < Convert.ToInt32(totalAVG) && aprAVG != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(mayAVG.ToString("#,##0;0;'-';@"),8 + rowIndex, 4 + totalDetailWidth, 1, 1, (mayAVG < Convert.ToInt32(totalAVG) && mayAVG != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(junAVG.ToString("#,##0;0;'-';@"),8 + rowIndex, 5 + totalDetailWidth, 1, 1, (junAVG < Convert.ToInt32(totalAVG) && junAVG != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(julAVG.ToString("#,##0;0;'-';@"),8 + rowIndex, 6 + totalDetailWidth, 1, 1, (julAVG < Convert.ToInt32(totalAVG) && julAVG != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(augAVG.ToString("#,##0;0;'-';@"),8 + rowIndex, 7 + totalDetailWidth, 1, 1, (augAVG < Convert.ToInt32(totalAVG) && augAVG != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(sepAVG.ToString("#,##0;0;'-';@"),8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sepAVG < Convert.ToInt32(totalAVG) && sepAVG != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(octAVG.ToString("#,##0;0;'-';@"),8 + rowIndex, 9 + totalDetailWidth, 1, 1, (octAVG < Convert.ToInt32(totalAVG) && octAVG != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(novAVG.ToString("#,##0;0;'-';@"),8 + rowIndex, 10 + totalDetailWidth, 1, 1, (novAVG < Convert.ToInt32(totalAVG) && novAVG != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(decAVG.ToString("#,##0;0;'-';@"),8 + rowIndex, 11 + totalDetailWidth, 1, 1, (decAVG < Convert.ToInt32(totalAVG) && decAVG != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(totalAVG.ToString("#,##0;0;'-';@"),8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue("",8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.PinkTotal); 
                #endregion

            }
            else
            {
                excelReport.ReplaceRestorePoint(RestorePoint.CompanyCodeRestorePoint.ToString(), dealer.ToString(), 0, totalRowDealer, dealerWidth, ExcelCellStyle.LeftBorderedTop);
                if (parameter[5].ToString() == "ALL")
                {
                    decimal average = totalArea / averageMonth;

                    //Masukin total Model Catagory
                    excelReport.SetCellValue("Total " + area.ToString(), 8 + rowIndex, 0, 1, dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + modelWidth + colorWidth, ExcelCellStyle.PurpleTotal);
                    excelReport.SetCellValue(janArea.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (janArea < Convert.ToInt32(average) && janArea != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(febArea.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (febArea < Convert.ToInt32(average) && febArea != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(marArea.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (marArea < Convert.ToInt32(average) && marArea != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(aprArea.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (aprArea < Convert.ToInt32(average) && aprArea != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(mayArea.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (mayArea < Convert.ToInt32(average) && mayArea != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(junArea.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (junArea < Convert.ToInt32(average) && junArea != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(julArea.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (julArea < Convert.ToInt32(average) && julArea != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(augArea.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (augArea < Convert.ToInt32(average) && augArea != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(sepArea.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sepArea < Convert.ToInt32(average) && sepArea != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(octArea.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (octArea < Convert.ToInt32(average) && octArea != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(novArea.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (novArea < Convert.ToInt32(average) && novArea != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(decArea.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (decArea < Convert.ToInt32(average) && decArea != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(totalArea.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                    excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);

                    rowIndex++;

                    janAVG = janArea / totalDealer;
                    febAVG = febArea / totalDealer;
                    marAVG = marArea / totalDealer;
                    aprAVG = aprArea / totalDealer;
                    mayAVG = mayArea / totalDealer;
                    junAVG = junArea / totalDealer;
                    julAVG = julArea / totalDealer;
                    augAVG = augArea / totalDealer;
                    sepAVG = sepArea / totalDealer;
                    octAVG = octArea / totalDealer;
                    novAVG = novArea / totalDealer;
                    decAVG = decArea / totalDealer;
                    totalAVG = (janAVG + febAVG + marAVG + aprAVG + mayAVG + junAVG + julAVG + augAVG + sepAVG + octAVG + novAVG + decAVG);
                    average = totalAVG / averageMonth;

                    excelReport.SetCellValue("AVG " + area.ToString(), 8 + rowIndex, 0, 1, dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + modelWidth + colorWidth, ExcelCellStyle.BrownTotal);
                    excelReport.SetCellValue(janAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (janAVG < Convert.ToInt32(totalAVG) && janAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(febAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (febAVG < Convert.ToInt32(totalAVG) && febAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(marAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (marAVG < Convert.ToInt32(totalAVG) && marAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(aprAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (aprAVG < Convert.ToInt32(totalAVG) && aprAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(mayAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (mayAVG < Convert.ToInt32(totalAVG) && mayAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(junAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (junAVG < Convert.ToInt32(totalAVG) && junAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(julAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (julAVG < Convert.ToInt32(totalAVG) && julAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(augAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (augAVG < Convert.ToInt32(totalAVG) && augAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(sepAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sepAVG < Convert.ToInt32(totalAVG) && sepAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(octAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (octAVG < Convert.ToInt32(totalAVG) && octAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(novAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (novAVG < Convert.ToInt32(totalAVG) && novAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(decAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (decAVG < Convert.ToInt32(totalAVG) && decAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(totalAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.BrownTotalNumber, true);
                    rowIndex++;
                }
                if (parameter[3].ToString() == "ALL")
                {
                    decimal average = totalGT / averageYear;
                    excelReport.SetCellValue("TOTAL NATIONAL", 8 + rowIndex, 0, 1, dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + modelWidth + colorWidth, ExcelCellStyle.GreenTotal);
                    excelReport.SetCellValue(janGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (janGT < Convert.ToInt32(average) && janGT != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                    excelReport.SetCellValue(febGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (febGT < Convert.ToInt32(average) && febGT != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                    excelReport.SetCellValue(marGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (marGT < Convert.ToInt32(average) && marGT != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                    excelReport.SetCellValue(aprGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (aprGT < Convert.ToInt32(average) && aprGT != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                    excelReport.SetCellValue(mayGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (mayGT < Convert.ToInt32(average) && mayGT != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                    excelReport.SetCellValue(junGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (junGT < Convert.ToInt32(average) && junGT != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                    excelReport.SetCellValue(julGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (julGT < Convert.ToInt32(average) && julGT != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                    excelReport.SetCellValue(augGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (augGT < Convert.ToInt32(average) && augGT != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                    excelReport.SetCellValue(sepGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sepGT < Convert.ToInt32(average) && sepGT != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                    excelReport.SetCellValue(octGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (octGT < Convert.ToInt32(average) && octGT != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                    excelReport.SetCellValue(novGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (novGT < Convert.ToInt32(average) && novGT != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                    excelReport.SetCellValue(decGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (decGT < Convert.ToInt32(average) && decGT != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                    excelReport.SetCellValue(totalGT.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.GreenTotalNumber, true);
                    excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.GreenTotalNumber, true);
                    rowIndex++;

                    janAVG = janGT / totalDealerGT;
                    febAVG = febGT / totalDealerGT;
                    marAVG = marGT / totalDealerGT;
                    aprAVG = aprGT / totalDealerGT;
                    mayAVG = mayGT / totalDealerGT;
                    junAVG = junGT / totalDealerGT;
                    julAVG = julGT / totalDealerGT;
                    augAVG = augGT / totalDealerGT;
                    sepAVG = sepGT / totalDealerGT;
                    octAVG = octGT / totalDealerGT;
                    novAVG = novGT / totalDealerGT;
                    decAVG = decGT / totalDealerGT;
                    totalAVG = (janAVG + febAVG + marAVG + aprAVG + mayAVG + junAVG + julAVG + augAVG + sepAVG + octAVG + novAVG + decAVG);
                    average = totalAVG / averageYear;

                    excelReport.SetCellValue("AVG NATIONAL", 8 + rowIndex, 0, 1, dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + modelWidth + colorWidth, ExcelCellStyle.BrownTotal);
                    excelReport.SetCellValue(janAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, (janAVG < Convert.ToInt32(totalAVG) && janAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(febAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, (febAVG < Convert.ToInt32(totalAVG) && febAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(marAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 2 + totalDetailWidth, 1, 1, (marAVG < Convert.ToInt32(totalAVG) && marAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(aprAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, (aprAVG < Convert.ToInt32(totalAVG) && aprAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(mayAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, (mayAVG < Convert.ToInt32(totalAVG) && mayAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(junAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 5 + totalDetailWidth, 1, 1, (junAVG < Convert.ToInt32(totalAVG) && junAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(julAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, (julAVG < Convert.ToInt32(totalAVG) && julAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(augAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, (augAVG < Convert.ToInt32(totalAVG) && augAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(sepAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sepAVG < Convert.ToInt32(totalAVG) && sepAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(octAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, (octAVG < Convert.ToInt32(totalAVG) && octAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(novAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, (novAVG < Convert.ToInt32(totalAVG) && novAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(decAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 11 + totalDetailWidth, 1, 1, (decAVG < Convert.ToInt32(totalAVG) && decAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(totalAVG.ToString("#,##0;0;'-';@"), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.BrownTotalNumber, true);
                    rowIndex++;
                }
                #region Grand Total II

                //excelReport.SetCellValue("GRAND TOTAL II",8 + rowIndex, 0, 1, dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + modelWidth + colorWidth, ExcelCellStyle.PinkTotal);
                //excelReport.SetCellValue(janGT.ToString("#,##0;0;'-';@"),8 + rowIndex, 0 + totalDetailWidth, 1, 1, (janGT < Convert.ToInt32(average) && janGT != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(febGT.ToString("#,##0;0;'-';@"),8 + rowIndex, 1 + totalDetailWidth, 1, 1, (febGT < Convert.ToInt32(average) && febGT != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(marGT.ToString("#,##0;0;'-';@"),8 + rowIndex, 2 + totalDetailWidth, 1, 1, (marGT < Convert.ToInt32(average) && marGT != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(aprGT.ToString("#,##0;0;'-';@"),8 + rowIndex, 3 + totalDetailWidth, 1, 1, (aprGT < Convert.ToInt32(average) && aprGT != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(mayGT.ToString("#,##0;0;'-';@"),8 + rowIndex, 4 + totalDetailWidth, 1, 1, (mayGT < Convert.ToInt32(average) && mayGT != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(junGT.ToString("#,##0;0;'-';@"),8 + rowIndex, 5 + totalDetailWidth, 1, 1, (junGT < Convert.ToInt32(average) && junGT != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(julGT.ToString("#,##0;0;'-';@"),8 + rowIndex, 6 + totalDetailWidth, 1, 1, (julGT < Convert.ToInt32(average) && julGT != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(augGT.ToString("#,##0;0;'-';@"),8 + rowIndex, 7 + totalDetailWidth, 1, 1, (augGT < Convert.ToInt32(average) && augGT != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(sepGT.ToString("#,##0;0;'-';@"),8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sepGT < Convert.ToInt32(average) && sepGT != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(octGT.ToString("#,##0;0;'-';@"),8 + rowIndex, 9 + totalDetailWidth, 1, 1, (octGT < Convert.ToInt32(average) && octGT != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(novGT.ToString("#,##0;0;'-';@"),8 + rowIndex, 10 + totalDetailWidth, 1, 1, (novGT < Convert.ToInt32(average) && novGT != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(decGT.ToString("#,##0;0;'-';@"),8 + rowIndex, 11 + totalDetailWidth, 1, 1, (decGT < Convert.ToInt32(average) && decGT != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(totalGT.ToString("#,##0;0;'-';@"),8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(average.ToString("#,##0;0;'-';@"),8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.PinkTotalNumber, true);
                //rowIndex++;

                //excelReport.SetCellValue("AVG GRAND TOTAL",8 + rowIndex, 0, 1, dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + modelWidth + colorWidth, ExcelCellStyle.PinkTotal);
                //excelReport.SetCellValue(janAVG.ToString("#,##0;0;'-';@"),8 + rowIndex, 0 + totalDetailWidth, 1, 1, (janAVG < Convert.ToInt32(totalAVG) && janAVG != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(febAVG.ToString("#,##0;0;'-';@"),8 + rowIndex, 1 + totalDetailWidth, 1, 1, (febAVG < Convert.ToInt32(totalAVG) && febAVG != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(marAVG.ToString("#,##0;0;'-';@"),8 + rowIndex, 2 + totalDetailWidth, 1, 1, (marAVG < Convert.ToInt32(totalAVG) && marAVG != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(aprAVG.ToString("#,##0;0;'-';@"),8 + rowIndex, 3 + totalDetailWidth, 1, 1, (aprAVG < Convert.ToInt32(totalAVG) && aprAVG != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(mayAVG.ToString("#,##0;0;'-';@"),8 + rowIndex, 4 + totalDetailWidth, 1, 1, (mayAVG < Convert.ToInt32(totalAVG) && mayAVG != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(junAVG.ToString("#,##0;0;'-';@"),8 + rowIndex, 5 + totalDetailWidth, 1, 1, (junAVG < Convert.ToInt32(totalAVG) && junAVG != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(julAVG.ToString("#,##0;0;'-';@"),8 + rowIndex, 6 + totalDetailWidth, 1, 1, (julAVG < Convert.ToInt32(totalAVG) && julAVG != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(augAVG.ToString("#,##0;0;'-';@"),8 + rowIndex, 7 + totalDetailWidth, 1, 1, (augAVG < Convert.ToInt32(totalAVG) && augAVG != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(sepAVG.ToString("#,##0;0;'-';@"),8 + rowIndex, 8 + totalDetailWidth, 1, 1, (sepAVG < Convert.ToInt32(totalAVG) && sepAVG != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(octAVG.ToString("#,##0;0;'-';@"),8 + rowIndex, 9 + totalDetailWidth, 1, 1, (octAVG < Convert.ToInt32(totalAVG) && octAVG != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(novAVG.ToString("#,##0;0;'-';@"),8 + rowIndex, 10 + totalDetailWidth, 1, 1, (novAVG < Convert.ToInt32(totalAVG) && novAVG != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(decAVG.ToString("#,##0;0;'-';@"),8 + rowIndex, 11 + totalDetailWidth, 1, 1, (decAVG < Convert.ToInt32(totalAVG) && decAVG != 0) ? ExcelCellStyle.PinkTotalRedNumber : ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue(totalAVG.ToString("#,##0;0;'-';@"),8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.PinkTotalNumber, true);
                //excelReport.SetCellValue("",8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.PinkTotal);

                #endregion
            }
            #endregion
        }
    }

}