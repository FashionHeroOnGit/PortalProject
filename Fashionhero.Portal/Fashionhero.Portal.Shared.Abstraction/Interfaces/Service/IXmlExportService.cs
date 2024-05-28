using System.Xml.Linq;

namespace Fashionhero.Portal.Shared.Abstraction.Interfaces.Service
{
    public interface IXmlExportService
    {
        /// <summary>
        ///     Generates a Xml file at the specified output location, with structure corresponding to the instance supplied.
        /// </summary>
        Task<XDocument> GenerateXmlDocument();
    }
}