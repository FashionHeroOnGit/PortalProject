using Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Dto;

namespace Fashionhero.Portal.Shared.Model.Dto
{
    public class ProductDto : IProductDto
    {
        /// <inheritdoc />
        public string Manufacturer { get; set; }

        /// <inheritdoc />
        public string LinkBase { get; set; }

        /// <inheritdoc />
        public string Category { get; set; }

        /// <inheritdoc />
        public string Brand { get; set; }

        /// <inheritdoc />
        public int ReferenceId { get; set; }
    }
}