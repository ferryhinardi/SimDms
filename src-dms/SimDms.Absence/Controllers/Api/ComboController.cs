using SimDms.Absence.Models.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Absence.Controllers.Api
{
    public class ComboController : BaseController
    {
        public JsonResult Departments()
        {
            var list = ctx.OrgGroups.Where(p => p.CompanyCode == CompanyCode && p.OrgGroupCode == "DEPT").OrderBy(p => p.OrgSeq).Select(p => new { value = p.OrgCode, text = p.OrgName.ToUpper() }).OrderBy(x => x.text).ToList();
            return Json(list);
        }

        public JsonResult Positions(string id = "")
        {
            var qry = ctx.Positions.Where(p => p.CompanyCode.Equals(CompanyCode) && (p.DeptCode.Equals(id) || p.DeptCode.Equals("COM"))).ToList();
            var list = qry.OrderBy(p => p.PosLevel).Select(p => new { value = p.PosCode, text = (p.PosName.ToUpper() + string.Format("  ({0} - {1})", p.PosCode, p.PosLevel)) }).ToList();
            return Json(list);
        }

        public JsonResult Lookups(string id = "")
        {
            var list = ctx.LookUpDtls.Where(p => p.CompanyCode == CompanyCode && p.CodeID == id).OrderBy(p => p.SeqNo).Select(p => new { value = p.LookUpValue, text = p.LookUpValueName.ToUpper() }).OrderBy(x => x.text).ToList();
            return Json(list);
        }

        public JsonResult HrLookups()
        {
            var list = ctx.HrLookupMappings.Where(p => p.CompanyCode == CompanyCode).Select(p => new { value = p.CodeID, text = p.CodeDescription.ToUpper() }).OrderBy(x => x.text).ToList();
            return Json(list);
        }

        public JsonResult Shifts()
        {
            var list = ctx.Shifts.Where(p => p.CompanyCode == CompanyCode).Select(p => new { value = p.ShiftCode, text = p.ShiftName.ToUpper() }).ToList();
            list.Insert(0, new { value = "-", text = "NON SHIFT" });
            return Json(list);
        }

        public JsonResult Branch()
        {
            var list = ctx.CoProfiles.OrderBy(m => m.BranchCode).Select(m => new { value = m.BranchCode, text = m.BranchCode + " - " + m.CompanyName });
            return Json(list);
        }

        public JsonResult Years()
        {
            List<object> listOfYears = new List<object>();
            int Now = DateTime.Now.Year;
            int Past = DateTime.Now.Year - 10;
            for (int i = Past; i <= Now; i++)
            {
                listOfYears.Add(new { value = i, text = i });
            }
            return Json(listOfYears);
        }

        public JsonResult HolidayYears()
        {
            var list = ctx.Holidays.Select(p => new { value = p.HolidayYear, text = p.HolidayYear }).Distinct().ToList();
            return Json(list.OrderByDescending(p => p.text));
        }

        public JsonResult Grades()
        {
            var data = (from x in ctx.GnMstLookupDtls
                        where
                        x.CodeID.Equals("ITSG") == true
                        orderby x.LookupValueName ascending
                        select new
                        {
                            text = x.LookupValueName.ToUpper(),
                            value = x.LookUpValue
                        }).ToList();

            return Json(data);
        }

        public JsonResult TrainingPreScoresAlternative()
        {
            var data = (from x in ctx.GnMstLookupDtls
                        where
                        x.CodeID.Equals("TRNPREALT") == true
                        orderby x.LookupValueName ascending
                        select new
                        {
                            text = x.LookupValueName.ToUpper(),
                            value = x.LookUpValue
                        }).ToList();

            return Json(data);
        }

        public JsonResult TrainingPostScoresAlternative()
        {
            var data = (from x in ctx.GnMstLookupDtls
                        where
                        x.CodeID.Equals("TRNPOSTALT") == true
                        orderby x.LookupValueName ascending
                        select new
                        {
                            text = x.LookupValueName.ToUpper(),
                            value = x.LookUpValue
                        }).ToList();

            return Json(data);
        }

        [HttpPost]
        public JsonResult Ranks()
        {
            var data = (from x in ctx.GnMstOrgGroups
                        where
                        x.OrgGroupCode.Equals("RANK") == true
                        orderby x.OrgName ascending
                        select new
                        {
                            text = x.OrgName.ToUpper(),
                            value = x.OrgCode
                        }).ToList();

            return Json(data);
        }

        [HttpPost]
        public JsonResult PersonnelStatus()
        {
            var data = (from x in ctx.GnMstLookupDtls
                        where
                        x.CodeID.Equals("PERS") == true
                        orderby x.LookupValueName ascending
                        select new
                        {
                            text = x.LookupValueName.ToUpper(),
                            value = x.LookUpValue
                        }).ToList();

            return Json(data);
        }

        [HttpPost]
        public JsonResult MaritalStatus()
        {
            var data = (from x in ctx.GnMstLookupDtls
                        where
                        x.CodeID.Equals("MRTL") == true
                        orderby x.LookupValueName ascending
                        select new
                        {
                            text = x.LookupValueName.ToUpper(),
                            value = x.LookUpValue
                        }).ToList();

            return Json(data);
        }

        [HttpPost]
        public JsonResult MaritalStatusDetails(string id = "")
        {
            var data = ctx.GnMstLookupDtls.OrderBy(x => x.LookupValueName);

            if (id.Equals("K"))
            {
                data = data.Where(x => x.CodeID == "MRTLK").OrderBy(x => x.LookupValueName);
            }
            else
            {
                data = data.Where(x => x.CodeID == "MRTLK0").OrderBy(x => x.LookupValueName);
            }

            return Json(data.Select(x => new
            {
                text = x.LookupValueName,
                value = x.LookUpValue
            }));
        }

        [HttpPost]
        public JsonResult UniformSizes()
        {
            var data = (from x in ctx.GnMstLookupDtls
                        where
                        x.CodeID.Equals("SIZE") == true
                        orderby x.LookupValueName ascending
                        select new
                        {
                            text = x.LookupValueName.ToUpper(),
                            value = x.LookUpValue
                        }).ToList();

            return Json(data);
        }

        [HttpPost]
        public JsonResult UniformSizeAlts()
        {
            var data = (from x in ctx.GnMstLookupDtls
                        where
                        x.CodeID.Equals("SIZEALT") == true
                        orderby x.LookupValueName ascending
                        select new
                        {
                            text = x.LookupValueName.ToUpper(),
                            value = x.LookUpValue
                        }).ToList();

            return Json(data);
        }

        [HttpPost]
        public JsonResult ShoesSize()
        {
            var data = (from x in ctx.GnMstLookupDtls
                        where
                        x.CodeID.Equals("SHOESSIZE") == true
                        orderby x.LookupValueName ascending
                        select new
                        {
                            text = x.LookupValueName.ToUpper(),
                            value = x.LookUpValue
                        }).ToList();

            return Json(data);
        }

        [HttpPost]
        public JsonResult BloodTypes()
        {
            var data = (from x in ctx.GnMstLookupDtls
                        where
                        x.CodeID.Equals("BLOOD") == true
                        orderby x.LookupValueName ascending
                        select new
                        {
                            text = x.LookupValueName.ToUpper(),
                            value = x.LookUpValue
                        }).ToList();

            return Json(data);
        }

        [HttpPost]
        public JsonResult FormalEducations()
        {
            var data = (from x in ctx.GnMstLookupDtls
                        where
                        x.CodeID.Equals("FEDU") == true
                        orderby x.LookupValueName ascending
                        select new
                        {
                            text = x.LookupValueName.ToUpper(),
                            value = x.LookUpValue
                        }).ToList();

            return Json(data);
        }

        [HttpPost]
        public JsonResult Genders()
        {
            var data = (from x in ctx.GnMstLookupDtls
                        where
                        x.CodeID.Equals("GNDR") == true
                        orderby x.LookupValueName ascending
                        select new
                        {
                            text = x.LookupValueName.ToUpper(),
                            value = x.LookUpValue
                        }).ToList();

            return Json(data);
        }

        [HttpPost]
        public JsonResult LookupCodes()
        {
            var data = (from x in ctx.HrLookupViews
                        select new
                        {
                            text = x.CodeDescription.ToUpper(),
                            value = x.CodeID.ToUpper()
                        }).OrderBy(x => x.text).Distinct().ToList().OrderBy(x => x.text);

            return Json(data);
        }

        [HttpPost]
        public JsonResult Religions()
        {
            var data = (from x in ctx.GnMstLookupDtls
                        where
                        x.CodeID.Equals("RLGN") == true
                        orderby x.LookupValueName ascending
                        select new
                        {
                            text = x.LookupValueName.ToUpper(),
                            value = x.LookUpValue
                        }).ToList();

            return Json(data);
        }

        [HttpPost]
        public JsonResult ResignCategories()
        {
            var data = (from x in ctx.GnMstLookupDtls
                        where
                        x.CodeID.Equals("RESIGNCTG") == true
                        orderby x.LookupValueName ascending
                        select new
                        {
                            text = x.LookupValueName.ToUpper(),
                            value = x.LookUpValue
                        }).ToList();

            return Json(data);
        }

        [HttpPost]
        public JsonResult ReasonCategories()
        {
            var data = (from x in ctx.GnMstLookupDtls
                        where
                        x.CodeID.Equals("REASONCTG") == true
                        orderby x.LookupValueName ascending
                        select new
                        {
                            text = x.LookupValueName.ToUpper(),
                            value = x.LookUpValue
                        }).ToList();

            return Json(data);
        }

        [HttpPost]
        public JsonResult Provinces()
        {
            var data = (from x in ctx.GnMstZipCodes
                        orderby x.Ibukota ascending
                        select new
                        {
                            text = x.Ibukota.ToUpper(),
                            value = x.Ibukota
                        }).Distinct().OrderBy(x => x.text).ToList();

            return Json(data);
        }

        [HttpPost]
        public JsonResult Cities(string id = "")
        {
            var data = (from x in ctx.GnMstZipCodes
                        where
                        x.Ibukota.Equals(id) == true
                        orderby x.KotaKabupaten ascending
                        select new
                        {
                            text = x.KotaKabupaten.ToUpper(),
                            value = x.KotaKabupaten
                        }).Distinct().ToList();

            return Json(data);
        }

        [HttpPost]
        public JsonResult Districts(string id = "")
        {

            var data = (from x in ctx.GnMstZipCodes
                        where
                        x.KotaKabupaten.Equals(id) == true
                        orderby x.KecamatanDistrik ascending
                        select new
                        {
                            text = x.KecamatanDistrik.ToUpper(),
                            value = x.KecamatanDistrik
                        }).Distinct().ToList();

            return Json(data);
        }

        [HttpPost]
        public JsonResult Villages(string id = "")
        {
            var data = (from x in ctx.GnMstZipCodes
                        where
                        x.KecamatanDistrik.Equals(id) == true
                        orderby x.KelurahanDesa ascending
                        select new
                        {
                            text = x.KelurahanDesa.ToUpper(),
                            value = x.KelurahanDesa
                        }).Distinct().ToList();

            return Json(data);
        }

        [HttpPost]
        public string ZipCode()
        {
            string provinceCode = Request["provinceCode"] ?? "";
            string cityCode = Request["cityCode"] ?? "";
            string districtCode = Request["districtCode"] ?? "";
            string villageCode = Request["villageCode"] ?? "";

            string zipCode = (from x in ctx.GnMstZipCodes
                              where
                              x.Ibukota.Equals(provinceCode) == true
                              &&
                              x.KotaKabupaten.Equals(cityCode) == true
                              &&
                              x.KecamatanDistrik.Equals(districtCode) == true
                              &&
                              x.KelurahanDesa.Equals(villageCode) == true
                              orderby x.ZipCode ascending
                              select x.ZipCode).FirstOrDefault();

            return zipCode;
        }

        public JsonResult TeamLeaders()
        {
            string Department = Request["Department"];
            string Position = Request["Position"];

            var queryable = ctx.Database.SqlQuery<ComboModel>("exec uspfn_HrGetTeamLeader @CompanyCode=@p0, @DeptCode=@p1, @PosCode=@p2", CompanyCode, Department, Position).ToList();

            return Json(queryable);
        }

        public JsonResult Employees()
        {
            var emps = ctx.Database.SqlQuery<ComboModel>("select distinct employeeid value, employeename + ' (' + employeeid + ')' text from hremployee where companycode=@p0 and isnull(isdeleted, 0) = 0 order by text", CompanyCode).ToList();

            return Json(emps);
        }
    }
}
