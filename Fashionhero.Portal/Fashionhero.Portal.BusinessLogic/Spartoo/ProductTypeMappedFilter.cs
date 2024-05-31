using Fashionhero.Portal.Shared.Abstraction.Enums;
using Fashionhero.Portal.Shared.Abstraction.Interfaces;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Entity;
using Fashionhero.Portal.Shared.Model;
using Fashionhero.Portal.Shared.Model.Entity;
using Microsoft.Extensions.Logging;

namespace Fashionhero.Portal.BusinessLogic.Spartoo
{
    public class ProductTypeMappedFilter : IFilter, IMapper
    {
        private readonly ILogger<ProductTypeMappedFilter> logger;
        private readonly Dictionary<string, int> productStyleMap;

        public ProductTypeMappedFilter(ILogger<ProductTypeMappedFilter> logger)
        {
            this.logger = logger;
            productStyleMap = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase)
            {
                {"bjoernborg-underbukser", 11475},
                {"claudio-underbukser", 11475},
                {"cristianoronaldo-underbukser", 11475},
                {"muchachomalo-underbukser", 11475},
                {"selected-underbukser", 11475},
                {"jackjones-underbukser", 11475},
                {"armani-underbukser", 11475},
                {"endurance-underbukser", 11475},
                {"tommyhilfiger-underbukser", 11475},
                {"ralphlauren-underbukser", 11475},
                {"comfyballs", 11475},
                {"jbs-t-shirts", 10056},
                {"claudio-t-shirts", 10056},
                {"calvinklein-t-shirts", 10056},
                {"tommyhilfiger-t-shirts", 10056},
                {"ralphlauren-t-shirts", 10056},
                {"resteroeds-t-shirts", 10056},
                {"undertoej", 10056},
                {"underbukser", 11475},
                {"jbs-underbukser", 11475},
                {"undertroejer", 10056},
                {"jbs-undertroejer", 10056},
                {"t-shirts", 10056},
                {"jbs-t-shirts-2", 10056},
                {"hugo-boss-underbukser", 11475},
                {"resterods-underbukser", 11475},
                {"resterods-t-shirts", 10056},
                {"resterods-undertroejer", 10056},
                {"troejer", 10054},
                {"resterods-troejer", 10054},
                {"sokker", 10087},
                {"jbs-sokker", 10087},
                {"claudio-undertoej", 10056},
                {"claudio-underbukser-2", 11475},
                {"muchachomalo-undertoej", 10056},
                {"muchachomalo-underbukser-2", 11475},
                {"claudio-sokker", 10087},
                {"hugo-boss-t-shirts", 10056},
                {"hugo-boss-sokker", 10087},
                {"bjoern-borg-undertoej", 10056},
                {"bjoern-borg-underbukser", 11475},
                {"underbukser-boern", 11475},
                {"bjoern-borg-underbukser-boern", 11475},
                {"cristiano-ronaldo-underbukser", 11475},
                {"cristiano-ronaldo-sokker", 10087},
                {"claudio-undertroejer", 10056},
                {"sokker-boern", 10087},
                {"cristiano-ronaldo-sokker-boern", 10087},
                {"hugo-boss-undertroejer", 10056},
                {"calvin-klein-undertoej", 10056},
                {"calvin-klein-underbukser", 11475},
                {"calvin-klein-t-shirts", 10056},
                {"calvin-klein-undertroejer", 10056},
                {"hugo-boss-troejer", 10054},
                {"badetoej", 10778},
                {"calvin-klein-badetoej", 10778},
                {"claudio-t-shirts-2", 10056},
                {"jbs-badetoej", 10778},
                {"hugo-boss-badetoej", 10778},
                {"selected-undertoej", 10056},
                {"selected-underbukser-2", 11475},
                {"cristiano-ronaldo-underbukser-boern", 11475},
                {"selected-undertroejer", 10056},
                {"armani-undertoej", 10056},
                {"armani-underbukser-2", 11475},
                {"armani-sokker", 10087},
                {"jbs-underbukser-boern", 11475},
                {"endurance-undertoej", 10056},
                {"endurance-underbukser-2", 11475},
                {"g-strenge", 11406},
                {"armani-g-strenge", 11406},
                {"iq-sox", 10087},
                {"endurance-sokker", 10087},
                {"tommy-hilfiger-undertoej", 10056},
                {"tommy-hilfiger-underbukser", 11475},
                {"traeningstoej", 10100},
                {"hugo-boss-traeningstoej", 10100},
                {"armani-troejer", 10054},
                {"endurance-traeningstoej", 10100},
                {"calvin-klein-traeningstoej", 10100},
                {"endurance-underbukser-boern", 11475},
                {"ralph-lauren-underbukser", 11475},
                {"ralph-lauren-t-shirts", 10056},
                {"ralph-lauren-undertroejer", 10056},
                {"solbriller", 9848},
                {"ray-ban", 9848},
                {"oakley", 9848},
                {"rejsetasker", 10096},
                {"bon-gout", 10498},
                {"tasker", 10498},
                {"computertasker", 10600},
                {"bon-gout-computertasker", 10600},
                {"nattoej", 9987},
                {"tommy-hilfiger-nattoej", 9987},
                {"hugo-boss-nattoej", 9987},
                {"armani-nattoej", 9987},
                {"undertoej-damer", 10056},
                {"haandtasker", 10093},
                {"bon-gout-haandtasker", 10093},
                {"armani-badetoej", 10778},
                {"comfyballs-underbukser", 11475},
                {"skuldertasker", 10596},
                {"bon-gout-skuldertasker", 10596},
                {"jbs-nattoej", 9987},
                {"jbs-traeningstoej", 10100},
                {"tilbehoer", 10100},
                {"poloer", 10001},
                {"tommy-hilfiger-troejer", 10054},
                {"jbs-troejer", 10054},
                {"bukser", 10004},
                {"jakker", 9999},
                {"jack-jones-underbukser", 11475},
                {"badekaaber", 11655},
                {"stroemper", 10087},
                {"stroempebukser", 10087},
                {"shorts", 10063},
                {"kopenhagen-shorts", 10063},
                {"decoy-stroemper", 10087},
                {"cruz-sandaler", 10041},
                {"sandaler", 10041},
                {"bh", 11470},
                {"tommyhilfiger-poloer", 10001},
                {"decoy-bh", 11470},
                {"endurance-traeningsudstyr", 10100},
                {"yogamaatte", 10100},
                {"redskaber", 10100},
                {"trusser", 11402},
                {"tops", 11471},
                {"decoy-trusser", 11402},
                {"decoy-tops", 11471},
                {"jbsbadekaaber", 11655},
                {"proactive-underbukser", 11475},
                {"jakker-2", 9999},
                {"endurance-jakker", 9999},
                {"hummel-jakker", 9999},
                {"gummistoevler", 10012},
                {"traeningsudstyr", 10100},
                {"hummel-loebetoej", 10620},
                {"hummel-traeningstoej", 10620},
                {"hummel-bukser", 10004},
                {"hummel-tops", 10511},
                {"weatherreport-jakker", 9999},
                {"jumpsuit", 9999},
                {"weatherreport-jumpsuit", 9999},
                {"resterods-undertroejer-2", 10056},
                {"lacoste-t-shirts", 10056},
                {"resterods-underbukser-2", 11475},
                {"lacoste-t-shirts-2", 10056},
                {"underbukser-herre", 11475},
                {"stroemper-2", 10087},
                {"decoy-stroemper-stroemper-2", 10087},
                {"tenson-jakker", 9999},
                {"superdry-troejer", 10054},
                {"superdry-t-shirts", 10056},
                {"superdry-poloer", 10001},
                {"superdry-shorts", 10063},
                {"superdry-sandaler-2", 10041},
                {"hugo-boss-underbukser-herre", 11475},
                {"hugo-boss-t-shirts-herre", 10056},
                {"hummel-t-shirts-herre", 10056},
                {"tops-2", 10511},
                {"endurance-skuldertasker", 10090},
                {"endurance-rejsetasker", 10096},
                {"mols-rejsetasker", 10096},
                {"endurance-haandtasker", 10093},
                {"mols-haandtasker", 10093},
                {"endurance-rygsaekke", 10091},
                {"mols-rygsaekke", 10091},
                {"endurance-baeltetasker", 10094},
                {"mols-baeltetasker", 10094},
                {"plejemidler", 10114},
                {"hugo-boss-undertroejer-2", 10056},
                {"hugo-boss-underbukser-2", 11475},
                {"hjemmesko", 10016},
                {"hummel-trusser", 11402},
            };
        }

        /// <inheritdoc />
        public ICollection<IProduct> FilterProducts(ICollection<IProduct> oldProducts)
        {
            logger.LogInformation(
                $"Filtering away Products without a Danish translation containing a valid {nameof(LocaleProduct.Type)} that can be translatable to a Product Style. Current count: {oldProducts.Count}");
            return oldProducts.Where(x =>
            {
                try
                {
                    ILocaleProduct? locale = x.Locales.FirstOrDefault(z => z.IsoName == Constants.DANISH_ISO_NAME);
                    if (locale == null)
                    {
                        logger.LogWarning(
                            $"Discarding {nameof(Product)} ({x.ReferenceId}), as no Danish Translation of products, for filtering by {nameof(LocaleProduct.Type)} was found.");
                        return false;
                    }

                    if (productStyleMap.Keys.Any(z =>
                            string.Equals(locale.Type, z, StringComparison.InvariantCultureIgnoreCase)))
                        return true;

                    logger.LogWarning($"Discarding {nameof(Product)} ({x.ReferenceId})," +
                                      $" as the Danish Localised {nameof(LocaleProduct.Type)} ({locale.Type}) is not convertible to a {nameof(Product)} Style number.");
                    return false;
                }
                catch (Exception e)
                {
                    logger.LogWarning(e,
                        $"Discarding {nameof(Product)} ({x.ReferenceId}), as some error occured during filtering by {nameof(LocaleProduct.Type)}.");
                    return false;
                }
            }).ToList();
        }

        /// <inheritdoc />
        public bool IsFilterOfType(FilterType filterType)
        {
            return filterType == FilterType.SPARTOO_TYPE;
        }

        /// <inheritdoc />
        public object GetDictionaryValue(object key)
        {
            if (key.GetType() != typeof(string))
                throw new ArgumentException("Expected key to be of type string");

            return productStyleMap.GetValueOrDefault((string) key);
        }

        /// <inheritdoc />
        public bool IsMapperOfType(MapType mapType)
        {
            return mapType == MapType.SPARTOO_TYPE;
        }
    }
}