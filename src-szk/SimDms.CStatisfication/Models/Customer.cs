using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.CStatisfication.Models
{
    [Table("CsCustomerView")]
    public class Customer
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string CustomerCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string Chassis { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string PhoneNo { get; set; }
        public string ColourCode { get; set; }
        public string PoliceRegNo { get; set; }
        public string Engine { get; set; }
        public string SalesmanCode { get; set; }
        public string SalesmanName { get; set; }
    }
}