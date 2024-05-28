using Fashionhero.Portal.Shared.Abstraction.Enums;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Entity;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Service;
using Fashionhero.Portal.Shared.Model.Entity;
using Jakubqwe.CurrencyConverter;
using Jakubqwe.CurrencyConverter.CurrencyRateProviders;
using Microsoft.Extensions.Logging;

namespace Fashionhero.Portal.BusinessLogic.Services
{
    public class CurrencyConverterService : ICurrencyConverterService
    {
        private readonly ILogger<CurrencyConverterService> logger;
        private readonly EcbCurrencyProvider provider;
        private Dictionary<(CurrencyCode fromCurrencyCode, CurrencyCode toCurrencyCode), decimal> rates;
        private bool hasGatheredDefaultRates = false;

        public CurrencyConverterService(ILogger<CurrencyConverterService> logger)
        {
            this.logger = logger;
            provider = new EcbCurrencyProvider();
            rates = new Dictionary<(CurrencyCode fromCurrencyCode, CurrencyCode toCurrencyCode), decimal>();
            PopulateDefaultRates();
        }

        private async Task PopulateDefaultRates()
        {
            await GetRate(CurrencyCode.DKK, CurrencyCode.EUR);
            hasGatheredDefaultRates = true;
        }

        /// <inheritdoc />
        public async Task<IPrice> ConvertPrice(IPrice fromPrice, CurrencyCode toCurrency)
        {
            while (!hasGatheredDefaultRates)
                Thread.Sleep(100);

            decimal rate = await GetRate(fromPrice.Currency, toCurrency);

            return new Price()
            {
                Discount = fromPrice.Discount != null ? Math.Round((decimal) (fromPrice.Discount * rate), 2) : default,
                NormalSell = Math.Round(fromPrice.NormalSell * rate, 2),
                Currency = toCurrency,
            };
        }

        public async Task<decimal> GetRate(CurrencyCode fromCode, CurrencyCode toCode)
        {
            if (rates.ContainsKey((fromCode, toCode)))
                return rates[(fromCode, toCode)];

            decimal newRate = await provider.GetRateAsync(ConvertCurrencyCodeToJakubweVersion(fromCode),
                ConvertCurrencyCodeToJakubweVersion(toCode));
            logger.LogInformation($"Caching currency rate for {fromCode} to {toCode}. Rate: {newRate}");
            rates.Add((fromCode, toCode), newRate);

            return newRate;
        }

        private static Currency ConvertCurrencyCodeToJakubweVersion(CurrencyCode code)
        {
            return Enum.Parse<Currency>(code.ToString());
        }
    }
}