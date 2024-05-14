namespace Fashionhero.Portal.Shared.Abstraction.Interfaces.Service
{
    public interface IXmlLoaderService
    {
        /// <summary>
        ///     Generates inventory in local Database from supplied Xml files.
        /// </summary>
        /// <param name="languagePaths"></param>
        /// <param name="inventoryPath"></param>
        Task UpdateInventory(ICollection<string> languagePaths, string inventoryPath);
    }
}