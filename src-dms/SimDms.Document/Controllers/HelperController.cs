using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Document.Models;
using System.Configuration;

namespace SimDms.Document.Controllers
{
    public class HelperController : BaseController
    {
        //
        // GET: /Helper/

        public ActionResult Index()
        {
            return View();
        }

        public static List<SysHelp> GetMenuHeader()
        {
            DataContext ctx = new DataContext();
            var listHeaderMenu = ctx.SysHelps.Where(m => m.MenuLevel == 0).OrderBy(m => m.MenuSeq).ToList();
            return listHeaderMenu;
        }

        public static List<SysHelp> GetMenuChild(int MenuHeader)
        {
            List<SysHelp> listChildMenu;
            if (MenuHeader != null)
            {
                DataContext ctx = new DataContext();
                listChildMenu = ctx.SysHelps.Where(m => m.MenuHeader == MenuHeader).OrderBy(m => m.MenuSeq).ToList();
            }
            else
            {
                listChildMenu = new List<SysHelp>();
            }
            return listChildMenu;
        }

        public static List<SysHelp> GetListOfModule(int MenuHeader, string SessionID, bool IsAuthenticated = false)
        {
            //Get List of Module
            DMSContext dtx = new DMSContext();
            DataContext ctx = new DataContext();

            var user = dtx.Database.SqlQuery<string>("select SessionUser from SysSession where sessionID = {0}", SessionID).FirstOrDefault();
            //SimDms Dealer Query
            var listModul = dtx.Database.SqlQuery<string>(@"select m.ModuleCaption from SysSession s 
                                                    join SysRoleUser r ON s.SessionUser = r.UserID 
                                                    JOIN SysRoleModule rm ON r.RoleID = rm.RoleID 
                                                    JOIN SysModule m ON m.ModuleId = rm.ModuleID 
                                                    WHERE s.SessionId ={0}", SessionID).ToList();
            //SimDmsWeb Query
//            var listModul = dtx.Database.SqlQuery<string>(@"select m.ModuleCaption from SysSession s 
//                                                            JOIN SysUser u on u.Username = s.SessionUser
//                                                            join SysRoleUser r ON u.UserId = r.UserId
//                                                            JOIN SysRoleModule rm ON r.RoleID = rm.RoleID
//                                                            JOIN SysModule m ON m.ModuleId = rm.ModuleID 
//                                                            WHERE CONVERT(VARCHAR(255), s.SessionId) = {0}", SessionID).ToList();
            //SimDmsDealer Query
            var role = dtx.Database.SqlQuery<string>("select RoleID from SysRoleUser where UserID ={0}", user).FirstOrDefault();
            
            //SimDms Query
//            var role = dtx.Database.SqlQuery<string>(@"SELECT b.RoleName FROM SysRoleUser a
//                                                    JOIN SysRole b ON a.RoleId = b.RoleId
//                                                    JOIN SysUser c ON a.UserId = c.UserId 
//                                                    where c.Username ={0}", user).FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(role))
            {
                if (!string.IsNullOrEmpty(role))
                {
                    var authorizeGroup = ConfigurationManager.AppSettings["AuthorizeGroup"].ToString();
                    if (authorizeGroup.IndexOf(role) != -1)
                        listModul = dtx.Database.SqlQuery<string>("select ModuleCaption from SysModule where ModuleID != 'dc'").ToList();
                }
            }
            List<SysHelp> listChildMenu;
            if (MenuHeader <= 1 && listModul.Count != 0)
            {
                listChildMenu = ctx.SysHelps.Where(m => m.MenuHeader == MenuHeader && listModul.Contains(m.MenuTitle)).OrderBy(m => m.MenuSeq).ToList();
            }
            else if (!string.IsNullOrEmpty(user) && listModul.Count != 0)
            {
                listChildMenu = ctx.SysHelps.Where(m => m.MenuHeader == MenuHeader).OrderBy(m => m.MenuSeq).ToList();
            }
            else if (IsAuthenticated)
            {
                listChildMenu = ctx.SysHelps.Where(m => m.MenuHeader == MenuHeader).OrderBy(m => m.MenuSeq).ToList();
            }
            else
            {
                listChildMenu = new List<SysHelp>();
            }
            return listChildMenu;
        }

        public static bool IsAuthorized(string SessionID)
        {
            bool returnVal = false;
            DMSContext dtx = new DMSContext();
            var authorizeGroup = ConfigurationManager.AppSettings["AuthorizeGroup"].ToString();
            //SimDMSDealer Query
            var group = dtx.Database.SqlQuery<string>("SELECT RoleID from SysRoleUser r " +
                "join SysSession s on r.UserID = s.SessionUser " +
                "WHERE s.SessionID = {0} ", SessionID).FirstOrDefault();

            //SIMDMSWeb Query
//            var group = dtx.Database.SqlQuery<string>(@"SELECT a.RoleName from SysRole a
//                                                        JOIN SysRoleUser b ON a.RoleId = b.RoleId
//                                                        JOIN SysUser c on c.UserId = b.UserId
//                                                        JOIN SysSession d ON c.Username = d.SessionUser
//                                                        WHERE d.SessionId = {0} ", SessionID).FirstOrDefault();

            if (string.IsNullOrWhiteSpace(group))
            {
                returnVal = false;
            }
            else if (authorizeGroup.ToString().ToLower().IndexOf(group) != -1)
            {
                returnVal = true;
            }
            return returnVal;
        }

    }
}
