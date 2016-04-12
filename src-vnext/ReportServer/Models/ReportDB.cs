namespace eXpressReport.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class ReportDB : DbContext
    {
        public ReportDB()
            : base("name=DataContext")
        {
        }

        public virtual DbSet<ReportSession> ReportSessions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ReportSession>()
                .Property(e => e.SessionId)
                .IsUnicode(false);

            modelBuilder.Entity<ReportSession>()
                .Property(e => e.ReportId)
                .IsUnicode(false);

            modelBuilder.Entity<ReportSession>()
                .Property(e => e.ConnectionId)
                .IsUnicode(false);

            modelBuilder.Entity<ReportSession>()
                .Property(e => e.SQL)
                .IsUnicode(false);

            modelBuilder.Entity<ReportSession>()
                .Property(e => e.Parameters)
                .IsUnicode(false);

            modelBuilder.Entity<ReportSession>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);
        }
    }
}
