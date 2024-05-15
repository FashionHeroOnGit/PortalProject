using Fashionhero.Portal.DataAccess.Core;
using Fashionhero.Portal.Shared.Model.Entity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fashionhero.Portal.DataAccess.Config
{
    public class LocaleProductEntityConfiguration : BaseEntityConfiguration<LocaleProduct>
    {
        /// <inheritdoc />
        public override void Configure(EntityTypeBuilder<LocaleProduct> builder)
        {
            base.Configure(builder);

            builder.HasKey(x => new {x.Id, x.ProductId,});
            builder.HasIndex(x => new {x.ReferenceId, x.IsoName,}).IsUnique();
        }
    }
}