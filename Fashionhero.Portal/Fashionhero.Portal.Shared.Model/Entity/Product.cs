using Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Entity;

namespace Fashionhero.Portal.Shared.Model.Entity
{
    public class Product : IProduct
    {
        private readonly int id;

        public Product(int id)
        {
            this.id = id;
        }

        /// <inheritdoc />
        public ICollection<IImage> Images { get; set; }

        /// <inheritdoc />
        public ICollection<ILocaleProduct> Locales { get; set; }

        /// <inheritdoc />
        public ICollection<ISize> Sizes { get; set; }

        /// <inheritdoc />
        public ICollection<IPrice> Prices { get; set; }

        /// <inheritdoc />
        public ICollection<ITag> ExtraTags { get; set; }

        /// <inheritdoc />
        public int Id
        {
            get => id;
            set => throw new InvalidOperationException("Id of an entity cannot be changed");
        }

        /// <inheritdoc />
        public DateTime CreatedDateTime { get; set; }

        /// <inheritdoc />
        public DateTime UpdatedDateTime { get; set; }

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

        /// <inheritdoc />
        public int TotalQuantity => Sizes.Sum(x => x.Quantity);
    }
}