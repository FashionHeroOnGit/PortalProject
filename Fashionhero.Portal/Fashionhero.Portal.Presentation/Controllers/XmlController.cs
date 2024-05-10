using Fashionhero.Portal.Shared.Abstraction.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;

namespace Fashionhero.Portal.Presentation.Controllers
{
    [Route(Constants.ROUTE_TEMPLATE)]
    [ApiController]
    public class XmlController : ControllerBase
    {
        private readonly ILogger<XmlController> logger;
        private readonly IXmlLoaderService loader;

        public XmlController(ILogger<XmlController> logger, IXmlLoaderService loader)
        {
            this.logger = logger;
            this.loader = loader;
        }

        [HttpPost]
        public async Task<IActionResult> GenerateInventory([FromBody] GenerateArguments args)
        {
            try
            {
                await loader.GenerateInventory(args.LanguagePaths, args.InventoryPath);
                return Ok();
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Error occured during execution of {nameof(GenerateInventory)}.");
                throw;
            }
        }

        public class GenerateArguments
        {
            public string[] LanguagePaths { get; set; }
            public string InventoryPath { get; set; }
        }
    }
}