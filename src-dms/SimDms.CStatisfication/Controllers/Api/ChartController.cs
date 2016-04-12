using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Spire.Xls;
//using Spire.Xls.Charts;
//using System.IO;
using ClosedXML.Excel;

namespace SimDms.CStatisfication.Controllers.Api
{
    public class ChartController : BaseController
    {
        public JsonResult CsMonitoring(string callback)
        {
            var json = Exec(new
            {
                query = "uspfn_CsChartMonitoring",
                param = new List<dynamic>
                {
                    new { key = "Inquiry", value = Request.Params["Inquiry"] },
                    new { key = "DateFrom", value = Request.Params["DateFrom"] },
                    new { key = "DateTo", value = Request.Params["DateTo"] },
                },
                result = "dataset"
            });
            return Json(json.Data);
        }

        public JsonResult CsMonitoring1(string callback)
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_CsChartMonitoring";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandTimeout = 3600;


            cmd.Parameters.Add(new SqlParameter("Inquiry", Request.Params["Inquiry"] ?? ""));
            cmd.Parameters.Add(new SqlParameter("DateFrom", Request.Params["DateFrom"] ?? ""));
            cmd.Parameters.Add(new SqlParameter("DateTo", Request.Params["DateTo"] ?? ""));

            var interval = Convert.ToInt64(Request.Params["Interval"] ?? "0");
            var datefrom = Convert.ToDateTime(Request.Params["DateFrom"] ?? "0");
            var dateto = Convert.ToDateTime(Request.Params["DateTo"] ?? "0");

            var cols = new List<string>();
            var series = new List<seriesModel>();
            var sourcedata = new List<int>();

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            if (ds.Tables.Count == 2 && ds.Tables[0].Rows.Count > 0)
            {
                var data = ds.Tables[0].AsEnumerable().ToList();
                var dlrs = ds.Tables[1];

                for (int i = 0; i < interval; i++)
                {
                    cols.Add(datefrom.AddDays(i).ToString("yyyy-MM-dd"));
                }

                for (int i = 0; i < dlrs.Rows.Count; i++)
                {
                    var seriesData = new seriesModel() { key = dlrs.Rows[i].ItemArray[0].ToString(), value = dlrs.Rows[i].ItemArray[1].ToString(), data = new List<int>() };
                    for (int j = 0; j < interval; j++)
                    {
                        var date = datefrom.AddDays(j).ToString("yyyy-MM-dd");
                        //Func<DataRow, bool> fn = new Func<DataRow, bool>(x => x.ItemArray[1].ToString() == seriesData.key && x.ItemArray[0].ToString() == date);

                        if (data.Count(x => x.ItemArray[1].ToString() == seriesData.key && x.ItemArray[0].ToString() == date) > 0)
                        {
                            var row = data.FirstOrDefault(x => x.ItemArray[1].ToString() == seriesData.key && x.ItemArray[0].ToString() == date).ItemArray;
                            seriesData.data.Add((int)row[2]);
                            sourcedata.Add((int)row[2]);
                        }
                        else
                        {
                            seriesData.data.Add(0);
                            sourcedata.Add(0);
                        }
                    }
                    series.Add(seriesData);
                }

            }
            //            if (result.length == 2 && result[0].length > 0)
            //            {
            //                var data = result[0];
            //                var dlrs = result[1];
            //                var source = { cols: [], series: [], data: []
            //    };

            //            for (var i = 0; i <= interval; i++) {
            //                source.cols.push(moment(filter.DateFrom, 'YYYY-MM-DD').add('days', i).format('YYYY-MM-DD'));
            //            }

            //dlrs.forEach(function(dlr)
            //{
            //    var series = { key: dlr.DealerCode, value: dlr.DealerName, data: [] };
            //                for (var i = 0; i <= interval; i++) {
            //                    var date = moment(filter.DateFrom, 'YYYY-MM-DD').add('days', i).format('YYYY-MM-DD');
            //var row = Enumerable
            //    .From(data)
            //    .Where('x => x["DealerCode"] == "' + series.key + '" && x["InputDate"] == "' + date + '"')
            //    .FirstOrDefault();
            //series.data.push((row == undefined) ? 0 : row.DataCount);

            //                    if (row) source.data.push(row.DataCount);
            //                }

            //                source.series.push(series);
            //            });

            //            generateChartBar(source);
            //        }

            return Json(new { cols = cols, series = series, data = sourcedata });
        }

        public JsonResult CsMonitoring2(string callback)
        {
            var cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600; 
            cmd.CommandText = "uspfn_CsChartMonitoring";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            cmd.Parameters.Add(new SqlParameter("Inquiry", Request.Params["Inquiry"] ?? ""));
            cmd.Parameters.Add(new SqlParameter("DateFrom", Request.Params["DateFrom"] ?? ""));
            cmd.Parameters.Add(new SqlParameter("DateTo", Request.Params["DateTo"] ?? ""));

            //var interval = Convert.ToInt64(Request.Params["Interval"] ?? "0");
            var datefrom = Convert.ToDateTime(Request.Params["DateFrom"] ?? "0");
            var dateto = Convert.ToDateTime(Request.Params["DateTo"] ?? "0");

            var cols = new List<string>();
            var series = new List<seriesModel2>();
            //var sourcedata = new List<int>();

            var da = new SqlDataAdapter(cmd);
            var ds = new DataSet();
            da.Fill(ds);

            //List<DataRow> data = null;

            if (ds.Tables.Count == 2 && ds.Tables[0].Rows.Count > 0)
            {
                var data = ds.Tables[0].AsEnumerable().ToList();
                var dlrs = ds.Tables[1].AsEnumerable();
                var interval = dateto.Day;
                for (int i = 0; i < interval; i++)
                {
                    cols.Add(datefrom.AddDays(i).ToString("yyyy-MM-dd"));
                }

                foreach (var dlr in dlrs)
                {
                    series.Add(new seriesModel2
                    {
                        key = dlr["DealerCode"].ToString(),
                        value = dlr["DealerName"].ToString(),
                        data = data.Where(x => x["DealerCode"].ToString() == dlr["DealerCode"].ToString())
                            .Select(x => new DataModel
                            {
                                Date = x["InputDate"].ToString(),
                                DataCount = (int)x["DataCount"]
                            }).ToList()
                    });
                }

            }
            return Json(new { cols = cols, series = series, data1 = GetJson(ds.Tables[0]) });
        }

        public JsonResult CsDataMonitoring(string callback)
        {
            var json = Exec(new
            {
                query = "uspfn_CsDataMonitoring",
                param = new List<dynamic>
                {
                    new { key = "DateInit", value = Request.Params["DateInit"] },
                    new { key = "DateReff", value = Request.Params["DateReff"] },
                    new { key = "Interval", value = Request.Params["Interval"] },
                },
                result = "dataset"
            });
            return Json(json.Data);
        }

        public JsonResult CsDataTDaysCallDO(string callback)
        {
            var json = Exec(new
            {
                query = "uspfn_CsDataTDaysCallDO",
                param = new List<dynamic>
                {
                    new { key = "BranchCode", value = Request.Params["BranchCode"] },
                    new { key = "Year", value = Request.Params["Year"] },
                    new { key = "Month", value = Request.Params["Month"] },
                },
                result = "dataset"
            });
            return Json(json.Data);
        }

        public JsonResult CsReportBPKBReminder() {
            var json = Exec(new
            {
                query = "uspfn_CsRptBPKBReminder",
                param = new List<dynamic>
                {
                    //new { key = "BranchCode", value = BranchCode },
                    new { key = "BranchCode", value = Request.Params["BranchCode"] },
                    new { key = "DateFrom", value = Request.Params["DateFrom"] },
                    new { key = "DateTo", value = Request.Params["DateTo"] },
                }
            });
            return Json(json.Data);
        }

        public JsonResult CsReportTDayCall()
        {
            var json = Exec(new
            {
                query = "uspfn_CsChartTDayCall",
                param = new List<dynamic>
                {
                    //new { key = "BranchCode", value = BranchCode },
                    new { key = "BranchCode", value = Request.Params["BranchCode"] },
                    new { key = "DateFrom", value = Request.Params["DateFrom"] },
                    new { key = "DateTo", value = Request.Params["DateTo"] },
                }
            });
            return Json(json.Data);
        }

        public JsonResult CsReportCustBirthday()
        {
            var json = Exec(new
            {
                query = "uspfn_GetMonitoringCustBirthday",
                param = new List<dynamic>
                {
                    //new { key = "BranchCode", value = BranchCode },
                    new { key = "BranchCode", value = Request.Params["BranchCode"] },
                    new { key = "PeriodYear", value = Request.Params["PeriodYear"] },
                    new { key = "ParMonth1", value = Request.Params["ParMonth1"] },
                    new { key = "ParMonth2", value = Request.Params["ParMonth2"] },
                    new { key = "ParStatus", value = Request.Params["ParStatus"] },
                }
            });
            return Json(json.Data);
        }

        public JsonResult CsReportSTNKExtention()
        {
            var json = Exec(new
            {
                query = "uspfn_CsChartStnkExt",
                param = new List<dynamic>
                {
                    new { key = "BranchCode", value = Request.Params["BranchCode"] },
                    new { key = "DateFrom", value = Request.Params["DateFrom"] },
                    new { key = "DateTo", value = Request.Params["DateTo"] },
                }
            });
            return Json(json.Data);
        }

        public ActionResult CsMonitoring1export()
        {
            DataSet ds = new DataSet();
            DataTable dt1 = new DataTable();
            DataTable dt2 = new DataTable();

            string Inquiry = Request["Inquiry"];
            string InquiryText = Request["InquiryText"];
            string DateFrom = Request["DateFrom"]+" 00:00:00";
            string DateTo = Request["DateTo"] + " 23:59:59";

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_CsChartMonitoringbydateexport";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 3600;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Inquiry", Inquiry);
            cmd.Parameters.AddWithValue("@DateFrom", DateFrom);
            cmd.Parameters.AddWithValue("@DateTo", DateTo);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            dt1 = ds.Tables[0];
            dt2 = ds.Tables[1];

            if (dt2.Rows.Count <=1)
            {
                return Json("Tidak ada data yang ditampilkan", "text/html", JsonRequestBehavior.AllowGet);
            }

            string inquiry = dt1.Rows[0][0].ToString(); 
            string periode = "";

            Inquiry = InquiryText;
            periode = (Convert.ToDateTime(DateFrom)).ToString("dd-MMM-yyyy") + " s/d " + (Convert.ToDateTime(DateTo)).ToString("dd-MMM-yyyy");


            var header = new List<List<dynamic>>() { };
            header.Add(new List<dynamic> { "Inquiry ", ": " + Inquiry });
            header.Add(new List<dynamic> { "Periode ", ": " + periode });

            //return GenerateReportXlsxNonTotal(dt2, "CsChartMonitoring", "CsChartMonitoring", header);
            return GenerateReportChart(dt2, "CsChartMonitoring", "CsChartMonitoring", header);
        }

        public ActionResult CsMonitoringbyperiodeexport()
        {
            DataSet ds = new DataSet();
            DataTable dt1 = new DataTable();
            DataTable dt2 = new DataTable();

            string Inquiry = Request["Inquiry"];
            string InquiryText = Request["InquiryText"];
            string DateFrom = Request["DateFrom"] + " 00:00:00";
            string DateTo = Request["DateTo"] + " 23:59:59";

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_CsChartMonitoringexport";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 3600;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Inquiry", Inquiry);
            cmd.Parameters.AddWithValue("@DateFrom", DateFrom);
            cmd.Parameters.AddWithValue("@DateTo", DateTo);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            dt1 = ds.Tables[0];
            dt2 = ds.Tables[1];

            if (dt2.Rows.Count <= 1)
            {
                return Json("Tidak ada data yang ditampilkan", "text/html", JsonRequestBehavior.AllowGet);
            }

            string inquiry = dt2.Rows[0][0].ToString();
            string periode = "";

            Inquiry = InquiryText;
            periode = (Convert.ToDateTime(DateFrom)).ToString("MMMM-yyyy");


            var header = new List<List<dynamic>>() { };
            header.Add(new List<dynamic> { "Inquiry ", ": " + Inquiry });
            header.Add(new List<dynamic> { "Periode ", ": " + periode });

            return GenerateReportChart(dt2, "CsChartMonitoring", "CsChartMonitoring", header);
            //return GenerateReportXlsxNonTotal(dt2, "CsChartMonitoring", "CsChartMonitoring", header);
        }

        public ActionResult csdatamonitoringexport()
        {
            DataSet ds = new DataSet();
            DataTable dt1 = new DataTable();
            DataTable dt2 = new DataTable();

            string DateInit = Request["DateInit"];
            string DateReff = Request["DateReff"];
            string Interval = Request["Interval"];

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_CsDataMonitoringexport";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 3600;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@DateInit", DateInit);
            cmd.Parameters.AddWithValue("@DateReff", DateReff);
            cmd.Parameters.AddWithValue("@Interval", Interval);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            dt1 = ds.Tables[0];
            //dt2 = ds.Tables[1];

            if (dt1.Rows.Count == 0)
            {
                return Json("Tidak ada data yang ditampilkan", "text/html", JsonRequestBehavior.AllowGet);
            }

            string branch = Convert.ToString(Interval) + " - Day(s)";
            string periode = (Convert.ToDateTime(DateInit)).ToString("dd-MMM-yyyy") + " s/d " + (Convert.ToDateTime(DateReff)).ToString("dd-MMM-yyyy");

            var header = new List<List<dynamic>>() { };
            header.Add(new List<dynamic> { "Interval Date ", ": " + branch });
            header.Add(new List<dynamic> { "Periode ", ": " + periode });

            return GenerateReportXlsxNonTotal(dt1, "csdatamonitoringexport", "csdatamonitoringexport", header);
        }

        public ActionResult CsDataTDaysCallDOexport()
        {
            DataSet ds = new DataSet();
            DataTable dt1 = new DataTable();
            DataTable dt2 = new DataTable();

            string BranchCode = Request["BranchCode"];
            string Year = Request["Year"];
            string Month = Request["Month"];
            string Periode = Request["Periode"];

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_CsDataTDaysCallDOexport";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 3600;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@Periode", Periode);
            cmd.Parameters.AddWithValue("@Year", Year);
            cmd.Parameters.AddWithValue("@Month", Month);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            dt1 = ds.Tables[0];
            dt2 = ds.Tables[1];

            if (dt1.Rows.Count == 0)
            {
                return Json("Tidak ada data yang ditampilkan", "text/html", JsonRequestBehavior.AllowGet);
            }

            string branch = dt1.Rows[0][0].ToString() + " - " + dt1.Rows[0][1].ToString();
            string periode = Periode;

            var header = new List<List<dynamic>>() { };
            header.Add(new List<dynamic> { "Do Monitoring ", ": " + branch });
            header.Add(new List<dynamic> { "Periode ", ": " + periode });

            return GenerateReportXlsx(dt2, "CsDataTDaysCallDO", "CsDataTDaysCallDO", header);

        }

        public ActionResult CsReportTDayCallExcell()
        {
            DataSet ds = new DataSet();
            DataTable dt1 = new DataTable();
            DataTable dt2 = new DataTable();

            string BranchCode = Request["BranchCode"];
            string DateFrom = Request["DateFrom"];
            string DateTo = Request["DateTo"];

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_CsChartTDayCallExport";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 3600;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@DateFrom", DateFrom);
            cmd.Parameters.AddWithValue("@DateTo", DateTo);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            dt1 = ds.Tables[0];
            dt2 = ds.Tables[1];

            if (dt1.Rows.Count == 0)
            {
                return Json("Tidak ada data yang ditampilkan", "text/html", JsonRequestBehavior.AllowGet);
            }

            string outletcode = "";
            string outletname = "";
            string periode = "";

            if (dt1.Rows.Count > 1)
            {
                 outletcode = "All";
                 outletname = "Branch";
                 periode = (Convert.ToDateTime(DateFrom)).ToString("dd-MMM-yyyy") + " s/d " + (Convert.ToDateTime(DateTo)).ToString("dd-MMM-yyyy");
            }
            else
            {
                outletcode = dt1.Rows[0][0].ToString();
                outletname = dt1.Rows[0][1].ToString();
                 periode = (Convert.ToDateTime(DateFrom)).ToString("dd-MMM-yyyy") + " s/d " + (Convert.ToDateTime(DateTo)).ToString("dd-MMM-yyyy");
            }

            var header = new List<List<dynamic>>() { };
            header.Add(new List<dynamic> { "Nama Branch ", ": " + outletcode + " - " + outletname });
            header.Add(new List<dynamic> { "Periode ", ": " + periode });

            return GenerateReportXlsx(dt2, "CsReportTDayCall", "CsReportTDayCall", header);

        }

        public ActionResult CsChartStnkExtforexport()
        {
            DataSet ds = new DataSet();
            DataTable dt1 = new DataTable();
            DataTable dt2 = new DataTable();

            string BranchCode = Request["BranchCode"];
            string DateFrom = Request["DateFrom"];
            string DateTo = Request["DateTo"];

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_CsChartStnkExtforexport";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 3600;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@DateFrom", DateFrom);
            cmd.Parameters.AddWithValue("@DateTo", DateTo);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            dt1 = ds.Tables[0];
            dt2 = ds.Tables[1];

            if (dt1.Rows.Count == 0)
            {
                return Json("Tidak ada data yang ditampilkan", "text/html", JsonRequestBehavior.AllowGet);
            }

            string outletcode = "";
            string outletname = "";
            string periode = "";

            if (dt1.Rows.Count > 1)
            {
                outletcode = "All";
                outletname = "Branch";
                periode = (Convert.ToDateTime(DateFrom)).ToString("dd-MMM-yyyy") + " s/d " + (Convert.ToDateTime(DateTo)).ToString("dd-MMM-yyyy");
            }
            else
            {
                outletcode = dt1.Rows[0][0].ToString();
                outletname = dt1.Rows[0][1].ToString();
                periode = (Convert.ToDateTime(DateFrom)).ToString("dd-MMM-yyyy") + " s/d " + (Convert.ToDateTime(DateTo)).ToString("dd-MMM-yyyy");
            }

            var header = new List<List<dynamic>>() { };
            header.Add(new List<dynamic> { "Nama Branch ", ": " + outletcode + " - " + outletname });
            header.Add(new List<dynamic> { "Periode ", ": " + periode });

            return GenerateReportXlsxNonTotal(dt2, "CsChartTDayCallExport", "CsChartTDayCallExport", header);
        }

        public ActionResult CsReportBPKBReminderexport()
        {
            DataSet ds = new DataSet();
            DataTable dt1 = new DataTable();
            DataTable dt2 = new DataTable();

            string BranchCode = Request["BranchCode"];
            string DateFrom = Request["DateFrom"];
            string DateTo = Request["DateTo"];

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_CsRptBPKBReminderexport";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 3600;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@DateFrom", DateFrom);
            cmd.Parameters.AddWithValue("@DateTo", DateTo);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            dt1 = ds.Tables[0];
            dt2 = ds.Tables[1];

            if (dt1.Rows.Count == 0)
            {
                return Json("Tidak ada data yang ditampilkan", "text/html", JsonRequestBehavior.AllowGet);
            }

            string outletcode = "";
            string outletname = "";
            string periode = "";

            if (dt1.Rows.Count > 1)
            {
                outletcode = "All";
                outletname = "Branch";
                periode = (Convert.ToDateTime(DateFrom)).ToString("dd-MMM-yyyy") + " s/d " + (Convert.ToDateTime(DateTo)).ToString("dd-MMM-yyyy");
            }
            else
            {
                outletcode = dt1.Rows[0][0].ToString();
                outletname = dt1.Rows[0][1].ToString();
                periode = (Convert.ToDateTime(DateFrom)).ToString("dd-MMM-yyyy") + " s/d " + (Convert.ToDateTime(DateTo)).ToString("dd-MMM-yyyy");
            }

            var header = new List<List<dynamic>>() { };
            header.Add(new List<dynamic> { "Nama Branch ", ": " + outletcode + " - " + outletname });
            header.Add(new List<dynamic> { "Periode ", ": " + periode });

            return GenerateReportXlsx(dt2, "CsRptBPKBReminder", "CsRptBPKBReminder", header);
        }

        public ActionResult CsReportCustBirthdayexport()
        {
            DataSet ds = new DataSet();
            DataTable dt1 = new DataTable();
            DataTable dt2 = new DataTable();

            string BranchCode = Request["BranchCode"];
            string PeriodYear = Request["PeriodYear"];
            string ParMonth1 = Request["ParMonth1"];
            string ParMonth2 = Request["ParMonth2"];
            string ParStatus = Request["ParStatus"];
            string ParMonth1Text = Request["ParMonth1Text"];
            string ParMonth2Text = Request["ParMonth2Text"];


            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_GetMonitoringCustBirthdayexport";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 3600;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@PeriodYear", PeriodYear);
            cmd.Parameters.AddWithValue("@ParMonth1", ParMonth1);
            cmd.Parameters.AddWithValue("@ParMonth2", ParMonth2);
            cmd.Parameters.AddWithValue("@ParStatus", ParStatus);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            dt1 = ds.Tables[0];
            dt2 = ds.Tables[1];

            if (dt1.Rows.Count == 0)
            {
                return Json("Tidak ada data yang ditampilkan", "text/html", JsonRequestBehavior.AllowGet);
            }

            string outletcode = "";
            string outletname = "";
            string periode = "";

            if (dt1.Rows.Count > 1)
            {
                outletcode = "All";
                outletname = "Branch";
                periode = ParMonth1Text + " s/d " + ParMonth2Text;
            }
            else
            {
                outletcode = dt1.Rows[0][0].ToString();
                outletname = dt1.Rows[0][2].ToString();
                periode = periode = ParMonth1Text + " s/d " + ParMonth2Text;
            }

            var header = new List<List<dynamic>>() { };
            header.Add(new List<dynamic> { "Nama ", ": " + outletcode + " - " + outletname });
            header.Add(new List<dynamic> { "Periode ", ": " + periode });

            return GenerateReportXlsxwithpercent(dt2, "MonitoringCustBirthday", "MonitoringCustBirthday", header);
        }

        #region Excel
        protected string GetExcelColumnNames(int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = String.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }

            return columnName;
        }

        protected JsonResult GenerateReportChart(DataTable dt, string sheetName, string fileName, List<List<dynamic>> header = null)
        {
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add(sheetName);
            var seqno = 1;
            fileName = fileName + "_" + DateTime.Now.ToString("yyyy_MMdd_HHmm");

            // add header information
            if (header != null)
            {
                seqno++;
                foreach (List<dynamic> row in header)
                {
                    //foreach (var col in row)
                    for (int i = 0; i < row.Count; i++)
                    {
                        var caption = row[i];
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnNames(i + 1), seqno)).Value = caption;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnNames(i + 1), seqno)).Style.Font.SetBold();
                    }
                    seqno++;
                }
                seqno++;
            }
            // set caption
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                ws.Cell(string.Format("{0}{1}", GetExcelColumnNames(i + 1), seqno)).Value = dt.Columns[i].Caption;
            }
            ws.Range(string.Format("A{0}:{1}{0}", seqno, GetExcelColumnNames(dt.Columns.Count)))
                .Style.Font.SetBold()
                .Fill.SetBackgroundColor(XLColor.CornflowerBlue)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            seqno++;
            int firtrow = seqno;
            string lastcolumn = "";

            // set cell value
            foreach (DataRow row in dt.Rows)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {//ws.Cell(lastRow + 1, iCol).Style.DateFormat.Format = "dd-MMM-yyyy";
                    var caption = dt.Columns[i].Caption;
                    if (row[caption].GetType().Name == "String")
                    {
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnNames(i + 1), seqno)).Value = "'" + row[caption];
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnNames(i + 1), seqno)).Style.Fill.SetBackgroundColor(XLColor.BeauBlue);
                    }
                    else if (row[caption].GetType().Name == "DateTime")
                    {
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnNames(i + 1), seqno)).Value = row[caption];
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnNames(i + 1), seqno)).Style.DateFormat.Format = "dd-MMM-yyyy";
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnNames(i + 1), seqno)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnNames(i + 1), seqno)).Style.Fill.SetBackgroundColor(XLColor.BeauBlue);
                    }
                    else if (row[caption].GetType().Name == "Decimal")
                    {
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnNames(i + 1), seqno)).Value = (Convert.ToDecimal(row[caption])) / 100;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnNames(i + 1), seqno)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnNames(i + 1), seqno)).Style.NumberFormat.Format = "0%";
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnNames(i + 1), seqno)).Style.Fill.SetBackgroundColor(XLColor.BeauBlue);
                    }
                    else
                    {
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnNames(i + 1), seqno)).Value = row[caption];
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnNames(i + 1), seqno)).Style.Fill.SetBackgroundColor(XLColor.BeauBlue);
                    }

                    lastcolumn = GetExcelColumnNames(i + 1);
                }

                seqno++;
            }

            // set width columns
            int lengths = 0;
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                if (dt.Rows[0][i].ToString().Length >= dt.Columns[i].Caption.Length)
                {
                    lengths = dt.Rows[0][i].ToString().Length;
                }
                else
                {
                    lengths = dt.Columns[i].Caption.Length;
                }
                ws.Columns((i + 1).ToString()).Width = lengths + 5;
            }

            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));

            int last = seqno - 1;
            Workbook book = new Workbook();
            book.LoadFromFile(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
            Worksheet sheet = book.Worksheets[sheetName];
            //Add chart and set chart data range
            Chart chart = sheet.Charts.Add(ExcelChartType.ColumnClustered);
            //chart.DataRange = sheet.Range["A5:AF21"];
            chart.DataRange = sheet.Range["A" + (firtrow-1) + ":" + lastcolumn + last];
            chart.SeriesDataFromRange = true;

            //Chart border  
            chart.ChartArea.Border.Weight = ChartLineWeightType.Medium;

            //Chart position  
            chart.LeftColumn = 1;
            chart.TopRow = seqno + 2;
            chart.RightColumn = seqno + 20;
            chart.BottomRow = seqno + 22;

            //Chart title  
            chart.ChartTitle = "";
            chart.ChartTitleArea.Font.FontName = "Calibri";
            chart.ChartTitleArea.Font.Size = 13;
            chart.ChartTitleArea.Font.IsBold = true;

            //Chart axis  
            chart.PrimaryCategoryAxis.Title = " ";
            chart.PrimaryValueAxis.Title = " ";
            chart.PrimaryValueAxis.TitleArea.TextRotationAngle = 90;

            //Chart legend  
            chart.Legend.Position = LegendPositionType.Top;

            book.SaveToFile(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));

            return Json(new
            {
                rows = dt.Rows.Count,
                cols = dt.Columns.Count,
                range = string.Format("A{0}:{1}{0}", seqno, GetExcelColumnName(dt.Columns.Count)),
                fileUrl = Url.Content("~/ReportTemp/" + fileName + ".xlsx"),
                header = header
            });
        }
        #endregion

        #region not used
        public JsonResult SvMonitoringByPeriode(string callback)
        {
            var json = Exec(new
            {
                query = "uspfn_SvChartMonitoringByPeriode",
                param = new List<dynamic>
                {
                    new { key = "Area", value = Request.Params["Area"] },
                    new { key = "Dealer", value = Request.Params["Dealer"] },
                    new { key = "Outlet", value = Request.Params["Outlet"] },
                    new { key = "Year", value = Request.Params["Year"] },
                    new { key = "Month", value = Request.Params["Month"] },
                },
                result = "dataset"
            });
            return Json(json.Data);
        }

        public JsonResult SvUnitIntakeSummary(string callback)
        {
            var json = Exec(new
            {
                query = "uspfn_SvChartUnitIntakeSummary",
                param = new List<dynamic>
                {
                    new { key = "Year", value = Request.Params["Year"] },
                    new { key = "Month", value = Request.Params["Month"] },
                    new { key = "Area", value = Request.Params["Area"] },
                    new { key = "Dealer", value = Request.Params["Dealer"] },
                },
                result = "dataset"
            });
            return Json(json.Data);
        }

        public JsonResult SvRegisterSpk1(string callback)
        {
            var json = Exec(new
            {
                query = "uspfn_SvChartRegisterSpk1",
                param = new List<dynamic>
                {
                    new { key = "DateFrom", value = Request.Params["DateFrom"] },
                    new { key = "DateTo", value = Request.Params["DateTo"] },
                    new { key = "Area", value = Request.Params["Area"] },
                    new { key = "Dealer", value = Request.Params["Dealer"] },
                },
                //result = "dataset"
            });
            return Json(json.Data);
        }
#endregion
    }
    
    public class seriesModel
    {
        public string key { get; set; }
        public string value { get; set; }
        public List<int> data { get; set; }
    }

    public class seriesModel2
    {
        public string key { get; set; }
        public string value { get; set; }
        public List<DataModel> data { get; set; }
    }

    public class DataModel
    {
        public string Date { get; set; }
        public int DataCount { get; set; }
    }

    public class ColsModel
    {
        public string DealerCode { get; set; }
        public string DealerName { get; set; }
    }
}