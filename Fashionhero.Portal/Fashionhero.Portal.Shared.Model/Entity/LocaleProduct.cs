using Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Entity;

namespace Fashionhero.Portal.Shared.Model.Entity
{
    public class LocaleProduct : ILocaleProduct
    {
        private readonly int id;

        public LocaleProduct(int id, Product product)
        {
            this.id = id;
            Product = product;
        }

        /// <inheritdoc />
        public IProduct Product { get; set; }

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
        public int Id
        {
            get => id;
            set => throw new InvalidOperationException("Id of an entity cannot be changed");
        }

        /// <inheritdoc />
        public int ProductId { get; set; }

        /// <inheritdoc />
        public DateTime CreatedDateTime { get; set; }

        /// <inheritdoc />
        public DateTime UpdatedDateTime { get; set; }
    }
}