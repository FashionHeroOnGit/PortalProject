using Fashionhero.Portal.Shared.Abstraction.Enums;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Entity;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Service;
using Fashionhero.Portal.Shared.Model.Entity;
using Jakubqwe.CurrencyConverter;
using Microsoft.Extensions.Logging;

namespace Fashionhero.Portal.BusinessLogic.Services
{
    public class CurrencyConverterService : ICurrencyConverterService
    {
        private readonly ILogger<CurrencyConverterService> logger;
        private readonly ICurrencyRateProvider provider;
        private readonly Dictionary<(CurrencyCode fromCurrencyCode, CurrencyCode toCurrencyCode), decimal> rates;
        private bool hasGatheredDefaultRates;

        public CurrencyConverterService(ILogger<CurrencyConverterService> logger, ICurrencyRateProvider provider)
        {
            this.logger = logger;
            this.provider = provider;
            rates = new Dictionary<(CurrencyCode fromCurrencyCode, CurrencyCode toCurrencyCode), decimal>();
            PopulateDefaultRates();
        }

        /// <inheritdoc />
        public async Task<IPrice> ConvertPrice(IPrice fromPrice, CurrencyCode toCurrency)
        {
            while (!hasGatheredDefaultRates)
                Thread.Sleep(100);

            decimal rate = await GetRate(fromPrice.Currency, toCurrency);

            return new Price
            {
                Discount = fromPrice.Discount != null ? Math.Round((decimal) (fromPrice.Discount * rate), 2) : default,
                NormalSell = Math.Round(fromPrice.NormalSell * rate, 2),
                Currency = toCurrency,
            };
        }

        public Task<decimal> GetRate(CurrencyCode fromCode, CurrencyCode toCode)
        {
            if (rates.ContainsKey((fromCode, toCode)))
                return Task.FromResult(rates[(fromCode, toCode)]);

            decimal newRate = provider.GetRate(ConvertCurrencyCodeToJakubweVersion(fromCode),
                ConvertCurrencyCodeToJakubweVersion(toCode));
            logger.LogInformation($"Caching currency rate for {fromCode} to {toCode}. Rate: {newRate}");
            rates.Add((fromCode, toCode), newRate);

            return Task.FromResult(newRate);
        }

        private static Currency ConvertCurrencyCodeToJakubweVersion(CurrencyCode code)
        {
            return Enum.Parse<Currency>(code.ToString());
        }

        private async Task PopulateDefaultRates()
        {
            await GetRate(CurrencyCode.DKK, CurrencyCode.EUR);
            hasGatheredDefaultRates = true;
        }
    }
}