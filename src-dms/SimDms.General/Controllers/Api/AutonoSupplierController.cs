using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.General.Models;
using SimDms.General.Models.Others;
using System.Web.Script.Serialization;
using TracerX;
using System.Transactions;


namespace SimDms.General.Controllers.Api
{
    public class AutonoSupplierController : BaseController  
    {
        public JsonResult LoadInfo() 
        {
            var g = ctx.GnMstSupplierUtilities.FirstOrDefault();
            var info = new GnMstSupplierUtility
            {
                CompanyCode = g.CompanyCode,
                BranchCode = g.BranchCode,
                Sequence = g.Sequence,
                IsAutoGenerate = g.IsAutoGenerate


            };
            return Json(info);
        }

        [HttpPost]
        public JsonResult Save(GnMstSupplierUtility model)
        {
            ResultModel result = InitializeResultModel();
            string userID = CurrentUser.UserId;
            DateTime currentTime = DateTime.Now;

            var me = ctx.GnMstSupplierUtilities.Find(CompanyCode, model.BranchCode);

            if (me == null)
            {
                me = new GnMstSupplierUtility();
                me.CreatedDate = currentTime;
                me.LastUpdateDate = currentTime;
                me.CreatedBy = userID;
                ctx.GnMstSupplierUtilities.Add(me);
            }
            else
            {
                me.LastUpdateDate = currentTime;
                me.LastUpdateBy = userID;
            }
            me.CompanyCode = CompanyCode;
            me.BranchCode = model.BranchCode;
            me.IsAutoGenerate = model.IsAutoGenerate;
            me.Sequence = model.Sequence; 

            try
            {
                ctx.SaveChanges();
                result.status = true;
                result.message = "Data Auto Number Supplier berhasil disimpan.";
                result.data = new
                {
                    IsAutoGenerate = me.IsAutoGenerate,
                    Sequence = me.Sequence
                };
            }
            catch (Exception Ex)
            {
                result.message = "Data Auto Number Supplier tidak bisa disimpan.";
                MyLogger.Info("Error on Auto Number Supplier saving: " + Ex.Message);
            }

            return Json(result);
        }
        
    }
}
