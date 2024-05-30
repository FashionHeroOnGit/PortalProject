using Fashionhero.Portal.BusinessLogic.Services;
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
    public class CurrencyFilterTests
    {
        private readonly Mock<ILogger<CurrencyFilter>> mockedLogger;

        public CurrencyFilterTests()
        {
            mockedLogger = new Mock<ILogger<CurrencyFilter>>();
        }

        [Fact]
        public void ItSaysFalseWhenAskedIfFilterIsAnythingOtherThanCurrencyFilter()
        {
            const bool expected = false;
            var sut = new CurrencyFilter(mockedLogger.Object);

            bool actual = sut.IsFilterOfType(FilterType.SPARTOO_TYPE);

            actual.Should().Be(expected);
        }

        [Fact]
        public void ItSaysTrueWhenAskedIfFilterIsCurrencyFilter()
        {
            const bool expected = true;
            var sut = new CurrencyFilter(mockedLogger.Object);

            bool actual = sut.IsFilterOfType(FilterType.SPARTOO_CURRENCY);

            actual.Should().Be(expected);
        }

        [Fact]
        public void ItRemovesInvalidProductsWhenApplyingTheFilter()
        {
            var expected = GenerateEmptyProductsList();
            var original = GenerateInvalidProducts();
            var sut = new CurrencyFilter(mockedLogger.Object);

            var actual = sut.FilterProducts(original);

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ItLogsWarningWhenFilterDiscardsProduct()
        {
            var expectedLogMessageFragment = "as it does not have a";
            var original = GenerateInvalidProducts();
            var sut = new CurrencyFilter(mockedLogger.Object);

            sut.FilterProducts(original);

            mockedLogger.VerifyLogWarningCalled();
            IInvocation? logInvocation = mockedLogger.TryGetInvocation(expectedLogMessageFragment);
            logInvocation.Should().NotBeNull();
        }

        private ICollection<IProduct> GenerateEmptyProductsList()
        {
            return TestEntitiesBuilder.BuildProducts([]).Cast<IProduct>().ToList();
        }

        private ICollection<IProduct> GenerateInvalidProducts()
        {
            return TestEntitiesBuilder.BuildProducts([
                new Product() {Prices = new List<IPrice>() {new Price() {Currency = CurrencyCode.USD,},},},
                new Product() {Prices = new List<IPrice>() {new Price() {Currency = CurrencyCode.USD,},},},
            ]).Cast<IProduct>().ToList();
        }
    }
}