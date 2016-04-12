using System.Runtime.InteropServices;
using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SimDms.Web.Models;
using System.IO;

namespace SimDms.Web.Reports.Service
{

     public class GenerateTextFileReport
     {
         public SysUser CurrentUser;
         #region Initialize
         //SysUser user = SysUser.Current;

         public StringBuilder sbHeaderReport = new StringBuilder();
         public StringBuilder sbGroupHeader = new StringBuilder();
         public StringBuilder sbTotalDetail = new StringBuilder();
         public StringBuilder sbData = new StringBuilder();

         public StringBuilder sbDataTxt = new StringBuilder();

         //private DataSet dsHeader = null;
         //private DataTable dtDetail = null;
         private string rptId = "", st1 = "", resultString = "", fileLoc = "", footerLabel = "";

         ////PrintDirect direct = new //PrintDirect();
         public int line = 0, pcWritten = 0, counterData = 0, widthPaper = 0, heightPaper = 0, lineHeader = 0, lineGroupHeader = 0, ttlHal = 0, totalLine = 0, ttlLineData = 0;
         int totalCountLine = 99999;
         string kdLine = "W272";
         System.IntPtr lhPrinter = new System.IntPtr();
         private bool printStatus = false, pageFull = true, spaceHeader = true, newLine = false;

         #endregion

         #region Method

         private string GetIndMonthName(object obj)
         {
             string retVal = "";
             try
             {
                 int i = Convert.ToInt32(obj);
                 if (i < 1 || i > 12) i = 1;
                 retVal = GetIndonesianMonth(i);
             }
             catch (Exception ex)
             {
                 Console.WriteLine(ex);
             }
             return retVal;
         }

         public string GetIndonesianMonth(int month)
         {
             string[] fullMonths = new string[] { "", "Januari", "Februari", "Maret", "April", "Mei", "Juni", "Juli", "Agustus", "September", "Oktober", "November", "Desember" };
             return fullMonths[month];
         }

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

         public string[] ConvertArrayText(string text, int spaceLength)
         {
             string[] tempArray = new string[10];

             if (text.Length < spaceLength)
             {
                 if (text.Length + 6 < spaceLength)
                     tempArray[0] = text;
             }
             else
             {
                 string[] splitTemp = text.Split(' ');
                 string hasilSplit = "";
                 int counter = 0;

                 for (int i = 0; i < splitTemp.Length; i++)
                 {
                     if (i != splitTemp.Length - 1)
                     {
                         if (hasilSplit.Length + splitTemp[i].Length < spaceLength)
                         {
                             hasilSplit += splitTemp[i].ToString().Trim() + " ";
                         }
                         else
                         {
                             tempArray[counter] = hasilSplit;
                             hasilSplit = splitTemp[i].ToString().Trim() + " ";
                             counter++;
                         }
                     }
                     else
                     {
                         if (hasilSplit.Length + splitTemp[i].Length < spaceLength)
                         {
                             hasilSplit += splitTemp[i].ToString().Trim() + " ";
                         }
                         else
                         {
                             tempArray[counter] = hasilSplit;
                             hasilSplit = splitTemp[i].ToString().Trim() + " ";
                             counter++;
                             tempArray[counter] = hasilSplit;
                         }

                     }
                 }
                 tempArray[counter] = hasilSplit;
             }
             return tempArray;
         }

         public string[] ConvertArrayList(string text, int spaceLength, int arrayLength)
         {
             string[] tempArray = new string[arrayLength];
             string[] listArray = text.Split('\n');
             string overSpace = "";
             int counter = 0;

             foreach (string row in listArray)
             {
                 if (row.Length <= spaceLength)
                     tempArray[counter] = row.Replace('\r', ' ');
                 else
                 {
                     tempArray[counter] = row.Substring(0, spaceLength);
                     overSpace = row.Substring(0, spaceLength);
                     if (row.Length - overSpace.Length > spaceLength)
                     {
                         counter++;
                         tempArray[counter] = row.Substring(spaceLength, spaceLength );
                         overSpace = row.Substring(spaceLength, spaceLength );
                         if (row.Length - spaceLength * 2 > spaceLength)
                         {
                             counter++;
                             tempArray[counter] = row.Substring(spaceLength * 2, spaceLength);
                             tempArray[counter] = tempArray[counter].Replace('\r', ' ');
                         }
                         else
                             tempArray[counter] = tempArray[counter].Replace('\r', ' ');
                     }
                 }
                 counter++;
             }
             if (counter != arrayLength)
             {
                 int currentCounter = counter;
                 for (int i = 0; i < arrayLength - currentCounter; i++)
                 {
                     tempArray[counter] = "";
                     counter++;
                 }
             }

             //if (text.Length < spaceLength)
             //{
             //    if (text.Length + 6 < spaceLength)
             //        tempArray[0] = text;
             //}
             //else
             //{
             //    string[] splitTemp = text.Split(' ');
             //    string hasilSplit = "";
             //    int innerCounter = 0;

             //    for (int i = 0; i < splitTemp.Length; i++)
             //    {
             //        if (i != splitTemp.Length - 1)
             //        {
             //            if (hasilSplit.Length + splitTemp[i].Length < spaceLength)
             //            {
             //                hasilSplit += splitTemp[i].ToString() + " ";
             //            }
             //            else
             //            {
             //                tempArray[counter] = hasilSplit;
             //                hasilSplit = splitTemp[i].ToString() + " ";
             //                counter++;
             //            }
             //        }
             //        else
             //        {
             //            if (hasilSplit.Length + splitTemp[i].Length < spaceLength)
             //            {
             //                hasilSplit += splitTemp[i].ToString() + " ";
             //            }
             //            else
             //            {
             //                tempArray[counter] = hasilSplit;
             //                hasilSplit = splitTemp[i].ToString() + " ";
             //                counter++;
             //                tempArray[counter] = hasilSplit;
             //            }

             //        }
             //    }
             //    tempArray[counter] = hasilSplit;
             //}
             return tempArray;
         }

         public string GetIndonesianDate(DateTime dt)
         {
             string[] fullMonths = new string[] {"", "JANUARI", "FEBRUARI", "MARET", "APRIL", "MEI", "JUNI", 
                "JULI", "AGUSTUS", "SEPTEMBER", "OKTOBER", "NOPEMBER", "DESEMBER"};

             string[] halfMonths = new string[] {"", "JAN", "FEB", "MAR", "APR", "MEI", "JUN", "JUL", "AGS", 
                "SEP", "OKT", "NOP", "DES"};

             string longFormat = dt.Day.ToString() + " " + fullMonths[dt.Month] + " " + dt.Year.ToString();
             string shortFormat = dt.Day.ToString() + "-" + halfMonths[dt.Month] + "-" + dt.Year.ToString();
             string customFormat = fullMonths[dt.Month] + " " + dt.Year.ToString();
             return "";
             //            return df.Equals(Format.LongFormat) ? longFormat : df.Equals(Format.ShortFormat) ? shortFormat :
             //              df.Equals(Format.CustomFormat) ? customFormat : string.Empty;
         }

         public void PrintAfterBreak()
         {
             PrintAfterBreak(true);
         }

         public void PrintAfterBreak(bool spaceHeader)
         {
             ttlHal++;

             sbHeaderReport = new StringBuilder();
             if (spaceHeader == true)
             {
                 GenerateHeader(rptId, widthPaper, ttlHal);
                 line = line + lineGroupHeader;
             }
             else
             {
                 GenerateHeader(rptId, widthPaper, ttlHal, false);
                 line = line + lineGroupHeader;
             }

             if (printStatus == true)
             {
                 PrintDirect.WritePrinter(lhPrinter, sbHeaderReport.ToString(), sbHeaderReport.Length, ref pcWritten);
                 PrintDirect.WritePrinter(lhPrinter, sbGroupHeader.ToString(), sbGroupHeader.Length, ref pcWritten);
             }
             else
             {
                 sbDataTxt.Append(sbHeaderReport.ToString());
                 sbDataTxt.Append(sbGroupHeader.ToString());
             }

             sbData = new StringBuilder();
             counterData = 0;
         }

         public void PrintAfterBreakSamePage()
         {
             StringBuilder sbLineBreak = new StringBuilder();
             StringBuilder sbLine = new StringBuilder();
             sbLineBreak.AppendLine();

             string papperFeed = "\f";

             if (kdLine == "W272")
                 sbLine.AppendLine("-".PadRight(272, '-'));
             else if (kdLine == "W233")
                 sbLine.AppendLine("-".PadRight(233, '-'));
             else if (kdLine == "W163")
                 sbLine.AppendLine("-".PadRight(163, '-'));
             else if (kdLine == "W163A")
                 sbLine.AppendLine("-".PadRight(163, '-'));
             else if (kdLine == "W136")
                 sbLine.AppendLine("-".PadRight(136, '-'));
             else if (kdLine == "W96")
                 sbLine.AppendLine("-".PadRight(96, '-'));
             else if (kdLine == "W80")
                 sbLine.AppendLine("-".PadRight(80, '-'));

             if (line <= 63)
             {
                 if (printStatus == true)

                     PrintDirect.WritePrinter(lhPrinter, sbGroupHeader.ToString(), sbGroupHeader.Length, ref pcWritten);
                 else
                     sbDataTxt.Append(sbGroupHeader.ToString());

                 sbData = new StringBuilder();
                 counterData = 0;
             }
             else if (line > 63)
             {
                 line = line - lineGroupHeader;
                 //line = line - ttlLineData;

                 for (int i = 0; i < 60; i++)
                 {
                     if (line == 60 || line == 61 || line == 62 || line == 63)
                     {
                         if (printStatus == true)
                         {
                             if (sbTotalDetail != null)
                                 PrintDirect.WritePrinter(lhPrinter, sbTotalDetail.ToString(), sbTotalDetail.Length, ref pcWritten);
                             else
                                 PrintDirect.WritePrinter(lhPrinter, sbLine.ToString(), sbLine.Length, ref pcWritten);
                             PrintDirect.WritePrinter(lhPrinter, papperFeed.ToString(), papperFeed.Length, ref pcWritten);
                             break;
                         }
                         else
                         {
                             if (sbTotalDetail != null)
                                 sbDataTxt.Append(sbTotalDetail.ToString());
                             else
                                 sbDataTxt.Append(sbLine.ToString());

                             break;
                         }
                     }
                     else if (line > 63)
                     {
                         if (printStatus == true)
                         {
                             PrintDirect.WritePrinter(lhPrinter, sbGroupHeader.ToString(), sbGroupHeader.Length, ref pcWritten);
                         }
                         else
                         {
                             sbDataTxt.Append(sbGroupHeader.ToString());
                         }

                         for (int j = 0; j < 60; j++)
                         {
                             if (printStatus == true && line == 63)
                                 PrintDirect.WritePrinter(lhPrinter, sbTotalDetail.ToString(), sbTotalDetail.Length, ref pcWritten);
                             else if (printStatus == false && line == 63)
                             {
                                 sbDataTxt.Append(sbData.ToString());
                                 line++;
                                 break;
                             }

                             StringBuilder sbLineBreakInner = new StringBuilder();
                             sbLineBreakInner.AppendLine();

                             if (printStatus == true)
                                 PrintDirect.WritePrinter(lhPrinter, sbLineBreakInner.ToString(), sbLineBreakInner.Length, ref pcWritten);
                             else
                                 sbDataTxt.Append(sbLineBreakInner.ToString());

                             line++;
                         }
                     }

                     if (printStatus == true)
                         PrintDirect.WritePrinter(lhPrinter, sbLineBreak.ToString(), sbLineBreak.Length, ref pcWritten);
                     else
                         sbDataTxt.Append(sbLineBreak.ToString());

                     line++;
                 }
             }
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
             else if (kdLine == "W163A")
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
                         if (printStatus == true)
                             PrintDirect.WritePrinter(lhPrinter, sbLineBreak.ToString(), sbLineBreak.Length, ref pcWritten);
                         else
                             sbDataTxt.Append(sbLineBreak.ToString());

                         if (line == 60)
                         {
                             if (printStatus == true)
                             {
                                 PrintDirect.WritePrinter(lhPrinter, sbLine.ToString(), sbLine.Length, ref pcWritten);
                                 PrintDirect.WritePrinter(lhPrinter, papperFeed.ToString(), papperFeed.Length, ref pcWritten);
                             }
                             else
                             {
                                 sbDataTxt.AppendLine(sbLineBreak.ToString());
                                 sbDataTxt.AppendLine(sbLineBreak.ToString());
                                 sbDataTxt.AppendLine(sbLineBreak.ToString());
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
                         if (printStatus == true)
                             PrintDirect.WritePrinter(lhPrinter, sbLineBreak.ToString(), sbLineBreak.Length, ref pcWritten);
                         else
                             sbDataTxt.Append(sbLineBreak.ToString());

                         if (line == 30)
                         {
                             if (printStatus == true)
                             {
                                 PrintDirect.WritePrinter(lhPrinter, sbLine.ToString(), sbLine.Length, ref pcWritten);
                                 PrintDirect.WritePrinter(lhPrinter, sbLineBreak.ToString(), sbLineBreak.Length, ref pcWritten);
                                 PrintDirect.WritePrinter(lhPrinter, sbLineBreak.ToString(), sbLineBreak.Length, ref pcWritten);
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
                 //}
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
             PrintDirect.EndPagePrinter(lhPrinter);
             PrintDirect.EndDocPrinter(lhPrinter);
             PrintDirect.ClosePrinter(lhPrinter);
         }

         public void GenerateHeader()
         {
             GenerateHeader(rptId, widthPaper, 0);
         }

         public void GenerateHeader(bool spaceHeaders)
         {
             GenerateHeader(rptId, widthPaper, 0, spaceHeaders);
         }

         public void GenerateHeader(string reportID, int width, int page)
         {
             GenerateHeader(reportID, width, page, true);
         }

         private void GenerateHeader(string reportID, int width, int page, bool spaceHeaders)
         {
             spaceHeader = spaceHeaders;
             sbHeaderReport = new StringBuilder();
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
                 //IDbContext ctx = DbFactory.Configure();

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
             else if (kdLine == "W163A")
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

             DOCINFO di = new DOCINFO();

             //text to print with a form feed character
             di.pDocName = reportID;
             di.pDataType = "RAW";

             //SysParameter oSysParameter = SysParameterBLL.GetRecord("DEFAULT_PRINTER_NAME");

             //Setting Printer
             if (print == true)
             {
                 PrintDirect.OpenPrinter(location, ref lhPrinter, 0);

                 PrintDirect.StartDocPrinter(lhPrinter, 1, ref di);
                 PrintDirect.StartPagePrinter(lhPrinter);


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
             else if (kdLine == "W163A")
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
             sbDataTxt.AppendLine("--pg--");
             //PrintDirect.WritePrinter(lhPrinter, "\f", 2, ref pcWritten);
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
                         line++; ttlLineData++;
                     }
                     else
                     {
                         sbData.AppendLine(value.PadLeft(widthChar, charFill).Substring(0, widthChar));
                         line++; ttlLineData++;
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
                         line++; ttlLineData++;
                     }
                     else
                     {
                         sbData.AppendLine(valueMoney.ToString(formatNumber).PadLeft(widthChar, ' ').Substring(0, widthChar));
                         line++; ttlLineData++;
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
             else if (kdLine == "W163A")
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

         public void ReplaceGroupHdr(string oldValue, string newValue, int maxChar)
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

         public void SetGroupHeaderLine()
         {
             if (kdLine == "W272")
                 sbGroupHeader.AppendLine("-".PadRight(272, '-'));
             else if (kdLine == "W233")
                 sbGroupHeader.AppendLine("-".PadRight(233, '-'));
             else if (kdLine == "W163")
                 sbGroupHeader.AppendLine("-".PadRight(163, '-'));
             else if (kdLine == "W163A")
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
             PrintData(empty5Space, false,false);
         }
         
         public void PrintData(bool empty5Space, bool pageBreak,bool eodata)
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
             else if (kdLine == "W163A")
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
                 if(eodata)
                 {
                     if (line > 50)
                         line = 60;
                 }
                 if (line < 60)
                 {
                     if (empty5Space == true)
                     {
                         if (counterData < 5)
                         {
                             if (printStatus == true)
                                 PrintDirect.WritePrinter(lhPrinter, sbData.ToString(), sbData.Length, ref pcWritten);
                             else
                                 sbDataTxt.Append(sbData.ToString());
                             sbData = new StringBuilder();
                             counterData++;
                         }
                         else
                         {
                             if (printStatus == true)
                             {
                                 PrintDirect.WritePrinter(lhPrinter, sbBreakLine.ToString(), sbBreakLine.Length, ref pcWritten);
                                 PrintDirect.WritePrinter(lhPrinter, sbData.ToString(), sbData.Length, ref pcWritten);
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
                         if (printStatus == true)
                             PrintDirect.WritePrinter(lhPrinter, sbData.ToString(), sbData.Length, ref pcWritten);
                         else
                             sbDataTxt.Append(sbData.ToString());
                         sbData = new StringBuilder();
                         counterData = 0;
                     }
                 }
                 else
                 {
                     //if (printStatus == true)
                     //    PrintDirect.WritePrinter(lhPrinter, sbLineBreak.ToString(), sbLineBreak.Length, ref pcWritten);
                     //else
                         sbDataTxt.Append(sbLineBreak.ToString());

                     SetDataReportPageBreak();
                     ttlHal++;

                     sbHeaderReport = new StringBuilder();

                     GenerateHeader(rptId, widthPaper, ttlHal);

                     line = 6 + lineGroupHeader;
                     //if (printStatus == true)
                     //{
                     //    PrintDirect.WritePrinter(lhPrinter, sbHeaderReport.ToString(), sbHeaderReport.Length, ref pcWritten);
                     //    PrintDirect.WritePrinter(lhPrinter, sbGroupHeader.ToString(), sbGroupHeader.Length, ref pcWritten);
                     //    PrintDirect.WritePrinter(lhPrinter, sbData.ToString(), sbData.Length, ref pcWritten);
                     //}
                     //else
                     //{
                         sbDataTxt.Append(sbHeaderReport.ToString());
                         sbDataTxt.Append(sbGroupHeader.ToString());
                         sbDataTxt.Append(sbData.ToString());
                    // }

                     sbData = new StringBuilder();
                     line += ttlLineData;
                     counterData = 1;
                 }
                 #endregion

                 if (pageBreak == true)
                 {
                     if (line < 60)
                     {
                         for (int z = 0; z < 60; z++)
                         {
                             if (line == 60)
                             {
                                 //if (printStatus == true)
                                 //    PrintDirect.WritePrinter(lhPrinter, sbLineBreak.ToString(), sbLineBreak.Length, ref pcWritten);
                                 //else
                                     sbDataTxt.Append(sbLineBreak.ToString());

                                 SetDataReportPageBreak();
                                 ttlHal++;

                                 sbHeaderReport = new StringBuilder();

                                 GenerateHeader(rptId, widthPaper, ttlHal);

                                 line = 6 + lineGroupHeader;
                                 //if (printStatus == true)
                                 //{
                                 //    PrintDirect.WritePrinter(lhPrinter, sbHeaderReport.ToString(), sbHeaderReport.Length, ref pcWritten);
                                 //    PrintDirect.WritePrinter(lhPrinter, sbGroupHeader.ToString(), sbGroupHeader.Length, ref pcWritten);
                                 //}
                                 //else
                                 //{
                                     sbDataTxt.Append(sbHeaderReport.ToString());
                                     sbDataTxt.Append(sbGroupHeader.ToString());
                                 //}

                                 sbData = new StringBuilder();
                                 line += ttlLineData;
                                 counterData = 1;
                                 break;
                             }

                             //if (printStatus == true)
                             //    PrintDirect.WritePrinter(lhPrinter, sbBreakLine.ToString(), sbBreakLine.Length, ref pcWritten);
                             //else
                                 sbDataTxt.Append(sbBreakLine.ToString());
                             line++;
                         }
                     }
                 }
             }
             else
             {
                 #region Half Page
                 if (eodata)
                 {
                     if (line > 20)
                         line = 30;
                 }
                 if (line < 30)
                 {
                     if (empty5Space == true)
                     {
                         if (counterData < 5)
                         {
                             //if (printStatus == true)
                             //    PrintDirect.WritePrinter(lhPrinter, sbData.ToString(), sbData.Length, ref pcWritten);
                             //else
                                 sbDataTxt.Append(sbData.ToString());

                             sbData = new StringBuilder();
                             counterData++;
                         }
                         else
                         {
                             //if (printStatus == true)
                             //{
                             //    PrintDirect.WritePrinter(lhPrinter, sbBreakLine.ToString(), sbBreakLine.Length, ref pcWritten);
                             //    PrintDirect.WritePrinter(lhPrinter, sbData.ToString(), sbData.Length, ref pcWritten);
                             //}
                             //else
                             //{
                                 sbDataTxt.Append(sbBreakLine.ToString());
                                 sbDataTxt.Append(sbData.ToString());
                            // }

                             sbData = new StringBuilder();
                             line++;
                             counterData = 1;
                         }
                     }
                     else
                     {
                         //if (printStatus == true)
                         //    PrintDirect.WritePrinter(lhPrinter, sbData.ToString(), sbData.Length, ref pcWritten);
                         //else
                             sbDataTxt.Append(sbData.ToString());
                         sbData = new StringBuilder();
                         counterData = 0;
                     }
                 }
                 else
                 {
                     StringBuilder sbBreak = new StringBuilder();

                     //if (printStatus == true)
                     //    PrintDirect.WritePrinter(lhPrinter, sbLineBreak.ToString(), sbLineBreak.Length, ref pcWritten);
                     //else
                         sbDataTxt.Append(sbLineBreak.ToString());

                     for (int z = 0; z < 30; z++)
                     {
                         if (line == 33)
                             break;
                         sbBreak.AppendLine();
                         line++;
                     }

                     if (sbBreak.Length != 0)
                         //if (printStatus == true)
                         //    PrintDirect.WritePrinter(lhPrinter, sbBreak.ToString(), sbBreak.Length, ref pcWritten);
                         //else
                             sbDataTxt.Append(sbBreak.ToString());
                     SetDataReportPageBreak();
                     ttlHal++;

                     sbHeaderReport = new StringBuilder();
                     GenerateHeader(rptId, widthPaper, ttlHal, spaceHeader);

                     if (spaceHeader == true)
                         line = 6 + lineGroupHeader;
                     else
                         line = 5 + lineGroupHeader;

                     //if (printStatus == true)
                     //{
                     //    PrintDirect.WritePrinter(lhPrinter, sbHeaderReport.ToString(), sbHeaderReport.Length, ref pcWritten);
                     //    PrintDirect.WritePrinter(lhPrinter, sbGroupHeader.ToString(), sbGroupHeader.Length, ref pcWritten);
                     //    PrintDirect.WritePrinter(lhPrinter, sbData.ToString(), sbData.Length, ref pcWritten);
                     //}
                     //else
                     //{
                         sbDataTxt.Append(sbHeaderReport.ToString());
                         sbDataTxt.Append(sbGroupHeader.ToString());
                         sbDataTxt.Append(sbData.ToString());
                     //}

                     sbData = new StringBuilder();
                     line++;
                     counterData = 1;
                 }
                 #endregion
             }
             ttlLineData = 0;
         }

         public void PrintHeader()
         {
             //if (printStatus == true)
             //{
             //    PrintDirect.WritePrinter(lhPrinter, sbHeaderReport.ToString(), sbHeaderReport.Length, ref pcWritten);
             //    PrintDirect.WritePrinter(lhPrinter, sbGroupHeader.ToString(), sbGroupHeader.Length, ref pcWritten);
             //}
             //else
             //{
                 sbDataTxt.Append(sbHeaderReport.ToString());
                 sbDataTxt.Append(sbGroupHeader.ToString());
            // }
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
             return PrintTotal(writeFile, lastData, createBy, false);
         }

         public bool PrintTotal(bool writeFile, bool lastData, bool createBy, bool clearGroupHeader)
         {
             try
             {
                 newLine = false;
                 //SysUser user = SysUser.Current;
                 string createUser = "Dicetak Oleh : " + CurrentUser.UserId; //user.UserId;
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
                 else if (kdLine == "W163A")
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
                     if (line < 60 && lastData == true)
                     {
                         //if (printStatus == true)
                         //{
                         //    if (counterData < 5)
                         //        PrintDirect.WritePrinter(lhPrinter, sbData.ToString(), sbData.Length, ref pcWritten);
                         //    else
                         //    {
                         //        PrintDirect.WritePrinter(lhPrinter, sbBreakLine.ToString(), sbBreakLine.Length, ref pcWritten);
                         //        PrintDirect.WritePrinter(lhPrinter, sbData.ToString(), sbData.Length, ref pcWritten);
                         //    }
                         //}
                         //else
                         //{
                             if (counterData < 5)
                                 sbDataTxt.Append(sbData.ToString());
                             else
                             {
                                 sbDataTxt.Append(sbBreakLine.ToString());
                                 sbDataTxt.Append(sbData.ToString());
                             }
                         //}
                     }
                     for (int i = 0; i < 60; i++)
                     {
                         if (line == 60 || line == 61 || line == 62 || line == 63)
                         {
                             //if (printStatus == true)
                             //{
                             //    PrintDirect.WritePrinter(lhPrinter, sbTotalDetail.ToString(), sbTotalDetail.Length, ref pcWritten);
                             //    PrintDirect.WritePrinter(lhPrinter, sbCreateBy.ToString(), sbCreateBy.Length, ref pcWritten);
                             //    PrintDirect.WritePrinter(lhPrinter, papperFeed.ToString(), papperFeed.Length, ref pcWritten);
                             //    break;
                             //}
                             //else
                             //{
                                 sbDataTxt.Append(sbTotalDetail.ToString());
                                 sbDataTxt.AppendLine(sbCreateBy.ToString());
                                 break;
                             //}
                         }
                         else if (line > 63)
                         {
                             SetDataReportPageBreak();
                             if (lastData == true)
                                 CheckLastLineforTotal();

                             ttlHal += 1;
                             if (spaceHeader == true)
                             {
                                 GenerateHeader(rptId, widthPaper, ttlHal, true);
                                 line = 6 + lineGroupHeader;
                             }
                             else
                             {
                                 GenerateHeader(rptId, widthPaper, ttlHal, false);
                                 line = 5 + lineGroupHeader;
                             }

                             //if (printStatus == true)
                             //{
                             //    PrintDirect.WritePrinter(lhPrinter, sbHeaderReport.ToString(), sbHeaderReport.Length, ref pcWritten);
                             //    PrintDirect.WritePrinter(lhPrinter, sbGroupHeader.ToString(), sbGroupHeader.Length, ref pcWritten);

                             //    if (lastData == true)
                             //        PrintDirect.WritePrinter(lhPrinter, sbData.ToString(), sbData.Length, ref pcWritten);
                             //}
                             //else
                             //{
                                 sbDataTxt.Append(sbHeaderReport.ToString());
                                 sbDataTxt.Append(sbGroupHeader.ToString());

                                 if (lastData == true)
                                     sbDataTxt.Append(sbData.ToString());
                             //}

                             for (int j = 0; j < 33; j++)
                             {
                                 //if (printStatus == true && line == 63)
                                 //    PrintDirect.WritePrinter(lhPrinter, sbTotalDetail.ToString(), sbTotalDetail.Length, ref pcWritten);
                                 //else if (printStatus == false && line == 63)
                                 //{
                                 //    sbDataTxt.Append(sbData.ToString());
                                 //    line++;
                                 //}

                                 StringBuilder sbLineBreakInner = new StringBuilder();
                                 sbLineBreakInner.AppendLine();

                                 //if (printStatus == true)
                                 //    PrintDirect.WritePrinter(lhPrinter, sbLineBreakInner.ToString(), sbLineBreakInner.Length, ref pcWritten);
                                 //else
                                     sbDataTxt.Append(sbLineBreakInner.ToString());

                                 line++;
                             }
                         }

                         StringBuilder sbLineBreak = new StringBuilder();
                         sbLineBreak.AppendLine();

                         if (printStatus == true)
                             PrintDirect.WritePrinter(lhPrinter, sbLineBreak.ToString(), sbLineBreak.Length, ref pcWritten);
                         else
                             sbDataTxt.Append(sbLineBreak.ToString());

                         line++;

                         //break;
                     }

                     if (printStatus == false)
                     {
                         if (writeFile == true)
                             SaveTextFile();
                     }

                     sbTotalDetail = new StringBuilder();
                     totalLine = 0;
                     counterData = 0;
                     line = 0;
                     if (clearGroupHeader == true)
                         sbGroupHeader = new StringBuilder();
                     return true;
                     #endregion
                 }
                 else
                 {
                     StringBuilder sbLineBreak = new StringBuilder();
                     sbLineBreak.AppendLine();

                     if (line <= 32 && lastData == true)
                     {
                         if (printStatus == true)
                             PrintDirect.WritePrinter(lhPrinter, sbData.ToString(), sbData.Length, ref pcWritten);
                         else
                             sbDataTxt.Append(sbData.ToString());

                         //line++;
                     }

                     for (int i = 0; i < 30; i++)
                     {
                         if (line == 32)
                         {
                             if (printStatus == true)
                             {
                                 PrintDirect.WritePrinter(lhPrinter, sbTotalDetail.ToString(), sbTotalDetail.Length, ref pcWritten);
                                 PrintDirect.WritePrinter(lhPrinter, sbCreateBy.ToString(), sbCreateBy.Length, ref pcWritten);
                                 PrintDirect.WritePrinter(lhPrinter, sbLineBreak.ToString(), sbLineBreak.Length, ref pcWritten);
                                 PrintDirect.WritePrinter(lhPrinter, sbLineBreak.ToString(), sbLineBreak.Length, ref pcWritten);
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
                         else if (line > 32)
                         {
                             if (lastData == true)
                                 CheckLastLineforTotal();

                             if (spaceHeader == true)
                             {
                                 GenerateHeader(rptId, widthPaper, ttlHal, true);
                                 line = 6 + lineGroupHeader + totalLine;
                             }
                             else
                             {
                                 GenerateHeader(rptId, widthPaper, ttlHal, false);
                                 line = 5 + lineGroupHeader + totalLine;
                             }

                             if (printStatus == true)
                             {
                                 PrintDirect.WritePrinter(lhPrinter, sbHeaderReport.ToString(), sbHeaderReport.Length, ref pcWritten);
                                 PrintDirect.WritePrinter(lhPrinter, sbGroupHeader.ToString(), sbGroupHeader.Length, ref pcWritten);
                                 if (lastData == true)
                                 {
                                     PrintDirect.WritePrinter(lhPrinter, sbData.ToString(), sbData.Length, ref pcWritten);
                                     line++;
                                 }
                             }
                             else
                             {
                                 SetDataReportPageBreak();
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
                             //if (printStatus == true)
                             //    PrintDirect.WritePrinter(lhPrinter, sbLineBreak.ToString(), sbLineBreak.Length, ref pcWritten);
                             //else
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
     [StructLayout(LayoutKind.Sequential)]
     public struct DOCINFO
     {
         [MarshalAs(UnmanagedType.LPWStr)]
         public string pDocName;
         [MarshalAs(UnmanagedType.LPWStr)]
         public string pOutputFile;
         [MarshalAs(UnmanagedType.LPWStr)]
         public string pDataType;
     }
     public class PrintDirect
     {
         [DllImport("winspool.drv", CharSet = CharSet.Unicode, ExactSpelling = false,
        CallingConvention = CallingConvention.StdCall, SetLastError = true)]
         public static extern long OpenPrinter(string pPrinterName, ref IntPtr phPrinter, int pDefault);

         [DllImport("winspool.drv", CharSet = CharSet.Unicode, ExactSpelling = false,
        CallingConvention = CallingConvention.StdCall)]
         public static extern long StartDocPrinter(IntPtr hPrinter, int Level, ref DOCINFO pDocInfo);

         [DllImport("winspool.drv", CharSet = CharSet.Unicode, ExactSpelling = true,
        CallingConvention = CallingConvention.StdCall)]
         public static extern long StartPagePrinter(IntPtr hPrinter);

         [DllImport("winspool.drv", CharSet = CharSet.Ansi, ExactSpelling = true,
        CallingConvention = CallingConvention.StdCall)]
         public static extern long WritePrinter(IntPtr hPrinter, string data, int buf, ref int pcWritten);

         [DllImport("winspool.drv", CharSet = CharSet.Unicode, ExactSpelling = true,
        CallingConvention = CallingConvention.StdCall)]
         public static extern long EndPagePrinter(IntPtr hPrinter);

         [DllImport("winspool.drv", CharSet = CharSet.Unicode, ExactSpelling = true,
        CallingConvention = CallingConvention.StdCall)]
         public static extern long EndDocPrinter(IntPtr hPrinter);

         [DllImport("winspool.drv", CharSet = CharSet.Unicode, ExactSpelling = true,
        CallingConvention = CallingConvention.StdCall)]
         public static extern long ClosePrinter(IntPtr hPrinter);
     }

}