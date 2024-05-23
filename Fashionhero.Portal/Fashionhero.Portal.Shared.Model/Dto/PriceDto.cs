using Fashionhero.Portal.Shared.Abstraction.Enums;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Dto;

namespace Fashionhero.Portal.Shared.Model.Dto
{
    public class PriceDto : IPriceDto
    {
        /// <inheritdoc />
        public float Sale { get; set; }

        /// <inheritdoc />
        public float? Discount { get; set; }

        /// <inheritdoc />
        public CurrencyCode Currency { get; set; }
    }
}