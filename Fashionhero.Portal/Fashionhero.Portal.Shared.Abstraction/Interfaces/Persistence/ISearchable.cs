namespace Fashionhero.Portal.Shared.Abstraction.Interfaces.Persistence
{
    public interface ISearchable
    {
        int Id { get; set; }
        DateTime CreatedDateTime { get; set; }
        DateTime UpdatedDateTime { get; set; }
    }
}