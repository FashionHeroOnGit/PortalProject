using Fashionhero.Portal.DataAccess.Core;
using Fashionhero.Portal.Shared.Model.Entity;
using Fashionhero.Portal.Shared.Model.Searchable;
using Microsoft.Extensions.Logging;

namespace Fashionhero.Portal.DataAccess.Manager
{
    public class ImageQueryManager : BaseEntityQueryManager<PortalDatabaseContext, Image, SearchableImage>
    {
        /// <inheritdoc />
        public ImageQueryManager(PortalDatabaseContext context, ILogger<ImageQueryManager> logger) : base(context,
            logger)
        {
        }

        /// <inheritdoc />
        protected override IQueryable<Image> AddQueryArguments(SearchableImage searchable, IQueryable<Image> query)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        protected override IQueryable<Image> GetBaseQuery()
        {
            return context.Images.AsQueryable();
        }
    }
}