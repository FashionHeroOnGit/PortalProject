using Fashionhero.Portal.Shared.Abstraction.Interfaces.Model.Dto;

namespace Fashionhero.Portal.Shared.Model.Dto
{
    public class SizeDto : ISizeDto
    {
        /// <inheritdoc />
        public int Quantity { get; set; }

        /// <inheritdoc />
        public long Ean { get; set; }

        /// <inheritdoc />
        public string Primary { get; set; }

        /// <inheritdoc />
        public string? Secondary { get; set; }

        /// <inheritdoc />
        public string LinkPostFix { get; set; }

        /// <inheritdoc />
        public string LinkBase { get; set; }

        /// <inheritdoc />
        public string ModelProductNumber { get; set; }

        /// <inheritdoc />
        public int ReferenceId { get; set; }


        /// <inheritdoc />
        public bool IsStocked => Quantity != 0;
    }
}