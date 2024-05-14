using Fashionhero.Portal.Shared.Abstraction.Enums;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Persistence;

namespace Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Searchable
{
    public interface ISearchablePrice : ISearchable
    {
        int ProductId { get; set; }
        float NormalSell { get; set; }
        float? Discount { get; set; }
        CurrencyCode Currency { get; set; }
    }
}