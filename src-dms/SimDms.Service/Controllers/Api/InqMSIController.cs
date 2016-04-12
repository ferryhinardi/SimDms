using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SimDms.Service.Models;
using SimDms.Common.DcsWs;
using SimDms.Common.Models;
using System.Text;
using System.IO;
using ClosedXML.Excel;

namespace SimDms.Service.Controllers.Api
{
    public class InqMSIController : BaseController
    {
        private const string dataID = "WMSIA";
        private string msg = "";
        private DcsWsSoapClient ws = new DcsWsSoapClient();

        public JsonResult Default()
        {
            return Json(new
            {
                CompanyCode = CompanyCode,
                CompanyName = CompanyName,
                BranchCode = BranchCode,
                BranchName = BranchName,
                Month = DateTime.Now.Month,
                Year = DateTime.Now.Year
            });
        }

        public JsonResult ListOfMonth()
        {
            List<Object> listObj = new List<Object>();
            string[] listMonth = new string[] { "Januari", "Februari", "Maret", "April", "Mei", "Juni", "Juli", "Agustus", "September", "Oktober", "November", "Desember" };
            int idx = 1;
            foreach (var month in listMonth)
            {
                listObj.Add(new { value = idx, text = month });
                idx++;
            }
            return Json(listObj);
        }

        public JsonResult ListOfYear()
        {
            var year = DateTime.Now.Year;
            List<Object> listObj = new List<Object>();
            for (int i = year - 5; i <= year; i++)
            {
                listObj.Add(new { value = i.ToString(), text = i.ToString() });
            }
            return Json(listObj);
        }

        public JsonResult LoadData(int month, int year)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;

            cmd.CommandText = "usprpt_SvRpReport021V3";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 3600;
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@ProductType", ProductType);
            cmd.Parameters.AddWithValue("@PeriodYear", year);
            cmd.Parameters.AddWithValue("@Month2", month);
            cmd.Parameters.AddWithValue("@UserId", CurrentUser.UserId);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            var data = from a in dt.AsEnumerable().AsQueryable()
                       select new MSI
                       {
                           CompanyCode = a[0].ToString(),
                           BranchCode = a[1].ToString(),
                           PeriodYear = Convert.ToDecimal(a[2].ToString()),
                           SeqNo = Convert.ToInt32(a[3].ToString()),
                           MsiGroup = a[4].ToString(),
                           MsiDesc = a[5].ToString(),
                           Unit = a[6].ToString(),
                           Average = Convert.ToDecimal(a[7].ToString()),
                           Total = Convert.ToDecimal(a[8].ToString()),
                           Jan = Convert.ToDecimal(a[9].ToString()),
                           Feb = Convert.ToDecimal(a[10].ToString()),
                           Mar = Convert.ToDecimal(a[11].ToString()),
                           Apr = Convert.ToDecimal(a[12].ToString()),
                           May = Convert.ToDecimal(a[13].ToString()),
                           Jun = Convert.ToDecimal(a[14].ToString()),
                           Jul = Convert.ToDecimal(a[15].ToString()),
                           Aug = Convert.ToDecimal(a[16].ToString()),
                           Sep = Convert.ToDecimal(a[17].ToString()),
                           Oct = Convert.ToDecimal(a[18].ToString()),
                           Nov = Convert.ToDecimal(a[19].ToString()),
                           Dec = Convert.ToDecimal(a[20].ToString())
                       };

            return Json(data);
        }

        public JsonResult GenerateWMSIA(int year, int month)
        {
            var dt = GenWMSIA(year, month);
            var data = "";

            if (dt.Rows.Count > 1)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    data += dt.Rows[i][0].ToString() + " ";
                }
            }

            return Json(data);
        }

        public FileContentResult DownloadFile(int year, int month)
        {
            var dt = GenWMSIA(year, month);
            var data = "";
            StringBuilder sb = new StringBuilder();

            if (dt.Rows.Count > 1)
            {
                //data = dt.Rows[0][0].ToString();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    data += dt.Rows[i][0].ToString() + "\n";
                }
            }

            string[] vars = data.Split('\n');
            int len = vars.Length; int con = 1;
            foreach (string var in vars)
            {
                if (len == con)
                    sb.Append(var);
                else
                    sb.AppendLine(var);
                con++;
            }

            string WMSIA = sb.ToString();
            byte[] content = new byte[WMSIA.Length * sizeof(char)];
            System.Buffer.BlockCopy(WMSIA.ToCharArray(), 0, content, 0, content.Length);
            string contentType = "application/text";
            Response.Clear();
            MemoryStream ms = new MemoryStream(content);
            Response.ContentType = "application/text";
            Response.AddHeader("content-disposition", "attachment;filename=WMSIA.txt");
            Response.Buffer = true;
            ms.WriteTo(Response.OutputStream);
            Response.End();
            //Parameters to file are
            //1. The File Path on the File Server
            //2. The content type MIME type
            //3. The parameter for the file save by the browser
            return File(content, contentType, "WMSIA.txt");
        }

        public JsonResult SendFile(int year, int month)
        {
            var dt = GenWMSIA(year, month);
            var data = "";
            StringBuilder sb = new StringBuilder();

            string[] vars = data.Split('\n');

            for (int i = 0; i < vars.Length; i++)
            {
                if (i + 1 < vars.Length) sb.AppendLine(vars[i]);
                else sb.Append(vars[i]);
            }

            string header = sb.ToString().Split('\n')[0];
            try
            {

                string result = ws.SendToDcs(dataID, CompanyCode, data, ProductType);
                if (result.StartsWith("FAIL")) return Json(new { success = false, message = result.Substring(5) });

                LogHeaderFile(dataID, CompanyCode, header, ProductType);
                msg = string.Format("{0} berhasil di upload", dataID);
                return Json(new { success = true, message = msg });
            }
            catch (Exception ex)
            {
                msg = string.Format("{0} gagal digenerate : {1}", dataID, ex.Message.ToString());
                return Json(new { success = false, message = msg });
            }
        }

        public JsonResult ValidateHeaderFile(int year, int month)
        {
            StringBuilder sb = new StringBuilder();
            DataTable dtHeader = new DataTable();
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            string data = "";
            cmd.CommandText = "uspfn_SvMsiFlatFile";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@PeriodYear", year);
            cmd.Parameters.AddWithValue("@PeriodMonth", month);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dtHeader);

            if (dtHeader.Rows.Count > 1)
            {
                for (int i = 0; i < dtHeader.Rows.Count; i++)
                {
                    data += dtHeader.Rows[i][0].ToString() + "\n";
                }
            }

            var result = true;

            string[] vars = data.Split('\n');

            for (int i = 0; i < vars.Length; i++)
            {
                if (i + 1 < vars.Length) sb.AppendLine(vars[i]);
                else sb.Append(vars[i]);
            }

            var header = sb.ToString();


            string qry = string.Format("select * from gnDcsUploadFile where DataID = '{0}' and Header = '{1}'", dataID, header);
            var dt = ctx.Database.SqlQuery<GnDcsUploadFile>(qry);
            if (dt.Count() > 0)
            {
                result = false;
                msg = string.Format("Data {0} sudah pernah dikirim pada {1}, apakah akan dikirim ulang?", dataID, dt.FirstOrDefault().CreatedDate);
            }

            return Json(new { success = result, message = msg });
        }

        private void LogHeaderFile(string dataID, string custCode, string header, string prodType)
        {
            string query = "exec uspfn_spLogHeader @p0,@p1,@p2,@p3,@p4,@p5";
            object[] Parameters = { dataID, custCode, prodType, "SEND", DateTime.Now, header };
            ctx.Database.ExecuteSqlCommand(query, Parameters);
        }

        private DataTable GenWMSIA(int year, int month)
        {
            StringBuilder sb = new StringBuilder();
            DataTable dt = new DataTable();
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            string data = "";
            cmd.CommandText = "uspfn_SvMsiFlatFile";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@PeriodYear", year);
            cmd.Parameters.AddWithValue("@PeriodMonth", month);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            return dt;
        }

        //start fhi 30-04-2015 : add areas, OrganizationsV2, branchv2,svmsiv2 and export excel for inqmsi v2
        public JsonResult Areas()
        {
            var query = string.Format(@"select distinct groupNo, area,
	                                        case when area='SUMATERA' then (GroupNo+150) else groupno end orders
                                        from GnMstDealerMapping
                                        order by orders");
            var queryable = ctx.Database.SqlQuery<Areas>(query).AsQueryable();
            return Json(queryable.Select(p => new { value = p.area, text = p.area.ToUpper(), p.groupNo, p.orders }).Distinct().OrderBy(p => p.orders));
        }

        public ActionResult isNational()
        {
            //string comp = CurrentUser.DealerCode;
            string comp = CompanyCode;
            string brcd = BranchCode;


            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_isNational";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", comp);
            cmd.Parameters.AddWithValue("@BranchCode", brcd);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            DataTable dt1 = new DataTable();
            DataTable dt2 = new DataTable();
            da.Fill(ds);

            dt1 = ds.Tables[0];
            dt2 = ds.Tables[1];

            var isnat = dt1.Rows[0][0].ToString();
            var area = "";
            var dealerCd = "";
            var dealerNm = "";
            var outletCd = "";
            var outletNm = "";
            if (dt2.Rows.Count > 0)
            {
                 area = dt2.Rows[0][1].ToString();
                 dealerCd = dt2.Rows[0][2].ToString();
                 dealerNm = dt2.Rows[0][2].ToString() + " - " + dt2.Rows[0][3].ToString();
                 outletCd = dt2.Rows[0][4].ToString();
                 outletNm = dt2.Rows[0][4].ToString() + " - " + dt2.Rows[0][5].ToString();
            }
  
            return Json(new { isNational = isnat, area = area, dealerCd = dealerCd, dealerNm = dealerNm, outletCd = outletCd, outletNm = outletNm });
        }

        public ActionResult OrganizationsV2(string area = "")
        {
            if (area == null || area == string.Empty)
            {
                var qry = ctx.OrganizationHdrs.OrderBy(p => p.CompanyCode).AsQueryable();
                return Json(qry.Select(p => new { value = p.CompanyCode, text = p.CompanyCode + " - " + p.CompanyName.ToUpper(), p.CompanyName }).OrderBy(p => p.CompanyName));
            }
            else
            {
                var qry1 = ctx.GnMstDealerMapping.Where(p => p.Area == area).AsQueryable();
                var companyCode = qry1.Select(q => q.DealerCode);
                var qry2 = ctx.OrganizationHdrs.Where(y => companyCode.Contains(y.CompanyCode));
                return Json(qry2.Select(p => new { value = p.CompanyCode, text = p.CompanyCode + " - " + p.CompanyName.ToUpper(), p.CompanyName }).OrderBy(p => p.CompanyName));
            }
        }

        public JsonResult BranchsV2(string area = "", string comp = "")
        {          
            var query = string.Format(@"select gdom.DealerCode,gdom.OutletCode,gmcp.CompanyName 
                                        from [gnMstDealerOutletMapping] gdom 
                                        inner join (select * from GnMstDealerMapping where Area='{0}' ) gdm on gdm.DealerCode=gdom.DealerCode 
                                        inner join gnMstCoProfile gmcp on gmcp.BranchCode=gdom.OutletCode
                                        where gdom.DealerCode='{1}'", area, comp);
            var queryable = ctx.Database.SqlQuery<BranchV2View>(query).AsQueryable();
            return Json(queryable.Select(p => new { value = p.OutletCode, text = p.CompanyName.ToUpper(), p.OutletCode }).Distinct().OrderBy(p => p.OutletCode));

        }

        public JsonResult SvMsiV2()
        {
            //string area = Request["Area"] ?? "";
            string comp = CompanyCode;//Request["CompanyCode"] ?? "";
            string bran = BranchCode;//Request["BranchCode"] ?? "";
            string crYr = Convert.ToString(DateTime.Today.Year);
            string year = string.IsNullOrWhiteSpace(Request["Year"]) ? crYr : Request["Year"];
            string dataSource = Request["DataSource"] ?? "";
            string month = Request["Month"];
            string sp = "";
            string DataSource = Request["DataSource"];
            if (dataSource == "Invoice")
            {
                sp = "usprpt_SvRpReport021V3Web";
                //sp = "uspfn_DlrInqMsiV2ByBranch";
            }
            else
            {
                sp = "usprpt_SvRpReport021V3SPKWeb";
                //sp = "uspfn_DlrInqMsiV2ByBranch_SPK";
            }

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = sp;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 360;
            cmd.Parameters.Clear();
            //cmd.Parameters.AddWithValue("@Area", area);
            cmd.Parameters.AddWithValue("@CompanyCode", comp);
            cmd.Parameters.AddWithValue("@BranchCode", bran);
            cmd.Parameters.AddWithValue("@ProductType", ProductType);
            cmd.Parameters.AddWithValue("@PeriodYear", year);
            cmd.Parameters.AddWithValue("@Month2", month);
            cmd.Parameters.AddWithValue("@UserId", CurrentUser.UserId);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            da.Fill(ds);

            if (comp == "" || bran == "")
            {
                dt = ds.Tables[1];
            }
            else
            {
                dt = ds.Tables[0];
            }
            return Json(GetJson(dt));
        }

        public ActionResult exportExcel(string Area, string Dealer, string Outlet, string SpID, string Year, string TextArea, string TextDealer, string TextOutlet,string DataSource, string Month)
        {
            string fileName = "";
            fileName = "Inq_Suzuki_MSI" + "_";
            fileName = fileName + DateTime.Now.ToString("yyyyMMdd-hhmmss");

            string area = Area ?? "";
            string dealer = CompanyCode; //Dealer ?? "";
            string outlet = BranchCode;//Outlet ?? "";
            var GnMstDealerOutletMapping = ctx.GnMstDealerOutletMapping.Where(x => x.DealerCode == dealer && x.OutletCode == outlet).FirstOrDefault();
            TextArea = GnMstDealerOutletMapping.OutletArea; 
            TextOutlet = GnMstDealerOutletMapping.OutletName;
            TextDealer = ctx.CoProfiles.Where(x => x.CompanyCode == dealer).FirstOrDefault().CompanyGovName;
            string crYr = Convert.ToString(DateTime.Today.Year);
            string year = string.IsNullOrWhiteSpace(Request["Year"]) ? crYr : Request["Year"];
            string month = Month;

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "EXEC " + SpID + " '" + area + "','" + dealer + "','" + outlet + "','" + year + "','"+month+"'";
            cmd.CommandTimeout = 360;

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            DataTable dt1 = new DataTable();
            DataTable dt2 = new DataTable();

            var wb = new XLWorkbook();

            if (dealer != "" && outlet != "")
            {
                dt1 = ds.Tables[0];
                if (dt1.Rows.Count == 0)
                {
                    return Json("Tidak ada data yang ditampilkan", "text/html", JsonRequestBehavior.AllowGet);
                }
                #region ** format Excell  By Branch **
                else
                {
                    int lastRow = 8;
                    int index = 0;
                    int indexRow = 0;

                    var msiGroupNameCheck = "";
                    var msiGroupName = "";

                    var sheetName = ds.Tables[0].Rows[0][1].ToString();
                    var ws = wb.Worksheets.Add(sheetName);

                    #region ** write header **
                    var hdrTable = ws.Range("A1:U7");
                    hdrTable.Style
                        .Font.SetBold()
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                        .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                    var rngTable = ws.Range("A7:U7");
                    rngTable.Style
                        .Font.SetBold()
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                        .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                        .Alignment.SetWrapText();

                    rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    rngTable.Style.Font.Bold = true;

                    ws.Columns("1").Width = 40;
                    ws.Columns("2").Width = 5;
                    ws.Columns("3").Width = 90;
                    ws.Columns("4").Width = 10;
                    ws.Columns("5").Width = 20;
                    ws.Columns("6").Width = 20;
                    ws.Columns("7").Width = 15;
                    ws.Columns("8").Width = 15;
                    ws.Columns("9").Width = 15;
                    ws.Columns("10").Width = 15;
                    ws.Columns("11").Width = 15;
                    ws.Columns("12").Width = 15;
                    ws.Columns("13").Width = 15;
                    ws.Columns("14").Width = 15;
                    ws.Columns("15").Width = 15;
                    ws.Columns("16").Width = 15;
                    ws.Columns("17").Width = 15;
                    ws.Columns("18").Width = 15;

                    //First Names   
                    ws.Cell("A1").Value = "Inquiry Suzuki MSI V2 by "+ DataSource;
                    ws.Cell("A1").Style.Font.SetBold().Font.SetFontSize(14);
                    ws.Cell("A2").Value = "Year";
                    ws.Cell("D2").Value = "Date";
                    ws.Cell("A3").Value = "Area";
                    ws.Cell("A4").Value = "Dealer";
                    ws.Cell("A5").Value = "Outlet";

                    ws.Cell("B2").Value = Year;
                    ws.Cell("E2").Value = DateTime.Now.ToString();
                    ws.Cell("E2").Style.DateFormat.Format = "DD-MMM-YYYY HH:mm";
                    ws.Cell("B3").Value = TextArea;
                    ws.Cell("B4").Value = TextDealer;
                    ws.Cell("B5").Value = TextOutlet;

                    ws.Cell("A7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("A7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("A7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("A7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("A7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("A7").Value = "MSI Group ";

                    ws.Cell("B7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    //ws.Cell("B7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("B7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("B7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("B7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    //ws.Cell("B7").Value = "No ";

                    //ws.Cell("C7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    //ws.Cell("C7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("C7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("C7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("C7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("C7").Value = "Description ";

                    //ws.Cell("D7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    //ws.Cell("D7").Value = "Unit ";

                    ws.Cell("E7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("E7").Value = "Average/Month ";

                    ws.Cell("F7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("F7").Value = "Total ";

                    ws.Cell("G7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("G7").Value = "Jan ";

                    ws.Cell("H7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("H7").Value = "Feb ";

                    ws.Cell("I7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("I7").Value = "Mar ";

                    ws.Cell("J7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("J7").Value = "Apr ";

                    ws.Cell("K7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("K7").Value = "May ";

                    ws.Cell("L7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("L7").Value = "Jun ";

                    ws.Cell("M7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("M7").Value = "Jul ";

                    ws.Cell("N7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("N7").Value = "Aug ";

                    ws.Cell("O7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("O7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("O7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("O7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("O7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("O7").Value = "Sep ";

                    ws.Cell("P7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("P7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("P7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("P7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("P7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("P7").Value = "Oct ";

                    ws.Cell("Q7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Q7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Q7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Q7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Q7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("Q7").Value = "Nov ";

                    ws.Cell("R7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("R7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("R7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("R7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("R7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("R7").Value = "Dec ";

                    #endregion

                    #region ** field values **
                    foreach (var row in dt1.Rows)
                    {
                        msiGroupName = ((System.Data.DataRow)(row)).ItemArray[4].ToString();
                        if (msiGroupNameCheck != msiGroupName)
                        {
                            indexRow = 0;
                            index = index + 1;
                            //if (index == 1)
                            //{
                            //    indexRow = indexRow + (35 - 1);
                            //}
                            //if (index == 2)
                            //{
                            //    indexRow = indexRow + (9 - 1);
                            //}
                            //if (index == 3)
                            //{
                            //    indexRow = indexRow + (49 - 1);
                            //}
                            //if (index == 4)
                            //{
                            //    indexRow = indexRow + (12 - 1);
                            //}
                            //if (index == 5)
                            //{
                            //    indexRow = indexRow + (13 - 1);
                            //}
                            //if (index == 6)
                            //{
                            //    indexRow = indexRow + (8 - 1);
                            //}
                            //if (index == 7)
                            //{
                            //    indexRow = indexRow + (1 - 1);
                            //}
                            //if (index == 8)
                            //{
                            //    indexRow = indexRow + (12 - 1);
                            //}
                            if (index == 1)
                            {
                                indexRow = indexRow + (30 - 1);
                            }
                            if (index == 2)
                            {
                                indexRow = indexRow + (5 - 1);
                            }
                            if (index == 3)
                            {
                                indexRow = indexRow + (24 - 1);
                            }

                            if (index == 4)
                            {
                                indexRow = indexRow + (12 - 1);
                            }
                            if (index == 5)
                            {
                                indexRow = indexRow + (9 - 1);
                            }
                            if (index == 6)
                            {
                                indexRow = indexRow + (8 - 1);
                            }
                            if (index == 7)
                            {
                                indexRow = indexRow + (12 - 1);
                            }

                            //MSI GROUP
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Merge();
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Value = ((System.Data.DataRow)(row)).ItemArray[4];
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;


                        }

                        //NO
                        ws.Cell("B" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        //ws.Cell("B" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("B" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("B" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("B" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[3];
                        ws.Cell("B" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        //MSI DESC
                        //ws.Cell("C" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        //ws.Cell("C" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("C" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("C" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("C" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[5];
                        ws.Cell("C" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        //UNIT
                        //ws.Cell("D" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("D" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("D" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("D" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("D" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[6];
                        ws.Cell("D" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        //AVERAGE
                        ws.Cell("E" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("E" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("E" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("E" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("E" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[7];
                        ws.Cell("E" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("E" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //TOTAL
                        ws.Cell("F" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("F" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("F" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("F" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("F" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[8];
                        ws.Cell("F" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("F" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH01
                        ws.Cell("G" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("G" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("G" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("G" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("G" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[9];
                        ws.Cell("G" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("G" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH02
                        ws.Cell("H" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("H" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("H" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("H" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("H" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[10];
                        ws.Cell("H" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("H" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH03
                        ws.Cell("I" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("I" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("I" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("I" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("I" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[11];
                        ws.Cell("I" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("I" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH04
                        ws.Cell("J" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("J" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("J" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("J" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("J" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[12];
                        ws.Cell("J" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("J" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH05
                        ws.Cell("K" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("K" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("K" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("K" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("K" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[13];
                        ws.Cell("K" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("K" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH06
                        ws.Cell("L" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("L" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("L" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("L" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("L" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[14];
                        ws.Cell("L" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("L" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH07
                        ws.Cell("M" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("M" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("M" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("M" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("M" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[15];
                        ws.Cell("M" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("M" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH08
                        ws.Cell("N" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("N" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("N" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("N" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("N" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[16];
                        ws.Cell("N" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("N" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH09
                        ws.Cell("O" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("O" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("O" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("O" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("O" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[17];
                        ws.Cell("O" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("O" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH10
                        ws.Cell("P" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("P" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("P" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("P" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("P" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[18];
                        ws.Cell("P" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("P" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH11
                        ws.Cell("Q" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("Q" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("Q" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("Q" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("Q" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[19];
                        ws.Cell("Q" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("Q" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH12
                        ws.Cell("R" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("R" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("R" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("R" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("R" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[20];
                        ws.Cell("R" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("R" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        msiGroupNameCheck = msiGroupName;
                        lastRow++;
                    }

                    #endregion

                }
                #endregion

            }
            else
            {
                dt1 = ds.Tables[0];
                dt2 = ds.Tables[1];

                if (dt1.Rows.Count == 0)
                {
                    return Json("Tidak ada data yang ditampilkan", "text/html", JsonRequestBehavior.AllowGet);
                }

                #region ** write Area==null **
                if (area == "")
                {
                    int lastRow = 8;
                    int index = 0;
                    int indexRow = 0;

                    area = "-- SELECT ALL --";
                    TextDealer = "-- SELECT ALL --";
                    TextOutlet = "-- SELECT ALL --";

                    var msiGroupNameCheck = "";
                    var msiGroupName = "";

                    var ws = wb.Worksheets.Add(area);

                    #region ** write header **
                    var hdrTable = ws.Range("A1:U7");
                    hdrTable.Style
                        .Font.SetBold()
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                        .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                    var rngTable = ws.Range("A7:U7");
                    rngTable.Style
                        .Font.SetBold()
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                        .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                        .Alignment.SetWrapText();

                    rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    rngTable.Style.Font.Bold = true;

                    ws.Columns("1").Width = 40;
                    ws.Columns("2").Width = 5;
                    ws.Columns("3").Width = 90;
                    ws.Columns("4").Width = 10;
                    ws.Columns("5").Width = 20;
                    ws.Columns("6").Width = 20;
                    ws.Columns("7").Width = 15;
                    ws.Columns("8").Width = 15;
                    ws.Columns("9").Width = 15;
                    ws.Columns("10").Width = 15;
                    ws.Columns("11").Width = 15;
                    ws.Columns("12").Width = 15;
                    ws.Columns("13").Width = 15;
                    ws.Columns("14").Width = 15;
                    ws.Columns("15").Width = 15;
                    ws.Columns("16").Width = 15;
                    ws.Columns("17").Width = 15;
                    ws.Columns("18").Width = 15;

                    //First Names   
                    ws.Cell("A1").Value = "Inquiry Suzuki MSI V2 by " + DataSource;
                    ws.Cell("A1").Style.Font.SetBold().Font.SetFontSize(14);
                    ws.Cell("A2").Value = "Year";
                    ws.Cell("D2").Value = "Date";
                    ws.Cell("A3").Value = "Area";
                    ws.Cell("A4").Value = "Dealer";
                    ws.Cell("A5").Value = "Outlet";

                    ws.Cell("B2").Value = Year;
                    ws.Cell("E2").Value = DateTime.Now.ToString();
                    ws.Cell("E2").Style.DateFormat.Format = "DD-MMM-YYYY HH:mm";
                    ws.Cell("B3").Value = TextArea;
                    ws.Cell("B4").Value = TextDealer;
                    ws.Cell("B5").Value = TextOutlet;

                    ws.Cell("A7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("A7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("A7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("A7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("A7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("A7").Value = "MSI Group ";

                    ws.Cell("B7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    //ws.Cell("B7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("B7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("B7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("B7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    //ws.Cell("B7").Value = "No ";

                    //ws.Cell("C7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    //ws.Cell("C7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("C7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("C7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("C7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("C7").Value = "Description ";

                    //ws.Cell("D7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    //ws.Cell("D7").Value = "Unit ";

                    ws.Cell("E7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("E7").Value = "Average/Month ";

                    ws.Cell("F7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("F7").Value = "Total ";

                    ws.Cell("G7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("G7").Value = "Jan ";

                    ws.Cell("H7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("H7").Value = "Feb ";

                    ws.Cell("I7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("I7").Value = "Mar ";

                    ws.Cell("J7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("J7").Value = "Apr ";

                    ws.Cell("K7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("K7").Value = "May ";

                    ws.Cell("L7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("L7").Value = "Jun ";

                    ws.Cell("M7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("M7").Value = "Jul ";

                    ws.Cell("N7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("N7").Value = "Aug ";

                    ws.Cell("O7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("O7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("O7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("O7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("O7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("O7").Value = "Sep ";

                    ws.Cell("P7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("P7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("P7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("P7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("P7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("P7").Value = "Oct ";

                    ws.Cell("Q7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Q7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Q7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Q7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Q7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("Q7").Value = "Nov ";

                    ws.Cell("R7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("R7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("R7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("R7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("R7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("R7").Value = "Dec ";

                    #endregion

                    #region ** field values **
                    foreach (var row in dt2.Rows)
                    {
                        msiGroupName = ((System.Data.DataRow)(row)).ItemArray[2].ToString();
                        if (msiGroupNameCheck != msiGroupName)
                        {
                            indexRow = 0;
                            index = index + 1;
                            if (index == 1)
                            {
                                indexRow = indexRow + (35- 1);
                            }
                            if (index == 2)
                            {
                                indexRow = indexRow + (9 - 1);
                            }
                            if (index == 3)
                            {
                                indexRow = indexRow + (49 - 1);
                            }

                            if (index == 4)
                            {
                                indexRow = indexRow + (12 - 1);
                            }
                            if (index == 5)
                            {
                                indexRow = indexRow + (13 - 1);
                            }
                            if (index == 6)
                            {
                                indexRow = indexRow + (8 - 1);
                            }
                            if (index == 7)
                            {
                                indexRow = indexRow + (1 - 1);
                            }
                            if (index == 8)
                            {
                                indexRow = indexRow + (12 - 1);
                            }
                            //MSI GROUP
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Merge();
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Value = ((System.Data.DataRow)(row)).ItemArray[2];
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;


                        }

                        //NO
                        ws.Cell("B" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        //ws.Cell("B" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("B" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("B" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("B" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[1];
                        ws.Cell("B" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        //MSI DESC
                        //ws.Cell("C" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        //ws.Cell("C" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("C" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("C" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("C" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[3];
                        ws.Cell("C" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        //UNIT
                        //ws.Cell("D" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("D" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("D" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("D" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("D" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[4];
                        ws.Cell("D" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        //AVERAGE
                        ws.Cell("E" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("E" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("E" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("E" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("E" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[5];
                        ws.Cell("E" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        //TOTAL
                        ws.Cell("F" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("F" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("F" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("F" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("F" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[6];
                        ws.Cell("F" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        //MONTH01
                        ws.Cell("G" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("G" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("G" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("G" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("G" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[7];
                        ws.Cell("G" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        //MONTH02
                        ws.Cell("H" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("H" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("H" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("H" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("H" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[8];
                        ws.Cell("H" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        //MONTH03
                        ws.Cell("I" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("I" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("I" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("I" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("I" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[9];
                        ws.Cell("I" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        //MONTH04
                        ws.Cell("J" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("J" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("J" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("J" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("J" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[10];
                        ws.Cell("J" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        //MONTH05
                        ws.Cell("K" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("K" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("K" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("K" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("K" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[11];
                        ws.Cell("K" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        //MONTH06
                        ws.Cell("L" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("L" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("L" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("L" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("L" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[12];
                        ws.Cell("L" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        //MONTH07
                        ws.Cell("M" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("M" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("M" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("M" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("M" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[13];
                        ws.Cell("M" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        //MONTH08
                        ws.Cell("N" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("N" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("N" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("N" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("N" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[14];
                        ws.Cell("N" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        //MONTH09
                        ws.Cell("O" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("O" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("O" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("O" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("O" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[15];
                        ws.Cell("O" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        //MONTH10
                        ws.Cell("P" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("P" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("P" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("P" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("P" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[16];
                        ws.Cell("P" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        //MONTH11
                        ws.Cell("Q" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("Q" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("Q" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("Q" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("Q" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[17];
                        ws.Cell("Q" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        //MONTH12
                        ws.Cell("R" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("R" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("R" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("R" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("R" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[18];
                        ws.Cell("R" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        msiGroupNameCheck = msiGroupName;
                        lastRow++;
                    }
                    #endregion

                }
                #endregion
                else
                {
                    int lastRow = 8;
                    int index = 0;
                    int indexRow = 0;

                    var msiGroupNameCheck = "";
                    var msiGroupName = "";
                    var sheetNameCheck = "";
                    var sheetName = area;
                    var dealerCode = "";

                    if (sheetName == "JAWA TIMUR / BALI / LOMBOK")
                    {
                        sheetName = "JAWA TIMUR - BALI - LOMBOK";
                    }

                    var ws = wb.Worksheets.Add(sheetName);
                    var hdrTable = ws.Range("A1:U7");
                    var rngTable = ws.Range("A7:U7");

                    #region ** write header summary **
                    hdrTable = ws.Range("A1:U7");
                    hdrTable.Style
                        .Font.SetBold()
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                        .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                    rngTable = ws.Range("A7:U7");
                    rngTable.Style
                        .Font.SetBold()
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                        .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                        .Alignment.SetWrapText();

                    rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    rngTable.Style.Font.Bold = true;

                    ws.Columns("1").Width = 40;
                    ws.Columns("2").Width = 5;
                    ws.Columns("3").Width = 90;
                    ws.Columns("4").Width = 10;
                    ws.Columns("5").Width = 20;
                    ws.Columns("6").Width = 20;
                    ws.Columns("7").Width = 15;
                    ws.Columns("8").Width = 15;
                    ws.Columns("9").Width = 15;
                    ws.Columns("10").Width = 15;
                    ws.Columns("11").Width = 15;
                    ws.Columns("12").Width = 15;
                    ws.Columns("13").Width = 15;
                    ws.Columns("14").Width = 15;
                    ws.Columns("15").Width = 15;
                    ws.Columns("16").Width = 15;
                    ws.Columns("17").Width = 15;
                    ws.Columns("18").Width = 15;

                    //First Names   
                    ws.Cell("A1").Value = "Inquiry Suzuki MSI V2 by " + DataSource;
                    ws.Cell("A1").Style.Font.SetBold().Font.SetFontSize(14);
                    ws.Cell("A2").Value = "Year";
                    ws.Cell("D2").Value = "Date";
                    ws.Cell("A3").Value = "Area";
                    ws.Cell("A4").Value = "Dealer";
                    ws.Cell("A5").Value = "Outlet";

                    ws.Range("B2:C2").Merge();
                    ws.Cell("B2").Value = Year;
                    ws.Cell("E2").Value = DateTime.Now.ToString();
                    ws.Cell("E2").Style.DateFormat.Format = "DD-MMM-YYYY HH:mm";
                    ws.Range("B3:C3").Merge();
                    ws.Cell("B3").Value = area;
                    ws.Range("B4:C4").Merge();
                    ws.Cell("B4").Value = TextDealer;
                    ws.Range("B5:C5").Merge();
                    ws.Cell("B5").Value = TextOutlet;

                    ws.Cell("A7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("A7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("A7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("A7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("A7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("A7").Value = "MSI Group ";

                    ws.Cell("B7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    //ws.Cell("B7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("B7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("B7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("B7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    //ws.Cell("B7").Value = "No ";

                    //ws.Cell("C7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    //ws.Cell("C7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("C7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("C7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("C7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("C7").Value = "Description ";

                    //ws.Cell("D7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    //ws.Cell("D7").Value = "Unit ";

                    ws.Cell("E7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("E7").Value = "Average/Month ";

                    ws.Cell("F7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("F7").Value = "Total ";

                    ws.Cell("G7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("G7").Value = "Jan ";

                    ws.Cell("H7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("H7").Value = "Feb ";

                    ws.Cell("I7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("I7").Value = "Mar ";

                    ws.Cell("J7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("J7").Value = "Apr ";

                    ws.Cell("K7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("K7").Value = "May ";

                    ws.Cell("L7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("L7").Value = "Jun ";

                    ws.Cell("M7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("M7").Value = "Jul ";

                    ws.Cell("N7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("N7").Value = "Aug ";

                    ws.Cell("O7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("O7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("O7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("O7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("O7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("O7").Value = "Sep ";

                    ws.Cell("P7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("P7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("P7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("P7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("P7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("P7").Value = "Oct ";

                    ws.Cell("Q7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Q7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Q7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Q7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Q7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("Q7").Value = "Nov ";

                    ws.Cell("R7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("R7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("R7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("R7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("R7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("R7").Value = "Dec ";

                    #endregion

                    #region ** field values summary **
                    foreach (var row in dt2.Rows)
                    {
                        msiGroupName = ((System.Data.DataRow)(row)).ItemArray[2].ToString();
                        if (msiGroupNameCheck != msiGroupName)
                        {
                            indexRow = 0;
                            index = index + 1;
                            if (index == 1)
                            {
                                indexRow = indexRow + (35 - 1);
                            }
                            if (index == 2)
                            {
                                indexRow = indexRow + (9 - 1);
                            }
                            if (index == 3)
                            {
                                indexRow = indexRow + (49 - 1);
                            }

                            if (index == 4)
                            {
                                indexRow = indexRow + (12 - 1);
                            }
                            if (index == 5)
                            {
                                indexRow = indexRow + (13 - 1);
                            }
                            if (index == 6)
                            {
                                indexRow = indexRow + (8 - 1);
                            }
                            if (index == 7)
                            {
                                indexRow = indexRow + (1 - 1);
                            }
                            if (index == 8)
                            {
                                indexRow = indexRow + (12 - 1);
                            }

                            //MSI GROUP
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Merge();
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Value = ((System.Data.DataRow)(row)).ItemArray[2];
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;


                        }

                        //NO
                        ws.Cell("B" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        //ws.Cell("B" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("B" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("B" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("B" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[1];
                        ws.Cell("B" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        //MSI DESC
                        //ws.Cell("C" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        //ws.Cell("C" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("C" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("C" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("C" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[3];
                        ws.Cell("C" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        //UNIT
                        //ws.Cell("D" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("D" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("D" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("D" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("D" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[4];
                        ws.Cell("D" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        //AVERAGE
                        ws.Cell("E" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("E" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("E" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("E" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("E" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[5];
                        ws.Cell("E" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("E" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //TOTAL
                        ws.Cell("F" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("F" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("F" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("F" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("F" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[6];
                        ws.Cell("F" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("F" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH01
                        ws.Cell("G" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("G" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("G" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("G" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("G" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[7];
                        ws.Cell("G" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("G" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH02
                        ws.Cell("H" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("H" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("H" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("H" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("H" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[8];
                        ws.Cell("H" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("H" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH03
                        ws.Cell("I" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("I" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("I" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("I" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("I" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[9];
                        ws.Cell("I" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("I" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH04
                        ws.Cell("J" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("J" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("J" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("J" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("J" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[10];
                        ws.Cell("J" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("J" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH05
                        ws.Cell("K" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("K" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("K" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("K" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("K" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[11];
                        ws.Cell("K" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("K" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH06
                        ws.Cell("L" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("L" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("L" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("L" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("L" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[12];
                        ws.Cell("L" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("L" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH07
                        ws.Cell("M" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("M" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("M" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("M" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("M" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[13];
                        ws.Cell("M" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("M" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH08
                        ws.Cell("N" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("N" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("N" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("N" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("N" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[14];
                        ws.Cell("N" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("N" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH09
                        ws.Cell("O" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("O" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("O" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("O" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("O" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[15];
                        ws.Cell("O" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("O" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH10
                        ws.Cell("P" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("P" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("P" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("P" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("P" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[16];
                        ws.Cell("P" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("P" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH11
                        ws.Cell("Q" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("Q" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("Q" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("Q" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("Q" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[17];
                        ws.Cell("Q" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("Q" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH12
                        ws.Cell("R" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("R" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("R" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("R" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("R" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[18];
                        ws.Cell("R" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("R" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        msiGroupNameCheck = msiGroupName;
                        lastRow++;
                    }

                    #endregion

                    #region ** write detail **

                    lastRow = 8;
                    index = 0;
                    indexRow = 0;

                    foreach (var row in dt1.Rows)
                    {
                        #region ** write header detail **

                        sheetNameCheck = ((System.Data.DataRow)(row)).ItemArray[1].ToString();
                        dealerCode = ((System.Data.DataRow)(row)).ItemArray[0].ToString();
                        if (sheetName != sheetNameCheck)
                        {
                            lastRow = 8;
                            index = 0;
                            indexRow = 0;

                            ws = wb.Worksheets.Add(sheetNameCheck);
                            sheetName = sheetNameCheck;
                            hdrTable = ws.Range("A1:U7");
                            hdrTable.Style
                        .Font.SetBold()
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                        .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                            rngTable = ws.Range("A7:U7");
                            rngTable.Style
                                .Font.SetBold()
                                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                .Alignment.SetWrapText();

                            rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            rngTable.Style.Font.Bold = true;

                            ws.Columns("1").Width = 40;
                            ws.Columns("2").Width = 5;
                            ws.Columns("3").Width = 90;
                            ws.Columns("4").Width = 10;
                            ws.Columns("5").Width = 20;
                            ws.Columns("6").Width = 20;
                            ws.Columns("7").Width = 15;
                            ws.Columns("8").Width = 15;
                            ws.Columns("9").Width = 15;
                            ws.Columns("10").Width = 15;
                            ws.Columns("11").Width = 15;
                            ws.Columns("12").Width = 15;
                            ws.Columns("13").Width = 15;
                            ws.Columns("14").Width = 15;
                            ws.Columns("15").Width = 15;
                            ws.Columns("16").Width = 15;
                            ws.Columns("17").Width = 15;
                            ws.Columns("18").Width = 15;

                            //First Names   
                            ws.Cell("A1").Value = "Inquiry Suzuki MSI V2";
                            ws.Cell("A1").Style.Font.SetBold().Font.SetFontSize(14);
                            ws.Cell("A2").Value = "Year";
                            ws.Cell("D2").Value = "Date";
                            ws.Cell("A3").Value = "Area";
                            ws.Cell("A4").Value = "Dealer";
                            ws.Cell("A5").Value = "Outlet";

                            ws.Range("B2:C2").Merge();
                            ws.Cell("B2").Value = Year;
                            ws.Cell("E2").Value = DateTime.Now.ToString();
                            ws.Cell("E2").Style.DateFormat.Format = "DD-MMM-YYYY HH:mm";
                            ws.Range("B3:C3").Merge();
                            ws.Cell("B3").Value = area;
                            ws.Range("B4:C4").Merge();
                            ws.Cell("B4").Value = dealerCode + " - " + ((System.Data.DataRow)(row)).ItemArray[21];
                            ws.Range("B5:C5").Merge();
                            ws.Cell("B5").Value = sheetName + " - " + ((System.Data.DataRow)(row)).ItemArray[22];

                            ws.Cell("A7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell("A7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell("A7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell("A7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Cell("A7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                            ws.Cell("A7").Value = "MSI Group ";

                            ws.Cell("B7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            //ws.Cell("B7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell("B7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell("B7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Cell("B7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                            //ws.Cell("B7").Value = "No ";

                            //ws.Cell("C7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            //ws.Cell("C7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell("C7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell("C7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Cell("C7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                            ws.Cell("C7").Value = "Description ";

                            //ws.Cell("D7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell("D7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell("D7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell("D7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Cell("D7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                            //ws.Cell("D7").Value = "Unit ";

                            ws.Cell("E7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell("E7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell("E7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell("E7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Cell("E7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                            ws.Cell("E7").Value = "Average/Month ";

                            ws.Cell("F7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell("F7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell("F7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell("F7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Cell("F7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                            ws.Cell("F7").Value = "Total ";

                            ws.Cell("G7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell("G7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell("G7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell("G7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Cell("G7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                            ws.Cell("G7").Value = "Jan ";

                            ws.Cell("H7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell("H7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell("H7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell("H7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Cell("H7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                            ws.Cell("H7").Value = "Feb ";

                            ws.Cell("I7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell("I7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell("I7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell("I7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Cell("I7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                            ws.Cell("I7").Value = "Mar ";

                            ws.Cell("J7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell("J7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell("J7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell("J7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Cell("J7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                            ws.Cell("J7").Value = "Apr ";

                            ws.Cell("K7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell("K7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell("K7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell("K7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Cell("K7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                            ws.Cell("K7").Value = "May ";

                            ws.Cell("L7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell("L7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell("L7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell("L7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Cell("L7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                            ws.Cell("L7").Value = "Jun ";

                            ws.Cell("M7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell("M7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell("M7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell("M7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Cell("M7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                            ws.Cell("M7").Value = "Jul ";

                            ws.Cell("N7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell("N7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell("N7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell("N7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Cell("N7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                            ws.Cell("N7").Value = "Aug ";

                            ws.Cell("O7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell("O7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell("O7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell("O7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Cell("O7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                            ws.Cell("O7").Value = "Sep ";

                            ws.Cell("P7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell("P7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell("P7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell("P7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Cell("P7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                            ws.Cell("P7").Value = "Oct ";

                            ws.Cell("Q7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell("Q7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell("Q7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell("Q7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Cell("Q7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                            ws.Cell("Q7").Value = "Nov ";

                            ws.Cell("R7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell("R7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell("R7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell("R7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Cell("R7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                            ws.Cell("R7").Value = "Dec ";

                        }
                        #endregion

                        #region** field values **

                        msiGroupName = ((System.Data.DataRow)(row)).ItemArray[4].ToString();
                        if (msiGroupNameCheck != msiGroupName)
                        {
                            indexRow = 0;
                            index = index + 1;
                            if (index == 1)
                            {
                                indexRow = indexRow + (35 - 1);
                            }
                            if (index == 2)
                            {
                                indexRow = indexRow + (9 - 1);
                            }
                            if (index == 3)
                            {
                                indexRow = indexRow + (49 - 1);
                            }

                            if (index == 4)
                            {
                                indexRow = indexRow + (12 - 1);
                            }
                            if (index == 5)
                            {
                                indexRow = indexRow + (13 - 1);
                            }
                            if (index == 6)
                            {
                                indexRow = indexRow + (8 - 1);
                            }
                            if (index == 7)
                            {
                                indexRow = indexRow + (1 - 1);
                            }
                            if (index == 8)
                            {
                                indexRow = indexRow + (12 - 1);
                            }

                            //MSI GROUP
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Merge();
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Value = ((System.Data.DataRow)(row)).ItemArray[4];
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;


                        }

                        //NO
                        ws.Cell("B" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        //ws.Cell("B" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("B" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("B" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("B" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[3];
                        ws.Cell("B" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        //MSI DESC
                        //ws.Cell("C" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        //ws.Cell("C" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("C" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("C" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("C" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[5];
                        ws.Cell("C" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        //UNIT
                        //ws.Cell("D" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("D" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("D" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("D" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("D" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[6];
                        ws.Cell("D" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        //AVERAGE
                        ws.Cell("E" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("E" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("E" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("E" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("E" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[7];
                        ws.Cell("E" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("E" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //TOTAL
                        ws.Cell("F" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("F" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("F" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("F" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("F" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[8];
                        ws.Cell("F" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("F" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH01
                        ws.Cell("G" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("G" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("G" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("G" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("G" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[9];
                        ws.Cell("G" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("G" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH02
                        ws.Cell("H" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("H" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("H" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("H" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("H" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[10];
                        ws.Cell("H" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("H" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH03
                        ws.Cell("I" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("I" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("I" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("I" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("I" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[11];
                        ws.Cell("I" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("I" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH04
                        ws.Cell("J" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("J" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("J" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("J" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("J" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[12];
                        ws.Cell("J" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("J" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH05
                        ws.Cell("K" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("K" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("K" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("K" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("K" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[13];
                        ws.Cell("K" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("K" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH06
                        ws.Cell("L" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("L" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("L" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("L" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("L" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[14];
                        ws.Cell("L" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("L" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH07
                        ws.Cell("M" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("M" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("M" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("M" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("M" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[15];
                        ws.Cell("M" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("M" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH08
                        ws.Cell("N" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("N" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("N" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("N" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("N" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[16];
                        ws.Cell("N" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("N" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH09
                        ws.Cell("O" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("O" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("O" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("O" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("O" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[17];
                        ws.Cell("O" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("O" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH10
                        ws.Cell("P" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("P" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("P" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("P" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("P" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[18];
                        ws.Cell("P" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("P" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH11
                        ws.Cell("Q" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("Q" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("Q" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("Q" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("Q" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[19];
                        ws.Cell("Q" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("Q" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH12
                        ws.Cell("R" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("R" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("R" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("R" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("R" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[20];
                        ws.Cell("R" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("R" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        msiGroupNameCheck = msiGroupName;
                        lastRow++;

                        #endregion

                    }

                    #endregion

                }

            }

            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
            return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));

        }

        //end
        public ActionResult SvMsiR2V2()
        {
            string comp = CompanyCode;
            string bran = BranchCode;
            string crYr = Convert.ToString(DateTime.Today.Year);
            string Year = string.IsNullOrWhiteSpace(Request["Year"]) ? crYr : Request["Year"];
            string dataSource = Request["DataSource"] ?? "";
            int month = Convert.ToInt32(Request["Month"]);
            string sp = "";
            string DataSource = Request["DataSource"];
            if (dataSource == "Invoice")
            {
                sp = "usprpt_SvRpReport021V3Web";
                //sp = "uspfn_DlrInqMsiV2ByBranch";
            }
            else
            {
                sp = "usprpt_SvRpReport021V3SPKWeb";
                //sp = "uspfn_DlrInqMsiV2ByBranch_SPK";
            }

            SqlCommand cmd1 = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd1.CommandTimeout = 1800;
            cmd1.CommandText = "uspfn_GetAreaDealerOutlet";

            cmd1.CommandType = CommandType.StoredProcedure;
            cmd1.Parameters.Clear();
            cmd1.Parameters.AddWithValue("@companyCode", CompanyCode);
            cmd1.Parameters.AddWithValue("@BranchCode", BranchCode);
            SqlDataAdapter ga = new SqlDataAdapter(cmd1);
            DataTable gt = new DataTable();

            ga.Fill(gt);

            int lastRow = 1;
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("MSI DATA");
            DateTime now = DateTime.Now;
            string fileName = "MSI DATA V2_" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");

            ws.Column(1).Width = 3;
            ws.Row(1).Height = 60;
            ws.Range("A1", "R1").Merge();
            ws.Cell("A" + lastRow).Value = "Major Service Indicators (MSI) - Motorcycle 2W";
            ws.Cell("A" + lastRow).Style.Fill.SetBackgroundColor(XLColor.FromArgb(55, 96, 145));
            ws.Cell("A" + lastRow).Style.Font.Bold = true;
            ws.Cell("A" + lastRow).Style.Font.SetFontColor(XLColor.White);

            //ws.Cell("B" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
            lastRow = 3;
            ws.Cell("B" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
            ws.Column(2).Width = 30;
            ws.Column(3).Width = 30;
            ws.Row(2).Height = 13;
            ws.Cell("B" + lastRow).Value = "Kode Dealer SIS ";
            ws.Cell("C" + lastRow).Value = ": " + CompanyCode;

            ws.Cell("E" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
            ws.Cell("E" + lastRow).Value = "Kota ";
            ws.Cell("F" + lastRow).Value = ": " + gt.Rows[0]["CITY"];

            lastRow = 4;
            ws.Cell("B" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
            ws.Cell("B" + lastRow).Value = "Dealer Name ";
            ws.Cell("C" + lastRow).Value = ": " + gt.Rows[0]["Dealer"];

            ws.Cell("E" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
            ws.Cell("E" + lastRow).Value = "Region ";
            ws.Cell("F" + lastRow).Value = ": " + gt.Rows[0]["Area"];

            lastRow = 5;
            ws.Cell("B" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
            ws.Cell("B" + lastRow).Value = "Outlet Name ";
            ws.Cell("C" + lastRow).Value = ": " + gt.Rows[0]["Showroom"];

            ws.Cell("E" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
            ws.Cell("E" + lastRow).Value = "Status Dealer ";
            ws.Cell("F" + lastRow).Value = ": " + gt.Rows[0]["StatusDealer"];

            lastRow = 6;
            ws.Cell("B" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
            ws.Cell("B" + lastRow).Value = "Alamat ";
            ws.Cell("C" + lastRow).Value = ": " + gt.Rows[0]["Address1"];

            ws.Cell("E" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
            ws.Cell("E" + lastRow).Value = "Tahun ";
            ws.Cell("F" + lastRow).Value = ": " + Year;

            ws.Cell("P" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
            ws.Cell("P" + lastRow).Value = "Print Date ";
            ws.Cell("Q" + lastRow).Value = ": " + String.Format("{0:g}", DateTime.Now);
            lastRow = 8;

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = sp;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 360;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", comp);
            cmd.Parameters.AddWithValue("@BranchCode", bran);
            cmd.Parameters.AddWithValue("@ProductType", ProductType);
            cmd.Parameters.AddWithValue("@PeriodYear", Year);
            cmd.Parameters.AddWithValue("@Month2", month);
            cmd.Parameters.AddWithValue("@UserId", CurrentUser.UserId);
            cmd.Parameters.AddWithValue("@istype", 1);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return GenerateExcelR2V2(wb, dt, lastRow, fileName);
        }

        private ActionResult GenerateExcelR2V2(XLWorkbook wb, DataTable dt, int lastRow, string fileName, bool isCustomHeader = false, bool isShowSummary = false)
        {
            var ws = wb.Worksheet(1);
            var tmpLastRow = lastRow;
            var rngTable = ws.Range(8, 1, 170, 19);
            rngTable.Style
                .Border.SetTopBorder(XLBorderStyleValues.Thin)
                .Border.SetBottomBorder(XLBorderStyleValues.Thin)
                .Border.SetLeftBorder(XLBorderStyleValues.Thin)
                .Border.SetRightBorder(XLBorderStyleValues.Thin)
                .Border.SetTopBorderColor(XLColor.FromArgb(55, 96, 145))
                .Border.SetBottomBorderColor(XLColor.FromArgb(55, 96, 145))
                .Border.SetLeftBorderColor(XLColor.FromArgb(55, 96, 145))
                .Border.SetRightBorderColor(XLColor.FromArgb(55, 96, 145));
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
                        ws.Cell(lastRow, iCol).Style.Fill.SetBackgroundColor(XLColor.White).Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        //ws.Cell(lastRow, iCol).Style.Font.SetBold().Font.SetFontSize(14);

                        switch (lastRow)
                        {
                            case 8:
                                if (iCol == 2)
                                {
                                    ws.Cell(lastRow + 1, 2).Value = "A. PENJUALAN";
                                    ws.Cell(lastRow + 1, 2).Style.Font.Bold = true;
                                    //ws.Cell(lastRow + 1, 2).Style.Font.SetFontSize(12);
                                    ws.Range('B' + (lastRow).ToString(), 'C' + (lastRow).ToString()).Merge().Style.Font.SetBold();
                                    ws.Range('B' + (lastRow + 1).ToString(), 'R' + (lastRow + 1).ToString()).Merge();
                                }
                                if (iCol == 3)
                                {
                                    ws.Cell(lastRow + 2, iCol - 1).Style.Font.SetFontSize(10);
                                    ws.Cell(lastRow + 2, iCol - 1).Value = val;
                                    ws.Range('B' + (lastRow + 2).ToString(), 'C' + (lastRow + 2).ToString()).Merge();
                                }
                                else
                                {
                                    ws.Cell(lastRow + 2, iCol).Style.Font.SetFontSize(10); // no and count of month
                                    ws.Cell(lastRow + 2, iCol).Value = val;
                                    if (iCol == 6 || iCol == 7)
                                    {
                                        ws.Cell(lastRow + 2, iCol).Style.Fill.SetBackgroundColor(XLColor.FromArgb(55, 96, 145));
                                        ws.Cell(lastRow + 2, iCol).Style.Font.SetFontColor(XLColor.White)
                                            .Border.SetTopBorderColor(XLColor.White)
                                                .Border.SetBottomBorderColor(XLColor.White)
                                                .Border.SetLeftBorderColor(XLColor.White)
                                                .Border.SetRightBorderColor(XLColor.White);
                                    }
                                }


                                break;
                            default:
                                ws.Cell(lastRow + 2, iCol).Style.Font.SetFontSize(10);
                                ws.Cell(lastRow + 2, iCol).Value = val;
                                if (iCol == 6 || iCol == 7)
                                {
                                    ws.Cell(lastRow + 2, iCol).Style.Fill.SetBackgroundColor(XLColor.FromArgb(55, 96, 145));
                                    ws.Cell(lastRow + 2, iCol).Style.Font.SetFontColor(XLColor.White)
                                        .Border.SetTopBorderColor(XLColor.White)
                                                .Border.SetBottomBorderColor(XLColor.White)
                                                .Border.SetLeftBorderColor(XLColor.White)
                                                .Border.SetRightBorderColor(XLColor.White);
                                }
                                break;
                        };
                    }
                    else
                    {
                        if (tmpLastRow != lastRow)
                        {
                            switch (lastRow)
                            {
                                case 47:
                                    if (iCol == 2)
                                    {
                                        ws.Cell(lastRow, 2).Value = "B. UNIT SERVIS";
                                        ws.Range('B' + (lastRow).ToString(), 'R' + (lastRow).ToString()).Merge().Style.Font.SetBold();
                                        //ws.Cell(lastRow, 2).Style.Font.SetFontSize(12);
                                    }
                                    if (iCol == 3)
                                    {
                                        ws.Cell(lastRow + 1, iCol - 1).Style.Font.SetFontSize(10);
                                        ws.Cell(lastRow + 1, iCol - 1).Value = val;
                                        ws.Range('B' + (lastRow + 1).ToString(), 'C' + (lastRow + 1).ToString()).Merge().Style.Font.SetBold();
                                    }
                                    else
                                    {
                                        ws.Cell(lastRow + 1, iCol).Style.Font.SetFontSize(10);
                                        ws.Cell(lastRow + 1, iCol).Value = val;
                                        if (iCol == 6 || iCol == 7)
                                        {
                                            ws.Cell(lastRow + 1, iCol).Style.Fill.SetBackgroundColor(XLColor.FromArgb(55, 96, 145));
                                            ws.Cell(lastRow + 1, iCol).Style.Font.SetFontColor(XLColor.White)
                                                .Border.SetTopBorderColor(XLColor.White)
                                                .Border.SetBottomBorderColor(XLColor.White)
                                                .Border.SetLeftBorderColor(XLColor.White)
                                                .Border.SetRightBorderColor(XLColor.White);
                                        }
                                    }
                                    break;
                                case 59:
                                    if (iCol == 2)
                                    {
                                        ws.Cell(lastRow, 2).Value = "C. JENIS PEKERJAAN";
                                        ws.Range('B' + (lastRow).ToString(), 'R' + (lastRow).ToString()).Merge().Style.Font.SetBold();
                                        //ws.Cell(lastRow, 2).Style.Font.SetFontSize(12);
                                    }
                                    if (iCol == 3)
                                    {
                                        ws.Cell(lastRow + 1, iCol - 1).Style.Font.SetFontSize(10);
                                        ws.Cell(lastRow + 1, iCol - 1).Value = val;
                                        ws.Range('B' + (lastRow + 1).ToString(), 'C' + (lastRow + 1).ToString()).Merge().Style.Font.SetBold();
                                    }
                                    else
                                    {
                                        ws.Cell(lastRow + 1, iCol).Style.Font.SetFontSize(10);
                                        ws.Cell(lastRow + 1, iCol).Value = val;
                                    }
                                    break;
                                case 111:
                                    if (iCol == 2)
                                    {
                                        ws.Cell(lastRow, 2).Value = "D. KEKUATAN BENGKEL SERVIS";
                                        ws.Range('B' + (lastRow).ToString(), 'R' + (lastRow).ToString()).Merge().Style.Font.SetBold();
                                        //ws.Cell(lastRow, 2).Style.Font.SetFontSize(12);
                                    }
                                    if (iCol == 3)
                                    {
                                        ws.Cell(lastRow + 1, iCol - 1).Style.Font.SetFontSize(10);
                                        ws.Cell(lastRow + 1, iCol - 1).Value = val;
                                        ws.Range('B' + (lastRow + 1).ToString(), 'C' + (lastRow + 1).ToString()).Merge().Style.Font.SetBold();
                                    }
                                    else
                                    {
                                        ws.Cell(lastRow + 1, iCol).Style.Font.SetFontSize(10);
                                        ws.Cell(lastRow + 1, iCol).Value = val;
                                    }
                                    break;
                                case 126:
                                    if (iCol == 2)
                                    {
                                        ws.Cell(lastRow, 2).Value = "E. INDIKATOR-INDIKATOR PRODUKTIVITAS";
                                        ws.Range('B' + (lastRow).ToString(), 'R' + (lastRow).ToString()).Merge().Style.Font.SetBold();
                                        //ws.Cell(lastRow, 2).Style.Font.SetFontSize(12);
                                    }
                                    if (iCol == 3)
                                    {
                                        ws.Cell(lastRow + 1, iCol - 1).Style.Font.SetFontSize(10);
                                        ws.Cell(lastRow + 1, iCol - 1).Value = val;
                                        ws.Range('B' + (lastRow + 1).ToString(), 'C' + (lastRow + 1).ToString()).Merge().Style.Font.SetBold();
                                    }
                                    else
                                    {
                                        ws.Cell(lastRow + 1, iCol).Style.Font.SetFontSize(10);
                                        ws.Cell(lastRow + 1, iCol).Value = val;
                                    }
                                    break;
                                case 142:
                                    if (iCol == 2)
                                    {
                                        ws.Cell(lastRow, 2).Value = "F. RETENSI SERVIS & AKTIVITAS MARKETING";
                                        ws.Range('B' + (lastRow).ToString(), 'R' + (lastRow).ToString()).Merge().Style.Font.SetBold();
                                        //ws.Cell(lastRow, 2).Style.Font.SetFontSize(12);
                                    }
                                    if (iCol == 3)
                                    {
                                        ws.Cell(lastRow + 1, iCol - 1).Style.Font.SetFontSize(10);
                                        ws.Cell(lastRow + 1, iCol - 1).Value = val;
                                        ws.Range('B' + (lastRow + 1).ToString(), 'C' + (lastRow + 1).ToString()).Merge().Style.Font.SetBold();
                                    }
                                    else
                                    {
                                        ws.Cell(lastRow + 1, iCol).Style.Font.SetFontSize(10);
                                        ws.Cell(lastRow + 1, iCol).Value = val;
                                        if (iCol == 6 || iCol == 7)
                                        {
                                            ws.Cell(lastRow + 1, iCol).Style.Fill.SetBackgroundColor(XLColor.FromArgb(55, 96, 145));
                                            ws.Cell(lastRow + 1, iCol).Style.Font.SetFontColor(XLColor.White)
                                                .Border.SetTopBorderColor(XLColor.White)
                                                .Border.SetBottomBorderColor(XLColor.White)
                                                .Border.SetLeftBorderColor(XLColor.White)
                                                .Border.SetRightBorderColor(XLColor.White);
                                        }
                                    }
                                    break;
                                case 153:
                                    if (iCol == 2)
                                    {
                                        ws.Cell(lastRow, 2).Value = "G. PENJUALAN UNIT SEPEDA MOTOR";
                                        ws.Range('B' + (lastRow).ToString(), 'R' + (lastRow).ToString()).Merge().Style.Font.SetBold();
                                        //ws.Cell(lastRow, 2).Style.Font.SetFontSize(12);
                                    }
                                    if (iCol == 3)
                                    {
                                        ws.Cell(lastRow + 1, iCol - 1).Style.Font.SetFontSize(10);
                                        ws.Cell(lastRow + 1, iCol - 1).Value = val;
                                        ws.Range('B' + (lastRow + 1).ToString(), 'C' + (lastRow + 1).ToString()).Merge().Style.Font.SetBold();
                                    }
                                    else
                                    {
                                        ws.Cell(lastRow + 1, iCol).Style.Font.SetFontSize(10);
                                        ws.Cell(lastRow + 1, iCol).Value = val;
                                        if (iCol == 6 || iCol == 7)
                                        {
                                            ws.Cell(lastRow + 1, iCol).Style.Fill.SetBackgroundColor(XLColor.FromArgb(55, 96, 145));
                                            ws.Cell(lastRow + 1, iCol).Style.Font.SetFontColor(XLColor.White)
                                                .Border.SetTopBorderColor(XLColor.White)
                                                .Border.SetBottomBorderColor(XLColor.White)
                                                .Border.SetLeftBorderColor(XLColor.White)
                                                .Border.SetRightBorderColor(XLColor.White);
                                        }
                                    }
                                    break;
                                case 157:
                                    if (iCol == 2)
                                    {
                                        ws.Cell(lastRow, 2).Value = "H. PERFORMA CS INDEKS";
                                        ws.Range('B' + (lastRow).ToString(), 'R' + (lastRow).ToString()).Merge().Style.Font.SetBold();
                                        //ws.Cell(lastRow, 2).Style.Font.SetFontSize(12);
                                    }
                                    if (iCol == 3)
                                    {
                                        ws.Cell(lastRow + 1, iCol - 1).Style.Font.SetFontSize(10);
                                        ws.Cell(lastRow + 1, iCol - 1).Value = val;
                                        ws.Range('B' + (lastRow + 1).ToString(), 'C' + (lastRow + 1).ToString()).Merge().Style.Font.SetBold();
                                    }
                                    else
                                    {
                                        ws.Cell(lastRow + 1, iCol).Style.Font.SetFontSize(10);
                                        ws.Cell(lastRow + 1, iCol).Value = val;
                                        if (iCol == 6 || iCol == 7)
                                        {
                                            ws.Cell(lastRow + 1, iCol).Style.Fill.SetBackgroundColor(XLColor.FromArgb(55, 96, 145));
                                            ws.Cell(lastRow + 1, iCol).Style.Font.SetFontColor(XLColor.White)
                                                .Border.SetTopBorderColor(XLColor.White)
                                                .Border.SetBottomBorderColor(XLColor.White)
                                                .Border.SetLeftBorderColor(XLColor.White)
                                                .Border.SetRightBorderColor(XLColor.White);
                                        }
                                    }
                                    break;
                                default:
                                    if (iCol == 3)
                                    {
                                        ws.Cell(lastRow + 1, iCol - 1).Style.Font.SetFontSize(10);
                                        ws.Cell(lastRow + 1, iCol - 1).Value = val;
                                        ws.Range('B' + (lastRow + 1).ToString(), 'C' + (lastRow + 1).ToString()).Merge();
                                    }
                                    else if (iCol != 2)
                                    {
                                        ws.Cell(lastRow + 1, iCol).Style.Font.SetFontSize(10);
                                        ws.Cell(lastRow + 1, iCol).Value = val;
                                        if (iCol == 6 || iCol == 7)
                                        {
                                            ws.Cell(lastRow + 1, iCol).Style.Fill.SetBackgroundColor(XLColor.FromArgb(55, 96, 145));
                                            ws.Cell(lastRow + 1, iCol).Style.Font.SetFontColor(XLColor.White)
                                                .Border.SetTopBorderColor(XLColor.White)
                                                .Border.SetBottomBorderColor(XLColor.White)
                                                .Border.SetLeftBorderColor(XLColor.White)
                                                .Border.SetRightBorderColor(XLColor.White);
                                        }
                                    }
                                    break;
                            };

                        }

                    }
                    iCol++;
                }
                switch (lastRow)
                {
                    case 8:
                        lastRow++; lastRow++;
                        break;
                    case 43:
                        lastRow++; lastRow++; lastRow++; lastRow++;
                        break;
                    case 55:
                        lastRow++; lastRow++; lastRow++; lastRow++;
                        break;
                    case 107:
                        lastRow++; lastRow++; lastRow++; lastRow++;
                        break;
                    case 122:
                        lastRow++; lastRow++; lastRow++; lastRow++;
                        break;
                    case 138:
                        lastRow++; lastRow++; lastRow++; lastRow++;
                        break;
                    case 149:
                        lastRow++; lastRow++; lastRow++; lastRow++;
                        break;
                    case 153:
                        lastRow++; lastRow++; lastRow++; lastRow++;
                        break;
                    case 168:
                        lastRow++; lastRow++; lastRow++; lastRow++;
                        break;
                    default:
                        lastRow++;
                        break;
                };
            }

            ws.Range(11, 2, 13, 2).Style.Alignment.Indent = 1;
            ws.Range(15, 2, 17, 2).Style.Alignment.Indent = 1;
            ws.Range(19, 2, 21, 2).Style.Alignment.Indent = 1;
            ws.Range(23, 2, 24, 2).Style.Alignment.Indent = 1;
            ws.Range(26, 2, 27, 2).Style.Alignment.Indent = 1;
            ws.Range(29, 2, 31, 2).Style.Alignment.Indent = 1;
            ws.Range(50, 2, 52, 2).Style.Alignment.Indent = 1;
            ws.Range(62, 2, 64, 2).Style.Alignment.Indent = 1;
            ws.Range(65, 2, 70, 2).Style.Alignment.Indent = 1;
            ws.Range(72, 2, 78, 2).Style.Alignment.Indent = 1;
            ws.Range(80, 2, 89, 2).Style.Alignment.Indent = 1;
            ws.Range(92, 2, 98, 2).Style.Alignment.Indent = 1;
            ws.Range(100, 2, 104, 2).Style.Alignment.Indent = 1;
            ws.Range(106, 2, 108, 2).Style.Alignment.Indent = 1;
            ws.Range(113, 2, 114, 2).Style.Alignment.Indent = 1;
            ws.Range(116, 2, 119, 2).Style.Alignment.Indent = 1;
            ws.Range(121, 2, 123, 2).Style.Alignment.Indent = 1;
            ws.Range(128, 2, 128, 2).Style.Alignment.Indent = 1;
            ws.Range(135, 2, 135, 2).Style.Alignment.Indent = 1;
            ws.Range(162, 2, 163, 2).Style.Alignment.Indent = 1;
            ws.Range(167, 2, 169, 2).Style.Alignment.Indent = 1;



            //Border none and thick

            int[] Brdr = new int[] { 9, 47, 59, 111, 126, 142, 153, 157 };
            foreach (var c in Brdr)
            {
                ws.Cell(c, 1).Style.Border.SetRightBorder(XLBorderStyleValues.None);
            }

            //ws.Range(170, 1, 170, 17).Style.Border.SetBottomBorder(XLBorderStyleValues.Thick);

            ws.Columns().Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            ws.Columns(4, 19).Width = 15;
            //color
            //ws.Columns().Style.Fill.SetBackgroundColor(XLColor.FromArgb(255, 255, 255));
            //ws.Rows().Style.Fill.SetBackgroundColor(XLColor.FromArgb(255, 255, 255));
            ws.Range(8, 1, 8, 1).Style.Fill.SetBackgroundColor(XLColor.FromArgb(55, 96, 145));
            ws.Range(8, 1, 8, 1).Style.Font.SetFontColor(XLColor.White)
                .Border.SetTopBorderColor(XLColor.White)
                .Border.SetBottomBorderColor(XLColor.White)
                .Border.SetLeftBorderColor(XLColor.White)
                .Border.SetRightBorderColor(XLColor.White);
            ws.Range(8, 1, 8, 1).Style.Font.Bold = true;

            int[] clls = new int[] { 45, 57, 109, 124, 140, 151, 155, 170 };
            foreach (var d in clls)
            {
                ws.Range('A' + (d).ToString(), 'R' + (d).ToString()).Merge();

                ws.Range(d + 1, 1, d + 1, 19).Style.Border.SetLeftBorder(XLBorderStyleValues.None);
                ws.Range(d + 1, 1, d + 1, 19).Style.Border.SetRightBorder(XLBorderStyleValues.None);
                ws.Range(d + 1, 1, d + 1, 19).Style.Border.SetTopBorder(XLBorderStyleValues.Thin).Border.SetTopBorderColor(XLColor.FromArgb(55, 96, 145));
                ws.Range(d + 1, 1, d + 1, 19).Style.Border.SetBottomBorder(XLBorderStyleValues.Thin).Border.SetTopBorderColor(XLColor.FromArgb(55, 96, 145));

                if (d == 170)
                {
                    ws.Range(d + 1, 1, d + 1, 19).Style.Border.SetLeftBorder(XLBorderStyleValues.None);
                    ws.Range(d + 1, 1, d + 1, 19).Style.Border.SetRightBorder(XLBorderStyleValues.None);
                    ws.Range(d + 1, 1, d + 1, 19).Style.Border.SetTopBorder(XLBorderStyleValues.Thin).Border.SetTopBorderColor(XLColor.Blue);
                    ws.Range(d + 1, 1, d + 1, 19).Style.Border.SetBottomBorder(XLBorderStyleValues.None).Border.SetTopBorderColor(XLColor.Blue);
                }
            }
            int[] cll = new int[] { 8, 10, 14, 18, 22, 25, 28, 32, 33, 34, 35, 38, 40, 41, 42, 43, 44, 49, 60, 61, 62, 63, 64, 71, 79, 90, 91, 99, 105, 112, 115, 120, 127, 128, 129, 130, 131, 132, 134, 135, 136, 137, 138, 139, 148, 149, 150, 160, 161, 167, 168, 169 };
            foreach (var c in cll)
            {
                ws.Range(c, 2, c, 19).Style.Fill.SetBackgroundColor(XLColor.FromArgb(55, 96, 145));
                ws.Range(c, 2, c, 19).Style.Font.Bold = true;
                ws.Range(c, 2, c, 19).Style.Font.SetFontColor(XLColor.White)
                .Border.SetTopBorderColor(XLColor.White)
                .Border.SetBottomBorderColor(XLColor.White)
                .Border.SetLeftBorderColor(XLColor.White)
                .Border.SetRightBorderColor(XLColor.White);
            }

            int[] cell = new int[] { 143, 144, 145, 146, 147 };
            foreach (var c in cell)
            {
                ws.Range(c, 5, c, 5).Style.Fill.SetBackgroundColor(XLColor.FromArgb(55, 96, 145));
                ws.Range(c, 5, c, 5).Style.Font.Bold = true;
                ws.Range(c, 5, c, 5).Style.Font.SetFontColor(XLColor.White)
                .Border.SetTopBorderColor(XLColor.White)
                .Border.SetBottomBorderColor(XLColor.White)
                .Border.SetLeftBorderColor(XLColor.White)
                .Border.SetRightBorderColor(XLColor.White);
            }

            int[] call = new int[] { 11, 49, 112, 133 };
            foreach (var c in call)
            {
                ws.Range(c, 8, c, 19).Style.Fill.SetBackgroundColor(XLColor.Red);
                ws.Range(c, 8, c, 19).Style.Font.Bold = true;
                ws.Range(c, 8, c, 19).Style.Font.SetFontColor(XLColor.White)
                .Border.SetTopBorderColor(XLColor.White)
                .Border.SetBottomBorderColor(XLColor.White)
                .Border.SetLeftBorderColor(XLColor.White)
                .Border.SetRightBorderColor(XLColor.White);
            }

            int[] coll = new int[] { 127, 128, 129, 130, 131, 132, 134, 135, 136, 137, 138, 139 };
            foreach (var c in coll)
            {
                ws.Cell(c, 7).Style.Fill.PatternType = XLFillPatternValues.DarkGray;
                ws.Cell(c, 7).Style.Fill.PatternColor = XLColor.Black;
                ws.Cell(c, 7).Style.Fill.PatternBackgroundColor = XLColor.FromArgb(55, 96, 145);
                ws.Cell(c, 7).Style.Font.Bold = true;
                ws.Cell(c, 7).Style.Font.SetFontColor(XLColor.White);
            }

            int[] xxx = new int[] { 158, 148, 149, 150, 160, 167, 168, 169 };
            foreach (var v in xxx)
            {
                ws.Cell(v, 7).Style.Fill.SetPatternType(XLFillPatternValues.DarkGray);
                ws.Cell(v, 7).Style.Fill.SetPatternColor(XLColor.Black);
                ws.Cell(v, 7).Style.Fill.SetPatternBackgroundColor(XLColor.FromArgb(219, 229, 241));
                ws.Cell(v, 7).Style.Font.Bold = true;
                ws.Cell(v, 7).Style.Font.SetFontColor(XLColor.White);
            }

            ws.Columns(1, 19).AdjustToContents();
            ws.Column(4).Delete();
            ws.Columns().Style.Font.SetFontName("Arial");
            ws.Columns().Style.Font.SetFontSize(10);
            ws.Cell("A1").Style.Font.SetFontSize(36);
            ws.Column(2).Width = 2.5;
            ws.Column(2).Width = 25;
            ws.Column(3).Width = 60;
            ws.Cell(109, 1).Value = "             *) Tanpa Kupon Servis gratis KSG";

            ws.Range(10, 5, 10, 19).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            ws.Range(10, 5, 10, 19).Style.NumberFormat.Format = "#,##0.0";

            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));

            return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
        }

    }
}
