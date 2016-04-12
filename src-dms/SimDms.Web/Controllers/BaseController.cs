using SimDms.Common;
using SimDms.Web.Models;
using SimDms.Web.Models.General;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TracerX;

namespace SimDms.Web.Controllers
{
    public class BaseController : Controller
    {
        protected LayoutContext ctx;

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            ctx = new LayoutContext(MyHelpers.GetConnString("LayoutContext"));            
        }

        protected string HtmlRender(string jsname)
        {
            return string.Format(@"<script src=""{0}{1}"" type=""text/javascript""><script>", Url.Content("~/assets/js/app/"), jsname);
        }

        protected string HtmlRender(string id, string jsname)
        {
            var jshtml = "";
            if (!string.IsNullOrWhiteSpace(jsname))
            {
                jshtml = string.Format(@"<script src=""{0}{1}"" type=""text/javascript""><script>",Url.Content("~/assets/js/app/"), jsname);
            }
            return string.Format(@"<div id=""{0}"" ></div>", id) + jshtml;
        }

        protected SysUserView CurrentUser
        {
            get
            {
                return ctx.SysUserViews.FirstOrDefault(a => a.UserId == User.Identity.Name);

                //return ctx.SysUserViews.Find(User.Identity.Name);
                //return ctx.SysUserViews.Where(x => x.UserId==User.Identity.Name).FirstOrDefault();

            }
        }

        protected string CompanyCode
        {
            get
            {
                //if (CurrentUser == null) return "";
                return CurrentUser.CompanyCode;
            }
        }

        protected string BranchCode
        {
            get
            {
                //if (CurrentUser == null) return "";
                return CurrentUser.BranchCode;
            }
        }

        protected string BranchName
        {
            get
            {
                //if (CurrentUser == null) return "";
                return CurrentUser.BranchName;
            }
        }

        protected string TypeOfGoods
        {
            get
            {
                //if (CurrentUser == null) return "";
                return CurrentUser.TypeOfGoods;
            }
        }

        protected string TypeOfGoodsName
        {
            get
            {
                //if (CurrentUser == null) return "";
                string s = "";
                var x = ctx.LookUpDtls.Find(CompanyCode, GnMstLookUpHdr.TypeOfGoods, CurrentUser.TypeOfGoods);
                if (x != null) s = x.LookUpValueName;
                return s;
            }
        }

        protected ResultModel InitializeResultModel()
        {
            return new ResultModel()
            {
                success = false,
                message = "",
                details = "",
                data = null
            };
        }

        protected object ObjectMapper(object ObjectFrom, object ObjectTo)
        {
            var listProperty = ObjectFrom.GetType().GetProperties();
            foreach (var prop in listProperty)
            {
                var PropObjB = (from obj in ObjectTo.GetType().GetProperties().ToList()
                                where obj.Name == prop.Name
                                select obj).FirstOrDefault();
                if (PropObjB != null)
                {
                    ObjectTo.GetType().GetProperty(prop.Name).SetValue(ObjectTo, prop.GetValue(ObjectFrom, null), null);
                }
            }
            return ObjectTo;
        }

        protected JsonResult Upload(HttpPostedFileBase file)
        {
            ResultModel result = this.InitializeResultModel();

            try
            {
                string uploadedFileName = "";
                string filePath = "";

                if (file.ContentLength > 0)
                {
                    uploadedFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    filePath = Path.Combine(Server.MapPath("~/uploads"), uploadedFileName);

                    file.SaveAs(filePath);
                }

                result.success = true;
                result.data = new
                {
                    status = true,
                    filePath = uploadedFileName
                };
            }
            catch (Exception)
            {
                result.success = false;
            }

            return Json(result);
        }

        protected int IndexOfChar(string str, char chr, int nthOccurence)
        {
            int indexOfChar = -1;
            char[] strChar = str.ToCharArray();
            int iterator = 0;
            int occurence = 0;
            bool isStopped = false;

            foreach (char c in str)
            {
                if (strChar[iterator] == chr && isStopped == false)
                {
                    occurence++;

                    if (nthOccurence == occurence)
                    {
                        indexOfChar = iterator;
                        isStopped = true;
                    }
                }
                iterator++;
            }

            return indexOfChar;
        }

        protected ResultModel InitializeResult()
        {
            return new ResultModel()
            {
                success = false,
                message = "",
                details = "",
                data  = null
            };
        }
    }
}
