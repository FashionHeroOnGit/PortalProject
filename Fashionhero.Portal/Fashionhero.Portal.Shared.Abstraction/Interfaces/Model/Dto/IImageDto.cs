using Fashionhero.Portal.Shared.Abstraction.Interfaces.Persistence;

namespace Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Dto
{
    public interface IImageDto : IDto
    {
        string Url { get; set; }
    }
}