using Fashionhero.Portal.Shared.Abstraction.Interfaces.Persistence;

namespace Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Searchable
{
    public interface ISearchableLocaleProduct : ISearchable, ICommonSearchable
    {
        int ProductId { get; set; }
        int ItemGroupId { get; set; }
        string IsoName { get; set; }
        string Title { get; set; }
        string? Description { get; set; }
        string Type { get; set; }
        string LocalType { get; set; }
        string CountryOrigin { get; set; }
        string Material { get; set; }
        string Gender { get; set; }
        string Colour { get; set; }
    }
}