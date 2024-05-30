using Fashionhero.Portal.Shared.Abstraction.Enums;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Searchable;

namespace Fashionhero.Portal.Shared.Model.Searchable
{
    public class SearchablePrice : ISearchablePrice
    {
        /// <inheritdoc />
        public float NormalSell { get; set; }

        /// <inheritdoc />
        public float? Discount { get; set; }

        /// <inheritdoc />
        public CurrencyCode Currency { get; set; }

        /// <inheritdoc />
        public int Id { get; set; }

        /// <inheritdoc />
        public DateTime CreatedDateTime { get; set; }

        /// <inheritdoc />
        public DateTime UpdatedDateTime { get; set; }

        /// <inheritdoc />
        public int ProductId { get; set; }

        /// <inheritdoc />
        public int ReferenceId { get; set; }
    }
}