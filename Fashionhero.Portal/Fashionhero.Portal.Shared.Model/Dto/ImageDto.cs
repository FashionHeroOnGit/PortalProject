using Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Dto;

namespace Fashionhero.Portal.Shared.Model.Dto
{
    public class ImageDto : IImageDto
    {
        /// <inheritdoc />
        public string Url { get; set; }
    }
}