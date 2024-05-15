﻿using System.Xml;
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

            var mappedProductsToUpdate =
                await MapLoadedProductsToDatabaseProducts(loadedProductsForUpdating, productsToUpdate);

            await productManager.UpdateEntities(mappedProductsToUpdate.Cast<Product>());
            await productManager.AddEntities(productsToAdd.Cast<Product>());
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
                    logger.LogWarning($"Multiple product sizes with same Ean ({eanCount.Key}) exist. " +
                                      $"Discarding all but the first found size. Discarded Reference Id's: {referenceIds}");
                }

                result.Add(eanCount.Value.First());
            }

            return Task.FromResult(result as ICollection<ISize>);
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

        private async Task<IProduct> MapLoadedProductsToDatabaseProduct(
            ICollection<IProduct> loadedProducts, IProduct databaseProduct)
        {
            try
            {
                IProduct loadedProduct =
                    loadedProducts.FirstOrDefault(x => x.ReferenceId == databaseProduct.ReferenceId) ??
                    throw new ArgumentException(
                        $"Expected to find a loaded product with following referenceId {databaseProduct.ReferenceId}, but none was found.");

                databaseProduct = ClearRemovedChildren(loadedProduct, databaseProduct);
                databaseProduct = AddNewChildren(loadedProduct, databaseProduct);

                databaseProduct.Sizes = await Task.WhenAll(
                    databaseProduct.Sizes.Select(x => MapLoadedSizesToDatabaseSize(loadedProduct.Sizes, x)));
                databaseProduct.Locales = await Task.WhenAll(databaseProduct.Locales.Select(x =>
                    MapLoadedLocaleProductsToDatabaseLocaleProduct(loadedProduct.Locales, x)));
                databaseProduct.Prices =
                    await Task.WhenAll(databaseProduct.Prices.Select(x =>
                        MapLoadedPricesToDatabasePrice(loadedProduct.Prices, x)));
                databaseProduct.ExtraTags =
                    await Task.WhenAll(databaseProduct.ExtraTags.Select(x =>
                        MapLoadedTagToDatabaseTag(loadedProduct.ExtraTags, x)));
                databaseProduct.Images =
                    await Task.WhenAll(databaseProduct.Images.Select(x =>
                        MapLoadedImagesToDatabaseImage(loadedProduct.Images, x)));

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

        private IProduct ClearRemovedChildren(IProduct loadedProduct, IProduct databaseProduct)
        {
            throw new NotImplementedException();
        }

        private IProduct AddNewChildren(IProduct loadedProduct, IProduct databaseProduct)
        {
            throw new NotImplementedException();
        }

        private Task<ISize> MapLoadedSizesToDatabaseSize(ICollection<ISize> loadedSizes, ISize databaseSize)
        {
            throw new NotImplementedException();
        }

        private Task<ITag> MapLoadedTagToDatabaseTag(ICollection<ITag> loadedTags, ITag databaseTag)
        {
            throw new NotImplementedException();
        }

        private Task<IImage> MapLoadedImagesToDatabaseImage(ICollection<IImage> loadedImages, IImage databaseImage)
        {
            throw new NotImplementedException();
        }

        private Task<IPrice> MapLoadedPricesToDatabasePrice(ICollection<IPrice> loadedPrices, IPrice databaseImage)
        {
            throw new NotImplementedException();
        }

        private Task<ILocaleProduct> MapLoadedLocaleProductsToDatabaseLocaleProduct(
            ICollection<ILocaleProduct> loadedLocaleProducts, ILocaleProduct databaseLocaleProduct)
        {
            throw new NotImplementedException();
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

        private ICollection<IPrice> GetPrices(XElement element)
        {
            return new List<IPrice>
            {
                new Price
                {
                    NormalSell = element.GetTaggedValueAsFloat(INVENTORY_REGULAR_PRICE, logger),
                    Discount = element.GetTaggedValueAsFloat(INVENTORY_SALE_PRICE, logger),
                    Currency = CurrencyCode.DKK,
                },
                new Price
                {
                    NormalSell = element.GetTaggedValueAsFloat(INVENTORY_PRICE_EUR, logger),
                    Discount = element.GetTaggedValueAsFloat(INVENTORY_SALE_PRICE_EUR, logger),
                    Currency = CurrencyCode.EUR,
                },
                new Price
                {
                    NormalSell = element.GetTaggedValueAsFloat(INVENTORY_PRICE_SEK, logger),
                    Discount = element.GetTaggedValueAsFloat(INVENTORY_SALE_PRICE_SEK, logger),
                    Currency = CurrencyCode.SEK,
                },
                new Price
                {
                    NormalSell = element.GetTaggedValueAsFloat(INVENTORY_PRICE_PLN, logger),
                    Discount = element.GetTaggedValueAsFloat(INVENTORY_SALE_PRICE_PLN, logger),
                    Currency = CurrencyCode.PLN,
                },
            };
        }

        private ICollection<IImage> GetImages(XElement element)
        {
            return new List<IImage>
            {
                new Image
                {
                    Url = element.GetTagValueAsString(INVENTORY_IMAGE_LINK, logger),
                },
            };
        }

        private ICollection<ITag> GetExtraTags(XElement element)
        {
            return new List<ITag>
            {
                new Tag
                {
                    Name = INVENTORY_SPARTOO_KODE,
                    Value = element.GetTagValueAsString(INVENTORY_SPARTOO_KODE, logger),
                },
            };
        }

        private async Task<ICollection<ISize>> GetSizes(string inventoryPath)
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

        private async Task<ICollection<ILocaleProduct>> ProcessLanguageXml(Dictionary<string, string> isoLanguageXmls)
        {
            var generateTasks = isoLanguageXmls.Select(GenerateLocaleProducts);
            var twoDimensionalLocaleProduct = await Task.WhenAll(generateTasks);

            return twoDimensionalLocaleProduct.SelectMany(x => x).ToList();
        }

        private async Task<ICollection<ILocaleProduct>> GenerateLocaleProducts(
            KeyValuePair<string, string> isoLanguageXml)
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