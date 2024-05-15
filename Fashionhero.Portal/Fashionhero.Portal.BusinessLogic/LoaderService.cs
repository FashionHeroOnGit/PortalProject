using System.Linq;
using System.Xml;
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
            var localeProducts = await ProcessLanguageXml(languageXmls);
            var sizes = (await GetSizes(inventoryXml)).Where(x => x.Quantity > 0).ToList();
            var cleanedSizes = await DiscardSizesWithDuplicateEan(sizes);
            var loadedProducts = await GetMasterProducts(inventoryXml, localeProducts, cleanedSizes);

            var databaseProducts = (await productManager.GetEntities(new SearchableProduct())).ToList();
            var loadedReferenceIds = loadedProducts.Select(x => x.ReferenceId).ToList();
            var databaseReferenceIds = databaseProducts.Select(x => x.ReferenceId).ToList();

            var productsToUpdate = databaseProducts.Where(x => loadedReferenceIds.Contains(x.ReferenceId)).ToList();
            var productsToAdd = loadedProducts.Where(x => !databaseReferenceIds.Contains(x.ReferenceId)).ToList();

            var updateReferenceIds = productsToUpdate.Select(x => x.ReferenceId).ToList();
            var loadedProductsForUpdating =
                loadedProducts.Where(x => updateReferenceIds.Contains(x.ReferenceId)).ToList();


            var mappedProductsToUpdate = await MapLoadedProductsToDatabaseProducts(loadedProductsForUpdating,
                productsToUpdate.Cast<IProduct>().ToList());

            await productManager.UpdateEntities(mappedProductsToUpdate.Cast<Product>());
            await productManager.AddEntities(productsToAdd.Cast<Product>());
        }

        private Task<IList<ISize>> DiscardSizesWithDuplicateEan(IList<ISize> sizes)
        {
            var eanCounter = new Dictionary<long, IList<ISize>>();
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
                    logger.LogWarning($"Multiple product sizes with same Ean ({eanCount.Key}) exist. " +
                                      $"Discarding all but the first found size. Discarded Reference Id's: {referenceIds}");
                }

                result.Add(eanCount.Value.First());
            }

            return Task.FromResult(result as IList<ISize>);
        }

        private async Task<IList<IProduct>> MapLoadedProductsToDatabaseProducts(
            IList<IProduct> loadedProducts, IList<IProduct> databaseProducts)
        {
            var loadedProductsList = loadedProducts.ToList();
            var databaseProductsList = databaseProducts.ToList();

            var mapTasks = databaseProductsList.Select(x => MapLoadedProductsToDatabaseProduct(loadedProductsList, x))
                .ToList();
            return await Task.WhenAll(mapTasks);
        }

        private async Task<IProduct> MapLoadedProductsToDatabaseProduct(
            IList<IProduct> loadedProducts, IProduct databaseProduct)
        {
            try
            {
                IProduct loadedProduct =
                    loadedProducts.FirstOrDefault(x => x.ReferenceId == databaseProduct.ReferenceId) ??
                    throw new ArgumentException(
                        $"Expected to find a loaded product with following {nameof(IProduct.ReferenceId)} ({databaseProduct.ReferenceId}), but none was found.");

                databaseProduct = ClearRemovedChildren(loadedProduct, databaseProduct);
                databaseProduct = AddNewChildren(loadedProduct, databaseProduct);

                databaseProduct.Sizes = await Task.WhenAll(
                    databaseProduct.Sizes.Select(x => MapLoadedSizesToDatabaseSize(loadedProduct.Sizes, x)));
                databaseProduct.Locales = await Task.WhenAll(databaseProduct.Locales.Select(x =>
                    MapLoadedLocaleProductsToDatabaseLocaleProduct(loadedProduct.Locales, x,
                        loadedProduct.ReferenceId)));
                databaseProduct.Prices =
                    await Task.WhenAll(databaseProduct.Prices.Select(x =>
                        MapLoadedPricesToDatabasePrice(loadedProduct.Prices, x)));
                databaseProduct.ExtraTags =
                    await Task.WhenAll(databaseProduct.ExtraTags.Select(x =>
                        MapLoadedTagToDatabaseTag(loadedProduct.ExtraTags, x)));
                databaseProduct.Images = loadedProduct.Images;

                databaseProduct.Manufacturer = loadedProduct.Manufacturer;
                databaseProduct.LinkBase = loadedProduct.LinkBase;
                databaseProduct.Brand = loadedProduct.Brand;
                databaseProduct.Category = loadedProduct.Category;

                return databaseProduct;
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Failed to map Loaded Product to an existing entry in the database.");
                throw;
            }
        }

        private List<TResult> GetExceptedList<TEntity, TResult>(
            IList<TEntity> sourceOne, IList<TEntity> sourceTwo, Func<TEntity, TResult> selector) where TEntity : IEntity
        {
            return sourceOne.Select(selector).Except(sourceTwo.Select(selector)).ToList();
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

        private IProduct AddNewChildren(IProduct loadedProduct, IProduct databaseProduct)
        {
            var newLocaleProductReferenceIds =
                GetExceptedList(loadedProduct.Locales, databaseProduct.Locales, x => x.ReferenceId);
            var localeProductsList = databaseProduct.Locales.ToList();
            localeProductsList.AddRange(loadedProduct.Locales.Where(x =>
                newLocaleProductReferenceIds.Contains(x.ReferenceId)));
            databaseProduct.Locales = localeProductsList;

            var newSizeReferenceIds = GetExceptedList(loadedProduct.Sizes, databaseProduct.Sizes, x => x.ReferenceId);
            var sizesList = databaseProduct.Sizes.ToList();
            sizesList.AddRange(loadedProduct.Sizes.Where(x => newSizeReferenceIds.Contains(x.ReferenceId)));
            databaseProduct.Sizes = sizesList;

            var newTagNames = GetExceptedList(loadedProduct.ExtraTags, databaseProduct.ExtraTags, x => x.Name);
            var tagsList = databaseProduct.ExtraTags.ToList();
            tagsList.AddRange(loadedProduct.ExtraTags.Where(x => newTagNames.Contains(x.Name)));
            databaseProduct.ExtraTags = tagsList;

            //var newImageUrls = GetExceptedList(loadedProduct.Images, databaseProduct.Images, x => x.Url);
            //var imagesList = databaseProduct.Images.ToList();
            //imagesList.AddRange(loadedProduct.Images.Where(x => newImageUrls.Contains(x.Url)));
            //databaseProduct.Images = imagesList;

            var newPriceCurrencies = GetExceptedList(loadedProduct.Prices, databaseProduct.Prices, x => x.Currency);
            var pricesList = databaseProduct.Prices.ToList();
            pricesList.AddRange(loadedProduct.Prices.Where(x => newPriceCurrencies.Contains(x.Currency)));
            databaseProduct.Prices = pricesList;

            return databaseProduct;
        }

        private Task<ISize> MapLoadedSizesToDatabaseSize(IList<ISize> loadedSizes, ISize databaseSize)
        {
            ISize loadedSize = loadedSizes.FirstOrDefault(x => x.ReferenceId == databaseSize.ReferenceId) ??
                               throw new ArgumentException(
                                   $"Expected to find a loaded {nameof(Size)} with following {nameof(ISize.ReferenceId)} ({databaseSize.ReferenceId}), but none was found.");

            databaseSize.Quantity = loadedSize.Quantity;
            databaseSize.LinkBase = loadedSize.LinkBase;
            databaseSize.Ean = loadedSize.Ean; // Todo: find out if the Ean changing is valid.
            databaseSize.ModelProductNumber = loadedSize.ModelProductNumber;
            databaseSize.LinkPostFix = loadedSize.LinkPostFix;
            databaseSize.Primary = loadedSize.Primary;
            databaseSize.Secondary = loadedSize.Secondary;

            return Task.FromResult(databaseSize);
        }

        private Task<ITag> MapLoadedTagToDatabaseTag(IList<ITag> loadedTags, ITag databaseTag)
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

        private Task<IPrice> MapLoadedPricesToDatabasePrice(IList<IPrice> loadedPrices, IPrice databasePrice)
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

        private Task<ILocaleProduct> MapLoadedLocaleProductsToDatabaseLocaleProduct(
            IList<ILocaleProduct> loadedLocaleProducts, ILocaleProduct databaseLocaleProduct, int parentReferenceId)
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

        private async Task<IList<IProduct>> GetMasterProducts(
            string inventoryPath, IList<ILocaleProduct> localeProducts, IList<ISize> sizes)
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

        private Task<Product> GenerateProduct(
            XElement element, IList<ILocaleProduct> localeProducts, IList<ISize> sizes)
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

        private IList<IPrice> GetPrices(XElement element)
        {
            return new List<IPrice>
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
        }

        private IList<IImage> GetImages(XElement element)
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

        private IList<ITag> GetExtraTags(XElement element)
        {
            return new List<ITag>
            {
                new Tag
                {
                    Name = INVENTORY_SPARTOO_KODE,
                    Value = element.GetTagValueAsString(INVENTORY_SPARTOO_KODE, logger),
                    ReferenceId = element.GetTagValueAsInt(INVENTORY_ID, logger),
                },
            };
        }

        private async Task<IList<ISize>> GetSizes(string inventoryPath)
        {
            XDocument document = GetDocument(inventoryPath);

            var xmlSizes = document.Elements(INVENTORY_ROOT).Elements(INVENTORY_SINGLE_PRODUCT).Where(x =>
            {
                XElement? element = x.Element(INVENTORY_LINK);
                return element != null && element.Value.Contains('?');
            }).Where(x => !x.Element(INVENTORY_EAN).IsEmptyValue());

            var generateTasks = xmlSizes.Select(GenerateSize);
            return await Task.WhenAll(generateTasks);
        }

        private Task<Size> GenerateSize(XElement element)
        {
            string[] splitLink = element.GetTagValueAsString(INVENTORY_LINK, logger).Split('?');

            return Task.FromResult(new Size
            {
                Quantity = element.GetTagValueAsInt(INVENTORY_STOCK, logger),
                LinkBase = splitLink[0],
                LinkPostFix = $"?{splitLink[1]}",
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

        private async Task<IList<ILocaleProduct>> ProcessLanguageXml(Dictionary<string, string> isoLanguageXmls)
        {
            var generateTasks = isoLanguageXmls.Select(GenerateLocaleProducts);
            var twoDimensionalLocaleProduct = await Task.WhenAll(generateTasks);

            return twoDimensionalLocaleProduct.SelectMany(x => x).ToList();
        }

        private async Task<IList<ILocaleProduct>> GenerateLocaleProducts(KeyValuePair<string, string> isoLanguageXml)
        {
            XDocument document = GetDocument(isoLanguageXml.Value);

            var xmlLocaleProducts = document.Elements(LOCALE_ROOT).Elements(LOCALE_SINGLE_PRODUCT);
            var generateTasks = xmlLocaleProducts.Select(x => GenerateLocaleProduct(x, isoLanguageXml.Key));
            return await Task.WhenAll(generateTasks);
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
    }
}