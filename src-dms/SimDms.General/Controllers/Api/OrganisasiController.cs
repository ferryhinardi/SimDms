using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Common.Models;
using SimDms.General.Models;

namespace SimDms.General.Controllers.Api
{
    public class OrganisasiController : BaseController
    {
        //
        // GET: /Organisasi/
        public ActionResult Index()
        {
            return View();
        }
        public decimal maxkey()
        {
            var max = ctx.Database.SqlQuery<decimal>("select max(a.Seqno) from gnMstOrganizationDtl a").FirstOrDefault();
            return max;
        }

        public ActionResult SaveOrganization(OrganizationHdr model)
        {
            object returnObj;
            try
            {
                var OrgHdr = ctx.OrganizationHdrs.Find(model.CompanyCode);
                if (OrgHdr == null)
                {
                    OrgHdr = new OrganizationHdr();
                    OrgHdr.CompanyCode = model.CompanyCode;
                    OrgHdr.CompanyAccNo = model.CompanyAccNo;
                    OrgHdr.CompanyName = model.CompanyName;
                    OrgHdr.CreatedBy = CurrentUser.UserId;
                    OrgHdr.CreatedDate = DateTime.Now;
                    ctx.OrganizationHdrs.Add(OrgHdr);
                    returnObj = new { success=true, message="data Kantor pusat berhasil disimpan"};
                }
                else
                {
                    OrgHdr.CompanyAccNo = model.CompanyAccNo;
                    OrgHdr.CompanyName = model.CompanyName;
                    OrgHdr.LastUpdateBy = CurrentUser.UserId;
                    OrgHdr.LastUpdateDate = DateTime.Now;
                    returnObj = new { success = true, message = "data Kantor pusat berhasil diupdate" };
                }
                ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                returnObj = new {success=true, message="Error pada saat menyimpan data kantor pusat, message="+ex.Message.ToString()};
            }
            return Json(returnObj);
        }

        public ActionResult SaveOrganizationDtl(GnMstOrganizationDtl model)
        {
            object returnObj;
            try
            {
                var OrgDtl = ctx.GnMstOrganizationDtls.Find(model.CompanyCode, model.BranchCode);
                if (OrgDtl == null)
                {
                    OrgDtl = new GnMstOrganizationDtl();
                    OrgDtl.CompanyCode = model.CompanyCode;
                    OrgDtl.BranchCode = model.BranchCode;
                    OrgDtl.BranchName = model.BranchName;
                    OrgDtl.BranchAccNo = model.BranchAccNo;
                    var max = maxkey();
                    OrgDtl.SeqNo = max + 1;
                    //if (max != 0 || max != null) { OrgDtl.IsBranch = true; }; //else { OrgDtl.IsBranch = false; }
                    OrgDtl.IsBranch = true ;
                    OrgDtl.CreatedBy = CurrentUser.UserId;
                    OrgDtl.CreatedDate = DateTime.Now;
                    ctx.GnMstOrganizationDtls.Add(OrgDtl);
                    returnObj = new { success = true, message = "Informasi Kantor Cabang berhasil di simpan" };
                }
                else
                {
                    OrgDtl.BranchAccNo = model.BranchAccNo;
                    OrgDtl.BranchName = model.BranchName;
                    OrgDtl.LastUpdateBy = CurrentUser.UserId;
                    OrgDtl.CreatedDate = DateTime.Now;
                    returnObj = new { success = true, message = "Informasi Kantor Cabang berhasil di update" };
                }
                ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                returnObj = new {success=false, message="Error ketika menyimpan data kantor cabang, Message="+ex.Message.ToString()};
            }
            return Json(returnObj);
        }

        public ActionResult GetOrganizationByCompanyCode(string CompanyCode)
        {
            object returnObj = null;
            var OrgHdr = ctx.OrganizationHdrs.Find(CompanyCode);
            if (OrgHdr != null)
                returnObj = new { success = true, data = OrgHdr };
            else
                returnObj = new {success=false, data= OrgHdr};
            return Json(returnObj);
        }

        public ActionResult DeleteOrganization(string CompanyCode)
        {
            object returnObj = null;
            try
            {
                var orgHdr = ctx.OrganizationHdrs.Find(CompanyCode);
                if (orgHdr != null)
                {
                    ctx.OrganizationHdrs.Remove(orgHdr);
                    var OrgDtls = ctx.GnMstOrganizationDtls.Where(m => m.CompanyCode == CompanyCode);
                    foreach (var OrgDtl in OrgDtls)
                    {
                        ctx.GnMstOrganizationDtls.Remove(OrgDtl);
                    }
                    ctx.SaveChanges();
                    returnObj = new { success=true, message="Data Kantor pusat berhasil di delete."};
                }
                else
                {
                    returnObj = new {success=false, message="Error ketika mendelete Kantor Pusat, Message=Data tidak ditemukan"};
                }
            }
            catch (Exception ex)
            {
                returnObj = new { success = false, message = "Error ketika mendelete Kantor Pusat, Message="+ex.ToString()};
            }
            return Json(returnObj);
        }

        public ActionResult DeleteOrganizationDtl(GnMstOrganizationDtl model)
        {
            object returnObj = null;
            try
            {
                var OrgDtl = ctx.GnMstOrganizationDtls.Find(model.CompanyCode, model.BranchCode);
                if(OrgDtl!=null)
                {
                    ctx.GnMstOrganizationDtls.Remove(OrgDtl);
                    ctx.SaveChanges();
                    returnObj = new { success = true, message = "Delete Kantor Cabang Berhasil" };
                }
                else
                {
                    returnObj = new {success=false, message="Error ketika mendelete data Kantor Cabang, Message= data tidak ditemukan"};
                }
            }
            catch (Exception ex)
            {
                returnObj = new { success = false, message = "Error ketika mendelete data Kantor Cabang, Message= "+ ex.Message.ToString() };
            }
            return Json(returnObj);
        }


        [HttpPost]
        public ActionResult GetOrganizationDtl(string CompanyCode)
        {
            var OrgDtl = ctx.GnMstOrganizationDtls.Where(m => m.CompanyCode == CompanyCode).ToList();
            return Json(new { success = true, data = OrgDtl });
        }

        public JsonResult CheckCompanyAccNo(string SegAccNo, string TipeSegAcc) 
        {
            var titleName = ctx.GnMstSegmentAccs.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == BranchCode && a.TipeSegAcc == TipeSegAcc && a.SegAccNo == SegAccNo).FirstOrDefault();
            if (titleName != null)
            {
                return Json(new
                {
                    success = true,
                    data = titleName
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    success = false,
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
