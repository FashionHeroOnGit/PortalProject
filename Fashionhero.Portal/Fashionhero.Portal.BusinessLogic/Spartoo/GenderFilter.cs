using Fashionhero.Portal.Shared.Abstraction.Enums.Spartoo;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Entity;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Spartoo;
using Fashionhero.Portal.Shared.Model;
using Fashionhero.Portal.Shared.Model.Entity;
using Microsoft.Extensions.Logging;

namespace Fashionhero.Portal.BusinessLogic.Spartoo
{
    public class GenderFilter : ISpartooFilter
    {
        private readonly Dictionary<string, char> productGenderMap;

        public GenderFilter()
        {
            productGenderMap = new Dictionary<string, char>(StringComparer.InvariantCultureIgnoreCase)
            {
                {"Mand", 'H'},
                {"Unisex", 'M'},
                {"Børn", 'K'},
                {"Dame", 'F'},
            };
        }

        /// <inheritdoc />
        public ICollection<IProduct> FilterProducts(ICollection<IProduct> oldProducts, ILogger logger)
        {
            logger.LogInformation(
                $"Filtering away Products without a Danish translation containing a valid gender that can be translatable to a Product Sex/Gender. Current count: {oldProducts.Count}");
            return oldProducts.Where(x =>
            {
                try
                {
                    ILocaleProduct locale = x.Locales.FirstOrDefault(z => z.IsoName == Constants.DANISH_ISO_NAME) ??
                                            throw new ArgumentException(
                                                $"Failed to find a Danish Translation of products, for filtering by {nameof(LocaleProduct.Gender)}.");

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
        }

        /// <inheritdoc />
        public object? GetDictionaryValue(string key)
        {
            return productGenderMap.GetValueOrDefault(key);
        }

        /// <inheritdoc />
        public bool IsFilterOfType(FilterType filterType)
        {
            return filterType == FilterType.GENDER;
        }
    }
}