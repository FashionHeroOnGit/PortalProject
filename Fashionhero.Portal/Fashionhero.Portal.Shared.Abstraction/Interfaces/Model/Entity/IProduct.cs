using Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Searchable;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Persistence;

namespace Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Entity
{
    public interface IProduct : ISearchableProduct, IEntity
    {
        ICollection<IImage> Images { get; set; }
        ICollection<ILocaleProduct> Locales { get; set; }
        ICollection<ISize> Sizes { get; set; }
        ICollection<IPrice> Prices { get; set; }
        ICollection<ITag> ExtraTags { get; set; }


        int TotalQuantity { get; }
    }
}