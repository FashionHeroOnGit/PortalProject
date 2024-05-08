using Fashionhero.Portal.DataAccess.Core;
using Fashionhero.Portal.Shared.Model.Entity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fashionhero.Portal.DataAccess.Config
{
    public class ImageEntityConfiguration : BaseEntityConfiguration<Image>
    {
        /// <inheritdoc />
        public override void Configure(EntityTypeBuilder<Image> builder)
        {
            base.Configure(builder);
        }
    }
}