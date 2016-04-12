using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace SimDms.DataWarehouse.Helpers
{
    public class OmRpSalRgs043Xls
    {
        private string area, dealer, outlet, salesType, marketModel,dealercode,outletcode;
        private decimal jan, feb, mar, apr, may, jun, jul, aug, sep, oct, nov, dec, total, avg;
        private decimal janGT, febGT, marGT, aprGT, mayGT, junGT, julGT, augGT, sepGT, octGT, novGT, decGT, totalGT, avgGT;

        private decimal monthPct, totalpct;
        private decimal month1, month2, year1, year2, ytd;
        private decimal month1Area, month2Area, year1Area, year2Area, totalArea;
        private decimal month1AreaAdd, month2AreaAdd, year1AreaAdd, year2AreaAdd, totalAreaAdd;


        private decimal totalTtl = 0, month1Ttl = 0, month2Ttl = 0, year1Ttl = 0, year2Ttl = 0;
        private decimal totalTtlGT = 0, month1TtlGT = 0, month2TtlGT = 0, year1TtlGT = 0, year2TtlGT = 0;


        private bool ws = false, rtl = false;
        private int rowIndex = 0, totalDetailWidth = 0, dealerWidth = 0, outletWidth = 0, totalOutlet = 0;

        private DataSet ds = new DataSet();
        Dictionary<int, string> parameter = new Dictionary<int, string>();
        ExcelFileWriter excelReport = new ExcelFileWriter(0);
        string fileName;

        public OmRpSalRgs043Xls(DataSet dataSet, Dictionary<int, string> param, string flName)
        {
            parameter = param;
            ds = dataSet;
            fileName = flName;
        }

        void printdata(DataRow row)
        {

            #region Total
            month1Area += decimal.Parse(row["Month1"].ToString());
            month2Area += decimal.Parse(row["Month2"].ToString());
            year1Area += decimal.Parse(row["Year1"].ToString());
            year2Area += decimal.Parse(row["Year2"].ToString());
            totalArea += decimal.Parse(row["Total"].ToString());

            month1AreaAdd += decimal.Parse(row["Month1"].ToString());
            month2AreaAdd += decimal.Parse(row["Month2"].ToString());
            year1AreaAdd += decimal.Parse(row["Year1"].ToString());
            year2AreaAdd += decimal.Parse(row["Year2"].ToString());
            totalAreaAdd += decimal.Parse(row["Total"].ToString());

            month1Ttl += decimal.Parse(row["Month1"].ToString());
            month2Ttl += decimal.Parse(row["Month2"].ToString());
            year1Ttl += decimal.Parse(row["Year1"].ToString());
            year2Ttl += decimal.Parse(row["Year2"].ToString());
            totalTtl += decimal.Parse(row["Total"].ToString());

            month1TtlGT += decimal.Parse(row["Month1"].ToString());
            month2TtlGT += decimal.Parse(row["Month2"].ToString());
            year1TtlGT += decimal.Parse(row["Year1"].ToString());
            year2TtlGT += decimal.Parse(row["Year2"].ToString());
            totalTtlGT += decimal.Parse(row["Total"].ToString());

            month1 = decimal.Parse(row["Month1"].ToString());
            month2 = decimal.Parse(row["Month2"].ToString());
            year1 = decimal.Parse(row["Year1"].ToString());
            year2 = decimal.Parse(row["Year2"].ToString());

            monthPct = month1 != 0 ? (month2 / month1) * 100 : 0;
            totalpct = year1 != 0 ? (year2 / year1) * 100 : 0;

            #endregion

            excelReport.SetCellValue(row["Total"].ToString(), 8 + rowIndex, 0 + totalDetailWidth, 1, 2, ExcelCellStyle.RightBorderedStandardNumber, true);
            excelReport.SetCellValue(row["Month1"].ToString(), 8 + rowIndex, 2 + totalDetailWidth, 1, 2, ExcelCellStyle.RightBorderedStandardNumber, true);
            excelReport.SetCellValue(row["Month2"].ToString(), 8 + rowIndex, 4 + totalDetailWidth, 1, 2, ExcelCellStyle.RightBorderedStandardNumber, true);
            excelReport.SetCellValue(monthPct.ToString("n2") + "%", 8 + rowIndex, 6 + totalDetailWidth, 1, 2, ExcelCellStyle.RightBorderedStandardNumber);


            excelReport.SetCellValue(decimal.Parse(row["Year1"].ToString()).ToString("#,###;(#,##0.00);\"-\";_(@_)"), 8 + rowIndex, 8 + totalDetailWidth, 1, 2, ExcelCellStyle.RightBorderedStandardNumber, true);
            excelReport.SetCellValue(decimal.Parse(row["Year2"].ToString()).ToString("#,###;(#,##0.00);\"-\";_(@_)"), 8 + rowIndex, 10 + totalDetailWidth, 1, 2, ExcelCellStyle.RightBorderedStandardNumber, true);            
            excelReport.SetCellValue(totalpct.ToString("n2") + "%", 8 + rowIndex, 12 + totalDetailWidth, 1, 2, ExcelCellStyle.RightBorderedStandardNumber);


        }

        void totalarea()
        {



            monthPct = month1Area != 0 ? (month2Area / month1Area) * 100 : 0;
            totalpct = year1Area != 0 ? (year2Area / year1Area) * 100 : 0;
            excelReport.SetCellValue("Total " + area, 8 + rowIndex, 0, 1, dealerWidth + outletWidth, ExcelCellStyle.BlueTotal);
            excelReport.SetCellValue(totalArea.ToString(), 8 + rowIndex, 0 + totalDetailWidth, 1, 2, ExcelCellStyle.BlueTotalNumber, true);
            excelReport.SetCellValue(month1Area.ToString(), 8 + rowIndex, 2 + totalDetailWidth, 1, 2, ExcelCellStyle.BlueTotalNumber, true);
            excelReport.SetCellValue(month2Area.ToString(), 8 + rowIndex, 4 + totalDetailWidth, 1, 2, ExcelCellStyle.BlueTotalNumber, true);
            excelReport.SetCellValue(monthPct.ToString("n2") + "%", 8 + rowIndex, 6 + totalDetailWidth, 1, 2, ExcelCellStyle.BlueTotalNumber);


            excelReport.SetCellValue(year1Area.ToString(), 8 + rowIndex, 8 + totalDetailWidth, 1, 2, ExcelCellStyle.BlueTotalNumber, true);
            excelReport.SetCellValue(year2Area.ToString(), 8 + rowIndex, 10 + totalDetailWidth, 1, 2, ExcelCellStyle.BlueTotalNumber, true);
            excelReport.SetCellValue(totalpct.ToString("n2") + "%", 8 + rowIndex, 12 + totalDetailWidth, 1, 2, ExcelCellStyle.BlueTotalNumber);


            month1Area = month2Area = year1Area = year2Area = totalArea = 0;
        }

        void printtotaldealer()
        {

            monthPct = month1Ttl != 0 ? (month2Ttl / month1Ttl) * 100 : 0;
            totalpct = year1Ttl != 0 ? (year2Ttl / year1Ttl) * 100 : 0;            
            
            excelReport.SetCellValue("Total", 8 + rowIndex, 0 + dealerWidth, 1, outletWidth, ExcelCellStyle.YellowTotal);

            excelReport.SetCellValue(totalTtl.ToString(), 8 + rowIndex, 0 + totalDetailWidth, 1, 2, ExcelCellStyle.YellowTotalNumber, true);
            excelReport.SetCellValue(month1Ttl.ToString(), 8 + rowIndex, 2 + totalDetailWidth, 1, 2, ExcelCellStyle.YellowTotalNumber, true);
            excelReport.SetCellValue(month2Ttl.ToString(), 8 + rowIndex, 4 + totalDetailWidth, 1, 2, ExcelCellStyle.YellowTotalNumber, true);
            excelReport.SetCellValue(monthPct.ToString("n2") + "%", 8 + rowIndex, 6 + totalDetailWidth, 1, 2, ExcelCellStyle.YellowTotalNumber);


            excelReport.SetCellValue(year1Ttl.ToString(), 8 + rowIndex, 8 + totalDetailWidth, 1, 2, ExcelCellStyle.YellowTotalNumber, true);
            excelReport.SetCellValue(year2Ttl.ToString(), 8 + rowIndex, 10 + totalDetailWidth, 1, 2, ExcelCellStyle.YellowTotalNumber, true);
            excelReport.SetCellValue(totalpct.ToString("n2") + "%", 8 + rowIndex, 12 + totalDetailWidth, 1, 2, ExcelCellStyle.YellowTotalNumber);
            
            totalTtl = month1Ttl = month2Ttl = year1Ttl = year2Ttl = 0;

        }

        public void CreateReport(string pserver)
        {
            excelReport = new ExcelFileWriter(fileName, "2016", "OmRpSalRgs041", pserver);


            DataTable dt0 = ds.Tables[0];

            area = dt0.Rows[0]["Area"].ToString();
            dealer = dt0.Rows[0]["CompanyName"].ToString();
            dealercode = dt0.Rows[0]["CompanyCode"].ToString();
            //salesType = dt0.Rows[0]["SalesType"].ToString();
            outlet = parameter[9].ToString() == "1" ? dt0.Rows[0]["BranchName"].ToString() : "";
            outletcode = parameter[9].ToString() == "1" ? dt0.Rows[0]["BranchCode"].ToString() : "";
            marketModel = parameter[10].ToString() == "1" ? dt0.Rows[0]["MarketModel"].ToString() : "";

            StringBuilder sb = new StringBuilder();
            //sb.Append("(by Area, Dealer");
            sb.Append(parameter[9].ToString() != "0" ? ", Outlet" : "");
            sb.Append(parameter[10].ToString() != "" ? " & Type : " + parameter[10].ToString() : " & All Type");
            //sb.Append(")");

            string title = sb.ToString().Trim().StartsWith(",") || sb.ToString().Trim().StartsWith("&") ? sb.ToString().Trim().Remove(0, 1) : sb.ToString();

            //Pemetaan ukuran column
            dealerWidth = 3;
            if (parameter[9].ToString() == "1")
                outletWidth = 3;

            totalDetailWidth = dealerWidth + outletWidth;

            int indentTitleDesc = 10;

            #region Header

            excelReport.SetCellValue("COMPARATION : MONTH to MONTH & YEAR to YEAR ", 0, 0, 1, dealerWidth + indentTitleDesc + indentTitleDesc + 2, ExcelCellStyle.Header);
            excelReport.SetCellValue(title, 1, 0, 1, dealerWidth + indentTitleDesc + indentTitleDesc + 2, ExcelCellStyle.Header2);

            excelReport.SetCellValue("Periode ", 2, 0, 1, dealerWidth, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue(": " + parameter[1].ToString(), 2, dealerWidth, 1, indentTitleDesc, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue("Branch Manager", 2, dealerWidth + indentTitleDesc, 1, 2, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue(": " + parameter[2].ToString(), 2, dealerWidth + indentTitleDesc + 2, 1, indentTitleDesc, ExcelCellStyle.LeftBold);

            excelReport.SetCellValue("Area", 3, 0, 1, dealerWidth, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue(": " + parameter[3].ToString(), 3, dealerWidth, 1, indentTitleDesc, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue("Sales Head", 3, dealerWidth + indentTitleDesc, 1, 2, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue(": " + parameter[4].ToString(), 3, dealerWidth + indentTitleDesc + 2, 1, indentTitleDesc, ExcelCellStyle.LeftBold);

            excelReport.SetCellValue("Dealer", 4, 0, 1, dealerWidth, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue(": " + parameter[5].ToString(), 4, dealerWidth, 1, indentTitleDesc, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue("Sales Coordinator", 4, dealerWidth + indentTitleDesc, 1, 2, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue(": " + parameter[6].ToString(), 4, dealerWidth + indentTitleDesc + 2, 1, indentTitleDesc, ExcelCellStyle.LeftBold);

            excelReport.SetCellValue("Outlet", 5, 0, 1, dealerWidth, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue(": " + parameter[7].ToString(), 5, dealerWidth, 1, indentTitleDesc, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue("Wiraniaga", 5, dealerWidth + indentTitleDesc, 1, 2, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue(": " + parameter[8].ToString(), 5, dealerWidth + indentTitleDesc + 2, 1, indentTitleDesc, ExcelCellStyle.LeftBold);

            excelReport.SetCellValue("Dealer", 6, 0, 2, dealerWidth, ExcelCellStyle.CenterBorderedBold);
            if (parameter[9].ToString() == "1")
                excelReport.SetCellValue("Outlet", 6, 0 + dealerWidth, 2, outletWidth, ExcelCellStyle.CenterBorderedBold);


            excelReport.SetCellValue(parameter[13].ToString(), 6, 0 + totalDetailWidth, 2, 2, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue(parameter[14].ToString(), 6, 2 + totalDetailWidth, 1, 6, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Jan - " + parameter[12].ToString(), 6, 8 + totalDetailWidth, 1, 6, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue(parameter[11].ToString() + " " + parameter[14].ToString(), 7, 2 + totalDetailWidth, 1, 2, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue(parameter[12].ToString() + " " + parameter[14].ToString(), 7, 4 + totalDetailWidth, 1, 2, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("%", 7, 6 + totalDetailWidth, 1, 2, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue(parameter[13].ToString(), 7, 8 + totalDetailWidth, 1, 2, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue(parameter[14].ToString(), 7, 10 + totalDetailWidth, 1, 2, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("%", 7, 12 + totalDetailWidth, 1, 2, ExcelCellStyle.CenterBorderedBold);

            //excelReport.CloseExcelFileWriter();
            //return;

            #endregion

            rowIndex = 0;



            area = "";
            dealer = "";
            dealercode = "";
            outletcode = "";
            outlet = "";
            marketModel = "";


            foreach (DataRow row in dt0.Rows)
            {
                #region Looping
                if (area == "")
                {
                    #region first run
                    dealer = row["CompanyName"].ToString();
                    dealercode = row["CompanyCode"].ToString();
                    area = row["Area"].ToString();
                    outlet = parameter[9].ToString() == "1" ? row["BranchName"].ToString() : "";
                    marketModel = parameter[10].ToString() == "1" ? row["MarketModel"].ToString() : "";
                    totalOutlet = 0;
                    if (parameter[9].ToString() == "1")
                    {
                        excelReport.RestorePoint(8 + rowIndex, "RestoreDealer");
                        excelReport.SetCellValue(row["BranchName"].ToString(), 8 + rowIndex, 0 + dealerWidth, 1, outletWidth, ExcelCellStyle.LeftBorderedStandard);
                    }
                    else
                    {
                        excelReport.SetCellValue(dealer.ToString(), 8 + rowIndex, 0, 1, dealerWidth, ExcelCellStyle.LeftBorderedStandard);
                    }

                    #endregion
                }
                else if (area == row["Area"].ToString() && dealercode == row["CompanyCode"].ToString() && outlet != row["BranchName"].ToString())
                {
                    #region Satu
                    totalOutlet++;
                    rowIndex++;
                    excelReport.SetCellValue(row["BranchName"].ToString(), 8 + rowIndex, 0 + dealerWidth, 1, outletWidth, ExcelCellStyle.LeftBorderedStandard);
                    outlet = row["BranchName"].ToString();

                    
                    total = month1 = month2 = year1 = year2 = 0;
                    #endregion
                }
                else if (area == row["Area"].ToString() && dealercode != row["CompanyCode"].ToString())
                {
                    #region Dua
                    if (parameter[9].ToString() == "1")
                    {

                        excelReport.ReplaceRestorePoint("RestoreDealer", dealer.ToString(), 0, totalOutlet + 2, dealerWidth, ExcelCellStyle.LeftBorderedStandard);
                        totalOutlet = 0;
                        rowIndex++;
                        printtotaldealer();
                        dealer = row["CompanyName"].ToString();
                        dealercode = row["CompanyCode"].ToString();
                        outlet = parameter[9].ToString() == "1" ? row["BranchName"].ToString() : "";
                        marketModel = parameter[10].ToString() == "1" ? row["MarketModel"].ToString() : "";
                        totalOutlet = 0;
                        rowIndex++;
                        excelReport.RestorePoint(8 + rowIndex, "RestoreDealer");
                        excelReport.SetCellValue(row["BranchName"].ToString(), 8 + rowIndex, 0 + dealerWidth, 1, outletWidth, ExcelCellStyle.LeftBorderedStandard);
                    }
                    else
                    {
                        rowIndex++;
                        dealer = row["CompanyName"].ToString();
                        dealercode = row["CompanyCode"].ToString();
                        outlet = parameter[9].ToString() == "1" ? row["BranchName"].ToString() : "";
                        marketModel = parameter[10].ToString() == "1" ? row["MarketModel"].ToString() : "";
                        totalOutlet = 0;
                        excelReport.SetCellValue(dealer.ToString(), 8 + rowIndex, 0, 1, dealerWidth, ExcelCellStyle.LeftBorderedStandard);
                    }
                    #endregion
                }
                else if (area != row["Area"].ToString())
                {
                    if (parameter[9].ToString() == "1")
                    {
                        rowIndex++;
                        excelReport.ReplaceRestorePoint("RestoreDealer", dealer.ToString(), 0, totalOutlet + 2, dealerWidth, ExcelCellStyle.LeftBorderedStandard);
                        printtotaldealer();
                        rowIndex++;
                        totalarea();
                        rowIndex++;
                        totalOutlet = 0;
                        excelReport.RestorePoint(8 + rowIndex, "RestoreDealer");
                        excelReport.SetCellValue(row["BranchName"].ToString(), 8 + rowIndex, 0 + dealerWidth, 1, outletWidth, ExcelCellStyle.LeftBorderedStandard);
                        dealer = row["CompanyName"].ToString();
                        dealercode = row["CompanyCode"].ToString();
                        area = row["Area"].ToString();
                        outlet = parameter[9].ToString() == "1" ? row["BranchName"].ToString() : "";
                        marketModel = parameter[10].ToString() == "1" ? row["MarketModel"].ToString() : "";
                    }
                    else
                    {
                        rowIndex++;
                        totalarea();
                        rowIndex++;

                        dealer = row["CompanyName"].ToString();
                        dealercode = row["CompanyCode"].ToString();
                        area = row["Area"].ToString();
                        outlet = parameter[9].ToString() == "1" ? row["BranchName"].ToString() : "";
                        marketModel = parameter[10].ToString() == "1" ? row["MarketModel"].ToString() : "";
                        excelReport.SetCellValue(dealer.ToString(), 8 + rowIndex, 0, 1, dealerWidth, ExcelCellStyle.LeftBorderedStandard);
                    }

                    
                }

                printdata(row);

                #endregion
                //end foreach
            }


            if (parameter[9].ToString() == "1")
            {
                rowIndex++;
                excelReport.ReplaceRestorePoint("RestoreDealer", dealer.ToString(), 0, totalOutlet + 2, dealerWidth, ExcelCellStyle.LeftBorderedStandard); //, TextAlignment.MiddleLeft, Orientation.HORIZONTAL, Color.White, 9, FontStyle.Regular, true);
                printtotaldealer();
            }
            
            rowIndex++;
            totalarea();
            excelReport.CloseExcelFileWriter();
        }

    }
}