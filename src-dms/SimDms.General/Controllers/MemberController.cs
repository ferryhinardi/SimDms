using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.General.Controllers
{
    public class MemberController : BaseController
    {
        public string User()
        {
            return HtmlRender("member/user.js");
        }

        public string Role()
        {
            return HtmlRender("member/role.js");
        }

        public string Module()
        {
            return HtmlRender("member/module.js");
        }

        public string RoleModule()
        {
            return HtmlRender("member/rolemodule.js");
        }

        public string Menu()
        {
            return HtmlRender("member/menu.js");
        }

        public string RoleMenu()
        {
            return HtmlRender("member/rolemenu.js");
        }

        public string RoleMenu2()
        {
            return HtmlRender("member/rolemenu2.js");
        }

        public string AutoNoCustomer()
        {
            return HtmlRender("member/autonocustomer.js");
        }

        public string AutoNoSupplier()
        {
            return HtmlRender("member/autonosupplier.js");
        }

        public string QueryAnalizer()
        {
            return HtmlRender("member/queryanalizer.js");
        }

        public string BackupDatabase()
        {
            return HtmlRender("member/backupdatabase.js");
        }

        public string BackupBranchdata()
        {
            return HtmlRender("member/backupbranchdata.js");
        }

        public string AdminAudit() 
        {
            return HtmlRender("member/adminaudit.js");
        }

        public string LanguageSetup()
        {
            return HtmlRender("member/languagesetup.js");
        }

        public string UploadBranchdata()
        {
            return HtmlRender("member/uploadbranchdata.js");
        }

        public string PrinterSetup()
        {
            return HtmlRender("member/printersetup.js");
        }
    }
}
