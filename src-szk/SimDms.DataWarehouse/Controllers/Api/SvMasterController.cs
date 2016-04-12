using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GeLang;
using SimDms.DataWarehouse.Models;
using SimDms.DataWarehouse.Helpers;
using System.IO;
using Microsoft.Reporting.WebForms;

namespace SimDms.DataWarehouse.Controllers.Api
{
    public class SvMasterController : BaseController
    {
        
        public JsonResult GetSvMStMRSRData() 
        {
            //var VIN = Request["VIN"] ?? "0";
            var data = ctx.Database.SqlQuery<ReffService>("select ProductType, RefferenceType, Description, RefferenceCode from svMstRefferenceService").ToList();
            return Json(new {result= data.Count, data = data });
        }

        [HttpPost]
        public JsonResult SvMStMRSRSave(ReffService model) 
        {
            ResultModel result = InitializeResult();
            var companycode = model.CompanyCode == null ? "0000000" : model.CompanyCode;
            var data = ctx.ReffServices.Find(companycode, model.ProductType, model.RefferenceType, model.RefferenceCode);
            if (data == null)
            {
                data = new ReffService();
                data.CompanyCode = companycode;
                data.ProductType = model.ProductType;
                data.RefferenceType = model.RefferenceType;
                data.RefferenceCode = model.RefferenceCode;

                ctx.ReffServices.Add(data);
            }

            data.Description = model.Description;
            data.IsActive = true;
            data.CreatedBy = CurrentUser.Username;
            data.CreatedDate = DateTime.Now;
            try
            {
                ctx.SaveChanges();

                result.status = true;
                result.message = "Data has been saved.";
            }
            catch (Exception)
            {
                result.message = "Sorry, Data cannot be saved into database.\nPlease, try again later!";
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult SvMStMRSRDelete(ReffService model)
        {
            ResultModel result = InitializeResult();
            var companycode = model.CompanyCode == "" ? "0000000" : model.CompanyCode;
            var module = ctx.ReffServices.Find(companycode, model.ProductType, model.RefferenceType, model.RefferenceCode);
            if (module != null)
            {
                ctx.ReffServices.Remove(module);
                try
                {
                    ctx.SaveChanges();

                    result.status = true;
                    result.message = "Data has been deleted.";
                }
                catch (Exception)
                {
                    result.message = "Sorry, data cannot be removed from database.\nPlease, try again later!";
                }
            }
            else
            {
                result.message = "There is no data to be deleted.";
            }

            return Json(result);
        }

        public JsonResult findComboSrvByComboMarketing()
        {
            string Type = Request["Type"] ?? "";
            string GroupNo = Request["GroupNo"] ?? "";
            string CompanyCode = Request["CompanyCode"] ?? "";
            string BranchCode = Request["BranchCode"] ?? "";
            try
            {
                SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
                cmd.CommandTimeout = 3600;
                cmd.CommandText = "uspfn_findComboSrvByComboMarketing";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();

                cmd.Parameters.AddWithValue("Type", Type);
                cmd.Parameters.AddWithValue("GroupNo", GroupNo);
                cmd.Parameters.AddWithValue("CompanyCode", CompanyCode);
                cmd.Parameters.AddWithValue("BranchCode", BranchCode);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);
                return new LargeJsonResult() { Data = GetJson(ds), MaxJsonLength = int.MaxValue };
            }
            catch (Exception e)
            {
                return Json(new { message = e.Message });
            }
        }

        public JsonResult SaveMappingComboSrvMarketing()
        {
            string Type = Request["Type"] ?? "";
            string GroupNo = Request["GroupNo"] ?? "";
            string CompanyCode = Request["CompanyCode"] ?? "";
            string BranchCode = Request["BranchCode"] ?? "";
            string GroupNoSrv = Request["GroupNoSrv"] ?? "";
            string GroupAreaSrv = Request["GroupAreaSrv"] ?? "";
            string CompanyCodeSrv = Request["CompanyCodeInput"] ?? "";
            string CompanyNameSrv = Request["CompanyNameInput"] ?? "";
            string BranchCodeSrv = Request["BranchCodeInput"] ?? "";
            string BranchNameSrv = Request["BranchNameInput"] ?? "";
            /*
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;
            cmd.CommandText = "uspfn_SaveMappingComboSrvGn";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            cmd.Parameters.AddWithValue("Type", Type);
            cmd.Parameters.AddWithValue("GroupNoGn", GroupNo);
            cmd.Parameters.AddWithValue("CompanyCodeGn", CompanyCode);
            cmd.Parameters.AddWithValue("BranchCodeGn", BranchCode);
            cmd.Parameters.AddWithValue("GroupNoSrv", GroupNoSrv);
            cmd.Parameters.AddWithValue("GroupAreaSrv", GroupAreaSrv);
            cmd.Parameters.AddWithValue("CompanyCodeSrv", CompanyCodeSrv);
            cmd.Parameters.AddWithValue("CompanyNameSrv", CompanyNameSrv);
            cmd.Parameters.AddWithValue("BranchCodeSrv", BranchCodeSrv);
            cmd.Parameters.AddWithValue("BranchNameSrv", BranchNameSrv);
            cmd.Parameters.AddWithValue("UserId", CurrentUser.Username);

            object returnValue = cmd.ExecuteScalar();
            return new LargeJsonResult() { Data = returnValue.ToString(), MaxJsonLength = int.MaxValue };
            */
            try
            {
                int to = 0; bool fa = true;
                if (ctx.Database.CommandTimeout.HasValue) to = ctx.Database.CommandTimeout.Value; else fa = false;

                ctx.Database.CommandTimeout = 3600;
                var data = ctx.Database.SqlQuery<int>("exec uspfn_SaveMappingComboSrvGn @Type=@p0, @GroupNoGn=@p1, @GroupNoSrv=@p2, @GroupAreaSrv=@p3, @CompanyCodeGn=@p4, @CompanyCodeSrv=@p5, @CompanyNameSrv=@p6, @BranchCodeGn=@p7, @BranchCodeSrv=@p8, @BranchNameSrv=@p9, @UserId=@p10", Type, GroupNo, GroupNoSrv, GroupAreaSrv, CompanyCode, CompanyCodeSrv, CompanyNameSrv, BranchCode, BranchCodeSrv, BranchNameSrv, CurrentUser.Username).AsQueryable();
                if (fa) ctx.Database.CommandTimeout = to; else ctx.Database.CommandTimeout = null;

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { message = e.Message });
            }
        }

        public JsonResult DeleteMappingComboSrvMarketing()
        {
            string Type = Request["Type"] ?? "";
            string GroupNoSrv = Request["GroupNoSrv"] ?? "";
            string CompanyCodeSrv = Request["CompanyCodeInput"] ?? "";
            string BranchCodeSrv = Request["BranchCodeInput"] ?? "";
            try
            {
                int to = 0; bool fa = true;
                if (ctx.Database.CommandTimeout.HasValue) to = ctx.Database.CommandTimeout.Value; else fa = false;

                ctx.Database.CommandTimeout = 3600;
                var data = ctx.Database.SqlQuery<int>("exec uspfn_DeleteMappingComboSrvGn @Type=@p0, @GroupNoSrv=@p1, @CompanyCodeSrv=@p2, @BranchCodeSrv=@p3", Type, GroupNoSrv, CompanyCodeSrv, BranchCodeSrv).AsQueryable();
                if (fa) ctx.Database.CommandTimeout = to; else ctx.Database.CommandTimeout = null;
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { message = e.Message });
            }
        }
    }
}
