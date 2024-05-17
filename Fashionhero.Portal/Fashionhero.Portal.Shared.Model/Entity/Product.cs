using Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Entity;
using Newtonsoft.Json;

namespace Fashionhero.Portal.Shared.Model.Entity
{
    public class Product : IProduct
    {
        private readonly int id;

        public Product(int id = 0)
        {
            this.id = id;
        }

        /// <inheritdoc />
        public ICollection<IImage> Images { get; set; } = new List<IImage>();

        /// <inheritdoc />
        public ICollection<ILocaleProduct> Locales { get; set; } = new List<ILocaleProduct>();

        /// <inheritdoc />
        public ICollection<ISize> Sizes { get; set; } = new List<ISize>();

        /// <inheritdoc />
        public ICollection<IPrice> Prices { get; set; } = new List<IPrice>();

        /// <inheritdoc />
        public ICollection<ITag> ExtraTags { get; set; } = new List<ITag>();

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