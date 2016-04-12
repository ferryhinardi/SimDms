using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Common.Models 
{
   [Table("GnMstReminder")]
    public class GnMstReminder
    {
    [Key]
    [Column(Order = 1)]
    public string CompanyCode { get; set; }
    [Key]
    [Column(Order = 2)]
    public int ReminderKey { get; set; }
    public string ReminderCode { get; set; }
    public string ReminderDescription { get; set; }
    public DateTime? ReminderDate { get; set; }
    public int ReminderTimePeriod { get; set; }
    public string ReminderTimeDim { get; set; }
    public bool? ReminderStatus { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string LastUpdateBy { get; set; }
    public DateTime? LastUpdateDate { get; set; }

    }

   public class ReminderView
   {
       public string CompanyCode { get; set; }
       public string ReminderKey { get; set; }
       public string ReminderCode { get; set; }
       public string ReminderDescription { get; set; }
       public string ReminderDate { get; set; }
       public string ReminderTimePeriod { get; set; }
       public string ReminderTimeDim { get; set; }
       public string ReminderStatus { get; set; }
   }
}