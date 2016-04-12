using System.Data.Entity;

namespace eXpressAPI.Models
{
    public partial class DataAccessContext : DbContext
    {
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<DocDetail> DDetails { get; set; }
        public virtual DbSet<DocHeader> DHeaders { get; set; }
        public virtual DbSet<DocType> DocTypes { get; set; }
        public virtual DbSet<Classification> Classifications { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Contact> Contacts { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Vendor> Vendors { get; set; }
        public virtual DbSet<ItemPrice> ItemPrices { get; set; }
        public virtual DbSet<Item> Items { get; set; }
        public virtual DbSet<ItemType> ItemTypes { get; set; }
        public virtual DbSet<Mfg> Mfgs { get; set; }
        public virtual DbSet<Prospek> Prospeks { get; set; }
        public virtual DbSet<SpecCategory> SpecCategories { get; set; }
        public virtual DbSet<SPECS> SPECS { get; set; }
        public virtual DbSet<UOM> UOM { get; set; }
        public virtual DbSet<Currency> Currencies { get; set; }

        public virtual DbSet<QaHeader> QaHeaders { get; set; }
        public virtual DbSet<QaDetail> QaDetails { get; set; }
        public virtual DbSet<QDetailType> DetailTypes { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<SPECINFO> InfoSpecs { get; set; }
        public virtual DbSet<VwItem> VwItems { get; set; }
        public virtual DbSet<VwPrice> VwPrices { get; set; }
        public virtual DbSet<VwItemType> VwItemTypes { get; set; }
        public virtual DbSet<VwProject> VwProjects { get; set; }
        public virtual DbSet<VwQuotation> VwQuotations { get; set; }

        public virtual DbSet<ConfigNumber> SetupNumber { get; set; }
        public virtual DbSet<QaDiscount> Discounts { get; set; }
        public virtual DbSet<Departement> Departements { get; set; }
    }
}