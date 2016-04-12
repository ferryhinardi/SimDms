using SimDms.Web.Models;
using SimDms.Web.Models.General;
using SimDms.Web.Reports;
using SimDms.Web.Reports.Sales;
using SimDms.Web.Reports.Service;
using SimDms.Web.Reports.Sparepart;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;


namespace SimDms.Web.Controllers
{
    public class TextReportController : BaseController
    {
        
       
        
        public JsonResult PrintReport(string rpt, string par, string rparam,string pnm)
        {
            var usr = ctx.SysUsers.Find(CurrentUser.UserId);
            rparam = string.Join(",", usr.CompanyCode, usr.BranchCode, par);
            var stxt = new XServiceText(rpt, usr,rparam.Split(','));
            //var DataTable dtParameter = GnMstLookUpDtlBLL.SelectbyCodeIDForGenerateTextFile(user.CompanyCode);
            var para = ctx.Database.SqlQuery<string>(string.Format(@"select 
               b.ParaValue
            from 
                SysParameter a
                left join gnMstLookUpDtl b on
                    b.CodeID = a.ParamValue
            where 
                a.ParamID = 'DEFAULT_PRINTER_TYPE' AND
                b.CompanyCode = {0} AND
                b.CodeID = a.ParamValue  order by b.SeqNo",usr.CompanyCode)).ToList();

            string sploc = "";
            int ploc = 0;
            switch (ploc)
            {

                case 0: sploc = @"\\172.16.101.122\EpsonLQ-2180";
                    break;
                case 1: sploc = @"\\Batsa02-pc\epson lq-2090 batsa02";
                    break;
                default: sploc = "CutePDF Writer";
                    break;
            }

            sploc = @"\\172.16.101.122\EpsonLQ-2180";
            stxt.SetDefaultParameter(para[0] + para[1], sploc, "", true, true,rparam);            
          
            try
            {
                stxt.Print();
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = ex.Message });    
            }
            

            return Json(new { success = true, msg = sploc+" " });


        }

        public ActionResult DownloadReport(string rpt, string par, string rparam)
        {
            
            var usr = ctx.SysUsers.Find(CurrentUser.UserId);
            string spparam = string.Join(",", usr.CompanyCode, usr.BranchCode, par);

            var docno = spparam
                        .Split(',')
                        .Where(x => x.IndexOf('/') > 0)
                        .FirstOrDefault();
            string flnm = docno == null ? rpt : docno;

            string[] w163a =new string[]{"SvRpTrn001","SvRpTrn001PrePrinted"};
            string[] W136 = new string[] { "OmRpSalesTrn009A","OmRpSalesTrn009" };
       

            if(w163a.Contains(rpt))
            {
                flnm = "W163A-IM-" + flnm;
            }   
            else if(W136.Contains(rpt)){
                flnm = "W136-IM-" + flnm;
            }
            else
            {
                flnm = "W96-IM-" + flnm;
            }            
           
            
            IXText stxt;

            if (rpt.ToLower().Substring(0, 2) == "sv")
            {
                stxt = new XServiceText(rpt, usr, spparam.Split(','));
                stxt.SetDefaultParameter("", "", "", false, true, rparam);
            }
            else if (rpt.ToLower().Substring(0, 2) == "sp")
            {
                stxt = new XSparepartText(rpt, usr, spparam.Split(','));
                stxt.SetDefaultParameter("", "", "", false, true, rparam);
            }
            else
            {
                stxt = new XSalesText(rpt, usr, spparam.Split(','));
                stxt.SetDefaultParameter("", "", "", false, true, rparam);
            }
                       

            Response.AddHeader("content-disposition", "attachment;filename=" + flnm+ ".txt");
            Response.ContentType = "text/plain";
            Response.Write(stxt.Print());
            Response.End();

            return new EmptyResult();
        }
    }
}
