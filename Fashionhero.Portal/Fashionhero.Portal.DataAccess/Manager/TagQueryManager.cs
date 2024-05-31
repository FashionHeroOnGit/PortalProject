using Fashionhero.Portal.DataAccess.Core;
using Fashionhero.Portal.Shared.Model.Entity;
using Fashionhero.Portal.Shared.Model.Searchable;
using Microsoft.Extensions.Logging;

namespace Fashionhero.Portal.DataAccess.Manager
{
    public class TagQueryManager : BaseEntityQueryManager<PortalDatabaseContext, Tag, SearchableTag>
    {
        /// <inheritdoc />
        public TagQueryManager(PortalDatabaseContext context, ILogger<TagQueryManager> logger) : base(context, logger)
        {
        }

        /// <inheritdoc />
        protected override IQueryable<Tag> AddQueryArguments(SearchableTag searchable, IQueryable<Tag> query)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        protected override IQueryable<Tag> GetBaseQuery()
        {
            return context.Tags.AsQueryable();
        }
    }
}