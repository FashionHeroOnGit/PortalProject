using Fashionhero.Portal.DataAccess.Core;
using Fashionhero.Portal.Shared.Model.Entity;
using Fashionhero.Portal.Shared.Model.Searchable;

namespace Fashionhero.Portal.DataAccess.Manager
{
    public class ImageQueryManager : BaseEntityQueryManager<PortalDatabaseContext, Image, SearchableImage>
    {
        /// <inheritdoc />
        public ImageQueryManager(PortalDatabaseContext context) : base(context)
        {
        }

        /// <inheritdoc />
        protected override IQueryable<Image> GetBaseQuery()
        {
            return context.Images.AsQueryable();
        }

        /// <inheritdoc />
        protected override IQueryable<Image> AddQueryArguments(SearchableImage searchable, IQueryable<Image> query)
        {
            throw new NotImplementedException();
        }
    }
}