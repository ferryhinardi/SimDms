using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.DataWarehouse.Models
{
    public class QaMonitoringOutletv2
    {
        public string GroupNo { get; set; }
        public string Area { get; set; }
        public string CompanyName { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string PICOutlet { get; set; }
        public string NoHP { get; set; }
        public string Email { get; set; }
        public int FakturPenjualan { get; set; }
        public int PermohonanFakPol { get; set; }
        public int QnrInput { get; set; }
        public string PersenFakturPenjualan { get; set; }
        public string PersenPermohonanFakPol { get; set; }
    }
}