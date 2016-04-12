using SimDms.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace SimDms.Web.Reports.Sales
{
    public class SalesGenerateTextFileReport
    {
        #region Initialize
        public enum Format { LongFormat, ShortFormat, CustomFormat };

        public SysUser CurrentUser;

        public StringBuilder sbHeaderReport = new StringBuilder();
        public StringBuilder sbGroupHeader = new StringBuilder();
        public StringBuilder sbTotalDetail = new StringBuilder();
        public StringBuilder sbData = new StringBuilder();
        
        public StringBuilder sbDataTxt = new StringBuilder();

        //private DataSet dsHeader = null;
        //private DataTable dtDetail = null;
        private string rptId = "", st1 = "", resultString = "", fileLoc = "", footerLabel = "";

        //PrintDirect direct = new PrintDirect();
        public int line = 0, pcWritten = 0, counterData = 0, widthPaper = 0, heightPaper = 0, lineHeader = 0, lineGroupHeader = 0, ttlHal = 0, totalLine = 0;
        int totalCountLine = 99999;
        string kdLine = "W272";
        System.IntPtr lhPrinter = new System.IntPtr();
        private bool printStatus = false, pageFull = true, spaceHeader = true, newLine = false;

        #endregion

        #region Method
        public string Terbilang(long val)
        {
            string[] bilangan = new string[] { "", "satu", "dua", "tiga", "empat", "lima", "enam", "tujuh", "delapan", "sembilan", "sepuluh", "sebelas" };
            string temp = "";

            if (val < 0) val = Math.Abs(val);
            if (val < 12)
                temp = bilangan[val].ToString();
            else if (val < 20)
                temp = Terbilang(val - 10).ToString() + " belas";
            else if (val < 100)
                temp = Terbilang(val / 10) + " puluh " + Terbilang(val % 10);
            else if (val < 200)
                temp = " seratus " + Terbilang(val - 100);
            else if (val < 1000)
                temp = Terbilang(val / 100) + " ratus " + Terbilang(val % 100);
            else if (val < 2000)
                temp = " seribu " + Terbilang(val - 1000);
            else if (val < 1000000)
                temp = Terbilang(val / 1000) + " ribu " + Terbilang(val % 1000);
            else if (val < 1000000000)
                temp = Terbilang(val / 1000000) + " juta " + Terbilang(val % 1000000);
            else if (val < 1000000000000)
                temp = Terbilang(val / 1000000000) + " miliar " + Terbilang(val % 1000000000);
            else if (val < 1000000000000000)
                temp = Terbilang(val / 1000000000000) + " trilyun " + Terbilang(val % 1000000000000);
            return temp;
        }

        public string[] ConvertArrayTerbilang(string terbilang, int spaceLength)
        {
            string[] tempArray = new string[3];
            bool statusRupiah = false;

            if (terbilang.Length < spaceLength)
            {
                if (terbilang.Length + 6 < spaceLength)
                    tempArray[0] = terbilang + " rupiah";
                else
                {
                    tempArray[0] = terbilang;
                    tempArray[1] = "rupiah";
                }
            }
            else
            {
                string[] splitTemp = terbilang.Split(' ');
                string hasilSplit = "";
                int counter = 0;

                for (int i = 0; i < splitTemp.Length; i++)
                {
                    if (i != splitTemp.Length - 1)
                    {
                        if (hasilSplit.Length + splitTemp[i].Length < spaceLength)
                        {
                            hasilSplit += splitTemp[i].ToString() + " ";
                        }
                        else
                        {
                            tempArray[counter] = hasilSplit;
                            hasilSplit = splitTemp[i].ToString() + " ";
                            counter++;
                        }
                    }
                    else
                    {
                        if (hasilSplit.Length + splitTemp[i].Length < spaceLength)
                        {
                            hasilSplit += splitTemp[i].ToString() + " rupiah";
                            statusRupiah = true;
                        }
                        else
                        {
                            tempArray[counter] = hasilSplit;
                            hasilSplit = splitTemp[i].ToString() + " rupiah";
                            counter++;
                            statusRupiah = true;
                            tempArray[counter] = hasilSplit;
                        }

                    }
                }
                tempArray[counter] = hasilSplit;
            }
            return tempArray;

        }

        public string GetIndonesianDate(DateTime dt, Format df)
        {
            string[] fullMonths = new string[] {"", "JANUARI", "FEBRUARI", "MARET", "APRIL", "MEI", "JUNI", 
                "JULI", "AGUSTUS", "SEPTEMBER", "OKTOBER", "NOPEMBER", "DESEMBER"};

            string[] halfMonths = new string[] {"", "JAN", "FEB", "MAR", "APR", "MEI", "JUN", "JUL", "AGS", 
                "SEP", "OKT", "NOP", "DES"};

            string longFormat = dt.Day.ToString() + " " + fullMonths[dt.Month] + " " + dt.Year.ToString();
            string shortFormat = dt.Day.ToString() + "-" + halfMonths[dt.Month] + "-" + dt.Year.ToString();
            string customFormat = fullMonths[dt.Month] + " " + dt.Year.ToString();

            return df.Equals(Format.LongFormat) ? longFormat : df.Equals(Format.ShortFormat) ? shortFormat :
                df.Equals(Format.CustomFormat) ? customFormat : string.Empty;
        }

        public void PrintAfterBreak()
        {
            PrintAfterBreak(true, true);
        }

        public void PrintAfterBreak(bool spaceHeader, bool headerReport)
        {
            ttlHal++;

            sbHeaderReport = new StringBuilder();
            if (spaceHeader == true)
            {
                if (headerReport != false)
                    GenerateHeader(rptId, widthPaper, ttlHal);
                line = line + lineGroupHeader;
            }
            else
            {
                if (headerReport != false)
                    GenerateHeader(rptId, widthPaper, ttlHal, false, true);
                line = line + lineGroupHeader;

            }

            if (printStatus == true)
            {
                if (headerReport != false) { }
                    //PrintDirect.WritePrinter(lhPrinter, sbHeaderReport.ToString(), sbHeaderReport.Length, ref pcWritten);
                //PrintDirect.WritePrinter(lhPrinter, sbGroupHeader.ToString(), sbGroupHeader.Length, ref pcWritten);
            }
            else
            {
                if (headerReport != false)
                    sbDataTxt.Append(sbHeaderReport.ToString());
                sbDataTxt.Append(sbGroupHeader.ToString());
            }

            sbData = new StringBuilder();
            counterData = 0;
        }

        public void CheckLastLineforTotal()
        {
            StringBuilder sbLineBreak = new StringBuilder();
            StringBuilder sbLine = new StringBuilder();

            string papperFeed = "\f";
            sbLineBreak.AppendLine();

            if (kdLine == "W272")
                sbLine.AppendLine("-".PadRight(272, '-'));
            else if (kdLine == "W233")
                sbLine.AppendLine("-".PadRight(233, '-'));
            else if (kdLine == "W163")
                sbLine.AppendLine("-".PadRight(163, '-'));
            else if (kdLine == "W136")
                sbLine.AppendLine("-".PadRight(136, '-'));
            else if (kdLine == "W96")
                sbLine.AppendLine("-".PadRight(96, '-'));
            else if (kdLine == "W80")
                sbLine.AppendLine("-".PadRight(80, '-'));

            if (pageFull == true)
            {
                if (line > 63)
                {
                    line = line - totalLine;

                    for (int z = 0; z < 60; z++)
                    {
                        if (printStatus == true){}
                            //PrintDirect.WritePrinter(lhPrinter, sbLineBreak.ToString(), sbLineBreak.Length, ref pcWritten);
                        else
                            sbDataTxt.Append(sbLineBreak.ToString());

                        if (line == 60)
                        {
                            if (printStatus == true)
                            {
                                //PrintDirect.WritePrinter(lhPrinter, sbLine.ToString(), sbLine.Length, ref pcWritten);
                                //PrintDirect.WritePrinter(lhPrinter, papperFeed.ToString(), papperFeed.Length, ref pcWritten);
                            }
                            else
                            {
                                sbDataTxt.AppendLine(sbLine.ToString());
                                sbDataTxt.AppendLine(sbLine.ToString());
                                sbDataTxt.AppendLine(sbLine.ToString());
                            }
                            ttlHal++;
                            line++;
                            break;
                        }
                        line++;
                    }
                }
            }
            else
            {
                if (line > 31)
                {
                    line = line - totalLine;

                    for (int z = 0; z < 30; z++)
                    {
                        if (printStatus == true){}
                            //PrintDirect.WritePrinter(lhPrinter, sbLineBreak.ToString(), sbLineBreak.Length, ref pcWritten);
                        else
                            sbDataTxt.Append(sbLineBreak.ToString());

                        if (line == 30)
                        {
                            if (printStatus == true)
                            {
                                //PrintDirect.WritePrinter(lhPrinter, sbLine.ToString(), sbLine.Length, ref pcWritten);
                                //PrintDirect.WritePrinter(lhPrinter, sbLineBreak.ToString(), sbLineBreak.Length, ref pcWritten);
                                //PrintDirect.WritePrinter(lhPrinter, sbLineBreak.ToString(), sbLineBreak.Length, ref pcWritten);
                            }
                            else
                            {
                                sbDataTxt.AppendLine(sbLine.ToString());
                                sbDataTxt.AppendLine(" ");
                            }
                            ttlHal++;
                            line++;
                            break;
                        }
                        line++;
                    }
                }
            }
        }

        private void SaveTextFile()
        {
            if (fileLoc != "")
                File.WriteAllText(fileLoc, sbDataTxt.ToString());
            else
            {
                //SaveFileDialog sfd = new SaveFileDialog();
                //sfd.Filter = "Text File|*.txt";
                //sfd.FileName = rptId.ToString();
                //sfd.InitialDirectory = "C:/";
                //if (sfd.ShowDialog() == DialogResult.OK)
                //{
                //    File.WriteAllText(sfd.FileName.ToString(), sbDataTxt.ToString());
                //}/
            }
        }

        public int TotalCountLine
        {
            set { totalCountLine = value; }
        }

        public string ResultString
        {
            get { return resultString; }
            set { resultString = value; }
        }

        public void SetPaperSize(int width, int height)
        {
            widthPaper = width;
            heightPaper = height;
        }

        public void SetParameterPrinter(string paramValue)
        {
            st1 = paramValue;
        }

        public string FooterLabel
        {
            set { footerLabel = value; }
        }

        public void PageBreak()
        {
            SetDataReportPageBreak();
        }

        public void CloseConnectionPrinter()
        {
            //PrintDirect.EndPagePrinter(lhPrinter);
            //PrintDirect.EndDocPrinter(lhPrinter);
            //PrintDirect.ClosePrinter(lhPrinter);
        }

        public void GenerateHeader()
        {
            GenerateHeader(rptId, widthPaper, 0);
        }

        public void GenerateHeader(bool spaceHeaders)
        {
            GenerateHeader(rptId, widthPaper, 0, spaceHeaders, true);
        }

        public void GenerateHeader2(bool bGenerateHeaders)
        {
            GenerateHeader(rptId, widthPaper, 0, true, bGenerateHeaders);
        }

        public void GenerateHeader2(bool spaceHeaders, bool bGenerateHeaders)
        {
            GenerateHeader(rptId, widthPaper, 0, spaceHeaders, bGenerateHeaders);
        }

        public void GenerateHeader(string reportID, int width, int page)
        {
            GenerateHeader(reportID, width, page, true, true);
        }

        private void GenerateHeader(string reportID, int width, int page, bool spaceHeaders, bool bGenerateHeader)
        {
            spaceHeader = spaceHeaders;
            sbHeaderReport = new StringBuilder();

            if (reportID == "OmRpSalesTrn003A")
            {
                sbHeaderReport.AppendLine("");
            }

            if (page == 0)
                page = 1;

            string coname = string.Empty;
            string address1 = string.Empty;
            string address2 = string.Empty;
            string address3 = string.Empty;
            string title = string.Empty;

            //SysUserActivity user = SysUserActivity.Current;

            if (reportID != string.Empty)
            {
                SimDms.Common.Models.CoProfile cp = new SimDms.Common.Models.CoProfile();

                using (var gnCtx = new SimDms.General.Models.DataContext())
                {
                    cp = gnCtx.CoProfiles.Find(CurrentUser.CompanyCode, CurrentUser.BranchCode);
                    title = gnCtx.Database.SqlQuery<string>("select reportinfo from sysreport where reportid='" + reportID + "'").FirstOrDefault();
                }

                if (cp != null)
                {

                    coname = cp.CompanyName;// row["CompanyName"].ToString();
                    address1 = cp.Address1;// row["Address1"].ToString();
                    address2 = cp.Address2; //row["Address2"].ToString();
                    address3 = cp.Address3;// row["PhoneNo"].ToString();
                }
            }

            //sbHeaderReport.Append(st1);
            //DataRow rowInfo = SysReportBLL.GetReportInfo(reportID);
            string reportInfo = title;// rowInfo["ReportInfo"].ToString();
            int lengthInfo = reportInfo.Length;
            decimal temp1 = 0;

            int headerInfo = 0, headerMiddle = 0;
            if (kdLine == "W272")
            {
                headerInfo = 240;
                headerMiddle = 270;
            }
            else if (kdLine == "W233")
            {
                headerInfo = 200;
                headerMiddle = 230;
            }
            else if (kdLine == "W163")
            {
                headerInfo = 140;
                headerMiddle = 160;
            }
            else if (kdLine == "W136")
            {
                headerInfo = 110;
                headerMiddle = 134;
            }
            else if (kdLine == "W96")
            {
                headerInfo = 70;
                headerMiddle = 94;
            }
            else if (kdLine == "W80")
            {
                headerInfo = 55;
                headerMiddle = 78;
            }

            if (bGenerateHeader)
            {
                sbHeaderReport.AppendLine(coname.PadRight(headerInfo, ' ') + "Hal : " + page);
                sbHeaderReport.AppendLine(address1.PadRight(headerInfo, ' ') + "Tgl : " + DateTime.Now.ToString("dd-MMM-yyyy"));
                sbHeaderReport.AppendLine(address2.PadRight(headerInfo, ' ') + "Jam : " + DateTime.Now.ToString("HH:MM:ss"));
                sbHeaderReport.AppendLine(address3.PadRight(headerInfo, ' ') + "PID : " + reportID);
                if (spaceHeader == true)
                {
                    sbHeaderReport.AppendLine();
                    line = 6;
                }
                else
                {
                    line = 5;
                }

                temp1 = (headerMiddle - lengthInfo) / 2;

                decimal leftTab = Math.Floor(temp1);

                sbHeaderReport.AppendLine(" ".PadRight((int)leftTab, ' ') + reportInfo);
            }
        }

        public void GenerateTextFileReports(string reportID, string location)
        {
            GenerateTextFileReports(reportID, location, true);
        }

        public void GenerateTextFileReports(string reportID, string location, bool print)
        {
            GenerateTextFileReports(reportID, location, "W272", print);
        }

        public void GenerateTextFileReports(string reportID, string location, bool print, string fileLocation)
        {
            GenerateTextFileReports(reportID, location, "W272", print, fileLocation);
        }

        public void GenerateTextFileReports(string reportID, string location, bool print, string fileLocation, bool fullPage)
        {
            GenerateTextFileReports(reportID, location, "W272", print, fileLocation, fullPage);
        }

        public void GenerateTextFileReports(string reportID, string location, string codeLineWidth)
        {
            GenerateTextFileReports(reportID, location, codeLineWidth, true);
        }

        public void GenerateTextFileReports(string reportID, string location, string codeLineWidth, bool print)
        {
            GenerateTextFileReports(reportID, location, codeLineWidth, print, "");
        }

        public void GenerateTextFileReports(string reportID, string location, string codeLineWidth, bool print, bool fullPage)
        {
            GenerateTextFileReports(reportID, location, codeLineWidth, print, "", fullPage);
        }

        public void GenerateTextFileReports(string reportID, string location, string codeLineWidth, bool print, string fileLocation)
        {
            GenerateTextFileReports(reportID, location, codeLineWidth, print, fileLocation, true);
        }

        public void GenerateTextFileReports(string reportID, string location, string codeLineWidth, bool print, string fileLocation, bool fullPage)
        {
            line = 0;

            if (widthPaper == 0)
            {
                widthPaper = 1400;
                heightPaper = 1100;
            }

            fileLoc = fileLocation;
            printStatus = print;
            kdLine = codeLineWidth;
            rptId = reportID;
            pageFull = fullPage;

            //DOCINFO di = new DOCINFO();

            // text to print with a form feed character
            //di.pDocName = reportID;
            //di.pDataType = "RAW";

            //SysParameter oSysParameter = SysParameterBLL.GetRecord("DEFAULT_PRINTER_NAME");

            //Setting Printer
            if (print == true)
            {
              //  PrintDirect.OpenPrinter(location, ref lhPrinter, 0);

                //PrintDirect.StartDocPrinter(lhPrinter, 1, ref di);
                //PrintDirect.StartPagePrinter(lhPrinter);
            }
        }

        #endregion

        #region Create Report
        #region Data Detail
        public void SetDataReportLine()
        {
            if (kdLine == "W272")
                sbData.AppendLine("-".PadRight(272, '-'));
            else if (kdLine == "W233")
                sbData.AppendLine("-".PadRight(233, '-'));
            else if (kdLine == "W163")
                sbData.AppendLine("-".PadRight(163, '-'));
            else if (kdLine == "W136")
                sbData.AppendLine("-".PadRight(136, '-'));
            else if (kdLine == "W96")
                sbData.AppendLine("-".PadRight(96, '-'));
            else if (kdLine == "W80")
                sbData.AppendLine("-".PadRight(80, '-'));

            line++;
            PrintData(false);
        }

        public void SetDataReportPageBreak()
        {
            //PrintDirect.WritePrinter(lhPrinter, "\f", 2, ref pcWritten);
            sbDataTxt.AppendLine("--pg--");
        }

        public void SetDataDetailSpace(int widthChar)
        {
            sbData.Append(" ".PadRight(widthChar, ' '));
        }

        public void SetDataDetailLineBreak()
        {
            sbData.AppendLine();
            line++;
            PrintData(false);
        }

        public void SetDataDetail(string value, int widthChar, char charFill)
        {
            SetDataDetail(value, widthChar, charFill, false);
        }

        public void SetDataDetail(string value, int widthChar, char charFill, bool spaceAfter)
        {
            SetDataDetail(value, widthChar, charFill, spaceAfter, false);
        }

        public void SetDataDetail(string value, int widthChar, char charFill, bool spaceAfter, bool lineBreak)
        {
            SetDataDetail(value, widthChar, charFill, spaceAfter, lineBreak, false);
        }

        public void SetDataDetail(string value, int widthChar, char charFill, bool spaceAfter, bool lineBreak, bool padLeft)
        {
            SetDataDetail(value, widthChar, charFill, spaceAfter, lineBreak, padLeft, false, "");
        }

        public void SetDataDetail(string value, int widthChar, char charFill, bool spaceAfter, bool lineBreak, bool padLeft, bool isNumber, string formatNumber)
        {
            if (isNumber == false)
            {
                if (lineBreak == true)
                {
                    if (padLeft == false)
                    {
                        sbData.AppendLine(value.PadRight(widthChar, charFill).Substring(0, widthChar));
                        line++;
                    }
                    else
                    {
                        sbData.AppendLine(value.PadLeft(widthChar, charFill).Substring(0, widthChar));
                        line++;
                    }
                }
                else
                {
                    if (padLeft == false)
                        sbData.Append(value.PadRight(widthChar, charFill).Substring(0, widthChar));
                    else
                        sbData.Append(value.PadLeft(widthChar, charFill).Substring(0, widthChar));
                }
            }
            else
            {
                decimal valueMoney = 0;
                decimal.TryParse(value, out valueMoney);

                if (lineBreak == true)
                {
                    if (padLeft == false)
                    {
                        sbData.AppendLine(valueMoney.ToString(formatNumber).PadRight(widthChar, ' ').Substring(0, widthChar));
                        line++;
                    }
                    else
                    {
                        sbData.AppendLine(valueMoney.ToString(formatNumber).PadLeft(widthChar, ' ').Substring(0, widthChar));
                        line++;
                    }
                }
                else
                {
                    if (padLeft == false)
                        sbData.Append(valueMoney.ToString(formatNumber).PadRight(widthChar, ' ').Substring(0, widthChar));
                    else
                        sbData.Append(valueMoney.ToString(formatNumber).PadLeft(widthChar, ' ').Substring(0, widthChar));
                }
            }

            if (spaceAfter == true)
                sbData.Append(" ");
        }
        #endregion

        #region Total Detail
        public void SetTotalDetailLine()
        {
            if (kdLine == "W272")
                sbTotalDetail.AppendLine("-".PadRight(272, '-'));
            else if (kdLine == "W233")
                sbTotalDetail.AppendLine("-".PadRight(233, '-'));
            else if (kdLine == "W163")
                sbTotalDetail.AppendLine("-".PadRight(163, '-'));
            else if (kdLine == "W136")
                sbTotalDetail.AppendLine("-".PadRight(136, '-'));
            else if (kdLine == "W96")
                sbTotalDetail.AppendLine("-".PadRight(96, '-'));
            else if (kdLine == "W80")
                sbTotalDetail.AppendLine("-".PadRight(80, '-'));

            totalLine++;
            line++;
        }

        public void SetTotalDetailSpace(int widthChar)
        {
            sbTotalDetail.Append(" ".PadRight(widthChar, ' '));
        }

        public void SetTotalDetail(string value, int widthChar, char charFill)
        {
            SetTotalDetail(value, widthChar, charFill, false);
        }

        public void SetTotalDetail(string value, int widthChar, char charFill, bool spaceAfter)
        {
            SetTotalDetail(value, widthChar, charFill, spaceAfter, false);
        }

        public void SetTotalDetail(string value, int widthChar, char charFill, bool spaceAfter, bool lineBreak)
        {
            SetTotalDetail(value, widthChar, charFill, spaceAfter, lineBreak, false);
        }

        public void SetTotalDetail(string value, int widthChar, char charFill, bool spaceAfter, bool lineBreak, bool padLeft)
        {
            SetTotalDetail(value, widthChar, charFill, spaceAfter, lineBreak, padLeft, false, "");
        }

        public void SetTotalDetail(string value, int widthChar, char charFill, bool spaceAfter, bool lineBreak, bool padLeft, bool isNumber, string formatNumber)
        {
            if (isNumber == false)
            {
                if (lineBreak == true)
                {
                    if (padLeft == false)
                    {
                        sbTotalDetail.AppendLine(value.PadRight(widthChar, charFill).Substring(0, widthChar));
                        totalLine++;
                        line++;
                    }
                    else
                    {
                        sbTotalDetail.AppendLine(value.PadLeft(widthChar, charFill).Substring(0, widthChar));
                        line++;
                        totalLine++;
                    }
                }
                else
                {
                    if (padLeft == false)
                        sbTotalDetail.Append(value.PadRight(widthChar, charFill).Substring(0, widthChar));
                    else
                        sbTotalDetail.Append(value.PadLeft(widthChar, charFill).Substring(0, widthChar));
                }
            }
            else
            {
                decimal valueMoney = 0;
                decimal.TryParse(value, out valueMoney);

                if (lineBreak == true)
                {
                    if (padLeft == true)
                    {
                        sbTotalDetail.AppendLine(valueMoney.ToString(formatNumber).PadLeft(widthChar, ' ').Substring(0, widthChar));
                        line++;
                        totalLine++;
                    }
                    else
                    {
                        sbTotalDetail.AppendLine(valueMoney.ToString(formatNumber).PadRight(widthChar, ' ').Substring(0, widthChar));
                        line++;
                        totalLine++;
                    }
                }
                else
                {
                    if (padLeft == true)
                        sbTotalDetail.Append(valueMoney.ToString(formatNumber).PadLeft(widthChar, ' ').Substring(0, widthChar));
                    else
                        sbTotalDetail.Append(valueMoney.ToString(formatNumber).PadRight(widthChar, ' ').Substring(0, widthChar));
                }
            }

            if (spaceAfter == true)
                sbTotalDetail.Append(" ");
        }
        #endregion

        #region Group Header

        public void CleanHeader()
        {
            sbGroupHeader = new StringBuilder();
            lineGroupHeader = 0;
        }

        public void ReplaceGroupHdr(string oldValue, string newValue)
        {
            ReplaceGroupHdr(oldValue, newValue, 0);
        }

        public void ReplaceGroupHdr(string oldValue, string newValue, int maxChar, int startIndex, int count)
        {
            string space = "";
            int oldLength = oldValue.Length;
            int newLength = newValue.Length;

            if (maxChar != 0)
            {
                if (oldLength < newLength)
                {
                    if (newLength < maxChar)
                    {
                        for (int i = 0; i < (newLength - oldLength); i++)
                        {
                            space += " ";
                        }

                        sbGroupHeader.Replace(oldValue + space, newValue, startIndex, count);
                    }
                    else
                    {
                        for (int i = 0; i < (newLength - oldLength); i++)
                        {
                            space += " ";
                        }

                        sbGroupHeader.Replace(oldValue + space, newValue.Substring(maxChar), startIndex, count);
                    }
                }
                else if (oldLength > newLength)
                {

                    for (int i = 0; i < (oldLength - newLength); i++)
                    {
                        space += " ";
                    }

                    sbGroupHeader.Replace(oldValue, newValue + space, startIndex, count);
                }
                else
                    sbGroupHeader.Replace(oldValue, newValue, startIndex, count);
            }
            else
            {
                sbGroupHeader.Replace(oldValue, newValue, startIndex, count);
            }
        }

        public void ReplaceGroupHdr(string oldValue, string newValue, int maxChar)
        {
            string space = "";
            int oldLength = oldValue.Length;
            int newLength = newValue.Length;

            if (oldValue != newValue)
            {
                if (maxChar != 0)
                {
                    if (oldLength < newLength)
                    {
                        if (newLength <= maxChar)
                        {
                            for (int i = 0; i < (newLength - oldLength); i++)
                            {
                                space += " ";
                            }

                            sbGroupHeader.Replace(oldValue + space, newValue);
                        }
                        else
                        {
                            for (int i = 0; i < (newLength - oldLength); i++)
                            {
                                space += " ";
                            }

                            sbGroupHeader.Replace(oldValue + space, newValue.Substring(maxChar));
                        }
                    }
                    else if (oldLength > newLength)
                    {

                        for (int i = 0; i < (oldLength - newLength); i++)
                        {
                            space += " ";
                        }

                        sbGroupHeader.Replace(oldValue, newValue + space);
                    }
                    else
                        sbGroupHeader.Replace(oldValue, newValue);
                }
                else
                {
                    sbGroupHeader.Replace(oldValue, newValue);
                }
            }
        }

        public void ReplaceGroupHdr(string oldValue, string newValue, int maxChar, int startIndex)
        {
            string space = "";
            int oldLength = oldValue.Length;
            int newLength = newValue.Length;

            if (oldValue != newValue)
            {
                if (maxChar != 0)
                {
                    if (oldLength < newLength)
                    {
                        if (newLength <= maxChar)
                        {
                            for (int i = 0; i < (newLength - oldLength); i++)
                            {
                                space += " ";
                            }
                            if (oldValue == string.Empty || oldValue == "")
                                sbGroupHeader.Replace(oldValue + space, newValue, startIndex, maxChar);
                            else
                                sbGroupHeader.Replace(oldValue + space, newValue);
                        }
                        else
                        {
                            for (int i = 0; i < (newLength - oldLength); i++)
                            {
                                space += " ";
                            }

                            if (oldValue == string.Empty || oldValue == "")
                                sbGroupHeader.Replace(oldValue + space, newValue.Substring(maxChar), startIndex, maxChar);
                            else
                                sbGroupHeader.Replace(oldValue + space, newValue.Substring(maxChar));
                        }
                    }
                    else if (oldLength > newLength)
                    {

                        for (int i = 0; i < (oldLength - newLength); i++)
                        {
                            space += " ";
                        }
                        sbGroupHeader.Replace(oldValue, newValue + space);
                    }
                    else
                        sbGroupHeader.Replace(oldValue, newValue);
                }
                else
                {
                    sbGroupHeader.Replace(oldValue, newValue);
                }
            }
        }

        public void SetGroupHeaderLine()
        {
            if (kdLine == "W272")
                sbGroupHeader.AppendLine("-".PadRight(272, '-'));
            else if (kdLine == "W233")
                sbGroupHeader.AppendLine("-".PadRight(233, '-'));
            else if (kdLine == "W163")
                sbGroupHeader.AppendLine("-".PadRight(163, '-'));
            else if (kdLine == "W136")
                sbGroupHeader.AppendLine("-".PadRight(136, '-'));
            else if (kdLine == "W96")
                sbGroupHeader.AppendLine("-".PadRight(96, '-'));
            else if (kdLine == "W80")
                sbGroupHeader.AppendLine("-".PadRight(80, '-'));

            line++;
            lineGroupHeader++;
        }

        public void SetGroupHeaderSpace(int width)
        {
            sbGroupHeader.Append(" ".PadRight(width, ' '));
        }

        public void SetGroupHeader(string value, int widthChar)
        {
            SetGroupHeader(value, widthChar, ' ', false, false, false);
        }

        public void SetGroupHeader(string value, int widthChar, char charFill)
        {
            SetGroupHeader(value, widthChar, charFill, false);
        }

        public void SetGroupHeader(string value, int widthChar, char charFill, bool spaceAfter)
        {
            SetGroupHeader(value, widthChar, charFill, spaceAfter, false);
        }

        public void SetGroupHeader(string value, int widthChar, char charFill, bool spaceAfter, bool lineBreak)
        {
            SetGroupHeader(value, widthChar, charFill, spaceAfter, lineBreak, false);
        }

        public void SetGroupHeader(string value, int widthChar, char charFill, bool spaceAfter, bool lineBreak, bool padLeft)
        {
            SetGroupHeader(value, widthChar, charFill, spaceAfter, lineBreak, padLeft, false);
        }

        public void SetGroupHeader(string value, int widthChar, char charFill, bool spaceAfter, bool lineBreak, bool padLeft, bool middleAlligment)
        {
            if (middleAlligment == false)
            {
                if (lineBreak == true)
                {
                    if (padLeft == false)
                    {
                        sbGroupHeader.AppendLine(value.PadRight(widthChar, charFill).Substring(0, widthChar));
                        line++; lineGroupHeader++;
                    }
                    else
                    {
                        sbGroupHeader.AppendLine(value.PadLeft(widthChar, charFill).Substring(0, widthChar));
                        line++; lineGroupHeader++;
                    }
                }
                else
                {
                    if (padLeft == false)
                        sbGroupHeader.Append(value.PadRight(widthChar, charFill).Substring(0, widthChar));
                    else
                        sbGroupHeader.Append(value.PadLeft(widthChar, charFill).Substring(0, widthChar));
                }
            }
            else
            {
                decimal valueInt = (decimal)value.Length;
                decimal substractValue = (widthChar - valueInt) / 2;
                decimal left = Math.Floor(substractValue);
                decimal right = widthChar - left - valueInt;

                if (lineBreak == true)
                {
                    sbGroupHeader.AppendLine(charFill.ToString().PadRight((int)left, charFill) + value + charFill.ToString().PadRight((int)right, charFill));
                    line++; lineGroupHeader++;
                }
                else
                {
                    sbGroupHeader.Append(charFill.ToString().PadRight((int)left, charFill) + value + charFill.ToString().PadRight((int)right, charFill));
                }
            }
            if (spaceAfter == true)
                sbGroupHeader.Append(" ");
        }
        #endregion
        #endregion

        #region Print
        public void PrintData()
        {
            PrintData(false);
        }

        public void PrintData(bool empty5Space)
        {
            StringBuilder sbLineBreak = new StringBuilder();
            StringBuilder sbBreakLine = new StringBuilder();

            sbBreakLine.AppendLine();

            if (kdLine == "W272")
                sbLineBreak.AppendLine("-".PadRight(272, '-'));
            else if (kdLine == "W233")
                sbLineBreak.AppendLine("-".PadRight(233, '-'));
            else if (kdLine == "W163")
                sbLineBreak.AppendLine("-".PadRight(163, '-'));
            else if (kdLine == "W136")
                sbLineBreak.AppendLine("-".PadRight(136, '-'));
            else if (kdLine == "W96")
                sbLineBreak.AppendLine("-".PadRight(96, '-'));
            else if (kdLine == "W80")
                sbLineBreak.AppendLine("-".PadRight(80, '-'));

            if (pageFull == true)
            {
                #region Full Page
                if (line < 60)
                {
                    if (empty5Space == true)
                    {
                        if (counterData < 5)
                        {
                            if (printStatus == true){}
                               ////PrintDirect.WritePrinter(lhPrinter, sbData.ToString(), sbData.Length, ref pcWritten);
                            else
                                sbDataTxt.Append(sbData.ToString());
                            sbData = new StringBuilder();
                            counterData++;
                        }
                        else
                        {
                            if (printStatus == true)
                            {
                                //PrintDirect.WritePrinter(lhPrinter, sbBreakLine.ToString(), sbBreakLine.Length, ref pcWritten);
                                //PrintDirect.WritePrinter(lhPrinter, sbData.ToString(), sbData.Length, ref pcWritten);
                            }
                            else
                            {
                                sbDataTxt.Append(sbBreakLine.ToString());
                                sbDataTxt.Append(sbData.ToString());
                            }

                            sbData = new StringBuilder();
                            line++;
                            counterData = 1;
                        }
                    }
                    else
                    {
                        if (printStatus == true){}
                            //PrintDirect.WritePrinter(lhPrinter, sbData.ToString(), sbData.Length, ref pcWritten);
                        else
                            sbDataTxt.Append(sbData.ToString());
                        sbData = new StringBuilder();
                        counterData++;
                    }
                }
                else
                {
                    if (printStatus == true){}
                        //PrintDirect.WritePrinter(lhPrinter, sbLineBreak.ToString(), sbLineBreak.Length, ref pcWritten);
                    else
                        sbDataTxt.Append(sbLineBreak.ToString());

                    SetDataReportPageBreak();
                    ttlHal++;

                    sbHeaderReport = new StringBuilder();

                    GenerateHeader(rptId, widthPaper, ttlHal);

                    line = 6 + lineGroupHeader;
                    if (printStatus == true)
                    {
                        //PrintDirect.WritePrinter(lhPrinter, sbHeaderReport.ToString(), sbHeaderReport.Length, ref pcWritten);
                        //PrintDirect.WritePrinter(lhPrinter, sbGroupHeader.ToString(), sbGroupHeader.Length, ref pcWritten);
                        //PrintDirect.WritePrinter(lhPrinter, sbData.ToString(), sbData.Length, ref pcWritten);
                    }
                    else
                    {
                        sbDataTxt.Append(sbHeaderReport.ToString());
                        sbDataTxt.Append(sbGroupHeader.ToString());
                        sbDataTxt.Append(sbData.ToString());
                    }

                    sbData = new StringBuilder();
                    line++;
                    counterData = 1;
                }
                #endregion
            }
            else
            {
                #region Half Page
                if (line < 31)
                {
                    if (empty5Space == true)
                    {
                        if (counterData < 5)
                        {
                            if (printStatus == true){}
                                //PrintDirect.WritePrinter(lhPrinter, sbData.ToString(), sbData.Length, ref pcWritten);
                            else
                                sbDataTxt.Append(sbData.ToString());

                            sbData = new StringBuilder();
                            counterData++;
                        }
                        else
                        {
                            if (printStatus == true)
                            {
                                //PrintDirect.WritePrinter(lhPrinter, sbBreakLine.ToString(), sbBreakLine.Length, ref pcWritten);
                                //PrintDirect.WritePrinter(lhPrinter, sbData.ToString(), sbData.Length, ref pcWritten);
                            }
                            else
                            {
                                sbDataTxt.Append(sbBreakLine.ToString());
                                sbDataTxt.Append(sbData.ToString());
                            }

                            sbData = new StringBuilder();
                            line++;
                            counterData = 1;
                        }
                    }
                    else
                    {
                        if (printStatus == true) { }
                           ////PrintDirect.WritePrinter(lhPrinter, sbData.ToString(), sbData.Length, ref pcWritten);
                        else
                            sbDataTxt.Append(sbData.ToString());
                        sbData = new StringBuilder();
                        counterData++;
                    }
                }
                else
                {
                    StringBuilder sbBreak = new StringBuilder();

                    if (printStatus == true){}
                       ////PrintDirect.WritePrinter(lhPrinter, sbLineBreak.ToString(), sbLineBreak.Length, ref pcWritten);
                    else
                        sbDataTxt.Append(sbLineBreak.ToString());

                    for (int z = 0; z < 31; z++)
                    {
                        if (line == 32)
                            break;
                        sbBreak.AppendLine();
                        line++;
                    }

                    if (sbBreak.Length != 0)
                        if (printStatus == true) { }
                        ////PrintDirect.WritePrinter(lhPrinter, sbBreak.ToString(), sbBreak.Length, ref pcWritten);
                        else
                            sbDataTxt.Append(sbBreak.ToString());


                    SetDataReportPageBreak();
                    ttlHal++;

                    sbHeaderReport = new StringBuilder();
                    GenerateHeader(rptId, widthPaper, ttlHal, spaceHeader, true);

                    if (spaceHeader == true)
                        line = 6 + lineGroupHeader;
                    else
                        line = 5 + lineGroupHeader;

                    if (printStatus == true)
                    {
                        //PrintDirect.WritePrinter(lhPrinter, sbHeaderReport.ToString(), sbHeaderReport.Length, ref pcWritten);
                        //PrintDirect.WritePrinter(lhPrinter, sbGroupHeader.ToString(), sbGroupHeader.Length, ref pcWritten);
                        //PrintDirect.WritePrinter(lhPrinter, sbData.ToString(), sbData.Length, ref pcWritten);
                    }
                    else
                    {
                        sbDataTxt.Append(sbHeaderReport.ToString());
                        sbDataTxt.Append(sbGroupHeader.ToString());
                        sbDataTxt.Append(sbData.ToString());
                    }

                    sbData = new StringBuilder();
                    line++;
                    counterData = 1;
                }
                #endregion
            }
        }

        public void PrintData(bool empty5Space, bool bGenerateHeader)
        {
            StringBuilder sbLineBreak = new StringBuilder();
            StringBuilder sbBreakLine = new StringBuilder();

            sbBreakLine.AppendLine();

            if (kdLine == "W272")
                sbLineBreak.AppendLine("-".PadRight(272, '-'));
            else if (kdLine == "W233")
                sbLineBreak.AppendLine("-".PadRight(233, '-'));
            else if (kdLine == "W163")
                sbLineBreak.AppendLine("-".PadRight(163, '-'));
            else if (kdLine == "W136")
                sbLineBreak.AppendLine("-".PadRight(136, '-'));
            else if (kdLine == "W96")
                sbLineBreak.AppendLine("-".PadRight(96, '-'));
            else if (kdLine == "W80")
                sbLineBreak.AppendLine("-".PadRight(80, '-'));

            if (pageFull == true)
            {
                #region Full Page
                if (line < 60)
                {
                    if (empty5Space == true)
                    {
                        if (counterData < 5)
                        {
                            if (printStatus == true){}
                               ////PrintDirect.WritePrinter(lhPrinter, sbData.ToString(), sbData.Length, ref pcWritten);
                            else
                                sbDataTxt.Append(sbData.ToString());
                            sbData = new StringBuilder();
                            counterData++;
                        }
                        else
                        {
                            if (printStatus == true)
                            {
                                //PrintDirect.WritePrinter(lhPrinter, sbBreakLine.ToString(), sbBreakLine.Length, ref pcWritten);
                                //PrintDirect.WritePrinter(lhPrinter, sbData.ToString(), sbData.Length, ref pcWritten);
                            }
                            else
                            {
                                sbDataTxt.Append(sbBreakLine.ToString());
                                sbDataTxt.Append(sbData.ToString());
                            }

                            sbData = new StringBuilder();
                            line++;
                            counterData = 1;
                        }
                    }
                    else
                    {
                        if (printStatus == true){}
                            //PrintDirect.WritePrinter(lhPrinter, sbData.ToString(), sbData.Length, ref pcWritten);
                        else
                            sbDataTxt.Append(sbData.ToString());
                        sbData = new StringBuilder();
                        counterData++;
                    }
                }
                else
                {
                    if (printStatus == true){}
                       ////PrintDirect.WritePrinter(lhPrinter, sbLineBreak.ToString(), sbLineBreak.Length, ref pcWritten);
                    else
                        sbDataTxt.Append(sbLineBreak.ToString());

                    SetDataReportPageBreak();
                    ttlHal++;

                    if (bGenerateHeader)
                    {
                        sbHeaderReport = new StringBuilder();
                        GenerateHeader(rptId, widthPaper, ttlHal);
                        line = 6 + lineGroupHeader;
                    }
                    else line = lineGroupHeader;

                    if (printStatus == true)
                    {
                        //PrintDirect.WritePrinter(lhPrinter, sbHeaderReport.ToString(), sbHeaderReport.Length, ref pcWritten);
                        //PrintDirect.WritePrinter(lhPrinter, sbGroupHeader.ToString(), sbGroupHeader.Length, ref pcWritten);
                        //PrintDirect.WritePrinter(lhPrinter, sbData.ToString(), sbData.Length, ref pcWritten);
                    }
                    else
                    {
                        sbDataTxt.Append(sbHeaderReport.ToString());
                        sbDataTxt.Append(sbGroupHeader.ToString());
                        sbDataTxt.Append(sbData.ToString());
                    }

                    sbData = new StringBuilder();
                    line++;
                    counterData = 1;
                }
                #endregion
            }
            else
            {
                #region Half Page
                if (line < 31)
                {
                    if (empty5Space == true)
                    {
                        if (counterData < 5)
                        {
                            if (printStatus == true){}
                               //PrintDirect.WritePrinter(lhPrinter, sbData.ToString(), sbData.Length, ref pcWritten);
                            else
                                sbDataTxt.Append(sbData.ToString());

                            sbData = new StringBuilder();
                            counterData++;
                        }
                        else
                        {
                            if (printStatus == true)
                            {
                               //PrintDirect.WritePrinter(lhPrinter, sbBreakLine.ToString(), sbBreakLine.Length, ref pcWritten);
                               //PrintDirect.WritePrinter(lhPrinter, sbData.ToString(), sbData.Length, ref pcWritten);
                            }
                            else
                            {
                                sbDataTxt.Append(sbBreakLine.ToString());
                                sbDataTxt.Append(sbData.ToString());
                            }

                            sbData = new StringBuilder();
                            line++;
                            counterData = 1;
                        }
                    }
                    else
                    {
                        if (printStatus == true){}
                           //PrintDirect.WritePrinter(lhPrinter, sbData.ToString(), sbData.Length, ref pcWritten);
                        else
                            sbDataTxt.Append(sbData.ToString());
                        sbData = new StringBuilder();
                        counterData++;
                    }
                }
                else
                {
                    StringBuilder sbBreak = new StringBuilder();

                    if (printStatus == true) { }
                       //PrintDirect.WritePrinter(lhPrinter, sbLineBreak.ToString(), sbLineBreak.Length, ref pcWritten);
                    else
                        sbDataTxt.Append(sbLineBreak.ToString());

                    for (int z = 0; z < 31; z++)
                    {
                        if (line == 32)
                            break;
                        sbBreak.AppendLine();
                        line++;
                    }

                    if (sbBreak.Length != 0)
                        if (printStatus == true){}
                           //PrintDirect.WritePrinter(lhPrinter, sbBreak.ToString(), sbBreak.Length, ref pcWritten);
                        else
                            sbDataTxt.Append(sbBreak.ToString());
                    SetDataReportPageBreak();
                    ttlHal++;

                    if (bGenerateHeader)
                    {
                        sbHeaderReport = new StringBuilder();
                        GenerateHeader(rptId, widthPaper, ttlHal, spaceHeader, true);

                        if (spaceHeader == true)
                            line = 6 + lineGroupHeader;
                        else
                            line = 5 + lineGroupHeader;
                    }
                    else line = lineGroupHeader;

                    if (printStatus == true)
                    {
                       //PrintDirect.WritePrinter(lhPrinter, sbHeaderReport.ToString(), sbHeaderReport.Length, ref pcWritten);
                       //PrintDirect.WritePrinter(lhPrinter, sbGroupHeader.ToString(), sbGroupHeader.Length, ref pcWritten);
                       //PrintDirect.WritePrinter(lhPrinter, sbData.ToString(), sbData.Length, ref pcWritten);
                    }
                    else
                    {
                        sbDataTxt.Append(sbHeaderReport.ToString());
                        sbDataTxt.Append(sbGroupHeader.ToString());
                        sbDataTxt.Append(sbData.ToString());
                    }

                    sbData = new StringBuilder();
                    line++;
                    counterData = 1;
                }
                #endregion
            }
        }

        public void PrintData(bool empty5Space, bool bGenerateHeader, bool bPrintBreak)
        {
            StringBuilder sbLineBreak = new StringBuilder();
            StringBuilder sbBreakLine = new StringBuilder();

            sbBreakLine.AppendLine();

            if (kdLine == "W272")
                sbLineBreak.AppendLine("-".PadRight(272, '-'));
            else if (kdLine == "W233")
                sbLineBreak.AppendLine("-".PadRight(233, '-'));
            else if (kdLine == "W163")
                sbLineBreak.AppendLine("-".PadRight(163, '-'));
            else if (kdLine == "W136")
                sbLineBreak.AppendLine("-".PadRight(136, '-'));
            else if (kdLine == "W96")
                sbLineBreak.AppendLine("-".PadRight(96, '-'));
            else if (kdLine == "W80")
                sbLineBreak.AppendLine("-".PadRight(80, '-'));

            if (pageFull == true)
            {
                #region Full Page
                if (line < 60)
                {
                    if (empty5Space == true)
                    {
                        if (counterData < 5)
                        {
                            if (printStatus == true){}
                               //PrintDirect.WritePrinter(lhPrinter, sbData.ToString(), sbData.Length, ref pcWritten);
                            else
                                sbDataTxt.Append(sbData.ToString());
                            sbData = new StringBuilder();
                            counterData++;
                        }
                        else
                        {
                            if (printStatus == true)
                            {
                               //PrintDirect.WritePrinter(lhPrinter, sbBreakLine.ToString(), sbBreakLine.Length, ref pcWritten);
                               //PrintDirect.WritePrinter(lhPrinter, sbData.ToString(), sbData.Length, ref pcWritten);
                            }
                            else
                            {
                                sbDataTxt.Append(sbBreakLine.ToString());
                                sbDataTxt.Append(sbData.ToString());
                            }

                            sbData = new StringBuilder();
                            line++;
                            counterData = 1;
                        }
                    }
                    else
                    {
                        if (printStatus == true){}
                           //PrintDirect.WritePrinter(lhPrinter, sbData.ToString(), sbData.Length, ref pcWritten);
                        else
                            sbDataTxt.Append(sbData.ToString());
                        sbData = new StringBuilder();
                        counterData++;
                    }
                }
                else
                {
                    if (bPrintBreak)
                    {
                        if (printStatus == true){}
                           //PrintDirect.WritePrinter(lhPrinter, sbLineBreak.ToString(), sbLineBreak.Length, ref pcWritten);
                        else
                            sbDataTxt.Append(sbLineBreak.ToString());
                    }

                    SetDataReportPageBreak();
                    ttlHal++;

                    if (bGenerateHeader)
                    {
                        sbHeaderReport = new StringBuilder();
                        GenerateHeader(rptId, widthPaper, ttlHal);
                        line = 6 + lineGroupHeader;
                    }
                    else line = lineGroupHeader;

                    if (printStatus == true)
                    {
                       //PrintDirect.WritePrinter(lhPrinter, sbHeaderReport.ToString(), sbHeaderReport.Length, ref pcWritten);
                       //PrintDirect.WritePrinter(lhPrinter, sbGroupHeader.ToString(), sbGroupHeader.Length, ref pcWritten);
                       //PrintDirect.WritePrinter(lhPrinter, sbData.ToString(), sbData.Length, ref pcWritten);
                    }
                    else
                    {
                        sbDataTxt.Append(sbHeaderReport.ToString());
                        sbDataTxt.Append(sbGroupHeader.ToString());
                        sbDataTxt.Append(sbData.ToString());
                    }

                    sbData = new StringBuilder();
                    line++;
                    counterData = 1;
                }
                #endregion
            }
            else
            {
                #region Half Page
                if (line < 31)
                {
                    if (empty5Space == true)
                    {
                        if (counterData < 5)
                        {
                            if (printStatus == true){}
                               //PrintDirect.WritePrinter(lhPrinter, sbData.ToString(), sbData.Length, ref pcWritten);
                            else
                                sbDataTxt.Append(sbData.ToString());

                            sbData = new StringBuilder();
                            counterData++;
                        }
                        else
                        {
                            if (printStatus == true)
                            {
                               //PrintDirect.WritePrinter(lhPrinter, sbBreakLine.ToString(), sbBreakLine.Length, ref pcWritten);
                               //PrintDirect.WritePrinter(lhPrinter, sbData.ToString(), sbData.Length, ref pcWritten);
                            }
                            else
                            {
                                sbDataTxt.Append(sbBreakLine.ToString());
                                sbDataTxt.Append(sbData.ToString());
                            }

                            sbData = new StringBuilder();
                            line++;
                            counterData = 1;
                        }
                    }
                    else
                    {
                        if (printStatus == true){}
                           //PrintDirect.WritePrinter(lhPrinter, sbData.ToString(), sbData.Length, ref pcWritten);
                        else
                            sbDataTxt.Append(sbData.ToString());
                        sbData = new StringBuilder();
                        counterData++;
                    }
                }
                else
                {
                    StringBuilder sbBreak = new StringBuilder();

                    if (bPrintBreak)
                    {
                        if (printStatus == true){}
                           //PrintDirect.WritePrinter(lhPrinter, sbLineBreak.ToString(), sbLineBreak.Length, ref pcWritten);
                        else
                            sbDataTxt.Append(sbLineBreak.ToString());
                    }
                    else
                    {
                        sbBreak.AppendLine();
                    }

                    for (int z = 0; z < 31; z++)
                    {
                        if (line == 32)
                            break;
                        sbBreak.AppendLine();
                        line++;
                    }

                    if (sbBreak.Length != 0)
                        if (printStatus == true) { }
                           //PrintDirect.WritePrinter(lhPrinter, sbBreak.ToString(), sbBreak.Length, ref pcWritten);
                        else
                            sbDataTxt.Append(sbBreak.ToString());
                    SetDataReportPageBreak();
                    ttlHal++;

                    if (bGenerateHeader)
                    {
                        sbHeaderReport = new StringBuilder();
                        GenerateHeader(rptId, widthPaper, ttlHal, spaceHeader, true);

                        if (spaceHeader == true)
                            line = 6 + lineGroupHeader;
                        else
                            line = 5 + lineGroupHeader;
                    }
                    else line = lineGroupHeader;

                    if (printStatus == true)
                    {
                       //PrintDirect.WritePrinter(lhPrinter, sbHeaderReport.ToString(), sbHeaderReport.Length, ref pcWritten);
                       //PrintDirect.WritePrinter(lhPrinter, sbGroupHeader.ToString(), sbGroupHeader.Length, ref pcWritten);
                       //PrintDirect.WritePrinter(lhPrinter, sbData.ToString(), sbData.Length, ref pcWritten);
                    }
                    else
                    {
                        sbDataTxt.Append(sbHeaderReport.ToString());
                        sbDataTxt.Append(sbGroupHeader.ToString());
                        sbDataTxt.Append(sbData.ToString());
                    }

                    sbData = new StringBuilder();
                    line++;
                    counterData = 1;
                }
                #endregion
            }
        }

        public void PrintHeader()
        {
            if (printStatus == true)
            {
               //PrintDirect.WritePrinter(lhPrinter, sbHeaderReport.ToString(), sbHeaderReport.Length, ref pcWritten);
               //PrintDirect.WritePrinter(lhPrinter, sbGroupHeader.ToString(), sbGroupHeader.Length, ref pcWritten);
            }
            else
            {
                sbDataTxt.Append(sbHeaderReport.ToString());
                sbDataTxt.Append(sbGroupHeader.ToString());
            }
            ttlHal++;
        }

        public bool PrintTotal()
        {
            return PrintTotal(true, false);
        }

        public bool PrintTotal(bool writeFile)
        {
            return PrintTotal(writeFile, false);
        }

        public bool PrintTotal(bool writeFile, bool lastData)
        {
            return PrintTotal(writeFile, lastData, true);
        }

        public bool PrintTotal(bool writeFile, bool lastData, bool createBy)
        {
            try
            {
                newLine = false;
                //SysUser user = SysUser.Current;
                string createUser = "Dicetak Oleh : " + CurrentUser.UserId;
                string papperFeed = "\f";

                StringBuilder sbBreakLine = new StringBuilder();
                sbBreakLine.AppendLine();
                StringBuilder sbCreateBy = new StringBuilder();

                if (kdLine == "W272")
                    sbCreateBy.Append(footerLabel.PadRight(242, ' ').Substring(0, 242));
                else if (kdLine == "W233")
                    sbCreateBy.Append(footerLabel.PadRight(203, ' ').Substring(0, 203));
                else if (kdLine == "W163")
                    sbCreateBy.Append(footerLabel.PadRight(133, ' ').Substring(0, 133));
                else if (kdLine == "W136")
                    sbCreateBy.Append(footerLabel.PadRight(106, ' ').Substring(0, 106));
                else if (kdLine == "W96")
                    sbCreateBy.Append(footerLabel.PadRight(66, ' ').Substring(0, 66));
                else if (kdLine == "W80")
                    sbCreateBy.Append(footerLabel.PadRight(50, ' ').Substring(0, 50));

                if (createBy == true)
                    sbCreateBy.Append(createUser.PadLeft(28, ' '));
                else
                    sbCreateBy.Append(" ");

                line++;
                totalLine++;

                if (pageFull == true)
                {
                    #region Full Page
                    if (line <= 60 && lastData == true)
                    {
                        if (printStatus == true)
                        {
                            if (counterData < 5){}
                               //PrintDirect.WritePrinter(lhPrinter, sbData.ToString(), sbData.Length, ref pcWritten);
                            else
                            {
                                if (line < 60){}//PrintDirect.WritePrinter(lhPrinter, sbBreakLine.ToString(), sbBreakLine.Length, ref pcWritten);
                               //PrintDirect.WritePrinter(lhPrinter, sbData.ToString(), sbData.Length, ref pcWritten);
                            }
                        }
                        else
                        {
                            if (counterData < 5)
                                sbDataTxt.Append(sbData.ToString());
                            else
                            {
                                if (line < 60) sbDataTxt.Append(sbBreakLine.ToString());
                                sbDataTxt.Append(sbData.ToString());
                            }
                        }
                    }
                    for (int i = 0; i < 60; i++)
                    {
                        if (line == 60 || line == 61 || line == 62 || line == 63)
                        {
                            if (printStatus == true)
                            {
                               //PrintDirect.WritePrinter(lhPrinter, sbTotalDetail.ToString(), sbTotalDetail.Length, ref pcWritten);
                               //PrintDirect.WritePrinter(lhPrinter, sbCreateBy.ToString(), sbCreateBy.Length, ref pcWritten);
                               //PrintDirect.WritePrinter(lhPrinter, papperFeed.ToString(), papperFeed.Length, ref pcWritten);
                                break;
                            }
                            else
                            {
                                sbDataTxt.Append(sbTotalDetail.ToString());
                                sbDataTxt.AppendLine(sbCreateBy.ToString());
                                break;
                            }
                        }
                        else if (line > 63)
                        {
                            if (lastData == true)
                                CheckLastLineforTotal();

                            if (spaceHeader == true)
                            {
                                GenerateHeader(rptId, widthPaper, ttlHal, true, true);
                                line = 6 + lineGroupHeader;
                            }
                            else
                            {
                                GenerateHeader(rptId, widthPaper, ttlHal, false, true);
                                line = 5 + lineGroupHeader;
                            }

                            if (printStatus == true)
                            {
                               //PrintDirect.WritePrinter(lhPrinter, sbHeaderReport.ToString(), sbHeaderReport.Length, ref pcWritten);
                               //PrintDirect.WritePrinter(lhPrinter, sbGroupHeader.ToString(), sbGroupHeader.Length, ref pcWritten);

                                if (lastData == true){}
                                   //PrintDirect.WritePrinter(lhPrinter, sbData.ToString(), sbData.Length, ref pcWritten);
                            }
                            else
                            {
                                sbDataTxt.Append(sbHeaderReport.ToString());
                                sbDataTxt.Append(sbGroupHeader.ToString());

                                if (lastData == true)
                                    sbDataTxt.Append(sbData.ToString());
                            }

                            for (int j = 0; j < 60; j++)
                            {
                                if (printStatus == true && line == 63){}
                                   //PrintDirect.WritePrinter(lhPrinter, sbTotalDetail.ToString(), sbTotalDetail.Length, ref pcWritten);
                                else if (printStatus == false && line == 63)
                                {
                                    sbDataTxt.Append(sbData.ToString());
                                    line++;
                                }

                                StringBuilder sbLineBreakInner = new StringBuilder();
                                sbLineBreakInner.AppendLine();

                                if (printStatus == true){}
                                   //PrintDirect.WritePrinter(lhPrinter, sbLineBreakInner.ToString(), sbLineBreakInner.Length, ref pcWritten);
                                else
                                    sbDataTxt.Append(sbLineBreakInner.ToString());

                                line++;
                            }
                        }

                        StringBuilder sbLineBreak = new StringBuilder();
                        sbLineBreak.AppendLine();

                        if (printStatus == true){}
                           //PrintDirect.WritePrinter(lhPrinter, sbLineBreak.ToString(), sbLineBreak.Length, ref pcWritten);
                        else
                            sbDataTxt.Append(sbLineBreak.ToString());

                        line++;

                    }

                    if (printStatus == false)
                    {
                        if (writeFile == true)
                            SaveTextFile();
                    }

                    sbTotalDetail = new StringBuilder();
                    totalLine = 0;
                    counterData = 0;
                    return true;
                    #endregion
                }
                else
                {
                    StringBuilder sbLineBreak = new StringBuilder();
                    sbLineBreak.AppendLine();

                    if (line <= 31 && lastData == true)
                    {
                        if (printStatus == true){}
                           //PrintDirect.WritePrinter(lhPrinter, sbData.ToString(), sbData.Length, ref pcWritten);
                        else
                            sbDataTxt.Append(sbData.ToString());

                        //line++;
                    }

                    for (int i = 0; i < 31; i++)
                    {
                        if (line == 31)
                        {
                            if (printStatus == true)
                            {
                               //PrintDirect.WritePrinter(lhPrinter, sbTotalDetail.ToString(), sbTotalDetail.Length, ref pcWritten);
                               //PrintDirect.WritePrinter(lhPrinter, sbCreateBy.ToString(), sbCreateBy.Length, ref pcWritten);
                               //PrintDirect.WritePrinter(lhPrinter, sbLineBreak.ToString(), sbLineBreak.Length, ref pcWritten);
                               //PrintDirect.WritePrinter(lhPrinter, sbLineBreak.ToString(), sbLineBreak.Length, ref pcWritten);
                                break;
                            }
                            else
                            {
                                sbDataTxt.Append(sbTotalDetail.ToString());
                                sbDataTxt.Append(sbCreateBy.ToString());
                                sbDataTxt.Append(sbLineBreak.ToString());
                                sbDataTxt.Append(sbLineBreak.ToString());
                                break;
                            }
                        }
                        else if (line > 31)
                        {
                            if (lastData == true)
                                CheckLastLineforTotal();

                            if (spaceHeader == true)
                            {
                                GenerateHeader(rptId, widthPaper, ttlHal, true, true);
                                line = 6 + lineGroupHeader + totalLine;
                            }
                            else
                            {
                                GenerateHeader(rptId, widthPaper, ttlHal, false, true);
                                line = 5 + lineGroupHeader + totalLine;
                            }

                            if (printStatus == true)
                            {
                               //PrintDirect.WritePrinter(lhPrinter, sbHeaderReport.ToString(), sbHeaderReport.Length, ref pcWritten);
                               //PrintDirect.WritePrinter(lhPrinter, sbGroupHeader.ToString(), sbGroupHeader.Length, ref pcWritten);
                                if (lastData == true)
                                {
                                   //PrintDirect.WritePrinter(lhPrinter, sbData.ToString(), sbData.Length, ref pcWritten);
                                    line++;
                                }
                            }
                            else
                            {
                                sbDataTxt.Append(sbHeaderReport.ToString());
                                sbDataTxt.Append(sbGroupHeader.ToString());
                                if (lastData == true)
                                {
                                    sbDataTxt.Append(sbData.ToString());

                                    line++;
                                }
                            }
                            newLine = true;
                        }
                        else
                        {
                            if (printStatus == true) { }
                               //PrintDirect.WritePrinter(lhPrinter, sbLineBreak.ToString(), sbLineBreak.Length, ref pcWritten);
                            else
                                sbDataTxt.AppendLine(" ");

                            line++;
                        }
                    }

                    if (printStatus == false)
                    {
                        if (writeFile == true)
                            SaveTextFile();
                    }

                    sbTotalDetail = new StringBuilder();
                    totalLine = 0;
                    counterData = 0;
                    return true;
                }


            }
            catch
            {
                return false;
            }
        }
        #endregion
    }
}