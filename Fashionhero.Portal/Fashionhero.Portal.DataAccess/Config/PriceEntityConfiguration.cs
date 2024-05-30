using Fashionhero.Portal.DataAccess.Core;
using Fashionhero.Portal.Shared.Model.Entity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fashionhero.Portal.DataAccess.Config
{
    public class PriceEntityConfiguration : BaseEntityConfiguration<Price>
    {
        /// <inheritdoc />
        public override void Configure(EntityTypeBuilder<Price> builder)
        {
            base.Configure(builder);

            builder.HasIndex(x => new {x.ReferenceId, x.Currency,}).IsUnique();
        }
    }
}