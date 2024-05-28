using Fashionhero.Portal.Shared.Abstraction.Interfaces.Persistence;

namespace Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Searchable
{
    public interface ISearchableSize : ISearchable, ICommonProductSearchable
    {
        int ProductId { get; set; }
        int Quantity { get; set; }
        long Ean { get; set; }
        string Primary { get; set; }
        string? Secondary { get; set; }
        string LinkPostFix { get; set; }


        bool IsStocked { get; }
    }
}