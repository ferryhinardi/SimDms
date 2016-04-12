//using SimDms.General.Models;
using SimDms.General.Models.Others;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using SimDms.Common.Models;
//using System.Web.Helpers;

namespace SimDms.General.Controllers.Api
{
    public class UserController : BaseController
    {
        public JsonResult Default()
        {
            return Json(new
            {
                CompanyCode = CompanyCode,
                CompanyName = CompanyName,
                IsActive = true,
                IsChangeBranchCode = false
            });
        }

        public JsonResult Save(SysUserView model)
        {
            ResultModel result = InitializeResultModel();
            string profitCenterCode = Request["ProfitCenter"] ?? "";

            var sysUser = ctx.SysUsers.Where(x => x.UserId == model.UserId).FirstOrDefault();
            if (sysUser == null)
            {
                sysUser = new SysUser();
                sysUser.UserId = model.UserId;
                sysUser.Password = FormsAuthentication.HashPasswordForStoringInConfigFile("123", "md5");
                ctx.SysUsers.Add(sysUser);
            }

            var sysUserProfitCenter = ctx.SysUserProfitCenters.Where(x => x.UserId==model.UserId).FirstOrDefault();
            if (sysUserProfitCenter == null)
            {
                sysUserProfitCenter = new SysUserProfitCenter();
                sysUserProfitCenter.UserId = model.UserId;

                ctx.SysUserProfitCenters.Add(sysUserProfitCenter);
            }

            sysUserProfitCenter.ProfitCenter = profitCenterCode;


            sysUser.FullName = model.FullName;
            sysUser.Email = model.Email ?? "";
            sysUser.CompanyCode = model.CompanyCode ?? "";
            sysUser.BranchCode = model.BranchCode ?? "";
            sysUser.TypeOfGoods = model.TypeOfGoods ?? "";
            sysUser.IsActive = model.IsActive;

            var record2 = ctx.SysRoleUsers.Find(model.UserId);
            if (record2 == null)
            {
                record2 = new SysRoleUser();
                ctx.SysRoleUsers.Add(record2);
            }
            record2.RoleId = model.RoleId;
            record2.UserId = model.UserId;

            try
            {
                ctx.SaveChanges();
                result.status = true;
                result.message = "User's data has been saved into database.";
            }
            catch
            {
                result.message = "Sorry, user's data cannot be saved into database.";
            }

            return Json(result);
        }

        public JsonResult Reset(SysUserView model)
        {
            ResultModel result = InitializeResultModel();
            result.status = false;

            try
            {

                var user = ctx.SysUsers.Where(x => x.UserId == model.UserId).OrderBy(x => x.UserId).FirstOrDefault();

                if (user == null)
                {
                    result.message = "User's does not exists in database.";
                }
                else
                {
                    user.Password = FormsAuthentication.HashPasswordForStoringInConfigFile("123", "md5");
                    user.RequiredChange = true;
                    ctx.SaveChanges();
                    result.status = true;
                    result.message = "User password has been reset. New password is 123.";
                }
            }
            catch(Exception ex)
            {
                result.message = ex.InnerException.Message;
            }

            return Json(result);
        }

        public JsonResult Delete(SysUser model)
        {
            ResultModel result = InitializeResultModel();

            var users = ctx.SysUsers.Where(x => x.UserId==model.UserId); ;
            var roles = ctx.SysRoleUsers.Where(x => x.UserId==model.UserId);

            if (users != null)
            {
                foreach (var item in users)
                {
                    ctx.SysUsers.Remove(item);
                }
                
                foreach (var item in roles)
                {
                    ctx.SysRoleUsers.Remove(item);
                }

                try
                {
                    ctx.SaveChanges();
                    result.status = true;
                    result.message = "User's data has been deleted.";
                }
                catch
                {
                    result.status = false;
                    result.message = "Sorry, we cannot delete user's data.\nPlease, try again later!";
                }
            }

            return Json(result);
        }

        public JsonResult CurrentBranch()
        {
            string currentBranchCode = CurrentUser.BranchCode;
            string currentBranchName = ctx.GnMstOrganizationDtls.Where(x => x.BranchCode == currentBranchCode).Select(x => x.BranchName).FirstOrDefault();

            return Json(new { 
                CurrentBranchCode = currentBranchCode,
                CurrentBranchName = currentBranchName
            });
        }

        public JsonResult ChangeBranch()
        {
            ResultModel result = InitializeResultModel();
            string nextBranch = Request["NextBranch"] ?? "";

            var user = ctx.SysUsers.Find(CurrentUser.UserId);
            if (user != null)
            {
                user.BranchCode = nextBranch;
            }

            try
            {
                ctx.SaveChanges();

                result.status = true;
                result.message = "Branch has been changed.";
            }
            catch (Exception)
            {
                result.message = "Sorry, an error occured or your user doesn't have a privilige to change Branch.";
            }

            return Json(result);
        }      

        public JsonResult ChangeProfitCenter()
        {
            var pc = ctx.SysUserProfitCenters.Find(CurrentUser.UserId);
            string newpc=Request["NextProfitCenter"];

            if(ctx.LookUpDtls.Find(CompanyCode,"PFCN",newpc)==null)
                return Json(new { success = false, message = "invalid Profit Center Code" });
           
            if (pc == null)
            {
                pc = new SysUserProfitCenter() { UserId = CurrentUser.UserId };
            }
            pc.ProfitCenter = newpc;

            try
            {
                ctx.SaveChanges();
                return Json(new {success=true,message = "Profit Center has been changed."});
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            
        }


        public JsonResult ChangeTPGO()
        {
            var pc = ctx.SysUsers.Find(CurrentUser.UserId);
            string newpc = Request["NextTPGO"];

            if (ctx.LookUpDtls.Find(CompanyCode, "TPGO", newpc) == null)
                return Json(new { success = false, message = "Invalid Type Of Goods" });

            pc.TypeOfGoods = newpc;

            try
            {
                ctx.SaveChanges();
                return Json(new { success = true, message = "Type Of Goods has been changed." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

        }

    }
}
