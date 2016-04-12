using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OfficeOpenXml;
using GeLang;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using SimDms.DataWarehouse.Models;
using System.Data.SqlClient;

namespace SimDms.DataWarehouse.Controllers.Api
{
    public class CsReviewsController : BaseController
    {
        [HttpPost]
        public JsonResult Save(CsReviews model)
        {
            var ent = ctx.CsReviews.Find(model.CompanyCode, model.BranchCode, model.EmployeeID, model.DateFrom, model.DateTo, model.Plan);

            if (ent != null)
            {
                ent.CommentbySIS = model.CommentbySIS;
            }

            try { ctx.SaveChanges(); }
            catch (Exception)
            {
                return Json(new { status = false });
            }
            return Json(new { status = true });
        }
    }
}