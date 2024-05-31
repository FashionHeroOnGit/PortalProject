using Fashionhero.Portal.Shared.Abstraction.Enums;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Entity;
using Fashionhero.Portal.Shared.Model.Entity;

namespace Fashionhero.Portal.BusinessLogic.Test.Core
{
    public static class TestEntitiesBuilder
    {
        public static Image BuildImage(int referenceId, string url = "someImage/")
        {
            return new Image
            {
                ReferenceId = referenceId,
                Url = url,
            };
        }

        public static LocaleProduct BuildLocaleProduct(
            int itemGroupId, int referenceId, string isoName, string title, string countryOrigin, string colour,
            string description = "", string gender = "", string material = "Test", string type = "One",
            string localeType = "One > Two > Three")
        {
            return new LocaleProduct
            {
                ItemGroupId = itemGroupId,
                ReferenceId = referenceId,
                IsoName = isoName,
                LocalType = localeType,
                Colour = colour,
                CountryOrigin = countryOrigin,
                Type = type,
                Title = title,
                Description = description,
                Gender = gender,
                Material = material,
            };
        }

        public static Price BuildPrice(decimal normalSell, int referenceId, CurrencyCode currency, decimal discount = 0)
        {
            return new Price
            {
                NormalSell = normalSell,
                ReferenceId = referenceId,
                Currency = currency,
                Discount = discount,
            };
        }

        public static Product BuildProduct(
            int referenceId, ICollection<IImage> images, ICollection<ILocaleProduct> locales, ICollection<ISize> sizes,
            ICollection<IPrice> prices, ICollection<ITag> extraTags, string brand, string category = "Cat",
            string linkBase = "someLink/", string modelProductNumber = "some-model-number")
        {
            return new Product
            {
                Locales = locales,
                ReferenceId = referenceId,
                Sizes = sizes,
                ExtraTags = extraTags,
                Images = images,
                Prices = prices,
                LinkBase = linkBase,
                Manufacturer = brand,
                Brand = brand,
                Category = category,
                ModelProductNumber = modelProductNumber,
            };
        }

        public static ICollection<Product> BuildProducts(ICollection<Product> products)
        {
            return products.Select(x =>
            {
                var images = x.Images.Count == 0
                    ? new List<IImage>
                    {
                        BuildImage(x.ReferenceId != default ? x.ReferenceId : 1),
                    }
                    : x.Images;
                var localeProducts = x.Locales.Count == 0
                    ? new List<ILocaleProduct>
                    {
                        BuildLocaleProduct(x.ReferenceId != default ? x.ReferenceId : 1, 1, "en", "Horse X - T", "EN",
                            "BLACK"),
                    }
                    : x.Locales;
                var sizes = x.Sizes.Count == 0
                    ? new List<ISize>
                    {
                        BuildSize(1, 2, 5769403877380),
                    }
                    : x.Sizes;
                var prices = x.Prices.Count == 0
                    ? new List<IPrice>
                    {
                        BuildPrice(449, x.ReferenceId != default ? x.ReferenceId : 1, CurrencyCode.DKK),
                        BuildPrice(60.15m, x.ReferenceId != default ? x.ReferenceId : 1, CurrencyCode.EUR),
                        BuildPrice(704.73m, x.ReferenceId != default ? x.ReferenceId : 1, CurrencyCode.SEK),
                        BuildPrice(256.63m, x.ReferenceId != default ? x.ReferenceId : 1, CurrencyCode.PLN),
                    }
                    : x.Prices;
                var tags = x.ExtraTags;
                return BuildProduct(x.ReferenceId != default ? x.ReferenceId : 1, images, localeProducts, sizes, prices,
                    tags, !string.IsNullOrWhiteSpace(x.Brand) ? x.Brand : "Horse");
            }).ToList();
        }

        public static Size BuildSize(
            int quantity, int referenceId, long ean, string modelProductNumber = "some-model-number",
            string primary = "T", string secondary = "", string linkPostFix = "?attribute_pa_stoerrelser=T",
            string linkBase = "someLink/")
        {
            return new Size
            {
                Quantity = quantity,
                ReferenceId = referenceId,
                Ean = ean,
                ModelProductNumber = modelProductNumber,
                Primary = primary,
                Secondary = secondary,
                LinkBase = linkBase,
                LinkPostFix = linkPostFix,
            };
        }

        public static Tag BuildTag(string name, int referenceId, string value = "")
        {
            return new Tag
            {
                Name = name,
                Value = value,
                ReferenceId = referenceId,
            };
        }
    }
}