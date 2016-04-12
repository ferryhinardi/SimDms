using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GeLang;
using SimDms.Sparepart.Models;
using TracerX;
using SimDms.Sparepart.BLL;
using System.Text;
using SimDms.Common;
using ClosedXML.Excel;


namespace SimDms.Sparepart.Controllers.Api
{
    public class UtilityController : BaseController
    {
        public JsonResult IsStockTaking()
        {
            string sthdrno = StockTackingBLL.Instance(CurrentUser.UserId).GetNoStockTakingReset();
            if (!string.IsNullOrEmpty(sthdrno))
            {
                return Json(new { success = true, STHDRNO = sthdrno, message = "NOMOR SO : " + sthdrno });
            }
            else
            {
                return Json(new { success = false, message = "Notes : Tidak ada Proses Stock Taking" });
            }
        }

        public JsonResult ResetStockTaking(string sthdrno)
        {

            var sql = string.Format("exec uspfn_ResetStockOpname '{0}','{1}','{2}','{3}'", CompanyCode, BranchCode, sthdrno, CurrentUser.UserId);
            //var queryable = ctx.Context.Database.SqlQuery<sp_SpSOSelectforLookup>(sql).AsQueryable();
            try
            {
                ctx.Database.ExecuteSqlCommand(sql);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Reset stock opname gagal! \n" + ex.Message });
            }

            return Json(new { success = true, message = "Sukses reset stock opname !" });

        }

        public JsonResult ChangePartType(string partype)
        {

            var x = ctx.LookUpDtls.Find(CompanyCode, GnMstLookUpHdr.TypeOfGoods, partype);
            if (x != null)
            {
                CurrentUser.TypeOfGoods = partype;
                try
                {
                    ctx.SaveChanges();
                    return Json(new { success = true, data = x, message = "Perubahan Type Part Berhasil" });
                }
                catch
                {
                    return Json(new { success = false, message = "Perubahan Type Part Gagal" });
                }

            }
            else
            {
                return Json(new { success = false, message = "Invalid Type Part" });
            }

        }

        public JsonResult ExportAosItemSpList(string typeOfGoods)
        {
            DataTable dt = new DataTable();
            var user = CurrentUser;

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_spAutomaticOrderSparepartList";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 3600;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", user.CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", user.BranchCode);
            cmd.Parameters.AddWithValue("@TypeOfGoods", typeOfGoods);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            List<dynamic> forHeader = new List<dynamic>();
            forHeader.Add("Generate Aos Item Sparepart List");
            forHeader.Add("Dealer Code : " + user.CompanyCode);
            forHeader.Add("Branch Code : " + user.BranchCode + " - " + user.CoProfile.CompanyGovName);
            forHeader.Add("Report Date : " + DateTime.Now.ToString("dd-MM-yyyy HH:mm"));

            if (dt.Columns .Count > 0)
            {
                return GenerateReportExcel(dt, "ExportAosItemSpList", "ExportAosItemSpList", forHeader);
            }
            return Json(new { success = false, message = "" });
        }

        protected JsonResult GenerateReportExcel(DataTable dt, string sheetName, string fileName, List<dynamic> header)
        {
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add(sheetName);
            var seqno = 1;
            fileName = fileName + "_" + DateTime.Now.ToString("yyyy_MMdd_HHmmss") + ".xlsx";
            int clm = dt.Columns.Count;

            // add header information
            if (header != null)
            {
                //seqno++;
                for (int i = 0; i < header.Count; i++)
                {
                    int o = 0;
                    var caption = header[i];
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(o + 1), seqno)).Value = caption;
                    ws.Range(string.Format("A{0}:{1}{0}", seqno, GetExcelColumnName(o + clm))).Merge()
                        .Style.Font.SetBold();
                    seqno++;
                }
                seqno++;
            }

            // set caption
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Value = dt.Columns[i].Caption;
                ws.Range(string.Format("A{0}:{1}{0}", seqno, GetExcelColumnName(i + 1)))
                    .Style.Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Fill.BackgroundColor = XLColor.Cobalt;
                ws.Range(string.Format("A{0}:{1}{0}", seqno, GetExcelColumnName(i + 1)))
                    .Style.Font.FontColor = XLColor.White;

                //set border
                ws.Range(string.Format("A{0}:{1}{0}", seqno, GetExcelColumnName(i + 1)))
                .Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Range(string.Format("A{0}:{1}{0}", seqno, GetExcelColumnName(i + 1)))
                    .Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Range(string.Format("A{0}:{1}{0}", seqno, GetExcelColumnName(i + 1)))
                    .Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range(string.Format("A{0}:{1}{0}", seqno, GetExcelColumnName(i + 1)))
                    .Style.Border.RightBorder = XLBorderStyleValues.Thin;
            }

            seqno++;
            // set cell value
            foreach (DataRow row in dt.Rows)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    var caption = dt.Columns[i].Caption;
                    // start set width columns
                    int length = 0;
                    length = row[caption].ToString().Length;
                    string order = (i + 1).ToString();
                    if (length > 10 && length < 21)
                    {
                        ws.Columns(order).Width = 30;
                    }
                    else if (length > 20)
                    {
                        ws.Columns(order).Width = 50;
                    }
                    else
                    {
                        ws.Columns(order).Width = dt.Columns[i].Caption.Length + 5;
                    }
                    // end                    

                    if (row[caption].GetType().Name == "String")
                    {
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Value = "'" + row[caption];
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno))
                            .Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno))
                            .Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno))
                            .Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno))
                            .Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    }
                    else if (row[caption].GetType().Name == "Decimal")
                    {
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Value = row[caption];
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno))
                           .Style.NumberFormat.Format = "#,##0.00";

                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno))
                            .Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno))
                            .Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno))
                            .Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno))
                            .Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    }
                    else
                    {
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Value = row[caption];
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno))
                            .Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno))
                            .Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno))
                            .Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno))
                            .Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    }
                }
                seqno++;
            }

            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName));

            return Json(new
            {
                rows = dt.Rows.Count,
                cols = dt.Columns.Count,
                range = string.Format("A{0}:{1}{0}", seqno, GetExcelColumnName(dt.Columns.Count)),
                fileUrl = Url.Content("~/ReportTemp/" + fileName),
                header = header
            });
        }

    }
}
