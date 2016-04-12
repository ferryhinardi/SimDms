using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Mvc;
using SimDms.Common;
using SimDms.Sales.Models;

namespace SimDms.Sales.Controllers.Api
{
    public class ComboController : BaseController
    {

        public JsonResult GroupPrice()
        {
            string sql = "select refferencecode Value from omMstRefference where RefferenceType='GRPR'";
            var data = ctx.Database.SqlQuery<String>(sql);
            return  Json(data, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetSalesModel()
        {
            string query = Request["query"] ?? "";
            string sql = "select SalesModelCode Value from omMstModel where SalesModelCode like '" + query + "%' and status=1 order by SalesModelCode";
            var data = ctx.Database.SqlQuery<String>(sql);
            return Json(new { data = data }, JsonRequestBehavior.AllowGet); 
        }

        public JsonResult GetSalesModelName()
        {
            string query = Request["query"] ?? "";
            string sql = "select SalesModelDesc Value from omMstModel where SalesModelCode like '" + query + "%' order by SalesModelCode";
            var data = ctx.Database.SqlQuery<String>(sql);
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadComboData(string CodeID)
        {
            var trans = ctx.LookUpDtls
                .Where(x => (x.CompanyCode == CompanyCode && x.CodeID == CodeID))
                .OrderBy(x => x.SeqNo)
                .Select(x => new { value = x.ParaValue, text = x.LookUpValueName }).ToList();
            return Json(trans, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadComboData2(string CodeID, string lookUpValue)
        {
            var trans = ctx.LookUpDtls
                .Where(x => (x.CompanyCode == CompanyCode && x.CodeID == CodeID && x.LookUpValue == lookUpValue))
                .OrderBy(x => x.SeqNo)
                .Select(x => new { value = x.ParaValue, text = x.LookUpValueName }).ToList();
            return Json(trans, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ComboSignature()
        {
            var trans = ctx.GnMstSignature
                .Where(x => (x.CompanyCode == CompanyCode && x.BranchCode == BranchCode && x.ProfitCenterCode == "100" && x.DocumentType == "KUN"))
                .OrderBy(x => x.SignName)
                .Select(x => new { value = x.SignName, text = x.TitleSign }).ToList();
            return Json(trans, JsonRequestBehavior.AllowGet);
        }

        private class CboItem
        {
            public string value { get; set; }
            public string text { get; set; }
        }

        public JsonResult loadAllCombo()
        {
            var lJKL = SelectReffByType("JKL");
            var lTPS = SelectReffByType("TPS");
            var lSPD = SelectReffByType("SPD");
            var lSUP = SelectReffByType("SUP");
            var lASN = SelectReffByType("ASN");
            var lSRI = SelectReffByType("SRI");
            var lFPG = SelectReffByType("FPG");
            var lPDK = SelectReffByType("PDK");
            var lHSL = SelectReffByType("HSL");
            var lPEK = SelectReffByType("PEK");
            var lUSE = SelectReffByType("USE");
            var lCPB = SelectReffByType("CPB");
            var lLSG = SelectReffByType("LSG");
            var lJWK = SelectReffByType("JWK");

            return Json(new 
            { 
                success = true, 
                jkl = lJKL,
                tps = lTPS,
                spd = lSPD,
                sup = lSUP,
                asn = lASN,
                sri = lSRI,
                fpg = lFPG,
                pdk = lPDK,
                hsl = lHSL,
                pek = lPEK,
                use = lUSE,
                cpb = lCPB,
                lsg = lLSG,
                jwk = lJWK

            });
        }

        private IQueryable SelectReffByType(string reff)
        {
            var qry = String.Format(@"SELECT a.RefferenceCode AS value, a.RefferenceDesc1 AS text
                                FROM dbo.omMstRefference a
                                WHERE a.CompanyCode = '{0}'
                                AND a.RefferenceType = '{1}'
                                AND a.Status != '0'", CompanyCode, reff);

            var list = ctx.Database.SqlQuery<CboItem>(qry).AsQueryable();
            return list;
        }

//        public JsonResult SelectReffByType(string reff)
//        {
//            var qry = String.Format(@"SELECT a.RefferenceCode AS value, a.RefferenceDesc1 AS text
//                                FROM dbo.omMstRefference a
//                                WHERE a.CompanyCode = '{0}'
//                                AND a.RefferenceType = '{1}'
//                                AND a.Status != '0'", CompanyCode, reff);

//            var list = ctx.Database.SqlQuery<CboItem>(qry).AsQueryable();
//            return Json(list);
//        }

        public JsonResult SelectReffByTypeAndCode(string reff, string code)
        {
            var qry = String.Format(@"SELECT a.RefferenceCode, a.RefferenceDesc1
                                FROM dbo.omMstRefference a
                                WHERE a.CompanyCode = '{0}'
                                AND a.RefferenceType = '{1}'
                                AND a.RefferenceCode = '{2}'
                                AND a.Status != '0'", CompanyCode, reff, code);
            var list = ctx.Database.SqlQuery<CboItem>(qry).AsQueryable();
            return Json(list);
        }

        public JsonResult Years()
        {
            List<object> listOfYears = new List<object>();
            //for (int i = 1900; i <= 2100; i++)
            //{
            //    listOfYears.Add(new { value = i, text = i });
            //}
            
            // update by fhi 15-04-2015 : update range year -+ 5 tahun
            int after = DateTime.Now.Year + 5;
            int before = DateTime.Now.Year - 100;
            for (int i = before; i <= after; i++)
            {
                listOfYears.Add(new { value = i, text = i });
            }
            //end

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

        public JsonResult RefferenceType()
        {
            var trans = ctx.RefferenceTypeView
                .OrderBy(x => x.RefferenceType)
                .Select(x => new { value = x.RefferenceType, text = x.RefferenceType }).ToList();
            return Json(trans, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ReffCombo(string RefferenceType)
        {
            var trans = ctx.MstRefferences
                .Where(x => (x.CompanyCode == CompanyCode && x.RefferenceType == RefferenceType))
                .OrderBy(x => x.RefferenceType)
                .Select(x => new { value = x.RefferenceCode, text = x.RefferenceDesc1 }).ToList();
            return Json(trans, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Status()
        {
            List<object> listOfStatus = new List<object>();

            listOfStatus.Add(new { value = 0, text = "OPEN" });
            listOfStatus.Add(new { value = 1, text = "PRINTED" });
            listOfStatus.Add(new { value = 2, text = "APPROVED" });
            listOfStatus.Add(new { value = 3, text = "CANCELED" });
            listOfStatus.Add(new { value = 5, text = "FINISHED" });
            
            return Json(listOfStatus);
        }

        public JsonResult StatusTransfer()
        {
            List<object> listOfStatus = new List<object>();

            listOfStatus.Add(new { value = 1, text = "OUTSTANDING" });
            listOfStatus.Add(new { value = 2, text = "RECEIVED" });

            return Json(listOfStatus);
        }

        public JsonResult UploadType()
        {
            List<object> listOfUploadType = new List<object>();

            listOfUploadType.Add(new { value = "SPORD", text = "Upload Data Order (SPORD)" });
            listOfUploadType.Add(new { value = "SDORD", text = "Upload Data BPU-DO (SDORD)" });
            //listOfUploadType.Add(new { value = "SPRIC", text = "Upload Data Price Manifest (SPRIC)" });
            listOfUploadType.Add(new { value = "SUADE", text = "Upload Unit Allocation Dealer (SUADE)" });
            listOfUploadType.Add(new { value = "SSJAL", text = "Upload Data BPU-SJ (SSJAL)" });
            listOfUploadType.Add(new { value = "SHPOK", text = "Upload Data HPP (SHPOK)" });
            listOfUploadType.Add(new { value = "SACCS", text = "Upload Data Aksesoris (SACCS)" });
            listOfUploadType.Add(new { value = "SFPO1", text = "Upload Data Faktur Polisi (SFPO1)" });
            listOfUploadType.Add(new { value = "SFPO2", text = "Upload Data Faktur Polisi (SFPO2)" });
            listOfUploadType.Add(new { value = "SFPLA", text = "Faktur Polisi Sub-Dealer (SFPLA)" });
            listOfUploadType.Add(new { value = "SFPLR", text = "Faktur Polisi Sub-Dealer (SFPLR)" });

            return Json(listOfUploadType);
        }
    }
}
