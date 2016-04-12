using SimDms.Tax;
using SimDms.Tax.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Common.Models;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using System.Diagnostics;
using ClosedXML.Excel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;


namespace SimDms.Tax.Controllers.Api
{
    public class ReportController : BaseController

    {
        public JsonResult PrintBukuPembelian()
        {
            var isBranch = ctx.OrganizationDtls.FirstOrDefault(f => f.CompanyCode == CompanyCode && f.BranchCode == BranchCode).IsBranch;
            var isMultiBranch = (ctx.OrganizationDtls.Count() > 1);
            var query = string.Format(@"
                select a.isFPJCentralized
                from 
	            gnMstCoProfile a
	            inner join gnMstOrganizationDtl b on a.CompanyCode=b.CompanyCode and a.BranchCode=b.BranchCode and b.IsBranch=0
                where a.CompanyCode='{0}'"
                , CompanyCode);

            var isFpjCentral = ctx.Database.SqlQuery<CoProfileView>(query).FirstOrDefault();
            return Json(new { success = true, isBranch = isBranch, isMultiBranch = isMultiBranch, isFpjCentral = isFpjCentral });
        }

        public JsonResult SetBranchInterface()
        {
            var isBranch = ctx.OrganizationDtls.FirstOrDefault(f => f.CompanyCode == CompanyCode && f.BranchCode == BranchCode).IsBranch;
            var isMultiBranch = (ctx.OrganizationDtls.Count() > 1);
            var query = string.Format(@"
                select a.isFPJCentralized
                from 
	            gnMstCoProfile a
	            inner join gnMstOrganizationDtl b on a.CompanyCode=b.CompanyCode and a.BranchCode=b.BranchCode and b.IsBranch=0
                where a.CompanyCode='{0}'"
                , CompanyCode);
            var isFpjCentral = ctx.Database.SqlQuery<CoProfileView>(query).FirstOrDefault().IsFPJCentralized;
            var query2 = string.Format(@"
                select ParaValue
                from gnMstLookUpDtl
                where CompanyCode='{0}'
                    and CodeID='CPPN'
                    and LookUpValue='STATUS'"
                , CompanyCode);
            var CekAllowedBranchCode = ctx.Database.SqlQuery<MstLookUpDtlView>(query2).FirstOrDefault().ParaValue;
            var Branch = ctx.OrganizationDtls.Find(CompanyCode, BranchCode);

            return Json(new { success = true, isBranch = isBranch, isMultiBranch = isMultiBranch, isFpjCentral = isFpjCentral, Branch = Branch, CekAllowedBranchCode = CekAllowedBranchCode });
        }

        public ActionResult UploadDataB(string CompanyCode, string BranchCode, string Year, string Month)
        {
            //ExcelFileWriter excelReport;
            string fileName = "";
            fileName = "Tabel_";
            fileName = fileName + DateTime.Now.ToString("yyyyMMdd");

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 1800;
            cmd.CommandText = @"
                SELECT
                    TaxCode KDFP
	                , TransactionCode KDLAM
	                , StatusCode KDSTS
	                , DocumentCode KDDOK
	                , REPLACE(REPLACE(NPWP, '.', ''), '-', '') NPWP
	                , SupplierName NAMA
	                , '0' + RIGHT(BranchCode, 2) KDCAB
	                , RIGHT(YEAR(TaxDate), 2) KDTHN
	                , RIGHT(TaxNo, 8) NOFP
	                , CONVERT(VARCHAR, TaxDate, 103) KDTHN
	                , NULL TGLSP
	                , PeriodMonth BLFP
                    , PeriodYear THNFP
	                , 0 KOR
	                , ISNULL(DPPAmt, 0) DPPAM
	                , ISNULL(PPNAmt, 0) PPNAM
	                , ISNULL(PPNBmAmt, 0) PPNBM
                FROM 
	                gnTaxIn a WITH(NOLOCK, NOWAIT)
                WHERE
	                CompanyCode = @CompanyCode
	                AND BranchCode = @BranchCode
	                AND ProductType = @ProductType
	                AND PeriodYear = @PeriodYear
	                AND PeriodMonth = @PeriodMonth
                    AND IsPKP = 1
                ORDER BY 
                    TaxNo";

            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@ProductType", ProductType);
            cmd.Parameters.AddWithValue("@PeriodYear", Year);
            cmd.Parameters.AddWithValue("@PeriodMonth", Month);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("datTable1");
            da.Fill(dt);

            if (dt.Rows.Count == 0)
            {
                return Json("Tidak ada data yang ditampilkan", "text/html", JsonRequestBehavior.AllowGet);
            }
            else
            {
                int lastRow = 1;

                var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("Laporan Pajak Masukan");

                return GenerateExcel(wb, dt, lastRow, fileName);
            }
        }

        public ActionResult UploadDataA(string CompanyCode, string BranchCode1, string Year, string Month)
        {
            var Branch = "";
            if (BranchCode1 == "")
            { Branch = "%"; }
            else { Branch = BranchCode; }

            //ExcelFileWriter excelReport;
            string fileName = "";
            fileName = "Tabel_";
            fileName = fileName + DateTime.Now.ToString("yyyyMMdd");

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 1800;
            cmd.CommandText = @"
                    SELECT KDFP , KDLAM , KDSTS , KDDOK , NPWP , NAMA , KDCAB , KDTHN , NOFP , TGLFP , TGLSP , BLFP , THNFP , KOR , SUM(DPPAM) DPPAM 
                    , SUM(PPNAM) PPNAM , SUM(PPNBM) PPNBM 
                FROM (
	                SELECT
		                TaxCode KDFP , TransactionCode KDLAM , StatusCode KDSTS, DocumentCode KDDOK, REPLACE(REPLACE(NPWP, '.', ''), '-', '') NPWP , CustomerName NAMA 
                        , '0' + RIGHT(BranchCode, 2) KDCAB , RIGHT(YEAR(TaxDate), 2) KDTHN , RIGHT(TaxNo, 8) NOFP , CONVERT(VARCHAR, TaxDate, 103) TGLFP
		                , NULL TGLSP , PeriodMonth BLFP , PeriodYear THNFP , 0 KOR , ISNULL(DPPAmt, 0) DPPAM , ISNULL(PPNAmt, 0) PPNAM , ISNULL(PPNBmAmt, 0) PPNBM
	                FROM 
		                gnTaxOut
	                WHERE
		                CompanyCode = @CompanyCode
		                AND BranchCode like @BranchCode
		                AND ProductType = @ProductType
		                AND PeriodYear = @PeriodYear
		                AND PeriodMonth = @PeriodMonth
		                AND IsPKP = 1
                ) a
                GROUP BY KDFP , KDLAM , KDSTS , KDDOK , NPWP , NAMA , KDCAB , KDTHN , NOFP , TGLFP , TGLSP , BLFP , THNFP , KOR
                ORDER BY NOFP";

            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", Branch);
            cmd.Parameters.AddWithValue("@ProductType", ProductType);
            cmd.Parameters.AddWithValue("@PeriodYear", Year);
            cmd.Parameters.AddWithValue("@PeriodMonth", Month);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("datTable1");
            da.Fill(dt);

            if (dt.Rows.Count == 0)
            {
                return Json("Tidak ada data yang ditampilkan", "text/html", JsonRequestBehavior.AllowGet);
            }
            else
            {
                int lastRow = 1;

                var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("Laporan Pajak Masukan");

                return GenerateExcel(wb, dt, lastRow, fileName);
            }
        }

        public JsonResult GetData4CSVFileNew(string CompanyCode, string BranchCode, string Year, string Month)
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 1800;
            cmd.CommandText = @"
                SELECT
                    TaxCode KodePajak
	                , TransactionCode KodeTransaksi
	                , StatusCode KodeStatus
	                , DocumentCode KodeDokumen
	                , '0' FlagVAT
	                , REPLACE(REPLACE(NPWP, '.', ''), '-', '') NPWP_NomorPaspor
	                , SupplierName NamaLawanTransaksi
	                , TaxNo NomorFaktur_Dokumen
	                , '1' JenisDokumen
	                , '' NomorFakturPengganti_Retur
	                , '' JenisDokumenDokumenPengganti_Retur
	                , CONVERT(VARCHAR, TaxDate, 103) TanggalFaktur_Dokumen
	                , '' TanggalSSP
	                , case when len(PeriodMonth) = 1 then '0' + convert(varchar, PeriodMonth, 1) + '0' +  convert(varchar, PeriodMonth, 1) 
	                    else convert(varchar, PeriodMonth, 2)+convert(varchar, PeriodMonth, 2) end MasaPajak
                    , PeriodYear TahunPajak
	                , 0 Pembetulan
	                , ISNULL(DPPAmt, 0) DPP
	                , ISNULL(PPNAmt, 0) PPN
	                , ISNULL(PPNBmAmt, 0) PPnBM
                FROM 
	                gnTaxIn a WITH(NOLOCK, NOWAIT)
                WHERE
	                CompanyCode = @CompanyCode
	                AND BranchCode = @BranchCode
	                AND ProductType = @ProductType
	                AND PeriodYear = @PeriodYear
	                AND PeriodMonth = @PeriodMonth
                    AND IsPKP = 1
                ORDER BY 
                    TaxNo";

            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@ProductType", ProductType);
            cmd.Parameters.AddWithValue("@PeriodYear", Year);
            cmd.Parameters.AddWithValue("@PeriodMonth", Month);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("datTable1");
            da.Fill(dt);
            var rowCount = dt.AsEnumerable().Count();
            if (dt.Rows.Count == 0)
            {
                return Json(new
                {
                    success = false,
                    message = "Data tidak ada!",
                    rowCount = rowCount
                });
            }
            else
            {
                List<string> lines = new List<string>();
                string line = string.Empty;

                // Add header into csv file
                foreach (DataColumn column in dt.Columns)
                    line += string.Format(";{0}", column.ColumnName);

                lines.Add((line.Length > 0) ? line.Substring(1) : string.Empty);

                // Add detail into csv file
                foreach (DataRow row in dt.Rows)
                {
                    line = string.Empty;
                    for (int i = 0; i < dt.Columns.Count - 2; i++)
                    {
                        object temp = (row[i] is string) ? (string)row[i] : row[i];
                        line += string.Format(";{0}", temp);
                    }
                    lines.Add((line.Length > 0) ? line.Substring(1) : string.Empty);
                }
                
                bool msg = ShowCsvReport(lines.ToArray(), "csvTaxIn_" + DateTime.Now.ToString("yyyMMdd"));
                return Json(new
                {
                    success = true,
                    message = "Download data berhasil",
                    rowCount = rowCount
                });
            }
        }

        public ActionResult GetData4XLSFileNew(string CompanyCode, string BranchCode, string Year, string Month)
        {
            //ExcelFileWriter excelReport;
            string fileName = "";
            fileName = "xlsTaxIn_";
            fileName = fileName + DateTime.Now.ToString("yyyyMMdd");

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 1800;
            cmd.CommandText = @"
                SELECT
                    TaxCode KodePajak
	                , TransactionCode KodeTransaksi
	                , StatusCode KodeStatus
	                , DocumentCode KodeDokumen
	                , '0' FlagVAT
	                , REPLACE(REPLACE(NPWP, '.', ''), '-', '') NPWP_NomorPaspor
	                , SupplierName NamaLawanTransaksi
	                , TaxNo NomorFaktur_Dokumen
	                , '1' JenisDokumen
	                , '' NomorFakturPengganti_Retur
	                , '' JenisDokumenDokumenPengganti_Retur
	                , CONVERT(VARCHAR, TaxDate, 103) TanggalFaktur_Dokumen
	                , '' TanggalSSP
	                , case when len(PeriodMonth) = 1 then '0' + convert(varchar, PeriodMonth, 1) + '0' +  convert(varchar, PeriodMonth, 1) 
	                    else convert(varchar, PeriodMonth, 2)+convert(varchar, PeriodMonth, 2) end MasaPajak
                    , PeriodYear TahunPajak
	                , 0 Pembetulan
	                , ISNULL(DPPAmt, 0) DPP
	                , ISNULL(PPNAmt, 0) PPN
	                , ISNULL(PPNBmAmt, 0) PPnBM
                FROM 
	                gnTaxIn a WITH(NOLOCK, NOWAIT)
                WHERE
	                CompanyCode = @CompanyCode
	                AND BranchCode = @BranchCode
	                AND ProductType = @ProductType
	                AND PeriodYear = @PeriodYear
	                AND PeriodMonth = @PeriodMonth
                    AND IsPKP = 1
                ORDER BY 
                    TaxNo";

            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@ProductType", ProductType);
            cmd.Parameters.AddWithValue("@PeriodYear", Year);
            cmd.Parameters.AddWithValue("@PeriodMonth", Month);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("datTable1");
            da.Fill(dt);
            if (dt.Rows.Count == 0)
            {
                return Json("Tidak ada data yang ditampilkan", "text/html", JsonRequestBehavior.AllowGet);
            }
            else
            {
                int lastRow = 1;

                var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("Laporan Pajak Masukan");

                return GenerateExcel(wb, dt, lastRow, fileName);
            }
        }

        public JsonResult GetData4CSVFileNewOut(string CompanyCode, string BranchCode1, string Year, string Month)
        {
            var Branch = "";
            if (BranchCode1 == "")
            { Branch = "%"; }
            else { Branch = BranchCode; }

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 1800;
            cmd.CommandText = "usprpt_GnGenerateCsvTax";

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", Branch);
            cmd.Parameters.AddWithValue("@PeriodMonth", Month);
            cmd.Parameters.AddWithValue("@PeriodYear", Year);
            cmd.Parameters.AddWithValue("@ProductType", ProductType);
            
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("datTable1");
            da.Fill(dt);
            var rowCount = dt.AsEnumerable().Count();
            if (dt.Rows.Count > 0)
            {
                List<string> lines = new List<string>();
                string line = string.Empty;

                // Add header into csv file
                foreach (DataColumn column in dt.Columns)
                    line += string.Format(";{0}", column.ColumnName);

                lines.Add((line.Length > 0) ? line.Substring(1) : string.Empty);

                // Add detail into csv file
                foreach (DataRow row in dt.Rows)
                {
                    line = string.Empty;
                    for (int i = 0; i < dt.Columns.Count - 2; i++)
                    {
                        object temp = (row[i] is string) ? (string)row[i] : row[i];
                        line += string.Format(";{0}", temp);
                    }
                    lines.Add((line.Length > 0) ? line.Substring(1) : string.Empty);
                }

                bool msg = ShowCsvReport(lines.ToArray(), "csvTaxOut_" + DateTime.Now.ToString("yyyMMdd"));

                return Json(new
                {
                    success = true,
                    message = "Download data berhasil",
                    rowCount = rowCount
                });
            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = "Data tidak ada!",
                    rowCount = rowCount
                });
            }
           
        }

        public ActionResult GetData4XLSFileNewOut(string CompanyCode, string BranchCode1, string Year, string Month) 
        {
            var Branch = "";
            if (BranchCode1 == "")
            { Branch = "%"; }
            else { Branch = BranchCode; }

            //ExcelFileWriter excelReport;
            string fileName = "";
            fileName = "Tabel_";
            fileName = fileName + DateTime.Now.ToString("yyyyMMdd");

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 1800;
            cmd.CommandText = "usprpt_GnGenerateCsvTax";

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", Branch);
            cmd.Parameters.AddWithValue("@PeriodMonth", Month);
            cmd.Parameters.AddWithValue("@PeriodYear", Year);
            cmd.Parameters.AddWithValue("@ProductType", ProductType);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("datTable1");
            da.Fill(dt);
            if (dt.Rows.Count == 0)
            {
                return Json("Tidak ada data yang ditampilkan", "text/html", JsonRequestBehavior.AllowGet);
            }
            else
            {
                int lastRow = 1;

                var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("Laporan Pajak Keluaran");

                return GenerateExcel(wb, dt, lastRow, fileName);
            }

        }

        private ActionResult GenerateExcel(XLWorkbook wb, DataTable dt, int lastRow, string fileName, bool isCustomHeader = false, bool isShowSummary = false)
        {
            var ws = wb.Worksheet(1);
            var tmpLastRow = lastRow;

            int iCol = 1;
            char iChar = 'A';
            foreach (DataRow dr in dt.Rows)
            {
                iCol = 1;
                iChar = 'A';
                foreach (DataColumn dc in dt.Columns)
                {
                    var val = dr[dc.ColumnName];
                    Type typ = dr[dc.ColumnName].GetType();
                    switch (Type.GetTypeCode(typ))
                    {
                        case TypeCode.DateTime:
                            ws.Cell(lastRow + 1, iCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            ws.Cell(lastRow + 1, iCol).Style.DateFormat.Format = "dd-MMM-yyyy";
                            break;
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Double:
                        case TypeCode.Single:
                            ws.Cell(lastRow + 1, iCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(lastRow + 1, iCol).Style.NumberFormat.Format = "#,##0";
                            break;
                        case TypeCode.Decimal:
                            ws.Cell(lastRow + 1, iCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(lastRow + 1, iCol).Style.NumberFormat.Format = "#,##0.0";
                            break;
                        case TypeCode.Boolean:
                            ws.Cell(lastRow + 1, iCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            break;
                        default:
                            if (string.IsNullOrWhiteSpace(val.ToString()) == false)
                            {
                                val = val.ToString().Substring(0, 1) == "0" ? val.ToString().Insert(0, "'") : val;
                            }
                            ws.Cell(lastRow + 1, iCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            break;
                    };

                    if (dr[dc.ColumnName].GetType() == typeof(DateTime))
                    {
                        ws.Cell(lastRow + 1, iCol).Style.DateFormat.Format = "dd-MMM-yyyy";
                    }

                    if (tmpLastRow == lastRow)
                    {
                        ws.Cell(lastRow, iCol).Value = dc.ColumnName;
                        // ws.Cell(lastRow, iCol).Style.Fill.SetBackgroundColor(XLColor.TeaRoseRose).Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell(lastRow, iCol).Style.Font.SetBold().Font.SetFontSize(10);

                        ws.Cell(lastRow + 1, iCol).Style.Font.SetFontSize(10);
                        ws.Cell(lastRow + 1, iCol).Value = val;
                    }
                    else
                    {
                        ws.Cell(lastRow + 1, iCol).Style.Font.SetFontSize(10);
                        ws.Cell(lastRow + 1, iCol).Value = val;
                    }

                    iCol++;
                    iChar++;
                }

                lastRow++;
            }

            if (isShowSummary)
            {
                ws.Cell(lastRow + 1, 1).Value = "TOTAL";
                int j = 2;
                for (char i = 'B'; i < iChar; i++)
                {
                    ws.Cell(lastRow + 1, j).FormulaA1 = "=SUM(" + i + (tmpLastRow + 1) + ":" + i + lastRow + ")";
                    j++;
                }
            }

            var rngTable = ws.Range(tmpLastRow + 1, 1, lastRow + 1, iCol - 1);
            rngTable.Style
                .Border.SetTopBorder(XLBorderStyleValues.Thin)
                .Border.SetBottomBorder(XLBorderStyleValues.Thin)
                .Border.SetLeftBorder(XLBorderStyleValues.Thin)
                .Border.SetRightBorder(XLBorderStyleValues.Thin);


            ws.Columns().Style.Alignment.SetVertical(XLAlignmentVerticalValues.Top);
            ws.Columns().Style.Alignment.SetWrapText();
            ws.Columns().AdjustToContents();

            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));

            return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
        }

        public JsonResult GetData4CSVFileSkemaTaxOutold(string CompanyCode, string BranchCode1, string Year, string Month, string table2)
        {
            var Branch = "";
            
            if (BranchCode1 == "") 
            { Branch = "%"; }
            else { Branch = BranchCode; }

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 1800;
            cmd.CommandText = "usprpt_GnGenerateCsvSkemaTaxOut";

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", Branch);
            cmd.Parameters.AddWithValue("@PeriodMonth", Month);
            cmd.Parameters.AddWithValue("@PeriodYear", Year);
            cmd.Parameters.AddWithValue("@ProductType", "4W");

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("datTable1");
            da.Fill(dt);
            var rowCount = dt.AsEnumerable().Count();
            
            SqlCommand cmd2 = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd2.CommandTimeout = 1800;
            //cmd2.CommandText = "SELECT '' LT, '' NPWP, '' NAMA, '' JALAN, '' BLOK, '' NOMOR, '' RT, '' RW, '' KECAMATAN, '' KELURAHAN, ''KABUPATEN, '' PROPINSI, ''KODE_POS, '' NOMOR_TELEPON";
            cmd2.CommandText = "usprpt_GnGenerateCsvSkemaTaxOut";
            cmd2.CommandType = CommandType.StoredProcedure;
            cmd2.Parameters.Clear();
            cmd2.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd2.Parameters.AddWithValue("@BranchCode", Branch);
            cmd2.Parameters.AddWithValue("@PeriodMonth", Month);
            cmd2.Parameters.AddWithValue("@PeriodYear", Year);
            cmd2.Parameters.AddWithValue("@ProductType", "4W");
            cmd2.Parameters.AddWithValue("@table", "2");
            SqlDataAdapter da2 = new SqlDataAdapter(cmd2);
            DataTable dt2 = new DataTable("datTable2");
            da2.Fill(dt2);

            SqlCommand cmd3 = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd3.CommandTimeout = 1800;
            cmd3.CommandText = "usprpt_GnGenerateCsvSkemaTaxOut";
            cmd3.CommandType = CommandType.StoredProcedure;
            cmd3.Parameters.Clear();
            cmd3.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd3.Parameters.AddWithValue("@BranchCode", Branch);
            cmd3.Parameters.AddWithValue("@PeriodMonth", Month);
            cmd3.Parameters.AddWithValue("@PeriodYear", Year);
            cmd3.Parameters.AddWithValue("@ProductType", "4W");
            cmd3.Parameters.AddWithValue("@table", "3");
            SqlDataAdapter da3 = new SqlDataAdapter(cmd3);
            DataTable dt3 = new DataTable("datTable3");
            da3.Fill(dt3);

            if (dt.Rows.Count > 0)
            {
                List<string> lines = new List<string>();
                string line = string.Empty;
                string line2 = string.Empty;
                string line3 = string.Empty;  

                // Add header into csv file
                foreach (DataColumn column in dt.Columns)
                    line += string.Format(";{0}", column.ColumnName);

                lines.Add((line.Length > 0) ? line.Substring(1) : string.Empty);
                if (table2 == "0")
                {
                    line = string.Empty;
                    foreach (DataColumn column2 in dt2.Columns)
                        line += string.Format(";{0}", column2.ColumnName);

                    lines.Add((line.Length > 0) ? line.Substring(1) : string.Empty);
                }
                
                line = string.Empty;
                foreach (DataColumn column3 in dt3.Columns)
                    line += string.Format(";{0}", column3.ColumnName);

                lines.Add((line.Length > 0) ? line.Substring(1) : string.Empty);
                // Add detail into csv file
                foreach (DataRow row in dt.Rows)
                {
                    line = string.Empty;
                    for (int i = 0; i < dt.Columns.Count-2; i++)
                    {
                        //line.Replace("\",\"", ";")
                        object temp = (row[i] is string) ? (string)row[i].ToString().Replace("'", "") : row[i];
                        line += string.Format(";{0}", temp);
                    }
                    lines.Add((line.Length > 0) ? line.Substring(1) : string.Empty);
                    var CusNamedt = row[19];
                    if (table2 == "0")
                    {
                        line2 = string.Empty;
                        DataRow[] foundRows = dt2.Select("CustomerCode='" + CusNamedt + "'");
                        if (foundRows.Count() > 0)
                        {
                            var row1 = foundRows[0].ItemArray;
                            for (int j = 0; j < row1.Length - 1; j++)
                            {
                                object temp2 = (row1[j] is string) ? (string)row1[j] : row1[j];
                                line2 += string.Format(";{0}", temp2);
                            }
                            lines.Add((line2.Length > 0) ? line2.Substring(1) : string.Empty);
                        }
                    }
                        
                        line3 = string.Empty;
                        var FPJNO = row[20];
                        DataRow[] foundRows2 = dt3.Select("CustomerCode='" + CusNamedt + "' AND FPJNO='" + FPJNO + "'");
                        if (foundRows2.Count() > 0)
                        {
                            for (int k = 0; k < foundRows2.Length ; k++)  
                            {
                                var row3 = foundRows2[k].ItemArray;
                                for (int h = 0; h < row3.Length - 2; h++)
                                {
                                    object temp3 = (row3[h] is string) ? (string)row3[h] : row3[h];
                                    line3 += string.Format(";{0}", temp3);
                                }
                                lines.Add((line3.Length > 0) ? line3.Substring(1) : string.Empty);
                            }
                        }
                    //}
                }
                bool msg = ShowCsvReport(lines.ToArray(), "csvSkemaTaxOut_" + DateTime.Now.ToString("yyyMMdd"));
                return Json(new
                {
                    success = true,
                    message = "Download data berhasil",
                    rowCount = rowCount
                });
            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = "Data Tidak Ada",
                    rowCount = rowCount
                });
            }
        }

        public JsonResult GetData4CSVFileSkemaTaxOut(string CompanyCode, string BranchFrom, string BranchTo, string profitCenterCode, string DateFrom, string DateTo, string table2, string separate, string delimeter)
        {
            var Branch = "";
            bool delimeted = delimeter == "1" ? true : false;
            //if (BranchCode1 == "")
            //{ Branch = "%"; }
            //else { Branch = BranchCode; }

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 7200;
            cmd.CommandText = "usprpt_GnGenerateCsvSkemaTaxOut";

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCodeFrom", BranchFrom);
            cmd.Parameters.AddWithValue("@BranchCodeTo", BranchTo);
            cmd.Parameters.AddWithValue("@ProfitCenterCode", profitCenterCode);
            cmd.Parameters.AddWithValue("@PeriodFrom", DateFrom);
            cmd.Parameters.AddWithValue("@PeriodTo", DateTo);
            cmd.Parameters.AddWithValue("@ProductType", ProductType);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("datTable1");
            da.Fill(dt);
            var rowCount = dt.AsEnumerable().Count();

            SqlCommand cmd2 = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd2.CommandTimeout = 7200;
            //cmd2.CommandText = "SELECT '' LT, '' NPWP, '' NAMA, '' JALAN, '' BLOK, '' NOMOR, '' RT, '' RW, '' KECAMATAN, '' KELURAHAN, ''KABUPATEN, '' PROPINSI, ''KODE_POS, '' NOMOR_TELEPON";
            cmd2.CommandText = "usprpt_GnGenerateCsvSkemaTaxOut";
            cmd2.CommandType = CommandType.StoredProcedure;
            cmd2.Parameters.Clear();
            cmd2.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd2.Parameters.AddWithValue("@BranchCodeFrom", BranchFrom);
            cmd2.Parameters.AddWithValue("@BranchCodeTo", BranchTo);
            cmd2.Parameters.AddWithValue("@ProfitCenterCode", profitCenterCode);
            cmd2.Parameters.AddWithValue("@PeriodFrom", DateFrom);
            cmd2.Parameters.AddWithValue("@PeriodTo", DateTo);
            cmd2.Parameters.AddWithValue("@ProductType", ProductType);
            cmd2.Parameters.AddWithValue("@table", "2");
            SqlDataAdapter da2 = new SqlDataAdapter(cmd2);
            DataTable dt2 = new DataTable("datTable2");
            da2.Fill(dt2);

            SqlCommand cmd3 = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd3.CommandTimeout = 7200;
            cmd3.CommandText = "usprpt_GnGenerateCsvSkemaTaxOut";
            cmd3.CommandType = CommandType.StoredProcedure;
            cmd3.Parameters.Clear();
            cmd3.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd3.Parameters.AddWithValue("@BranchCodeFrom", BranchFrom);
            cmd3.Parameters.AddWithValue("@BranchCodeTo", BranchTo);
            cmd3.Parameters.AddWithValue("@ProfitCenterCode", profitCenterCode);
            cmd3.Parameters.AddWithValue("@PeriodFrom", DateFrom);
            cmd3.Parameters.AddWithValue("@PeriodTo", DateTo);
            cmd3.Parameters.AddWithValue("@ProductType", ProductType);
            cmd3.Parameters.AddWithValue("@table", "3");
            SqlDataAdapter da3 = new SqlDataAdapter(cmd3);
            DataTable dt3 = new DataTable("datTable3");
            da3.Fill(dt3);

            if (dt.Rows.Count > 0)
            {
                List<string> lines = new List<string>();
                var sb = new StringBuilder(); 
                string line = string.Empty;
                string line2 = string.Empty;
                string line3 = string.Empty;

                // Add header into csv file
                foreach (DataColumn column in dt.Columns) {
                    if (column.ColumnName != "CUSTOMERCODE")
                    //{
                        //if (column.ColumnName == "FK")
                        //    //line += string.Format(@"""{0}""", column.ColumnName);
                        //    line += "\"" + "FK" + "\"";
                            
                        //else
                        if (delimeted)
                            line += string.Format(separate + "\"{0}\"", column.ColumnName);
                        else line += string.Format(separate + "{0}", column.ColumnName);
                    //}
                }
                //lines.Add((line.Length > 0) ? line.Substring(1) : string.Empty);
                sb.AppendLine((line.Length > 0) ? line.Substring(1) : string.Empty);

                //if (table2 == "0")
                //{
                    line = string.Empty;
                    foreach (DataColumn column2 in dt2.Columns)
                    {
                        if (column2.ColumnName != "CUSTOMERCODE")
                            if (delimeted)
                                line += string.Format(separate + "\"{0}\"", column2.ColumnName);
                            else line += string.Format(separate + "{0}", column2.ColumnName);
                    }
                    //lines.Add((line.Length > 0) ? line.Substring(1) : string.Empty);
                    sb.AppendLine((line.Length > 0) ? line.Substring(1) : string.Empty);
                //}

                line = string.Empty;
                foreach (DataColumn column3 in dt3.Columns) {
                    if (column3.ColumnName != "CUSTOMERCODE" && column3.ColumnName != "NOMOR_FAKTUR" && column3.ColumnName != "FPJNo")
                        if (delimeted)
                            line += string.Format(separate + "\"{0}\"", column3.ColumnName);
                        else line += string.Format(separate + "{0}", column3.ColumnName);
                }
                //lines.Add((line.Length > 0) ? line.Substring(1) : string.Empty);
                sb.AppendLine((line.Length > 0) ? line.Substring(1) : string.Empty);
                // Add detail into csv file
                foreach (DataRow row in dt.Rows)
                {
                    line = string.Empty;
                    for (int i = 0; i < dt.Columns.Count - 1; i++)
                    {
                        //line.Replace("\",\"", ";")
                        object temp = (row[i] is string) ? (string)row[i] : row[i];
                        if (delimeted)
                            line += string.Format(separate + "\"{0}\"", temp);
                        else line += string.Format(separate + "{0}", temp);
                    }
                    //lines.Add((line.Length > 0) ? line.Substring(1) : string.Empty);
                    sb.AppendLine((line.Length > 0) ? line.Substring(1) : string.Empty);
                    var CusNamedt = row[19];
                    DataRow[] foundRows = dt2.Select("CustomerCode='" + CusNamedt + "'");
                        line2 = string.Empty;
                        if (table2 == "1")
                        {
                            foundRows = dt2.Select("CustomerCode='" + CusNamedt + "' and NPWP<>'000000000000000'");
                        }
                        
                        if (foundRows.Count() > 0)
                        {
                            var row1 = foundRows[0].ItemArray;
                            for (int j = 0; j < row1.Length - 1; j++)
                            {
                                object temp2 = (row1[j] is string) ? (string)row1[j] : row1[j];
                                if (delimeted)
                                    line2 += string.Format(separate + "\"{0}\"", temp2);
                                else line2 += string.Format(separate + "{0}", temp2);
                            }
                            //lines.Add((line2.Length > 0) ? line2.Substring(1) : string.Empty);
                            sb.AppendLine((line2.Length > 0) ? line2.Substring(1) : string.Empty);
                        }
                    


                    var NOMOR_FAKTUR = row[3];
                    DataRow[] foundRows2 = dt3.Select("CustomerCode='" + CusNamedt + "' AND NOMOR_FAKTUR='" + NOMOR_FAKTUR + "'");
                    if (foundRows2.Count() > 0)
                    {
                        for (int k = 0; k < foundRows2.Length; k++)
                        {
                            line3 = string.Empty;
                            var row3 = foundRows2[k].ItemArray;
                            for (int h = 0; h < row3.Length - 2; h++)
                            {
                                object temp3 = (row3[h] is string) ? (string)row3[h] : row3[h];
                                if (delimeted)
                                    line3 += string.Format(separate + "\"{0}\"", temp3);
                                else line3 += string.Format(separate + "{0}", temp3);
                            }
                            sb.AppendLine((line3.Length > 0) ? line3.Substring(1) : string.Empty);
                            //lines.Add((line3.Length > 0) ? line3.Substring(1) : string.Empty);
                        }
                    }
                    //}
                }
                //bool msg = ShowCsvReport(lines.ToArray(), "csvSkemaTaxOut_" + DateTime.Now.ToString("yyyMMdd"));
                var text = sb.ToString();
                var content = new byte[text.Length * sizeof(char)];
                Buffer.BlockCopy(text.ToCharArray(), 0, content, 0, content.Length);
                var sessionName = "txtSkemaTaxOut_" + DateTime.Now.ToString("yyyMMdd");
                TempData.Add(sessionName, text);
                return Json(new
                {
                    success = true,
                    message = "Download data berhasil",
                    sessionName = sessionName,
                    rowCount = rowCount
                });
            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = "Data Tidak Ada",
                    rowCount = rowCount
                });
            }
        }
        
        public static bool ShowCsvReport(string[] lines, string name)
        {
            string path = Path.GetTempFileName().Replace(".tmp", ".csv");
            if (name != string.Empty)
            {
                string dir = Path.GetTempPath() + Path.GetRandomFileName();
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                path = string.Format("{0}\\{1}.csv", dir, name);
            }

            System.IO.File.WriteAllLines(path, lines);
            ProcessStartInfo psInfo = new ProcessStartInfo(path);
            Process proc = new Process();
            proc.StartInfo = psInfo;
            return proc.Start();
        }

        public FileContentResult DownloadFile(string sessionName)
        {
            //var content = TempData[sessionName] as byte[];

            byte[] dataX = Encoding.UTF8.GetBytes(TempData[sessionName].ToString());

            //var bytes = Encoding.UTF8.GetBytes(content);
            TempData.Clear();
            var stream = new MemoryStream(dataX);

            //var stream = new MemoryStream(content, Encoding.UTF8);
            var contentType = "text/csv";



            Response.Clear();
            Response.ContentType = contentType;
            //Response.AddHeader("content-disposition", "attachment;filename=" + "csvSkemaTaxOut_" + DateTime.Now.ToString("yyyMMdd") + ".txt");
            Response.AddHeader("content-disposition", "attachment;filename=" + sessionName + ".txt");
            Response.Buffer = true;
            stream.WriteTo(Response.OutputStream);
            Response.End();


            return File(dataX, contentType, "");
        }

        public ActionResult GetData4ExelFileSkemaTaxOut(string CompanyCode, string BranchFrom, string BranchTo, string profitCenterCode, string DateFrom, string DateTo, string table2) 
        {
            var Branch = "";

            //if (BranchCode1 == "")
            //{ Branch = "%"; }
            //else { Branch = BranchCode; }
                
            //ExcelFileWriter excelReport;
            string fileName = "";
            fileName = "ExelSkemaTaxOut_";
            fileName = fileName + DateTime.Now.ToString("yyyyMMdd");

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 7200;
            cmd.CommandText = "usprpt_GnGenerateCsvSkemaTaxOut";

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCodeFrom", BranchFrom);
            cmd.Parameters.AddWithValue("@BranchCodeTo", BranchTo);
            cmd.Parameters.AddWithValue("@ProfitCenterCode", profitCenterCode);
            cmd.Parameters.AddWithValue("@PeriodFrom", DateFrom);
            cmd.Parameters.AddWithValue("@PeriodTo", DateTo);
            cmd.Parameters.AddWithValue("@ProductType", ProductType);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("datTable1");
            da.Fill(dt);
            var rowCount = dt.AsEnumerable().Count();

            SqlCommand cmd2 = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd2.CommandTimeout = 7200;
            //cmd2.CommandText = "SELECT '' LT, '' NPWP, '' NAMA, '' JALAN, '' BLOK, '' NOMOR, '' RT, '' RW, '' KECAMATAN, '' KELURAHAN, ''KABUPATEN, '' PROPINSI, ''KODE_POS, '' NOMOR_TELEPON";
            cmd2.CommandText = "usprpt_GnGenerateCsvSkemaTaxOut";
            cmd2.CommandType = CommandType.StoredProcedure;
            cmd2.Parameters.Clear();
            cmd2.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd2.Parameters.AddWithValue("@BranchCodeFrom", BranchFrom);
            cmd2.Parameters.AddWithValue("@BranchCodeTo", BranchTo);
            cmd2.Parameters.AddWithValue("@ProfitCenterCode", profitCenterCode);
            cmd2.Parameters.AddWithValue("@PeriodFrom", DateFrom);
            cmd2.Parameters.AddWithValue("@PeriodTo", DateTo);
            cmd2.Parameters.AddWithValue("@ProductType", ProductType);
            cmd2.Parameters.AddWithValue("@table", "2");
            SqlDataAdapter da2 = new SqlDataAdapter(cmd2);
            DataTable dt2 = new DataTable("datTable2");
            da2.Fill(dt2);

            SqlCommand cmd3 = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd3.CommandTimeout = 7200;
            cmd3.CommandText = "usprpt_GnGenerateCsvSkemaTaxOut";
            cmd3.CommandType = CommandType.StoredProcedure;
            cmd3.Parameters.Clear();
            cmd3.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd3.Parameters.AddWithValue("@BranchCodeFrom", BranchFrom);
            cmd3.Parameters.AddWithValue("@BranchCodeTo", BranchTo);
            cmd3.Parameters.AddWithValue("@ProfitCenterCode", profitCenterCode);
            cmd3.Parameters.AddWithValue("@PeriodFrom", DateFrom);
            cmd3.Parameters.AddWithValue("@PeriodTo", DateTo);
            cmd3.Parameters.AddWithValue("@ProductType", ProductType);
            cmd3.Parameters.AddWithValue("@table", "3");
            SqlDataAdapter da3 = new SqlDataAdapter(cmd3);
            DataTable dt3 = new DataTable("datTable3");
            da3.Fill(dt3);

            if (dt.Rows.Count == 0)
            {
                return Json("Tidak ada data yang ditampilkan", "text/html", JsonRequestBehavior.AllowGet);
            }
            else
            {
                int lastRow = 1;
                int i = 1;
                int j = 1;
                int k = 1; 

                var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("eFaktur Pajak Keluaran");
                //dt.Columns.Remove("CUSTOMERCODE");
                //dt.Columns.Remove("FPJNO");
                foreach (DataColumn column in dt.Columns)
                {
                    if (column.ColumnName != "CUSTOMERCODE")
                    {
                        ws.Cell(lastRow, i).Style.Font.SetBold().Font.SetFontSize(10);
                        ws.Cell(lastRow, i).Style.Font.SetFontSize(10);
                        ws.Cell(lastRow, i).Value = column.ColumnName;
                        i++;
                    }
                }
                lastRow++;

                //if (table2 == "0")
                //{
                    foreach (DataColumn column2 in dt2.Columns)
                    {
                        if (column2.ColumnName != "CUSTOMERCODE")
                        {
                            ws.Cell(lastRow, j).Style.Font.SetBold().Font.SetFontSize(10);
                            ws.Cell(lastRow, j).Style.Font.SetFontSize(10);
                            ws.Cell(lastRow, j).Value = column2.ColumnName;
                            j++;
                        }
                    }
                    lastRow++;
                //}
                

                foreach (DataColumn column3 in dt3.Columns)
                {
                    if (column3.ColumnName != "CUSTOMERCODE" && column3.ColumnName != "NOMOR_FAKTUR" && column3.ColumnName != "FPJNo")
                    {
                        ws.Cell(lastRow, k).Style.Font.SetBold().Font.SetFontSize(10);
                        ws.Cell(lastRow, k).Style.Font.SetFontSize(10);
                        ws.Cell(lastRow, k).Value = column3.ColumnName;
                        k++;
                    }
                }

                return GenerateExcelSkema(wb, dt, dt2, dt3, lastRow, fileName, table2, false, false);
            }
        }

        private ActionResult GenerateExcelSkema(XLWorkbook wb, DataTable dt, DataTable dt2, DataTable dt3, int lastRow, string fileName, string tbl, bool isCustomHeader = false, bool isShowSummary = false)
        {
            var ws = wb.Worksheet(1);
            var tmpLastRow = lastRow;

            int iCol = 1;
            char iChar = 'A';
            int iCol2 = 1;
            char iChar2 = 'A';
            int iCol3 = 1;
            char iChar3 = 'A'; 
            foreach (DataRow dr in dt.Rows)
            {
                iCol = 1;
                iChar = 'A';
                foreach (DataColumn dc in dt.Columns)
                {
                    if (dc.ColumnName.ToString() != "CUSTOMERCODE")
                    {
                        var val = dr[dc.ColumnName];
                        Type typ = dr[dc.ColumnName].GetType();
                        switch (Type.GetTypeCode(typ))
                        {
                            case TypeCode.DateTime:
                                ws.Cell(lastRow + 1, iCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                                ws.Cell(lastRow + 1, iCol).Style.DateFormat.Format = "dd-MMM-yyyy";
                                break;
                            case TypeCode.Int16:
                            case TypeCode.Int32:
                            case TypeCode.Int64:
                            case TypeCode.Double:
                            case TypeCode.Single:
                                ws.Cell(lastRow + 1, iCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(lastRow + 1, iCol).Style.NumberFormat.Format = "#,##0";
                                break;
                            case TypeCode.Decimal:
                                ws.Cell(lastRow + 1, iCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                ws.Cell(lastRow + 1, iCol).Style.NumberFormat.Format = "#,##0.0";
                                break;
                            case TypeCode.Boolean:
                                ws.Cell(lastRow + 1, iCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                break;
                            default:
                                if (string.IsNullOrWhiteSpace(val.ToString()) == false)
                                {
                                    val = val.ToString().Substring(0, 1) == "0" ? val.ToString().Insert(0, "'") : val;
                                }
                                ws.Cell(lastRow + 1, iCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                                break;
                        };

                        if (dr[dc.ColumnName].GetType() == typeof(DateTime))
                        {
                            ws.Cell(lastRow + 1, iCol).Style.DateFormat.Format = "dd-MMM-yyyy";
                        }

                        if (tmpLastRow == lastRow)
                        {
                            if (isCustomHeader == true)
                            {
                                ws.Cell(lastRow, iCol).Value = dc.ColumnName;
                                // ws.Cell(lastRow, iCol).Style.Fill.SetBackgroundColor(XLColor.TeaRoseRose).Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                ws.Cell(lastRow, iCol).Style.Font.SetBold().Font.SetFontSize(10);
                            }
                            ws.Cell(lastRow + 1, iCol).Style.Font.SetFontSize(10);
                            ws.Cell(lastRow + 1, iCol).Value = val;
                        }
                        else
                        {
                            ws.Cell(lastRow + 1, iCol).Style.Font.SetFontSize(10);
                            ws.Cell(lastRow + 1, iCol).Value = val;
                        }

                        iCol++;
                        iChar++;
                    }
                 }
                lastRow++;
                var CusNamedt = dr[19];
                var NoFaktur = dr[3];
                var foundRows = dt2.Select("CUSTOMERCODE='" + CusNamedt + "'");
                if (tbl == "1")
                {
                    foundRows = dt2.Select("CUSTOMERCODE='" + CusNamedt + "' and NPWP<>'000000000000000'");
                }

                    if (foundRows.Count() > 0)
                    {
                        foreach (DataRow dr2 in foundRows)
                        {
                            iCol2 = 1;
                            iChar2 = 'A';
                            foreach (DataColumn dc2 in dt2.Columns)
                            {
                                if (dc2.ColumnName.ToString() != "CUSTOMERCODE")
                                {
                                    var val = dr2[dc2.ColumnName];
                                    Type typ = dr2[dc2.ColumnName].GetType();
                                    switch (Type.GetTypeCode(typ))
                                    {
                                        case TypeCode.DateTime:
                                            ws.Cell(lastRow + 1, iCol2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                                            ws.Cell(lastRow + 1, iCol2).Style.DateFormat.Format = "dd-MMM-yyyy";
                                            break;
                                        case TypeCode.Int16:
                                        case TypeCode.Int32:
                                        case TypeCode.Int64:
                                        case TypeCode.Double:
                                        case TypeCode.Single:
                                            ws.Cell(lastRow + 1, iCol2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                            ws.Cell(lastRow + 1, iCol2).Style.NumberFormat.Format = "#,##0";
                                            break;
                                        case TypeCode.Decimal:
                                            ws.Cell(lastRow + 1, iCol2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                            ws.Cell(lastRow + 1, iCol2).Style.NumberFormat.Format = "#,##0.0";
                                            break;
                                        case TypeCode.Boolean:
                                            ws.Cell(lastRow + 1, iCol2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                            break;
                                        default:
                                            if (string.IsNullOrWhiteSpace(val.ToString()) == false)
                                            {
                                                val = val.ToString().Substring(0, 1) == "0" ? val.ToString().Insert(0, "'") : val;
                                            }
                                            ws.Cell(lastRow + 1, iCol2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                                            break;
                                    };

                                    if (dr2[dc2.ColumnName].GetType() == typeof(DateTime))
                                    {
                                        ws.Cell(lastRow + 1, iCol2).Style.DateFormat.Format = "dd-MMM-yyyy";
                                    }

                                    if (tmpLastRow == lastRow)
                                    {
                                        if (isCustomHeader == true)
                                        {
                                            ws.Cell(lastRow, iCol2).Value = dc2.ColumnName;
                                            // ws.Cell(lastRow, iCol).Style.Fill.SetBackgroundColor(XLColor.TeaRoseRose).Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                            ws.Cell(lastRow, iCol2).Style.Font.SetBold().Font.SetFontSize(10);
                                        }
                                        ws.Cell(lastRow + 1, iCol2).Style.Font.SetFontSize(10);
                                        ws.Cell(lastRow + 1, iCol2).Value = val;
                                    }
                                    else
                                    {
                                        ws.Cell(lastRow + 1, iCol2).Style.Font.SetFontSize(10);
                                        ws.Cell(lastRow + 1, iCol2).Value = val;
                                    }

                                    iCol2++;
                                    iChar2++;
                                }
                            }
                            lastRow++;
                        }
                    }
                
                


                DataRow[] foundRows2 = dt3.Select("CustomerCode='" + CusNamedt + "' AND NOMOR_FAKTUR='" + NoFaktur + "'");
                if (foundRows2.Count() > 0)
                {
                    foreach (DataRow dr3 in dt3.Select("CustomerCode='" + CusNamedt + "' AND NOMOR_FAKTUR='" + NoFaktur + "'"))
                    {
                        iCol3 = 1;
                        iChar3 = 'A';
                        foreach (DataColumn dc3 in dt3.Columns)
                        {
                            if (dc3.ColumnName.ToString() != "CUSTOMERCODE" && dc3.ColumnName.ToString() != "NOMOR_FAKTUR" && dc3.ColumnName.ToString() != "FPJNo")
                            {
                                var val = dr3[dc3.ColumnName];
                                Type typ = dr3[dc3.ColumnName].GetType();
                                switch (Type.GetTypeCode(typ))
                                {
                                    case TypeCode.DateTime:
                                        ws.Cell(lastRow + 1, iCol3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                                        ws.Cell(lastRow + 1, iCol3).Style.DateFormat.Format = "dd-MMM-yyyy";
                                        break;
                                    case TypeCode.Int16:
                                    case TypeCode.Int32:
                                    case TypeCode.Int64:
                                    case TypeCode.Double:
                                    case TypeCode.Single:
                                        ws.Cell(lastRow + 1, iCol3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                        ws.Cell(lastRow + 1, iCol3).Style.NumberFormat.Format = "#,##0";
                                        break;
                                    case TypeCode.Decimal:
                                        ws.Cell(lastRow + 1, iCol3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                        ws.Cell(lastRow + 1, iCol3).Style.NumberFormat.Format = "#,##0.0";
                                        break;
                                    case TypeCode.Boolean:
                                        ws.Cell(lastRow + 1, iCol3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                        break;
                                    default:
                                        if (string.IsNullOrWhiteSpace(val.ToString()) == false)
                                        {
                                            val = val.ToString().Substring(0, 1) == "0" ? val.ToString().Insert(0, "'") : val;
                                        }
                                        ws.Cell(lastRow + 1, iCol3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                                        break;
                                };

                                if (dr3[dc3.ColumnName].GetType() == typeof(DateTime))
                                {
                                    ws.Cell(lastRow + 1, iCol3).Style.DateFormat.Format = "dd-MMM-yyyy";
                                }

                                if (tmpLastRow == lastRow)
                                {
                                    if (isCustomHeader == true)
                                    {
                                        ws.Cell(lastRow, iCol3).Value = dc3.ColumnName;
                                        // ws.Cell(lastRow, iCol).Style.Fill.SetBackgroundColor(XLColor.TeaRoseRose).Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                        ws.Cell(lastRow, iCol3).Style.Font.SetBold().Font.SetFontSize(10);
                                    }
                                    ws.Cell(lastRow + 1, iCol3).Style.Font.SetFontSize(10);
                                    ws.Cell(lastRow + 1, iCol3).Value = val;
                                }
                                else
                                {
                                    ws.Cell(lastRow + 1, iCol3).Style.Font.SetFontSize(10);
                                    ws.Cell(lastRow + 1, iCol3).Value = val;
                                }

                                iCol3++;
                                iChar3++;
                            }
                        }
                        lastRow++;
                    }
                }
            }

            if (isShowSummary)
            {
                ws.Cell(lastRow + 1, 1).Value = "TOTAL";
                int j = 2;
                for (char i = 'B'; i < iChar; i++)
                {
                    ws.Cell(lastRow + 1, j).FormulaA1 = "=SUM(" + i + (tmpLastRow + 1) + ":" + i + lastRow + ")";
                    j++;
                }
            }

            //var rngTable = ws.Range(tmpLastRow + 1, 1, lastRow + 1, iCol - 1);
            //rngTable.Style
            //    .Border.SetTopBorder(XLBorderStyleValues.Thin)
            //    .Border.SetBottomBorder(XLBorderStyleValues.Thin)
            //    .Border.SetLeftBorder(XLBorderStyleValues.Thin)
            //    .Border.SetRightBorder(XLBorderStyleValues.Thin);


            ws.Columns().Style.Alignment.SetVertical(XLAlignmentVerticalValues.Top);
            ws.Columns().Style.Alignment.SetWrapText();
            ws.Columns().AdjustToContents();

            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));

            return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
        }

        public JsonResult GetData4TxtFileSkemaTaxIn(string CompanyCode, string BranchCode1, string DateFrom, string DateTo, string table2, string separate, string delimeter)
        {
            var Branch = "";
            bool delimeted = delimeter == "1" ? true : false;
            if (BranchCode1 == "")
            { Branch = "%"; }
            else { Branch = BranchCode; }

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 1800;
            cmd.CommandText = "usprpt_EFakturTaxIn";

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", Branch);
            cmd.Parameters.AddWithValue("@PeriodFrom", DateFrom);
            cmd.Parameters.AddWithValue("@PeriodTo", DateTo);
            cmd.Parameters.AddWithValue("@ProductType", ProductType);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("datTable1");
            da.Fill(dt);
            var rowCount = dt.AsEnumerable().Count();

            if (dt.Rows.Count > 0)
            {
                List<string> lines = new List<string>();
                var sb = new StringBuilder();
                string line = string.Empty;
                string line2 = string.Empty;
                string line3 = string.Empty;

                // Add header into csv file
                foreach (DataColumn column in dt.Columns)
                {
                    if (delimeted)
                        line += string.Format(separate +"\"{0}\"", column.ColumnName);
                    else line += string.Format(separate + "{0}", column.ColumnName);
                }
                //lines.Add((line.Length > 0) ? line.Substring(1) : string.Empty);
                sb.AppendLine((line.Length > 0) ? line.Substring(1) : string.Empty);

                // Add detail into csv file
               
                foreach (DataRow row in dt.Rows)
                {
                    line = string.Empty;
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        //line.Replace("\",\"", ";")
                        object temp = (row[i] is string) ? (string)row[i] : row[i];
                        if (delimeted)
                            line += string.Format(separate + "\"{0}\"", temp);
                        else line += string.Format(separate + "{0}", temp);
                    }
                    //lines.Add((line.Length > 0) ? line.Substring(1) : string.Empty);
                    sb.AppendLine((line.Length > 0) ? line.Substring(1) : string.Empty);
                }
                //bool msg = ShowCsvReport(lines.ToArray(), "csvSkemaTaxOut_" + DateTime.Now.ToString("yyyMMdd"));
                var text = sb.ToString();
                var content = new byte[text.Length * sizeof(char)];
                Buffer.BlockCopy(text.ToCharArray(), 0, content, 0, content.Length);
                var sessionName = "txtSkemaTaxIn_" + DateTime.Now.ToString("yyyMMdd");
                //TempData.Add(sessionName, content);
                TempData.Add(sessionName, text);
                return Json(new
                {
                    success = true,
                    message = "Download data berhasil",
                    sessionName = sessionName,
                    rowCount = rowCount
                });
            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = "Data Tidak Ada",
                    rowCount = rowCount
                });
            }
        }

        public ActionResult GetData4ExelEFakturTaxIn(string CompanyCode, string BranchCode1, string DateFrom, string DateTo, string table2)
        {
            var Branch = "";
            if (BranchCode1 == "")
            { Branch = "%"; }
            else { Branch = BranchCode; }

            //ExcelFileWriter excelReport;
            string fileName = "";
            fileName = "ExelEFakturTaxIn_";
            fileName = fileName + DateTime.Now.ToString("yyyyMMdd");

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 1800;
            cmd.CommandText = "usprpt_EFakturTaxIn";

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", CurrentUser.BranchCode);
            cmd.Parameters.AddWithValue("@PeriodFrom", DateFrom);
            cmd.Parameters.AddWithValue("@PeriodTo", DateTo);
            cmd.Parameters.AddWithValue("@ProductType", ProductType);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("datTable1");
            da.Fill(dt);
            var rowCount = dt.AsEnumerable().Count();

            if (dt.Rows.Count == 0)
            {
                return Json("Tidak ada data yang ditampilkan", "text/html", JsonRequestBehavior.AllowGet);
            }
            else
            {
                int lastRow = 1;
                int i = 1;

                var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("eFaktur Pajak Masukan 2014");

                //foreach (DataColumn column in dt.Columns)
                //{
                //    ws.Cell(lastRow, i).Style.Font.SetBold().Font.SetFontSize(10);
                //    ws.Cell(lastRow, i).Style.Font.SetFontSize(10);
                //    ws.Cell(lastRow, i).Style.Border.SetTopBorder(XLBorderStyleValues.Thin);
                //    ws.Cell(lastRow, i).Style.Border.SetBottomBorder(XLBorderStyleValues.Thin);
                //    ws.Cell(lastRow, i).Style.Border.SetLeftBorder(XLBorderStyleValues.Thin);
                //    ws.Cell(lastRow, i).Style.Border.SetRightBorder(XLBorderStyleValues.Thin);
                //    ws.Cell(lastRow, i).Value = column.ColumnName;
                //    i++;
                //}
                //lastRow++;
                return GenerateExcel(wb, dt, lastRow, fileName, true, false);
            }
        }
    }
}
