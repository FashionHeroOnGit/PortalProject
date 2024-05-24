using System.Xml.Linq;
using Fashionhero.Portal.BusinessLogic.Extensions;
using Fashionhero.Portal.Shared.Abstraction.Enums;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Entity;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Persistence;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Service;
using Fashionhero.Portal.Shared.Model.Entity;
using Fashionhero.Portal.Shared.Model.Searchable;
using Microsoft.Extensions.Logging;

namespace Fashionhero.Portal.BusinessLogic
{
    public class LoaderService : IXmlLoaderService
    {
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

        private const string INVENTORY_ROOT = "products";
        private const string INVENTORY_SINGLE_PRODUCT = "product";
        private const string INVENTORY_ID = "id";
        private const string INVENTORY_LINK = "link";
        private const string INVENTORY_IMAGE_LINK = "image_link";

        private const string
            INVENTORY_AVAILABILITY = "availability"; // Unused as 'Availability' is calculated from stock

        private const string INVENTORY_REGULAR_PRICE = "regular_price";
        private const string INVENTORY_SALE_PRICE = "sale_price";
        private const string INVENTORY_PRICE_EUR = "price_eur";
        private const string INVENTORY_SALE_PRICE_EUR = "sale_price_eur";
        private const string INVENTORY_PRICE_SEK = "price_sek";
        private const string INVENTORY_SALE_PRICE_SEK = "sale_price_sek";
        private const string INVENTORY_PRICE_PLN = "price_pln";
        private const string INVENTORY_SALE_PRICE_PLN = "sale_price_pln";
        private const string INVENTORY_MODEL_PRODUCT_NUMBER = "mpn";
        private const string INVENTORY_BRAND = "brand";
        private const string INVENTORY_EAN = "gtin";
        private const string INVENTORY_STOCK = "stock";
        private const string INVENTORY_SIZE_A = "size-a";
        private const string INVENTORY_SIZE_B = "size-b";
        private const string INVENTORY_SPARTOO_KODE = "spartoo-kode";
        private const string INVENTORY_PRODUCT_CATEGORY = "product_category";
        private readonly ILogger<LoaderService> logger;
        private readonly IEntityQueryManager<Product, SearchableProduct> productManager;


        public LoaderService(
            ILogger<LoaderService> logger, IEntityQueryManager<Product, SearchableProduct> productManager)
        {
            this.logger = logger;
            this.productManager = productManager;
        }

        /// <inheritdoc />
        public async Task UpdateInventory(Dictionary<string, string> languageXmls, string inventoryXml)
        {
            var loadedLocaleProducts = await ProcessLanguageXml(languageXmls);
            var loadedSizes = await GetSizes(inventoryXml);
            var cleanedLoadedSizes = await CleanLoadedSizes(loadedSizes);
            var loadedProducts = await GetMasterProducts(inventoryXml, loadedLocaleProducts, cleanedLoadedSizes);
            var cleanedLoadedProducts = await CleanLoadedProducts(loadedProducts);

            var databaseProducts = (await productManager.GetEntities(new SearchableProduct())).ToList();
            var loadedReferenceIds = cleanedLoadedProducts.Select(x => x.ReferenceId).ToList();
            var databaseReferenceIds = databaseProducts.Select(x => x.ReferenceId).ToList();

            var productsToUpdate = databaseProducts.Where(x => loadedReferenceIds.Contains(x.ReferenceId)).ToList();
            var productsToAdd = cleanedLoadedProducts.Where(x => !databaseReferenceIds.Contains(x.ReferenceId))
                .ToList();
            var productsToDelete = databaseProducts.Where(x => !loadedReferenceIds.Contains(x.ReferenceId)).ToList();

            var updateReferenceIds = productsToUpdate.Select(x => x.ReferenceId).ToList();
            var loadedProductsForUpdating =
                cleanedLoadedProducts.Where(x => updateReferenceIds.Contains(x.ReferenceId)).ToList();

            var mappedProductsToUpdate =
                await MapLoadedProductsToDatabaseProducts(loadedProductsForUpdating, productsToUpdate);

            await productManager.DeleteEntities(productsToDelete);
            await productManager.UpdateEntities(mappedProductsToUpdate.Cast<Product>().ToList());
            await productManager.AddEntities(productsToAdd.Cast<Product>().ToList());
        }

        private IProduct AddNewChildren(IProduct loadedProduct, IProduct databaseProduct)
        {
            databaseProduct.Locales = AddNewLocaleProducts(loadedProduct, databaseProduct);
            databaseProduct.Sizes = AddNewSizes(loadedProduct, databaseProduct);
            databaseProduct.ExtraTags = AddNewTags(loadedProduct, databaseProduct);
            //databaseProduct.Images = AddNewImages(loadedProduct, databaseProduct); // Commented, as Images are currently just overwritten.
            databaseProduct.Prices = AddNewPrices(loadedProduct, databaseProduct);

            return databaseProduct;
        }

        private ICollection<IImage> AddNewImages(IProduct loadedProduct, IProduct databaseProduct)
        {
            var currentImageUrls = databaseProduct.Images.Select(x => x.Url).ToList();
            var imagesList = databaseProduct.Images.ToList();
            if (loadedProduct.Images.All(x => currentImageUrls.Contains(x.Url)))
                return imagesList;

            imagesList.AddRange(loadedProduct.Images.Where(x => !currentImageUrls.Contains(x.Url)));

            if (currentImageUrls.Count * 2 <= imagesList.Count)
                logger.LogWarning(
                    $"Large growth of {nameof(Image)} on {nameof(Product)} ({databaseProduct.ReferenceId}) detected. " +
                    $"Grew from {currentImageUrls.Count} to {imagesList.Count}.");

            return imagesList;
        }

        private ICollection<ILocaleProduct> AddNewLocaleProducts(IProduct loadedProduct, IProduct databaseProduct)
        {
            var currentLocaleProductIsoNames = databaseProduct.Locales.Select(x => x.IsoName).ToList();
            var localeProductsList = databaseProduct.Locales.ToList();
            if (loadedProduct.Locales.All(x => currentLocaleProductIsoNames.Contains(x.IsoName)))
                return localeProductsList;

            localeProductsList.AddRange(loadedProduct.Locales.Where(x =>
                !currentLocaleProductIsoNames.Contains(x.IsoName)));

            if (currentLocaleProductIsoNames.Count * 2 <= localeProductsList.Count)
                logger.LogWarning(
                    $"Large growth of {nameof(LocaleProduct)} on {nameof(Product)} ({databaseProduct.ReferenceId}) detected. " +
                    $"Grew from {currentLocaleProductIsoNames.Count} to {localeProductsList.Count}.");
            return localeProductsList;
        }

        private ICollection<IPrice> AddNewPrices(IProduct loadedProduct, IProduct databaseProduct)
        {
            var currentPriceCurrencies = databaseProduct.Prices.Select(x => x.Currency).ToList();
            var pricesList = databaseProduct.Prices.ToList();
            if (loadedProduct.Prices.All(x => currentPriceCurrencies.Contains(x.Currency)))
                return pricesList;

            pricesList.AddRange(loadedProduct.Prices.Where(x => !currentPriceCurrencies.Contains(x.Currency)));

            if (currentPriceCurrencies.Count * 2 <= pricesList.Count)
                logger.LogWarning(
                    $"Large growth of {nameof(Image)} on {nameof(Product)} ({databaseProduct.ReferenceId}) detected. " +
                    $"Grew from {currentPriceCurrencies.Count} to {pricesList.Count}.");

            return pricesList;
        }

        private ICollection<ISize> AddNewSizes(IProduct loadedProduct, IProduct databaseProduct)
        {
            var currentSizeReferenceIds = databaseProduct.Sizes.Select(x => x.ReferenceId).ToList();
            var sizesList = databaseProduct.Sizes.ToList();

            if (loadedProduct.Sizes.All(x => currentSizeReferenceIds.Contains(x.ReferenceId)))
                return sizesList;

            sizesList.AddRange(loadedProduct.Sizes.Where(x => !currentSizeReferenceIds.Contains(x.ReferenceId)));

            if (currentSizeReferenceIds.Count * 2 <= sizesList.Count)
                logger.LogWarning(
                    $"Large growth of {nameof(Size)} on {nameof(Product)} ({databaseProduct.ReferenceId}) detected. " +
                    $"Grew from {currentSizeReferenceIds.Count} to {sizesList.Count}.");

            return sizesList;
        }

        private ICollection<ITag> AddNewTags(IProduct loadedProduct, IProduct databaseProduct)
        {
            var currentTagNames = databaseProduct.ExtraTags.Select(x => x.Name).ToList();
            var tagsList = databaseProduct.ExtraTags.ToList();
            if (loadedProduct.ExtraTags.All(x => currentTagNames.Contains(x.Name)))
                return tagsList;

            tagsList.AddRange(loadedProduct.ExtraTags.Where(x => !currentTagNames.Contains(x.Name)));

            if (currentTagNames.Count * 2 <= tagsList.Count)
                logger.LogWarning(
                    $"Large growth of {nameof(Tag)} on {nameof(Product)} ({databaseProduct.ReferenceId}) detected. " +
                    $"Grew from {currentTagNames.Count} to {tagsList.Count}.");

            return tagsList;
        }

        private bool ChooseSizeXmlElements(XElement element)
        {
            XElement linkElement = element.GetTaggedElement(INVENTORY_LINK);
            XElement eanElement = element.GetTaggedElement(INVENTORY_EAN);
            XElement primarySizeElement = element.GetTaggedElement(INVENTORY_SIZE_A);

            bool containsQuestionMark = linkElement.Value.Contains('?');
            bool isEmptyEan = eanElement.IsEmptyValue();

            if (containsQuestionMark && !isEmptyEan)
                return true;
            if (eanElement.Value.Contains("X23"))
                return true;
            return !primarySizeElement.Value.Contains(',') && !containsQuestionMark && !isEmptyEan;
        }

        private async Task<ICollection<IProduct>> CleanLoadedProducts(ICollection<IProduct> rawLoadedProducts)
        {
            var filteredLoadedProducts = rawLoadedProducts.Where(x =>
            {
                if (x.Sizes.Count != 0)
                    return true;

                logger.LogWarning(
                    $"Discarding {nameof(Product)} ({x.ReferenceId}), as it failed to get any sizes attached.");
                return false;
            }).ToList();

            return await DiscardProductsWithDuplicateLink(filteredLoadedProducts);
        }

        private async Task<ICollection<ISize>> CleanLoadedSizes(ICollection<ISize> rawLoadedSizes)
        {
            var filteredLoadedSizes = rawLoadedSizes.Where(x =>
            {
                if (x.Quantity != default)
                    return true;

                logger.LogWarning(
                    $"Discarding {nameof(Size)} ({x.ReferenceId}) from loaded data, as it has none left.");
                return false;
            }).Where(x =>
            {
                if (x.Ean != default)
                    return true;

                logger.LogWarning(
                    $"Discarding {nameof(Size)} ({x.ReferenceId}) from loaded data, as it is missing a valid Ean number.");
                return false;
            }).ToList();

            return await DiscardSizesWithDuplicateEan(filteredLoadedSizes);
        }

        private IProduct ClearRemovedChildren(IProduct loadedProduct, IProduct databaseProduct)
        {
            var toDeleteLocaleProductReferenceIds =
                GetExceptedList(databaseProduct.Locales, loadedProduct.Locales, x => x.ReferenceId);
            databaseProduct.Locales = databaseProduct.Locales
                .Where(x => !toDeleteLocaleProductReferenceIds.Contains(x.ReferenceId)).ToList();

            var toDeleteSizeReferenceIds = GetExceptedList(databaseProduct.Sizes, loadedProduct.Sizes,
                x => x.ReferenceId);
            databaseProduct.Sizes = databaseProduct.Sizes.Where(x => !toDeleteSizeReferenceIds.Contains(x.ReferenceId))
                .ToList();

            var toDeleteTagNames = GetExceptedList(databaseProduct.ExtraTags, loadedProduct.ExtraTags, x => x.Name);
            databaseProduct.ExtraTags =
                databaseProduct.ExtraTags.Where(x => !toDeleteTagNames.Contains(x.Name)).ToList();

            //var toDeleteImageUrl = GetExceptedList(databaseProduct.Images, loadedProduct.Images, x => x.Url);
            //databaseProduct.Images = databaseProduct.Images.Where(x => !toDeleteImageUrl.Contains(x.Url)).ToList();

            var toDeletePriceCurrency = GetExceptedList(databaseProduct.Prices, loadedProduct.Prices, x => x.Currency);
            databaseProduct.Prices =
                databaseProduct.Prices.Where(x => !toDeletePriceCurrency.Contains(x.Currency)).ToList();

            return databaseProduct;
        }

        private ICollection<ITag> DiscardEmptyTags(ICollection<ITag> originalTags)
        {
            return originalTags.Where(x =>
            {
                if (!string.IsNullOrWhiteSpace(x.Value))
                    return true;

                logger.LogWarning(
                    $"Discarding {nameof(Tag)} ({x.Name}) for {nameof(Product)} ({x.ReferenceId}) as the {nameof(Tag.Value)} is empty.");
                return false;
            }).ToList();
        }

        private ICollection<IPrice> DiscardInvalidPrices(ICollection<IPrice> originalPrices)
        {
            return originalPrices.Where(x =>
            {
                if (x.NormalSell != 0 || (x.Discount != null && x.Discount != 0))
                    return true;

                logger.LogWarning(
                    $"Discarding {nameof(Price)} ({x.Currency}) for {nameof(Product)} ({x.ReferenceId}), as it needs either a normal or discount listing price to be valid.");
                return false;
            }).ToList();
        }

        private Task<ICollection<IProduct>> DiscardProductsWithDuplicateLink(ICollection<IProduct> loadedProducts)
        {
            var linkCounter = new Dictionary<string, ICollection<IProduct>>();
            foreach (IProduct loadedProduct in loadedProducts)
            {
                if (!linkCounter.ContainsKey(loadedProduct.LinkBase))
                    linkCounter.Add(loadedProduct.LinkBase, new List<IProduct>());
                linkCounter[loadedProduct.LinkBase].Add(loadedProduct);
            }

            var result = new List<IProduct>();

            foreach (var linkCount in linkCounter)
            {
                if (linkCount.Value.Count > 1)
                {
                    string referenceIds = string.Join(", ", linkCount.Value.Skip(1).Select(x => x.ReferenceId));
                    logger.LogWarning(
                        $"Multiple Products with the same {nameof(Product.LinkBase)} ({linkCount.Key}) exist. " +
                        $"Discarding all but the first found {nameof(Product)}. Discarded Reference Id's: {referenceIds}");
                }

                result.Add(linkCount.Value.First());
            }

            return Task.FromResult(result as ICollection<IProduct>);
        }

        private Task<ICollection<ISize>> DiscardSizesWithDuplicateEan(ICollection<ISize> sizes)
        {
            var eanCounter = new Dictionary<long, ICollection<ISize>>();
            foreach (ISize size in sizes)
            {
                if (!eanCounter.ContainsKey(size.Ean))
                    eanCounter.Add(size.Ean, new List<ISize>());
                eanCounter[size.Ean].Add(size);
            }

            var result = new List<ISize>();
            foreach (var eanCount in eanCounter)
            {
                if (eanCount.Value.Count > 1)
                {
                    string referenceIds = string.Join(", ", eanCount.Value.Skip(1).Select(x => x.ReferenceId));
                    logger.LogWarning(
                        $"Multiple {nameof(Product)} Sizes with same {nameof(Size.Ean)} ({eanCount.Key}) exist. " +
                        $"Discarding all but the first found {nameof(Size)}. Discarded Reference Id's: {referenceIds}");
                }

                result.Add(eanCount.Value.First());
            }

            return Task.FromResult(result as ICollection<ISize>);
        }

        private Task<LocaleProduct> GenerateLocaleProduct(XElement element, string isoName)
        {
            return Task.FromResult(new LocaleProduct
            {
                IsoName = isoName,
                ReferenceId = element.GetTagValueAsInt(LOCALE_ID, logger),
                Title = element.GetTagValueAsString(LOCALE_TITLE, logger),
                Description = element.GetTagValueAsString(LOCALE_DESCRIPTION, logger),
                ItemGroupId = element.GetTagValueAsInt(LOCALE_ITEM_GROUP_ID, logger),
                Type = element.GetTagValueAsString(LOCALE_PRODUCT_TYPE, logger),
                LocalType = element.GetTagValueAsString(LOCALE_PRODUCT_TYPE_LOCAL, logger),
                Colour = element.GetTagValueAsString(LOCALE_COLOR, logger),
                Gender = element.GetTagValueAsString(LOCALE_GENDER, logger),
                Material = element.GetTagValueAsString(LOCALE_MATERIALE, logger),
                CountryOrigin = element.GetTagValueAsString(LOCALE_COUNTRY_OF_ORIGIN, logger),
            });
        }

        private async Task<ICollection<ILocaleProduct>> GenerateLocaleProducts(
            KeyValuePair<string, string> isoLanguageXml)
        {
            XDocument document = GetDocument(isoLanguageXml.Value);

            var xmlLocaleProducts = document.Elements(LOCALE_ROOT).Elements(LOCALE_SINGLE_PRODUCT);
            var generateTasks = xmlLocaleProducts.Select(x => GenerateLocaleProduct(x, isoLanguageXml.Key));
            return await Task.WhenAll(generateTasks);
        }

        private Task<Product> GenerateProduct(
            XElement element, ICollection<ILocaleProduct> localeProducts, ICollection<ISize> sizes)
        {
            string[] splitLink = element.GetTagValueAsString(INVENTORY_LINK, logger).Split('?');
            var newProduct = new Product
            {
                ReferenceId = element.GetTagValueAsInt(INVENTORY_ID, logger),
                LinkBase = splitLink[0],
                Brand = element.GetTagValueAsString(INVENTORY_BRAND, logger),
                Category = element.GetTagValueAsString(INVENTORY_PRODUCT_CATEGORY, logger),
                Manufacturer = element.GetTagValueAsString(INVENTORY_BRAND, logger),
                Sizes = sizes.Where(x => x.LinkBase == splitLink[0]).ToList(),
                ExtraTags = GetExtraTags(element),
                Images = GetImages(element),
                Prices = GetPrices(element),
            };

            newProduct.Locales = localeProducts.Where(x => x.ItemGroupId == newProduct.ReferenceId).ToList();

            return Task.FromResult(newProduct);
        }

        private Task<Size> GenerateSize(XElement element)
        {
            string[] splitLink = element.GetTagValueAsString(INVENTORY_LINK, logger).Split('?');

            return Task.FromResult(new Size
            {
                Quantity = element.GetTagValueAsInt(INVENTORY_STOCK, logger),
                LinkBase = splitLink[0],
                LinkPostFix = splitLink.Length > 1 ? $"?{splitLink[1]}" : string.Empty,
                Ean = element.GetTaggedValueAsLong(INVENTORY_EAN, logger),
                ReferenceId = element.GetTagValueAsInt(INVENTORY_ID, logger),
                ModelProductNumber = element.GetTagValueAsString(INVENTORY_MODEL_PRODUCT_NUMBER, logger),
                Primary = element.GetTagValueAsString(INVENTORY_SIZE_A, logger),
                Secondary = element.GetTagValueAsString(INVENTORY_SIZE_B, logger),
            });
        }

        private static XDocument GetDocument(string xml)
        {
            return XDocument.Parse(xml, LoadOptions.PreserveWhitespace);
        }

        private List<TResult> GetExceptedList<TEntity, TResult>(
            ICollection<TEntity> sourceOne, ICollection<TEntity> sourceTwo, Func<TEntity, TResult> selector)
            where TEntity : IEntity
        {
            return sourceOne.Select(selector).Except(sourceTwo.Select(selector)).ToList();
        }

        private ICollection<ITag> GetExtraTags(XElement element)
        {
            var tags = new List<ITag>
            {
                new Tag
                {
                    Name = INVENTORY_SPARTOO_KODE,
                    Value = element.GetTagValueAsString(INVENTORY_SPARTOO_KODE, logger),
                    ReferenceId = element.GetTagValueAsInt(INVENTORY_ID, logger),
                },
            };

            return DiscardEmptyTags(tags);
        }

        private ICollection<IImage> GetImages(XElement element)
        {
            return new List<IImage>
            {
                new Image
                {
                    Url = element.GetTagValueAsString(INVENTORY_IMAGE_LINK, logger),
                    ReferenceId = element.GetTagValueAsInt(INVENTORY_ID, logger),
                },
            };
        }

        private async Task<ICollection<IProduct>> GetMasterProducts(
            string inventoryPath, ICollection<ILocaleProduct> localeProducts, ICollection<ISize> sizes)
        {
            XDocument document = GetDocument(inventoryPath);

            logger.LogWarning(
                $"{nameof(Product)}s' '{nameof(Product.Manufacturer)}' attribute is set to the value of the '{INVENTORY_BRAND}' tag, " +
                $"as the Spartoo description of the expected value sounds like it being the brand.");

            var xmlProducts = document.Elements(INVENTORY_ROOT).Elements(INVENTORY_SINGLE_PRODUCT).Where(x =>
            {
                XElement? element = x.Element(INVENTORY_LINK);
                return element != null && !element.Value.Contains('?');
            });
            var generateTasks = xmlProducts.Select(x => GenerateProduct(x, localeProducts, sizes));
            return await Task.WhenAll(generateTasks);
        }

        private ICollection<IPrice> GetPrices(XElement element)
        {
            var prices = new List<IPrice>
            {
                new Price
                {
                    NormalSell = element.GetTaggedValueAsFloat(INVENTORY_REGULAR_PRICE, logger),
                    Discount = element.GetTaggedValueAsFloat(INVENTORY_SALE_PRICE, logger),
                    Currency = CurrencyCode.DKK,
                    ReferenceId = element.GetTagValueAsInt(INVENTORY_ID, logger),
                },
                new Price
                {
                    NormalSell = element.GetTaggedValueAsFloat(INVENTORY_PRICE_EUR, logger),
                    Discount = element.GetTaggedValueAsFloat(INVENTORY_SALE_PRICE_EUR, logger),
                    Currency = CurrencyCode.EUR,
                    ReferenceId = element.GetTagValueAsInt(INVENTORY_ID, logger),
                },
                new Price
                {
                    NormalSell = element.GetTaggedValueAsFloat(INVENTORY_PRICE_SEK, logger),
                    Discount = element.GetTaggedValueAsFloat(INVENTORY_SALE_PRICE_SEK, logger),
                    Currency = CurrencyCode.SEK,
                    ReferenceId = element.GetTagValueAsInt(INVENTORY_ID, logger),
                },
                new Price
                {
                    NormalSell = element.GetTaggedValueAsFloat(INVENTORY_PRICE_PLN, logger),
                    Discount = element.GetTaggedValueAsFloat(INVENTORY_SALE_PRICE_PLN, logger),
                    Currency = CurrencyCode.PLN,
                    ReferenceId = element.GetTagValueAsInt(INVENTORY_ID, logger),
                },
            };

            return DiscardInvalidPrices(prices);
        }

        private async Task<ICollection<ISize>> GetSizes(string inventoryXml)
        {
            XDocument document = GetDocument(inventoryXml);

            var xmlSizes = document.Elements(INVENTORY_ROOT).Elements(INVENTORY_SINGLE_PRODUCT).ToList();
            foreach (XElement xmlSize in xmlSizes)
                ReportInvalidXmlFields(xmlSize);

            var generateTasks = xmlSizes.Where(ChooseSizeXmlElements).Select(GenerateSize).ToList();
            return await Task.WhenAll(generateTasks);
        }

        private Task<ILocaleProduct> MapLoadedLocaleProductsToDatabaseLocaleProduct(
            ICollection<ILocaleProduct> loadedLocaleProducts, ILocaleProduct databaseLocaleProduct,
            int parentReferenceId)
        {
            ILocaleProduct loadedLocaleProduct =
                loadedLocaleProducts.FirstOrDefault(x =>
                    x.ItemGroupId == parentReferenceId && x.IsoName == databaseLocaleProduct.IsoName &&
                    x.ReferenceId == databaseLocaleProduct.ReferenceId) ?? throw new ArgumentException(
                    $"Expected to find a loaded {nameof(LocaleProduct)} with following {nameof(ILocaleProduct.ItemGroupId)} ({parentReferenceId}), " +
                    $" {nameof(ILocaleProduct.IsoName)} ({databaseLocaleProduct.IsoName}) and {nameof(ILocaleProduct.ReferenceId)} ({databaseLocaleProduct.ReferenceId}), but none was found.");

            databaseLocaleProduct.Colour = loadedLocaleProduct.Colour;
            databaseLocaleProduct.CountryOrigin = loadedLocaleProduct.CountryOrigin;
            databaseLocaleProduct.Gender = loadedLocaleProduct.Gender;
            databaseLocaleProduct.Description = loadedLocaleProduct.Description;
            databaseLocaleProduct.Title = loadedLocaleProduct.Title;
            databaseLocaleProduct.Type = loadedLocaleProduct.Type;
            databaseLocaleProduct.LocalType = loadedLocaleProduct.LocalType;
            databaseLocaleProduct.Material = loadedLocaleProduct.Material;

            return Task.FromResult(databaseLocaleProduct);
        }

        private Task<IPrice> MapLoadedPricesToDatabasePrice(ICollection<IPrice> loadedPrices, IPrice databasePrice)
        {
            IPrice loadedPrice =
                loadedPrices.FirstOrDefault(x =>
                    x.Currency == databasePrice.Currency && x.ReferenceId == databasePrice.ReferenceId) ??
                throw new ArgumentException(
                    $"Expected to find a loaded {nameof(Price)} with following {nameof(ISize.ReferenceId)} ({databasePrice.ReferenceId}) " +
                    $"and {nameof(IPrice.Currency)} ({databasePrice.Currency}), but none was found.");

            databasePrice.Discount = loadedPrice.Discount;
            databasePrice.NormalSell = loadedPrice.NormalSell;

            return Task.FromResult(databasePrice);
        }

        private async Task<IProduct> MapLoadedProductsToDatabaseProduct(
            ICollection<IProduct> loadedProducts, IProduct databaseProduct)
        {
            try
            {
                IProduct loadedProduct =
                    loadedProducts.FirstOrDefault(x => x.ReferenceId == databaseProduct.ReferenceId) ??
                    throw new ArgumentException(
                        $"Expected to find a loaded product with following {nameof(IProduct.ReferenceId)} ({databaseProduct.ReferenceId}), but none was found.");

                databaseProduct = ClearRemovedChildren(loadedProduct, databaseProduct);
                databaseProduct = AddNewChildren(loadedProduct, databaseProduct);

                databaseProduct.Sizes = (await Task.WhenAll(
                    databaseProduct.Sizes.Select(x => MapLoadedSizesToDatabaseSize(loadedProduct.Sizes, x)))).ToList();
                databaseProduct.Locales = (await Task.WhenAll(databaseProduct.Locales.Select(x =>
                    MapLoadedLocaleProductsToDatabaseLocaleProduct(loadedProduct.Locales, x,
                        loadedProduct.ReferenceId)))).ToList();
                databaseProduct.Prices =
                    (await Task.WhenAll(databaseProduct.Prices.Select(x =>
                        MapLoadedPricesToDatabasePrice(loadedProduct.Prices, x)))).ToList();
                databaseProduct.ExtraTags =
                    (await Task.WhenAll(databaseProduct.ExtraTags.Select(x =>
                        MapLoadedTagToDatabaseTag(loadedProduct.ExtraTags, x)))).ToList();
                databaseProduct.Images = loadedProduct.Images;

                databaseProduct.Manufacturer = loadedProduct.Manufacturer;
                databaseProduct.LinkBase = loadedProduct.LinkBase;
                databaseProduct.Brand = loadedProduct.Brand;
                databaseProduct.Category = loadedProduct.Category;

                return databaseProduct;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to map Loaded Product to an existing entry in the database.");
                throw;
            }
        }

        private async Task<ICollection<IProduct>> MapLoadedProductsToDatabaseProducts(
            IEnumerable<IProduct> loadedProducts, IEnumerable<IProduct> databaseProducts)
        {
            var loadedProductsList = loadedProducts.ToList();
            var databaseProductsList = databaseProducts.ToList();

            var mapTasks = databaseProductsList.Select(x => MapLoadedProductsToDatabaseProduct(loadedProductsList, x))
                .ToList();
            return await Task.WhenAll(mapTasks);
        }

        private Task<ISize> MapLoadedSizesToDatabaseSize(ICollection<ISize> loadedSizes, ISize databaseSize)
        {
            ISize loadedSize = loadedSizes.FirstOrDefault(x => x.ReferenceId == databaseSize.ReferenceId) ??
                               throw new ArgumentException(
                                   $"Expected to find a loaded {nameof(Size)} with following {nameof(ISize.ReferenceId)} ({databaseSize.ReferenceId}), but none was found.");

            databaseSize.Quantity = loadedSize.Quantity;
            databaseSize.LinkBase = loadedSize.LinkBase;
            databaseSize.ModelProductNumber = loadedSize.ModelProductNumber;
            databaseSize.LinkPostFix = loadedSize.LinkPostFix;
            databaseSize.Primary = loadedSize.Primary;
            databaseSize.Secondary = loadedSize.Secondary;
            //databaseSize.Ean = loadedSize.Ean; // Ean Should not get updated, according to Thomas.

            return Task.FromResult(databaseSize);
        }

        private Task<ITag> MapLoadedTagToDatabaseTag(ICollection<ITag> loadedTags, ITag databaseTag)
        {
            ITag loadedTag =
                loadedTags.FirstOrDefault(x =>
                    x.Name == databaseTag.Name && x.ReferenceId == databaseTag.ReferenceId) ??
                throw new ArgumentException(
                    $"Expected to find a loaded {nameof(Tag)} with following {nameof(ITag.Name)} ({databaseTag.Name}) " +
                    $"and {nameof(ITag.ReferenceId)} ({databaseTag.ReferenceId}), but none was found.");

            databaseTag.Value = loadedTag.Value;

            return Task.FromResult(databaseTag);
        }

        private async Task<ICollection<ILocaleProduct>> ProcessLanguageXml(Dictionary<string, string> isoLanguageXmls)
        {
            var generateTasks = isoLanguageXmls.Select(GenerateLocaleProducts);
            var twoDimensionalLocaleProduct = await Task.WhenAll(generateTasks);

            return twoDimensionalLocaleProduct.SelectMany(x => x).ToList();
        }

        private void ReportInvalidXmlFields(XElement element)
        {
            XElement linkElement = element.GetTaggedElement(INVENTORY_LINK);
            XElement eanElement = element.GetTaggedElement(INVENTORY_EAN);
            XElement referenceIdElement = element.GetTaggedElement(INVENTORY_ID);

            bool containsQuestionMark = linkElement.Value.Contains('?');
            bool isEmptyEan = eanElement.IsEmptyValue();

            if (containsQuestionMark && isEmptyEan)
                logger.LogWarning(
                    $"Xml Element ({referenceIdElement.Value}) appear to be a {nameof(Size)}, but is missing an {nameof(Size.Ean)}.");
        }
    }
}