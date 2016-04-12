using SimDms.DataWarehouse.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;

namespace SimDms.DataWarehouse.Helpers
{
    
        public class OmRpSalRgs045Xcl
        {
            #region Properties

            System.Data.DataSet ds0;
            Dictionary<int, string> parameter = new Dictionary<int, string>();
            private string fileName = "";

            private string area, dealer,dealercode, outlet, year;
            private int dealerWidth, outletWidth, totalDetailWidth, totalWidth;
            private int startCount = 7;

            private Dictionary<int, decimal> listTotal = new Dictionary<int, decimal>();
            private Dictionary<int, decimal> listTotalJabodetabek = new Dictionary<int, decimal>();
            private Dictionary<int, decimal> listGrandTotal = new Dictionary<int, decimal>();
            private decimal total, totalCabang, totalCabangJabodetabek;

            private string national = "0", profitcenterName;
            private string groupNo = "";
            SysUserView user ;
            #endregion

            public OmRpSalRgs045Xcl(System.Data.DataSet ds, Dictionary<int, string> param, string saveFileName, string statusNational, string profitCenterName)
            {
                ds0 = ds;
                parameter = param;
                fileName = saveFileName;
                national = statusNational;
                profitcenterName = profitCenterName;
            }
            /// <summary>
            /// Total Below
            /// </summary>
            public void CreateReport(string pserver)
            {
                DataTable dtHeader = ds0.Tables[0];
                DataTable dtHeader1 = ds0.Tables[1];
                DataTable dtDetail = ds0.Tables[2];

                StringBuilder sb = new StringBuilder();
                //sb.Append("(by Area, Dealer ");
                sb.Append(parameter[9] == "1" ? ", OUTLET" : "");
                //sb.Append(" & TYPE");
                //sb.Append(")");
                string title = sb.ToString().StartsWith(",") ? sb.ToString().Remove(0, 1) : sb.ToString().Trim().StartsWith("&") ? sb.ToString().Trim().Remove(0, 1) : sb.ToString();

                //Pemetaan ukuran column
                dealerWidth = 1;
                outletWidth = parameter[9] == "1" ? 1 : 0;
                totalWidth = 1;
                totalDetailWidth = dealerWidth + outletWidth;
                int startColumnFinal = dealerWidth + outletWidth + totalWidth;

                int indentTitleDesc = 6;
                int rowIndex = 0;
                int headerWidth = 0;
                int rowDealerCount = 0;

                area = dtDetail.Rows[0]["Area"].ToString();
                dealer = dtDetail.Rows[0]["CompanyName"].ToString();
                dealercode = dtDetail.Rows[0]["CompanyCode"].ToString();
                groupNo = dtDetail.Rows[0]["GroupNo"].ToString();
                if (parameter[9] == "1") outlet = dtDetail.Rows[0]["BranchName"].ToString();
                year = dtDetail.Rows[0]["Year"].ToString();

                ExcelFileWriter excelReport = new ExcelFileWriter(fileName, dtDetail.Rows[0]["Year"].ToString(), "OmRpSalRgs045",pserver);
                CreateHeader(excelReport, rowIndex, title, indentTitleDesc, dtHeader, dtHeader1, year.ToString());
                int startColumn = parameter[9] == "1" ? 3 : 2;

                foreach (DataRow row in dtDetail.Rows)
                {
                    startColumn = parameter[9] == "1" ? 3 : 2;
                    if (row["Area"].ToString() == "NSDS" && row["CompanyCode"].ToString() == "NSDS") break;

                    if (year == row["Year"].ToString() &&
                        area != row["Area"].ToString() &&
                        (dealer != row["CompanyName"].ToString() &&
                        groupNo != row["GroupNo"].ToString())
                        )
                    {
                        #region satu
                        excelReport.ReplaceRestorePoint(ExcelRestorePoint.DealerRestorePoint.ToString(), dealer.ToString(), 0, rowDealerCount, 1, ExcelCellStyle.LeftBorderedStandardWrap);

                        if (area != "JABODETABEK__")
                            excelReport.SetCellValue("TOTAL " + area, 9 + rowIndex, 0, 1, dealerWidth + outletWidth, ExcelCellStyle.BlueTotal);
                        else
                            excelReport.SetCellValue("TOTAL DEALER " + area, 9 + rowIndex, 0, 1, dealerWidth + outletWidth, ExcelCellStyle.BlueTotal);


                        excelReport.SetCellValue(totalCabang.ToString(), 9 + rowIndex, 0 + dealerWidth + outletWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);

                        //Insert data detail
                        for (int i = startCount; i < row.ItemArray.Length - 1; i++)
                        {
                            excelReport.SetCellValue(listTotal[i].ToString(), 9 + rowIndex, startColumn, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                            startColumn++;
                        }

                        if (parameter[4].ToString() == "ALL" && parameter[6].ToString() == "ALL" && area == "JABODETABEK__")
                        {
                            rowIndex++;
                            startColumn = parameter[9] == "1" ? 3 : 2;
                            excelReport.SetCellValue("TOTAL JABODETABEK", 9 + rowIndex, 0, 1, dealerWidth + outletWidth, ExcelCellStyle.BlueTotal);

                            excelReport.SetCellValue(totalCabangJabodetabek.ToString(), 9 + rowIndex, 0 + dealerWidth + outletWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);

                            //Insert data detail
                            for (int i = startCount; i < row.ItemArray.Length - 1; i++)
                            {
                                excelReport.SetCellValue(listTotalJabodetabek[i].ToString(), 9 + rowIndex, startColumn, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                                startColumn++;
                            }
                        }

                        rowIndex++;
                        rowDealerCount = 0;
                        totalCabang = 0;
                        startColumn = parameter[9] == "1" ? 3 : 2;
                        excelReport.RestorePoint(9 + rowIndex, ExcelRestorePoint.DealerRestorePoint.ToString());
                        listTotal = new Dictionary<int, decimal>();
                        area = row["Area"].ToString();
                        dealer = row["CompanyName"].ToString();
                        dealercode = row["CompanyCode"].ToString();
                        groupNo = row["groupNo"].ToString();
                        #endregion
                    }
                    else if ((year == row["Year"].ToString() &&
                        area == row["Area"].ToString() &&
                        dealer != row["CompanyName"].ToString()) ||
                        (year == row["Year"].ToString() &&
                        area == row["Area"].ToString() &&
                        groupNo != row["groupNo"].ToString())
                        )
                    {
                        #region dua
                        excelReport.ReplaceRestorePoint(ExcelRestorePoint.DealerRestorePoint.ToString(), dealer.ToString(), 0, rowDealerCount, 1, ExcelCellStyle.LeftBorderedStandardWrap);
                        excelReport.RestorePoint(9 + rowIndex, ExcelRestorePoint.DealerRestorePoint.ToString());
                        dealer = row["CompanyName"].ToString();
                        dealercode = row["CompanyCode"].ToString();
                        groupNo = row["groupNo"].ToString();
                        rowDealerCount = 0; 
                        #endregion
                    }
                    else if (year != row["Year"].ToString())
                    {
                        if (parameter[5].ToString() == "ALL")
                        {
                            excelReport.ReplaceRestorePoint(ExcelRestorePoint.DealerRestorePoint.ToString(), dealer.ToString(), 0, rowDealerCount, 1, ExcelCellStyle.LeftBorderedStandardWrap);
                            excelReport.SetCellValue("TOTAL " + area, 9 + rowIndex, 0, 1, dealerWidth + outletWidth, ExcelCellStyle.BlueTotal);

                            excelReport.SetCellValue(totalCabang.ToString(), 9 + rowIndex, 0 + dealerWidth + outletWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);

                            //Insert data detail
                            for (int i = startCount; i < row.ItemArray.Length - 1; i++)
                            {
                                excelReport.SetCellValue(listTotal[i].ToString(), 9 + rowIndex, startColumn, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                                startColumn++;
                            }

                            rowIndex++;
                        }

                        #region Total Dealer
                        if (parameter[3].ToString() == "ALL")
                        {
                            excelReport.SetCellValue("TOTAL NATIONAL", 9 + rowIndex, 0, 1, dealerWidth + outletWidth, ExcelCellStyle.YellowTotal);

                            excelReport.SetCellValue(total.ToString(), 9 + rowIndex, 0 + dealerWidth + outletWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);

                            startColumnFinal = dealerWidth + outletWidth + totalWidth;

                            //Insert data detail
                            for (int i = startCount; i < listTotal.Count + startCount; i++)
                            {
                                excelReport.SetCellValue(listGrandTotal[i].ToString(), 9 + rowIndex, startColumnFinal, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
                                startColumnFinal++;
                            }
                        }
                        #endregion

                        rowDealerCount = 0;
                        totalCabang = 0;
                        startColumn = parameter[9] == "1" ? 3 : 2;
                        area = row["Area"].ToString();
                        dealer = row["CompanyName"].ToString();
                        dealercode = row["CompanyCode"].ToString();
                        groupNo = row["groupNo"].ToString();
                        if (parameter[9] == "1") outlet = row["BranchName"].ToString();
                        year = row["Year"].ToString();

                        excelReport.ChangeSheet(row["Year"].ToString());

                        CreateHeader(excelReport, rowIndex, title, indentTitleDesc, dtHeader, dtHeader1, year);

                        listTotal = new Dictionary<int, decimal>();
                        listGrandTotal = new Dictionary<int, decimal>();
                        total = totalCabang = 0;

                        rowIndex = 0;
                        excelReport.RestorePoint(9 + rowIndex, ExcelRestorePoint.DealerRestorePoint.ToString());

                    }
                    else if (rowIndex == 0)
                    {
                        excelReport.RestorePoint(9 + rowIndex, ExcelRestorePoint.DealerRestorePoint.ToString());
                    }
                    //excelReport.SetCellValue(row["CompanyName"].ToString(), 9 + rowIndex, 0, 1, 1, ExcelCellStyle.LeftBorderedStandardWrap);

                    if (parameter[9] == "1")
                    {
                        if (row["BranchName"].ToString().Contains("ZZZZ"))
                            excelReport.SetCellValue(row["BranchName"].ToString().Remove(0, 4), 9 + rowIndex, 0 + dealerWidth, 1, 1, ExcelCellStyle.LeftBorderedBold);
                        else
                            excelReport.SetCellValue(row["BranchName"].ToString(), 9 + rowIndex, 0 + dealerWidth, 1, 1, ExcelCellStyle.LeftBorderedStandardWrap);
                    }

                    if (parameter[9] == "1")
                    {
                        if (row["BranchName"].ToString().Contains("ZZZZ"))
                            excelReport.SetCellValue(row["Total"].ToString(), 9 + rowIndex, 0 + dealerWidth + outletWidth, 1, 1, ExcelCellStyle.RightBorderedBoldNumber, true);
                        else
                            excelReport.SetCellValue(row["Total"].ToString(), 9 + rowIndex, 0 + dealerWidth + outletWidth, 1, 1, ExcelCellStyle.RightBorderedStandardNumber, true);
                    }
                    else
                        excelReport.SetCellValue(row["Total"].ToString(), 9 + rowIndex, 0 + dealerWidth + outletWidth, 1, 1, ExcelCellStyle.RightBorderedStandardNumber, true);

                    if (parameter[9] != "1")
                    {
                        //Insert data detail
                        for (int i = startCount; i < row.ItemArray.Length - 1; i++)
                        {
                            excelReport.SetCellValue(row[i] != null ? row[i].ToString() : "0", 9 + rowIndex, startColumn, 1, 1, ExcelCellStyle.RightBorderedStandardNumber, true);
                            startColumn++;
                        }
                    }
                    else
                    {
                        if (row["BranchName"].ToString().Contains("ZZZZ"))
                        {
                            //Insert data detail
                            for (int i = startCount; i < row.ItemArray.Length - 1; i++)
                            {
                                excelReport.SetCellValue(row[i] != null ? row[i].ToString() : "0", 9 + rowIndex, startColumn, 1, 1, ExcelCellStyle.RightBorderedBoldNumber, true);
                                startColumn++;
                            }
                        }
                        else
                        {
                            //Insert data detail
                            for (int i = startCount; i < row.ItemArray.Length - 1; i++)
                            {
                                excelReport.SetCellValue(row[i] != null ? row[i].ToString() : "0", 9 + rowIndex, startColumn, 1, 1, ExcelCellStyle.RightBorderedStandardNumber, true);
                                startColumn++;
                            }
                        }
                    }

                    #region Save Total

                    // untuk menyimpan nilai total
                    for (int i = startCount; i < row.ItemArray.Length - 1; i++)
                    {
                        if (parameter[9] != "1")
                        {
                            if (!listTotal.ContainsKey(i))
                            {
                                if (parameter[9] == "1" && row[4].ToString() != "Total")
                                    listTotal.Add(i, row[i].ToString() != "" ? decimal.Parse(row[i].ToString()) : 0);
                                else if (parameter[9] == "1" && row[4].ToString() == "Total")
                                    listTotal.Add(i, 0);
                                else if (parameter[9] == "0")
                                    listTotal.Add(i, row[i].ToString() != "" ? decimal.Parse(row[i].ToString()) : 0);
                            }
                            else
                            {
                                decimal tempValue = listTotal[i];
                                decimal temp = row[i].ToString() != "" ? decimal.Parse(row[i].ToString()) : 0;
                                listTotal[i] = tempValue + temp;
                            }

                            if (!listTotalJabodetabek.ContainsKey(i))
                            {
                                if (parameter[9] == "1" && row[4].ToString() != "Total")
                                    listTotalJabodetabek.Add(i, row[i].ToString() != "" ? decimal.Parse(row[i].ToString()) : 0);
                                else if (parameter[9] == "1" && row[4].ToString() == "Total")
                                    listTotalJabodetabek.Add(i, 0);
                                else if (parameter[9] == "0")
                                    listTotalJabodetabek.Add(i, row[i].ToString() != "" ? decimal.Parse(row[i].ToString()) : 0);
                            }
                            else
                            {
                                decimal tempValue = listTotalJabodetabek[i];
                                decimal temp = row[i].ToString() != "" ? decimal.Parse(row[i].ToString()) : 0;
                                listTotalJabodetabek[i] = tempValue + temp;
                            }

                            if (!listGrandTotal.ContainsKey(i))
                            {
                                if (parameter[9] == "1" && row[4].ToString() != "Total")
                                    listGrandTotal.Add(i, row[i].ToString() != "" ? decimal.Parse(row[i].ToString()) : 0);
                                else if (parameter[9] == "1" && row[4].ToString() == "Total")
                                    listGrandTotal.Add(i, 0);
                                else if (parameter[9] == "0")
                                    listGrandTotal.Add(i, row[i].ToString() != "" ? decimal.Parse(row[i].ToString()) : 0);
                            }
                            else
                            {
                                decimal temtGValue = listGrandTotal[i];
                                decimal tempG = row[i].ToString() != "" ? decimal.Parse(row[i].ToString()) : 0;
                                listGrandTotal[i] = temtGValue + tempG;
                            }
                        }
                        else
                        {
                            if (!row["BranchName"].ToString().Contains("Total"))
                            {
                                if (!listTotal.ContainsKey(i))
                                {
                                    if (parameter[9] == "1" && row[4].ToString() != "Total")
                                        listTotal.Add(i, row[i].ToString() != "" ? decimal.Parse(row[i].ToString()) : 0);
                                    else if (parameter[9] == "1" && row[4].ToString() == "Total")
                                        listTotal.Add(i, 0);
                                    else if (parameter[9] == "0")
                                        listTotal.Add(i, row[i].ToString() != "" ? decimal.Parse(row[i].ToString()) : 0);
                                }
                                else
                                {
                                    decimal tempValue = listTotal[i];
                                    decimal temp = row[i].ToString() != "" ? decimal.Parse(row[i].ToString()) : 0;
                                    listTotal[i] = tempValue + temp;
                                }

                                if (!listTotalJabodetabek.ContainsKey(i))
                                {
                                    if (parameter[9] == "1" && row[4].ToString() != "Total")
                                        listTotalJabodetabek.Add(i, row[i].ToString() != "" ? decimal.Parse(row[i].ToString()) : 0);
                                    else if (parameter[9] == "1" && row[4].ToString() == "Total")
                                        listTotalJabodetabek.Add(i, 0);
                                    else if (parameter[9] == "0")
                                        listTotalJabodetabek.Add(i, row[i].ToString() != "" ? decimal.Parse(row[i].ToString()) : 0);
                                }
                                else
                                {
                                    decimal tempValue = listTotalJabodetabek[i];
                                    decimal temp = row[i].ToString() != "" ? decimal.Parse(row[i].ToString()) : 0;
                                    listTotalJabodetabek[i] = tempValue + temp;
                                }

                                if (!listGrandTotal.ContainsKey(i))
                                {
                                    if (parameter[9] == "1" && row[4].ToString() != "Total")
                                        listGrandTotal.Add(i, row[i].ToString() != "" ? decimal.Parse(row[i].ToString()) : 0);
                                    else if (parameter[9] == "1" && row[4].ToString() == "Total")
                                        listGrandTotal.Add(i, 0);
                                    else if (parameter[9] == "0")
                                        listGrandTotal.Add(i, row[i].ToString() != "" ? decimal.Parse(row[i].ToString()) : 0);
                                }
                                else
                                {
                                    decimal temtGValue = listGrandTotal[i];
                                    decimal tempG = row[i].ToString() != "" ? decimal.Parse(row[i].ToString()) : 0;
                                    listGrandTotal[i] = temtGValue + tempG;
                                }
                            }
                        }
                    }

                    if (row["BranchName"].ToString().Contains("Total"))
                    {
                        totalCabang += decimal.Parse(row["Total"].ToString());
                        totalCabangJabodetabek += decimal.Parse(row["Total"].ToString());
                        total += decimal.Parse(row["Total"].ToString());
                    }

                    #endregion

                    rowIndex++;
                    rowDealerCount++;
                }

                excelReport.ReplaceRestorePoint(ExcelRestorePoint.DealerRestorePoint.ToString(), dealer.ToString(), 0, rowDealerCount, 1, ExcelCellStyle.LeftBorderedStandardWrap);
                startColumnFinal = dealerWidth + outletWidth + totalWidth;

                #region Total Area
                if (parameter[5].ToString() == "ALL")
                {
                    if (national == "0")
                    {
                        if (area != "JABODETABEK__")
                            excelReport.SetCellValue("TOTAL " + area, 9 + rowIndex, 0, 1, dealerWidth + outletWidth, ExcelCellStyle.BlueTotal);
                        else
                            excelReport.SetCellValue("TOTAL DEALER " + area, 9 + rowIndex, 0, 1, dealerWidth + outletWidth, ExcelCellStyle.BlueTotal);

                        excelReport.SetCellValue(totalCabang.ToString(), 9 + rowIndex, 0 + dealerWidth + outletWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);

                        //Insert data detail
                        for (int i = startCount; i < listTotal.Count + startCount; i++)
                        {
                            excelReport.SetCellValue(listTotal[i].ToString(), 9 + rowIndex, startColumnFinal, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                            startColumnFinal++;
                        }

                        rowIndex++;
                    }
                }

                startColumnFinal = dealerWidth + outletWidth + totalWidth;
                if (parameter[4].ToString() == "ALL" && parameter[6].ToString() == "ALL" && area == "JABODETABEK__")
                {
                    startColumn = parameter[9] == "1" ? 3 : 2;
                    excelReport.SetCellValue("TOTAL JABODETABEK", 9 + rowIndex, 0, 1, dealerWidth + outletWidth, ExcelCellStyle.BlueTotal);

                    excelReport.SetCellValue(totalCabangJabodetabek.ToString(), 9 + rowIndex, 0 + dealerWidth + outletWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);

                    //Insert data detail
                    for (int i = startCount; i < listTotalJabodetabek.Count + startCount; i++)
                    {
                        excelReport.SetCellValue(listTotalJabodetabek[i].ToString(), 9 + rowIndex, startColumnFinal, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                        startColumnFinal++;
                    }

                    rowIndex++;
                }
                #endregion

                #region Total SIS
                if (parameter[10].ToString() == "RETAIL" || parameter[10].ToString() == "SALES" || parameter[10].ToString() == "FPOL")
                {
                    //excelReport.SetCellValue("TOTAL SIS RETAIL SALES", 9 + rowIndex, 0, 1, dealerWidth + outletWidth, ExcelCellStyle.PinkTotal);

                    //excelReport.SetCellValue(dtDetail.Rows[dtDetail.Rows.Count - 1]["Total"].ToString(), 9 + rowIndex, 0 + dealerWidth + outletWidth, 1, 1, ExcelCellStyle.PinkTotalNumber, true);

                    //startColumnFinal = dealerWidth + outletWidth + totalWidth;
                    //total += decimal.Parse(dtDetail.Rows[dtDetail.Rows.Count - 1]["Total"].ToString());

                    ////Insert data detail
                    //for (int i = startCount; i < listTotal.Count + startCount; i++)
                    //{
                    //    excelReport.SetCellValue(dtDetail.Rows[dtDetail.Rows.Count - 1][i].ToString(), 9 + rowIndex, startColumnFinal, 1, 1, ExcelCellStyle.PinkTotalNumber, true);
                    //    startColumnFinal++;

                    //    decimal temtGValue = listGrandTotal[i];
                    //    decimal tempG = Convert.ToDecimal(dtDetail.Rows[dtDetail.Rows.Count - 1][i].ToString());
                    //    listGrandTotal[i] = temtGValue + tempG;
                    //}
                    //rowIndex++;
                }
                #endregion

                #region Total Dealer
                if (parameter[3].ToString() == "ALL")
                {
                    //excelReport.SetCellValue("TOTAL NATIONAL", 9 + rowIndex, 0, 1, dealerWidth + outletWidth, ExcelCellStyle.YellowTotal);

                    //excelReport.SetCellValue(total.ToString(), 9 + rowIndex, 0 + dealerWidth + outletWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);

                    //startColumnFinal = dealerWidth + outletWidth + totalWidth;

                    ////Insert data detail
                    //for (int i = startCount; i < listTotal.Count + startCount; i++)
                    //{
                    //    excelReport.SetCellValue(listGrandTotal[i].ToString(), 9 + rowIndex, startColumnFinal, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
                    //    startColumnFinal++;
                    //}

                    //rowIndex++;
                }
                #endregion


                //if (national == "1")
                //{
                    #region Direct
                    //excelReport.SetCellValue("DIRECT", 9 + rowIndex, 0, 1, dealerWidth + outletWidth, ExcelCellStyle.PurpleTotal);

                    //startColumnFinal = dealerWidth + outletWidth;

                    ////Insert data detail
                    //for (int i = 5; i < listTotal.Count + 5; i++)
                    //{
                    //    excelReport.SetCellValue(listTotal[i].ToString(), 9 + rowIndex, startColumnFinal, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                    //    startColumnFinal++;
                    //}

                    //rowIndex++;
                    #endregion

                    #region Sim/Sis
                    //excelReport.SetCellValue("SIM/SIS", 9 + rowIndex, 0, 1, dealerWidth + outletWidth, ExcelCellStyle.PinkTotal);

                    //startColumnFinal = dealerWidth + outletWidth;

                    ////Insert data detail
                    //for (int i = 5; i < listTotal.Count + 5; i++)
                    //{
                    //excelReport.SetCellValue(listTotal[i].ToString(), 9 + rowIndex, startColumnFinal, 1, 1, ExcelCellStyle.PinkTotalNumber, true);
                    //    startColumnFinal++;
                    //}

                    //rowIndex++;
                    #endregion

                    #region Grand Total II
                    excelReport.SetCellValue("GRAND TOTAL II", 9 + rowIndex, 0, 1, dealerWidth + outletWidth, ExcelCellStyle.PurpleTotal);
                    excelReport.SetCellValue(total.ToString(), 9 + rowIndex, 0 + dealerWidth + outletWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);

                    startColumnFinal = dealerWidth + outletWidth + totalWidth;

                    //Insert data detail
                    for (int i = startCount; i < listTotal.Count + startCount; i++)
                    {
                        excelReport.SetCellValue(listGrandTotal[i].ToString(), 9 + rowIndex, startColumnFinal, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                        startColumnFinal++;
                    }
                    #endregion
                //}

                excelReport.CloseExcelFileWriter();
            }

            /// <summary>
            /// Total Up
            /// </summary>
            public void TopCreateReport(string pserver)
            {
                DataTable dtHeader = ds0.Tables[0];
                DataTable dtHeader1 = ds0.Tables[1];
                DataTable dtDetail = ds0.Tables[2];

                StringBuilder sb = new StringBuilder();
                //sb.Append("(by Area, Dealer ");
                sb.Append(parameter[9] == "1" ? ", OUTLET" : "");
                sb.Append(" & TYPE");
                //sb.Append(")");
                string title = sb.ToString().StartsWith(",") ? sb.ToString().Remove(0, 1) : sb.ToString().Trim().StartsWith("&") ? sb.ToString().Trim().Remove(0, 1) : sb.ToString();

                //Pemetaan ukuran column
                dealerWidth = 1;
                outletWidth = parameter[9] == "1" ? 1 : 0;
                totalWidth = 1;
                totalDetailWidth = dealerWidth + outletWidth;
                int startColumnFinal = dealerWidth + outletWidth + totalWidth;

                int indentTitleDesc = 6;
                int rowIndex = 0;
                int headerWidth = 0;
                int rowDealerCount = 0;

                area = dtDetail.Rows[0]["Area"].ToString();
                dealer = dtDetail.Rows[0]["CompanyName"].ToString();
                dealercode = dtDetail.Rows[0]["CompanyCode"].ToString();
                if (parameter[9] == "1") outlet = dtDetail.Rows[0]["BranchName"].ToString();
                year = dtDetail.Rows[0]["Year"].ToString();

                ExcelFileWriter excelReport = new ExcelFileWriter(fileName, dtDetail.Rows[0]["Year"].ToString(), "OmRpSalRgs045", pserver);
                CreateHeader(excelReport, rowIndex, title, indentTitleDesc, dtHeader, dtHeader1, year.ToString());

                foreach (DataRow row in dtDetail.Rows)
                {
                    int startColumn = parameter[9] == "1" ? 3 : 2;

                    if (year == row["Year"].ToString() &&
                        area != row["Area"].ToString() &&
                        dealer != row["CompanyName"].ToString())
                    {
                        excelReport.ReplaceRestorePoint(ExcelRestorePoint.DealerRestorePoint.ToString(), dealer.ToString(), 0, rowDealerCount, 1, ExcelCellStyle.LeftBorderedStandardWrap);

                        if (area != "JABODETABEK__")
                            excelReport.SetCellValue("TOTAL " + area, 9 + rowIndex, 0, 1, dealerWidth + outletWidth, ExcelCellStyle.BlueTotal);
                        else
                            excelReport.SetCellValue("TOTAL DEALER " + area, 9 + rowIndex, 0, 1, dealerWidth + outletWidth, ExcelCellStyle.BlueTotal);


                        excelReport.SetCellValue(totalCabang.ToString(), 9 + rowIndex, 0 + dealerWidth + outletWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);

                        //Insert data detail
                        for (int i = startCount; i < row.ItemArray.Length - 1; i++)
                        {
                            excelReport.SetCellValue(listTotal[i].ToString(), 9 + rowIndex, startColumn, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                            startColumn++;
                        }

                        if (parameter[2].ToString() == "ALL" && parameter[4].ToString() == "ALL" && parameter[6].ToString() == "ALL" && area == "JABODETABEK")
                        {
                            rowIndex++;
                            startColumn = parameter[9] == "1" ? 3 : 2;
                            excelReport.SetCellValue("TOTAL JABODETABEK", 9 + rowIndex, 0, 1, dealerWidth + outletWidth, ExcelCellStyle.BlueTotal);

                            excelReport.SetCellValue(totalCabangJabodetabek.ToString(), 9 + rowIndex, 0 + dealerWidth + outletWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);

                            //Insert data detail
                            for (int i = startCount; i < row.ItemArray.Length - 1; i++)
                            {
                                excelReport.SetCellValue(listTotalJabodetabek[i].ToString(), 9 + rowIndex, startColumn, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                                startColumn++;
                            }
                        }

                        rowIndex++;
                        rowDealerCount = 0;
                        totalCabang = 0;
                        startColumn = parameter[9] == "1" ? 3 : 2;
                        excelReport.RestorePoint(9 + rowIndex, ExcelRestorePoint.DealerRestorePoint.ToString());
                        listTotal = new Dictionary<int, decimal>();
                        area = row["Area"].ToString();
                        dealer = row["CompanyName"].ToString();
                        dealercode = row["CompanyCode"].ToString();
                        groupNo = row["groupNo"].ToString();
                    }
                    else if (year == row["Year"].ToString() &&
                        area == row["Area"].ToString() &&
                        dealer != row["CompanyName"].ToString() && 
                        dealercode !=row["CompanyCode"].ToString()
                        )
                    {
                        excelReport.ReplaceRestorePoint(ExcelRestorePoint.DealerRestorePoint.ToString(), dealer.ToString(), 0, rowDealerCount, 1, ExcelCellStyle.LeftBorderedStandardWrap);
                        excelReport.RestorePoint(9 + rowIndex, ExcelRestorePoint.DealerRestorePoint.ToString());
                        dealer = row["CompanyName"].ToString();
                        dealercode = row["CompanyCode"].ToString();
                        groupNo = row["groupNo"].ToString();
                        rowDealerCount = 0;
                    }
                    else if (year != row["Year"].ToString())
                    {
                        if (parameter[5].ToString() == "ALL")
                        {
                            excelReport.ReplaceRestorePoint(ExcelRestorePoint.DealerRestorePoint.ToString(), dealer.ToString(), 0, rowDealerCount, 1, ExcelCellStyle.LeftBorderedStandardWrap);
                            excelReport.SetCellValue("TOTAL " + area, 9 + rowIndex, 0, 1, dealerWidth + outletWidth, ExcelCellStyle.BlueTotal);

                            excelReport.SetCellValue(totalCabang.ToString(), 9 + rowIndex, 0 + dealerWidth + outletWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);

                            //Insert data detail
                            for (int i = startCount; i < row.ItemArray.Length - 1; i++)
                            {
                                excelReport.SetCellValue(listTotal[i].ToString(), 9 + rowIndex, startColumn, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                                startColumn++;
                            }

                            rowIndex++;
                        }

                        #region Total Dealer
                        if (parameter[3].ToString() == "ALL")
                        {
                            excelReport.SetCellValue("TOTAL NATIONAL", 9 + rowIndex, 0, 1, dealerWidth + outletWidth, ExcelCellStyle.YellowTotal);

                            excelReport.SetCellValue(total.ToString(), 9 + rowIndex, 0 + dealerWidth + outletWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);

                            startColumnFinal = dealerWidth + outletWidth + totalWidth;

                            //Insert data detail
                            for (int i = startCount; i < listTotal.Count + startCount; i++)
                            {
                                excelReport.SetCellValue(listGrandTotal[i].ToString(), 9 + rowIndex, startColumnFinal, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
                                startColumnFinal++;
                            }
                        }
                        #endregion

                        rowDealerCount = 0;
                        totalCabang = 0;
                        startColumn = parameter[9] == "1" ? 3 : 2;
                        area = row["Area"].ToString();
                        dealer = row["CompanyName"].ToString();
                        dealercode = row["CompanyCode"].ToString();
                        groupNo = row["groupNo"].ToString();
                        if (parameter[9] == "1") outlet = row["BranchName"].ToString();
                        year = row["Year"].ToString();

                        excelReport.ChangeSheet(row["Year"].ToString());

                        CreateHeader(excelReport, rowIndex, title, indentTitleDesc, dtHeader, dtHeader1, year);

                        listTotal = new Dictionary<int, decimal>();
                        listGrandTotal = new Dictionary<int, decimal>();
                        total = totalCabang = 0;

                        rowIndex = 0;
                        excelReport.RestorePoint(9 + rowIndex, ExcelRestorePoint.DealerRestorePoint.ToString());

                    }
                    else if (rowIndex == 0)
                    {
                        excelReport.RestorePoint(9 + rowIndex, ExcelRestorePoint.DealerRestorePoint.ToString());
                    }

                    //excelReport.SetCellValue(row["CompanyName"].ToString(), 9 + rowIndex, 0, 1, 1, ExcelCellStyle.LeftBorderedStandardWrap);

                    if (parameter[9] == "1")
                    {
                        if (row["BranchName"].ToString().Contains("0000"))
                            excelReport.SetCellValue(row["BranchName"].ToString().Remove(0, 4), 9 + rowIndex, 0 + dealerWidth, 1, 1, ExcelCellStyle.LeftBorderedBold);
                        else
                            excelReport.SetCellValue(row["BranchName"].ToString(), 9 + rowIndex, 0 + dealerWidth, 1, 1, ExcelCellStyle.LeftBorderedStandardWrap);
                    }

                    if (parameter[9] == "1")
                    {
                        if (row["BranchName"].ToString().Contains("0000"))
                            excelReport.SetCellValue(row["Total"].ToString(), 9 + rowIndex, 0 + dealerWidth + outletWidth, 1, 1, ExcelCellStyle.RightBorderedBoldNumber, true);
                        else
                            excelReport.SetCellValue(row["Total"].ToString(), 9 + rowIndex, 0 + dealerWidth + outletWidth, 1, 1, ExcelCellStyle.RightBorderedStandardNumber, true);
                    }
                    else
                        excelReport.SetCellValue(row["Total"].ToString(), 9 + rowIndex, 0 + dealerWidth + outletWidth, 1, 1, ExcelCellStyle.RightBorderedStandardNumber, true);

                    if (parameter[9] != "1")
                    {
                        //Insert data detail
                        for (int i = startCount; i < row.ItemArray.Length - 1; i++)
                        {
                            excelReport.SetCellValue(row[i] != null ? row[i].ToString() : "0", 9 + rowIndex, startColumn, 1, 1, ExcelCellStyle.RightBorderedStandardNumber, true);
                            startColumn++;
                        }
                    }
                    else
                    {
                        if (row["BranchName"].ToString().Contains("0000"))
                        {
                            //Insert data detail
                            for (int i = startCount; i < row.ItemArray.Length - 1; i++)
                            {
                                excelReport.SetCellValue(row[i] != null ? row[i].ToString() : "0", 9 + rowIndex, startColumn, 1, 1, ExcelCellStyle.RightBorderedBoldNumber, true);
                                startColumn++;
                            }
                        }
                        else
                        {
                            //Insert data detail
                            for (int i = startCount; i < row.ItemArray.Length - 1; i++)
                            {
                                excelReport.SetCellValue(row[i] != null ? row[i].ToString() : "0", 9 + rowIndex, startColumn, 1, 1, ExcelCellStyle.RightBorderedStandardNumber, true);
                                startColumn++;
                            }
                        }
                    }

                    #region Save Total

                    // untuk menyimpan nilai total
                    for (int i = startCount; i < row.ItemArray.Length - 1; i++)
                    {
                        if (parameter[9] != "1")
                        {
                            if (!listTotal.ContainsKey(i))
                            {
                                if (parameter[9] == "1" && row[4].ToString() != "Total")
                                    listTotal.Add(i, row[i].ToString() != "" ? decimal.Parse(row[i].ToString()) : 0);
                                else if (parameter[9] == "1" && row[4].ToString() == "Total")
                                    listTotal.Add(i, 0);
                                else if (parameter[9] == "0")
                                    listTotal.Add(i, row[i].ToString() != "" ? decimal.Parse(row[i].ToString()) : 0);
                            }
                            else
                            {
                                decimal tempValue = listTotal[i];
                                decimal temp = row[i].ToString() != "" ? decimal.Parse(row[i].ToString()) : 0;
                                listTotal[i] = tempValue + temp;
                            }

                            if (!listTotalJabodetabek.ContainsKey(i))
                            {
                                if (parameter[9] == "1" && row[4].ToString() != "Total")
                                    listTotalJabodetabek.Add(i, row[i].ToString() != "" ? decimal.Parse(row[i].ToString()) : 0);
                                else if (parameter[9] == "1" && row[4].ToString() == "Total")
                                    listTotalJabodetabek.Add(i, 0);
                                else if (parameter[9] == "0")
                                    listTotalJabodetabek.Add(i, row[i].ToString() != "" ? decimal.Parse(row[i].ToString()) : 0);
                            }
                            else
                            {
                                decimal tempValue = listTotalJabodetabek[i];
                                decimal temp = row[i].ToString() != "" ? decimal.Parse(row[i].ToString()) : 0;
                                listTotalJabodetabek[i] = tempValue + temp;
                            }

                            if (!listGrandTotal.ContainsKey(i))
                            {
                                if (parameter[9] == "1" && row[4].ToString() != "Total")
                                    listGrandTotal.Add(i, row[i].ToString() != "" ? decimal.Parse(row[i].ToString()) : 0);
                                else if (parameter[9] == "1" && row[4].ToString() == "Total")
                                    listGrandTotal.Add(i, 0);
                                else if (parameter[9] == "0")
                                    listGrandTotal.Add(i, row[i].ToString() != "" ? decimal.Parse(row[i].ToString()) : 0);
                            }
                            else
                            {
                                decimal temtGValue = listGrandTotal[i];
                                decimal tempG = row[i].ToString() != "" ? decimal.Parse(row[i].ToString()) : 0;
                                listGrandTotal[i] = temtGValue + tempG;
                            }
                        }
                        else
                        {
                            if (!row["BranchName"].ToString().Contains("Total"))
                            {
                                if (!listTotal.ContainsKey(i))
                                {
                                    if (parameter[9] == "1" && row[4].ToString() != "Total")
                                        listTotal.Add(i, row[i].ToString() != "" ? decimal.Parse(row[i].ToString()) : 0);
                                    else if (parameter[9] == "1" && row[4].ToString() == "Total")
                                        listTotal.Add(i, 0);
                                    else if (parameter[9] == "0")
                                        listTotal.Add(i, row[i].ToString() != "" ? decimal.Parse(row[i].ToString()) : 0);
                                }
                                else
                                {
                                    decimal tempValue = listTotal[i];
                                    decimal temp = row[i].ToString() != "" ? decimal.Parse(row[i].ToString()) : 0;
                                    listTotal[i] = tempValue + temp;
                                }

                                if (!listTotalJabodetabek.ContainsKey(i))
                                {
                                    if (parameter[9] == "1" && row[4].ToString() != "Total")
                                        listTotalJabodetabek.Add(i, row[i].ToString() != "" ? decimal.Parse(row[i].ToString()) : 0);
                                    else if (parameter[9] == "1" && row[4].ToString() == "Total")
                                        listTotalJabodetabek.Add(i, 0);
                                    else if (parameter[9] == "0")
                                        listTotalJabodetabek.Add(i, row[i].ToString() != "" ? decimal.Parse(row[i].ToString()) : 0);
                                }
                                else
                                {
                                    decimal tempValue = listTotalJabodetabek[i];
                                    decimal temp = row[i].ToString() != "" ? decimal.Parse(row[i].ToString()) : 0;
                                    listTotalJabodetabek[i] = tempValue + temp;
                                }

                                if (!listGrandTotal.ContainsKey(i))
                                {
                                    if (parameter[9] == "1" && row[4].ToString() != "Total")
                                        listGrandTotal.Add(i, row[i].ToString() != "" ? decimal.Parse(row[i].ToString()) : 0);
                                    else if (parameter[9] == "1" && row[4].ToString() == "Total")
                                        listGrandTotal.Add(i, 0);
                                    else if (parameter[9] == "0")
                                        listGrandTotal.Add(i, row[i].ToString() != "" ? decimal.Parse(row[i].ToString()) : 0);
                                }
                                else
                                {
                                    decimal temtGValue = listGrandTotal[i];
                                    decimal tempG = row[i].ToString() != "" ? decimal.Parse(row[i].ToString()) : 0;
                                    listGrandTotal[i] = temtGValue + tempG;
                                }
                            }
                        }
                    }

                    if (row["BranchName"].ToString().Contains("Total"))
                    {
                        totalCabang += decimal.Parse(row["Total"].ToString());
                        totalCabangJabodetabek += decimal.Parse(row["Total"].ToString());
                        total += decimal.Parse(row["Total"].ToString());
                    }

                    #endregion

                    rowIndex++;
                    rowDealerCount++;
                }

                excelReport.ReplaceRestorePoint(ExcelRestorePoint.DealerRestorePoint.ToString(), dealer.ToString(), 0, rowDealerCount, 1, ExcelCellStyle.LeftBorderedStandardWrap);
                startColumnFinal = dealerWidth + outletWidth + totalWidth;

                #region Total Area
                if (parameter[5].ToString() == "ALL")
                {
                    if (national == "1")
                    {
                        excelReport.SetCellValue("TOTAL " + area, 9 + rowIndex, 0, 1, dealerWidth + outletWidth, ExcelCellStyle.BlueTotal);
                        excelReport.SetCellValue(totalCabang.ToString(), 9 + rowIndex, 0 + dealerWidth + outletWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);

                        //Insert data detail
                        for (int i = startCount; i < listTotal.Count + startCount; i++)
                        {
                            excelReport.SetCellValue(listTotal[i].ToString(), 9 + rowIndex, startColumnFinal, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
                            startColumnFinal++;
                        }

                        rowIndex++;
                    }
                }
                #endregion

                #region Total Dealer
                if (parameter[3].ToString() == "ALL")
                {
                    excelReport.SetCellValue("TOTAL NATIONAL", 9 + rowIndex, 0, 1, dealerWidth + outletWidth, ExcelCellStyle.YellowTotal);

                    excelReport.SetCellValue(total.ToString(), 9 + rowIndex, 0 + dealerWidth + outletWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);

                    startColumnFinal = dealerWidth + outletWidth + totalWidth;

                    //Insert data detail
                    for (int i = startCount; i < listTotal.Count + startCount; i++)
                    {
                        excelReport.SetCellValue(listGrandTotal[i].ToString(), 9 + rowIndex, startColumnFinal, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
                        startColumnFinal++;
                    }

                    rowIndex++;
                }
                #endregion
                if (national == "1")
                {
                    #region Direct
                    //excelReport.SetCellValue("DIRECT", 9 + rowIndex, 0, 1, dealerWidth + outletWidth, ExcelCellStyle.PurpleTotal);

                    //startColumnFinal = dealerWidth + outletWidth;

                    ////Insert data detail
                    //for (int i = 5; i < listTotal.Count + 5; i++)
                    //{
                    //    excelReport.SetCellValue(listTotal[i].ToString(), 9 + rowIndex, startColumnFinal, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                    //    startColumnFinal++;
                    //}

                    //rowIndex++;
                    #endregion

                    #region Sim/Sis
                    //excelReport.SetCellValue("SIM/SIS", 9 + rowIndex, 0, 1, dealerWidth + outletWidth, ExcelCellStyle.PinkTotal);

                    //startColumnFinal = dealerWidth + outletWidth;

                    ////Insert data detail
                    //for (int i = 5; i < listTotal.Count + 5; i++)
                    //{
                    //excelReport.SetCellValue(listTotal[i].ToString(), 9 + rowIndex, startColumnFinal, 1, 1, ExcelCellStyle.PinkTotalNumber, true);
                    //    startColumnFinal++;
                    //}

                    //rowIndex++;
                    #endregion

                    #region Grand Total II
                    //excelReport.SetCellValue("GRAND TOTAL II", 9 + rowIndex, 0, 1, dealerWidth + outletWidth, ExcelCellStyle.PurpleTotal);
                    //excelReport.SetCellValue(total.ToString(), 9 + rowIndex, 0 + dealerWidth + outletWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);

                    //startColumnFinal = dealerWidth + outletWidth + totalWidth;

                    ////Insert data detail
                    //for (int i = startCount; i < listTotal.Count + startCount; i++)
                    //{
                    //    excelReport.SetCellValue(listGrandTotal[i].ToString(), 9 + rowIndex, startColumnFinal, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                    //    startColumnFinal++;
                    //}
                    #endregion
                }

                excelReport.CloseExcelFileWriter();
            }

            public void CreateHeader(ExcelFileWriter excelReport, int rowIndex, string title, int indentTitleDesc, DataTable dtHeader, DataTable dtHeader1, string year)
            {
                excelReport.SettingColumnWidth(2); //dealer
                if (parameter[9] == "1") excelReport.SettingColumnWidth(4);// Outlet
                excelReport.SettingColumnWidth(1);

                excelReport.SetCellValue("SALES REPORT ", 0, 0, 1, dealerWidth + outletWidth + indentTitleDesc + indentTitleDesc + 3, ExcelCellStyle.Header, false, "20");
                excelReport.SetCellValue(profitcenterName + " - " + parameter[11].ToString() + " " + parameter[10].ToString() + " , " + title, 1, 0, 1, dealerWidth + outletWidth + indentTitleDesc + indentTitleDesc + 3, ExcelCellStyle.Header2, false);
                excelReport.SetCellValue("Periode ", 2, 0, 1, dealerWidth, ExcelCellStyle.LeftBold);
                excelReport.SetCellValue(": " + parameter[1], 2, dealerWidth, 1, indentTitleDesc, ExcelCellStyle.LeftBold);
                excelReport.SetCellValue("Branch Manager", 2, dealerWidth + outletWidth + indentTitleDesc, 1, 3, ExcelCellStyle.LeftBold);
                excelReport.SetCellValue(": " + parameter[2], 2, dealerWidth + outletWidth + indentTitleDesc + 3, 1, indentTitleDesc, ExcelCellStyle.LeftBold);

                excelReport.SetCellValue("Area", 3, 0, 1, dealerWidth, ExcelCellStyle.LeftBold);
                excelReport.SetCellValue(": " + parameter[3], 3, dealerWidth, 1, indentTitleDesc, ExcelCellStyle.LeftBold);
                excelReport.SetCellValue("Sales Head", 3, dealerWidth + outletWidth + indentTitleDesc, 1, 3, ExcelCellStyle.LeftBold);
                excelReport.SetCellValue(": " + parameter[4], 3, dealerWidth + outletWidth + indentTitleDesc + 3, 1, indentTitleDesc, ExcelCellStyle.LeftBold);

                excelReport.SetCellValue("Dealer", 4, 0, 1, dealerWidth, ExcelCellStyle.LeftBold);
                excelReport.SetCellValue(": " + parameter[5], 4, dealerWidth, 1, indentTitleDesc, ExcelCellStyle.LeftBold);
                excelReport.SetCellValue("Sales Coordinator", 4, dealerWidth + outletWidth + indentTitleDesc, 1, 3, ExcelCellStyle.LeftBold);
                excelReport.SetCellValue(": " + parameter[6], 4, dealerWidth + outletWidth + indentTitleDesc + 3, 1, indentTitleDesc, ExcelCellStyle.LeftBold);

                excelReport.SetCellValue("Outlet", 5, 0, 1, dealerWidth, ExcelCellStyle.LeftBold);
                excelReport.SetCellValue(": " + parameter[7], 5, dealerWidth, 1, indentTitleDesc, ExcelCellStyle.LeftBold);
                excelReport.SetCellValue("Wiraniaga", 5, dealerWidth + outletWidth + indentTitleDesc, 1, 3, ExcelCellStyle.LeftBold);
                excelReport.SetCellValue(": " + parameter[8], 5, dealerWidth + outletWidth + indentTitleDesc + 3, 1, indentTitleDesc, ExcelCellStyle.LeftBold);

                excelReport.SetCellValue("Dealer", 7, 0, 2, dealerWidth, ExcelCellStyle.CenterBorderedBold);
                if (parameter[9] == "1")
                    excelReport.SetCellValue("Outlet", 7, 0 + dealerWidth, 2, outletWidth, ExcelCellStyle.CenterBorderedBold);

                excelReport.SetCellValue("Total", 7, 0 + dealerWidth + outletWidth, 2, 1, ExcelCellStyle.CenterBorderedBold);

                int headerWidth = 0;
                int rowDealerCount = 0;
                // Coding buat Header
                foreach (DataRow row in dtHeader.Rows)
                {
                    excelReport.SetCellValue(row["GroupMarketModel"].ToString(), 7, 0 + dealerWidth + outletWidth + totalWidth + headerWidth, 1, int.Parse(row["Count"].ToString()), ExcelCellStyle.CenterBorderedBold);                    
                    headerWidth += int.Parse(row["Count"].ToString());
                }

                int headerDtlWidth = 0;
                //Coding buat header detailnya
                foreach (DataRow row in dtHeader1.Rows)
                {
                    excelReport.SetCellValue(row["ColumnMarketModel"].ToString(), 8, 0 + dealerWidth + outletWidth + totalWidth + headerDtlWidth, 1, 1, ExcelCellStyle.CenterBorderedBold);
                    headerDtlWidth++;
                }
            }
        }
        public enum ExcelRestorePoint { DealerRestorePoint };
}