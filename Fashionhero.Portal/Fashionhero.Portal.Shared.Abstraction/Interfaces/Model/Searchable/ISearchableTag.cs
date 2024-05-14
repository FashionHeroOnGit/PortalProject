using Fashionhero.Portal.Shared.Abstraction.Interfaces.Persistence;

namespace Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Searchable
{
    public interface ISearchableTag : ISearchable
    {
        int ProductId { get; set; }
        string Name { get; set; }
        string Value { get; set; }
    }
}