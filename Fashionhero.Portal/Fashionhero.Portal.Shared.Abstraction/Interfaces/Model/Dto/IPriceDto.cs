using Fashionhero.Portal.Shared.Abstraction.Enums;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Persistence;

namespace Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Dto
{
    public interface IPriceDto : IDto
    {
        float Sale { get; set; }
        float? Discount { get; set; }
        CurrencyCode Currency { get; set; }
    }
}