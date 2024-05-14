﻿using Fashionhero.Portal.DataAccess.Core;
using Fashionhero.Portal.Shared.Model.Entity;
using Fashionhero.Portal.Shared.Model.Searchable;

namespace Fashionhero.Portal.DataAccess.Manager
{
    public class ProductQueryManager : BaseEntityQueryManager<PortalDatabaseContext, Product, SearchableProduct>
    {
        /// <inheritdoc />
        public ProductQueryManager(PortalDatabaseContext context) : base(context)
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
            if (searchable.ReferenceId != default)
                query = query.Where(x => x.ReferenceId == searchable.ReferenceId);

            return query;
        }
    }
}