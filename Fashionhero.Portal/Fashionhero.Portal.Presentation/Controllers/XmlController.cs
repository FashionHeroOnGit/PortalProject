using System.Text;
using System.Xml.Linq;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;
using FileIO = System.IO.File;

namespace Fashionhero.Portal.Presentation.Controllers
{
    [Route(Constants.ROUTE_TEMPLATE)]
    [ApiController]
    public class XmlController : ControllerBase
    {
        private readonly IXmlExportService exporter;
        private readonly IXmlLoaderService loader;
        private readonly ILogger<XmlController> logger;

        public XmlController(ILogger<XmlController> logger, IXmlLoaderService loader, IXmlExportService exporter)
        {
            this.logger = logger;
            this.loader = loader;
            this.exporter = exporter;
        }

        private static string GetIsoName(string languagePath)
        {
            string[] split = new FileInfo(languagePath).Name.Split('_');
            return split.Length == 1 ? Shared.Model.Constants.DANISH_ISO_NAME : split[0];
        }

        [HttpGet]
        public async Task<IActionResult> GetSpartooFile()
        {
            const string contentType = "text/xml";
            const string fileName = $"ExportToSpartoo.xml";

            try
            {
                XDocument document = await exporter.GenerateXmlDocument();

                if (Encoding.UTF8 is not UTF8Encoding encoding)
                    throw new Exception("Failed to create UTF-8 Encoding object.");

                byte[] bytes = encoding.GetBytes(document.ToString());

                return File(bytes, contentType, fileName);
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Error occured during execution of {nameof(GetSpartooFile)}.");
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateInventory([FromBody] UpdateInventoryArguments args)
        {
            try
            {
                if (args.LanguagePaths.Any(x => !x.EndsWith(".xml")))
                    throw new ArgumentException("One or more of the supplied language files are not an .xml file.");
                if (!args.InventoryPath.EndsWith(".xml"))
                    throw new ArgumentException("The supplied inventory path is not an .xml file.");

                var languageXmls = args.LanguagePaths.Select(FileIO.ReadAllText).ToList();
                var isoNames = args.LanguagePaths.Select(GetIsoName);
                string inventoryXml = await FileIO.ReadAllTextAsync(args.InventoryPath);
                var isoLanguageXmls = isoNames.Zip(languageXmls).ToDictionary(x => x.First, x => x.Second);

                await loader.UpdateInventory(isoLanguageXmls, inventoryXml);
                return Ok();
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Error occured during execution of {nameof(UpdateInventory)}.");
                throw;
            }
        }

        public class UpdateInventoryArguments
        {
            public string[] LanguagePaths { get; set; }
            public string InventoryPath { get; set; }
        }
    }
}