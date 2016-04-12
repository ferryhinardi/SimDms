using SimDms.Common;
using SimDms.Common.DcsWs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Sparepart.Controllers.Api
{
    public class InquiryDCSDataController : BaseController
    {
        private DcsWsSoapClient ws = new DcsWsSoapClient();

        public JsonResult Default()
        {
            int year = DateTime.Today.Year;
            int month = DateTime.Today.Month;

            var status = false;

            try
            {
                status = ws.IsValid();
            }
            catch
            {
                status = false;
            }

            return Json(new
            {
                FirstPeriod = new DateTime(year, month, 1),
                EndPeriod = new DateTime(year, month, 1).AddMonths(1).AddDays(-1),
                stat = status,
                Status = status ? "Online" : "Offline"
            });
        }

        public JsonResult Retrieve(string DataID, DateTime FirstPeriod, DateTime EndPeriod)
        {
            try
            {
                var data = ws.RetrieveData(CompanyCode, DataID, FirstPeriod, EndPeriod);
                DataTable dt = new DataTable();

                if (data.Count > 0)
                {
                    if (data[0].StartsWith("FAIL"))
                    {
                        return Json(new { success = false, message = data[0].Substring(5) });
                    }
                    else
                    {

                        dt.Columns.Add("ID", typeof(int));
                        dt.Columns.Add("DataID", typeof(string));
                        dt.Columns.Add("DealerCode", typeof(string));
                        dt.Columns.Add("CreatedDate", typeof(DateTime));
                        dt.Columns.Add("Contents", typeof(string));
                        dt.Columns.Add("Status", typeof(string));
                        dt.Columns.Add("ProductType", typeof(string));
                        dt.Columns.Add("Info", typeof(string));
                        foreach (string var in data)
                        {
                            string s = var + ",";
                            string[] items = s.Split(',');
                            items[7] = items[4].Split('\n')[0];
                            try
                            {
                                dt.Rows.Add(items);
                            }
                            catch
                            {

                            }
                        }

                    }
                }

                var retrieveData = dt.AsEnumerable().Select(a => new DCSData
                {
                    ID = a.Field<int>("ID"),
                    DataID = a.Field<string>("DataId"),
                    DealerCode = a.Field<string>("DealerCode"),
                    CreatedDate = a.Field<DateTime>("CreatedDate"),
                    Contents = a.Field<string>("Contents"),
                    Status = a.Field<string>("Status"),
                    ProductType = a.Field<string>("ProductType"),
                    Info = a.Field<string>("Info")
                }).OrderByDescending(a => a.ID);

                return Json(new { success = true, data = retrieveData });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
        }

        public class DCSData
        {
            public int ID { get; set; }
            public string DataID { get; set; }
            public string DealerCode { get; set; }
            public DateTime? CreatedDate { get; set; }
            public string Contents { get; set; }
            public string Status { get; set; }
            public string ProductType { get; set; }
            public string Info { get; set; }
        }
    }
}
