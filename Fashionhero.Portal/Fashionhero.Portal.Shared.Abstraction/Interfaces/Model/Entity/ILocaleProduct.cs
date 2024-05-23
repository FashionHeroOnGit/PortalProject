using Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Searchable;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Persistence;

namespace Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Entity
{
    public interface ILocaleProduct : ISearchableLocaleProduct, IEntity
    {
        IProduct Product { get; set; }
    }
}