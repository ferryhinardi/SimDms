using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GeLang;
using SimDms.Sparepart.Models;

namespace SimDms.Sparepart.Controllers.Api
{
    public class TargetPenjualanController : BaseController
    {

        public JsonResult getdatatable(decimal dYear,decimal dMonth)
        {
            var queryable = ctx.Database.SqlQuery<sp_spMstSalesTargetDtlview>("sp_spMstSalesTargetDtl '" + CompanyCode + "','" + BranchCode + "','" +dYear + "','"+ dMonth +"'").AsQueryable();
    
            //var record = queryable.Select(x => new
            //{
            //    CategoryCode = x.CategoryCode,
            //    CategoryName = x.CategoryName,
            //    QtyTarget = x.QtyTarget,
            //    AmountTarget = x.AmountTarget,
            //    TotalJaringan = x.TotalJaringan
            //});

            return Json(queryable);



        }

        public JsonResult getSumdatatable(decimal dYear, decimal dMonth)
        {
            var queryable = ctx.Database.SqlQuery<sp_spMstSalesTargetDtlviewSum>("sp_spMstSalesTargetDtlSum '" + CompanyCode + "','" + BranchCode + "','" + dYear + "','" + dMonth + "'").FirstOrDefault();


            return Json(queryable,JsonRequestBehavior.AllowGet);



        }

        public JsonResult Save(spMstSalesTarget model)
        {
            string msg = "";
            var record = ctx.spMstSalesTargets.Find(CompanyCode, BranchCode, model.Year, model.Month);

            if (record == null)
            {
                record = new spMstSalesTarget
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    Year = model.Year,
                    Month = model.Month,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now
                };

                ctx.spMstSalesTargets.Add(record);
                msg = "New Sales Target added...";
            }
            else
            {
                ctx.spMstSalesTargets.Attach(record);
                msg = "Sales Target updated";
            }


            record.QtyTarget = model.QtyTarget;
            record.AmountTarget = model.AmountTarget;
            record.TotalJaringan = model.TotalJaringan;
 
 
            record.LastUpdateBy = CurrentUser.UserId;
            record.LastUpdateDate = DateTime.Now;

            try
            {
                ctx.SaveChanges();
              

                return Json(new { success = true, message = msg, data = record });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }




        public JsonResult Delete(spMstSalesTarget model)
        {
            try
            {
                var record = ctx.spMstSalesTargets.Find(CompanyCode, BranchCode, model.Year, model.Month);
 
                if (record == null)
                {
                    return Json(new { success = false, message = "Record not found or has been deleted" });
                }
                else
                {
                    ctx.spMstSalesTargets.Remove(record);
                }

                ctx.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }




        public JsonResult Save2(sp_spMstSalesTargetDtlview model)
        {
            string msg = "";
            
            var record = ctx.spMstSalesTargetDtls.Find(CompanyCode, BranchCode, model.Year, model.Month,model.CategoryCode);

            if (record == null)
            {
                record = new spMstSalesTargetDtl
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    Year = model.Year,
                    Month = model.Month,
                    CategoryCode=model.CategoryCode,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now
                };

                ctx.spMstSalesTargetDtls.Add(record);
                msg = "New sales target added...";
            }
            else
            {
                ctx.spMstSalesTargetDtls.Attach(record);
                msg = "Sales target updated";
            }


            record.QtyTarget =  model.QtyTarget;
            record.AmountTarget = model.AmountTarget;
            record.TotalJaringan = model.TotalJaringan; 
            record.LastUpdateBy = CurrentUser.UserId;
            record.LastUpdateDate = DateTime.Now;

            try
            {
                ctx.SaveChanges();

                var queryable = ctx.Database.SqlQuery<sp_spMstSalesTargetDtlview>("sp_spMstSalesTargetDtl '" + CompanyCode + "','" + BranchCode + "','" + model.Year + "','" + model.Month + "'").AsQueryable();
                
                if (queryable != null)
                {
                    var records = queryable.Select(x => new
                    {
                        Month = x.Month,
                        CategoryCode = x.CategoryCode,
                        CategoryName = x.CategoryName,
                        QtyTarget = x.QtyTarget,
                        AmountTarget = x.AmountTarget,
                        TotalJaringan = x.TotalJaringan
                    }).ToList();

                    var query = from t in records
                                group t by t.Month into grouping
                                select new
                                {
                                    Month = grouping.Key,
                                    QtyTarget = grouping.Sum(x => x.QtyTarget),
                                    AmountTarget = grouping.Sum(x => x.AmountTarget),
                                    TotalJaringan = grouping.Sum(x => x.TotalJaringan)
                                };

                  
                    
                    var recordHdr = ctx.spMstSalesTargets.Find(CompanyCode, BranchCode, model.Year, model.Month);
                    if (recordHdr != null)
                    {
                        ctx.spMstSalesTargets.Attach(recordHdr);
                    }

                    recordHdr.QtyTarget = query.FirstOrDefault().QtyTarget;
                    recordHdr.AmountTarget = query.FirstOrDefault().AmountTarget;
                    recordHdr.TotalJaringan = query.FirstOrDefault().TotalJaringan; 

                    recordHdr.LastUpdateBy = CurrentUser.UserId;
                    recordHdr.LastUpdateDate = DateTime.Now;
                    ctx.SaveChanges();

                    return Json(new { success = true, data = records, total = query.FirstOrDefault(), count = records.Count });
                }
                else
                {
                    return Json(new { success = true, count = 0, total = new { QtyTarget = 0, AmountTarget = 0, TotalJaringan = 0 } });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }




        public JsonResult Delete2(spMstSalesTargetDtl model)
        {
   
            var record = ctx.spMstSalesTargetDtls.Find(CompanyCode, BranchCode, model.Year, model.Month, model.CategoryCode);

 
            if (record == null)
            {
                return Json(new { success = false, message = "Record not found or has been deleted" });
            }
            else
            {
                ctx.spMstSalesTargetDtls.Remove(record);
            }

            try
            {
                ctx.SaveChanges();

                var queryable = ctx.Database.SqlQuery<sp_spMstSalesTargetDtlview>("sp_spMstSalesTargetDtl '" + CompanyCode + "','" + BranchCode + "','" + model.Year + "','" + model.Month + "'").AsQueryable();

                if (queryable != null)
                {
                    var records = queryable.Select(x => new
                    {
                        Month = x.Month,
                        CategoryCode = x.CategoryCode,
                        CategoryName = x.CategoryName,
                        QtyTarget = x.QtyTarget,
                        AmountTarget = x.AmountTarget,
                        TotalJaringan = x.TotalJaringan
                    }).ToList();

                    var query = from t in records
                                group t by t.Month into grouping
                                select new
                                {
                                    Month = grouping.Key,
                                    QtyTarget = grouping.Sum(x => x.QtyTarget),
                                    AmountTarget = grouping.Sum(x => x.AmountTarget),
                                    TotalJaringan = grouping.Sum(x => x.TotalJaringan)
                                };
                    
                    var recordHdr = ctx.spMstSalesTargets.Find(CompanyCode, BranchCode, model.Year, model.Month);
                    if (recordHdr != null)
                    {
                        ctx.spMstSalesTargets.Attach(recordHdr);
                    }

                    recordHdr.QtyTarget = query.FirstOrDefault().QtyTarget;
                    recordHdr.AmountTarget = query.FirstOrDefault().AmountTarget;
                    recordHdr.TotalJaringan = query.FirstOrDefault().TotalJaringan;

                    recordHdr.LastUpdateBy = CurrentUser.UserId;
                    recordHdr.LastUpdateDate = DateTime.Now;
                    ctx.SaveChanges();

                    return Json(new { success = true, data = records, total = query.FirstOrDefault(), count = records.Count });
                }
                else
                {
                    return Json(new { success = true, count = 0, total = new { QtyTarget=0, AmountTarget=0, TotalJaringan=0 } });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        public JsonResult getAmountAll(string Year, string Month) 
        {
            var queryable = ctx.Database.SqlQuery<sp_spMstSalesTargetDtlview>("sp_spMstSalesTargetDtl '" + CompanyCode + "','" + BranchCode + "','" + Year + "','" + Month + "'").AsQueryable();

            if (queryable != null)
            {
                var records = queryable.Select(x => new
                {
                    Month = x.Month,
                    CategoryCode = x.CategoryCode,
                    CategoryName = x.CategoryName,
                    QtyTarget = x.QtyTarget,
                    AmountTarget = x.AmountTarget,
                    TotalJaringan = x.TotalJaringan
                }).ToList();

                var query = from t in records
                            group t by t.Month into grouping
                            select new
                            {
                                Month = grouping.Key,
                                QtyTarget = grouping.Sum(x => x.QtyTarget),
                                AmountTarget = grouping.Sum(x => x.AmountTarget),
                                TotalJaringan = grouping.Sum(x => x.TotalJaringan)
                            };

                return Json(new { success = true, data = records, total = query.FirstOrDefault(), count = records.Count });
            }
            else
            {
                return Json(new { success = true, count = 0, total = new { QtyTarget = 0, AmountTarget = 0, TotalJaringan = 0 } });
            }
        }
 
 
 
 
    }
}
