using eXpressReport.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace eXpressReport.Controllers
{
    public class HomeController : Controller
    {
        protected ReportDB db = new ReportDB();       

        public ActionResult Index(string id)
        {
            var report = db.ReportSessions.Find(id);
            if (report != null)
            {
                ViewBag.reportid = report.ReportId;
                ViewBag.Parameters = report.Parameters;
            }
            return View();
        }


        public static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            // Convert the byte array to hexadecimal string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            return sb.ToString();
        }

        public JsonResult Session(ReportSession data)
        {
            data.SessionId = CreateMD5(data.CreatedBy + DateTime.Now.ToString("G"));
            data.CreatedDate = DateTime.Now;

            db.ReportSessions.Add(data);
            db.SaveChanges();

            var sql = "delete from reportsession where createdby='" + data.CreatedBy + "' and sessionid !='" + data.SessionId + "'";
            db.Database.SqlQuery<object>(sql).FirstOrDefault();

            return Json(new { success = true, SessionId = data.SessionId }, JsonRequestBehavior.AllowGet);
        }

    }
}