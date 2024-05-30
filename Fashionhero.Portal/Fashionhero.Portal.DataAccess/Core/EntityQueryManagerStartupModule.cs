using Fashionhero.Portal.Shared.Abstraction.Interfaces.Persistence;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Startup;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Fashionhero.Portal.DataAccess.Core
{
    public class EntityQueryManagerStartupModule<TQuery, TEntity, TSearchable, TDto> : IStartupModule
        where TQuery : class, IEntityQueryManager<TEntity, TSearchable, TDto>
        where TEntity : class, IEntity
        where TSearchable : class, ISearchable
        where TDto : class, IDto
    {
        private ILogger<EntityQueryManagerStartupModule<TQuery, TEntity, TSearchable, TDto>>? logger;


        /// <inheritdoc />
        public void ConfigureServices(IServiceCollection services)
        {
            logger = services.BuildServiceProvider()
                .GetService<ILogger<EntityQueryManagerStartupModule<TQuery, TEntity, TSearchable, TDto>>>();

            services.AddScoped<IEntityQueryManager<TEntity, TSearchable, TDto>, TQuery>();

            logger?.LogDebug("Completed Configuration of Services.");
        }

        /// <inheritdoc />
        public void ConfigureApplication(IApplicationBuilder app)
        {
        }
    }
}