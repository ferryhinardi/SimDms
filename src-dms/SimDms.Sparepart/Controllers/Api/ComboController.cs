using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Mvc;
using SimDms.Common;

namespace SimDms.Sparepart.Controllers.Api
{
    public class ComboController : BaseController
    {


        public JsonResult LoadLookup(string CodeID)
        {
            var user = CurrentUser;
            var listLkp = ctx.LookUpDtls.Where(m => m.CompanyCode == user.CompanyCode && m.CodeID == CodeID).OrderBy(x => x.ParaValue).Select(m => new { value = m.ParaValue, text = m.LookUpValueName }).ToList();
            return Json(listLkp,JsonRequestBehavior.AllowGet);
        }

        public JsonResult ComboADO(string TypeID = "1", string pCode = "")
        {
            var comboData = ctx.Database.SqlQuery<MyComboData>("[uspfn_spInquiry_ListADO] '" + pCode + "'," + TypeID ).ToList();
            return Json(comboData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PartTypeLookup()
        {
            var user = CurrentUser;
            var listLkp = ctx.LookUpDtls.Where(m => m.CompanyCode == user.CompanyCode && m.CodeID == "TPGO").Select(m=>new {value=m.ParaValue, text=m.LookUpValueName}).ToList();
            return Json(listLkp);
        }
        public JsonResult PartCategoryLookup()
        {
            var user = CurrentUser;
            var listLkp = ctx.LookUpDtls.Where(m => m.CompanyCode == user.CompanyCode && m.CodeID == "PRCT").Select(m => new { value = m.ParaValue, text = m.LookUpValueName }).ToList();
            return Json(listLkp);
        }
        
        public JsonResult GetLookupByCodeID(string CodeID)
        {
               var user = CurrentUser;
            var listLkp = ctx.LookUpDtls.Where(m => m.CompanyCode == user.CompanyCode && m.CodeID == CodeID).Select(m => new { value = m.LookUpValue, text = m.LookUpValueName }).ToList();
            return Json(listLkp);
        }

        public JsonResult Years()
        {
            List<object> listOfYears = new List<object>();
            int Now = DateTime.Now.Year + 5;
            int Past = DateTime.Now.Year - 5;
            for (int i = Past; i <= Now; i++)
            {
                listOfYears.Add(new { value = i, text = i });
            }
            return Json(listOfYears);
        }

        public JsonResult TypePart()
        {
            //var record = ctx.GnMstLookUpDtls.Where(a => a.CompanyCode == CompanyCode && a.CodeID == "TPGO").ToList();

            return Json(ctx.LookUpDtls.Where(a => a.CompanyCode == CompanyCode && a.CodeID == "TPGO").OrderBy(x => x.LookUpValue)
                .Select(x=> new {text=x.LookUpValueName,value=x.LookUpValue}));

            //foreach (var row in record) {
            ///    record.Add(new { value = row.LookUpValue, text = row.LookUpValueName});
            //}
        }

        public JsonResult YearsOld()
        {
            List<object> listOfYears = new List<object>();
            for (int i = 1900; i <= 2100; i++)
            {
                listOfYears.Add(new { value = i, text = i });
            }
            return Json(listOfYears);
        }

        public JsonResult Months()
        {
            List<object> listOfMonths = new List<object>();
            int Now = DateTime.Now.Year;
            int Past = DateTime.Now.Year - 10;

            listOfMonths.Add(new { value = 1, text = "January" });
            listOfMonths.Add(new { value = 2, text = "February" });
            listOfMonths.Add(new { value = 3, text = "March" });
            listOfMonths.Add(new { value = 4, text = "April" });
            listOfMonths.Add(new { value = 5, text = "May" });
            listOfMonths.Add(new { value = 6, text = "June" });
            listOfMonths.Add(new { value = 7, text = "July" });
            listOfMonths.Add(new { value = 8, text = "August" });
            listOfMonths.Add(new { value = 9, text = "September" });
            listOfMonths.Add(new { value = 10, text = "October" });
            listOfMonths.Add(new { value = 11, text = "November" });
            listOfMonths.Add(new { value = 12, text = "December" });

                return Json(listOfMonths);
        }

        public JsonResult DataIDCategory()
        {
            List<object> listOfID = new List<object>();
            listOfID.Add(new {value="PINVS", text="PINVS" });
            listOfID.Add(new {value="PPRCD", text="PPRCD" });
            listOfID.Add(new {value="PMODP", text="PMODP" });
            listOfID.Add(new {value="PMDLM", text="PMDLM" });
            listOfID.Add(new {value="MSMDL", text="MSMDL" });
            return Json(listOfID);
        }

        public JsonResult Select4FakturNo()
        {
            var user = CurrentUser;
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "select 'IN'+BillType value, ('IN'+BillType + ' - ' + Description) as text from svMstBillingTYpe where CompanyCode = @CompanyCode";
            cmd.Parameters.AddWithValue("@CompanyCode", user.CompanyCode);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult LoadComboData(string CodeID)
        {
            var trans = ctx.LookUpDtls
                .Where(x => (x.CompanyCode == CompanyCode && x.CodeID == CodeID))
                .OrderBy(x => x.SeqNo)
                .Select(x => new { value = x.LookUpValue, text = x.LookUpValueName }).ToList();
            return Json(trans,JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadBranchCode()
        {
            var trans = ctx.CoProfiles.OrderBy(x => x.CompanyName).
                Select(x => new { value = x.BranchCode, text = x.CompanyName }).ToList();
            return Json(trans, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ComboBranchCode()
        {
            var trans = ctx.GnMstDealerOutletMappings.
                Select(x => new { value = x.OutletCode, text = x.OutletCode + " - " + x.OutletAbbreviation }).ToList();
            return Json(trans, JsonRequestBehavior.AllowGet);
        }

        public JsonResult MovingCode()
        {
            var trans = ctx.spMstMovingCodes
                //.Where(x => (x.CompanyCode == CompanyCode && x.CodeID == CodeID))
                .OrderBy(x => x.MovingCode)
                .Select(x => new { value = x.MovingCode, text = x.MovingCodeName }).ToList();
            return Json(trans, JsonRequestBehavior.AllowGet);
        }

        public string LoadScalarData(string CodeID, string varName)
        {
            string s = "";
            var trans = ctx.LookUpDtls
                .Where(x => (x.CompanyCode == CompanyCode && x.CodeID == CodeID && x.LookUpValue == varName)).FirstOrDefault();
            if (trans != null)
            {
                s = trans.LookUpValueName;
            }
            return s;
        }

        public JsonResult SysMessages(string ErrNo)
        {
            var trans = ctx.SysMsgs
                .Where(x => (x.MessageCode == ErrNo))
                .Select(x => new { ErrNo = x.MessageCode, ErrDesc = x.MessageCaption }).FirstOrDefault();
            return Json(trans);
        }

        public JsonResult TransactionType(string CodeID)
        {
            var user = CurrentUser;
            var listLkp = ctx.LookUpDtls.Where(m => m.CompanyCode == user.CompanyCode && m.CodeID == CodeID).Select(m => new { value = m.LookUpValue, text = m.LookUpValueName }).ToList();
            return Json(listLkp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult OrderNo()
        {
            var user = CurrentUser;
            List<string> assignedMenus = ctx.SpTrnSORDHdrs.Where(m => m.CompanyCode == user.CompanyCode && m.BranchCode == BranchCode).Select(x => x.OrderNo).ToList();
            var listLkp = ctx.spTrnPREQHdr.Where(x => assignedMenus.Contains(x.REQNo) == false && x.SupplierCode == BranchCode).Select(m => new { value = m.REQNo, text = m.REQNo }).ToList();
            //var listLkp = ctx.spTrnPREQHdr.Where(m => m.CompanyCode == user.CompanyCode && m.BranchCode == BranchCode).Select(m => new { value = m.REQNo, text = m.REQNo }).ToList();
            return Json(listLkp, JsonRequestBehavior.AllowGet); //SpTrnSORDHdrs
        }

        public JsonResult UploadType()
        {
            List<object> listOfUploadType = new List<object>();

            listOfUploadType.Add(new { value = "PINVS", text = "Upload Data Invoice (PINVS)" });
            listOfUploadType.Add(new { value = "PORDS", text = "Upload Data Order (PORDS)" });
            listOfUploadType.Add(new { value = "TSTKD", text = "Upload Data Transfer Stock (TSTKD)" });
            listOfUploadType.Add(new { value = "PMODP", text = "Upload Data Subtitusi (PMODP)" });
            listOfUploadType.Add(new { value = "PPRCD", text = "Upload Data Data Price (PPRCD)" });
            listOfUploadType.Add(new { value = "PMDLM", text = "Upload Part Model (PMDLM)" });
            listOfUploadType.Add(new { value = "MSMDL", text = "Upload Master Model (MSMDL)" });

            return Json(listOfUploadType);
        }

        /// <summary>
        /// Filter LookUpValue "8"
        /// </summary>
        /// <returns></returns>
        public JsonResult ComboOrderType()
        {
            var trans = ctx.LookUpDtls
                .Where(x => (x.CompanyCode == CompanyCode && x.CodeID == GnMstLookUpHdr.OrderType && x.LookUpValue != "8"))
                .OrderBy(x => x.SeqNo)
                .Select(x => new { value = x.LookUpValue, text = x.LookUpValueName }).ToList();
            return Json(trans, JsonRequestBehavior.AllowGet);
        }

    }
}
