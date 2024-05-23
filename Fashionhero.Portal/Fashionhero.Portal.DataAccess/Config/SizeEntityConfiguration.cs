﻿using Fashionhero.Portal.DataAccess.Core;
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

            builder.HasIndex(x => x.Ean).IsUnique();
        }
    }
}