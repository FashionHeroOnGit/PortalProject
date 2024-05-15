using Fashionhero.Portal.DataAccess.Core;
using Fashionhero.Portal.Shared.Model.Entity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fashionhero.Portal.DataAccess.Config
{
    public class SizeEntityConfiguration : BaseEntityConfiguration<Size>
    {
        /// <inheritdoc />
        public override void Configure(EntityTypeBuilder<Size> builder)
        {
            base.Configure(builder);

            builder.HasKey(x => new {x.Id, x.ProductId,});
            builder.HasIndex(x => x.Ean).IsUnique();
            builder.HasIndex(x => x.ReferenceId).IsUnique();
            builder.HasIndex(x => new {x.ProductId, x.Primary, x.Secondary,}).IsUnique();
        }
    }
}