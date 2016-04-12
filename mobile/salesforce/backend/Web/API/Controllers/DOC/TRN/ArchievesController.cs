using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using eXpressAPI.Models;
using Kendo.DynamicLinq;
using System;
using eXpressAPP;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System.Web.UI;
using MongoDB.Bson;
using System.Text.RegularExpressions;

namespace eXpressAPI.Controllers
{
    [Authorize]
    [RoutePrefix("potaindo/doc/archieve")]
    [Route("{action=List}")]
    public class ArchieveController : DefaultController
    {
        private string entityName = "Input Dokumen";
        private string menuId = "32100";
        private CodeEditorRepository myFiles = new CodeEditorRepository();

        public JsonResult List(int take, int skip, IEnumerable<Sort> sort, Kendo.DynamicLinq.Filter filter)
        {
            if (IsAllowAccess(menuId, "list") != "1")
            {
                SaveResult ret = new SaveResult(0, false);
                ret.message = "Access denied";
                Response.StatusCode = 401;
                return Json(ret, JsonRequestBehavior.AllowGet);
            }

            string search = Request["search"];
            var list = db.DHeaders.Where(x => x.CompanyCode == CompanyId).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                list = list.Where(x => x.Description.Contains(search) || x.Title.Contains(search) || x.Keyword.Contains(search) );
            }
            if (sort == null)
            {
                list = list.OrderByDescending(x => x.DocNo);
            }
            return Json(list.ToDataSourceResult(take, skip, sort, filter));
        }

        private void TrimArrayString(ref DocHeader data)
        {
            if (data.DeptId != null)
            {
                var n = data.DeptId.Length;
                if (n > 3)
                {
                    data.DeptId = data.DeptId.Substring(1, n - 2).Replace("\"","");
                }
            }
            if (data.DocType != null)
            {
                var n = data.DocType.Length;
                if (n > 3)
                {
                    data.DocType = data.DocType.Substring(1, n - 2).Replace("\"","");
                }
            }
        }

        public JsonResult GridAdd(DocHeader data)
        {
            SaveResult ret = new SaveResult(0, false);

            if (IsAllowAccess(menuId, "add") != "1")
            {
                ret.message = "Access denied";
                return Json(ret, JsonRequestBehavior.AllowGet);
            }

            if (data.DocNo == "<AUTO>")
            {
                data.DocNo = GetAutoNo("DOCUMENT");
                
                TrimArrayString(ref data);
                ret = EntryAdd(data, string.Format("Buat data {0} berhasil",entityName));
            }
            else
            {
                ret.message = string.Format("Data {0} dengan Id yang Anda masukkan sudah tersedia",entityName);
            }

            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GridUpdate(DocHeader data)
        {
            TrimArrayString(ref data);

            if (IsAllowAccess(menuId, "update") == "1")
            {
                var doc = new Document();
                doc.DocNo = data.ReffNo;
                doc.AutoNo = data.DocNo;
                doc.Title = data.Title;
                doc.Department = data.DeptId;
                doc.DocType = data.DocType;
                doc.Description = data.Description;
                doc.Tag = data.Keyword;
                doc.Private = data.IsPrivate;
                myFiles.UpdateDocument(doc);

                return Json(EntryUpdate(data, string.Format("Update data {0} berhasil", entityName)),
                    JsonRequestBehavior.AllowGet);
            }
            
            SaveResult ret = new SaveResult(0, false) { message = "Access denied" };
            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GridDelete(DocHeader data)
        {
            if (IsAllowAccess(menuId, "delete") == "1")
                return Json(EntryDelete(data, string.Format("Delete data {0} berhasil", entityName)),
                    JsonRequestBehavior.AllowGet);

            SaveResult ret = new SaveResult(0, false) { message = "Access denied" };
            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AttachmentUpload(Archieve data)
        {
            if (IsAllowAccess(menuId, "upload") != "1")
            {
                SaveResult ret = new SaveResult(0, false) { message = "Access denied" };
                return Json(ret, JsonRequestBehavior.AllowGet);
            }

            if (data.Department != null)
            {
                var n = data.Department.Length;
                if (n > 3)
                {
                    data.Department = data.Department.Substring(1, n - 2).Replace("\"", "");
                }
            }

            if (data.DocType != null)
            {
                var n = data.DocType.Length;
                if (n > 3)
                {
                    data.DocType = data.DocType.Substring(1, n - 2).Replace("\"", "");
                }
            }
             
            var listData = myFiles.AddFile(data);

            return Json(new { success = true, list = listData }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AttachmentList(string Id)
        {
            if (IsAllowAccess(menuId, "listdownload") != "1")
            {
                SaveResult ret = new SaveResult(0, false) { message = "Access denied to load file list" };
                return Json(ret, JsonRequestBehavior.AllowGet);
            }

            var listData = myFiles.GetList(Id);
            return Json(new { success = true, list = listData }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AttachmentDelete(string Id)
        {
            if (IsAllowAccess(menuId, "filedelete") != "1")
            {
                SaveResult ret = new SaveResult(0, false) { message = "Access denied" };
                return Json(ret, JsonRequestBehavior.AllowGet);
            }

            myFiles.DeleteFile(Id);
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [MyOutputCache(Location = OutputCacheLocation.ServerAndClient)]
        public ActionResult Thumbnail(string Id)
        {
            if (IsAllowAccess(menuId, "download") != "1")
            {
                return null;
            }

            try
            {
                var myFileInfo = myFiles.GetFileInfo(Id);
                if (myFileInfo != null)
                {
                    Response.Headers.Remove("Content-Disposition");
                    Response.AppendHeader("Content-Disposition", "inline; filename=" + myFileInfo.FileName);
                    if (myFileInfo.FileType.Contains("image"))
                    {
                        return File(myFiles.CreateThumbnail(myFiles.GetFileStream(myFileInfo.Fileid, myFileInfo.Compress)), myFileInfo.FileType ?? "application/octet-stream");
                    }
                    else
                    {
                        var fnX = Settings.GetSetting(myFileInfo.FileType);
                        if (string.IsNullOrEmpty(fnX))
                        {
                            fnX = Settings.GetSetting("IMAGE_NOT_AVAILABLE");
                        }

                        if (fnX.Contains("cdn/"))
                        {
                            var filename = fnX.Substring(4).ToLower();
                            var fi = myFiles.GetFileInfo3(filename);
                            return File(myFiles.GetFile3(fi.Fileid), fi.FileType ?? "application/octet-stream");
                        }
                        else
                        {
                            return File(Server.MapPath(fnX), "image/jpeg");
                        }
                    }
                }
            }
            catch (Exception) { }

            return null;
        }
        
        [HttpGet]
        [MyOutputCache(Location = OutputCacheLocation.ServerAndClient)]
        public ActionResult ThumbnailLarge(string Id)
        {
            if (IsAllowAccess(menuId, "download") != "1")
            {
                return null;
            }

            try
            {
                var myFileInfo = myFiles.GetFileInfo(Id);
                if (myFileInfo != null)
                {
                    Response.Headers.Remove("Content-Disposition");
                    Response.AppendHeader("Content-Disposition", "inline; filename=" + myFileInfo.FileName);
                    if (myFileInfo.FileType.Contains("image"))
                    {
                        return File(myFiles.CreateThumbnail2(myFiles.GetFileStream(myFileInfo.Fileid, myFileInfo.Compress)), myFileInfo.FileType ?? "application/octet-stream");
                    }
                    else
                    {
                        var fnX = Settings.GetSetting(myFileInfo.FileType);
                        if (string.IsNullOrEmpty(fnX))
                        {
                            fnX = Settings.GetSetting("IMAGE_NOT_AVAILABLE");
                        }

                        if (fnX.Contains("cdn/"))
                        {
                            var filename = fnX.Substring(4).ToLower();
                            var fi = myFiles.GetFileInfo3(filename);
                            return File(myFiles.GetFile3(fi.Fileid), fi.FileType ?? "application/octet-stream");
                        }
                        else
                        {
                            return File(Server.MapPath(fnX), "image/jpeg");
                        }
                    }
                }
            }
            catch (Exception) { }

            return null;
        }

        [HttpGet]
        [MyOutputCache(Location = OutputCacheLocation.ServerAndClient)]
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
            catch (Exception) { }

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

        public JsonResult GeneralList()
        {
            //var listData = myFiles.GetGeneralList();
            //return Json(new { success = true, list = listData }, JsonRequestBehavior.AllowGet);

            if (IsAllowAccess(menuId, "download") != "1")
            {
                return null;
            }

            try
            {
                var UMUM = Settings.GetSetting("GENERAL_DOCID");

                IMongoQuery query = Query.Matches("DocType", BsonRegularExpression.Create(new Regex(UMUM, RegexOptions.IgnoreCase)));

                var item = myFiles._Document.Find(query).SetSortOrder(SortBy.Descending("UploadDate"));

                List<SearchResult> MyResult = new List<SearchResult>();

                int i = 0;

                foreach (var x in item)
                {
                    IMongoQuery query2 = Query.EQ("AutoNo", x.AutoNo);
                    var item2 = myFiles._Files.Find(query2);

                    foreach (var y in item2)
                    {
                        i++;
                        var z = new SearchResult();
                        z.AutoNo = x.AutoNo;
                        z.Title = x.Title;
                        z.Description = x.Description;
                        z.FileId = y.Fileid.ToString();
                        z.FileName = y.FileName;
                        z.UploadBy = y.UploadBy;
                        z.UploadDate = y.UploadDate;
                        z.Tag = x.Tag;
                        MyResult.Add(z);

                        if (i >= 16)
                        {
                            break;
                        }
                    }

                    if (i >= 16)
                    {
                        break;
                    }
                }

                return Json(new { success = true, List = MyResult }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception) { }

            return null;
        }

        private object CurrentListDepts(string allowAll)
        {
            var data = db.Database.SqlQuery<TreeDeptMenu>("exec [sp_role_department] '" + RoleId + "'").ToList();

            var data1 = (from q in data
                         where q.Level == 0
                         select q).ToList();

            TreeData root = new TreeData();
            root.Id = "0";

            foreach (var x in data1)
            {
                var status = x.Status || (allowAll == "*");

                if (status)
                {
                    var y = new TreeData();
                    y.Id = x.CodeId;
                    y.Name = x.Name;
                    y.value1 = status;
                    ListNavDept(ref y, data, x.CodeId, allowAll);
                    root.Children.Add(y);
                }
            }

            return root;
        }
        
        private object CurrentListDocs(string allowAll)
        {
            var data = db.Database.SqlQuery<TreeDeptMenu>("exec [sp_role_doctype] '" + RoleId + "'").ToList();

            var data1 = (from q in data
                         where q.Level == 0
                         select q).ToList();

            TreeData root = new TreeData();
            root.Id = "0";

            foreach (var x in data1)
            {
                var status = x.Status || (allowAll == "*");

                if (status)
                {
                    var y = new TreeData();
                    y.Id = x.CodeId;
                    y.Name = x.Name;
                    y.value1 = status;
                    root.Children.Add(y);
                }
            }

            return root;
        }

        private void ListNavDept(ref TreeData refMenu, List<TreeDeptMenu> menus, string ParentId, string allowAll)
        {
            var myMenus = (from q in menus
                           where q.ParentId == ParentId
                           select q).ToList();

            foreach (var x in myMenus)
            {
                var status = x.Status || (allowAll == "*");

                if (status)
                {
                    var y = new TreeData();
                    y.Id = x.CodeId;
                    y.Name = x.Name;
                    y.value1 = status;

                    ListNavDept(ref y, menus, x.CodeId, allowAll);
                    refMenu.Children.Add(y);
                }
            }
        }

        private bool IsContains(string a, string b)
        {
            var c = a.Split(',');
            var d = b.Split(',');
            bool result = false;

            foreach (var e in c)
            {
                foreach (var f in d)
                {
                    if (e == f)
                    {
                        result = true;
                        break;
                    }
                }
                if (result) break;
            }

            return result;
        }

        public JsonResult Search(string Id)
        {
            if (IsAllowAccess(menuId, "download") != "1")
            {
                return null;
            }

            try
            {
                var UMUM = Settings.GetSetting("GENERAL_DOCID");
                string allTags = "";

                IMongoQuery query = Query.Matches("Description2", BsonRegularExpression.Create(new Regex(Id, RegexOptions.IgnoreCase)));

                var item = myFiles._Document.Find(query).SetSortOrder(SortBy.Descending("UploadDate"));

                List<SearchResult> MyResult = new List<SearchResult>();

                var checkAccess = db.Database.SqlQuery<ComboList>("exec sp_role_mappingdoc '" + UserId + "'").FirstOrDefault();

                var allowDept = checkAccess.value;
                var allowDoc = checkAccess.text;

                var sessionDepts = Request["dept"];
                var sessionDocs = Request["docs"];

                if (sessionDepts != null)
                {
                    allowDept = sessionDepts;
                    allowDoc = sessionDocs;
                }

                int i = 0;

                foreach (var x in item)
                {
                    IMongoQuery query2 = Query.EQ("AutoNo", x.AutoNo);
                    var item2 = myFiles._Files.Find(query2);
                    allTags += x.Tag + " ";

                    // Authorization // Check Access
                    bool IsAllowDept = (allowDept == "*");
                    bool IsAllowDocT = (allowDoc == "*");
                    bool IsPublic = IsContains(x.DocType.ToLower(), UMUM.ToLower());
                    bool Private = x.Private;
                    bool IsPrivate = true;
                    
                    if (!IsAllowDept && !string.IsNullOrWhiteSpace(allowDept))
                    {
                        IsAllowDept = IsContains(allowDept, x.Department);
                    }

                    if (!IsAllowDocT && !string.IsNullOrWhiteSpace(allowDoc))
                    {
                        IsAllowDocT = IsContains(allowDoc, x.DocType);
                    }

                    if (sessionDocs != null)
                    {
                        IsPublic = IsContains(sessionDocs.ToLower(), UMUM.ToLower());
                    }

                    if ((IsAllowDept && IsAllowDocT) || IsPublic)
                    {
                        foreach (var y in item2)
                        {
                            i++;
                            var z = new SearchResult();
                            z.AutoNo = x.AutoNo;
                            z.Title = x.Title;
                            z.Description = x.Description;
                            z.FileId = y.Fileid.ToString();
                            z.FileName = y.FileName;
                            z.UploadBy = y.UploadBy;
                            z.UploadDate = y.UploadDate;
                            z.Tag = x.Tag;

                            if (Private == true)
                            {
                                IsPrivate = (UserId == y.UploadBy) ? true : false;
                            }

                            if (IsPrivate)
                            {
                                MyResult.Add(z);
                            }
                        }
                    }

                    if (i > 100)
                    {
                        break;
                    }
                }

                return Json(new { success = true, List = MyResult, Tags = allTags.TrimEnd(), Depts = CurrentListDepts(allowDept), Docs = CurrentListDocs(allowDoc) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception) { }

            return null;
        }
    }
}