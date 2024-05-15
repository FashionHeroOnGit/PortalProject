using Fashionhero.Portal.Shared.Abstraction.Interfaces.Persistence;

namespace Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Searchable
{
    public interface ISearchableImage : ISearchable, ICommonSearchable
    {
        int ProductId { get; set; }
        string Url { get; set; }
    }
}