using Fashionhero.Portal.DataAccess.Core;
using Fashionhero.Portal.Shared.Model.Entity;
using Fashionhero.Portal.Shared.Model.Searchable;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fashionhero.Portal.DataAccess.Manager
{
    public class ProductQueryManager : BaseEntityQueryManager<PortalDatabaseContext, Product, SearchableProduct>
    {
        /// <inheritdoc />
        public ProductQueryManager(PortalDatabaseContext context, ILogger<ProductQueryManager> logger) : base(context,
            logger)
        {
        }

        /// <inheritdoc />
        protected override IQueryable<Product> GetBaseQuery()
        {
            return context.Products.AsQueryable();
        }

        /// <inheritdoc />
        protected override IQueryable<Product> AddQueryArguments(
            SearchableProduct searchable, IQueryable<Product> query)
        {
            query = query.Include(x => x.Locales);
            query = query.Include(x => x.Sizes);
            query = query.Include(x => x.ExtraTags);
            query = query.Include(x => x.Prices);
            query = query.Include(x => x.Images);

            if (searchable.ReferenceId != default)
                query = query.Where(x => x.ReferenceId == searchable.ReferenceId);

            return query;
        }
    }
}