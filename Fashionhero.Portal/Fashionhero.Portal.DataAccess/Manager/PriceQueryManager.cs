﻿using Fashionhero.Portal.DataAccess.Core;
using Fashionhero.Portal.Shared.Model.Entity;
using Fashionhero.Portal.Shared.Model.Searchable;

namespace Fashionhero.Portal.DataAccess.Manager
{
    public class PriceQueryManager : BaseEntityQueryManager<PortalDatabaseContext, Price, SearchablePrice>
    {
        /// <inheritdoc />
        public PriceQueryManager(PortalDatabaseContext context) : base(context)
        {
        }

        /// <inheritdoc />
        protected override IQueryable<Price> GetBaseQuery()
        {
            return context.Prices.AsQueryable();
        }

        /// <inheritdoc />
        protected override IQueryable<Price> AddQueryArguments(SearchablePrice searchable, IQueryable<Price> query)
        {
            throw new NotImplementedException();
        }
    }
}