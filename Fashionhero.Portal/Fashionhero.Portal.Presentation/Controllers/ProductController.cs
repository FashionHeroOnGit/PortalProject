using Fashionhero.Portal.Shared.Abstraction.Interfaces.Persistence;
using Fashionhero.Portal.Shared.Model.Dto;
using Fashionhero.Portal.Shared.Model.Entity;
using Fashionhero.Portal.Shared.Model.Searchable;
using Microsoft.AspNetCore.Mvc;

namespace Fashionhero.Portal.Presentation.Controllers
{
    [Route(Constants.ROUTE_TEMPLATE)]
    [ApiController]
    public class ProductController : EntityController<Product, SearchableProduct, ProductDto>
    {
        /// <inheritdoc />
        public ProductController(IEntityQueryManager<Product, SearchableProduct, ProductDto> manager) : base(manager)
        {
        }
    }
}
