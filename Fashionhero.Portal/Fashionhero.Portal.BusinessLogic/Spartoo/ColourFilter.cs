using Fashionhero.Portal.Shared.Abstraction.Enums.Spartoo;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Entity;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Spartoo;
using Microsoft.Extensions.Logging;

namespace Fashionhero.Portal.BusinessLogic.Spartoo
{
    public class ColourFilter : ISpartooFilter
    {
        private readonly Dictionary<string, int> colourIdMap;

        public ColourFilter()
        {
            colourIdMap = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase)
            {
                {"sort", 38},
                {"hvid", 1},
                {"blå/sort", 594},
                {"sort/guld rem", 594},
                {"blå", 19},
                {"sort/grøn/blå", 594},
                {"rød/sort", 594},
                {"lyseblå", 19},
                {"grøn", 25},
                {"sort/farvet rem", 594},
                {"sort/grå/hvid", 594},
                {"sort/rød/blå", 594},
                {"hvid/grå/sort", 594},
                {"sort/blå/grøn", 594},
                {"sort/blå/hvid", 594},
                {"flerfarvet", 594},
                {"mblå", 19},
                {"gråblå", 19},
                {"fler-farvetblå", 19},
                {"gul", 4},
                {"orange", 7},
                {"rød", 8},
                {"brun", 28},
                {"grå", 35},
                {"guld", 41},
                {"bordeaux", 7136},
            };
        }

        /// <inheritdoc />
        public ICollection<IProduct> FilterProducts(ICollection<IProduct> oldProducts, ILogger logger)
        {
            return oldProducts;
        }

        /// <inheritdoc />
        public object? GetDictionaryValue(string key)
        {
            bool success = colourIdMap.TryGetValue(key, out int result);
            return success ? result : 534;
        }

        /// <inheritdoc />
        public bool IsFilterOfType(FilterType filterType)
        {
            return filterType == FilterType.COLOUR;
        }
    }
}