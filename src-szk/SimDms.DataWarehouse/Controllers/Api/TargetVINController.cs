using System;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using ClosedXML.Excel;
using SimDms.DataWarehouse.Models;
using System.Globalization;
using System.Data.SqlClient;

namespace SimDms.DataWarehouse.Controllers.Api
{
    public class TargetVINController : BaseController
    {
        DateTimeFormatInfo mfi = new DateTimeFormatInfo();
        int Rows = 0;
        #region Master Target Active VIN

        public JsonResult Default()
        {
            return Json(new
            {
                Year = DateTime.Now.Year
            });
        }

        public JsonResult getActiveVIN()
        {
            return Json(null);
        }

        public JsonResult CheckTargetVIN(string Year)
        {
            int year = Convert.ToInt16(Year);
            if (ctx.TargetVINs.Where(a => a.Year == year).Count() == 0)
            {
                return Json(new { success = false, message = "Tidak ada data yang ditampilkan!" });
            }
            else
            {
                return Json(new { success = true });
            }
        }

        public ActionResult GenerateTemplate(string Year, bool IsGenerate)
        {
            int year = Convert.ToInt16(Year);
            DateTime now = DateTime.Now;
            string fileName = "Input_TargetActiveVIN";

            //var datas = IsGenerate ? ctx.svMasterDealerMappings
            //    .Select(a => new TargetVINModel { GroupNo = a.GroupNo.Value, Area = a.Area, DealerCode = a.DealerCode, DealerName = a.DealerName, Target = 0 })
            //    .Distinct().OrderBy(a => a.GroupNo)
            //    : ctx.TargetVINs.Where(a => a.Year == year).Select(a => new TargetVINModel { GroupNo = a.GroupNo, Area = ctx.svMasterDealerMappings.FirstOrDefault(c => c.GroupNo == a.GroupNo).Area, DealerCode = a.DealerCode, DealerName = a.DealerName, Target = a.Target });
            //            var datas = ctx.Database.SqlQuery<TargetVINModel>(@"select a.GroupNo, Area, DealerCode, OutletCode, OutletName, Target
            //                    from (select distinct GroupNo, DealerCode, OutletCode, OutletName, 0 Target
            //                from vw_svMstDealerOutletMapping) a
            //                   inner join vw_svMstArea b
            //	                on a.GroupNo = b.GroupNo
            //                order by SeqNO ");    
            var datas = IsGenerate ? ctx.Database.SqlQuery<TargetVINModel>(@"select a.GroupNo, Area, DealerCode, OutletCode, OutletName, RevisitVin, NewVin, Target
                    from (select distinct GroupNo, DealerCode, OutletCode, OutletName, 0 RevisitVin, 0 NewVin, 0 Target
                from vw_svMstDealerOutletMapping) a
                   inner join vw_svMstArea b
	                on a.GroupNo = b.GroupNo
                order by SeqNO ")
                : ctx.Database.SqlQuery<TargetVINModel>(@"select a.GroupNo, Area, DealerCode, OutletCode, OutletName, RevisitVin, NewVin, Target 
                from svMstActiveVIN a                 
                 inner join vw_svMstArea b
	                on a.GroupNo = b.GroupNo
                where YEAR = {0}
                order by SeqNO", Year);
      
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Target Active VIN");

            // Setting Title

            ws.Cell("A1").Value = "TARGET ACTIVE VIN";
            var Title = ws.Range("A1:H1");
            Title.Merge();
            Title.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            Title.Style.Font.FontSize = 16;
            Title.Style.Font.SetBold(true);

            ws.Cell("B3").Value = "Tahun : " + Year;
            ws.Cell("B3").Style.Font.SetBold(true);

            // Setting Header

            ws.Column("A").Width = 3.29;
            ws.Column("B").Width = 25;
            ws.Column("C").Width = 12.15;
            ws.Column("D").Width = 12.15;
            ws.Column("E").Width = 41.15;
            ws.Column("F").Width = 12;
            ws.Column("G").Width = 12;
            ws.Column("H").Width = 12;

            var No = ws.Cell("A5");
            var Area = ws.Cell("B5");
            var DealerCode = ws.Cell("C5");
            var OutletCode = ws.Cell("D5");
            var DealerName = ws.Cell("E5");
            var Revisit = ws.Cell("F5");
            var baru = ws.Cell("G5");
            var Target = ws.Cell("H5"); 

            No.Value = "NO.";
            Area.Value = "AREA";
            DealerCode.Value = "KODE DEALER";
            OutletCode.Value = "KODE OUTLET";
            DealerName.Value = "NAMA OUTLET";
            Revisit.Value = "TOTAL TARGET\nREVISIT VIN";
            baru.Value = "TOTAL TARGET NEW\nACTIVE-VIN";
            Target.Value = "TOTAL TARGET\nACTIVE VIN";

            // Setting Style

            int iRow = 8;

            for (int iColumn = 1; iColumn <= iRow; iColumn++)
            {
                ws.Cell(5, iColumn).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                ws.Cell(5, iColumn).Style.Alignment.SetWrapText(true);
                ws.Cell(5, iColumn).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                ws.Cell(5, iColumn).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                ws.Cell(5, iColumn).Style.Font.SetBold(true);
                ws.Cell(5, iColumn).Style.Font.FontSize = 10;
                ws.Cell(5, iColumn).Style.Fill.SetBackgroundColor(XLColor.FromArgb(147, 205, 221));
            }

            // data

            int startIndex = 6;
            int seq = 1;
            foreach (var data in datas)
            {
                ws.Cell(startIndex, 1).Value = seq;
                ws.Cell(startIndex, 2).Value = data.Area;
                ws.Cell(startIndex, 3).Value = data.DealerCode;
                ws.Cell(startIndex, 4).Value = data.OutletCode;
                ws.Cell(startIndex, 5).Value = data.OutletName;
                ws.Cell(startIndex, 6).Value = data.RevisitVin;
                ws.Cell(startIndex, 7).Value = data.NewVin;
                ws.Cell(startIndex, 8).Value = data.Target;

                for (int iBorder = 1; iBorder <= 8; iBorder++)
                {
                    ws.Cell(startIndex, iBorder).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                }

                startIndex++;
                seq++;
            }

            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
            return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
        }


        public JsonResult UploadFile(HttpPostedFileBase file)
        {
            try
            {
                var fileType = file.FileName.ToLower().EndsWith("xlsx");

                if (!fileType)
                {
                    throw new Exception(
                    "File format is not supported. Please use XLSX Excel File");
                }
                //Open the Excel file using ClosedXML.
                using (XLWorkbook workBook = new XLWorkbook(file.InputStream))
                {
                    //Read the first Sheet from Excel file.
                    IXLWorksheet workSheet = workBook.Worksheet(1);

                    //Create a new DataTable.
                    DataTable dt = new DataTable();

                    //Loop through the Worksheet rows.
                    bool firstRow = true;
                    foreach (IXLRow row in workSheet.Rows())
                    {
                        //Use the first row to add columns to DataTable.
                        if (firstRow)
                        {
                            foreach (IXLCell cell in row.Cells())
                            {
                                dt.Columns.Add(cell.Value.ToString());
                            }
                            firstRow = false;
                        }
                        else
                        {
                            //Add rows to DataTable.
                            dt.Rows.Add();
                            int i = 0;
                            foreach (IXLCell cell in row.Cells())
                            {
                                dt.Rows[dt.Rows.Count - 1][i] = cell.Value.ToString();
                                i++;
                            }
                        }
                    }

                    string[] getYear = dt.Rows[0][0].ToString().Split(':');
                    var year = Convert.ToInt16(getYear[1].Trim());

                    List<TargetVIN> targetVINList = new List<TargetVIN>();
                    foreach (DataRow row in dt.AsEnumerable().Skip(2))
                    {
                        var targetVIN = new TargetVIN();
                        targetVIN.GroupNo = GetGroupNo(ctx, row[1].ToString());
                        targetVIN.DealerCode = row[2].ToString();
                        targetVIN.OutletCode = row[3].ToString();
                        targetVIN.OutletName = row[4].ToString();
                        targetVIN.RevisitVin = string.IsNullOrEmpty(row[5].ToString()) ? 0 : Convert.ToInt32(row[5]);
                        targetVIN.NewVin = string.IsNullOrEmpty(row[5].ToString()) ? 0 : Convert.ToInt32(row[6]);
                        targetVIN.Target = string.IsNullOrEmpty(row[5].ToString()) ? 0 : Convert.ToInt32(row[7]);
                        targetVINList.Add(targetVIN);
                    }

                    var _save = InsertActiveVIN(ctx, year, targetVINList, CurrentUser.Username);

                    if (_save == false){
                        return Json(new { success = _save, message = "Upload Failed." });
                    }
                    else
                    {
                        return Json(new { success = _save, message = "Upload success." });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private static int GetGroupNo(DataContext ctx, string Area)
        {
            return ctx.svMasterDealerMappings.FirstOrDefault(a => a.Area == Area).GroupNo.Value;
        }

        private static string GetArea(DataContext ctx, int GroupNo)
        {
            return ctx.svMasterDealerMappings.FirstOrDefault(a => a.GroupNo == GroupNo).Area;
        }

        private static bool InsertActiveVIN(DataContext ctx, int year, List<TargetVIN> targetVINs, string user)
        {
            var data = ctx.TargetVINs.Where(a => a.Year == year);
            var iSave = false;

            using (var trans = ctx.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                try
                {
                    if (data.Count() > 0)
                    {
                        var query = string.Format("delete svMstActiveVIN where Year = {0}", year);
                        ctx.Database.ExecuteSqlCommand(query);
                    }

                    foreach (var target in targetVINs)
                    {
                        var targetVIN = new TargetVIN();
                        targetVIN.GroupNo = target.GroupNo;
                        targetVIN.DealerCode = target.DealerCode;
                        targetVIN.OutletCode = target.OutletCode;
                        targetVIN.OutletName = target.OutletName;
                        targetVIN.Year = year;
                        targetVIN.RevisitVin = target.RevisitVin;
                        targetVIN.NewVin = target.NewVin;
                        targetVIN.Target = target.Target;
                        targetVIN.CreatedBy = user;
                        targetVIN.CreatedDate = DateTime.Now;
                        targetVIN.LastUpdateBy = user;
                        targetVIN.LastUpdateDate = DateTime.Now;
                        ctx.TargetVINs.Add(targetVIN);
                    }

                    iSave = ctx.SaveChanges() > 0;
                    trans.Commit();
                    return iSave;
                }
                catch (Exception ex)
                {
                    trans.Dispose();
                    return iSave;
                }
            }
        }

        #endregion

        #region Inquiry Target Active VIN
        private class prmVIN 
        {
            public string Area { get; set; }
            public string Dlr { get; set; }
            public string Otlt { get; set; }
            public string UIOStart { get; set; } 
            public string UIOEnd{ get; set; }
            public string YearStart { get; set; }
            public string YearEnd { get; set; }
            public string Vin { get; set; } 
            public string InqOpt { get; set; }
            public int istype { get; set; }
        }

        private class DlrList
        {
            public string Number { get; set; } 
            public string SeqNo { get; set; } 
            public string GroupNo { get; set; }
            public string Area { get; set; }
            public string GNDealerCode { get; set; }
            public string GNOutletCode { get; set; }
            public string OutletName { get; set; }
        }
        private DataTable CreateTable(prmVIN param, string SPName)
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600; cmd.CommandText = SPName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@GroupNo", param.Area);
            cmd.Parameters.AddWithValue("@CompanyCode", param.Dlr);
            cmd.Parameters.AddWithValue("@BranchCode", param.Otlt);
            cmd.Parameters.AddWithValue("@PeriodVin", param.Vin);
            cmd.Parameters.AddWithValue("@YearStart", param.YearStart);
            cmd.Parameters.AddWithValue("@YearEnd", param.YearEnd);
            cmd.Parameters.AddWithValue("@UIOStart", param.UIOStart);
            cmd.Parameters.AddWithValue("@UIOEnd", param.UIOEnd);
            cmd.Parameters.AddWithValue("@InqOption", param.InqOpt);
            cmd.Parameters.AddWithValue("@istype", param.istype);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public JsonResult ActiveVINType(int option)
        {
            List<Object> listObj = new List<Object>();

            listObj.Add(new { value = "Revisit", text = "Revisit" });
            if (option == 0)
            {
                listObj.Add(new { value = "NewVisit", text = "New Visit" });
            }
            return Json(listObj);
        }

        public ActionResult GenerateSummaryVIN
        (string area, string dealer, string outlet, string yearStart, string yearEnd, string VIN, string uiostart, string uioend, string typeAktifVin) 
        {
            var data = ctx.Database.SqlQuery<getAreaDealerOutlet>("exec uspfn_GetAreaDealerOutletSvr @GroupNo=@p0, @dealercode=@p1, @BranchCode=@p2", area, dealer, outlet);
            var areaName = data.FirstOrDefault().Area;
            var dealerName = data.FirstOrDefault().Dealer;
            var outletName = data.FirstOrDefault().Showroom;

            DateTime now = DateTime.Now;
            string fileName = "Active VIN";
            //tAvailableDate = DateTime.Now.AddDays(21);
            var wb = new XLWorkbook();

            #region Sales Unit
            var ws1 = wb.Worksheets.Add("Sales Unit");
            ws1.Style.Font.SetFontName("calibri");
            ws1.Style.Font.SetFontSize(9);
            ws1.ColumnWidth = 8.43;
            ws1.Column(1).Width = 25;
            ws1.Column(2).Width = 44;
            ws1.Row(7).Height = 55;
            ws1.Row(7).Style.Font.SetFontColor(XLColor.White);

            // Setting Title

            ws1.Cell("A1").Value = "Sales Unit";
            var Title = ws1.Range("A1:C1");
            Title.Merge();
            Title.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
            Title.Style.Font.FontSize = 16;
            Title.Style.Font.SetBold(true);

            ws1.Cell("A3").Value = "AREA";
            ws1.Cell("A4").Value = "DEALER";
            ws1.Cell("A5").Value = "OUTLET";
            //ws1.Cell("A6").Value = "TIPE ACTIVE VIN";
            //ws1.Cell("A7").Value = "PERIOD OF ACTIVE VIN";

            ws1.Cell("B3").Value = ": " + areaName;
            ws1.Cell("B4").Value = ": " + dealerName;
            ws1.Cell("B5").Value = ": " + outletName;
            //ws1.Cell("B6").Value = ": ";
            //ws1.Cell("B7").Value = ": ";

            ws1.Range(3, 1, 5, 1).Style.Font.Bold = true;
            // Setting Header

            // ROW 9
            var lastRowSU = 7;
            
            #endregion
             
            #region Summary VIN
            var ws = wb.Worksheets.Add("Summary VIN");
            ws.Style.Font.SetFontName("calibri");
            ws.Style.Font.SetFontSize(9);
            ws.ColumnWidth = 8.43;
            ws.Column(1).Width = 27;
            ws.Column(2).Width = 44;

            // Setting Title

            ws.Cell("A1").Value = "SUMMARY VIN";
            var Titles = ws.Range("A1:C1");
            Titles.Merge();
            Titles.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
            Titles.Style.Font.FontSize = 16;
            Titles.Style.Font.SetBold(true);

            ws.Cell("A3").Value = "AREA";
            ws.Cell("A4").Value = "DEALER";
            ws.Cell("A5").Value = "OUTLET";
            //ws.Cell("A6").Value = "TIPE ACTIVE VIN";
            ws.Cell("A6").Value = "PERIOD OF ACTIVE VIN";

            ws.Cell("B3").Value = ": " + areaName;
            ws.Cell("B4").Value = ": " + dealerName;
            ws.Cell("B5").Value = ": " + outletName;
            //ws.Cell("B6").Value = ": ALL"; //+ (typeAktifVin == "" ?  : typeAktifVin);
            ws.Cell("B6").Value = ": " + (VIN == "" ? "ALL" : VIN);

            ws.Range(3, 1, 7, 1).Style.Font.Bold = true;
            // Setting Header

            // ROW 10
            ws.Cell("A10").Value = "Region";
            var Region = ws.Range("A10:A12");
            Region.Merge();
            Region.Style.Alignment.SetWrapText(true);
            Region.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            Region.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

            ws.Cell("B10").Value = "Nama Dealer";
            var Dealer = ws.Range("B10:B12");
            Dealer.Merge();
            Dealer.Style.Alignment.SetWrapText(true);
            Dealer.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            Dealer.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

            ws.Cell("C10").Value = Convert.ToInt32(VIN) - 1;
            var prevYear = ws.Range("C10:E10");
            prevYear.Merge();
            //prevYear.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            prevYear.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            prevYear.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

            ws.Cell("F10").Value = VIN;
            var nowYear = ws.Range("F10:O10");
            nowYear.Merge();
            nowYear.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            nowYear.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            // ROW 11 old UIO
            ws.Range("C11:C12").Merge();
            ws.Range("D11:D12").Merge();
            ws.Range("E11:E12").Merge();
            ws.Cell("C11").Value = "UIO " + (Convert.ToInt32(uiostart) - 1).ToString() + " - " + (Convert.ToInt32(uioend) - 1).ToString();
            ws.Cell("D11").Value = "Total Active VIN";
            ws.Cell("E11").Value = "Percentage (%)";

            // ROW 11 new UIO
            ws.Range("F11:F12").Merge();
            ws.Cell("F11").Value = "UIO " + uiostart + " - " + uioend;
            ws.Range("G11:J11").Merge().Style.Alignment.SetWrapText(true);
            ws.Cell("G11").Value = "Target";// Active VIN";
            ws.Range("K11:M11").Merge().Style.Alignment.SetWrapText(true); ;
            ws.Cell("K11").Value = "Actual Active VIN\n(Excl. Sales Unit " + VIN + ")";//"Total Active VIN (Excl. Sales Unit" + VIN + ")";
            ws.Range("N11:O11").Merge().Style.Alignment.SetWrapText(true); ;
            ws.Cell("N11").Value = "Percentage (%)\nTotal Active VIN";

            // ROW 12
            ws.Cell("G12").Value = "Total Revisit VIN";
            ws.Cell("H12").Value = "Total New Active VIN";
            ws.Cell("I12").Value = "Total Active VIN";
            ws.Cell("J12").Value = "Percentage (%) Total Active VIN";
            ws.Cell("K12").Value = "Revisit Active VIN\n(From Prev year)";
            ws.Cell("L12").Value = "Total New Active VIN";
            ws.Cell("M12").Value = "Total Active VIN";
            ws.Cell("N12").Value = "to UIO";
            ws.Cell("O12").Value = "to Total Target";

            int istype = 0;
            int mColumn = 16;
            //if (typeAktifVin == "")
            //{
                #region All
                // ROW 9
                ws.Cell("P9").Value = "Period of Monitoring";
                var Header = ws.Range("P9:AY9");
                Header.Merge();
                Header.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                Header.Style.Fill.SetBackgroundColor(XLColor.FromArgb(0, 176, 80));
                Header.Style.Font.FontSize = 11;
                Header.Style.Font.SetBold(true);

                for (int i = 1; i < 13; i++)
                {
                    ws.Cell(10, mColumn).Value = "'" + mfi.GetAbbreviatedMonthName(i) + "-" + VIN; //now.Year.ToString().Substring(2);
                    var x = ws.Range(ws.Cell(10, mColumn).Address, ws.Cell(10, mColumn + 2).Address);
                    x.Merge();
                    var y = ws.Range(ws.Cell(11, mColumn).Address, ws.Cell(12, mColumn).Address);
                    y.Merge();
                    var z = ws.Range(ws.Cell(11, mColumn + 1).Address, ws.Cell(12, mColumn + 1).Address);
                    z.Merge();
                    var a = ws.Range(ws.Cell(11, mColumn + 2).Address, ws.Cell(12, mColumn + 2).Address);
                    a.Merge();

                    ws.Cell(11, mColumn).Value = "Revisit Active VIN (From Prev year)";
                    ws.Cell(11, mColumn + 1).Value = "New Active VIN (Excl.Sales Unit " + VIN + ")";
                    ws.Cell(11, mColumn + 2).Value = "New VIN " + VIN + "\n(Sales Unit " + VIN + ")";

                    mColumn = mColumn + 3;
                }
                // Setting Style
                for (int iColumn = 1; iColumn <= 51; iColumn++)
                {
                    ws.Cell(10, iColumn).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                    ws.Cell(10, iColumn).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    ws.Cell(10, iColumn).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                    ws.Cell(10, iColumn).Style.Fill.SetBackgroundColor(XLColor.FromArgb(215, 228, 188));
                    ws.Cell(10, iColumn).Style.Font.SetBold(true);

                    ws.Cell(11, iColumn).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                    ws.Cell(11, iColumn).Style.Alignment.SetWrapText(true);
                    ws.Cell(11, iColumn).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    ws.Cell(11, iColumn).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                    ws.Cell(11, iColumn).Style.Fill.SetBackgroundColor(XLColor.FromArgb(215, 228, 188));
                    ws.Cell(11, iColumn).Style.Font.SetBold(true);

                    ws.Cell(12, iColumn).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                    ws.Cell(12, iColumn).Style.Alignment.SetWrapText(true);
                    ws.Cell(12, iColumn).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    ws.Cell(12, iColumn).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                    ws.Cell(12, iColumn).Style.Fill.SetBackgroundColor(XLColor.FromArgb(215, 228, 188));
                    ws.Cell(12, iColumn).Style.Font.SetBold(true);
                }
                #endregion All
            //}
            //else
            //{
            //    ws.Cell("P9").Value = "Period of Monitoring";
            //    var Header = ws.Range("P9:AM9");
            //    Header.Merge();
            //    Header.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            //    Header.Style.Fill.SetBackgroundColor(XLColor.FromArgb(0, 176, 80));
            //    Header.Style.Font.FontSize = 11;
            //    Header.Style.Font.SetBold(true);
            //}
            #region New Revisit
            //if (typeAktifVin == "Revisit")
            //{
            //    istype = 1;
            //    #region Revisit
            //    mColumn = 10;
            //    for (int i = 1; i < 13; i++)
            //    {
            //        var x = ws.Range(ws.Cell(10, mColumn).Address, ws.Cell(10, mColumn + 1).Address);
            //        x.Merge();
            //        var y = ws.Range(ws.Cell(11, mColumn).Address, ws.Cell(12, mColumn).Address);
            //        y.Merge();
            //        var z = ws.Range(ws.Cell(11, mColumn + 1).Address, ws.Cell(12, mColumn + 1).Address);
            //        z.Merge();
            //        ws.Cell(10, mColumn).Value = "'" + mfi.GetAbbreviatedMonthName(i) + "-" + VIN; //now.Year.ToString().Substring(2);
            //        ws.Cell(11, mColumn).Value = "Revisit Active VIN (From Prev year)";
            //        ws.Cell(11, mColumn + 1).Value = "New VIN " + VIN + "\n(Sales Unit " + VIN + ")";
            //        mColumn = mColumn + 2;
            //    }

            //    // Setting Style
            //    for (int iColumn = 1; iColumn <= 39; iColumn++)
            //    {
            //        ws.Cell(10, iColumn).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            //        ws.Cell(10, iColumn).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            //        ws.Cell(10, iColumn).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            //        ws.Cell(10, iColumn).Style.Fill.SetBackgroundColor(XLColor.FromArgb(215, 228, 188));
            //        ws.Cell(10, iColumn).Style.Font.SetBold(true);

            //        ws.Cell(11, iColumn).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            //        ws.Cell(11, iColumn).Style.Alignment.SetWrapText(true);
            //        ws.Cell(11, iColumn).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            //        ws.Cell(11, iColumn).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            //        ws.Cell(11, iColumn).Style.Fill.SetBackgroundColor(XLColor.FromArgb(215, 228, 188));
            //        ws.Cell(11, iColumn).Style.Font.SetBold(true);

            //        ws.Cell(12, iColumn).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            //        ws.Cell(12, iColumn).Style.Alignment.SetWrapText(true);
            //        ws.Cell(12, iColumn).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            //        ws.Cell(12, iColumn).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            //        ws.Cell(12, iColumn).Style.Fill.SetBackgroundColor(XLColor.FromArgb(215, 228, 188));
            //        ws.Cell(12, iColumn).Style.Font.SetBold(true);
            //    }
            //    #endregion Revisit
            //}
            //if (typeAktifVin == "NewVisit")
            //{
            //    istype = 2;
            //    #region New Visit
            //    mColumn = 10;

            //    for (int i = 1; i < 13; i++)
            //    {
            //        var x = ws.Range(ws.Cell(10, mColumn).Address, ws.Cell(10, mColumn + 1).Address);
            //        x.Merge();
            //        var y = ws.Range(ws.Cell(11, mColumn).Address, ws.Cell(12, mColumn).Address);
            //        y.Merge();
            //        var z = ws.Range(ws.Cell(11, mColumn + 1).Address, ws.Cell(12, mColumn + 1).Address);
            //        z.Merge();
            //        ws.Cell(10, mColumn).Value = "'" + mfi.GetAbbreviatedMonthName(i) + "-" + VIN; //now.Year.ToString().Substring(2);
            //        ws.Cell(11, mColumn).Value = "New Active VIN (Excl.Sales Unit " + VIN + ")";
            //        ws.Cell(11, mColumn + 1).Value = "New VIN " + VIN + "\n(Sales Unit " + VIN + ")";
            //        mColumn = mColumn + 2;
            //    }

            //    // Setting Style
            //    for (int iColumn = 1; iColumn <= 39; iColumn++)
            //    {
            //        ws.Cell(10, iColumn).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            //        ws.Cell(10, iColumn).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            //        ws.Cell(10, iColumn).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            //        ws.Cell(10, iColumn).Style.Fill.SetBackgroundColor(XLColor.FromArgb(215, 228, 188));
            //        ws.Cell(10, iColumn).Style.Font.SetBold(true);

            //        ws.Cell(11, iColumn).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            //        ws.Cell(11, iColumn).Style.Alignment.SetWrapText(true);
            //        ws.Cell(11, iColumn).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            //        ws.Cell(11, iColumn).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            //        ws.Cell(11, iColumn).Style.Fill.SetBackgroundColor(XLColor.FromArgb(215, 228, 188));
            //        ws.Cell(11, iColumn).Style.Font.SetBold(true);

            //        ws.Cell(12, iColumn).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            //        ws.Cell(12, iColumn).Style.Alignment.SetWrapText(true);
            //        ws.Cell(12, iColumn).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            //        ws.Cell(12, iColumn).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            //        ws.Cell(12, iColumn).Style.Fill.SetBackgroundColor(XLColor.FromArgb(215, 228, 188));
            //        ws.Cell(12, iColumn).Style.Font.SetBold(true);
            //    }
            //    #endregion Revisit
            //}
            #endregion New Revisit

            #endregion

            ws.Row(11).Height = 48;
            ws.Range("G11:J12").Style.Fill.SetBackgroundColor(XLColor.FromArgb(192, 0, 0));
            ws.Range("N11:O12").Style.Fill.SetBackgroundColor(XLColor.FromArgb(192, 0, 0));
            ws.Range("G11:J12").Style.Font.SetFontColor(XLColor.White);
            ws.Range("N11:O12").Style.Font.SetFontColor(XLColor.White);
            prmVIN param = new prmVIN
            {
                Area = area,
                Dlr = dealer,
                Otlt = outlet,
                UIOStart = uiostart,
                UIOEnd = uioend,
                Vin = VIN,
                YearStart = yearStart == "" ? "1980" : yearStart,
                YearEnd = yearEnd,
                InqOpt = "summary",
                istype = 0
            };

            DataTable dt1 = CreateTable(param, "usprpt_InqSalesUnit");
            DataTable dt = CreateTable(param, "usprpt_InqActiveVinv2");
            //DataTable dt2 = CreateTable(param, "usprpt_InqSalesUnit"); 
            //return GenerateExcel( wb, dt, dt1, 12, fileName, true);
            GenerateExcel(wb, dt, dt1, 13, fileName, true);
            GenerateDetailVIN(wb, fileName, area, dealer, outlet, yearStart, yearEnd, VIN, uiostart, uioend, typeAktifVin);
            GenerateExcelInvalid(wb, fileName, area, dealer, outlet, yearStart, yearEnd, VIN, uiostart, uioend, typeAktifVin);
            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
            return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
        }

        private XLWorkbook GenerateExcel(XLWorkbook wb, DataTable dt, DataTable dt1, int lastRow, string fileName, bool isCustomize = false, bool isCustomHeader = false, bool isShowSummary = false)
        {
            var MRow = lastRow;

            #region SalesUnit
            var ws1 = wb.Worksheet(1);
            var tmpLastRows = MRow - 6;
            var LastRows = MRow - 6;

            int iCols = 1;
            foreach (DataRow dr in dt1.Rows)
            {
                iCols = 1;
                foreach (DataColumn dc in dt1.Columns)
                {
                    var val = dr[dc.ColumnName];
                    Type typ = dr[dc.ColumnName].GetType();
                    switch (Type.GetTypeCode(typ))
                    {
                        case TypeCode.DateTime:
                            ws1.Cell(LastRows, iCols).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            ws1.Cell(LastRows, iCols).Style.DateFormat.Format = "dd-MMM-yyyy";
                            break;
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Double:
                        case TypeCode.Single:
                            ws1.Cell(LastRows, iCols).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws1.Cell(LastRows, iCols).Style.NumberFormat.Format = "_(* #,##0_);_(* (#,##0);_(* \" - \";_(@_)";
                            break;
                        case TypeCode.Decimal:
                            ws1.Cell(LastRows, iCols).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws1.Cell(LastRows, iCols).Style.NumberFormat.Format = "#,##0.0";
                            break;
                        case TypeCode.Boolean:
                            ws1.Cell(LastRows, iCols).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            break;
                        default:
                            if (string.IsNullOrWhiteSpace(val.ToString()) == false)
                            {
                                val = val.ToString().Substring(0, 1) == "0" ? val.ToString().Insert(0, "'") : val;
                            }
                            ws1.Cell(LastRows, iCols).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            break;
                    };

                    if (dr[dc.ColumnName].GetType() == typeof(DateTime))
                    {
                        ws1.Cell(LastRows, iCols).Style.DateFormat.Format = "dd-MMM-yyyy";
                    }

                    if (tmpLastRows == LastRows )
                    {
                        ws1.Cell(LastRows, iCols).Value = "'"+ dc.ColumnName;
                        ws1.Cell(LastRows, iCols).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        //ws1.Cell(LastRows, iCols).Style.Fill.SetBackgroundColor(XLColor.Yellow).Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        //ws1.Cell(LastRows, iCols).Style.Font.SetBold().Font.SetFontSize(10);

                        ws1.Cell(LastRows + 1, iCols).Style.Font.SetFontSize(10);
                        ws1.Cell(LastRows + 1, iCols).Value = val;
                    }
                    else
                    {
                        ws1.Cell(LastRows + 1, iCols).Style.Font.SetFontSize(10);
                        ws1.Cell(LastRows + 1, iCols).Value = val;
                    }

                    iCols++;
                }

                LastRows++;
            }
            ws1.Cell(LastRows + 1, 1).Value = "TOTAL";
            var column = 3;
            for (char i = 'C'; i <= 'P'; i++)
            {
                ws1.Cell(LastRows + 1, column).FormulaA1 = "=SUM(" + i + (tmpLastRows + 1) + ":" + i + (LastRows) + ")";
                column++;
            }
            //for (char i = 'A'; i <= 'G'; i++)
            //{
            //    ws1.Cell(LastRows, column).FormulaA1 = "=SUM(A" + i + (tmpLastRows + 1) + ":A" + i + (LastRows - 1) + ")";
            //    column++;
            //} 
            var rngTables = ws1.Range(tmpLastRows, 1, LastRows, iCols - 1);
            //rngTables.Style.Font.Bold = true;
                //.Border.SetTopBorder(XLBorderStyleValues.Thin)
                //.Border.SetBottomBorder(XLBorderStyleValues.Thin)
                //.Border.SetLeftBorder(XLBorderStyleValues.Thin)
                //.Border.SetRightBorder(XLBorderStyleValues.Thin);
            var Header = ws1.Range(tmpLastRows, 1, tmpLastRows, iCols - 1);
            Header.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            Header.Style.Fill.SetBackgroundColor(XLColor.FromArgb(55, 96, 145));
            Header.Style.Font.FontSize = 11;
            Header.Style.Font.SetBold(true);

            var IsiData = ws1.Range((tmpLastRows + 1), 1, LastRows, iCols - 1);
            IsiData.Style.Fill.SetBackgroundColor(XLColor.FromArgb(219, 229, 241));
            IsiData.Style.Font.FontSize = 11;

            ws1.Range(LastRows + 1, 1, LastRows + 1, iCols - 1).Style.NumberFormat.Format = "_(* #,##0_);_(* (#,##0);_(* \" - \";_(@_)";
            ws1.Range(LastRows + 1, 1, LastRows + 1, iCols - 1).Style.Font.Bold = true;
            ws1.Range(LastRows + 1, 1, LastRows + 1, iCols - 1).Style.Fill.SetBackgroundColor(XLColor.FromArgb(191, 191, 191));
            ws1.Range(LastRows + 1, 1, LastRows + 1, iCols - 1).Style.Font.FontSize = 11;
            //ws1.Columns(1, 2).AdjustToContents();
            #endregion

            #region Summary Vin
            var ws = wb.Worksheet(2);
            var tmpLastRow = MRow;

            int iCol = 1;
            foreach (DataRow dr in dt.Rows)
            {
                iCol = 1;
                foreach (DataColumn dc in dt.Columns)
                {
                    var val = dr[dc.ColumnName];
                    Type typ = dr[dc.ColumnName].GetType();
                    switch (Type.GetTypeCode(typ))
                    {
                        case TypeCode.DateTime:
                            ws.Cell(lastRow, iCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            ws.Cell(lastRow, iCol).Style.DateFormat.Format = "dd-MMM-yyyy";
                            break;
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Double:
                        case TypeCode.Single:
                            ws.Cell(lastRow, iCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(lastRow, iCol).Style.NumberFormat.Format = "_(* #,##0_);_(* (#,##0);_(* \" - \";_(@_)";
                            break;
                        case TypeCode.Decimal:
                            ws.Cell(lastRow, iCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(lastRow, iCol).Style.NumberFormat.Format = "#,##0.0";
                            break;
                        case TypeCode.Boolean:
                            ws.Cell(lastRow, iCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            break;
                        default:
                            if (string.IsNullOrWhiteSpace(val.ToString()) == false)
                            {
                                val = val.ToString().Substring(0, 1) == "0" ? val.ToString().Insert(0, "'") : val;
                            }
                            ws.Cell(lastRow, iCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            break;
                    };

                    if (dr[dc.ColumnName].GetType() == typeof(DateTime))
                    {
                        ws.Cell(lastRow, iCol).Style.DateFormat.Format = "dd-MMM-yyyy";
                    }

                    if (tmpLastRow == lastRow && isCustomize == false)
                    {
                        ws.Cell(lastRow, iCol).Value = dc.ColumnName;
                        ws.Cell(lastRow, iCol).Style.Fill.SetBackgroundColor(XLColor.Yellow).Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell(lastRow, iCol).Style.Font.SetBold().Font.SetFontSize(10);

                        ws.Cell(lastRow + 1, iCol).Style.Font.SetFontSize(10);
                        ws.Cell(lastRow + 1, iCol).Value = val;                        
                    }
                    else
                    {
                        ws.Cell(lastRow, iCol).Style.Font.SetFontSize(10);
                        if (iCol == 5 || iCol == 10 || iCol == 13 || iCol == 14 || iCol == 15)
                        {
                            if (iCol == 5)
                            {
                                ws.Cell(lastRow, iCol).FormulaA1 = "=IFERROR(D" + (lastRow) + "/C" + lastRow +",0)";//"=D" + (lastRow) + "/C" + lastRow; //
                            }

                            if (iCol == 10)
                            {
                                ws.Cell(lastRow, iCol).FormulaA1 = "=IFERROR(I" + (lastRow) + "/F" + lastRow +",0)";//"=I" + (lastRow) + "/F" + lastRow;
                            }

                            if (iCol == 13)
                            {
                                ws.Cell(lastRow, iCol).FormulaA1 = "=K" + (lastRow) + "+L" + lastRow;
                            }

                            if (iCol == 14)
                            {
                                ws.Cell(lastRow, iCol).FormulaA1 = "=IFERROR(M" + (lastRow) + "/F" + lastRow +",0)";//"=M" + (lastRow) + "/F" + lastRow;
                            }

                            if (iCol == 15)
                            {
                                ws.Cell(lastRow, iCol).FormulaA1 = "=IFERROR(M" + (lastRow) + "/I" + lastRow +",0)";//"=M" + (lastRow) + "/I" + lastRow;
                            }

                        }
                        else
                        {
                            ws.Cell(lastRow , iCol).Value = val;
                        }
                        //ws.Cell(lastRow, iCol).Value = val;
                    }

                    iCol++;
                }

                lastRow++;
            }
            ws.Cell(lastRow, 1).Value = "TOTAL";
            column = 3;
            if (iCol > 26)
            {
                char alphaStart = GetChar(column - 1);
                for (char i = alphaStart; i <= 'Z'; i++)
                {
                    if (column == 5 || column == 10 || column == 14 || column == 15)
                    {
                        if (column == 5)
                        {
                            ws.Cell(lastRow, column).FormulaA1 = "=IFERROR(D" + (lastRow) + "/C" + lastRow + ",0)";//"=D" + (lastRow) + "/C" + lastRow;
                        }

                        if (column == 10)
                        {
                            ws.Cell(lastRow, column).FormulaA1 = "=IFERROR(I" + (lastRow) + "/F" + lastRow + ",0)";//"=I" + (lastRow) + "/F" + lastRow;
                        }

                        if (column == 14)
                        {
                            ws.Cell(lastRow, column).FormulaA1 = "=IFERROR(M" + (lastRow) + "/F" + lastRow + ",0)";//"=M" + (lastRow) + "/F" + lastRow;
                        }

                        if (column == 15)
                        {
                            ws.Cell(lastRow, column).FormulaA1 = "=IFERROR(M" + (lastRow) + "/I" + lastRow + ",0)";//"=M" + (lastRow) + "/I" + lastRow;
                        }
                    }
                    else
                    {
                        ws.Cell(lastRow, column).FormulaA1 = "=SUM(" + i + (tmpLastRow) + ":" + i + (lastRow - 1) + ")";
                    }
                    
                    column++;
                }

                char alphaEnd = GetChar((iCol - 26) - 2);
                for (char i = 'A'; i <= alphaEnd; i++)
                {
                    ws.Cell(lastRow, column).FormulaA1 = "=SUM(A" + i + (tmpLastRow) + ":A" + i + (lastRow - 1) + ")";
                    column++;
                }
            }
            else
            {
                char alphaStart = GetChar(column - 1);
                char alphaEnd = GetChar((26 - iCol) - 1);
                for (char i = alphaStart; i <= alphaEnd; i++)
                {
                    ws.Cell(lastRow, column).FormulaA1 = "=SUM(" + i + (tmpLastRow) + ":" + i + (lastRow - 1) + ")";
                    column++;
                }
            }
            var rngTable = ws.Range(tmpLastRow, 1, lastRow, iCol - 1);
            rngTable.Style
                .Border.SetTopBorder(XLBorderStyleValues.Thin)
                .Border.SetBottomBorder(XLBorderStyleValues.Thin)
                .Border.SetLeftBorder(XLBorderStyleValues.Thin)
                .Border.SetRightBorder(XLBorderStyleValues.Thin);
            rngTable.Style.Font.FontSize = 9;
            var IsiDatas = ws.Range(tmpLastRow, 1, lastRow, iCol - 1);
            IsiDatas.Style.Fill.SetBackgroundColor(XLColor.FromArgb(234, 241, 221));

            ws.Range(lastRow, 1, lastRow, iCol - 1).Style.NumberFormat.Format = "_(* #,##0_);_(* (#,##0);_(* \" - \";_(@_)";
            ws.Range(lastRow, 1, lastRow, iCol - 1).Style.Font.Bold = true;
            ws.Range(lastRow, 1, lastRow, iCol - 1).Style.Fill.SetBackgroundColor(XLColor.FromArgb(191, 191, 191));
            ws.Column(5).Style.NumberFormat.Format = "0%";
            ws.Column(10).Style.NumberFormat.Format = "0%";
            ws.Column(14).Style.NumberFormat.Format = "0%";
            ws.Column(15).Style.NumberFormat.Format = "0%";
            //ws.Columns(1, 2).AdjustToContents();
            #endregion
            return wb;
            //wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
            //return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
        }

        private XLWorkbook GenerateDetailVIN 
        (XLWorkbook wb, string fileName, string area, string dealer, string outlet, string yearStart, string yearEnd, string VIN, string uiostart, string uioend, string typeAktifVin)
        {
            //string fileName = "Detail VIN";
            var data = ctx.Database.SqlQuery<getAreaDealerOutlet>("exec uspfn_GetAreaDealerOutletSvr @GroupNo=@p0, @dealercode=@p1, @BranchCode=@p2", area, dealer, outlet);
            var areaName = data.FirstOrDefault().Area;
            var dealerName = data.FirstOrDefault().Dealer;
            var outletName = data.FirstOrDefault().Showroom;
            int istype = 0;
            int j = 2;
            //var wb = new XLWorkbook();

            #region Detail Vin Header
            var ws2 = wb.Worksheets.Add("Detail Vin");
            ws2.Style.Font.SetFontName("calibri");
            ws2.Style.Font.SetFontSize(9);
            ws2.ColumnWidth = 8.43;
            ws2.Column(1).Width = 21;
            ws2.Column(2).Width = 26;
            ws2.Column(3).Width = 2;
            ws2.Column(4).Width = 17;
            ws2.Column(5).Width = 17;
            ws2.Column(6).Width = 17;
            //ws2.Row(9).Height = 55;
            //ws2.Row(9).Style.Alignment.SetWrapText();
            //ws2.Row(9).Style.Font.SetFontColor(XLColor.White);

            // Setting Title

            ws2.Cell("A1").Value = "Detail VIN";
            var Titl = ws2.Range("A1:C1");
            Titl.Merge();
            Titl.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
            Titl.Style.Font.FontSize = 16;
            Titl.Style.Font.SetBold(true);

            ws2.Cell("A3").Value = "AREA";
            ws2.Cell("A4").Value = "DEALER";
            ws2.Cell("A5").Value = "OUTLET";
            ws2.Cell("A6").Value = "PRODUCTION YEAR PERIOD";
            ws2.Cell("A7").Value = "TIPE ACTIVE VIN";

            ws2.Cell("B3").Value = ": " + (area == "" ? "ALL" : areaName);
            ws2.Cell("B4").Value = ": " + (dealer == "" ? "ALL" : dealerName);
            ws2.Cell("B5").Value = ": " + (outlet == "" ? "ALL" : outletName);
            ws2.Cell("B6").Value = ": " + yearStart + " s/d " + yearEnd;
            ws2.Cell("B7").Value = ": " + (typeAktifVin == "" ? "ALL/ Revisit/ New Visit" : typeAktifVin);

            // ws2.Cell("B7").Value = ": ";

            ws2.Range(3, 1, 9, 1).Style.Font.Bold = true;
            ws2.Range("A8:B8");
            #endregion
            //var listdealer = ctx.Database.SqlQuery<DlrList>(@"exec uspfn_GetDealerOutletSvrList @GroupNo=@p0, @DealerCode=@p1, @OutletCode=@p1", area, dealer, outlet);
            var DlrName = outletName == "All" ? dealerName : outletName;
            Rows = 9;
            if (typeAktifVin == "Revisit")
            {
                istype = 1;
                j = 1;
            }
            else if (typeAktifVin == "New Visit")
            {
                istype = 2;
                j = 2;
            }
           
            for (var i = istype; i <= j; i++)
            {
                if (i == 0)
                {
                    typeAktifVin = "ALL";
                }else if (i == 1)
                {
                    typeAktifVin = "Revisit";
                }
                else 
                {
                    typeAktifVin = "New Visit";
                }
                   
                ws2.Cell("A" + Rows).Value = "Detail Active-VIN per intakes " + VIN + " : " + typeAktifVin;
                ws2.Cell("A" + Rows).Style.Font.SetBold(true);
                Rows = Rows + 1;
                prmVIN param = new prmVIN
                {
                    Area = area,
                    Dlr = dealer,
                    Otlt = outlet,
                    UIOStart = uiostart,
                    UIOEnd = uioend,
                    Vin = VIN,
                    YearStart = yearStart == "" ? "1980" : yearStart,
                    YearEnd = yearEnd,
                    InqOpt = "detail",
                    istype = i
                };
                DataTable dt = CreateTable(param, "usprpt_InqDtlVin");
                GenerateExcelDtl(wb, dt, Rows, fileName, areaName, DlrName, true);
            }
           
            return wb;
            //wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
            //return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
        }

        private XLWorkbook GenerateExcelDtl(XLWorkbook wb, DataTable dt, int lastRow, string fileName, string Region, string DealerName, bool isCustomize = false, bool isCustomHeader = false, bool isShowSummary = false)
        {
            var MRow = lastRow;

            #region Detail Vin
            var ws1 = wb.Worksheet(3); 
            var tmpLastRows = MRow;
            var LastRows = MRow;

            int iCols = 4;
            foreach (DataRow dr in dt.Rows)
            {
                iCols = 4;
                foreach (DataColumn dc in dt.Columns)
                {
                    var val = dr[dc.ColumnName];
                    Type typ = dr[dc.ColumnName].GetType();
                    switch (Type.GetTypeCode(typ))
                    {
                        case TypeCode.DateTime:
                            ws1.Cell(LastRows, iCols).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            ws1.Cell(LastRows, iCols).Style.DateFormat.Format = "dd-MMM-yyyy";
                            break;
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Double:
                        case TypeCode.Single:
                            ws1.Cell(LastRows, iCols).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws1.Cell(LastRows, iCols).Style.NumberFormat.Format = "_(* #,##0_);_(* (#,##0);_(* \" - \";_(@_)";
                            break;
                        case TypeCode.Decimal:
                            ws1.Cell(LastRows, iCols).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws1.Cell(LastRows, iCols).Style.NumberFormat.Format = "#,##0.0";
                            break;
                        case TypeCode.Boolean:
                            ws1.Cell(LastRows, iCols).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            break;
                        default:
                            if (string.IsNullOrWhiteSpace(val.ToString()) == false)
                            {
                                val = val.ToString().Substring(0, 1) == "0" ? val.ToString().Insert(0, "'") : val;
                            }
                            ws1.Cell(LastRows, iCols).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            break;
                    };

                    if (dr[dc.ColumnName].GetType() == typeof(DateTime))
                    {
                        ws1.Cell(LastRows, iCols).Style.DateFormat.Format = "dd-MMM-yyyy";
                    }

                    if (tmpLastRows == LastRows)
                    {
                        ws1.Cell(LastRows, iCols).Value = "'" + dc.ColumnName;
                        ws1.Cell(LastRows, iCols).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        //ws1.Cell(LastRows, iCols).Style.Fill.SetBackgroundColor(XLColor.Yellow).Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        //ws1.Cell(LastRows, iCols).Style.Font.SetBold().Font.SetFontSize(10);

                        ws1.Cell(LastRows + 1, iCols).Style.Font.SetFontSize(10);
                        ws1.Cell(LastRows + 1, iCols).Value = val;
                    }
                    else
                    {
                        ws1.Cell(LastRows + 1, iCols).Style.Font.SetFontSize(10);
                        ws1.Cell(LastRows + 1, iCols).Value = val;
                    }

                    iCols++;
                }

                LastRows++;
            }
            ws1.Cell(LastRows + 1, 4).Value = "Active-VIN Ratio";
            var column = 5;
            if (iCols > 26)
            {
                char alphaStart = GetChar(column - 1);
                for (char i = alphaStart; i <= 'Z'; i++)
                {
                    ws1.Cell(LastRows + 1, column).FormulaA1 = "=IFERROR(" + i + (LastRows - 1) + "/" + i + (LastRows) + ",0)";//"=" + i + (LastRows-1) + "/" + i + (LastRows);
                    column++;
                }

                char alphaEnd = GetChar((iCols - 26) - 1);
                for (char i = 'A'; i <= alphaEnd; i++)
                {
                    ws1.Cell(LastRows + 1, column).FormulaA1 = "=IFERROR(A" + i + (LastRows - 1) + "/A" + i + (LastRows) + ",0)";
                    column++;
                }
            }
            else
            {
                char alphaStart = GetChar(column - 1);
                char alphaEnd = GetChar(iCols - 2);
                for (char i = alphaStart; i <= alphaEnd; i++)
                {
                    ws1.Cell(LastRows + 1, column).FormulaA1 = "=IFERROR(" + i + (LastRows - 1) + "/" + i + (LastRows) + ",0)";
                    column++;
                }
            }
            //for (char i = 'E'; i <= 'Z'; i++)
            //{
            //    ws1.Cell(LastRows + 1, column).FormulaA1 = "=" + i + (LastRows - 1) + "/" + i + (LastRows);
            //    column++;
            //}
            ws1.Row(LastRows + 1).Style.Font.SetFontColor(XLColor.White);

            var rngTables = ws1.Range(tmpLastRows, 4, LastRows - 2, iCols - 1);
            rngTables.Style.Font.Bold = true;
            //.Border.SetTopBorder(XLBorderStyleValues.Thin)
            //.Border.SetBottomBorder(XLBorderStyleValues.Thin)
            //.Border.SetLeftBorder(XLBorderStyleValues.Thin)
            //.Border.SetRightBorder(XLBorderStyleValues.Thin);
            var Header = ws1.Range(tmpLastRows, 4, tmpLastRows, iCols - 1);
            Header.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            Header.Style.Fill.SetBackgroundColor(XLColor.FromArgb(55, 96, 145));
            Header.Style.Font.FontSize = 11;
            Header.Style.Font.SetBold(true);
            Header.Style.Font.SetFontColor(XLColor.White);
            Header.Style.Alignment.SetWrapText();

            var IsiData = ws1.Range((tmpLastRows + 1), 4, LastRows - 2, iCols - 1);
            IsiData.Style.Fill.SetBackgroundColor(XLColor.FromArgb(219, 229, 241));
            IsiData.Style.Font.FontSize = 11;
            ws1.Range(tmpLastRows + 1, 1, LastRows + 1, 1).Merge();
            ws1.Range(tmpLastRows + 1, 2, LastRows + 1, 2).Merge();
            ws1.Range(tmpLastRows + 1, 4, LastRows + 1, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            ws1.Range(tmpLastRows + 1, 4, LastRows, 4).Style.Font.SetFontColor(XLColor.White);
            ws1.Range(tmpLastRows + 1, 4, LastRows - 1, 4).Style.Fill.SetBackgroundColor(XLColor.FromArgb(55, 96, 145));
            
            //setup Total Active VIN
            ws1.Range(LastRows - 1, 4, LastRows - 1, iCols - 1).Style.NumberFormat.Format = "_(* #,##0_);_(* (#,##0);_(* \" - \";_(@_)";
            ws1.Range(LastRows - 1, 4, LastRows - 1, iCols - 1).Style.Font.Bold = true;
            ws1.Range(LastRows - 1, 4, LastRows - 1, iCols - 1).Style.Font.SetFontColor(XLColor.White);
            ws1.Range(LastRows - 1, 4, LastRows - 1, iCols - 1).Style.Fill.SetBackgroundColor(XLColor.FromArgb(55, 96, 145));
            
            //setup Total Sales unit
            ws1.Range(LastRows, 4, LastRows, iCols - 1).Style.NumberFormat.Format = "_(* #,##0_);_(* (#,##0);_(* \" - \";_(@_)";
            ws1.Range(LastRows, 4, LastRows, iCols - 1).Style.Font.Bold = true;
            ws1.Range(LastRows, 4, LastRows, iCols - 1).Style.Font.SetFontColor(XLColor.Black);
            ws1.Range(LastRows, 4, LastRows, iCols - 1).Style.Fill.SetBackgroundColor(XLColor.FromArgb(255, 192, 0));

            //setup Active VIN Ratio
            ws1.Range(LastRows + 1, 4, LastRows + 1, iCols - 1).Style.Font.Bold = true;
            ws1.Range(LastRows + 1, 4, LastRows + 1, iCols - 1).Style.Fill.SetBackgroundColor(XLColor.FromArgb(0, 176, 80));
            ws1.Range(LastRows + 1, 4, LastRows + 1, iCols - 1).Style.NumberFormat.Format = "0%";
            //Header Kiri Title
            ws1.Cell(tmpLastRows, 1).Value = "Region";
            ws1.Cell(tmpLastRows, 2).Value = "Dealer Name";
            var HeaderD = ws1.Range(tmpLastRows, 1, tmpLastRows, 2);
            HeaderD.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            HeaderD.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            HeaderD.Style.Fill.SetBackgroundColor(XLColor.FromArgb(141, 180, 227));
            HeaderD.Style.Font.FontSize = 11;
            HeaderD.Style.Font.SetBold(true);
            HeaderD.Style.Font.SetFontColor(XLColor.Black);

            //Header Kiri Isi
            ws1.Cell(tmpLastRows + 1, 1).Value = Region;
            ws1.Cell(tmpLastRows + 1, 2).Value = DealerName;
            var IsiDatas = ws1.Range((tmpLastRows + 1), 1, LastRows + 1, 2);
            IsiDatas.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            IsiDatas.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            IsiDatas.Style.Fill.SetBackgroundColor(XLColor.FromArgb(219, 229, 241));
            IsiDatas.Style.Font.FontSize = 11;
            IsiDatas.Style.Font.SetBold(true);
            IsiDatas.Style.Alignment.SetWrapText();
            
            //ws1.Columns(1, 2).AdjustToContents();
            #endregion
            Rows = LastRows + 4;
            return wb;
        }

        private XLWorkbook GenerateExcelInvalid
        (XLWorkbook wb, string fileName, string area, string dealer, string outlet, string yearStart, string yearEnd, string VIN, string uiostart, string uioend, string typeAktifVin)
        {
            //string fileName = "Invalid VIN"; 
            var data = ctx.Database.SqlQuery<getAreaDealerOutlet>("exec uspfn_GetAreaDealerOutletSvr @GroupNo=@p0, @dealercode=@p1, @BranchCode=@p2", area, dealer, outlet);
            var areaName = data.FirstOrDefault().Area;
            var dealerName = data.FirstOrDefault().Dealer;
            var outletName = data.FirstOrDefault().Showroom;

            //var wb = new XLWorkbook();

            #region Detail Vin
            var ws2 = wb.Worksheets.Add("Invalid Vin");
            ws2.Style.Font.SetFontName("calibri");
            ws2.Style.Font.SetFontSize(9);
            ws2.ColumnWidth = 8.43;
            ws2.Column(1).Width = 21;
            ws2.Column(2).Width = 26;
            ws2.Column(3).Width = 2;
            ws2.Column(4).Width = 17;
            ws2.Column(5).Width = 17;
            ws2.Column(6).Width = 17;

            // Setting Title
            ws2.Cell("A1").Value = "Invalid VIN";
            var Titl = ws2.Range("A1:C1");
            Titl.Merge();
            Titl.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
            Titl.Style.Font.FontSize = 16;
            Titl.Style.Font.SetBold(true);

            ws2.Cell("A3").Value = "AREA";
            ws2.Cell("A4").Value = "DEALER";
            ws2.Cell("A5").Value = "OUTLET";
            ws2.Cell("A6").Value = "SERVICE YEAR PERIOD";

            ws2.Cell("B3").Value = ": " + (area == "" ? "ALL" : areaName);
            ws2.Cell("B4").Value = ": " + (dealer == "" ? "ALL" : dealerName);
            ws2.Cell("B5").Value = ": " + (outlet == "" ? "ALL" : outletName);
            ws2.Cell("B6").Value = ": " + VIN;// +" s/d " + yearEnd;

            // ws2.Cell("B7").Value = ": ";

            //ws2.Range(3, 1, 8, 1).Style.Font.Bold = true;
            //ws2.Range("A8:B8");
            #endregion
            
            prmVIN param = new prmVIN
            {
                Area = area,
                Dlr = dealer,
                Otlt = outlet,
                UIOStart = uiostart,
                UIOEnd = uioend,
                Vin = VIN,
                YearStart = yearStart == "" ? "1980" : yearStart,
                YearEnd = yearEnd,
                InqOpt = "invalid",
                istype = 0
            };
            DataTable dt = CreateTable(param, "usprpt_InqInvalidVin");
            GenerateExcelInvld(wb, dt, 8, fileName, true);
            return wb;
            //wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
            //return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
        }

        private XLWorkbook GenerateExcelInvld(XLWorkbook wb, DataTable dt, int lastRow, string fileName, bool isCustomize = false, bool isCustomHeader = false, bool isShowSummary = false)
        {
            var MRow = lastRow; 

            #region Invalid Vin
            var ws1 = wb.Worksheet(4);
            var tmpLastRows = MRow;
            var LastRows = MRow;

            int iCols = 1;
            foreach (DataRow dr in dt.Rows)
            {
                iCols = 1;
                foreach (DataColumn dc in dt.Columns)
                {
                    var val = dr[dc.ColumnName];
                    Type typ = dr[dc.ColumnName].GetType();
                    switch (Type.GetTypeCode(typ))
                    {
                        case TypeCode.DateTime:
                            ws1.Cell(LastRows, iCols).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            ws1.Cell(LastRows, iCols).Style.DateFormat.Format = "dd-MMM-yyyy";
                            break;
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Double:
                        case TypeCode.Single:
                            ws1.Cell(LastRows, iCols).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws1.Cell(LastRows, iCols).Style.NumberFormat.Format = "_(* #,##0_);_(* (#,##0);_(* \" - \";_(@_)";
                            break;
                        case TypeCode.Decimal:
                            ws1.Cell(LastRows, iCols).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws1.Cell(LastRows, iCols).Style.NumberFormat.Format = "#,##0";
                            break;
                        case TypeCode.Boolean:
                            ws1.Cell(LastRows, iCols).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            break;
                        default:
                            if (string.IsNullOrWhiteSpace(val.ToString()) == false)
                            {
                                val = val.ToString().Substring(0, 1) == "0" ? val.ToString().Insert(0, "'") : val;
                            }
                            ws1.Cell(LastRows, iCols).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            break;
                    };

                    if (dr[dc.ColumnName].GetType() == typeof(DateTime))
                    {
                        ws1.Cell(LastRows, iCols).Style.DateFormat.Format = "dd-MMM-yyyy";
                    }

                    if (tmpLastRows == LastRows)
                    {
                        ws1.Cell(LastRows, iCols).Value = "'" + dc.ColumnName;
                        ws1.Cell(LastRows, iCols).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        //ws1.Cell(LastRows, iCols).Style.Fill.SetBackgroundColor(XLColor.Yellow).Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        //ws1.Cell(LastRows, iCols).Style.Font.SetBold().Font.SetFontSize(10);

                        ws1.Cell(LastRows + 1, iCols).Style.Font.SetFontSize(10);
                        ws1.Cell(LastRows + 1, iCols).Value = val;
                    }
                    else
                    {
                        ws1.Cell(LastRows + 1, iCols).Style.Font.SetFontSize(10);
                        ws1.Cell(LastRows + 1, iCols).Value = val;
                    }

                    iCols++;
                }

                LastRows++;
            }
            
            var rngTables = ws1.Range(tmpLastRows, 1, LastRows - 2, iCols - 1);
            //rngTables.Style.Font.Bold = true;
            //.Border.SetTopBorder(XLBorderStyleValues.Thin)
            //.Border.SetBottomBorder(XLBorderStyleValues.Thin)
            //.Border.SetLeftBorder(XLBorderStyleValues.Thin)
            //.Border.SetRightBorder(XLBorderStyleValues.Thin);
            var Header = ws1.Range(tmpLastRows, 1, tmpLastRows, iCols - 1);
            Header.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            Header.Style.Fill.SetBackgroundColor(XLColor.FromArgb(100, 149, 237));
            Header.Style.Font.FontSize = 11;
            Header.Style.Font.SetBold(true);
            Header.Style.Font.SetFontColor(XLColor.White);
            Header.Style.Alignment.SetWrapText();
            #endregion
            ws1.Columns().AdjustToContents(); 
            return wb;
        }

        private static char GetChar(int colnumber)
        {
            char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            return alpha[colnumber];
        }
        #endregion
    }
}