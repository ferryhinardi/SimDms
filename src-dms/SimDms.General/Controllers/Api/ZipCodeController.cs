using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.General.Models;
//using SimDms.Sparepart.Models;
using SimDms.General.Models.Others;
using SimDms.Common;
using SimDms.Common.Models;
using System.Web.Script.Serialization;
using TracerX;
using System.Transactions;
using GeLang;

namespace SimDms.General.Controllers.Api
{
    public class ZipCodeController : BaseController
    {
        public JsonResult ZipCodeLoad(int cols)
        {
            //var zipcode = ctx.GnMstZipCode.Where(p => p.CompanyCode == CompanyCode);
            //var queryable = ctx.GnMstZipCode.OrderBy(p => p.CreatedDate asc).AsQueryable();

            var field = "";
            var value = "";

            string dynamicFilter = "";

            for (int i = 0; i < cols; i++)
            {
                field = Request["filter[filters][" + i + "][field]"] ?? "";
                value = Request["filter[filters][" + i + "][value]"] ?? "";

                if (dynamicFilter == "")
                {
                    dynamicFilter += value != "" ? " AND " + field + " LIKE '%" + value + "%'" : "";
                }
                else
                {
                    dynamicFilter += value != "" ? " AND " + field + " LIKE '%" + value + "%'" : "";
                }
            }

            dynamicFilter = dynamicFilter != "" ? dynamicFilter += "" : "";

            var query = string.Format(@"
                select top 500 ZipCode, KelurahanDesa, KecamatanDistrik, 
                KotaKabupaten, IbuKota, isCity, Notes from GnMstZipCode
                where CompanyCode ='{0}' {1}
                ORDER BY CreatedDate DESC
                       ", CompanyCode, dynamicFilter);
            var queryable = ctx.Database.SqlQuery<GnMstZipCodeView>(query).AsQueryable();
            //return Json(sqlstr);
            return Json(queryable.toKG()); 
        }
        [HttpPost]
        public JsonResult Save(GnMstZipCode model)
        {
            ResultModel result = InitializeResultModel();
            string userID = CurrentUser.UserId;
            string companyCode = CompanyCode;
            string branchCode = BranchCode;
            DateTime currentTime = DateTime.Now;

            var zipcode = ctx.GnMstZipCode.Find(companyCode, model.ZipCode, model.KelurahanDesa, model.KecamatanDistrik, model.KotaKabupaten, model.IbuKota);

            if (zipcode == null)
            {
                zipcode = new GnMstZipCode();
                zipcode.CreatedDate = currentTime;
                zipcode.LastUpdateDate = currentTime;
                zipcode.CreatedBy = userID;
                ctx.GnMstZipCode.Add(zipcode);
            }
            else
            {
                zipcode.LastUpdateDate = currentTime;
                zipcode.LastUpdateBy = userID;
            }
            zipcode.CompanyCode= companyCode;
            zipcode.ZipCode= model.ZipCode;
            zipcode.KelurahanDesa= model.KelurahanDesa;
            zipcode.KecamatanDistrik= model.KecamatanDistrik;
            zipcode.isCity= model.isCity;
            zipcode.KotaKabupaten= model.KotaKabupaten;
            zipcode.IbuKota= model.IbuKota;
            zipcode.Notes= model.Notes;  
            zipcode.isLocked= model.isLocked;
            zipcode.LockingBy= model.LockingBy;
            zipcode.LockingDate = model.LockingDate;
            try
            {
                ctx.SaveChanges();
                result.status = true;
                result.message = "Data Kode Pos berhasil disimpan.";
                result.data = new
                {
                    ZipCode = zipcode.ZipCode,
                    IbuKota = zipcode.IbuKota
                };
            }
            catch (Exception Ex)
            {
                result.message = "Data Kode Pos tidak bisa disimpan.";
                MyLogger.Info("Error on Zip Code saving: " + Ex.Message);
            }

            return Json(result);
        }


        public JsonResult Delete(GnMstZipCode model)
        {
            object returnObj = null;
            string companyCode = CompanyCode;
            using (TransactionScope trans = new TransactionScope())
            {
                try
                {
                    var zipcode = ctx.GnMstZipCode.Find(companyCode, model.ZipCode, model.KelurahanDesa, model.KecamatanDistrik, model.KotaKabupaten, model.IbuKota);
                    if (zipcode != null)
                    {
                        ctx.GnMstZipCode.Remove(zipcode);
                        ctx.SaveChanges();
                        returnObj = new { success = true, message = "Data Kode Pos berhasil di delete." };
                        trans.Complete();
                    }
                    else
                    {
                        returnObj = new { success = false, message = "Error ketika menghapus Kode Pos, Message=Data tidak ditemukan" };
                        trans.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    returnObj = new { success = false, message = "Error ketika menghapus Kode Pos, Message=" + ex.ToString() };
                    trans.Dispose();
                }
            }
            return Json(returnObj);
        }
    }

}
