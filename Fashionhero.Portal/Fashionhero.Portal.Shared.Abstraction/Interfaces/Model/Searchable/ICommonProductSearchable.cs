namespace Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Searchable
{
    public interface ICommonProductSearchable : ICommonSearchable
    {
        string LinkBase { get; set; }
        string ModelProductNumber { get; set; }
    }
}