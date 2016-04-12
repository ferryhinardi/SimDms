using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GeLang;
using SimDms.CStatisfication.Models;
using System.Data.SqlClient;
using System.Data;
using SimDms.Common.Models;

namespace SimDms.CStatisfication.Controllers.Api
{
    public class ReviewController : BaseController
    {
        public JsonResult Default()
        {
            var sign = 0;
            var Employee = ctx.HrEmployees.Where(a => a.CompanyCode == CompanyCode && a.RelatedUser == CurrentUser.UserId).FirstOrDefault();

            if (Employee != null)
            {
                if (Employee.Position == "GM")
                {
                    sign = 1;
                }
            }

            if (CurrentUser.UserId == "ga")
            {
                sign = 1;
            }

            var data = new
            {
                CompanyCode = CompanyCode,
                BranchCode = BranchCode,
                CompanyName = CompanyName,
                BranchName = BranchName,
                sign = sign
            };
            return Json(data);
        }

        public JsonResult save(DateTime? InputFrom, DateTime? InputTo)
        {
            string acts = Request["Plan"] ?? "";
            string EmployeeID = Request["EmployeeID"] ?? "";
            string _BranchCode = Request["BranchCode"] ?? BranchCode;

            if (EmployeeID == "")
            {
                var Employee = ctx.HrEmployees.Where(a => a.CompanyCode == CompanyCode && a.RelatedUser == CurrentUser.UserId).FirstOrDefault();

                if (Employee != null)
                {
                    if (Employee.Position != "BM" && CurrentUser.UserId != "ga")
                    {
                        return Json(new { success = false, message = "User yang digunakan untuk input PDCA bukan BM" });
                    }
                    EmployeeID = Employee.Position == "BM" ? Employee.EmployeeID : CurrentUser.UserId;
                }
                else if (CurrentUser.UserId == "ga")
                {
                    EmployeeID = CurrentUser.UserId;
                }
                else
                {
                    return Json(new { success = false, message = "User yang digunakan untuk input PDCA bukan BM" });
                }
            }

            CsSetting act = ctx.CsSettings.FirstOrDefault(x => x.SettingLink2 == acts);

            var review = ctx.CsReviews.Find(CompanyCode, _BranchCode, EmployeeID, InputFrom, InputTo, act.SettingLink2);
            if (review == null)
            {
                review = new CsReview();
                review.CompanyCode = CompanyCode;
                review.BranchCode = _BranchCode;
                //if (Employee.Position == "BM" || CurrentUser.UserId == "ga")
                //{
                    review.EmployeeID = EmployeeID;
                //}
                review.DateFrom = InputFrom.Value;
                review.DateTo = InputTo.Value;
                review.Plan = act.SettingLink2;
                review.CreatedBy = CurrentUser.UserId;
                review.CreatedDate = DateTime.Now;
                ctx.CsReviews.Add(review);
            }
            review.Do = act.SettingLink3;
            review.Check = Request["Check"] ?? "";
            review.Action = Request["Action"] ?? "";
            review.CommentbyGM = Request["CommentbyGM"] ?? "";
            //review.CommentbySIS = Request["CommentbySIS"] ?? "";
            review.PIC = Request["PIC"] ?? "";
            review.LastupdateBy = CurrentUser.UserId;
            review.LastupdateDate = DateTime.Now;

            try
            {
                ctx.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message + " - " + ex.InnerException });
            }

        }

        public JsonResult delete(DateTime? InputFrom, DateTime? InputTo)
        {
            string acts = Request["Plan"] ?? "";
            string EmployeeID = Request["EmployeeID"] ?? "";
            string _BranchCode = Request["BranchCode"] ?? BranchCode;
            
            CsSetting act = ctx.CsSettings.FirstOrDefault(x => x.SettingLink2 == acts);
            CsReview review = new CsReview();
            review = ctx.CsReviews.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.BranchCode == _BranchCode && x.EmployeeID == EmployeeID && x.DateFrom == InputFrom && x.DateTo == InputTo && x.Plan == act.SettingLink2);
            try
            {
                ctx.CsReviews.Remove(review);
                ctx.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception)
            {
                return Json(new { success = false });
            }

        }

        public JsonResult Reviews(string BranchCode)
        {
            string pDateFrom = Request["DateFrom"];
            string pDateTo = Request["DateTo"];
            //string branchCode = Request["BranchCode"] ?? "";

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = @"select a.*, dom.OutletCode, dom.OutletAbbreviation 
                                  from CsReviews a
                            inner join gnMstDealerOutletMapping dom 
                                    on dom.DealerCode = a.CompanyCode and dom.OutletCode = a.BranchCode
                                 where a.companycode = @CompanyCode 
                                   and a.BranchCode = CASE @BranchCode WHEN '' THEN a.BranchCode ELSE @BranchCode END
                                   and convert(varchar, datefrom, 112) >= @DateFrom and convert(varchar, dateto, 112) <= @DateTo";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@DateFrom", pDateFrom);
            cmd.Parameters.AddWithValue("@DateTo", pDateTo);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult Branchs()
        {
            bool result = false;
            var Branch = "";
            var Employee = ctx.HrEmployees.Where(a => a.CompanyCode == CompanyCode && a.RelatedUser == CurrentUser.UserId).FirstOrDefault();

            if (Employee != null)
            {
                if (Employee.Position != "GM")
                {
                    result = true;
                    Branch = BranchCode;
                }
            }

            return Json(new { success = result, Branch = Branch });
        }
    }
}