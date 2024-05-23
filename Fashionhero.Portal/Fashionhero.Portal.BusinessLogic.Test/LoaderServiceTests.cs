using Fashionhero.Portal.BusinessLogic.Test.Core;
using Fashionhero.Portal.Shared.Abstraction.Enums;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Entity;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Persistence;
using Fashionhero.Portal.Shared.Model.Entity;
using Fashionhero.Portal.Shared.Model.Searchable;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Fashionhero.Portal.BusinessLogic.Test
{
    public class LoaderServiceTests
    {
        protected Mock<ILogger<LoaderService>> mockedLogger;
        protected Mock<IEntityQueryManager<Product, SearchableProduct>> mockedQueryManager;

        public LoaderServiceTests()
        {
            mockedQueryManager = new Mock<IEntityQueryManager<Product, SearchableProduct>>();
            mockedLogger = new Mock<ILogger<LoaderService>>();

            mockedQueryManager.Setup(x => x.GetEntities(It.IsAny<SearchableProduct>()))
                .ReturnsAsync(new List<Product>().AsEnumerable());
        }

        [Fact]
        public async void AddNewLocaleProductDuringUpdate()
        {
            (string inventoryXml, var languageXml) = GetSutArguments(nameof(AddNewLocaleProductDuringUpdate));
            languageXml.Add("da", LoadXmlFileContent($@"Xml\{nameof(AddNewLocaleProductDuringUpdate)}\Language2.xml"));
            var expectedProducts = AddNewLocaleProductDuringUpdateData(false);
            var sut = new LoaderService(mockedLogger.Object, mockedQueryManager.Object);
            mockedQueryManager.Setup(x => x.GetEntities(It.IsAny<SearchableProduct>()))
                .ReturnsAsync(AddNewLocaleProductDuringUpdateData(true));

            await sut.UpdateInventory(languageXml, inventoryXml);

            mockedQueryManager.Verify(x => x.UpdateEntities(It.IsAny<ICollection<Product>>()));
            IInvocation updateEntitiesInvocation = mockedQueryManager.Invocations.First(x => x.IsVerified);
            object updateEntitiesParameter = updateEntitiesInvocation.Arguments[0];
            updateEntitiesParameter.Should().BeEquivalentTo(expectedProducts);
        }

        [Fact]
        public async void AddNewSizeDuringUpdate()
        {
            (string inventoryXml, var languageXml) = GetSutArguments(nameof(AddNewSizeDuringUpdate));
            var expectedProducts = AddNewSizeDuringUpdateData(false);
            var sut = new LoaderService(mockedLogger.Object, mockedQueryManager.Object);
            mockedQueryManager.Setup(x => x.GetEntities(It.IsAny<SearchableProduct>()))
                .ReturnsAsync(AddNewSizeDuringUpdateData(true));

            await sut.UpdateInventory(languageXml, inventoryXml);

            mockedQueryManager.Verify(x => x.UpdateEntities(It.IsAny<ICollection<Product>>()));
            IInvocation updateEntitiesInvocation = mockedQueryManager.Invocations.First(x => x.IsVerified);
            object updateEntitiesParameter = updateEntitiesInvocation.Arguments[0];
            updateEntitiesParameter.Should().BeEquivalentTo(expectedProducts);
        }

        [Fact]
        public async void SaveGeneratedProducts()
        {
            (string inventoryXml, var languageXml) = GetSutArguments(nameof(SaveGeneratedProducts));
            var expectedProducts = SaveGeneratedProductsData();
            var sut = new LoaderService(mockedLogger.Object, mockedQueryManager.Object);

            await sut.UpdateInventory(languageXml, inventoryXml);

            mockedQueryManager.Verify(x => x.AddEntities(It.IsAny<ICollection<Product>>()));
            IInvocation addEntitiesInvocation = mockedQueryManager.Invocations.First(x => x.IsVerified);
            object addEntitiesParameter = addEntitiesInvocation.Arguments[0];
            addEntitiesParameter.Should().BeEquivalentTo(expectedProducts);
        }

        private ICollection<Product> AddNewLocaleProductDuringUpdateData(bool isDatabaseMockResult)
        {
            return isDatabaseMockResult
                ? BuildProducts([
                    new Product
                    {
                        Locales = new List<ILocaleProduct>
                        {
                            TestEntitiesBuilder.BuildLocaleProduct(1, 1, "en", "Horse X - T", "EN", "BLACK"),
                        },
                    },
                ])
                : BuildProducts([
                    new Product
                    {
                        Locales = new List<ILocaleProduct>
                        {
                            TestEntitiesBuilder.BuildLocaleProduct(1, 1, "en", "Horse X - T", "EN", "BLACK"),
                            TestEntitiesBuilder.BuildLocaleProduct(1, 1, "da", "Horse X - T", "DA", "BLACK"),
                        },
                    },
                ]);
        }

        private ICollection<Product> AddNewSizeDuringUpdateData(bool isDatabaseMockResult)
        {
            return isDatabaseMockResult
                ? BuildProducts([
                    new Product
                    {
                        Sizes = new List<ISize>
                        {
                            TestEntitiesBuilder.BuildSize(1, 2, 5769403877380),
                        },
                    },
                ])
                : BuildProducts([
                    new Product
                    {
                        Sizes = new List<ISize>
                        {
                            TestEntitiesBuilder.BuildSize(1, 2, 5769403877380),
                            TestEntitiesBuilder.BuildSize(1, 3, 5763404879388),
                        },
                    },
                ]);
        }

        private ICollection<Product> BuildProducts(ICollection<Product> products)
        {
            return products.Select(x =>
            {
                var images = x.Images.Count == 0
                    ? new List<IImage>
                    {
                        TestEntitiesBuilder.BuildImage(1),
                    }
                    : x.Images;
                var localeProducts = x.Locales.Count == 0
                    ? new List<ILocaleProduct>
                    {
                        TestEntitiesBuilder.BuildLocaleProduct(1, 1, "en", "Horse X - T", "EN", "BLACK"),
                    }
                    : x.Locales;
                var sizes = x.Sizes.Count == 0
                    ? new List<ISize>
                    {
                        TestEntitiesBuilder.BuildSize(1, 2, 5769403877380),
                    }
                    : x.Sizes;
                var prices = x.Prices.Count == 0
                    ? new List<IPrice>
                    {
                        TestEntitiesBuilder.BuildPrice(449, 1, CurrencyCode.DKK),
                        TestEntitiesBuilder.BuildPrice(60.15F, 1, CurrencyCode.EUR),
                        TestEntitiesBuilder.BuildPrice(704.73F, 1, CurrencyCode.SEK),
                        TestEntitiesBuilder.BuildPrice(256.63F, 1, CurrencyCode.PLN),
                    }
                    : x.Prices;
                var tags = x.ExtraTags.Count == 0
                    ? new List<ITag>
                    {
                        TestEntitiesBuilder.BuildTag("spartoo-kode", 1),
                    }
                    : x.ExtraTags;
                return TestEntitiesBuilder.BuildProduct(x.ReferenceId != default ? x.ReferenceId : 1, images,
                    localeProducts, sizes, prices, tags, !string.IsNullOrWhiteSpace(x.Brand) ? x.Brand : "Horse");
            }).ToList();
        }

        private (string, Dictionary<string, string>) GetSutArguments(string testName)
        {
            var inventoryXmlFile = $@"Xml\{testName}\Inventory.xml";
            var languageXmlFile = $@"Xml\{testName}\Language.xml";
            string inventoryXml = LoadXmlFileContent(inventoryXmlFile);
            Dictionary<string, string> languageXml = new()
            {
                {
                    "en",
                    LoadXmlFileContent(languageXmlFile)
                },
            };

            return (inventoryXml, languageXml);
        }

        private string LoadXmlFileContent(string fileName)
        {
            return File.ReadAllText(Path.Combine(@"..\..\..\", fileName));
        }

        private ICollection<Product> SaveGeneratedProductsData()
        {
            return BuildProducts([new Product(),]);
        }
    }
}