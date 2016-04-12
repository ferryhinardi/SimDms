using SimDms.DataWarehouse.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SimDms.DataWarehouse.Controllers.Api
{
    public class MstAOSWarrantyPartsController : BaseController
    {
        public JsonResult GetPartName(string partNo)
        {
            var partName = "";
            try
            {
                partName = ctx.MstItemInfos.Find("6006406", partNo).PartName;
            }
            catch
            {
                partName = "";
            }

            return Json(new { partName = partName, status = partName == "" ? "Part No tidak terdaftar" : "" });
        }

        public JsonResult getWarrantyParts(string partNo)
        {
            int i = 1;
            var data = ctx.MstAOSWarrantyParts.Where(a=>a.PartNo == (string.IsNullOrEmpty(partNo) ? a.PartNo : partNo)).ToList().Select(a => new WarrantyPartsModel
            {
                No = i++,
                PartNo = a.PartNo,
                isWarrantyParts = a.isWarrantyParts.Value,
                PartName = ctx.MstItemInfos.Find(CompanyCode, a.PartNo).PartName,
                Status = ""
            });
            return Json(data);
        }

        public JsonResult Save(string Data)
        {
            var user = "";
            if (CurrentUser.Username.Length > 15)
            {
                user = CurrentUser.Username.Substring(1, 15);
            }
            else
            {
                user = CurrentUser.Username;
            }

            JavaScriptSerializer ser = new JavaScriptSerializer();
            List<WarrantyPartsSave> WarrantyParts = ser.Deserialize<List<WarrantyPartsSave>>(Data);
            List<WarrantyPartsModel> retWarrantyParts = new List<WarrantyPartsModel>();
            var status = "";

            WarrantyParts = WarrantyParts.Where(a => a.No != "" && a.No != null).ToList();

            foreach (var item in WarrantyParts)
            {
                var warrantyPart = new spMstAOSWarrantyParts();
                var part = new SpMstItemInfo();

                try
                {
                    warrantyPart = ctx.MstAOSWarrantyParts.Find(item.PartNo);
                    part = ctx.MstItemInfos.Find(CompanyCode, item.PartNo);
                }
                catch
                {
                    status = item.Status;
                }

                if (part != null)
                {
                    try
                    {
                        if (warrantyPart == null)
                        {
                            status = "Part Save";
                            warrantyPart = new spMstAOSWarrantyParts();
                            warrantyPart.PartNo = item.PartNo;
                            ctx.MstAOSWarrantyParts.Add(warrantyPart);
                        }
                        warrantyPart.isWarrantyParts = string.IsNullOrEmpty(item.isWarrantyParts) ? false : item.isWarrantyParts.ToLower() == "true";
                        warrantyPart.CreatedBy = user;
                        warrantyPart.CreatedDate = DateTime.Now;
                        warrantyPart.LastUpdateBy = user;
                        warrantyPart.LastUpdateDate = DateTime.Now;

                        ctx.SaveChanges();

                        status = status == "" ? "Data Updated " + DateTime.Now.ToString("dd-MM-yyyy") : status;
                    }
                    catch (Exception e)
                    {
                        status = e.Message;
                    }
                }
                retWarrantyParts.Add(new WarrantyPartsModel { No = Convert.ToInt32(item.No), PartNo = item.PartNo, isWarrantyParts = warrantyPart.isWarrantyParts.Value, PartName = item.PartName, Status = status });
            }

            return Json(new { success = true, data = retWarrantyParts });
        }

        public JsonResult Publish()
        {
            var success = ctx.Database.ExecuteSqlCommand("exec uspfn_SpAOSWarrantyPartsPublish") > 0;

            return Json(new { success = success });
        }
    }
}