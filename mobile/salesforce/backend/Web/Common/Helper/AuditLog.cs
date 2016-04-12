using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eXpressAPI.Models
{

    //[Table("SM_AuditLog")]
    //public class AuditLog
    //{
    //    [Key]
    //    public Guid AuditLogID { get; set; }

    //    [Required]
    //    [MaxLength(50)]
    //    public string UserID { get; set; }

    //    [Required]
    //    public DateTime EventDateUTC { get; set; }

    //    [Required]
    //    [MaxLength(1)]
    //    public string EventType { get; set; }

    //    [Required]
    //    [MaxLength(100)]
    //    public string TableName { get; set; }

    //    [Required]
    //    [MaxLength(100)]
    //    public string RecordID { get; set; }

    //    [Required]
    //    [MaxLength(100)]
    //    public string ColumnName { get; set; }

    //    public string OriginalValue { get; set; }

    //    public string NewValue { get; set; }
    //}

    public partial class AccessPermissionDetail
    {
        public string roleid { get; set; }
        public string menuid { get; set; }
        public string accessname { get; set; }
        public string action { get; set; }
    }

    public class KeyValueParam
    {
        public string _id { get; set; }
        public string key { get; set; }
        public string value { get; set; }
        public string remove { get; set; }
    }

    public partial class ReportParameters
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Project { get; set; }
        public string Customer { get; set; }
        public string Sales { get; set; }
        public string ItemType { get; set; }
        public string DocType { get; set; }
    }

    [Table("ReportSession")]
    public partial class ReportSession
    {
        [Key]
        [StringLength(50)]
        public string SessionId { get; set; }

        [StringLength(150)]
        public string ReportId { get; set; }

        [StringLength(250)]
        public string ConnectionId { get; set; }

        [Column(TypeName = "text")]
        public string SQL { get; set; }

        [Column(TypeName = "text")]
        public string Parameters { get; set; }

        [StringLength(32)]
        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }
    }

    public abstract class BaseEntityObject
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string CompanyCode { get; set; }

        [DefaultValue("0")]
        public bool Status { get; set; }

        [DefaultValue("0")]
        public bool IsDeleted { get; set; }

        [StringLength(32)]
        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        [StringLength(32)]
        public string ChangeBy { get; set; }

        public DateTime? ChangeDate { get; set; }

    }

    public abstract class BaseCompany
    {

        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(10)]
        public string Abbr { get; set; }

        [StringLength(250)]
        public string Address { get; set; }

        [StringLength(250)]
        public string Address2 { get; set; }

        [StringLength(250)]
        public string Address3 { get; set; }

        [StringLength(75)]
        public string City { get; set; }

        [StringLength(8)]
        public string ZIP { get; set; }

        [StringLength(20)]
        public string PhoneNo { get; set; }

        [StringLength(20)]
        public string FaxNo { get; set; }

        [StringLength(50)]
        public string Website { get; set; }

        [DefaultValue("1")]
        public bool Status { get; set; }

        [StringLength(32)]
        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        [StringLength(32)]
        public string ChangeBy { get; set; }

        public DateTime? ChangeDate { get; set; }

    }

    public class SysNavigation
    {
        public SysNavigation()
        {
            Detail = new List<SysNavigation>();
        }

        public string MenuId { get; set; }
        public string MenuCaption { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }
        public string ModuleName { get; set; }
        public string ParentName { get; set; }

        public virtual List<SysNavigation> Detail { get; set; }
    }
    
    public class TreeData
    {
        public TreeData()
        {
            Children = new List<TreeData>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public bool value1 { get; set; }
        public bool value2 { get; set; }
        public bool value3 { get; set; }

        public virtual List<TreeData> Children { get; set; }
    }

    public class TreeRoleMenu
    {
        public string RoleId { get; set; }
        public string MenuId { get; set; }
        public string ParentId { get; set; }
        public string Name { get; set; }
        public int Seq { get; set; }
        public int Level { get; set; }
        public bool value1 { get; set; }
        public bool value2 { get; set; }
        public bool value3 { get; set; }
    }


    public class TreeDeptMenu
    {
        public string RoleId { get; set; }
        public string CodeId { get; set; }
        public string ParentId { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public bool Status { get; set; }
    }

    public class ComboList
    {
        public ComboList()
        {

        }
        public ComboList(string v, string t)
        {
            value = v;
            text = t;
        }
        public string value { get; set; }
        public string text { get; set; }
    }

    public class RoleMapping
    {
        public string roleid { get; set; }
        public string models { get; set; }
    }


    public class ComboListEx
    {
        public string value { get; set; }
        public string text { get; set; }
        public string companyid { get; set; }
    }


    public class ComboListExt
    {
        public string value { get; set; }
        public string text { get; set; }
        public string CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAbbr { get; set; }
    }

    public class RoleMappingList
    {
        public string RoleId { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string IDX { get; set; }
        public string CompanyName { get; set; }
    }

    public class ExecScalarResult
    {
        public string result { get; set; }
    }

    public class oTimeSetup
    {
        public int Seq { get; set; }
        public string DayType { get; set; }
        public double H1 { get; set; }
        public double H2 { get; set; }
        public double H3 { get; set; }
        public double H4 { get; set; }
        public double H5 { get; set; }
        public double H6 { get; set; }
        public double H7 { get; set; }
        public double H8 { get; set; }
        public double H9 { get; set; }
        public double H10 { get; set; }

    }
}