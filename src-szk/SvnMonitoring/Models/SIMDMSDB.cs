namespace SVNMON.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Data.Entity.ModelConfiguration.Conventions;
    using System.Threading.Tasks;
    using System.Linq.Expressions;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;


    public partial class SimDMSDB : DbContext
    {
        public SimDMSDB()
            : base("name=SimDMS")
        {
             
        }

        public virtual DbSet<OnlineDealer> OnlineDealers { get; set; }
        public virtual DbSet<SysRepository> SysRepositories { get; set; }
        
    }

    [Table("SysDealerOnlineView")]
    public class OnlineDealer
    {
        [Key]
        public string DealerCode { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public bool Status { get; set; }
        public string ProductType { get; set; }
        public string SessionId { get; set; }
        public string Location { get; set; }
        public string LastProccess { get; set; }
        public DateTime LastUpdate { get; set; }
    }

    [Table("SysRepository")]
    public class SysRepository
    {
        [Key]
        [Column(Order=0)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order=1)]
        public string Name { get; set; }
        public string PathName { get; set; }
        public DateTime LastUpdate { get; set; }
        public string LastMessage { get; set; }
        public string Revision { get; set; }
        public bool Active { get; set; }
    }


    public interface IRepository<T> where T : class
    {
        Task<int> AddAsync(T t);
        Task<int> RemoveAsync(T t);
        Task<List<T>> GetAllAsync();
        Task<int> UpdateAsync(T t);
        Task<int> CountAsync();
        Task<T> FindAsync(Expression<Func<T, bool>> match);
        Task<List<T>> FindAllAsync(Expression<Func<T, bool>> match);
    }
}
