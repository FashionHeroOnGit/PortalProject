using Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Dto;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Persistence;

namespace Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Searchable
{
    public interface ISearchableSize : ISizeDto, ISearchable
    {
        int ProductId { get; set; }
    }
}