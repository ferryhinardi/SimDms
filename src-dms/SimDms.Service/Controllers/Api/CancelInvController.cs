using SimDms.Service.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Service.Controllers.Api
{
    public class CancelInvController : BaseController
    {
        public JsonResult Default()
        {
            var user = ctx.SysUsers.FirstOrDefault();

            var data = (from p in ctx.Employees
                       where p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && (new string[] { "3", "7" }).Contains(p.TitleCode) && p.PersonnelStatus == "1" &&
                            p.EmployeeID == user.UserId
                       select new
                       {
                           ForemanID = p.EmployeeID,
                           ForemanName = p.EmployeeName
                       }).FirstOrDefault();

            var admin = ctx.SysRoleUsers.Where(p => p.UserId == user.UserId && p.RoleId.ToString().ToUpper().Contains("ADMIN"));

            return Json(new
            {
                BranchCode = BranchCode,
                BranchName = BranchName              
            });
        }

        public JsonResult SelectInqInvCancel(string inv1, string inv2)
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;

            cmd.CommandText = "uspfn_SvInqInvCancel";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@Invoice1", inv1);
            cmd.Parameters.AddWithValue("@Invoice2", inv2);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            var list = GetJson(dt);

            return Json(new { data = list });
        }

        public JsonResult GetDescInvoice()
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;

            cmd.CommandText = "uspfn_SvTrnInvoiceJournal";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@ProductType", ProductType);
            cmd.Parameters.AddWithValue("@InvoiceNo", Request["InvoiceNo"]);
            cmd.Parameters.AddWithValue("@UserID", CurrentUser.UserId);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            var list = GetJson(dt);

            return Json(new { data = list });
        }

        public JsonResult RePosting(string invoiceNo)
        {
            using (var tran = ctx.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    string[] listInvoice = new string[] { };
                    listInvoice = invoiceNo.ToString().Split(',');

                    for (int i = 0; i < listInvoice.Count(); i++)
                    {
                        ctx.Database.ExecuteSqlCommand(
                            "exec uspfn_RepostingCancelInvoice @p0, @p1, @p2",
                            CompanyCode, BranchCode, listInvoice[i].ToString());
                    }

                    tran.Commit();
                    
                    return Json(new { Message = "" });
                }
                catch (Exception ex)
                {
                    tran.Rollback();

                    return Json(new { Message = ex.Message });
                }
            }
        }

        public JsonResult CancelInvoice(string invoiceNo)
        {
            using (var tran = ctx.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    string[] listInvoice = new string[] { };
                    listInvoice = invoiceNo.ToString().Split(',');

                    for (int i = 0; i < listInvoice.Count(); i++)
                    {
                        ctx.Database.ExecuteSqlCommand(
                            "exec uspfn_SvTrnInvoiceCancel @p0, @p1, @p2, @p3",
                            CompanyCode, BranchCode, listInvoice[i].ToString(), CurrentUser.UserId);

                        ctx.Database.ExecuteSqlCommand(
                            "exec uspfn_SvTrnSdMovementDelDocNo @p0, @p1, @p2",
                            CompanyCode, BranchCode, listInvoice[i].ToString());
                    }

                    tran.Commit();

                    return Json(new { Message = "" });
                }
                catch (Exception ex)
                {
                    tran.Rollback();

                    return Json(new { Message = ex.Message });
                }
            }
        }
    }
}
