using Fashionhero.Portal.BusinessLogic.Spartoo;
using Fashionhero.Portal.BusinessLogic.Test.Core;
using Fashionhero.Portal.BusinessLogic.Test.Extensions;
using Fashionhero.Portal.Shared.Abstraction.Enums;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Entity;
using Fashionhero.Portal.Shared.Model.Entity;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Fashionhero.Portal.BusinessLogic.Test.Spartoo
{
    public class ProductTypeMappedFilterTests
    {
        private readonly Mock<ILogger<ProductTypeMappedFilter>> mockedLogger;

        public ProductTypeMappedFilterTests()
        {
            mockedLogger = new Mock<ILogger<ProductTypeMappedFilter>>();
        }

        private static ICollection<IProduct> GenerateInvalidProductsWithInvalidType()
        {
            return TestEntitiesBuilder.BuildProducts([
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
                    Locales = new List<ILocaleProduct>
                    {
                        new LocaleProduct
                        {
                            IsoName = "dk",
                            Type = "invalid",
                        },
                    },
                },
            ]).Cast<IProduct>().ToList();
        }

        private static ICollection<IProduct> GenerateInvalidProductsWithoutDanishTranslation()
        {
            return TestEntitiesBuilder.BuildProducts([
                new Product
                {
                    Locales = new List<ILocaleProduct>
                    {
                        new LocaleProduct
                        {
                            IsoName = "en",
                        },
                    },
                },
                new Product
                {
                    Locales = new List<ILocaleProduct>
                    {
                        new LocaleProduct
                        {
                            IsoName = "en",
                        },
                    },
                },
            ]).Cast<IProduct>().ToList();
        }

        private static ICollection<IProduct> GenerateValidProducts()
        {
            return TestEntitiesBuilder.BuildProducts([
                new Product
                {
                    Locales = new List<ILocaleProduct>
                    {
                        new LocaleProduct
                        {
                            IsoName = "dk",
                            Type = "t-shirts",
                        },
                    },
                },
                new Product
                {
                    Locales = new List<ILocaleProduct>
                    {
                        new LocaleProduct
                        {
                            IsoName = "dk",
                            Type = "t-shirts",
                        },
                    },
                },
            ]).Cast<IProduct>().ToList();
        }

        [Fact]
        public void ItDoesNotRemovesProductsWhenApplyingTheFilter()
        {
            var expected = GenerateValidProducts();
            var original = GenerateValidProducts();
            var sut = new ProductTypeMappedFilter(mockedLogger.Object);

            var actual = sut.FilterProducts(original);

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ItLogsWarningWhenFilterDiscardsProductBecauseOfInvalidType()
        {
            const string expectedLogMessageFragment = "is not convertible to a";
            var original = GenerateInvalidProductsWithInvalidType();
            var sut = new ProductTypeMappedFilter(mockedLogger.Object);

            sut.FilterProducts(original);

            mockedLogger.VerifyLogWarningCalled();
            IInvocation? logInvocation = mockedLogger.TryGetInvocation(expectedLogMessageFragment);
            logInvocation.Should().NotBeNull();
        }

        [Fact]
        public void ItLogsWarningWhenFilterDiscardsProductBecauseOfMissingDanishTranslation()
        {
            const string expectedLogMessageFragment = "as no Danish Translation of products";
            var original = GenerateInvalidProductsWithoutDanishTranslation();
            var sut = new ProductTypeMappedFilter(mockedLogger.Object);

            sut.FilterProducts(original);

            mockedLogger.VerifyLogWarningCalled();
            IInvocation? logInvocation = mockedLogger.TryGetInvocation(expectedLogMessageFragment);
            logInvocation.Should().NotBeNull();
        }

        [Fact]
        public void ItRemovesInvalidProductsWhenApplyingTheFilter()
        {
            var expected = TestEntitiesBuilder.GenerateEmptyProductsList();
            var original = GenerateInvalidProductsWithoutDanishTranslation();
            var sut = new ProductTypeMappedFilter(mockedLogger.Object);

            var actual = sut.FilterProducts(original);

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ItReturnsDefaultValueWhenKeyDoesNotExistInDictionary()
        {
            const int expected = default;
            var sut = new ProductTypeMappedFilter(mockedLogger.Object);

            object actual = sut.GetDictionaryValue("some random text that is not a key in the dictionary");

            actual.Should().BeOfType<int>();
            actual.Should().Be(expected);
        }

        [Fact]
        public void ItReturnsValueFromDictionaryWhenKeyExists()
        {
            const int expected = 10056;
            var sut = new ProductTypeMappedFilter(mockedLogger.Object);

            object actual = sut.GetDictionaryValue("t-shirts");

            actual.Should().BeOfType<int>();
            actual.Should().Be(expected);
        }

        [Fact]
        public void ItSaysFalseWhenAskedIfFilterIsAnythingOtherThanProductTypeFilter()
        {
            const bool expected = false;
            var sut = new ProductTypeMappedFilter(mockedLogger.Object);

            bool actual = sut.IsFilterOfType(FilterType.SPARTOO_CURRENCY);

            actual.Should().Be(expected);
        }

        [Fact]
        public void ItSaysFalseWhenAskedIfMapperIsAnythingOtherThanProductTypeMapper()
        {
            const bool expected = false;
            var sut = new ProductTypeMappedFilter(mockedLogger.Object);

            bool actual = sut.IsMapperOfType(MapType.SPARTOO_COLOUR);

            actual.Should().Be(expected);
        }

        [Fact]
        public void ItSaysTrueWhenAskedIfFilterIsProductTypeFilter()
        {
            const bool expected = true;
            var sut = new ProductTypeMappedFilter(mockedLogger.Object);

            bool actual = sut.IsFilterOfType(FilterType.SPARTOO_TYPE);

            actual.Should().Be(expected);
        }

        [Fact]
        public void ItSaysTrueWhenAskedIfMapperIsProductTypeMapper()
        {
            const bool expected = true;
            var sut = new ProductTypeMappedFilter(mockedLogger.Object);

            bool actual = sut.IsMapperOfType(MapType.SPARTOO_TYPE);

            actual.Should().Be(expected);
        }
    }
}