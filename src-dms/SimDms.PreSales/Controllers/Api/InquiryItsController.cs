using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.IO;
using System.Transactions;
using SimDms.Common.Models;
using SimDms.Common.DcsWs;
using System.Diagnostics;
using ClosedXML.Excel;

namespace SimDms.PreSales.Controllers.Api
{
    public class InquiryItsController : BaseController
    {
        public JsonResult Default()
        {
            var Position = "";
            var list = ctx.HrEmployees.Where(p => p.Department == "SALES" && p.PersonnelStatus == "1").Select(p => new { p.CompanyCode, p.EmployeeID, p.EmployeeName, p.Grade, p.TeamLeader, p.RelatedUser, p.Position });
            var empl = list.Where(p => p.CompanyCode == CompanyCode && p.RelatedUser == CurrentUser.UserId).FirstOrDefault();
            var itsg = ctx.LookUpDtls.Where(p => p.CompanyCode == CompanyCode && p.CodeID == "ITSG").OrderBy(p => p.SeqNo).Select(p => new { value = p.LookUpValue, text = p.LookUpValueName.ToUpper() }).ToList();
            var outlet = ctx.PmBranchOutlets.Where(e => e.CompanyCode == CompanyCode && e.BranchCode == BranchCode).FirstOrDefault();

            if (CurrentUser.UserId == "ga")
            {
                empl = list.Where(p => p.CompanyCode == CompanyCode && (p.Position == "GM" || p.Position == "COO" || p.Position == "COE")).FirstOrDefault();

                if (empl == null) {
                    // Kondisi Khusus Dealer SBT untuk login ga
                    return Json(new
                    {
                        success = true,
                        data = new
                        {
                            Position = "GM",
                            CompanyCode = CompanyCode,
                            BranchCode = BranchCode,
                        }
                    }); 
                }
            }

            var AdditionalJob = ctx.HrEmployeeAdditionalJobs.Where(a => a.CompanyCode == CompanyCode && a.EmployeeID == empl.EmployeeID).FirstOrDefault();
            if (AdditionalJob != null)
            {
                if (empl.Position != AdditionalJob.Position)
                {
                    Position = empl.Position + AdditionalJob.Position;
                }
                else
                {
                    Position = empl.Position;
                }
            }
            else
            {
                Position = empl.Position;
            }

            if (empl != null)
            {
                var curdate = DateTime.Now;
                string NikSL = "", NikSC = "", NikSH = "", NikBM="" ;
                string NikSLName = "", NikSCName = "", NikSHName = "", NikBMName = ""; 

                dynamic EmpSLList = new {};
                dynamic EmpSCList = new {};
                dynamic EmpSHList = new {};
                dynamic EmpBMList = new {};

                if (Position == "S")
                {
                    NikSL = empl.EmployeeID;
                    NikSLName = empl.EmployeeName;
                    EmpSLList = new List<dynamic>() { new { value = empl.EmployeeID, text = empl.EmployeeName } };

                    var oSC = list.Where(p => p.EmployeeID == empl.TeamLeader).FirstOrDefault();
                    if (oSC != null)
                    {
                        //NikSC = oSC.EmployeeID;
                        //NikSCName = oSC.EmployeeName;
                        //EmpSCList = list.Where(p => p.EmployeeID == oSC.EmployeeID).Select(p => new { value = p.EmployeeID, text = p.EmployeeName });

                        //var oSH = list.Where(p => p.EmployeeID == oSC.TeamLeader).FirstOrDefault();
                        //if (oSH != null)
                        //{
                            NikSH = oSC.EmployeeID;
                            NikSHName = oSC.EmployeeName;
                            EmpSHList = list.Where(p => p.EmployeeID == oSC.EmployeeID).Select(p => new { value = p.EmployeeID, text = p.EmployeeName });

                            var oBM = list.Where(p => p.EmployeeID == oSC.TeamLeader).FirstOrDefault();
                            if (oBM != null)
                            {
                                NikBM = oBM.EmployeeID;
                                NikBMName = oBM.EmployeeName;
                                EmpBMList = list.Where(p => p.EmployeeID == oBM.EmployeeID).Select(p => new { value = p.EmployeeID, text = p.EmployeeName });
                            }
                        //}
                    }
                }
                else if (Position == "SC")
                {
                    NikSC = empl.EmployeeID;
                    NikSCName = empl.EmployeeName;
                    EmpSLList = list.Where(p => p.TeamLeader == empl.EmployeeID).Select(p => new { value = p.EmployeeID, text = p.EmployeeName });
                    EmpSCList = new List<dynamic>() { new { value = empl.EmployeeID, text = empl.EmployeeName } };

                    var oSH = list.Where(p => p.EmployeeID == empl.TeamLeader).FirstOrDefault();
                    if (oSH != null)
                    {
                        NikSH = oSH.EmployeeID;
                        NikSHName = oSH.EmployeeName;
                        EmpSHList = list.Where(p => p.EmployeeID == oSH.EmployeeID).Select(p => new { value = p.EmployeeID, text = p.EmployeeName });
                        
                        var oBM = list.Where(p => p.EmployeeID == oSH.TeamLeader).FirstOrDefault();
                        if (oBM != null)
                        {
                            NikBM = oBM.EmployeeID;
                            NikBMName = oBM.EmployeeName;
                            EmpBMList = list.Where(p => p.EmployeeID == oBM.EmployeeID).Select(p => new { value = p.EmployeeID, text = p.EmployeeName });
                        }
                    }
                }
                else if (Position == "SH" || Position == "SHSTD")
                {
                    NikSH = empl.EmployeeID;
                    NikSHName = empl.EmployeeName;
                    EmpSCList = list.Where(p => p.TeamLeader == empl.EmployeeID).Select(p => new { value = p.EmployeeID, text = p.EmployeeName });
                    EmpSHList = new List<dynamic>() { new { value = empl.EmployeeID, text = empl.EmployeeName } };

                    var oBM = list.Where(p => p.EmployeeID == empl.TeamLeader).FirstOrDefault();
                    if (oBM != null)
                    {
                        NikBM = oBM.EmployeeID;
                        NikBMName = oBM.EmployeeName;
                        EmpBMList = list.Where(p => p.EmployeeID == oBM.EmployeeID).Select(p => new { value = p.EmployeeID, text = p.EmployeeName });
                    }
                }
                else if (Position == "BM")
                {
                    NikBM = empl.EmployeeID;
                    NikBMName = empl.EmployeeName;
                    EmpSHList = list.Where(p => p.TeamLeader == empl.EmployeeID).Select(p => new { value = p.EmployeeID, text = p.EmployeeName });
                    EmpBMList = new List<dynamic>() { new { value = empl.EmployeeID, text = empl.EmployeeName } };

                    var TeamLeader = ctx.HrEmployees
                    .Where(x => x.CompanyCode == CompanyCode && x.Department == "SALES" && x.Position == "SH" && x.TeamLeader == empl.EmployeeID && x.PersonnelStatus == "1")
                    .Select(x => x.EmployeeID).ToList();

                    EmpSLList = ctx.HrEmployees
                        .Where(x => (x.TeamLeader.Contains(x.TeamLeader) == true))
                       .Select(x => new { value = x.EmployeeID, text = x.EmployeeName }).ToList();
                }
                else if (Position == "BMSH")
                {
                    NikBM = empl.EmployeeID;
                    NikBMName = empl.EmployeeName;
                    EmpSHList = (from x in ctx.HrEmployees
                                 where x.CompanyCode == CompanyCode && x.Department == "SALES" && x.Position == "SH" && x.TeamLeader == empl.EmployeeID && x.PersonnelStatus == "1"
                                 select new { value = x.EmployeeID, text = x.EmployeeName })
                            .Union(from y in ctx.HrEmployeeAdditionalJobs
                                   join z in ctx.HrEmployees
                                   on new { y.CompanyCode, y.EmployeeID } equals new { z.CompanyCode, z.EmployeeID }
                                   where y.CompanyCode == CompanyCode && y.Department == "SALES" && y.Position == "SH" && y.EmployeeID == empl.EmployeeID
                                   select new { value = y.EmployeeID, text = z.EmployeeName });
                    EmpBMList = new List<dynamic>() { new { value = empl.EmployeeID, text = empl.EmployeeName } };

                    var TeamLeader = ctx.HrEmployees
                    .Where(x => x.CompanyCode == CompanyCode && x.Department == "SALES" && x.Position == "SH" && x.TeamLeader == empl.EmployeeID && x.PersonnelStatus == "1")
                    .Select(x => x.EmployeeID).ToList();

                    EmpSLList = ctx.HrEmployees
                        .Where(x => (x.TeamLeader.Contains(x.TeamLeader) == true))
                       .Select(x => new { value = x.EmployeeID, text = x.EmployeeName }).ToList();
                }

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        //DateFrom = new DateTime(curdate.Year, curdate.Month, 1),
                        //DateTo = curdate,
                        Position = AdditionalJob != null ? empl.Position != AdditionalJob.Position ? empl.Position+AdditionalJob.Position : empl.Position : empl.Position,
                        NikSL = NikSL,
                        NikSLName = NikSLName,
                        //NikSLstext = "-- ALL SALESMAN --",
                        EmpSLList = EmpSLList,
                        NikSC = NikSC,
                        NikSCName = NikSCName,
                        EmpSCList = EmpSCList,
                        NikSH = NikSH,
                        NikSHName = NikSHName,
                        EmpSHList = EmpSHList,
                        NikBM = NikBM,
                        NikBMName = NikBMName,
                        EmpBMList = EmpBMList,
                        //ItsgList = itsg,
                        //OutletCode = (outlet == null) ? "" : outlet.OutletID,
                        //OutletName = (outlet == null) ? "" : outlet.OutletName,
                        CompanyCode = CompanyCode,
                        BranchCode = BranchCode,
                    }
                });
            }
            else
            {
                return Json(new { success = false, message = "User belum link dengan salesman" });
            }
        }

        public JsonResult LostCase()
        {
            string pEmployeeID = Request["Nik"];
            string pDateFrom = Request["DateFrom"];
            string pDateTo = Request["DateTo"];

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "usp_itsinqlostcase";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@EmployeeID", pEmployeeID);
            cmd.Parameters.AddWithValue("@DateFrom", pDateFrom);
            cmd.Parameters.AddWithValue("@DateTo", pDateTo);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult LostByType()
        {
            string pEmployeeID = Request["Nik"];
            string pDateFrom = Request["DateFrom"];
            string pDateTo = Request["DateTo"];

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "usp_itsinqlostbytype";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@EmployeeID", pEmployeeID);
            cmd.Parameters.AddWithValue("@DateFrom", pDateFrom);
            cmd.Parameters.AddWithValue("@DateTo", pDateTo);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult LostBySource()
        {
            string pEmployeeID = Request["Nik"];
            string pDateFrom = Request["DateFrom"];
            string pDateTo = Request["DateTo"];

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "usp_itsinqlostbysource";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@EmployeeID", pEmployeeID);
            cmd.Parameters.AddWithValue("@DateFrom", pDateFrom);
            cmd.Parameters.AddWithValue("@DateTo", pDateTo);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult FollowUpInqury()
        {
            string companyCode = CompanyCode;
            string branchCode = BranchCode;
            string dateAwal = Request["DateFrom"];
            string dateAkhir = Request["DateTo"];
            string outlet = Request["Outlet"];
            string head = Request["SH"];
            string spv = Request["SC"];
            string emp = Request["SALES"];
            string detailData = Request["DetData"];
            string ItsSis = Request["ItsSis"];
            string param = "0";
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;

            if (ItsSis == "true")
            {
                cmd.CommandText = "uspfn_itsinqfollowup2";
            }else
            {
                cmd.CommandText = "uspfn_itsinqfollowup";
            }
            
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", companyCode);
            cmd.Parameters.AddWithValue("@BranchCode", branchCode);
            cmd.Parameters.AddWithValue("@DateAwal", dateAwal);
            cmd.Parameters.AddWithValue("@DateAkhir", dateAkhir);
            cmd.Parameters.AddWithValue("@Outlet", outlet);
            cmd.Parameters.AddWithValue("@SPV", spv);
            cmd.Parameters.AddWithValue("@EMP", emp);
            cmd.Parameters.AddWithValue("@Head", head);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult OutstandingProspek()
        {
            string companyCode = (string.IsNullOrWhiteSpace(Request["companyCode"])?"": Request["companyCode"].ToString());
            string branchCode = (string.IsNullOrWhiteSpace(Request["branchCode"])?"":Request["branchCode"].ToString());
            string periode = (string.IsNullOrWhiteSpace(Request["periode"])?"":Request["periode"].ToString());
            string loginAs = (string.IsNullOrWhiteSpace(Request["loginAs"])?"":Request["loginAs"].ToString());
            string coo = (string.IsNullOrWhiteSpace(Request["coo"])?"":Request["coo"].ToString());
            string bm = (string.IsNullOrWhiteSpace(Request["bm"]) ? "" : Request["bm"].ToString());
            string sh = (string.IsNullOrWhiteSpace(Request["sh"].ToString()) ? "":Request["sh"].ToString());
            string sc = (string.IsNullOrWhiteSpace(Request["sc"])?"":Request["sc"].ToString());
            string sales = (string.IsNullOrWhiteSpace(Request["sales"]) ? "" : Request["sales"].ToString());
            string tableType = (string.IsNullOrWhiteSpace(Request["sales"]) ? "" : Request["tableType"].ToString());

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "usprpt_PmRpInqOutStandingSimDms";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", companyCode);
            cmd.Parameters.AddWithValue("@BranchCode", branchCode);
            cmd.Parameters.AddWithValue("@Period", periode);
            cmd.Parameters.AddWithValue("@LoginAS", loginAs);
            cmd.Parameters.AddWithValue("@COO", coo);
            cmd.Parameters.AddWithValue("@BranchManager", bm);
            cmd.Parameters.AddWithValue("@SalesHead", sh);
            cmd.Parameters.AddWithValue("@SalesCoordinator", sc);
            cmd.Parameters.AddWithValue("@Salesman", sales);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            DataTable dt = null;
            da.Fill(ds);
            switch (tableType.ToLower())
            {
                case "bysales":
                    dt = ds.Tables[0];
                    break;
                case "bytype":
                    dt = ds.Tables[1];
                    break;
                case "bysource":
                    dt = ds.Tables[2];
                    break;
                default:
                    dt = new DataTable();
                    break;
            }
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult Summary()
        {
            try
            {
                string companyCode = (string.IsNullOrWhiteSpace(Request["companyCode"])?"":Request["companyCode"].ToString());
                string branchCode = (string.IsNullOrWhiteSpace(Request["branchCode"])?"":Request["branchCode"].ToString());
                string periodeBegin = (string.IsNullOrWhiteSpace(Request["periodeBegin"])?"":Request["periodeBegin"].ToString());
                string periodeEnd = (string.IsNullOrWhiteSpace(Request["periodeEnd"]) ? "" : Request["periodeEnd"].ToString());
                string loginAs = (string.IsNullOrWhiteSpace(Request["loginAs"])?"":Request["loginAs"].ToString());
                string coo = (string.IsNullOrWhiteSpace(Request["coo"])?"":Request["coo"].ToString());
                string bm = (string.IsNullOrWhiteSpace(Request["bm"])?"":Request["bm"].ToString());
                string sh = (string.IsNullOrWhiteSpace(Request["sh"])?"":Request["sh"].ToString());
                string sc = (string.IsNullOrWhiteSpace(Request["sc"])?"":Request["sc"].ToString());
                string sales =  (string.IsNullOrWhiteSpace(Request["sales"])?"":Request["sales"].ToString());
                string tableType = (string.IsNullOrWhiteSpace(Request["tableType"])?"":Request["tableType"].ToString());

                SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
                cmd.CommandText = "usprpt_PmRpInqSummary";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 100;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@CompanyCode", companyCode);
                cmd.Parameters.AddWithValue("@BranchCode", branchCode);
                cmd.Parameters.AddWithValue("@PeriodBegin", periodeBegin);
                cmd.Parameters.AddWithValue("@PeriodEnd", periodeEnd);
                cmd.Parameters.AddWithValue("@LoginAS", loginAs);
                cmd.Parameters.AddWithValue("@COO", coo);
                cmd.Parameters.AddWithValue("@BranchManager", bm);
                cmd.Parameters.AddWithValue("@SalesHead", sh);
                cmd.Parameters.AddWithValue("@SalesCoordinator", sc);
                cmd.Parameters.AddWithValue("@Salesman", sales);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                DataTable dt = null;
                da.Fill(ds);
                switch (tableType.ToLower())
                {
                    case "bysales":
                        dt = ds.Tables[0];
                        break;
                    case "bytype":
                        dt = ds.Tables[1];
                        break;
                    case "bysource":
                        dt = ds.Tables[2];
                        break;
                    default:
                        dt = new DataTable();
                        break;
                }
                da.Fill(dt);
                return Json(GetJson(dt));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public JsonResult InqPeriode()
        {
            string companyCode = (string.IsNullOrWhiteSpace(Request["companyCode"]) ? "" : Request["companyCode"].ToString());
            string branchCode = (string.IsNullOrWhiteSpace(Request["branchCode"]) ? "" : Request["branchCode"].ToString());
            string periodeBegin = (string.IsNullOrWhiteSpace(Request["periodeBegin"]) ? "" : Request["periodeBegin"].ToString());
            string periodeEnd = (string.IsNullOrWhiteSpace(Request["periodeEnd"]) ? "" : Request["periodeEnd"].ToString());
            string outlet = (string.IsNullOrWhiteSpace(Request["outlet"]) ? "" : Request["outlet"].ToString());
            string spv = (string.IsNullOrWhiteSpace(Request["sc"]) ? "" : Request["sc"].ToString());
            string emp = (string.IsNullOrWhiteSpace(Request["sales"]) ? "" : Request["sales"].ToString());
            string head = (string.IsNullOrWhiteSpace(Request["sh"]) ? "" : Request["sh"].ToString());
            string tableType = (string.IsNullOrWhiteSpace(Request["tableType"]) ? "" : Request["tableType"].ToString());


            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_itsinqbyperiode";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 100;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", companyCode);
            cmd.Parameters.AddWithValue("@BranchCode", branchCode);
            cmd.Parameters.AddWithValue("@DateAwal", periodeBegin);
            cmd.Parameters.AddWithValue("@DateAkhir", periodeEnd);
            cmd.Parameters.AddWithValue("@Outlet", outlet);
            cmd.Parameters.AddWithValue("@SPV", spv);
            cmd.Parameters.AddWithValue("@EMP", emp);
            cmd.Parameters.AddWithValue("@Head", head);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        #region "SalesAchievement"
        public JsonResult AchievementSalesMan()
        {
            string companyCode = (string.IsNullOrWhiteSpace(Request["CompanyCode"]) ? "" : Request["CompanyCode"].ToString());
            string bmEmployeeID = (string.IsNullOrWhiteSpace(Request["NikBM"]) ? "" : Request["NikBM"].ToString());
            string shEmployeeID = (string.IsNullOrWhiteSpace(Request["NikSH"]) ? "" : Request["NikSH"].ToString());
            string scEmployeeID = (string.IsNullOrWhiteSpace(Request["NikSC"]) ? "" : Request["NikSC"].ToString());
            string smEmployeeID = (string.IsNullOrWhiteSpace(Request["NikSL"]) ? "" : Request["NikSL"].ToString());
            string year = (string.IsNullOrWhiteSpace(Request["Year"]) ? "1990" : Request["Year"].ToString());
            
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_itsinqAchievementSalesman";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", companyCode);
            cmd.Parameters.AddWithValue("@BMEmployeeID", bmEmployeeID);
            cmd.Parameters.AddWithValue("@SHEmployeeID", shEmployeeID);
            cmd.Parameters.AddWithValue("@SCEmployeeID", scEmployeeID);
            cmd.Parameters.AddWithValue("@SMEmployeeID", smEmployeeID);
            cmd.Parameters.AddWithValue("@Year", int.Parse(year));

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult AchievementSourceData()
        {
            string companyCode = (string.IsNullOrWhiteSpace(Request["CompanyCode"]) ? "" : Request["CompanyCode"].ToString());
            string bmEmployeeID = (string.IsNullOrWhiteSpace(Request["NikBM"]) ? "" : Request["NikBM"].ToString());
            string shEmployeeID = (string.IsNullOrWhiteSpace(Request["NikSH"]) ? "" : Request["NikSH"].ToString());
            string scEmployeeID = (string.IsNullOrWhiteSpace(Request["NikSC"]) ? "" : Request["NikSC"].ToString());
            string smEmployeeID = (string.IsNullOrWhiteSpace(Request["NikSL"]) ? "" : Request["NikSL"].ToString());
            string year = (string.IsNullOrWhiteSpace(Request["Year"]) ? "1990" : Request["Year"].ToString());

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_itsinqAchievementSourceData";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", companyCode);
            cmd.Parameters.AddWithValue("@BMEmployeeID", bmEmployeeID);
            cmd.Parameters.AddWithValue("@SHEmployeeID", shEmployeeID);
            cmd.Parameters.AddWithValue("@SCEmployeeID", scEmployeeID);
            cmd.Parameters.AddWithValue("@SMEmployeeID", smEmployeeID);
            cmd.Parameters.AddWithValue("@Year", int.Parse(year));

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult AchievementSalesType()
        {
            string companyCode = (string.IsNullOrWhiteSpace(Request["CompanyCode"]) ? "" : Request["CompanyCode"].ToString());
            string bmEmployeeID = (string.IsNullOrWhiteSpace(Request["NikBM"]) ? "" : Request["NikBM"].ToString());
            string shEmployeeID = (string.IsNullOrWhiteSpace(Request["NikSH"]) ? "" : Request["NikSH"].ToString());
            string scEmployeeID = (string.IsNullOrWhiteSpace(Request["NikSC"]) ? "" : Request["NikSC"].ToString());
            string smEmployeeID = (string.IsNullOrWhiteSpace(Request["NikSL"]) ? "" : Request["NikSL"].ToString());
            string year = (string.IsNullOrWhiteSpace(Request["Year"]) ? "1990" : Request["Year"].ToString());

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_itsinqAchievementSalesType";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", companyCode);
            cmd.Parameters.AddWithValue("@BMEmployeeID", bmEmployeeID);
            cmd.Parameters.AddWithValue("@SHEmployeeID", shEmployeeID);
            cmd.Parameters.AddWithValue("@SCEmployeeID", scEmployeeID);
            cmd.Parameters.AddWithValue("@SMEmployeeID", smEmployeeID);
            cmd.Parameters.AddWithValue("@Year", int.Parse(year));

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult AchievementProspect()
        {
            string companyCode = (string.IsNullOrWhiteSpace(Request["CompanyCode"]) ? "" : Request["CompanyCode"].ToString());
            string bmEmployeeID = (string.IsNullOrWhiteSpace(Request["NikBM"]) ? "" : Request["NikBM"].ToString());
            string shEmployeeID = (string.IsNullOrWhiteSpace(Request["NikSH"]) ? "" : Request["NikSH"].ToString());
            string scEmployeeID = (string.IsNullOrWhiteSpace(Request["NikSC"]) ? "" : Request["NikSC"].ToString());
            string smEmployeeID = (string.IsNullOrWhiteSpace(Request["NikSL"]) ? "" : Request["NikSL"].ToString());
            string year = (string.IsNullOrWhiteSpace(Request["Year"]) ? "1990" : Request["Year"].ToString());

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_itsinqAchievementProsStat";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", companyCode);
            cmd.Parameters.AddWithValue("@BMEmployeeID", bmEmployeeID);
            cmd.Parameters.AddWithValue("@SHEmployeeID", shEmployeeID);
            cmd.Parameters.AddWithValue("@SCEmployeeID", scEmployeeID);
            cmd.Parameters.AddWithValue("@SMEmployeeID", smEmployeeID);
            cmd.Parameters.AddWithValue("@Year", int.Parse(year));

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        #endregion


        public JsonResult InqITS()
        {
            //check nationality
            var areas = new List<dynamic>();
            var dealers = new List<dynamic>();
            var outlets = new List<dynamic>();
            string area="", dealer = "";
            bool isOutlet = false;
            DateTime periodStart, periodEnd;
            var lookUp = ctx.LookUpDtls.Find(CurrentUser.CompanyCode, "QSLS", "NATIONAL");
            int isNational = 0;
            if (lookUp != null)
            {
                isNational = int.Parse(lookUp.ParaValue);
            }

            if (lookUp.ParaValue == "1")
            {
                periodStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                periodEnd = DateTime.Now;
            }
            else
            {
                //get defaultdate period
                var coProfileSales = ctx.CoProfileSaleses.Find(CurrentUser.CompanyCode, CurrentUser.BranchCode);
                periodStart = coProfileSales.PeriodBeg.Value;
                periodEnd = coProfileSales.PeriodEnd.Value;
            }

            var organization = ctx.OrganizationDtls.Find(CurrentUser.CompanyCode, CurrentUser.BranchCode);
            if (organization.IsBranch == true)
            {
                var dealerMaps = ctx.DealerMappings.Where(m => m.isActive == true).ToList();
                if (dealerMaps.Count != 0)
                {
                    var outletMaps = ctx.DealerOutletMappings.Where(m => m.DealerCode == CurrentUser.CompanyCode);
                    foreach (var outletMap in outletMaps)
                    {
                        outlets.Add(new { value = outletMap.OutletCode, text = outletMap.OutletName });
                    }
                    dealers.Add(new { text = dealerMaps[0].DealerName, value = dealerMaps[0].DealerCode });
                    areas.Add(new { text = dealerMaps[0].Area, value = dealerMaps[0].Area });
                    dealer = dealerMaps[0].DealerCode;
                    area = dealerMaps[0].Area;
                    isOutlet = true;
                }
                else
                {
                    foreach (var dealerMap in dealerMaps)
                    {
                        areas.Add(new { text = dealerMap.Area, value = dealerMap.Area});
                    }
                    isOutlet = false;
                }
            }

            return Json(new{
                success = true,
                data = new{
                    periodStart = periodStart,
                    periodEnd = periodEnd,
                    dealer = dealer,
                    area = area,
                    isOutlet = isOutlet,
                    areas = areas,
                    dealers = dealers,
                    outlets = outlets,
                },
            });
        }

        public JsonResult InquiryITS()
        {
            string periodStart = Request["periodStart"];
            string periodEnd = Request["periodEnd"];
            string area = Request["area"];
            string dealer = Request["dealer"];
            string outlet = Request["outlet"];

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_InquiryITS";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@StartDate", periodStart);
            cmd.Parameters.AddWithValue("@EndDate", periodEnd);
            cmd.Parameters.AddWithValue("@Area", area);
            cmd.Parameters.AddWithValue("@DealerCode", dealer);
            cmd.Parameters.AddWithValue("@OutletCode", outlet);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult InquiryITSOutstanding()
        {
            string periodStart = Request["periodStart"];
            string area = Request["area"];
            string dealer = Request["dealer"];
            string outlet = Request["outlet"];
            string summary = Request["summary"];

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_InquiryITSOutsNUpdate";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@StartDate", periodStart);
            cmd.Parameters.AddWithValue("@Area", area);
            cmd.Parameters.AddWithValue("@CompanyCode", dealer);
            cmd.Parameters.AddWithValue("@BranchCode", outlet);
            cmd.Parameters.AddWithValue("@Summary", summary);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult itsLoadInqSummary(String IdTab, String DateFrom, String DateTo, String EmpID1, String EmpID2, String EmpID3, bool IsGM = false) 
        {
            var result = ctx.Database.SqlQuery<SimDms.PreSales.Models.ItsInqSummery>("select * from pmkdp");
            var branchcode = BranchCode;
            if (IsGM && EmpID1 == "" && EmpID2 == "" && EmpID3 == "")
            {
                branchcode = "";
            }
            else if (IsGM && EmpID1 != "")
            {
                branchcode = ctx.HrEmployeeMutations.Where(a => a.CompanyCode == CompanyCode && a.EmployeeID == EmpID1 && a.IsDeleted == false).OrderByDescending(b => b.CreatedDate).FirstOrDefault().BranchCode;             
            }

            var qry = String.Empty;
            qry = String.Format("exec usprpt_PmRpInqSummaryWeb '{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}'", CompanyCode, branchcode, DateFrom, DateTo, EmpID1, EmpID2, EmpID3, IdTab);
            result = ctx.Database.SqlQuery<SimDms.PreSales.Models.ItsInqSummery>(qry);
            return Json (result);
        }

        public JsonResult itsFollowUpGetBranch()
        {
            var Position = "";
            var list = ctx.HrEmployees.Where(p => p.Department == "SALES").Select(p => new {p.CompanyCode, p.EmployeeID, p.RelatedUser, p.Position});
            var eid = list.Where(p => p.CompanyCode == CompanyCode && p.RelatedUser == CurrentUser.UserId).FirstOrDefault();
            if (eid == null && CurrentUser.UserId == "ga") { Position = "COO"; } else { Position = eid.Position; }
            //if (eid.Position != "COO")
            if (Position != "COO")
            {
                var qry = "SELECT TOP 1 BranchCode FROM HrEmployeeMutation WHERE EmployeeID = '" + eid.EmployeeID + "' AND CompanyCode = '" + CompanyCode + "' AND IsDeleted = '0' ORDER BY MutationDate DESC";
                var result = ctx.Database.SqlQuery<SimDms.PreSales.Models.ItsGetBranch>(qry).FirstOrDefault();

                return Json(new { success = true, data = result }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (CurrentUser.UserId == "ga")
                {
                    return Json(new { success = true, data = "" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, data = "" }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        public JsonResult itsUserProperties()
        {
            var outlet = ctx.PmBranchOutlets.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode).FirstOrDefault().OutletID;
            var outletname = ctx.OrganizationDtls.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode).FirstOrDefault().BranchName;
            var list = ctx.HrEmployees.Where(p => p.Department == "SALES").Select(p => new {p.CompanyCode, p.EmployeeID, p.EmployeeName, p.RelatedUser, p.Position, p.TeamLeader});
            var eid = list.Where(p => p.CompanyCode == CompanyCode && p.RelatedUser == CurrentUser.UserId).FirstOrDefault();
            //var Branch = ctx.HrEmployeeMutations.Where(a => a.CompanyCode == CompanyCode && a.EmployeeID == eid.EmployeeID && a.IsDeleted == false).OrderByDescending(b => b.CreatedDate).FirstOrDefault();
            var g = ctx.CoProfiles.Find(CompanyCode, BranchCode);
            var bCoo = false; 
            if (eid != null)
            {
                var AdditionalJob = ctx.HrEmployeeAdditionalJobs.Where(a => a.CompanyCode == CompanyCode && a.EmployeeID == eid.EmployeeID).FirstOrDefault();
                if (eid.Position == "COO" || eid.Position == "CEO" || eid.Position == "GM" )
                {
                    bCoo = true;
                }

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        UserID = CurrentUser.UserId,
                        EmployeeID = eid.EmployeeID,
                        EmployeeName = eid.EmployeeName,
                        //Position = eid.Position,
                        Position = AdditionalJob == null ? eid.Position : eid.Position == AdditionalJob.Position ? eid.Position : eid.Position+AdditionalJob.Position,
                        isCOO = bCoo,
                        Branch = BranchCode,
                        //Branch = Branch == null? null : Branch.BranchCode,
                        ProductType = g.ProductType,
                        cmpCode = CompanyCode,
                        OutletCode = BranchCode,
                        outletID = outlet,
                        outletName = outletname,
                        UpperLvl = eid.TeamLeader
                    }
                });
            }
            else
            {
                if (CurrentUser.UserId == "ga")
                {
                    return Json(new
                    {
                        success = true,
                        data = new
                        {
                            UserID = "", //ctx.HrEmployees.Where(a=> a.CompanyCode == CompanyCode && a.Department == "SALES" && a.PersonnelStatus == "1" && (a.Position == "GM" || a.Position == "COO" || a.Position == "CEO")).FirstOrDefault().RelatedUser,
                            EmployeeID = "", //ctx.HrEmployees.Where(a=> a.CompanyCode == CompanyCode && a.Department == "SALES" && a.PersonnelStatus == "1" && (a.Position == "GM" || a.Position == "COO" || a.Position == "CEO")).FirstOrDefault().EmployeeID,
                            EmployeeName = "", //ctx.HrEmployees.Where(a=> a.CompanyCode == CompanyCode && a.Department == "SALES" && a.PersonnelStatus == "1" && (a.Position == "GM" || a.Position == "COO" || a.Position == "CEO")).FirstOrDefault().EmployeeName,
                            Position = "COO", //ctx.HrEmployees.Where(a=> a.CompanyCode == CompanyCode && a.Department == "SALES" && a.PersonnelStatus == "1" && (a.Position == "GM" || a.Position == "COO" || a.Position == "CEO")).FirstOrDefault().Position,
                            isCOO = true,
                            Branch = BranchCode,
                            //Branch = Branch == null ? null : Branch.BranchCode,
                            ProductType = g.ProductType,
                            cmpCode = CompanyCode,
                            OutletCode = BranchCode,
                            outletID = outlet,
                            outletName = outletname,
                            UpperLvl = ""
                        }
                    });
                }
                else
                {
                    return Json(new { success = false, message = "User belum link dengan salesman" });
                }
            }
        }

        public JsonResult itsLoadFollowUp(string DateFrom, string DateTo, string Branch, string Emp1, string Emp2, string Emp3, string outletID, string Param)
        {
            //var qry = "exec usprpt_PmRpInqFollowUpWeb @p0, @p1, @p2, @p3, @p4, @p5, @p6";
            //var result = ctx.Database.SqlQuery<SimDms.PreSales.Models.ItsInqFollowUp>(qry, CompanyCode, Branch, DateFrom.Date, DateTo.Date, Emp1, Emp2, Emp3);
            var qry = string.Format(@"usprpt_PmRpInqFollowUpWeb '{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}'
                        ", CompanyCode, Branch, DateFrom, DateTo, Emp1, Emp2, Emp3, outletID, Param);
            var result = ctx.Database.SqlQuery<SimDms.PreSales.Models.ItsInqFollowUp>(qry).AsQueryable();
            return Json(result);
        }

        public JsonResult itsLoadFollowUp2(string DateFrom, string DateTo, string Branch, string Emp1, string Emp2, string Emp3)
        {
            //var qry = "exec usprpt_PmRpInqFollowUpWeb2 @p0, @p1, @p2, @p3, @p4, @p5, @p6";
            //var result = ctx.Database.SqlQuery<SimDms.PreSales.Models.ItsInqFollowUp>(qry, CompanyCode, Branch, DateFrom.Date, DateTo.Date, Emp1, Emp2, Emp3);
            var qry = string.Format(@"usprpt_PmRpInqFollowUpWeb2 '{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}'
                        ", CompanyCode, Branch, DateFrom, DateTo, Emp1, Emp2, Emp3);
            var result = ctx.Database.SqlQuery<SimDms.PreSales.Models.ItsInqFollowUp>(qry).AsQueryable();
            return Json(result);
        }
        public JsonResult itsLoadPeriode(string DateFrom, string DateTo, string Branch, string Emp1, string Emp2, string Emp3)
        {
            var qry = "exec usprpt_PmRpInqPeriodeWeb @p0, @p1, @p2, @p3, @p4, @p5, @p6";
            var result = ctx.Database.SqlQuery<SimDms.PreSales.Models.ItsInqFollowUp>(qry, CompanyCode, Branch, DateFrom, DateTo, Emp1, Emp2, Emp3);
            return Json(result);
        }

        private class vAreaCode
        {
            public string Area { get; set; }
        }

        public JsonResult inqGetArea(string grpCode)
        {
           var qry = "SELECT Area FROM gnMstDealerMapping WHERE GroupNo=@p0";
           return Json (ctx.Database.SqlQuery<vAreaCode>(qry, grpCode).FirstOrDefault());
        }

        public JsonResult inqItsLoadQuery(string begindate, string enddate, string area, string dealer, string outlet)
        {
            //var qry = "exec uspfn_InquiryITS @p0, @p1, @p2, @p3, @p4";
            //var result = ctx.Database.SqlQuery<SimDms.PreSales.Models.inqItsLoadData>(qry, begindate, enddate, area, dealer, outlet);
            //return Json(result);

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 1800;
            cmd.CommandText = "uspfn_InquiryITS";

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            cmd.Parameters.AddWithValue("@StartDate", begindate);
            cmd.Parameters.AddWithValue("@EndDate", enddate);
            cmd.Parameters.AddWithValue("@Area", area);
            cmd.Parameters.AddWithValue("@DealerCode", dealer);
            cmd.Parameters.AddWithValue("@OutletCode", outlet);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            return Json(new { success = true, dtbl = dt }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult inqItsMktLoadQuery(string begindate, string enddate, string area, string dealer, string outlet)
        {
            //var qry = "exec uspfn_OmInquiryMKT @p0, @p1, @p2, @p3, @p4";
            //var result = ctx.Database.SqlQuery<SimDms.PreSales.Models.inqItsMktLoadData>(qry, begindate, enddate, area, dealer, outlet);
            //return Json(result);

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 1800;
            cmd.CommandText = "uspfn_OmInquiryMKT";

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            cmd.Parameters.AddWithValue("@StartDate", begindate);
            cmd.Parameters.AddWithValue("@EndDate", enddate);
            cmd.Parameters.AddWithValue("@Area", area);
            cmd.Parameters.AddWithValue("@DealerCode", dealer);
            cmd.Parameters.AddWithValue("@OutletCode", outlet);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            return Json(new { success = true, dtbl = dt }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetGenerateFollowUp(string Branch, string DateFrom, string DateTo, string outletID, string Emp, string Param, string Head)
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 1800;
            cmd.CommandText = "usprpt_PmRpInqFollowUpDtlNew";

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", Branch);
            cmd.Parameters.AddWithValue("@DateAwal", DateFrom);
            cmd.Parameters.AddWithValue("@DateAkhir", DateTo);
            cmd.Parameters.AddWithValue("@Outlet", outletID);
            cmd.Parameters.AddWithValue("@EMP", Emp);
            cmd.Parameters.AddWithValue("@Param", Param);
            cmd.Parameters.AddWithValue("@Head", Head);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dtResult = new DataTable("datTable1");
            da.Fill(dtResult);

            //StringBuilder sb = new StringBuilder();
            string sb = GenSITSR(dtResult);

            var WFRES = new byte[sb.Length * sizeof(char)];
            Buffer.BlockCopy(sb.ToCharArray(), 0, WFRES, 0, WFRES.Length);
            Session.Add("WFRES", WFRES);

            return Json(new { success = true, contents = sb });

        }

        private string GenSITSR(DataTable dtResult)
        {
            string dealercode = "", branchcode = "", dealerName = "", branchName = "", branchManager = "";

            string alamatProspek, telpRumah, alamatPer, handPhone, employee, grade, salesCoo, lostCase, testDrive,
                salesHead, merkLain, voiceOfCust, caraBayar, leasing, downPayment, tenor;
            string dayInq, monthInq, daySpk, monthSpk, dayFoll, monthFoll;

            dealerName = CompanyName;
            branchName = BranchName;
            dealercode = CompanyCode;
            branchcode = BranchCode;

            StringBuilder sb = new StringBuilder();
            sb.Append("H");
            sb.Append("SITSR");
            sb.Append((dealercode.Length > 10) ? dealercode.Substring(0, 10) : dealercode.PadRight(10, ' '));
            sb.Append(((string)"1000000").PadRight(10, ' '));
            sb.Append((dealerName.Length > 50) ? dealerName.Substring(0, 50) : dealerName.PadRight(50, ' '));
            sb.Append((branchcode.Length > 10) ? branchcode.Substring(0, 10) : branchcode.PadRight(10, ' '));
            sb.Append((branchName.Length > 50) ? branchName.Substring(0, 50) : branchName.PadRight(50, ' '));
            sb.Append((branchManager.Length > 50) ? branchManager.Substring(0, 50) : branchManager.PadRight(50, ' '));
            sb.Append(dtResult.Rows.Count.ToString().PadLeft(6, '0'));
            sb.Append(((string)" ").PadRight(74, ' '));

            foreach (DataRow row in dtResult.Rows)
            {
                alamatProspek = row["AlamatProspek"].ToString();
                telpRumah = row["TelpRumah"].ToString();
                alamatPer = row["AlamatPerusahaan"].ToString();
                handPhone = row["Handphone"].ToString();
                employee = row["Employee"].ToString();
                lostCase = row["LostCaseCategory"].ToString();
                testDrive = row["TestDrive"].ToString();
                merkLain = row["MerkLain"].ToString();
                voiceOfCust = row["LostCaseVoiceOfCustomer"].ToString();
                caraBayar = row["CaraBayar"].ToString();
                leasing = row["Leasing"].ToString();
                downPayment = row["DownPayment"].ToString();
                tenor = row["Tenor"].ToString();

                sb.AppendLine();
                sb.Append("1");
                sb.Append(row["InquiryNumber"].ToString().PadLeft(18, '0'));

                dayInq = "00" + row["DayInquiryDate"].ToString();
                monthInq = "00" + row["MonthInquiryDate"].ToString();
                sb.Append(dayInq.Substring(dayInq.Length - 2, 2));
                sb.Append(monthInq.Substring(monthInq.Length - 2, 2));
                sb.Append(row["YearInquiryDate"].ToString().PadRight(4, ' '));

                sb.Append(row["TipeKendaraan"].ToString().PadRight(20, ' '));
                sb.Append(row["Variant"].ToString().PadRight(50, ' '));
                sb.Append(row["Warna"].ToString().PadRight(100, ' '));
                sb.Append(row["PerolehanData"].ToString().PadRight(15, ' '));
                sb.Append(row["QuantityInquiry"].ToString().PadRight(18, ' '));

                dayFoll = "00" + row["DayNextFollowUpDate"].ToString();
                monthFoll = "00" + row["MonthNextFollowUpDate"].ToString();
                sb.Append(dayFoll.Substring(dayFoll.Length - 2, 2));
                sb.Append(monthFoll.Substring(monthFoll.Length - 2, 2));
                sb.Append(row["YearNextFollowUpDate"].ToString().PadRight(4, ' '));

                sb.Append(row["LastProgress"].ToString().PadRight(15, ' '));

                daySpk = "00" + row["DaySPKDate"].ToString();
                monthSpk = "00" + row["MonthSPKDate"].ToString();
                sb.Append(daySpk.Substring(daySpk.Length - 2, 2));
                sb.Append(monthSpk.Substring(monthSpk.Length - 2, 2));
                sb.Append(row["YearSPKDate"].ToString().PadRight(4, ' '));
                sb.Append(((string)"").PadRight(5, ' '));

                sb.AppendLine();
                sb.Append("2");
                sb.Append(row["Pelanggan"].ToString().PadRight(50, ' '));
                sb.Append((alamatProspek.Length > 200) ? alamatProspek.Substring(0, 200) : alamatProspek.PadRight(200, ' '));
                sb.Append((telpRumah.Length > 15) ? telpRumah.Substring(0, 15) : telpRumah.PadRight(15, ' '));

                sb.AppendLine();
                sb.Append("3");
                sb.Append(row["NamaPerusahaan"].ToString().PadRight(50, ' '));
                sb.Append((alamatPer.Length > 200) ? alamatPer.Substring(0, 200) : alamatPer.PadRight(200, ' '));
                sb.Append((handPhone.Length > 15) ? handPhone.Substring(0, 15) : handPhone.PadRight(15, ' '));

                sb.AppendLine();
                sb.Append("4");
                sb.Append((employee.Length > 100) ? employee.Substring(0, 100) : employee.PadRight(100, ' '));
                sb.Append((lostCase.Length > 50) ? lostCase.Substring(0, 50) : lostCase.PadRight(50, ' '));

                sb.AppendLine();
                sb.Append("5");
                sb.Append((testDrive.Length > 15) ? testDrive.Substring(0, 15) : testDrive.PadRight(15, ' '));
                sb.Append((merkLain.Length > 50) ? merkLain.Substring(0, 50) : merkLain.PadRight(50, ' '));
                sb.Append(((string)"").PadRight(100, ' '));

                sb.AppendLine();
                sb.Append("6");
                sb.Append((voiceOfCust.Length > 200) ? voiceOfCust.Substring(0, 200) : voiceOfCust.PadRight(200, ' '));
                sb.Append((caraBayar.Length > 15) ? caraBayar.Substring(0, 15) : caraBayar.PadRight(15, ' '));
                sb.Append((leasing.Length > 30) ? leasing.Substring(0, 30) : leasing.PadRight(30, ' '));
                sb.Append((downPayment.Length > 15) ? downPayment.Substring(0, 15) : downPayment.PadRight(15, ' '));
                sb.Append((tenor.Length > 2) ? tenor.Substring(0, 2) : tenor.PadLeft(2, '0'));
                sb.Append(((string)"").PadRight(3, ' '));
            }
            return sb.ToString();
        }

        public FileContentResult SaveGenerateFollowUp()
        {
            var file = Session["WFRES"] as byte[];

            Session.Clear();

            var ms = new MemoryStream(file);
            string contentType = "application/text";

            Response.Clear();
            Response.ContentType = contentType;
            Response.AddHeader("content-disposition", "attachment;filename=WFRES.txt");
            Response.Buffer = true;
            ms.WriteTo(Response.OutputStream);
            Response.End();

            return File(file, contentType, "WFRES.txt");
        }

        public JsonResult ValidateHeaderFile(string Contents)
        {
            string DataId = "SITSR";
            var result = true;
            var msg = "";

            string header = Contents.Split('\n')[0];

            string qry = string.Format("select * from gnDcsUploadFile where DataID = '{0}' and Header = '{1}'", DataId, header);
            var dt = ctx.Database.SqlQuery<GnDcsUploadFile>(qry);
            if (dt.Count() > 0)
            {
                result = false;
                msg = string.Format("Data {0} sudah pernah dikirim pada {1}, apakah akan dikirim ulang?", DataId, dt.FirstOrDefault().CreatedDate);
            }

            return Json(new { success = result, message = msg });
        }

        public JsonResult SendFile(string Contents)
        {
            string DataId = "SITSR";
            var ProductType = ctx.CoProfiles.Where(a=> a.CompanyCode == CompanyCode && a.BranchCode == BranchCode).FirstOrDefault().ProductType;

            DcsWsSoapClient ws = new DcsWsSoapClient();

            string header = Contents.Split('\n')[0];

            var msg = "";

            Session.Clear();

            try
            {
                string result = ws.SendToDcs(DataId, CompanyCode, header, ProductType);
                if (result.StartsWith("FAIL")) return Json(new { success = false, message = result.Substring(5) });

                LogHeaderFile(DataId, CompanyCode, header, ProductType);
                msg = string.Format("{0} berhasil di upload", DataId);
                return Json(new { success = true, message = msg });
            }
            catch
            {
                msg = string.Format("{0} gagal digenerate", DataId);
                return Json(new { success = false, message = msg });
            }
        }

        private void LogHeaderFile(string dataID, string custCode, string header, string prodType)
        {
            string query = "exec uspfn_spLogHeader @p0,@p1,@p2,@p3,@p4,@p5";
            object[] Parameters = { dataID, custCode, prodType, "SEND", DateTime.Now, header };
            ctx.Database.ExecuteSqlCommand(query, Parameters);
        }


        public JsonResult inqItsStatusLoadQuery(string begindate, string enddate, string area, string dealer, string outlet, string groupModel, string tipeKendaraan, string variant, bool summary)
        {
            //var qry = "exec uspfn_InquiryITSStatusQuery @p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8";
            //var result = ctx.Database.SqlQuery<SimDms.PreSales.Models.inqItsStatusLoadData>(qry, begindate, enddate, area, dealer, outlet, groupModel, tipeKendaraan, variant, summary ? "1" : "0");
            //return Json(result);

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 1800;
            cmd.CommandText = "uspfn_InquiryITSStatusQuery_Rev";

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            cmd.Parameters.AddWithValue("@StartDate", begindate);
            cmd.Parameters.AddWithValue("@EndDate", enddate);
            cmd.Parameters.AddWithValue("@Area", area);
            cmd.Parameters.AddWithValue("@CompanyCode", dealer);
            cmd.Parameters.AddWithValue("@BranchCode", outlet);
            cmd.Parameters.AddWithValue("@GroupModel", groupModel);
            cmd.Parameters.AddWithValue("@TipeKendaraan", tipeKendaraan);
            cmd.Parameters.AddWithValue("@Variant", variant);
            cmd.Parameters.AddWithValue("@Summary", summary);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            return Json(new { success = true, dtbl = dt }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult itsPivotFollowUp()
        {
            string DateFrom = Request["DateFrom"] ?? "";
            string DateTo = Request["DateTo"] ?? "";
            string Branch = Request["Branch"] ?? "";
            string Emp1 = Request["Emp1"] ?? "";
            string Emp2 = Request["Emp2"] ?? "";
            string Emp3 = Request["Emp3"] ?? "";

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 1800;
            cmd.CommandText = Request["PivotId"];

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", Branch);
            cmd.Parameters.AddWithValue("@PeriodBegin", DateFrom);
            cmd.Parameters.AddWithValue("@PeriodEnd", DateTo);
            cmd.Parameters.AddWithValue("@BranchManager", Emp1);
            cmd.Parameters.AddWithValue("@SalesHead", Emp2);
            cmd.Parameters.AddWithValue("@Salesman", Emp3);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("datTable1");
            DataSet ds = new DataSet();
            da.Fill(ds);
            dt = ds.Tables[0];

            return Json(new { success = true, data = dt }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult itsUserPropertiesAdmin()
        {
            var user = ctx.SysRoleUsers.Where(p => p.UserID == CurrentUser.UserId && p.RoleID == "ADMIN").FirstOrDefault();
            var g = ctx.CoProfiles.Find(CompanyCode, BranchCode);
            var h = ctx.DealerOutletMappings.Where(a => a.DealerCode == CompanyCode && a.OutletCode == BranchCode).FirstOrDefault();
            if (user != null)
            {
                return Json(new
                {
                    success = true,
                    data = new
                    {
                        UserID = CurrentUser.UserId,
                        ProductType = g.ProductType,
                        cmpCode = CompanyCode,
                        OutletCode = BranchCode,
                        OutletName = h.OutletName
                    }
                });
            }
            else
            {
                return Json(new { success = false, message = "Maaf,, Anda bukan login sebagai user ADMIN" });
            }
        }

        public JsonResult getBranchEmployee(string Employeeid)
        {
            var Branch = ctx.HrEmployeeMutations.Where(p => p.EmployeeID == Employeeid && p.IsDeleted == false).OrderByDescending(a => a.CreatedDate).FirstOrDefault().BranchCode;
            if (Branch != null)
            {
                return Json(new
                {
                    success = true,
                    data = new
                    {
                        Branch = Branch
                    }
                });
            }
            else
            {
                return Json(new { success = false, message = "Maaf,, Employee Tidak Memiliki Branch" });
            }
        }

        public JsonResult defaultproductivity()
        {
            var SHID = String.Empty;
            var BMID = String.Empty;
            var SMID = String.Empty;
            var SHName = String.Empty;
            var BMName = String.Empty;
            var SMName = String.Empty;
            var branchcode = BranchCode;
            var bCoo = false;
            //Position position = ctx.Positions.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.UserId == CurrentUser.UserId).FirstOrDefault();
            var position = ctx.HrEmployees.Where(p => p.CompanyCode == CompanyCode && p.RelatedUser == CurrentUser.UserId).FirstOrDefault();
            var Mutations = ctx.HrEmployeeMutations.Where(p => p.CompanyCode == CompanyCode && p.EmployeeID == position.EmployeeID && p.IsDeleted == false).OrderByDescending(a => a.CreatedDate).FirstOrDefault();
            if (Mutations != null)
            {
                branchcode = Mutations.BranchCode;
            }

            if (position == null)
            {
                return Json(new { success = false, message = "Anda tidak mendapatkan akses untuk menu ini" });
            }

            if (position.Position == "COO" || position.Position == "CEO" || position.Position == "GM")
            {
                bCoo = true;
            }

            if (position.Position == "BM")
            {
                BMName = position.EmployeeName;
                BMID = position.EmployeeID;
            }

            var empBM = ctx.HrEmployees.FirstOrDefault();

            if (position.Position == "SH")
            {
                empBM = ctx.HrEmployees.Where(a => a.EmployeeID == position.TeamLeader).FirstOrDefault();
                SHName = position.EmployeeName;
                BMName = empBM.EmployeeName;
                SHID = position.EmployeeID;
                BMID = empBM.EmployeeID;
            }

            var empSH = ctx.HrEmployees.FirstOrDefault();
            if (position.Position == "S")
            {
                empSH = ctx.HrEmployees.Where(a => a.EmployeeID == position.TeamLeader).FirstOrDefault();
                empBM = ctx.HrEmployees.Where(a => a.EmployeeID == empSH.TeamLeader).FirstOrDefault();
                SHName = empSH.EmployeeName;
                BMName = empBM.EmployeeName;
                SHID = empSH.EmployeeID;
                BMID = empBM.EmployeeID;
                SMID = position.EmployeeID;
                SMName = position.EmployeeName;
            }

            string sql = string.Format("exec uspfn_gnInquiryBtn 'CABANG', '{0}', '', '2'", CompanyCode);
            var AreaList = (from p in ctx.Database.SqlQuery<SimDms.PreSales.Models.InquiryBtn>(sql)
                            where p.GroupNo != "000"
                            select new SimDms.PreSales.Models.InquiryBtn()
                            {
                                Area = p.Area
                            }).Select(p => new { value = p.Area, text = p.Area }).FirstOrDefault();

            var dealerList = (from p in ctx.Database.SqlQuery<SimDms.PreSales.Models.InquiryBtn>(sql)
                              where p.GroupNo != "000"
                              select new SimDms.PreSales.Models.InquiryBtn()
                              {
                                  DealerCode = p.DealerCode,
                                  DealerName = p.DealerName
                              }).Select(p => new { value = p.DealerCode, text = p.DealerName }).FirstOrDefault();

            return Json(new
            {
                success = true,
                data = new
                {
                    CompanyCode = CompanyCode,
                    CompanyName = CompanyName,
                    BranchCode = branchcode,
                    //BranchName = BranchName,
                    IsBranch = IsBranch,
                    DateFrom = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                    DateTo = DateTime.Now,
                    PositionID = (position == null) ? "0" : position.Position,
                    EmployeeID = (position == null) ? "" : position.EmployeeID,
                    Salesman = SMID,
                    SalesmanName = SMName,
                    SalesHead = SHID,
                    BranchManager = BMID,
                    SalesHeadName = SHName,
                    BranchManagerName = BMName,
                    NationalSLS = NationalSLS,
                    Area = (NationalSLS == "0") ? AreaList.value : "",
                    Dealer = (NationalSLS == "0") ? dealerList.value : "",
                    Outlet = (IsBranch) ? branchcode : "",
                    IsGM = bCoo
                }
            });

        }    
    }
}
