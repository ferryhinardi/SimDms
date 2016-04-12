using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Web.Models
{
    [Table("RegisteredUser")]
    public class RegisteredUser
    {
        [Key]
        [Column(Order=1)]
        public string UserID { get; set; }
        public string DealerCode { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool? IsApproved { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string TokenID { get; set; }
        public DateTime TokenExpiredDate { get; set; }

    }
}