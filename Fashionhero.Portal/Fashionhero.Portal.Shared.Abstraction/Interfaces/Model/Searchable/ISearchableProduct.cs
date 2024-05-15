using Fashionhero.Portal.Shared.Abstraction.Interfaces.Persistence;

namespace Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Searchable
{
    public interface ISearchableProduct : ISearchable, ICommonSearchable
    {
        string Manufacturer { get; set; }
        string LinkBase { get; set; }
        string Category { get; set; }
        string Brand { get; set; }
    }
}