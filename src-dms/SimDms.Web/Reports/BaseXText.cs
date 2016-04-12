using SimDms.Common;
using SimDms.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.Web.Reports
{
    public abstract class BaseXText
    {

        SysUser CurrentUser;

        private object[] setTextParameter, paramText;
        private string paramReport = "", printerLoc = "", fileLocation = "", reportID = "";
        private bool print = true, fullPage = true;

        //protected SimDms.Service.Models.DataContext ctx = new SimDms.Service.Models.DataContext(MyHelpers.GetConnString("DataContext"));
        // private DataSet dsSource = null;
        //private DataTable dt = null;

        protected BaseXText(string reportIDText, SysUser usr,  params object[] pparamText)
        {
            paramText = pparamText;
            reportID = reportIDText;
            CurrentUser = usr;


        }

        protected BaseXText(string reportIDText)//, DataSet ds)
        {
            //dsSource = ds;
            reportID = reportIDText;
        }
        protected abstract void SetDefaultParameter();
        protected abstract string Print();

    }


    public interface IXText
    {       
        string Print();
        void SetDefaultParameter(string paramReportText, string printerLocText, string fileLocationText, bool printText, bool fullPageText,string srparam);
        
    }

    public interface IRptProc
    {
        SysUser CurrentUser { get; set; }

        string CreateReport(string rptId, string sproc, string sparam, string printerloc, bool print, bool fullpage, object[] oparam, string paramReport);
    }
}