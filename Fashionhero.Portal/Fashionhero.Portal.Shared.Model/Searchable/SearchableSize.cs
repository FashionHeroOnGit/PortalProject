using Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Searchable;

namespace Fashionhero.Portal.Shared.Model.Searchable
{
    public class SearchableSize : ISearchableSize
    {
        /// <inheritdoc />
        public int Quantity { get; set; }

        /// <inheritdoc />
        public long Ean { get; set; }

        /// <inheritdoc />
        public string Primary { get; set; }

        /// <inheritdoc />
        public string? Secondary { get; set; }

        /// <inheritdoc />
        public string LinkPostFix { get; set; }

        /// <inheritdoc />
        public string LinkBase { get; set; }

        /// <inheritdoc />
        public string ModelProductNumber { get; set; }

        /// <inheritdoc />
        public int ReferenceId { get; set; }

        /// <inheritdoc />
        public int Id { get; set; }

        /// <inheritdoc />
        public DateTime CreatedDateTime { get; set; }

        /// <inheritdoc />
        public DateTime UpdatedDateTime { get; set; }

        /// <inheritdoc />
        public int ProductId { get; set; }

        /// <inheritdoc />
        public bool IsStocked => Quantity != 0;
    }
}