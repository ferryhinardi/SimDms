using SimDms.Service.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Service.Controllers.Api
{
    public class EventController : BaseController
    {

        public JsonResult Default()
        {
            return Json(new
            {

                EventDate = DateTime.Now,
                EventStartDate = DateTime.Now,
                EventEndDate = DateTime.Now,
                LaborDiscPct = "0.00",
                PartsDiscPct = "0.00",
                MaterialDiscPct = "0.00",
                EventNo = "EVT/XX/YYYYYY"

            });
        }

          //public string getData()
          //{
          //    //var transdate = ctx.CoProfileServices.Find(CompanyCode, BranchCode).TransDate;
          //    //return GetNewDocumentNo("EVT", transdate.Value);
          //    return GetNewDocumentNo("EVT", DateTime.Now);
          //}


        public JsonResult Save(svMstEvent model, string BasicModel)
        {
            string sqlstr = string.Empty;
            //string NoDoc = "";
            try
            {
                //sqlstr = ctx.Database.ExecuteSqlCommand(" Exec uspfn_GnDocumentGetNew '" + CompanyCode + "','" + BranchCode + "','EVT','" + CurrentUser.UserId + "','" + model.EventDate + "'").ToString();
                sqlstr = ctx.Database.SqlQuery<string>("uspfn_GnDocumentGetNew '" + CompanyCode + "','" + BranchCode + "','EVT','" + CurrentUser.UserId + "','" + model.EventDate + "'").FirstOrDefault();
            }
            catch (Exception ex) 
            {
                return Json(new { success = false, message = ex.Message });
            }
            
            int jum;
            if (BasicModel == "ALL")
            {
                 jum = BasCount();
                var data = getBasicMod();

                for (var i = 0; i < jum; i++)
                {


                    //string word = getData();
                    var record = ctx.svMstEvents.Find(CompanyCode, model.EventNo);
                    if (record == null)
                    {
                        record = new svMstEvent
                        {
                            CompanyCode = CompanyCode,
                            EventNo = sqlstr,
                            CreatedBy = CurrentUser.UserId,
                            CreatedDate = DateTime.Now,

                        };
                        ctx.svMstEvents.Add(record);
                    }
                    record.EventDate = model.EventDate;
                    record.Description = model.Description;
                    record.BasicModel = data[i];
                    record.JobType = model.JobType;
                    record.EventStartDate = model.EventStartDate;
                    record.EventEndDate = model.EventEndDate;
                    record.IsDiscount = true;
                    record.LaborDiscPct = model.LaborDiscPct;
                    record.PartsDiscPct = model.PartsDiscPct;
                    record.MaterialDiscPct = model.MaterialDiscPct;
                    record.TotalAmount = model.TotalAmount;
                    record.IsActive = model.IsActive;
                    record.IsLocked = false;
                    record.LockingDate = DateTime.Now;
                    record.LastupdateBy = CurrentUser.UserId;
                    record.LastupdateDate = DateTime.Now;

                }

            }
            else {
                jum = 1;
                //string word = getData();
                var record = ctx.svMstEvents.Find(CompanyCode, model.EventNo);
                if (record == null)
                {
                    record = new svMstEvent
                    {
                        CompanyCode = CompanyCode,
                        EventNo = sqlstr,
                        CreatedBy = CurrentUser.UserId,
                        CreatedDate = DateTime.Now,

                    };
                    ctx.svMstEvents.Add(record);
                }
                record.EventDate = model.EventDate;
                record.Description = model.Description;
                record.BasicModel = model.BasicModel;
                record.JobType = model.JobType;
                record.EventStartDate = model.EventStartDate;
                record.EventEndDate = model.EventEndDate;
                record.IsDiscount = true;
                record.LaborDiscPct = model.LaborDiscPct;
                record.PartsDiscPct = model.PartsDiscPct;
                record.MaterialDiscPct = model.MaterialDiscPct;
                record.TotalAmount = model.TotalAmount;
                record.IsActive = model.IsActive;
                //record.IsLocked = false;
                //record.LockingDate = DateTime.Now;
                record.LastupdateBy = CurrentUser.UserId;
                //record.LastupdateDate = DateTime.Now;
            }
              try
              {
                  ctx.SaveChanges();
                  return Json(new { success = true, jum });
              }
              catch (Exception ex)
              {
                  return Json(new { success = false, message = ex.Message });
              }
           
        }

        public JsonResult deleteData(svMstEvent model)
        {

            var record = ctx.svMstEvents.Find(CompanyCode, model.EventNo);
            if (record != null)
            {
                ctx.svMstEvents.Remove(record);
            }

            try
            {
                ctx.SaveChanges();
                return Json(new { success = true, jum });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }


        }

        public JsonResult CBasmodOpen(string BasicModel)
        {
            var queryable = ctx.SvCBasmodViews.Where(a => a.CompanyCode == CompanyCode && a.ProductType == ProductType && a.BasicModel == BasicModel).FirstOrDefault();
            return Json(new { success = queryable != null, data = queryable });
        }

        public JsonResult JobView(string JobType)
        {
            var queryable = ctx.SvJobViews.Where(x => x.CompanyCode == CompanyCode && x.ProductType == ProductType && x.JobType == JobType).FirstOrDefault();
            return Json(new { success = queryable != null, data = queryable });

        }
        public List<string> getBasicMod()
        {
            var record = ctx.SvEventBMs.Where(a => a.CompanyCode == CompanyCode).Select(a => a.BasicModel).ToList();            
            return record;
        }

        public int BasCount() {
            var record = ctx.SvEventBMs.Where(a => a.CompanyCode == CompanyCode).Select(a => new
            {
                BasicModel = a.BasicModel,
            });
            var jum = record.Count();            
            return jum;
        }


        public object jum { get; set; }

        public JsonResult BasicModel()
        {
            var trans = ctx.svMstEvents
                .Where(x => x.CompanyCode == CompanyCode)
                .OrderBy(x => x.BasicModel)
                .Select(x => new { value = x.BasicModel, text = x.BasicModel }).ToList();
            return Json(trans, JsonRequestBehavior.AllowGet);
        }

        public JsonResult JobType()
        {
            var trans = ctx.svMstEvents
                .Where(x => x.CompanyCode == CompanyCode)
                .OrderBy(x => x.JobType)
                .Select(x => new { value = x.JobType, text = x.JobType }).ToList();
            return Json(trans, JsonRequestBehavior.AllowGet);
        }
    }
}
