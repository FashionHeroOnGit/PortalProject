using Fashionhero.Portal.Shared.Abstraction.Enums;

namespace Fashionhero.Portal.Shared.Abstraction.Interfaces
{
    public interface IMapper
    {
        object GetDictionaryValue(object key);
        bool IsMapperOfType(MapType mapType);
    }
}