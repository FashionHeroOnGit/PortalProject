using Fashionhero.Portal.Shared.Abstraction.Enums;
using Fashionhero.Portal.Shared.Abstraction.Interfaces;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Entity;
using Fashionhero.Portal.Shared.Model.Entity;
using Microsoft.Extensions.Logging;

namespace Fashionhero.Portal.BusinessLogic.Spartoo
{
    public class ImageFilter : IFilter
    {
        /// <inheritdoc />
        public ICollection<IProduct> FilterProducts(ICollection<IProduct> oldProducts, ILogger logger)
        {
            logger.LogInformation($"Filtering away Products without valid images. Current count: {oldProducts.Count}.");
            return oldProducts.Where(x =>
            {
                if (x.Images.Any(z => z.Url.EndsWith(".jpg")))
                    return true;

                logger.LogWarning(
                    $"Discarding {nameof(Product)} ({x.ReferenceId}), as it does not have any .jpg images.");
                return false;
            }).ToList();
        }

        /// <inheritdoc />
        public bool IsFilterOfType(FilterType filterType)
        {
            return filterType == FilterType.SPARTOO_IMAGE;
        }
    }
}