namespace Fashionhero.Portal.Shared.Abstraction.Interfaces.Persistence
{
    public interface IEntity : ISearchable
    {
        DateTime CreatedDateTime { get; set; }
        DateTime UpdatedDateTime { get; set; }
    }
}