namespace Fashionhero.Portal.Shared.Abstraction.Interfaces.Service
{
    public interface IXmlLoaderService
    {
        /// <summary>
        ///     Generates inventory in local Database from supplied Xml files.
        /// </summary>
        /// <param name="languageXmls"></param>
        /// <param name="inventoryXml"></param>
        Task UpdateInventory(Dictionary<string, string> languageXmls, string inventoryXml);
    }
}