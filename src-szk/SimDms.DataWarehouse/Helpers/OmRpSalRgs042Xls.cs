using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace SimDms.DataWarehouse.Helpers
{
    public class OmRpSalRgs042Xls
    {
        private string area, dealer, outlet, salesType, marketModel,dealercode,outletcode;
        private decimal jan, feb, mar, apr, may, jun, jul, aug, sep, oct, nov, dec, total, avg;
        private decimal janGT, febGT, marGT, aprGT, mayGT, junGT, julGT, augGT, sepGT, octGT, novGT, decGT, totalGT, avgGT;

        private decimal janWs, febWs, marWs, aprWs, mayWs, junWs, julWs, augWs, sepWs, octWs, novWs, decWs, totalWs;
        private decimal janRtl, febRtl, marRtl, aprRtl, mayRtl, junRtl, julRtl, augRtl, sepRtl, octRtl, novRtl, decRtl, totalRtl;
        private decimal janPct, febPct, marPct, aprPct, mayPct, junPct, julPct, augPct, sepPct, octPct, novPct, decPct, totalPct;

        private decimal janWsTtl, febWsTtl, marWsTtl, aprWsTtl, mayWsTtl, junWsTtl, julWsTtl, augWsTtl, sepWsTtl, octWsTtl, novWsTtl, decWsTtl, totalWsTtl;
        private decimal janRtlTtl, febRtlTtl, marRtlTtl, aprRtlTtl, mayRtlTtl, junRtlTtl, julRtlTtl, augRtlTtl, sepRtlTtl, octRtlTtl, novRtlTtl, decRtlTtl, totalRtlTtl;
        private decimal janPctTtl, febPctTtl, marPctTtl, aprPctTtl, mayPctTtl, junPctTtl, julPctTtl, augPctTtl, sepPctTtl, octPctTtl, novPctTtl, decPctTtl, totalPctTtl;

        private decimal janWsArea, febWsArea, marWsArea, aprWsArea, mayWsArea, junWsArea, julWsArea, augWsArea, sepWsArea, octWsArea, novWsArea, decWsArea, totalWsArea;
        private decimal janRtlArea, febRtlArea, marRtlArea, aprRtlArea, mayRtlArea, junRtlArea, julRtlArea, augRtlArea, sepRtlArea, octRtlArea, novRtlArea, decRtlArea, totalRtlArea;
        private decimal janPctArea, febPctArea, marPctArea, aprPctArea, mayPctArea, junPctArea, julPctArea, augPctArea, sepPctArea, octPctArea, novPctArea, decPctArea, totalPctArea;

        private decimal janWsAreaJabodetabek, febWsAreaJabodetabek, marWsAreaJabodetabek, aprWsAreaJabodetabek, mayWsAreaJabodetabek, junWsAreaJabodetabek, julWsAreaJabodetabek, augWsAreaJabodetabek, sepWsAreaJabodetabek, octWsAreaJabodetabek, novWsAreaJabodetabek, decWsAreaJabodetabek, totalWsAreaJabodetabek;
        private decimal janRtlAreaJabodetabek, febRtlAreaJabodetabek, marRtlAreaJabodetabek, aprRtlAreaJabodetabek, mayRtlAreaJabodetabek, junRtlAreaJabodetabek, julRtlAreaJabodetabek, augRtlAreaJabodetabek, sepRtlAreaJabodetabek, octRtlAreaJabodetabek, novRtlAreaJabodetabek, decRtlAreaJabodetabek, totalRtlAreaJabodetabek;
        private decimal janPctAreaJabodetabek, febPctAreaJabodetabek, marPctAreaJabodetabek, aprPctAreaJabodetabek, mayPctAreaJabodetabek, junPctAreaJabodetabek, julPctAreaJabodetabek, augPctAreaJabodetabek, sepPctAreaJabodetabek, octPctAreaJabodetabek, novPctAreaJabodetabek, decPctAreaJabodetabek, totalPctAreaJabodetabek;

        private decimal janWsGT, febWsGT, marWsGT, aprWsGT, mayWsGT, junWsGT, julWsGT, augWsGT, sepWsGT, octWsGT, novWsGT, decWsGT, totalWsGT;
        private decimal janRtlGT, febRtlGT, marRtlGT, aprRtlGT, mayRtlGT, junRtlGT, julRtlGT, augRtlGT, sepRtlGT, octRtlGT, novRtlGT, decRtlGT, totalRtlGT;
        private decimal janPctGT, febPctGT, marPctGT, aprPctGT, mayPctGT, junPctGT, julPctGT, augPctGT, sepPctGT, octPctGT, novPctGT, decPctGT, totalPctGT;

        private bool ws = false, rtl = false;
        private int rowIndex = 0, totalDetailWidth = 0, dealerWidth = 0, outletWidth = 0, totalOutlet = 0;

        private DataSet ds = new DataSet();
        Dictionary<int, string> parameter = new Dictionary<int, string>();
        ExcelFileWriter excelReport = new ExcelFileWriter(0);
        string fileName;

        public OmRpSalRgs042Xls(DataSet dataSet, Dictionary<int, string> param, string flName)
        {
            parameter = param;
            ds = dataSet;
            fileName = flName;
        }

        void printdata(DataRow row)
        {

            #region Total
            janWs = decimal.Parse(row["A1"].ToString());
            febWs = decimal.Parse(row["A2"].ToString());
            marWs = decimal.Parse(row["A3"].ToString());
            aprWs = decimal.Parse(row["A4"].ToString());
            mayWs = decimal.Parse(row["A5"].ToString());
            junWs = decimal.Parse(row["A6"].ToString());
            julWs = decimal.Parse(row["A7"].ToString());
            augWs = decimal.Parse(row["A8"].ToString());
            sepWs = decimal.Parse(row["A9"].ToString());
            octWs = decimal.Parse(row["A10"].ToString());
            novWs = decimal.Parse(row["A11"].ToString());
            decWs = decimal.Parse(row["A12"].ToString());
            totalWs = janWs + febWs + marWs + aprWs + mayWs + junWs + julWs + augWs + sepWs + octWs + novWs + decWs;

            janWsTtl += decimal.Parse(row["A1"].ToString());
            febWsTtl += decimal.Parse(row["A2"].ToString());
            marWsTtl += decimal.Parse(row["A3"].ToString());
            aprWsTtl += decimal.Parse(row["A4"].ToString());
            mayWsTtl += decimal.Parse(row["A5"].ToString());
            junWsTtl += decimal.Parse(row["A6"].ToString());
            julWsTtl += decimal.Parse(row["A7"].ToString());
            augWsTtl += decimal.Parse(row["A8"].ToString());
            sepWsTtl += decimal.Parse(row["A9"].ToString());
            octWsTtl += decimal.Parse(row["A10"].ToString());
            novWsTtl += decimal.Parse(row["A11"].ToString());
            decWsTtl += decimal.Parse(row["A12"].ToString());


            janWsArea += decimal.Parse(row["A1"].ToString());
            febWsArea += decimal.Parse(row["A2"].ToString());
            marWsArea += decimal.Parse(row["A3"].ToString());
            aprWsArea += decimal.Parse(row["A4"].ToString());
            mayWsArea += decimal.Parse(row["A5"].ToString());
            junWsArea += decimal.Parse(row["A6"].ToString());
            julWsArea += decimal.Parse(row["A7"].ToString());
            augWsArea += decimal.Parse(row["A8"].ToString());
            sepWsArea += decimal.Parse(row["A9"].ToString());
            octWsArea += decimal.Parse(row["A10"].ToString());
            novWsArea += decimal.Parse(row["A11"].ToString());
            decWsArea += decimal.Parse(row["A12"].ToString());
            totalWsArea += janWsArea + febWsArea + marWsArea + aprWsArea + mayWsArea + junWsArea + julWsArea + augWsArea + sepWsArea + octWsArea + novWsArea + decWsArea;

            janWsAreaJabodetabek += decimal.Parse(row["A1"].ToString());
            febWsAreaJabodetabek += decimal.Parse(row["A2"].ToString());
            marWsAreaJabodetabek += decimal.Parse(row["A3"].ToString());
            aprWsAreaJabodetabek += decimal.Parse(row["A4"].ToString());
            mayWsAreaJabodetabek += decimal.Parse(row["A5"].ToString());
            junWsAreaJabodetabek += decimal.Parse(row["A6"].ToString());
            julWsAreaJabodetabek += decimal.Parse(row["A7"].ToString());
            augWsAreaJabodetabek += decimal.Parse(row["A8"].ToString());
            sepWsAreaJabodetabek += decimal.Parse(row["A9"].ToString());
            octWsAreaJabodetabek += decimal.Parse(row["A10"].ToString());
            novWsAreaJabodetabek += decimal.Parse(row["A11"].ToString());
            decWsAreaJabodetabek += decimal.Parse(row["A12"].ToString());
            totalWsAreaJabodetabek += janWsAreaJabodetabek + febWsAreaJabodetabek + marWsAreaJabodetabek + aprWsAreaJabodetabek + mayWsAreaJabodetabek + junWsAreaJabodetabek + julWsAreaJabodetabek + augWsAreaJabodetabek + sepWsAreaJabodetabek + octWsAreaJabodetabek + novWsAreaJabodetabek + decWsAreaJabodetabek;

            janWsGT += decimal.Parse(row["A1"].ToString());
            febWsGT += decimal.Parse(row["A2"].ToString());
            marWsGT += decimal.Parse(row["A3"].ToString());
            aprWsGT += decimal.Parse(row["A4"].ToString());
            mayWsGT += decimal.Parse(row["A5"].ToString());
            junWsGT += decimal.Parse(row["A6"].ToString());
            julWsGT += decimal.Parse(row["A7"].ToString());
            augWsGT += decimal.Parse(row["A8"].ToString());
            sepWsGT += decimal.Parse(row["A9"].ToString());
            octWsGT += decimal.Parse(row["A10"].ToString());
            novWsGT += decimal.Parse(row["A11"].ToString());
            decWsGT += decimal.Parse(row["A12"].ToString());
            totalWsGT = janWsGT + febWsGT + marWsGT + aprWsGT + mayWsGT + junWsGT + julWsGT + augWsGT + sepWsGT + octWsGT + novWsGT + decWsGT;






            janRtl = decimal.Parse(row["T1"].ToString());
            febRtl = decimal.Parse(row["T2"].ToString());
            marRtl = decimal.Parse(row["T3"].ToString());
            aprRtl = decimal.Parse(row["T4"].ToString());
            mayRtl = decimal.Parse(row["T5"].ToString());
            junRtl = decimal.Parse(row["T6"].ToString());
            julRtl = decimal.Parse(row["T7"].ToString());
            augRtl = decimal.Parse(row["T8"].ToString());
            sepRtl = decimal.Parse(row["T9"].ToString());
            octRtl = decimal.Parse(row["T10"].ToString());
            novRtl = decimal.Parse(row["T11"].ToString());
            decRtl = decimal.Parse(row["T12"].ToString());
            totalRtl = janRtl + febRtl + marRtl + aprRtl + mayRtl + junRtl + julRtl + augRtl + sepRtl + octRtl + novRtl + decRtl;

            janRtlTtl += decimal.Parse(row["T1"].ToString());
            febRtlTtl += decimal.Parse(row["T2"].ToString());
            marRtlTtl += decimal.Parse(row["T3"].ToString());
            aprRtlTtl += decimal.Parse(row["T4"].ToString());
            mayRtlTtl += decimal.Parse(row["T5"].ToString());
            junRtlTtl += decimal.Parse(row["T6"].ToString());
            julRtlTtl += decimal.Parse(row["T7"].ToString());
            augRtlTtl += decimal.Parse(row["T8"].ToString());
            sepRtlTtl += decimal.Parse(row["T9"].ToString());
            octRtlTtl += decimal.Parse(row["T10"].ToString());
            novRtlTtl += decimal.Parse(row["T11"].ToString());
            decRtlTtl += decimal.Parse(row["T12"].ToString());


            janRtlArea += decimal.Parse(row["T1"].ToString());
            febRtlArea += decimal.Parse(row["T2"].ToString());
            marRtlArea += decimal.Parse(row["T3"].ToString());
            aprRtlArea += decimal.Parse(row["T4"].ToString());
            mayRtlArea += decimal.Parse(row["T5"].ToString());
            junRtlArea += decimal.Parse(row["T6"].ToString());
            julRtlArea += decimal.Parse(row["T7"].ToString());
            augRtlArea += decimal.Parse(row["T8"].ToString());
            sepRtlArea += decimal.Parse(row["T9"].ToString());
            octRtlArea += decimal.Parse(row["T10"].ToString());
            novRtlArea += decimal.Parse(row["T11"].ToString());
            decRtlArea += decimal.Parse(row["T12"].ToString());
            totalRtlArea += janRtlArea + febRtlArea + marRtlArea + aprRtlArea + mayRtlArea + junRtlArea + julRtlArea + augRtlArea + sepRtlArea + octRtlArea + novRtlArea + decRtlArea;

            janRtlAreaJabodetabek += decimal.Parse(row["T1"].ToString());
            febRtlAreaJabodetabek += decimal.Parse(row["T2"].ToString());
            marRtlAreaJabodetabek += decimal.Parse(row["T3"].ToString());
            aprRtlAreaJabodetabek += decimal.Parse(row["T4"].ToString());
            mayRtlAreaJabodetabek += decimal.Parse(row["T5"].ToString());
            junRtlAreaJabodetabek += decimal.Parse(row["T6"].ToString());
            julRtlAreaJabodetabek += decimal.Parse(row["T7"].ToString());
            augRtlAreaJabodetabek += decimal.Parse(row["T8"].ToString());
            sepRtlAreaJabodetabek += decimal.Parse(row["T9"].ToString());
            octRtlAreaJabodetabek += decimal.Parse(row["T10"].ToString());
            novRtlAreaJabodetabek += decimal.Parse(row["T11"].ToString());
            decRtlAreaJabodetabek += decimal.Parse(row["T12"].ToString());
            totalRtlAreaJabodetabek += janRtlAreaJabodetabek + febRtlAreaJabodetabek + marRtlAreaJabodetabek + aprRtlAreaJabodetabek + mayRtlAreaJabodetabek + junRtlAreaJabodetabek + julRtlAreaJabodetabek + augRtlAreaJabodetabek + sepRtlAreaJabodetabek + octRtlAreaJabodetabek + novRtlAreaJabodetabek + decRtlAreaJabodetabek;

            janRtlGT += decimal.Parse(row["T1"].ToString());
            febRtlGT += decimal.Parse(row["T2"].ToString());
            marRtlGT += decimal.Parse(row["T3"].ToString());
            aprRtlGT += decimal.Parse(row["T4"].ToString());
            mayRtlGT += decimal.Parse(row["T5"].ToString());
            junRtlGT += decimal.Parse(row["T6"].ToString());
            julRtlGT += decimal.Parse(row["T7"].ToString());
            augRtlGT += decimal.Parse(row["T8"].ToString());
            sepRtlGT += decimal.Parse(row["T9"].ToString());
            octRtlGT += decimal.Parse(row["T10"].ToString());
            novRtlGT += decimal.Parse(row["T11"].ToString());
            decRtlGT += decimal.Parse(row["T12"].ToString());
            totalRtlGT = janRtlGT + febRtlGT + marRtlGT + aprRtlGT + mayRtlGT + junRtlGT + julRtlGT + augRtlGT + sepRtlGT + octRtlGT + novRtlGT + decRtlGT;

            jan += decimal.Parse(row["T1"].ToString());
            feb += decimal.Parse(row["T2"].ToString());
            mar += decimal.Parse(row["T3"].ToString());
            apr += decimal.Parse(row["T4"].ToString());
            may += decimal.Parse(row["T5"].ToString());
            jun += decimal.Parse(row["T6"].ToString());
            jul += decimal.Parse(row["T7"].ToString());
            aug += decimal.Parse(row["T8"].ToString());
            sep += decimal.Parse(row["T9"].ToString());
            oct += decimal.Parse(row["T10"].ToString());
            nov += decimal.Parse(row["T11"].ToString());
            dec += decimal.Parse(row["T12"].ToString());
            total = jan + feb + mar + apr + may + jun + jul + aug + sep + oct + nov + dec;

            janPct = janRtl != 0 ? (janWs / janRtl) * 100 : 0;
            febPct = febRtl != 0 ? (febWs / febRtl) * 100 : 0;
            marPct = marRtl != 0 ? (marWs / marRtl) * 100 : 0;
            aprPct = aprRtl != 0 ? (aprWs / aprRtl) * 100 : 0;
            mayPct = mayRtl != 0 ? (mayWs / mayRtl) * 100 : 0;
            junPct = junRtl != 0 ? (junWs / junRtl) * 100 : 0;
            julPct = julRtl != 0 ? (julWs / julRtl) * 100 : 0;
            augPct = augRtl != 0 ? (augWs / augRtl) * 100 : 0;
            sepPct = sepRtl != 0 ? (sepWs / sepRtl) * 100 : 0;
            octPct = octRtl != 0 ? (octWs / octRtl) * 100 : 0;
            novPct = novRtl != 0 ? (novWs / novRtl) * 100 : 0;
            decPct = decRtl != 0 ? (decWs / decRtl) * 100 : 0;
            totalPct = totalRtl != 0 ? (totalWs / totalRtl) * 100 : 0;

            #endregion

            excelReport.SetCellValue(row["A1"].ToString(), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, ExcelCellStyle.RightBorderedStandardNumber, true);
            excelReport.SetCellValue(row["T1"].ToString(), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, ExcelCellStyle.RightBorderedStandardNumber, true);
            excelReport.SetCellValue(janPct.ToString("n2") + "%", 8 + rowIndex, 2 + totalDetailWidth, 1, 1, ExcelCellStyle.RightBorderedStandardNumber);

            excelReport.SetCellValue(row["A2"].ToString(), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, ExcelCellStyle.RightBorderedStandardNumber, true);
            excelReport.SetCellValue(row["T2"].ToString(), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, ExcelCellStyle.RightBorderedStandardNumber, true);
            excelReport.SetCellValue(febPct.ToString("n2") + "%", 8 + rowIndex, 5 + totalDetailWidth, 1, 1, ExcelCellStyle.RightBorderedStandardNumber);

            excelReport.SetCellValue(row["A3"].ToString(), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, ExcelCellStyle.RightBorderedStandardNumber, true);
            excelReport.SetCellValue(row["T3"].ToString(), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, ExcelCellStyle.RightBorderedStandardNumber, true);
            excelReport.SetCellValue(marPct.ToString("n2") + "%", 8 + rowIndex, 8 + totalDetailWidth, 1, 1, ExcelCellStyle.RightBorderedStandardNumber);

            excelReport.SetCellValue(row["A4"].ToString(), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, ExcelCellStyle.RightBorderedStandardNumber, true);
            excelReport.SetCellValue(row["T4"].ToString(), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, ExcelCellStyle.RightBorderedStandardNumber, true);
            excelReport.SetCellValue(aprPct.ToString("n2") + "%", 8 + rowIndex, 11 + totalDetailWidth, 1, 1, ExcelCellStyle.RightBorderedStandardNumber);

            excelReport.SetCellValue(row["A5"].ToString(), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.RightBorderedStandardNumber, true);
            excelReport.SetCellValue(row["T5"].ToString(), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.RightBorderedStandardNumber, true);
            excelReport.SetCellValue(mayPct.ToString("n2") + "%", 8 + rowIndex, 14 + totalDetailWidth, 1, 1, ExcelCellStyle.RightBorderedStandardNumber);

            excelReport.SetCellValue(row["A6"].ToString(), 8 + rowIndex, 15 + totalDetailWidth, 1, 1, ExcelCellStyle.RightBorderedStandardNumber, true);
            excelReport.SetCellValue(row["T6"].ToString(), 8 + rowIndex, 16 + totalDetailWidth, 1, 1, ExcelCellStyle.RightBorderedStandardNumber, true);
            excelReport.SetCellValue(junPct.ToString("n2") + "%", 8 + rowIndex, 17 + totalDetailWidth, 1, 1, ExcelCellStyle.RightBorderedStandardNumber);

            excelReport.SetCellValue(row["A7"].ToString(), 8 + rowIndex, 18 + totalDetailWidth, 1, 1, ExcelCellStyle.RightBorderedStandardNumber, true);
            excelReport.SetCellValue(row["T7"].ToString(), 8 + rowIndex, 19 + totalDetailWidth, 1, 1, ExcelCellStyle.RightBorderedStandardNumber, true);
            excelReport.SetCellValue(julPct.ToString("n2") + "%", 8 + rowIndex, 20 + totalDetailWidth, 1, 1, ExcelCellStyle.RightBorderedStandardNumber);

            excelReport.SetCellValue(row["A8"].ToString(), 8 + rowIndex, 21 + totalDetailWidth, 1, 1, ExcelCellStyle.RightBorderedStandardNumber, true);
            excelReport.SetCellValue(row["T8"].ToString(), 8 + rowIndex, 22 + totalDetailWidth, 1, 1, ExcelCellStyle.RightBorderedStandardNumber, true);
            excelReport.SetCellValue(augPct.ToString("n2") + "%", 8 + rowIndex, 23 + totalDetailWidth, 1, 1, ExcelCellStyle.RightBorderedStandardNumber);

            excelReport.SetCellValue(row["A9"].ToString(), 8 + rowIndex, 24 + totalDetailWidth, 1, 1, ExcelCellStyle.RightBorderedStandardNumber, true);
            excelReport.SetCellValue(row["T9"].ToString(), 8 + rowIndex, 25 + totalDetailWidth, 1, 1, ExcelCellStyle.RightBorderedStandardNumber, true);
            excelReport.SetCellValue(sepPct.ToString("n2") + "%", 8 + rowIndex, 26 + totalDetailWidth, 1, 1, ExcelCellStyle.RightBorderedStandardNumber);

            excelReport.SetCellValue(row["A10"].ToString(), 8 + rowIndex, 27 + totalDetailWidth, 1, 1, ExcelCellStyle.RightBorderedStandardNumber, true);
            excelReport.SetCellValue(row["T10"].ToString(), 8 + rowIndex, 28 + totalDetailWidth, 1, 1, ExcelCellStyle.RightBorderedStandardNumber, true);
            excelReport.SetCellValue(octPct.ToString("n2") + "%", 8 + rowIndex, 29 + totalDetailWidth, 1, 1, ExcelCellStyle.RightBorderedStandardNumber);

            excelReport.SetCellValue(row["A11"].ToString(), 8 + rowIndex, 30 + totalDetailWidth, 1, 1, ExcelCellStyle.RightBorderedStandardNumber, true);
            excelReport.SetCellValue(row["T11"].ToString(), 8 + rowIndex, 31 + totalDetailWidth, 1, 1, ExcelCellStyle.RightBorderedStandardNumber, true);
            excelReport.SetCellValue(novPct.ToString("n2") + "%", 8 + rowIndex, 32 + totalDetailWidth, 1, 1, ExcelCellStyle.RightBorderedStandardNumber);

            excelReport.SetCellValue(row["A12"].ToString(), 8 + rowIndex, 33 + totalDetailWidth, 1, 1, ExcelCellStyle.RightBorderedStandardNumber, true);
            excelReport.SetCellValue(row["T12"].ToString(), 8 + rowIndex, 34 + totalDetailWidth, 1, 1, ExcelCellStyle.RightBorderedStandardNumber, true);
            excelReport.SetCellValue(decPct.ToString("n2") + "%", 8 + rowIndex, 35 + totalDetailWidth, 1, 1, ExcelCellStyle.RightBorderedStandardNumber);

            excelReport.SetCellValue(totalWs.ToString(), 8 + rowIndex, 36 + totalDetailWidth, 1, 1, ExcelCellStyle.RightBorderedStandardNumber, true);
            excelReport.SetCellValue(totalRtl.ToString(), 8 + rowIndex, 37 + totalDetailWidth, 1, 1, ExcelCellStyle.RightBorderedStandardNumber, true);
            excelReport.SetCellValue(totalPct.ToString("n2") + "%", 8 + rowIndex, 38 + totalDetailWidth, 1, 1, ExcelCellStyle.RightBorderedStandardNumber);
        }

        void totalarea()
        {

            janPctArea = janRtlArea != 0 ? (janWsArea / janRtlArea) * 100 : 0;
            febPctArea = febRtlArea != 0 ? (febWsArea / febRtlArea) * 100 : 0;
            marPctArea = marRtlArea != 0 ? (marWsArea / marRtlArea) * 100 : 0;
            aprPctArea = aprRtlArea != 0 ? (aprWsArea / aprRtlArea) * 100 : 0;
            mayPctArea = mayRtlArea != 0 ? (mayWsArea / mayRtlArea) * 100 : 0;
            junPctArea = junRtlArea != 0 ? (junWsArea / junRtlArea) * 100 : 0;
            julPctArea = julRtlArea != 0 ? (julWsArea / julRtlArea) * 100 : 0;
            augPctArea = augRtlArea != 0 ? (augWsArea / augRtlArea) * 100 : 0;
            sepPctArea = sepRtlArea != 0 ? (sepWsArea / sepRtlArea) * 100 : 0;
            octPctArea = octRtlArea != 0 ? (octWsArea / octRtlArea) * 100 : 0;
            novPctArea = novRtlArea != 0 ? (novWsArea / novRtlArea) * 100 : 0;
            decPctArea = decRtlArea != 0 ? (decWsArea / decRtlArea) * 100 : 0;
            totalPctArea = totalRtlArea != 0 ? (totalWsArea / totalRtlArea) * 100 : 0;

            excelReport.SetCellValue("Total " + area, 8 + rowIndex, 0, 1, dealerWidth + outletWidth, ExcelCellStyle.BlueTotal); //, TextAlignment.MiddleLeft, Orientation.HORIZONTAL, Color.White, 9, FontStyle.Regular, true);
            excelReport.SetCellValue(janWsArea.ToString(), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
            excelReport.SetCellValue(janRtlArea.ToString(), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
            excelReport.SetCellValue(janPctArea.ToString("n2") + "%", 8 + rowIndex, 2 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber);

            excelReport.SetCellValue(febWsArea.ToString(), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
            excelReport.SetCellValue(febRtlArea.ToString(), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
            excelReport.SetCellValue(febPctArea.ToString("n2") + "%", 8 + rowIndex, 5 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber);

            excelReport.SetCellValue(marWsArea.ToString(), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
            excelReport.SetCellValue(marRtlArea.ToString(), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
            excelReport.SetCellValue(marPctArea.ToString("n2") + "%", 8 + rowIndex, 8 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber);

            excelReport.SetCellValue(aprWsArea.ToString(), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
            excelReport.SetCellValue(aprRtlArea.ToString(), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
            excelReport.SetCellValue(aprPctArea.ToString("n2") + "%", 8 + rowIndex, 11 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber);

            excelReport.SetCellValue(mayWsArea.ToString(), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
            excelReport.SetCellValue(mayRtlArea.ToString(), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
            excelReport.SetCellValue(mayPctArea.ToString("n2") + "%", 8 + rowIndex, 14 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber);

            excelReport.SetCellValue(junWsArea.ToString(), 8 + rowIndex, 15 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
            excelReport.SetCellValue(junRtlArea.ToString(), 8 + rowIndex, 16 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
            excelReport.SetCellValue(junPctArea.ToString("n2") + "%", 8 + rowIndex, 17 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber);

            excelReport.SetCellValue(julWsArea.ToString(), 8 + rowIndex, 18 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
            excelReport.SetCellValue(julRtlArea.ToString(), 8 + rowIndex, 19 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
            excelReport.SetCellValue(julPctArea.ToString("n2") + "%", 8 + rowIndex, 20 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber);

            excelReport.SetCellValue(augWsArea.ToString(), 8 + rowIndex, 21 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
            excelReport.SetCellValue(augRtlArea.ToString(), 8 + rowIndex, 22 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
            excelReport.SetCellValue(augPctArea.ToString("n2") + "%", 8 + rowIndex, 23 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber);

            excelReport.SetCellValue(sepWsArea.ToString(), 8 + rowIndex, 24 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
            excelReport.SetCellValue(sepRtlArea.ToString(), 8 + rowIndex, 25 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
            excelReport.SetCellValue(sepPctArea.ToString("n2") + "%", 8 + rowIndex, 26 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber);

            excelReport.SetCellValue(octWsArea.ToString(), 8 + rowIndex, 27 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
            excelReport.SetCellValue(octRtlArea.ToString(), 8 + rowIndex, 28 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
            excelReport.SetCellValue(octPctArea.ToString("n2") + "%", 8 + rowIndex, 29 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber);

            excelReport.SetCellValue(novWsArea.ToString(), 8 + rowIndex, 30 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
            excelReport.SetCellValue(novRtlArea.ToString(), 8 + rowIndex, 31 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
            excelReport.SetCellValue(novPctArea.ToString("n2") + "%", 8 + rowIndex, 32 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber);

            excelReport.SetCellValue(decWsArea.ToString(), 8 + rowIndex, 33 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
            excelReport.SetCellValue(decRtlArea.ToString(), 8 + rowIndex, 34 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
            excelReport.SetCellValue(decPctArea.ToString("n2") + "%", 8 + rowIndex, 35 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber);

            excelReport.SetCellValue(totalWsArea.ToString(), 8 + rowIndex, 36 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
            excelReport.SetCellValue(totalRtlArea.ToString(), 8 + rowIndex, 37 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber, true);
            excelReport.SetCellValue(totalPctArea.ToString("n2") + "%", 8 + rowIndex, 38 + totalDetailWidth, 1, 1, ExcelCellStyle.BlueTotalNumber);

        }

        void printtotaldealer()
        {

            totalRtlTtl += janRtlTtl + febRtlTtl + marRtlTtl + aprRtlTtl + mayRtlTtl + junRtlTtl + julRtlTtl + augRtlTtl + sepRtlTtl + octRtlTtl + novRtlTtl + decRtlTtl;
            totalWsTtl += janWsTtl + febWsTtl + marWsTtl + aprWsTtl + mayWsTtl + junWsTtl + julWsTtl + augWsTtl + sepWsTtl + octWsTtl + novWsTtl + decWsTtl;

            excelReport.SetCellValue("Total", 8 + rowIndex, 0 + dealerWidth, 1, outletWidth, ExcelCellStyle.YellowTotal);

            excelReport.SetCellValue(janWsTtl.ToString(), 8 + rowIndex, 0 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
            excelReport.SetCellValue(janRtlTtl.ToString(), 8 + rowIndex, 1 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
            excelReport.SetCellValue(janPct.ToString("n2") + "%", 8 + rowIndex, 2 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber);
            excelReport.SetCellValue(febWsTtl.ToString(), 8 + rowIndex, 3 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
            excelReport.SetCellValue(febRtlTtl.ToString(), 8 + rowIndex, 4 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
            excelReport.SetCellValue(febPct.ToString("n2") + "%", 8 + rowIndex, 5 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber);
            excelReport.SetCellValue(marWsTtl.ToString(), 8 + rowIndex, 6 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
            excelReport.SetCellValue(marRtlTtl.ToString(), 8 + rowIndex, 7 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
            excelReport.SetCellValue(marPct.ToString("n2") + "%", 8 + rowIndex, 8 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber);
            excelReport.SetCellValue(aprWsTtl.ToString(), 8 + rowIndex, 9 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
            excelReport.SetCellValue(aprRtlTtl.ToString(), 8 + rowIndex, 10 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
            excelReport.SetCellValue(aprPct.ToString("n2") + "%", 8 + rowIndex, 11 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber);
            excelReport.SetCellValue(mayWsTtl.ToString(), 8 + rowIndex, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
            excelReport.SetCellValue(mayRtlTtl.ToString(), 8 + rowIndex, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
            excelReport.SetCellValue(mayPct.ToString("n2") + "%", 8 + rowIndex, 14 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber);
            excelReport.SetCellValue(junWsTtl.ToString(), 8 + rowIndex, 15 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
            excelReport.SetCellValue(junRtlTtl.ToString(), 8 + rowIndex, 16 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
            excelReport.SetCellValue(junPct.ToString("n2") + "%", 8 + rowIndex, 17 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber);
            excelReport.SetCellValue(julWsTtl.ToString(), 8 + rowIndex, 18 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
            excelReport.SetCellValue(julRtlTtl.ToString(), 8 + rowIndex, 19 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
            excelReport.SetCellValue(julPct.ToString("n2") + "%", 8 + rowIndex, 20 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber);
            excelReport.SetCellValue(augWsTtl.ToString(), 8 + rowIndex, 21 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
            excelReport.SetCellValue(augRtlTtl.ToString(), 8 + rowIndex, 22 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
            excelReport.SetCellValue(augPct.ToString("n2") + "%", 8 + rowIndex, 23 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber);
            excelReport.SetCellValue(sepWsTtl.ToString(), 8 + rowIndex, 24 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
            excelReport.SetCellValue(sepRtlTtl.ToString(), 8 + rowIndex, 25 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
            excelReport.SetCellValue(sepPct.ToString("n2") + "%", 8 + rowIndex, 26 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber);
            excelReport.SetCellValue(octWsTtl.ToString(), 8 + rowIndex, 27 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
            excelReport.SetCellValue(octRtlTtl.ToString(), 8 + rowIndex, 28 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
            excelReport.SetCellValue(octPct.ToString("n2") + "%", 8 + rowIndex, 29 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber);
            excelReport.SetCellValue(novWsTtl.ToString(), 8 + rowIndex, 30 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
            excelReport.SetCellValue(novRtlTtl.ToString(), 8 + rowIndex, 31 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
            excelReport.SetCellValue(novPct.ToString("n2") + "%", 8 + rowIndex, 32 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber);
            excelReport.SetCellValue(decWsTtl.ToString(), 8 + rowIndex, 33 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
            excelReport.SetCellValue(decRtlTtl.ToString(), 8 + rowIndex, 34 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
            excelReport.SetCellValue(decPct.ToString("n2") + "%", 8 + rowIndex, 35 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber);

            excelReport.SetCellValue(totalWsTtl.ToString(), 8 + rowIndex, 36 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
            excelReport.SetCellValue(totalRtlTtl.ToString(), 8 + rowIndex, 37 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber, true);
            excelReport.SetCellValue(totalPct.ToString("n2") + "%", 8 + rowIndex, 38 + totalDetailWidth, 1, 1, ExcelCellStyle.YellowTotalNumber);

            janWsTtl = febWsTtl = marWsTtl = aprWsTtl = mayWsTtl = junWsTtl = julWsTtl = augWsTtl = sepWsTtl = octWsTtl = novWsTtl = decWsTtl = totalWsTtl = 0;
            janRtlTtl = febRtlTtl = marRtlTtl = aprRtlTtl = mayRtlTtl = junRtlTtl = julRtlTtl = augRtlTtl = sepRtlTtl = octRtlTtl = novRtlTtl = decRtlTtl = totalRtlTtl = 0;

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

            excelReport.SetCellValue("ACTUAL vs TARGET COMPARATION REPORT ", 0, 0, 1, dealerWidth + indentTitleDesc + indentTitleDesc + 2, ExcelCellStyle.Header);
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


            excelReport.SetCellValue("Jan", 6, 0 + totalDetailWidth, 1, 3, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Feb", 6, 3 + totalDetailWidth, 1, 3, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Mar", 6, 6 + totalDetailWidth, 1, 3, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Apr", 6, 9 + totalDetailWidth, 1, 3, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("May", 6, 12 + totalDetailWidth, 1, 3, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Jun", 6, 15 + totalDetailWidth, 1, 3, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Jul", 6, 18 + totalDetailWidth, 1, 3, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Aug", 6, 21 + totalDetailWidth, 1, 3, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Sep", 6, 24 + totalDetailWidth, 1, 3, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Oct", 6, 27 + totalDetailWidth, 1, 3, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Nov", 6, 30 + totalDetailWidth, 1, 3, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Dec", 6, 33 + totalDetailWidth, 1, 3, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Total", 6, 36 + totalDetailWidth, 1, 3, ExcelCellStyle.CenterBorderedBold);

            excelReport.SetCellValue("Actual", 7, 0 + totalDetailWidth, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Target", 7, 1 + totalDetailWidth, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("%", 7, 2 + totalDetailWidth, 1, 1, ExcelCellStyle.CenterBorderedBold);

            excelReport.SetCellValue("Actual", 7, 3 + totalDetailWidth, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Target", 7, 4 + totalDetailWidth, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("%", 7, 5 + totalDetailWidth, 1, 1, ExcelCellStyle.CenterBorderedBold);

            excelReport.SetCellValue("Actual", 7, 6 + totalDetailWidth, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Target", 7, 7 + totalDetailWidth, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("%", 7, 8 + totalDetailWidth, 1, 1, ExcelCellStyle.CenterBorderedBold);

            excelReport.SetCellValue("Actual", 7, 9 + totalDetailWidth, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Target", 7, 10 + totalDetailWidth, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("%", 7, 11 + totalDetailWidth, 1, 1, ExcelCellStyle.CenterBorderedBold);

            excelReport.SetCellValue("Actual", 7, 12 + totalDetailWidth, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Target", 7, 13 + totalDetailWidth, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("%", 7, 14 + totalDetailWidth, 1, 1, ExcelCellStyle.CenterBorderedBold);

            excelReport.SetCellValue("Actual", 7, 15 + totalDetailWidth, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Target", 7, 16 + totalDetailWidth, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("%", 7, 17 + totalDetailWidth, 1, 1, ExcelCellStyle.CenterBorderedBold);

            excelReport.SetCellValue("Actual", 7, 18 + totalDetailWidth, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Target", 7, 19 + totalDetailWidth, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("%", 7, 20 + totalDetailWidth, 1, 1, ExcelCellStyle.CenterBorderedBold);

            excelReport.SetCellValue("Actual", 7, 21 + totalDetailWidth, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Target", 7, 22 + totalDetailWidth, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("%", 7, 23 + totalDetailWidth, 1, 1, ExcelCellStyle.CenterBorderedBold);

            excelReport.SetCellValue("Actual", 7, 24 + totalDetailWidth, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Target", 7, 25 + totalDetailWidth, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("%", 7, 26 + totalDetailWidth, 1, 1, ExcelCellStyle.CenterBorderedBold);

            excelReport.SetCellValue("Actual", 7, 27 + totalDetailWidth, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Target", 7, 28 + totalDetailWidth, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("%", 7, 29 + totalDetailWidth, 1, 1, ExcelCellStyle.CenterBorderedBold);

            excelReport.SetCellValue("Actual", 7, 30 + totalDetailWidth, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Target", 7, 31 + totalDetailWidth, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("%", 7, 32 + totalDetailWidth, 1, 1, ExcelCellStyle.CenterBorderedBold);

            excelReport.SetCellValue("Actual", 7, 33 + totalDetailWidth, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Target", 7, 34 + totalDetailWidth, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("%", 7, 35 + totalDetailWidth, 1, 1, ExcelCellStyle.CenterBorderedBold);

            excelReport.SetCellValue("Actual", 7, 36 + totalDetailWidth, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Target", 7, 37 + totalDetailWidth, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("%", 7, 38 + totalDetailWidth, 1, 1, ExcelCellStyle.CenterBorderedBold);

            #endregion

            rowIndex = 0;



            area = "";
            dealer = "";
            dealercode = "";
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

                    janWs = febWs = marWs = aprWs = mayWs = junWs = julWs = augWs = sepWs = octWs = novWs = decWs = totalWs = 0;
                    janRtl = febRtl = marRtl = aprRtl = mayRtl = junRtl = julRtl = augRtl = sepRtl = octRtl = novRtl = decRtl = totalRtl = 0;
                    janPct = febPct = marPct = aprPct = mayPct = junPct = julPct = augPct = sepPct = octPct = novPct = decPct = totalPct = 0;

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

                    janWsArea = febWsArea = marWsArea = aprWsArea = mayWsArea = junWsArea = julWsArea = augWsArea = sepWsArea = octWsArea = novWsArea = decWsArea = totalWsArea = 0;
                    janRtlArea = febRtlArea = marRtlArea = aprRtlArea = mayRtlArea = junRtlArea = julRtlArea = augRtlArea = sepRtlArea = octRtlArea = novRtlArea = decRtlArea = totalRtlArea = 0;
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