using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Service.Models;

namespace SimDms.Service.Controllers.Api
{
    public class KsgInvController : BaseController
    {
        //
        // GET: /KsgInv/

        public JsonResult Default()
        {
            var gDate = Convert.ToDateTime(ctx.CoProfileServices.Find(CompanyCode, BranchCode).TransDate);

            return Json(new
            {
                CompanyCode = CompanyCode,
                CompanyName = CompanyName,
                BranchCode = BranchCode,
                BranchName = BranchName,
                GenerateDate = new DateTime(gDate.Year, gDate.Month, gDate.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second),
                IsBranch = IsBranch,
            });
        }

        public JsonResult GetPdiFsc(string invoiceFrom, string invoiceTo, bool isCampaign, int jobType)
        {
            bool isPdi = false;
            bool isFsc = false;
            bool isFscCampaign = false;

            switch (jobType)
            {
                case 0:
                    isPdi = true;
                    break;
                case 1:
                    isFsc = true;
                    break;
                case 2:
                    isFscCampaign = true;
                    break;
            }

            DataSet ds = new DataSet();
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;

            cmd.CommandText = "uspfn_SvTrnGetPdiFsc";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@InvoiceFrom", invoiceFrom);
            cmd.Parameters.AddWithValue("@InvoiceTo", invoiceTo);
            cmd.Parameters.AddWithValue("@IsCampaign", isCampaign);
            cmd.Parameters.AddWithValue("@IsPdi", isPdi);
            cmd.Parameters.AddWithValue("@IsFsc", isFsc);
            cmd.Parameters.AddWithValue("@IsFscCampaign", isFscCampaign);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            if (ds != null)
            {
                return Json(new { success = true, info = GetJson(ds.Tables[0]), total = GetJson(ds.Tables[1]), totalitem = ds.Tables[0].Rows.Count });
            }
            else
            {
                return Json("NoData", JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetPdiFscAll(string branchFrom, string branchTo, DateTime PeriodeDateFrom, DateTime PeriodeDateTo, bool isCampaign, int jobType)
        {
            var ds = GetPdiFscAllBranch(branchFrom, branchTo, PeriodeDateFrom, PeriodeDateTo, isCampaign, jobType);

            if (ds != null)
            {
                return Json(new { success = true, form = GetJson(ds.Tables[0]), info = GetJson(ds.Tables[1]), totalitem = ds.Tables[1].Rows.Count });
            }
            else
            {
                return Json("NoData", JsonRequestBehavior.AllowGet);
            }

        }

        private DataSet GetPdiFscAllBranch(string branchFrom, string branchTo, DateTime PeriodeDateFrom, DateTime PeriodeDateTo, bool isCampaign,int jobType)
        {
            var gDate = Convert.ToDateTime(ctx.CoProfileServices.Find(CompanyCode, BranchCode).TransDate);
            var GenerateDate = new DateTime(gDate.Year, gDate.Month, gDate.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

            string group = "";
            group = jobType == 0 ? "PDI%" : "FSC%";

            bool isFscCampaign = false;
            isFscCampaign = jobType == 2 ? true : false;

            DataSet ds = new DataSet();
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;

            cmd.CommandText = "uspfn_SvInqPdiFscAllBranch";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@ProductType", ProductType);
            cmd.Parameters.AddWithValue("@BranchCode1", branchFrom);
            cmd.Parameters.AddWithValue("@BranchCode2", branchTo);
            cmd.Parameters.AddWithValue("@InvoiceDate1", PeriodeDateFrom);
            cmd.Parameters.AddWithValue("@InvoiceDate2", PeriodeDateTo);
            cmd.Parameters.AddWithValue("@IsCampaign", isCampaign);
            cmd.Parameters.AddWithValue("@IsFSCCampaign", isFscCampaign);
            cmd.Parameters.AddWithValue("@TransDate", GenerateDate);
            cmd.Parameters.AddWithValue("@GroupJobType", group);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            return ds;
        }

        public JsonResult GetKsg(string genno)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;

            cmd.CommandText = "uspfn_SvGetKsg";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@ProductType", ProductType);
            cmd.Parameters.AddWithValue("@GenerateNo", genno);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            if (ds != null)
            {
                return Json(new { success = true, form = GetJson(ds.Tables[0]), info = GetJson(ds.Tables[1]), total = GetJson(ds.Tables[2]), totalitem = ds.Tables[1].Rows.Count });
            }
            else
            {
                return Json("NoData", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Save(string invoiceFrom, string invoiceTo, bool isCampaign, int jobType)
        {
            var gDate = Convert.ToDateTime(ctx.CoProfileServices.Find(CompanyCode, BranchCode).TransDate);
            var GenerateDate = new DateTime(gDate.Year, gDate.Month, gDate.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

            bool isPdi = false;
            bool isFsc = false;
            bool isFscCampaign = false;

            switch (jobType)
            {
                case 0:
                    isPdi = true;
                    break;
                case 1:
                    isFsc = true;
                    break;
                case 2:
                    isFscCampaign = true;
                    break;
            }

            object[] parameters = { CompanyCode, BranchCode, invoiceFrom, invoiceTo, isCampaign, isPdi, isFsc, isFscCampaign, GenerateDate, CurrentUser.UserId };

            var docNo = ctx.Database.SqlQuery<string>("exec uspfn_SvTrnSavePdiFsc {0},{1},{2},{3},{4},{5},{6},{7},{8},{9}", parameters);

            if (docNo != null)
            {
                return Json(new { success = true, genno = docNo, message = "Data Saved" });
            }
            else
            {
                return Json(new { message = "Failed Save Data" });
            }
        }

        public ActionResult Delete(string GenerateNo)
        {
            bool deleted = false;
            try
            {
                object[] parameters = { CompanyCode, BranchCode, ProductType, GenerateNo, CurrentUser.UserId };
                ctx.Database.ExecuteSqlCommand("exec uspfn_SvUtlKsgDelete {0},{1},{2},{3},{4}", parameters);
                deleted = true;
            }
            catch { }

            if (deleted)
            {
                return Json(new { success = true, message = "Data Deleted" });
            }
            return View();
        }

        public ActionResult SaveAll(string branchFrom, string branchTo, DateTime PeriodeDateFrom, DateTime PeriodeDateTo, bool isCampaign, int jobType)
        {
            var Holding = ctx.OrganizationDtls.Where(a=>a.CompanyCode == CompanyCode && a.IsBranch == false).ToList();

            if (Holding != null && Holding.Count() > 1)
            {
                return Json(new { message = "Terdapat Lebih Dari Satu User Holding, Data Tidak Bisa Disimpan !" });
            }
            else
            {
                var gDate = Convert.ToDateTime(ctx.CoProfileServices.Find(CompanyCode, BranchCode).TransDate);
                var GenerateDate = new DateTime(gDate.Year, gDate.Month, gDate.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

                var ds = GetPdiFscAllBranch(branchFrom, branchTo, PeriodeDateFrom, PeriodeDateTo, isCampaign, jobType);
                DataTable dtInvoice = ds.Tables[0];

                if (dtInvoice.Rows.Count <= 0)
                {
                    return Json(new { success = false, generateNo = "" });
                }

                DateTime svrdate = DateTime.Now;

                var generateNo = GetNewDocumentNo("FSC", GenerateDate);

                try
                {
                    // Insert Pdi Fsc
                    PdiFsc oPdiFsc = new PdiFsc();
                    oPdiFsc.CompanyCode = CompanyCode;
                    oPdiFsc.BranchCode = Holding[0].BranchCode;
                    oPdiFsc.ProductType = ProductType;
                    oPdiFsc.GenerateNo = generateNo;
                    oPdiFsc.GenerateDate = GenerateDate;
                    oPdiFsc.SourceData = "0";
                    oPdiFsc.FromInvoiceNo = string.Format("{0}", dtInvoice.Rows[0]["InvoiceNo"]);
                    oPdiFsc.ToInvoiceNo = string.Format("{0}", dtInvoice.Rows[dtInvoice.Rows.Count - 1]["InvoiceNo"]);
                    oPdiFsc.FPJNo = string.Format("{0}", dtInvoice.Rows[0]["FPJNo"]);
                    oPdiFsc.FPJDate = (dtInvoice.Rows[0]["FPJDate"].ToString() == "" || dtInvoice.Rows[0]["FPJDate"].ToString() == null)
                                            ? Convert.ToDateTime("1900-01-01") : Convert.ToDateTime(dtInvoice.Rows[0]["FPJDate"]);
                    oPdiFsc.FPJGovNo = string.Format("{0}", dtInvoice.Rows[0]["FPJGovNo"]); ;
                    oPdiFsc.SenderDealerCode = CompanyCode;
                    oPdiFsc.ReceiverDealerCode = CompanyCode;
                    oPdiFsc.SenderDealerName = CompanyName;
                    oPdiFsc.RefferenceNo = string.Empty;
                    oPdiFsc.RefferenceDate = Convert.ToDateTime("1900-01-01 00:00:00.000"); 
                    oPdiFsc.TotalNoOfItem = dtInvoice.Rows.Count;
                    oPdiFsc.IsCampaign = Convert.ToBoolean(dtInvoice.Rows[0]["IsCampaign"]);
                    oPdiFsc.PostingFlag = "2";
                    oPdiFsc.TotalLaborAmt = 0;
                    oPdiFsc.TotalMaterialAmt = 0;
                    oPdiFsc.TotalAmt = 0;
                    for (int i = 0; i < dtInvoice.Rows.Count; i++)
                    {
                        oPdiFsc.TotalLaborAmt += Convert.ToDecimal(dtInvoice.Rows[i]["LaborAmount"]);
                        oPdiFsc.TotalMaterialAmt += Convert.ToDecimal(dtInvoice.Rows[i]["MaterialAmount"]);
                        oPdiFsc.TotalAmt += Convert.ToDecimal(dtInvoice.Rows[i]["PdiFscAmount"]); ;
                    }
                    oPdiFsc.TotalLaborPaymentAmt = 0;
                    oPdiFsc.TotalMaterialPaymentAmt = 0;
                    oPdiFsc.TotalPaymentAmt = 0;
                    oPdiFsc.BatchNo = "";
                    oPdiFsc.CreatedBy = CurrentUser.UserId;
                    oPdiFsc.CreatedDate = svrdate;
                    oPdiFsc.LastupdateBy = CurrentUser.UserId;
                    oPdiFsc.LastupdateDate = svrdate;
                    if (ctx.PdiFscs.Add(oPdiFsc) != null)
                    {
                        for (int i = 0; i < dtInvoice.Rows.Count; i++)
                        {
                            PdiFscApplication oPdiFscApp = new PdiFscApplication();
                            oPdiFscApp.CompanyCode = oPdiFsc.CompanyCode;
                            oPdiFscApp.BranchCode = oPdiFsc.BranchCode;
                            oPdiFscApp.ProductType = oPdiFsc.ProductType;
                            oPdiFscApp.GenerateNo = oPdiFsc.GenerateNo;
                            oPdiFscApp.GenerateSeq = i + 1;
                            oPdiFscApp.BranchCodeInv = dtInvoice.Rows[i]["BranchCode"].ToString();
                            oPdiFscApp.InvoiceNo = dtInvoice.Rows[i]["InvoiceNo"].ToString();
                            oPdiFscApp.PdiFscStatus = "0";
                            oPdiFscApp.ServiceBookNo = dtInvoice.Rows[i]["ServiceBookNo"].ToString();
                            oPdiFscApp.BasicModel = dtInvoice.Rows[i]["BasicModel"].ToString();
                            oPdiFscApp.TransmissionType = dtInvoice.Rows[i]["TransmissionType"].ToString();
                            oPdiFscApp.ChassisCode = dtInvoice.Rows[i]["ChassisCode"].ToString();
                            oPdiFscApp.ChassisNo = Convert.ToDecimal(dtInvoice.Rows[i]["ChassisNo"]);
                            oPdiFscApp.EngineCode = dtInvoice.Rows[i]["EngineCode"].ToString();
                            oPdiFscApp.EngineNo = Convert.ToDecimal(dtInvoice.Rows[i]["EngineNo"]);
                            oPdiFscApp.PdiFsc = Convert.ToDecimal(dtInvoice.Rows[i]["PdiFscSeq"]);
                            oPdiFscApp.ServiceDate = Convert.ToDateTime(dtInvoice.Rows[i]["ServiceDate"]);
                            if (!(dtInvoice.Rows[i]["DeliveryDate"] is DBNull))
                                oPdiFscApp.DeliveryDate = Convert.ToDateTime(dtInvoice.Rows[i]["DeliveryDate"]);
                            if (!(dtInvoice.Rows[i]["RegisteredDate"] is DBNull))
                                oPdiFscApp.RegisteredDate = Convert.ToDateTime(dtInvoice.Rows[i]["RegisteredDate"]);
                            oPdiFscApp.Odometer = Convert.ToDecimal(dtInvoice.Rows[i]["Odometer"]);
                            oPdiFscApp.LaborAmount = Convert.ToDecimal(dtInvoice.Rows[i]["LaborAmount"]);
                            oPdiFscApp.MaterialAmount = Convert.ToDecimal(dtInvoice.Rows[i]["MaterialAmount"]);
                            oPdiFscApp.PdiFscAmount = Convert.ToDecimal(dtInvoice.Rows[i]["PdiFscAmount"]);
                            oPdiFscApp.CreatedBy = oPdiFsc.CreatedBy;
                            oPdiFscApp.CreatedDate = oPdiFsc.CreatedDate;
                            oPdiFscApp.LastupdateBy = oPdiFsc.LastupdateBy;
                            oPdiFscApp.LastupdateDate = oPdiFsc.LastupdateDate;
                            ctx.PdiFscApplications.Add(oPdiFscApp);
                        }

                        string group = "";
                        group = jobType == 0 ? "PDI%" : "FSC%";

                        object[] postingParams = { CompanyCode, ProductType, branchFrom, branchTo, PeriodeDateFrom, PeriodeDateTo, isCampaign, group, CurrentUser.UserId };
                        ctx.Database.ExecuteSqlCommand("exec uspfn_SvInvUpdPostingFlag {0},{1},{2},{3},{4},{5},{6},{7},{8}", postingParams);

                    }

                    ctx.SaveChanges();
                    if (Request.IsAjaxRequest())
                    {
                        return Json(new { success = true, genNo = generateNo, message = "Data Saved" });
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }

            }

            return View();
        }
    }
}
