using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.DataWarehouse.Models
{
    public class LiveStockPartTransDetail : LiveStockPartInqDetail
    {
        public decimal Average { set; get; }
        public string MovingCode { set; get; }
    }
}