using System.Linq;
using System.Web.Mvc;
using System.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using eXpressAPI.Models;

namespace eXpressAPI.Controllers
{
    [Authorize]
    [RoutePrefix("appdata/list")]
    [Route("{action=index}")]
    public class ComboController : DefaultController
    {
        public string Index()
        {
            return "List";
        }
        
        public JsonResult Menus()
        {
            string callback = Request["callback"];
            string sfilter = Request["filter[filters][0][value]"] ?? "";
            var data = db.Menus.AsQueryable();
            if (!string.IsNullOrEmpty(sfilter))
            {
                data = data.Where(x => x.Name.Contains(sfilter));
            }

            return Json(data.Select(x => new { value = x.MenuId, text = x.MenuId + " - " + x.Name }).ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult department()
        {
            string callback = Request["callback"];
            string sfilter = Request["filter[filters][0][value]"] ?? "";
            var data = db.Departements.AsQueryable();
            if (!string.IsNullOrEmpty(sfilter))
            {
                data = data.Where(x => x.Name.Contains(sfilter));
            }

            return Json(data.Select(x => new { value = x.Code, text = x.Code + " - " + x.Name }).ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Roles()
        {
            return Json(db.Roles.Select(x => new { value = x.RoleId, text = x.Name, sales = x.IsSalary }),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult Companies()
        {
            return Json(ListCompany(), JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult Kategori()
        {
            string sfilter = Request["filter[filters][0][value]"] ?? "";
            var data = db.Categories.Where(x => x.CompanyCode == CompanyId).AsQueryable();
                       
            if (!string.IsNullOrEmpty(sfilter))
            {
                data = data.Where(x => x.Name.Contains(sfilter));
            }

            return Json(data.Select(o => new { value = o.Code, text = o.Name }).ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Kelas()
        {
            string sfilter = Request["filter[filters][0][value]"] ?? "";
            var data = db.Classifications.Where(x => x.CompanyCode == CompanyId).AsQueryable();
                       
            if (!string.IsNullOrEmpty(sfilter))
            {
                data = data.Where(x => x.Name.Contains(sfilter));
            }

            return Json(data.Select(o => new { value = o.Code, text = o.Name }).ToList(), JsonRequestBehavior.AllowGet);
        }      
       
        public JsonResult UOM()
        {
            string sfilter = Request["filter[filters][0][value]"] ?? "";
            var data = db.UOM.Where(x => x.CompanyCode == CompanyId).AsQueryable();
                       
            if (!string.IsNullOrEmpty(sfilter))
            {
                data = data.Where(x => x.Name.Contains(sfilter));
            }

            return Json(data.Select(o => new { value = o.Code, text = o.Name }).ToList(), JsonRequestBehavior.AllowGet);
        }        
        
        public JsonResult MFG()
        {
            string sfilter = Request["filter[filters][0][value]"] ?? "";
            var data = db.Mfgs.Where(x => x.CompanyCode == CompanyId).AsQueryable();
                       
            if (!string.IsNullOrEmpty(sfilter))
            {
                data = data.Where(x => x.Name.Contains(sfilter));
            }

            return Json(data.Select(o => new { value = o.Code, text = o.Name }).ToList(), JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult KategoriSpec()
        {
            string sfilter = Request["filter[filters][0][value]"] ?? "";
            var data = db.SpecCategories.Where(x => x.CompanyCode == CompanyId ).AsQueryable();
                       
            if (!string.IsNullOrEmpty(sfilter))
            {
                data = data.Where(x => x.Name.Contains(sfilter));
            }

            return Json(data.Select(o => new { value = o.Code, text = o.Name }).ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Currency()
        {
            var list = db.Currencies.Where(x => x.CompanyCode == CompanyId).
                       Select(o => new { value = o.Code, text = o.Name }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Tipe()
        {
            string sfilter = Request["filter[filters][0][value]"] ?? "";
            var data = db.ItemTypes.Where(x => x.CompanyCode == CompanyId).AsQueryable();
                       
            if (!string.IsNullOrEmpty(sfilter))
            {
                data = data.Where(x => (x.Name.Contains(sfilter) || x.Code.Contains(sfilter)) );
            }

            return Json(data.Select(o => new { value = o.Code, text = o.Name }).ToList(), JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult ContactByCustomer()
        {
            string id = Request["filter[filters][0][value]"] ?? "";
            string sfilter = Request["filter[filters][1][value]"] ?? "";

            var data = db.Contacts.Where(x => x.CompanyCode == CompanyId && x.Code == id && x.ContactType == "CONTACT").AsQueryable();

            if (!string.IsNullOrEmpty(sfilter))
            {
                data = data.Where(x => (x.Name.Contains(sfilter) || x.Code.Contains(sfilter)));
            }

            return Json(data.Select(o => new { value = o.ContactId, text = o.Name }).ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ContactPerson()
        {
            string sfilter = Request["filter[filters][0][value]"] ?? "";
            var data = db.Contacts.Where(x => x.CompanyCode == CompanyId && x.ContactType == "CONTACT").AsQueryable();

            if (!string.IsNullOrEmpty(sfilter))
            {
                data = data.Where(x => (x.Name.Contains(sfilter) || x.Code.Contains(sfilter)));
            }

            return Json(data.Select(o => new { value = o.ContactId, text = o.Name }).ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult SalesPerson()
        {
            string sfilter = Request["filter[filters][0][value]"] ?? "";
            var data = db.Contacts.Where(x => x.CompanyCode == CompanyId && x.ContactType == "SALES" ).AsQueryable();

            if (!string.IsNullOrEmpty(sfilter))
            {
                data = data.Where(x => (x.Name.Contains(sfilter) || x.Code.Contains(sfilter)));
            }

            return Json(data.Select(o => new { value = o.ContactId, text = o.Name }).ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GoogleUser()
        {
            string sfilter = Request["filter[filters][0][value]"] ?? "";
            string sql = "SELECT * FROM vw_googleuser";
            
            if (!string.IsNullOrEmpty(sfilter))
            {
                sql += " where name like '" + sfilter + "%'";
            }

            var ret = SqlQuery(sql);

            string json = "[]";

            if (ret.data != null)
            {
                json = JsonConvert.SerializeObject(ret.Table(), new DataSetConverter());
            }

            return Json(JsonConvert.DeserializeObject(json), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Projects()
        {
            string sfilter = Request["filter[filters][0][value]"] ?? "";
            string SQL = "select * from vw_projects where companycode='" + CompanyId + "' and status=1";

            if (!string.IsNullOrEmpty(sfilter))
            {
                SQL += " and (code like '" + sfilter + "%' or name like '" + sfilter + "%'";
            }

            var ret = SqlQuery(SQL);

            string json = "[]";

            if (ret.data != null)
            {
                json = JsonConvert.SerializeObject(ret.Table(), new DataSetConverter());
            }

            return Json(JsonConvert.DeserializeObject(json), JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult Item()
        {
            string sfilter = Request["filter[filters][0][value]"] ?? "";
            var data = db.Items.Where(x => x.CompanyCode == CompanyId).AsQueryable();           
            if (!string.IsNullOrEmpty(sfilter))
            {
                data = data.Where(x => (x.Name.Contains(sfilter) || x.Code.Contains(sfilter)) );
            }
            return Json(data.Select(o => new { value = o.Code, text = o.Name }).ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ItemFromSheet()
        {
            string query = Request["query"] ?? "";
            string typeId = Request["typeid"] ?? "";
            string sql = "EXEC sp_getItemByName '" + query + "','" + CompanyId + "','" + typeId + "'";
            var ret = SqlQuery(sql);
            DataTable dt = null;

            if (ret.Count > 0)
            {
                dt = ret.Table();
            }

            return Json(new { data = dt }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Customers()
        {
            string sfilter = Request["filter[filters][0][value]"] ?? "";
            var data = db.Customers.Where(x => x.CompanyCode == CompanyId).AsQueryable();

            if (!string.IsNullOrEmpty(sfilter))
            {
                data = data.Where(x => (x.Name.Contains(sfilter) || x.Code.Contains(sfilter)));
            }

            return Json(data.Select(o => new { value = o.Code, text = o.Name, address = o.Address, city = o.City, phone = o.PhoneNo }).ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ItemPriceReff()
        {
            string query = Request["query"] ?? "";

            string itemId = Request["itemid"] ?? "";
            string curr = Request["currency"] ?? "";

            string sql = "EXEC sp_getItemPriceReff '" + itemId + "','" + CompanyId + "','" + curr + "'";
            var ret = SqlQuery(sql);
            DataTable dt = null;

            if (ret.Count > 0)
            {
                dt = ret.Table();
            }

            return Json(dt, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetListAccessName()
        {
            string sql = "SELECT [AccessName]  FROM [dbo].[SM_AccessName] order by [AccessName]";
            var ret = SqlQuery(sql);
            DataTable dt = null;

            if (ret.Count > 0)
            {
                dt = ret.Table();
            }

            return Json(dt, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetListDepts()
        {
            string sql = string.Format("exec [sp_role_get_list] '{0}','DEPARTMENT'", RoleId);
            var ret = SqlQuery(sql);
            DataTable dt = null;

            if (ret.Count > 0)
            {
                dt = ret.Table();
            }

            return Json(dt, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetListDocTypes()
        {
            string sql = string.Format("exec [sp_role_get_list] '{0}','DOCTYPE'", RoleId);
            var ret = SqlQuery(sql);
            DataTable dt = null;

            if (ret.Count > 0)
            {
                dt = ret.Table();
            }

            return Json(dt, JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult CurrentListDocTypes()
        {
            string sql = string.Format("exec [sp_role_get_list] '{0}','DOCTYPE'", RoleId);
            var ret = SqlQuery(sql);
            DataTable dt = null;
            TreeData root = new TreeData();
            root.Id = "0";

            if (ret.Count > 0)
            {
                dt = ret.Table();
                for (int i=0; i< ret.Count; i++)
                {
                    var y = new TreeData();
                    var r = dt.Rows[i];
                    y.Id = r["value"].ToString();
                    y.Name = r["text"].ToString();
                    y.value1 = true;
                    root.Children.Add(y);
                }
            }

            return Json(root, JsonRequestBehavior.AllowGet);
        }
    }
}