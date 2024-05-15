using Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Searchable;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Persistence;

namespace Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Entity
{
    public interface IProduct : ISearchableProduct, IEntity
    {
        IList<IImage> Images { get; set; }
        IList<ILocaleProduct> Locales { get; set; }
        IList<ISize> Sizes { get; set; }
        IList<IPrice> Prices { get; set; }
        IList<ITag> ExtraTags { get; set; }


        int TotalQuantity { get; }
    }
}