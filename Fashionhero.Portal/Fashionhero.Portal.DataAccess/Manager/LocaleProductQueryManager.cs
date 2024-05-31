using Fashionhero.Portal.DataAccess.Core;
using Fashionhero.Portal.Shared.Model.Entity;
using Fashionhero.Portal.Shared.Model.Searchable;
using Microsoft.Extensions.Logging;

namespace Fashionhero.Portal.DataAccess.Manager
{
    public class LocaleProductQueryManager : BaseEntityQueryManager<PortalDatabaseContext, LocaleProduct,
        SearchableLocaleProduct>
    {
        /// <inheritdoc />
        public LocaleProductQueryManager(
            PortalDatabaseContext context, ILogger<LocaleProductQueryManager> logger) : base(context, logger)
        {
        }

        /// <inheritdoc />
        protected override IQueryable<LocaleProduct> AddQueryArguments(
            SearchableLocaleProduct searchable, IQueryable<LocaleProduct> query)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        protected override IQueryable<LocaleProduct> GetBaseQuery()
        {
            return context.LocaleProducts.AsQueryable();
        }
    }
}