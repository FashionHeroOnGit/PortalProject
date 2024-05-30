using Fashionhero.Portal.Shared.Abstraction.Enums;
using Fashionhero.Portal.Shared.Abstraction.Interfaces;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Entity;
using Fashionhero.Portal.Shared.Model.Entity;
using Microsoft.Extensions.Logging;

namespace Fashionhero.Portal.BusinessLogic.Spartoo
{
    public class SizeFilter : IFilter
    {
        private readonly ILogger<SizeFilter> logger;

        public SizeFilter(ILogger<SizeFilter> logger)
        {
            this.logger = logger;
        }

        /// <inheritdoc />
        public ICollection<IProduct> FilterProducts(ICollection<IProduct> oldProducts)
        {
            logger.LogInformation(
                $"Filtering away Products where one or more sizes is missing a {nameof(Size.Primary)} {nameof(Size)}. Current count: {oldProducts.Count}");
            return oldProducts.Where(x =>
            {
                if (x.Sizes.All(z => !string.IsNullOrWhiteSpace(z.Primary)))
                    return true;

                var invalidSizesReferenceIds = x.Sizes.Where(z => string.IsNullOrWhiteSpace(z.Primary))
                    .Select(z => z.ReferenceId).ToList();

                logger.LogWarning(
                    $"Discarding {nameof(Product)} ({x.ReferenceId}), as one or more sizes is missing its {nameof(Size.Primary)} {nameof(Size)}." +
                    $" Missing: {string.Join(", ", invalidSizesReferenceIds)}");
                return false;
            }).ToList();
        }

        /// <inheritdoc />
        public bool IsFilterOfType(FilterType filterType)
        {
            return filterType == FilterType.SPARTOO_SIZE;
        }
    }
}