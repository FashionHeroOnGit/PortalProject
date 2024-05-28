using Fashionhero.Portal.Shared.Abstraction.Enums;
using Fashionhero.Portal.Shared.Abstraction.Enums.Spartoo;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Entity;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Spartoo;
using Fashionhero.Portal.Shared.Model.Entity;
using Microsoft.Extensions.Logging;

namespace Fashionhero.Portal.BusinessLogic.Spartoo
{
    public class CurrencyFilter : ISpartooFilter
    {
        /// <inheritdoc />
        public ICollection<IProduct> FilterProducts(ICollection<IProduct> oldProducts, ILogger logger)
        {
            logger.LogInformation($"Filtering away Products without a DKK price. Current count: {oldProducts.Count}");
            return oldProducts.Where(x =>
            {
                if (x.Prices.Any(z => z.Currency == CurrencyCode.DKK))
                    return true;

                logger.LogWarning(
                    $"Discarding {nameof(Product)} ({x.ReferenceId}), as it does not have a {CurrencyCode.DKK} price.");
                return false;
            }).ToList();
        }

        /// <inheritdoc />
        public object? GetDictionaryValue(string key)
        {
            return default;
        }

        /// <inheritdoc />
        public bool IsFilterOfType(FilterType filterType)
        {
            return filterType == FilterType.CURRENCY;
        }
    }
}