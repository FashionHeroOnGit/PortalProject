﻿using Fashionhero.Portal.DataAccess.Core;
using Fashionhero.Portal.Shared.Model.Dto;
using Fashionhero.Portal.Shared.Model.Entity;
using Fashionhero.Portal.Shared.Model.Searchable;

namespace Fashionhero.Portal.DataAccess.Manager
{
    public class TagQueryManager : BaseEntityQueryManager<PortalDatabaseContext, Tag, SearchableTag, TagDto>
    {
        /// <inheritdoc />
        public TagQueryManager(PortalDatabaseContext context) : base(context)
        {
        }

        /// <inheritdoc />
        protected override Tag BuildEntity(TagDto dto)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        protected override IQueryable<Tag> GetBaseQuery()
        {
            return context.Tags.AsQueryable();
        }

        /// <inheritdoc />
        protected override IQueryable<Tag> AddQueryArguments(SearchableTag searchable, IQueryable<Tag> query)
        {
            throw new NotImplementedException();
        }
    }
}