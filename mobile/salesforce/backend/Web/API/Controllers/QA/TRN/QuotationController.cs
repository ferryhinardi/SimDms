using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using eXpressAPI.Models;
using Kendo.DynamicLinq;
using eXpressAPP;
using System.Web.UI;

namespace eXpressAPI.Controllers
{
    [Authorize]
    [RoutePrefix("potaindo/quo/trans")]
    [Route("{action=List}")]
    public class QuotatioController : DefaultController
    {
        private string entityName = "Quotation";
        private string menuId = "22100";
        private CodeEditorRepository myFiles = new CodeEditorRepository();

        public JsonResult List(int take, int skip, IEnumerable<Sort> sort, Kendo.DynamicLinq.Filter filter)
        {
            // Check Access
            if (IsAllowAccess(menuId, "list") != "1")
            {
                SaveResult ret = new SaveResult(0, false);
                ret.message = "Access denied";
                Response.StatusCode = 401;
                return Json(ret, JsonRequestBehavior.AllowGet);
            }

            string search = Request["search"];
            var list = db.VwQuotations.Where(x => x.CompanyCode == CompanyId).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                list =
                    list.Where(
                        x =>
                            x.Description.Contains(search) || x.ProjectName.Contains(search) ||
                            x.AutoNo.Contains(search) || x.CustomerName.Contains(search));
            }

            // If Role is Sales Active, user just allow to view current user data only (base on createdby)
            // Jadi, jika role yg digunakan oleh user tersetting is sales = true, maka user tersebut hanya dapat melihat datanya saja, tidak semua quotation yg ada didatabase
            if (LoginAsSales)
            {
                list = list.Where(y => y.CreatedBy == (UserId) || y.Sales == SalesCode);
            }

            if (sort == null)
            {
                list = list.OrderByDescending(x => x.AutoNo);
            }
            return Json(list.ToDataSourceResult(take, skip, sort, filter));
        }

        public JsonResult GridAdd(QaHeader data)
        {
            SaveResult ret = new SaveResult(0, false);

            // Check access dari sisi backend, base on user id, menu id dan access name
            // di sisi client, gunakan function isAllowAccess untuk mengaktifkan atau menonaktifkan button 
            if (IsAllowAccess(menuId, "add") != "1")
            {
                ret.message = "Access denied";
                return Json(ret, JsonRequestBehavior.AllowGet);
            }

            if (data.AutoNo == "<AUTO>")
            {
                data.AutoNo = GetAutoNo("QUOTATION");
                data.Initial = data.TypeId;
                data.DocType = "SALES";
                data.HeaderPrint = Settings.HeaderQuoMess;
                data.FooterPrint = Settings.FooterQuoMess;

                SqlQuery("exec sp_create_quotation '" + CompanyId + "','" + UserId + "','" + data.AutoNo + "','" + data.TypeId + "'");

                data.SubTotal = 0;
                data.GrandTotal = 0;
                data.Discount = 0;
                data.DiscountPrice = 0;

                ret = EntryAdd(data, string.Format("Buat data {0} berhasil",entityName));

                SqlQuery(string.Format("exec sp_calculate_price '{0}','{1}' ", CompanyId, data.AutoNo));
            }
            else
            {
                ret.message = string.Format("Data {0} dengan Id yang Anda masukkan sudah tersedia",entityName);
            }

            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GridUpdate(QaHeader data)
        {
            if (IsAllowAccess(menuId, "update") != "1")
            {
                SaveResult ret = new SaveResult(0, false);
                ret.message = "Access denied";
                return Json(ret, JsonRequestBehavior.AllowGet);
            }

            JavaScriptSerializer jss = new JavaScriptSerializer();

            List<DiscountList> list = jss.Deserialize<List<DiscountList>>(data.ListDiscount);

            SqlQuery("exec sp_cleanup_quotation '" + CompanyId +  "','" + data.AutoNo + "'");

            if (list.Count > 0)
            {
                foreach (var item in list)
                {
                    var ds = new QaDiscount();
                    ds.CompanyCode = CompanyId;
                    ds.TypeId = data.AutoNo;
                    ds.Currency = item.Currency;
                    ds.Discount = item.Discount;
                    ds.Status = true;

                    EntryAdd(ds);
                }
            }

            SqlQuery(string.Format("exec sp_calculate_price '{0}','{1}' ", CompanyId, data.AutoNo));

            try
            {
                var retItem = EntryUpdate(data, string.Format("Update data {0} berhasil", entityName));

                var doc = new Document();
                doc.DocNo = data.DocNo;
                doc.ReffNo = data.ReffNo;
                doc.AutoNo = data.AutoNo;
                doc.Title = data.Title;
                doc.Department = myFiles.GetSetting("QUO_DEPTID");
                doc.DocType = myFiles.GetSetting("QUO_DOCID");
                doc.Description = data.Description;
                doc.Description2 = data.CloseDescription;
                doc.Tag = data.Title + ", " + data.DocNo;
                myFiles.UpdateDocument(doc);

                return Json(retItem, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
            }

            return null;
        }

        public JsonResult GridDelete(QaHeader data)
        {
            if (IsAllowAccess(menuId, "delete") != "1")
            {
                SaveResult ret = new SaveResult(0, false);
                ret.message = "Access denied";
                return Json(ret, JsonRequestBehavior.AllowGet);
            }

            return Json(EntryDelete(data, string.Format("Delete data {0} berhasil",entityName)), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GridClone(QaHeader data)
        {
            if (IsAllowAccess(menuId, "clone") != "1")
            {
                SaveResult ret1 = new SaveResult(0, false);
                ret1.message = "Access denied";
                return Json(ret1, JsonRequestBehavior.AllowGet);
            }

            var ret = SqlQuery("exec sp_clone_quotation '" + CompanyId + "','" + UserId + "','" + data.AutoNo + "','" + data.DocNo + "'");
            ret.message = "Clonning Item type success ( " + data.AutoNo + " >> " + ret.Value() + " ) ";
            return Json(ret, JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult ResetType(string TypeId, string Nomor)
        {
            if (IsAllowAccess(menuId, "reset") != "1")
            {
                SaveResult ret1 = new SaveResult(0, false);
                ret1.message = "Access deied";
                return Json(ret1, JsonRequestBehavior.AllowGet);
            }

            var ret = SqlQuery("exec sp_create_quotation '" + CompanyId + "','" + UserId + "','" + Nomor + "','" + TypeId + "', 1");

            ret.message = "Master Type for this quotation has been reset... ";
            ret.success = true;

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
            string Calculate = Request["Calculate"] ?? "";

            if (Calculate == "1")
            {
                SqlQuery(string.Format("exec sp_calculate_price '{0}','{1}' ", CompanyId, TypeId ));
            }

            var myData = SqlQuery(string.Format("exec sp_getlistingdetail '{0}','{1}','{2}', '{3}'",  CompanyId, TypeId,SpecId, Search));

            return Json(myData.Result(data), JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadDiscount(DataSourceFilter data)
        {
            string TypeId = Request["typeid"] ?? "";

            var myData = SqlQuery(string.Format("exec [sp_get_quotation_discount] '{0}','{1}' ", CompanyId, TypeId ));

            return Json(myData.Result(data), JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveDetail(string data)
        {
            string specid = Request["specid"];
            string typeid = Request["typeid"];
            
            if (IsAllowAccess(menuId, "savedetail") != "1")
            {
                SaveResult ret = new SaveResult(0, false);
                ret.message = "Access denied";
                return Json(ret, JsonRequestBehavior.AllowGet);
            }

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
                string sql = "UPDATE QA_SPECS SET IsDeleted=1, ChangeDate=getdate(), ChangeBy='" + UserId + "' where companycode='" + CompanyId + "' and typeId='" + typeid + "' and speccategorycode='" + specid + "'";
                SqlQuery(sql);

                if (!string.IsNullOrEmpty(data))
                {
                    JavaScriptSerializer jss = new JavaScriptSerializer();

                    List<SPECS> list = jss.Deserialize<List<SPECS>>(data);

                    if (list.Count > 0)
                    {
                        foreach (var item in list)
                        {
                            if (item.ItemId == null)
                            {
                                item.ItemId = "";
                            }

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
                            catch (Exception) { }
                        }
                    }
                }
            }
            
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AttachmentUpload(Archieve data)
        {
            if (IsAllowAccess(menuId, "upload") != "1")
            {
                SaveResult ret = new SaveResult(0, false);
                ret.message = "Access denied";
                return Json(ret, JsonRequestBehavior.AllowGet);
            }
            data.Department = myFiles.GetSetting("QUO_DEPTID");
            data.DocType = myFiles.GetSetting("QUO_DOCID");
            var listData = myFiles.AddFile(data);
            return Json(new {success = true, list = listData }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AttachmentList(string Id)
        {
            if (IsAllowAccess(menuId, "listdownload") != "1")
            {
                SaveResult ret = new SaveResult(0, false);
                ret.message = "Access denied to load file list";
                return Json(ret, JsonRequestBehavior.AllowGet);
            }

            var listData = myFiles.GetList(Id);
            return Json(new { success = true, list = listData }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AttachmentDelete(string Id)
        {
            if (IsAllowAccess(menuId, "filedelete") != "1")
            {
                SaveResult ret = new SaveResult(0, false);
                ret.message = "Access denied";
                return Json(ret, JsonRequestBehavior.AllowGet);
            }

            myFiles.DeleteFile(Id);
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [OutputCache(Duration = 86400, Location = OutputCacheLocation.ServerAndClient)]
        public FileResult AttachmentDownload(string Id)
        {
            if (IsAllowAccess(menuId, "download") != "1")
            {
                return null;
            }

            try
            {
                var myFileInfo = myFiles.GetFileInfo(Id);
                return File(myFiles.GetFileStream(myFileInfo.Fileid, myFileInfo.Compress), myFileInfo.FileType ?? "application/octet-stream", myFileInfo.FileName);
            }
            catch (Exception) {}

            return null;
        }

        [HttpGet]
        public FileResult AttachmentDownloadAll(string Id)
        {
            if (IsAllowAccess(menuId, "download") != "1")
            {
                return null;
            }

            try
            {
                var myFileName = Id.Replace(@"/", "-") + ".zip";
                return File(myFiles.GetAllFilesByNo(Id), "application/zip", myFileName);
            }
            catch (Exception) { }

            return null;
        }
        
        private string ExNull(string param)
        {
            return string.IsNullOrEmpty(param) ? "" : param;
        }

        public JsonResult QueryReport(ReportParameters data)
        {
            SaveResult ret = new SaveResult(0, false);

            if (IsAllowAccess("23100", "search") != "1")
            {
                ret.message = "Access denied";
                Response.StatusCode = 401;
                return Json(ret, JsonRequestBehavior.AllowGet);
            }

            ret = SqlQuery("exec sp_quotation_general_report '" + CompanyId + "','" + UserId + "','" + ExNull(data.StartDate) +
                "','" + ExNull(data.EndDate) + "','" + ExNull(data.Project) + "','" + ExNull(data.Customer) + "','" +
                    ExNull(data.Sales) + "','" + ExNull(data.ItemType) + "','" + ExNull(data.DocType) + "'");

            return Json(ret, JsonRequestBehavior.AllowGet);
        }
    }
}