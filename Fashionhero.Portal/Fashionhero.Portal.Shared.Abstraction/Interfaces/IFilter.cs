using Fashionhero.Portal.Shared.Abstraction.Enums;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Entity;

namespace Fashionhero.Portal.Shared.Abstraction.Interfaces
{
    public interface IFilter
    {
        ICollection<IProduct> FilterProducts(ICollection<IProduct> oldProducts);

        bool IsFilterOfType(FilterType filterType);
    }
}