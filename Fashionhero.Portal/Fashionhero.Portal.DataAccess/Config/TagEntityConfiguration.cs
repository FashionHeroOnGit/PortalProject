using Fashionhero.Portal.DataAccess.Core;
using Fashionhero.Portal.Shared.Model.Entity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fashionhero.Portal.DataAccess.Config
{
    public class TagEntityConfiguration : BaseEntityConfiguration<Tag>
    {
        /// <inheritdoc />
        public override void Configure(EntityTypeBuilder<Tag> builder)
        {
            base.Configure(builder);

            builder.HasIndex(x => new {x.Name, x.ProductId,}).IsUnique();
            builder.HasIndex(x => new {x.Name, x.ReferenceId,}).IsUnique();
        }
    }
}