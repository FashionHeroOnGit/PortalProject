using Fashionhero.Portal.Shared.Abstraction.Interfaces.Persistence;

namespace Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Dto
{
    public interface IProductDto : IDto
    {
        string Manufacturer { get; set; }
        string LinkBase { get; set; }
        string Category { get; set; }
        string Brand { get; set; }
        int ReferenceId { get; set; }
    }
}