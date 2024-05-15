using Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Searchable;

namespace Fashionhero.Portal.Shared.Model.Searchable
{
    public class SearchableTag : ISearchableTag
    {
        /// <inheritdoc />
        public string Name { get; set; }

        /// <inheritdoc />
        public string Value { get; set; }

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