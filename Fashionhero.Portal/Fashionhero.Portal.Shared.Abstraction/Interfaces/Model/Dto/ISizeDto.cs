using Fashionhero.Portal.Shared.Abstraction.Interfaces.Persistence;

namespace Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Dto
{
    public interface ISizeDto : IDto
    {
        int Quantity { get; set; }
        long Ean { get; set; }
        string Primary { get; set; }
        string? Secondary { get; set; }
        string LinkPostFix { get; set; }
        string ModelProductNumber { get; set; }
        int ReferenceId { get; set; }

        bool IsStocked { get; }
    }
}