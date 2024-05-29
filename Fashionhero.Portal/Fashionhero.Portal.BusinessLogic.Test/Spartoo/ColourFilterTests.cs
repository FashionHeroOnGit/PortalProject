using Fashionhero.Portal.BusinessLogic.Services;
using Fashionhero.Portal.BusinessLogic.Spartoo;
using Fashionhero.Portal.BusinessLogic.Test.Core;
using Fashionhero.Portal.Shared.Abstraction.Enums.Spartoo;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Entity;
using Fashionhero.Portal.Shared.Model.Entity;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Fashionhero.Portal.BusinessLogic.Test.Spartoo
{
    public class ColourFilterTests
    {
        private readonly Mock<ILogger<SpartooService>> mockedLogger;

        public ColourFilterTests()
        {
            mockedLogger = new Mock<ILogger<SpartooService>>();
        }

        [Fact]
        public void ItSaysFalseWhenAskedIfFilterIsAnythingOtherThanColourFilter()
        {
            const bool expected = false;
            var sut = new ColourFilter();

            bool actual = sut.IsFilterOfType(FilterType.TYPE);

            actual.Should().Be(expected);
        }

        [Fact]
        public void ItSaysTrueWhenAskedIfFilterIsColourFilter()
        {
            const bool expected = true;
            var sut = new ColourFilter();

            bool actual = sut.IsFilterOfType(FilterType.COLOUR);

            actual.Should().Be(expected);
        }

        [Fact]
        public void ItReturnsValueFromDictionaryWhenKeyExists()
        {
            var expected = 1;
            var sut = new ColourFilter();

            object? actual = sut.GetDictionaryValue("hvid");

            actual.Should().NotBeNull();
            actual.Should().BeOfType<int>();
            actual.Should().Be(expected);
        }

        [Fact]
        public void ItReturnsDefaultValueWhenKeyDoesNotExistInDictionary()
        {
            var expected = 534;
            var sut = new ColourFilter();

            object? actual = sut.GetDictionaryValue("some random text that is not a key in the dictionary");

            actual.Should().NotBeNull();
            actual.Should().BeOfType<int>();
            actual.Should().Be(expected);
        }

        [Fact]
        public void ItMakesNoChangesWhenApplyingTheFilterToProducts()
        {
            var expected = ItMakesNoChangesWhenApplyingTheFilterToProductsData();
            var original = ItMakesNoChangesWhenApplyingTheFilterToProductsData();
            var sut = new ColourFilter();

            var actual = sut.FilterProducts(original, mockedLogger.Object);

            actual.Should().BeEquivalentTo(expected);
        }

        private ICollection<IProduct> ItMakesNoChangesWhenApplyingTheFilterToProductsData()
        {
            return TestEntitiesBuilder.BuildProducts([new Product(), new Product(),]).Cast<IProduct>().ToList();
        }
    }
}