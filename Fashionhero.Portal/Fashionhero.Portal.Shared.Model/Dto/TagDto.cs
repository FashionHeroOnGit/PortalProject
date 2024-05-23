using Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Dto;

namespace Fashionhero.Portal.Shared.Model.Dto
{
    public class TagDto : ITagDto
    {
        /// <inheritdoc />
        public string Name { get; set; }

        /// <inheritdoc />
        public string Value { get; set; }
    }
}