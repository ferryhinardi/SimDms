using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Tax.Controllers
{
    public class CetakFormController : BaseController 
    {
        #region Cetak From
        public string FakturPajak()
        {
            return HtmlRender("cetakform/fakturpajak.js");
        }
        #endregion
    }
}
