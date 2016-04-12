using SimDms.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SimDms.Web.Reports.Sparepart
{
    public class txtSpRpTrn033 : IRptProc
    {
        public Models.SysUser CurrentUser { get; set; }
        private SimDms.Sparepart.Models.DataContext ctx = new SimDms.Sparepart.Models.DataContext(MyHelpers.GetConnString("DataContext"));

        private object[] setTextParameter;
        private string msg = "";

        public void SetTextParameter(params object[] textParam)
        {
            setTextParameter = textParam;
        }

        public string CreateReport(string rptId, string sproc, string sparam, string printerloc, bool print, bool fullpage, object[] oparam, string paramReport)
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "exec " + sproc + " " + sparam;
            cmd.CommandType = CommandType.Text;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            SetTextParameter(oparam);
            return CreateReportSpRpTrn033(rptId, dt, sparam, printerloc, print, "", fullpage);
        }

        #region SpRpTrn033
        private string CreateReportSpRpTrn033(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage)
        {
            string[] arrayHeader = new string[7];
            string pickingSlipNo, printSeq, typeOfGoods, hasil;
            SpGenerateTextFileReport gtf = new SpGenerateTextFileReport();
            gtf.CurrentUser = CurrentUser;
            gtf.SetParameterPrinter(paramReport);
            gtf.SetPaperSize(1100, 850);
            gtf.GenerateTextFileReports(recordId, printerLoc, "W96", print, fileLocation, fullPage);
            gtf.GenerateHeader();

            pickingSlipNo = (dt.Rows[0]["PickingSlipNo"].ToString() != null) ? dt.Rows[0]["PickingSlipNo"].ToString() : string.Empty;
            printSeq = (dt.Rows[0]["PrintSeq"].ToString() != null) ? dt.Rows[0]["PrintSeq"].ToString() : string.Empty;
            typeOfGoods = (dt.Rows[0]["TypeOfGoods"].ToString() != null) ? dt.Rows[0]["TypeOfGoods"].ToString() : string.Empty;
            hasil = string.Format("{0}({1})({2})", pickingSlipNo, printSeq, typeOfGoods);

            arrayHeader[0] = hasil;
            arrayHeader[1] = dt.Rows[0]["PickingListType"].ToString();
            arrayHeader[2] = dt.Rows[0]["PickingSlipDate"].ToString();
            arrayHeader[3] = dt.Rows[0]["TransType"].ToString();
            arrayHeader[4] = dt.Rows[0]["PaymentType"].ToString();
            arrayHeader[5] = dt.Rows[0]["ExPickingSlipNo"].ToString();
            arrayHeader[6] = dt.Rows[0]["CustomerName"].ToString();

            CreateHdrSpRpTrn033(gtf, arrayHeader);
            gtf.PrintHeader();

            string psNo = "";
            int noUrut = 0;
            decimal ttlSupply = 0, ttlDemand = 0;
            bool lastData = false;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (psNo == "")
                {
                    psNo = dt.Rows[i]["PickingSlipNo"].ToString();

                    noUrut++;
                    gtf.SetDataDetail(noUrut.ToString(), 3, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["PartNo"].ToString(), 20, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 20, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["LocationCode"].ToString(), 8, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["QtyPicked"].ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["QtyOrder"].ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["OnHand"].ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["Allocation"].ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["MovingCode"].ToString(), 2, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["WarehouseCode"].ToString(), 2, ' ', false, true);

                    gtf.PrintData(fullPage);
                }
                else if (psNo != dt.Rows[i]["PickingSlipNo"].ToString())
                {
                    gtf.SetTotalDetailLine();

                    gtf.SetTotalDetail("Total : ", 54, ' ', true, false);
                    gtf.SetTotalDetail(ttlSupply.ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetTotalDetail(ttlDemand.ToString(), 8, ' ', false, true, true, true, "n2");
                    gtf.SetTotalDetailLine();

                    gtf.SetTotalDetail("NILAI PICKING LIST : ", 21, ' ');
                    gtf.SetTotalDetail(dt.Rows[i - 1]["TotFinalSalesAmt"].ToString(), 45, ' ', true, false, false, true, "n0");
                    gtf.SetTotalDetail("NO. ORDER : " + dt.Rows[i - 1]["OrderNo"].ToString(), 50, ' ', false, true);
                    gtf.SetTotalDetail("KETERANGAN :" + dt.Rows[i - 1]["Remark"].ToString(), 96, ' ', false, true);
                    gtf.SetTotalDetailSpace(65);
                    gtf.SetTotalDetail(dt.Rows[i - 1]["CityName"].ToString() + ", " + DateTime.Now.ToString("dd-MMM-yyyy"), 31, ' ', false, true);

                    gtf.SetTotalDetailSpace(1);
                    gtf.SetTotalDetail("Diterima oleh :", 22, ' ', true);
                    gtf.SetTotalDetailSpace(1);
                    gtf.SetTotalDetail("Diserahkan oleh :", 22, ' ', true);
                    gtf.SetTotalDetailSpace(1);
                    gtf.SetTotalDetail("Diperikasa oleh :", 22, ' ', true);
                    gtf.SetTotalDetailSpace(1);
                    gtf.SetTotalDetail("Dibuat oleh :", 22, ' ', false, true);
                    gtf.SetTotalDetail(" ", 1, ' ', false, true);
                    gtf.SetTotalDetail(" ", 1, ' ', false, true);
                    gtf.SetTotalDetailSpace(1);
                    gtf.SetTotalDetail(dt.Rows[i - 1]["SignName_1"] != null ? dt.Rows[i - 1]["SignName_1"].ToString() : " ", 22, ' ', true);
                    gtf.SetTotalDetailSpace(1);
                    gtf.SetTotalDetail(dt.Rows[i - 1]["Picker"] != null ? dt.Rows[i - 1]["Picker"].ToString() : " ", 22, ' ', true);
                    gtf.SetTotalDetailSpace(1);
                    gtf.SetTotalDetail(dt.Rows[i - 1]["SignName_3"] != null ? dt.Rows[i - 1]["SignName_3"].ToString() : " ", 22, ' ', true);
                    gtf.SetTotalDetailSpace(1);
                    gtf.SetTotalDetail(dt.Rows[i - 1]["SignName_4"] != null ? dt.Rows[i - 1]["SignName_4"].ToString() : " ", 22, ' ', false, true);
                    gtf.SetTotalDetailSpace(1);
                    gtf.SetTotalDetail("-", 22, '-', true);
                    gtf.SetTotalDetailSpace(1);
                    gtf.SetTotalDetail("-", 22, '-', true);
                    gtf.SetTotalDetailSpace(1);
                    gtf.SetTotalDetail("-", 22, '-', true);
                    gtf.SetTotalDetailSpace(1);
                    gtf.SetTotalDetail("-", 22, '-', false, true);
                    gtf.SetTotalDetailSpace(1);
                    gtf.SetTotalDetail(dt.Rows[i - 1]["TitleSign_1"].ToString(), 22, ' ', true);
                    gtf.SetTotalDetailSpace(1);
                    gtf.SetTotalDetail(dt.Rows[i - 1]["TitleSign_2"].ToString(), 22, ' ', true);
                    gtf.SetTotalDetailSpace(1);
                    gtf.SetTotalDetail(dt.Rows[i - 1]["TitleSign_3"].ToString(), 22, ' ', true);
                    gtf.SetTotalDetailSpace(1);
                    gtf.SetTotalDetail(dt.Rows[i - 1]["TitleSign_4"].ToString(), 22, ' ', false, true);
                    gtf.PrintTotal(false, lastData, false);

                    ttlSupply = ttlDemand = 0;

                    pickingSlipNo = (dt.Rows[i]["PickingSlipNo"].ToString() != null) ? dt.Rows[i]["PickingSlipNo"].ToString() : string.Empty;
                    printSeq = (dt.Rows[i]["PrintSeq"].ToString() != null) ? dt.Rows[i]["PrintSeq"].ToString() : string.Empty;
                    typeOfGoods = (dt.Rows[i]["TypeOfGoods"].ToString() != null) ? dt.Rows[i]["TypeOfGoods"].ToString() : string.Empty;
                    hasil = string.Format("{0}({1})({2})", pickingSlipNo, printSeq, typeOfGoods);

                    arrayHeader[0] = hasil;
                    arrayHeader[1] = dt.Rows[i]["PickingListType"].ToString();
                    arrayHeader[2] = dt.Rows[i]["PickingSlipDate"].ToString();
                    arrayHeader[3] = dt.Rows[i]["TransType"].ToString();
                    arrayHeader[4] = dt.Rows[i]["PaymentType"].ToString();
                    arrayHeader[5] = dt.Rows[i]["ExPickingSlipNo"].ToString();
                    arrayHeader[6] = dt.Rows[i]["CustomerName"].ToString();

                    CreateHdrSpRpTrn033(gtf, arrayHeader);
                    gtf.PrintAfterBreak();

                    lastData = false;
                    noUrut = 1;

                    gtf.SetDataDetail(noUrut.ToString(), 3, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["PartNo"].ToString(), 20, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 20, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["LocationCode"].ToString(), 8, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["QtyPicked"].ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["QtyOrder"].ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["OnHand"].ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["Allocation"].ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["MovingCode"].ToString(), 2, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["WarehouseCode"].ToString(), 2, ' ', false, true);
                    gtf.PrintData(fullPage);

                    psNo = dt.Rows[i]["PickingSlipNo"].ToString();
                }
                else if (psNo == dt.Rows[i]["PickingSlipNo"].ToString())
                {
                    noUrut++;
                    gtf.SetDataDetail(noUrut.ToString(), 3, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["PartNo"].ToString(), 20, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 20, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["LocationCode"].ToString(), 8, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["QtyPicked"].ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["QtyOrder"].ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["OnHand"].ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["Allocation"].ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["MovingCode"].ToString(), 2, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["WarehouseCode"].ToString(), 2, ' ', false, true);
                    if (i + 1 < dt.Rows.Count)
                    {
                        if (psNo == dt.Rows[i + 1]["PickingSlipNo"].ToString())
                            gtf.PrintData(fullPage);
                        else
                            lastData = true;
                    }
                    else
                        lastData = true;
                }

                ttlSupply += decimal.Parse(dt.Rows[i]["QtyPicked"].ToString());
                ttlDemand += decimal.Parse(dt.Rows[i]["QtyOrder"].ToString());
            }
            gtf.SetTotalDetailLine();

            gtf.SetTotalDetail("Total : ", 54, ' ', true, false);
            gtf.SetTotalDetail(ttlSupply.ToString(), 8, ' ', true, false, true, true, "n2");
            gtf.SetTotalDetail(ttlDemand.ToString(), 8, ' ', false, true, true, true, "n2");
            gtf.SetTotalDetailLine();

            gtf.SetTotalDetail("NILAI PICKING LIST : " + dt.Rows[dt.Rows.Count - 1]["TotFinalSalesAmt"].ToString(), 45, ' ', true);
            gtf.SetTotalDetail("NO. ORDER : " + dt.Rows[dt.Rows.Count - 1]["OrderNo"].ToString(), 50, ' ', false, true);
            gtf.SetTotalDetail("KETERANGAN :" + dt.Rows[dt.Rows.Count - 1]["Remark"].ToString(), 96, ' ', false, true);
            gtf.SetTotalDetailSpace(65);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["CityName"].ToString() + ", " + DateTime.Now.ToString("dd-MMM-yyyy"), 31, ' ', false, true);

            gtf.SetTotalDetailSpace(1);
            gtf.SetTotalDetail("Diterima oleh :", 22, ' ', true);
            gtf.SetTotalDetailSpace(1);
            gtf.SetTotalDetail("Diserahkan oleh :", 22, ' ', true);
            gtf.SetTotalDetailSpace(1);
            gtf.SetTotalDetail("Diperikasa oleh :", 22, ' ', true);
            gtf.SetTotalDetailSpace(1);
            gtf.SetTotalDetail("Dibuat oleh :", 22, ' ', false, true);
            gtf.SetTotalDetail(" ", 1, ' ', false, true);
            gtf.SetTotalDetail(" ", 1, ' ', false, true);
            gtf.SetTotalDetailSpace(1);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["SignName_1"].ToString(), 22, ' ', true);
            gtf.SetTotalDetailSpace(1);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["Picker"].ToString(), 22, ' ', true);
            gtf.SetTotalDetailSpace(1);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["SignName_3"].ToString(), 22, ' ', true);
            gtf.SetTotalDetailSpace(1);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["SignName_4"].ToString(), 22, ' ', false, true);
            gtf.SetTotalDetailSpace(1);
            gtf.SetTotalDetail("-", 22, '-', true);
            gtf.SetTotalDetailSpace(1);
            gtf.SetTotalDetail("-", 22, '-', true);
            gtf.SetTotalDetailSpace(1);
            gtf.SetTotalDetail("-", 22, '-', true);
            gtf.SetTotalDetailSpace(1);
            gtf.SetTotalDetail("-", 22, '-', false, true);
            gtf.SetTotalDetailSpace(1);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["TitleSign_1"].ToString(), 22, ' ', true);
            gtf.SetTotalDetailSpace(1);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["TitleSign_2"].ToString(), 22, ' ', true);
            gtf.SetTotalDetailSpace(1);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["TitleSign_3"].ToString(), 22, ' ', true);
            gtf.SetTotalDetailSpace(1);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["TitleSign_4"].ToString(), 22, ' ', false, true);

            if (print == true)
                gtf.PrintTotal(true, lastData, false);
            else
            {
                if (gtf.PrintTotal(true, lastData, false) == true)
                    msg = "Save Berhasil";
                else
                    msg = "Save Gagal";
            }

            if (print == true)
                gtf.CloseConnectionPrinter();

            return gtf.sbDataTxt.ToString();
        }

        private string CreateReportSpRpTrn033V2(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage)
        {
            string[] arrayHeader = new string[7];
            string pickingSlipNo, printSeq, typeOfGoods, hasil;
            SpGenerateTextFileReport gtf = new SpGenerateTextFileReport();
            gtf.SetParameterPrinter(paramReport);
            gtf.SetPaperSize(1100, 850);
            gtf.GenerateTextFileReports(recordId, printerLoc, "W96", print, fileLocation, fullPage);
            gtf.GenerateHeader();

            pickingSlipNo = (dt.Rows[0]["PickingSlipNo"].ToString() != null) ? dt.Rows[0]["PickingSlipNo"].ToString() : string.Empty;
            printSeq = (dt.Rows[0]["PrintSeq"].ToString() != null) ? dt.Rows[0]["PrintSeq"].ToString() : string.Empty;
            typeOfGoods = (dt.Rows[0]["TypeOfGoods"].ToString() != null) ? dt.Rows[0]["TypeOfGoods"].ToString() : string.Empty;
            hasil = string.Format("{0}({1})({2})", pickingSlipNo, printSeq, typeOfGoods);

            arrayHeader[0] = hasil;
            arrayHeader[1] = dt.Rows[0]["PickingListType"].ToString();
            arrayHeader[2] = dt.Rows[0]["PickingSlipDate"].ToString();
            arrayHeader[3] = dt.Rows[0]["TransType"].ToString();
            arrayHeader[4] = dt.Rows[0]["PaymentType"].ToString();
            arrayHeader[5] = dt.Rows[0]["ExPickingSlipNo"].ToString();
            arrayHeader[6] = dt.Rows[0]["CustomerName"].ToString();

            CreateHdrSpRpTrn033(gtf, arrayHeader);
            gtf.PrintHeader();

            string docNo = "", psNo = "";
            int noUrut = 0;
            decimal ttlSupply = 0, ttlDemand = 0;
            bool lastData = false;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (psNo == "")
                {
                    psNo = dt.Rows[i]["PickingSlipNo"].ToString();
                    docNo = dt.Rows[i]["DocNo"].ToString();

                    noUrut++;
                    gtf.SetDataDetail("NOMOR DOKUMENT : ", 17, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["DocNo"].ToString(), 30, ' ', false, true);
                    gtf.SetDataDetail(noUrut.ToString(), 3, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["PartNo"].ToString(), 20, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 20, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["LocationCode"].ToString(), 8, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["QtyPicked"].ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["QtyOrder"].ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["OnHand"].ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["Allocation"].ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["MovingCode"].ToString(), 2, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["WarehouseCode"].ToString(), 2, ' ', false, true);

                    gtf.PrintData(fullPage);
                }
                else if (psNo != dt.Rows[i]["PickingSlipNo"].ToString())
                {
                    gtf.SetTotalDetailLine();

                    gtf.SetTotalDetail("Total : ", 54, ' ', true, false);
                    gtf.SetTotalDetail(ttlSupply.ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetTotalDetail(ttlDemand.ToString(), 8, ' ', false, true, true, true, "n2");
                    gtf.SetTotalDetailLine();

                    gtf.SetTotalDetail("NILAI PICKING LIST : ", 21, ' ');
                    gtf.SetTotalDetail(dt.Rows[i - 1]["TotFinalSalesAmt"].ToString(), 45, ' ', true, false, false, true, "n0");
                    gtf.SetTotalDetail("NO. ORDER : " + dt.Rows[i - 1]["OrderNo"].ToString(), 50, ' ', false, true);
                    gtf.SetTotalDetail("KETERANGAN :" + dt.Rows[i - 1]["Remark"].ToString(), 96, ' ', false, true);
                    gtf.SetTotalDetailSpace(65);
                    gtf.SetTotalDetail(dt.Rows[i - 1]["CityName"].ToString() + ", " + DateTime.Now.ToString("dd-MMM-yyyy"), 31, ' ', false, true);

                    gtf.SetTotalDetailSpace(1);
                    gtf.SetTotalDetail("Diterima oleh :", 22, ' ', true);
                    gtf.SetTotalDetailSpace(1);
                    gtf.SetTotalDetail("Diserahkan oleh :", 22, ' ', true);
                    gtf.SetTotalDetailSpace(1);
                    gtf.SetTotalDetail("Diperikasa oleh :", 22, ' ', true);
                    gtf.SetTotalDetailSpace(1);
                    gtf.SetTotalDetail("Dibuat oleh :", 22, ' ', false, true);
                    gtf.SetTotalDetail(" ", 1, ' ', false, true);
                    gtf.SetTotalDetail(" ", 1, ' ', false, true);
                    gtf.SetTotalDetailSpace(1);
                    gtf.SetTotalDetail(dt.Rows[i - 1]["SignName_1"] != null ? dt.Rows[i - 1]["SignName_1"].ToString() : " ", 22, ' ', true);
                    gtf.SetTotalDetailSpace(1);
                    gtf.SetTotalDetail(dt.Rows[i - 1]["Picker"] != null ? dt.Rows[i - 1]["Picker"].ToString() : " ", 22, ' ', true);
                    gtf.SetTotalDetailSpace(1);
                    gtf.SetTotalDetail(dt.Rows[i - 1]["SignName_3"] != null ? dt.Rows[i - 1]["SignName_3"].ToString() : " ", 22, ' ', true);
                    gtf.SetTotalDetailSpace(1);
                    gtf.SetTotalDetail(dt.Rows[i - 1]["SignName_4"] != null ? dt.Rows[i - 1]["SignName_4"].ToString() : " ", 22, ' ', false, true);
                    gtf.SetTotalDetailSpace(1);
                    gtf.SetTotalDetail("-", 22, '-', true);
                    gtf.SetTotalDetailSpace(1);
                    gtf.SetTotalDetail("-", 22, '-', true);
                    gtf.SetTotalDetailSpace(1);
                    gtf.SetTotalDetail("-", 22, '-', true);
                    gtf.SetTotalDetailSpace(1);
                    gtf.SetTotalDetail("-", 22, '-', false, true);
                    gtf.SetTotalDetailSpace(1);
                    gtf.SetTotalDetail(dt.Rows[i - 1]["TitleSign_1"].ToString(), 22, ' ', true);
                    gtf.SetTotalDetailSpace(1);
                    gtf.SetTotalDetail(dt.Rows[i - 1]["TitleSign_2"].ToString(), 22, ' ', true);
                    gtf.SetTotalDetailSpace(1);
                    gtf.SetTotalDetail(dt.Rows[i - 1]["TitleSign_3"].ToString(), 22, ' ', true);
                    gtf.SetTotalDetailSpace(1);
                    gtf.SetTotalDetail(dt.Rows[i - 1]["TitleSign_4"].ToString(), 22, ' ', false, true);
                    gtf.PrintTotal(false, lastData, false);

                    ttlSupply = ttlDemand = 0;

                    pickingSlipNo = (dt.Rows[i]["PickingSlipNo"].ToString() != null) ? dt.Rows[i]["PickingSlipNo"].ToString() : string.Empty;
                    printSeq = (dt.Rows[i]["PrintSeq"].ToString() != null) ? dt.Rows[i]["PrintSeq"].ToString() : string.Empty;
                    typeOfGoods = (dt.Rows[i]["TypeOfGoods"].ToString() != null) ? dt.Rows[i]["TypeOfGoods"].ToString() : string.Empty;
                    hasil = string.Format("{0}({1})({2})", pickingSlipNo, printSeq, typeOfGoods);

                    arrayHeader[0] = hasil;
                    arrayHeader[1] = dt.Rows[i]["PickingListType"].ToString();
                    arrayHeader[2] = dt.Rows[i]["PickingSlipDate"].ToString();
                    arrayHeader[3] = dt.Rows[i]["TransType"].ToString();
                    arrayHeader[4] = dt.Rows[i]["PaymentType"].ToString();
                    arrayHeader[5] = dt.Rows[i]["ExPickingSlipNo"].ToString();
                    arrayHeader[6] = dt.Rows[i]["CustomerName"].ToString();

                    CreateHdrSpRpTrn033(gtf, arrayHeader);
                    gtf.PrintAfterBreak();

                    lastData = false;
                    noUrut = 1;

                    gtf.SetDataDetail("NOMOR DOKUMENT : ", 17, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["DocNo"].ToString(), 30, ' ', false, true);

                    gtf.SetDataDetail(noUrut.ToString(), 3, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["PartNo"].ToString(), 20, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 20, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["LocationCode"].ToString(), 8, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["QtyPicked"].ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["QtyOrder"].ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["OnHand"].ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["Allocation"].ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["MovingCode"].ToString(), 2, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["WarehouseCode"].ToString(), 2, ' ', false, true);
                    gtf.PrintData(fullPage);

                    docNo = dt.Rows[i]["DocNo"].ToString();
                    psNo = dt.Rows[i]["PickingSlipNo"].ToString();
                }
                else if (psNo == dt.Rows[i]["PickingSlipNo"].ToString())
                {
                    if (docNo != dt.Rows[i]["DocNo"].ToString())
                    {
                        gtf.SetDataDetail("NOMOR DOKUMENT : ", 17, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["DocNo"].ToString(), 30, ' ', false, true);
                        docNo = dt.Rows[i]["DocNo"].ToString();
                        noUrut = 0;
                    }

                    noUrut++;
                    gtf.SetDataDetail(noUrut.ToString(), 3, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["PartNo"].ToString(), 20, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 20, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["LocationCode"].ToString(), 8, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["QtyPicked"].ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["QtyOrder"].ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["OnHand"].ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["Allocation"].ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["MovingCode"].ToString(), 2, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["WarehouseCode"].ToString(), 2, ' ', false, true);
                    if (i + 1 < dt.Rows.Count)
                    {
                        if (psNo == dt.Rows[i + 1]["PickingSlipNo"].ToString())
                            gtf.PrintData(fullPage);
                        else
                            lastData = true;
                    }
                    else
                        lastData = true;
                }

                ttlSupply += decimal.Parse(dt.Rows[i]["QtyPicked"].ToString());
                ttlDemand += decimal.Parse(dt.Rows[i]["QtyOrder"].ToString());
            }
            gtf.SetTotalDetailLine();

            gtf.SetTotalDetail("Total : ", 54, ' ', true, false);
            gtf.SetTotalDetail(ttlSupply.ToString(), 8, ' ', true, false, true, true, "n2");
            gtf.SetTotalDetail(ttlDemand.ToString(), 8, ' ', false, true, true, true, "n2");
            gtf.SetTotalDetailLine();

            gtf.SetTotalDetail("NILAI PICKING LIST : " + dt.Rows[dt.Rows.Count - 1]["TotFinalSalesAmt"].ToString(), 45, ' ', true);
            gtf.SetTotalDetail("NO. ORDER : " + dt.Rows[dt.Rows.Count - 1]["OrderNo"].ToString(), 50, ' ', false, true);
            gtf.SetTotalDetail("KETERANGAN :" + dt.Rows[dt.Rows.Count - 1]["Remark"].ToString(), 96, ' ', false, true);
            gtf.SetTotalDetailSpace(65);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["CityName"].ToString() + ", " + DateTime.Now.ToString("dd-MMM-yyyy"), 31, ' ', false, true);

            gtf.SetTotalDetailSpace(1);
            gtf.SetTotalDetail("Diterima oleh :", 22, ' ', true);
            gtf.SetTotalDetailSpace(1);
            gtf.SetTotalDetail("Diserahkan oleh :", 22, ' ', true);
            gtf.SetTotalDetailSpace(1);
            gtf.SetTotalDetail("Diperikasa oleh :", 22, ' ', true);
            gtf.SetTotalDetailSpace(1);
            gtf.SetTotalDetail("Dibuat oleh :", 22, ' ', false, true);
            gtf.SetTotalDetail(" ", 1, ' ', false, true);
            gtf.SetTotalDetail(" ", 1, ' ', false, true);
            gtf.SetTotalDetailSpace(1);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["SignName_1"].ToString(), 22, ' ', true);
            gtf.SetTotalDetailSpace(1);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["Picker"].ToString(), 22, ' ', true);
            gtf.SetTotalDetailSpace(1);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["SignName_3"].ToString(), 22, ' ', true);
            gtf.SetTotalDetailSpace(1);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["SignName_4"].ToString(), 22, ' ', false, true);
            gtf.SetTotalDetailSpace(1);
            gtf.SetTotalDetail("-", 22, '-', true);
            gtf.SetTotalDetailSpace(1);
            gtf.SetTotalDetail("-", 22, '-', true);
            gtf.SetTotalDetailSpace(1);
            gtf.SetTotalDetail("-", 22, '-', true);
            gtf.SetTotalDetailSpace(1);
            gtf.SetTotalDetail("-", 22, '-', false, true);
            gtf.SetTotalDetailSpace(1);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["TitleSign_1"].ToString(), 22, ' ', true);
            gtf.SetTotalDetailSpace(1);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["TitleSign_2"].ToString(), 22, ' ', true);
            gtf.SetTotalDetailSpace(1);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["TitleSign_3"].ToString(), 22, ' ', true);
            gtf.SetTotalDetailSpace(1);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["TitleSign_4"].ToString(), 22, ' ', false, true);

            if (print == true)
                gtf.PrintTotal(true, lastData, false);
            else
            {
                if (gtf.PrintTotal(true, lastData, false) == true)
                    msg = "Save Berhasil";
                else
                    msg = "Save Gagal";
            }

            if (print == true)
                gtf.CloseConnectionPrinter();

            return gtf.sbDataTxt.ToString();
        }

        private void CreateHdrSpRpTrn033(SpGenerateTextFileReport gtf, string[] arrayHeader)
        {
            gtf.CleanHeader();

            gtf.SetGroupHeader("NO. PICKING LIST", 18, ' ');
            gtf.SetGroupHeader(": " + arrayHeader[0].ToString(), 41, ' ', true);
            gtf.SetGroupHeader("JENIS PICKING LIST", 21, ' ');
            gtf.SetGroupHeader(": " + arrayHeader[1].ToString(), 15, ' ', false, true);
            gtf.SetGroupHeader("TGL. PICKING LIST", 18, ' ');
            gtf.SetGroupHeader(": " + DateTime.Parse(arrayHeader[2].ToString()).ToString("dd-MMM-yyyy"), 41, ' ', true);
            gtf.SetGroupHeader("KODE TRANSAKSI", 21, ' ');
            gtf.SetGroupHeader(": " + arrayHeader[3].ToString(), 15, ' ', false, true);
            gtf.SetGroupHeader("JENIS PEMBAYARAN", 18, ' ');
            gtf.SetGroupHeader(": " + arrayHeader[4].ToString(), 41, ' ', true);
            gtf.SetGroupHeader("EX. NO. PICKING SLIP", 21, ' ');
            gtf.SetGroupHeader(": " + arrayHeader[5].ToString(), 15, ' ', false, true);
            gtf.SetGroupHeader("CUSTOMER", 18, ' ');
            gtf.SetGroupHeader(": " + arrayHeader[6].ToString(), 78, ' ', false, true);

            gtf.SetGroupHeaderLine();
            gtf.SetGroupHeaderSpace(54);
            gtf.SetGroupHeader("QUANTITY", 36, ' ', false, true, false, true);
            gtf.SetGroupHeaderSpace(54);
            gtf.SetGroupHeader("-", 36, '-', false, true);
            gtf.SetGroupHeader("NO.", 3, ' ', true);
            gtf.SetGroupHeader("NO. PART", 20, ' ', true);
            gtf.SetGroupHeader("NAMA PART", 20, ' ', true);
            gtf.SetGroupHeader("LOKASI", 8, ' ', true);
            gtf.SetGroupHeader("SUPPLY", 8, ' ', true, false, true);
            gtf.SetGroupHeader("DEMAND", 8, ' ', true, false, true);
            gtf.SetGroupHeader("ON HAND", 8, ' ', true, false, true);
            gtf.SetGroupHeader("ALLOC", 8, ' ', true, false, true);
            gtf.SetGroupHeader("MC", 2, ' ', true);
            gtf.SetGroupHeader("WH", 2, ' ', false, true, true);
            gtf.SetGroupHeaderLine();
        }
        #endregion
    }
}