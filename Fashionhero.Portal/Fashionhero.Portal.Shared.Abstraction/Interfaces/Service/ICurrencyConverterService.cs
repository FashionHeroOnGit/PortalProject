using Fashionhero.Portal.Shared.Abstraction.Enums;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Entity;

namespace Fashionhero.Portal.Shared.Abstraction.Interfaces.Service
{
    public interface ICurrencyConverterService
    {
        Task<IPrice> ConvertPrice(IPrice fromPrice, CurrencyCode toCurrency);
        Task<decimal> GetRate(CurrencyCode fromCode, CurrencyCode toCode);
    }
}