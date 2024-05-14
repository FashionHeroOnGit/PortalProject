using Fashionhero.Portal.DataAccess.Core;
using Fashionhero.Portal.Shared.Model.Entity;
using Fashionhero.Portal.Shared.Model.Searchable;

namespace Fashionhero.Portal.DataAccess.Manager
{
    public class LocaleProductQueryManager : BaseEntityQueryManager<PortalDatabaseContext, LocaleProduct,
        SearchableLocaleProduct>
    {
        /// <inheritdoc />
        public LocaleProductQueryManager(PortalDatabaseContext context) : base(context)
        {
        }

        /// <inheritdoc />
        protected override IQueryable<LocaleProduct> GetBaseQuery()
        {
            return context.LocaleProducts.AsQueryable();
        }

        /// <inheritdoc />
        protected override IQueryable<LocaleProduct> AddQueryArguments(
            SearchableLocaleProduct searchable, IQueryable<LocaleProduct> query)
        {
            throw new NotImplementedException();
        }
    }
}