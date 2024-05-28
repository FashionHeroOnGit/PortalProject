using Fashionhero.Portal.Shared.Abstraction.Enums;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Persistence;

namespace Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Searchable
{
    public interface ISearchablePrice : ISearchable, ICommonSearchable
    {
        int ProductId { get; set; }
        decimal NormalSell { get; set; }
        decimal? Discount { get; set; }
        CurrencyCode Currency { get; set; }
    }
}