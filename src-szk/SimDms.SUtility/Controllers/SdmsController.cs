using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.SUtility.Controllers
{
    public class SdmsController : BaseController
    {
        public string DataList()
        {
            return HtmlRender("dms/datalist.js");
        }

        public string LastUpd()
        {
            return HtmlRender("dms/lastupd.js");
        }

        public string DataProc()
        {
            return HtmlRender("dms/dataproc.js");
        }

        public string InqSdms()
        {
            return HtmlRender("inq/sdms.js");
        }

        public string DlrInfo()
        {
            return HtmlRender("inq/dlrinfo.js");
        }

        public string SchedLog()
        {
            return HtmlRender("inq/schedlog.js");
        }

        public string CollData()
        {
            return HtmlRender("inq/colldata.js");
        }

        public string CollDataSum()
        {
            return HtmlRender("inq/colldatasum.js");
        }

        public string Users()
        {
            return HtmlRender("useraccess/user.js");
        }

        public string sqlpub()
        {
            return HtmlRender("useraccess/sql.js");
        }

        public string Modules()
        {
            return HtmlRender("useraccess/module.js");
        }

        public string Menus()
        {
            return HtmlRender("useraccess/menu.js");
        }

        public string Roles()
        {
            return HtmlRender("useraccess/role.js");
        }

        public string RoleMenus()
        {
            return HtmlRender("useraccess/rolemenu.js");
        }

        public string RoleModules()
        {
            return HtmlRender("useraccess/rolemodule.js");
        }

        public string DmsUpload()
        {
            return HtmlRender("dcs/inq-upload-file.js");
        }

        public string DmsDownload()
        {
            return HtmlRender("dcs/inq-download-file.js");
        }


        public string LastTransDate()
        {
            return HtmlRender("inq/lasttransdate.js");
        }


        public string CustomerDealerInfo()
        {
            return HtmlRender("inq/CustomerDealerInfo.js");
        }

        public string FormMonitoring()
        {
            return HtmlRender("monitoring/formmonitoring.js");
        }

        public string Calender()
        {
            return HtmlRender("useraccess/calender.js");
        }

        public string WrfReport()
        {
            return HtmlRender("monitoring/wrfreport.js");
        }
    }
}
