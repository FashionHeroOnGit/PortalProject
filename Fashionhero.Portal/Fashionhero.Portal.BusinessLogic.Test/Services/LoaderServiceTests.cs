using Fashionhero.Portal.BusinessLogic.Services;
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

namespace Fashionhero.Portal.BusinessLogic.Test.Services
{
    public class LoaderServiceTests
    {
        private readonly Mock<ILogger<LoaderService>> mockedLogger;
        private readonly Mock<IEntityQueryManager<Product, SearchableProduct>> mockedQueryManager;

        public LoaderServiceTests()
        {
            mockedQueryManager = new Mock<IEntityQueryManager<Product, SearchableProduct>>();
            mockedLogger = new Mock<ILogger<LoaderService>>();

            mockedQueryManager.Setup(x => x.GetEntities(It.IsAny<SearchableProduct>()))
                .ReturnsAsync(new List<Product>().AsEnumerable());
        }

        private static string BuildInventoryTestFilePath(string testName)
        {
            return $@".\Xml\{nameof(LoaderServiceTests)}\{testName}\Inventory.xml";
        }

        private static string BuildLanguageTestFilePath(string testName, int fileNumber = 0)
        {
            return
                $@".\Xml\{nameof(LoaderServiceTests)}\{testName}\Language{(fileNumber != default ? fileNumber.ToString() : "")}.xml";
        }

        [Fact]
        public async void ItAddsNewLocaleProductDuringUpdate()
        {
            (string inventoryXml, var languageXml) = GetSutArguments(nameof(ItAddsNewLocaleProductDuringUpdate));
            languageXml.Add("da",
                LoadXmlFileContent(BuildLanguageTestFilePath(nameof(ItAddsNewLocaleProductDuringUpdate), 2)));
            var expectedProducts = ItAddsNewLocaleProductDuringUpdateExpectedData();
            var sut = new LoaderService(mockedLogger.Object, mockedQueryManager.Object);
            mockedQueryManager.Setup(x => x.GetEntities(It.IsAny<SearchableProduct>()))
                .ReturnsAsync(ItAddsNewLocaleProductDuringUpdateDatabaseData());

            await sut.UpdateInventory(languageXml, inventoryXml);

            mockedQueryManager.Verify(x => x.UpdateEntities(It.IsAny<ICollection<Product>>()));
            IInvocation updateEntitiesInvocation = mockedQueryManager.Invocations.First(x => x.IsVerified);
            object updateEntitiesParameter = updateEntitiesInvocation.Arguments[0];
            updateEntitiesParameter.Should().BeEquivalentTo(expectedProducts);
        }

        [Fact]
        public async void ItAddsNewSizeDuringUpdate()
        {
            (string inventoryXml, var languageXml) = GetSutArguments(nameof(ItAddsNewSizeDuringUpdate));
            var expectedProducts = ItAddsNewSizeDuringUpdateExpectedData();
            var sut = new LoaderService(mockedLogger.Object, mockedQueryManager.Object);
            mockedQueryManager.Setup(x => x.GetEntities(It.IsAny<SearchableProduct>()))
                .ReturnsAsync(ItAddsNewSizeDuringUpdateDatabaseData());

            await sut.UpdateInventory(languageXml, inventoryXml);

            mockedQueryManager.Verify(x => x.UpdateEntities(It.IsAny<ICollection<Product>>()));
            IInvocation updateEntitiesInvocation = mockedQueryManager.Invocations.First(x => x.IsVerified);
            object updateEntitiesParameter = updateEntitiesInvocation.Arguments[0];
            updateEntitiesParameter.Should().BeEquivalentTo(expectedProducts);
        }

        [Fact]
        public async void ItDeletesProductsThatNoLongerExist()
        {
            (string inventoryXml, var languageXml) = GetSutArguments(nameof(ItDeletesProductsThatNoLongerExist));
            var expectedProducts = ItDeletesProductsThatNoLongerExistExpectedData();
            var sut = new LoaderService(mockedLogger.Object, mockedQueryManager.Object);
            mockedQueryManager.Setup(x => x.GetEntities(It.IsAny<SearchableProduct>())).ReturnsAsync(
                ItDeletesProductsThatNoLongerExistDatabaseData());

            await sut.UpdateInventory(languageXml, inventoryXml);

            mockedQueryManager.Verify(x => x.DeleteEntities(It.IsAny<ICollection<Product>>()));
            IInvocation deleteEntitiesInvocation = mockedQueryManager.Invocations.First(x => x.IsVerified);
            object deleteEntitiesParameter = deleteEntitiesInvocation.Arguments[0];
            deleteEntitiesParameter.Should().BeEquivalentTo(expectedProducts);
        }

        [Fact]
        public async void ItLogsWarningWhenElementLooksLikeSizeButIsMissingEan()
        {
            (string inventoryXml, var languageXml) =
                GetSutArguments(nameof(ItLogsWarningWhenElementLooksLikeSizeButIsMissingEan));
            const string expectedLogMessageFragment = "is missing an ean";
            var sut = new LoaderService(mockedLogger.Object, mockedQueryManager.Object);

            await sut.UpdateInventory(languageXml, inventoryXml);

            VerifyLogWarningCalled();
            IInvocation? logInvocation = mockedLogger.Invocations.FirstOrDefault(x =>
            {
                var message = x.Arguments[2].ToString();
                return !string.IsNullOrWhiteSpace(message) && message.ToLowerInvariant()
                    .Contains(expectedLogMessageFragment.ToLowerInvariant());
            });
            logInvocation.Should().NotBeNull();
        }

        [Fact]
        public async void ItLogsWarningWhenManyLocaleProductsAreAttachedDuringUpdate()
        {
            (string inventoryXml, var languageXml) =
                GetSutArguments(nameof(ItLogsWarningWhenManyLocaleProductsAreAttachedDuringUpdate));
            languageXml.Add("da",
                LoadXmlFileContent(
                    BuildLanguageTestFilePath(nameof(ItLogsWarningWhenManyLocaleProductsAreAttachedDuringUpdate), 2)));
            languageXml.Add("de",
                LoadXmlFileContent(
                    BuildLanguageTestFilePath(nameof(ItLogsWarningWhenManyLocaleProductsAreAttachedDuringUpdate), 3)));
            const string expectedLogMessageFragment = "large growth";
            var sut = new LoaderService(mockedLogger.Object, mockedQueryManager.Object);
            mockedQueryManager.Setup(x => x.GetEntities(It.IsAny<SearchableProduct>())).ReturnsAsync(BareBoneData());

            await sut.UpdateInventory(languageXml, inventoryXml);

            VerifyLogWarningCalled();
            IInvocation? logInvocation = mockedLogger.Invocations.FirstOrDefault(x =>
            {
                var message = x.Arguments[2].ToString();
                return !string.IsNullOrWhiteSpace(message) && message.ToLowerInvariant()
                    .Contains(expectedLogMessageFragment.ToLowerInvariant());
            });
            logInvocation.Should().NotBeNull();
        }

        [Fact]
        public async void ItLogsWarningWhenManySizesAreAttachedDuringUpdate()
        {
            (string inventoryXml, var languageXml) =
                GetSutArguments(nameof(ItLogsWarningWhenManySizesAreAttachedDuringUpdate));

            const string expectedLogMessageFragment = "large growth";
            var sut = new LoaderService(mockedLogger.Object, mockedQueryManager.Object);
            mockedQueryManager.Setup(x => x.GetEntities(It.IsAny<SearchableProduct>())).ReturnsAsync(BareBoneData());

            await sut.UpdateInventory(languageXml, inventoryXml);

            VerifyLogWarningCalled();
            IInvocation? logInvocation = mockedLogger.Invocations.FirstOrDefault(x =>
            {
                var message = x.Arguments[2].ToString();
                return !string.IsNullOrWhiteSpace(message) && message.ToLowerInvariant()
                    .Contains(expectedLogMessageFragment.ToLowerInvariant());
            });
            logInvocation.Should().NotBeNull();
        }

        [Fact]
        public async void ItLogsWarningWhenMultipleProductsWithSameBaseLinkIsGenerated()
        {
            (string inventoryXml, var languageXml) =
                GetSutArguments(nameof(ItLogsWarningWhenMultipleProductsWithSameBaseLinkIsGenerated));

            const string expectedLogMessageFragment = "multiple Products with the same";
            var sut = new LoaderService(mockedLogger.Object, mockedQueryManager.Object);
            mockedQueryManager.Setup(x => x.GetEntities(It.IsAny<SearchableProduct>())).ReturnsAsync(BareBoneData());

            await sut.UpdateInventory(languageXml, inventoryXml);

            VerifyLogWarningCalled();
            IInvocation? logInvocation = mockedLogger.Invocations.FirstOrDefault(x =>
            {
                var message = x.Arguments[2].ToString();
                return !string.IsNullOrWhiteSpace(message) && message.ToLowerInvariant()
                    .Contains(expectedLogMessageFragment.ToLowerInvariant());
            });
            logInvocation.Should().NotBeNull();
        }

        [Fact]
        public async void ItLogsWarningWhenMultipleSizesWithSameEanIsGenerated()
        {
            (string inventoryXml, var languageXml) =
                GetSutArguments(nameof(ItLogsWarningWhenMultipleSizesWithSameEanIsGenerated));

            const string expectedLogMessageFragment = "sizes with same";
            var sut = new LoaderService(mockedLogger.Object, mockedQueryManager.Object);
            mockedQueryManager.Setup(x => x.GetEntities(It.IsAny<SearchableProduct>())).ReturnsAsync(BareBoneData());

            await sut.UpdateInventory(languageXml, inventoryXml);

            VerifyLogWarningCalled();
            IInvocation? logInvocation = mockedLogger.Invocations.FirstOrDefault(x =>
            {
                var message = x.Arguments[2].ToString();
                return !string.IsNullOrWhiteSpace(message) && message.ToLowerInvariant()
                    .Contains(expectedLogMessageFragment.ToLowerInvariant());
            });
            logInvocation.Should().NotBeNull();
        }

        [Fact]
        public async void ItLogsWarningWhenNoSizesGetsAttached()
        {
            (string inventoryXml, var languageXml) = GetSutArguments(nameof(ItLogsWarningWhenNoSizesGetsAttached));
            const string expectedLogMessageFragment = "sizes attached";
            var sut = new LoaderService(mockedLogger.Object, mockedQueryManager.Object);

            await sut.UpdateInventory(languageXml, inventoryXml);

            VerifyLogWarningCalled();
            IInvocation? logInvocation = mockedLogger.Invocations.FirstOrDefault(x =>
            {
                var message = x.Arguments[2].ToString();
                return !string.IsNullOrWhiteSpace(message) && message.ToLowerInvariant()
                    .Contains(expectedLogMessageFragment.ToLowerInvariant());
            });
            logInvocation.Should().NotBeNull();
        }

        [Fact]
        public async void ItLogsWarningWhenPriceIsDiscardedDueToInvalidListingPrice()
        {
            (string inventoryXml, var languageXml) =
                GetSutArguments(nameof(ItLogsWarningWhenPriceIsDiscardedDueToInvalidListingPrice));
            const string expectedLogMessageFragment = "needs either a normal or discount listing price to be valid";
            var sut = new LoaderService(mockedLogger.Object, mockedQueryManager.Object);

            await sut.UpdateInventory(languageXml, inventoryXml);

            VerifyLogWarningCalled();
            IInvocation? logInvocation = mockedLogger.Invocations.FirstOrDefault(x =>
            {
                var message = x.Arguments[2].ToString();
                return !string.IsNullOrWhiteSpace(message) && message.ToLowerInvariant()
                    .Contains(expectedLogMessageFragment.ToLowerInvariant());
            });
            logInvocation.Should().NotBeNull();
        }

        [Fact]
        public async void ItLogsWarningWhenSizeDoesNotHaveValidEan()
        {
            (string inventoryXml, var languageXml) = GetSutArguments(nameof(ItLogsWarningWhenSizeDoesNotHaveValidEan));
            const string expectedLogMessageFragment = "missing a valid ean";
            var sut = new LoaderService(mockedLogger.Object, mockedQueryManager.Object);

            await sut.UpdateInventory(languageXml, inventoryXml);

            VerifyLogWarningCalled();
            IInvocation? logInvocation = mockedLogger.Invocations.FirstOrDefault(x =>
            {
                var message = x.Arguments[2].ToString();
                return !string.IsNullOrWhiteSpace(message) && message.ToLowerInvariant()
                    .Contains(expectedLogMessageFragment.ToLowerInvariant());
            });
            logInvocation.Should().NotBeNull();
        }

        [Fact]
        public async void ItLogsWarningWhenSizeHasNoneLeft()
        {
            (string inventoryXml, var languageXml) = GetSutArguments(nameof(ItLogsWarningWhenSizeHasNoneLeft));
            const string expectedLogMessageFragment = "has none left";
            var sut = new LoaderService(mockedLogger.Object, mockedQueryManager.Object);

            await sut.UpdateInventory(languageXml, inventoryXml);

            VerifyLogWarningCalled();
            IInvocation? logInvocation = mockedLogger.Invocations.FirstOrDefault(x =>
            {
                var message = x.Arguments[2].ToString();
                return !string.IsNullOrWhiteSpace(message) && message.ToLowerInvariant()
                    .Contains(expectedLogMessageFragment.ToLowerInvariant());
            });
            logInvocation.Should().NotBeNull();
        }

        [Fact]
        public async void ItLogsWarningWhenTagIsDiscardedDueToEmptyValue()
        {
            (string inventoryXml, var languageXml) =
                GetSutArguments(nameof(ItLogsWarningWhenTagIsDiscardedDueToEmptyValue));
            const string expectedLogMessageFragment = "value is empty";
            var sut = new LoaderService(mockedLogger.Object, mockedQueryManager.Object);

            await sut.UpdateInventory(languageXml, inventoryXml);

            VerifyLogWarningCalled();
            IInvocation? logInvocation = mockedLogger.Invocations.FirstOrDefault(x =>
            {
                var message = x.Arguments[2].ToString();
                return !string.IsNullOrWhiteSpace(message) && message.ToLowerInvariant()
                    .Contains(expectedLogMessageFragment.ToLowerInvariant());
            });
            logInvocation.Should().NotBeNull();
        }

        [Fact]
        public async void ItSavesGeneratedProducts()
        {
            (string inventoryXml, var languageXml) = GetSutArguments(nameof(ItSavesGeneratedProducts));
            var expectedProducts = BareBoneData();
            var sut = new LoaderService(mockedLogger.Object, mockedQueryManager.Object);

            await sut.UpdateInventory(languageXml, inventoryXml);

            mockedQueryManager.Verify(x => x.AddEntities(It.IsAny<ICollection<Product>>()));
            IInvocation addEntitiesInvocation = mockedQueryManager.Invocations.First(x => x.IsVerified);
            object addEntitiesParameter = addEntitiesInvocation.Arguments[0];
            addEntitiesParameter.Should().BeEquivalentTo(expectedProducts);
        }

        private ICollection<Product> BareBoneData()
        {
            return TestEntitiesBuilder.BuildProducts([new Product(),]);
        }


        private (string, Dictionary<string, string>) GetSutArguments(string testName)
        {
            string inventoryXmlFile = BuildInventoryTestFilePath(testName);
            string languageXmlFile = BuildLanguageTestFilePath(testName);
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

        private ICollection<Product> ItAddsNewLocaleProductDuringUpdateDatabaseData()
        {
            return TestEntitiesBuilder.BuildProducts([
                new Product
                {
                    Locales = new List<ILocaleProduct>
                    {
                        TestEntitiesBuilder.BuildLocaleProduct(1, 1, "en", "Horse X - T", "EN", "BLACK"),
                    },
                },
            ]);
        }

        private ICollection<Product> ItAddsNewLocaleProductDuringUpdateExpectedData()
        {
            return TestEntitiesBuilder.BuildProducts([
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

        private ICollection<Product> ItAddsNewSizeDuringUpdateDatabaseData()
        {
            return TestEntitiesBuilder.BuildProducts([
                new Product
                {
                    Sizes = new List<ISize>
                    {
                        TestEntitiesBuilder.BuildSize(1, 2, 5769403877380),
                    },
                },
            ]);
        }

        private ICollection<Product> ItAddsNewSizeDuringUpdateExpectedData()
        {
            return TestEntitiesBuilder.BuildProducts([
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

        private ICollection<Product> ItDeletesProductsThatNoLongerExistDatabaseData()
        {
            return TestEntitiesBuilder.BuildProducts([
                new Product {ReferenceId = 1,},
                new Product {ReferenceId = 2,},
                new Product {ReferenceId = 3,},
                new Product {ReferenceId = 4,},
            ]);
        }

        private ICollection<Product> ItDeletesProductsThatNoLongerExistExpectedData()
        {
            return TestEntitiesBuilder.BuildProducts([
                new Product {ReferenceId = 3,},
                new Product {ReferenceId = 4,},
            ]);
        }

        private string LoadXmlFileContent(string fileName)
        {
            return File.ReadAllText(Path.Combine(@"..\..\..\", fileName));
        }

        /// <summary>
        ///     Found here:
        ///     https://stackoverflow.com/questions/62091109/how-to-verify-log-message-in-unit-testing-for-a-passing-test
        /// </summary>
        private void VerifyLogWarningCalled()
        {
            mockedLogger.Verify(
                x => x.Log(It.Is<LogLevel>(l => l == LogLevel.Warning), It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((obj, type) => true), It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception?, string>>((obj, type) => true)), Times.AtLeastOnce());
        }
    }
}