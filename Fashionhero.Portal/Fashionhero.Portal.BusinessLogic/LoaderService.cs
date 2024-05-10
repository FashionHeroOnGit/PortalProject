using System.Xml;
using System.Xml.Linq;
using Fashionhero.Portal.BusinessLogic.Extensions;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Entity;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Service;
using Fashionhero.Portal.Shared.Model.Entity;
using Microsoft.Extensions.Logging;

namespace Fashionhero.Portal.BusinessLogic
{
    public class LoaderService : IXmlLoaderService
    {
        private readonly ILogger<LoaderService> logger;
        private const string LOCALE_ROOT = "products";
        private const string LOCALE_SINGLE_PRODUCT = "product";
        private const string LOCALE_ID = "id";
        private const string LOCALE_TITLE = "title";
        private const string LOCALE_DESCRIPTION = "description";
        private const string LOCALE_ITEM_GROUP_ID = "item_group_id";
        private const string LOCALE_PRODUCT_TYPE = "product_type";
        private const string LOCALE_PRODUCT_TYPE_LOCAL = "product_type_local";
        private const string LOCALE_COLOR = "color";
        private const string LOCALE_GENDER = "gender";
        private const string LOCALE_MATERIALE = "materiale";
        private const string LOCALE_COUNTRY_OF_ORIGIN = "country_of_origin";


        public LoaderService(ILogger<LoaderService> logger)
        {
            this.logger = logger;
        }

        /// <inheritdoc />
        public async Task GenerateInventory(ICollection<string> languagePaths, string inventoryPath)
        {
            var localeProducts = await ProcessLanguagePaths(languagePaths);
        }

        private async Task<ICollection<ILocaleProduct>> ProcessLanguagePaths(ICollection<string> languagePaths)
        {
            if (languagePaths.Any(x => !x.EndsWith(".xml")))
                throw new ArgumentException("One or more of the supplied language files are not an .xml file.");

            var generateTasks = languagePaths.Select(GenerateLocaleProducts);
            var twoDimensionalLocaleProduct = await Task.WhenAll(generateTasks);

            return twoDimensionalLocaleProduct.SelectMany(x => x).ToList();
        }

        private async Task<ICollection<ILocaleProduct>> GenerateLocaleProducts(string languagePath)
        {
            XDocument document = await XDocument.LoadAsync(
                XmlReader.Create(File.OpenRead(languagePath), new XmlReaderSettings {Async = true,}),
                LoadOptions.PreserveWhitespace, CancellationToken.None);

            var xmlProducts = document.Elements(LOCALE_ROOT).Elements(LOCALE_SINGLE_PRODUCT);

            return xmlProducts.Select(x => GenerateLocaleProduct(x, languagePath)).ToList();
        }

        private ILocaleProduct GenerateLocaleProduct(XElement element, string languagePath)
        {
            return new LocaleProduct
            {
                IsoName = GetIsoName(languagePath),
                ReferenceId = element.Element(LOCALE_ID).GetValueAsInt(),
                Title = element.Element(LOCALE_TITLE).GetValue(),
                Description = element.Element(LOCALE_DESCRIPTION).GetValue(),
                ItemGroupId = element.Element(LOCALE_ITEM_GROUP_ID).GetValueAsInt(),
                Type = element.Element(LOCALE_PRODUCT_TYPE).GetValue(),
                LocalType = element.Element(LOCALE_PRODUCT_TYPE_LOCAL).GetValue(),
                Colour = element.Element(LOCALE_COLOR).GetValue(),
                Gender = element.Element(LOCALE_GENDER).GetValue(),
                Material = element.Element(LOCALE_MATERIALE).GetValue(),
                CountryOrigin = element.Element(LOCALE_COUNTRY_OF_ORIGIN).GetValue(),
            };
        }

        private string GetIsoName(string languagePath)
        {
            string[] split = new FileInfo(languagePath).Name.Split('_');
            return split.Length == 1 ? "dk" : split[0];
        }
    }
}