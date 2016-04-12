using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.General.Controllers.Api
{
    public class ComboController : BaseController
    {
        public JsonResult LoadLookup(string CodeID)
        {
            var user = CurrentUser;
            var listLkp = ctx.LookUpDtls.Where(m => m.CompanyCode == user.CompanyCode && m.CodeID == CodeID).OrderBy(x => x.LookUpValue).Select(m => new { value = m.LookUpValue, text = m.LookUpValueName }).ToList();
            return Json(listLkp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Users()
        {
            var list = ctx.SysUsers.Select(p => new { value = p.UserId, text = p.FullName }).ToList();
            return Json(list);
        }

        public JsonResult Roles()
        {
            var list = ctx.SysRoles.Where(x => x.RoleName.Trim().ToLower() != "admin").Select(p => new { value = p.RoleId, text = p.RoleName }).ToList();

            string currentRole = CurrentRole();

            if (currentRole.Trim().ToLower() == "admin")
            {
                list = ctx.SysRoles.Select(p => new { value = p.RoleId, text = p.RoleName }).ToList();
            }
            else
            {
                list = ctx.SysRoles.Where(x => x.RoleName.Trim().ToLower() != "admin").Select(p => new { value = p.RoleId, text = p.RoleName }).ToList();
            }

            return Json(list);
        }

        public JsonResult Modules() 
        {
            var list = ctx.SysModules.Where(x => x.IsPublish != false).OrderBy(x => x.ModuleIndex).Select(p => new { value = p.ModuleID, text = p.ModuleCaption }).ToList();

            string currentRole = CurrentRole();

            //if (currentRole.Trim().ToLower() == "admin")
            //{
            //    list = ctx.SysRoles.Select(p => new { value = p.RoleId, text = p.RoleName }).ToList();
            //}
            //else
            //{
            //    list = ctx.SysRoles.Where(x => x.RoleName.Trim().ToLower() != "admin").Select(p => new { value = p.RoleId, text = p.RoleName }).ToList();
            //}

            return Json(list);
        }

        public JsonResult SubModule() 
        {
            string ModuleId = Request["id"] ?? "";
            var list = ctx.SysMenus.Where(x => x.MenuHeader == ModuleId).Select(p => new { value = p.MenuId, text = p.MenuCaption }).ToList();

            string currentRole = CurrentRole();

            //if (currentRole.Trim().ToLower() == "admin")
            //{
            //    list = ctx.SysRoles.Select(p => new { value = p.RoleId, text = p.RoleName }).ToList();
            //}
            //else
            //{
            //    list = ctx.SysRoles.Where(x => x.RoleName.Trim().ToLower() != "admin").Select(p => new { value = p.RoleId, text = p.RoleName }).ToList();
            //}

            return Json(list);
        }

        public JsonResult SubSubModule() 
        {
            string ModuleId = Request["id"] ?? "";
            var list = ctx.SysMenus.Where(x => x.MenuHeader == ModuleId).Select(p => new { value = p.MenuId, text = p.MenuCaption }).ToList();

            string currentRole = CurrentRole();

            //if (currentRole.Trim().ToLower() == "admin")
            //{
            //    list = ctx.SysRoles.Select(p => new { value = p.RoleId, text = p.RoleName }).ToList();
            //}
            //else
            //{
            //    list = ctx.SysRoles.Where(x => x.RoleName.Trim().ToLower() != "admin").Select(p => new { value = p.RoleId, text = p.RoleName }).ToList();
            //}

            return Json(list);
        }

        public JsonResult Organizations()
        {
            var qry = ctx.OrganizationHdrs.OrderBy(p => p.CompanyName).AsQueryable();
            //var dlr = DealerType;
            //if (!string.IsNullOrWhiteSpace(dlr)) { qry = qry.Where(p => p.DealerType == dlr); }
            //return Json(qry.Select(p => new { value = p.CompanyCode, text = p.CompanyCode + " - " + p.CompanyName.ToUpper(), p.CompanyName }).OrderBy(p => p.CompanyName));
            return Json(qry.Select(p => new { value = p.CompanyCode, text = p.CompanyName.ToUpper(), p.CompanyName }).OrderBy(p => p.CompanyName));
        }

        public JsonResult Branchs()
        {
            var list = ctx.CoProfiles.OrderBy(p => p.BranchCode).Select(p => new { value = p.BranchCode, text = p.CompanyName }).ToList();
            return Json(list);
        }

        public JsonResult TypeOfGoods()
        {
            var list = ctx.LookUpDtls.Where(p => p.CodeID == "TPGO").OrderBy(p => p.SeqNo)
                            .Select(p => new { value = p.LookUpValue, text = p.LookUpValueName }).ToList();
            return Json(list);
        }

        public JsonResult ProfitCenters()
        {
            var list = ctx.LookUpDtls.Where(p => p.CodeID == "PFCN").OrderBy(p => p.SeqNo)
                            .Select(p => new { value = p.LookUpValue, text = p.LookUpValueName }).ToList();
            return Json(list);
        }

        public JsonResult Menus()
        {
            int level = 0;
            try
            {
                level = Convert.ToInt32(Request["currentMenuLevel"] ?? "0");
            }
            catch (Exception) { }
            var list = ctx.SysMenus.AsQueryable();

            if (level > 1)
            {
                list = list.Where(x => x.MenuLevel == (level-1));           
            }
            else if (level == 1)
            {
                var listModule = ctx.SysModules.AsQueryable();
                var dataModule = listModule.Select(x => new { text = x.ModuleCaption + " - " + x.ModuleID, value = x.ModuleID }).OrderBy(x => x.text);
                return Json(dataModule);
            }

            var data = list.Select(x => new { text = x.MenuCaption + " - " + x.MenuId, value = x.MenuId }).OrderBy(x => x.text);

            return Json(data);
        }

        public JsonResult Branches()
        {
            string currentBranch = CurrentUser.BranchCode;
            var data = ctx.GnMstOrganizationDtls.Where(x => x.BranchCode != currentBranch).OrderBy(x => x.BranchCode).Select(x => new
            {
                text = x.BranchCode + " - " + x.BranchName,
                value = x.BranchCode
            });

            return Json(data);
        }

        public JsonResult CustomerGenders()
        {
            var data = (from x in ctx.LookUpDtls
                        where
                        x.CodeID.Equals("GNDR") == true
                        orderby x.LookUpValueName ascending
                        select new
                        {
                            text = x.LookUpValueName.ToUpper(),
                            value = x.LookUpValue == "L" ? "M" : "F"
                        }).ToList();

            return Json(data);
        }

        public JsonResult CustomerTypes()
        {
            var data = (from x in ctx.LookUpDtls
                        where
                        x.CodeID.Equals("CSTP") == true
                        orderby x.LookUpValueName ascending
                        select new
                        {
                            text = x.LookUpValueName.ToUpper(),
                            value = x.LookUpValue
                        }).ToList();

            return Json(data);
        }

        public JsonResult CustomerStatuses()
        {
            var data = (from x in ctx.LookUpDtls
                        where
                        x.CodeID.Equals("STPR") == true
                        &&
                        x.SeqNo < 3
                        orderby x.LookUpValueName ascending
                        select new
                        {
                            text = x.LookUpValueName.ToUpper(),
                            value = x.LookUpValue
                        }).ToList();

            return Json(data);
        }

        public JsonResult PaymentTypes()
        {
            var data = (from x in ctx.LookUpDtls
                        where
                        x.CodeID.Equals("PYBY") == true
                        //&&
                        //x.SeqNo < 3
                        orderby x.LookUpValueName ascending
                        select new
                        {
                            text = x.LookUpValueName.ToUpper(),
                            value = x.LookUpValue
                        }).ToList();

            return Json(data);
        }

        public JsonResult TOP()
        {        
            bool IsShowedAll = false;
            try
            {
                IsShowedAll = Convert.ToBoolean(Request["IsShowedAll"] ?? "false");
            }
            catch (Exception) { }

            if (IsShowedAll)
            {
                var data = (from x in ctx.LookUpDtls
                            where
                            x.CodeID.Equals("TOPC") == true
                            orderby x.LookUpValueName ascending
                            select new
                            {
                                text = x.LookUpValueName.ToUpper(),
                                value = x.LookUpValue,
                                x.SeqNo
                            }).OrderBy(x => x.text).ToList();

                return Json(data);
            }
            else
            {
                var data = (from x in ctx.LookUpDtls
                            where
                            x.CodeID.Equals("TOPC") == true
                            &&
                            x.ParaValue.Equals("0") == true
                            orderby x.LookUpValueName ascending
                            select new
                            {
                                text = x.LookUpValueName.ToUpper(),
                                value = x.LookUpValue,
                                x.SeqNo
                            }).OrderBy(x => x.text).ToList();

                return Json(data);
            }
        }
    }
}
                      

                     