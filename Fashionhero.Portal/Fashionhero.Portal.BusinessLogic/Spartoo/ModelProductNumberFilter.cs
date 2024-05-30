using Fashionhero.Portal.Shared.Abstraction.Enums;
using Fashionhero.Portal.Shared.Abstraction.Interfaces;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Entity;
using Fashionhero.Portal.Shared.Model.Entity;
using Microsoft.Extensions.Logging;

namespace Fashionhero.Portal.BusinessLogic.Spartoo
{
    public class ModelProductNumberFilter : IFilter
    {
        /// <inheritdoc />
        public ICollection<IProduct> FilterProducts(ICollection<IProduct> oldProducts, ILogger logger)
        {
            logger.LogInformation(
                $"Filtering away Products without a Model Product Number. Current count: {oldProducts.Count}.");
            return oldProducts.Where(x =>
            {
                if (!string.IsNullOrWhiteSpace(x.ModelProductNumber))
                    return true;

                logger.LogWarning(
                    $"Discarding {nameof(Product)} ({x.ReferenceId}), as it is missing a Model Product Number.");
                return false;
            }).ToList();
        }

        /// <inheritdoc />
        public bool IsFilterOfType(FilterType filterType)
        {
            return filterType == FilterType.SPARTOO_MODEL_PRODUCT_NUMBER;
        }
    }
}