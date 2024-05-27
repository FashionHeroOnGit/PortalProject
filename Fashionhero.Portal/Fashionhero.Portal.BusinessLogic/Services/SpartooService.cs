using System.Xml.Linq;
using Fashionhero.Portal.BusinessLogic.Spartoo;
using Fashionhero.Portal.Shared.Abstraction.Enums;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Entity;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Persistence;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Service;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Spartoo;
using Fashionhero.Portal.Shared.Model.Entity;
using Fashionhero.Portal.Shared.Model.Searchable;
using Microsoft.Extensions.Logging;

namespace Fashionhero.Portal.BusinessLogic.Services
{
    public class SpartooService : IXmlExportService
    {
        private readonly ILogger<SpartooService> logger;
        private readonly IEntityQueryManager<Product, SearchableProduct> productManager;

        private List<ISpartooFilter> filters;

        public SpartooService(
            ILogger<SpartooService> logger, IEntityQueryManager<Product, SearchableProduct> productManager)
        {
            this.logger = logger;
            this.productManager = productManager;
            filters = new List<ISpartooFilter>()
            {
                new ImageFilter(),
                new CurrencyFilter(),
                new GenderFilter(),
                new ProductTypeFilter(),
            };
        }

        /// <inheritdoc />
        public async Task<XDocument> GenerateXmlDocument()
        {
            var databaseProducts = await productManager.GetEntities(new SearchableProduct());
            var validatedProducts = await DiscardInvalidProducts(databaseProducts.ToList());

            var xProducts = new XElement(XmlTagConstants.SPARTOO_PRODUCTS);
            var xRoot = new XElement(XmlTagConstants.SPARTOO_ROOT, xProducts);
            var xmlDocument = new XDocument(xRoot);

            return xmlDocument;
        }

        private async Task<ICollection<Product>> DiscardInvalidProducts(ICollection<Product> databaseProducts)
        {
            var filteredProducts = filters.Aggregate(databaseProducts,
                (current, filter) =>
                    (ICollection<Product>) filter.FilterProducts((ICollection<IProduct>) current, logger));

            logger.LogInformation($"Count After filtering away invalid Products: {filteredProducts.Count}");
            return filteredProducts;
        }
    }
}