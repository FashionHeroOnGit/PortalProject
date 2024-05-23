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
        }
    }
}