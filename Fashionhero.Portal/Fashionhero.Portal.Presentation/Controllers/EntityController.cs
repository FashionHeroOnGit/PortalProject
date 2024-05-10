using Fashionhero.Portal.Shared.Abstraction.Interfaces.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace Fashionhero.Portal.Presentation.Controllers
{
    public abstract class EntityController<TEntity, TSearchable, TDto> : ControllerBase where TEntity : class, IEntity
        where TSearchable : class, ISearchable, new()
        where TDto : class, IDto
    {
        private readonly IEntityQueryManager<TEntity, TSearchable, TDto> manager;

        protected EntityController(IEntityQueryManager<TEntity, TSearchable, TDto> manager)
        {
            this.manager = manager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var entities = await manager.GetEntities(new TSearchable());
                return Ok(entities);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                IEntity payment = await manager.GetEntity(new TSearchable {Id = id,});
                return Ok(payment);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetByQuery([FromBody] TSearchable searchable)
        {
            try
            {
                IEntity payment = await manager.GetEntity(searchable);
                return Ok(payment);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddSingle([FromBody] TDto entity)
        {
            try
            {
                TEntity newPayment = await manager.AddEntity(entity);
                return Ok(newPayment);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddMultiple([FromBody] IEnumerable<TDto> entities)
        {
            try
            {
                var newEntities = await manager.AddEntities(entities);
                return Ok(newEntities);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateSingle([FromBody] TEntity entity)
        {
            try
            {
                TEntity updateEntity = await manager.UpdateEntity(entity);
                return Ok(updateEntity);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateMultiple([FromBody] IEnumerable<TEntity> entities)
        {
            try
            {
                var updateEntities = await manager.UpdateEntities(entities);
                return Ok(updateEntities);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteByQuery([FromBody] TSearchable searchable)
        {
            try
            {
                await manager.DeleteEntity(searchable);
                return Ok();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [HttpDelete("id")]
        public async Task<IActionResult> DeleteById(int id)
        {
            try
            {
                await manager.DeleteEntityById(id);
                return Ok();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}