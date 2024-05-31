using Fashionhero.Portal.BusinessLogic.Services;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Persistence;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Service;
using Fashionhero.Portal.Shared.Model.Entity;
using Fashionhero.Portal.Shared.Model.Searchable;
using Microsoft.Extensions.Logging;
using Moq;

namespace Fashionhero.Portal.BusinessLogic.Test.Services
{
    public class SpartooServiceTests
    {
        private readonly Mock<ILogger<SpartooService>> mockedLogger;
        private readonly Mock<IEntityQueryManager<Product, SearchableProduct>> mockedProductManager;
        private readonly Mock<ICurrencyConverterService> mockedConverterService;

        public SpartooServiceTests()
        {
            mockedConverterService = new Mock<ICurrencyConverterService>();
            mockedLogger = new Mock<ILogger<SpartooService>>();
            mockedProductManager = new Mock<IEntityQueryManager<Product, SearchableProduct>>();
        }
    }
}