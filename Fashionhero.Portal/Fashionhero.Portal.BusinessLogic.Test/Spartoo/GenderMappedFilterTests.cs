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
    public class GenderMappedFilterTests
    {
        private readonly Mock<ILogger<GenderMappedFilter>> mockedLogger;

        public GenderMappedFilterTests()
        {
            mockedLogger = new Mock<ILogger<GenderMappedFilter>>();
        }

        [Fact]
        public void ItDoesNotRemovesProductsWhenApplyingTheFilter()
        {
            var expected = GenerateValidProducts();
            var original = GenerateValidProducts();
            var sut = new GenderMappedFilter(mockedLogger.Object);

            var actual = sut.FilterProducts(original);

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ItLogsWarningWhenFilterDiscardsProductBecauseOfInvalidGender()
        {
            const string expectedLogMessageFragment = "is not convertible to a";
            var original = GenerateInvalidProductsWithInvalidGenders();
            var sut = new GenderMappedFilter(mockedLogger.Object);

            sut.FilterProducts(original);

            mockedLogger.VerifyLogWarningCalled();
            IInvocation? logInvocation = mockedLogger.TryGetInvocation(expectedLogMessageFragment);
            logInvocation.Should().NotBeNull();
        }

        [Fact]
        public void ItLogsWarningWhenFilterDiscardsProductBecauseOfMissingDanishTranslation()
        {
            const string expectedLogMessageFragment = "as no Danish Translation of products";
            var original = GenerateInvalidProductsWithMissingDanishTranslation();
            var sut = new GenderMappedFilter(mockedLogger.Object);

            sut.FilterProducts(original);

            mockedLogger.VerifyLogWarningCalled();
            IInvocation? logInvocation = mockedLogger.TryGetInvocation(expectedLogMessageFragment);
            logInvocation.Should().NotBeNull();
        }

        [Fact]
        public void ItRemovesInvalidProductsWhenApplyingTheFilter()
        {
            var expected = GenerateEmptyProductsList();
            var original = GenerateInvalidProductsWithMissingDanishTranslation();
            var sut = new GenderMappedFilter(mockedLogger.Object);

            var actual = sut.FilterProducts(original);

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ItReturnsDefaultValueWhenKeyDoesNotExistInDictionary()
        {
            const char expected = default;
            var sut = new GenderMappedFilter(mockedLogger.Object);

            object actual = sut.GetDictionaryValue("some random text that is not a key in the dictionary");

            actual.Should().BeOfType<char>();
            actual.Should().Be(expected);
        }

        [Fact]
        public void ItReturnsValueFromDictionaryWhenKeyExists()
        {
            const char expected = 'H';
            var sut = new GenderMappedFilter(mockedLogger.Object);

            object actual = sut.GetDictionaryValue("Mand");

            actual.Should().BeOfType<char>();
            actual.Should().Be(expected);
        }

        [Fact]
        public void ItSaysFalseWhenAskedIfFilterIsAnythingOtherThanGenderFilter()
        {
            const bool expected = false;
            var sut = new GenderMappedFilter(mockedLogger.Object);

            bool actual = sut.IsFilterOfType(FilterType.SPARTOO_CURRENCY);

            actual.Should().Be(expected);
        }

        [Fact]
        public void ItSaysFalseWhenAskedIfFilterIsAnythingOtherThanGenderMapper()
        {
            const bool expected = false;
            var sut = new GenderMappedFilter(mockedLogger.Object);

            bool actual = sut.IsMapperOfType(MapType.SPARTOO_COLOUR);

            actual.Should().Be(expected);
        }

        [Fact]
        public void ItSaysTrueWhenAskedIfFilterIsGenderFilter()
        {
            const bool expected = true;
            var sut = new GenderMappedFilter(mockedLogger.Object);

            bool actual = sut.IsFilterOfType(FilterType.SPARTOO_GENDER);

            actual.Should().Be(expected);
        }

        [Fact]
        public void ItSaysTrueWhenAskedIfFilterIsGenderMapper()
        {
            const bool expected = true;
            var sut = new GenderMappedFilter(mockedLogger.Object);

            bool actual = sut.IsMapperOfType(MapType.SPARTOO_GENDER);

            actual.Should().Be(expected);
        }

        private static ICollection<IProduct> GenerateEmptyProductsList()
        {
            return TestEntitiesBuilder.BuildProducts([]).Cast<IProduct>().ToList();
        }

        private static ICollection<IProduct> GenerateInvalidProductsWithInvalidGenders()
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
                    Locales = new List<ILocaleProduct>
                    {
                        new LocaleProduct
                        {
                            IsoName = "dk",
                            Gender = "invalid",
                        },
                    },
                },
            ]).Cast<IProduct>().ToList();
        }

        private static ICollection<IProduct> GenerateInvalidProductsWithMissingDanishTranslation()
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
                            Gender = "Mand",
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
                            Gender = "Mand",
                        },
                    },
                },
            ]).Cast<IProduct>().ToList();
        }
    }
}