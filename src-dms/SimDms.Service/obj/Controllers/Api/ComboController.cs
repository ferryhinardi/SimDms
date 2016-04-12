using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Mvc;
using SimDms.Common;


namespace SimDms.Service.Controllers.Api
{
    public class ComboController : BaseController
    {
        //
        // GET: /Combo/

        public JsonResult ServiceRefference()
        {
            var refType = ( string.IsNullOrWhiteSpace(HttpContext.Request["RefType"])?"GRPJOBTY":HttpContext.Request["RefType"]);
            var user = CurrentUser;
            var coProfile = ctx.CoProfiles.Where(m=>m.CompanyCode == user.CompanyCode && m.BranchCode == user.BranchCode).FirstOrDefault();
            var list = (from mstReff in ctx.svMstRefferenceServices
                        where mstReff.CompanyCode == user.CompanyCode && mstReff.ProductType == coProfile.ProductType && mstReff.RefferenceType == refType
                        select new { value=mstReff.RefferenceCode, text=mstReff.Description }).ToList();
            return Json(list);
        }

        public JsonResult PartTypeLookup()
        {
            var user = CurrentUser;
            var listLkp = ctx.LookUpDtls.Where(m => m.CompanyCode == user.CompanyCode && m.CodeID == "TPGO").Select(m=>new {value=m.ParaValue, text=m.LookUpValueName}).ToList();
            return Json(listLkp);
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

        public JsonResult TransmissionTypes()
        {
            var trans = ctx.LookUpDtls.Where(x => x.CompanyCode == CompanyCode && x.CodeID == "TRTY")
                .OrderBy(x => x.SeqNo).Select(x => new { value = x.LookUpValue, text = x.LookUpValue }).ToList();
            return Json(trans);
        }

        public JsonResult LoadLaborCode()
        {
            var trans = ctx.SvLaborCodes.Where(x => x.CompanyCode == CompanyCode)
                .OrderBy(x => x.BasicModel).Select(x => new { value = x.BasicModel, text = x.BasicModel }).ToList();
            return Json(trans);
        }

        public JsonResult BillType()
        {
            var item = ctx.svMstBillTypes.Where(x => x.CompanyCode == CompanyCode).Select(x => new { value = x.BillType, text = x.BillType + " - " + x.Description}).ToList();
            return Json(item, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListLookupDtlByCodeID()
        {
            var codeID = HttpContext.Request["CodeID"];
            var companyCode = CurrentUser.CompanyCode;
            var Lookups = ctx.LookUpDtls.Where(e => e.CompanyCode == companyCode && e.CodeID == codeID).Select(e=> new{value=e.LookUpValue, text=e.LookUpValueName}).OrderBy(p => p.text).ToList();
            return Json(Lookups);
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

        public JsonResult ServiceAdvisor()
        {
            List<object> listOfServiceAdv = new List<object>();
            string[] filter = { "3", "7" };

            var data = ctx.Employees.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && filter.Contains(a.TitleCode));

            foreach(var sa in data)
            {
                listOfServiceAdv.Add(new { value = sa.EmployeeID, text = sa.EmployeeName});
            }

            return Json(listOfServiceAdv);
        }

        public JsonResult SelectActiveCustomer()
        {
            var records = ctx.Customers.Where(p => p.CompanyCode == CompanyCode && p.Status == "1").ToList()
                .Select(p => new {
                    p.CustomerCode,
                    p.CustomerName,
                    p.Address1,
                    p.Address2,
                    p.Address3,
                    p.Address4,
                    Address = p.Address1 + ' ' + p.Address2 + ' ' + p.Address3 + ' ' + p.Address4,
                    Status = (p.Status == "1") ? "Aktif" : "Tidak Aktif"
                });

            return Json(records.AsQueryable().toKG());
        }

        public JsonResult BasicModelWarranty()
        {
            var companyCode = CurrentUser.CompanyCode;
            var record = ctx.SvMstWarranties.Where(e => e.CompanyCode == CompanyCode).Select(e => new { value = e.BasicModel, text = e.BasicModel }).OrderBy(p => p.text).ToList().Distinct();
            return Json(record);
        }

        public JsonResult OperationNoWarranty()
        {
            var companyCode = CurrentUser.CompanyCode;
            var record = ctx.SvMstWarranties.Where(e => e.CompanyCode == CompanyCode).Select(e => new { value = e.OperationNo, text = e.OperationNo }).OrderBy(p => p.text).ToList().Distinct();
            return Json(record);
        }

        public JsonResult BasicModel()
        {
            var trans = ctx.SvBasicKsgViews
                .Where(x => x.CompanyCode == CompanyCode && x.ProductType == ProductType)
                .OrderBy(x => x.BasicModel)
                .Select(x => new { value = x.BasicModel, text = x.BasicModel }).ToList();
            return Json(trans, JsonRequestBehavior.AllowGet);
        }
    }
}
