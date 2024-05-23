using Fashionhero.Portal.Shared.Abstraction.Interfaces.Persistence;
using Fashionhero.Portal.Shared.Abstraction.Interfaces.Startup;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Fashionhero.Portal.DataAccess.Core
{
    public class EntityQueryManagerStartupModule<TQuery, TEntity, TSearchable, TDto> : IStartupModule
        where TQuery : class, IEntityQueryManager<TEntity, TSearchable, TDto>
        where TEntity : class, IEntity
        where TSearchable : class, ISearchable
        where TDto : class, IDto
    {
        /// <inheritdoc />
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IEntityQueryManager<TEntity, TSearchable, TDto>, TQuery>();
        }

        /// <inheritdoc />
        public void ConfigureApplication(IApplicationBuilder app)
        {
        }
    }
}