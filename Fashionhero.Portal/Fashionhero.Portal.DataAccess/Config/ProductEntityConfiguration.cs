using Fashionhero.Portal.DataAccess.Core;
using Fashionhero.Portal.Shared.Model.Entity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fashionhero.Portal.DataAccess.Config
{
    public class ProductEntityConfiguration : BaseEntityConfiguration<Product>
    {
        /// <inheritdoc />
        public override void Configure(EntityTypeBuilder<Product> builder)
        {
            base.Configure(builder);

            builder.HasMany(x => (IList<Image>) x.Images).WithOne(x => (Product) x.Product)
                .HasForeignKey(x => x.ProductId).IsRequired();
            builder.HasMany(x => (IList<LocaleProduct>) x.Locales).WithOne(x => (Product) x.Product)
                .HasForeignKey(x => x.ProductId).IsRequired();
            builder.HasMany(x => (IList<Price>) x.Prices).WithOne(x => (Product) x.Product)
                .HasForeignKey(x => x.ProductId).IsRequired();
            builder.HasMany(x => (IList<Size>) x.Sizes).WithOne(x => (Product) x.Product)
                .HasForeignKey(x => x.ProductId).IsRequired();
            builder.HasMany(x => (IList<Tag>) x.ExtraTags).WithOne(x => (Product) x.Product)
                .HasForeignKey(x => x.ProductId).IsRequired();
        }
    }
}