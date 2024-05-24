using System.Xml.Linq;
using Fashionhero.Portal.Shared.Abstraction.Enums;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Entity;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Persistence;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Service;
using Fashionhero.Portal.Shared.Model.Entity;
using Fashionhero.Portal.Shared.Model.Searchable;
using Microsoft.Extensions.Logging;

namespace Fashionhero.Portal.BusinessLogic
{
    public class SpartooService : IXmlExportService
    {
        private readonly ILogger<SpartooService> logger;
        private readonly IEntityQueryManager<Product, SearchableProduct> productManager;
        private const string SPARTOO_ROOT = "root";
        private const string SPARTOO_PRODUCTS = "products";
        private readonly Dictionary<string, char> productGenderMap;
        private readonly Dictionary<string, int> productStyleMap;

        public SpartooService(
            ILogger<SpartooService> logger, IEntityQueryManager<Product, SearchableProduct> productManager)
        {
            this.logger = logger;
            this.productManager = productManager;
            productGenderMap = new Dictionary<string, char>()
            {
                {"Mand", 'H'},
                {"Unisex", 'M'},
                {"Børn", 'K'},
                {"Dame", 'F'},
            };
            productStyleMap = new Dictionary<string, int>()
            {
                {"Bjørn Borg", 11475},
                // ...
            };
        }

        /// <inheritdoc />
        public async Task<XDocument> GenerateXmlDocument()
        {
            var databaseProducts = await productManager.GetEntities(new SearchableProduct());
            var validatedProducts = await DiscardInvalidProducts(databaseProducts.ToList());

            var xProducts = new XElement(SPARTOO_PRODUCTS);
            var xRoot = new XElement(SPARTOO_ROOT, xProducts);
            var xmlDocument = new XDocument(xRoot);

            return xmlDocument;
        }

        private async Task<ICollection<Product>> DiscardInvalidProducts(ICollection<Product> databaseProducts)
        {
            var productsImageFiltered = FilteringBasedOnImageUrl(databaseProducts);
            var productsCurrencyFiltered = FilteringBasedOnPriceCurrency(productsImageFiltered);
            var productsGenderFiltered = FilteringBasedOnDanishLocaleGender(productsCurrencyFiltered);
            var productsTypeFiltered = FilteredBasedOnProductType(productsGenderFiltered);

            logger.LogInformation($"Count After filtering away invalid Products: {productsGenderFiltered.Count}");
            return productsGenderFiltered;
        }

        private ICollection<Product> FilteredBasedOnProductType(ICollection<Product> productsGenderFiltered)
        {
            logger.LogInformation(
                $"Filtering away Products without a Danish translation containing a valid gender that can be translatable to a Product Sex/Gender. Current count: {productsCurrencyFiltered.Count}");
            var productsTypeFiltered = productsGenderFiltered.Where(x =>
            {
                try
                {
                    ILocaleProduct locale =
                        x.Locales.FirstOrDefault(z => z.IsoName == Shared.Model.Constants.DANISH_ISO_NAME) ??
                        throw new ArgumentException(
                            $"Failed to fine a Danish Translation of products, for filtering by {nameof(LocaleProduct.Gender)}.");


                    logger.LogWarning($"Discarding {nameof(Product)} ({x.ReferenceId})," +
                                      $" as the Danish Localised {nameof(LocaleProduct.Type)} ({locale.Type}) is not convertible to a {nameof(Product)} Style number.");
                    return false;
                }
                catch (Exception e)
                {
                    logger.LogWarning(e,
                        $"Discarding {nameof(Product)} ({x.ReferenceId}), as some error occured during filtering by {nameof(LocaleProduct.Gender)}.");
                    return false;
                }
            }).ToList();
            return productsGenderFiltered;
        }

        private ICollection<Product> FilteringBasedOnDanishLocaleGender(ICollection<Product> productsCurrencyFiltered)
        {
            logger.LogInformation(
                $"Filtering away Products without a Danish translation containing a valid gender that can be translatable to a Product Sex/Gender. Current count: {productsCurrencyFiltered.Count}");
            var productsGenderFiltered = productsCurrencyFiltered.Where(x =>
            {
                try
                {
                    ILocaleProduct locale =
                        x.Locales.FirstOrDefault(z => z.IsoName == Shared.Model.Constants.DANISH_ISO_NAME) ??
                        throw new ArgumentException(
                            $"Failed to fine a Danish Translation of products, for filtering by {nameof(LocaleProduct.Gender)}.");

                    if (productGenderMap.Keys.Any(z =>
                            string.Equals(locale.Gender, z, StringComparison.InvariantCultureIgnoreCase)))
                        return true;

                    logger.LogWarning($"Discarding {nameof(Product)} ({x.ReferenceId})," +
                                      $" as the Danish Localised {nameof(LocaleProduct.Gender)} ({locale.Gender}) is not convertible to a {nameof(Product)} Sex/Gender." +
                                      $" Expected values: {string.Join(", ", productGenderMap.Keys)}.");
                    return false;
                }
                catch (Exception e)
                {
                    logger.LogWarning(e,
                        $"Discarding {nameof(Product)} ({x.ReferenceId}), as some error occured during filtering by {nameof(LocaleProduct.Gender)}.");
                    return false;
                }
            }).ToList();
            return productsGenderFiltered;
        }

        private ICollection<Product> FilteringBasedOnPriceCurrency(ICollection<Product> productsImageFiltered)
        {
            logger.LogInformation(
                $"Filtering away Products without a DKK price. Current count: {productsImageFiltered.Count}");
            var productsCurrencyFiltered = productsImageFiltered.Where(x =>
            {
                if (x.Prices.Any(z => z.Currency == CurrencyCode.DKK))
                    return true;

                logger.LogWarning(
                    $"Discarding {nameof(Product)} ({x.ReferenceId}), as it does not have a {CurrencyCode.DKK} price.");
                return false;
            }).ToList();
            return productsCurrencyFiltered;
        }

        private ICollection<Product> FilteringBasedOnImageUrl(ICollection<Product> databaseProducts)
        {
            logger.LogInformation(
                $"Filtering away Products without valid images. Current count: {databaseProducts.Count}.");
            var productsImageFiltered = databaseProducts.Where(x =>
            {
                if (x.Images.Any(z => z.Url.EndsWith(".jpg")))
                    return true;

                logger.LogWarning(
                    $"Discarding {nameof(Product)} ({x.ReferenceId}), as it does not have any .jpg images.");
                return false;
            }).ToList();
            return productsImageFiltered;
        }
    }
}