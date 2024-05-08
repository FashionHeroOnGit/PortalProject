using Fashionhero.Portal.Shared.Abstraction.Interfaces.Persistence;

namespace Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Dto
{
    public interface ITagDto : IDto
    {
        string Name { get; set; }
        string Value { get; set; }
    }
}