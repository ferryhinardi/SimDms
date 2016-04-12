using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.DataWarehouse.Models
{
    public class LiveStockPartInqDetail : LiveStockPartDetail
    {
        public string DealerName { get; set; }
        public string ContactPersonName { get; set; }
        public string FaxNo { get; set; }
        public string PhoneNo { get; set; }
        public string HandPhoneNo { get; set; }
        public string EmailAddr { get; set; }
    }
}