using Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Searchable;

namespace Fashionhero.Portal.Shared.Model.Searchable
{
    public class SearchableLocaleProduct : ISearchableLocaleProduct
    {
        /// <inheritdoc />
        public int ItemGroupId { get; set; }

        /// <inheritdoc />
        public string IsoName { get; set; }

        /// <inheritdoc />
        public string Title { get; set; }

        /// <inheritdoc />
        public string? Description { get; set; }

        /// <inheritdoc />
        public string Type { get; set; }

        /// <inheritdoc />
        public string LocalType { get; set; }

        /// <inheritdoc />
        public string CountryOrigin { get; set; }

        /// <inheritdoc />
        public string Material { get; set; }

        /// <inheritdoc />
        public string? Gender { get; set; }

        /// <inheritdoc />
        public string Colour { get; set; }

        /// <inheritdoc />
        public int ReferenceId { get; set; }

        /// <inheritdoc />
        public int Id { get; set; }

        /// <inheritdoc />
        public int ProductId { get; set; }
    }
}