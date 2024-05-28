using Fashionhero.Portal.Shared.Abstraction.Interfaces.Persistence;

namespace Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Searchable
{
    public interface ISearchableProduct : ISearchable, ICommonProductSearchable
    {
        string Manufacturer { get; set; }
        string Category { get; set; }
        string Brand { get; set; }
    }
}