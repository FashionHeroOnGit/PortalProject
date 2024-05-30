using Fashionhero.Portal.BusinessLogic.Services;
using Fashionhero.Portal.BusinessLogic.Spartoo;
using Fashionhero.Portal.BusinessLogic.Test.Core;
using Fashionhero.Portal.Shared.Abstraction.Enums;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Entity;
using Fashionhero.Portal.Shared.Model.Entity;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Fashionhero.Portal.BusinessLogic.Test.Spartoo
{
    public class CurrencyFilterTests
    {
        private readonly Mock<ILogger<SpartooService>> mockedLogger;

        public CurrencyFilterTests()
        {
            mockedLogger = new Mock<ILogger<SpartooService>>();
        }

        [Fact]
        public void ItSaysFalseWhenAskedIfFilterIsAnythingOtherThanCurrencyFilter()
        {
            const bool expected = false;
            var sut = new CurrencyFilter();

            bool actual = sut.IsFilterOfType(FilterType.SPARTOO_TYPE);

            actual.Should().Be(expected);
        }

        [Fact]
        public void ItSaysTrueWhenAskedIfFilterIsCurrencyFilter()
        {
            const bool expected = true;
            var sut = new CurrencyFilter();

            bool actual = sut.IsFilterOfType(FilterType.SPARTOO_CURRENCY);

            actual.Should().Be(expected);
        }

        [Fact]
        public void ItMakesNoChangesWhenApplyingTheFilterToProducts()
        {
            var expected = ItMakesNoChangesWhenApplyingTheFilterToProductsExpectedData();
            var original = ItMakesNoChangesWhenApplyingTheFilterToProductsOriginalData();
            var sut = new CurrencyFilter();

            var actual = sut.FilterProducts(original, mockedLogger.Object);

            actual.Should().BeEquivalentTo(expected);
        }

        private ICollection<IProduct> ItMakesNoChangesWhenApplyingTheFilterToProductsExpectedData()
        {
            return TestEntitiesBuilder.BuildProducts([]).Cast<IProduct>().ToList();
        }

        private ICollection<IProduct> ItMakesNoChangesWhenApplyingTheFilterToProductsOriginalData()
        {
            return TestEntitiesBuilder.BuildProducts([
                new Product() {Prices = new List<IPrice>() {new Price() {Currency = CurrencyCode.USD,},},},
                new Product() {Prices = new List<IPrice>() {new Price() {Currency = CurrencyCode.USD,},},},
            ]).Cast<IProduct>().ToList();
        }
    }
}