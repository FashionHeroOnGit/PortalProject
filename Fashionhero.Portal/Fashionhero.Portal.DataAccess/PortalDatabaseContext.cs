using Fashionhero.Portal.DataAccess.Config;
using Fashionhero.Portal.DataAccess.Core;
using Fashionhero.Portal.Shared.Model.Entity;
using Microsoft.EntityFrameworkCore;

namespace Fashionhero.Portal.DataAccess
{
    public class PortalDatabaseContext : BaseDatabaseContext
    {
        public DbSet<Image> Images { get; set; }
        public DbSet<LocaleProduct> LocaleProducts { get; set; }
        public DbSet<Price> Prices { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Size> Sizes { get; set; }
        public DbSet<Tag> Tags { get; set; }

        /// <inheritdoc />
        public PortalDatabaseContext(DbContextOptions options) : base(options)
        {
        }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new ImageEntityConfiguration());
            modelBuilder.ApplyConfiguration(new LocaleProductEntityConfiguration());
            modelBuilder.ApplyConfiguration(new PriceEntityConfiguration());
            modelBuilder.ApplyConfiguration(new ProductEntityConfiguration());
            modelBuilder.ApplyConfiguration(new SizeEntityConfiguration());
            modelBuilder.ApplyConfiguration(new TagEntityConfiguration());
        }
    }
}