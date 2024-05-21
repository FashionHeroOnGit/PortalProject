using Fashionhero.Portal.DataAccess;
using Fashionhero.Portal.DataAccess.Core;
using Fashionhero.Portal.DataAccess.Manager;
using Fashionhero.Portal.Presentation.Core;
using Fashionhero.Portal.Shared.Model.Entity;
using Fashionhero.Portal.Shared.Model.Searchable;
using Microsoft.EntityFrameworkCore;

namespace Fashionhero.Portal.Presentation
{
    public class Startup : ModularStartup
    {
        private const string DATABASE_CONNECTION_STRING_NAME = "mainDb";
        private const string LOG_FILE = "Storage/portal.log";

        public Startup(IConfiguration config) : base(config)
        {
            AddModule(new LoggingStartupModule(LOG_FILE));

            AddModule(new ApiStartupModule());
            AddModule(new SwaggerStartupModule("Portal"));

            AddModule(new DatabaseContextStartupModule<PortalDatabaseContext>(options =>
            {
                options.UseSqlite(Configuration.GetConnectionString(DATABASE_CONNECTION_STRING_NAME),
                    o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
#if DEBUG
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
#endif
            }));

            AddModule(new EntityQueryManagerStartupModule<ImageQueryManager, Image, SearchableImage>());
            AddModule(
                new EntityQueryManagerStartupModule<LocaleProductQueryManager, LocaleProduct,
                    SearchableLocaleProduct>());
            AddModule(new EntityQueryManagerStartupModule<PriceQueryManager, Price, SearchablePrice>());
            AddModule(new EntityQueryManagerStartupModule<ProductQueryManager, Product, SearchableProduct>());
            AddModule(new EntityQueryManagerStartupModule<SizeQueryManager, Size, SearchableSize>());
            AddModule(new EntityQueryManagerStartupModule<TagQueryManager, Tag, SearchableTag>());

            AddModule(new PortalStartupModule());
        }
    }
}