using SimDms.SUtility.Models;
using SimDms.SUtility.Models.Others;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;

namespace SimDms.SUtility.Controllers.Api
{
    public class UserController : BaseController
    {
        [HttpPost]
        public JsonResult Default()
        {
            var defaultRole = ctx.SysRoles.Where(x => x.RoleName.ToLower().Contains("admin") == true).Select(x => x.RoleId).FirstOrDefault();

            return Json(new
            {
                IsApproved = true,
                RoleId = defaultRole
            });
        }

        public JsonResult Save(SysUserView model)
        {
            ResultModel result = InitializeResult();

            var user = ctx.SysUsers.Where(x => x.UserId == model.UserId).OrderBy(x => x.Username).FirstOrDefault();

            if (user == null)
            {
                user = new SysUser();
                user.UserId = Guid.NewGuid();
                //user.Password = FormsAuthentication.HashPasswordForStoringInConfigFile("123", "md5");
                user.Password = Crypto.HashPassword("SimDmsSuzuki");
                ctx.SysUsers.Add(user);
            }
            user.Username = model.Username;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email ?? "";
            user.IsApproved = model.IsApproved;
            user.DealerCode = model.DealerCode ?? "";
            user.OutletCode = model.OutletCode ?? "";
            
            var role = ctx.SysRoleUsers.Where(x => x.UserId == user.UserId).FirstOrDefault();
            if (role == null)
            {
                role = new SysRoleUser();
                role.UserId = user.UserId;
                ctx.SysRoleUsers.Add(role);
            }
            role.RoleId = model.RoleId;

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

        public JsonResult Delete(SysUser model)
        {
            ResultModel result = InitializeResult();

            var users = ctx.SysUsers.Where(x => x.UserId == model.UserId).OrderBy(x => x.Username);
            if (users != null && users.Count() > 0)
            {
                foreach (var item in users)
                {
                    ctx.SysUsers.Remove(item);
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

        public JsonResult Reset(SysUserView model)
        {
            ResultModel result = InitializeResult();

            var user = ctx.SysUsers.Where(x => x.UserId == model.UserId).OrderBy(x => x.Username).FirstOrDefault();
            
            user.Password = Crypto.HashPassword("123456");
            user.IsLockedOut = false;
            user.PasswordFailuresSinceLastSuccess = 0;
 
            try
            {
                ctx.SaveChanges();
                result.status = true;
                result.message = "User's data has been saved into database.";
            }
            catch (Exception ex)
            {

                result.message = "Sorry, user's data cannot be saved into database." + ex.Message;
            }

            return Json(result);
        }

        public JsonResult DealerList()
        {
            var list = from a in ctx.DealerInfos
                       select new
                       {
                           value = a.DealerCode,
                           text = a.DealerCode + " - " + a.DealerName
                       };
            return Json(list);
        }

        public JsonResult OutletList(string dealerCode)
        {
            var list = from a in ctx.OutletInfos
                       where a.CompanyCode == dealerCode
                       select new
                       {
                           value = a.BranchCode,
                           text = a.BranchCode + " - " + a.BranchName
                       };
            return Json(list);
        }

        public JsonResult GenerateUser()
        {
            DataTable dt = new DataTable();

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "Select UserName, FirstName, LastName, RoleId, RoleName, IsApprovedDescription from SysUserView order by RoleId,Username";
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 3600;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            return GenerateReportXls(dt, "User", "User");
        }
    }
}
