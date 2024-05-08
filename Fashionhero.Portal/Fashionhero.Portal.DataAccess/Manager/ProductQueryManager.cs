using Fashionhero.Portal.DataAccess.Core;
using Fashionhero.Portal.Shared.Model.Dto;
using Fashionhero.Portal.Shared.Model.Entity;
using Fashionhero.Portal.Shared.Model.Searchable;

namespace Fashionhero.Portal.DataAccess.Manager
{
    public class
        ProductQueryManager : BaseEntityQueryManager<PortalDatabaseContext, Product, SearchableProduct, ProductDto>
    {
        /// <inheritdoc />
        public ProductQueryManager(PortalDatabaseContext context) : base(context)
        {
        }

        /// <inheritdoc />
        protected override Product BuildEntity(ProductDto dto)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
    }
}