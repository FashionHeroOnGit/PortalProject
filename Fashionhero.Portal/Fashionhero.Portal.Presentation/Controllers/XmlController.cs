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
        public async Task<IActionResult> UpdateInventory([FromBody] UpdateInventoryArguments args)
        {
            try
            {
                await loader.UpdateInventory(args.LanguagePaths, args.InventoryPath);
                return Ok();
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Error occured during execution of {nameof(UpdateInventory)}.");
                throw;
            }
        }

        public class UpdateInventoryArguments
        {
            public string[] LanguagePaths { get; set; }
            public string InventoryPath { get; set; }
        }
    }
}