using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GeLang;
using SimDms.Sparepart.Models;
using SimDms.Common;
using SimDms.Common.Models;

namespace SimDms.Sparepart.Controllers.Api
{
    public class ReportPenerimaanController : BaseController
    {


        public JsonResult Signature(string DocumentType)
        {
            var record = ctx.GnMstSignatures.Find(CompanyCode, BranchCode, ProfitCenter, DocumentType, 1);
            try
            {
                if (record != null)
                {
                    return Json(new { success = true, signName = record.SignName, titleSign = record.TitleSign });
                }
                else {
                    return Json(new { success = true, signName = "", titleSign = "" });
                }
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
        }

        public JsonResult SetID()
        {
            var record = ctx.LookUpDtls.Find(CompanyCode, "SPNOTE", "WRS").ParaValue;
            try
            {
                if (record != null)
                {
                    return Json(new { success = true, status = record });
                }
                else
                {
                    return Json(new { success = true, status = record });
                }
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
        }



        public JsonResult UpdatePrintSeq(string DocNo, string DocNo1, string table, string column)
        {
            var query = string.Format(@"
            UPDATE {5} 
            SET PrintSeq = PrintSeq + 1
            , LastUpdateBy = '{4}'
            , LastUpdateDate = GetDate()
            WHERE CompanyCode = '{0}'
            AND BranchCode = '{1}'
            AND {6} BETWEEN '{2}' AND '{3}'", CompanyCode, BranchCode, DocNo, DocNo1, CurrentUser.UserId, table, column);
            try
            {
                ctx.Database.ExecuteSqlCommand(query);
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false });
            }
        }

        public JsonResult Convert(convert_data model)
        {
            var date = model.DocPeriod.ToString("yyyy-MM-dd");
            

            return Json(new { success = true, date = date, realdate = model.DocPeriod});
        }

        public class convert_data {
            public DateTime DocPeriod { get; set; }
            public string PartType { get; set; }
        }

    }
}
