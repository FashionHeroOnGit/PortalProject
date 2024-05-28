﻿using Fashionhero.Portal.Shared.Abstraction.Enums.Spartoo;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Entity;
using Microsoft.Extensions.Logging;

namespace Fashionhero.Portal.Shared.Abstraction.Interfaces.Spartoo
{
    public interface ISpartooFilter
    {
        ICollection<IProduct> FilterProducts(ICollection<IProduct> oldProducts, ILogger logger);

        object? GetDictionaryValue(string key);
        bool IsFilterOfType(FilterType filterType);
    }
}