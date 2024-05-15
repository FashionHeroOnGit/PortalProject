using Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Entity;

namespace Fashionhero.Portal.Shared.Model.Entity
{
    public class Product : IProduct
    {
        private readonly int id;

        /// <summary>
        /// Constructor for Entity Framework Core
        /// </summary>
        /// <param name="id"></param>
        /// <param name="images"></param>
        /// <param name="locales"></param>
        /// <param name="sizes"></param>
        /// <param name="prices"></param>
        /// <param name="extraTags"></param>
        private Product(
            int id, IList<IImage> images, IList<ILocaleProduct> locales, IList<ISize> sizes, IList<IPrice> prices,
            IList<ITag> extraTags)
        {
            this.id = id;
            Images = images.ToList();
            Locales = locales.ToList();
            Sizes = sizes.ToList();
            Prices = prices.ToList();
            ExtraTags = extraTags.ToList();
        }

        public Product(int id = 0)
        {
            this.id = id;
        }

        /// <inheritdoc />
        public IList<IImage> Images { get; set; }

        /// <inheritdoc />
        public IList<ILocaleProduct> Locales { get; set; }

        /// <inheritdoc />
        public IList<ISize> Sizes { get; set; }

        /// <inheritdoc />
        public IList<IPrice> Prices { get; set; }

        /// <inheritdoc />
        public IList<ITag> ExtraTags { get; set; }

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