using Fashionhero.Portal.Shared.Abstraction.Enums;
using Fashionhero.Portal.Shared.Abstraction.Interfaces;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Entity;
using Fashionhero.Portal.Shared.Model.Entity;
using Microsoft.Extensions.Logging;

namespace Fashionhero.Portal.BusinessLogic.Spartoo
{
    public class CurrencyFilter : IFilter
    {
        private readonly ILogger<CurrencyFilter> logger;

        public CurrencyFilter(ILogger<CurrencyFilter> logger)
        {
            this.logger = logger;
        }

        /// <inheritdoc />
        public ICollection<IProduct> FilterProducts(ICollection<IProduct> oldProducts)
        {
            logger.LogInformation($"Filtering away Products without a DKK price. Current count: {oldProducts.Count}");
            return oldProducts.Where(x =>
            {
                if (x.Prices.Any(z => z.Currency == CurrencyCode.DKK))
                    return true;

                logger.LogWarning(
                    $"Discarding {nameof(Product)} ({x.ReferenceId}), as it does not have a {CurrencyCode.DKK} {nameof(Price)}.");
                return false;
            }).ToList();
        }

        /// <inheritdoc />
        public bool IsFilterOfType(FilterType filterType)
        {
            return filterType == FilterType.SPARTOO_CURRENCY;
        }
    }
}