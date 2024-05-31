using System.Xml.Linq;
using Fashionhero.Portal.BusinessLogic.Spartoo;
using Fashionhero.Portal.Shared.Abstraction.Enums;
using Fashionhero.Portal.Shared.Abstraction.Interfaces;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Entity;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Persistence;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Service;
using Fashionhero.Portal.Shared.Model;
using Fashionhero.Portal.Shared.Model.Entity;
using Fashionhero.Portal.Shared.Model.Searchable;
using Microsoft.Extensions.Logging;

namespace Fashionhero.Portal.BusinessLogic.Services
{
    public class SpartooService : IXmlExportService
    {
        private readonly ICurrencyConverterService converterService;
        private readonly ILogger<SpartooService> logger;
        private readonly IEntityQueryManager<Product, SearchableProduct> productManager;

        private List<IFilter> filters;
        private List<IMapper> mappers;

        public SpartooService(
            ILogger<SpartooService> logger, IEntityQueryManager<Product, SearchableProduct> productManager,
            ICurrencyConverterService converterService, ILoggerFactory loggerFactory)
        {
            this.logger = logger;
            this.productManager = productManager;
            this.converterService = converterService;

            InitializeDataProcessors(loggerFactory);
        }

        /// <inheritdoc />
        public async Task<XDocument> GenerateXmlDocument()
        {
            var databaseProducts = await productManager.GetEntities(new SearchableProduct());
            var filteredProducts = await DiscardInvalidProducts(databaseProducts.ToList());
            var generateTasks = filteredProducts.Select(BuildXmlProduct);

            var productElements = (await Task.WhenAll(generateTasks)).ToList();
            var productsElement = new XElement(XmlTagConstants.SPARTOO_PRODUCTS_ROOT, productElements);
            var rootElement = new XElement(XmlTagConstants.SPARTOO_ROOT, productsElement);
            var xmlDocument = new XDocument(rootElement);

            return xmlDocument;
        }

        private static Task<XElement> BuildXmlPhotos(IImage image, int index)
        {
            return Task.FromResult(new XElement(XmlTagConstants.SpartooPhotoUrl(index + 1), image.Url));
        }

        private static Task<XElement> BuildXmlSize(ISize size)
        {
            var sizeNameElement = new XElement(XmlTagConstants.SPARTOO_SIZE_NAME, size.Primary);
            var sizeQuantityElement = new XElement(XmlTagConstants.SPARTOO_SIZE_QUANTITY, size.Quantity);
            var sizeReferenceElement =
                new XElement(XmlTagConstants.SPARTOO_SIZE_REFERENCE,
                    size.ModelProductNumber); // Todo: figure out what value this element should have.
            var eanElement = new XElement(XmlTagConstants.SPARTOO_EAN, size.Ean);

            var sizeElement = new XElement(XmlTagConstants.SPARTOO_SIZE, sizeNameElement, sizeQuantityElement,
                sizeReferenceElement, eanElement);

            return Task.FromResult(sizeElement);
        }

        private async Task<XElement> BuildXmlLanguage(ILocaleProduct localeProduct)
        {
            var isoCodeElement = new XElement(XmlTagConstants.SPARTOO_CODE, localeProduct.IsoName);
            var productNameElement = new XElement(XmlTagConstants.SPARTOO_PRODUCT_NAME, localeProduct.Title);
            var productDescriptionElement =
                new XElement(XmlTagConstants.SPARTOO_PRODUCT_DESCRIPTION, localeProduct.Description);
            var productColourElement = new XElement(XmlTagConstants.SPARTOO_PRODUCT_COLOR, localeProduct.Colour);

            IPrice dkkPrice = localeProduct.Product.Prices.First(x => x.Currency == CurrencyCode.DKK);
            IPrice convertedPrice = await converterService.ConvertPrice(dkkPrice, CurrencyCode.EUR);
            if (convertedPrice.NormalSell > 400)
            {
                decimal rate = await converterService.GetRate(CurrencyCode.DKK, CurrencyCode.EUR);
                logger.LogWarning($"Price after currency conversion wound up above 400 EUR." +
                                  $" Original DKK: {dkkPrice.NormalSell}. Rate used: {rate}, {nameof(Product)}: {localeProduct.ItemGroupId}");
            }

            var productPriceElement = new XElement(XmlTagConstants.SPARTOO_PRODUCT_PRICE, convertedPrice.NormalSell);
            // Todo: Make 'discount' element somehow...

            var languageElement = new XElement(XmlTagConstants.SPARTOO_LANGUAGE, isoCodeElement, productNameElement,
                productDescriptionElement, productColourElement, productPriceElement);

            return languageElement;
        }

        private async Task<XElement> BuildXmlProduct(Product product)
        {
            ILocaleProduct locale = product.Locales.First(x =>
                string.Equals(x.IsoName, Constants.DANISH_ISO_NAME, StringComparison.InvariantCultureIgnoreCase));
            object productSex = mappers.First(x => x.IsMapperOfType(MapType.SPARTOO_GENDER))
                .GetDictionaryValue(locale.Gender);
            object colourId = mappers.First(x => x.IsMapperOfType(MapType.SPARTOO_COLOUR))
                .GetDictionaryValue(locale.Colour);
            object productStyle = mappers.First(x => x.IsMapperOfType(MapType.SPARTOO_TYPE))
                .GetDictionaryValue(locale.Type);

            var languageGenerationTask = product.Locales.Select(BuildXmlLanguage);
            var sizeGenerationTask = product.Sizes.Select(BuildXmlSize);
            var imageGenerationTasks = product.Images.Where(x => x.Url.EndsWith(".jpg")).Take(8).Select(BuildXmlPhotos);

            var languageElements = (await Task.WhenAll(languageGenerationTask)).ToList();
            var sizeElements = (await Task.WhenAll(sizeGenerationTask)).ToList();
            var urlElements = (await Task.WhenAll(imageGenerationTasks)).ToList();

            var referenceElement = new XElement(XmlTagConstants.SPARTOO_REFERENCE_PARTNER, product.ModelProductNumber);
            var manufacturerElement = new XElement(XmlTagConstants.SPARTOO_MANUFACTURERS_NAME, product.Manufacturer);
            var productSexElement = new XElement(XmlTagConstants.SPARTOO_PRODUCT_SEX, productSex);
            var productQuantityElement = new XElement(XmlTagConstants.SPARTOO_PRODUCT_QUANTITY, product.TotalQuantity);
            var colourIdElement = new XElement(XmlTagConstants.SPARTOO_COLOUR_ID, colourId);
            var productStyleElement = new XElement(XmlTagConstants.SPARTOO_PRODUCT_STYLE, productStyle);
            var countryOriginElement = new XElement(XmlTagConstants.SPARTOO_COUNTRY_ORIGIN, locale.CountryOrigin);
            var languagesElement = new XElement(XmlTagConstants.SPARTOO_LANGUAGES, languageElements);
            var sizeListElement = new XElement(XmlTagConstants.SPARTOO_SIZE_LIST, sizeElements);
            var photosElement = new XElement(XmlTagConstants.SPARTOO_PHOTOS, urlElements);

            var productElement = new XElement(XmlTagConstants.SPARTOO_PRODUCT, referenceElement, manufacturerElement,
                productSexElement, productQuantityElement, colourIdElement, productStyleElement, countryOriginElement,
                languagesElement, sizeListElement, photosElement);

            return productElement;
        }


        private Task<ICollection<Product>> DiscardInvalidProducts(ICollection<Product> databaseProducts)
        {
            var filteredProducts = filters.Aggregate(databaseProducts,
                (current, filter) => filter.FilterProducts(current.Cast<IProduct>().ToList()).Cast<Product>().ToList());

            logger.LogInformation($"Count After filtering away invalid Products: {filteredProducts.Count}");
            return Task.FromResult(filteredProducts);
        }

        private void InitializeDataProcessors(ILoggerFactory loggerFactory)
        {
            var genderMappedFilter = new GenderMappedFilter(loggerFactory.CreateLogger<GenderMappedFilter>());
            var productTypeMappedFilter =
                new ProductTypeMappedFilter(loggerFactory.CreateLogger<ProductTypeMappedFilter>());

            filters =
            [
                genderMappedFilter,
                productTypeMappedFilter,
                new ImageFilter(loggerFactory.CreateLogger<ImageFilter>()),
                new ModelProductNumberFilter(loggerFactory.CreateLogger<ModelProductNumberFilter>()),
                new CurrencyFilter(loggerFactory.CreateLogger<CurrencyFilter>()),
                new SizeFilter(loggerFactory.CreateLogger<SizeFilter>()),
            ];
            mappers =
            [
                new ColourMapper(),
                genderMappedFilter,
                productTypeMappedFilter,
            ];
        }
    }
}