using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace SimDms.Sparepart.Models
{
    public class spPembelianView
    {
        [Key]
        [Column(Order=1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string SuggorNo { get; set; }
        public DateTime? SuggorDate { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string TypeOfGoods { get; set; }
        public string MovingCode { get; set; }
        public string OrderType { get; set; }
    }
}
