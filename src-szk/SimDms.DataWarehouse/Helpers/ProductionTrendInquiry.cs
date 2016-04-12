using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.ComponentModel;
using System.Web;
using System.Windows;
using System.Data;

namespace SimDms.DataWarehouse.Helpers
{
    public class ProductionTrendInquiry
    {
        #region Properties

        private System.Data.DataSet dt0;
        private Dictionary<int, string> parameter = new Dictionary<int, string>();
        private string fileName = "";
        private ExcelFileWriter excelReport;


        private string seq, seq2, area, dealer, outlet, branchHead, salesHead, salesCoordinator, salesMan, marketModel, modelCatagory, color, grade, year;
        private int rowIndex = 0, colIndex = 0, currentYear = 1900;
        private int dealerWidth = 0, outletWidth = 0, salesHeadWidth = 0, salesCoordinatorWidth = 0, salesManWidth = 0, modelWidth = 0, colorWidth = 0, totalDetailWidth = 0, gradeWidth = 0;
        private decimal janGT, febGT, marGT, aprGT, mayGT, junGT, julGT, augGT, sepGT, octGT, novGT, decGT, totalGT;
        private decimal janGTII, febGTII, marGTII, aprGTII, mayGTII, junGTII, julGTII, augGTII, sepGTII, octGTII, novGTII, decGTII, totalGTII;
        private decimal janAVG, febAVG, marAVG, aprAVG, mayAVG, junAVG, julAVG, augAVG, sepAVG, octAVG, novAVG, decAVG, totalAVG;
        private decimal currentAverage = 0, totalDealer = 0;
        private int indentTitleDesc = 0;
        private string pServer = "";

        #region Properties Background Worker

        private AutoResetEvent[] waitHandle;
        private Dictionary<int, StringBuilder> sbSave = new Dictionary<int, StringBuilder>();

        #endregion

        #endregion

        #region Constructor

        public ProductionTrendInquiry(System.Data.DataSet ds, Dictionary<int, string> param, string saveFileName)
        {
            dt0 = ds;
            parameter = param;
            fileName = saveFileName;
        }

        #endregion

        #region Private Method

        public void CreateReport(string pserver)
        {
            pServer = pserver;
            BeginBackgroundWorker();
        }

        #endregion

        #region Background Worker

        private void BeginBackgroundWorker()
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.WorkerReportsProgress = true;
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunCompleted);
            bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
            bw.RunWorkerAsync();
        }

        private void bw_DoWork(object sender, DoWorkEventArgs args)
        {
            //Application.DoEvents();
            BackgroundWorker worker = sender as BackgroundWorker;
           //DarkScreen.ShowBlackScreen("Creating Excel in progress...");

            #region Process Create Excel

            seq = dt0.Tables[0].Rows[0]["Seq"].ToString();
            seq2 = dt0.Tables[0].Rows[0]["Seq2"].ToString();
            area = dt0.Tables[0].Rows[0]["Area"].ToString();
            dealer = dt0.Tables[0].Rows[0]["CompanyName"].ToString();
            outlet = dt0.Tables[0].Rows[0]["BranchName"].ToString();
            salesHead = dt0.Tables[0].Rows[0]["SalesHeadName"].ToString();
            salesCoordinator = dt0.Tables[0].Rows[0]["SalesCoordinatorName"].ToString();
            salesMan = parameter[9] == "1" ? dt0.Tables[0].Rows[0]["SalesmanName"].ToString() : "";
            grade = parameter[9] == "1" ? dt0.Tables[0].Rows[0]["Grade"].ToString() : "";
            modelCatagory = dt0.Tables[0].Columns.Contains("ModelCatagory") == true ? dt0.Tables[0].Rows[0]["ModelCatagory"].ToString() : "";
            marketModel = parameter[10] == "1" ? dt0.Tables[0].Rows[0]["MarketModel"].ToString() : "";
            color = parameter[11] == "1" ? dt0.Tables[0].Rows[0]["ColourName"].ToString() : "";
            currentYear = Convert.ToInt32(dt0.Tables[0].Rows[0]["Year"].ToString());
            year = dt0.Tables[0].Rows[0]["Year"].ToString();

            StringBuilder sb = new StringBuilder();
            //sb.Append("(");
            sb.Append("by Outlet, Sales Head, Sales Coordinator ");
            sb.Append(parameter[9] == "1" ? ", Salesman" : "");
            sb.Append(parameter[10] == "1" ? ", Type" : "");
            sb.Append(parameter[13] != "" ? " : " + parameter[13].ToString() : "");
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
            modelWidth = (parameter[10] == "1" && parameter[13] == "") ? 1 : 0;
            colorWidth = parameter[11] == "1" ? 1 : 0;

            totalDetailWidth = dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + modelWidth + colorWidth + gradeWidth;

            indentTitleDesc = 6;

            excelReport = new ExcelFileWriter(fileName, currentYear.ToString(), "OmRpSalRgs038", pServer);
            CreateHeader(excelReport, title, rowIndex, indentTitleDesc, currentYear.ToString());

            
            DataRow[] dtCabang = dt0.Tables[0].Select();
            DataRow[] dtJabodetabek = dt0.Tables[0].Select("GroupNo = '1'");// .Select("GroupNo = '105'");
            DataRow[] dtWIB = dt0.Tables[0].Select("GroupNo = '1'");//.Select("GroupNo in ('200','300','400','500','600','700','800')");
            DataRow[] dtWIT = dt0.Tables[0].Select("GroupNo = '1'");//.Select("GroupNo not in ('100','105','200','300','400','500','600','700','800','NSDS')");

            waitHandle = new AutoResetEvent[] { new AutoResetEvent(false), new AutoResetEvent(false), new AutoResetEvent(false), new AutoResetEvent(false) };
            //OmRpSalRgs038NewXcl xclAll = new OmRpSalRgs038NewXcl(parameter);
            OmRpSalRgs038NewXcl xcl = new OmRpSalRgs038NewXcl(parameter);
            OmRpSalRgs038NewXcl xcl2 = new OmRpSalRgs038NewXcl(parameter);
            OmRpSalRgs038NewXcl xcl3 = new OmRpSalRgs038NewXcl(parameter);
            OmRpSalRgs038NewXcl xcl4 = new OmRpSalRgs038NewXcl(parameter);

            if (worker.CancellationPending == true)
            {
                args.Cancel = true;
            }
            else
            {

                //ThreadPool.QueueUserWorkItem(new WaitCallback(xclAll.CreateReportBackgroundWorker), new WaitHandleDataBackgroundClass(dt0.Tables[0].Select(), dt0.Tables[1], title, waitHandle[0], worker, 0));
                ThreadPool.QueueUserWorkItem(new WaitCallback(xcl.CreateReportBackgroundWorker), new WaitHandleDataBackgroundClass(dtCabang, dt0.Tables[1], title, waitHandle[0], worker, 0));
                ThreadPool.QueueUserWorkItem(new WaitCallback(xcl2.CreateReportBackgroundWorker), new WaitHandleDataBackgroundClass(dtJabodetabek, dt0.Tables[1], title, waitHandle[1], worker, 1));
                ThreadPool.QueueUserWorkItem(new WaitCallback(xcl3.CreateReportBackgroundWorker), new WaitHandleDataBackgroundClass(dtWIB, dt0.Tables[1], title, waitHandle[2], worker, 2));
                ThreadPool.QueueUserWorkItem(new WaitCallback(xcl4.CreateReportBackgroundWorker), new WaitHandleDataBackgroundClass(dtWIT, dt0.Tables[1], title, waitHandle[3], worker, 3));

                int workThreads, completionThreads;
                ThreadPool.GetMaxThreads(out workThreads, out completionThreads);
                ThreadPool.SetMaxThreads((int)Math.Ceiling((decimal)workThreads / 2), (int)Math.Ceiling((decimal)completionThreads / 2));
                WaitHandle.WaitAll(waitHandle);
            }

            //DarkScreen.CloseBlackScreen();
            #endregion
        }

        private void bw_RunCompleted(object e, RunWorkerCompletedEventArgs args)
        {
            if (args.Cancelled)
            {
            }
            else
            {
                if (sbSave.ContainsKey(0))
                {
                    excelReport.InsertString("</Row>");
                    excelReport.InsertString(sbSave[0]);
                }

                if (sbSave.ContainsKey(1))
                {
                    excelReport.InsertString("</Row>");
                    excelReport.InsertString(sbSave[1]);
                    if (sbSave.ContainsKey(0))
                        CreateFooterJBDTK(excelReport, 8, indentTitleDesc, true, true);
                }
                if (sbSave.ContainsKey(2))
                {
                    excelReport.InsertString("</Row>");
                    excelReport.InsertString(sbSave[2]);
                }
                if (sbSave.ContainsKey(3))
                {
                    excelReport.InsertString("</Row>");
                    excelReport.InsertString(sbSave[3]);
                }

                if (parameter[3].ToString() == "ALL")
                    CreateFooter(excelReport, 10, indentTitleDesc, true, true);
                excelReport.CloseExcelFileWriter();
            }
        }

        private void bw_ProgressChanged(object e, ProgressChangedEventArgs args)
        {
            if (sbSave.ContainsKey(args.ProgressPercentage))
                sbSave[args.ProgressPercentage] = sbSave[args.ProgressPercentage].AppendLine((args.UserState as ObjectUserState).sb.ToString());
            else
                sbSave.Add(args.ProgressPercentage, (args.UserState as ObjectUserState).sb);

            if ((args.UserState as ObjectUserState).source == "0" || (args.UserState as ObjectUserState).source == "1")
            {
                janGTII += (args.UserState as ObjectUserState).jan;
                febGTII += (args.UserState as ObjectUserState).feb;
                marGTII += (args.UserState as ObjectUserState).mar;
                aprGTII += (args.UserState as ObjectUserState).apr;
                mayGTII += (args.UserState as ObjectUserState).may;
                junGTII += (args.UserState as ObjectUserState).jun;
                julGTII += (args.UserState as ObjectUserState).jul;
                augGTII += (args.UserState as ObjectUserState).aug;
                sepGTII += (args.UserState as ObjectUserState).sep;
                octGTII += (args.UserState as ObjectUserState).oct;
                novGTII += (args.UserState as ObjectUserState).nov;
                decGTII += (args.UserState as ObjectUserState).dec;
                totalGTII += (args.UserState as ObjectUserState).total;
            }

            janGT += (args.UserState as ObjectUserState).jan;
            febGT += (args.UserState as ObjectUserState).feb;
            marGT += (args.UserState as ObjectUserState).mar;
            aprGT += (args.UserState as ObjectUserState).apr;
            mayGT += (args.UserState as ObjectUserState).may;
            junGT += (args.UserState as ObjectUserState).jun;
            julGT += (args.UserState as ObjectUserState).jul;
            augGT += (args.UserState as ObjectUserState).aug;
            sepGT += (args.UserState as ObjectUserState).sep;
            octGT += (args.UserState as ObjectUserState).oct;
            novGT += (args.UserState as ObjectUserState).nov;
            decGT += (args.UserState as ObjectUserState).dec;
            totalGT += (args.UserState as ObjectUserState).total;
            totalDealer += (args.UserState as ObjectUserState).totalDealer;

            currentAverage = (args.UserState as ObjectUserState).avgYear > currentAverage ? (args.UserState as ObjectUserState).avgYear : currentAverage;
        }

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
            if (parameter[10] == "1" && parameter[13] == "")
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
            if (parameter[10] == "1" && parameter[13] == "")
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
        }

        public void CreateFooter(ExcelFileWriter excelReport, int rowIndex, int indentTitleDesc, bool boolModel, bool boolArea)
        {
            decimal average = 0;
            #region Total
            CreateSisTotal(excelReport, dt0.Tables[0].Rows[dt0.Tables[0].Rows.Count - 1], rowIndex, totalDetailWidth);
            rowIndex++;
            if (parameter[9] == "1" && parameter[10] == "1")
            {
                average = totalGT / currentAverage;
                excelReport.SetCellValue("TOTAL NATIONAL", rowIndex, 0, 1, dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + modelWidth + colorWidth, ExcelCellStyle.GreenTotal);
                excelReport.SetCellValue(janGT.ToString("#,##0;0;'-';@"), rowIndex, 0 + totalDetailWidth, 1, 1, (janGT < Convert.ToInt32(average) && janGT != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(febGT.ToString("#,##0;0;'-';@"), rowIndex, 1 + totalDetailWidth, 1, 1, (febGT < Convert.ToInt32(average) && febGT != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(marGT.ToString("#,##0;0;'-';@"), rowIndex, 2 + totalDetailWidth, 1, 1, (marGT < Convert.ToInt32(average) && marGT != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(aprGT.ToString("#,##0;0;'-';@"), rowIndex, 3 + totalDetailWidth, 1, 1, (aprGT < Convert.ToInt32(average) && aprGT != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(mayGT.ToString("#,##0;0;'-';@"), rowIndex, 4 + totalDetailWidth, 1, 1, (mayGT < Convert.ToInt32(average) && mayGT != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(junGT.ToString("#,##0;0;'-';@"), rowIndex, 5 + totalDetailWidth, 1, 1, (junGT < Convert.ToInt32(average) && junGT != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(julGT.ToString("#,##0;0;'-';@"), rowIndex, 6 + totalDetailWidth, 1, 1, (julGT < Convert.ToInt32(average) && julGT != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(augGT.ToString("#,##0;0;'-';@"), rowIndex, 7 + totalDetailWidth, 1, 1, (augGT < Convert.ToInt32(average) && augGT != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(sepGT.ToString("#,##0;0;'-';@"), rowIndex, 8 + totalDetailWidth, 1, 1, (sepGT < Convert.ToInt32(average) && sepGT != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(octGT.ToString("#,##0;0;'-';@"), rowIndex, 9 + totalDetailWidth, 1, 1, (octGT < Convert.ToInt32(average) && octGT != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(novGT.ToString("#,##0;0;'-';@"), rowIndex, 10 + totalDetailWidth, 1, 1, (novGT < Convert.ToInt32(average) && novGT != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(decGT.ToString("#,##0;0;'-';@"), rowIndex, 11 + totalDetailWidth, 1, 1, (decGT < Convert.ToInt32(average) && decGT != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(totalGT.ToString("#,##0;0;'-';@"), rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(average.ToString("#,##0.#;0.0;'-';@"), rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.GreenTotalDecimal, true);
                rowIndex++;

                janAVG = janGT / totalDealer;
                febAVG = febGT / totalDealer;
                marAVG = marGT / totalDealer;
                aprAVG = aprGT / totalDealer;
                mayAVG = mayGT / totalDealer;
                junAVG = junGT / totalDealer;
                julAVG = julGT / totalDealer;
                augAVG = augGT / totalDealer;
                sepAVG = sepGT / totalDealer;
                octAVG = octGT / totalDealer;
                novAVG = novGT / totalDealer;
                decAVG = decGT / totalDealer;
                totalAVG = (janAVG + febAVG + marAVG + aprAVG + mayAVG + junAVG + julAVG + augAVG + sepAVG + octAVG + novAVG + decAVG);
                average = totalAVG / currentAverage;

                excelReport.SetCellValue("AVG NATIONAL", rowIndex, 0, 1, dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + modelWidth + colorWidth, ExcelCellStyle.BrownTotal);
                excelReport.SetCellValue(janAVG.ToString("#,##0;0;'-';@"), rowIndex, 0 + totalDetailWidth, 1, 1, (janAVG < Convert.ToInt32(totalAVG) && janAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(febAVG.ToString("#,##0;0;'-';@"), rowIndex, 1 + totalDetailWidth, 1, 1, (febAVG < Convert.ToInt32(totalAVG) && febAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(marAVG.ToString("#,##0;0;'-';@"), rowIndex, 2 + totalDetailWidth, 1, 1, (marAVG < Convert.ToInt32(totalAVG) && marAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(aprAVG.ToString("#,##0;0;'-';@"), rowIndex, 3 + totalDetailWidth, 1, 1, (aprAVG < Convert.ToInt32(totalAVG) && aprAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(mayAVG.ToString("#,##0;0;'-';@"), rowIndex, 4 + totalDetailWidth, 1, 1, (mayAVG < Convert.ToInt32(totalAVG) && mayAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(junAVG.ToString("#,##0;0;'-';@"), rowIndex, 5 + totalDetailWidth, 1, 1, (junAVG < Convert.ToInt32(totalAVG) && junAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(julAVG.ToString("#,##0;0;'-';@"), rowIndex, 6 + totalDetailWidth, 1, 1, (julAVG < Convert.ToInt32(totalAVG) && julAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(augAVG.ToString("#,##0;0;'-';@"), rowIndex, 7 + totalDetailWidth, 1, 1, (augAVG < Convert.ToInt32(totalAVG) && augAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(sepAVG.ToString("#,##0;0;'-';@"), rowIndex, 8 + totalDetailWidth, 1, 1, (sepAVG < Convert.ToInt32(totalAVG) && sepAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(octAVG.ToString("#,##0;0;'-';@"), rowIndex, 9 + totalDetailWidth, 1, 1, (octAVG < Convert.ToInt32(totalAVG) && octAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(novAVG.ToString("#,##0;0;'-';@"), rowIndex, 10 + totalDetailWidth, 1, 1, (novAVG < Convert.ToInt32(totalAVG) && novAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(decAVG.ToString("#,##0;0;'-';@"), rowIndex, 11 + totalDetailWidth, 1, 1, (decAVG < Convert.ToInt32(totalAVG) && decAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(totalAVG.ToString("#,##0;0;'-';@"), rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(average.ToString("#,##0.#;0.0;'-';@"), rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.BrownTotalDecimal, true);
                rowIndex++;
            }
            else if (parameter[9].ToString() == "0" && parameter[10].ToString() == "1")
            {
                average = totalGT / currentAverage;
                excelReport.SetCellValue("TOTAL NATIONAL", rowIndex, 0, 1, dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + modelWidth + colorWidth, ExcelCellStyle.GreenTotal);
                excelReport.SetCellValue(janGT.ToString("#,##0;0;'-';@"), rowIndex, 0 + totalDetailWidth, 1, 1, (janGT < Convert.ToInt32(average) && janGT != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(febGT.ToString("#,##0;0;'-';@"), rowIndex, 1 + totalDetailWidth, 1, 1, (febGT < Convert.ToInt32(average) && febGT != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(marGT.ToString("#,##0;0;'-';@"), rowIndex, 2 + totalDetailWidth, 1, 1, (marGT < Convert.ToInt32(average) && marGT != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(aprGT.ToString("#,##0;0;'-';@"), rowIndex, 3 + totalDetailWidth, 1, 1, (aprGT < Convert.ToInt32(average) && aprGT != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(mayGT.ToString("#,##0;0;'-';@"), rowIndex, 4 + totalDetailWidth, 1, 1, (mayGT < Convert.ToInt32(average) && mayGT != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(junGT.ToString("#,##0;0;'-';@"), rowIndex, 5 + totalDetailWidth, 1, 1, (junGT < Convert.ToInt32(average) && junGT != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(julGT.ToString("#,##0;0;'-';@"), rowIndex, 6 + totalDetailWidth, 1, 1, (julGT < Convert.ToInt32(average) && julGT != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(augGT.ToString("#,##0;0;'-';@"), rowIndex, 7 + totalDetailWidth, 1, 1, (augGT < Convert.ToInt32(average) && augGT != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(sepGT.ToString("#,##0;0;'-';@"), rowIndex, 8 + totalDetailWidth, 1, 1, (sepGT < Convert.ToInt32(average) && sepGT != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(octGT.ToString("#,##0;0;'-';@"), rowIndex, 9 + totalDetailWidth, 1, 1, (octGT < Convert.ToInt32(average) && octGT != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(novGT.ToString("#,##0;0;'-';@"), rowIndex, 10 + totalDetailWidth, 1, 1, (novGT < Convert.ToInt32(average) && novGT != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(decGT.ToString("#,##0;0;'-';@"), rowIndex, 11 + totalDetailWidth, 1, 1, (decGT < Convert.ToInt32(average) && decGT != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(totalGT.ToString("#,##0;0;'-';@"), rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(average.ToString("#,##0.#;0.0;'-';@"), rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.GreenTotalDecimal, true);
                rowIndex++;

                janAVG = janGT / totalDealer;
                febAVG = febGT / totalDealer;
                marAVG = marGT / totalDealer;
                aprAVG = aprGT / totalDealer;
                mayAVG = mayGT / totalDealer;
                junAVG = junGT / totalDealer;
                julAVG = julGT / totalDealer;
                augAVG = augGT / totalDealer;
                sepAVG = sepGT / totalDealer;
                octAVG = octGT / totalDealer;
                novAVG = novGT / totalDealer;
                decAVG = decGT / totalDealer;
                totalAVG = (janAVG + febAVG + marAVG + aprAVG + mayAVG + junAVG + julAVG + augAVG + sepAVG + octAVG + novAVG + decAVG);
                average = totalAVG / currentAverage;

                excelReport.SetCellValue("AVG NATIONAL", rowIndex, 0, 1, dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + modelWidth + colorWidth, ExcelCellStyle.BrownTotal);
                excelReport.SetCellValue(janAVG.ToString("#,##0;0;'-';@"), rowIndex, 0 + totalDetailWidth, 1, 1, (janAVG < Convert.ToInt32(totalAVG) && janAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(febAVG.ToString("#,##0;0;'-';@"), rowIndex, 1 + totalDetailWidth, 1, 1, (febAVG < Convert.ToInt32(totalAVG) && febAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(marAVG.ToString("#,##0;0;'-';@"), rowIndex, 2 + totalDetailWidth, 1, 1, (marAVG < Convert.ToInt32(totalAVG) && marAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(aprAVG.ToString("#,##0;0;'-';@"), rowIndex, 3 + totalDetailWidth, 1, 1, (aprAVG < Convert.ToInt32(totalAVG) && aprAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(mayAVG.ToString("#,##0;0;'-';@"), rowIndex, 4 + totalDetailWidth, 1, 1, (mayAVG < Convert.ToInt32(totalAVG) && mayAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(junAVG.ToString("#,##0;0;'-';@"), rowIndex, 5 + totalDetailWidth, 1, 1, (junAVG < Convert.ToInt32(totalAVG) && junAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(julAVG.ToString("#,##0;0;'-';@"), rowIndex, 6 + totalDetailWidth, 1, 1, (julAVG < Convert.ToInt32(totalAVG) && julAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(augAVG.ToString("#,##0;0;'-';@"), rowIndex, 7 + totalDetailWidth, 1, 1, (augAVG < Convert.ToInt32(totalAVG) && augAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(sepAVG.ToString("#,##0;0;'-';@"), rowIndex, 8 + totalDetailWidth, 1, 1, (sepAVG < Convert.ToInt32(totalAVG) && sepAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(octAVG.ToString("#,##0;0;'-';@"), rowIndex, 9 + totalDetailWidth, 1, 1, (octAVG < Convert.ToInt32(totalAVG) && octAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(novAVG.ToString("#,##0;0;'-';@"), rowIndex, 10 + totalDetailWidth, 1, 1, (novAVG < Convert.ToInt32(totalAVG) && novAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(decAVG.ToString("#,##0;0;'-';@"), rowIndex, 11 + totalDetailWidth, 1, 1, (decAVG < Convert.ToInt32(totalAVG) && decAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(totalAVG.ToString("#,##0;0;'-';@"), rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(average.ToString("#,##0.#;0.0;'-';@"), rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.BrownTotalDecimal, true);
                rowIndex++;
            }
            else
            {
                average = totalGT / currentAverage;
                excelReport.SetCellValue("TOTAL NATIONAL", rowIndex, 0, 1, dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + modelWidth + colorWidth, ExcelCellStyle.GreenTotal);
                excelReport.SetCellValue(janGT.ToString("#,##0;0;'-';@"), rowIndex, 0 + totalDetailWidth, 1, 1, (janGT < Convert.ToInt32(average) && janGT != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(febGT.ToString("#,##0;0;'-';@"), rowIndex, 1 + totalDetailWidth, 1, 1, (febGT < Convert.ToInt32(average) && febGT != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(marGT.ToString("#,##0;0;'-';@"), rowIndex, 2 + totalDetailWidth, 1, 1, (marGT < Convert.ToInt32(average) && marGT != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(aprGT.ToString("#,##0;0;'-';@"), rowIndex, 3 + totalDetailWidth, 1, 1, (aprGT < Convert.ToInt32(average) && aprGT != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(mayGT.ToString("#,##0;0;'-';@"), rowIndex, 4 + totalDetailWidth, 1, 1, (mayGT < Convert.ToInt32(average) && mayGT != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(junGT.ToString("#,##0;0;'-';@"), rowIndex, 5 + totalDetailWidth, 1, 1, (junGT < Convert.ToInt32(average) && junGT != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(julGT.ToString("#,##0;0;'-';@"), rowIndex, 6 + totalDetailWidth, 1, 1, (julGT < Convert.ToInt32(average) && julGT != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(augGT.ToString("#,##0;0;'-';@"), rowIndex, 7 + totalDetailWidth, 1, 1, (augGT < Convert.ToInt32(average) && augGT != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(sepGT.ToString("#,##0;0;'-';@"), rowIndex, 8 + totalDetailWidth, 1, 1, (sepGT < Convert.ToInt32(average) && sepGT != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(octGT.ToString("#,##0;0;'-';@"), rowIndex, 9 + totalDetailWidth, 1, 1, (octGT < Convert.ToInt32(average) && octGT != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(novGT.ToString("#,##0;0;'-';@"), rowIndex, 10 + totalDetailWidth, 1, 1, (novGT < Convert.ToInt32(average) && novGT != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(decGT.ToString("#,##0;0;'-';@"), rowIndex, 11 + totalDetailWidth, 1, 1, (decGT < Convert.ToInt32(average) && decGT != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(totalGT.ToString("#,##0;0;'-';@"), rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(average.ToString("#,##0.#;0.0;'-';@"), rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.GreenTotalDecimal, true);
                rowIndex++;

                janAVG = janGT / totalDealer;
                febAVG = febGT / totalDealer;
                marAVG = marGT / totalDealer;
                aprAVG = aprGT / totalDealer;
                mayAVG = mayGT / totalDealer;
                junAVG = junGT / totalDealer;
                julAVG = julGT / totalDealer;
                augAVG = augGT / totalDealer;
                sepAVG = sepGT / totalDealer;
                octAVG = octGT / totalDealer;
                novAVG = novGT / totalDealer;
                decAVG = decGT / totalDealer;
                totalAVG = (janAVG + febAVG + marAVG + aprAVG + mayAVG + junAVG + julAVG + augAVG + sepAVG + octAVG + novAVG + decAVG);
                average = totalAVG / currentAverage;

                excelReport.SetCellValue("AVG NATIONAL", rowIndex, 0, 1, dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + modelWidth + colorWidth, ExcelCellStyle.BrownTotal);
                excelReport.SetCellValue(janAVG.ToString("#,##0;0;'-';@"), rowIndex, 0 + totalDetailWidth, 1, 1, (janAVG < Convert.ToInt32(totalAVG) && janAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(febAVG.ToString("#,##0;0;'-';@"), rowIndex, 1 + totalDetailWidth, 1, 1, (febAVG < Convert.ToInt32(totalAVG) && febAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(marAVG.ToString("#,##0;0;'-';@"), rowIndex, 2 + totalDetailWidth, 1, 1, (marAVG < Convert.ToInt32(totalAVG) && marAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(aprAVG.ToString("#,##0;0;'-';@"), rowIndex, 3 + totalDetailWidth, 1, 1, (aprAVG < Convert.ToInt32(totalAVG) && aprAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(mayAVG.ToString("#,##0;0;'-';@"), rowIndex, 4 + totalDetailWidth, 1, 1, (mayAVG < Convert.ToInt32(totalAVG) && mayAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(junAVG.ToString("#,##0;0;'-';@"), rowIndex, 5 + totalDetailWidth, 1, 1, (junAVG < Convert.ToInt32(totalAVG) && junAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(julAVG.ToString("#,##0;0;'-';@"), rowIndex, 6 + totalDetailWidth, 1, 1, (julAVG < Convert.ToInt32(totalAVG) && julAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(augAVG.ToString("#,##0;0;'-';@"), rowIndex, 7 + totalDetailWidth, 1, 1, (augAVG < Convert.ToInt32(totalAVG) && augAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(sepAVG.ToString("#,##0;0;'-';@"), rowIndex, 8 + totalDetailWidth, 1, 1, (sepAVG < Convert.ToInt32(totalAVG) && sepAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(octAVG.ToString("#,##0;0;'-';@"), rowIndex, 9 + totalDetailWidth, 1, 1, (octAVG < Convert.ToInt32(totalAVG) && octAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(novAVG.ToString("#,##0;0;'-';@"), rowIndex, 10 + totalDetailWidth, 1, 1, (novAVG < Convert.ToInt32(totalAVG) && novAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(decAVG.ToString("#,##0;0;'-';@"), rowIndex, 11 + totalDetailWidth, 1, 1, (decAVG < Convert.ToInt32(totalAVG) && decAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(totalAVG.ToString("#,##0;0;'-';@"), rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(average.ToString("#,##0.#;0.0;'-';@"), rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.BrownTotalDecimal, true);
                rowIndex++;
            }
            #endregion
        }

        private void CreateSisTotal(ExcelFileWriter excelReport, DataRow dt0, int rowIndex, int totalDetailWidth)
        {
            #region Total SIS
            if (parameter[12].ToString() == "RETAIL" || parameter[12].ToString() == "SALES" || parameter[12].ToString() == "FPOL")
            {
                decimal janSis = Convert.ToDecimal(dt0["Jan"].ToString());
                decimal febSis = Convert.ToDecimal(dt0["Feb"].ToString());
                decimal marSis = Convert.ToDecimal(dt0["Mar"].ToString());
                decimal aprSis = Convert.ToDecimal(dt0["Apr"].ToString());
                decimal maySis = Convert.ToDecimal(dt0["May"].ToString());
                decimal junSis = Convert.ToDecimal(dt0["Jun"].ToString());
                decimal julSis = Convert.ToDecimal(dt0["Jul"].ToString());
                decimal augSis = Convert.ToDecimal(dt0["Aug"].ToString());
                decimal sepSis = Convert.ToDecimal(dt0["Sep"].ToString());
                decimal octSis = Convert.ToDecimal(dt0["Oct"].ToString());
                decimal novSis = Convert.ToDecimal(dt0["Nov"].ToString());
                decimal decSis = Convert.ToDecimal(dt0["Dec"].ToString());


                janGT += janSis;
                febGT += febSis;
                marGT += marSis;
                aprGT += aprSis;
                mayGT += maySis;
                junGT += junSis;
                julGT += julSis;
                augGT += augSis;
                sepGT += sepSis;
                octGT += octSis;
                novGT += novSis;
                decGT += decSis;
                totalGT += janSis + febSis + marSis + aprSis + maySis + junSis + julSis + augSis + sepSis + octSis + novSis + decSis;
                decimal totalSis = janSis + febSis + marSis + aprSis + maySis + junSis + julSis + augSis + sepSis + octSis + novSis + decSis;
                decimal avgSis = totalSis / currentAverage;
                totalDealer++;
                excelReport.SetCellValue("TOTAL SIS RETAIL SALES", rowIndex, 0, 1, dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + modelWidth + colorWidth, ExcelCellStyle.PurpleTotal);
                excelReport.SetCellValue(janSis.ToString("#,##0;0;'-';@"), rowIndex, 0 + totalDetailWidth, 1, 1, (janSis < Convert.ToInt32(avgSis) && janSis != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                excelReport.SetCellValue(febSis.ToString("#,##0;0;'-';@"), rowIndex, 1 + totalDetailWidth, 1, 1, (febSis < Convert.ToInt32(avgSis) && febSis != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                excelReport.SetCellValue(marSis.ToString("#,##0;0;'-';@"), rowIndex, 2 + totalDetailWidth, 1, 1, (marSis < Convert.ToInt32(avgSis) && marSis != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                excelReport.SetCellValue(aprSis.ToString("#,##0;0;'-';@"), rowIndex, 3 + totalDetailWidth, 1, 1, (aprSis < Convert.ToInt32(avgSis) && aprSis != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                excelReport.SetCellValue(maySis.ToString("#,##0;0;'-';@"), rowIndex, 4 + totalDetailWidth, 1, 1, (maySis < Convert.ToInt32(avgSis) && maySis != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                excelReport.SetCellValue(junSis.ToString("#,##0;0;'-';@"), rowIndex, 5 + totalDetailWidth, 1, 1, (junSis < Convert.ToInt32(avgSis) && junSis != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                excelReport.SetCellValue(julSis.ToString("#,##0;0;'-';@"), rowIndex, 6 + totalDetailWidth, 1, 1, (julSis < Convert.ToInt32(avgSis) && julSis != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                excelReport.SetCellValue(augSis.ToString("#,##0;0;'-';@"), rowIndex, 7 + totalDetailWidth, 1, 1, (augSis < Convert.ToInt32(avgSis) && augSis != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                excelReport.SetCellValue(sepSis.ToString("#,##0;0;'-';@"), rowIndex, 8 + totalDetailWidth, 1, 1, (sepSis < Convert.ToInt32(avgSis) && sepSis != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                excelReport.SetCellValue(octSis.ToString("#,##0;0;'-';@"), rowIndex, 9 + totalDetailWidth, 1, 1, (octSis < Convert.ToInt32(avgSis) && octSis != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                excelReport.SetCellValue(novSis.ToString("#,##0;0;'-';@"), rowIndex, 10 + totalDetailWidth, 1, 1, (novSis < Convert.ToInt32(avgSis) && novSis != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                excelReport.SetCellValue(decSis.ToString("#,##0;0;'-';@"), rowIndex, 11 + totalDetailWidth, 1, 1, (decSis < Convert.ToInt32(avgSis) && decSis != 0) ? ExcelCellStyle.PurpleTotalRedNumber : ExcelCellStyle.PurpleTotalNumber, true);
                excelReport.SetCellValue(Convert.ToDecimal(dt0["Total"].ToString()).ToString("#,##0;0;'-';@"), rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalNumber, true);
                excelReport.SetCellValue(avgSis.ToString("#,##0.#;0.0;'-';@"), rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.PurpleTotalDecimal, true);
            }
            #endregion
        }

        public void CreateFooterJBDTK(ExcelFileWriter excelReport, int rowIndex, int indentTitleDesc, bool boolModel, bool boolArea)
        {
            decimal average = 0;
            #region Total
            if (parameter[9] == "1" && parameter[10] == "1")
            {

                if (boolArea)
                {
                    average = totalGTII / currentAverage;
                    excelReport.SetCellValue("TOTAL JABODETABEK", rowIndex, 0, 1, dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + modelWidth + colorWidth, ExcelCellStyle.GreenTotal);
                    excelReport.SetCellValue(janGTII.ToString("#,##0;0;'-';@"), rowIndex, 0 + totalDetailWidth, 1, 1, (janGTII < Convert.ToInt32(average) && janGTII != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                    excelReport.SetCellValue(febGTII.ToString("#,##0;0;'-';@"), rowIndex, 1 + totalDetailWidth, 1, 1, (febGTII < Convert.ToInt32(average) && febGTII != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                    excelReport.SetCellValue(marGTII.ToString("#,##0;0;'-';@"), rowIndex, 2 + totalDetailWidth, 1, 1, (marGTII < Convert.ToInt32(average) && marGTII != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                    excelReport.SetCellValue(aprGTII.ToString("#,##0;0;'-';@"), rowIndex, 3 + totalDetailWidth, 1, 1, (aprGTII < Convert.ToInt32(average) && aprGTII != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                    excelReport.SetCellValue(mayGTII.ToString("#,##0;0;'-';@"), rowIndex, 4 + totalDetailWidth, 1, 1, (mayGTII < Convert.ToInt32(average) && mayGTII != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                    excelReport.SetCellValue(junGTII.ToString("#,##0;0;'-';@"), rowIndex, 5 + totalDetailWidth, 1, 1, (junGTII < Convert.ToInt32(average) && junGTII != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                    excelReport.SetCellValue(julGTII.ToString("#,##0;0;'-';@"), rowIndex, 6 + totalDetailWidth, 1, 1, (julGTII < Convert.ToInt32(average) && julGTII != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                    excelReport.SetCellValue(augGTII.ToString("#,##0;0;'-';@"), rowIndex, 7 + totalDetailWidth, 1, 1, (augGTII < Convert.ToInt32(average) && augGTII != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                    excelReport.SetCellValue(sepGTII.ToString("#,##0;0;'-';@"), rowIndex, 8 + totalDetailWidth, 1, 1, (sepGTII < Convert.ToInt32(average) && sepGTII != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                    excelReport.SetCellValue(octGTII.ToString("#,##0;0;'-';@"), rowIndex, 9 + totalDetailWidth, 1, 1, (octGTII < Convert.ToInt32(average) && octGTII != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                    excelReport.SetCellValue(novGTII.ToString("#,##0;0;'-';@"), rowIndex, 10 + totalDetailWidth, 1, 1, (novGTII < Convert.ToInt32(average) && novGTII != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                    excelReport.SetCellValue(decGTII.ToString("#,##0;0;'-';@"), rowIndex, 11 + totalDetailWidth, 1, 1, (decGTII < Convert.ToInt32(average) && decGTII != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                    excelReport.SetCellValue(totalGTII.ToString("#,##0;0;'-';@"), rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.GreenTotalNumber, true);
                    excelReport.SetCellValue(average.ToString("#,##0.#;0.0;'-';@"), rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.GreenTotalDecimal, true);
                    rowIndex++;

                    janAVG = janGTII / 2;
                    febAVG = febGTII / 2;
                    marAVG = marGTII / 2;
                    aprAVG = aprGTII / 2;
                    mayAVG = mayGTII / 2;
                    junAVG = junGTII / 2;
                    julAVG = julGTII / 2;
                    augAVG = augGTII / 2;
                    sepAVG = sepGTII / 2;
                    octAVG = octGTII / 2;
                    novAVG = novGTII / 2;
                    decAVG = decGTII / 2;
                    totalAVG = (janAVG + febAVG + marAVG + aprAVG + mayAVG + junAVG + julAVG + augAVG + sepAVG + octAVG + novAVG + decAVG);
                    average = totalAVG / currentAverage;

                    excelReport.SetCellValue("AVG JABODETABEK", rowIndex, 0, 1, dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + modelWidth + colorWidth, ExcelCellStyle.BrownTotal);
                    excelReport.SetCellValue(janAVG.ToString("#,##0;0;'-';@"), rowIndex, 0 + totalDetailWidth, 1, 1, (janAVG < Convert.ToInt32(totalAVG) && janAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(febAVG.ToString("#,##0;0;'-';@"), rowIndex, 1 + totalDetailWidth, 1, 1, (febAVG < Convert.ToInt32(totalAVG) && febAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(marAVG.ToString("#,##0;0;'-';@"), rowIndex, 2 + totalDetailWidth, 1, 1, (marAVG < Convert.ToInt32(totalAVG) && marAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(aprAVG.ToString("#,##0;0;'-';@"), rowIndex, 3 + totalDetailWidth, 1, 1, (aprAVG < Convert.ToInt32(totalAVG) && aprAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(mayAVG.ToString("#,##0;0;'-';@"), rowIndex, 4 + totalDetailWidth, 1, 1, (mayAVG < Convert.ToInt32(totalAVG) && mayAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(junAVG.ToString("#,##0;0;'-';@"), rowIndex, 5 + totalDetailWidth, 1, 1, (junAVG < Convert.ToInt32(totalAVG) && junAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(julAVG.ToString("#,##0;0;'-';@"), rowIndex, 6 + totalDetailWidth, 1, 1, (julAVG < Convert.ToInt32(totalAVG) && julAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(augAVG.ToString("#,##0;0;'-';@"), rowIndex, 7 + totalDetailWidth, 1, 1, (augAVG < Convert.ToInt32(totalAVG) && augAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(sepAVG.ToString("#,##0;0;'-';@"), rowIndex, 8 + totalDetailWidth, 1, 1, (sepAVG < Convert.ToInt32(totalAVG) && sepAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(octAVG.ToString("#,##0;0;'-';@"), rowIndex, 9 + totalDetailWidth, 1, 1, (octAVG < Convert.ToInt32(totalAVG) && octAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(novAVG.ToString("#,##0;0;'-';@"), rowIndex, 10 + totalDetailWidth, 1, 1, (novAVG < Convert.ToInt32(totalAVG) && novAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(decAVG.ToString("#,##0;0;'-';@"), rowIndex, 11 + totalDetailWidth, 1, 1, (decAVG < Convert.ToInt32(totalAVG) && decAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(totalAVG.ToString("#,##0;0;'-';@"), rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.BrownTotalNumber, true);
                    excelReport.SetCellValue(average.ToString("#,##0.#;0.0;'-';@"), rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.BrownTotalDecimal, true);
                    rowIndex++;
                }
            }
            else if (parameter[9].ToString() == "0" && parameter[10].ToString() == "1")
            {
                average = totalGTII / currentAverage;
                excelReport.SetCellValue("TOTAL JABODETABEK", rowIndex, 0, 1, dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + modelWidth + colorWidth, ExcelCellStyle.GreenTotal);
                excelReport.SetCellValue(janGTII.ToString("#,##0;0;'-';@"), rowIndex, 0 + totalDetailWidth, 1, 1, (janGTII < Convert.ToInt32(average) && janGTII != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(febGTII.ToString("#,##0;0;'-';@"), rowIndex, 1 + totalDetailWidth, 1, 1, (febGTII < Convert.ToInt32(average) && febGTII != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(marGTII.ToString("#,##0;0;'-';@"), rowIndex, 2 + totalDetailWidth, 1, 1, (marGTII < Convert.ToInt32(average) && marGTII != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(aprGTII.ToString("#,##0;0;'-';@"), rowIndex, 3 + totalDetailWidth, 1, 1, (aprGTII < Convert.ToInt32(average) && aprGTII != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(mayGTII.ToString("#,##0;0;'-';@"), rowIndex, 4 + totalDetailWidth, 1, 1, (mayGTII < Convert.ToInt32(average) && mayGTII != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(junGTII.ToString("#,##0;0;'-';@"), rowIndex, 5 + totalDetailWidth, 1, 1, (junGTII < Convert.ToInt32(average) && junGTII != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(julGTII.ToString("#,##0;0;'-';@"), rowIndex, 6 + totalDetailWidth, 1, 1, (julGTII < Convert.ToInt32(average) && julGTII != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(augGTII.ToString("#,##0;0;'-';@"), rowIndex, 7 + totalDetailWidth, 1, 1, (augGTII < Convert.ToInt32(average) && augGTII != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(sepGTII.ToString("#,##0;0;'-';@"), rowIndex, 8 + totalDetailWidth, 1, 1, (sepGTII < Convert.ToInt32(average) && sepGTII != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(octGTII.ToString("#,##0;0;'-';@"), rowIndex, 9 + totalDetailWidth, 1, 1, (octGTII < Convert.ToInt32(average) && octGTII != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(novGTII.ToString("#,##0;0;'-';@"), rowIndex, 10 + totalDetailWidth, 1, 1, (novGTII < Convert.ToInt32(average) && novGTII != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(decGTII.ToString("#,##0;0;'-';@"), rowIndex, 11 + totalDetailWidth, 1, 1, (decGTII < Convert.ToInt32(average) && decGTII != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(totalGTII.ToString("#,##0;0;'-';@"), rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(average.ToString("#,##0.#;0.0;'-';@"), rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.GreenTotalDecimal, true);
                rowIndex++;

                janAVG = janGTII / 2;
                febAVG = febGTII / 2;
                marAVG = marGTII / 2;
                aprAVG = aprGTII / 2;
                mayAVG = mayGTII / 2;
                junAVG = junGTII / 2;
                julAVG = julGTII / 2;
                augAVG = augGTII / 2;
                sepAVG = sepGTII / 2;
                octAVG = octGTII / 2;
                novAVG = novGTII / 2;
                decAVG = decGTII / 2;
                totalAVG = (janAVG + febAVG + marAVG + aprAVG + mayAVG + junAVG + julAVG + augAVG + sepAVG + octAVG + novAVG + decAVG);
                average = totalAVG / currentAverage;

                excelReport.SetCellValue("AVG JABODETABEK", rowIndex, 0, 1, dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + modelWidth + colorWidth, ExcelCellStyle.BrownTotal);
                excelReport.SetCellValue(janAVG.ToString("#,##0;0;'-';@"), rowIndex, 0 + totalDetailWidth, 1, 1, (janAVG < Convert.ToInt32(totalAVG) && janAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(febAVG.ToString("#,##0;0;'-';@"), rowIndex, 1 + totalDetailWidth, 1, 1, (febAVG < Convert.ToInt32(totalAVG) && febAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(marAVG.ToString("#,##0;0;'-';@"), rowIndex, 2 + totalDetailWidth, 1, 1, (marAVG < Convert.ToInt32(totalAVG) && marAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(aprAVG.ToString("#,##0;0;'-';@"), rowIndex, 3 + totalDetailWidth, 1, 1, (aprAVG < Convert.ToInt32(totalAVG) && aprAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(mayAVG.ToString("#,##0;0;'-';@"), rowIndex, 4 + totalDetailWidth, 1, 1, (mayAVG < Convert.ToInt32(totalAVG) && mayAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(junAVG.ToString("#,##0;0;'-';@"), rowIndex, 5 + totalDetailWidth, 1, 1, (junAVG < Convert.ToInt32(totalAVG) && junAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(julAVG.ToString("#,##0;0;'-';@"), rowIndex, 6 + totalDetailWidth, 1, 1, (julAVG < Convert.ToInt32(totalAVG) && julAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(augAVG.ToString("#,##0;0;'-';@"), rowIndex, 7 + totalDetailWidth, 1, 1, (augAVG < Convert.ToInt32(totalAVG) && augAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(sepAVG.ToString("#,##0;0;'-';@"), rowIndex, 8 + totalDetailWidth, 1, 1, (sepAVG < Convert.ToInt32(totalAVG) && sepAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(octAVG.ToString("#,##0;0;'-';@"), rowIndex, 9 + totalDetailWidth, 1, 1, (octAVG < Convert.ToInt32(totalAVG) && octAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(novAVG.ToString("#,##0;0;'-';@"), rowIndex, 10 + totalDetailWidth, 1, 1, (novAVG < Convert.ToInt32(totalAVG) && novAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(decAVG.ToString("#,##0;0;'-';@"), rowIndex, 11 + totalDetailWidth, 1, 1, (decAVG < Convert.ToInt32(totalAVG) && decAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(totalAVG.ToString("#,##0;0;'-';@"), rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(average.ToString("#,##0.#;0.0;'-';@"), rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.BrownTotalDecimal, true);
                rowIndex++;
            }
            else
            {
                average = totalGTII / currentAverage;
                excelReport.SetCellValue("TOTAL JABODETABEK", rowIndex, 0, 1, dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + modelWidth + colorWidth, ExcelCellStyle.GreenTotal);
                excelReport.SetCellValue(janGTII.ToString("#,##0;0;'-';@"), rowIndex, 0 + totalDetailWidth, 1, 1, (janGT < Convert.ToInt32(average) && janGT != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(febGTII.ToString("#,##0;0;'-';@"), rowIndex, 1 + totalDetailWidth, 1, 1, (febGT < Convert.ToInt32(average) && febGT != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(marGTII.ToString("#,##0;0;'-';@"), rowIndex, 2 + totalDetailWidth, 1, 1, (marGT < Convert.ToInt32(average) && marGT != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(aprGTII.ToString("#,##0;0;'-';@"), rowIndex, 3 + totalDetailWidth, 1, 1, (aprGT < Convert.ToInt32(average) && aprGT != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(mayGTII.ToString("#,##0;0;'-';@"), rowIndex, 4 + totalDetailWidth, 1, 1, (mayGT < Convert.ToInt32(average) && mayGT != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(junGTII.ToString("#,##0;0;'-';@"), rowIndex, 5 + totalDetailWidth, 1, 1, (junGT < Convert.ToInt32(average) && junGT != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(julGTII.ToString("#,##0;0;'-';@"), rowIndex, 6 + totalDetailWidth, 1, 1, (julGT < Convert.ToInt32(average) && julGT != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(augGTII.ToString("#,##0;0;'-';@"), rowIndex, 7 + totalDetailWidth, 1, 1, (augGT < Convert.ToInt32(average) && augGT != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(sepGTII.ToString("#,##0;0;'-';@"), rowIndex, 8 + totalDetailWidth, 1, 1, (sepGT < Convert.ToInt32(average) && sepGT != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(octGTII.ToString("#,##0;0;'-';@"), rowIndex, 9 + totalDetailWidth, 1, 1, (octGT < Convert.ToInt32(average) && octGT != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(novGTII.ToString("#,##0;0;'-';@"), rowIndex, 10 + totalDetailWidth, 1, 1, (novGT < Convert.ToInt32(average) && novGT != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(decGTII.ToString("#,##0;0;'-';@"), rowIndex, 11 + totalDetailWidth, 1, 1, (decGT < Convert.ToInt32(average) && decGT != 0) ? ExcelCellStyle.GreenTotalRedNumber : ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(totalGTII.ToString("#,##0;0;'-';@"), rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.GreenTotalNumber, true);
                excelReport.SetCellValue(average.ToString("#,##0.#;0.0;'-';@"), rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.GreenTotalDecimal, true);
                rowIndex++;

                janAVG = janGTII / 2;
                febAVG = febGTII / 2;
                marAVG = marGTII / 2;
                aprAVG = aprGTII / 2;
                mayAVG = mayGTII / 2;
                junAVG = junGTII / 2;
                julAVG = julGTII / 2;
                augAVG = augGTII / 2;
                sepAVG = sepGTII / 2;
                octAVG = octGTII / 2;
                novAVG = novGTII / 2;
                decAVG = decGTII / 2;
                totalAVG = (janAVG + febAVG + marAVG + aprAVG + mayAVG + junAVG + julAVG + augAVG + sepAVG + octAVG + novAVG + decAVG);
                average = totalAVG / currentAverage;

                excelReport.SetCellValue("AVG JABODETABEK", rowIndex, 0, 1, dealerWidth + outletWidth + salesHeadWidth + salesCoordinatorWidth + salesManWidth + gradeWidth + modelWidth + colorWidth, ExcelCellStyle.BrownTotal);
                excelReport.SetCellValue(janAVG.ToString("#,##0;0;'-';@"), rowIndex, 0 + totalDetailWidth, 1, 1, (janAVG < Convert.ToInt32(totalAVG) && janAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(febAVG.ToString("#,##0;0;'-';@"), rowIndex, 1 + totalDetailWidth, 1, 1, (febAVG < Convert.ToInt32(totalAVG) && febAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(marAVG.ToString("#,##0;0;'-';@"), rowIndex, 2 + totalDetailWidth, 1, 1, (marAVG < Convert.ToInt32(totalAVG) && marAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(aprAVG.ToString("#,##0;0;'-';@"), rowIndex, 3 + totalDetailWidth, 1, 1, (aprAVG < Convert.ToInt32(totalAVG) && aprAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(mayAVG.ToString("#,##0;0;'-';@"), rowIndex, 4 + totalDetailWidth, 1, 1, (mayAVG < Convert.ToInt32(totalAVG) && mayAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(junAVG.ToString("#,##0;0;'-';@"), rowIndex, 5 + totalDetailWidth, 1, 1, (junAVG < Convert.ToInt32(totalAVG) && junAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(julAVG.ToString("#,##0;0;'-';@"), rowIndex, 6 + totalDetailWidth, 1, 1, (julAVG < Convert.ToInt32(totalAVG) && julAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(augAVG.ToString("#,##0;0;'-';@"), rowIndex, 7 + totalDetailWidth, 1, 1, (augAVG < Convert.ToInt32(totalAVG) && augAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(sepAVG.ToString("#,##0;0;'-';@"), rowIndex, 8 + totalDetailWidth, 1, 1, (sepAVG < Convert.ToInt32(totalAVG) && sepAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(octAVG.ToString("#,##0;0;'-';@"), rowIndex, 9 + totalDetailWidth, 1, 1, (octAVG < Convert.ToInt32(totalAVG) && octAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(novAVG.ToString("#,##0;0;'-';@"), rowIndex, 10 + totalDetailWidth, 1, 1, (novAVG < Convert.ToInt32(totalAVG) && novAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(decAVG.ToString("#,##0;0;'-';@"), rowIndex, 11 + totalDetailWidth, 1, 1, (decAVG < Convert.ToInt32(totalAVG) && decAVG != 0) ? ExcelCellStyle.BrownTotalRedNumber : ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(totalAVG.ToString("#,##0;0;'-';@"), rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.BrownTotalNumber, true);
                excelReport.SetCellValue(average.ToString("#,##0.#;0.0;'-';@"), rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.BrownTotalDecimal, true);
                rowIndex++;
            }
            #endregion
        }

        #endregion
    }

    public class WaitHandleDataBackgroundClass
    {
        public DataRow[] dtStored;
        public DataTable dtAVG;
        public AutoResetEvent are;
        public BackgroundWorker bw;
        public string title;
        public int queueID;

        public WaitHandleDataBackgroundClass(DataRow[] dt, DataTable _dtAvg, string _title, AutoResetEvent _are, BackgroundWorker _bw, int queue)
        {
            dtStored = dt;
            are = _are;
            bw = _bw;
            title = _title;
            queueID = queue;
            dtAVG = _dtAvg;
        }
    }

    public class ObjectUserState
    {
        public StringBuilder sb = new StringBuilder();
        public string source;
        public decimal jan, feb, mar, apr, may, jun, jul, aug, sep, oct, nov, dec, total, avgYear, totalDealer;

        public ObjectUserState(StringBuilder _sb, string _source, decimal _jan, decimal _feb, decimal _mar, decimal _apr, decimal _may, decimal _jun, decimal _jul, decimal _aug, decimal _sep, decimal _oct, decimal _nov, decimal _dec, decimal _total, decimal _avgYear, decimal _totalDealer)
        {
            sb = _sb;
            source = _source;
            jan = _jan;
            feb = _feb;
            mar = _mar;
            apr = _apr;
            may = _may;
            jun = _jun;
            jul = _jul;
            aug = _aug;
            sep = _sep;
            oct = _oct;
            nov = _nov;
            dec = _dec;
            total = _total;
            avgYear = _avgYear;
            totalDealer = _totalDealer;
        }
    }
}