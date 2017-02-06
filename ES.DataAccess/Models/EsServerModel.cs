namespace ES.DataAccess.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class EsServerModel : DbContext
    {
        public EsServerModel()
            : base("name=EsServerModel")
        {
        }

        public virtual DbSet<ESSharedProducts> ESSharedProducts { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ESSharedProducts>()
                .Property(e => e.CostPrice)
                .HasPrecision(12, 2);

            modelBuilder.Entity<ESSharedProducts>()
                .Property(e => e.OldPrice)
                .HasPrecision(12, 2);

            modelBuilder.Entity<ESSharedProducts>()
                .Property(e => e.Price)
                .HasPrecision(12, 2);

            modelBuilder.Entity<ESSharedProducts>()
                .Property(e => e.Discount)
                .HasPrecision(5, 2);

            modelBuilder.Entity<ESSharedProducts>()
                .Property(e => e.DealerPrice)
                .HasPrecision(12, 2);

            modelBuilder.Entity<ESSharedProducts>()
                .Property(e => e.DealerDiscount)
                .HasPrecision(5, 2);

            modelBuilder.Entity<ESSharedProducts>()
                .Property(e => e.MinQuantity)
                .HasPrecision(12, 2);
        }
    }
}
