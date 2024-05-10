using Fashionhero.Portal.Shared.Abstraction.Enums;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Entity;

namespace Fashionhero.Portal.Shared.Model.Entity
{
    public class Price : IPrice
    {
        private readonly int id;

        public Price(int id = 0)
        {
            this.id = id;
        }

        /// <inheritdoc />
        public IProduct Product { get; set; }

        /// <inheritdoc />
        public float Sale { get; set; }

        /// <inheritdoc />
        public float? Discount { get; set; }

        /// <inheritdoc />
        public CurrencyCode Currency { get; set; }

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