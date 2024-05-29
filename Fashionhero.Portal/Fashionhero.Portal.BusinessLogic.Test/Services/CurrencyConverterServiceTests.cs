using Fashionhero.Portal.BusinessLogic.Services;
using Fashionhero.Portal.Shared.Abstraction.Enums;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Entity;
using Fashionhero.Portal.Shared.Model.Entity;
using FluentAssertions;
using Jakubqwe.CurrencyConverter;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Fashionhero.Portal.BusinessLogic.Test.Services
{
    public class CurrencyConverterServiceTests
    {
        private readonly Mock<ILogger<CurrencyConverterService>> mockedLogger;
        private readonly Mock<ICurrencyRateProvider> mockedProvider;

        public CurrencyConverterServiceTests()
        {
            mockedLogger = new Mock<ILogger<CurrencyConverterService>>();
            mockedProvider = new Mock<ICurrencyRateProvider>();

            mockedProvider.Setup(x => x.GetRate(Currency.DKK, Currency.EUR)).Returns(0.13m);
        }

        [Fact]
        public async void ItConvertsPriceToTargetCurrency()
        {
            IPrice originalPrice = new Price {Currency = CurrencyCode.DKK, NormalSell = 1500, Discount = 750,};
            IPrice expectedPrice = new Price {Currency = CurrencyCode.EUR, NormalSell = 195, Discount = 97.5m,};
            var sut = new CurrencyConverterService(mockedLogger.Object, mockedProvider.Object);

            IPrice actualPrice = await sut.ConvertPrice(originalPrice, CurrencyCode.EUR);

            actualPrice.Should().BeEquivalentTo(expectedPrice);
        }

        [Fact]
        public async void ItGetsTheRateFromOneCurrencyToAnother()
        {
            const decimal expectedRate = 0.13m;
            var sut = new CurrencyConverterService(mockedLogger.Object, mockedProvider.Object);

            decimal actualRate = await sut.GetRate(CurrencyCode.DKK, CurrencyCode.EUR);

            actualRate.Should().Be(expectedRate);
        }

        [Fact]
        public async void ItGetsTheRateFromOneCurrencyToAnotherOnlyOnce()
        {
            const decimal expectedRate = 0.13m;
            const int expectedCallAmount = 1;
            var sut = new CurrencyConverterService(mockedLogger.Object, mockedProvider.Object);

            decimal actualRate1 = await sut.GetRate(CurrencyCode.DKK, CurrencyCode.EUR);
            decimal actualRate2 = await sut.GetRate(CurrencyCode.DKK, CurrencyCode.EUR);

            mockedProvider.Invocations.Count.Should().Be(expectedCallAmount);
            actualRate1.Should().Be(expectedRate);
            actualRate2.Should().Be(expectedRate);
        }
    }
}