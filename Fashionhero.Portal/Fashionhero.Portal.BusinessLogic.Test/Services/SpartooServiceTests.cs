using System.Xml.Linq;
using Fashionhero.Portal.BusinessLogic.Services;
using Fashionhero.Portal.BusinessLogic.Test.Core;
using Fashionhero.Portal.Shared.Abstraction.Enums;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Entity;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Persistence;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Service;
using Fashionhero.Portal.Shared.Model.Entity;
using Fashionhero.Portal.Shared.Model.Searchable;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Fashionhero.Portal.BusinessLogic.Test.Services
{
    public class SpartooServiceTests
    {
        private readonly Mock<ICurrencyConverterService> mockedConverterService;
        private readonly Mock<ILogger<SpartooService>> mockedLogger;
        private readonly Mock<ILoggerFactory> mockedLoggerFactory;
        private readonly Mock<IEntityQueryManager<Product, SearchableProduct>> mockedProductManager;

        public SpartooServiceTests()
        {
            mockedConverterService = new Mock<ICurrencyConverterService>();
            mockedLogger = new Mock<ILogger<SpartooService>>();
            mockedProductManager = new Mock<IEntityQueryManager<Product, SearchableProduct>>();
            mockedLoggerFactory = new Mock<ILoggerFactory>();

            mockedProductManager.Setup(x => x.GetEntities(It.IsAny<SearchableProduct>()))
                .ReturnsAsync(new List<Product>().AsEnumerable());
            mockedConverterService.Setup(x => x.GetRate(CurrencyCode.DKK, CurrencyCode.EUR)).ReturnsAsync(0.13m);
            mockedLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(new Mock<ILogger>().Object);
            mockedConverterService.Setup(x => x.ConvertPrice(It.IsAny<Price>(), It.IsAny<CurrencyCode>()))
                .ReturnsAsync(new Price() {Currency = CurrencyCode.EUR, NormalSell = 195,});
        }

        [Fact]
        public async void ItReturnsEmptyDocumentWhenNoProductsExist()
        {
            XDocument expected = GenerateEmptyDocument();
            var sut = new SpartooService(mockedLogger.Object, mockedProductManager.Object,
                mockedConverterService.Object, mockedLoggerFactory.Object);

            XDocument actual = await sut.GenerateXmlDocument();

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async void ItReturnsEmptyDocumentWhenNoProductsRemainAfterFiltering()
        {
            XDocument expected = GenerateEmptyDocument();
            mockedProductManager.Setup(x => x.GetEntities(It.IsAny<SearchableProduct>()))
                .ReturnsAsync(GenerateIndividuallyInvalidProducts().Cast<Product>().AsEnumerable());
            var sut = new SpartooService(mockedLogger.Object, mockedProductManager.Object,
                mockedConverterService.Object, mockedLoggerFactory.Object);

            XDocument actual = await sut.GenerateXmlDocument();

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async void ItReturnsFilledDocumentWhenGivenValidProducts()
        {
            XDocument expected = GetTestXmlFile(nameof(ItReturnsFilledDocumentWhenGivenValidProducts));
            mockedProductManager.Setup(x => x.GetEntities(It.IsAny<SearchableProduct>()))
                .ReturnsAsync(GenerateValidProducts().Cast<Product>().AsEnumerable());
            var sut = new SpartooService(mockedLogger.Object, mockedProductManager.Object,
                mockedConverterService.Object, mockedLoggerFactory.Object);

            XDocument actual = await sut.GenerateXmlDocument();

            actual.Should().BeEquivalentTo(expected);
        }

        private static XDocument GetTestXmlFile(string testName)
        {
            return XDocument.Parse(TestHelpers.LoadXmlFileContent(BuildExportTestFilePath(testName)));
        }

        private static string BuildExportTestFilePath(string testName)
        {
            return $@".\Xml\{nameof(SpartooServiceTests)}\{testName}\ExportToSpartoo.xml";
        }

        private static ICollection<IProduct> GenerateValidProducts()
        {
            var localeProduct = new LocaleProduct()
            {
                Colour = "sort",
                IsoName = "dk",
                Type = "t-shirts",
                Gender = "Mand",
                Title = "Lorem Ipsum",
                CountryOrigin = "DK",
            };
            var product = new Product()
            {
                Locales = new List<ILocaleProduct>()
                {
                    localeProduct,
                },
                ModelProductNumber = "some-model-product-number",
                Prices = new List<IPrice>()
                {
                    new Price()
                    {
                        Currency = CurrencyCode.DKK,
                        NormalSell = 1500,
                    },
                },
                Images = new List<IImage>()
                {
                    new Image()
                    {
                        Url = "someLocation/someImage.jpg",
                    },
                },
                Sizes = new List<ISize>()
                {
                    new Size()
                    {
                        Primary = "XL",
                        Quantity = 1,
                        Ean = 5719483876380,
                        ModelProductNumber = "some-model-product-number",
                    },
                },
            };
            localeProduct.Product = product;

            return TestEntitiesBuilder.BuildProducts([product,]).Cast<IProduct>().ToList();
        }

        private static ICollection<IProduct> GenerateIndividuallyInvalidProducts()
        {
            return TestEntitiesBuilder.BuildProducts([
                new Product
                {
                    Locales = new List<ILocaleProduct>
                    {
                        new LocaleProduct
                        {
                            IsoName = "dk",
                            Gender = "invalid",
                        },
                    },
                },
                new Product
                {
                    Images = new List<IImage>
                    {
                        new Image
                        {
                            Url = "some-invalid-extension.png",
                        },
                    },
                },
                new Product
                {
                    ModelProductNumber = "",
                },
                new Product
                {
                    Locales = new List<ILocaleProduct>
                    {
                        new LocaleProduct
                        {
                            IsoName = "dk",
                            Type = "invalid",
                        },
                    },
                },
                new Product
                {
                    Sizes = new List<ISize>()
                    {
                        new Size(),
                    },
                },
                new Product
                {
                    Prices = new List<IPrice>
                    {
                        new Price
                        {
                            Currency = CurrencyCode.USD,
                        },
                    },
                },
            ], true).Cast<IProduct>().ToList();
        }

        private static XDocument GenerateEmptyDocument()
        {
            var productsRoot = new XElement(XmlTagConstants.SPARTOO_PRODUCTS_ROOT);
            var root = new XElement(XmlTagConstants.SPARTOO_ROOT, productsRoot);
            return new XDocument(root);
        }
    }
}