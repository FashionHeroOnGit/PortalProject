using Fashionhero.Portal.Shared.Abstraction.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fashionhero.Portal.DataAccess.Core
{
    public abstract class BaseEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
        where TEntity : class, IEntity
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            Type type = typeof(TEntity);
            var idName = $"{type.Name}Id";

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName(idName);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
        }
    }
}