using Fashionhero.Portal.DataAccess.Core;
using Fashionhero.Portal.Shared.Model.Dto;
using Fashionhero.Portal.Shared.Model.Entity;
using Fashionhero.Portal.Shared.Model.Searchable;

namespace Fashionhero.Portal.DataAccess.Manager
{
    public class SizeQueryManager : BaseEntityQueryManager<PortalDatabaseContext, Size, SearchableSize, SizeDto>
    {
        /// <inheritdoc />
        public SizeQueryManager(PortalDatabaseContext context) : base(context)
        {
        }

        /// <inheritdoc />
        protected override Size BuildEntity(SizeDto dto)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        protected override IQueryable<Size> GetBaseQuery()
        {
            return context.Sizes.AsQueryable();
        }

        /// <inheritdoc />
        protected override IQueryable<Size> AddQueryArguments(SearchableSize searchable, IQueryable<Size> query)
        {
            throw new NotImplementedException();
        }
    }
}