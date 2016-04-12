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
    [RoutePrefix("potaindo/qa/item")]
    [Route("{action=List}")]
    public class ItemController : DefaultController
    {
        private string entityName = "Master Item";
        private string menuId = "21401";

        public JsonResult List(int take, int skip, IEnumerable<Sort> sort, Kendo.DynamicLinq.Filter filter)
        {
            if (IsAllowAccess(menuId, "list") != "1")
            {
                SaveResult ret = new SaveResult(0, false) { message = "Access denied" };
                Response.StatusCode = 401;
                return Json(ret, JsonRequestBehavior.AllowGet);
            }

            string search = Request["search"];
            var list = db.VwItems.Where(x => x.CompanyCode == CompanyId).AsQueryable();

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

        public JsonResult GridAdd(Item data)
        {
            SaveResult ret = new SaveResult(0, false);

            if (IsAllowAccess(menuId, "add") != "1")
            {
                ret.message = "Access denied";
                return Json(ret, JsonRequestBehavior.AllowGet);
            }

            var isFound = db.Items.Where(x => x.Code == data.Code && x.CompanyCode == CompanyId).ToList();

            if (isFound.Count == 0)
            {
                data.Code = GetAutoNo("PART");
                ret = EntryAdd(data, string.Format("Buat data {0} berhasil",entityName));
            }
            else
            {
                ret.message = string.Format("Data {0} dengan Id yang Anda masukkan sudah tersedia",entityName);
            }

            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GridUpdate(Item data)
        {
            if (IsAllowAccess(menuId, "update") == "1")
                return Json(EntryUpdate(data, string.Format("Update data {0} berhasil", entityName)),
                    JsonRequestBehavior.AllowGet);

            SaveResult ret = new SaveResult(0, false) { message = "Access denied" };
            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GridDelete(Item data)
        {
            if (IsAllowAccess(menuId, "delete") == "1")
                return Json(EntryDelete(data, string.Format("Delete data {0} berhasil", entityName)),
                    JsonRequestBehavior.AllowGet);

            SaveResult ret = new SaveResult(0, false) { message = "Access denied" };
            return Json(ret, JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult GetPriceList()
        {
            if (IsAllowAccess(menuId, "loadprice") != "1")
            {
                SaveResult ret = new SaveResult(0, false) { message = "Access denied to load price list" };
                return Json(ret, JsonRequestBehavior.AllowGet);
            }

            string ItemId = Request["ItemId"] ?? "";
            var list = db.VwPrices.Where(x => x.CompanyCode == CompanyId && x.ItemId == ItemId).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdatePriceList(string data)
        {
            if (IsAllowAccess(menuId, "updateprice") != "1")
            {
                SaveResult ret = new SaveResult(0, false) {message = "Access denied"};
                return Json(ret, JsonRequestBehavior.AllowGet);
            }

            JavaScriptSerializer jss = new JavaScriptSerializer();

            List<EntryItemPrice> list = jss.Deserialize<List<EntryItemPrice>>(data);

            if (list.Count > 0)
            {
                var SQL = string.Format("DELETE FROM [QA_ITEM_PRICES] where CompanyCode='{0}' and ItemId='{1}'", CompanyId, list[0].ItemId);
                db.Database.ExecuteSqlCommand(SQL);
                db.SaveChanges();
            }

            foreach (var item in list)
            {
                try
                {
                    var Curr = db.Currencies.FirstOrDefault(x => x.CompanyCode == CompanyId && x.Name == item.CurrName);
                    //var checkCUrr = Curr.Code;
                    //var checkREff = item.Description;

                    //if (item.LastCurr != null)
                    //{
                    //    checkCUrr = item.LastCurr;
                    //}

                    //if (item.LastRef != null)
                    //{
                    //    checkREff = item.Description;
                    //}



                    //var findPrice = db.ItemPrices.Find(CompanyId, item.ItemId, checkREff, item.UOM, checkCUrr);

                    //if (findPrice == null)
                    //{
                        var newItem = new ItemPrice();
                        newItem.CompanyCode = CompanyId;
                        newItem.ItemId = item.ItemId;
                        newItem.Description = item.Description;
                        newItem.Currency = Curr.Code;
                        newItem.UOM = item.UOM;
                        newItem.Price = item.Price;
                        newItem.EffectiveDate = item.EffectiveDate;
                        newItem.ExpiredDate = item.ExpiredDate;
                        newItem.Status = true;

                        db.ItemPrices.Add(newItem);

                    //}
                    //else
                    //{
                    //    findPrice.Price = item.Price;
                    //    findPrice.ExpiredDate = item.ExpiredDate;
                    //    if (item.ExpiredDate < DateTime.Now)
                    //    {
                    //        findPrice.Status = false;
                    //    }
                    //    findPrice.Currency = Curr.Code;
                    //    db.Entry(findPrice).State = System.Data.Entity.EntityState.Modified;
                        
                    //}
                    SaveChanges();


                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);     
                }

            }

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }
    }
}