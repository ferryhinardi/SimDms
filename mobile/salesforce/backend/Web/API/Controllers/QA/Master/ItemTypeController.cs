using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using eXpressAPI.Models;
using Kendo.DynamicLinq;

namespace eXpressAPI.Controllers
{
    [Authorize]
    [RoutePrefix("potaindo/qa/tipe")]
    [Route("{action=List}")]
    public class tipeController : DefaultController
    {
        private string entityName = "Master Type";
        private string menuId = "21300";

        public JsonResult List(int take, int skip, IEnumerable<Sort> sort, Kendo.DynamicLinq.Filter filter)
        {
            if (IsAllowAccess(menuId, "list") != "1")
            {
                SaveResult ret = new SaveResult(0, false) { message = "Access denied" };
                Response.StatusCode = 401;
                return Json(ret, JsonRequestBehavior.AllowGet);
            }

            string search = Request["search"];
            var list = db.VwItemTypes.Where(x => x.CompanyCode == CompanyId).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                list = list.Where(x => x.Description.Contains(search) || x.Name.Contains(search) || x.Code.Contains(search) || x.CategoryName.Contains(search));
            }
            if (sort == null)
            {
                list = list.OrderByDescending(x => x.Code);
            }
            return Json(list.ToDataSourceResult(take, skip, sort, filter));
        }

        public JsonResult GridAdd(ItemType data)
        {
            SaveResult ret = new SaveResult(0, false);

            if (IsAllowAccess(menuId, "add") != "1")
            {
                ret.message = "Access denied";
                return Json(ret, JsonRequestBehavior.AllowGet);
            }

            var isFound = db.ItemTypes.Where(x => x.Code == data.Code && x.CompanyCode == CompanyId).ToList();

            if (isFound.Count == 0)
            {
                data.Code = GetAutoNo("TYPE");
                ret = EntryAdd(data, string.Format("Buat data {0} berhasil",entityName));
            }
            else
            {
                ret.message = string.Format("Data {0} dengan Id yang Anda masukkan sudah tersedia",entityName);
            }

            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GridUpdate(ItemType data)
        {
            if (IsAllowAccess(menuId, "update") == "1")
                return Json(EntryUpdate(data, string.Format("Update data {0} berhasil", entityName)),
                    JsonRequestBehavior.AllowGet);

            SaveResult ret = new SaveResult(0, false) { message = "Access denied" };
            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GridDelete(ItemType data)
        {
            if (IsAllowAccess(menuId, "delete") == "1")
                return Json(EntryDelete(data, string.Format("Delete data {0} berhasil", entityName)),
                    JsonRequestBehavior.AllowGet);

            SaveResult ret = new SaveResult(0, false) { message = "Access denied" };
            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GridClone(ItemType data)
        {
            if (IsAllowAccess(menuId, "clone") != "1")
            {
                SaveResult ret1 = new SaveResult(0, false) { message = "Access denied" };
                return Json(ret1, JsonRequestBehavior.AllowGet);
            }

            var ret = SqlQuery("exec sp_clone_itemtypes '" + CompanyId + "','" + UserId + "','" + data.Code + "','" + data.Model + "'");
            ret.message = "Clonning Item type success ( " + data.Code + " >> " + ret.Value() + " ) ";
            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadDetail(DataSourceFilter data)
        {
            if (IsAllowAccess(menuId, "loaddetail") != "1")
            {
                SaveResult ret = new SaveResult(0, false) { message = "Access denied to load detail" };
                Response.StatusCode = 401;
                return Json(ret, JsonRequestBehavior.AllowGet);
            }

            string TypeId = Request["typeid"] ?? "";
            string SpecId = Request["specid"] ?? "";
            string Search = Request["search"] ?? "";

            var myData = SqlQuery(string.Format("exec sp_getlistingdetail '{0}','{1}','{2}', '{3}'",  CompanyId, TypeId,SpecId, Search));

            return Json(myData.Result(data), JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveDetail(string data)
        {
            if (IsAllowAccess(menuId, "savedetail") != "1")
            {
                SaveResult ret = new SaveResult(0, false) { message = "Access denied" };
                return Json(ret, JsonRequestBehavior.AllowGet);
            }

            string specid = Request["specid"];
            string typeid = Request["typeid"];

            if (LoginAsSales)
            {
                if (string.IsNullOrEmpty(data)) return Json(new {success = true}, JsonRequestBehavior.AllowGet);

                JavaScriptSerializer jss = new JavaScriptSerializer();
                List<SPECS> list = jss.Deserialize<List<SPECS>>(data);

                foreach (var item in list)
                {
                    SqlQuery("UPDATE QA_SPECS SET Description='" + item.Description + "', Value='" + item.Value +
                             "', Qty='" + item.Qty + "', Total=Price*" + item.Qty +
                             "-Discount, ChangeDate=GETDATE(), ChangeBy='" + UserId + "' WHERE CompanyCode='" +
                             CompanyId + "' AND No='" + item.No + "' AND TypeId='" + item.TypeId + "' AND ItemId='" +
                             item.ItemId + "' AND SpecCategoryCode='" + item.SpecCategoryCode + "'");
                }
            }
            else
            {
                string sql = "DELETE FROM   QA_SPECS where companycode='" + CompanyId + "' and typeId='" + typeid + "' and speccategorycode='" + specid + "' and IsDeleted=1;";
                sql += "UPDATE QA_SPECS SET IsDeleted=1, ChangeDate=getdate(), ChangeBy='" + UserId + "' where companycode='" + CompanyId + "' and typeId='" + typeid + "' and speccategorycode='" + specid + "'";
                SqlQuery(sql);

                if (!string.IsNullOrEmpty(data))
                {
                    JavaScriptSerializer jss = new JavaScriptSerializer();

                    List<SPECS> list = jss.Deserialize<List<SPECS>>(data);

                    if (list.Count > 0)
                    {
                        foreach (var item in list)
                        {
                            item.ItemId = item.ItemId ?? "";

                            var existData = db.SPECS.Find(CompanyId, item.No, item.TypeId, item.ItemId, item.SpecCategoryCode);

                            var currcode = db.Currencies.FirstOrDefault(x => x.CompanyCode == CompanyId && x.Name == item.Currency);
                            if (currcode != null)
                            {
                                item.Currency = currcode.Code;
                            }

                            var uomCode = db.UOM.FirstOrDefault(x => x.CompanyCode == CompanyId && x.Name == item.UOM);
                            if (uomCode != null)
                            {
                                item.UOM = uomCode.Code;
                            }

                            item.Price = item.Price ?? 0;
                            item.Qty = item.Qty ?? 0;
                            item.Discount = item.Discount ?? 0;
                            item.Total = item.Total ?? 0;

                            if (existData == null)
                            {
                                db.SPECS.Add(item);
                            }
                            else
                            {
                                existData.SpecCategory = item.SpecCategory;
                                existData.IsDeleted = false;
                                existData.Description = item.Description;
                                existData.Value = item.Value;
                                existData.Refference = item.Refference;
                                existData.UOM = item.UOM;
                                existData.Price = item.Price;
                                existData.Qty = item.Qty;
                                existData.Discount = item.Discount;
                                existData.Total = item.Total;
                                existData.IsHeader = item.IsHeader;
                                existData.Currency = item.Currency;
                                existData.Printable = item.Printable;
                                db.Entry(existData).State = System.Data.Entity.EntityState.Modified;
                            }

                            try
                            {
                                SaveChanges();
                            }
                            catch (Exception ex) { }
                        }
                    }
                }
            }
            
            
            
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }
    }
}