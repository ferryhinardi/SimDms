using SimDms.General.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GeLang;
using System.Data.SqlClient;
using TracerX;
using System.Data;
using System.Web.Script.Serialization;
using SimDms.Common.Models;
using SimDms.Common;

namespace SimDms.General.Controllers.Api
{
    public class GridController : BaseController
    {
        public JsonResult Users()
        {
            string userID = Request["filterUserId"] ?? "";
            string name = Request["filterFullName"] ?? "";
            string email = Request["filterEmail"] ?? "";

            var queryable = ctx.SysUserViews.AsQueryable();

            if (!string.IsNullOrWhiteSpace(userID)) { queryable = queryable.Where(x => x.UserId.Contains(userID)); }
            if (!string.IsNullOrWhiteSpace(name)) { queryable = queryable.Where(x => x.FullName.Contains(name)); }
            if (!string.IsNullOrWhiteSpace(email)) { queryable = queryable.Where(x => x.Email.Contains(email)); }

            return Json(queryable);
        }

        public JsonResult Roles()
        {
            string filterRoleName = Request["filterRoleName"] ?? "";
            string filterRoleId = Request["filterRoleId"] ?? "";

            var queryable = ctx.SysRoles.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filterRoleId)) { queryable = queryable.Where(x => x.RoleId.Contains(filterRoleId)); }
            if (!string.IsNullOrWhiteSpace(filterRoleName)) { queryable = queryable.Where(x => x.RoleName.Contains(filterRoleName)); }

            return Json(queryable);
        }

        public JsonResult Menus()
        {
            string roleID = Request["RoleID"] ?? "";
            string header = Request["ModuleId"] ?? "";
            string menuid = Request["SubModuleId"] ?? "";
            string submenu = Request["SubSubModuleId"] ?? ""; 
            string menuID = Request["filterMenuID"] ?? "";
            string menuCaption = Request["filterMenuCaption"] ?? "";
            string gridType = Request["gridType"] ?? "";
            int menuIndex = 0;
            int menuLevel = 0;

            try
            {
                menuIndex = Convert.ToInt32(Request["filterMenuIndex"] ?? "0");
            }
            catch (Exception) { }
            try
            {
                menuLevel = Convert.ToInt32(Request["filterMenuLevel"] ?? "0");
            }
            catch (Exception) { }

            List<string> assignedMenus = ctx.SysRoleMenus.Where(x => x.RoleId == roleID).Select(x => x.MenuId).ToList();
            var queryable = ctx.SysMenus.Where(x => assignedMenus.Contains(x.MenuId) == false);

            List<string> headerMenus = ctx.SysMenus.Where(x => x.MenuHeader == header).Select(x => x.MenuId).ToList();

            if (menuid == "" && submenu == "" && header != "")
            {
                queryable = ctx.SysMenus.Where(x => assignedMenus.Contains(x.MenuId) == false && headerMenus.Contains(x.MenuHeader)).OrderBy(x => x.MenuIndex);
            }

            if (submenu == "" && menuid != "" && header != "")
            {
                queryable = ctx.SysMenus.Where(x => assignedMenus.Contains(x.MenuId) == false && (x.MenuId == menuid || x.MenuHeader == menuid)).OrderBy(x => x.MenuIndex);
            }

            if (submenu != "" && menuid != "" && header != "")
            {
                queryable = ctx.SysMenus.Where(x => assignedMenus.Contains(x.MenuId) == false && (x.MenuId == submenu || x.MenuHeader == submenu)).OrderBy(x => x.MenuIndex);
            }

            if (menuLevel > 0)
            {
                queryable = queryable.Where(x => x.MenuLevel == menuLevel);
            }

            if (menuIndex > 0)
            {
                queryable = queryable.Where(x => x.MenuIndex == menuIndex);
            }

            if (!string.IsNullOrWhiteSpace(menuID))
            {
                queryable = queryable.Where(x => x.MenuId.Contains(menuID));
            }

            if (!string.IsNullOrWhiteSpace(menuCaption))
            {
                queryable = queryable.Where(x => x.MenuCaption.Contains(menuCaption));
            }

            if (gridType == "local")
            {
                return Json(queryable);
            }

            return Json(queryable.KGrid());
        }

        public JsonResult Modules()
        {
            string roleID = Request["RoleID"] ?? "";
            string moduleID = Request["filterModuleID"] ?? "";
            string moduleCaption = Request["filterModuleCaption"] ?? "";
            string gridType = Request["gridType"] ?? "";
            int moduleIndex = 0;

            try
            {
                moduleIndex = Convert.ToInt32(Request["filterModuleIndex"] ?? "0");
            }
            catch (Exception) { }

            List<string> assignedModules = ctx.SysRoleModules.Where(x => x.RoleID == roleID).Select(x => x.ModuleID).ToList();
            var queryable = ctx.SysModuleViews.Where(x => assignedModules.Contains(x.ModuleID) == false);

            if (!string.IsNullOrWhiteSpace(moduleID))
            {
                queryable = queryable.Where(x => x.ModuleID.Contains(moduleID));
            }

            if (!string.IsNullOrWhiteSpace(moduleCaption))
            {
                queryable = queryable.Where(x => x.ModuleCaption.Contains(moduleCaption));
            }

            if (moduleIndex > 0)
            {
                queryable = queryable.Where(x => x.ModuleIndex == moduleIndex);
            }

            if (gridType == "local")
            {
                return Json(queryable);
            }

            return Json(queryable.KGrid());
        }

        public JsonResult RoleModules()
        {
            string roleID = Request["RoleId"] ?? "";
            List<string> moduleIds = ctx.SysRoleModules.Where(p => p.RoleID == roleID).Select(x => x.ModuleID).ToList();

            var queryable = ctx.SysModuleViews.Where(x => moduleIds.Contains(x.ModuleID));

            return Json(queryable);
        }

        public JsonResult RoleMenus()
        {
            string roleid = Request["RoleId"] ?? "";
            string header = Request["ModuleId"] ?? "";
            string menuid = Request["SubModuleId"] ?? "";
            string lvl2 = Request["SubSubModuleId"] ?? ""; 

            List<string> menuids = ctx.SysRoleMenus.Where(p => p.RoleId == roleid).Select(x => x.MenuId).ToList();
            var queryable = ctx.SysMenus.Where(x => menuids.Contains(x.MenuId)).OrderBy(x =>x.MenuIndex);

            if (menuid == "" && header != "" && lvl2 =="")
            {
                queryable = ctx.SysMenus.Where(x => menuids.Contains(x.MenuId) && x.MenuHeader == header).OrderBy(x => x.MenuIndex);
            }
            if (menuid != "" && header != "" && lvl2 == "")
            {
                queryable = ctx.SysMenus.Where(x => menuids.Contains(x.MenuId) && (x.MenuId == menuid || x.MenuHeader == menuid)).OrderBy(x => x.MenuIndex).OrderBy(x => x.MenuLevel);
            }
            if (menuid != "" && lvl2 != "")
            {
                queryable = ctx.SysMenus.Where(x => menuids.Contains(x.MenuId) && (x.MenuId == lvl2 || x.MenuHeader == lvl2)).OrderBy(x => x.MenuIndex).OrderBy(x => x.MenuLevel);
            }
            return Json(queryable);
        }

        public JsonResult CustomerProfitCenters()
        {
            string customerCode = Request["CustomerCode"] ?? "";

            var data = ctx.Database.SqlQuery<CustomerProfitCenterModel>("exec uspfn_SelectCustomerProfitCenter @CompanyCode=@p0, @BranchCode=@p1, @CustomerCode=@p2", CompanyCode, BranchCode, customerCode);

            return Json(data);
        }

        public JsonResult CustomerDiscounts()
        {
            string customerCode = Request["CustomerCode"] ?? "";
            var data = ctx.Database.SqlQuery<CustomerDiscountModel>("exec uspfn_SelectCustomerDiscount @CompanyCode=@p0, @BranchCode=@p1, @CustomerCode=@p2", CompanyCode, BranchCode, customerCode);
        
            return Json(data);
        }

        public JsonResult CustomerBanks()
        {
            string customerCode = Request["CustomerCode"] ?? "";

            var data = ctx.Database.SqlQuery<CustomerBankModel>("exec uspfn_SelectCustomerBank @CompanyCode=@p0, @CustomerCode=@p1", CompanyCode, customerCode);

            return Json(data);
        }

        public JsonResult InquiryCustomers(bool AllowPeriod, DateTime StartDate, DateTime EndDate, string Branch)
        {

            try
            {
                string orderBy = string.Empty;
                string param = string.Empty;
                string docDate = string.Empty;
                string dtFirstDate, dtLastDate;
                string flag1 = AllowPeriod ? "1" : "0";

                dtFirstDate = StartDate.ToString("yyyy-MM-dd");
                dtLastDate = EndDate.ToString("yyyy-MM-dd");
                                

                SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
                cmd.CommandText = "usprpt_QueryCustomerDealer2 '" + flag1 + "','" + dtFirstDate + "','" + dtLastDate + "','','','" + Branch + "'";

                MyLogger.Log.Info("Inquiry Customers: EXEC " + cmd.CommandText);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet dt = new DataSet();
                da.Fill(dt);
                var records = dt.Tables[0];
                var rows = records.Rows.Count;

                return Json(new { success = true, data = records, total = rows }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

        }


        public string RolePermission()
        {
            string roleid = Request["RoleId"] ?? "USER";
            string callback = Request["callback"] ?? "";

            var queryable = ctx.Database.SqlQuery<SysRoleMenuPermission>("exec uspfn_sysgetpermission '" + roleid + "'").ToList();

            JavaScriptSerializer ser = new JavaScriptSerializer();
            return callback + string.Format("({0})", ser.Serialize(queryable));
            //return Json(queryable, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ZipCodes()
        {
            string sql = string.Format(@"EXEC uspfn_gnMstZipCodeBrowse_Web '{0}', '{1}', '{2}'",
                CompanyCode, Helpers.GetDynamicFilter(Request), 500);

            var records = ctx.Database.SqlQuery<GnMstZipCodeView>(sql).AsQueryable();
            return Json(records.toKG(ApplyFilterKendoGrid.False));
        }
    }
}
